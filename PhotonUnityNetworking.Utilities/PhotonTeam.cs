using System;

namespace Photon.Pun.UtilityScripts
{
	[Serializable]
	public class PhotonTeam
	{
		public override string ToString()
		{
			return string.Format("{0} [{1}]", this.Name, this.Code);
		}

		public string Name;

		public byte Code;
	}
}
