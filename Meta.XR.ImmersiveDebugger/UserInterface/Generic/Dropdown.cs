using System;
using System.Collections;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Dropdown : Controller
	{
		private bool IsMenuVisible
		{
			get
			{
				return this._flex.Visibility;
			}
		}

		private float DefaultHeight
		{
			get
			{
				return this._baseLabel.RectTransform.rect.size.y;
			}
		}

		public string Label
		{
			get
			{
				return this._baseLabel.Label;
			}
			set
			{
				this._baseLabel.Label = value;
				this._tweak.Value = value;
			}
		}

		private ImageStyle BackgroundStyle
		{
			set
			{
				this._backgroundImageStyle = value;
				this._background.Sprite = value.sprite;
				this._background.Color = value.color;
				this._background.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		internal void SetupMenu(TweakEnum tweak)
		{
			this._tweak = tweak;
			this.Label = this._tweak.Value;
			this.SetupDropdownList();
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._baseLabel = base.Append<ButtonWithLabel>("label");
			this._baseLabel.LayoutStyle = Style.Instantiate<LayoutStyle>("DropdownValueItem");
			this._baseLabel.TextStyle = Style.Load<TextStyle>("MemberValue");
			this._baseLabel.BackgroundStyle = Style.Instantiate<ImageStyle>("DropdownValueBackgroundRoot");
			ButtonWithLabel baseLabel = this._baseLabel;
			baseLabel.Callback = (Action)Delegate.Combine(baseLabel.Callback, new Action(this.OnDropdownClick));
			Icon icon = this._baseLabel.Append<Icon>("icon");
			icon.LayoutStyle = Style.Load<LayoutStyle>("DropdownArrowIcon");
			ImageStyle imageStyle = Style.Load<ImageStyle>("DownArrowIcon");
			icon.Texture = imageStyle.icon;
			icon.Color = imageStyle.color;
			this._rootLayoutStyle = base.Owner.LayoutStyle;
			this._inspectorPanel = base.gameObject.GetComponentInParent<InspectorPanel>();
		}

		private void OnDropdownClick()
		{
			this.SetDropdownMenuVisibility(!this.IsMenuVisible);
		}

		internal void OnMenuItemClick(DropdownMenuItem menuItem)
		{
			this.Label = menuItem.Label;
			this.SetDropdownMenuVisibility(false);
		}

		private void SetDropdownMenuVisibility(bool visible)
		{
			if (visible)
			{
				this._flex.Show();
			}
			else
			{
				this._flex.Hide();
			}
			this._requestBackgroundUpdate = true;
		}

		private void Update()
		{
			if (!this._requestBackgroundUpdate)
			{
				return;
			}
			this._requestBackgroundUpdate = false;
			float num = this.DefaultHeight + this._flex.RectTransform.rect.size.y;
			this._rootLayoutStyle.size.y = (this._flex.Visibility ? num : this.DefaultHeight);
			float num2 = this.DefaultHeight - 2f;
			this._background.RectTransform.sizeDelta = new Vector2(this._background.RectTransform.sizeDelta.x, this._rootLayoutStyle.size.y - num2);
			base.RefreshLayout();
			base.StartCoroutine(this.UpdateScrollPosition(this._flex.Visibility));
		}

		private IEnumerator UpdateScrollPosition(bool dropdownIsShowing)
		{
			if (!dropdownIsShowing)
			{
				yield return new WaitForEndOfFrame();
				this._inspectorPanel.ScrollView.Progress = this._previousScrollPosition;
				yield break;
			}
			this._previousScrollPosition = this._inspectorPanel.ScrollView.Progress;
			ScrollRect scrollRect = this._inspectorPanel.ScrollView.ScrollRect;
			float menuHeight = this._flex.RectTransform.rect.size.y;
			yield return new WaitForEndOfFrame();
			float num = Mathf.Abs(scrollRect.content.rect.size.y - this._inspectorPanel.ScrollView.RectTransform.rect.size.y);
			float num2 = menuHeight / num;
			this._inspectorPanel.ScrollView.Progress = Mathf.Clamp01(this._inspectorPanel.ScrollView.Progress + num2);
			yield break;
		}

		private void HideDropdownItems()
		{
			this._flex.Hide();
		}

		private void SetupDropdownList()
		{
			this._flex = base.Append<Flex>("list");
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("DropdownValuesFlex");
			this._background = this._flex.Append<Background>("background");
			this._background.LayoutStyle = Style.Instantiate<LayoutStyle>("DropdownBackground");
			this.BackgroundStyle = Style.Load<ImageStyle>("DropdownBackground");
			Array array = null;
			FieldInfo fieldInfo = this._tweak.Member as FieldInfo;
			Type type = (fieldInfo != null) ? fieldInfo.FieldType : null;
			PropertyInfo propertyInfo = this._tweak.Member as PropertyInfo;
			Type type2 = (propertyInfo != null) ? propertyInfo.PropertyType : null;
			if (type != null)
			{
				array = Enum.GetValues(type);
			}
			else if (type2 != null)
			{
				array = Enum.GetValues(type2);
			}
			foreach (object obj in array)
			{
				this.AppendValue(obj.ToString());
			}
			this.HideDropdownItems();
		}

		private void AppendValue(string data)
		{
			DropdownMenuItem dropdownMenuItem = this._flex.Append<DropdownMenuItem>("menu_item_" + data);
			dropdownMenuItem.Label = data;
			dropdownMenuItem.RegisterDropdownSourceMenu(this);
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._backgroundImageStyle.colorHover.a = (base.Transparent ? 0.6f : 1f);
			this._background.Color = (base.Transparent ? this._backgroundImageStyle.colorOff : this._backgroundImageStyle.color);
		}

		private Flex _flex;

		private TweakEnum _tweak;

		private ButtonWithLabel _baseLabel;

		private Background _background;

		private bool _requestBackgroundUpdate;

		private LayoutStyle _rootLayoutStyle;

		private InspectorPanel _inspectorPanel;

		private float _previousScrollPosition;

		private ImageStyle _backgroundImageStyle;
	}
}
