using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerCanvas : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.prefabs == null)
			{
				this.prefabs = new List<DebugUIPrefabBundle>();
			}
			if (this.m_PrefabsMap == null)
			{
				this.m_PrefabsMap = new Dictionary<Type, Transform>();
			}
			if (this.m_UIPanels == null)
			{
				this.m_UIPanels = new List<DebugUIHandlerPanel>();
			}
			DebugManager.instance.RegisterRootCanvas(this);
		}

		private void Update()
		{
			int state = DebugManager.instance.GetState();
			if (this.m_DebugTreeState != state)
			{
				this.ResetAllHierarchy();
			}
			this.HandleInput();
			if (this.m_UIPanels != null && this.m_SelectedPanel < this.m_UIPanels.Count && this.m_UIPanels[this.m_SelectedPanel] != null)
			{
				this.m_UIPanels[this.m_SelectedPanel].UpdateScroll();
			}
		}

		internal void RequestHierarchyReset()
		{
			this.m_DebugTreeState = -1;
		}

		private void ResetAllHierarchy()
		{
			foreach (object obj in base.transform)
			{
				CoreUtils.Destroy(((Transform)obj).gameObject);
			}
			this.Rebuild();
		}

		private void Rebuild()
		{
			this.m_PrefabsMap.Clear();
			foreach (DebugUIPrefabBundle debugUIPrefabBundle in this.prefabs)
			{
				Type type = Type.GetType(debugUIPrefabBundle.type);
				if (type != null && debugUIPrefabBundle.prefab != null)
				{
					this.m_PrefabsMap.Add(type, debugUIPrefabBundle.prefab);
				}
			}
			this.m_UIPanels.Clear();
			this.m_DebugTreeState = DebugManager.instance.GetState();
			ReadOnlyCollection<DebugUI.Panel> panels = DebugManager.instance.panels;
			DebugUIHandlerWidget selectedWidget = null;
			foreach (DebugUI.Panel panel in panels)
			{
				if (!panel.isEditorOnly)
				{
					if (panel.children.Count((DebugUI.Widget x) => !x.isEditorOnly && !x.isHidden) != 0)
					{
						GameObject gameObject = Object.Instantiate<Transform>(this.panelPrefab, base.transform, false).gameObject;
						gameObject.name = panel.displayName;
						DebugUIHandlerPanel component = gameObject.GetComponent<DebugUIHandlerPanel>();
						component.SetPanel(panel);
						component.Canvas = this;
						this.m_UIPanels.Add(component);
						DebugUIHandlerContainer component2 = gameObject.GetComponent<DebugUIHandlerContainer>();
						DebugUIHandlerWidget debugUIHandlerWidget = null;
						this.Traverse(panel, component2.contentHolder, null, ref debugUIHandlerWidget);
						if (debugUIHandlerWidget != null && debugUIHandlerWidget.GetWidget().queryPath.Contains(panel.queryPath))
						{
							selectedWidget = debugUIHandlerWidget;
						}
					}
				}
			}
			this.ActivatePanel(this.m_SelectedPanel, selectedWidget);
		}

		private void Traverse(DebugUI.IContainer container, Transform parentTransform, DebugUIHandlerWidget parentUIHandler, ref DebugUIHandlerWidget selectedHandler)
		{
			DebugUIHandlerWidget debugUIHandlerWidget = null;
			for (int i = 0; i < container.children.Count; i++)
			{
				DebugUI.Widget widget = container.children[i];
				if (!widget.isEditorOnly && !widget.isHidden)
				{
					Transform value;
					if (!this.m_PrefabsMap.TryGetValue(widget.GetType(), out value))
					{
						foreach (KeyValuePair<Type, Transform> keyValuePair in this.m_PrefabsMap)
						{
							if (keyValuePair.Key.IsAssignableFrom(widget.GetType()))
							{
								value = keyValuePair.Value;
								break;
							}
						}
					}
					if (value == null)
					{
						string str = "DebugUI widget doesn't have a prefab: ";
						Type type = widget.GetType();
						Debug.LogWarning(str + ((type != null) ? type.ToString() : null));
					}
					else
					{
						GameObject gameObject = Object.Instantiate<Transform>(value, parentTransform, false).gameObject;
						gameObject.name = widget.displayName;
						DebugUIHandlerWidget component = gameObject.GetComponent<DebugUIHandlerWidget>();
						if (component == null)
						{
							string str2 = "DebugUI prefab is missing a DebugUIHandler for: ";
							Type type2 = widget.GetType();
							Debug.LogWarning(str2 + ((type2 != null) ? type2.ToString() : null));
						}
						else
						{
							if (!string.IsNullOrEmpty(this.m_CurrentQueryPath) && widget.queryPath.Equals(this.m_CurrentQueryPath))
							{
								selectedHandler = component;
							}
							if (debugUIHandlerWidget != null)
							{
								debugUIHandlerWidget.nextUIHandler = component;
							}
							component.previousUIHandler = debugUIHandlerWidget;
							debugUIHandlerWidget = component;
							component.parentUIHandler = parentUIHandler;
							component.SetWidget(widget);
							DebugUIHandlerContainer component2 = gameObject.GetComponent<DebugUIHandlerContainer>();
							if (component2 != null)
							{
								DebugUI.IContainer container2 = widget as DebugUI.IContainer;
								if (container2 != null)
								{
									this.Traverse(container2, component2.contentHolder, component, ref selectedHandler);
								}
							}
						}
					}
				}
			}
		}

		private DebugUIHandlerWidget GetWidgetFromPath(string queryPath)
		{
			if (string.IsNullOrEmpty(queryPath))
			{
				return null;
			}
			return this.m_UIPanels[this.m_SelectedPanel].GetComponentsInChildren<DebugUIHandlerWidget>().FirstOrDefault((DebugUIHandlerWidget w) => w.GetWidget().queryPath == queryPath);
		}

		private void ActivatePanel(int index, DebugUIHandlerWidget selectedWidget = null)
		{
			if (this.m_UIPanels.Count == 0)
			{
				return;
			}
			if (index >= this.m_UIPanels.Count)
			{
				index = this.m_UIPanels.Count - 1;
			}
			this.m_UIPanels.ForEach(delegate(DebugUIHandlerPanel p)
			{
				p.gameObject.SetActive(false);
			});
			this.m_UIPanels[index].gameObject.SetActive(true);
			this.m_SelectedPanel = index;
			if (selectedWidget == null)
			{
				selectedWidget = this.m_UIPanels[index].GetFirstItem();
			}
			this.ChangeSelection(selectedWidget, true);
		}

		internal void ChangeSelection(DebugUIHandlerWidget widget, bool fromNext)
		{
			if (widget == null)
			{
				return;
			}
			if (this.m_SelectedWidget != null)
			{
				this.m_SelectedWidget.OnDeselection();
			}
			DebugUIHandlerWidget selectedWidget = this.m_SelectedWidget;
			this.m_SelectedWidget = widget;
			this.SetScrollTarget(widget);
			if (!this.m_SelectedWidget.OnSelection(fromNext, selectedWidget))
			{
				if (fromNext)
				{
					this.SelectNextItem();
					return;
				}
				this.SelectPreviousItem();
				return;
			}
			else
			{
				if (this.m_SelectedWidget == null || this.m_SelectedWidget.GetWidget() == null)
				{
					this.m_CurrentQueryPath = string.Empty;
					return;
				}
				this.m_CurrentQueryPath = this.m_SelectedWidget.GetWidget().queryPath;
				return;
			}
		}

		internal void SelectPreviousItem()
		{
			if (this.m_SelectedWidget == null)
			{
				return;
			}
			DebugUIHandlerWidget debugUIHandlerWidget = this.m_SelectedWidget.Previous();
			if (debugUIHandlerWidget != null)
			{
				this.ChangeSelection(debugUIHandlerWidget, false);
			}
		}

		internal void SelectNextPanel()
		{
			int num = this.m_SelectedPanel + 1;
			if (num >= this.m_UIPanels.Count)
			{
				num = 0;
			}
			num = Mathf.Clamp(num, 0, this.m_UIPanels.Count - 1);
			this.ActivatePanel(num, null);
		}

		internal void SelectPreviousPanel()
		{
			int num = this.m_SelectedPanel - 1;
			if (num < 0)
			{
				num = this.m_UIPanels.Count - 1;
			}
			num = Mathf.Clamp(num, 0, this.m_UIPanels.Count - 1);
			this.ActivatePanel(num, null);
		}

		internal void SelectNextItem()
		{
			if (this.m_SelectedWidget == null)
			{
				return;
			}
			DebugUIHandlerWidget debugUIHandlerWidget = this.m_SelectedWidget.Next();
			if (debugUIHandlerWidget != null)
			{
				this.ChangeSelection(debugUIHandlerWidget, true);
			}
		}

		private void ChangeSelectionValue(float multiplier)
		{
			if (this.m_SelectedWidget == null)
			{
				return;
			}
			bool fast = DebugManager.instance.GetAction(DebugAction.Multiplier) != 0f;
			if (multiplier < 0f)
			{
				this.m_SelectedWidget.OnDecrement(fast);
				return;
			}
			this.m_SelectedWidget.OnIncrement(fast);
		}

		private void ActivateSelection()
		{
			if (this.m_SelectedWidget == null)
			{
				return;
			}
			this.m_SelectedWidget.OnAction();
		}

		private void HandleInput()
		{
			if (DebugManager.instance.GetAction(DebugAction.PreviousDebugPanel) != 0f)
			{
				this.SelectPreviousPanel();
			}
			if (DebugManager.instance.GetAction(DebugAction.NextDebugPanel) != 0f)
			{
				this.SelectNextPanel();
			}
			if (DebugManager.instance.GetAction(DebugAction.Action) != 0f)
			{
				this.ActivateSelection();
			}
			if (DebugManager.instance.GetAction(DebugAction.MakePersistent) != 0f && this.m_SelectedWidget != null)
			{
				DebugManager.instance.TogglePersistent(this.m_SelectedWidget.GetWidget(), null);
			}
			float action = DebugManager.instance.GetAction(DebugAction.MoveHorizontal);
			if (action != 0f)
			{
				this.ChangeSelectionValue(action);
			}
			float action2 = DebugManager.instance.GetAction(DebugAction.MoveVertical);
			if (action2 != 0f)
			{
				if (action2 < 0f)
				{
					this.SelectNextItem();
					return;
				}
				this.SelectPreviousItem();
			}
		}

		internal void SetScrollTarget(DebugUIHandlerWidget widget)
		{
			if (this.m_UIPanels != null && this.m_SelectedPanel < this.m_UIPanels.Count && this.m_UIPanels[this.m_SelectedPanel] != null)
			{
				this.m_UIPanels[this.m_SelectedPanel].SetScrollTarget(widget);
			}
		}

		private int m_DebugTreeState;

		private Dictionary<Type, Transform> m_PrefabsMap;

		public Transform panelPrefab;

		public List<DebugUIPrefabBundle> prefabs;

		private List<DebugUIHandlerPanel> m_UIPanels;

		private int m_SelectedPanel;

		private DebugUIHandlerWidget m_SelectedWidget;

		private string m_CurrentQueryPath;
	}
}
