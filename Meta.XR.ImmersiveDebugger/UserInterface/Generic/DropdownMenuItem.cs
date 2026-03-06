using System;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	internal class DropdownMenuItem : ButtonWithLabel
	{
		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			base.LayoutStyle = Style.Load<LayoutStyle>("DropdownValueItem");
			base.TextStyle = Style.Instantiate<TextStyle>("MemberValue");
			base.BackgroundStyle = Style.Instantiate<ImageStyle>("DropdownValueBackground");
		}

		internal void RegisterDropdownSourceMenu(Dropdown dropdown)
		{
			this._dropdown = dropdown;
		}

		public override void OnPointerClick()
		{
			base.OnPointerClick();
			Dropdown dropdown = this._dropdown;
			if (dropdown == null)
			{
				return;
			}
			dropdown.OnMenuItemClick(this);
		}

		private Dropdown _dropdown;
	}
}
