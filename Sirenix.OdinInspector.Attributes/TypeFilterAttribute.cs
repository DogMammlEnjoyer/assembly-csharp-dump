using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class TypeFilterAttribute : Attribute
	{
		[Obsolete("Use the FilterGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MemberName
		{
			get
			{
				return this.FilterGetter;
			}
			set
			{
				this.FilterGetter = value;
			}
		}

		public TypeFilterAttribute(string filterGetter)
		{
			this.FilterGetter = filterGetter;
		}

		public string FilterGetter;

		public string DropdownTitle;

		public bool DrawValueNormally;
	}
}
