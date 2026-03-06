using System;

namespace System
{
	/// <summary>Provides data for the event that is raised when there is an exception that is not handled in any application domain.</summary>
	[Serializable]
	public class UnhandledExceptionEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.UnhandledExceptionEventArgs" /> class with the exception object and a common language runtime termination flag.</summary>
		/// <param name="exception">The exception that is not handled.</param>
		/// <param name="isTerminating">
		///   <see langword="true" /> if the runtime is terminating; otherwise, <see langword="false" />.</param>
		public UnhandledExceptionEventArgs(object exception, bool isTerminating)
		{
			this._exception = exception;
			this._isTerminating = isTerminating;
		}

		/// <summary>Gets the unhandled exception object.</summary>
		/// <returns>The unhandled exception object.</returns>
		public object ExceptionObject
		{
			get
			{
				return this._exception;
			}
		}

		/// <summary>Indicates whether the common language runtime is terminating.</summary>
		/// <returns>
		///   <see langword="true" /> if the runtime is terminating; otherwise, <see langword="false" />.</returns>
		public bool IsTerminating
		{
			get
			{
				return this._isTerminating;
			}
		}

		private object _exception;

		private bool _isTerminating;
	}
}
