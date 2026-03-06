using System;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Haptic
	{
		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Haptic)
			{
				SteamVR_Input_BindingFile_Haptic steamVR_Input_BindingFile_Haptic = (SteamVR_Input_BindingFile_Haptic)obj;
				return steamVR_Input_BindingFile_Haptic.output == this.output && steamVR_Input_BindingFile_Haptic.path == this.path;
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
