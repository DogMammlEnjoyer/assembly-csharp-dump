using System;
using System.Linq;
using UnityEngine;

namespace Photon.Voice.Unity
{
	public class MicWrapper : IAudioReader<float>, IDataReader<float>, IDisposable, IAudioDesc
	{
		public AudioClip Mic
		{
			get
			{
				return this.mic;
			}
		}

		public MicWrapper(string device, int suggestedFrequency, ILogger logger)
		{
			try
			{
				this.device = device;
				this.logger = logger;
				if (UnityMicrophone.devices.Length < 1)
				{
					this.Error = "No microphones found (UnityMicrophone.devices is empty)";
					logger.LogError("[PV] MicWrapper: " + this.Error, Array.Empty<object>());
				}
				else
				{
					if (!string.IsNullOrEmpty(device) && !UnityMicrophone.devices.Contains(device))
					{
						logger.LogError(string.Format("[PV] MicWrapper: \"{0}\" is not a valid Unity microphone device, falling back to default one", device), Array.Empty<object>());
						device = null;
					}
					int num;
					int num2;
					UnityMicrophone.GetDeviceCaps(device, out num, out num2);
					int frequency = suggestedFrequency;
					if (suggestedFrequency < num || (num2 != 0 && suggestedFrequency > num2))
					{
						logger.LogWarning("[PV] MicWrapper does not support suggested frequency {0} (min: {1}, max: {2}). Setting to {2}", new object[]
						{
							suggestedFrequency,
							num,
							num2
						});
						frequency = num2;
					}
					this.mic = UnityMicrophone.Start(device, true, 1, frequency);
				}
			}
			catch (Exception ex)
			{
				this.Error = ex.ToString();
				if (this.Error == null)
				{
					this.Error = "Exception in MicWrapper constructor";
				}
				logger.LogError("[PV] MicWrapper: " + this.Error, Array.Empty<object>());
			}
		}

		public int SamplingRate
		{
			get
			{
				if (this.Error != null)
				{
					return 0;
				}
				return this.mic.frequency;
			}
		}

		public int Channels
		{
			get
			{
				if (this.Error != null)
				{
					return 0;
				}
				return this.mic.channels;
			}
		}

		public string Error { get; protected set; }

		public void Dispose()
		{
			UnityMicrophone.End(this.device);
			Object.Destroy(this.mic);
			this.mic = null;
		}

		public virtual bool Read(float[] buffer)
		{
			if (this.Error != null)
			{
				return false;
			}
			int position = UnityMicrophone.GetPosition(this.device);
			if (position < this.micPrevPos)
			{
				this.micLoopCnt++;
			}
			this.micPrevPos = position;
			int num = this.micLoopCnt * this.mic.samples + position;
			if (this.mic.channels == 0)
			{
				this.Error = "Number of channels is 0 in Read()";
				this.logger.LogError("[PV] MicWrapper: " + this.Error, Array.Empty<object>());
				return false;
			}
			int num2 = buffer.Length / this.mic.channels;
			int num3 = this.readAbsPos + num2;
			if (num3 < num)
			{
				this.mic.GetData(buffer, this.readAbsPos % this.mic.samples);
				this.readAbsPos = num3;
				return true;
			}
			return false;
		}

		protected AudioClip mic;

		protected string device;

		protected ILogger logger;

		protected int micPrevPos;

		protected int micLoopCnt;

		protected int readAbsPos;
	}
}
