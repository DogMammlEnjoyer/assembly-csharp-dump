using System;

namespace Cysharp.Threading.Tasks.Internal
{
	internal sealed class SingleAssignmentDisposable : IDisposable
	{
		public bool IsDisposed
		{
			get
			{
				object obj = this.gate;
				bool result;
				lock (obj)
				{
					result = this.disposed;
				}
				return result;
			}
		}

		public IDisposable Disposable
		{
			get
			{
				return this.current;
			}
			set
			{
				IDisposable disposable = null;
				object obj = this.gate;
				bool flag2;
				lock (obj)
				{
					flag2 = this.disposed;
					disposable = this.current;
					if (!flag2)
					{
						if (value == null)
						{
							return;
						}
						this.current = value;
					}
				}
				if (flag2 && value != null)
				{
					value.Dispose();
					return;
				}
				if (disposable != null)
				{
					throw new InvalidOperationException("Disposable is already set");
				}
			}
		}

		public void Dispose()
		{
			IDisposable disposable = null;
			object obj = this.gate;
			lock (obj)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					disposable = this.current;
					this.current = null;
				}
			}
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		private readonly object gate = new object();

		private IDisposable current;

		private bool disposed;
	}
}
