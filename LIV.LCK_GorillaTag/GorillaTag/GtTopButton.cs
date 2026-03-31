using System;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtTopButton : MonoBehaviour
	{
		public void TapStarted()
		{
			if (this._currentState == GtTopButton.TopButtonState.Disabled)
			{
				return;
			}
			this._visualsTrans.localPosition = this._disabledOrPressedLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			this.EvaluateState(GtTopButton.TopButtonState.Selected);
			this._oppositeButton.EvaluateState(GtTopButton.TopButtonState.Default);
			this.OnTap.Invoke();
		}

		public void TapEnded()
		{
			if (this._currentState == GtTopButton.TopButtonState.Disabled)
			{
				return;
			}
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		private void EvaluateState(GtTopButton.TopButtonState state)
		{
			switch (state)
			{
			case GtTopButton.TopButtonState.Default:
				this.SetDefaultState();
				return;
			case GtTopButton.TopButtonState.Selected:
				this.SetSelectedState();
				return;
			case GtTopButton.TopButtonState.Disabled:
				this.SetDisabledState();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public void SetDefaultState()
		{
			this._bodyRenderer.material = this._settings.DefaultBodyMaterial;
			this._currentState = GtTopButton.TopButtonState.Default;
		}

		public void SetSelectedState()
		{
			this._bodyRenderer.material = this._settings.SelectedBodyMaterial;
			this._currentState = GtTopButton.TopButtonState.Selected;
		}

		public void SetDisabledState()
		{
			if (this._currentState == GtTopButton.TopButtonState.Disabled)
			{
				return;
			}
			this._visualsTrans.localPosition = this._disabledOrPressedLocalPosition;
			this._previousState = this._currentState;
			this._currentState = GtTopButton.TopButtonState.Disabled;
		}

		public void RestoreButtonState()
		{
			if (this._previousState == GtTopButton.TopButtonState.Selected)
			{
				this.SetSelectedState();
				this._visualsTrans.localPosition = this._defaultLocalPosition;
				return;
			}
			if (this._previousState == GtTopButton.TopButtonState.Default)
			{
				this.SetDefaultState();
				this._visualsTrans.localPosition = this._defaultLocalPosition;
			}
		}

		private void OnValidate()
		{
			if (this._currentState == GtTopButton.TopButtonState.Default)
			{
				this.SetDefaultState();
				this._visualsTrans.localPosition = this._defaultLocalPosition;
				return;
			}
			if (this._currentState == GtTopButton.TopButtonState.Selected)
			{
				this.SetSelectedState();
				this._visualsTrans.localPosition = this._defaultLocalPosition;
				return;
			}
			if (this._currentState == GtTopButton.TopButtonState.Disabled)
			{
				this.SetDisabledState();
			}
		}

		[Header("Global Settings")]
		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[Header("Parameters")]
		[SerializeField]
		private GtTopButton.TopButtonState _currentState;

		[SerializeField]
		private GtTopButton.TopButtonState _previousState;

		[Header("Elements")]
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		[SerializeField]
		private Transform _visualsTrans;

		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[SerializeField]
		private GtTopButton _oppositeButton;

		[Space(10f)]
		[Header("Events")]
		public UnityEvent OnTap;

		private Vector3 _defaultLocalPosition = Vector3.zero;

		private Vector3 _disabledOrPressedLocalPosition = new Vector3(0f, -0.045f, 0f);

		private enum TopButtonState
		{
			Default,
			Selected,
			Disabled
		}
	}
}
