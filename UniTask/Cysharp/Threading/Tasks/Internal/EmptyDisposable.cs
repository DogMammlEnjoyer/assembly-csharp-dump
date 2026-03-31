using System;

namespace Cysharp.Threading.Tasks.Internal
{
	internal class EmptyDisposable : IDisposable
	{
		private EmptyDisposable()
		{
		}

		public void Dispose()
		{
		}

		public static EmptyDisposable Instance = new EmptyDisposable();
	}
}
