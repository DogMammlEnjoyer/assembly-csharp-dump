using System;

namespace Valve.VR
{
	public static class SteamVR_Input_ActionFile_ActionTypes
	{
		public static string boolean = "boolean";

		public static string vector1 = "vector1";

		public static string vector2 = "vector2";

		public static string vector3 = "vector3";

		public static string vibration = "vibration";

		public static string pose = "pose";

		public static string skeleton = "skeleton";

		public static string skeletonLeftPath = "\\skeleton\\hand\\left";

		public static string skeletonRightPath = "\\skeleton\\hand\\right";

		public static string[] listAll = new string[]
		{
			SteamVR_Input_ActionFile_ActionTypes.boolean,
			SteamVR_Input_ActionFile_ActionTypes.vector1,
			SteamVR_Input_ActionFile_ActionTypes.vector2,
			SteamVR_Input_ActionFile_ActionTypes.vector3,
			SteamVR_Input_ActionFile_ActionTypes.vibration,
			SteamVR_Input_ActionFile_ActionTypes.pose,
			SteamVR_Input_ActionFile_ActionTypes.skeleton
		};

		public static string[] listIn = new string[]
		{
			SteamVR_Input_ActionFile_ActionTypes.boolean,
			SteamVR_Input_ActionFile_ActionTypes.vector1,
			SteamVR_Input_ActionFile_ActionTypes.vector2,
			SteamVR_Input_ActionFile_ActionTypes.vector3,
			SteamVR_Input_ActionFile_ActionTypes.pose,
			SteamVR_Input_ActionFile_ActionTypes.skeleton
		};

		public static string[] listOut = new string[]
		{
			SteamVR_Input_ActionFile_ActionTypes.vibration
		};

		public static string[] listSkeletons = new string[]
		{
			SteamVR_Input_ActionFile_ActionTypes.skeletonLeftPath,
			SteamVR_Input_ActionFile_ActionTypes.skeletonRightPath
		};
	}
}
