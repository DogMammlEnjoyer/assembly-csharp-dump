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
	public class HPReverbG2ControllerProfile : OpenXRInteractionFeature
	{
		protected internal override bool OnInstanceCreate(ulong instance)
		{
			return OpenXRRuntime.IsExtensionEnabled("XR_EXT_hp_mixed_reality_controller") && base.OnInstanceCreate(instance);
		}

		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(HPReverbG2ControllerProfile.ReverbG2Controller);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("HP Reverb G2 Controller OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("ReverbG2Controller");
		}

		protected override string GetDeviceLayoutName()
		{
			return "ReverbG2Controller";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "hpreverbg2controller",
				localizedName = "HP Reverb G2 Controller OpenXR",
				desiredInteractionProfile = "/interaction_profiles/hp/mixed_reality_controller",
				manufacturer = "HP",
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
								interactionPath = "/input/x/click",
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller",
								userPaths = new List<string>
								{
									"/user/hand/left"
								}
							},
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/a/click",
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller",
								userPaths = new List<string>
								{
									"/user/hand/right"
								}
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
								interactionPath = "/input/y/click",
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller",
								userPaths = new List<string>
								{
									"/user/hand/left"
								}
							},
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/b/click",
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller",
								userPaths = new List<string>
								{
									"/user/hand/right"
								}
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
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
								interactionProfileName = "/interaction_profiles/hp/mixed_reality_controller"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.hpreverb";

		public const string profile = "/interaction_profiles/hp/mixed_reality_controller";

		public const string buttonX = "/input/x/click";

		public const string buttonY = "/input/y/click";

		public const string buttonA = "/input/a/click";

		public const string buttonB = "/input/b/click";

		public const string menu = "/input/menu/click";

		public const string squeeze = "/input/squeeze/value";

		public const string trigger = "/input/trigger/value";

		public const string thumbstick = "/input/thumbstick";

		public const string thumbstickClick = "/input/thumbstick/click";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string haptic = "/output/haptic";

		private const string kDeviceLocalizedName = "HP Reverb G2 Controller OpenXR";

		[Preserve]
		[InputControlLayout(displayName = "HP Reverb G2 Controller (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class ReverbG2Controller : XRControllerWithRumble
		{
			[Preserve]
			[InputControl(aliases = new string[]
			{
				"A",
				"X",
				"buttonA",
				"buttonX"
			}, usage = "PrimaryButton")]
			public ButtonControl primaryButton { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"B",
				"Y",
				"buttonB",
				"buttonY"
			}, usage = "SecondaryButton")]
			public ButtonControl secondaryButton { get; private set; }

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
			[InputControl(usage = "Trigger")]
			public AxisControl trigger { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"indexButton",
				"indexTouched",
				"triggerbutton"
			}, usage = "TriggerButton")]
			public ButtonControl triggerPressed { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Primary2DAxis",
				"Joystick"
			}, usage = "Primary2DAxis")]
			public Vector2Control thumbstick { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"JoystickOrPadPressed",
				"thumbstickClick",
				"joystickClicked"
			}, usage = "Primary2DAxisClick")]
			public ButtonControl thumbstickClicked { get; private set; }

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
			[InputControl(offset = 29U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 32U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 36U, alias = "gripPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 48U, alias = "gripOrientation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 96U)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 108U, alias = "pointerOrientation")]
			public QuaternionControl pointerRotation { get; private set; }

			[Preserve]
			[InputControl(usage = "Haptic")]
			public HapticControl haptic { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.primaryButton = base.GetChildControl<ButtonControl>("primaryButton");
				this.secondaryButton = base.GetChildControl<ButtonControl>("secondaryButton");
				this.menu = base.GetChildControl<ButtonControl>("menu");
				this.grip = base.GetChildControl<AxisControl>("grip");
				this.gripPressed = base.GetChildControl<ButtonControl>("gripPressed");
				this.trigger = base.GetChildControl<AxisControl>("trigger");
				this.triggerPressed = base.GetChildControl<ButtonControl>("triggerPressed");
				this.thumbstick = base.GetChildControl<StickControl>("thumbstick");
				this.thumbstickClicked = base.GetChildControl<ButtonControl>("thumbstickClicked");
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
