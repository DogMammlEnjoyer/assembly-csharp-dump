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
	public class KHRSimpleControllerProfile : OpenXRInteractionFeature
	{
		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(KHRSimpleControllerProfile.KHRSimpleController);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("KHR Simple Controller OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout(typeof(KHRSimpleControllerProfile.KHRSimpleController).Name);
		}

		protected override string GetDeviceLayoutName()
		{
			return "KHRSimpleController";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "khrsimplecontroller",
				localizedName = "KHR Simple Controller OpenXR",
				desiredInteractionProfile = "/interaction_profiles/khr/simple_controller",
				manufacturer = "Khronos",
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
						name = "select",
						localizedName = "Select",
						type = OpenXRInteractionFeature.ActionType.Binary,
						usages = new List<string>
						{
							"PrimaryButton"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/select/click",
								interactionProfileName = "/interaction_profiles/khr/simple_controller"
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
								interactionProfileName = "/interaction_profiles/khr/simple_controller"
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
								interactionProfileName = "/interaction_profiles/khr/simple_controller"
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
								interactionProfileName = "/interaction_profiles/khr/simple_controller"
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
								interactionProfileName = "/interaction_profiles/khr/simple_controller"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.khrsimpleprofile";

		public const string profile = "/interaction_profiles/khr/simple_controller";

		public const string select = "/input/select/click";

		public const string menu = "/input/menu/click";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string haptic = "/output/haptic";

		private const string kDeviceLocalizedName = "KHR Simple Controller OpenXR";

		[Preserve]
		[InputControlLayout(displayName = "Khronos Simple Controller (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class KHRSimpleController : XRControllerWithRumble
		{
			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Secondary",
				"selectbutton"
			}, usage = "PrimaryButton")]
			public ButtonControl select { get; private set; }

			[Preserve]
			[InputControl(aliases = new string[]
			{
				"Primary",
				"menubutton"
			}, usage = "MenuButton")]
			public ButtonControl menu { get; private set; }

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
			[InputControl(offset = 2U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 4U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 8U, alias = "gripPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 20U, alias = "gripOrientation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 68U)]
			public Vector3Control pointerPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 80U, alias = "pointerOrientation")]
			public QuaternionControl pointerRotation { get; private set; }

			[Preserve]
			[InputControl(usage = "Haptic")]
			public HapticControl haptic { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.menu = base.GetChildControl<ButtonControl>("menu");
				this.select = base.GetChildControl<ButtonControl>("select");
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
