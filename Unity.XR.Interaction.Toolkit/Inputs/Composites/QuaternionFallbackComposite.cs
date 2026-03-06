using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Composites
{
	[Preserve]
	public class QuaternionFallbackComposite : FallbackComposite<Quaternion>
	{
		public override Quaternion ReadValue(ref InputBindingCompositeContext context)
		{
			InputControl inputControl;
			Quaternion result = context.ReadValue<Quaternion, FallbackComposite<Quaternion>.QuaternionCompositeComparer>(this.first, out inputControl, default(FallbackComposite<Quaternion>.QuaternionCompositeComparer));
			if (inputControl != null)
			{
				return result;
			}
			result = context.ReadValue<Quaternion, FallbackComposite<Quaternion>.QuaternionCompositeComparer>(this.second, out inputControl, default(FallbackComposite<Quaternion>.QuaternionCompositeComparer));
			if (inputControl != null)
			{
				return result;
			}
			return context.ReadValue<Quaternion, FallbackComposite<Quaternion>.QuaternionCompositeComparer>(this.third, default(FallbackComposite<Quaternion>.QuaternionCompositeComparer));
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		[Preserve]
		private static void Initialize()
		{
		}

		[Preserve]
		static QuaternionFallbackComposite()
		{
			InputSystem.RegisterBindingComposite<QuaternionFallbackComposite>(null);
		}

		[InputControl(layout = "Quaternion")]
		public int first;

		[InputControl(layout = "Quaternion")]
		public int second;

		[InputControl(layout = "Quaternion")]
		public int third;
	}
}
