using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion
{
	[DisallowMultipleComponent]
	[NetworkBehaviourWeaved(14)]
	public sealed class NetworkTransform : NetworkTRSP, INetworkTRSPTeleport, IBeforeAllTicks, IPublicFacingInterface, IAfterAllTicks, IBeforeCopyPreviousState
	{
		public bool AutoUpdateAreaOfInterestOverride
		{
			get
			{
				return this._autoAOIOverride;
			}
			set
			{
				this._aoiAutoUpdateOriginal = value;
				this._autoAOIOverride = value;
			}
		}

		private void Awake()
		{
			this._aoiAutoUpdateOriginal = this._autoAOIOverride;
			base.TryGetComponent<Transform>(out this._transform);
		}

		private void CopyToEngine()
		{
			bool flag = base.IsMainTRSP && this.SyncParent;
			if (flag)
			{
				NetworkTRSP.SetParentTransform(this, this._transform, base.State.Parent);
			}
			this._transform.localPosition = base.State.Position;
			this._transform.localRotation = base.State.Rotation;
			bool syncScale = this.SyncScale;
			if (syncScale)
			{
				this._transform.localScale = base.State.Scale;
			}
		}

		private void CopyToBuffer()
		{
			Transform transform = this._transform;
			base.State.Position = transform.localPosition;
			base.State.Rotation = transform.localRotation;
			bool syncScale = this.SyncScale;
			if (syncScale)
			{
				base.State.Scale = transform.localScale;
			}
			bool isMainTRSP = base.IsMainTRSP;
			if (isMainTRSP)
			{
				Transform parent = transform.parent;
				bool flag = parent;
				bool flag2 = flag && this._aoiEnabled && this._autoAOIOverride;
				if (flag2)
				{
					NetworkTRSP.ResolveAOIOverride(this, parent);
				}
				else
				{
					this.SetAreaOfInterestOverride(null);
				}
				bool syncParent = this.SyncParent;
				if (syncParent)
				{
					bool flag3 = flag;
					if (flag3)
					{
						NetworkBehaviour behaviour;
						bool flag4 = parent.TryGetComponent<NetworkBehaviour>(out behaviour);
						if (flag4)
						{
							base.State.Parent = behaviour;
						}
						else
						{
							base.State.Parent = NetworkTRSPData.NonNetworkedParent;
						}
					}
					else
					{
						base.State.Parent = default(NetworkBehaviourId);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool CanInterpolate()
		{
			bool flag = base.Runner.Mode == SimulationModes.Server || (this.DisableSharedModeInterpolation && ((base.Runner.Topology == Topologies.Shared && base.HasStateAuthority) || base.Runner.GameMode == GameMode.Single));
			return !flag;
		}

		void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
		{
			bool flag = !this.CanInterpolate();
			if (!flag)
			{
				if (resimulation)
				{
					this.CopyToEngine();
				}
				else
				{
					bool flag2 = this._render && this._simulation;
					if (flag2)
					{
						bool flag3 = base.IsMainTRSP && this.SyncParent && base.transform.parent == this._renderParent;
						if (flag3)
						{
							NetworkTRSP.SetParentTransform(this, this._transform, base.State.Parent);
						}
						bool flag4 = this._transform.localPosition == this._renderPosition && this._transform.localRotation == this._renderRotation;
						if (flag4)
						{
							this._transform.localPosition = base.State.Position;
							this._transform.localRotation = base.State.Rotation;
						}
					}
					this._render = false;
					this._simulation = false;
				}
			}
		}

		void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
		{
			this.CopyToBuffer();
			this._simulation = true;
		}

		void IBeforeCopyPreviousState.BeforeCopyPreviousState()
		{
			this.CopyToBuffer();
		}

		public void Teleport(Vector3? position = null, Quaternion? rotation = null)
		{
			NetworkTRSP.Teleport(this, this._transform, position, rotation);
		}

		public override void SetAreaOfInterestOverride(NetworkObject obj)
		{
			base.SetAreaOfInterestOverride(obj);
			bool flag = obj;
			if (flag)
			{
				this._autoAOIOverride = false;
			}
			else
			{
				this._autoAOIOverride = this._aoiAutoUpdateOriginal;
			}
		}

		public override void Spawned()
		{
			bool flag = !this._transform;
			if (flag)
			{
				this.Awake();
			}
			this._aoiEnabled = base.Runner.Config.Simulation.AreaOfInterestEnabled;
			this._initial = default(Tick);
			bool flag2 = base.Object.HasStateAuthority && !base.Object.Meta.HasSnapshots;
			if (flag2)
			{
				this.CopyToBuffer();
			}
			else
			{
				this.CopyToEngine();
			}
		}

		public override void Render()
		{
			bool flag = !this.CanInterpolate();
			if (!flag)
			{
				NetworkTRSP.Render(this, this._transform, this.SyncScale, this.SyncParent, true, ref this._initial);
				this._render = true;
				this._renderPosition = this._transform.localPosition;
				this._renderRotation = this._transform.localRotation;
				this._renderParent = this._transform.parent;
			}
		}

		[SerializeField]
		[InlineHelp]
		public bool SyncScale = false;

		[SerializeField]
		[InlineHelp]
		public bool SyncParent = false;

		private Tick _initial;

		private Transform _transform;

		private bool _simulation;

		private bool _aoiEnabled;

		private bool _aoiAutoUpdateOriginal;

		[SerializeField]
		[InlineHelp]
		private bool _autoAOIOverride = true;

		[SerializeField]
		[InlineHelp]
		public bool DisableSharedModeInterpolation = false;

		private bool _render;

		private Vector3 _renderPosition;

		private Quaternion _renderRotation;

		private Transform _renderParent;
	}
}
