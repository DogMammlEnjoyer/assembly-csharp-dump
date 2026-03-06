using System;

namespace Pathfinding
{
	[Serializable]
	public abstract class PathModifier : IPathModifier
	{
		public abstract int Order { get; }

		public void Awake(Seeker seeker)
		{
			this.seeker = seeker;
			if (seeker != null)
			{
				seeker.RegisterModifier(this);
			}
		}

		public void OnDestroy(Seeker seeker)
		{
			if (seeker != null)
			{
				seeker.DeregisterModifier(this);
			}
		}

		public virtual void PreProcess(Path path)
		{
		}

		public abstract void Apply(Path path);

		[NonSerialized]
		public Seeker seeker;
	}
}
