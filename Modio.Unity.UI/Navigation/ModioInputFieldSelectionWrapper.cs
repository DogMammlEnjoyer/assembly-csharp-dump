using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Extensions;
using Modio.Unity.UI.Components.Selectables;
using Modio.Unity.UI.Input;
using Modio.Unity.UI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modio.Unity.UI.Navigation
{
	public class ModioInputFieldSelectionWrapper : Selectable, ISubmitHandler, IEventSystemHandler
	{
		protected override void Awake()
		{
			base.Awake();
			this._inputField = base.GetComponentInChildren<TMP_InputField>();
			Navigation navigation = this._inputField.navigation;
			navigation.mode = Navigation.Mode.None;
			this._inputField.navigation = navigation;
			this._layoutElement = base.GetComponent<LayoutElement>();
			this._inputField.onSelect.AddListener(delegate(string s)
			{
				ModioPanelManager.GetInstance().PushFocusSuppression();
				ModioUIInput.AddHandler(ModioUIInput.ModioAction.Cancel, new Action(this.OnPressedCancel));
				this.UpdateAnimation(true);
			});
			this._inputField.onDeselect.AddListener(delegate(string s)
			{
				this.<Awake>g__DelayPopFocusSuppression|6_3().ForgetTaskSafely();
			});
			this._inputField.onEndEdit.AddListener(new UnityAction<string>(this.OnEndEdit));
			this._inputField.onValueChanged.AddListener(delegate(string s)
			{
				if (string.IsNullOrEmpty(s))
				{
					this.UpdateAnimation(false);
				}
			});
			if (this._disableWhenCollapsed != null)
			{
				this._disableWhenCollapsed.SetActive(false);
			}
		}

		protected override void OnDestroy()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Cancel, new Action(this.OnPressedCancel));
			base.OnDestroy();
		}

		private void OnEndEdit(string s)
		{
			if (EventSystem.current.currentSelectedGameObject == this._inputField.gameObject)
			{
				if (!this._keepFocusOnSubmit)
				{
					ModioPanelManager.GetInstance().PopFocusSuppression(ModioPanelBase.GainedFocusCause.RegainingFocusFromStackedPanel);
					return;
				}
				if (!EventSystem.current.alreadySelecting)
				{
					EventSystem.current.SetSelectedGameObject(base.gameObject);
				}
			}
		}

		private void OnPressedCancel()
		{
			if (EventSystem.current.currentSelectedGameObject == this._inputField.gameObject)
			{
				this._inputField.OnDeselect(null);
				this.UpdateSelectedVisuals(true);
			}
		}

		private void UpdateAnimation(bool gainingFocus = false)
		{
			if (!this._animateSelectionWidth)
			{
				return;
			}
			bool flag = this._inputField.isFocused || gainingFocus || this._inputField.text.Length > 0;
			if (this._isExpanded != flag)
			{
				this._isExpanded = flag;
				base.StartCoroutine(this.Animate(flag));
			}
		}

		private IEnumerator Animate(bool hasFocus)
		{
			float startWidth = this._layoutElement.flexibleWidth;
			int targetWidth = hasFocus ? 40 : 0;
			float duration = 0.3f;
			if (this._disableWhenCollapsed != null)
			{
				this._disableWhenCollapsed.SetActive(true);
			}
			for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
			{
				float t2 = t * t;
				if (!hasFocus)
				{
					t2 = 1f - (1f - t) * (1f - t);
				}
				this._layoutElement.flexibleWidth = Mathf.Lerp(startWidth, (float)targetWidth, t2);
				yield return null;
			}
			this._layoutElement.flexibleWidth = (float)targetWidth;
			if (this._disableWhenCollapsed != null)
			{
				this._disableWhenCollapsed.SetActive(hasFocus);
			}
			yield break;
		}

		public override void OnSelect(BaseEventData eventData)
		{
			this.UpdateSelectedVisuals(true);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			this.UpdateSelectedVisuals(false);
		}

		private void UpdateSelectedVisuals(bool selected)
		{
			ModioUIInputField modioUIInputField = this._inputField as ModioUIInputField;
			if (modioUIInputField == null)
			{
				return;
			}
			IModioUISelectable.SelectionState state = selected ? IModioUISelectable.SelectionState.Selected : IModioUISelectable.SelectionState.Normal;
			modioUIInputField.DoVisualOnlyStateTransition(state, false);
		}

		public void OnSubmit(BaseEventData eventData)
		{
			this.SelectInputField();
		}

		public void SelectInputField()
		{
			base.StartCoroutine(this.SelectChildDelayed());
		}

		private IEnumerator SelectChildDelayed()
		{
			yield return new WaitForEndOfFrame();
			this._inputField.Select();
			yield break;
		}

		[CompilerGenerated]
		private Task <Awake>g__DelayPopFocusSuppression|6_3()
		{
			ModioInputFieldSelectionWrapper.<<Awake>g__DelayPopFocusSuppression|6_3>d <<Awake>g__DelayPopFocusSuppression|6_3>d;
			<<Awake>g__DelayPopFocusSuppression|6_3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<Awake>g__DelayPopFocusSuppression|6_3>d.<>4__this = this;
			<<Awake>g__DelayPopFocusSuppression|6_3>d.<>1__state = -1;
			<<Awake>g__DelayPopFocusSuppression|6_3>d.<>t__builder.Start<ModioInputFieldSelectionWrapper.<<Awake>g__DelayPopFocusSuppression|6_3>d>(ref <<Awake>g__DelayPopFocusSuppression|6_3>d);
			return <<Awake>g__DelayPopFocusSuppression|6_3>d.<>t__builder.Task;
		}

		private TMP_InputField _inputField;

		private LayoutElement _layoutElement;

		private bool _isExpanded;

		[SerializeField]
		private bool _keepFocusOnSubmit;

		[SerializeField]
		private bool _animateSelectionWidth;

		[SerializeField]
		private GameObject _disableWhenCollapsed;
	}
}
