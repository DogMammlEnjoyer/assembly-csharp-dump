using System;
using UnityEngine;

namespace Meta.WitAi.Utilities
{
	[Serializable]
	public struct VoiceServiceReference
	{
		public VoiceService VoiceService
		{
			get
			{
				if (!this.voiceService)
				{
					VoiceService[] array = Resources.FindObjectsOfTypeAll<VoiceService>();
					if (array != null)
					{
						this.voiceService = Array.Find<VoiceService>(array, (VoiceService o) => o.gameObject.scene.rootCount != 0);
					}
				}
				return this.voiceService;
			}
		}

		[SerializeField]
		internal VoiceService voiceService;
	}
}
