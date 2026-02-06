using System;

namespace Photon.Voice
{
	internal class DeviceEnumeratorNotSupported : DeviceEnumeratorBase
	{
		public override bool IsSupported
		{
			get
			{
				return false;
			}
		}

		public DeviceEnumeratorNotSupported(ILogger logger, string message) : base(logger)
		{
			this.message = message;
		}

		public override void Refresh()
		{
		}

		public override string Error
		{
			get
			{
				return this.message;
			}
		}

		public override void Dispose()
		{
		}

		private string message;
	}
}
