using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class PropertyOrderAttribute : Attribute
	{
		public PropertyOrderAttribute()
		{
		}

		public PropertyOrderAttribute(float order)
		{
			this.Order = order;
		}

		public float Order;
	}
}
