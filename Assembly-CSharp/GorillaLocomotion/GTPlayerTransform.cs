using System;
using GorillaTag.Gravity;
using UnityEngine;

namespace GorillaLocomotion
{
	public class GTPlayerTransform : MonkeGravityController
	{
		public static Vector3 Up { get; private set; } = Vector3.up;

		public static Vector3 PhysicsUp { get; private set; } = Vector3.up;

		public static Vector3 Down { get; private set; } = Vector3.down;

		public static Vector3 PhysicsDown { get; private set; } = Vector3.down;

		public static Vector3 Forward { get; private set; } = Vector3.forward;

		public static Vector3 Right { get; private set; } = Vector3.right;

		public static Quaternion BodyRotation
		{
			get
			{
				return GTPlayerTransform.k_bodyTransform.rotation;
			}
		}

		public static bool UseNetRotation { get; set; } = false;

		public static bool IgnoreGravityRotation { get; set; } = false;

		public static bool IgnoreGravityForce { get; set; } = false;

		public static Vector3 RotationPosOffsetChange
		{
			get
			{
				return GTPlayerTransform.k_rotationPosOffsetChange;
			}
		}

		public static GTPlayerTransform Instance { get; private set; }

		public static void RotateToUp(in Vector3 targetUp)
		{
			if (targetUp == GTPlayerTransform.Up)
			{
				GTPlayerTransform.Instance.ClearRotationRecovery();
				return;
			}
			Vector3 up = GTPlayerTransform.Up;
			GTPlayerTransform.RotateFromToDirection(up, targetUp);
		}

		public static void RotateToForward(in Vector3 targetForward)
		{
			if (targetForward == GTPlayerTransform.Forward)
			{
				return;
			}
			Vector3 forward = GTPlayerTransform.Forward;
			GTPlayerTransform.RotateFromToDirection(forward, targetForward);
		}

		public static void RotateFromToDirection(in Vector3 currentDir, in Vector3 targetDir)
		{
			Quaternion rotation = GTPlayerTransform.k_transform.rotation;
			Quaternion quaternion = Quaternion.FromToRotation(currentDir, targetDir) * rotation;
			GTPlayerTransform.SetRotation(quaternion, rotation);
		}

		public static void RotateBy(in Quaternion rotation)
		{
			Quaternion rotation2 = GTPlayerTransform.k_transform.rotation;
			Quaternion quaternion = rotation2 * rotation;
			GTPlayerTransform.SetRotation(quaternion, rotation2);
		}

		public static void SetRotation(in Quaternion targetRotation)
		{
			Quaternion rotation = GTPlayerTransform.k_transform.rotation;
			GTPlayerTransform.SetRotation(targetRotation, rotation);
		}

		private static void SetRotation(in Quaternion newRotation, in Quaternion currentRotation)
		{
			ref readonly GTPlayer.HandState leftHandRef = ref GTPlayerTransform.k_playerInstance.LeftHandRef;
			ref readonly GTPlayer.HandState rightHandRef = ref GTPlayerTransform.k_playerInstance.RightHandRef;
			Vector3 position = GTPlayerTransform.k_transform.position;
			Quaternion quaternion = newRotation * Quaternion.Inverse(currentRotation);
			Vector3 position2 = GTPlayerTransform.k_bodyTransform.position;
			Vector3 vector = GTPlayerTransform.GetRotatedDifference(position, position2, quaternion);
			if (leftHandRef.wasColliding || leftHandRef.wasSliding)
			{
				Vector3 rotatedDifference = GTPlayerTransform.GetRotatedDifference(position, leftHandRef.lastPosition, quaternion);
				Vector3 lhs = Vector3.Normalize(rotatedDifference);
				RaycastHit lastHitInfo = leftHandRef.lastHitInfo;
				if (Vector3.Dot(lhs, lastHitInfo.normal) <= 0f)
				{
					vector -= rotatedDifference;
				}
			}
			if (rightHandRef.wasColliding || rightHandRef.wasSliding)
			{
				Vector3 rotatedDifference2 = GTPlayerTransform.GetRotatedDifference(position, rightHandRef.lastPosition, quaternion);
				Vector3 lhs2 = Vector3.Normalize(rotatedDifference2);
				RaycastHit lastHitInfo = rightHandRef.lastHitInfo;
				if (Vector3.Dot(lhs2, lastHitInfo.normal) <= 0f)
				{
					vector -= rotatedDifference2;
				}
			}
			GTPlayerTransform.k_rotationPosOffsetChange -= vector;
			GTPlayerTransform.k_rigidBody.position = position - vector;
			GTPlayerTransform.k_rigidBody.rotation = newRotation;
			GTPlayerTransform.Up = newRotation * Vector3.up;
			GTPlayerTransform.Down = GTPlayerTransform.Up * -1f;
			GTPlayerTransform.Forward = newRotation * Vector3.forward;
			GTPlayerTransform.Right = newRotation * Vector3.right;
		}

