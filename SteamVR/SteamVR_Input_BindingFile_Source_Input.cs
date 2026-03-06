using System;
using System.Collections.Generic;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Source_Input : Dictionary<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary>
	{
		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Source_Input)
			{
				SteamVR_Input_BindingFile_Source_Input steamVR_Input_BindingFile_Source_Input = (SteamVR_Input_BindingFile_Source_Input)obj;
				if (this == steamVR_Input_BindingFile_Source_Input)
				{
					return true;
				}
				if (base.Count == steamVR_Input_BindingFile_Source_Input.Count)
				{
					foreach (KeyValuePair<string, SteamVR_Input_BindingFile_Source_Input_StringDictionary> keyValuePair in this)
					{
						if (!steamVR_Input_BindingFile_Source_Input.ContainsKey(keyValuePair.Key))
						{
							return false;
						}
						if (!base[keyValuePair.Key].Equals(steamVR_Input_BindingFile_Source_Input[keyValuePair.Key]))
						{
							return false;
						}
					}
					return true;
				}
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
