using System;

namespace g3
{
	public struct VertexConstraint
	{
		public VertexConstraint(bool isFixed, int setID = -1)
		{
			this.Fixed = isFixed;
			this.FixedSetID = setID;
			this.Target = null;
		}

		public VertexConstraint(IProjectionTarget target)
		{
			this.Fixed = false;
			this.FixedSetID = -1;
			this.Target = target;
		}

		public bool Fixed;

		public int FixedSetID;

		public IProjectionTarget Target;

		public const int InvalidSetID = -1;

		public static readonly VertexConstraint Unconstrained = new VertexConstraint
		{
			Fixed = false,
			FixedSetID = -1,
			Target = null
		};

		public static readonly VertexConstraint Pinned = new VertexConstraint
		{
			Fixed = true,
			FixedSetID = -1,
			Target = null
		};
	}
}
