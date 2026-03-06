using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal class HardwareRayTracingShader : IRayTracingShader
	{
		internal HardwareRayTracingShader(RayTracingShader shader, string dispatchFuncName, GraphicsBuffer unused)
		{
			this.m_Shader = shader;
			this.m_ShaderDispatchFuncName = dispatchFuncName;
		}

		public uint3 GetThreadGroupSizes()
		{
			return new uint3(1U, 1U, 1U);
		}

		public void SetAccelerationStructure(CommandBuffer cmd, string name, IRayTracingAccelStruct accelStruct)
		{
			cmd.SetRayTracingShaderPass(this.m_Shader, "RayTracing");
			HardwareRayTracingAccelStruct hardwareRayTracingAccelStruct = accelStruct as HardwareRayTracingAccelStruct;
			cmd.SetRayTracingAccelerationStructure(this.m_Shader, Shader.PropertyToID(name + "accelStruct"), hardwareRayTracingAccelStruct.accelStruct);
		}

		public void SetIntParam(CommandBuffer cmd, int nameID, int val)
		{
			cmd.SetRayTracingIntParam(this.m_Shader, nameID, val);
		}

		public void SetFloatParam(CommandBuffer cmd, int nameID, float val)
		{
			cmd.SetRayTracingFloatParam(this.m_Shader, nameID, val);
		}

		public void SetVectorParam(CommandBuffer cmd, int nameID, Vector4 val)
		{
			cmd.SetRayTracingVectorParam(this.m_Shader, nameID, val);
		}

		public void SetMatrixParam(CommandBuffer cmd, int nameID, Matrix4x4 val)
		{
			cmd.SetRayTracingMatrixParam(this.m_Shader, nameID, val);
		}

		public void SetTextureParam(CommandBuffer cmd, int nameID, RenderTargetIdentifier rt)
		{
			cmd.SetRayTracingTextureParam(this.m_Shader, nameID, rt);
		}

		public void SetBufferParam(CommandBuffer cmd, int nameID, GraphicsBuffer buffer)
		{
			cmd.SetRayTracingBufferParam(this.m_Shader, nameID, buffer);
		}

		public void SetBufferParam(CommandBuffer cmd, int nameID, ComputeBuffer buffer)
		{
			cmd.SetRayTracingBufferParam(this.m_Shader, nameID, buffer);
		}

		public void Dispatch(CommandBuffer cmd, GraphicsBuffer scratchBuffer, uint width, uint height, uint depth)
		{
			cmd.DispatchRays(this.m_Shader, this.m_ShaderDispatchFuncName, width, height, depth, null);
		}

		public void Dispatch(CommandBuffer cmd, GraphicsBuffer scratchBuffer, GraphicsBuffer argsBuffer)
		{
			cmd.DispatchRays(this.m_Shader, this.m_ShaderDispatchFuncName, argsBuffer, RayTracingHelper.k_DimensionByteOffset, null);
		}

		public ulong GetTraceScratchBufferRequiredSizeInBytes(uint width, uint height, uint depth)
		{
			return 0UL;
		}

		private readonly RayTracingShader m_Shader;

		private readonly string m_ShaderDispatchFuncName;
	}
}
