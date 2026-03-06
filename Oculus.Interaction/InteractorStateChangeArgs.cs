using System;

namespace Oculus.Interaction
{
	public struct InteractorStateChangeArgs
	{
		public readonly InteractorState PreviousState { get; }

		public readonly InteractorState NewState { get; }

		public InteractorStateChangeArgs(InteractorState previousState, InteractorState newState)
		{
			this.PreviousState = previousState;
			this.NewState = newState;
		}
	}
}
