using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class Inspector : Controller, IInspector
	{
		public ImageStyle BackgroundStyle
		{
			set
			{
				this._background.Sprite = value.sprite;
				this._background.Color = value.color;
				this._background.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		public string Title
		{
			get
			{
				return this._title.Label;
			}
			set
			{
				this._title.Label = value;
			}
		}

		public InstanceHandle InstanceHandle
		{
			get
			{
				return this._instanceHandle;
			}
			set
			{
				this._instanceHandle = value;
				Object instance = this._instanceHandle.Instance;
				string title = (instance != null) ? (instance.name + " - " + this._instanceHandle.Type.Name) : (this._instanceHandle.Type.Name ?? "");
				this.Title = title;
				this.UpdateInstanceState(false);
			}
		}

		public Toggle Foldout
		{
			get
			{
				return this._foldout;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._background = base.Append<Background>("background");
			this._background.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			this._backgroundImageStyle = Style.Load<ImageStyle>("InspectorBackground");
			this.BackgroundStyle = this._backgroundImageStyle;
			this._title = base.Append<ToggleWithLabel>("title");
			this._title.LayoutStyle = Style.Load<LayoutStyle>("InspectorTitle");
			this._title.Background.LayoutStyle = Style.Load<LayoutStyle>("InspectorTitleBackground");
			this._title.BackgroundStyle = Style.Load<ImageStyle>("InspectorTitleBackground");
			this._foldout = base.Append<Toggle>("foldout");
			this._foldout.LayoutStyle = Style.Load<LayoutStyle>("InspectorFoldout");
			this._foldout.Icon = Resources.Load<Texture2D>("Textures/caret_right_icon");
			this._foldout.IconStyle = Style.Load<ImageStyle>("InspectorFoldoutIcon");
			this._flex = base.Append<Flex>("list");
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("InspectorFlex");
			this._foldout.StateChanged = new Action<bool>(this.OnStateChanged);
			this._foldout.Callback = new Action(this._foldout.ToggleState);
			this._title.Callback = new Action(this._foldout.ToggleState);
			this._foldout.State = true;
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._background.Color = (base.Transparent ? this._backgroundImageStyle.colorOff : this._backgroundImageStyle.color);
		}

		public void UpdateBackground(bool transparent)
		{
			base.Transparent = transparent;
			this.OnTransparencyChanged();
		}

		public IMember RegisterMember(MemberInfo memberInfo, DebugMember attribute)
		{
			Member member;
			if (!this._registry.TryGetValue(memberInfo, out member))
			{
				member = this._flex.Append<Member>(memberInfo.Name);
				member.LayoutStyle = Style.Instantiate<LayoutStyle>("Member");
				member.Title = (string.IsNullOrEmpty(attribute.DisplayName) ? (memberInfo.Name ?? "") : attribute.DisplayName);
				if (!string.IsNullOrEmpty(attribute.Description))
				{
					member.RegisterDescriptor();
					member.Description = attribute.Description;
				}
				member.PillColor = attribute.Color;
				this._registry.Add(memberInfo, member);
				if (!this._foldout.State)
				{
					this._flex.Forget(member);
				}
			}
			return member;
		}

		public IMember GetMember(MemberInfo memberInfo)
		{
			Member result;
			this._registry.TryGetValue(memberInfo, out result);
			return result;
		}

		private void OnStateChanged(bool state)
		{
			this._foldout.Icon = Resources.Load<Texture2D>(state ? "Textures/caret_down_icon" : "Textures/caret_right_icon");
			if (state)
			{
				foreach (KeyValuePair<MemberInfo, Member> keyValuePair in this._registry)
				{
					this._flex.Remember(keyValuePair.Value);
				}
				this._flex.LayoutStyle = Style.Load<LayoutStyle>("InspectorFlex");
				return;
			}
			this._flex.ForgetAll();
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("InspectorFlexFold");
		}

		private void Update()
		{
			this.UpdateInstanceState(false);
		}

		private void UpdateInstanceState(bool force = false)
		{
			Behaviour behaviour = this.InstanceHandle.Instance as Behaviour;
			if (behaviour != null)
			{
				this.UpdateInstanceState(behaviour != null && behaviour.isActiveAndEnabled, force);
				return;
			}
			this.UpdateInstanceState(true, force);
		}

		private void UpdateInstanceState(bool state, bool force = false)
		{
			if (this._previousEnabledState == state && !force)
			{
				return;
			}
			this._title.TextStyle = Style.Load<TextStyle>(state ? "InspectorTitle" : "InspectorTitleDeactivated");
			this._previousEnabledState = state;
		}

		private InstanceHandle _instanceHandle;

		private ToggleWithLabel _title;

		private Flex _flex;

		private Background _background;

		private readonly Dictionary<MemberInfo, Member> _registry = new Dictionary<MemberInfo, Member>();

		private ImageStyle _backgroundImageStyle;

		private Toggle _foldout;

		private bool _previousEnabledState;
	}
}
