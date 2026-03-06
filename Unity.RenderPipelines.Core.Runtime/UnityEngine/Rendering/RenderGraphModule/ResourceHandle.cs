using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	internal readonly struct ResourceHandle : IEquatable<ResourceHandle>
	{
		public int index
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (int)(this.m_Value & 65535U);
			}
		}

		public int iType
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (int)this.type;
			}
		}

		public int version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Version;
			}
		}

		public RenderGraphResourceType type
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Type;
			}
		}

		internal ResourceHandle(int value, RenderGraphResourceType type, bool shared)
		{
			this.m_Value = (uint)((value & 65535) | (int)(shared ? ResourceHandle.s_SharedResourceValidBit : ResourceHandle.s_CurrentValidBit));
			this.m_Type = type;
			this.m_Version = -1;
		}

		internal ResourceHandle(in ResourceHandle h, int version)
		{
			this.m_Value = h.m_Value;
			this.m_Type = h.type;
			this.m_Version = version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsValid()
		{
			uint num = this.m_Value & 4294901760U;
			return num != 0U && (num == ResourceHandle.s_CurrentValidBit || num == ResourceHandle.s_SharedResourceValidBit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsNull()
		{
			return this.index == 0;
		}

		public static void NewFrame(int executionIndex)
		{
			uint num = ResourceHandle.s_CurrentValidBit;
			ResourceHandle.s_CurrentValidBit = (uint)((uint)(executionIndex >> 16 ^ (executionIndex & 65535) * 58546883) << 16);
			if (ResourceHandle.s_CurrentValidBit == 0U || ResourceHandle.s_CurrentValidBit == ResourceHandle.s_SharedResourceValidBit)
			{
				uint num2 = 1U;
				while (num == num2 << 16)
				{
					num2 += 1U;
				}
				ResourceHandle.s_CurrentValidBit = num2 << 16;
			}
		}

		public bool IsVersioned
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Version >= 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ResourceHandle hdl)
		{
			return hdl.m_Value == this.m_Value && hdl.m_Version == this.m_Version && hdl.type == this.type;
		}

		private const uint kValidityMask = 4294901760U;

		private const uint kIndexMask = 65535U;

		private readonly uint m_Value;

		private readonly int m_Version;

		private readonly RenderGraphResourceType m_Type;

		private static uint s_CurrentValidBit = 65536U;

		private static uint s_SharedResourceValidBit = 2147418112U;
	}
}
