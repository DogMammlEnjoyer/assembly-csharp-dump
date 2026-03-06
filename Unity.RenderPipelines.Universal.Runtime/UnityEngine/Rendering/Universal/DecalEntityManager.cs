using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalEntityManager : IDisposable
	{
		public Material errorMaterial
		{
			get
			{
				if (this.m_ErrorMaterial == null)
				{
					this.m_ErrorMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/InternalErrorShader"));
				}
				return this.m_ErrorMaterial;
			}
		}

		public Mesh decalProjectorMesh
		{
			get
			{
				if (this.m_DecalProjectorMesh == null)
				{
					this.m_DecalProjectorMesh = CoreUtils.CreateCubeMesh(new Vector4(-0.5f, -0.5f, -0.5f, 1f), new Vector4(0.5f, 0.5f, 0.5f, 1f));
				}
				return this.m_DecalProjectorMesh;
			}
		}

		public DecalEntityManager()
		{
			this.m_AddDecalSampler = new ProfilingSampler("DecalEntityManager.CreateDecalEntity");
			this.m_ResizeChunks = new ProfilingSampler("DecalEntityManager.ResizeChunks");
			this.m_SortChunks = new ProfilingSampler("DecalEntityManager.SortChunks");
		}

		public bool IsValid(DecalEntity decalEntity)
		{
			return this.m_DecalEntityIndexer.IsValid(decalEntity);
		}

		public DecalEntity CreateDecalEntity(DecalProjector decalProjector)
		{
			Material material = decalProjector.material;
			if (material == null)
			{
				material = this.errorMaterial;
			}
			DecalEntity result;
			using (new ProfilingScope(this.m_AddDecalSampler))
			{
				int num = this.CreateChunkIndex(material);
				int count = this.entityChunks[num].count;
				DecalEntity decalEntity = this.m_DecalEntityIndexer.CreateDecalEntity(count, num);
				DecalEntityChunk decalEntityChunk = this.entityChunks[num];
				DecalCachedChunk decalCachedChunk = this.cachedChunks[num];
				DecalCulledChunk decalCulledChunk = this.culledChunks[num];
				DecalDrawCallChunk decalDrawCallChunk = this.drawCallChunks[num];
				if (this.entityChunks[num].capacity == this.entityChunks[num].count)
				{
					using (new ProfilingScope(this.m_ResizeChunks))
					{
						int num2 = this.entityChunks[num].capacity + this.entityChunks[num].capacity;
						num2 = math.max(8, num2);
						decalEntityChunk.SetCapacity(num2);
						decalCachedChunk.SetCapacity(num2);
						decalCulledChunk.SetCapacity(num2);
						decalDrawCallChunk.SetCapacity(num2);
					}
				}
				decalEntityChunk.Push();
				decalCachedChunk.Push();
				decalCulledChunk.Push();
				decalDrawCallChunk.Push();
				decalEntityChunk.decalProjectors[count] = decalProjector;
				decalEntityChunk.decalEntities[count] = decalEntity;
				decalEntityChunk.transformAccessArray.Add(decalProjector.transform);
				this.UpdateDecalEntityData(decalEntity, decalProjector);
				result = decalEntity;
			}
			return result;
		}

		private int CreateChunkIndex(Material material)
		{
			int result;
			if (!this.m_MaterialToChunkIndex.TryGetValue(material, out result))
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetMatrixArray("_NormalToWorld", new Matrix4x4[DecalDrawSystem.MaxBatchSize]);
				materialPropertyBlock.SetFloatArray("_DecalLayerMaskFromDecal", new float[DecalDrawSystem.MaxBatchSize]);
				this.entityChunks.Add(new DecalEntityChunk
				{
					material = material
				});
				this.cachedChunks.Add(new DecalCachedChunk
				{
					propertyBlock = materialPropertyBlock
				});
				this.culledChunks.Add(new DecalCulledChunk());
				this.drawCallChunks.Add(new DecalDrawCallChunk
				{
					subCallCounts = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory)
				});
				this.m_CombinedChunks.Add(default(DecalEntityManager.CombinedChunks));
				this.m_CombinedChunkRemmap.Add(0);
				this.m_MaterialToChunkIndex.Add(material, this.chunkCount);
				int num = this.chunkCount;
				this.chunkCount = num + 1;
				return num;
			}
			return result;
		}

		public void UpdateAllDecalEntitiesData()
		{
			foreach (DecalEntityChunk decalEntityChunk in this.entityChunks)
			{
				for (int i = 0; i < decalEntityChunk.count; i++)
				{
					DecalProjector decalProjector = decalEntityChunk.decalProjectors[i];
					if (!(decalProjector == null))
					{
						DecalEntity decalEntity = decalEntityChunk.decalEntities[i];
						if (this.IsValid(decalEntity))
						{
							this.UpdateDecalEntityData(decalEntity, decalProjector);
						}
					}
				}
			}
		}

		public void UpdateDecalEntityData(DecalEntity decalEntity, DecalProjector decalProjector)
		{
			DecalEntityIndexer.DecalEntityItem item = this.m_DecalEntityIndexer.GetItem(decalEntity);
			int chunkIndex = item.chunkIndex;
			int arrayIndex = item.arrayIndex;
			DecalCachedChunk decalCachedChunk = this.cachedChunks[chunkIndex];
			decalCachedChunk.sizeOffsets[arrayIndex] = Matrix4x4.Translate(decalProjector.decalOffset) * Matrix4x4.Scale(decalProjector.decalSize);
			float drawDistance = decalProjector.drawDistance;
			float fadeScale = decalProjector.fadeScale;
			float startAngleFade = decalProjector.startAngleFade;
			float endAngleFade = decalProjector.endAngleFade;
			Vector4 uvScaleBias = decalProjector.uvScaleBias;
			int layer = decalProjector.gameObject.layer;
			ulong sceneCullingMask = decalProjector.gameObject.sceneCullingMask;
			float fadeFactor = decalProjector.fadeFactor;
			decalCachedChunk.drawDistances[arrayIndex] = new Vector2(drawDistance, fadeScale);
			if (startAngleFade == 180f)
			{
				decalCachedChunk.angleFades[arrayIndex] = new Vector2(0f, 0f);
			}
			else
			{
				float num = startAngleFade / 180f;
				float num2 = endAngleFade / 180f;
				float num3 = Mathf.Max(0.0001f, num2 - num);
				decalCachedChunk.angleFades[arrayIndex] = new Vector2(1f - (0.25f - num) / num3, -0.25f / num3);
			}
			decalCachedChunk.uvScaleBias[arrayIndex] = uvScaleBias;
			decalCachedChunk.layerMasks[arrayIndex] = layer;
			decalCachedChunk.sceneLayerMasks[arrayIndex] = sceneCullingMask;
			decalCachedChunk.fadeFactors[arrayIndex] = fadeFactor;
			decalCachedChunk.scaleModes[arrayIndex] = decalProjector.scaleMode;
			decalCachedChunk.renderingLayerMasks[arrayIndex] = RenderingLayerUtils.ToValidRenderingLayers(decalProjector.renderingLayerMask);
			decalCachedChunk.positions[arrayIndex] = decalProjector.transform.position;
			decalCachedChunk.rotation[arrayIndex] = decalProjector.transform.rotation;
			decalCachedChunk.scales[arrayIndex] = decalProjector.transform.lossyScale;
			decalCachedChunk.dirty[arrayIndex] = true;
		}

		public void DestroyDecalEntity(DecalEntity decalEntity)
		{
			if (!this.m_DecalEntityIndexer.IsValid(decalEntity))
			{
				return;
			}
			DecalEntityIndexer.DecalEntityItem item = this.m_DecalEntityIndexer.GetItem(decalEntity);
			this.m_DecalEntityIndexer.DestroyDecalEntity(decalEntity);
			int chunkIndex = item.chunkIndex;
			int arrayIndex = item.arrayIndex;
			DecalEntityChunk decalEntityChunk = this.entityChunks[chunkIndex];
			DecalCachedChunk decalCachedChunk = this.cachedChunks[chunkIndex];
			DecalCulledChunk decalCulledChunk = this.culledChunks[chunkIndex];
			DecalChunk decalChunk = this.drawCallChunks[chunkIndex];
			int num = decalEntityChunk.count - 1;
			if (arrayIndex != num)
			{
				this.m_DecalEntityIndexer.UpdateIndex(decalEntityChunk.decalEntities[num], arrayIndex);
			}
			decalEntityChunk.RemoveAtSwapBack(arrayIndex);
			decalCachedChunk.RemoveAtSwapBack(arrayIndex);
			decalCulledChunk.RemoveAtSwapBack(arrayIndex);
			decalChunk.RemoveAtSwapBack(arrayIndex);
		}

		public void Update()
		{
			using (new ProfilingScope(this.m_SortChunks))
			{
				for (int i = 0; i < this.chunkCount; i++)
				{
					if (this.entityChunks[i].material == null)
					{
						this.entityChunks[i].material = this.errorMaterial;
					}
				}
				for (int j = 0; j < this.chunkCount; j++)
				{
					this.m_CombinedChunks[j] = new DecalEntityManager.CombinedChunks
					{
						entityChunk = this.entityChunks[j],
						cachedChunk = this.cachedChunks[j],
						culledChunk = this.culledChunks[j],
						drawCallChunk = this.drawCallChunks[j],
						previousChunkIndex = j,
						valid = (this.entityChunks[j].count != 0)
					};
				}
				this.m_CombinedChunks.Sort(delegate(DecalEntityManager.CombinedChunks a, DecalEntityManager.CombinedChunks b)
				{
					if (a.valid && !b.valid)
					{
						return -1;
					}
					if (!a.valid && b.valid)
					{
						return 1;
					}
					if (a.cachedChunk.drawOrder < b.cachedChunk.drawOrder)
					{
						return -1;
					}
					if (a.cachedChunk.drawOrder > b.cachedChunk.drawOrder)
					{
						return 1;
					}
					return a.entityChunk.material.GetHashCode().CompareTo(b.entityChunk.material.GetHashCode());
				});
				bool flag = false;
				for (int k = 0; k < this.chunkCount; k++)
				{
					if (this.m_CombinedChunks[k].previousChunkIndex != k || !this.m_CombinedChunks[k].valid)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					int num = 0;
					this.m_MaterialToChunkIndex.Clear();
					for (int l = 0; l < this.chunkCount; l++)
					{
						DecalEntityManager.CombinedChunks combinedChunks = this.m_CombinedChunks[l];
						if (!this.m_CombinedChunks[l].valid)
						{
							combinedChunks.entityChunk.currentJobHandle.Complete();
							combinedChunks.cachedChunk.currentJobHandle.Complete();
							combinedChunks.culledChunk.currentJobHandle.Complete();
							combinedChunks.drawCallChunk.currentJobHandle.Complete();
							combinedChunks.entityChunk.Dispose();
							combinedChunks.cachedChunk.Dispose();
							combinedChunks.culledChunk.Dispose();
							combinedChunks.drawCallChunk.Dispose();
						}
						else
						{
							this.entityChunks[l] = combinedChunks.entityChunk;
							this.cachedChunks[l] = combinedChunks.cachedChunk;
							this.culledChunks[l] = combinedChunks.culledChunk;
							this.drawCallChunks[l] = combinedChunks.drawCallChunk;
							if (!this.m_MaterialToChunkIndex.ContainsKey(this.entityChunks[l].material))
							{
								this.m_MaterialToChunkIndex.Add(this.entityChunks[l].material, l);
							}
							this.m_CombinedChunkRemmap[combinedChunks.previousChunkIndex] = l;
							num++;
						}
					}
					if (this.chunkCount > num)
					{
						this.entityChunks.RemoveRange(num, this.chunkCount - num);
						this.cachedChunks.RemoveRange(num, this.chunkCount - num);
						this.culledChunks.RemoveRange(num, this.chunkCount - num);
						this.drawCallChunks.RemoveRange(num, this.chunkCount - num);
						this.m_CombinedChunks.RemoveRange(num, this.chunkCount - num);
						this.chunkCount = num;
					}
					this.m_DecalEntityIndexer.RemapChunkIndices(this.m_CombinedChunkRemmap);
				}
			}
		}

		public void Dispose()
		{
			CoreUtils.Destroy(this.m_ErrorMaterial);
			CoreUtils.Destroy(this.m_DecalProjectorMesh);
			foreach (DecalEntityChunk decalEntityChunk in this.entityChunks)
			{
				decalEntityChunk.currentJobHandle.Complete();
			}
			foreach (DecalCachedChunk decalCachedChunk in this.cachedChunks)
			{
				decalCachedChunk.currentJobHandle.Complete();
			}
			foreach (DecalCulledChunk decalCulledChunk in this.culledChunks)
			{
				decalCulledChunk.currentJobHandle.Complete();
			}
			foreach (DecalDrawCallChunk decalDrawCallChunk in this.drawCallChunks)
			{
				decalDrawCallChunk.currentJobHandle.Complete();
			}
			foreach (DecalEntityChunk decalEntityChunk2 in this.entityChunks)
			{
				decalEntityChunk2.Dispose();
			}
			foreach (DecalCachedChunk decalCachedChunk2 in this.cachedChunks)
			{
				decalCachedChunk2.Dispose();
			}
			foreach (DecalCulledChunk decalCulledChunk2 in this.culledChunks)
			{
				decalCulledChunk2.Dispose();
			}
			foreach (DecalDrawCallChunk decalDrawCallChunk2 in this.drawCallChunks)
			{
				decalDrawCallChunk2.Dispose();
			}
			this.m_DecalEntityIndexer.Clear();
			this.m_MaterialToChunkIndex.Clear();
			this.entityChunks.Clear();
			this.cachedChunks.Clear();
			this.culledChunks.Clear();
			this.drawCallChunks.Clear();
			this.m_CombinedChunks.Clear();
			this.chunkCount = 0;
		}

		public List<DecalEntityChunk> entityChunks = new List<DecalEntityChunk>();

		public List<DecalCachedChunk> cachedChunks = new List<DecalCachedChunk>();

		public List<DecalCulledChunk> culledChunks = new List<DecalCulledChunk>();

		public List<DecalDrawCallChunk> drawCallChunks = new List<DecalDrawCallChunk>();

		public int chunkCount;

		private ProfilingSampler m_AddDecalSampler;

		private ProfilingSampler m_ResizeChunks;

		private ProfilingSampler m_SortChunks;

		private DecalEntityIndexer m_DecalEntityIndexer = new DecalEntityIndexer();

		private Dictionary<Material, int> m_MaterialToChunkIndex = new Dictionary<Material, int>();

		private List<DecalEntityManager.CombinedChunks> m_CombinedChunks = new List<DecalEntityManager.CombinedChunks>();

		private List<int> m_CombinedChunkRemmap = new List<int>();

		private Material m_ErrorMaterial;

		private Mesh m_DecalProjectorMesh;

		private struct CombinedChunks
		{
			public DecalEntityChunk entityChunk;

			public DecalCachedChunk cachedChunk;

			public DecalCulledChunk culledChunk;

			public DecalDrawCallChunk drawCallChunk;

			public int previousChunkIndex;

			public bool valid;
		}
	}
}
