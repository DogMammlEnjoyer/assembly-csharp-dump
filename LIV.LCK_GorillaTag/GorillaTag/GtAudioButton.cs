using System;
using UnityEngine;
using UnityEngine.Events;

namespace Liv.Lck.GorillaTag
{
	public class GtAudioButton : MonoBehaviour
	{
		private void OnValidate()
		{
			this.SetUp();
			this.SetProgress(this._progress);
		}

		private void Start()
		{
			this.SetUp();
			this._defaultLocalPosition = this._visualsTrans.localPosition;
		}

		public void TapStarted()
		{
			this.onTap.Invoke(new UnityAction<bool>(this.ProcessState));
			this._visualsTrans.localPosition = this._defaultLocalPosition + Vector3.forward * -this._settings.ActiveButtonOffset;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void TapEnded()
		{
			this._visualsTrans.localPosition = this._defaultLocalPosition;
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		public void SetProgress(float progress)
		{
			this._propertyBlock.SetFloat(this.PROGRESS, progress);
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		public void SetActiveState(bool isActive)
		{
			this._isActive = isActive;
			if (!this._isActive)
			{
				this._iconRenderer.color = this._settings.InactiveIconColor;
			}
		}

		private void SetUp()
		{
			this._propertyBlock = new MaterialPropertyBlock();
			this.ProcessState(true);
		}

		private void ProcessState(bool isOn)
		{
			if (!this._isActive)
			{
				return;
			}
			this._iconRenderer.sprite = (isOn ? this._onIcon : this._offIcon);
			this._iconRenderer.color = (isOn ? this._settings.PrimaryIconColor : this._settings.SecondaryIconColor);
			this._propertyBlock.SetFloat(this.IS_ON, (float)(isOn ? 1 : 0));
			this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
		}

		public UnityEvent<UnityAction<bool>> onTap;

		[SerializeField]
		private GtUiSettings _settings;

		[Space(10f)]
		[Header("Parameters")]
		[SerializeField]
		[Range(0f, 1f)]
		private float _progress;

		[Space(10f)]
		[Header("Elements")]
		[SerializeField]
		private SpriteRenderer _iconRenderer;

		[SerializeField]
		private Sprite _onIcon;

		[SerializeField]
		private Sprite _offIcon;

		[SerializeField]
		private MeshRenderer _bodyRenderer;

		[SerializeField]
		private Transform _visualsTrans;

		[SerializeField]
		private LckDiscreetAudioController _audioController;

		[SerializeField]
		private bool _isActive = true;

		private string PROGRESS = "_Progress";

		private string IS_ON = "_Is_On";

		private Vector3 _defaultLocalPosition;

		private MaterialPropertyBlock _propertyBlock;
	}
}
