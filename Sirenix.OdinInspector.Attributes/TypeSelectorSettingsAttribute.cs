using System;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class TypeSelectorSettingsAttribute : Attribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"showNoneItem"
		})]
		public bool ShowNoneItem
		{
			get
			{
				return this.showNoneItem.GetValueOrDefault();
			}
			set
			{
				this.showNoneItem = new bool?(value);
			}
		}

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"showCategories"
		})]
		public bool ShowCategories
		{
			get
			{
				return this.showCategories.GetValueOrDefault();
			}
			set
			{
				this.showCategories = new bool?(value);
			}
		}

		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"preferNamespaces"
		})]
		public bool PreferNamespaces
		{
			get
			{
				return this.preferNamespaces.GetValueOrDefault();
			}
			set
			{
				this.preferNamespaces = new bool?(value);
			}
		}

		public bool ShowNoneItemIsSet
		{
			get
			{
				return this.showNoneItem != null;
			}
		}

		public bool ShowCategoriesIsSet
		{
			get
			{
				return this.showCategories != null;
			}
		}

		public bool PreferNamespacesIsSet
		{
			get
			{
				return this.preferNamespaces != null;
			}
		}

		public const string FILTER_TYPES_FUNCTION_NAMED_VALUE = "type";

		public string FilterTypesFunction;

		private bool? showNoneItem;

		private bool? showCategories;

		private bool? preferNamespaces;
	}
}
