using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public class PlayerLoopPruning : MonoBehaviour
{
	private void Start()
	{
		this.isAndroid = (Application.platform == RuntimePlatform.Android);
		PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
		PlayerLoop.SetPlayerLoop(this.RemoveSystem<PreLateUpdate>(currentPlayerLoop));
	}

	private PlayerLoopSystem RemoveSystem<T>(in PlayerLoopSystem loopSystem) where T : struct
	{
		PlayerLoopSystem result = new PlayerLoopSystem
		{
			loopConditionFunction = loopSystem.loopConditionFunction,
			type = loopSystem.type,
			updateDelegate = loopSystem.updateDelegate,
			updateFunction = loopSystem.updateFunction
		};
		List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
		if (loopSystem.subSystemList != null)
		{
			for (int i = 0; i < loopSystem.subSystemList.Length; i++)
			{
				PlayerLoopSystem playerLoopSystem = loopSystem.subSystemList[i];
				PlayerLoopSystem item = new PlayerLoopSystem
				{
					loopConditionFunction = playerLoopSystem.loopConditionFunction,
					type = playerLoopSystem.type,
					updateDelegate = playerLoopSystem.updateDelegate,
					updateFunction = playerLoopSystem.updateFunction
				};
				if (playerLoopSystem.subSystemList != null)
				{
					List<PlayerLoopSystem> list2 = new List<PlayerLoopSystem>();
					for (int j = 0; j < playerLoopSystem.subSystemList.Length; j++)
					{
						if (!this.removeSubsystemList.Contains(playerLoopSystem.subSystemList[j].type.Name) && (!this.isAndroid || !this.androidSubsystemExtras.Contains(playerLoopSystem.subSystemList[j].type.Name)))
						{
							list2.Add(playerLoopSystem.subSystemList[j]);
						}
					}
					item.subSystemList = list2.ToArray();
				}
				list.Add(item);
			}
		}
		PlayerLoopSystem item2 = new PlayerLoopSystem
		{
			type = typeof(PlayerLoopPruning),
			updateDelegate = new PlayerLoopSystem.UpdateFunction(PlayerLoopPruning.PhaseSyncDestroyer3000Start)
		};
		PlayerLoopSystem item3 = new PlayerLoopSystem
		{
			type = typeof(PlayerLoopPruning),
			updateDelegate = new PlayerLoopSystem.UpdateFunction(PlayerLoopPruning.PhaseSyncDestroyer3000End)
		};
		list.Insert(0, item2);
		list.Add(item3);
		result.subSystemList = list.ToArray();
		return result;
	}

	private static void PhaseSyncDestroyer3000Start()
	{
		PlayerLoopPruning.slop = (float)PlayerLoopPruning.sw.ElapsedTicks / 10000000f * 0.1f + PlayerLoopPruning.slop * 0.9f;
		PlayerLoopPruning.sw.Restart();
	}

	private static void PhaseSyncDestroyer3000End()
	{
		long elapsedTicks = PlayerLoopPruning.sw.ElapsedTicks;
		long num = (long)((1f / (float)Application.targetFrameRate - PlayerLoopPruning.slop) * 10000000f);
		long num2 = num - elapsedTicks;
		if (num2 < 0L)
		{
			PlayerLoopPruning.sw.Restart();
			return;
		}
		Thread.Sleep((int)(num2 / 10000L));
		while (PlayerLoopPruning.sw.ElapsedTicks < num)
		{
			Thread.Sleep(0);
		}
		PlayerLoopPruning.sw.Restart();
	}

	public List<string> removeSubsystemList;

	public List<string> androidSubsystemExtras;

	private bool isAndroid;

	private static Stopwatch sw = new Stopwatch();

	private static float slop = 0.0002f;
}
