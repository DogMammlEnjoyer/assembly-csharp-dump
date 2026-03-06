using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal.Internal
{
	public class AdditionalLightsShadowCasterPass : ScriptableRenderPass
	{
		public AdditionalLightsShadowCasterPass(RenderPassEvent evt)
		{
			base.profilingSampler = new ProfilingSampler("Draw Additional Lights Shadowmap");
			base.renderPassEvent = evt;
			this.m_PassData = new AdditionalLightsShadowCasterPass.PassData();
			this.m_UseStructuredBuffer = RenderingUtils.useStructuredBuffer;
			int maxVisibleAdditionalLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
			int num = maxVisibleAdditionalLights + 1;
			int num2 = this.m_UseStructuredBuffer ? num : Math.Min(num, maxVisibleAdditionalLights);
			this.m_AdditionalLightIndexToVisibleLightIndex = new short[num2];
			this.m_VisibleLightIndexToAdditionalLightIndex = new short[num];
			this.m_VisibleLightIndexToIsCastingShadows = new bool[num];
			this.m_AdditionalLightIndexToShadowParams = new Vector4[num2];
			AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams = new Vector4[num2];
			for (int i = 0; i < AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams.Length; i++)
			{
				AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams[i] = AdditionalLightsShadowCasterPass.c_DefaultShadowParams;
			}
			if (!this.m_UseStructuredBuffer)
			{
				this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix = new Matrix4x4[maxVisibleAdditionalLights];
			}
			this.m_EmptyShadowmapNeedsClear = true;
		}

		public void Dispose()
		{
			RTHandle additionalLightsShadowmapHandle = this.m_AdditionalLightsShadowmapHandle;
			if (additionalLightsShadowmapHandle != null)
			{
				additionalLightsShadowmapHandle.Release();
			}
			RTHandle emptyAdditionalLightShadowmapTexture = this.m_EmptyAdditionalLightShadowmapTexture;
			if (emptyAdditionalLightShadowmapTexture == null)
			{
				return;
			}
			emptyAdditionalLightShadowmapTexture.Release();
		}

		internal static float CalcGuardAngle(float frustumAngleInDegrees, float guardBandSizeInTexels, float sliceResolutionInTexels)
		{
			float num = frustumAngleInDegrees * 0.017453292f / 2f;
			float num2 = Mathf.Tan(num);
			float num3 = sliceResolutionInTexels / 2f;
			float num4 = guardBandSizeInTexels / 2f;
			float num5 = 1f + num4 / num3;
			float num6 = Mathf.Atan(num2 * num5) - num;
			return 2f * num6 * 57.29578f;
		}

		internal static float GetPointLightShadowFrustumFovBiasInDegrees(int shadowSliceResolution, bool shadowFiltering)
		{
			float num = 4f;
			if (shadowSliceResolution <= 8)
			{
				if (!AdditionalLightsShadowCasterPass.m_IssuedMessageAboutPointLightHardShadowResolutionTooSmall)
				{
					Debug.LogWarning("Too many additional punctual lights shadows, increase shadow atlas size or remove some shadowed lights");
					AdditionalLightsShadowCasterPass.m_IssuedMessageAboutPointLightHardShadowResolutionTooSmall = true;
				}
			}
			else if (shadowSliceResolution <= 16)
			{
				num = 43f;
			}
			else if (shadowSliceResolution <= 32)
			{
				num = 18.55f;
			}
			else if (shadowSliceResolution <= 64)
			{
				num = 8.63f;
			}
			else if (shadowSliceResolution <= 128)
			{
				num = 4.13f;
			}
			else if (shadowSliceResolution <= 256)
			{
				num = 2.03f;
			}
			else if (shadowSliceResolution <= 512)
			{
				num = 1f;
			}
			else if (shadowSliceResolution <= 1024)
			{
				num = 0.5f;
			}
			else if (shadowSliceResolution <= 2048)
			{
				num = 0.25f;
			}
			if (shadowFiltering)
			{
				if (shadowSliceResolution <= 16)
				{
					if (!AdditionalLightsShadowCasterPass.m_IssuedMessageAboutPointLightSoftShadowResolutionTooSmall)
					{
						Debug.LogWarning("Too many additional punctual lights shadows to use Soft Shadows. Increase shadow atlas size, remove some shadowed lights or use Hard Shadows.");
						AdditionalLightsShadowCasterPass.m_IssuedMessageAboutPointLightSoftShadowResolutionTooSmall = true;
					}
				}
				else if (shadowSliceResolution <= 32)
				{
					num += 9.35f;
				}
				else if (shadowSliceResolution <= 64)
				{
					num += 4.07f;
				}
				else if (shadowSliceResolution <= 128)
				{
					num += 1.77f;
				}
				else if (shadowSliceResolution <= 256)
				{
					num += 0.85f;
				}
				else if (shadowSliceResolution <= 512)
				{
					num += 0.39f;
				}
				else if (shadowSliceResolution <= 1024)
				{
					num += 0.17f;
				}
				else if (shadowSliceResolution <= 2048)
				{
					num += 0.074f;
				}
			}
			return num;
		}

		private ulong ResolutionLog2ForHash(int resolution)
		{
			if (resolution <= 1024)
			{
				if (resolution == 512)
				{
					return 9UL;
				}
				if (resolution == 1024)
				{
					return 10UL;
				}
			}
			else
			{
				if (resolution == 2048)
				{
					return 11UL;
				}
				if (resolution == 4096)
				{
					return 12UL;
				}
			}
			return 8UL;
		}

		private ulong ComputeShadowRequestHash(UniversalLightData lightData, UniversalShadowData shadowData)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			ulong num3 = 0UL;
			ulong num4 = 0UL;
			ulong num5 = 0UL;
			ulong num6 = 0UL;
			ulong num7 = 0UL;
			ulong num8 = 0UL;
			NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
			for (int i = 0; i < visibleLights.Length; i++)
			{
				ref VisibleLight ptr = ref visibleLights.UnsafeElementAt(i);
				Light light = ptr.light;
				if (ShadowUtils.IsValidShadowCastingLight(lightData, i, ptr.lightType, light.shadows, light.shadowStrength))
				{
					LightType lightType = ptr.lightType;
					if (lightType != LightType.Spot)
					{
						if (lightType == LightType.Point)
						{
							num += 1UL;
						}
					}
					else
					{
						num2 += 1UL;
					}
					int num9 = shadowData.resolution[i];
					if (num9 <= 512)
					{
						if (num9 != 128)
						{
							if (num9 != 256)
							{
								if (num9 == 512)
								{
									num5 += 1UL;
								}
							}
							else
							{
								num4 += 1UL;
							}
						}
						else
						{
							num3 += 1UL;
						}
					}
					else if (num9 != 1024)
					{
						if (num9 != 2048)
						{
							if (num9 == 4096)
							{
								num8 += 1UL;
							}
						}
						else
						{
							num7 += 1UL;
						}
					}
					else
					{
						num6 += 1UL;
					}
				}
			}
			return this.ResolutionLog2ForHash(shadowData.additionalLightsShadowmapWidth) - 8UL | num << 3 | num2 << 11 | num3 << 19 | num4 << 27 | num5 << 35 | num6 << 43 | num7 << 50 | num8 << 57;
		}

		private float GetLightTypeIdentifierForShadowParams(LightType lightType)
		{
			if (lightType == LightType.Spot)
			{
				return 0f;
			}
			if (lightType != LightType.Point)
			{
				return -1f;
			}
			return 1f;
		}

		private bool UsesBakedShadows(Light light)
		{
			return light.bakingOutput.lightmapBakeType != LightmapBakeType.Realtime;
		}

		public bool Setup(ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData renderingData2 = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			return this.Setup(renderingData2, cameraData, lightData, shadowData);
		}

		public bool Setup(UniversalRenderingData renderingData, UniversalCameraData cameraData, UniversalLightData lightData, UniversalShadowData shadowData)
		{
			bool result;
			using (new ProfilingScope(this.m_ProfilingSetupSampler))
			{
				bool additionalLightShadowsEnabled = shadowData.additionalLightShadowsEnabled;
				if (!additionalLightShadowsEnabled)
				{
					if (this.AnyAdditionalLightHasMixedShadows(lightData))
					{
						result = this.SetupForEmptyRendering(cameraData.renderer.stripShadowsOffVariants, additionalLightShadowsEnabled, lightData, shadowData);
					}
					else
					{
						result = false;
					}
				}
				else if (!shadowData.supportsAdditionalLightShadows)
				{
					result = this.SetupForEmptyRendering(cameraData.renderer.stripShadowsOffVariants, additionalLightShadowsEnabled, lightData, shadowData);
				}
				else
				{
					this.Clear();
					this.renderTargetWidth = shadowData.additionalLightsShadowmapWidth;
					this.renderTargetHeight = shadowData.additionalLightsShadowmapHeight;
					NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
					ref AdditionalLightsShadowAtlasLayout ptr = ref shadowData.shadowAtlasLayout;
					if (this.m_VisibleLightIndexToAdditionalLightIndex.Length < visibleLights.Length)
					{
						this.m_VisibleLightIndexToAdditionalLightIndex = new short[visibleLights.Length];
						this.m_VisibleLightIndexToIsCastingShadows = new bool[visibleLights.Length];
					}
					int num = this.m_UseStructuredBuffer ? visibleLights.Length : Math.Min(visibleLights.Length, UniversalRenderPipeline.maxVisibleAdditionalLights);
					if (this.m_AdditionalLightIndexToVisibleLightIndex.Length < num)
					{
						this.m_AdditionalLightIndexToVisibleLightIndex = new short[num];
						this.m_AdditionalLightIndexToShadowParams = new Vector4[num];
					}
					int totalShadowSlicesCount = ptr.GetTotalShadowSlicesCount();
					int totalShadowResolutionRequestCount = ptr.GetTotalShadowResolutionRequestCount();
					int shadowSlicesScaleFactor = ptr.GetShadowSlicesScaleFactor();
					bool flag = ptr.HasTooManyShadowMaps();
					int atlasSize = ptr.GetAtlasSize();
					if (totalShadowSlicesCount < totalShadowResolutionRequestCount && !this.m_IssuedMessageAboutRemovedShadowSlices)
					{
						Debug.LogWarning(string.Format("Too many additional punctual lights shadows to look good, URP removed {0} shadow maps to make the others fit in the shadow atlas. To avoid this, increase shadow atlas size, remove some shadowed lights, replace soft shadows by hard shadows ; or replace point lights by spot lights", totalShadowResolutionRequestCount - totalShadowSlicesCount));
						this.m_IssuedMessageAboutRemovedShadowSlices = true;
					}
					if (!this.m_IssuedMessageAboutShadowMapsTooBig && flag)
					{
						Debug.LogWarning(string.Format("Too many additional punctual lights shadows. URP tried reducing shadow resolutions by {0} but it was still too much. Increase shadow atlas size, decrease big shadow resolutions, or reduce the number of shadow maps active in the same frame (currently was {1}).", shadowSlicesScaleFactor, totalShadowSlicesCount));
						this.m_IssuedMessageAboutShadowMapsTooBig = true;
					}
					if (!this.m_IssuedMessageAboutShadowMapsRescale && shadowSlicesScaleFactor > 1)
					{
						Debug.Log(string.Format("Reduced additional punctual light shadows resolution by {0} to make {1} shadow maps fit in the {2}x{3} shadow atlas. To avoid this, increase shadow atlas size, decrease big shadow resolutions, or reduce the number of shadow maps active in the same frame", new object[]
						{
							shadowSlicesScaleFactor,
							totalShadowSlicesCount,
							atlasSize,
							atlasSize
						}));
						this.m_IssuedMessageAboutShadowMapsRescale = true;
					}
					if (this.m_AdditionalLightsShadowSlices == null || this.m_AdditionalLightsShadowSlices.Length < totalShadowSlicesCount)
					{
						this.m_AdditionalLightsShadowSlices = new ShadowSliceData[totalShadowSlicesCount];
					}
					if (this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix == null || (this.m_UseStructuredBuffer && this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix.Length < totalShadowSlicesCount))
					{
						this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix = new Matrix4x4[totalShadowSlicesCount];
					}
					for (int i = 0; i < num; i++)
					{
						this.m_AdditionalLightIndexToShadowParams[i] = AdditionalLightsShadowCasterPass.c_DefaultShadowParams;
					}
					for (int j = 0; j < this.m_VisibleLightIndexToAdditionalLightIndex.Length; j++)
					{
						this.m_VisibleLightIndexToAdditionalLightIndex[j] = -1;
						this.m_VisibleLightIndexToIsCastingShadows[j] = false;
					}
					short num2 = 0;
					short num3 = 0;
					bool supportsSoftShadows = shadowData.supportsSoftShadows;
					UniversalRenderer universalRenderer = (UniversalRenderer)cameraData.renderer;
					bool flag2 = universalRenderer.renderingModeActual == RenderingMode.Deferred;
					bool shadowTransparentReceive = universalRenderer.shadowTransparentReceive;
					bool flag3 = !flag2 || shadowTransparentReceive;
					for (int k = 0; k < visibleLights.Length; k++)
					{
						if (k != lightData.mainLightIndex)
						{
							short num4;
							if (flag3)
							{
								num2 = (num4 = num2) + 1;
							}
							else
							{
								num4 = num3;
							}
							short num5 = num4;
							this.m_VisibleLightIndexToAdditionalLightIndex[k] = num5;
							if ((int)num5 < this.m_AdditionalLightIndexToVisibleLightIndex.Length)
							{
								this.m_AdditionalLightIndexToVisibleLightIndex[(int)num5] = (short)k;
								if (this.m_ShadowSliceToAdditionalLightIndex.Count < totalShadowSlicesCount)
								{
									ref VisibleLight ptr2 = ref visibleLights.UnsafeElementAt(k);
									Light light = ptr2.light;
									if (light == null)
									{
										break;
									}
									LightType lightType = ptr2.lightType;
									bool flag4 = this.UsesBakedShadows(light);
									float lightTypeIdentifierForShadowParams = this.GetLightTypeIdentifierForShadowParams(lightType);
									int punctualLightShadowSlicesCount = ShadowUtils.GetPunctualLightShadowSlicesCount(lightType);
									bool flag5 = ShadowUtils.IsValidShadowCastingLight(lightData, k, ptr2.lightType, light.shadows, light.shadowStrength);
									if (flag5 && this.m_ShadowSliceToAdditionalLightIndex.Count + punctualLightShadowSlicesCount > totalShadowSlicesCount)
									{
										if (!this.m_IssuedMessageAboutShadowSlicesTooMany)
										{
											Debug.Log("There are too many shadowed additional punctual lights active at the same time, URP will not render all the shadows. To ensure all shadows are rendered, reduce the number of shadowed additional lights in the scene ; make sure they are not active at the same time ; or replace point lights by spot lights (spot lights use less shadow maps than point lights).");
											this.m_IssuedMessageAboutShadowSlicesTooMany = true;
											break;
										}
										break;
									}
									else
									{
										float y = ShadowUtils.SoftShadowQualityToShaderProperty(light, supportsSoftShadows && light.shadows == LightShadows.Soft);
										int count = this.m_ShadowSliceToAdditionalLightIndex.Count;
										bool flag6 = false;
										byte b = 0;
										while ((int)b < punctualLightShadowSlicesCount)
										{
											int count2 = this.m_ShadowSliceToAdditionalLightIndex.Count;
											Bounds bounds;
											bool shadowCasterBounds = renderingData.cullResults.GetShadowCasterBounds(k, out bounds);
											if (!shadowData.supportsAdditionalLightShadows || !flag5 || !shadowCasterBounds)
											{
												if (flag4 && lightTypeIdentifierForShadowParams > -1f)
												{
													this.m_AdditionalLightIndexToShadowParams[(int)num5] = new Vector4(light.shadowStrength, y, lightTypeIdentifierForShadowParams, (float)num5);
													this.m_VisibleLightIndexToIsCastingShadows[k] = flag4;
												}
											}
											else if (ptr.HasSpaceForLight(k))
											{
												if (lightType == LightType.Spot)
												{
													ref URPLightShadowCullingInfos ptr3 = ref shadowData.visibleLightsShadowCullingInfos.UnsafeElementAt(k);
													ref ShadowSliceData ptr4 = ref ptr3.slices.UnsafeElementAt(0);
													this.m_AdditionalLightsShadowSlices[count2].viewMatrix = ptr4.viewMatrix;
													this.m_AdditionalLightsShadowSlices[count2].projectionMatrix = ptr4.projectionMatrix;
													this.m_AdditionalLightsShadowSlices[count2].splitData = ptr4.splitData;
													if (ptr3.IsSliceValid(0))
													{
														this.m_ShadowSliceToAdditionalLightIndex.Add(num5);
														this.m_GlobalShadowSliceIndexToPerLightShadowSliceIndex.Add(b);
														this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix[count2] = ptr4.shadowTransform;
														this.m_AdditionalLightIndexToShadowParams[(int)num5] = new Vector4(light.shadowStrength, y, lightTypeIdentifierForShadowParams, (float)count);
														flag6 = true;
													}
												}
												else if (lightType == LightType.Point)
												{
													ref URPLightShadowCullingInfos ptr5 = ref shadowData.visibleLightsShadowCullingInfos.UnsafeElementAt(k);
													ref ShadowSliceData ptr6 = ref ptr5.slices.UnsafeElementAt((int)b);
													this.m_AdditionalLightsShadowSlices[count2].viewMatrix = ptr6.viewMatrix;
													this.m_AdditionalLightsShadowSlices[count2].projectionMatrix = ptr6.projectionMatrix;
													this.m_AdditionalLightsShadowSlices[count2].splitData = ptr6.splitData;
													if (ptr5.IsSliceValid((int)b))
													{
														this.m_ShadowSliceToAdditionalLightIndex.Add(num5);
														this.m_GlobalShadowSliceIndexToPerLightShadowSliceIndex.Add(b);
														this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix[count2] = ptr6.shadowTransform;
														this.m_AdditionalLightIndexToShadowParams[(int)num5] = new Vector4(light.shadowStrength, y, lightTypeIdentifierForShadowParams, (float)count);
														flag6 = true;
													}
												}
											}
											b += 1;
										}
										if (flag6)
										{
											this.m_VisibleLightIndexToIsCastingShadows[k] = true;
											this.m_VisibleLightIndexToAdditionalLightIndex[k] = num5;
											this.m_AdditionalLightIndexToVisibleLightIndex[(int)num5] = (short)k;
											num3 += 1;
										}
										else
										{
											this.m_VisibleLightIndexToIsCastingShadows[k] = flag4;
											this.m_AdditionalLightIndexToShadowParams[(int)num5] = new Vector4(light.shadowStrength, y, lightTypeIdentifierForShadowParams, AdditionalLightsShadowCasterPass.c_DefaultShadowParams.w);
										}
									}
								}
							}
						}
					}
					if (num3 == 0)
					{
						result = this.SetupForEmptyRendering(cameraData.renderer.stripShadowsOffVariants, additionalLightShadowsEnabled, lightData, shadowData);
					}
					else
					{
						int count3 = this.m_ShadowSliceToAdditionalLightIndex.Count;
						int num6 = 0;
						int num7 = 0;
						for (int l = 0; l < totalShadowSlicesCount; l++)
						{
							AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest sortedShadowResolutionRequest = ptr.GetSortedShadowResolutionRequest(l);
							num6 = Mathf.Max(num6, (int)(sortedShadowResolutionRequest.offsetX + sortedShadowResolutionRequest.allocatedResolution));
							num7 = Mathf.Max(num7, (int)(sortedShadowResolutionRequest.offsetY + sortedShadowResolutionRequest.allocatedResolution));
						}
						this.renderTargetWidth = Mathf.NextPowerOfTwo(num6);
						this.renderTargetHeight = Mathf.NextPowerOfTwo(num7);
						float num8 = 1f / (float)this.renderTargetWidth;
						float num9 = 1f / (float)this.renderTargetHeight;
						for (int m = 0; m < count3; m++)
						{
							int num10 = (int)this.m_ShadowSliceToAdditionalLightIndex[m];
							if (!Mathf.Approximately(this.m_AdditionalLightIndexToShadowParams[num10].x, 0f) && !Mathf.Approximately(this.m_AdditionalLightIndexToShadowParams[num10].w, -1f))
							{
								int originalVisibleLightIndex = (int)this.m_AdditionalLightIndexToVisibleLightIndex[num10];
								int sliceIndex = (int)this.m_GlobalShadowSliceIndexToPerLightShadowSliceIndex[m];
								AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest sliceShadowResolutionRequest = ptr.GetSliceShadowResolutionRequest(originalVisibleLightIndex, sliceIndex);
								int allocatedResolution = (int)sliceShadowResolutionRequest.allocatedResolution;
								Matrix4x4 identity = Matrix4x4.identity;
								identity.m00 = (float)allocatedResolution * num8;
								identity.m11 = (float)allocatedResolution * num9;
								this.m_AdditionalLightsShadowSlices[m].offsetX = (int)sliceShadowResolutionRequest.offsetX;
								this.m_AdditionalLightsShadowSlices[m].offsetY = (int)sliceShadowResolutionRequest.offsetY;
								this.m_AdditionalLightsShadowSlices[m].resolution = allocatedResolution;
								identity.m03 = (float)this.m_AdditionalLightsShadowSlices[m].offsetX * num8;
								identity.m13 = (float)this.m_AdditionalLightsShadowSlices[m].offsetY * num9;
								this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix[m] = identity * this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix[m];
							}
						}
						this.UpdateTextureDescriptorIfNeeded();
						this.m_MaxShadowDistanceSq = cameraData.maxShadowDistance * cameraData.maxShadowDistance;
						this.m_CascadeBorder = shadowData.mainLightShadowCascadeBorder;
						this.m_CreateEmptyShadowmap = false;
						base.useNativeRenderPass = true;
						result = true;
					}
				}
			}
			return result;
		}

		private void UpdateTextureDescriptorIfNeeded()
		{
			if (this.m_AdditionalLightShadowDescriptor.width != this.renderTargetWidth || this.m_AdditionalLightShadowDescriptor.height != this.renderTargetHeight || this.m_AdditionalLightShadowDescriptor.depthBufferBits != 16 || this.m_AdditionalLightShadowDescriptor.colorFormat != RenderTextureFormat.Shadowmap)
			{
				this.m_AdditionalLightShadowDescriptor = new RenderTextureDescriptor(this.renderTargetWidth, this.renderTargetHeight, RenderTextureFormat.Shadowmap, 16);
			}
		}

		private bool AnyAdditionalLightHasMixedShadows(UniversalLightData lightData)
		{
			for (int i = 0; i < lightData.visibleLights.Length; i++)
			{
				if (i != lightData.mainLightIndex)
				{
					Light light = lightData.visibleLights[i].light;
					if (light.shadows != LightShadows.None && light.bakingOutput.isBaked && light.bakingOutput.mixedLightingMode != MixedLightingMode.IndirectOnly && light.bakingOutput.lightmapBakeType == LightmapBakeType.Mixed)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool SetupForEmptyRendering(bool stripShadowsOffVariants, bool shadowsEnabled, UniversalLightData lightData, UniversalShadowData shadowData)
		{
			if (!stripShadowsOffVariants)
			{
				return false;
			}
			shadowData.isKeywordAdditionalLightShadowsEnabled = true;
			this.m_CreateEmptyShadowmap = true;
			base.useNativeRenderPass = false;
			this.m_SetKeywordForEmptyShadowmap = shadowsEnabled;
			float x;
			float y;
			ShadowUtils.GetScaleAndBiasForLinearDistanceFade(this.m_MaxShadowDistanceSq, this.m_CascadeBorder, out x, out y);
			AdditionalLightsShadowCasterPass.s_EmptyAdditionalShadowFadeParams = new Vector4(x, y, 0f, 0f);
			NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
			if (AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams.Length < visibleLights.Length)
			{
				this.m_VisibleLightIndexToAdditionalLightIndex = new short[visibleLights.Length];
				this.m_VisibleLightIndexToIsCastingShadows = new bool[visibleLights.Length];
				AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams = new Vector4[visibleLights.Length];
				AdditionalLightsShadowCasterPass.isAdditionalShadowParamsDirty = true;
			}
			if (AdditionalLightsShadowCasterPass.isAdditionalShadowParamsDirty)
			{
				AdditionalLightsShadowCasterPass.isAdditionalShadowParamsDirty = false;
				Debug.LogWarning(string.Format("The number of visible additional lights {0} exceeds the maximum supported lights {1}.", visibleLights.Length, UniversalRenderPipeline.maxVisibleAdditionalLights) + " Please refer URP documentation to change maximum number of visible lights or reduce the number of lights to maximum allowed additional lights.");
			}
			short num = 0;
			for (int i = 0; i < visibleLights.Length; i++)
			{
				if (i != lightData.mainLightIndex)
				{
					Light light = visibleLights.UnsafeElementAt(i).light;
					if (!(light == null))
					{
						float lightTypeIdentifierForShadowParams = this.GetLightTypeIdentifierForShadowParams(light.type);
						if (lightTypeIdentifierForShadowParams >= 0f)
						{
							short num2 = num;
							num = num2 + 1;
							short num3 = num2;
							LightShadows shadows = light.shadows;
							if (shadows > LightShadows.None)
							{
								bool flag = shadows != LightShadows.Soft;
								bool supportsSoftShadows = shadowData.supportsSoftShadows;
								float y2 = ShadowUtils.SoftShadowQualityToShaderProperty(light, supportsSoftShadows && flag);
								AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams[(int)num3] = new Vector4(light.shadowStrength, y2, lightTypeIdentifierForShadowParams, (float)num3);
							}
							else
							{
								AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams[(int)num3] = AdditionalLightsShadowCasterPass.c_DefaultShadowParams;
							}
							this.m_VisibleLightIndexToAdditionalLightIndex[i] = num3;
							this.m_VisibleLightIndexToIsCastingShadows[i] = this.UsesBakedShadows(light);
						}
					}
				}
			}
			return true;
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			if (this.m_CreateEmptyShadowmap)
			{
				if (ShadowUtils.ShadowRTReAllocateIfNeeded(ref this.m_EmptyAdditionalLightShadowmapTexture, 1, 1, 16, 1, 0f, "_EmptyAdditionalLightShadowmapTexture"))
				{
					this.m_EmptyShadowmapNeedsClear = true;
				}
				if (!this.m_EmptyShadowmapNeedsClear)
				{
					return;
				}
				base.ConfigureTarget(this.m_EmptyAdditionalLightShadowmapTexture);
				this.m_EmptyShadowmapNeedsClear = false;
			}
			else
			{
				ShadowUtils.ShadowRTReAllocateIfNeeded(ref this.m_AdditionalLightsShadowmapHandle, this.renderTargetWidth, this.renderTargetHeight, 16, 1, 0f, "_AdditionalLightsShadowmapTexture");
				base.ConfigureTarget(this.m_AdditionalLightsShadowmapHandle);
			}
			base.ConfigureClear(ClearFlag.All, Color.black);
		}

		[Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
		public unsafe override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			ContextContainer frameData = renderingData.frameData;
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			RasterCommandBuffer rasterCommandBuffer = CommandBufferHelpers.GetRasterCommandBuffer(*renderingData.commandBuffer);
			if (this.m_CreateEmptyShadowmap)
			{
				if (this.m_SetKeywordForEmptyShadowmap)
				{
					rasterCommandBuffer.EnableKeyword(ShaderGlobalKeywords.AdditionalLightShadows);
				}
				AdditionalLightsShadowCasterPass.SetShadowParamsForEmptyShadowmap(rasterCommandBuffer);
				universalRenderingData.commandBuffer.SetGlobalTexture(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalLightsShadowmapID, this.m_EmptyAdditionalLightShadowmapTexture);
				return;
			}
			UniversalShadowData universalShadowData = frameData.Get<UniversalShadowData>();
			if (!universalShadowData.supportsAdditionalLightShadows)
			{
				return;
			}
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			this.InitPassData(ref this.m_PassData, cameraData, lightData, universalShadowData);
			this.m_PassData.allocatedShadowAtlasSize = this.m_AdditionalLightsShadowmapHandle.referenceSize;
			this.InitRendererLists(ref universalRenderingData.cullResults, ref this.m_PassData, context, null, false);
			this.RenderAdditionalShadowmapAtlas(rasterCommandBuffer, ref this.m_PassData, false);
			universalRenderingData.commandBuffer.SetGlobalTexture(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalLightsShadowmapID, this.m_AdditionalLightsShadowmapHandle.nameID);
		}

		public int GetShadowLightIndexFromLightIndex(int visibleLightIndex)
		{
			if (visibleLightIndex < 0 || visibleLightIndex >= this.m_VisibleLightIndexToAdditionalLightIndex.Length || !this.m_VisibleLightIndexToIsCastingShadows[visibleLightIndex])
			{
				return -1;
			}
			return (int)this.m_VisibleLightIndexToAdditionalLightIndex[visibleLightIndex];
		}

		private void Clear()
		{
			this.m_ShadowSliceToAdditionalLightIndex.Clear();
			this.m_GlobalShadowSliceIndexToPerLightShadowSliceIndex.Clear();
		}

		internal static void SetShadowParamsForEmptyShadowmap(RasterCommandBuffer rasterCommandBuffer)
		{
			rasterCommandBuffer.SetGlobalVector(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowFadeParams, AdditionalLightsShadowCasterPass.s_EmptyAdditionalShadowFadeParams);
			if (RenderingUtils.useStructuredBuffer)
			{
				ComputeBuffer additionalLightShadowParamsStructuredBuffer = ShaderData.instance.GetAdditionalLightShadowParamsStructuredBuffer(AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams.Length);
				additionalLightShadowParamsStructuredBuffer.SetData(AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams);
				rasterCommandBuffer.SetGlobalBuffer(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowParams_SSBO, additionalLightShadowParamsStructuredBuffer);
				return;
			}
			if (AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams.Length <= UniversalRenderPipeline.maxVisibleAdditionalLights)
			{
				rasterCommandBuffer.SetGlobalVectorArray(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowParams, AdditionalLightsShadowCasterPass.s_EmptyAdditionalLightIndexToShadowParams);
			}
		}

		private void RenderAdditionalShadowmapAtlas(RasterCommandBuffer cmd, ref AdditionalLightsShadowCasterPass.PassData data, bool useRenderGraph)
		{
			NativeArray<VisibleLight> visibleLights = data.lightData.visibleLights;
			bool flag = false;
			using (new ProfilingScope(cmd, ProfilingSampler.Get<URPProfileId>(URPProfileId.AdditionalLightsShadow)))
			{
				if (!useRenderGraph)
				{
					ShadowUtils.SetWorldToCameraAndCameraToWorldMatrices(cmd, data.viewMatrix);
				}
				bool flag2 = false;
				int count = this.m_ShadowSliceToAdditionalLightIndex.Count;
				if (count > 0)
				{
					cmd.SetKeyword(ShaderGlobalKeywords.CastingPunctualLightShadow, true);
				}
				Vector4 b = new Vector4(-10f, -10f, -10f, -10f);
				for (int i = 0; i < count; i++)
				{
					int num = (int)this.m_ShadowSliceToAdditionalLightIndex[i];
					if (!ShadowUtils.FastApproximately(this.m_AdditionalLightIndexToShadowParams[num].x, 0f) && !ShadowUtils.FastApproximately(this.m_AdditionalLightIndexToShadowParams[num].w, -1f))
					{
						int num2 = (int)this.m_AdditionalLightIndexToVisibleLightIndex[num];
						ref VisibleLight ptr = ref visibleLights.UnsafeElementAt(num2);
						ShadowSliceData shadowSliceData = this.m_AdditionalLightsShadowSlices[i];
						Vector4 shadowBias = ShadowUtils.GetShadowBias(ref ptr, num2, data.shadowData, shadowSliceData.projectionMatrix, (float)shadowSliceData.resolution);
						if (i == 0 || !ShadowUtils.FastApproximately(shadowBias, b))
						{
							ShadowUtils.SetShadowBias(cmd, shadowBias);
							b = shadowBias;
						}
						Vector3 lightPosition = ptr.localToWorldMatrix.GetColumn(3);
						ShadowUtils.SetLightPosition(cmd, lightPosition);
						RendererList rendererList = useRenderGraph ? data.shadowRendererListsHdl[i] : data.shadowRendererLists[i];
						ShadowUtils.RenderShadowSlice(cmd, ref shadowSliceData, ref rendererList, shadowSliceData.projectionMatrix, shadowSliceData.viewMatrix);
						flag |= (ptr.light.shadows == LightShadows.Soft);
						flag2 = true;
					}
				}
				bool flag3 = data.shadowData.supportsMainLightShadows && data.lightData.mainLightIndex != -1 && visibleLights[data.lightData.mainLightIndex].light.shadows == LightShadows.Soft;
				bool flag4 = !data.stripShadowsOffVariants;
				data.shadowData.isKeywordAdditionalLightShadowsEnabled = (!flag4 || flag2);
				cmd.SetKeyword(ShaderGlobalKeywords.AdditionalLightShadows, data.shadowData.isKeywordAdditionalLightShadowsEnabled);
				bool flag5 = data.shadowData.supportsSoftShadows && (flag3 || flag);
				data.shadowData.isKeywordSoftShadowsEnabled = flag5;
				ShadowUtils.SetSoftShadowQualityShaderKeywords(cmd, data.shadowData);
				if (flag2)
				{
					this.SetupAdditionalLightsShadowReceiverConstants(cmd, data.allocatedShadowAtlasSize, data.useStructuredBuffer, flag5);
				}
			}
		}

		private void SetupAdditionalLightsShadowReceiverConstants(RasterCommandBuffer cmd, Vector2Int allocatedShadowAtlasSize, bool useStructuredBuffer, bool softShadows)
		{
			if (useStructuredBuffer)
			{
				ComputeBuffer additionalLightShadowParamsStructuredBuffer = ShaderData.instance.GetAdditionalLightShadowParamsStructuredBuffer(this.m_AdditionalLightIndexToShadowParams.Length);
				additionalLightShadowParamsStructuredBuffer.SetData(this.m_AdditionalLightIndexToShadowParams);
				cmd.SetGlobalBuffer(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowParams_SSBO, additionalLightShadowParamsStructuredBuffer);
				ComputeBuffer additionalLightShadowSliceMatricesStructuredBuffer = ShaderData.instance.GetAdditionalLightShadowSliceMatricesStructuredBuffer(this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix.Length);
				additionalLightShadowSliceMatricesStructuredBuffer.SetData(this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix);
				cmd.SetGlobalBuffer(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalLightsWorldToShadow_SSBO, additionalLightShadowSliceMatricesStructuredBuffer);
			}
			else
			{
				cmd.SetGlobalVectorArray(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowParams, this.m_AdditionalLightIndexToShadowParams);
				cmd.SetGlobalMatrixArray(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalLightsWorldToShadow, this.m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix);
			}
			float x;
			float y;
			ShadowUtils.GetScaleAndBiasForLinearDistanceFade(this.m_MaxShadowDistanceSq, this.m_CascadeBorder, out x, out y);
			cmd.SetGlobalVector(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowFadeParams, new Vector4(x, y, 0f, 0f));
			if (softShadows)
			{
				Vector2 vector = Vector2.one / allocatedShadowAtlasSize;
				Vector2 vector2 = vector * 0.5f;
				cmd.SetGlobalVector(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowOffset0, new Vector4(-vector2.x, -vector2.y, vector2.x, -vector2.y));
				cmd.SetGlobalVector(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowOffset1, new Vector4(-vector2.x, vector2.y, vector2.x, vector2.y));
				cmd.SetGlobalVector(AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalShadowmapSize, new Vector4(vector.x, vector.y, (float)allocatedShadowAtlasSize.x, (float)allocatedShadowAtlasSize.y));
			}
		}

		private void InitPassData(ref AdditionalLightsShadowCasterPass.PassData passData, UniversalCameraData cameraData, UniversalLightData lightData, UniversalShadowData shadowData)
		{
			passData.pass = this;
			passData.lightData = lightData;
			passData.shadowData = shadowData;
			passData.viewMatrix = cameraData.GetViewMatrix(0);
			passData.stripShadowsOffVariants = cameraData.renderer.stripShadowsOffVariants;
			passData.emptyShadowmap = this.m_CreateEmptyShadowmap;
			passData.setKeywordForEmptyShadowmap = this.m_SetKeywordForEmptyShadowmap;
			passData.useStructuredBuffer = this.m_UseStructuredBuffer;
		}

		private void InitRendererLists(ref CullingResults cullResults, ref AdditionalLightsShadowCasterPass.PassData passData, ScriptableRenderContext context, RenderGraph renderGraph, bool useRenderGraph)
		{
			if (this.m_CreateEmptyShadowmap)
			{
				return;
			}
			for (int i = 0; i < this.m_ShadowSliceToAdditionalLightIndex.Count; i++)
			{
				int num = (int)this.m_ShadowSliceToAdditionalLightIndex[i];
				int lightIndex = (int)this.m_AdditionalLightIndexToVisibleLightIndex[num];
				ShadowDrawingSettings shadowDrawingSettings = new ShadowDrawingSettings(cullResults, lightIndex)
				{
					useRenderingLayerMaskTest = UniversalRenderPipeline.asset.useRenderingLayers
				};
				if (useRenderGraph)
				{
					passData.shadowRendererListsHdl[i] = renderGraph.CreateShadowRendererList(ref shadowDrawingSettings);
				}
				else
				{
					passData.shadowRendererLists[i] = context.CreateShadowRendererList(ref shadowDrawingSettings);
				}
			}
		}

		internal TextureHandle Render(RenderGraph graph, ContextContainer frameData)
		{
			UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
			UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
			UniversalLightData lightData = frameData.Get<UniversalLightData>();
			UniversalShadowData shadowData = frameData.Get<UniversalShadowData>();
			AdditionalLightsShadowCasterPass.PassData passData;
			TextureHandle result;
			using (IRasterRenderGraphBuilder rasterRenderGraphBuilder = graph.AddRasterRenderPass<AdditionalLightsShadowCasterPass.PassData>(base.passName, out passData, base.profilingSampler, ".\\Library\\PackageCache\\com.unity.render-pipelines.universal@bc6f352be672\\Runtime\\Passes\\AdditionalLightsShadowCasterPass.cs", 1003))
			{
				this.InitPassData(ref passData, cameraData, lightData, shadowData);
				this.InitRendererLists(ref universalRenderingData.cullResults, ref passData, default(ScriptableRenderContext), graph, true);
				TextureHandle textureHandle;
				if (!this.m_CreateEmptyShadowmap)
				{
					for (int i = 0; i < this.m_ShadowSliceToAdditionalLightIndex.Count; i++)
					{
						rasterRenderGraphBuilder.UseRendererList(passData.shadowRendererListsHdl[i]);
					}
					textureHandle = UniversalRenderer.CreateRenderGraphTexture(graph, this.m_AdditionalLightShadowDescriptor, "_AdditionalLightsShadowmapTexture", true, ShadowUtils.m_ForceShadowPointSampling ? FilterMode.Point : FilterMode.Bilinear, TextureWrapMode.Clamp);
					rasterRenderGraphBuilder.SetRenderAttachmentDepth(textureHandle, AccessFlags.Write);
				}
				else
				{
					textureHandle = graph.defaultResources.defaultShadowTexture;
				}
				TextureDesc descriptor = textureHandle.GetDescriptor(graph);
				passData.allocatedShadowAtlasSize = new Vector2Int(descriptor.width, descriptor.height);
				rasterRenderGraphBuilder.AllowGlobalStateModification(true);
				if (textureHandle.IsValid())
				{
					rasterRenderGraphBuilder.SetGlobalTextureAfterPass(textureHandle, AdditionalLightsShadowCasterPass.AdditionalShadowsConstantBuffer._AdditionalLightsShadowmapID);
				}
				rasterRenderGraphBuilder.SetRenderFunc<AdditionalLightsShadowCasterPass.PassData>(delegate(AdditionalLightsShadowCasterPass.PassData data, RasterGraphContext context)
				{
					RasterCommandBuffer cmd = context.cmd;
					if (!data.emptyShadowmap)
					{
						data.pass.RenderAdditionalShadowmapAtlas(cmd, ref data, true);
						return;
					}
					if (data.setKeywordForEmptyShadowmap)
					{
						cmd.EnableKeyword(ShaderGlobalKeywords.AdditionalLightShadows);
					}
					AdditionalLightsShadowCasterPass.SetShadowParamsForEmptyShadowmap(cmd);
				});
				result = textureHandle;
			}
			return result;
		}

		[Obsolete("AdditionalLightsShadowCasterPass.m_AdditionalShadowsBufferId was deprecated. Shadow slice matrix is now passed to the GPU using an entry in buffer m_AdditionalLightsWorldToShadow_SSBO", true)]
		public static int m_AdditionalShadowsBufferId;

		[Obsolete("AdditionalLightsShadowCasterPass.m_AdditionalShadowsIndicesId was deprecated. Shadow slice index is now passed to the GPU using last member of an entry in buffer m_AdditionalShadowParams_SSBO", true)]
		public static int m_AdditionalShadowsIndicesId;

		internal RTHandle m_AdditionalLightsShadowmapHandle;

		private int renderTargetWidth;

		private int renderTargetHeight;

		private bool m_CreateEmptyShadowmap;

		private bool m_SetKeywordForEmptyShadowmap;

		private bool m_EmptyShadowmapNeedsClear;

		private bool m_IssuedMessageAboutShadowSlicesTooMany;

		private bool m_IssuedMessageAboutShadowMapsRescale;

		private bool m_IssuedMessageAboutShadowMapsTooBig;

		private bool m_IssuedMessageAboutRemovedShadowSlices;

		private static bool m_IssuedMessageAboutPointLightHardShadowResolutionTooSmall;

		private static bool m_IssuedMessageAboutPointLightSoftShadowResolutionTooSmall;

		private readonly bool m_UseStructuredBuffer;

		private float m_MaxShadowDistanceSq;

		private float m_CascadeBorder;

		private AdditionalLightsShadowCasterPass.PassData m_PassData;

		private RTHandle m_EmptyAdditionalLightShadowmapTexture;

		private bool[] m_VisibleLightIndexToIsCastingShadows;

		private short[] m_VisibleLightIndexToAdditionalLightIndex;

		private short[] m_AdditionalLightIndexToVisibleLightIndex;

		private Vector4[] m_AdditionalLightIndexToShadowParams;

		private Matrix4x4[] m_AdditionalLightShadowSliceIndexTo_WorldShadowMatrix;

		private ShadowSliceData[] m_AdditionalLightsShadowSlices;

		private readonly List<byte> m_GlobalShadowSliceIndexToPerLightShadowSliceIndex = new List<byte>();

		private readonly List<short> m_ShadowSliceToAdditionalLightIndex = new List<short>();

		private readonly Dictionary<int, ulong> m_ShadowRequestsHashes = new Dictionary<int, ulong>();

		private readonly ProfilingSampler m_ProfilingSetupSampler = new ProfilingSampler("Setup Additional Shadows");

		private RenderTextureDescriptor m_AdditionalLightShadowDescriptor;

		private const int k_ShadowmapBufferBits = 16;

		private const int k_EmptyShadowMapDimensions = 1;

		private const float k_LightTypeIdentifierInShadowParams_Spot = 0f;

		private const float k_LightTypeIdentifierInShadowParams_Point = 1f;

		private const string k_AdditionalLightShadowMapTextureName = "_AdditionalLightsShadowmapTexture";

		private const string k_EmptyAdditionalLightShadowMapTextureName = "_EmptyAdditionalLightShadowmapTexture";

		private static readonly Vector4 c_DefaultShadowParams = new Vector4(0f, 0f, 0f, -1f);

		private static Vector4 s_EmptyAdditionalShadowFadeParams;

		private static Vector4[] s_EmptyAdditionalLightIndexToShadowParams;

		private static bool isAdditionalShadowParamsDirty;

		private static class AdditionalShadowsConstantBuffer
		{
			public static readonly int _AdditionalLightsWorldToShadow = Shader.PropertyToID("_AdditionalLightsWorldToShadow");

			public static readonly int _AdditionalShadowParams = Shader.PropertyToID("_AdditionalShadowParams");

			public static readonly int _AdditionalShadowOffset0 = Shader.PropertyToID("_AdditionalShadowOffset0");

			public static readonly int _AdditionalShadowOffset1 = Shader.PropertyToID("_AdditionalShadowOffset1");

			public static readonly int _AdditionalShadowFadeParams = Shader.PropertyToID("_AdditionalShadowFadeParams");

			public static readonly int _AdditionalShadowmapSize = Shader.PropertyToID("_AdditionalShadowmapSize");

			public static readonly int _AdditionalLightsShadowmapID = Shader.PropertyToID("_AdditionalLightsShadowmapTexture");

			public static readonly int _AdditionalLightsWorldToShadow_SSBO = Shader.PropertyToID("_AdditionalLightsWorldToShadow_SSBO");

			public static readonly int _AdditionalShadowParams_SSBO = Shader.PropertyToID("_AdditionalShadowParams_SSBO");
		}

		private class PassData
		{
			internal int shadowmapID;

			internal bool emptyShadowmap;

			internal bool setKeywordForEmptyShadowmap;

			internal bool useStructuredBuffer;

			internal bool stripShadowsOffVariants;

			internal Matrix4x4 viewMatrix;

			internal Vector2Int allocatedShadowAtlasSize;

			internal TextureHandle shadowmapTexture;

			internal UniversalLightData lightData;

			internal UniversalShadowData shadowData;

			internal AdditionalLightsShadowCasterPass pass;

			internal readonly RendererList[] shadowRendererLists = new RendererList[256];

			internal readonly RendererListHandle[] shadowRendererListsHdl = new RendererListHandle[256];
		}
	}
}
