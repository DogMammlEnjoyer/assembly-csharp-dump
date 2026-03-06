using System;
using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	internal struct AdditionalLightsShadowAtlasLayout
	{
		public AdditionalLightsShadowAtlasLayout(UniversalLightData lightData, UniversalShadowData shadowData, UniversalCameraData cameraData)
		{
			bool useStructuredBuffer = RenderingUtils.useStructuredBuffer;
			NativeArray<VisibleLight> visibleLights = lightData.visibleLights;
			int length = visibleLights.Length;
			if (AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas == null)
			{
				AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas = new List<RectInt>();
			}
			if (AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests == null)
			{
				AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests = new List<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest>();
			}
			if (AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance == null || AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance.Length < length)
			{
				AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance = new float[length];
			}
			if (AdditionalLightsShadowAtlasLayout.s_CompareShadowResolutionRequest == null)
			{
				AdditionalLightsShadowAtlasLayout.s_CompareShadowResolutionRequest = AdditionalLightsShadowAtlasLayout.CreateCompareShadowResolutionRequesPredicate();
			}
			if (!useStructuredBuffer)
			{
				int maxVisibleAdditionalLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
				if (AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Capacity < maxVisibleAdditionalLights)
				{
					AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Capacity = maxVisibleAdditionalLights;
				}
				if (AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests.Count < length)
				{
					AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests.Capacity = length;
					int num = length - AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests.Count + 1;
					for (int i = 0; i < num; i++)
					{
						AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests.Add(default(AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest));
					}
				}
			}
			AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Clear();
			ushort num2 = 0;
			for (int j = 0; j < visibleLights.Length; j++)
			{
				if (j == lightData.mainLightIndex)
				{
					AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[j] = float.MaxValue;
				}
				else
				{
					ref VisibleLight ptr = ref visibleLights.UnsafeElementAt(j);
					Light light = ptr.light;
					LightType lightType = ptr.lightType;
					LightShadows shadows = light.shadows;
					float shadowStrength = light.shadowStrength;
					if (!ShadowUtils.IsValidShadowCastingLight(lightData, j, lightType, shadows, shadowStrength))
					{
						AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[j] = float.MaxValue;
					}
					else
					{
						bool softShadow = shadows == LightShadows.Soft;
						bool pointLightShadow = lightType == LightType.Point;
						ushort visibleLightIndex = (ushort)j;
						ushort requestedResolution = (ushort)shadowData.resolution[j];
						int punctualLightShadowSlicesCount = ShadowUtils.GetPunctualLightShadowSlicesCount(lightType);
						ushort num3 = 0;
						while ((int)num3 < punctualLightShadowSlicesCount)
						{
							if ((int)num2 >= AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests.Count)
							{
								AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests.Add(default(AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest));
							}
							AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest value = AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests[(int)num2];
							value.visibleLightIndex = visibleLightIndex;
							value.perLightShadowSliceIndex = num3;
							value.requestedResolution = requestedResolution;
							value.softShadow = softShadow;
							value.pointLightShadow = pointLightShadow;
							AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests[(int)num2] = value;
							num2 += 1;
							num3 += 1;
						}
						AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[j] = (cameraData.worldSpaceCameraPos - light.transform.position).sqrMagnitude;
					}
				}
			}
			if (AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests == null || AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests.Length < (int)num2)
			{
				AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests = new AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest[(int)num2];
			}
			for (int k = 0; k < (int)num2; k++)
			{
				AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests[k] = AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests[k];
			}
			using (new ProfilingScope(Sorting.s_QuickSortSampler))
			{
				Sorting.QuickSort<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest>(AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests, 0, (int)(num2 - 1), AdditionalLightsShadowAtlasLayout.s_CompareShadowResolutionRequest);
			}
			this.m_SortedShadowResolutionRequests = new NativeArray<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest>(AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests, Allocator.Temp);
			int num4 = useStructuredBuffer ? ((int)num2) : Math.Min((int)num2, UniversalRenderPipeline.maxVisibleAdditionalLights);
			int additionalLightsShadowmapWidth = shadowData.additionalLightsShadowmapWidth;
			bool flag = false;
			int num5 = 1;
			while (!flag && num4 > 0)
			{
				AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest shadowResolutionRequest = this.m_SortedShadowResolutionRequests[num4 - 1];
				num5 = AdditionalLightsShadowAtlasLayout.EstimateScaleFactorNeededToFitAllShadowsInAtlas(this.m_SortedShadowResolutionRequests, num4, additionalLightsShadowmapWidth);
				if ((int)shadowResolutionRequest.requestedResolution >= num5 * ShadowUtils.MinimalPunctualLightShadowResolution(shadowResolutionRequest.softShadow))
				{
					flag = true;
				}
				else
				{
					int num6 = num4;
					LightType lightType2 = shadowResolutionRequest.pointLightShadow ? LightType.Point : LightType.Spot;
					num4 = num6 - ShadowUtils.GetPunctualLightShadowSlicesCount(lightType2);
				}
			}
			for (int l = num4; l < this.m_SortedShadowResolutionRequests.Length; l++)
			{
				this.m_SortedShadowResolutionRequests[l] = default(AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest);
			}
			this.m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex = new NativeArray<int>(visibleLights.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int m = 0; m < this.m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex.Length; m++)
			{
				this.m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex[m] = -1;
			}
			for (int n = num4 - 1; n >= 0; n--)
			{
				int visibleLightIndex2 = (int)AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests[n].visibleLightIndex;
				this.m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex[visibleLightIndex2] = n;
			}
			bool flag2 = false;
			bool flag3 = false;
			int num7 = num5;
			while (!flag2 && !flag3)
			{
				AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Clear();
				AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Add(new RectInt(0, 0, additionalLightsShadowmapWidth, additionalLightsShadowmapWidth));
				flag2 = true;
				for (int num8 = 0; num8 < num4; num8++)
				{
					int num9 = (int)this.m_SortedShadowResolutionRequests[num8].requestedResolution / num7;
					if (num9 < ShadowUtils.MinimalPunctualLightShadowResolution(this.m_SortedShadowResolutionRequests[num8].softShadow))
					{
						flag3 = true;
						break;
					}
					bool flag4 = false;
					for (int num10 = 0; num10 < AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Count; num10++)
					{
						RectInt rectInt = AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas[num10];
						int width = rectInt.width;
						if (width >= num9)
						{
							int height = rectInt.height;
							int x = rectInt.x;
							int y = rectInt.y;
							ref AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest ptr2 = ref this.m_SortedShadowResolutionRequests.UnsafeElementAtMutable(num8);
							ptr2.offsetX = (ushort)x;
							ptr2.offsetY = (ushort)y;
							ptr2.allocatedResolution = (ushort)num9;
							AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.RemoveAt(num10);
							int num11 = num4 - num8 - 1;
							int num12 = 0;
							int num13 = num9;
							int num14 = num9;
							int num15 = x;
							int num16 = y;
							while (num12 < num11)
							{
								num15 += num13;
								if (num15 + num13 > x + width)
								{
									num15 = x;
									num16 += num14;
									if (num16 + num14 > y + height)
									{
										break;
									}
								}
								AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas.Insert(num10 + num12, new RectInt(num15, num16, num13, num14));
								num12++;
							}
							flag4 = true;
							break;
						}
					}
					if (!flag4)
					{
						flag2 = false;
						break;
					}
				}
				if (!flag2 && !flag3)
				{
					num7 *= 2;
				}
			}
			this.m_TooManyShadowMaps = flag3;
			this.m_ShadowSlicesScaleFactor = num7;
			this.m_TotalShadowSlicesCount = num4;
			this.m_TotalShadowResolutionRequestCount = (int)num2;
			this.m_AtlasSize = additionalLightsShadowmapWidth;
		}

		public int GetTotalShadowSlicesCount()
		{
			return this.m_TotalShadowSlicesCount;
		}

		public int GetTotalShadowResolutionRequestCount()
		{
			return this.m_TotalShadowResolutionRequestCount;
		}

		public bool HasTooManyShadowMaps()
		{
			return this.m_TooManyShadowMaps;
		}

		public int GetShadowSlicesScaleFactor()
		{
			return this.m_ShadowSlicesScaleFactor;
		}

		public int GetAtlasSize()
		{
			return this.m_AtlasSize;
		}

		public bool HasSpaceForLight(int originalVisibleLightIndex)
		{
			return this.m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex[originalVisibleLightIndex] != -1;
		}

		public AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest GetSortedShadowResolutionRequest(int sortedShadowResolutionRequestIndex)
		{
			return this.m_SortedShadowResolutionRequests[sortedShadowResolutionRequestIndex];
		}

		public AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest GetSliceShadowResolutionRequest(int originalVisibleLightIndex, int sliceIndex)
		{
			int num = this.m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex[originalVisibleLightIndex];
			return this.m_SortedShadowResolutionRequests[num + sliceIndex];
		}

		public static void ClearStaticCaches()
		{
			AdditionalLightsShadowAtlasLayout.s_UnusedAtlasSquareAreas = null;
			AdditionalLightsShadowAtlasLayout.s_ShadowResolutionRequests = null;
			AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance = null;
			AdditionalLightsShadowAtlasLayout.s_CompareShadowResolutionRequest = null;
			AdditionalLightsShadowAtlasLayout.s_SortedShadowResolutionRequests = null;
		}

		private static int EstimateScaleFactorNeededToFitAllShadowsInAtlas(in NativeArray<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest> shadowResolutionRequests, int endIndex, int atlasSize)
		{
			long num = (long)(atlasSize * atlasSize);
			long num2 = 0L;
			for (int i = 0; i < endIndex; i++)
			{
				long num3 = num2;
				NativeArray<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest> nativeArray = shadowResolutionRequests;
				ushort requestedResolution = nativeArray[i].requestedResolution;
				nativeArray = shadowResolutionRequests;
				num2 = num3 + (long)(requestedResolution * nativeArray[i].requestedResolution);
			}
			int num4 = 1;
			while (num2 > num * (long)num4 * (long)num4)
			{
				num4 *= 2;
			}
			return num4;
		}

		private static Func<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest, AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest, int> CreateCompareShadowResolutionRequesPredicate()
		{
			return delegate(AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest curr, AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest other)
			{
				if (curr.requestedResolution <= other.requestedResolution && (curr.requestedResolution != other.requestedResolution || curr.softShadow || !other.softShadow) && (curr.requestedResolution != other.requestedResolution || curr.softShadow != other.softShadow || AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[(int)curr.visibleLightIndex] >= AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[(int)other.visibleLightIndex]) && (curr.requestedResolution != other.requestedResolution || curr.softShadow != other.softShadow || AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[(int)curr.visibleLightIndex] != AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[(int)other.visibleLightIndex] || curr.visibleLightIndex >= other.visibleLightIndex) && (curr.requestedResolution != other.requestedResolution || curr.softShadow != other.softShadow || AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[(int)curr.visibleLightIndex] != AdditionalLightsShadowAtlasLayout.s_VisibleLightIndexToCameraSquareDistance[(int)other.visibleLightIndex] || curr.visibleLightIndex != other.visibleLightIndex || curr.perLightShadowSliceIndex >= other.perLightShadowSliceIndex))
				{
					return 1;
				}
				return -1;
			};
		}

		private static List<RectInt> s_UnusedAtlasSquareAreas;

		private static List<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest> s_ShadowResolutionRequests;

		private static float[] s_VisibleLightIndexToCameraSquareDistance;

		private static Func<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest, AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest, int> s_CompareShadowResolutionRequest;

		private static AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest[] s_SortedShadowResolutionRequests;

		private NativeArray<AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest> m_SortedShadowResolutionRequests;

		private NativeArray<int> m_VisibleLightIndexToSortedShadowResolutionRequestsFirstSliceIndex;

		private int m_TotalShadowSlicesCount;

		private int m_TotalShadowResolutionRequestCount;

		private bool m_TooManyShadowMaps;

		private int m_ShadowSlicesScaleFactor;

		private int m_AtlasSize;

		internal struct ShadowResolutionRequest
		{
			public bool softShadow
			{
				get
				{
					return this.m_ShadowProperties.HasFlag(AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions.SoftShadow);
				}
				set
				{
					if (value)
					{
						this.m_ShadowProperties |= AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions.SoftShadow;
						return;
					}
					this.m_ShadowProperties &= ~AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions.SoftShadow;
				}
			}

			public bool pointLightShadow
			{
				get
				{
					return this.m_ShadowProperties.HasFlag(AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions.PointLightShadow);
				}
				set
				{
					if (value)
					{
						this.m_ShadowProperties |= AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions.PointLightShadow;
						return;
					}
					this.m_ShadowProperties &= ~AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions.PointLightShadow;
				}
			}

			public ushort visibleLightIndex;

			public ushort perLightShadowSliceIndex;

			public ushort requestedResolution;

			public ushort offsetX;

			public ushort offsetY;

			public ushort allocatedResolution;

			private AdditionalLightsShadowAtlasLayout.ShadowResolutionRequest.SettingsOptions m_ShadowProperties;

			[Flags]
			private enum SettingsOptions : ushort
			{
				None = 0,
				SoftShadow = 1,
				PointLightShadow = 2,
				All = 65535
			}
		}
	}
}
