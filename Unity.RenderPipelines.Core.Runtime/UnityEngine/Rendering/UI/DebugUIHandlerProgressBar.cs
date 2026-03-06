using System;
using UnityEngine.UI;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerProgressBar : DebugUIHandlerWidget
	{
		protected override void OnEnable()
		{
			this.m_Timer = 0f;
		}

		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Value = base.CastWidget<DebugUI.ProgressBarValue>();
			this.nameLabel.text = this.m_Value.displayName;
			this.UpdateValue();
		}

		public override bool OnSelection(bool fromNext, DebugUIHandlerWidget previous)
		{
			this.nameLabel.color = this.colorSelected;
			return true;
		}

		public override void OnDeselection()
		{
			this.nameLabel.color = this.colorDefault;
		}

		private void Update()
		{
			if (this.m_Timer >= this.m_Value.refreshRate)
			{
				this.UpdateValue();
				this.m_Timer -= this.m_Value.refreshRate;
			}
			this.m_Timer += Time.deltaTime;
		}

		private void UpdateValue()
		{
			float num = (float)this.m_Value.GetValue();
			this.valueLabel.text = this.m_Value.FormatString(num);
			Vector3 localScale = this.progressBarRect.localScale;
			localScale.x = num;
			this.progressBarRect.localScale = localScale;
		}

		public Text nameLabel;

		public Text valueLabel;

		public RectTransform progressBarRect;

		private DebugUI.ProgressBarValue m_Value;

		private float m_Timer;
	}
}
