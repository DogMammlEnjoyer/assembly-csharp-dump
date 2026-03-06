using System;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class Member : Controller, IMember
	{
		public string Title
		{
			get
			{
				return this._title.Content;
			}
			set
			{
				this._title.Content = value.ToDisplayText(22);
			}
		}

		public string Description
		{
			get
			{
				return this._description.Content;
			}
			set
			{
				this._description.Content = value;
			}
		}

		public Color PillColor
		{
			set
			{
				this._defaultPillColor = value;
				this._transparentPillColor = value;
				this._transparentPillColor.a = 0.8f;
				this._pill.Color = (base.Transparent ? this._transparentPillColor : this._defaultPillColor);
			}
		}

		public ImageStyle PillStyle
		{
			set
			{
				this._pill.Sprite = value.sprite;
				this._pill.Color = value.color;
				this._pill.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._flex = base.Append<Flex>("list");
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("MemberFlex");
			this._pill = this._flex.Append<Background>("pill");
			this._pill.LayoutStyle = Style.Load<LayoutStyle>("PillVertical");
			this._pillBackgroundStyle = Style.Load<ImageStyle>("PillInfo");
			this.PillStyle = this._pillBackgroundStyle;
			this._title = this._flex.Append<Label>("title");
			this._title.LayoutStyle = Style.Load<LayoutStyle>("MemberTitle");
			this._title.TextStyle = Style.Load<TextStyle>("MemberTitle");
			this._verticalFlex = base.Append<Flex>("vertical");
			this._verticalFlex.LayoutStyle = Style.Load<LayoutStyle>("VerticalValueFlex");
			this._valueFlex = this._verticalFlex.Append<Flex>("values");
			this._valueFlex.LayoutStyle = Style.Instantiate<LayoutStyle>("MemberValueFlex");
		}

		public void RegisterDescriptor()
		{
			this._description = this._verticalFlex.Append<TextArea>("description");
			this._description.Label.LayoutStyle.margin = new Vector2(4f, 4f);
			this._description.Background.LayoutStyle.margin = new Vector2(0f, 0f);
			this._description.LayoutStyle = Style.Instantiate<LayoutStyle>("MemberDescriptor");
			this._description.TextStyle = Style.Load<TextStyle>("MemberDescriptorValue");
			this._description.BackgroundStyle = Style.Load<ImageStyle>("MemberDescriptionBackground");
			base.RefreshLayout();
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._pill.Color = (base.Transparent ? this._transparentPillColor : this._defaultPillColor);
		}

		public ActionHook GetAction()
		{
			if (!(this._action != null))
			{
				return null;
			}
			return this._action.Action;
		}

		public void RegisterAction(ActionHook action)
		{
			if (this._action == null)
			{
				this._action = this._valueFlex.Append<ButtonForAction>("action");
				this._action.LayoutStyle = Style.Load<LayoutStyle>("MemberAction");
				this._action.TextStyle = Style.Load<TextStyle>("MemberValue");
				this._action.BackgroundStyle = Style.Load<ImageStyle>("MemberActionBackground");
				string input = string.IsNullOrEmpty(action.Attribute.DisplayName) ? (action.MemberInfo.Name ?? "") : action.Attribute.DisplayName;
				this._action.Label = input.ToDisplayText(64);
				this._flex.Hide();
			}
			this._action.Action = action;
		}

		public GizmoHook GetGizmo()
		{
			if (!(this._gizmo != null))
			{
				return null;
			}
			return this._gizmo.Hook;
		}

		public void RegisterGizmo(GizmoHook gizmo)
		{
			if (this._gizmo == null)
			{
				this._gizmo = this._valueFlex.Append<ToggleForGizmo>("gizmo");
				this._gizmo.LayoutStyle = Style.Load<LayoutStyle>("MemberButton");
				this._gizmo.Icon = Resources.Load<Texture2D>("Textures/eye_icon");
				this._gizmo.IconStyle = Style.Load<ImageStyle>("MiniButtonIcon");
			}
			this._gizmo.Hook = gizmo;
		}

		public Watch GetWatch()
		{
			if (!(this._values != null))
			{
				return null;
			}
			return this._values.Watch;
		}

		public void RegisterWatch(Watch watch)
		{
			if (this._values == null)
			{
				this._values = this._valueFlex.Append<Values>("watch");
			}
			this._values.Setup(watch);
		}

		public void RegisterEnum(TweakEnum tweak)
		{
			Dropdown dropdown = this._valueFlex.Append<Dropdown>("dropdown");
			dropdown.LayoutStyle = Style.Instantiate<LayoutStyle>("DropdownMemberValue");
			dropdown.SetupMenu(tweak);
		}

		public void RegisterTexture(WatchTexture watchTexture)
		{
			Image image = this._valueFlex.Append<Image>("texture");
			image.LayoutStyle = Style.Instantiate<LayoutStyle>("TextureValue");
			image.Setup(watchTexture);
			base.RefreshLayout();
		}

		public Tweak GetTweak()
		{
			if (!(this._slider != null))
			{
				return null;
			}
			return this._slider.Tweak;
		}

		public void RegisterTweak(Tweak tweak)
		{
			if (tweak is Tweak<float> || tweak is Tweak<int>)
			{
				this.AddSlider(tweak);
				return;
			}
			if (!(tweak is Tweak<bool>))
			{
				return;
			}
			this.AddToggle(tweak);
		}

		private void AddToggle(Tweak tweak)
		{
			if (this._switch == null)
			{
				this._switch = this._valueFlex.Prepend<Switch>("switch");
				this._switch.LayoutStyle = Style.Load<LayoutStyle>("MemberButtonToggle");
				this._switch.SetToggleIcons(Resources.Load<Texture2D>("Textures/toggle_on"), Resources.Load<Texture2D>("Textures/toggle_off"));
				this._switch.IconStyle = Style.Load<ImageStyle>("ToggleButtonIcon");
				this._switch.Callback = delegate()
				{
					this._switch.State = !this._switch.State;
				};
			}
			this._switch.Tweak = tweak;
		}

		private void AddSlider(Tweak tweak)
		{
			if (this._slider == null)
			{
				this._slider = this._valueFlex.Append<Slider>("slider");
				this._slider.LayoutStyle = Style.Load<LayoutStyle>("MemberSlider");
				this._slider.EmptyBackgroundStyle = Style.Load<ImageStyle>("MemberValueBackground");
				this._slider.FillBackgroundStyle = Style.Load<ImageStyle>("MemberActionBackground");
			}
			this._slider.Tweak = tweak;
		}

		private Label _title;

		private TextArea _description;

		private Flex _flex;

		private Flex _valueFlex;

		private Flex _verticalFlex;

		private Values _values;

		private ButtonForAction _action;

		private Slider _slider;

		private Switch _switch;

		private ToggleForGizmo _gizmo;

		private Background _pill;

		private ImageStyle _pillBackgroundStyle;

		private Color _defaultPillColor;

		private Color _transparentPillColor;
	}
}
