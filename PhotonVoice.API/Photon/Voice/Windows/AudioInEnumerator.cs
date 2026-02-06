using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Photon.Voice.Windows
{
	public class AudioInEnumerator : DeviceEnumeratorBase
	{
		[DllImport("AudioIn")]
		private static extern IntPtr Photon_Audio_In_CreateMicEnumerator();

		[DllImport("AudioIn")]
		private static extern void Photon_Audio_In_DestroyMicEnumerator(IntPtr handle);

		[DllImport("AudioIn")]
		private static extern int Photon_Audio_In_MicEnumerator_Count(IntPtr handle);

		[DllImport("AudioIn")]
		private static extern IntPtr Photon_Audio_In_MicEnumerator_NameAtIndex(IntPtr handle, int idx);

		[DllImport("AudioIn")]
		private static extern int Photon_Audio_In_MicEnumerator_IDAtIndex(IntPtr handle, int idx);

		public AudioInEnumerator(ILogger logger) : base(logger)
		{
			this.Refresh();
		}

		public override void Refresh()
		{
			this.Dispose();
			try
			{
				this.handle = AudioInEnumerator.Photon_Audio_In_CreateMicEnumerator();
				int num = AudioInEnumerator.Photon_Audio_In_MicEnumerator_Count(this.handle);
				this.devices = new List<DeviceInfo>();
				for (int i = 0; i < num; i++)
				{
					this.devices.Add(new DeviceInfo(AudioInEnumerator.Photon_Audio_In_MicEnumerator_IDAtIndex(this.handle, i), Marshal.PtrToStringAuto(AudioInEnumerator.Photon_Audio_In_MicEnumerator_NameAtIndex(this.handle, i))));
				}
				this.Error = null;
			}
			catch (Exception ex)
			{
				this.Error = ex.ToString();
				if (this.Error == null)
				{
					this.Error = "Exception in AudioInEnumerator.Refresh()";
				}
			}
		}

		public override void Dispose()
		{
			if (this.handle != IntPtr.Zero && this.Error == null)
			{
				AudioInEnumerator.Photon_Audio_In_DestroyMicEnumerator(this.handle);
				this.handle = IntPtr.Zero;
			}
		}

		private const string lib_name = "AudioIn";

		private IntPtr handle;
	}
}
