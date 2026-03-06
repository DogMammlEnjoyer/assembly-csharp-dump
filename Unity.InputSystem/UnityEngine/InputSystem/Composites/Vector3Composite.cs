using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DisplayStringFormat("{up}+{down}/{left}+{right}/{forward}+{backward}")]
	[DisplayName("Up/Down/Left/Right/Forward/Backward Composite")]
	public class Vector3Composite : InputBindingComposite<Vector3>
	{
		public override Vector3 ReadValue(ref InputBindingCompositeContext context)
		{
			if (this.mode == Vector3Composite.Mode.Analog)
			{
				float num = context.ReadValue<float>(this.up);
				float num2 = context.ReadValue<float>(this.down);
				float num3 = context.ReadValue<float>(this.left);
				float num4 = context.ReadValue<float>(this.right);
				float num5 = context.ReadValue<float>(this.forward);
				float num6 = context.ReadValue<float>(this.backward);
				return new Vector3(num4 - num3, num - num2, num5 - num6);
			}
			float num7 = context.ReadValueAsButton(this.up) ? 1f : 0f;
			float num8 = context.ReadValueAsButton(this.down) ? -1f : 0f;
			float num9 = context.ReadValueAsButton(this.left) ? -1f : 0f;
			float num10 = context.ReadValueAsButton(this.right) ? 1f : 0f;
			float num11 = context.ReadValueAsButton(this.forward) ? 1f : 0f;
			float num12 = context.ReadValueAsButton(this.backward) ? -1f : 0f;
			Vector3 normalized = new Vector3(num9 + num10, num7 + num8, num11 + num12);
			if (this.mode == Vector3Composite.Mode.DigitalNormalized)
			{
				normalized = normalized.normalized;
			}
			return normalized;
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return this.ReadValue(ref context).magnitude;
		}

		[InputControl(layout = "Axis")]
		public int up;

		[InputControl(layout = "Axis")]
		public int down;

		[InputControl(layout = "Axis")]
		public int left;

		[InputControl(layout = "Axis")]
		public int right;

		[InputControl(layout = "Axis")]
		public int forward;

		[InputControl(layout = "Axis")]
		public int backward;

		public Vector3Composite.Mode mode;

		public enum Mode
		{
			Analog,
			DigitalNormalized,
			Digital
		}
	}
}
