using System;

namespace UnityEngine.UIElements
{
	internal class ReusableMultiColumnListViewItem : ReusableListViewItem
	{
		public override VisualElement rootElement
		{
			get
			{
				return base.bindableElement;
			}
		}

		public override void Init(VisualElement item)
		{
		}

		public void Init(VisualElement container, Columns columns, bool usesAnimatedDrag)
		{
			int num = 0;
			base.bindableElement = container;
			foreach (Column column in columns.visibleList)
			{
				bool flag = columns.IsPrimary(column);
				if (flag)
				{
					VisualElement visualElement = container[num];
					VisualElement item = visualElement.GetProperty(MultiColumnController.bindableElementPropertyName) as VisualElement;
					base.UpdateHierarchy(visualElement, item, usesAnimatedDrag);
					break;
				}
				num++;
			}
		}
	}
}
