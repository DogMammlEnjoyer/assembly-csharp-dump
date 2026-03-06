using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	public class ChaperoneInfo : MonoBehaviour
	{
		public bool initialized { get; private set; }

		public float playAreaSizeX { get; private set; }

		public float playAreaSizeZ { get; private set; }

		public bool roomscale { get; private set; }

		public static SteamVR_Events.Action InitializedAction(UnityAction action)
		{
			return new SteamVR_Events.ActionNoArgs(ChaperoneInfo.Initialized, action);
		}

		public static ChaperoneInfo instance
		{
			get
			{
				if (ChaperoneInfo._instance == null)
				{
					ChaperoneInfo._instance = new GameObject("[ChaperoneInfo]").AddComponent<ChaperoneInfo>();
					ChaperoneInfo._instance.initialized = false;
					ChaperoneInfo._instance.playAreaSizeX = 1f;
					ChaperoneInfo._instance.playAreaSizeZ = 1f;
					ChaperoneInfo._instance.roomscale = false;
					Object.DontDestroyOnLoad(ChaperoneInfo._instance.gameObject);
				}
				return ChaperoneInfo._instance;
			}
		}

		private IEnumerator Start()
		{
			CVRChaperone chaperone = OpenVR.Chaperone;
			if (chaperone == null)
			{
				Debug.LogWarning("<b>[SteamVR Interaction]</b> Failed to get IVRChaperone interface.");
				this.initialized = true;
				yield break;
			}
			float num;
			float num2;
			for (;;)
			{
				num = 0f;
				num2 = 0f;
				if (chaperone.GetPlayAreaSize(ref num, ref num2))
				{
					break;
				}
				yield return null;
			}
			this.initialized = true;
			this.playAreaSizeX = num;
			this.playAreaSizeZ = num2;
			this.roomscale = (Mathf.Max(num, num2) > 1.01f);
			Debug.LogFormat("<b>[SteamVR Interaction]</b> ChaperoneInfo initialized. {2} play area {0:0.00}m x {1:0.00}m", new object[]
			{
				num,
				num2,
				this.roomscale ? "Roomscale" : "Standing"
			});
			ChaperoneInfo.Initialized.Send();
			yield break;
			yield break;
		}

		public static SteamVR_Events.Event Initialized = new SteamVR_Events.Event();

		private static ChaperoneInfo _instance;
	}
}
