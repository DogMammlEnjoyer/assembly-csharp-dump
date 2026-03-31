using System;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	internal interface IStateMachineRunnerPromise<T> : IUniTaskSource<!0>, IUniTaskSource
	{
		Action MoveNext { get; }

		UniTask<T> Task { get; }

		void SetResult(T result);

		void SetException(Exception exception);
	}
}
