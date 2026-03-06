using System;
using System.Collections;
using System.Runtime.ExceptionServices;

namespace Fusion
{
	public sealed class FusionCoroutine : ICoroutine, IAsyncOperation, IEnumerator, IDisposable
	{
		public FusionCoroutine(IEnumerator inner)
		{
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			this._inner = inner;
		}

		public event Action<IAsyncOperation> Completed
		{
			add
			{
				this._completed = (Action<IAsyncOperation>)Delegate.Combine(this._completed, value);
				if (this.IsDone)
				{
					value(this);
				}
			}
			remove
			{
				this._completed = (Action<IAsyncOperation>)Delegate.Remove(this._completed, value);
			}
		}

		public bool IsDone { get; private set; }

		public ExceptionDispatchInfo Error { get; private set; }

		bool IEnumerator.MoveNext()
		{
			bool result;
			try
			{
				if (this._inner.MoveNext())
				{
					result = true;
				}
				else
				{
					this.IsDone = true;
					Action<IAsyncOperation> completed = this._completed;
					if (completed != null)
					{
						completed(this);
					}
					result = false;
				}
			}
			catch (Exception source)
			{
				this.IsDone = true;
				this.Error = ExceptionDispatchInfo.Capture(source);
				Action<IAsyncOperation> completed2 = this._completed;
				if (completed2 != null)
				{
					completed2(this);
				}
				result = false;
			}
			return result;
		}

		void IEnumerator.Reset()
		{
			this._inner.Reset();
			this.IsDone = false;
			this.Error = null;
		}

		object IEnumerator.Current
		{
			get
			{
				return this._inner.Current;
			}
		}

		public void Dispose()
		{
			IDisposable disposable = this._inner as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		private readonly IEnumerator _inner;

		private Action<IAsyncOperation> _completed;

		private float _progress;

		private Action _activateAsync;
	}
}
