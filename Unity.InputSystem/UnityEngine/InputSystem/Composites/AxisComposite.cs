using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DisplayStringFormat("{negative}/{positive}")]
	[DisplayName("Positive/Negative Binding")]
	public class AxisComposite : InputBindingComposite<float>
	{
		public float midPoint
		{
			get
			{
				return (this.maxValue + this.minValue) / 2f;
			}
		}

		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			float num = Mathf.Abs(context.ReadValue<float>(this.negative));
			float num2 = Mathf.Abs(context.ReadValue<float>(this.positive));
			bool flag = num > Mathf.Epsilon;
			bool flag2 = num2 > Mathf.Epsilon;
			if (flag == flag2)
			{
				switch (this.whichSideWins)
				{
				case AxisComposite.WhichSideWins.Neither:
					return this.midPoint;
				case AxisComposite.WhichSideWins.Positive:
					flag = false;
					break;
				}
			}
			float midPoint = this.midPoint;
			if (flag)
			{
				return midPoint - (midPoint - this.minValue) * num;
			}
			return midPoint + (this.maxValue - midPoint) * num2;
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			float num = this.ReadValue(ref context);
			if (num < this.midPoint)
			{
				num = Mathf.Abs(num - this.midPoint);
				return NormalizeProcessor.Normalize(num, 0f, Mathf.Abs(this.minValue), 0f);
			}
			num = Mathf.Abs(num - this.midPoint);
			return NormalizeProcessor.Normalize(num, 0f, Mathf.Abs(this.maxValue), 0f);
		}

		[InputControl(layout = "Axis")]
		public int negative;

		[InputControl(layout = "Axis")]
		public int positive;

		[Tooltip("Value to return when the negative side is fully actuated.")]
		public float minValue = -1f;

		[Tooltip("Value to return when the positive side is fully actuated.")]
		public float maxValue = 1f;

		[Tooltip("If both the positive and negative side are actuated, decides what value to return. 'Neither' (default) means that the resulting value is the midpoint between min and max. 'Positive' means that max will be returned. 'Negative' means that min will be returned.")]
		public AxisComposite.WhichSideWins whichSideWins;

		public enum WhichSideWins
		{
			Neither,
			Positive,
			Negative
		}
	}
}
