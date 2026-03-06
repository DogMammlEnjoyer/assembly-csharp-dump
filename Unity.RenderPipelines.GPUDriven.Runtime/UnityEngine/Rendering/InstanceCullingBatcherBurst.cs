using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	[BurstCompile]
	internal static class InstanceCullingBatcherBurst
	{
		private static void RemoveDrawRange(in RangeKey key, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeList<DrawRange> drawRanges)
		{
			int num = rangeHash[key];
			ref DrawRange ptr = ref drawRanges.ElementAt(drawRanges.Length - 1);
			rangeHash[ptr.key] = num;
			rangeHash.Remove(key);
			drawRanges.RemoveAtSwapBack(num);
		}

		private static void RemoveDrawBatch(in DrawKey key, ref NativeList<DrawRange> drawRanges, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawBatch> drawBatches)
		{
			int num = batchHash[key];
			int index = rangeHash[key.range];
			ref DrawRange ptr = ref drawRanges.ElementAt(index);
			ref DrawRange ptr2 = ref ptr;
			int num2 = ptr2.drawCount - 1;
			ptr2.drawCount = num2;
			if (num2 == 0)
			{
				InstanceCullingBatcherBurst.RemoveDrawRange(ptr.key, ref rangeHash, ref drawRanges);
			}
			ref DrawBatch ptr3 = ref drawBatches.ElementAt(drawBatches.Length - 1);
			batchHash[ptr3.key] = num;
			batchHash.Remove(key);
			drawBatches.RemoveAtSwapBack(num);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$PostfixBurstDelegate))]
		public static void RemoveDrawInstanceIndices(in NativeArray<int> drawInstanceIndices, ref NativeList<DrawInstance> drawInstances, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawRange> drawRanges, ref NativeList<DrawBatch> drawBatches)
		{
			InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$BurstDirectCall.Invoke(drawInstanceIndices, ref drawInstances, ref rangeHash, ref batchHash, ref drawRanges, ref drawBatches);
		}

		private static ref DrawRange EditDrawRange(in RangeKey key, NativeParallelHashMap<RangeKey, int> rangeHash, NativeList<DrawRange> drawRanges)
		{
			int length;
			if (!rangeHash.TryGetValue(key, out length))
			{
				DrawRange drawRange = new DrawRange
				{
					key = key,
					drawCount = 0,
					drawOffset = 0
				};
				length = drawRanges.Length;
				rangeHash.Add(key, length);
				drawRanges.Add(drawRange);
			}
			return drawRanges.ElementAt(length);
		}

		private static ref DrawBatch EditDrawBatch(in DrawKey key, in SubMeshDescriptor subMeshDescriptor, NativeParallelHashMap<DrawKey, int> batchHash, NativeList<DrawBatch> drawBatches)
		{
			MeshProceduralInfo procInfo = default(MeshProceduralInfo);
			procInfo.topology = subMeshDescriptor.topology;
			procInfo.baseVertex = (uint)subMeshDescriptor.baseVertex;
			procInfo.firstIndex = (uint)subMeshDescriptor.indexStart;
			procInfo.indexCount = (uint)subMeshDescriptor.indexCount;
			int length;
			if (!batchHash.TryGetValue(key, out length))
			{
				DrawBatch drawBatch = new DrawBatch
				{
					key = key,
					instanceCount = 0,
					instanceOffset = 0,
					procInfo = procInfo
				};
				length = drawBatches.Length;
				batchHash.Add(key, length);
				drawBatches.Add(drawBatch);
			}
			return drawBatches.ElementAt(length);
		}

		private unsafe static void ProcessRenderer(int i, bool implicitInstanceIndices, in GPUDrivenRendererGroupData rendererData, NativeParallelHashMap<int, BatchMeshID> batchMeshHash, NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialDataHash, NativeParallelHashMap<int, BatchMaterialID> batchMaterialHash, NativeArray<InstanceHandle> instances, NativeList<DrawInstance> drawInstances, NativeParallelHashMap<RangeKey, int> rangeHash, NativeList<DrawRange> drawRanges, NativeParallelHashMap<DrawKey, int> batchHash, NativeList<DrawBatch> drawBatches)
		{
			NativeArray<int> nativeArray = rendererData.meshIndex;
			int index = nativeArray[i];
			nativeArray = rendererData.meshID;
			int key = nativeArray[index];
			NativeArray<GPUDrivenMeshLodInfo> meshLodInfo = rendererData.meshLodInfo;
			GPUDrivenMeshLodInfo gpudrivenMeshLodInfo = meshLodInfo[index];
			NativeArray<short> nativeArray2 = rendererData.subMeshCount;
			short num = nativeArray2[index];
			nativeArray = rendererData.subMeshDescOffset;
			int num2 = nativeArray[index];
			BatchMeshID meshID = batchMeshHash[key];
			nativeArray = rendererData.rendererGroupID;
			int num3 = nativeArray[i];
			nativeArray2 = rendererData.subMeshStartIndex;
			short num4 = nativeArray2[i];
			nativeArray = rendererData.gameObjectLayer;
			int num5 = nativeArray[i];
			NativeArray<uint> renderingLayerMask = rendererData.renderingLayerMask;
			uint renderingLayerMask2 = renderingLayerMask[i];
			nativeArray = rendererData.materialsOffset;
			int num6 = nativeArray[i];
			nativeArray2 = rendererData.materialsCount;
			short num7 = nativeArray2[i];
			nativeArray = rendererData.lightmapIndex;
			int num8 = nativeArray[i];
			NativeArray<GPUDrivenPackedRendererData> packedRendererData = rendererData.packedRendererData;
			GPUDrivenPackedRendererData gpudrivenPackedRendererData = packedRendererData[i];
			nativeArray = rendererData.rendererPriority;
			int rendererPriority = nativeArray[i];
			int num9;
			int num10;
			if (implicitInstanceIndices)
			{
				num9 = 1;
				num10 = i;
			}
			else
			{
				nativeArray = rendererData.instancesCount;
				num9 = nativeArray[i];
				nativeArray = rendererData.instancesOffset;
				num10 = nativeArray[i];
			}
			if (num9 == 0)
			{
				return;
			}
			InstanceComponentGroup instanceComponentGroup = InstanceComponentGroup.Default;
			if (gpudrivenPackedRendererData.hasTree)
			{
				instanceComponentGroup |= InstanceComponentGroup.Wind;
			}
			if ((num8 & 65535) >= 65534)
			{
				if (gpudrivenPackedRendererData.lightProbeUsage == LightProbeUsage.BlendProbes)
				{
					instanceComponentGroup |= InstanceComponentGroup.LightProbe;
				}
			}
			else
			{
				instanceComponentGroup |= InstanceComponentGroup.Lightmap;
			}
			int num11 = (int)num7;
			Span<GPUDrivenPackedMaterialData> span = new Span<GPUDrivenPackedMaterialData>(stackalloc byte[checked(unchecked((UIntPtr)num11) * (UIntPtr)sizeof(GPUDrivenPackedMaterialData))], num11);
			bool flag = true;
			for (int j = 0; j < (int)num7; j++)
			{
				if (j >= (int)num)
				{
					Debug.LogWarning("Material count in the shared material list is higher than sub mesh count for the mesh. Object may be corrupted.");
				}
				else
				{
					nativeArray = rendererData.materialIndex;
					int index2 = nativeArray[num6 + j];
					NativeArray<GPUDrivenPackedMaterialData> packedMaterialData = rendererData.packedMaterialData;
					GPUDrivenPackedMaterialData gpudrivenPackedMaterialData;
					if (packedMaterialData.Length > 0)
					{
						packedMaterialData = rendererData.packedMaterialData;
						gpudrivenPackedMaterialData = packedMaterialData[index2];
					}
					else
					{
						nativeArray = rendererData.materialID;
						int key2 = nativeArray[index2];
						packedMaterialDataHash.TryGetValue(key2, out gpudrivenPackedMaterialData);
					}
					flag &= gpudrivenPackedMaterialData.isIndirectSupported;
					*span[j] = gpudrivenPackedMaterialData;
				}
			}
			RangeKey range = new RangeKey
			{
				layer = (byte)num5,
				renderingLayerMask = renderingLayerMask2,
				motionMode = gpudrivenPackedRendererData.motionVecGenMode,
				shadowCastingMode = gpudrivenPackedRendererData.shadowCastingMode,
				staticShadowCaster = gpudrivenPackedRendererData.staticShadowCaster,
				rendererPriority = rendererPriority,
				supportsIndirect = flag
			};
			ref DrawRange ptr = ref InstanceCullingBatcherBurst.EditDrawRange(range, rangeHash, drawRanges);
			for (int k = 0; k < (int)num7; k++)
			{
				if (k >= (int)num)
				{
					Debug.LogWarning("Material count in the shared material list is higher than sub mesh count for the mesh. Object may be corrupted.");
				}
				else
				{
					nativeArray = rendererData.materialIndex;
					int index3 = nativeArray[num6 + k];
					nativeArray = rendererData.materialID;
					int num12 = nativeArray[index3];
					GPUDrivenPackedMaterialData gpudrivenPackedMaterialData2 = *span[k];
					if (num12 == 0)
					{
						Debug.LogWarning("Material in the shared materials list is null. Object will be partially rendered.");
					}
					else
					{
						BatchMaterialID materialID;
						batchMaterialHash.TryGetValue(num12, out materialID);
						BatchDrawCommandFlags batchDrawCommandFlags = BatchDrawCommandFlags.LODCrossFadeValuePacked;
						batchDrawCommandFlags |= BatchDrawCommandFlags.UseLegacyLightmapsKeyword;
						if (gpudrivenPackedMaterialData2.isMotionVectorsPassEnabled)
						{
							batchDrawCommandFlags |= BatchDrawCommandFlags.HasMotion;
						}
						if (gpudrivenPackedMaterialData2.isTransparent)
						{
							batchDrawCommandFlags |= BatchDrawCommandFlags.HasSortingPosition;
						}
						if (gpudrivenPackedMaterialData2.supportsCrossFade)
						{
							batchDrawCommandFlags |= BatchDrawCommandFlags.LODCrossFadeKeyword;
						}
						int num13 = math.max(gpudrivenMeshLodInfo.levelCount, 1);
						for (int l = 0; l < num13; l++)
						{
							int num14 = (int)num4 + k;
							NativeArray<SubMeshDescriptor> subMeshDesc = rendererData.subMeshDesc;
							SubMeshDescriptor subMeshDescriptor = subMeshDesc[num2 + num14 * num13 + l];
							DrawKey key3 = new DrawKey
							{
								materialID = materialID,
								meshID = meshID,
								submeshIndex = num14,
								activeMeshLod = (gpudrivenMeshLodInfo.lodSelectionActive ? l : -1),
								flags = batchDrawCommandFlags,
								transparentInstanceId = (gpudrivenPackedMaterialData2.isTransparent ? num3 : 0),
								range = range,
								overridenComponents = (uint)instanceComponentGroup,
								lightmapIndex = num8
							};
							ref DrawBatch ptr2 = ref InstanceCullingBatcherBurst.EditDrawBatch(key3, subMeshDescriptor, batchHash, drawBatches);
							if (ptr2.instanceCount == 0)
							{
								ptr.drawCount++;
							}
							ptr2.instanceCount += num9;
							for (int m = 0; m < num9; m++)
							{
								int index4 = num10 + m;
								InstanceHandle instanceHandle = instances[index4];
								DrawInstance drawInstance = default(DrawInstance);
								drawInstance.key = key3;
								drawInstance.instanceIndex = instanceHandle.index;
								drawInstances.Add(drawInstance);
							}
						}
					}
				}
			}
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MonoPInvokeCallback(typeof(InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$PostfixBurstDelegate))]
		public static void CreateDrawBatches(bool implicitInstanceIndices, in NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData, in NativeParallelHashMap<int, BatchMeshID> batchMeshHash, in NativeParallelHashMap<int, BatchMaterialID> batchMaterialHash, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialDataHash, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeList<DrawRange> drawRanges, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawBatch> drawBatches, ref NativeList<DrawInstance> drawInstances)
		{
			InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$BurstDirectCall.Invoke(implicitInstanceIndices, instances, rendererData, batchMeshHash, batchMaterialHash, packedMaterialDataHash, ref rangeHash, ref drawRanges, ref batchHash, ref drawBatches, ref drawInstances);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void RemoveDrawInstanceIndices$BurstManaged(in NativeArray<int> drawInstanceIndices, ref NativeList<DrawInstance> drawInstances, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawRange> drawRanges, ref NativeList<DrawBatch> drawBatches)
		{
			DrawInstance* unsafePtr = drawInstances.GetUnsafePtr<DrawInstance>();
			int num = drawInstances.Length - 1;
			NativeArray<int> nativeArray = drawInstanceIndices;
			for (int i = nativeArray.Length - 1; i >= 0; i--)
			{
				nativeArray = drawInstanceIndices;
				int num2 = nativeArray[i];
				DrawInstance* ptr = unsafePtr + num2;
				int index = batchHash[ptr->key];
				ref DrawBatch ptr2 = ref drawBatches.ElementAt(index);
				ref DrawBatch ptr3 = ref ptr2;
				int num3 = ptr3.instanceCount - 1;
				ptr3.instanceCount = num3;
				if (num3 == 0)
				{
					InstanceCullingBatcherBurst.RemoveDrawBatch(ptr2.key, ref drawRanges, ref rangeHash, ref batchHash, ref drawBatches);
				}
				void* destination = (void*)ptr;
				DrawInstance* ptr4 = unsafePtr;
				IntPtr intPtr = (IntPtr)(num--);
				UnsafeUtility.MemCpy(destination, (void*)((byte*)ptr4 + intPtr * (IntPtr)sizeof(DrawInstance)), (long)sizeof(DrawInstance));
			}
			drawInstances.ResizeUninitialized(num + 1);
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void CreateDrawBatches$BurstManaged(bool implicitInstanceIndices, in NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData, in NativeParallelHashMap<int, BatchMeshID> batchMeshHash, in NativeParallelHashMap<int, BatchMaterialID> batchMaterialHash, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialDataHash, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeList<DrawRange> drawRanges, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawBatch> drawBatches, ref NativeList<DrawInstance> drawInstances)
		{
			int num = 0;
			for (;;)
			{
				int num2 = num;
				NativeArray<int> rendererGroupID = rendererData.rendererGroupID;
				if (num2 >= rendererGroupID.Length)
				{
					break;
				}
				InstanceCullingBatcherBurst.ProcessRenderer(num, implicitInstanceIndices, rendererData, batchMeshHash, packedMaterialDataHash, batchMaterialHash, instances, drawInstances, rangeHash, drawRanges, batchHash, drawBatches);
				num++;
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void RemoveDrawInstanceIndices_00000188$PostfixBurstDelegate(in NativeArray<int> drawInstanceIndices, ref NativeList<DrawInstance> drawInstances, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawRange> drawRanges, ref NativeList<DrawBatch> drawBatches);

		internal static class RemoveDrawInstanceIndices_00000188$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$BurstDirectCall.Pointer == 0)
				{
					InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$PostfixBurstDelegate>(new InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$PostfixBurstDelegate(InstanceCullingBatcherBurst.RemoveDrawInstanceIndices)).Value;
				}
				A_0 = InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(in NativeArray<int> drawInstanceIndices, ref NativeList<DrawInstance> drawInstances, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawRange> drawRanges, ref NativeList<DrawBatch> drawBatches)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InstanceCullingBatcherBurst.RemoveDrawInstanceIndices_00000188$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(Unity.Collections.NativeArray`1<System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.DrawInstance>&,Unity.Collections.NativeParallelHashMap`2<UnityEngine.Rendering.RangeKey,System.Int32>&,Unity.Collections.NativeParallelHashMap`2<UnityEngine.Rendering.DrawKey,System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.DrawRange>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.DrawBatch>&), ref drawInstanceIndices, ref drawInstances, ref rangeHash, ref batchHash, ref drawRanges, ref drawBatches, functionPointer);
						return;
					}
				}
				InstanceCullingBatcherBurst.RemoveDrawInstanceIndices$BurstManaged(drawInstanceIndices, ref drawInstances, ref rangeHash, ref batchHash, ref drawRanges, ref drawBatches);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void CreateDrawBatches_0000018C$PostfixBurstDelegate(bool implicitInstanceIndices, in NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData, in NativeParallelHashMap<int, BatchMeshID> batchMeshHash, in NativeParallelHashMap<int, BatchMaterialID> batchMaterialHash, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialDataHash, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeList<DrawRange> drawRanges, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawBatch> drawBatches, ref NativeList<DrawInstance> drawInstances);

		internal static class CreateDrawBatches_0000018C$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$BurstDirectCall.Pointer == 0)
				{
					InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$PostfixBurstDelegate>(new InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$PostfixBurstDelegate(InstanceCullingBatcherBurst.CreateDrawBatches)).Value;
				}
				A_0 = InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr result = (IntPtr)0;
				InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$BurstDirectCall.GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Invoke(bool implicitInstanceIndices, in NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData, in NativeParallelHashMap<int, BatchMeshID> batchMeshHash, in NativeParallelHashMap<int, BatchMaterialID> batchMaterialHash, in NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialDataHash, ref NativeParallelHashMap<RangeKey, int> rangeHash, ref NativeList<DrawRange> drawRanges, ref NativeParallelHashMap<DrawKey, int> batchHash, ref NativeList<DrawBatch> drawBatches, ref NativeList<DrawInstance> drawInstances)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = InstanceCullingBatcherBurst.CreateDrawBatches_0000018C$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Boolean,Unity.Collections.NativeArray`1<UnityEngine.Rendering.InstanceHandle>&,UnityEngine.Rendering.GPUDrivenRendererGroupData&,Unity.Collections.NativeParallelHashMap`2<System.Int32,UnityEngine.Rendering.BatchMeshID>&,Unity.Collections.NativeParallelHashMap`2<System.Int32,UnityEngine.Rendering.BatchMaterialID>&,Unity.Collections.NativeParallelHashMap`2<System.Int32,UnityEngine.Rendering.GPUDrivenPackedMaterialData>&,Unity.Collections.NativeParallelHashMap`2<UnityEngine.Rendering.RangeKey,System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.DrawRange>&,Unity.Collections.NativeParallelHashMap`2<UnityEngine.Rendering.DrawKey,System.Int32>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.DrawBatch>&,Unity.Collections.NativeList`1<UnityEngine.Rendering.DrawInstance>&), implicitInstanceIndices, ref instances, ref rendererData, ref batchMeshHash, ref batchMaterialHash, ref packedMaterialDataHash, ref rangeHash, ref drawRanges, ref batchHash, ref drawBatches, ref drawInstances, functionPointer);
						return;
					}
				}
				InstanceCullingBatcherBurst.CreateDrawBatches$BurstManaged(implicitInstanceIndices, instances, rendererData, batchMeshHash, batchMaterialHash, packedMaterialDataHash, ref rangeHash, ref drawRanges, ref batchHash, ref drawBatches, ref drawInstances);
			}

			private static IntPtr Pointer;
		}
	}
}
