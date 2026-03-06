using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct InputStateBlock
	{
		public static int GetSizeOfPrimitiveFormatInBits(FourCC type)
		{
			if (type == InputStateBlock.FormatBit || type == InputStateBlock.FormatSBit)
			{
				return 1;
			}
			if (type == InputStateBlock.FormatInt || type == InputStateBlock.FormatUInt)
			{
				return 32;
			}
			if (type == InputStateBlock.FormatShort || type == InputStateBlock.FormatUShort)
			{
				return 16;
			}
			if (type == InputStateBlock.FormatByte || type == InputStateBlock.FormatSByte)
			{
				return 8;
			}
			if (type == InputStateBlock.FormatLong || type == InputStateBlock.FormatULong)
			{
				return 64;
			}
			if (type == InputStateBlock.FormatFloat)
			{
				return 32;
			}
			if (type == InputStateBlock.FormatDouble)
			{
				return 64;
			}
			if (type == InputStateBlock.FormatVector2)
			{
				return 64;
			}
			if (type == InputStateBlock.FormatVector3)
			{
				return 96;
			}
			if (type == InputStateBlock.FormatQuaternion)
			{
				return 128;
			}
			if (type == InputStateBlock.FormatVector2Short)
			{
				return 32;
			}
			if (type == InputStateBlock.FormatVector3Short)
			{
				return 48;
			}
			if (type == InputStateBlock.FormatVector2Byte)
			{
				return 16;
			}
			if (type == InputStateBlock.FormatVector3Byte)
			{
				return 24;
			}
			return -1;
		}

		public static FourCC GetPrimitiveFormatFromType(Type type)
		{
			if (type == typeof(int))
			{
				return InputStateBlock.FormatInt;
			}
			if (type == typeof(uint))
			{
				return InputStateBlock.FormatUInt;
			}
			if (type == typeof(short))
			{
				return InputStateBlock.FormatShort;
			}
			if (type == typeof(ushort))
			{
				return InputStateBlock.FormatUShort;
			}
			if (type == typeof(byte))
			{
				return InputStateBlock.FormatByte;
			}
			if (type == typeof(sbyte))
			{
				return InputStateBlock.FormatSByte;
			}
			if (type == typeof(long))
			{
				return InputStateBlock.FormatLong;
			}
			if (type == typeof(ulong))
			{
				return InputStateBlock.FormatULong;
			}
			if (type == typeof(float))
			{
				return InputStateBlock.FormatFloat;
			}
			if (type == typeof(double))
			{
				return InputStateBlock.FormatDouble;
			}
			if (type == typeof(Vector2))
			{
				return InputStateBlock.FormatVector2;
			}
			if (type == typeof(Vector3))
			{
				return InputStateBlock.FormatVector3;
			}
			if (type == typeof(Quaternion))
			{
				return InputStateBlock.FormatQuaternion;
			}
			return default(FourCC);
		}

		public FourCC format { readonly get; set; }

		public uint byteOffset
		{
			get
			{
				return this.m_ByteOffset;
			}
			set
			{
				this.m_ByteOffset = value;
			}
		}

		public uint bitOffset { readonly get; set; }

		public uint sizeInBits { readonly get; set; }

		internal uint alignedSizeInBytes
		{
			get
			{
				return this.sizeInBits + 7U >> 3;
			}
		}

		internal uint effectiveByteOffset
		{
			get
			{
				return this.byteOffset + (this.bitOffset >> 3);
			}
		}

		internal uint effectiveBitOffset
		{
			get
			{
				return this.byteOffset * 8U + this.bitOffset;
			}
		}

		public unsafe int ReadInt(void* statePtr)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1396853076)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num != 1113150533)
						{
							goto IL_FA;
						}
						return (int)(*ptr);
					}
					else
					{
						if (this.sizeInBits != 1U)
						{
							return (int)MemoryHelpers.ReadMultipleBitsAsUInt((void*)ptr, this.bitOffset, this.sizeInBits);
						}
						if (!MemoryHelpers.ReadSingleBit((void*)ptr, this.bitOffset))
						{
							return 0;
						}
						return 1;
					}
				}
				else if (num != 1229870112)
				{
					if (num != 1396853076)
					{
						goto IL_FA;
					}
					if (this.sizeInBits != 1U)
					{
						return MemoryHelpers.ReadExcessKMultipleBitsAsInt((void*)ptr, this.bitOffset, this.sizeInBits);
					}
					if (!MemoryHelpers.ReadSingleBit((void*)ptr, this.bitOffset))
					{
						return -1;
					}
					return 1;
				}
			}
			else if (num <= 1397248596)
			{
				if (num == 1396857172)
				{
					return (int)(*(sbyte*)ptr);
				}
				if (num != 1397248596)
				{
					goto IL_FA;
				}
				return (int)(*(short*)ptr);
			}
			else if (num != 1430867540)
			{
				if (num != 1431521364)
				{
					goto IL_FA;
				}
				return (int)(*(ushort*)ptr);
			}
			return *(int*)ptr;
			IL_FA:
			throw new InvalidOperationException(string.Format("State format '{0}' is not supported as integer format", this.format));
		}

		public unsafe void WriteInt(void* statePtr, int value)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1396853076)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num != 1113150533)
						{
							goto IL_FB;
						}
						*ptr = (byte)value;
						return;
					}
					else
					{
						if (this.sizeInBits == 1U)
						{
							MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value != 0);
							return;
						}
						MemoryHelpers.WriteUIntAsMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, (uint)value);
						return;
					}
				}
				else if (num != 1229870112)
				{
					if (num != 1396853076)
					{
						goto IL_FB;
					}
					if (this.sizeInBits == 1U)
					{
						MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value > 0);
						return;
					}
					MemoryHelpers.WriteIntAsExcessKMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, value);
					return;
				}
			}
			else if (num <= 1397248596)
			{
				if (num == 1396857172)
				{
					*ptr = (byte)((sbyte)value);
					return;
				}
				if (num != 1397248596)
				{
					goto IL_FB;
				}
				*(short*)ptr = (short)value;
				return;
			}
			else if (num != 1430867540)
			{
				if (num != 1431521364)
				{
					goto IL_FB;
				}
				*(short*)ptr = (short)((ushort)value);
				return;
			}
			*(int*)ptr = value;
			return;
			IL_FB:
			throw new Exception(string.Format("State format '{0}' is not supported as integer format", this.format));
		}

		public unsafe float ReadFloat(void* statePtr)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1229870112)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num == 1113150533)
						{
							return NumberHelpers.UIntToNormalizedFloat((uint)(*ptr), 0U, 255U);
						}
					}
					else
					{
						if (this.sizeInBits != 1U)
						{
							return MemoryHelpers.ReadMultipleBitsAsNormalizedUInt((void*)ptr, this.bitOffset, this.sizeInBits);
						}
						if (!MemoryHelpers.ReadSingleBit((void*)ptr, this.bitOffset))
						{
							return 0f;
						}
						return 1f;
					}
				}
				else
				{
					if (num == 1145195552)
					{
						return (float)(*(double*)ptr);
					}
					if (num == 1179407392)
					{
						return *(float*)ptr;
					}
					if (num == 1229870112)
					{
						return NumberHelpers.IntToNormalizedFloat(*(int*)ptr, int.MinValue, int.MaxValue) * 2f - 1f;
					}
				}
			}
			else if (num <= 1396857172)
			{
				if (num != 1396853076)
				{
					if (num == 1396857172)
					{
						return NumberHelpers.IntToNormalizedFloat((int)(*(sbyte*)ptr), -128, 127) * 2f - 1f;
					}
				}
				else
				{
					if (this.sizeInBits != 1U)
					{
						return MemoryHelpers.ReadMultipleBitsAsNormalizedUInt((void*)ptr, this.bitOffset, this.sizeInBits) * 2f - 1f;
					}
					if (!MemoryHelpers.ReadSingleBit((void*)ptr, this.bitOffset))
					{
						return -1f;
					}
					return 1f;
				}
			}
			else
			{
				if (num == 1397248596)
				{
					return NumberHelpers.IntToNormalizedFloat((int)(*(short*)ptr), -32768, 32767) * 2f - 1f;
				}
				if (num == 1430867540)
				{
					return NumberHelpers.UIntToNormalizedFloat(*(uint*)ptr, 0U, uint.MaxValue);
				}
				if (num == 1431521364)
				{
					return NumberHelpers.UIntToNormalizedFloat((uint)(*(ushort*)ptr), 0U, 65535U);
				}
			}
			throw new InvalidOperationException(string.Format("State format '{0}' is not supported as floating-point format", this.format));
		}

		public unsafe void WriteFloat(void* statePtr, float value)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1229870112)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num == 1113150533)
						{
							*ptr = (byte)NumberHelpers.NormalizedFloatToUInt(value, 0U, 255U);
							return;
						}
					}
					else
					{
						if (this.sizeInBits == 1U)
						{
							MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value >= 0.5f);
							return;
						}
						MemoryHelpers.WriteNormalizedUIntAsMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, value);
						return;
					}
				}
				else
				{
					if (num == 1145195552)
					{
						*(double*)ptr = (double)value;
						return;
					}
					if (num == 1179407392)
					{
						*(float*)ptr = value;
						return;
					}
					if (num == 1229870112)
					{
						*(int*)ptr = NumberHelpers.NormalizedFloatToInt(value * 0.5f + 0.5f, int.MinValue, int.MaxValue);
						return;
					}
				}
			}
			else if (num <= 1396857172)
			{
				if (num != 1396853076)
				{
					if (num == 1396857172)
					{
						*ptr = (byte)((sbyte)NumberHelpers.NormalizedFloatToInt(value * 0.5f + 0.5f, -128, 127));
						return;
					}
				}
				else
				{
					if (this.sizeInBits == 1U)
					{
						MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value >= 0f);
						return;
					}
					MemoryHelpers.WriteNormalizedUIntAsMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, value * 0.5f + 0.5f);
					return;
				}
			}
			else
			{
				if (num == 1397248596)
				{
					*(short*)ptr = (short)NumberHelpers.NormalizedFloatToInt(value * 0.5f + 0.5f, -32768, 32767);
					return;
				}
				if (num == 1430867540)
				{
					*(int*)ptr = (int)NumberHelpers.NormalizedFloatToUInt(value, 0U, uint.MaxValue);
					return;
				}
				if (num == 1431521364)
				{
					*(short*)ptr = (short)((ushort)NumberHelpers.NormalizedFloatToUInt(value, 0U, 65535U));
					return;
				}
			}
			throw new Exception(string.Format("State format '{0}' is not supported as floating-point format", this.format));
		}

		internal PrimitiveValue FloatToPrimitiveValue(float value)
		{
			int num = this.format;
			if (num <= 1229870112)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num == 1113150533)
						{
							return (byte)NumberHelpers.NormalizedFloatToUInt(value, 0U, 255U);
						}
					}
					else
					{
						if (this.sizeInBits == 1U)
						{
							return value >= 0.5f;
						}
						return (int)NumberHelpers.NormalizedFloatToUInt(value, 0U, (uint)((1L << (int)this.sizeInBits) - 1L));
					}
				}
				else
				{
					if (num == 1145195552)
					{
						return value;
					}
					if (num == 1179407392)
					{
						return value;
					}
					if (num == 1229870112)
					{
						return NumberHelpers.NormalizedFloatToInt(value * 0.5f + 0.5f, int.MinValue, int.MaxValue);
					}
				}
			}
			else if (num <= 1396857172)
			{
				if (num != 1396853076)
				{
					if (num == 1396857172)
					{
						return (sbyte)NumberHelpers.NormalizedFloatToInt(value * 0.5f + 0.5f, -128, 127);
					}
				}
				else
				{
					if (this.sizeInBits == 1U)
					{
						return value >= 0f;
					}
					int intMinValue = (int)(-(int)((int)1L << (int)(this.sizeInBits - 1U)));
					int intMaxValue = (int)((1L << (int)(this.sizeInBits - 1U)) - 1L);
					return NumberHelpers.NormalizedFloatToInt(value, intMinValue, intMaxValue);
				}
			}
			else
			{
				if (num == 1397248596)
				{
					return (short)NumberHelpers.NormalizedFloatToInt(value * 0.5f + 0.5f, -32768, 32767);
				}
				if (num == 1430867540)
				{
					return NumberHelpers.NormalizedFloatToUInt(value, 0U, uint.MaxValue);
				}
				if (num == 1431521364)
				{
					return (ushort)NumberHelpers.NormalizedFloatToUInt(value, 0U, 65535U);
				}
			}
			throw new Exception(string.Format("State format '{0}' is not supported as floating-point format", this.format));
		}

		public unsafe double ReadDouble(void* statePtr)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1229870112)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num == 1113150533)
						{
							return (double)NumberHelpers.UIntToNormalizedFloat((uint)(*ptr), 0U, 255U);
						}
					}
					else
					{
						if (this.sizeInBits == 1U)
						{
							return (double)(MemoryHelpers.ReadSingleBit((void*)ptr, this.bitOffset) ? 1f : 0f);
						}
						return (double)MemoryHelpers.ReadMultipleBitsAsNormalizedUInt((void*)ptr, this.bitOffset, this.sizeInBits);
					}
				}
				else
				{
					if (num == 1145195552)
					{
						return *(double*)ptr;
					}
					if (num == 1179407392)
					{
						return (double)(*(float*)ptr);
					}
					if (num == 1229870112)
					{
						return (double)(NumberHelpers.IntToNormalizedFloat(*(int*)ptr, int.MinValue, int.MaxValue) * 2f - 1f);
					}
				}
			}
			else if (num <= 1396857172)
			{
				if (num != 1396853076)
				{
					if (num == 1396857172)
					{
						return (double)(NumberHelpers.IntToNormalizedFloat((int)(*(sbyte*)ptr), -128, 127) * 2f - 1f);
					}
				}
				else
				{
					if (this.sizeInBits == 1U)
					{
						return (double)(MemoryHelpers.ReadSingleBit((void*)ptr, this.bitOffset) ? 1f : -1f);
					}
					return (double)(MemoryHelpers.ReadMultipleBitsAsNormalizedUInt((void*)ptr, this.bitOffset, this.sizeInBits) * 2f - 1f);
				}
			}
			else
			{
				if (num == 1397248596)
				{
					return (double)(NumberHelpers.IntToNormalizedFloat((int)(*(short*)ptr), -32768, 32767) * 2f - 1f);
				}
				if (num == 1430867540)
				{
					return (double)NumberHelpers.UIntToNormalizedFloat(*(uint*)ptr, 0U, uint.MaxValue);
				}
				if (num == 1431521364)
				{
					return (double)NumberHelpers.UIntToNormalizedFloat((uint)(*(ushort*)ptr), 0U, 65535U);
				}
			}
			throw new Exception(string.Format("State format '{0}' is not supported as floating-point format", this.format));
		}

		public unsafe void WriteDouble(void* statePtr, double value)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1229870112)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num == 1113150533)
						{
							*ptr = (byte)NumberHelpers.NormalizedFloatToUInt((float)value, 0U, 255U);
							return;
						}
					}
					else
					{
						if (this.sizeInBits == 1U)
						{
							MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value >= 0.5);
							return;
						}
						MemoryHelpers.WriteNormalizedUIntAsMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, (float)value);
						return;
					}
				}
				else
				{
					if (num == 1145195552)
					{
						*(double*)ptr = value;
						return;
					}
					if (num == 1179407392)
					{
						*(float*)ptr = (float)value;
						return;
					}
					if (num == 1229870112)
					{
						*(int*)ptr = NumberHelpers.NormalizedFloatToInt((float)value * 0.5f + 0.5f, int.MinValue, int.MaxValue);
						return;
					}
				}
			}
			else if (num <= 1396857172)
			{
				if (num != 1396853076)
				{
					if (num == 1396857172)
					{
						*ptr = (byte)((sbyte)NumberHelpers.NormalizedFloatToInt((float)value * 0.5f + 0.5f, -128, 127));
						return;
					}
				}
				else
				{
					if (this.sizeInBits == 1U)
					{
						MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value >= 0.0);
						return;
					}
					MemoryHelpers.WriteNormalizedUIntAsMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, (float)value * 0.5f + 0.5f);
					return;
				}
			}
			else
			{
				if (num == 1397248596)
				{
					*(short*)ptr = (short)NumberHelpers.NormalizedFloatToInt((float)value * 0.5f + 0.5f, -32768, 32767);
					return;
				}
				if (num == 1430867540)
				{
					*(int*)ptr = (int)NumberHelpers.NormalizedFloatToUInt((float)value, 0U, uint.MaxValue);
					return;
				}
				if (num == 1431521364)
				{
					*(short*)ptr = (short)((ushort)NumberHelpers.NormalizedFloatToUInt((float)value, 0U, 65535U));
					return;
				}
			}
			throw new InvalidOperationException(string.Format("State format '{0}' is not supported as floating-point format", this.format));
		}

		public unsafe void Write(void* statePtr, PrimitiveValue value)
		{
			byte* ptr = (byte*)statePtr + this.byteOffset;
			int num = this.format;
			if (num <= 1229870112)
			{
				if (num <= 1113150533)
				{
					if (num != 1112101920)
					{
						if (num == 1113150533)
						{
							*ptr = value.ToByte(null);
							return;
						}
					}
					else
					{
						if (this.sizeInBits == 1U)
						{
							MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value.ToBoolean(null));
							return;
						}
						MemoryHelpers.WriteUIntAsMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, value.ToUInt32(null));
						return;
					}
				}
				else
				{
					if (num == 1179407392)
					{
						*(float*)ptr = value.ToSingle(null);
						return;
					}
					if (num == 1229870112)
					{
						*(int*)ptr = value.ToInt32(null);
						return;
					}
				}
			}
			else if (num <= 1396857172)
			{
				if (num != 1396853076)
				{
					if (num == 1396857172)
					{
						*ptr = (byte)value.ToSByte(null);
						return;
					}
				}
				else
				{
					if (this.sizeInBits == 1U)
					{
						MemoryHelpers.WriteSingleBit((void*)ptr, this.bitOffset, value.ToBoolean(null));
						return;
					}
					MemoryHelpers.WriteIntAsExcessKMultipleBits((void*)ptr, this.bitOffset, this.sizeInBits, value.ToInt32(null));
					return;
				}
			}
			else
			{
				if (num == 1397248596)
				{
					*(short*)ptr = value.ToInt16(null);
					return;
				}
				if (num == 1430867540)
				{
					*(int*)ptr = (int)value.ToUInt32(null);
					return;
				}
				if (num == 1431521364)
				{
					*(short*)ptr = (short)value.ToUInt16(null);
					return;
				}
			}
			throw new NotImplementedException(string.Format("Writing primitive value of type '{0}' into state block with format '{1}'", value.type, this.format));
		}

		public unsafe void CopyToFrom(void* toStatePtr, void* fromStatePtr)
		{
			if (this.bitOffset != 0U || this.sizeInBits % 8U != 0U)
			{
				throw new NotImplementedException("Copying bitfields");
			}
			byte* source = (byte*)fromStatePtr + this.byteOffset;
			UnsafeUtility.MemCpy((void*)((byte*)toStatePtr + this.byteOffset), (void*)source, (long)((ulong)this.alignedSizeInBytes));
		}

		public const uint InvalidOffset = 4294967295U;

		public const uint AutomaticOffset = 4294967294U;

		public static readonly FourCC FormatInvalid = new FourCC(0);

		internal const int kFormatInvalid = 0;

		public static readonly FourCC FormatBit = new FourCC('B', 'I', 'T', ' ');

		internal const int kFormatBit = 1112101920;

		public static readonly FourCC FormatSBit = new FourCC('S', 'B', 'I', 'T');

		internal const int kFormatSBit = 1396853076;

		public static readonly FourCC FormatInt = new FourCC('I', 'N', 'T', ' ');

		internal const int kFormatInt = 1229870112;

		public static readonly FourCC FormatUInt = new FourCC('U', 'I', 'N', 'T');

		internal const int kFormatUInt = 1430867540;

		public static readonly FourCC FormatShort = new FourCC('S', 'H', 'R', 'T');

		internal const int kFormatShort = 1397248596;

		public static readonly FourCC FormatUShort = new FourCC('U', 'S', 'H', 'T');

		internal const int kFormatUShort = 1431521364;

		public static readonly FourCC FormatByte = new FourCC('B', 'Y', 'T', 'E');

		internal const int kFormatByte = 1113150533;

		public static readonly FourCC FormatSByte = new FourCC('S', 'B', 'Y', 'T');

		internal const int kFormatSByte = 1396857172;

		public static readonly FourCC FormatLong = new FourCC('L', 'N', 'G', ' ');

		internal const int kFormatLong = 1280198432;

		public static readonly FourCC FormatULong = new FourCC('U', 'L', 'N', 'G');

		internal const int kFormatULong = 1431064135;

		public static readonly FourCC FormatFloat = new FourCC('F', 'L', 'T', ' ');

		internal const int kFormatFloat = 1179407392;

		public static readonly FourCC FormatDouble = new FourCC('D', 'B', 'L', ' ');

		internal const int kFormatDouble = 1145195552;

		public static readonly FourCC FormatVector2 = new FourCC('V', 'E', 'C', '2');

		internal const int kFormatVector2 = 1447379762;

		public static readonly FourCC FormatVector3 = new FourCC('V', 'E', 'C', '3');

		internal const int kFormatVector3 = 1447379763;

		public static readonly FourCC FormatQuaternion = new FourCC('Q', 'U', 'A', 'T');

		internal const int kFormatQuaternion = 1364541780;

		public static readonly FourCC FormatVector2Short = new FourCC('V', 'C', '2', 'S');

		public static readonly FourCC FormatVector3Short = new FourCC('V', 'C', '3', 'S');

		public static readonly FourCC FormatVector2Byte = new FourCC('V', 'C', '2', 'B');

		public static readonly FourCC FormatVector3Byte = new FourCC('V', 'C', '3', 'B');

		public static readonly FourCC FormatPose = new FourCC('P', 'o', 's', 'e');

		internal const int kFormatPose = 1349481317;

		internal uint m_ByteOffset;
	}
}
