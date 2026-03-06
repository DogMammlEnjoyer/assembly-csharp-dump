using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class OnValueChangedAttribute : Attribute
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

		public OnValueChangedAttribute(string action, bool includeChildren = false)
		{
			this.Action = action;
			this.IncludeChildren = includeChildren;
		}

		public string Action;

		public bool IncludeChildren;

		public bool InvokeOnUndoRedo = true;

		public bool InvokeOnInitialize;
	}
}
