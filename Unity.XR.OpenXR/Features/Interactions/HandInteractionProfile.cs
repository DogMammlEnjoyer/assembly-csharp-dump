using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
	public class HandInteractionProfile : OpenXRInteractionFeature
	{
		protected internal override bool OnInstanceCreate(ulong instance)
		{
			return OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_interaction") && base.OnInstanceCreate(instance);
		}

		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(HandInteractionProfile.HandInteraction);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Hand Interaction OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("HandInteraction");
		}

		protected override string GetDeviceLayoutName()
		{
			return "HandInteraction";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "handinteraction",
				localizedName = "Hand Interaction OpenXR",
				desiredInteractionProfile = "/interaction_profiles/ext/hand_interaction_ext",
				manufacturer = "",
				serialNumber = "",
				deviceInfos = new List<OpenXRInteractionFeature.DeviceConfig>
				{
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Left),
						userPath = "/user/hand/left"
					},
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Right),
						userPath = "/user/hand/right"
					}
				},
				actions = new List<OpenXRInteractionFeature.ActionConfig>
				{
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "devicePose",
						localizedName = "Grip Pose",
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
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
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
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PokePose",
						localizedName = "Poke Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						usages = new List<string>
						{
							"Poke"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/poke_ext/pose",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PinchPose",
						localizedName = "Pinch Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						usages = new List<string>
						{
							"Pinch"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/pinch_ext/pose",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PinchValue",
						localizedName = "Pinch Value",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"PinchValue"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/pinch_ext/value",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PinchTouched",
						localizedName = "Pinch Touched",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PinchTouched"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/pinch_ext/value",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PinchReady",
						localizedName = "Pinch Ready",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PinchReady"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/pinch_ext/ready_ext",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PointerActivateValue",
						localizedName = "Pointer Activate Value",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"PointerActivateValue"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/aim_activate_ext/value",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PointerActivated",
						localizedName = "Pointer Activated",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PointerActivated"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/aim_activate_ext/value",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PointerActivateReady",
						localizedName = "Pointer Activate Ready",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PointerActivateReady"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/aim_activate_ext/ready_ext",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "GraspValue",
						localizedName = "Grasp Value",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"GraspValue"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/grasp_ext/value",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "GraspFirm",
						localizedName = "Grasp Firm",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"GraspFirm"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/grasp_ext/value",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "GraspReady",
						localizedName = "Grasp Ready",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"GraspReady"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/grasp_ext/ready_ext",
								interactionProfileName = "/interaction_profiles/ext/hand_interaction_ext"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.handinteraction";

		public const string profile = "/interaction_profiles/ext/hand_interaction_ext";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string poke = "/input/poke_ext/pose";

		public const string pinch = "/input/pinch_ext/pose";

		public const string pinchValue = "/input/pinch_ext/value";

		public const string pinchReady = "/input/pinch_ext/ready_ext";

		public const string pointerActivateValue = "/input/aim_activate_ext/value";

		public const string pointerActivateReady = "/input/aim_activate_ext/ready_ext";

		public const string graspValue = "/input/grasp_ext/value";

		public const string graspReady = "/input/grasp_ext/ready_ext";

		private const string kDeviceLocalizedName = "Hand Interaction OpenXR";

		public const string extensionString = "XR_EXT_hand_interaction";

		[Preserve]
		[InputControlLayout(displayName = "Hand Interaction (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class HandInteraction : XRController
		{
			[Preserve]
			[InputControl(offset = 0U, aliases = new string[]
			{
				"device",
				"gripPose"
			}, usage = "Device")]
			public PoseControl devicePose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, alias = "aimPose", usage = "Pointer")]
			public PoseControl pointer { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, usage = "Poke")]
			public PoseControl pokePose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, usage = "Pinch")]
			public PoseControl pinchPose { get; private set; }

			[Preserve]
			[InputControl(usage = "PinchValue")]
			public AxisControl pinchValue { get; private set; }

			[Preserve]
			[InputControl(usage = "PinchTouched")]
			public ButtonControl pinchTouched { get; private set; }

			[Preserve]
			[InputControl(usage = "PinchReady")]
			public ButtonControl pinchReady { get; private set; }

			[Preserve]
			[InputControl(usage = "PointerActivateValue")]
			public AxisControl pointerActivateValue { get; private set; }

			[Preserve]
			[InputControl(usage = "PointerActivated")]
			public ButtonControl pointerActivated { get; private set; }

			[Preserve]
			[InputControl(usage = "PointerActivateReady")]
			public ButtonControl pointerActivateReady { get; private set; }

			[Preserve]
			[InputControl(usage = "GraspValue")]
			public AxisControl graspValue { get; private set; }

			[Preserve]
			[InputControl(usage = "GraspFirm")]
			public ButtonControl graspFirm { get; private set; }

			[Preserve]
			[InputControl(usage = "GraspReady")]
			public ButtonControl graspReady { get; private set; }

			[Preserve]
			[InputControl(offset = 2U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 4U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 8U, noisy = true, alias = "gripPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 20U, noisy = true, alias = "gripRotation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 68U, noisy = true)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 80U, noisy = true)]
			public QuaternionControl pointerRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 128U, noisy = true)]
			public Vector3Control pokePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 140U, noisy = true)]
			public QuaternionControl pokeRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 188U, noisy = true)]
			public Vector3Control pinchPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 200U, noisy = true)]
			public QuaternionControl pinchRotation { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.devicePose = base.GetChildControl<PoseControl>("devicePose");
				this.pointer = base.GetChildControl<PoseControl>("pointer");
				this.pokePose = base.GetChildControl<PoseControl>("pokePose");
				this.pinchPose = base.GetChildControl<PoseControl>("pinchPose");
				this.pinchValue = base.GetChildControl<AxisControl>("pinchValue");
				this.pinchTouched = base.GetChildControl<ButtonControl>("pinchTouched");
				this.pinchReady = base.GetChildControl<ButtonControl>("pinchReady");
				this.pointerActivateValue = base.GetChildControl<AxisControl>("pointerActivateValue");
				this.pointerActivated = base.GetChildControl<ButtonControl>("pointerActivated");
				this.pointerActivateReady = base.GetChildControl<ButtonControl>("pointerActivateReady");
				this.graspValue = base.GetChildControl<AxisControl>("graspValue");
				this.graspFirm = base.GetChildControl<ButtonControl>("graspFirm");
				this.graspReady = base.GetChildControl<ButtonControl>("graspReady");
				this.isTracked = base.GetChildControl<ButtonControl>("isTracked");
				this.trackingState = base.GetChildControl<IntegerControl>("trackingState");
				this.devicePosition = base.GetChildControl<Vector3Control>("devicePosition");
				this.deviceRotation = base.GetChildControl<QuaternionControl>("deviceRotation");
				this.pointerPosition = base.GetChildControl<Vector3Control>("pointerPosition");
				this.pointerRotation = base.GetChildControl<QuaternionControl>("pointerRotation");
				this.pokePosition = base.GetChildControl<Vector3Control>("pokePosition");
				this.pokeRotation = base.GetChildControl<QuaternionControl>("pokeRotation");
				this.pinchPosition = base.GetChildControl<Vector3Control>("pinchPosition");
				this.pinchRotation = base.GetChildControl<QuaternionControl>("pinchRotation");
			}
		}
	}
}
