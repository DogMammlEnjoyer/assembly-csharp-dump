using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal class UIRLayoutUpdater : BaseVisualTreeUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return UIRLayoutUpdater.s_ProfilerMarker;
			}
		}

		public unsafe override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Layout)) == (VersionChangeType)0;
			if (!flag)
			{
				LayoutNode lhs = *ve.layoutNode;
				bool flag2 = lhs != LayoutNode.Undefined && lhs.UsesMeasure;
				if (flag2)
				{
					lhs.MarkDirty();
				}
			}
		}

		public override void Update()
		{
			int num = 0;
			bool isDirty = base.visualTree.layoutNode.IsDirty;
			if (isDirty)
			{
				this.missedHierarchyChangeEventsList.Clear();
				while (base.visualTree.layoutNode.IsDirty)
				{
					this.changeEventsList.Clear();
					bool flag = num > 0;
					if (flag)
					{
						base.panel.ApplyStyles();
					}
					base.panel.duringLayoutPhase = true;
					base.visualTree.layoutNode.CalculateLayout(float.NaN, float.NaN);
					base.panel.duringLayoutPhase = false;
					this.UpdateSubTree(base.visualTree, this.changeEventsList);
					this.DispatchChangeEvents(this.changeEventsList, num);
					bool flag2 = !base.visualTree.layoutNode.IsDirty;
					if (flag2)
					{
						this.DispatchMissedHierarchyChangeEvents(this.missedHierarchyChangeEventsList, num);
						this.missedHierarchyChangeEventsList.Clear();
					}
					bool flag3 = num++ >= 10;
					if (flag3)
					{
						string str = "Layout update is struggling to process current layout (consider simplifying to avoid recursive layout): ";
						VisualElement visualTree = base.visualTree;
						Debug.LogError(str + ((visualTree != null) ? visualTree.ToString() : null));
						break;
					}
				}
			}
			base.visualTree.focusController.ReevaluateFocus();
		}

		private static bool UpdateHierarchyDisplayed(VisualElement ve, List<ValueTuple<Rect, Rect, VisualElement>> changeEvents, bool inheritedDisplayed = true)
		{
			bool flag = inheritedDisplayed & ve.resolvedStyle.display != DisplayStyle.None;
			bool flag2 = inheritedDisplayed && !flag;
			if (flag2)
			{
				ve.disableRendering = true;
			}
			else
			{
				bool flag3 = flag;
				if (flag3)
				{
					ve.disableRendering = false;
				}
			}
			bool flag4 = ve.areAncestorsAndSelfDisplayed == flag;
			bool result;
			if (flag4)
			{
				result = false;
			}
			else
			{
				ve.areAncestorsAndSelfDisplayed = flag;
				bool flag5 = !flag;
				if (flag5)
				{
					if (inheritedDisplayed)
					{
						ve.IncrementVersion(VersionChangeType.Size);
					}
					bool flag6 = ve.HasSelfEventInterests(EventBase<GeometryChangedEvent>.EventCategory);
					if (flag6)
					{
						changeEvents.Add(new ValueTuple<Rect, Rect, VisualElement>(ve.lastLayout, Rect.zero, ve));
					}
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						UIRLayoutUpdater.UpdateHierarchyDisplayed(ve.hierarchy[i], changeEvents, flag);
					}
				}
				result = true;
			}
			return result;
		}

		private void UpdateSubTree(VisualElement ve, List<ValueTuple<Rect, Rect, VisualElement>> changeEvents)
		{
			bool flag = UIRLayoutUpdater.UpdateHierarchyDisplayed(ve, changeEvents, true);
			bool flag2 = !ve.areAncestorsAndSelfDisplayed;
			if (!flag2)
			{
				Rect rect = new Rect(ve.layoutNode.LayoutX, ve.layoutNode.LayoutY, ve.layoutNode.LayoutWidth, ve.layoutNode.LayoutHeight);
				Rect rect2 = new Rect(ve.layoutNode.LayoutPaddingLeft, ve.layoutNode.LayoutPaddingLeft, ve.layoutNode.LayoutPaddingRight, ve.layoutNode.LayoutPaddingBottom);
				Rect lastPseudoPadding = new Rect(rect2.x, rect2.y, rect.width - (rect2.x + rect2.width), rect.height - (rect2.y + rect2.height));
				Rect lastLayout = ve.lastLayout;
				Rect lastPseudoPadding2 = ve.lastPseudoPadding;
				VersionChangeType versionChangeType = (VersionChangeType)0;
				bool flag3 = lastLayout.size != rect.size;
				bool flag4 = lastPseudoPadding2.size != lastPseudoPadding.size;
				bool flag5 = flag3 || flag4;
				if (flag5)
				{
					versionChangeType |= (VersionChangeType.Size | VersionChangeType.Repaint);
				}
				bool flag6 = rect.position != lastLayout.position;
				bool flag7 = lastPseudoPadding.position != lastPseudoPadding2.position;
				bool flag8 = flag6 || flag7 || flag;
				if (flag8)
				{
					versionChangeType |= VersionChangeType.Transform;
				}
				bool flag9 = flag;
				if (flag9)
				{
					versionChangeType |= VersionChangeType.Size;
				}
				bool flag10 = (versionChangeType & (VersionChangeType.Transform | VersionChangeType.Size)) == VersionChangeType.Size;
				if (flag10)
				{
					bool flag11 = !ve.hasDefaultRotationAndScale;
					if (flag11)
					{
						bool flag12 = !Mathf.Approximately(ve.resolvedStyle.transformOrigin.x, 0f) || !Mathf.Approximately(ve.resolvedStyle.transformOrigin.y, 0f);
						if (flag12)
						{
							versionChangeType |= VersionChangeType.Transform;
						}
					}
				}
				bool flag13 = versionChangeType > (VersionChangeType)0;
				if (flag13)
				{
					ve.IncrementVersion(versionChangeType);
				}
				ve.lastLayout = rect;
				ve.lastPseudoPadding = lastPseudoPadding;
				bool hasNewLayout = ve.layoutNode.HasNewLayout;
				bool flag14 = hasNewLayout;
				if (flag14)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						VisualElement visualElement = ve.hierarchy[i];
						bool hasNewLayout2 = visualElement.layoutNode.HasNewLayout;
						if (hasNewLayout2)
						{
							this.UpdateSubTree(visualElement, changeEvents);
						}
					}
				}
				bool flag15 = ve.HasSelfEventInterests(EventBase<GeometryChangedEvent>.EventCategory);
				if (flag15)
				{
					bool flag16 = flag3 || flag6 || flag;
					if (flag16)
					{
						changeEvents.Add(new ValueTuple<Rect, Rect, VisualElement>(flag ? Rect.zero : lastLayout, rect, ve));
						bool receivesHierarchyGeometryChangedEvents = ve.receivesHierarchyGeometryChangedEvents;
						if (receivesHierarchyGeometryChangedEvents)
						{
							this.missedHierarchyChangeEventsList.Remove(ve);
						}
					}
					else
					{
						bool flag17 = ve.receivesHierarchyGeometryChangedEvents && ve.boundingBoxDirtiedSinceLastLayoutPass;
						if (flag17)
						{
							this.missedHierarchyChangeEventsList.Add(ve);
						}
					}
				}
				ve.boundingBoxDirtiedSinceLastLayoutPass = false;
				bool flag18 = hasNewLayout;
				if (flag18)
				{
					ve.layoutNode.MarkLayoutSeen();
				}
			}
		}

		private void DispatchChangeEvents(List<ValueTuple<Rect, Rect, VisualElement>> changeEvents, int currentLayoutPass)
		{
			foreach (ValueTuple<Rect, Rect, VisualElement> valueTuple in changeEvents)
			{
				Rect item = valueTuple.Item1;
				Rect item2 = valueTuple.Item2;
				VisualElement item3 = valueTuple.Item3;
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(item, item2))
				{
					pooled.layoutPass = currentLayoutPass;
					pooled.elementTarget = item3;
					EventDispatchUtilities.HandleEventAtTargetAndDefaultPhase(pooled, base.panel, item3);
				}
			}
		}

		private void DispatchMissedHierarchyChangeEvents(List<VisualElement> missedHierarchyChangeEvents, int currentLayoutPass)
		{
			foreach (VisualElement visualElement in missedHierarchyChangeEvents)
			{
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(Rect.zero, visualElement.layout))
				{
					pooled.layoutPass = currentLayoutPass;
					pooled.elementTarget = visualElement;
					EventDispatchUtilities.HandleEventAtTargetAndDefaultPhase(pooled, base.panel, visualElement);
				}
			}
		}

		public const int kMaxValidateLayoutCount = 10;

		private static readonly string s_Description = "UIElements.UpdateLayout";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(UIRLayoutUpdater.s_Description);

		private static readonly ProfilerMarker k_ComputeLayoutMarker = new ProfilerMarker("LayoutUpdater.ComputeLayout");

		private static readonly ProfilerMarker k_UpdateSubTreeMarker = new ProfilerMarker("LayoutUpdater.UpdateSubTree");

		private static readonly ProfilerMarker k_DispatchChangeEventsMarker = new ProfilerMarker("LayoutUpdater.DispatchChangeEvents");

		private List<ValueTuple<Rect, Rect, VisualElement>> changeEventsList = new List<ValueTuple<Rect, Rect, VisualElement>>();

		private List<VisualElement> missedHierarchyChangeEventsList = new List<VisualElement>();
	}
}
