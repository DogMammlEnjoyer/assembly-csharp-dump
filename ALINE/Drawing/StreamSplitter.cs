using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Drawing
{
	[BurstCompile]
	internal struct StreamSplitter : IJob
	{
		static StreamSplitter()
		{
			StreamSplitter.CommandSizes[0] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<Color32>();
			StreamSplitter.CommandSizes[1] = UnsafeUtility.SizeOf<CommandBuilder.Command>();
			StreamSplitter.CommandSizes[2] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<float4x4>();
			StreamSplitter.CommandSizes[3] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<float4x4>();
			StreamSplitter.CommandSizes[4] = UnsafeUtility.SizeOf<CommandBuilder.Command>();
			StreamSplitter.CommandSizes[5] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.LineData>();
			StreamSplitter.CommandSizes[7] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.CircleXZData>();
			StreamSplitter.CommandSizes[10] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.SphereData>();
			StreamSplitter.CommandSizes[6] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.CircleData>();
			StreamSplitter.CommandSizes[8] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.CircleData>();
			StreamSplitter.CommandSizes[9] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.CircleXZData>();
			StreamSplitter.CommandSizes[11] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.BoxData>();
			StreamSplitter.CommandSizes[12] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.PlaneData>();
			StreamSplitter.CommandSizes[13] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.BoxData>();
			StreamSplitter.CommandSizes[14] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.TriangleData>();
			StreamSplitter.CommandSizes[15] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.PersistData>();
			StreamSplitter.CommandSizes[16] = UnsafeUtility.SizeOf<CommandBuilder.Command>();
			StreamSplitter.CommandSizes[17] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.TextData>();
			StreamSplitter.CommandSizes[18] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.TextData3D>();
			StreamSplitter.CommandSizes[19] = UnsafeUtility.SizeOf<CommandBuilder.Command>() + UnsafeUtility.SizeOf<CommandBuilder.LineWidthData>();
			StreamSplitter.CommandSizes[20] = UnsafeUtility.SizeOf<CommandBuilder.Command>();
			StreamSplitter.CommandSizes[21] = UnsafeUtility.SizeOf<CommandBuilder.Command>();
		}

		public unsafe void Execute()
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			NativeArray<int> nativeArray = new NativeArray<int>(32, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(32, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray3 = new NativeArray<int>(32, Allocator.Temp, NativeArrayOptions.ClearMemory);
			UnsafeAppendBuffer unsafeAppendBuffer = *this.staticBuffer;
			UnsafeAppendBuffer unsafeAppendBuffer2 = *this.dynamicBuffer;
			UnsafeAppendBuffer unsafeAppendBuffer3 = *this.persistentBuffer;
			unsafeAppendBuffer.Reset();
			unsafeAppendBuffer2.Reset();
			unsafeAppendBuffer3.Reset();
			for (int i = 0; i < this.inputBuffers.Length; i++)
			{
				int num4 = 0;
				int num5 = 0;
				UnsafeAppendBuffer.Reader reader = this.inputBuffers[i].AsReader();
				if (unsafeAppendBuffer.Capacity < unsafeAppendBuffer.Length + reader.Size)
				{
					unsafeAppendBuffer.SetCapacity(math.ceilpow2(unsafeAppendBuffer.Length + reader.Size));
				}
				if (unsafeAppendBuffer2.Capacity < unsafeAppendBuffer2.Length + reader.Size)
				{
					unsafeAppendBuffer2.SetCapacity(math.ceilpow2(unsafeAppendBuffer2.Length + reader.Size));
				}
				if (unsafeAppendBuffer3.Capacity < unsafeAppendBuffer3.Length + reader.Size)
				{
					unsafeAppendBuffer3.SetCapacity(math.ceilpow2(unsafeAppendBuffer3.Length + reader.Size));
				}
				*this.staticBuffer = unsafeAppendBuffer;
				*this.dynamicBuffer = unsafeAppendBuffer2;
				*this.persistentBuffer = unsafeAppendBuffer3;
				while (reader.Offset < reader.Size)
				{
					CommandBuilder.Command command = (CommandBuilder.Command)(*(int*)(reader.Ptr + reader.Offset));
					int num6 = 1 << (int)(command & (CommandBuilder.Command)255);
					int num7 = StreamSplitter.CommandSizes[(int)(command & (CommandBuilder.Command)255)] + (((command & CommandBuilder.Command.PushColorInline) != CommandBuilder.Command.PushColor) ? UnsafeUtility.SizeOf<Color32>() : 0);
					bool flag = (num6 & StreamSplitter.MetaCommands) != 0;
					if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.Text)
					{
						CommandBuilder.TextData textData = *(CommandBuilder.TextData*)(reader.Ptr + reader.Offset + num7 - sizeof(CommandBuilder.TextData));
						num7 += textData.numCharacters * UnsafeUtility.SizeOf<ushort>();
					}
					else if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.Text3D)
					{
						CommandBuilder.TextData3D textData3D = *(CommandBuilder.TextData3D*)(reader.Ptr + reader.Offset + num7 - sizeof(CommandBuilder.TextData3D));
						num7 += textData3D.numCharacters * UnsafeUtility.SizeOf<ushort>();
					}
					if ((num6 & StreamSplitter.DynamicCommands) != 0 && num5 == 0)
					{
						if (!flag)
						{
							num2 = unsafeAppendBuffer2.Length;
						}
						UnsafeUtility.MemCpy((void*)(unsafeAppendBuffer2.Ptr + unsafeAppendBuffer2.Length), (void*)(reader.Ptr + reader.Offset), (long)num7);
						unsafeAppendBuffer2.Length += num7;
					}
					if ((num6 & StreamSplitter.StaticCommands) != 0 && num5 == 0)
					{
						if (!flag)
						{
							num = unsafeAppendBuffer.Length;
						}
						UnsafeUtility.MemCpy((void*)(unsafeAppendBuffer.Ptr + unsafeAppendBuffer.Length), (void*)(reader.Ptr + reader.Offset), (long)num7);
						unsafeAppendBuffer.Length += num7;
					}
					if ((num6 & StreamSplitter.MetaCommands) != 0 || num5 > 0)
					{
						if (num5 > 0 && !flag)
						{
							num3 = unsafeAppendBuffer3.Length;
						}
						UnsafeUtility.MemCpy((void*)(unsafeAppendBuffer3.Ptr + unsafeAppendBuffer3.Length), (void*)(reader.Ptr + reader.Offset), (long)num7);
						unsafeAppendBuffer3.Length += num7;
					}
					if ((num6 & StreamSplitter.PushCommands) != 0)
					{
						nativeArray[num4] = unsafeAppendBuffer.Length - num7;
						nativeArray2[num4] = unsafeAppendBuffer2.Length - num7;
						nativeArray3[num4] = unsafeAppendBuffer3.Length - num7;
						num4++;
						if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.PushPersist)
						{
							num5++;
						}
						if (num4 >= 32)
						{
							return;
						}
					}
					else if ((num6 & StreamSplitter.PopCommands) != 0)
					{
						num4--;
						if (num4 < 0)
						{
							return;
						}
						if (num < nativeArray[num4])
						{
							unsafeAppendBuffer.Length = nativeArray[num4];
						}
						if (num2 < nativeArray2[num4])
						{
							unsafeAppendBuffer2.Length = nativeArray2[num4];
						}
						if (num3 < nativeArray3[num4])
						{
							unsafeAppendBuffer3.Length = nativeArray3[num4];
						}
						if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.PopPersist)
						{
							num5--;
							if (num5 < 0)
							{
								return;
							}
						}
					}
					reader.Offset += num7;
				}
				if (num4 != 0)
				{
					return;
				}
				if (reader.Offset != reader.Size)
				{
					return;
				}
			}
			*this.staticBuffer = unsafeAppendBuffer;
			*this.dynamicBuffer = unsafeAppendBuffer2;
			*this.persistentBuffer = unsafeAppendBuffer3;
		}

		public NativeArray<UnsafeAppendBuffer> inputBuffers;

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeAppendBuffer* staticBuffer;

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeAppendBuffer* dynamicBuffer;

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeAppendBuffer* persistentBuffer;

		internal static readonly int PushCommands = 557069;

		internal static readonly int PopCommands = 1114130;

		internal static readonly int MetaCommands = StreamSplitter.PushCommands | StreamSplitter.PopCommands;

		internal static readonly int DynamicCommands = 2492352 | StreamSplitter.MetaCommands;

		internal static readonly int StaticCommands = 30752 | StreamSplitter.MetaCommands;

		internal static readonly int[] CommandSizes = new int[22];
	}
}
