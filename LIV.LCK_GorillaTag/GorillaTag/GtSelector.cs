using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtSelector : MonoBehaviour
	{
		private void OnValidate()
		{
			this.EvaluateCameraMode(this._mode);
		}

		private void Awake()
		{
			this.InitSetUp();
		}

		private void Start()
		{
			this.EvaluateState(this._state);
			this.EvaluateCameraMode(this._mode);
		}

		public void TapStarted()
		{
			this.onCameraModeUpdate.Invoke(this._mode);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void ListenToCameraModeChanged(CameraMode mode)
		{
			this.EvaluateState((mode == this._mode) ? SelectorState.Selected : SelectorState.Default);
		}

		private void InitSetUp()
		{
			this._defaultLocalPosition = this._visualsTrans.localPosition;
		}

		private void EvaluateCameraMode(CameraMode mode)
		{
			switch (mode)
			{
			case CameraMode.Selfie:
				this._iconRenderer.sprite = this._settings.SelfieModeAsset.Icon;
				this._textMesh.text = this._settings.SelfieModeAsset.Name.ToUpper();
				return;
			case CameraMode.FirstPerson:
				this._iconRenderer.sprite = this._settings.FirstPersonModeAsset.Icon;
				this._textMesh.text = this._settings.FirstPersonModeAsset.Name.ToUpper();
				return;
			case CameraMode.ThirdPerson:
				this._iconRenderer.sprite = this._settings.ThirdPersonModeAsset.Icon;
				this._textMesh.text = this._settings.ThirdPersonModeAsset.Name.ToUpper();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void EvaluateState(SelectorState state)
		{
			if (state == SelectorState.Default)
			{
				this.SetDefaultState();
				return;
			}
			if (state != SelectorState.Selected)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.SetSelectedState();
		}

		private void SetDefaultState()
		{
			this._bodyRenderer.material = this._settings.DefaultBodyMaterial;
			this._iconRenderer.color = this._settings.PrimaryIconColor;
			this._textMesh.color = this._settings.PrimaryTextColor;
			this._visualsTrans.localPosition = this._defaultLocalPosition;
		}

		private void SetSelectedState()
		{
			this._bodyRenderer.material = this._settings.SelectedBodyMaterial;
			this._iconRenderer.color = this._settings.PrimaryIconColor;
			this._textMesh.color = this._settings.PrimaryTextColor;
			this._visualsTrans.localPosition = this._defaultLocalPosition - new Vector3(0f, 0f, this._settings.ActiveButtonOffset);
		}

		[Header("Global Settings")]
		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[Header("Parameters")]
		[SerializeField]
		private CameraMode _mode;

		[SerializeField]
		private SelectorState _state;

		[Header("Elements")]
		[SerializeField]
		private MeshRenderer _bodyRenderer;

		[SerializeField]
		private SpriteRenderer _iconRenderer;

		[SerializeField]
		private TextMeshPro _textMesh;

		[SerializeField]
		private Transform _visualsTrans;

		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[HideInInspector]
		public UnityEvent<CameraMode> onCameraModeUpdate;

		private Vector3 _defaultLocalPosition;
	}
}
