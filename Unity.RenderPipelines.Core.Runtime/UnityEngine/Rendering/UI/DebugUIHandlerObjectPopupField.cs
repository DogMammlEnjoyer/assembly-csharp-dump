using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerObjectPopupField : DebugUIHandlerField<DebugUI.ObjectPopupField>
	{
		internal override void SetWidget(DebugUI.Widget widget)
		{
			base.SetWidget(widget);
			this.m_Index = 0;
		}

		private void ChangeSelectedObject()
		{
			if (this.m_Field == null)
			{
				return;
			}
			IEnumerable<Object> enumerable = this.m_Field.getObjects();
			if (enumerable == null)
			{
				return;
			}
			Object[] array = enumerable.ToArray<Object>();
			int num = array.Length;
			if (this.m_Index >= num)
			{
				this.m_Index = 0;
			}
			else if (this.m_Index < 0)
			{
				this.m_Index = num - 1;
			}
			Object value = array[this.m_Index];
			this.m_Field.SetValue(value);
			this.UpdateValueLabel();
		}

		public override void OnIncrement(bool fast)
		{
			this.m_Index++;
			this.ChangeSelectedObject();
		}

		public override void OnDecrement(bool fast)
		{
			this.m_Index--;
			this.ChangeSelectedObject();
		}

		public override void UpdateValueLabel()
		{
			Object value = this.m_Field.GetValue();
			string labelText = (value != null) ? value.name : "Empty";
			base.SetLabelText(labelText);
		}

		private int m_Index;
	}
}
