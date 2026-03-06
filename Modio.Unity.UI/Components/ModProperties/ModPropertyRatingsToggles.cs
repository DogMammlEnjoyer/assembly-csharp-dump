using System;
using System.Threading.Tasks;
using Modio.Mods;
using Modio.Unity.UI.Panels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyRatingsToggles : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._mod = mod;
			ModRating currentUserRating = mod.CurrentUserRating;
			this._positiveVoteToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.PositiveToggleValueChanged));
			this._negativeVoteToggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.NegativeToggleValueChanged));
			this._positiveVoteToggle.isOn = (currentUserRating == ModRating.Positive);
			this._negativeVoteToggle.isOn = (currentUserRating == ModRating.Negative);
			this._positiveVoteToggle.onValueChanged.AddListener(new UnityAction<bool>(this.PositiveToggleValueChanged));
			this._negativeVoteToggle.onValueChanged.AddListener(new UnityAction<bool>(this.NegativeToggleValueChanged));
		}

		private void PositiveToggleValueChanged(bool arg0)
		{
			Task<Error> task = this._mod.RateMod(this._positiveVoteToggle.isOn ? ModRating.Positive : ModRating.None);
			ModioErrorPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.MonitorTaskThenOpenPanelIfError(task);
		}

		private void NegativeToggleValueChanged(bool toggleValue)
		{
			Task<Error> task = this._mod.RateMod(this._negativeVoteToggle.isOn ? ModRating.Negative : ModRating.None);
			ModioErrorPanelGeneric panelOfType = ModioPanelManager.GetPanelOfType<ModioErrorPanelGeneric>();
			if (panelOfType == null)
			{
				return;
			}
			panelOfType.MonitorTaskThenOpenPanelIfError(task);
		}

		[SerializeField]
		private Toggle _positiveVoteToggle;

		[SerializeField]
		private Toggle _negativeVoteToggle;

		private Mod _mod;
	}
}
