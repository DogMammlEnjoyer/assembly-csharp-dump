using System;

namespace Cysharp.Threading.Tasks
{
	internal static class UniTaskCompletionSourceCoreShared
	{
		private static void CompletionSentinel(object _)
		{
			throw new InvalidOperationException("The sentinel delegate should never be invoked.");
		}

		internal static readonly Action<object> s_sentinel = new Action<object>(UniTaskCompletionSourceCoreShared.CompletionSentinel);
	}
}
