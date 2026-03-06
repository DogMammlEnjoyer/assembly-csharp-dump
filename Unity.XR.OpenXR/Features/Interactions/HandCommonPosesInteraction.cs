using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
	public class HandCommonPosesInteraction : OpenXRInteractionFeature
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
			return OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_interaction") && base.OnInstanceCreate(instance);
		}

		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(HandCommonPosesInteraction.HandInteractionPoses);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("Hand Interaction Poses OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("HandInteractionPoses");
		}

		protected override OpenXRInteractionFeature.InteractionProfileType GetInteractionProfileType()
		{
			if (!typeof(HandCommonPosesInteraction.HandInteractionPoses).IsSubclassOf(typeof(XRController)))
			{
				return OpenXRInteractionFeature.InteractionProfileType.Device;
			}
			return OpenXRInteractionFeature.InteractionProfileType.XRController;
		}

		protected override string GetDeviceLayoutName()
		{
			return "HandInteractionPoses";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "handinteractionposes",
				localizedName = "Hand Interaction Poses OpenXR",
				desiredInteractionProfile = "/interaction_profiles/unity/hand_interaction_poses",
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
								interactionProfileName = "/interaction_profiles/unity/hand_interaction_poses"
							}
						},
						isAdditive = true
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
								interactionProfileName = "/interaction_profiles/unity/hand_interaction_poses"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PokePose",
						localizedName = "Poke Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/poke_ext/pose",
								interactionProfileName = "/interaction_profiles/unity/hand_interaction_poses"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "PinchPose",
						localizedName = "Pinch Pose",
						type = OpenXRInteractionFeature.ActionType.Pose,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/pinch_ext/pose",
								interactionProfileName = "/interaction_profiles/unity/hand_interaction_poses"
							}
						},
						isAdditive = true
					}
				}
			};
			base.AddActionMap(map);
		}

		internal override void AddAdditiveActions(List<OpenXRInteractionFeature.ActionMapConfig> actionMaps, OpenXRInteractionFeature.ActionMapConfig additiveMap)
		{
			foreach (OpenXRInteractionFeature.ActionMapConfig actionMapConfig in actionMaps)
			{
				if ((from d in actionMapConfig.deviceInfos
				where d.userPath != null && (string.CompareOrdinal(d.userPath, "/user/hand/left") == 0 || string.CompareOrdinal(d.userPath, "/user/hand/right") == 0)
				select d).Any<OpenXRInteractionFeature.DeviceConfig>())
				{
					using (IEnumerator<OpenXRInteractionFeature.ActionConfig> enumerator2 = (from a in additiveMap.actions
					where a.isAdditive
					select a).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							OpenXRInteractionFeature.ActionConfig additiveAction = enumerator2.Current;
							bool flag = false;
							Func<OpenXRInteractionFeature.ActionBinding, bool> <>9__3;
							foreach (OpenXRInteractionFeature.ActionConfig actionConfig in (from m in actionMapConfig.actions
							where m.type == OpenXRInteractionFeature.ActionType.Pose
							select m).Distinct<OpenXRInteractionFeature.ActionConfig>().ToList<OpenXRInteractionFeature.ActionConfig>())
							{
								IEnumerable<OpenXRInteractionFeature.ActionBinding> bindings = actionConfig.bindings;
								Func<OpenXRInteractionFeature.ActionBinding, bool> predicate;
								if ((predicate = <>9__3) == null)
								{
									predicate = (<>9__3 = ((OpenXRInteractionFeature.ActionBinding b) => b.interactionPath != null && string.CompareOrdinal(b.interactionPath, additiveAction.bindings[0].interactionPath) == 0));
								}
								if (bindings.Where(predicate).Any<OpenXRInteractionFeature.ActionBinding>())
								{
									actionConfig.isAdditive = true;
									flag = true;
								}
							}
							if (!flag)
							{
								actionMapConfig.actions.Add(additiveAction);
							}
						}
					}
				}
			}
		}

		public const string featureId = "com.unity.openxr.feature.input.handinteractionposes";

		public const string profile = "/interaction_profiles/unity/hand_interaction_poses";

		public const string grip = "/input/grip/pose";

		public const string aim = "/input/aim/pose";

		public const string poke = "/input/poke_ext/pose";

		public const string pinch = "/input/pinch_ext/pose";

		private const string kDeviceLocalizedName = "Hand Interaction Poses OpenXR";

		public const string extensionString = "XR_EXT_hand_interaction";

		[Preserve]
		[InputControlLayout(displayName = "Hand Interaction Poses (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		}, isGenericTypeOfDevice = true)]
		public class HandInteractionPoses : OpenXRDevice
		{
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
			[InputControl(offset = 0U)]
			public UnityEngine.InputSystem.XR.PoseControl pokePose { get; private set; }

			[Preserve]
			[InputControl(offset = 0U)]
			public UnityEngine.InputSystem.XR.PoseControl pinchPose { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.devicePose = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("devicePose");
				this.pointer = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("pointer");
				this.pokePose = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("pokePose");
				this.pinchPose = base.GetChildControl<UnityEngine.InputSystem.XR.PoseControl>("pinchPose");
			}
		}
	}
}
