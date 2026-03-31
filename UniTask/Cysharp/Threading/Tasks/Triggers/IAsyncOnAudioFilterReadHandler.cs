using System;
using System.Runtime.CompilerServices;

namespace Cysharp.Threading.Tasks.Triggers
{
	public interface IAsyncOnAudioFilterReadHandler
	{
		[return: TupleElementNames(new string[]
		{
			"data",
			"channels"
		})]
		UniTask<ValueTuple<float[], int>> OnAudioFilterReadAsync();
	}
}
