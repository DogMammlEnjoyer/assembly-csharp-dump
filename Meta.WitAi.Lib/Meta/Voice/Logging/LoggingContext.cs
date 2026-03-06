using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Meta.WitAi;

namespace Meta.Voice.Logging
{
	public class LoggingContext
	{
		internal LoggingContext(StackTrace stackTrace)
		{
			this._stackTrace = stackTrace;
			this._callSiteSourceLineNumber = -1;
		}

		internal LoggingContext([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			this._stackTrace = null;
			this._callSiteMemberName = memberName;
			this._callSiteSourceFilePath = sourceFilePath;
			this._callSiteSourceLineNumber = sourceLineNumber;
		}

		public override string ToString()
		{
			StackTrace stackTrace = this._stackTrace;
			if (stackTrace == null)
			{
				return null;
			}
			return stackTrace.ToString();
		}

		private void AppendSingleFrame(StringBuilder sb, bool colorLogs)
		{
			if (this._callSiteSourceLineNumber > 0)
			{
				sb.Append("=== STACK TRACE ===\n");
				string callSiteSourceFilePath = this._callSiteSourceFilePath;
				sb.Append(string.Format("[{0}:{1}] ", callSiteSourceFilePath, this._callSiteSourceLineNumber));
				sb.Append(colorLogs ? ("<color=#39CC8F>" + this._callSiteMemberName + "</color>(?)\n") : (this._callSiteMemberName + "(?)\n"));
			}
		}

		private void AppendFullStack(StringBuilder sb, bool colorLogs, StackFrame[] frames)
		{
			sb.Append("=== STACK TRACE ===\n");
			foreach (StackFrame stackFrame in frames)
			{
				MethodBase method = stackFrame.GetMethod();
				Type declaringType = method.DeclaringType;
				if (!(declaringType == null) && !LoggingContext.IsLoggingClass(method.DeclaringType) && !LoggingContext.IsSystemClass(method.DeclaringType))
				{
					string fileName = stackFrame.GetFileName();
					int fileLineNumber = stackFrame.GetFileLineNumber();
					string str = string.Join(", ", from p in method.GetParameters()
					select p.ParameterType.Name ?? "");
					if (!string.IsNullOrEmpty(fileName))
					{
						string fileName2 = Path.GetFileName(fileName);
						fileName.Replace(this._workingDirectory, "");
						sb.Append(string.Format("[{0}:{1}] ", fileName2, fileLineNumber));
					}
					string text = method.Name ?? "";
					sb.Append((declaringType != null) ? declaringType.Name : null);
					sb.Append('.');
					sb.Append(colorLogs ? ("<color=#39CC8F>" + method.Name + "</color>") : (text ?? ""));
					sb.Append("(" + str + ")\n");
				}
			}
		}

		public void AppendRelevantContext(StringBuilder sb, bool colorLogs)
		{
			StackTrace stackTrace = this._stackTrace;
			StackFrame[] array = (stackTrace != null) ? stackTrace.GetFrames() : null;
			if (array == null)
			{
				this.AppendSingleFrame(sb, colorLogs);
				return;
			}
			this.AppendFullStack(sb, colorLogs, array);
		}

		[return: TupleElementNames(new string[]
		{
			"fileName",
			"lineNumber"
		})]
		public ValueTuple<string, int> GetCallSite()
		{
			if (this._callSiteSourceLineNumber > 0)
			{
				return new ValueTuple<string, int>(this._callSiteSourceFilePath, this._callSiteSourceLineNumber);
			}
			if (this._stackTrace == null)
			{
				return new ValueTuple<string, int>(string.Empty, 0);
			}
			for (int i = 1; i < this._stackTrace.FrameCount; i++)
			{
				StackFrame frame = this._stackTrace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (!(method.DeclaringType == null) && !LoggingContext.IsLoggingClass(method.DeclaringType) && !LoggingContext.IsSystemClass(method.DeclaringType))
				{
					string fileName = frame.GetFileName();
					string item = (fileName != null) ? fileName.Replace('\\', '/') : null;
					int fileLineNumber = frame.GetFileLineNumber();
					return new ValueTuple<string, int>(item, fileLineNumber);
				}
			}
			return new ValueTuple<string, int>(string.Empty, 0);
		}

		private static bool IsLoggingClass(Type type)
		{
			return typeof(ICoreLogger).IsAssignableFrom(type) || typeof(ILogWriter).IsAssignableFrom(type) || type == typeof(VLog);
		}

		private static bool IsSystemClass(Type type)
		{
			string @namespace = type.Namespace;
			return @namespace != null && (@namespace.StartsWith("Unity") || @namespace.StartsWith("System") || @namespace.StartsWith("Microsoft"));
		}

		private readonly StackTrace _stackTrace;

		private readonly string _callSiteMemberName;

		private readonly string _callSiteSourceFilePath;

		private readonly int _callSiteSourceLineNumber;

		private readonly string _workingDirectory = Directory.GetCurrentDirectory();
	}
}
