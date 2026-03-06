using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Rendering.UI;

namespace UnityEngine.Rendering
{
	public sealed class DebugManager
	{
		private void RegisterActions()
		{
			this.m_DebugActions = new DebugActionDesc[9];
			this.m_DebugActionStates = new DebugActionState[9];
			this.AddAction(DebugAction.EnableDebugMenu, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Enable Debug", false),
				repeatMode = DebugActionRepeatMode.Never
			});
			this.AddAction(DebugAction.ResetAll, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Debug Reset", false),
				repeatMode = DebugActionRepeatMode.Never
			});
			this.AddAction(DebugAction.NextDebugPanel, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Debug Next", false),
				repeatMode = DebugActionRepeatMode.Never
			});
			this.AddAction(DebugAction.PreviousDebugPanel, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Debug Previous", false),
				repeatMode = DebugActionRepeatMode.Never
			});
			DebugActionDesc debugActionDesc = new DebugActionDesc();
			debugActionDesc.buttonAction = this.debugActionMap.FindAction("Debug Validate", false);
			debugActionDesc.repeatMode = DebugActionRepeatMode.Never;
			this.AddAction(DebugAction.Action, debugActionDesc);
			this.AddAction(DebugAction.MakePersistent, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Debug Persistent", false),
				repeatMode = DebugActionRepeatMode.Never
			});
			DebugActionDesc debugActionDesc2 = new DebugActionDesc();
			debugActionDesc2.buttonAction = this.debugActionMap.FindAction("Debug Multiplier", false);
			debugActionDesc2.repeatMode = DebugActionRepeatMode.Delay;
			debugActionDesc.repeatDelay = 0f;
			this.AddAction(DebugAction.Multiplier, debugActionDesc2);
			this.AddAction(DebugAction.MoveVertical, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Debug Vertical", false),
				repeatMode = DebugActionRepeatMode.Delay,
				repeatDelay = 0.16f
			});
			this.AddAction(DebugAction.MoveHorizontal, new DebugActionDesc
			{
				buttonAction = this.debugActionMap.FindAction("Debug Horizontal", false),
				repeatMode = DebugActionRepeatMode.Delay,
				repeatDelay = 0.16f
			});
		}

		internal void EnableInputActions()
		{
			foreach (InputAction inputAction in this.debugActionMap)
			{
				inputAction.Enable();
			}
		}

		private void AddAction(DebugAction action, DebugActionDesc desc)
		{
			this.m_DebugActions[(int)action] = desc;
			this.m_DebugActionStates[(int)action] = new DebugActionState();
		}

		private void SampleAction(int actionIndex)
		{
			DebugActionDesc debugActionDesc = this.m_DebugActions[actionIndex];
			DebugActionState debugActionState = this.m_DebugActionStates[actionIndex];
			if (!debugActionState.runningAction && debugActionDesc.buttonAction != null)
			{
				float num = debugActionDesc.buttonAction.ReadValue<float>();
				if (!Mathf.Approximately(num, 0f))
				{
					debugActionState.TriggerWithButton(debugActionDesc.buttonAction, num);
				}
			}
		}

		private void UpdateAction(int actionIndex)
		{
			DebugActionDesc desc = this.m_DebugActions[actionIndex];
			DebugActionState debugActionState = this.m_DebugActionStates[actionIndex];
			if (debugActionState.runningAction)
			{
				debugActionState.Update(desc);
			}
		}

		internal void UpdateActions()
		{
			for (int i = 0; i < this.m_DebugActions.Length; i++)
			{
				this.UpdateAction(i);
				this.SampleAction(i);
			}
		}

		internal float GetAction(DebugAction action)
		{
			return this.m_DebugActionStates[(int)action].actionState;
		}

		internal bool GetActionToggleDebugMenuWithTouch()
		{
			if (!EnhancedTouchSupport.enabled)
			{
				return false;
			}
			ReadOnlyArray<Touch> activeTouches = Touch.activeTouches;
			int count = activeTouches.Count;
			TouchPhase? touchPhase = null;
			if (count == 3)
			{
				foreach (Touch touch in activeTouches)
				{
					if ((touchPhase == null || touch.phase == touchPhase.Value) && touch.tapCount == 2)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal bool GetActionReleaseScrollTarget()
		{
			bool flag = Mouse.current != null && Mouse.current.scroll.ReadValue() != Vector2.zero;
			bool flag2 = Touchscreen.current != null;
			return flag || flag2;
		}

		private void RegisterInputs()
		{
			this.debugActionMap.AddAction("Enable Debug", InputActionType.Button, null, null, null, null, null).AddCompositeBinding("ButtonWithOneModifier", null, null).With("Modifier", "<Gamepad>/rightStickPress", null, null).With("Button", "<Gamepad>/leftStickPress", null, null).With("Modifier", "<Keyboard>/leftCtrl", null, null).With("Button", "<Keyboard>/backspace", null, null);
			this.debugActionMap.AddAction("Debug Reset", InputActionType.Button, null, null, null, null, null).AddCompositeBinding("ButtonWithOneModifier", null, null).With("Modifier", "<Gamepad>/rightStickPress", null, null).With("Button", "<Gamepad>/b", null, null).With("Modifier", "<Keyboard>/leftAlt", null, null).With("Button", "<Keyboard>/backspace", null, null);
			InputAction action = this.debugActionMap.AddAction("Debug Next", InputActionType.Button, null, null, null, null, null);
			action.AddBinding("<Keyboard>/pageDown", null, null, null);
			action.AddBinding("<Gamepad>/rightShoulder", null, null, null);
			InputAction action2 = this.debugActionMap.AddAction("Debug Previous", InputActionType.Button, null, null, null, null, null);
			action2.AddBinding("<Keyboard>/pageUp", null, null, null);
			action2.AddBinding("<Gamepad>/leftShoulder", null, null, null);
			InputAction action3 = this.debugActionMap.AddAction("Debug Validate", InputActionType.Button, null, null, null, null, null);
			action3.AddBinding("<Keyboard>/enter", null, null, null);
			action3.AddBinding("<Gamepad>/a", null, null, null);
			InputAction action4 = this.debugActionMap.AddAction("Debug Persistent", InputActionType.Button, null, null, null, null, null);
			action4.AddBinding("<Keyboard>/rightShift", null, null, null);
			action4.AddBinding("<Gamepad>/x", null, null, null);
			InputAction action5 = this.debugActionMap.AddAction("Debug Multiplier", InputActionType.Value, null, null, null, null, null);
			action5.AddBinding("<Keyboard>/leftShift", null, null, null);
			action5.AddBinding("<Gamepad>/y", null, null, null);
			this.debugActionMap.AddAction("Debug Vertical", InputActionType.Value, null, null, null, null, null).AddCompositeBinding("1DAxis", null, null).With("Positive", "<Gamepad>/dpad/up", null, null).With("Negative", "<Gamepad>/dpad/down", null, null).With("Positive", "<Keyboard>/upArrow", null, null).With("Negative", "<Keyboard>/downArrow", null, null);
			this.debugActionMap.AddAction("Debug Horizontal", InputActionType.Value, null, null, null, null, null).AddCompositeBinding("1DAxis", null, null).With("Positive", "<Gamepad>/dpad/right", null, null).With("Negative", "<Gamepad>/dpad/left", null, null).With("Positive", "<Keyboard>/rightArrow", null, null).With("Negative", "<Keyboard>/leftArrow", null, null);
		}

		public static DebugManager instance
		{
			get
			{
				return DebugManager.s_Instance.Value;
			}
		}

		private void UpdateReadOnlyCollection()
		{
			this.m_Panels.Sort();
			this.m_ReadOnlyPanels = this.m_Panels.AsReadOnly();
		}

		public ReadOnlyCollection<DebugUI.Panel> panels
		{
			get
			{
				if (this.m_ReadOnlyPanels == null)
				{
					this.UpdateReadOnlyCollection();
				}
				return this.m_ReadOnlyPanels;
			}
		}

		public event Action<bool> onDisplayRuntimeUIChanged = delegate(bool <p0>)
		{
		};

		public event Action onSetDirty = delegate()
		{
		};

		private event Action resetData;

		public bool isAnyDebugUIActive
		{
			get
			{
				return this.displayRuntimeUI || this.displayPersistentRuntimeUI;
			}
		}

		private DebugManager()
		{
		}

		public void RefreshEditor()
		{
			this.refreshEditorRequested = true;
		}

		public void Reset()
		{
			Action action = this.resetData;
			if (action != null)
			{
				action();
			}
			this.ReDrawOnScreenDebug();
		}

		public void ReDrawOnScreenDebug()
		{
			if (this.displayRuntimeUI)
			{
				DebugUIHandlerCanvas rootUICanvas = this.m_RootUICanvas;
				if (rootUICanvas == null)
				{
					return;
				}
				rootUICanvas.RequestHierarchyReset();
			}
		}

		public void RegisterData(IDebugData data)
		{
			this.resetData += data.GetReset();
		}

		public void UnregisterData(IDebugData data)
		{
			this.resetData -= data.GetReset();
		}

		public int GetState()
		{
			int num = 17;
			foreach (DebugUI.Panel panel in this.m_Panels)
			{
				num = num * 23 + panel.GetHashCode();
			}
			return num;
		}

		internal void RegisterRootCanvas(DebugUIHandlerCanvas root)
		{
			this.m_Root = root.gameObject;
			this.m_RootUICanvas = root;
		}

		internal void ChangeSelection(DebugUIHandlerWidget widget, bool fromNext)
		{
			this.m_RootUICanvas.ChangeSelection(widget, fromNext);
		}

		internal void SetScrollTarget(DebugUIHandlerWidget widget)
		{
			if (this.m_RootUICanvas != null)
			{
				this.m_RootUICanvas.SetScrollTarget(widget);
			}
		}

		private void EnsurePersistentCanvas()
		{
			if (this.m_RootUIPersistentCanvas == null)
			{
				DebugUIHandlerPersistentCanvas debugUIHandlerPersistentCanvas = Object.FindFirstObjectByType<DebugUIHandlerPersistentCanvas>();
				if (debugUIHandlerPersistentCanvas == null)
				{
					this.m_PersistentRoot = Object.Instantiate<Transform>(Resources.Load<Transform>("DebugUIPersistentCanvas")).gameObject;
					this.m_PersistentRoot.name = "[Debug Canvas - Persistent]";
					this.m_PersistentRoot.transform.localPosition = Vector3.zero;
				}
				else
				{
					this.m_PersistentRoot = debugUIHandlerPersistentCanvas.gameObject;
				}
				this.m_RootUIPersistentCanvas = this.m_PersistentRoot.GetComponent<DebugUIHandlerPersistentCanvas>();
			}
		}

		internal void TogglePersistent(DebugUI.Widget widget, int? forceTupleIndex = null)
		{
			if (widget == null)
			{
				return;
			}
			this.EnsurePersistentCanvas();
			DebugUI.Value value = widget as DebugUI.Value;
			if (value != null)
			{
				this.m_RootUIPersistentCanvas.Toggle(value, null);
				return;
			}
			DebugUI.ValueTuple valueTuple = widget as DebugUI.ValueTuple;
			if (valueTuple == null)
			{
				DebugUI.Container container = widget as DebugUI.Container;
				if (container != null)
				{
					int value2 = container.children.Max(delegate(DebugUI.Widget w)
					{
						DebugUI.ValueTuple valueTuple2 = w as DebugUI.ValueTuple;
						if (valueTuple2 == null)
						{
							return -1;
						}
						return valueTuple2.pinnedElementIndex;
					});
					using (IEnumerator<DebugUI.Widget> enumerator = container.children.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DebugUI.Widget widget2 = enumerator.Current;
							if (widget2 is DebugUI.Value || widget2 is DebugUI.ValueTuple)
							{
								this.TogglePersistent(widget2, new int?(value2));
							}
						}
						return;
					}
				}
				Debug.Log("Only readonly items can be made persistent.");
				return;
			}
			this.m_RootUIPersistentCanvas.Toggle(valueTuple, forceTupleIndex);
		}

		private void OnPanelDirty(DebugUI.Panel panel)
		{
			this.onSetDirty();
		}

		public int PanelIndex([DisallowNull] string displayName)
		{
			if (displayName == null)
			{
				displayName = string.Empty;
			}
			for (int i = 0; i < this.m_Panels.Count; i++)
			{
				if (displayName.Equals(this.m_Panels[i].displayName, StringComparison.InvariantCultureIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		public string PanelDiplayName([DisallowNull] int panelIndex)
		{
			if (panelIndex < 0 || panelIndex > this.m_Panels.Count - 1)
			{
				return string.Empty;
			}
			return this.m_Panels[panelIndex].displayName;
		}

		public void RequestEditorWindowPanelIndex(int index)
		{
			this.m_RequestedPanelIndex = new int?(index);
		}

		internal int? GetRequestedEditorWindowPanelIndex()
		{
			int? requestedPanelIndex = this.m_RequestedPanelIndex;
			this.m_RequestedPanelIndex = null;
			return requestedPanelIndex;
		}

		public DebugUI.Panel GetPanel(string displayName, bool createIfNull = false, int groupIndex = 0, bool overrideIfExist = false)
		{
			int num = this.PanelIndex(displayName);
			DebugUI.Panel panel = (num >= 0) ? this.m_Panels[num] : null;
			if (panel != null)
			{
				if (!overrideIfExist)
				{
					return panel;
				}
				panel.onSetDirty -= this.OnPanelDirty;
				this.RemovePanel(panel);
				panel = null;
			}
			if (createIfNull)
			{
				panel = new DebugUI.Panel
				{
					displayName = displayName,
					groupIndex = groupIndex
				};
				panel.onSetDirty += this.OnPanelDirty;
				this.m_Panels.Add(panel);
				this.UpdateReadOnlyCollection();
			}
			return panel;
		}

		public int FindPanelIndex(string displayName)
		{
			return this.m_Panels.FindIndex((DebugUI.Panel p) => p.displayName == displayName);
		}

		public void RemovePanel(string displayName)
		{
			DebugUI.Panel panel = null;
			foreach (DebugUI.Panel panel2 in this.m_Panels)
			{
				if (panel2.displayName == displayName)
				{
					panel2.onSetDirty -= this.OnPanelDirty;
					panel = panel2;
					break;
				}
			}
			this.RemovePanel(panel);
		}

		public void RemovePanel(DebugUI.Panel panel)
		{
			if (panel == null)
			{
				return;
			}
			this.m_Panels.Remove(panel);
			this.UpdateReadOnlyCollection();
		}

		public DebugUI.Widget[] GetItems(DebugUI.Flags flags)
		{
			List<DebugUI.Widget> list;
			DebugUI.Widget[] result;
			using (ListPool<DebugUI.Widget>.Get(out list))
			{
				foreach (DebugUI.Panel container in this.m_Panels)
				{
					DebugUI.Widget[] itemsFromContainer = this.GetItemsFromContainer(flags, container);
					list.AddRange(itemsFromContainer);
				}
				result = list.ToArray();
			}
			return result;
		}

		internal DebugUI.Widget[] GetItemsFromContainer(DebugUI.Flags flags, DebugUI.IContainer container)
		{
			List<DebugUI.Widget> list;
			DebugUI.Widget[] result;
			using (ListPool<DebugUI.Widget>.Get(out list))
			{
				foreach (DebugUI.Widget widget in container.children)
				{
					if (widget.flags.HasFlag(flags))
					{
						list.Add(widget);
					}
					else
					{
						DebugUI.IContainer container2 = widget as DebugUI.IContainer;
						if (container2 != null)
						{
							list.AddRange(this.GetItemsFromContainer(flags, container2));
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		public DebugUI.Widget GetItem(string queryPath)
		{
			foreach (DebugUI.Panel container in this.m_Panels)
			{
				DebugUI.Widget item = this.GetItem(queryPath, container);
				if (item != null)
				{
					return item;
				}
			}
			return null;
		}

		private DebugUI.Widget GetItem(string queryPath, DebugUI.IContainer container)
		{
			foreach (DebugUI.Widget widget in container.children)
			{
				if (widget.queryPath == queryPath)
				{
					return widget;
				}
				DebugUI.IContainer container2 = widget as DebugUI.IContainer;
				if (container2 != null)
				{
					DebugUI.Widget item = this.GetItem(queryPath, container2);
					if (item != null)
					{
						return item;
					}
				}
			}
			return null;
		}

		public static event Action<DebugManager.UIMode, bool> windowStateChanged;

		public bool displayEditorUI
		{
			get
			{
				return this.editorUIState.open;
			}
			set
			{
				this.editorUIState.open = value;
			}
		}

		public bool enableRuntimeUI
		{
			get
			{
				return this.m_EnableRuntimeUI;
			}
			set
			{
				if (value != this.m_EnableRuntimeUI)
				{
					this.m_EnableRuntimeUI = value;
					DebugUpdater.SetEnabled(value);
				}
			}
		}

		public bool displayRuntimeUI
		{
			get
			{
				return this.m_Root != null && this.m_Root.activeInHierarchy;
			}
			set
			{
				if (value)
				{
					this.m_Root = Object.Instantiate<Transform>(Resources.Load<Transform>("DebugUICanvas")).gameObject;
					this.m_Root.name = "[Debug Canvas]";
					this.m_Root.transform.localPosition = Vector3.zero;
					this.m_RootUICanvas = this.m_Root.GetComponent<DebugUIHandlerCanvas>();
					this.m_Root.SetActive(true);
				}
				else
				{
					CoreUtils.Destroy(this.m_Root);
					this.m_Root = null;
					this.m_RootUICanvas = null;
				}
				this.onDisplayRuntimeUIChanged(value);
				DebugUpdater.HandleInternalEventSystemComponents(value);
				this.runtimeUIState.open = (this.m_Root != null && this.m_Root.activeInHierarchy);
			}
		}

		public bool displayPersistentRuntimeUI
		{
			get
			{
				return this.m_RootUIPersistentCanvas != null && this.m_PersistentRoot.activeInHierarchy;
			}
			set
			{
				if (value)
				{
					this.EnsurePersistentCanvas();
					return;
				}
				CoreUtils.Destroy(this.m_PersistentRoot);
				this.m_PersistentRoot = null;
				this.m_RootUIPersistentCanvas = null;
			}
		}

		[Obsolete("Use DebugManager.instance.displayEditorUI property instead. #from(23.1)")]
		public void ToggleEditorUI(bool open)
		{
			this.editorUIState.open = open;
		}

		private const string kEnableDebugBtn1 = "Enable Debug Button 1";

		private const string kEnableDebugBtn2 = "Enable Debug Button 2";

		private const string kDebugPreviousBtn = "Debug Previous";

		private const string kDebugNextBtn = "Debug Next";

		private const string kValidateBtn = "Debug Validate";

		private const string kPersistentBtn = "Debug Persistent";

		private const string kDPadVertical = "Debug Vertical";

		private const string kDPadHorizontal = "Debug Horizontal";

		private const string kMultiplierBtn = "Debug Multiplier";

		private const string kResetBtn = "Debug Reset";

		private const string kEnableDebug = "Enable Debug";

		private DebugActionDesc[] m_DebugActions;

		private DebugActionState[] m_DebugActionStates;

		private InputActionMap debugActionMap = new InputActionMap("Debug Menu");

		private static readonly Lazy<DebugManager> s_Instance = new Lazy<DebugManager>(() => new DebugManager());

		private ReadOnlyCollection<DebugUI.Panel> m_ReadOnlyPanels;

		private readonly List<DebugUI.Panel> m_Panels = new List<DebugUI.Panel>();

		public bool refreshEditorRequested;

		private int? m_RequestedPanelIndex;

		private GameObject m_Root;

		private DebugUIHandlerCanvas m_RootUICanvas;

		private GameObject m_PersistentRoot;

		private DebugUIHandlerPersistentCanvas m_RootUIPersistentCanvas;

		private DebugManager.UIState editorUIState = new DebugManager.UIState
		{
			mode = DebugManager.UIMode.EditorMode
		};

		private bool m_EnableRuntimeUI = true;

		private DebugManager.UIState runtimeUIState = new DebugManager.UIState
		{
			mode = DebugManager.UIMode.RuntimeMode
		};

		public enum UIMode
		{
			EditorMode,
			RuntimeMode
		}

		private class UIState
		{
			public bool open
			{
				get
				{
					return this.m_Open;
				}
				set
				{
					if (this.m_Open == value)
					{
						return;
					}
					this.m_Open = value;
					Action<DebugManager.UIMode, bool> windowStateChanged = DebugManager.windowStateChanged;
					if (windowStateChanged == null)
					{
						return;
					}
					windowStateChanged(this.mode, this.m_Open);
				}
			}

			public DebugManager.UIMode mode;

			[SerializeField]
			private bool m_Open;
		}
	}
}
