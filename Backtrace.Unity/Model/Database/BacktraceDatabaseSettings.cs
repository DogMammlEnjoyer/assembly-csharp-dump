using System;
using Backtrace.Unity.Types;

namespace Backtrace.Unity.Model.Database
{
	public class BacktraceDatabaseSettings
	{
		public BacktraceDatabaseSettings(string databasePath, BacktraceConfiguration configuration)
		{
			if (configuration == null || string.IsNullOrEmpty(databasePath))
			{
				return;
			}
			this.DatabasePath = databasePath;
			this._configuration = configuration;
			this._retryInterval = (uint)((configuration.RetryInterval > 0) ? this._configuration.RetryInterval : 60);
		}

		public string DatabasePath { get; private set; }

		public uint MaxRecordCount
		{
			get
			{
				return Convert.ToUInt32(this._configuration.MaxRecordCount);
			}
		}

		public long MaxDatabaseSize
		{
			get
			{
				return this._configuration.MaxDatabaseSize * 1000L * 1000L;
			}
		}

		public bool AutoSendMode
		{
			get
			{
				return this._configuration.AutoSendMode;
			}
		}

		public uint RetryInterval
		{
			get
			{
				return this._retryInterval;
			}
		}

		public uint RetryLimit
		{
			get
			{
				return Convert.ToUInt32(this._configuration.RetryLimit);
			}
		}

		public DeduplicationStrategy DeduplicationStrategy
		{
			get
			{
				return this._configuration.DeduplicationStrategy;
			}
		}

		public bool GenerateScreenshotOnException
		{
			get
			{
				return this._configuration.GenerateScreenshotOnException;
			}
		}

		public bool AddUnityLogToReport
		{
			get
			{
				return this._configuration.AddUnityLogToReport;
			}
		}

		public RetryOrder RetryOrder
		{
			get
			{
				return this._configuration.RetryOrder;
			}
		}

		public MiniDumpType MinidumpType
		{
			get
			{
				return this._configuration.MinidumpType;
			}
		}

		private readonly BacktraceConfiguration _configuration;

		private readonly uint _retryInterval;
	}
}
