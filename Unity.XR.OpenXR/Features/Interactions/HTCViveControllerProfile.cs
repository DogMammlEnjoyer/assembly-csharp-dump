using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
	public class HTCViveControllerProfile : OpenXRInteractionFeature
	{
		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(HTCViveControllerProfile.ViveController);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("HTC Vive Controller OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("ViveController");
		}

		protected override string GetDeviceLayoutName()
		{
			return "ViveController";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "htcvivecontroller",
				localizedName = "HTC Vive Controller OpenXR",
				desiredInteractionProfile = "/interaction_profiles/htc/vive_controller",
				manufacturer = "HTC",
				serialNumber = "",
				deviceInfos = new List<OpenXRInteractionFeature.DeviceConfig>
				{
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left),
						userPath = "/user/hand/left"
					},
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right),
						userPath = "/user/hand/right"
					}
				},
				actions = new List<OpenXRInteractionFeature.ActionConfig>
				{
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "grip",
						localizedName = "Grip",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"Grip"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/squeeze/click",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "gripPressed",
						localizedName = "Grip Pressed",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"GripButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/squeeze/click",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "menu",
						localizedName = "Menu",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"MenuButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/menu/click",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "select",
						localizedName = "Select",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"SystemButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/system/click",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trigger",
						localizedName = "Trigger",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"Trigger"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trigger/value",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "triggerPressed",
						localizedName = "Trigger Pressed",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"TriggerButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trigger/click",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpad",
						localizedName = "Trackpad",
						type = OpenXRInteractionFeature.ActionType.Axis2D,
						usages = new List<string>
						{
							"Primary2DAxis"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadTouched",
						localizedName = "Trackpad Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"Primary2DAxisTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/touch",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadClicked",
						localizedName = "Trackpad Clicked",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"Primary2DAxisClick"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/click",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "devicePose",
						localizedName = "Device Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						usages = new List<string>
						{
							"Device"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/grip/pose",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "pointer",
						localizedName = "Pointer Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						usages = new List<string>
						{
							"Pointer"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/aim/pose",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "haptic",
						localizedName = "Haptic Output",
						type = OpenXRInteractionFeature.ActionType.Vibrate,
						usages = new List<string>
						{
							"Haptic"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/output/haptic",
								interactionProfileName = "/interaction_profiles/htc/vive_controller"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.htcvive";

		public const string profile = "/interaction_profiles/htc/vive_controller";

		public const string system = "/input/system/click";

		public const string squeeze = "/input/squeeze/click";

		public const string menu = "/input/menu/click";

		public const string trigger = "/input/trigger/value";

		public const string triggerClick = "/input/trigger/click";

		public const string trackpad = "/input/trackpad";

		public const string trackpadClick = "/input/trackpad/click";

		public const string trackpadTouch = "/input/trackpad/touch";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string haptic = "/output/haptic";

		private const string kDeviceLocalizedName = "HTC Vive Controller OpenXR";

		[Preserve]
		[InputControlLayout(displayName = "HTC Vive Controller (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class ViveController : XRControllerWithRumble
		{
			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Secondary",
				"selectbutton"
			}, usage = "SystemButton")]
			public ButtonControl select { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"GripAxis",
				"squeeze"
			}, usage = "Grip")]
			public AxisControl grip { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"GripButton",
				"squeezeClicked"
			}, usage = "GripButton")]
			public ButtonControl gripPressed { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Primary",
				"menubutton"
			}, usage = "MenuButton")]
			public ButtonControl menu { get; private set; }

			[Preserve]
			[InputControl(alias = "triggeraxis", usage = "Trigger")]
			public AxisControl trigger { get; private set; }

			[Preserve]
			[InputControl(alias = "triggerbutton", usage = "TriggerButton")]
			public ButtonControl triggerPressed { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Primary2DAxis",
				"touchpadaxes",
				"touchpad"
			}, usage = "Primary2DAxis")]
			public Vector2Control trackpad { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"joystickorpadpressed",
				"touchpadpressed"
			}, usage = "Primary2DAxisClick")]
			public ButtonControl trackpadClicked { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"joystickorpadtouched",
				"touchpadtouched"
			}, usage = "Primary2DAxisTouch")]
			public ButtonControl trackpadTouched { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, aliases = new string[]
			{
				"device",
				"gripPose"
			}, usage = "Device")]
			public UnityEngine.InputSystem.XR.PoseControl devicePose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, alias = "aimPose", usage = "Pointer")]
			public UnityEngine.InputSystem.XR.PoseControl pointer { get; private set; }

			[Preserve]
			[InputControl(offset = 26U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 28U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 32U, alias = "gripPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 44U, alias = "gripOrientation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 92U)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 104U, alias = "pointerOrientation")]
			public QuaternionControl pointerRotation { get; private set; }

			[Preserve]
			[InputControl(usage = "Haptic")]
			public HapticControl haptic { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.select = base.GetChildControl<ButtonControl>("select");
				this.grip = base.GetChildControl<AxisControl>("grip");
				this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
				this.menu = base.GetChildControl<ButtonControl>("menu");
				this.trigger = base.GetChildControl<AxisControl>("trigger");
				this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
				this.trackpad = base.GetChildControl<StickControl>("trackpad");
				this.trackpadClicked = base.GetChildControl<ButtonControl>("trackpadClicked");
				this.trackpadTouched = base.GetChildControl<ButtonControl>("trackpadTouched");
				this.pointer = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("pointer");
				this.pointerPosition = base.GetChildControl<Vector3Control>("pointerPosition");
				this.pointerRotation = base.GetChildControl<QuaternionControl>("pointerRotation");
				this.devicePose = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("devicePose");
				this.isTracked = base.GetChildControl<ButtonControl>("isTracked");
				this.trackingState = base.GetChildControl<IntegerControl>("trackingState");
				this.devicePosition = base.GetChildControl<Vector3Control>("devicePosition");
				this.deviceRotation = base.GetChildControl<QuaternionControl>("deviceRotation");
				this.haptic = base.GetChildControl<HapticControl>("haptic");
			}
		}
	}
}
