using System;
using Modio.Unity.UI.Components;
using Modio.Unity.UI.Components.Selectables;
using UnityEngine;

namespace Modio.Unity.UI.Panels
{
	public class ModOptionsPopupPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			base.Awake();
			this._modioUIMod = base.GetComponent<ModioUIMod>();
		}

		public void OpenPanel(ModioUIMod modUI)
		{
			base.OpenPanel();
			if (this._popupPositioning == null)
			{
				this._popupPositioning = base.GetComponentInChildren<ModioPopupPositioning>();
			}
			this._modioUIMod.SetMod(modUI.Mod);
			ModioUIButton component = modUI.GetComponent<ModioUIButton>();
			this._buttonToHighlight = component;
			this._popupPositioning.PositionNextTo((RectTransform)modUI.transform);
		}

		public override void OnLostFocus()
		{
			if (this._buttonToHighlight != null)
			{
				this._buttonToHighlight.DoVisualOnlyStateTransition(IModioUISelectable.SelectionState.Normal, false);
			}
			base.OnLostFocus();
		}

		public override void FocusedPanelLateUpdate()
		{
			base.FocusedPanelLateUpdate();
			if (this._buttonToHighlight != null)
			{
				this._buttonToHighlight.DoVisualOnlyStateTransition(IModioUISelectable.SelectionState.Highlighted, false);
			}
		}

		private ModioUIMod _modioUIMod;

		private RectTransform _rectToPosition;

		private RectTransform _rectToPositionWithin;

		private ModioPopupPositioning _popupPositioning;

		private ModioUIButton _buttonToHighlight;
	}
}
