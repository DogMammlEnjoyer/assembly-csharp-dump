using System;
using System.Collections.Generic;
using System.Linq;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	public class BacktraceUnhandledException : Exception
	{
		public bool Header
		{
			get
			{
				return this._header;
			}
		}

		public override string Message
		{
			get
			{
				return this._message;
			}
		}

		public string Classifier { get; set; }

		public override string StackTrace
		{
			get
			{
				return this._stacktrace;
			}
		}

		public LogType Type { get; set; }

		internal bool NativeStackTrace { get; private set; }

		public BacktraceUnhandledException(string message, string stacktrace) : base(message)
		{
			this.Type = LogType.Exception;
			this._message = message;
			this._stacktrace = stacktrace;
			if (!string.IsNullOrEmpty(stacktrace))
			{
				IEnumerable<string> enumerable = this._stacktrace.Split('\n', StringSplitOptions.None);
				string beginningOfTheFrame = enumerable.ElementAt(0);
				string stackTraceErrorMessage = this.GetStackTraceErrorMessage(beginningOfTheFrame);
				if (!string.IsNullOrEmpty(stackTraceErrorMessage))
				{
					this._message = stackTraceErrorMessage;
					this._header = true;
					enumerable = enumerable.Skip(1);
				}
				this.StackFrames = this.ConvertStackFrames(enumerable);
			}
			if (string.IsNullOrEmpty(stacktrace) || this.StackFrames.Count == 0)
			{
				this._message = message;
				BacktraceStackTrace backtraceStackTrace = new BacktraceStackTrace(null);
				this.StackFrames = backtraceStackTrace.StackFrames;
			}
			this.TrySetClassifier();
		}

		private string GetStackTraceErrorMessage(string beginningOfTheFrame)
		{
			beginningOfTheFrame = beginningOfTheFrame.Trim();
			if (beginningOfTheFrame.IndexOf("Exception:") != -1)
			{
				return beginningOfTheFrame;
			}
			if (beginningOfTheFrame.IndexOf('(') == -1 || beginningOfTheFrame.IndexOf(')') == -1)
			{
				return beginningOfTheFrame;
			}
			return string.Empty;
		}

		private List<BacktraceStackFrame> ConvertStackFrames(IEnumerable<string> frames)
		{
			List<BacktraceStackFrame> list = new List<BacktraceStackFrame>();
			for (int i = 0; i < frames.Count<string>(); i++)
			{
				string text = frames.ElementAt(i);
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = text.Trim();
					int num = text2.IndexOf(')');
					if (num == -1)
					{
						list.Add(new BacktraceStackFrame
						{
							FunctionName = text
						});
					}
					else
					{
						if (num < 1 && text2[num - 1] != '(')
						{
							list.Add(new BacktraceStackFrame
							{
								FunctionName = text
							});
						}
						list.Add(this.ConvertFrame(text2, num));
					}
				}
			}
			return list;
		}

		private BacktraceStackFrame ConvertFrame(string frameString, int methodNameEndIndex)
		{
			if (frameString.StartsWith("0x"))
			{
				return this.SetNativeStackTraceInformation(frameString);
			}
			if (frameString.StartsWith("#"))
			{
				return this.SetJITStackTraceInformation(frameString);
			}
			return this.SetDefaultStackTraceInformation(frameString, methodNameEndIndex);
		}

		private BacktraceStackFrame SetJITStackTraceInformation(string frameString)
		{
			BacktraceStackFrame backtraceStackFrame = new BacktraceStackFrame
			{
				StackFrameType = BacktraceStackFrameType.Native
			};
			if (!frameString.StartsWith("#"))
			{
				backtraceStackFrame.FunctionName = frameString;
				return backtraceStackFrame;
			}
			frameString = frameString.Substring(frameString.IndexOf(' ')).Trim();
			int num = frameString.IndexOf("(Mono JIT Code)");
			if (num != -1)
			{
				frameString = frameString.Substring(num + "(Mono JIT Code)".Length).Trim();
			}
			int num2 = frameString.IndexOf("(wrapper managed-to-native)");
			if (num2 != -1)
			{
				frameString = frameString.Substring(num2 + "(wrapper managed-to-native)".Length).Trim();
			}
			int num3 = frameString.IndexOf('(');
			int num4 = frameString.IndexOf(')');
			if (num3 != -1 && num4 != -1 && num4 > num3)
			{
				backtraceStackFrame.FunctionName = frameString.Substring(0, num3).Trim();
			}
			else
			{
				backtraceStackFrame.FunctionName = frameString;
			}
			if (!string.IsNullOrEmpty(backtraceStackFrame.FunctionName))
			{
				int num5 = backtraceStackFrame.FunctionName.IndexOf(':');
				if (num5 != -1)
				{
					backtraceStackFrame.Library = backtraceStackFrame.FunctionName.Substring(0, num5).Trim();
					backtraceStackFrame.FunctionName = backtraceStackFrame.FunctionName.Substring(num5 + 1).Trim();
				}
				else
				{
					backtraceStackFrame.Library = "native";
				}
			}
			return backtraceStackFrame;
		}

		private BacktraceStackFrame SetNativeStackTraceInformation(string frameString)
		{
			BacktraceStackFrame backtraceStackFrame = new BacktraceStackFrame
			{
				StackFrameType = BacktraceStackFrameType.Native
			};
			int num = frameString.IndexOf(' ');
			if (num == -1)
			{
				backtraceStackFrame.FunctionName = frameString;
				return backtraceStackFrame;
			}
			backtraceStackFrame.Address = frameString.Substring(0, num);
			int num2 = num + 1;
			if (frameString[num2] == '(')
			{
				num2++;
				int num3 = frameString.IndexOf(')', num2);
				backtraceStackFrame.Library = frameString.Substring(num2, num3 - num2);
				num2 = num3 + 2;
			}
			backtraceStackFrame.FunctionName = frameString.Substring(num2);
			if (backtraceStackFrame.FunctionName.StartsWith("(wrapper managed-to-native)"))
			{
				backtraceStackFrame.FunctionName = backtraceStackFrame.FunctionName.Replace("(wrapper managed-to-native)", string.Empty).Trim();
			}
			if (backtraceStackFrame.FunctionName.StartsWith("(wrapper runtime-invoke)"))
			{
				backtraceStackFrame.FunctionName = backtraceStackFrame.FunctionName.Replace("(wrapper runtime-invoke)", string.Empty).Trim();
			}
			int num4 = backtraceStackFrame.FunctionName.IndexOf('[');
			int num5 = backtraceStackFrame.FunctionName.IndexOf(']');
			if (num4 != -1 && num5 != -1)
			{
				num4++;
				string[] array = backtraceStackFrame.FunctionName.Substring(num4, num5 - num4).Split(new char[]
				{
					':'
				}, 2);
				if (array.Length == 2)
				{
					int.TryParse(array[1], out backtraceStackFrame.Line);
					backtraceStackFrame.Library = array[0];
					backtraceStackFrame.FunctionName = backtraceStackFrame.FunctionName.Substring(num5 + 2);
				}
			}
			return backtraceStackFrame;
		}

		private BacktraceStackFrame SetAndroidStackTraceInformation(string frameString, int parameterStart, int parameterEnd)
		{
			BacktraceStackFrame backtraceStackFrame = new BacktraceStackFrame
			{
				FunctionName = frameString.Substring(0, parameterStart - 1),
				StackFrameType = BacktraceStackFrameType.Android
			};
			string text = frameString.Substring(parameterStart, parameterEnd - parameterStart);
			string[] array = text.Split(':', StringSplitOptions.None);
			if (array.Length == 2)
			{
				backtraceStackFrame.Library = array[0];
				int.TryParse(array[1], out backtraceStackFrame.Line);
			}
			else if (frameString.StartsWith("java.lang") || text == "Unknown Source")
			{
				backtraceStackFrame.Library = text;
			}
			return backtraceStackFrame;
		}

		private BacktraceStackFrame SetDefaultStackTraceInformation(string frameString, int methodNameEndIndex)
		{
			if (frameString.StartsWith("(wrapper remoting-invoke-with-check)"))
			{
				frameString = frameString.Replace("(wrapper remoting-invoke-with-check)", string.Empty);
			}
			int num = frameString.IndexOf('(', methodNameEndIndex + 1);
			if (num == -1)
			{
				return new BacktraceStackFrame
				{
					FunctionName = frameString,
					StackFrameType = BacktraceStackFrameType.Dotnet
				};
			}
			int length = frameString.Length - num;
			string text = frameString.Trim().Substring(num, length);
			int num2 = text.LastIndexOf(':') + 1;
			int num3 = text.LastIndexOf(')') - num2;
			BacktraceStackFrame backtraceStackFrame = new BacktraceStackFrame
			{
				FunctionName = frameString.Substring(0, methodNameEndIndex + 1).Trim(),
				StackFrameType = BacktraceStackFrameType.Dotnet
			};
			if (num3 > 0 && num2 > 0)
			{
				int.TryParse(text.Substring(num2, num3), out backtraceStackFrame.Line);
			}
			if (text[0] == '(' && num2 != -1)
			{
				int num4 = text.StartsWith("(at") ? 3 : 1;
				int num5 = (num2 == 0) ? (text.LastIndexOf(')') - num4) : (num2 - 1 - num4);
				if (num5 < 0)
				{
					return backtraceStackFrame;
				}
				string text2 = text.Substring(num4, num5);
				backtraceStackFrame.Library = ((text2 == null) ? string.Empty : text2.Trim());
				if (!string.IsNullOrEmpty(backtraceStackFrame.Library) && string.Copy(backtraceStackFrame.Library).Replace("0", string.Empty).Length <= 2)
				{
					backtraceStackFrame.Library = null;
				}
				if (string.IsNullOrEmpty(backtraceStackFrame.Library))
				{
					backtraceStackFrame.Library = backtraceStackFrame.FunctionName.Substring(0, backtraceStackFrame.FunctionName.LastIndexOf(".", backtraceStackFrame.FunctionName.IndexOf("(")));
				}
			}
			return backtraceStackFrame;
		}

		private void TrySetClassifier()
		{
			this.Classifier = "error";
			if (string.IsNullOrEmpty(this._message))
			{
				return;
			}
			if (this._message.EndsWith("Exception"))
			{
				this.Classifier = this._message.Split(' ', StringSplitOptions.None).Last<string>();
				return;
			}
			string[] array = this._message.Split(':', StringSplitOptions.None);
			string text = array[0].Trim();
			if (!string.IsNullOrEmpty(text) && text.EndsWith("Exception"))
			{
				if (text == "AndroidJavaException" && text.Length > 1 && array[1].EndsWith("Exception"))
				{
					this.Classifier = array[1].Trim();
					return;
				}
				this.Classifier = text;
			}
		}

		private bool _header;

		private static string[] _javaExtensions = new string[]
		{
			".java",
			".kt",
			"java."
		};

		private string _message;

		private readonly string _stacktrace;

		public readonly List<BacktraceStackFrame> StackFrames;
	}
}
