using System;
using UnityEngine;

namespace Photon.Voice.Unity
{
	public class AndroidAudioInAEC : IAudioPusher<short>, IAudioDesc, IDisposable, IResettable
	{
		public AndroidAudioInAEC(ILogger logger, bool enableAEC = false, bool enableAGC = false, bool enableNS = false)
		{
			this.logger = logger;
			try
			{
				this.callback = new AndroidAudioInAEC.DataCallback();
				this.audioIn = new AndroidJavaObject("com.exitgames.photon.audioinaec.AudioInAEC", Array.Empty<object>());
				int num = this.audioIn.Call<int>("GetMinBufferSize", new object[]
				{
					44100,
					this.Channels
				});
				logger.LogInfo("[PV] AndroidAudioInAEC: AndroidJavaObject created: aec: {0}/{1}, agc: {2}/{3}, ns: {4}/{5} minBufSize: {6}", new object[]
				{
					enableAEC,
					this.audioIn.Call<bool>("AECIsAvailable", Array.Empty<object>()),
					enableAGC,
					this.audioIn.Call<bool>("AGCIsAvailable", Array.Empty<object>()),
					enableNS,
					this.audioIn.Call<bool>("NSIsAvailable", Array.Empty<object>()),
					num
				});
				AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				bool flag = this.audioIn.Call<bool>("Start", new object[]
				{
					@static,
					this.callback,
					44100,
					this.Channels,
					num * 4,
					enableAEC,
					enableAGC,
					enableNS
				});
				if (flag)
				{
					this.audioInSampleRate = this.audioIn.Call<int>("GetSampleRate", Array.Empty<object>());
					logger.LogInfo("[PV] AndroidAudioInAEC: AndroidJavaObject started: {0}, sampling rate: {1}, channels: {2}, record buffer size: {3}", new object[]
					{
						flag,
						this.SamplingRate,
						this.Channels,
						num * 4
					});
				}
				else
				{
					this.Error = "[PV] AndroidAudioInAEC constructor: calling Start java method failure";
					logger.LogError("[PV] AndroidAudioInAEC: {0}", new object[]
					{
						this.Error
					});
				}
			}
			catch (Exception ex)
			{
				this.Error = ex.ToString();
				if (this.Error == null)
				{
					this.Error = "Exception in AndroidAudioInAEC constructor";
				}
				logger.LogError("[PV] AndroidAudioInAEC: {0}", new object[]
				{
					this.Error
				});
			}
		}

		public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
		{
			if (this.Error == null)
			{
				int info = bufferFactory.Info;
				this.javaBuf = AndroidJNI.NewGlobalRef(AndroidJNI.NewShortArray(info));
				this.callback.SetCallback(callback, this.javaBuf);
				IntPtr methodID = AndroidJNI.GetMethodID(this.audioIn.GetRawClass(), "SetBuffer", "([S)Z");
				if (!AndroidJNI.CallBooleanMethod(this.audioIn.GetRawObject(), methodID, new jvalue[]
				{
					new jvalue
					{
						l = this.javaBuf
					}
				}))
				{
					this.Error = "AndroidAudioInAEC.SetCallback(): calling SetBuffer java method failure";
				}
			}
			if (this.Error != null)
			{
				this.logger.LogError("[PV] AndroidAudioInAEC: {0}", new object[]
				{
					this.Error
				});
			}
		}

		public int Channels
		{
			get
			{
				return 1;
			}
		}

		public int SamplingRate
		{
			get
			{
				return this.audioInSampleRate;
			}
		}

		public string Error { get; private set; }

		public void Reset()
		{
			if (this.audioIn != null)
			{
				this.audioIn.Call("Reset", Array.Empty<object>());
			}
		}

		public void Dispose()
		{
			if (this.audioIn != null)
			{
				this.audioIn.Call<bool>("Stop", Array.Empty<object>());
			}
		}

		private AndroidJavaObject audioIn;

		private IntPtr javaBuf;

		private ILogger logger;

		private int audioInSampleRate;

		private AndroidAudioInAEC.DataCallback callback;

		private class DataCallback : AndroidJavaProxy
		{
			public DataCallback() : base("com.exitgames.photon.audioinaec.AudioInAEC$DataCallback")
			{
			}

			public void SetCallback(Action<short[]> callback, IntPtr javaBuf)
			{
				this.callback = callback;
				this.javaBuf = javaBuf;
			}

			public void OnData()
			{
				if (this.callback != null)
				{
					short[] array = AndroidJNI.FromShortArray(this.javaBuf);
					this.cntFrame++;
					this.cntShort += array.Length;
					this.callback(array);
				}
			}

			public void OnStop()
			{
				AndroidJNI.DeleteGlobalRef(this.javaBuf);
			}

			private Action<short[]> callback;

			private IntPtr javaBuf;

			private int cntFrame;

			private int cntShort;
		}
	}
}
