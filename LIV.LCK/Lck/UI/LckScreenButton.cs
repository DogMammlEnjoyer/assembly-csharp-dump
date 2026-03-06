using System;
using Liv.Lck.Settings;
using Liv.Lck.Tablet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckScreenButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this.SetIconColor(this._colors.HighlightedColor);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.HoverSound);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this.SetIconColor(this._colors.PressedColor);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			this._clickedObject = eventData.pointerEnter;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
			this.SetIconColor(this._colors.HighlightedColor);
			if (this._clickedObject != eventData.pointerEnter)
			{
				this.SetDefaultButtonColors();
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this.SetIconColor(this._colors.NormalColor);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this._isDisabled)
			{
				return;
			}
			if (other.gameObject.tag == LckSettings.Instance.TriggerEnterTag && this.IsValidTap(other.ClosestPoint(base.transform.position)) && !LCKCameraController.ColliderButtonsInUse)
			{
				LCKCameraController.ColliderButtonsInUse = true;
				this._hasCollided = true;
				this.SetIconColor(this._colors.PressedColor);
				this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
				this._button.onClick.Invoke();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this._isDisabled)
			{
				return;
			}
			if (other.gameObject.tag == LckSettings.Instance.TriggerEnterTag && this._hasCollided)
			{
				this.SetIconColor(this._colors.NormalColor);
				this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
				this._hasCollided = false;
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private bool IsValidTap(Vector3 tapPosition)
		{
			Vector3 to = tapPosition - base.transform.position;
			return Vector3.Angle(-base.transform.forward, to) < 90f;
		}

		public void DisableForDuration(float duration)
		{
			this._isDisabled = true;
			this.SetIconColor(this._colors.NormalColor);
			this._icon.gameObject.SetActive(false);
			base.Invoke("ReEnableButton", duration);
		}

		private void ReEnableButton()
		{
			this._icon.gameObject.SetActive(true);
			this._isDisabled = false;
		}

		private void SetIconColor(Color color)
		{
			if (this._icon != null)
			{
				this._icon.color = color;
			}
		}

		public void SetDefaultButtonColors()
		{
			this.SetIconColor(this._colors.NormalColor);
		}

		private void OnValidate()
		{
			if (this._icon && this._colors)
			{
				this.SetDefaultButtonColors();
			}
		}

		[Header("References")]
		[SerializeField]
		private LckButtonColors _colors;

		[SerializeField]
		private Button _button;

		[SerializeField]
		private Image _icon;

		[Header("Audio")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		private bool _isDisabled;

		private bool _hasCollided;

		private GameObject _clickedObject;
	}
}
