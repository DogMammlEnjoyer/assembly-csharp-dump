using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables
{
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public abstract class TweenableVariableAsyncBase<T> : TweenableVariableBase<T>, IDisposable where T : struct, IEquatable<T>
	{
		public new T Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				if (this.m_HasJobPending && this.m_OutputInitialized)
				{
					this.CompleteJob();
					this.m_JobOutputStore[0] = value;
				}
				base.Value = value;
			}
		}

		public void Dispose()
		{
			if (this.m_OutputInitialized)
			{
				this.UpdateStateFromCompletedJob();
				this.m_JobOutputStore.Dispose();
				this.m_OutputInitialized = false;
			}
			if (this.m_NativeCurve.isCreated)
			{
				this.m_NativeCurve.Dispose();
				this.m_CurveDirty = true;
			}
		}

		private NativeCurve GetNativeCurve()
		{
			this.RefreshCurve();
			return this.m_NativeCurve;
		}

		private void RefreshCurve()
		{
			if (this.m_CurveDirty || !this.m_NativeCurve.isCreated)
			{
				this.m_NativeCurve.Update(base.animationCurve, 1024);
				this.m_CurveDirty = false;
			}
		}

		protected override void PreprocessTween()
		{
			base.PreprocessTween();
			this.UpdateStateFromCompletedJob();
		}

		protected override void ExecuteTween(T startValue, T targetValue, float tweenAmount, bool useCurve = false)
		{
			if (tweenAmount > 0.99999f)
			{
				this.Value = targetValue;
				return;
			}
			T stateOriginValue = useCurve ? startValue : targetValue;
			float tweenAmount2 = useCurve ? 1f : tweenAmount;
			byte stateTransitionIncrement = useCurve ? ((byte)math.ceil(tweenAmount * 255f)) : byte.MaxValue;
			TweenJobData<T> tweenJobData = new TweenJobData<T>
			{
				initialValue = base.initialValue,
				stateOriginValue = stateOriginValue,
				stateTargetValue = targetValue,
				stateTransitionIncrement = stateTransitionIncrement,
				nativeCurve = this.GetNativeCurve(),
				tweenStartValue = startValue,
				tweenAmount = tweenAmount2,
				outputData = this.GetJobOutputStore()
			};
			this.m_LastJobHandle = this.ScheduleTweenJob(ref tweenJobData);
			this.m_HasJobPending = true;
		}

		private void UpdateStateFromCompletedJob()
		{
			if (!this.CompleteJob())
			{
				return;
			}
			this.Value = this.GetJobOutputStore()[0];
		}

		protected abstract JobHandle ScheduleTweenJob(ref TweenJobData<T> jobData);

		private NativeArray<T> GetJobOutputStore()
		{
			if (!this.m_OutputInitialized)
			{
				this.m_JobOutputStore = new NativeArray<T>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				this.m_OutputInitialized = true;
				DisposableManagerSingleton.RegisterDisposable(this);
			}
			return this.m_JobOutputStore;
		}

		protected override void OnAnimationCurveChanged(AnimationCurve value)
		{
			base.OnAnimationCurveChanged(value);
			this.m_CurveDirty = true;
		}

		private bool CompleteJob()
		{
			if (!this.m_OutputInitialized || !this.m_HasJobPending)
			{
				return false;
			}
			this.m_LastJobHandle.Complete();
			this.m_LastJobHandle = default(JobHandle);
			this.m_HasJobPending = false;
			return true;
		}

		private bool m_OutputInitialized;

		private NativeArray<T> m_JobOutputStore;

		private bool m_CurveDirty = true;

		private NativeCurve m_NativeCurve;

		private bool m_HasJobPending;

		private JobHandle m_LastJobHandle;
	}
}
