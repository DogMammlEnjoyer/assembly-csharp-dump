using System;

namespace Fusion
{
	public interface INetworkAssetSource<T>
	{
		void Acquire(bool synchronous);

		void Release();

		T WaitForResult();

		bool IsCompleted { get; }

		string Description { get; }
	}
}
