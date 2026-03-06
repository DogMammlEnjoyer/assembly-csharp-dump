using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	public class ConstantBuffer<CBType> : ConstantBufferBase where CBType : struct
	{
		public ConstantBuffer()
		{
			this.m_GPUConstantBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<CBType>(), ComputeBufferType.Constant);
		}

		public void UpdateData(CommandBuffer cmd, in CBType data)
		{
			this.m_Data[0] = data;
			cmd.SetBufferData(this.m_GPUConstantBuffer, this.m_Data);
		}

		public void UpdateData(in CBType data)
		{
			this.m_Data[0] = data;
			this.m_GPUConstantBuffer.SetData(this.m_Data);
		}

		public void SetGlobal(CommandBuffer cmd, int shaderId)
		{
			this.m_GlobalBindings.Add(shaderId);
			cmd.SetGlobalConstantBuffer(this.m_GPUConstantBuffer, shaderId, 0, this.m_GPUConstantBuffer.stride);
		}

		public void SetGlobal(int shaderId)
		{
			this.m_GlobalBindings.Add(shaderId);
			Shader.SetGlobalConstantBuffer(shaderId, this.m_GPUConstantBuffer, 0, this.m_GPUConstantBuffer.stride);
		}

		public void Set(CommandBuffer cmd, ComputeShader cs, int shaderId)
		{
			cmd.SetComputeConstantBufferParam(cs, shaderId, this.m_GPUConstantBuffer, 0, this.m_GPUConstantBuffer.stride);
		}

		public void Set(ComputeShader cs, int shaderId)
		{
			cs.SetConstantBuffer(shaderId, this.m_GPUConstantBuffer, 0, this.m_GPUConstantBuffer.stride);
		}

		public void Set(Material mat, int shaderId)
		{
			mat.SetConstantBuffer(shaderId, this.m_GPUConstantBuffer, 0, this.m_GPUConstantBuffer.stride);
		}

		public void Set(MaterialPropertyBlock mpb, int shaderId)
		{
			mpb.SetConstantBuffer(shaderId, this.m_GPUConstantBuffer, 0, this.m_GPUConstantBuffer.stride);
		}

		public void PushGlobal(CommandBuffer cmd, in CBType data, int shaderId)
		{
			this.UpdateData(cmd, data);
			this.SetGlobal(cmd, shaderId);
		}

		public void PushGlobal(in CBType data, int shaderId)
		{
			this.UpdateData(data);
			this.SetGlobal(shaderId);
		}

		public override void Release()
		{
			foreach (int nameID in this.m_GlobalBindings)
			{
				Shader.SetGlobalConstantBuffer(nameID, null, 0, 0);
			}
			this.m_GlobalBindings.Clear();
			CoreUtils.SafeRelease(this.m_GPUConstantBuffer);
		}

		private HashSet<int> m_GlobalBindings = new HashSet<int>();

		private CBType[] m_Data = new CBType[1];

		private ComputeBuffer m_GPUConstantBuffer;
	}
}
