using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal class HardwareRayTracingBackend : IRayTracingBackend
	{
		public HardwareRayTracingBackend(RayTracingResources resources)
		{
			this.m_Resources = resources;
		}

		public IRayTracingShader CreateRayTracingShader(Object shader, string kernelName, GraphicsBuffer dispatchBuffer)
		{
			return new HardwareRayTracingShader((RayTracingShader)shader, kernelName, dispatchBuffer);
		}

		public IRayTracingAccelStruct CreateAccelerationStructure(AccelerationStructureOptions options, ReferenceCounter counter)
		{
			return new HardwareRayTracingAccelStruct(options, this.m_Resources.hardwareRayTracingMaterial, counter, options.enableCompaction);
		}

		public ulong GetRequiredTraceScratchBufferSizeInBytes(uint width, uint height, uint depth)
		{
			return 0UL;
		}

		private readonly RayTracingResources m_Resources;
	}
}
