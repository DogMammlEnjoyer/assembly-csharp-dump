using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Backtrace.Unity.Common;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Breadcrumbs;
using Backtrace.Unity.Model.Breadcrumbs.Storage;
using Backtrace.Unity.Model.Database;
using Backtrace.Unity.Runtime.Native;
using Backtrace.Unity.Services;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity
{
	[RequireComponent(typeof(BacktraceClient))]
	public class BacktraceDatabase : MonoBehaviour, IBacktraceDatabase
	{
		public IBacktraceBreadcrumbs Breadcrumbs
		{
			get
			{
				if (this._breadcrumbs == null && this.Enable && this.Configuration.EnableBreadcrumbsSupport && BacktraceBreadcrumbs.CanStoreBreadcrumbs(this.Configuration.LogLevel, this.Configuration.BacktraceBreadcrumbsLevel))
				{
					this._breadcrumbs = new BacktraceBreadcrumbs(new BacktraceStorageLogManager(this.Configuration.GetFullDatabasePath()), this.Configuration.BacktraceBreadcrumbsLevel, this.Configuration.LogLevel);
				}
				return this._breadcrumbs;
			}
		}

		public string DatabasePath { get; protected set; }

		public int ScreenshotQuality
		{
			get
			{
				return this.BacktraceDatabaseFileContext.ScreenshotQuality;
			}
			set
			{
				this.BacktraceDatabaseFileContext.ScreenshotQuality = value;
			}
		}

		public int ScreenshotMaxHeight
		{
			get
			{
				return this.BacktraceDatabaseFileContext.ScreenshotMaxHeight;
			}
			set
			{
				this.BacktraceDatabaseFileContext.ScreenshotMaxHeight = value;
			}
		}

		public static BacktraceDatabase Instance
		{
			get
			{
				return BacktraceDatabase._instance;
			}
		}

		public DeduplicationStrategy DeduplicationStrategy
		{
			get
			{
				if (this.BacktraceDatabaseContext == null)
				{
					return DeduplicationStrategy.None;
				}
				return this.BacktraceDatabaseContext.DeduplicationStrategy;
			}
			set
			{
				if (!this.Enable)
				{
					throw new InvalidOperationException("Backtrace Database is disabled");
				}
				this.BacktraceDatabaseContext.DeduplicationStrategy = value;
			}
		}

		protected BacktraceDatabaseSettings DatabaseSettings { get; set; }

		public IBacktraceApi BacktraceApi { get; set; }

		protected virtual IBacktraceDatabaseContext BacktraceDatabaseContext { get; set; }

		internal IBacktraceDatabaseFileContext BacktraceDatabaseFileContext { get; set; }

		public bool Enable { get; private set; }

		public void Reload()
		{
			if (this.Configuration == null)
			{
				this._client = base.GetComponent<BacktraceClient>();
				this.Configuration = this._client.Configuration;
			}
			if (BacktraceDatabase.Instance != null)
			{
				return;
			}
			if (this.Configuration == null || !this.Configuration.IsValid())
			{
				this.Enable = false;
				return;
			}
			this.Enable = (this.Configuration.Enabled && this.InitializeDatabasePaths());
			if (!this.Enable)
			{
				if (this.Configuration.Enabled)
				{
					Debug.LogWarning("Cannot initialize database - invalid path to database. Database is disabled");
				}
				return;
			}
			this.DatabaseSettings = new BacktraceDatabaseSettings(this.DatabasePath, this.Configuration);
			this.SetupMultisceneSupport();
			this._lastConnection = Time.unscaledTime;
			BacktraceDatabase.LastFrameTime = Time.unscaledTime;
			this.BacktraceDatabaseContext = new BacktraceDatabaseContext(this.DatabaseSettings);
			this.BacktraceDatabaseFileContext = new BacktraceDatabaseFileContext(this.DatabaseSettings);
			this.BacktraceApi = new BacktraceApi(this.Configuration.ToCredentials(), false);
			this._reportLimitWatcher = new ReportLimitWatcher(Convert.ToUInt32(this.Configuration.ReportPerMin));
		}

		public void OnDisable()
		{
			this.Enable = false;
		}

		private void Awake()
		{
			this.Reload();
		}

		internal void Update()
		{
			if (!this.Enable)
			{
				return;
			}
			if (this._breadcrumbs != null)
			{
				this._breadcrumbs.Update();
			}
			BacktraceDatabase.LastFrameTime = Time.unscaledTime;
			if (!this.DatabaseSettings.AutoSendMode)
			{
				return;
			}
			if (Time.unscaledTime - this._lastConnection > this.DatabaseSettings.RetryInterval)
			{
				this._lastConnection = Time.unscaledTime;
				if (this._timerBackgroundWork || !this.BacktraceDatabaseContext.Any())
				{
					return;
				}
				this._timerBackgroundWork = true;
				this.SendData(this.BacktraceDatabaseContext.FirstOrDefault());
				this._timerBackgroundWork = false;
			}
		}

		private void Start()
		{
			if (!this.Enable)
			{
				return;
			}
			string breadcrumbPath = string.Empty;
			string text = string.Empty;
			if (this.Breadcrumbs != null)
			{
				breadcrumbPath = this.Breadcrumbs.GetBreadcrumbLogPath();
				text = this.Breadcrumbs.Archive();
			}
			this.LoadReports(breadcrumbPath, text);
			this.RemoveOrphaned();
			bool flag = this.Enable && this.Configuration.SendUnhandledGameCrashesOnGameStartup && base.isActiveAndEnabled;
			bool flag2 = this._client && this._client.NativeClient != null && this._client.NativeClient is IStartupMinidumpSender;
			if (flag && flag2)
			{
				IStartupMinidumpSender startupMinidumpSender = this._client.NativeClient as IStartupMinidumpSender;
				IList<string> nativeAttachments = this._client.GetNativeAttachments();
				if (!string.IsNullOrEmpty(text))
				{
					nativeAttachments.Add(text);
				}
				base.StartCoroutine(startupMinidumpSender.SendMinidumpOnStartup(nativeAttachments, this.BacktraceApi));
			}
			this.EnableBreadcrumbsSupport();
			if (this.DatabaseSettings.AutoSendMode)
			{
				this._lastConnection = Time.unscaledTime;
				this.SendData(this.BacktraceDatabaseContext.FirstOrDefault());
			}
		}

		public void SetApi(IBacktraceApi backtraceApi)
		{
			this.BacktraceApi = backtraceApi;
		}

		public bool Enabled()
		{
			return this.Enable;
		}

		public BacktraceDatabaseSettings GetSettings()
		{
			return this.DatabaseSettings;
		}

		public void Clear()
		{
			if (this.BacktraceDatabaseContext != null)
			{
				this.BacktraceDatabaseContext.Clear();
			}
			if (this.BacktraceDatabaseContext != null)
			{
				this.BacktraceDatabaseFileContext.Clear();
			}
		}

		public BacktraceDatabaseRecord Add(BacktraceData data, bool @lock = true)
		{
			if (data == null || !this.Enable)
			{
				return null;
			}
			if (!this.ValidateDatabaseSize())
			{
				return null;
			}
			string hash = this.BacktraceDatabaseContext.GetHash(data);
			if (!string.IsNullOrEmpty(hash))
			{
				BacktraceDatabaseRecord recordByHash = this.BacktraceDatabaseContext.GetRecordByHash(hash);
				if (recordByHash != null)
				{
					this.BacktraceDatabaseContext.AddDuplicate(recordByHash);
					return recordByHash;
				}
			}
			IEnumerable<string> source = this.BacktraceDatabaseFileContext.GenerateRecordAttachments(data);
			for (int i = 0; i < source.Count<string>(); i++)
			{
				if (!string.IsNullOrEmpty(source.ElementAt(i)))
				{
					data.Attachments.Add(source.ElementAt(i));
				}
			}
			if (this.Breadcrumbs != null)
			{
				data.Attachments.Add(this.Breadcrumbs.GetBreadcrumbLogPath());
				data.Attributes.Attributes["breadcrumbs.lastId"] = this.Breadcrumbs.BreadcrumbId().ToString("F0", CultureInfo.InvariantCulture);
			}
			BacktraceDatabaseRecord backtraceDatabaseRecord = new BacktraceDatabaseRecord(data)
			{
				Hash = hash
			};
			if (!this.BacktraceDatabaseFileContext.Save(backtraceDatabaseRecord))
			{
				this.BacktraceDatabaseFileContext.Delete(backtraceDatabaseRecord);
				return null;
			}
			this.BacktraceDatabaseContext.Add(backtraceDatabaseRecord);
			if (!@lock)
			{
				backtraceDatabaseRecord.Unlock();
			}
			return backtraceDatabaseRecord;
		}

		[Obsolete("Please use Add method with Backtrace data parameter instead")]
		public BacktraceDatabaseRecord Add(BacktraceReport backtraceReport, Dictionary<string, string> attributes, MiniDumpType miniDumpType = MiniDumpType.None)
		{
			if (!this.Enable || backtraceReport == null)
			{
				return null;
			}
			BacktraceData data = backtraceReport.ToBacktraceData(attributes, this.Configuration.GameObjectDepth);
			return this.Add(data, true);
		}

		public IEnumerable<BacktraceDatabaseRecord> Get()
		{
			if (this.BacktraceDatabaseContext != null)
			{
				return this.BacktraceDatabaseContext.Get();
			}
			return new List<BacktraceDatabaseRecord>();
		}

		public void Delete(BacktraceDatabaseRecord record)
		{
			if (this.BacktraceDatabaseContext != null)
			{
				this.BacktraceDatabaseContext.Delete(record);
			}
			if (this.BacktraceDatabaseFileContext != null)
			{
				this.BacktraceDatabaseFileContext.Delete(record);
			}
		}

		public void Flush()
		{
			if (!this.Enable || !this.BacktraceDatabaseContext.Any())
			{
				return;
			}
			this.FlushRecord(this.BacktraceDatabaseContext.FirstOrDefault());
		}

		public void Send()
		{
			if (!this.Enable || !this.BacktraceDatabaseContext.Any())
			{
				return;
			}
			this.SendData(this.BacktraceDatabaseContext.FirstOrDefault());
		}

		private void FlushRecord(BacktraceDatabaseRecord record)
		{
			if (record == null)
			{
				return;
			}
			Stopwatch stopwatch = this.Configuration.PerformanceStatistics ? Stopwatch.StartNew() : null;
			string text = record.BacktraceDataJson();
			this.Delete(record);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (this.Configuration.PerformanceStatistics)
			{
				stopwatch.Stop();
				dictionary["performance.database.flush"] = stopwatch.GetMicroseconds();
			}
			if (text == null)
			{
				return;
			}
			dictionary["_mod_duplicate"] = record.Count.ToString(CultureInfo.InvariantCulture);
			base.StartCoroutine(this.BacktraceApi.Send(text, record.Attachments, dictionary, delegate(BacktraceResult result)
			{
				record = this.BacktraceDatabaseContext.FirstOrDefault();
				this.FlushRecord(record);
			}));
		}

		private void SendData(BacktraceDatabaseRecord record)
		{
			if (record == null)
			{
				return;
			}
			Stopwatch stopwatch = this.Configuration.PerformanceStatistics ? Stopwatch.StartNew() : null;
			string text = (record != null) ? record.BacktraceDataJson() : null;
			if (string.IsNullOrEmpty(text))
			{
				this.Delete(record);
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (this.Configuration.PerformanceStatistics)
			{
				stopwatch.Stop();
				dictionary["performance.database.send"] = stopwatch.GetMicroseconds();
			}
			dictionary["_mod_duplicate"] = record.Count.ToString(CultureInfo.InvariantCulture);
			base.StartCoroutine(this.BacktraceApi.Send(text, record.Attachments, dictionary, delegate(BacktraceResult sendResult)
			{
				record.Unlock();
				if (sendResult.Status == BacktraceResultStatus.ServerError || sendResult.Status == BacktraceResultStatus.NetworkError)
				{
					this.IncrementBatchRetry();
					return;
				}
				this.Delete(record);
				if (!this._reportLimitWatcher.WatchReport((long)DateTimeHelper.Timestamp(), true))
				{
					return;
				}
				record = this.BacktraceDatabaseContext.FirstOrDefault();
				this.SendData(record);
			}));
		}

		public int Count()
		{
			return this.BacktraceDatabaseContext.Count();
		}

		protected virtual void RemoveOrphaned()
		{
			IEnumerable<BacktraceDatabaseRecord> existingRecords = this.BacktraceDatabaseContext.Get();
			this.BacktraceDatabaseFileContext.RemoveOrphaned(existingRecords);
		}

		protected virtual void SetupMultisceneSupport()
		{
			if (this.Configuration.DestroyOnLoad)
			{
				return;
			}
			Object.DontDestroyOnLoad(base.gameObject);
			BacktraceDatabase._instance = this;
		}

		protected virtual bool InitializeDatabasePaths()
		{
			if (!this.Configuration.Enabled)
			{
				return false;
			}
			this.DatabasePath = this.Configuration.GetFullDatabasePath();
			if (string.IsNullOrEmpty(this.DatabasePath))
			{
				Debug.LogWarning("Backtrace database path is empty or unavailable.");
				return false;
			}
			bool flag = Directory.Exists(this.DatabasePath);
			if (!flag && this.Configuration.CreateDatabase)
			{
				try
				{
					flag = Directory.CreateDirectory(this.DatabasePath).Exists;
				}
				catch (Exception)
				{
					return false;
				}
			}
			if (!flag)
			{
				Debug.LogWarning(string.Format("Backtrace database path doesn't exist. Database path: {0}", this.DatabasePath));
			}
			return flag;
		}

		protected virtual void LoadReports(string breadcrumbPath, string breadcrumbArchive)
		{
			if (!this.Enable)
			{
				return;
			}
			FileInfo[] array = this.BacktraceDatabaseFileContext.GetRecords().ToArray<FileInfo>();
			if (array.Length == 0)
			{
				return;
			}
			bool flag = !string.IsNullOrEmpty(breadcrumbArchive);
			FileInfo[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				BacktraceDatabaseRecord backtraceDatabaseRecord = BacktraceDatabaseRecord.ReadFromFile(array2[i]);
				if (backtraceDatabaseRecord != null)
				{
					if (!this.BacktraceDatabaseFileContext.IsValidRecord(backtraceDatabaseRecord))
					{
						this.BacktraceDatabaseFileContext.Delete(backtraceDatabaseRecord);
					}
					else
					{
						if (flag && backtraceDatabaseRecord.Attachments.Remove(breadcrumbPath))
						{
							backtraceDatabaseRecord.Attachments.Add(breadcrumbArchive);
						}
						this.BacktraceDatabaseContext.Add(backtraceDatabaseRecord);
						this.ValidateDatabaseSize();
						backtraceDatabaseRecord.Unlock();
					}
				}
			}
		}

		private bool ValidateDatabaseSize()
		{
			bool flag = this.ReachedMaximumNumberOfRecords();
			bool flag2 = this.ReachedDiskSpaceLimit();
			if (flag || flag2)
			{
				int num = 5;
				while (this.ReachedDiskSpaceLimit() || this.ReachedMaximumNumberOfRecords())
				{
					BacktraceDatabaseRecord backtraceDatabaseRecord = this.BacktraceDatabaseContext.LastOrDefault();
					if (backtraceDatabaseRecord != null)
					{
						this.BacktraceDatabaseContext.Delete(backtraceDatabaseRecord);
						this.BacktraceDatabaseFileContext.Delete(backtraceDatabaseRecord);
					}
					num--;
					if (num == 0)
					{
						break;
					}
				}
				return num != 0;
			}
			return true;
		}

		private bool ReachedDiskSpaceLimit()
		{
			return this.DatabaseSettings.MaxDatabaseSize != 0L && this.BacktraceDatabaseContext.GetSize() > this.DatabaseSettings.MaxDatabaseSize;
		}

		private bool ReachedMaximumNumberOfRecords()
		{
			return (long)(this.BacktraceDatabaseContext.Count() + 1) > (long)((ulong)this.DatabaseSettings.MaxRecordCount) && this.DatabaseSettings.MaxRecordCount > 0U;
		}

		public bool ValidConsistency()
		{
			return this.BacktraceDatabaseFileContext.ValidFileConsistency();
		}

		public long GetDatabaseSize()
		{
			return this.BacktraceDatabaseContext.GetSize();
		}

		public void SetReportWatcher(ReportLimitWatcher reportLimitWatcher)
		{
			this._reportLimitWatcher = reportLimitWatcher;
		}

		private void IncrementBatchRetry()
		{
			IEnumerable<BacktraceDatabaseRecord> recordsToDelete = this.BacktraceDatabaseContext.GetRecordsToDelete();
			this.BacktraceDatabaseContext.IncrementBatchRetry();
			if (recordsToDelete != null && recordsToDelete.Count<BacktraceDatabaseRecord>() != 0)
			{
				foreach (BacktraceDatabaseRecord record in recordsToDelete)
				{
					this.BacktraceDatabaseFileContext.Delete(record);
				}
			}
		}

		internal string GetBreadcrumbsPath()
		{
			if (this._breadcrumbs == null)
			{
				return string.Empty;
			}
			return this._breadcrumbs.GetBreadcrumbLogPath();
		}

		public bool EnableBreadcrumbsSupport()
		{
			return this.Breadcrumbs != null && this._breadcrumbs.EnableBreadcrumbs();
		}

		private bool _timerBackgroundWork;

		public BacktraceConfiguration Configuration;

		private BacktraceBreadcrumbs _breadcrumbs;

		private BacktraceClient _client;

		internal static float LastFrameTime;

		private static BacktraceDatabase _instance;

		private float _lastConnection;

		private ReportLimitWatcher _reportLimitWatcher;
	}
}
