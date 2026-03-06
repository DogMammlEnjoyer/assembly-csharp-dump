using System;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal class LightBatch
	{
		private static int batchLightMod
		{
			get
			{
				return LightBuffer.kLightMod;
			}
		}

		private static float batchRunningIndex
		{
			get
			{
				return (float)(LightBatch.sBatchIndexCounter++ % LightBuffer.kLightMod) / (float)LightBuffer.kLightMod;
			}
		}

		public static bool isBatchingSupported
		{
			get
			{
				return false;
			}
		}

		internal NativeArray<PerLight2D> nativeBuffer
		{
			get
			{
				if (this.lightBuffer[this.activeCount] == null)
				{
					this.lightBuffer[this.activeCount] = new LightBuffer();
				}
				return this.lightBuffer[this.activeCount].nativeBuffer;
			}
		}

		internal GraphicsBuffer graphicsBuffer
		{
			get
			{
				if (this.lightBuffer[this.activeCount] == null)
				{
					this.lightBuffer[this.activeCount] = new LightBuffer();
				}
				return this.lightBuffer[this.activeCount].graphicsBuffer;
			}
		}

		internal NativeArray<int> lightMarker
		{
			get
			{
				if (this.lightBuffer[this.activeCount] == null)
				{
					this.lightBuffer[this.activeCount] = new LightBuffer();
				}
				return this.lightBuffer[this.activeCount].lightMarkers;
			}
		}

		internal PerLight2D GetLight(int index)
		{
			return this.nativeBuffer[index];
		}

		internal static int batchSlotIndex
		{
			get
			{
				return (int)(LightBatch.batchRunningIndex * (float)LightBuffer.kLightMod);
			}
		}

		internal void SetLight(int index, PerLight2D light)
		{
			this.nativeBuffer[index] = light;
		}

		internal static float GetBatchColor()
		{
			return (float)LightBatch.batchSlotIndex / (float)LightBatch.batchLightMod;
		}

		internal static int GetBatchSlotIndex(float channelColor)
		{
			return (int)(channelColor * (float)LightBuffer.kLightMod);
		}

		private static int Hash(Light2D light, Material material)
		{
			return (-2128831035 * 16777619 ^ material.GetHashCode()) * 16777619 ^ ((light.lightCookieSprite == null) ? 0 : light.lightCookieSprite.GetHashCode());
		}

		private void Validate()
		{
		}

		private void OnAssemblyReload()
		{
			for (int i = 0; i < LightBuffer.kCount; i++)
			{
				this.lightBuffer[this.activeCount].Release();
			}
		}

		private void ResetInternals()
		{
			for (int i = 0; i < LightBuffer.kCount; i++)
			{
				if (this.lightBuffer[i] != null)
				{
					this.lightBuffer[i].Reset();
				}
			}
		}

		private void SetBuffer()
		{
			this.Validate();
			this.graphicsBuffer.SetData<PerLight2D>(this.nativeBuffer, this.lightCount, this.lightCount, math.min(LightBuffer.kBatchMax, LightBuffer.kMax - this.lightCount));
		}

		internal int SlotIndex(int x)
		{
			return this.lightCount + x;
		}

		internal void Reset()
		{
			if (LightBatch.isBatchingSupported)
			{
				this.maxIndex = 0;
				this.hashCode = 0;
				this.batchCount = 0;
				this.lightCount = 0;
				this.activeCount = 0;
				Shader.SetGlobalBuffer("_Light2DBuffer", this.graphicsBuffer);
			}
		}

		internal bool CanBatch(Light2D light, Material material, int index, out int lightHash)
		{
			lightHash = LightBatch.Hash(light, material);
			this.hashCode = ((this.hashCode == 0) ? lightHash : this.hashCode);
			if (this.batchCount == 0)
			{
				this.hashCode = lightHash;
			}
			else if (this.hashCode != lightHash || this.SlotIndex(index) >= LightBuffer.kMax || this.lightMarker[index] == 1)
			{
				this.hashCode = lightHash;
				return false;
			}
			return true;
		}

		internal bool AddBatch(Light2D light, Material material, Matrix4x4 mat, Mesh mesh, int subset, int lightHash, int index)
		{
			this.cachedLight = light;
			this.cachedMaterial = material;
			this.matrices[this.batchCount] = mat;
			this.lightMeshes[this.batchCount] = mesh;
			this.subsets[this.batchCount] = subset;
			this.batchCount++;
			this.maxIndex = math.max(this.maxIndex, index);
			this.lightMarker[index] = 1;
			return true;
		}

		internal void Flush(RasterCommandBuffer cmd)
		{
			if (this.batchCount > 0)
			{
				using (new ProfilingScope(cmd, LightBatch.profilingDrawBatched))
				{
					this.SetBuffer();
					cmd.SetGlobalInt(LightBatch.k_BufferOffset, this.lightCount);
					cmd.DrawMultipleMeshes(this.matrices, this.lightMeshes, this.subsets, this.batchCount, this.cachedMaterial, -1, null);
				}
				this.lightCount = this.lightCount + this.maxIndex + 1;
			}
			for (int i = 0; i < this.batchCount; i++)
			{
				this.lightMeshes[i] = null;
			}
			this.ResetInternals();
			this.batchCount = 0;
			this.maxIndex = 0;
		}

		private static readonly ProfilingSampler profilingDrawBatched = new ProfilingSampler("Light2D Batcher");

		private static readonly int k_BufferOffset = Shader.PropertyToID("_BatchBufferOffset");

		private static int sBatchIndexCounter = 0;

		private int[] subsets = new int[LightBuffer.kMax];

		private Mesh[] lightMeshes = new Mesh[LightBuffer.kMax];

		private Matrix4x4[] matrices = new Matrix4x4[LightBuffer.kMax];

		private LightBuffer[] lightBuffer = new LightBuffer[LightBuffer.kCount];

		private Light2D cachedLight;

		private Material cachedMaterial;

		private int hashCode;

		private int lightCount;

		private int maxIndex;

		private int batchCount;

		private int activeCount;
	}
}
