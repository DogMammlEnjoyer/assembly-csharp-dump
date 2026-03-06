using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal class NativePassCompiler : IDisposable
	{
		public NativePassCompiler(RenderGraphCompilationCache cache)
		{
			this.m_CompilationCache = cache;
			this.defaultContextData = new CompilerContextData();
			this.toVisitPassIds = new Stack<int>(100);
		}

		~NativePassCompiler()
		{
			this.Cleanup();
		}

		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		public void Cleanup()
		{
			CompilerContextData compilerContextData = this.contextData;
			if (compilerContextData != null)
			{
				compilerContextData.Dispose();
			}
			CompilerContextData compilerContextData2 = this.defaultContextData;
			if (compilerContextData2 != null)
			{
				compilerContextData2.Dispose();
			}
			if (this.m_BeginRenderPassAttachments.IsCreated)
			{
				this.m_BeginRenderPassAttachments.Dispose();
			}
		}

		public bool Initialize(RenderGraphResourceRegistry resources, List<RenderGraphPass> renderPasses, RenderGraphDebugParams debugParams, string debugName, bool useCompilationCaching, int graphHash, int frameIndex)
		{
			bool result = false;
			if (!useCompilationCaching)
			{
				this.contextData = this.defaultContextData;
			}
			else
			{
				result = this.m_CompilationCache.GetCompilationCache(graphHash, frameIndex, out this.contextData);
			}
			this.graph.m_ResourcesForDebugOnly = resources;
			this.graph.m_RenderPasses = renderPasses;
			this.graph.disablePassCulling = debugParams.disablePassCulling;
			this.graph.disablePassMerging = debugParams.disablePassMerging;
			this.graph.debugName = debugName;
			this.Clear(!useCompilationCaching);
			return result;
		}

		public void Compile(RenderGraphResourceRegistry resources)
		{
			this.SetupContextData(resources);
			this.BuildGraph();
			this.CullUnusedRenderPasses();
			this.TryMergeNativePasses();
			this.FindResourceUsageRanges();
			this.DetectMemoryLessResources();
			this.PrepareNativeRenderPasses();
		}

		public void Clear(bool clearContextData)
		{
			if (clearContextData)
			{
				this.contextData.Clear();
			}
			this.toVisitPassIds.Clear();
		}

		private void SetPassStatesForNativePass(int nativePassId)
		{
			NativePassData.SetPassStatesForNativePass(this.contextData, nativePassId);
		}

		private void SetupContextData(RenderGraphResourceRegistry resources)
		{
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_SetupContextData)))
			{
				this.contextData.Initialize(resources, 100);
			}
		}

		private void BuildGraph()
		{
			CompilerContextData compilerContextData = this.contextData;
			List<RenderGraphPass> renderPasses = this.graph.m_RenderPasses;
			compilerContextData.passData.ResizeUninitialized(renderPasses.Count);
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_BuildGraph)))
			{
				for (int i = 0; i < renderPasses.Count; i++)
				{
					RenderGraphPass renderGraphPass = renderPasses[i];
					ref PassData ptr = ref compilerContextData.passData.ElementAt(i);
					ptr.ResetAndInitialize(renderGraphPass, i);
					DynamicArray<Name> passNames = compilerContextData.passNames;
					Name name = new Name(renderGraphPass.name, true);
					passNames.Add(name);
					if (ptr.hasSideEffects)
					{
						this.toVisitPassIds.Push(i);
					}
					if (ptr.type == RenderGraphPassType.Raster)
					{
						ptr.firstFragment = compilerContextData.fragmentData.Length;
						TextureAccess textureAccess = renderGraphPass.depthAccess;
						if (textureAccess.textureHandle.handle.IsValid())
						{
							ptr.fragmentInfoHasDepth = true;
							if (compilerContextData.AddToFragmentList(renderGraphPass.depthAccess, ptr.firstFragment, ptr.numFragments))
							{
								ptr.AddFragment(renderGraphPass.depthAccess.textureHandle.handle, compilerContextData);
							}
						}
						for (int j = 0; j < renderGraphPass.colorBufferMaxIndex + 1; j++)
						{
							if (renderGraphPass.colorBufferAccess[j].textureHandle.handle.IsValid() && compilerContextData.AddToFragmentList(renderGraphPass.colorBufferAccess[j], ptr.firstFragment, ptr.numFragments))
							{
								ptr.AddFragment(renderGraphPass.colorBufferAccess[j].textureHandle.handle, compilerContextData);
							}
						}
						if (renderGraphPass.hasShadingRateImage)
						{
							textureAccess = renderGraphPass.shadingRateAccess;
							if (textureAccess.textureHandle.handle.IsValid())
							{
								ptr.shadingRateImageIndex = compilerContextData.fragmentData.Length;
								compilerContextData.AddToFragmentList(renderGraphPass.shadingRateAccess, ptr.shadingRateImageIndex, 0);
							}
						}
						ptr.firstFragmentInput = compilerContextData.fragmentData.Length;
						for (int k = 0; k < renderGraphPass.fragmentInputMaxIndex + 1; k++)
						{
							if (renderGraphPass.fragmentInputAccess[k].textureHandle.IsValid())
							{
								TextureAccess[] fragmentInputAccess = renderGraphPass.fragmentInputAccess;
								if (compilerContextData.AddToFragmentList(renderGraphPass.fragmentInputAccess[k], ptr.firstFragmentInput, ptr.numFragmentInputs))
								{
									ptr.AddFragmentInput(renderGraphPass.fragmentInputAccess[k].textureHandle.handle, compilerContextData);
								}
							}
						}
						ptr.firstRandomAccessResource = compilerContextData.randomAccessResourceData.Length;
						for (int l = 0; l < renderPasses[i].randomAccessResourceMaxIndex + 1; l++)
						{
							ref RenderGraphPass.RandomWriteResourceInfo ptr2 = ref renderPasses[i].randomAccessResource[l];
							if (ptr2.h.IsValid() && compilerContextData.AddToRandomAccessResourceList(ptr2.h, l, ptr2.preserveCounterValue, ptr.firstRandomAccessResource, ptr.numRandomAccessResources))
							{
								ptr.AddRandomAccessResource();
							}
						}
						int numFragments = ptr.numFragments;
					}
					ptr.firstInput = compilerContextData.inputData.Length;
					ptr.firstOutput = compilerContextData.outputData.Length;
					for (int m = 0; m < 3; m++)
					{
						List<ResourceHandle> list = renderGraphPass.resourceWriteLists[m];
						int count = list.Count;
						for (int n = 0; n < count; n++)
						{
							ResourceHandle resourceHandle = list[n];
							if (compilerContextData.UnversionedResourceData(resourceHandle).isImported && !ptr.hasSideEffects)
							{
								ptr.hasSideEffects = true;
								this.toVisitPassIds.Push(i);
							}
							compilerContextData.resources[resourceHandle].SetWritingPass(compilerContextData, resourceHandle, i);
							CompilerContextData compilerContextData2 = compilerContextData;
							PassOutputData passOutputData = new PassOutputData(resourceHandle);
							compilerContextData2.outputData.Add(passOutputData);
							ptr.numOutputs++;
						}
						List<ResourceHandle> list2 = renderGraphPass.resourceReadLists[m];
						int count2 = list2.Count;
						for (int num = 0; num < count2; num++)
						{
							ResourceHandle resourceHandle2 = list2[num];
							compilerContextData.resources[resourceHandle2].RegisterReadingPass(compilerContextData, resourceHandle2, i, ptr.numInputs);
							CompilerContextData compilerContextData3 = compilerContextData;
							PassInputData passInputData = new PassInputData(resourceHandle2);
							compilerContextData3.inputData.Add(passInputData);
							ptr.numInputs++;
						}
						List<ResourceHandle> list3 = renderGraphPass.transientResourceList[m];
						int count3 = list3.Count;
						for (int num2 = 0; num2 < count3; num2++)
						{
							ResourceHandle resourceHandle3 = list3[num2];
							compilerContextData.resources[resourceHandle3].RegisterReadingPass(compilerContextData, resourceHandle3, i, ptr.numInputs);
							CompilerContextData compilerContextData4 = compilerContextData;
							PassInputData passInputData = new PassInputData(resourceHandle3);
							compilerContextData4.inputData.Add(passInputData);
							ptr.numInputs++;
							compilerContextData.resources[resourceHandle3].SetWritingPass(compilerContextData, resourceHandle3, i);
							CompilerContextData compilerContextData5 = compilerContextData;
							PassOutputData passOutputData = new PassOutputData(resourceHandle3);
							compilerContextData5.outputData.Add(passOutputData);
							ptr.numOutputs++;
						}
					}
				}
			}
		}

		private void CullUnusedRenderPasses()
		{
			CompilerContextData compilerContextData = this.contextData;
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_CullNodes)))
			{
				if (!this.graph.disablePassCulling)
				{
					compilerContextData.CullAllPasses(true);
					while (this.toVisitPassIds.Count != 0)
					{
						int index = this.toVisitPassIds.Pop();
						ref PassData ptr = ref compilerContextData.passData.ElementAt(index);
						if (ptr.culled)
						{
							ReadOnlySpan<PassInputData> readOnlySpan = ptr.Inputs(compilerContextData);
							for (int i = 0; i < readOnlySpan.Length; i++)
							{
								ref readonly PassInputData ptr2 = ref readOnlySpan[i];
								int writePassId = compilerContextData.resources[ptr2.resource].writePassId;
								this.toVisitPassIds.Push(writePassId);
							}
							ptr.culled = false;
						}
					}
					int length = compilerContextData.passData.Length;
					for (int j = 0; j < length; j++)
					{
						ref PassData ptr3 = ref compilerContextData.passData.ElementAt(j);
						if (ptr3.culled)
						{
							ReadOnlySpan<PassOutputData> readOnlySpan2 = ptr3.Outputs(compilerContextData);
							for (int i = 0; i < readOnlySpan2.Length; i++)
							{
								ResourceHandle resource = readOnlySpan2[i].resource;
								if (resource.version == compilerContextData.UnversionedResourceData(resource).latestVersionNumber)
								{
									compilerContextData.UnversionedResourceData(resource).latestVersionNumber--;
								}
							}
							ReadOnlySpan<PassInputData> readOnlySpan = ptr3.Inputs(compilerContextData);
							for (int i = 0; i < readOnlySpan.Length; i++)
							{
								ResourceHandle resource2 = readOnlySpan[i].resource;
								compilerContextData.resources[resource2].RemoveReadingPass(compilerContextData, resource2, ptr3.passId);
							}
						}
					}
				}
			}
		}

		private void TryMergeNativePasses()
		{
			CompilerContextData compilerContextData = this.contextData;
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_TryMergeNativePasses)))
			{
				int num = -1;
				for (int i = 0; i < compilerContextData.passData.Length; i++)
				{
					ref PassData ptr = ref compilerContextData.passData.ElementAt(i);
					if (!ptr.culled)
					{
						if (num == -1)
						{
							if (ptr.type == RenderGraphPassType.Raster)
							{
								CompilerContextData compilerContextData2 = compilerContextData;
								NativePassData nativePassData = new NativePassData(ref ptr, compilerContextData);
								compilerContextData2.nativePassData.Add(nativePassData);
								ptr.nativePassIndex = ref compilerContextData.nativePassData.LastIndex<NativePassData>();
								num = ptr.nativePassIndex;
							}
						}
						else
						{
							PassBreakAudit passBreakAudit = this.graph.disablePassMerging ? new PassBreakAudit(PassBreakReason.PassMergingDisabled, i) : NativePassData.TryMerge(this.contextData, num, i);
							if (passBreakAudit.reason != PassBreakReason.Merged)
							{
								this.SetPassStatesForNativePass(num);
								if (passBreakAudit.reason == PassBreakReason.NonRasterPass)
								{
									num = -1;
								}
								else
								{
									CompilerContextData compilerContextData3 = compilerContextData;
									NativePassData nativePassData = new NativePassData(ref ptr, compilerContextData);
									compilerContextData3.nativePassData.Add(nativePassData);
									ptr.nativePassIndex = ref compilerContextData.nativePassData.LastIndex<NativePassData>();
									num = ptr.nativePassIndex;
								}
							}
						}
					}
				}
				if (num >= 0)
				{
					this.SetPassStatesForNativePass(num);
				}
			}
		}

		private void FindResourceUsageRanges()
		{
			CompilerContextData compilerContextData = this.contextData;
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_FindResourceUsageRanges)))
			{
				for (int i = 0; i < compilerContextData.passData.Length; i++)
				{
					ref PassData ptr = ref compilerContextData.passData.ElementAt(i);
					if (!ptr.culled)
					{
						ReadOnlySpan<PassInputData> readOnlySpan = ptr.Inputs(compilerContextData);
						for (int j = 0; j < readOnlySpan.Length; j++)
						{
							ResourceHandle resource = readOnlySpan[j].resource;
							ref ResourceUnversionedData ptr2 = ref compilerContextData.UnversionedResourceData(resource);
							ptr2.lastUsePassID = -1;
							if (resource.version == 0 && ptr2.firstUsePassID < 0)
							{
								ptr2.firstUsePassID = ptr.passId;
								ptr.AddFirstUse(resource, compilerContextData);
							}
							if (ptr2.latestVersionNumber == resource.version)
							{
								ptr2.tag++;
							}
						}
						ReadOnlySpan<PassOutputData> readOnlySpan2 = ptr.Outputs(compilerContextData);
						for (int j = 0; j < readOnlySpan2.Length; j++)
						{
							ResourceHandle resource2 = readOnlySpan2[j].resource;
							ref ResourceUnversionedData ptr3 = ref compilerContextData.UnversionedResourceData(resource2);
							if (resource2.version == 1 && ptr3.firstUsePassID < 0)
							{
								ptr3.firstUsePassID = ptr.passId;
								ptr.AddFirstUse(resource2, compilerContextData);
							}
							if (ptr3.latestVersionNumber == resource2.version)
							{
								ptr3.lastWritePassID = ptr.passId;
							}
						}
					}
				}
				for (int k = 0; k < compilerContextData.passData.Length; k++)
				{
					ref PassData ptr4 = ref compilerContextData.passData.ElementAt(k);
					if (!ptr4.culled)
					{
						ptr4.waitOnGraphicsFencePassId = -1;
						ptr4.insertGraphicsFence = false;
						ReadOnlySpan<PassInputData> readOnlySpan = ptr4.Inputs(compilerContextData);
						for (int j = 0; j < readOnlySpan.Length; j++)
						{
							ResourceHandle resource3 = readOnlySpan[j].resource;
							ref ResourceUnversionedData ptr5 = ref compilerContextData.UnversionedResourceData(resource3);
							if (ptr5.latestVersionNumber == resource3.version)
							{
								int num = ptr5.tag - 1;
								if (num == 0)
								{
									ptr5.lastUsePassID = ptr4.passId;
									ptr4.AddLastUse(resource3, compilerContextData);
								}
								ptr5.tag = num;
							}
							if (ptr4.waitOnGraphicsFencePassId == -1)
							{
								ref ResourceVersionedData ptr6 = ref compilerContextData.VersionedResourceData(resource3);
								if (ptr6.written)
								{
									ref PassData ptr7 = ref compilerContextData.passData.ElementAt(ptr6.writePassId);
									if (ptr7.asyncCompute != ptr4.asyncCompute)
									{
										ptr4.waitOnGraphicsFencePassId = ptr7.passId;
									}
								}
							}
						}
						ReadOnlySpan<PassOutputData> readOnlySpan2 = ptr4.Outputs(compilerContextData);
						for (int j = 0; j < readOnlySpan2.Length; j++)
						{
							ResourceHandle resource4 = readOnlySpan2[j].resource;
							ref ResourceUnversionedData ptr8 = ref compilerContextData.UnversionedResourceData(resource4);
							ref ResourceVersionedData ptr9 = ref compilerContextData.VersionedResourceData(resource4);
							if (ptr8.latestVersionNumber == resource4.version && ptr9.numReaders == 0)
							{
								ptr8.lastUsePassID = ptr4.passId;
								ptr4.AddLastUse(resource4, compilerContextData);
							}
							int numReaders = ptr9.numReaders;
							for (int l = 0; l < numReaders; l++)
							{
								int index = compilerContextData.resources.IndexReader(resource4, l);
								ref ResourceReaderData ptr10 = ref compilerContextData.resources.readerData[resource4.iType].ElementAt(index);
								ref PassData ptr11 = ref compilerContextData.passData.ElementAt(ptr10.passId);
								if (ptr4.asyncCompute != ptr11.asyncCompute)
								{
									ptr4.insertGraphicsFence = true;
									break;
								}
							}
						}
					}
				}
			}
		}

		private void PrepareNativeRenderPasses()
		{
			for (int i = 0; i < this.contextData.nativePassData.Length; i++)
			{
				ref NativePassData nativePass = ref this.contextData.nativePassData.ElementAt(i);
				this.DetermineLoadStoreActions(ref nativePass);
			}
		}

		private static bool IsGlobalTextureInPass(RenderGraphPass pass, ResourceHandle handle)
		{
			foreach (ValueTuple<TextureHandle, int> valueTuple in pass.setGlobalsList)
			{
				if (valueTuple.Item1.handle.index == handle.index)
				{
					return true;
				}
			}
			return false;
		}

		private void DetectMemoryLessResources()
		{
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_DetectMemorylessResources)))
			{
				foreach (NativePassData ptr in this.contextData.NativePasses)
				{
					ReadOnlySpan<PassData> readOnlySpan = ptr.GraphPasses(this.contextData);
					ReadOnlySpan<PassData> readOnlySpan2 = readOnlySpan;
					for (int i = 0; i < readOnlySpan2.Length; i++)
					{
						ref readonly PassData ptr2 = ref readOnlySpan2[i];
						ReadOnlySpan<ResourceHandle> readOnlySpan3 = ptr2.FirstUsedResources(this.contextData);
						for (int j = 0; j < readOnlySpan3.Length; j++)
						{
							ref readonly ResourceHandle ptr3 = ref readOnlySpan3[j];
							ref ResourceUnversionedData ptr4 = ref this.contextData.UnversionedResourceData(ptr3);
							if (ptr3.type == RenderGraphResourceType.Texture && !ptr4.isImported)
							{
								bool flag = NativePassCompiler.IsGlobalTextureInPass(this.graph.m_RenderPasses[ptr2.passId], ptr3);
								ReadOnlySpan<PassData> readOnlySpan4 = readOnlySpan;
								for (int k = 0; k < readOnlySpan4.Length; k++)
								{
									ref readonly PassData ptr5 = ref readOnlySpan4[k];
									ReadOnlySpan<ResourceHandle> readOnlySpan5 = ptr5.LastUsedResources(this.contextData);
									for (int l = 0; l < readOnlySpan5.Length; l++)
									{
										ref readonly ResourceHandle ptr6 = ref readOnlySpan5[l];
										ref ResourceUnversionedData ptr7 = ref this.contextData.UnversionedResourceData(ptr6);
										if (ptr6.type == RenderGraphResourceType.Texture && !ptr7.isImported && ptr3.index == ptr6.index && !flag && (ptr.numNativeSubPasses > 1 || ptr5.IsUsedAsFragment(ptr3, this.contextData)))
										{
											ptr4.memoryLess = true;
											ptr7.memoryLess = true;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		internal static bool IsSameNativeSubPass(ref SubPassDescriptor a, ref SubPassDescriptor b)
		{
			if (a.flags != b.flags || a.colorOutputs.Length != b.colorOutputs.Length || a.inputs.Length != b.inputs.Length)
			{
				return false;
			}
			for (int i = 0; i < a.colorOutputs.Length; i++)
			{
				if (a.colorOutputs[i] != b.colorOutputs[i])
				{
					return false;
				}
			}
			for (int j = 0; j < a.inputs.Length; j++)
			{
				if (a.inputs[j] != b.inputs[j])
				{
					return false;
				}
			}
			return true;
		}

		private void ExecuteInitializeResource(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, in PassData pass)
		{
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_ExecuteInitializeResources)))
			{
				resources.forceManualClearOfResource = true;
				if (pass.type == RenderGraphPassType.Raster && pass.nativePassIndex >= 0)
				{
					if (pass.mergeState == PassMergeState.Begin || pass.mergeState == PassMergeState.None)
					{
						ReadOnlySpan<PassData> readOnlySpan = this.contextData.nativePassData.ElementAt(pass.nativePassIndex).GraphPasses(this.contextData);
						for (int i = 0; i < readOnlySpan.Length; i++)
						{
							ref readonly PassData ptr = ref readOnlySpan[i];
							ReadOnlySpan<ResourceHandle> readOnlySpan2 = ptr.FirstUsedResources(this.contextData);
							for (int j = 0; j < readOnlySpan2.Length; j++)
							{
								ref readonly ResourceHandle ptr2 = ref readOnlySpan2[j];
								ref ResourceUnversionedData ptr3 = ref this.contextData.UnversionedResourceData(ptr2);
								bool flag = ptr.IsUsedAsFragment(ptr2, this.contextData);
								resources.forceManualClearOfResource = !flag;
								if (!ptr3.memoryLess)
								{
									if (!ptr3.isImported)
									{
										resources.CreatePooledResource(rgContext, ptr2.iType, ptr2.index);
									}
									else if (ptr3.clear && resources.forceManualClearOfResource)
									{
										resources.ClearResource(rgContext, ptr2.iType, ptr2.index);
									}
								}
							}
						}
					}
				}
				else
				{
					ReadOnlySpan<ResourceHandle> readOnlySpan2 = pass.FirstUsedResources(this.contextData);
					for (int i = 0; i < readOnlySpan2.Length; i++)
					{
						ref readonly ResourceHandle ptr4 = ref readOnlySpan2[i];
						ref ResourceUnversionedData ptr5 = ref this.contextData.UnversionedResourceData(ptr4);
						if (!ptr5.isImported)
						{
							resources.CreatePooledResource(rgContext, ptr4.iType, ptr4.index);
						}
						else if (ptr5.clear)
						{
							resources.ClearResource(rgContext, ptr4.iType, ptr4.index);
						}
					}
				}
				resources.forceManualClearOfResource = true;
			}
		}

		private void DetermineLoadStoreActions(ref NativePassData nativePass)
		{
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_PrepareNativePass)))
			{
				this.contextData.passData.ElementAt(nativePass.firstGraphPass);
				this.contextData.passData.ElementAt(nativePass.lastGraphPass);
				if (nativePass.fragments.size > 0)
				{
					ref FixedAttachmentArray<PassFragmentData> ptr = ref nativePass.fragments;
					int num = 0;
					for (;;)
					{
						int num2 = num;
						FixedAttachmentArray<PassFragmentData> fixedAttachmentArray = ptr;
						if (num2 >= fixedAttachmentArray.size)
						{
							break;
						}
						fixedAttachmentArray = ptr;
						ref PassFragmentData ptr2 = ref fixedAttachmentArray[num];
						ResourceHandle resource = ptr2.resource;
						bool memoryless = false;
						int mipLevel = ptr2.mipLevel;
						int depthSlice = ptr2.depthSlice;
						RenderBufferLoadAction loadAction = RenderBufferLoadAction.DontCare;
						RenderBufferStoreAction storeAction = RenderBufferStoreAction.DontCare;
						bool flag = ptr2.accessFlags.HasFlag(AccessFlags.Write) && !ptr2.accessFlags.HasFlag(AccessFlags.Discard);
						ref ResourceUnversionedData ptr3 = ref this.contextData.UnversionedResourceData(ptr2.resource);
						bool isImported = ptr3.isImported;
						int lastUsePassID = ptr3.lastUsePassID;
						bool flag2 = lastUsePassID >= nativePass.lastGraphPass + 1;
						if (ptr2.accessFlags.HasFlag(AccessFlags.Read) || flag)
						{
							if (ptr3.firstUsePassID < nativePass.firstGraphPass)
							{
								loadAction = RenderBufferLoadAction.Load;
								if (flag2)
								{
									storeAction = RenderBufferStoreAction.Store;
								}
							}
							else if (isImported)
							{
								if (ptr3.clear)
								{
									loadAction = RenderBufferLoadAction.Clear;
								}
								else
								{
									loadAction = RenderBufferLoadAction.Load;
								}
							}
							else
							{
								loadAction = RenderBufferLoadAction.Clear;
							}
						}
						if (ptr2.accessFlags.HasFlag(AccessFlags.Write))
						{
							if (nativePass.samples <= 1)
							{
								if (flag2)
								{
									storeAction = RenderBufferStoreAction.Store;
								}
								else if (isImported)
								{
									if (ptr3.discard)
									{
										storeAction = RenderBufferStoreAction.DontCare;
									}
									else
									{
										storeAction = RenderBufferStoreAction.Store;
									}
								}
								else
								{
									storeAction = RenderBufferStoreAction.DontCare;
								}
							}
							else
							{
								storeAction = RenderBufferStoreAction.DontCare;
								bool flag3 = ptr3.latestVersionNumber == ptr2.resource.version;
								bool flag4 = isImported && flag3;
								if (lastUsePassID >= nativePass.firstGraphPass + nativePass.numGraphPasses)
								{
									bool flag5 = flag4 && !ptr3.discard;
									bool flag6 = flag4 && !ptr3.bindMS;
									ReadOnlySpan<ResourceReaderData> readOnlySpan = this.contextData.Readers(ptr2.resource);
									for (int i = 0; i < readOnlySpan.Length; i++)
									{
										ref readonly ResourceReaderData ptr4 = ref readOnlySpan[i];
										ref PassData ptr5 = ref this.contextData.passData.ElementAt(ptr4.passId);
										bool flag7 = ptr5.IsUsedAsFragment(ptr2.resource, this.contextData);
										if (ptr5.type == RenderGraphPassType.Unsafe)
										{
											flag5 = true;
											flag6 = !ptr3.bindMS;
											break;
										}
										if (flag7)
										{
											flag5 = true;
										}
										else if (ptr3.bindMS)
										{
											flag5 = true;
										}
										else
										{
											flag6 = true;
										}
									}
									if (flag5 && flag6)
									{
										storeAction = RenderBufferStoreAction.StoreAndResolve;
									}
									else if (flag6)
									{
										storeAction = RenderBufferStoreAction.Resolve;
									}
									else if (flag5)
									{
										storeAction = RenderBufferStoreAction.Store;
									}
								}
								else if (flag4)
								{
									if (ptr3.bindMS)
									{
										if (ptr3.discard)
										{
											storeAction = RenderBufferStoreAction.DontCare;
										}
										else
										{
											storeAction = RenderBufferStoreAction.Store;
										}
									}
									else if (ptr3.discard)
									{
										storeAction = ((nativePass.hasDepth && nativePass.attachments.size == 0) ? RenderBufferStoreAction.DontCare : RenderBufferStoreAction.Resolve);
									}
									else
									{
										storeAction = RenderBufferStoreAction.StoreAndResolve;
									}
								}
							}
						}
						if (ptr3.memoryLess)
						{
							memoryless = true;
						}
						NativePassAttachment nativePassAttachment = new NativePassAttachment(resource, loadAction, storeAction, memoryless, mipLevel, depthSlice);
						nativePass.attachments.Add(nativePassAttachment);
						num++;
					}
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateNativePass(in NativePassData nativePass, int width, int height, int depth, int samples, int attachmentCount)
		{
			if (RenderGraph.enableValidityChecks)
			{
				FixedAttachmentArray<NativePassAttachment> attachments = nativePass.attachments;
				if (attachments.size == 0 || nativePass.numNativeSubPasses == 0)
				{
					throw new Exception("Empty render pass");
				}
				if (width == 0 || height == 0 || depth == 0 || samples == 0 || nativePass.numNativeSubPasses == 0 || attachmentCount == 0)
				{
					throw new Exception("Invalid render pass properties. One or more properties are zero.");
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void ValidateAttachment(in RenderTargetInfo attRenderTargetInfo, RenderGraphResourceRegistry resources, int nativePassWidth, int nativePassHeight, int nativePassMSAASamples, bool isVrs)
		{
			if (RenderGraph.enableValidityChecks)
			{
				if (isVrs)
				{
					Vector2Int allocTileSize = ShadingRateImage.GetAllocTileSize(nativePassWidth, nativePassHeight);
					if (attRenderTargetInfo.width != allocTileSize.x || attRenderTargetInfo.height != allocTileSize.y || attRenderTargetInfo.msaaSamples != 1)
					{
						throw new Exception("Low level rendergraph error: Shading rate image attachment in renderpass does not match!");
					}
				}
				else if (attRenderTargetInfo.width != nativePassWidth || attRenderTargetInfo.height != nativePassHeight || attRenderTargetInfo.msaaSamples != nativePassMSAASamples)
				{
					throw new Exception("Low level rendergraph error: Attachments in renderpass do not match!");
				}
			}
		}

		internal unsafe void ExecuteBeginRenderPass(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, ref NativePassData nativePass)
		{
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_ExecuteBeginRenderpassCommand)))
			{
				ref FixedAttachmentArray<NativePassAttachment> ptr = ref nativePass.attachments;
				int size = ptr.size;
				int width = nativePass.width;
				int height = nativePass.height;
				int volumeDepth = nativePass.volumeDepth;
				int samples = nativePass.samples;
				NativeArray<SubPassDescriptor> subPasses = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SubPassDescriptor>((void*)(this.contextData.nativeSubPassData.GetUnsafeReadOnlyPtr<SubPassDescriptor>() + nativePass.firstNativeSubPass), nativePass.numNativeSubPasses, Allocator.None);
				if (nativePass.hasFoveatedRasterization)
				{
					rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
				}
				if (nativePass.hasShadingRateStates)
				{
					rgContext.cmd.SetShadingRateFragmentSize(nativePass.shadingRateFragmentSize);
					rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Primitive, nativePass.primitiveShadingRateCombiner);
					rgContext.cmd.SetShadingRateCombiner(ShadingRateCombinerStage.Fragment, nativePass.fragmentShadingRateCombiner);
				}
				if (!this.m_BeginRenderPassAttachments.IsCreated)
				{
					this.m_BeginRenderPassAttachments = new NativeList<AttachmentDescriptor>(8, Allocator.Persistent);
				}
				this.m_BeginRenderPassAttachments.Resize(size, NativeArrayOptions.UninitializedMemory);
				for (int i = 0; i < size; i++)
				{
					ref ResourceHandle ptr2 = ref ptr[i].handle;
					RenderTargetInfo renderTargetInfo;
					resources.GetRenderTargetInfo(ptr2, out renderTargetInfo);
					ref AttachmentDescriptor ptr3 = ref this.m_BeginRenderPassAttachments.ElementAt(i);
					ptr3 = new AttachmentDescriptor(renderTargetInfo.format);
					if (!ptr[i].memoryless)
					{
						RTHandle texture = resources.GetTexture(ptr2.index);
						RenderTargetIdentifier renderTargetIdentifier = texture;
						ptr3.loadStoreTarget = new RenderTargetIdentifier(renderTargetIdentifier, ptr[i].mipLevel, CubemapFace.Unknown, ptr[i].depthSlice);
						if (ptr[i].storeAction == RenderBufferStoreAction.Resolve || ptr[i].storeAction == RenderBufferStoreAction.StoreAndResolve)
						{
							ptr3.resolveTarget = texture;
						}
					}
					ptr3.loadAction = ptr[i].loadAction;
					ptr3.storeAction = ptr[i].storeAction;
					if (ptr[i].loadAction == RenderBufferLoadAction.Clear)
					{
						ptr3.clearColor = Color.red;
						ptr3.clearDepth = 1f;
						ptr3.clearStencil = 0U;
						TextureDesc textureResourceDesc = resources.GetTextureResourceDesc(ptr2, true);
						if (i == 0 && nativePass.hasDepth)
						{
							ptr3.clearDepth = 1f;
						}
						else
						{
							ptr3.clearColor = textureResourceDesc.clearColor;
						}
					}
				}
				NativeArray<AttachmentDescriptor> attachments = this.m_BeginRenderPassAttachments.AsArray();
				int depthAttachmentIndex = nativePass.hasDepth ? 0 : -1;
				ReadOnlySpan<byte> empty = ReadOnlySpan<byte>.Empty;
				rgContext.cmd.BeginRenderPass(width, height, volumeDepth, samples, attachments, depthAttachmentIndex, nativePass.shadingRateImageIndex, subPasses, empty);
				CommandBuffer.ThrowOnSetRenderTarget = true;
			}
		}

		private void ExecuteDestroyResource(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, ref PassData pass)
		{
			using (new ProfilingScope(ProfilingSampler.Get<NativePassCompiler.NativeCompilerProfileId>(NativePassCompiler.NativeCompilerProfileId.NRPRGComp_ExecuteDestroyResources)))
			{
				rgContext.renderGraphPool.ReleaseAllTempAlloc();
				if (pass.type == RenderGraphPassType.Raster && pass.nativePassIndex >= 0)
				{
					if (pass.mergeState == PassMergeState.End || pass.mergeState == PassMergeState.None)
					{
						ReadOnlySpan<PassData> readOnlySpan = this.contextData.nativePassData.ElementAt(pass.nativePassIndex).GraphPasses(this.contextData);
						for (int i = 0; i < readOnlySpan.Length; i++)
						{
							ReadOnlySpan<ResourceHandle> readOnlySpan2 = readOnlySpan[i].LastUsedResources(this.contextData);
							for (int j = 0; j < readOnlySpan2.Length; j++)
							{
								ref readonly ResourceHandle ptr = ref readOnlySpan2[j];
								ref ResourceUnversionedData ptr2 = ref this.contextData.UnversionedResourceData(ptr);
								if (!ptr2.isImported && !ptr2.memoryLess)
								{
									resources.ReleasePooledResource(rgContext, ptr.iType, ptr.index);
								}
							}
						}
					}
				}
				else
				{
					ReadOnlySpan<ResourceHandle> readOnlySpan2 = pass.LastUsedResources(this.contextData);
					for (int i = 0; i < readOnlySpan2.Length; i++)
					{
						ref readonly ResourceHandle ptr3 = ref readOnlySpan2[i];
						if (!this.contextData.UnversionedResourceData(ptr3).isImported)
						{
							resources.ReleasePooledResource(rgContext, ptr3.iType, ptr3.index);
						}
					}
				}
			}
		}

		internal void ExecuteSetRandomWriteTarget(in CommandBuffer cmd, RenderGraphResourceRegistry resources, int index, ResourceHandle resource, bool preserveCounterValue = true)
		{
			if (resource.type == RenderGraphResourceType.Texture)
			{
				RTHandle texture = resources.GetTexture(resource.index);
				cmd.SetRandomWriteTarget(index, texture);
				return;
			}
			if (resource.type != RenderGraphResourceType.Buffer)
			{
				throw new Exception(string.Format("Invalid resource type {0}, expected texture or buffer", resource.type));
			}
			GraphicsBuffer buffer = resources.GetBuffer(resource.index);
			if (preserveCounterValue)
			{
				cmd.SetRandomWriteTarget(index, buffer);
				return;
			}
			cmd.SetRandomWriteTarget(index, buffer, false);
		}

		internal void ExecuteGraphNode(ref InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, RenderGraphPass pass)
		{
			rgContext.executingPass = pass;
			if (!pass.HasRenderFunc())
			{
				throw new InvalidOperationException(string.Format("RenderPass {0} was not provided with an execute function.", pass.name));
			}
			using (new ProfilingScope(rgContext.cmd, pass.customSampler))
			{
				pass.Execute(rgContext);
				foreach (ValueTuple<TextureHandle, int> valueTuple in pass.setGlobalsList)
				{
					rgContext.cmd.SetGlobalTexture(valueTuple.Item2, valueTuple.Item1);
				}
			}
		}

		public unsafe void ExecuteGraph(InternalRenderGraphContext rgContext, RenderGraphResourceRegistry resources, in List<RenderGraphPass> passes)
		{
			bool flag = false;
			this.previousCommandBuffer = rgContext.cmd;
			rgContext.cmd.ClearRandomWriteTargets();
			for (int i = 0; i < this.contextData.passData.Length; i++)
			{
				ref PassData ptr = ref this.contextData.passData.ElementAt(i);
				if (!ptr.culled)
				{
					bool flag2 = ptr.type == RenderGraphPassType.Raster;
					this.ExecuteInitializeResource(rgContext, resources, ptr);
					bool flag3 = ptr.type == RenderGraphPassType.Compute && ptr.asyncCompute;
					if (flag3)
					{
						if (!rgContext.contextlessTesting)
						{
							rgContext.renderContext.ExecuteCommandBuffer(rgContext.cmd);
						}
						rgContext.cmd.Clear();
						CommandBuffer commandBuffer = CommandBufferPool.Get("async cmd");
						commandBuffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);
						rgContext.cmd = commandBuffer;
					}
					if (ptr.waitOnGraphicsFencePassId != -1)
					{
						GraphicsFence fence = this.contextData.fences[ptr.waitOnGraphicsFencePassId];
						rgContext.cmd.WaitOnAsyncGraphicsFence(fence);
					}
					bool flag4 = false;
					if (flag2 && ptr.mergeState <= PassMergeState.Begin && ptr.nativePassIndex >= 0)
					{
						ref NativePassData ptr2 = ref this.contextData.nativePassData.ElementAt(ptr.nativePassIndex);
						if (ptr2.fragments.size > 0)
						{
							this.ExecuteBeginRenderPass(rgContext, resources, ref ptr2);
							flag4 = true;
							flag = true;
						}
					}
					if (ptr.mergeState >= PassMergeState.SubPass && ptr.beginNativeSubpass)
					{
						if (!flag)
						{
							throw new Exception("Compiler error: Pass is marked as beginning a native sub pass but no pass is currently active.");
						}
						rgContext.cmd.NextSubPass();
					}
					if (ptr.numRandomAccessResources > 0)
					{
						ReadOnlySpan<PassRandomWriteData> readOnlySpan = ptr.RandomWriteTextures(this.contextData);
						for (int j = 0; j < readOnlySpan.Length; j++)
						{
							PassRandomWriteData passRandomWriteData = *readOnlySpan[j];
							this.ExecuteSetRandomWriteTarget(rgContext.cmd, resources, passRandomWriteData.index, passRandomWriteData.resource, true);
						}
					}
					this.ExecuteGraphNode(ref rgContext, resources, passes[ptr.passId]);
					if (ptr.numRandomAccessResources > 0)
					{
						rgContext.cmd.ClearRandomWriteTargets();
					}
					if (ptr.insertGraphicsFence)
					{
						GraphicsFence value = rgContext.cmd.CreateAsyncGraphicsFence();
						this.contextData.fences[ptr.passId] = value;
					}
					if (flag2)
					{
						if (((ptr.mergeState == PassMergeState.None && flag4) || ptr.mergeState == PassMergeState.End) && ptr.nativePassIndex >= 0)
						{
							ref NativePassData ptr3 = ref this.contextData.nativePassData.ElementAt(ptr.nativePassIndex);
							if (ptr3.fragments.size > 0)
							{
								if (!flag)
								{
									throw new Exception("Compiler error: Generated a subpass pass but no pass is currently active.");
								}
								if (ptr3.hasFoveatedRasterization)
								{
									rgContext.cmd.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
								}
								rgContext.cmd.EndRenderPass();
								CommandBuffer.ThrowOnSetRenderTarget = false;
								flag = false;
								if (ptr3.hasShadingRateStates || ptr3.hasShadingRateImage)
								{
									rgContext.cmd.ResetShadingRate();
								}
							}
						}
					}
					else if (flag3)
					{
						rgContext.renderContext.ExecuteCommandBufferAsync(rgContext.cmd, ComputeQueueType.Background);
						CommandBufferPool.Release(rgContext.cmd);
						rgContext.cmd = this.previousCommandBuffer;
					}
					this.ExecuteDestroyResource(rgContext, resources, ref ptr);
				}
			}
		}

		private unsafe static RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo MakeAttachmentInfo(CompilerContextData ctx, in NativePassData nativePass, int attachmentIndex)
		{
			FixedAttachmentArray<NativePassAttachment> attachments = nativePass.attachments;
			NativePassAttachment nativePassAttachment = *attachments[attachmentIndex];
			ResourceUnversionedData resourceUnversionedData = *ctx.UnversionedResourceData(nativePassAttachment.handle);
			FixedAttachmentArray<LoadAudit> loadAudit = nativePass.loadAudit;
			LoadAudit loadAudit2 = *loadAudit[attachmentIndex];
			string text = LoadAudit.LoadReasonMessages[(int)loadAudit2.reason];
			if (loadAudit2.passId >= 0)
			{
				text = text.Replace("{pass}", "<b>" + ctx.passNames[loadAudit2.passId].name + "</b>");
			}
			FixedAttachmentArray<StoreAudit> storeAudit = nativePass.storeAudit;
			StoreAudit storeAudit2 = *storeAudit[attachmentIndex];
			string text2 = StoreAudit.StoreReasonMessages[(int)storeAudit2.reason];
			if (storeAudit2.passId >= 0)
			{
				text2 = text2.Replace("{pass}", "<b>" + ctx.passNames[storeAudit2.passId].name + "</b>");
			}
			string text3 = string.Empty;
			if (storeAudit2.msaaReason != StoreReason.InvalidReason && storeAudit2.msaaReason != StoreReason.NoMSAABuffer)
			{
				text3 = StoreAudit.StoreReasonMessages[(int)storeAudit2.msaaReason];
				if (storeAudit2.msaaPassId >= 0)
				{
					text3 = text3.Replace("{pass}", "<b>" + ctx.passNames[storeAudit2.msaaPassId].name + "</b>");
				}
			}
			return new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo
			{
				resourceName = resourceUnversionedData.GetName(ctx, nativePassAttachment.handle),
				attachmentIndex = attachmentIndex,
				loadReason = text,
				storeReason = text2,
				storeMsaaReason = text3,
				attachment = nativePassAttachment
			};
		}

		internal static string MakePassBreakInfoMessage(CompilerContextData ctx, in NativePassData nativePass)
		{
			string str = "";
			if (nativePass.breakAudit.breakPass >= 0)
			{
				str = str + "Failed to merge " + ctx.passNames[nativePass.breakAudit.breakPass].name + " into this native pass.\n";
			}
			return str + PassBreakAudit.BreakReasonMessages[(int)nativePass.breakAudit.reason];
		}

		internal static string MakePassMergeMessage(CompilerContextData ctx, in PassData pass, in PassData prevPass, PassBreakAudit mergeResult)
		{
			string text = (mergeResult.reason == PassBreakReason.Merged) ? "The passes are <b>compatible</b> to be merged.\n\n" : "The passes are <b>incompatible</b> to be merged.\n\n";
			PassData passData = pass;
			string text2 = NativePassCompiler.InjectSpaces(passData.GetName(ctx).name);
			passData = prevPass;
			string text3 = NativePassCompiler.InjectSpaces(passData.GetName(ctx).name);
			switch (mergeResult.reason)
			{
			case PassBreakReason.TargetSizeMismatch:
				return text + "The fragment attachments of the passes have different sizes or sample counts.\n" + string.Format("- {0}: {1}x{2}, {3} sample(s).\n", new object[]
				{
					text3,
					prevPass.fragmentInfoWidth,
					prevPass.fragmentInfoHeight,
					prevPass.fragmentInfoSamples
				}) + string.Format("- {0}: {1}x{2}, {3} sample(s).", new object[]
				{
					text2,
					pass.fragmentInfoWidth,
					pass.fragmentInfoHeight,
					pass.fragmentInfoSamples
				});
			case PassBreakReason.NextPassReadsTexture:
				return string.Concat(new string[]
				{
					text,
					text3,
					" output is sampled by ",
					text2,
					" as a regular texture, the pass needs to break."
				});
			case PassBreakReason.NextPassTargetsTexture:
				return string.Concat(new string[]
				{
					text,
					text3,
					" reads a texture that ",
					text2,
					" targets to, the pass needs to break."
				});
			case PassBreakReason.NonRasterPass:
				return text + string.Format("{0} is type {1}. Only Raster passes can be merged.", text3, prevPass.type);
			case PassBreakReason.DifferentDepthTextures:
				return string.Concat(new string[]
				{
					text,
					text3,
					" uses a different depth buffer than ",
					text2,
					"."
				});
			case PassBreakReason.AttachmentLimitReached:
				return text + string.Format("Merging the passes would use more than {0} attachments.", 8);
			case PassBreakReason.SubPassLimitReached:
				return text + string.Format("Merging the passes would use more than {0} native subpasses.", 8);
			case PassBreakReason.EndOfGraph:
				return text + "The pass is the last pass in the graph.";
			case PassBreakReason.DifferentShadingRateImages:
				return string.Concat(new string[]
				{
					text,
					text3,
					" uses a different shading rate image than ",
					text2,
					"."
				});
			case PassBreakReason.DifferentShadingRateStates:
				return string.Concat(new string[]
				{
					text,
					text3,
					" uses different shading rate states than ",
					text2,
					"."
				});
			case PassBreakReason.PassMergingDisabled:
				return text + "The pass merging is disabled.";
			case PassBreakReason.Merged:
				if (pass.nativePassIndex == prevPass.nativePassIndex && pass.mergeState != PassMergeState.None)
				{
					return text + "Passes are merged.";
				}
				return text + "Passes can be merged but are not recorded consecutively.";
			}
			throw new ArgumentOutOfRangeException();
		}

		private static string InjectSpaces(string camelCaseString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < camelCaseString.Length; i++)
			{
				if (char.IsUpper(camelCaseString[i]) && i != 0 && char.IsLower(camelCaseString[i - 1]))
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(camelCaseString[i]);
			}
			return stringBuilder.ToString();
		}

		internal unsafe void GenerateNativeCompilerDebugData(ref RenderGraph.DebugData debugData)
		{
			ref CompilerContextData ptr = ref this.contextData;
			debugData.isNRPCompiler = true;
			Dictionary<ValueTuple<RenderGraphResourceType, int>, List<int>> dictionary = new Dictionary<ValueTuple<RenderGraphResourceType, int>, List<int>>();
			Dictionary<ValueTuple<RenderGraphResourceType, int>, List<int>> dictionary2 = new Dictionary<ValueTuple<RenderGraphResourceType, int>, List<int>>();
			foreach (RenderGraphPass renderGraphPass in this.graph.m_RenderPasses)
			{
				for (int i = 0; i < 3; i++)
				{
					int length = ptr.resources.unversionedData[i].Length;
					for (int j = 0; j < length; j++)
					{
						foreach (ResourceHandle item in renderGraphPass.resourceReadLists[i])
						{
							if (!renderGraphPass.implicitReadsList.Contains(item) && item.type == (RenderGraphResourceType)i && item.index == j)
							{
								ValueTuple<RenderGraphResourceType, int> key = new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)i, j);
								if (!dictionary.ContainsKey(key))
								{
									dictionary[key] = new List<int>();
								}
								dictionary[key].Add(renderGraphPass.index);
							}
						}
						foreach (ResourceHandle resourceHandle in renderGraphPass.resourceWriteLists[i])
						{
							if (resourceHandle.type == (RenderGraphResourceType)i && resourceHandle.index == j)
							{
								ValueTuple<RenderGraphResourceType, int> key2 = new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)i, j);
								if (!dictionary2.ContainsKey(key2))
								{
									dictionary2[key2] = new List<int>();
								}
								dictionary2[key2].Add(renderGraphPass.index);
							}
						}
						foreach (ResourceHandle resourceHandle2 in renderGraphPass.transientResourceList[i])
						{
							if (resourceHandle2.type == (RenderGraphResourceType)i && resourceHandle2.index == j)
							{
								ValueTuple<RenderGraphResourceType, int> key3 = new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)i, j);
								if (!dictionary.ContainsKey(key3))
								{
									dictionary[key3] = new List<int>();
								}
								dictionary[key3].Add(renderGraphPass.index);
								if (!dictionary2.ContainsKey(key3))
								{
									dictionary2[key3] = new List<int>();
								}
								dictionary2[key3].Add(renderGraphPass.index);
							}
						}
					}
				}
			}
			for (int k = 0; k < 3; k++)
			{
				int length2 = ptr.resources.unversionedData[k].Length;
				for (int l = 0; l < length2; l++)
				{
					ref ResourceUnversionedData ptr2 = ref ptr.resources.unversionedData[k].ElementAt(l);
					RenderGraph.DebugData.ResourceData resourceData = default(RenderGraph.DebugData.ResourceData);
					RenderGraphResourceType renderGraphResourceType = (RenderGraphResourceType)k;
					bool flag = l == 0;
					if (!flag)
					{
						string name = ptr.resources.resourceNames[k][l].name;
						resourceData.name = ((!string.IsNullOrEmpty(name)) ? name : "(unnamed)");
						resourceData.imported = ptr2.isImported;
					}
					else
					{
						resourceData.name = "<null>";
						resourceData.imported = true;
					}
					RenderTargetInfo renderTargetInfo = default(RenderTargetInfo);
					if (renderGraphResourceType == RenderGraphResourceType.Texture && !flag)
					{
						ResourceHandle resourceHandle3 = new ResourceHandle(l, renderGraphResourceType, false);
						try
						{
							this.graph.m_ResourcesForDebugOnly.GetRenderTargetInfo(resourceHandle3, out renderTargetInfo);
						}
						catch (Exception)
						{
						}
					}
					resourceData.creationPassIndex = ptr2.firstUsePassID;
					resourceData.releasePassIndex = ptr2.lastUsePassID;
					resourceData.textureData = new RenderGraph.DebugData.TextureResourceData();
					resourceData.textureData.width = ptr2.width;
					resourceData.textureData.height = ptr2.height;
					resourceData.textureData.depth = ptr2.volumeDepth;
					resourceData.textureData.samples = ptr2.msaaSamples;
					resourceData.textureData.format = renderTargetInfo.format;
					resourceData.textureData.bindMS = ptr2.bindMS;
					resourceData.textureData.clearBuffer = ptr2.clear;
					resourceData.memoryless = ptr2.memoryLess;
					resourceData.consumerList = new List<int>();
					resourceData.producerList = new List<int>();
					if (dictionary.ContainsKey(new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)k, l)))
					{
						resourceData.consumerList = dictionary[new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)k, l)];
					}
					if (dictionary2.ContainsKey(new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)k, l)))
					{
						resourceData.producerList = dictionary2[new ValueTuple<RenderGraphResourceType, int>((RenderGraphResourceType)k, l)];
					}
					debugData.resourceLists[k].Add(resourceData);
				}
			}
			for (int m = 0; m < ptr.passData.Length; m++)
			{
				RenderGraphPass renderGraphPass2 = this.graph.m_RenderPasses[m];
				ref PassData ptr3 = ref ptr.passData.ElementAt(m);
				string name2 = NativePassCompiler.InjectSpaces(ptr3.GetName(ptr).name);
				RenderGraph.DebugData.PassData passData = default(RenderGraph.DebugData.PassData);
				passData.name = name2;
				passData.type = ptr3.type;
				passData.culled = ptr3.culled;
				passData.async = ptr3.asyncCompute;
				passData.nativeSubPassIndex = ptr3.nativeSubPassIndex;
				passData.generateDebugData = renderGraphPass2.generateDebugData;
				passData.resourceReadLists = new List<int>[3];
				passData.resourceWriteLists = new List<int>[3];
				RenderGraph.DebugData.s_PassScriptMetadata.TryGetValue(renderGraphPass2, out passData.scriptInfo);
				passData.syncFromPassIndex = -1;
				passData.syncToPassIndex = -1;
				passData.nrpInfo = new RenderGraph.DebugData.PassData.NRPInfo();
				passData.nrpInfo.width = ptr3.fragmentInfoWidth;
				passData.nrpInfo.height = ptr3.fragmentInfoHeight;
				passData.nrpInfo.volumeDepth = ptr3.fragmentInfoVolumeDepth;
				passData.nrpInfo.samples = ptr3.fragmentInfoSamples;
				passData.nrpInfo.hasDepth = ptr3.fragmentInfoHasDepth;
				foreach (ValueTuple<TextureHandle, int> valueTuple in renderGraphPass2.setGlobalsList)
				{
					passData.nrpInfo.setGlobals.Add(valueTuple.Item1.handle.index);
				}
				for (int n = 0; n < 3; n++)
				{
					passData.resourceReadLists[n] = new List<int>();
					passData.resourceWriteLists[n] = new List<int>();
					foreach (ResourceHandle item2 in renderGraphPass2.resourceReadLists[n])
					{
						if (!renderGraphPass2.implicitReadsList.Contains(item2))
						{
							passData.resourceReadLists[n].Add(item2.index);
						}
					}
					foreach (ResourceHandle resourceHandle4 in renderGraphPass2.resourceWriteLists[n])
					{
						passData.resourceWriteLists[n].Add(resourceHandle4.index);
					}
				}
				ReadOnlySpan<PassFragmentData> readOnlySpan = ptr3.FragmentInputs(ptr);
				for (int num = 0; num < readOnlySpan.Length; num++)
				{
					PassFragmentData passFragmentData = *readOnlySpan[num];
					passData.nrpInfo.textureFBFetchList.Add(passFragmentData.resource.index);
				}
				debugData.passList.Add(passData);
			}
			foreach (NativePassData ptr4 in ptr.NativePasses)
			{
				List<int> list = new List<int>();
				for (int num2 = ptr4.firstGraphPass; num2 < ptr4.lastGraphPass + 1; num2++)
				{
					list.Add(num2);
				}
				if (ptr4.numGraphPasses > 0)
				{
					RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo nativeRenderPassInfo = new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo();
					nativeRenderPassInfo.passBreakReasoning = NativePassCompiler.MakePassBreakInfoMessage(ptr, ptr4);
					nativeRenderPassInfo.attachmentInfos = new List<RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.AttachmentInfo>();
					int num3 = 0;
					for (;;)
					{
						int num4 = num3;
						FixedAttachmentArray<NativePassAttachment> attachments = ptr4.attachments;
						if (num4 >= attachments.size)
						{
							break;
						}
						nativeRenderPassInfo.attachmentInfos.Add(NativePassCompiler.MakeAttachmentInfo(ptr, ptr4, num3));
						num3++;
					}
					nativeRenderPassInfo.passCompatibility = new Dictionary<int, RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo>();
					nativeRenderPassInfo.mergedPassIds = list;
					for (int num5 = 0; num5 < list.Count; num5++)
					{
						int index = list[num5];
						RenderGraph.DebugData.PassData passData2 = debugData.passList[index];
						passData2.nrpInfo.nativePassInfo = nativeRenderPassInfo;
						debugData.passList[index] = passData2;
					}
				}
			}
			for (int num6 = 0; num6 < ptr.passData.Length; num6++)
			{
				ref PassData ptr5 = ref ptr.passData.ElementAt(num6);
				RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo nativePassInfo = debugData.passList[ptr5.passId].nrpInfo.nativePassInfo;
				if (nativePassInfo != null)
				{
					ReadOnlySpan<PassInputData> readOnlySpan2 = ptr5.Inputs(ptr);
					for (int num = 0; num < readOnlySpan2.Length; num++)
					{
						ref readonly PassInputData ptr6 = ref readOnlySpan2[num];
						ref ResourceVersionedData ptr7 = ref ptr.VersionedResourceData(ptr6.resource);
						if (ptr7.written)
						{
							PassData passData3 = ptr.passData[ptr7.writePassId];
							PassBreakAudit passBreakAudit = (passData3.nativePassIndex >= 0) ? NativePassData.CanMerge(ptr, passData3.nativePassIndex, ptr5.passId) : new PassBreakAudit(PassBreakReason.NonRasterPass, ptr5.passId);
							string message = "This pass writes to a resource that is read by the currently selected pass.\n\n" + NativePassCompiler.MakePassMergeMessage(ptr, ptr5, passData3, passBreakAudit);
							nativePassInfo.passCompatibility.TryAdd(passData3.passId, new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo
							{
								message = message,
								isCompatible = (passBreakAudit.reason == PassBreakReason.Merged)
							});
						}
					}
					if (ptr5.nativePassIndex >= 0)
					{
						ReadOnlySpan<PassOutputData> readOnlySpan3 = ptr5.Outputs(ptr);
						for (int num = 0; num < readOnlySpan3.Length; num++)
						{
							ref readonly PassOutputData ptr8 = ref readOnlySpan3[num];
							if (ptr.UnversionedResourceData(ptr8.resource).lastUsePassID != ptr5.passId)
							{
								int numReaders = ptr.VersionedResourceData(ptr8.resource).numReaders;
								for (int num7 = 0; num7 < numReaders; num7++)
								{
									int index2 = ptr.resources.IndexReader(ptr8.resource, num7);
									ref ResourceReaderData ptr9 = ref ptr.resources.readerData[ptr8.resource.iType].ElementAt(index2);
									PassData passData4 = ptr.passData[ptr9.passId];
									PassBreakAudit passBreakAudit2 = NativePassData.CanMerge(ptr, ptr5.nativePassIndex, passData4.passId);
									string message2 = "This pass reads a resource that is written to by the currently selected pass.\n\n" + NativePassCompiler.MakePassMergeMessage(ptr, passData4, ptr5, passBreakAudit2);
									nativePassInfo.passCompatibility.TryAdd(passData4.passId, new RenderGraph.DebugData.PassData.NRPInfo.NativeRenderPassInfo.PassCompatibilityInfo
									{
										message = message2,
										isCompatible = (passBreakAudit2.reason == PassBreakReason.Merged)
									});
								}
							}
						}
					}
				}
			}
		}

		internal NativePassCompiler.RenderGraphInputInfo graph;

		internal CompilerContextData contextData;

		internal CompilerContextData defaultContextData;

		internal CommandBuffer previousCommandBuffer;

		private Stack<int> toVisitPassIds;

		private RenderGraphCompilationCache m_CompilationCache;

		internal const int k_EstimatedPassCount = 100;

		internal const int k_MaxSubpass = 8;

		private NativeList<AttachmentDescriptor> m_BeginRenderPassAttachments;

		private const int ArbitraryMaxNbMergedPasses = 16;

		private DynamicArray<Name> graphPassNamesForDebug = new DynamicArray<Name>(16);

		internal struct RenderGraphInputInfo
		{
			public RenderGraphResourceRegistry m_ResourcesForDebugOnly;

			public List<RenderGraphPass> m_RenderPasses;

			public string debugName;

			public bool disablePassCulling;

			public bool disablePassMerging;
		}

		internal enum NativeCompilerProfileId
		{
			NRPRGComp_PrepareNativePass,
			NRPRGComp_SetupContextData,
			NRPRGComp_BuildGraph,
			NRPRGComp_CullNodes,
			NRPRGComp_TryMergeNativePasses,
			NRPRGComp_FindResourceUsageRanges,
			NRPRGComp_DetectMemorylessResources,
			NRPRGComp_ExecuteInitializeResources,
			NRPRGComp_ExecuteBeginRenderpassCommand,
			NRPRGComp_ExecuteDestroyResources
		}
	}
}
