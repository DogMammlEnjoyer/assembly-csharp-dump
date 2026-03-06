using System;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerObjectList : DebugUIHandlerField<DebugUI.ObjectListField>
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Index = 0;
		}

		public override void OnIncrement(bool fast)
		{
			this.m_Index++;
			this.UpdateValueLabel();
		}

		public override void OnDecrement(bool fast)
		{
			this.m_Index--;
			this.UpdateValueLabel();
		}

		public override void UpdateValueLabel()
		{
			string labelText = "Empty";
			Object[] value = this.m_Field.GetValue();
			if (value != null)
			{
				this.m_Index = Math.Clamp(this.m_Index, 0, value.Length - 1);
				labelText = value[this.m_Index].name;
			}
			base.SetLabelText(labelText);
		}

		private int m_Index;
	}
}
