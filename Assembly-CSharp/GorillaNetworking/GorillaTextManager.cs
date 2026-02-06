using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaTextManager : MonoBehaviourPostTick
	{
		public static void RegisterText(GorillaText text)
		{
			if (GorillaTextManager.instance == null)
			{
				GorillaTextManager.CreateManager();
			}
			if (!GorillaTextManager.instance.gorillaTexts.Contains(text))
			{
				GorillaTextManager.instance.gorillaTexts.Add(text);
			}
		}

		private void Awake()
		{
			if (GorillaTextManager.instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			GorillaTextManager.instance = this;
		}

		public override void PostTick()
		{
			for (int i = 0; i < this.gorillaTexts.Count; i++)
			{
				this.gorillaTexts[i].InvokeIfUpdated();
			}
		}

		public static void CreateManager()
		{
			GorillaTextManager gorillaTextManager = new GameObject("GorillaTextManager").AddComponent<GorillaTextManager>();
			gorillaTextManager.gorillaTexts = new List<GorillaText>();
			GorillaTextManager.instance = gorillaTextManager;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gorillaTextManager);
			}
		}

		public static GorillaTextManager instance;

		public List<GorillaText> gorillaTexts = new List<GorillaText>();
	}
}
