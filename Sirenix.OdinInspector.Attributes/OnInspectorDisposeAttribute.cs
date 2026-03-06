using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[Conditional("UNITY_EDITOR")]
	[DontApplyToListElements]
	[IncludeMyAttributes]
	[HideInTables]
	public class OnInspectorDisposeAttribute : ShowInInspectorAttribute
	{
		public OnInspectorDisposeAttribute()
		{
		}

		public OnInspectorDisposeAttribute(string action)
		{
			this.Action = action;
		}

		public string Action;
	}
}
