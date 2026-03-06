using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class VisualElementFocusRing : IFocusRing
	{
		public VisualElementFocusRing(VisualElement root, VisualElementFocusRing.DefaultFocusOrder dfo = VisualElementFocusRing.DefaultFocusOrder.ChildOrder)
		{
			this.defaultFocusOrder = dfo;
			this.root = root;
			this.m_FocusRing = new List<VisualElementFocusRing.FocusRingRecord>();
		}

		private FocusController focusController
		{
			get
			{
				return this.root.focusController;
			}
		}

		public VisualElementFocusRing.DefaultFocusOrder defaultFocusOrder { get; set; }

		private int FocusRingAutoIndexSort(VisualElementFocusRing.FocusRingRecord a, VisualElementFocusRing.FocusRingRecord b)
		{
			int result;
			switch (this.defaultFocusOrder)
			{
			default:
				result = Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
				break;
			case VisualElementFocusRing.DefaultFocusOrder.PositionXY:
			{
				VisualElement visualElement = a.m_Focusable as VisualElement;
				VisualElement visualElement2 = b.m_Focusable as VisualElement;
				bool flag = visualElement != null && visualElement2 != null;
				if (flag)
				{
					bool flag2 = visualElement.layout.position.x < visualElement2.layout.position.x;
					if (flag2)
					{
						result = -1;
						break;
					}
					bool flag3 = visualElement.layout.position.x > visualElement2.layout.position.x;
					if (flag3)
					{
						result = 1;
						break;
					}
					bool flag4 = visualElement.layout.position.y < visualElement2.layout.position.y;
					if (flag4)
					{
						result = -1;
						break;
					}
					bool flag5 = visualElement.layout.position.y > visualElement2.layout.position.y;
					if (flag5)
					{
						result = 1;
						break;
					}
				}
				result = Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
				break;
			}
			case VisualElementFocusRing.DefaultFocusOrder.PositionYX:
			{
				VisualElement visualElement3 = a.m_Focusable as VisualElement;
				VisualElement visualElement4 = b.m_Focusable as VisualElement;
				bool flag6 = visualElement3 != null && visualElement4 != null;
				if (flag6)
				{
					bool flag7 = visualElement3.layout.position.y < visualElement4.layout.position.y;
					if (flag7)
					{
						result = -1;
						break;
					}
					bool flag8 = visualElement3.layout.position.y > visualElement4.layout.position.y;
					if (flag8)
					{
						result = 1;
						break;
					}
					bool flag9 = visualElement3.layout.position.x < visualElement4.layout.position.x;
					if (flag9)
					{
						result = -1;
						break;
					}
					bool flag10 = visualElement3.layout.position.x > visualElement4.layout.position.x;
					if (flag10)
					{
						result = 1;
						break;
					}
				}
				result = Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
				break;
			}
			}
			return result;
		}

		private int FocusRingSort(VisualElementFocusRing.FocusRingRecord a, VisualElementFocusRing.FocusRingRecord b)
		{
			bool flag = a.m_Focusable.tabIndex == 0 && b.m_Focusable.tabIndex == 0;
			int result;
			if (flag)
			{
				result = this.FocusRingAutoIndexSort(a, b);
			}
			else
			{
				bool flag2 = a.m_Focusable.tabIndex == 0;
				if (flag2)
				{
					result = 1;
				}
				else
				{
					bool flag3 = b.m_Focusable.tabIndex == 0;
					if (flag3)
					{
						result = -1;
					}
					else
					{
						int num = Comparer<int>.Default.Compare(a.m_Focusable.tabIndex, b.m_Focusable.tabIndex);
						bool flag4 = num == 0;
						if (flag4)
						{
							num = this.FocusRingAutoIndexSort(a, b);
						}
						result = num;
					}
				}
			}
			return result;
		}

		private void DoUpdate()
		{
			this.m_FocusRing.Clear();
			bool flag = this.root != null;
			if (flag)
			{
				List<VisualElementFocusRing.FocusRingRecord> list = new List<VisualElementFocusRing.FocusRingRecord>();
				int num = 0;
				this.BuildRingForScopeRecursive(this.root, ref num, list);
				this.SortAndFlattenScopeLists(list);
			}
		}

		private void BuildRingForScopeRecursive(VisualElement ve, ref int scopeIndex, List<VisualElementFocusRing.FocusRingRecord> scopeList)
		{
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				bool flag = visualElement.parent != null && visualElement == visualElement.parent.contentContainer;
				bool flag2 = visualElement.isCompositeRoot || flag;
				if (flag2)
				{
					VisualElementFocusRing.FocusRingRecord focusRingRecord = new VisualElementFocusRing.FocusRingRecord();
					int num = scopeIndex;
					scopeIndex = num + 1;
					focusRingRecord.m_AutoIndex = num;
					focusRingRecord.m_Focusable = visualElement;
					focusRingRecord.m_IsSlot = flag;
					focusRingRecord.m_ScopeNavigationOrder = new List<VisualElementFocusRing.FocusRingRecord>();
					VisualElementFocusRing.FocusRingRecord focusRingRecord2 = focusRingRecord;
					scopeList.Add(focusRingRecord2);
					int num2 = 0;
					this.BuildRingForScopeRecursive(visualElement, ref num2, focusRingRecord2.m_ScopeNavigationOrder);
				}
				else
				{
					bool flag3 = visualElement.canGrabFocus && visualElement.areAncestorsAndSelfDisplayed && visualElement.tabIndex >= 0;
					if (flag3)
					{
						VisualElementFocusRing.FocusRingRecord focusRingRecord3 = new VisualElementFocusRing.FocusRingRecord();
						int num = scopeIndex;
						scopeIndex = num + 1;
						focusRingRecord3.m_AutoIndex = num;
						focusRingRecord3.m_Focusable = visualElement;
						focusRingRecord3.m_IsSlot = false;
						focusRingRecord3.m_ScopeNavigationOrder = null;
						scopeList.Add(focusRingRecord3);
					}
					this.BuildRingForScopeRecursive(visualElement, ref scopeIndex, scopeList);
				}
			}
		}

		private void SortAndFlattenScopeLists(List<VisualElementFocusRing.FocusRingRecord> rootScopeList)
		{
			bool flag = rootScopeList != null;
			if (flag)
			{
				rootScopeList.Sort(new Comparison<VisualElementFocusRing.FocusRingRecord>(this.FocusRingSort));
				foreach (VisualElementFocusRing.FocusRingRecord focusRingRecord in rootScopeList)
				{
					bool flag2 = focusRingRecord.m_Focusable.canGrabFocus && focusRingRecord.m_Focusable.tabIndex >= 0;
					if (flag2)
					{
						bool flag3 = !focusRingRecord.m_Focusable.excludeFromFocusRing;
						if (flag3)
						{
							this.m_FocusRing.Add(focusRingRecord);
						}
						this.SortAndFlattenScopeLists(focusRingRecord.m_ScopeNavigationOrder);
					}
					else
					{
						bool isSlot = focusRingRecord.m_IsSlot;
						if (isSlot)
						{
							this.SortAndFlattenScopeLists(focusRingRecord.m_ScopeNavigationOrder);
						}
					}
					focusRingRecord.m_ScopeNavigationOrder = null;
				}
			}
		}

		private int GetFocusableInternalIndex(Focusable f)
		{
			bool flag = f != null;
			if (flag)
			{
				for (int i = 0; i < this.m_FocusRing.Count; i++)
				{
					bool flag2 = f == this.m_FocusRing[i].m_Focusable;
					if (flag2)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public FocusChangeDirection GetFocusChangeDirection(Focusable currentFocusable, EventBase e)
		{
			bool flag = e == null;
			if (flag)
			{
				throw new ArgumentNullException("e");
			}
			bool flag2 = e.eventTypeId == EventBase<PointerDownEvent>.TypeId();
			if (flag2)
			{
				Focusable target;
				bool focusableParentForPointerEvent = this.focusController.GetFocusableParentForPointerEvent(e.elementTarget, out target);
				if (focusableParentForPointerEvent)
				{
					return VisualElementFocusChangeTarget.GetPooled(target);
				}
			}
			bool flag3 = currentFocusable != null && currentFocusable.isIMGUIContainer;
			FocusChangeDirection result;
			if (flag3)
			{
				result = FocusChangeDirection.none;
			}
			else
			{
				bool flag4 = e.eventTypeId == EventBase<NavigationMoveEvent>.TypeId();
				if (flag4)
				{
					NavigationMoveEvent.Direction direction = ((NavigationMoveEvent)e).direction;
					result = ((direction == NavigationMoveEvent.Direction.Next) ? VisualElementFocusChangeDirection.right : ((direction == NavigationMoveEvent.Direction.Previous) ? VisualElementFocusChangeDirection.left : FocusChangeDirection.none));
				}
				else
				{
					result = FocusChangeDirection.none;
				}
			}
			return result;
		}

		public Focusable GetNextFocusable(Focusable currentFocusable, FocusChangeDirection direction)
		{
			bool flag = direction == FocusChangeDirection.none || direction == FocusChangeDirection.unspecified;
			Focusable result;
			if (flag)
			{
				result = currentFocusable;
			}
			else
			{
				VisualElementFocusChangeTarget visualElementFocusChangeTarget = direction as VisualElementFocusChangeTarget;
				bool flag2 = visualElementFocusChangeTarget != null;
				if (flag2)
				{
					result = visualElementFocusChangeTarget.target;
				}
				else
				{
					result = this.GetNextFocusableInSequence(currentFocusable, direction);
				}
			}
			return result;
		}

		internal Focusable GetNextFocusableInSequence(Focusable currentFocusable, FocusChangeDirection direction)
		{
			this.DoUpdate();
			bool flag = this.m_FocusRing.Count == 0;
			Focusable result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int focusableInternalIndex = this.GetFocusableInternalIndex(currentFocusable);
				bool flag2 = currentFocusable != null && focusableInternalIndex == -1;
				if (flag2)
				{
					bool flag3 = direction == VisualElementFocusChangeDirection.right;
					if (flag3)
					{
						return this.GetNextFocusableInTree(currentFocusable as VisualElement);
					}
					bool flag4 = direction == VisualElementFocusChangeDirection.left;
					if (flag4)
					{
						return this.GetPreviousFocusableInTree(currentFocusable as VisualElement);
					}
				}
				int num = 0;
				bool flag5 = direction == VisualElementFocusChangeDirection.right;
				if (flag5)
				{
					num = focusableInternalIndex + 1;
					bool flag6 = num == this.m_FocusRing.Count;
					if (flag6)
					{
						num = 0;
					}
					while (this.m_FocusRing[num].m_Focusable.delegatesFocus)
					{
						num++;
						bool flag7 = num == this.m_FocusRing.Count;
						if (flag7)
						{
							return null;
						}
					}
				}
				else
				{
					bool flag8 = direction == VisualElementFocusChangeDirection.left;
					if (flag8)
					{
						num = focusableInternalIndex - 1;
						bool flag9 = num < 0;
						if (flag9)
						{
							num = this.m_FocusRing.Count - 1;
						}
						while (this.m_FocusRing[num].m_Focusable.delegatesFocus)
						{
							num--;
							bool flag10 = num == -1;
							if (flag10)
							{
								return null;
							}
						}
					}
				}
				result = this.m_FocusRing[num].m_Focusable;
			}
			return result;
		}

		internal VisualElement GetNextFocusableInTree(VisualElement currentFocusable)
		{
			bool flag = currentFocusable == null;
			VisualElement result;
			if (flag)
			{
				result = null;
			}
			else
			{
				VisualElement nextElementDepthFirst = this.GetNextElementDepthFirst(currentFocusable);
				while (!nextElementDepthFirst.canGrabFocus || nextElementDepthFirst.tabIndex < 0 || nextElementDepthFirst.excludeFromFocusRing)
				{
					nextElementDepthFirst = this.GetNextElementDepthFirst(nextElementDepthFirst);
					bool flag2 = nextElementDepthFirst == currentFocusable;
					if (flag2)
					{
						return currentFocusable;
					}
				}
				result = nextElementDepthFirst;
			}
			return result;
		}

		internal VisualElement GetPreviousFocusableInTree(VisualElement currentFocusable)
		{
			bool flag = currentFocusable == null;
			VisualElement result;
			if (flag)
			{
				result = null;
			}
			else
			{
				VisualElement previousElementDepthFirst = this.GetPreviousElementDepthFirst(currentFocusable);
				while (!previousElementDepthFirst.canGrabFocus || previousElementDepthFirst.tabIndex < 0 || previousElementDepthFirst.excludeFromFocusRing)
				{
					previousElementDepthFirst = this.GetPreviousElementDepthFirst(previousElementDepthFirst);
					bool flag2 = previousElementDepthFirst == currentFocusable;
					if (flag2)
					{
						return currentFocusable;
					}
				}
				result = previousElementDepthFirst;
			}
			return result;
		}

		private VisualElement GetNextElementDepthFirst(VisualElement ve)
		{
			ve = ve.GetNextElementDepthFirst();
			bool flag = !this.root.Contains(ve);
			if (flag)
			{
				ve = this.root;
			}
			return ve;
		}

		private VisualElement GetPreviousElementDepthFirst(VisualElement ve)
		{
			ve = ve.GetPreviousElementDepthFirst();
			bool flag = !this.root.Contains(ve);
			if (flag)
			{
				ve = this.root;
				while (ve.childCount > 0)
				{
					ve = ve.hierarchy.ElementAt(ve.childCount - 1);
				}
			}
			return ve;
		}

		private readonly VisualElement root;

		private List<VisualElementFocusRing.FocusRingRecord> m_FocusRing;

		public enum DefaultFocusOrder
		{
			ChildOrder,
			PositionXY,
			PositionYX
		}

		private class FocusRingRecord
		{
			public int m_AutoIndex;

			public Focusable m_Focusable;

			public bool m_IsSlot;

			public List<VisualElementFocusRing.FocusRingRecord> m_ScopeNavigationOrder;
		}
	}
}
