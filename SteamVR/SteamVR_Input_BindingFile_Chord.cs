using System;
using System.Collections.Generic;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Input_BindingFile_Chord
	{
		public override bool Equals(object obj)
		{
			if (obj is SteamVR_Input_BindingFile_Chord)
			{
				SteamVR_Input_BindingFile_Chord steamVR_Input_BindingFile_Chord = (SteamVR_Input_BindingFile_Chord)obj;
				if (this.output == steamVR_Input_BindingFile_Chord.output && this.inputs != null && steamVR_Input_BindingFile_Chord.inputs != null && this.inputs.Count == steamVR_Input_BindingFile_Chord.inputs.Count)
				{
					for (int i = 0; i < this.inputs.Count; i++)
					{
						if (this.inputs[i] != null && steamVR_Input_BindingFile_Chord.inputs[i] != null && this.inputs[i].Count == steamVR_Input_BindingFile_Chord.inputs[i].Count)
						{
							for (int j = 0; j < this.inputs[i].Count; j++)
							{
								if (this.inputs[i][j] != steamVR_Input_BindingFile_Chord.inputs[i][j])
								{
									return false;
								}
							}
							return true;
						}
					}
				}
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string output;

		public List<List<string>> inputs = new List<List<string>>();
	}
}
