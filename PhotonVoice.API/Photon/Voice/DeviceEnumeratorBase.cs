using System;
using System.Collections;
using System.Collections.Generic;

namespace Photon.Voice
{
	public abstract class DeviceEnumeratorBase : IDeviceEnumerator, IDisposable, IEnumerable<DeviceInfo>, IEnumerable
	{
		public DeviceEnumeratorBase(ILogger logger)
		{
			this.logger = logger;
		}

		public virtual bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public virtual string Error { get; protected set; }

		public IEnumerator<DeviceInfo> GetEnumerator()
		{
			return this.devices.GetEnumerator();
		}

		public abstract void Refresh();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public abstract void Dispose();

		protected List<DeviceInfo> devices = new List<DeviceInfo>();

		protected ILogger logger;
	}
}
