using System;
using System.Diagnostics;
using System.IO;

namespace WebSocketSharp
{
	public class Logger
	{
		public Logger() : this(LogLevel.Error, null, null)
		{
		}

		public Logger(LogLevel level) : this(level, null, null)
		{
		}

		public Logger(LogLevel level, string file, Action<LogData, string> output)
		{
			this._level = level;
			this._file = file;
			this._output = (output ?? new Action<LogData, string>(Logger.defaultOutput));
			this._sync = new object();
		}

		public string File
		{
			get
			{
				return this._file;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					this._file = value;
					this.Warn(string.Format("The current path to the log file has been changed to {0}.", this._file));
				}
			}
		}

		public LogLevel Level
		{
			get
			{
				return this._level;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					this._level = value;
					this.Warn(string.Format("The current logging level has been changed to {0}.", this._level));
				}
			}
		}

		public Action<LogData, string> Output
		{
			get
			{
				return this._output;
			}
			set
			{
				object sync = this._sync;
				lock (sync)
				{
					this._output = (value ?? new Action<LogData, string>(Logger.defaultOutput));
					this.Warn("The current output action has been changed.");
				}
			}
		}

		private static void defaultOutput(LogData data, string path)
		{
			string value = data.ToString();
			Console.WriteLine(value);
			bool flag = path != null && path.Length > 0;
			if (flag)
			{
				Logger.writeToFile(value, path);
			}
		}

		private void output(string message, LogLevel level)
		{
			object sync = this._sync;
			lock (sync)
			{
				bool flag = this._level > level;
				if (!flag)
				{
					try
					{
						LogData logData = new LogData(level, new StackFrame(2, true), message);
						this._output(logData, this._file);
					}
					catch (Exception ex)
					{
						LogData logData = new LogData(LogLevel.Fatal, new StackFrame(0, true), ex.Message);
						Console.WriteLine(logData.ToString());
					}
				}
			}
		}

		private static void writeToFile(string value, string path)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, true))
			{
				using (TextWriter textWriter = TextWriter.Synchronized(streamWriter))
				{
					textWriter.WriteLine(value);
				}
			}
		}

		public void Debug(string message)
		{
			bool flag = this._level > LogLevel.Debug;
			if (!flag)
			{
				this.output(message, LogLevel.Debug);
			}
		}

		public void Error(string message)
		{
			bool flag = this._level > LogLevel.Error;
			if (!flag)
			{
				this.output(message, LogLevel.Error);
			}
		}

		public void Fatal(string message)
		{
			this.output(message, LogLevel.Fatal);
		}

		public void Info(string message)
		{
			bool flag = this._level > LogLevel.Info;
			if (!flag)
			{
				this.output(message, LogLevel.Info);
			}
		}

		public void Trace(string message)
		{
			bool flag = this._level > LogLevel.Trace;
			if (!flag)
			{
				this.output(message, LogLevel.Trace);
			}
		}

		public void Warn(string message)
		{
			bool flag = this._level > LogLevel.Warn;
			if (!flag)
			{
				this.output(message, LogLevel.Warn);
			}
		}

		private volatile string _file;

		private volatile LogLevel _level;

		private Action<LogData, string> _output;

		private object _sync;
	}
}
