using System;
using System.Linq;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

namespace Oculus.Interaction.UnityXR
{
	public class FromUnityXRControllerDataSource : DataSource<ControllerDataAsset>
	{
		public void SetTimeFrameCountProvider(Func<int> frameCountProvider)
		{
			if (frameCountProvider == null)
			{
				frameCountProvider = FromUnityXRControllerDataSource.DefaultFrameCountProvider;
			}
			this._frameCountProvider = frameCountProvider;
		}

		private void Awake()
		{
			this.TrackingToWorldTransformer = (this._trackingToWorldTransformer as ITrackingToWorldTransformer);
			this.UpdateConfig();
			InputActionMap inputActionMap = (this._handedness == Handedness.Left) ? this._leftHandControllerBindings : this._rightHandControllerBindings;
			this._dataAsset.IsDominantHand = (this._handedness > Handedness.Left);
			string[] names = Enum.GetNames(typeof(ControllerButtonUsage));
			for (int i = 0; i < names.Length; i++)
			{
				string text = names[i];
				if (!(text == ControllerButtonUsage.None.ToString()))
				{
					ControllerButtonUsage usage = Enum.Parse<ControllerButtonUsage>(text);
					inputActionMap[text].started += delegate(InputAction.CallbackContext _)
					{
						this._dataAsset.Input.SetButton(usage, true);
					};
					inputActionMap[text].canceled += delegate(InputAction.CallbackContext _)
					{
						this._dataAsset.Input.SetButton(usage, false);
					};
				}
			}
			names = Enum.GetNames(typeof(ControllerAxis1DUsage));
			for (int i = 0; i < names.Length; i++)
			{
				string usageName = names[i];
				if (!(usageName == ControllerAxis1DUsage.None.ToString()))
				{
					inputActionMap[usageName].performed += delegate(InputAction.CallbackContext context)
					{
						this._dataAsset.Input.SetAxis1D(Enum.Parse<ControllerAxis1DUsage>(usageName), context.ReadValue<float>());
					};
					inputActionMap[usageName].canceled += delegate(InputAction.CallbackContext context)
					{
						this._dataAsset.Input.SetAxis1D(Enum.Parse<ControllerAxis1DUsage>(usageName), 0f);
					};
				}
			}
			names = Enum.GetNames(typeof(ControllerAxis2DUsage));
			for (int i = 0; i < names.Length; i++)
			{
				string usageName = names[i];
				if (!(usageName == ControllerAxis2DUsage.None.ToString()))
				{
					inputActionMap[usageName].performed += delegate(InputAction.CallbackContext context)
					{
						this._dataAsset.Input.SetAxis2D(Enum.Parse<ControllerAxis2DUsage>(usageName), context.ReadValue<Vector2>());
					};
					inputActionMap[usageName].canceled += delegate(InputAction.CallbackContext context)
					{
						this._dataAsset.Input.SetAxis2D(Enum.Parse<ControllerAxis2DUsage>(usageName), Vector2.zero);
					};
				}
			}
			inputActionMap["RootPose"].performed += delegate(InputAction.CallbackContext context)
			{
				PoseState poseState = context.ReadValue<PoseState>();
				this._dataAsset.RootPose = new Pose(poseState.position, poseState.rotation);
				this._dataAsset.RootPose = FromUnityXRControllerDataSource.FlipZ(this._dataAsset.RootPose);
				ControllerDataAsset dataAsset = this._dataAsset;
				dataAsset.RootPose.rotation = dataAsset.RootPose.rotation * ((this._dataAsset.Config.Handedness == Handedness.Left) ? FromUnityXRControllerDataSource.OpenXRToOVRLeftRotTipInverted : FromUnityXRControllerDataSource.OpenXRToOVRRightRotTipInverted);
				this._dataAsset.RootPose = FromUnityXRControllerDataSource.FlipZ(this._dataAsset.RootPose);
				this._dataAsset.RootPoseOrigin = PoseOrigin.RawTrackedPose;
				this._dataAsset.IsTracked = (poseState.trackingState.HasFlag(InputTrackingState.Position) && poseState.trackingState.HasFlag(InputTrackingState.Rotation));
				this._dataAsset.IsDataValid = this._dataAsset.IsTracked;
				this._dataAsset.IsConnected = this._dataAsset.IsTracked;
				int num = this._frameCountProvider();
				if (this._lastRequiredUpdate != num)
				{
					this._lastRequiredUpdate = num;
					this.MarkInputDataRequiresUpdate();
				}
			};
			inputActionMap["RootPose"].canceled += delegate(InputAction.CallbackContext _)
			{
				this._dataAsset.RootPoseOrigin = PoseOrigin.None;
			};
			inputActionMap["PointerPose"].performed += delegate(InputAction.CallbackContext context)
			{
				PoseState poseState = context.ReadValue<PoseState>();
				this._dataAsset.PointerPose = new Pose(poseState.position, poseState.rotation);
				this._dataAsset.PointerPoseOrigin = PoseOrigin.RawTrackedPose;
			};
			inputActionMap["PointerPose"].canceled += delegate(InputAction.CallbackContext _)
			{
				this._dataAsset.PointerPoseOrigin = PoseOrigin.None;
			};
			inputActionMap.Enable();
		}

