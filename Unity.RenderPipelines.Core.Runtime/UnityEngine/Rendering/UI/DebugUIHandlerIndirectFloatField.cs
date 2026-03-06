using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerIndirectFloatField : DebugUIHandlerWidget
	{
		public void Init()
		{
			this.UpdateValueLabel();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			this.valueLabel.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
			this.valueLabel.color = this.colorDefault;
		}

		public override void OnIncrement(bool fast)
		{
			this.ChangeValue(fast, 1f);
		}

		public override void OnDecrement(bool fast)
		{
			this.ChangeValue(fast, -1f);
		}

		private void ChangeValue(bool fast, float multiplier)
		{
			float num = this.getter();
			num += this.incStepGetter() * (fast ? this.incStepMultGetter() : 1f) * multiplier;
			this.setter(num);
			this.UpdateValueLabel();
		}

		private void UpdateValueLabel()
		{
			if (this.valueLabel != null)
			{
				this.valueLabel.text = this.getter().ToString("N" + this.decimalsGetter().ToString());
			}
		}

		public Text nameLabel;

		public Text valueLabel;

		public Func<float> getter;

		public Action<float> setter;

		public Func<float> incStepGetter;

		public Func<float> incStepMultGetter;

		public Func<float> decimalsGetter;
	}
}
