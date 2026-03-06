using System;

namespace Pathfinding
{
	[Serializable]
	public abstract class MonoModifier : VersionedMonoBehaviour, IPathModifier
	{
		protected virtual void OnEnable()
		{
			this.seeker = base.GetComponent<Seeker>();
			if (this.seeker != null)
			{
				this.seeker.RegisterModifier(this);
			}
		}

		protected virtual void OnDisable()
		{
			if (this.seeker != null)
			{
				this.seeker.DeregisterModifier(this);
			}
		}

		public abstract int Order { get; }

		public virtual void PreProcess(Path path)
		{
		}

		public abstract void Apply(Path path);

		[NonSerialized]
		public Seeker seeker;
	}
}
