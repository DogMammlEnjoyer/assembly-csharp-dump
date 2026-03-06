using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal struct PassData
	{
		public bool fragmentInfoHasShadingRateImage
		{
			get
			{
				return this.shadingRateImageIndex > 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Name GetName(CompilerContextData ctx)
		{
			return ctx.GetFullPassName(this.passId);
		}

		public PassData(in RenderGraphPass pass, int passIndex)
		{
			this.passId = passIndex;
			this.type = pass.type;
			this.asyncCompute = pass.enableAsyncCompute;
			this.hasSideEffects = !pass.allowPassCulling;
			this.hasFoveatedRasterization = pass.enableFoveatedRasterization;
			this.mergeState = PassMergeState.None;
			this.nativePassIndex = -1;
			this.nativeSubPassIndex = -1;
			this.beginNativeSubpass = false;
			this.culled = false;
			this.tag = 0;
			this.firstInput = 0;
			this.numInputs = 0;
			this.firstOutput = 0;
			this.numOutputs = 0;
			this.firstFragment = 0;
			this.numFragments = 0;
			this.firstRandomAccessResource = 0;
			this.numRandomAccessResources = 0;
			this.firstFragmentInput = 0;
			this.numFragmentInputs = 0;
			this.firstCreate = 0;
			this.numCreated = 0;
			this.firstDestroy = 0;
			this.numDestroyed = 0;
			this.fragmentInfoValid = false;
			this.fragmentInfoWidth = 0;
			this.fragmentInfoHeight = 0;
			this.fragmentInfoVolumeDepth = 0;
			this.fragmentInfoSamples = 0;
			this.fragmentInfoHasDepth = false;
			this.insertGraphicsFence = false;
			this.waitOnGraphicsFencePassId = -1;
			this.hasShadingRateStates = pass.hasShadingRateStates;
			this.shadingRateFragmentSize = pass.shadingRateFragmentSize;
			this.primitiveShadingRateCombiner = pass.primitiveShadingRateCombiner;
			this.fragmentShadingRateCombiner = pass.fragmentShadingRateCombiner;
			this.shadingRateImageIndex = -1;
		}

		public void ResetAndInitialize(in RenderGraphPass pass, int passIndex)
		{
			this.passId = passIndex;
			this.type = pass.type;
			this.asyncCompute = pass.enableAsyncCompute;
			this.hasSideEffects = !pass.allowPassCulling;
			this.hasFoveatedRasterization = pass.enableFoveatedRasterization;
			this.mergeState = PassMergeState.None;
			this.nativePassIndex = -1;
			this.nativeSubPassIndex = -1;
			this.beginNativeSubpass = false;
			this.culled = false;
			this.tag = 0;
			this.firstInput = 0;
			this.numInputs = 0;
			this.firstOutput = 0;
			this.numOutputs = 0;
			this.firstFragment = 0;
			this.numFragments = 0;
			this.firstFragmentInput = 0;
			this.numFragmentInputs = 0;
			this.firstRandomAccessResource = 0;
			this.numRandomAccessResources = 0;
			this.firstCreate = 0;
			this.numCreated = 0;
			this.firstDestroy = 0;
			this.numDestroyed = 0;
			this.fragmentInfoValid = false;
			this.fragmentInfoWidth = 0;
			this.fragmentInfoHeight = 0;
			this.fragmentInfoVolumeDepth = 0;
			this.fragmentInfoSamples = 0;
			this.fragmentInfoHasDepth = false;
			this.insertGraphicsFence = false;
			this.waitOnGraphicsFencePassId = -1;
			this.hasShadingRateStates = pass.hasShadingRateStates;
			this.shadingRateFragmentSize = pass.shadingRateFragmentSize;
			this.primitiveShadingRateCombiner = pass.primitiveShadingRateCombiner;
			this.fragmentShadingRateCombiner = pass.fragmentShadingRateCombiner;
			this.shadingRateImageIndex = -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<PassOutputData> Outputs(CompilerContextData ctx)
		{
			return ref ctx.outputData.MakeReadOnlySpan(this.firstOutput, this.numOutputs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<PassInputData> Inputs(CompilerContextData ctx)
		{
			return ref ctx.inputData.MakeReadOnlySpan(this.firstInput, this.numInputs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<PassFragmentData> Fragments(CompilerContextData ctx)
		{
			return ref ctx.fragmentData.MakeReadOnlySpan(this.firstFragment, this.numFragments);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly PassFragmentData ShadingRateImage(CompilerContextData ctx)
		{
			return ctx.fragmentData[this.shadingRateImageIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<PassFragmentData> FragmentInputs(CompilerContextData ctx)
		{
			return ref ctx.fragmentData.MakeReadOnlySpan(this.firstFragmentInput, this.numFragmentInputs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<ResourceHandle> FirstUsedResources(CompilerContextData ctx)
		{
			return ref ctx.createData.MakeReadOnlySpan(this.firstCreate, this.numCreated);
		}

		public ReadOnlySpan<PassRandomWriteData> RandomWriteTextures(CompilerContextData ctx)
		{
			return ref ctx.randomAccessResourceData.MakeReadOnlySpan(this.firstRandomAccessResource, this.numRandomAccessResources);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<ResourceHandle> LastUsedResources(CompilerContextData ctx)
		{
			return ref ctx.destroyData.MakeReadOnlySpan(this.firstDestroy, this.numDestroyed);
		}

		private void SetupAndValidateFragmentInfo(ResourceHandle h, CompilerContextData ctx)
		{
			ref ResourceUnversionedData ptr = ref ctx.UnversionedResourceData(h);
			if (!this.fragmentInfoValid)
			{
				this.fragmentInfoWidth = ptr.width;
				this.fragmentInfoHeight = ptr.height;
				this.fragmentInfoSamples = ptr.msaaSamples;
				this.fragmentInfoVolumeDepth = ptr.volumeDepth;
				this.fragmentInfoValid = true;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddFragment(ResourceHandle h, CompilerContextData ctx)
		{
			this.SetupAndValidateFragmentInfo(h, ctx);
			this.numFragments++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddFragmentInput(ResourceHandle h, CompilerContextData ctx)
		{
			this.SetupAndValidateFragmentInfo(h, ctx);
			this.numFragmentInputs++;
		}

		internal void AddRandomAccessResource()
		{
			this.numRandomAccessResources++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddFirstUse(ResourceHandle h, CompilerContextData ctx)
		{
			ReadOnlySpan<ResourceHandle> readOnlySpan = this.FirstUsedResources(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly ResourceHandle ptr = ref readOnlySpan[i];
				if (ptr.index == h.index && ptr.type == h.type)
				{
					return;
				}
			}
			ctx.createData.Add(h);
			int num = ref ctx.createData.LastIndex<ResourceHandle>();
			if (this.numCreated == 0)
			{
				this.firstCreate = num;
			}
			this.numCreated++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void AddLastUse(ResourceHandle h, CompilerContextData ctx)
		{
			ReadOnlySpan<ResourceHandle> readOnlySpan = this.LastUsedResources(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly ResourceHandle ptr = ref readOnlySpan[i];
				if (ptr.index == h.index && ptr.type == h.type)
				{
					return;
				}
			}
			ctx.destroyData.Add(h);
			int num = ref ctx.destroyData.LastIndex<ResourceHandle>();
			if (this.numDestroyed == 0)
			{
				this.firstDestroy = num;
			}
			this.numDestroyed++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal readonly bool IsUsedAsFragment(ResourceHandle h, CompilerContextData ctx)
		{
			if (h.type != RenderGraphResourceType.Texture)
			{
				return false;
			}
			if (this.type != RenderGraphPassType.Raster)
			{
				return false;
			}
			ReadOnlySpan<PassFragmentData> readOnlySpan = this.Fragments(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (readOnlySpan[i].resource.index == h.index)
				{
					return true;
				}
			}
			readOnlySpan = this.FragmentInputs(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				if (readOnlySpan[i].resource.index == h.index)
				{
					return true;
				}
			}
			return false;
		}

		public int passId;

		public RenderGraphPassType type;

		public bool hasFoveatedRasterization;

		public int tag;

		public ShadingRateFragmentSize shadingRateFragmentSize;

		public ShadingRateCombiner primitiveShadingRateCombiner;

		public ShadingRateCombiner fragmentShadingRateCombiner;

		public PassMergeState mergeState;

		public int nativePassIndex;

		public int nativeSubPassIndex;

		public int firstInput;

		public int numInputs;

		public int firstOutput;

		public int numOutputs;

		public int firstFragment;

		public int numFragments;

		public int firstFragmentInput;

		public int numFragmentInputs;

		public int firstRandomAccessResource;

		public int numRandomAccessResources;

		public int firstCreate;

		public int numCreated;

		public int firstDestroy;

		public int numDestroyed;

		public int shadingRateImageIndex;

		public int fragmentInfoWidth;

		public int fragmentInfoHeight;

		public int fragmentInfoVolumeDepth;

		public int fragmentInfoSamples;

		public int waitOnGraphicsFencePassId;

		public bool asyncCompute;

		public bool hasSideEffects;

		public bool culled;

		public bool beginNativeSubpass;

		public bool fragmentInfoValid;

		public bool fragmentInfoHasDepth;

		public bool insertGraphicsFence;

		public bool hasShadingRateStates;
	}
}
