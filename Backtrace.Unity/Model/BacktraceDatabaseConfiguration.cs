using System;
using System.IO;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Model
{
	[Serializable]
	public class BacktraceDatabaseConfiguration : BacktraceClientConfiguration
	{
		public bool ValidDatabasePath()
		{
			if (string.IsNullOrEmpty(this.DatabasePath))
			{
				return false;
			}
			string text = this.DatabasePath;
			if (!Path.IsPathRooted(text))
			{
				text = Path.GetFullPath(Path.Combine(Application.dataPath, text));
			}
			this.Enabled = Directory.Exists(text);
			return true;
		}

		public bool Enabled;

		public string DatabasePath;

		public bool AutoSendMode = true;

		public bool CreateDatabase;

		public bool GenerateScreenshotOnException;

		public DeduplicationStrategy DeduplicationStrategy;

		public int MaxRecordCount;

		public long MaxDatabaseSize;

		public int RetryInterval = 60;

		public int RetryLimit = 3;

		public RetryOrder RetryOrder;
	}
}
