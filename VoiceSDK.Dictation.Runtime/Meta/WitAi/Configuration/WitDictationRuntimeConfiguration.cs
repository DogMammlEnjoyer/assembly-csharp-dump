using System;
using Oculus.Voice.Dictation.Configuration;
using UnityEngine;

namespace Meta.WitAi.Configuration
{
	[Serializable]
	public class WitDictationRuntimeConfiguration : WitRuntimeConfiguration
	{
		protected override Vector2 RecordingTimeRange
		{
			get
			{
				return new Vector2(-1f, 300f);
			}
		}

		[Header("Dictation")]
		[SerializeField]
		public DictationConfiguration dictationConfiguration;
	}
}
