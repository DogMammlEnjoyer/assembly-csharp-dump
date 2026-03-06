using System;
using Modio.Unity.UI.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modio.Unity.UI.Panels
{
	public abstract class ModioPanelBase : MonoBehaviour
	{
		public bool HasFocus { get; private set; }

		public event Action<bool> OnHasFocusChanged;

		protected virtual void Awake()
		{
			ModioPanelManager.GetInstance().RegisterPanel(this);
		}

		protected virtual void Start()
		{
			if (this._startHidden && !this.HasFocus)
			{
				if (this._panelToEnable == null)
				{
					base.gameObject.SetActive(false);
					return;
				}
				this._panelToEnable.SetActive(false);
			}
		}

		protected virtual void OnDestroy()
		{
			if (this.HasFocus)
			{
				this.OnLostFocus();
			}
		}

		public void OpenPanel()
		{
			Transform parent = base.transform.parent;
			if (parent != null && !parent.gameObject.activeInHierarchy)
			{
				Debug.LogWarning(string.Format("Attempted to open panel {0} with disabled parent. Suppressing this to avoid lost input.", this));
				return;
			}
			if (this._panelToEnable == null)
			{
				base.gameObject.SetActive(true);
			}
			else
			{
				this._panelToEnable.SetActive(true);
			}
			if (this._openOnTopOf != null && ((this._openOnTopOf._panelToEnable == null) ? (!base.gameObject.activeSelf) : (!this._openOnTopOf._panelToEnable.activeSelf)))
			{
				this._openOnTopOf.OpenPanel();
			}
			ModioPanelManager.GetInstance().OpenPanel(this);
		}

		public void ClosePanel()
		{
			ModioPanelManager.GetInstance().ClosePanel(this);
			if (this._panelToEnable == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this._panelToEnable.SetActive(false);
		}

		public virtual void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			this.HasFocus = true;
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Cancel, new Action(this.CancelPressed));
			ModioUIInput.SwappedControlScheme += this.OnSwappedControlScheme;
			if (selectionBehaviour == ModioPanelBase.GainedFocusCause.RegainingFocusFromStackedPanel && this._lastSelectedGameObject != null && this._lastSelectedGameObject.activeInHierarchy && !EventSystem.current.alreadySelecting)
			{
				this.SetSelectedGameObject(this._lastSelectedGameObject);
				this.NewSelectionWhileFocused(this._lastSelectedGameObject);
			}
			else if (selectionBehaviour != ModioPanelBase.GainedFocusCause.InputSuppressionChangeOnly)
			{
				this.DoDefaultSelection();
			}
			Action<bool> onHasFocusChanged = this.OnHasFocusChanged;
			if (onHasFocusChanged == null)
			{
				return;
			}
			onHasFocusChanged(true);
		}

		public virtual void OnLostFocus()
		{
			this.HasFocus = false;
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Cancel, new Action(this.CancelPressed));
			ModioUIInput.SwappedControlScheme -= this.OnSwappedControlScheme;
			Action<bool> onHasFocusChanged = this.OnHasFocusChanged;
			if (onHasFocusChanged == null)
			{
				return;
			}
			onHasFocusChanged(false);
		}

		protected virtual void CancelPressed()
		{
			this.ClosePanel();
		}

		public virtual void DoDefaultSelection()
		{
			if (this._selectOnOpen != null)
			{
				this._selectOnOpen.Select();
				this.NewSelectionWhileFocused(this._selectOnOpen.gameObject);
			}
		}

		public virtual void FocusedPanelLateUpdate()
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null && currentSelectedGameObject.activeInHierarchy)
			{
				if (this._lastSelectedGameObject != currentSelectedGameObject)
				{
					this.NewSelectionWhileFocused(currentSelectedGameObject);
					return;
				}
			}
			else if (ModioUIInput.IsUsingGamepad)
			{
				this.DoDefaultSelection();
			}
		}

		public virtual void SetSelectedGameObject(GameObject selection)
		{
			EventSystem.current.SetSelectedGameObject(selection);
		}

		public void OverrideLastSelectedGameObject(GameObject selection)
		{
			this._lastSelectedGameObject = selection;
		}

		protected virtual void NewSelectionWhileFocused(GameObject currentSelection)
		{
			this._lastSelectedGameObject = currentSelection;
		}

		private void OnSwappedControlScheme(bool isController)
		{
			if (isController)
			{
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					return;
				}
				float num = float.MaxValue;
				Selectable selectable = null;
				Vector3 mousePosition = Input.mousePosition;
				foreach (Selectable selectable2 in base.GetComponentsInChildren<Selectable>())
				{
					if (selectable2.gameObject.activeInHierarchy && selectable2.navigation.mode != Navigation.Mode.None && selectable2.interactable)
					{
						RectTransform rectTransform = selectable2.transform as RectTransform;
						if (!(rectTransform == null))
						{
							float sqrMagnitude = (rectTransform.TransformPoint(rectTransform.rect.center) - mousePosition).sqrMagnitude;
							if (sqrMagnitude <= num)
							{
								selectable = selectable2;
								num = sqrMagnitude;
							}
						}
					}
				}
				if (selectable != null)
				{
					selectable.Select();
				}
			}
		}

		[SerializeField]
		private GameObject _panelToEnable;

		[SerializeField]
		private Selectable _selectOnOpen;

		[SerializeField]
		private bool _startHidden;

		[SerializeField]
		private ModioPanelBase _openOnTopOf;

		private GameObject _lastSelectedGameObject;

		public enum GainedFocusCause
		{
			OpeningFromClosed,
			RegainingFocusFromStackedPanel,
			InputSuppressionChangeOnly
		}
	}
}
