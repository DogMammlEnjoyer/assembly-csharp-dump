using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public abstract class PropertyGroupAttribute : Attribute
	{
		public PropertyGroupAttribute(string groupId, float order)
		{
			this.GroupID = groupId;
			this.Order = order;
			int num = groupId.LastIndexOf('/');
			this.GroupName = ((num >= 0 && num < groupId.Length) ? groupId.Substring(num + 1) : groupId);
		}

		public PropertyGroupAttribute(string groupId) : this(groupId, 0f)
		{
		}

		public PropertyGroupAttribute Combine(PropertyGroupAttribute other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (other.GetType() != base.GetType())
			{
				throw new ArgumentException("Attributes to combine are not of the same type.");
			}
			if (other.GroupID != this.GroupID)
			{
				throw new ArgumentException("PropertyGroupAttributes to combine must have the same group id.");
			}
			if (this.Order == 0f)
			{
				this.Order = other.Order;
			}
			else if (other.Order != 0f)
			{
				this.Order = Math.Min(this.Order, other.Order);
			}
			this.HideWhenChildrenAreInvisible &= other.HideWhenChildrenAreInvisible;
			if (this.VisibleIf == null)
			{
				this.VisibleIf = other.VisibleIf;
			}
			this.AnimateVisibility &= other.AnimateVisibility;
			this.CombineValuesWith(other);
			return this;
		}

		protected virtual void CombineValuesWith(PropertyGroupAttribute other)
		{
		}

		public string GroupID;

		public string GroupName;

		public float Order;

		public bool HideWhenChildrenAreInvisible = true;

		public string VisibleIf;

		public bool AnimateVisibility = true;
	}
}
