using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Composites
{
	[Preserve]
	public class Vector3FallbackComposite : FallbackComposite<Vector3>
	{
		public override Vector3 ReadValue(ref InputBindingCompositeContext context)
		{
			InputControl inputControl;
			Vector3 result = context.ReadValue<Vector3, Vector3MagnitudeComparer>(this.first, out inputControl, default(Vector3MagnitudeComparer));
			if (inputControl != null)
			{
				return result;
			}
			result = context.ReadValue<Vector3, Vector3MagnitudeComparer>(this.second, out inputControl, default(Vector3MagnitudeComparer));
			if (inputControl != null)
			{
				return result;
			}
			return context.ReadValue<Vector3, Vector3MagnitudeComparer>(this.third, default(Vector3MagnitudeComparer));
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		[Preserve]
		private static void Initialize()
		{
		}

		[Preserve]
		static Vector3FallbackComposite()
		{
			InputSystem.RegisterBindingComposite<Vector3FallbackComposite>(null);
		}

		[InputControl(layout = "Vector3")]
		public int first;

		[InputControl(layout = "Vector3")]
		public int second;

		[InputControl(layout = "Vector3")]
		public int third;
	}
}
