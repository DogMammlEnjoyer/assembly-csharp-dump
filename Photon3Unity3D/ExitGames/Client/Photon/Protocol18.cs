using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ExitGames.Client.Photon.StructWrapping;

namespace ExitGames.Client.Photon
{
	public class Protocol18 : IProtocol
	{
		public override string ProtocolType
		{
			get
			{
				return "GpBinaryV18";
			}
		}

		public override byte[] VersionBytes
		{
			get
			{
				return this.versionBytes;
			}
		}

		public override void Serialize(StreamBuffer dout, object serObject, bool setType)
		{
			this.Write(dout, serObject, setType);
		}

		public override void SerializeShort(StreamBuffer dout, short serObject, bool setType)
		{
			this.WriteInt16(dout, serObject, setType);
		}

		public override void SerializeString(StreamBuffer dout, string serObject, bool setType)
		{
			this.WriteString(dout, serObject, setType);
		}

		public override object Deserialize(StreamBuffer din, byte type, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None)
		{
			return this.Read(din, type, IProtocol.DeserializationFlags.None, null);
		}

		public override short DeserializeShort(StreamBuffer din)
		{
			return this.ReadInt16(din);
		}

		public override byte DeserializeByte(StreamBuffer din)
		{
			return this.ReadByte(din);
		}

		private static Type GetAllowedDictionaryKeyTypes(Protocol18.GpType gpType)
		{
			switch (gpType)
			{
			case Protocol18.GpType.Byte:
			case Protocol18.GpType.ByteZero:
				return typeof(byte);
			case Protocol18.GpType.Short:
			case Protocol18.GpType.ShortZero:
				return typeof(short);
			case Protocol18.GpType.Float:
			case Protocol18.GpType.FloatZero:
				return typeof(float);
			case Protocol18.GpType.Double:
			case Protocol18.GpType.DoubleZero:
				return typeof(double);
			case Protocol18.GpType.String:
				return typeof(string);
			case Protocol18.GpType.CompressedInt:
			case Protocol18.GpType.Int1:
			case Protocol18.GpType.Int1_:
			case Protocol18.GpType.Int2:
			case Protocol18.GpType.Int2_:
			case Protocol18.GpType.IntZero:
				return typeof(int);
			case Protocol18.GpType.CompressedLong:
			case Protocol18.GpType.L1:
			case Protocol18.GpType.L1_:
			case Protocol18.GpType.L2:
			case Protocol18.GpType.L2_:
			case Protocol18.GpType.LongZero:
				return typeof(long);
			}
			throw new Exception(string.Format("{0} is not a valid Type as Dictionary key.", gpType));
		}

		private static Type GetClrArrayType(Protocol18.GpType gpType)
		{
			switch (gpType)
			{
			case Protocol18.GpType.Boolean:
			case Protocol18.GpType.BooleanFalse:
			case Protocol18.GpType.BooleanTrue:
				return typeof(bool);
			case Protocol18.GpType.Byte:
			case Protocol18.GpType.ByteZero:
				return typeof(byte);
			case Protocol18.GpType.Short:
			case Protocol18.GpType.ShortZero:
				return typeof(short);
			case Protocol18.GpType.Float:
			case Protocol18.GpType.FloatZero:
				return typeof(float);
			case Protocol18.GpType.Double:
			case Protocol18.GpType.DoubleZero:
				return typeof(double);
			case Protocol18.GpType.String:
				return typeof(string);
			case Protocol18.GpType.Null:
			case Protocol18.GpType.Custom:
			case Protocol18.GpType.Dictionary:
			case (Protocol18.GpType)22:
			case Protocol18.GpType.ObjectArray:
				break;
			case Protocol18.GpType.CompressedInt:
			case Protocol18.GpType.Int1:
			case Protocol18.GpType.Int1_:
			case Protocol18.GpType.Int2:
			case Protocol18.GpType.Int2_:
			case Protocol18.GpType.IntZero:
				return typeof(int);
			case Protocol18.GpType.CompressedLong:
			case Protocol18.GpType.L1:
			case Protocol18.GpType.L1_:
			case Protocol18.GpType.L2:
			case Protocol18.GpType.L2_:
			case Protocol18.GpType.LongZero:
				return typeof(long);
			case Protocol18.GpType.Hashtable:
				return typeof(Hashtable);
			case Protocol18.GpType.OperationRequest:
				return typeof(OperationRequest);
			case Protocol18.GpType.OperationResponse:
				return typeof(OperationResponse);
			case Protocol18.GpType.EventData:
				return typeof(EventData);
			default:
				switch (gpType)
				{
				case Protocol18.GpType.BooleanArray:
					return typeof(bool[]);
				case Protocol18.GpType.ByteArray:
					return typeof(byte[]);
				case Protocol18.GpType.ShortArray:
					return typeof(short[]);
				case Protocol18.GpType.FloatArray:
					return typeof(float[]);
				case Protocol18.GpType.DoubleArray:
					return typeof(double[]);
				case Protocol18.GpType.StringArray:
					return typeof(string[]);
				case (Protocol18.GpType)72:
					break;
				case Protocol18.GpType.CompressedIntArray:
					return typeof(int[]);
				case Protocol18.GpType.CompressedLongArray:
					return typeof(long[]);
				default:
					if (gpType == Protocol18.GpType.HashtableArray)
					{
						return typeof(Hashtable[]);
					}
					break;
				}
				break;
			}
			return null;
		}

