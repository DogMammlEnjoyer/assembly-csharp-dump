using System;

namespace Oculus.Interaction
{
	public interface IInteractable : IInteractableView
	{
		void Enable();

		void Disable();

		int MaxInteractors { get; set; }

		int MaxSelectingInteractors { get; set; }

		void RemoveInteractorByIdentifier(int id);
	}
}
