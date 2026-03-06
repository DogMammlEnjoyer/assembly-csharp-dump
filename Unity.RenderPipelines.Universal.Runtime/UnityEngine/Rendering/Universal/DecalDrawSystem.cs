using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	internal abstract class DecalDrawSystem
	{
		public Material overrideMaterial { get; set; }

		public DecalDrawSystem(string sampler, DecalEntityManager entityManager)
		{
			this.m_EntityManager = entityManager;
			this.m_WorldToDecals = new Matrix4x4[DecalDrawSystem.MaxBatchSize];
			this.m_NormalToDecals = new Matrix4x4[DecalDrawSystem.MaxBatchSize];
			this.m_DecalLayerMasks = new float[DecalDrawSystem.MaxBatchSize];
			this.m_Sampler = new ProfilingSampler(sampler);
		}

		public void Execute(CommandBuffer cmd)
		{
			this.Execute(CommandBufferHelpers.GetRasterCommandBuffer(cmd));
		}

		internal void Execute(RasterCommandBuffer cmd)
		{
			using (new ProfilingScope(cmd, this.m_Sampler))
			{
				for (int i = 0; i < this.m_EntityManager.chunkCount; i++)
				{
					this.Execute(cmd, this.m_EntityManager.entityChunks[i], this.m_EntityManager.cachedChunks[i], this.m_EntityManager.drawCallChunks[i], this.m_EntityManager.entityChunks[i].count);
				}
			}
		}

		protected virtual Material GetMaterial(DecalEntityChunk decalEntityChunk)
		{
			return decalEntityChunk.material;
		}

		protected abstract int GetPassIndex(DecalCachedChunk decalCachedChunk);

		private void Execute(RasterCommandBuffer cmd, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk, int count)
		{
			decalCachedChunk.currentJobHandle.Complete();
			decalDrawCallChunk.currentJobHandle.Complete();
			Material material = this.GetMaterial(decalEntityChunk);
			int passIndex = this.GetPassIndex(decalCachedChunk);
			if (count == 0 || passIndex == -1 || material == null)
			{
				return;
			}
			if (SystemInfo.supportsInstancing && material.enableInstancing)
			{
				this.DrawInstanced(cmd, decalEntityChunk, decalCachedChunk, decalDrawCallChunk, passIndex);
				return;
			}
			this.Draw(cmd, decalEntityChunk, decalCachedChunk, decalDrawCallChunk, passIndex);
		}

		private void Draw(RasterCommandBuffer cmd, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk, int passIndex)
		{
			Mesh decalProjectorMesh = this.m_EntityManager.decalProjectorMesh;
			Material material = this.GetMaterial(decalEntityChunk);
			decalCachedChunk.propertyBlock.SetVector("unity_LightData", new Vector4(1f, 1f, 1f, 0f));
			int subCallCount = decalDrawCallChunk.subCallCount;
			for (int i = 0; i < subCallCount; i++)
			{
				DecalSubDrawCall decalSubDrawCall = decalDrawCallChunk.subCalls[i];
				for (int j = decalSubDrawCall.start; j < decalSubDrawCall.end; j++)
				{
					decalCachedChunk.propertyBlock.SetMatrix("_NormalToWorld", decalDrawCallChunk.normalToDecals[j]);
					decalCachedChunk.propertyBlock.SetFloat("_DecalLayerMaskFromDecal", decalDrawCallChunk.renderingLayerMasks[j]);
					cmd.DrawMesh(decalProjectorMesh, decalDrawCallChunk.decalToWorlds[j], material, 0, passIndex, decalCachedChunk.propertyBlock);
				}
			}
		}

		private void DrawInstanced(RasterCommandBuffer cmd, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk, int passIndex)
		{
			Mesh decalProjectorMesh = this.m_EntityManager.decalProjectorMesh;
			Material material = this.GetMaterial(decalEntityChunk);
			decalCachedChunk.propertyBlock.SetVector("unity_LightData", new Vector4(1f, 1f, 1f, 0f));
			int subCallCount = decalDrawCallChunk.subCallCount;
			for (int i = 0; i < subCallCount; i++)
			{
				DecalSubDrawCall decalSubDrawCall = decalDrawCallChunk.subCalls[i];
				NativeArray<Matrix4x4>.Copy(decalDrawCallChunk.decalToWorlds.Reinterpret<Matrix4x4>(), decalSubDrawCall.start, this.m_WorldToDecals, 0, decalSubDrawCall.count);
				NativeArray<Matrix4x4>.Copy(decalDrawCallChunk.normalToDecals.Reinterpret<Matrix4x4>(), decalSubDrawCall.start, this.m_NormalToDecals, 0, decalSubDrawCall.count);
				NativeArray<float>.Copy(decalDrawCallChunk.renderingLayerMasks.Reinterpret<float>(), decalSubDrawCall.start, this.m_DecalLayerMasks, 0, decalSubDrawCall.count);
				decalCachedChunk.propertyBlock.SetMatrixArray("_NormalToWorld", this.m_NormalToDecals);
				decalCachedChunk.propertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", this.m_DecalLayerMasks);
				cmd.DrawMeshInstanced(decalProjectorMesh, 0, material, passIndex, this.m_WorldToDecals, decalSubDrawCall.end - decalSubDrawCall.start, decalCachedChunk.propertyBlock);
			}
		}

		public void Execute(in CameraData cameraData)
		{
			using (new ProfilingScope(this.m_Sampler))
			{
				for (int i = 0; i < this.m_EntityManager.chunkCount; i++)
				{
					this.Execute(cameraData, this.m_EntityManager.entityChunks[i], this.m_EntityManager.cachedChunks[i], this.m_EntityManager.drawCallChunks[i], this.m_EntityManager.entityChunks[i].count);
				}
			}
		}

		private void Execute(in CameraData cameraData, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk, int count)
		{
			decalCachedChunk.currentJobHandle.Complete();
			decalDrawCallChunk.currentJobHandle.Complete();
			Material material = this.GetMaterial(decalEntityChunk);
			int passIndex = this.GetPassIndex(decalCachedChunk);
			if (count == 0 || passIndex == -1 || material == null)
			{
				return;
			}
			if (SystemInfo.supportsInstancing && material.enableInstancing)
			{
				this.DrawInstanced(cameraData, decalEntityChunk, decalCachedChunk, decalDrawCallChunk);
				return;
			}
			this.Draw(cameraData, decalEntityChunk, decalCachedChunk, decalDrawCallChunk);
		}

		private unsafe void Draw(in CameraData cameraData, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk)
		{
			Mesh decalProjectorMesh = this.m_EntityManager.decalProjectorMesh;
			Material material = this.GetMaterial(decalEntityChunk);
			int subCallCount = decalDrawCallChunk.subCallCount;
			for (int i = 0; i < subCallCount; i++)
			{
				DecalSubDrawCall decalSubDrawCall = decalDrawCallChunk.subCalls[i];
				for (int j = decalSubDrawCall.start; j < decalSubDrawCall.end; j++)
				{
					decalCachedChunk.propertyBlock.SetMatrix("_NormalToWorld", decalDrawCallChunk.normalToDecals[j]);
					decalCachedChunk.propertyBlock.SetFloat("_DecalLayerMaskFromDecal", decalDrawCallChunk.renderingLayerMasks[j]);
					Mesh mesh = decalProjectorMesh;
					Matrix4x4 matrix = decalDrawCallChunk.decalToWorlds[j];
					Material material2 = material;
					int layer = decalCachedChunk.layerMasks[j];
					CameraData cameraData2 = cameraData;
					Graphics.DrawMesh(mesh, matrix, material2, layer, *cameraData2.camera, 0, decalCachedChunk.propertyBlock);
				}
			}
		}

		private unsafe void DrawInstanced(in CameraData cameraData, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk)
		{
			Mesh decalProjectorMesh = this.m_EntityManager.decalProjectorMesh;
			Material material = this.GetMaterial(decalEntityChunk);
			decalCachedChunk.propertyBlock.SetVector("unity_LightData", new Vector4(1f, 1f, 1f, 0f));
			int subCallCount = decalDrawCallChunk.subCallCount;
			for (int i = 0; i < subCallCount; i++)
			{
				DecalSubDrawCall decalSubDrawCall = decalDrawCallChunk.subCalls[i];
				NativeArray<Matrix4x4>.Copy(decalDrawCallChunk.decalToWorlds.Reinterpret<Matrix4x4>(), decalSubDrawCall.start, this.m_WorldToDecals, 0, decalSubDrawCall.count);
				NativeArray<Matrix4x4>.Copy(decalDrawCallChunk.normalToDecals.Reinterpret<Matrix4x4>(), decalSubDrawCall.start, this.m_NormalToDecals, 0, decalSubDrawCall.count);
				NativeArray<float>.Copy(decalDrawCallChunk.renderingLayerMasks.Reinterpret<float>(), decalSubDrawCall.start, this.m_DecalLayerMasks, 0, decalSubDrawCall.count);
				decalCachedChunk.propertyBlock.SetMatrixArray("_NormalToWorld", this.m_NormalToDecals);
				decalCachedChunk.propertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", this.m_DecalLayerMasks);
				Mesh mesh = decalProjectorMesh;
				int submeshIndex = 0;
				Material material2 = material;
				Matrix4x4[] worldToDecals = this.m_WorldToDecals;
				int count = decalSubDrawCall.count;
				MaterialPropertyBlock propertyBlock = decalCachedChunk.propertyBlock;
				ShadowCastingMode castShadows = ShadowCastingMode.On;
				bool receiveShadows = true;
				int layer = 0;
				CameraData cameraData2 = cameraData;
				Graphics.DrawMeshInstanced(mesh, submeshIndex, material2, worldToDecals, count, propertyBlock, castShadows, receiveShadows, layer, *cameraData2.camera);
			}
		}

		internal static readonly uint MaxBatchSize = 250U;

		protected DecalEntityManager m_EntityManager;

		private Matrix4x4[] m_WorldToDecals;

		private Matrix4x4[] m_NormalToDecals;

		private float[] m_DecalLayerMasks;

		private ProfilingSampler m_Sampler;
	}
}
