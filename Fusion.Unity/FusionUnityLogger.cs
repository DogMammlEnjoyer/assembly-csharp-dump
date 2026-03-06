using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Fusion
{
	public class FusionUnityLogger : FusionUnityLoggerBase
	{
		public FusionUnityLogger(Thread mainThread, bool isDarkMode) : base(mainThread, isDarkMode)
		{
		}

		protected override ValueTuple<string, Object> CreateMessage(in FusionUnityLoggerBase.LogContext context)
		{
			bool flag;
			StringBuilder threadSafeStringBuilder = base.GetThreadSafeStringBuilder(out flag);
			ILogSource source = context.Source;
			Object @object = (source != null) ? source.GetUnityObject() : null;
			ValueTuple<string, Object> result;
			try
			{
				base.AppendPrefix(threadSafeStringBuilder, context.Flags, context.Prefix);
				int length = threadSafeStringBuilder.Length;
				if (@object != null)
				{
					NetworkRunner networkRunner = @object as NetworkRunner;
					if (networkRunner != null)
					{
						this.TryAppendRunnerPrefix(threadSafeStringBuilder, networkRunner);
					}
					else
					{
						NetworkObject networkObject = @object as NetworkObject;
						if (networkObject != null)
						{
							this.TryAppendNetworkObjectPrefix(threadSafeStringBuilder, networkObject);
						}
						else
						{
							SimulationBehaviour simulationBehaviour = @object as SimulationBehaviour;
							if (simulationBehaviour != null)
							{
								this.TryAppendSimulationBehaviourPrefix(threadSafeStringBuilder, simulationBehaviour);
							}
							else
							{
								base.AppendNameThreadSafe(threadSafeStringBuilder, @object);
							}
						}
					}
				}
				if (this.LogActiveRunnerTick)
				{
					List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
					while (instancesEnumerator.MoveNext())
					{
						NetworkRunner networkRunner2 = instancesEnumerator.Current;
						if (!(networkRunner2 == null) && networkRunner2.IsSimulationUpdating)
						{
							threadSafeStringBuilder.Append(string.Format("[Tick {0}{1}{2}] ", networkRunner2.Tick, networkRunner2.IsFirstTick ? "F" : "", (networkRunner2.Stage == (SimulationStages)0) ? "" : string.Format(" {0}", networkRunner2.Stage)));
						}
					}
				}
				if (threadSafeStringBuilder.Length > length)
				{
					threadSafeStringBuilder.Append(": ");
				}
				threadSafeStringBuilder.Append(context.Message);
				result = new ValueTuple<string, Object>(threadSafeStringBuilder.ToString(), flag ? @object : null);
			}
			finally
			{
				threadSafeStringBuilder.Clear();
			}
			return result;
		}

		private bool TryAppendRunnerPrefix(StringBuilder builder, NetworkRunner runner)
		{
			if (runner == null)
			{
				return false;
			}
			NetworkProjectConfig config = runner.Config;
			if (config == null || config.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
			{
				return false;
			}
			base.AppendNameThreadSafe(builder, runner);
			PlayerRef localPlayer = runner.LocalPlayer;
			if (localPlayer.IsRealPlayer)
			{
				builder.Append("[P").Append(localPlayer.PlayerId).Append("]");
			}
			else
			{
				builder.Append("[P-]");
			}
			return true;
		}

		private bool TryAppendNetworkObjectPrefix(StringBuilder builder, NetworkObject networkObject)
		{
			if (networkObject == null)
			{
				return false;
			}
			base.AppendNameThreadSafe(builder, networkObject);
			if (networkObject.Id.IsValid)
			{
				builder.Append(" ");
				builder.Append(networkObject.Id.ToString());
			}
			int length = builder.Length;
			if (this.TryAppendRunnerPrefix(builder, networkObject.Runner))
			{
				builder.Insert(length, '@');
			}
			return true;
		}

		private bool TryAppendSimulationBehaviourPrefix(StringBuilder builder, SimulationBehaviour simulationBehaviour)
		{
			if (simulationBehaviour == null)
			{
				return false;
			}
			base.AppendNameThreadSafe(builder, simulationBehaviour);
			NetworkBehaviour networkBehaviour = simulationBehaviour as NetworkBehaviour;
			if (networkBehaviour != null && networkBehaviour.Id.IsValid)
			{
				builder.Append(" ");
				builder.Append(networkBehaviour.Id.ToString());
			}
			int length = builder.Length;
			if (this.TryAppendRunnerPrefix(builder, simulationBehaviour.Runner))
			{
				builder.Insert(length, '@');
			}
			return true;
		}

		public bool LogActiveRunnerTick;
	}
}
