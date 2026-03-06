using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.Universal
{
	internal class LightBuffer
	{
		internal GraphicsBuffer graphicsBuffer
		{
			get
			{
				if (this.m_GraphicsBuffer == null)
				{
					this.m_GraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, LightBuffer.kMax, UnsafeUtility.SizeOf<PerLight2D>());
				}
				return this.m_GraphicsBuffer;
			}
		}

		internal NativeArray<int> lightMarkers
		{
			get
			{
				return this.m_Markers;
			}
		}

		internal NativeArray<PerLight2D> nativeBuffer
		{
			get
			{
				return this.m_NativeBuffer;
			}
		}

		internal void Release()
		{
			this.m_GraphicsBuffer.Release();
			this.m_GraphicsBuffer = null;
		}

		internal void Reset()
		{
			UnsafeUtility.MemClear(this.m_Markers.GetUnsafePtr<int>(), (long)(UnsafeUtility.SizeOf<int>() * LightBuffer.kBatchMax));
			UnsafeUtility.MemClear(this.m_NativeBuffer.GetUnsafePtr<PerLight2D>(), (long)(UnsafeUtility.SizeOf<PerLight2D>() * LightBuffer.kMax));
		}

		internal static readonly int kMax = 16384;

		internal static readonly int kCount = 1;

		internal static readonly int kLightMod = 64;

		internal static readonly int kBatchMax = 256;

		private GraphicsBuffer m_GraphicsBuffer;

		private NativeArray<int> m_Markers = new NativeArray<int>(LightBuffer.kBatchMax, Allocator.Persistent, NativeArrayOptions.ClearMemory);

		private NativeArray<PerLight2D> m_NativeBuffer = new NativeArray<PerLight2D>(LightBuffer.kMax, Allocator.Persistent, NativeArrayOptions.ClearMemory);
	}
}
