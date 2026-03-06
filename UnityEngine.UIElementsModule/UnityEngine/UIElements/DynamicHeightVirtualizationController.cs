using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class DynamicHeightVirtualizationController<T> : VerticalVirtualizationController<T> where T : ReusableCollectionItem, new()
	{
		internal IReadOnlyDictionary<int, float> itemHeightCache
		{
			get
			{
				return this.m_ItemHeightCache;
			}
		}

		private float defaultExpectedHeight
		{
			get
			{
				bool flag = this.m_MinimumItemHeight > 0f;
				float result;
				if (flag)
				{
					result = this.m_MinimumItemHeight;
				}
				else
				{
					bool flag2 = this.m_CollectionView.m_ItemHeightIsInline && this.m_CollectionView.fixedItemHeight > 0f;
					if (flag2)
					{
						result = this.m_CollectionView.fixedItemHeight;
					}
					else
					{
						result = 22f;
					}
				}
				return result;
			}
		}

		private float contentPadding
		{
			get
			{
				return base.serializedData.contentPadding;
			}
			set
			{
				this.m_CollectionView.scrollView.contentContainer.style.paddingTop = value;
				base.serializedData.contentPadding = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private float contentHeight
		{
			get
			{
				return base.serializedData.contentHeight;
			}
			set
			{
				this.m_CollectionView.scrollView.contentContainer.style.height = value;
				base.serializedData.contentHeight = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private int anchoredIndex
		{
			get
			{
				return base.serializedData.anchoredItemIndex;
			}
			set
			{
				base.serializedData.anchoredItemIndex = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private float anchorOffset
		{
			get
			{
				return base.serializedData.anchorOffset;
			}
			set
			{
				base.serializedData.anchorOffset = value;
				this.m_CollectionView.SaveViewData();
			}
		}

		private float viewportMaxOffset
		{
			get
			{
				return this.m_ScrollView.scrollOffset.y + this.m_ScrollView.contentViewport.layout.height;
			}
		}

		protected override bool alwaysRebindOnRefresh
		{
			get
			{
				return false;
			}
		}

		public DynamicHeightVirtualizationController(BaseVerticalCollectionView collectionView) : base(collectionView)
		{
			this.m_FillCallback = new Action(this.Fill);
			this.m_GeometryChangedCallback = new Action<ReusableCollectionItem>(this.OnRecycledItemGeometryChanged);
			this.m_IndexOutOfBoundsPredicate = new Predicate<int>(this.IsIndexOutOfBounds);
			this.m_ScrollResetCallback = new Action(this.ResetScroll);
			collectionView.RegisterCallback<DetachFromPanelEvent>(new EventCallback<DetachFromPanelEvent>(this.OnDetachFromPanelEvent), TrickleDown.NoTrickleDown);
			collectionView.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChangedEvent), TrickleDown.NoTrickleDown);
		}

		private void OnGeometryChangedEvent(GeometryChangedEvent _)
		{
			bool flag = this.m_ScrolledToItemIndex != null;
			if (flag)
			{
				bool flag2 = base.ShouldDeferScrollToItem(this.m_ScrolledToItemIndex ?? -1);
				if (flag2)
				{
					base.ScheduleDeferredScrollToItem();
				}
				this.m_ScrolledToItemIndex = null;
			}
		}

		public override void Refresh(bool rebuild)
		{
			this.CleanItemHeightCache();
			int count = this.m_ActiveItems.Count;
			bool flag = false;
			if (rebuild)
			{
				this.m_WaitingCache.Clear();
			}
			else
			{
				flag |= (this.m_WaitingCache.RemoveWhere(this.m_IndexOutOfBoundsPredicate) > 0);
			}
			base.Refresh(rebuild);
			this.m_ScrollDirection = DynamicHeightVirtualizationController<T>.ScrollDirection.Idle;
			this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.None;
			bool flag2 = this.m_CollectionView.HasValidDataAndBindings();
			if (flag2)
			{
				bool flag3 = flag || count != this.m_ActiveItems.Count;
				if (flag3)
				{
					bool flag4 = this.m_RefreshScrollOffsetScheduledItem == null;
					if (flag4)
					{
						this.m_RefreshScrollOffsetScheduledItem = this.m_CollectionView.schedule.Execute(new Action(this.RefreshScrollOffset));
					}
					else
					{
						bool flag5 = !this.m_RefreshScrollOffsetScheduledItem.isActive;
						if (flag5)
						{
							this.m_RefreshScrollOffsetScheduledItem.Resume();
						}
					}
				}
				this.ScheduleFill();
			}
		}

		public override void ScrollToItem(int index)
		{
			bool flag = index < -1;
			if (!flag)
			{
				bool flag2 = this.visibleItemCount == 0;
				if (flag2)
				{
					this.m_ScrolledToItemIndex = new int?(index);
				}
				else
				{
					base.ShouldDeferScrollToItem(index);
					float height = this.m_ScrollView.contentContainer.layout.height;
					float height2 = this.m_ScrollView.contentViewport.layout.height;
					Vector2 scrollOffset = this.m_ScrollView.scrollOffset;
					bool flag3 = index == -1;
					if (flag3)
					{
						this.m_ForcedLastVisibleItem = base.itemsCount - 1;
						this.m_ForcedFirstVisibleItem = -1;
						this.m_StickToBottom = true;
						this.m_ScrollView.scrollOffset = new Vector2(0f, (height2 >= height) ? 0f : height);
					}
					else
					{
						bool flag4 = this.firstVisibleIndex >= index;
						if (flag4)
						{
							Vector2 vector = new Vector2(0f, this.GetContentHeightForIndex(index - 1));
							bool flag5 = vector == this.m_ScrollView.scrollOffset;
							if (flag5)
							{
								return;
							}
							this.m_ForcedFirstVisibleItem = index;
							this.m_ForcedLastVisibleItem = -1;
							this.m_ScrollView.scrollOffset = vector;
						}
						else
						{
							float contentHeightForIndex = this.GetContentHeightForIndex(index);
							bool flag6 = float.IsNaN(height2) || contentHeightForIndex < this.contentPadding + height2;
							if (flag6)
							{
								return;
							}
							float y = contentHeightForIndex - height2 + 22f;
							this.m_ForcedLastVisibleItem = index;
							this.m_ForcedFirstVisibleItem = -1;
							this.m_ScrollView.scrollOffset = new Vector2(0f, y);
						}
					}
					bool flag7 = scrollOffset == this.m_ScrollView.scrollOffset;
					if (flag7)
					{
						this.OnScrollUpdate();
					}
				}
			}
		}

		public override void Resize(Vector2 size)
		{
			float expectedContentHeight = this.GetExpectedContentHeight();
			this.contentHeight = Mathf.Max(expectedContentHeight, this.contentHeight);
			float height = this.m_ScrollView.contentViewport.layout.height;
			float num = Mathf.Max(0f, this.contentHeight - height);
			float valueWithoutNotify = Mathf.Min(base.serializedData.scrollOffset.y, num);
			this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num);
			this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(valueWithoutNotify);
			base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.value;
			float num2 = this.m_CollectionView.ResolveItemHeight(size.y);
			int num3 = Mathf.CeilToInt(num2 / this.defaultExpectedHeight);
			int num4 = num3;
			bool flag = num4 <= 0;
			if (!flag)
			{
				num4 += 2;
				int num5 = Mathf.Min(num4, base.itemsCount);
				bool flag2 = this.m_ActiveItems.Count != num5;
				if (flag2)
				{
					int count = this.m_ActiveItems.Count;
					bool flag3 = count > num5;
					if (flag3)
					{
						int num6 = count - num5;
						for (int i = 0; i < num6; i++)
						{
							int activeItemsIndex = this.m_ActiveItems.Count - 1;
							this.ReleaseItem(activeItemsIndex);
						}
					}
					else
					{
						int num7 = num5 - this.m_ActiveItems.Count;
						int num8 = (this.firstVisibleIndex < 0) ? 0 : this.firstVisibleIndex;
						for (int j = 0; j < num7; j++)
						{
							int num9 = j + num8 + count;
							T orMakeItemAtIndex = this.GetOrMakeItemAtIndex(-1, -1);
							bool flag4 = this.IsIndexOutOfBounds(num9);
							if (flag4)
							{
								this.HideItem(this.m_ActiveItems.Count - 1);
							}
							else
							{
								base.Setup(orMakeItemAtIndex, num9);
								this.MarkWaitingForLayout(orMakeItemAtIndex);
							}
						}
					}
				}
				long num10 = DateTime.UtcNow.Ticks / 10000L;
				bool flag5 = (float)(num10 - this.m_TimeSinceFillScheduledMs) > 100f && this.m_TimeSinceFillScheduledMs != 0L && !this.m_FillExecuted;
				if (flag5)
				{
					this.Fill();
					this.ResetScroll();
					this.m_TimeSinceFillScheduledMs = 0L;
				}
				else
				{
					bool flag6 = this.m_TimeSinceFillScheduledMs == 0L;
					if (flag6)
					{
						this.m_TimeSinceFillScheduledMs = DateTime.UtcNow.Ticks / 10000L;
					}
					this.ScheduleFill();
					this.ScheduleScrollDirectionReset();
					this.m_FillExecuted = false;
				}
				this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
			}
		}

		public override void OnScroll(Vector2 scrollOffset)
		{
			bool flag = this.m_DelayedScrollOffset == scrollOffset;
			if (!flag)
			{
				this.m_DelayedScrollOffset = scrollOffset;
				bool flag2 = this.m_ForcedFirstVisibleItem != -1 || this.m_ForcedLastVisibleItem != -1;
				if (flag2)
				{
					this.OnScrollUpdate();
					this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.ForcedScroll;
				}
				else
				{
					DynamicHeightVirtualizationController<T>.VirtualizationChange lastChange = this.m_LastChange;
					bool flag3 = lastChange == DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize || lastChange == DynamicHeightVirtualizationController<T>.VirtualizationChange.ForcedScroll;
					if (flag3)
					{
						float height = this.m_ScrollView.contentViewport.layout.height;
						float num = Mathf.Max(0f, this.contentHeight - height);
						float valueWithoutNotify = Mathf.Min(this.m_ScrollView.scrollOffset.y, num);
						this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num);
						this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(valueWithoutNotify);
					}
					else
					{
						base.ScheduleScroll();
					}
				}
			}
		}

		private void OnDetachFromPanelEvent(DetachFromPanelEvent evt)
		{
			IVisualElementScheduledItem scheduledItem = this.m_ScheduledItem;
			bool flag = scheduledItem != null && scheduledItem.isActive;
			if (flag)
			{
				this.m_ScheduledItem.Pause();
				this.m_ScheduledItem = null;
			}
			IVisualElementScheduledItem scrollResetScheduledItem = this.m_ScrollResetScheduledItem;
			bool flag2 = scrollResetScheduledItem != null && scrollResetScheduledItem.isActive;
			if (flag2)
			{
				this.m_ScrollResetScheduledItem.Pause();
				this.m_ScrollResetScheduledItem = null;
			}
		}

		protected override void OnScrollUpdate()
		{
			Vector2 vector = float.IsNegativeInfinity(this.m_DelayedScrollOffset.y) ? this.m_ScrollView.scrollOffset : this.m_DelayedScrollOffset;
			bool flag = float.IsNaN(this.m_ScrollView.contentViewport.layout.height) || float.IsNaN(vector.y);
			if (!flag)
			{
				this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.Scroll;
				float expectedContentHeight = this.GetExpectedContentHeight();
				this.contentHeight = Mathf.Max(expectedContentHeight, this.contentHeight);
				this.m_ScrollDirection = ((vector.y < this.m_ScrollView.scrollOffset.y) ? DynamicHeightVirtualizationController<T>.ScrollDirection.Up : DynamicHeightVirtualizationController<T>.ScrollDirection.Down);
				float num = Mathf.Max(0f, this.contentHeight - this.m_ScrollView.contentViewport.layout.height);
				bool flag2 = vector.y <= 0f;
				if (flag2)
				{
					this.m_ForcedFirstVisibleItem = 0;
				}
				this.m_StickToBottom = (num > 0f && Math.Abs(vector.y - this.m_ScrollView.verticalScroller.highValue) < float.Epsilon);
				this.m_ScrollView.SetScrollOffsetWithoutNotify(vector);
				base.serializedData.scrollOffset = this.m_ScrollView.scrollOffset;
				this.m_CollectionView.SaveViewData();
				int num2 = (this.m_ForcedFirstVisibleItem != -1) ? this.m_ForcedFirstVisibleItem : this.GetFirstVisibleItem(this.m_ScrollView.scrollOffset.y);
				float contentHeightForIndex = this.GetContentHeightForIndex(num2 - 1);
				this.contentPadding = contentHeightForIndex;
				this.m_ForcedFirstVisibleItem = -1;
				bool flag3 = num2 != this.firstVisibleIndex;
				if (flag3)
				{
					this.CycleItems(num2);
				}
				else
				{
					this.Fill();
				}
				this.ScheduleScrollDirectionReset();
				this.m_DelayedScrollOffset = Vector2.negativeInfinity;
			}
		}

		private void CycleItems(int firstIndex)
		{
			bool flag = firstIndex == this.firstVisibleIndex;
			if (!flag)
			{
				T firstVisibleItem = base.firstVisibleItem;
				this.contentPadding = this.GetContentHeightForIndex(firstIndex - 1);
				this.firstVisibleIndex = firstIndex;
				bool flag2 = this.m_ActiveItems.Count > 0;
				if (flag2)
				{
					bool flag3 = firstVisibleItem == null || this.m_ActiveItems.Count <= Mathf.Abs(this.firstVisibleIndex - firstVisibleItem.index);
					if (!flag3)
					{
						bool flag4 = this.firstVisibleIndex < firstVisibleItem.index;
						if (flag4)
						{
							int num = firstVisibleItem.index - this.firstVisibleIndex;
							List<T> scrollInsertionList = this.m_ScrollInsertionList;
							for (int i = 0; i < num; i++)
							{
								List<T> activeItems = this.m_ActiveItems;
								T t = activeItems[activeItems.Count - 1];
								scrollInsertionList.Insert(0, t);
								this.m_ActiveItems.RemoveAt(this.m_ActiveItems.Count - 1);
								t.rootElement.SendToBack();
							}
							this.m_ActiveItems.InsertRange(0, scrollInsertionList);
							this.m_ScrollInsertionList.Clear();
						}
						else
						{
							List<T> scrollInsertionList2 = this.m_ScrollInsertionList;
							int num2 = 0;
							int num3 = -1;
							while (this.firstVisibleIndex > this.m_ActiveItems[num2].index && num3 < this.m_ActiveItems[num2].index)
							{
								T t2 = this.m_ActiveItems[num2];
								num3 = t2.index;
								scrollInsertionList2.Add(t2);
								num2++;
								t2.rootElement.BringToFront();
							}
							this.m_ActiveItems.RemoveRange(0, num2);
							this.m_ActiveItems.AddRange(scrollInsertionList2);
							this.m_ScrollInsertionList.Clear();
						}
					}
					float num4 = this.contentPadding;
					for (int j = 0; j < this.m_ActiveItems.Count; j++)
					{
						T t3 = this.m_ActiveItems[j];
						int num5 = this.firstVisibleIndex + j;
						int index = t3.index;
						bool flag5 = t3.rootElement.style.display == DisplayStyle.Flex;
						this.m_WaitingCache.Remove(index);
						bool flag6 = this.IsIndexOutOfBounds(num5);
						if (flag6)
						{
							this.HideItem(j);
						}
						else
						{
							base.Setup(t3, num5);
							bool flag7 = num4 > this.viewportMaxOffset;
							bool flag8 = flag7;
							if (flag8)
							{
								this.HideItem(j);
							}
							else
							{
								bool flag9 = num5 != index || !flag5;
								if (flag9)
								{
									this.MarkWaitingForLayout(t3);
								}
							}
							num4 += this.GetExpectedItemHeight(num5);
						}
					}
				}
				bool flag10 = this.m_LastChange != DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
				if (flag10)
				{
					this.UpdateAnchor();
				}
				this.ScheduleFill();
			}
		}

		private bool NeedsFill()
		{
			bool flag = this.m_LastChange != DynamicHeightVirtualizationController<T>.VirtualizationChange.None || this.anchoredIndex < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				T t = base.lastVisibleItem;
				int num = (t != null) ? t.index : -1;
				float num2 = this.contentPadding;
				bool flag2 = num2 > this.m_ScrollView.scrollOffset.y;
				if (flag2)
				{
					result = true;
				}
				else
				{
					for (int i = this.firstVisibleIndex; i < base.itemsCount; i++)
					{
						bool flag3 = num2 > this.viewportMaxOffset || (num2 == this.viewportMaxOffset && !this.m_StickToBottom);
						if (flag3)
						{
							break;
						}
						num2 += this.GetExpectedItemHeight(i);
						bool flag4 = i > num;
						if (flag4)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		private void Fill()
		{
			bool flag = !this.m_CollectionView.HasValidDataAndBindings() || this.firstVisibleIndex < 0;
			if (!flag)
			{
				this.m_FillExecuted = true;
				bool flag2 = this.m_ActiveItems.Count == 0;
				if (flag2)
				{
					this.contentHeight = 0f;
					this.contentPadding = 0f;
				}
				else
				{
					bool flag3 = this.anchoredIndex < 0;
					if (!flag3)
					{
						bool flag4 = this.contentPadding > this.contentHeight;
						if (flag4)
						{
							this.OnScrollUpdate();
						}
						else
						{
							float num = this.contentPadding;
							float num2 = this.contentPadding;
							int num3 = 0;
							for (int i = this.firstVisibleIndex; i < base.itemsCount; i++)
							{
								bool flag5 = num2 > this.viewportMaxOffset || (num2 == this.viewportMaxOffset && !this.m_StickToBottom);
								if (flag5)
								{
									break;
								}
								num2 += this.GetExpectedItemHeight(i);
								T t = this.m_ActiveItems[num3++];
								bool flag6 = t.index != i || t.rootElement.style.display == DisplayStyle.None;
								if (flag6)
								{
									base.Setup(t, i);
									this.MarkWaitingForLayout(t);
								}
								bool flag7 = num3 >= this.m_ActiveItems.Count;
								if (flag7)
								{
									break;
								}
							}
							bool flag8 = this.firstVisibleIndex > 0 && this.contentPadding > this.m_ScrollView.scrollOffset.y;
							if (flag8)
							{
								List<T> scrollInsertionList = this.m_ScrollInsertionList;
								for (int j = this.m_ActiveItems.Count - 1; j >= num3; j--)
								{
									bool flag9 = this.firstVisibleIndex == 0;
									if (flag9)
									{
										break;
									}
									T t2 = this.m_ActiveItems[j];
									scrollInsertionList.Insert(0, t2);
									this.m_ActiveItems.RemoveAt(this.m_ActiveItems.Count - 1);
									t2.rootElement.SendToBack();
									int num4 = this.firstVisibleIndex - 1;
									this.firstVisibleIndex = num4;
									int num5 = num4;
									base.Setup(t2, num5);
									this.MarkWaitingForLayout(t2);
									num -= this.GetExpectedItemHeight(num5);
									bool flag10 = num < this.m_ScrollView.scrollOffset.y;
									if (flag10)
									{
										break;
									}
								}
								this.m_ActiveItems.InsertRange(0, scrollInsertionList);
								this.m_ScrollInsertionList.Clear();
							}
							this.contentPadding = num;
							this.contentHeight = this.GetExpectedContentHeight();
							bool flag11 = this.m_LastChange != DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
							if (flag11)
							{
								this.UpdateAnchor();
							}
							bool flag12 = this.m_WaitingCache.Count == 0;
							if (flag12)
							{
								this.m_ScrollView.scrollOffset = base.serializedData.scrollOffset;
								this.ResetScroll();
								this.ApplyScrollViewUpdate(true);
							}
						}
					}
				}
			}
		}

		private void UpdateScrollViewContainer(float previousHeight, float newHeight)
		{
			bool stickToBottom = this.m_StickToBottom;
			if (!stickToBottom)
			{
				bool flag = this.m_ForcedLastVisibleItem >= 0;
				if (flag)
				{
					float contentHeightForIndex = this.GetContentHeightForIndex(this.m_ForcedLastVisibleItem);
					this.m_ScrollView.SetScrollOffsetWithoutNotify(new Vector2(this.m_ScrollView.scrollOffset.x, contentHeightForIndex + 22f - this.m_ScrollView.contentViewport.layout.height));
				}
				else
				{
					bool flag2 = this.m_ScrollDirection == DynamicHeightVirtualizationController<T>.ScrollDirection.Up;
					if (flag2)
					{
						this.m_ScrollView.SetScrollOffsetWithoutNotify(new Vector2(this.m_ScrollView.scrollOffset.x, this.m_ScrollView.scrollOffset.y + (newHeight - previousHeight)));
					}
				}
			}
		}

		private void ApplyScrollViewUpdate(bool dimensionsOnly = false)
		{
			float contentPadding = this.contentPadding;
			float y = this.m_ScrollView.scrollOffset.y;
			float num = y - contentPadding;
			bool flag = this.anchoredIndex >= 0;
			if (flag)
			{
				bool flag2 = this.firstVisibleIndex != this.anchoredIndex;
				if (flag2)
				{
					this.CycleItems(this.anchoredIndex);
					this.ScheduleFill();
				}
				this.firstVisibleIndex = this.anchoredIndex;
				num = this.anchorOffset;
			}
			float expectedContentHeight = this.GetExpectedContentHeight();
			this.contentHeight = expectedContentHeight;
			this.contentPadding = this.GetContentHeightForIndex(this.firstVisibleIndex - 1);
			float num2 = Mathf.Max(0f, this.m_ScrollView.RoundToPanelPixelSize(expectedContentHeight - this.m_ScrollView.contentViewport.layout.height));
			float valueWithoutNotify = Mathf.Min(this.contentPadding + num, num2);
			bool flag3 = this.m_StickToBottom && num2 > 0f;
			if (flag3)
			{
				valueWithoutNotify = num2;
			}
			else
			{
				bool flag4 = this.m_ForcedLastVisibleItem != -1;
				if (flag4)
				{
					float contentHeightForIndex = this.GetContentHeightForIndex(this.m_ForcedLastVisibleItem);
					float value = contentHeightForIndex + 22f - this.m_ScrollView.contentViewport.layout.height;
					valueWithoutNotify = Mathf.Clamp(value, 0f, num2);
					this.m_ForcedLastVisibleItem = -1;
				}
			}
			this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(num2);
			this.m_ScrollView.verticalScroller.slider.SetValueWithoutNotify(valueWithoutNotify);
			base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.slider.value;
			bool flag5 = dimensionsOnly || this.m_LastChange == DynamicHeightVirtualizationController<T>.VirtualizationChange.Resize;
			if (flag5)
			{
				this.ScheduleScrollDirectionReset();
			}
			else
			{
				bool flag6 = this.NeedsFill();
				if (flag6)
				{
					this.Fill();
				}
				else
				{
					float num3 = this.contentPadding;
					int firstVisibleIndex = this.firstVisibleIndex;
					List<T> scrollInsertionList = this.m_ScrollInsertionList;
					int num4 = 0;
					for (int i = 0; i < this.m_ActiveItems.Count; i++)
					{
						T t = this.m_ActiveItems[i];
						int index = t.index;
						bool flag7 = index < 0;
						if (flag7)
						{
							break;
						}
						float expectedItemHeight = this.GetExpectedItemHeight(index);
						bool flag8 = this.m_ActiveItems[i].rootElement.style.display == DisplayStyle.Flex;
						if (flag8)
						{
							bool flag9 = num3 + expectedItemHeight < this.m_ScrollView.scrollOffset.y;
							if (flag9)
							{
								t.rootElement.BringToFront();
								this.HideItem(i);
								scrollInsertionList.Add(t);
								num4++;
								int firstVisibleIndex2 = this.firstVisibleIndex;
								this.firstVisibleIndex = firstVisibleIndex2 + 1;
							}
							else
							{
								bool flag10 = num3 > this.viewportMaxOffset;
								if (flag10)
								{
									this.HideItem(i);
								}
							}
						}
						num3 += this.GetExpectedItemHeight(index);
					}
					this.m_ActiveItems.RemoveRange(0, num4);
					this.m_ActiveItems.AddRange(scrollInsertionList);
					this.m_ScrollInsertionList.Clear();
					bool flag11 = this.firstVisibleIndex != firstVisibleIndex;
					if (flag11)
					{
						this.contentPadding = this.GetContentHeightForIndex(this.firstVisibleIndex - 1);
						this.UpdateAnchor();
					}
					this.ScheduleScrollDirectionReset();
					this.m_CollectionView.SaveViewData();
				}
				base.ScheduleDeferredScrollToItem();
			}
		}

		private void UpdateAnchor()
		{
			this.anchoredIndex = this.firstVisibleIndex;
			this.anchorOffset = this.m_ScrollView.scrollOffset.y - this.contentPadding;
		}

		private void ScheduleFill()
		{
			bool flag = this.m_ScheduledItem == null;
			if (flag)
			{
				this.m_ScheduledItem = this.m_CollectionView.schedule.Execute(this.m_FillCallback);
			}
			else
			{
				this.m_ScheduledItem.Pause();
				this.m_ScheduledItem.Resume();
			}
		}

		private void ScheduleScrollDirectionReset()
		{
			bool flag = this.m_ScrollResetScheduledItem == null;
			if (flag)
			{
				this.m_ScrollResetScheduledItem = this.m_CollectionView.schedule.Execute(this.m_ScrollResetCallback);
			}
			else
			{
				this.m_ScrollResetScheduledItem.Pause();
				this.m_ScrollResetScheduledItem.Resume();
			}
		}

		private void ResetScroll()
		{
			this.m_ScrollDirection = DynamicHeightVirtualizationController<T>.ScrollDirection.Idle;
			this.m_LastChange = DynamicHeightVirtualizationController<T>.VirtualizationChange.None;
			this.m_ScrollView.UpdateContentViewTransform();
			this.UpdateAnchor();
			this.m_CollectionView.SaveViewData();
		}

		public override int GetIndexFromPosition(Vector2 position)
		{
			int num = 0;
			for (float num2 = 0f; num2 < position.y; num2 += this.GetExpectedItemHeight(num++))
			{
			}
			return num - 1;
		}

		public override float GetExpectedItemHeight(int index)
		{
			int draggedIndex = base.GetDraggedIndex();
			bool flag = draggedIndex >= 0 && index == draggedIndex;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				float num;
				result = (this.m_ItemHeightCache.TryGetValue(index, out num) ? num : this.defaultExpectedHeight);
			}
			return result;
		}

		private int GetFirstVisibleItem(float offset)
		{
			bool flag = offset <= 0f;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int num = -1;
				while (offset > 0f)
				{
					num++;
					float expectedItemHeight = this.GetExpectedItemHeight(num);
					offset -= expectedItemHeight;
				}
				result = num;
			}
			return result;
		}

		public override float GetExpectedContentHeight()
		{
			return this.m_AccumulatedHeight + (float)(base.itemsCount - this.m_ItemHeightCache.Count) * this.defaultExpectedHeight;
		}

		private float GetContentHeightForIndex(int lastIndex)
		{
			DynamicHeightVirtualizationController<T>.<>c__DisplayClass67_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			bool flag = lastIndex < 0;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				CS$<>8__locals1.draggedIndex = base.GetDraggedIndex();
				DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo;
				bool flag2 = this.m_HighestCachedIndex <= lastIndex && this.m_ContentHeightCache.TryGetValue(this.m_HighestCachedIndex, out contentHeightCacheInfo);
				if (flag2)
				{
					result = this.<GetContentHeightForIndex>g__GetContentHeightFromCachedHeight|67_0(lastIndex, contentHeightCacheInfo, ref CS$<>8__locals1);
				}
				else
				{
					float num = 0f;
					for (int i = lastIndex; i >= 0; i--)
					{
						DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo2;
						bool flag3 = this.m_ContentHeightCache.TryGetValue(i, out contentHeightCacheInfo2);
						if (flag3)
						{
							return num + this.<GetContentHeightForIndex>g__GetContentHeightFromCachedHeight|67_0(i, contentHeightCacheInfo2, ref CS$<>8__locals1);
						}
						num += ((CS$<>8__locals1.draggedIndex == i) ? 0f : this.defaultExpectedHeight);
					}
					result = num;
				}
			}
			return result;
		}

		private DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo GetCachedContentHeight(int index)
		{
			while (index >= 0)
			{
				DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo result;
				bool flag = this.m_ContentHeightCache.TryGetValue(index, out result);
				if (flag)
				{
					return result;
				}
				index--;
			}
			return default(DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo);
		}

		private void RegisterItemHeight(int index, float height)
		{
			bool flag = height <= 0f;
			if (!flag)
			{
				float num = this.m_CollectionView.ResolveItemHeight(height);
				float num2;
				bool flag2 = this.m_ItemHeightCache.TryGetValue(index, out num2);
				if (flag2)
				{
					this.m_AccumulatedHeight -= num2;
				}
				this.m_AccumulatedHeight += num;
				this.m_ItemHeightCache[index] = num;
				bool flag3 = index > this.m_HighestCachedIndex;
				if (flag3)
				{
					this.m_HighestCachedIndex = index;
				}
				bool flag4 = num2 == 0f;
				DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo cachedContentHeight = this.GetCachedContentHeight(index - 1);
				this.m_ContentHeightCache[index] = new DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo(cachedContentHeight.sum + num, cachedContentHeight.count + 1);
				foreach (KeyValuePair<int, float> keyValuePair in this.m_ItemHeightCache)
				{
					bool flag5 = keyValuePair.Key > index;
					if (flag5)
					{
						DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo = this.m_ContentHeightCache[keyValuePair.Key];
						this.m_ContentHeightCache[keyValuePair.Key] = new DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo(contentHeightCacheInfo.sum - num2 + num, flag4 ? (contentHeightCacheInfo.count + 1) : contentHeightCacheInfo.count);
					}
				}
			}
		}

		private void UnregisterItemHeight(int index)
		{
			float num;
			bool flag = !this.m_ItemHeightCache.TryGetValue(index, out num);
			if (!flag)
			{
				this.m_AccumulatedHeight -= num;
				this.m_ItemHeightCache.Remove(index);
				this.m_ContentHeightCache.Remove(index);
				int num2 = -1;
				foreach (KeyValuePair<int, float> keyValuePair in this.m_ItemHeightCache)
				{
					bool flag2 = keyValuePair.Key > index;
					if (flag2)
					{
						DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo contentHeightCacheInfo = this.m_ContentHeightCache[keyValuePair.Key];
						this.m_ContentHeightCache[keyValuePair.Key] = new DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo(contentHeightCacheInfo.sum - num, contentHeightCacheInfo.count - 1);
					}
					bool flag3 = keyValuePair.Key > num2;
					if (flag3)
					{
						num2 = keyValuePair.Key;
					}
				}
				this.m_HighestCachedIndex = num2;
			}
		}

		private void CleanItemHeightCache()
		{
			bool flag = !this.IsIndexOutOfBounds(this.m_HighestCachedIndex);
			if (!flag)
			{
				List<int> list = CollectionPool<List<int>, int>.Get();
				try
				{
					foreach (int num in this.m_ItemHeightCache.Keys)
					{
						bool flag2 = this.IsIndexOutOfBounds(num);
						if (flag2)
						{
							list.Add(num);
						}
					}
					foreach (int index in list)
					{
						this.UnregisterItemHeight(index);
					}
				}
				finally
				{
					CollectionPool<List<int>, int>.Release(list);
				}
				this.m_MinimumItemHeight = -1f;
			}
		}

		private void OnRecycledItemGeometryChanged(ReusableCollectionItem item)
		{
			bool flag = item.index == -1 || item.isDragGhost || float.IsNaN(item.rootElement.layout.height) || item.rootElement.layout.height == 0f;
			if (!flag)
			{
				bool flag2 = this.UpdateRegisteredHeight(item);
				if (flag2)
				{
					this.ApplyScrollViewUpdate(false);
				}
			}
		}

		private bool UpdateRegisteredHeight(ReusableCollectionItem item)
		{
			bool flag = item.index == -1 || item.isDragGhost || float.IsNaN(item.rootElement.layout.height) || item.rootElement.layout.height == 0f;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = item.rootElement.layout.height < this.defaultExpectedHeight;
				if (flag2)
				{
					this.m_MinimumItemHeight = item.rootElement.layout.height;
					this.Resize(this.m_ScrollView.layout.size);
				}
				float num = item.rootElement.layout.height - item.rootElement.resolvedStyle.paddingTop;
				float b;
				bool flag3 = this.m_ItemHeightCache.TryGetValue(item.index, out b);
				float num2 = flag3 ? this.GetExpectedItemHeight(item.index) : this.defaultExpectedHeight;
				bool flag4 = this.m_WaitingCache.Count == 0;
				if (flag4)
				{
					bool flag5 = num > num2;
					if (flag5)
					{
						this.m_StickToBottom = false;
					}
					else
					{
						float num3 = num - num2;
						float num4 = Mathf.Max(0f, this.contentHeight - this.m_ScrollView.contentViewport.layout.height);
						this.m_StickToBottom = (num4 > 0f && this.m_ScrollView.scrollOffset.y >= this.m_ScrollView.verticalScroller.highValue + num3);
					}
				}
				bool flag6 = !flag3 || !Mathf.Approximately(num, b);
				if (flag6)
				{
					this.RegisterItemHeight(item.index, num);
					this.UpdateScrollViewContainer(num2, num);
					bool flag7 = this.m_WaitingCache.Count == 0;
					if (flag7)
					{
						return true;
					}
				}
				result = (this.m_WaitingCache.Remove(item.index) && this.m_WaitingCache.Count == 0);
			}
			return result;
		}

		internal override T GetOrMakeItemAtIndex(int activeItemIndex = -1, int scrollViewIndex = -1)
		{
			T orMakeItemAtIndex = base.GetOrMakeItemAtIndex(activeItemIndex, scrollViewIndex);
			orMakeItemAtIndex.onGeometryChanged += this.m_GeometryChangedCallback;
			return orMakeItemAtIndex;
		}

		internal override void ReleaseItem(int activeItemsIndex)
		{
			T t = this.m_ActiveItems[activeItemsIndex];
			t.onGeometryChanged -= this.m_GeometryChangedCallback;
			int index = t.index;
			this.UnregisterItemHeight(index);
			base.ReleaseItem(activeItemsIndex);
			this.m_WaitingCache.Remove(index);
		}

		internal override void StartDragItem(ReusableCollectionItem item)
		{
			this.m_WaitingCache.Remove(item.index);
			base.StartDragItem(item);
			this.m_DraggedItem.onGeometryChanged -= this.m_GeometryChangedCallback;
		}

		internal override void EndDrag(int dropIndex)
		{
			bool flag = this.m_DraggedItem.index < dropIndex;
			int index = this.m_DraggedItem.index;
			int num = flag ? 1 : -1;
			float expectedItemHeight = this.GetExpectedItemHeight(index);
			for (int num2 = index; num2 != dropIndex; num2 += num)
			{
				float expectedItemHeight2 = this.GetExpectedItemHeight(num2);
				float expectedItemHeight3 = this.GetExpectedItemHeight(num2 + num);
				bool flag2 = Mathf.Approximately(expectedItemHeight2, expectedItemHeight3);
				if (!flag2)
				{
					this.RegisterItemHeight(num2, expectedItemHeight3);
				}
			}
			this.RegisterItemHeight(flag ? (dropIndex - 1) : dropIndex, expectedItemHeight);
			bool flag3 = this.firstVisibleIndex > this.m_DraggedItem.index;
			if (flag3)
			{
				this.firstVisibleIndex = this.GetFirstVisibleItem(this.m_ScrollView.scrollOffset.y);
				this.UpdateAnchor();
			}
			this.m_DraggedItem.onGeometryChanged += this.m_GeometryChangedCallback;
			base.EndDrag(dropIndex);
		}

		private void HideItem(int activeItemsIndex)
		{
			T t = this.m_ActiveItems[activeItemsIndex];
			t.rootElement.style.display = DisplayStyle.None;
			this.m_WaitingCache.Remove(t.index);
		}

		private void MarkWaitingForLayout(T item)
		{
			bool isDragGhost = item.isDragGhost;
			if (!isDragGhost)
			{
				this.m_WaitingCache.Add(item.index);
				item.rootElement.lastLayout = Rect.zero;
				item.rootElement.MarkDirtyRepaint();
			}
		}

		private bool IsIndexOutOfBounds(int i)
		{
			return this.m_CollectionView.itemsSource == null || i >= base.itemsCount;
		}

		private void RefreshScrollOffset()
		{
			this.contentHeight = this.GetExpectedContentHeight();
			float highValueWithoutNotify = Mathf.Max(0f, this.contentHeight - this.m_ScrollView.contentViewport.layout.height);
			this.m_ScrollView.verticalScroller.slider.SetHighValueWithoutNotify(highValueWithoutNotify);
			this.m_ScrollView.scrollOffset = base.serializedData.scrollOffset;
			base.serializedData.scrollOffset.y = this.m_ScrollView.verticalScroller.value;
		}

		[CompilerGenerated]
		private float <GetContentHeightForIndex>g__GetContentHeightFromCachedHeight|67_0(int index, in DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo heightInfo, ref DynamicHeightVirtualizationController<T>.<>c__DisplayClass67_0 A_3)
		{
			bool flag = A_3.draggedIndex >= 0 && index >= A_3.draggedIndex;
			float result;
			if (flag)
			{
				result = heightInfo.sum + (float)(index - heightInfo.count + 1) * this.defaultExpectedHeight - this.m_DraggedItem.rootElement.layout.height;
			}
			else
			{
				result = heightInfo.sum + (float)(index - heightInfo.count + 1) * this.defaultExpectedHeight;
			}
			return result;
		}

		private int m_HighestCachedIndex = -1;

		private readonly Dictionary<int, float> m_ItemHeightCache = new Dictionary<int, float>(32);

		private readonly Dictionary<int, DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo> m_ContentHeightCache = new Dictionary<int, DynamicHeightVirtualizationController<T>.ContentHeightCacheInfo>(32);

		private readonly HashSet<int> m_WaitingCache = new HashSet<int>(32);

		private int? m_ScrolledToItemIndex;

		private int m_ForcedFirstVisibleItem = -1;

		private int m_ForcedLastVisibleItem = -1;

		private bool m_StickToBottom;

		private DynamicHeightVirtualizationController<T>.VirtualizationChange m_LastChange;

		private DynamicHeightVirtualizationController<T>.ScrollDirection m_ScrollDirection;

		private Vector2 m_DelayedScrollOffset = Vector2.negativeInfinity;

		private float m_AccumulatedHeight;

		private float m_MinimumItemHeight = -1f;

		private Action m_FillCallback;

		private Action m_ScrollResetCallback;

		private Action<ReusableCollectionItem> m_GeometryChangedCallback;

		private IVisualElementScheduledItem m_ScheduledItem;

		private IVisualElementScheduledItem m_ScrollResetScheduledItem;

		private IVisualElementScheduledItem m_RefreshScrollOffsetScheduledItem;

		private Predicate<int> m_IndexOutOfBoundsPredicate;

		private bool m_FillExecuted;

		private long m_TimeSinceFillScheduledMs;

		private const float k_ForceRefreshIntervalInMilliseconds = 100f;

		private readonly struct ContentHeightCacheInfo
		{
			public ContentHeightCacheInfo(float sum, int count)
			{
				this.sum = sum;
				this.count = count;
			}

			public readonly float sum;

			public readonly int count;
		}

		private enum VirtualizationChange
		{
			None,
			Resize,
			Scroll,
			ForcedScroll
		}

		private enum ScrollDirection
		{
			Idle,
			Up,
			Down
		}
	}
}
