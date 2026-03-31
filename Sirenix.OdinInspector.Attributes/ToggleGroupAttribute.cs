using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class ToggleGroupAttribute : PropertyGroupAttribute
	{
		public ToggleGroupAttribute(string toggleMemberName, float order = 0f, string groupTitle = null) : base(toggleMemberName, order)
		{
			this.ToggleGroupTitle = groupTitle;
			this.CollapseOthersOnExpand = true;
		}

		public ToggleGroupAttribute(string toggleMemberName, string groupTitle) : this(toggleMemberName, 0f, groupTitle)
		{
		}

		[Obsolete("Use [ToggleGroup(\"toggleMemberName\", groupTitle: \"$titleStringMemberName\")] instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ToggleGroupAttribute(string toggleMemberName, float order, string groupTitle, string titleStringMemberName) : base(toggleMemberName, order)
		{
			this.ToggleGroupTitle = groupTitle;
			this.CollapseOthersOnExpand = true;
		}

		public string ToggleMemberName
		{
			get
			{
				return this.GroupName;
			}
		}

		[Obsolete("Add a $ infront of group title instead, i.e: \"$MyStringMember\".")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string TitleStringMemberName { get; set; }

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			ToggleGroupAttribute toggleGroupAttribute = other as ToggleGroupAttribute;
			if (this.ToggleGroupTitle == null)
			{
				this.ToggleGroupTitle = toggleGroupAttribute.ToggleGroupTitle;
			}
			else if (toggleGroupAttribute.ToggleGroupTitle == null)
			{
				toggleGroupAttribute.ToggleGroupTitle = this.ToggleGroupTitle;
			}
			this.CollapseOthersOnExpand = (this.CollapseOthersOnExpand && toggleGroupAttribute.CollapseOthersOnExpand);
			toggleGroupAttribute.CollapseOthersOnExpand = this.CollapseOthersOnExpand;
		}

		public string ToggleGroupTitle;

		[LabelWidth(160f)]
		public bool CollapseOthersOnExpand;
	}
}
