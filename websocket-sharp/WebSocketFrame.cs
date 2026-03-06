using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebSocketSharp
{
	internal class WebSocketFrame : IEnumerable<byte>, IEnumerable
	{
		private WebSocketFrame()
		{
		}

		internal WebSocketFrame(Opcode opcode, PayloadData payloadData, bool mask) : this(Fin.Final, opcode, payloadData, false, mask)
		{
		}

		internal WebSocketFrame(Fin fin, Opcode opcode, byte[] data, bool compressed, bool mask) : this(fin, opcode, new PayloadData(data), compressed, mask)
		{
		}

		internal WebSocketFrame(Fin fin, Opcode opcode, PayloadData payloadData, bool compressed, bool mask)
		{
			this._fin = fin;
			this._opcode = opcode;
			this._rsv1 = ((opcode.IsData() && compressed) ? Rsv.On : Rsv.Off);
			this._rsv2 = Rsv.Off;
			this._rsv3 = Rsv.Off;
			ulong length = payloadData.Length;
			bool flag = length < 126UL;
			if (flag)
			{
				this._payloadLength = (byte)length;
				this._extPayloadLength = WebSocket.EmptyBytes;
			}
			else
			{
				bool flag2 = length < 65536UL;
				if (flag2)
				{
					this._payloadLength = 126;
					this._extPayloadLength = ((ushort)length).ToByteArray(ByteOrder.Big);
				}
				else
				{
					this._payloadLength = 127;
					this._extPayloadLength = length.ToByteArray(ByteOrder.Big);
				}
			}
			if (mask)
			{
				this._mask = Mask.On;
				this._maskingKey = WebSocketFrame.createMaskingKey();
				payloadData.Mask(this._maskingKey);
			}
			else
			{
				this._mask = Mask.Off;
				this._maskingKey = WebSocket.EmptyBytes;
			}
			this._payloadData = payloadData;
		}

		internal ulong ExactPayloadLength
		{
			get
			{
				return (this._payloadLength < 126) ? ((ulong)this._payloadLength) : ((this._payloadLength == 126) ? ((ulong)this._extPayloadLength.ToUInt16(ByteOrder.Big)) : this._extPayloadLength.ToUInt64(ByteOrder.Big));
			}
		}

		internal int ExtendedPayloadLengthWidth
		{
			get
			{
				return (this._payloadLength < 126) ? 0 : ((this._payloadLength == 126) ? 2 : 8);
			}
		}

		public byte[] ExtendedPayloadLength
		{
			get
			{
				return this._extPayloadLength;
			}
		}

		public Fin Fin
		{
			get
			{
				return this._fin;
			}
		}

		public bool IsBinary
		{
			get
			{
				return this._opcode == Opcode.Binary;
			}
		}

		public bool IsClose
		{
			get
			{
				return this._opcode == Opcode.Close;
			}
		}

		public bool IsCompressed
		{
			get
			{
				return this._rsv1 == Rsv.On;
			}
		}

		public bool IsContinuation
		{
			get
			{
				return this._opcode == Opcode.Cont;
			}
		}

		public bool IsControl
		{
			get
			{
				return this._opcode >= Opcode.Close;
			}
		}

		public bool IsData
		{
			get
			{
				return this._opcode == Opcode.Text || this._opcode == Opcode.Binary;
			}
		}

		public bool IsFinal
		{
			get
			{
				return this._fin == Fin.Final;
			}
		}

		public bool IsFragment
		{
			get
			{
				return this._fin == Fin.More || this._opcode == Opcode.Cont;
			}
		}

		public bool IsMasked
		{
			get
			{
				return this._mask == Mask.On;
			}
		}

		public bool IsPing
		{
			get
			{
				return this._opcode == Opcode.Ping;
			}
		}

		public bool IsPong
		{
			get
			{
				return this._opcode == Opcode.Pong;
			}
		}

		public bool IsText
		{
			get
			{
				return this._opcode == Opcode.Text;
			}
		}

		public ulong Length
		{
			get
			{
				return (ulong)(2L + (long)(this._extPayloadLength.Length + this._maskingKey.Length) + (long)this._payloadData.Length);
			}
		}

		public Mask Mask
		{
			get
			{
				return this._mask;
			}
		}

		public byte[] MaskingKey
		{
			get
			{
				return this._maskingKey;
			}
		}

		public Opcode Opcode
		{
			get
			{
				return this._opcode;
			}
		}

		public PayloadData PayloadData
		{
			get
			{
				return this._payloadData;
			}
		}

		public byte PayloadLength
		{
			get
			{
				return this._payloadLength;
			}
		}

		public Rsv Rsv1
		{
			get
			{
				return this._rsv1;
			}
		}

		public Rsv Rsv2
		{
			get
			{
				return this._rsv2;
			}
		}

		public Rsv Rsv3
		{
			get
			{
				return this._rsv3;
			}
		}

		private static byte[] createMaskingKey()
		{
			byte[] array = new byte[4];
			WebSocket.RandomNumber.GetBytes(array);
			return array;
		}

		private static string dump(WebSocketFrame frame)
		{
			ulong length = frame.Length;
			long num = (long)(length / 4UL);
			int num2 = (int)(length % 4UL);
			bool flag = num < 10000L;
			int num3;
			string arg;
			if (flag)
			{
				num3 = 4;
				arg = "{0,4}";
			}
			else
			{
				bool flag2 = num < 65536L;
				if (flag2)
				{
					num3 = 4;
					arg = "{0,4:X}";
				}
				else
				{
					bool flag3 = num < 4294967296L;
					if (flag3)
					{
						num3 = 8;
						arg = "{0,8:X}";
					}
					else
					{
						num3 = 16;
						arg = "{0,16:X}";
					}
				}
			}
			string arg2 = string.Format("{{0,{0}}}", num3);
			string format = string.Format("\r\n{0} 01234567 89ABCDEF 01234567 89ABCDEF\r\n{0}+--------+--------+--------+--------+\\n", arg2);
			string lineFmt = string.Format("{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", arg);
			string format2 = string.Format("{0}+--------+--------+--------+--------+", arg2);
			StringBuilder buff = new StringBuilder(64);
			Func<Action<string, string, string, string>> func = delegate()
			{
				long lineCnt = 0L;
				return delegate(string arg1, string arg2, string arg3, string arg4)
				{
					StringBuilder buff = buff;
					string lineFmt = lineFmt;
					object[] array2 = new object[5];
					int num6 = 0;
					long num7 = lineCnt + 1L;
					lineCnt = num7;
					array2[num6] = num7;
					array2[1] = arg1;
					array2[2] = arg2;
					array2[3] = arg3;
					array2[4] = arg4;
					buff.AppendFormat(lineFmt, array2);
				};
			};
			Action<string, string, string, string> action = func();
			byte[] array = frame.ToArray();
			buff.AppendFormat(format, string.Empty);
			for (long num4 = 0L; num4 <= num; num4 += 1L)
			{
				long num5 = num4 * 4L;
				bool flag4 = num4 < num;
				checked
				{
					if (flag4)
					{
						action(Convert.ToString(array[(int)((IntPtr)num5)], 2).PadLeft(8, '0'), Convert.ToString(array[(int)((IntPtr)(unchecked(num5 + 1L)))], 2).PadLeft(8, '0'), Convert.ToString(array[(int)((IntPtr)(unchecked(num5 + 2L)))], 2).PadLeft(8, '0'), Convert.ToString(array[(int)((IntPtr)(unchecked(num5 + 3L)))], 2).PadLeft(8, '0'));
					}
					else
					{
						bool flag5 = num2 > 0;
						if (flag5)
						{
							action(Convert.ToString(array[(int)((IntPtr)num5)], 2).PadLeft(8, '0'), (num2 >= 2) ? Convert.ToString(array[(int)((IntPtr)(unchecked(num5 + 1L)))], 2).PadLeft(8, '0') : string.Empty, (num2 == 3) ? Convert.ToString(array[(int)((IntPtr)(unchecked(num5 + 2L)))], 2).PadLeft(8, '0') : string.Empty, string.Empty);
						}
					}
				}
			}
			buff.AppendFormat(format2, string.Empty);
			return buff.ToString();
		}

		private static string print(WebSocketFrame frame)
		{
			byte payloadLength = frame._payloadLength;
			string text = (payloadLength > 125) ? frame.ExactPayloadLength.ToString() : string.Empty;
			string text2 = BitConverter.ToString(frame._maskingKey);
			string text3 = (payloadLength == 0) ? string.Empty : ((payloadLength > 125) ? "---" : ((!frame.IsText || frame.IsFragment || frame.IsMasked || frame.IsCompressed) ? frame._payloadData.ToString() : WebSocketFrame.utf8Decode(frame._payloadData.ApplicationData)));
			string format = "\r\n                    FIN: {0}\r\n                   RSV1: {1}\r\n                   RSV2: {2}\r\n                   RSV3: {3}\r\n                 Opcode: {4}\r\n                   MASK: {5}\r\n         Payload Length: {6}\r\nExtended Payload Length: {7}\r\n            Masking Key: {8}\r\n           Payload Data: {9}";
			return string.Format(format, new object[]
			{
				frame._fin,
				frame._rsv1,
				frame._rsv2,
				frame._rsv3,
				frame._opcode,
				frame._mask,
				payloadLength,
				text,
				text2,
				text3
			});
		}

		private static WebSocketFrame processHeader(byte[] header)
		{
			bool flag = header.Length != 2;
			if (flag)
			{
				string message = "The header part of a frame could not be read.";
				throw new WebSocketException(message);
			}
			Fin fin = ((header[0] & 128) == 128) ? Fin.Final : Fin.More;
			Rsv rsv = ((header[0] & 64) == 64) ? Rsv.On : Rsv.Off;
			Rsv rsv2 = ((header[0] & 32) == 32) ? Rsv.On : Rsv.Off;
			Rsv rsv3 = ((header[0] & 16) == 16) ? Rsv.On : Rsv.Off;
			byte opcode = header[0] & 15;
			Mask mask = ((header[1] & 128) == 128) ? Mask.On : Mask.Off;
			byte b = header[1] & 127;
			bool flag2 = !opcode.IsSupported();
			if (flag2)
			{
				string message2 = "A frame has an unsupported opcode.";
				throw new WebSocketException(CloseStatusCode.ProtocolError, message2);
			}
			bool flag3 = !opcode.IsData() && rsv == Rsv.On;
			if (flag3)
			{
				string message3 = "A non data frame is compressed.";
				throw new WebSocketException(CloseStatusCode.ProtocolError, message3);
			}
			bool flag4 = opcode.IsControl();
			if (flag4)
			{
				bool flag5 = fin == Fin.More;
				if (flag5)
				{
					string message4 = "A control frame is fragmented.";
					throw new WebSocketException(CloseStatusCode.ProtocolError, message4);
				}
				bool flag6 = b > 125;
				if (flag6)
				{
					string message5 = "A control frame has too long payload length.";
					throw new WebSocketException(CloseStatusCode.ProtocolError, message5);
				}
			}
			return new WebSocketFrame
			{
				_fin = fin,
				_rsv1 = rsv,
				_rsv2 = rsv2,
				_rsv3 = rsv3,
				_opcode = (Opcode)opcode,
				_mask = mask,
				_payloadLength = b
			};
		}

		private static WebSocketFrame readExtendedPayloadLength(Stream stream, WebSocketFrame frame)
		{
			int extendedPayloadLengthWidth = frame.ExtendedPayloadLengthWidth;
			bool flag = extendedPayloadLengthWidth == 0;
			WebSocketFrame result;
			if (flag)
			{
				frame._extPayloadLength = WebSocket.EmptyBytes;
				result = frame;
			}
			else
			{
				byte[] array = stream.ReadBytes(extendedPayloadLengthWidth);
				bool flag2 = array.Length != extendedPayloadLengthWidth;
				if (flag2)
				{
					string message = "The extended payload length of a frame could not be read.";
					throw new WebSocketException(message);
				}
				frame._extPayloadLength = array;
				result = frame;
			}
			return result;
		}

		private static void readExtendedPayloadLengthAsync(Stream stream, WebSocketFrame frame, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			int len = frame.ExtendedPayloadLengthWidth;
			bool flag = len == 0;
			if (flag)
			{
				frame._extPayloadLength = WebSocket.EmptyBytes;
				completed(frame);
			}
			else
			{
				stream.ReadBytesAsync(len, delegate(byte[] bytes)
				{
					bool flag2 = bytes.Length != len;
					if (flag2)
					{
						string message = "The extended payload length of a frame could not be read.";
						throw new WebSocketException(message);
					}
					frame._extPayloadLength = bytes;
					completed(frame);
				}, error);
			}
		}

		private static WebSocketFrame readHeader(Stream stream)
		{
			byte[] header = stream.ReadBytes(2);
			return WebSocketFrame.processHeader(header);
		}

		private static void readHeaderAsync(Stream stream, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			stream.ReadBytesAsync(2, delegate(byte[] bytes)
			{
				WebSocketFrame obj = WebSocketFrame.processHeader(bytes);
				completed(obj);
			}, error);
		}

		private static WebSocketFrame readMaskingKey(Stream stream, WebSocketFrame frame)
		{
			bool flag = !frame.IsMasked;
			WebSocketFrame result;
			if (flag)
			{
				frame._maskingKey = WebSocket.EmptyBytes;
				result = frame;
			}
			else
			{
				int num = 4;
				byte[] array = stream.ReadBytes(num);
				bool flag2 = array.Length != num;
				if (flag2)
				{
					string message = "The masking key of a frame could not be read.";
					throw new WebSocketException(message);
				}
				frame._maskingKey = array;
				result = frame;
			}
			return result;
		}

		private static void readMaskingKeyAsync(Stream stream, WebSocketFrame frame, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			bool flag = !frame.IsMasked;
			if (flag)
			{
				frame._maskingKey = WebSocket.EmptyBytes;
				completed(frame);
			}
			else
			{
				int len = 4;
				stream.ReadBytesAsync(len, delegate(byte[] bytes)
				{
					bool flag2 = bytes.Length != len;
					if (flag2)
					{
						string message = "The masking key of a frame could not be read.";
						throw new WebSocketException(message);
					}
					frame._maskingKey = bytes;
					completed(frame);
				}, error);
			}
		}

		private static WebSocketFrame readPayloadData(Stream stream, WebSocketFrame frame)
		{
			ulong exactPayloadLength = frame.ExactPayloadLength;
			bool flag = exactPayloadLength > PayloadData.MaxLength;
			if (flag)
			{
				string message = "A frame has too long payload length.";
				throw new WebSocketException(CloseStatusCode.TooBig, message);
			}
			bool flag2 = exactPayloadLength == 0UL;
			WebSocketFrame result;
			if (flag2)
			{
				frame._payloadData = PayloadData.Empty;
				result = frame;
			}
			else
			{
				long num = (long)exactPayloadLength;
				byte[] array = (frame._payloadLength < 127) ? stream.ReadBytes((int)exactPayloadLength) : stream.ReadBytes(num, 1024);
				bool flag3 = (long)array.Length != num;
				if (flag3)
				{
					string message2 = "The payload data of a frame could not be read.";
					throw new WebSocketException(message2);
				}
				frame._payloadData = new PayloadData(array, num);
				result = frame;
			}
			return result;
		}

		private static void readPayloadDataAsync(Stream stream, WebSocketFrame frame, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			ulong exactPayloadLength = frame.ExactPayloadLength;
			bool flag = exactPayloadLength > PayloadData.MaxLength;
			if (flag)
			{
				string message = "A frame has too long payload length.";
				throw new WebSocketException(CloseStatusCode.TooBig, message);
			}
			bool flag2 = exactPayloadLength == 0UL;
			if (flag2)
			{
				frame._payloadData = PayloadData.Empty;
				completed(frame);
			}
			else
			{
				long len = (long)exactPayloadLength;
				Action<byte[]> completed2 = delegate(byte[] bytes)
				{
					bool flag4 = (long)bytes.Length != len;
					if (flag4)
					{
						string message2 = "The payload data of a frame could not be read.";
						throw new WebSocketException(message2);
					}
					frame._payloadData = new PayloadData(bytes, len);
					completed(frame);
				};
				bool flag3 = frame._payloadLength < 127;
				if (flag3)
				{
					stream.ReadBytesAsync((int)exactPayloadLength, completed2, error);
				}
				else
				{
					stream.ReadBytesAsync(len, 1024, completed2, error);
				}
			}
		}

		private static string utf8Decode(byte[] bytes)
		{
			string result;
			try
			{
				result = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		internal static WebSocketFrame CreateCloseFrame(PayloadData payloadData, bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Close, payloadData, false, mask);
		}

		internal static WebSocketFrame CreatePingFrame(bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Ping, PayloadData.Empty, false, mask);
		}

		internal static WebSocketFrame CreatePingFrame(byte[] data, bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Ping, new PayloadData(data), false, mask);
		}

		internal static WebSocketFrame CreatePongFrame(PayloadData payloadData, bool mask)
		{
			return new WebSocketFrame(Fin.Final, Opcode.Pong, payloadData, false, mask);
		}

		internal static WebSocketFrame ReadFrame(Stream stream, bool unmask)
		{
			WebSocketFrame webSocketFrame = WebSocketFrame.readHeader(stream);
			WebSocketFrame.readExtendedPayloadLength(stream, webSocketFrame);
			WebSocketFrame.readMaskingKey(stream, webSocketFrame);
			WebSocketFrame.readPayloadData(stream, webSocketFrame);
			if (unmask)
			{
				webSocketFrame.Unmask();
			}
			return webSocketFrame;
		}

		internal static void ReadFrameAsync(Stream stream, bool unmask, Action<WebSocketFrame> completed, Action<Exception> error)
		{
			Action<WebSocketFrame> <>9__3;
			Action<WebSocketFrame> <>9__2;
			Action<WebSocketFrame> <>9__1;
			WebSocketFrame.readHeaderAsync(stream, delegate(WebSocketFrame frame)
			{
				Stream stream2 = stream;
				Action<WebSocketFrame> completed2;
				if ((completed2 = <>9__1) == null)
				{
					completed2 = (<>9__1 = delegate(WebSocketFrame frame1)
					{
						Stream stream3 = stream;
						Action<WebSocketFrame> completed3;
						if ((completed3 = <>9__2) == null)
						{
							completed3 = (<>9__2 = delegate(WebSocketFrame frame2)
							{
								Stream stream4 = stream;
								Action<WebSocketFrame> completed4;
								if ((completed4 = <>9__3) == null)
								{
									completed4 = (<>9__3 = delegate(WebSocketFrame frame3)
									{
										bool unmask2 = unmask;
										if (unmask2)
										{
											frame3.Unmask();
										}
										completed(frame3);
									});
								}
								WebSocketFrame.readPayloadDataAsync(stream4, frame2, completed4, error);
							});
						}
						WebSocketFrame.readMaskingKeyAsync(stream3, frame1, completed3, error);
					});
				}
				WebSocketFrame.readExtendedPayloadLengthAsync(stream2, frame, completed2, error);
			}, error);
		}

		internal void Unmask()
		{
			bool flag = this._mask == Mask.Off;
			if (!flag)
			{
				this._payloadData.Mask(this._maskingKey);
				this._maskingKey = WebSocket.EmptyBytes;
				this._mask = Mask.Off;
			}
		}

		public IEnumerator<byte> GetEnumerator()
		{
			foreach (byte b in this.ToArray())
			{
				yield return b;
			}
			byte[] array = null;
			yield break;
		}

		public void Print(bool dumped)
		{
			string value = dumped ? WebSocketFrame.dump(this) : WebSocketFrame.print(this);
			Console.WriteLine(value);
		}

		public string PrintToString(bool dumped)
		{
			return dumped ? WebSocketFrame.dump(this) : WebSocketFrame.print(this);
		}

		public byte[] ToArray()
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int num = (int)this._fin;
				num = (int)((byte)(num << 1) + this._rsv1);
				num = (int)((byte)(num << 1) + this._rsv2);
				num = (int)((byte)(num << 1) + this._rsv3);
				num = (int)((byte)(num << 4) + this._opcode);
				num = (int)((byte)(num << 1) + this._mask);
				num = (num << 7) + (int)this._payloadLength;
				ushort value = (ushort)num;
				byte[] buffer = value.ToByteArray(ByteOrder.Big);
				memoryStream.Write(buffer, 0, 2);
				bool flag = this._payloadLength > 125;
				if (flag)
				{
					int count = (this._payloadLength == 126) ? 2 : 8;
					memoryStream.Write(this._extPayloadLength, 0, count);
				}
				bool flag2 = this._mask == Mask.On;
				if (flag2)
				{
					memoryStream.Write(this._maskingKey, 0, 4);
				}
				bool flag3 = this._payloadLength > 0;
				if (flag3)
				{
					byte[] array = this._payloadData.ToArray();
					bool flag4 = this._payloadLength < 127;
					if (flag4)
					{
						memoryStream.Write(array, 0, array.Length);
					}
					else
					{
						memoryStream.WriteBytes(array, 1024);
					}
				}
				memoryStream.Close();
				result = memoryStream.ToArray();
			}
			return result;
		}

		public override string ToString()
		{
			byte[] value = this.ToArray();
			return BitConverter.ToString(value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private byte[] _extPayloadLength;

		private Fin _fin;

		private Mask _mask;

		private byte[] _maskingKey;

		private Opcode _opcode;

		private PayloadData _payloadData;

		private byte _payloadLength;

		private Rsv _rsv1;

		private Rsv _rsv2;

		private Rsv _rsv3;
	}
}
