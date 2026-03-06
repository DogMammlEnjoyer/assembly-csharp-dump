using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerIndirectToggle : DebugUIHandlerWidget
	{
		public void Init()
		{
			this.UpdateValueLabel();
			this.valueToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
		}

		private void OnToggleValueChanged(bool value)
		{
			this.setter(this.index, value);
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			this.checkmarkImage.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
			this.checkmarkImage.color = this.colorDefault;
		}

		public override void OnAction()
		{
			bool arg = !this.getter(this.index);
			this.setter(this.index, arg);
			this.UpdateValueLabel();
		}

		internal void UpdateValueLabel()
		{
			if (this.valueToggle != null)
			{
				this.valueToggle.isOn = this.getter(this.index);
			}
		}

		public Text nameLabel;

		public Toggle valueToggle;

		public Image checkmarkImage;

		public Func<int, bool> getter;

		public Action<int, bool> setter;

		internal int index;
	}
}
