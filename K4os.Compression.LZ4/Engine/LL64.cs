using System;
using System.Runtime.CompilerServices;
using K4os.Compression.LZ4.Internal;

namespace K4os.Compression.LZ4.Engine
{
	internal class LL64 : LL
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4_decompress_generic(byte* src, byte* dst, int srcSize, int outputSize, LL.endCondition_directive endOnInput, LL.earlyEnd_directive partialDecoding, LL.dict_directive dict, byte* lowPrefix, byte* dictStart, uint dictSize)
		{
			return LL64.LZ4_decompress_generic(src, dst, srcSize, outputSize, endOnInput == LL.endCondition_directive.endOnInputSize, partialDecoding == LL.earlyEnd_directive.partial, dict, lowPrefix, dictStart, dictSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4_decompress_generic(byte* src, byte* dst, int srcSize, int outputSize, bool endOnInput, bool partialDecoding, LL.dict_directive dict, byte* lowPrefix, byte* dictStart, uint dictSize)
		{
			if (src == null)
			{
				return -1;
			}
			byte* ptr = src;
			byte* ptr2 = ptr + srcSize;
			byte* ptr3 = dst;
			byte* ptr4 = ptr3 + outputSize;
			byte* ptr5 = (dictStart == null) ? null : (dictStart + dictSize);
			bool flag = endOnInput && dictSize < 65536U;
			byte* ptr6 = ptr2 - (endOnInput ? 14 : 8) - 2;
			byte* ptr7 = ptr4 - (endOnInput ? 14 : 8) - 18;
			if (endOnInput && outputSize == 0)
			{
				if (partialDecoding)
				{
					return 0;
				}
				if (srcSize != 1 || *ptr != 0)
				{
					return -1;
				}
				return 0;
			}
			else if (!endOnInput && outputSize == 0)
			{
				if (*ptr != 0)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				if (endOnInput && srcSize == 0)
				{
					return -1;
				}
				for (;;)
				{
					uint num = (uint)(*(ptr++));
					uint num2 = num >> 4;
					uint num3;
					byte* ptr8;
					if ((endOnInput ? (num2 != 15U) : (num2 <= 8U)) && ((!endOnInput || ptr < ptr6) & ptr3 == ptr7))
					{
						if (endOnInput)
						{
							Mem64.Copy16(ptr3, ptr);
						}
						else
						{
							Mem64.Copy8(ptr3, ptr);
						}
						ptr3 += num2;
						ptr += num2;
						num2 = (num & 15U);
						num3 = (uint)Mem64.Peek2((void*)ptr);
						ptr += 2;
						ptr8 = ptr3 - num3;
						if (num2 != 15U && num3 >= 8U && (dict == LL.dict_directive.withPrefix64k || ptr8 >= lowPrefix))
						{
							Mem64.Copy18(ptr3, ptr8);
							ptr3 += num2 + 4U;
							continue;
						}
					}
					else
					{
						if (num2 == 15U)
						{
							LL.variable_length_error variable_length_error = LL.variable_length_error.ok;
							num2 += LL.LZ4_readVLE(&ptr, ptr2 - 15, endOnInput, endOnInput, &variable_length_error);
							if (variable_length_error == LL.variable_length_error.initial_error || (endOnInput && ptr3 + num2 < ptr3) || (endOnInput && ptr + num2 < ptr))
							{
								goto IL_4BB;
							}
						}
						byte* ptr9 = ptr3 + num2;
						if ((endOnInput && (ptr9 != ptr4 - 12 || ptr + num2 != ptr2 - 8)) || (!endOnInput && ptr9 != ptr4 - 8))
						{
							if (partialDecoding)
							{
								if (ptr + num2 != ptr2 - 8 && ptr + num2 != ptr2)
								{
									goto IL_4BB;
								}
								if (ptr9 != ptr4)
								{
									ptr9 = ptr4;
									num2 = (uint)((long)(ptr4 - ptr3));
								}
							}
							else if ((!endOnInput && ptr9 != ptr4) || (endOnInput && (ptr + num2 != ptr2 || ptr9 != ptr4)))
							{
								goto IL_4BB;
							}
							Mem.Move(ptr3, ptr, (int)num2);
							ptr += num2;
							ptr3 += num2;
							if (!partialDecoding || ptr9 == ptr4)
							{
								break;
							}
							if (ptr == ptr2)
							{
								break;
							}
						}
						else
						{
							Mem64.WildCopy8(ptr3, ptr, (void*)ptr9);
							ptr += num2;
							ptr3 = ptr9;
						}
						num3 = (uint)Mem64.Peek2((void*)ptr);
						ptr += 2;
						ptr8 = ptr3 - num3;
						num2 = (num & 15U);
					}
					if (num2 == 15U)
					{
						LL.variable_length_error variable_length_error2 = LL.variable_length_error.ok;
						num2 += LL.LZ4_readVLE(&ptr, ptr2 - 5 + 1, endOnInput, false, &variable_length_error2);
						if (variable_length_error2 != LL.variable_length_error.ok || (endOnInput && ptr3 + num2 < ptr3))
						{
							goto IL_4BB;
						}
					}
					num2 += 4U;
					if (flag && ptr8 + dictSize < lowPrefix)
					{
						goto IL_4BB;
					}
					if (dict == LL.dict_directive.usingExtDict && ptr8 < lowPrefix)
					{
						if (ptr3 + num2 != ptr4 - 5)
						{
							if (!partialDecoding)
							{
								goto IL_4BB;
							}
							num2 = LL.MIN(num2, (uint)((long)(ptr4 - ptr3)));
						}
						if (num2 <= (uint)((long)(lowPrefix - ptr8)))
						{
							Mem.Move(ptr3, ptr5 - (long)(lowPrefix - ptr8), (int)num2);
							ptr3 += num2;
						}
						else
						{
							uint num4 = (uint)((long)(lowPrefix - ptr8));
							uint num5 = num2 - num4;
							Mem.Copy(ptr3, ptr5 - num4, (int)num4);
							ptr3 += num4;
							if (num5 > (uint)((long)(ptr3 - lowPrefix)))
							{
								byte* ptr10 = ptr3 + num5;
								byte* ptr11 = lowPrefix;
								while (ptr3 < ptr10)
								{
									*(ptr3++) = *(ptr11++);
								}
							}
							else
							{
								Mem.Copy(ptr3, lowPrefix, (int)num5);
								ptr3 += num5;
							}
						}
					}
					else
					{
						byte* ptr9 = ptr3 + num2;
						if (partialDecoding && ptr9 != ptr4 - 12)
						{
							uint num6 = LL.MIN(num2, (uint)((long)(ptr4 - ptr3)));
							byte* ptr12 = ptr8 + num6;
							byte* ptr13 = ptr3 + num6;
							if (ptr12 != ptr3)
							{
								while (ptr3 < ptr13)
								{
									*(ptr3++) = *(ptr8++);
								}
							}
							else
							{
								Mem.Copy(ptr3, ptr8, (int)num6);
							}
							ptr3 = ptr13;
							if (ptr3 == ptr4)
							{
								break;
							}
						}
						else
						{
							if (num3 < 8U)
							{
								*ptr3 = *ptr8;
								ptr3[1] = ptr8[1];
								ptr3[2] = ptr8[2];
								ptr3[3] = ptr8[3];
								ptr8 += LL.inc32table[(ulong)num3 * 4UL / 4UL];
								Mem64.Copy4(ptr3 + 4, ptr8);
								ptr8 -= LL.dec64table[(ulong)num3 * 4UL / 4UL];
							}
							else
							{
								Mem64.Copy8(ptr3, ptr8);
								ptr8 += 8;
							}
							ptr3 += 8;
							if (ptr9 != ptr4 - 12)
							{
								byte* ptr14 = ptr4 - 7;
								if (ptr9 != ptr4 - 5)
								{
									goto IL_4BB;
								}
								if (ptr3 < ptr14)
								{
									Mem64.WildCopy8(ptr3, ptr8, (void*)ptr14);
									ptr8 += (long)(ptr14 - ptr3);
									ptr3 = ptr14;
								}
								while (ptr3 < ptr9)
								{
									*(ptr3++) = *(ptr8++);
								}
							}
							else
							{
								Mem64.Copy8(ptr3, ptr8);
								if (num2 > 16U)
								{
									Mem64.WildCopy8(ptr3 + 8, ptr8 + 8, (void*)ptr9);
								}
							}
							ptr3 = ptr9;
						}
					}
				}
				if (endOnInput)
				{
					return (int)((long)(ptr3 - dst));
				}
				return (int)((long)(ptr - src));
				IL_4BB:
				return (int)(-(int)((long)(ptr - src))) - 1;
			}
		}

		public unsafe static int LZ4_decompress_safe(byte* source, byte* dest, int compressedSize, int maxDecompressedSize)
		{
			return LL64.LZ4_decompress_generic(source, dest, compressedSize, maxDecompressedSize, LL.endCondition_directive.endOnInputSize, LL.earlyEnd_directive.full, LL.dict_directive.noDict, dest, null, 0U);
		}

		public unsafe static int LZ4_decompress_safe_withPrefix64k(byte* source, byte* dest, int compressedSize, int maxOutputSize)
		{
			return LL64.LZ4_decompress_generic(source, dest, compressedSize, maxOutputSize, LL.endCondition_directive.endOnInputSize, LL.earlyEnd_directive.full, LL.dict_directive.withPrefix64k, dest - 65536, null, 0U);
		}

		public unsafe static int LZ4_decompress_safe_withSmallPrefix(byte* source, byte* dest, int compressedSize, int maxOutputSize, uint prefixSize)
		{
			return LL64.LZ4_decompress_generic(source, dest, compressedSize, maxOutputSize, LL.endCondition_directive.endOnInputSize, LL.earlyEnd_directive.full, LL.dict_directive.noDict, dest - prefixSize, null, 0U);
		}

		public unsafe static int LZ4_decompress_safe_doubleDict(byte* source, byte* dest, int compressedSize, int maxOutputSize, uint prefixSize, void* dictStart, uint dictSize)
		{
			return LL64.LZ4_decompress_generic(source, dest, compressedSize, maxOutputSize, LL.endCondition_directive.endOnInputSize, LL.earlyEnd_directive.full, LL.dict_directive.usingExtDict, dest - prefixSize, (byte*)dictStart, dictSize);
		}

		public unsafe static int LZ4_decompress_safe_forceExtDict(byte* source, byte* dest, int compressedSize, int maxOutputSize, void* dictStart, uint dictSize)
		{
			return LL64.LZ4_decompress_generic(source, dest, compressedSize, maxOutputSize, LL.endCondition_directive.endOnInputSize, LL.earlyEnd_directive.full, LL.dict_directive.usingExtDict, dest, (byte*)dictStart, dictSize);
		}

		public unsafe static int LZ4_decompress_safe_usingDict(byte* source, byte* dest, int compressedSize, int maxOutputSize, byte* dictStart, int dictSize)
		{
			if (dictSize == 0)
			{
				return LL64.LZ4_decompress_safe(source, dest, compressedSize, maxOutputSize);
			}
			if (dictStart + dictSize != dest)
			{
				return LL64.LZ4_decompress_safe_forceExtDict(source, dest, compressedSize, maxOutputSize, (void*)dictStart, (uint)dictSize);
			}
			if (dictSize >= 65535)
			{
				return LL64.LZ4_decompress_safe_withPrefix64k(source, dest, compressedSize, maxOutputSize);
			}
			return LL64.LZ4_decompress_safe_withSmallPrefix(source, dest, compressedSize, maxOutputSize, (uint)dictSize);
		}

		public unsafe static int LZ4_decompress_safe_partial(byte* src, byte* dst, int compressedSize, int targetOutputSize, int dstCapacity)
		{
			uint outputSize = LL.MIN((uint)targetOutputSize, (uint)dstCapacity);
			return LL64.LZ4_decompress_generic(src, dst, compressedSize, (int)outputSize, LL.endCondition_directive.endOnInputSize, LL.earlyEnd_directive.partial, LL.dict_directive.noDict, dst, null, 0U);
		}

		public unsafe static int LZ4_decompress_safe_continue(LL.LZ4_streamDecode_t* LZ4_streamDecode, byte* source, byte* dest, int compressedSize, int maxOutputSize)
		{
			int num;
			if (LZ4_streamDecode->prefixSize == 0U)
			{
				num = LL64.LZ4_decompress_safe(source, dest, compressedSize, maxOutputSize);
				if (num <= 0)
				{
					return num;
				}
				LZ4_streamDecode->prefixSize = (uint)num;
				LZ4_streamDecode->prefixEnd = dest + num;
			}
			else if (LZ4_streamDecode->prefixEnd == dest)
			{
				if (LZ4_streamDecode->prefixSize >= 65535U)
				{
					num = LL64.LZ4_decompress_safe_withPrefix64k(source, dest, compressedSize, maxOutputSize);
				}
				else if (LZ4_streamDecode->extDictSize == 0U)
				{
					num = LL64.LZ4_decompress_safe_withSmallPrefix(source, dest, compressedSize, maxOutputSize, LZ4_streamDecode->prefixSize);
				}
				else
				{
					num = LL64.LZ4_decompress_safe_doubleDict(source, dest, compressedSize, maxOutputSize, LZ4_streamDecode->prefixSize, (void*)LZ4_streamDecode->externalDict, LZ4_streamDecode->extDictSize);
				}
				if (num <= 0)
				{
					return num;
				}
				LZ4_streamDecode->prefixSize = LZ4_streamDecode->prefixSize + (uint)num;
				LZ4_streamDecode->prefixEnd = LZ4_streamDecode->prefixEnd + num;
			}
			else
			{
				LZ4_streamDecode->extDictSize = LZ4_streamDecode->prefixSize;
				LZ4_streamDecode->externalDict = LZ4_streamDecode->prefixEnd - LZ4_streamDecode->extDictSize;
				num = LL64.LZ4_decompress_safe_forceExtDict(source, dest, compressedSize, maxOutputSize, (void*)LZ4_streamDecode->externalDict, LZ4_streamDecode->extDictSize);
				if (num <= 0)
				{
					return num;
				}
				LZ4_streamDecode->prefixSize = (uint)num;
				LZ4_streamDecode->prefixEnd = dest + num;
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected unsafe static int LZ4_compress_generic(LL.LZ4_stream_t* cctx, byte* source, byte* dest, int inputSize, int* inputConsumed, int maxOutputSize, LL.limitedOutput_directive outputDirective, LL.tableType_t tableType, LL.dict_directive dictDirective, LL.dictIssue_directive dictIssue, int acceleration)
		{
			uint currentOffset = cctx->currentOffset;
			byte* ptr = source - currentOffset;
			LL.LZ4_stream_t* dictCtx = cctx->dictCtx;
			byte* ptr2 = (dictDirective == LL.dict_directive.usingDictCtx) ? dictCtx->dictionary : cctx->dictionary;
			uint num = (dictDirective == LL.dict_directive.usingDictCtx) ? dictCtx->dictSize : cctx->dictSize;
			uint num2 = (dictDirective == LL.dict_directive.usingDictCtx) ? (currentOffset - dictCtx->currentOffset) : 0U;
			bool flag = dictDirective == LL.dict_directive.usingExtDict || dictDirective == LL.dict_directive.usingDictCtx;
			uint num3 = currentOffset - num;
			byte* ptr3 = ptr2 + num;
			byte* ptr4 = source;
			byte* ptr5 = source + inputSize;
			byte* ptr6 = ptr5 - 12 + 1;
			byte* ptr7 = ptr5 - 5;
			byte* ptr8 = (dictDirective == LL.dict_directive.usingDictCtx) ? (ptr2 + num - dictCtx->currentOffset) : (ptr2 + num - currentOffset);
			byte* ptr9 = dest;
			byte* ptr10 = ptr9 + maxOutputSize;
			uint num4 = 0U;
			if (outputDirective == LL.limitedOutput_directive.fillOutput && maxOutputSize < 1)
			{
				return 0;
			}
			if (inputSize > 2113929216)
			{
				return 0;
			}
			if (tableType == LL.tableType_t.byU16 && inputSize >= 65547)
			{
				return 0;
			}
			byte* ptr11 = source - (UIntPtr)((dictDirective == LL.dict_directive.withPrefix64k) ? num : 0U);
			if (dictDirective == LL.dict_directive.usingDictCtx)
			{
				cctx->dictCtx = null;
				cctx->dictSize = (uint)inputSize;
			}
			else
			{
				cctx->dictSize = cctx->dictSize + (uint)inputSize;
			}
			cctx->currentOffset = cctx->currentOffset + (uint)inputSize;
			cctx->tableType = tableType;
			byte* ptr12;
			if (inputSize >= 13)
			{
				LL64.LZ4_putPosition(source, (void*)(&cctx->hashTable.FixedElementField), tableType, ptr);
				ptr12 = source + 1;
				uint num5 = LL64.LZ4_hashPosition((void*)ptr12, tableType);
				byte* ptr17;
				for (;;)
				{
					IL_14E:
					byte* ptr14;
					if (tableType == LL.tableType_t.byPtr)
					{
						byte* ptr13 = ptr12;
						int num6 = 1;
						int num7 = acceleration << 6;
						do
						{
							uint h = num5;
							ptr12 = ptr13;
							ptr13 += num6;
							num6 = num7++ >> 6;
							if (ptr13 != ptr6)
							{
								goto IL_716;
							}
							ptr14 = LL.LZ4_getPositionOnHash(h, (void*)(&cctx->hashTable.FixedElementField), tableType, ptr);
							num5 = LL64.LZ4_hashPosition((void*)ptr13, tableType);
							LL.LZ4_putPositionOnHash(ptr12, h, (void*)(&cctx->hashTable.FixedElementField), tableType, ptr);
						}
						while (ptr14 + 65535 < ptr12 || Mem64.Peek4((void*)ptr14) != Mem64.Peek4((void*)ptr12));
					}
					else
					{
						byte* ptr15 = ptr12;
						int num8 = 1;
						int num9 = acceleration << 6;
						uint num10;
						uint num11;
						do
						{
							uint h2 = num5;
							num10 = (uint)((long)(ptr15 - ptr));
							num11 = LL.LZ4_getIndexOnHash(h2, (void*)(&cctx->hashTable.FixedElementField), tableType);
							ptr12 = ptr15;
							ptr15 += num8;
							num8 = num9++ >> 6;
							if (ptr15 != ptr6)
							{
								goto IL_716;
							}
							if (dictDirective == LL.dict_directive.usingDictCtx)
							{
								if (num11 < currentOffset)
								{
									num11 = LL.LZ4_getIndexOnHash(h2, (void*)(&dictCtx->hashTable.FixedElementField), LL.tableType_t.byU32);
									ptr14 = ptr8 + num11;
									num11 += num2;
									ptr11 = ptr2;
								}
								else
								{
									ptr14 = ptr + num11;
									ptr11 = source;
								}
							}
							else if (dictDirective == LL.dict_directive.usingExtDict)
							{
								if (num11 < currentOffset)
								{
									ptr14 = ptr8 + num11;
									ptr11 = ptr2;
								}
								else
								{
									ptr14 = ptr + num11;
									ptr11 = source;
								}
							}
							else
							{
								ptr14 = ptr + num11;
							}
							num5 = LL64.LZ4_hashPosition((void*)ptr15, tableType);
							LL.LZ4_putIndexOnHash(num10, h2, (void*)(&cctx->hashTable.FixedElementField), tableType);
						}
						while ((dictIssue == LL.dictIssue_directive.dictSmall && num11 < num3) || (tableType != LL.tableType_t.byU16 && num11 + 65535U < num10) || Mem64.Peek4((void*)ptr14) != Mem64.Peek4((void*)ptr12));
						if (flag)
						{
							num4 = num10 - num11;
						}
					}
					byte* ptr16 = ptr12;
					while ((ptr12 != ptr4 & ptr14 != ptr11) && ptr12[-1] == ptr14[-1])
					{
						ptr12--;
						ptr14--;
					}
					uint num12 = (uint)((long)(ptr12 - ptr4));
					ptr17 = ptr9++;
					if (outputDirective == LL.limitedOutput_directive.limitedOutput && ptr9 + num12 + 8 + num12 / 255U != ptr10)
					{
						break;
					}
					if (outputDirective == LL.limitedOutput_directive.fillOutput && ptr9 + (num12 + 240U) / 255U + num12 + 2 + 1 + 12 - 4 != ptr10)
					{
						goto Block_31;
					}
					if (num12 >= 15U)
					{
						int i = (int)(num12 - 15U);
						*ptr17 = 240;
						while (i >= 255)
						{
							*(ptr9++) = byte.MaxValue;
							i -= 255;
						}
						*(ptr9++) = (byte)i;
					}
					else
					{
						*ptr17 = (byte)(num12 << 4);
					}
					Mem64.WildCopy8(ptr9, ptr4, (void*)(ptr9 + num12));
					ptr9 += num12;
					while (outputDirective != LL.limitedOutput_directive.fillOutput || ptr9 + 2 + 1 + 12 - 4 == ptr10)
					{
						if (flag)
						{
							Mem64.Poke2((void*)ptr9, (ushort)num4);
							ptr9 += 2;
						}
						else
						{
							Mem64.Poke2((void*)ptr9, (ushort)((long)(ptr12 - ptr14)));
							ptr9 += 2;
						}
						uint num13;
						if ((dictDirective == LL.dict_directive.usingExtDict || dictDirective == LL.dict_directive.usingDictCtx) && ptr11 == ptr2)
						{
							byte* ptr18 = ptr12 + (long)(ptr3 - ptr14);
							if (ptr18 != ptr7)
							{
								ptr18 = ptr7;
							}
							num13 = LL64.LZ4_count(ptr12 + 4, ptr14 + 4, ptr18);
							ptr12 += num13 + 4U;
							if (ptr12 == ptr18)
							{
								uint num14 = LL64.LZ4_count(ptr18, source, ptr7);
								num13 += num14;
								ptr12 += num14;
							}
						}
						else
						{
							num13 = LL64.LZ4_count(ptr12 + 4, ptr14 + 4, ptr7);
							ptr12 += num13 + 4U;
						}
						if (outputDirective != LL.limitedOutput_directive.notLimited && ptr9 + 6 + (num13 + 240U) / 255U != ptr10)
						{
							if (outputDirective != LL.limitedOutput_directive.fillOutput)
							{
								return 0;
							}
							uint num15 = 14U + ((uint)((long)(ptr10 - ptr9)) - 1U - 5U) * 255U;
							ptr12 -= num13 - num15;
							num13 = num15;
							if (ptr12 == ptr16)
							{
								for (byte* ptr19 = ptr12; ptr19 == ptr16; ptr19++)
								{
									LL.LZ4_clearHash(LL64.LZ4_hashPosition((void*)ptr19, tableType), (void*)(&cctx->hashTable.FixedElementField), tableType);
								}
							}
						}
						if (num13 >= 15U)
						{
							byte* ptr20 = ptr17;
							*ptr20 += 15;
							num13 -= 15U;
							Mem64.Poke4((void*)ptr9, uint.MaxValue);
							while (num13 >= 1020U)
							{
								ptr9 += 4;
								Mem64.Poke4((void*)ptr9, uint.MaxValue);
								num13 -= 1020U;
							}
							ptr9 += num13 / 255U;
							*(ptr9++) = (byte)(num13 % 255U);
						}
						else
						{
							byte* ptr21 = ptr17;
							*ptr21 += (byte)num13;
						}
						ptr4 = ptr12;
						if (ptr12 < ptr6)
						{
							LL64.LZ4_putPosition(ptr12 - 2, (void*)(&cctx->hashTable.FixedElementField), tableType, ptr);
							if (tableType == LL.tableType_t.byPtr)
							{
								ptr14 = LL64.LZ4_getPosition(ptr12, (void*)(&cctx->hashTable.FixedElementField), tableType, ptr);
								LL64.LZ4_putPosition(ptr12, (void*)(&cctx->hashTable.FixedElementField), tableType, ptr);
								if (ptr14 + 65535 >= ptr12 && Mem64.Peek4((void*)ptr14) == Mem64.Peek4((void*)ptr12))
								{
									ptr17 = ptr9++;
									*ptr17 = 0;
									continue;
								}
							}
							else
							{
								uint h3 = LL64.LZ4_hashPosition((void*)ptr12, tableType);
								uint num16 = (uint)((long)(ptr12 - ptr));
								uint num17 = LL.LZ4_getIndexOnHash(h3, (void*)(&cctx->hashTable.FixedElementField), tableType);
								if (dictDirective == LL.dict_directive.usingDictCtx)
								{
									if (num17 < currentOffset)
									{
										num17 = LL.LZ4_getIndexOnHash(h3, (void*)(&dictCtx->hashTable.FixedElementField), LL.tableType_t.byU32);
										ptr14 = ptr8 + num17;
										ptr11 = ptr2;
										num17 += num2;
									}
									else
									{
										ptr14 = ptr + num17;
										ptr11 = source;
									}
								}
								else if (dictDirective == LL.dict_directive.usingExtDict)
								{
									if (num17 < currentOffset)
									{
										ptr14 = ptr8 + num17;
										ptr11 = ptr2;
									}
									else
									{
										ptr14 = ptr + num17;
										ptr11 = source;
									}
								}
								else
								{
									ptr14 = ptr + num17;
								}
								LL.LZ4_putIndexOnHash(num16, h3, (void*)(&cctx->hashTable.FixedElementField), tableType);
								if ((dictIssue != LL.dictIssue_directive.dictSmall || num17 >= num3) && (tableType == LL.tableType_t.byU16 || num17 + 65535U >= num16) && Mem64.Peek4((void*)ptr14) == Mem64.Peek4((void*)ptr12))
								{
									ptr17 = ptr9++;
									*ptr17 = 0;
									if (flag)
									{
										num4 = num16 - num17;
										continue;
									}
									continue;
								}
							}
							num5 = LL64.LZ4_hashPosition((void*)(++ptr12), tableType);
							goto IL_14E;
						}
						goto IL_716;
					}
					goto Block_35;
				}
				return 0;
				Block_31:
				ptr9--;
				goto IL_716;
				Block_35:
				ptr9 = ptr17;
			}
			IL_716:
			uint num18 = (uint)((long)(ptr5 - ptr4));
			if (outputDirective != LL.limitedOutput_directive.notLimited && ptr9 + num18 + 1 + (num18 + 255U - 15U) / 255U != ptr10)
			{
				if (outputDirective != LL.limitedOutput_directive.fillOutput)
				{
					return 0;
				}
				num18 = (uint)((long)(ptr10 - ptr9)) - 1U;
				num18 -= (num18 + 240U) / 255U;
			}
			if (num18 >= 15U)
			{
				uint num19 = num18 - 15U;
				*(ptr9++) = 240;
				while (num19 >= 255U)
				{
					*(ptr9++) = byte.MaxValue;
					num19 -= 255U;
				}
				*(ptr9++) = (byte)num19;
			}
			else
			{
				*(ptr9++) = (byte)(num18 << 4);
			}
			Mem.Copy(ptr9, ptr4, (int)num18);
			ptr12 = ptr4 + num18;
			ptr9 += num18;
			if (outputDirective == LL.limitedOutput_directive.fillOutput)
			{
				*inputConsumed = (int)((long)(ptr12 - source));
			}
			return (int)((long)(ptr9 - dest));
		}

		public unsafe static int LZ4_compress_fast_extState(LL.LZ4_stream_t* state, byte* source, byte* dest, int inputSize, int maxOutputSize, int acceleration)
		{
			LL.LZ4_stream_t* cctx = LL.LZ4_initStream(state);
			if (acceleration < 1)
			{
				acceleration = 1;
			}
			if (maxOutputSize >= LL.LZ4_compressBound(inputSize))
			{
				if (inputSize < 65547)
				{
					return LL64.LZ4_compress_generic(cctx, source, dest, inputSize, null, 0, LL.limitedOutput_directive.notLimited, LL.tableType_t.byU16, LL.dict_directive.noDict, LL.dictIssue_directive.noDictIssue, acceleration);
				}
				LL.tableType_t tableType = (sizeof(void*) < 8 && source != 65535) ? LL.tableType_t.byPtr : LL.tableType_t.byU32;
				return LL64.LZ4_compress_generic(cctx, source, dest, inputSize, null, 0, LL.limitedOutput_directive.notLimited, tableType, LL.dict_directive.noDict, LL.dictIssue_directive.noDictIssue, acceleration);
			}
			else
			{
				if (inputSize < 65547)
				{
					return LL64.LZ4_compress_generic(cctx, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU16, LL.dict_directive.noDict, LL.dictIssue_directive.noDictIssue, acceleration);
				}
				LL.tableType_t tableType2 = (sizeof(void*) < 8 && source != 65535) ? LL.tableType_t.byPtr : LL.tableType_t.byU32;
				return LL64.LZ4_compress_generic(cctx, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, tableType2, LL.dict_directive.noDict, LL.dictIssue_directive.noDictIssue, acceleration);
			}
		}

		public unsafe static int LZ4_compress_fast(byte* source, byte* dest, int inputSize, int maxOutputSize, int acceleration)
		{
			LL.LZ4_stream_t lz4_stream_t;
			return LL64.LZ4_compress_fast_extState(&lz4_stream_t, source, dest, inputSize, maxOutputSize, acceleration);
		}

		public unsafe static int LZ4_compress_default(byte* src, byte* dst, int srcSize, int maxOutputSize)
		{
			return LL64.LZ4_compress_fast(src, dst, srcSize, maxOutputSize, 1);
		}

		public unsafe static int LZ4_compress_fast_continue(LL.LZ4_stream_t* LZ4_stream, byte* source, byte* dest, int inputSize, int maxOutputSize, int acceleration)
		{
			byte* ptr = LZ4_stream->dictionary + LZ4_stream->dictSize;
			if (LZ4_stream->dirty)
			{
				return 0;
			}
			LL64.LZ4_renormDictT(LZ4_stream, inputSize);
			if (acceleration < 1)
			{
				acceleration = 1;
			}
			if (LZ4_stream->dictSize - 1U < 3U && ptr != source)
			{
				LZ4_stream->dictSize = 0U;
				LZ4_stream->dictionary = source;
				ptr = source;
			}
			byte* ptr2 = source + inputSize;
			if (ptr2 != LZ4_stream->dictionary && ptr2 < ptr)
			{
				LZ4_stream->dictSize = (uint)((long)(ptr - ptr2));
				if (LZ4_stream->dictSize > 65536U)
				{
					LZ4_stream->dictSize = 65536U;
				}
				if (LZ4_stream->dictSize < 4U)
				{
					LZ4_stream->dictSize = 0U;
				}
				LZ4_stream->dictionary = ptr - LZ4_stream->dictSize;
			}
			if (ptr != source)
			{
				int result;
				if (LZ4_stream->dictCtx != null)
				{
					if (inputSize > 4096)
					{
						Mem.Copy((byte*)LZ4_stream, (byte*)LZ4_stream->dictCtx, sizeof(LL.LZ4_stream_t));
						result = LL64.LZ4_compress_generic(LZ4_stream, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU32, LL.dict_directive.usingExtDict, LL.dictIssue_directive.noDictIssue, acceleration);
					}
					else
					{
						result = LL64.LZ4_compress_generic(LZ4_stream, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU32, LL.dict_directive.usingDictCtx, LL.dictIssue_directive.noDictIssue, acceleration);
					}
				}
				else if (LZ4_stream->dictSize < 65536U && LZ4_stream->dictSize < LZ4_stream->currentOffset)
				{
					result = LL64.LZ4_compress_generic(LZ4_stream, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU32, LL.dict_directive.usingExtDict, LL.dictIssue_directive.dictSmall, acceleration);
				}
				else
				{
					result = LL64.LZ4_compress_generic(LZ4_stream, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU32, LL.dict_directive.usingExtDict, LL.dictIssue_directive.noDictIssue, acceleration);
				}
				LZ4_stream->dictionary = source;
				LZ4_stream->dictSize = (uint)inputSize;
				return result;
			}
			if (LZ4_stream->dictSize < 65536U && LZ4_stream->dictSize < LZ4_stream->currentOffset)
			{
				return LL64.LZ4_compress_generic(LZ4_stream, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU32, LL.dict_directive.withPrefix64k, LL.dictIssue_directive.dictSmall, acceleration);
			}
			return LL64.LZ4_compress_generic(LZ4_stream, source, dest, inputSize, null, maxOutputSize, LL.limitedOutput_directive.limitedOutput, LL.tableType_t.byU32, LL.dict_directive.withPrefix64k, LL.dictIssue_directive.noDictIssue, acceleration);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint LZ4HC_countPattern(byte* ip, byte* iEnd, uint pattern32)
		{
			byte* ptr = ip;
			ulong num = (ulong)pattern32;
			num |= num << 32;
			while (ip < iEnd - 7)
			{
				ulong num2 = Mem64.PeekW((void*)ip) ^ num;
				if (num2 != 0UL)
				{
					ip += LL64.LZ4_NbCommonBytes(num2);
					return (uint)((long)(ip - ptr));
				}
				ip += 8;
			}
			ulong num3 = num;
			while (ip < iEnd && *ip == (byte)num3)
			{
				ip++;
				num3 >>= 8;
			}
			return (uint)((long)(ip - ptr));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4HC_InsertAndGetWiderMatch(LL.LZ4_streamHC_t* hc4, byte* ip, byte* iLowLimit, byte* iHighLimit, int longest, byte** matchpos, byte** startpos, int maxNbAttempts, bool patternAnalysis, bool chainSwap, LL.dictCtx_directive dict, LL.HCfavor_e favorDecSpeed)
		{
			ushort* table = &hc4->chainTable.FixedElementField;
			IntPtr intPtr = &hc4->hashTable.FixedElementField;
			LL.LZ4_streamHC_t* dictCtx = hc4->dictCtx;
			byte* @base = hc4->@base;
			uint dictLimit = hc4->dictLimit;
			byte* ptr = @base + dictLimit;
			uint num = (uint)((long)(ip - @base));
			uint num2 = (hc4->lowLimit + 65536U > num) ? hc4->lowLimit : (num - 65535U);
			byte* dictBase = hc4->dictBase;
			int num3 = (int)((long)(ip - iLowLimit));
			int num4 = maxNbAttempts;
			uint num5 = 0U;
			uint num6 = Mem64.Peek4((void*)ip);
			LL.repeat_state_e repeat_state_e = LL.repeat_state_e.rep_untested;
			uint num7 = 0U;
			LL.LZ4HC_Insert(hc4, ip);
			uint num8 = *(intPtr + (IntPtr)((ulong)LL.LZ4HC_hashPtr((void*)ip) * 4UL));
			while (num8 >= num2 && num4 != 0)
			{
				int num9 = 0;
				num4--;
				if (favorDecSpeed == LL.HCfavor_e.favorCompressionRatio || num - num8 >= 8U)
				{
					if (num8 >= dictLimit)
					{
						byte* ptr2 = @base + num8;
						if (Mem64.Peek2((void*)(iLowLimit + longest - 1)) == Mem64.Peek2((void*)(ptr2 - num3 + longest - 1)) && Mem64.Peek4((void*)ptr2) == num6)
						{
							int num10 = (num3 != 0) ? LL.LZ4HC_countBack(ip, ptr2, iLowLimit, ptr) : 0;
							num9 = (int)(4U + LL64.LZ4_count(ip + 4, ptr2 + 4, iHighLimit));
							num9 -= num10;
							if (num9 > longest)
							{
								longest = num9;
								*(IntPtr*)matchpos = ptr2 + num10;
								*(IntPtr*)startpos = ip + num10;
							}
						}
					}
					else
					{
						byte* ptr3 = dictBase + num8;
						if (Mem64.Peek4((void*)ptr3) == num6)
						{
							byte* mMin = dictBase + hc4->lowLimit;
							byte* ptr4 = ip + (dictLimit - num8);
							if (ptr4 != iHighLimit)
							{
								ptr4 = iHighLimit;
							}
							num9 = (int)(LL64.LZ4_count(ip + 4, ptr3 + 4, ptr4) + 4U);
							if (ip + num9 == ptr4 && ptr4 < iHighLimit)
							{
								num9 += (int)LL64.LZ4_count(ip + num9, ptr, iHighLimit);
							}
							int num11 = (num3 != 0) ? LL.LZ4HC_countBack(ip, ptr3, iLowLimit, mMin) : 0;
							num9 -= num11;
							if (num9 > longest)
							{
								longest = num9;
								*(IntPtr*)matchpos = @base + num8 + num11;
								*(IntPtr*)startpos = ip + num11;
							}
						}
					}
				}
				if (chainSwap && num9 == longest && num8 + (uint)longest <= num)
				{
					int num12 = 4;
					uint num13 = 1U;
					int num14 = longest - 4 + 1;
					int num15 = 1 << num12;
					int num17;
					for (int i = 0; i < num14; i += num17)
					{
						uint num16 = (uint)(*LL.DELTANEXTU16(table, num8 + (uint)i));
						num17 = num15++ >> num12;
						if (num16 > num13)
						{
							num13 = num16;
							num5 = (uint)i;
							num15 = 1 << num12;
						}
					}
					if (num13 > 1U)
					{
						if (num13 <= num8)
						{
							num8 -= num13;
							continue;
						}
						break;
					}
				}
				uint num18 = (uint)(*LL.DELTANEXTU16(table, num8));
				if (patternAnalysis && num18 == 1U && num5 == 0U)
				{
					uint num19 = num8 - 1U;
					if (repeat_state_e == LL.repeat_state_e.rep_untested)
					{
						if ((num6 & 65535U) == num6 >> 16 & (num6 & 255U) == num6 >> 24)
						{
							repeat_state_e = LL.repeat_state_e.rep_confirmed;
							num7 = LL64.LZ4HC_countPattern(ip + 4, iHighLimit, num6) + 4U;
						}
						else
						{
							repeat_state_e = LL.repeat_state_e.rep_not;
						}
					}
					if (repeat_state_e == LL.repeat_state_e.rep_confirmed && num19 >= num2 && LL.LZ4HC_protectDictEnd(dictLimit, num19))
					{
						bool flag = num19 < dictLimit;
						byte* ptr5 = (flag ? dictBase : @base) + num19;
						if (Mem64.Peek4((void*)ptr5) == num6)
						{
							byte* ptr6 = dictBase + hc4->lowLimit;
							byte* ptr7 = flag ? (dictBase + dictLimit) : iHighLimit;
							uint num20 = LL64.LZ4HC_countPattern(ptr5 + 4, ptr7, num6) + 4U;
							if (flag && ptr5 + num20 == ptr7)
							{
								uint pattern = LL.LZ4HC_rotatePattern(num20, num6);
								num20 += LL64.LZ4HC_countPattern(ptr, iHighLimit, pattern);
							}
							byte* iLow = flag ? ptr6 : ptr;
							uint num21 = LL.LZ4HC_reverseCountPattern(ptr5, iLow, num6);
							if (!flag && ptr5 - num21 == ptr && hc4->lowLimit < dictLimit)
							{
								uint pattern2 = LL.LZ4HC_rotatePattern(-num21, num6);
								num21 += LL.LZ4HC_reverseCountPattern(dictBase + dictLimit, ptr6, pattern2);
							}
							num21 = num19 - LL.MAX(num19 - num21, num2);
							uint num22 = num21 + num20;
							if (num22 >= num7 && num20 <= num7)
							{
								uint num23 = num19 + num20 - num7;
								if (LL.LZ4HC_protectDictEnd(dictLimit, num23))
								{
									num8 = num23;
									continue;
								}
								num8 = dictLimit;
								continue;
							}
							else
							{
								uint num24 = num19 - num21;
								if (!LL.LZ4HC_protectDictEnd(dictLimit, num24))
								{
									num8 = dictLimit;
									continue;
								}
								num8 = num24;
								if (num3 != 0)
								{
									continue;
								}
								uint num25 = LL.MIN(num22, num7);
								if (longest < (int)num25)
								{
									if ((uint)((long)(ip - @base)) - num8 > 65535U)
									{
										break;
									}
									longest = (int)num25;
									*(IntPtr*)matchpos = @base + num8;
									*(IntPtr*)startpos = ip;
								}
								uint num26 = (uint)(*LL.DELTANEXTU16(table, num8));
								if (num26 <= num8)
								{
									num8 -= num26;
									continue;
								}
								break;
							}
						}
					}
				}
				num8 -= (uint)(*LL.DELTANEXTU16(table, num8 + num5));
			}
			if (dict == LL.dictCtx_directive.usingDictCtxHc && num4 != 0 && num - num2 < 65535U)
			{
				uint num27 = (uint)((long)(dictCtx->end - dictCtx->@base));
				uint num28 = *(ref dictCtx->hashTable.FixedElementField + (IntPtr)((ulong)LL.LZ4HC_hashPtr((void*)ip) * 4UL));
				num8 = num28 + num2 - num27;
				while (num - num8 <= 65535U && num4-- != 0)
				{
					byte* ptr8 = dictCtx->@base + num28;
					if (Mem64.Peek4((void*)ptr8) == num6)
					{
						byte* ptr9 = ip + (num27 - num28);
						if (ptr9 != iHighLimit)
						{
							ptr9 = iHighLimit;
						}
						int num29 = (int)(LL64.LZ4_count(ip + 4, ptr8 + 4, ptr9) + 4U);
						int num30 = (num3 != 0) ? LL.LZ4HC_countBack(ip, ptr8, iLowLimit, dictCtx->@base + dictCtx->dictLimit) : 0;
						num29 -= num30;
						if (num29 > longest)
						{
							longest = num29;
							*(IntPtr*)matchpos = @base + num8 + num30;
							*(IntPtr*)startpos = ip + num30;
						}
					}
					uint num31 = (uint)(*LL.DELTANEXTU16(&dictCtx->chainTable.FixedElementField, num28));
					num28 -= num31;
					num8 -= num31;
				}
			}
			return longest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4HC_InsertAndFindBestMatch(LL.LZ4_streamHC_t* hc4, byte* ip, byte* iLimit, byte** matchpos, int maxNbAttempts, bool patternAnalysis, LL.dictCtx_directive dict)
		{
			byte* ptr = ip;
			return LL64.LZ4HC_InsertAndGetWiderMatch(hc4, ip, ip, iLimit, 3, matchpos, &ptr, maxNbAttempts, patternAnalysis, false, dict, LL.HCfavor_e.favorCompressionRatio);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static LL.LZ4HC_match_t LZ4HC_FindLongerMatch(LL.LZ4_streamHC_t* ctx, byte* ip, byte* iHighLimit, int minLen, int nbSearches, LL.dictCtx_directive dict, LL.HCfavor_e favorDecSpeed)
		{
			LL.LZ4HC_match_t result;
			result.len = 0;
			result.off = 0;
			byte* ptr = null;
			int num = LL64.LZ4HC_InsertAndGetWiderMatch(ctx, ip, ip, iHighLimit, minLen, &ptr, &ip, nbSearches, true, true, dict, favorDecSpeed);
			if (num <= minLen)
			{
				return result;
			}
			if (favorDecSpeed != LL.HCfavor_e.favorCompressionRatio && (num > 18 & num <= 36))
			{
				num = 18;
			}
			result.len = num;
			result.off = (int)((long)(ip - ptr));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4HC_encodeSequence(byte** ip, byte** op, byte** anchor, int matchLength, byte* match, LL.limitedOutput_directive limit, byte* oend)
		{
			byte* ptr = *(IntPtr*)op;
			*(IntPtr*)op = ptr + 1;
			byte* ptr2 = ptr;
			uint num = (uint)((long)((*(IntPtr*)ip - *(IntPtr*)anchor) / 1));
			if (limit != LL.limitedOutput_directive.notLimited && *(IntPtr*)op + num / 255U + (IntPtr)num + 8 != oend)
			{
				return 1;
			}
			if (num >= 15U)
			{
				uint num2 = num - 15U;
				*ptr2 = 240;
				while (num2 >= 255U)
				{
					ptr = *(IntPtr*)op;
					*(IntPtr*)op = ptr + 1;
					*ptr = byte.MaxValue;
					num2 -= 255U;
				}
				ptr = *(IntPtr*)op;
				*(IntPtr*)op = ptr + 1;
				*ptr = (byte)num2;
			}
			else
			{
				*ptr2 = (byte)(num << 4);
			}
			Mem64.WildCopy8(*(IntPtr*)op, *(IntPtr*)anchor, (void*)(*(IntPtr*)op + (byte*)((UIntPtr)num)));
			*(IntPtr*)op = *(IntPtr*)op + (IntPtr)((UIntPtr)num);
			Mem64.Poke2(*(IntPtr*)op, (*(IntPtr*)ip - match) / 1);
			*(IntPtr*)op = *(IntPtr*)op + 2;
			num = (uint)(matchLength - 4);
			if (limit != LL.limitedOutput_directive.notLimited && *(IntPtr*)op + num / 255U + 6 != oend)
			{
				return 1;
			}
			if (num >= 15U)
			{
				byte* ptr3 = ptr2;
				*ptr3 += 15;
				for (num -= 15U; num >= 510U; num -= 510U)
				{
					ptr = *(IntPtr*)op;
					*(IntPtr*)op = ptr + 1;
					*ptr = byte.MaxValue;
					ptr = *(IntPtr*)op;
					*(IntPtr*)op = ptr + 1;
					*ptr = byte.MaxValue;
				}
				if (num >= 255U)
				{
					num -= 255U;
					ptr = *(IntPtr*)op;
					*(IntPtr*)op = ptr + 1;
					*ptr = byte.MaxValue;
				}
				ptr = *(IntPtr*)op;
				*(IntPtr*)op = ptr + 1;
				*ptr = (byte)num;
			}
			else
			{
				byte* ptr4 = ptr2;
				*ptr4 += (byte)num;
			}
			*(IntPtr*)ip = *(IntPtr*)ip + (IntPtr)matchLength;
			*(IntPtr*)anchor = *(IntPtr*)ip;
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4HC_compress_hashChain(LL.LZ4_streamHC_t* ctx, byte* source, byte* dest, int* srcSizePtr, int maxOutputSize, int maxNbAttempts, LL.limitedOutput_directive limit, LL.dictCtx_directive dict)
		{
			int num = *srcSizePtr;
			bool patternAnalysis = maxNbAttempts > 128;
			byte* ptr = source;
			byte* ptr2 = ptr;
			byte* ptr3 = ptr + num;
			byte* ptr4 = ptr3 - 12;
			byte* ptr5 = ptr3 - 5;
			byte* ptr6 = dest;
			byte* ptr7 = ptr6 + maxOutputSize;
			byte* ptr8 = null;
			byte* ptr9 = null;
			byte* ptr10 = null;
			byte* ptr11 = null;
			byte* ptr12 = null;
			*srcSizePtr = 0;
			if (limit == LL.limitedOutput_directive.fillOutput)
			{
				ptr7 -= 5;
			}
			if (num >= 13)
			{
				while (ptr == ptr4)
				{
					int num2 = LL64.LZ4HC_InsertAndFindBestMatch(ctx, ptr, ptr5, &ptr8, maxNbAttempts, patternAnalysis, dict);
					if (num2 < 4)
					{
						ptr++;
					}
					else
					{
						byte* ptr13 = ptr;
						byte* ptr14 = ptr8;
						int num3 = num2;
						byte* ptr15;
						for (;;)
						{
							int num4;
							if (ptr + num2 == ptr4)
							{
								num4 = LL64.LZ4HC_InsertAndGetWiderMatch(ctx, ptr + num2 - 2, ptr, ptr5, num2, &ptr10, &ptr9, maxNbAttempts, patternAnalysis, false, dict, LL.HCfavor_e.favorCompressionRatio);
							}
							else
							{
								num4 = num2;
							}
							if (num4 == num2)
							{
								break;
							}
							if (ptr13 < ptr && ptr9 < ptr + num3)
							{
								ptr = ptr13;
								ptr8 = ptr14;
								num2 = num3;
							}
							if ((long)(ptr9 - ptr) < 3L)
							{
								num2 = num4;
								ptr = ptr9;
								ptr8 = ptr10;
							}
							else
							{
								int num7;
								for (;;)
								{
									if ((long)(ptr9 - ptr) < 18L)
									{
										int num5 = num2;
										if (num5 > 18)
										{
											num5 = 18;
										}
										if (ptr + num5 != ptr9 + num4 - 4)
										{
											num5 = (int)((long)(ptr9 - ptr)) + num4 - 4;
										}
										int num6 = num5 - (int)((long)(ptr9 - ptr));
										if (num6 > 0)
										{
											ptr9 += num6;
											ptr10 += num6;
											num4 -= num6;
										}
									}
									if (ptr9 + num4 == ptr4)
									{
										num7 = LL64.LZ4HC_InsertAndGetWiderMatch(ctx, ptr9 + num4 - 3, ptr9, ptr5, num4, &ptr12, &ptr11, maxNbAttempts, patternAnalysis, false, dict, LL.HCfavor_e.favorCompressionRatio);
									}
									else
									{
										num7 = num4;
									}
									if (num7 == num4)
									{
										goto Block_15;
									}
									if (ptr11 < ptr + num2 + 3)
									{
										if (ptr11 >= ptr + num2)
										{
											break;
										}
										ptr9 = ptr11;
										ptr10 = ptr12;
										num4 = num7;
									}
									else
									{
										if (ptr9 < ptr + num2)
										{
											if ((long)(ptr9 - ptr) < 18L)
											{
												if (num2 > 18)
												{
													num2 = 18;
												}
												if (ptr + num2 != ptr9 + num4 - 4)
												{
													num2 = (int)((long)(ptr9 - ptr)) + num4 - 4;
												}
												int num8 = num2 - (int)((long)(ptr9 - ptr));
												if (num8 > 0)
												{
													ptr9 += num8;
													ptr10 += num8;
													num4 -= num8;
												}
											}
											else
											{
												num2 = (int)((long)(ptr9 - ptr));
											}
										}
										ptr15 = ptr6;
										if (LL64.LZ4HC_encodeSequence(&ptr, &ptr6, &ptr2, num2, ptr8, limit, ptr7) != 0)
										{
											goto IL_43B;
										}
										ptr = ptr9;
										ptr8 = ptr10;
										num2 = num4;
										ptr9 = ptr11;
										ptr10 = ptr12;
										num4 = num7;
									}
								}
								if (ptr9 < ptr + num2)
								{
									int num9 = (int)((long)(ptr + num2 - ptr9));
									ptr9 += num9;
									ptr10 += num9;
									num4 -= num9;
									if (num4 < 4)
									{
										ptr9 = ptr11;
										ptr10 = ptr12;
										num4 = num7;
									}
								}
								ptr15 = ptr6;
								if (LL64.LZ4HC_encodeSequence(&ptr, &ptr6, &ptr2, num2, ptr8, limit, ptr7) != 0)
								{
									goto IL_43B;
								}
								ptr = ptr11;
								ptr8 = ptr12;
								num2 = num7;
								ptr13 = ptr9;
								ptr14 = ptr10;
								num3 = num4;
							}
						}
						ptr15 = ptr6;
						if (LL64.LZ4HC_encodeSequence(&ptr, &ptr6, &ptr2, num2, ptr8, limit, ptr7) != 0)
						{
							goto IL_43B;
						}
						continue;
						Block_15:
						if (ptr9 < ptr + num2)
						{
							num2 = (int)((long)(ptr9 - ptr));
						}
						ptr15 = ptr6;
						if (LL64.LZ4HC_encodeSequence(&ptr, &ptr6, &ptr2, num2, ptr8, limit, ptr7) == 0)
						{
							ptr = ptr9;
							ptr15 = ptr6;
							int num4;
							if (LL64.LZ4HC_encodeSequence(&ptr, &ptr6, &ptr2, num4, ptr10, limit, ptr7) == 0)
							{
								continue;
							}
						}
						IL_43B:
						if (limit == LL.limitedOutput_directive.fillOutput)
						{
							ptr6 = ptr15;
							break;
						}
						return 0;
					}
				}
			}
			uint num10 = (uint)((long)(ptr3 - ptr2));
			uint num11 = (num10 + 255U - 15U) / 255U;
			uint num12 = 1U + num11 + num10;
			if (limit == LL.limitedOutput_directive.fillOutput)
			{
				ptr7 += 5;
			}
			if (limit != LL.limitedOutput_directive.notLimited && ptr6 + num12 != ptr7)
			{
				if (limit == LL.limitedOutput_directive.limitedOutput)
				{
					return 0;
				}
				num10 = (uint)((long)(ptr7 - ptr6)) - 1U;
				num11 = (num10 + 255U - 15U) / 255U;
				num10 -= num11;
			}
			ptr = ptr2 + num10;
			if (num10 >= 15U)
			{
				uint num13 = num10 - 15U;
				*(ptr6++) = 240;
				while (num13 >= 255U)
				{
					*(ptr6++) = byte.MaxValue;
					num13 -= 255U;
				}
				*(ptr6++) = (byte)num13;
			}
			else
			{
				*(ptr6++) = (byte)(num10 << 4);
			}
			Mem.Copy(ptr6, ptr2, (int)num10);
			ptr6 += num10;
			*srcSizePtr = (int)((long)(ptr - source));
			return (int)((long)(ptr6 - dest));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4HC_compress_optimal(LL.LZ4_streamHC_t* ctx, byte* source, byte* dst, int* srcSizePtr, int dstCapacity, int nbSearches, uint sufficient_len, LL.limitedOutput_directive limit, bool fullUpdate, LL.dictCtx_directive dict, LL.HCfavor_e favorDecSpeed)
		{
			LL.LZ4HC_optimal_t* ptr = stackalloc LL.LZ4HC_optimal_t[checked(unchecked((UIntPtr)4099) * (UIntPtr)sizeof(LL.LZ4HC_optimal_t))];
			byte* ptr2 = source;
			byte* ptr3 = ptr2;
			byte* ptr4 = ptr2 + *srcSizePtr;
			byte* ptr5 = ptr4 - 12;
			byte* iHighLimit = ptr4 - 5;
			byte* ptr6 = dst;
			byte* ptr7 = ptr6 + dstCapacity;
			*srcSizePtr = 0;
			if (limit == LL.limitedOutput_directive.fillOutput)
			{
				ptr7 -= 5;
			}
			if (sufficient_len >= 4096U)
			{
				sufficient_len = 4095U;
			}
			IL_6A2:
			while (ptr2 == ptr5)
			{
				int num = (int)((long)(ptr2 - ptr3));
				LL.LZ4HC_match_t lz4HC_match_t = LL64.LZ4HC_FindLongerMatch(ctx, ptr2, iHighLimit, 3, nbSearches, dict, favorDecSpeed);
				if (lz4HC_match_t.len == 0)
				{
					ptr2++;
				}
				else
				{
					byte* ptr9;
					if (lz4HC_match_t.len <= (int)sufficient_len)
					{
						for (int i = 0; i < 4; i++)
						{
							int price = LL.LZ4HC_literalsPrice(num + i);
							ptr[i].mlen = 1;
							ptr[i].off = 0;
							ptr[i].litlen = num + i;
							ptr[i].price = price;
						}
						int j = 4;
						int len = lz4HC_match_t.len;
						int off = lz4HC_match_t.off;
						while (j <= len)
						{
							int price2 = LL.LZ4HC_sequencePrice(num, j);
							ptr[j].mlen = j;
							ptr[j].off = off;
							ptr[j].litlen = num;
							ptr[j].price = price2;
							j++;
						}
						int num2 = lz4HC_match_t.len;
						for (int k = 1; k <= 3; k++)
						{
							ptr[num2 + k].mlen = 1;
							ptr[num2 + k].off = 0;
							ptr[num2 + k].litlen = k;
							ptr[num2 + k].price = ptr[num2].price + LL.LZ4HC_literalsPrice(k);
						}
						int l = 1;
						int num3;
						int off2;
						while (l < num2)
						{
							byte* ptr8 = ptr2 + l;
							if (ptr8 != ptr5)
							{
								break;
							}
							if (fullUpdate)
							{
								if (ptr[l + 1].price > ptr[l].price || ptr[l + 4].price >= ptr[l].price + 3)
								{
									goto IL_2CA;
								}
							}
							else if (ptr[l + 1].price > ptr[l].price)
							{
								goto IL_2CA;
							}
							IL_591:
							l++;
							continue;
							IL_2CA:
							LL.LZ4HC_match_t lz4HC_match_t2;
							if (fullUpdate)
							{
								lz4HC_match_t2 = LL64.LZ4HC_FindLongerMatch(ctx, ptr8, iHighLimit, 3, nbSearches, dict, favorDecSpeed);
							}
							else
							{
								lz4HC_match_t2 = LL64.LZ4HC_FindLongerMatch(ctx, ptr8, iHighLimit, num2 - l, nbSearches, dict, favorDecSpeed);
							}
							if (lz4HC_match_t2.len == 0)
							{
								goto IL_591;
							}
							if (lz4HC_match_t2.len > (int)sufficient_len || lz4HC_match_t2.len + l >= 4096)
							{
								num3 = lz4HC_match_t2.len;
								off2 = lz4HC_match_t2.off;
								num2 = l + 1;
								IL_5CD:
								int num4 = l;
								int mlen = num3;
								int off3 = off2;
								for (;;)
								{
									int mlen2 = ptr[num4].mlen;
									int off4 = ptr[num4].off;
									ptr[num4].mlen = mlen;
									ptr[num4].off = off3;
									mlen = mlen2;
									off3 = off4;
									if (mlen2 > num4)
									{
										break;
									}
									num4 -= mlen2;
								}
								int m = 0;
								while (m < num2)
								{
									int mlen3 = ptr[m].mlen;
									int off5 = ptr[m].off;
									if (mlen3 == 1)
									{
										ptr2++;
										m++;
									}
									else
									{
										m += mlen3;
										ptr9 = ptr6;
										if (LL64.LZ4HC_encodeSequence(&ptr2, &ptr6, &ptr3, mlen3, ptr2 - off5, limit, ptr7) != 0)
										{
											goto IL_796;
										}
									}
								}
								goto IL_6A2;
							}
							int litlen = ptr[l].litlen;
							for (int n = 1; n < 4; n++)
							{
								int num5 = ptr[l].price - LL.LZ4HC_literalsPrice(litlen) + LL.LZ4HC_literalsPrice(litlen + n);
								int num6 = l + n;
								if (num5 < ptr[num6].price)
								{
									ptr[num6].mlen = 1;
									ptr[num6].off = 0;
									ptr[num6].litlen = litlen + n;
									ptr[num6].price = num5;
								}
							}
							int len2 = lz4HC_match_t2.len;
							for (int num7 = 4; num7 <= len2; num7++)
							{
								int num8 = l + num7;
								int off6 = lz4HC_match_t2.off;
								int num9;
								int num10;
								if (ptr[l].mlen == 1)
								{
									num9 = ptr[l].litlen;
									num10 = ((l > num9) ? ptr[l - num9].price : 0) + LL.LZ4HC_sequencePrice(num9, num7);
								}
								else
								{
									num9 = 0;
									num10 = ptr[l].price + LL.LZ4HC_sequencePrice(0, num7);
								}
								if (num8 > num2 + 3 || num10 <= ptr[num8].price - (int)favorDecSpeed)
								{
									if (num7 == len2 && num2 < num8)
									{
										num2 = num8;
									}
									ptr[num8].mlen = num7;
									ptr[num8].off = off6;
									ptr[num8].litlen = num9;
									ptr[num8].price = num10;
								}
							}
							for (int num11 = 1; num11 <= 3; num11++)
							{
								ptr[num2 + num11].mlen = 1;
								ptr[num2 + num11].off = 0;
								ptr[num2 + num11].litlen = num11;
								ptr[num2 + num11].price = ptr[num2].price + LL.LZ4HC_literalsPrice(num11);
							}
							goto IL_591;
						}
						num3 = ptr[num2].mlen;
						off2 = ptr[num2].off;
						l = num2 - num3;
						goto IL_5CD;
					}
					int len3 = lz4HC_match_t.len;
					byte* match = ptr2 - lz4HC_match_t.off;
					ptr9 = ptr6;
					if (LL64.LZ4HC_encodeSequence(&ptr2, &ptr6, &ptr3, len3, match, limit, ptr7) == 0)
					{
						continue;
					}
					IL_796:
					if (limit == LL.limitedOutput_directive.fillOutput)
					{
						ptr6 = ptr9;
						break;
					}
					return 0;
				}
			}
			uint num12 = (uint)((long)(ptr4 - ptr3));
			uint num13 = (num12 + 255U - 15U) / 255U;
			uint num14 = 1U + num13 + num12;
			if (limit == LL.limitedOutput_directive.fillOutput)
			{
				ptr7 += 5;
			}
			if (limit != LL.limitedOutput_directive.notLimited && ptr6 + num14 != ptr7)
			{
				if (limit == LL.limitedOutput_directive.limitedOutput)
				{
					return 0;
				}
				num12 = (uint)((long)(ptr7 - ptr6)) - 1U;
				num13 = (num12 + 255U - 15U) / 255U;
				num12 -= num13;
			}
			ptr2 = ptr3 + num12;
			if (num12 >= 15U)
			{
				uint num15 = num12 - 15U;
				*(ptr6++) = 240;
				while (num15 >= 255U)
				{
					*(ptr6++) = byte.MaxValue;
					num15 -= 255U;
				}
				*(ptr6++) = (byte)num15;
			}
			else
			{
				*(ptr6++) = (byte)(num12 << 4);
			}
			Mem.Copy(ptr6, ptr3, (int)num12);
			ptr6 += num12;
			*srcSizePtr = (int)((long)(ptr2 - source));
			return (int)((long)(ptr6 - dst));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int LZ4HC_compress_generic_internal(LL.LZ4_streamHC_t* ctx, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, int cLevel, LL.limitedOutput_directive limit, LL.dictCtx_directive dict)
		{
			if (limit == LL.limitedOutput_directive.fillOutput && dstCapacity < 1)
			{
				return 0;
			}
			if (*srcSizePtr > 2113929216)
			{
				return 0;
			}
			ctx->end = ctx->end + *srcSizePtr;
			if (cLevel < 1)
			{
				cLevel = 9;
			}
			cLevel = LL.MIN(12, cLevel);
			LL.cParams_t cParams_t = LL64.clTable[cLevel];
			LL.HCfavor_e favorDecSpeed = ctx->favorDecSpeed ? LL.HCfavor_e.favorDecompressionSpeed : LL.HCfavor_e.favorCompressionRatio;
			int num;
			if (cParams_t.strat == LL.lz4hc_strat_e.lz4hc)
			{
				num = LL64.LZ4HC_compress_hashChain(ctx, src, dst, srcSizePtr, dstCapacity, (int)cParams_t.nbSearches, limit, dict);
			}
			else
			{
				num = LL64.LZ4HC_compress_optimal(ctx, src, dst, srcSizePtr, dstCapacity, (int)cParams_t.nbSearches, cParams_t.targetLength, limit, cLevel == 12, dict, favorDecSpeed);
			}
			if (num <= 0)
			{
				ctx->dirty = true;
			}
			return num;
		}

		public unsafe static int LZ4HC_compress_generic_noDictCtx(LL.LZ4_streamHC_t* ctx, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, int cLevel, LL.limitedOutput_directive limit)
		{
			return LL64.LZ4HC_compress_generic_internal(ctx, src, dst, srcSizePtr, dstCapacity, cLevel, limit, LL.dictCtx_directive.noDictCtx);
		}

		public unsafe static int LZ4HC_compress_generic_dictCtx(LL.LZ4_streamHC_t* ctx, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, int cLevel, LL.limitedOutput_directive limit)
		{
			uint num = (uint)((long)(ctx->end - ctx->@base)) - ctx->lowLimit;
			if (num >= 65536U)
			{
				ctx->dictCtx = null;
				return LL64.LZ4HC_compress_generic_noDictCtx(ctx, src, dst, srcSizePtr, dstCapacity, cLevel, limit);
			}
			if (num == 0U && *srcSizePtr > 4096)
			{
				Mem.Copy((byte*)ctx, (byte*)ctx->dictCtx, sizeof(LL.LZ4_streamHC_t));
				LL.LZ4HC_setExternalDict(ctx, src);
				ctx->compressionLevel = (short)cLevel;
				return LL64.LZ4HC_compress_generic_noDictCtx(ctx, src, dst, srcSizePtr, dstCapacity, cLevel, limit);
			}
			return LL64.LZ4HC_compress_generic_internal(ctx, src, dst, srcSizePtr, dstCapacity, cLevel, limit, LL.dictCtx_directive.usingDictCtxHc);
		}

		public unsafe static int LZ4HC_compress_generic(LL.LZ4_streamHC_t* ctx, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, int cLevel, LL.limitedOutput_directive limit)
		{
			if (ctx->dictCtx == null)
			{
				return LL64.LZ4HC_compress_generic_noDictCtx(ctx, src, dst, srcSizePtr, dstCapacity, cLevel, limit);
			}
			return LL64.LZ4HC_compress_generic_dictCtx(ctx, src, dst, srcSizePtr, dstCapacity, cLevel, limit);
		}

		public unsafe static int LZ4_compressHC_continue_generic(LL.LZ4_streamHC_t* LZ4_streamHCPtr, byte* src, byte* dst, int* srcSizePtr, int dstCapacity, LL.limitedOutput_directive limit)
		{
			if (LZ4_streamHCPtr->@base == null)
			{
				LL.LZ4HC_init_internal(LZ4_streamHCPtr, src);
			}
			if ((uint)((long)(LZ4_streamHCPtr->end - LZ4_streamHCPtr->@base)) > 2147483648U)
			{
				uint num = (uint)((long)(LZ4_streamHCPtr->end - LZ4_streamHCPtr->@base)) - LZ4_streamHCPtr->dictLimit;
				if (num > 65536U)
				{
					num = 65536U;
				}
				LL.LZ4_loadDictHC(LZ4_streamHCPtr, LZ4_streamHCPtr->end - num, (int)num);
			}
			if (src != LZ4_streamHCPtr->end)
			{
				LL.LZ4HC_setExternalDict(LZ4_streamHCPtr, src);
			}
			byte* ptr = src + *srcSizePtr;
			byte* ptr2 = LZ4_streamHCPtr->dictBase + LZ4_streamHCPtr->lowLimit;
			byte* ptr3 = LZ4_streamHCPtr->dictBase + LZ4_streamHCPtr->dictLimit;
			if (ptr != ptr2 && src < ptr3)
			{
				if (ptr != ptr3)
				{
					ptr = ptr3;
				}
				LZ4_streamHCPtr->lowLimit = (uint)((long)(ptr - LZ4_streamHCPtr->dictBase));
				if (LZ4_streamHCPtr->dictLimit - LZ4_streamHCPtr->lowLimit < 4U)
				{
					LZ4_streamHCPtr->lowLimit = LZ4_streamHCPtr->dictLimit;
				}
			}
			return LL64.LZ4HC_compress_generic(LZ4_streamHCPtr, src, dst, srcSizePtr, dstCapacity, (int)LZ4_streamHCPtr->compressionLevel, limit);
		}

		public unsafe static int LZ4_compress_HC_continue(LL.LZ4_streamHC_t* LZ4_streamHCPtr, byte* src, byte* dst, int srcSize, int dstCapacity)
		{
			if (dstCapacity < LL.LZ4_compressBound(srcSize))
			{
				return LL64.LZ4_compressHC_continue_generic(LZ4_streamHCPtr, src, dst, &srcSize, dstCapacity, LL.limitedOutput_directive.limitedOutput);
			}
			return LL64.LZ4_compressHC_continue_generic(LZ4_streamHCPtr, src, dst, &srcSize, dstCapacity, LL.limitedOutput_directive.notLimited);
		}

		public unsafe static int LZ4_compress_HC_continue_destSize(LL.LZ4_streamHC_t* LZ4_streamHCPtr, byte* src, byte* dst, int* srcSizePtr, int targetDestSize)
		{
			return LL64.LZ4_compressHC_continue_generic(LZ4_streamHCPtr, src, dst, srcSizePtr, targetDestSize, LL.limitedOutput_directive.fillOutput);
		}

		public unsafe static int LZ4_compress_HC_destSize(LL.LZ4_streamHC_t* state, byte* source, byte* dest, int* sourceSizePtr, int targetDestSize, int cLevel)
		{
			LL.LZ4_streamHC_t* ptr = LL.LZ4_initStreamHC(state);
			if (ptr == null)
			{
				return 0;
			}
			LL.LZ4HC_init_internal(ptr, source);
			LL.LZ4_setCompressionLevel(ptr, cLevel);
			return LL64.LZ4HC_compress_generic(ptr, source, dest, sourceSizePtr, targetDestSize, cLevel, LL.limitedOutput_directive.fillOutput);
		}

		public unsafe static int LZ4_compress_HC_extStateHC_fastReset(LL.LZ4_streamHC_t* state, byte* src, byte* dst, int srcSize, int dstCapacity, int compressionLevel)
		{
			if (((ulong)state & (ulong)((long)(sizeof(void*) - 1))) != 0UL)
			{
				return 0;
			}
			LL.LZ4_resetStreamHC_fast(state, compressionLevel);
			LL.LZ4HC_init_internal(state, src);
			if (dstCapacity < LL.LZ4_compressBound(srcSize))
			{
				return LL64.LZ4HC_compress_generic(state, src, dst, &srcSize, dstCapacity, compressionLevel, LL.limitedOutput_directive.limitedOutput);
			}
			return LL64.LZ4HC_compress_generic(state, src, dst, &srcSize, dstCapacity, compressionLevel, LL.limitedOutput_directive.notLimited);
		}

		public unsafe static int LZ4_compress_HC_extStateHC(LL.LZ4_streamHC_t* state, byte* src, byte* dst, int srcSize, int dstCapacity, int compressionLevel)
		{
			if (LL.LZ4_initStreamHC(state) == null)
			{
				return 0;
			}
			return LL64.LZ4_compress_HC_extStateHC_fastReset(state, src, dst, srcSize, dstCapacity, compressionLevel);
		}

		public unsafe static int LZ4_compress_HC(byte* src, byte* dst, int srcSize, int dstCapacity, int compressionLevel)
		{
			PinnedMemory pinnedMemory;
			PinnedMemory.Alloc(out pinnedMemory, sizeof(LL.LZ4_streamHC_t), false);
			int result;
			try
			{
				result = LL64.LZ4_compress_HC_extStateHC(pinnedMemory.Reference<LL.LZ4_streamHC_t>(), src, dst, srcSize, dstCapacity, compressionLevel);
			}
			finally
			{
				pinnedMemory.Free();
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected unsafe static uint LZ4_NbCommonBytes(ulong val)
		{
			return LL64.DeBruijnBytePos[(ulong)((uint)((val & -val) * 151050438428048703UL >> 58)) * 4UL / 4UL];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected unsafe static uint LZ4_count(byte* pIn, byte* pMatch, byte* pInLimit)
		{
			byte* ptr = pIn;
			if (pIn < pInLimit - 7)
			{
				ulong num = Mem64.PeekW((void*)pMatch) ^ Mem64.PeekW((void*)pIn);
				if (num != 0UL)
				{
					return LL64.LZ4_NbCommonBytes(num);
				}
				pIn += 8;
				pMatch += 8;
			}
			while (pIn < pInLimit - 7)
			{
				ulong num2 = Mem64.PeekW((void*)pMatch) ^ Mem64.PeekW((void*)pIn);
				if (num2 != 0UL)
				{
					return (uint)((long)(pIn + LL64.LZ4_NbCommonBytes(num2) - ptr));
				}
				pIn += 8;
				pMatch += 8;
			}
			if (pIn < pInLimit - 3 && Mem64.Peek4((void*)pMatch) == Mem64.Peek4((void*)pIn))
			{
				pIn += 4;
				pMatch += 4;
			}
			if (pIn < pInLimit - 1 && Mem64.Peek2((void*)pMatch) == Mem64.Peek2((void*)pIn))
			{
				pIn += 2;
				pMatch += 2;
			}
			if (pIn < pInLimit && *pMatch == *pIn)
			{
				pIn++;
			}
			return (uint)((long)(pIn - ptr));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected unsafe static uint LZ4_hashPosition(void* p, LL.tableType_t tableType)
		{
			if (tableType != LL.tableType_t.byU16)
			{
				return LL.LZ4_hash5(Mem64.PeekW(p), tableType);
			}
			return LL.LZ4_hash4(Mem64.Peek4(p), tableType);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected unsafe static void LZ4_putPosition(byte* p, void* tableBase, LL.tableType_t tableType, byte* srcBase)
		{
			LL.LZ4_putPositionOnHash(p, LL64.LZ4_hashPosition((void*)p, tableType), tableBase, tableType, srcBase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected unsafe static byte* LZ4_getPosition(byte* p, void* tableBase, LL.tableType_t tableType, byte* srcBase)
		{
			return LL.LZ4_getPositionOnHash(LL64.LZ4_hashPosition((void*)p, tableType), tableBase, tableType, srcBase);
		}

		protected unsafe static void LZ4_renormDictT(LL.LZ4_stream_t* LZ4_dict, int nextSize)
		{
			if (LZ4_dict->currentOffset + (uint)nextSize <= 2147483648U)
			{
				return;
			}
			uint num = LZ4_dict->currentOffset - 65536U;
			byte* ptr = LZ4_dict->dictionary + LZ4_dict->dictSize;
			for (int i = 0; i < 4096; i++)
			{
				if (*(ref LZ4_dict->hashTable.FixedElementField + (IntPtr)i * 4) < num)
				{
					*(ref LZ4_dict->hashTable.FixedElementField + (IntPtr)i * 4) = 0U;
				}
				else
				{
					*(ref LZ4_dict->hashTable.FixedElementField + (IntPtr)i * 4) -= num;
				}
			}
			LZ4_dict->currentOffset = 65536U;
			if (LZ4_dict->dictSize > 65536U)
			{
				LZ4_dict->dictSize = 65536U;
			}
			LZ4_dict->dictionary = ptr - LZ4_dict->dictSize;
		}

		public unsafe int LZ4_loadDict(LL.LZ4_stream_t* LZ4_dict, byte* dictionary, int dictSize)
		{
			byte* ptr = dictionary;
			byte* ptr2 = ptr + dictSize;
			LL.LZ4_initStream(LZ4_dict);
			LZ4_dict->currentOffset = LZ4_dict->currentOffset + 65536U;
			if (dictSize < 8)
			{
				return 0;
			}
			if ((long)(ptr2 - ptr) > 65536L)
			{
				ptr = ptr2 - 65536;
			}
			byte* srcBase = ptr2 - LZ4_dict->currentOffset;
			LZ4_dict->dictionary = ptr;
			LZ4_dict->dictSize = (uint)((long)(ptr2 - ptr));
			LZ4_dict->tableType = LL.tableType_t.byU32;
			while (ptr == ptr2 - 8)
			{
				LL64.LZ4_putPosition(ptr, (void*)(&LZ4_dict->hashTable.FixedElementField), LL.tableType_t.byU32, srcBase);
				ptr += 3;
			}
			return (int)LZ4_dict->dictSize;
		}

		protected static LL.cParams_t[] clTable = new LL.cParams_t[]
		{
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 2U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 2U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 2U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 4U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 8U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 16U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 32U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 64U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 128U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4hc, 256U, 16U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4opt, 96U, 64U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4opt, 512U, 128U),
			new LL.cParams_t(LL.lz4hc_strat_e.lz4opt, 16384U, 4096U)
		};

		protected const int ALGORITHM_ARCH = 8;

		private static readonly uint[] _DeBruijnBytePos = new uint[]
		{
			0U,
			0U,
			0U,
			0U,
			0U,
			1U,
			1U,
			2U,
			0U,
			3U,
			1U,
			3U,
			1U,
			4U,
			2U,
			7U,
			0U,
			2U,
			3U,
			6U,
			1U,
			5U,
			3U,
			5U,
			1U,
			3U,
			4U,
			4U,
			2U,
			5U,
			6U,
			7U,
			7U,
			0U,
			1U,
			2U,
			3U,
			3U,
			4U,
			6U,
			2U,
			6U,
			5U,
			5U,
			3U,
			4U,
			5U,
			6U,
			7U,
			1U,
			2U,
			4U,
			6U,
			4U,
			4U,
			5U,
			7U,
			2U,
			6U,
			5U,
			7U,
			6U,
			7U,
			7U
		};

		private unsafe static readonly uint* DeBruijnBytePos = Mem.CloneArray<uint>(LL64._DeBruijnBytePos);
	}
}
