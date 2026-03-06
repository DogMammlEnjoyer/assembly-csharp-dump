using System;

namespace UnityEngine.InputSystem
{
	public interface IInputInteraction<TValue> : IInputInteraction where TValue : struct
	{
	}
}