		private static Vector3 GetRotatedDifference(in Vector3 pivotPoint, in Vector3 worldPoint, in Quaternion rotation)
		{
			Vector3 vector = worldPoint - pivotPoint;
			return rotation * vector - vector;
		}

		public static void ApplyRotationOverride(in Quaternion rotation, int frameTime)
		{
			GTPlayerTransform.SetRotation(rotation);
			GTPlayerTransform.k_rotationOverrideFrameTime = frameTime;
		}

		public static void ResetRotationPositionOffset()
		{
			GTPlayerTransform.k_rotationPosOffsetChange = Vector3.zero;
		}

		public static void EnableNetworkRotations()
		{
			GTPlayerTransform.UseNetRotation = true;
		}

		public static void DisableNetworkRotations()
		{
			GTPlayerTransform.UseNetRotation = false;
		}

		protected override void Awake()
		{
			base.Awake();
			if (!base.Register)
			{
				Debug.LogError("GTPlayerTransform: failed to load required references", base.gameObject);
			}
			GTPlayerTransform.Instance = this;
			GTPlayerTransform.k_transform = this.m_targetTransform;
			GTPlayerTransform.k_rigidBody = this.m_targetRigidBody;
			GTPlayerTransform.k_bodyTransform = this.m_gtPlayerBodyTransform;
			GTPlayerTransform.k_playerInstance = this.m_gtPlayerInstance;
			GTPlayerTransform.Up = GTPlayerTransform.k_transform.up;
			GTPlayerTransform.Forward = GTPlayerTransform.k_transform.forward;
			GTPlayerTransform.Right = GTPlayerTransform.k_transform.right;
			GTPlayerTransform.Down = GTPlayerTransform.Up * -1f;
			this.m_globalGravityIntent = false;
		}

		public override void ApplyGravityUpRotation(in Vector3 upDir, float speed)
		{
			if (GTPlayerTransform.IgnoreGravityRotation || GTPlayerTransform.k_rotationOverrideFrameTime >= Time.frameCount - 1)
			{
				return;
			}
			if (base.InstantRotation)
			{
				GTPlayerTransform.RotateToUp(upDir);
				return;
			}
			Vector3 vector = Vector3.RotateTowards(GTPlayerTransform.Up, upDir, speed, 0f);
			GTPlayerTransform.RotateToUp(vector);
		}

		public override void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
		{
			if (GTPlayerTransform.IgnoreGravityForce || GTPlayerTransform.k_playerInstance.isClimbing || GTPlayerTransform.k_playerInstance.GravityOverrideCount > 0)
			{
				return;
			}
			Vector3 vector = force * GTPlayerTransform.k_playerInstance.scale;
			base.ApplyGravityForce(vector, forceType);
		}

		public override Vector3 GetWorldPoint()
		{
			return GTPlayerTransform.k_bodyTransform.position;
		}

		public override void CallBack()
		{
			base.CallBack();
			GTPlayerTransform.PhysicsUp = base.GravityUp;
			GTPlayerTransform.PhysicsDown = base.GravityDown;
			if (base.GravityZonesCount > 0)
			{
				return;
			}
			if (GTPlayerTransform.Up != GTPlayerTransform.PhysicsUp)
			{
				Vector3 physicsUp = GTPlayerTransform.PhysicsUp;
				this.ApplyGravityUpRotation(physicsUp, MonkeGravityManager.DefaultGravityInfo.rotationSpeed * Time.fixedDeltaTime);
			}
		}

		private static Vector3 k_rotationPosOffsetChange = Vector3.zero;

		private static Transform k_transform;

		private static Rigidbody k_rigidBody;

		private static Transform k_bodyTransform;

		private static GTPlayer k_playerInstance;

		private static int k_rotationOverrideFrameTime;

		[SerializeField]
		private Transform m_gtPlayerBodyTransform;

		[SerializeField]
		private GTPlayer m_gtPlayerInstance;
	}
}
