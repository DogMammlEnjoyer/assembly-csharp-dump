using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct GPUInstanceDataBufferBuilder : IDisposable
	{
		private MetadataValue CreateMetadataValue(int nameID, int gpuAddress, bool isOverridden)
		{
			return new MetadataValue
			{
				NameID = nameID,
				Value = (uint)(gpuAddress | (isOverridden ? int.MinValue : 0))
			};
		}

		public void AddComponent<[IsUnmanaged] T>(int propertyID, bool isOverriden, bool isPerInstance, InstanceType instanceType, InstanceComponentGroup componentGroup = InstanceComponentGroup.Default) where T : struct, ValueType
		{
			this.AddComponent(propertyID, isOverriden, UnsafeUtility.SizeOf<T>(), isPerInstance, instanceType, componentGroup);
		}

		public void AddComponent(int propertyID, bool isOverriden, int byteSize, bool isPerInstance, InstanceType instanceType, InstanceComponentGroup componentGroup)
		{
			if (!this.m_Components.IsCreated)
			{
				this.m_Components = new NativeList<GPUInstanceComponentDesc>(64, Allocator.Temp);
			}
			int length = this.m_Components.Length;
			GPUInstanceComponentDesc gpuinstanceComponentDesc = new GPUInstanceComponentDesc(propertyID, byteSize, isOverriden, isPerInstance, instanceType, componentGroup);
			this.m_Components.Add(gpuinstanceComponentDesc);
		}

		public unsafe GPUInstanceDataBuffer Build(in InstanceNumInfo instanceNumInfo)
		{
			int num = 0;
			NativeArray<int> data = new NativeArray<int>(this.m_Components.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int> data2 = new NativeArray<int>(this.m_Components.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int> data3 = new NativeArray<int>(this.m_Components.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<Vector2Int> data4 = new NativeArray<Vector2Int>(this.m_Components.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			GPUInstanceDataBuffer gpuinstanceDataBuffer = new GPUInstanceDataBuffer();
			gpuinstanceDataBuffer.instanceNumInfo = instanceNumInfo;
			gpuinstanceDataBuffer.instancesNumPrefixSum = new NativeArray<int>(2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			gpuinstanceDataBuffer.instancesSpan = new NativeArray<int>(2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num2 = 0;
			for (int i = 0; i < 2; i++)
			{
				gpuinstanceDataBuffer.instancesNumPrefixSum[i] = num2;
				num2 += *(ref instanceNumInfo.InstanceNums.FixedElementField + (IntPtr)i * 4);
				GPUInstanceDataBuffer gpuinstanceDataBuffer2 = gpuinstanceDataBuffer;
				int index = i;
				InstanceNumInfo instanceNumInfo2 = instanceNumInfo;
				gpuinstanceDataBuffer2.instancesSpan[index] = instanceNumInfo2.GetInstanceNumIncludingChildren((InstanceType)i);
			}
			gpuinstanceDataBuffer.layoutVersion = GPUInstanceDataBuffer.NextVersion();
			gpuinstanceDataBuffer.version = 0;
			gpuinstanceDataBuffer.defaultMetadata = new NativeArray<MetadataValue>(this.m_Components.Length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			gpuinstanceDataBuffer.descriptions = new NativeArray<GPUInstanceComponentDesc>(this.m_Components.Length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			gpuinstanceDataBuffer.nameToMetadataMap = new NativeParallelHashMap<int, int>(this.m_Components.Length, Allocator.Persistent);
			gpuinstanceDataBuffer.gpuBufferComponentAddress = new NativeArray<int>(this.m_Components.Length, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			int num3 = UnsafeUtility.SizeOf<Vector4>();
			int num4 = 4 * num3;
			for (int j = 0; j < this.m_Components.Length; j++)
			{
				GPUInstanceComponentDesc gpuinstanceComponentDesc = this.m_Components[j];
				gpuinstanceDataBuffer.descriptions[j] = gpuinstanceComponentDesc;
				int num5 = gpuinstanceDataBuffer.instancesNumPrefixSum[(int)gpuinstanceComponentDesc.instanceType];
				int num6 = num5 + gpuinstanceDataBuffer.instancesSpan[(int)gpuinstanceComponentDesc.instanceType];
				int num7 = gpuinstanceComponentDesc.isPerInstance ? (num6 - num5) : 1;
				data4[j] = new Vector2Int(num5, num5 + num7);
				int num8 = num4 - num5 * gpuinstanceComponentDesc.byteSize;
				gpuinstanceDataBuffer.gpuBufferComponentAddress[j] = num8;
				gpuinstanceDataBuffer.defaultMetadata[j] = this.CreateMetadataValue(gpuinstanceComponentDesc.propertyID, num8, gpuinstanceComponentDesc.isOverriden);
				data2[j] = num8;
				data3[j] = gpuinstanceComponentDesc.byteSize;
				int num9 = gpuinstanceComponentDesc.byteSize * num7;
				num4 += num9;
				gpuinstanceDataBuffer.nameToMetadataMap.TryAdd(gpuinstanceComponentDesc.propertyID, j);
				if (gpuinstanceComponentDesc.isPerInstance)
				{
					data[num] = j;
					num++;
				}
			}
			gpuinstanceDataBuffer.byteSize = num4;
			gpuinstanceDataBuffer.gpuBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, gpuinstanceDataBuffer.byteSize / 4, 4);
			gpuinstanceDataBuffer.gpuBuffer.SetData<Vector4>(new NativeArray<Vector4>(4, Allocator.Temp, NativeArrayOptions.ClearMemory), 0, 0, 4);
			gpuinstanceDataBuffer.validComponentsIndicesGpuBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, num, 4);
			gpuinstanceDataBuffer.validComponentsIndicesGpuBuffer.SetData<int>(data, 0, 0, num);
			gpuinstanceDataBuffer.componentAddressesGpuBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, this.m_Components.Length, 4);
			gpuinstanceDataBuffer.componentAddressesGpuBuffer.SetData<int>(data2, 0, 0, this.m_Components.Length);
			gpuinstanceDataBuffer.componentInstanceIndexRangesGpuBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, this.m_Components.Length, 8);
			gpuinstanceDataBuffer.componentInstanceIndexRangesGpuBuffer.SetData<Vector2Int>(data4, 0, 0, this.m_Components.Length);
			gpuinstanceDataBuffer.componentByteCountsGpuBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, this.m_Components.Length, 4);
			gpuinstanceDataBuffer.componentByteCountsGpuBuffer.SetData<int>(data3, 0, 0, this.m_Components.Length);
			gpuinstanceDataBuffer.perInstanceComponentCount = num;
			data.Dispose();
			data2.Dispose();
			data3.Dispose();
			return gpuinstanceDataBuffer;
		}

		public void Dispose()
		{
			if (this.m_Components.IsCreated)
			{
				this.m_Components.Dispose();
			}
		}

		private NativeList<GPUInstanceComponentDesc> m_Components;
	}
}
