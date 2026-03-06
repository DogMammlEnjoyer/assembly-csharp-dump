using System;
using System.Diagnostics;
using System.Linq;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	public class AssetSelectorAttribute : Attribute
	{
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

		public bool IsUniqueList = true;

		public bool DrawDropdownForListElements = true;

		public bool DisableListAddButtonBehaviour;

		public bool ExcludeExistingValuesInList;

		public bool ExpandAllMenuItems = true;

		public bool FlattenTreeView;

		public int DropdownWidth;

		public int DropdownHeight;

		public string DropdownTitle;

		public string[] SearchInFolders;

		public string Filter;
	}
}
