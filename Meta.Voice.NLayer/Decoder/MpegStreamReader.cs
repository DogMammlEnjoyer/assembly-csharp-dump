using System;
using System.IO;
using System.Threading.Tasks;

namespace Meta.Voice.NLayer.Decoder
{
	internal class MpegStreamReader
	{
		internal MpegStreamReader(Stream source)
		{
			this._source = source;
			this._canSeek = source.CanSeek;
			this._readOffset = 0L;
			this._eofOffset = long.MaxValue;
			FrameBase frameBase = this.FindNextFrame();
			while (frameBase != null && !(frameBase is MpegFrame))
			{
				frameBase = this.FindNextFrame();
			}
			if (frameBase == null)
			{
				throw new InvalidDataException("Not a valid MPEG file!");
			}
			frameBase = this.FindNextFrame();
			if (frameBase == null || !(frameBase is MpegFrame))
			{
				throw new InvalidDataException("Not a valid MPEG file!");
			}
			this._current = this._first;
		}

		private FrameBase FindNextFrame()
		{
			if (this._endFound)
			{
				return null;
			}
			MpegFrame lastFree = this._lastFree;
			long num = this._readOffset;
			object frameLock = this._frameLock;
			FrameBase result;
			lock (frameLock)
			{
				byte[] array = new byte[4];
				try
				{
					if (this.Read(this._readOffset, array, 0, 4) == 4)
					{
						ID3Frame id3Frame;
						RiffHeaderFrame riffHeaderFrame;
						MpegFrame mpegFrame;
						ID3Frame id3Frame2;
						for (;;)
						{
							uint syncMark = (uint)((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
							num = this._readOffset;
							if (this._id3Frame == null)
							{
								id3Frame = ID3Frame.TrySync(syncMark);
								if (id3Frame != null && id3Frame.Validate(this._readOffset, this))
								{
									break;
								}
							}
							if (this._first == null && this._riffHeaderFrame == null)
							{
								riffHeaderFrame = RiffHeaderFrame.TrySync(syncMark);
								if (riffHeaderFrame != null && riffHeaderFrame.Validate(this._readOffset, this))
								{
									goto Block_13;
								}
							}
							mpegFrame = MpegFrame.TrySync(syncMark);
							if (mpegFrame != null && mpegFrame.Validate(this._readOffset, this) && (lastFree == null || (mpegFrame.Layer == lastFree.Layer && mpegFrame.Version == lastFree.Version && mpegFrame.SampleRate == lastFree.SampleRate && mpegFrame.BitRateIndex <= 0)))
							{
								goto IL_1A1;
							}
							if (this._last != null)
							{
								id3Frame2 = ID3Frame.TrySync(syncMark);
								if (id3Frame2 != null && id3Frame2.Validate(this._readOffset, this))
								{
									goto Block_28;
								}
							}
							this._readOffset += 1L;
							if (this._first == null || !this._canSeek)
							{
								this.DiscardThrough(this._readOffset, true);
							}
							Buffer.BlockCopy(array, 1, array, 0, 3);
							if (this.Read(this._readOffset + 3L, array, 3, 1) != 1)
							{
								goto IL_37A;
							}
						}
						if (!this._canSeek)
						{
							id3Frame.SaveBuffer();
						}
						this._readOffset += (long)id3Frame.Length;
						this.DiscardThrough(this._readOffset, true);
						return this._id3Frame = id3Frame;
						Block_13:
						this._readOffset += (long)riffHeaderFrame.Length;
						this.DiscardThrough(this._readOffset, true);
						return this._riffHeaderFrame = riffHeaderFrame;
						IL_1A1:
						if (!this._canSeek)
						{
							mpegFrame.SaveBuffer();
							this.DiscardThrough(this._readOffset + (long)mpegFrame.FrameLength, true);
						}
						this._readOffset += (long)mpegFrame.FrameLength;
						if (this._first == null)
						{
							if (this._vbrInfo == null && (this._vbrInfo = mpegFrame.ParseVBR()) != null)
							{
								return this.FindNextFrame();
							}
							mpegFrame.Number = 0;
							this._first = (this._last = mpegFrame);
						}
						else
						{
							if (mpegFrame.SampleCount != this._first.SampleCount)
							{
								this._mixedFrameSize = true;
							}
							mpegFrame.SampleOffset = (long)this._last.SampleCount + this._last.SampleOffset;
							mpegFrame.Number = this._last.Number + 1;
							this._last = (this._last.Next = mpegFrame);
						}
						if (mpegFrame.BitRateIndex == 0)
						{
							this._lastFree = mpegFrame;
						}
						return mpegFrame;
						Block_28:
						if (!this._canSeek)
						{
							id3Frame2.SaveBuffer();
						}
						if (id3Frame2.Version == 1)
						{
							this._id3v1Frame = id3Frame2;
						}
						else
						{
							this._id3Frame.Merge(id3Frame2);
						}
						this._readOffset += (long)id3Frame2.Length;
						this.DiscardThrough(this._readOffset, true);
						return id3Frame2;
					}
					IL_37A:
					num += 4L;
					this._endFound = true;
					result = null;
				}
				finally
				{
					if (lastFree != null)
					{
						lastFree.Length = (int)(num - lastFree.Offset);
						if (!this._canSeek)
						{
							throw new InvalidOperationException("Free frames cannot be read properly from forward-only streams!");
						}
						if (this._lastFree == lastFree)
						{
							this._lastFree = null;
						}
					}
				}
			}
			return result;
		}

		internal int Read(long offset, byte[] buffer, int index, int count)
		{
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (index < 0 || index + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this._readBuf.Read(this, offset, buffer, index, count);
		}

		internal int ReadByte(long offset)
		{
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			return this._readBuf.ReadByte(this, offset);
		}

		internal void DiscardThrough(long offset, bool minimalRead)
		{
			this._readBuf.DiscardThrough(offset);
		}

		internal void ReadToEnd()
		{
			try
			{
				int num = 40000;
				if (this._id3Frame != null)
				{
					num += this._id3Frame.Length;
				}
				while (!this._endFound)
				{
					this.FindNextFrame();
					while (!this._canSeek && FrameBase.TotalAllocation >= num)
					{
						Task.Delay(500).Wait();
					}
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		internal bool CanSeek
		{
			get
			{
				return this._canSeek;
			}
		}

		internal long SampleCount
		{
			get
			{
				if (this._vbrInfo != null)
				{
					return this._vbrInfo.VBRStreamSampleCount;
				}
				if (!this._canSeek)
				{
					return -1L;
				}
				this.ReadToEnd();
				return (long)this._last.SampleCount + this._last.SampleOffset;
			}
		}

		internal int SampleRate
		{
			get
			{
				if (this._vbrInfo != null)
				{
					return this._vbrInfo.SampleRate;
				}
				return this._first.SampleRate;
			}
		}

		internal int Channels
		{
			get
			{
				if (this._vbrInfo != null)
				{
					return this._vbrInfo.Channels;
				}
				return this._first.Channels;
			}
		}

		internal int FirstFrameSampleCount
		{
			get
			{
				if (this._first == null)
				{
					return 0;
				}
				return this._first.SampleCount;
			}
		}

		internal long SeekTo(long sampleNumber)
		{
			if (!this._canSeek)
			{
				throw new InvalidOperationException("Cannot seek!");
			}
			int num = (int)(sampleNumber / (long)this._first.SampleCount);
			MpegFrame mpegFrame = this._first;
			if (this._current != null && this._current.Number <= num && this._current.SampleOffset <= sampleNumber)
			{
				mpegFrame = this._current;
				num -= mpegFrame.Number;
			}
			while (!this._mixedFrameSize && --num >= 0)
			{
				if (mpegFrame == null)
				{
					break;
				}
				if (mpegFrame == this._last && !this._endFound)
				{
					do
					{
						this.FindNextFrame();
					}
					while (mpegFrame == this._last && !this._endFound);
				}
				if (this._mixedFrameSize)
				{
					break;
				}
				mpegFrame = mpegFrame.Next;
			}
			while (mpegFrame != null && mpegFrame.SampleOffset + (long)mpegFrame.SampleCount < sampleNumber)
			{
				if (mpegFrame == this._last && !this._endFound)
				{
					do
					{
						this.FindNextFrame();
					}
					while (mpegFrame == this._last && !this._endFound);
				}
				mpegFrame = mpegFrame.Next;
			}
			if (mpegFrame == null)
			{
				return -1L;
			}
			return (this._current = mpegFrame).SampleOffset;
		}

		internal MpegFrame NextFrame()
		{
			MpegFrame current = this._current;
			if (current != null)
			{
				if (this._canSeek)
				{
					current.SaveBuffer();
					this.DiscardThrough(current.Offset + (long)current.FrameLength, false);
				}
				if (current == this._last && !this._endFound)
				{
					do
					{
						this.FindNextFrame();
					}
					while (current == this._last && !this._endFound);
				}
				this._current = current.Next;
				if (!this._canSeek)
				{
					object frameLock = this._frameLock;
					lock (frameLock)
					{
						MpegFrame first = this._first;
						this._first = first.Next;
						first.Next = null;
					}
				}
			}
			return current;
		}

		internal MpegFrame GetCurrentFrame()
		{
			return this._current;
		}

		private ID3Frame _id3Frame;

		private ID3Frame _id3v1Frame;

		private RiffHeaderFrame _riffHeaderFrame;

		private VBRInfo _vbrInfo;

		private MpegFrame _first;

		private MpegFrame _current;

		private MpegFrame _last;

		private MpegFrame _lastFree;

		private long _readOffset;

		private long _eofOffset;

		private Stream _source;

		private bool _canSeek;

		private bool _endFound;

		private bool _mixedFrameSize;

		private object _readLock = new object();

		private object _frameLock = new object();

		private MpegStreamReader.ReadBuffer _readBuf = new MpegStreamReader.ReadBuffer(2048);

		private class ReadBuffer
		{
			public ReadBuffer(int initialSize)
			{
				initialSize = 2 << (int)Math.Log((double)initialSize, 2.0);
				this.Data = new byte[initialSize];
			}

			public int Read(MpegStreamReader reader, long offset, byte[] buffer, int index, int count)
			{
				object localLock = this._localLock;
				lock (localLock)
				{
					int srcOffset = this.EnsureFilled(reader, offset, ref count);
					Buffer.BlockCopy(this.Data, srcOffset, buffer, index, count);
				}
				return count;
			}

			public int ReadByte(MpegStreamReader reader, long offset)
			{
				object localLock = this._localLock;
				lock (localLock)
				{
					int num = 1;
					int num2 = this.EnsureFilled(reader, offset, ref num);
					if (num == 1)
					{
						return (int)this.Data[num2];
					}
				}
				return -1;
			}

			private int EnsureFilled(MpegStreamReader reader, long offset, ref int count)
			{
				int num = (int)(offset - this.BaseOffset);
				int num2 = num + count;
				if (num < 0 || num2 > this.End)
				{
					int num3 = 0;
					int num4 = 0;
					int num5 = 0;
					long num6 = 0L;
					if (num < 0)
					{
						if (!reader._source.CanSeek)
						{
							throw new InvalidOperationException("Cannot seek backwards on a forward-only stream!");
						}
						if (this.End > 0 && (num + this.Data.Length > 0 || (this.Data.Length * 2 <= 16384 && num + this.Data.Length * 2 > 0)))
						{
							num2 = this.End;
						}
						num6 = offset;
						if (num2 < 0)
						{
							this.Truncate();
							this.BaseOffset = offset;
							num = 0;
							num2 = count;
							num4 = count;
						}
						else
						{
							num5 = -num2;
							num4 = -num;
						}
					}
					else if (num2 < this.Data.Length)
					{
						num4 = num2 - this.End;
						num3 = this.End;
						num6 = this.BaseOffset + (long)num3;
					}
					else if (num2 - this.DiscardCount < this.Data.Length)
					{
						num5 = this.DiscardCount;
						num3 = this.End;
						num4 = num2 - num3;
						num6 = this.BaseOffset + (long)num3;
					}
					else if (this.Data.Length * 2 <= 16384)
					{
						num5 = this.DiscardCount;
						num3 = this.End;
						num4 = num2 - this.End;
						num6 = this.BaseOffset + (long)num3;
					}
					else
					{
						this.Truncate();
						this.BaseOffset = offset;
						num6 = offset;
						num = 0;
						num2 = count;
						num4 = count;
					}
					if (num2 - num5 > this.Data.Length || num3 + num4 - num5 > this.Data.Length)
					{
						int i;
						for (i = this.Data.Length * 2; i < num2 - num5; i *= 2)
						{
						}
						byte[] array = new byte[i];
						if (num5 < 0)
						{
							Buffer.BlockCopy(this.Data, 0, array, -num5, this.End + num5);
							this.DiscardCount = 0;
						}
						else
						{
							Buffer.BlockCopy(this.Data, num5, array, 0, this.End - num5);
							this.DiscardCount -= num5;
						}
						this.Data = array;
					}
					else if (num5 != 0)
					{
						if (num5 > 0)
						{
							Buffer.BlockCopy(this.Data, num5, this.Data, 0, this.End - num5);
							this.DiscardCount -= num5;
						}
						else
						{
							int j = 0;
							int num7 = this.Data.Length - 1;
							int num8 = this.Data.Length - 1 - num5;
							while (j < num5)
							{
								this.Data[num8] = this.Data[num7];
								j++;
								num7--;
								num8--;
							}
							this.DiscardCount = 0;
						}
					}
					this.BaseOffset += (long)num5;
					num3 -= num5;
					num -= num5;
					num2 -= num5;
					this.End -= num5;
					object readLock = reader._readLock;
					lock (readLock)
					{
						if (num4 > 0 && reader._source.Position != num6 && num6 < reader._eofOffset)
						{
							if (reader._canSeek)
							{
								try
								{
									reader._source.Position = num6;
									goto IL_392;
								}
								catch (EndOfStreamException)
								{
									reader._eofOffset = reader._source.Length;
									num4 = 0;
									goto IL_392;
								}
							}
							long num9 = num6 - reader._source.Position;
							while ((num9 -= 1L) >= 0L)
							{
								if (reader._source.ReadByte() == -1)
								{
									reader._eofOffset = reader._source.Position;
									num4 = 0;
									break;
								}
							}
						}
						IL_392:
						while (num4 > 0 && num6 < reader._eofOffset)
						{
							int num10 = reader._source.Read(this.Data, num3, num4);
							if (num10 == 0)
							{
								break;
							}
							num3 += num10;
							num6 += (long)num10;
							num4 -= num10;
						}
						if (num3 > this.End)
						{
							this.End = num3;
						}
						if (this.End < num2)
						{
							count = Math.Max(0, this.End - num);
						}
						else if (this.End < this.Data.Length)
						{
							int num11 = reader._source.Read(this.Data, this.End, this.Data.Length - this.End);
							this.End += num11;
						}
					}
				}
				return num;
			}

			public void DiscardThrough(long offset)
			{
				object localLock = this._localLock;
				lock (localLock)
				{
					int val = (int)(offset - this.BaseOffset);
					this.DiscardCount = Math.Max(val, this.DiscardCount);
					if (this.DiscardCount >= this.Data.Length)
					{
						this.CommitDiscard();
					}
				}
			}

			private void Truncate()
			{
				this.End = 0;
				this.DiscardCount = 0;
			}

			private void CommitDiscard()
			{
				if (this.DiscardCount >= this.Data.Length || this.DiscardCount >= this.End)
				{
					this.BaseOffset += (long)this.DiscardCount;
					this.End = 0;
				}
				else
				{
					Buffer.BlockCopy(this.Data, this.DiscardCount, this.Data, 0, this.End - this.DiscardCount);
					this.BaseOffset += (long)this.DiscardCount;
					this.End -= this.DiscardCount;
				}
				this.DiscardCount = 0;
			}

			public byte[] Data;

			public long BaseOffset;

			public int End;

			public int DiscardCount;

			private object _localLock = new object();
		}
	}
}
