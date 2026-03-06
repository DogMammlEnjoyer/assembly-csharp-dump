using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalCreateDrawCallSystem
	{
		public float maxDrawDistance
		{
			get
			{
				return this.m_MaxDrawDistance;
			}
			set
			{
				this.m_MaxDrawDistance = value;
			}
		}

		public DecalCreateDrawCallSystem(DecalEntityManager entityManager, float maxDrawDistance)
		{
			this.m_EntityManager = entityManager;
			this.m_Sampler = new ProfilingSampler("DecalCreateDrawCallSystem.Execute");
			this.m_MaxDrawDistance = maxDrawDistance;
		}

		public void Execute()
		{
			using (new ProfilingScope(this.m_Sampler))
			{
				for (int i = 0; i < this.m_EntityManager.chunkCount; i++)
				{
					this.Execute(this.m_EntityManager.cachedChunks[i], this.m_EntityManager.culledChunks[i], this.m_EntityManager.drawCallChunks[i], this.m_EntityManager.cachedChunks[i].count);
				}
			}
		}

		private void Execute(DecalCachedChunk cachedChunk, DecalCulledChunk culledChunk, DecalDrawCallChunk drawCallChunk, int count)
		{
			if (count == 0)
			{
				return;
			}
			JobHandle currentJobHandle = new DecalCreateDrawCallSystem.DrawCallJob
			{
				decalToWorlds = cachedChunk.decalToWorlds,
				normalToWorlds = cachedChunk.normalToWorlds,
				sizeOffsets = cachedChunk.sizeOffsets,
				drawDistances = cachedChunk.drawDistances,
				angleFades = cachedChunk.angleFades,
				uvScaleBiases = cachedChunk.uvScaleBias,
				layerMasks = cachedChunk.layerMasks,
				sceneLayerMasks = cachedChunk.sceneLayerMasks,
				fadeFactors = cachedChunk.fadeFactors,
				boundingSpheres = cachedChunk.boundingSpheres,
				renderingLayerMasks = cachedChunk.renderingLayerMasks,
				cameraPosition = culledChunk.cameraPosition,
				sceneCullingMask = culledChunk.sceneCullingMask,
				cullingMask = culledChunk.cullingMask,
				visibleDecalIndices = culledChunk.visibleDecalIndices,
				visibleDecalCount = culledChunk.visibleDecalCount,
				maxDrawDistance = this.m_MaxDrawDistance,
				decalToWorldsDraw = drawCallChunk.decalToWorlds,
				normalToDecalsDraw = drawCallChunk.normalToDecals,
				renderingLayerMasksDraw = drawCallChunk.renderingLayerMasks,
				subCalls = drawCallChunk.subCalls,
				subCallCount = drawCallChunk.subCallCounts
			}.Schedule(cachedChunk.currentJobHandle);
			drawCallChunk.currentJobHandle = currentJobHandle;
			cachedChunk.currentJobHandle = currentJobHandle;
		}

		private DecalEntityManager m_EntityManager;

		private ProfilingSampler m_Sampler;

		private float m_MaxDrawDistance;

		[BurstCompile]
		private struct DrawCallJob : IJob
		{
			public void Execute()
			{
				int value = 0;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this.visibleDecalCount; i++)
				{
					int index = this.visibleDecalIndices[i];
					int num3 = 1 << this.layerMasks[index];
					if ((this.cullingMask & num3) != 0)
					{
						BoundingSphere boundingSphere = this.boundingSpheres[index];
						float2 @float = this.drawDistances[index];
						float magnitude = (this.cameraPosition - boundingSphere.position).magnitude;
						float num4 = math.min(@float.x, this.maxDrawDistance) + boundingSphere.radius;
						if (magnitude <= num4)
						{
							this.decalToWorldsDraw[num] = this.decalToWorlds[index];
							float num5 = this.fadeFactors[index];
							float2 float2 = this.angleFades[index];
							float4 float3 = this.uvScaleBiases[index];
							float4x4 value2 = this.normalToWorlds[index];
							float num6 = num5 * math.clamp((num4 - magnitude) / (num4 * (1f - @float.y)), 0f, 1f);
							value2.c0.w = float3.x;
							value2.c1.w = float3.y;
							value2.c2.w = float3.z;
							value2.c3 = new float4(num6 * 1f, float2.x, float2.y, float3.w);
							this.normalToDecalsDraw[num] = value2;
							this.renderingLayerMasksDraw[num] = this.renderingLayerMasks[index];
							num++;
							if ((long)(num - num2) >= (long)((ulong)DecalDrawSystem.MaxBatchSize))
							{
								this.subCalls[value++] = new DecalSubDrawCall
								{
									start = num2,
									end = num
								};
								num2 = num;
							}
						}
					}
				}
				if (num - num2 != 0)
				{
					this.subCalls[value++] = new DecalSubDrawCall
					{
						start = num2,
						end = num
					};
				}
				this.subCallCount[0] = value;
			}

			[ReadOnly]
			public NativeArray<float4x4> decalToWorlds;

			[ReadOnly]
			public NativeArray<float4x4> normalToWorlds;

			[ReadOnly]
			public NativeArray<float4x4> sizeOffsets;

			[ReadOnly]
			public NativeArray<float2> drawDistances;

			[ReadOnly]
			public NativeArray<float2> angleFades;

			[ReadOnly]
			public NativeArray<float4> uvScaleBiases;

			[ReadOnly]
			public NativeArray<int> layerMasks;

			[ReadOnly]
			public NativeArray<ulong> sceneLayerMasks;

			[ReadOnly]
			public NativeArray<float> fadeFactors;

			[ReadOnly]
			public NativeArray<BoundingSphere> boundingSpheres;

			[ReadOnly]
			public NativeArray<uint> renderingLayerMasks;

			public Vector3 cameraPosition;

			public ulong sceneCullingMask;

			public int cullingMask;

			[ReadOnly]
			public NativeArray<int> visibleDecalIndices;

			public int visibleDecalCount;

			public float maxDrawDistance;

			[WriteOnly]
			public NativeArray<float4x4> decalToWorldsDraw;

			[WriteOnly]
			public NativeArray<float4x4> normalToDecalsDraw;

			[WriteOnly]
			public NativeArray<float> renderingLayerMasksDraw;

			[WriteOnly]
			public NativeArray<DecalSubDrawCall> subCalls;

			[WriteOnly]
			public NativeArray<int> subCallCount;
		}
	}
}
