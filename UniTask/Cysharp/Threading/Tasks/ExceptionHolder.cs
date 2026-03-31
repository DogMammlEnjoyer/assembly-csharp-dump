using System;
using System.Runtime.ExceptionServices;

namespace Cysharp.Threading.Tasks
{
	internal class ExceptionHolder
	{
		public ExceptionHolder(ExceptionDispatchInfo exception)
		{
			this.exception = exception;
		}

		public ExceptionDispatchInfo GetException()
		{
			if (!this.calledGet)
			{
				this.calledGet = true;
				GC.SuppressFinalize(this);
			}
			return this.exception;
		}

		~ExceptionHolder()
		{
			if (!this.calledGet)
			{
				UniTaskScheduler.PublishUnobservedTaskException(this.exception.SourceException);
			}
		}

		private ExceptionDispatchInfo exception;

		private bool calledGet;
	}
}
