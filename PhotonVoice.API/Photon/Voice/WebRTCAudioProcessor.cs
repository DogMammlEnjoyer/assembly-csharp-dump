using System;
using System.Collections.Generic;
using System.Threading;

namespace Photon.Voice
{
	public class WebRTCAudioProcessor : WebRTCAudioLib, IProcessor<short>, IDisposable
	{
		public int AECStreamDelayMs
		{
			set
			{
				if (this.reverseStreamDelayMs != value)
				{
					this.reverseStreamDelayMs = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.REVERSE_STREAM_DELAY_MS, value);
					}
				}
			}
		}

		public bool AEC
		{
			set
			{
				if (this.aec != value)
				{
					this.aec = value;
					this.InitReverseStream();
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AEC, this.aec ? 1 : 0);
					}
					this.aecm = (!this.aec && this.aecm);
				}
			}
		}

		public bool AECHighPass
		{
			set
			{
				if (this.aecHighPass != value)
				{
					this.aecHighPass = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AEC_HIGH_PASS_FILTER, value ? 1 : 0);
					}
				}
			}
		}

		public bool AECMobile
		{
			set
			{
				if (this.aecm != value)
				{
					this.aecm = value;
					this.InitReverseStream();
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AECM, this.aecm ? 1 : 0);
					}
					this.aec = (!this.aecm && this.aec);
				}
			}
		}

		public bool HighPass
		{
			set
			{
				if (this.highPass != value)
				{
					this.highPass = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.HIGH_PASS_FILTER, value ? 1 : 0);
					}
				}
			}
		}

		public bool NoiseSuppression
		{
			set
			{
				if (this.ns != value)
				{
					this.ns = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.NS, value ? 1 : 0);
					}
				}
			}
		}

		public bool AGC
		{
			set
			{
				if (this.agc != value)
				{
					this.agc = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AGC, value ? 1 : 0);
					}
				}
			}
		}

		public int AGCCompressionGain
		{
			set
			{
				if (this.agcCompressionGain != value)
				{
					if (value < 0 || value > 90)
					{
						this.logger.LogError("[PV] WebRTCAudioProcessor: new AGCCompressionGain value {0} not in range [0..90]", new object[]
						{
							value
						});
						return;
					}
					this.agcCompressionGain = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AGC_COMPRESSION_GAIN, value);
					}
				}
			}
		}

		public int AGCTargetLevel
		{
			set
			{
				if (this.agcTargetLevel != value)
				{
					if (value > 31 || value < 0)
					{
						this.logger.LogError("[PV] WebRTCAudioProcessor: new AGCTargetLevel value {0} not in range [0..31]", new object[]
						{
							value
						});
						return;
					}
					this.agcTargetLevel = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AGC_TARGET_LEVEL_DBFS, value);
					}
				}
			}
		}

		public bool AGC2
		{
			set
			{
				if (this.agc2 != value)
				{
					this.agc2 = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.AGC2, value ? 1 : 0);
					}
				}
			}
		}

		public bool VAD
		{
			set
			{
				if (this.vad != value)
				{
					this.vad = value;
					if (this.proc != IntPtr.Zero)
					{
						this.setParam(WebRTCAudioLib.Param.VAD, value ? 1 : 0);
					}
				}
			}
		}

		public bool Bypass
		{
			private get
			{
				return this.bypass;
			}
			set
			{
				if (this.bypass != value)
				{
					this.logger.LogInfo("[PV] WebRTCAudioProcessor: setting bypass=" + value.ToString(), Array.Empty<object>());
				}
				this.bypass = value;
			}
		}

		public WebRTCAudioProcessor(ILogger logger, int frameSize, int samplingRate, int channels, int reverseSamplingRate, int reverseChannels)
		{
			bool flag = false;
			foreach (int num in WebRTCAudioProcessor.SupportedSamplingRates)
			{
				if (samplingRate == num)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				logger.LogError("[PV] WebRTCAudioProcessor: input sampling rate ({0}) must be 8000, 16000, 32000 or 48000", new object[]
				{
					samplingRate
				});
				this.disposed = true;
				return;
			}
			this.logger = logger;
			this.inFrameSize = frameSize;
			this.processFrameSize = samplingRate * 10 / 1000;
			if (this.inFrameSize / this.processFrameSize * this.processFrameSize != this.inFrameSize)
			{
				logger.LogError("[PV] WebRTCAudioProcessor: input frame size ({0} samples / {1} ms) must be equal to or N times more than webrtc processing frame size ({2} samples / 10 ms)", new object[]
				{
					this.inFrameSize,
					1000f * (float)this.inFrameSize / (float)samplingRate,
					this.processFrameSize
				});
				this.disposed = true;
				return;
			}
			this.samplingRate = samplingRate;
			this.channels = channels;
			this.reverseSamplingRate = reverseSamplingRate;
			this.reverseChannels = reverseChannels;
			this.proc = WebRTCAudioLib.webrtc_audio_processor_create(samplingRate, channels, this.processFrameSize, samplingRate, reverseChannels);
			WebRTCAudioLib.webrtc_audio_processor_init(this.proc);
			logger.LogInfo("[PV] WebRTCAudioProcessor create sampling rate {0}, channels{1}, frame size {2}, frame samples {3}, reverseChannels {4}", new object[]
			{
				samplingRate,
				channels,
				this.processFrameSize,
				this.inFrameSize / this.channels,
				this.reverseChannels
			});
		}

		private void InitReverseStream()
		{
			lock (this)
			{
				if (!this.aecInited)
				{
					if (!this.disposed)
					{
						int num = this.processFrameSize * this.reverseSamplingRate / this.samplingRate * this.reverseChannels;
						this.reverseFramer = new Framer<float>(num);
						this.reverseBufferFactory = new FactoryPrimitiveArrayPool<short>(50, "WebRTCAudioProcessor Reverse Buffers", this.inFrameSize);
						this.logger.LogInfo("[PV] WebRTCAudioProcessor Init reverse stream: frame size {0}, reverseSamplingRate {1}, reverseChannels {2}", new object[]
						{
							num,
							this.reverseSamplingRate,
							this.reverseChannels
						});
						if (!this.reverseStreamThreadRunning)
						{
							Thread thread = new Thread(new ThreadStart(this.ReverseStreamThread));
							thread.Start();
							Util.SetThreadName(thread, "[PV] WebRTCProcRevStream");
						}
						if (this.reverseSamplingRate != this.samplingRate)
						{
							this.logger.LogWarning("[PV] WebRTCAudioProcessor AEC: output sampling rate {0} != {1} capture sampling rate. For better AEC, set audio source (microphone) and audio output samping rates to the same value.", new object[]
							{
								this.reverseSamplingRate,
								this.samplingRate
							});
						}
						this.aecInited = true;
					}
				}
			}
		}

		public short[] Process(short[] buf)
		{
			if (this.Bypass)
			{
				return buf;
			}
			if (this.disposed)
			{
				return buf;
			}
			if (this.proc == IntPtr.Zero)
			{
				return buf;
			}
			if (buf.Length != this.inFrameSize)
			{
				this.logger.LogError("[PV] WebRTCAudioProcessor Process: frame size expected: {0}, passed: {1}", new object[]
				{
					this.inFrameSize,
					buf
				});
				return buf;
			}
			bool flag = false;
			for (int i = 0; i < this.inFrameSize; i += this.processFrameSize)
			{
				bool flag2 = true;
				int num = WebRTCAudioLib.webrtc_audio_processor_process(this.proc, buf, i, out flag2);
				if (flag2)
				{
					flag = true;
				}
				if (this.lastProcessErr != num)
				{
					this.lastProcessErr = num;
					this.logger.LogError("[PV] WebRTCAudioProcessor Process: webrtc_audio_processor_process() error {0}", new object[]
					{
						num
					});
					return buf;
				}
			}
			if (this.vad && !flag)
			{
				return null;
			}
			return buf;
		}

		public void OnAudioOutFrameFloat(float[] data)
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.aecInited)
			{
				return;
			}
			if (this.proc == IntPtr.Zero)
			{
				return;
			}
			foreach (float[] array in this.reverseFramer.Frame(data))
			{
				short[] array2 = this.reverseBufferFactory.New();
				if (array.Length != array2.Length)
				{
					AudioUtil.ResampleAndConvert(array, array2, array2.Length, this.reverseChannels);
				}
				else
				{
					AudioUtil.Convert(array, array2, array2.Length);
				}
				Queue<short[]> obj = this.reverseStreamQueue;
				lock (obj)
				{
					if (this.reverseStreamQueue.Count < 49)
					{
						this.reverseStreamQueue.Enqueue(array2);
						this.reverseStreamQueueReady.Set();
					}
					else
					{
						this.logger.LogError("[PV] WebRTCAudioProcessor Reverse stream queue overflow", Array.Empty<object>());
						this.reverseBufferFactory.Free(array2);
					}
				}
			}
		}

		private void ReverseStreamThread()
		{
			this.logger.LogInfo("[PV] WebRTCAudioProcessor: Starting reverse stream thread", Array.Empty<object>());
			this.reverseStreamThreadRunning = true;
			try
			{
				while (!this.disposed)
				{
					this.reverseStreamQueueReady.WaitOne();
					for (;;)
					{
						short[] array = null;
						Queue<short[]> obj = this.reverseStreamQueue;
						lock (obj)
						{
							if (this.reverseStreamQueue.Count > 0)
							{
								array = this.reverseStreamQueue.Dequeue();
							}
						}
						if (array == null)
						{
							break;
						}
						int num = WebRTCAudioLib.webrtc_audio_processor_process_reverse(this.proc, array, array.Length);
						this.reverseBufferFactory.Free(array);
						if (this.lastProcessReverseErr != num)
						{
							this.lastProcessReverseErr = num;
							this.logger.LogError("[PV] WebRTCAudioProcessor: OnAudioOutFrameFloat: webrtc_audio_processor_process_reverse() error {0}", new object[]
							{
								num
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				ILogger logger = this.logger;
				string str = "[PV] WebRTCAudioProcessor: ReverseStreamThread Exceptions: ";
				Exception ex2 = ex;
				logger.LogError(str + ((ex2 != null) ? ex2.ToString() : null), Array.Empty<object>());
			}
			finally
			{
				this.logger.LogInfo("[PV] WebRTCAudioProcessor: Exiting reverse stream thread", Array.Empty<object>());
				this.reverseStreamThreadRunning = false;
			}
		}

		private int setParam(WebRTCAudioLib.Param param, int v)
		{
			if (this.disposed)
			{
				return 0;
			}
			this.logger.LogInfo("[PV] WebRTCAudioProcessor: setting param " + param.ToString() + "=" + v.ToString(), Array.Empty<object>());
			return WebRTCAudioLib.webrtc_audio_processor_set_param(this.proc, (int)param, v);
		}

		public void Dispose()
		{
			lock (this)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					this.logger.LogInfo("[PV] WebRTCAudioProcessor: destroying...", Array.Empty<object>());
					this.reverseStreamQueueReady.Set();
					if (this.proc != IntPtr.Zero)
					{
						while (this.reverseStreamThreadRunning)
						{
							Thread.Sleep(1);
						}
						WebRTCAudioLib.webrtc_audio_processor_destroy(this.proc);
						this.logger.LogInfo("[PV] WebRTCAudioProcessor: destroyed", Array.Empty<object>());
					}
				}
			}
		}

		private const int REVERSE_BUFFER_POOL_CAPACITY = 50;

		private int reverseStreamDelayMs;

		private bool aec;

		private bool aecHighPass = true;

		private bool aecm;

		private bool highPass;

		private bool ns;

		private bool agc = true;

		private int agcCompressionGain = 9;

		private int agcTargetLevel = 3;

		private bool agc2;

		private bool vad;

		private bool reverseStreamThreadRunning;

		private Queue<short[]> reverseStreamQueue = new Queue<short[]>();

		private AutoResetEvent reverseStreamQueueReady = new AutoResetEvent(false);

		private FactoryPrimitiveArrayPool<short> reverseBufferFactory;

		private bool bypass;

		private int inFrameSize;

		private int processFrameSize;

		private int samplingRate;

		private int channels;

		private IntPtr proc;

		private bool disposed;

		private Framer<float> reverseFramer;

		private int reverseSamplingRate;

		private int reverseChannels;

		private ILogger logger;

		private const int supportedFrameLenMs = 10;

		public static readonly int[] SupportedSamplingRates = new int[]
		{
			8000,
			16000,
			32000,
			48000
		};

		private bool aecInited;

		private int lastProcessErr;

		private int lastProcessReverseErr;
	}
}
