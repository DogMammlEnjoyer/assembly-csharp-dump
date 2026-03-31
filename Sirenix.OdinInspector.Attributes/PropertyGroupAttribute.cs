using System;
using System.Diagnostics;
using UnityEngine;

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
			if (groupId == null)
			{
				this.GroupName = string.Empty;
				return;
			}
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

		private static bool ValidateGroupName(string value, ref string errorMessage)
		{
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}
			if (value.Contains("."))
			{
				errorMessage = "GroupName can't contain the '.' character";
				return false;
			}
			return true;
		}

		[HideInInspector]
		public string GroupID;

		[Delayed]
		[ValidateInput("ValidateGroupName", null, InfoMessageType.Error)]
		public string GroupName;

		[HideInInspector]
		public float Order;

		[LabelWidth(200f)]
		public bool HideWhenChildrenAreInvisible = true;

		[LabelWidth(200f)]
		public bool AnimateVisibility = true;

		public string VisibleIf;
	}
}
