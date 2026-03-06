using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backtrace.Unity.Model.JsonData
{
	public class SourceCodeData
	{
		internal SourceCodeData(IEnumerable<BacktraceStackFrame> exceptionStack)
		{
			if (exceptionStack == null || exceptionStack.Count<BacktraceStackFrame>() == 0)
			{
				return;
			}
			foreach (BacktraceStackFrame backtraceStackFrame in exceptionStack)
			{
				if (!string.IsNullOrEmpty(backtraceStackFrame.SourceCode))
				{
					string sourceCode = backtraceStackFrame.SourceCode;
					SourceCodeData.SourceCode value = SourceCodeData.SourceCode.FromExceptionStack(backtraceStackFrame);
					this.data.Add(sourceCode, value);
				}
			}
		}

		public Dictionary<string, SourceCodeData.SourceCode> data = new Dictionary<string, SourceCodeData.SourceCode>();

		public class SourceCode
		{
			public int StartLine { get; set; }

			public int StartColumn { get; set; }

			private string _sourceCodeFullPath { get; set; }

			public string SourceCodeFullPath
			{
				get
				{
					if (!string.IsNullOrEmpty(this._sourceCodeFullPath))
					{
						return Path.GetFileName(this._sourceCodeFullPath);
					}
					return string.Empty;
				}
				set
				{
					this._sourceCodeFullPath = value;
				}
			}

			public static SourceCodeData.SourceCode FromExceptionStack(BacktraceStackFrame stackFrame)
			{
				return new SourceCodeData.SourceCode
				{
					StartColumn = stackFrame.Column,
					StartLine = stackFrame.Line,
					SourceCodeFullPath = stackFrame.SourceCodeFullPath
				};
			}
		}
	}
}
