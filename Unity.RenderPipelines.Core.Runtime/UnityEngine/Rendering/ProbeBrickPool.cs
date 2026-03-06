using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	internal class ProbeBrickPool
	{
		internal static int DivRoundUp(int x, int y)
		{
			return (x + y - 1) / y;
		}

		internal int estimatedVMemCost { get; private set; }

		internal static void Initialize()
		{
			if (!SystemInfo.supportsComputeShaders)
			{
				return;
			}
			ProbeVolumeRuntimeResources renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<ProbeVolumeRuntimeResources>();
			ProbeBrickPool.s_DataUploadCS = ((renderPipelineSettings != null) ? renderPipelineSettings.probeVolumeUploadDataCS : null);
			ProbeVolumeRuntimeResources renderPipelineSettings2 = GraphicsSettings.GetRenderPipelineSettings<ProbeVolumeRuntimeResources>();
			ProbeBrickPool.s_DataUploadL2CS = ((renderPipelineSettings2 != null) ? renderPipelineSettings2.probeVolumeUploadDataL2CS : null);
			if (ProbeBrickPool.s_DataUploadCS != null)
			{
				ProbeBrickPool.s_DataUploadKernel = (ProbeBrickPool.s_DataUploadCS ? ProbeBrickPool.s_DataUploadCS.FindKernel("UploadData") : -1);
				ProbeBrickPool.s_DataUpload_Shared = new LocalKeyword(ProbeBrickPool.s_DataUploadCS, "PROBE_VOLUMES_SHARED_DATA");
				ProbeBrickPool.s_DataUpload_ProbeOcclusion = new LocalKeyword(ProbeBrickPool.s_DataUploadCS, "PROBE_VOLUMES_PROBE_OCCLUSION");
				ProbeBrickPool.s_DataUpload_SkyOcclusion = new LocalKeyword(ProbeBrickPool.s_DataUploadCS, "PROBE_VOLUMES_SKY_OCCLUSION");
				ProbeBrickPool.s_DataUpload_SkyShadingDirection = new LocalKeyword(ProbeBrickPool.s_DataUploadCS, "PROBE_VOLUMES_SKY_SHADING_DIRECTION");
			}
			if (ProbeBrickPool.s_DataUploadL2CS != null)
			{
				ProbeBrickPool.s_DataUploadL2Kernel = (ProbeBrickPool.s_DataUploadL2CS ? ProbeBrickPool.s_DataUploadL2CS.FindKernel("UploadDataL2") : -1);
			}
		}

		internal Texture GetValidityTexture()
		{
			return this.m_Pool.TexValidity;
		}

		internal Texture GetSkyOcclusionTexture()
		{
			return this.m_Pool.TexSkyOcclusion;
		}

		internal Texture GetSkyShadingDirectionIndicesTexture()
		{
			return this.m_Pool.TexSkyShadingDirectionIndices;
		}

		internal Texture GetProbeOcclusionTexture()
		{
			return this.m_Pool.TexProbeOcclusion;
		}

		internal ProbeBrickPool(ProbeVolumeTextureMemoryBudget memoryBudget, ProbeVolumeSHBands shBands, bool allocateValidityData = false, bool allocateRenderingLayerData = false, bool allocateSkyOcclusion = false, bool allocateSkyShadingData = false, bool allocateProbeOcclusionData = false)
		{
			this.m_NextFreeChunk.x = (this.m_NextFreeChunk.y = (this.m_NextFreeChunk.z = 0));
			this.m_SHBands = shBands;
			this.m_ContainsValidity = allocateValidityData;
			this.m_ContainsProbeOcclusion = allocateProbeOcclusionData;
			this.m_ContainsRenderingLayers = allocateRenderingLayerData;
			this.m_ContainsSkyOcclusion = allocateSkyOcclusion;
			this.m_ContainsSkyShadingDirection = allocateSkyShadingData;
			this.m_FreeList = new Stack<ProbeBrickPool.BrickChunkAlloc>(256);
			int width;
			int height;
			int depth;
			ProbeBrickPool.DerivePoolSizeFromBudget(memoryBudget, out width, out height, out depth);
			this.AllocatePool(width, height, depth);
			this.m_AvailableChunkCount = this.m_Pool.width / 512 * (this.m_Pool.height / 4) * (this.m_Pool.depth / 4);
		}

		internal void AllocatePool(int width, int height, int depth)
		{
			int estimatedVMemCost;
			this.m_Pool = ProbeBrickPool.CreateDataLocation(width * height * depth, false, this.m_SHBands, "APV", true, this.m_ContainsValidity, this.m_ContainsRenderingLayers, this.m_ContainsSkyOcclusion, this.m_ContainsSkyShadingDirection, this.m_ContainsProbeOcclusion, out estimatedVMemCost);
			this.estimatedVMemCost = estimatedVMemCost;
		}

		public int GetRemainingChunkCount()
		{
			return this.m_AvailableChunkCount;
		}

		internal void EnsureTextureValidity()
		{
			if (this.m_Pool.TexL0_L1rx == null)
			{
				this.m_Pool.Cleanup();
				this.AllocatePool(this.m_Pool.width, this.m_Pool.height, this.m_Pool.depth);
			}
		}

		internal bool EnsureTextureValidity(bool renderingLayers, bool skyOcclusion, bool skyDirection, bool probeOcclusion)
		{
			if (this.m_ContainsRenderingLayers != renderingLayers || this.m_ContainsSkyOcclusion != skyOcclusion || this.m_ContainsSkyShadingDirection != skyDirection || this.m_ContainsProbeOcclusion != probeOcclusion)
			{
				this.m_Pool.Cleanup();
				this.m_ContainsRenderingLayers = renderingLayers;
				this.m_ContainsSkyOcclusion = skyOcclusion;
				this.m_ContainsSkyShadingDirection = skyDirection;
				this.m_ContainsProbeOcclusion = probeOcclusion;
				this.AllocatePool(this.m_Pool.width, this.m_Pool.height, this.m_Pool.depth);
				return false;
			}
			return true;
		}

		internal static int GetChunkSizeInBrickCount()
		{
			return 128;
		}

		internal static int GetChunkSizeInProbeCount()
		{
			return 8192;
		}

		internal int GetPoolWidth()
		{
			return this.m_Pool.width;
		}

		internal int GetPoolHeight()
		{
			return this.m_Pool.height;
		}

		internal Vector3Int GetPoolDimensions()
		{
			return new Vector3Int(this.m_Pool.width, this.m_Pool.height, this.m_Pool.depth);
		}

		internal void GetRuntimeResources(ref ProbeReferenceVolume.RuntimeResources rr)
		{
			rr.L0_L1rx = (this.m_Pool.TexL0_L1rx as RenderTexture);
			rr.L1_G_ry = (this.m_Pool.TexL1_G_ry as RenderTexture);
			rr.L1_B_rz = (this.m_Pool.TexL1_B_rz as RenderTexture);
			rr.L2_0 = (this.m_Pool.TexL2_0 as RenderTexture);
			rr.L2_1 = (this.m_Pool.TexL2_1 as RenderTexture);
			rr.L2_2 = (this.m_Pool.TexL2_2 as RenderTexture);
			rr.L2_3 = (this.m_Pool.TexL2_3 as RenderTexture);
			rr.ProbeOcclusion = (this.m_Pool.TexProbeOcclusion as RenderTexture);
			rr.Validity = (this.m_Pool.TexValidity as RenderTexture);
			rr.SkyOcclusionL0L1 = (this.m_Pool.TexSkyOcclusion as RenderTexture);
			rr.SkyShadingDirectionIndices = (this.m_Pool.TexSkyShadingDirectionIndices as RenderTexture);
		}

		internal void Clear()
		{
			this.m_FreeList.Clear();
			this.m_NextFreeChunk.x = (this.m_NextFreeChunk.y = (this.m_NextFreeChunk.z = 0));
		}

		internal static int GetChunkCount(int brickCount)
		{
			int num = 128;
			return (brickCount + num - 1) / num;
		}

		internal bool Allocate(int numberOfBrickChunks, List<ProbeBrickPool.BrickChunkAlloc> outAllocations, bool ignoreErrorLog)
		{
			while (this.m_FreeList.Count > 0 && numberOfBrickChunks > 0)
			{
				outAllocations.Add(this.m_FreeList.Pop());
				numberOfBrickChunks--;
				this.m_AvailableChunkCount--;
			}
			uint num = 0U;
			while ((ulong)num < (ulong)((long)numberOfBrickChunks))
			{
				if (this.m_NextFreeChunk.z >= this.m_Pool.depth)
				{
					if (!ignoreErrorLog)
					{
						Debug.LogError("Cannot allocate more brick chunks, probe volume brick pool is full.");
					}
					this.Deallocate(outAllocations);
					outAllocations.Clear();
					return false;
				}
				outAllocations.Add(this.m_NextFreeChunk);
				this.m_AvailableChunkCount--;
				this.m_NextFreeChunk.x = this.m_NextFreeChunk.x + 512;
				if (this.m_NextFreeChunk.x >= this.m_Pool.width)
				{
					this.m_NextFreeChunk.x = 0;
					this.m_NextFreeChunk.y = this.m_NextFreeChunk.y + 4;
					if (this.m_NextFreeChunk.y >= this.m_Pool.height)
					{
						this.m_NextFreeChunk.y = 0;
						this.m_NextFreeChunk.z = this.m_NextFreeChunk.z + 4;
					}
				}
				num += 1U;
			}
			return true;
		}

		internal void Deallocate(List<ProbeBrickPool.BrickChunkAlloc> allocations)
		{
			this.m_AvailableChunkCount += allocations.Count;
			foreach (ProbeBrickPool.BrickChunkAlloc item in allocations)
			{
				this.m_FreeList.Push(item);
			}
		}

		internal void Update(ProbeBrickPool.DataLocation source, List<ProbeBrickPool.BrickChunkAlloc> srcLocations, List<ProbeBrickPool.BrickChunkAlloc> dstLocations, int destStartIndex, ProbeVolumeSHBands bands)
		{
			for (int i = 0; i < srcLocations.Count; i++)
			{
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = srcLocations[i];
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc2 = dstLocations[destStartIndex + i];
				for (int j = 0; j < 4; j++)
				{
					int srcWidth = Mathf.Min(512, source.width - brickChunkAlloc.x);
					Graphics.CopyTexture(source.TexL0_L1rx, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL0_L1rx, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
					Graphics.CopyTexture(source.TexL1_G_ry, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL1_G_ry, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
					Graphics.CopyTexture(source.TexL1_B_rz, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL1_B_rz, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
					if (this.m_ContainsValidity)
					{
						Graphics.CopyTexture(source.TexValidity, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexValidity, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
					}
					if (this.m_ContainsSkyOcclusion)
					{
						Graphics.CopyTexture(source.TexSkyOcclusion, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexSkyOcclusion, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
						if (this.m_ContainsSkyShadingDirection)
						{
							Graphics.CopyTexture(source.TexSkyShadingDirectionIndices, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexSkyShadingDirectionIndices, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
						}
					}
					if (bands == ProbeVolumeSHBands.SphericalHarmonicsL2)
					{
						Graphics.CopyTexture(source.TexL2_0, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL2_0, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
						Graphics.CopyTexture(source.TexL2_1, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL2_1, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
						Graphics.CopyTexture(source.TexL2_2, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL2_2, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
						Graphics.CopyTexture(source.TexL2_3, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexL2_3, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
					}
					if (this.m_ContainsProbeOcclusion)
					{
						Graphics.CopyTexture(source.TexProbeOcclusion, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexProbeOcclusion, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
					}
				}
			}
		}

		internal void Update(CommandBuffer cmd, ProbeReferenceVolume.CellStreamingScratchBuffer dataBuffer, ProbeReferenceVolume.CellStreamingScratchBufferLayout layout, List<ProbeBrickPool.BrickChunkAlloc> dstLocations, bool updateSharedData, Texture validityTexture, ProbeVolumeSHBands bands, bool skyOcclusion, Texture skyOcclusionTexture, bool skyShadingDirections, Texture skyShadingDirectionsTexture, bool probeOcclusion)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get<CoreProfileId>(CoreProfileId.APVDiskStreamingUpdatePool)))
			{
				int count = dstLocations.Count;
				cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_L0_L1Rx, this.m_Pool.TexL0_L1rx);
				cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_L1G_L1Ry, this.m_Pool.TexL1_G_ry);
				cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_L1B_L1Rz, this.m_Pool.TexL1_B_rz);
				if (updateSharedData)
				{
					cmd.EnableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_Shared);
					cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_Shared, validityTexture);
					if (skyOcclusion)
					{
						cmd.EnableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_SkyOcclusion);
						cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_SkyOcclusionL0L1, skyOcclusionTexture);
						if (skyShadingDirections)
						{
							cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_SkyShadingDirectionIndices, skyShadingDirectionsTexture);
							cmd.EnableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_SkyShadingDirection);
						}
						else
						{
							cmd.DisableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_SkyShadingDirection);
						}
					}
				}
				else
				{
					cmd.DisableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_Shared);
					cmd.DisableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_SkyOcclusion);
					cmd.DisableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_SkyShadingDirection);
				}
				if (bands == ProbeVolumeSHBands.SphericalHarmonicsL2)
				{
					cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool.s_DataUploadL2Kernel, ProbeBrickPool._Out_L2_0, this.m_Pool.TexL2_0);
					cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool.s_DataUploadL2Kernel, ProbeBrickPool._Out_L2_1, this.m_Pool.TexL2_1);
					cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool.s_DataUploadL2Kernel, ProbeBrickPool._Out_L2_2, this.m_Pool.TexL2_2);
					cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool.s_DataUploadL2Kernel, ProbeBrickPool._Out_L2_3, this.m_Pool.TexL2_3);
				}
				if (probeOcclusion)
				{
					cmd.EnableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_ProbeOcclusion);
					cmd.SetComputeTextureParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._Out_ProbeOcclusion, this.m_Pool.TexProbeOcclusion);
				}
				else
				{
					cmd.DisableKeyword(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUpload_ProbeOcclusion);
				}
				int threadGroupsX = ProbeBrickPool.DivRoundUp(2048, 64);
				ConstantBuffer.Push<ProbeReferenceVolume.CellStreamingScratchBufferLayout>(cmd, layout, ProbeBrickPool.s_DataUploadCS, ProbeBrickPool._ProbeVolumeScratchBufferLayout);
				cmd.SetComputeBufferParam(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, ProbeBrickPool._ProbeVolumeScratchBuffer, dataBuffer.buffer);
				cmd.DispatchCompute(ProbeBrickPool.s_DataUploadCS, ProbeBrickPool.s_DataUploadKernel, threadGroupsX, 1, count);
				if (bands == ProbeVolumeSHBands.SphericalHarmonicsL2)
				{
					ConstantBuffer.Push<ProbeReferenceVolume.CellStreamingScratchBufferLayout>(cmd, layout, ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool._ProbeVolumeScratchBufferLayout);
					cmd.SetComputeBufferParam(ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool.s_DataUploadL2Kernel, ProbeBrickPool._ProbeVolumeScratchBuffer, dataBuffer.buffer);
					cmd.DispatchCompute(ProbeBrickPool.s_DataUploadL2CS, ProbeBrickPool.s_DataUploadL2Kernel, threadGroupsX, 1, count);
				}
			}
		}

		internal void UpdateValidity(ProbeBrickPool.DataLocation source, List<ProbeBrickPool.BrickChunkAlloc> srcLocations, List<ProbeBrickPool.BrickChunkAlloc> dstLocations, int destStartIndex)
		{
			for (int i = 0; i < srcLocations.Count; i++)
			{
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = srcLocations[i];
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc2 = dstLocations[destStartIndex + i];
				for (int j = 0; j < 4; j++)
				{
					int srcWidth = Mathf.Min(512, source.width - brickChunkAlloc.x);
					Graphics.CopyTexture(source.TexValidity, brickChunkAlloc.z + j, 0, brickChunkAlloc.x, brickChunkAlloc.y, srcWidth, 4, this.m_Pool.TexValidity, brickChunkAlloc2.z + j, 0, brickChunkAlloc2.x, brickChunkAlloc2.y);
				}
			}
		}

		internal static Vector3Int ProbeCountToDataLocSize(int numProbes)
		{
			int num = numProbes / 64;
			int num2 = 512;
			int num3 = (num + num2 * num2 - 1) / (num2 * num2);
			int num5;
			int num4;
			if (num3 > 1)
			{
				num4 = (num5 = num2);
			}
			else
			{
				num4 = (num + num2 - 1) / num2;
				if (num4 > 1)
				{
					num5 = num2;
				}
				else
				{
					num5 = num;
				}
			}
			num5 *= 4;
			num4 *= 4;
			num3 *= 4;
			return new Vector3Int(num5, num4, num3);
		}

		private static int EstimateMemoryCost(int width, int height, int depth, GraphicsFormat format)
		{
			int num = (format == GraphicsFormat.R16G16B16A16_SFloat) ? 8 : ((format == GraphicsFormat.R8G8B8A8_UNorm) ? 4 : 1);
			return width * height * depth * num;
		}

		internal static int EstimateMemoryCostForBlending(ProbeVolumeTextureMemoryBudget memoryBudget, bool compressed, ProbeVolumeSHBands bands)
		{
			if (memoryBudget == (ProbeVolumeTextureMemoryBudget)0)
			{
				return 0;
			}
			int x;
			int y;
			int z;
			ProbeBrickPool.DerivePoolSizeFromBudget(memoryBudget, out x, out y, out z);
			Vector3Int vector3Int = ProbeBrickPool.ProbeCountToDataLocSize(x * y * z);
			x = vector3Int.x;
			y = vector3Int.y;
			z = vector3Int.z;
			int num = 0;
			GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
			GraphicsFormat format2 = compressed ? GraphicsFormat.RGBA_BC7_UNorm : GraphicsFormat.R8G8B8A8_UNorm;
			num += ProbeBrickPool.EstimateMemoryCost(x, y, z, format);
			num += ProbeBrickPool.EstimateMemoryCost(x, y, z, format2) * 2;
			if (bands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				num += ProbeBrickPool.EstimateMemoryCost(x, y, z, format2) * 3;
			}
			return num;
		}

		public static Texture CreateDataTexture(int width, int height, int depth, GraphicsFormat format, string name, bool allocateRendertexture, ref int allocatedBytes)
		{
			allocatedBytes += ProbeBrickPool.EstimateMemoryCost(width, height, depth, format);
			Texture texture;
			if (allocateRendertexture)
			{
				texture = new RenderTexture(new RenderTextureDescriptor
				{
					width = width,
					height = height,
					volumeDepth = depth,
					graphicsFormat = format,
					mipCount = 1,
					enableRandomWrite = SystemInfo.supportsComputeShaders,
					dimension = TextureDimension.Tex3D,
					msaaSamples = 1
				});
			}
			else
			{
				texture = new Texture3D(width, height, depth, format, TextureCreationFlags.None, 1);
			}
			texture.hideFlags = HideFlags.HideAndDontSave;
			texture.name = name;
			if (allocateRendertexture)
			{
				(texture as RenderTexture).Create();
			}
			return texture;
		}

		public static ProbeBrickPool.DataLocation CreateDataLocation(int numProbes, bool compressed, ProbeVolumeSHBands bands, string name, bool allocateRendertexture, bool allocateValidityData, bool allocateRenderingLayers, bool allocateSkyOcclusionData, bool allocateSkyShadingDirectionData, bool allocateProbeOcclusionData, out int allocatedBytes)
		{
			Vector3Int vector3Int = ProbeBrickPool.ProbeCountToDataLocSize(numProbes);
			int x = vector3Int.x;
			int y = vector3Int.y;
			int z = vector3Int.z;
			GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
			GraphicsFormat format2 = compressed ? GraphicsFormat.RGBA_BC7_UNorm : GraphicsFormat.R8G8B8A8_UNorm;
			GraphicsFormat format3 = allocateRenderingLayers ? GraphicsFormat.R32_SFloat : (SystemInfo.IsFormatSupported(GraphicsFormat.R8_UNorm, GraphicsFormatUsage.Sample | GraphicsFormatUsage.LoadStore) ? GraphicsFormat.R8_UNorm : GraphicsFormat.R8G8B8A8_UNorm);
			allocatedBytes = 0;
			ProbeBrickPool.DataLocation result;
			result.TexL0_L1rx = ProbeBrickPool.CreateDataTexture(x, y, z, format, name + "_TexL0_L1rx", allocateRendertexture, ref allocatedBytes);
			result.TexL1_G_ry = ProbeBrickPool.CreateDataTexture(x, y, z, format2, name + "_TexL1_G_ry", allocateRendertexture, ref allocatedBytes);
			result.TexL1_B_rz = ProbeBrickPool.CreateDataTexture(x, y, z, format2, name + "_TexL1_B_rz", allocateRendertexture, ref allocatedBytes);
			if (allocateValidityData)
			{
				result.TexValidity = ProbeBrickPool.CreateDataTexture(x, y, z, format3, name + "_Validity", allocateRendertexture, ref allocatedBytes);
			}
			else
			{
				result.TexValidity = null;
			}
			if (allocateSkyOcclusionData)
			{
				result.TexSkyOcclusion = ProbeBrickPool.CreateDataTexture(x, y, z, GraphicsFormat.R16G16B16A16_SFloat, name + "_SkyOcclusion", allocateRendertexture, ref allocatedBytes);
			}
			else
			{
				result.TexSkyOcclusion = null;
			}
			if (allocateSkyShadingDirectionData)
			{
				result.TexSkyShadingDirectionIndices = ProbeBrickPool.CreateDataTexture(x, y, z, GraphicsFormat.R8_UNorm, name + "_SkyShadingDirectionIndices", allocateRendertexture, ref allocatedBytes);
			}
			else
			{
				result.TexSkyShadingDirectionIndices = null;
			}
			if (allocateProbeOcclusionData)
			{
				result.TexProbeOcclusion = ProbeBrickPool.CreateDataTexture(x, y, z, GraphicsFormat.R8G8B8A8_UNorm, name + "_ProbeOcclusion", allocateRendertexture, ref allocatedBytes);
			}
			else
			{
				result.TexProbeOcclusion = null;
			}
			if (bands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				result.TexL2_0 = ProbeBrickPool.CreateDataTexture(x, y, z, format2, name + "_TexL2_0", allocateRendertexture, ref allocatedBytes);
				result.TexL2_1 = ProbeBrickPool.CreateDataTexture(x, y, z, format2, name + "_TexL2_1", allocateRendertexture, ref allocatedBytes);
				result.TexL2_2 = ProbeBrickPool.CreateDataTexture(x, y, z, format2, name + "_TexL2_2", allocateRendertexture, ref allocatedBytes);
				result.TexL2_3 = ProbeBrickPool.CreateDataTexture(x, y, z, format2, name + "_TexL2_3", allocateRendertexture, ref allocatedBytes);
			}
			else
			{
				result.TexL2_0 = null;
				result.TexL2_1 = null;
				result.TexL2_2 = null;
				result.TexL2_3 = null;
			}
			result.width = x;
			result.height = y;
			result.depth = z;
			return result;
		}

		private static void DerivePoolSizeFromBudget(ProbeVolumeTextureMemoryBudget memoryBudget, out int width, out int height, out int depth)
		{
			width = (int)memoryBudget;
			height = (int)memoryBudget;
			depth = 4;
		}

		internal void Cleanup()
		{
			this.m_Pool.Cleanup();
		}

		internal static readonly int _Out_L0_L1Rx = Shader.PropertyToID("_Out_L0_L1Rx");

		internal static readonly int _Out_L1G_L1Ry = Shader.PropertyToID("_Out_L1G_L1Ry");

		internal static readonly int _Out_L1B_L1Rz = Shader.PropertyToID("_Out_L1B_L1Rz");

		internal static readonly int _Out_Shared = Shader.PropertyToID("_Out_Shared");

		internal static readonly int _Out_ProbeOcclusion = Shader.PropertyToID("_Out_ProbeOcclusion");

		internal static readonly int _Out_SkyOcclusionL0L1 = Shader.PropertyToID("_Out_SkyOcclusionL0L1");

		internal static readonly int _Out_SkyShadingDirectionIndices = Shader.PropertyToID("_Out_SkyShadingDirectionIndices");

		internal static readonly int _Out_L2_0 = Shader.PropertyToID("_Out_L2_0");

		internal static readonly int _Out_L2_1 = Shader.PropertyToID("_Out_L2_1");

		internal static readonly int _Out_L2_2 = Shader.PropertyToID("_Out_L2_2");

		internal static readonly int _Out_L2_3 = Shader.PropertyToID("_Out_L2_3");

		internal static readonly int _ProbeVolumeScratchBufferLayout = Shader.PropertyToID("CellStreamingScratchBufferLayout");

		internal static readonly int _ProbeVolumeScratchBuffer = Shader.PropertyToID("_ScratchBuffer");

		private const int kChunkSizeInBricks = 128;

		internal const int kBrickCellCount = 3;

		internal const int kBrickProbeCountPerDim = 4;

		internal const int kBrickProbeCountTotal = 64;

		internal const int kChunkProbeCountPerDim = 512;

		private const int kMaxPoolWidth = 2048;

		internal ProbeBrickPool.DataLocation m_Pool;

		private ProbeBrickPool.BrickChunkAlloc m_NextFreeChunk;

		private Stack<ProbeBrickPool.BrickChunkAlloc> m_FreeList;

		private int m_AvailableChunkCount;

		private ProbeVolumeSHBands m_SHBands;

		private bool m_ContainsValidity;

		private bool m_ContainsProbeOcclusion;

		private bool m_ContainsRenderingLayers;

		private bool m_ContainsSkyOcclusion;

		private bool m_ContainsSkyShadingDirection;

		private static ComputeShader s_DataUploadCS;

		private static int s_DataUploadKernel;

		private static ComputeShader s_DataUploadL2CS;

		private static int s_DataUploadL2Kernel;

		private static LocalKeyword s_DataUpload_Shared;

		private static LocalKeyword s_DataUpload_ProbeOcclusion;

		private static LocalKeyword s_DataUpload_SkyOcclusion;

		private static LocalKeyword s_DataUpload_SkyShadingDirection;

		[DebuggerDisplay("Chunk ({x}, {y}, {z})")]
		public struct BrickChunkAlloc
		{
			internal int flattenIndex(int sx, int sy)
			{
				return this.z * (sx * sy) + this.y * sx + this.x;
			}

			public int x;

			public int y;

			public int z;
		}

		public struct DataLocation
		{
			internal void Cleanup()
			{
				CoreUtils.Destroy(this.TexL0_L1rx);
				CoreUtils.Destroy(this.TexL1_G_ry);
				CoreUtils.Destroy(this.TexL1_B_rz);
				CoreUtils.Destroy(this.TexL2_0);
				CoreUtils.Destroy(this.TexL2_1);
				CoreUtils.Destroy(this.TexL2_2);
				CoreUtils.Destroy(this.TexL2_3);
				CoreUtils.Destroy(this.TexProbeOcclusion);
				CoreUtils.Destroy(this.TexValidity);
				CoreUtils.Destroy(this.TexSkyOcclusion);
				CoreUtils.Destroy(this.TexSkyShadingDirectionIndices);
				this.TexL0_L1rx = null;
				this.TexL1_G_ry = null;
				this.TexL1_B_rz = null;
				this.TexL2_0 = null;
				this.TexL2_1 = null;
				this.TexL2_2 = null;
				this.TexL2_3 = null;
				this.TexProbeOcclusion = null;
				this.TexValidity = null;
				this.TexSkyOcclusion = null;
				this.TexSkyShadingDirectionIndices = null;
			}

			internal Texture TexL0_L1rx;

			internal Texture TexL1_G_ry;

			internal Texture TexL1_B_rz;

			internal Texture TexL2_0;

			internal Texture TexL2_1;

			internal Texture TexL2_2;

			internal Texture TexL2_3;

			internal Texture TexProbeOcclusion;

			internal Texture TexValidity;

			internal Texture TexSkyOcclusion;

			internal Texture TexSkyShadingDirectionIndices;

			internal int width;

			internal int height;

			internal int depth;
		}
	}
}
