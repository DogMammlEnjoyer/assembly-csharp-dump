using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Backtrace.Unity.Model.Database
{
	public class BacktraceDatabaseRecord
	{
		internal string RecordPath { get; set; }

		internal string DiagnosticDataPath { get; set; }

		internal long Size { get; set; }

		internal BacktraceData Record { get; set; }

		public ICollection<string> Attachments { get; private set; }

		internal string DiagnosticDataJson { get; set; }

		public bool Duplicated
		{
			get
			{
				return this._count != 1;
			}
		}

		public int Count
		{
			get
			{
				return this._count;
			}
		}

		public string BacktraceDataJson()
		{
			if (!string.IsNullOrEmpty(this.DiagnosticDataJson))
			{
				return this.DiagnosticDataJson;
			}
			if (this.Record != null)
			{
				return this.Record.ToJson();
			}
			if (string.IsNullOrEmpty(this.DiagnosticDataPath) || !File.Exists(this.DiagnosticDataPath))
			{
				return null;
			}
			this.DiagnosticDataJson = File.ReadAllText(this.DiagnosticDataPath);
			return this.DiagnosticDataJson;
		}

		public BacktraceData BacktraceData
		{
			get
			{
				if (this.Record != null)
				{
					this.Record.Deduplication = this.Count;
					return this.Record;
				}
				return null;
			}
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(new BacktraceDatabaseRecord.BacktraceDatabaseRawRecord
			{
				Id = this.Id.ToString(),
				recordName = this.RecordPath,
				dataPath = this.DiagnosticDataPath,
				size = this.Size,
				hash = this.Hash,
				attachments = new List<string>(this.Attachments)
			}, false);
		}

		public static BacktraceDatabaseRecord Deserialize(string json)
		{
			return new BacktraceDatabaseRecord(JsonUtility.FromJson<BacktraceDatabaseRecord.BacktraceDatabaseRawRecord>(json));
		}

		private BacktraceDatabaseRecord(BacktraceDatabaseRecord.BacktraceDatabaseRawRecord rawRecord)
		{
			this.Id = new Guid(rawRecord.Id);
			this.RecordPath = rawRecord.recordName;
			this.DiagnosticDataPath = rawRecord.dataPath;
			this.Size = rawRecord.size;
			this.Hash = rawRecord.hash;
			this.Attachments = rawRecord.attachments;
		}

		public BacktraceDatabaseRecord(BacktraceData data)
		{
			this.Id = data.Uuid;
			this.Record = data;
			this.Attachments = data.Attachments;
		}

		public virtual void Increment()
		{
			this._count++;
		}

		internal static BacktraceDatabaseRecord ReadFromFile(FileInfo file)
		{
			if (!file.Exists)
			{
				return null;
			}
			BacktraceDatabaseRecord result;
			using (StreamReader streamReader = file.OpenText())
			{
				string json = streamReader.ReadToEnd();
				try
				{
					result = BacktraceDatabaseRecord.Deserialize(json);
				}
				catch (Exception)
				{
					result = null;
				}
			}
			return result;
		}

		public virtual void Unlock()
		{
			this.Locked = false;
			this.Record = null;
		}

		public Guid Id = Guid.NewGuid();

		internal bool Locked;

		public string Hash = string.Empty;

		private int _count = 1;

		[Serializable]
		private struct BacktraceDatabaseRawRecord
		{
			public string Id;

			public string recordName;

			public string dataPath;

			public long size;

			public string hash;

			public List<string> attachments;
		}
	}
}
