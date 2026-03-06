using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.VFX;

namespace UnityEngine.Rendering.Universal
{
	public abstract class ScriptableRenderer : IDisposable
	{
		[Obsolete("cameraDepth has been renamed to cameraDepthTarget. (UnityUpgradable) -> cameraDepthTarget", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RenderTargetIdentifier cameraDepth
		{
			get
			{
				return this.m_CameraDepthTarget.nameID;
			}
		}

		internal void ResetNativeRenderPassFrameData()
		{
			if (this.m_MergeableRenderPassesMapArrays == null)
			{
				this.m_MergeableRenderPassesMapArrays = new int[10][];
			}
			for (int i = 0; i < 10; i++)
			{
				if (this.m_MergeableRenderPassesMapArrays[i] == null)
				{
					this.m_MergeableRenderPassesMapArrays[i] = new int[20];
				}
				for (int j = 0; j < 20; j++)
				{
					this.m_MergeableRenderPassesMapArrays[i][j] = -1;
				}
			}
			this.m_firstPassIndexOfLastMergeableGroup = 0;
		}

		internal void SetupNativeRenderPassFrameData(UniversalCameraData cameraData, bool isRenderPassEnabled)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.setupFrameData))
			{
				int count = this.m_ActiveRenderPassQueue.Count;
				this.m_MergeableRenderPassesMap.Clear();
				this.m_RenderPassesAttachmentCount.Clear();
				uint num = 0U;
				for (int i = 0; i < this.m_ActiveRenderPassQueue.Count; i++)
				{
					ScriptableRenderPass scriptableRenderPass = this.m_ActiveRenderPassQueue[i];
					if (this.IsRenderPassEnabled(scriptableRenderPass))
					{
						if (i >= 20)
						{
							Debug.LogError(string.Format("Exceeded the maximum number of Render Passes (${0}). Please consider using Render Graph to support a higher number of render passes with Native RenderPass, note support will be enabled by default.", 20));
							return;
						}
						scriptableRenderPass.renderPassQueueIndex = i;
						ScriptableRenderer.RenderPassDescriptor desc = this.InitializeRenderPassDescriptor(cameraData, scriptableRenderPass);
						Hash128 hash = ScriptableRenderer.CreateRenderPassHash(desc, num);
						this.m_PassIndexToPassHash[i] = hash;
						if (!this.m_MergeableRenderPassesMap.ContainsKey(hash))
						{
							this.m_MergeableRenderPassesMap.Add(hash, this.m_MergeableRenderPassesMapArrays[this.m_MergeableRenderPassesMap.Count]);
							this.m_RenderPassesAttachmentCount.Add(hash, 0);
							this.m_firstPassIndexOfLastMergeableGroup = i;
						}
						else if (this.m_MergeableRenderPassesMap[hash][ScriptableRenderer.GetValidPassIndexCount(this.m_MergeableRenderPassesMap[hash]) - 1] != i - 1)
						{
							num += 1U;
							hash = ScriptableRenderer.CreateRenderPassHash(desc, num);
							this.m_PassIndexToPassHash[i] = hash;
							this.m_MergeableRenderPassesMap.Add(hash, this.m_MergeableRenderPassesMapArrays[this.m_MergeableRenderPassesMap.Count]);
							this.m_RenderPassesAttachmentCount.Add(hash, 0);
							this.m_firstPassIndexOfLastMergeableGroup = i;
						}
						this.m_MergeableRenderPassesMap[hash][ScriptableRenderer.GetValidPassIndexCount(this.m_MergeableRenderPassesMap[hash])] = i;
					}
				}
				for (int j = 0; j < this.m_ActiveRenderPassQueue.Count; j++)
				{
					this.m_ActiveRenderPassQueue[j].m_ColorAttachmentIndices = new NativeArray<int>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
					this.m_ActiveRenderPassQueue[j].m_InputAttachmentIndices = new NativeArray<int>(8, Allocator.Temp, NativeArrayOptions.ClearMemory);
				}
			}
		}

		internal void UpdateFinalStoreActions(int[] currentMergeablePasses, UniversalCameraData cameraData, bool isLastMergeableGroup)
		{
			for (int i = 0; i < this.m_FinalColorStoreAction.Length; i++)
			{
				this.m_FinalColorStoreAction[i] = RenderBufferStoreAction.Store;
			}
			this.m_FinalDepthStoreAction = RenderBufferStoreAction.Store;
			foreach (int num in currentMergeablePasses)
			{
				if (!ScriptableRenderer.m_UseOptimizedStoreActions || num == -1)
				{
					break;
				}
				ScriptableRenderPass scriptableRenderPass = this.m_ActiveRenderPassQueue[num];
				int num2 = scriptableRenderPass.overrideCameraTarget ? ScriptableRenderer.GetFirstAllocatedRTHandle(scriptableRenderPass).rt.descriptor.msaaSamples : ((cameraData.targetTexture != null) ? cameraData.targetTexture.descriptor.msaaSamples : cameraData.cameraTargetDescriptor.msaaSamples);
				bool flag = cameraData.renderer != null && cameraData.renderer.supportedRenderingFeatures.msaa;
				if (!cameraData.camera.allowMSAA || !flag)
				{
					num2 = 1;
				}
				for (int k = 0; k < this.m_FinalColorStoreAction.Length; k++)
				{
					if (this.m_FinalColorStoreAction[k] == RenderBufferStoreAction.Store || this.m_FinalColorStoreAction[k] == RenderBufferStoreAction.StoreAndResolve || scriptableRenderPass.overriddenColorStoreActions[k])
					{
						this.m_FinalColorStoreAction[k] = scriptableRenderPass.colorStoreActions[k];
					}
					if (num2 > 1)
					{
						if (this.m_FinalColorStoreAction[k] == RenderBufferStoreAction.Store)
						{
							this.m_FinalColorStoreAction[k] = RenderBufferStoreAction.StoreAndResolve;
						}
						else if (this.m_FinalColorStoreAction[k] == RenderBufferStoreAction.DontCare)
						{
							this.m_FinalColorStoreAction[k] = RenderBufferStoreAction.Resolve;
						}
						else if (isLastMergeableGroup && this.m_FinalColorStoreAction[k] == RenderBufferStoreAction.Resolve)
						{
							this.m_FinalColorStoreAction[k] = RenderBufferStoreAction.StoreAndResolve;
						}
					}
				}
				if (this.m_FinalDepthStoreAction == RenderBufferStoreAction.Store || (this.m_FinalDepthStoreAction == RenderBufferStoreAction.StoreAndResolve && scriptableRenderPass.depthStoreAction == RenderBufferStoreAction.Resolve) || scriptableRenderPass.overriddenDepthStoreAction)
				{
					this.m_FinalDepthStoreAction = scriptableRenderPass.depthStoreAction;
				}
			}
		}

