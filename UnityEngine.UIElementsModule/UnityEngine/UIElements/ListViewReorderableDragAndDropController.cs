using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class ListViewReorderableDragAndDropController : BaseReorderableDragAndDropController
	{
		public ListViewReorderableDragAndDropController(BaseListView view) : base(view)
		{
			this.m_ListView = view;
		}

		public override DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args)
		{
			bool flag = args.dragAndDropPosition == DragAndDropPosition.OverItem || !this.CanDrop();
			DragVisualMode result;
			if (flag)
			{
				result = DragVisualMode.Rejected;
			}
			else
			{
				result = ((args.dragAndDropData.source == this.m_ListView) ? DragVisualMode.Move : DragVisualMode.Rejected);
			}
			return result;
		}

		public override void OnDrop(IListDragAndDropArgs args)
		{
			base.OnDrop(args);
			bool flag = !this.m_ListView.reorderable;
			if (!flag)
			{
				int insertAtIndex = args.insertAtIndex;
				int num = 0;
				int num2 = 0;
				for (int i = this.m_SortedSelectedIds.Count - 1; i >= 0; i--)
				{
					int id = this.m_SortedSelectedIds[i];
					int num3 = this.m_View.viewController.GetIndexForId(id);
					bool flag2 = num3 < 0;
					if (!flag2)
					{
						int num4 = insertAtIndex - num;
						bool flag3 = num3 >= insertAtIndex;
						if (flag3)
						{
							num3 += num2;
							num2++;
						}
						else
						{
							bool flag4 = num3 < num4;
							if (flag4)
							{
								num++;
								num4--;
							}
						}
						this.m_ListView.viewController.Move(num3, num4);
					}
				}
				bool flag5 = this.m_ListView.selectionType > SelectionType.None;
				if (flag5)
				{
					List<int> list = new List<int>();
					for (int j = 0; j < this.m_SortedSelectedIds.Count; j++)
					{
						list.Add(insertAtIndex - num + j);
					}
					this.m_ListView.SetSelectionWithoutNotify(list);
				}
				else
				{
					this.m_ListView.ClearSelection();
				}
			}
		}

		protected readonly BaseListView m_ListView;
	}
}
