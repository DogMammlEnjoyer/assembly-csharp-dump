using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[Conditional("UNITY_EDITOR")]
	[DontApplyToListElements]
	[IncludeMyAttributes]
	[HideInTables]
	public class OnInspectorInitAttribute : ShowInInspectorAttribute
	{
		public OnInspectorInitAttribute()
		{
		}

		public OnInspectorInitAttribute(string action)
		{
			this.Action = action;
		}

		public string Action;
	}
}
