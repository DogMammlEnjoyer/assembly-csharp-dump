using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal struct GPUInstanceDataBufferUploader : IDisposable
	{
		public GPUInstanceDataBufferUploader(in NativeArray<GPUInstanceComponentDesc> descriptions, int capacity, InstanceType instanceType)
		{
			this.m_Capacity = capacity;
			this.m_InstanceCount = 0;
			this.m_UintPerInstance = 0;
			NativeArray<GPUInstanceComponentDesc> nativeArray = descriptions;
			this.m_ComponentDataIndex = new NativeArray<int>(nativeArray.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			nativeArray = descriptions;
			this.m_ComponentIsInstanced = new NativeArray<bool>(nativeArray.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			nativeArray = descriptions;
			this.m_DescriptionsUintSize = new NativeArray<int>(nativeArray.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			nativeArray = descriptions;
			this.m_WritenComponentIndices = new NativeList<int>(nativeArray.Length, Allocator.TempJob);
			this.m_DummyArray = new NativeArray<int>(0, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num = UnsafeUtility.SizeOf<uint>();
			int num2 = 0;
			for (;;)
			{
				int num3 = num2;
				nativeArray = descriptions;
				if (num3 >= nativeArray.Length)
				{
					break;
				}
				nativeArray = descriptions;
				GPUInstanceComponentDesc gpuinstanceComponentDesc = nativeArray[num2];
				this.m_ComponentIsInstanced[num2] = gpuinstanceComponentDesc.isPerInstance;
				if (gpuinstanceComponentDesc.instanceType == instanceType)
				{
					this.m_ComponentDataIndex[num2] = this.m_UintPerInstance;
					int index = num2;
					nativeArray = descriptions;
					this.m_DescriptionsUintSize[index] = nativeArray[num2].byteSize / num;
					this.m_UintPerInstance += (gpuinstanceComponentDesc.isPerInstance ? (gpuinstanceComponentDesc.byteSize / num) : 0);
				}
				else
				{
					this.m_ComponentDataIndex[num2] = -1;
					this.m_DescriptionsUintSize[num2] = 0;
				}
				num2++;
			}
			this.m_TmpDataBuffer = new NativeArray<uint>(this.m_Capacity * this.m_UintPerInstance, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
		}

		public IntPtr GetUploadBufferPtr()
		{
			return new IntPtr(this.m_TmpDataBuffer.GetUnsafePtr<uint>());
		}

		public int GetUIntPerInstance()
		{
			return this.m_UintPerInstance;
		}

		public int GetParamUIntOffset(int parameterIndex)
		{
			return this.m_ComponentDataIndex[parameterIndex];
		}

		public int PrepareParamWrite<[IsUnmanaged] T>(int parameterIndex) where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>() / UnsafeUtility.SizeOf<uint>();
			if (!this.m_WritenComponentIndices.Contains(parameterIndex))
			{
				this.m_WritenComponentIndices.Add(parameterIndex);
			}
			return this.GetParamUIntOffset(parameterIndex);
		}

		public void AllocateUploadHandles(int handlesLength)
		{
			this.m_InstanceCount = handlesLength;
		}

		public JobHandle WriteInstanceDataJob<[IsUnmanaged] T>(int parameterIndex, NativeArray<T> instanceData) where T : struct, ValueType
		{
			return this.WriteInstanceDataJob<T>(parameterIndex, instanceData, this.m_DummyArray);
		}

		public JobHandle WriteInstanceDataJob<[IsUnmanaged] T>(int parameterIndex, NativeArray<T> instanceData, NativeArray<int> gatherIndices) where T : struct, ValueType
		{
			if (this.m_InstanceCount == 0)
			{
				return default(JobHandle);
			}
			bool gatherData = gatherIndices.Length != 0;
			int uintPerParameter = UnsafeUtility.SizeOf<T>() / UnsafeUtility.SizeOf<uint>();
			if (!this.m_WritenComponentIndices.Contains(parameterIndex))
			{
				this.m_WritenComponentIndices.Add(parameterIndex);
			}
			return new GPUInstanceDataBufferUploader.WriteInstanceDataParameterJob
			{
				gatherData = gatherData,
				gatherIndices = gatherIndices,
				parameterIndex = parameterIndex,
				uintPerParameter = uintPerParameter,
				uintPerInstance = this.m_UintPerInstance,
				componentDataIndex = this.m_ComponentDataIndex,
				instanceData = instanceData.Reinterpret<uint>(UnsafeUtility.SizeOf<T>()),
				tmpDataBuffer = this.m_TmpDataBuffer
			}.Schedule(this.m_InstanceCount, 512, default(JobHandle));
		}

		public void SubmitToGpu(GPUInstanceDataBuffer instanceDataBuffer, NativeArray<GPUInstanceIndex> gpuInstanceIndices, ref GPUInstanceDataBufferUploader.GPUResources gpuResources, bool submitOnlyWrittenParams)
		{
			if (this.m_InstanceCount == 0)
			{
				return;
			}
			instanceDataBuffer.version++;
			int num = UnsafeUtility.SizeOf<uint>();
			int num2 = this.m_UintPerInstance * num;
			gpuResources.CreateResources(this.m_InstanceCount, num2, this.m_ComponentDataIndex.Length, this.m_WritenComponentIndices.Length);
			gpuResources.instanceData.SetData<uint>(this.m_TmpDataBuffer, 0, 0, this.m_InstanceCount * this.m_UintPerInstance);
			gpuResources.instanceIndices.SetData<GPUInstanceIndex>(gpuInstanceIndices, 0, 0, this.m_InstanceCount);
			gpuResources.inputComponentOffsets.SetData<int>(this.m_ComponentDataIndex, 0, 0, this.m_ComponentDataIndex.Length);
			gpuResources.cs.SetInt(GPUInstanceDataBufferUploader.UploadKernelIDs._InputInstanceCounts, this.m_InstanceCount);
			gpuResources.cs.SetInt(GPUInstanceDataBufferUploader.UploadKernelIDs._InputInstanceByteSize, num2);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputInstanceData, gpuResources.instanceData);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputInstanceIndices, gpuResources.instanceIndices);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputComponentOffsets, gpuResources.inputComponentOffsets);
			if (submitOnlyWrittenParams)
			{
				gpuResources.validComponentIndices.SetData<int>(this.m_WritenComponentIndices.AsArray(), 0, 0, this.m_WritenComponentIndices.Length);
				gpuResources.cs.SetInt(GPUInstanceDataBufferUploader.UploadKernelIDs._InputValidComponentCounts, this.m_WritenComponentIndices.Length);
				gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputValidComponentIndices, gpuResources.validComponentIndices);
			}
			else
			{
				gpuResources.cs.SetInt(GPUInstanceDataBufferUploader.UploadKernelIDs._InputValidComponentCounts, instanceDataBuffer.perInstanceComponentCount);
				gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputValidComponentIndices, instanceDataBuffer.validComponentsIndicesGpuBuffer);
			}
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputComponentAddresses, instanceDataBuffer.componentAddressesGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputComponentByteCounts, instanceDataBuffer.componentByteCountsGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._InputComponentInstanceIndexRanges, instanceDataBuffer.componentInstanceIndexRangesGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferUploader.UploadKernelIDs._OutputBuffer, instanceDataBuffer.gpuBuffer);
			gpuResources.cs.Dispatch(gpuResources.kernelId, (this.m_InstanceCount + 63) / 64, 1, 1);
			this.m_InstanceCount = 0;
			this.m_WritenComponentIndices.Clear();
		}

		public void SubmitToGpu(GPUInstanceDataBuffer instanceDataBuffer, NativeArray<InstanceHandle> instances, ref GPUInstanceDataBufferUploader.GPUResources gpuResources, bool submitOnlyWrittenParams)
		{
			if (this.m_InstanceCount == 0)
			{
				return;
			}
			NativeArray<GPUInstanceIndex> gpuInstanceIndices = new NativeArray<GPUInstanceIndex>(instances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			instanceDataBuffer.CPUInstanceArrayToGPUInstanceArray(instances, gpuInstanceIndices);
			this.SubmitToGpu(instanceDataBuffer, gpuInstanceIndices, ref gpuResources, submitOnlyWrittenParams);
			gpuInstanceIndices.Dispose();
		}

		public void Dispose()
		{
			if (this.m_ComponentDataIndex.IsCreated)
			{
				this.m_ComponentDataIndex.Dispose();
			}
			if (this.m_ComponentIsInstanced.IsCreated)
			{
				this.m_ComponentIsInstanced.Dispose();
			}
			if (this.m_DescriptionsUintSize.IsCreated)
			{
				this.m_DescriptionsUintSize.Dispose();
			}
			if (this.m_TmpDataBuffer.IsCreated)
			{
				this.m_TmpDataBuffer.Dispose();
			}
			if (this.m_WritenComponentIndices.IsCreated)
			{
				this.m_WritenComponentIndices.Dispose();
			}
			if (this.m_DummyArray.IsCreated)
			{
				this.m_DummyArray.Dispose();
			}
		}

		private int m_UintPerInstance;

		private int m_Capacity;

		private int m_InstanceCount;

		private NativeArray<bool> m_ComponentIsInstanced;

		private NativeArray<int> m_ComponentDataIndex;

		private NativeArray<int> m_DescriptionsUintSize;

		private NativeArray<uint> m_TmpDataBuffer;

		private NativeList<int> m_WritenComponentIndices;

		private NativeArray<int> m_DummyArray;

		private static class UploadKernelIDs
		{
			public static readonly int _InputValidComponentCounts = Shader.PropertyToID("_InputValidComponentCounts");

			public static readonly int _InputInstanceCounts = Shader.PropertyToID("_InputInstanceCounts");

			public static readonly int _InputInstanceByteSize = Shader.PropertyToID("_InputInstanceByteSize");

			public static readonly int _InputComponentOffsets = Shader.PropertyToID("_InputComponentOffsets");

			public static readonly int _InputInstanceData = Shader.PropertyToID("_InputInstanceData");

			public static readonly int _InputInstanceIndices = Shader.PropertyToID("_InputInstanceIndices");

			public static readonly int _InputValidComponentIndices = Shader.PropertyToID("_InputValidComponentIndices");

			public static readonly int _InputComponentAddresses = Shader.PropertyToID("_InputComponentAddresses");

			public static readonly int _InputComponentByteCounts = Shader.PropertyToID("_InputComponentByteCounts");

			public static readonly int _InputComponentInstanceIndexRanges = Shader.PropertyToID("_InputComponentInstanceIndexRanges");

			public static readonly int _OutputBuffer = Shader.PropertyToID("_OutputBuffer");
		}

		public struct GPUResources : IDisposable
		{
			public void LoadShaders(GPUResidentDrawerResources resources)
			{
				if (this.cs == null)
				{
					this.cs = resources.instanceDataBufferUploadKernels;
					this.kernelId = this.cs.FindKernel("MainUploadScatterInstances");
				}
			}

			public void CreateResources(int newInstanceCount, int sizePerInstance, int newComponentCounts, int validComponentIndicesCount)
			{
				int num = newInstanceCount * sizePerInstance;
				if (num > this.m_InstanceDataByteSize || this.instanceData == null)
				{
					if (this.instanceData != null)
					{
						this.instanceData.Release();
					}
					this.instanceData = new ComputeBuffer((num + 3) / 4, 4, ComputeBufferType.Raw);
					this.m_InstanceDataByteSize = num;
				}
				if (newInstanceCount > this.m_InstanceCount || this.instanceIndices == null)
				{
					if (this.instanceIndices != null)
					{
						this.instanceIndices.Release();
					}
					this.instanceIndices = new ComputeBuffer(newInstanceCount, 4, ComputeBufferType.Raw);
					this.m_InstanceCount = newInstanceCount;
				}
				if (newComponentCounts > this.m_ComponentCounts || this.inputComponentOffsets == null)
				{
					if (this.inputComponentOffsets != null)
					{
						this.inputComponentOffsets.Release();
					}
					this.inputComponentOffsets = new ComputeBuffer(newComponentCounts, 4, ComputeBufferType.Raw);
					this.m_ComponentCounts = newComponentCounts;
				}
				if (validComponentIndicesCount > this.m_ValidComponentIndicesCount || this.validComponentIndices == null)
				{
					if (this.validComponentIndices != null)
					{
						this.validComponentIndices.Release();
					}
					this.validComponentIndices = new ComputeBuffer(validComponentIndicesCount, 4, ComputeBufferType.Raw);
					this.m_ValidComponentIndicesCount = validComponentIndicesCount;
				}
			}

			public void Dispose()
			{
				this.cs = null;
				if (this.instanceData != null)
				{
					this.instanceData.Release();
				}
				if (this.instanceIndices != null)
				{
					this.instanceIndices.Release();
				}
				if (this.inputComponentOffsets != null)
				{
					this.inputComponentOffsets.Release();
				}
				if (this.validComponentIndices != null)
				{
					this.validComponentIndices.Release();
				}
			}

			public ComputeBuffer instanceData;

			public ComputeBuffer instanceIndices;

			public ComputeBuffer inputComponentOffsets;

			public ComputeBuffer validComponentIndices;

			public ComputeShader cs;

			public int kernelId;

			private int m_InstanceDataByteSize;

			private int m_InstanceCount;

			private int m_ComponentCounts;

			private int m_ValidComponentIndicesCount;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		internal struct WriteInstanceDataParameterJob : IJobParallelFor
		{
			public unsafe void Execute(int index)
			{
				int num = (this.gatherData ? this.gatherIndices[index] : index) * this.uintPerParameter;
				int num2 = UnsafeUtility.SizeOf<uint>();
				uint* source = (uint*)((byte*)this.instanceData.GetUnsafePtr<uint>() + (IntPtr)num * 4);
				UnsafeUtility.MemCpy((void*)((byte*)((byte*)this.tmpDataBuffer.GetUnsafePtr<uint>() + (IntPtr)(index * this.uintPerInstance) * 4) + (IntPtr)this.componentDataIndex[this.parameterIndex] * 4), (void*)source, (long)(this.uintPerParameter * num2));
			}

			public const int k_BatchSize = 512;

			[ReadOnly]
			public bool gatherData;

			[ReadOnly]
			public int parameterIndex;

			[ReadOnly]
			public int uintPerParameter;

			[ReadOnly]
			public int uintPerInstance;

			[ReadOnly]
			public NativeArray<int> componentDataIndex;

			[ReadOnly]
			public NativeArray<int> gatherIndices;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[ReadOnly]
			public NativeArray<uint> instanceData;

			[NativeDisableContainerSafetyRestriction]
			[NoAlias]
			[WriteOnly]
			public NativeArray<uint> tmpDataBuffer;
		}
	}
}
