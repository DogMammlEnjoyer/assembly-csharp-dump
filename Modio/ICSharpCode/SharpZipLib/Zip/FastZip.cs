using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class FastZip
	{
		public FastZip()
		{
		}

		public FastZip(ZipEntryFactory.TimeSetting timeSetting)
		{
			this.entryFactory_ = new ZipEntryFactory(timeSetting);
			this.restoreDateTimeOnExtract_ = true;
		}

		public FastZip(DateTime time)
		{
			this.entryFactory_ = new ZipEntryFactory(time);
			this.restoreDateTimeOnExtract_ = true;
		}

		public FastZip(FastZipEvents events)
		{
			this.events_ = events;
		}

		public bool CreateEmptyDirectories
		{
			get
			{
				return this.createEmptyDirectories_;
			}
			set
			{
				this.createEmptyDirectories_ = value;
			}
		}

		public string Password
		{
			get
			{
				return this.password_;
			}
			set
			{
				this.password_ = value;
			}
		}

		public ZipEncryptionMethod EntryEncryptionMethod { get; set; } = ZipEncryptionMethod.ZipCrypto;

		public INameTransform NameTransform
		{
			get
			{
				return this.entryFactory_.NameTransform;
			}
			set
			{
				this.entryFactory_.NameTransform = value;
			}
		}

		public IEntryFactory EntryFactory
		{
			get
			{
				return this.entryFactory_;
			}
			set
			{
				if (value == null)
				{
					this.entryFactory_ = new ZipEntryFactory();
					return;
				}
				this.entryFactory_ = value;
			}
		}

		public UseZip64 UseZip64
		{
			get
			{
				return this.useZip64_;
			}
			set
			{
				this.useZip64_ = value;
			}
		}

		public bool RestoreDateTimeOnExtract
		{
			get
			{
				return this.restoreDateTimeOnExtract_;
			}
			set
			{
				this.restoreDateTimeOnExtract_ = value;
			}
		}

		public bool RestoreAttributesOnExtract
		{
			get
			{
				return this.restoreAttributesOnExtract_;
			}
			set
			{
				this.restoreAttributesOnExtract_ = value;
			}
		}

		public Deflater.CompressionLevel CompressionLevel
		{
			get
			{
				return this.compressionLevel_;
			}
			set
			{
				this.compressionLevel_ = value;
			}
		}

		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, directoryFilter);
		}

		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, string fileFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, null);
		}

		public void CreateZip(Stream outputStream, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter)
		{
			this.CreateZip(outputStream, sourceDirectory, recurse, fileFilter, directoryFilter, false);
		}

		public void CreateZip(Stream outputStream, string sourceDirectory, bool recurse, string fileFilter, string directoryFilter, bool leaveOpen)
		{
			FileSystemScanner scanner = new FileSystemScanner(fileFilter, directoryFilter);
			this.CreateZip(outputStream, sourceDirectory, recurse, scanner, leaveOpen);
		}

		public void CreateZip(string zipFileName, string sourceDirectory, bool recurse, IScanFilter fileFilter, IScanFilter directoryFilter)
		{
			this.CreateZip(File.Create(zipFileName), sourceDirectory, recurse, fileFilter, directoryFilter, false);
		}

		public void CreateZip(Stream outputStream, string sourceDirectory, bool recurse, IScanFilter fileFilter, IScanFilter directoryFilter, bool leaveOpen = false)
		{
			FileSystemScanner scanner = new FileSystemScanner(fileFilter, directoryFilter);
			this.CreateZip(outputStream, sourceDirectory, recurse, scanner, leaveOpen);
		}

		private void CreateZip(Stream outputStream, string sourceDirectory, bool recurse, FileSystemScanner scanner, bool leaveOpen)
		{
			this.NameTransform = new ZipNameTransform(sourceDirectory);
			this.sourceDirectory_ = sourceDirectory;
			using (this.outputStream_ = new ZipOutputStream(outputStream))
			{
				this.outputStream_.SetLevel((int)this.CompressionLevel);
				this.outputStream_.IsStreamOwner = !leaveOpen;
				this.outputStream_.NameTransform = null;
				if (!string.IsNullOrEmpty(this.password_) && this.EntryEncryptionMethod != ZipEncryptionMethod.None)
				{
					this.outputStream_.Password = this.password_;
				}
				this.outputStream_.UseZip64 = this.UseZip64;
				scanner.ProcessFile = (ProcessFileHandler)Delegate.Combine(scanner.ProcessFile, new ProcessFileHandler(this.ProcessFile));
				if (this.CreateEmptyDirectories)
				{
					scanner.ProcessDirectory += this.ProcessDirectory;
				}
				if (this.events_ != null)
				{
					if (this.events_.FileFailure != null)
					{
						scanner.FileFailure = (FileFailureHandler)Delegate.Combine(scanner.FileFailure, this.events_.FileFailure);
					}
					if (this.events_.DirectoryFailure != null)
					{
						scanner.DirectoryFailure = (DirectoryFailureHandler)Delegate.Combine(scanner.DirectoryFailure, this.events_.DirectoryFailure);
					}
				}
				scanner.Scan(sourceDirectory, recurse);
			}
		}

		public void ExtractZip(string zipFileName, string targetDirectory, string fileFilter)
		{
			this.ExtractZip(zipFileName, targetDirectory, FastZip.Overwrite.Always, null, fileFilter, null, this.restoreDateTimeOnExtract_, false);
		}

		public void ExtractZip(string zipFileName, string targetDirectory, FastZip.Overwrite overwrite, FastZip.ConfirmOverwriteDelegate confirmDelegate, string fileFilter, string directoryFilter, bool restoreDateTime, bool allowParentTraversal = false)
		{
			Stream inputStream = File.Open(zipFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			this.ExtractZip(inputStream, targetDirectory, overwrite, confirmDelegate, fileFilter, directoryFilter, restoreDateTime, true, allowParentTraversal);
		}

		public void ExtractZip(Stream inputStream, string targetDirectory, FastZip.Overwrite overwrite, FastZip.ConfirmOverwriteDelegate confirmDelegate, string fileFilter, string directoryFilter, bool restoreDateTime, bool isStreamOwner, bool allowParentTraversal = false)
		{
			if (overwrite == FastZip.Overwrite.Prompt && confirmDelegate == null)
			{
				throw new ArgumentNullException("confirmDelegate");
			}
			this.continueRunning_ = true;
			this.overwrite_ = overwrite;
			this.confirmDelegate_ = confirmDelegate;
			this.extractNameTransform_ = new WindowsNameTransform(targetDirectory, allowParentTraversal);
			this.fileFilter_ = new NameFilter(fileFilter);
			this.directoryFilter_ = new NameFilter(directoryFilter);
			this.restoreDateTimeOnExtract_ = restoreDateTime;
			using (this.zipFile_ = new ZipFile(inputStream, !isStreamOwner))
			{
				if (this.password_ != null)
				{
					this.zipFile_.Password = this.password_;
				}
				IEnumerator enumerator = this.zipFile_.GetEnumerator();
				while (this.continueRunning_ && enumerator.MoveNext())
				{
					ZipEntry zipEntry = (ZipEntry)enumerator.Current;
					if (zipEntry.IsFile)
					{
						if (this.directoryFilter_.IsMatch(Path.GetDirectoryName(zipEntry.Name)) && this.fileFilter_.IsMatch(zipEntry.Name))
						{
							this.ExtractEntry(zipEntry);
						}
					}
					else if (zipEntry.IsDirectory && this.directoryFilter_.IsMatch(zipEntry.Name) && this.CreateEmptyDirectories)
					{
						this.ExtractEntry(zipEntry);
					}
				}
			}
		}

		private void ProcessDirectory(object sender, DirectoryEventArgs e)
		{
			if (!e.HasMatchingFiles && this.CreateEmptyDirectories)
			{
				if (this.events_ != null)
				{
					this.events_.OnProcessDirectory(e.Name, e.HasMatchingFiles);
				}
				if (e.ContinueRunning && e.Name != this.sourceDirectory_)
				{
					ZipEntry entry = this.entryFactory_.MakeDirectoryEntry(e.Name);
					this.outputStream_.PutNextEntry(entry);
				}
			}
		}

		private void ProcessFile(object sender, ScanEventArgs e)
		{
			if (this.events_ != null && this.events_.ProcessFile != null)
			{
				this.events_.ProcessFile(sender, e);
			}
			if (e.ContinueRunning)
			{
				try
				{
					using (FileStream fileStream = File.Open(e.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						ZipEntry entry = this.entryFactory_.MakeFileEntry(e.Name);
						this.ConfigureEntryEncryption(entry);
						this.outputStream_.PutNextEntry(entry);
						this.AddFileContents(e.Name, fileStream);
					}
				}
				catch (Exception e2)
				{
					if (this.events_ == null)
					{
						this.continueRunning_ = false;
						throw;
					}
					this.continueRunning_ = this.events_.OnFileFailure(e.Name, e2);
				}
			}
		}

		private void ConfigureEntryEncryption(ZipEntry entry)
		{
			if (!string.IsNullOrEmpty(this.Password) && entry.AESEncryptionStrength == 0)
			{
				ZipEncryptionMethod entryEncryptionMethod = this.EntryEncryptionMethod;
				if (entryEncryptionMethod == ZipEncryptionMethod.AES128)
				{
					entry.AESKeySize = 128;
					return;
				}
				if (entryEncryptionMethod != ZipEncryptionMethod.AES256)
				{
					return;
				}
				entry.AESKeySize = 256;
			}
		}

		private void AddFileContents(string name, Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.buffer_ == null)
			{
				this.buffer_ = new byte[4096];
			}
			if (this.events_ != null && this.events_.Progress != null)
			{
				StreamUtils.Copy(stream, this.outputStream_, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, this, name);
			}
			else
			{
				StreamUtils.Copy(stream, this.outputStream_, this.buffer_);
			}
			if (this.events_ != null)
			{
				this.continueRunning_ = this.events_.OnCompletedFile(name);
			}
		}

		private void ExtractFileEntry(ZipEntry entry, string targetName)
		{
			bool flag = true;
			if (this.overwrite_ != FastZip.Overwrite.Always && File.Exists(targetName))
			{
				flag = (this.overwrite_ == FastZip.Overwrite.Prompt && this.confirmDelegate_ != null && this.confirmDelegate_(targetName));
			}
			if (flag)
			{
				if (this.events_ != null)
				{
					this.continueRunning_ = this.events_.OnProcessFile(entry.Name);
				}
				if (this.continueRunning_)
				{
					try
					{
						using (FileStream fileStream = File.Create(targetName))
						{
							if (this.buffer_ == null)
							{
								this.buffer_ = new byte[4096];
							}
							using (Stream inputStream = this.zipFile_.GetInputStream(entry))
							{
								if (this.events_ != null && this.events_.Progress != null)
								{
									StreamUtils.Copy(inputStream, fileStream, this.buffer_, this.events_.Progress, this.events_.ProgressInterval, this, entry.Name, entry.Size);
								}
								else
								{
									StreamUtils.Copy(inputStream, fileStream, this.buffer_);
								}
							}
							if (this.events_ != null)
							{
								this.continueRunning_ = this.events_.OnCompletedFile(entry.Name);
							}
						}
						if (this.restoreDateTimeOnExtract_)
						{
							switch (this.entryFactory_.Setting)
							{
							case ZipEntryFactory.TimeSetting.LastWriteTime:
								File.SetLastWriteTime(targetName, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.LastWriteTimeUtc:
								File.SetLastWriteTimeUtc(targetName, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.CreateTime:
								File.SetCreationTime(targetName, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.CreateTimeUtc:
								File.SetCreationTimeUtc(targetName, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.LastAccessTime:
								File.SetLastAccessTime(targetName, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.LastAccessTimeUtc:
								File.SetLastAccessTimeUtc(targetName, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.Fixed:
								File.SetLastWriteTime(targetName, this.entryFactory_.FixedDateTime);
								break;
							default:
								throw new ZipException("Unhandled time setting in ExtractFileEntry");
							}
						}
						if (this.RestoreAttributesOnExtract && entry.IsDOSEntry && entry.ExternalFileAttributes != -1)
						{
							FileAttributes fileAttributes = (FileAttributes)entry.ExternalFileAttributes;
							fileAttributes &= (FileAttributes.ReadOnly | FileAttributes.Hidden | FileAttributes.Archive | FileAttributes.Normal);
							File.SetAttributes(targetName, fileAttributes);
						}
					}
					catch (Exception e)
					{
						if (this.events_ == null)
						{
							this.continueRunning_ = false;
							throw;
						}
						this.continueRunning_ = this.events_.OnFileFailure(targetName, e);
					}
				}
			}
		}

		private void ExtractEntry(ZipEntry entry)
		{
			bool flag = entry.IsCompressionMethodSupported();
			string text = entry.Name;
			if (flag)
			{
				if (entry.IsFile)
				{
					text = this.extractNameTransform_.TransformFile(text);
				}
				else if (entry.IsDirectory)
				{
					text = this.extractNameTransform_.TransformDirectory(text);
				}
				flag = !string.IsNullOrEmpty(text);
			}
			string text2 = string.Empty;
			if (flag)
			{
				if (entry.IsDirectory)
				{
					text2 = text;
				}
				else
				{
					text2 = Path.GetDirectoryName(Path.GetFullPath(text));
				}
			}
			if (flag && !Directory.Exists(text2) && (!entry.IsDirectory || this.CreateEmptyDirectories))
			{
				try
				{
					FastZipEvents fastZipEvents = this.events_;
					this.continueRunning_ = (fastZipEvents == null || fastZipEvents.OnProcessDirectory(text2, true));
					if (this.continueRunning_)
					{
						Directory.CreateDirectory(text2);
						if (entry.IsDirectory && this.restoreDateTimeOnExtract_)
						{
							switch (this.entryFactory_.Setting)
							{
							case ZipEntryFactory.TimeSetting.LastWriteTime:
								Directory.SetLastWriteTime(text2, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.LastWriteTimeUtc:
								Directory.SetLastWriteTimeUtc(text2, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.CreateTime:
								Directory.SetCreationTime(text2, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.CreateTimeUtc:
								Directory.SetCreationTimeUtc(text2, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.LastAccessTime:
								Directory.SetLastAccessTime(text2, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.LastAccessTimeUtc:
								Directory.SetLastAccessTimeUtc(text2, entry.DateTime);
								break;
							case ZipEntryFactory.TimeSetting.Fixed:
								Directory.SetLastWriteTime(text2, this.entryFactory_.FixedDateTime);
								break;
							default:
								throw new ZipException("Unhandled time setting in ExtractEntry");
							}
						}
					}
					else
					{
						flag = false;
					}
				}
				catch (Exception e)
				{
					flag = false;
					if (this.events_ == null)
					{
						this.continueRunning_ = false;
						throw;
					}
					if (entry.IsDirectory)
					{
						this.continueRunning_ = this.events_.OnDirectoryFailure(text, e);
					}
					else
					{
						this.continueRunning_ = this.events_.OnFileFailure(text, e);
					}
				}
			}
			if (flag && entry.IsFile)
			{
				this.ExtractFileEntry(entry, text);
			}
		}

		private static int MakeExternalAttributes(FileInfo info)
		{
			return (int)info.Attributes;
		}

		private static bool NameIsValid(string name)
		{
			return !string.IsNullOrEmpty(name) && name.IndexOfAny(Path.GetInvalidPathChars()) < 0;
		}

		private bool continueRunning_;

		private byte[] buffer_;

		private ZipOutputStream outputStream_;

		private ZipFile zipFile_;

		private string sourceDirectory_;

		private NameFilter fileFilter_;

		private NameFilter directoryFilter_;

		private FastZip.Overwrite overwrite_;

		private FastZip.ConfirmOverwriteDelegate confirmDelegate_;

		private bool restoreDateTimeOnExtract_;

		private bool restoreAttributesOnExtract_;

		private bool createEmptyDirectories_;

		private FastZipEvents events_;

		private IEntryFactory entryFactory_ = new ZipEntryFactory();

		private INameTransform extractNameTransform_;

		private UseZip64 useZip64_ = UseZip64.Dynamic;

		private Deflater.CompressionLevel compressionLevel_ = Deflater.CompressionLevel.DEFAULT_COMPRESSION;

		private string password_;

		public enum Overwrite
		{
			Prompt,
			Never,
			Always
		}

		public delegate bool ConfirmOverwriteDelegate(string fileName);
	}
}
