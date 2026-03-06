using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Skeleton
	{
		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Skeleton)
			{
				SteamVR_Input_BindingFile_Skeleton steamVR_Input_BindingFile_Skeleton = (SteamVR_Input_BindingFile_Skeleton)obj;
				return steamVR_Input_BindingFile_Skeleton.output == this.output && steamVR_Input_BindingFile_Skeleton.path == this.path;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string output;

		public string path;
	}
}
