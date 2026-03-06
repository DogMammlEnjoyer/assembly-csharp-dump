using System;

namespace Oculus.Interaction
{
	public struct InteractableStateChangeArgs
	{
		public readonly InteractableState PreviousState { get; }

		public readonly InteractableState NewState { get; }

		public InteractableStateChangeArgs(InteractableState previousState, InteractableState newState)
		{
			this.PreviousState = previousState;
			this.NewState = newState;
		}
	}
}
