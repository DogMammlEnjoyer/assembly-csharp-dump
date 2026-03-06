using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class HardwareRayTracingAccelStruct : IRayTracingAccelStruct, IDisposable
	{
		public RayTracingAccelerationStructure accelStruct { get; }

		internal HardwareRayTracingAccelStruct(AccelerationStructureOptions options, Shader hwMaterialShader, ReferenceCounter counter, bool enableCompaction)
		{
			this.m_HWMaterialShader = hwMaterialShader;
			this.LoadRayTracingMaterial();
			this.m_BuildFlags = (RayTracingAccelerationStructureBuildFlags)options.buildFlags;
			this.accelStruct = new RayTracingAccelerationStructure(new RayTracingAccelerationStructure.Settings
			{
				rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything,
				managementMode = RayTracingAccelerationStructure.ManagementMode.Manual,
				enableCompaction = enableCompaction,
				layerMask = 255,
				buildFlagsStaticGeometries = this.m_BuildFlags
			});
			this.m_Counter = counter;
			this.m_Counter.Inc();
		}

		public void Dispose()
		{
			this.m_Counter.Dec();
			RayTracingAccelerationStructure accelStruct = this.accelStruct;
			if (accelStruct != null)
			{
				accelStruct.Dispose();
			}
			if (this.m_RayTracingMaterial != null)
			{
				Utils.Destroy(this.m_RayTracingMaterial);
			}
		}

		public int AddInstance(MeshInstanceDesc meshInstance)
		{
			this.LoadRayTracingMaterial();
			RayTracingMeshInstanceConfig rayTracingMeshInstanceConfig = new RayTracingMeshInstanceConfig(meshInstance.mesh, (uint)meshInstance.subMeshIndex, this.m_RayTracingMaterial);
			rayTracingMeshInstanceConfig.mask = meshInstance.mask;
			rayTracingMeshInstanceConfig.enableTriangleCulling = meshInstance.enableTriangleCulling;
			rayTracingMeshInstanceConfig.frontTriangleCounterClockwise = meshInstance.frontTriangleCounterClockwise;
			int num = this.accelStruct.AddInstance(rayTracingMeshInstanceConfig, meshInstance.localToWorldMatrix, null, meshInstance.instanceID);
			this.m_Meshes.Add(num, meshInstance.mesh);
			return num;
		}

		public void RemoveInstance(int instanceHandle)
		{
			this.m_Meshes.Remove(instanceHandle);
			this.accelStruct.RemoveInstance(instanceHandle);
		}

		public void ClearInstances()
		{
			this.m_Meshes.Clear();
			this.accelStruct.ClearInstances();
		}

		public void UpdateInstanceTransform(int instanceHandle, Matrix4x4 localToWorldMatrix)
		{
			this.accelStruct.UpdateInstanceTransform(instanceHandle, localToWorldMatrix);
		}

		public void UpdateInstanceID(int instanceHandle, uint instanceID)
		{
			this.accelStruct.UpdateInstanceID(instanceHandle, instanceID);
		}

		public void UpdateInstanceMask(int instanceHandle, uint mask)
		{
			this.accelStruct.UpdateInstanceMask(instanceHandle, mask);
		}

		public void Build(CommandBuffer cmd, GraphicsBuffer scratchBuffer)
		{
			RayTracingAccelerationStructure.BuildSettings buildSettings = new RayTracingAccelerationStructure.BuildSettings
			{
				buildFlags = this.m_BuildFlags,
				relativeOrigin = Vector3.zero
			};
			cmd.BuildRayTracingAccelerationStructure(this.accelStruct, buildSettings);
		}

		public ulong GetBuildScratchBufferRequiredSizeInBytes()
		{
			return 0UL;
		}

		private void LoadRayTracingMaterial()
		{
			if (this.m_RayTracingMaterial == null)
			{
				this.m_RayTracingMaterial = new Material(this.m_HWMaterialShader);
			}
		}

		private readonly Shader m_HWMaterialShader;

		private Material m_RayTracingMaterial;

		private readonly RayTracingAccelerationStructureBuildFlags m_BuildFlags;

		private readonly Dictionary<int, Mesh> m_Meshes = new Dictionary<int, Mesh>();

		private readonly ReferenceCounter m_Counter;
	}
}