		private Protocol18.GpType GetCodeOfType(Type type)
		{
			bool flag = type == null;
			Protocol18.GpType result;
			if (flag)
			{
				result = Protocol18.GpType.Null;
			}
			else
			{
				bool flag2 = type == typeof(StructWrapper<>);
				if (flag2)
				{
					result = Protocol18.GpType.Unknown;
				}
				else
				{
					bool flag3 = type.IsPrimitive || type.IsEnum;
					if (flag3)
					{
						TypeCode typeCode = Type.GetTypeCode(type);
						result = this.GetCodeOfTypeCode(typeCode);
					}
					else
					{
						bool flag4 = type == typeof(string);
						if (flag4)
						{
							result = Protocol18.GpType.String;
						}
						else
						{
							bool isArray = type.IsArray;
							if (isArray)
							{
								Type elementType = type.GetElementType();
								bool flag5 = elementType == null;
								if (flag5)
								{
									throw new InvalidDataException(string.Format("Arrays of type {0} are not supported", type));
								}
								bool isPrimitive = elementType.IsPrimitive;
								if (isPrimitive)
								{
									switch (Type.GetTypeCode(elementType))
									{
									case TypeCode.Boolean:
										return Protocol18.GpType.BooleanArray;
									case TypeCode.Byte:
										return Protocol18.GpType.ByteArray;
									case TypeCode.Int16:
										return Protocol18.GpType.ShortArray;
									case TypeCode.Int32:
										return Protocol18.GpType.CompressedIntArray;
									case TypeCode.Int64:
										return Protocol18.GpType.CompressedLongArray;
									case TypeCode.Single:
										return Protocol18.GpType.FloatArray;
									case TypeCode.Double:
										return Protocol18.GpType.DoubleArray;
									}
								}
								bool isArray2 = elementType.IsArray;
								if (isArray2)
								{
									result = Protocol18.GpType.Array;
								}
								else
								{
									bool flag6 = elementType == typeof(string);
									if (flag6)
									{
										result = Protocol18.GpType.StringArray;
									}
									else
									{
										bool flag7 = elementType == typeof(object) || elementType == typeof(StructWrapper);
										if (flag7)
										{
											result = Protocol18.GpType.ObjectArray;
										}
										else
										{
											bool flag8 = elementType == typeof(Hashtable);
											if (flag8)
											{
												result = Protocol18.GpType.HashtableArray;
											}
											else
											{
												bool flag9 = elementType.IsGenericType && typeof(Dictionary<, >) == elementType.GetGenericTypeDefinition();
												if (flag9)
												{
													result = Protocol18.GpType.DictionaryArray;
												}
												else
												{
													result = Protocol18.GpType.CustomTypeArray;
												}
											}
										}
									}
								}
							}
							else
							{
								bool flag10 = type == typeof(Hashtable);
								if (flag10)
								{
									result = Protocol18.GpType.Hashtable;
								}
								else
								{
									bool flag11 = type == typeof(List<object>);
									if (flag11)
									{
										result = Protocol18.GpType.ObjectArray;
									}
									else
									{
										bool flag12 = type.IsGenericType && typeof(Dictionary<, >) == type.GetGenericTypeDefinition();
										if (flag12)
										{
											result = Protocol18.GpType.Dictionary;
										}
										else
										{
											bool flag13 = type == typeof(EventData);
											if (flag13)
											{
												result = Protocol18.GpType.EventData;
											}
											else
											{
												bool flag14 = type == typeof(OperationRequest);
												if (flag14)
												{
													result = Protocol18.GpType.OperationRequest;
												}
												else
												{
													bool flag15 = type == typeof(OperationResponse);
													if (flag15)
													{
														result = Protocol18.GpType.OperationResponse;
													}
													else
													{
														result = Protocol18.GpType.Unknown;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private Protocol18.GpType GetCodeOfTypeCode(TypeCode type)
		{
			switch (type)
			{
			case TypeCode.Boolean:
				return Protocol18.GpType.Boolean;
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				break;
			case TypeCode.Byte:
				return Protocol18.GpType.Byte;
			case TypeCode.Int16:
				return Protocol18.GpType.Short;
			case TypeCode.Int32:
				return Protocol18.GpType.CompressedInt;
			case TypeCode.Int64:
				return Protocol18.GpType.CompressedLong;
			case TypeCode.Single:
				return Protocol18.GpType.Float;
			case TypeCode.Double:
				return Protocol18.GpType.Double;
			default:
				if (type == TypeCode.String)
				{
					return Protocol18.GpType.String;
				}
				break;
			}
			return Protocol18.GpType.Unknown;
		}

		private object Read(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			return this.Read(stream, this.ReadByte(stream), flags, parameters);
		}

		private object Read(StreamBuffer stream, byte gpType, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None, ParameterDictionary parameters = null)
		{
			int num = (int)((gpType >= 128) ? (gpType - 128) : gpType);
			int num2 = (num >= 64) ? (num - 64) : num;
			bool flag = (flags & IProtocol.DeserializationFlags.WrapIncomingStructs) == IProtocol.DeserializationFlags.WrapIncomingStructs;
			bool flag2 = gpType >= 128 && gpType <= 228;
			if (!flag2)
			{
				switch (gpType)
				{
				case 2:
				{
					bool flag3 = this.ReadBoolean(stream);
					return flag ? parameters.wrapperPools.Acquire(flag3) : flag3;
				}
				case 3:
				{
					byte b = this.ReadByte(stream);
					return flag ? parameters.wrapperPools.Acquire(b) : b;
				}
				case 4:
				{
					short num3 = this.ReadInt16(stream);
					return flag ? parameters.wrapperPools.Acquire<short>(num3) : num3;
				}
				case 5:
				{
					float num4 = this.ReadSingle(stream);
					return flag ? parameters.wrapperPools.Acquire<float>(num4) : num4;
				}
				case 6:
				{
					double num5 = this.ReadDouble(stream);
					return flag ? parameters.wrapperPools.Acquire<double>(num5) : num5;
				}
				case 7:
					return this.ReadString(stream);
				case 8:
					return null;
				case 9:
				{
					int num6 = this.ReadCompressedInt32(stream);
					return flag ? parameters.wrapperPools.Acquire<int>(num6) : num6;
				}
				case 10:
				{
					long num7 = this.ReadCompressedInt64(stream);
					return flag ? parameters.wrapperPools.Acquire<long>(num7) : num7;
				}
				case 11:
				{
					int num8 = this.ReadInt1(stream, false);
					return flag ? parameters.wrapperPools.Acquire<int>(num8) : num8;
				}
				case 12:
				{
					int num9 = this.ReadInt1(stream, true);
					return flag ? parameters.wrapperPools.Acquire<int>(num9) : num9;
				}
				case 13:
				{
					int num10 = this.ReadInt2(stream, false);
					return flag ? parameters.wrapperPools.Acquire<int>(num10) : num10;
				}
				case 14:
				{
					int num11 = this.ReadInt2(stream, true);
					return flag ? parameters.wrapperPools.Acquire<int>(num11) : num11;
				}
				case 15:
				{
					long num12 = (long)this.ReadInt1(stream, false);
					return flag ? parameters.wrapperPools.Acquire<long>(num12) : num12;
				}
				case 16:
				{
					long num13 = (long)this.ReadInt1(stream, true);
					return flag ? parameters.wrapperPools.Acquire<long>(num13) : num13;
				}
				case 17:
				{
					long num14 = (long)this.ReadInt2(stream, false);
					return flag ? parameters.wrapperPools.Acquire<long>(num14) : num14;
				}
				case 18:
				{
					long num15 = (long)this.ReadInt2(stream, true);
					return flag ? parameters.wrapperPools.Acquire<long>(num15) : num15;
				}
				case 19:
					return this.ReadCustomType(stream, 0);
				case 20:
					return this.ReadDictionary(stream, flags, parameters);
				case 21:
					return this.ReadHashtable(stream, flags, parameters);
				case 23:
					return this.ReadObjectArray(stream, flags, parameters);
				case 24:
					return this.DeserializeOperationRequest(stream, IProtocol.DeserializationFlags.None);
				case 25:
					return this.DeserializeOperationResponse(stream, flags);
				case 26:
					return this.DeserializeEventData(stream, null, IProtocol.DeserializationFlags.None);
				case 27:
				{
					bool flag4 = false;
					return flag ? parameters.wrapperPools.Acquire(flag4) : flag4;
				}
				case 28:
				{
					bool flag5 = true;
					return flag ? parameters.wrapperPools.Acquire(flag5) : flag5;
				}
				case 29:
				{
					short num16 = 0;
					return flag ? parameters.wrapperPools.Acquire<short>(num16) : num16;
				}
				case 30:
				{
					int num17 = 0;
					return flag ? parameters.wrapperPools.Acquire<int>(num17) : num17;
				}
				case 31:
				{
					long num18 = 0L;
					return flag ? parameters.wrapperPools.Acquire<long>(num18) : num18;
				}
				case 32:
				{
					float num19 = 0f;
					return flag ? parameters.wrapperPools.Acquire<float>(num19) : num19;
				}
				case 33:
				{
					double num20 = 0.0;
					return flag ? parameters.wrapperPools.Acquire<double>(num20) : num20;
				}
				case 34:
				{
					byte b2 = 0;
					return flag ? parameters.wrapperPools.Acquire(b2) : b2;
				}
				case 64:
					return this.ReadArrayInArray(stream, flags, parameters);
				case 66:
					return this.ReadBooleanArray(stream);
				case 67:
					return this.ReadByteArray(stream);
				case 68:
					return this.ReadInt16Array(stream);
				case 69:
					return this.ReadSingleArray(stream);
				case 70:
					return this.ReadDoubleArray(stream);
				case 71:
					return this.ReadStringArray(stream);
				case 73:
					return this.ReadCompressedInt32Array(stream);
				case 74:
					return this.ReadCompressedInt64Array(stream);
				case 83:
					return this.ReadCustomTypeArray(stream);
				case 84:
					return this.ReadDictionaryArray(stream, flags, parameters);
				case 85:
					return this.ReadHashtableArray(stream, flags, parameters);
				}
				throw new InvalidDataException(string.Format("GpTypeCode not found: {0}(0x{0:X}). Is not a CustomType either. Pos: {1} Available: {2}", gpType, stream.Position, stream.Available));
			}
			return this.ReadCustomType(stream, gpType);
		}

		internal bool ReadBoolean(StreamBuffer stream)
		{
			return stream.ReadByte() > 0;
		}

		internal byte ReadByte(StreamBuffer stream)
		{
			return stream.ReadByte();
		}

		internal short ReadInt16(StreamBuffer stream)
		{
			int num;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(2, out num);
			return (short)((int)bufferAndAdvance[num++] | (int)bufferAndAdvance[num] << 8);
		}

		internal ushort ReadUShort(StreamBuffer stream)
		{
			int num;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(2, out num);
			return (ushort)((int)bufferAndAdvance[num++] | (int)bufferAndAdvance[num] << 8);
		}

		internal int ReadInt32(StreamBuffer stream)
		{
			int num;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out num);
			return (int)bufferAndAdvance[num++] << 24 | (int)bufferAndAdvance[num++] << 16 | (int)bufferAndAdvance[num++] << 8 | (int)bufferAndAdvance[num];
		}

		internal long ReadInt64(StreamBuffer stream)
		{
			int num;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out num);
			return (long)((ulong)bufferAndAdvance[num++] << 56 | (ulong)bufferAndAdvance[num++] << 48 | (ulong)bufferAndAdvance[num++] << 40 | (ulong)bufferAndAdvance[num++] << 32 | (ulong)bufferAndAdvance[num++] << 24 | (ulong)bufferAndAdvance[num++] << 16 | (ulong)bufferAndAdvance[num++] << 8 | (ulong)bufferAndAdvance[num]);
		}

		internal float ReadSingle(StreamBuffer stream)
		{
			int startIndex;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out startIndex);
			return BitConverter.ToSingle(bufferAndAdvance, startIndex);
		}

		internal double ReadDouble(StreamBuffer stream)
		{
			int startIndex;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(8, out startIndex);
			return BitConverter.ToDouble(bufferAndAdvance, startIndex);
		}

		internal ByteArraySlice ReadNonAllocByteArray(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			ByteArraySlice byteArraySlice = this.ByteArraySlicePool.Acquire((int)num);
			stream.Read(byteArraySlice.Buffer, 0, (int)num);
			byteArraySlice.Count = (int)num;
			return byteArraySlice;
		}

		internal byte[] ReadByteArray(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			byte[] array = new byte[num];
			stream.Read(array, 0, (int)num);
			return array;
		}

		public object ReadCustomType(StreamBuffer stream, byte gpType = 0)
		{
			bool flag = gpType == 0;
			byte b;
			if (flag)
			{
				b = stream.ReadByte();
			}
			else
			{
				b = gpType - 128;
			}
			int num = (int)this.ReadCompressedUInt32(stream);
			bool flag2 = num < 0;
			if (flag2)
			{
				throw new InvalidDataException("ReadCustomType read negative size value: " + num.ToString() + " before position: " + stream.Position.ToString());
			}
			bool flag3 = num <= stream.Available;
			CustomType customType;
			bool flag4 = !flag3 || num > 32767 || !Protocol.CodeDict.TryGetValue(b, out customType);
			object result;
			if (flag4)
			{
				UnknownType unknownType = new UnknownType
				{
					TypeCode = b,
					Size = num
				};
				int num2 = flag3 ? num : stream.Available;
				bool flag5 = num2 > 0;
				if (flag5)
				{
					byte[] array = new byte[num2];
					stream.Read(array, 0, num2);
					unknownType.Data = array;
				}
				result = unknownType;
			}
			else
			{
				bool flag6 = customType.DeserializeStreamFunction == null;
				if (flag6)
				{
					byte[] array2 = new byte[num];
					stream.Read(array2, 0, num);
					result = customType.DeserializeFunction(array2);
				}
				else
				{
					int position = stream.Position;
					object obj = customType.DeserializeStreamFunction(stream, (short)num);
					int num3 = stream.Position - position;
					bool flag7 = num3 != num;
					if (flag7)
					{
						stream.Position = position + num;
					}
					result = obj;
				}
			}
			return result;
		}

		public override EventData DeserializeEventData(StreamBuffer din, EventData target = null, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None)
		{
			bool flag = target != null;
			EventData eventData;
			if (flag)
			{
				target.Reset();
				eventData = target;
			}
			else
			{
				eventData = new EventData();
			}
			eventData.Code = this.ReadByte(din);
			short num = (short)this.ReadByte(din);
			bool flag2 = (flags & IProtocol.DeserializationFlags.AllowPooledByteArray) == IProtocol.DeserializationFlags.AllowPooledByteArray;
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				byte b = din.ReadByte();
				byte b2 = din.ReadByte();
				bool flag3 = !flag2;
				object value;
				if (flag3)
				{
					value = this.Read(din, b2, flags, eventData.Parameters);
					goto IL_14F;
				}
				bool flag4 = b2 == 67;
				if (flag4)
				{
					value = this.ReadNonAllocByteArray(din);
				}
				else
				{
					bool flag5 = b == eventData.SenderKey;
					if (flag5)
					{
						Protocol18.GpType gpType = (Protocol18.GpType)b2;
						Protocol18.GpType gpType2 = gpType;
						switch (gpType2)
						{
						case Protocol18.GpType.CompressedInt:
							eventData.Sender = this.ReadCompressedInt32(din);
							break;
						case Protocol18.GpType.CompressedLong:
							break;
						case Protocol18.GpType.Int1:
							eventData.Sender = this.ReadInt1(din, false);
							break;
						case Protocol18.GpType.Int1_:
							eventData.Sender = this.ReadInt1(din, true);
							break;
						case Protocol18.GpType.Int2:
							eventData.Sender = this.ReadInt2(din, false);
							break;
						case Protocol18.GpType.Int2_:
							eventData.Sender = this.ReadInt2(din, true);
							break;
						default:
							if (gpType2 == Protocol18.GpType.IntZero)
							{
								eventData.Sender = 0;
							}
							break;
						}
						goto IL_15E;
					}
					value = this.Read(din, b2, flags, eventData.Parameters);
				}
				goto IL_14F;
				IL_15E:
				num2 += 1U;
				continue;
				IL_14F:
				eventData.Parameters.Add(b, value);
				goto IL_15E;
			}
			return eventData;
		}

		[Obsolete("Use ParameterDictionary instead.")]
		private Dictionary<byte, object> ReadParameterTable(StreamBuffer stream, Dictionary<byte, object> target = null, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None)
		{
			short num = (short)this.ReadByte(stream);
			Dictionary<byte, object> dictionary = (target != null) ? target : new Dictionary<byte, object>((int)num);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				byte key = stream.ReadByte();
				byte b = stream.ReadByte();
				bool flag = b == 67 && (flags & IProtocol.DeserializationFlags.AllowPooledByteArray) == IProtocol.DeserializationFlags.AllowPooledByteArray;
				object value;
				if (flag)
				{
					value = this.ReadNonAllocByteArray(stream);
				}
				else
				{
					value = this.Read(stream, b, flags, null);
				}
				dictionary[key] = value;
				num2 += 1U;
			}
			return dictionary;
		}

		private ParameterDictionary ReadParameterDictionary(StreamBuffer stream, ParameterDictionary target = null, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None)
		{
			short num = (short)this.ReadByte(stream);
			ParameterDictionary parameterDictionary = (target != null) ? target : new ParameterDictionary((int)num);
			bool flag = (flags & IProtocol.DeserializationFlags.AllowPooledByteArray) == IProtocol.DeserializationFlags.AllowPooledByteArray;
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				byte code = stream.ReadByte();
				byte b = stream.ReadByte();
				bool flag2 = flag && b == 67;
				object value;
				if (flag2)
				{
					value = this.ReadNonAllocByteArray(stream);
				}
				else
				{
					value = this.Read(stream, b, flags, parameterDictionary);
				}
				parameterDictionary.Add(code, value);
				num2 += 1U;
			}
			return parameterDictionary;
		}

		public Hashtable ReadHashtable(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			int num = (int)this.ReadCompressedUInt32(stream);
			Hashtable hashtable = new Hashtable(num);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				object obj = this.Read(stream, flags, parameters);
				object value = this.Read(stream, flags, parameters);
				bool flag = obj == null;
				if (!flag)
				{
					StructWrapper<byte> structWrapper = obj as StructWrapper<byte>;
					bool flag2 = structWrapper == null;
					if (flag2)
					{
						hashtable[obj] = value;
					}
					else
					{
						hashtable[structWrapper.Unwrap<byte>()] = value;
					}
				}
				num2 += 1U;
			}
			return hashtable;
		}

		public int[] ReadIntArray(StreamBuffer stream)
		{
			int num = this.ReadInt32(stream);
			int[] array = new int[num];
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				array[(int)num2] = this.ReadInt32(stream);
				num2 += 1U;
			}
			return array;
		}

		public override OperationRequest DeserializeOperationRequest(StreamBuffer din, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None)
		{
			OperationRequest operationRequest = new OperationRequest();
			operationRequest.OperationCode = this.ReadByte(din);
			operationRequest.Parameters = this.ReadParameterDictionary(din, operationRequest.Parameters, flags);
			return operationRequest;
		}

		public override OperationResponse DeserializeOperationResponse(StreamBuffer stream, IProtocol.DeserializationFlags flags = IProtocol.DeserializationFlags.None)
		{
			OperationResponse operationResponse = new OperationResponse();
			operationResponse.OperationCode = this.ReadByte(stream);
			operationResponse.ReturnCode = this.ReadInt16(stream);
			operationResponse.DebugMessage = (this.Read(stream, this.ReadByte(stream), flags, operationResponse.Parameters) as string);
			operationResponse.Parameters = this.ReadParameterDictionary(stream, operationResponse.Parameters, flags);
			return operationResponse;
		}

		public override DisconnectMessage DeserializeDisconnectMessage(StreamBuffer stream)
		{
			return new DisconnectMessage
			{
				Code = this.ReadInt16(stream),
				DebugMessage = (this.Read(stream, this.ReadByte(stream), IProtocol.DeserializationFlags.None, null) as string),
				Parameters = this.ReadParameterTable(stream, null, IProtocol.DeserializationFlags.None)
			};
		}

		internal string ReadString(StreamBuffer stream)
		{
			int num = (int)this.ReadCompressedUInt32(stream);
			bool flag = num == 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				int index = 0;
				byte[] bufferAndAdvance = stream.GetBufferAndAdvance(num, out index);
				result = Encoding.UTF8.GetString(bufferAndAdvance, index, num);
			}
			return result;
		}

		private object ReadCustomTypeArray(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			byte b = stream.ReadByte();
			CustomType customType;
			bool flag = !Protocol.CodeDict.TryGetValue(b, out customType);
			object result;
			if (flag)
			{
				int position = stream.Position;
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					int num3 = (int)this.ReadCompressedUInt32(stream);
					int available = stream.Available;
					int num4 = (num3 > available) ? available : num3;
					stream.Position += num4;
				}
				result = new UnknownType[]
				{
					new UnknownType
					{
						TypeCode = b,
						Size = stream.Position - position
					}
				};
			}
			else
			{
				Array array = Array.CreateInstance(customType.Type, (int)num);
				for (uint num5 = 0U; num5 < num; num5 += 1U)
				{
					int num6 = (int)this.ReadCompressedUInt32(stream);
					bool flag2 = num6 < 0;
					if (flag2)
					{
						throw new InvalidDataException("ReadCustomTypeArray read negative size value: " + num6.ToString() + " before position: " + stream.Position.ToString());
					}
					bool flag3 = num6 > stream.Available || num6 > 32767;
					if (flag3)
					{
						stream.Position = stream.Length;
						throw new InvalidDataException("ReadCustomTypeArray read size value: " + num6.ToString() + " larger than short.MaxValue or available data: " + stream.Available.ToString());
					}
					bool flag4 = customType.DeserializeStreamFunction == null;
					object obj;
					if (flag4)
					{
						byte[] array2 = new byte[num6];
						stream.Read(array2, 0, num6);
						obj = customType.DeserializeFunction(array2);
					}
					else
					{
						int position2 = stream.Position;
						obj = customType.DeserializeStreamFunction(stream, (short)num6);
						int num7 = stream.Position - position2;
						bool flag5 = num7 != num6;
						if (flag5)
						{
							stream.Position = position2 + num6;
						}
					}
					bool flag6 = obj != null && customType.Type.IsAssignableFrom(obj.GetType());
					if (flag6)
					{
						array.SetValue(obj, (int)num5);
					}
				}
				result = array;
			}
			return result;
		}

		private Type ReadDictionaryType(StreamBuffer stream, out Protocol18.GpType keyReadType, out Protocol18.GpType valueReadType)
		{
			keyReadType = (Protocol18.GpType)stream.ReadByte();
			Protocol18.GpType gpType = (Protocol18.GpType)stream.ReadByte();
			valueReadType = gpType;
			bool flag = keyReadType == Protocol18.GpType.Unknown;
			Type type;
			if (flag)
			{
				type = typeof(object);
			}
			else
			{
				type = Protocol18.GetAllowedDictionaryKeyTypes(keyReadType);
			}
			bool flag2 = gpType == Protocol18.GpType.Unknown;
			Type type2;
			if (flag2)
			{
				type2 = typeof(object);
			}
			else
			{
				bool flag3 = gpType == Protocol18.GpType.Dictionary;
				if (flag3)
				{
					type2 = this.ReadDictionaryType(stream);
				}
				else
				{
					bool flag4 = gpType == Protocol18.GpType.Array;
					if (flag4)
					{
						type2 = this.GetDictArrayType(stream);
						valueReadType = Protocol18.GpType.Unknown;
					}
					else
					{
						bool flag5 = gpType == Protocol18.GpType.ObjectArray;
						if (flag5)
						{
							type2 = typeof(object[]);
						}
						else
						{
							bool flag6 = gpType == Protocol18.GpType.HashtableArray;
							if (flag6)
							{
								type2 = typeof(Hashtable[]);
							}
							else
							{
								type2 = Protocol18.GetClrArrayType(gpType);
							}
						}
					}
				}
			}
			return typeof(Dictionary<, >).MakeGenericType(new Type[]
			{
				type,
				type2
			});
		}

		private Type ReadDictionaryType(StreamBuffer stream)
		{
			Protocol18.GpType gpType = (Protocol18.GpType)stream.ReadByte();
			Protocol18.GpType gpType2 = (Protocol18.GpType)stream.ReadByte();
			bool flag = gpType == Protocol18.GpType.Unknown;
			Type type;
			if (flag)
			{
				type = typeof(object);
			}
			else
			{
				type = Protocol18.GetAllowedDictionaryKeyTypes(gpType);
			}
			bool flag2 = gpType2 == Protocol18.GpType.Unknown;
			Type type2;
			if (flag2)
			{
				type2 = typeof(object);
			}
			else
			{
				bool flag3 = gpType2 == Protocol18.GpType.Dictionary;
				if (flag3)
				{
					type2 = this.ReadDictionaryType(stream);
				}
				else
				{
					bool flag4 = gpType2 == Protocol18.GpType.Array;
					if (flag4)
					{
						type2 = this.GetDictArrayType(stream);
					}
					else
					{
						type2 = Protocol18.GetClrArrayType(gpType2);
					}
				}
			}
			return typeof(Dictionary<, >).MakeGenericType(new Type[]
			{
				type,
				type2
			});
		}

		private Type GetDictArrayType(StreamBuffer stream)
		{
			Protocol18.GpType gpType = (Protocol18.GpType)stream.ReadByte();
			int num = 0;
			while (gpType == Protocol18.GpType.Array)
			{
				num++;
				gpType = (Protocol18.GpType)stream.ReadByte();
			}
			Type clrArrayType = Protocol18.GetClrArrayType(gpType);
			Type type = clrArrayType.MakeArrayType();
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				type = type.MakeArrayType();
				num2 += 1U;
			}
			return type;
		}

		private IDictionary ReadDictionary(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			Protocol18.GpType keyReadType;
			Protocol18.GpType valueReadType;
			Type type = this.ReadDictionaryType(stream, out keyReadType, out valueReadType);
			bool flag = type == null;
			IDictionary result;
			if (flag)
			{
				result = null;
			}
			else
			{
				IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
				bool flag2 = dictionary == null;
				if (flag2)
				{
					result = null;
				}
				else
				{
					this.ReadDictionaryElements(stream, keyReadType, valueReadType, dictionary, flags, parameters);
					result = dictionary;
				}
			}
			return result;
		}

		private bool ReadDictionaryElements(StreamBuffer stream, Protocol18.GpType keyReadType, Protocol18.GpType valueReadType, IDictionary dictionary, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			uint num = this.ReadCompressedUInt32(stream);
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				object obj = (keyReadType == Protocol18.GpType.Unknown) ? this.Read(stream, flags, parameters) : this.Read(stream, (byte)keyReadType, IProtocol.DeserializationFlags.None, null);
				object value = (valueReadType == Protocol18.GpType.Unknown) ? this.Read(stream, flags, parameters) : this.Read(stream, (byte)valueReadType, IProtocol.DeserializationFlags.None, null);
				bool flag = obj == null;
				if (!flag)
				{
					dictionary.Add(obj, value);
				}
			}
			return true;
		}

