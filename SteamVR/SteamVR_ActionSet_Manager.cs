using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
	public static class SteamVR_ActionSet_Manager
	{
		public static void Initialize()
		{
			SteamVR_ActionSet_Manager.activeActionSetSize = (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t));
		}

		public static void DisableAllActionSets()
		{
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_Input.actionSets[i].Deactivate(SteamVR_Input_Sources.Any);
				SteamVR_Input.actionSets[i].Deactivate(SteamVR_Input_Sources.LeftHand);
				SteamVR_Input.actionSets[i].Deactivate(SteamVR_Input_Sources.RightHand);
			}
		}

		public static void UpdateActionStates(bool force = false)
		{
			if (force || Time.frameCount != SteamVR_ActionSet_Manager.lastFrameUpdated)
			{
				SteamVR_ActionSet_Manager.lastFrameUpdated = Time.frameCount;
				if (SteamVR_ActionSet_Manager.changed)
				{
					SteamVR_ActionSet_Manager.UpdateActionSetsArray();
				}
				if (SteamVR_ActionSet_Manager.rawActiveActionSetArray != null && SteamVR_ActionSet_Manager.rawActiveActionSetArray.Length != 0 && OpenVR.Input != null)
				{
					EVRInputError evrinputError = OpenVR.Input.UpdateActionState(SteamVR_ActionSet_Manager.rawActiveActionSetArray, SteamVR_ActionSet_Manager.activeActionSetSize);
					if (evrinputError != EVRInputError.None)
					{
						Debug.LogError("<b>[SteamVR]</b> UpdateActionState error: " + evrinputError.ToString());
					}
				}
			}
		}

		public static void SetChanged()
		{
			SteamVR_ActionSet_Manager.changed = true;
		}

		private static void UpdateActionSetsArray()
		{
			List<VRActiveActionSet_t> list = new List<VRActiveActionSet_t>();
			SteamVR_Input_Sources[] allSources = SteamVR_Input_Source.GetAllSources();
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = SteamVR_Input.actionSets[i];
				foreach (SteamVR_Input_Sources inputSource in allSources)
				{
					if (steamVR_ActionSet.ReadRawSetActive(inputSource))
					{
						VRActiveActionSet_t vractiveActionSet_t = default(VRActiveActionSet_t);
						vractiveActionSet_t.ulActionSet = steamVR_ActionSet.handle;
						vractiveActionSet_t.nPriority = steamVR_ActionSet.ReadRawSetPriority(inputSource);
						vractiveActionSet_t.ulRestrictedToDevice = SteamVR_Input_Source.GetHandle(inputSource);
						int num = 0;
						while (num < list.Count && list[num].nPriority <= vractiveActionSet_t.nPriority)
						{
							num++;
						}
						list.Insert(num, vractiveActionSet_t);
					}
				}
			}
			SteamVR_ActionSet_Manager.changed = false;
			SteamVR_ActionSet_Manager.rawActiveActionSetArray = list.ToArray();
			if (Application.isEditor || SteamVR_ActionSet_Manager.updateDebugTextInBuilds)
			{
				SteamVR_ActionSet_Manager.UpdateDebugText();
			}
		}

		public static SteamVR_ActionSet GetSetFromHandle(ulong handle)
		{
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = SteamVR_Input.actionSets[i];
				if (steamVR_ActionSet.handle == handle)
				{
					return steamVR_ActionSet;
				}
			}
			return null;
		}

		private static void UpdateDebugText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < SteamVR_ActionSet_Manager.rawActiveActionSetArray.Length; i++)
			{
				VRActiveActionSet_t vractiveActionSet_t = SteamVR_ActionSet_Manager.rawActiveActionSetArray[i];
				stringBuilder.Append(vractiveActionSet_t.nPriority);
				stringBuilder.Append("\t");
				stringBuilder.Append(SteamVR_Input_Source.GetSource(vractiveActionSet_t.ulRestrictedToDevice));
				stringBuilder.Append("\t");
				stringBuilder.Append(SteamVR_ActionSet_Manager.GetSetFromHandle(vractiveActionSet_t.ulActionSet).GetShortName());
				stringBuilder.Append("\n");
			}
			SteamVR_ActionSet_Manager.debugActiveSetListText = stringBuilder.ToString();
		}

		public static VRActiveActionSet_t[] rawActiveActionSetArray;

		[NonSerialized]
		private static uint activeActionSetSize;

		private static bool changed;

		private static int lastFrameUpdated;

		public static string debugActiveSetListText;

		public static bool updateDebugTextInBuilds;
	}
}
