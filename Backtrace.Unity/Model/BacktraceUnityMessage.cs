using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	internal class BacktraceUnityMessage
	{
		public BacktraceUnityMessage(BacktraceReport report)
		{
			if (report == null)
			{
				throw new ArgumentException("report");
			}
			this.Message = report.Message;
			if (report.ExceptionTypeReport)
			{
				this.Type = LogType.Exception;
				this.StackTrace = this.GetFormattedStackTrace(report.Exception.StackTrace);
				this._formattedMessage = this.GetFormattedMessage(true);
				return;
			}
			this.StackTrace = string.Empty;
			this.Type = LogType.Warning;
			this._formattedMessage = this.GetFormattedMessage(true);
		}

		public BacktraceUnityMessage(string message, string stacktrace, LogType type)
		{
			this.Message = message;
			this.StackTrace = this.GetFormattedStackTrace(stacktrace);
			this.Type = type;
			this._formattedMessage = this.GetFormattedMessage(false);
		}

		private string GetFormattedMessage(bool backtraceFrame)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[{0}] {1}<{2}>: {3}", new object[]
			{
				DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture),
				backtraceFrame ? "(Backtrace)" : string.Empty,
				Enum.GetName(typeof(LogType), this.Type),
				this.Message
			});
			if (this.IsUnhandledException())
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(string.IsNullOrEmpty(this.StackTrace) ? "No stack trace available" : this.StackTrace);
			}
			return stringBuilder.ToString();
		}

		private string GetFormattedStackTrace(string stacktrace)
		{
			if (string.IsNullOrEmpty(stacktrace) || !stacktrace.EndsWith("\n"))
			{
				return stacktrace;
			}
			return stacktrace.Remove(stacktrace.LastIndexOf("\n"));
		}

		public bool IsUnhandledException()
		{
			return (this.Type == LogType.Exception || this.Type == LogType.Error) && !string.IsNullOrEmpty(this.Message);
		}

		public override string ToString()
		{
			return this._formattedMessage;
		}

		private readonly string _formattedMessage;

		public readonly string Message;

		public readonly string StackTrace;

		public readonly LogType Type;
	}
}
