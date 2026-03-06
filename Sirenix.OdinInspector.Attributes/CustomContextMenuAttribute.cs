using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class CustomContextMenuAttribute : Attribute
	{
		[Obsolete("Use the Action member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MethodName
		{
			get
			{
				return this.Action;
			}
			set
			{
				this.Action = value;
			}
		}

		public CustomContextMenuAttribute(string menuItem, string action)
		{
			this.MenuItem = menuItem;
			this.Action = action;
		}

		public string MenuItem;

		public string Action;
	}
}
