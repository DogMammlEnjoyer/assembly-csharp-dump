using System;
using Meta.XR.ImmersiveDebugger.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Slider : Button, IDragHandler, IEventSystemHandler, IInitializePotentialDragHandler
	{
		internal Tweak Tweak { get; set; }

		public ImageStyle EmptyBackgroundStyle
		{
			set
			{
				this._emptyBackgroundStyle = value;
				this._emptyBackground.Sprite = value.sprite;
				this._emptyBackground.Color = value.color;
				this._emptyBackground.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		public ImageStyle FillBackgroundStyle
		{
			set
			{
				this._fillBackgroundStyle = value;
				this._fillBackground.Sprite = value.sprite;
				this._fillBackground.Color = value.color;
				this._fillBackground.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			Background background = base.Append<Background>("raycast_background");
			background.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			background.Color = new Color(0f, 0f, 0f, 0f);
			background.Sprite = null;
			this._emptyBackground = base.Append<Background>("empty_background");
			this._emptyBackground.LayoutStyle = Style.Load<LayoutStyle>("SliderBackground");
			this._emptyBackground.RaycastTarget = true;
			this._fillBackground = base.Append<Background>("fill_background");
			this._fillBackground.LayoutStyle = Style.Load<LayoutStyle>("SliderFill");
			this._pill = base.Append<Icon>("pill");
			this._pill.LayoutStyle = Style.Load<LayoutStyle>("SliderPill");
			this._pill.Texture = Resources.Load<Texture2D>("Textures/icon_background_02");
			this._pill.Color = Color.white;
			this._pill.RaycastTarget = true;
		}

		private void UpdatePillPosition()
		{
			Tweak tweak = this.Tweak;
			if (tweak == null || !tweak.Valid)
			{
				return;
			}
			float width = base.RectTransform.rect.width;
			float num = this.Tweak.Tween * width;
			this._pill.RectTransform.anchoredPosition = new Vector2(num, 0f);
			this._fillBackground.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
		}

		private void Update()
		{
			this.UpdatePillPosition();
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			Vector2 vector;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.RectTransform, eventData.position, eventData.enterEventCamera, out vector))
			{
				return;
			}
			Rect rect = base.RectTransform.rect;
			this.Tweak.Tween = Mathf.InverseLerp(rect.min.x, rect.max.x, vector.x);
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.Tweak != null && eventData.button == PointerEventData.InputButton.Left;
		}

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			eventData.useDragThreshold = false;
		}

		private Background _emptyBackground;

		private Background _fillBackground;

		private Icon _pill;

		private ImageStyle _emptyBackgroundStyle;

		private ImageStyle _fillBackgroundStyle;
	}
}
