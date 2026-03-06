using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace Fusion
{
	public abstract class FusionEditorLog
	{
		[Conditional("FUSION_EDITOR_TRACE")]
		public static void TraceConfig(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Config]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void WarnConfig(string msg)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Config]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogConfig(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Config]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void ErrorConfig(string msg)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Config]</color> " + msg);
		}

		[Conditional("FUSION_EDITOR_TRACE")]
		public static void TraceInstaller(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Installer]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void WarnInstaller(string msg)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Installer]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogInstaller(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Installer]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void ErrorInstaller(string msg)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Installer]</color> " + msg);
		}

		public static void SetPrefixColor(Color color)
		{
			FusionEditorLog.SetPrefixColor(color);
		}

		public static void SetPrefixColor(Color32 c)
		{
			FusionEditorLog.s_prefixColor = "=" + FusionEditorLog.Color32ToHex(c);
		}

		private static string Color32ToHex(Color32 color)
		{
			return string.Format("#{0:X6}", (int)color.r << 16 | (int)color.g << 8 | (int)color.b);
		}

		public static void Initialize(bool isDarkMode)
		{
			if (isDarkMode)
			{
				FusionEditorLog.SetPrefixColor(FusionUnityLoggerBase.DefaultDarkPrefixColor);
				return;
			}
			FusionEditorLog.SetPrefixColor(FusionUnityLoggerBase.DefaultLightPrefixColor);
		}

		[Conditional("UNITY_ASSERTIONS")]
		[AssertionMethod]
		[ContractAnnotation("condition: false => halt")]
		public static void Assert(bool condition, string message)
		{
		}

		[Conditional("UNITY_ASSERTIONS")]
		[AssertionMethod]
		[ContractAnnotation("condition: false => halt")]
		public static void Assert(bool condition)
		{
		}

		[Conditional("FUSION_EDITOR_TRACE_IMPORT")]
		public static void TraceImport(string assetPath, string msg)
		{
			Debug.Log(string.Concat(new string[]
			{
				"<color",
				FusionEditorLog.s_prefixColor,
				">[FusionEditor/Import]</color> ",
				assetPath,
				": ",
				msg
			}));
		}

		[Conditional("UNITY_EDITOR")]
		public static void WarnImport(string assetPath, string msg)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"<color",
				FusionEditorLog.s_prefixColor,
				">[FusionEditor/Import]</color> ",
				assetPath,
				": ",
				msg
			}));
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogImport(string assetPath, string msg)
		{
			Debug.Log(string.Concat(new string[]
			{
				"<color",
				FusionEditorLog.s_prefixColor,
				">[FusionEditor/Import]</color> ",
				assetPath,
				": ",
				msg
			}));
		}

		[Conditional("UNITY_EDITOR")]
		public static void ErrorImport(string assetPath, string msg)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"<color",
				FusionEditorLog.s_prefixColor,
				">[FusionEditor/Import]</color> ",
				assetPath,
				": ",
				msg
			}));
		}

		[Conditional("FUSION_EDITOR_TRACE_IMPORT")]
		public static void TraceImport(string msg, Object asset)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color>: " + msg, asset);
		}

		[Conditional("UNITY_EDITOR")]
		public static void WarnImport(string msg, Object asset)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color>: " + msg, asset);
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogImport(string msg, Object asset)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color>: " + msg, asset);
		}

		[Conditional("UNITY_EDITOR")]
		public static void ErrorImport(string msg, Object asset)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color>: " + msg, asset);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Warn(string msg, Object obj)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color>: " + msg, obj);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(string msg, Object obj)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color>: " + msg, obj);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Error(string msg, Object obj)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color>: " + msg, obj);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Exception(string message, Exception ex)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"<color",
				FusionEditorLog.s_prefixColor,
				">[FusionEditor]</color>: ",
				message,
				" <i>See next error log entry for details.</i>"
			}));
			ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(ex);
			Thread thread = new Thread(delegate()
			{
				edi.Throw();
			});
			thread.Start();
			thread.Join();
		}

		[Conditional("UNITY_EDITOR")]
		public static void Exception(Exception ex)
		{
			Debug.LogWarning(string.Format("<color{0}>[FusionEditor]</color>: {1} <i>See next error log entry for details.</i>", FusionEditorLog.s_prefixColor, ex.GetType()));
			ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(ex);
			Thread thread = new Thread(delegate()
			{
				edi.Throw();
			});
			thread.Start();
			thread.Join();
		}

		[Conditional("FUSION_EDITOR_TRACE")]
		public static void Trace(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Warn(string msg)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Log(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Error(string msg)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor]</color> " + msg);
		}

		[Conditional("FUSION_EDITOR_TRACE_IMPORT")]
		public static void TraceImport(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void WarnImport(string msg)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogImport(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void ErrorImport(string msg)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Import]</color> " + msg);
		}

		[Conditional("FUSION_EDITOR_TRACE_INSPECTOR")]
		public static void TraceInspector(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Inspector]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void WarnInspector(string msg)
		{
			Debug.LogWarning("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Inspector]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void LogInspector(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Inspector]</color> " + msg);
		}

		[Conditional("UNITY_EDITOR")]
		public static void ErrorInspector(string msg)
		{
			Debug.LogError("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Inspector]</color> " + msg);
		}

		[Conditional("FUSION_EDITOR_TRACE_TEST")]
		public static void TraceTest(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Test]</color> " + msg);
		}

		[Conditional("FUSION_EDITOR_TRACE_MPPM")]
		public static void TraceMppm(string msg)
		{
			Debug.Log("<color" + FusionEditorLog.s_prefixColor + ">[FusionEditor/Mppm]</color> " + msg);
		}

		private static string s_prefixColor;
	}
}
