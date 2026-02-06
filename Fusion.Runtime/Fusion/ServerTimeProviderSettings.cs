using System;

namespace Fusion
{
	internal struct ServerTimeProviderSettings
	{
		public static ServerTimeProviderSettings Default()
		{
			TickRate.Resolved resolved = TickRate.Resolve(TickRate.Default);
			return new ServerTimeProviderSettings
			{
				SimDeltaTime = resolved.ClientTickDelta
			};
		}

		public double SimDeltaTime;
	}
}
