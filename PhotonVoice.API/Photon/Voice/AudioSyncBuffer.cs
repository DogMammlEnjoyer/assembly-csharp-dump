using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
	public class AudioSyncBuffer<T> : IAudioOut<T>
	{
		public AudioSyncBuffer(int playDelayMs, ILogger logger, string logPrefix, bool debugInfo)
		{
			this.playDelayMs = playDelayMs;
			this.logger = logger;
			this.logPrefix = logPrefix;
			this.debugInfo = debugInfo;
		}

		public int Lag
		{
			get
			{
				int result;
				lock (this)
				{
					result = (int)((float)this.frameQueue.Count * (float)this.frameSamples * 1000f / (float)this.sampleRate);
				}
				return result;
			}
		}

		public bool IsPlaying
		{
			get
			{
				bool result;
				lock (this)
				{
					result = this.started;
				}
				return result;
			}
		}

		public void Start(int sampleRate, int channels, int frameSamples)
		{
			lock (this)
			{
				this.started = false;
				this.sampleRate = sampleRate;
				this.channels = channels;
				this.frameSamples = frameSamples;
				this.frameSize = frameSamples * channels;
				int num = this.playDelayMs * sampleRate / 1000 + frameSamples;
				this.maxDevPlayDelaySamples = num / 2;
				this.targetPlayDelaySamples = num + this.maxDevPlayDelaySamples;
				if (this.framePool.Info != this.frameSize)
				{
					this.framePool.Init(this.frameSize);
				}
				while (this.frameQueue.Count > 0)
				{
					this.dequeueFrameQueue();
				}
				this.emptyFrame = new T[this.frameSize];
				int num2 = this.targetPlayDelaySamples / this.frameSamples;
				this.curPlayingFrameSamplePos = this.targetPlayDelaySamples % this.frameSamples;
				while (this.frameQueue.Count < num2)
				{
					this.frameQueue.Enqueue(this.emptyFrame);
				}
				this.started = true;
			}
		}

		public void Service()
		{
		}

		public void Read(T[] outBuf, int outChannels, int outSampleRate)
		{
			lock (this)
			{
				if (this.started)
				{
					int num = 0;
					while ((this.frameQueue.Count * this.frameSamples - this.curPlayingFrameSamplePos) * this.channels * outSampleRate >= (outBuf.Length - num) * this.sampleRate)
					{
						int num2 = this.curPlayingFrameSamplePos * this.channels;
						T[] array = this.frameQueue.Peek();
						int num3 = outBuf.Length - num;
						int num4 = array.Length - num2;
						if (num4 * outChannels * outSampleRate > num3 * this.channels * this.sampleRate)
						{
							int num5 = num3 * this.channels * this.sampleRate / (outChannels * outSampleRate);
							if (this.sampleRate == outSampleRate && this.channels == outChannels)
							{
								Buffer.BlockCopy(array, num2 * this.elementSize, outBuf, num * this.elementSize, num3 * this.elementSize);
							}
							else
							{
								AudioUtil.Resample<T>(array, num2, num5, this.channels, outBuf, num, num3, outChannels);
							}
							this.curPlayingFrameSamplePos += num5 / this.channels;
							break;
						}
						int num6 = num4 * outChannels * outSampleRate / (this.channels * this.sampleRate);
						if (this.sampleRate == outSampleRate && this.channels == outChannels)
						{
							Buffer.BlockCopy(array, num2 * this.elementSize, outBuf, num * this.elementSize, num4 * this.elementSize);
						}
						else
						{
							AudioUtil.Resample<T>(array, num2, num4, this.channels, outBuf, num, num6, outChannels);
						}
						num += num6;
						this.curPlayingFrameSamplePos = 0;
						this.dequeueFrameQueue();
						if (num6 == num3)
						{
							break;
						}
					}
				}
			}
		}

		public void Push(T[] frame)
		{
			lock (this)
			{
				if (this.started)
				{
					if (frame.Length != 0)
					{
						if (frame.Length != this.frameSize)
						{
							this.logger.LogError("{0} AudioSyncBuffer audio frames are not of size: {1} != {2}", new object[]
							{
								this.logPrefix,
								frame.Length,
								this.frameSize
							});
						}
						else
						{
							if (this.framePool.Info != frame.Length)
							{
								this.framePool.Init(frame.Length);
							}
							T[] array = this.framePool.AcquireOrCreate();
							Buffer.BlockCopy(frame, 0, array, 0, Buffer.ByteLength(frame));
							lock (this)
							{
								this.frameQueue.Enqueue(array);
								this.syncFrameQueue();
							}
						}
					}
				}
			}
		}

		public void Flush()
		{
		}

		public void Stop()
		{
			lock (this)
			{
				this.started = false;
			}
		}

		private void dequeueFrameQueue()
		{
			T[] array = this.frameQueue.Dequeue();
			if (array != this.emptyFrame)
			{
				this.framePool.Release(array, array.Length);
			}
		}

		private void syncFrameQueue()
		{
			int num = this.frameQueue.Count * this.frameSamples - this.curPlayingFrameSamplePos;
			if (num > this.targetPlayDelaySamples + this.maxDevPlayDelaySamples)
			{
				int num2 = this.targetPlayDelaySamples / this.frameSamples;
				this.curPlayingFrameSamplePos = this.targetPlayDelaySamples % this.frameSamples;
				while (this.frameQueue.Count > num2)
				{
					this.dequeueFrameQueue();
				}
				if (this.debugInfo)
				{
					this.logger.LogWarning("{0} AudioSynctBuffer overrun {1} {2} {3} {4}", new object[]
					{
						this.logPrefix,
						this.targetPlayDelaySamples - this.maxDevPlayDelaySamples,
						this.targetPlayDelaySamples + this.maxDevPlayDelaySamples,
						num,
						num2,
						this.curPlayingFrameSamplePos
					});
					return;
				}
			}
			else if (num < this.targetPlayDelaySamples - this.maxDevPlayDelaySamples)
			{
				int num3 = this.targetPlayDelaySamples / this.frameSamples;
				this.curPlayingFrameSamplePos = this.targetPlayDelaySamples % this.frameSamples;
				while (this.frameQueue.Count < num3)
				{
					this.frameQueue.Enqueue(this.emptyFrame);
				}
				if (this.debugInfo)
				{
					this.logger.LogWarning("{0} AudioSyncBuffer underrun {1} {2} {3} {4}", new object[]
					{
						this.logPrefix,
						this.targetPlayDelaySamples - this.maxDevPlayDelaySamples,
						this.targetPlayDelaySamples + this.maxDevPlayDelaySamples,
						num,
						num3,
						this.curPlayingFrameSamplePos
					});
				}
			}
		}

		public virtual void ToggleAudioSource(bool toggle)
		{
		}

		private int curPlayingFrameSamplePos;

		private int sampleRate;

		private int channels;

		private int frameSamples;

		private int frameSize;

		private bool started;

		private int maxDevPlayDelaySamples;

		private int targetPlayDelaySamples;

		private int playDelayMs;

		private readonly ILogger logger;

		private readonly string logPrefix;

		private readonly bool debugInfo;

		private readonly int elementSize = Marshal.SizeOf(typeof(T));

		private T[] emptyFrame;

		private Queue<T[]> frameQueue = new Queue<T[]>();

		public const int FRAME_POOL_CAPACITY = 50;

		private PrimitiveArrayPool<T> framePool = new PrimitiveArrayPool<T>(50, "AudioSyncBuffer");
	}
}
