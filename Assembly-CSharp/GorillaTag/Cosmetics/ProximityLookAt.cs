using System;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	public class ProximityLookAt : MonoBehaviour, IGorillaSliceableSimple
	{
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			if (this.transferableParent != null)
			{
				this.ownerRig = this.transferableParent.ownerRig;
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = base.GetComponentInParent<VRRig>();
			}
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.CacheSettings();
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
			this.lookTarget = null;
			this.lastTargetSwitchTime = float.NegativeInfinity;
		}

		private void OnValidate()
		{
			this.CacheSettings();
		}

		private void CacheSettings()
		{
			this.normalizedLocalForward = ProximityLookAt.LocalAxisToVector(this.localForward);
			this.cosAngle = Mathf.Cos(this.targetSearchAngleDegrees * 0.017453292f);
			this.sqrRadius = this.lookRadius * this.lookRadius;
		}

		private static Vector3 LocalAxisToVector(ProximityLookAt.LocalAxis axis)
		{
			switch (axis)
			{
			case ProximityLookAt.LocalAxis.Forward:
				return Vector3.forward;
			case ProximityLookAt.LocalAxis.Back:
				return Vector3.back;
			case ProximityLookAt.LocalAxis.Right:
				return Vector3.right;
			case ProximityLookAt.LocalAxis.Left:
				return Vector3.left;
			case ProximityLookAt.LocalAxis.Up:
				return Vector3.up;
			case ProximityLookAt.LocalAxis.Down:
				return Vector3.down;
			default:
				return Vector3.forward;
			}
		}

		public void SliceUpdate()
		{
			Transform x = this.FindTarget();
			if (x == this.lookTarget)
			{
				return;
			}
			if (Time.time - this.lastTargetSwitchTime < this.targetSwitchCooldown)
			{
				return;
			}
			this.lookTarget = x;
			this.lastTargetSwitchTime = Time.time;
		}

		private void LateUpdate()
		{
			if (this.lookTransforms == null)
			{
				return;
			}
			Vector3 vector = base.transform.TransformDirection(this.normalizedLocalForward);
			for (int i = 0; i < this.lookTransforms.Length; i++)
			{
				Transform transform = this.lookTransforms[i];
				if (!(transform == null))
				{
					Vector3 vector2 = (this.lookTarget != null) ? (this.lookTarget.position - transform.position).normalized : vector;
					vector2 = Vector3.RotateTowards(vector, vector2, this.lookAtAngleDegreeMax * 0.017453292f, 0f);
					if (this.pivotConstraint != null)
					{
						Vector3 vector3 = this.pivotConstraint.InverseTransformDirection(vector2);
						vector3.y = Mathf.Clamp(vector3.y, this.minPivotY, this.maxPivotY);
						vector2 = this.pivotConstraint.TransformDirection(vector3.normalized);
					}
					Vector3 forward = Vector3.RotateTowards(transform.rotation * Vector3.forward, vector2, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
					transform.rotation = ((this.pivotConstraint != null) ? Quaternion.LookRotation(forward, this.pivotConstraint.up) : Quaternion.LookRotation(forward));
				}
			}
		}

		private Transform FindTarget()
		{
			if (!PhotonNetwork.InRoom)
			{
				return GorillaTagger.Instance.offlineVRRig.tagSound.transform;
			}
			Vector3 lhs = base.transform.TransformDirection(this.normalizedLocalForward);
			float num = float.NegativeInfinity;
			Transform result = null;
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (this.includeOwner || !(vrrig == this.ownerRig))
				{
					Vector3 vector = vrrig.tagSound.transform.position - base.transform.position;
					if (vector.sqrMagnitude <= this.sqrRadius)
					{
						Vector3 normalized = vector.normalized;
						float num2 = Vector3.Dot(lhs, normalized);
						if (num2 >= this.cosAngle && num2 > num)
						{
							num = num2;
							result = vrrig.tagSound.transform;
						}
					}
				}
			}
			return result;
		}

		[Header("Settings")]
		[SerializeField]
		private Transform[] lookTransforms;

		[Tooltip("The local axis that points 'forward' on this transform.")]
		[SerializeField]
		private ProximityLookAt.LocalAxis localForward = ProximityLookAt.LocalAxis.Down;

		[SerializeField]
		private float lookRadius = 0.5f;

		[Tooltip("The cone angle in degrees used to detect nearby players.Only players within this angle of the forward direction are considered as targets.")]
		[SerializeField]
		private float targetSearchAngleDegrees = 60f;

		[Tooltip("How far in degrees the transform can physically rotate from its rest position.Should be less than or equal to targetSearchAngleDegrees")]
		[SerializeField]
		private float lookAtAngleDegreeMax = 45f;

		[SerializeField]
		private float rotSpeed = 180f;

		[Tooltip("Seconds to hold the current target before switching to a new one")]
		[SerializeField]
		private float targetSwitchCooldown = 0.5f;

		[Tooltip("Whether the cosmetic owner can be considered as a look target.")]
		[SerializeField]
		private bool includeOwner;

		[Header("Pivot Clamping (Optional)")]
		[Tooltip("Assign a pivot transform to constrain rotation relative to it. Leave empty to skip clamping.")]
		[SerializeField]
		private Transform pivotConstraint;

		[SerializeField]
		private float minPivotY = -1f;

		[SerializeField]
		private float maxPivotY = 1f;

		private TransferrableObject transferableParent;

		private VRRig ownerRig;

		private Transform lookTarget;

		private Vector3 normalizedLocalForward;

		private float cosAngle;

		private float sqrRadius;

		private float lastTargetSwitchTime = float.NegativeInfinity;

		public enum LocalAxis
		{
			Forward,
			Back,
			Right,
			Left,
			Up,
			Down
		}
	}
}
