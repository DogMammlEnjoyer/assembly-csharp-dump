using System;
using System.IO;
using System.Text;

namespace Fusion
{
	public class TextWriterLogStream : LogStream
	{
		public TextWriterLogStream(TextWriter writer, bool disposeWriter, string prefix = null)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this._writer = writer;
			this._disposeWriter = disposeWriter;
			this._prefix = prefix;
		}

		public override void Log(ILogSource source, string message)
		{
			this.Log(message);
		}

		public override void Log(string message)
		{
			try
			{
				if (!string.IsNullOrEmpty(this._prefix))
				{
					this._builder.Append(this._prefix);
					this._builder.Append(" ");
				}
				this._builder.Append(message);
				this._writer.WriteLine(this._builder.ToString());
			}
			finally
			{
				this._builder.Clear();
			}
		}

		public override void Log(ILogSource source, string message, Exception error)
		{
			this.Log(message, error);
		}

		public override void Log(string message, Exception error)
		{
			try
			{
				if (!string.IsNullOrEmpty(this._prefix))
				{
					this._builder.Append(this._prefix);
					this._builder.Append(" ");
				}
				if (!string.IsNullOrEmpty(message))
				{
					this._builder.Append(message);
					this._builder.Append(" ");
				}
				this._builder.Append(error.Message);
				this._writer.WriteLine(this._builder.ToString());
				this._writer.WriteLine(error.StackTrace);
			}
			finally
			{
				this._builder.Clear();
			}
		}

		public override void Log(Exception error)
		{
			this.Log(null, error);
		}

		public override void Dispose()
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

		private StringBuilder _builder = new StringBuilder();

		private TextWriter _writer;

		private bool _disposeWriter;

		private string _prefix;
	}
}
