using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Subscription
{
	public class FeatureTogglesScreen : MonoBehaviour, IGorillaSliceableSimple
	{
		private int NumPages
		{
			get
			{
				if (this._features.Length % 3 != 0)
				{
					return this._features.Length / 3 + 1;
				}
				return this._features.Length / 3;
			}
		}

		private int LastPageIndex
		{
			get
			{
				return Math.Max(0, this.NumPages - 1);
			}
		}

		private void Awake()
		{
			this._nextButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnNextButtonPressed));
			this._backButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnBackButtonPressed));
			this._exitButton.button.buttonPressed.AddListener(new UnityAction<SITouchscreenButton.SITouchscreenButtonType, int, int>(this.OnExitButtonPressed));
			this.MarkDirty();
		}

		private void OnNextButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
			this._currentPage++;
			if (this._currentPage > this.LastPageIndex)
			{
				this._currentPage = this.LastPageIndex;
			}
			this.MarkDirty();
		}

		private void OnBackButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
			this._currentPage--;
			if (this._currentPage < 0)
			{
				this._currentPage = 0;
			}
			this.MarkDirty();
		}

		private void OnExitButtonPressed(SITouchscreenButton.SITouchscreenButtonType type, int data, int actorNr)
		{
		}

		public void SliceUpdate()
		{
			if (!this._dirty)
			{
				return;
			}
			this._backButton.gameObject.SetActive(this._currentPage != 0);
			this._nextButton.gameObject.SetActive(this._currentPage != this.LastPageIndex);
			this.UpdateFeatureToggleUI();
			this._dirty = false;
		}

		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.MarkDirty();
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		private void UpdateFeatureToggleUI()
		{
			for (int i = 0; i < this._featureToggleUi.Length; i++)
			{
				FeatureToggleUI featureToggleUI = this._featureToggleUi[i];
				int num = this._currentPage * 3 + i;
				bool flag = num < this._features.Length;
				featureToggleUI.gameObject.SetActive(flag);
				if (flag)
				{
					FeatureTogglesScreen.Feature feature = this._features[num];
					featureToggleUI.AttachToFeature(feature);
				}
			}
		}

		public void MarkDirty()
		{
			this._dirty = true;
		}

		private const int TogglesPerPage = 3;

		[SerializeField]
		private FeatureTogglesScreen.Feature[] _features;

		[SerializeField]
		private SITouchscreenButtonContainer _nextButton;

		[SerializeField]
		private SITouchscreenButtonContainer _backButton;

		[SerializeField]
		private SITouchscreenButtonContainer _exitButton;

		[SerializeField]
		private FeatureToggleUI[] _featureToggleUi;

		private int _currentPage;

		private bool _dirty = true;

		[Serializable]
		public class Feature
		{
			public string DisplayName = string.Empty;

			public SubscriptionManager.SubscriptionFeatures Value;

			public UnityEvent OnPressed;

			public UnityEvent<bool> OnToggle;

			public string UnavailableMessage = "NOT AVAILABLE ON THIS DEVICE";
		}
	}
}
