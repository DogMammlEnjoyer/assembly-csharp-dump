using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Fusion
{
	public sealed class NetworkRunnerUpdaterDefault : INetworkRunnerUpdater
	{
		[RuntimeInitializeOnLoadMethod]
		private static void ClearStatics()
		{
			NetworkRunnerUpdaterDefault._instances.Clear();
			NetworkRunnerUpdaterDefault._instanceCount = -1;
		}

		public static bool RegisterInPlayerLoop(NetworkRunnerUpdaterDefaultInvokeSettings updateSettings, NetworkRunnerUpdaterDefaultInvokeSettings renderSettings)
		{
			bool flag = NetworkRunnerUpdaterDefault._registration == null;
			bool result;
			if (flag)
			{
				NetworkRunnerUpdaterDefault._registration = new NetworkRunnerUpdaterDefault.PlayerLoopSystemRegistration(updateSettings, renderSettings);
				result = true;
			}
			else
			{
				bool flag2 = NetworkRunnerUpdaterDefault._registration.UpdateSettings != updateSettings || NetworkRunnerUpdaterDefault._registration.RenderSettings != renderSettings;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn("PlayerLoopSystemRegistration already exists with different settings (" + string.Format("Update: {0} vs {1}, ", NetworkRunnerUpdaterDefault._registration.UpdateSettings, updateSettings) + string.Format("Render: {0} vs {1}. ", NetworkRunnerUpdaterDefault._registration.RenderSettings, renderSettings) + "If you intend to change the timings, please call UnregisterFromPlayerLoop first.");
					}
				}
				result = false;
			}
			return result;
		}

		public static bool UnregisterFromPlayerLoop()
		{
			bool flag = NetworkRunnerUpdaterDefault._registration == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				NetworkRunnerUpdaterDefault._registration.Dispose();
				NetworkRunnerUpdaterDefault._registration = null;
				result = true;
			}
			return result;
		}

		void INetworkRunnerUpdater.Initialize(NetworkRunner runner)
		{
			DebugLogStream logDebug = InternalLogStreams.LogDebug;
			if (logDebug != null)
			{
				logDebug.Log(runner, "Adding to the default updater");
			}
			NetworkRunnerUpdaterDefault.RegisterInPlayerLoop(this.UpdateSettings, this.RenderSettings);
			NetworkRunnerUpdaterDefault._instances.Add(runner);
		}

		void INetworkRunnerUpdater.Shutdown(NetworkRunner runner)
		{
			int num = NetworkRunnerUpdaterDefault._instances.IndexOf(runner);
			bool flag = num < 0;
			if (!flag)
			{
				bool flag2 = NetworkRunnerUpdaterDefault._instanceCount >= 0;
				if (flag2)
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Log(NetworkRunnerUpdaterDefault._instances[num], "Removing from the default updater");
					}
					NetworkRunnerUpdaterDefault._instances[num] = null;
				}
				else
				{
					DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
					if (logDebug2 != null)
					{
						logDebug2.Log(NetworkRunnerUpdaterDefault._instances[num], "Removing from the default updater (deferred)");
					}
					NetworkRunnerUpdaterDefault._instances.RemoveAt(num);
				}
			}
		}

		private static void InvokeUpdate()
		{
			Assert.Check<int>(NetworkRunnerUpdaterDefault._instanceCount < 0, "Expected _instanceCount being negative {0}", NetworkRunnerUpdaterDefault._instanceCount);
			NetworkRunnerUpdaterDefault._instanceCount = NetworkRunnerUpdaterDefault._instances.Count;
			for (int i = 0; i < NetworkRunnerUpdaterDefault._instanceCount; i++)
			{
				NetworkRunner networkRunner = NetworkRunnerUpdaterDefault._instances[i];
				bool flag = BehaviourUtils.IsAlive(networkRunner);
				if (flag)
				{
					SimulationConfig.SimulationTimeMode simulationUpdateTimeMode = networkRunner._simulation.Config.SimulationUpdateTimeMode;
					SimulationConfig.SimulationTimeMode simulationTimeMode = simulationUpdateTimeMode;
					float num;
					if (simulationTimeMode != SimulationConfig.SimulationTimeMode.UnscaledDeltaTime)
					{
						if (simulationTimeMode != SimulationConfig.SimulationTimeMode.DeltaTime)
						{
							throw new NotImplementedException();
						}
						num = Time.deltaTime;
					}
					else
					{
						num = Time.unscaledDeltaTime;
					}
					networkRunner.UpdateInternal((double)num);
				}
			}
		}

		private static void InvokeRender()
		{
			Assert.Check<int>(NetworkRunnerUpdaterDefault._instanceCount >= 0, "Expected _instanceCount not being negative {0}", NetworkRunnerUpdaterDefault._instanceCount);
			Assert.Check(NetworkRunnerUpdaterDefault._instanceCount <= NetworkRunnerUpdaterDefault._instances.Count);
			bool flag = false;
			try
			{
				for (int i = 0; i < NetworkRunnerUpdaterDefault._instanceCount; i++)
				{
					NetworkRunner networkRunner = NetworkRunnerUpdaterDefault._instances[i];
					bool flag2 = BehaviourUtils.IsAlive(networkRunner);
					if (flag2)
					{
						networkRunner.RenderInternal();
					}
					else
					{
						flag = true;
					}
				}
			}
			finally
			{
				NetworkRunnerUpdaterDefault._instanceCount = -1;
				bool flag3 = flag;
				if (flag3)
				{
					NetworkRunnerUpdaterDefault._instances.RemoveAll((NetworkRunner x) => BehaviourUtils.IsNotAlive(x));
				}
			}
		}

		private static List<NetworkRunner> _instances = new List<NetworkRunner>();

		private static int _instanceCount = -1;

		private static NetworkRunnerUpdaterDefault.PlayerLoopSystemRegistration _registration;

		public NetworkRunnerUpdaterDefaultInvokeSettings UpdateSettings = new NetworkRunnerUpdaterDefaultInvokeSettings
		{
			ReferencePlayerLoopSystem = typeof(Update.ScriptRunBehaviourUpdate),
			AddMode = UnityPlayerLoopSystemAddMode.Before
		};

		public NetworkRunnerUpdaterDefaultInvokeSettings RenderSettings = new NetworkRunnerUpdaterDefaultInvokeSettings
		{
			ReferencePlayerLoopSystem = typeof(Update.ScriptRunBehaviourUpdate),
			AddMode = UnityPlayerLoopSystemAddMode.After
		};

		public struct NetworkRunnerUpdate
		{
		}

		public struct NetworkRunnerRender
		{
		}

		private class PlayerLoopSystemRegistration : IDisposable
		{
			public PlayerLoopSystemRegistration(NetworkRunnerUpdaterDefaultInvokeSettings updateSettings, NetworkRunnerUpdaterDefaultInvokeSettings renderSettings)
			{
				this.UpdateSettings = updateSettings;
				this.RenderSettings = renderSettings;
				bool flag = this.UpdateSettings.ReferencePlayerLoopSystem == null;
				if (flag)
				{
					throw new ArgumentException("UpdateSettings");
				}
				bool flag2 = this.RenderSettings.ReferencePlayerLoopSystem == null;
				if (flag2)
				{
					throw new ArgumentException("RenderSettings");
				}
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log(string.Format("Registering in PlayerLoop, Update:{0} Render:{1}", this.UpdateSettings, this.RenderSettings));
				}
				PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
				Type referencePlayerLoopSystem = this.UpdateSettings.ReferencePlayerLoopSystem;
				UnityPlayerLoopSystemAddMode addMode = this.UpdateSettings.AddMode;
				Type typeFromHandle = typeof(NetworkRunnerUpdaterDefault.NetworkRunnerUpdate);
				PlayerLoopSystem.UpdateFunction updateDelegate;
				if ((updateDelegate = NetworkRunnerUpdaterDefault.PlayerLoopSystemRegistration.<>O.<0>__InvokeUpdate) == null)
				{
					updateDelegate = (NetworkRunnerUpdaterDefault.PlayerLoopSystemRegistration.<>O.<0>__InvokeUpdate = new PlayerLoopSystem.UpdateFunction(NetworkRunnerUpdaterDefault.InvokeUpdate));
				}
				UnityPlayerLoopSystemUtils.AddToPlayerLoop(ref currentPlayerLoop, referencePlayerLoopSystem, addMode, typeFromHandle, updateDelegate);
				Type referencePlayerLoopSystem2 = this.RenderSettings.ReferencePlayerLoopSystem;
				UnityPlayerLoopSystemAddMode addMode2 = this.RenderSettings.AddMode;
				Type typeFromHandle2 = typeof(NetworkRunnerUpdaterDefault.NetworkRunnerRender);
				PlayerLoopSystem.UpdateFunction updateDelegate2;
				if ((updateDelegate2 = NetworkRunnerUpdaterDefault.PlayerLoopSystemRegistration.<>O.<1>__InvokeRender) == null)
				{
					updateDelegate2 = (NetworkRunnerUpdaterDefault.PlayerLoopSystemRegistration.<>O.<1>__InvokeRender = new PlayerLoopSystem.UpdateFunction(NetworkRunnerUpdaterDefault.InvokeRender));
				}
				UnityPlayerLoopSystemUtils.AddToPlayerLoop(ref currentPlayerLoop, referencePlayerLoopSystem2, addMode2, typeFromHandle2, updateDelegate2);
				PlayerLoop.SetPlayerLoop(currentPlayerLoop);
			}

			public void Dispose()
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log("Unregistering from PlayerLoop");
				}
				PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
				UnityPlayerLoopSystemUtils.RemoveFromPlayerLoop(ref currentPlayerLoop, typeof(NetworkRunnerUpdaterDefault.NetworkRunnerUpdate));
				UnityPlayerLoopSystemUtils.RemoveFromPlayerLoop(ref currentPlayerLoop, typeof(NetworkRunnerUpdaterDefault.NetworkRunnerRender));
				PlayerLoop.SetPlayerLoop(currentPlayerLoop);
			}

			public NetworkRunnerUpdaterDefaultInvokeSettings UpdateSettings;

			public NetworkRunnerUpdaterDefaultInvokeSettings RenderSettings;

			[CompilerGenerated]
			private static class <>O
			{
				public static PlayerLoopSystem.UpdateFunction <0>__InvokeUpdate;

				public static PlayerLoopSystem.UpdateFunction <1>__InvokeRender;
			}
		}
	}
}
