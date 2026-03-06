using System;
using System.Collections.Generic;
using System.Linq;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Database;
using Backtrace.Unity.Types;

namespace Backtrace.Unity.Services
{
	internal class BacktraceDatabaseContext : IBacktraceDatabaseContext, IDisposable
	{
		internal IDictionary<int, List<BacktraceDatabaseRecord>> BatchRetry { get; private set; }

		internal RetryOrder RetryOrder { get; set; }

		public DeduplicationStrategy DeduplicationStrategy { get; set; }

		public BacktraceDatabaseContext(BacktraceDatabaseSettings settings)
		{
			this._retryNumber = checked((int)settings.RetryLimit);
			this.RetryOrder = settings.RetryOrder;
			this.DeduplicationStrategy = settings.DeduplicationStrategy;
			this.SetupBatch();
		}

		private void SetupBatch()
		{
			this.BatchRetry = new Dictionary<int, List<BacktraceDatabaseRecord>>();
			if (this._retryNumber == 0)
			{
				throw new ArgumentException(string.Format("{0} have to be greater than 0!", "_retryNumber"));
			}
			for (int i = 0; i < this._retryNumber; i++)
			{
				this.BatchRetry[i] = new List<BacktraceDatabaseRecord>();
			}
		}

		public string GetHash(BacktraceData backtraceData)
		{
			string text = (backtraceData == null) ? string.Empty : (backtraceData.Report.Fingerprint ?? string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (this.DeduplicationStrategy == DeduplicationStrategy.None)
			{
				return string.Empty;
			}
			return new DeduplicationModel(backtraceData, this.DeduplicationStrategy).GetSha();
		}

		public BacktraceDatabaseRecord GetRecordByHash(string hash)
		{
			for (int i = 0; i < this.BatchRetry.Count; i++)
			{
				for (int j = 0; j < this.BatchRetry[i].Count; j++)
				{
					if (this.BatchRetry[i][j].Hash == hash)
					{
						BacktraceDatabaseRecord backtraceDatabaseRecord = this.BatchRetry[i][j];
						backtraceDatabaseRecord.Locked = true;
						return backtraceDatabaseRecord;
					}
				}
			}
			return null;
		}

		public BacktraceDatabaseRecord Add(BacktraceDatabaseRecord backtraceRecord)
		{
			if (backtraceRecord == null)
			{
				throw new NullReferenceException("BacktraceDatabaseRecord");
			}
			backtraceRecord.Locked = true;
			this.TotalSize += backtraceRecord.Size;
			this.BatchRetry[0].Add(backtraceRecord);
			this.TotalRecords++;
			return backtraceRecord;
		}

		public bool Any(BacktraceDatabaseRecord record)
		{
			return this.BatchRetry.SelectMany((KeyValuePair<int, List<BacktraceDatabaseRecord>> n) => n.Value).Any((BacktraceDatabaseRecord n) => n.Id == record.Id);
		}

		public bool Any()
		{
			return this.TotalRecords != 0;
		}

		public void Delete(BacktraceDatabaseRecord record)
		{
			if (record == null)
			{
				return;
			}
			for (int i = 0; i < this.BatchRetry.Keys.Count; i++)
			{
				int key = this.BatchRetry.Keys.ElementAt(i);
				for (int j = 0; j < this.BatchRetry[key].Count; j++)
				{
					BacktraceDatabaseRecord backtraceDatabaseRecord = this.BatchRetry[key].ElementAt(j);
					if (backtraceDatabaseRecord.Id == record.Id)
					{
						this.BatchRetry[key].Remove(backtraceDatabaseRecord);
						if (backtraceDatabaseRecord.Count > 0)
						{
							this.TotalRecords -= backtraceDatabaseRecord.Count;
						}
						else
						{
							this.TotalRecords--;
						}
						this.TotalSize -= backtraceDatabaseRecord.Size;
						return;
					}
				}
			}
		}

		public void IncrementBatchRetry()
		{
			this.RemoveMaxRetries();
			this.IncrementBatches();
		}

		private void IncrementBatches()
		{
			for (int i = this._retryNumber - 2; i >= 0; i--)
			{
				List<BacktraceDatabaseRecord> value = this.BatchRetry[i];
				this.BatchRetry[i] = new List<BacktraceDatabaseRecord>();
				this.BatchRetry[i + 1] = value;
			}
		}

		private void RemoveMaxRetries()
		{
			List<BacktraceDatabaseRecord> list = this.BatchRetry[this._retryNumber - 1];
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				BacktraceDatabaseRecord backtraceDatabaseRecord = list[i];
				if (backtraceDatabaseRecord.Count > 0)
				{
					this.TotalRecords -= backtraceDatabaseRecord.Count;
				}
				else
				{
					this.TotalRecords--;
				}
				this.TotalSize -= backtraceDatabaseRecord.Size;
			}
		}

