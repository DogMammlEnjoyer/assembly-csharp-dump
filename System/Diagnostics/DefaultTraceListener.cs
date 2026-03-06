using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Diagnostics
{
	/// <summary>Provides the default output methods and behavior for tracing.</summary>
	public class DefaultTraceListener : TraceListener
	{
		static DefaultTraceListener()
		{
			if (!DefaultTraceListener.OnWin32)
			{
				string environmentVariable = Environment.GetEnvironmentVariable("MONO_TRACE_LISTENER");
				if (environmentVariable != null)
				{
					string monoTraceFile;
					string monoTracePrefix;
					if (environmentVariable.StartsWith("Console.Out"))
					{
						monoTraceFile = "Console.Out";
						monoTracePrefix = DefaultTraceListener.GetPrefix(environmentVariable, "Console.Out");
					}
					else if (environmentVariable.StartsWith("Console.Error"))
					{
						monoTraceFile = "Console.Error";
						monoTracePrefix = DefaultTraceListener.GetPrefix(environmentVariable, "Console.Error");
					}
					else
					{
						monoTraceFile = environmentVariable;
						monoTracePrefix = "";
					}
					DefaultTraceListener.MonoTraceFile = monoTraceFile;
					DefaultTraceListener.MonoTracePrefix = monoTracePrefix;
				}
			}
		}

		private static string GetPrefix(string var, string target)
		{
			if (var.Length > target.Length)
			{
				return var.Substring(target.Length + 1);
			}
			return "";
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.DefaultTraceListener" /> class with "Default" as its <see cref="P:System.Diagnostics.TraceListener.Name" /> property value.</summary>
		public DefaultTraceListener() : base("Default")
		{
		}

		/// <summary>Gets or sets a value indicating whether the application is running in user-interface mode.</summary>
		/// <returns>
		///   <see langword="true" /> if user-interface mode is enabled; otherwise, <see langword="false" />.</returns>
		[MonoTODO("AssertUiEnabled defaults to False; should follow Environment.UserInteractive.")]
		public bool AssertUiEnabled
		{
			get
			{
				return this.assertUiEnabled;
			}
			set
			{
				this.assertUiEnabled = value;
			}
		}

		/// <summary>Gets or sets the name of a log file to write trace or debug messages to.</summary>
		/// <returns>The name of a log file to write trace or debug messages to.</returns>
		[MonoTODO]
		public string LogFileName
		{
			get
			{
				return this.logFileName;
			}
			set
			{
				this.logFileName = value;
			}
		}

		/// <summary>Emits or displays a message and a stack trace for an assertion that always fails.</summary>
		/// <param name="message">The message to emit or display.</param>
		public override void Fail(string message)
		{
			base.Fail(message);
		}

		/// <summary>Emits or displays detailed messages and a stack trace for an assertion that always fails.</summary>
		/// <param name="message">The message to emit or display.</param>
		/// <param name="detailMessage">The detailed message to emit or display.</param>
		public override void Fail(string message, string detailMessage)
		{
			base.Fail(message, detailMessage);
			if (this.ProcessUI(message, detailMessage) == DefaultTraceListener.DialogResult.Abort)
			{
				Thread.CurrentThread.Abort();
			}
			this.WriteLine(new StackTrace().ToString());
		}

		private DefaultTraceListener.DialogResult ProcessUI(string message, string detailMessage)
		{
			if (!this.AssertUiEnabled)
			{
				return DefaultTraceListener.DialogResult.None;
			}
			object obj;
			MethodInfo method;
			try
			{
				Assembly assembly = Assembly.Load("System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
				if (assembly == null)
				{
					return DefaultTraceListener.DialogResult.None;
				}
				Type type = assembly.GetType("System.Windows.Forms.MessageBoxButtons");
				obj = Enum.Parse(type, "AbortRetryIgnore");
				method = assembly.GetType("System.Windows.Forms.MessageBox").GetMethod("Show", new Type[]
				{
					typeof(string),
					typeof(string),
					type
				});
			}
			catch
			{
				return DefaultTraceListener.DialogResult.None;
			}
			if (method == null || obj == null)
			{
				return DefaultTraceListener.DialogResult.None;
			}
			string text = string.Format("Assertion Failed: {0} to quit, {1} to debug, {2} to continue", "Abort", "Retry", "Ignore");
			string text2 = string.Format("{0}{1}{2}{1}{1}{3}", new object[]
			{
				message,
				Environment.NewLine,
				detailMessage,
				new StackTrace()
			});
			string a = method.Invoke(null, new object[]
			{
				text2,
				text,
				obj
			}).ToString();
			if (a == "Ignore")
			{
				return DefaultTraceListener.DialogResult.Ignore;
			}
			if (!(a == "Abort"))
			{
				return DefaultTraceListener.DialogResult.Retry;
			}
			return DefaultTraceListener.DialogResult.Abort;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void WriteWindowsDebugString(char* message);

		private unsafe void WriteDebugString(string message)
		{
			if (DefaultTraceListener.OnWin32)
			{
				fixed (string text = message)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					DefaultTraceListener.WriteWindowsDebugString(ptr);
				}
				return;
			}
			this.WriteMonoTrace(message);
		}

		private void WriteMonoTrace(string message)
		{
			string monoTraceFile = DefaultTraceListener.MonoTraceFile;
			if (monoTraceFile == "Console.Out")
			{
				Console.Out.Write(message);
				return;
			}
			if (!(monoTraceFile == "Console.Error"))
			{
				this.WriteLogFile(message, DefaultTraceListener.MonoTraceFile);
				return;
			}
			Console.Error.Write(message);
		}

		private void WritePrefix()
		{
			if (!DefaultTraceListener.OnWin32)
			{
				this.WriteMonoTrace(DefaultTraceListener.MonoTracePrefix);
			}
		}

		private void WriteImpl(string message)
		{
			if (base.NeedIndent)
			{
				this.WriteIndent();
				this.WritePrefix();
			}
			if (Debugger.IsLogging())
			{
				Debugger.Log(0, null, message);
			}
			else
			{
				this.WriteDebugString(message);
			}
			this.WriteLogFile(message, this.LogFileName);
		}

		private void WriteLogFile(string message, string logFile)
		{
			if (logFile != null && logFile.Length != 0)
			{
				FileInfo fileInfo = new FileInfo(logFile);
				StreamWriter streamWriter = null;
				try
				{
					if (fileInfo.Exists)
					{
						streamWriter = fileInfo.AppendText();
					}
					else
					{
						streamWriter = fileInfo.CreateText();
					}
				}
				catch
				{
					return;
				}
				using (streamWriter)
				{
					streamWriter.Write(message);
					streamWriter.Flush();
				}
			}
		}

		/// <summary>Writes the output to the <see langword="OutputDebugString" /> function and to the <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" /> method.</summary>
		/// <param name="message">The message to write to <see langword="OutputDebugString" /> and <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" />.</param>
		public override void Write(string message)
		{
			this.WriteImpl(message);
		}

		/// <summary>Writes the output to the <see langword="OutputDebugString" /> function and to the <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" /> method, followed by a carriage return and line feed (\r\n).</summary>
		/// <param name="message">The message to write to <see langword="OutputDebugString" /> and <see cref="M:System.Diagnostics.Debugger.Log(System.Int32,System.String,System.String)" />.</param>
		public override void WriteLine(string message)
		{
			string message2 = message + Environment.NewLine;
			this.WriteImpl(message2);
			base.NeedIndent = true;
		}

		private static readonly bool OnWin32 = Path.DirectorySeparatorChar == '\\';

		private const string ConsoleOutTrace = "Console.Out";

		private const string ConsoleErrorTrace = "Console.Error";

		private static readonly string MonoTracePrefix;

		private static readonly string MonoTraceFile;

		private string logFileName;

		private bool assertUiEnabled;

		private enum DialogResult
		{
			None,
			Retry,
			Ignore,
			Abort
		}
	}
}
