using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace UnityEngine.VFX.SDF
{
	public class MeshToSDFBaker : IDisposable
	{
		public RenderTexture SdfTexture
		{
			get
			{
				return this.m_DistanceTexture;
			}
		}

		private static Mesh InitMeshFromList(List<Mesh> meshes, List<Matrix4x4> transforms)
		{
			int count = meshes.Count;
			if (count != transforms.Count)
			{
				throw new ArgumentException("The number of meshes must be the same as the number of transforms");
			}
			List<CombineInstance> list = new List<CombineInstance>();
			for (int i = 0; i < count; i++)
			{
				Mesh mesh = meshes[i];
				for (int j = 0; j < mesh.subMeshCount; j++)
				{
					list.Add(new CombineInstance
					{
						mesh = meshes[i],
						subMeshIndex = j,
						transform = transforms[i]
					});
				}
			}
			Mesh mesh2 = new Mesh();
			mesh2.indexFormat = IndexFormat.UInt32;
			mesh2.CombineMeshes(list.ToArray());
			return mesh2;
		}

		private void InitCommandBuffer()
		{
			if (this.m_Cmd == null)
			{
				this.m_Cmd = new CommandBuffer
				{
					name = "SDFBakingCommand"
				};
			}
		}

		private int GetTotalVoxelCount()
		{
			return this.m_Dimensions[0] * this.m_Dimensions[1] * this.m_Dimensions[2];
		}

		private void InitSizeBox()
		{
			this.m_MaxExtent = Mathf.Max(this.m_SizeBox.x, Mathf.Max(this.m_SizeBox.y, this.m_SizeBox.z));
			float num = 0f;
			if (this.m_MaxExtent == this.m_SizeBox.x)
			{
				this.m_Dimensions[0] = Mathf.Max(Mathf.RoundToInt((float)this.m_maxResolution * this.m_SizeBox.x / this.m_MaxExtent), 1);
				this.m_Dimensions[1] = Mathf.Max(Mathf.CeilToInt((float)this.m_maxResolution * this.m_SizeBox.y / this.m_MaxExtent), 1);
				this.m_Dimensions[2] = Mathf.Max(Mathf.CeilToInt((float)this.m_maxResolution * this.m_SizeBox.z / this.m_MaxExtent), 1);
				num = this.m_MaxExtent / (float)this.m_Dimensions[0];
			}
			else if (this.m_MaxExtent == this.m_SizeBox.y)
			{
				this.m_Dimensions[1] = Mathf.Max(Mathf.RoundToInt((float)this.m_maxResolution * this.m_SizeBox.y / this.m_MaxExtent), 1);
				this.m_Dimensions[0] = Mathf.Max(Mathf.CeilToInt((float)this.m_maxResolution * this.m_SizeBox.x / this.m_MaxExtent), 1);
				this.m_Dimensions[2] = Mathf.Max(Mathf.CeilToInt((float)this.m_maxResolution * this.m_SizeBox.z / this.m_MaxExtent), 1);
				num = this.m_MaxExtent / (float)this.m_Dimensions[1];
			}
			else if (this.m_MaxExtent == this.m_SizeBox.z)
			{
				this.m_Dimensions[2] = Mathf.Max(Mathf.RoundToInt((float)this.m_maxResolution * this.m_SizeBox.z / this.m_MaxExtent), 1);
				this.m_Dimensions[1] = Mathf.Max(Mathf.CeilToInt((float)this.m_maxResolution * this.m_SizeBox.y / this.m_MaxExtent), 1);
				this.m_Dimensions[0] = Mathf.Max(Mathf.CeilToInt((float)this.m_maxResolution * this.m_SizeBox.x / this.m_MaxExtent), 1);
				num = this.m_MaxExtent / (float)this.m_Dimensions[2];
			}
			if ((long)this.GetTotalVoxelCount() > (long)((ulong)MeshToSDFBaker.kMaxAbsoluteGridSize))
			{
				throw new ArgumentException(string.Format("The size of the voxel grid is too big (>2^{0}), reduce the resolution, or provide a thinner bounding box.", Mathf.Log(MeshToSDFBaker.kMaxAbsoluteGridSize, 2f)));
			}
			for (int i = 0; i < 3; i++)
			{
				this.m_SizeBox[i] = (float)this.m_Dimensions[i] * num;
			}
		}

		public Vector3Int GetGridSize()
		{
			return new Vector3Int(this.m_Dimensions[0], this.m_Dimensions[1], this.m_Dimensions[2]);
		}

		public Vector3 GetActualBoxSize()
		{
			return this.m_SizeBox;
		}

		public MeshToSDFBaker(Vector3 sizeBox, Vector3 center, int maxRes, Mesh mesh, int signPassesCount = 1, float threshold = 0.5f, float sdfOffset = 0f, CommandBuffer cmd = null)
		{
			this.LoadRuntimeResources();
			this.m_Mesh = mesh;
			if (cmd != null)
			{
				this.m_Cmd = cmd;
				this.m_OwnsCommandBuffer = false;
			}
			this.SetParameters(sizeBox, center, maxRes, signPassesCount, threshold, sdfOffset);
			this.Init();
		}

		public MeshToSDFBaker(Vector3 sizeBox, Vector3 center, int maxRes, List<Mesh> meshes, List<Matrix4x4> transforms, int signPassesCount = 1, float threshold = 0.5f, float sdfOffset = 0f, CommandBuffer cmd = null) : this(sizeBox, center, maxRes, MeshToSDFBaker.InitMeshFromList(meshes, transforms), signPassesCount, threshold, sdfOffset, cmd)
		{
		}

		~MeshToSDFBaker()
		{
			if (!this.m_IsDisposed)
			{
				Debug.LogWarning("Dispose() should be called explicitly when an MeshToSDFBaker instance is finished being used.");
			}
		}

		public void Reinit(Vector3 sizeBox, Vector3 center, int maxRes, Mesh mesh, int signPassesCount = 1, float threshold = 0.5f, float sdfOffset = 0f)
		{
			this.m_Mesh = mesh;
			this.SetParameters(sizeBox, center, maxRes, signPassesCount, threshold, sdfOffset);
			this.Init();
		}

		public void Reinit(Vector3 sizeBox, Vector3 center, int maxRes, List<Mesh> meshes, List<Matrix4x4> transforms, int signPassesCount = 1, float threshold = 0.5f, float sdfOffset = 0f)
		{
			this.Reinit(sizeBox, center, maxRes, MeshToSDFBaker.InitMeshFromList(meshes, transforms), signPassesCount, threshold, sdfOffset);
		}

		private void SetParameters(Vector3 sizeBox, Vector3 center, int maxRes, int signPassesCount, float threshold, float sdfOffset)
		{
			if (this.m_SignPassesCount >= 20)
			{
				throw new ArgumentException("The signPassCount argument should be smaller than 20.");
			}
			this.m_SignPassesCount = signPassesCount;
			this.m_InOutThreshold = threshold;
			this.m_SdfOffset = sdfOffset;
			this.m_Center = center;
			this.m_SizeBox = sizeBox;
			this.m_maxResolution = maxRes;
		}

		private void LoadRuntimeResources()
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
			{
				Debug.LogWarning("MeshToSDFBaker compute shaders are not supported on OpenGLES3");
			}
			this.m_RuntimeResources = VFXRuntimeResources.runtimeResources;
			if (this.m_RuntimeResources == null)
			{
				throw new InvalidOperationException("VFX Runtime Resources could not be loaded.");
			}
		}

		private void InitTextures()
		{
			RenderTextureDescriptor rtDesc = new RenderTextureDescriptor
			{
				graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat,
				dimension = TextureDimension.Tex3D,
				enableRandomWrite = true,
				width = this.m_Dimensions[0],
				height = this.m_Dimensions[1],
				volumeDepth = this.m_Dimensions[2],
				msaaSamples = 1
			};
			RenderTextureDescriptor rtDesc2 = new RenderTextureDescriptor
			{
				graphicsFormat = GraphicsFormat.R16_SFloat,
				dimension = TextureDimension.Tex3D,
				enableRandomWrite = true,
				width = this.m_Dimensions[0],
				height = this.m_Dimensions[1],
				volumeDepth = this.m_Dimensions[2],
				msaaSamples = 1
			};
			RenderTextureDescriptor rtDesc3 = new RenderTextureDescriptor
			{
				graphicsFormat = GraphicsFormat.R32_SFloat,
				dimension = TextureDimension.Tex3D,
				enableRandomWrite = true,
				width = this.m_Dimensions[0],
				height = this.m_Dimensions[1],
				volumeDepth = this.m_Dimensions[2],
				msaaSamples = 1
			};
			this.CreateRenderTextureIfNeeded(ref this.m_textureVoxel, rtDesc);
			this.CreateRenderTextureIfNeeded(ref this.m_textureVoxelBis, rtDesc);
			if (this.m_RayMaps == null)
			{
				this.m_RayMaps = new RenderTexture[2];
			}
			if (this.m_SignMaps == null)
			{
				this.m_SignMaps = new RenderTexture[2];
			}
			for (int i = 0; i < 2; i++)
			{
				this.CreateRenderTextureIfNeeded(ref this.m_RayMaps[i], rtDesc);
				this.CreateRenderTextureIfNeeded(ref this.m_SignMaps[i], rtDesc3);
			}
			this.CreateRenderTextureIfNeeded(ref this.m_DistanceTexture, rtDesc2);
			this.CreateGraphicsBufferIfNeeded(ref this.m_bufferVoxel, this.GetTotalVoxelCount(), 16);
			this.InitPrefixSumBuffers();
		}

		private void Init()
		{
			this.m_Mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
			this.m_Mesh.indexBufferTarget |= GraphicsBuffer.Target.Raw;
			this.InitSizeBox();
			this.InitCommandBuffer();
			this.m_ThreadGroupSize = 512;
			this.m_computeShader = this.m_RuntimeResources.sdfRayMapCS;
			if (this.m_computeShader == null)
			{
				throw new InvalidOperationException("VFX Runtime Resources could not be loaded correctly.");
			}
			if (this.m_Kernels == null)
			{
				this.m_Kernels = new MeshToSDFBaker.Kernels(this.m_computeShader);
			}
			this.InitTextures();
			RenderTextureDescriptor rtDesc = default(RenderTextureDescriptor);
			rtDesc.width = this.m_Dimensions[0];
			rtDesc.height = this.m_Dimensions[1];
			rtDesc.graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
			rtDesc.volumeDepth = 1;
			rtDesc.msaaSamples = 1;
			rtDesc.dimension = TextureDimension.Tex2D;
			if (this.m_RenderTextureViews == null)
			{
				this.m_RenderTextureViews = new RenderTexture[3];
			}
			for (int i = 0; i < 3; i++)
			{
				switch (i)
				{
				case 0:
					rtDesc.width = this.m_Dimensions[0];
					rtDesc.height = this.m_Dimensions[1];
					this.CreateRenderTextureIfNeeded(ref this.m_RenderTextureViews[i], rtDesc);
					break;
				case 1:
					rtDesc.width = this.m_Dimensions[2];
					rtDesc.height = this.m_Dimensions[0];
					this.CreateRenderTextureIfNeeded(ref this.m_RenderTextureViews[i], rtDesc);
					break;
				case 2:
					rtDesc.width = this.m_Dimensions[1];
					rtDesc.height = this.m_Dimensions[2];
					this.CreateRenderTextureIfNeeded(ref this.m_RenderTextureViews[i], rtDesc);
					break;
				}
			}
			if (this.m_Material == null || this.m_Material[0] == null || this.m_Material[1] == null || this.m_Material[2] == null)
			{
				this.m_Material = new Material[3];
				Shader sdfRayMapShader = this.m_RuntimeResources.sdfRayMapShader;
				if (sdfRayMapShader == null)
				{
					throw new InvalidOperationException("VFX Runtime Resources could not be loaded correctly.");
				}
				for (int j = 0; j < 3; j++)
				{
					this.m_Material[j] = new Material(sdfRayMapShader);
				}
			}
			if (this.m_WorldToClip == null)
			{
				this.m_WorldToClip = new Matrix4x4[3];
			}
			if (this.m_ProjMat == null)
			{
				this.m_ProjMat = new Matrix4x4[3];
			}
			if (this.m_ViewMat == null)
			{
				this.m_ViewMat = new Matrix4x4[3];
			}
			this.UpdateCameras();
		}

		private void UpdateCameras()
		{
			Vector3 pos = this.m_Center + Vector3.back * (this.m_SizeBox.z * 0.5f + 1f);
			Quaternion rot = Quaternion.identity;
			float num = 1f;
			float far = num + this.m_SizeBox.z;
			this.m_WorldToClip[0] = this.ComputeOrthographicWorldToClip(pos, rot, this.m_SizeBox.x, this.m_SizeBox.y, num, far, out this.m_ProjMat[0], out this.m_ViewMat[0]);
			pos = this.m_Center + Vector3.down * (this.m_SizeBox.y * 0.5f + 1f);
			rot = Quaternion.Euler(-90f, -90f, 0f);
			far = num + this.m_SizeBox.y;
			this.m_WorldToClip[1] = this.ComputeOrthographicWorldToClip(pos, rot, this.m_SizeBox.z, this.m_SizeBox.x, num, far, out this.m_ProjMat[1], out this.m_ViewMat[1]);
			pos = this.m_Center + Vector3.left * (this.m_SizeBox.x * 0.5f + 1f);
			rot = Quaternion.Euler(0f, 90f, 90f);
			far = num + this.m_SizeBox.x;
			this.m_WorldToClip[2] = this.ComputeOrthographicWorldToClip(pos, rot, this.m_SizeBox.y, this.m_SizeBox.z, num, far, out this.m_ProjMat[2], out this.m_ViewMat[2]);
		}

		private Matrix4x4 ComputeOrthographicWorldToClip(Vector3 pos, Quaternion rot, float width, float height, float near, float far, out Matrix4x4 proj, out Matrix4x4 view)
		{
			proj = Matrix4x4.Ortho(-width / 2f, width / 2f, -height / 2f, height / 2f, near, far);
			proj = GL.GetGPUProjectionMatrix(proj, false);
			view = Matrix4x4.TRS(pos, rot, new Vector3(1f, 1f, -1f)).inverse;
			return proj * view;
		}

		private int iDivUp(int a, int b)
		{
			if (a % b == 0)
			{
				return a / b;
			}
			return a / b + 1;
		}

		private Vector2Int GetThreadGroupsCount(int nbThreads, int threadCountPerGroup)
		{
			Vector2Int zero = Vector2Int.zero;
			int num = (nbThreads + threadCountPerGroup - 1) / threadCountPerGroup;
			zero.y = 1 + num / 65535;
			zero.x = num / zero.y;
			return zero;
		}

		private void PrefixSumCount()
		{
			int totalVoxelCount = this.GetTotalVoxelCount();
			this.m_Cmd.BeginSample("BakeSDF.PrefixSum");
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.numElem, totalVoxelCount);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.inBucketSum, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_CounterBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.inBucketSum, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_TmpBuffer);
			Vector2Int threadGroupsCount = this.GetThreadGroupsCount(totalVoxelCount, this.m_ThreadGroupSize);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.dispatchWidth, threadGroupsCount.x);
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.inBucketSum, threadGroupsCount.x, threadGroupsCount.y, 1);
			int num = this.iDivUp(totalVoxelCount, this.m_ThreadGroupSize);
			if (num > this.m_ThreadGroupSize)
			{
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.toBlockSumBuffer, MeshToSDFBaker.ShaderProperties.inputCounter, this.m_CounterBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.toBlockSumBuffer, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_TmpBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.toBlockSumBuffer, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_SumBlocksBuffer);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.toBlockSumBuffer, Mathf.CeilToInt((float)totalVoxelCount / (float)(this.m_ThreadGroupSize * this.m_ThreadGroupSize)), 1, 1);
				this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.numElem, num);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.inBucketSum, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_SumBlocksBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.inBucketSum, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_InSumBlocksBuffer);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.inBucketSum, Mathf.CeilToInt((float)totalVoxelCount / (float)(this.m_ThreadGroupSize * this.m_ThreadGroupSize)), 1, 1);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.blockSums, MeshToSDFBaker.ShaderProperties.inputCounter, this.m_SumBlocksBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.blockSums, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_InSumBlocksBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.blockSums, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_SumBlocksAdditional);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.blockSums, Mathf.CeilToInt((float)totalVoxelCount / (float)(this.m_ThreadGroupSize * this.m_ThreadGroupSize * this.m_ThreadGroupSize)), 1, 1);
				this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.exclusive, 0);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_InSumBlocksBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.auxBuffer, this.m_SumBlocksAdditional);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.inputCounter, this.m_SumBlocksBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_AccumSumBlocks);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.finalSum, Mathf.CeilToInt((float)totalVoxelCount / (float)(this.m_ThreadGroupSize * this.m_ThreadGroupSize)), 1, 1);
			}
			else
			{
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.blockSums, MeshToSDFBaker.ShaderProperties.inputCounter, this.m_CounterBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.blockSums, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_TmpBuffer);
				this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.blockSums, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_AccumSumBlocks);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.blockSums, Mathf.CeilToInt((float)totalVoxelCount / (float)(this.m_ThreadGroupSize * this.m_ThreadGroupSize)), 1, 1);
			}
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.numElem, totalVoxelCount);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.exclusive, 0);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.inputBuffer, this.m_TmpBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.auxBuffer, this.m_AccumSumBlocks);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.inputCounter, this.m_CounterBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.finalSum, MeshToSDFBaker.ShaderProperties.resultBuffer, this.m_AccumCounterBuffer);
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.finalSum, threadGroupsCount.x, threadGroupsCount.y, 1);
			this.m_Cmd.EndSample("BakeSDF.PrefixSum");
		}

		private void SurfaceClosing()
		{
			this.m_Cmd.BeginSample("BakeSDF.SurfaceClosing");
			if (this.m_SignPassesCount == 0)
			{
				this.m_InOutThreshold *= 6f;
			}
			this.m_Cmd.SetComputeFloatParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.threshold, this.m_InOutThreshold);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.surfaceClosing, MeshToSDFBaker.ShaderProperties.signMap, this.GetSignMapPrincipal(this.m_SignPassesCount));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.surfaceClosing, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.GetTextureVoxelPrincipal(0));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.surfaceClosing, this.iDivUp(this.m_Dimensions[0], 4), this.iDivUp(this.m_Dimensions[1], 4), this.iDivUp(this.m_Dimensions[2], 4));
			this.m_Cmd.EndSample("BakeSDF.SurfaceClosing");
		}

		private RenderTexture GetTextureVoxelPrincipal(int step)
		{
			if (step % 2 == 0)
			{
				return this.m_textureVoxel;
			}
			return this.m_textureVoxelBis;
		}

		private RenderTexture GetTextureVoxelBis(int step)
		{
			if (step % 2 == 0)
			{
				return this.m_textureVoxelBis;
			}
			return this.m_textureVoxel;
		}

		private void JFA()
		{
			this.m_Cmd.BeginSample("BakeSDF.JFA");
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.toTextureNormalized, MeshToSDFBaker.ShaderProperties.voxelsBuffer, this.m_bufferVoxel);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.toTextureNormalized, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.GetTextureVoxelPrincipal(0));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.toTextureNormalized, Mathf.CeilToInt((float)this.m_Dimensions[0] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 4f));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.jfa, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.GetTextureVoxelPrincipal(0), 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.jfa, MeshToSDFBaker.ShaderProperties.voxelsTmpTexture, this.GetTextureVoxelBis(0), 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.copyTextures, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.GetTextureVoxelPrincipal(0), 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.copyTextures, MeshToSDFBaker.ShaderProperties.voxelsTmpTexture, this.GetTextureVoxelBis(0), 0);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.offset, 1);
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.jfa, Mathf.CeilToInt((float)this.m_Dimensions[0] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 4f));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.copyTextures, Mathf.CeilToInt((float)this.m_Dimensions[0] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 4f));
			this.m_nStepsJFA = Mathf.CeilToInt(Mathf.Log((float)this.m_maxResolution, 2f));
			for (int i = 1; i <= this.m_nStepsJFA; i++)
			{
				int val = Mathf.FloorToInt(Mathf.Pow(2f, (float)(this.m_nStepsJFA - i)) + 0.5f);
				this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.offset, val);
				this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.jfa, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.GetTextureVoxelPrincipal(i), 0);
				this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.jfa, MeshToSDFBaker.ShaderProperties.voxelsTmpTexture, this.GetTextureVoxelBis(i), 0);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.jfa, Mathf.CeilToInt((float)this.m_Dimensions[0] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 4f));
			}
			this.m_Cmd.EndSample("BakeSDF.JFA");
		}

		private void GenerateRayMap()
		{
			this.m_RayMapUseCounter = 0;
			this.m_Cmd.BeginSample("BakeSDF.Raymap");
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.generateRayMapLocal, MeshToSDFBaker.ShaderProperties.accumCounter, this.m_AccumCounterBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.generateRayMapLocal, MeshToSDFBaker.ShaderProperties.triangleIDs, this.m_TrianglesInVoxels);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.generateRayMapLocal, MeshToSDFBaker.ShaderProperties.trianglesUV, this.m_TrianglesUV);
			this.m_Cmd.BeginSample("BakeSDF.LocalRaymap");
			for (int i = 0; i < 8; i++)
			{
				this.m_OffsetRayMap[0] = (i & 1);
				this.m_OffsetRayMap[1] = (i & 2) >> 1;
				this.m_OffsetRayMap[2] = (i & 4) >> 2;
				this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.generateRayMapLocal, MeshToSDFBaker.ShaderProperties.rayMap, this.GetRayMapPrincipal(this.m_RayMapUseCounter));
				this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.generateRayMapLocal, MeshToSDFBaker.ShaderProperties.rayMapTmp, this.GetRayMapBis(this.m_RayMapUseCounter));
				this.m_Cmd.SetComputeIntParams(this.m_computeShader, MeshToSDFBaker.ShaderProperties.offsetRayMap, this.m_OffsetRayMap);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.generateRayMapLocal, Mathf.CeilToInt((float)this.m_Dimensions[0] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 8f));
				this.m_RayMapUseCounter++;
			}
			this.m_Cmd.EndSample("BakeSDF.LocalRaymap");
			this.m_Cmd.BeginSample("BakeSDF.GlobalRaymap");
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.rayMapScanX, MeshToSDFBaker.ShaderProperties.rayMap, this.GetRayMapPrincipal(this.m_RayMapUseCounter));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.rayMapScanX, MeshToSDFBaker.ShaderProperties.rayMapTmp, this.GetRayMapBis(this.m_RayMapUseCounter));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.rayMapScanX, 1, Mathf.CeilToInt((float)this.m_Dimensions[1] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 8f));
			this.m_RayMapUseCounter++;
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.rayMapScanY, MeshToSDFBaker.ShaderProperties.rayMap, this.GetRayMapPrincipal(this.m_RayMapUseCounter));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.rayMapScanY, MeshToSDFBaker.ShaderProperties.rayMapTmp, this.GetRayMapBis(this.m_RayMapUseCounter));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.rayMapScanY, Mathf.CeilToInt((float)this.m_Dimensions[0] / 8f), 1, Mathf.CeilToInt((float)this.m_Dimensions[2] / 8f));
			this.m_RayMapUseCounter++;
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.rayMapScanZ, MeshToSDFBaker.ShaderProperties.rayMap, this.GetRayMapPrincipal(this.m_RayMapUseCounter));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.rayMapScanZ, MeshToSDFBaker.ShaderProperties.rayMapTmp, this.GetRayMapBis(this.m_RayMapUseCounter));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.rayMapScanZ, Mathf.CeilToInt((float)this.m_Dimensions[0] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 8f), 1);
			this.m_Cmd.EndSample("BakeSDF.GlobalRaymap");
			this.m_Cmd.EndSample("BakeSDF.Raymap");
		}

		private RenderTexture GetRayMapPrincipal(int step)
		{
			return this.m_RayMaps[step % 2];
		}

		private RenderTexture GetRayMapBis(int step)
		{
			return this.m_RayMaps[(step + 1) % 2];
		}

		private RenderTexture GetSignMapPrincipal(int step)
		{
			return this.m_SignMaps[step % 2];
		}

		private RenderTexture GetSignMapBis(int step)
		{
			return this.m_SignMaps[(step + 1) % 2];
		}

		private void SignPass()
		{
			this.m_Cmd.BeginSample("BakeSDF.SignPass");
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.signPass6Rays, MeshToSDFBaker.ShaderProperties.rayMap, this.GetRayMapPrincipal(this.m_RayMapUseCounter));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.signPass6Rays, MeshToSDFBaker.ShaderProperties.signMap, this.GetSignMapPrincipal(0));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.signPass6Rays, Mathf.CeilToInt((float)this.m_Dimensions[0] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 4f));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.signPassNeighbors, MeshToSDFBaker.ShaderProperties.rayMap, this.GetRayMapPrincipal(this.m_RayMapUseCounter));
			int num = 8;
			float num2 = 6f;
			this.m_Cmd.SetComputeFloatParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.normalizeFactor, num2);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.numNeighbours, num);
			int signPassesCount = this.m_SignPassesCount;
			for (int i = 1; i <= signPassesCount; i++)
			{
				this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.passId, i);
				this.m_Cmd.SetComputeFloatParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.normalizeFactor, num2);
				this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.signPassNeighbors, MeshToSDFBaker.ShaderProperties.signMap, this.GetSignMapPrincipal(i));
				this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.signPassNeighbors, MeshToSDFBaker.ShaderProperties.signMapTmp, this.GetSignMapBis(i));
				this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.needNormalize, (i == signPassesCount) ? 1 : 0);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.signPassNeighbors, Mathf.CeilToInt((float)this.m_Dimensions[0] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 4f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 4f));
				num2 += (float)(num * 6) * num2;
			}
			this.m_Cmd.EndSample("BakeSDF.SignPass");
		}

		public void BakeSDF()
		{
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
			{
				throw new NotSupportedException("MeshToSDFBaker compute shaders are not supported on OpenGLES3");
			}
			this.m_Cmd.BeginSample("BakeSDF");
			this.UpdateCameras();
			this.m_Cmd.SetComputeIntParams(this.m_computeShader, MeshToSDFBaker.ShaderProperties.size, this.m_Dimensions);
			this.CreateGraphicsBufferIfNeeded(ref this.m_bufferVoxel, this.GetTotalVoxelCount(), 16);
			this.InitPrefixSumBuffers();
			this.InitMeshBuffers();
			int num = (int)Mathf.Pow((float)this.m_maxResolution, 2f) * (int)Mathf.Pow((float)this.nTriangles, 0.5f);
			num = (int)Mathf.Max((float)((long)this.nTriangles * 30L), (float)num);
			num = Mathf.Min(402653184, num);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.upperBoundCount, num);
			this.ClearRenderTexturesAndBuffers();
			this.InitGeometryBuffers(num);
			this.BuildGeometry();
			this.FirstDraw();
			this.PrefixSumCount();
			this.SecondDraw();
			this.GenerateRayMap();
			this.SignPass();
			this.SurfaceClosing();
			this.JFA();
			this.PerformDistanceTransformWinding();
			this.m_Cmd.EndSample("BakeSDF");
			if (this.m_OwnsCommandBuffer)
			{
				this.m_Cmd.ClearRandomWriteTargets();
				Graphics.ExecuteCommandBuffer(this.m_Cmd);
				this.m_Cmd.Clear();
			}
		}

		private void InitMeshBuffers()
		{
			if (this.m_Mesh.GetVertexAttributeFormat(VertexAttribute.Position) != VertexAttributeFormat.Float32)
			{
				throw new ArgumentException("The SDF Baker only supports the VertexAttributeFormat Float32 for the Position attribute.");
			}
			int vertexAttributeStream = this.m_Mesh.GetVertexAttributeStream(VertexAttribute.Position);
			this.m_VertexBufferOffset = this.m_Mesh.GetVertexAttributeOffset(VertexAttribute.Position);
			GraphicsBuffer verticesBuffer = this.m_VerticesBuffer;
			if (verticesBuffer != null)
			{
				verticesBuffer.Dispose();
			}
			GraphicsBuffer indicesBuffer = this.m_IndicesBuffer;
			if (indicesBuffer != null)
			{
				indicesBuffer.Dispose();
			}
			this.m_VerticesBuffer = this.m_Mesh.GetVertexBuffer(vertexAttributeStream);
			this.m_IndicesBuffer = this.m_Mesh.GetIndexBuffer();
			this.nTriangles = 0;
			for (int i = 0; i < this.m_Mesh.subMeshCount; i++)
			{
				this.nTriangles += this.m_Mesh.GetSubMesh(i).indexCount;
			}
			this.nTriangles /= 3;
		}

		private void FirstDraw()
		{
			this.m_Cmd.BeginSample("BakeSDF.FirstDraw");
			for (int i = 0; i < 3; i++)
			{
				this.m_Material[i].SetInt(MeshToSDFBaker.ShaderProperties.dimX, this.m_Dimensions[0]);
				this.m_Material[i].SetInt(MeshToSDFBaker.ShaderProperties.dimY, this.m_Dimensions[1]);
				this.m_Material[i].SetInt(MeshToSDFBaker.ShaderProperties.dimZ, this.m_Dimensions[2]);
				this.m_Material[i].SetInt(MeshToSDFBaker.ShaderProperties.currentAxis, i);
				this.m_Material[i].SetBuffer(MeshToSDFBaker.ShaderProperties.verticesBuffer, this.m_VerticesOutBuffer);
				this.m_Material[i].SetBuffer(MeshToSDFBaker.ShaderProperties.coordFlipBuffer, this.m_CoordFlipBuffer);
			}
			for (int j = 0; j < 3; j++)
			{
				this.m_Cmd.ClearRandomWriteTargets();
				this.m_Cmd.SetRenderTarget(this.m_RenderTextureViews[j]);
				this.m_Cmd.ClearRenderTarget(true, true, Color.black, 1f);
				this.m_Cmd.SetRandomWriteTarget(4 + MeshToSDFBaker.kNbActualRT, this.m_AabbBuffer, false);
				this.m_Cmd.SetRandomWriteTarget(1 + MeshToSDFBaker.kNbActualRT, this.m_bufferVoxel, false);
				this.m_Cmd.SetRandomWriteTarget(2 + MeshToSDFBaker.kNbActualRT, this.m_CounterBuffer, false);
				this.m_Cmd.SetViewProjectionMatrices(this.m_ViewMat[j], this.m_ProjMat[j]);
				this.m_Cmd.DrawProcedural(Matrix4x4.identity, this.m_Material[j], 0, MeshTopology.Triangles, this.nTriangles * 3);
			}
			this.m_Cmd.ClearRandomWriteTargets();
			this.m_Cmd.EndSample("BakeSDF.FirstDraw");
		}

		private void SecondDraw()
		{
			this.m_Cmd.BeginSample("BakeSDF.SecondDraw");
			for (int i = 0; i < 3; i++)
			{
				this.m_Cmd.ClearRandomWriteTargets();
				this.m_Cmd.SetRenderTarget(this.m_RenderTextureViews[i]);
				this.m_Cmd.ClearRenderTarget(true, true, Color.black, 1f);
				this.m_Cmd.SetRandomWriteTarget(4 + MeshToSDFBaker.kNbActualRT, this.m_AabbBuffer, false);
				this.m_Cmd.SetRandomWriteTarget(3 + MeshToSDFBaker.kNbActualRT, this.m_TrianglesInVoxels, false);
				this.m_Cmd.SetRandomWriteTarget(2 + MeshToSDFBaker.kNbActualRT, this.m_AccumCounterBuffer, false);
				this.m_Cmd.SetViewProjectionMatrices(this.m_ViewMat[i], this.m_ProjMat[i]);
				this.m_Cmd.DrawProcedural(Matrix4x4.identity, this.m_Material[i], 1, MeshTopology.Triangles, this.nTriangles * 3);
			}
			this.m_Cmd.ClearRandomWriteTargets();
			this.m_Cmd.EndSample("BakeSDF.SecondDraw");
		}

		private void BuildGeometry()
		{
			this.m_Cmd.BeginSample("BakeSDF.FakeGeometryShader");
			Vector3 vector = this.m_Center - this.m_SizeBox * 0.5f;
			Vector3 vector2 = this.m_Center + this.m_SizeBox * 0.5f;
			for (int i = 0; i < 3; i++)
			{
				this.m_MinBoundsExtended[i] = vector[i];
				this.m_MaxBoundsExtended[i] = vector2[i];
			}
			this.m_Cmd.SetComputeFloatParams(this.m_computeShader, MeshToSDFBaker.ShaderProperties.minBoundsExtended, this.m_MinBoundsExtended);
			this.m_Cmd.SetComputeFloatParams(this.m_computeShader, MeshToSDFBaker.ShaderProperties.maxBoundsExtended, this.m_MaxBoundsExtended);
			this.m_Cmd.SetComputeFloatParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.maxExtent, this.m_MaxExtent);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.nTriangles, this.nTriangles);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.vertexPositionOffset, this.m_VertexBufferOffset);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.vertexStride, this.m_VerticesBuffer.stride);
			this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.indexStride, this.m_IndicesBuffer.stride);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.chooseDirectionTriangleOnly, MeshToSDFBaker.ShaderProperties.indicesBuffer, this.m_IndicesBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.chooseDirectionTriangleOnly, MeshToSDFBaker.ShaderProperties.verticesBuffer, this.m_VerticesBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.chooseDirectionTriangleOnly, MeshToSDFBaker.ShaderProperties.coordFlipBuffer, this.m_CoordFlipBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.conservativeRasterization, MeshToSDFBaker.ShaderProperties.indicesBuffer, this.m_IndicesBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.conservativeRasterization, MeshToSDFBaker.ShaderProperties.verticesBuffer, this.m_VerticesBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.conservativeRasterization, MeshToSDFBaker.ShaderProperties.verticesOutBuffer, this.m_VerticesOutBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.conservativeRasterization, MeshToSDFBaker.ShaderProperties.coordFlipBuffer, this.m_CoordFlipBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.conservativeRasterization, MeshToSDFBaker.ShaderProperties.aabbBuffer, this.m_AabbBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.generateTrianglesUV, MeshToSDFBaker.ShaderProperties.rw_trianglesUV, this.m_TrianglesUV);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.generateTrianglesUV, MeshToSDFBaker.ShaderProperties.indicesBuffer, this.m_IndicesBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.generateTrianglesUV, MeshToSDFBaker.ShaderProperties.verticesBuffer, this.m_VerticesBuffer);
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.generateTrianglesUV, Mathf.CeilToInt((float)this.nTriangles / 64f), 1, 1);
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.chooseDirectionTriangleOnly, Mathf.CeilToInt((float)this.nTriangles / 64f), 1, 1);
			for (int j = 0; j < 3; j++)
			{
				this.m_Cmd.SetComputeIntParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.currentAxis, j);
				this.m_Cmd.SetComputeMatrixParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.worldToClip, this.m_WorldToClip[j]);
				this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.conservativeRasterization, Mathf.CeilToInt((float)this.nTriangles / 64f), 1, 1);
			}
			this.m_Cmd.EndSample("BakeSDF.FakeGeometryShader");
		}

		private void InitGeometryBuffers(int upperBoundCount)
		{
			this.CreateGraphicsBufferIfNeeded(ref this.m_VerticesOutBuffer, 3 * this.nTriangles, 16);
			this.CreateGraphicsBufferIfNeeded(ref this.m_CoordFlipBuffer, this.nTriangles, 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_AabbBuffer, this.nTriangles, 16);
			this.CreateGraphicsBufferIfNeeded(ref this.m_TrianglesInVoxels, upperBoundCount, 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_TrianglesUV, this.nTriangles, 36);
		}

		private void InitPrefixSumBuffers()
		{
			this.CreateGraphicsBufferIfNeeded(ref this.m_CounterBuffer, this.GetTotalVoxelCount(), 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_AccumCounterBuffer, this.GetTotalVoxelCount(), 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_AccumSumBlocks, Mathf.CeilToInt((float)this.GetTotalVoxelCount() / (float)this.m_ThreadGroupSize), 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_SumBlocksBuffer, Mathf.CeilToInt((float)this.GetTotalVoxelCount() / (float)this.m_ThreadGroupSize), 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_InSumBlocksBuffer, Mathf.CeilToInt((float)this.GetTotalVoxelCount() / (float)this.m_ThreadGroupSize), 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_TmpBuffer, this.GetTotalVoxelCount(), 4);
			this.CreateGraphicsBufferIfNeeded(ref this.m_SumBlocksAdditional, Mathf.CeilToInt((float)this.GetTotalVoxelCount() / (float)(this.m_ThreadGroupSize * this.m_ThreadGroupSize)), 4);
		}

		private void ClearRenderTexturesAndBuffers()
		{
			this.m_Cmd.BeginSample("BakeSDF.ClearTexturesAndBuffers");
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.m_textureVoxel, 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.voxelsTmpTexture, this.m_textureVoxelBis, 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.rayMap, this.m_RayMaps[0], 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.rw_rayMapTmp, this.m_RayMaps[1], 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.signMap, this.m_SignMaps[0], 0);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.signMapTmp, this.m_SignMaps[1]);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.voxelsBuffer, this.m_bufferVoxel);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.counter, this.m_CounterBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, MeshToSDFBaker.ShaderProperties.accumCounter, this.m_AccumCounterBuffer);
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.clearTexturesAndBuffers, Mathf.CeilToInt((float)this.m_Dimensions[0] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 8f));
			this.m_Cmd.EndSample("BakeSDF.ClearTexturesAndBuffers");
		}

		private void PerformDistanceTransformWinding()
		{
			this.m_Cmd.BeginSample("BakeSDF.DistanceTransform");
			this.m_Cmd.SetComputeFloatParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.threshold, this.m_InOutThreshold);
			this.m_Cmd.SetComputeFloatParam(this.m_computeShader, MeshToSDFBaker.ShaderProperties.sdfOffset, this.m_SdfOffset);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.distanceTransform, MeshToSDFBaker.ShaderProperties.voxelsTexture, this.GetTextureVoxelPrincipal(this.m_nStepsJFA + 1));
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.distanceTransform, MeshToSDFBaker.ShaderProperties.distanceTexture, this.m_DistanceTexture);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.distanceTransform, MeshToSDFBaker.ShaderProperties.accumCounter, this.m_AccumCounterBuffer);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.distanceTransform, MeshToSDFBaker.ShaderProperties.triangleIDs, this.m_TrianglesInVoxels);
			this.m_Cmd.SetComputeBufferParam(this.m_computeShader, this.m_Kernels.distanceTransform, MeshToSDFBaker.ShaderProperties.trianglesUV, this.m_TrianglesUV);
			this.m_Cmd.SetComputeTextureParam(this.m_computeShader, this.m_Kernels.distanceTransform, MeshToSDFBaker.ShaderProperties.signMap, this.GetSignMapPrincipal(this.m_SignPassesCount));
			this.m_Cmd.DispatchCompute(this.m_computeShader, this.m_Kernels.distanceTransform, Mathf.CeilToInt((float)this.m_Dimensions[0] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[1] / 8f), Mathf.CeilToInt((float)this.m_Dimensions[2] / 8f));
			this.m_Cmd.EndSample("BakeSDF.DistanceTransform");
		}

		private void ReleaseBuffersAndTextures()
		{
			this.ReleaseRenderTexture(ref this.m_textureVoxel);
			this.ReleaseRenderTexture(ref this.m_textureVoxelBis);
			this.ReleaseRenderTexture(ref this.m_DistanceTexture);
			for (int i = 0; i < 3; i++)
			{
				this.ReleaseRenderTexture(ref this.m_RenderTextureViews[i]);
				if (Application.isPlaying)
				{
					Object.Destroy(this.m_Material[i]);
				}
				else
				{
					Object.DestroyImmediate(this.m_Material[i]);
				}
			}
			for (int j = 0; j < 2; j++)
			{
				this.ReleaseRenderTexture(ref this.m_SignMaps[j]);
				this.ReleaseRenderTexture(ref this.m_RayMaps[j]);
			}
			this.ReleaseGraphicsBuffer(ref this.m_bufferVoxel);
			this.ReleaseGraphicsBuffer(ref this.m_TrianglesUV);
			this.ReleaseGraphicsBuffer(ref this.m_TrianglesInVoxels);
			this.ReleaseGraphicsBuffer(ref this.m_IndicesBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_VerticesBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_VerticesOutBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_CoordFlipBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_AabbBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_TmpBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_AccumSumBlocks);
			this.ReleaseGraphicsBuffer(ref this.m_SumBlocksBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_InSumBlocksBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_SumBlocksAdditional);
			this.ReleaseGraphicsBuffer(ref this.m_CounterBuffer);
			this.ReleaseGraphicsBuffer(ref this.m_AccumCounterBuffer);
		}

		public void Dispose()
		{
			this.ReleaseBuffersAndTextures();
			GC.SuppressFinalize(this);
			this.m_IsDisposed = true;
		}

		private void CreateGraphicsBufferIfNeeded(ref GraphicsBuffer gb, int length, int stride)
		{
			if (gb != null && gb.count == length && gb.stride == stride)
			{
				return;
			}
			this.ReleaseGraphicsBuffer(ref gb);
			gb = new GraphicsBuffer(GraphicsBuffer.Target.Structured, length, stride);
			this.m_IsDisposed = false;
		}

		private void ReleaseGraphicsBuffer(ref GraphicsBuffer gb)
		{
			if (gb != null)
			{
				gb.Release();
			}
			gb = null;
		}

		private void CreateRenderTextureIfNeeded(ref RenderTexture rt, RenderTextureDescriptor rtDesc)
		{
			if (rt != null && rt.width == rtDesc.width && rt.height == rtDesc.height && rt.volumeDepth == rtDesc.volumeDepth && rt.graphicsFormat == rtDesc.graphicsFormat)
			{
				return;
			}
			this.ReleaseRenderTexture(ref rt);
			rt = new RenderTexture(rtDesc);
			rt.hideFlags = HideFlags.DontSave;
			rt.Create();
			this.m_IsDisposed = false;
		}

		private void ReleaseRenderTexture(ref RenderTexture rt)
		{
			if (rt != null)
			{
				rt.Release();
				if (Application.isPlaying)
				{
					Object.Destroy(rt);
				}
				else
				{
					Object.DestroyImmediate(rt);
				}
			}
			rt = null;
		}

		private RenderTexture[] m_RayMaps;

		private RenderTexture[] m_SignMaps;

		private RenderTexture[] m_RenderTextureViews;

		private GraphicsBuffer m_CounterBuffer;

		private GraphicsBuffer m_AccumCounterBuffer;

		private GraphicsBuffer m_TrianglesInVoxels;

		private GraphicsBuffer m_TrianglesUV;

		private GraphicsBuffer m_TmpBuffer;

		private GraphicsBuffer m_AccumSumBlocks;

		private GraphicsBuffer m_SumBlocksBuffer;

		private GraphicsBuffer m_InSumBlocksBuffer;

		private GraphicsBuffer m_SumBlocksAdditional;

		private GraphicsBuffer m_IndicesBuffer;

		private GraphicsBuffer m_VerticesBuffer;

		private GraphicsBuffer m_VerticesOutBuffer;

		private GraphicsBuffer m_CoordFlipBuffer;

		private GraphicsBuffer m_AabbBuffer;

		private int m_VertexBufferOffset;

		private int m_ThreadGroupSize = 512;

		private int m_SignPassesCount;

		private float m_InOutThreshold;

		private Material[] m_Material;

		private Matrix4x4[] m_WorldToClip;

		private Matrix4x4[] m_ProjMat;

		private Matrix4x4[] m_ViewMat;

		private int m_nStepsJFA;

		private MeshToSDFBaker.Kernels m_Kernels;

		private Mesh m_Mesh;

		private RenderTexture m_textureVoxel;

		private RenderTexture m_textureVoxelBis;

		private RenderTexture m_DistanceTexture;

		private GraphicsBuffer m_bufferVoxel;

		private ComputeShader m_computeShader;

		private int m_maxResolution;

		private float m_MaxExtent;

		private float m_SdfOffset;

		private int nTriangles;

		private Vector3 m_SizeBox;

		private Vector3 m_Center;

		private CommandBuffer m_Cmd;

		private bool m_OwnsCommandBuffer = true;

		private bool m_IsDisposed;

		private int[] m_Dimensions = new int[3];

		private int[] m_OffsetRayMap = new int[3];

		private float[] m_MinBoundsExtended = new float[3];

		private float[] m_MaxBoundsExtended = new float[3];

		private int m_RayMapUseCounter;

		internal static uint kMaxRecommandedGridSize = 16777216U;

		internal static uint kMaxAbsoluteGridSize = 134217728U;

		private static int kNbActualRT = 0;

		internal VFXRuntimeResources m_RuntimeResources;

		private static class ShaderProperties
		{
			internal static int indicesBuffer = Shader.PropertyToID("indices");

			internal static int verticesBuffer = Shader.PropertyToID("vertices");

			internal static int vertexPositionOffset = Shader.PropertyToID("vertexPositionOffset");

			internal static int vertexStride = Shader.PropertyToID("vertexStride");

			internal static int indexStride = Shader.PropertyToID("indexStride");

			internal static int coordFlipBuffer = Shader.PropertyToID("coordFlip");

			internal static int verticesOutBuffer = Shader.PropertyToID("verticesOut");

			internal static int aabbBuffer = Shader.PropertyToID("aabb");

			internal static int worldToClip = Shader.PropertyToID("worldToClip");

			internal static int currentAxis = Shader.PropertyToID("currentAxis");

			internal static int voxelsBuffer = Shader.PropertyToID("voxelsBuffer");

			internal static int rw_trianglesUV = Shader.PropertyToID("rw_trianglesUV");

			internal static int trianglesUV = Shader.PropertyToID("trianglesUV");

			internal static int voxelsTexture = Shader.PropertyToID("voxels");

			internal static int voxelsTmpTexture = Shader.PropertyToID("voxelsTmp");

			internal static int rayMap = Shader.PropertyToID("rayMap");

			internal static int rayMapTmp = Shader.PropertyToID("rayMapTmp");

			internal static int rw_rayMapTmp = Shader.PropertyToID("rw_rayMapTmp");

			internal static int nTriangles = Shader.PropertyToID("nTriangles");

			internal static int minBoundsExtended = Shader.PropertyToID("minBoundsExtended");

			internal static int maxBoundsExtended = Shader.PropertyToID("maxBoundsExtended");

			internal static int maxExtent = Shader.PropertyToID("maxExtent");

			internal static int upperBoundCount = Shader.PropertyToID("upperBoundCount");

			internal static int counter = Shader.PropertyToID("counter");

			internal static int dimX = Shader.PropertyToID("dimX");

			internal static int dimY = Shader.PropertyToID("dimY");

			internal static int dimZ = Shader.PropertyToID("dimZ");

			internal static int size = Shader.PropertyToID("size");

			internal static int inputBuffer = Shader.PropertyToID("Input");

			internal static int inputCounter = Shader.PropertyToID("inputCounter");

			internal static int auxBuffer = Shader.PropertyToID("auxBuffer");

			internal static int resultBuffer = Shader.PropertyToID("Result");

			internal static int numElem = Shader.PropertyToID("numElem");

			internal static int exclusive = Shader.PropertyToID("exclusive");

			internal static int dispatchWidth = Shader.PropertyToID("dispatchWidth");

			internal static int src = Shader.PropertyToID("src");

			internal static int dest = Shader.PropertyToID("dest");

			internal static int signMap = Shader.PropertyToID("signMap");

			internal static int threshold = Shader.PropertyToID("threshold");

			internal static int signMapTmp = Shader.PropertyToID("signMapTmp");

			internal static int normalizeFactor = Shader.PropertyToID("normalizeFactor");

			internal static int numNeighbours = Shader.PropertyToID("numNeighbours");

			internal static int passId = Shader.PropertyToID("passId");

			internal static int needNormalize = Shader.PropertyToID("needNormalize");

			internal static int offset = Shader.PropertyToID("offset");

			internal static int offsetRayMap = Shader.PropertyToID("offsetRayMap");

			internal static int triangleIDs = Shader.PropertyToID("triangleIDs");

			internal static int accumCounter = Shader.PropertyToID("accumCounter");

			internal static int distanceTexture = Shader.PropertyToID("distanceTexture");

			internal static int sdfOffset = Shader.PropertyToID("sdfOffset");
		}

		internal class Kernels
		{
			internal Kernels(ComputeShader computeShader)
			{
				this.inBucketSum = computeShader.FindKernel("InBucketSum");
				this.blockSums = computeShader.FindKernel("BlockSums");
				this.finalSum = computeShader.FindKernel("FinalSum");
				this.toTextureNormalized = computeShader.FindKernel("ToTextureNormalized");
				this.copyTextures = computeShader.FindKernel("CopyTextures");
				this.jfa = computeShader.FindKernel("JFA");
				this.distanceTransform = computeShader.FindKernel("DistanceTransform");
				this.copyBuffers = computeShader.FindKernel("CopyBuffers");
				this.generateRayMapLocal = computeShader.FindKernel("GenerateRayMapLocal");
				this.rayMapScanX = computeShader.FindKernel("RayMapScanX");
				this.rayMapScanY = computeShader.FindKernel("RayMapScanY");
				this.rayMapScanZ = computeShader.FindKernel("RayMapScanZ");
				this.signPass6Rays = computeShader.FindKernel("SignPass6Rays");
				this.signPassNeighbors = computeShader.FindKernel("SignPassNeighbors");
				this.toBlockSumBuffer = computeShader.FindKernel("ToBlockSumBuffer");
				this.clearTexturesAndBuffers = computeShader.FindKernel("ClearTexturesAndBuffers");
				this.copyToBuffer = computeShader.FindKernel("CopyToBuffer");
				this.generateTrianglesUV = computeShader.FindKernel("GenerateTrianglesUV");
				this.conservativeRasterization = computeShader.FindKernel("ConservativeRasterization");
				this.chooseDirectionTriangleOnly = computeShader.FindKernel("ChooseDirectionTriangleOnly");
				this.surfaceClosing = computeShader.FindKernel("SurfaceClosing");
			}

			internal int inBucketSum = -1;

			internal int blockSums = -1;

			internal int finalSum = -1;

			internal int toTextureNormalized = -1;

			internal int copyTextures = -1;

			internal int jfa = -1;

			internal int distanceTransform = -1;

			internal int copyBuffers = -1;

			internal int generateRayMapLocal = -1;

			internal int rayMapScanX = -1;

			internal int rayMapScanY = -1;

			internal int rayMapScanZ = -1;

			internal int signPass6Rays = -1;

			internal int signPassNeighbors = -1;

			internal int toBlockSumBuffer = -1;

			internal int clearTexturesAndBuffers = -1;

			internal int copyToBuffer = -1;

			internal int generateTrianglesUV = -1;

			internal int conservativeRasterization = -1;

			internal int chooseDirectionTriangleOnly = -1;

			internal int surfaceClosing = -1;
		}
	}
}
