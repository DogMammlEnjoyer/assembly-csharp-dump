using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class ChunkStream
	{
		public ChunkStream(WebHeaderCollection headers)
		{
			this._headers = headers;
			this._chunkSize = -1;
			this._chunks = new List<Chunk>();
			this._saved = new StringBuilder();
		}

		internal int Count
		{
			get
			{
				return this._count;
			}
		}

		internal byte[] EndBuffer
		{
			get
			{
				return this._endBuffer;
			}
		}

		internal int Offset
		{
			get
			{
				return this._offset;
			}
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public bool WantsMore
		{
			get
			{
				return this._state < InputChunkState.End;
			}
		}

		private int read(byte[] buffer, int offset, int count)
		{
			int num = 0;
			int count2 = this._chunks.Count;
			for (int i = 0; i < count2; i++)
			{
				Chunk chunk = this._chunks[i];
				bool flag = chunk == null;
				if (!flag)
				{
					bool flag2 = chunk.ReadLeft == 0;
					if (flag2)
					{
						this._chunks[i] = null;
					}
					else
					{
						num += chunk.Read(buffer, offset + num, count - num);
						bool flag3 = num == count;
						if (flag3)
						{
							break;
						}
					}
				}
			}
			return num;
		}

		private InputChunkState seekCrLf(byte[] buffer, ref int offset, int length)
		{
			bool flag = !this._sawCr;
			int num;
			if (flag)
			{
				num = offset;
				offset = num + 1;
				bool flag2 = buffer[num] != 13;
				if (flag2)
				{
					ChunkStream.throwProtocolViolation("CR is expected.");
				}
				this._sawCr = true;
				bool flag3 = offset == length;
				if (flag3)
				{
					return InputChunkState.DataEnded;
				}
			}
			num = offset;
			offset = num + 1;
			bool flag4 = buffer[num] != 10;
			if (flag4)
			{
				ChunkStream.throwProtocolViolation("LF is expected.");
			}
			return InputChunkState.None;
		}

		private InputChunkState setChunkSize(byte[] buffer, ref int offset, int length)
		{
			byte b = 0;
			while (offset < length)
			{
				int num = offset;
				offset = num + 1;
				b = buffer[num];
				bool sawCr = this._sawCr;
				if (sawCr)
				{
					bool flag = b != 10;
					if (flag)
					{
						ChunkStream.throwProtocolViolation("LF is expected.");
					}
					break;
				}
				bool flag2 = b == 13;
				if (flag2)
				{
					this._sawCr = true;
				}
				else
				{
					bool flag3 = b == 10;
					if (flag3)
					{
						ChunkStream.throwProtocolViolation("LF is unexpected.");
					}
					bool gotIt = this._gotIt;
					if (!gotIt)
					{
						bool flag4 = b == 32 || b == 59;
						if (flag4)
						{
							this._gotIt = true;
						}
						else
						{
							this._saved.Append((char)b);
						}
					}
				}
			}
			bool flag5 = this._saved.Length > 20;
			if (flag5)
			{
				ChunkStream.throwProtocolViolation("The chunk size is too big.");
			}
			bool flag6 = b != 10;
			InputChunkState result;
			if (flag6)
			{
				result = InputChunkState.None;
			}
			else
			{
				string s = this._saved.ToString();
				try
				{
					this._chunkSize = int.Parse(s, NumberStyles.HexNumber);
				}
				catch
				{
					ChunkStream.throwProtocolViolation("The chunk size cannot be parsed.");
				}
				this._chunkRead = 0;
				bool flag7 = this._chunkSize == 0;
				if (flag7)
				{
					this._trailerState = 2;
					result = InputChunkState.Trailer;
				}
				else
				{
					result = InputChunkState.Data;
				}
			}
			return result;
		}

		private InputChunkState setTrailer(byte[] buffer, ref int offset, int length)
		{
			while (offset < length)
			{
				bool flag = this._trailerState == 4;
				if (flag)
				{
					break;
				}
				int num = offset;
				offset = num + 1;
				byte b = buffer[num];
				this._saved.Append((char)b);
				bool flag2 = this._trailerState == 1 || this._trailerState == 3;
				if (flag2)
				{
					bool flag3 = b != 10;
					if (flag3)
					{
						ChunkStream.throwProtocolViolation("LF is expected.");
					}
					this._trailerState++;
				}
				else
				{
					bool flag4 = b == 13;
					if (flag4)
					{
						this._trailerState++;
					}
					else
					{
						bool flag5 = b == 10;
						if (flag5)
						{
							ChunkStream.throwProtocolViolation("LF is unexpected.");
						}
						this._trailerState = 0;
					}
				}
			}
			int length2 = this._saved.Length;
			bool flag6 = length2 > 4196;
			if (flag6)
			{
				ChunkStream.throwProtocolViolation("The trailer is too long.");
			}
			bool flag7 = this._trailerState < 4;
			InputChunkState result;
			if (flag7)
			{
				result = InputChunkState.Trailer;
			}
			else
			{
				bool flag8 = length2 == 2;
				if (flag8)
				{
					result = InputChunkState.End;
				}
				else
				{
					this._saved.Length = length2 - 2;
					string s = this._saved.ToString();
					StringReader stringReader = new StringReader(s);
					for (;;)
					{
						string text = stringReader.ReadLine();
						bool flag9 = text == null || text.Length == 0;
						if (flag9)
						{
							break;
						}
						this._headers.Add(text);
					}
					result = InputChunkState.End;
				}
			}
			return result;
		}

		private static void throwProtocolViolation(string message)
		{
			throw new WebException(message, null, WebExceptionStatus.ServerProtocolViolation, null);
		}

		private void write(byte[] buffer, int offset, int length)
		{
			bool flag = this._state == InputChunkState.End;
			if (flag)
			{
				ChunkStream.throwProtocolViolation("The chunks were ended.");
			}
			bool flag2 = this._state == InputChunkState.None;
			if (flag2)
			{
				this._state = this.setChunkSize(buffer, ref offset, length);
				bool flag3 = this._state == InputChunkState.None;
				if (flag3)
				{
					return;
				}
				this._saved.Length = 0;
				this._sawCr = false;
				this._gotIt = false;
			}
			bool flag4 = this._state == InputChunkState.Data;
			if (flag4)
			{
				bool flag5 = offset >= length;
				if (flag5)
				{
					return;
				}
				this._state = this.writeData(buffer, ref offset, length);
				bool flag6 = this._state == InputChunkState.Data;
				if (flag6)
				{
					return;
				}
			}
			bool flag7 = this._state == InputChunkState.DataEnded;
			if (flag7)
			{
				bool flag8 = offset >= length;
				if (flag8)
				{
					return;
				}
				this._state = this.seekCrLf(buffer, ref offset, length);
				bool flag9 = this._state == InputChunkState.DataEnded;
				if (flag9)
				{
					return;
				}
				this._sawCr = false;
			}
			bool flag10 = this._state == InputChunkState.Trailer;
			if (flag10)
			{
				bool flag11 = offset >= length;
				if (flag11)
				{
					return;
				}
				this._state = this.setTrailer(buffer, ref offset, length);
				bool flag12 = this._state == InputChunkState.Trailer;
				if (flag12)
				{
					return;
				}
				this._saved.Length = 0;
			}
			bool flag13 = this._state == InputChunkState.End;
			if (flag13)
			{
				this._endBuffer = buffer;
				this._offset = offset;
				this._count = length - offset;
			}
			else
			{
				bool flag14 = offset >= length;
				if (!flag14)
				{
					this.write(buffer, offset, length);
				}
			}
		}

		private InputChunkState writeData(byte[] buffer, ref int offset, int length)
		{
			int num = length - offset;
			int num2 = this._chunkSize - this._chunkRead;
			bool flag = num > num2;
			if (flag)
			{
				num = num2;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(buffer, offset, array, 0, num);
			Chunk item = new Chunk(array);
			this._chunks.Add(item);
			offset += num;
			this._chunkRead += num;
			return (this._chunkRead == this._chunkSize) ? InputChunkState.DataEnded : InputChunkState.Data;
		}

		internal void ResetChunkStore()
		{
			this._chunkRead = 0;
			this._chunkSize = -1;
			this._chunks.Clear();
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			bool flag = count <= 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = this.read(buffer, offset, count);
			}
			return result;
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			bool flag = count <= 0;
			if (!flag)
			{
				this.write(buffer, offset, offset + count);
			}
		}

		private int _chunkRead;

		private int _chunkSize;

		private List<Chunk> _chunks;

		private int _count;

		private byte[] _endBuffer;

		private bool _gotIt;

		private WebHeaderCollection _headers;

		private int _offset;

		private StringBuilder _saved;

		private bool _sawCr;

		private InputChunkState _state;

		private int _trailerState;
	}
}
