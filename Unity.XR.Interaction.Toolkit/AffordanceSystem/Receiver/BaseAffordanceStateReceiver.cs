using System;
using System.Text;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public abstract class BaseAffordanceStateReceiver<T> : MonoBehaviour, IAffordanceStateReceiver<T>, IAffordanceStateReceiver where T : struct, IEquatable<T>
	{
		public BaseAffordanceStateProvider affordanceStateProvider
		{
			get
			{
				return this.m_AffordanceStateProvider;
			}
			set
			{
				this.m_AffordanceStateProvider = value;
			}
		}

		public bool replaceIdleStateValueWithInitialValue
		{
			get
			{
				return this.m_ReplaceIdleStateValueWithInitialValue;
			}
			set
			{
				this.m_ReplaceIdleStateValueWithInitialValue = value;
			}
		}

		public BaseAffordanceTheme<T> affordanceTheme
		{
			get
			{
				return this.m_AffordanceTheme;
			}
			set
			{
				this.m_AffordanceTheme = value;
				this.OnAffordanceThemeChanged(value);
			}
		}

		protected abstract BaseAffordanceTheme<T> defaultAffordanceTheme { get; }

		protected abstract BindableVariable<T> affordanceValue { get; }

		public IReadOnlyBindableVariable<T> currentAffordanceValue
		{
			get
			{
				return this.affordanceValue;
			}
		}

		public IReadOnlyBindableVariable<AffordanceStateData> currentAffordanceStateData
		{
			get
			{
				return this.m_AffordanceStateData;
			}
		}

		protected T initialValue { get; set; }

		protected bool initialValueCaptured { get; set; }

		protected virtual void Awake()
		{
			if (this.m_AffordanceStateProvider == null)
			{
				this.m_AffordanceStateProvider = base.GetComponentInParent<BaseAffordanceStateProvider>();
			}
		}

		protected virtual void OnEnable()
		{
			this.Initialize();
		}

		protected virtual void OnDisable()
		{
			if (this.m_AffordanceStateProvider != null)
			{
				this.m_AffordanceStateProvider.UnregisterAffordanceReceiver(this);
			}
		}

		protected virtual void Start()
		{
			this.Initialize();
			if (this.m_AffordanceStateProvider == null)
			{
				XRLoggingUtils.LogError(string.Format("Missing Affordance State Provider reference. Please set one on {0}.", this), this);
			}
		}

		private void Initialize()
		{
			if (!this.m_Initialized)
			{
				if (this.m_AffordanceStateProvider == null)
				{
					return;
				}
				if (this.affordanceTheme == null)
				{
					if (this.defaultAffordanceTheme == null)
					{
						return;
					}
					this.defaultAffordanceTheme.ValidateTheme();
					BaseAffordanceTheme<T> baseAffordanceTheme = this.GenerateNewAffordanceThemeInstance();
					baseAffordanceTheme.CopyFrom(this.defaultAffordanceTheme);
					this.affordanceTheme = baseAffordanceTheme;
				}
				this.m_Initialized = true;
			}
			this.m_AffordanceStateProvider.RegisterAffordanceReceiver(this);
		}

		protected abstract BaseAffordanceTheme<T> GenerateNewAffordanceThemeInstance();

		protected virtual void OnAffordanceThemeChanged(BaseAffordanceTheme<T> newValue)
		{
			this.LogIfMissingAffordanceStates(newValue);
		}

		private void LogIfMissingAffordanceStates(BaseAffordanceTheme<T> theme)
		{
			if (theme.GetAffordanceThemeDataForIndex(AffordanceStateShortcuts.stateCount - 1) == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				for (byte b = 0; b < AffordanceStateShortcuts.stateCount; b += 1)
				{
					AffordanceThemeData<T> affordanceThemeDataForIndex = theme.GetAffordanceThemeDataForIndex(b);
					stringBuilder.Append(string.Format("Expected: {0} \"{1}\",\tActual: ", b, AffordanceStateShortcuts.GetNameForIndex(b)));
					stringBuilder.AppendLine((affordanceThemeDataForIndex != null) ? string.Format("{0} \"{1}\"", b, affordanceThemeDataForIndex.stateName) : "<b>(None)</b>");
					if (affordanceThemeDataForIndex != null)
					{
						num++;
					}
				}
				Debug.LogWarning("Affordance Theme on affordance receiver is missing a potential affordance state. Expected states:" + string.Format("\nExpected Count: {0}, Actual Count: {1}", AffordanceStateShortcuts.stateCount, num) + string.Format("\n{0}", stringBuilder), this);
			}
		}

		public virtual void OnAffordanceStateUpdated(AffordanceStateData previousState, AffordanceStateData newState)
		{
			this.m_AffordanceStateData.Value = newState;
		}

		protected virtual void ConsumeAffordance(T newValue)
		{
			this.affordanceValue.Value = newValue;
			this.OnAffordanceValueUpdated(newValue);
		}

		protected abstract void OnAffordanceValueUpdated(T newValue);

		protected virtual void CaptureInitialValue()
		{
			if (this.initialValueCaptured)
			{
				return;
			}
			this.initialValue = this.GetCurrentValueForCapture();
			this.affordanceValue.Value = this.initialValue;
			this.initialValueCaptured = true;
		}

		protected virtual T GetCurrentValueForCapture()
		{
			return this.affordanceValue.Value;
		}

		protected virtual T ProcessTargetAffordanceValue(T newValue)
		{
			return newValue;
		}

		[SerializeField]
		[Tooltip("Affordance state provider component to subscribe to.")]
		private BaseAffordanceStateProvider m_AffordanceStateProvider;

		[SerializeField]
		[Tooltip("If true, the initial captured state value for the receiver will replace the idle value in the affordance theme.")]
		private bool m_ReplaceIdleStateValueWithInitialValue;

		private BaseAffordanceTheme<T> m_AffordanceTheme;

		private readonly BindableVariable<AffordanceStateData> m_AffordanceStateData = new BindableVariable<AffordanceStateData>(default(AffordanceStateData), true, null, false);

		private bool m_Initialized;
	}
}
