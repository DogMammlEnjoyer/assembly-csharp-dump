using System;
using System.IO;
using System.Text;

namespace CSCore.Codecs.WAV
{
	public class WaveWriter : IDisposable, IWriteable
	{
		public bool IsDisposed
		{
			get
			{
				return this._isDisposed;
			}
		}

		public bool IsDisposing
		{
			get
			{
				return this._isDisposing;
			}
		}

		public WaveWriter(string fileName, WaveFormat waveFormat) : this(File.OpenWrite(fileName), waveFormat)
		{
			this._closeStream = true;
		}

		public WaveWriter(Stream stream, WaveFormat waveFormat)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("Stream not writeable.", "stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("Stream not seekable.", "stream");
			}
			this._isDisposing = false;
			this._isDisposed = false;
			this._stream = stream;
			this._waveStartPosition = stream.Position;
			this._writer = new BinaryWriter(stream);
			for (int i = 0; i < 44; i++)
			{
				this._writer.Write(0);
			}
			this._waveFormat = waveFormat;
			this.WriteHeader();
			this._closeStream = false;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[Obsolete("Use the Extensions.WriteToWaveStream extension instead.")]
		public static void WriteToFile(string filename, IWaveSource source, bool deleteFileIfAlreadyExists, int maxlength = -1)
		{
			if (deleteFileIfAlreadyExists && File.Exists(filename))
			{
				File.Delete(filename);
			}
			int num = 0;
			byte[] array = new byte[source.WaveFormat.BytesPerSecond];
			using (WaveWriter waveWriter = new WaveWriter(filename, source.WaveFormat))
			{
				int num2;
				while ((num2 = source.Read(array, 0, array.Length)) > 0)
				{
					waveWriter.Write(array, 0, num2);
					num += num2;
					if (maxlength != -1 && num > maxlength)
					{
						break;
					}
				}
			}
		}

		public void WriteSample(float sample)
		{
			this.CheckObjectDisposed();
			if (sample < -1f || sample > 1f)
			{
				sample = Math.Max(-1f, Math.Min(1f, sample));
			}
			if (this._waveFormat.IsPCM())
			{
				int bitsPerSample = this._waveFormat.BitsPerSample;
				if (bitsPerSample <= 16)
				{
					if (bitsPerSample == 8)
					{
						this.Write((byte)(255f * sample));
						return;
					}
					if (bitsPerSample == 16)
					{
						this.Write((short)(32767f * sample));
						return;
					}
				}
				else
				{
					if (bitsPerSample == 24)
					{
						byte[] bytes = BitConverter.GetBytes((int)(8388607f * sample));
						this.Write(new byte[]
						{
							bytes[0],
							bytes[1],
							bytes[2]
						}, 0, 3);
						return;
					}
					if (bitsPerSample == 32)
					{
						this.Write((int)(2.1474836E+09f * sample));
						return;
					}
				}
				throw new InvalidOperationException("Invalid Waveformat", new InvalidOperationException("Invalid BitsPerSample while using PCM encoding."));
			}
			if (this._waveFormat.IsIeeeFloat())
			{
				this.Write(sample);
				return;
			}
			if (this._waveFormat.WaveFormatTag == AudioEncoding.Extensible && this._waveFormat.BitsPerSample == 32)
			{
				this.Write(65535 * (int)sample);
				return;
			}
			throw new InvalidOperationException("Invalid Waveformat: Waveformat has to be PCM[8, 16, 24, 32] or IeeeFloat[32]");
		}

		public void WriteSamples(float[] samples, int offset, int count)
		{
			this.CheckObjectDisposed();
			for (int i = offset; i < offset + count; i++)
			{
				this.WriteSample(samples[i]);
			}
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			this.CheckObjectDisposed();
			this._stream.Write(buffer, offset, count);
			this._dataLength += count;
		}

		public void Write(byte value)
		{
			this.CheckObjectDisposed();
			this._writer.Write(value);
			this._dataLength++;
		}

		public void Write(short value)
		{
			this.CheckObjectDisposed();
			this._writer.Write(value);
			this._dataLength += 2;
		}

		public void Write(int value)
		{
			this.CheckObjectDisposed();
			this._writer.Write(value);
			this._dataLength += 4;
		}

		public void Write(float value)
		{
			this.CheckObjectDisposed();
			this._writer.Write(value);
			this._dataLength += 4;
		}

		private void WriteHeader()
		{
			this._writer.Flush();
			long position = this._stream.Position;
			this._stream.Position = this._waveStartPosition;
			this.WriteRiffHeader();
			this.WriteFmtChunk();
			this.WriteDataChunk();
			this._writer.Flush();
			this._stream.Position = position;
		}

		private void WriteRiffHeader()
		{
			this._writer.Write(Encoding.UTF8.GetBytes("RIFF"));
			this._writer.Write((int)(this._stream.Length - 8L));
			this._writer.Write(Encoding.UTF8.GetBytes("WAVE"));
		}

		private void WriteFmtChunk()
		{
			AudioEncoding audioEncoding = this._waveFormat.WaveFormatTag;
			if (audioEncoding == AudioEncoding.Extensible && this._waveFormat is WaveFormatExtensible)
			{
				audioEncoding = AudioSubTypes.EncodingFromSubType((this._waveFormat as WaveFormatExtensible).SubFormat);
			}
			this._writer.Write(Encoding.UTF8.GetBytes("fmt "));
			this._writer.Write(16);
			this._writer.Write((short)audioEncoding);
			this._writer.Write((short)this._waveFormat.Channels);
			this._writer.Write(this._waveFormat.SampleRate);
			this._writer.Write(this._waveFormat.BytesPerSecond);
			this._writer.Write((short)this._waveFormat.BlockAlign);
			this._writer.Write((short)this._waveFormat.BitsPerSample);
		}

		private void WriteDataChunk()
		{
			this._writer.Write(Encoding.UTF8.GetBytes("data"));
			this._writer.Write(this._dataLength);
		}

		private void CheckObjectDisposed()
		{
			if (this._isDisposed)
			{
				throw new ObjectDisposedException("WaveWriter");
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._isDisposed)
			{
				return;
			}
			if (!disposing)
			{
				return;
			}
			try
			{
				this._isDisposing = true;
				this.WriteHeader();
			}
			catch (Exception)
			{
			}
			finally
			{
				if (this._closeStream)
				{
					if (this._writer != null)
					{
						this._writer.Close();
						this._writer = null;
					}
					if (this._stream != null)
					{
						this._stream.Dispose();
						this._stream = null;
					}
				}
				this._isDisposing = false;
			}
			this._isDisposed = true;
		}

		~WaveWriter()
		{
			this.Dispose(false);
		}

		private readonly WaveFormat _waveFormat;

		private readonly long _waveStartPosition;

		private int _dataLength;

		private bool _isDisposed;

		private Stream _stream;

		private BinaryWriter _writer;

		private bool _isDisposing;

		private readonly bool _closeStream;
	}
}
