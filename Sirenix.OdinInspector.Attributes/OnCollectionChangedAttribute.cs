using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class OnCollectionChangedAttribute : Attribute
	{
		public OnCollectionChangedAttribute()
		{
		}

		public OnCollectionChangedAttribute(string after)
		{
			this.After = after;
		}

		public OnCollectionChangedAttribute(string before, string after)
		{
			this.Before = before;
			this.After = after;
		}

		public string Before;

		public string After;
	}
}
