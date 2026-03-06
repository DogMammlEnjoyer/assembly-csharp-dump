using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Skeleton_Poser : MonoBehaviour
	{
		public int blendPoseCount
		{
			get
			{
				return this.blendPoses.Length;
			}
		}

		protected void Awake()
		{
			if (this.previewLeftInstance != null)
			{
				Object.DestroyImmediate(this.previewLeftInstance);
			}
			if (this.previewRightInstance != null)
			{
				Object.DestroyImmediate(this.previewRightInstance);
			}
			this.blendPoses = new SteamVR_Skeleton_Poser.SkeletonBlendablePose[this.skeletonAdditionalPoses.Count + 1];
			for (int i = 0; i < this.blendPoseCount; i++)
			{
				this.blendPoses[i] = new SteamVR_Skeleton_Poser.SkeletonBlendablePose(this.GetPoseByIndex(i));
				this.blendPoses[i].PoseToSnapshots();
			}
			this.boneCount = this.skeletonMainPose.leftHand.bonePositions.Length;
			this.blendedSnapshotL = new SteamVR_Skeleton_PoseSnapshot(this.boneCount, SteamVR_Input_Sources.LeftHand);
			this.blendedSnapshotR = new SteamVR_Skeleton_PoseSnapshot(this.boneCount, SteamVR_Input_Sources.RightHand);
		}

		public void SetBlendingBehaviourValue(string behaviourName, float value)
		{
			SteamVR_Skeleton_Poser.PoseBlendingBehaviour poseBlendingBehaviour = this.FindBlendingBehaviour(behaviourName, true);
			if (poseBlendingBehaviour != null)
			{
				poseBlendingBehaviour.value = value;
				if (poseBlendingBehaviour.type != SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes.Manual)
				{
					Debug.LogWarning("[SteamVR] Blending Behaviour: " + behaviourName + " is not a manual behaviour. Its value will likely be overriden.", this);
				}
			}
		}

		public float GetBlendingBehaviourValue(string behaviourName)
		{
			SteamVR_Skeleton_Poser.PoseBlendingBehaviour poseBlendingBehaviour = this.FindBlendingBehaviour(behaviourName, true);
			if (poseBlendingBehaviour != null)
			{
				return poseBlendingBehaviour.value;
			}
			return 0f;
		}

		public void SetBlendingBehaviourEnabled(string behaviourName, bool value)
		{
			SteamVR_Skeleton_Poser.PoseBlendingBehaviour poseBlendingBehaviour = this.FindBlendingBehaviour(behaviourName, true);
			if (poseBlendingBehaviour != null)
			{
				poseBlendingBehaviour.enabled = value;
			}
		}

		public bool GetBlendingBehaviourEnabled(string behaviourName)
		{
			SteamVR_Skeleton_Poser.PoseBlendingBehaviour poseBlendingBehaviour = this.FindBlendingBehaviour(behaviourName, true);
			return poseBlendingBehaviour != null && poseBlendingBehaviour.enabled;
		}

		public SteamVR_Skeleton_Poser.PoseBlendingBehaviour GetBlendingBehaviour(string behaviourName)
		{
			return this.FindBlendingBehaviour(behaviourName, true);
		}

		protected SteamVR_Skeleton_Poser.PoseBlendingBehaviour FindBlendingBehaviour(string behaviourName, bool throwErrors = true)
		{
			SteamVR_Skeleton_Poser.PoseBlendingBehaviour poseBlendingBehaviour = this.blendingBehaviours.Find((SteamVR_Skeleton_Poser.PoseBlendingBehaviour b) => b.name == behaviourName);
			if (poseBlendingBehaviour == null)
			{
				if (throwErrors)
				{
					Debug.LogError("[SteamVR] Blending Behaviour: " + behaviourName + " not found on Skeleton Poser: " + base.gameObject.name, this);
				}
				return null;
			}
			return poseBlendingBehaviour;
		}

		public SteamVR_Skeleton_Pose GetPoseByIndex(int index)
		{
			if (index == 0)
			{
				return this.skeletonMainPose;
			}
			return this.skeletonAdditionalPoses[index - 1];
		}

		private SteamVR_Skeleton_PoseSnapshot GetHandSnapshot(SteamVR_Input_Sources inputSource)
		{
			if (inputSource == SteamVR_Input_Sources.LeftHand)
			{
				return this.blendedSnapshotL;
			}
			return this.blendedSnapshotR;
		}

		public SteamVR_Skeleton_PoseSnapshot GetBlendedPose(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources handType)
		{
			this.UpdatePose(skeletonAction, handType);
			return this.GetHandSnapshot(handType);
		}

		public SteamVR_Skeleton_PoseSnapshot GetBlendedPose(SteamVR_Behaviour_Skeleton skeletonBehaviour)
		{
			return this.GetBlendedPose(skeletonBehaviour.skeletonAction, skeletonBehaviour.inputSource);
		}

		public void UpdatePose(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources inputSource)
		{
			if (this.poseUpdatedThisFrame)
			{
				return;
			}
			this.poseUpdatedThisFrame = true;
			if (skeletonAction.activeBinding)
			{
				this.blendPoses[0].UpdateAdditiveAnimation(skeletonAction, inputSource);
			}
			SteamVR_Skeleton_PoseSnapshot handSnapshot = this.GetHandSnapshot(inputSource);
			handSnapshot.CopyFrom(this.blendPoses[0].GetHandSnapshot(inputSource));
			this.ApplyBlenderBehaviours(skeletonAction, inputSource, handSnapshot);
			if (inputSource == SteamVR_Input_Sources.RightHand)
			{
				this.blendedSnapshotR = handSnapshot;
			}
			if (inputSource == SteamVR_Input_Sources.LeftHand)
			{
				this.blendedSnapshotL = handSnapshot;
			}
		}

		protected void ApplyBlenderBehaviours(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources inputSource, SteamVR_Skeleton_PoseSnapshot snapshot)
		{
			for (int i = 0; i < this.blendingBehaviours.Count; i++)
			{
				this.blendingBehaviours[i].Update(Time.deltaTime, inputSource);
				if (this.blendingBehaviours[i].enabled && this.blendingBehaviours[i].influence * this.blendingBehaviours[i].value > 0.01f)
				{
					if (this.blendingBehaviours[i].pose != 0 && skeletonAction.activeBinding)
					{
						this.blendPoses[this.blendingBehaviours[i].pose].UpdateAdditiveAnimation(skeletonAction, inputSource);
					}
					this.blendingBehaviours[i].ApplyBlending(snapshot, this.blendPoses, inputSource);
				}
			}
		}

		protected void LateUpdate()
		{
			this.poseUpdatedThisFrame = false;
		}

		protected Vector3 BlendVectors(Vector3[] vectors, float[] weights)
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < vectors.Length; i++)
			{
				vector += vectors[i] * weights[i];
			}
			return vector;
		}

		protected Quaternion BlendQuaternions(Quaternion[] quaternions, float[] weights)
		{
			Quaternion quaternion = Quaternion.identity;
			for (int i = 0; i < quaternions.Length; i++)
			{
				quaternion *= Quaternion.Slerp(Quaternion.identity, quaternions[i], weights[i]);
			}
			return quaternion;
		}

		public Vector3 GetTargetHandPosition(SteamVR_Behaviour_Skeleton hand, Transform origin)
		{
			Vector3 position = origin.position;
			Quaternion rotation = hand.transform.rotation;
			hand.transform.rotation = this.GetBlendedPose(hand).rotation;
			origin.position = hand.transform.TransformPoint(this.GetBlendedPose(hand).position);
			Vector3 position2 = origin.InverseTransformPoint(hand.transform.position);
			origin.position = position;
			hand.transform.rotation = rotation;
			return origin.TransformPoint(position2);
		}

		public Quaternion GetTargetHandRotation(SteamVR_Behaviour_Skeleton hand, Transform origin)
		{
			Quaternion rotation = origin.rotation;
			origin.rotation = hand.transform.rotation * this.GetBlendedPose(hand).rotation;
			Quaternion rhs = Quaternion.Inverse(origin.rotation) * hand.transform.rotation;
			origin.rotation = rotation;
			return origin.rotation * rhs;
		}

		public bool poseEditorExpanded = true;

		public bool blendEditorExpanded = true;

		public string[] poseNames;

		public GameObject overridePreviewLeftHandPrefab;

		public GameObject overridePreviewRightHandPrefab;

		public SteamVR_Skeleton_Pose skeletonMainPose;

		public List<SteamVR_Skeleton_Pose> skeletonAdditionalPoses = new List<SteamVR_Skeleton_Pose>();

		[SerializeField]
		protected bool showLeftPreview;

		[SerializeField]
		protected bool showRightPreview = true;

		[SerializeField]
		protected GameObject previewLeftInstance;

		[SerializeField]
		protected GameObject previewRightInstance;

		[SerializeField]
		protected int previewPoseSelection;

		public List<SteamVR_Skeleton_Poser.PoseBlendingBehaviour> blendingBehaviours = new List<SteamVR_Skeleton_Poser.PoseBlendingBehaviour>();

		public SteamVR_Skeleton_PoseSnapshot blendedSnapshotL;

		public SteamVR_Skeleton_PoseSnapshot blendedSnapshotR;

		private SteamVR_Skeleton_Poser.SkeletonBlendablePose[] blendPoses;

		private int boneCount;

		private bool poseUpdatedThisFrame;

		public float scale;

		public class SkeletonBlendablePose
		{
			public SteamVR_Skeleton_PoseSnapshot GetHandSnapshot(SteamVR_Input_Sources inputSource)
			{
				if (inputSource == SteamVR_Input_Sources.LeftHand)
				{
					return this.snapshotL;
				}
				return this.snapshotR;
			}

			public void UpdateAdditiveAnimation(SteamVR_Action_Skeleton skeletonAction, SteamVR_Input_Sources inputSource)
			{
				if (skeletonAction.GetSkeletalTrackingLevel() == EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated)
				{
					return;
				}
				SteamVR_Skeleton_PoseSnapshot handSnapshot = this.GetHandSnapshot(inputSource);
				SteamVR_Skeleton_Pose_Hand hand = this.pose.GetHand(inputSource);
				for (int i = 0; i < this.snapshotL.bonePositions.Length; i++)
				{
					int fingerForBone = SteamVR_Skeleton_JointIndexes.GetFingerForBone(i);
					SteamVR_Skeleton_FingerExtensionTypes movementTypeForBone = hand.GetMovementTypeForBone(i);
					if (movementTypeForBone == SteamVR_Skeleton_FingerExtensionTypes.Free)
					{
						handSnapshot.bonePositions[i] = skeletonAction.bonePositions[i];
						handSnapshot.boneRotations[i] = skeletonAction.boneRotations[i];
					}
					if (movementTypeForBone == SteamVR_Skeleton_FingerExtensionTypes.Extend)
					{
						handSnapshot.bonePositions[i] = Vector3.Lerp(hand.bonePositions[i], skeletonAction.bonePositions[i], 1f - skeletonAction.fingerCurls[fingerForBone]);
						handSnapshot.boneRotations[i] = Quaternion.Lerp(hand.boneRotations[i], skeletonAction.boneRotations[i], 1f - skeletonAction.fingerCurls[fingerForBone]);
					}
					if (movementTypeForBone == SteamVR_Skeleton_FingerExtensionTypes.Contract)
					{
						handSnapshot.bonePositions[i] = Vector3.Lerp(hand.bonePositions[i], skeletonAction.bonePositions[i], skeletonAction.fingerCurls[fingerForBone]);
						handSnapshot.boneRotations[i] = Quaternion.Lerp(hand.boneRotations[i], skeletonAction.boneRotations[i], skeletonAction.fingerCurls[fingerForBone]);
					}
				}
			}

			public SkeletonBlendablePose(SteamVR_Skeleton_Pose p)
			{
				this.pose = p;
				this.snapshotR = new SteamVR_Skeleton_PoseSnapshot(p.rightHand.bonePositions.Length, SteamVR_Input_Sources.RightHand);
				this.snapshotL = new SteamVR_Skeleton_PoseSnapshot(p.leftHand.bonePositions.Length, SteamVR_Input_Sources.LeftHand);
			}

			public void PoseToSnapshots()
			{
				this.snapshotR.position = this.pose.rightHand.position;
				this.snapshotR.rotation = this.pose.rightHand.rotation;
				this.pose.rightHand.bonePositions.CopyTo(this.snapshotR.bonePositions, 0);
				this.pose.rightHand.boneRotations.CopyTo(this.snapshotR.boneRotations, 0);
				this.snapshotL.position = this.pose.leftHand.position;
				this.snapshotL.rotation = this.pose.leftHand.rotation;
				this.pose.leftHand.bonePositions.CopyTo(this.snapshotL.bonePositions, 0);
				this.pose.leftHand.boneRotations.CopyTo(this.snapshotL.boneRotations, 0);
			}

			public SkeletonBlendablePose()
			{
			}

			public SteamVR_Skeleton_Pose pose;

			public SteamVR_Skeleton_PoseSnapshot snapshotR;

			public SteamVR_Skeleton_PoseSnapshot snapshotL;
		}

		[Serializable]
		public class PoseBlendingBehaviour
		{
			public void Update(float deltaTime, SteamVR_Input_Sources inputSource)
			{
				if (this.type == SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes.AnalogAction)
				{
					if (this.smoothingSpeed == 0f)
					{
						this.value = this.action_single.GetAxis(inputSource);
					}
					else
					{
						this.value = Mathf.Lerp(this.value, this.action_single.GetAxis(inputSource), deltaTime * this.smoothingSpeed);
					}
				}
				if (this.type == SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes.BooleanAction)
				{
					if (this.smoothingSpeed == 0f)
					{
						this.value = (float)(this.action_bool.GetState(inputSource) ? 1 : 0);
						return;
					}
					this.value = Mathf.Lerp(this.value, (float)(this.action_bool.GetState(inputSource) ? 1 : 0), deltaTime * this.smoothingSpeed);
				}
			}

			public void ApplyBlending(SteamVR_Skeleton_PoseSnapshot snapshot, SteamVR_Skeleton_Poser.SkeletonBlendablePose[] blendPoses, SteamVR_Input_Sources inputSource)
			{
				SteamVR_Skeleton_PoseSnapshot handSnapshot = blendPoses[this.pose].GetHandSnapshot(inputSource);
				if (this.mask.GetFinger(0) || !this.useMask)
				{
					snapshot.position = Vector3.Lerp(snapshot.position, handSnapshot.position, this.influence * this.value);
					snapshot.rotation = Quaternion.Slerp(snapshot.rotation, handSnapshot.rotation, this.influence * this.value);
				}
				for (int i = 0; i < snapshot.bonePositions.Length; i++)
				{
					if (this.mask.GetFinger(SteamVR_Skeleton_JointIndexes.GetFingerForBone(i) + 1) || !this.useMask)
					{
						snapshot.bonePositions[i] = Vector3.Lerp(snapshot.bonePositions[i], handSnapshot.bonePositions[i], this.influence * this.value);
						snapshot.boneRotations[i] = Quaternion.Slerp(snapshot.boneRotations[i], handSnapshot.boneRotations[i], this.influence * this.value);
					}
				}
			}

			public PoseBlendingBehaviour()
			{
				this.enabled = true;
				this.influence = 1f;
			}

			public string name;

			public bool enabled = true;

			public float influence = 1f;

			public int pose = 1;

			public float value;

			public SteamVR_Action_Single action_single;

			public SteamVR_Action_Boolean action_bool;

			public float smoothingSpeed;

			public SteamVR_Skeleton_Poser.PoseBlendingBehaviour.BlenderTypes type;

			public bool useMask;

			public SteamVR_Skeleton_HandMask mask = new SteamVR_Skeleton_HandMask();

			public bool previewEnabled;

			public enum BlenderTypes
			{
				Manual,
				AnalogAction,
				BooleanAction
			}
		}
	}
}
