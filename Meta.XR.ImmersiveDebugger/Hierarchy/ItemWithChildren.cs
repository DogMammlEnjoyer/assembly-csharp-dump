using System;
using System.Collections.Generic;

namespace Meta.XR.ImmersiveDebugger.Hierarchy
{
	internal abstract class ItemWithChildren<TargetType, ChildType, ChildTargetType> : Item<TargetType> where ChildType : Item<ChildTargetType>, new()
	{
		protected abstract bool CompareChildren(ChildTargetType lhs, ChildTargetType rhs);

		protected abstract ChildTargetType[] FetchExpectedChildren();

		public override int ComputeNumberOfChildren()
		{
			return this.FetchExpectedChildren().Length;
		}

		private void MarkChildrenDirty()
		{
			foreach (ChildType childType in this._children)
			{
				childType.Dirty = true;
			}
		}

		private void ClearDirtyChildren()
		{
			foreach (ChildType childType in this._children)
			{
				if (childType.Dirty)
				{
					childType.Clear();
				}
			}
		}

		private ChildType GetChild(ChildTargetType target)
		{
			foreach (ChildType childType in this._children)
			{
				if (this.CompareChildren(childType.TypedOwner, target))
				{
					return childType;
				}
			}
			return default(ChildType);
		}

		public override void ClearChildren()
		{
			foreach (ChildType childType in this._children)
			{
				childType.Clear();
			}
			this._children.Clear();
			base.ClearChildren();
		}

		public override void BuildChildren()
		{
			if (!this.Valid)
			{
				base.Clear();
				return;
			}
			this.MarkChildrenDirty();
			this.BuildChildrenInternal();
			this.ClearDirtyChildren();
		}

		private void BuildChildrenInternal()
		{
			foreach (ChildTargetType childTargetType in this.FetchExpectedChildren())
			{
				ChildType childType = this.GetChild(childTargetType);
				if (childType != null)
				{
					childType.Dirty = false;
				}
				else
				{
					childType = Activator.CreateInstance<ChildType>();
					childType.Dirty = false;
					childType.SetOwner(childTargetType);
					this._children.Add(childType);
					childType.Register(this);
				}
			}
		}

		public override bool ComputeNeedsRefresh()
		{
			foreach (ChildTargetType target in this.FetchExpectedChildren())
			{
				if (this.GetChild(target) == null)
				{
					return true;
				}
			}
			return false;
		}

		private readonly List<ChildType> _children = new List<ChildType>();
	}
}
