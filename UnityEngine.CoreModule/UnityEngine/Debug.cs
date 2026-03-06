using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Debug/Debug.bindings.h")]
	[NativeHeader("Runtime/Diagnostics/Validation.h")]
	[NativeHeader("Runtime/Diagnostics/IntegrityCheck.h")]
	public class Debug
	{
		public static ILogger unityLogger
		{
			get
			{
				return Debug.s_Logger;
			}
		}

		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
		{
			bool depthTest = true;
			Debug.DrawLine(start, end, color, duration, depthTest);
		}

		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end, Color color)
		{
			bool depthTest = true;
			float duration = 0f;
			Debug.DrawLine(start, end, color, duration, depthTest);
		}

		[ExcludeFromDocs]
		public static void DrawLine(Vector3 start, Vector3 end)
		{
			bool depthTest = true;
			float duration = 0f;
			Color white = Color.white;
			Debug.DrawLine(start, end, white, duration, depthTest);
		}

		[FreeFunction("DebugDrawLine", IsThreadSafe = true)]
		public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine_Injected(ref start, ref end, ref color, duration, depthTest);
		}

		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
		{
			bool depthTest = true;
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}

		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir, Color color)
		{
			bool depthTest = true;
			float duration = 0f;
			Debug.DrawRay(start, dir, color, duration, depthTest);
		}

		[ExcludeFromDocs]
		public static void DrawRay(Vector3 start, Vector3 dir)
		{
			bool depthTest = true;
			float duration = 0f;
			Color white = Color.white;
			Debug.DrawRay(start, dir, white, duration, depthTest);
		}

		public static void DrawRay(Vector3 start, Vector3 dir, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
		{
			Debug.DrawLine(start, start + dir, color, duration, depthTest);
		}

		[FreeFunction("PauseEditor")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Break();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DebugBreak();

		[ThreadSafe]
		public unsafe static int ExtractStackTraceNoAlloc(byte* buffer, int bufferMax, string projectFolder)
		{
			int result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(projectFolder, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = projectFolder.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = Debug.ExtractStackTraceNoAlloc_Injected(buffer, bufferMax, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		public static void Log(object message)
		{
			Debug.unityLogger.Log(LogType.Log, message);
		}

		public static void Log(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Log, message, context);
		}

		public static void LogFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Log, format, args);
		}

		public static void LogFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Log, context, format, args);
		}

		public static void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
		{
			DebugLogHandler debugLogHandler = Debug.unityLogger.logHandler as DebugLogHandler;
			bool flag = debugLogHandler == null;
			if (flag)
			{
				Debug.unityLogger.LogFormat(logType, context, format, args);
			}
			else
			{
				bool flag2 = Debug.unityLogger.IsLogTypeAllowed(logType);
				if (flag2)
				{
					debugLogHandler.LogFormat(logType, logOptions, context, format, args);
				}
			}
		}

		public static void LogError(object message)
		{
			Debug.unityLogger.Log(LogType.Error, message);
		}

		public static void LogError(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Error, message, context);
		}

		public static void LogErrorFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Error, format, args);
		}

		public static void LogErrorFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Error, context, format, args);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearDeveloperConsole();

		public static extern bool developerConsoleEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static extern bool developerConsoleVisible { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		public static void LogException(Exception exception)
		{
			Debug.unityLogger.LogException(exception, null);
		}

		public static void LogException(Exception exception, Object context)
		{
			Debug.unityLogger.LogException(exception, context);
		}

		public static void LogWarning(object message)
		{
			Debug.unityLogger.Log(LogType.Warning, message);
		}

		public static void LogWarning(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Warning, message, context);
		}

		public static void LogWarningFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Warning, format, args);
		}

		public static void LogWarningFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Warning, context, format, args);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, "Assertion failed");
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, Object context)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, "Assertion failed", context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, object message, Object context)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void Assert(bool condition, string message, Object context)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.Log(LogType.Assert, message, context);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, string format, params object[] args)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, format, args);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void AssertFormat(bool condition, Object context, string format, params object[] args)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
			}
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message)
		{
			Debug.unityLogger.Log(LogType.Assert, message);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message, Object context)
		{
			Debug.unityLogger.Log(LogType.Assert, message, context);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Assert, format, args);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertionFormat(Object context, string format, params object[] args)
		{
			Debug.unityLogger.LogFormat(LogType.Assert, context, format, args);
		}

		public static extern bool isDebugBuild { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[NativeThrows]
		internal static extern DiagnosticSwitch[] diagnosticSwitches { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.UIElementsModule",
			"UnityEngine.TextCoreTextEngineModule",
			"UnityEngine.IMGUIModule"
		})]
		internal static DiagnosticSwitch GetDiagnosticSwitch(string name)
		{
			foreach (DiagnosticSwitch diagnosticSwitch in Debug.diagnosticSwitches)
			{
				bool flag = diagnosticSwitch.name == name;
				if (flag)
				{
					return diagnosticSwitch;
				}
			}
			throw new ArgumentException("Could not find DiagnosticSwitch named " + name);
		}

		[RequiredByNativeCode]
		internal static bool CallOverridenDebugHandler(Exception exception, Object obj)
		{
			bool flag = Debug.unityLogger.logHandler is DebugLogHandler;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				try
				{
					Debug.unityLogger.LogException(exception, obj);
				}
				catch (Exception arg)
				{
					Debug.s_DefaultLogger.LogError(string.Format("Invalid exception thrown from custom {0}.LogException(). Message: {1}", Debug.unityLogger.logHandler.GetType(), arg), obj);
					return false;
				}
				result = true;
			}
			return result;
		}

		[RequiredByNativeCode]
		internal static bool IsLoggingEnabled()
		{
			bool flag = Debug.unityLogger.logHandler is DebugLogHandler;
			bool logEnabled;
			if (flag)
			{
				logEnabled = Debug.unityLogger.logEnabled;
			}
			else
			{
				logEnabled = Debug.s_DefaultLogger.logEnabled;
			}
			return logEnabled;
		}

		[FreeFunction("RetrieveStartupLogs")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Debug.StartupLog[] RetrieveStartupLogs();

		[FreeFunction("CheckApplicationIntegrity")]
		public static string CheckIntegrity(IntegrityCheckLevel level)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				Debug.CheckIntegrity_Injected(level, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[FreeFunction("IsValidationLevelEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValidationLevelEnabled(ValidationLevel level);

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Conditional("UNITY_ASSERTIONS")]
		[Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
		public static void Assert(bool condition, string format, params object[] args)
		{
			bool flag = !condition;
			if (flag)
			{
				Debug.unityLogger.LogFormat(LogType.Assert, format, args);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Debug.logger is obsolete. Please use Debug.unityLogger instead (UnityUpgradable) -> unityLogger")]
		public static ILogger logger
		{
			get
			{
				return Debug.s_Logger;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawLine_Injected([In] ref Vector3 start, [In] ref Vector3 end, [DefaultValue("Color.white")] [In] ref Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int ExtractStackTraceNoAlloc_Injected(byte* buffer, int bufferMax, ref ManagedSpanWrapper projectFolder);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CheckIntegrity_Injected(IntegrityCheckLevel level, out ManagedSpanWrapper ret);

		internal static readonly ILogger s_DefaultLogger = new Logger(new DebugLogHandler());

		internal static ILogger s_Logger = new Logger(new DebugLogHandler());

		[NativeHeader("Runtime/Export/Debug/LogCapture.bindings.h")]
		public struct StartupLog
		{
			public long timestamp;

			public LogType logType;

			public string message;
		}
	}
}
