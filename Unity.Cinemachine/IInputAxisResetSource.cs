using System;

namespace Unity.Cinemachine
{
	public interface IInputAxisResetSource
	{
		void RegisterResetHandler(Action handler);

		void UnregisterResetHandler(Action handler);

		bool HasResetHandler { get; }
	}
}
