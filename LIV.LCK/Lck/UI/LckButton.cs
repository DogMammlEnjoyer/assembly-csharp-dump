using System;
using Liv.Lck.Settings;
using Liv.Lck.Tablet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		private void Start()
		{
			this._propertyBlock = new MaterialPropertyBlock();
			this._colorId = Shader.PropertyToID("_Color");
		}

		public void SetLabelText(string text)
		{
			this._labelText.text = text;
		}

		public void SetIsDisabled(bool isDisabled)
		{
			this._isDisabled = isDisabled;
			this._iconImage.color = (isDisabled ? this._colors.HighlightedColor : Color.white);
			this._labelText.color = (isDisabled ? this._colors.HighlightedColor : Color.white);
			this._button.interactable = !isDisabled;
			this.SetMeshColor(isDisabled ? this._colors.DisabledColor : this._colors.NormalColor);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this.SetMeshColor(this._colors.HighlightedColor);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.HoverSound);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 40f);
			this.SetMeshColor(this._colors.PressedColor);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			this._clickedObject = eventData.pointerEnter;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
			if (this._clickedObject != eventData.pointerEnter)
			{
				this._button.OnPointerClick(eventData);
				return;
			}
			this.SetMeshColor(this._colors.HighlightedColor);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this._isDisabled)
			{
				return;
			}
			this.SetMeshColor(this._colors.NormalColor);
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
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
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 40f);
				this.SetMeshColor(this._colors.PressedColor);
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
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				this.SetMeshColor(this._colors.NormalColor);
				this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
				this._hasCollided = false;
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				this._hasCollided = false;
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				this.SetMeshColor(this._colors.NormalColor);
				if (this._isDisabled)
				{
					this.SetIsDisabled(true);
				}
			}
		}

		private bool IsValidTap(Vector3 tapPosition)
		{
			Vector3 to = tapPosition - base.transform.position;
			return Vector3.Angle(-base.transform.forward, to) < 65f;
		}

		private void OnValidate()
		{
			if (this._labelText)
			{
				this._labelText.text = this._name;
			}
			if (this._colors && this._button)
			{
				ColorBlock colors = this._button.colors;
				colors.normalColor = this._colors.NormalColor;
				colors.highlightedColor = this._colors.HighlightedColor;
				colors.pressedColor = this._colors.PressedColor;
				colors.selectedColor = this._colors.SelectedColor;
				colors.disabledColor = this._colors.DisabledColor;
				if (this._button.colors != colors)
				{
					this._button.colors = colors;
				}
			}
			if (!this._renderer)
			{
				return;
			}
			this._propertyBlock = new MaterialPropertyBlock();
			this.SetMeshColor(this._colors.NormalColor);
		}

		private void SetMeshColor(Color color)
		{
			this._propertyBlock.SetColor(this._colorId, color);
			this._renderer.SetPropertyBlock(this._propertyBlock);
		}

		[Header("Settings")]
		[SerializeField]
		private string _name;

		[SerializeField]
		private LckButtonColors _colors;

		[Header("References")]
		[SerializeField]
		private TextMeshProUGUI _labelText;

		[SerializeField]
		private Image _iconImage;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private RectTransform _visuals;

		[SerializeField]
		private Button _button;

		[Header("Audio")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		private GameObject _clickedObject;

		private bool _hasCollided;

		private MaterialPropertyBlock _propertyBlock;

		private int _colorId;

		private bool _isDisabled;
	}
}
