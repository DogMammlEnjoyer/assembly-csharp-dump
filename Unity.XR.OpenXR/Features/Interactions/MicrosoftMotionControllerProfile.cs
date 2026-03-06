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
	public class MicrosoftMotionControllerProfile : OpenXRInteractionFeature
	{
		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(MicrosoftMotionControllerProfile.WMRSpatialController);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Windows MR Controller OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("WMRSpatialController");
		}

		protected override string GetDeviceLayoutName()
		{
			return "WMRSpatialController";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "microsoftmotioncontroller",
				localizedName = "Windows MR Controller OpenXR",
				desiredInteractionProfile = "/interaction_profiles/microsoft/motion_controller",
				manufacturer = "Microsoft",
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
						name = "joystick",
						localizedName = "Joystick",
						type = OpenXRInteractionFeature.ActionType.Axis2D,
						usages = new List<string>
						{
							"Primary2DAxis"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick",
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "touchpad",
						localizedName = "Touchpad",
						type = OpenXRInteractionFeature.ActionType.Axis2D,
						usages = new List<string>
						{
							"Secondary2DAxis"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad",
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
							}
						}
					},
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionPath = "/input/trigger/value",
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "joystickClicked",
						localizedName = "JoystickClicked",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"Primary2DAxisClick"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick/click",
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "touchpadClicked",
						localizedName = "Touchpad Clicked",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"Secondary2DAxisClick"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/click",
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "touchpadTouched",
						localizedName = "Touchpad Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"Secondary2DAxisTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/touch",
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
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
								interactionProfileName = "/interaction_profiles/microsoft/motion_controller"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.microsoftmotioncontroller";

		public const string profile = "/interaction_profiles/microsoft/motion_controller";

		public const string menu = "/input/menu/click";

		public const string squeeze = "/input/squeeze/click";

		public const string trigger = "/input/trigger/value";

		public const string thumbstick = "/input/thumbstick";

		public const string thumbstickClick = "/input/thumbstick/click";

		public const string trackpad = "/input/trackpad";

		public const string trackpadClick = "/input/trackpad/click";

		public const string trackpadTouch = "/input/trackpad/touch";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string haptic = "/output/haptic";

		private const string kDeviceLocalizedName = "Windows MR Controller OpenXR";

		[Preserve]
		[InputControlLayout(displayName = "Windows MR Controller (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class WMRSpatialController : XRControllerWithRumble
		{
			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Primary2DAxis",
				"thumbstickaxes",
				"thumbstick"
			}, usage = "Primary2DAxis")]
			public Vector2Control joystick { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Secondary2DAxis",
				"touchpadaxes",
				"trackpad"
			}, usage = "Secondary2DAxis")]
			public Vector2Control touchpad { get; private set; }

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
			[InputControl(aliases = new string[]
			{
				"triggeraxis"
			}, usage = "Trigger")]
			public AxisControl trigger { get; private set; }

			[Preserve]
			[InputControl(alias = "triggerbutton", usage = "TriggerButton")]
			public ButtonControl triggerPressed { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"joystickClicked",
				"thumbstickpressed"
			}, usage = "Primary2DAxisClick")]
			public ButtonControl joystickClicked { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"joystickorpadpressed",
				"touchpadpressed",
				"trackpadClicked"
			}, usage = "Secondary2DAxisClick")]
			public ButtonControl touchpadClicked { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"joystickorpadtouched",
				"touchpadtouched",
				"trackpadTouched"
			}, usage = "Secondary2DAxisTouch")]
			public ButtonControl touchpadTouched { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, aliases = new string[]
			{
				"device",
				"gripPose"
			}, usage = "Device")]
			public UnityEngine.InputSystem.XR.PoseControl devicePose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, aliases = new string[]
			{
				"aimPose"
			}, usage = "Pointer")]
			public UnityEngine.InputSystem.XR.PoseControl pointer { get; private set; }

			[Preserve]
			[InputControl(offset = 32U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 36U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 40U, aliases = new string[]
			{
				"gripPosition"
			})]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 52U, aliases = new string[]
			{
				"gripOrientation"
			})]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 100U)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 112U, aliases = new string[]
			{
				"pointerOrientation"
			})]
			public QuaternionControl pointerRotation { get; private set; }

			[Preserve]
			[InputControl(usage = "Haptic")]
			public HapticControl haptic { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.joystick = base.GetChildControl<StickControl>("joystick");
				this.trigger = base.GetChildControl<AxisControl>("trigger");
				this.touchpad = base.GetChildControl<StickControl>("touchpad");
				this.grip = base.GetChildControl<AxisControl>("grip");
				this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
				this.menu = base.GetChildControl<ButtonControl>("menu");
				this.joystickClicked = base.GetChildControl<ButtonControl>("joystickClicked");
				this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
				this.touchpadClicked = base.GetChildControl<ButtonControl>("touchpadClicked");
				this.touchpadTouched = base.GetChildControl<ButtonControl>("touchPadTouched");
				this.devicePose = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("devicePose");
				this.pointer = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("pointer");
				this.isTracked = base.GetChildControl<ButtonControl>("isTracked");
				this.trackingState = base.GetChildControl<IntegerControl>("trackingState");
				this.devicePosition = base.GetChildControl<Vector3Control>("devicePosition");
				this.deviceRotation = base.GetChildControl<QuaternionControl>("deviceRotation");
				this.pointerPosition = base.GetChildControl<Vector3Control>("pointerPosition");
				this.pointerRotation = base.GetChildControl<QuaternionControl>("pointerRotation");
				this.haptic = base.GetChildControl<HapticControl>("haptic");
			}
		}
	}
}
