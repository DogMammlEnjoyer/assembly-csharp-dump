using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal class ResourcesData
	{
		public ResourcesData()
		{
			this.unversionedData = new NativeList<ResourceUnversionedData>[3];
			this.versionedData = new NativeList<ResourceVersionedData>[3];
			this.readerData = new NativeList<ResourceReaderData>[3];
			this.resourceNames = new DynamicArray<Name>[3];
			this.MaxVersions = new int[3];
			this.MaxReaders = new int[3];
			for (int i = 0; i < 3; i++)
			{
				this.resourceNames[i] = new DynamicArray<Name>(0);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.unversionedData[i].IsCreated)
				{
					this.unversionedData[i].Clear();
				}
				if (this.versionedData[i].IsCreated)
				{
					this.versionedData[i].Clear();
				}
				if (this.readerData[i].IsCreated)
				{
					this.readerData[i].Clear();
				}
				this.resourceNames[i].Clear();
			}
		}

		private void AllocateAndResizeNativeListIfNeeded<[IsUnmanaged] T>(ref NativeList<T> nativeList, int size, NativeArrayOptions options) where T : struct, ValueType
		{
			if (!nativeList.IsCreated)
			{
				nativeList = new NativeList<T>(size, AllocatorManager.Persistent);
			}
			nativeList.Resize(size, options);
		}

		public unsafe void Initialize(RenderGraphResourceRegistry resources)
		{
			for (int i = 0; i < 3; i++)
			{
				RenderGraphResourceType type = (RenderGraphResourceType)i;
				int resourceCount = resources.GetResourceCount(type);
				uint num = 0U;
				uint num2 = 0U;
				this.AllocateAndResizeNativeListIfNeeded<ResourceUnversionedData>(ref this.unversionedData[i], resourceCount, NativeArrayOptions.UninitializedMemory);
				this.resourceNames[i].Resize(resourceCount, true);
				if (resourceCount > 0)
				{
					ResourceUnversionedData value = default(ResourceUnversionedData);
					value.InitializeNullResource();
					this.unversionedData[i][0] = value;
					*this.resourceNames[i][0] = new Name("", false);
				}
				for (int j = 1; j < resourceCount; j++)
				{
					ResourceHandle resourceHandle = new ResourceHandle(j, type, false);
					IRenderGraphResource resourceLowLevel = resources.GetResourceLowLevel(resourceHandle);
					*this.resourceNames[i][j] = new Name(resourceLowLevel.GetName(), false);
					switch (i)
					{
					case 0:
					{
						RenderTargetInfo renderTargetInfo;
						resources.GetRenderTargetInfo(resourceHandle, out renderTargetInfo);
						ref TextureDesc desc = ref (resourceLowLevel as TextureResource).desc;
						bool isResourceShared = resources.IsRenderGraphResourceShared(resourceHandle);
						this.unversionedData[i][j] = new ResourceUnversionedData(resourceLowLevel, ref renderTargetInfo, ref desc, isResourceShared);
						break;
					}
					case 1:
					{
						ref BufferDesc _ = ref (resourceLowLevel as BufferResource).desc;
						bool isResourceShared2 = resources.IsRenderGraphResourceShared(resourceHandle);
						this.unversionedData[i][j] = new ResourceUnversionedData(resourceLowLevel, ref _, isResourceShared2);
						break;
					}
					case 2:
					{
						ref RayTracingAccelerationStructureDesc _2 = ref (resourceLowLevel as RayTracingAccelerationStructureResource).desc;
						bool isResourceShared3 = resources.IsRenderGraphResourceShared(resourceHandle);
						this.unversionedData[i][j] = new ResourceUnversionedData(resourceLowLevel, ref _2, isResourceShared3);
						break;
					}
					default:
						throw new Exception("Unsupported resource type: " + i.ToString());
					}
					num = Math.Max(num, resourceLowLevel.readCount);
					num2 = Math.Max(num2, resourceLowLevel.writeCount);
				}
				this.MaxReaders[i] = (int)(num + 1U);
				this.MaxVersions[i] = (int)(num2 + 1U);
				this.AllocateAndResizeNativeListIfNeeded<ResourceVersionedData>(ref this.versionedData[i], this.MaxVersions[i] * resourceCount, NativeArrayOptions.ClearMemory);
				this.AllocateAndResizeNativeListIfNeeded<ResourceReaderData>(ref this.readerData[i], this.MaxVersions[i] * this.MaxReaders[i] * resourceCount, NativeArrayOptions.ClearMemory);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Index(ResourceHandle h)
		{
			return h.index * this.MaxVersions[h.iType] + h.version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexReader(ResourceHandle h, int readerID)
		{
			return (h.index * this.MaxVersions[h.iType] + h.version) * this.MaxReaders[h.iType] + readerID;
		}

		public ResourceVersionedData this[ResourceHandle h]
		{
			get
			{
				return this.versionedData[h.iType].ElementAt(this.Index(h));
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < 3; i++)
			{
				if (this.versionedData[i].IsCreated)
				{
					this.versionedData[i].Dispose();
				}
				if (this.unversionedData[i].IsCreated)
				{
					this.unversionedData[i].Dispose();
				}
				if (this.readerData[i].IsCreated)
				{
					this.readerData[i].Dispose();
				}
			}
		}

		public NativeList<ResourceUnversionedData>[] unversionedData;

		public NativeList<ResourceVersionedData>[] versionedData;

		public NativeList<ResourceReaderData>[] readerData;

		public int[] MaxVersions;

		public int[] MaxReaders;

		public DynamicArray<Name>[] resourceNames;
	}
}
