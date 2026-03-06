using System;
using Oculus.Interaction.GrabAPI;
using UnityEngine;

namespace Oculus.Interaction.Input.UnityXR
{
	public abstract class FromOpenXRHandDataSource : DataSource<HandDataAsset>
	{
		protected virtual void Awake()
		{
			this.HmdData = (this._hmdData as IHmd);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		protected override void UpdateData()
		{
			for (int i = 0; i < 26; i++)
			{
				int num = (int)HandJointUtils.JointParentList[i];
				this._dataAsset.Joints[i] = ((num < 0) ? Quaternion.identity : (Quaternion.Inverse(this._dataAsset.JointPoses[num].rotation) * this._dataAsset.JointPoses[i].rotation));
			}
			this.UpdateHandScale(this._dataAsset.JointPoses[7].position, this._dataAsset.JointPoses[8].position);
			if (this._dataAsset.IsDataValidAndConnected && this._shouldMockHandTrackingAim)
			{
				this.PopulateMockHandTrackingAim(this._dataAsset.JointPoses[0]);
			}
		}

		private void UpdateHandScale(Vector3 indexProximal, Vector3 indexIntermediate)
		{
			float num = Vector3.Distance(indexProximal, indexIntermediate);
			this._dataAsset.HandScale = num / FromOpenXRHandDataSource.DefaultSkeletonIndexMagnitude;
			float d = 1f / this._dataAsset.HandScale;
			for (int i = 0; i < 26; i++)
			{
				Pose[] jointPoses = this._dataAsset.JointPoses;
				int num2 = i;
				jointPoses[num2].position = jointPoses[num2].position * d;
			}
		}

		private void PopulateMockHandTrackingAim(Pose xrPalmPose)
		{
			this._dataAsset.PointerPose = xrPalmPose.GetTransformedBy(new Pose(FromOpenXRHandDataSource.TrackedRemoteAimOffset, Quaternion.identity));
			this._dataAsset.PointerPoseOrigin = PoseOrigin.SyntheticPose;
			this._dataAsset.IsDominantHand = (this._dataAsset.Config.Handedness == Handedness.Right);
			Pose[] jointPoses = this._dataAsset.JointPoses;
			if (this._fingerGrabAPI == null)
			{
				this._fingerGrabAPI = new PinchGrabAPI(this.HmdData);
			}
			this._fingerGrabAPI.Update(jointPoses, this._dataAsset.Config.Handedness, this._dataAsset.Root, this._dataAsset.HandScale);
			this.PopulateMockHandTrackingAimFinger(HandFinger.Index);
			this.PopulateMockHandTrackingAimFinger(HandFinger.Middle);
			this.PopulateMockHandTrackingAimFinger(HandFinger.Ring);
			this.PopulateMockHandTrackingAimFinger(HandFinger.Pinky);
		}

		private void PopulateMockHandTrackingAimFinger(HandFinger finger)
		{
			this._dataAsset.FingerPinchStrength[(int)finger] = this._fingerGrabAPI.GetFingerGrabScore(finger);
			this._dataAsset.IsFingerPinching[(int)finger] = (this._dataAsset.FingerPinchStrength[(int)finger] > 0.8f);
		}

		protected override HandDataAsset DataAsset
		{
			get
			{
				return this._dataAsset;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FromOpenXRHandDataSource()
		{
			Vector3 position = HandSkeleton.DefaultLeftSkeleton[8].pose.position;
			FromOpenXRHandDataSource.DefaultSkeletonIndexMagnitude = position.magnitude;
			FromOpenXRHandDataSource.TrackedRemoteAimOffset = new Vector3(0f, 0f, -0.055f);
		}

		private static readonly float DefaultSkeletonIndexMagnitude;

		private const float PressThreshold = 0.8f;

		private static readonly Vector3 TrackedRemoteAimOffset;

		[SerializeField]
		[Interface(typeof(IHmd), new Type[]
		{

		})]
		private Object _hmdData;

		private IHmd HmdData;

		protected readonly HandDataAsset _dataAsset = new HandDataAsset();

		protected bool _shouldMockHandTrackingAim;

		private PinchGrabAPI _fingerGrabAPI;
	}
}
