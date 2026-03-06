using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Backtrace.Unity.Common;
using Backtrace.Unity.Interfaces;
using Backtrace.Unity.Model;
using Backtrace.Unity.Model.Database;
using UnityEngine;

namespace Backtrace.Unity.Services
{
	internal class BacktraceDatabaseFileContext : IBacktraceDatabaseFileContext
	{
		public int ScreenshotQuality
		{
			get
			{
				return this._attachmentManager.ScreenshotQuality;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("ScreenshotQuality has to be greater than 0");
				}
				if (value > 100)
				{
					throw new ArgumentException("ScreenshotQuality cannot be larger than 100");
				}
				this._attachmentManager.ScreenshotQuality = value;
			}
		}

		public int ScreenshotMaxHeight
		{
			get
			{
				return this._attachmentManager.ScreenshotMaxHeight;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("ScreenshotMaxHeight has to be greater than 0");
				}
				this._attachmentManager.ScreenshotMaxHeight = value;
			}
		}

		public BacktraceDatabaseFileContext(BacktraceDatabaseSettings settings)
		{
			this._attachmentManager = new BacktraceDatabaseAttachmentManager(settings);
			this._maxDatabaseSize = settings.MaxDatabaseSize;
			this._maxRecordNumber = settings.MaxRecordCount;
			this._path = settings.DatabasePath;
			this._databaseDirectoryInfo = new DirectoryInfo(this._path);
		}

		public IEnumerable<FileInfo> GetAll()
		{
			return this._databaseDirectoryInfo.GetFiles();
		}

		public IEnumerable<FileInfo> GetRecords()
		{
			return from n in this._databaseDirectoryInfo.GetFiles("*-record.json", SearchOption.TopDirectoryOnly)
			orderby n.CreationTime
			select n;
		}

		public void RemoveOrphaned(IEnumerable<BacktraceDatabaseRecord> existingRecords)
		{
			IEnumerable<string> source = from n in existingRecords
			select n.Id.ToString();
			IEnumerable<FileInfo> all = this.GetAll();
			for (int i = 0; i < all.Count<FileInfo>(); i++)
			{
				FileInfo file = all.ElementAt(i);
				try
				{
					if (!file.Name.StartsWith("bt-breadcrumbs"))
					{
						if (!this._possibleDatabaseExtension.Any((string n) => n == file.Extension))
						{
							file.Delete();
						}
						else
						{
							int num = file.Name.LastIndexOf('-');
							if (num == -1)
							{
								file.Delete();
							}
							else
							{
								string value = file.Name.Substring(0, num);
								if (!source.Contains(value))
								{
									file.Delete();
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning(string.Format("Cannot remove file in path: {0}. Reason: {1}", file.FullName, ex.Message));
				}
			}
		}

		public bool ValidFileConsistency()
		{
			FileInfo[] files = this._databaseDirectoryInfo.GetFiles();
			long num = 0L;
			long num2 = 0L;
			foreach (FileInfo fileInfo in files)
			{
				if (Regex.Match(fileInfo.FullName, "*-record.json").Success)
				{
					num2 += 1L;
					if ((ulong)this._maxRecordNumber > (ulong)num2)
					{
						return false;
					}
				}
				num += fileInfo.Length;
				if (num > this._maxDatabaseSize)
				{
					return false;
				}
			}
			return true;
		}

		public void Clear()
		{
			FileInfo[] files = this._databaseDirectoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
		}

		public void Delete(BacktraceDatabaseRecord record)
		{
			this.Delete(record.DiagnosticDataPath);
			this.Delete(record.RecordPath);
			if (record.Attachments == null || record.Attachments.Count == 0)
			{
				return;
			}
			foreach (string path in record.Attachments)
			{
				this.Delete(path);
			}
		}

		private bool IsDatabaseDependency(string path)
		{
			return !string.IsNullOrEmpty(path) && File.Exists(path) && ClientPathHelper.IsFileInDatabaseDirectory(this._path, path) && !path.EndsWith("bt-breadcrumbs-0");
		}

		private void Delete(string path)
		{
			try
			{
				if (this.IsDatabaseDependency(path))
				{
					File.Delete(path);
				}
			}
			catch (IOException ex)
			{
				Debug.Log(string.Format("File {0} is in use. Message: {1}", path, ex.Message));
			}
			catch (Exception ex2)
			{
				Debug.Log(string.Format("Cannot delete file: {0}. Message: {1}", path, ex2.Message));
			}
		}

		public IEnumerable<string> GenerateRecordAttachments(BacktraceData data)
		{
			return this._attachmentManager.GetReportAttachments(data);
		}

		public bool Save(BacktraceDatabaseRecord record)
		{
			bool result;
			try
			{
				string uuidString = record.BacktraceData.UuidString;
				record.DiagnosticDataJson = record.BacktraceData.ToJson();
				record.DiagnosticDataPath = Path.Combine(this._path, string.Format("{0}-attachment.json", uuidString));
				record.Size += (long)this.Save(record.DiagnosticDataJson, record.DiagnosticDataPath);
				if (record.Attachments != null && record.Attachments.Count != 0)
				{
					foreach (string text in record.Attachments)
					{
						if (this.IsDatabaseDependency(text))
						{
							record.Size += new FileInfo(text).Length;
						}
					}
				}
				record.RecordPath = Path.Combine(this._path, string.Format("{0}-record.json", uuidString));
				string text2 = record.ToJson();
				record.Size += (long)Encoding.Unicode.GetByteCount(text2);
				this.Save(text2, record.RecordPath);
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning(string.Format("Backtrace: Cannot save record on the hard drive. Reason: {0}", ex.Message));
				this.Delete(record);
				result = false;
			}
			return result;
		}

		private int Save(string json, string destPath)
		{
			if (string.IsNullOrEmpty(json))
			{
				return 0;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			using (FileStream fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
			{
				fileStream.Write(bytes, 0, bytes.Length);
			}
			return bytes.Length;
		}

		public bool IsValidRecord(BacktraceDatabaseRecord record)
		{
			return record != null && File.Exists(record.DiagnosticDataPath);
		}

		private string[] _possibleDatabaseExtension = new string[]
		{
			".dmp",
			".json",
			".jpg",
			".log"
		};

		private readonly long _maxDatabaseSize;

		private readonly uint _maxRecordNumber;

		private readonly DirectoryInfo _databaseDirectoryInfo;

		private const string RecordFilterRegex = "*-record.json";

		internal readonly BacktraceDatabaseAttachmentManager _attachmentManager;

		private readonly string _path;
	}
}
