using System;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	public class HandPhysics : MonoBehaviour
	{
		private void Start()
		{
			this.hand = base.GetComponent<Hand>();
			this.handCollider = Object.Instantiate<GameObject>(this.handColliderPrefab.gameObject).GetComponent<HandCollider>();
			Vector3 localPosition = this.handCollider.transform.localPosition;
			Quaternion localRotation = this.handCollider.transform.localRotation;
			this.handCollider.transform.parent = Player.instance.transform;
			this.handCollider.transform.localPosition = localPosition;
			this.handCollider.transform.localRotation = localRotation;
			this.handCollider.hand = this;
			base.GetComponent<SteamVR_Behaviour_Pose>().onTransformUpdated.AddListener(new UnityAction<SteamVR_Behaviour_Pose, SteamVR_Input_Sources>(this.UpdateHand));
		}

		private void FixedUpdate()
		{
			if (this.hand.skeleton == null)
			{
				return;
			}
			this.initialized = true;
			this.UpdateCenterPoint();
			this.handCollider.MoveTo(this.targetPosition, this.targetRotation);
			if ((this.handCollider.transform.position - this.targetPosition).sqrMagnitude > 0.36f)
			{
				this.handCollider.TeleportTo(this.targetPosition, this.targetRotation);
			}
			this.UpdateFingertips();
		}

		private void UpdateCenterPoint()
		{
			Vector3 b = this.hand.skeleton.GetBonePosition(12, false) - this.hand.skeleton.GetBonePosition(0, false);
			if (this.hand.HasSkeleton())
			{
				this.handCollider.SetCenterPoint(this.hand.skeleton.transform.position + b);
			}
		}

		private void UpdatePositions()
		{
			if (this.hand.currentAttachedObject != null)
			{
				this.collisionsEnabled = false;
			}
			else if (!this.collisionsEnabled)
			{
				this.clearanceBuffer[0] = null;
				Physics.OverlapSphereNonAlloc(this.hand.objectAttachmentPoint.position, 0.1f, this.clearanceBuffer);
				if (this.clearanceBuffer[0] == null)
				{
					this.collisionsEnabled = true;
				}
			}
			this.handCollider.SetCollisionDetectionEnabled(this.collisionsEnabled);
			if (this.hand.skeleton == null)
			{
				return;
			}
			this.initialized = true;
			this.wristToRoot = Matrix4x4.TRS(this.ProcessPos(1, this.hand.skeleton.GetBone(1).localPosition), this.ProcessRot(1, this.hand.skeleton.GetBone(1).localRotation), Vector3.one).inverse;
			this.rootToArmature = Matrix4x4.TRS(this.ProcessPos(0, this.hand.skeleton.GetBone(0).localPosition), this.ProcessRot(0, this.hand.skeleton.GetBone(0).localRotation), Vector3.one).inverse;
			this.wristToArmature = (this.wristToRoot * this.rootToArmature).inverse;
			this.targetPosition = base.transform.TransformPoint(this.wristToArmature.MultiplyPoint3x4(Vector3.zero));
			this.targetRotation = base.transform.rotation * this.wristToArmature.GetRotation();
			if (Time.timeScale == 0f)
			{
				this.handCollider.TeleportTo(this.targetPosition, this.targetRotation);
			}
		}

		private void UpdateFingertips()
		{
			this.wrist = this.hand.skeleton.GetBone(1);
			for (int i = 0; i < 5; i++)
			{
				int boneForFingerTip = SteamVR_Skeleton_JointIndexes.GetBoneForFingerTip(i);
				for (int j = 0; j < this.handCollider.fingerColliders[i].Length; j++)
				{
					int joint = boneForFingerTip - 1 - j;
					if (this.handCollider.fingerColliders[i][j] != null)
					{
						this.handCollider.fingerColliders[i][j].localPosition = this.wrist.InverseTransformPoint(this.hand.skeleton.GetBone(joint).position);
					}
				}
			}
		}

		private void UpdateHand(SteamVR_Behaviour_Pose pose, SteamVR_Input_Sources inputSource)
		{
			if (!this.initialized)
			{
				return;
			}
			this.UpdateCenterPoint();
			this.UpdatePositions();
			Quaternion rotation = this.handCollider.transform.rotation * this.wristToArmature.inverse.GetRotation();
			this.hand.mainRenderModel.transform.rotation = rotation;
			Vector3 position = this.handCollider.transform.TransformPoint(this.wristToArmature.inverse.MultiplyPoint3x4(Vector3.zero));
			this.hand.mainRenderModel.transform.position = position;
		}

		private Vector3 ProcessPos(int boneIndex, Vector3 pos)
		{
			if (this.hand.skeleton.mirroring != SteamVR_Behaviour_Skeleton.MirrorType.None)
			{
				return SteamVR_Behaviour_Skeleton.MirrorPosition(boneIndex, pos);
			}
			return pos;
		}

		private Quaternion ProcessRot(int boneIndex, Quaternion rot)
		{
			if (this.hand.skeleton.mirroring != SteamVR_Behaviour_Skeleton.MirrorType.None)
			{
				return SteamVR_Behaviour_Skeleton.MirrorRotation(boneIndex, rot);
			}
			return rot;
		}

		[Tooltip("Hand collider prefab to instantiate")]
		public HandCollider handColliderPrefab;

		[HideInInspector]
		public HandCollider handCollider;

		[Tooltip("Layers to consider when checking if an area is clear")]
		public LayerMask clearanceCheckMask;

		[HideInInspector]
		public Hand hand;

		private const float handResetDistance = 0.6f;

		private const float collisionReenableClearanceRadius = 0.1f;

		private bool initialized;

		private bool collisionsEnabled = true;

		private Matrix4x4 wristToRoot;

		private Matrix4x4 rootToArmature;

		private Matrix4x4 wristToArmature;

		private Vector3 targetPosition = Vector3.zero;

		private Quaternion targetRotation = Quaternion.identity;

		private const int wristBone = 1;

		private const int rootBone = 0;

		private Collider[] clearanceBuffer = new Collider[1];

		private Transform wrist;

		private const int thumbBone = 4;

		private const int indexBone = 9;

		private const int middleBone = 14;

		private const int ringBone = 19;

		private const int pinkyBone = 24;
	}
}
