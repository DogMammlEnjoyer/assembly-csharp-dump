using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	[DontApplyToListElements]
	public sealed class ListDrawerSettingsAttribute : Attribute
	{
		public bool ShowPaging
		{
			get
			{
				return this.paging;
			}
			set
			{
				this.paging = value;
				this.pagingHasValue = true;
			}
		}

		public bool DraggableItems
		{
			get
			{
				return this.draggable;
			}
			set
			{
				this.draggable = value;
				this.draggableHasValue = true;
			}
		}

		public int NumberOfItemsPerPage
		{
			get
			{
				return this.numberOfItemsPerPage;
			}
			set
			{
				this.numberOfItemsPerPage = value;
				this.numberOfItemsPerPageHasValue = true;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
				this.isReadOnlyHasValue = true;
			}
		}

		public bool ShowItemCount
		{
			get
			{
				return this.showItemCount;
			}
			set
			{
				this.showItemCount = value;
				this.showItemCountHasValue = true;
			}
		}

		[Obsolete("Use ShowFoldout instead, which is what Expanded has always done. If you want to control the default expanded state, use DefaultExpandedState. Expanded has been implemented wrong for a long time.", false)]
		public bool Expanded
		{
			get
			{
				return !this.ShowFoldout;
			}
			set
			{
				this.ShowFoldout = !value;
			}
		}

		public bool DefaultExpandedState
		{
			get
			{
				return this.defaultExpandedState;
			}
			set
			{
				this.defaultExpandedStateHasValue = true;
				this.defaultExpandedState = value;
			}
		}

		public bool ShowIndexLabels
		{
			get
			{
				return this.showIndexLabels;
			}
			set
			{
				this.showIndexLabels = value;
				this.showIndexLabelsHasValue = true;
			}
		}

		public string OnTitleBarGUI
		{
			get
			{
				return this.onTitleBarGUI;
			}
			set
			{
				this.onTitleBarGUI = value;
			}
		}

		public bool PagingHasValue
		{
			get
			{
				return this.pagingHasValue;
			}
		}

		public bool ShowItemCountHasValue
		{
			get
			{
				return this.showItemCountHasValue;
			}
		}

		public bool NumberOfItemsPerPageHasValue
		{
			get
			{
				return this.numberOfItemsPerPageHasValue;
			}
		}

		public bool DraggableHasValue
		{
			get
			{
				return this.draggableHasValue;
			}
		}

		public bool IsReadOnlyHasValue
		{
			get
			{
				return this.isReadOnlyHasValue;
			}
		}

		public bool ShowIndexLabelsHasValue
		{
			get
			{
				return this.showIndexLabelsHasValue;
			}
		}

		public bool DefaultExpandedStateHasValue
		{
			get
			{
				return this.defaultExpandedStateHasValue;
			}
		}

		public bool HideAddButton;

		public bool HideRemoveButton;

		public string ListElementLabelName;

		public string CustomAddFunction;

		public string CustomRemoveIndexFunction;

		public string CustomRemoveElementFunction;

		public string OnBeginListElementGUI;

		public string OnEndListElementGUI;

		public bool AlwaysAddDefaultValue;

		public bool AddCopiesLastElement;

		public string ElementColor;

		private string onTitleBarGUI;

		private int numberOfItemsPerPage;

		private bool paging;

		private bool draggable;

		private bool isReadOnly;

		private bool showItemCount;

		private bool pagingHasValue;

		private bool draggableHasValue;

		private bool isReadOnlyHasValue;

		private bool showItemCountHasValue;

		private bool numberOfItemsPerPageHasValue;

		private bool showIndexLabels;

		private bool showIndexLabelsHasValue;

		private bool defaultExpandedStateHasValue;

		private bool defaultExpandedState;

		public bool ShowFoldout = true;
	}
}
