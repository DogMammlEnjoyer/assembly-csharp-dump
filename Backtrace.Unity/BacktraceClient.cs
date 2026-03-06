using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Backtrace.Unity.Common;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Breadcrumbs;
using Backtrace.Unity.Model.Database;
using Backtrace.Unity.Model.JsonData;
using Backtrace.Unity.Runtime.Native;
using Backtrace.Unity.Services;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity
{
	public class BacktraceClient : MonoBehaviour, IBacktraceClient
	{
		public IBacktraceBreadcrumbs Breadcrumbs
		{
			get
			{
				return this._breadcrumbs;
			}
		}

		public bool Enabled { get; private set; }

		internal AttributeProvider AttributeProvider
		{
			get
			{
				if (this._attributeProvider == null)
				{
					this._attributeProvider = new AttributeProvider();
				}
				return this._attributeProvider;
			}
			set
			{
				this._attributeProvider = value;
			}
		}

		public IBacktraceMetrics Metrics
		{
			get
			{
				if (this._metrics == null && this.Configuration != null && this.Configuration.EnableMetricsSupport)
				{
					string universeName = this.Configuration.GetUniverseName();
					string token = this.Configuration.GetToken();
					this._metrics = new BacktraceMetrics(this.AttributeProvider, (long)((ulong)this.Configuration.GetEventAggregationIntervalTimerInMs()), BacktraceMetrics.GetDefaultUniqueEventsUrl(universeName, token), BacktraceMetrics.GetDefaultSummedEventsUrl(universeName, token))
					{
						IgnoreSslValidation = this.Configuration.IgnoreSslValidation
					};
				}
				return this._metrics;
			}
		}

		internal Random Random
		{
			get
			{
				if (this._random == null)
				{
					this._random = new Random();
				}
				return this._random;
			}
		}

		public string this[string index]
		{
			get
			{
				return this.AttributeProvider[index];
			}
			set
			{
				this.AttributeProvider[index] = value;
				if (this._nativeClient != null)
				{
					this._nativeClient.SetAttribute(index, value);
				}
			}
		}

		public void AddAttachment(string pathToAttachment)
		{
			this._clientReportAttachments.Add(pathToAttachment);
		}

		public IEnumerable<string> GetAttachments()
		{
			return this._clientReportAttachments;
		}

		public void SetAttributes(Dictionary<string, string> attributes)
		{
			if (attributes == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> keyValuePair in attributes)
			{
				this[keyValuePair.Key] = keyValuePair.Value;
			}
		}

		public int GetAttributesCount()
		{
			return this.AttributeProvider.Count();
		}

		public static BacktraceClient Instance
		{
			get
			{
				return BacktraceClient._instance;
			}
		}

		public Action<Exception> OnServerError
		{
			get
			{
				if (this.BacktraceApi != null)
				{
					return this.BacktraceApi.OnServerError;
				}
				return null;
			}
			set
			{
				if (this.ValidClientConfiguration())
				{
					this.BacktraceApi.OnServerError = value;
				}
			}
		}

		public Func<string, BacktraceData, BacktraceResult> RequestHandler
		{
			get
			{
				if (this.BacktraceApi != null)
				{
					return this.BacktraceApi.RequestHandler;
				}
				return null;
			}
			set
			{
				if (this.ValidClientConfiguration())
				{
					this.BacktraceApi.RequestHandler = value;
				}
			}
		}

		public Action<BacktraceResult> OnServerResponse
		{
			get
			{
				if (this.BacktraceApi != null)
				{
					return this.BacktraceApi.OnServerResponse;
				}
				return null;
			}
			set
			{
				if (this.ValidClientConfiguration())
				{
					this.BacktraceApi.OnServerResponse = value;
				}
			}
		}

		public Action<BacktraceReport> OnClientReportLimitReached
		{
			get
			{
				return this._onClientReportLimitReached;
			}
			set
			{
				if (this.ValidClientConfiguration())
				{
					this._onClientReportLimitReached = value;
				}
			}
		}

		internal INativeClient NativeClient
		{
			get
			{
				return this._nativeClient;
			}
		}

		public bool EnablePerformanceStatistics
		{
			get
			{
				return this.Configuration.PerformanceStatistics;
			}
		}

		public int GameObjectDepth
		{
			get
			{
				if (this.Configuration.GameObjectDepth != 0)
				{
					return this.Configuration.GameObjectDepth;
				}
				return 16;
			}
		}

		internal IBacktraceApi BacktraceApi
		{
			get
			{
				return this._backtraceApi;
			}
			set
			{
				this._backtraceApi = value;
				if (this.Database != null)
				{
					this.Database.SetApi(this._backtraceApi);
				}
			}
		}

		internal ReportLimitWatcher ReportLimitWatcher
		{
			get
			{
				return this._reportLimitWatcher;
			}
			set
			{
				this._reportLimitWatcher = value;
				if (this.Database != null)
				{
					this.Database.SetReportWatcher(this._reportLimitWatcher);
				}
			}
		}

		public static BacktraceClient Initialize(BacktraceConfiguration configuration, Dictionary<string, string> attributes = null, string gameObjectName = "BacktraceClient")
		{
			if (string.IsNullOrEmpty(gameObjectName))
			{
				throw new ArgumentException("Missing game object name");
			}
			if (configuration == null || string.IsNullOrEmpty(configuration.ServerUrl))
			{
				throw new ArgumentException("Missing valid configuration");
			}
			if (BacktraceClient.Instance != null)
			{
				return BacktraceClient.Instance;
			}
			GameObject gameObject = new GameObject(gameObjectName, new Type[]
			{
				typeof(BacktraceClient),
				typeof(BacktraceDatabase)
			});
			BacktraceClient component = gameObject.GetComponent<BacktraceClient>();
			component.Configuration = configuration;
			if (configuration.Enabled)
			{
				gameObject.GetComponent<BacktraceDatabase>().Configuration = configuration;
			}
			gameObject.SetActive(true);
			component.Refresh();
			component.SetAttributes(attributes);
			return component;
		}

		public static BacktraceClient Initialize(string url, string databasePath, Dictionary<string, string> attributes = null, string gameObjectName = "BacktraceClient")
		{
			return BacktraceClient.Initialize(url, databasePath, attributes, null, gameObjectName);
		}

		public static BacktraceClient Initialize(string url, string databasePath, Dictionary<string, string> attributes = null, string[] attachments = null, string gameObjectName = "BacktraceClient")
		{
			BacktraceConfiguration backtraceConfiguration = ScriptableObject.CreateInstance<BacktraceConfiguration>();
			backtraceConfiguration.ServerUrl = url;
			backtraceConfiguration.AttachmentPaths = attachments;
			backtraceConfiguration.Enabled = true;
			backtraceConfiguration.DatabasePath = databasePath;
			backtraceConfiguration.CreateDatabase = true;
			return BacktraceClient.Initialize(backtraceConfiguration, attributes, gameObjectName);
		}

		public static BacktraceClient Initialize(string url, Dictionary<string, string> attributes = null, string gameObjectName = "BacktraceClient")
		{
			return BacktraceClient.Initialize(url, attributes, new string[0], gameObjectName);
		}

		public static BacktraceClient Initialize(string url, Dictionary<string, string> attributes = null, string[] attachments = null, string gameObjectName = "BacktraceClient")
		{
			BacktraceConfiguration backtraceConfiguration = ScriptableObject.CreateInstance<BacktraceConfiguration>();
			backtraceConfiguration.ServerUrl = url;
			backtraceConfiguration.AttachmentPaths = attachments;
			backtraceConfiguration.Enabled = false;
			return BacktraceClient.Initialize(backtraceConfiguration, attributes, gameObjectName);
		}

		public void OnDisable()
		{
			this.Enabled = false;
		}

		public void Refresh()
		{
			if (this.Configuration == null || !this.Configuration.IsValid())
			{
				return;
			}
			if (BacktraceClient.Instance != null)
			{
				return;
			}
			this.Enabled = true;
			this._current = Thread.CurrentThread;
			this.CaptureUnityMessages();
			this._reportLimitWatcher = new ReportLimitWatcher(Convert.ToUInt32(this.Configuration.ReportPerMin));
			this._clientReportAttachments = this.Configuration.GetAttachmentPaths();
			this.BacktraceApi = new BacktraceApi(new BacktraceCredentials(this.Configuration.GetValidServerUrl()), this.Configuration.IgnoreSslValidation);
			this.BacktraceApi.EnablePerformanceStatistics = this.Configuration.PerformanceStatistics;
			if (!this.Configuration.DestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
				BacktraceClient._instance = this;
			}
			this.EnableMetrics(false);
			string text = string.Empty;
			if (this.Configuration.Enabled)
			{
				this.Database = base.GetComponent<BacktraceDatabase>();
				if (this.Database != null)
				{
					this.Database.Reload();
					this._breadcrumbs = (BacktraceBreadcrumbs)this.Database.Breadcrumbs;
					this.Database.SetApi(this.BacktraceApi);
					this.Database.SetReportWatcher(this._reportLimitWatcher);
					if (this._breadcrumbs != null)
					{
						text = this._breadcrumbs.GetBreadcrumbLogPath();
					}
				}
			}
			if (this.Database != null)
			{
				IDictionary<string, string> attributes = this.AttributeProvider.GenerateAttributes(false);
				IList<string> nativeAttachments = this.GetNativeAttachments();
				if (!string.IsNullOrEmpty(text))
				{
					nativeAttachments.Add(text);
				}
				this._nativeClient = NativeClientFactory.CreateNativeClient(this.Configuration, base.name, this._breadcrumbs, attributes, nativeAttachments);
				this.AttributeProvider.AddDynamicAttributeProvider(this._nativeClient);
			}
		}

		public bool EnableBreadcrumbsSupport()
		{
			return this.Database != null && this.Database.EnableBreadcrumbsSupport();
		}

		public bool EnableMetrics()
		{
			return this.EnableMetrics(true);
		}

		private bool EnableMetrics(bool enableIfConfigurationIsDisabled = true)
		{
			if (!this.Configuration.EnableMetricsSupport)
			{
				if (!enableIfConfigurationIsDisabled)
				{
					return false;
				}
				Debug.LogWarning("Event aggregation configuration was disabled. Enabling it manually via API");
			}
			return this.EnableMetrics("guid");
		}

		public bool EnableMetrics(string uniqueAttributeName = "guid")
		{
			string universeName = this.Configuration.GetUniverseName();
			if (string.IsNullOrEmpty(universeName))
			{
				Debug.LogWarning("Cannot initialize event aggregation - Unknown Backtrace URL.");
				return false;
			}
			string token = this.Configuration.GetToken();
			this.EnableMetrics(BacktraceMetrics.GetDefaultUniqueEventsUrl(universeName, token), BacktraceMetrics.GetDefaultSummedEventsUrl(universeName, token), this.Configuration.GetEventAggregationIntervalTimerInMs(), uniqueAttributeName);
			return true;
		}

		public bool EnableMetrics(string uniqueEventsSubmissionUrl, string summedEventsSubmissionUrl, uint timeIntervalInSec = 1800U, string uniqueAttributeName = "guid")
		{
			if (this._metrics != null)
			{
				Debug.LogWarning("Backtrace metrics support is already enabled. Please use BacktraceClient.Metrics.");
				return false;
			}
			this._metrics = new BacktraceMetrics(this.AttributeProvider, (long)((ulong)timeIntervalInSec), uniqueEventsSubmissionUrl, summedEventsSubmissionUrl)
			{
				StartupUniqueAttributeName = uniqueAttributeName,
				IgnoreSslValidation = this.Configuration.IgnoreSslValidation
			};
			this.StartupMetrics();
			return true;
		}

		private void StartupMetrics()
		{
			this.AttributeProvider.AddScopedAttributeProvider(this.Metrics);
			this._metrics.SendStartupEvent();
		}

		private void OnApplicationQuit()
		{
			if (this._nativeClient != null)
			{
				this._nativeClient.Disable();
			}
		}

		private void Awake()
		{
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.FromMonoBehavior("Application awake", LogType.Assert, null);
			}
			this.Refresh();
		}

		private void LateUpdate()
		{
			if (this._nativeClient != null)
			{
				this._nativeClient.Update(Time.unscaledTime);
			}
			if (this._metrics != null)
			{
				this._metrics.Tick(Time.unscaledTime);
			}
			if (this.BackgroundExceptions.Count == 0)
			{
				return;
			}
			while (this.BackgroundExceptions.Count > 0)
			{
				this.SendReport(this.BackgroundExceptions.Pop(), null);
			}
		}

		private void OnDestroy()
		{
			this.Enabled = false;
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.FromMonoBehavior("Backtrace Client: OnDestroy", LogType.Warning, null);
				this._breadcrumbs.UnregisterEvents();
			}
			BacktraceClient._instance = null;
			Application.logMessageReceived -= this.HandleUnityMessage;
			Application.logMessageReceivedThreaded -= this.HandleUnityBackgroundException;
			if (this._nativeClient != null)
			{
				this._nativeClient.Disable();
			}
		}

		public void SetClientReportLimit(uint reportPerMin)
		{
			if (!this.Enabled)
			{
				Debug.LogWarning("Please enable BacktraceClient first.");
				return;
			}
			this._reportLimitWatcher.SetClientReportLimit(reportPerMin);
		}

		public void Send(string message, List<string> attachmentPaths = null, Dictionary<string, string> attributes = null)
		{
			if (!this.ShouldSendReport(message, attachmentPaths, attributes))
			{
				return;
			}
			BacktraceReport report = new BacktraceReport(message, attributes, attachmentPaths);
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.FromBacktrace(report);
			}
			this._backtraceLogManager.Enqueue(report);
			this.SendReport(report, null);
		}

		public void Send(Exception exception, List<string> attachmentPaths = null, Dictionary<string, string> attributes = null)
		{
			if (!this.ShouldSendReport(exception, attachmentPaths, attributes, true))
			{
				return;
			}
			BacktraceReport report = new BacktraceReport(exception, attributes, attachmentPaths);
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.FromBacktrace(report);
			}
			this._backtraceLogManager.Enqueue(report);
			this.SendReport(report, null);
		}

		public void Send(BacktraceReport report, Action<BacktraceResult> sendCallback = null)
		{
			if (!this.ShouldSendReport(report))
			{
				return;
			}
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.FromBacktrace(report);
			}
			this._backtraceLogManager.Enqueue(report);
			this.SendReport(report, sendCallback);
		}

		private void SendReport(BacktraceReport report, Action<BacktraceResult> sendCallback = null)
		{
			if (this.BacktraceApi == null)
			{
				Debug.LogWarning("Backtrace API doesn't exist. Please validate client token or server url!");
				return;
			}
			if (!this.Enabled)
			{
				return;
			}
			base.StartCoroutine(this.CollectDataAndSend(report, sendCallback));
		}

		private IEnumerator CollectDataAndSend(BacktraceReport report, Action<BacktraceResult> sendCallback)
		{
			Dictionary<string, string> queryAttributes = new Dictionary<string, string>();
			Stopwatch stopWatch = this.EnablePerformanceStatistics ? Stopwatch.StartNew() : new Stopwatch();
			BacktraceData data = this.SetupBacktraceData(report);
			if (this.EnablePerformanceStatistics)
			{
				stopWatch.Stop();
				queryAttributes["performance.report"] = stopWatch.GetMicroseconds();
			}
			if (this.BeforeSend != null)
			{
				data = this.BeforeSend(data);
				if (data == null)
				{
					yield break;
				}
			}
			BacktraceDatabaseRecord record = null;
			if (this.Database != null && this.Database.Enabled())
			{
				yield return WaitForFrame.Wait();
				if (this.EnablePerformanceStatistics)
				{
					stopWatch.Restart();
				}
				record = this.Database.Add(data, true);
				if (record == null)
				{
					yield break;
				}
				data = record.BacktraceData;
				if (this.EnablePerformanceStatistics)
				{
					stopWatch.Stop();
					queryAttributes["performance.database"] = stopWatch.GetMicroseconds();
				}
				if (record.Duplicated)
				{
					record.Unlock();
					yield break;
				}
			}
			if (this.EnablePerformanceStatistics)
			{
				stopWatch.Restart();
			}
			string json = (record != null) ? record.BacktraceDataJson() : data.ToJson();
			if (this.EnablePerformanceStatistics)
			{
				stopWatch.Stop();
				queryAttributes["performance.json"] = stopWatch.GetMicroseconds();
			}
			yield return WaitForFrame.Wait();
			if (string.IsNullOrEmpty(json))
			{
				yield break;
			}
			if (this.RequestHandler != null)
			{
				yield return this.RequestHandler(this.BacktraceApi.ServerUrl, data);
				yield break;
			}
			if (data.Deduplication != 0)
			{
				queryAttributes["_mod_duplicate"] = data.Deduplication.ToString(CultureInfo.InvariantCulture);
			}
			base.StartCoroutine(this.BacktraceApi.Send(json, data.Attachments, queryAttributes, delegate(BacktraceResult result)
			{
				if (record != null)
				{
					record.Unlock();
					if (this.Database != null && result.Status != BacktraceResultStatus.ServerError && result.Status != BacktraceResultStatus.NetworkError)
					{
						this.Database.Delete(record);
					}
				}
				this.HandleInnerException(report);
				if (sendCallback != null)
				{
					sendCallback(result);
				}
			}));
			yield break;
		}

		private BacktraceData SetupBacktraceData(BacktraceReport report)
		{
			string text = this._backtraceLogManager.Disabled ? new BacktraceUnityMessage(report).ToString() : this._backtraceLogManager.ToSourceCode();
			report.AssignSourceCodeToReport(text);
			report.SetReportFingerprint(this.Configuration.UseNormalizedExceptionMessage);
			report.AttachmentPaths.AddRange(this._clientReportAttachments);
			BacktraceData backtraceData = report.ToBacktraceData(null, this.GameObjectDepth);
			this.AttributeProvider.AddAttributes(backtraceData.Attributes.Attributes, true);
			return backtraceData;
		}

		private void CaptureUnityMessages()
		{
			this._backtraceLogManager = new BacktraceLogManager(this.Configuration.NumberOfLogs);
			if (this.Configuration.HandleUnhandledExceptions)
			{
				Application.logMessageReceived += this.HandleUnityMessage;
				Application.logMessageReceivedThreaded += this.HandleUnityBackgroundException;
			}
		}

		internal void OnApplicationPause(bool pause)
		{
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.FromMonoBehavior("Application pause", LogType.Assert, new Dictionary<string, string>
				{
					{
						"paused",
						pause.ToString(CultureInfo.InvariantCulture).ToLower()
					}
				});
			}
			if (this._nativeClient != null)
			{
				this._nativeClient.PauseAnrThread(pause);
			}
		}

		internal void HandleUnityBackgroundException(string message, string stackTrace, LogType type)
		{
			if (Thread.CurrentThread == this._current)
			{
				return;
			}
			this.HandleUnityMessage(message, stackTrace, type);
		}

		internal void HandleUnityMessage(string message, string stackTrace, LogType type)
		{
			if (!this.Enabled)
			{
				return;
			}
			BacktraceUnityMessage unityMessage = new BacktraceUnityMessage(message, stackTrace, type);
			this._backtraceLogManager.Enqueue(unityMessage);
			if (!this.Configuration.HandleUnhandledExceptions)
			{
				return;
			}
			if (string.IsNullOrEmpty(message) || (type != LogType.Error && type != LogType.Exception))
			{
				return;
			}
			BacktraceUnhandledException ex = null;
			bool invokeSkipApi = true;
			if (type == LogType.Error)
			{
				if (this.Configuration.ReportFilterType.HasFlag(ReportFilterType.Error))
				{
					return;
				}
				if (this.SamplingShouldSkip())
				{
					if (this.SkipReport == null)
					{
						return;
					}
					ex = new BacktraceUnhandledException(message, stackTrace)
					{
						Type = type
					};
					if (this.ShouldSkipReport(ReportFilterType.Error, ex, string.Empty))
					{
						return;
					}
					invokeSkipApi = false;
				}
			}
			if (ex == null)
			{
				ex = new BacktraceUnhandledException(message, stackTrace)
				{
					Type = type
				};
			}
			BacktraceReport report = new BacktraceReport(ex, null, null);
			this.SendUnhandledExceptionReport(report, invokeSkipApi);
		}

		private bool SamplingShouldSkip()
		{
			return this.Configuration && this.Configuration.Sampling != 1.0 && this.Random.NextDouble() > this.Configuration.Sampling;
		}

		private void SendUnhandledExceptionReport(BacktraceReport report, bool invokeSkipApi = true)
		{
			if (this.OnUnhandledApplicationException != null)
			{
				this.OnUnhandledApplicationException(report.Exception);
			}
			if (this.ShouldSendReport(report.Exception, null, null, invokeSkipApi))
			{
				this.SendReport(report, null);
			}
		}

		private bool ShouldSendReport(Exception exception, List<string> attachmentPaths, Dictionary<string, string> attributes, bool invokeSkipApi = true)
		{
			ReportFilterType type = ReportFilterType.Exception;
			if (exception is BacktraceUnhandledException)
			{
				BacktraceUnhandledException ex = exception as BacktraceUnhandledException;
				type = ((ex.Classifier == "ANRException") ? ReportFilterType.Hang : ((ex.Type == LogType.Exception) ? ReportFilterType.UnhandledException : ReportFilterType.Error));
			}
			if (invokeSkipApi && this.ShouldSkipReport(type, exception, string.Empty))
			{
				return false;
			}
			if (!this._reportLimitWatcher.WatchReport((long)DateTimeHelper.Timestamp(), true))
			{
				if (this.OnClientReportLimitReached != null)
				{
					BacktraceReport obj = new BacktraceReport(exception, attributes, attachmentPaths);
					this._onClientReportLimitReached(obj);
				}
				return false;
			}
			if (Thread.CurrentThread.ManagedThreadId != this._current.ManagedThreadId)
			{
				BacktraceReport backtraceReport = new BacktraceReport(exception, attributes, attachmentPaths);
				backtraceReport.Attributes["exception.thread"] = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
				this.BackgroundExceptions.Push(backtraceReport);
				return false;
			}
			return true;
		}

		private bool ShouldSendReport(string message, List<string> attachmentPaths, Dictionary<string, string> attributes)
		{
			if (this.ShouldSkipReport(ReportFilterType.Message, null, message))
			{
				return false;
			}
			if (!this._reportLimitWatcher.WatchReport((long)DateTimeHelper.Timestamp(), true))
			{
				if (this.OnClientReportLimitReached != null)
				{
					BacktraceReport obj = new BacktraceReport(message, attributes, attachmentPaths);
					this._onClientReportLimitReached(obj);
				}
				return false;
			}
			if (Thread.CurrentThread.ManagedThreadId != this._current.ManagedThreadId)
			{
				BacktraceReport backtraceReport = new BacktraceReport(message, attributes, attachmentPaths);
				backtraceReport.Attributes["exception.thread"] = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
				this.BackgroundExceptions.Push(backtraceReport);
				return false;
			}
			return true;
		}

		private bool ShouldSendReport(BacktraceReport report)
		{
			if (this.ShouldSkipReport(report.ExceptionTypeReport ? ReportFilterType.Exception : ReportFilterType.Message, report.Exception, report.Message))
			{
				return false;
			}
			if (!this._reportLimitWatcher.WatchReport((long)DateTimeHelper.Timestamp(), true))
			{
				if (this.OnClientReportLimitReached != null)
				{
					this._onClientReportLimitReached(report);
				}
				return false;
			}
			if (Thread.CurrentThread.ManagedThreadId != this._current.ManagedThreadId)
			{
				report.Attributes["exception.thread"] = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
				this.BackgroundExceptions.Push(report);
				return false;
			}
			return true;
		}

		private void HandleInnerException(BacktraceReport report)
		{
			BacktraceReport backtraceReport = report.CreateInnerReport();
			if (backtraceReport != null && this.ShouldSendReport(backtraceReport))
			{
				this.SendReport(backtraceReport, null);
			}
		}

		private bool ValidClientConfiguration()
		{
			bool flag = this.BacktraceApi == null || !this.Enabled;
			if (flag)
			{
				Debug.LogWarning("Cannot set method if configuration contain invalid url to Backtrace server or client is disabled");
			}
			return !flag;
		}

		private bool ShouldSkipReport(ReportFilterType type, Exception exception, string message)
		{
			return this.Enabled && (this.Configuration.ReportFilterType.HasFlag(type) || (this.SkipReport != null && this.SkipReport(type, exception, message)));
		}

		internal IList<string> GetNativeAttachments()
		{
			return (from n in this._clientReportAttachments
			where !string.IsNullOrEmpty(n)
			select n).OrderBy(new Func<string, string>(Path.GetFileName), StringComparer.InvariantCultureIgnoreCase).ToList<string>();
		}

		public const string VERSION = "3.9.1";

		internal const string DefaultBacktraceGameObjectName = "BacktraceClient";

		public BacktraceConfiguration Configuration;

		private BacktraceBreadcrumbs _breadcrumbs;

		private AttributeProvider _attributeProvider;

		private BacktraceMetrics _metrics;

		private Random _random;

		internal Stack<BacktraceReport> BackgroundExceptions = new Stack<BacktraceReport>();

		private HashSet<string> _clientReportAttachments;

		private static BacktraceClient _instance;

		public IBacktraceDatabase Database;

		private IBacktraceApi _backtraceApi;

		private ReportLimitWatcher _reportLimitWatcher;

		private BacktraceLogManager _backtraceLogManager;

		internal Action<BacktraceReport> _onClientReportLimitReached;

		public Func<BacktraceData, BacktraceData> BeforeSend;

		public Func<ReportFilterType, Exception, string, bool> SkipReport;

		public Action<Exception> OnUnhandledApplicationException;

		private INativeClient _nativeClient;

		private Thread _current;
	}
}
