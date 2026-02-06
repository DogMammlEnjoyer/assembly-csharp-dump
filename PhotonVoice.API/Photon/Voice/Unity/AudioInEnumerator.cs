using System;
using System.Collections.Generic;

namespace Photon.Voice.Unity
{
	public class AudioInEnumerator : DeviceEnumeratorBase
	{
		public AudioInEnumerator(ILogger logger) : base(logger)
		{
			this.Refresh();
		}

		public override void Refresh()
		{
			string[] devices = UnityMicrophone.devices;
			this.devices = new List<DeviceInfo>();
			foreach (string name in devices)
			{
				this.devices.Add(new DeviceInfo(name));
			}
		}

		public override string Error
		{
			get
			{
				return null;
			}
		}

		public override void Dispose()
		{
		}
	}
}
