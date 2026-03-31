using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	[DontApplyToListElements]
	public sealed class ListDrawerSettingsAttribute : Attribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"paging",
			"pagingHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"draggable",
			"draggableHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"numberOfItemsPerPage",
			"numberOfItemsPerPageHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"isReadOnly",
			"isReadOnlyHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"showItemCount",
			"showItemCountHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"defaultExpandedState",
			"defaultExpandedStateHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"showIndexLabels",
			"showIndexLabelsHasValue"
		})]
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

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"onTitleBarGUI"
		})]
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

		[LabelWidth(200f)]
		public string CustomRemoveIndexFunction;

		[LabelWidth(200f)]
		public string CustomRemoveElementFunction;

		public string OnBeginListElementGUI;

		public string OnEndListElementGUI;

		public bool AlwaysAddDefaultValue;

		public bool AddCopiesLastElement;

		[ColorResolver]
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
