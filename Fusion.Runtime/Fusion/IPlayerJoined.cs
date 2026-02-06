using System;

namespace Fusion
{
	public interface IPlayerJoined : IPublicFacingInterface
	{
		void PlayerJoined(PlayerRef player);
	}
}
