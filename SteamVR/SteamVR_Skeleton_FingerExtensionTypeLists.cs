using System;
using System.Linq;

namespace Valve.VR
{
	public class SteamVR_Skeleton_FingerExtensionTypeLists
	{
		public SteamVR_Skeleton_FingerExtensionTypes[] enumList
		{
			get
			{
				if (this._enumList == null)
				{
					this._enumList = (SteamVR_Skeleton_FingerExtensionTypes[])Enum.GetValues(typeof(SteamVR_Skeleton_FingerExtensionTypes));
				}
				return this._enumList;
			}
		}

		public string[] stringList
		{
			get
			{
				if (this._stringList == null)
				{
					this._stringList = (from element in this.enumList
					select element.ToString()).ToArray<string>();
				}
				return this._stringList;
			}
		}

		private SteamVR_Skeleton_FingerExtensionTypes[] _enumList;

		private string[] _stringList;
	}
}
