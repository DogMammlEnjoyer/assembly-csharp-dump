using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Liv.Lck
{
	internal static class LckUpdateManager
	{
		public static void RegisterSingleEarlyUpdate(ILckEarlyUpdate earlyUpdateSystem)
		{
			if (LckUpdateManager._earlyUpdateSystem != null)
			{
				LckLog.LogWarning(string.Format("LCK EarlyUpdateSystem already has a reference ({0}). Note only one system is supported.", LckUpdateManager._earlyUpdateSystem), "RegisterSingleEarlyUpdate", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckUpdateManager.cs", 25);
			}
			LckUpdateManager._earlyUpdateSystem = earlyUpdateSystem;
		}

		public static void UnregisterSingleEarlyUpdate(ILckEarlyUpdate earlyUpdateSystem)
		{
			if (LckUpdateManager._earlyUpdateSystem == earlyUpdateSystem)
			{
				LckUpdateManager._earlyUpdateSystem = null;
			}
		}

		public static void RegisterSingleLateUpdate(ILckLateUpdate lateUpdateSystem)
		{
			if (LckUpdateManager._lateUpdateSystem != null)
			{
				LckLog.LogWarning(string.Format("LCK LateUpdateSystem already has a reference ({0}). Note only one system is supported.", LckUpdateManager._lateUpdateSystem), "RegisterSingleLateUpdate", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckUpdateManager.cs", 48);
			}
			LckUpdateManager._lateUpdateSystem = lateUpdateSystem;
		}

		public static void UnregisterSingleLateUpdate(ILckLateUpdate lateUpdateSystem)
		{
			if (LckUpdateManager._lateUpdateSystem == lateUpdateSystem)
			{
				LckUpdateManager._lateUpdateSystem = null;
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			PlayerLoopSystem systemToAdd = new PlayerLoopSystem
			{
				subSystemList = null,
				updateDelegate = new PlayerLoopSystem.UpdateFunction(LckUpdateManager.OnEarlyUpdate),
				type = typeof(LckEarlyUpdate)
			};
			PlayerLoopSystem systemToAdd2 = new PlayerLoopSystem
			{
				subSystemList = null,
				updateDelegate = new PlayerLoopSystem.UpdateFunction(LckUpdateManager.OnLateUpdate),
				type = typeof(LckLateUpdate)
			};
			PlayerLoopSystem playerLoopSystem = LckUpdateManager.AddSystem<EarlyUpdate>(currentPlayerLoop, systemToAdd);
			PlayerLoop.SetPlayerLoop(LckUpdateManager.AddSystem<PostLateUpdate>(playerLoopSystem, systemToAdd2));
		}

		private static PlayerLoopSystem AddSystem<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem systemToAdd) where T : struct
		{
			PlayerLoopSystem result = new PlayerLoopSystem
			{
				loopConditionFunction = loopSystem.loopConditionFunction,
				type = loopSystem.type,
				updateDelegate = loopSystem.updateDelegate,
				updateFunction = loopSystem.updateFunction
			};
			Type typeFromHandle = typeof(T);
			PlayerLoopSystem[] subSystemList = loopSystem.subSystemList;
			List<PlayerLoopSystem> list = new List<PlayerLoopSystem>((subSystemList != null) ? subSystemList.Length : 0);
			foreach (PlayerLoopSystem playerLoopSystem in loopSystem.subSystemList)
			{
				list.Add(playerLoopSystem);
				if (playerLoopSystem.type == typeFromHandle)
				{
					list.Add(systemToAdd);
				}
			}
			result.subSystemList = list.ToArray();
			return result;
		}

		private static void OnEarlyUpdate()
		{
			ILckEarlyUpdate earlyUpdateSystem = LckUpdateManager._earlyUpdateSystem;
			if (earlyUpdateSystem == null)
			{
				return;
			}
			earlyUpdateSystem.EarlyUpdate();
		}

		private static void OnLateUpdate()
		{
			ILckLateUpdate lateUpdateSystem = LckUpdateManager._lateUpdateSystem;
			if (lateUpdateSystem == null)
			{
				return;
			}
			lateUpdateSystem.LateUpdate();
		}

		private static ILckEarlyUpdate _earlyUpdateSystem;

		private static ILckLateUpdate _lateUpdateSystem;
	}
}
