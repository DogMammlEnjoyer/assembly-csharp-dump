using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ShowIfAttribute : Attribute
	{
		[Obsolete("Use the Condition member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		public ShowIfAttribute(string condition, bool animate = true)
		{
			this.Condition = condition;
			this.Animate = animate;
		}

		public ShowIfAttribute(string condition, object optionalValue, bool animate = true)
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
