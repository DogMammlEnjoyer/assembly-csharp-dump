using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion
{
	[ScriptHelp(BackColor = ScriptHeaderBackColor.Green)]
	[HelpURL("https://doc.photonengine.com/fusion/current/manual/network-object#simulationbehaviour")]
	public abstract class SimulationBehaviour : Behaviour
	{
		public bool CanReceiveRenderCallback
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.Flags & (SimulationBehaviourRuntimeFlags.PendingRemoval | SimulationBehaviourRuntimeFlags.IsUnityDestroyed | SimulationBehaviourRuntimeFlags.IsUnityDisabled)) == (SimulationBehaviourRuntimeFlags)0;
			}
		}

		public bool CanReceiveSimulationCallback
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.Flags & (SimulationBehaviourRuntimeFlags.PendingRemoval | SimulationBehaviourRuntimeFlags.IsUnityDestroyed | SimulationBehaviourRuntimeFlags.IsUnityDisabled)) == (SimulationBehaviourRuntimeFlags)0 && (this.Flags & (SimulationBehaviourRuntimeFlags.IsGlobal | SimulationBehaviourRuntimeFlags.InSimulation)) > (SimulationBehaviourRuntimeFlags)0;
			}
		}

		public NetworkRunner Runner
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._runner;
			}
		}

		public NetworkObject Object
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._object;
			}
		}

		public virtual void FixedUpdateNetwork()
		{
		}

		internal virtual void PreRender()
		{
		}

		public virtual void Render()
		{
		}

		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
		}

		private void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
		}

		private void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void MakeOwned(NetworkRunner runner, NetworkObject obj)
		{
			this._runner = runner;
			this._object = obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void MakeUnowned()
		{
			this._runner = null;
			this._object = null;
		}

		[Conditional("DEBUG")]
		internal void DebugNotifySpawned()
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(this, "Spawned");
			}
		}

		[Conditional("DEBUG")]
		internal void DebugNotifyDespawned()
		{
			TraceLogStream logTraceObject = InternalLogStreams.LogTraceObject;
			if (logTraceObject != null)
			{
				logTraceObject.Log(this, "Despawned");
			}
		}

		protected internal override void GetDumpString(StringBuilder builder)
		{
			builder.Append("[");
			builder.Append(base.DebugNameThreadSafe);
			NetworkBehaviour networkBehaviour = this as NetworkBehaviour;
			bool flag = networkBehaviour != null && networkBehaviour.Id.IsValid;
			if (flag)
			{
				builder.Append(" ");
				builder.Append(networkBehaviour.Id.ToString());
			}
			int length = builder.Length;
			bool flag2 = NetworkRunner.TryGetPrettyRunnerName(builder, this.Runner);
			if (flag2)
			{
				builder.Insert(length, "@");
			}
			builder.Append("]");
		}

		[NonSerialized]
		internal SimulationBehaviour Prev;

		[NonSerialized]
		internal SimulationBehaviour Next;

		[NonSerialized]
		internal SimulationBehaviourRuntimeFlags Flags = SimulationBehaviourRuntimeFlags.IsUnityDisabled;

		private NetworkRunner _runner;

		private NetworkObject _object;
	}
}
