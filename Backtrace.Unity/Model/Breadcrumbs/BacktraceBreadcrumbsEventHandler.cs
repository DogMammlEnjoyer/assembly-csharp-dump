using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Backtrace.Unity.Model.Breadcrumbs
{
	internal sealed class BacktraceBreadcrumbsEventHandler
	{
		public bool HasRegisteredEvents { get; set; }

		public BacktraceBreadcrumbsEventHandler(BacktraceBreadcrumbs breadcrumbs)
		{
			this._thread = Thread.CurrentThread;
			this._breadcrumbs = breadcrumbs;
			this.HasRegisteredEvents = false;
		}

		public void Register(BacktraceBreadcrumbType level)
		{
			this._registeredLevel = level;
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.Navigation))
			{
				this.HasRegisteredEvents = true;
				SceneManager.activeSceneChanged += this.HandleSceneChanged;
				SceneManager.sceneLoaded += this.SceneManager_sceneLoaded;
				SceneManager.sceneUnloaded += this.SceneManager_sceneUnloaded;
			}
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.System))
			{
				this.HasRegisteredEvents = true;
				Application.lowMemory += this.HandleLowMemory;
				Application.quitting += this.HandleApplicationQuitting;
				Application.focusChanged += this.Application_focusChanged;
			}
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.Log))
			{
				this.HasRegisteredEvents = true;
				Application.logMessageReceived += this.HandleMessage;
				Application.logMessageReceivedThreaded += this.HandleBackgroundMessage;
			}
		}

		public void Unregister()
		{
			if (!this.HasRegisteredEvents)
			{
				return;
			}
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.Navigation))
			{
				SceneManager.activeSceneChanged -= this.HandleSceneChanged;
				SceneManager.sceneLoaded -= this.SceneManager_sceneLoaded;
				SceneManager.sceneUnloaded -= this.SceneManager_sceneUnloaded;
			}
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.System))
			{
				Application.lowMemory -= this.HandleLowMemory;
				Application.quitting -= this.HandleApplicationQuitting;
				Application.focusChanged -= this.Application_focusChanged;
			}
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.Log))
			{
				Application.logMessageReceived -= this.HandleMessage;
				Application.logMessageReceivedThreaded -= this.HandleBackgroundMessage;
			}
		}

		private void SceneManager_sceneUnloaded(Scene scene)
		{
			string message = string.Format("SceneManager:scene {0} unloaded", scene.name);
			this.Log(message, LogType.Log, BreadcrumbLevel.Navigation, null);
		}

		private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			string message = string.Format("SceneManager:scene {0} loaded", scene.name);
			this.Log(message, LogType.Log, BreadcrumbLevel.Navigation, new Dictionary<string, string>
			{
				{
					"LoadSceneMode",
					loadSceneMode.ToString()
				}
			});
		}

		private void HandleSceneChanged(Scene sceneFrom, Scene sceneTo)
		{
			string message = string.Format("SceneManager:scene changed from {0} to {1}", string.IsNullOrEmpty(sceneFrom.name) ? "(no scene)" : sceneFrom.name, sceneTo.name);
			this.Log(message, LogType.Log, BreadcrumbLevel.Navigation, new Dictionary<string, string>
			{
				{
					"from",
					sceneFrom.name
				},
				{
					"to",
					sceneTo.name
				}
			});
		}

		private void HandleLowMemory()
		{
			this.Log("Application:low memory", LogType.Warning, BreadcrumbLevel.System, null);
		}

		private void HandleApplicationQuitting()
		{
			this.Log("Application:quitting", LogType.Log, BreadcrumbLevel.System, null);
		}

		private void HandleBackgroundMessage(string condition, string stackTrace, LogType type)
		{
			if (Thread.CurrentThread == this._thread)
			{
				return;
			}
			this.HandleMessage(condition, stackTrace, type);
		}

		private void HandleMessage(string condition, string stackTrace, LogType type)
		{
			object obj;
			if (type != LogType.Error && type != LogType.Exception)
			{
				obj = null;
			}
			else
			{
				(obj = new Dictionary<string, string>()).Add("stackTrace", stackTrace);
			}
			Dictionary<string, string> attributes = obj;
			this.Log(condition, type, BreadcrumbLevel.Log, attributes);
		}

		private void Application_focusChanged(bool hasFocus)
		{
			this.Log("Application:focus changed.", LogType.Assert, BreadcrumbLevel.System, new Dictionary<string, string>
			{
				{
					"hasFocus",
					hasFocus.ToString()
				}
			});
		}

		private void Log(string message, LogType level, BreadcrumbLevel breadcrumbLevel, IDictionary<string, string> attributes = null)
		{
			UnityEngineLogLevel type = BacktraceBreadcrumbs.ConvertLogTypeToLogLevel(level);
			if (!this._breadcrumbs.ShouldLog(breadcrumbLevel, type))
			{
				return;
			}
			this._breadcrumbs.AddBreadcrumbs(message, breadcrumbLevel, type, attributes);
		}

		private void LogNewNetworkStatus(NetworkReachability status)
		{
			this._networkStatus = status;
			this.Log(string.Format("Network:{0}", status), LogType.Log, BreadcrumbLevel.System, null);
		}

		internal void Update()
		{
			if (this._registeredLevel.HasFlag(BacktraceBreadcrumbType.System) && Application.internetReachability != this._networkStatus)
			{
				this.LogNewNetworkStatus(Application.internetReachability);
			}
		}

		private readonly BacktraceBreadcrumbs _breadcrumbs;

		private BacktraceBreadcrumbType _registeredLevel;

		private NetworkReachability _networkStatus;

		private Thread _thread;
	}
}
