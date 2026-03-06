using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	public class InputSettings : ScriptableObject
	{
		public InputSettings.UpdateMode updateMode
		{
			get
			{
				return this.m_UpdateMode;
			}
			set
			{
				if (this.m_UpdateMode == value)
				{
					return;
				}
				this.m_UpdateMode = value;
				this.OnChange();
			}
		}

		public InputSettings.ScrollDeltaBehavior scrollDeltaBehavior
		{
			get
			{
				return this.m_ScrollDeltaBehavior;
			}
			set
			{
				if (this.m_ScrollDeltaBehavior == value)
				{
					return;
				}
				this.m_ScrollDeltaBehavior = value;
				this.OnChange();
			}
		}

		public bool compensateForScreenOrientation
		{
			get
			{
				return this.m_CompensateForScreenOrientation;
			}
			set
			{
				if (this.m_CompensateForScreenOrientation == value)
				{
					return;
				}
				this.m_CompensateForScreenOrientation = value;
				this.OnChange();
			}
		}

		[Obsolete("filterNoiseOnCurrent is deprecated, filtering of noise is always enabled now.", false)]
		public bool filterNoiseOnCurrent
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public float defaultDeadzoneMin
		{
			get
			{
				return this.m_DefaultDeadzoneMin;
			}
			set
			{
				if (this.m_DefaultDeadzoneMin == value)
				{
					return;
				}
				this.m_DefaultDeadzoneMin = value;
				this.OnChange();
			}
		}

		public float defaultDeadzoneMax
		{
			get
			{
				return this.m_DefaultDeadzoneMax;
			}
			set
			{
				if (this.m_DefaultDeadzoneMax == value)
				{
					return;
				}
				this.m_DefaultDeadzoneMax = value;
				this.OnChange();
			}
		}

		public float defaultButtonPressPoint
		{
			get
			{
				return this.m_DefaultButtonPressPoint;
			}
			set
			{
				if (this.m_DefaultButtonPressPoint == value)
				{
					return;
				}
				this.m_DefaultButtonPressPoint = Mathf.Clamp(value, 0.0001f, float.MaxValue);
				this.OnChange();
			}
		}

		public float buttonReleaseThreshold
		{
			get
			{
				return this.m_ButtonReleaseThreshold;
			}
			set
			{
				if (this.m_ButtonReleaseThreshold == value)
				{
					return;
				}
				this.m_ButtonReleaseThreshold = value;
				this.OnChange();
			}
		}

		public float defaultTapTime
		{
			get
			{
				return this.m_DefaultTapTime;
			}
			set
			{
				if (this.m_DefaultTapTime == value)
				{
					return;
				}
				this.m_DefaultTapTime = value;
				this.OnChange();
			}
		}

		public float defaultSlowTapTime
		{
			get
			{
				return this.m_DefaultSlowTapTime;
			}
			set
			{
				if (this.m_DefaultSlowTapTime == value)
				{
					return;
				}
				this.m_DefaultSlowTapTime = value;
				this.OnChange();
			}
		}

		public float defaultHoldTime
		{
			get
			{
				return this.m_DefaultHoldTime;
			}
			set
			{
				if (this.m_DefaultHoldTime == value)
				{
					return;
				}
				this.m_DefaultHoldTime = value;
				this.OnChange();
			}
		}

		public float tapRadius
		{
			get
			{
				return this.m_TapRadius;
			}
			set
			{
				if (this.m_TapRadius == value)
				{
					return;
				}
				this.m_TapRadius = value;
				this.OnChange();
			}
		}

		public float multiTapDelayTime
		{
			get
			{
				return this.m_MultiTapDelayTime;
			}
			set
			{
				if (this.m_MultiTapDelayTime == value)
				{
					return;
				}
				this.m_MultiTapDelayTime = value;
				this.OnChange();
			}
		}

		public InputSettings.BackgroundBehavior backgroundBehavior
		{
			get
			{
				return this.m_BackgroundBehavior;
			}
			set
			{
				if (this.m_BackgroundBehavior == value)
				{
					return;
				}
				this.m_BackgroundBehavior = value;
				this.OnChange();
			}
		}

		public InputSettings.EditorInputBehaviorInPlayMode editorInputBehaviorInPlayMode
		{
			get
			{
				return this.m_EditorInputBehaviorInPlayMode;
			}
			set
			{
				if (this.m_EditorInputBehaviorInPlayMode == value)
				{
					return;
				}
				this.m_EditorInputBehaviorInPlayMode = value;
				this.OnChange();
			}
		}

		public InputSettings.InputActionPropertyDrawerMode inputActionPropertyDrawerMode
		{
			get
			{
				return this.m_InputActionPropertyDrawerMode;
			}
			set
			{
				if (this.m_InputActionPropertyDrawerMode == value)
				{
					return;
				}
				this.m_InputActionPropertyDrawerMode = value;
				this.OnChange();
			}
		}

		public int maxEventBytesPerUpdate
		{
			get
			{
				return this.m_MaxEventBytesPerUpdate;
			}
			set
			{
				if (this.m_MaxEventBytesPerUpdate == value)
				{
					return;
				}
				this.m_MaxEventBytesPerUpdate = value;
				this.OnChange();
			}
		}

		public int maxQueuedEventsPerUpdate
		{
			get
			{
				return this.m_MaxQueuedEventsPerUpdate;
			}
			set
			{
				if (this.m_MaxQueuedEventsPerUpdate == value)
				{
					return;
				}
				this.m_MaxQueuedEventsPerUpdate = value;
				this.OnChange();
			}
		}

		public ReadOnlyArray<string> supportedDevices
		{
			get
			{
				return new ReadOnlyArray<string>(this.m_SupportedDevices);
			}
			set
			{
				if (this.supportedDevices.Count == value.Count)
				{
					bool flag = false;
					for (int i = 0; i < this.supportedDevices.Count; i++)
					{
						if (this.m_SupportedDevices[i] != value[i])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return;
					}
				}
				this.m_SupportedDevices = value.ToArray();
				this.OnChange();
			}
		}

		public bool disableRedundantEventsMerging
		{
			get
			{
				return this.m_DisableRedundantEventsMerging;
			}
			set
			{
				if (this.m_DisableRedundantEventsMerging == value)
				{
					return;
				}
				this.m_DisableRedundantEventsMerging = value;
				this.OnChange();
			}
		}

		public bool shortcutKeysConsumeInput
		{
			get
			{
				return this.m_ShortcutKeysConsumeInputs;
			}
			set
			{
				if (this.m_ShortcutKeysConsumeInputs == value)
				{
					return;
				}
				this.m_ShortcutKeysConsumeInputs = value;
				this.OnChange();
			}
		}

		public void SetInternalFeatureFlag(string featureName, bool enabled)
		{
			if (string.IsNullOrEmpty(featureName))
			{
				throw new ArgumentNullException("featureName");
			}
			if (this.m_FeatureFlags == null)
			{
				this.m_FeatureFlags = new HashSet<string>();
			}
			if (enabled)
			{
				this.m_FeatureFlags.Add(featureName.ToUpperInvariant());
			}
			else
			{
				this.m_FeatureFlags.Remove(featureName.ToUpperInvariant());
			}
			this.OnChange();
		}

		internal bool IsFeatureEnabled(string featureName)
		{
			return this.m_FeatureFlags != null && this.m_FeatureFlags.Contains(featureName.ToUpperInvariant());
		}

		internal void OnChange()
		{
			if (InputSystem.settings == this)
			{
				InputSystem.s_Manager.ApplySettings();
			}
		}

		private static bool CompareFloats(float a, float b)
		{
			return a - b <= float.Epsilon;
		}

		private static bool CompareSets<T>(ReadOnlyArray<T> a, ReadOnlyArray<T> b)
		{
			if (a == null)
			{
				return b == null;
			}
			if (b == null)
			{
				return false;
			}
			for (int i = 0; i < a.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < b.Count; j++)
				{
					T t = a[i];
					if (t.Equals(b[j]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private static bool CompareFeatureFlag(InputSettings a, InputSettings b, string featureName)
		{
			return a.IsFeatureEnabled(featureName) == b.IsFeatureEnabled(featureName);
		}

		internal static bool AreEqual(InputSettings a, InputSettings b)
		{
			if (a == null)
			{
				return b == null;
			}
			return b != null && (a == b || (a.updateMode == b.updateMode && a.compensateForScreenOrientation == b.compensateForScreenOrientation && InputSettings.CompareFloats(a.defaultDeadzoneMin, b.defaultDeadzoneMin) && InputSettings.CompareFloats(a.defaultDeadzoneMax, b.defaultDeadzoneMax) && InputSettings.CompareFloats(a.defaultButtonPressPoint, b.defaultButtonPressPoint) && InputSettings.CompareFloats(a.buttonReleaseThreshold, b.buttonReleaseThreshold) && InputSettings.CompareFloats(a.defaultTapTime, b.defaultTapTime) && InputSettings.CompareFloats(a.defaultSlowTapTime, b.defaultSlowTapTime) && InputSettings.CompareFloats(a.defaultHoldTime, b.defaultHoldTime) && InputSettings.CompareFloats(a.tapRadius, b.tapRadius) && InputSettings.CompareFloats(a.multiTapDelayTime, b.multiTapDelayTime) && a.backgroundBehavior == b.backgroundBehavior && a.editorInputBehaviorInPlayMode == b.editorInputBehaviorInPlayMode && a.inputActionPropertyDrawerMode == b.inputActionPropertyDrawerMode && a.maxEventBytesPerUpdate == b.maxEventBytesPerUpdate && a.maxQueuedEventsPerUpdate == b.maxQueuedEventsPerUpdate && InputSettings.CompareSets<string>(a.supportedDevices, b.supportedDevices) && a.disableRedundantEventsMerging == b.disableRedundantEventsMerging && a.shortcutKeysConsumeInput == b.shortcutKeysConsumeInput && InputSettings.CompareFeatureFlag(a, b, "USE_OPTIMIZED_CONTROLS") && InputSettings.CompareFeatureFlag(a, b, "USE_READ_VALUE_CACHING") && InputSettings.CompareFeatureFlag(a, b, "PARANOID_READ_VALUE_CACHING_CHECKS") && InputSettings.CompareFeatureFlag(a, b, "DISABLE_UNITY_REMOTE_SUPPORT") && InputSettings.CompareFeatureFlag(a, b, "RUN_PLAYER_UPDATES_IN_EDIT_MODE") && InputSettings.CompareFeatureFlag(a, b, "USE_IMGUI_EDITOR_FOR_ASSETS")));
		}

		[Tooltip("Determine which type of devices are used by the application. By default, this is empty meaning that all devices recognized by Unity will be used. Restricting the set of supported devices will make only those devices appear in the input system.")]
		[SerializeField]
		private string[] m_SupportedDevices;

		[Tooltip("Determine when Unity processes events. By default, accumulated input events are flushed out before each fixed update and before each dynamic update. This setting can be used to restrict event processing to only where the application needs it.")]
		[SerializeField]
		private InputSettings.UpdateMode m_UpdateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;

		[SerializeField]
		private InputSettings.ScrollDeltaBehavior m_ScrollDeltaBehavior;

		[SerializeField]
		private int m_MaxEventBytesPerUpdate = 5242880;

		[SerializeField]
		private int m_MaxQueuedEventsPerUpdate = 1000;

		[SerializeField]
		private bool m_CompensateForScreenOrientation = true;

		[SerializeField]
		private InputSettings.BackgroundBehavior m_BackgroundBehavior;

		[SerializeField]
		private InputSettings.EditorInputBehaviorInPlayMode m_EditorInputBehaviorInPlayMode;

		[SerializeField]
		private InputSettings.InputActionPropertyDrawerMode m_InputActionPropertyDrawerMode;

		[SerializeField]
		private float m_DefaultDeadzoneMin = 0.125f;

		[SerializeField]
		private float m_DefaultDeadzoneMax = 0.925f;

		[Min(0.0001f)]
		[SerializeField]
		private float m_DefaultButtonPressPoint = 0.5f;

		[SerializeField]
		private float m_ButtonReleaseThreshold = 0.75f;

		[SerializeField]
		private float m_DefaultTapTime = 0.2f;

		[SerializeField]
		private float m_DefaultSlowTapTime = 0.5f;

		[SerializeField]
		private float m_DefaultHoldTime = 0.4f;

		[SerializeField]
		private float m_TapRadius = 5f;

		[SerializeField]
		private float m_MultiTapDelayTime = 0.75f;

		[SerializeField]
		private bool m_DisableRedundantEventsMerging;

		[SerializeField]
		private bool m_ShortcutKeysConsumeInputs;

		[NonSerialized]
		internal HashSet<string> m_FeatureFlags;

		internal const int s_OldUnsupportedFixedAndDynamicUpdateSetting = 0;

		public enum UpdateMode
		{
			ProcessEventsInDynamicUpdate = 1,
			ProcessEventsInFixedUpdate,
			ProcessEventsManually
		}

		public enum ScrollDeltaBehavior
		{
			UniformAcrossAllPlatforms,
			KeepPlatformSpecificInputRange
		}

		public enum BackgroundBehavior
		{
			ResetAndDisableNonBackgroundDevices,
			ResetAndDisableAllDevices,
			IgnoreFocus
		}

		public enum EditorInputBehaviorInPlayMode
		{
			PointersAndKeyboardsRespectGameViewFocus,
			AllDevicesRespectGameViewFocus,
			AllDeviceInputAlwaysGoesToGameView
		}

		public enum InputActionPropertyDrawerMode
		{
			Compact,
			MultilineEffective,
			MultilineBoth
		}
	}
}
