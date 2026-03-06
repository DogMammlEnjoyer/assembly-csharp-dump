using System;
using Modio.Unity.UI.Components;
using Modio.Unity.UI.Input;
using UnityEngine;

namespace Modio.Unity.UI.Panels
{
	public class ModFilterPanel : ModioPanelBase
	{
		protected override void Awake()
		{
			this._filterDisplay = base.GetComponentInChildren<ModioUIFilterDisplay>();
			base.Awake();
		}

		protected override void CancelPressed()
		{
			base.ClosePanel();
			this._filterDisplay.ApplyFilter();
		}

		public override void DoDefaultSelection()
		{
			GameObject defaultSelection = this._filterDisplay.GetDefaultSelection();
			if (defaultSelection)
			{
				this.SetSelectedGameObject(defaultSelection);
				return;
			}
			base.DoDefaultSelection();
		}

		public override void OnGainedFocus(ModioPanelBase.GainedFocusCause selectionBehaviour)
		{
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.Filter, new Action(this.CancelPressed));
			ModioUIInput.AddHandler(ModioUIInput.ModioAction.FilterClear, new Action(this._filterDisplay.ClearFilter));
			base.OnGainedFocus(selectionBehaviour);
		}

		public override void OnLostFocus()
		{
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.Filter, new Action(this.CancelPressed));
			ModioUIInput.RemoveHandler(ModioUIInput.ModioAction.FilterClear, new Action(this._filterDisplay.ClearFilter));
			base.OnLostFocus();
		}

		private ModioUIFilterDisplay _filterDisplay;
	}
}
