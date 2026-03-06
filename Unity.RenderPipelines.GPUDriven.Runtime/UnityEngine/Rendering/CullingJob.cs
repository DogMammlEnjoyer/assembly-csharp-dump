using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	[BurstCompile]
	internal struct CullingJob : IJobParallelFor
	{
		private static uint PackFloatToUint8(float percent)
		{
			uint num = (uint)((1f + percent) * 127f + 0.5f);
			if (percent < 0f)
			{
				num = math.clamp(num, 0U, 126U);
			}
			else
			{
				num = math.clamp(num, 128U, 254U);
			}
			return num;
		}

		private unsafe uint CalculateLODVisibility(int instanceIndex, int sharedInstanceIndex, InstanceFlags instanceFlags)
		{
			uint num = this.sharedInstanceData.lodGroupAndMasks[sharedInstanceIndex];
			if (num == 4294967295U)
			{
				if (this.viewType >= BatchCullingViewType.SelectionOutline || (instanceFlags & InstanceFlags.SmallMeshCulling) == InstanceFlags.None || this.minScreenRelativeHeight == 0f)
				{
					return 255U;
				}
				ref readonly AABB ptr = ref this.instanceData.worldAABBs.UnsafeElementAt(instanceIndex);
				float num2 = math.sqrt(this.isOrtho ? this.sqrScreenRelativeMetric : LODRenderingUtils.CalculateSqrPerspectiveDistance(ptr.center, this.cameraPosition, this.sqrScreenRelativeMetric));
				float3 @float = ptr.extents * 2f;
				float size = math.max(math.max(@float.x, @float.y), @float.z);
				float num3 = LODRenderingUtils.CalculateLODDistance(this.minScreenRelativeHeight, size);
				if (num3 < num2)
				{
					return 127U;
				}
				float relativeScreenHeight = this.minScreenRelativeHeight + 0.1f * this.minScreenRelativeHeight;
				float num4 = Mathf.Max(0f, num3 - LODRenderingUtils.CalculateLODDistance(relativeScreenHeight, size));
				float num5 = (num3 - num2) / num4;
				if (num5 <= 1f)
				{
					return CullingJob.PackFloatToUint8(num5);
				}
				return 255U;
			}
			else
			{
				uint index = num >> 8;
				uint num6 = num & 255U;
				ref LODGroupCullingData ptr2 = ref this.lodGroupCullingData.ElementAt((int)index);
				if (ptr2.forceLODMask == 0)
				{
					float num7 = this.isOrtho ? this.sqrScreenRelativeMetric : LODRenderingUtils.CalculateSqrPerspectiveDistance(ptr2.worldSpaceReferencePoint, this.cameraPosition, this.sqrScreenRelativeMetric);
					uint num8 = uint.MaxValue << this.maxLOD;
					num6 &= num8;
					int num9 = math.max(math.tzcnt(num6) - 1, this.maxLOD);
					num6 >>= num9;
					while (num6 > 0U)
					{
						float num10 = (num9 == this.maxLOD) ? 0f : (*(ref ptr2.sqrDistances.FixedElementField + (IntPtr)(num9 - 1) * 4));
						float num11 = *(ref ptr2.sqrDistances.FixedElementField + (IntPtr)num9 * 4);
						if (num7 < num10)
						{
							break;
						}
						if (num7 > num11)
						{
							num9++;
							num6 >>= 1;
						}
						else
						{
							CullingJob.CrossFadeType crossFadeType = (CullingJob.CrossFadeType)(num6 & 3U);
							if (crossFadeType == CullingJob.CrossFadeType.kDisabled)
							{
								return 127U;
							}
							if (crossFadeType == CullingJob.CrossFadeType.kVisible)
							{
								return 255U;
							}
							float num12 = math.sqrt(num7);
							float num13 = math.sqrt(num11);
							if (*(ref ptr2.percentageFlags.FixedElementField + num9))
							{
								if (crossFadeType == CullingJob.CrossFadeType.kCrossFadeIn)
								{
									return 128U;
								}
								if (crossFadeType == CullingJob.CrossFadeType.kCrossFadeOut)
								{
									float num14 = (num9 > 0) ? math.sqrt(*(ref ptr2.sqrDistances.FixedElementField + (IntPtr)(num9 - 1) * 4)) : ptr2.worldSpaceSize;
									return CullingJob.PackFloatToUint8(math.max(num12 - num14, 0f) / (num13 - num14)) | 256U;
								}
							}
							else
							{
								float num15 = *(ref ptr2.transitionDistances.FixedElementField + (IntPtr)num9 * 4);
								float num16 = num13 - num12;
								if (num16 < num15)
								{
									float num17 = num16 / num15;
									if (crossFadeType == CullingJob.CrossFadeType.kCrossFadeIn)
									{
										num17 = -num17;
									}
									return CullingJob.PackFloatToUint8(num17);
								}
								if (crossFadeType != CullingJob.CrossFadeType.kCrossFadeOut)
								{
									return 127U;
								}
								return 255U;
							}
						}
					}
					return 127U;
				}
				if (((uint)ptr2.forceLODMask & num6) == 0U)
				{
					return 127U;
				}
				return 255U;
			}
		}

		private uint CalculateVisibilityMask(int instanceIndex, int sharedInstanceIndex, InstanceFlags instanceFlags)
		{
			if (this.cullingLayerMask == 0U)
			{
				return 0U;
			}
			if (((ulong)this.cullingLayerMask & (ulong)(1L << (this.sharedInstanceData.gameObjectLayers[sharedInstanceIndex] & 31))) == 0UL)
			{
				return 0U;
			}
			if (this.cullLightmappedShadowCasters && (instanceFlags & InstanceFlags.AffectsLightmaps) != InstanceFlags.None)
			{
				return 0U;
			}
			if (this.viewType == BatchCullingViewType.Camera && (instanceFlags & InstanceFlags.IsShadowsOnly) != InstanceFlags.None)
			{
				return 0U;
			}
			if (this.viewType == BatchCullingViewType.Light && (instanceFlags & InstanceFlags.IsShadowsOff) != InstanceFlags.None)
			{
				return 0U;
			}
			ref readonly AABB ptr = ref this.instanceData.worldAABBs.UnsafeElementAt(instanceIndex);
			uint num = FrustumPlaneCuller.ComputeSplitVisibilityMask(this.frustumPlanePackets, this.frustumSplitInfos, ptr);
			if (num != 0U && this.receiverSplitInfos.Length > 0)
			{
				num &= ReceiverSphereCuller.ComputeSplitVisibilityMask(this.lightFacingFrustumPlanes, this.receiverSplitInfos, this.worldToLightSpaceRotation, ptr);
			}
			if (num != 0U && this.occlusionBuffer != IntPtr.Zero)
			{
				num = (BatchRendererGroup.OcclusionTestAABB(this.occlusionBuffer, ptr.ToBounds()) ? num : 0U);
			}
			return num;
		}

		private uint ComputeMeshLODLevel(int instanceIndex, int sharedInstanceIndex)
		{
			ref readonly GPUDrivenRendererMeshLodData ptr = ref this.instanceData.meshLodData.UnsafeElementAt(instanceIndex);
			GPUDrivenMeshLodInfo gpudrivenMeshLodInfo = this.sharedInstanceData.meshLodInfos[sharedInstanceIndex];
			if (ptr.forceLod >= 0)
			{
				return (uint)math.clamp(ptr.forceLod, 0, gpudrivenMeshLodInfo.levelCount - 1);
			}
			ref readonly AABB ptr2 = ref this.instanceData.worldAABBs.UnsafeElementAt(instanceIndex);
			float num = math.max(math.lengthsq(ptr2.extents), 1E-05f) * 4f;
			return (uint)math.floor(math.clamp(math.max(math.log2(Math.Sqrt((double)((this.isOrtho ? this.sqrMeshLodSelectionConstant : LODRenderingUtils.CalculateSqrPerspectiveDistance(ptr2.center, this.cameraPosition, this.sqrMeshLodSelectionConstant)) / num))) * (double)gpudrivenMeshLodInfo.lodSlope + (double)gpudrivenMeshLodInfo.lodBias, 0.0) + (double)ptr.lodSelectionBias, 0.0, (double)(gpudrivenMeshLodInfo.levelCount - 1)));
		}

		private uint ComputeMeshLODCrossfade(int instanceIndex, ref uint meshLodLevel)
		{
			byte b = this.cameraInstanceData.meshLods[instanceIndex];
			if (b == 255)
			{
				this.cameraInstanceData.meshLods[instanceIndex] = (byte)meshLodLevel;
				return 255U;
			}
			byte b2 = this.cameraInstanceData.crossFades[instanceIndex];
			if (b2 == 255)
			{
				if ((uint)b == meshLodLevel)
				{
					return 255U;
				}
				this.cameraInstanceData.meshLods[instanceIndex] = (byte)meshLodLevel;
				this.cameraInstanceData.crossFades[instanceIndex] = ((meshLodLevel < (uint)b) ? 128 : 1);
				meshLodLevel = (uint)b;
				return 255U;
			}
			else
			{
				if ((long)(b2 - 1) % 127L == 0L)
				{
					meshLodLevel = (uint)b;
					return 255U;
				}
				meshLodLevel = (uint)(b | ((b2 > 127) ? 192 : 64));
				return (uint)b2;
			}
		}

		private void EnforcePreviousFrameMeshLOD(int instanceIndex, ref uint meshLodLevel)
		{
			ref byte ptr = ref this.cameraInstanceData.meshLods.ElementAt(instanceIndex);
			if (ptr != 255)
			{
				meshLodLevel = (uint)ptr;
			}
		}

		public void Execute(int instanceIndex)
		{
			InstanceHandle instance = this.instanceData.instances[instanceIndex];
			int num = this.sharedInstanceData.InstanceToIndex(this.instanceData, instance);
			InstanceFlags instanceFlags = this.sharedInstanceData.flags[num].instanceFlags;
			uint num2 = this.CalculateVisibilityMask(instanceIndex, num, instanceFlags);
			if (num2 == 0U)
			{
				this.rendererVisibilityMasks[instance.index] = 0;
				return;
			}
			uint num3 = this.CalculateLODVisibility(instanceIndex, num, instanceFlags);
			if (num3 == 127U)
			{
				this.rendererVisibilityMasks[instance.index] = 0;
				return;
			}
			if (this.binningConfig.supportsMotionCheck)
			{
				bool flag = this.instanceData.movedInPreviousFrameBits.Get(instanceIndex);
				num2 = (num2 << 1 | (flag ? 1U : 0U));
			}
			uint num4 = 0U;
			bool flag2 = (instanceFlags & InstanceFlags.HasMeshLod) > InstanceFlags.None;
			if (flag2)
			{
				num4 = this.ComputeMeshLODLevel(instanceIndex, num);
			}
			if (this.binningConfig.supportsCrossFade)
			{
				if (flag2 && this.animateCrossFades)
				{
					if (num3 == 255U)
					{
						num3 = this.ComputeMeshLODCrossfade(instanceIndex, ref num4);
					}
					else
					{
						this.EnforcePreviousFrameMeshLOD(instanceIndex, ref num4);
					}
				}
				num2 = (num2 << 1 | ((num3 < 255U) ? 1U : 0U));
			}
			this.rendererVisibilityMasks[instance.index] = (byte)num2;
			this.rendererMeshLodSettings[instance.index] = (byte)num4;
			this.rendererCrossFadeValues[instance.index] = (byte)(num3 & 255U);
		}

		public const int k_BatchSize = 32;

		public const uint k_MeshLodCrossfadeActive = 64U;

		public const uint k_MeshLodCrossfadeSignBit = 128U;

		public const uint k_MeshLodCrossfadeBits = 192U;

		public const uint k_LODFadeOff = 255U;

		public const uint k_LODFadeZeroPacked = 127U;

		public const uint k_LODFadeIsSpeedTree = 256U;

		private const uint k_InvalidCrossFadeAndLevel = 4294967295U;

		private const uint k_VisibilityMaskNotVisible = 0U;

		private const float k_SmallMeshTransitionWidth = 0.1f;

		[ReadOnly]
		public BinningConfig binningConfig;

		[ReadOnly]
		public BatchCullingViewType viewType;

		[ReadOnly]
		public float3 cameraPosition;

		[ReadOnly]
		public float sqrMeshLodSelectionConstant;

		[ReadOnly]
		public float sqrScreenRelativeMetric;

		[ReadOnly]
		public float minScreenRelativeHeight;

		[ReadOnly]
		public bool isOrtho;

		[ReadOnly]
		public bool cullLightmappedShadowCasters;

		[ReadOnly]
		public int maxLOD;

		[ReadOnly]
		public uint cullingLayerMask;

		[ReadOnly]
		public ulong sceneCullingMask;

		[ReadOnly]
		public bool animateCrossFades;

		[ReadOnly]
		public NativeArray<FrustumPlaneCuller.PlanePacket4> frustumPlanePackets;

		[ReadOnly]
		public NativeArray<FrustumPlaneCuller.SplitInfo> frustumSplitInfos;

		[ReadOnly]
		public NativeArray<Plane> lightFacingFrustumPlanes;

		[ReadOnly]
		public NativeArray<ReceiverSphereCuller.SplitInfo> receiverSplitInfos;

		public float3x3 worldToLightSpaceRotation;

		[ReadOnly]
		public CPUInstanceData.ReadOnly instanceData;

		[ReadOnly]
		public CPUSharedInstanceData.ReadOnly sharedInstanceData;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[ReadOnly]
		public NativeList<LODGroupCullingData> lodGroupCullingData;

		[NativeDisableUnsafePtrRestriction]
		[ReadOnly]
		public IntPtr occlusionBuffer;

		[NativeDisableContainerSafetyRestriction]
		public CPUPerCameraInstanceData.PerCameraInstanceDataArrays cameraInstanceData;

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<byte> rendererVisibilityMasks;

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<byte> rendererMeshLodSettings;

		[NativeDisableParallelForRestriction]
		[WriteOnly]
		public NativeArray<byte> rendererCrossFadeValues;

		private enum CrossFadeType
		{
			kDisabled,
			kCrossFadeOut,
			kCrossFadeIn,
			kVisible
		}
	}
}
