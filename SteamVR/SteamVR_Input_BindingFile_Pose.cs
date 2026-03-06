using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Pose
	{
		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Pose)
			{
				SteamVR_Input_BindingFile_Pose steamVR_Input_BindingFile_Pose = (SteamVR_Input_BindingFile_Pose)obj;
				return steamVR_Input_BindingFile_Pose.output == this.output && steamVR_Input_BindingFile_Pose.path == this.path;
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
