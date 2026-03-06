using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	[NativeHeader("Runtime/Math/Matrix4x4.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class BatchRendererGroup : IDisposable
	{
		public unsafe BatchRendererGroup(BatchRendererGroup.OnPerformCulling cullingCallback, IntPtr userContext)
		{
			this.m_PerformCulling = cullingCallback;
			this.m_GroupHandle = BatchRendererGroup.Create(this, (void*)userContext);
		}

		public unsafe BatchRendererGroup(BatchRendererGroupCreateInfo info)
		{
			this.m_PerformCulling = info.cullingCallback;
			this.m_GroupHandle = BatchRendererGroup.Create(this, (void*)info.userContext);
			this.m_FinishedCulling = info.finishedCullingCallback;
		}

		public void Dispose()
		{
			BatchRendererGroup.Destroy(this.m_GroupHandle);
			this.m_GroupHandle = IntPtr.Zero;
		}

		public ThreadedBatchContext GetThreadedBatchContext()
		{
			return new ThreadedBatchContext
			{
				batchRendererGroup = this.m_GroupHandle
			};
		}

		private BatchID AddDrawCommandBatch(IntPtr values, int count, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchID result;
			BatchRendererGroup.AddDrawCommandBatch_Injected(intPtr, values, count, ref buffer, bufferOffset, windowSize, out result);
			return result;
		}

		public BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer)
		{
			return this.AddDrawCommandBatch((IntPtr)batchMetadata.GetUnsafeReadOnlyPtr<MetadataValue>(), batchMetadata.Length, buffer, 0U, 0U);
		}

		public BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
		{
			return this.AddDrawCommandBatch((IntPtr)batchMetadata.GetUnsafeReadOnlyPtr<MetadataValue>(), batchMetadata.Length, buffer, bufferOffset, windowSize);
		}

		private void RemoveDrawCommandBatch(BatchID batchID)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.RemoveDrawCommandBatch_Injected(intPtr, ref batchID);
		}

		public void RemoveBatch(BatchID batchID)
		{
			this.RemoveDrawCommandBatch(batchID);
		}

		private void SetDrawCommandBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.SetDrawCommandBatchBuffer_Injected(intPtr, ref batchID, ref buffer);
		}

		public void SetBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
		{
			this.SetDrawCommandBatchBuffer(batchID, buffer);
		}

		public BatchMaterialID RegisterMaterial(Material material)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchMaterialID result;
			BatchRendererGroup.RegisterMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material), out result);
			return result;
		}

		internal unsafe void RegisterMaterials(ReadOnlySpan<int> materialID, Span<BatchMaterialID> batchMaterialID)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = materialID;
			fixed (int* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<BatchMaterialID> span = batchMaterialID;
				fixed (BatchMaterialID* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					BatchRendererGroup.RegisterMaterials_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void UnregisterMaterial(BatchMaterialID material)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.UnregisterMaterial_Injected(intPtr, ref material);
		}

		public Material GetRegisteredMaterial(BatchMaterialID material)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(BatchRendererGroup.GetRegisteredMaterial_Injected(intPtr, ref material));
		}

		public BatchMeshID RegisterMesh(Mesh mesh)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchMeshID result;
			BatchRendererGroup.RegisterMesh_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(mesh), out result);
			return result;
		}

		internal unsafe void RegisterMeshes(ReadOnlySpan<int> meshID, Span<BatchMeshID> batchMeshID)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = meshID;
			fixed (int* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				Span<BatchMeshID> span = batchMeshID;
				fixed (BatchMeshID* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					BatchRendererGroup.RegisterMeshes_Injected(intPtr, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public void UnregisterMesh(BatchMeshID mesh)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.UnregisterMesh_Injected(intPtr, ref mesh);
		}

		public Mesh GetRegisteredMesh(BatchMeshID mesh)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(BatchRendererGroup.GetRegisteredMesh_Injected(intPtr, ref mesh));
		}

		public void SetGlobalBounds(Bounds bounds)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.SetGlobalBounds_Injected(intPtr, ref bounds);
		}

		public void SetPickingMaterial(Material material)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.SetPickingMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		public void SetErrorMaterial(Material material)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.SetErrorMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		public void SetLoadingMaterial(Material material)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			BatchRendererGroup.SetLoadingMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		public unsafe void SetEnabledViewTypes(BatchCullingViewType[] viewTypes)
		{
			IntPtr intPtr = BatchRendererGroup.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<BatchCullingViewType> span = new Span<BatchCullingViewType>(viewTypes);
			fixed (BatchCullingViewType* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				BatchRendererGroup.SetEnabledViewTypes_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern BatchBufferTarget GetBufferTarget();

		public static BatchBufferTarget BufferTarget
		{
			get
			{
				return BatchRendererGroup.GetBufferTarget();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetConstantBufferMaxWindowSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetConstantBufferOffsetAlignment();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr Create([Unmarshalled] BatchRendererGroup group, void* userContext);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr groupHandle);

		[RequiredByNativeCode]
		private unsafe static void InvokeOnPerformCulling(BatchRendererGroup group, ref BatchRendererCullingOutput context, ref LODParameters lodParameters, IntPtr userContext)
		{
			NativeArray<Plane> inCullingPlanes = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Plane>((void*)context.cullingPlanes, context.cullingPlaneCount, Allocator.Invalid);
			NativeArray<CullingSplit> inCullingSplits = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<CullingSplit>((void*)context.cullingSplits, context.cullingSplitCount, Allocator.Invalid);
			NativeArray<BatchCullingOutputDrawCommands> drawCommands = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BatchCullingOutputDrawCommands>((void*)context.drawCommands, 1, Allocator.Invalid);
			try
			{
				BatchCullingOutput cullingOutput = new BatchCullingOutput
				{
					drawCommands = drawCommands,
					customCullingResult = new NativeArray<IntPtr>(1, Allocator.Temp, NativeArrayOptions.ClearMemory)
				};
				context.cullingJobsFence = group.m_PerformCulling(group, new BatchCullingContext(inCullingPlanes, inCullingSplits, lodParameters, context.localToWorldMatrix, context.viewType, context.projectionType, context.cullingFlags, context.viewID, context.cullingLayerMask, context.sceneCullingMask, context.splitExclusionMask, context.receiverPlaneOffset, context.receiverPlaneCount, context.occlusionBuffer), cullingOutput, userContext);
				context.customCullingResult = cullingOutput.customCullingResult[0];
			}
			finally
			{
				JobHandle.ScheduleBatchedJobs();
			}
		}

		[RequiredByNativeCode]
		private static void InvokeOnFinishedCulling(BatchRendererGroup group, IntPtr customCullingResult)
		{
			try
			{
				bool flag = group.m_FinishedCulling != null;
				if (flag)
				{
					group.m_FinishedCulling(customCullingResult);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[FreeFunction("BatchRendererGroup::OcclusionTestAABB", IsThreadSafe = true)]
		internal static bool OcclusionTestAABB(IntPtr occlusionBuffer, Bounds aabb)
		{
			return BatchRendererGroup.OcclusionTestAABB_Injected(occlusionBuffer, ref aabb);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddDrawCommandBatch_Injected(IntPtr _unity_self, IntPtr values, int count, [In] ref GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize, out BatchID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveDrawCommandBatch_Injected(IntPtr _unity_self, [In] ref BatchID batchID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDrawCommandBatchBuffer_Injected(IntPtr _unity_self, [In] ref BatchID batchID, [In] ref GraphicsBufferHandle buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterMaterial_Injected(IntPtr _unity_self, IntPtr material, out BatchMaterialID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterMaterials_Injected(IntPtr _unity_self, ref ManagedSpanWrapper materialID, ref ManagedSpanWrapper batchMaterialID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterMaterial_Injected(IntPtr _unity_self, [In] ref BatchMaterialID material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRegisteredMaterial_Injected(IntPtr _unity_self, [In] ref BatchMaterialID material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterMesh_Injected(IntPtr _unity_self, IntPtr mesh, out BatchMeshID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterMeshes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper meshID, ref ManagedSpanWrapper batchMeshID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnregisterMesh_Injected(IntPtr _unity_self, [In] ref BatchMeshID mesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetRegisteredMesh_Injected(IntPtr _unity_self, [In] ref BatchMeshID mesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalBounds_Injected(IntPtr _unity_self, [In] ref Bounds bounds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPickingMaterial_Injected(IntPtr _unity_self, IntPtr material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetErrorMaterial_Injected(IntPtr _unity_self, IntPtr material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLoadingMaterial_Injected(IntPtr _unity_self, IntPtr material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnabledViewTypes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper viewTypes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool OcclusionTestAABB_Injected(IntPtr occlusionBuffer, [In] ref Bounds aabb);

		private IntPtr m_GroupHandle = IntPtr.Zero;

		private BatchRendererGroup.OnPerformCulling m_PerformCulling;

		private BatchRendererGroup.OnFinishedCulling m_FinishedCulling;

		public delegate JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext, BatchCullingOutput cullingOutput, IntPtr userContext);

		public delegate void OnFinishedCulling(IntPtr customCullingResult);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(BatchRendererGroup batchRendererGroup)
			{
				return batchRendererGroup.m_GroupHandle;
			}
		}
	}
}
