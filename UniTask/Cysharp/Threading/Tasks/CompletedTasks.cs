using System;

namespace Cysharp.Threading.Tasks
{
	internal static class CompletedTasks
	{
		public static readonly UniTask<AsyncUnit> AsyncUnit = UniTask.FromResult<AsyncUnit>(Cysharp.Threading.Tasks.AsyncUnit.Default);

		public static readonly UniTask<bool> True = UniTask.FromResult<bool>(true);

		public static readonly UniTask<bool> False = UniTask.FromResult<bool>(false);

		public static readonly UniTask<int> Zero = UniTask.FromResult<int>(0);

		public static readonly UniTask<int> MinusOne = UniTask.FromResult<int>(-1);

		public static readonly UniTask<int> One = UniTask.FromResult<int>(1);
	}
}
