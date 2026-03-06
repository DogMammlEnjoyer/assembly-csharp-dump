using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[DontApplyToListElements]
	[Conditional("UNITY_EDITOR")]
	public sealed class HideIfAttribute : Attribute
	{
		[Obsolete("Use the Condition member instead.", false)]
		public string MemberName
		{
			get
			{
				return this.Condition;
			}
			set
			{
				this.Condition = value;
			}
		}

		public HideIfAttribute(string condition, bool animate = true)
		{
			this.Condition = condition;
			this.Animate = animate;
		}

		public HideIfAttribute(string condition, object optionalValue, bool animate = true)
		{
			this.Condition = condition;
			this.Value = optionalValue;
			this.Animate = animate;
		}

		public string Condition;

		public object Value;

		public bool Animate;
	}
}
