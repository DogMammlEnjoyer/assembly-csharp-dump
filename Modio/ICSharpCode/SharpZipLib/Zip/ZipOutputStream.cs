using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Encryption;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipOutputStream : DeflaterOutputStream
	{
		public ZipOutputStream(Stream baseOutputStream) : base(baseOutputStream, new Deflater(-1, true))
		{
		}

		public ZipOutputStream(Stream baseOutputStream, int bufferSize) : base(baseOutputStream, new Deflater(-1, true), bufferSize)
		{
		}

		public bool IsFinished
		{
			get
			{
				return this.entries == null;
			}
		}

		public void SetComment(string comment)
		{
			byte[] array = ZipStrings.ConvertToArray(comment);
			if (array.Length > 65535)
			{
				throw new ArgumentOutOfRangeException("comment");
			}
			this.zipComment = array;
		}

		public void SetLevel(int level)
		{
			this.deflater_.SetLevel(level);
			this.defaultCompressionLevel = level;
		}

		public int GetLevel()
		{
			return this.deflater_.GetLevel();
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

		public INameTransform NameTransform { get; set; } = new PathTransformer();

		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				if (value != null && value.Length == 0)
				{
					this.password = null;
					return;
				}
				this.password = value;
			}
		}

		private void WriteLeShort(int value)
		{
			this.baseOutputStream_.WriteByte((byte)(value & 255));
			this.baseOutputStream_.WriteByte((byte)(value >> 8 & 255));
		}

		private void WriteLeInt(int value)
		{
			this.WriteLeShort(value);
			this.WriteLeShort(value >> 16);
		}

		private void WriteLeLong(long value)
		{
			this.WriteLeInt((int)value);
			this.WriteLeInt((int)(value >> 32));
		}

		private void TransformEntryName(ZipEntry entry)
		{
			if (this.NameTransform != null)
			{
				if (entry.IsDirectory)
				{
					entry.Name = this.NameTransform.TransformDirectory(entry.Name);
					return;
				}
				entry.Name = this.NameTransform.TransformFile(entry.Name);
			}
		}

		public void PutNextEntry(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			if (this.entries == null)
			{
				throw new InvalidOperationException("ZipOutputStream was finished");
			}
			if (this.curEntry != null)
			{
				this.CloseEntry();
			}
			if (this.entries.Count == 2147483647)
			{
				throw new ZipException("Too many entries for Zip file");
			}
			CompressionMethod compressionMethod = entry.CompressionMethod;
			if (compressionMethod != CompressionMethod.Deflated && compressionMethod != CompressionMethod.Stored)
			{
				throw new NotImplementedException("Compression method not supported");
			}
			if (entry.AESKeySize > 0 && string.IsNullOrEmpty(this.Password))
			{
				throw new InvalidOperationException("The Password property must be set before AES encrypted entries can be added");
			}
			int level = this.defaultCompressionLevel;
			entry.Flags &= 2048;
			this.patchEntryHeader = false;
			bool flag;
			if (entry.Size == 0L)
			{
				entry.CompressedSize = entry.Size;
				entry.Crc = 0L;
				compressionMethod = CompressionMethod.Stored;
				flag = true;
			}
			else
			{
				flag = (entry.Size >= 0L && entry.HasCrc && entry.CompressedSize >= 0L);
				if (compressionMethod == CompressionMethod.Stored)
				{
					if (!flag)
					{
						if (!base.CanPatchEntries)
						{
							compressionMethod = CompressionMethod.Deflated;
							level = 0;
						}
					}
					else
					{
						entry.CompressedSize = entry.Size;
						flag = entry.HasCrc;
					}
				}
			}
			if (!flag)
			{
				if (!base.CanPatchEntries)
				{
					entry.Flags |= 8;
				}
				else
				{
					this.patchEntryHeader = true;
				}
			}
			if (this.Password != null)
			{
				entry.IsCrypted = true;
				if (entry.Crc < 0L)
				{
					entry.Flags |= 8;
				}
			}
			entry.Offset = this.offset;
			entry.CompressionMethod = compressionMethod;
			this.curMethod = compressionMethod;
			this.sizePatchPos = -1L;
			if (this.useZip64_ == UseZip64.On || (entry.Size < 0L && this.useZip64_ == UseZip64.Dynamic))
			{
				entry.ForceZip64();
			}
			this.WriteLeInt(67324752);
			this.WriteLeShort(entry.Version);
			this.WriteLeShort(entry.Flags);
			this.WriteLeShort((int)((byte)entry.CompressionMethodForHeader));
			this.WriteLeInt((int)entry.DosTime);
			if (flag)
			{
				this.WriteLeInt((int)entry.Crc);
				if (entry.LocalHeaderRequiresZip64)
				{
					this.WriteLeInt(-1);
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt((int)entry.CompressedSize + entry.EncryptionOverheadSize);
					this.WriteLeInt((int)entry.Size);
				}
			}
			else
			{
				if (this.patchEntryHeader)
				{
					this.crcPatchPos = this.baseOutputStream_.Position;
				}
				this.WriteLeInt(0);
				if (this.patchEntryHeader)
				{
					this.sizePatchPos = this.baseOutputStream_.Position;
				}
				if (entry.LocalHeaderRequiresZip64 || this.patchEntryHeader)
				{
					this.WriteLeInt(-1);
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt(0);
					this.WriteLeInt(0);
				}
			}
			this.TransformEntryName(entry);
			byte[] array = ZipStrings.ConvertToArray(entry.Flags, entry.Name);
			if (array.Length > 65535)
			{
				throw new ZipException("Entry name too long.");
			}
			ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
			if (entry.LocalHeaderRequiresZip64)
			{
				zipExtraData.StartNewEntry();
				if (flag)
				{
					zipExtraData.AddLeLong(entry.Size);
					zipExtraData.AddLeLong(entry.CompressedSize + (long)entry.EncryptionOverheadSize);
				}
				else
				{
					zipExtraData.AddLeLong(-1L);
					zipExtraData.AddLeLong(-1L);
				}
				zipExtraData.AddNewEntry(1);
				if (!zipExtraData.Find(1))
				{
					throw new ZipException("Internal error cant find extra data");
				}
				if (this.patchEntryHeader)
				{
					this.sizePatchPos = (long)zipExtraData.CurrentReadIndex;
				}
			}
			else
			{
				zipExtraData.Delete(1);
			}
			if (entry.AESKeySize > 0)
			{
				ZipOutputStream.AddExtraDataAES(entry, zipExtraData);
			}
			byte[] entryData = zipExtraData.GetEntryData();
			this.WriteLeShort(array.Length);
			this.WriteLeShort(entryData.Length);
			if (array.Length != 0)
			{
				this.baseOutputStream_.Write(array, 0, array.Length);
			}
			if (entry.LocalHeaderRequiresZip64 && this.patchEntryHeader)
			{
				this.sizePatchPos += this.baseOutputStream_.Position;
			}
			if (entryData.Length != 0)
			{
				this.baseOutputStream_.Write(entryData, 0, entryData.Length);
			}
			this.offset += (long)(30 + array.Length + entryData.Length);
			if (entry.AESKeySize > 0)
			{
				this.offset += (long)entry.AESOverheadSize;
			}
			this.curEntry = entry;
			this.crc.Reset();
			if (compressionMethod == CompressionMethod.Deflated)
			{
				this.deflater_.Reset();
				this.deflater_.SetLevel(level);
			}
			this.size = 0L;
			if (entry.IsCrypted)
			{
				if (entry.AESKeySize > 0)
				{
					this.WriteAESHeader(entry);
					return;
				}
				if (entry.Crc < 0L)
				{
					this.WriteEncryptionHeader(entry.DosTime << 16);
					return;
				}
				this.WriteEncryptionHeader(entry.Crc);
			}
		}

		public void CloseEntry()
		{
			if (this.curEntry == null)
			{
				throw new InvalidOperationException("No open entry");
			}
			long totalOut = this.size;
			if (this.curMethod == CompressionMethod.Deflated)
			{
				if (this.size >= 0L)
				{
					base.Finish();
					totalOut = this.deflater_.TotalOut;
				}
				else
				{
					this.deflater_.Reset();
				}
			}
			else if (this.curMethod == CompressionMethod.Stored)
			{
				base.GetAuthCodeIfAES();
			}
			if (this.curEntry.AESKeySize > 0)
			{
				this.baseOutputStream_.Write(this.AESAuthCode, 0, 10);
				this.curEntry.Crc = 0L;
			}
			else if (this.curEntry.Crc < 0L)
			{
				this.curEntry.Crc = this.crc.Value;
			}
			else if (this.curEntry.Crc != this.crc.Value)
			{
				throw new ZipException(string.Format("crc was {0}, but {1} was expected", this.crc.Value, this.curEntry.Crc));
			}
			if (this.curEntry.Size < 0L)
			{
				this.curEntry.Size = this.size;
			}
			else if (this.curEntry.Size != this.size)
			{
				throw new ZipException(string.Format("size was {0}, but {1} was expected", this.size, this.curEntry.Size));
			}
			if (this.curEntry.CompressedSize < 0L)
			{
				this.curEntry.CompressedSize = totalOut;
			}
			else if (this.curEntry.CompressedSize != totalOut)
			{
				throw new ZipException(string.Format("compressed size was {0}, but {1} expected", totalOut, this.curEntry.CompressedSize));
			}
			this.offset += totalOut;
			if (this.curEntry.IsCrypted)
			{
				this.curEntry.CompressedSize += (long)this.curEntry.EncryptionOverheadSize;
			}
			if (this.patchEntryHeader)
			{
				this.patchEntryHeader = false;
				long position = this.baseOutputStream_.Position;
				this.baseOutputStream_.Seek(this.crcPatchPos, SeekOrigin.Begin);
				this.WriteLeInt((int)this.curEntry.Crc);
				if (this.curEntry.LocalHeaderRequiresZip64)
				{
					if (this.sizePatchPos == -1L)
					{
						throw new ZipException("Entry requires zip64 but this has been turned off");
					}
					this.baseOutputStream_.Seek(this.sizePatchPos, SeekOrigin.Begin);
					this.WriteLeLong(this.curEntry.Size);
					this.WriteLeLong(this.curEntry.CompressedSize);
				}
				else
				{
					this.WriteLeInt((int)this.curEntry.CompressedSize);
					this.WriteLeInt((int)this.curEntry.Size);
				}
				this.baseOutputStream_.Seek(position, SeekOrigin.Begin);
			}
			if ((this.curEntry.Flags & 8) != 0)
			{
				this.WriteLeInt(134695760);
				this.WriteLeInt((int)this.curEntry.Crc);
				if (this.curEntry.LocalHeaderRequiresZip64)
				{
					this.WriteLeLong(this.curEntry.CompressedSize);
					this.WriteLeLong(this.curEntry.Size);
					this.offset += 24L;
				}
				else
				{
					this.WriteLeInt((int)this.curEntry.CompressedSize);
					this.WriteLeInt((int)this.curEntry.Size);
					this.offset += 16L;
				}
			}
			this.entries.Add(this.curEntry);
			this.curEntry = null;
		}

		private void InitializePassword(string password)
		{
			PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
			byte[] rgbKey = PkzipClassic.GenerateKeys(ZipStrings.ConvertToArray(password));
			this.cryptoTransform_ = pkzipClassicManaged.CreateEncryptor(rgbKey, null);
		}

		private void InitializeAESPassword(ZipEntry entry, string rawPassword, out byte[] salt, out byte[] pwdVerifier)
		{
			salt = new byte[entry.AESSaltLen];
			ZipOutputStream._aesRnd.GetBytes(salt);
			int blockSize = entry.AESKeySize / 8;
			this.cryptoTransform_ = new ZipAESTransform(rawPassword, salt, blockSize, true);
			pwdVerifier = ((ZipAESTransform)this.cryptoTransform_).PwdVerifier;
		}

		private void WriteEncryptionHeader(long crcValue)
		{
			this.offset += 12L;
			this.InitializePassword(this.Password);
			byte[] array = new byte[12];
			using (RNGCryptoServiceProvider rngcryptoServiceProvider = new RNGCryptoServiceProvider())
			{
				rngcryptoServiceProvider.GetBytes(array);
			}
			array[11] = (byte)(crcValue >> 24);
			base.EncryptBlock(array, 0, array.Length);
			this.baseOutputStream_.Write(array, 0, array.Length);
		}

		private static void AddExtraDataAES(ZipEntry entry, ZipExtraData extraData)
		{
			extraData.StartNewEntry();
			extraData.AddLeShort(2);
			extraData.AddLeShort(17729);
			extraData.AddData(entry.AESEncryptionStrength);
			extraData.AddLeShort((int)entry.CompressionMethod);
			extraData.AddNewEntry(39169);
		}

		private void WriteAESHeader(ZipEntry entry)
		{
			byte[] array;
			byte[] array2;
			this.InitializeAESPassword(entry, this.Password, out array, out array2);
			this.baseOutputStream_.Write(array, 0, array.Length);
			this.baseOutputStream_.Write(array2, 0, array2.Length);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.curEntry == null)
			{
				throw new InvalidOperationException("No open entry.");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot be negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Cannot be negative");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("Invalid offset/count combination");
			}
			if (this.curEntry.AESKeySize == 0)
			{
				this.crc.Update(new ArraySegment<byte>(buffer, offset, count));
			}
			this.size += (long)count;
			CompressionMethod compressionMethod = this.curMethod;
			if (compressionMethod != CompressionMethod.Stored)
			{
				if (compressionMethod == CompressionMethod.Deflated)
				{
					base.Write(buffer, offset, count);
					return;
				}
			}
			else
			{
				if (this.Password != null)
				{
					this.CopyAndEncrypt(buffer, offset, count);
					return;
				}
				this.baseOutputStream_.Write(buffer, offset, count);
			}
		}

		private void CopyAndEncrypt(byte[] buffer, int offset, int count)
		{
			byte[] array = new byte[4096];
			while (count > 0)
			{
				int num = (count < 4096) ? count : 4096;
				Array.Copy(buffer, offset, array, 0, num);
				base.EncryptBlock(array, 0, num);
				this.baseOutputStream_.Write(array, 0, num);
				count -= num;
				offset += num;
			}
		}

		public override void Finish()
		{
			if (this.entries == null)
			{
				return;
			}
			if (this.curEntry != null)
			{
				this.CloseEntry();
			}
			long noOfEntries = (long)this.entries.Count;
			long num = 0L;
			foreach (ZipEntry zipEntry in this.entries)
			{
				this.WriteLeInt(33639248);
				this.WriteLeShort(zipEntry.HostSystem << 8 | zipEntry.VersionMadeBy);
				this.WriteLeShort(zipEntry.Version);
				this.WriteLeShort(zipEntry.Flags);
				this.WriteLeShort((int)((short)zipEntry.CompressionMethodForHeader));
				this.WriteLeInt((int)zipEntry.DosTime);
				this.WriteLeInt((int)zipEntry.Crc);
				if (zipEntry.IsZip64Forced() || zipEntry.CompressedSize >= (long)((ulong)-1))
				{
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt((int)zipEntry.CompressedSize);
				}
				if (zipEntry.IsZip64Forced() || zipEntry.Size >= (long)((ulong)-1))
				{
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt((int)zipEntry.Size);
				}
				byte[] array = ZipStrings.ConvertToArray(zipEntry.Flags, zipEntry.Name);
				if (array.Length > 65535)
				{
					throw new ZipException("Name too long.");
				}
				ZipExtraData zipExtraData = new ZipExtraData(zipEntry.ExtraData);
				if (zipEntry.CentralHeaderRequiresZip64)
				{
					zipExtraData.StartNewEntry();
					if (zipEntry.IsZip64Forced() || zipEntry.Size >= (long)((ulong)-1))
					{
						zipExtraData.AddLeLong(zipEntry.Size);
					}
					if (zipEntry.IsZip64Forced() || zipEntry.CompressedSize >= (long)((ulong)-1))
					{
						zipExtraData.AddLeLong(zipEntry.CompressedSize);
					}
					if (zipEntry.Offset >= (long)((ulong)-1))
					{
						zipExtraData.AddLeLong(zipEntry.Offset);
					}
					zipExtraData.AddNewEntry(1);
				}
				else
				{
					zipExtraData.Delete(1);
				}
				if (zipEntry.AESKeySize > 0)
				{
					ZipOutputStream.AddExtraDataAES(zipEntry, zipExtraData);
				}
				byte[] entryData = zipExtraData.GetEntryData();
				byte[] array2 = (zipEntry.Comment != null) ? ZipStrings.ConvertToArray(zipEntry.Flags, zipEntry.Comment) : Empty.Array<byte>();
				if (array2.Length > 65535)
				{
					throw new ZipException("Comment too long.");
				}
				this.WriteLeShort(array.Length);
				this.WriteLeShort(entryData.Length);
				this.WriteLeShort(array2.Length);
				this.WriteLeShort(0);
				this.WriteLeShort(0);
				if (zipEntry.ExternalFileAttributes != -1)
				{
					this.WriteLeInt(zipEntry.ExternalFileAttributes);
				}
				else if (zipEntry.IsDirectory)
				{
					this.WriteLeInt(16);
				}
				else
				{
					this.WriteLeInt(0);
				}
				if (zipEntry.Offset >= (long)((ulong)-1))
				{
					this.WriteLeInt(-1);
				}
				else
				{
					this.WriteLeInt((int)zipEntry.Offset);
				}
				if (array.Length != 0)
				{
					this.baseOutputStream_.Write(array, 0, array.Length);
				}
				if (entryData.Length != 0)
				{
					this.baseOutputStream_.Write(entryData, 0, entryData.Length);
				}
				if (array2.Length != 0)
				{
					this.baseOutputStream_.Write(array2, 0, array2.Length);
				}
				num += (long)(46 + array.Length + entryData.Length + array2.Length);
			}
			using (ZipHelperStream zipHelperStream = new ZipHelperStream(this.baseOutputStream_))
			{
				zipHelperStream.WriteEndOfCentralDirectory(noOfEntries, num, this.offset, this.zipComment);
			}
			this.entries = null;
		}

		public override void Flush()
		{
			if (this.curMethod == CompressionMethod.Stored)
			{
				this.baseOutputStream_.Flush();
				return;
			}
			base.Flush();
		}

		private List<ZipEntry> entries = new List<ZipEntry>();

		private Crc32 crc = new Crc32();

		private ZipEntry curEntry;

		private int defaultCompressionLevel = -1;

		private CompressionMethod curMethod = CompressionMethod.Deflated;

		private long size;

		private long offset;

		private byte[] zipComment = Empty.Array<byte>();

		private bool patchEntryHeader;

		private long crcPatchPos = -1L;

		private long sizePatchPos = -1L;

		private UseZip64 useZip64_ = UseZip64.Dynamic;

		private string password;

		private static RandomNumberGenerator _aesRnd = RandomNumberGenerator.Create();
	}
}
