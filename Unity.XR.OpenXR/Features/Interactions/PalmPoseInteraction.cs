using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
	public class PalmPoseInteraction : OpenXRInteractionFeature
	{
		internal override bool IsAdditive
		{
			get
			{
				return true;
			}
		}

		protected internal override bool OnInstanceCreate(ulong instance)
		{
			return OpenXRRuntime.IsExtensionEnabled("XR_EXT_palm_pose") && base.OnInstanceCreate(instance);
		}

		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(PalmPoseInteraction.PalmPose);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Palm Pose Interaction OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("PalmPose");
		}

		protected override string GetDeviceLayoutName()
		{
			return "PalmPose";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "palmposeinteraction",
				localizedName = "Palm Pose Interaction OpenXR",
				desiredInteractionProfile = "/interaction_profiles/ext/palmpose",
				manufacturer = "",
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
						name = "palmpose",
						localizedName = "Palm Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						bindings = this.AddBindingBasedOnRuntimeAPIVersion(),
						isAdditive = true
					}
				}
			};
			base.AddActionMap(map);
		}

		internal List<OpenXRInteractionFeature.ActionBinding> AddBindingBasedOnRuntimeAPIVersion()
		{
			List<OpenXRInteractionFeature.ActionBinding> result;
			if (OpenXRRuntime.isRuntimeAPIVersionGreaterThan1_1())
			{
				result = new List<OpenXRInteractionFeature.ActionBinding>
				{
					new OpenXRInteractionFeature.ActionBinding
					{
						interactionPath = "/input/grip_surface/pose",
						interactionProfileName = "/interaction_profiles/ext/palmpose"
					}
				};
			}
			else
			{
				result = new List<OpenXRInteractionFeature.ActionBinding>
				{
					new OpenXRInteractionFeature.ActionBinding
					{
						interactionPath = "/input/palm_ext/pose",
						interactionProfileName = "/interaction_profiles/ext/palmpose"
					}
				};
			}
			return result;
		}

		internal override void AddAdditiveActions(List<OpenXRInteractionFeature.ActionMapConfig> actionMaps, OpenXRInteractionFeature.ActionMapConfig additiveMap)
		{
			foreach (OpenXRInteractionFeature.ActionMapConfig actionMapConfig in actionMaps)
			{
				if ((from d in actionMapConfig.deviceInfos
				where d.userPath != null && (string.CompareOrdinal(d.userPath, "/user/hand/left") == 0 || string.CompareOrdinal(d.userPath, "/user/hand/right") == 0)
				select d).Any<OpenXRInteractionFeature.DeviceConfig>())
				{
					foreach (OpenXRInteractionFeature.ActionConfig item in from a in additiveMap.actions
					where a.isAdditive
					select a)
					{
						actionMapConfig.actions.Add(item);
					}
				}
			}
		}

		public const string featureId = "com.unity.openxr.feature.input.palmpose";

		public const string palmPose = "/input/palm_ext/pose";

		public const string gripSurfacePose = "/input/grip_surface/pose";

		public const string profile = "/interaction_profiles/ext/palmpose";

		private const string kDeviceLocalizedName = "Palm Pose Interaction OpenXR";

		public const string extensionString = "XR_EXT_palm_pose";

		[Preserve]
		[InputControlLayout(displayName = "Palm Pose (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class PalmPose : XRController
		{
			[Preserve]
			[InputControl(offset = 0U)]
			public PoseControl palmPose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U)]
			public new ButtonControl isTracked { get; private set; }

			[Preserve]
			[InputControl(offset = 4U)]
			public new IntegerControl trackingState { get; private set; }

			[Preserve]
			[InputControl(offset = 8U, noisy = true, alias = "palmPosition")]
			public new Vector3Control devicePosition { get; private set; }

			[Preserve]
			[InputControl(offset = 20U, noisy = true, alias = "palmRotation")]
			public new QuaternionControl deviceRotation { get; private set; }

			[Preserve]
			[InputControl(offset = 8U, noisy = true)]
			public Vector3Control palmPosition { get; private set; }

			[Preserve]
			[InputControl(offset = 20U, noisy = true)]
			public QuaternionControl palmRotation { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.palmPose = base.GetChildControl<PoseControl>("palmPose");
			}
		}
	}
}
