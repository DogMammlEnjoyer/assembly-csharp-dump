using System;

namespace Fusion
{
	public interface IDespawned : IPublicFacingInterface
	{
		void Despawned(NetworkRunner runner, bool hasState);
	}
}
