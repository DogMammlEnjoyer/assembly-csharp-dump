using System;
using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Features
{
	[Serializable]
	public abstract class OpenXRInteractionFeature : OpenXRFeature
	{
		internal virtual bool IsAdditive
		{
			get
			{
				return false;
			}
		}

		protected virtual void RegisterDeviceLayout()
		{
		}

		protected virtual void UnregisterDeviceLayout()
		{
		}

		protected virtual void RegisterActionMapsWithRuntime()
		{
		}

		protected internal override bool OnInstanceCreate(ulong xrSession)
		{
			this.RegisterDeviceLayout();
			return true;
		}

		protected virtual OpenXRInteractionFeature.InteractionProfileType GetInteractionProfileType()
		{
			return OpenXRInteractionFeature.InteractionProfileType.XRController;
		}

		protected virtual string GetDeviceLayoutName()
		{
			return "";
		}

		internal void CreateActionMaps(List<OpenXRInteractionFeature.ActionMapConfig> configs)
		{
			OpenXRInteractionFeature.m_CreatedActionMaps = configs;
			this.RegisterActionMapsWithRuntime();
			OpenXRInteractionFeature.m_CreatedActionMaps = null;
		}

		protected void AddActionMap(OpenXRInteractionFeature.ActionMapConfig map)
		{
			if (map == null)
			{
				throw new ArgumentNullException("map");
			}
			if (OpenXRInteractionFeature.m_CreatedActionMaps == null)
			{
				throw new InvalidOperationException("ActionMap must be added from within the RegisterActionMapsWithRuntime method");
			}
			OpenXRInteractionFeature.m_CreatedActionMaps.Add(map);
		}

		internal virtual void AddAdditiveActions(List<OpenXRInteractionFeature.ActionMapConfig> actionMaps, OpenXRInteractionFeature.ActionMapConfig additiveMap)
		{
		}

		protected internal override void OnEnabledChange()
		{
			base.OnEnabledChange();
		}

		internal static void RegisterLayouts()
		{
			foreach (OpenXRFeature openXRFeature in OpenXRSettings.Instance.GetFeatures<OpenXRInteractionFeature>())
			{
				if (openXRFeature.enabled)
				{
					((OpenXRInteractionFeature)openXRFeature).RegisterDeviceLayout();
				}
			}
		}

		private static List<OpenXRInteractionFeature.ActionMapConfig> m_CreatedActionMaps = null;

		private static Dictionary<OpenXRInteractionFeature.InteractionProfileType, Dictionary<string, bool>> m_InteractionProfileEnabledMaps = new Dictionary<OpenXRInteractionFeature.InteractionProfileType, Dictionary<string, bool>>();

		[Serializable]
		protected internal enum ActionType
		{
			Binary,
			Axis1D,
			Axis2D,
			Pose,
			Vibrate,
			Count
		}

		[Serializable]
		protected internal class ActionBinding
		{
			public string interactionProfileName;

			public string interactionPath;

			public List<string> userPaths;
		}

		[Serializable]
		protected internal class ActionConfig
		{
			public string name;

			public OpenXRInteractionFeature.ActionType type;

			public string localizedName;

			public List<OpenXRInteractionFeature.ActionBinding> bindings;

			public List<string> usages;

			public bool isAdditive;
		}

		protected internal class DeviceConfig
		{
			public InputDeviceCharacteristics characteristics;

			public string userPath;
		}

		[Serializable]
		protected internal class ActionMapConfig
		{
			public string name;

			public string localizedName;

			public List<OpenXRInteractionFeature.DeviceConfig> deviceInfos;

			public List<OpenXRInteractionFeature.ActionConfig> actions;

			public string desiredInteractionProfile;

			public string manufacturer;

			public string serialNumber;
		}

		public static class UserPaths
		{
			public const string leftHand = "/user/hand/left";

			public const string rightHand = "/user/hand/right";

			public const string head = "/user/head";

			public const string gamepad = "/user/gamepad";

			public const string treadmill = "/user/treadmill";
		}

		public enum InteractionProfileType
		{
			Device,
			XRController
		}
	}
}
