using System;
using System.Collections.Generic;
using Backtrace.Unity.Common;
using Backtrace.Unity.Extensions;

namespace Backtrace.Unity.Model
{
	public class BacktraceReport
	{
		public string Fingerprint { get; set; }

		public string Factor { get; set; }

		public Dictionary<string, string> Attributes { get; private set; }

		public string Message { get; private set; }

		public Exception Exception { get; private set; }

		public List<string> AttachmentPaths { get; set; }

		public List<BacktraceStackFrame> DiagnosticStack { get; set; }

		public string Symbolication { get; set; }

		public BacktraceReport(string message, Dictionary<string, string> attributes = null, List<string> attachmentPaths = null) : this(null, attributes, attachmentPaths)
		{
			this.Message = message;
			this.SetStacktraceInformation();
			this.SetDefaultAttributes();
		}

		public BacktraceReport(Exception exception, Dictionary<string, string> attributes = null, List<string> attachmentPaths = null)
		{
			this.Attributes = (attributes ?? new Dictionary<string, string>());
			this.AttachmentPaths = (attachmentPaths ?? new List<string>());
			this.Exception = exception;
			this.ExceptionTypeReport = (exception != null);
			if (this.ExceptionTypeReport)
			{
				this.Message = exception.Message;
				this.SetClassifierInfo();
				this.SetStacktraceInformation();
			}
			this.SetDefaultAttributes();
		}

		public void UseSymbolication(string symbolication)
		{
			this.Symbolication = symbolication;
		}

		private void SetDefaultAttributes()
		{
			this.Attributes["error.message"] = this.Message;
			if (!this.Attributes.ContainsKey("error.type"))
			{
				this.Attributes["error.type"] = "Message";
			}
		}

		internal void AssignSourceCodeToReport(string text)
		{
			if (this.DiagnosticStack == null || this.DiagnosticStack.Count == 0)
			{
				return;
			}
			this.SourceCode = new BacktraceSourceCode
			{
				Text = text
			};
			foreach (BacktraceStackFrame backtraceStackFrame in this.DiagnosticStack)
			{
				backtraceStackFrame.SourceCode = BacktraceSourceCode.SOURCE_CODE_PROPERTY;
			}
		}

		private void SetClassifierInfo()
		{
			if (!this.ExceptionTypeReport)
			{
				this.Classifier = string.Empty;
				this.Attributes["error.type"] = "Message";
			}
			if (!(this.Exception is BacktraceUnhandledException))
			{
				this.Attributes["error.type"] = "Exception";
				this.Classifier = this.Exception.GetType().Name;
				return;
			}
			this.Classifier = (this.Exception as BacktraceUnhandledException).Classifier;
			string classifier = this.Classifier;
			if (classifier == "ANRException")
			{
				this.Attributes["error.type"] = "Hang";
				return;
			}
			if (!(classifier == "OOMException"))
			{
				this.Attributes["error.type"] = "Unhandled exception";
				return;
			}
			this.Attributes["error.type"] = "OOMException";
		}

		internal void SetReportFingerprint(bool generateFingerprint)
		{
			if (generateFingerprint && ((this.Exception != null && string.IsNullOrEmpty(this.Exception.StackTrace)) || this.DiagnosticStack == null || this.DiagnosticStack.Count == 0))
			{
				string value = string.IsNullOrEmpty(this.Message) ? "0000000000000000000000000000000000000000000000000000000000000000" : this.Message.OnlyLetters().GetSha();
				this.Attributes["_mod_fingerprint"] = value;
			}
			if (!string.IsNullOrEmpty(this.Factor))
			{
				this.Attributes["_mod_factor"] = this.Factor;
			}
			if (!string.IsNullOrEmpty(this.Fingerprint))
			{
				this.Attributes["_mod_fingerprint"] = this.Fingerprint;
			}
		}

		internal BacktraceData ToBacktraceData(Dictionary<string, string> clientAttributes, int gameObjectDepth)
		{
			return new BacktraceData(this, clientAttributes, gameObjectDepth);
		}

		internal void SetStacktraceInformation()
		{
			BacktraceStackTrace backtraceStackTrace = new BacktraceStackTrace(this.Exception);
			this.DiagnosticStack = backtraceStackTrace.StackFrames;
		}

		internal BacktraceReport CreateInnerReport()
		{
			if (!this.ExceptionTypeReport || this.Exception.InnerException == null)
			{
				return null;
			}
			BacktraceReport backtraceReport = (BacktraceReport)base.MemberwiseClone();
			backtraceReport.Exception = this.Exception.InnerException;
			backtraceReport.SetStacktraceInformation();
			backtraceReport.Classifier = backtraceReport.Exception.GetType().Name;
			return backtraceReport;
		}

		private const string ErrorTypeAttributeName = "error.type";

		public readonly Guid Uuid = Guid.NewGuid();

		public readonly long Timestamp = (long)DateTimeHelper.Timestamp();

		public readonly bool ExceptionTypeReport;

		public string Classifier = string.Empty;

		public BacktraceSourceCode SourceCode;
	}
}
