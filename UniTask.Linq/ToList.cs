using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks.CompilerServices;

namespace Cysharp.Threading.Tasks.Linq
{
	internal static class ToList
	{
		internal static UniTask<List<TSource>> ToListAsync<TSource>(IUniTaskAsyncEnumerable<TSource> source, CancellationToken cancellationToken)
		{
			ToList.<ToListAsync>d__0<TSource> <ToListAsync>d__;
			<ToListAsync>d__.<>t__builder = AsyncUniTaskMethodBuilder<List<TSource>>.Create();
			<ToListAsync>d__.source = source;
			<ToListAsync>d__.cancellationToken = cancellationToken;
			<ToListAsync>d__.<>1__state = -1;
			<ToListAsync>d__.<>t__builder.Start<ToList.<ToListAsync>d__0<TSource>>(ref <ToListAsync>d__);
			return <ToListAsync>d__.<>t__builder.Task;
		}
	}
}
