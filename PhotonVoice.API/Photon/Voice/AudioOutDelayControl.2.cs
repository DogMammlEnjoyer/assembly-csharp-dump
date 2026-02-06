using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
	public abstract class AudioOutDelayControl<T> : AudioOutDelayControl, IAudioOut<T>
	{
		public abstract int OutPos { get; }

		public abstract void OutCreate(int frequency, int channels, int bufferSamples);

		public abstract void OutStart();

		public abstract void OutWrite(T[] data, int offsetSamples);

		public AudioOutDelayControl(bool processInService, AudioOutDelayControl.PlayDelayConfig playDelayConfig, ILogger logger, string logPrefix, bool debugInfo)
		{
			this.processInService = processInService;
			this.playDelayConfig = playDelayConfig.Clone();
			this.logger = logger;
			this.logPrefix = logPrefix;
			this.debugInfo = debugInfo;
		}

		public int Lag
		{
			get
			{
				return (int)(((float)this.clipWriteSamplePos - (this.started ? ((float)this.playLoopCount * (float)this.bufferSamples + (float)this.OutPos) : 0f)) * 1000f / (float)this.frequency);
			}
		}

		public bool IsFlushed
		{
			get
			{
				return !this.started || this.flushed;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return !this.IsFlushed && Environment.TickCount - this.lastPushTime < 100;
			}
		}

		public void Start(int frequency, int channels, int frameSamples)
		{
			this.frequency = frequency;
			this.channels = channels;
			this.targetDelaySamples = this.playDelayConfig.Low * frequency / 1000 + frameSamples;
			this.upperTargetDelaySamples = this.playDelayConfig.High * frequency / 1000 + frameSamples;
			if (this.upperTargetDelaySamples < this.targetDelaySamples + 2 * frameSamples)
			{
				this.upperTargetDelaySamples = this.targetDelaySamples + 2 * frameSamples;
			}
			int max = this.playDelayConfig.Max;
			this.maxDelaySamples = this.playDelayConfig.Max * frequency / 1000;
			if (this.maxDelaySamples < this.upperTargetDelaySamples)
			{
				this.maxDelaySamples = this.upperTargetDelaySamples;
			}
			this.bufferSamples = 3 * this.maxDelaySamples;
			this.frameSamples = frameSamples;
			this.frameSize = frameSamples * channels;
			this.clipWriteSamplePos = this.targetDelaySamples;
			if (this.framePool.Info != this.frameSize)
			{
				this.framePool.Init(this.frameSize);
			}
			this.zeroFrame = new T[this.frameSize];
			this.resampledFrame = new T[this.frameSize];
			this.tempoChangeHQ = false;
			if (!this.tempoChangeHQ)
			{
				this.tempoUp = new AudioUtil.TempoUp<T>();
			}
			this.OutCreate(frequency, channels, this.bufferSamples);
			this.OutStart();
			this.started = true;
			this.logger.LogInfo("{0} Start: {1} bs={2} ch={3} f={4} tds={5} utds={6} mds={7} speed={8} tempo={9}", new object[]
			{
				this.logPrefix,
				(this.sizeofT == 2) ? "short" : "float",
				this.bufferSamples,
				channels,
				frequency,
				this.targetDelaySamples,
				this.upperTargetDelaySamples,
				this.maxDelaySamples,
				this.playDelayConfig.SpeedUpPerc,
				this.tempoChangeHQ ? "HQ" : "LQ"
			});
		}

		private bool processFrame(T[] frame, int playSamplePos)
		{
			int num = this.clipWriteSamplePos - playSamplePos;
			if (!this.flushed)
			{
				if (num > this.maxDelaySamples)
				{
					if (this.debugInfo)
					{
						this.logger.LogDebug("{0} overrun {1} {2} {3} {4} {5}", new object[]
						{
							this.logPrefix,
							this.upperTargetDelaySamples,
							num,
							playSamplePos,
							this.clipWriteSamplePos,
							playSamplePos + this.targetDelaySamples
						});
					}
					this.clipWriteSamplePos = playSamplePos + this.maxDelaySamples;
					num = this.maxDelaySamples;
				}
				else if (num < 0)
				{
					if (this.debugInfo)
					{
						this.logger.LogDebug("{0} underrun {1} {2} {3} {4} {5}", new object[]
						{
							this.logPrefix,
							this.upperTargetDelaySamples,
							num,
							playSamplePos,
							this.clipWriteSamplePos,
							playSamplePos + this.targetDelaySamples
						});
					}
					this.clipWriteSamplePos = playSamplePos + this.targetDelaySamples;
					num = this.targetDelaySamples;
				}
			}
			if (frame == null)
			{
				this.flushed = true;
				if (this.debugInfo)
				{
					this.logger.LogDebug("{0} stream flush pause {1} {2} {3} {4} {5}", new object[]
					{
						this.logPrefix,
						this.upperTargetDelaySamples,
						num,
						playSamplePos,
						this.clipWriteSamplePos,
						playSamplePos + this.targetDelaySamples
					});
				}
				if (this.catchingUp)
				{
					this.catchingUp = false;
					if (this.debugInfo)
					{
						this.logger.LogDebug("{0} stream sync reset {1} {2} {3} {4} {5}", new object[]
						{
							this.logPrefix,
							this.upperTargetDelaySamples,
							num,
							playSamplePos,
							this.clipWriteSamplePos,
							playSamplePos + this.targetDelaySamples
						});
					}
				}
				return true;
			}
			if (this.flushed)
			{
				this.clipWriteSamplePos = playSamplePos + this.targetDelaySamples;
				num = this.targetDelaySamples;
				this.flushed = false;
				if (this.debugInfo)
				{
					this.logger.LogDebug("{0} stream unpause {1} {2} {3} {4} {5}", new object[]
					{
						this.logPrefix,
						this.upperTargetDelaySamples,
						num,
						playSamplePos,
						this.clipWriteSamplePos,
						playSamplePos + this.targetDelaySamples
					});
				}
			}
			if (num > this.upperTargetDelaySamples && !this.catchingUp)
			{
				if (!this.tempoChangeHQ)
				{
					this.tempoUp.Begin(this.channels, this.playDelayConfig.SpeedUpPerc, 6);
				}
				this.catchingUp = true;
				if (this.debugInfo)
				{
					this.logger.LogDebug("{0} stream sync started {1} {2} {3} {4} {5}", new object[]
					{
						this.logPrefix,
						this.upperTargetDelaySamples,
						num,
						playSamplePos,
						this.clipWriteSamplePos,
						playSamplePos + this.targetDelaySamples
					});
				}
			}
			bool flag = false;
			if (num <= this.targetDelaySamples && this.catchingUp)
			{
				if (!this.tempoChangeHQ)
				{
					int num2 = this.tempoUp.End(frame);
					int num3 = frame.Length / this.channels - num2;
					Buffer.BlockCopy(frame, num2 * this.channels * this.sizeofT, this.resampledFrame, 0, num3 * this.channels * this.sizeofT);
					this.writeResampled(this.resampledFrame, num3);
					flag = true;
				}
				this.catchingUp = false;
				if (this.debugInfo)
				{
					this.logger.LogDebug("{0} stream sync finished {1} {2} {3} {4} {5}", new object[]
					{
						this.logPrefix,
						this.upperTargetDelaySamples,
						num,
						playSamplePos,
						this.clipWriteSamplePos,
						playSamplePos + this.targetDelaySamples
					});
				}
			}
			if (flag)
			{
				return false;
			}
			if (this.catchingUp)
			{
				if (!this.tempoChangeHQ)
				{
					int resampledLenSamples = this.tempoUp.Process(frame, this.resampledFrame);
					this.writeResampled(this.resampledFrame, resampledLenSamples);
				}
			}
			else
			{
				this.OutWrite(frame, this.clipWriteSamplePos % this.bufferSamples);
				this.clipWriteSamplePos += frame.Length / this.channels;
			}
			return false;
		}

		public void Service()
		{
			if (this.started)
			{
				int outPos = this.OutPos;
				if (outPos < this.sourceTimeSamplesPrev)
				{
					this.playLoopCount++;
				}
				this.sourceTimeSamplesPrev = outPos;
				int num = this.playLoopCount * this.bufferSamples + outPos;
				if (this.processInService)
				{
					Queue<T[]> obj = this.frameQueue;
					lock (obj)
					{
						while (this.frameQueue.Count > 0)
						{
							T[] array = this.frameQueue.Dequeue();
							if (this.processFrame(array, num))
							{
								return;
							}
							this.framePool.Release(array, array.Length);
						}
					}
				}
				int num2 = this.playSamplePosPrev;
				int num3 = num - this.bufferSamples;
				if (num2 < num3)
				{
					num2 = num3;
				}
				int num4 = (num - num2 - 1) / this.frameSamples + 1;
				for (int i = num - num4 * this.frameSamples; i < num; i += this.frameSamples)
				{
					int num5 = i % this.bufferSamples;
					if (num5 < 0)
					{
						num5 += this.bufferSamples;
					}
					this.OutWrite(this.zeroFrame, num5);
				}
				this.playSamplePosPrev = num;
			}
		}

		private int writeResampled(T[] f, int resampledLenSamples)
		{
			int num = (f.Length - resampledLenSamples * this.channels) * this.sizeofT;
			if (num > 0)
			{
				Buffer.BlockCopy(this.zeroFrame, 0, f, resampledLenSamples * this.channels * this.sizeofT, num);
			}
			this.OutWrite(f, this.clipWriteSamplePos % this.bufferSamples);
			this.clipWriteSamplePos += resampledLenSamples;
			return resampledLenSamples;
		}

		public void Push(T[] frame)
		{
			if (!this.started)
			{
				return;
			}
			if (frame.Length == 0)
			{
				return;
			}
			if (frame.Length != this.frameSize)
			{
				this.logger.LogError("{0} audio frames are not of size: {1} != {2}", new object[]
				{
					this.logPrefix,
					frame.Length,
					this.frameSize
				});
				return;
			}
			if (this.processInService)
			{
				T[] array = this.framePool.AcquireOrCreate();
				Buffer.BlockCopy(frame, 0, array, 0, frame.Length * this.sizeofT);
				Queue<T[]> obj = this.frameQueue;
				lock (obj)
				{
					this.frameQueue.Enqueue(array);
					goto IL_BE;
				}
			}
			this.processFrame(frame, this.playLoopCount * this.bufferSamples + this.OutPos);
			IL_BE:
			this.lastPushTime = Environment.TickCount;
		}

		public void Flush()
		{
			if (this.processInService)
			{
				Queue<T[]> obj = this.frameQueue;
				lock (obj)
				{
					this.frameQueue.Enqueue(null);
					return;
				}
			}
			this.processFrame(null, this.playLoopCount * this.bufferSamples + this.OutPos);
		}

		public virtual void Stop()
		{
			this.started = false;
		}

		public virtual void ToggleAudioSource(bool toggle)
		{
		}

		private readonly int sizeofT = Marshal.SizeOf<T>(default(T));

		private const int TEMPO_UP_SKIP_GROUP = 6;

		private int frameSamples;

		private int frameSize;

		protected int bufferSamples;

		protected int frequency;

		private int clipWriteSamplePos;

		private int playSamplePosPrev;

		private int sourceTimeSamplesPrev;

		private int playLoopCount;

		private AudioOutDelayControl.PlayDelayConfig playDelayConfig;

		protected int channels;

		private bool started;

		private bool flushed = true;

		private int targetDelaySamples;

		private int upperTargetDelaySamples;

		private int maxDelaySamples;

		private const int NO_PUSH_TIMEOUT_MS = 100;

		private int lastPushTime = Environment.TickCount - 100;

		protected readonly ILogger logger;

		protected readonly string logPrefix;

		private readonly bool debugInfo;

		private readonly bool processInService;

		private T[] zeroFrame;

		private T[] resampledFrame;

		private AudioUtil.TempoUp<T> tempoUp;

		private bool tempoChangeHQ;

		private Queue<T[]> frameQueue = new Queue<T[]>();

		public const int FRAME_POOL_CAPACITY = 50;

		private PrimitiveArrayPool<T> framePool = new PrimitiveArrayPool<T>(50, "AudioOutDelayControl");

		private bool catchingUp;
	}
}
