using System;
using UnityEngine;

namespace Valve.VR
{
	[Serializable]
	public class SteamVR_Action_Skeleton : SteamVR_Action_Pose_Base<SteamVR_Action_Skeleton_Source_Map, SteamVR_Action_Skeleton_Source>, ISteamVR_Action_Skeleton_Source, ISerializationCallbackReceiver
	{
		public event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveChange
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= value;
			}
		}

		public event SteamVR_Action_Skeleton.ActiveChangeHandler onActiveBindingChange
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onActiveBindingChange -= value;
			}
		}

		public event SteamVR_Action_Skeleton.ChangeHandler onChange
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onChange += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onChange -= value;
			}
		}

		public event SteamVR_Action_Skeleton.UpdateHandler onUpdate
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= value;
			}
		}

		public event SteamVR_Action_Skeleton.TrackingChangeHandler onTrackingChanged
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= value;
			}
		}

		public event SteamVR_Action_Skeleton.ValidPoseChangeHandler onValidPoseChanged
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= value;
			}
		}

		public event SteamVR_Action_Skeleton.DeviceConnectedChangeHandler onDeviceConnectedChanged
		{
			add
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += value;
			}
			remove
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= value;
			}
		}

		public virtual void UpdateValue(bool skipStateAndEventUpdates)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].UpdateValue(skipStateAndEventUpdates);
		}

		public void UpdateValueWithoutEvents()
		{
			this.sourceMap[SteamVR_Input_Sources.Any].UpdateValue(true);
		}

		public void UpdateTransform(Transform transformToUpdate)
		{
			base.UpdateTransform(SteamVR_Input_Sources.Any, transformToUpdate);
		}

		public Vector3[] bonePositions
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].bonePositions;
			}
		}

		public Quaternion[] boneRotations
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].boneRotations;
			}
		}

		public Vector3[] lastBonePositions
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastBonePositions;
			}
		}

		public Quaternion[] lastBoneRotations
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations;
			}
		}

		public EVRSkeletalMotionRange rangeOfMotion
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion;
			}
			set
			{
				this.sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion = value;
			}
		}

		public EVRSkeletalTransformSpace skeletalTransformSpace
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace;
			}
			set
			{
				this.sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace = value;
			}
		}

		public EVRSummaryType summaryDataType
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].summaryDataType;
			}
			set
			{
				this.sourceMap[SteamVR_Input_Sources.Any].summaryDataType = value;
			}
		}

		public EVRSkeletalTrackingLevel skeletalTrackingLevel
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].skeletalTrackingLevel;
			}
		}

		public float thumbCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].thumbCurl;
			}
		}

		public float indexCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].indexCurl;
			}
		}

		public float middleCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].middleCurl;
			}
		}

		public float ringCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].ringCurl;
			}
		}

		public float pinkyCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].pinkyCurl;
			}
		}

		public float thumbIndexSplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].thumbIndexSplay;
			}
		}

		public float indexMiddleSplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].indexMiddleSplay;
			}
		}

		public float middleRingSplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].middleRingSplay;
			}
		}

		public float ringPinkySplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].ringPinkySplay;
			}
		}

		public float lastThumbCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastThumbCurl;
			}
		}

		public float lastIndexCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastIndexCurl;
			}
		}

		public float lastMiddleCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastMiddleCurl;
			}
		}

		public float lastRingCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastRingCurl;
			}
		}

		public float lastPinkyCurl
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastPinkyCurl;
			}
		}

		public float lastThumbIndexSplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastThumbIndexSplay;
			}
		}

		public float lastIndexMiddleSplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastIndexMiddleSplay;
			}
		}

		public float lastMiddleRingSplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastMiddleRingSplay;
			}
		}

		public float lastRingPinkySplay
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastRingPinkySplay;
			}
		}

		public float[] fingerCurls
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls;
			}
		}

		public float[] fingerSplays
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays;
			}
		}

		public float[] lastFingerCurls
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls;
			}
		}

		public float[] lastFingerSplays
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays;
			}
		}

		public bool poseChanged
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].poseChanged;
			}
		}

		public bool onlyUpdateSummaryData
		{
			get
			{
				return this.sourceMap[SteamVR_Input_Sources.Any].onlyUpdateSummaryData;
			}
			set
			{
				this.sourceMap[SteamVR_Input_Sources.Any].onlyUpdateSummaryData = value;
			}
		}

		public bool GetActive()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].active;
		}

		public bool GetSetActive()
		{
			return this.actionSet.IsActive(SteamVR_Input_Sources.Any);
		}

		public bool GetVelocitiesAtTimeOffset(float secondsFromNow, out Vector3 velocity, out Vector3 angularVelocity)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetVelocitiesAtTimeOffset(secondsFromNow, out velocity, out angularVelocity);
		}

		public bool GetPoseAtTimeOffset(float secondsFromNow, out Vector3 position, out Quaternion rotation, out Vector3 velocity, out Vector3 angularVelocity)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetPoseAtTimeOffset(secondsFromNow, out position, out rotation, out velocity, out angularVelocity);
		}

		public Vector3 GetLocalPosition()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].localPosition;
		}

		public Quaternion GetLocalRotation()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].localRotation;
		}

		public Vector3 GetVelocity()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].velocity;
		}

		public Vector3 GetAngularVelocity()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].angularVelocity;
		}

		public bool GetDeviceIsConnected()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].deviceIsConnected;
		}

		public bool GetPoseIsValid()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].poseIsValid;
		}

		public ETrackingResult GetTrackingResult()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].trackingState;
		}

		public Vector3 GetLastLocalPosition()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastLocalPosition;
		}

		public Quaternion GetLastLocalRotation()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastLocalRotation;
		}

		public Vector3 GetLastVelocity()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastVelocity;
		}

		public Vector3 GetLastAngularVelocity()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastAngularVelocity;
		}

		public bool GetLastDeviceIsConnected()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastDeviceIsConnected;
		}

		public bool GetLastPoseIsValid()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastPoseIsValid;
		}

		public ETrackingResult GetLastTrackingResult()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastTrackingState;
		}

		public int boneCount
		{
			get
			{
				return (int)this.GetBoneCount();
			}
		}

		public Vector3[] GetBonePositions(bool copy = false)
		{
			if (copy)
			{
				return (Vector3[])this.sourceMap[SteamVR_Input_Sources.Any].bonePositions.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].bonePositions;
		}

		public Quaternion[] GetBoneRotations(bool copy = false)
		{
			if (copy)
			{
				return (Quaternion[])this.sourceMap[SteamVR_Input_Sources.Any].boneRotations.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].boneRotations;
		}

		public Vector3[] GetLastBonePositions(bool copy = false)
		{
			if (copy)
			{
				return (Vector3[])this.sourceMap[SteamVR_Input_Sources.Any].lastBonePositions.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].lastBonePositions;
		}

		public Quaternion[] GetLastBoneRotations(bool copy = false)
		{
			if (copy)
			{
				return (Quaternion[])this.sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].lastBoneRotations;
		}

		public void SetRangeOfMotion(EVRSkeletalMotionRange range)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].rangeOfMotion = range;
		}

		public void SetSkeletalTransformSpace(EVRSkeletalTransformSpace space)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].skeletalTransformSpace = space;
		}

		public uint GetBoneCount()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetBoneCount();
		}

		public int[] GetBoneHierarchy()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetBoneHierarchy();
		}

		public string GetBoneName(int boneIndex)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetBoneName(boneIndex);
		}

		public SteamVR_Utils.RigidTransform[] GetReferenceTransforms(EVRSkeletalTransformSpace transformSpace, EVRSkeletalReferencePose referencePose)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetReferenceTransforms(transformSpace, referencePose);
		}

		public EVRSkeletalTrackingLevel GetSkeletalTrackingLevel()
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetSkeletalTrackingLevel();
		}

		public float[] GetFingerCurls(bool copy = false)
		{
			if (copy)
			{
				return (float[])this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls;
		}

		public float[] GetLastFingerCurls(bool copy = false)
		{
			if (copy)
			{
				return (float[])this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls;
		}

		public float[] GetFingerSplays(bool copy = false)
		{
			if (copy)
			{
				return (float[])this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays;
		}

		public float[] GetLastFingerSplays(bool copy = false)
		{
			if (copy)
			{
				return (float[])this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays.Clone();
			}
			return this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays;
		}

		public float GetFingerCurl(int finger)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].fingerCurls[finger];
		}

		public float GetSplay(int fingerGapIndex)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].fingerSplays[fingerGapIndex];
		}

		public float GetFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger)
		{
			return this.GetFingerCurl((int)finger);
		}

		public float GetSplay(SteamVR_Skeleton_FingerSplayIndexEnum fingerSplay)
		{
			return this.GetSplay((int)fingerSplay);
		}

		public float GetLastFingerCurl(int finger)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastFingerCurls[finger];
		}

		public float GetLastSplay(int fingerGapIndex)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].lastFingerSplays[fingerGapIndex];
		}

		public float GetLastFingerCurl(SteamVR_Skeleton_FingerIndexEnum finger)
		{
			return this.GetLastFingerCurl((int)finger);
		}

		public float GetLastSplay(SteamVR_Skeleton_FingerSplayIndexEnum fingerSplay)
		{
			return this.GetLastSplay((int)fingerSplay);
		}

		public string GetLocalizedName(params EVRInputStringBits[] localizedParts)
		{
			return this.sourceMap[SteamVR_Input_Sources.Any].GetLocalizedOriginPart(localizedParts);
		}

		public void RemoveAllListeners(SteamVR_Input_Sources input_Sources)
		{
			this.sourceMap[input_Sources].RemoveAllListeners();
		}

		public void AddOnDeviceConnectedChanged(SteamVR_Action_Skeleton.DeviceConnectedChangeHandler functionToCall)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged += functionToCall;
		}

		public void RemoveOnDeviceConnectedChanged(SteamVR_Action_Skeleton.DeviceConnectedChangeHandler functionToStopCalling)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onDeviceConnectedChanged -= functionToStopCalling;
		}

		public void AddOnTrackingChanged(SteamVR_Action_Skeleton.TrackingChangeHandler functionToCall)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged += functionToCall;
		}

		public void RemoveOnTrackingChanged(SteamVR_Action_Skeleton.TrackingChangeHandler functionToStopCalling)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onTrackingChanged -= functionToStopCalling;
		}

		public void AddOnValidPoseChanged(SteamVR_Action_Skeleton.ValidPoseChangeHandler functionToCall)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged += functionToCall;
		}

		public void RemoveOnValidPoseChanged(SteamVR_Action_Skeleton.ValidPoseChangeHandler functionToStopCalling)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onValidPoseChanged -= functionToStopCalling;
		}

		public void AddOnActiveChangeListener(SteamVR_Action_Skeleton.ActiveChangeHandler functionToCall)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange += functionToCall;
		}

		public void RemoveOnActiveChangeListener(SteamVR_Action_Skeleton.ActiveChangeHandler functionToStopCalling)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onActiveChange -= functionToStopCalling;
		}

		public void AddOnChangeListener(SteamVR_Action_Skeleton.ChangeHandler functionToCall)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onChange += functionToCall;
		}

		public void RemoveOnChangeListener(SteamVR_Action_Skeleton.ChangeHandler functionToStopCalling)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onChange -= functionToStopCalling;
		}

		public void AddOnUpdateListener(SteamVR_Action_Skeleton.UpdateHandler functionToCall)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onUpdate += functionToCall;
		}

		public void RemoveOnUpdateListener(SteamVR_Action_Skeleton.UpdateHandler functionToStopCalling)
		{
			this.sourceMap[SteamVR_Input_Sources.Any].onUpdate -= functionToStopCalling;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			base.InitAfterDeserialize();
		}

		public const int numBones = 31;

		public static Quaternion steamVRFixUpRotation = Quaternion.AngleAxis(180f, Vector3.up);

		public delegate void ActiveChangeHandler(SteamVR_Action_Skeleton fromAction, bool active);

		public delegate void ChangeHandler(SteamVR_Action_Skeleton fromAction);

		public delegate void UpdateHandler(SteamVR_Action_Skeleton fromAction);

		public delegate void TrackingChangeHandler(SteamVR_Action_Skeleton fromAction, ETrackingResult trackingState);

		public delegate void ValidPoseChangeHandler(SteamVR_Action_Skeleton fromAction, bool validPose);

		public delegate void DeviceConnectedChangeHandler(SteamVR_Action_Skeleton fromAction, bool deviceConnected);
	}
}
