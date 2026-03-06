using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Subscription
{
	[RequireComponent(typeof(SITouchscreenButtonContainer))]
	public class FeatureToggleUI : MonoBehaviour
	{
		public SITouchscreenButtonContainer ButtonContainer { get; private set; }

		public string LabelText
		{
			get
			{
				return this._label.text;
			}
			set
			{
				this._label.text = value;
			}
		}

		private void Awake()
		{
			this.ButtonContainer = base.gameObject.GetComponent<SITouchscreenButtonContainer>();
		}

		public void AttachToFeature(FeatureTogglesScreen.Feature feature)
		{
			this.ButtonContainer.button.buttonPressed.RemoveAllListeners();
			this.ButtonContainer.button.buttonToggled.RemoveAllListeners();
			this.LabelText = feature.DisplayName;
			bool state2 = SubscriptionManager.GetSubscriptionSettingBool(feature.Value);
			bool flag = SubscriptionManager.IsSubscriptionFeatureAvailable(feature.Value);
			bool flag2 = true;
			if (flag && flag2)
			{
				this.ButtonContainer.button.buttonPressed.AddListener(delegate(SITouchscreenButton.SITouchscreenButtonType type, int data, int nr)
				{
					this.OnPressed(nr, feature);
				});
				this.ButtonContainer.button.buttonToggled.AddListener(delegate(SITouchscreenButton.SITouchscreenButtonType type, int data, int nr, bool state)
				{
					this.OnToggled(nr, feature, state);
				});
				this._unavailable.gameObject.SetActive(false);
			}
			else
			{
				state2 = false;
				this._unavailable.gameObject.SetActive(true);
				if (!flag2)
				{
					this._unavailable.text = "ENABLE PERMISSION IN QUEST SETTINGS";
				}
				else
				{
					this._unavailable.text = "NOT AVAILABLE ON THIS DEVICE";
				}
			}
			this.ButtonContainer.button.SetToggleState(state2, false);
			this.ButtonContainer.UpdateToggleVisual();
		}

		private void OnPressed(int actorNr, FeatureTogglesScreen.Feature feature)
		{
			if (Time.time < this._disableUntil)
			{
				return;
			}
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this._disableUntil = Time.time + 0.5f;
			feature.OnPressed.Invoke();
		}

		private void OnToggled(int actorNr, FeatureTogglesScreen.Feature feature, bool state)
		{
			if (Time.time < this._disableUntil)
			{
				return;
			}
			if (actorNr != NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return;
			}
			this._disableUntil = Time.time + 0.5f;
			feature.OnToggle.Invoke(state);
			this.ButtonContainer.button.SetToggleState(state, false);
			this.ButtonContainer.UpdateToggleVisual();
		}

		[SerializeField]
		private TextMeshPro _label;

		[SerializeField]
		private TextMeshPro _unavailable;

		private const float DEBOUNCE_TIME = 0.5f;

		private float _disableUntil = float.MinValue;
	}
}
