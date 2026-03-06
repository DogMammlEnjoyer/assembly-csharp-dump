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

		public int NumberOfItemsBeforeEnablingSearch;

		public bool IsUniqueList;

		public bool DrawDropdownForListElements;

		public bool DisableListAddButtonBehaviour;

		public bool ExcludeExistingValuesInList;

		public bool ExpandAllMenuItems;

		public bool AppendNextDrawer;

		public bool DisableGUIInAppendedDrawer;

		public bool DoubleClickToConfirm;

		public bool FlattenTreeView;

		public int DropdownWidth;

		public int DropdownHeight;

		public string DropdownTitle;

		public bool SortDropdownItems;

		public bool HideChildProperties;

		public bool CopyValues = true;

		public bool OnlyChangeValueOnConfirm;
	}
}
