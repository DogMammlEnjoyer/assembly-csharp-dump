using System;
using UnityEngine;

namespace Meta.WitAi.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class DropDownAttribute : PropertyAttribute
	{
		public string OptionListGetterName { get; }

		public bool RefreshOnRepaint { get; }

		public bool AllowInvalid { get; }

		public bool ShowPropertyIfListIsEmpty { get; }

		public bool ShowRefreshButton { get; }

		public string RefreshMethodName { get; }

		public bool ShowSearch { get; }

		public DropDownAttribute(string optionListGetterName, bool refreshOnRepaint = false, bool allowInvalid = false, bool showPropertyIfListIsEmpty = true, bool showRefreshButton = true, string refreshMethodName = null, bool showSearch = false)
		{
			this.OptionListGetterName = optionListGetterName;
			this.RefreshOnRepaint = refreshOnRepaint;
			this.AllowInvalid = allowInvalid;
			this.ShowPropertyIfListIsEmpty = showPropertyIfListIsEmpty;
			this.ShowRefreshButton = showRefreshButton;
			this.RefreshMethodName = refreshMethodName;
			this.ShowSearch = showSearch;
		}
	}
}
