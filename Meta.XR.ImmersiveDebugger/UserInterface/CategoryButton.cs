using System;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	internal class CategoryButton : Toggle
	{
		internal Category Category
		{
			get
			{
				return this._category;
			}
			set
			{
				this._category = value;
				this._label.Content = this._category.Label;
			}
		}

		internal int Counter
		{
			get
			{
				return this._counter;
			}
			set
			{
				this._counter = value;
				this._counter = Math.Max(0, this._counter);
				Label subLabel = this._subLabel;
				int counter = this._counter;
				string content;
				if (counter != 0)
				{
					if (counter != 1)
					{
						content = string.Format("{0} objects tracked", this._counter);
					}
					else
					{
						content = "1 object tracked";
					}
				}
				else
				{
					content = "No objects tracked";
				}
				subLabel.Content = content;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._flex = base.Append<Flex>("flex");
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("CategoryButtonFlex");
			this._label = this._flex.Append<Label>("label");
			this._label.LayoutStyle = Style.Load<LayoutStyle>("CategoryLabel");
			this._label.TextStyle = Style.Load<TextStyle>("CategoryLabel");
			this._subLabel = this._flex.Append<Label>("sublabel");
			this._subLabel.LayoutStyle = Style.Load<LayoutStyle>("CategorySubLabel");
			this._subLabel.TextStyle = Style.Load<TextStyle>("CategorySubLabel");
			base.IconStyle = Style.Load<ImageStyle>("None");
			base.BackgroundStyle = Style.Instantiate<ImageStyle>("CategoryButtonBackground");
		}

		private Category _category;

		private int _counter;

		private Label _label;

		private Label _subLabel;

		private Flex _flex;
	}
}
