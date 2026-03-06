using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector.Internal;

namespace Sirenix.OdinInspector
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	public class TabGroupAttribute : PropertyGroupAttribute, ISubGroupProviderAttribute
	{
		public TabGroupAttribute(string tab, bool useFixedHeight = false, float order = 0f) : this("_DefaultTabGroup", tab, useFixedHeight, order)
		{
		}

		public TabGroupAttribute(string group, string tab, bool useFixedHeight = false, float order = 0f) : base(group, order)
		{
			this.TabId = tab;
			this.UseFixedHeight = useFixedHeight;
			this.Tabs = new List<TabGroupAttribute>
			{
				this
			};
		}

		public TabGroupAttribute(string group, string tab, SdfIconType icon, bool useFixedHeight = false, float order = 0f) : this(group, tab, useFixedHeight, order)
		{
			this.Icon = icon;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			TabGroupAttribute tabGroupAttribute = other as TabGroupAttribute;
			if (tabGroupAttribute.TabId != null)
			{
				if (tabGroupAttribute.TabLayouting != TabLayouting.MultiRow)
				{
					this.TabLayouting = tabGroupAttribute.TabLayouting;
				}
				this.UseFixedHeight = (this.UseFixedHeight || tabGroupAttribute.UseFixedHeight);
				this.Paddingless = (this.Paddingless || tabGroupAttribute.Paddingless);
				this.HideTabGroupIfTabGroupOnlyHasOneTab = (this.HideTabGroupIfTabGroupOnlyHasOneTab || tabGroupAttribute.HideTabGroupIfTabGroupOnlyHasOneTab);
				bool flag = false;
				for (int i = 0; i < this.Tabs.Count; i++)
				{
					TabGroupAttribute tabGroupAttribute2 = this.Tabs[i];
					if (tabGroupAttribute2.TabId == tabGroupAttribute.TabId)
					{
						if (tabGroupAttribute2.TextColor == null)
						{
							tabGroupAttribute2.TextColor = tabGroupAttribute.TextColor;
						}
						if (tabGroupAttribute2.Icon == SdfIconType.None)
						{
							tabGroupAttribute2.Icon = tabGroupAttribute.Icon;
						}
						if (tabGroupAttribute2.TabName == null)
						{
							tabGroupAttribute2.TabName = tabGroupAttribute.TabName;
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.Tabs.Add(tabGroupAttribute);
				}
			}
		}

		IList<PropertyGroupAttribute> ISubGroupProviderAttribute.GetSubGroupAttributes()
		{
			int num = 0;
			List<PropertyGroupAttribute> list = new List<PropertyGroupAttribute>(this.Tabs.Count)
			{
				new TabGroupAttribute.TabSubGroupAttribute(this, this.GroupID + "/" + this.TabId, (float)num++)
			};
			foreach (TabGroupAttribute tabGroupAttribute in this.Tabs)
			{
				if (tabGroupAttribute.TabId != this.TabId)
				{
					list.Add(new TabGroupAttribute.TabSubGroupAttribute(tabGroupAttribute, this.GroupID + "/" + tabGroupAttribute.TabId, (float)num++));
				}
			}
			return list;
		}

		string ISubGroupProviderAttribute.RepathMemberAttribute(PropertyGroupAttribute attr)
		{
			TabGroupAttribute tabGroupAttribute = (TabGroupAttribute)attr;
			return this.GroupID + "/" + tabGroupAttribute.TabId;
		}

		public const string DEFAULT_NAME = "_DefaultTabGroup";

		public string TabName;

		public string TabId;

		public bool UseFixedHeight;

		public bool Paddingless;

		public bool HideTabGroupIfTabGroupOnlyHasOneTab;

		public string TextColor;

		public SdfIconType Icon;

		public TabLayouting TabLayouting;

		public List<TabGroupAttribute> Tabs;

		[Conditional("UNITY_EDITOR")]
		public class TabSubGroupAttribute : PropertyGroupAttribute
		{
			public TabSubGroupAttribute(TabGroupAttribute tab, string groupId, float order) : base(groupId, order)
			{
				this.Tab = tab;
			}

			protected override void CombineValuesWith(PropertyGroupAttribute other)
			{
				TabGroupAttribute.TabSubGroupAttribute tabSubGroupAttribute = other as TabGroupAttribute.TabSubGroupAttribute;
				if (tabSubGroupAttribute != null)
				{
					if (this.Tab.TextColor == null)
					{
						this.Tab.TextColor = tabSubGroupAttribute.Tab.TextColor;
					}
					if (this.Tab.Icon == SdfIconType.None)
					{
						this.Tab.Icon = tabSubGroupAttribute.Tab.Icon;
					}
					if (this.Tab.TabName == null)
					{
						this.Tab.TabName = tabSubGroupAttribute.Tab.TabName;
					}
				}
			}

			public TabGroupAttribute Tab;
		}
	}
}
