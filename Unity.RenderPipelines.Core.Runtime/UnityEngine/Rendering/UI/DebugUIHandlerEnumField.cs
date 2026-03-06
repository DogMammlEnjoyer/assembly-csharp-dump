using System;

namespace UnityEngine.Rendering.UI
{
	public class DebugUIHandlerEnumField : DebugUIHandlerField<DebugUI.EnumField>
	{
		public override void OnIncrement(bool fast)
		{
			if (this.m_Field.enumValues.Length == 0)
			{
				return;
			}
			int[] enumValues = this.m_Field.enumValues;
			int num = this.m_Field.currentIndex;
			if (num == enumValues.Length - 1)
			{
				num = 0;
			}
			else if (fast)
			{
				int[] quickSeparators = this.m_Field.quickSeparators;
				if (quickSeparators == null)
				{
					this.m_Field.InitQuickSeparators();
					quickSeparators = this.m_Field.quickSeparators;
				}
				int num2 = 0;
				while (num2 < quickSeparators.Length && num + 1 > quickSeparators[num2])
				{
					num2++;
				}
				if (num2 == quickSeparators.Length)
				{
					num = 0;
				}
				else
				{
					num = quickSeparators[num2];
				}
			}
			else
			{
				num++;
			}
			this.m_Field.SetValue(enumValues[num]);
			this.m_Field.currentIndex = num;
			this.UpdateValueLabel();
		}

		public override void OnDecrement(bool fast)
		{
			if (this.m_Field.enumValues.Length == 0)
			{
				return;
			}
			int[] enumValues = this.m_Field.enumValues;
			int num = this.m_Field.currentIndex;
			if (num == 0)
			{
				if (fast)
				{
					int[] quickSeparators = this.m_Field.quickSeparators;
					if (quickSeparators == null)
					{
						this.m_Field.InitQuickSeparators();
						quickSeparators = this.m_Field.quickSeparators;
					}
					num = quickSeparators[quickSeparators.Length - 1];
				}
				else
				{
					num = enumValues.Length - 1;
				}
			}
			else if (fast)
			{
				int[] quickSeparators2 = this.m_Field.quickSeparators;
				if (quickSeparators2 == null)
				{
					this.m_Field.InitQuickSeparators();
					quickSeparators2 = this.m_Field.quickSeparators;
				}
				int num2 = quickSeparators2.Length - 1;
				while (num2 > 0 && num <= quickSeparators2[num2])
				{
					num2--;
				}
				num = quickSeparators2[num2];
			}
			else
			{
				num--;
			}
			this.m_Field.SetValue(enumValues[num]);
			this.m_Field.currentIndex = num;
			this.UpdateValueLabel();
		}

		public override void UpdateValueLabel()
		{
			int num = this.m_Field.currentIndex;
			if (num < 0)
			{
				num = 0;
			}
			base.SetLabelText(this.m_Field.enumNames[num].text);
		}
	}
}
