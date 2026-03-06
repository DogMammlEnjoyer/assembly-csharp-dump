using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
	public class SteamVR_Input
	{
		public static event Action onNonVisualActionsUpdated;

		public static event SteamVR_Input.PosesUpdatedHandler onPosesUpdated;

		public static event SteamVR_Input.SkeletonsUpdatedHandler onSkeletonsUpdated;

		public static bool isStartupFrame
		{
			get
			{
				return Time.frameCount >= SteamVR_Input.startupFrame - 1 && Time.frameCount <= SteamVR_Input.startupFrame + 1;
			}
		}

		static SteamVR_Input()
		{
			SteamVR_Input.FindPreinitializeMethod();
		}

		public static void ForcePreinitialize()
		{
			SteamVR_Input.FindPreinitializeMethod();
		}

		private static void FindPreinitializeMethod()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type type = assemblies[i].GetType("Valve.VR.SteamVR_Actions");
				if (type != null)
				{
					MethodInfo method = type.GetMethod("PreInitialize");
					if (method != null)
					{
						method.Invoke(null, null);
						return;
					}
				}
			}
		}

		public static void Initialize(bool force = false)
		{
			if (SteamVR_Input.initialized && !force)
			{
				return;
			}
			SteamVR_Input.initializing = true;
			SteamVR_Input.startupFrame = Time.frameCount;
			SteamVR_ActionSet_Manager.Initialize();
			SteamVR_Input_Source.Initialize();
			for (int i = 0; i < SteamVR_Input.actions.Length; i++)
			{
				SteamVR_Input.actions[i].Initialize(true, true);
			}
			for (int j = 0; j < SteamVR_Input.actionSets.Length; j++)
			{
				SteamVR_Input.actionSets[j].Initialize(true, true);
			}
			if (SteamVR_Settings.instance.activateFirstActionSetOnStart)
			{
				if (SteamVR_Input.actionSets.Length != 0)
				{
					SteamVR_Input.actionSets[0].Activate(SteamVR_Input_Sources.Any, 0, false);
				}
				else
				{
					Debug.LogError("<b>[SteamVR]</b> No action sets to activate.");
				}
			}
			SteamVR_Action_Pose.SetTrackingUniverseOrigin(SteamVR_Settings.instance.trackingSpace);
			SteamVR_Input.initialized = true;
			SteamVR_Input.initializing = false;
		}

		public static void PreinitializeFinishActionSets()
		{
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_Input.actionSets[i].FinishPreInitialize();
			}
		}

		public static void PreinitializeActionSetDictionaries()
		{
			SteamVR_Input.actionSetsByPath.Clear();
			SteamVR_Input.actionSetsByPathLowered.Clear();
			SteamVR_Input.actionSetsByPathCache.Clear();
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = SteamVR_Input.actionSets[i];
				SteamVR_Input.actionSetsByPath.Add(steamVR_ActionSet.fullPath, steamVR_ActionSet);
				SteamVR_Input.actionSetsByPathLowered.Add(steamVR_ActionSet.fullPath.ToLower(), steamVR_ActionSet);
			}
		}

		public static void PreinitializeActionDictionaries()
		{
			SteamVR_Input.actionsByPath.Clear();
			SteamVR_Input.actionsByPathLowered.Clear();
			SteamVR_Input.actionsByPathCache.Clear();
			for (int i = 0; i < SteamVR_Input.actions.Length; i++)
			{
				SteamVR_Action steamVR_Action = SteamVR_Input.actions[i];
				SteamVR_Input.actionsByPath.Add(steamVR_Action.fullPath, steamVR_Action);
				SteamVR_Input.actionsByPathLowered.Add(steamVR_Action.fullPath.ToLower(), steamVR_Action);
			}
		}

		public static void Update()
		{
			if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
			{
				return;
			}
			if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnUpdate))
			{
				SteamVR_Input.UpdateNonVisualActions();
			}
			if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnUpdate))
			{
				SteamVR_Input.UpdateVisualActions(false);
			}
		}

		public static void LateUpdate()
		{
			if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
			{
				return;
			}
			if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
			{
				SteamVR_Input.UpdateNonVisualActions();
			}
			if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnLateUpdate))
			{
				SteamVR_Input.UpdateVisualActions(false);
				return;
			}
			SteamVR_Input.UpdateSkeletonActions(true);
		}

		public static void FixedUpdate()
		{
			if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
			{
				return;
			}
			if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
			{
				SteamVR_Input.UpdateNonVisualActions();
			}
			if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnFixedUpdate))
			{
				SteamVR_Input.UpdateVisualActions(false);
			}
		}

		public static void OnPreCull()
		{
			if (!SteamVR_Input.initialized || SteamVR_Input.isStartupFrame)
			{
				return;
			}
			if (SteamVR.settings.IsInputUpdateMode(SteamVR_UpdateModes.OnPreCull))
			{
				SteamVR_Input.UpdateNonVisualActions();
			}
			if (SteamVR.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
			{
				SteamVR_Input.UpdateVisualActions(false);
			}
		}

		public static void UpdateVisualActions(bool skipStateAndEventUpdates = false)
		{
			if (!SteamVR_Input.initialized)
			{
				return;
			}
			SteamVR_ActionSet_Manager.UpdateActionStates(false);
			SteamVR_Input.UpdatePoseActions(skipStateAndEventUpdates);
			SteamVR_Input.UpdateSkeletonActions(skipStateAndEventUpdates);
		}

		public static void UpdatePoseActions(bool skipSendingEvents = false)
		{
			if (!SteamVR_Input.initialized)
			{
				return;
			}
			for (int i = 0; i < SteamVR_Input.actionsPose.Length; i++)
			{
				SteamVR_Input.actionsPose[i].UpdateValues(skipSendingEvents);
			}
			if (SteamVR_Input.onPosesUpdated != null)
			{
				SteamVR_Input.onPosesUpdated(false);
			}
		}

		public static void UpdateSkeletonActions(bool skipSendingEvents = false)
		{
			if (!SteamVR_Input.initialized)
			{
				return;
			}
			for (int i = 0; i < SteamVR_Input.actionsSkeleton.Length; i++)
			{
				SteamVR_Input.actionsSkeleton[i].UpdateValue(skipSendingEvents);
			}
			if (SteamVR_Input.onSkeletonsUpdated != null)
			{
				SteamVR_Input.onSkeletonsUpdated(skipSendingEvents);
			}
		}

		public static void UpdateNonVisualActions()
		{
			if (!SteamVR_Input.initialized)
			{
				return;
			}
			SteamVR_ActionSet_Manager.UpdateActionStates(false);
			for (int i = 0; i < SteamVR_Input.actionsNonPoseNonSkeletonIn.Length; i++)
			{
				SteamVR_Input.actionsNonPoseNonSkeletonIn[i].UpdateValues();
			}
			if (SteamVR_Input.onNonVisualActionsUpdated != null)
			{
				SteamVR_Input.onNonVisualActionsUpdated();
			}
		}

		protected static void ShowBindingHintsForSets(VRActiveActionSet_t[] sets, ulong highlightAction = 0UL)
		{
			if (SteamVR_Input.sizeVRActiveActionSet_t == 0U)
			{
				SteamVR_Input.sizeVRActiveActionSet_t = (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t));
			}
			OpenVR.Input.ShowBindingsForActionSet(sets, SteamVR_Input.sizeVRActiveActionSet_t, highlightAction);
		}

		public static bool ShowBindingHints(ISteamVR_Action_In originToHighlight)
		{
			if (originToHighlight != null)
			{
				SteamVR_Input.setCache[0].ulActionSet = originToHighlight.actionSet.handle;
				SteamVR_Input.ShowBindingHintsForSets(SteamVR_Input.setCache, originToHighlight.activeOrigin);
				return true;
			}
			return false;
		}

		public static bool ShowBindingHints(ISteamVR_ActionSet setToShow)
		{
			if (setToShow != null)
			{
				SteamVR_Input.setCache[0].ulActionSet = setToShow.handle;
				SteamVR_Input.ShowBindingHintsForSets(SteamVR_Input.setCache, 0UL);
				return true;
			}
			return false;
		}

		public static void ShowBindingHintsForActiveActionSets(ulong highlightAction = 0UL)
		{
			if (SteamVR_Input.sizeVRActiveActionSet_t == 0U)
			{
				SteamVR_Input.sizeVRActiveActionSet_t = (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t));
			}
			OpenVR.Input.ShowBindingsForActionSet(SteamVR_ActionSet_Manager.rawActiveActionSetArray, SteamVR_Input.sizeVRActiveActionSet_t, highlightAction);
		}

		public static T GetActionDataFromPath<T>(string path, bool caseSensitive = false) where T : SteamVR_Action_Source_Map
		{
			SteamVR_Action baseActionFromPath = SteamVR_Input.GetBaseActionFromPath(path, caseSensitive);
			if (baseActionFromPath != null)
			{
				return (T)((object)baseActionFromPath.GetSourceMap());
			}
			return default(T);
		}

		public static SteamVR_ActionSet_Data GetActionSetDataFromPath(string path, bool caseSensitive = false)
		{
			SteamVR_ActionSet actionSetFromPath = SteamVR_Input.GetActionSetFromPath(path, caseSensitive);
			if (actionSetFromPath != null)
			{
				return actionSetFromPath.GetActionSetData();
			}
			return null;
		}

		public static T GetActionFromPath<T>(string path, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_Action, new()
		{
			SteamVR_Action baseActionFromPath = SteamVR_Input.GetBaseActionFromPath(path, caseSensitive);
			if (baseActionFromPath != null)
			{
				return baseActionFromPath.GetCopy<T>();
			}
			if (returnNulls)
			{
				return default(T);
			}
			return SteamVR_Input.CreateFakeAction<T>(path, caseSensitive);
		}

		public static SteamVR_Action GetBaseActionFromPath(string path, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			if (caseSensitive)
			{
				if (SteamVR_Input.actionsByPath.ContainsKey(path))
				{
					return SteamVR_Input.actionsByPath[path];
				}
			}
			else
			{
				if (SteamVR_Input.actionsByPathCache.ContainsKey(path))
				{
					return SteamVR_Input.actionsByPathCache[path];
				}
				if (SteamVR_Input.actionsByPath.ContainsKey(path))
				{
					SteamVR_Input.actionsByPathCache.Add(path, SteamVR_Input.actionsByPath[path]);
					return SteamVR_Input.actionsByPath[path];
				}
				string key = path.ToLower();
				if (SteamVR_Input.actionsByPathLowered.ContainsKey(key))
				{
					SteamVR_Input.actionsByPathCache.Add(path, SteamVR_Input.actionsByPathLowered[key]);
					return SteamVR_Input.actionsByPathLowered[key];
				}
				SteamVR_Input.actionsByPathCache.Add(path, null);
			}
			return null;
		}

		public static bool HasActionPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetBaseActionFromPath(path, caseSensitive) != null;
		}

		public static bool HasAction(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetBaseAction(null, actionName, caseSensitive) != null;
		}

		public static bool HasAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetBaseAction(actionSetName, actionName, caseSensitive) != null;
		}

		public static SteamVR_Action_Boolean GetBooleanActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Boolean>(path, caseSensitive, false);
		}

		public static SteamVR_Action_Single GetSingleActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Single>(path, caseSensitive, false);
		}

		public static SteamVR_Action_Vector2 GetVector2ActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Vector2>(path, caseSensitive, false);
		}

		public static SteamVR_Action_Vector3 GetVector3ActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Vector3>(path, caseSensitive, false);
		}

		public static SteamVR_Action_Vibration GetVibrationActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Vibration>(path, caseSensitive, false);
		}

		public static SteamVR_Action_Pose GetPoseActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Pose>(path, caseSensitive, false);
		}

		public static SteamVR_Action_Skeleton GetSkeletonActionFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionFromPath<SteamVR_Action_Skeleton>(path, caseSensitive, false);
		}

		public static T GetAction<T>(string actionSetName, string actionName, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_Action, new()
		{
			SteamVR_Action baseAction = SteamVR_Input.GetBaseAction(actionSetName, actionName, caseSensitive);
			if (baseAction != null)
			{
				return baseAction.GetCopy<T>();
			}
			if (returnNulls)
			{
				return default(T);
			}
			return SteamVR_Input.CreateFakeAction<T>(actionSetName, actionName, caseSensitive);
		}

		public static SteamVR_Action GetBaseAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			if (SteamVR_Input.actions == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(actionSetName))
			{
				for (int i = 0; i < SteamVR_Input.actions.Length; i++)
				{
					if (caseSensitive)
					{
						if (SteamVR_Input.actions[i].GetShortName() == actionName)
						{
							return SteamVR_Input.actions[i];
						}
					}
					else if (string.Equals(SteamVR_Input.actions[i].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
					{
						return SteamVR_Input.actions[i];
					}
				}
			}
			else
			{
				SteamVR_ActionSet actionSet = SteamVR_Input.GetActionSet(actionSetName, caseSensitive, true);
				if (actionSet != null)
				{
					for (int j = 0; j < actionSet.allActions.Length; j++)
					{
						if (caseSensitive)
						{
							if (actionSet.allActions[j].GetShortName() == actionName)
							{
								return actionSet.allActions[j];
							}
						}
						else if (string.Equals(actionSet.allActions[j].GetShortName(), actionName, StringComparison.CurrentCultureIgnoreCase))
						{
							return actionSet.allActions[j];
						}
					}
				}
			}
			return null;
		}

		private static T CreateFakeAction<T>(string actionSetName, string actionName, bool caseSensitive) where T : SteamVR_Action, new()
		{
			if (typeof(T) == typeof(SteamVR_Action_Vibration))
			{
				return SteamVR_Action.CreateUninitialized<T>(actionSetName, SteamVR_ActionDirections.Out, actionName, caseSensitive);
			}
			return SteamVR_Action.CreateUninitialized<T>(actionSetName, SteamVR_ActionDirections.In, actionName, caseSensitive);
		}

		private static T CreateFakeAction<T>(string actionPath, bool caseSensitive) where T : SteamVR_Action, new()
		{
			return SteamVR_Action.CreateUninitialized<T>(actionPath, caseSensitive);
		}

		public static T GetAction<T>(string actionName, bool caseSensitive = false) where T : SteamVR_Action, new()
		{
			return SteamVR_Input.GetAction<T>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Boolean GetBooleanAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Boolean GetBooleanAction(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Boolean>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Single GetSingleAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Single>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Single GetSingleAction(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Single>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Vector2 GetVector2Action(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Vector2>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Vector2 GetVector2Action(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Vector2>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Vector3 GetVector3Action(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Vector3>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Vector3 GetVector3Action(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Vector3>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Pose GetPoseAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Pose>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Pose GetPoseAction(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Pose>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Skeleton GetSkeletonAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Skeleton>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Skeleton GetSkeletonAction(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Skeleton>(null, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Vibration GetVibrationAction(string actionSetName, string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Vibration>(actionSetName, actionName, caseSensitive, false);
		}

		public static SteamVR_Action_Vibration GetVibrationAction(string actionName, bool caseSensitive = false)
		{
			return SteamVR_Input.GetAction<SteamVR_Action_Vibration>(null, actionName, caseSensitive, false);
		}

		public static T GetActionSet<T>(string actionSetName, bool caseSensitive = false, bool returnNulls = false) where T : SteamVR_ActionSet, new()
		{
			if (SteamVR_Input.actionSets == null)
			{
				if (returnNulls)
				{
					return default(T);
				}
				return SteamVR_ActionSet.CreateFromName<T>(actionSetName);
			}
			else
			{
				for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
				{
					if (caseSensitive)
					{
						if (SteamVR_Input.actionSets[i].GetShortName() == actionSetName)
						{
							return SteamVR_Input.actionSets[i].GetCopy<T>();
						}
					}
					else if (string.Equals(SteamVR_Input.actionSets[i].GetShortName(), actionSetName, StringComparison.CurrentCultureIgnoreCase))
					{
						return SteamVR_Input.actionSets[i].GetCopy<T>();
					}
				}
				if (returnNulls)
				{
					return default(T);
				}
				return SteamVR_ActionSet.CreateFromName<T>(actionSetName);
			}
		}

		public static SteamVR_ActionSet GetActionSet(string actionSetName, bool caseSensitive = false, bool returnsNulls = false)
		{
			return SteamVR_Input.GetActionSet<SteamVR_ActionSet>(actionSetName, caseSensitive, returnsNulls);
		}

		protected static bool HasActionSet(string name, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionSet(name, caseSensitive, true) != null;
		}

		public static T GetActionSetFromPath<T>(string path, bool caseSensitive = false, bool returnsNulls = false) where T : SteamVR_ActionSet, new()
		{
			if (SteamVR_Input.actionSets == null || SteamVR_Input.actionSets[0] == null || string.IsNullOrEmpty(path))
			{
				if (returnsNulls)
				{
					return default(T);
				}
				return SteamVR_ActionSet.Create<T>(path);
			}
			else
			{
				if (caseSensitive)
				{
					if (SteamVR_Input.actionSetsByPath.ContainsKey(path))
					{
						return SteamVR_Input.actionSetsByPath[path].GetCopy<T>();
					}
				}
				else if (SteamVR_Input.actionSetsByPathCache.ContainsKey(path))
				{
					SteamVR_ActionSet steamVR_ActionSet = SteamVR_Input.actionSetsByPathCache[path];
					if (steamVR_ActionSet == null)
					{
						return default(T);
					}
					return steamVR_ActionSet.GetCopy<T>();
				}
				else
				{
					if (SteamVR_Input.actionSetsByPath.ContainsKey(path))
					{
						SteamVR_Input.actionSetsByPathCache.Add(path, SteamVR_Input.actionSetsByPath[path]);
						return SteamVR_Input.actionSetsByPath[path].GetCopy<T>();
					}
					string key = path.ToLower();
					if (SteamVR_Input.actionSetsByPathLowered.ContainsKey(key))
					{
						SteamVR_Input.actionSetsByPathCache.Add(path, SteamVR_Input.actionSetsByPathLowered[key]);
						return SteamVR_Input.actionSetsByPath[key].GetCopy<T>();
					}
					SteamVR_Input.actionSetsByPathCache.Add(path, null);
				}
				if (returnsNulls)
				{
					return default(T);
				}
				return SteamVR_ActionSet.Create<T>(path);
			}
		}

		public static SteamVR_ActionSet GetActionSetFromPath(string path, bool caseSensitive = false)
		{
			return SteamVR_Input.GetActionSetFromPath<SteamVR_ActionSet>(path, caseSensitive, false);
		}

		public static bool GetState(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Boolean action2 = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive, false);
			return action2 != null && action2.GetState(inputSource);
		}

		public static bool GetState(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetState(null, action, inputSource, caseSensitive);
		}

		public static bool GetStateDown(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Boolean action2 = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive, false);
			return action2 != null && action2.GetStateDown(inputSource);
		}

		public static bool GetStateDown(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetStateDown(null, action, inputSource, caseSensitive);
		}

		public static bool GetStateUp(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Boolean action2 = SteamVR_Input.GetAction<SteamVR_Action_Boolean>(actionSet, action, caseSensitive, false);
			return action2 != null && action2.GetStateUp(inputSource);
		}

		public static bool GetStateUp(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetStateUp(null, action, inputSource, caseSensitive);
		}

		public static float GetFloat(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Single action2 = SteamVR_Input.GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive, false);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return 0f;
		}

		public static float GetFloat(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetFloat(null, action, inputSource, caseSensitive);
		}

		public static float GetSingle(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Single action2 = SteamVR_Input.GetAction<SteamVR_Action_Single>(actionSet, action, caseSensitive, false);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return 0f;
		}

		public static float GetSingle(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetFloat(null, action, inputSource, caseSensitive);
		}

		public static Vector2 GetVector2(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Vector2 action2 = SteamVR_Input.GetAction<SteamVR_Action_Vector2>(actionSet, action, caseSensitive, false);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return Vector2.zero;
		}

		public static Vector2 GetVector2(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetVector2(null, action, inputSource, caseSensitive);
		}

		public static Vector3 GetVector3(string actionSet, string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			SteamVR_Action_Vector3 action2 = SteamVR_Input.GetAction<SteamVR_Action_Vector3>(actionSet, action, caseSensitive, false);
			if (action2 != null)
			{
				return action2.GetAxis(inputSource);
			}
			return Vector3.zero;
		}

		public static Vector3 GetVector3(string action, SteamVR_Input_Sources inputSource, bool caseSensitive = false)
		{
			return SteamVR_Input.GetVector3(null, action, inputSource, caseSensitive);
		}

		public static SteamVR_ActionSet[] GetActionSets()
		{
			return SteamVR_Input.actionSets;
		}

		public static T[] GetActions<T>() where T : SteamVR_Action
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle == typeof(SteamVR_Action))
			{
				return SteamVR_Input.actions as T[];
			}
			if (typeFromHandle == typeof(ISteamVR_Action_In))
			{
				return SteamVR_Input.actionsIn as T[];
			}
			if (typeFromHandle == typeof(ISteamVR_Action_Out))
			{
				return SteamVR_Input.actionsOut as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Boolean))
			{
				return SteamVR_Input.actionsBoolean as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Single))
			{
				return SteamVR_Input.actionsSingle as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Vector2))
			{
				return SteamVR_Input.actionsVector2 as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Vector3))
			{
				return SteamVR_Input.actionsVector3 as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Pose))
			{
				return SteamVR_Input.actionsPose as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Skeleton))
			{
				return SteamVR_Input.actionsSkeleton as T[];
			}
			if (typeFromHandle == typeof(SteamVR_Action_Vibration))
			{
				return SteamVR_Input.actionsVibration as T[];
			}
			Debug.Log("<b>[SteamVR]</b> Wrong type.");
			return null;
		}

		internal static bool ShouldMakeCopy()
		{
			return !SteamVR_Behaviour.isPlaying;
		}

		public static string GetLocalizedName(ulong originHandle, params EVRInputStringBits[] localizedParts)
		{
			int num = 0;
			for (int i = 0; i < localizedParts.Length; i++)
			{
				num |= (int)localizedParts[i];
			}
			StringBuilder stringBuilder = new StringBuilder(500);
			OpenVR.Input.GetOriginLocalizedName(originHandle, stringBuilder, 500U, num);
			return stringBuilder.ToString();
		}

		public static bool CheckOldLocation()
		{
			return false;
		}

		public static void IdentifyActionsFile(bool showLogs = true)
		{
			string actionsFilePath = SteamVR_Input.GetActionsFilePath(true);
			if (File.Exists(actionsFilePath))
			{
				if (OpenVR.Input == null)
				{
					Debug.LogError("<b>[SteamVR]</b> Could not instantiate OpenVR Input interface.");
					return;
				}
				EVRInputError evrinputError = OpenVR.Input.SetActionManifestPath(actionsFilePath);
				if (evrinputError != EVRInputError.None)
				{
					Debug.LogError("<b>[SteamVR]</b> Error loading action manifest into SteamVR: " + evrinputError.ToString());
					return;
				}
				if (SteamVR_Input.actions != null)
				{
					int num = SteamVR_Input.actions.Length;
					if (showLogs)
					{
						Debug.Log(string.Format("<b>[SteamVR]</b> Successfully loaded {0} actions from action manifest into SteamVR ({1})", num, actionsFilePath));
						return;
					}
				}
				else if (showLogs)
				{
					Debug.LogWarning("<b>[SteamVR]</b> No actions found, but the action manifest was loaded. This usually means you haven't generated actions. Window -> SteamVR Input -> Save and Generate.");
					return;
				}
			}
			else if (showLogs)
			{
				Debug.LogError("<b>[SteamVR]</b> Could not find actions file at: " + actionsFilePath);
			}
		}

		public static bool HasFileInMemoryBeenModified()
		{
			string actionsFilePath = SteamVR_Input.GetActionsFilePath(true);
			if (File.Exists(actionsFilePath))
			{
				string usedString = File.ReadAllText(actionsFilePath);
				string badMD5Hash = SteamVR_Utils.GetBadMD5Hash(usedString);
				string badMD5Hash2 = SteamVR_Utils.GetBadMD5Hash(JsonConvert.SerializeObject(SteamVR_Input.actionFile, Formatting.Indented, new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				}));
				return badMD5Hash != badMD5Hash2;
			}
			return true;
		}

		public static bool CreateEmptyActionsFile(bool completelyEmpty = false)
		{
			string actionsFilePath = SteamVR_Input.GetActionsFilePath(true);
			if (File.Exists(actionsFilePath))
			{
				Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file already exists in project root: {0}", new object[]
				{
					actionsFilePath
				});
				return false;
			}
			SteamVR_Input.actionFile = new SteamVR_Input_ActionFile();
			if (!completelyEmpty)
			{
				SteamVR_Input.actionFile.action_sets.Add(SteamVR_Input_ActionFile_ActionSet.CreateNew());
				SteamVR_Input.actionFile.actions.Add(SteamVR_Input_ActionFile_Action.CreateNew(SteamVR_Input.actionFile.action_sets[0].shortName, SteamVR_ActionDirections.In, SteamVR_Input_ActionFile_ActionTypes.boolean));
			}
			string contents = JsonConvert.SerializeObject(SteamVR_Input.actionFile, Formatting.Indented, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
			File.WriteAllText(actionsFilePath, contents);
			SteamVR_Input.actionFile.InitializeHelperLists();
			SteamVR_Input.fileInitialized = true;
			return true;
		}

		public static bool DoesActionsFileExist()
		{
			return File.Exists(SteamVR_Input.GetActionsFilePath(true));
		}

		public static bool InitializeFile(bool force = false, bool showErrors = true)
		{
			bool flag = SteamVR_Input.DoesActionsFileExist();
			string actionsFilePath = SteamVR_Input.GetActionsFilePath(true);
			if (flag)
			{
				string usedString = File.ReadAllText(actionsFilePath);
				if (SteamVR_Input.fileInitialized || (SteamVR_Input.fileInitialized && !force))
				{
					string badMD5Hash = SteamVR_Utils.GetBadMD5Hash(usedString);
					if (badMD5Hash == SteamVR_Input.actionFileHash)
					{
						return true;
					}
					SteamVR_Input.actionFileHash = badMD5Hash;
				}
				SteamVR_Input.actionFile = SteamVR_Input_ActionFile.Open(SteamVR_Input.GetActionsFilePath(true));
				SteamVR_Input.fileInitialized = true;
				return true;
			}
			if (showErrors)
			{
				Debug.LogErrorFormat("<b>[SteamVR]</b> Actions file does not exist in project root: {0}", new object[]
				{
					actionsFilePath
				});
			}
			return false;
		}

		public static string GetActionsFileFolder(bool fullPath = true)
		{
			string streamingAssetsPath = Application.streamingAssetsPath;
			if (!Directory.Exists(streamingAssetsPath))
			{
				Directory.CreateDirectory(streamingAssetsPath);
			}
			string text = Path.Combine(streamingAssetsPath, "SteamVR");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static string GetActionsFilePath(bool fullPath = true)
		{
			return SteamVR_Utils.SanitizePath(Path.Combine(SteamVR_Input.GetActionsFileFolder(fullPath), SteamVR_Settings.instance.actionsFilePath), true);
		}

		public static string GetActionsFileName()
		{
			return SteamVR_Settings.instance.actionsFilePath;
		}

		public static bool DeleteManifestAndBindings()
		{
			if (!SteamVR_Input.DoesActionsFileExist())
			{
				return false;
			}
			SteamVR_Input.InitializeFile(false, true);
			foreach (string text in SteamVR_Input.actionFile.GetFilesToCopy(false))
			{
				new FileInfo(text).IsReadOnly = false;
				File.Delete(text);
			}
			string actionsFilePath = SteamVR_Input.GetActionsFilePath(true);
			if (File.Exists(actionsFilePath))
			{
				new FileInfo(actionsFilePath).IsReadOnly = false;
				File.Delete(actionsFilePath);
				SteamVR_Input.actionFile = null;
				SteamVR_Input.fileInitialized = false;
				return true;
			}
			return false;
		}

		public static void OpenBindingUI(SteamVR_ActionSet actionSetToEdit = null, SteamVR_Input_Sources deviceBindingToEdit = SteamVR_Input_Sources.Any)
		{
			ulong handle = SteamVR_Input_Source.GetHandle(deviceBindingToEdit);
			ulong ulActionSetHandle = 0UL;
			if (actionSetToEdit != null)
			{
				ulActionSetHandle = actionSetToEdit.handle;
			}
			OpenVR.Input.OpenBindingUI(null, ulActionSetHandle, handle, false);
		}

		public const string defaultInputGameObjectName = "[SteamVR Input]";

		private const string localizationKeyName = "localization";

		public static bool fileInitialized = false;

		public static bool initialized = false;

		public static bool preInitialized = false;

		public static SteamVR_Input_ActionFile actionFile;

		public static string actionFileHash;

		protected static bool initializing = false;

		protected static int startupFrame = 0;

		public static SteamVR_ActionSet[] actionSets;

		public static SteamVR_Action[] actions;

		public static ISteamVR_Action_In[] actionsIn;

		public static ISteamVR_Action_Out[] actionsOut;

		public static SteamVR_Action_Boolean[] actionsBoolean;

		public static SteamVR_Action_Single[] actionsSingle;

		public static SteamVR_Action_Vector2[] actionsVector2;

		public static SteamVR_Action_Vector3[] actionsVector3;

		public static SteamVR_Action_Pose[] actionsPose;

		public static SteamVR_Action_Skeleton[] actionsSkeleton;

		public static SteamVR_Action_Vibration[] actionsVibration;

		public static ISteamVR_Action_In[] actionsNonPoseNonSkeletonIn;

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPath = new Dictionary<string, SteamVR_ActionSet>();

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathLowered = new Dictionary<string, SteamVR_ActionSet>();

		protected static Dictionary<string, SteamVR_Action> actionsByPath = new Dictionary<string, SteamVR_Action>();

		protected static Dictionary<string, SteamVR_Action> actionsByPathLowered = new Dictionary<string, SteamVR_Action>();

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByPathCache = new Dictionary<string, SteamVR_ActionSet>();

		protected static Dictionary<string, SteamVR_Action> actionsByPathCache = new Dictionary<string, SteamVR_Action>();

		protected static Dictionary<string, SteamVR_Action> actionsByNameCache = new Dictionary<string, SteamVR_Action>();

		protected static Dictionary<string, SteamVR_ActionSet> actionSetsByNameCache = new Dictionary<string, SteamVR_ActionSet>();

		private static uint sizeVRActiveActionSet_t = 0U;

		private static VRActiveActionSet_t[] setCache = new VRActiveActionSet_t[1];

		public delegate void PosesUpdatedHandler(bool skipSendingEvents);

		public delegate void SkeletonsUpdatedHandler(bool skipSendingEvents);
	}
}
