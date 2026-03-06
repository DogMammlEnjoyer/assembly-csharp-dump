using System;
using UnityEngine.Scripting;

namespace Meta.WitAi.TTS.Data
{
	[Serializable]
	public class TTSVisemeEvent : TTSEvent<Viseme>
	{
		[Preserve]
		public static Viseme GetVisemeAot(string inViseme)
		{
			Viseme result;
			Enum.TryParse<Viseme>(inViseme, out result);
			return result;
		}
	}
}
