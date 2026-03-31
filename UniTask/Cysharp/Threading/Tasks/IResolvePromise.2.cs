using System;

namespace Cysharp.Threading.Tasks
{
	public interface IResolvePromise<T>
	{
		bool TrySetResult(T value);
	}
}
