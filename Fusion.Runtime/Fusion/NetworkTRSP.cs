using System;
using UnityEngine;

namespace Fusion
{
	[DisallowMultipleComponent]
	[NetworkBehaviourWeaved(14)]
	public class NetworkTRSP : NetworkBehaviour
	{
		public bool IsMainTRSP { get; internal set; }

		public unsafe NetworkTRSPData Data
		{
			get
			{
				return base.StateBufferIsValid ? (*base.ReinterpretState<NetworkTRSPData>(0)) : default(NetworkTRSPData);
			}
		}

		protected ref NetworkTRSPData State
		{
			get
			{
				return base.ReinterpretState<NetworkTRSPData>(0);
			}
		}

		public virtual void SetAreaOfInterestOverride(NetworkObject obj)
		{
			bool flag = obj;
			if (flag)
			{
				Assert.Always(obj.RuntimeFlags.CheckFlag(NetworkObjectRuntimeFlags.HasMainNetworkTRSP), "area of interest proxy must have a main network trsp");
			}
			this.State.AreaOfInterestOverride = obj;
		}

		protected static void Teleport(NetworkTRSP behaviour, Transform transform, Vector3? position = null, Quaternion? rotation = null)
		{
			bool flag = position != null;
			if (flag)
			{
				transform.position = position.Value;
				behaviour.State.Position = transform.localPosition;
			}
			bool flag2 = rotation != null;
			if (flag2)
			{
				transform.rotation = rotation.Value;
				behaviour.State.Rotation = transform.localRotation;
			}
			behaviour.State.TeleportKey++;
		}

		protected static void SetParentTransform(NetworkTRSP behaviour, Transform transform, NetworkBehaviourId parentId)
		{
			bool isValid = parentId.IsValid;
			if (isValid)
			{
				NetworkBehaviour networkBehaviour;
				bool flag = behaviour.Runner.TryFindBehaviour(parentId, out networkBehaviour);
				if (flag)
				{
					bool flag2 = transform.parent != networkBehaviour.transform;
					if (flag2)
					{
						transform.parent = networkBehaviour.transform;
					}
				}
			}
			else
			{
				bool flag3 = parentId.Behaviour == 0 && transform.parent;
				if (flag3)
				{
					transform.parent = null;
				}
			}
		}

		protected static void ResolveAOIOverride(NetworkTRSP behaviour, Transform parent)
		{
			Transform transform = parent;
			while (transform)
			{
				NetworkObject networkObject;
				bool flag = transform.TryGetComponent<NetworkObject>(out networkObject);
				if (flag)
				{
					Assert.Always(networkObject != behaviour.Object, "objects parent NetworkObject is itself?");
					behaviour.State.AreaOfInterestOverride = networkObject;
					break;
				}
				transform = transform.parent;
			}
			bool flag2 = !transform;
			if (flag2)
			{
				behaviour.State.AreaOfInterestOverride = default(NetworkId);
			}
		}

		private void OnEnable()
		{
			bool flag = base.Object != null && (base.Object.RuntimeFlags & NetworkObjectRuntimeFlags.Spawned) > NetworkObjectRuntimeFlags.None;
			if (flag)
			{
				this.reenabledTick = base.Runner.Tick;
			}
			NetworkBehaviourUtils.InternalOnEnable(this);
		}

