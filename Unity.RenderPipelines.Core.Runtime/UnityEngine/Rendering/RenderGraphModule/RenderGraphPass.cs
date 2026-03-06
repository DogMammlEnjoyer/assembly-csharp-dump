using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule
{
	[DebuggerDisplay("RenderPass: {name} (Index:{index} Async:{enableAsyncCompute})")]
	internal abstract class RenderGraphPass
	{
		public abstract void Execute(InternalRenderGraphContext renderGraphContext);

		public abstract void Release(RenderGraphObjectPool pool);

		public abstract bool HasRenderFunc();

		public abstract int GetRenderFuncHash();

		public string name { get; protected set; }

		public int index { get; protected set; }

		public RenderGraphPassType type { get; internal set; }

		public ProfilingSampler customSampler { get; protected set; }

		public bool enableAsyncCompute { get; protected set; }

		public bool allowPassCulling { get; protected set; }

		public bool allowGlobalState { get; protected set; }

		public bool enableFoveatedRasterization { get; protected set; }

		public TextureAccess depthAccess { get; protected set; }

		public TextureAccess[] colorBufferAccess { get; protected set; } = new TextureAccess[RenderGraph.kMaxMRTCount];

		public int colorBufferMaxIndex { get; protected set; } = -1;

		public bool hasShadingRateImage { get; protected set; }

		public TextureAccess shadingRateAccess { get; protected set; }

		public bool hasShadingRateStates { get; protected set; }

		public ShadingRateFragmentSize shadingRateFragmentSize { get; protected set; }

		public ShadingRateCombiner primitiveShadingRateCombiner { get; protected set; }

		public ShadingRateCombiner fragmentShadingRateCombiner { get; protected set; }

		public TextureAccess[] fragmentInputAccess { get; protected set; } = new TextureAccess[RenderGraph.kMaxMRTCount];

		public int fragmentInputMaxIndex { get; protected set; } = -1;

		public RenderGraphPass.RandomWriteResourceInfo[] randomAccessResource { get; protected set; } = new RenderGraphPass.RandomWriteResourceInfo[RenderGraph.kMaxMRTCount];

		public int randomAccessResourceMaxIndex { get; protected set; } = -1;

		public bool generateDebugData { get; protected set; }

		public bool allowRendererListCulling { get; protected set; }

		public RenderGraphPass()
		{
			for (int i = 0; i < 3; i++)
			{
				this.resourceReadLists[i] = new List<ResourceHandle>();
				this.resourceWriteLists[i] = new List<ResourceHandle>();
				this.transientResourceList[i] = new List<ResourceHandle>();
			}
		}

		public void Clear()
		{
			this.name = "";
			this.index = -1;
			this.customSampler = null;
			for (int i = 0; i < 3; i++)
			{
				this.resourceReadLists[i].Clear();
				this.resourceWriteLists[i].Clear();
				this.transientResourceList[i].Clear();
			}
			this.usedRendererListList.Clear();
			this.setGlobalsList.Clear();
			this.useAllGlobalTextures = false;
			this.implicitReadsList.Clear();
			this.enableAsyncCompute = false;
			this.allowPassCulling = true;
			this.allowRendererListCulling = true;
			this.allowGlobalState = false;
			this.enableFoveatedRasterization = false;
			this.generateDebugData = true;
			this.colorBufferMaxIndex = -1;
			this.fragmentInputMaxIndex = -1;
			this.randomAccessResourceMaxIndex = -1;
			this.depthAccess = default(TextureAccess);
			this.hasShadingRateImage = false;
			this.hasShadingRateStates = false;
			this.shadingRateFragmentSize = ShadingRateFragmentSize.FragmentSize1x1;
			this.primitiveShadingRateCombiner = ShadingRateCombiner.Keep;
			this.fragmentShadingRateCombiner = ShadingRateCombiner.Keep;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasRenderAttachments()
		{
			return this.depthAccess.textureHandle.IsValid() || this.colorBufferAccess[0].textureHandle.IsValid() || this.colorBufferMaxIndex > 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsTransient(in ResourceHandle res)
		{
			for (int i = 0; i < this.transientResourceList[res.iType].Count; i++)
			{
				if (this.transientResourceList[res.iType][i].index == res.index)
				{
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsWritten(in ResourceHandle res)
		{
			for (int i = 0; i < this.resourceWriteLists[res.iType].Count; i++)
			{
				if (this.resourceWriteLists[res.iType][i].index == res.index)
				{
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsRead(in ResourceHandle res)
		{
			if (res.IsVersioned)
			{
				return this.resourceReadLists[res.iType].Contains(res);
			}
			for (int i = 0; i < this.resourceReadLists[res.iType].Count; i++)
			{
				if (this.resourceReadLists[res.iType][i].index == res.index)
				{
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAttachment(in TextureHandle res)
		{
			if (this.depthAccess.textureHandle.IsValid() && this.depthAccess.textureHandle.handle.index == res.handle.index)
			{
				return true;
			}
			for (int i = 0; i < this.colorBufferAccess.Length; i++)
			{
				if (this.colorBufferAccess[i].textureHandle.IsValid() && this.colorBufferAccess[i].textureHandle.handle.index == res.handle.index)
				{
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddResourceWrite(in ResourceHandle res)
		{
			this.resourceWriteLists[res.iType].Add(res);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddResourceRead(in ResourceHandle res)
		{
			this.resourceReadLists[res.iType].Add(res);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddTransientResource(in ResourceHandle res)
		{
			this.transientResourceList[res.iType].Add(res);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UseRendererList(in RendererListHandle rendererList)
		{
			this.usedRendererListList.Add(rendererList);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnableAsyncCompute(bool value)
		{
			this.enableAsyncCompute = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AllowPassCulling(bool value)
		{
			this.allowPassCulling = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnableFoveatedRasterization(bool value)
		{
			this.enableFoveatedRasterization = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AllowRendererListCulling(bool value)
		{
			this.allowRendererListCulling = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AllowGlobalState(bool value)
		{
			this.allowGlobalState = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GenerateDebugData(bool value)
		{
			this.generateDebugData = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetColorBuffer(in TextureHandle resource, int index)
		{
			this.colorBufferMaxIndex = Math.Max(this.colorBufferMaxIndex, index);
			this.colorBufferAccess[index] = new TextureAccess(this.colorBufferAccess[index], resource);
			this.AddResourceWrite(resource.handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetColorBufferRaw(in TextureHandle resource, int index, AccessFlags accessFlags, int mipLevel, int depthSlice)
		{
			if (this.colorBufferAccess[index].textureHandle.handle.Equals(resource.handle) || !this.colorBufferAccess[index].textureHandle.IsValid())
			{
				this.colorBufferMaxIndex = Math.Max(this.colorBufferMaxIndex, index);
				this.colorBufferAccess[index] = new TextureAccess(resource, accessFlags, mipLevel, depthSlice);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetFragmentInputRaw(in TextureHandle resource, int index, AccessFlags accessFlags, int mipLevel, int depthSlice)
		{
			if (this.fragmentInputAccess[index].textureHandle.handle.Equals(resource.handle) || !this.fragmentInputAccess[index].textureHandle.IsValid())
			{
				this.fragmentInputMaxIndex = Math.Max(this.fragmentInputMaxIndex, index);
				this.fragmentInputAccess[index] = new TextureAccess(resource, accessFlags, mipLevel, depthSlice);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetRandomWriteResourceRaw(in ResourceHandle resource, int index, bool preserveCounterValue, AccessFlags accessFlags)
		{
			if (this.randomAccessResource[index].h.Equals(resource) || !this.randomAccessResource[index].h.IsValid())
			{
				this.randomAccessResourceMaxIndex = Math.Max(this.randomAccessResourceMaxIndex, index);
				RenderGraphPass.RandomWriteResourceInfo[] randomAccessResource = this.randomAccessResource;
				randomAccessResource[index].h = resource;
				randomAccessResource[index].preserveCounterValue = preserveCounterValue;
				return;
			}
			throw new InvalidOperationException("You can only bind a single texture to an random write input index. Verify your indexes are correct.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDepthBuffer(in TextureHandle resource, DepthAccess flags)
		{
			this.depthAccess = new TextureAccess(resource, (AccessFlags)flags, 0, 0);
			if ((flags & DepthAccess.Read) != (DepthAccess)0)
			{
				this.AddResourceRead(resource.handle);
			}
			if ((flags & DepthAccess.Write) != (DepthAccess)0)
			{
				this.AddResourceWrite(resource.handle);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDepthBufferRaw(in TextureHandle resource, AccessFlags accessFlags, int mipLevel, int depthSlice)
		{
			if (this.depthAccess.textureHandle.handle.Equals(resource.handle) || !this.depthAccess.textureHandle.IsValid())
			{
				this.depthAccess = new TextureAccess(resource, accessFlags, mipLevel, depthSlice);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ComputeTextureHash(ref HashFNV1A32 generator, in ResourceHandle handle, RenderGraphResourceRegistry resources)
		{
			if (handle.index == 0)
			{
				return;
			}
			int num;
			if (resources.IsRenderGraphResourceImported(handle))
			{
				TextureResource textureResource = resources.GetTextureResource(handle);
				RTHandle graphicsResource = textureResource.graphicsResource;
				ref TextureDesc ptr = ref textureResource.desc;
				Texture externalTexture = graphicsResource.externalTexture;
				if (externalTexture != null)
				{
					num = (int)externalTexture.graphicsFormat;
					generator.Append(num);
					num = (int)externalTexture.dimension;
					generator.Append(num);
					num = externalTexture.width;
					generator.Append(num);
					num = externalTexture.height;
					generator.Append(num);
					RenderTexture renderTexture = externalTexture as RenderTexture;
					if (renderTexture != null)
					{
						num = renderTexture.antiAliasing;
						generator.Append(num);
					}
				}
				else if (graphicsResource.rt != null)
				{
					RenderTexture rt = graphicsResource.rt;
					num = (int)rt.graphicsFormat;
					generator.Append(num);
					num = (int)rt.dimension;
					generator.Append(num);
					num = rt.antiAliasing;
					generator.Append(num);
					if (graphicsResource.useScaling)
					{
						if (graphicsResource.scaleFunc != null)
						{
							num = DelegateHashCodeUtils.GetFuncHashCode(graphicsResource.scaleFunc);
							generator.Append(num);
						}
						else
						{
							Vector2 scaleFactor = graphicsResource.scaleFactor;
							generator.Append(scaleFactor);
						}
					}
					else
					{
						num = rt.width;
						generator.Append(num);
						num = rt.height;
						generator.Append(num);
					}
				}
				else if (graphicsResource.nameID != default(RenderTargetIdentifier))
				{
					num = (int)ptr.format;
					generator.Append(num);
					num = (int)ptr.dimension;
					generator.Append(num);
					num = (int)ptr.msaaSamples;
					generator.Append(num);
					generator.Append(ptr.width);
					generator.Append(ptr.height);
				}
				generator.Append(ptr.clearBuffer);
				generator.Append(ptr.discardBuffer);
				return;
			}
			TextureDesc textureResourceDesc = resources.GetTextureResourceDesc(handle, false);
			num = (int)textureResourceDesc.format;
			generator.Append(num);
			num = (int)textureResourceDesc.dimension;
			generator.Append(num);
			num = (int)textureResourceDesc.msaaSamples;
			generator.Append(num);
			generator.Append(textureResourceDesc.clearBuffer);
			generator.Append(textureResourceDesc.discardBuffer);
			switch (textureResourceDesc.sizeMode)
			{
			case TextureSizeMode.Explicit:
				generator.Append(textureResourceDesc.width);
				generator.Append(textureResourceDesc.height);
				return;
			case TextureSizeMode.Scale:
				generator.Append(textureResourceDesc.scale);
				return;
			case TextureSizeMode.Functor:
				num = DelegateHashCodeUtils.GetFuncHashCode(textureResourceDesc.func);
				generator.Append(num);
				return;
			default:
				return;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ComputeHashForTextureAccess(ref HashFNV1A32 generator, in ResourceHandle handle, in TextureAccess textureAccess)
		{
			int num = handle.index;
			generator.Append(num);
			num = (int)textureAccess.flags;
			generator.Append(num);
			generator.Append(textureAccess.mipLevel);
			generator.Append(textureAccess.depthSlice);
		}

		public void ComputeHash(ref HashFNV1A32 generator, RenderGraphResourceRegistry resources)
		{
			int num = (int)this.type;
			generator.Append(num);
			bool flag = this.enableAsyncCompute;
			generator.Append(flag);
			flag = this.allowPassCulling;
			generator.Append(flag);
			flag = this.allowGlobalState;
			generator.Append(flag);
			flag = this.enableFoveatedRasterization;
			generator.Append(flag);
			ResourceHandle handle = this.depthAccess.textureHandle.handle;
			if (handle.IsValid())
			{
				this.ComputeTextureHash(ref generator, handle, resources);
				TextureAccess textureAccess = this.depthAccess;
				RenderGraphPass.ComputeHashForTextureAccess(ref generator, handle, textureAccess);
			}
			for (int i = 0; i < this.colorBufferMaxIndex + 1; i++)
			{
				TextureAccess textureAccess2 = this.colorBufferAccess[i];
				ResourceHandle handle2 = textureAccess2.textureHandle.handle;
				if (handle2.IsValid())
				{
					this.ComputeTextureHash(ref generator, handle2, resources);
					RenderGraphPass.ComputeHashForTextureAccess(ref generator, handle2, textureAccess2);
				}
			}
			num = this.colorBufferMaxIndex;
			generator.Append(num);
			flag = this.hasShadingRateImage;
			generator.Append(flag);
			if (this.hasShadingRateImage)
			{
				ResourceHandle handle3 = this.shadingRateAccess.textureHandle.handle;
				if (handle3.IsValid())
				{
					this.ComputeTextureHash(ref generator, handle3, resources);
					TextureAccess textureAccess = this.shadingRateAccess;
					RenderGraphPass.ComputeHashForTextureAccess(ref generator, handle3, textureAccess);
				}
			}
			flag = this.hasShadingRateStates;
			generator.Append(flag);
			num = (int)this.shadingRateFragmentSize;
			generator.Append(num);
			num = (int)this.primitiveShadingRateCombiner;
			generator.Append(num);
			num = (int)this.fragmentShadingRateCombiner;
			generator.Append(num);
			for (int j = 0; j < this.fragmentInputMaxIndex + 1; j++)
			{
				TextureAccess textureAccess3 = this.fragmentInputAccess[j];
				ResourceHandle handle4 = textureAccess3.textureHandle.handle;
				if (handle4.IsValid())
				{
					this.ComputeTextureHash(ref generator, handle4, resources);
					RenderGraphPass.ComputeHashForTextureAccess(ref generator, handle4, textureAccess3);
				}
			}
			for (int k = 0; k < this.randomAccessResourceMaxIndex + 1; k++)
			{
				RenderGraphPass.RandomWriteResourceInfo randomWriteResourceInfo = this.randomAccessResource[k];
				if (randomWriteResourceInfo.h.IsValid())
				{
					num = randomWriteResourceInfo.h.index;
					generator.Append(num);
					generator.Append(randomWriteResourceInfo.preserveCounterValue);
				}
			}
			num = this.randomAccessResourceMaxIndex;
			generator.Append(num);
			num = this.fragmentInputMaxIndex;
			generator.Append(num);
			flag = this.generateDebugData;
			generator.Append(flag);
			flag = this.allowRendererListCulling;
			generator.Append(flag);
			for (int l = 0; l < 3; l++)
			{
				List<ResourceHandle> list = this.resourceReadLists[l];
				int count = list.Count;
				for (int m = 0; m < count; m++)
				{
					num = list[m].index;
					generator.Append(num);
				}
				List<ResourceHandle> list2 = this.resourceWriteLists[l];
				int count2 = list2.Count;
				for (int n = 0; n < count2; n++)
				{
					num = list2[n].index;
					generator.Append(num);
				}
				List<ResourceHandle> list3 = this.transientResourceList[l];
				int count3 = list3.Count;
				for (int num2 = 0; num2 < count3; num2++)
				{
					num = list3[num2].index;
					generator.Append(num);
				}
			}
			int count4 = this.usedRendererListList.Count;
			for (int num3 = 0; num3 < count4; num3++)
			{
				num = this.usedRendererListList[num3].handle;
				generator.Append(num);
			}
			int count5 = this.setGlobalsList.Count;
			for (int num4 = 0; num4 < count5; num4++)
			{
				ValueTuple<TextureHandle, int> valueTuple = this.setGlobalsList[num4];
				num = valueTuple.Item1.handle.index;
				generator.Append(num);
				generator.Append(valueTuple.Item2);
			}
			generator.Append(this.useAllGlobalTextures);
			int count6 = this.implicitReadsList.Count;
			for (int num5 = 0; num5 < count6; num5++)
			{
				num = this.implicitReadsList[num5].index;
				generator.Append(num);
			}
			num = this.GetRenderFuncHash();
			generator.Append(num);
		}

		public void SetShadingRateImage(in TextureHandle shadingRateImage, AccessFlags accessFlags, int mipLevel, int depthSlice)
		{
			if (ShadingRateInfo.supportsPerImageTile)
			{
				this.hasShadingRateImage = true;
				this.shadingRateAccess = new TextureAccess(shadingRateImage, accessFlags, mipLevel, depthSlice);
				this.AddResourceRead(this.shadingRateAccess.textureHandle.handle);
			}
		}

		public void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize)
		{
			if (ShadingRateInfo.supportsPerDrawCall)
			{
				this.hasShadingRateStates = true;
				this.shadingRateFragmentSize = shadingRateFragmentSize;
			}
		}

		public void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
		{
			if (ShadingRateInfo.supportsPerImageTile)
			{
				if (stage == ShadingRateCombinerStage.Primitive)
				{
					this.hasShadingRateStates = true;
					this.primitiveShadingRateCombiner = combiner;
					return;
				}
				if (stage != ShadingRateCombinerStage.Fragment)
				{
					return;
				}
				this.hasShadingRateStates = true;
				this.fragmentShadingRateCombiner = combiner;
			}
		}

		public List<ResourceHandle>[] resourceReadLists = new List<ResourceHandle>[3];

		public List<ResourceHandle>[] resourceWriteLists = new List<ResourceHandle>[3];

		public List<ResourceHandle>[] transientResourceList = new List<ResourceHandle>[3];

		public List<RendererListHandle> usedRendererListList = new List<RendererListHandle>();

		public List<ValueTuple<TextureHandle, int>> setGlobalsList = new List<ValueTuple<TextureHandle, int>>();

		public bool useAllGlobalTextures;

		public List<ResourceHandle> implicitReadsList = new List<ResourceHandle>();

		public struct RandomWriteResourceInfo
		{
			public ResourceHandle h;

			public bool preserveCounterValue;
		}
	}
}
