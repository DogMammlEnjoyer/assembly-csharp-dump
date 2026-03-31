using System;

namespace Cysharp.Threading.Tasks
{
	public interface IPromise<T> : IResolvePromise<T>, IRejectPromise, ICancelPromise
	{
	}
}
