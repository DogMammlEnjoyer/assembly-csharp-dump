using System;

namespace Fusion.Photon.Realtime
{
	[Serializable]
	public class FusionAppSettings : AppSettings
	{
		public new FusionAppSettings GetCopy()
		{
			FusionAppSettings fusionAppSettings = new FusionAppSettings();
			base.CopyTo(fusionAppSettings);
			fusionAppSettings.encryptionMode = this.encryptionMode;
			fusionAppSettings.emptyRoomTtl = this.emptyRoomTtl;
			return fusionAppSettings;
		}

		public override string ToString()
		{
			return string.Format("encryptionMode {0}, emptyRoomTtl {1}, {2}", this.encryptionMode, this.emptyRoomTtl, base.ToStringFull());
		}

		[InlineHelp]
		public EncryptionMode encryptionMode;

		[InlineHelp]
		public int emptyRoomTtl;
	}
}
