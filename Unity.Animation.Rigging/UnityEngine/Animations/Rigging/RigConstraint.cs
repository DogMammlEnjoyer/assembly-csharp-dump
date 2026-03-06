using System;

namespace UnityEngine.Animations.Rigging
{
	public class RigConstraint<TJob, TData, TBinder> : MonoBehaviour, IRigConstraint where TJob : struct, IWeightedAnimationJob where TData : struct, IAnimationJobData where TBinder : AnimationJobBinder<TJob, TData>, new()
	{
		public void Reset()
		{
			this.m_Weight = 1f;
			this.m_Data.SetDefaultValues();
		}

		public bool IsValid()
		{
			return this.m_Data.IsValid();
		}

		protected virtual void OnValidate()
		{
			this.m_Weight = Mathf.Clamp01(this.m_Weight);
		}

		public ref TData data
		{
			get
			{
				return ref this.m_Data;
			}
		}

		public float weight
		{
			get
			{
				return this.m_Weight;
			}
			set
			{
				this.m_Weight = Mathf.Clamp01(value);
			}
		}

		public IAnimationJob CreateJob(Animator animator)
		{
			TJob tjob = RigConstraint<TJob, TData, TBinder>.s_Binder.Create(animator, ref this.m_Data, this);
			tjob.jobWeight = FloatProperty.BindCustom(animator, ConstraintsUtils.ConstructCustomPropertyName(this, ConstraintProperties.s_Weight));
			return tjob;
		}

		public void DestroyJob(IAnimationJob job)
		{
			RigConstraint<TJob, TData, TBinder>.s_Binder.Destroy((TJob)((object)job));
		}

		public void UpdateJob(IAnimationJob job)
		{
			RigConstraint<TJob, TData, TBinder>.s_Binder.Update((TJob)((object)job), ref this.m_Data);
		}

		IAnimationJobBinder IRigConstraint.binder
		{
			get
			{
				return RigConstraint<TJob, TData, TBinder>.s_Binder;
			}
		}

		IAnimationJobData IRigConstraint.data
		{
			get
			{
				return this.m_Data;
			}
		}

		Component IRigConstraint.component
		{
			get
			{
				return this;
			}
		}

		[SerializeField]
		[Range(0f, 1f)]
		protected float m_Weight = 1f;

		[SerializeField]
		[ExpandChildren]
		protected TData m_Data;

		private static readonly TBinder s_Binder = Activator.CreateInstance<TBinder>();
	}
}
