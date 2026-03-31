using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[TupleElementNames(new string[]
	{
		"data",
		"channels"
	})]
	[DisallowMultipleComponent]
	public sealed class AsyncAudioFilterReadTrigger : AsyncTriggerBase<ValueTuple<float[], int>>
	{
		private void OnAudioFilterRead(float[] data, int channels)
		{
			base.RaiseEvent(new ValueTuple<float[], int>(data, channels));
		}

		public IAsyncOnAudioFilterReadHandler GetOnAudioFilterReadAsyncHandler()
		{
			return new AsyncTriggerHandler<ValueTuple<float[], int>>(this, false);
		}

		public IAsyncOnAudioFilterReadHandler GetOnAudioFilterReadAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<ValueTuple<float[], int>>(this, cancellationToken, false);
		}

		[return: TupleElementNames(new string[]
		{
			"data",
			"channels"
		})]
		public UniTask<ValueTuple<float[], int>> OnAudioFilterReadAsync()
		{
			return ((IAsyncOnAudioFilterReadHandler)new AsyncTriggerHandler<ValueTuple<float[], int>>(this, true)).OnAudioFilterReadAsync();
		}

		[return: TupleElementNames(new string[]
		{
			"data",
			"channels"
		})]
		public UniTask<ValueTuple<float[], int>> OnAudioFilterReadAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnAudioFilterReadHandler)new AsyncTriggerHandler<ValueTuple<float[], int>>(this, cancellationToken, true)).OnAudioFilterReadAsync();
		}
	}
}
