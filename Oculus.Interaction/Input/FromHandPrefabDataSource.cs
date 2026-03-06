using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public class FromHandPrefabDataSource : DataSource<HandDataAsset>
	{
		protected override HandDataAsset DataAsset
		{
			get
			{
				return this._handDataAsset;
			}
		}

		public Handedness Handedness
		{
			get
			{
				return this._handedness;
			}
		}

		public List<Transform> JointTransforms
		{
			get
			{
				return this._jointTransformsOpenXR;
			}
		}

		protected virtual void Awake()
		{
			this.HandSkeletonProvider = (this._handSkeletonProvider as IHandSkeletonProvider);
			if (this._trackingToWorldTransformer != null)
			{
				this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
			}
			this._handDataAsset.Config.Handedness = this._handedness;
		}

		protected override void Start()
		{
			base.Start();
			HandDataSourceConfig config = this._handDataAsset.Config;
			config.TrackingToWorldTransformer = this.TrackingToWorldTransformer;
			config.HandSkeleton = this.HandSkeletonProvider[this._handedness];
			if (!this._hidePrefabOnStart)
			{
				return;
			}
			Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}

		protected override void UpdateData()
		{
			this._handDataAsset.IsDataValid = true;
			this._handDataAsset.IsConnected = true;
			this._handDataAsset.IsTracked = true;
			this._handDataAsset.IsHighConfidence = true;
			this._handDataAsset.RootPoseOrigin = PoseOrigin.SyntheticPose;
			this._handDataAsset.Root = base.transform.GetPose(Space.World);
			this._handDataAsset.HandScale = 1f;
			for (int i = 0; i < 26; i++)
			{
				Transform transform = this.JointTransforms[i];
				if (transform == null)
				{
					this._handDataAsset.Joints[i] = Quaternion.identity;
					this._handDataAsset.JointPoses[i] = new Pose(Vector3.zero, Quaternion.identity);
				}
				else
				{
					this._handDataAsset.Joints[i] = transform.transform.localRotation;
					Pose pose = transform.transform.GetPose(Space.World);
					pose = base.transform.Delta(pose);
					this._handDataAsset.JointPoses[i] = pose;
				}
			}
		}

		public Transform GetTransformFor(HandJointId jointId)
		{
			return this.JointTransforms[(int)jointId];
		}

		private readonly HandDataAsset _handDataAsset = new HandDataAsset();

		[SerializeField]
		private Handedness _handedness;

		[SerializeField]
		private bool _hidePrefabOnStart = true;

		[HideInInspector]
		[SerializeField]
		private List<Transform> _jointTransforms = new List<Transform>();

		[HideInInspector]
		[SerializeField]
		private List<Transform> _jointTransformsOpenXR = new List<Transform>();

		[SerializeField]
		[Interface(typeof(IHandSkeletonProvider), new Type[]
		{

		})]
		private Object _handSkeletonProvider;

		private IHandSkeletonProvider HandSkeletonProvider;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		[Optional]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;
	}
}
