using System;

namespace UnityEngine.Animations.Rigging
{
	public class OverrideRigConstraint<TConstraint, TJob, TData, TBinder> : IRigConstraint where TConstraint : MonoBehaviour, IRigConstraint where TJob : struct, IWeightedAnimationJob where TData : struct, IAnimationJobData where TBinder : AnimationJobBinder<TJob, TData>, new()
	{
		public OverrideRigConstraint(TConstraint baseConstraint)
		{
			this.m_Constraint = baseConstraint;
		}

		public IAnimationJob CreateJob(Animator animator)
		{
			TJob tjob = (TJob)((object)OverrideRigConstraint<TConstraint, TJob, TData, TBinder>.s_Binder.Create(animator, this.m_Constraint.data, this.m_Constraint));
			tjob.jobWeight = FloatProperty.BindCustom(animator, ConstraintsUtils.ConstructCustomPropertyName(this.m_Constraint, ConstraintProperties.s_Weight));
			return tjob;
		}

		public void DestroyJob(IAnimationJob job)
		{
			OverrideRigConstraint<TConstraint, TJob, TData, TBinder>.s_Binder.Destroy((TJob)((object)job));
		}

		public void UpdateJob(IAnimationJob job)
		{
			OverrideRigConstraint<TConstraint, TJob, TData, TBinder>.s_Binder.Update(job, this.m_Constraint.data);
		}

		public bool IsValid()
		{
			return this.m_Constraint.IsValid();
		}

		IAnimationJobBinder IRigConstraint.binder
		{
			get
			{
				return OverrideRigConstraint<TConstraint, TJob, TData, TBinder>.s_Binder;
			}
		}

		IAnimationJobData IRigConstraint.data
		{
			get
			{
				return this.m_Constraint.data;
			}
		}

		Component IRigConstraint.component
		{
			get
			{
				return this.m_Constraint.component;
			}
		}

		public float weight
		{
			get
			{
				return this.m_Constraint.weight;
			}
			set
			{
				this.m_Constraint.weight = value;
			}
		}

		[SerializeField]
		protected TConstraint m_Constraint;

		private static readonly TBinder s_Binder = Activator.CreateInstance<TBinder>();
	}
}