		internal void SetNativeRenderPassMRTAttachmentList(ScriptableRenderPass renderPass, UniversalCameraData cameraData, bool needCustomCameraColorClear, ClearFlag cameraClearFlag)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.setMRTAttachmentsList))
			{
				int renderPassQueueIndex = renderPass.renderPassQueueIndex;
				Hash128 hash = this.m_PassIndexToPassHash[renderPassQueueIndex];
				int[] array = this.m_MergeableRenderPassesMap[hash];
				if (array.First<int>() == renderPassQueueIndex)
				{
					this.m_RenderPassesAttachmentCount[hash] = 0;
					this.UpdateFinalStoreActions(array, cameraData, renderPassQueueIndex == this.m_firstPassIndexOfLastMergeableGroup);
					int num = 0;
					bool flag = false;
					foreach (int num2 in array)
					{
						if (num2 == -1)
						{
							break;
						}
						ScriptableRenderPass scriptableRenderPass = this.m_ActiveRenderPassQueue[num2];
						for (int j = 0; j < scriptableRenderPass.m_ColorAttachmentIndices.Length; j++)
						{
							scriptableRenderPass.m_ColorAttachmentIndices[j] = -1;
						}
						for (int k = 0; k < scriptableRenderPass.m_InputAttachmentIndices.Length; k++)
						{
							scriptableRenderPass.m_InputAttachmentIndices[k] = -1;
						}
						uint validColorBufferCount = RenderingUtils.GetValidColorBufferCount(scriptableRenderPass.colorAttachmentHandles);
						int num3 = 0;
						while ((long)num3 < (long)((ulong)validColorBufferCount))
						{
							AttachmentDescriptor attachmentDescriptor = new AttachmentDescriptor((scriptableRenderPass.renderTargetFormat[num3] != GraphicsFormat.None) ? scriptableRenderPass.renderTargetFormat[num3] : UniversalRenderPipeline.MakeRenderTextureGraphicsFormat(cameraData.isHdrEnabled, cameraData.hdrColorBufferPrecision, Graphics.preserveFramebufferAlpha));
							RTHandle rthandle = scriptableRenderPass.overrideCameraTarget ? scriptableRenderPass.colorAttachmentHandles[num3] : this.m_CameraColorTarget;
							int num4 = ScriptableRenderer.FindAttachmentDescriptorIndexInList(rthandle.nameID, this.m_ActiveColorAttachmentDescriptors);
							if (ScriptableRenderer.m_UseOptimizedStoreActions)
							{
								attachmentDescriptor.storeAction = this.m_FinalColorStoreAction[num3];
							}
							if (num4 == -1)
							{
								this.m_ActiveColorAttachmentDescriptors[num] = attachmentDescriptor;
								bool flag2 = (scriptableRenderPass.clearFlag & ClearFlag.Color) > ClearFlag.None;
								this.m_ActiveColorAttachmentDescriptors[num].ConfigureTarget(rthandle.nameID, !flag2, true);
								if (scriptableRenderPass.colorAttachmentHandles[num3].nameID == this.m_CameraColorTarget.nameID && needCustomCameraColorClear && (cameraClearFlag & ClearFlag.Color) != ClearFlag.None)
								{
									this.m_ActiveColorAttachmentDescriptors[num].ConfigureClear(cameraData.backgroundColor, 1f, 0U);
								}
								else if (flag2)
								{
									this.m_ActiveColorAttachmentDescriptors[num].ConfigureClear(CoreUtils.ConvertSRGBToActiveColorSpace(scriptableRenderPass.clearColor), 1f, 0U);
								}
								scriptableRenderPass.m_ColorAttachmentIndices[num3] = num;
								num++;
								Dictionary<Hash128, int> renderPassesAttachmentCount = this.m_RenderPassesAttachmentCount;
								Hash128 key = hash;
								int num5 = renderPassesAttachmentCount[key];
								renderPassesAttachmentCount[key] = num5 + 1;
							}
							else
							{
								scriptableRenderPass.m_ColorAttachmentIndices[num3] = num4;
							}
							num3++;
						}
						if (ScriptableRenderer.PassHasInputAttachments(scriptableRenderPass))
						{
							flag = true;
							this.SetupInputAttachmentIndices(scriptableRenderPass);
						}
						this.m_ActiveDepthAttachmentDescriptor = new AttachmentDescriptor(SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil));
						bool flag3 = (cameraClearFlag & ClearFlag.DepthStencil) > ClearFlag.None;
						this.m_ActiveDepthAttachmentDescriptor.ConfigureTarget(scriptableRenderPass.overrideCameraTarget ? scriptableRenderPass.depthAttachmentHandle.nameID : this.m_CameraDepthTarget.nameID, !flag3, true);
						if (flag3)
						{
							this.m_ActiveDepthAttachmentDescriptor.ConfigureClear(Color.black, 1f, 0U);
						}
						if (ScriptableRenderer.m_UseOptimizedStoreActions)
						{
							this.m_ActiveDepthAttachmentDescriptor.storeAction = this.m_FinalDepthStoreAction;
						}
					}
					if (flag)
					{
						this.SetupTransientInputAttachments(this.m_RenderPassesAttachmentCount[hash]);
					}
				}
			}
		}

		private bool IsDepthOnlyRenderTexture(RenderTexture t)
		{
			return t.graphicsFormat == GraphicsFormat.None;
		}

		internal void SetNativeRenderPassAttachmentList(ScriptableRenderPass renderPass, UniversalCameraData cameraData, RTHandle passColorAttachment, RTHandle passDepthAttachment, ClearFlag finalClearFlag, Color finalClearColor)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.setAttachmentList))
			{
				int renderPassQueueIndex = renderPass.renderPassQueueIndex;
				Hash128 hash = this.m_PassIndexToPassHash[renderPassQueueIndex];
				int[] array = this.m_MergeableRenderPassesMap[hash];
				if (array.First<int>() == renderPassQueueIndex)
				{
					this.m_RenderPassesAttachmentCount[hash] = 0;
					this.UpdateFinalStoreActions(array, cameraData, renderPassQueueIndex == this.m_firstPassIndexOfLastMergeableGroup);
					int num = 0;
					foreach (int num2 in array)
					{
						if (num2 == -1)
						{
							break;
						}
						ScriptableRenderPass scriptableRenderPass = this.m_ActiveRenderPassQueue[num2];
						for (int j = 0; j < scriptableRenderPass.m_ColorAttachmentIndices.Length; j++)
						{
							scriptableRenderPass.m_ColorAttachmentIndices[j] = -1;
						}
						bool flag = cameraData.targetTexture != null;
						bool flag2 = (scriptableRenderPass.colorAttachmentHandle.rt != null && this.IsDepthOnlyRenderTexture(scriptableRenderPass.colorAttachmentHandle.rt)) || (flag && this.IsDepthOnlyRenderTexture(cameraData.targetTexture));
						AttachmentDescriptor attachmentDescriptor;
						int msaaSamples;
						RenderTargetIdentifier target;
						if (new RenderTargetIdentifier(passColorAttachment.nameID, 0, CubemapFace.Unknown, 0) != BuiltinRenderTextureType.CameraTarget)
						{
							attachmentDescriptor = new AttachmentDescriptor(flag2 ? passColorAttachment.rt.descriptor.depthStencilFormat : passColorAttachment.rt.descriptor.graphicsFormat);
							msaaSamples = passColorAttachment.rt.descriptor.msaaSamples;
							target = passColorAttachment.nameID;
						}
						else
						{
							attachmentDescriptor = new AttachmentDescriptor((scriptableRenderPass.renderTargetFormat[0] != GraphicsFormat.None) ? scriptableRenderPass.renderTargetFormat[0] : UniversalRenderPipeline.MakeRenderTextureGraphicsFormat(cameraData.isHdrEnabled, cameraData.hdrColorBufferPrecision, Graphics.preserveFramebufferAlpha));
							msaaSamples = cameraData.cameraTargetDescriptor.msaaSamples;
							target = (flag ? new RenderTargetIdentifier(cameraData.targetTexture) : BuiltinRenderTextureType.CameraTarget);
						}
						attachmentDescriptor.ConfigureTarget(target, (finalClearFlag & ClearFlag.Color) == ClearFlag.None, true);
						if (ScriptableRenderer.PassHasInputAttachments(scriptableRenderPass))
						{
							this.SetupInputAttachmentIndices(scriptableRenderPass);
						}
						this.m_ActiveDepthAttachmentDescriptor = new AttachmentDescriptor(SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil));
						this.m_ActiveDepthAttachmentDescriptor.ConfigureTarget((passDepthAttachment.nameID != BuiltinRenderTextureType.CameraTarget) ? passDepthAttachment.nameID : (flag ? new RenderTargetIdentifier(cameraData.targetTexture.depthBuffer, 0, CubemapFace.Unknown, 0) : BuiltinRenderTextureType.Depth), (finalClearFlag & ClearFlag.Depth) == ClearFlag.None, true);
						if (finalClearFlag != ClearFlag.None)
						{
							if (cameraData.renderType != CameraRenderType.Overlay || (flag2 && (finalClearFlag & ClearFlag.Color) != ClearFlag.None))
							{
								attachmentDescriptor.ConfigureClear(finalClearColor, 1f, 0U);
							}
							if ((finalClearFlag & ClearFlag.Depth) != ClearFlag.None)
							{
								this.m_ActiveDepthAttachmentDescriptor.ConfigureClear(Color.black, 1f, 0U);
							}
						}
						if (msaaSamples > 1)
						{
							attachmentDescriptor.ConfigureResolveTarget(target);
							if (RenderingUtils.MultisampleDepthResolveSupported())
							{
								this.m_ActiveDepthAttachmentDescriptor.ConfigureResolveTarget(this.m_ActiveDepthAttachmentDescriptor.loadStoreTarget);
							}
						}
						if (ScriptableRenderer.m_UseOptimizedStoreActions)
						{
							attachmentDescriptor.storeAction = this.m_FinalColorStoreAction[0];
							this.m_ActiveDepthAttachmentDescriptor.storeAction = this.m_FinalDepthStoreAction;
						}
						int num3 = ScriptableRenderer.FindAttachmentDescriptorIndexInList(num, attachmentDescriptor, this.m_ActiveColorAttachmentDescriptors);
						if (num3 == -1)
						{
							scriptableRenderPass.m_ColorAttachmentIndices[0] = num;
							this.m_ActiveColorAttachmentDescriptors[num] = attachmentDescriptor;
							num++;
							Dictionary<Hash128, int> renderPassesAttachmentCount = this.m_RenderPassesAttachmentCount;
							Hash128 key = hash;
							int num4 = renderPassesAttachmentCount[key];
							renderPassesAttachmentCount[key] = num4 + 1;
						}
						else
						{
							scriptableRenderPass.m_ColorAttachmentIndices[0] = num3;
						}
					}
				}
			}
		}

		internal unsafe void ExecuteNativeRenderPass(ScriptableRenderContext context, ScriptableRenderPass renderPass, UniversalCameraData cameraData, ref RenderingData renderingData)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.execute))
			{
				int renderPassQueueIndex = renderPass.renderPassQueueIndex;
				Hash128 key = this.m_PassIndexToPassHash[renderPassQueueIndex];
				int[] array = this.m_MergeableRenderPassesMap[key];
				int num = this.m_RenderPassesAttachmentCount[key];
				bool flag = (renderPass.colorAttachmentHandle.rt != null && this.IsDepthOnlyRenderTexture(renderPass.colorAttachmentHandle.rt)) || (cameraData.targetTexture != null && this.IsDepthOnlyRenderTexture(cameraData.targetTexture));
				bool flag2 = flag || !renderPass.overrideCameraTarget || (renderPass.overrideCameraTarget && renderPass.depthAttachmentHandle.nameID != BuiltinRenderTextureType.CameraTarget);
				NativeArray<AttachmentDescriptor> attachments = new NativeArray<AttachmentDescriptor>((flag2 && !flag) ? (num + 1) : 1, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < num; i++)
				{
					attachments[i] = this.m_ActiveColorAttachmentDescriptors[i];
				}
				if (flag2 && !flag)
				{
					attachments[num] = this.m_ActiveDepthAttachmentDescriptor;
				}
				ScriptableRenderer.RenderPassDescriptor renderPassDescriptor = this.InitializeRenderPassDescriptor(cameraData, renderPass);
				int validPassIndexCount = ScriptableRenderer.GetValidPassIndexCount(array);
				uint subPassAttachmentIndicesCount = ScriptableRenderer.GetSubPassAttachmentIndicesCount(renderPass);
				NativeArray<int> colors = new NativeArray<int>((int)((!flag) ? subPassAttachmentIndicesCount : 0U), Allocator.Temp, NativeArrayOptions.ClearMemory);
				if (!flag)
				{
					int num2 = 0;
					while ((long)num2 < (long)((ulong)subPassAttachmentIndicesCount))
					{
						colors[num2] = renderPass.m_ColorAttachmentIndices[num2];
						num2++;
					}
				}
				if (validPassIndexCount == 1 || array[0] == renderPassQueueIndex)
				{
					if (ScriptableRenderer.PassHasInputAttachments(renderPass))
					{
						Debug.LogWarning("First pass in a RenderPass should not have input attachments.");
					}
					context.BeginRenderPass(renderPassDescriptor.w, renderPassDescriptor.h, Math.Max(renderPassDescriptor.samples, 1), attachments, flag2 ? ((!flag) ? num : 0) : -1);
					attachments.Dispose();
					context.BeginSubPass(colors, false);
					this.m_LastBeginSubpassPassIndex = renderPassQueueIndex;
				}
				else if (!ScriptableRenderer.AreAttachmentIndicesCompatible(this.m_ActiveRenderPassQueue[this.m_LastBeginSubpassPassIndex], this.m_ActiveRenderPassQueue[renderPassQueueIndex]))
				{
					context.EndSubPass();
					if (ScriptableRenderer.PassHasInputAttachments(this.m_ActiveRenderPassQueue[renderPassQueueIndex]))
					{
						context.BeginSubPass(colors, this.m_ActiveRenderPassQueue[renderPassQueueIndex].m_InputAttachmentIndices, false);
					}
					else
					{
						context.BeginSubPass(colors, false);
					}
					this.m_LastBeginSubpassPassIndex = renderPassQueueIndex;
				}
				else if (ScriptableRenderer.PassHasInputAttachments(this.m_ActiveRenderPassQueue[renderPassQueueIndex]))
				{
					context.EndSubPass();
					context.BeginSubPass(colors, this.m_ActiveRenderPassQueue[renderPassQueueIndex].m_InputAttachmentIndices, false);
					this.m_LastBeginSubpassPassIndex = renderPassQueueIndex;
				}
				colors.Dispose();
				renderPass.Execute(context, ref renderingData);
				context.ExecuteCommandBuffer(*renderingData.commandBuffer);
				renderingData.commandBuffer->Clear();
				if (validPassIndexCount == 1 || array[validPassIndexCount - 1] == renderPassQueueIndex)
				{
					context.EndSubPass();
					context.EndRenderPass();
					this.m_LastBeginSubpassPassIndex = 0;
				}
				for (int j = 0; j < this.m_ActiveColorAttachmentDescriptors.Length; j++)
				{
					this.m_ActiveColorAttachmentDescriptors[j] = RenderingUtils.emptyAttachment;
					this.m_IsActiveColorAttachmentTransient[j] = false;
				}
				this.m_ActiveDepthAttachmentDescriptor = RenderingUtils.emptyAttachment;
			}
		}

		internal void SetupInputAttachmentIndices(ScriptableRenderPass pass)
		{
			int validInputAttachmentCount = ScriptableRenderer.GetValidInputAttachmentCount(pass);
			pass.m_InputAttachmentIndices = new NativeArray<int>(validInputAttachmentCount, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < validInputAttachmentCount; i++)
			{
				pass.m_InputAttachmentIndices[i] = ScriptableRenderer.FindAttachmentDescriptorIndexInList(pass.m_InputAttachments[i], this.m_ActiveColorAttachmentDescriptors);
				if (pass.m_InputAttachmentIndices[i] == -1)
				{
					Debug.LogWarning("RenderPass Input attachment not found in the current RenderPass");
				}
				else if (!this.m_IsActiveColorAttachmentTransient[pass.m_InputAttachmentIndices[i]])
				{
					this.m_IsActiveColorAttachmentTransient[pass.m_InputAttachmentIndices[i]] = pass.IsInputAttachmentTransient(i);
				}
			}
		}

		internal void SetupTransientInputAttachments(int attachmentCount)
		{
			for (int i = 0; i < attachmentCount; i++)
			{
				if (this.m_IsActiveColorAttachmentTransient[i])
				{
					this.m_ActiveColorAttachmentDescriptors[i].loadAction = RenderBufferLoadAction.DontCare;
					this.m_ActiveColorAttachmentDescriptors[i].storeAction = RenderBufferStoreAction.DontCare;
					this.m_ActiveColorAttachmentDescriptors[i].loadStoreTarget = BuiltinRenderTextureType.None;
				}
			}
		}

		internal static uint GetSubPassAttachmentIndicesCount(ScriptableRenderPass pass)
		{
			uint num = 0U;
			using (NativeArray<int>.Enumerator enumerator = pass.m_ColorAttachmentIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current >= 0)
					{
						num += 1U;
					}
				}
			}
			return num;
		}

		internal static bool AreAttachmentIndicesCompatible(ScriptableRenderPass lastSubPass, ScriptableRenderPass currentSubPass)
		{
			uint subPassAttachmentIndicesCount = ScriptableRenderer.GetSubPassAttachmentIndicesCount(lastSubPass);
			uint subPassAttachmentIndicesCount2 = ScriptableRenderer.GetSubPassAttachmentIndicesCount(currentSubPass);
			if (subPassAttachmentIndicesCount2 != subPassAttachmentIndicesCount)
			{
				return false;
			}
			uint num = 0U;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)subPassAttachmentIndicesCount2))
			{
				int num3 = 0;
				while ((long)num3 < (long)((ulong)subPassAttachmentIndicesCount))
				{
					if (currentSubPass.m_ColorAttachmentIndices[num2] == lastSubPass.m_ColorAttachmentIndices[num3])
					{
						num += 1U;
					}
					num3++;
				}
				num2++;
			}
			return num == subPassAttachmentIndicesCount2;
		}

		internal static uint GetValidColorAttachmentCount(AttachmentDescriptor[] colorAttachments)
		{
			uint num = 0U;
			if (colorAttachments != null)
			{
				for (int i = 0; i < colorAttachments.Length; i++)
				{
					if (colorAttachments[i] != RenderingUtils.emptyAttachment)
					{
						num += 1U;
					}
				}
			}
			return num;
		}

		internal static int GetValidInputAttachmentCount(ScriptableRenderPass renderPass)
		{
			int num = renderPass.m_InputAttachments.Length;
			if (num != 8)
			{
				return num;
			}
			for (int i = 0; i < num; i++)
			{
				if (renderPass.m_InputAttachments[i] == null)
				{
					return i;
				}
			}
			return num;
		}

		internal static int FindAttachmentDescriptorIndexInList(int attachmentIdx, AttachmentDescriptor attachmentDescriptor, AttachmentDescriptor[] attachmentDescriptors)
		{
			int result = -1;
			for (int i = 0; i <= attachmentIdx; i++)
			{
				AttachmentDescriptor attachmentDescriptor2 = attachmentDescriptors[i];
				if (attachmentDescriptor2.loadStoreTarget == attachmentDescriptor.loadStoreTarget && attachmentDescriptor2.graphicsFormat == attachmentDescriptor.graphicsFormat)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		internal static int FindAttachmentDescriptorIndexInList(RenderTargetIdentifier target, AttachmentDescriptor[] attachmentDescriptors)
		{
			for (int i = 0; i < attachmentDescriptors.Length; i++)
			{
				AttachmentDescriptor attachmentDescriptor = attachmentDescriptors[i];
				if (attachmentDescriptor.loadStoreTarget == target)
				{
					return i;
				}
			}
			return -1;
		}

		internal static int GetValidPassIndexCount(int[] array)
		{
			if (array == null)
			{
				return 0;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == -1)
				{
					return i;
				}
			}
			return array.Length - 1;
		}

		internal static RTHandle GetFirstAllocatedRTHandle(ScriptableRenderPass pass)
		{
			for (int i = 0; i < pass.colorAttachmentHandles.Length; i++)
			{
				if (pass.colorAttachmentHandles[i].rt != null)
				{
					return pass.colorAttachmentHandles[i];
				}
			}
			return pass.colorAttachmentHandles[0];
		}

		internal static bool PassHasInputAttachments(ScriptableRenderPass renderPass)
		{
			return renderPass.m_InputAttachments.Length != 8 || renderPass.m_InputAttachments[0] != null;
		}

		internal static Hash128 CreateRenderPassHash(int width, int height, int depthID, int sample, uint hashIndex)
		{
			return new Hash128((uint)((width << 4) + height), (uint)depthID, (uint)sample, hashIndex);
		}

		internal static Hash128 CreateRenderPassHash(ScriptableRenderer.RenderPassDescriptor desc, uint hashIndex)
		{
			return ScriptableRenderer.CreateRenderPassHash(desc.w, desc.h, desc.depthID, desc.samples, hashIndex);
		}

		internal static void GetRenderTextureDescriptor(UniversalCameraData cameraData, ScriptableRenderPass renderPass, out RenderTextureDescriptor targetRT)
		{
			if (!renderPass.overrideCameraTarget || (renderPass.colorAttachmentHandle.rt == null && renderPass.depthAttachmentHandle.rt == null))
			{
				targetRT = cameraData.cameraTargetDescriptor;
				if (cameraData.targetTexture != null)
				{
					targetRT.width = cameraData.scaledWidth;
					targetRT.height = cameraData.scaledHeight;
					return;
				}
			}
			else
			{
				RTHandle firstAllocatedRTHandle = ScriptableRenderer.GetFirstAllocatedRTHandle(renderPass);
				targetRT = ((firstAllocatedRTHandle.rt != null) ? firstAllocatedRTHandle.rt.descriptor : renderPass.depthAttachmentHandle.rt.descriptor);
			}
		}

		private ScriptableRenderer.RenderPassDescriptor InitializeRenderPassDescriptor(UniversalCameraData cameraData, ScriptableRenderPass renderPass)
		{
			RenderTextureDescriptor renderTextureDescriptor;
			ScriptableRenderer.GetRenderTextureDescriptor(cameraData, renderPass, out renderTextureDescriptor);
			RTHandle rthandle = renderPass.overrideCameraTarget ? renderPass.depthAttachmentHandle : this.cameraDepthTargetHandle;
			int rtID = (renderTextureDescriptor.graphicsFormat == GraphicsFormat.None && renderTextureDescriptor.depthStencilFormat != GraphicsFormat.None) ? renderPass.colorAttachmentHandle.GetHashCode() : rthandle.GetHashCode();
			return new ScriptableRenderer.RenderPassDescriptor(renderTextureDescriptor.width, renderTextureDescriptor.height, renderTextureDescriptor.msaaSamples, rtID);
		}

		public virtual int SupportedCameraStackingTypes()
		{
			return 0;
		}

		public bool SupportsCameraStackingType(CameraRenderType cameraRenderType)
		{
			return (this.SupportedCameraStackingTypes() & 1 << (int)cameraRenderType) != 0;
		}

		protected internal virtual bool SupportsMotionVectors()
		{
			return false;
		}

		protected internal virtual bool SupportsCameraOpaque()
		{
			return false;
		}

		protected internal virtual bool SupportsCameraNormals()
		{
			return false;
		}

		protected ProfilingSampler profilingExecute { get; set; }

		internal DebugHandler DebugHandler { get; }

		public static void SetCameraMatrices(CommandBuffer cmd, ref CameraData cameraData, bool setInverseMatrices)
		{
			ScriptableRenderer.SetCameraMatrices(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData.universalCameraData, setInverseMatrices, cameraData.IsCameraProjectionMatrixFlipped());
		}

		public static void SetCameraMatrices(CommandBuffer cmd, UniversalCameraData cameraData, bool setInverseMatrices)
		{
			ScriptableRenderer.SetCameraMatrices(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData, setInverseMatrices, cameraData.IsCameraProjectionMatrixFlipped());
		}

		internal static void SetCameraMatrices(RasterCommandBuffer cmd, UniversalCameraData cameraData, bool setInverseMatrices, bool isTargetFlipped)
		{
			if (cameraData.xr.enabled)
			{
				cameraData.PushBuiltinShaderConstantsXR(cmd, isTargetFlipped);
				XRSystemUniversal.MarkShaderProperties(cmd, cameraData.xrUniversal, isTargetFlipped);
				return;
			}
			Matrix4x4 viewMatrix = cameraData.GetViewMatrix(0);
			Matrix4x4 projectionMatrix = cameraData.GetProjectionMatrix(0);
			cmd.SetViewProjectionMatrices(viewMatrix, projectionMatrix);
			if (setInverseMatrices)
			{
				Matrix4x4 gpuprojectionMatrix = cameraData.GetGPUProjectionMatrix(isTargetFlipped, 0);
				Matrix4x4 matrix4x = Matrix4x4.Inverse(viewMatrix);
				Matrix4x4 matrix4x2 = Matrix4x4.Inverse(gpuprojectionMatrix);
				Matrix4x4 value = matrix4x * matrix4x2;
				Matrix4x4 value2 = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * viewMatrix;
				Matrix4x4 inverse = value2.inverse;
				cmd.SetGlobalMatrix(ShaderPropertyId.worldToCameraMatrix, value2);
				cmd.SetGlobalMatrix(ShaderPropertyId.cameraToWorldMatrix, inverse);
				cmd.SetGlobalMatrix(ShaderPropertyId.inverseViewMatrix, matrix4x);
				cmd.SetGlobalMatrix(ShaderPropertyId.inverseProjectionMatrix, matrix4x2);
				cmd.SetGlobalMatrix(ShaderPropertyId.inverseViewAndProjectionMatrix, value);
			}
		}

		private void SetPerCameraShaderVariables(RasterCommandBuffer cmd, UniversalCameraData cameraData)
		{
			this.SetPerCameraShaderVariables(cmd, cameraData, new Vector2Int(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height), cameraData.IsCameraProjectionMatrixFlipped());
		}

		private void SetPerCameraShaderVariables(RasterCommandBuffer cmd, UniversalCameraData cameraData, Vector2Int cameraTargetSizeCopy, bool isTargetFlipped)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.setPerCameraShaderVariables))
			{
				Camera camera = cameraData.camera;
				float num = (float)cameraTargetSizeCopy.x;
				float num2 = (float)cameraTargetSizeCopy.y;
				float num3 = (float)camera.pixelWidth;
				float num4 = (float)camera.pixelHeight;
				if (cameraData.renderType == CameraRenderType.Overlay)
				{
					num3 = (float)cameraData.pixelWidth;
					num4 = (float)cameraData.pixelHeight;
				}
				if (cameraData.xr.enabled)
				{
					num3 = (float)cameraTargetSizeCopy.x;
					num4 = (float)cameraTargetSizeCopy.y;
					this.useRenderPassEnabled = false;
				}
				if (camera.allowDynamicResolution)
				{
					if (cameraData.xr.enabled)
					{
						num = (float)cameraData.xr.renderTargetScaledWidth;
						num2 = (float)cameraData.xr.renderTargetScaledHeight;
					}
					else
					{
						num *= ScalableBufferManager.widthScaleFactor;
						num2 *= ScalableBufferManager.heightScaleFactor;
					}
				}
				float nearClipPlane = camera.nearClipPlane;
				float farClipPlane = camera.farClipPlane;
				float num5 = Mathf.Approximately(nearClipPlane, 0f) ? 0f : (1f / nearClipPlane);
				float num6 = Mathf.Approximately(farClipPlane, 0f) ? 0f : (1f / farClipPlane);
				float w = camera.orthographic ? 1f : 0f;
				float num7 = 1f - farClipPlane * num5;
				float num8 = farClipPlane * num5;
				Vector4 vector = new Vector4(num7, num8, num7 * num6, num8 * num6);
				if (SystemInfo.usesReversedZBuffer)
				{
					vector.y += vector.x;
					vector.x = -vector.x;
					vector.w += vector.z;
					vector.z = -vector.z;
				}
				if (cameraData.renderType == CameraRenderType.Overlay)
				{
					float x = isTargetFlipped ? -1f : 1f;
					Vector4 value = new Vector4(x, nearClipPlane, farClipPlane, 1f * num6);
					cmd.SetGlobalVector(ShaderPropertyId.projectionParams, value);
				}
				Vector4 value2 = new Vector4(camera.orthographicSize * cameraData.aspectRatio, camera.orthographicSize, 0f, w);
				cmd.SetGlobalVector(ShaderPropertyId.worldSpaceCameraPos, cameraData.worldSpaceCameraPos);
				cmd.SetGlobalVector(ShaderPropertyId.screenParams, new Vector4(num3, num4, 1f + 1f / num3, 1f + 1f / num4));
				cmd.SetGlobalVector(ShaderPropertyId.scaledScreenParams, new Vector4(num, num2, 1f + 1f / num, 1f + 1f / num2));
				cmd.SetGlobalVector(ShaderPropertyId.zBufferParams, vector);
				cmd.SetGlobalVector(ShaderPropertyId.orthoParams, value2);
				cmd.SetGlobalVector(ShaderPropertyId.screenSize, new Vector4(num, num2, 1f / num, 1f / num2));
				cmd.SetKeyword(ShaderGlobalKeywords.SCREEN_COORD_OVERRIDE, cameraData.useScreenCoordOverride);
				cmd.SetGlobalVector(ShaderPropertyId.screenSizeOverride, cameraData.screenSizeOverride);
				cmd.SetGlobalVector(ShaderPropertyId.screenCoordScaleBias, cameraData.screenCoordScaleBias);
				cmd.SetGlobalVector(ShaderPropertyId.rtHandleScale, Vector4.one);
				float num9 = Math.Min((float)(-(float)Math.Log((double)(num3 / num), 2.0)), 0f);
				float val = Math.Min(cameraData.taaSettings.mipBias, 0f);
				num9 = Math.Min(num9, val);
				cmd.SetGlobalVector(ShaderPropertyId.globalMipBias, new Vector2(num9, Mathf.Pow(2f, num9)));
				ScriptableRenderer.SetCameraMatrices(cmd, cameraData, true, isTargetFlipped);
			}
		}

		private void SetPerCameraBillboardProperties(RasterCommandBuffer cmd, UniversalCameraData cameraData)
		{
			Matrix4x4 viewMatrix = cameraData.GetViewMatrix(0);
			Vector3 worldSpaceCameraPos = cameraData.worldSpaceCameraPos;
			cmd.SetKeyword(ShaderGlobalKeywords.BillboardFaceCameraPos, QualitySettings.billboardsFaceCameraPosition);
			Vector3 vector;
			Vector3 vector2;
			float w;
			ScriptableRenderer.CalculateBillboardProperties(viewMatrix, out vector, out vector2, out w);
			cmd.SetGlobalVector(ShaderPropertyId.billboardNormal, new Vector4(vector2.x, vector2.y, vector2.z, 0f));
			cmd.SetGlobalVector(ShaderPropertyId.billboardTangent, new Vector4(vector.x, vector.y, vector.z, 0f));
			cmd.SetGlobalVector(ShaderPropertyId.billboardCameraParams, new Vector4(worldSpaceCameraPos.x, worldSpaceCameraPos.y, worldSpaceCameraPos.z, w));
		}

		private static void CalculateBillboardProperties(in Matrix4x4 worldToCameraMatrix, out Vector3 billboardTangent, out Vector3 billboardNormal, out float cameraXZAngle)
		{
			Matrix4x4 matrix4x = worldToCameraMatrix;
			matrix4x = matrix4x.transpose;
			Vector3 vector = new Vector3(matrix4x.m00, matrix4x.m10, matrix4x.m20);
			Vector3 vector2 = new Vector3(matrix4x.m01, matrix4x.m11, matrix4x.m21);
			Vector3 lhs = new Vector3(matrix4x.m02, matrix4x.m12, matrix4x.m22);
			Vector3 up = Vector3.up;
			Vector3 vector3 = Vector3.Cross(lhs, up);
			billboardTangent = ((!Mathf.Approximately(vector3.sqrMagnitude, 0f)) ? vector3.normalized : vector);
			billboardNormal = Vector3.Cross(up, billboardTangent);
			billboardNormal = ((!Mathf.Approximately(billboardNormal.sqrMagnitude, 0f)) ? billboardNormal.normalized : vector2);
			Vector3 vector4 = new Vector3(0f, 0f, 1f);
			float y = vector4.x * billboardTangent.z - vector4.z * billboardTangent.x;
			float x = vector4.x * billboardTangent.x + vector4.z * billboardTangent.z;
			cameraXZAngle = Mathf.Atan2(y, x);
			if (cameraXZAngle < 0f)
			{
				cameraXZAngle += 6.2831855f;
			}
		}

		private void SetPerCameraClippingPlaneProperties(RasterCommandBuffer cmd, UniversalCameraData cameraData)
		{
			this.SetPerCameraClippingPlaneProperties(cmd, cameraData, cameraData.IsCameraProjectionMatrixFlipped());
		}

		private void SetPerCameraClippingPlaneProperties(RasterCommandBuffer cmd, in UniversalCameraData cameraData, bool isTargetFlipped)
		{
			Matrix4x4 gpuprojectionMatrix = cameraData.GetGPUProjectionMatrix(isTargetFlipped, 0);
			Matrix4x4 viewMatrix = cameraData.GetViewMatrix(0);
			Matrix4x4 worldToProjectionMatrix = CoreMatrixUtils.MultiplyProjectionMatrix(gpuprojectionMatrix, viewMatrix, cameraData.camera.orthographic);
			Plane[] array = ScriptableRenderer.s_Planes;
			GeometryUtility.CalculateFrustumPlanes(worldToProjectionMatrix, array);
			Vector4[] array2 = ScriptableRenderer.s_VectorPlanes;
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new Vector4(array[i].normal.x, array[i].normal.y, array[i].normal.z, array[i].distance);
			}
			cmd.SetGlobalVectorArray(ShaderPropertyId.cameraWorldClipPlanes, array2);
		}

		private static void SetShaderTimeValues(IBaseCommandBuffer cmd, float time, float deltaTime, float smoothDeltaTime)
		{
			float f = time / 8f;
			float f2 = time / 4f;
			float f3 = time / 2f;
			float num = time - ShaderUtils.PersistentDeltaTime;
			Vector4 value = time * new Vector4(0.05f, 1f, 2f, 3f);
			Vector4 value2 = new Vector4(Mathf.Sin(f), Mathf.Sin(f2), Mathf.Sin(f3), Mathf.Sin(time));
			Vector4 value3 = new Vector4(Mathf.Cos(f), Mathf.Cos(f2), Mathf.Cos(f3), Mathf.Cos(time));
			Vector4 value4 = new Vector4(deltaTime, 1f / deltaTime, smoothDeltaTime, 1f / smoothDeltaTime);
			Vector4 value5 = new Vector4(time, Mathf.Sin(time), Mathf.Cos(time), 0f);
			Vector4 value6 = new Vector4(num, Mathf.Sin(num), Mathf.Cos(num), 0f);
			cmd.SetGlobalVector(ShaderPropertyId.time, value);
			cmd.SetGlobalVector(ShaderPropertyId.sinTime, value2);
			cmd.SetGlobalVector(ShaderPropertyId.cosTime, value3);
			cmd.SetGlobalVector(ShaderPropertyId.deltaTime, value4);
			cmd.SetGlobalVector(ShaderPropertyId.timeParameters, value5);
			cmd.SetGlobalVector(ShaderPropertyId.lastTimeParameters, value6);
		}

		[Obsolete("Use cameraColorTargetHandle", true)]
		public RenderTargetIdentifier cameraColorTarget
		{
			get
			{
				throw new NotSupportedException("cameraColorTarget has been deprecated. Use cameraColorTargetHandle instead");
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public RTHandle cameraColorTargetHandle
		{
			get
			{
				if (!this.m_IsPipelineExecuting)
				{
					Debug.LogError("You can only call cameraColorTargetHandle inside the scope of a ScriptableRenderPass. Otherwise the pipeline camera target texture might have not been created or might have already been disposed.");
					return null;
				}
				return this.m_CameraColorTarget;
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal virtual RTHandle GetCameraColorFrontBuffer(CommandBuffer cmd)
		{
			return null;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal virtual RTHandle GetCameraColorBackBuffer(CommandBuffer cmd)
		{
			return null;
		}

		[Obsolete("Use cameraDepthTargetHandle", true)]
		public RenderTargetIdentifier cameraDepthTarget
		{
			get
			{
				throw new NotSupportedException("cameraDepthTarget has been deprecated. Use cameraDepthTargetHandle instead");
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public RTHandle cameraDepthTargetHandle
		{
			get
			{
				if (!this.m_IsPipelineExecuting)
				{
					Debug.LogError("You can only call cameraDepthTargetHandle inside the scope of a ScriptableRenderPass. Otherwise the pipeline camera target texture might have not been created or might have already been disposed.");
					return null;
				}
				return this.m_CameraDepthTarget;
			}
		}

		protected List<ScriptableRendererFeature> rendererFeatures
		{
			get
			{
				return this.m_RendererFeatures;
			}
		}

		protected List<ScriptableRenderPass> activeRenderPassQueue
		{
			get
			{
				return this.m_ActiveRenderPassQueue;
			}
		}

		public ScriptableRenderer.RenderingFeatures supportedRenderingFeatures { get; set; } = new ScriptableRenderer.RenderingFeatures();

		public GraphicsDeviceType[] unsupportedGraphicsDeviceTypes { get; set; } = new GraphicsDeviceType[0];

		internal ContextContainer frameData
		{
			get
			{
				return this.m_frameData;
			}
		}

		internal bool useDepthPriming { get; set; }

		internal bool stripShadowsOffVariants { get; set; }

		internal bool stripAdditionalLightOffVariants { get; set; }

		public ScriptableRenderer(ScriptableRendererData data)
		{
			this.profilingExecute = new ProfilingSampler("ScriptableRenderer.Execute: " + data.name);
			foreach (ScriptableRendererFeature scriptableRendererFeature in data.rendererFeatures)
			{
				if (!(scriptableRendererFeature == null))
				{
					scriptableRendererFeature.Create();
					this.m_RendererFeatures.Add(scriptableRendererFeature);
				}
			}
			this.ResetNativeRenderPassFrameData();
			this.useRenderPassEnabled = data.useNativeRenderPass;
			this.Clear(CameraRenderType.Base);
			this.m_ActiveRenderPassQueue.Clear();
			if (UniversalRenderPipeline.asset)
			{
				this.m_StoreActionsOptimizationSetting = UniversalRenderPipeline.asset.storeActionsOptimization;
			}
			ScriptableRenderer.m_UseOptimizedStoreActions = (this.m_StoreActionsOptimizationSetting != StoreActionsOptimization.Store);
		}

		public void Dispose()
		{
			for (int i = 0; i < this.m_RendererFeatures.Count; i++)
			{
				if (!(this.rendererFeatures[i] == null))
				{
					try
					{
						this.rendererFeatures[i].Dispose();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
			}
			this.Dispose(true);
			this.hasReleasedRTs = true;
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			DebugHandler debugHandler = this.DebugHandler;
			if (debugHandler == null)
			{
				return;
			}
			debugHandler.Dispose();
		}

		internal virtual void ReleaseRenderTargets()
		{
		}

		[Obsolete("Use RTHandles for colorTarget and depthTarget", true)]
		public void ConfigureCameraTarget(RenderTargetIdentifier colorTarget, RenderTargetIdentifier depthTarget)
		{
			throw new NotSupportedException("ConfigureCameraTarget with RenderTargetIdentifier has been deprecated. Use it with RTHandles instead");
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public void ConfigureCameraTarget(RTHandle colorTarget, RTHandle depthTarget)
		{
			this.m_CameraColorTarget = colorTarget;
			this.m_CameraDepthTarget = depthTarget;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void ConfigureCameraTarget(RTHandle colorTarget, RTHandle depthTarget, RTHandle resolveTarget)
		{
			this.m_CameraColorTarget = colorTarget;
			this.m_CameraDepthTarget = depthTarget;
			this.m_CameraResolveTarget = resolveTarget;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal void ConfigureCameraColorTarget(RTHandle colorTarget)
		{
			this.m_CameraColorTarget = colorTarget;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public abstract void Setup(ScriptableRenderContext context, ref RenderingData renderingData);

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public virtual void SetupLights(ScriptableRenderContext context, ref RenderingData renderingData)
		{
		}

		public virtual void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
		{
		}

		public virtual void FinishRendering(CommandBuffer cmd)
		{
		}

		public virtual void OnBeginRenderGraphFrame()
		{
		}

		internal virtual void OnRecordRenderGraph(RenderGraph renderGraph, ScriptableRenderContext context)
		{
		}

		public virtual void OnEndRenderGraphFrame()
		{
		}

		private void InitRenderGraphFrame(RenderGraph renderGraph)
		{
			ScriptableRenderer.PassData passData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<ScriptableRenderer.PassData>(ScriptableRenderer.Profiling.initRenderGraphFrame.name, out passData, ScriptableRenderer.Profiling.initRenderGraphFrame, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ScriptableRenderer.cs", 914))
			{
				passData.renderer = this;
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.SetRenderFunc<ScriptableRenderer.PassData>(delegate(ScriptableRenderer.PassData data, UnsafeGraphContext rgContext)
				{
					UnsafeCommandBuffer cmd = rgContext.cmd;
					float time = Time.time;
					float deltaTime = Time.deltaTime;
					float smoothDeltaTime = Time.smoothDeltaTime;
					ScriptableRenderer.ClearRenderingState(cmd);
					ScriptableRenderer.SetShaderTimeValues(cmd, time, deltaTime, smoothDeltaTime);
				});
			}
		}

		internal void ProcessVFXCameraCommand(RenderGraph renderGraph)
		{
			UniversalRenderingData renderingData = this.frameData.Get<UniversalRenderingData>();
			UniversalCameraData universalCameraData = this.frameData.Get<UniversalCameraData>();
			XRPass xr = universalCameraData.xr;
			ScriptableRenderer.VFXProcessCameraPassData vfxprocessCameraPassData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<ScriptableRenderer.VFXProcessCameraPassData>("ProcessVFXCameraCommand", out vfxprocessCameraPassData, ScriptableRenderer.Profiling.vfxProcessCamera, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ScriptableRenderer.cs", 952))
			{
				vfxprocessCameraPassData.camera = universalCameraData.camera;
				vfxprocessCameraPassData.renderingData = renderingData;
				vfxprocessCameraPassData.cameraXRSettings.viewTotal = (xr.enabled ? 2U : 1U);
				vfxprocessCameraPassData.cameraXRSettings.viewCount = (uint)(xr.enabled ? xr.viewCount : 1);
				vfxprocessCameraPassData.cameraXRSettings.viewOffset = (uint)xr.multipassId;
				vfxprocessCameraPassData.xrPass = (xr.enabled ? xr : null);
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.SetRenderFunc<ScriptableRenderer.VFXProcessCameraPassData>(delegate(ScriptableRenderer.VFXProcessCameraPassData data, UnsafeGraphContext context)
				{
					if (data.xrPass != null)
					{
						data.xrPass.StartSinglePass(context.cmd);
					}
					CommandBufferHelpers.VFXManager_ProcessCameraCommand(data.camera, context.cmd, data.cameraXRSettings, data.renderingData.cullResults);
					if (data.xrPass != null)
					{
						data.xrPass.StopSinglePass(context.cmd);
					}
				});
			}
		}

		internal void SetupRenderGraphCameraProperties(RenderGraph renderGraph, bool isTargetBackbuffer)
		{
			ScriptableRenderer.PassData passData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<ScriptableRenderer.PassData>(ScriptableRenderer.Profiling.setupCamera.name, out passData, ScriptableRenderer.Profiling.setupCamera, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ScriptableRenderer.cs", 981))
			{
				passData.renderer = this;
				passData.cameraData = this.frameData.Get<UniversalCameraData>();
				passData.cameraTargetSizeCopy = new Vector2Int(passData.cameraData.cameraTargetDescriptor.width, passData.cameraData.cameraTargetDescriptor.height);
				passData.isTargetBackbuffer = isTargetBackbuffer;
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<ScriptableRenderer.PassData>(delegate(ScriptableRenderer.PassData data, RasterGraphContext context)
				{
					bool flag = !SystemInfo.graphicsUVStartsAtTop || data.isTargetBackbuffer;
					if (data.cameraData.renderType == CameraRenderType.Base)
					{
						context.cmd.SetupCameraProperties(data.cameraData.camera);
						data.renderer.SetPerCameraShaderVariables(context.cmd, data.cameraData, data.cameraTargetSizeCopy, !flag);
					}
					else
					{
						data.renderer.SetPerCameraShaderVariables(context.cmd, data.cameraData, data.cameraTargetSizeCopy, !flag);
						data.renderer.SetPerCameraClippingPlaneProperties(context.cmd, data.cameraData, !flag);
						data.renderer.SetPerCameraBillboardProperties(context.cmd, data.cameraData);
					}
					float time = Time.time;
					float deltaTime = Time.deltaTime;
					float smoothDeltaTime = Time.smoothDeltaTime;
					ScriptableRenderer.SetShaderTimeValues(context.cmd, time, deltaTime, smoothDeltaTime);
				});
			}
		}

		internal void DrawRenderGraphGizmos(RenderGraph renderGraph, ContextContainer frameData, TextureHandle color, TextureHandle depth, GizmoSubset gizmoSubset)
		{
		}

		internal void DrawRenderGraphWireOverlay(RenderGraph renderGraph, ContextContainer frameData, TextureHandle color)
		{
		}

		internal void BeginRenderGraphXRRendering(RenderGraph renderGraph)
		{
			UniversalCameraData universalCameraData = this.frameData.Get<UniversalCameraData>();
			if (!universalCameraData.xr.enabled)
			{
				return;
			}
			bool flag = XRSystem.GetRenderViewportScale() == 1f;
			universalCameraData.xrUniversal.canFoveateIntermediatePasses = (!PlatformAutoDetect.isXRMobile || flag);
			ScriptableRenderer.BeginXRPassData beginXRPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<ScriptableRenderer.BeginXRPassData>("BeginXRRendering", out beginXRPassData, ScriptableRenderer.Profiling.beginXRRendering, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ScriptableRenderer.cs", 1129))
			{
				beginXRPassData.cameraData = universalCameraData;
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<ScriptableRenderer.BeginXRPassData>(delegate(ScriptableRenderer.BeginXRPassData data, RasterGraphContext context)
				{
					if (data.cameraData.xr.enabled)
					{
						if (data.cameraData.xrUniversal.isLateLatchEnabled)
						{
							data.cameraData.xrUniversal.canMarkLateLatch = true;
						}
						data.cameraData.xr.StartSinglePass(context.cmd);
						if (data.cameraData.xr.supportsFoveatedRendering)
						{
							context.cmd.ConfigureFoveatedRendering(data.cameraData.xr.foveatedRenderingInfo);
							if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster))
							{
								context.cmd.SetKeyword(ShaderGlobalKeywords.FoveatedRenderingNonUniformRaster, true);
							}
						}
					}
				});
			}
		}

		internal void EndRenderGraphXRRendering(RenderGraph renderGraph)
		{
			UniversalCameraData universalCameraData = this.frameData.Get<UniversalCameraData>();
			if (!universalCameraData.xr.enabled)
			{
				return;
			}
			ScriptableRenderer.EndXRPassData endXRPassData;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<ScriptableRenderer.EndXRPassData>("EndXRRendering", out endXRPassData, ScriptableRenderer.Profiling.endXRRendering, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ScriptableRenderer.cs", 1169))
			{
				endXRPassData.cameraData = universalCameraData;
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				rasterRenderGraphBuilder.SetRenderFunc<ScriptableRenderer.EndXRPassData>(delegate(ScriptableRenderer.EndXRPassData data, RasterGraphContext context)
				{
					if (data.cameraData.xr.enabled)
					{
						data.cameraData.xr.StopSinglePass(context.cmd);
					}
					if (XRSystem.foveatedRenderingCaps != FoveatedRenderingCaps.None)
					{
						if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster))
						{
							context.cmd.SetKeyword(ShaderGlobalKeywords.FoveatedRenderingNonUniformRaster, false);
						}
						context.cmd.ConfigureFoveatedRendering(IntPtr.Zero);
					}
				});
			}
		}

		private void SetEditorTarget(RenderGraph renderGraph)
		{
			ScriptableRenderer.DummyData dummyData;
			using (IUnsafeRenderGraphBuilder unsafeRenderGraphBuilder = renderGraph.AddUnsafePass<ScriptableRenderer.DummyData>("SetEditorTarget", out dummyData, ScriptableRenderer.Profiling.setEditorTarget, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\ScriptableRenderer.cs", 1201))
			{
				unsafeRenderGraphBuilder.AllowPassCulling(false);
				unsafeRenderGraphBuilder.SetRenderFunc<ScriptableRenderer.DummyData>(delegate(ScriptableRenderer.DummyData data, UnsafeGraphContext context)
				{
					context.cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.DontCare);
				});
			}
		}

		internal void RecordRenderGraph(RenderGraph renderGraph, ScriptableRenderContext context)
		{
			using (new ProfilingScope(ProfilingSampler.Get<URPProfileId>(URPProfileId.RecordRenderGraph)))
			{
				this.OnBeginRenderGraphFrame();
				using (new ProfilingScope(ScriptableRenderer.Profiling.sortRenderPasses))
				{
					ScriptableRenderer.SortStable(this.m_ActiveRenderPassQueue);
				}
				this.InitRenderGraphFrame(renderGraph);
				using (new ProfilingScope(ScriptableRenderer.Profiling.recordRenderGraph))
				{
					this.OnRecordRenderGraph(renderGraph, context);
				}
				this.OnEndRenderGraphFrame();
			}
		}

		internal void FinishRenderGraphRendering(CommandBuffer cmd)
		{
			UniversalCameraData universalCameraData = this.frameData.Get<UniversalCameraData>();
			this.OnFinishRenderGraphRendering(cmd);
			this.InternalFinishRenderingCommon(cmd, universalCameraData.resolveFinalTarget);
		}

		internal virtual void OnFinishRenderGraphRendering(CommandBuffer cmd)
		{
		}

		internal void RecordCustomRenderGraphPassesInEventRange(RenderGraph renderGraph, RenderPassEvent eventStart, RenderPassEvent eventEnd)
		{
			if (eventStart != eventEnd)
			{
				foreach (ScriptableRenderPass scriptableRenderPass in this.m_ActiveRenderPassQueue)
				{
					if (scriptableRenderPass.renderPassEvent >= eventStart && scriptableRenderPass.renderPassEvent < eventEnd)
					{
						scriptableRenderPass.RecordRenderGraph(renderGraph, this.m_frameData);
					}
				}
			}
		}

		internal void CalculateSplitEventRange(RenderPassEvent startInjectionPoint, RenderPassEvent targetEvent, out RenderPassEvent startEvent, out RenderPassEvent splitEvent, out RenderPassEvent endEvent)
		{
			int renderPassEventRange = ScriptableRenderPass.GetRenderPassEventRange(startInjectionPoint);
			startEvent = startInjectionPoint;
			endEvent = startEvent + renderPassEventRange;
			splitEvent = (RenderPassEvent)Math.Clamp((int)targetEvent, (int)startEvent, (int)endEvent);
		}

		internal void RecordCustomRenderGraphPasses(RenderGraph renderGraph, RenderPassEvent startInjectionPoint, RenderPassEvent endInjectionPoint)
		{
			int renderPassEventRange = ScriptableRenderPass.GetRenderPassEventRange(endInjectionPoint);
			this.RecordCustomRenderGraphPassesInEventRange(renderGraph, startInjectionPoint, endInjectionPoint + renderPassEventRange);
		}

		internal void RecordCustomRenderGraphPasses(RenderGraph renderGraph, RenderPassEvent injectionPoint)
		{
			this.RecordCustomRenderGraphPasses(renderGraph, injectionPoint, injectionPoint);
		}

		internal void SetPerCameraProperties(ScriptableRenderContext context, UniversalCameraData cameraData, Camera camera, CommandBuffer cmd)
		{
			if (cameraData.renderType == CameraRenderType.Base)
			{
				context.SetupCameraProperties(camera, false);
				this.SetPerCameraShaderVariables(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData);
				return;
			}
			this.SetPerCameraShaderVariables(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData);
			this.SetPerCameraClippingPlaneProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData);
			this.SetPerCameraBillboardProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			bool flag = DebugDisplaySettings<UniversalRenderPipelineDebugDisplaySettings>.Instance.renderingSettings.sceneOverrideMode == DebugSceneOverrideMode.None;
			this.hasReleasedRTs = false;
			this.m_IsPipelineExecuting = true;
			UniversalCameraData universalCameraData = renderingData.frameData.Get<UniversalCameraData>();
			Camera camera = universalCameraData.camera;
			if (this.rendererFeatures.Count != 0 && !renderingData.cameraData.isPreviewCamera)
			{
				this.SetupRenderPasses(renderingData);
			}
			CommandBuffer commandBuffer = *renderingData.commandBuffer;
			CommandBuffer cmd = renderingData.cameraData.xr.enabled ? null : commandBuffer;
			using (new ProfilingScope(cmd, this.profilingExecute))
			{
				this.InternalStartRendering(context, ref renderingData);
				float time = Time.time;
				float deltaTime = Time.deltaTime;
				float smoothDeltaTime = Time.smoothDeltaTime;
				ScriptableRenderer.ClearRenderingState(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer));
				ScriptableRenderer.SetShaderTimeValues(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), time, deltaTime, smoothDeltaTime);
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				using (new ProfilingScope(ScriptableRenderer.Profiling.sortRenderPasses))
				{
					ScriptableRenderer.SortStable(this.m_ActiveRenderPassQueue);
				}
				using (new ProfilingScope(ScriptableRenderer.Profiling.RenderPass.configure))
				{
					foreach (ScriptableRenderPass scriptableRenderPass in this.activeRenderPassQueue)
					{
						scriptableRenderPass.Configure(commandBuffer, universalCameraData.cameraTargetDescriptor);
					}
					context.ExecuteCommandBuffer(commandBuffer);
					commandBuffer.Clear();
				}
				this.SetupNativeRenderPassFrameData(universalCameraData, this.useRenderPassEnabled);
				ScriptableRenderer.RenderBlocks renderBlocks = new ScriptableRenderer.RenderBlocks(this.m_ActiveRenderPassQueue);
				try
				{
					using (new ProfilingScope(ScriptableRenderer.Profiling.setupLights))
					{
						this.SetupLights(context, ref renderingData);
					}
					using (new ProfilingScope(ScriptableRenderer.Profiling.setupCamera))
					{
						this.SetPerCameraProperties(context, universalCameraData, camera, commandBuffer);
						VFXCameraXRSettings camXRSettings;
						camXRSettings.viewTotal = (universalCameraData.xr.enabled ? 2U : 1U);
						camXRSettings.viewCount = (uint)(universalCameraData.xr.enabled ? universalCameraData.xr.viewCount : 1);
						camXRSettings.viewOffset = (uint)universalCameraData.xr.multipassId;
						if (universalCameraData.xr.enabled)
						{
							universalCameraData.xr.StartSinglePass(commandBuffer);
						}
						VFXManager.ProcessCameraCommand(camera, commandBuffer, camXRSettings, *renderingData.cullResults);
						if (universalCameraData.xr.enabled)
						{
							universalCameraData.xr.StopSinglePass(commandBuffer);
						}
					}
					if (renderBlocks.GetLength(ScriptableRenderer.RenderPassBlock.BeforeRendering) > 0)
					{
						using (new ProfilingScope(ScriptableRenderer.Profiling.RenderBlock.beforeRendering))
						{
							this.ExecuteBlock(ScriptableRenderer.RenderPassBlock.BeforeRendering, renderBlocks, context, ref renderingData, false);
						}
					}
					using (new ProfilingScope(ScriptableRenderer.Profiling.setupCamera))
					{
						this.SetPerCameraProperties(context, universalCameraData, camera, commandBuffer);
						ScriptableRenderer.SetShaderTimeValues(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), time, deltaTime, smoothDeltaTime);
					}
					context.ExecuteCommandBuffer(commandBuffer);
					commandBuffer.Clear();
					this.BeginXRRendering(commandBuffer, context, ref renderingData.cameraData);
					if (renderBlocks.GetLength(ScriptableRenderer.RenderPassBlock.MainRenderingOpaque) > 0)
					{
						using (new ProfilingScope(ScriptableRenderer.Profiling.RenderBlock.mainRenderingOpaque))
						{
							this.ExecuteBlock(ScriptableRenderer.RenderPassBlock.MainRenderingOpaque, renderBlocks, context, ref renderingData, false);
						}
					}
					if (renderBlocks.GetLength(ScriptableRenderer.RenderPassBlock.MainRenderingTransparent) > 0)
					{
						using (new ProfilingScope(ScriptableRenderer.Profiling.RenderBlock.mainRenderingTransparent))
						{
							this.ExecuteBlock(ScriptableRenderer.RenderPassBlock.MainRenderingTransparent, renderBlocks, context, ref renderingData, false);
						}
					}
					if (universalCameraData.xr.enabled)
					{
						universalCameraData.xrUniversal.canMarkLateLatch = false;
					}
					if (renderBlocks.GetLength(ScriptableRenderer.RenderPassBlock.AfterRendering) > 0)
					{
						using (new ProfilingScope(ScriptableRenderer.Profiling.RenderBlock.afterRendering))
						{
							this.ExecuteBlock(ScriptableRenderer.RenderPassBlock.AfterRendering, renderBlocks, context, ref renderingData, false);
						}
					}
					this.EndXRRendering(commandBuffer, context, ref renderingData.cameraData);
					this.InternalFinishRenderingExecute(context, commandBuffer, universalCameraData.resolveFinalTarget);
					for (int i = 0; i < this.m_ActiveRenderPassQueue.Count; i++)
					{
						this.m_ActiveRenderPassQueue[i].m_ColorAttachmentIndices.Dispose();
						this.m_ActiveRenderPassQueue[i].m_InputAttachmentIndices.Dispose();
					}
				}
				finally
				{
					((IDisposable)renderBlocks).Dispose();
				}
			}
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}

		public void EnqueuePass(ScriptableRenderPass pass)
		{
			this.m_ActiveRenderPassQueue.Add(pass);
			if (this.disableNativeRenderPassInFeatures)
			{
				pass.useNativeRenderPass = false;
			}
		}

		protected static ClearFlag GetCameraClearFlag(ref CameraData cameraData)
		{
			return ScriptableRenderer.GetCameraClearFlag(cameraData.universalCameraData);
		}

		protected static ClearFlag GetCameraClearFlag(UniversalCameraData cameraData)
		{
			CameraClearFlags clearFlags = cameraData.camera.clearFlags;
			if (cameraData.renderType == CameraRenderType.Overlay)
			{
				if (!cameraData.clearDepth)
				{
					return ClearFlag.None;
				}
				return ClearFlag.DepthStencil;
			}
			else
			{
				DebugHandler debugHandler = cameraData.renderer.DebugHandler;
				if (debugHandler != null && debugHandler.IsActiveForCamera(cameraData.isPreviewCamera) && debugHandler.IsScreenClearNeeded)
				{
					return ClearFlag.All;
				}
				if (clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null && cameraData.postProcessEnabled && cameraData.xr.enabled)
				{
					return ClearFlag.All;
				}
				if ((clearFlags != CameraClearFlags.Skybox || !(RenderSettings.skybox != null)) && clearFlags != CameraClearFlags.Nothing)
				{
					return ClearFlag.All;
				}
				if (cameraData.cameraTargetDescriptor.msaaSamples > 1)
				{
					cameraData.camera.backgroundColor = Color.black;
					return ClearFlag.All;
				}
				return ClearFlag.DepthStencil;
			}
		}

		internal void OnPreCullRenderPasses(in CameraData cameraData)
		{
			for (int i = 0; i < this.rendererFeatures.Count; i++)
			{
				if (this.rendererFeatures[i].isActive)
				{
					this.rendererFeatures[i].OnCameraPreCull(this, cameraData);
				}
			}
		}

		internal void AddRenderPasses(ref RenderingData renderingData)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.addRenderPasses))
			{
				for (int i = 0; i < this.rendererFeatures.Count; i++)
				{
					if (this.rendererFeatures[i].isActive)
					{
						if (!this.rendererFeatures[i].SupportsNativeRenderPass())
						{
							this.disableNativeRenderPassInFeatures = true;
						}
						this.rendererFeatures[i].AddRenderPasses(this, ref renderingData);
						this.disableNativeRenderPassInFeatures = false;
					}
				}
				int count = this.activeRenderPassQueue.Count;
				for (int j = count - 1; j >= 0; j--)
				{
					if (this.activeRenderPassQueue[j] == null)
					{
						this.activeRenderPassQueue.RemoveAt(j);
					}
				}
				if (count > 0 && this.m_StoreActionsOptimizationSetting == StoreActionsOptimization.Auto)
				{
					ScriptableRenderer.m_UseOptimizedStoreActions = false;
				}
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		protected void SetupRenderPasses(in RenderingData renderingData)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.setupRenderPasses))
			{
				for (int i = 0; i < this.rendererFeatures.Count; i++)
				{
					if (this.rendererFeatures[i].isActive)
					{
						this.rendererFeatures[i].SetupRenderPasses(this, renderingData);
					}
				}
			}
		}

		private static void ClearRenderingState(IBaseCommandBuffer cmd)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.clearRenderingState))
			{
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadows, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MainLightShadowCascades, false);
				cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightsVertex, false);
				cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightsPixel, false);
				cmd.SetKeyword(ShaderGlobalKeywords.ClusterLightLoop, false);
				cmd.SetKeyword(ShaderGlobalKeywords.ForwardPlus, false);
				cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightShadows, false);
				cmd.SetKeyword(ShaderGlobalKeywords.ReflectionProbeBlending, false);
				cmd.SetKeyword(ShaderGlobalKeywords.ReflectionProbeBoxProjection, false);
				cmd.SetKeyword(ShaderGlobalKeywords.ReflectionProbeAtlas, false);
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadows, false);
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsLow, false);
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsMedium, false);
				cmd.SetKeyword(ShaderGlobalKeywords.SoftShadowsHigh, false);
				cmd.SetKeyword(ShaderGlobalKeywords.MixedLightingSubtractive, false);
				cmd.SetKeyword(ShaderGlobalKeywords.LightmapShadowMixing, false);
				cmd.SetKeyword(ShaderGlobalKeywords.ShadowsShadowMask, false);
				cmd.SetKeyword(ShaderGlobalKeywords.LinearToSRGBConversion, false);
				cmd.SetKeyword(ShaderGlobalKeywords.LightLayers, false);
				cmd.SetGlobalVector(ScreenSpaceAmbientOcclusionPass.s_AmbientOcclusionParamID, Vector4.zero);
			}
		}

		internal void Clear(CameraRenderType cameraType)
		{
			ScriptableRenderer.m_ActiveColorAttachments[0] = ScriptableRenderer.k_CameraTarget;
			for (int i = 1; i < ScriptableRenderer.m_ActiveColorAttachments.Length; i++)
			{
				ScriptableRenderer.m_ActiveColorAttachments[i] = null;
			}
			for (int j = 0; j < ScriptableRenderer.m_ActiveColorAttachments.Length; j++)
			{
				RenderTargetIdentifier[] activeColorAttachmentIDs = ScriptableRenderer.m_ActiveColorAttachmentIDs;
				int num = j;
				RTHandle rthandle = ScriptableRenderer.m_ActiveColorAttachments[j];
				activeColorAttachmentIDs[num] = ((rthandle != null) ? rthandle.nameID : 0);
			}
			ScriptableRenderer.m_ActiveDepthAttachment = ScriptableRenderer.k_CameraTarget;
			this.m_FirstTimeCameraColorTargetIsBound = (cameraType == CameraRenderType.Base);
			this.m_FirstTimeCameraDepthTargetIsBound = true;
			this.m_CameraColorTarget = null;
			this.m_CameraDepthTarget = null;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private void ExecuteBlock(int blockIndex, in ScriptableRenderer.RenderBlocks renderBlocks, ScriptableRenderContext context, ref RenderingData renderingData, bool submit = false)
		{
			UniversalCameraData cameraData = renderingData.frameData.Get<UniversalCameraData>();
			ScriptableRenderer.RenderBlocks renderBlocks2 = renderBlocks;
			foreach (int index in renderBlocks2.GetRange(blockIndex))
			{
				ScriptableRenderPass renderPass = this.m_ActiveRenderPassQueue[index];
				this.ExecuteRenderPass(context, renderPass, cameraData, ref renderingData);
			}
			if (submit)
			{
				context.Submit();
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private bool IsRenderPassEnabled(ScriptableRenderPass renderPass)
		{
			return renderPass.useNativeRenderPass && this.useRenderPassEnabled;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private unsafe void ExecuteRenderPass(ScriptableRenderContext context, ScriptableRenderPass renderPass, UniversalCameraData cameraData, ref RenderingData renderingData)
		{
			using (new ProfilingScope(renderPass.profilingSampler))
			{
				CommandBuffer commandBuffer = *renderingData.commandBuffer;
				if (cameraData.xr.supportsFoveatedRendering && ((renderPass.renderPassEvent >= RenderPassEvent.BeforeRenderingPrePasses && renderPass.renderPassEvent < RenderPassEvent.BeforeRenderingPostProcessing) || (renderPass.renderPassEvent > RenderPassEvent.AfterRendering && XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.FoveationImage))))
				{
					commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Enabled);
				}
				using (new ProfilingScope(ScriptableRenderer.Profiling.RenderPass.setRenderPassAttachments))
				{
					this.SetRenderPassAttachments(commandBuffer, renderPass, cameraData);
				}
				context.ExecuteCommandBuffer(commandBuffer);
				commandBuffer.Clear();
				if (this.IsRenderPassEnabled(renderPass) && cameraData.isRenderPassSupportedCamera)
				{
					this.ExecuteNativeRenderPass(context, renderPass, cameraData, ref renderingData);
				}
				else
				{
					renderPass.Execute(context, ref renderingData);
					context.ExecuteCommandBuffer(commandBuffer);
					commandBuffer.Clear();
				}
				if (cameraData.xr.enabled)
				{
					if (cameraData.xr.supportsFoveatedRendering)
					{
						commandBuffer.SetFoveatedRenderingMode(FoveatedRenderingMode.Disabled);
					}
					XRSystemUniversal.UnmarkShaderProperties(CommandBufferHelpers.GetRasterCommandBuffer(commandBuffer), cameraData.xrUniversal);
					context.ExecuteCommandBuffer(commandBuffer);
					commandBuffer.Clear();
				}
			}
		}

		internal bool IsSceneFilteringEnabled(Camera camera)
		{
			return false;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private void SetRenderPassAttachments(CommandBuffer cmd, ScriptableRenderPass renderPass, UniversalCameraData cameraData)
		{
			Camera camera = cameraData.camera;
			ClearFlag cameraClearFlag = ScriptableRenderer.GetCameraClearFlag(cameraData);
			if (RenderingUtils.GetValidColorBufferCount(renderPass.colorAttachmentHandles) == 0U)
			{
				return;
			}
			if (RenderingUtils.IsMRT(renderPass.colorAttachmentHandles))
			{
				bool flag = false;
				bool flag2 = false;
				int num = RenderingUtils.IndexOf(renderPass.colorAttachmentHandles, this.m_CameraColorTarget);
				if (num != -1 && this.m_FirstTimeCameraColorTargetIsBound)
				{
					this.m_FirstTimeCameraColorTargetIsBound = false;
					flag = ((cameraClearFlag & ClearFlag.Color) != (renderPass.clearFlag & ClearFlag.Color) || cameraData.backgroundColor != renderPass.clearColor);
				}
				RenderTargetIdentifier nameID = this.m_CameraDepthTarget.nameID;
				if (cameraData.xr.enabled)
				{
					nameID = new RenderTargetIdentifier(nameID, 0, CubemapFace.Unknown, -1);
				}
				if (new RenderTargetIdentifier(renderPass.depthAttachmentHandle.nameID, 0, CubemapFace.Unknown, 0) == new RenderTargetIdentifier(nameID, 0, CubemapFace.Unknown, 0) && this.m_FirstTimeCameraDepthTargetIsBound)
				{
					this.m_FirstTimeCameraDepthTargetIsBound = false;
					flag2 = ((cameraClearFlag & ClearFlag.DepthStencil) != (renderPass.clearFlag & ClearFlag.DepthStencil));
				}
				if (flag)
				{
					if ((cameraClearFlag & ClearFlag.Color) != ClearFlag.None && (!this.IsRenderPassEnabled(renderPass) || !cameraData.isRenderPassSupportedCamera))
					{
						ScriptableRenderer.SetRenderTarget(cmd, renderPass.colorAttachmentHandles[num], renderPass.depthAttachmentHandle, ClearFlag.Color, cameraData.backgroundColor);
					}
					if ((renderPass.clearFlag & ClearFlag.Color) != ClearFlag.None)
					{
						uint num2 = RenderingUtils.CountDistinct(renderPass.colorAttachmentHandles, this.m_CameraColorTarget);
						RTHandle[] array = ScriptableRenderer.m_TrimmedColorAttachmentCopies[(int)num2];
						int num3 = 0;
						for (int i = 0; i < renderPass.colorAttachmentHandles.Length; i++)
						{
							if (renderPass.colorAttachmentHandles[i] != null && renderPass.colorAttachmentHandles[i].nameID != 0 && renderPass.colorAttachmentHandles[i].nameID != this.m_CameraColorTarget.nameID)
							{
								array[num3] = renderPass.colorAttachmentHandles[i];
								num3++;
							}
						}
						RenderTargetIdentifier[] array2 = ScriptableRenderer.m_TrimmedColorAttachmentCopyIDs[(int)num2];
						int num4 = 0;
						while ((long)num4 < (long)((ulong)num2))
						{
							array2[num4] = array[num4].nameID;
							num4++;
						}
						if ((long)num3 != (long)((ulong)num2))
						{
							Debug.LogError("writeIndex and otherTargetsCount values differed. writeIndex:" + num3.ToString() + " otherTargetsCount:" + num2.ToString());
						}
						if (!this.IsRenderPassEnabled(renderPass) || !cameraData.isRenderPassSupportedCamera)
						{
							ScriptableRenderer.SetRenderTarget(cmd, array, array2, this.m_CameraDepthTarget, ClearFlag.Color, renderPass.clearColor);
						}
					}
				}
				ClearFlag clearFlag = ClearFlag.None;
				clearFlag |= (flag2 ? (cameraClearFlag & ClearFlag.DepthStencil) : (renderPass.clearFlag & ClearFlag.DepthStencil));
				clearFlag |= (flag ? (this.IsRenderPassEnabled(renderPass) ? (cameraClearFlag & ClearFlag.Color) : ClearFlag.None) : (renderPass.clearFlag & ClearFlag.Color));
				if (this.IsRenderPassEnabled(renderPass) && cameraData.isRenderPassSupportedCamera)
				{
					this.SetNativeRenderPassMRTAttachmentList(renderPass, cameraData, flag, clearFlag);
				}
				if (!RenderingUtils.SequenceEqual(renderPass.colorAttachmentHandles, ScriptableRenderer.m_ActiveColorAttachments) || renderPass.depthAttachmentHandle.nameID != ScriptableRenderer.m_ActiveDepthAttachment || clearFlag != ClearFlag.None)
				{
					int num5 = RenderingUtils.LastValid(renderPass.colorAttachmentHandles);
					if (num5 >= 0)
					{
						int num6 = num5 + 1;
						RTHandle[] array3 = ScriptableRenderer.m_TrimmedColorAttachmentCopies[num6];
						for (int j = 0; j < num6; j++)
						{
							array3[j] = renderPass.colorAttachmentHandles[j];
						}
						RenderTargetIdentifier[] array4 = ScriptableRenderer.m_TrimmedColorAttachmentCopyIDs[num6];
						for (int k = 0; k < num6; k++)
						{
							array4[k] = array3[k].nameID;
						}
						if (!this.IsRenderPassEnabled(renderPass) || !cameraData.isRenderPassSupportedCamera)
						{
							RTHandle depthAttachment = this.m_CameraDepthTarget;
							if (renderPass.overrideCameraTarget)
							{
								depthAttachment = renderPass.depthAttachmentHandle;
							}
							else
							{
								this.m_FirstTimeCameraDepthTargetIsBound = false;
							}
							ScriptableRenderer.SetRenderTarget(cmd, array3, array4, depthAttachment, clearFlag, renderPass.clearColor);
						}
						if (cameraData.xr.enabled)
						{
							bool renderIntoTexture = RenderingUtils.IndexOf(renderPass.colorAttachmentHandles, cameraData.xr.renderTarget) == -1;
							cameraData.PushBuiltinShaderConstantsXR(CommandBufferHelpers.GetRasterCommandBuffer(cmd), renderIntoTexture);
							XRSystemUniversal.MarkShaderProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData.xrUniversal, renderIntoTexture);
							return;
						}
					}
				}
			}
			else
			{
				RTHandle rthandle = renderPass.colorAttachmentHandle;
				RTHandle rthandle2 = renderPass.depthAttachmentHandle;
				if (!renderPass.overrideCameraTarget)
				{
					if (renderPass.renderPassEvent < RenderPassEvent.BeforeRenderingPrePasses)
					{
						return;
					}
					rthandle = this.m_CameraColorTarget;
					rthandle2 = this.m_CameraDepthTarget;
				}
				ClearFlag clearFlag2 = ClearFlag.None;
				Color color;
				if (rthandle.nameID == this.m_CameraColorTarget.nameID && this.m_FirstTimeCameraColorTargetIsBound)
				{
					this.m_FirstTimeCameraColorTargetIsBound = false;
					clearFlag2 |= (cameraClearFlag & ClearFlag.Color);
					if (SystemInfo.usesLoadStoreActions && new RenderTargetIdentifier(rthandle.nameID, 0, CubemapFace.Unknown, 0) != BuiltinRenderTextureType.CameraTarget)
					{
						clearFlag2 |= renderPass.clearFlag;
					}
					color = cameraData.backgroundColor;
					if (this.m_FirstTimeCameraDepthTargetIsBound)
					{
						this.m_FirstTimeCameraDepthTargetIsBound = false;
						clearFlag2 |= (cameraClearFlag & ClearFlag.DepthStencil);
					}
				}
				else
				{
					clearFlag2 |= (renderPass.clearFlag & ClearFlag.Color);
					color = renderPass.clearColor;
				}
				if (new RenderTargetIdentifier(this.m_CameraDepthTarget.nameID, 0, CubemapFace.Unknown, 0) != BuiltinRenderTextureType.CameraTarget && (rthandle2.nameID == this.m_CameraDepthTarget.nameID || rthandle.nameID == this.m_CameraDepthTarget.nameID) && this.m_FirstTimeCameraDepthTargetIsBound)
				{
					this.m_FirstTimeCameraDepthTargetIsBound = false;
					clearFlag2 |= (cameraClearFlag & ClearFlag.DepthStencil);
				}
				else
				{
					clearFlag2 |= (renderPass.clearFlag & ClearFlag.DepthStencil);
				}
				if (this.IsSceneFilteringEnabled(camera))
				{
					color.a = 0f;
					clearFlag2 &= ~ClearFlag.Depth;
				}
				if (this.DebugHandler != null && this.DebugHandler.IsActiveForCamera(cameraData.isPreviewCamera))
				{
					this.DebugHandler.TryGetScreenClearColor(ref color);
				}
				if (this.IsRenderPassEnabled(renderPass) && cameraData.isRenderPassSupportedCamera)
				{
					this.SetNativeRenderPassAttachmentList(renderPass, cameraData, rthandle, rthandle2, clearFlag2, color);
					return;
				}
				bool flag3 = false;
				if (rthandle.nameID != ScriptableRenderer.m_ActiveColorAttachments[0])
				{
					flag3 = true;
				}
				for (int l = 1; l < ScriptableRenderer.m_ActiveColorAttachments.Length; l++)
				{
					if (renderPass.colorAttachmentHandles[l] != ScriptableRenderer.m_ActiveColorAttachments[l])
					{
						flag3 = true;
						break;
					}
				}
				if (flag3 || rthandle2.nameID != ScriptableRenderer.m_ActiveDepthAttachment || clearFlag2 != ClearFlag.None || renderPass.colorStoreActions[0] != ScriptableRenderer.m_ActiveColorStoreActions[0] || renderPass.depthStoreAction != ScriptableRenderer.m_ActiveDepthStoreAction)
				{
					ScriptableRenderer.SetRenderTarget(cmd, rthandle, rthandle2, clearFlag2, color, renderPass.colorStoreActions[0], renderPass.depthStoreAction);
					if (cameraData.xr.enabled)
					{
						bool renderIntoTexture2 = rthandle.nameID != cameraData.xr.renderTarget;
						cameraData.PushBuiltinShaderConstantsXR(CommandBufferHelpers.GetRasterCommandBuffer(cmd), renderIntoTexture2);
						XRSystemUniversal.MarkShaderProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData.xrUniversal, renderIntoTexture2);
					}
				}
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private void BeginXRRendering(CommandBuffer cmd, ScriptableRenderContext context, ref CameraData cameraData)
		{
			if (cameraData.xr.enabled)
			{
				if (cameraData.xrUniversal.isLateLatchEnabled)
				{
					cameraData.xrUniversal.canMarkLateLatch = true;
				}
				cameraData.xr.StartSinglePass(cmd);
				if (cameraData.xr.supportsFoveatedRendering)
				{
					cmd.ConfigureFoveatedRendering(cameraData.xr.foveatedRenderingInfo);
					if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster))
					{
						cmd.SetKeyword(ShaderGlobalKeywords.FoveatedRenderingNonUniformRaster, true);
					}
				}
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private void EndXRRendering(CommandBuffer cmd, ScriptableRenderContext context, ref CameraData cameraData)
		{
			if (cameraData.xr.enabled)
			{
				cameraData.xr.StopSinglePass(cmd);
				if (XRSystem.foveatedRenderingCaps != FoveatedRenderingCaps.None)
				{
					if (XRSystem.foveatedRenderingCaps.HasFlag(FoveatedRenderingCaps.NonUniformRaster))
					{
						cmd.SetKeyword(ShaderGlobalKeywords.FoveatedRenderingNonUniformRaster, false);
					}
					cmd.ConfigureFoveatedRendering(IntPtr.Zero);
				}
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();
			}
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal static void SetRenderTarget(CommandBuffer cmd, RTHandle colorAttachment, RTHandle depthAttachment, ClearFlag clearFlag, Color clearColor)
		{
			ScriptableRenderer.m_ActiveColorAttachments[0] = colorAttachment;
			for (int i = 1; i < ScriptableRenderer.m_ActiveColorAttachments.Length; i++)
			{
				ScriptableRenderer.m_ActiveColorAttachments[i] = null;
			}
			for (int j = 0; j < ScriptableRenderer.m_ActiveColorAttachments.Length; j++)
			{
				RenderTargetIdentifier[] activeColorAttachmentIDs = ScriptableRenderer.m_ActiveColorAttachmentIDs;
				int num = j;
				RTHandle rthandle = ScriptableRenderer.m_ActiveColorAttachments[j];
				activeColorAttachmentIDs[num] = ((rthandle != null) ? rthandle.nameID : 0);
			}
			ScriptableRenderer.m_ActiveColorStoreActions[0] = RenderBufferStoreAction.Store;
			ScriptableRenderer.m_ActiveDepthStoreAction = RenderBufferStoreAction.Store;
			for (int k = 1; k < ScriptableRenderer.m_ActiveColorStoreActions.Length; k++)
			{
				ScriptableRenderer.m_ActiveColorStoreActions[k] = RenderBufferStoreAction.Store;
			}
			ScriptableRenderer.m_ActiveDepthAttachment = depthAttachment;
			RenderBufferLoadAction colorLoadAction = ((clearFlag & ClearFlag.Color) != ClearFlag.None) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			RenderBufferLoadAction depthLoadAction = ((clearFlag & ClearFlag.Depth) != ClearFlag.None || (clearFlag & ClearFlag.Stencil) != ClearFlag.None) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			if (colorAttachment.rt == null && depthAttachment.rt == null && depthAttachment.nameID == ScriptableRenderer.k_CameraTarget.nameID)
			{
				ScriptableRenderer.SetRenderTarget(cmd, colorAttachment, colorLoadAction, RenderBufferStoreAction.Store, colorAttachment, depthLoadAction, RenderBufferStoreAction.Store, clearFlag, clearColor);
				return;
			}
			ScriptableRenderer.SetRenderTarget(cmd, colorAttachment, colorLoadAction, RenderBufferStoreAction.Store, depthAttachment, depthLoadAction, RenderBufferStoreAction.Store, clearFlag, clearColor);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		internal static void SetRenderTarget(CommandBuffer cmd, RTHandle colorAttachment, RTHandle depthAttachment, ClearFlag clearFlag, Color clearColor, RenderBufferStoreAction colorStoreAction, RenderBufferStoreAction depthStoreAction)
		{
			ScriptableRenderer.m_ActiveColorAttachments[0] = colorAttachment;
			for (int i = 1; i < ScriptableRenderer.m_ActiveColorAttachments.Length; i++)
			{
				ScriptableRenderer.m_ActiveColorAttachments[i] = null;
			}
			for (int j = 0; j < ScriptableRenderer.m_ActiveColorAttachments.Length; j++)
			{
				RenderTargetIdentifier[] activeColorAttachmentIDs = ScriptableRenderer.m_ActiveColorAttachmentIDs;
				int num = j;
				RTHandle rthandle = ScriptableRenderer.m_ActiveColorAttachments[j];
				activeColorAttachmentIDs[num] = ((rthandle != null) ? rthandle.nameID : 0);
			}
			ScriptableRenderer.m_ActiveColorStoreActions[0] = colorStoreAction;
			ScriptableRenderer.m_ActiveDepthStoreAction = depthStoreAction;
			for (int k = 1; k < ScriptableRenderer.m_ActiveColorStoreActions.Length; k++)
			{
				ScriptableRenderer.m_ActiveColorStoreActions[k] = RenderBufferStoreAction.Store;
			}
			ScriptableRenderer.m_ActiveDepthAttachment = depthAttachment;
			RenderBufferLoadAction colorLoadAction = ((clearFlag & ClearFlag.Color) != ClearFlag.None) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			RenderBufferLoadAction depthLoadAction = ((clearFlag & ClearFlag.Depth) != ClearFlag.None) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			if (!ScriptableRenderer.m_UseOptimizedStoreActions)
			{
				if (colorStoreAction != RenderBufferStoreAction.StoreAndResolve)
				{
					colorStoreAction = RenderBufferStoreAction.Store;
				}
				if (depthStoreAction != RenderBufferStoreAction.StoreAndResolve)
				{
					depthStoreAction = RenderBufferStoreAction.Store;
				}
			}
			ScriptableRenderer.SetRenderTarget(cmd, colorAttachment, colorLoadAction, colorStoreAction, depthAttachment, depthLoadAction, depthStoreAction, clearFlag, clearColor);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private static void SetRenderTarget(CommandBuffer cmd, RTHandle colorAttachment, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RTHandle depthAttachment, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, ClearFlag clearFlags, Color clearColor)
		{
			if (depthAttachment.nameID == BuiltinRenderTextureType.CameraTarget)
			{
				CoreUtils.SetRenderTarget(cmd, colorAttachment, colorLoadAction, colorStoreAction, colorAttachment, depthLoadAction, depthStoreAction, clearFlags, clearColor, 0, CubemapFace.Unknown, -1);
				return;
			}
			CoreUtils.SetRenderTarget(cmd, colorAttachment, colorLoadAction, colorStoreAction, depthAttachment, depthLoadAction, depthStoreAction, clearFlags, clearColor, 0, CubemapFace.Unknown, -1);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		private static void SetRenderTarget(CommandBuffer cmd, RTHandle[] colorAttachments, RenderTargetIdentifier[] colorAttachmentIDs, RTHandle depthAttachment, ClearFlag clearFlag, Color clearColor)
		{
			ScriptableRenderer.m_ActiveColorAttachments = colorAttachments;
			ScriptableRenderer.m_ActiveColorAttachmentIDs = colorAttachmentIDs;
			ScriptableRenderer.m_ActiveDepthAttachment = depthAttachment;
			CoreUtils.SetRenderTarget(cmd, ScriptableRenderer.m_ActiveColorAttachmentIDs, depthAttachment, clearFlag, clearColor);
		}

		internal virtual void SwapColorBuffer(CommandBuffer cmd)
		{
		}

		internal virtual void EnableSwapBufferMSAA(bool enable)
		{
		}

		[Conditional("UNITY_EDITOR")]
		private void DrawGizmos(ScriptableRenderContext context, Camera camera, GizmoSubset gizmoSubset, ref RenderingData renderingData)
		{
		}

		[Conditional("UNITY_EDITOR")]
		private void DrawWireOverlay(ScriptableRenderContext context, Camera camera)
		{
			context.DrawWireOverlay(camera);
		}

		private unsafe void InternalStartRendering(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.internalStartRendering))
			{
				for (int i = 0; i < this.m_ActiveRenderPassQueue.Count; i++)
				{
					this.m_ActiveRenderPassQueue[i].OnCameraSetup(*renderingData.commandBuffer, ref renderingData);
				}
			}
			context.ExecuteCommandBuffer(*renderingData.commandBuffer);
			renderingData.commandBuffer->Clear();
		}

		private void InternalFinishRenderingCommon(CommandBuffer cmd, bool resolveFinalTarget)
		{
			using (new ProfilingScope(ScriptableRenderer.Profiling.internalFinishRenderingCommon))
			{
				for (int i = 0; i < this.m_ActiveRenderPassQueue.Count; i++)
				{
					this.m_ActiveRenderPassQueue[i].FrameCleanup(cmd);
				}
				if (resolveFinalTarget)
				{
					for (int j = 0; j < this.m_ActiveRenderPassQueue.Count; j++)
					{
						this.m_ActiveRenderPassQueue[j].OnFinishCameraStackRendering(cmd);
					}
					this.FinishRendering(cmd);
					this.m_IsPipelineExecuting = false;
				}
				this.m_ActiveRenderPassQueue.Clear();
			}
		}

		private void InternalFinishRenderingExecute(ScriptableRenderContext context, CommandBuffer cmd, bool resolveFinalTarget)
		{
			this.InternalFinishRenderingCommon(cmd, resolveFinalTarget);
			this.ResetNativeRenderPassFrameData();
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}

		private protected int AdjustAndGetScreenMSAASamples(RenderGraph renderGraph, bool useIntermediateColorTarget)
		{
			if (!SystemInfo.supportsMultisampledBackBuffer)
			{
				return 1;
			}
			if (UniversalRenderPipeline.canOptimizeScreenMSAASamples && useIntermediateColorTarget && renderGraph.nativeRenderPassesEnabled && Screen.msaaSamples > 1)
			{
				Screen.SetMSAASamples(1);
			}
			if (Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				return Mathf.Max(Screen.msaaSamples, 1);
			}
			return Mathf.Max(UniversalRenderPipeline.startFrameScreenMSAASamples, 1);
		}

		internal static void SortStable(List<ScriptableRenderPass> list)
		{
			for (int i = 1; i < list.Count; i++)
			{
				ScriptableRenderPass scriptableRenderPass = list[i];
				int num = i - 1;
				while (num >= 0 && scriptableRenderPass < list[num])
				{
					list[num + 1] = list[num];
					num--;
				}
				list[num + 1] = scriptableRenderPass;
			}
		}

		internal virtual bool supportsNativeRenderPassRendergraphCompiler
		{
			get
			{
				return false;
			}
		}

		public virtual bool supportsGPUOcclusion
		{
			get
			{
				return false;
			}
		}

		internal const int kRenderPassMapSize = 10;

		internal const int kRenderPassMaxCount = 20;

		private int m_LastBeginSubpassPassIndex;

		private Dictionary<Hash128, int[]> m_MergeableRenderPassesMap = new Dictionary<Hash128, int[]>(10);

		private int[][] m_MergeableRenderPassesMapArrays;

		private Hash128[] m_PassIndexToPassHash = new Hash128[20];

		private Dictionary<Hash128, int> m_RenderPassesAttachmentCount = new Dictionary<Hash128, int>(10);

		private int m_firstPassIndexOfLastMergeableGroup;

		private AttachmentDescriptor[] m_ActiveColorAttachmentDescriptors = new AttachmentDescriptor[]
		{
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment,
			RenderingUtils.emptyAttachment
		};

		private AttachmentDescriptor m_ActiveDepthAttachmentDescriptor;

		private bool[] m_IsActiveColorAttachmentTransient = new bool[8];

		internal RenderBufferStoreAction[] m_FinalColorStoreAction = new RenderBufferStoreAction[8];

		internal RenderBufferStoreAction m_FinalDepthStoreAction;

		internal bool hasReleasedRTs = true;

		internal static ScriptableRenderer current = null;

		private StoreActionsOptimization m_StoreActionsOptimizationSetting;

		private static bool m_UseOptimizedStoreActions = false;

		private const int k_RenderPassBlockCount = 4;

		protected static readonly RTHandle k_CameraTarget = RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget);

		private List<ScriptableRenderPass> m_ActiveRenderPassQueue = new List<ScriptableRenderPass>(32);

		private List<ScriptableRendererFeature> m_RendererFeatures = new List<ScriptableRendererFeature>(10);

		private RTHandle m_CameraColorTarget;

		private RTHandle m_CameraDepthTarget;

		private RTHandle m_CameraResolveTarget;

		private bool m_FirstTimeCameraColorTargetIsBound = true;

		private bool m_FirstTimeCameraDepthTargetIsBound = true;

		private bool m_IsPipelineExecuting;

		internal bool disableNativeRenderPassInFeatures;

		internal bool useRenderPassEnabled;

		private static RenderTargetIdentifier[] m_ActiveColorAttachmentIDs = new RenderTargetIdentifier[8];

		private static RTHandle[] m_ActiveColorAttachments = new RTHandle[8];

		private static RTHandle m_ActiveDepthAttachment;

		private ContextContainer m_frameData = new ContextContainer();

		private static RenderBufferStoreAction[] m_ActiveColorStoreActions = new RenderBufferStoreAction[8];

		private static RenderBufferStoreAction m_ActiveDepthStoreAction = RenderBufferStoreAction.Store;

		private static RenderTargetIdentifier[][] m_TrimmedColorAttachmentCopyIDs = new RenderTargetIdentifier[][]
		{
			Array.Empty<RenderTargetIdentifier>(),
			new RenderTargetIdentifier[1],
			new RenderTargetIdentifier[2],
			new RenderTargetIdentifier[3],
			new RenderTargetIdentifier[4],
			new RenderTargetIdentifier[5],
			new RenderTargetIdentifier[6],
			new RenderTargetIdentifier[7],
			new RenderTargetIdentifier[8]
		};

		private static RTHandle[][] m_TrimmedColorAttachmentCopies = new RTHandle[][]
		{
			Array.Empty<RTHandle>(),
			new RTHandle[1],
			new RTHandle[2],
			new RTHandle[3],
			new RTHandle[4],
			new RTHandle[5],
			new RTHandle[6],
			new RTHandle[7],
			new RTHandle[8]
		};

		private static Plane[] s_Planes = new Plane[6];

		private static Vector4[] s_VectorPlanes = new Vector4[6];

		private static class Profiling
		{
			public static readonly ProfilingSampler setMRTAttachmentsList = new ProfilingSampler("NativeRenderPass SetNativeRenderPassMRTAttachmentList");

			public static readonly ProfilingSampler setAttachmentList = new ProfilingSampler("NativeRenderPass SetNativeRenderPassAttachmentList");

			public static readonly ProfilingSampler execute = new ProfilingSampler("NativeRenderPass ExecuteNativeRenderPass");

			public static readonly ProfilingSampler setupFrameData = new ProfilingSampler("NativeRenderPass SetupNativeRenderPassFrameData");

			private const string k_Name = "ScriptableRenderer";

			public static readonly ProfilingSampler setPerCameraShaderVariables = new ProfilingSampler("ScriptableRenderer.SetPerCameraShaderVariables");

			public static readonly ProfilingSampler sortRenderPasses = new ProfilingSampler("Sort Render Passes");

			public static readonly ProfilingSampler recordRenderGraph = new ProfilingSampler("On Record Render Graph");

			public static readonly ProfilingSampler setupLights = new ProfilingSampler("ScriptableRenderer.SetupLights");

			public static readonly ProfilingSampler setupCamera = new ProfilingSampler("Setup Camera Properties");

			public static readonly ProfilingSampler vfxProcessCamera = new ProfilingSampler("VFX Process Camera");

			public static readonly ProfilingSampler addRenderPasses = new ProfilingSampler("ScriptableRenderer.AddRenderPasses");

			public static readonly ProfilingSampler setupRenderPasses = new ProfilingSampler("ScriptableRenderer.SetupRenderPasses");

			public static readonly ProfilingSampler clearRenderingState = new ProfilingSampler("ScriptableRenderer.ClearRenderingState");

			public static readonly ProfilingSampler internalStartRendering = new ProfilingSampler("ScriptableRenderer.InternalStartRendering");

			public static readonly ProfilingSampler internalFinishRenderingCommon = new ProfilingSampler("ScriptableRenderer.InternalFinishRenderingCommon");

			public static readonly ProfilingSampler drawGizmos = new ProfilingSampler("DrawGizmos");

			public static readonly ProfilingSampler drawWireOverlay = new ProfilingSampler("DrawWireOverlay");

			internal static readonly ProfilingSampler beginXRRendering = new ProfilingSampler("Begin XR Rendering");

			internal static readonly ProfilingSampler endXRRendering = new ProfilingSampler("End XR Rendering");

			internal static readonly ProfilingSampler initRenderGraphFrame = new ProfilingSampler("Initialize Frame");

			internal static readonly ProfilingSampler setEditorTarget = new ProfilingSampler("Set Editor Target");

			public static class RenderBlock
			{
				private const string k_Name = "RenderPassBlock";

				public static readonly ProfilingSampler beforeRendering = new ProfilingSampler("RenderPassBlock.BeforeRendering");

				public static readonly ProfilingSampler mainRenderingOpaque = new ProfilingSampler("RenderPassBlock.MainRenderingOpaque");

				public static readonly ProfilingSampler mainRenderingTransparent = new ProfilingSampler("RenderPassBlock.MainRenderingTransparent");

				public static readonly ProfilingSampler afterRendering = new ProfilingSampler("RenderPassBlock.AfterRendering");
			}

			public static class RenderPass
			{
				private const string k_Name = "ScriptableRenderPass";

				public static readonly ProfilingSampler configure = new ProfilingSampler("ScriptableRenderPass.Configure");

				public static readonly ProfilingSampler setRenderPassAttachments = new ProfilingSampler("ScriptableRenderPass.SetRenderPassAttachments");
			}
		}

		internal struct RenderPassDescriptor
		{
			internal RenderPassDescriptor(int width, int height, int sampleCount, int rtID)
			{
				this.w = width;
				this.h = height;
				this.samples = sampleCount;
				this.depthID = rtID;
			}

			internal int w;

			internal int h;

			internal int samples;

			internal int depthID;
		}

		public class RenderingFeatures
		{
			[Obsolete("cameraStacking has been deprecated use SupportedCameraRenderTypes() in ScriptableRenderer instead.", true)]
			public bool cameraStacking { get; set; }

			public bool msaa { get; set; } = true;
		}

		private static class RenderPassBlock
		{
			public static readonly int BeforeRendering = 0;

			public static readonly int MainRenderingOpaque = 1;

			public static readonly int MainRenderingTransparent = 2;

			public static readonly int AfterRendering = 3;
		}

		private class VFXProcessCameraPassData
		{
			internal UniversalRenderingData renderingData;

			internal Camera camera;

			internal VFXCameraXRSettings cameraXRSettings;

			internal XRPass xrPass;
		}

		private class DrawGizmosPassData
		{
			public RendererListHandle gizmoRenderList;

			public TextureHandle color;

			public TextureHandle depth;
		}

		private class DrawWireOverlayPassData
		{
			public RendererListHandle wireOverlayList;
		}

		private class BeginXRPassData
		{
			internal UniversalCameraData cameraData;
		}

		private class EndXRPassData
		{
			public UniversalCameraData cameraData;
		}

		private class DummyData
		{
		}

		private class PassData
		{
			internal ScriptableRenderer renderer;

			internal UniversalCameraData cameraData;

			internal bool isTargetBackbuffer;

			internal Vector2Int cameraTargetSizeCopy;
		}

		internal struct RenderBlocks : IDisposable
		{
			public RenderBlocks(List<ScriptableRenderPass> activeRenderPassQueue)
			{
				this.m_BlockEventLimits = new NativeArray<RenderPassEvent>(4, Allocator.Temp, NativeArrayOptions.ClearMemory);
				this.m_BlockRanges = new NativeArray<int>(this.m_BlockEventLimits.Length + 1, Allocator.Temp, NativeArrayOptions.ClearMemory);
				this.m_BlockRangeLengths = new NativeArray<int>(this.m_BlockRanges.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				this.m_BlockEventLimits[ScriptableRenderer.RenderPassBlock.BeforeRendering] = RenderPassEvent.BeforeRenderingPrePasses;
				this.m_BlockEventLimits[ScriptableRenderer.RenderPassBlock.MainRenderingOpaque] = RenderPassEvent.AfterRenderingOpaques;
				this.m_BlockEventLimits[ScriptableRenderer.RenderPassBlock.MainRenderingTransparent] = RenderPassEvent.AfterRenderingPostProcessing;
				this.m_BlockEventLimits[ScriptableRenderer.RenderPassBlock.AfterRendering] = (RenderPassEvent)2147483647;
				this.FillBlockRanges(activeRenderPassQueue);
				this.m_BlockEventLimits.Dispose();
				for (int i = 0; i < this.m_BlockRanges.Length - 1; i++)
				{
					this.m_BlockRangeLengths[i] = this.m_BlockRanges[i + 1] - this.m_BlockRanges[i];
				}
			}

			public void Dispose()
			{
				this.m_BlockRangeLengths.Dispose();
				this.m_BlockRanges.Dispose();
			}

			private void FillBlockRanges(List<ScriptableRenderPass> activeRenderPassQueue)
			{
				int index = 0;
				int num = 0;
				this.m_BlockRanges[index++] = 0;
				for (int i = 0; i < this.m_BlockEventLimits.Length - 1; i++)
				{
					while (num < activeRenderPassQueue.Count && activeRenderPassQueue[num].renderPassEvent < this.m_BlockEventLimits[i])
					{
						num++;
					}
					this.m_BlockRanges[index++] = num;
				}
				this.m_BlockRanges[index] = activeRenderPassQueue.Count;
			}

			public int GetLength(int index)
			{
				return this.m_BlockRangeLengths[index];
			}

			public ScriptableRenderer.RenderBlocks.BlockRange GetRange(int index)
			{
				return new ScriptableRenderer.RenderBlocks.BlockRange(this.m_BlockRanges[index], this.m_BlockRanges[index + 1]);
			}

			private NativeArray<RenderPassEvent> m_BlockEventLimits;

			private NativeArray<int> m_BlockRanges;

			private NativeArray<int> m_BlockRangeLengths;

			public struct BlockRange : IDisposable
			{
				public BlockRange(int begin, int end)
				{
					this.m_Current = ((begin < end) ? begin : end);
					this.m_End = ((end >= begin) ? end : begin);
					this.m_Current--;
				}

				public ScriptableRenderer.RenderBlocks.BlockRange GetEnumerator()
				{
					return this;
				}

				public bool MoveNext()
				{
					int num = this.m_Current + 1;
					this.m_Current = num;
					return num < this.m_End;
				}

				public int Current
				{
					get
					{
						return this.m_Current;
					}
				}

				public void Dispose()
				{
				}

				private int m_Current;

				private int m_End;
			}
		}
	}
}
