using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Camera/GPUDrivenProcessor.h")]
	[RequiredByNativeCode]
	internal class GPUDrivenProcessor
	{
		internal List<Mesh> scratchMeshes { get; private set; }

		internal List<Material> scratchMaterials { get; private set; }

		public GPUDrivenProcessor()
		{
			this.m_Ptr = GPUDrivenProcessor.Internal_Create();
			this.scratchMeshes = new List<Mesh>();
			this.scratchMaterials = new List<Material>();
		}

		~GPUDrivenProcessor()
		{
			this.Destroy();
		}

		public void Dispose()
		{
			this.scratchMeshes = null;
			this.scratchMaterials = null;
			this.Destroy();
			GC.SuppressFinalize(this);
		}

		private void Destroy()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				GPUDrivenProcessor.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		private unsafe void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<int> renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param, bool materialUpdateOnly)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = renderersID;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				GPUDrivenProcessor.EnableGPUDrivenRenderingAndDispatchRendererData_Injected(intPtr, ref managedSpanWrapper, callback, meshes, materials, param, materialUpdateOnly);
			}
		}

		public void EnableGPUDrivenRenderingAndDispatchRendererData(ReadOnlySpan<int> renderersID, GPUDrivenRendererDataCallback callback, bool materialUpdateOnly = false)
		{
			this.scratchMeshes.Clear();
			this.scratchMaterials.Clear();
			this.EnableGPUDrivenRenderingAndDispatchRendererData(renderersID, GPUDrivenProcessor.s_NativeRendererCallback, this.scratchMeshes, this.scratchMaterials, callback, materialUpdateOnly);
		}

		public unsafe void DisableGPUDrivenRendering(ReadOnlySpan<int> renderersID)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = renderersID;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				GPUDrivenProcessor.DisableGPUDrivenRendering_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		private unsafe void DispatchLODGroupData(ReadOnlySpan<int> lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = lodGroupID;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				GPUDrivenProcessor.DispatchLODGroupData_Injected(intPtr, ref managedSpanWrapper, callback, param);
			}
		}

		public void DispatchLODGroupData(ReadOnlySpan<int> lodGroupID, GPUDrivenLODGroupDataCallback callback)
		{
			this.DispatchLODGroupData(lodGroupID, GPUDrivenProcessor.s_NativeLODGroupCallback, callback);
		}

		public bool enablePartialRendering
		{
			get
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GPUDrivenProcessor.get_enablePartialRendering_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GPUDrivenProcessor.set_enablePartialRendering_Injected(intPtr, value);
			}
		}

		public bool enableMaterialFilters
		{
			get
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GPUDrivenProcessor.get_enableMaterialFilters_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GPUDrivenProcessor.set_enableMaterialFilters_Injected(intPtr, value);
			}
		}

		public void AddMaterialFilters([NotNull] GPUDrivenMaterialFilterEntry[] filters)
		{
			if (filters == null)
			{
				ThrowHelper.ThrowArgumentNullException(filters, "filters");
			}
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GPUDrivenProcessor.AddMaterialFilters_Injected(intPtr, filters);
		}

		public void ClearMaterialFilters()
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GPUDrivenProcessor.ClearMaterialFilters_Injected(intPtr);
		}

		public int GetMaterialFilterFlags(Material material)
		{
			IntPtr intPtr = GPUDrivenProcessor.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GPUDrivenProcessor.GetMaterialFilterFlags_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		[FreeFunction("GPUDrivenProcessor::ClassifyMaterials", IsThreadSafe = true)]
		private unsafe static int ClassifyMaterialsImpl(ReadOnlySpan<EntityId> materialIDs, Span<EntityId> unsupportedMaterialIDs, Span<EntityId> supportedMaterialIDs, Span<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			ReadOnlySpan<EntityId> readOnlySpan = materialIDs;
			fixed (EntityId* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<EntityId> span = unsupportedMaterialIDs;
				fixed (EntityId* ptr2 = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span.Length);
					Span<EntityId> span2 = supportedMaterialIDs;
					fixed (EntityId* ptr3 = span2.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)ptr3, span2.Length);
						Span<GPUDrivenPackedMaterialData> span3 = supportedPackedMaterialDatas;
						int result;
						fixed (GPUDrivenPackedMaterialData* pinnableReference = span3.GetPinnableReference())
						{
							ManagedSpanWrapper managedSpanWrapper4 = new ManagedSpanWrapper((void*)pinnableReference, span3.Length);
							result = GPUDrivenProcessor.ClassifyMaterialsImpl_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, ref managedSpanWrapper3, ref managedSpanWrapper4);
							ptr = null;
							ptr2 = null;
							ptr3 = null;
						}
						return result;
					}
				}
			}
		}

		public static int ClassifyMaterials(NativeArray<EntityId> materialIDs, NativeArray<EntityId> unsupportedMaterialIDs, NativeArray<EntityId> supportedMaterialIDs, NativeArray<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			return GPUDrivenProcessor.ClassifyMaterialsImpl(materialIDs, unsupportedMaterialIDs, supportedMaterialIDs, supportedPackedMaterialDatas);
		}

		public static int ClassifyMaterials(NativeArray<int> materialIDs, NativeArray<int> unsupportedMaterialIDs, NativeArray<int> supportedMaterialIDs, NativeArray<GPUDrivenPackedMaterialData> supportedPackedMaterialDatas)
		{
			Assert.IsTrue(4 == sizeof(EntityId));
			NativeArray<EntityId> nativeArray = materialIDs.Reinterpret<EntityId>();
			ReadOnlySpan<EntityId> materialIDs2 = nativeArray;
			NativeArray<EntityId> nativeArray2 = unsupportedMaterialIDs.Reinterpret<EntityId>();
			Span<EntityId> unsupportedMaterialIDs2 = nativeArray2;
			NativeArray<EntityId> nativeArray3 = supportedMaterialIDs.Reinterpret<EntityId>();
			return GPUDrivenProcessor.ClassifyMaterialsImpl(materialIDs2, unsupportedMaterialIDs2, nativeArray3, supportedPackedMaterialDatas);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableGPUDrivenRenderingAndDispatchRendererData_Injected(IntPtr _unity_self, ref ManagedSpanWrapper renderersID, GPUDrivenRendererDataNativeCallback callback, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback param, bool materialUpdateOnly);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableGPUDrivenRendering_Injected(IntPtr _unity_self, ref ManagedSpanWrapper renderersID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DispatchLODGroupData_Injected(IntPtr _unity_self, ref ManagedSpanWrapper lodGroupID, GPUDrivenLODGroupDataNativeCallback callback, GPUDrivenLODGroupDataCallback param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enablePartialRendering_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enablePartialRendering_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableMaterialFilters_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableMaterialFilters_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddMaterialFilters_Injected(IntPtr _unity_self, GPUDrivenMaterialFilterEntry[] filters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearMaterialFilters_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMaterialFilterFlags_Injected(IntPtr _unity_self, IntPtr material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ClassifyMaterialsImpl_Injected(ref ManagedSpanWrapper materialIDs, ref ManagedSpanWrapper unsupportedMaterialIDs, ref ManagedSpanWrapper supportedMaterialIDs, ref ManagedSpanWrapper supportedPackedMaterialDatas);

		internal IntPtr m_Ptr;

		private static GPUDrivenRendererDataNativeCallback s_NativeRendererCallback = delegate(in GPUDrivenRendererGroupDataNative nativeData, List<Mesh> meshes, List<Material> materials, GPUDrivenRendererDataCallback callback)
		{
			NativeArray<int> rendererGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.rendererGroupID, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<Bounds> localBounds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Bounds>((void*)nativeData.localBounds, (nativeData.localBounds == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<Vector4> lightmapScaleOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>((void*)nativeData.lightmapScaleOffset, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> gameObjectLayer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.gameObjectLayer, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<uint> renderingLayerMask = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>((void*)nativeData.renderingLayerMask, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> lodGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.lodGroupID, (nativeData.lodGroupID == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> lightmapIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.motionVecGenMode, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<GPUDrivenPackedRendererData> packedRendererData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedRendererData>((void*)nativeData.packedRendererData, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> rendererPriority = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.rendererPriority, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> meshIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.meshIndex, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<short> subMeshStartIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.subMeshStartIndex, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> materialsOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialsOffset, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<short> materialsCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.materialsCount, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> instancesOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
			NativeArray<int> instancesCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
			NativeArray<GPUDrivenRendererEditorData> editorData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenRendererEditorData>((void*)nativeData.editorData, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<GPUDrivenRendererMeshLodData> meshLodData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenRendererMeshLodData>((void*)nativeData.meshLodData, nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> invalidRendererGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.invalidRendererGroupID, nativeData.invalidRendererGroupIDCount, Allocator.Invalid);
			NativeArray<Matrix4x4> localToWorldMatrix = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)nativeData.localToWorldMatrix, (nativeData.localToWorldMatrix == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<Matrix4x4> prevLocalToWorldMatrix = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)nativeData.prevLocalToWorldMatrix, (nativeData.prevLocalToWorldMatrix == null) ? 0 : nativeData.rendererGroupCount, Allocator.Invalid);
			NativeArray<int> rendererGroupIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(null, 0, Allocator.Invalid);
			NativeArray<int> meshID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.meshID, nativeData.meshCount, Allocator.Invalid);
			NativeArray<GPUDrivenMeshLodInfo> meshLodInfo = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenMeshLodInfo>((void*)nativeData.meshLodInfo, nativeData.meshCount, Allocator.Invalid);
			NativeArray<short> subMeshCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.subMeshCount, nativeData.meshCount, Allocator.Invalid);
			NativeArray<int> subMeshDescOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.subMeshDescOffset, nativeData.meshCount, Allocator.Invalid);
			NativeArray<SubMeshDescriptor> subMeshDesc = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<SubMeshDescriptor>((void*)nativeData.subMeshDesc, nativeData.subMeshDescCount, Allocator.Invalid);
			NativeArray<int> materialIndex = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialIndex, nativeData.materialIndexCount, Allocator.Invalid);
			NativeArray<int> materialID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialID, nativeData.materialCount, Allocator.Invalid);
			NativeArray<GPUDrivenPackedMaterialData> packedMaterialData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<GPUDrivenPackedMaterialData>((void*)nativeData.packedMaterialData, (nativeData.packedMaterialData == null) ? 0 : nativeData.materialCount, Allocator.Invalid);
			NativeArray<int> materialFilterFlags = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.materialFilterFlags, (nativeData.packedMaterialData == null) ? 0 : nativeData.materialCount, Allocator.Invalid);
			GPUDrivenRendererGroupData gpudrivenRendererGroupData = new GPUDrivenRendererGroupData
			{
				rendererGroupID = rendererGroupID,
				localBounds = localBounds,
				lightmapScaleOffset = lightmapScaleOffset,
				gameObjectLayer = gameObjectLayer,
				renderingLayerMask = renderingLayerMask,
				lodGroupID = lodGroupID,
				lightmapIndex = lightmapIndex,
				packedRendererData = packedRendererData,
				rendererPriority = rendererPriority,
				meshIndex = meshIndex,
				subMeshStartIndex = subMeshStartIndex,
				materialsOffset = materialsOffset,
				materialsCount = materialsCount,
				instancesOffset = instancesOffset,
				instancesCount = instancesCount,
				editorData = editorData,
				invalidRendererGroupID = invalidRendererGroupID,
				meshLodData = meshLodData,
				localToWorldMatrix = localToWorldMatrix,
				prevLocalToWorldMatrix = prevLocalToWorldMatrix,
				rendererGroupIndex = rendererGroupIndex,
				meshID = meshID,
				meshLodInfo = meshLodInfo,
				subMeshCount = subMeshCount,
				subMeshDescOffset = subMeshDescOffset,
				subMeshDesc = subMeshDesc,
				materialIndex = materialIndex,
				materialID = materialID,
				packedMaterialData = packedMaterialData,
				materialFilterFlags = materialFilterFlags
			};
			callback(gpudrivenRendererGroupData, meshes, materials);
		};

		private static GPUDrivenLODGroupDataNativeCallback s_NativeLODGroupCallback = delegate(in GPUDrivenLODGroupDataNative nativeData, GPUDrivenLODGroupDataCallback callback)
		{
			NativeArray<int> lodGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.lodGroupID, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<int> lodOffset = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.lodOffset, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<int> lodCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.lodCount, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<LODFadeMode> fadeMode = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<LODFadeMode>((void*)nativeData.fadeMode, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<Vector3> worldSpaceReferencePoint = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>((void*)nativeData.worldSpaceReferencePoint, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<float> worldSpaceSize = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>((void*)nativeData.worldSpaceSize, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<short> renderersCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.renderersCount, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<bool> lastLODIsBillboard = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<bool>((void*)nativeData.lastLODIsBillboard, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<byte> forceLODMask = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)nativeData.forceLODMask, nativeData.lodGroupCount, Allocator.Invalid);
			NativeArray<int> invalidLODGroupID = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)nativeData.invalidLODGroupID, nativeData.invalidLODGroupCount, Allocator.Invalid);
			NativeArray<short> lodRenderersCount = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<short>((void*)nativeData.lodRenderersCount, nativeData.lodDataCount, Allocator.Invalid);
			NativeArray<float> lodScreenRelativeTransitionHeight = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>((void*)nativeData.lodScreenRelativeTransitionHeight, nativeData.lodDataCount, Allocator.Invalid);
			NativeArray<float> lodFadeTransitionWidth = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>((void*)nativeData.lodFadeTransitionWidth, nativeData.lodDataCount, Allocator.Invalid);
			GPUDrivenLODGroupData gpudrivenLODGroupData = new GPUDrivenLODGroupData
			{
				lodGroupID = lodGroupID,
				lodOffset = lodOffset,
				lodCount = lodCount,
				fadeMode = fadeMode,
				worldSpaceReferencePoint = worldSpaceReferencePoint,
				worldSpaceSize = worldSpaceSize,
				renderersCount = renderersCount,
				lastLODIsBillboard = lastLODIsBillboard,
				forceLODMask = forceLODMask,
				invalidLODGroupID = invalidLODGroupID,
				lodRenderersCount = lodRenderersCount,
				lodScreenRelativeTransitionHeight = lodScreenRelativeTransitionHeight,
				lodFadeTransitionWidth = lodFadeTransitionWidth
			};
			callback(gpudrivenLODGroupData);
		};

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(GPUDrivenProcessor obj)
			{
				return obj.m_Ptr;
			}
		}
	}
}
