using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Action_Skeleton_Source : SteamVR_Action_Pose_Source, ISteamVR_Action_Skeleton_Source
	{
		public new event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveChange;

		public new event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveBindingChange;

		public new event SteamVR_Action_Skeleton.ChangeHandler onChange;

		public new event SteamVR_Action_Skeleton.UpdateHandler onUpdate;

		public new event SteamVR_Action_Skeleton.TrackingChangeHandler onTrackingChanged;

		public new event SteamVR_Action_Skeleton.ValidPoseChangeHandler onValidPoseChanged;

		public new event SteamVR_Action_Skeleton.DeviceConnectedChangeHandler onDeviceConnectedChanged;

		public override bool activeBinding
		{
			get
			{
				return this.skeletonActionData.bActive;
			}
		}

		public override bool lastActiveBinding
		{
			get
			{
				return this.lastSkeletonActionData.bActive;
			}
		}

		public Vector3[] bonePositions { get; protected set; }

		public Quaternion[] boneRotations { get; protected set; }

		public Vector3[] lastBonePositions { get; protected set; }

		public Quaternion[] lastBoneRotations { get; protected set; }

		public EVRSkeletalMotionRange rangeOfMotion { get; set; }

		public EVRSkeletalTransformSpace skeletalTransformSpace { get; set; }

		public EVRSummaryType summaryDataType { get; set; }

		public float thumbCurl
		{
			get
			{
				return this.fingerCurls[0];
			}
		}

		public float indexCurl
		{
			get
			{
				return this.fingerCurls[1];
			}
		}

		public float middleCurl
		{
			get
			{
				return this.fingerCurls[2];
			}
		}

		public float ringCurl
		{
			get
			{
				return this.fingerCurls[3];
			}
		}

		public float pinkyCurl
		{
			get
			{
				return this.fingerCurls[4];
			}
		}

		public float thumbIndexSplay
		{
			get
			{
				return this.fingerSplays[0];
			}
		}

		public float indexMiddleSplay
		{
			get
			{
				return this.fingerSplays[1];
			}
		}

		public float middleRingSplay
		{
			get
			{
				return this.fingerSplays[2];
			}
		}

		public float ringPinkySplay
		{
			get
			{
				return this.fingerSplays[3];
			}
		}

		public float lastThumbCurl
		{
			get
			{
				return this.lastFingerCurls[0];
			}
		}

		public float lastIndexCurl
		{
			get
			{
				return this.lastFingerCurls[1];
			}
		}

		public float lastMiddleCurl
		{
			get
			{
				return this.lastFingerCurls[2];
			}
		}

		public float lastRingCurl
		{
			get
			{
				return this.lastFingerCurls[3];
			}
		}

		public float lastPinkyCurl
		{
			get
			{
				return this.lastFingerCurls[4];
			}
		}

		public float lastThumbIndexSplay
		{
			get
			{
				return this.lastFingerSplays[0];
			}
		}

		public float lastIndexMiddleSplay
		{
			get
			{
				return this.lastFingerSplays[1];
			}
		}

		public float lastMiddleRingSplay
		{
			get
			{
				return this.lastFingerSplays[2];
			}
		}

		public float lastRingPinkySplay
		{
			get
			{
				return this.lastFingerSplays[3];
			}
		}

		public float[] fingerCurls { get; protected set; }

		public float[] fingerSplays { get; protected set; }

		public float[] lastFingerCurls { get; protected set; }

		public float[] lastFingerSplays { get; protected set; }

		public bool poseChanged { get; protected set; }

		public bool onlyUpdateSummaryData { get; set; }

		public override void Preinitialize(SteamVR_Action wrappingAction, SteamVR_Input_Sources forInputSource)
		{
			base.Preinitialize(wrappingAction, forInputSource);
			this.skeletonAction = (SteamVR_Action_Skeleton)wrappingAction;
			this.bonePositions = new Vector3[31];
			this.lastBonePositions = new Vector3[31];
			this.boneRotations = new Quaternion[31];
			this.lastBoneRotations = new Quaternion[31];
			this.rangeOfMotion = EVRSkeletalMotionRange.WithController;
			this.skeletalTransformSpace = EVRSkeletalTransformSpace.Parent;
			this.fingerCurls = new float[SteamVR_Skeleton_FingerIndexes.enumArray.Length];
			this.fingerSplays = new float[SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length];
			this.lastFingerCurls = new float[SteamVR_Skeleton_FingerIndexes.enumArray.Length];
			this.lastFingerSplays = new float[SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length];
		}

		public override void Initialize()
		{
			base.Initialize();
			if (SteamVR_Action_Skeleton_Source.skeletonActionData_size == 0U)
			{
				SteamVR_Action_Skeleton_Source.skeletonActionData_size = (uint)Marshal.SizeOf(typeof(InputSkeletalActionData_t));
			}
		}

		public override void RemoveAllListeners()
		{
			base.RemoveAllListeners();
			if (this.onActiveChange != null)
			{
				Delegate[] invocationList = this.onActiveChange.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate @delegate in invocationList)
					{
						this.onActiveChange -= (SteamVR_Action_Skeleton.ActiveChangeHandler)@delegate;
					}
				}
			}
			if (this.onChange != null)
			{
				Delegate[] invocationList = this.onChange.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate2 in invocationList)
					{
						this.onChange -= (SteamVR_Action_Skeleton.ChangeHandler)delegate2;
					}
				}
			}
			if (this.onUpdate != null)
			{
				Delegate[] invocationList = this.onUpdate.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate3 in invocationList)
					{
						this.onUpdate -= (SteamVR_Action_Skeleton.UpdateHandler)delegate3;
					}
				}
			}
			if (this.onTrackingChanged != null)
			{
				Delegate[] invocationList = this.onTrackingChanged.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate4 in invocationList)
					{
						this.onTrackingChanged -= (SteamVR_Action_Skeleton.TrackingChangeHandler)delegate4;
					}
				}
			}
			if (this.onValidPoseChanged != null)
			{
				Delegate[] invocationList = this.onValidPoseChanged.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate5 in invocationList)
					{
						this.onValidPoseChanged -= (SteamVR_Action_Skeleton.ValidPoseChangeHandler)delegate5;
					}
				}
			}
			if (this.onDeviceConnectedChanged != null)
			{
				Delegate[] invocationList = this.onDeviceConnectedChanged.GetInvocationList();
				if (invocationList != null)
				{
					foreach (Delegate delegate6 in invocationList)
					{
						this.onDeviceConnectedChanged -= (SteamVR_Action_Skeleton.DeviceConnectedChangeHandler)delegate6;
					}
				}
			}
		}

		public override void UpdateValue()
		{
			this.UpdateValue(false);
		}

		public override void UpdateValue(bool skipStateAndEventUpdates)
		{
			this.lastActive = this.active;
			this.lastSkeletonActionData = this.skeletonActionData;
			this.lastSkeletalSummaryData = this.skeletalSummaryData;
			if (!this.onlyUpdateSummaryData)
			{
				for (int i = 0; i < 31; i++)
				{
					this.lastBonePositions[i] = this.bonePositions[i];
					this.lastBoneRotations[i] = this.boneRotations[i];
				}
			}
			for (int j = 0; j < SteamVR_Skeleton_FingerIndexes.enumArray.Length; j++)
			{
				this.lastFingerCurls[j] = this.fingerCurls[j];
			}
			for (int k = 0; k < SteamVR_Skeleton_FingerSplayIndexes.enumArray.Length; k++)
			{
				this.lastFingerSplays[k] = this.fingerSplays[k];
			}
			base.UpdateValue(true);
			this.poseChanged = this.changed;
			EVRInputError evrinputError = OpenVR.Input.GetSkeletalActionData(base.handle, ref this.skeletonActionData, SteamVR_Action_Skeleton_Source.skeletonActionData_size);
			if (evrinputError != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetSkeletalActionData error (",
					base.fullPath,
					"): ",
					evrinputError.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
				return;
			}
			if (this.active)
			{
				if (!this.onlyUpdateSummaryData)
				{
					evrinputError = OpenVR.Input.GetSkeletalBoneData(base.handle, this.skeletalTransformSpace, this.rangeOfMotion, this.tempBoneTransforms);
					if (evrinputError != EVRInputError.None)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"<b>[SteamVR]</b> GetSkeletalBoneData error (",
							base.fullPath,
							"): ",
							evrinputError.ToString(),
							" handle: ",
							base.handle.ToString()
						}));
					}
					for (int l = 0; l < this.tempBoneTransforms.Length; l++)
					{
						this.bonePositions[l].x = -this.tempBoneTransforms[l].position.v0;
						this.bonePositions[l].y = this.tempBoneTransforms[l].position.v1;
						this.bonePositions[l].z = this.tempBoneTransforms[l].position.v2;
						this.boneRotations[l].x = this.tempBoneTransforms[l].orientation.x;
						this.boneRotations[l].y = -this.tempBoneTransforms[l].orientation.y;
						this.boneRotations[l].z = -this.tempBoneTransforms[l].orientation.z;
						this.boneRotations[l].w = this.tempBoneTransforms[l].orientation.w;
					}
					this.boneRotations[0] = SteamVR_Action_Skeleton.steamVRFixUpRotation * this.boneRotations[0];
				}
				this.UpdateSkeletalSummaryData(this.summaryDataType, true);
			}
			if (!this.changed)
			{
				for (int m = 0; m < this.tempBoneTransforms.Length; m++)
				{
					if (Vector3.Distance(this.lastBonePositions[m], this.bonePositions[m]) > this.changeTolerance)
					{
						this.changed = true;
						break;
					}
					if (Mathf.Abs(Quaternion.Angle(this.lastBoneRotations[m], this.boneRotations[m])) > this.changeTolerance)
					{
						this.changed = true;
						break;
					}
				}
			}
			if (this.changed)
			{
				base.changedTime = Time.realtimeSinceStartup;
			}
			if (!skipStateAndEventUpdates)
			{
				this.CheckAndSendEvents();
			}
		}

		public int boneCount
		{
			get
			{
				return (int)this.GetBoneCount();
			}
		}

		public uint GetBoneCount()
		{
			uint result = 0U;
			EVRInputError boneCount = OpenVR.Input.GetBoneCount(base.handle, ref result);
			if (boneCount != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetBoneCount error (",
					base.fullPath,
					"): ",
					boneCount.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			return result;
		}

		public int[] boneHierarchy
		{
			get
			{
				return this.GetBoneHierarchy();
			}
		}

		public int[] GetBoneHierarchy()
		{
			int[] array = new int[this.GetBoneCount()];
			EVRInputError boneHierarchy = OpenVR.Input.GetBoneHierarchy(base.handle, array);
			if (boneHierarchy != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetBoneHierarchy error (",
					base.fullPath,
					"): ",
					boneHierarchy.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			return array;
		}

		public string GetBoneName(int boneIndex)
		{
			StringBuilder stringBuilder = new StringBuilder(255);
			EVRInputError boneName = OpenVR.Input.GetBoneName(base.handle, boneIndex, stringBuilder, 255U);
			if (boneName != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetBoneName error (",
					base.fullPath,
					"): ",
					boneName.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			return stringBuilder.ToString();
		}

		public SteamVR_Utils.RigidTransform[] GetReferenceTransforms(EVRSkeletalTransformSpace transformSpace, EVRSkeletalReferencePose referencePose)
		{
			SteamVR_Utils.RigidTransform[] array = new SteamVR_Utils.RigidTransform[this.GetBoneCount()];
			VRBoneTransform_t[] array2 = new VRBoneTransform_t[array.Length];
			EVRInputError skeletalReferenceTransforms = OpenVR.Input.GetSkeletalReferenceTransforms(base.handle, transformSpace, referencePose, array2);
			if (skeletalReferenceTransforms != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetSkeletalReferenceTransforms error (",
					base.fullPath,
					"): ",
					skeletalReferenceTransforms.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			for (int i = 0; i < array2.Length; i++)
			{
				Vector3 pos = new Vector3(-array2[i].position.v0, array2[i].position.v1, array2[i].position.v2);
				Quaternion rot = new Quaternion(array2[i].orientation.x, -array2[i].orientation.y, -array2[i].orientation.z, array2[i].orientation.w);
				array[i] = new SteamVR_Utils.RigidTransform(pos, rot);
			}
			if (array.Length != 0)
			{
				Quaternion lhs = Quaternion.AngleAxis(180f, Vector3.up);
				array[0].rot = lhs * array[0].rot;
			}
			return array;
		}

		public EVRSkeletalTrackingLevel skeletalTrackingLevel
		{
			get
			{
				return this.GetSkeletalTrackingLevel();
			}
		}

		public EVRSkeletalTrackingLevel GetSkeletalTrackingLevel()
		{
			EVRSkeletalTrackingLevel result = EVRSkeletalTrackingLevel.VRSkeletalTracking_Estimated;
			EVRInputError skeletalTrackingLevel = OpenVR.Input.GetSkeletalTrackingLevel(base.handle, ref result);
			if (skeletalTrackingLevel != EVRInputError.None)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"<b>[SteamVR]</b> GetSkeletalTrackingLevel error (",
					base.fullPath,
					"): ",
					skeletalTrackingLevel.ToString(),
					" handle: ",
					base.handle.ToString()
				}));
			}
			return result;
		}

		protected VRSkeletalSummaryData_t GetSkeletalSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromAnimation, bool force = false)
		{
			this.UpdateSkeletalSummaryData(summaryType, force);
			return this.skeletalSummaryData;
		}

		protected void UpdateSkeletalSummaryData(EVRSummaryType summaryType = EVRSummaryType.FromAnimation, bool force = false)
		{
			if (force || (this.summaryDataType != this.summaryDataType && this.active))
			{
				EVRInputError evrinputError = OpenVR.Input.GetSkeletalSummaryData(base.handle, summaryType, ref this.skeletalSummaryData);
				if (evrinputError != EVRInputError.None)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"<b>[SteamVR]</b> GetSkeletalSummaryData error (",
						base.fullPath,
						"): ",
						evrinputError.ToString(),
						" handle: ",
						base.handle.ToString()
					}));
				}
				this.fingerCurls[0] = this.skeletalSummaryData.flFingerCurl0;
				this.fingerCurls[1] = this.skeletalSummaryData.flFingerCurl1;
				this.fingerCurls[2] = this.skeletalSummaryData.flFingerCurl2;
				this.fingerCurls[3] = this.skeletalSummaryData.flFingerCurl3;
				this.fingerCurls[4] = this.skeletalSummaryData.flFingerCurl4;
				this.fingerSplays[0] = this.skeletalSummaryData.flFingerSplay0;
				this.fingerSplays[1] = this.skeletalSummaryData.flFingerSplay1;
				this.fingerSplays[2] = this.skeletalSummaryData.flFingerSplay2;
				this.fingerSplays[3] = this.skeletalSummaryData.flFingerSplay3;
			}
		}

		protected override void CheckAndSendEvents()
		{
			if (base.trackingState != base.lastTrackingState && this.onTrackingChanged != null)
			{
				this.onTrackingChanged(this.skeletonAction, base.trackingState);
			}
			if (base.poseIsValid != base.lastPoseIsValid && this.onValidPoseChanged != null)
			{
				this.onValidPoseChanged(this.skeletonAction, base.poseIsValid);
			}
			if (base.deviceIsConnected != base.lastDeviceIsConnected && this.onDeviceConnectedChanged != null)
			{
				this.onDeviceConnectedChanged(this.skeletonAction, base.deviceIsConnected);
			}
			if (this.changed && this.onChange != null)
			{
				this.onChange(this.skeletonAction);
			}
			if (this.active != this.lastActive && this.onActiveChange != null)
			{
				this.onActiveChange(this.skeletonAction, this.active);
			}
			if (this.activeBinding != this.lastActiveBinding && this.onActiveBindingChange != null)
			{
				this.onActiveBindingChange(this.skeletonAction, this.activeBinding);
			}
			if (this.onUpdate != null)
			{
				this.onUpdate(this.skeletonAction);
			}
		}

		protected static uint skeletonActionData_size;

		protected VRSkeletalSummaryData_t skeletalSummaryData;

		protected VRSkeletalSummaryData_t lastSkeletalSummaryData;

		protected SteamVR_Action_Skeleton skeletonAction;

		protected VRBoneTransform_t[] tempBoneTransforms = new VRBoneTransform_t[31];

		protected InputSkeletalActionData_t skeletonActionData;

		protected InputSkeletalActionData_t lastSkeletonActionData;

		protected InputSkeletalActionData_t tempSkeletonActionData;
	}
}