		public IEnumerable<BacktraceDatabaseRecord> Get()
		{
			return this.BatchRetry.SelectMany((KeyValuePair<int, List<BacktraceDatabaseRecord>> n) => n.Value);
		}

		public int Count()
		{
			int num = 0;
			for (int i = 0; i < this.BatchRetry.Count; i++)
			{
				for (int j = 0; j < this.BatchRetry[i].Count; j++)
				{
					num += this.BatchRetry[i][j].Count;
				}
			}
			return num;
		}

		public void Dispose()
		{
			this.TotalRecords = 0;
			this.BatchRetry.Clear();
		}

		public void Clear()
		{
			this.TotalRecords = 0;
			this.TotalSize = 0L;
			foreach (KeyValuePair<int, List<BacktraceDatabaseRecord>> keyValuePair in this.BatchRetry)
			{
				keyValuePair.Value.Clear();
			}
		}

		public BacktraceDatabaseRecord LastOrDefault()
		{
			if (this.RetryOrder != RetryOrder.Stack)
			{
				return this.GetFirstRecord();
			}
			return this.GetLastRecord();
		}

		public BacktraceDatabaseRecord FirstOrDefault()
		{
			if (this.RetryOrder != RetryOrder.Queue)
			{
				return this.GetLastRecord();
			}
			return this.GetFirstRecord();
		}

		public BacktraceDatabaseRecord FirstOrDefault(Func<BacktraceDatabaseRecord, bool> predicate)
		{
			return this.BatchRetry.SelectMany((KeyValuePair<int, List<BacktraceDatabaseRecord>> n) => n.Value).FirstOrDefault(predicate);
		}

		private BacktraceDatabaseRecord GetFirstRecord()
		{
			for (int i = 0; i < this._retryNumber; i++)
			{
				if (this.BatchRetry.ContainsKey(i))
				{
					if (this.BatchRetry[i].Any((BacktraceDatabaseRecord n) => !n.Locked))
					{
						BacktraceDatabaseRecord backtraceDatabaseRecord = this.BatchRetry[i].FirstOrDefault((BacktraceDatabaseRecord n) => !n.Locked);
						if (backtraceDatabaseRecord == null)
						{
							return null;
						}
						backtraceDatabaseRecord.Locked = true;
						return backtraceDatabaseRecord;
					}
				}
			}
			return null;
		}

		private BacktraceDatabaseRecord GetLastRecord()
		{
			for (int i = this._retryNumber - 1; i >= 0; i--)
			{
				if (this.BatchRetry[i].Any((BacktraceDatabaseRecord n) => !n.Locked))
				{
					BacktraceDatabaseRecord backtraceDatabaseRecord = this.BatchRetry[i].Last((BacktraceDatabaseRecord n) => !n.Locked);
					backtraceDatabaseRecord.Locked = true;
					return backtraceDatabaseRecord;
				}
			}
			return null;
		}

		public long GetSize()
		{
			return this.TotalSize;
		}

		public int GetTotalNumberOfRecords()
		{
			return this.Count();
		}

		public IEnumerable<BacktraceDatabaseRecord> GetRecordsToDelete()
		{
			return this.BatchRetry[this._retryNumber - 1];
		}

		public void AddDuplicate(BacktraceDatabaseRecord record)
		{
			record.Increment();
			this.TotalRecords++;
		}

		internal long TotalSize;

		internal int TotalRecords;

		private readonly int _retryNumber;
	}
}
