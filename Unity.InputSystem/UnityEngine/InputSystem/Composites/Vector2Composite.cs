using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Composites
{
	[DisplayStringFormat("{up}/{left}/{down}/{right}")]
	[DisplayName("Up/Down/Left/Right Composite")]
	public class Vector2Composite : InputBindingComposite<Vector2>
	{
		public override Vector2 ReadValue(ref InputBindingCompositeContext context)
		{
			Vector2Composite.Mode mode = this.mode;
			if (mode == Vector2Composite.Mode.Analog)
			{
				float num = context.ReadValue<float>(this.up);
				float num2 = context.ReadValue<float>(this.down);
				float num3 = context.ReadValue<float>(this.left);
				float num4 = context.ReadValue<float>(this.right);
				return DpadControl.MakeDpadVector(num, num2, num3, num4);
			}
			bool flag = context.ReadValueAsButton(this.up);
			bool flag2 = context.ReadValueAsButton(this.down);
			bool flag3 = context.ReadValueAsButton(this.left);
			bool flag4 = context.ReadValueAsButton(this.right);
			if (!this.normalize)
			{
				mode = Vector2Composite.Mode.Digital;
			}
			return DpadControl.MakeDpadVector(flag, flag2, flag3, flag4, mode == Vector2Composite.Mode.DigitalNormalized);
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

		[Obsolete("Use Mode.DigitalNormalized with 'mode' instead")]
		public bool normalize = true;

		public Vector2Composite.Mode mode;

		public enum Mode
		{
			Analog = 2,
			DigitalNormalized = 0,
			Digital
		}
	}
}
