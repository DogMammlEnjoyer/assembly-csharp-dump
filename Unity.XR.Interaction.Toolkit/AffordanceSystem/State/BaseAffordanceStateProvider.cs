using System;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.XR.CoreUtils.Bindings;
using Unity.XR.CoreUtils.Bindings.Variables;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public abstract class BaseAffordanceStateProvider : MonoBehaviour
	{
		public float transitionDuration
		{
			get
			{
				return this.m_TransitionDuration;
			}
			set
			{
				this.m_TransitionDuration = value;
				this.RefreshTransitionDuration();
			}
		}

		public bool isCurrentlyTransitioning
		{
			get
			{
				return !this.m_CompletingTweens || this.m_ScheduledJobs.Count > 0;
			}
		}

		public IReadOnlyBindableVariable<AffordanceStateData> currentAffordanceStateData
		{
			get
			{
				return this.m_AffordanceStateData;
			}
		}

		protected virtual void OnValidate()
		{
			this.RefreshTransitionDuration();
		}

		protected virtual void OnEnable()
		{
			this.RefreshTransitionDuration();
			this.BindToProviders();
		}

		protected virtual void OnDisable()
		{
			this.ClearBindings();
		}

		protected virtual void Update()
		{
			if (this.m_IsFirstFrame)
			{
				this.OnAffordanceStateUpdated(this.m_AffordanceStateData.Value);
				this.DoTween(1f);
				this.m_IsFirstFrame = false;
				return;
			}
			this.DoTween((this.m_InterpolationSpeed > 0f) ? (Time.deltaTime * this.m_InterpolationSpeed) : 1f);
		}

		protected virtual void BindToProviders()
		{
			this.ClearBindings();
			this.m_IsFirstFrame = true;
			this.AddBinding(this.m_AffordanceStateData.SubscribeAndUpdate(new Action<AffordanceStateData>(this.OnAffordanceStateUpdated)));
		}

		protected virtual void ClearBindings()
		{
			this.m_BindingsGroup.Clear();
		}

		protected void AddBinding(IEventBinding binding)
		{
			this.m_BindingsGroup.AddBinding(binding);
		}

		public void UpdateAffordanceState(AffordanceStateData newAffordanceStateData)
		{
			this.m_AffordanceStateDataBeforeSet = this.m_AffordanceStateData.Value;
			this.m_AffordanceStateData.Value = newAffordanceStateData;
		}

		private void OnAffordanceStateUpdated(AffordanceStateData newAffordanceStateData)
		{
			this.m_PreviousAffordanceStateData = this.m_AffordanceStateDataBeforeSet;
			for (int i = 0; i < this.m_AsyncAffordanceReceivers.Count; i++)
			{
				this.m_AsyncAffordanceReceivers[i].OnAffordanceStateUpdated(this.m_PreviousAffordanceStateData, newAffordanceStateData);
			}
			for (int j = 0; j < this.m_SynchronousAffordanceReceivers.Count; j++)
			{
				this.m_SynchronousAffordanceReceivers[j].OnAffordanceStateUpdated(this.m_PreviousAffordanceStateData, newAffordanceStateData);
			}
			this.m_TimeSinceLastStateUpdate = 0f;
			this.m_CompletingTweens = false;
		}

		public bool RegisterAffordanceReceiver(IAffordanceStateReceiver receiver)
		{
			IAsyncAffordanceStateReceiver asyncAffordanceStateReceiver = receiver as IAsyncAffordanceStateReceiver;
			if (asyncAffordanceStateReceiver != null)
			{
				return this.RegisterAffordanceReceiver(asyncAffordanceStateReceiver);
			}
			ISynchronousAffordanceStateReceiver synchronousAffordanceStateReceiver = receiver as ISynchronousAffordanceStateReceiver;
			if (synchronousAffordanceStateReceiver != null)
			{
				return this.RegisterAffordanceReceiver(synchronousAffordanceStateReceiver);
			}
			if (receiver != null)
			{
				Debug.LogError("Unhandled type of IAffordanceStateReceiver: " + receiver.GetType().Name, this);
			}
			return false;
		}

		private bool RegisterAffordanceReceiver(IAsyncAffordanceStateReceiver receiver)
		{
			return this.m_AsyncAffordanceReceivers.Add(receiver);
		}

		private bool RegisterAffordanceReceiver(ISynchronousAffordanceStateReceiver receiver)
		{
			return this.m_SynchronousAffordanceReceivers.Add(receiver);
		}

		public bool UnregisterAffordanceReceiver(IAffordanceStateReceiver receiver)
		{
			IAsyncAffordanceStateReceiver asyncAffordanceStateReceiver = receiver as IAsyncAffordanceStateReceiver;
			if (asyncAffordanceStateReceiver != null)
			{
				return this.UnregisterAffordanceReceiver(asyncAffordanceStateReceiver);
			}
			ISynchronousAffordanceStateReceiver synchronousAffordanceStateReceiver = receiver as ISynchronousAffordanceStateReceiver;
			if (synchronousAffordanceStateReceiver != null)
			{
				return this.UnregisterAffordanceReceiver(synchronousAffordanceStateReceiver);
			}
			if (receiver != null)
			{
				Debug.LogError("Unhandled type of IAffordanceStateReceiver: " + receiver.GetType().Name, this);
			}
			return false;
		}

		private bool UnregisterAffordanceReceiver(IAsyncAffordanceStateReceiver receiver)
		{
			this.CompleteJobs();
			return this.m_AsyncAffordanceReceivers.Remove(receiver);
		}

		private bool UnregisterAffordanceReceiver(ISynchronousAffordanceStateReceiver receiver)
		{
			return this.m_SynchronousAffordanceReceivers.Remove(receiver);
		}

		private bool CompleteJobs()
		{
			for (int i = 0; i < this.m_ScheduledJobs.Count; i++)
			{
				this.m_ScheduledJobs[i].Complete();
			}
			bool result = this.m_ScheduledJobs.Count > 0;
			this.m_ScheduledJobs.Clear();
			return result;
		}

		private void DoTween(float tweenTarget)
		{
			if (this.CompleteJobs())
			{
				for (int i = 0; i < this.m_AsyncAffordanceReceivers.Count; i++)
				{
					this.m_AsyncAffordanceReceivers[i].UpdateStateFromCompletedJob();
				}
			}
			float num = tweenTarget;
			if (this.m_TimeSinceLastStateUpdate > this.m_MaxTransitionDuration || num > 0.99f)
			{
				if (this.m_CompletingTweens)
				{
					return;
				}
				num = 1f;
				this.m_CompletingTweens = true;
			}
			for (int j = 0; j < this.m_AsyncAffordanceReceivers.Count; j++)
			{
				this.m_ScheduledJobs.Add(this.m_AsyncAffordanceReceivers[j].HandleTween(num));
			}
			for (int k = 0; k < this.m_SynchronousAffordanceReceivers.Count; k++)
			{
				this.m_SynchronousAffordanceReceivers[k].HandleTween(num);
			}
			this.m_TimeSinceLastStateUpdate += Time.deltaTime;
		}

		private void RefreshTransitionDuration()
		{
			this.m_InterpolationSpeed = ((this.m_TransitionDuration > 0f) ? (1f / this.m_TransitionDuration) : 0f);
			this.m_MaxTransitionDuration = this.m_TransitionDuration * 4f;
		}

		[SerializeField]
		[Range(0f, 5f)]
		[Tooltip("Duration of transition in seconds. 0 means no smoothing.")]
		private float m_TransitionDuration = 0.125f;

		private readonly BindableVariable<AffordanceStateData> m_AffordanceStateData = new BindableVariable<AffordanceStateData>(default(AffordanceStateData), true, null, false);

		private AffordanceStateData m_AffordanceStateDataBeforeSet;

		private AffordanceStateData m_PreviousAffordanceStateData;

		private readonly HashSetList<IAsyncAffordanceStateReceiver> m_AsyncAffordanceReceivers = new HashSetList<IAsyncAffordanceStateReceiver>(0);

		private readonly HashSetList<ISynchronousAffordanceStateReceiver> m_SynchronousAffordanceReceivers = new HashSetList<ISynchronousAffordanceStateReceiver>(0);

		private readonly List<JobHandle> m_ScheduledJobs = new List<JobHandle>();

		private readonly BindingsGroup m_BindingsGroup = new BindingsGroup();

		private float m_TimeSinceLastStateUpdate;

		private bool m_IsFirstFrame = true;

		private bool m_CompletingTweens;

		private float m_InterpolationSpeed = 8f;

		private float m_MaxTransitionDuration = 5f;
	}
}
