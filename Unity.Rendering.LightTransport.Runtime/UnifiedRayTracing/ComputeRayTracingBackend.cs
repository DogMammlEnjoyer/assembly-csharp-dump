using System;
using UnityEngine.Rendering.RadeonRays;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal class ComputeRayTracingBackend : IRayTracingBackend
	{
		public ComputeRayTracingBackend(RayTracingResources resources)
		{
			this.m_Resources = resources;
		}

		public IRayTracingShader CreateRayTracingShader(Object shader, string kernelName, GraphicsBuffer dispatchBuffer)
		{
			return new ComputeRayTracingShader((ComputeShader)shader, kernelName, dispatchBuffer);
		}

		public IRayTracingAccelStruct CreateAccelerationStructure(AccelerationStructureOptions options, ReferenceCounter counter)
		{
			return new ComputeRayTracingAccelStruct(options, this.m_Resources, counter, 67108864);
		}

		public ulong GetRequiredTraceScratchBufferSizeInBytes(uint width, uint height, uint depth)
		{
			return RadeonRaysAPI.GetTraceMemoryRequirements(width * height * depth) * (ulong)RayTracingContext.GetScratchBufferStrideInBytes();
		}

		private readonly RayTracingResources m_Resources;
	}
}