		private object[] ReadObjectArray(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			uint num = this.ReadCompressedUInt32(stream);
			object[] array = new object[num];
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				object obj = this.Read(stream, flags, parameters);
				array[(int)num2] = obj;
			}
			return array;
		}

		private StructWrapper[] ReadWrapperArray(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			uint num = this.ReadCompressedUInt32(stream);
			StructWrapper[] array = new StructWrapper[num];
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				object obj = this.Read(stream, flags, parameters);
				array[(int)num2] = (obj as StructWrapper);
				bool flag = obj == null;
				if (flag)
				{
					Debug.WriteLine("Error: ReadWrapperArray hit null");
				}
				bool flag2 = array[(int)num2] == null;
				if (flag2)
				{
					Debug.WriteLine("Error: ReadWrapperArray null wrapper");
				}
			}
			return array;
		}

		private bool[] ReadBooleanArray(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			bool[] array = new bool[num];
			int i = (int)(num / 8U);
			int num2 = 0;
			while (i > 0)
			{
				byte b = stream.ReadByte();
				array[num2++] = ((b & 1) == 1);
				array[num2++] = ((b & 2) == 2);
				array[num2++] = ((b & 4) == 4);
				array[num2++] = ((b & 8) == 8);
				array[num2++] = ((b & 16) == 16);
				array[num2++] = ((b & 32) == 32);
				array[num2++] = ((b & 64) == 64);
				array[num2++] = ((b & 128) == 128);
				i--;
			}
			bool flag = (long)num2 < (long)((ulong)num);
			if (flag)
			{
				byte b2 = stream.ReadByte();
				int num3 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					array[num2++] = ((b2 & Protocol18.boolMasks[num3]) == Protocol18.boolMasks[num3]);
					num3++;
				}
			}
			return array;
		}

		internal short[] ReadInt16Array(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			short[] array = new short[num];
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)array.Length))
			{
				array[(int)num2] = this.ReadInt16(stream);
				num2 += 1U;
			}
			return array;
		}

		private float[] ReadSingleArray(StreamBuffer stream)
		{
			int num = (int)this.ReadCompressedUInt32(stream);
			int num2 = num * 4;
			float[] array = new float[num];
			int srcOffset;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(num2, out srcOffset);
			Buffer.BlockCopy(bufferAndAdvance, srcOffset, array, 0, num2);
			return array;
		}

		private double[] ReadDoubleArray(StreamBuffer stream)
		{
			int num = (int)this.ReadCompressedUInt32(stream);
			int num2 = num * 8;
			double[] array = new double[num];
			int srcOffset;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(num2, out srcOffset);
			Buffer.BlockCopy(bufferAndAdvance, srcOffset, array, 0, num2);
			return array;
		}

		internal string[] ReadStringArray(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			string[] array = new string[num];
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)array.Length))
			{
				array[(int)num2] = this.ReadString(stream);
				num2 += 1U;
			}
			return array;
		}

		private Hashtable[] ReadHashtableArray(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			uint num = this.ReadCompressedUInt32(stream);
			Hashtable[] array = new Hashtable[num];
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				array[(int)num2] = this.ReadHashtable(stream, flags, parameters);
			}
			return array;
		}

		private IDictionary[] ReadDictionaryArray(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			Protocol18.GpType keyReadType;
			Protocol18.GpType valueReadType;
			Type type = this.ReadDictionaryType(stream, out keyReadType, out valueReadType);
			uint num = this.ReadCompressedUInt32(stream);
			IDictionary[] array = (IDictionary[])Array.CreateInstance(type, (int)num);
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				array[(int)num2] = (IDictionary)Activator.CreateInstance(type);
				this.ReadDictionaryElements(stream, keyReadType, valueReadType, array[(int)num2], flags, parameters);
			}
			return array;
		}

		private Array ReadArrayInArray(StreamBuffer stream, IProtocol.DeserializationFlags flags, ParameterDictionary parameters)
		{
			uint num = this.ReadCompressedUInt32(stream);
			Array array = null;
			Type type = null;
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				object obj = this.Read(stream, flags, parameters);
				Array array2 = obj as Array;
				bool flag = array2 != null;
				if (flag)
				{
					bool flag2 = array == null;
					if (flag2)
					{
						type = array2.GetType();
						array = Array.CreateInstance(type, (int)num);
					}
					bool flag3 = type.IsAssignableFrom(array2.GetType());
					if (flag3)
					{
						array.SetValue(array2, (int)num2);
					}
				}
			}
			return array;
		}

		internal int ReadInt1(StreamBuffer stream, bool signNegative)
		{
			int result;
			if (signNegative)
			{
				result = (int)(-(int)stream.ReadByte());
			}
			else
			{
				result = (int)stream.ReadByte();
			}
			return result;
		}

		internal int ReadInt2(StreamBuffer stream, bool signNegative)
		{
			int result;
			if (signNegative)
			{
				result = (int)(-(int)this.ReadUShort(stream));
			}
			else
			{
				result = (int)this.ReadUShort(stream);
			}
			return result;
		}

		internal int ReadCompressedInt32(StreamBuffer stream)
		{
			uint value = this.ReadCompressedUInt32(stream);
			return this.DecodeZigZag32(value);
		}

		private uint ReadCompressedUInt32(StreamBuffer stream)
		{
			uint num = 0U;
			int num2 = 0;
			byte[] buffer = stream.GetBuffer();
			int num3 = stream.Position;
			while (num2 != 35)
			{
				bool flag = num3 >= stream.Length;
				if (flag)
				{
					stream.Position = stream.Length;
					throw new EndOfStreamException(string.Concat(new string[]
					{
						"Failed to read full uint. offset: ",
						num3.ToString(),
						" stream.Length: ",
						stream.Length.ToString(),
						" data.Length: ",
						buffer.Length.ToString(),
						" stream.Available: ",
						stream.Available.ToString()
					}));
				}
				byte b = buffer[num3];
				num3++;
				num |= (uint)((uint)(b & 127) << num2);
				num2 += 7;
				bool flag2 = (b & 128) == 0;
				if (flag2)
				{
					break;
				}
			}
			stream.Position = num3;
			return num;
		}

		internal long ReadCompressedInt64(StreamBuffer stream)
		{
			ulong value = this.ReadCompressedUInt64(stream);
			return this.DecodeZigZag64(value);
		}

		private ulong ReadCompressedUInt64(StreamBuffer stream)
		{
			ulong num = 0UL;
			int num2 = 0;
			byte[] buffer = stream.GetBuffer();
			int num3 = stream.Position;
			while (num2 != 70)
			{
				bool flag = num3 >= buffer.Length;
				if (flag)
				{
					throw new EndOfStreamException("Failed to read full ulong.");
				}
				byte b = buffer[num3];
				num3++;
				num |= (ulong)((ulong)((long)(b & 127)) << num2);
				num2 += 7;
				bool flag2 = (b & 128) == 0;
				if (flag2)
				{
					break;
				}
			}
			stream.Position = num3;
			return num;
		}

		internal int[] ReadCompressedInt32Array(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			int[] array = new int[num];
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)array.Length))
			{
				array[(int)num2] = this.ReadCompressedInt32(stream);
				num2 += 1U;
			}
			return array;
		}

		internal long[] ReadCompressedInt64Array(StreamBuffer stream)
		{
			uint num = this.ReadCompressedUInt32(stream);
			long[] array = new long[num];
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)array.Length))
			{
				array[(int)num2] = this.ReadCompressedInt64(stream);
				num2 += 1U;
			}
			return array;
		}

		private int DecodeZigZag32(uint value)
		{
			return (int)((ulong)(value >> 1) ^ -(ulong)(value & 1U));
		}

		private long DecodeZigZag64(ulong value)
		{
			return (long)(value >> 1 ^ -(long)(value & 1UL));
		}

		internal void Write(StreamBuffer stream, object value, bool writeType)
		{
			bool flag = value == null;
			if (flag)
			{
				this.Write(stream, value, Protocol18.GpType.Null, writeType);
			}
			else
			{
				this.Write(stream, value, this.GetCodeOfType(value.GetType()), writeType);
			}
		}

		private void Write(StreamBuffer stream, object value, Protocol18.GpType gpType, bool writeType)
		{
			switch (gpType)
			{
			case Protocol18.GpType.Unknown:
			{
				bool flag = value is ByteArraySlice;
				if (flag)
				{
					ByteArraySlice buffer = (ByteArraySlice)value;
					this.WriteByteArraySlice(stream, buffer, writeType);
					return;
				}
				bool flag2 = value is ArraySegment<byte>;
				if (flag2)
				{
					ArraySegment<byte> seg = (ArraySegment<byte>)value;
					this.WriteArraySegmentByte(stream, seg, writeType);
					return;
				}
				StructWrapper structWrapper = value as StructWrapper;
				bool flag3 = structWrapper != null;
				if (flag3)
				{
					switch (structWrapper.wrappedType)
					{
					case WrappedType.Bool:
						this.WriteBoolean(stream, value.Get<bool>(), writeType);
						break;
					case WrappedType.Byte:
						this.WriteByte(stream, value.Get<byte>(), writeType);
						break;
					case WrappedType.Int16:
						this.WriteInt16(stream, value.Get<short>(), writeType);
						break;
					case WrappedType.Int32:
						this.WriteCompressedInt32(stream, value.Get<int>(), writeType);
						break;
					case WrappedType.Int64:
						this.WriteCompressedInt64(stream, value.Get<long>(), writeType);
						break;
					case WrappedType.Single:
						this.WriteSingle(stream, value.Get<float>(), writeType);
						break;
					case WrappedType.Double:
						this.WriteDouble(stream, value.Get<double>(), writeType);
						break;
					default:
						this.WriteCustomType(stream, value, writeType);
						break;
					}
					return;
				}
				break;
			}
			case (Protocol18.GpType)1:
			case Protocol18.GpType.Int1:
			case Protocol18.GpType.Int1_:
			case Protocol18.GpType.Int2:
			case Protocol18.GpType.Int2_:
			case Protocol18.GpType.L1:
			case Protocol18.GpType.L1_:
			case Protocol18.GpType.L2:
			case Protocol18.GpType.L2_:
			case (Protocol18.GpType)22:
				return;
			case Protocol18.GpType.Boolean:
				this.WriteBoolean(stream, (bool)value, writeType);
				return;
			case Protocol18.GpType.Byte:
				this.WriteByte(stream, (byte)value, writeType);
				return;
			case Protocol18.GpType.Short:
				this.WriteInt16(stream, (short)value, writeType);
				return;
			case Protocol18.GpType.Float:
				this.WriteSingle(stream, (float)value, writeType);
				return;
			case Protocol18.GpType.Double:
				this.WriteDouble(stream, (double)value, writeType);
				return;
			case Protocol18.GpType.String:
				this.WriteString(stream, (string)value, writeType);
				return;
			case Protocol18.GpType.Null:
				if (writeType)
				{
					stream.WriteByte(8);
				}
				return;
			case Protocol18.GpType.CompressedInt:
				this.WriteCompressedInt32(stream, (int)value, writeType);
				return;
			case Protocol18.GpType.CompressedLong:
				this.WriteCompressedInt64(stream, (long)value, writeType);
				return;
			case Protocol18.GpType.Custom:
				break;
			case Protocol18.GpType.Dictionary:
				this.WriteDictionary(stream, (IDictionary)value, writeType);
				return;
			case Protocol18.GpType.Hashtable:
				this.WriteHashtable(stream, (Hashtable)value, writeType);
				return;
			case Protocol18.GpType.ObjectArray:
				this.WriteObjectArray(stream, (IList)value, writeType);
				return;
			case Protocol18.GpType.OperationRequest:
				this.SerializeOperationRequest(stream, (OperationRequest)value, writeType);
				return;
			case Protocol18.GpType.OperationResponse:
				this.SerializeOperationResponse(stream, (OperationResponse)value, writeType);
				return;
			case Protocol18.GpType.EventData:
				this.SerializeEventData(stream, (EventData)value, writeType);
				return;
			default:
				switch (gpType)
				{
				case Protocol18.GpType.Array:
					this.WriteArrayInArray(stream, value, writeType);
					return;
				case (Protocol18.GpType)65:
				case (Protocol18.GpType)72:
				case (Protocol18.GpType)75:
				case (Protocol18.GpType)76:
				case (Protocol18.GpType)77:
				case (Protocol18.GpType)78:
				case (Protocol18.GpType)79:
				case (Protocol18.GpType)80:
				case (Protocol18.GpType)81:
				case (Protocol18.GpType)82:
					return;
				case Protocol18.GpType.BooleanArray:
					this.WriteBoolArray(stream, (bool[])value, writeType);
					return;
				case Protocol18.GpType.ByteArray:
					this.WriteByteArray(stream, (byte[])value, writeType);
					return;
				case Protocol18.GpType.ShortArray:
					this.WriteInt16Array(stream, (short[])value, writeType);
					return;
				case Protocol18.GpType.FloatArray:
					this.WriteSingleArray(stream, (float[])value, writeType);
					return;
				case Protocol18.GpType.DoubleArray:
					this.WriteDoubleArray(stream, (double[])value, writeType);
					return;
				case Protocol18.GpType.StringArray:
					this.WriteStringArray(stream, value, writeType);
					return;
				case Protocol18.GpType.CompressedIntArray:
					this.WriteInt32ArrayCompressed(stream, (int[])value, writeType);
					return;
				case Protocol18.GpType.CompressedLongArray:
					this.WriteInt64ArrayCompressed(stream, (long[])value, writeType);
					return;
				case Protocol18.GpType.CustomTypeArray:
					this.WriteCustomTypeArray(stream, value, writeType);
					return;
				case Protocol18.GpType.DictionaryArray:
					this.WriteDictionaryArray(stream, (IDictionary[])value, writeType);
					return;
				case Protocol18.GpType.HashtableArray:
					this.WriteHashtableArray(stream, value, writeType);
					return;
				default:
					return;
				}
				break;
			}
			this.WriteCustomType(stream, value, writeType);
		}

		public override void SerializeEventData(StreamBuffer stream, EventData serObject, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(26);
			}
			stream.WriteByte(serObject.Code);
			this.WriteParameterTable(stream, serObject.Parameters);
		}

		private void WriteParameterTable(StreamBuffer stream, Dictionary<byte, object> parameters)
		{
			bool flag = parameters == null || parameters.Count == 0;
			if (flag)
			{
				this.WriteByte(stream, 0, false);
			}
			else
			{
				this.WriteByte(stream, (byte)parameters.Count, false);
				foreach (KeyValuePair<byte, object> keyValuePair in parameters)
				{
					stream.WriteByte(keyValuePair.Key);
					this.Write(stream, keyValuePair.Value, true);
				}
			}
		}

		private void WriteParameterTable(StreamBuffer stream, ParameterDictionary parameters)
		{
			bool flag = parameters == null || parameters.Count == 0;
			if (flag)
			{
				this.WriteByte(stream, 0, false);
			}
			else
			{
				this.WriteByte(stream, (byte)parameters.Count, false);
				foreach (KeyValuePair<byte, object> keyValuePair in parameters)
				{
					stream.WriteByte(keyValuePair.Key);
					this.Write(stream, keyValuePair.Value, true);
				}
			}
		}

		private void SerializeOperationRequest(StreamBuffer stream, OperationRequest operation, bool setType)
		{
			this.SerializeOperationRequest(stream, operation.OperationCode, operation.Parameters, setType);
		}

		[Obsolete("Use ParameterDictionary instead.")]
		public override void SerializeOperationRequest(StreamBuffer stream, byte operationCode, Dictionary<byte, object> parameters, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(24);
			}
			stream.WriteByte(operationCode);
			this.WriteParameterTable(stream, parameters);
		}

		public override void SerializeOperationRequest(StreamBuffer stream, byte operationCode, ParameterDictionary parameters, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(24);
			}
			stream.WriteByte(operationCode);
			this.WriteParameterTable(stream, parameters);
		}

		public override void SerializeOperationResponse(StreamBuffer stream, OperationResponse serObject, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(25);
			}
			stream.WriteByte(serObject.OperationCode);
			this.WriteInt16(stream, serObject.ReturnCode, false);
			bool flag = string.IsNullOrEmpty(serObject.DebugMessage);
			if (flag)
			{
				stream.WriteByte(8);
			}
			else
			{
				stream.WriteByte(7);
				this.WriteString(stream, serObject.DebugMessage, false);
			}
			this.WriteParameterTable(stream, serObject.Parameters);
		}

		internal void WriteByte(StreamBuffer stream, byte value, bool writeType)
		{
			if (writeType)
			{
				bool flag = value == 0;
				if (flag)
				{
					stream.WriteByte(34);
					return;
				}
				stream.WriteByte(3);
			}
			stream.WriteByte(value);
		}

		internal void WriteBoolean(StreamBuffer stream, bool value, bool writeType)
		{
			if (writeType)
			{
				if (value)
				{
					stream.WriteByte(28);
				}
				else
				{
					stream.WriteByte(27);
				}
			}
			else
			{
				stream.WriteByte(value ? 1 : 0);
			}
		}

		internal void WriteUShort(StreamBuffer stream, ushort value)
		{
			stream.WriteBytes((byte)value, (byte)(value >> 8));
		}

		internal void WriteInt16(StreamBuffer stream, short value, bool writeType)
		{
			if (writeType)
			{
				bool flag = value == 0;
				if (flag)
				{
					stream.WriteByte(29);
					return;
				}
				stream.WriteByte(4);
			}
			stream.WriteBytes((byte)value, (byte)(value >> 8));
		}

		internal void WriteDouble(StreamBuffer stream, double value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(6);
			}
			int dstOffset;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(8, out dstOffset);
			double[] obj = this.memDoubleBlock;
			lock (obj)
			{
				this.memDoubleBlock[0] = value;
				Buffer.BlockCopy(this.memDoubleBlock, 0, bufferAndAdvance, dstOffset, 8);
			}
		}

		internal void WriteSingle(StreamBuffer stream, float value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(5);
			}
			int dstOffset;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out dstOffset);
			float[] obj = this.memFloatBlock;
			lock (obj)
			{
				this.memFloatBlock[0] = value;
				Buffer.BlockCopy(this.memFloatBlock, 0, bufferAndAdvance, dstOffset, 4);
			}
		}

		internal void WriteString(StreamBuffer stream, string value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(7);
			}
			int byteCount = Encoding.UTF8.GetByteCount(value);
			bool flag = byteCount > 32767;
			if (flag)
			{
				throw new NotSupportedException("Strings that exceed a UTF8-encoded byte-length of 32767 (short.MaxValue) are not supported. Yours is: " + byteCount.ToString());
			}
			this.WriteIntLength(stream, byteCount);
			int byteIndex = 0;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(byteCount, out byteIndex);
			Encoding.UTF8.GetBytes(value, 0, value.Length, bufferAndAdvance, byteIndex);
		}

		private void WriteHashtable(StreamBuffer stream, object value, bool writeType)
		{
			Hashtable hashtable = (Hashtable)value;
			if (writeType)
			{
				stream.WriteByte(21);
			}
			this.WriteIntLength(stream, hashtable.Count);
			Dictionary<object, object>.KeyCollection keys = hashtable.Keys;
			foreach (object obj in keys)
			{
				this.Write(stream, obj, true);
				this.Write(stream, hashtable[obj], true);
			}
		}

		internal void WriteByteArray(StreamBuffer stream, byte[] value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(67);
			}
			this.WriteIntLength(stream, value.Length);
			stream.Write(value, 0, value.Length);
		}

		private void WriteArraySegmentByte(StreamBuffer stream, ArraySegment<byte> seg, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(67);
			}
			int count = seg.Count;
			this.WriteIntLength(stream, count);
			bool flag = count > 0;
			if (flag)
			{
				stream.Write(seg.Array, seg.Offset, count);
			}
		}

		private void WriteByteArraySlice(StreamBuffer stream, ByteArraySlice buffer, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(67);
			}
			int count = buffer.Count;
			this.WriteIntLength(stream, count);
			stream.Write(buffer.Buffer, buffer.Offset, count);
			buffer.Release();
		}

		internal void WriteInt32ArrayCompressed(StreamBuffer stream, int[] value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(73);
			}
			this.WriteIntLength(stream, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				this.WriteCompressedInt32(stream, value[i], false);
			}
		}

		private void WriteInt64ArrayCompressed(StreamBuffer stream, long[] values, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(74);
			}
			this.WriteIntLength(stream, values.Length);
			for (int i = 0; i < values.Length; i++)
			{
				this.WriteCompressedInt64(stream, values[i], false);
			}
		}

		internal void WriteBoolArray(StreamBuffer stream, bool[] value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(66);
			}
			this.WriteIntLength(stream, value.Length);
			int i = value.Length >> 3;
			uint num = (uint)(i + 1);
			byte[] array = new byte[num];
			int num2 = 0;
			int j = 0;
			while (i > 0)
			{
				byte b = 0;
				bool flag = value[j++];
				if (flag)
				{
					b |= 1;
				}
				bool flag2 = value[j++];
				if (flag2)
				{
					b |= 2;
				}
				bool flag3 = value[j++];
				if (flag3)
				{
					b |= 4;
				}
				bool flag4 = value[j++];
				if (flag4)
				{
					b |= 8;
				}
				bool flag5 = value[j++];
				if (flag5)
				{
					b |= 16;
				}
				bool flag6 = value[j++];
				if (flag6)
				{
					b |= 32;
				}
				bool flag7 = value[j++];
				if (flag7)
				{
					b |= 64;
				}
				bool flag8 = value[j++];
				if (flag8)
				{
					b |= 128;
				}
				array[num2] = b;
				i--;
				num2++;
			}
			bool flag9 = j < value.Length;
			if (flag9)
			{
				byte b2 = 0;
				int num3 = 0;
				while (j < value.Length)
				{
					bool flag10 = value[j];
					if (flag10)
					{
						b2 |= (byte)(1 << num3);
					}
					num3++;
					j++;
				}
				array[num2] = b2;
				num2++;
			}
			stream.Write(array, 0, num2);
		}

		internal void WriteInt16Array(StreamBuffer stream, short[] value, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(68);
			}
			this.WriteIntLength(stream, value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				this.WriteInt16(stream, value[i], false);
			}
		}

		internal void WriteSingleArray(StreamBuffer stream, float[] values, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(69);
			}
			this.WriteIntLength(stream, values.Length);
			int num = values.Length * 4;
			int dstOffset;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(num, out dstOffset);
			Buffer.BlockCopy(values, 0, bufferAndAdvance, dstOffset, num);
		}

		internal void WriteDoubleArray(StreamBuffer stream, double[] values, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(70);
			}
			this.WriteIntLength(stream, values.Length);
			int num = values.Length * 8;
			int dstOffset;
			byte[] bufferAndAdvance = stream.GetBufferAndAdvance(num, out dstOffset);
			Buffer.BlockCopy(values, 0, bufferAndAdvance, dstOffset, num);
		}

		internal void WriteStringArray(StreamBuffer stream, object value0, bool writeType)
		{
			string[] array = (string[])value0;
			if (writeType)
			{
				stream.WriteByte(71);
			}
			this.WriteIntLength(stream, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = array[i] == null;
				if (flag)
				{
					throw new InvalidDataException("Unexpected - cannot serialize string array with null element " + i.ToString());
				}
				this.WriteString(stream, array[i], false);
			}
		}

		private void WriteObjectArray(StreamBuffer stream, object array, bool writeType)
		{
			this.WriteObjectArray(stream, (IList)array, writeType);
		}

		private void WriteObjectArray(StreamBuffer stream, IList array, bool writeType)
		{
			if (writeType)
			{
				stream.WriteByte(23);
			}
			this.WriteIntLength(stream, array.Count);
			for (int i = 0; i < array.Count; i++)
			{
				object value = array[i];
				this.Write(stream, value, true);
			}
		}

		private void WriteArrayInArray(StreamBuffer stream, object value, bool writeType)
		{
			object[] array = (object[])value;
			stream.WriteByte(64);
			this.WriteIntLength(stream, array.Length);
			foreach (object value2 in array)
			{
				this.Write(stream, value2, true);
			}
		}

		private void WriteCustomTypeBody(CustomType customType, StreamBuffer stream, object value)
		{
			bool flag = customType.SerializeStreamFunction == null;
			if (flag)
			{
				byte[] array = customType.SerializeFunction(value);
				this.WriteIntLength(stream, array.Length);
				stream.Write(array, 0, array.Length);
			}
			else
			{
				int position = stream.Position;
				stream.Position++;
				uint num = (uint)customType.SerializeStreamFunction(stream, value);
				int num2 = stream.Position - position - 1;
				bool flag2 = (long)num2 != (long)((ulong)num);
				if (flag2)
				{
					string[] array2 = new string[7];
					array2[0] = "Serialization for Custom Type '";
					int num3 = 1;
					Type type = value.GetType();
					array2[num3] = ((type != null) ? type.ToString() : null);
					array2[2] = "' returns size ";
					array2[3] = num.ToString();
					array2[4] = " bytes but wrote ";
					array2[5] = num2.ToString();
					array2[6] = " bytes. Sending the latter as size.";
					Debug.WriteLine(string.Concat(array2));
				}
				int num4 = this.WriteCompressedUInt32(this.memCustomTypeBodyLengthSerialized, (uint)num2);
				bool flag3 = num4 == 1;
				if (flag3)
				{
					stream.GetBuffer()[position] = this.memCustomTypeBodyLengthSerialized[0];
				}
				else
				{
					for (int i = 0; i < num4 - 1; i++)
					{
						stream.WriteByte(0);
					}
					Buffer.BlockCopy(stream.GetBuffer(), position + 1, stream.GetBuffer(), position + num4, num2);
					Buffer.BlockCopy(this.memCustomTypeBodyLengthSerialized, 0, stream.GetBuffer(), position, num4);
					stream.Position = position + num4 + num2;
				}
			}
		}

		private void WriteCustomType(StreamBuffer stream, object value, bool writeType)
		{
			StructWrapper structWrapper = value as StructWrapper;
			bool flag = structWrapper != null;
			Type type;
			if (flag)
			{
				type = structWrapper.ttype;
			}
			else
			{
				type = value.GetType();
			}
			CustomType customType;
			bool flag2 = Protocol.TypeDict.TryGetValue(type, out customType);
			if (flag2)
			{
				if (writeType)
				{
					bool flag3 = customType.Code < 100;
					if (flag3)
					{
						stream.WriteByte(128 + customType.Code);
					}
					else
					{
						stream.WriteByte(19);
						stream.WriteByte(customType.Code);
					}
				}
				else
				{
					stream.WriteByte(customType.Code);
				}
				this.WriteCustomTypeBody(customType, stream, value);
				return;
			}
			string str = "Write failed. Custom type not found: ";
			Type type2 = type;
			throw new Exception(str + ((type2 != null) ? type2.ToString() : null));
		}

		private void WriteCustomTypeArray(StreamBuffer stream, object value, bool writeType)
		{
			IList list = (IList)value;
			Type elementType = value.GetType().GetElementType();
			CustomType customType;
			bool flag = Protocol.TypeDict.TryGetValue(elementType, out customType);
			if (flag)
			{
				if (writeType)
				{
					stream.WriteByte(83);
				}
				this.WriteIntLength(stream, list.Count);
				stream.WriteByte(customType.Code);
				foreach (object value2 in list)
				{
					this.WriteCustomTypeBody(customType, stream, value2);
				}
				return;
			}
			string str = "Write failed. Custom type of element not found: ";
			Type type = elementType;
			throw new Exception(str + ((type != null) ? type.ToString() : null));
		}

		private bool WriteArrayHeader(StreamBuffer stream, Type type)
		{
			Type elementType = type.GetElementType();
			while (elementType.IsArray)
			{
				stream.WriteByte(64);
				elementType = elementType.GetElementType();
			}
			Protocol18.GpType codeOfType = this.GetCodeOfType(elementType);
			bool flag = codeOfType == Protocol18.GpType.Unknown;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				stream.WriteByte((byte)(codeOfType | Protocol18.GpType.CustomTypeSlim));
				result = true;
			}
			return result;
		}

		private void WriteDictionaryElements(StreamBuffer stream, IDictionary dictionary, Protocol18.GpType keyWriteType, Protocol18.GpType valueWriteType)
		{
			this.WriteIntLength(stream, dictionary.Count);
			foreach (object obj in dictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				this.Write(stream, dictionaryEntry.Key, keyWriteType == Protocol18.GpType.Unknown);
				this.Write(stream, dictionaryEntry.Value, valueWriteType == Protocol18.GpType.Unknown);
			}
		}

		private void WriteDictionary(StreamBuffer stream, object dict, bool setType)
		{
			if (setType)
			{
				stream.WriteByte(20);
			}
			Protocol18.GpType keyWriteType;
			Protocol18.GpType valueWriteType;
			this.WriteDictionaryHeader(stream, dict.GetType(), out keyWriteType, out valueWriteType);
			IDictionary dictionary = (IDictionary)dict;
			this.WriteDictionaryElements(stream, dictionary, keyWriteType, valueWriteType);
		}

		private void WriteDictionaryHeader(StreamBuffer stream, Type type, out Protocol18.GpType keyWriteType, out Protocol18.GpType valueWriteType)
		{
			Type[] genericArguments = type.GetGenericArguments();
			bool flag = genericArguments[0] == typeof(object);
			if (flag)
			{
				stream.WriteByte(0);
				keyWriteType = Protocol18.GpType.Unknown;
			}
			else
			{
				bool flag2 = !genericArguments[0].IsPrimitive && genericArguments[0] != typeof(string);
				if (flag2)
				{
					string str = "Unexpected - cannot serialize Dictionary with key type: ";
					Type type2 = genericArguments[0];
					throw new InvalidDataException(str + ((type2 != null) ? type2.ToString() : null));
				}
				keyWriteType = this.GetCodeOfType(genericArguments[0]);
				bool flag3 = keyWriteType == Protocol18.GpType.Unknown;
				if (flag3)
				{
					string str2 = "Unexpected - cannot serialize Dictionary with key type: ";
					Type type3 = genericArguments[0];
					throw new InvalidDataException(str2 + ((type3 != null) ? type3.ToString() : null));
				}
				stream.WriteByte((byte)keyWriteType);
			}
			bool flag4 = genericArguments[1] == typeof(object);
			if (flag4)
			{
				stream.WriteByte(0);
				valueWriteType = Protocol18.GpType.Unknown;
			}
			else
			{
				bool isArray = genericArguments[1].IsArray;
				if (isArray)
				{
					bool flag5 = !this.WriteArrayType(stream, genericArguments[1], out valueWriteType);
					if (flag5)
					{
						string str3 = "Unexpected - cannot serialize Dictionary with value type: ";
						Type type4 = genericArguments[1];
						throw new InvalidDataException(str3 + ((type4 != null) ? type4.ToString() : null));
					}
				}
				else
				{
					valueWriteType = this.GetCodeOfType(genericArguments[1]);
					bool flag6 = valueWriteType == Protocol18.GpType.Unknown;
					if (flag6)
					{
						string str4 = "Unexpected - cannot serialize Dictionary with value type: ";
						Type type5 = genericArguments[1];
						throw new InvalidDataException(str4 + ((type5 != null) ? type5.ToString() : null));
					}
					bool flag7 = valueWriteType == Protocol18.GpType.Array;
					if (flag7)
					{
						bool flag8 = !this.WriteArrayHeader(stream, genericArguments[1]);
						if (flag8)
						{
							string str5 = "Unexpected - cannot serialize Dictionary with value type: ";
							Type type6 = genericArguments[1];
							throw new InvalidDataException(str5 + ((type6 != null) ? type6.ToString() : null));
						}
					}
					else
					{
						bool flag9 = valueWriteType == Protocol18.GpType.Dictionary;
						if (flag9)
						{
							stream.WriteByte((byte)valueWriteType);
							Protocol18.GpType gpType;
							Protocol18.GpType gpType2;
							this.WriteDictionaryHeader(stream, genericArguments[1], out gpType, out gpType2);
						}
						else
						{
							stream.WriteByte((byte)valueWriteType);
						}
					}
				}
			}
		}

		private bool WriteArrayType(StreamBuffer stream, Type type, out Protocol18.GpType writeType)
		{
			Type elementType = type.GetElementType();
			bool flag = elementType == null;
			if (flag)
			{
				throw new InvalidDataException("Unexpected - cannot serialize array with type: " + ((type != null) ? type.ToString() : null));
			}
			bool isArray = elementType.IsArray;
			bool result;
			if (isArray)
			{
				while (elementType != null && elementType.IsArray)
				{
					stream.WriteByte(64);
					elementType = elementType.GetElementType();
				}
				byte value = (byte)(this.GetCodeOfType(elementType) | Protocol18.GpType.Array);
				stream.WriteByte(value);
				writeType = Protocol18.GpType.Array;
				result = true;
			}
			else
			{
				bool isPrimitive = elementType.IsPrimitive;
				if (isPrimitive)
				{
					byte b = (byte)(this.GetCodeOfType(elementType) | Protocol18.GpType.Array);
					bool flag2 = b == 226;
					if (flag2)
					{
						b = 67;
					}
					stream.WriteByte(b);
					bool flag3 = Enum.IsDefined(typeof(Protocol18.GpType), b);
					if (flag3)
					{
						writeType = (Protocol18.GpType)b;
						result = true;
					}
					else
					{
						writeType = Protocol18.GpType.Unknown;
						result = false;
					}
				}
				else
				{
					bool flag4 = elementType == typeof(string);
					if (flag4)
					{
						stream.WriteByte(71);
						writeType = Protocol18.GpType.StringArray;
						result = true;
					}
					else
					{
						bool flag5 = elementType == typeof(object);
						if (flag5)
						{
							stream.WriteByte(23);
							writeType = Protocol18.GpType.ObjectArray;
							result = true;
						}
						else
						{
							bool flag6 = elementType == typeof(Hashtable);
							if (flag6)
							{
								stream.WriteByte(85);
								writeType = Protocol18.GpType.HashtableArray;
								result = true;
							}
							else
							{
								writeType = Protocol18.GpType.Unknown;
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		private void WriteHashtableArray(StreamBuffer stream, object value, bool writeType)
		{
			Hashtable[] array = (Hashtable[])value;
			if (writeType)
			{
				stream.WriteByte(85);
			}
			this.WriteIntLength(stream, array.Length);
			foreach (Hashtable value2 in array)
			{
				this.WriteHashtable(stream, value2, false);
			}
		}

		private void WriteDictionaryArray(StreamBuffer stream, IDictionary[] dictArray, bool writeType)
		{
			stream.WriteByte(84);
			Protocol18.GpType keyWriteType;
			Protocol18.GpType valueWriteType;
			this.WriteDictionaryHeader(stream, dictArray.GetType().GetElementType(), out keyWriteType, out valueWriteType);
			this.WriteIntLength(stream, dictArray.Length);
			foreach (IDictionary dictionary in dictArray)
			{
				this.WriteDictionaryElements(stream, dictionary, keyWriteType, valueWriteType);
			}
		}

		private void WriteIntLength(StreamBuffer stream, int value)
		{
			this.WriteCompressedUInt32(stream, (uint)value);
		}

		private void WriteVarInt32(StreamBuffer stream, int value, bool writeType)
		{
			this.WriteCompressedInt32(stream, value, writeType);
		}

		private void WriteCompressedInt32(StreamBuffer stream, int value, bool writeType)
		{
			if (writeType)
			{
				bool flag = value == 0;
				if (flag)
				{
					stream.WriteByte(30);
					return;
				}
				bool flag2 = value > 0;
				if (flag2)
				{
					bool flag3 = value <= 255;
					if (flag3)
					{
						stream.WriteByte(11);
						stream.WriteByte((byte)value);
						return;
					}
					bool flag4 = value <= 65535;
					if (flag4)
					{
						stream.WriteByte(13);
						this.WriteUShort(stream, (ushort)value);
						return;
					}
				}
				else
				{
					bool flag5 = value >= -65535;
					if (flag5)
					{
						bool flag6 = value >= -255;
						if (flag6)
						{
							stream.WriteByte(12);
							stream.WriteByte((byte)(-(byte)value));
							return;
						}
						bool flag7 = value >= -65535;
						if (flag7)
						{
							stream.WriteByte(14);
							this.WriteUShort(stream, (ushort)(-(ushort)value));
							return;
						}
					}
				}
			}
			if (writeType)
			{
				stream.WriteByte(9);
			}
			uint value2 = this.EncodeZigZag32(value);
			this.WriteCompressedUInt32(stream, value2);
		}

		private void WriteCompressedInt64(StreamBuffer stream, long value, bool writeType)
		{
			if (writeType)
			{
				bool flag = value == 0L;
				if (flag)
				{
					stream.WriteByte(31);
					return;
				}
				bool flag2 = value > 0L;
				if (flag2)
				{
					bool flag3 = value <= 255L;
					if (flag3)
					{
						stream.WriteByte(15);
						stream.WriteByte((byte)value);
						return;
					}
					bool flag4 = value <= 65535L;
					if (flag4)
					{
						stream.WriteByte(17);
						this.WriteUShort(stream, (ushort)value);
						return;
					}
				}
				else
				{
					bool flag5 = value >= -65535L;
					if (flag5)
					{
						bool flag6 = value >= -255L;
						if (flag6)
						{
							stream.WriteByte(16);
							stream.WriteByte((byte)(-(byte)value));
							return;
						}
						bool flag7 = value >= -65535L;
						if (flag7)
						{
							stream.WriteByte(18);
							this.WriteUShort(stream, (ushort)(-(ushort)value));
							return;
						}
					}
				}
			}
			if (writeType)
			{
				stream.WriteByte(10);
			}
			ulong value2 = this.EncodeZigZag64(value);
			this.WriteCompressedUInt64(stream, value2);
		}

		private void WriteCompressedUInt32(StreamBuffer stream, uint value)
		{
			byte[] obj = this.memCompressedUInt32;
			lock (obj)
			{
				stream.Write(this.memCompressedUInt32, 0, this.WriteCompressedUInt32(this.memCompressedUInt32, value));
			}
		}

		private int WriteCompressedUInt32(byte[] buffer, uint value)
		{
			int num = 0;
			buffer[num] = (byte)(value & 127U);
			for (value >>= 7; value > 0U; value >>= 7)
			{
				int num2 = num;
				buffer[num2] |= 128;
				buffer[++num] = (byte)(value & 127U);
			}
			return num + 1;
		}

		private void WriteCompressedUInt64(StreamBuffer stream, ulong value)
		{
			int num = 0;
			byte[] obj = this.memCompressedUInt64;
			lock (obj)
			{
				this.memCompressedUInt64[num] = (byte)(value & 127UL);
				for (value >>= 7; value > 0UL; value >>= 7)
				{
					byte[] array = this.memCompressedUInt64;
					int num2 = num;
					array[num2] |= 128;
					this.memCompressedUInt64[++num] = (byte)(value & 127UL);
				}
				num++;
				stream.Write(this.memCompressedUInt64, 0, num);
			}
		}

		private uint EncodeZigZag32(int value)
		{
			return (uint)(value << 1 ^ value >> 31);
		}

		private ulong EncodeZigZag64(long value)
		{
			return (ulong)(value << 1 ^ value >> 63);
		}

		private readonly byte[] versionBytes = new byte[]
		{
			1,
			8
		};

		private static readonly byte[] boolMasks = new byte[]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64,
			128
		};

		private readonly double[] memDoubleBlock = new double[1];

		private readonly float[] memFloatBlock = new float[1];

		private readonly byte[] memCustomTypeBodyLengthSerialized = new byte[5];

		private readonly byte[] memCompressedUInt32 = new byte[5];

		private byte[] memCompressedUInt64 = new byte[10];

		public enum GpType : byte
		{
			Unknown,
			Boolean = 2,
			Byte,
			Short,
			Float,
			Double,
			String,
			Null,
			CompressedInt,
			CompressedLong,
			Int1,
			Int1_,
			Int2,
			Int2_,
			L1,
			L1_,
			L2,
			L2_,
			Custom,
			CustomTypeSlim = 128,
			Dictionary = 20,
			Hashtable,
			ObjectArray = 23,
			OperationRequest,
			OperationResponse,
			EventData,
			BooleanFalse,
			BooleanTrue,
			ShortZero,
			IntZero,
			LongZero,
			FloatZero,
			DoubleZero,
			ByteZero,
			Array = 64,
			BooleanArray = 66,
			ByteArray,
			ShortArray,
			DoubleArray = 70,
			FloatArray = 69,
			StringArray = 71,
			HashtableArray = 85,
			DictionaryArray = 84,
			CustomTypeArray = 83,
			CompressedIntArray = 73,
			CompressedLongArray
		}
	}
}
