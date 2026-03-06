using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Composites
{
	[Preserve]
	public class ButtonFallbackComposite : FallbackComposite<float>
	{
		public override float ReadValue(ref InputBindingCompositeContext context)
		{
			InputControl inputControl;
			float result = context.ReadValue<float>(this.first, out inputControl);
			if (inputControl != null)
			{
				return result;
			}
			result = context.ReadValue<float>(this.second, out inputControl);
			if (inputControl != null)
			{
				return result;
			}
			return context.ReadValue<float>(this.third);
		}

		public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
		{
			return this.ReadValue(ref context);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		[Preserve]
		private static void Initialize()
		{
		}

		[Preserve]
		static ButtonFallbackComposite()
		{
			InputSystem.RegisterBindingComposite<ButtonFallbackComposite>(null);
		}

		[InputControl(layout = "Button")]
		public int first;

		[InputControl(layout = "Button")]
		public int second;

		[InputControl(layout = "Button")]
		public int third;
	}
}
