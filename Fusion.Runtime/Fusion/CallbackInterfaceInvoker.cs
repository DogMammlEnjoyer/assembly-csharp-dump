using System;

namespace Fusion
{
	internal static class CallbackInterfaceInvoker
	{
		public static void IBeforeCopyPreviousState(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeCopyPreviousState));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeCopyPreviousState), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeCopyPreviousState)simulationBehaviour).BeforeCopyPreviousState();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeClientPredictionReset(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeClientPredictionReset));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeClientPredictionReset), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeClientPredictionReset)simulationBehaviour).BeforeClientPredictionReset();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterClientPredictionReset(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterClientPredictionReset));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterClientPredictionReset), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IAfterClientPredictionReset)simulationBehaviour).AfterClientPredictionReset();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeUpdateRemotePrefabs(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeUpdateRemotePrefabs));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeUpdateRemotePrefabs), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeUpdateRemotePrefabs)simulationBehaviour).BeforeUpdateRemotePrefabs();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterUpdateRemotePrefabs(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterUpdateRemotePrefabs));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterUpdateRemotePrefabs), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IAfterUpdateRemotePrefabs)simulationBehaviour).AfterUpdateRemotePrefabs();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeTick(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeTick));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeTick), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeTick)simulationBehaviour).BeforeTick();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterTick(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterTick));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterTick), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IAfterTick)simulationBehaviour).AfterTick();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeAllTicks(SimulationBehaviourUpdater updater, bool resimulation, int tickCount)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeAllTicks));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeAllTicks), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeAllTicks)simulationBehaviour).BeforeAllTicks(resimulation, tickCount);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterAllTicks(SimulationBehaviourUpdater updater, bool resimulation, int tickCount)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterAllTicks));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterAllTicks), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IAfterAllTicks)simulationBehaviour).AfterAllTicks(resimulation, tickCount);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeSimulation(SimulationBehaviourUpdater updater, int forwardTickCount)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeSimulation));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeSimulation), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeSimulation)simulationBehaviour).BeforeSimulation(forwardTickCount);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IPlayerJoined(SimulationBehaviourUpdater updater, PlayerRef player)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IPlayerJoined));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IPlayerJoined), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IPlayerJoined)simulationBehaviour).PlayerJoined(player);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IPlayerLeft(SimulationBehaviourUpdater updater, PlayerRef player)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IPlayerLeft));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IPlayerLeft), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IPlayerLeft)simulationBehaviour).PlayerLeft(player);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeHitboxRegistration(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeHitboxRegistration));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeHitboxRegistration), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeHitboxRegistration)simulationBehaviour).BeforeHitboxRegistration();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterUpdate(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterUpdate));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterUpdate), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IAfterUpdate)simulationBehaviour).AfterUpdate();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IBeforeUpdate(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IBeforeUpdate));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IBeforeUpdate), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IBeforeUpdate)simulationBehaviour).BeforeUpdate();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterRender(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterRender));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterRender), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveRenderCallback = simulationBehaviour.CanReceiveRenderCallback;
							if (canReceiveRenderCallback)
							{
								((IAfterRender)simulationBehaviour).AfterRender();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void ISceneLoadDone(SimulationBehaviourUpdater updater, in SceneLoadDoneArgs sceneLoadDoneArgs)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(ISceneLoadDone));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(ISceneLoadDone), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((ISceneLoadDone)simulationBehaviour).SceneLoadDone(sceneLoadDoneArgs);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void ISceneLoadStart(SimulationBehaviourUpdater updater, SceneRef sceneRef)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(ISceneLoadStart));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(ISceneLoadStart), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((ISceneLoadStart)simulationBehaviour).SceneLoadStart(sceneRef);
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}

		public static void IAfterHostMigration(SimulationBehaviourUpdater updater)
		{
			try
			{
				int callbackCount = updater.GetCallbackCount(typeof(IAfterHostMigration));
				for (int i = 0; i < callbackCount; i++)
				{
					SimulationBehaviour simulationBehaviour;
					using (updater.GetCallbackHead(typeof(IAfterHostMigration), i, out simulationBehaviour))
					{
						while (BehaviourUtils.IsNotNull(simulationBehaviour))
						{
							SimulationBehaviour next = simulationBehaviour.Next;
							bool canReceiveSimulationCallback = simulationBehaviour.CanReceiveSimulationCallback;
							if (canReceiveSimulationCallback)
							{
								((IAfterHostMigration)simulationBehaviour).AfterHostMigration();
							}
							simulationBehaviour = next;
						}
					}
				}
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
		}
	}
}
