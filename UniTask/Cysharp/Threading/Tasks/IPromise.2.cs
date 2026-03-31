using System;

namespace Cysharp.Threading.Tasks
{
	public interface IPromise : IResolvePromise, IRejectPromise, ICancelPromise
	{
	}
}
