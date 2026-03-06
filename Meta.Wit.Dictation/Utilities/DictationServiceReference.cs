using System;
using Meta.WitAi.Dictation;
using UnityEngine;

namespace Meta.WitAi.Utilities
{
	[Serializable]
	public struct DictationServiceReference
	{
		public DictationService DictationService
		{
			get
			{
				if (!this.dictationService)
				{
					DictationService[] array = Resources.FindObjectsOfTypeAll<DictationService>();
					if (array != null)
					{
						this.dictationService = Array.Find<DictationService>(array, (DictationService o) => o.gameObject.scene.rootCount != 0);
					}
				}
				return this.dictationService;
			}
		}

		[SerializeField]
		internal DictationService dictationService;
	}
}
