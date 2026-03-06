using System;

namespace System.IO
{
	/// <summary>Provides data for the <see cref="E:System.IO.FileSystemWatcher.Error" /> event.</summary>
	public class ErrorEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.ErrorEventArgs" /> class.</summary>
		/// <param name="exception">An <see cref="T:System.Exception" /> that represents the error that occurred.</param>
		public ErrorEventArgs(Exception exception)
		{
			this.exception = exception;
		}

		/// <summary>Gets the <see cref="T:System.Exception" /> that represents the error that occurred.</summary>
		/// <returns>An <see cref="T:System.Exception" /> that represents the error that occurred.</returns>
		public virtual Exception GetException()
		{
			return this.exception;
		}

		private Exception exception;
	}
}
