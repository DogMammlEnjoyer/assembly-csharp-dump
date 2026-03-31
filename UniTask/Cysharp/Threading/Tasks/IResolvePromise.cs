using System;

namespace Cysharp.Threading.Tasks
{
	public interface IResolvePromise
	{
		bool TrySetResult();
	}
}
