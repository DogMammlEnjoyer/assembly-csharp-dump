using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Jobs;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalUpdateCachedSystem
	{
		public DecalUpdateCachedSystem(DecalEntityManager entityManager)
		{
			this.m_EntityManager = entityManager;
			this.m_Sampler = new ProfilingSampler("DecalUpdateCachedSystem.Execute");
			this.m_SamplerJob = new ProfilingSampler("DecalUpdateCachedSystem.ExecuteJob");
		}

		public void Execute()
		{
			using (new ProfilingScope(this.m_Sampler))
			{
				for (int i = 0; i < this.m_EntityManager.chunkCount; i++)
				{
					this.Execute(this.m_EntityManager.entityChunks[i], this.m_EntityManager.cachedChunks[i], this.m_EntityManager.entityChunks[i].count);
				}
			}
		}

		private void Execute(DecalEntityChunk entityChunk, DecalCachedChunk cachedChunk, int count)
		{
			if (count == 0)
			{
				return;
			}
			cachedChunk.currentJobHandle.Complete();
			Material material = entityChunk.material;
			if (material.HasProperty("_DrawOrder"))
			{
				cachedChunk.drawOrder = material.GetInt("_DrawOrder");
			}
			if (!cachedChunk.isCreated)
			{
				int passIndexDBuffer = material.FindPass("DBufferProjector");
				cachedChunk.passIndexDBuffer = passIndexDBuffer;
				int passIndexEmissive = material.FindPass("DecalProjectorForwardEmissive");
				cachedChunk.passIndexEmissive = passIndexEmissive;
				int passIndexScreenSpace = material.FindPass("DecalScreenSpaceProjector");
				cachedChunk.passIndexScreenSpace = passIndexScreenSpace;
				int passIndexGBuffer = material.FindPass("DecalGBufferProjector");
				cachedChunk.passIndexGBuffer = passIndexGBuffer;
				cachedChunk.isCreated = true;
			}
			using (new ProfilingScope(this.m_SamplerJob))
			{
				JobHandle currentJobHandle = new DecalUpdateCachedSystem.UpdateTransformsJob
				{
					positions = cachedChunk.positions,
					rotations = cachedChunk.rotation,
					scales = cachedChunk.scales,
					dirty = cachedChunk.dirty,
					scaleModes = cachedChunk.scaleModes,
					sizeOffsets = cachedChunk.sizeOffsets,
					decalToWorlds = cachedChunk.decalToWorlds,
					normalToWorlds = cachedChunk.normalToWorlds,
					boundingSpheres = cachedChunk.boundingSpheres,
					minDistance = float.Epsilon
				}.Schedule(entityChunk.transformAccessArray, default(JobHandle));
				cachedChunk.currentJobHandle = currentJobHandle;
			}
		}

		private DecalEntityManager m_EntityManager;

		private ProfilingSampler m_Sampler;

		private ProfilingSampler m_SamplerJob;

		[BurstCompile]
		public struct UpdateTransformsJob : IJobParallelForTransform
		{
			private float DistanceBetweenQuaternions(quaternion a, quaternion b)
			{
				return math.distancesq(a.value, b.value);
			}

			public void Execute(int index, TransformAccess transform)
			{
				bool flag = math.distancesq(transform.position, this.positions[index]) > this.minDistance;
				if (flag)
				{
					this.positions[index] = transform.position;
				}
				bool flag2 = this.DistanceBetweenQuaternions(transform.rotation, this.rotations[index]) > this.minDistance;
				if (flag2)
				{
					this.rotations[index] = transform.rotation;
				}
				bool flag3 = math.distancesq(transform.localScale, this.scales[index]) > this.minDistance;
				if (flag3)
				{
					this.scales[index] = transform.localScale;
				}
				if (!flag && !flag2 && !flag3 && !this.dirty[index])
				{
					return;
				}
				float4x4 float4x;
				if (this.scaleModes[index] == DecalScaleMode.InheritFromHierarchy)
				{
					float4x = transform.localToWorldMatrix;
					float4x = math.mul(float4x, new float4x4(DecalUpdateCachedSystem.UpdateTransformsJob.k_MinusYtoZRotation, float3.zero));
				}
				else
				{
					quaternion rotation = math.mul(transform.rotation, DecalUpdateCachedSystem.UpdateTransformsJob.k_MinusYtoZRotation);
					float4x = float4x4.TRS(this.positions[index], rotation, new float3(1f, 1f, 1f));
				}
				float4x4 float4x2 = float4x;
				float4 c = float4x2.c1;
				float4x2.c1 = float4x2.c2;
				float4x2.c2 = c;
				this.normalToWorlds[index] = float4x2;
				float4x4 b = this.sizeOffsets[index];
				float4x4 float4x3 = math.mul(float4x, b);
				this.decalToWorlds[index] = float4x3;
				this.boundingSpheres[index] = this.GetDecalProjectBoundingSphere(float4x3);
				this.dirty[index] = false;
			}

			private BoundingSphere GetDecalProjectBoundingSphere(Matrix4x4 decalToWorld)
			{
				float4 @float = new float4(-0.5f, -0.5f, -0.5f, 1f);
				float4 float2 = new float4(0.5f, 0.5f, 0.5f, 1f);
				@float = math.mul(decalToWorld, @float);
				float2 = math.mul(decalToWorld, float2);
				float3 xyz = ((float2 + @float) / 2f).xyz;
				float radius = math.length(float2 - @float) / 2f;
				return new BoundingSphere
				{
					position = xyz,
					radius = radius
				};
			}

			private static readonly quaternion k_MinusYtoZRotation = quaternion.EulerXYZ(-1.5707964f, 0f, 0f);

			public NativeArray<float3> positions;

			public NativeArray<quaternion> rotations;

			public NativeArray<float3> scales;

			public NativeArray<bool> dirty;

			[ReadOnly]
			public NativeArray<DecalScaleMode> scaleModes;

			[ReadOnly]
			public NativeArray<float4x4> sizeOffsets;

			[WriteOnly]
			public NativeArray<float4x4> decalToWorlds;

			[WriteOnly]
			public NativeArray<float4x4> normalToWorlds;

			[WriteOnly]
			public NativeArray<BoundingSphere> boundingSpheres;

			public float minDistance;
		}
	}
}
