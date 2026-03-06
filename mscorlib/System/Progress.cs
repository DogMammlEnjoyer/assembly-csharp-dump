using System;
using System.Threading;

namespace System
{
	/// <summary>Provides an <see cref="T:System.IProgress`1" /> that invokes callbacks for each reported progress value.</summary>
	/// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
	public class Progress<T> : IProgress<T>
	{
		/// <summary>Initializes the <see cref="T:System.Progress`1" /> object.</summary>
		public Progress()
		{
			this._synchronizationContext = (SynchronizationContext.Current ?? ProgressStatics.DefaultContext);
			this._invokeHandlers = new SendOrPostCallback(this.InvokeHandlers);
		}

		/// <summary>Initializes the <see cref="T:System.Progress`1" /> object with the specified callback.</summary>
		/// <param name="handler">A handler to invoke for each reported progress value. This handler will be invoked in addition to any delegates registered with the <see cref="E:System.Progress`1.ProgressChanged" /> event. Depending on the <see cref="T:System.Threading.SynchronizationContext" /> instance captured by the <see cref="T:System.Progress`1" /> at construction, it is possible that this handler instance could be invoked concurrently with itself.</param>
		public Progress(Action<T> handler) : this()
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			this._handler = handler;
		}

		/// <summary>Raised for each reported progress value.</summary>
		public event EventHandler<T> ProgressChanged;

		/// <summary>Reports a progress change.</summary>
		/// <param name="value">The value of the updated progress.</param>
		protected virtual void OnReport(T value)
		{
			bool handler = this._handler != null;
			EventHandler<T> progressChanged = this.ProgressChanged;
			if (handler || progressChanged != null)
			{
				this._synchronizationContext.Post(this._invokeHandlers, value);
			}
		}

		void IProgress<!0>.Report(T value)
		{
			this.OnReport(value);
		}

		private void InvokeHandlers(object state)
		{
			T t = (T)((object)state);
			Action<T> handler = this._handler;
			EventHandler<T> progressChanged = this.ProgressChanged;
			if (handler != null)
			{
				handler(t);
			}
			if (progressChanged != null)
			{
				progressChanged(this, t);
			}
		}

		private readonly SynchronizationContext _synchronizationContext;

		private readonly Action<T> _handler;

		private readonly SendOrPostCallback _invokeHandlers;
	}
}
