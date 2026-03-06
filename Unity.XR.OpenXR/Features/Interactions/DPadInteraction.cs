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
	public class DPadInteraction : OpenXRInteractionFeature
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
			string[] array = this.extensionStrings;
			for (int i = 0; i < array.Length; i++)
			{
				if (!OpenXRRuntime.IsExtensionEnabled(array[i]))
				{
					return false;
				}
			}
			return base.OnInstanceCreate(instance);
		}

		protected override void RegisterDeviceLayout()
		{
			Type typeFromHandle = typeof(DPadInteraction.DPad);
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			inputDeviceMatcher = inputDeviceMatcher.WithInterface("^(XRInput)", true);
			InputSystem.RegisterLayout(typeFromHandle, name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("DPad Interaction OpenXR", true)));
		}

		protected override void UnregisterDeviceLayout()
		{
			InputSystem.RemoveLayout("DPad");
		}

		protected override string GetDeviceLayoutName()
		{
			return "DPad";
		}

		protected override void RegisterActionMapsWithRuntime()
		{
			OpenXRInteractionFeature.ActionMapConfig map = new OpenXRInteractionFeature.ActionMapConfig
			{
				name = "dpadinteraction",
				localizedName = "DPad Interaction OpenXR",
				desiredInteractionProfile = "/interaction_profiles/unity/dpad",
				manufacturer = "",
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
						name = "thumbstickDpadUp",
						localizedName = " Thumbstick Dpad Up",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick/dpad_up",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "thumbstickDpadDown",
						localizedName = "Thumbstick Dpad Down",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick/dpad_down",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "thumbstickDpadLeft",
						localizedName = "Thumbstick Dpad Left",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick/dpad_left",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "thumbstickDpadRight",
						localizedName = "Thumbstick Dpad Right",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/thumbstick/dpad_right",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadDpadUp",
						localizedName = "Trackpad Dpad Up",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/dpad_up",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadDpadDown",
						localizedName = "Trackpad Dpad Down",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/dpad_down",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadDpadLeft",
						localizedName = "Trackpad Dpad Left",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/dpad_left",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadDpadRight",
						localizedName = "Trackpad Dpad Right",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/dpad_right",
								interactionProfileName = "/interaction_profiles/unity/dpad"
							}
						},
						isAdditive = true
					},
					new OpenXRInteractionFeature.ActionConfig
					{
						name = "trackpadDpadCenter",
						localizedName = "Trackpad Dpad Center",
						type = OpenXRInteractionFeature.ActionType.Binary,
						bindings = new List<OpenXRInteractionFeature.ActionBinding>
						{
							new OpenXRInteractionFeature.ActionBinding
							{
								interactionPath = "/input/trackpad/dpad_center",
								interactionProfileName = "/interaction_profiles/unity/dpad"
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
					bool flag = false;
					bool flag2 = false;
					foreach (OpenXRInteractionFeature.ActionConfig actionConfig in actionMapConfig.actions)
					{
						if (!flag)
						{
							if (actionConfig.bindings.FirstOrDefault((OpenXRInteractionFeature.ActionBinding b) => b.interactionPath.Contains("trackpad")) != null)
							{
								flag = true;
							}
						}
						if (!flag2)
						{
							if (actionConfig.bindings.FirstOrDefault((OpenXRInteractionFeature.ActionBinding b) => b.interactionPath.Contains("thumbstick")) != null)
							{
								flag2 = true;
							}
						}
					}
					foreach (OpenXRInteractionFeature.ActionConfig actionConfig2 in from a in additiveMap.actions
					where a.isAdditive
					select a)
					{
						if ((flag && actionConfig2.name.StartsWith("trackpad")) || (flag2 && actionConfig2.name.StartsWith("thumbstick")))
						{
							actionMapConfig.actions.Add(actionConfig2);
						}
					}
				}
			}
		}

		public const string featureId = "com.unity.openxr.feature.input.dpadinteraction";

		public float forceThresholdLeft = 0.5f;

		public float forceThresholdReleaseLeft = 0.4f;

		public float centerRegionLeft = 0.5f;

		public float wedgeAngleLeft = 1.5707964f;

		public bool isStickyLeft;

		public float forceThresholdRight = 0.5f;

		public float forceThresholdReleaseRight = 0.4f;

		public float centerRegionRight = 0.5f;

		public float wedgeAngleRight = 1.5707964f;

		public bool isStickyRight;

		public const string thumbstickDpadUp = "/input/thumbstick/dpad_up";

		public const string thumbstickDpadDown = "/input/thumbstick/dpad_down";

		public const string thumbstickDpadLeft = "/input/thumbstick/dpad_left";

		public const string thumbstickDpadRight = "/input/thumbstick/dpad_right";

		public const string trackpadDpadUp = "/input/trackpad/dpad_up";

		public const string trackpadDpadDown = "/input/trackpad/dpad_down";

		public const string trackpadDpadLeft = "/input/trackpad/dpad_left";

		public const string trackpadDpadRight = "/input/trackpad/dpad_right";

		public const string trackpadDpadCenter = "/input/trackpad/dpad_center";

		public const string profile = "/interaction_profiles/unity/dpad";

		private const string kDeviceLocalizedName = "DPad Interaction OpenXR";

		public string[] extensionStrings = new string[]
		{
			"XR_KHR_binding_modification",
			"XR_EXT_dpad_binding"
		};

		[Preserve]
		[InputControlLayout(displayName = "D-Pad Binding (OpenXR)", commonUsages = new string[]
		{
			"LeftHand",
			"RightHand"
		})]
		public class DPad : XRController
		{
			[Preserve]
			[InputControl]
			public ButtonControl thumbstickDpadUp { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl thumbstickDpadDown { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl thumbstickDpadLeft { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl thumbstickDpadRight { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl trackpadDpadUp { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl trackpadDpadDown { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl trackpadDpadLeft { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl trackpadDpadRight { get; private set; }

			[Preserve]
			[InputControl]
			public ButtonControl trackpadDpadCenter { get; private set; }

			protected override void FinishSetup()
			{
				base.FinishSetup();
				this.thumbstickDpadUp = base.GetChildControl<ButtonControl>("thumbstickDpadUp");
				this.thumbstickDpadDown = base.GetChildControl<ButtonControl>("thumbstickDpadDown");
				this.thumbstickDpadLeft = base.GetChildControl<ButtonControl>("thumbstickDpadLeft");
				this.thumbstickDpadRight = base.GetChildControl<ButtonControl>("thumbstickDpadRight");
				this.trackpadDpadUp = base.GetChildControl<ButtonControl>("trackpadDpadUp");
				this.trackpadDpadDown = base.GetChildControl<ButtonControl>("trackpadDpadDown");
				this.trackpadDpadLeft = base.GetChildControl<ButtonControl>("trackpadDpadLeft");
				this.trackpadDpadRight = base.GetChildControl<ButtonControl>("trackpadDpadRight");
				this.trackpadDpadCenter = base.GetChildControl<ButtonControl>("trackpadDpadCenter");
			}
		}
	}
}
