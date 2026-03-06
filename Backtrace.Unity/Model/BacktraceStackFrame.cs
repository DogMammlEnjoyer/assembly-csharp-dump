using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Backtrace.Unity.Json;
using Backtrace.Unity.Types;

namespace Backtrace.Unity.Model
{
	public class BacktraceStackFrame
	{
		public string FileName
		{
			get
			{
				if (string.IsNullOrEmpty(this.Library))
				{
					return this.GetFileNameFromFunctionName();
				}
				if (this.Library.IndexOfAny(Path.GetInvalidPathChars()) != -1 || !Path.HasExtension(Path.GetFileName(this.Library)))
				{
					return this.GetFileNameFromFunctionName();
				}
				return this.GetFileNameFromLibraryName();
			}
		}

		public bool InvalidFrame { get; set; }

		public BacktraceJObject ToJson()
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject(new Dictionary<string, string>
			{
				{
					"funcName",
					this.FunctionName
				},
				{
					"path",
					this.FileName
				},
				{
					"metadata_token",
					this.MemberInfo
				},
				{
					"assembly",
					this.Assembly
				}
			});
			backtraceJObject.Add("address", (long)this.ILOffset);
			if (!string.IsNullOrEmpty(this.Library) && (!this.Library.StartsWith("<") || !this.Library.EndsWith(">")))
			{
				backtraceJObject.Add("library", this.Library);
			}
			if (this.Line != 0)
			{
				backtraceJObject.Add("line", (long)this.Line);
			}
			if (this.Column != 0)
			{
				backtraceJObject.Add("column", (long)this.Column);
			}
			if (!string.IsNullOrEmpty(this.SourceCode))
			{
				backtraceJObject.Add("sourceCode", this.SourceCode);
			}
			return backtraceJObject;
		}

		public BacktraceStackFrame()
		{
		}

		public BacktraceStackFrame(StackFrame frame, bool generatedByException)
		{
			if (frame == null)
			{
				this.InvalidFrame = true;
				return;
			}
			MethodBase method = frame.GetMethod();
			if (method == null)
			{
				this.InvalidFrame = true;
				return;
			}
			Type declaringType = method.DeclaringType;
			string assembly = "unknown";
			if (declaringType != null)
			{
				string name = declaringType.Assembly.GetName().Name;
				if (name != null)
				{
					assembly = name;
					if (name == "Backtrace.Unity")
					{
						this.InvalidFrame = true;
						return;
					}
				}
			}
			this.FunctionName = this.GetMethodName(method);
			this.SourceCodeFullPath = frame.GetFileName();
			this.Line = frame.GetFileLineNumber();
			this.ILOffset = frame.GetILOffset();
			this.Assembly = assembly;
			this.Library = (string.IsNullOrEmpty(this.SourceCodeFullPath) ? method.DeclaringType.ToString() : this.SourceCodeFullPath);
			this.Column = frame.GetFileColumnNumber();
			try
			{
				this.MemberInfo = method.MetadataToken.ToString(CultureInfo.InvariantCulture);
			}
			catch (InvalidOperationException)
			{
			}
			this.InvalidFrame = false;
		}

		private string GetMethodName(MethodBase method)
		{
			string arg = method.Name.StartsWith(".") ? method.Name.Substring(1, method.Name.Length - 1) : method.Name;
			return string.Format("{0}.{1}()", (method.DeclaringType == null) ? null : method.DeclaringType.ToString(), arg);
		}

		private string GetFileNameFromLibraryName()
		{
			string text = Path.GetFileName(this.Library).Trim();
			int num = text.LastIndexOf(".");
			if (num == -1 || text.IndexOf(".") == num)
			{
				return text;
			}
			text = text.Substring(num + 1);
			BacktraceStackFrameType stackFrameType = this.StackFrameType;
			if (stackFrameType == BacktraceStackFrameType.Dotnet)
			{
				return string.Format("{0}.cs", text);
			}
			if (stackFrameType != BacktraceStackFrameType.Android)
			{
				return text;
			}
			return string.Format("{0}.java", text);
		}

		private string GetFileNameFromFunctionName()
		{
			if (string.IsNullOrEmpty(this.FunctionName))
			{
				return string.Empty;
			}
			int num = this.FunctionName.IndexOf('(');
			if (num == -1)
			{
				num = this.FunctionName.Length - 1;
			}
			int num2 = -1;
			for (int i = 0; i < BacktraceStackFrame._frameSeparators.Length; i++)
			{
				num2 = this.FunctionName.LastIndexOf(BacktraceStackFrame._frameSeparators[i], num);
				if (num2 != -1)
				{
					break;
				}
			}
			if (num2 == -1)
			{
				return string.Empty;
			}
			string[] array = this.FunctionName.Substring(0, num2).Split(new char[]
			{
				'.'
			});
			int num3 = array.Length - 1;
			string text = array[num3];
			while (string.IsNullOrEmpty(text) && num3 > 0)
			{
				text = array[num3 - 1];
				num3--;
			}
			if (string.IsNullOrEmpty(text))
			{
				return this.Library;
			}
			if ((text.IndexOfAny(Path.GetInvalidPathChars()) == -1 && Path.HasExtension(text)) || this.StackFrameType == BacktraceStackFrameType.Unknown)
			{
				return text;
			}
			BacktraceStackFrameType stackFrameType = this.StackFrameType;
			if (stackFrameType == BacktraceStackFrameType.Dotnet)
			{
				return string.Format("{0}.cs", text);
			}
			if (stackFrameType != BacktraceStackFrameType.Android)
			{
				return text;
			}
			return string.Format("{0}.java", text);
		}

		public override string ToString()
		{
			return string.Format("{0} (at {1}:{2})", this.FunctionName, this.Library, this.Line.ToString(CultureInfo.InvariantCulture));
		}

		private static string[] _frameSeparators = new string[]
		{
			"::",
			":",
			"."
		};

		public string FunctionName;

		internal BacktraceStackFrameType StackFrameType;

		public int Line;

		public string MemberInfo;

		public string SourceCodeFullPath;

		public int Column;

		public int ILOffset;

		public string SourceCode;

		public string Address;

		public string Assembly;

		public string Library;
	}
}
