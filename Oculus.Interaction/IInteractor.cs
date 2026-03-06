using System;

namespace Oculus.Interaction
{
	public interface IInteractor : IInteractorView, IUpdateDriver
	{
		void Preprocess();

		void Process();

		void Postprocess();

		void ProcessCandidate();

		void Enable();

		void Disable();

		void Hover();

		void Unhover();

		void Select();

		void Unselect();

		bool ShouldHover { get; }

		bool ShouldUnhover { get; }

		bool ShouldSelect { get; }

		bool ShouldUnselect { get; }
	}
}
