using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.Universal
{
	internal class ShaderData : IDisposable
	{
		private ShaderData()
		{
		}

		internal static ShaderData instance
		{
			get
			{
				if (ShaderData.m_Instance == null)
				{
					ShaderData.m_Instance = new ShaderData();
				}
				return ShaderData.m_Instance;
			}
		}

		public void Dispose()
		{
			this.DisposeBuffer(ref this.m_LightDataBuffer);
			this.DisposeBuffer(ref this.m_LightIndicesBuffer);
			this.DisposeBuffer(ref this.m_AdditionalLightShadowParamsStructuredBuffer);
			this.DisposeBuffer(ref this.m_AdditionalLightShadowSliceMatricesStructuredBuffer);
		}

		internal ComputeBuffer GetLightDataBuffer(int size)
		{
			return this.GetOrUpdateBuffer<ShaderInput.LightData>(ref this.m_LightDataBuffer, size);
		}

		internal ComputeBuffer GetLightIndicesBuffer(int size)
		{
			return this.GetOrUpdateBuffer<int>(ref this.m_LightIndicesBuffer, size);
		}

		internal ComputeBuffer GetAdditionalLightShadowParamsStructuredBuffer(int size)
		{
			return this.GetOrUpdateBuffer<Vector4>(ref this.m_AdditionalLightShadowParamsStructuredBuffer, size);
		}

		internal ComputeBuffer GetAdditionalLightShadowSliceMatricesStructuredBuffer(int size)
		{
			return this.GetOrUpdateBuffer<Matrix4x4>(ref this.m_AdditionalLightShadowSliceMatricesStructuredBuffer, size);
		}

		private ComputeBuffer GetOrUpdateBuffer<T>(ref ComputeBuffer buffer, int size) where T : struct
		{
			if (buffer == null)
			{
				buffer = new ComputeBuffer(size, Marshal.SizeOf<T>());
			}
			else if (size > buffer.count)
			{
				buffer.Dispose();
				buffer = new ComputeBuffer(size, Marshal.SizeOf<T>());
			}
			return buffer;
		}

		private void DisposeBuffer(ref ComputeBuffer buffer)
		{
			if (buffer != null)
			{
				buffer.Dispose();
				buffer = null;
			}
		}

		private static ShaderData m_Instance;

		private ComputeBuffer m_LightDataBuffer;

		private ComputeBuffer m_LightIndicesBuffer;

		private ComputeBuffer m_AdditionalLightShadowParamsStructuredBuffer;

		private ComputeBuffer m_AdditionalLightShadowSliceMatricesStructuredBuffer;
	}
}
