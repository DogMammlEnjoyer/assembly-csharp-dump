using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	internal class InstanceDataSystem : IDisposable
	{
		public bool hasBoundingSpheres
		{
			get
			{
				return this.m_EnableBoundingSpheres;
			}
		}

		public CPUInstanceData.ReadOnly instanceData
		{
			get
			{
				return this.m_InstanceData.AsReadOnly();
			}
		}

		public CPUPerCameraInstanceData perCameraInstanceData
		{
			get
			{
				return this.m_PerCameraInstanceData;
			}
		}

		public int cameraCount
		{
			get
			{
				return this.m_PerCameraInstanceData.cameraCount;
			}
		}

		public CPUSharedInstanceData.ReadOnly sharedInstanceData
		{
			get
			{
				return this.m_SharedInstanceData.AsReadOnly();
			}
		}

		public NativeArray<InstanceHandle> aliveInstances
		{
			get
			{
				return this.m_InstanceData.instances.GetSubArray(0, this.m_InstanceData.instancesLength);
			}
		}

		public InstanceDataSystem(int maxInstances, bool enableBoundingSpheres, GPUResidentDrawerResources resources)
		{
			this.m_InstanceAllocators = default(InstanceAllocators);
			this.m_SharedInstanceData = default(CPUSharedInstanceData);
			this.m_InstanceData = default(CPUInstanceData);
			this.m_PerCameraInstanceData = default(CPUPerCameraInstanceData);
			this.m_InstanceAllocators.Initialize();
			this.m_SharedInstanceData.Initialize(maxInstances);
			this.m_InstanceData.Initialize(maxInstances);
			this.m_PerCameraInstanceData.Initialize(maxInstances);
			this.m_RendererGroupInstanceMultiHash = new NativeParallelMultiHashMap<int, InstanceHandle>(maxInstances, Allocator.Persistent);
			this.m_TransformUpdateCS = resources.transformUpdaterKernels;
			this.m_WindDataUpdateCS = resources.windDataUpdaterKernels;
			this.m_TransformInitKernel = this.m_TransformUpdateCS.FindKernel("ScatterInitTransformMain");
			this.m_TransformUpdateKernel = this.m_TransformUpdateCS.FindKernel("ScatterUpdateTransformMain");
			this.m_MotionUpdateKernel = this.m_TransformUpdateCS.FindKernel("ScatterUpdateMotionMain");
			this.m_ProbeUpdateKernel = this.m_TransformUpdateCS.FindKernel("ScatterUpdateProbesMain");
			if (enableBoundingSpheres)
			{
				this.m_TransformUpdateCS.EnableKeyword("PROCESS_BOUNDING_SPHERES");
			}
			else
			{
				this.m_TransformUpdateCS.DisableKeyword("PROCESS_BOUNDING_SPHERES");
			}
			this.m_WindDataCopyHistoryKernel = this.m_WindDataUpdateCS.FindKernel("WindDataCopyHistoryMain");
			this.m_EnableBoundingSpheres = enableBoundingSpheres;
		}

		public void Dispose()
		{
			this.m_InstanceAllocators.Dispose();
			this.m_SharedInstanceData.Dispose();
			this.m_InstanceData.Dispose();
			this.m_PerCameraInstanceData.Dispose();
			this.m_RendererGroupInstanceMultiHash.Dispose();
			ComputeBuffer updateIndexQueueBuffer = this.m_UpdateIndexQueueBuffer;
			if (updateIndexQueueBuffer != null)
			{
				updateIndexQueueBuffer.Dispose();
			}
			ComputeBuffer probeUpdateDataQueueBuffer = this.m_ProbeUpdateDataQueueBuffer;
			if (probeUpdateDataQueueBuffer != null)
			{
				probeUpdateDataQueueBuffer.Dispose();
			}
			ComputeBuffer probeOcclusionUpdateDataQueueBuffer = this.m_ProbeOcclusionUpdateDataQueueBuffer;
			if (probeOcclusionUpdateDataQueueBuffer != null)
			{
				probeOcclusionUpdateDataQueueBuffer.Dispose();
			}
			ComputeBuffer transformUpdateDataQueueBuffer = this.m_TransformUpdateDataQueueBuffer;
			if (transformUpdateDataQueueBuffer != null)
			{
				transformUpdateDataQueueBuffer.Dispose();
			}
			ComputeBuffer boundingSpheresUpdateDataQueueBuffer = this.m_BoundingSpheresUpdateDataQueueBuffer;
			if (boundingSpheresUpdateDataQueueBuffer == null)
			{
				return;
			}
			boundingSpheresUpdateDataQueueBuffer.Dispose();
		}

		public int GetMaxInstancesOfType(InstanceType instanceType)
		{
			return this.m_InstanceAllocators.GetInstanceHandlesLength(instanceType);
		}

		public int GetAliveInstancesOfType(InstanceType instanceType)
		{
			return this.m_InstanceAllocators.GetInstancesLength(instanceType);
		}

		private void EnsureIndexQueueBufferCapacity(int capacity)
		{
			if (this.m_UpdateIndexQueueBuffer == null || this.m_UpdateIndexQueueBuffer.count < capacity)
			{
				ComputeBuffer updateIndexQueueBuffer = this.m_UpdateIndexQueueBuffer;
				if (updateIndexQueueBuffer != null)
				{
					updateIndexQueueBuffer.Dispose();
				}
				this.m_UpdateIndexQueueBuffer = new ComputeBuffer(capacity, 4, ComputeBufferType.Raw);
			}
		}

		private void EnsureProbeBuffersCapacity(int capacity)
		{
			this.EnsureIndexQueueBufferCapacity(capacity);
			if (this.m_ProbeUpdateDataQueueBuffer == null || this.m_ProbeUpdateDataQueueBuffer.count < capacity)
			{
				ComputeBuffer probeUpdateDataQueueBuffer = this.m_ProbeUpdateDataQueueBuffer;
				if (probeUpdateDataQueueBuffer != null)
				{
					probeUpdateDataQueueBuffer.Dispose();
				}
				ComputeBuffer probeOcclusionUpdateDataQueueBuffer = this.m_ProbeOcclusionUpdateDataQueueBuffer;
				if (probeOcclusionUpdateDataQueueBuffer != null)
				{
					probeOcclusionUpdateDataQueueBuffer.Dispose();
				}
				this.m_ProbeUpdateDataQueueBuffer = new ComputeBuffer(capacity, Marshal.SizeOf<SHUpdatePacket>(), ComputeBufferType.Structured);
				this.m_ProbeOcclusionUpdateDataQueueBuffer = new ComputeBuffer(capacity, Marshal.SizeOf<Vector4>(), ComputeBufferType.Structured);
			}
		}

		private void EnsureTransformBuffersCapacity(int capacity)
		{
			this.EnsureIndexQueueBufferCapacity(capacity);
			int num = capacity * 2;
			if (this.m_TransformUpdateDataQueueBuffer == null || this.m_TransformUpdateDataQueueBuffer.count < num)
			{
				ComputeBuffer transformUpdateDataQueueBuffer = this.m_TransformUpdateDataQueueBuffer;
				if (transformUpdateDataQueueBuffer != null)
				{
					transformUpdateDataQueueBuffer.Dispose();
				}
				ComputeBuffer boundingSpheresUpdateDataQueueBuffer = this.m_BoundingSpheresUpdateDataQueueBuffer;
				if (boundingSpheresUpdateDataQueueBuffer != null)
				{
					boundingSpheresUpdateDataQueueBuffer.Dispose();
				}
				this.m_TransformUpdateDataQueueBuffer = new ComputeBuffer(num, Marshal.SizeOf<TransformUpdatePacket>(), ComputeBufferType.Structured);
				if (this.m_EnableBoundingSpheres)
				{
					this.m_BoundingSpheresUpdateDataQueueBuffer = new ComputeBuffer(capacity, Marshal.SizeOf<float4>(), ComputeBufferType.Structured);
				}
			}
		}

		private JobHandle ScheduleInterpolateProbesAndUpdateTetrahedronCache(int queueCount, NativeArray<InstanceHandle> probeUpdateInstanceQueue, NativeArray<int> compactTetrahedronCache, NativeArray<Vector3> probeQueryPosition, NativeArray<SphericalHarmonicsL2> probeUpdateDataQueue, NativeArray<Vector4> probeOcclusionUpdateDataQueue)
		{
			LightProbesQuery lightProbesQuery = new LightProbesQuery(Allocator.TempJob);
			InstanceDataSystem.CalculateInterpolatedLightAndOcclusionProbesBatchJob jobData = new InstanceDataSystem.CalculateInterpolatedLightAndOcclusionProbesBatchJob
			{
				lightProbesQuery = lightProbesQuery,
				probesCount = queueCount,
				queryPostitions = probeQueryPosition,
				compactTetrahedronCache = compactTetrahedronCache,
				probesSphericalHarmonics = probeUpdateDataQueue,
				probesOcclusion = probeOcclusionUpdateDataQueue
			};
			int arrayLength = 1 + queueCount / 8;
			JobHandle jobHandle = jobData.Schedule(arrayLength, 1, default(JobHandle));
			lightProbesQuery.Dispose(jobHandle);
			return new InstanceDataSystem.ScatterTetrahedronCacheIndicesJob
			{
				compactTetrahedronCache = compactTetrahedronCache,
				probeInstances = probeUpdateInstanceQueue,
				instanceData = this.m_InstanceData
			}.Schedule(queueCount, 128, jobHandle);
		}

		private void DispatchProbeUpdateCommand(int queueCount, NativeArray<InstanceHandle> probeInstanceQueue, NativeArray<SphericalHarmonicsL2> probeUpdateDataQueue, NativeArray<Vector4> probeOcclusionUpdateDataQueue, RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			this.EnsureProbeBuffersCapacity(queueCount);
			NativeArray<GPUInstanceIndex> nativeArray = new NativeArray<GPUInstanceIndex>(queueCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			outputBuffer.CPUInstanceArrayToGPUInstanceArray(probeInstanceQueue.GetSubArray(0, queueCount), nativeArray);
			this.m_UpdateIndexQueueBuffer.SetData<GPUInstanceIndex>(nativeArray, 0, 0, queueCount);
			this.m_ProbeUpdateDataQueueBuffer.SetData<SphericalHarmonicsL2>(probeUpdateDataQueue, 0, 0, queueCount);
			this.m_ProbeOcclusionUpdateDataQueueBuffer.SetData<Vector4>(probeOcclusionUpdateDataQueue, 0, 0, queueCount);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._ProbeUpdateQueueCount, queueCount);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._SHUpdateVec4Offset, renderersParameters.shCoefficients.uintOffset);
			this.m_TransformUpdateCS.SetBuffer(this.m_ProbeUpdateKernel, InstanceDataSystem.InstanceTransformUpdateIDs._ProbeUpdateIndexQueue, this.m_UpdateIndexQueueBuffer);
			this.m_TransformUpdateCS.SetBuffer(this.m_ProbeUpdateKernel, InstanceDataSystem.InstanceTransformUpdateIDs._ProbeUpdateDataQueue, this.m_ProbeUpdateDataQueueBuffer);
			this.m_TransformUpdateCS.SetBuffer(this.m_ProbeUpdateKernel, InstanceDataSystem.InstanceTransformUpdateIDs._ProbeOcclusionUpdateDataQueue, this.m_ProbeOcclusionUpdateDataQueueBuffer);
			this.m_TransformUpdateCS.SetBuffer(this.m_ProbeUpdateKernel, InstanceDataSystem.InstanceTransformUpdateIDs._OutputProbeBuffer, outputBuffer.gpuBuffer);
			this.m_TransformUpdateCS.Dispatch(this.m_ProbeUpdateKernel, (queueCount + 63) / 64, 1, 1);
			nativeArray.Dispose();
		}

		private void DispatchMotionUpdateCommand(int motionQueueCount, NativeArray<InstanceHandle> transformInstanceQueue, RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			this.EnsureTransformBuffersCapacity(motionQueueCount);
			NativeArray<GPUInstanceIndex> nativeArray = new NativeArray<GPUInstanceIndex>(motionQueueCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			outputBuffer.CPUInstanceArrayToGPUInstanceArray(transformInstanceQueue.GetSubArray(0, motionQueueCount), nativeArray);
			this.m_UpdateIndexQueueBuffer.SetData<GPUInstanceIndex>(nativeArray, 0, 0, motionQueueCount);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateQueueCount, motionQueueCount);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputL2WVec4Offset, renderersParameters.localToWorld.uintOffset);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputW2LVec4Offset, renderersParameters.worldToLocal.uintOffset);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputPrevL2WVec4Offset, renderersParameters.matrixPreviousM.uintOffset);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputPrevW2LVec4Offset, renderersParameters.matrixPreviousMI.uintOffset);
			this.m_TransformUpdateCS.SetBuffer(this.m_MotionUpdateKernel, InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateIndexQueue, this.m_UpdateIndexQueueBuffer);
			this.m_TransformUpdateCS.SetBuffer(this.m_MotionUpdateKernel, InstanceDataSystem.InstanceTransformUpdateIDs._OutputTransformBuffer, outputBuffer.gpuBuffer);
			this.m_TransformUpdateCS.Dispatch(this.m_MotionUpdateKernel, (motionQueueCount + 63) / 64, 1, 1);
			nativeArray.Dispose();
		}

		private void DispatchTransformUpdateCommand(bool initialize, int transformQueueCount, NativeArray<InstanceHandle> transformInstanceQueue, NativeArray<TransformUpdatePacket> updateDataQueue, NativeArray<float4> boundingSphereUpdateDataQueue, RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			this.EnsureTransformBuffersCapacity(transformQueueCount);
			int count;
			int kernelIndex;
			if (initialize)
			{
				count = transformQueueCount * 2;
				kernelIndex = this.m_TransformInitKernel;
			}
			else
			{
				count = transformQueueCount;
				kernelIndex = this.m_TransformUpdateKernel;
			}
			NativeArray<GPUInstanceIndex> nativeArray = new NativeArray<GPUInstanceIndex>(transformQueueCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			outputBuffer.CPUInstanceArrayToGPUInstanceArray(transformInstanceQueue.GetSubArray(0, transformQueueCount), nativeArray);
			this.m_UpdateIndexQueueBuffer.SetData<GPUInstanceIndex>(nativeArray, 0, 0, transformQueueCount);
			this.m_TransformUpdateDataQueueBuffer.SetData<TransformUpdatePacket>(updateDataQueue, 0, 0, count);
			if (this.m_EnableBoundingSpheres)
			{
				this.m_BoundingSpheresUpdateDataQueueBuffer.SetData<float4>(boundingSphereUpdateDataQueue, 0, 0, transformQueueCount);
			}
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateQueueCount, transformQueueCount);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputL2WVec4Offset, renderersParameters.localToWorld.uintOffset);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputW2LVec4Offset, renderersParameters.worldToLocal.uintOffset);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputPrevL2WVec4Offset, renderersParameters.matrixPreviousM.uintOffset);
			this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateOutputPrevW2LVec4Offset, renderersParameters.matrixPreviousMI.uintOffset);
			this.m_TransformUpdateCS.SetBuffer(kernelIndex, InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateIndexQueue, this.m_UpdateIndexQueueBuffer);
			this.m_TransformUpdateCS.SetBuffer(kernelIndex, InstanceDataSystem.InstanceTransformUpdateIDs._TransformUpdateDataQueue, this.m_TransformUpdateDataQueueBuffer);
			if (this.m_EnableBoundingSpheres)
			{
				this.m_TransformUpdateCS.SetInt(InstanceDataSystem.InstanceTransformUpdateIDs._BoundingSphereOutputVec4Offset, renderersParameters.boundingSphere.uintOffset);
				this.m_TransformUpdateCS.SetBuffer(kernelIndex, InstanceDataSystem.InstanceTransformUpdateIDs._BoundingSphereDataQueue, this.m_BoundingSpheresUpdateDataQueueBuffer);
			}
			this.m_TransformUpdateCS.SetBuffer(kernelIndex, InstanceDataSystem.InstanceTransformUpdateIDs._OutputTransformBuffer, outputBuffer.gpuBuffer);
			this.m_TransformUpdateCS.Dispatch(kernelIndex, (transformQueueCount + 63) / 64, 1, 1);
			nativeArray.Dispose();
		}

		private void DispatchWindDataCopyHistoryCommand(NativeArray<GPUInstanceIndex> gpuInstanceIndices, RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			int windDataCopyHistoryKernel = this.m_WindDataCopyHistoryKernel;
			int length = gpuInstanceIndices.Length;
			this.EnsureIndexQueueBufferCapacity(length);
			this.m_UpdateIndexQueueBuffer.SetData<GPUInstanceIndex>(gpuInstanceIndices, 0, 0, length);
			this.m_WindDataUpdateCS.SetInt(InstanceDataSystem.InstanceWindDataUpdateIDs._WindDataQueueCount, length);
			for (int i = 0; i < 16; i++)
			{
				this.m_ScratchWindParamAddressArray[i * 4] = renderersParameters.windParams[i].gpuAddress;
			}
			this.m_WindDataUpdateCS.SetInts(InstanceDataSystem.InstanceWindDataUpdateIDs._WindParamAddressArray, this.m_ScratchWindParamAddressArray);
			for (int j = 0; j < 16; j++)
			{
				this.m_ScratchWindParamAddressArray[j * 4] = renderersParameters.windHistoryParams[j].gpuAddress;
			}
			this.m_WindDataUpdateCS.SetInts(InstanceDataSystem.InstanceWindDataUpdateIDs._WindHistoryParamAddressArray, this.m_ScratchWindParamAddressArray);
			this.m_WindDataUpdateCS.SetBuffer(windDataCopyHistoryKernel, InstanceDataSystem.InstanceWindDataUpdateIDs._WindDataUpdateIndexQueue, this.m_UpdateIndexQueueBuffer);
			this.m_WindDataUpdateCS.SetBuffer(windDataCopyHistoryKernel, InstanceDataSystem.InstanceWindDataUpdateIDs._WindDataBuffer, outputBuffer.gpuBuffer);
			this.m_WindDataUpdateCS.Dispatch(windDataCopyHistoryKernel, (length + 63) / 64, 1, 1);
		}

		private unsafe void UpdateInstanceMotionsData(in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			NativeArray<InstanceHandle> nativeArray = new NativeArray<InstanceHandle>(this.m_InstanceData.instancesLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			new InstanceDataSystem.MotionUpdateJob
			{
				queueWriteBase = 0,
				instanceData = this.m_InstanceData,
				atomicUpdateQueueCount = new UnsafeAtomicCounter32((void*)(&num)),
				transformUpdateInstanceQueue = nativeArray
			}.Schedule((this.m_InstanceData.instancesLength + 63) / 64, 16, default(JobHandle)).Complete();
			if (num > 0)
			{
				this.DispatchMotionUpdateCommand(num, nativeArray, renderersParameters, outputBuffer);
			}
			nativeArray.Dispose();
		}

		private unsafe void UpdateInstanceTransformsData(bool initialize, NativeArray<InstanceHandle> instances, NativeArray<Matrix4x4> localToWorldMatrices, NativeArray<Matrix4x4> prevLocalToWorldMatrices, in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			NativeArray<InstanceHandle> nativeArray = new NativeArray<InstanceHandle>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<TransformUpdatePacket> nativeArray2 = new NativeArray<TransformUpdatePacket>(initialize ? (instances.Length * 2) : instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<float4> nativeArray3 = new NativeArray<float4>(this.m_EnableBoundingSpheres ? instances.Length : 0, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<InstanceHandle> nativeArray4 = new NativeArray<InstanceHandle>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<int> compactTetrahedronCache = new NativeArray<int>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<Vector3> probeQueryPosition = new NativeArray<Vector3>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<SphericalHarmonicsL2> probeUpdateDataQueue = new NativeArray<SphericalHarmonicsL2>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<Vector4> probeOcclusionUpdateDataQueue = new NativeArray<Vector4>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			int num2 = 0;
			InstanceDataSystem.TransformUpdateJob jobData = new InstanceDataSystem.TransformUpdateJob
			{
				initialize = initialize,
				enableBoundingSpheres = this.m_EnableBoundingSpheres,
				instances = instances,
				localToWorldMatrices = localToWorldMatrices,
				prevLocalToWorldMatrices = prevLocalToWorldMatrices,
				atomicTransformQueueCount = new UnsafeAtomicCounter32((void*)(&num)),
				sharedInstanceData = this.m_SharedInstanceData,
				instanceData = this.m_InstanceData,
				transformUpdateInstanceQueue = nativeArray,
				transformUpdateDataQueue = nativeArray2,
				boundingSpheresDataQueue = nativeArray3
			};
			InstanceDataSystem.ProbesUpdateJob jobData2 = new InstanceDataSystem.ProbesUpdateJob
			{
				instances = instances,
				instanceData = this.m_InstanceData,
				sharedInstanceData = this.m_SharedInstanceData,
				atomicProbesQueueCount = new UnsafeAtomicCounter32((void*)(&num2)),
				probeInstanceQueue = nativeArray4,
				compactTetrahedronCache = compactTetrahedronCache,
				probeQueryPosition = probeQueryPosition
			};
			JobHandle dependsOn = jobData.ScheduleBatch(instances.Length, 64, default(JobHandle));
			jobData2.ScheduleBatch(instances.Length, 64, dependsOn).Complete();
			if (num2 > 0)
			{
				this.ScheduleInterpolateProbesAndUpdateTetrahedronCache(num2, nativeArray4, compactTetrahedronCache, probeQueryPosition, probeUpdateDataQueue, probeOcclusionUpdateDataQueue).Complete();
				this.DispatchProbeUpdateCommand(num2, nativeArray4, probeUpdateDataQueue, probeOcclusionUpdateDataQueue, renderersParameters, outputBuffer);
			}
			if (num > 0)
			{
				this.DispatchTransformUpdateCommand(initialize, num, nativeArray, nativeArray2, nativeArray3, renderersParameters, outputBuffer);
			}
			nativeArray.Dispose();
			nativeArray2.Dispose();
			nativeArray3.Dispose();
			nativeArray4.Dispose();
			compactTetrahedronCache.Dispose();
			probeQueryPosition.Dispose();
			probeUpdateDataQueue.Dispose();
			probeOcclusionUpdateDataQueue.Dispose();
		}

		private unsafe void UpdateInstanceProbesData(NativeArray<InstanceHandle> instances, in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			NativeArray<InstanceHandle> nativeArray = new NativeArray<InstanceHandle>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<int> compactTetrahedronCache = new NativeArray<int>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<Vector3> probeQueryPosition = new NativeArray<Vector3>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<SphericalHarmonicsL2> probeUpdateDataQueue = new NativeArray<SphericalHarmonicsL2>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<Vector4> probeOcclusionUpdateDataQueue = new NativeArray<Vector4>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			int num = 0;
			new InstanceDataSystem.ProbesUpdateJob
			{
				instances = instances,
				instanceData = this.m_InstanceData,
				sharedInstanceData = this.m_SharedInstanceData,
				atomicProbesQueueCount = new UnsafeAtomicCounter32((void*)(&num)),
				probeInstanceQueue = nativeArray,
				compactTetrahedronCache = compactTetrahedronCache,
				probeQueryPosition = probeQueryPosition
			}.ScheduleBatch(instances.Length, 64, default(JobHandle)).Complete();
			if (num > 0)
			{
				this.ScheduleInterpolateProbesAndUpdateTetrahedronCache(num, nativeArray, compactTetrahedronCache, probeQueryPosition, probeUpdateDataQueue, probeOcclusionUpdateDataQueue).Complete();
				this.DispatchProbeUpdateCommand(num, nativeArray, probeUpdateDataQueue, probeOcclusionUpdateDataQueue, renderersParameters, outputBuffer);
			}
			nativeArray.Dispose();
			compactTetrahedronCache.Dispose();
			probeQueryPosition.Dispose();
			probeUpdateDataQueue.Dispose();
			probeOcclusionUpdateDataQueue.Dispose();
		}

		public void UpdateInstanceWindDataHistory(NativeArray<GPUInstanceIndex> gpuInstanceIndices, RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			if (gpuInstanceIndices.Length == 0)
			{
				return;
			}
			this.DispatchWindDataCopyHistoryCommand(gpuInstanceIndices, renderersParameters, outputBuffer);
		}

		public unsafe void ReallocateAndGetInstances(in GPUDrivenRendererGroupData rendererData, NativeArray<InstanceHandle> instances)
		{
			int instancesCount = 0;
			int num = 0;
			NativeArray<int> nativeArray = rendererData.instancesCount;
			bool flag = nativeArray.Length == 0;
			if (flag)
			{
				InstanceDataSystem.QueryRendererGroupInstancesJob jobData = new InstanceDataSystem.QueryRendererGroupInstancesJob
				{
					rendererGroupInstanceMultiHash = this.m_RendererGroupInstanceMultiHash,
					rendererGroupIDs = rendererData.rendererGroupID,
					instances = instances,
					atomicNonFoundInstancesCount = new UnsafeAtomicCounter32((void*)(&num))
				};
				nativeArray = rendererData.rendererGroupID;
				jobData.ScheduleBatch(nativeArray.Length, 128, default(JobHandle)).Complete();
				instancesCount = num;
			}
			else
			{
				InstanceDataSystem.QueryRendererGroupInstancesMultiJob jobData2 = new InstanceDataSystem.QueryRendererGroupInstancesMultiJob
				{
					rendererGroupInstanceMultiHash = this.m_RendererGroupInstanceMultiHash,
					rendererGroupIDs = rendererData.rendererGroupID,
					instancesOffsets = rendererData.instancesOffset,
					instancesCounts = rendererData.instancesCount,
					instances = instances,
					atomicNonFoundSharedInstancesCount = new UnsafeAtomicCounter32((void*)(&instancesCount)),
					atomicNonFoundInstancesCount = new UnsafeAtomicCounter32((void*)(&num))
				};
				nativeArray = rendererData.rendererGroupID;
				jobData2.ScheduleBatch(nativeArray.Length, 128, default(JobHandle)).Complete();
			}
			this.m_InstanceData.EnsureFreeInstances(num);
			this.m_PerCameraInstanceData.Grow(this.m_InstanceData.instancesCapacity);
			this.m_SharedInstanceData.EnsureFreeInstances(instancesCount);
			InstanceDataSystemBurst.ReallocateInstances(flag, rendererData.rendererGroupID, rendererData.packedRendererData, rendererData.instancesOffset, rendererData.instancesCount, ref this.m_InstanceAllocators, ref this.m_InstanceData, ref this.m_PerCameraInstanceData, ref this.m_SharedInstanceData, ref instances, ref this.m_RendererGroupInstanceMultiHash);
		}

		public void FreeRendererGroupInstances(NativeArray<int> rendererGroupsID)
		{
			NativeArray<int>.ReadOnly readOnly = rendererGroupsID.AsReadOnly();
			InstanceDataSystemBurst.FreeRendererGroupInstances(readOnly, ref this.m_InstanceAllocators, ref this.m_InstanceData, ref this.m_PerCameraInstanceData, ref this.m_SharedInstanceData, ref this.m_RendererGroupInstanceMultiHash);
		}

		public void FreeInstances(NativeArray<InstanceHandle> instances)
		{
			NativeArray<InstanceHandle>.ReadOnly readOnly = instances.AsReadOnly();
			InstanceDataSystemBurst.FreeInstances(readOnly, ref this.m_InstanceAllocators, ref this.m_InstanceData, ref this.m_PerCameraInstanceData, ref this.m_SharedInstanceData, ref this.m_RendererGroupInstanceMultiHash);
		}

		public JobHandle ScheduleUpdateInstanceDataJob(NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData, NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataMap)
		{
			NativeArray<int> nativeArray = rendererData.instancesCount;
			bool implicitInstanceIndices = nativeArray.Length == 0;
			InstanceDataSystem.UpdateRendererInstancesJob jobData = new InstanceDataSystem.UpdateRendererInstancesJob
			{
				implicitInstanceIndices = implicitInstanceIndices,
				instances = instances,
				rendererData = rendererData,
				lodGroupDataMap = lodGroupDataMap,
				instanceData = this.m_InstanceData,
				sharedInstanceData = this.m_SharedInstanceData,
				perCameraInstanceData = this.m_PerCameraInstanceData
			};
			nativeArray = rendererData.rendererGroupID;
			return jobData.Schedule(nativeArray.Length, 128, default(JobHandle));
		}

		public void UpdateAllInstanceProbes(in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			NativeArray<InstanceHandle> subArray = this.m_InstanceData.instances.GetSubArray(0, this.m_InstanceData.instancesLength);
			if (subArray.Length == 0)
			{
				return;
			}
			this.UpdateInstanceProbesData(subArray, renderersParameters, outputBuffer);
		}

		public void InitializeInstanceTransforms(NativeArray<InstanceHandle> instances, NativeArray<Matrix4x4> localToWorldMatrices, NativeArray<Matrix4x4> prevLocalToWorldMatrices, in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			if (instances.Length == 0)
			{
				return;
			}
			this.UpdateInstanceTransformsData(true, instances, localToWorldMatrices, prevLocalToWorldMatrices, renderersParameters, outputBuffer);
		}

		public void UpdateInstanceTransforms(NativeArray<InstanceHandle> instances, NativeArray<Matrix4x4> localToWorldMatrices, in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			if (instances.Length == 0)
			{
				return;
			}
			this.UpdateInstanceTransformsData(false, instances, localToWorldMatrices, localToWorldMatrices, renderersParameters, outputBuffer);
		}

		public void UpdateInstanceMotions(in RenderersParameters renderersParameters, GPUInstanceDataBuffer outputBuffer)
		{
			if (this.m_InstanceData.instancesLength == 0)
			{
				return;
			}
			this.UpdateInstanceMotionsData(renderersParameters, outputBuffer);
		}

		public JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeArray<InstanceHandle> instances)
		{
			if (rendererGroupIDs.Length == 0)
			{
				return default(JobHandle);
			}
			return new InstanceDataSystem.QueryRendererGroupInstancesJob
			{
				rendererGroupInstanceMultiHash = this.m_RendererGroupInstanceMultiHash,
				rendererGroupIDs = rendererGroupIDs,
				instances = instances
			}.ScheduleBatch(rendererGroupIDs.Length, 128, default(JobHandle));
		}

		public JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeList<InstanceHandle> instances)
		{
			if (rendererGroupIDs.Length == 0)
			{
				return default(JobHandle);
			}
			NativeArray<int> instancesOffset = new NativeArray<int>(rendererGroupIDs.Length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			NativeArray<int> instancesCount = new NativeArray<int>(rendererGroupIDs.Length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			JobHandle jobHandle = this.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instancesOffset, instancesCount, instances);
			instancesOffset.Dispose(jobHandle);
			instancesCount.Dispose(jobHandle);
			return jobHandle;
		}

		public JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeArray<int> instancesOffset, NativeArray<int> instancesCount, NativeList<InstanceHandle> instances)
		{
			if (rendererGroupIDs.Length == 0)
			{
				return default(JobHandle);
			}
			JobHandle dependsOn = new InstanceDataSystem.QueryRendererGroupInstancesCountJob
			{
				instanceData = this.m_InstanceData,
				sharedInstanceData = this.m_SharedInstanceData,
				rendererGroupInstanceMultiHash = this.m_RendererGroupInstanceMultiHash,
				rendererGroupIDs = rendererGroupIDs,
				instancesCount = instancesCount
			}.ScheduleBatch(rendererGroupIDs.Length, 128, default(JobHandle));
			JobHandle dependsOn2 = new InstanceDataSystem.ComputeInstancesOffsetAndResizeInstancesArrayJob
			{
				instancesCount = instancesCount,
				instancesOffset = instancesOffset,
				instances = instances
			}.Schedule(dependsOn);
			return new InstanceDataSystem.QueryRendererGroupInstancesMultiJob
			{
				rendererGroupInstanceMultiHash = this.m_RendererGroupInstanceMultiHash,
				rendererGroupIDs = rendererGroupIDs,
				instancesOffsets = instancesOffset,
				instancesCounts = instancesCount,
				instances = instances.AsDeferredJobArray()
			}.ScheduleBatch(rendererGroupIDs.Length, 128, dependsOn2);
		}

		public JobHandle ScheduleQuerySortedMeshInstancesJob(NativeArray<int> sortedMeshIDs, NativeList<InstanceHandle> instances)
		{
			if (sortedMeshIDs.Length == 0)
			{
				return default(JobHandle);
			}
			instances.Capacity = this.m_InstanceData.instancesLength;
			return new InstanceDataSystem.QuerySortedMeshInstancesJob
			{
				instanceData = this.m_InstanceData,
				sharedInstanceData = this.m_SharedInstanceData,
				sortedMeshID = sortedMeshIDs,
				instances = instances
			}.ScheduleBatch(this.m_InstanceData.instancesLength, 64, default(JobHandle));
		}

		public JobHandle ScheduleCollectInstancesLODGroupAndMasksJob(NativeArray<InstanceHandle> instances, NativeArray<uint> lodGroupAndMasks)
		{
			return new InstanceDataSystem.CollectInstancesLODGroupsAndMasksJob
			{
				instanceData = this.instanceData,
				sharedInstanceData = this.sharedInstanceData,
				instances = instances,
				lodGroupAndMasks = lodGroupAndMasks
			}.Schedule(instances.Length, 128, default(JobHandle));
		}

		public bool InternalSanityCheckStates()
		{
			NativeParallelHashMap<SharedInstanceHandle, int> nativeParallelHashMap = new NativeParallelHashMap<SharedInstanceHandle, int>(64, Allocator.Temp);
			int num = 0;
			for (int i = 0; i < this.m_InstanceData.handlesLength; i++)
			{
				InstanceHandle instance = InstanceHandle.FromInt(i);
				if (this.m_InstanceData.IsValidInstance(instance))
				{
					SharedInstanceHandle key = this.m_InstanceData.Get_SharedInstance(instance);
					int num2;
					if (nativeParallelHashMap.TryGetValue(key, out num2))
					{
						nativeParallelHashMap[key] = num2 + 1;
					}
					else
					{
						nativeParallelHashMap.Add(key, 1);
					}
					num++;
				}
			}
			if (this.m_InstanceData.instancesLength != num)
			{
				return false;
			}
			int num3 = 0;
			for (int j = 0; j < this.m_SharedInstanceData.handlesLength; j++)
			{
				SharedInstanceHandle sharedInstanceHandle = new SharedInstanceHandle
				{
					index = j
				};
				if (this.m_SharedInstanceData.IsValidInstance(sharedInstanceHandle))
				{
					int num4 = this.m_SharedInstanceData.Get_RefCount(sharedInstanceHandle);
					if (nativeParallelHashMap[sharedInstanceHandle] != num4)
					{
						return false;
					}
					num3++;
				}
			}
			return this.m_SharedInstanceData.instancesLength == num3;
		}

		public unsafe void GetVisibleTreeInstances(in ParallelBitArray compactedVisibilityMasks, in ParallelBitArray processedBits, NativeList<int> visibeTreeRendererIDs, NativeList<InstanceHandle> visibeTreeInstances, bool becomeVisibleOnly, out int becomeVisibeTreeInstancesCount)
		{
			becomeVisibeTreeInstancesCount = 0;
			int aliveInstancesOfType = this.GetAliveInstancesOfType(InstanceType.SpeedTree);
			if (aliveInstancesOfType == 0)
			{
				return;
			}
			visibeTreeRendererIDs.ResizeUninitialized(aliveInstancesOfType);
			visibeTreeInstances.ResizeUninitialized(aliveInstancesOfType);
			int num = 0;
			new InstanceDataSystem.GetVisibleNonProcessedTreeInstancesJob
			{
				becomeVisible = true,
				instanceData = this.m_InstanceData,
				sharedInstanceData = this.m_SharedInstanceData,
				compactedVisibilityMasks = compactedVisibilityMasks,
				processedBits = processedBits,
				rendererIDs = visibeTreeRendererIDs.AsArray(),
				instances = visibeTreeInstances.AsArray(),
				atomicTreeInstancesCount = new UnsafeAtomicCounter32((void*)(&num))
			}.ScheduleBatch(this.m_InstanceData.instancesLength, 64, default(JobHandle)).Complete();
			becomeVisibeTreeInstancesCount = num;
			if (!becomeVisibleOnly)
			{
				new InstanceDataSystem.GetVisibleNonProcessedTreeInstancesJob
				{
					becomeVisible = false,
					instanceData = this.m_InstanceData,
					sharedInstanceData = this.m_SharedInstanceData,
					compactedVisibilityMasks = compactedVisibilityMasks,
					processedBits = processedBits,
					rendererIDs = visibeTreeRendererIDs.AsArray(),
					instances = visibeTreeInstances.AsArray(),
					atomicTreeInstancesCount = new UnsafeAtomicCounter32((void*)(&num))
				}.ScheduleBatch(this.m_InstanceData.instancesLength, 64, default(JobHandle)).Complete();
			}
			visibeTreeRendererIDs.ResizeUninitialized(num);
			visibeTreeInstances.ResizeUninitialized(num);
		}

		public void UpdatePerFrameInstanceVisibility(in ParallelBitArray compactedVisibilityMasks)
		{
			new InstanceDataSystem.UpdateCompactedInstanceVisibilityJob
			{
				instanceData = this.m_InstanceData,
				compactedVisibilityMasks = compactedVisibilityMasks
			}.ScheduleBatch(this.m_InstanceData.instancesLength, 64, default(JobHandle)).Complete();
		}

		public void DeallocatePerCameraInstanceData(NativeArray<int> cameraIDs)
		{
			this.m_PerCameraInstanceData.DeallocateCameras(cameraIDs);
		}

		public void AllocatePerCameraInstanceData(NativeArray<int> cameraIDs)
		{
			this.m_PerCameraInstanceData.AllocateCameras(cameraIDs);
		}

		private unsafe static int AtomicAddLengthNoResize<[IsUnmanaged] T>(in NativeList<T> list, int count) where T : struct, ValueType
		{
			NativeList<T> nativeList = list;
			return Interlocked.Add(ref nativeList.GetUnsafeList()->m_length, count) - count;
		}

		private InstanceAllocators m_InstanceAllocators;

		private CPUSharedInstanceData m_SharedInstanceData;

		private CPUInstanceData m_InstanceData;

		private CPUPerCameraInstanceData m_PerCameraInstanceData;

		private NativeParallelMultiHashMap<int, InstanceHandle> m_RendererGroupInstanceMultiHash;

		private ComputeShader m_TransformUpdateCS;

		private ComputeShader m_WindDataUpdateCS;

		private int m_TransformInitKernel;

		private int m_TransformUpdateKernel;

		private int m_MotionUpdateKernel;

		private int m_ProbeUpdateKernel;

		private int m_LODUpdateKernel;

		private int m_WindDataCopyHistoryKernel;

		private ComputeBuffer m_UpdateIndexQueueBuffer;

		private ComputeBuffer m_ProbeUpdateDataQueueBuffer;

		private ComputeBuffer m_ProbeOcclusionUpdateDataQueueBuffer;

		private ComputeBuffer m_TransformUpdateDataQueueBuffer;

		private ComputeBuffer m_BoundingSpheresUpdateDataQueueBuffer;

		private bool m_EnableBoundingSpheres;

		private readonly int[] m_ScratchWindParamAddressArray = new int[64];

		private static class InstanceTransformUpdateIDs
		{
			public static readonly int _TransformUpdateQueueCount = Shader.PropertyToID("_TransformUpdateQueueCount");

			public static readonly int _TransformUpdateOutputL2WVec4Offset = Shader.PropertyToID("_TransformUpdateOutputL2WVec4Offset");

			public static readonly int _TransformUpdateOutputW2LVec4Offset = Shader.PropertyToID("_TransformUpdateOutputW2LVec4Offset");

			public static readonly int _TransformUpdateOutputPrevL2WVec4Offset = Shader.PropertyToID("_TransformUpdateOutputPrevL2WVec4Offset");

			public static readonly int _TransformUpdateOutputPrevW2LVec4Offset = Shader.PropertyToID("_TransformUpdateOutputPrevW2LVec4Offset");

			public static readonly int _BoundingSphereOutputVec4Offset = Shader.PropertyToID("_BoundingSphereOutputVec4Offset");

			public static readonly int _TransformUpdateDataQueue = Shader.PropertyToID("_TransformUpdateDataQueue");

			public static readonly int _TransformUpdateIndexQueue = Shader.PropertyToID("_TransformUpdateIndexQueue");

			public static readonly int _BoundingSphereDataQueue = Shader.PropertyToID("_BoundingSphereDataQueue");

			public static readonly int _OutputTransformBuffer = Shader.PropertyToID("_OutputTransformBuffer");

			public static readonly int _ProbeUpdateQueueCount = Shader.PropertyToID("_ProbeUpdateQueueCount");

			public static readonly int _SHUpdateVec4Offset = Shader.PropertyToID("_SHUpdateVec4Offset");

			public static readonly int _ProbeUpdateDataQueue = Shader.PropertyToID("_ProbeUpdateDataQueue");

			public static readonly int _ProbeOcclusionUpdateDataQueue = Shader.PropertyToID("_ProbeOcclusionUpdateDataQueue");

			public static readonly int _ProbeUpdateIndexQueue = Shader.PropertyToID("_ProbeUpdateIndexQueue");

			public static readonly int _OutputProbeBuffer = Shader.PropertyToID("_OutputProbeBuffer");
		}

		private static class InstanceWindDataUpdateIDs
		{
			public static readonly int _WindDataQueueCount = Shader.PropertyToID("_WindDataQueueCount");

			public static readonly int _WindDataUpdateIndexQueue = Shader.PropertyToID("_WindDataUpdateIndexQueue");

			public static readonly int _WindDataBuffer = Shader.PropertyToID("_WindDataBuffer");

			public static readonly int _WindParamAddressArray = Shader.PropertyToID("_WindParamAddressArray");

			public static readonly int _WindHistoryParamAddressArray = Shader.PropertyToID("_WindHistoryParamAddressArray");
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct QueryRendererGroupInstancesCountJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				for (int i = startIndex; i < startIndex + count; i++)
				{
					int key = this.rendererGroupIDs[i];
					InstanceHandle instance;
					NativeParallelMultiHashMapIterator<int> nativeParallelMultiHashMapIterator;
					if (this.rendererGroupInstanceMultiHash.TryGetFirstValue(key, out instance, out nativeParallelMultiHashMapIterator))
					{
						SharedInstanceHandle instance2 = this.instanceData.Get_SharedInstance(instance);
						int value = this.sharedInstanceData.Get_RefCount(instance2);
						this.instancesCount[i] = value;
					}
					else
					{
						this.instancesCount[i] = 0;
					}
				}
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public CPUInstanceData instanceData;

			[ReadOnly]
			public CPUSharedInstanceData sharedInstanceData;

			[ReadOnly]
			public NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<int> rendererGroupIDs;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[WriteOnly]
			public NativeArray<int> instancesCount;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct ComputeInstancesOffsetAndResizeInstancesArrayJob : IJob
		{
			public void Execute()
			{
				int num = 0;
				for (int i = 0; i < this.instancesCount.Length; i++)
				{
					this.instancesOffset[i] = num;
					num += this.instancesCount[i];
				}
				this.instances.ResizeUninitialized(num);
			}

			[ReadOnly]
			public NativeArray<int> instancesCount;

			[WriteOnly]
			public NativeArray<int> instancesOffset;

			public NativeList<InstanceHandle> instances;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct QueryRendererGroupInstancesJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				int num = 0;
				for (int i = startIndex; i < startIndex + count; i++)
				{
					InstanceHandle value;
					NativeParallelMultiHashMapIterator<int> nativeParallelMultiHashMapIterator;
					if (this.rendererGroupInstanceMultiHash.TryGetFirstValue(this.rendererGroupIDs[i], out value, out nativeParallelMultiHashMapIterator))
					{
						this.instances[i] = value;
					}
					else
					{
						num++;
						this.instances[i] = InstanceHandle.Invalid;
					}
				}
				if (this.atomicNonFoundInstancesCount.Counter != null && num > 0)
				{
					this.atomicNonFoundInstancesCount.Add(num);
				}
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<int> rendererGroupIDs;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[WriteOnly]
			public NativeArray<InstanceHandle> instances;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicNonFoundInstancesCount;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct QueryRendererGroupInstancesMultiJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				int num = 0;
				int num2 = 0;
				for (int i = startIndex; i < startIndex + count; i++)
				{
					int key = this.rendererGroupIDs[i];
					int num3 = this.instancesOffsets[i];
					int num4 = this.instancesCounts[i];
					InstanceHandle value;
					NativeParallelMultiHashMapIterator<int> nativeParallelMultiHashMapIterator;
					bool flag = this.rendererGroupInstanceMultiHash.TryGetFirstValue(key, out value, out nativeParallelMultiHashMapIterator);
					if (!flag)
					{
						num++;
					}
					for (int j = 0; j < num4; j++)
					{
						int index = num3 + j;
						if (flag)
						{
							this.instances[index] = value;
							flag = this.rendererGroupInstanceMultiHash.TryGetNextValue(out value, ref nativeParallelMultiHashMapIterator);
						}
						else
						{
							num2++;
							this.instances[index] = InstanceHandle.Invalid;
						}
					}
				}
				if (this.atomicNonFoundSharedInstancesCount.Counter != null && num > 0)
				{
					this.atomicNonFoundSharedInstancesCount.Add(num);
				}
				if (this.atomicNonFoundInstancesCount.Counter != null && num2 > 0)
				{
					this.atomicNonFoundInstancesCount.Add(num2);
				}
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public NativeParallelMultiHashMap<int, InstanceHandle> rendererGroupInstanceMultiHash;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<int> rendererGroupIDs;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<int> instancesOffsets;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<int> instancesCounts;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[WriteOnly]
			public NativeArray<InstanceHandle> instances;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicNonFoundSharedInstancesCount;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicNonFoundInstancesCount;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct QuerySortedMeshInstancesJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				ulong num = 0UL;
				for (int i = 0; i < count; i++)
				{
					int index = startIndex + i;
					InstanceHandle instanceHandle = this.instanceData.instances[index];
					SharedInstanceHandle instance = this.instanceData.sharedInstances[index];
					int value = this.sharedInstanceData.Get_MeshID(instance);
					if (this.sortedMeshID.BinarySearch(value) >= 0)
					{
						num |= 1UL << i;
					}
				}
				int num2 = math.countbits(num);
				if (num2 > 0)
				{
					int num3 = InstanceDataSystem.AtomicAddLengthNoResize<InstanceHandle>(this.instances, num2);
					int num4 = math.tzcnt(num);
					while (num != 0UL)
					{
						int index2 = startIndex + num4;
						this.instances[num3] = this.instanceData.instances[index2];
						num3++;
						num &= ~(1UL << num4);
						num4 = math.tzcnt(num);
					}
				}
			}

			public const int k_BatchSize = 64;

			[ReadOnly]
			public CPUInstanceData instanceData;

			[ReadOnly]
			public CPUSharedInstanceData sharedInstanceData;

			[ReadOnly]
			public NativeArray<int> sortedMeshID;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeList<InstanceHandle> instances;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct CalculateInterpolatedLightAndOcclusionProbesBatchJob : IJobParallelFor
		{
			public void Execute(int index)
			{
				int num = index * 8;
				int length = math.min(this.probesCount, num + 8) - num;
				NativeArray<int> subArray = this.compactTetrahedronCache.GetSubArray(num, length);
				NativeArray<Vector3> subArray2 = this.queryPostitions.GetSubArray(num, length);
				NativeArray<SphericalHarmonicsL2> subArray3 = this.probesSphericalHarmonics.GetSubArray(num, length);
				NativeArray<Vector4> subArray4 = this.probesOcclusion.GetSubArray(num, length);
				this.lightProbesQuery.CalculateInterpolatedLightAndOcclusionProbes(subArray2, subArray, subArray3, subArray4);
			}

			public const int k_BatchSize = 1;

			public const int k_CalculatedProbesPerBatch = 8;

			[ReadOnly]
			public int probesCount;

			[ReadOnly]
			public LightProbesQuery lightProbesQuery;

			[NativeDisableParallelForRestriction]
			[ReadOnly]
			public NativeArray<Vector3> queryPostitions;

			[NativeDisableParallelForRestriction]
			public NativeArray<int> compactTetrahedronCache;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<SphericalHarmonicsL2> probesSphericalHarmonics;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<Vector4> probesOcclusion;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct ScatterTetrahedronCacheIndicesJob : IJobParallelFor
		{
			public void Execute(int index)
			{
				InstanceHandle instance = this.probeInstances[index];
				this.instanceData.Set_TetrahedronCacheIndex(instance, this.compactTetrahedronCache[index]);
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public NativeArray<InstanceHandle> probeInstances;

			[ReadOnly]
			public NativeArray<int> compactTetrahedronCache;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[NativeDisableParallelForRestriction]
			public CPUInstanceData instanceData;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct TransformUpdateJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				ulong num = 0UL;
				for (int i = 0; i < count; i++)
				{
					InstanceHandle instance = this.instances[startIndex + i];
					if (instance.valid)
					{
						if (!this.initialize)
						{
							int index = this.instanceData.InstanceToIndex(instance);
							int index2 = this.sharedInstanceData.InstanceToIndex(this.instanceData, instance);
							int transformUpdateFlags = (int)this.sharedInstanceData.flags[index2].transformUpdateFlags;
							bool flag = this.instanceData.movedInCurrentFrameBits.Get(index);
							if ((transformUpdateFlags & 2) != 0 || flag)
							{
								goto IL_8B;
							}
						}
						num |= 1UL << i;
					}
					IL_8B:;
				}
				int num2 = math.countbits(num);
				if (num2 > 0)
				{
					int num3 = this.atomicTransformQueueCount.Add(num2);
					int num4 = math.tzcnt(num);
					while (num != 0UL)
					{
						int index3 = startIndex + num4;
						InstanceHandle instanceHandle = this.instances[index3];
						int index4 = this.instanceData.InstanceToIndex(instanceHandle);
						int index5 = this.sharedInstanceData.InstanceToIndex(this.instanceData, instanceHandle);
						bool flag2 = (this.sharedInstanceData.flags[index5].transformUpdateFlags & TransformUpdateFlags.IsPartOfStaticBatch) > TransformUpdateFlags.None;
						this.instanceData.movedInCurrentFrameBits.Set(index4, !flag2);
						this.transformUpdateInstanceQueue[num3] = instanceHandle;
						ref float4x4 ptr = ref UnsafeUtility.ArrayElementAsRef<float4x4>(this.localToWorldMatrices.GetUnsafeReadOnlyPtr<Matrix4x4>(), index3);
						ref AABB ptr2 = ref UnsafeUtility.ArrayElementAsRef<AABB>(this.sharedInstanceData.localAABBs.GetUnsafePtr<AABB>(), index5);
						AABB aabb = AABB.Transform(ptr, ptr2);
						this.instanceData.worldAABBs[index4] = aabb;
						if (this.initialize)
						{
							PackedMatrix packedMatrix = PackedMatrix.FromFloat4x4(ptr);
							Matrix4x4 matrix4x = this.prevLocalToWorldMatrices[index3];
							PackedMatrix packedMatrix2 = PackedMatrix.FromMatrix4x4(matrix4x);
							this.transformUpdateDataQueue[num3 * 2] = new TransformUpdatePacket
							{
								localToWorld0 = packedMatrix.packed0,
								localToWorld1 = packedMatrix.packed1,
								localToWorld2 = packedMatrix.packed2
							};
							this.transformUpdateDataQueue[num3 * 2 + 1] = new TransformUpdatePacket
							{
								localToWorld0 = packedMatrix2.packed0,
								localToWorld1 = packedMatrix2.packed1,
								localToWorld2 = packedMatrix2.packed2
							};
						}
						else
						{
							Matrix4x4 matrix4x = ptr;
							PackedMatrix packedMatrix3 = PackedMatrix.FromMatrix4x4(matrix4x);
							this.transformUpdateDataQueue[num3] = new TransformUpdatePacket
							{
								localToWorld0 = packedMatrix3.packed0,
								localToWorld1 = packedMatrix3.packed1,
								localToWorld2 = packedMatrix3.packed2
							};
							float num5 = math.determinant((float3x3)ptr);
							this.instanceData.localToWorldIsFlippedBits.Set(index4, num5 < 0f);
						}
						if (this.enableBoundingSpheres)
						{
							this.boundingSpheresDataQueue[num3] = new float4(aabb.center.x, aabb.center.y, aabb.center.z, math.distance(aabb.max, aabb.min) * 0.5f);
						}
						num3++;
						num &= ~(1UL << num4);
						num4 = math.tzcnt(num);
					}
				}
			}

			public const int k_BatchSize = 64;

			[ReadOnly]
			public bool initialize;

			[ReadOnly]
			public bool enableBoundingSpheres;

			[ReadOnly]
			public NativeArray<InstanceHandle> instances;

			[ReadOnly]
			public NativeArray<Matrix4x4> localToWorldMatrices;

			[ReadOnly]
			public NativeArray<Matrix4x4> prevLocalToWorldMatrices;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicTransformQueueCount;

			[NativeDisableParallelForRestriction]
			public CPUSharedInstanceData sharedInstanceData;

			[NativeDisableParallelForRestriction]
			public CPUInstanceData instanceData;

			[NativeDisableParallelForRestriction]
			public NativeArray<InstanceHandle> transformUpdateInstanceQueue;

			[NativeDisableParallelForRestriction]
			public NativeArray<TransformUpdatePacket> transformUpdateDataQueue;

			[NativeDisableParallelForRestriction]
			public NativeArray<float4> boundingSpheresDataQueue;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct ProbesUpdateJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				ulong num = 0UL;
				for (int i = 0; i < count; i++)
				{
					InstanceHandle instance = this.instances[startIndex + i];
					if (instance.valid)
					{
						int index = this.sharedInstanceData.InstanceToIndex(this.instanceData, instance);
						if ((this.sharedInstanceData.flags[index].transformUpdateFlags & TransformUpdateFlags.HasLightProbeCombined) > TransformUpdateFlags.None)
						{
							num |= 1UL << i;
						}
					}
				}
				int num2 = math.countbits(num);
				if (num2 > 0)
				{
					int num3 = this.atomicProbesQueueCount.Add(num2);
					int num4 = math.tzcnt(num);
					while (num != 0UL)
					{
						InstanceHandle instanceHandle = this.instances[startIndex + num4];
						int index2 = this.instanceData.InstanceToIndex(instanceHandle);
						ref AABB ptr = ref UnsafeUtility.ArrayElementAsRef<AABB>(this.instanceData.worldAABBs.GetUnsafePtr<AABB>(), index2);
						this.probeInstanceQueue[num3] = instanceHandle;
						this.probeQueryPosition[num3] = ptr.center;
						this.compactTetrahedronCache[num3] = this.instanceData.tetrahedronCacheIndices[index2];
						num3++;
						num &= ~(1UL << num4);
						num4 = math.tzcnt(num);
					}
				}
			}

			public const int k_BatchSize = 64;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<InstanceHandle> instances;

			[NativeDisableParallelForRestriction]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public CPUInstanceData instanceData;

			[ReadOnly]
			public CPUSharedInstanceData sharedInstanceData;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicProbesQueueCount;

			[NativeDisableParallelForRestriction]
			public NativeArray<InstanceHandle> probeInstanceQueue;

			[NativeDisableParallelForRestriction]
			public NativeArray<int> compactTetrahedronCache;

			[NativeDisableParallelForRestriction]
			public NativeArray<Vector3> probeQueryPosition;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct MotionUpdateJob : IJobParallelFor
		{
			public void Execute(int chunk_index)
			{
				int num = math.min(this.instanceData.instancesLength - 64 * chunk_index, 64);
				ulong num2 = ulong.MaxValue >> 64 - num;
				ulong num3 = this.instanceData.movedInCurrentFrameBits.GetChunk(chunk_index) & num2;
				ulong num4 = this.instanceData.movedInPreviousFrameBits.GetChunk(chunk_index) & num2;
				this.instanceData.movedInCurrentFrameBits.SetChunk(chunk_index, 0UL);
				this.instanceData.movedInPreviousFrameBits.SetChunk(chunk_index, num3);
				ulong num5 = num4 & ~num3;
				int num6 = math.countbits(num5);
				int num7 = this.queueWriteBase;
				if (num6 > 0)
				{
					num7 += this.atomicUpdateQueueCount.Add(num6);
				}
				for (int i = math.tzcnt(num5); i < 64; i = math.tzcnt(num5))
				{
					int index = 64 * chunk_index + i;
					this.transformUpdateInstanceQueue[num7] = this.instanceData.IndexToInstance(index);
					num7++;
					num5 &= ~(1UL << i);
				}
			}

			public const int k_BatchSize = 16;

			[ReadOnly]
			public int queueWriteBase;

			[NativeDisableParallelForRestriction]
			public CPUInstanceData instanceData;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicUpdateQueueCount;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<InstanceHandle> transformUpdateInstanceQueue;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct UpdateRendererInstancesJob : IJobParallelFor
		{
			public void Execute(int index)
			{
				int rendererGroupID = this.rendererData.rendererGroupID[index];
				int index2 = this.rendererData.meshIndex[index];
				GPUDrivenPackedRendererData gpudrivenPackedRendererData = this.rendererData.packedRendererData[index];
				int key = this.rendererData.lodGroupID[index];
				int gameObjectLayer = this.rendererData.gameObjectLayer[index];
				int num = this.rendererData.lightmapIndex[index];
				AABB localBounds = this.rendererData.localBounds[index].ToAABB();
				int num2 = this.rendererData.materialsOffset[index];
				int num3 = (int)this.rendererData.materialsCount[index];
				int meshID = this.rendererData.meshID[index2];
				GPUDrivenMeshLodInfo meshLodInfo = this.rendererData.meshLodInfo[index2];
				InstanceFlags instanceFlags = InstanceFlags.None;
				TransformUpdateFlags transformUpdateFlags = TransformUpdateFlags.None;
				int num4 = num & 65535;
				if (num4 >= 65534 && gpudrivenPackedRendererData.lightProbeUsage == LightProbeUsage.BlendProbes)
				{
					transformUpdateFlags |= TransformUpdateFlags.HasLightProbeCombined;
				}
				if (gpudrivenPackedRendererData.isPartOfStaticBatch)
				{
					transformUpdateFlags |= TransformUpdateFlags.IsPartOfStaticBatch;
				}
				ShadowCastingMode shadowCastingMode = gpudrivenPackedRendererData.shadowCastingMode;
				if (shadowCastingMode != ShadowCastingMode.Off)
				{
					if (shadowCastingMode == ShadowCastingMode.ShadowsOnly)
					{
						instanceFlags |= InstanceFlags.IsShadowsOnly;
					}
				}
				else
				{
					instanceFlags |= InstanceFlags.IsShadowsOff;
				}
				if (meshLodInfo.lodSelectionActive)
				{
					instanceFlags |= InstanceFlags.HasMeshLod;
				}
				if (num4 != 65535)
				{
					instanceFlags |= InstanceFlags.AffectsLightmaps;
				}
				if (gpudrivenPackedRendererData.smallMeshCulling)
				{
					instanceFlags |= InstanceFlags.SmallMeshCulling;
				}
				uint lodGroupAndMask = uint.MaxValue;
				GPUInstanceIndex gpuinstanceIndex;
				if (this.lodGroupDataMap.TryGetValue(key, out gpuinstanceIndex) && gpudrivenPackedRendererData.lodMask > 0)
				{
					lodGroupAndMask = (uint)(gpuinstanceIndex.index << 8 | (int)gpudrivenPackedRendererData.lodMask);
				}
				int num5;
				int num6;
				if (this.implicitInstanceIndices)
				{
					num5 = 1;
					num6 = index;
				}
				else
				{
					num5 = this.rendererData.instancesCount[index];
					num6 = this.rendererData.instancesOffset[index];
				}
				if (num5 > 0)
				{
					InstanceHandle instance = this.instances[num6];
					SharedInstanceHandle instance2 = this.instanceData.Get_SharedInstance(instance);
					SmallIntegerArray smallIntegerArray = new SmallIntegerArray(num3, Allocator.Persistent);
					for (int i = 0; i < num3; i++)
					{
						int index3 = this.rendererData.materialIndex[num2 + i];
						int value = this.rendererData.materialID[index3];
						smallIntegerArray[i] = value;
					}
					this.sharedInstanceData.Set(instance2, rendererGroupID, smallIntegerArray, meshID, localBounds, transformUpdateFlags, instanceFlags, lodGroupAndMask, meshLodInfo, gameObjectLayer, this.sharedInstanceData.Get_RefCount(instance2));
					for (int j = 0; j < num5; j++)
					{
						int index4 = num6 + j;
						ref Matrix4x4 ptr = ref UnsafeUtility.ArrayElementAsRef<Matrix4x4>(this.rendererData.localToWorldMatrix.GetUnsafeReadOnlyPtr<Matrix4x4>(), index4);
						AABB value2 = AABB.Transform(ptr, localBounds);
						instance = this.instances[index4];
						bool value3 = math.determinant((float3x3)ptr) < 0f;
						int num7 = this.instanceData.InstanceToIndex(instance);
						this.perCameraInstanceData.SetDefault(num7);
						this.instanceData.localToWorldIsFlippedBits.Set(num7, value3);
						this.instanceData.worldAABBs[num7] = value2;
						this.instanceData.tetrahedronCacheIndices[num7] = -1;
						this.instanceData.meshLodData[num7] = this.rendererData.meshLodData[index];
					}
				}
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public bool implicitInstanceIndices;

			[ReadOnly]
			public GPUDrivenRendererGroupData rendererData;

			[ReadOnly]
			public NativeArray<InstanceHandle> instances;

			[ReadOnly]
			public NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataMap;

			[NativeDisableParallelForRestriction]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public CPUInstanceData instanceData;

			[NativeDisableParallelForRestriction]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public CPUSharedInstanceData sharedInstanceData;

			[NativeDisableParallelForRestriction]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public CPUPerCameraInstanceData perCameraInstanceData;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct CollectInstancesLODGroupsAndMasksJob : IJobParallelFor
		{
			public void Execute(int index)
			{
				InstanceHandle instance = this.instances[index];
				int index2 = this.sharedInstanceData.InstanceToIndex(this.instanceData, instance);
				this.lodGroupAndMasks[index] = this.sharedInstanceData.lodGroupAndMasks[index2];
			}

			public const int k_BatchSize = 128;

			[ReadOnly]
			public NativeArray<InstanceHandle> instances;

			[ReadOnly]
			public CPUInstanceData.ReadOnly instanceData;

			[ReadOnly]
			public CPUSharedInstanceData.ReadOnly sharedInstanceData;

			[WriteOnly]
			public NativeArray<uint> lodGroupAndMasks;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct GetVisibleNonProcessedTreeInstancesJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				int chunk_index = startIndex / 64;
				ulong chunk = this.instanceData.visibleInPreviousFrameBits.GetChunk(chunk_index);
				ulong chunk2 = this.processedBits.GetChunk(chunk_index);
				ulong num = 0UL;
				for (int i = 0; i < count; i++)
				{
					int index = startIndex + i;
					InstanceHandle instanceHandle = this.instanceData.IndexToInstance(index);
					if (instanceHandle.type == InstanceType.SpeedTree && this.compactedVisibilityMasks.Get(instanceHandle.index))
					{
						ulong num2 = 1UL << i;
						if ((chunk2 & num2) <= 0UL)
						{
							bool flag = (chunk & num2) > 0UL;
							if (this.becomeVisible)
							{
								if (!flag)
								{
									num |= num2;
								}
							}
							else if (flag)
							{
								num |= num2;
							}
						}
					}
				}
				int num3 = math.countbits(num);
				if (num3 > 0)
				{
					this.processedBits.SetChunk(chunk_index, chunk2 | num);
					int num4 = this.atomicTreeInstancesCount.Add(num3);
					int num5 = math.tzcnt(num);
					while (num != 0UL)
					{
						int index2 = startIndex + num5;
						InstanceHandle instanceHandle2 = this.instanceData.IndexToInstance(index2);
						SharedInstanceHandle instance = this.instanceData.Get_SharedInstance(instanceHandle2);
						int value = this.sharedInstanceData.Get_RendererGroupID(instance);
						this.rendererIDs[num4] = value;
						this.instances[num4] = instanceHandle2;
						num4++;
						num &= ~(1UL << num5);
						num5 = math.tzcnt(num);
					}
				}
			}

			public const int k_BatchSize = 64;

			[ReadOnly]
			public CPUInstanceData instanceData;

			[ReadOnly]
			public CPUSharedInstanceData sharedInstanceData;

			[ReadOnly]
			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			public ParallelBitArray compactedVisibilityMasks;

			[ReadOnly]
			public bool becomeVisible;

			[NativeDisableParallelForRestriction]
			public ParallelBitArray processedBits;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<int> rendererIDs;

			[NativeDisableParallelForRestriction]
			[WriteOnly]
			public NativeArray<InstanceHandle> instances;

			[NativeDisableUnsafePtrRestriction]
			public UnsafeAtomicCounter32 atomicTreeInstancesCount;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct UpdateCompactedInstanceVisibilityJob : IJobParallelForBatch
		{
			public void Execute(int startIndex, int count)
			{
				ulong num = 0UL;
				for (int i = 0; i < count; i++)
				{
					int index = startIndex + i;
					if (this.compactedVisibilityMasks.Get(this.instanceData.IndexToInstance(index).index))
					{
						num |= 1UL << i;
					}
				}
				this.instanceData.visibleInPreviousFrameBits.SetChunk(startIndex / 64, num);
			}

			public const int k_BatchSize = 64;

			[ReadOnly]
			public ParallelBitArray compactedVisibilityMasks;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[NativeDisableParallelForRestriction]
			public CPUInstanceData instanceData;
		}
	}
}
