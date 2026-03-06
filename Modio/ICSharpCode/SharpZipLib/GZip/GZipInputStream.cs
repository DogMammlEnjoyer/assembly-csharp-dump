using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ICSharpCode.SharpZipLib.GZip
{
	public class GZipInputStream : InflaterInputStream
	{
		public GZipInputStream(Stream baseInputStream) : this(baseInputStream, 4096)
		{
		}

		public GZipInputStream(Stream baseInputStream, int size) : base(baseInputStream, new Inflater(true), size)
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num;
			do
			{
				if (!this.readGZIPHeader)
				{
					try
					{
						if (!this.ReadHeader())
						{
							return 0;
						}
					}
					catch (Exception ex) when (this.completedLastBlock && (ex is GZipException || ex is EndOfStreamException))
					{
						return 0;
					}
				}
				num = base.Read(buffer, offset, count);
				if (num > 0)
				{
					this.crc.Update(new ArraySegment<byte>(buffer, offset, num));
				}
				if (this.inf.IsFinished)
				{
					this.ReadFooter();
				}
			}
			while (num <= 0 && count != 0);
			return num;
		}

		public string GetFilename()
		{
			return this.fileName;
		}

		private bool ReadHeader()
		{
			this.crc = new Crc32();
			if (this.inputBuffer.Available <= 0)
			{
				this.inputBuffer.Fill();
				if (this.inputBuffer.Available <= 0)
				{
					return false;
				}
			}
			Crc32 crc = new Crc32();
			byte b = this.inputBuffer.ReadLeByte();
			crc.Update((int)b);
			if (b != 31)
			{
				throw new GZipException("Error GZIP header, first magic byte doesn't match");
			}
			b = this.inputBuffer.ReadLeByte();
			if (b != 139)
			{
				throw new GZipException("Error GZIP header,  second magic byte doesn't match");
			}
			crc.Update((int)b);
			byte b2 = this.inputBuffer.ReadLeByte();
			if (b2 != 8)
			{
				throw new GZipException("Error GZIP header, data not in deflate format");
			}
			crc.Update((int)b2);
			byte b3 = this.inputBuffer.ReadLeByte();
			crc.Update((int)b3);
			if ((b3 & 224) != 0)
			{
				throw new GZipException("Reserved flag bits in GZIP header != 0");
			}
			GZipFlags gzipFlags = (GZipFlags)b3;
			for (int i = 0; i < 6; i++)
			{
				crc.Update((int)this.inputBuffer.ReadLeByte());
			}
			if (gzipFlags.HasFlag(GZipFlags.FEXTRA))
			{
				byte b4 = this.inputBuffer.ReadLeByte();
				byte b5 = this.inputBuffer.ReadLeByte();
				crc.Update((int)b4);
				crc.Update((int)b5);
				int num = (int)b5 << 8 | (int)b4;
				for (int j = 0; j < num; j++)
				{
					crc.Update((int)this.inputBuffer.ReadLeByte());
				}
			}
			if (gzipFlags.HasFlag(GZipFlags.FNAME))
			{
				byte[] array = new byte[1024];
				int num2 = 0;
				int num3;
				while ((num3 = (int)this.inputBuffer.ReadLeByte()) > 0)
				{
					if (num2 < 1024)
					{
						array[num2++] = (byte)num3;
					}
					crc.Update(num3);
				}
				crc.Update(num3);
				this.fileName = GZipConstants.Encoding.GetString(array, 0, num2);
			}
			else
			{
				this.fileName = null;
			}
			if (gzipFlags.HasFlag(GZipFlags.FCOMMENT))
			{
				int bval;
				while ((bval = (int)this.inputBuffer.ReadLeByte()) > 0)
				{
					crc.Update(bval);
				}
				crc.Update(bval);
			}
			if (gzipFlags.HasFlag(GZipFlags.FHCRC))
			{
				byte b6 = this.inputBuffer.ReadLeByte();
				if (b6 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				int num4 = (int)this.inputBuffer.ReadLeByte();
				if (num4 < 0)
				{
					throw new EndOfStreamException("EOS reading GZIP header");
				}
				if (((int)b6 << 8 | num4) != ((int)crc.Value & 65535))
				{
					throw new GZipException("Header CRC value mismatch");
				}
			}
			this.readGZIPHeader = true;
			return true;
		}

		private void ReadFooter()
		{
			byte[] array = new byte[8];
			long num = this.inf.TotalOut & (long)((ulong)-1);
			this.inputBuffer.Available += this.inf.RemainingInput;
			this.inf.Reset();
			int num2;
			for (int i = 8; i > 0; i -= num2)
			{
				num2 = this.inputBuffer.ReadClearTextBuffer(array, 8 - i, i);
				if (num2 <= 0)
				{
					throw new EndOfStreamException("EOS reading GZIP footer");
				}
			}
			int num3 = (int)(array[0] & byte.MaxValue) | (int)(array[1] & byte.MaxValue) << 8 | (int)(array[2] & byte.MaxValue) << 16 | (int)array[3] << 24;
			if (num3 != (int)this.crc.Value)
			{
				throw new GZipException("GZIP crc sum mismatch, theirs \"" + num3.ToString() + "\" and ours \"" + ((int)this.crc.Value).ToString());
			}
			uint num4 = (uint)((int)(array[4] & byte.MaxValue) | (int)(array[5] & byte.MaxValue) << 8 | (int)(array[6] & byte.MaxValue) << 16 | (int)array[7] << 24);
			if (num != (long)((ulong)num4))
			{
				throw new GZipException("Number of bytes mismatch in footer");
			}
			this.readGZIPHeader = false;
			this.completedLastBlock = true;
		}

		protected Crc32 crc;

		private bool readGZIPHeader;

		private bool completedLastBlock;

		private string fileName;
	}
}
