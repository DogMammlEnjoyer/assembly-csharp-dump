using System;
using System.Collections.Generic;
using System.Diagnostics;
using Backtrace.Unity.Types;

namespace Backtrace.Unity.Model
{
	internal class BacktraceStackTrace
	{
		public BacktraceStackTrace(Exception exception)
		{
			this._exception = exception;
			this.Initialize();
		}

		private void Initialize()
		{
			bool generatedByException = this._exception != null;
			if (this._exception == null)
			{
				StackFrame[] frames = new StackTrace(true).GetFrames();
				this.SetStacktraceInformation(frames, generatedByException);
				return;
			}
			if (this._exception is BacktraceUnhandledException)
			{
				BacktraceUnhandledException ex = this._exception as BacktraceUnhandledException;
				this.StackFrames.InsertRange(0, ex.StackFrames);
				return;
			}
			StackFrame[] frames2 = new StackTrace(this._exception, true).GetFrames();
			if (frames2 == null || frames2.Length == 0)
			{
				frames2 = new StackTrace(true).GetFrames();
			}
			this.SetStacktraceInformation(frames2, true);
		}

		private void SetStacktraceInformation(StackFrame[] frames, bool generatedByException = false)
		{
			if (frames == null || frames.Length == 0)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < frames.Length; i++)
			{
				BacktraceStackFrame backtraceStackFrame = new BacktraceStackFrame(frames[i], generatedByException);
				if (!backtraceStackFrame.InvalidFrame)
				{
					backtraceStackFrame.StackFrameType = BacktraceStackFrameType.Dotnet;
					this.StackFrames.Insert(num, backtraceStackFrame);
					num++;
				}
			}
		}

		public readonly List<BacktraceStackFrame> StackFrames = new List<BacktraceStackFrame>();

		private readonly Exception _exception;
	}
}
