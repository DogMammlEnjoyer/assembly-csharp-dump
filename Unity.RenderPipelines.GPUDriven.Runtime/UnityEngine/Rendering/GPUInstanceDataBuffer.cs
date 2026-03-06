using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal class GPUInstanceDataBuffer : IDisposable
	{
		public static int NextVersion()
		{
			return ++GPUInstanceDataBuffer.s_NextLayoutVersion;
		}

		public bool valid
		{
			get
			{
				return this.instancesSpan.IsCreated;
			}
		}

		private static GPUInstanceIndex CPUInstanceToGPUInstance(in NativeArray<int> instancesNumPrefixSum, InstanceHandle instance)
		{
			if (!instance.valid || instance.type >= InstanceType.Count)
			{
				return GPUInstanceIndex.Invalid;
			}
			int type = (int)instance.type;
			int instanceIndex = instance.instanceIndex;
			NativeArray<int> nativeArray = instancesNumPrefixSum;
			int index = nativeArray[type] + instanceIndex;
			return new GPUInstanceIndex
			{
				index = index
			};
		}

		public int GetPropertyIndex(int propertyID, bool assertOnFail = true)
		{
			int result;
			if (this.nameToMetadataMap.TryGetValue(propertyID, out result))
			{
				return result;
			}
			return -1;
		}

		public int GetGpuAddress(string strName, bool assertOnFail = true)
		{
			int propertyIndex = this.GetPropertyIndex(Shader.PropertyToID(strName), false);
			if (assertOnFail)
			{
			}
			if (propertyIndex == -1)
			{
				return -1;
			}
			return this.gpuBufferComponentAddress[propertyIndex];
		}

		public int GetGpuAddress(int propertyID, bool assertOnFail = true)
		{
			int propertyIndex = this.GetPropertyIndex(propertyID, assertOnFail);
			if (propertyIndex == -1)
			{
				return -1;
			}
			return this.gpuBufferComponentAddress[propertyIndex];
		}

		public GPUInstanceIndex CPUInstanceToGPUInstance(InstanceHandle instance)
		{
			return GPUInstanceDataBuffer.CPUInstanceToGPUInstance(this.instancesNumPrefixSum, instance);
		}

		public InstanceHandle GPUInstanceToCPUInstance(GPUInstanceIndex gpuInstanceIndex)
		{
			int num = gpuInstanceIndex.index;
			InstanceType instanceType = InstanceType.Count;
			for (int i = 0; i < 2; i++)
			{
				int instanceNum = this.instanceNumInfo.GetInstanceNum((InstanceType)i);
				if (num < instanceNum)
				{
					instanceType = (InstanceType)i;
					break;
				}
				num -= instanceNum;
			}
			if (instanceType == InstanceType.Count)
			{
				return InstanceHandle.Invalid;
			}
			return InstanceHandle.Create(num, instanceType);
		}

		public void CPUInstanceArrayToGPUInstanceArray(NativeArray<InstanceHandle> instances, NativeArray<GPUInstanceIndex> gpuInstanceIndices)
		{
			new GPUInstanceDataBuffer.ConvertCPUInstancesToGPUInstancesJob
			{
				instancesNumPrefixSum = this.instancesNumPrefixSum,
				instances = instances,
				gpuInstanceIndices = gpuInstanceIndices
			}.Schedule(instances.Length, 512, default(JobHandle)).Complete();
		}

		public void Dispose()
		{
			if (this.instancesSpan.IsCreated)
			{
				this.instancesSpan.Dispose();
			}
			if (this.instancesNumPrefixSum.IsCreated)
			{
				this.instancesNumPrefixSum.Dispose();
			}
			if (this.descriptions.IsCreated)
			{
				this.descriptions.Dispose();
			}
			if (this.defaultMetadata.IsCreated)
			{
				this.defaultMetadata.Dispose();
			}
			if (this.gpuBufferComponentAddress.IsCreated)
			{
				this.gpuBufferComponentAddress.Dispose();
			}
			if (this.nameToMetadataMap.IsCreated)
			{
				this.nameToMetadataMap.Dispose();
			}
			if (this.gpuBuffer != null)
			{
				this.gpuBuffer.Release();
			}
			if (this.validComponentsIndicesGpuBuffer != null)
			{
				this.validComponentsIndicesGpuBuffer.Release();
			}
			if (this.componentAddressesGpuBuffer != null)
			{
				this.componentAddressesGpuBuffer.Release();
			}
			if (this.componentInstanceIndexRangesGpuBuffer != null)
			{
				this.componentInstanceIndexRangesGpuBuffer.Release();
			}
			if (this.componentByteCountsGpuBuffer != null)
			{
				this.componentByteCountsGpuBuffer.Release();
			}
		}

		public GPUInstanceDataBuffer.ReadOnly AsReadOnly()
		{
			return new GPUInstanceDataBuffer.ReadOnly(this);
		}

		private static int s_NextLayoutVersion;

		public InstanceNumInfo instanceNumInfo;

		public NativeArray<int> instancesNumPrefixSum;

		public NativeArray<int> instancesSpan;

		public int byteSize;

		public int perInstanceComponentCount;

		public int version;

		public int layoutVersion;

		public GraphicsBuffer gpuBuffer;

		public GraphicsBuffer validComponentsIndicesGpuBuffer;

		public GraphicsBuffer componentAddressesGpuBuffer;

		public GraphicsBuffer componentInstanceIndexRangesGpuBuffer;

		public GraphicsBuffer componentByteCountsGpuBuffer;

		public NativeArray<GPUInstanceComponentDesc> descriptions;

		public NativeArray<MetadataValue> defaultMetadata;

		public NativeArray<int> gpuBufferComponentAddress;

		public NativeParallelHashMap<int, int> nameToMetadataMap;

		internal readonly struct ReadOnly
		{
			public ReadOnly(GPUInstanceDataBuffer buffer)
			{
				this.instancesNumPrefixSum = buffer.instancesNumPrefixSum;
			}

			public GPUInstanceIndex CPUInstanceToGPUInstance(InstanceHandle instance)
			{
				return GPUInstanceDataBuffer.CPUInstanceToGPUInstance(this.instancesNumPrefixSum, instance);
			}

			public void CPUInstanceArrayToGPUInstanceArray(NativeArray<InstanceHandle> instances, NativeArray<GPUInstanceIndex> gpuInstanceIndices)
			{
				new GPUInstanceDataBuffer.ConvertCPUInstancesToGPUInstancesJob
				{
					instancesNumPrefixSum = this.instancesNumPrefixSum,
					instances = instances,
					gpuInstanceIndices = gpuInstanceIndices
				}.Schedule(instances.Length, 512, default(JobHandle)).Complete();
			}

			private readonly NativeArray<int> instancesNumPrefixSum;
		}

		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		private struct ConvertCPUInstancesToGPUInstancesJob : IJobParallelFor
		{
			public void Execute(int index)
			{
				this.gpuInstanceIndices[index] = GPUInstanceDataBuffer.CPUInstanceToGPUInstance(this.instancesNumPrefixSum, this.instances[index]);
			}

			public const int k_BatchSize = 512;

			[ReadOnly]
			public NativeArray<int> instancesNumPrefixSum;

			[ReadOnly]
			public NativeArray<InstanceHandle> instances;

			[WriteOnly]
			public NativeArray<GPUInstanceIndex> gpuInstanceIndices;
		}
	}
}
