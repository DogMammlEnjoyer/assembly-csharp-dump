using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
	public interface IAsyncOnRenderImageHandler
	{
		[return: TupleElementNames(new string[]
		{
			"source",
			"destination"
		})]
		UniTask<ValueTuple<RenderTexture, RenderTexture>> OnRenderImageAsync();
	}
}
