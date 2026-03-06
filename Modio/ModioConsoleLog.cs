using System;

namespace Modio
{
	public class ModioConsoleLog : IModioLogHandler
	{
		public ModioConsoleLog() : this("[mod.io] ")
		{
		}

		public ModioConsoleLog(string logPrefix)
		{
			this._logPrefix = logPrefix;
		}

		public void LogHandler(LogLevel logLevel, object message)
		{
			string text;
			if (logLevel != LogLevel.Error)
			{
				if (logLevel != LogLevel.Warning)
				{
					text = string.Empty;
				}
				else
				{
					text = "[WARNING] ";
				}
			}
			else
			{
				text = "[ERROR] ";
			}
			string text2 = text;
			Console.WriteLine(string.Format("{0}{1}{2}{3}", new object[]
			{
				this._logPrefix,
				text2,
				text2,
				message
			}));
		}

		private readonly string _logPrefix = "[mod.io] ";
	}
}
