using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class RayTracingContext : IDisposable
	{
		public RayTracingContext(RayTracingBackend backend, RayTracingResources resources)
		{
			if (!RayTracingContext.IsBackendSupported(backend))
			{
				throw new InvalidOperationException("Unsupported backend: " + backend.ToString());
			}
			this.BackendType = backend;
			if (backend == RayTracingBackend.Hardware)
			{
				this.m_Backend = new HardwareRayTracingBackend(resources);
			}
			else if (backend == RayTracingBackend.Compute)
			{
				this.m_Backend = new ComputeRayTracingBackend(resources);
			}
			this.Resources = resources;
			this.m_DispatchBuffer = RayTracingHelper.CreateDispatchDimensionBuffer();
		}

		public void Dispose()
		{
			if (this.m_AccelStructCounter.value != 0UL)
			{
				Debug.LogError("Memory Leak. Please call .Dispose() on all the IAccelerationStructure resources that have been created with this context before calling RayTracingContext.Dispose()");
			}
			GraphicsBuffer dispatchBuffer = this.m_DispatchBuffer;
			if (dispatchBuffer == null)
			{
				return;
			}
			dispatchBuffer.Release();
		}

		public static bool IsBackendSupported(RayTracingBackend backend)
		{
			if (backend == RayTracingBackend.Hardware)
			{
				return SystemInfo.supportsRayTracing;
			}
			return backend == RayTracingBackend.Compute && SystemInfo.supportsComputeShaders;
		}

		public IRayTracingShader CreateRayTracingShader(Object shader)
		{
			return this.m_Backend.CreateRayTracingShader(shader, "MainRayGenShader", this.m_DispatchBuffer);
		}

		public static uint GetScratchBufferStrideInBytes()
		{
			return 4U;
		}

		public IRayTracingShader CreateRayTracingShader(RayTracingShader rtShader)
		{
			return this.m_Backend.CreateRayTracingShader(rtShader, "MainRayGenShader", this.m_DispatchBuffer);
		}

		public IRayTracingShader CreateRayTracingShader(ComputeShader computeShader)
		{
			return this.m_Backend.CreateRayTracingShader(computeShader, "MainRayGenShader", this.m_DispatchBuffer);
		}

		public IRayTracingAccelStruct CreateAccelerationStructure(AccelerationStructureOptions options)
		{
			return this.m_Backend.CreateAccelerationStructure(options, this.m_AccelStructCounter);
		}

		public ulong GetRequiredTraceScratchBufferSizeInBytes(uint width, uint height, uint depth)
		{
			return this.m_Backend.GetRequiredTraceScratchBufferSizeInBytes(width, height, depth);
		}

		public RayTracingBackend BackendType { get; private set; }

		public RayTracingResources Resources;

		private readonly IRayTracingBackend m_Backend;

		private readonly ReferenceCounter m_AccelStructCounter = new ReferenceCounter();

		private readonly GraphicsBuffer m_DispatchBuffer;
	}
}
