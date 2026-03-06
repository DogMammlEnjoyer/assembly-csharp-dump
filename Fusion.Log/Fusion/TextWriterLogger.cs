using System;
using System.IO;
using System.Text;

namespace Fusion
{
	[Obsolete]
	public class TextWriterLogger : ILogger, IDisposable
	{
		public TextWriterLogger(TextWriter writer, bool disposeWriter)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this._writer = writer;
			this._disposeWriter = disposeWriter;
		}

		public virtual void Dispose()
		{
			if (!this._disposeWriter)
			{
				return;
			}
			if (this._writer == null)
			{
				return;
			}
			TextWriter writer = this._writer;
			this._writer = null;
			writer.Dispose();
		}

		public virtual void Log(LogType logType, object message, in LogContext logContext)
		{
			try
			{
				if (logType == LogType.Debug)
				{
					this._builder.Append("[DEBUG] ");
				}
				else if (logType == LogType.Trace)
				{
					this._builder.Append("[TRACE] ");
				}
				if (!string.IsNullOrEmpty(logContext.Prefix))
				{
					this._builder.Append(logContext.Prefix);
					this._builder.Append(": ");
				}
				this._builder.Append(message);
				this._writer.WriteLine(this._builder.ToString());
			}
			finally
			{
				this._builder.Clear();
			}
		}

		public virtual void LogException(Exception ex, in LogContext logContext)
		{
			try
			{
				this._builder.Append(logContext.Prefix);
				this._builder.Append(ex.Message);
				this._writer.WriteLine(this._builder.ToString());
				this._writer.WriteLine(ex.StackTrace);
			}
			finally
			{
				this._builder.Clear();
			}
		}

		private StringBuilder _builder = new StringBuilder();

		private TextWriter _writer;

		private bool _disposeWriter;
	}
}
