using System;

namespace Fusion
{
	public interface IBeforeTick : IPublicFacingInterface
	{
		void BeforeTick();
	}
}
