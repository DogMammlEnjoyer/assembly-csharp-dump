using System;

namespace Valve.VR
{
	public static class SteamVR_Input_ActionFile_ActionSet_Usages
	{
		public static string leftright = "leftright";

		public static string single = "single";

		public static string hidden = "hidden";

		public static string leftrightDescription = "per hand";

		public static string singleDescription = "mirrored";

		public static string hiddenDescription = "hidden";

		public static string[] listValues = new string[]
		{
			SteamVR_Input_ActionFile_ActionSet_Usages.leftright,
			SteamVR_Input_ActionFile_ActionSet_Usages.single,
			SteamVR_Input_ActionFile_ActionSet_Usages.hidden
		};

		public static string[] listDescriptions = new string[]
		{
			SteamVR_Input_ActionFile_ActionSet_Usages.leftrightDescription,
			SteamVR_Input_ActionFile_ActionSet_Usages.singleDescription,
			SteamVR_Input_ActionFile_ActionSet_Usages.hiddenDescription
		};
	}
}
