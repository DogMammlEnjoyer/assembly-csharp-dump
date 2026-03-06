using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Position Control/Cinemachine Third Person Follow")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Body)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineThirdPersonFollow.html")]
	public class CinemachineThirdPersonFollow : CinemachineComponentBase, CinemachineFreeLookModifier.IModifierValueSource, CinemachineFreeLookModifier.IModifiablePositionDamping, CinemachineFreeLookModifier.IModifiableDistance
	{
		public Collider CurrentObstacle { get; set; }

		private void OnValidate()
		{
			this.CameraSide = Mathf.Clamp(this.CameraSide, -1f, 1f);
			this.Damping.x = Mathf.Max(0f, this.Damping.x);
			this.Damping.y = Mathf.Max(0f, this.Damping.y);
			this.Damping.z = Mathf.Max(0f, this.Damping.z);
			this.AvoidObstacles.CameraRadius = Mathf.Max(0.001f, this.AvoidObstacles.CameraRadius);
			this.AvoidObstacles.DampingIntoCollision = Mathf.Max(0f, this.AvoidObstacles.DampingIntoCollision);
			this.AvoidObstacles.DampingFromCollision = Mathf.Max(0f, this.AvoidObstacles.DampingFromCollision);
		}

		private void Reset()
		{
			this.ShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
			this.VerticalArmLength = 0.4f;
			this.CameraSide = 1f;
			this.CameraDistance = 2f;
			this.Damping = new Vector3(0.1f, 0.5f, 0.3f);
			this.AvoidObstacles = CinemachineThirdPersonFollow.ObstacleSettings.Default;
		}

		float CinemachineFreeLookModifier.IModifierValueSource.NormalizedModifierValue
		{
			get
			{
				Vector3 referenceUp = base.VirtualCamera.State.ReferenceUp;
				Quaternion followTargetRotation = base.FollowTargetRotation;
				return Mathf.Clamp(Vector3.SignedAngle(followTargetRotation * Vector3.up, referenceUp, followTargetRotation * Vector3.right), -90f, 90f) / -90f;
			}
		}

		Vector3 CinemachineFreeLookModifier.IModifiablePositionDamping.PositionDamping
		{
			get
			{
				return this.Damping;
			}
			set
			{
				this.Damping = value;
			}
		}

		float CinemachineFreeLookModifier.IModifiableDistance.Distance
		{
			get
			{
				return this.CameraDistance;
			}
			set
			{
				this.CameraDistance = value;
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && base.FollowTarget != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Body;
			}
		}

		public override float GetMaxDampTime()
		{
			return Mathf.Max(this.AvoidObstacles.Enabled ? Mathf.Max(this.AvoidObstacles.DampingIntoCollision, this.AvoidObstacles.DampingFromCollision) : 0f, Mathf.Max(this.Damping.x, Mathf.Max(this.Damping.y, this.Damping.z)));
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (this.IsValid)
			{
				if (!base.VirtualCamera.PreviousStateIsValid)
				{
					deltaTime = -1f;
				}
				this.PositionCamera(ref curState, deltaTime);
			}
		}

		public override void OnTargetObjectWarped(Transform target, Vector3 positionDelta)
		{
			base.OnTargetObjectWarped(target, positionDelta);
			if (target == base.FollowTarget)
			{
				this.m_PreviousFollowTargetPosition += positionDelta;
			}
		}

		private void PositionCamera(ref CameraState curState, float deltaTime)
		{
			Vector3 referenceUp = curState.ReferenceUp;
			Vector3 followTargetPosition = base.FollowTargetPosition;
			Quaternion followTargetRotation = base.FollowTargetRotation;
			Vector3 a = followTargetRotation * Vector3.forward;
			Quaternion heading = CinemachineThirdPersonFollow.GetHeading(followTargetRotation, referenceUp);
			if (deltaTime < 0f)
			{
				this.m_DampingCorrection = Vector3.zero;
				this.m_CamPosCollisionCorrection = 0f;
			}
			else
			{
				this.m_DampingCorrection += Quaternion.Inverse(heading) * (this.m_PreviousFollowTargetPosition - followTargetPosition);
				this.m_DampingCorrection -= base.VirtualCamera.DetachedFollowTargetDamp(this.m_DampingCorrection, this.Damping, deltaTime);
			}
			this.m_PreviousFollowTargetPosition = followTargetPosition;
			Vector3 root = followTargetPosition;
			Vector3 vector;
			Vector3 vector2;
			this.GetRawRigPositions(root, followTargetRotation, heading, out vector, out vector2);
			Vector3 vector3 = vector2 - a * (this.CameraDistance - this.m_DampingCorrection.z);
			this.CurrentObstacle = null;
			if (this.AvoidObstacles.Enabled)
			{
				float num = 0f;
				Vector3 root2 = this.ResolveCollisions(root, vector2, -1f, this.AvoidObstacles.CameraRadius * 1.05f, ref num);
				vector3 = this.ResolveCollisions(root2, vector3, deltaTime, this.AvoidObstacles.CameraRadius, ref this.m_CamPosCollisionCorrection);
			}
			curState.RawPosition = vector3;
			curState.RawOrientation = followTargetRotation;
			if (!curState.HasLookAt() || curState.ReferenceLookAt.Equals(followTargetPosition))
			{
				curState.ReferenceLookAt = followTargetPosition + a * 0.01f;
			}
		}

		public void GetRigPositions(out Vector3 root, out Vector3 shoulder, out Vector3 hand)
		{
			Vector3 referenceUp = base.VirtualCamera.State.ReferenceUp;
			Quaternion followTargetRotation = base.FollowTargetRotation;
			Quaternion heading = CinemachineThirdPersonFollow.GetHeading(followTargetRotation, referenceUp);
			root = this.m_PreviousFollowTargetPosition;
			this.GetRawRigPositions(root, followTargetRotation, heading, out shoulder, out hand);
			if (this.AvoidObstacles.Enabled)
			{
				float num = 0f;
				hand = this.ResolveCollisions(root, hand, -1f, this.AvoidObstacles.CameraRadius * 1.05f, ref num);
			}
		}

		internal static Quaternion GetHeading(Quaternion targetRot, Vector3 up)
		{
			Vector3 vector = targetRot * Vector3.forward;
			Vector3 vector2 = Vector3.Cross(up, Vector3.Cross(vector.ProjectOntoPlane(up), up));
			if (vector2.AlmostZero())
			{
				vector2 = Vector3.Cross(targetRot * Vector3.right, up);
			}
			return Quaternion.LookRotation(vector2, up);
		}

		private void GetRawRigPositions(Vector3 root, Quaternion targetRot, Quaternion heading, out Vector3 shoulder, out Vector3 hand)
		{
			Vector3 shoulderOffset = this.ShoulderOffset;
			shoulderOffset.x = Mathf.Lerp(-shoulderOffset.x, shoulderOffset.x, this.CameraSide);
			shoulderOffset.x += this.m_DampingCorrection.x;
			shoulderOffset.y += this.m_DampingCorrection.y;
			shoulder = root + heading * shoulderOffset;
			hand = shoulder + targetRot * new Vector3(0f, this.VerticalArmLength, 0f);
		}

		private Vector3 ResolveCollisions(Vector3 root, Vector3 tip, float deltaTime, float cameraRadius, ref float collisionCorrection)
		{
			if (this.AvoidObstacles.CollisionFilter.value == 0)
			{
				return tip;
			}
			Vector3 vector = tip - root;
			float magnitude = vector.magnitude;
			if (magnitude < 0.0001f)
			{
				return tip;
			}
			vector /= magnitude;
			Vector3 vector2 = tip;
			float num = 0f;
			RaycastHit raycastHit;
			if (RuntimeUtility.SphereCastIgnoreTag(new Ray(root, vector), cameraRadius, out raycastHit, magnitude, this.AvoidObstacles.CollisionFilter, this.AvoidObstacles.IgnoreTag))
			{
				this.CurrentObstacle = raycastHit.collider;
				num = (raycastHit.point + raycastHit.normal * cameraRadius - tip).magnitude;
			}
			collisionCorrection += ((deltaTime < 0f) ? (num - collisionCorrection) : Damper.Damp(num - collisionCorrection, (num > collisionCorrection) ? this.AvoidObstacles.DampingIntoCollision : this.AvoidObstacles.DampingFromCollision, deltaTime));
			if (collisionCorrection > 0.0001f)
			{
				vector2 -= vector * collisionCorrection;
			}
			return vector2;
		}

		[Tooltip("How responsively the camera tracks the target.  Each axis (camera-local) can have its own setting.  Value is the approximate time it takes the camera to catch up to the target's new position.  Smaller values give a more rigid effect, larger values give a squishier one")]
		public Vector3 Damping;

		[Header("Rig")]
		[Tooltip("Position of the shoulder pivot relative to the Follow target origin.  This offset is in target-local space")]
		public Vector3 ShoulderOffset;

		[Tooltip("Vertical offset of the hand in relation to the shoulder.  Arm length will affect the follow target's screen position when the camera rotates vertically")]
		public float VerticalArmLength;

		[Tooltip("Specifies which shoulder (left, right, or in-between) the camera is on")]
		[Range(0f, 1f)]
		public float CameraSide;

		[Tooltip("How far behind the hand the camera will be placed")]
		public float CameraDistance;

		[FoldoutWithEnabledButton("Enabled")]
		public CinemachineThirdPersonFollow.ObstacleSettings AvoidObstacles = CinemachineThirdPersonFollow.ObstacleSettings.Default;

		private Vector3 m_PreviousFollowTargetPosition;

		private Vector3 m_DampingCorrection;

		private float m_CamPosCollisionCorrection;

		[Serializable]
		public struct ObstacleSettings
		{
			internal static CinemachineThirdPersonFollow.ObstacleSettings Default
			{
				get
				{
					return new CinemachineThirdPersonFollow.ObstacleSettings
					{
						Enabled = false,
						CollisionFilter = 1,
						IgnoreTag = string.Empty,
						CameraRadius = 0.2f,
						DampingIntoCollision = 0f,
						DampingFromCollision = 0.5f
					};
				}
			}

			[Tooltip("If enabled, camera will be pulled in front of occluding obstacles")]
			public bool Enabled;

			[Tooltip("Camera will avoid obstacles on these layers")]
			public LayerMask CollisionFilter;

			[TagField]
			[Tooltip("Obstacles with this tag will be ignored.  It is a good idea to set this field to the target's tag")]
			public string IgnoreTag;

			[Tooltip("Specifies how close the camera can get to obstacles")]
			[Range(0f, 1f)]
			public float CameraRadius;

			[Range(0f, 10f)]
			[Tooltip("How gradually the camera moves to correct for occlusions.  Higher numbers will move the camera more gradually.")]
			public float DampingIntoCollision;

			[Range(0f, 10f)]
			[Tooltip("How gradually the camera returns to its normal position after having been corrected by the built-in collision resolution system.  Higher numbers will move the camera more gradually back to normal.")]
			public float DampingFromCollision;
		}
	}
}
