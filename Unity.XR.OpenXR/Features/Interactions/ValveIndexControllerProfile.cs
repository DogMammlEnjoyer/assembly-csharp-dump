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
	public class ValveIndexControllerProfile : OpenXRInteractionFeature
	{
		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(ValveIndexControllerProfile.ValveIndexController);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Index Controller OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("ValveIndexController");
		}

		protected override string GetDeviceLayoutName()
		{
			return "ValveIndexController";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "valveindexcontroller",
				localizedName = "Index Controller OpenXR",
				desiredInteractionProfile = "/interaction_profiles/valve/index_controller",
				manufacturer = "Valve",
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
						name = "system",
						localizedName = "System",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"MenuButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/system/click",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "systemTouched",
						localizedName = "System Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"MenuTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/system/touch",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "primaryButton",
						localizedName = "Primary Button",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PrimaryButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/a/click",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "primaryTouched",
						localizedName = "Primary Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PrimaryTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/a/touch",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "secondaryButton",
						localizedName = "Secondary Button",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"SecondaryButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/b/click",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "secondaryTouched",
						localizedName = "Secondary Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"SecondaryTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/b/touch",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
								interactionPath = "/input/squeeze/value",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
								interactionPath = "/input/squeeze/value",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "gripForce",
						localizedName = "Grip Force",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"GripForce"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/squeeze/force",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "triggerPressed",
						localizedName = "Triggger Pressed",
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "triggerTouched",
						localizedName = "Trigger Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"TriggerTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trigger/touch",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "thumbstick",
						localizedName = "Thumbstick",
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "thumbstickClicked",
						localizedName = "Thumbstick Clicked",
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "thumbstickTouched",
						localizedName = "Thumbstick Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"Primary2DAxisTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick/touch",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
							"Secondary2DAxis"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadForce",
						localizedName = "Trackpad Force",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"Secondary2DAxisForce"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/force",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
							"Secondary2DAxisTouch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/touch",
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
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
								interactionProfileName = "/interaction_profiles/valve/index_controller"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.valveindex";

		public const string profile = "/interaction_profiles/valve/index_controller";

		public const string system = "/input/system/click";

		public const string systemTouch = "/input/system/touch";

		public const string buttonA = "/input/a/click";

		public const string buttonATouch = "/input/a/touch";

		public const string buttonB = "/input/b/click";

		public const string buttonBTouch = "/input/b/touch";

		public const string squeeze = "/input/squeeze/value";

		public const string squeezeForce = "/input/squeeze/force";

		public const string triggerClick = "/input/trigger/click";

		public const string trigger = "/input/trigger/value";

		public const string triggerTouch = "/input/trigger/touch";

		public const string thumbstick = "/input/thumbstick";

		public const string thumbstickClick = "/input/thumbstick/click";

		public const string thumbstickTouch = "/input/thumbstick/touch";

		public const string trackpad = "/input/trackpad";

		public const string trackpadForce = "/input/trackpad/force";

		public const string trackpadTouch = "/input/trackpad/touch";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string haptic = "/output/haptic";

		private const string kDeviceLocalizedName = "Index Controller OpenXR";

		[Preserve]
		[InputControlLayout(displayName = "Index Controller (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class ValveIndexController : XRControllerWithRumble
		{
			[Preserve]
			[InputControl(alias = "systemButton", usage = "MenuButton")]
			public ButtonControl system { get; private set; }

			[Preserve]
			[InputControl(usage = "MenuTouch")]
			public ButtonControl systemTouched { get; private set; }

			[Preserve]
			[InputControl(usage = "PrimaryButton")]
			public ButtonControl primaryButton { get; private set; }

			[Preserve]
			[InputControl(usage = "PrimaryTouch")]
			public ButtonControl primaryTouched { get; private set; }

			[Preserve]
			[InputControl(usage = "SecondaryButton")]
			public ButtonControl secondaryButton { get; private set; }

			[Preserve]
			[InputControl(usage = "SecondaryTouch")]
			public ButtonControl secondaryTouched { get; private set; }

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
			[InputControl(alias = "squeezeForce", usage = "GripForce")]
			public AxisControl gripForce { get; private set; }

			[Preserve]
			[InputControl(usage = "Trigger")]
			public AxisControl trigger { get; private set; }

			[Preserve]
			[InputControl(usage = "TriggerButton")]
			public ButtonControl triggerPressed { get; private set; }

			[Preserve]
			[InputControl(usage = "TriggerTouch")]
			public ButtonControl triggerTouched { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"joystick",
				"Primary2DAxis"
			}, usage = "Primary2DAxis")]
			public Vector2Control thumbstick { get; private set; }

			[Preserve]
			[InputControl(alias = "joystickClicked", usage = "Primary2DAxisClick")]
			public ButtonControl thumbstickClicked { get; private set; }

			[Preserve]
			[InputControl(alias = "joystickTouched", usage = "Primary2DAxisTouch")]
			public ButtonControl thumbstickTouched { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"touchpad",
				"Secondary2DAxis"
			}, usage = "Secondary2DAxis")]
			public Vector2Control trackpad { get; private set; }

			[Preserve]
			[InputControl(alias = "touchpadTouched", usage = "Secondary2DAxisTouch")]
			public ButtonControl trackpadTouched { get; private set; }

			[Preserve]
			[InputControl(alias = "touchpadForce", usage = "Secondary2DAxisForce")]
			public AxisControl trackpadForce { get; private set; }

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
			[InputControl(offset = 53U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 56U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 60U, alias = "gripPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 72U, alias = "gripOrientation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 120U)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 132U, alias = "pointerOrientation")]
			public QuaternionControl pointerRotation { get; private set; }

			[Preserve]
			[InputControl(usage = "Haptic")]
			public HapticControl haptic { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.system = base.GetChildControl<ButtonControl>("system");
				this.systemTouched = base.GetChildControl<ButtonControl>("systemTouched");
				this.primaryButton = base.GetChildControl<ButtonControl>("primaryButton");
				this.primaryTouched = base.GetChildControl<ButtonControl>("primaryTouched");
				this.secondaryButton = base.GetChildControl<ButtonControl>("secondaryButton");
				this.secondaryTouched = base.GetChildControl<ButtonControl>("secondaryTouched");
				this.grip = base.GetChildControl<AxisControl>("grip");
				this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
				this.gripForce = base.GetChildControl<AxisControl>("gripForce");
				this.trigger = base.GetChildControl<AxisControl>("trigger");
				this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
				this.triggerTouched = base.GetChildControl<ButtonControl>("triggerTouched");
				this.thumbstick = base.GetChildControl<StickControl>("thumbstick");
				this.thumbstickClicked = base.GetChildControl<ButtonControl>("thumbstickClicked");
				this.thumbstickTouched = base.GetChildControl<ButtonControl>("thumbstickTouched");
				this.trackpad = base.GetChildControl<StickControl>("trackpad");
				this.trackpadTouched = base.GetChildControl<ButtonControl>("trackpadTouched");
				this.trackpadForce = base.GetChildControl<AxisControl>("trackpadForce");
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
