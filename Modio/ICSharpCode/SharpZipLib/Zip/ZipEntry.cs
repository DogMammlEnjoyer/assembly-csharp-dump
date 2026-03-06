using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class ZipEntry
	{
		public ZipEntry(string name) : this(name, 0, 51, CompressionMethod.Deflated)
		{
		}

		internal ZipEntry(string name, int versionRequiredToExtract) : this(name, versionRequiredToExtract, 51, CompressionMethod.Deflated)
		{
		}

		internal ZipEntry(string name, int versionRequiredToExtract, int madeByInfo, CompressionMethod method)
		{
			this.externalFileAttributes = -1;
			this.method = CompressionMethod.Deflated;
			this.zipFileIndex = -1L;
			base..ctor();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length > 65535)
			{
				throw new ArgumentException("Name is too long", "name");
			}
			if (versionRequiredToExtract != 0 && versionRequiredToExtract < 10)
			{
				throw new ArgumentOutOfRangeException("versionRequiredToExtract");
			}
			this.DateTime = DateTime.Now;
			this.name = name;
			this.versionMadeBy = (ushort)madeByInfo;
			this.versionToExtract = (ushort)versionRequiredToExtract;
			this.method = method;
			this.IsUnicodeText = ZipStrings.UseUnicode;
		}

		[Obsolete("Use Clone instead")]
		public ZipEntry(ZipEntry entry)
		{
			this.externalFileAttributes = -1;
			this.method = CompressionMethod.Deflated;
			this.zipFileIndex = -1L;
			base..ctor();
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			this.known = entry.known;
			this.name = entry.name;
			this.size = entry.size;
			this.compressedSize = entry.compressedSize;
			this.crc = entry.crc;
			this.dateTime = entry.DateTime;
			this.method = entry.method;
			this.comment = entry.comment;
			this.versionToExtract = entry.versionToExtract;
			this.versionMadeBy = entry.versionMadeBy;
			this.externalFileAttributes = entry.externalFileAttributes;
			this.flags = entry.flags;
			this.zipFileIndex = entry.zipFileIndex;
			this.offset = entry.offset;
			this.forceZip64_ = entry.forceZip64_;
			if (entry.extra != null)
			{
				this.extra = new byte[entry.extra.Length];
				Array.Copy(entry.extra, 0, this.extra, 0, entry.extra.Length);
			}
		}

		public bool HasCrc
		{
			get
			{
				return (this.known & ZipEntry.Known.Crc) > ZipEntry.Known.None;
			}
		}

		public bool IsCrypted
		{
			get
			{
				return this.HasFlag(GeneralBitFlags.Encrypted);
			}
			set
			{
				this.SetFlag(GeneralBitFlags.Encrypted, value);
			}
		}

		public bool IsUnicodeText
		{
			get
			{
				return this.HasFlag(GeneralBitFlags.UnicodeText);
			}
			set
			{
				this.SetFlag(GeneralBitFlags.UnicodeText, value);
			}
		}

		internal byte CryptoCheckValue
		{
			get
			{
				return this.cryptoCheckValue_;
			}
			set
			{
				this.cryptoCheckValue_ = value;
			}
		}

		public int Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public long ZipFileIndex
		{
			get
			{
				return this.zipFileIndex;
			}
			set
			{
				this.zipFileIndex = value;
			}
		}

		public long Offset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.offset = value;
			}
		}

		public int ExternalFileAttributes
		{
			get
			{
				if ((this.known & ZipEntry.Known.ExternalAttributes) != ZipEntry.Known.None)
				{
					return this.externalFileAttributes;
				}
				return -1;
			}
			set
			{
				this.externalFileAttributes = value;
				this.known |= ZipEntry.Known.ExternalAttributes;
			}
		}

		public int VersionMadeBy
		{
			get
			{
				return (int)(this.versionMadeBy & 255);
			}
		}

		public bool IsDOSEntry
		{
			get
			{
				return this.HostSystem == 0 || this.HostSystem == 10;
			}
		}

		private bool HasDosAttributes(int attributes)
		{
			bool flag = false;
			if ((this.known & ZipEntry.Known.ExternalAttributes) != ZipEntry.Known.None)
			{
				flag |= ((this.HostSystem == 0 || this.HostSystem == 10) && (this.ExternalFileAttributes & attributes) == attributes);
			}
			return flag;
		}

		public int HostSystem
		{
			get
			{
				return this.versionMadeBy >> 8 & 255;
			}
			set
			{
				this.versionMadeBy &= 255;
				this.versionMadeBy |= (ushort)((value & 255) << 8);
			}
		}

		public int Version
		{
			get
			{
				if (this.versionToExtract != 0)
				{
					return (int)(this.versionToExtract & 255);
				}
				if (this.AESKeySize > 0)
				{
					return 51;
				}
				if (CompressionMethod.BZip2 == this.method)
				{
					return 46;
				}
				if (this.CentralHeaderRequiresZip64)
				{
					return 45;
				}
				if (CompressionMethod.Deflated == this.method || this.IsDirectory || this.IsCrypted)
				{
					return 20;
				}
				if (this.HasDosAttributes(8))
				{
					return 11;
				}
				return 10;
			}
		}

		public bool CanDecompress
		{
			get
			{
				return this.Version <= 51 && (this.Version == 10 || this.Version == 11 || this.Version == 20 || this.Version == 45 || this.Version == 46 || this.Version == 51) && this.IsCompressionMethodSupported();
			}
		}

		public void ForceZip64()
		{
			this.forceZip64_ = true;
		}

		public bool IsZip64Forced()
		{
			return this.forceZip64_;
		}

		public bool LocalHeaderRequiresZip64
		{
			get
			{
				bool flag = this.forceZip64_;
				if (!flag)
				{
					ulong num = this.compressedSize;
					if (this.versionToExtract == 0 && this.IsCrypted)
					{
						num += (ulong)((long)this.EncryptionOverheadSize);
					}
					flag = ((this.size >= (ulong)-1 || num >= (ulong)-1) && (this.versionToExtract == 0 || this.versionToExtract >= 45));
				}
				return flag;
			}
		}

		public bool CentralHeaderRequiresZip64
		{
			get
			{
				return this.LocalHeaderRequiresZip64 || this.offset >= (long)((ulong)-1);
			}
		}

		public long DosTime
		{
			get
			{
				if ((this.known & ZipEntry.Known.Time) == ZipEntry.Known.None)
				{
					return 0L;
				}
				uint num = (uint)this.DateTime.Year;
				uint num2 = (uint)this.DateTime.Month;
				uint num3 = (uint)this.DateTime.Day;
				uint num4 = (uint)this.DateTime.Hour;
				uint num5 = (uint)this.DateTime.Minute;
				uint num6 = (uint)this.DateTime.Second;
				if (num < 1980U)
				{
					num = 1980U;
					num2 = 1U;
					num3 = 1U;
					num4 = 0U;
					num5 = 0U;
					num6 = 0U;
				}
				else if (num > 2107U)
				{
					num = 2107U;
					num2 = 12U;
					num3 = 31U;
					num4 = 23U;
					num5 = 59U;
					num6 = 59U;
				}
				return (long)((ulong)((num - 1980U & 127U) << 25 | num2 << 21 | num3 << 16 | num4 << 11 | num5 << 5 | num6 >> 1));
			}
			set
			{
				uint num = (uint)value;
				uint second = Math.Min(59U, 2U * (num & 31U));
				uint minute = Math.Min(59U, num >> 5 & 63U);
				uint hour = Math.Min(23U, num >> 11 & 31U);
				uint month = Math.Max(1U, Math.Min(12U, (uint)(value >> 21) & 15U));
				uint year = (num >> 25 & 127U) + 1980U;
				int day = Math.Max(1, Math.Min(DateTime.DaysInMonth((int)year, (int)month), (int)(value >> 16 & 31L)));
				this.DateTime = new DateTime((int)year, (int)month, day, (int)hour, (int)minute, (int)second, DateTimeKind.Unspecified);
			}
		}

		public DateTime DateTime
		{
			get
			{
				return this.dateTime;
			}
			set
			{
				this.dateTime = value;
				this.known |= ZipEntry.Known.Time;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public long Size
		{
			get
			{
				if ((this.known & ZipEntry.Known.Size) == ZipEntry.Known.None)
				{
					return -1L;
				}
				return (long)this.size;
			}
			set
			{
				this.size = (ulong)value;
				this.known |= ZipEntry.Known.Size;
			}
		}

		public long CompressedSize
		{
			get
			{
				if ((this.known & ZipEntry.Known.CompressedSize) == ZipEntry.Known.None)
				{
					return -1L;
				}
				return (long)this.compressedSize;
			}
			set
			{
				this.compressedSize = (ulong)value;
				this.known |= ZipEntry.Known.CompressedSize;
			}
		}

		public long Crc
		{
			get
			{
				if ((this.known & ZipEntry.Known.Crc) == ZipEntry.Known.None)
				{
					return -1L;
				}
				return (long)((ulong)this.crc & (ulong)-1);
			}
			set
			{
				if (((ulong)this.crc & 18446744069414584320UL) != 0UL)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.crc = (uint)value;
				this.known |= ZipEntry.Known.Crc;
			}
		}

		public CompressionMethod CompressionMethod
		{
			get
			{
				return this.method;
			}
			set
			{
				this.method = value;
			}
		}

		internal CompressionMethod CompressionMethodForHeader
		{
			get
			{
				if (this.AESKeySize <= 0)
				{
					return this.method;
				}
				return CompressionMethod.WinZipAES;
			}
		}

		public byte[] ExtraData
		{
			get
			{
				return this.extra;
			}
			set
			{
				if (value == null)
				{
					this.extra = null;
					return;
				}
				if (value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.extra = new byte[value.Length];
				Array.Copy(value, 0, this.extra, 0, value.Length);
			}
		}

		public int AESKeySize
		{
			get
			{
				switch (this._aesEncryptionStrength)
				{
				case 0:
					return 0;
				case 1:
					return 128;
				case 2:
					return 192;
				case 3:
					return 256;
				default:
					throw new ZipException("Invalid AESEncryptionStrength " + this._aesEncryptionStrength.ToString());
				}
			}
			set
			{
				if (value == 0)
				{
					this._aesEncryptionStrength = 0;
					return;
				}
				if (value == 128)
				{
					this._aesEncryptionStrength = 1;
					return;
				}
				if (value != 256)
				{
					throw new ZipException("AESKeySize must be 0, 128 or 256: " + value.ToString());
				}
				this._aesEncryptionStrength = 3;
			}
		}

		internal byte AESEncryptionStrength
		{
			get
			{
				return (byte)this._aesEncryptionStrength;
			}
		}

		internal int AESSaltLen
		{
			get
			{
				return this.AESKeySize / 16;
			}
		}

		internal int AESOverheadSize
		{
			get
			{
				return 12 + this.AESSaltLen;
			}
		}

		internal int EncryptionOverheadSize
		{
			get
			{
				if (!this.IsCrypted)
				{
					return 0;
				}
				if (this._aesEncryptionStrength != 0)
				{
					return this.AESOverheadSize;
				}
				return 12;
			}
		}

		internal void ProcessExtraData(bool localHeader)
		{
			ZipExtraData zipExtraData = new ZipExtraData(this.extra);
			if (zipExtraData.Find(1))
			{
				this.forceZip64_ = true;
				if (zipExtraData.ValueLength < 4)
				{
					throw new ZipException("Extra data extended Zip64 information length is invalid");
				}
				if (this.size == (ulong)-1)
				{
					this.size = (ulong)zipExtraData.ReadLong();
				}
				if (this.compressedSize == (ulong)-1)
				{
					this.compressedSize = (ulong)zipExtraData.ReadLong();
				}
				if (!localHeader && this.offset == (long)((ulong)-1))
				{
					this.offset = zipExtraData.ReadLong();
				}
			}
			else if ((this.versionToExtract & 255) >= 45 && (this.size == (ulong)-1 || this.compressedSize == (ulong)-1))
			{
				throw new ZipException("Zip64 Extended information required but is missing.");
			}
			this.DateTime = (ZipEntry.GetDateTime(zipExtraData) ?? this.DateTime);
			if (this.method == CompressionMethod.WinZipAES)
			{
				this.ProcessAESExtraData(zipExtraData);
			}
		}

		private static DateTime? GetDateTime(ZipExtraData extraData)
		{
			ExtendedUnixData data = extraData.GetData<ExtendedUnixData>();
			if (data != null && data.Include.HasFlag(ExtendedUnixData.Flags.ModificationTime))
			{
				return new DateTime?(data.ModificationTime);
			}
			return null;
		}

		private void ProcessAESExtraData(ZipExtraData extraData)
		{
			if (!extraData.Find(39169))
			{
				throw new ZipException("AES Extra Data missing");
			}
			this.versionToExtract = 51;
			int valueLength = extraData.ValueLength;
			if (valueLength < 7)
			{
				throw new ZipException("AES Extra Data Length " + valueLength.ToString() + " invalid.");
			}
			int aesVer = extraData.ReadShort();
			extraData.ReadShort();
			int aesEncryptionStrength = extraData.ReadByte();
			int num = extraData.ReadShort();
			this._aesVer = aesVer;
			this._aesEncryptionStrength = aesEncryptionStrength;
			this.method = (CompressionMethod)num;
		}

		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				if (value != null && value.Length > 65535)
				{
					throw new ArgumentOutOfRangeException("value", "cannot exceed 65535");
				}
				this.comment = value;
			}
		}

		public bool IsDirectory
		{
			get
			{
				return (this.name.Length > 0 && (this.name[this.name.Length - 1] == '/' || this.name[this.name.Length - 1] == '\\')) || this.HasDosAttributes(16);
			}
		}

		public bool IsFile
		{
			get
			{
				return !this.IsDirectory && !this.HasDosAttributes(8);
			}
		}

		public bool IsCompressionMethodSupported()
		{
			return ZipEntry.IsCompressionMethodSupported(this.CompressionMethod);
		}

		public object Clone()
		{
			ZipEntry zipEntry = (ZipEntry)base.MemberwiseClone();
			if (this.extra != null)
			{
				zipEntry.extra = new byte[this.extra.Length];
				Array.Copy(this.extra, 0, zipEntry.extra, 0, this.extra.Length);
			}
			return zipEntry;
		}

		public override string ToString()
		{
			return this.name;
		}

		public static bool IsCompressionMethodSupported(CompressionMethod method)
		{
			return method == CompressionMethod.Deflated || method == CompressionMethod.Stored || method == CompressionMethod.BZip2;
		}

		public static string CleanName(string name)
		{
			if (name == null)
			{
				return string.Empty;
			}
			if (Path.IsPathRooted(name))
			{
				name = name.Substring(Path.GetPathRoot(name).Length);
			}
			name = name.Replace("\\", "/");
			while (name.Length > 0 && name[0] == '/')
			{
				name = name.Remove(0, 1);
			}
			return name;
		}

		private ZipEntry.Known known;

		private int externalFileAttributes;

		private ushort versionMadeBy;

		private string name;

		private ulong size;

		private ulong compressedSize;

		private ushort versionToExtract;

		private uint crc;

		private DateTime dateTime;

		private CompressionMethod method;

		private byte[] extra;

		private string comment;

		private int flags;

		private long zipFileIndex;

		private long offset;

		private bool forceZip64_;

		private byte cryptoCheckValue_;

		private int _aesVer;

		private int _aesEncryptionStrength;

		[Flags]
		private enum Known : byte
		{
			None = 0,
			Size = 1,
			CompressedSize = 2,
			Crc = 4,
			Time = 8,
			ExternalAttributes = 16
		}
	}
}
