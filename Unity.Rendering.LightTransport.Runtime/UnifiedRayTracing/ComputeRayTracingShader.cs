using System;
using Unity.Mathematics;
using UnityEngine.Rendering.RadeonRays;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal class ComputeRayTracingShader : IRayTracingShader
	{
		internal ComputeRayTracingShader(ComputeShader shader, string dispatchFuncName, GraphicsBuffer dispatchBuffer)
		{
			this.m_Shader = shader;
			this.m_KernelIndex = this.m_Shader.FindKernel(dispatchFuncName);
			this.m_ComputeIndirectDispatchDimsKernelIndex = this.m_Shader.FindKernel("ComputeIndirectDispatchDims");
			this.m_Shader.GetKernelThreadGroupSizes(this.m_KernelIndex, out this.m_ThreadGroupSizes.x, out this.m_ThreadGroupSizes.y, out this.m_ThreadGroupSizes.z);
			this.m_DispatchBuffer = dispatchBuffer;
		}

		public uint3 GetThreadGroupSizes()
		{
			return this.m_ThreadGroupSizes;
		}

		public void SetAccelerationStructure(CommandBuffer cmd, string name, IRayTracingAccelStruct accelStruct)
		{
			(accelStruct as ComputeRayTracingAccelStruct).Bind(cmd, name, this);
		}

		public void SetIntParam(CommandBuffer cmd, int nameID, int val)
		{
			cmd.SetComputeIntParam(this.m_Shader, nameID, val);
		}

		public void SetFloatParam(CommandBuffer cmd, int nameID, float val)
		{
			cmd.SetComputeFloatParam(this.m_Shader, nameID, val);
		}

		public void SetVectorParam(CommandBuffer cmd, int nameID, Vector4 val)
		{
			cmd.SetComputeVectorParam(this.m_Shader, nameID, val);
		}

		public void SetMatrixParam(CommandBuffer cmd, int nameID, Matrix4x4 val)
		{
			cmd.SetComputeMatrixParam(this.m_Shader, nameID, val);
		}

		public void SetTextureParam(CommandBuffer cmd, int nameID, RenderTargetIdentifier rt)
		{
			cmd.SetComputeTextureParam(this.m_Shader, this.m_KernelIndex, nameID, rt);
		}

		public void SetBufferParam(CommandBuffer cmd, int nameID, GraphicsBuffer buffer)
		{
			cmd.SetComputeBufferParam(this.m_Shader, this.m_KernelIndex, nameID, buffer);
		}

		public void SetBufferParam(CommandBuffer cmd, int nameID, ComputeBuffer buffer)
		{
			cmd.SetComputeBufferParam(this.m_Shader, this.m_KernelIndex, nameID, buffer);
		}

		public void Dispatch(CommandBuffer cmd, GraphicsBuffer scratchBuffer, uint width, uint height, uint depth)
		{
			this.GetTraceScratchBufferRequiredSizeInBytes(width, height, depth);
			cmd.SetComputeBufferParam(this.m_Shader, this.m_KernelIndex, SID.g_stack, scratchBuffer);
			cmd.SetBufferData(this.m_DispatchBuffer, new uint[]
			{
				width,
				height,
				depth
			});
			this.SetBufferParam(cmd, SID.g_dispatch_dimensions, this.m_DispatchBuffer);
			uint threadGroupsX = (uint)GraphicsHelpers.DivUp((int)width, this.m_ThreadGroupSizes.x);
			uint threadGroupsY = (uint)GraphicsHelpers.DivUp((int)height, this.m_ThreadGroupSizes.y);
			uint threadGroupsZ = (uint)GraphicsHelpers.DivUp((int)depth, this.m_ThreadGroupSizes.z);
			cmd.DispatchCompute(this.m_Shader, this.m_KernelIndex, (int)threadGroupsX, (int)threadGroupsY, (int)threadGroupsZ);
		}

		public void Dispatch(CommandBuffer cmd, GraphicsBuffer scratchBuffer, GraphicsBuffer argsBuffer)
		{
			this.SetIndirectDispatchDimensions(cmd, argsBuffer);
			this.DispatchIndirect(cmd, scratchBuffer, argsBuffer);
		}

		internal void SetIndirectDispatchDimensions(CommandBuffer cmd, GraphicsBuffer argsBuffer)
		{
			cmd.SetComputeBufferParam(this.m_Shader, this.m_ComputeIndirectDispatchDimsKernelIndex, SID.g_dispatch_dimensions, argsBuffer);
			cmd.SetComputeBufferParam(this.m_Shader, this.m_ComputeIndirectDispatchDimsKernelIndex, SID.g_dispatch_dims_in_workgroups, this.m_DispatchBuffer);
			cmd.DispatchCompute(this.m_Shader, this.m_ComputeIndirectDispatchDimsKernelIndex, 1, 1, 1);
		}

		internal void DispatchIndirect(CommandBuffer cmd, GraphicsBuffer scratchBuffer, GraphicsBuffer argsBuffer)
		{
			cmd.SetComputeBufferParam(this.m_Shader, this.m_KernelIndex, SID.g_stack, scratchBuffer);
			cmd.SetComputeBufferParam(this.m_Shader, this.m_KernelIndex, SID.g_dispatch_dimensions, argsBuffer);
			cmd.DispatchCompute(this.m_Shader, this.m_KernelIndex, this.m_DispatchBuffer, 0U);
		}

		public ulong GetTraceScratchBufferRequiredSizeInBytes(uint width, uint height, uint depth)
		{
			return RadeonRaysAPI.GetTraceMemoryRequirements(width * height * depth) * 4UL;
		}

		private readonly ComputeShader m_Shader;

		private readonly int m_KernelIndex;

		private readonly int m_ComputeIndirectDispatchDimsKernelIndex;

		private uint3 m_ThreadGroupSizes;

		private readonly GraphicsBuffer m_DispatchBuffer;
	}
}