		private static Quaternion FlipZ(Quaternion q)
		{
			return new Quaternion
			{
				x = -q.x,
				y = -q.y,
				z = q.z,
				w = q.w
			};
		}

		public static Pose FlipZ(Pose p)
		{
			p.rotation = FromUnityXRControllerDataSource.FlipZ(p.rotation);
			p.position = new Vector3
			{
				x = p.position.x,
				y = p.position.y,
				z = -p.position.z
			};
			return p;
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

		protected override void UpdateData()
		{
		}

		private void UpdateConfig()
		{
			this._config.Handedness = this._handedness;
			this._config.TrackingToWorldTransformer = this.TrackingToWorldTransformer;
			this._dataAsset.Config = this._config;
		}

		protected override ControllerDataAsset DataAsset
		{
			get
			{
				return this._dataAsset;
			}
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

		[Header("Shared Configuration")]
		[SerializeField]
		private Handedness _handedness;

		[SerializeField]
		[Interface(typeof(ITrackingToWorldTransformer), new Type[]
		{

		})]
		private Object _trackingToWorldTransformer;

		private ITrackingToWorldTransformer TrackingToWorldTransformer;

		private static string ControllerActionMap = "{\n            \"maps\": [\n                {\n                    \"name\": \"XRController\",\n                    \"actions\": [\n                        {\n                            \"name\": \"PrimaryButton\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/primaryButton\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"PrimaryTouch\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/primaryTouched\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"SecondaryButton\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/secondaryButton\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"SecondaryTouch\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/secondaryTouched\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"GripButton\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/gripPressed\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"TriggerButton\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/triggerPressed\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"TriggerTouch\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/triggerTouched\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"MenuButton\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/menu\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Primary2DAxisClick\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/thumbstickClicked\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Primary2DAxisTouch\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/thumbstickTouched\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Thumbrest\",\n                            \"expectedControlLayout\": \"Integer\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/thumbrestTouched\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Trigger\",\n                            \"expectedControlLayout\": \"Axis1D\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/trigger\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Grip\",\n                            \"expectedControlLayout\": \"Axis1D\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/grip\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Primary2DAxis\",\n                            \"expectedControlLayout\": \"Axis2D\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/thumbstick\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"Secondary2DAxis\",\n                            \"expectedControlLayout\": \"Axis2D\",\n                            \"bindings\": [\n                                {\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"RootPose\",\n                            \"expectedControlLayout\": \"Pose\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/devicePose\"\n                                }\n                            ]\n                        },\n                        {\n                            \"name\": \"PointerPose\",\n                            \"expectedControlLayout\": \"Pose\",\n                            \"bindings\": [\n                                {\n                                    \"path\":\"<XRController>{LeftHand}/pointer\"\n                                }\n                            ]\n                        }\n                    ]\n                }\n            ]}";

		[SerializeField]
		private InputActionMap _leftHandControllerBindings = InputActionMap.FromJson(FromUnityXRControllerDataSource.ControllerActionMap).FirstOrDefault<InputActionMap>();

		[SerializeField]
		private InputActionMap _rightHandControllerBindings = InputActionMap.FromJson(FromUnityXRControllerDataSource.ControllerActionMap.Replace("{LeftHand}", "{RightHand}")).FirstOrDefault<InputActionMap>();

		private readonly ControllerDataAsset _dataAsset = new ControllerDataAsset();

		private readonly ControllerDataSourceConfig _config = new ControllerDataSourceConfig();

		private static readonly Quaternion OpenXRToOVRLeftRotTipInverted = Quaternion.Inverse(Quaternion.AngleAxis(90f, Vector3.forward));

		private static readonly Quaternion OpenXRToOVRRightRotTipInverted = Quaternion.Inverse(Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(-90f, Vector3.forward));

		private static readonly Func<int> DefaultFrameCountProvider = () => Time.frameCount;

		private Func<int> _frameCountProvider = FromUnityXRControllerDataSource.DefaultFrameCountProvider;

		private int _lastRequiredUpdate;
	}
}
