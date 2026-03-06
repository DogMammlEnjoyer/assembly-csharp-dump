using System;

namespace UnityEngine.InputSystem
{
	public interface IInputInteraction
	{
		void Process(ref InputInteractionContext context);

		void Reset();
	}
}
