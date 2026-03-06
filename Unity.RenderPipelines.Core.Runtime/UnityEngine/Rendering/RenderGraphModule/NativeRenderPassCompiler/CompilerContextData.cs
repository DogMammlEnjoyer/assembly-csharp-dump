using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler
{
	internal class CompilerContextData : IDisposable, RenderGraph.ICompiledGraph
	{
		public CompilerContextData()
		{
			this.fences = new Dictionary<int, GraphicsFence>();
			this.resources = new ResourcesData();
			this.passNames = new DynamicArray<Name>(0, false);
		}

		private void AllocateNativeDataStructuresIfNeeded(int estimatedNumPasses)
		{
			if (!this.m_AreNativeListsAllocated)
			{
				this.passData = new NativeList<PassData>(estimatedNumPasses, AllocatorManager.Persistent);
				this.inputData = new NativeList<PassInputData>(estimatedNumPasses * 2, AllocatorManager.Persistent);
				this.outputData = new NativeList<PassOutputData>(estimatedNumPasses * 2, AllocatorManager.Persistent);
				this.fragmentData = new NativeList<PassFragmentData>(estimatedNumPasses * 4, AllocatorManager.Persistent);
				this.randomAccessResourceData = new NativeList<PassRandomWriteData>(4, AllocatorManager.Persistent);
				this.nativePassData = new NativeList<NativePassData>(estimatedNumPasses, AllocatorManager.Persistent);
				this.nativeSubPassData = new NativeList<SubPassDescriptor>(estimatedNumPasses, AllocatorManager.Persistent);
				this.createData = new NativeList<ResourceHandle>(estimatedNumPasses * 2, AllocatorManager.Persistent);
				this.destroyData = new NativeList<ResourceHandle>(estimatedNumPasses * 2, AllocatorManager.Persistent);
				this.m_AreNativeListsAllocated = true;
			}
		}

		public void Initialize(RenderGraphResourceRegistry resourceRegistry, int estimatedNumPasses)
		{
			this.resources.Initialize(resourceRegistry);
			this.passNames.Reserve(estimatedNumPasses, false);
			this.AllocateNativeDataStructuresIfNeeded(estimatedNumPasses);
		}

		public void Clear()
		{
			this.passNames.Clear();
			this.resources.Clear();
			if (this.m_AreNativeListsAllocated)
			{
				this.passData.Clear();
				this.fences.Clear();
				this.inputData.Clear();
				this.outputData.Clear();
				this.fragmentData.Clear();
				this.randomAccessResourceData.Clear();
				this.nativePassData.Clear();
				this.nativeSubPassData.Clear();
				this.createData.Clear();
				this.destroyData.Clear();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref ResourceUnversionedData UnversionedResourceData(ResourceHandle h)
		{
			return this.resources.unversionedData[h.iType].ElementAt(h.index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref ResourceVersionedData VersionedResourceData(ResourceHandle h)
		{
			return this.resources[h];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<ResourceReaderData> Readers(ResourceHandle h)
		{
			int first = this.resources.IndexReader(h, 0);
			int numReaders = this.resources[h].numReaders;
			return ref this.resources.readerData[h.iType].MakeReadOnlySpan(first, numReaders);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref ResourceReaderData ResourceReader(ResourceHandle h, int i)
		{
			ref ResourceVersionedData ptr = ref this.resources[h];
			return this.resources.readerData[h.iType].ElementAt(this.resources.IndexReader(h, 0) + i);
		}

		public bool AddToFragmentList(TextureAccess access, int listFirstIndex, int numItems)
		{
			for (int i = listFirstIndex; i < listFirstIndex + numItems; i++)
			{
				if (this.fragmentData.ElementAt(i).resource.index == access.textureHandle.handle.index)
				{
					return false;
				}
			}
			PassFragmentData passFragmentData = new PassFragmentData(access.textureHandle.handle, access.flags, access.mipLevel, access.depthSlice);
			this.fragmentData.Add(passFragmentData);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe Name GetFullPassName(int passId)
		{
			return *this.passNames[passId];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetPassName(int passId)
		{
			return this.passNames[passId].name;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetResourceName(ResourceHandle h)
		{
			return this.resources.resourceNames[h.iType][h.index].name;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetResourceVersionedName(ResourceHandle h)
		{
			return this.GetResourceName(h) + " V" + h.version.ToString();
		}

		public bool AddToRandomAccessResourceList(ResourceHandle h, int randomWriteSlotIndex, bool preserveCounterValue, int listFirstIndex, int numItems)
		{
			PassRandomWriteData passRandomWriteData;
			for (int i = listFirstIndex; i < listFirstIndex + numItems; i++)
			{
				passRandomWriteData = this.randomAccessResourceData[i];
				if (passRandomWriteData.resource.index == h.index)
				{
					passRandomWriteData = this.randomAccessResourceData[i];
					if (passRandomWriteData.resource.type == h.type)
					{
						passRandomWriteData = this.randomAccessResourceData[i];
						if (passRandomWriteData.resource.version != h.version)
						{
							throw new Exception("Trying to UseTextureRandomWrite two versions of the same resource");
						}
						return false;
					}
				}
			}
			passRandomWriteData = new PassRandomWriteData(h, randomWriteSlotIndex, preserveCounterValue);
			this.randomAccessResourceData.Add(passRandomWriteData);
			return true;
		}

		public void TagAllPasses(int value)
		{
			for (int i = 0; i < this.passData.Length; i++)
			{
				this.passData.ElementAt(i).tag = value;
			}
		}

		public void CullAllPasses(bool isCulled)
		{
			for (int i = 0; i < this.passData.Length; i++)
			{
				this.passData.ElementAt(i).culled = isCulled;
			}
		}

		public CompilerContextData.NativePassIterator NativePasses
		{
			get
			{
				return new CompilerContextData.NativePassIterator(this);
			}
		}

		internal List<NativePassData> GetNativePasses()
		{
			List<NativePassData> list = new List<NativePassData>();
			foreach (NativePassData ptr in this.NativePasses)
			{
				list.Add(ptr);
			}
			return list;
		}

		~CompilerContextData()
		{
			this.Cleanup();
		}

		public void Dispose()
		{
			this.Cleanup();
			GC.SuppressFinalize(this);
		}

		private void Cleanup()
		{
			this.resources.Dispose();
			if (this.m_AreNativeListsAllocated)
			{
				this.passData.Dispose();
				this.inputData.Dispose();
				this.outputData.Dispose();
				this.fragmentData.Dispose();
				this.createData.Dispose();
				this.destroyData.Dispose();
				this.randomAccessResourceData.Dispose();
				this.nativePassData.Dispose();
				this.nativeSubPassData.Dispose();
				this.m_AreNativeListsAllocated = false;
			}
		}

		public ResourcesData resources;

		public NativeList<PassData> passData;

		public Dictionary<int, GraphicsFence> fences;

		public DynamicArray<Name> passNames;

		public NativeList<PassInputData> inputData;

		public NativeList<PassOutputData> outputData;

		public NativeList<PassFragmentData> fragmentData;

		public NativeList<ResourceHandle> createData;

		public NativeList<ResourceHandle> destroyData;

		public NativeList<PassRandomWriteData> randomAccessResourceData;

		public NativeList<NativePassData> nativePassData;

		public NativeList<SubPassDescriptor> nativeSubPassData;

		private bool m_AreNativeListsAllocated;

		public ref struct NativePassIterator
		{
			public NativePassIterator(CompilerContextData ctx)
			{
				this.m_Ctx = ctx;
				this.m_Index = -1;
			}

			public ref readonly NativePassData Current
			{
				get
				{
					return this.m_Ctx.nativePassData.ElementAt(this.m_Index);
				}
			}

			public bool MoveNext()
			{
				bool flag;
				do
				{
					this.m_Index++;
					flag = (this.m_Index < this.m_Ctx.nativePassData.Length);
				}
				while (flag && !this.m_Ctx.nativePassData.ElementAt(this.m_Index).IsValid());
				return flag;
			}

			public CompilerContextData.NativePassIterator GetEnumerator()
			{
				return this;
			}

			private readonly CompilerContextData m_Ctx;

			private int m_Index;
		}
	}
}
