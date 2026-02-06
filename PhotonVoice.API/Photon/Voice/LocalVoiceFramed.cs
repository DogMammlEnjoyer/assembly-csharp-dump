using System;
using System.Collections.Generic;
using System.Threading;

namespace Photon.Voice
{
	public class LocalVoiceFramed<T> : LocalVoiceFramedBase
	{
		protected T[] processFrame(T[] buf)
		{
			List<IProcessor<T>> obj = this.processors;
			lock (obj)
			{
				foreach (IProcessor<T> processor in this.processors)
				{
					buf = processor.Process(buf);
					if (buf == null)
					{
						break;
					}
				}
			}
			return buf;
		}

		public void AddPostProcessor(params IProcessor<T>[] processors)
		{
			List<IProcessor<T>> obj = this.processors;
			lock (obj)
			{
				foreach (IProcessor<T> item in processors)
				{
					this.processors.Add(item);
				}
			}
		}

		public void AddPreProcessor(params IProcessor<T>[] processors)
		{
			List<IProcessor<T>> obj = this.processors;
			lock (obj)
			{
				foreach (IProcessor<T> item in processors)
				{
					List<IProcessor<T>> list = this.processors;
					int num = this.preProcessorsCnt;
					this.preProcessorsCnt = num + 1;
					list.Insert(num, item);
				}
			}
		}

		public void ClearProcessors()
		{
			List<IProcessor<T>> obj = this.processors;
			lock (obj)
			{
				this.processors.Clear();
				this.preProcessorsCnt = 0;
			}
		}

		internal LocalVoiceFramed(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, int channelId, int frameSize) : base(voiceClient, encoder, id, voiceInfo, channelId, frameSize)
		{
			if (frameSize == 0)
			{
				throw new Exception(base.LogPrefix + ": non 0 frame size required for framed stream");
			}
			this.framer = new Framer<T>(base.FrameSize);
			this.bufferFactory = new FactoryPrimitiveArrayPool<T>(50, base.Name + " Data", base.FrameSize);
		}

		public FactoryPrimitiveArrayPool<T> BufferFactory
		{
			get
			{
				return this.bufferFactory;
			}
		}

		public bool PushDataAsyncReady
		{
			get
			{
				Queue<T[]> obj = this.pushDataQueue;
				bool result;
				lock (obj)
				{
					result = (this.pushDataQueue.Count < 49);
				}
				return result;
			}
		}

		public void PushDataAsync(T[] buf)
		{
			if (this.disposed)
			{
				return;
			}
			if (!this.dataEncodeThreadStarted)
			{
				this.voiceClient.logger.LogInfo(base.LogPrefix + ": Starting data encode thread", Array.Empty<object>());
				Thread thread = new Thread(new ThreadStart(this.PushDataAsyncThread));
				thread.Start();
				Util.SetThreadName(thread, "[PV] EncData " + base.shortName);
				this.dataEncodeThreadStarted = true;
			}
			if (this.PushDataAsyncReady)
			{
				Queue<T[]> obj = this.pushDataQueue;
				lock (obj)
				{
					this.pushDataQueue.Enqueue(buf);
				}
				this.pushDataQueueReady.Set();
				return;
			}
			this.bufferFactory.Free(buf, buf.Length);
			if (this.framesSkipped == this.framesSkippedNextLog)
			{
				this.voiceClient.logger.LogWarning(base.LogPrefix + ": PushData queue overflow. Frames skipped: " + (this.framesSkipped + 1).ToString(), Array.Empty<object>());
				this.framesSkippedNextLog = this.framesSkipped + 10;
			}
			this.framesSkipped++;
		}

		private void PushDataAsyncThread()
		{
			try
			{
				while (!this.exitThread)
				{
					this.pushDataQueueReady.WaitOne();
					while (!this.exitThread)
					{
						T[] array = null;
						Queue<T[]> obj = this.pushDataQueue;
						lock (obj)
						{
							if (this.pushDataQueue.Count > 0)
							{
								array = this.pushDataQueue.Dequeue();
							}
						}
						if (array == null)
						{
							break;
						}
						this.PushData(array);
						this.bufferFactory.Free(array, array.Length);
					}
				}
			}
			catch (Exception ex)
			{
				ILogger logger = this.voiceClient.logger;
				string logPrefix = base.LogPrefix;
				string str = ": Exception in encode thread: ";
				Exception ex2 = ex;
				logger.LogError(logPrefix + str + ((ex2 != null) ? ex2.ToString() : null), Array.Empty<object>());
				throw ex;
			}
			finally
			{
				this.Dispose();
				this.bufferFactory.Dispose();
				this.pushDataQueueReady.Close();
				this.voiceClient.logger.LogInfo(base.LogPrefix + ": Exiting data encode thread", Array.Empty<object>());
			}
		}

		public void PushData(T[] buf)
		{
			if (this.voiceClient.transport.IsChannelJoined(this.channelId) && base.TransmitEnabled)
			{
				if (this.encoder is IEncoderDirect<T[]>)
				{
					object disposeLock = this.disposeLock;
					lock (disposeLock)
					{
						if (!this.disposed)
						{
							foreach (T[] buf2 in this.framer.Frame(buf))
							{
								T[] array = this.processFrame(buf2);
								if (array != null)
								{
									this.processNullFramesCnt = 0;
									((IEncoderDirect<T[]>)this.encoder).Input(array);
								}
								else
								{
									this.processNullFramesCnt++;
									if (this.processNullFramesCnt == 1)
									{
										this.encoder.EndOfStream();
									}
								}
							}
						}
						return;
					}
				}
				throw new Exception(base.LogPrefix + ": PushData(T[]) called on encoder of unsupported type " + ((this.encoder == null) ? "null" : this.encoder.GetType().ToString()));
			}
		}

		public override void Dispose()
		{
			this.exitThread = true;
			object disposeLock = this.disposeLock;
			lock (disposeLock)
			{
				if (!this.disposed)
				{
					List<IProcessor<T>> obj = this.processors;
					lock (obj)
					{
						foreach (IProcessor<T> processor in this.processors)
						{
							processor.Dispose();
						}
					}
					base.Dispose();
					this.pushDataQueueReady.Set();
				}
			}
			base.Dispose();
		}

		private Framer<T> framer;

		private int preProcessorsCnt;

		private List<IProcessor<T>> processors = new List<IProcessor<T>>();

		private bool dataEncodeThreadStarted;

		private Queue<T[]> pushDataQueue = new Queue<T[]>();

		private AutoResetEvent pushDataQueueReady = new AutoResetEvent(false);

		private FactoryPrimitiveArrayPool<T> bufferFactory;

		private int framesSkippedNextLog;

		private int framesSkipped;

		private bool exitThread;

		private int processNullFramesCnt;
	}
}
