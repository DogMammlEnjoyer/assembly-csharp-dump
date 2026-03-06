using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public abstract class BaseAsyncAffordanceStateReceiver<T> : BaseAffordanceStateReceiver<T>, IAsyncAffordanceStateReceiver, IAffordanceStateReceiver where T : struct, IEquatable<T>
	{
		protected virtual void OnDestroy()
		{
			this.m_LastJobHandle.Complete();
			if (this.m_JobOutputStore.IsCreated)
			{
				this.m_JobOutputStore.Dispose();
			}
			if (this.m_NativeCurve.isCreated)
			{
				this.m_NativeCurve.Dispose();
			}
		}

		public JobHandle HandleTween(float tweenTarget)
		{
			this.CaptureInitialValue();
			AffordanceStateData value = base.currentAffordanceStateData.Value;
			AffordanceThemeData<T> affordanceThemeDataForIndex = base.affordanceTheme.GetAffordanceThemeDataForIndex(value.stateIndex);
			if (affordanceThemeDataForIndex == null)
			{
				string nameForIndex = AffordanceStateShortcuts.GetNameForIndex(value.stateIndex);
				XRLoggingUtils.LogError(string.Format("Missing theme data for affordance state index {0} \"{1}\" with {2}.", value.stateIndex, nameForIndex, this), this);
				return default(JobHandle);
			}
			T newValue = affordanceThemeDataForIndex.animationStateStartValue;
			T newValue2 = affordanceThemeDataForIndex.animationStateEndValue;
			if (base.replaceIdleStateValueWithInitialValue && value.stateIndex == 1)
			{
				newValue = base.initialValue;
				newValue2 = base.initialValue;
			}
			TweenJobData<T> tweenJobData = new TweenJobData<T>
			{
				initialValue = base.initialValue,
				stateOriginValue = this.ProcessTargetAffordanceValue(newValue),
				stateTargetValue = this.ProcessTargetAffordanceValue(newValue2),
				stateTransitionIncrement = value.stateTransitionIncrement,
				nativeCurve = this.m_NativeCurve,
				tweenStartValue = base.currentAffordanceValue.Value,
				tweenAmount = tweenTarget,
				outputData = this.GetJobOutputStore()
			};
			this.m_LastJobHandle = this.ScheduleTweenJob(ref tweenJobData);
			return this.m_LastJobHandle;
		}

		public void UpdateStateFromCompletedJob()
		{
			if (!this.m_OutputInitialized)
			{
				return;
			}
			this.ConsumeAffordance(this.GetJobOutputStore()[0]);
		}

		protected abstract JobHandle ScheduleTweenJob(ref TweenJobData<T> jobData);

		protected override void OnAffordanceThemeChanged(BaseAffordanceTheme<T> newValue)
		{
			base.OnAffordanceThemeChanged(newValue);
			this.m_NativeCurve.Update(newValue.animationCurve, 1024);
		}

		private NativeArray<T> GetJobOutputStore()
		{
			if (!this.m_OutputInitialized && base.enabled)
			{
				this.m_JobOutputStore = new NativeArray<T>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.m_OutputInitialized = true;
			}
			return this.m_JobOutputStore;
		}

		private NativeArray<T> m_JobOutputStore;

		private NativeCurve m_NativeCurve;

		private JobHandle m_LastJobHandle;

		private bool m_OutputInitialized;
	}
}
