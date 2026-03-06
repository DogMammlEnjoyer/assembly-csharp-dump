using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class TabLayout
	{
		public TabLayout(TabView tabView, bool isVertical)
		{
			this.m_TabView = tabView;
			this.m_TabHeaders = tabView.tabHeaders;
			this.m_IsVertical = isVertical;
		}

		public static float GetHeight(VisualElement t)
		{
			return t.boundingBox.height;
		}

		public static float GetWidth(VisualElement t)
		{
			return t.boundingBox.width;
		}

		public float GetTabOffset(VisualElement tab)
		{
			bool flag = !tab.visible;
			float result;
			if (flag)
			{
				result = float.NaN;
			}
			else
			{
				float num = 0f;
				int num2 = this.m_TabHeaders.IndexOf(tab);
				for (int i = 0; i < num2; i++)
				{
					VisualElement t = this.m_TabHeaders[i];
					float num3 = this.m_IsVertical ? TabLayout.GetHeight(t) : TabLayout.GetWidth(t);
					bool flag2 = float.IsNaN(num3);
					if (!flag2)
					{
						num += num3;
					}
				}
				result = num;
			}
			return result;
		}

		private void InitOrderTabs()
		{
			if (this.m_TabHeaders == null)
			{
				this.m_TabHeaders = new List<VisualElement>();
			}
		}

		public void ReorderDisplay(int from, int to)
		{
			this.InitOrderTabs();
			this.m_TabView.ReorderTab(from, to);
		}

		private TabView m_TabView;

		private List<VisualElement> m_TabHeaders;

		private bool m_IsVertical;
	}
}
