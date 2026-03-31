using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Cysharp.Threading.Tasks
{
	public static class PlayerLoopHelper
	{
		public static SynchronizationContext UnitySynchronizationContext
		{
			get
			{
				return PlayerLoopHelper.unitySynchronizationContext;
			}
		}

		public static int MainThreadId
		{
			get
			{
				return PlayerLoopHelper.mainThreadId;
			}
		}

		internal static string ApplicationDataPath
		{
			get
			{
				return PlayerLoopHelper.applicationDataPath;
			}
		}

		public static bool IsMainThread
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId == PlayerLoopHelper.mainThreadId;
			}
		}

		internal static bool IsEditorApplicationQuitting { get; private set; }

		private static PlayerLoopSystem[] InsertRunner(PlayerLoopSystem loopSystem, bool injectOnFirst, Type loopRunnerYieldType, ContinuationQueue cq, Type loopRunnerType, PlayerLoopRunner runner)
		{
			PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
			{
				type = loopRunnerYieldType,
				updateDelegate = new PlayerLoopSystem.UpdateFunction(cq.Run)
			};
			PlayerLoopSystem playerLoopSystem2 = new PlayerLoopSystem
			{
				type = loopRunnerType,
				updateDelegate = new PlayerLoopSystem.UpdateFunction(runner.Run)
			};
			PlayerLoopSystem[] array = PlayerLoopHelper.RemoveRunner(loopSystem, loopRunnerYieldType, loopRunnerType);
			PlayerLoopSystem[] array2 = new PlayerLoopSystem[array.Length + 2];
			Array.Copy(array, 0, array2, injectOnFirst ? 2 : 0, array.Length);
			if (injectOnFirst)
			{
				array2[0] = playerLoopSystem;
				array2[1] = playerLoopSystem2;
			}
			else
			{
				array2[array2.Length - 2] = playerLoopSystem;
				array2[array2.Length - 1] = playerLoopSystem2;
			}
			return array2;
		}

		private static PlayerLoopSystem[] RemoveRunner(PlayerLoopSystem loopSystem, Type loopRunnerYieldType, Type loopRunnerType)
		{
			return (from ls in loopSystem.subSystemList
			where ls.type != loopRunnerYieldType && ls.type != loopRunnerType
			select ls).ToArray<PlayerLoopSystem>();
		}

		private static PlayerLoopSystem[] InsertUniTaskSynchronizationContext(PlayerLoopSystem loopSystem)
		{
			PlayerLoopSystem item = new PlayerLoopSystem
			{
				type = typeof(UniTaskSynchronizationContext),
				updateDelegate = new PlayerLoopSystem.UpdateFunction(UniTaskSynchronizationContext.Run)
			};
			List<PlayerLoopSystem> list = new List<PlayerLoopSystem>((from ls in loopSystem.subSystemList
			where ls.type != typeof(UniTaskSynchronizationContext)
			select ls).ToArray<PlayerLoopSystem>());
			int num = list.FindIndex((PlayerLoopSystem x) => x.type.Name == "ScriptRunDelayedTasks");
			if (num == -1)
			{
				num = list.FindIndex((PlayerLoopSystem x) => x.type.Name == "UniTaskLoopRunnerUpdate");
			}
			list.Insert(num + 1, item);
			return list.ToArray();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			PlayerLoopHelper.unitySynchronizationContext = SynchronizationContext.Current;
			PlayerLoopHelper.mainThreadId = Thread.CurrentThread.ManagedThreadId;
			try
			{
				PlayerLoopHelper.applicationDataPath = Application.dataPath;
			}
			catch
			{
			}
			if (PlayerLoopHelper.runners != null)
			{
				return;
			}
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			PlayerLoopHelper.Initialize(ref currentPlayerLoop, InjectPlayerLoopTimings.All);
		}

		private static int FindLoopSystemIndex(PlayerLoopSystem[] playerLoopList, Type systemType)
		{
			for (int i = 0; i < playerLoopList.Length; i++)
			{
				if (playerLoopList[i].type == systemType)
				{
					return i;
				}
			}
			throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
		}

		private static void InsertLoop(PlayerLoopSystem[] copyList, InjectPlayerLoopTimings injectTimings, Type loopType, InjectPlayerLoopTimings targetTimings, int index, bool injectOnFirst, Type loopRunnerYieldType, Type loopRunnerType, PlayerLoopTiming playerLoopTiming)
		{
			int num = PlayerLoopHelper.FindLoopSystemIndex(copyList, loopType);
			if ((injectTimings & targetTimings) == targetTimings)
			{
				copyList[num].subSystemList = PlayerLoopHelper.InsertRunner(copyList[num], injectOnFirst, loopRunnerYieldType, PlayerLoopHelper.yielders[index] = new ContinuationQueue(playerLoopTiming), loopRunnerType, PlayerLoopHelper.runners[index] = new PlayerLoopRunner(playerLoopTiming));
				return;
			}
			copyList[num].subSystemList = PlayerLoopHelper.RemoveRunner(copyList[num], loopRunnerYieldType, loopRunnerType);
		}

		public static void Initialize(ref PlayerLoopSystem playerLoop, InjectPlayerLoopTimings injectTimings = InjectPlayerLoopTimings.All)
		{
			PlayerLoopHelper.yielders = new ContinuationQueue[16];
			PlayerLoopHelper.runners = new PlayerLoopRunner[16];
			PlayerLoopSystem[] array = playerLoop.subSystemList.ToArray<PlayerLoopSystem>();
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(Initialization), InjectPlayerLoopTimings.Initialization, 0, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldInitialization), typeof(UniTaskLoopRunners.UniTaskLoopRunnerInitialization), PlayerLoopTiming.Initialization);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(Initialization), InjectPlayerLoopTimings.LastInitialization, 1, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldInitialization), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastInitialization), PlayerLoopTiming.LastInitialization);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(EarlyUpdate), InjectPlayerLoopTimings.EarlyUpdate, 2, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldEarlyUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerEarlyUpdate), PlayerLoopTiming.EarlyUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(EarlyUpdate), InjectPlayerLoopTimings.LastEarlyUpdate, 3, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldEarlyUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastEarlyUpdate), PlayerLoopTiming.LastEarlyUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(FixedUpdate), InjectPlayerLoopTimings.FixedUpdate, 4, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldFixedUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerFixedUpdate), PlayerLoopTiming.FixedUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(FixedUpdate), InjectPlayerLoopTimings.LastFixedUpdate, 5, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldFixedUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastFixedUpdate), PlayerLoopTiming.LastFixedUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(PreUpdate), InjectPlayerLoopTimings.PreUpdate, 6, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldPreUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerPreUpdate), PlayerLoopTiming.PreUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(PreUpdate), InjectPlayerLoopTimings.LastPreUpdate, 7, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldPreUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastPreUpdate), PlayerLoopTiming.LastPreUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(Update), InjectPlayerLoopTimings.Update, 8, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerUpdate), PlayerLoopTiming.Update);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(Update), InjectPlayerLoopTimings.LastUpdate, 9, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastUpdate), PlayerLoopTiming.LastUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(PreLateUpdate), InjectPlayerLoopTimings.PreLateUpdate, 10, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldPreLateUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerPreLateUpdate), PlayerLoopTiming.PreLateUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(PreLateUpdate), InjectPlayerLoopTimings.LastPreLateUpdate, 11, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldPreLateUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastPreLateUpdate), PlayerLoopTiming.LastPreLateUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(PostLateUpdate), InjectPlayerLoopTimings.PostLateUpdate, 12, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldPostLateUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerPostLateUpdate), PlayerLoopTiming.PostLateUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(PostLateUpdate), InjectPlayerLoopTimings.LastPostLateUpdate, 13, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldPostLateUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastPostLateUpdate), PlayerLoopTiming.LastPostLateUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(TimeUpdate), InjectPlayerLoopTimings.TimeUpdate, 14, true, typeof(UniTaskLoopRunners.UniTaskLoopRunnerYieldTimeUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerTimeUpdate), PlayerLoopTiming.TimeUpdate);
			PlayerLoopHelper.InsertLoop(array, injectTimings, typeof(TimeUpdate), InjectPlayerLoopTimings.LastTimeUpdate, 15, false, typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastYieldTimeUpdate), typeof(UniTaskLoopRunners.UniTaskLoopRunnerLastTimeUpdate), PlayerLoopTiming.LastTimeUpdate);
			int num = PlayerLoopHelper.FindLoopSystemIndex(array, typeof(Update));
			array[num].subSystemList = PlayerLoopHelper.InsertUniTaskSynchronizationContext(array[num]);
			playerLoop.subSystemList = array;
			PlayerLoop.SetPlayerLoop(playerLoop);
		}

		public static void AddAction(PlayerLoopTiming timing, IPlayerLoopItem action)
		{
			PlayerLoopRunner playerLoopRunner = PlayerLoopHelper.runners[(int)timing];
			if (playerLoopRunner == null)
			{
				PlayerLoopHelper.ThrowInvalidLoopTiming(timing);
			}
			playerLoopRunner.AddAction(action);
		}

		private static void ThrowInvalidLoopTiming(PlayerLoopTiming playerLoopTiming)
		{
			throw new InvalidOperationException("Target playerLoopTiming is not injected. Please check PlayerLoopHelper.Initialize. PlayerLoopTiming:" + playerLoopTiming.ToString());
		}

		public static void AddContinuation(PlayerLoopTiming timing, Action continuation)
		{
			ContinuationQueue continuationQueue = PlayerLoopHelper.yielders[(int)timing];
			if (continuationQueue == null)
			{
				PlayerLoopHelper.ThrowInvalidLoopTiming(timing);
			}
			continuationQueue.Enqueue(continuation);
		}

		public static void DumpCurrentPlayerLoop()
		{
			ref PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("PlayerLoop List");
			foreach (PlayerLoopSystem playerLoopSystem in currentPlayerLoop.subSystemList)
			{
				stringBuilder.AppendFormat("------{0}------", playerLoopSystem.type.Name);
				stringBuilder.AppendLine();
				if (playerLoopSystem.subSystemList == null)
				{
					stringBuilder.AppendFormat("{0} has no subsystems!", playerLoopSystem.ToString());
					stringBuilder.AppendLine();
				}
				else
				{
					foreach (PlayerLoopSystem playerLoopSystem2 in playerLoopSystem.subSystemList)
					{
						stringBuilder.AppendFormat("{0}", playerLoopSystem2.type.Name);
						stringBuilder.AppendLine();
						if (playerLoopSystem2.subSystemList != null)
						{
							Debug.LogWarning("More Subsystem:" + playerLoopSystem2.subSystemList.Length.ToString());
						}
					}
				}
			}
			Debug.Log(stringBuilder.ToString());
		}

		public static bool IsInjectedUniTaskPlayerLoop()
		{
			foreach (PlayerLoopSystem playerLoopSystem in PlayerLoop.GetCurrentPlayerLoop().subSystemList)
			{
				if (playerLoopSystem.subSystemList != null)
				{
					PlayerLoopSystem[] subSystemList2 = playerLoopSystem.subSystemList;
					for (int j = 0; j < subSystemList2.Length; j++)
					{
						if (subSystemList2[j].type == typeof(UniTaskLoopRunners.UniTaskLoopRunnerInitialization))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static readonly ContinuationQueue ThrowMarkerContinuationQueue = new ContinuationQueue(PlayerLoopTiming.Initialization);

		private static readonly PlayerLoopRunner ThrowMarkerPlayerLoopRunner = new PlayerLoopRunner(PlayerLoopTiming.Initialization);

		private static int mainThreadId;

		private static string applicationDataPath;

		private static SynchronizationContext unitySynchronizationContext;

		private static ContinuationQueue[] yielders;

		private static PlayerLoopRunner[] runners;
	}
}
