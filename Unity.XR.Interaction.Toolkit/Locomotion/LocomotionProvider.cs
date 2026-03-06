using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
	[DefaultExecutionOrder(-210)]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public abstract class LocomotionProvider : MonoBehaviour
	{
		public LocomotionMediator mediator
		{
			get
			{
				return this.m_Mediator;
			}
			set
			{
				this.m_Mediator = value;
			}
		}

		public int transformationPriority
		{
			get
			{
				return this.m_TransformationPriority;
			}
			set
			{
				this.m_TransformationPriority = value;
			}
		}

		public LocomotionState locomotionState
		{
			get
			{
				if (!(this.m_Mediator != null))
				{
					return LocomotionState.Idle;
				}
				return this.m_Mediator.GetProviderLocomotionState(this);
			}
		}

		public bool isLocomotionActive
		{
			get
			{
				return this.locomotionState.IsActive();
			}
		}

		public virtual bool canStartMoving
		{
			get
			{
				return true;
			}
		}

		public event Action<LocomotionProvider, LocomotionState> locomotionStateChanged;

		public event Action<LocomotionProvider> locomotionStarted;

		public event Action<LocomotionProvider> locomotionEnded;

		public event Action<LocomotionProvider> beforeStepLocomotion;

		public event Action<LocomotionProvider> afterStepLocomotion;

		internal static List<LocomotionProvider> locomotionProviders { get; } = new List<LocomotionProvider>();

		internal static event Action<LocomotionProvider> locomotionProvidersChanged;

		protected virtual void Awake()
		{
			if (this.m_System == null)
			{
				this.m_System = base.GetComponentInParent<LocomotionSystem>();
				if (this.m_System == null)
				{
					ComponentLocatorUtility<LocomotionSystem>.TryFindComponent(out this.m_System);
				}
			}
			if (this.m_Mediator == null)
			{
				this.m_Mediator = base.GetComponentInParent<LocomotionMediator>();
				if (this.m_Mediator == null)
				{
					ComponentLocatorUtility<LocomotionMediator>.TryFindComponent(out this.m_Mediator);
				}
			}
			if (this.m_Mediator == null && this.m_System == null)
			{
				Debug.LogError("Locomotion Provider requires a Locomotion Mediator or Locomotion System (legacy) in the scene.", this);
				base.enabled = false;
			}
			LocomotionProvider.locomotionProviders.Add(this);
			Action<LocomotionProvider> action = LocomotionProvider.locomotionProvidersChanged;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		protected bool TryPrepareLocomotion()
		{
			return this.m_Mediator != null && this.m_Mediator.TryPrepareLocomotion(this);
		}

		protected bool TryStartLocomotionImmediately()
		{
			return this.m_Mediator != null && this.m_Mediator.TryStartLocomotion(this);
		}

		protected bool TryEndLocomotion()
		{
			return this.m_Mediator != null && this.m_Mediator.TryEndLocomotion(this);
		}

		protected virtual void OnLocomotionStarting()
		{
		}

		protected virtual void OnLocomotionEnding()
		{
		}

		protected virtual void OnLocomotionStateChanging(LocomotionState state)
		{
		}

		internal void OnLocomotionStateChanging(LocomotionState oldState, LocomotionState state, XRBodyTransformer transformer)
		{
			if (state == LocomotionState.Moving)
			{
				if (oldState == LocomotionState.Ended && this.m_AnyTransformationsQueued)
				{
					Debug.LogWarning("LocomotionProvider (" + base.GetType().Name + ") changed state from LocomotionState.Ended to LocomotionState.Moving before its queued transformations have been applied. The deferred OnLocomotionEnding method call and locomotionEnded event will not be invoked.", this);
				}
				this.m_ActiveBodyTransformer = transformer;
				this.Subscribe(transformer);
			}
			else if (state == LocomotionState.Ended)
			{
				this.m_ActiveBodyTransformer = null;
			}
			this.OnLocomotionStateChanging(state);
			Action<LocomotionProvider, LocomotionState> action = this.locomotionStateChanged;
			if (action != null)
			{
				action(this, state);
			}
			if (state != LocomotionState.Moving)
			{
				if (state == LocomotionState.Ended && !this.m_AnyTransformationsQueued)
				{
					this.Unsubscribe();
					this.OnLocomotionEnding();
					Action<LocomotionProvider> action2 = this.locomotionEnded;
					if (action2 == null)
					{
						return;
					}
					action2(this);
				}
				return;
			}
			this.OnLocomotionStarting();
			Action<LocomotionProvider> action3 = this.locomotionStarted;
			if (action3 == null)
			{
				return;
			}
			action3(this);
		}

		protected bool TryQueueTransformation(IXRBodyTransformation bodyTransformation)
		{
			if (!this.CanQueueTransformation())
			{
				return false;
			}
			this.m_ActiveBodyTransformer.QueueTransformation(bodyTransformation, this.m_TransformationPriority);
			this.m_AnyTransformationsQueued = true;
			return true;
		}

		protected bool TryQueueTransformation(IXRBodyTransformation bodyTransformation, int priority)
		{
			if (!this.CanQueueTransformation())
			{
				return false;
			}
			this.m_ActiveBodyTransformer.QueueTransformation(bodyTransformation, priority);
			this.m_AnyTransformationsQueued = true;
			return true;
		}

		private bool CanQueueTransformation()
		{
			if (this.m_ActiveBodyTransformer == null)
			{
				if (this.locomotionState == LocomotionState.Moving)
				{
					Debug.LogError("Cannot queue transformation because reference to active XR Body Transformer is null, even though Locomotion Provider is in Moving state. This should not happen.", this);
				}
				return false;
			}
			return true;
		}

		private void Subscribe(XRBodyTransformer transformer)
		{
			if (this.m_SubscribedTransformer == transformer)
			{
				return;
			}
			this.Unsubscribe();
			transformer.beforeApplyTransformations += this.OnBeforeApplyTransformations;
			transformer.afterApplyTransformations += this.OnAfterApplyTransformations;
			this.m_SubscribedTransformer = transformer;
		}

		private void Unsubscribe()
		{
			if (this.m_SubscribedTransformer == null)
			{
				return;
			}
			this.m_SubscribedTransformer.beforeApplyTransformations -= this.OnBeforeApplyTransformations;
			this.m_SubscribedTransformer.afterApplyTransformations -= this.OnAfterApplyTransformations;
			this.m_SubscribedTransformer = null;
		}

		private void OnBeforeApplyTransformations(XRBodyTransformer transformer)
		{
			if (this.m_AnyTransformationsQueued)
			{
				Action<LocomotionProvider> action = this.beforeStepLocomotion;
				if (action == null)
				{
					return;
				}
				action(this);
			}
		}

		private void OnAfterApplyTransformations(ApplyBodyTransformationsEventArgs args)
		{
			if (this.m_AnyTransformationsQueued)
			{
				Action<LocomotionProvider> action = this.afterStepLocomotion;
				if (action != null)
				{
					action(this);
				}
			}
			this.m_AnyTransformationsQueued = false;
			if (this.m_ActiveBodyTransformer == null)
			{
				this.Unsubscribe();
				this.OnLocomotionEnding();
				Action<LocomotionProvider> action2 = this.locomotionEnded;
				if (action2 == null)
				{
					return;
				}
				action2(this);
			}
		}

		[Obsolete("startLocomotion has been deprecated in XRI 3.0.0. Use beginLocomotion instead. (UnityUpgradable) -> beginLocomotion", true)]
		public event Action<LocomotionSystem> startLocomotion;

		[Obsolete("LocomotionSystem is deprecated in XRI 3.0.0 and will be removed in a future release. Use mediator instead.", false)]
		public LocomotionSystem system
		{
			get
			{
				return this.m_System;
			}
			set
			{
				this.m_System = value;
			}
		}

		[Obsolete("locomotionPhase is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionState instead.", false)]
		public LocomotionPhase locomotionPhase { get; protected set; }

		[Obsolete("beginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionStarted instead.", false)]
		public event Action<LocomotionSystem> beginLocomotion;

		[Obsolete("endLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Use locomotionEnded instead.", false)]
		public event Action<LocomotionSystem> endLocomotion;

		[Obsolete("CanBeginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, query isLocomotionActive to check if locomotion can start.", false)]
		protected bool CanBeginLocomotion()
		{
			return !(this.m_System == null) && !this.m_System.busy;
		}

		[Obsolete("BeginLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, call TryPrepareLocomotion when locomotion start input occurs.", false)]
		protected bool BeginLocomotion()
		{
			if (this.m_System == null)
			{
				return false;
			}
			bool flag = this.m_System.RequestExclusiveOperation(this) == RequestResult.Success;
			if (flag)
			{
				Action<LocomotionSystem> action = this.beginLocomotion;
				if (action == null)
				{
					return flag;
				}
				action(this.m_System);
			}
			return flag;
		}

		[Obsolete("EndLocomotion is deprecated in XRI 3.0.0 and will be removed in a future release. Instead, call TryEndLocomotion when locomotion end input has completed.", false)]
		protected bool EndLocomotion()
		{
			if (this.m_System == null)
			{
				return false;
			}
			bool flag = this.m_System.FinishExclusiveOperation(this) == RequestResult.Success;
			if (flag)
			{
				Action<LocomotionSystem> action = this.endLocomotion;
				if (action == null)
				{
					return flag;
				}
				action(this.m_System);
			}
			return flag;
		}

		[SerializeField]
		[Tooltip("The behavior that this provider communicates with for access to the mediator's XR Body Transformer. If one is not provided, this provider will attempt to locate one during its Awake call.")]
		private LocomotionMediator m_Mediator;

		[SerializeField]
		[Tooltip("The queue order of this provider's transformations of the XR Origin. The lower the value, the earlier the transformations are applied.")]
		private int m_TransformationPriority;

		private XRBodyTransformer m_ActiveBodyTransformer;

		private XRBodyTransformer m_SubscribedTransformer;

		private bool m_AnyTransformationsQueued;

		[Tooltip("(Deprecated) The Locomotion System that this locomotion provider communicates with for exclusive access to an XR Origin. If one is not provided, the behavior will attempt to locate one during its Awake call.")]
		[Obsolete("LocomotionSystem is deprecated in XRI 3.0.0 and will be removed in a future release. Use mediator instead.", false)]
		private LocomotionSystem m_System;
	}
}
