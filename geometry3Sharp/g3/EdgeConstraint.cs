using System;

namespace g3
{
	public struct EdgeConstraint
	{
		public EdgeConstraint(EdgeRefineFlags rflags)
		{
			this.refineFlags = rflags;
			this.Target = null;
			this.TrackingSetID = -1;
		}

		public EdgeConstraint(EdgeRefineFlags rflags, IProjectionTarget target)
		{
			this.refineFlags = rflags;
			this.Target = target;
			this.TrackingSetID = -1;
		}

		public bool CanFlip
		{
			get
			{
				return (this.refineFlags & EdgeRefineFlags.NoFlip) == EdgeRefineFlags.NoConstraint;
			}
		}

		public bool CanSplit
		{
			get
			{
				return (this.refineFlags & EdgeRefineFlags.NoSplit) == EdgeRefineFlags.NoConstraint;
			}
		}

		public bool CanCollapse
		{
			get
			{
				return (this.refineFlags & EdgeRefineFlags.NoCollapse) == EdgeRefineFlags.NoConstraint;
			}
		}

		public bool NoModifications
		{
			get
			{
				return (this.refineFlags & EdgeRefineFlags.FullyConstrained) == EdgeRefineFlags.FullyConstrained;
			}
		}

		public bool IsUnconstrained
		{
			get
			{
				return this.refineFlags == EdgeRefineFlags.NoConstraint && this.Target == null;
			}
		}

		private EdgeRefineFlags refineFlags;

		public IProjectionTarget Target;

		public int TrackingSetID;

		public static readonly EdgeConstraint Unconstrained = new EdgeConstraint
		{
			refineFlags = EdgeRefineFlags.NoConstraint,
			TrackingSetID = -1
		};

		public static readonly EdgeConstraint NoFlips = new EdgeConstraint
		{
			refineFlags = EdgeRefineFlags.NoFlip,
			TrackingSetID = -1
		};

		public static readonly EdgeConstraint FullyConstrained = new EdgeConstraint
		{
			refineFlags = EdgeRefineFlags.FullyConstrained,
			TrackingSetID = -1
		};
	}
}
