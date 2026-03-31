using System;
using System.Threading;

namespace Cysharp.Threading.Tasks.Linq
{
	internal sealed class CancellationTokenDisposable : IDisposable
	{
		public CancellationToken Token
		{
			get
			{
				return this.cts.Token;
			}
		}

		public void Dispose()
		{
			if (!this.cts.IsCancellationRequested)
			{
				this.cts.Cancel();
			}
		}

		private readonly CancellationTokenSource cts = new CancellationTokenSource();
	}
}
