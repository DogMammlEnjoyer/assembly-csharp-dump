using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[Serializable]
	public abstract class XRTargetEvaluator : IDisposable
	{
		internal static bool IsInstanceType(Type evaluatorType)
		{
			return evaluatorType != null && !evaluatorType.IsInterface && !evaluatorType.IsAbstract && !evaluatorType.IsGenericType && typeof(XRTargetEvaluator).IsAssignableFrom(evaluatorType);
		}

		internal static XRTargetEvaluator CreateInstance(Type evaluatorType, XRTargetFilter filter)
		{
			if (XRTargetEvaluator.IsInstanceType(evaluatorType))
			{
				XRTargetEvaluator xrtargetEvaluator = Activator.CreateInstance(evaluatorType) as XRTargetEvaluator;
				if (xrtargetEvaluator != null)
				{
					xrtargetEvaluator.m_Filter = filter;
					xrtargetEvaluator.m_Weight = AnimationCurve.Linear(0f, 0f, 1f, 1f);
					return xrtargetEvaluator;
				}
			}
			return null;
		}

		public XRTargetFilter filter
		{
			get
			{
				return this.m_Filter;
			}
		}

		public bool enabled
		{
			get
			{
				return this.m_Enabled;
			}
			set
			{
				if (this.m_Enabled == value || this.disposed)
				{
					return;
				}
				if (this.m_Filter.isProcessing && !value)
				{
					throw new InvalidOperationException(string.Concat(new string[]
					{
						"Cannot disable an evaluator ",
						base.GetType().Name,
						" while its filter ",
						this.m_Filter.name,
						" is processing."
					}));
				}
				this.m_Enabled = value;
				if (!this.m_IsAwake || !this.m_Filter.isActiveAndEnabled)
				{
					return;
				}
				if (value)
				{
					this.EnableInternal();
					return;
				}
				this.DisableInternal();
			}
		}

		public AnimationCurve weight
		{
			get
			{
				return this.m_Weight;
			}
			set
			{
				this.m_Weight = value;
			}
		}

		public bool disposed
		{
			get
			{
				return this.m_Filter == null;
			}
		}

		private void RegisterHandlers()
		{
			if (this.m_IsRegistered || this.disposed)
			{
				return;
			}
			this.m_IsRegistered = true;
			this.m_Filter.RegisterEvaluatorHandlers(this);
		}

		private void UnregisterHandlers()
		{
			if (!this.m_IsRegistered || this.disposed)
			{
				return;
			}
			this.m_IsRegistered = false;
			this.m_Filter.UnregisterEvaluatorHandlers(this);
		}

		internal void AwakeInternal()
		{
			if (this.m_IsAwake || this.disposed)
			{
				return;
			}
			this.m_IsAwake = true;
			this.Awake();
			this.RegisterHandlers();
		}

		internal void DisposeInternal()
		{
			if (!this.m_IsAwake)
			{
				return;
			}
			this.m_IsAwake = false;
			this.UnregisterHandlers();
			this.OnDispose();
			this.m_Filter = null;
		}

		internal void EnableInternal()
		{
			if (this.m_IsEnabled || this.disposed)
			{
				return;
			}
			this.m_IsEnabled = true;
			this.OnEnable();
		}

		internal void DisableInternal()
		{
			if (!this.m_IsEnabled)
			{
				return;
			}
			this.m_IsEnabled = false;
			this.OnDisable();
		}

		protected virtual void Awake()
		{
		}

		protected virtual void OnDispose()
		{
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		public virtual void Reset()
		{
		}

		public float GetWeightedScore(IXRInteractor interactor, IXRInteractable target)
		{
			return this.m_Weight.Evaluate(this.CalculateNormalizedScore(interactor, target));
		}

		protected abstract float CalculateNormalizedScore(IXRInteractor interactor, IXRInteractable target);

		public void Dispose()
		{
			if (this.m_Filter != null)
			{
				this.m_Filter.RemoveEvaluator(this);
			}
		}

		[HideInInspector]
		[SerializeField]
		private XRTargetFilter m_Filter;

		[HideInInspector]
		[SerializeField]
		[XRTargetEvaluatorEnabled]
		private bool m_Enabled = true;

		[Tooltip("The weight curve of this evaluator. Use this curve to configure the returned score.\n\nThe x-axis is the normalized score (calculated in CalculateNormalizedScore) and the y-axis is the returned score of this evaluator.")]
		[SerializeField]
		private AnimationCurve m_Weight;

		private bool m_IsAwake;

		private bool m_IsEnabled;

		private bool m_IsRegistered;
	}
}
