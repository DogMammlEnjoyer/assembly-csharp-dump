using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class ValueDropdownAttribute : Attribute
	{
		[Obsolete("Use the ValuesGetter member instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string MemberName
		{
			get
			{
				return this.ValuesGetter;
			}
			set
			{
				this.ValuesGetter = value;
			}
		}

		public ValueDropdownAttribute(string valuesGetter)
		{
			this.NumberOfItemsBeforeEnablingSearch = 10;
			this.ValuesGetter = valuesGetter;
			this.DrawDropdownForListElements = true;
		}

		public string ValuesGetter;

		[LabelWidth(230f)]
		public int NumberOfItemsBeforeEnablingSearch;

		[LabelWidth(230f)]
		public bool IsUniqueList;

		[LabelWidth(230f)]
		public bool DrawDropdownForListElements;

		[LabelWidth(230f)]
		public bool DisableListAddButtonBehaviour;

		[LabelWidth(230f)]
		public bool ExcludeExistingValuesInList;

		[LabelWidth(230f)]
		public bool ExpandAllMenuItems;

		[LabelWidth(230f)]
		public bool AppendNextDrawer;

		[LabelWidth(230f)]
		public bool DisableGUIInAppendedDrawer;

		[LabelWidth(230f)]
		public bool DoubleClickToConfirm;

		[LabelWidth(230f)]
		public bool FlattenTreeView;

		public int DropdownWidth;

		public int DropdownHeight;

		public string DropdownTitle;

		[LabelWidth(230f)]
		public bool SortDropdownItems;

		[LabelWidth(230f)]
		public bool HideChildProperties;

		[LabelWidth(230f)]
		public bool CopyValues = true;

		[LabelWidth(230f)]
		public bool OnlyChangeValueOnConfirm;
	}
}
