using System;

namespace Cysharp.Threading.Tasks.CompilerServices
{
	internal interface IStateMachineRunnerPromise : IUniTaskSource
	{
		Action MoveNext { get; }

		UniTask Task { get; }

		void SetResult();

		void SetException(Exception exception);
	}
}
