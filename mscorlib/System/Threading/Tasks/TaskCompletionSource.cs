using System;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
	/// <summary>Represents the producer side of a <see cref="T:System.Threading.Tasks.Task`1" /> unbound to a delegate, providing access to the consumer side through the <see cref="P:System.Threading.Tasks.TaskCompletionSource`1.Task" /> property.</summary>
	/// <typeparam name="TResult">The type of the result value assocatied with this <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" />.</typeparam>
	public class TaskCompletionSource<TResult>
	{
		/// <summary>Creates a <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" />.</summary>
		public TaskCompletionSource()
		{
			this._task = new Task<TResult>();
		}

		/// <summary>Creates a <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" /> with the specified options.</summary>
		/// <param name="creationOptions">The options to use when creating the underlying <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="creationOptions" /> represent options invalid for use with a <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" />.</exception>
		public TaskCompletionSource(TaskCreationOptions creationOptions) : this(null, creationOptions)
		{
		}

		/// <summary>Creates a <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" /> with the specified state.</summary>
		/// <param name="state">The state to use as the underlying <see cref="T:System.Threading.Tasks.Task`1" />'s AsyncState.</param>
		public TaskCompletionSource(object state) : this(state, TaskCreationOptions.None)
		{
		}

		/// <summary>Creates a <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" /> with the specified state and options.</summary>
		/// <param name="state">The state to use as the underlying <see cref="T:System.Threading.Tasks.Task`1" />'s AsyncState.</param>
		/// <param name="creationOptions">The options to use when creating the underlying <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="creationOptions" /> represent options invalid for use with a <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" />.</exception>
		public TaskCompletionSource(object state, TaskCreationOptions creationOptions)
		{
			this._task = new Task<TResult>(state, creationOptions);
		}

		/// <summary>Gets the <see cref="T:System.Threading.Tasks.Task`1" /> created by this <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" />.</summary>
		/// <returns>Returns the <see cref="T:System.Threading.Tasks.Task`1" /> created by this <see cref="T:System.Threading.Tasks.TaskCompletionSource`1" />.</returns>
		public Task<TResult> Task
		{
			get
			{
				return this._task;
			}
		}

		private void SpinUntilCompleted()
		{
			SpinWait spinWait = default(SpinWait);
			while (!this._task.IsCompleted)
			{
				spinWait.SpinOnce();
			}
		}

		/// <summary>Attempts to transition the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and binds it to a specified exception.</summary>
		/// <param name="exception">The exception to bind to this <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <returns>True if the operation was successful; otherwise, false.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="P:System.Threading.Tasks.TaskCompletionSource`1.Task" /> was disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> argument is null.</exception>
		public bool TrySetException(Exception exception)
		{
			if (exception == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exception);
			}
			bool flag = this._task.TrySetException(exception);
			if (!flag && !this._task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		/// <summary>Attempts to transition the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and binds a collection of exception objects to it.</summary>
		/// <param name="exceptions">The collection of exceptions to bind to this <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <returns>True if the operation was successful; otherwise, false.</returns>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="P:System.Threading.Tasks.TaskCompletionSource`1.Task" /> was disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="exceptions" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentException">There are one or more null elements in <paramref name="exceptions" />.  
		///  -or-  
		///  The <paramref name="exceptions" /> collection is empty.</exception>
		public bool TrySetException(IEnumerable<Exception> exceptions)
		{
			if (exceptions == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exceptions);
			}
			List<Exception> list = new List<Exception>();
			foreach (Exception ex in exceptions)
			{
				if (ex == null)
				{
					ThrowHelper.ThrowArgumentException(ExceptionResource.TaskCompletionSourceT_TrySetException_NullException, ExceptionArgument.exceptions);
				}
				list.Add(ex);
			}
			if (list.Count == 0)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.TaskCompletionSourceT_TrySetException_NoExceptions, ExceptionArgument.exceptions);
			}
			bool flag = this._task.TrySetException(list);
			if (!flag && !this._task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		/// <summary>Transitions the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and binds it to a specified exception.</summary>
		/// <param name="exception">The exception to bind to this <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="P:System.Threading.Tasks.TaskCompletionSource`1.Task" /> was disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> argument is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying <see cref="T:System.Threading.Tasks.Task`1" /> is already in one of the three final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" />, <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" />, or <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" />.</exception>
		public void SetException(Exception exception)
		{
			if (exception == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.exception);
			}
			if (!this.TrySetException(exception))
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
			}
		}

		/// <summary>Transitions the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state and binds a collection of exception objects to it.</summary>
		/// <param name="exceptions">The collection of exceptions to bind to this <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="P:System.Threading.Tasks.TaskCompletionSource`1.Task" /> was disposed.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="exceptions" /> argument is null.</exception>
		/// <exception cref="T:System.ArgumentException">There are one or more null elements in <paramref name="exceptions" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying <see cref="T:System.Threading.Tasks.Task`1" /> is already in one of the three final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" />, <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" />, or <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" />.</exception>
		public void SetException(IEnumerable<Exception> exceptions)
		{
			if (!this.TrySetException(exceptions))
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
			}
		}

		/// <summary>Attempts to transition the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state.</summary>
		/// <param name="result">The result value to bind to this <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <returns>True if the operation was successful; otherwise, false.</returns>
		public bool TrySetResult(TResult result)
		{
			bool flag = this._task.TrySetResult(result);
			if (!flag)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		/// <summary>Transitions the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state.</summary>
		/// <param name="result">The result value to bind to this <see cref="T:System.Threading.Tasks.Task`1" />.</param>
		/// <exception cref="T:System.ObjectDisposedException">The <see cref="P:System.Threading.Tasks.TaskCompletionSource`1.Task" /> was disposed.</exception>
		/// <exception cref="T:System.InvalidOperationException">The underlying <see cref="T:System.Threading.Tasks.Task`1" /> is already in one of the three final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" />, <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" />, or <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" />.</exception>
		public void SetResult(TResult result)
		{
			if (!this.TrySetResult(result))
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
			}
		}

		/// <summary>Attempts to transition the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" /> state.</summary>
		/// <returns>True if the operation was successful; false if the operation was unsuccessful or the object has already been disposed.</returns>
		public bool TrySetCanceled()
		{
			return this.TrySetCanceled(default(CancellationToken));
		}

		/// <summary>Attempts to transition the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" /> state and enables a cancellation token to be stored in the canceled task.</summary>
		/// <param name="cancellationToken">A cancellation token.</param>
		/// <returns>
		///   <see langword="true" /> if the operation is successful; otherwise, <see langword="false" />.</returns>
		public bool TrySetCanceled(CancellationToken cancellationToken)
		{
			bool flag = this._task.TrySetCanceled(cancellationToken);
			if (!flag && !this._task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		/// <summary>Transitions the underlying <see cref="T:System.Threading.Tasks.Task`1" /> into the <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" /> state.</summary>
		/// <exception cref="T:System.InvalidOperationException">The underlying <see cref="T:System.Threading.Tasks.Task`1" /> is already in one of the three final states: <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" />, <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" />, or <see cref="F:System.Threading.Tasks.TaskStatus.Canceled" />, or if the underlying <see cref="T:System.Threading.Tasks.Task`1" /> has already been disposed.</exception>
		public void SetCanceled()
		{
			if (!this.TrySetCanceled())
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.TaskT_TransitionToFinal_AlreadyCompleted);
			}
		}

		private readonly Task<TResult> _task;
	}
}
