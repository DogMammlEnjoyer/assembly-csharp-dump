using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal struct NativePassData
	{
		public bool hasShadingRateImage
		{
			get
			{
				return this.shadingRateImageIndex >= 0;
			}
		}

		public NativePassData(ref PassData pass, CompilerContextData ctx)
		{
			this.firstGraphPass = pass.passId;
			this.lastGraphPass = pass.passId;
			this.numGraphPasses = 1;
			this.firstNativeSubPass = -1;
			this.numNativeSubPasses = 0;
			this.fragments = default(FixedAttachmentArray<PassFragmentData>);
			this.attachments = default(FixedAttachmentArray<NativePassAttachment>);
			this.width = pass.fragmentInfoWidth;
			this.height = pass.fragmentInfoHeight;
			this.volumeDepth = pass.fragmentInfoVolumeDepth;
			this.samples = pass.fragmentInfoSamples;
			this.hasDepth = pass.fragmentInfoHasDepth;
			this.hasFoveatedRasterization = pass.hasFoveatedRasterization;
			this.loadAudit = default(FixedAttachmentArray<LoadAudit>);
			this.storeAudit = default(FixedAttachmentArray<StoreAudit>);
			this.breakAudit = new PassBreakAudit(PassBreakReason.NotOptimized, -1);
			ReadOnlySpan<PassFragmentData> readOnlySpan = pass.Fragments(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData data = ref readOnlySpan[i];
				this.fragments.Add(data);
			}
			readOnlySpan = pass.FragmentInputs(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData data2 = ref readOnlySpan[i];
				this.fragments.Add(data2);
			}
			if (pass.fragmentInfoHasShadingRateImage && !this.hasFoveatedRasterization)
			{
				this.shadingRateImageIndex = this.fragments.size;
				PassFragmentData passFragmentData = pass.ShadingRateImage(ctx);
				this.fragments.Add(passFragmentData);
			}
			else
			{
				this.shadingRateImageIndex = -1;
			}
			this.hasShadingRateStates = (pass.hasShadingRateStates && !this.hasFoveatedRasterization);
			this.shadingRateFragmentSize = pass.shadingRateFragmentSize;
			this.primitiveShadingRateCombiner = pass.primitiveShadingRateCombiner;
			this.fragmentShadingRateCombiner = pass.fragmentShadingRateCombiner;
			NativePassData.TryMergeNativeSubPass(ctx, ref this, ref pass);
		}

		public SubPassFlags GetSubPassFlagForMerging()
		{
			if (!this.hasDepth)
			{
				throw new Exception("SubPassFlag for merging can not be determined if native pass doesn't have a depth attachment");
			}
			return SubPassFlags.ReadOnlyDepth;
		}

		public void Clear()
		{
			this.firstGraphPass = 0;
			this.numGraphPasses = 0;
			this.attachments.Clear();
			this.fragments.Clear();
			this.loadAudit.Clear();
			this.storeAudit.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly bool IsValid()
		{
			return this.numGraphPasses > 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly ReadOnlySpan<PassData> GraphPasses(CompilerContextData ctx)
		{
			if (this.lastGraphPass - this.firstGraphPass + 1 == this.numGraphPasses)
			{
				return ref ctx.passData.MakeReadOnlySpan(this.firstGraphPass, this.numGraphPasses);
			}
			NativeArray<PassData> nativeArray = new NativeArray<PassData>(this.numGraphPasses, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int i = this.firstGraphPass;
			int num = 0;
			while (i < this.lastGraphPass + 1)
			{
				PassData passData = ctx.passData[i];
				if (!passData.culled)
				{
					nativeArray[num++] = passData;
				}
				i++;
			}
			return nativeArray;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void GetGraphPassNames(CompilerContextData ctx, DynamicArray<Name> dest)
		{
			ReadOnlySpan<PassData> readOnlySpan = this.GraphPasses(ctx);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassData ptr = ref readOnlySpan[i];
				PassData passData = ptr;
				Name name = passData.GetName(ctx);
				dest.Add(name);
			}
		}

		public unsafe static PassBreakAudit CanMerge(CompilerContextData contextData, int activeNativePassId, int passIdToMerge)
		{
			ref PassData ptr = ref contextData.passData.ElementAt(passIdToMerge);
			if (ptr.type != RenderGraphPassType.Raster)
			{
				return new PassBreakAudit(PassBreakReason.NonRasterPass, passIdToMerge);
			}
			ref NativePassData ptr2 = ref contextData.nativePassData.ElementAt(activeNativePassId);
			if (ptr.numFragments > 0 || ptr.numFragmentInputs > 0)
			{
				if (ptr2.width != ptr.fragmentInfoWidth || ptr2.height != ptr.fragmentInfoHeight || ptr2.volumeDepth != ptr.fragmentInfoVolumeDepth || ptr2.samples != ptr.fragmentInfoSamples)
				{
					return new PassBreakAudit(PassBreakReason.TargetSizeMismatch, passIdToMerge);
				}
				if (ptr2.hasDepth && ptr.fragmentInfoHasDepth)
				{
					ref PassFragmentData ptr3 = ref contextData.fragmentData.ElementAt(ptr.firstFragment);
					if (ptr2.fragments[0].resource.index != ptr3.resource.index)
					{
						return new PassBreakAudit(PassBreakReason.DifferentDepthTextures, passIdToMerge);
					}
				}
				if (ptr2.hasFoveatedRasterization != ptr.hasFoveatedRasterization)
				{
					return new PassBreakAudit(PassBreakReason.FRStateMismatch, passIdToMerge);
				}
				if (ptr2.hasShadingRateImage != ptr.fragmentInfoHasShadingRateImage)
				{
					return new PassBreakAudit(PassBreakReason.DifferentShadingRateImages, passIdToMerge);
				}
				if (ptr2.hasShadingRateImage)
				{
					PassFragmentData passFragmentData = ptr.ShadingRateImage(contextData);
					PassFragmentData passFragmentData2 = *ptr2.fragments[ptr2.shadingRateImageIndex];
					if (passFragmentData2.resource.index != passFragmentData.resource.index)
					{
						return new PassBreakAudit(PassBreakReason.DifferentShadingRateImages, passIdToMerge);
					}
				}
				if (ptr2.hasShadingRateStates != ptr.hasShadingRateStates)
				{
					return new PassBreakAudit(PassBreakReason.DifferentShadingRateStates, passIdToMerge);
				}
				if (ptr2.hasShadingRateStates && (ptr2.shadingRateFragmentSize != ptr.shadingRateFragmentSize || ptr2.primitiveShadingRateCombiner != ptr.primitiveShadingRateCombiner || ptr2.fragmentShadingRateCombiner != ptr.fragmentShadingRateCombiner))
				{
					return new PassBreakAudit(PassBreakReason.DifferentShadingRateStates, passIdToMerge);
				}
			}
			ReadOnlySpan<PassInputData> readOnlySpan = ptr.Inputs(contextData);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ResourceHandle resource = readOnlySpan[i].resource;
				int writePassId = contextData.resources[resource].writePassId;
				if (writePassId >= ptr2.firstGraphPass && writePassId < ptr2.lastGraphPass + 1 && !ptr.IsUsedAsFragment(resource, contextData))
				{
					return new PassBreakAudit(PassBreakReason.NextPassReadsTexture, passIdToMerge);
				}
			}
			FixedAttachmentArray<PassFragmentData> fixedAttachmentArray = default(FixedAttachmentArray<PassFragmentData>);
			int num = 8 - ptr2.fragments.size;
			ReadOnlySpan<PassFragmentData> readOnlySpan2 = ptr.Fragments(contextData);
			for (int i = 0; i < readOnlySpan2.Length; i++)
			{
				ref readonly PassFragmentData ptr4 = ref readOnlySpan2[i];
				bool flag = false;
				for (int j = 0; j < ptr2.fragments.size; j++)
				{
					if (PassFragmentData.SameSubResource(ptr2.fragments[j], ptr4))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					if (num == 0)
					{
						return new PassBreakAudit(PassBreakReason.AttachmentLimitReached, passIdToMerge);
					}
					fixedAttachmentArray.Add(ptr4);
					num--;
				}
				for (int k = ptr2.firstGraphPass; k <= ptr2.lastGraphPass; k++)
				{
					ref PassData ptr5 = ref contextData.passData.ElementAt(k);
					readOnlySpan = ptr5.Inputs(contextData);
					for (int l = 0; l < readOnlySpan.Length; l++)
					{
						ref readonly PassInputData ptr6 = ref readOnlySpan[l];
						if (ptr6.resource.index == ptr4.resource.index && !ptr5.IsUsedAsFragment(ptr6.resource, contextData))
						{
							return new PassBreakAudit(PassBreakReason.NextPassTargetsTexture, passIdToMerge);
						}
					}
				}
			}
			readOnlySpan2 = ptr.FragmentInputs(contextData);
			for (int i = 0; i < readOnlySpan2.Length; i++)
			{
				ref readonly PassFragmentData ptr7 = ref readOnlySpan2[i];
				bool flag2 = false;
				for (int m = 0; m < ptr2.fragments.size; m++)
				{
					if (PassFragmentData.SameSubResource(ptr2.fragments[m], ptr7))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					if (num == 0)
					{
						return new PassBreakAudit(PassBreakReason.AttachmentLimitReached, passIdToMerge);
					}
					fixedAttachmentArray.Add(ptr7);
					num--;
				}
			}
			if (ptr2.numGraphPasses >= 8 && !NativePassData.CanMergeNativeSubPass(contextData, ref ptr2, ref ptr))
			{
				return new PassBreakAudit(PassBreakReason.SubPassLimitReached, passIdToMerge);
			}
			return new PassBreakAudit(PassBreakReason.Merged, passIdToMerge);
		}

		private static bool CanMergeNativeSubPass(CompilerContextData contextData, ref NativePassData nativePass, ref PassData passToMerge)
		{
			if (passToMerge.numFragments == 0 && passToMerge.numFragmentInputs == 0)
			{
				return true;
			}
			if (nativePass.numNativeSubPasses == 0)
			{
				return false;
			}
			ref SubPassDescriptor ptr = ref contextData.nativeSubPassData.ElementAt(nativePass.firstNativeSubPass + nativePass.numNativeSubPasses - 1);
			bool fragmentInfoHasDepth = passToMerge.fragmentInfoHasDepth;
			int num = fragmentInfoHasDepth ? -1 : 0;
			if (passToMerge.numFragments + num != ptr.colorOutputs.Length)
			{
				return false;
			}
			if (passToMerge.numFragmentInputs != ptr.inputs.Length)
			{
				return false;
			}
			SubPassFlags subPassFlags = SubPassFlags.None;
			if (!fragmentInfoHasDepth && nativePass.hasDepth)
			{
				subPassFlags = nativePass.GetSubPassFlagForMerging();
			}
			ref FixedAttachmentArray<PassFragmentData> ptr2 = ref nativePass.fragments;
			int num2 = 0;
			ReadOnlySpan<PassFragmentData> readOnlySpan = passToMerge.Fragments(contextData);
			int i;
			for (i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData ptr3 = ref readOnlySpan[i];
				if (!fragmentInfoHasDepth || num2 != 0)
				{
					int num3 = -1;
					int num4 = 0;
					for (;;)
					{
						int num5 = num4;
						FixedAttachmentArray<PassFragmentData> fixedAttachmentArray = ptr2;
						if (num5 >= fixedAttachmentArray.size)
						{
							break;
						}
						fixedAttachmentArray = ptr2;
						if (PassFragmentData.SameSubResource(fixedAttachmentArray[num4], ptr3))
						{
							goto Block_12;
						}
						num4++;
					}
					IL_117:
					if (num3 < 0 || num3 != ptr.colorOutputs[num2 + num])
					{
						return false;
					}
					goto IL_131;
					Block_12:
					num3 = num4;
					goto IL_117;
				}
				subPassFlags = (ptr3.accessFlags.HasFlag(AccessFlags.Write) ? SubPassFlags.None : SubPassFlags.ReadOnlyDepth);
				IL_131:
				num2++;
			}
			int num6 = 0;
			readOnlySpan = passToMerge.FragmentInputs(contextData);
			i = 0;
			while (i < readOnlySpan.Length)
			{
				ref readonly PassFragmentData y = ref readOnlySpan[i];
				int num7 = -1;
				int num8 = 0;
				for (;;)
				{
					int num9 = num8;
					FixedAttachmentArray<PassFragmentData> fixedAttachmentArray = ptr2;
					if (num9 >= fixedAttachmentArray.size)
					{
						break;
					}
					fixedAttachmentArray = ptr2;
					if (PassFragmentData.SameSubResource(fixedAttachmentArray[num8], y))
					{
						goto Block_15;
					}
					num8++;
				}
				IL_1AA:
				if (num7 < 0 || num7 != ptr.inputs[num6])
				{
					return false;
				}
				num6++;
				i++;
				continue;
				Block_15:
				num7 = num8;
				goto IL_1AA;
			}
			return subPassFlags == ptr.flags;
		}

		public static void TryMergeNativeSubPass(CompilerContextData contextData, ref NativePassData nativePass, ref PassData passToMerge)
		{
			ref FixedAttachmentArray<PassFragmentData> ptr = ref nativePass.fragments;
			if (nativePass.numNativeSubPasses == 0 && nativePass.fragments.size > 0)
			{
				nativePass.firstNativeSubPass = contextData.nativeSubPassData.Length;
			}
			SubPassDescriptor subPassDescriptor = default(SubPassDescriptor);
			if (passToMerge.numFragments == 0 && passToMerge.numFragmentInputs == 0)
			{
				passToMerge.nativeSubPassIndex = nativePass.numNativeSubPasses - 1;
				passToMerge.beginNativeSubpass = false;
				return;
			}
			if (!passToMerge.fragmentInfoHasDepth && nativePass.hasDepth)
			{
				subPassDescriptor.flags = nativePass.GetSubPassFlagForMerging();
			}
			int num = 0;
			int num2 = passToMerge.fragmentInfoHasDepth ? -1 : 0;
			subPassDescriptor.colorOutputs = new AttachmentIndexArray(passToMerge.numFragments + num2);
			ReadOnlySpan<PassFragmentData> readOnlySpan = passToMerge.Fragments(contextData);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData ptr2 = ref readOnlySpan[i];
				if (passToMerge.fragmentInfoHasDepth && num == 0)
				{
					subPassDescriptor.flags = (ptr2.accessFlags.HasFlag(AccessFlags.Write) ? SubPassFlags.None : SubPassFlags.ReadOnlyDepth);
				}
				else
				{
					int value = -1;
					for (int j = 0; j < ptr.size; j++)
					{
						if (PassFragmentData.SameSubResource(ptr[j], ptr2))
						{
							value = j;
							break;
						}
					}
					subPassDescriptor.colorOutputs[num + num2] = value;
				}
				num++;
			}
			int num3 = 0;
			subPassDescriptor.inputs = new AttachmentIndexArray(passToMerge.numFragmentInputs);
			readOnlySpan = passToMerge.FragmentInputs(contextData);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData y = ref readOnlySpan[i];
				int value2 = -1;
				for (int k = 0; k < ptr.size; k++)
				{
					if (PassFragmentData.SameSubResource(ptr[k], y))
					{
						value2 = k;
						break;
					}
				}
				subPassDescriptor.inputs[num3] = value2;
				num3++;
			}
			if (passToMerge.fragmentInfoHasShadingRateImage)
			{
				subPassDescriptor.flags |= SubPassFlags.UseShadingRateImage;
			}
			if (nativePass.numNativeSubPasses == 0 || !NativePassCompiler.IsSameNativeSubPass(ref subPassDescriptor, contextData.nativeSubPassData.ElementAt(nativePass.firstNativeSubPass + nativePass.numNativeSubPasses - 1)))
			{
				contextData.nativeSubPassData.Add(subPassDescriptor);
				nativePass.numNativeSubPasses++;
				passToMerge.beginNativeSubpass = true;
			}
			else
			{
				passToMerge.beginNativeSubpass = false;
			}
			passToMerge.nativeSubPassIndex = nativePass.numNativeSubPasses - 1;
		}

		private unsafe void AddDepthAttachmentFirstDuringMerge(CompilerContextData contextData, in PassFragmentData depthAttachment)
		{
			this.fragments.Add(depthAttachment);
			this.hasDepth = true;
			int size = this.fragments.size;
			if (size == 1)
			{
				return;
			}
			int num = size - 1;
			ref PassFragmentData ptr = ref this.fragments[0];
			ref PassFragmentData ptr2 = ref this.fragments[num];
			PassFragmentData passFragmentData = *this.fragments[num];
			PassFragmentData passFragmentData2 = *this.fragments[0];
			ptr = passFragmentData;
			ptr2 = passFragmentData2;
			SubPassFlags subPassFlagForMerging = this.GetSubPassFlagForMerging();
			for (int i = this.firstNativeSubPass; i < this.firstNativeSubPass + this.numNativeSubPasses; i++)
			{
				ref SubPassDescriptor ptr3 = ref contextData.nativeSubPassData.ElementAt(i);
				ptr3.flags = subPassFlagForMerging;
				for (int j = 0; j < ptr3.colorOutputs.Length; j++)
				{
					if (ptr3.colorOutputs[j] == 0)
					{
						ptr3.colorOutputs[j] = num;
					}
				}
				for (int k = 0; k < ptr3.inputs.Length; k++)
				{
					if (ptr3.inputs[k] == 0)
					{
						ptr3.inputs[k] = num;
					}
				}
			}
			if (this.hasShadingRateImage && this.shadingRateImageIndex == 0)
			{
				this.shadingRateImageIndex = num;
			}
		}

		public static PassBreakAudit TryMerge(CompilerContextData contextData, int activeNativePassId, int passIdToMerge)
		{
			PassBreakAudit passBreakAudit = NativePassData.CanMerge(contextData, activeNativePassId, passIdToMerge);
			if (passBreakAudit.reason != PassBreakReason.Merged)
			{
				return passBreakAudit;
			}
			ref PassData ptr = ref contextData.passData.ElementAt(passIdToMerge);
			ref NativePassData ptr2 = ref contextData.nativePassData.ElementAt(activeNativePassId);
			ptr.mergeState = PassMergeState.SubPass;
			if (ptr.nativePassIndex >= 0)
			{
				contextData.nativePassData.ElementAt(ptr.nativePassIndex).Clear();
			}
			ptr.nativePassIndex = activeNativePassId;
			ptr2.numGraphPasses++;
			ptr2.lastGraphPass = passIdToMerge;
			if (!ptr2.hasDepth && ptr.fragmentInfoHasDepth)
			{
				ref NativePassData ptr3 = ref ptr2;
				PassFragmentData passFragmentData = contextData.fragmentData[ptr.firstFragment];
				ptr3.AddDepthAttachmentFirstDuringMerge(contextData, passFragmentData);
			}
			ReadOnlySpan<PassFragmentData> readOnlySpan = ptr.Fragments(contextData);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData ptr4 = ref readOnlySpan[i];
				bool flag = false;
				for (int j = 0; j < ptr2.fragments.size; j++)
				{
					ref PassFragmentData ptr5 = ref ptr2.fragments[j];
					if (PassFragmentData.SameSubResource(ptr5, ptr4))
					{
						AccessFlags accessFlags = ptr4.accessFlags;
						if (ptr5.accessFlags.HasFlag(AccessFlags.Discard))
						{
							accessFlags &= ~AccessFlags.Read;
						}
						ptr5 = new PassFragmentData(new ResourceHandle(ref ptr5.resource, ptr4.resource.version), ptr5.accessFlags | accessFlags, ptr5.mipLevel, ptr5.depthSlice);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ptr2.fragments.Add(ptr4);
				}
			}
			readOnlySpan = ptr.FragmentInputs(contextData);
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				ref readonly PassFragmentData ptr6 = ref readOnlySpan[i];
				bool flag2 = false;
				for (int k = 0; k < ptr2.fragments.size; k++)
				{
					ref PassFragmentData ptr7 = ref ptr2.fragments[k];
					if (PassFragmentData.SameSubResource(ptr7, ptr6))
					{
						AccessFlags accessFlags2 = ptr6.accessFlags;
						if (ptr7.accessFlags.HasFlag(AccessFlags.Discard))
						{
							accessFlags2 &= ~AccessFlags.Read;
						}
						ptr7 = new PassFragmentData(new ResourceHandle(ref ptr7.resource, ptr6.resource.version), ptr7.accessFlags | accessFlags2, ptr7.mipLevel, ptr7.depthSlice);
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					ptr2.fragments.Add(ptr6);
				}
			}
			NativePassData.TryMergeNativeSubPass(contextData, ref ptr2, ref ptr);
			NativePassData.SetPassStatesForNativePass(contextData, activeNativePassId);
			return passBreakAudit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetPassStatesForNativePass(CompilerContextData contextData, int nativePassId)
		{
			ref NativePassData ptr = ref contextData.nativePassData.ElementAt(nativePassId);
			if (ptr.numGraphPasses > 1)
			{
				contextData.passData.ElementAt(ptr.firstGraphPass).mergeState = PassMergeState.Begin;
				int num = ptr.lastGraphPass - ptr.firstGraphPass + 1;
				for (int i = 1; i < num; i++)
				{
					int index = ptr.firstGraphPass + i;
					if (contextData.passData.ElementAt(index).culled)
					{
						contextData.passData.ElementAt(index).mergeState = PassMergeState.None;
					}
					else
					{
						contextData.passData.ElementAt(ptr.firstGraphPass + i).mergeState = PassMergeState.SubPass;
					}
				}
				contextData.passData.ElementAt(ptr.lastGraphPass).mergeState = PassMergeState.End;
				return;
			}
			contextData.passData.ElementAt(ptr.firstGraphPass).mergeState = PassMergeState.None;
		}

		public FixedAttachmentArray<LoadAudit> loadAudit;

		public FixedAttachmentArray<StoreAudit> storeAudit;

		public PassBreakAudit breakAudit;

		public FixedAttachmentArray<PassFragmentData> fragments;

		public FixedAttachmentArray<NativePassAttachment> attachments;

		public int firstGraphPass;

		public int lastGraphPass;

		public int numGraphPasses;

		public int firstNativeSubPass;

		public int numNativeSubPasses;

		public int width;

		public int height;

		public int volumeDepth;

		public int samples;

		public int shadingRateImageIndex;

		public bool hasDepth;

		public bool hasFoveatedRasterization;

		public bool hasShadingRateStates;

		public ShadingRateFragmentSize shadingRateFragmentSize;

		public ShadingRateCombiner primitiveShadingRateCombiner;

		public ShadingRateCombiner fragmentShadingRateCombiner;
	}
}
