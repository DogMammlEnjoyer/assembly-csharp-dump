using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Composites
{
	[Preserve]
	public class IntegerFallbackComposite : FallbackComposite<int>
	{
		public override int ReadValue(ref InputBindingCompositeContext context)
		{
			InputControl inputControl;
			int result = context.ReadValue<int>(this.first, out inputControl);
			if (inputControl != null)
			{
				return result;
			}
			result = context.ReadValue<int>(this.second, out inputControl);
			if (inputControl != null)
			{
				return result;
			}
			return context.ReadValue<int>(this.third);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		[Preserve]
		private static void Initialize()
		{
		}

		[Preserve]
		static IntegerFallbackComposite()
		{
			InputSystem.RegisterBindingComposite<IntegerFallbackComposite>(null);
		}

		[InputControl(layout = "Integer")]
		public int first;

		[InputControl(layout = "Integer")]
		public int second;

		[InputControl(layout = "Integer")]
		public int third;
	}
}
