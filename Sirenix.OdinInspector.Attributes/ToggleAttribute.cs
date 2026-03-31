using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ToggleAttribute : Attribute
	{
		public ToggleAttribute(string toggleMemberName)
		{
			this.ToggleMemberName = toggleMemberName;
			this.CollapseOthersOnExpand = true;
		}

		public string ToggleMemberName;

		[LabelWidth(160f)]
		public bool CollapseOthersOnExpand;
	}
}
