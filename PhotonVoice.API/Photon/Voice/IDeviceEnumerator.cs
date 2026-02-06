using System;
using System.Collections;
using System.Collections.Generic;

namespace Photon.Voice
{
	public interface IDeviceEnumerator : IDisposable, IEnumerable<DeviceInfo>, IEnumerable
	{
		bool IsSupported { get; }

		void Refresh();

		string Error { get; }
	}
}
