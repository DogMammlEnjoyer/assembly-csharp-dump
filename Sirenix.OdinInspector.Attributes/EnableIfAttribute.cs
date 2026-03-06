using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class EnableIfAttribute : Attribute
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

		public EnableIfAttribute(string condition)
		{
			this.Condition = condition;
		}

		public EnableIfAttribute(string condition, object optionalValue)
		{
			this.Condition = condition;
			this.Value = optionalValue;
		}

		public string Condition;

		public object Value;
	}
}
