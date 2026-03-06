using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class SyntheticHand : Hand
	{
		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				this._jointLockProgressCurves[i] = new ProgressCurve(this._jointLockCurve);
				this._jointUnlockProgressCurves[i] = new ProgressCurve(this._jointUnlockCurve);
			}
			this.EndStart(ref this._started);
		}

		protected override void Apply(HandDataAsset data)
		{
			if (!base.Started || !data.IsDataValid || !data.IsTracked || !data.IsHighConfidence)
			{
				data.IsConnected = false;
				data.RootPoseOrigin = PoseOrigin.None;
				this._hasConnectedData = false;
				return;
			}
			this.UpdateRequired();
			this._lastStates.CopyFrom(data);
			if (!this._hasConnectedData)
			{
				ref this._constrainedWristPose.CopyFrom(data.Root);
				this._hasConnectedData = true;
			}
			this.UpdateJointsRotation(data);
			this.UpdateRootPose(ref data.Root);
			this.SyncDataPoses(data);
			data.RootPoseOrigin = PoseOrigin.SyntheticPose;
		}

		private void SyncDataPoses(HandDataAsset data)
		{
			for (int i = 0; i < 26; i++)
			{
				int num = (int)HandJointUtils.JointParentList[i];
				if (num >= 0)
				{
					Vector3 position = PoseUtils.Delta(this._lastStates.JointPoses[num], this._lastStates.JointPoses[i]).position;
					Pose[] jointPoses = data.JointPoses;
					int num2 = num;
					Pose pose = new Pose(position, data.Joints[i]);
					PoseUtils.Multiply(jointPoses[num2], pose, ref data.JointPoses[i]);
				}
			}
		}

		private void UpdateRootPose(ref Pose root)
		{
			float t = this._wristPositionLocked ? this._wristPositionLockCurve.Progress() : this._wristPositionUnlockCurve.Progress();
			Vector3 b = Vector3.Lerp(root.position, this._desiredWristPose.position, this._wristPositionOverrideFactor);
			root.position = Vector3.Lerp(this._constrainedWristPose.position, b, t);
			float t2 = this._wristRotationLocked ? this._wristRotationLockCurve.Progress() : this._wristRotationUnlockCurve.Progress();
			Quaternion b2 = Quaternion.Lerp(root.rotation, this._desiredWristPose.rotation, this._wristRotationOverrideFactor);
			root.rotation = Quaternion.Lerp(this._constrainedWristPose.rotation, b2, t2);
			ref this._lastWristPose.CopyFrom(root);
		}

		private void UpdateJointsRotation(HandDataAsset data)
		{
			float num = 0f;
			Quaternion[] joints = data.Joints;
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				JointFreedom jointFreedom = this._jointsFreedomLevels[i];
				Quaternion quaternion = this.AmendMetacarpalRotation(i, joints);
				float t = this._jointsOverrideFactor[i];
				int num2 = (int)FingersMetadata.HAND_JOINT_IDS[i];
				if (jointFreedom != JointFreedom.Free)
				{
					if (jointFreedom == JointFreedom.Locked)
					{
						joints[num2] = Quaternion.Slerp(joints[num2], quaternion, t);
					}
					else if (jointFreedom == JointFreedom.Constrained)
					{
						bool flag = false;
						if (FingersMetadata.HAND_JOINT_CAN_SPREAD[i])
						{
							flag = true;
							num = 0f;
						}
						Vector3 rightThumbSide = Constants.RightThumbSide;
						Vector3 rightDorsal = Constants.RightDorsal;
						Quaternion quaternion2 = quaternion * Quaternion.Euler(rightThumbSide * -90f * num);
						float num3 = SyntheticHand.OverFlex(joints[num2], quaternion2);
						num = Mathf.Max(num, num3);
						if (num3 < 0f)
						{
							joints[num2] = Quaternion.Slerp(joints[num2], quaternion2, t);
						}
						else if (flag)
						{
							Quaternion quaternion3 = joints[num2];
							float d = Vector3.SignedAngle(quaternion3 * rightThumbSide, quaternion2 * rightThumbSide, quaternion3 * rightDorsal);
							float d2 = 1f - Mathf.Clamp01(num3 * this._spreadAllowance);
							quaternion3 *= Quaternion.Euler(rightDorsal * d * d2);
							joints[num2] = quaternion3;
						}
					}
				}
				float t2 = (this._jointsFreedomLevels[i] == JointFreedom.Free) ? this._jointUnlockProgressCurves[i].Progress() : this._jointLockProgressCurves[i].Progress();
				joints[num2] = Quaternion.Slerp(this._constrainedJointRotations[i], joints[num2], t2);
				this._lastSyntheticRotation[i] = joints[num2];
			}
		}

		private Quaternion AmendMetacarpalRotation(int jointIndex, in Quaternion[] sourceRotations)
		{
			HandJointId handJointId = FingersMetadata.HAND_JOINT_IDS[jointIndex];
			int num = (int)handJointId;
			if (handJointId == HandJointId.HandIndex0 || handJointId == HandJointId.HandMiddle0 || handJointId == HandJointId.HandRing0)
			{
				return sourceRotations[num];
			}
			if (handJointId == HandJointId.HandIndex1 || handJointId == HandJointId.HandMiddle1 || handJointId == HandJointId.HandRing1)
			{
				return Quaternion.Inverse(sourceRotations[num - 1]) * this._desiredJointsRotation[jointIndex];
			}
			return this._desiredJointsRotation[jointIndex];
		}

		public void OverrideAllJoints(in Quaternion[] jointRotations, float overrideFactor)
		{
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				this._desiredJointsRotation[i] = jointRotations[i];
				this._jointsOverrideFactor[i] = overrideFactor;
			}
		}

		public void OverrideFingerRotations(HandFinger finger, Quaternion[] rotations, float overrideFactor)
		{
			int[] array = FingersMetadata.FINGER_TO_JOINT_INDEX[(int)finger];
			for (int i = 0; i < array.Length; i++)
			{
				this.OverrideJointRotationAtIndex(array[i], rotations[i], overrideFactor);
			}
		}

		public void OverrideJointRotation(HandJointId jointId, Quaternion rotation, float overrideFactor)
		{
			int jointIndex = FingersMetadata.HandJointIdToIndex(jointId);
			this.OverrideJointRotationAtIndex(jointIndex, rotation, overrideFactor);
		}

		private void OverrideJointRotationAtIndex(int jointIndex, Quaternion rotation, float overrideFactor)
		{
			this._desiredJointsRotation[jointIndex] = rotation;
			this._jointsOverrideFactor[jointIndex] = overrideFactor;
		}

		public void LockFingerAtCurrent(in HandFinger finger)
		{
			JointFreedom jointFreedom = JointFreedom.Locked;
			this.SetFingerFreedom(finger, jointFreedom, false);
			int num = (int)finger;
			foreach (int num2 in FingersMetadata.FINGER_TO_JOINT_INDEX[num])
			{
				int num3 = (int)FingersMetadata.HAND_JOINT_IDS[num2];
				this._desiredJointsRotation[num2] = this._lastStates.Joints[num3];
				this._jointsOverrideFactor[num2] = 1f;
			}
		}

		public void LockJoint(in HandJointId jointId, Quaternion rotation, float overrideFactor = 1f)
		{
			int num = FingersMetadata.HandJointIdToIndex(jointId);
			this._desiredJointsRotation[num] = rotation;
			this._jointsOverrideFactor[num] = 1f;
			int jointId2 = num;
			JointFreedom jointFreedom = JointFreedom.Locked;
			this.SetJointFreedomAtIndex(jointId2, jointFreedom, false);
		}

		public void SetFingerFreedom(in HandFinger finger, in JointFreedom freedomLevel, bool skipAnimation = false)
		{
			int[] array = FingersMetadata.FINGER_TO_JOINT_INDEX[(int)finger];
			for (int i = 0; i < array.Length; i++)
			{
				this.SetJointFreedomAtIndex(array[i], freedomLevel, skipAnimation);
			}
		}

		public void SetJointFreedom(in HandJointId jointId, in JointFreedom freedomLevel, bool skipAnimation = false)
		{
			int jointId2 = FingersMetadata.HandJointIdToIndex(jointId);
			this.SetJointFreedomAtIndex(jointId2, freedomLevel, skipAnimation);
		}

		public JointFreedom GetJointFreedom(in HandJointId jointId)
		{
			int num = FingersMetadata.HandJointIdToIndex(jointId);
			return this._jointsFreedomLevels[num];
		}

		public void FreeAllJoints()
		{
			for (int i = 0; i < FingersMetadata.HAND_JOINT_IDS.Length; i++)
			{
				int jointId = i;
				JointFreedom jointFreedom = JointFreedom.Free;
				this.SetJointFreedomAtIndex(jointId, jointFreedom, false);
			}
		}

		private void SetJointFreedomAtIndex(int jointId, in JointFreedom freedomLevel, bool skipAnimation = false)
		{
			if (this._jointsFreedomLevels[jointId] != freedomLevel)
			{
				bool locked = freedomLevel == JointFreedom.Locked || freedomLevel == JointFreedom.Constrained;
				SyntheticHand.UpdateProgressCurve(ref this._jointLockProgressCurves[jointId], ref this._jointUnlockProgressCurves[jointId], locked, skipAnimation);
				this._constrainedJointRotations[jointId] = this._lastSyntheticRotation[jointId];
			}
			this._jointsFreedomLevels[jointId] = freedomLevel;
		}

		public void LockWristPose(Pose wristPose, float overrideFactor = 1f, SyntheticHand.WristLockMode lockMode = SyntheticHand.WristLockMode.Full, bool worldPose = false, bool skipAnimation = false)
		{
			Pose pose = (worldPose && base.TrackingToWorldTransformer != null) ? base.TrackingToWorldTransformer.ToTrackingPose(wristPose) : wristPose;
			if ((lockMode & SyntheticHand.WristLockMode.Position) != (SyntheticHand.WristLockMode)0)
			{
				this.LockWristPosition(pose.position, overrideFactor, skipAnimation);
			}
			if ((lockMode & SyntheticHand.WristLockMode.Rotation) != (SyntheticHand.WristLockMode)0)
			{
				this.LockWristRotation(pose.rotation, overrideFactor, skipAnimation);
			}
		}

		public void LockWristPosition(Vector3 position, float overrideFactor = 1f, bool skipAnimation = false)
		{
			this._wristPositionOverrideFactor = overrideFactor;
			this._desiredWristPose.position = position;
			if (!this._wristPositionLocked)
			{
				this._wristPositionLocked = true;
				this.SyntheticWristLockChangedState(SyntheticHand.WristLockMode.Position, skipAnimation);
			}
		}

		public void LockWristRotation(Quaternion rotation, float overrideFactor = 1f, bool skipAnimation = false)
		{
			this._wristRotationOverrideFactor = overrideFactor;
			this._desiredWristPose.rotation = rotation;
			if (!this._wristRotationLocked)
			{
				this._wristRotationLocked = true;
				this.SyntheticWristLockChangedState(SyntheticHand.WristLockMode.Rotation, skipAnimation);
			}
		}

		public void FreeWrist(SyntheticHand.WristLockMode lockMode = SyntheticHand.WristLockMode.Full)
		{
			if ((lockMode & SyntheticHand.WristLockMode.Position) != (SyntheticHand.WristLockMode)0 && this._wristPositionLocked)
			{
				this._wristPositionOverrideFactor = 0f;
				this._wristPositionLocked = false;
				this.SyntheticWristLockChangedState(SyntheticHand.WristLockMode.Position, false);
			}
			if ((lockMode & SyntheticHand.WristLockMode.Rotation) != (SyntheticHand.WristLockMode)0 && this._wristRotationLocked)
			{
				this._wristRotationOverrideFactor = 0f;
				this._wristRotationLocked = false;
				this.SyntheticWristLockChangedState(SyntheticHand.WristLockMode.Rotation, false);
			}
		}

		private void SyntheticWristLockChangedState(SyntheticHand.WristLockMode lockMode, bool skipAnimation = false)
		{
			if ((lockMode & SyntheticHand.WristLockMode.Position) != (SyntheticHand.WristLockMode)0)
			{
				SyntheticHand.UpdateProgressCurve(ref this._wristPositionLockCurve, ref this._wristPositionUnlockCurve, this._wristPositionLocked, skipAnimation);
				this._constrainedWristPose.position = this._lastWristPose.position;
			}
			if ((lockMode & SyntheticHand.WristLockMode.Rotation) != (SyntheticHand.WristLockMode)0)
			{
				SyntheticHand.UpdateProgressCurve(ref this._wristRotationLockCurve, ref this._wristRotationUnlockCurve, this._wristRotationLocked, skipAnimation);
				this._constrainedWristPose.rotation = this._lastWristPose.rotation;
			}
		}

		private static float OverFlex(in Quaternion desiredLocalRot, in Quaternion maxLocalRot)
		{
			Vector3 lhs = desiredLocalRot * Constants.RightDistal;
			Vector3 lhs2 = desiredLocalRot * Constants.RightPinkySide;
			Vector3 rhs = maxLocalRot * Constants.RightDistal;
			Vector3 rhs2 = Vector3.Cross(lhs, rhs);
			return Vector3.Dot(lhs2, rhs2);
		}

		private static void UpdateProgressCurve(ref ProgressCurve lockProgress, ref ProgressCurve unlockProgress, bool locked, bool skipAnimation)
		{
			ProgressCurve progressCurve = locked ? lockProgress : unlockProgress;
			if (skipAnimation)
			{
				progressCurve.End();
				return;
			}
			progressCurve.Start();
		}

		public void InjectAllSyntheticHandModifier(DataSource<HandDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier, ProgressCurve wristPositionLockCurve, ProgressCurve wristPositionUnlockCurve, ProgressCurve wristRotationLockCurve, ProgressCurve wristRotationUnlockCurve, ProgressCurve jointLockCurve, ProgressCurve jointUnlockCurve, float spreadAllowance)
		{
			base.InjectAllHand(updateMode, updateAfter, modifyDataFromSource, applyModifier);
			this.InjectWristPositionLockCurve(wristPositionLockCurve);
			this.InjectWristPositionUnlockCurve(wristPositionUnlockCurve);
			this.InjectWristRotationLockCurve(wristRotationLockCurve);
			this.InjectWristRotationUnlockCurve(wristRotationUnlockCurve);
			this.InjectJointLockCurve(jointLockCurve);
			this.InjectJointUnlockCurve(jointUnlockCurve);
			this.InjectSpreadAllowance(spreadAllowance);
		}

		public void InjectWristPositionLockCurve(ProgressCurve wristPositionLockCurve)
		{
			this._wristPositionLockCurve = wristPositionLockCurve;
		}

		public void InjectWristPositionUnlockCurve(ProgressCurve wristPositionUnlockCurve)
		{
			this._wristPositionUnlockCurve = wristPositionUnlockCurve;
		}

		public void InjectWristRotationLockCurve(ProgressCurve wristRotationLockCurve)
		{
			this._wristRotationLockCurve = wristRotationLockCurve;
		}

		public void InjectWristRotationUnlockCurve(ProgressCurve wristRotationUnlockCurve)
		{
			this._wristRotationUnlockCurve = wristRotationUnlockCurve;
		}

		public void InjectJointLockCurve(ProgressCurve jointLockCurve)
		{
			this._jointLockCurve = jointLockCurve;
		}

		public void InjectJointUnlockCurve(ProgressCurve jointUnlockCurve)
		{
			this._jointUnlockCurve = jointUnlockCurve;
		}

		public void InjectSpreadAllowance(float spreadAllowance)
		{
			this._spreadAllowance = spreadAllowance;
		}

		[SerializeField]
		private ProgressCurve _wristPositionLockCurve;

		[SerializeField]
		private ProgressCurve _wristPositionUnlockCurve;

		[SerializeField]
		private ProgressCurve _wristRotationLockCurve;

		[SerializeField]
		private ProgressCurve _wristRotationUnlockCurve;

		[SerializeField]
		private ProgressCurve _jointLockCurve;

		[SerializeField]
		private ProgressCurve _jointUnlockCurve;

		[SerializeField]
		[Tooltip("Use this factor to control how much the fingers can spread when nearby a constrained pose.")]
		private float _spreadAllowance = 5f;

		public Action UpdateRequired = delegate()
		{
		};

		private readonly HandDataAsset _lastStates = new HandDataAsset();

		private float _wristPositionOverrideFactor;

		private float _wristRotationOverrideFactor;

		private float[] _jointsOverrideFactor = new float[FingersMetadata.HAND_JOINT_IDS.Length];

		private ProgressCurve[] _jointLockProgressCurves = new ProgressCurve[FingersMetadata.HAND_JOINT_IDS.Length];

		private ProgressCurve[] _jointUnlockProgressCurves = new ProgressCurve[FingersMetadata.HAND_JOINT_IDS.Length];

		private Pose _desiredWristPose;

		private bool _wristPositionLocked;

		private bool _wristRotationLocked;

		private Pose _constrainedWristPose;

		private Pose _lastWristPose;

		private Quaternion[] _desiredJointsRotation = new Quaternion[FingersMetadata.HAND_JOINT_IDS.Length];

		private Quaternion[] _constrainedJointRotations = new Quaternion[FingersMetadata.HAND_JOINT_IDS.Length];

		private Quaternion[] _lastSyntheticRotation = new Quaternion[FingersMetadata.HAND_JOINT_IDS.Length];

		private JointFreedom[] _jointsFreedomLevels = new JointFreedom[FingersMetadata.HAND_JOINT_IDS.Length];

		private bool _hasConnectedData;

		[Flags]
		public enum WristLockMode
		{
			Position = 1,
			Rotation = 2,
			Full = 3
		}
	}
}
