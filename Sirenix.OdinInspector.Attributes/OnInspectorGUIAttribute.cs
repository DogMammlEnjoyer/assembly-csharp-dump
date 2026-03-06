using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class OnInspectorGUIAttribute : ShowInInspectorAttribute
	{
		public OnInspectorGUIAttribute()
		{
		}

		public OnInspectorGUIAttribute(string action, bool append = true)
		{
			if (append)
			{
				this.Append = action;
				return;
			}
			this.Prepend = action;
		}

		public OnInspectorGUIAttribute(string prepend, string append)
		{
			this.Prepend = prepend;
			this.Append = append;
		}

		public string Prepend;

		public string Append;

		[Obsolete("Use the Prepend member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string PrependMethodName;

		[Obsolete("Use the Append member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string AppendMethodName;
	}
}
