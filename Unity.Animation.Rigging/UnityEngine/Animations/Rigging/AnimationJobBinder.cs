using System;
using UnityEngine.Playables;

namespace UnityEngine.Animations.Rigging
{
	public abstract class AnimationJobBinder<TJob, TData> : IAnimationJobBinder where TJob : struct, IAnimationJob where TData : struct, IAnimationJobData
	{
		public abstract TJob Create(Animator animator, ref TData data, Component component);

		public abstract void Destroy(TJob job);

		public virtual void Update(TJob job, ref TData data)
		{
		}

		IAnimationJob IAnimationJobBinder.Create(Animator animator, IAnimationJobData data, Component component)
		{
			TData tdata = (TData)((object)data);
			return this.Create(animator, ref tdata, component);
		}

		void IAnimationJobBinder.Destroy(IAnimationJob job)
		{
			this.Destroy((TJob)((object)job));
		}

		void IAnimationJobBinder.Update(IAnimationJob job, IAnimationJobData data)
		{
			TData tdata = (TData)((object)data);
			this.Update((TJob)((object)job), ref tdata);
		}

		AnimationScriptPlayable IAnimationJobBinder.CreatePlayable(PlayableGraph graph, IAnimationJob job)
		{
			return AnimationScriptPlayable.Create<TJob>(graph, (TJob)((object)job), 0);
		}
	}
}
