using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice.Windows
{
	public class WindowsAudioInPusher : IAudioPusher<short>, IAudioDesc, IDisposable
	{
		[DllImport("AudioIn")]
		private static extern IntPtr Photon_Audio_In_Create(int instanceID, WindowsAudioInPusher.SystemMode systemMode, int micDevIdx, int spkDevIdx, Action<int, IntPtr, int> callback, bool featrModeOn, bool noiseSup, bool agc, bool cntrClip);

		[DllImport("AudioIn")]
		private static extern void Photon_Audio_In_Destroy(IntPtr handler);

		public WindowsAudioInPusher(int deviceID, ILogger logger)
		{
			try
			{
				Dictionary<int, WindowsAudioInPusher> obj = WindowsAudioInPusher.instancePerHandle;
				lock (obj)
				{
					this.handle = WindowsAudioInPusher.Photon_Audio_In_Create(WindowsAudioInPusher.instanceCnt, WindowsAudioInPusher.SystemMode.SINGLE_CHANNEL_AEC, deviceID, -1, new Action<int, IntPtr, int>(WindowsAudioInPusher.nativePushCallback), true, true, true, true);
					this.instanceID = WindowsAudioInPusher.instanceCnt;
					WindowsAudioInPusher.instancePerHandle.Add(WindowsAudioInPusher.instanceCnt++, this);
				}
			}
			catch (Exception ex)
			{
				this.Error = ex.ToString();
				if (this.Error == null)
				{
					this.Error = "Exception in WindowsAudioInPusher constructor";
				}
				logger.LogError("[PV] WindowsAudioInPusher: " + this.Error, Array.Empty<object>());
			}
		}

		[MonoPInvokeCallback(typeof(WindowsAudioInPusher.CallbackDelegate))]
		private static void nativePushCallback(int instanceID, IntPtr buf, int len)
		{
			Dictionary<int, WindowsAudioInPusher> obj = WindowsAudioInPusher.instancePerHandle;
			WindowsAudioInPusher windowsAudioInPusher;
			bool flag2;
			lock (obj)
			{
				flag2 = WindowsAudioInPusher.instancePerHandle.TryGetValue(instanceID, out windowsAudioInPusher);
			}
			if (flag2)
			{
				windowsAudioInPusher.push(buf, len);
			}
		}

		public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
		{
			this.bufferFactory = bufferFactory;
			this.pushCallback = callback;
		}

		private void push(IntPtr buf, int lenBytes)
		{
			if (this.pushCallback != null)
			{
				int num = lenBytes / 2;
				short[] array = this.bufferFactory.New(num);
				Marshal.Copy(buf, array, 0, num);
				this.pushCallback(array);
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
				return 16000;
			}
		}

		public string Error { get; private set; }

		public void Dispose()
		{
			Dictionary<int, WindowsAudioInPusher> obj = WindowsAudioInPusher.instancePerHandle;
			lock (obj)
			{
				WindowsAudioInPusher.instancePerHandle.Remove(this.instanceID);
			}
			if (this.handle != IntPtr.Zero)
			{
				WindowsAudioInPusher.Photon_Audio_In_Destroy(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		private IntPtr handle;

		private int instanceID;

		private Action<short[]> pushCallback;

		private ObjectFactory<short[], int> bufferFactory;

		private static int instanceCnt;

		private static Dictionary<int, WindowsAudioInPusher> instancePerHandle = new Dictionary<int, WindowsAudioInPusher>();

		private enum SystemMode
		{
			SINGLE_CHANNEL_AEC,
			OPTIBEAM_ARRAY_ONLY = 2,
			OPTIBEAM_ARRAY_AND_AEC = 4,
			SINGLE_CHANNEL_NSAGC
		}

		private delegate void CallbackDelegate(int instanceID, IntPtr buf, int len);
	}
}
