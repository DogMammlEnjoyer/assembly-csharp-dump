using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Backtrace.Unity.Common;
using Backtrace.Unity.Json;

namespace Backtrace.Unity.Model.Breadcrumbs.Storage
{
	internal sealed class BacktraceStorageLogManager : IBacktraceLogManager, IArchiveableBreadcrumbManager
	{
		public string BreadcrumbsFilePath { get; private set; }

		public long BreadcrumbsSize
		{
			get
			{
				return this._breadcrumbsSize;
			}
			set
			{
				if (value < 10000L)
				{
					throw new ArgumentException("Breadcrumbs size must be greater or equal to 10kB");
				}
				this._breadcrumbsSize = value;
			}
		}

		internal IBreadcrumbFile BreadcrumbFile { get; set; }

		public BacktraceStorageLogManager(string storagePath)
		{
			if (string.IsNullOrEmpty(storagePath))
			{
				throw new ArgumentException("Breadcrumbs storage path is null or empty");
			}
			this._storagePath = storagePath;
			this.BreadcrumbsFilePath = Path.Combine(this._storagePath, "bt-breadcrumbs-0");
			this.BreadcrumbFile = new BreadcrumbFile(this.BreadcrumbsFilePath);
		}

		public bool Enable()
		{
			if (this.currentSize != 0L)
			{
				return true;
			}
			try
			{
				if (this.BreadcrumbFile.Exists())
				{
					this.BreadcrumbFile.Delete();
				}
				using (Stream createStream = this.BreadcrumbFile.GetCreateStream())
				{
					createStream.Write(BacktraceStorageLogManager.StartOfDocument, 0, BacktraceStorageLogManager.StartOfDocument.Length);
					createStream.Write(BacktraceStorageLogManager.EndOfDocument, 0, BacktraceStorageLogManager.EndOfDocument.Length);
				}
				this._emptyFile = true;
				this.currentSize = (long)(BacktraceStorageLogManager.StartOfDocument.Length + BacktraceStorageLogManager.EndOfDocument.Length);
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		public bool Add(string message, BreadcrumbLevel level, UnityEngineLogLevel type, IDictionary<string, string> attributes)
		{
			if (this.currentSize == 0L)
			{
				return false;
			}
			object lockObject = this._lockObject;
			byte[] bytes;
			lock (lockObject)
			{
				double breadcrumbId = this._breadcrumbId;
				this._breadcrumbId = breadcrumbId + 1.0;
				double id = breadcrumbId;
				BacktraceJObject backtraceJObject = this.CreateBreadcrumbJson(id, message, level, type, attributes);
				bytes = Encoding.UTF8.GetBytes(backtraceJObject.ToJson());
				if (this.currentSize + (long)bytes.Length > this.BreadcrumbsSize)
				{
					try
					{
						this.ClearOldLogs();
					}
					catch (Exception)
					{
						return false;
					}
				}
			}
			bool result;
			try
			{
				result = this.AppendBreadcrumb(bytes);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private BacktraceJObject CreateBreadcrumbJson(double id, string message, BreadcrumbLevel level, UnityEngineLogLevel type, IDictionary<string, string> attributes)
		{
			BacktraceJObject backtraceJObject = new BacktraceJObject();
			backtraceJObject.Add("timestamp", DateTimeHelper.TimestampMs(), "F0");
			backtraceJObject.Add("id", id, "F0");
			backtraceJObject.Add("type", Enum.GetName(typeof(BreadcrumbLevel), level).ToLower());
			backtraceJObject.Add("level", Enum.GetName(typeof(UnityEngineLogLevel), type).ToLower());
			backtraceJObject.Add("message", message);
			if (attributes != null && attributes.Count > 0)
			{
				backtraceJObject.Add("attributes", new BacktraceJObject(attributes));
			}
			return backtraceJObject;
		}

		private bool AppendBreadcrumb(byte[] bytes)
		{
			long num = (long)(BacktraceStorageLogManager.EndOfDocument.Length * -1);
			using (Stream writeStream = this.BreadcrumbFile.GetWriteStream())
			{
				writeStream.Position = writeStream.Length - (long)BacktraceStorageLogManager.EndOfDocument.Length;
				if (!this._emptyFile)
				{
					writeStream.Write(BacktraceStorageLogManager.NewRow, 0, BacktraceStorageLogManager.NewRow.Length);
					num += (long)BacktraceStorageLogManager.NewRow.Length;
				}
				else
				{
					this._emptyFile = false;
				}
				writeStream.Write(bytes, 0, bytes.Length);
				writeStream.Write(BacktraceStorageLogManager.EndOfDocument, 0, BacktraceStorageLogManager.EndOfDocument.Length);
				num += (long)(bytes.Length + BacktraceStorageLogManager.EndOfDocument.Length);
			}
			this.currentSize += num;
			this._logSize.Enqueue((long)bytes.Length);
			return true;
		}

		private void ClearOldLogs()
		{
			long nextStartPosition = this.GetNextStartPosition();
			using (Stream iostream = this.BreadcrumbFile.GetIOStream())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					long num = iostream.Length - nextStartPosition;
					iostream.Seek(num * -1L, SeekOrigin.End);
					iostream.CopyTo(memoryStream);
					iostream.SetLength(num + (long)BacktraceStorageLogManager.StartOfDocument.Length);
					memoryStream.Position = 0L;
					iostream.Position = 0L;
					iostream.Write(BacktraceStorageLogManager.StartOfDocument, 0, BacktraceStorageLogManager.StartOfDocument.Length);
					memoryStream.CopyTo(iostream);
				}
			}
			this.currentSize -= nextStartPosition;
			this.currentSize += (long)BacktraceStorageLogManager.StartOfDocument.Length;
		}

		private long GetNextStartPosition()
		{
			double num = (double)this.BreadcrumbsSize - (double)this.BreadcrumbsSize * 0.7;
			long num2 = (long)BacktraceStorageLogManager.StartOfDocument.Length;
			int num3 = BacktraceStorageLogManager.NewRow.Length;
			while ((double)num2 < num)
			{
				if (this._logSize.Count == 0)
				{
					return num2;
				}
				num2 += this._logSize.Dequeue() + (long)num3;
			}
			return num2;
		}

		public bool Clear()
		{
			bool result;
			try
			{
				this.currentSize = 0L;
				this._logSize.Clear();
				this.BreadcrumbFile.Delete();
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public int Length()
		{
			return this._logSize.Count;
		}

		public double BreadcrumbId()
		{
			return this._breadcrumbId;
		}

		public string Archive()
		{
			if (!File.Exists(this.BreadcrumbsFilePath))
			{
				return string.Empty;
			}
			string text = Path.Combine(this._storagePath, string.Format("{0}-1", "bt-breadcrumbs"));
			File.Copy(this.BreadcrumbsFilePath, text, true);
			return text;
		}

		public const int MinimumBreadcrumbsFileSize = 10000;

		private long _breadcrumbsSize = 64000L;

		internal const string BreadcrumbLogFilePrefix = "bt-breadcrumbs";

		internal const string BreadcrumbLogFileName = "bt-breadcrumbs-0";

		internal static byte[] NewRow = Encoding.UTF8.GetBytes(",\n");

		internal static byte[] EndOfDocument = Encoding.UTF8.GetBytes("\n]");

		internal static byte[] StartOfDocument = Encoding.UTF8.GetBytes("[\n");

		private bool _emptyFile = true;

		private double _breadcrumbId = DateTimeHelper.TimestampMs();

		private object _lockObject = new object();

		private long currentSize;

		private readonly Queue<long> _logSize = new Queue<long>();

		private readonly string _storagePath;
	}
}
