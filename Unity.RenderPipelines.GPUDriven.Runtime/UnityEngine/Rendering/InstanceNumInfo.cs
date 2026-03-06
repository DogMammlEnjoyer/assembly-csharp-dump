using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering
{
	internal struct InstanceNumInfo
	{
		public unsafe void InitDefault()
		{
			for (int i = 0; i < 2; i++)
			{
				*(ref this.InstanceNums.FixedElementField + (IntPtr)i * 4) = 0;
			}
		}

		public unsafe InstanceNumInfo(InstanceType type, int instanceNum)
		{
			this.InitDefault();
			*(ref this.InstanceNums.FixedElementField + (IntPtr)type * 4) = instanceNum;
		}

		public unsafe InstanceNumInfo(int meshRendererNum = 0, int speedTreeNum = 0)
		{
			this.InitDefault();
			this.InstanceNums.FixedElementField = meshRendererNum;
			*(ref this.InstanceNums.FixedElementField + 4) = speedTreeNum;
		}

		public unsafe int GetInstanceNum(InstanceType type)
		{
			return *(ref this.InstanceNums.FixedElementField + (IntPtr)type * 4);
		}

		public int GetInstanceNumIncludingChildren(InstanceType type)
		{
			int num = this.GetInstanceNum(type);
			foreach (InstanceType type2 in InstanceTypeInfo.GetChildTypes(type))
			{
				num += this.GetInstanceNumIncludingChildren(type2);
			}
			return num;
		}

		public unsafe int GetTotalInstanceNum()
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				num += *(ref this.InstanceNums.FixedElementField + (IntPtr)i * 4);
			}
			return num;
		}

		[FixedBuffer(typeof(int), 2)]
		public InstanceNumInfo.<InstanceNums>e__FixedBuffer InstanceNums;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <InstanceNums>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
