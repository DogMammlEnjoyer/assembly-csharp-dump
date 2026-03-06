using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class TreeViewReorderableDragAndDropController : BaseReorderableDragAndDropController
	{
		public TreeViewReorderableDragAndDropController(BaseTreeView view) : base(view)
		{
			this.m_TreeView = view;
			this.m_ExpandDropItemCallback = new Action(this.ExpandDropItem);
		}

		protected override int CompareId(int id1, int id2)
		{
			bool flag = id1 == id2;
			int result;
			if (flag)
			{
				result = id1.CompareTo(id2);
			}
			else
			{
				int num = id1;
				int num2 = id2;
				List<int> list;
				using (CollectionPool<List<int>, int>.Get(out list))
				{
					while (num != BaseTreeView.invalidId)
					{
						list.Add(num);
						num = this.m_TreeView.viewController.GetParentId(num);
					}
					List<int> list2;
					using (CollectionPool<List<int>, int>.Get(out list2))
					{
						while (num2 != BaseTreeView.invalidId)
						{
							list2.Add(num2);
							num2 = this.m_TreeView.viewController.GetParentId(num2);
						}
						list.Add(BaseTreeView.invalidId);
						list2.Add(BaseTreeView.invalidId);
						int i = 0;
						while (i < list.Count)
						{
							int item = list[i];
							int num3 = list2.IndexOf(item);
							bool flag2 = num3 >= 0;
							if (flag2)
							{
								bool flag3 = i == 0;
								if (flag3)
								{
									return -1;
								}
								int id3 = (i > 0) ? list[i - 1] : id1;
								int id4 = (num3 > 0) ? list2[num3 - 1] : id2;
								int childIndexForId = this.m_TreeView.viewController.GetChildIndexForId(id3);
								int childIndexForId2 = this.m_TreeView.viewController.GetChildIndexForId(id4);
								return childIndexForId.CompareTo(childIndexForId2);
							}
							else
							{
								i++;
							}
						}
						throw new ArgumentOutOfRangeException("[UI Toolkit] Trying to reorder ids that are not in the same tree.");
					}
				}
			}
			return result;
		}

		public override StartDragArgs SetupDragAndDrop(IEnumerable<int> itemIds, bool skipText = false)
		{
			StartDragArgs startDragArgs = base.SetupDragAndDrop(itemIds, skipText);
			this.m_DropData.draggedIds = base.GetSortedSelectedIds().ToArray<int>();
			return this.m_TreeView.reorderable ? startDragArgs : new StartDragArgs(string.Empty, DragVisualMode.Rejected);
		}

		public override DragVisualMode HandleDragAndDrop(IListDragAndDropArgs args)
		{
			return (args.dragAndDropData.source == this.m_TreeView && this.CanDrop()) ? DragVisualMode.Move : DragVisualMode.Rejected;
		}

		public override bool CanDrop()
		{
			bool result;
			if (!base.CanDrop())
			{
				TreeViewReorderableDragAndDropController.DropData dropData = this.m_DropData;
				result = (dropData != null && dropData.draggedIds != null);
			}
			else
			{
				result = true;
			}
			return result;
		}

		public override void OnDrop(IListDragAndDropArgs args)
		{
			base.OnDrop(args);
			bool flag;
			if (this.m_TreeView.reorderable)
			{
				TreeViewReorderableDragAndDropController.DropData dropData = this.m_DropData;
				flag = (((dropData != null) ? dropData.draggedIds : null) == null);
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			if (!flag2)
			{
				int parentId = args.parentId;
				int childIndex = args.childIndex;
				int num = 0;
				bool flag3 = args.dragAndDropPosition == DragAndDropPosition.OverItem || (parentId == -1 && childIndex == -1);
				List<ValueTuple<int, int>> list;
				using (CollectionPool<List<ValueTuple<int, int>>, ValueTuple<int, int>>.Get(out list))
				{
					foreach (int id in this.m_DropData.draggedIds)
					{
						int parentId2 = this.m_TreeView.viewController.GetParentId(id);
						int childIndexForId = this.m_TreeView.viewController.GetChildIndexForId(id);
						list.Add(new ValueTuple<int, int>(parentId2, childIndexForId));
						bool flag4 = flag3;
						if (flag4)
						{
							this.m_TreeView.viewController.Move(id, parentId, -1, false);
						}
						else
						{
							int childIndex2 = childIndex + num;
							bool flag5 = parentId2 != parentId || childIndexForId >= childIndex;
							if (flag5)
							{
								num++;
							}
							this.m_TreeView.viewController.Move(id, parentId, childIndex2, false);
						}
					}
					bool flag6 = args.dragAndDropPosition == DragAndDropPosition.OverItem;
					if (flag6)
					{
						this.m_TreeView.viewController.ExpandItem(parentId, false, false);
					}
					IVisualElementScheduledItem expandDropItemScheduledItem = this.m_ExpandDropItemScheduledItem;
					if (expandDropItemScheduledItem != null)
					{
						expandDropItemScheduledItem.Pause();
					}
					this.m_TreeView.RefreshItems();
					for (int j = 0; j < this.m_DropData.draggedIds.Length; j++)
					{
						int id2 = this.m_DropData.draggedIds[j];
						ValueTuple<int, int> valueTuple = list[j];
						int parentId3 = this.m_TreeView.viewController.GetParentId(id2);
						int childIndexForId2 = this.m_TreeView.viewController.GetChildIndexForId(id2);
						bool flag7 = valueTuple.Item1 == parentId3 && valueTuple.Item2 == childIndexForId2;
						if (!flag7)
						{
							this.m_TreeView.viewController.RaiseItemParentChanged(id2, parentId);
						}
					}
				}
			}
		}

		public override void DragCleanup()
		{
			base.DragCleanup();
			bool flag = this.m_DropData != null;
			if (flag)
			{
				bool flag2 = this.m_DropData.expandedIdsBeforeDrag != null;
				if (flag2)
				{
					this.RestoreExpanded(new List<int>(this.m_DropData.expandedIdsBeforeDrag));
				}
				this.m_DropData = new TreeViewReorderableDragAndDropController.DropData();
			}
			IVisualElementScheduledItem expandDropItemScheduledItem = this.m_ExpandDropItemScheduledItem;
			if (expandDropItemScheduledItem != null)
			{
				expandDropItemScheduledItem.Pause();
			}
		}

		private void RestoreExpanded(List<int> ids)
		{
			foreach (int num in this.m_TreeView.viewController.GetAllItemIds(null))
			{
				bool flag = !ids.Contains(num);
				if (flag)
				{
					this.m_TreeView.CollapseItem(num, false, true);
				}
			}
		}

		public override void HandleAutoExpand(ReusableCollectionItem item, Vector2 pointerPosition)
		{
			int id = item.id;
			Rect worldBound = item.bindableElement.worldBound;
			Rect rect = new Rect(worldBound.x, worldBound.y + 4f, worldBound.width, worldBound.height - 8f);
			bool flag = rect.Contains(pointerPosition);
			Vector2 vector = this.m_DropData.expandItemBeginPosition - pointerPosition;
			bool flag2 = id != this.m_DropData.lastItemId || !flag || vector.sqrMagnitude >= 100f;
			if (flag2)
			{
				this.m_DropData.lastItemId = id;
				this.m_DropData.expandItemBeginTimerMs = (float)Panel.TimeSinceStartupMs();
				this.m_DropData.expandItemBeginPosition = pointerPosition;
				this.DelayExpandDropItem();
			}
		}

		private void DelayExpandDropItem()
		{
			bool flag = this.m_ExpandDropItemScheduledItem == null;
			if (flag)
			{
				this.m_ExpandDropItemScheduledItem = this.m_TreeView.schedule.Execute(this.m_ExpandDropItemCallback).Every(10L);
			}
			else
			{
				this.m_ExpandDropItemScheduledItem.Pause();
				this.m_ExpandDropItemScheduledItem.Resume();
			}
		}

		private void ExpandDropItem()
		{
			bool flag = (float)Panel.TimeSinceStartupMs() - this.m_DropData.expandItemBeginTimerMs > 700f;
			bool flag2 = flag;
			int lastItemId = this.m_DropData.lastItemId;
			bool flag3 = this.m_TreeView.viewController.Exists(lastItemId) && flag2;
			if (flag3)
			{
				bool flag4 = this.m_TreeView.viewController.HasChildren(lastItemId);
				bool flag5 = this.m_TreeView.IsExpanded(lastItemId);
				bool flag6 = !flag4 || flag5;
				if (!flag6)
				{
					List<int> list = CollectionPool<List<int>, int>.Get();
					this.m_TreeView.viewController.GetExpandedItemIds(list);
					TreeViewReorderableDragAndDropController.DropData dropData = this.m_DropData;
					if (dropData.expandedIdsBeforeDrag == null)
					{
						dropData.expandedIdsBeforeDrag = list.ToArray();
					}
					this.m_DropData.expandItemBeginTimerMs = (float)Panel.TimeSinceStartupMs();
					this.m_DropData.lastItemId = 0;
					this.m_TreeView.ExpandItem(lastItemId, false, true);
					CollectionPool<List<int>, int>.Release(list);
				}
			}
		}

		private const long k_ExpandUpdateIntervalMs = 10L;

		private const float k_DropExpandTimeoutMs = 700f;

		private const float k_DropDeltaPosition = 100f;

		private const float k_HalfDropBetweenHeight = 4f;

		protected TreeViewReorderableDragAndDropController.DropData m_DropData = new TreeViewReorderableDragAndDropController.DropData();

		protected readonly BaseTreeView m_TreeView;

		private IVisualElementScheduledItem m_ExpandDropItemScheduledItem;

		private Action m_ExpandDropItemCallback;

		protected class DropData
		{
			public int[] expandedIdsBeforeDrag;

			public int[] draggedIds;

			public int lastItemId = -1;

			public float expandItemBeginTimerMs;

			public Vector2 expandItemBeginPosition;
		}
	}
}
