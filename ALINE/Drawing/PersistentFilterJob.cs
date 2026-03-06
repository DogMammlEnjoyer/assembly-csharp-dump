using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Drawing
{
	[BurstCompile]
	internal struct PersistentFilterJob : IJob
	{
		public unsafe void Execute()
		{
			NativeArray<bool> nativeArray = new NativeArray<bool>(32, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int> nativeArray2 = new NativeArray<int>(32, Allocator.Temp, NativeArrayOptions.ClearMemory);
			UnsafeAppendBuffer unsafeAppendBuffer = *this.buffer;
			long num = 0L;
			long num2 = 0L;
			bool flag = false;
			int num3 = 0;
			long num4 = -1L;
			while (num2 < (long)unsafeAppendBuffer.Length)
			{
				CommandBuilder.Command command = (CommandBuilder.Command)(*(int*)(unsafeAppendBuffer.Ptr + num2));
				int num5 = 1 << (int)(command & (CommandBuilder.Command)255);
				bool flag2 = (num5 & StreamSplitter.MetaCommands) != 0;
				int num6 = StreamSplitter.CommandSizes[(int)(command & (CommandBuilder.Command)255)] + (((command & CommandBuilder.Command.PushColorInline) != CommandBuilder.Command.PushColor) ? UnsafeUtility.SizeOf<Color32>() : 0);
				if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.Text)
				{
					CommandBuilder.TextData textData = *(CommandBuilder.TextData*)(unsafeAppendBuffer.Ptr + num2 + num6 - sizeof(CommandBuilder.TextData));
					num6 += textData.numCharacters * UnsafeUtility.SizeOf<ushort>();
				}
				else if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.Text3D)
				{
					CommandBuilder.TextData3D textData3D = *(CommandBuilder.TextData3D*)(unsafeAppendBuffer.Ptr + num2 + num6 - sizeof(CommandBuilder.TextData3D));
					num6 += textData3D.numCharacters * UnsafeUtility.SizeOf<ushort>();
				}
				if (flag || flag2)
				{
					if (!flag2)
					{
						num4 = num;
					}
					if (num != num2)
					{
						UnsafeUtility.MemMove((void*)(unsafeAppendBuffer.Ptr + num), (void*)(unsafeAppendBuffer.Ptr + num2), (long)num6);
					}
					num += (long)num6;
				}
				if ((num5 & StreamSplitter.PushCommands) != 0)
				{
					if ((command & (CommandBuilder.Command)255) == CommandBuilder.Command.PushPersist)
					{
						CommandBuilder.PersistData persistData = *(CommandBuilder.PersistData*)(unsafeAppendBuffer.Ptr + num2 + num6 - sizeof(CommandBuilder.PersistData));
						flag = (this.time <= persistData.endTime);
					}
					nativeArray2[num3] = (int)(num - (long)num6);
					nativeArray[num3] = flag;
					num3++;
					if (num3 >= 32)
					{
						this.buffer->Length = 0;
						return;
					}
				}
				else if ((num5 & StreamSplitter.PopCommands) != 0)
				{
					num3--;
					if (num3 < 0)
					{
						this.buffer->Length = 0;
						return;
					}
					if ((int)num4 < nativeArray2[num3])
					{
						num = (long)nativeArray2[num3];
					}
					flag = nativeArray[num3];
				}
				num2 += (long)num6;
			}
			unsafeAppendBuffer.Length = (int)num;
			if (num3 != 0)
			{
				this.buffer->Length = 0;
				return;
			}
			*this.buffer = unsafeAppendBuffer;
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe UnsafeAppendBuffer* buffer;

		public float time;
	}
}
