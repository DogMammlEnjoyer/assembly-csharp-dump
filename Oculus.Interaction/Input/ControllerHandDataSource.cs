using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.Input
{
	public class ControllerHandDataSource : DataSource<HandDataAsset>
	{
		public Transform Root
		{
			get
			{
				return this._openXRRoot;
			}
			set
			{
				this._openXRRoot = value;
			}
		}

		public bool RootIsLocal
		{
			get
			{
				return this._rootIsLocal;
			}
			set
			{
				this._rootIsLocal = value;
			}
		}

		public Transform[] Joints
		{
			get
			{
				return this._openXRJointTransforms;
			}
		}

		protected override HandDataAsset DataAsset
		{
			get
			{
				return this._handDataAsset;
			}
		}

		private HandDataSourceConfig Config
		{
			get
			{
				if (this._config == null)
				{
					this._config = new HandDataSourceConfig();
				}
				return this._config;
			}
		}

		protected virtual void Awake()
		{
			if (this._root != null)
			{
				this._root.gameObject.SetActive(false);
			}
			if (this._openXRRoot != null)
			{
				this._openXRRoot.gameObject.SetActive(true);
			}
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.UpdateConfig();
			this.EndStart(ref this._started);
		}

		private void UpdateConfig()
		{
			ControllerDataSourceConfig config = this._controllerSource.GetData().Config;
			this.Config.Handedness = config.Handedness;
			this.Config.TrackingToWorldTransformer = config.TrackingToWorldTransformer;
			this.Config.HandSkeleton = HandSkeleton.FromJoints(this.Joints);
		}

		protected override void UpdateData()
		{
			ControllerDataAsset data = this._controllerSource.GetData();
			this._handDataAsset.Config = this.Config;
			this._handDataAsset.IsDataValid = data.IsDataValid;
			this._handDataAsset.IsConnected = data.IsConnected;
			if (!this._handDataAsset.IsConnected || !base.isActiveAndEnabled)
			{
				this._handDataAsset.IsTracked = false;
				this._handDataAsset.RootPoseOrigin = PoseOrigin.None;
				this._handDataAsset.PointerPoseOrigin = PoseOrigin.None;
				this._handDataAsset.IsHighConfidence = false;
				for (int i = 0; i < 5; i++)
				{
					this._handDataAsset.IsFingerPinching[i] = false;
					this._handDataAsset.IsFingerHighConfidence[i] = false;
				}
				return;
			}
			this._handDataAsset.IsTracked = data.IsTracked;
			this._handDataAsset.IsHighConfidence = true;
			this._handDataAsset.IsDominantHand = data.IsDominantHand;
			float trigger = data.Input.Trigger;
			float grip = data.Input.Grip;
			bool triggerButton = data.Input.TriggerButton;
			bool gripButton = data.Input.GripButton;
			this._handDataAsset.IsFingerHighConfidence[0] = true;
			this._handDataAsset.IsFingerPinching[0] = (triggerButton || gripButton);
			this._handDataAsset.FingerPinchStrength[0] = Mathf.Max(trigger, grip);
			this._handDataAsset.IsFingerHighConfidence[1] = true;
			this._handDataAsset.IsFingerPinching[1] = triggerButton;
			this._handDataAsset.FingerPinchStrength[1] = trigger;
			this._handDataAsset.IsFingerHighConfidence[2] = true;
			this._handDataAsset.IsFingerPinching[2] = gripButton;
			this._handDataAsset.FingerPinchStrength[2] = grip;
			this._handDataAsset.IsFingerHighConfidence[3] = true;
			this._handDataAsset.IsFingerPinching[3] = false;
			this._handDataAsset.FingerPinchStrength[3] = 0f;
			this._handDataAsset.IsFingerHighConfidence[4] = true;
			this._handDataAsset.IsFingerPinching[4] = false;
			this._handDataAsset.FingerPinchStrength[4] = 0f;
			this._handDataAsset.PointerPoseOrigin = PoseOrigin.FilteredTrackedPose;
			this._handDataAsset.PointerPose = data.PointerPose;
			for (int j = 0; j < this.Joints.Length; j++)
			{
				this._handDataAsset.Joints[j] = this.Joints[j].localRotation;
				this._handDataAsset.JointPoses[j] = this.Root.Delta(this.Joints[j]);
			}
			if (this._rootIsLocal)
			{
				Pose pose = this.Root.GetPose(Space.Self);
				Pose rootPose = data.RootPose;
				PoseUtils.Multiply(rootPose, pose, ref this._handDataAsset.Root);
				this._handDataAsset.HandScale = this.Root.localScale.x;
			}
			else
			{
				this._handDataAsset.Root = this.Root.GetPose(Space.World);
				this._handDataAsset.HandScale = this.Root.lossyScale.x;
			}
			this._handDataAsset.RootPoseOrigin = PoseOrigin.FilteredTrackedPose;
		}

		public void InjectAllControllerHandDataSource(DataSource<HandDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, DataSource<ControllerDataAsset> controllerSource, Transform[] jointTransforms)
		{
			base.InjectAllDataSource(updateMode, updateAfter);
			this.InjectControllerSource(controllerSource);
			this.InjectJointTransforms(jointTransforms);
		}

		public void InjectControllerSource(DataSource<ControllerDataAsset> controllerSource)
		{
			this._controllerSource = controllerSource;
		}

		[Obsolete("Use InjectJointTransforms instead")]
		public void InjectBones(Transform[] joints)
		{
			this.InjectJointTransforms(joints);
		}

		public void InjectJointTransforms(Transform[] jointTransforms)
		{
			this._openXRJointTransforms = jointTransforms;
		}

		[SerializeField]
		private DataSource<ControllerDataAsset> _controllerSource;

		[SerializeField]
		private Transform _root;

		[SerializeField]
		private Transform _openXRRoot;

		[SerializeField]
		private bool _rootIsLocal = true;

		[SerializeField]
		[FormerlySerializedAs("_bones")]
		[FormerlySerializedAs("_joints")]
		private Transform[] _jointTransforms;

		[SerializeField]
		private Transform[] _openXRJointTransforms;

		private HandDataSourceConfig _config;

		private readonly HandDataAsset _handDataAsset = new HandDataAsset();
	}
}
