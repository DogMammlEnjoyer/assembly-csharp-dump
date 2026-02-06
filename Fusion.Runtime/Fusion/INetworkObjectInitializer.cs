using System;

namespace Fusion
{
	public interface INetworkObjectInitializer
	{
		void InitializeNetworkState(NetworkObject networkObject);
	}
}
