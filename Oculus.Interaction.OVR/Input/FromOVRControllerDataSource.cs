using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class FromOVRControllerDataSource : DataSource<ControllerDataAsset>
	{
		public IOVRCameraRigRef CameraRigRef { get; private set; }

		public bool ProcessLateUpdates
		{
			get
			{
				return this._processLateUpdates;
			}
			set
			{
				this._processLateUpdates = value;
			}
		}

		protected void Awake()
		{
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
			this.CameraRigRef = (this._cameraRigRef as IOVRCameraRigRef);
			this.UpdateConfig();
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			if (this._handedness == Handedness.Left)
			{
				this._ovrController = OVRInput.Controller.LTouch;
			}
			else
			{
				this._ovrController = OVRInput.Controller.RTouch;
			}
			this._pointerPoseSelector = new OVRPointerPoseSelector(this._handedness);
			this.UpdateConfig();
			this.EndStart(ref this._started);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this._started)
			{
				this.CameraRigRef.WhenInputDataDirtied += this.HandleInputDataDirtied;
			}
		}

		protected override void OnDisable()
		{
			if (this._started)
			{
				this.CameraRigRef.WhenInputDataDirtied -= this.HandleInputDataDirtied;
			}
			base.OnDisable();
			this.MarkInputDataRequiresUpdate();
		}

		private void HandleInputDataDirtied(bool isLateUpdate)
		{
			if (isLateUpdate && !this._processLateUpdates)
			{
				return;
			}
			this.MarkInputDataRequiresUpdate();
		}

		private ControllerDataSourceConfig Config
		{
			get
			{
				if (this._config != null)
				{
					return this._config;
				}
				this._config = new ControllerDataSourceConfig
				{
					Handedness = this._handedness
				};
				return this._config;
			}
		}

		private void UpdateConfig()
		{
			this.Config.Handedness = this._handedness;
			this.Config.TrackingToWorldTransformer = this.TrackingToWorldTransformer;
		}

		protected override void UpdateData()
		{
			this._controllerDataAsset.Config = this.Config;
			this._controllerDataAsset.IsDataValid = true;
			this._controllerDataAsset.IsConnected = ((OVRInput.GetConnectedControllers() & this._ovrController) > OVRInput.Controller.None);
			if (!this._controllerDataAsset.IsConnected || !base.isActiveAndEnabled)
			{
				this._controllerDataAsset.IsConnected = false;
				this._controllerDataAsset.IsTracked = false;
				this._controllerDataAsset.Input = default(ControllerInput);
				this._controllerDataAsset.RootPoseOrigin = PoseOrigin.None;
				return;
			}
			this._controllerDataAsset.IsTracked = true;
			OVRInput.Handedness dominantHand = OVRInput.GetDominantHand();
			this._controllerDataAsset.IsDominantHand = ((dominantHand == OVRInput.Handedness.LeftHanded && this._handedness == Handedness.Left) || (dominantHand == OVRInput.Handedness.RightHanded && this._handedness == Handedness.Right));
			this._controllerDataAsset.Input.Clear();
			OVRInput.Controller ovrController = this._ovrController;
			IUsage[] controllerUsageMappings = FromOVRControllerDataSource.ControllerUsageMappings;
			for (int i = 0; i < controllerUsageMappings.Length; i++)
			{
				controllerUsageMappings[i].Apply(this._controllerDataAsset, ovrController);
			}
			this._controllerDataAsset.RootPose = new Pose(OVRInput.GetLocalControllerPosition(this._ovrController), OVRInput.GetLocalControllerRotation(this._ovrController));
			this._controllerDataAsset.RootPoseOrigin = PoseOrigin.RawTrackedPose;
			Matrix4x4 matrix4x = Matrix4x4.TRS(this._controllerDataAsset.RootPose.position, this._controllerDataAsset.RootPose.rotation, Vector3.one);
			this._controllerDataAsset.PointerPose = new Pose(matrix4x.MultiplyPoint3x4(this._pointerPoseSelector.LocalPointerPose.position), this._controllerDataAsset.RootPose.rotation * this._pointerPoseSelector.LocalPointerPose.rotation);
			this._controllerDataAsset.PointerPoseOrigin = PoseOrigin.RawTrackedPose;
		}

		protected override ControllerDataAsset DataAsset
		{
			get
			{
				return this._controllerDataAsset;
			}
		}

		public void InjectAllFromOVRControllerDataSource(DataSource<ControllerDataAsset>.UpdateModeFlags updateMode, IDataSource updateAfter, Handedness handedness, ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			base.InjectAllDataSource(updateMode, updateAfter);
			this.InjectHandedness(handedness);
			this.InjectTrackingToWorldTransformer(trackingToWorldTransformer);
		}

		public void InjectHandedness(Handedness handedness)
		{
			this._handedness = handedness;
		}

		public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
		{
			this._trackingToWorldTransformer = (trackingToWorldTransformer as Object);
			this.TrackingToWorldTransformer = trackingToWorldTransformer;
		}

		[Header("OVR Data Source")]
		[SerializeField]
		[Interface(typeof(IOVRCameraRigRef), new Type[]
		{

		})]
		private Object _cameraRigRef;

		[SerializeField]
		private bool _processLateUpdates;

		[Header("Shared Configuration")]
		[SerializeField]
		private Handedness _handedness;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;

		private readonly ControllerDataAsset _controllerDataAsset = new ControllerDataAsset();

		private OVRInput.Controller _ovrController;

		private ControllerDataSourceConfig _config;

		private OVRPointerPoseSelector _pointerPoseSelector;

		private static readonly IUsage[] ControllerUsageMappings = new IUsage[]
		{
			new UsageButtonMapping(ControllerButtonUsage.PrimaryButton, OVRInput.Button.One),
			new UsageTouchMapping(ControllerButtonUsage.PrimaryTouch, OVRInput.Touch.One),
			new UsageButtonMapping(ControllerButtonUsage.SecondaryButton, OVRInput.Button.Two),
			new UsageTouchMapping(ControllerButtonUsage.SecondaryTouch, OVRInput.Touch.Two),
			new UsageButtonMapping(ControllerButtonUsage.GripButton, OVRInput.Button.PrimaryHandTrigger),
			new UsageButtonMapping(ControllerButtonUsage.TriggerButton, OVRInput.Button.PrimaryIndexTrigger),
			new UsageButtonMapping(ControllerButtonUsage.MenuButton, OVRInput.Button.Start),
			new UsageButtonMapping(ControllerButtonUsage.Primary2DAxisClick, OVRInput.Button.PrimaryThumbstick),
			new UsageTouchMapping(ControllerButtonUsage.Primary2DAxisTouch, OVRInput.Touch.PrimaryThumbstick),
			new UsageTouchMapping(ControllerButtonUsage.Thumbrest, OVRInput.Touch.PrimaryThumbRest),
			new UsageAxis1DMapping(ControllerAxis1DUsage.Trigger, OVRInput.Axis1D.PrimaryIndexTrigger),
			new UsageAxis1DMapping(ControllerAxis1DUsage.Grip, OVRInput.Axis1D.PrimaryHandTrigger),
			new UsageAxis2DMapping(ControllerAxis2DUsage.Primary2DAxis, OVRInput.Axis2D.PrimaryThumbstick)
		};
	}
}
