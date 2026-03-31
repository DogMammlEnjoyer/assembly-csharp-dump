using System;
using System.Diagnostics;
using System.Linq;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	public class AssetSelectorAttribute : Attribute
	{
		[ShowInInspector]
		[DelayedProperty]
		[OdinDesignerBinding(new string[]
		{
			"SearchInFolders"
		})]
		public string Paths
		{
			get
			{
				if (this.SearchInFolders != null)
				{
					return string.Join(",", this.SearchInFolders);
				}
				return null;
			}
			set
			{
				this.SearchInFolders = (from x in value.Split(new char[]
				{
					'|'
				})
				select x.Trim().Trim(new char[]
				{
					'/',
					'\\'
				})).ToArray<string>();
			}
		}

		[LabelWidth(200f)]
		public bool IsUniqueList = true;

		[LabelWidth(200f)]
		public bool DrawDropdownForListElements = true;

		[LabelWidth(200f)]
		public bool DisableListAddButtonBehaviour;

		[LabelWidth(200f)]
		public bool ExcludeExistingValuesInList;

		[LabelWidth(200f)]
		public bool ExpandAllMenuItems = true;

		[LabelWidth(200f)]
		public bool FlattenTreeView;

		public int DropdownWidth;

		public int DropdownHeight;

		public string DropdownTitle;

		public string[] SearchInFolders;

		public string Filter;
	}
}