		protected unsafe static void Render(NetworkTRSP behaviour, Transform transform, bool syncScale, bool syncParent, bool local, ref Tick initial)
		{
			NetworkBehaviourBuffer networkBehaviourBuffer;
			NetworkBehaviourBuffer networkBehaviourBuffer2;
			float num;
			bool flag = behaviour.TryGetSnapshotsBuffers(out networkBehaviourBuffer, out networkBehaviourBuffer2, out num);
			if (flag)
			{
				NetworkTRSPData networkTRSPData = networkBehaviourBuffer.ReinterpretState<NetworkTRSPData>(0);
				NetworkTRSPData networkTRSPData2 = networkBehaviourBuffer2.ReinterpretState<NetworkTRSPData>(0);
				PlayerRef a = *behaviour.Object.Meta.StateAuthority;
				bool flag2 = initial == 0;
				if (flag2)
				{
					initial = networkBehaviourBuffer2.Tick;
				}
				bool flag3 = initial == networkBehaviourBuffer2.Tick || behaviour.reenabledTick == networkBehaviourBuffer2.Tick;
				if (flag3)
				{
					num = 1f;
				}
				bool flag4 = networkTRSPData.TeleportKey != networkTRSPData2.TeleportKey;
				if (flag4)
				{
					num = ((num < 0.5f) ? 0f : 1f);
				}
				if (syncParent)
				{
					bool flag5 = networkTRSPData.Parent != networkTRSPData2.Parent;
					if (flag5)
					{
						NetworkTRSP.SetParentTransform(behaviour, transform, (num < 0.5f) ? networkTRSPData.Parent : networkTRSPData2.Parent);
						num = ((num < 0.5f) ? 0f : 1f);
					}
					else
					{
						NetworkTRSP.SetParentTransform(behaviour, transform, networkTRSPData.Parent);
					}
				}
				if (syncScale)
				{
					Vector3Compressed scale = networkTRSPData.Scale;
					Vector3Compressed scale2 = networkTRSPData2.Scale;
					bool flag6 = scale2 == scale;
					if (flag6)
					{
						bool flag7 = transform.localScale != scale;
						if (flag7)
						{
							transform.localScale = scale;
						}
					}
					else
					{
						transform.localScale = Vector3.Lerp(scale, scale2, num);
					}
				}
				Vector3 vector = Vector3.Lerp(networkTRSPData.Position, networkTRSPData2.Position, num);
				Quaternion quaternion = Quaternion.Slerp(networkTRSPData.Rotation, networkTRSPData2.Rotation, num);
				bool flag8 = a != behaviour._previousRenderStateAuth;
				if (flag8)
				{
					Vector3 value = local ? (transform.localPosition - vector) : (transform.position - vector);
					Quaternion value2 = local ? (quaternion * Quaternion.Inverse(transform.localRotation)) : (quaternion * Quaternion.Inverse(transform.rotation));
					behaviour._stateAuthorityChangePositionError = new Vector3?(value);
					behaviour._stateAuthorityChangeRotationError = new Quaternion?(value2);
				}
				bool flag9 = behaviour._stateAuthorityChangePositionError != null && behaviour._stateAuthorityChangeErrorCorrectionDelta > 0f;
				if (flag9)
				{
					Vector3 vector2 = behaviour._stateAuthorityChangePositionError.Value;
					vector += vector2;
					vector2 = Vector3.MoveTowards(vector2, Vector3.zero, behaviour._stateAuthorityChangeErrorCorrectionDelta);
					behaviour._stateAuthorityChangePositionError = new Vector3?(vector2);
					bool flag10 = vector2.Equals(Vector3.zero);
					if (flag10)
					{
						behaviour._stateAuthorityChangePositionError = null;
					}
				}
				bool flag11 = behaviour._stateAuthorityChangeRotationError != null && behaviour._stateAuthorityChangeErrorCorrectionDelta > 0f;
				if (flag11)
				{
					Quaternion quaternion2 = behaviour._stateAuthorityChangeRotationError.Value;
					quaternion *= quaternion2;
					quaternion2 = Quaternion.RotateTowards(quaternion2, Quaternion.identity, behaviour._stateAuthorityChangeErrorCorrectionDelta);
					behaviour._stateAuthorityChangeRotationError = new Quaternion?(quaternion2);
					bool flag12 = quaternion2.Equals(Quaternion.identity);
					if (flag12)
					{
						behaviour._stateAuthorityChangeRotationError = null;
					}
				}
				if (local)
				{
					transform.localPosition = vector;
					transform.localRotation = quaternion;
				}
				else
				{
					transform.SetPositionAndRotation(vector, quaternion);
				}
			}
			behaviour._previousRenderStateAuth = *behaviour.Object.Meta.StateAuthority;
		}

		private PlayerRef _previousRenderStateAuth;

		private Vector3? _stateAuthorityChangePositionError;

		private Quaternion? _stateAuthorityChangeRotationError;

		[SerializeField]
		[InlineHelp]
		private float _stateAuthorityChangeErrorCorrectionDelta = 0f;

		protected Tick reenabledTick;
	}
}
