using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_ActionSet_Data : ISteamVR_ActionSet
	{
		public SteamVR_Action[] allActions { get; set; }

		public ISteamVR_Action_In[] nonVisualInActions { get; set; }

		public ISteamVR_Action_In[] visualActions { get; set; }

		public SteamVR_Action_Pose[] poseActions { get; set; }

		public SteamVR_Action_Skeleton[] skeletonActions { get; set; }

		public ISteamVR_Action_Out[] outActionArray { get; set; }

		public string fullPath { get; set; }

		public string usage { get; set; }

		public ulong handle { get; set; }

		public void PreInitialize()
		{
		}

		public void FinishPreInitialize()
		{
			List<SteamVR_Action> list = new List<SteamVR_Action>();
			List<ISteamVR_Action_In> list2 = new List<ISteamVR_Action_In>();
			List<ISteamVR_Action_In> list3 = new List<ISteamVR_Action_In>();
			List<SteamVR_Action_Pose> list4 = new List<SteamVR_Action_Pose>();
			List<SteamVR_Action_Skeleton> list5 = new List<SteamVR_Action_Skeleton>();
			List<ISteamVR_Action_Out> list6 = new List<ISteamVR_Action_Out>();
			if (SteamVR_Input.actions == null)
			{
				Debug.LogError("<b>[SteamVR Input]</b> Actions not initialized!");
				return;
			}
			for (int i = 0; i < SteamVR_Input.actions.Length; i++)
			{
				SteamVR_Action steamVR_Action = SteamVR_Input.actions[i];
				if (steamVR_Action.actionSet.GetActionSetData() == this)
				{
					list.Add(steamVR_Action);
					if (steamVR_Action is ISteamVR_Action_Boolean || steamVR_Action is ISteamVR_Action_Single || steamVR_Action is ISteamVR_Action_Vector2 || steamVR_Action is ISteamVR_Action_Vector3)
					{
						list2.Add((ISteamVR_Action_In)steamVR_Action);
					}
					else if (steamVR_Action is SteamVR_Action_Pose)
					{
						list3.Add((ISteamVR_Action_In)steamVR_Action);
						list4.Add((SteamVR_Action_Pose)steamVR_Action);
					}
					else if (steamVR_Action is SteamVR_Action_Skeleton)
					{
						list3.Add((ISteamVR_Action_In)steamVR_Action);
						list5.Add((SteamVR_Action_Skeleton)steamVR_Action);
					}
					else if (steamVR_Action is ISteamVR_Action_Out)
					{
						list6.Add((ISteamVR_Action_Out)steamVR_Action);
					}
					else
					{
						Debug.LogError("<b>[SteamVR Input]</b> Action doesn't implement known interface: " + steamVR_Action.fullPath);
					}
				}
			}
			this.allActions = list.ToArray();
			this.nonVisualInActions = list2.ToArray();
			this.visualActions = list3.ToArray();
			this.poseActions = list4.ToArray();
			this.skeletonActions = list5.ToArray();
			this.outActionArray = list6.ToArray();
		}

		public void Initialize()
		{
			ulong handle = 0UL;
			EVRInputError actionSetHandle = OpenVR.Input.GetActionSetHandle(this.fullPath.ToLower(), ref handle);
			this.handle = handle;
			if (actionSetHandle != EVRInputError.None)
			{
				Debug.LogError("<b>[SteamVR]</b> GetActionSetHandle (" + this.fullPath + ") error: " + actionSetHandle.ToString());
			}
			this.initialized = true;
		}

		public bool IsActive(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
		{
			return this.initialized && (this.rawSetActive[(int)source] || this.rawSetActive[0]);
		}

		public float GetTimeLastChanged(SteamVR_Input_Sources source = SteamVR_Input_Sources.Any)
		{
			if (this.initialized)
			{
				return this.rawSetLastChanged[(int)source];
			}
			return 0f;
		}

		public void Activate(SteamVR_Input_Sources activateForSource = SteamVR_Input_Sources.Any, int priority = 0, bool disableAllOtherActionSets = false)
		{
			if (disableAllOtherActionSets)
			{
				SteamVR_ActionSet_Manager.DisableAllActionSets();
			}
			if (!this.rawSetActive[(int)activateForSource])
			{
				this.rawSetActive[(int)activateForSource] = true;
				SteamVR_ActionSet_Manager.SetChanged();
				this.rawSetLastChanged[(int)activateForSource] = Time.realtimeSinceStartup;
			}
			if (this.rawSetPriority[(int)activateForSource] != priority)
			{
				this.rawSetPriority[(int)activateForSource] = priority;
				SteamVR_ActionSet_Manager.SetChanged();
				this.rawSetLastChanged[(int)activateForSource] = Time.realtimeSinceStartup;
			}
		}

		public void Deactivate(SteamVR_Input_Sources forSource = SteamVR_Input_Sources.Any)
		{
			if (this.rawSetActive[(int)forSource])
			{
				this.rawSetLastChanged[(int)forSource] = Time.realtimeSinceStartup;
				SteamVR_ActionSet_Manager.SetChanged();
			}
			this.rawSetActive[(int)forSource] = false;
			this.rawSetPriority[(int)forSource] = 0;
		}

		public string GetShortName()
		{
			if (this.cachedShortName == null)
			{
				this.cachedShortName = SteamVR_Input_ActionFile.GetShortName(this.fullPath);
			}
			return this.cachedShortName;
		}

		public bool ReadRawSetActive(SteamVR_Input_Sources inputSource)
		{
			return this.rawSetActive[(int)inputSource];
		}

		public float ReadRawSetLastChanged(SteamVR_Input_Sources inputSource)
		{
			return this.rawSetLastChanged[(int)inputSource];
		}

		public int ReadRawSetPriority(SteamVR_Input_Sources inputSource)
		{
			return this.rawSetPriority[(int)inputSource];
		}

		protected bool[] rawSetActive = new bool[SteamVR_Input_Source.numSources];

		protected float[] rawSetLastChanged = new float[SteamVR_Input_Source.numSources];

		protected int[] rawSetPriority = new int[SteamVR_Input_Source.numSources];

		protected bool initialized;

		private string cachedShortName;
	}
}
