using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	[TupleElementNames(new string[]
	{
		"source",
		"destination"
	})]
	[DisallowMultipleComponent]
	public sealed class AsyncRenderImageTrigger : AsyncTriggerBase<ValueTuple<RenderTexture, RenderTexture>>
	{
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			base.RaiseEvent(new ValueTuple<RenderTexture, RenderTexture>(source, destination));
		}

		public IAsyncOnRenderImageHandler GetOnRenderImageAsyncHandler()
		{
			return new AsyncTriggerHandler<ValueTuple<RenderTexture, RenderTexture>>(this, false);
		}

		public IAsyncOnRenderImageHandler GetOnRenderImageAsyncHandler(CancellationToken cancellationToken)
		{
			return new AsyncTriggerHandler<ValueTuple<RenderTexture, RenderTexture>>(this, cancellationToken, false);
		}

		[return: TupleElementNames(new string[]
		{
			"source",
			"destination"
		})]
		public UniTask<ValueTuple<RenderTexture, RenderTexture>> OnRenderImageAsync()
		{
			return ((IAsyncOnRenderImageHandler)new AsyncTriggerHandler<ValueTuple<RenderTexture, RenderTexture>>(this, true)).OnRenderImageAsync();
		}

		[return: TupleElementNames(new string[]
		{
			"source",
			"destination"
		})]
		public UniTask<ValueTuple<RenderTexture, RenderTexture>> OnRenderImageAsync(CancellationToken cancellationToken)
		{
			return ((IAsyncOnRenderImageHandler)new AsyncTriggerHandler<ValueTuple<RenderTexture, RenderTexture>>(this, cancellationToken, true)).OnRenderImageAsync();
		}
	}
}
