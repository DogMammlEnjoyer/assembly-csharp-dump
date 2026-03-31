using System;

namespace Cysharp.Threading.Tasks
{
	public interface IAsyncReactiveProperty<T> : IReadOnlyAsyncReactiveProperty<T>, IUniTaskAsyncEnumerable<T>
	{
		T Value { get; set; }
	}
}
