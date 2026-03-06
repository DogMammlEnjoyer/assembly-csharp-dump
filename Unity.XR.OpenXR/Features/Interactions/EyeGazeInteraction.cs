using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
	public class EyeGazeInteraction : OpenXRInteractionFeature
	{
		protected internal override bool OnInstanceCreate(ulong instance)
		{
			return OpenXRRuntime.IsExtensionEnabled("XR_EXT_eye_gaze_interaction") && base.OnInstanceCreate(instance);
		}

		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(EyeGazeInteraction.EyeGazeDevice);
			string name = "EyeGaze";
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Eye Tracking OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("EyeGaze");
		}

		protected override OpenXRInteractionFeature.InteractionProfileType GetInteractionProfileType()
		{
			if (!typeof(EyeGazeInteraction.EyeGazeDevice).IsSubclassOf(typeof(XRController)))
			{
				return OpenXRInteractionFeature.InteractionProfileType.Device;
			}
			return OpenXRInteractionFeature.InteractionProfileType.XRController;
		}

		protected override string GetDeviceLayoutName()
		{
			return "EyeGaze";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "eyegaze",
				localizedName = "Eye Tracking OpenXR",
				desiredInteractionProfile = "/interaction_profiles/ext/eye_gaze_interaction",
				manufacturer = "",
				serialNumber = "",
				deviceInfos = new List<OpenXRInteractionFeature.DeviceConfig>
				{
					new OpenXRInteractionFeature.DeviceConfig
					{
						characteristics = (InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.TrackedDevice),
						userPath = "/user/eyes_ext"
					}
				},
				actions = new List<OpenXRInteractionFeature.ActionConfig>
				{
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "pose",
						localizedName = "Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						usages = new List<string>
						{
							"Device",
							"gaze"
						},
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/gaze_ext/pose",
								interactionProfileName = "/interaction_profiles/ext/eye_gaze_interaction"
							}
						}
					}
				}
			};
			base.AddActionMap(map);
		}

		public const string featureId = "com.unity.openxr.feature.input.eyetracking";

		private const string userPath = "/user/eyes_ext";

		private const string profile = "/interaction_profiles/ext/eye_gaze_interaction";

		private const string pose = "/input/gaze_ext/pose";

		private const string kDeviceLocalizedName = "Eye Tracking OpenXR";

		public const string extensionString = "XR_EXT_eye_gaze_interaction";

		private const string layoutName = "EyeGaze";

		[Preserve]
		[InputControlLayout(displayName = "Eye Gaze (OpenXR)", isGenericTypeOfDevice = true)]
		public class EyeGazeDevice : OpenXRDevice
		{
			[Preserve]
			[InputControl(offset = 0U, usages = new string[]
			{
				"Device",
				"gaze"
			})]
			public UnityEngine.InputSystem.XR.PoseControl pose { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.pose = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("pose");
			}
		}
	}
}
