using System;
using System.Collections.Generic;
using UnityEngine;

namespace Backtrace.Unity.Model.Breadcrumbs
{
	internal sealed class BacktraceBreadcrumbs : IBacktraceBreadcrumbs
	{
		public BacktraceBreadcrumbType BreadcrumbsLevel { get; internal set; }

		public UnityEngineLogLevel UnityLogLevel { get; set; }

		public BacktraceBreadcrumbs(IBacktraceLogManager logManager, BacktraceBreadcrumbType level, UnityEngineLogLevel unityLogLevel)
		{
			this.BreadcrumbsLevel = level;
			this.UnityLogLevel = unityLogLevel;
			this.LogManager = logManager;
			this.EventHandler = new BacktraceBreadcrumbsEventHandler(this);
		}

		public void UnregisterEvents()
		{
			this.EventHandler.Unregister();
		}

		public bool ClearBreadcrumbs()
		{
			return this.LogManager.Clear();
		}

		public bool EnableBreadcrumbs(BacktraceBreadcrumbType level, UnityEngineLogLevel unityLogLevel)
		{
			this.UnityLogLevel = unityLogLevel;
			this.BreadcrumbsLevel = level;
			return this.EnableBreadcrumbs();
		}

		public bool EnableBreadcrumbs()
		{
			if (this._enabled)
			{
				return false;
			}
			if (!this.LogManager.Enable())
			{
				return false;
			}
			this.EventHandler.Register(this.BreadcrumbsLevel);
			return true;
		}

		public bool FromBacktrace(BacktraceReport report)
		{
			UnityEngineLogLevel type = report.ExceptionTypeReport ? UnityEngineLogLevel.Error : UnityEngineLogLevel.Info;
			return this.ShouldLog(BreadcrumbLevel.System, type) && this.AddBreadcrumbs(report.Message, BreadcrumbLevel.System, type, null);
		}

		public bool FromMonoBehavior(string message, LogType type, IDictionary<string, string> attributes)
		{
			return this.AddBreadcrumbs(message, BreadcrumbLevel.System, BacktraceBreadcrumbs.ConvertLogTypeToLogLevel(type), attributes);
		}

		public string GetBreadcrumbLogPath()
		{
			return this.LogManager.BreadcrumbsFilePath;
		}

		public bool Info(string message)
		{
			return this.Log(message, LogType.Log, null);
		}

		public bool Info(string message, IDictionary<string, string> attributes)
		{
			return this.Log(message, LogType.Log, attributes);
		}

		public bool Warning(string message)
		{
			return this.Log(message, LogType.Warning, null);
		}

		public bool Warning(string message, IDictionary<string, string> attributes)
		{
			return this.Log(message, LogType.Warning, attributes);
		}

		public bool Debug(string message, IDictionary<string, string> attributes)
		{
			return this.Log(message, LogType.Assert, attributes);
		}

		public bool Debug(string message)
		{
			return this.Log(message, LogType.Assert);
		}

		public bool Exception(string message)
		{
			return this.Log(message, LogType.Exception, null);
		}

		public bool Exception(Exception exception, IDictionary<string, string> attributes)
		{
			return this.Log(exception.Message, LogType.Exception, attributes);
		}

		public bool Exception(Exception exception)
		{
			return this.Log(exception.Message, LogType.Exception, null);
		}

		public bool Exception(string message, IDictionary<string, string> attributes)
		{
			return this.Log(message, LogType.Exception, attributes);
		}

		public bool Log(string message, LogType type)
		{
			return this.Log(message, type, null);
		}

		public bool Log(string message, LogType logType, IDictionary<string, string> attributes)
		{
			return this.Log(message, BreadcrumbLevel.Manual, logType, attributes);
		}

		public bool Log(string message, BreadcrumbLevel level, LogType logType, IDictionary<string, string> attributes)
		{
			UnityEngineLogLevel type = BacktraceBreadcrumbs.ConvertLogTypeToLogLevel(logType);
			return this.AddBreadcrumbs(message, level, type, attributes);
		}

		internal bool AddBreadcrumbs(string message, BreadcrumbLevel level, UnityEngineLogLevel type, IDictionary<string, string> attributes = null)
		{
			return this.ShouldLog(level, type) && this.LogManager.Add(message, level, type, attributes);
		}

		internal bool ShouldLog(BreadcrumbLevel level, UnityEngineLogLevel type)
		{
			return this.ShouldLog((BacktraceBreadcrumbType)level, type);
		}

		internal bool ShouldLog(BacktraceBreadcrumbType level, UnityEngineLogLevel type)
		{
			return this.BreadcrumbsLevel.HasFlag(level) && this.UnityLogLevel.HasFlag(type);
		}

		internal static UnityEngineLogLevel ConvertLogTypeToLogLevel(LogType type)
		{
			switch (type)
			{
			case LogType.Error:
			case LogType.Exception:
				return UnityEngineLogLevel.Error;
			case LogType.Assert:
				return UnityEngineLogLevel.Debug;
			case LogType.Warning:
				return UnityEngineLogLevel.Warning;
			}
			return UnityEngineLogLevel.Info;
		}

		public double BreadcrumbId()
		{
			return this.LogManager.BreadcrumbId();
		}

		public void Update()
		{
			this.EventHandler.Update();
		}

		public static bool CanStoreBreadcrumbs(UnityEngineLogLevel logLevel, BacktraceBreadcrumbType backtraceBreadcrumbsLevel)
		{
			return backtraceBreadcrumbsLevel != BacktraceBreadcrumbType.None && logLevel > UnityEngineLogLevel.None;
		}

		public string Archive()
		{
			IArchiveableBreadcrumbManager archiveableBreadcrumbManager = this.LogManager as IArchiveableBreadcrumbManager;
			if (archiveableBreadcrumbManager == null)
			{
				return string.Empty;
			}
			return archiveableBreadcrumbManager.Archive();
		}

		internal readonly IBacktraceLogManager LogManager;

		internal readonly BacktraceBreadcrumbsEventHandler EventHandler;

		private bool _enabled;
	}
}
