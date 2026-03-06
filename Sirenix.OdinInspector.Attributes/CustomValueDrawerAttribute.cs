using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class CustomValueDrawerAttribute : Attribute
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

		public CustomValueDrawerAttribute(string action)
		{
			this.Action = action;
		}

		public string Action;
	}
}
