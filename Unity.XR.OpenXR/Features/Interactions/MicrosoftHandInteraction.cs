using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
	public class MicrosoftHandInteraction : OpenXRInteractionFeature
	{
		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(MicrosoftHandInteraction.HoloLensHand);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("HoloLens Hand OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("HoloLensHand");
		}

		protected override string GetDeviceLayoutName()
		{
			return "HoloLensHand";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "microsofthandinteraction",
				localizedName = "HoloLens Hand OpenXR",
				desiredInteractionProfile = "/interaction_profiles/microsoft/hand_interaction",
				manufacturer = "Microsoft",
				serialNumber = "",
				deviceInfos = new List<OpenXRInteractionFeature.DeviceConfig>
				{
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left),
						userPath = "/user/hand/left"
					},
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right),
						userPath = "/user/hand/right"
					}
				},
				actions = new List<OpenXRInteractionFeature.ActionConfig>
				{
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "select",
						localizedName = "Select",
						type = OpenXRInteractionFeature.ActionType.Axis1D,
						usages = new List<string>
						{
							"PrimaryAxis"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/select/value",
								interactionProfileName = "/interaction_profiles/microsoft/hand_interaction"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "selectPressed",
						localizedName = "Select Pressed",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PrimaryButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/select/value",
								interactionProfileName = "/interaction_profiles/microsoft/hand_interaction"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "squeeze",
						localizedName = "Squeeze",
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
								interactionProfileName = "/interaction_profiles/microsoft/hand_interaction"
							}
						}
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "squeezePressed",
						localizedName = "Squeeze Pressed",
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
								interactionProfileName = "/interaction_profiles/microsoft/hand_interaction"
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
								interactionProfileName = "/interaction_profiles/microsoft/hand_interaction"
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
								interactionProfileName = "/interaction_profiles/microsoft/hand_interaction"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.handtracking";

		public const string extensionString = "XR_MSFT_hand_interaction";

		public const string profile = "/interaction_profiles/microsoft/hand_interaction";

		public const string select = "/input/select/value";

		public const string squeeze = "/input/squeeze/value";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		private const string kDeviceLocalizedName = "HoloLens Hand OpenXR";

		[Preserve]
		[InputControlLayout(displayName = "Hololens Hand (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class HoloLensHand : XRController
		{
			[Preserve]
			[InputControl(usage = "PrimaryAxis")]
			public AxisControl select { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Primary",
				"selectbutton"
			}, usages = new string[]
			{
				"PrimaryButton"
			})]
			public ButtonControl selectPressed { get; private set; }

			[Preserve]
			[InputControl(alias = "Secondary", usage = "Grip")]
			public AxisControl squeeze { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"GripButton",
				"squeezeClicked"
			}, usages = new string[]
			{
				"GripButton"
			})]
			public ButtonControl squeezePressed { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, alias = "device", usage = "Device")]
			public PoseControl devicePose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U, usage = "Pointer")]
			public PoseControl pointer { get; private set; }

			[Preserve]
			[InputControl(offset = 132U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 136U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 20U, alias = "gripPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 32U, alias = "gripOrientation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 80U)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 92U, alias = "pointerOrientation")]
			public QuaternionControl pointerRotation { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.select = base.GetChildControl<AxisControl>("select");
				this.selectPressed = base.GetChildControl<ButtonControl>("selectPressed");
				this.squeeze = base.GetChildControl<AxisControl>("squeeze");
				this.squeezePressed = base.GetChildControl<ButtonControl>("squeezePressed");
				this.devicePose = base.GetChildControl<PoseControl>("devicePose");
				this.pointer = base.GetChildControl<PoseControl>("pointer");
				this.isTracked = base.GetChildControl<ButtonControl>("isTracked");
				this.trackingState = base.GetChildControl<IntegerControl>("trackingState");
				this.devicePosition = base.GetChildControl<Vector3Control>("devicePosition");
				this.deviceRotation = base.GetChildControl<QuaternionControl>("deviceRotation");
				this.pointerPosition = base.GetChildControl<Vector3Control>("pointerPosition");
				this.pointerRotation = base.GetChildControl<QuaternionControl>("pointerRotation");
			}
		}
	}
}
