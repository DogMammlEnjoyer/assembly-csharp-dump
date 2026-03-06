using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal sealed class AccelStructAdapter : IDisposable
	{
		internal AccelStructInstances Instances
		{
			get
			{
				return this._instances;
			}
		}

		public AccelStructAdapter(IRayTracingAccelStruct accelStruct, GeometryPool geometryPool)
		{
			this._objectHandleToInstances = new Dictionary<int, AccelStructAdapter.InstanceIDs[]>();
			base..ctor();
			this._accelStruct = accelStruct;
			this._instances = new AccelStructInstances(geometryPool);
		}

		public AccelStructAdapter(IRayTracingAccelStruct accelStruct, RayTracingResources resources)
		{
			GeometryPoolDesc geometryPoolDesc = GeometryPoolDesc.NewDefault();
			this..ctor(accelStruct, new GeometryPool(ref geometryPoolDesc, resources.geometryPoolKernels, resources.copyBuffer));
		}

		public IRayTracingAccelStruct GetAccelerationStructure()
		{
			return this._accelStruct;
		}

		public GeometryPool GeometryPool
		{
			get
			{
				return this._instances.geometryPool;
			}
		}

		public void Bind(CommandBuffer cmd, string propertyName, IRayTracingShader shader)
		{
			shader.SetAccelerationStructure(cmd, propertyName, this._accelStruct);
			this._instances.Bind(cmd, shader);
		}

		public void Dispose()
		{
			AccelStructInstances instances = this._instances;
			if (instances != null)
			{
				instances.Dispose();
			}
			this._instances = null;
			IRayTracingAccelStruct accelStruct = this._accelStruct;
			if (accelStruct != null)
			{
				accelStruct.Dispose();
			}
			this._accelStruct = null;
			this._objectHandleToInstances.Clear();
		}

		public unsafe void AddInstance(int objectHandle, Component meshRendererOrTerrain, Span<uint> perSubMeshMask, Span<uint> perSubMeshMaterialIDs, uint renderingLayerMask)
		{
			Terrain terrain = meshRendererOrTerrain as Terrain;
			if (terrain != null)
			{
				TerrainDesc terrainDesc;
				terrainDesc.terrain = terrain;
				terrainDesc.localToWorldMatrix = terrain.transform.localToWorldMatrix;
				terrainDesc.mask = *perSubMeshMask[0];
				terrainDesc.renderingLayerMask = renderingLayerMask;
				terrainDesc.materialID = *perSubMeshMaterialIDs[0];
				terrainDesc.enableTriangleCulling = true;
				terrainDesc.frontTriangleCounterClockwise = false;
				this.AddInstance(objectHandle, terrainDesc);
				return;
			}
			MeshRenderer meshRenderer = (MeshRenderer)meshRendererOrTerrain;
			Mesh sharedMesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
			this.AddInstance(objectHandle, sharedMesh, meshRenderer.transform.localToWorldMatrix, perSubMeshMask, perSubMeshMaterialIDs, renderingLayerMask);
		}

		public unsafe void AddInstance(int objectHandle, Mesh mesh, Matrix4x4 localToWorldMatrix, Span<uint> perSubMeshMask, Span<uint> perSubMeshMaterialIDs, uint renderingLayerMask)
		{
			int subMeshCount = mesh.subMeshCount;
			AccelStructAdapter.InstanceIDs[] array = new AccelStructAdapter.InstanceIDs[subMeshCount];
			for (int i = 0; i < subMeshCount; i++)
			{
				MeshInstanceDesc meshInstance = new MeshInstanceDesc(mesh, i)
				{
					localToWorldMatrix = localToWorldMatrix,
					mask = *perSubMeshMask[i]
				};
				array[i].InstanceID = this._instances.AddInstance(meshInstance, *perSubMeshMaterialIDs[i], renderingLayerMask);
				meshInstance.instanceID = (uint)array[i].InstanceID;
				array[i].AccelStructID = this._accelStruct.AddInstance(meshInstance);
			}
			this._objectHandleToInstances.Add(objectHandle, array);
		}

		private void AddInstance(int objectHandle, TerrainDesc terrainDesc)
		{
			List<AccelStructAdapter.InstanceIDs> list = new List<AccelStructAdapter.InstanceIDs>();
			this.AddHeightmap(terrainDesc, ref list);
			this.AddTrees(terrainDesc, ref list);
			this._objectHandleToInstances.Add(objectHandle, list.ToArray());
		}

		private void AddHeightmap(TerrainDesc terrainDesc, ref List<AccelStructAdapter.InstanceIDs> instanceHandles)
		{
			Mesh mesh = TerrainToMesh.Convert(terrainDesc.terrain);
			MeshInstanceDesc instanceDesc = new MeshInstanceDesc(mesh, 0);
			instanceDesc.localToWorldMatrix = terrainDesc.localToWorldMatrix;
			instanceDesc.mask = terrainDesc.mask;
			instanceDesc.enableTriangleCulling = terrainDesc.enableTriangleCulling;
			instanceDesc.frontTriangleCounterClockwise = terrainDesc.frontTriangleCounterClockwise;
			instanceHandles.Add(this.AddInstance(instanceDesc, terrainDesc.materialID, terrainDesc.renderingLayerMask));
		}

		private void AddTrees(TerrainDesc terrainDesc, ref List<AccelStructAdapter.InstanceIDs> instanceHandles)
		{
			TerrainData terrainData = terrainDesc.terrain.terrainData;
			float4x4 float4x = terrainDesc.localToWorldMatrix;
			float3 rhs = new float3((float)terrainData.heightmapResolution, 1f, (float)terrainData.heightmapResolution) * terrainData.heightmapScale;
			float3 lhs = new float3(float4x[3].x, float4x[3].y, float4x[3].z);
			foreach (TreeInstance treeInstance in terrainData.treeInstances)
			{
				Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(lhs + new float3(treeInstance.position) * rhs, Quaternion.AngleAxis(treeInstance.rotation, Vector3.up), new Vector3(treeInstance.widthScale, treeInstance.heightScale, treeInstance.widthScale));
				GameObject prefab = terrainData.treePrototypes[treeInstance.prototypeIndex].prefab;
				GameObject gameObject = prefab.gameObject;
				LODGroup lodgroup;
				if (prefab.TryGetComponent<LODGroup>(out lodgroup))
				{
					LOD[] lods = lodgroup.GetLODs();
					if (lods.Length != 0 && lods[0].renderers.Length != 0)
					{
						gameObject = (lods[0].renderers[0] as MeshRenderer).gameObject;
					}
				}
				MeshFilter meshFilter;
				if (gameObject.TryGetComponent<MeshFilter>(out meshFilter))
				{
					Mesh sharedMesh = meshFilter.sharedMesh;
					for (int j = 0; j < sharedMesh.subMeshCount; j++)
					{
						MeshInstanceDesc instanceDesc = new MeshInstanceDesc(sharedMesh, j);
						instanceDesc.localToWorldMatrix = localToWorldMatrix;
						instanceDesc.mask = terrainDesc.mask;
						instanceDesc.enableTriangleCulling = terrainDesc.enableTriangleCulling;
						instanceDesc.frontTriangleCounterClockwise = terrainDesc.frontTriangleCounterClockwise;
						instanceHandles.Add(this.AddInstance(instanceDesc, terrainDesc.materialID, 1U << prefab.gameObject.layer));
					}
				}
			}
		}

		private AccelStructAdapter.InstanceIDs AddInstance(MeshInstanceDesc instanceDesc, uint materialID, uint renderingLayerMask)
		{
			AccelStructAdapter.InstanceIDs instanceIDs = new AccelStructAdapter.InstanceIDs
			{
				InstanceID = this._instances.AddInstance(instanceDesc, materialID, renderingLayerMask)
			};
			instanceDesc.instanceID = (uint)instanceIDs.InstanceID;
			instanceIDs.AccelStructID = this._accelStruct.AddInstance(instanceDesc);
			return instanceIDs;
		}

		public void RemoveInstance(int objectHandle)
		{
			AccelStructAdapter.InstanceIDs[] array;
			this._objectHandleToInstances.TryGetValue(objectHandle, out array);
			foreach (AccelStructAdapter.InstanceIDs instanceIDs in array)
			{
				this._instances.RemoveInstance(instanceIDs.InstanceID);
				this._accelStruct.RemoveInstance(instanceIDs.AccelStructID);
			}
			this._objectHandleToInstances.Remove(objectHandle);
		}

		public void UpdateInstanceTransform(int objectHandle, Matrix4x4 localToWorldMatrix)
		{
			AccelStructAdapter.InstanceIDs[] array;
			this._objectHandleToInstances.TryGetValue(objectHandle, out array);
			foreach (AccelStructAdapter.InstanceIDs instanceIDs in array)
			{
				this._instances.UpdateInstanceTransform(instanceIDs.InstanceID, localToWorldMatrix);
				this._accelStruct.UpdateInstanceTransform(instanceIDs.AccelStructID, localToWorldMatrix);
			}
		}

		public unsafe void UpdateInstanceMaterialIDs(int objectHandle, Span<uint> perSubMeshMaterialIDs)
		{
			AccelStructAdapter.InstanceIDs[] array;
			this._objectHandleToInstances.TryGetValue(objectHandle, out array);
			int num = 0;
			foreach (AccelStructAdapter.InstanceIDs instanceIDs in array)
			{
				this._instances.UpdateInstanceMaterialID(instanceIDs.InstanceID, *perSubMeshMaterialIDs[num++]);
			}
		}

		public unsafe void UpdateInstanceMask(int objectHandle, Span<uint> perSubMeshMask)
		{
			AccelStructAdapter.InstanceIDs[] array;
			this._objectHandleToInstances.TryGetValue(objectHandle, out array);
			int num = 0;
			foreach (AccelStructAdapter.InstanceIDs instanceIDs in array)
			{
				this._instances.UpdateInstanceMask(instanceIDs.InstanceID, *perSubMeshMask[num]);
				this._accelStruct.UpdateInstanceMask(instanceIDs.AccelStructID, *perSubMeshMask[num]);
				num++;
			}
		}

		public void UpdateInstanceMask(int objectHandle, uint mask)
		{
			AccelStructAdapter.InstanceIDs[] array;
			this._objectHandleToInstances.TryGetValue(objectHandle, out array);
			uint[] array2 = new uint[array.Length];
			Array.Fill<uint>(array2, mask);
			int num = 0;
			foreach (AccelStructAdapter.InstanceIDs instanceIDs in array)
			{
				this._instances.UpdateInstanceMask(instanceIDs.InstanceID, array2[num]);
				this._accelStruct.UpdateInstanceMask(instanceIDs.AccelStructID, array2[num]);
				num++;
			}
		}

		public void Build(CommandBuffer cmd, ref GraphicsBuffer scratchBuffer)
		{
			RayTracingHelper.ResizeScratchBufferForBuild(this._accelStruct, ref scratchBuffer);
			this._accelStruct.Build(cmd, scratchBuffer);
		}

		public void NextFrame()
		{
			this._instances.NextFrame();
		}

		public bool GetInstanceIDs(int rendererID, out int[] instanceIDs)
		{
			AccelStructAdapter.InstanceIDs[] array;
			if (!this._objectHandleToInstances.TryGetValue(rendererID, out array))
			{
				instanceIDs = null;
				return false;
			}
			instanceIDs = Array.ConvertAll<AccelStructAdapter.InstanceIDs, int>(array, (AccelStructAdapter.InstanceIDs item) => item.InstanceID);
			return true;
		}

		private IRayTracingAccelStruct _accelStruct;

		private AccelStructInstances _instances;

		private readonly Dictionary<int, AccelStructAdapter.InstanceIDs[]> _objectHandleToInstances;

		private struct InstanceIDs
		{
			public int InstanceID;

			public int AccelStructID;
		}
	}
}
