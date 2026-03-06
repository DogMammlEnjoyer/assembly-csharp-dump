using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.UIBuilderModule"
	})]
	internal class Panel : BaseVisualElementPanel
	{
		public sealed override VisualElement visualTree
		{
			get
			{
				return this.m_RootContainer;
			}
		}

		internal Panel.UIFrameState GetFrameState()
		{
			return new Panel.UIFrameState(this);
		}

		public sealed override EventDispatcher dispatcher { get; set; }

		public TimerEventScheduler timerEventScheduler
		{
			get
			{
				TimerEventScheduler result;
				if ((result = this.m_Scheduler) == null)
				{
					result = (this.m_Scheduler = new TimerEventScheduler());
				}
				return result;
			}
		}

		internal override IScheduler scheduler
		{
			get
			{
				return this.timerEventScheduler;
			}
		}

		internal VisualTreeUpdater visualTreeUpdater
		{
			get
			{
				return this.m_VisualTreeUpdater;
			}
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal override IStylePropertyAnimationSystem styleAnimationSystem
		{
			get
			{
				return this.m_StylePropertyAnimationSystem;
			}
			set
			{
				bool flag = this.m_StylePropertyAnimationSystem == value;
				if (!flag)
				{
					try
					{
						IStylePropertyAnimationSystem stylePropertyAnimationSystem = this.m_StylePropertyAnimationSystem;
						if (stylePropertyAnimationSystem != null)
						{
							stylePropertyAnimationSystem.CancelAllAnimations();
						}
					}
					finally
					{
						this.m_StylePropertyAnimationSystem = value;
					}
				}
			}
		}

		public override ScriptableObject ownerObject { get; protected set; }

		public override ContextType contextType { get; }

		public override SavePersistentViewData saveViewData { get; set; }

		public override GetViewDataDictionary getViewDataDictionary { get; set; }

		public sealed override FocusController focusController { get; set; }

		public override EventInterests IMGUIEventInterests { get; set; }

		internal static LoadResourceFunction loadResourceFunc { private get; set; }

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule"
		})]
		internal static Object LoadResource(string pathName, Type type, float dpiScaling)
		{
			bool flag = Panel.loadResourceFunc != null;
			Object result;
			if (flag)
			{
				result = Panel.loadResourceFunc(pathName, type, dpiScaling);
			}
			else
			{
				result = Resources.Load(pathName, type);
			}
			return result;
		}

		internal void Focus()
		{
			this.m_JustReceivedFocus = true;
		}

		internal void Blur()
		{
			FocusController focusController = this.focusController;
			if (focusController != null)
			{
				focusController.BlurLastFocusedElement();
			}
		}

		public void ValidateFocus()
		{
			bool justReceivedFocus = this.m_JustReceivedFocus;
			if (justReceivedFocus)
			{
				this.m_JustReceivedFocus = false;
				FocusController focusController = this.focusController;
				if (focusController != null)
				{
					focusController.SetFocusToLastFocusedElement();
				}
			}
		}

		internal string name
		{
			get
			{
				return this.m_PanelName;
			}
			set
			{
				this.m_PanelName = value;
				this.CreateMarkers();
			}
		}

		public IDebugPanelChangeReceiver panelChangeReceiver
		{
			get
			{
				return this.m_PanelChangeReceiver;
			}
			set
			{
				this.m_PanelChangeReceiver = value;
				bool flag = value != null;
				if (flag)
				{
					Debug.LogWarning("IPanelChangeReceiver suscribed to panel '" + this.name + "' and may affect performance. The callback should be used only in debugging scenario and won't work outside development builds");
				}
			}
		}

		private void CreateMarkers()
		{
			string str = string.IsNullOrEmpty(this.m_PanelName) ? "Panel" : this.m_PanelName;
			this.m_MarkerPrepareRepaint = new ProfilerMarker(str + ".PrepareRepaint");
			this.m_MarkerRender = new ProfilerMarker(str + ".Render");
			this.m_MarkerValidateLayout = new ProfilerMarker(str + ".ValidateLayout");
			this.m_MarkerTickScheduledActions = new ProfilerMarker(str + ".TickScheduledActions");
			this.m_MarkerTickScheduledActionsPreLayout = new ProfilerMarker(str + ".TickScheduledActionsPreLayout");
			this.m_MarkerTickScheduledActionsPostLayout = new ProfilerMarker(str + ".TickScheduledActionsPostLayout");
			this.m_MarkerPanelChangeReceiver = new ProfilerMarker(str + ".ExecutePanelChangeReceiverCallback");
		}

		internal static TimeMsFunction TimeSinceStartup { get; set; }

		public override int IMGUIContainersCount { get; set; }

		public override IMGUIContainer rootIMGUIContainer { get; set; }

		internal override uint version
		{
			get
			{
				return this.m_Version;
			}
		}

		internal override uint repaintVersion
		{
			get
			{
				return this.m_RepaintVersion;
			}
		}

		internal override uint hierarchyVersion
		{
			get
			{
				return this.m_HierarchyVersion;
			}
		}

		public override AtlasBase atlas
		{
			get
			{
				return this.m_Atlas;
			}
			set
			{
				bool flag = this.m_Atlas != value;
				if (flag)
				{
					AtlasBase atlas = this.m_Atlas;
					if (atlas != null)
					{
						atlas.InvokeRemovedFromPanel(this);
					}
					this.m_Atlas = value;
					base.InvokeAtlasChanged();
					AtlasBase atlas2 = this.m_Atlas;
					if (atlas2 != null)
					{
						atlas2.InvokeAssignedToPanel(this);
					}
				}
			}
		}

		public Panel(ScriptableObject ownerObject, ContextType contextType, EventDispatcher dispatcher)
		{
			Debug.Assert(contextType == ContextType.Player, "In a player, panel context type must be set to Player.");
			contextType = ContextType.Player;
			this.ownerObject = ownerObject;
			this.contextType = contextType;
			this.dispatcher = dispatcher;
			this.repaintData = new RepaintData();
			this.cursorManager = new CursorManager();
			base.contextualMenuManager = null;
			this.dataBindingManager = new DataBindingManager(this);
			this.m_VisualTreeUpdater = new VisualTreeUpdater(this);
			base.SetSpecializedHierarchyFlagsUpdater();
			this.m_RootContainer = ((contextType == ContextType.Editor) ? new EditorPanelRootElement() : new PanelRootElement());
			this.visualTree.SetPanel(this);
			this.focusController = new FocusController(new VisualElementFocusRing(this.visualTree, VisualElementFocusRing.DefaultFocusOrder.ChildOrder));
			this.styleAnimationSystem = new StylePropertyAnimationSystem();
			this.CreateMarkers();
			base.InvokeHierarchyChanged(this.visualTree, HierarchyChangeType.AddedToParent, null);
			this.atlas = new DynamicAtlas();
		}

		protected override void Dispose(bool disposing)
		{
			bool disposed = base.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.atlas = null;
					this.visualTree.Clear();
					this.m_VisualTreeUpdater.Dispose();
				}
				base.Dispose(disposing);
			}
		}

		public static long TimeSinceStartupMs()
		{
			TimeMsFunction timeSinceStartup = Panel.TimeSinceStartup;
			return (timeSinceStartup != null) ? timeSinceStartup() : Panel.DefaultTimeSinceStartupMs();
		}

		internal static long DefaultTimeSinceStartupMs()
		{
			return (long)(Time.realtimeSinceStartup * 1000f);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.UIBuilderModule",
			"Assembly-CSharp-testable"
		})]
		internal static VisualElement PickAllWithoutValidatingLayout(VisualElement root, Vector2 point)
		{
			return Panel.PickAll(root, point, null, false);
		}

		internal static VisualElement PickAll(VisualElement root, Vector2 point, List<VisualElement> picked = null, bool includeIgnoredElement = false)
		{
			return Panel.PerformPick(root, point, picked, includeIgnoredElement);
		}

		private static VisualElement PerformPick(VisualElement root, Vector2 point, List<VisualElement> picked = null, bool includeIgnoredElement = false)
		{
			bool flag = root.resolvedStyle.display == DisplayStyle.None;
			VisualElement result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = root.pickingMode == PickingMode.Ignore && root.hierarchy.childCount == 0 && !includeIgnoredElement;
				if (flag2)
				{
					result = null;
				}
				else
				{
					bool flag3 = !root.worldBoundingBox.Contains(point);
					if (flag3)
					{
						result = null;
					}
					else
					{
						Vector2 localPoint = root.WorldToLocal(point);
						bool flag4 = root.ContainsPoint(localPoint);
						bool flag5 = !flag4 && root.ShouldClip();
						if (flag5)
						{
							result = null;
						}
						else
						{
							VisualElement visualElement = null;
							int childCount = root.hierarchy.childCount;
							for (int i = childCount - 1; i >= 0; i--)
							{
								VisualElement root2 = root.hierarchy[i];
								VisualElement visualElement2 = Panel.PerformPick(root2, point, picked, includeIgnoredElement);
								bool flag6 = visualElement == null && visualElement2 != null;
								if (flag6)
								{
									bool flag7 = picked == null;
									if (flag7)
									{
										return visualElement2;
									}
									visualElement = visualElement2;
								}
							}
							bool flag8 = root.visible && (root.pickingMode == PickingMode.Position || includeIgnoredElement) && flag4;
							if (flag8)
							{
								if (picked != null)
								{
									picked.Add(root);
								}
								bool flag9 = visualElement == null;
								if (flag9)
								{
									visualElement = root;
								}
							}
							result = visualElement;
						}
					}
				}
			}
			return result;
		}

		public override VisualElement PickAll(Vector2 point, List<VisualElement> picked)
		{
			this.ValidateLayout();
			bool flag = picked != null;
			if (flag)
			{
				picked.Clear();
			}
			return Panel.PickAll(this.visualTree, point, picked, false);
		}

		public override VisualElement Pick(Vector2 point, int pointerId)
		{
			this.ValidateLayout();
			Vector2 p;
			bool flag;
			VisualElement topElementUnderPointer = this.m_TopElementUnderPointers.GetTopElementUnderPointer(pointerId, out p, out flag);
			bool flag2 = !flag && Panel.<Pick>g__PixelOf|105_0(p) == Panel.<Pick>g__PixelOf|105_0(point);
			VisualElement result;
			if (flag2)
			{
				result = topElementUnderPointer;
			}
			else
			{
				result = Panel.PickAll(this.visualTree, point, null, false);
			}
			return result;
		}

		public override void ValidateLayout()
		{
			using (new IMGUIContainer.UITKScope())
			{
				bool flag = !this.m_ValidatingLayout;
				if (flag)
				{
					this.m_ValidatingLayout = true;
					this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
					this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
					this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
					this.m_ValidatingLayout = false;
				}
			}
		}

		public override void UpdateAnimations()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Animation);
		}

		public override void UpdateBindings()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Bindings);
		}

		public override void UpdateDataBinding()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.DataBinding);
		}

		public override void TickSchedulingUpdaters()
		{
			using (new IMGUIContainer.UITKScope())
			{
				using (this.m_MarkerTickScheduledActions.Auto())
				{
					this.timerEventScheduler.UpdateScheduledEvents();
					this.ValidateFocus();
					this.UpdateBindings();
					this.UpdateDataBinding();
					this.UpdateAnimations();
					this.m_LastTickedHierarchyVersion = this.m_HierarchyVersion;
				}
			}
		}

		public override void ApplyStyles()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
		}

		public override void UpdateForRepaint()
		{
			bool flag = this.m_LastTickedHierarchyVersion != this.m_HierarchyVersion;
			if (flag)
			{
				this.TickSchedulingUpdaters();
			}
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.DataBinding);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.TransformClip);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Repaint);
		}

		internal void UpdateWithoutRepaint()
		{
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Bindings);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.DataBinding);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Styles);
			this.m_VisualTreeUpdater.UpdateVisualTreePhase(VisualTreeUpdatePhase.Layout);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal static event Action<Panel> beforeAnyRepaint;

		public override void Repaint(Event e)
		{
			using (new IMGUIContainer.UITKScope())
			{
				this.m_RepaintVersion = this.version;
				this.repaintData.repaintEvent = e;
				base.InvokeBeforeUpdate();
				Action<Panel> action = Panel.beforeAnyRepaint;
				if (action != null)
				{
					action(this);
				}
				using (this.m_MarkerPrepareRepaint.Auto())
				{
					this.UpdateForRepaint();
				}
			}
		}

		public override void Render()
		{
			using (new IMGUIContainer.UITKScope())
			{
				base.Render();
			}
		}

		internal override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			this.m_Version += 1U;
			this.m_VisualTreeUpdater.OnVersionChanged(ve, versionChangeType);
			bool flag = this.panelChangeReceiver != null;
			if (flag)
			{
				using (this.m_MarkerPanelChangeReceiver.Auto())
				{
					this.panelChangeReceiver.OnVisualElementChange(ve, versionChangeType);
				}
			}
			bool flag2 = (versionChangeType & VersionChangeType.Hierarchy) == VersionChangeType.Hierarchy;
			if (flag2)
			{
				this.m_HierarchyVersion += 1U;
			}
		}

		internal override void SetUpdater(IVisualTreeUpdater updater, VisualTreeUpdatePhase phase)
		{
			this.m_VisualTreeUpdater.SetUpdater(updater, phase);
		}

		internal override IVisualTreeUpdater GetUpdater(VisualTreeUpdatePhase phase)
		{
			return this.m_VisualTreeUpdater.GetUpdater(phase);
		}

		internal virtual Color HyperlinkColor
		{
			get
			{
				return Color.blue;
			}
		}

		[CompilerGenerated]
		internal static Vector2Int <Pick>g__PixelOf|105_0(Vector2 p)
		{
			return Vector2Int.FloorToInt(p);
		}

		internal const int k_DefaultPixelsPerUnit = 100;

		private VisualElement m_RootContainer;

		private VisualTreeUpdater m_VisualTreeUpdater;

		private IStylePropertyAnimationSystem m_StylePropertyAnimationSystem;

		private string m_PanelName;

		private uint m_Version = 0U;

		private uint m_RepaintVersion = 0U;

		private uint m_HierarchyVersion = 0U;

		private uint m_LastTickedHierarchyVersion = 0U;

		private ProfilerMarker m_MarkerPrepareRepaint;

		private ProfilerMarker m_MarkerRender;

		private ProfilerMarker m_MarkerValidateLayout;

		private ProfilerMarker m_MarkerTickScheduledActions;

		protected ProfilerMarker m_MarkerTickScheduledActionsPreLayout;

		protected ProfilerMarker m_MarkerTickScheduledActionsPostLayout;

		private ProfilerMarker m_MarkerPanelChangeReceiver;

		private static ProfilerMarker s_MarkerPickAll = new ProfilerMarker("UIElements.PickAll");

		private TimerEventScheduler m_Scheduler;

		private bool m_JustReceivedFocus;

		private IDebugPanelChangeReceiver m_PanelChangeReceiver;

		private AtlasBase m_Atlas;

		private bool m_ValidatingLayout = false;

		internal class UIFrameState
		{
			internal virtual long[] updatersFrameCount { get; }

			internal virtual long schedulerFrameCount { get; }

			internal virtual bool isPanelDirty { get; }

			internal virtual ContextType panelContextType { get; }

			internal UIFrameState()
			{
			}

			internal UIFrameState(Panel panel)
			{
				this.isPanelDirty = panel.isDirty;
				this.panelContextType = panel.contextType;
				this.schedulerFrameCount = panel.scheduler.FrameCount;
				this.updatersFrameCount = panel.visualTreeUpdater.GetUpdatersFrameCount();
			}

			public static bool operator >(Panel.UIFrameState leftOperand, Panel.UIFrameState rightOperand)
			{
				return leftOperand.HasFullUIFrameOccurredSince(rightOperand);
			}

			public static bool operator <(Panel.UIFrameState leftOperand, Panel.UIFrameState rightOperand)
			{
				return rightOperand.HasFullUIFrameOccurredSince(leftOperand);
			}

			private bool HasFullUIFrameOccurredSince(Panel.UIFrameState reference)
			{
				bool flag = this.panelContextType != reference.panelContextType;
				if (flag)
				{
					throw new NotSupportedException("Comparison is only valid for frames with the same ContextType.");
				}
				bool flag2 = this.schedulerFrameCount <= reference.schedulerFrameCount;
				bool result;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = !this.isPanelDirty && this.panelContextType == ContextType.Editor;
					if (!flag3)
					{
						for (int i = 0; i < this.updatersFrameCount.Length; i++)
						{
							bool flag4 = this.updatersFrameCount[i] <= reference.updatersFrameCount[i];
							if (flag4)
							{
								return false;
							}
						}
					}
					result = true;
				}
				return result;
			}
		}
	}
}
