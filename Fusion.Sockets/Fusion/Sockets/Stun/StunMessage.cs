using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Fusion.Sockets.Stun
{
	internal class StunMessage
	{
		private static HashSet<int> StunMessageTypeValues
		{
			get
			{
				bool flag = StunMessage._stunMessageTypeValues == null;
				if (flag)
				{
					StunMessage._stunMessageTypeValues = new HashSet<int>();
					foreach (object obj in Enum.GetValues(typeof(StunMessage.StunMessageType)))
					{
						StunMessage._stunMessageTypeValues.Add((int)obj);
					}
				}
				return StunMessage._stunMessageTypeValues;
			}
		}

		public StunMessage.StunMessageType Type { get; private set; }

		public Guid ID
		{
			get
			{
				bool flag = this._id.Equals(Guid.Empty);
				if (flag)
				{
					byte[] array = new byte[16];
					Array.Copy(this.TransactionID, array, 12);
					this._id = new Guid(array);
				}
				return this._id;
			}
		}

		private byte[] TransactionID { get; set; }

		public IPEndPoint MappedAddress
		{
			get
			{
				object obj;
				bool flag = this.Attributes.TryGetValue(StunMessage.AttributeType.MappedAddress, out obj);
				IPEndPoint result;
				if (flag)
				{
					result = (obj as IPEndPoint);
				}
				else
				{
					result = null;
				}
				return result;
			}
			set
			{
				this.Attributes[StunMessage.AttributeType.MappedAddress] = value;
			}
		}

		public string UserName { get; set; } = null;

		public StunErrorAttribute ErrorCode { get; set; } = null;

		private Dictionary<StunMessage.AttributeType, object> Attributes { get; set; }

		public StunMessage(Guid msgID, StunMessage.StunMessageType messageType = StunMessage.StunMessageType.BindingRequest)
		{
			this.Type = messageType;
			this.TransactionID = new byte[12];
			Array.Copy(msgID.Equals(Guid.Empty) ? Guid.NewGuid().ToByteArray() : msgID.ToByteArray(), this.TransactionID, 12);
			this.Attributes = new Dictionary<StunMessage.AttributeType, object>();
		}

		public unsafe static bool IsStunMessage(byte* data, int length)
		{
			bool flag = length <= 0 || length < 20;
			bool result;
			if (flag)
			{
				TraceLogStream logTraceStun = InternalLogStreams.LogTraceStun;
				if (logTraceStun != null)
				{
					logTraceStun.Log("Invalid STUN Message Size");
				}
				result = false;
			}
			else
			{
				int num = (int)(*data) << 8 | (int)data[1];
				int num2 = (int)data[4] << 24 | (int)data[5] << 16 | (int)data[6] << 8 | (int)data[7];
				bool flag2 = num2 == 554869826 && (num & 49152) == 0 && StunMessage.StunMessageTypeValues.Contains(num);
				TraceLogStream logTraceStun2 = InternalLogStreams.LogTraceStun;
				if (logTraceStun2 != null)
				{
					logTraceStun2.Log(string.Format("STUN Message Type: {0}, Magic Cookie: {1}, Result: {2}", num, num2, flag2));
				}
				result = flag2;
			}
			return result;
		}

		public unsafe static StunMessage TryParse(byte* data, int length)
		{
			bool flag = length <= 0 || length < 20;
			StunMessage result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int num = 0;
				int num2 = (int)data[num++] << 8 | (int)data[num++];
				bool flag2 = (num2 & 49152) == 0 && Enum.IsDefined(typeof(StunMessage.StunMessageType), num2);
				if (flag2)
				{
					StunMessage.StunMessageType type = (StunMessage.StunMessageType)num2;
					int num3 = (int)data[num++] << 8 | (int)data[num++];
					int num4 = (int)data[num++] << 24 | (int)data[num++] << 16 | (int)data[num++] << 8 | (int)data[num++];
					bool flag3 = num4 != 554869826;
					if (flag3)
					{
						result = null;
					}
					else
					{
						StunMessage stunMessage = new StunMessage(Guid.Empty, StunMessage.StunMessageType.BindingRequest)
						{
							Type = type,
							TransactionID = new byte[12]
						};
						for (int i = 0; i < 12; i++)
						{
							stunMessage.TransactionID[i] = data[num++];
						}
						while (num - 20 < num3)
						{
							stunMessage.ReadAttribute(data, ref num);
						}
						result = stunMessage;
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public byte[] Serialize()
		{
			byte[] array = new byte[512];
			int num = 0;
			array[num++] = (byte)(this.Type >> 8);
			array[num++] = (byte)(this.Type & (StunMessage.StunMessageType)255);
			int num2 = num;
			array[num++] = 0;
			array[num++] = 0;
			array[num++] = 33;
			array[num++] = 18;
			array[num++] = 164;
			array[num++] = 66;
			Array.Copy(this.TransactionID, 0, array, num, 12);
			num += 12;
			this.WriteAttributes(array, ref num);
			int num3 = num - 20;
			array[num2++] = (byte)(num3 >> 8);
			array[num2++] = (byte)(num3 & 255);
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, array2.Length);
			return array2;
		}

		private void WriteAttributes(byte[] msg, ref int offset)
		{
			foreach (KeyValuePair<StunMessage.AttributeType, object> keyValuePair in this.Attributes)
			{
				StunMessage.AttributeType key = keyValuePair.Key;
				StunMessage.AttributeType attributeType = key;
				if (attributeType <= StunMessage.AttributeType.UnknownAttribute)
				{
					if (attributeType != StunMessage.AttributeType.MappedAddress)
					{
						switch (attributeType)
						{
						case StunMessage.AttributeType.Username:
						{
							byte[] bytes = Encoding.ASCII.GetBytes((string)keyValuePair.Value);
							int num = offset;
							offset = num + 1;
							msg[num] = 0;
							num = offset;
							offset = num + 1;
							msg[num] = 6;
							num = offset;
							offset = num + 1;
							msg[num] = (byte)(bytes.Length >> 8);
							num = offset;
							offset = num + 1;
							msg[num] = (byte)(bytes.Length & 255);
							Array.Copy(bytes, 0, msg, offset, bytes.Length);
							offset += bytes.Length;
							break;
						}
						case StunMessage.AttributeType.ErrorCode:
						{
							byte[] bytes2 = Encoding.ASCII.GetBytes(this.ErrorCode.ReasonText);
							int num = offset;
							offset = num + 1;
							msg[num] = 0;
							num = offset;
							offset = num + 1;
							msg[num] = 9;
							num = offset;
							offset = num + 1;
							msg[num] = 0;
							num = offset;
							offset = num + 1;
							msg[num] = (byte)(4 + bytes2.Length);
							num = offset;
							offset = num + 1;
							msg[num] = 0;
							num = offset;
							offset = num + 1;
							msg[num] = 0;
							num = offset;
							offset = num + 1;
							msg[num] = (byte)Math.Floor((double)(this.ErrorCode.Code / 100));
							num = offset;
							offset = num + 1;
							msg[num] = (byte)(this.ErrorCode.Code & 255);
							Array.Copy(bytes2, msg, bytes2.Length);
							offset += bytes2.Length;
							break;
						}
						}
					}
					else
					{
						this.StoreEndPoint(StunMessage.AttributeType.MappedAddress, (IPEndPoint)keyValuePair.Value, msg, ref offset);
					}
				}
				else if (attributeType != StunMessage.AttributeType.Realm)
				{
					if (attributeType != StunMessage.AttributeType.Nonce)
					{
						if (attributeType != StunMessage.AttributeType.XorMappedAddress)
						{
						}
					}
				}
			}
		}

		private unsafe void ReadAttribute(byte* data, ref int offset)
		{
			int num = offset;
			offset = num + 1;
			StunMessage.AttributeType attributeType = (StunMessage.AttributeType)(data[num] << 8);
			num = offset;
			offset = num + 1;
			StunMessage.AttributeType attributeType2 = attributeType | (StunMessage.AttributeType)data[num];
			num = offset;
			offset = num + 1;
			int num2 = (int)data[num] << 8;
			num = offset;
			offset = num + 1;
			int num3 = num2 | (int)data[num];
			int num4 = offset;
			try
			{
				StunMessage.AttributeType attributeType3 = attributeType2;
				StunMessage.AttributeType attributeType4 = attributeType3;
				if (attributeType4 != StunMessage.AttributeType.MappedAddress)
				{
					switch (attributeType4)
					{
					case StunMessage.AttributeType.Username:
						break;
					case (StunMessage.AttributeType)7:
						break;
					case StunMessage.AttributeType.MessageIntegrity:
						break;
					case StunMessage.AttributeType.ErrorCode:
						break;
					case StunMessage.AttributeType.UnknownAttribute:
					{
						TraceLogStream logTraceStun = InternalLogStreams.LogTraceStun;
						if (logTraceStun != null)
						{
							logTraceStun.Error("UnknownAttribute");
						}
						break;
					}
					default:
						if (attributeType4 == StunMessage.AttributeType.XorMappedAddress)
						{
							TraceLogStream logTraceStun2 = InternalLogStreams.LogTraceStun;
							if (logTraceStun2 != null)
							{
								logTraceStun2.Log("AttributeType.XorMappedAddress");
							}
							this.MappedAddress = this.ParseXorEndPoint(data, ref offset);
						}
						break;
					}
				}
				else
				{
					TraceLogStream logTraceStun3 = InternalLogStreams.LogTraceStun;
					if (logTraceStun3 != null)
					{
						logTraceStun3.Log("AttributeType.MappedAddress");
					}
					this.MappedAddress = this.ParseEndPoint(data, ref offset);
				}
			}
			catch (Exception message)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Error(message);
				}
			}
			offset = num4 + num3;
		}

		private unsafe IPEndPoint ParseEndPoint(byte* data, ref int offset)
		{
			offset++;
			int num = offset;
			offset = num + 1;
			byte b = data[num];
			num = offset;
			offset = num + 1;
			int num2 = (int)data[num] << 8;
			num = offset;
			offset = num + 1;
			int port = num2 | (int)data[num];
			byte[] array = new byte[4];
			byte[] array2 = array;
			int num3 = 0;
			num = offset;
			offset = num + 1;
			array2[num3] = data[num];
			byte[] array3 = array;
			int num4 = 1;
			num = offset;
			offset = num + 1;
			array3[num4] = data[num];
			byte[] array4 = array;
			int num5 = 2;
			num = offset;
			offset = num + 1;
			array4[num5] = data[num];
			byte[] array5 = array;
			int num6 = 3;
			num = offset;
			offset = num + 1;
			array5[num6] = data[num];
			return new IPEndPoint(new IPAddress(array), port);
		}

		private unsafe IPEndPoint ParseXorEndPoint(byte* data, ref int offset)
		{
			offset++;
			int num = offset;
			offset = num + 1;
			StunMessage.IPFamily ipfamily = (StunMessage.IPFamily)data[num];
			num = offset;
			offset = num + 1;
			int num2 = (int)data[num] << 8;
			num = offset;
			offset = num + 1;
			int num3 = num2 | (int)data[num];
			num3 ^= 8466;
			StunMessage.IPFamily ipfamily2 = ipfamily;
			StunMessage.IPFamily ipfamily3 = ipfamily2;
			IPEndPoint result;
			if (ipfamily3 != StunMessage.IPFamily.IPv4)
			{
				if (ipfamily3 != StunMessage.IPFamily.IPv6)
				{
					result = null;
				}
				else
				{
					num = offset;
					offset = num + 1;
					ulong num4 = (ulong)data[num] << 56;
					num = offset;
					offset = num + 1;
					ulong num5 = num4 | (ulong)data[num] << 48;
					num = offset;
					offset = num + 1;
					ulong num6 = num5 | (ulong)data[num] << 40;
					num = offset;
					offset = num + 1;
					ulong num7 = num6 | (ulong)data[num] << 32;
					num = offset;
					offset = num + 1;
					ulong num8 = num7 | (ulong)data[num] << 24;
					num = offset;
					offset = num + 1;
					ulong num9 = num8 | (ulong)data[num] << 16;
					num = offset;
					offset = num + 1;
					ulong num10 = num9 | (ulong)data[num] << 8;
					num = offset;
					offset = num + 1;
					ulong num11 = num10 | (ulong)data[num];
					num = offset;
					offset = num + 1;
					ulong num12 = (ulong)data[num] << 56;
					num = offset;
					offset = num + 1;
					ulong num13 = num12 | (ulong)data[num] << 48;
					num = offset;
					offset = num + 1;
					ulong num14 = num13 | (ulong)data[num] << 40;
					num = offset;
					offset = num + 1;
					ulong num15 = num14 | (ulong)data[num] << 32;
					num = offset;
					offset = num + 1;
					ulong num16 = num15 | (ulong)data[num] << 24;
					num = offset;
					offset = num + 1;
					ulong num17 = num16 | (ulong)data[num] << 16;
					num = offset;
					offset = num + 1;
					ulong num18 = num17 | (ulong)data[num] << 8;
					num = offset;
					offset = num + 1;
					ulong num19 = num18 | (ulong)data[num];
					ulong num20 = (ulong)BitConverter.ToUInt32(this.TransactionID, 0);
					ulong num21 = BitConverter.ToUInt64(this.TransactionID, 4);
					num20 = (num20 << 32 | 1118048801UL);
					ulong value = num11 ^ num20;
					ulong value2 = num19 ^ num21;
					byte[] bytes = BitConverter.GetBytes(value);
					byte[] bytes2 = BitConverter.GetBytes(value2);
					byte[] array = new byte[16];
					Array.Copy(bytes, 0, array, 0, bytes.Length);
					Array.Copy(bytes2, 0, array, 8, bytes2.Length);
					result = new IPEndPoint(new IPAddress(array), num3);
				}
			}
			else
			{
				byte[] array2 = new byte[4];
				byte[] array3 = array2;
				int num22 = 0;
				num = offset;
				offset = num + 1;
				array3[num22] = data[num];
				byte[] array4 = array2;
				int num23 = 1;
				num = offset;
				offset = num + 1;
				array4[num23] = data[num];
				byte[] array5 = array2;
				int num24 = 2;
				num = offset;
				offset = num + 1;
				array5[num24] = data[num];
				byte[] array6 = array2;
				int num25 = 3;
				num = offset;
				offset = num + 1;
				array6[num25] = data[num];
				bool isLittleEndian = BitConverter.IsLittleEndian;
				if (isLittleEndian)
				{
					Array.Reverse<byte>(array2);
				}
				uint num26 = BitConverter.ToUInt32(array2, 0);
				num26 ^= 554869826U;
				byte[] bytes3 = BitConverter.GetBytes(num26);
				bool isLittleEndian2 = BitConverter.IsLittleEndian;
				if (isLittleEndian2)
				{
					Array.Reverse<byte>(bytes3);
				}
				num26 = BitConverter.ToUInt32(bytes3, 0);
				result = new IPEndPoint((long)((ulong)num26), num3);
			}
			return result;
		}

		private void StoreEndPoint(StunMessage.AttributeType type, IPEndPoint endPoint, byte[] message, ref int offset)
		{
			int num = offset;
			offset = num + 1;
			message[num] = (byte)(type >> 8);
			num = offset;
			offset = num + 1;
			message[num] = (byte)(type & (StunMessage.AttributeType)255);
			num = offset;
			offset = num + 1;
			message[num] = 0;
			num = offset;
			offset = num + 1;
			message[num] = 8;
			num = offset;
			offset = num + 1;
			message[num] = 0;
			num = offset;
			offset = num + 1;
			message[num] = 1;
			num = offset;
			offset = num + 1;
			message[num] = (byte)(endPoint.Port >> 8);
			num = offset;
			offset = num + 1;
			message[num] = (byte)(endPoint.Port & 255);
			byte[] addressBytes = endPoint.Address.GetAddressBytes();
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[0];
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[0];
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[0];
			num = offset;
			offset = num + 1;
			message[num] = addressBytes[0];
		}

		private Guid _id = Guid.Empty;

		private static HashSet<int> _stunMessageTypeValues;

		public enum StunMessageType
		{
			BindingRequest = 1,
			BindingResponse = 257,
			BindingErrorResponse = 273,
			SharedSecretRequest = 2,
			SharedSecretResponse = 258,
			SharedSecretErrorResponse = 274
		}

		private enum AttributeType
		{
			MappedAddress = 1,
			Username = 6,
			MessageIntegrity = 8,
			ErrorCode,
			UnknownAttribute,
			Realm = 20,
			Nonce,
			XorMappedAddress = 32
		}

		internal static class StunDefines
		{
			public const int STUN_MAGIC_COOKIE = 554869826;

			public const ulong STUN_MAGIC_COOKIE_NETWORK_ORDER = 1118048801UL;

			public const short STUN_MAGIC_COOKIE_PARTIAL = 8466;

			public const int STUN_XOR_FINGERPRINT = 1398035790;

			public const int HEADER_SIZE = 20;

			public const int TRANSACTION_ID_SIZE = 12;
		}

		private enum IPFamily
		{
			IPv4 = 1,
			IPv6
		}
	}
}
