using System;

namespace UnityEngine.Rendering
{
	internal struct GPUInstanceDataBufferGrower : IDisposable
	{
		public unsafe GPUInstanceDataBufferGrower(GPUInstanceDataBuffer sourceBuffer, in InstanceNumInfo instanceNumInfo)
		{
			this.m_SrcBuffer = sourceBuffer;
			this.m_DstBuffer = null;
			bool flag = false;
			for (int i = 0; i < 2; i++)
			{
				if (*(ref instanceNumInfo.InstanceNums.FixedElementField + (IntPtr)i * 4) > *(ref sourceBuffer.instanceNumInfo.InstanceNums.FixedElementField + (IntPtr)i * 4))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			GPUInstanceDataBufferBuilder gpuinstanceDataBufferBuilder = default(GPUInstanceDataBufferBuilder);
			foreach (GPUInstanceComponentDesc gpuinstanceComponentDesc in sourceBuffer.descriptions)
			{
				gpuinstanceDataBufferBuilder.AddComponent(gpuinstanceComponentDesc.propertyID, gpuinstanceComponentDesc.isOverriden, gpuinstanceComponentDesc.byteSize, gpuinstanceComponentDesc.isPerInstance, gpuinstanceComponentDesc.instanceType, gpuinstanceComponentDesc.componentGroup);
			}
			this.m_DstBuffer = gpuinstanceDataBufferBuilder.Build(instanceNumInfo);
			gpuinstanceDataBufferBuilder.Dispose();
		}

		public GPUInstanceDataBuffer SubmitToGpu(ref GPUInstanceDataBufferGrower.GPUResources gpuResources)
		{
			if (this.m_DstBuffer == null)
			{
				return this.m_SrcBuffer;
			}
			if (this.m_SrcBuffer.instanceNumInfo.GetTotalInstanceNum() == 0)
			{
				return this.m_DstBuffer;
			}
			gpuResources.CreateResources();
			gpuResources.cs.SetInt(GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._InputValidComponentCounts, this.m_SrcBuffer.perInstanceComponentCount);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._ValidComponentIndices, this.m_SrcBuffer.validComponentsIndicesGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._ComponentByteCounts, this.m_SrcBuffer.componentByteCountsGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._InputComponentAddresses, this.m_SrcBuffer.componentAddressesGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._InputComponentInstanceIndexRanges, this.m_SrcBuffer.componentInstanceIndexRangesGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._OutputComponentAddresses, this.m_DstBuffer.componentAddressesGpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._InputBuffer, this.m_SrcBuffer.gpuBuffer);
			gpuResources.cs.SetBuffer(gpuResources.kernelId, GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._OutputBuffer, this.m_DstBuffer.gpuBuffer);
			for (int i = 0; i < 2; i++)
			{
				int instanceNum = this.m_SrcBuffer.instanceNumInfo.GetInstanceNum((InstanceType)i);
				if (instanceNum > 0)
				{
					int val = this.m_SrcBuffer.instancesNumPrefixSum[i];
					int val2 = this.m_DstBuffer.instancesNumPrefixSum[i];
					gpuResources.cs.SetInt(GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._InstanceCounts, instanceNum);
					gpuResources.cs.SetInt(GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._InstanceOffset, val);
					gpuResources.cs.SetInt(GPUInstanceDataBufferGrower.CopyInstancesKernelIDs._OutputInstanceOffset, val2);
					gpuResources.cs.Dispatch(gpuResources.kernelId, (instanceNum + 63) / 64, 1, 1);
				}
			}
			return this.m_DstBuffer;
		}

		public void Dispose()
		{
		}

		private GPUInstanceDataBuffer m_SrcBuffer;

		private GPUInstanceDataBuffer m_DstBuffer;

		private static class CopyInstancesKernelIDs
		{
			public static readonly int _InputValidComponentCounts = Shader.PropertyToID("_InputValidComponentCounts");

			public static readonly int _InstanceCounts = Shader.PropertyToID("_InstanceCounts");

			public static readonly int _InstanceOffset = Shader.PropertyToID("_InstanceOffset");

			public static readonly int _OutputInstanceOffset = Shader.PropertyToID("_OutputInstanceOffset");

			public static readonly int _ValidComponentIndices = Shader.PropertyToID("_ValidComponentIndices");

			public static readonly int _ComponentByteCounts = Shader.PropertyToID("_ComponentByteCounts");

			public static readonly int _InputComponentAddresses = Shader.PropertyToID("_InputComponentAddresses");

			public static readonly int _OutputComponentAddresses = Shader.PropertyToID("_OutputComponentAddresses");

			public static readonly int _InputComponentInstanceIndexRanges = Shader.PropertyToID("_InputComponentInstanceIndexRanges");

			public static readonly int _InputBuffer = Shader.PropertyToID("_InputBuffer");

			public static readonly int _OutputBuffer = Shader.PropertyToID("_OutputBuffer");
		}

		public struct GPUResources : IDisposable
		{
			public void LoadShaders(GPUResidentDrawerResources resources)
			{
				if (this.cs == null)
				{
					this.cs = resources.instanceDataBufferCopyKernels;
					this.kernelId = this.cs.FindKernel("MainCopyInstances");
				}
			}

			public void CreateResources()
			{
			}

			public void Dispose()
			{
				this.cs = null;
			}

			public ComputeShader cs;

			public int kernelId;
		}
	}
}
