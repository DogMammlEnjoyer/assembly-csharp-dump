using System;
using Liv.Lck.Settings;
using Liv.Lck.Tablet;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Liv.Lck.UI
{
	public class LckToggle : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
	{
		public bool IsDisabled { get; private set; }

		private void Awake()
		{
			this._defaultColors = new Tuple<LckButtonColors, LckButtonColors>(this._colors, this._colorsOn);
			this._defaultIcons = new Tuple<Sprite, Sprite>(this._icon, this._iconOn);
		}

		private void Start()
		{
			this.ValidateMeshColors();
			this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
		}

		public void OnToggleValueChanged(bool value)
		{
			this.ValidateIcon();
			this.ValidateColors();
			this.ValidateMeshColors();
			if (this._toggle.group != null && !value)
			{
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.IsDisabled)
			{
				return;
			}
			this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.HighlightedColor : this._colors.HighlightedColor);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.HoverSound);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (this.IsDisabled)
			{
				return;
			}
			this._visuals.anchoredPosition3D = this._togglePressedPosition;
			this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.PressedColor : this._colors.PressedColor);
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
			if (eventData != null)
			{
				this._clickedObject = eventData.pointerEnter;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (this.IsDisabled)
			{
				return;
			}
			if (this._toggle.group == null || !this._stayPressedDownWhenToggled)
			{
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
			}
			if (eventData != null && this._clickedObject != eventData.pointerEnter)
			{
				this._toggle.OnPointerClick(eventData);
				this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.NormalColor : this._colors.NormalColor);
				return;
			}
			this.SetMeshColor((!this._toggle.isOn && this._colorsOn) ? this._colorsOn.HighlightedColor : this._colors.HighlightedColor);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.IsDisabled)
			{
				return;
			}
			this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.NormalColor : this._colors.NormalColor);
			if (this._toggle.group == null || !this._stayPressedDownWhenToggled)
			{
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (this.IsDisabled)
			{
				return;
			}
			if (other.gameObject.CompareTag(LckSettings.Instance.TriggerEnterTag) && this.IsValidTap(other.ClosestPoint(base.transform.position)) && !LCKCameraController.ColliderButtonsInUse)
			{
				LCKCameraController.ColliderButtonsInUse = true;
				this._collided = true;
				this.OnPointerDown(null);
				this._toggle.isOn = !this._toggle.isOn;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (this.IsDisabled)
			{
				return;
			}
			if (this._collided)
			{
				this.OnPointerUp(null);
				this.OnPointerExit(null);
				this._collided = false;
				LCKCameraController.ColliderButtonsInUse = false;
			}
		}

		private bool IsValidTap(Vector3 tapPosition)
		{
			Vector3 to = tapPosition - base.transform.position;
			return Vector3.Angle(-base.transform.forward, to) < 90f;
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				this._collided = false;
				if (this._stayPressedDownWhenToggled && this._toggle.isOn)
				{
					this._visuals.anchoredPosition3D = this._togglePressedPosition;
				}
				else
				{
					this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				}
				this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.NormalColor : this._colors.NormalColor);
			}
		}

		public void SetDisabledState(bool usePressedPosition = false)
		{
			this.IsDisabled = true;
			if (usePressedPosition)
			{
				this._visuals.anchoredPosition3D = this._togglePressedPosition;
			}
			else
			{
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			}
			this._toggle.enabled = false;
		}

		public void RestoreToggleState()
		{
			this.IsDisabled = false;
			this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			this._toggle.enabled = true;
		}

		public void SetToggleVisualsOff()
		{
			this._toggle.SetIsOnWithoutNotify(false);
			this.SetMeshColor(this._colors.NormalColor);
			this.ValidateIcon();
		}

		public void SetToggleVisualsOn()
		{
			this._toggle.SetIsOnWithoutNotify(true);
			this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.NormalColor : this._colors.NormalColor);
			this.ValidateIcon();
		}

		public void SetCustomColors(LckButtonColors colors, LckButtonColors colorsOn)
		{
			this._colors = colors;
			this._colorsOn = colorsOn;
			this.ValidateMeshColors();
		}

		public void RestoreDefaultColors()
		{
			this._colors = this._defaultColors.Item1;
			this._colorsOn = this._defaultColors.Item2;
			this.ValidateMeshColors();
		}

		public void SetCustomIcons(Sprite icon, Sprite iconOn)
		{
			this._icon = icon;
			this._iconOn = iconOn;
			this.ValidateIcon();
		}

		public void RestoreDefaultIcons()
		{
			this._icon = this._defaultIcons.Item1;
			this._iconOn = this._defaultIcons.Item2;
			this.ValidateIcon();
		}

		private void ValidateColors()
		{
			if (this._colors)
			{
				if (this._toggle.isOn && this._colorsOn)
				{
					ColorBlock colors = this._toggle.colors;
					colors.normalColor = this._colorsOn.NormalColor;
					colors.highlightedColor = this._colorsOn.HighlightedColor;
					colors.pressedColor = this._colorsOn.PressedColor;
					colors.selectedColor = this._colorsOn.SelectedColor;
					colors.disabledColor = this._colorsOn.DisabledColor;
					if (this._toggle.colors != colors)
					{
						this._toggle.colors = colors;
						return;
					}
				}
				else
				{
					ColorBlock colors2 = this._toggle.colors;
					colors2.normalColor = this._colors.NormalColor;
					colors2.highlightedColor = this._colors.HighlightedColor;
					colors2.pressedColor = this._colors.PressedColor;
					colors2.selectedColor = this._colors.SelectedColor;
					colors2.disabledColor = this._colors.DisabledColor;
					if (this._toggle.colors != colors2)
					{
						this._toggle.colors = colors2;
					}
				}
			}
		}

		private void ValidateIcon()
		{
			if (this._iconImage && this._icon)
			{
				if (this._toggle.isOn && this._iconOn != null)
				{
					this._iconImage.sprite = this._iconOn;
				}
				else
				{
					this._iconImage.sprite = this._icon;
				}
				if (!this._iconImage.gameObject.activeSelf)
				{
					this._iconImage.gameObject.SetActive(true);
				}
				if (this._labelText && this._labelText.gameObject.activeSelf)
				{
					this._labelText.gameObject.SetActive(false);
					return;
				}
			}
			else
			{
				if (this._iconImage && this._iconImage.gameObject.activeSelf)
				{
					this._iconImage.gameObject.SetActive(false);
				}
				if (this._labelText && !this._labelText.gameObject.activeSelf)
				{
					this._labelText.gameObject.SetActive(true);
				}
			}
		}

		private void ValidateMeshColors()
		{
			if (!this._renderer)
			{
				return;
			}
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			if (this._colorId == 0)
			{
				this._colorId = Shader.PropertyToID("_Color");
			}
			this.SetMeshColor((this._toggle.isOn && this._colorsOn) ? this._colorsOn.NormalColor : this._colors.NormalColor);
		}

		private void OnValidate()
		{
			if (this._labelText)
			{
				this._labelText.text = this._name;
			}
			if (this._toggle.group != null && this._stayPressedDownWhenToggled && this._toggle.isOn)
			{
				this._visuals.anchoredPosition3D = new Vector3(0f, 0f, 40f);
			}
			this.ValidateIcon();
			this.ValidateColors();
			this.ValidateMeshColors();
		}

		private void SetMeshColor(Color color)
		{
			if (this._propertyBlock == null)
			{
				this._propertyBlock = new MaterialPropertyBlock();
			}
			this._propertyBlock.SetColor(this._colorId, color);
			this._renderer.SetPropertyBlock(this._propertyBlock);
		}

		[Header("Settings")]
		[SerializeField]
		private string _name;

		[SerializeField]
		private Sprite _icon;

		[SerializeField]
		private Sprite _iconOn;

		private Tuple<Sprite, Sprite> _defaultIcons;

		[SerializeField]
		private LckButtonColors _colors;

		[SerializeField]
		private LckButtonColors _colorsOn;

		private Tuple<LckButtonColors, LckButtonColors> _defaultColors;

		[SerializeField]
		private Vector3 _togglePressedPosition = new Vector3(0f, 0f, 40f);

		[Header("Toggle Group Settings")]
		[SerializeField]
		private bool _stayPressedDownWhenToggled;

		[Header("References")]
		[SerializeField]
		private TextMeshProUGUI _labelText;

		[SerializeField]
		private Renderer _renderer;

		[SerializeField]
		private RectTransform _visuals;

		[SerializeField]
		private Image _iconImage;

		[SerializeField]
		private Toggle _toggle;

		[Header("Audio")]
		[SerializeField]
		private LckDiscreetAudioController _audioController;

		private bool _collided;

		private GameObject _clickedObject;

		private MaterialPropertyBlock _propertyBlock;

		private int _colorId;
	}
}
