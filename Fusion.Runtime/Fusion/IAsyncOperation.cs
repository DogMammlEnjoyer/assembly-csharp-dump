using System;
using System.Runtime.ExceptionServices;

namespace Fusion
{
	public interface IAsyncOperation
	{
		bool IsDone { get; }

		event Action<IAsyncOperation> Completed;

		ExceptionDispatchInfo Error { get; }
	}
}
