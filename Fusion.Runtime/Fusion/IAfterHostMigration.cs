using System;

namespace Fusion
{
	public interface IAfterHostMigration : IPublicFacingInterface
	{
		void AfterHostMigration();
	}
}
