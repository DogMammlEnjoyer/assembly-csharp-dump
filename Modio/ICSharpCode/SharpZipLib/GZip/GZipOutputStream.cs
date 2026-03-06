using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace ICSharpCode.SharpZipLib.GZip
{
	public class GZipOutputStream : DeflaterOutputStream
	{
		public GZipOutputStream(Stream baseOutputStream) : this(baseOutputStream, 4096)
		{
		}

		public GZipOutputStream(Stream baseOutputStream, int size) : base(baseOutputStream, new Deflater(-1, true), size)
		{
		}

		public void SetLevel(int level)
		{
			if (level < 0 || level > 9)
			{
				throw new ArgumentOutOfRangeException("level", "Compression level must be 0-9");
			}
			this.deflater_.SetLevel(level);
		}

		public int GetLevel()
		{
			return this.deflater_.GetLevel();
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = GZipOutputStream.CleanFilename(value);
				if (string.IsNullOrEmpty(this.fileName))
				{
					this.flags &= ~GZipFlags.FNAME;
					return;
				}
				this.flags |= GZipFlags.FNAME;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.state_ == GZipOutputStream.OutputState.Header)
			{
				this.WriteHeader();
			}
			if (this.state_ != GZipOutputStream.OutputState.Footer)
			{
				throw new InvalidOperationException("Write not permitted in current state");
			}
			this.crc.Update(new ArraySegment<byte>(buffer, offset, count));
			base.Write(buffer, offset, count);
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				this.Finish();
			}
			finally
			{
				if (this.state_ != GZipOutputStream.OutputState.Closed)
				{
					this.state_ = GZipOutputStream.OutputState.Closed;
					if (base.IsStreamOwner)
					{
						this.baseOutputStream_.Dispose();
					}
				}
			}
		}

		public override void Flush()
		{
			if (this.state_ == GZipOutputStream.OutputState.Header)
			{
				this.WriteHeader();
			}
			base.Flush();
		}

		public override void Finish()
		{
			if (this.state_ == GZipOutputStream.OutputState.Header)
			{
				this.WriteHeader();
			}
			if (this.state_ == GZipOutputStream.OutputState.Footer)
			{
				this.state_ = GZipOutputStream.OutputState.Finished;
				base.Finish();
				uint num = (uint)(this.deflater_.TotalIn & (long)((ulong)-1));
				uint num2 = (uint)(this.crc.Value & (long)((ulong)-1));
				byte[] array = new byte[]
				{
					(byte)num2,
					(byte)(num2 >> 8),
					(byte)(num2 >> 16),
					(byte)(num2 >> 24),
					(byte)num,
					(byte)(num >> 8),
					(byte)(num >> 16),
					(byte)(num >> 24)
				};
				this.baseOutputStream_.Write(array, 0, array.Length);
			}
		}

		private static string CleanFilename(string path)
		{
			return path.Substring(path.LastIndexOf('/') + 1);
		}

		private void WriteHeader()
		{
			if (this.state_ == GZipOutputStream.OutputState.Header)
			{
				this.state_ = GZipOutputStream.OutputState.Footer;
				int num = (int)((DateTime.Now.Ticks - new DateTime(1970, 1, 1).Ticks) / 10000000L);
				byte[] array = new byte[]
				{
					31,
					139,
					8,
					0,
					0,
					0,
					0,
					0,
					0,
					byte.MaxValue
				};
				array[3] = this.flags;
				array[4] = (byte)num;
				array[5] = (byte)(num >> 8);
				array[6] = (byte)(num >> 16);
				array[7] = (byte)(num >> 24);
				byte[] array2 = array;
				this.baseOutputStream_.Write(array2, 0, array2.Length);
				if (this.flags.HasFlag(GZipFlags.FNAME))
				{
					byte[] bytes = GZipConstants.Encoding.GetBytes(this.fileName);
					this.baseOutputStream_.Write(bytes, 0, bytes.Length);
					this.baseOutputStream_.Write(new byte[1], 0, 1);
				}
			}
		}

		protected Crc32 crc = new Crc32();

		private GZipOutputStream.OutputState state_;

		private string fileName;

		private GZipFlags flags;

		private enum OutputState
		{
			Header,
			Footer,
			Finished,
			Closed
		}
	}
}
