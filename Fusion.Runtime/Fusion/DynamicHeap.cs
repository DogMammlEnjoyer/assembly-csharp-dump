using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Collections.LowLevel.Unsafe;

namespace Fusion
{
	public struct DynamicHeap
	{
		internal int MemoryReserved
		{
			get
			{
				return (this._blocksUsed - 1) * this._config.BlockPageCount * 32768;
			}
		}

		internal double MemoryAllocated
		{
			get
			{
				return Math.Round((double)this._memoryAllocated / 1024.0 / 1024.0, 3);
			}
		}

		internal int ObjectsAllocated
		{
			get
			{
				return this._objectsAllocated;
			}
		}

		internal int GCRoots
		{
			get
			{
				return this._rootListCount;
			}
		}

		internal DynamicHeap.Phase GCPhase
		{
			get
			{
				return this._gcPhase;
			}
		}

		internal unsafe static void Destroy(DynamicHeap* heap)
		{
			bool flag = heap == null;
			if (!flag)
			{
				for (int i = 1; i < heap->_blocksUsed; i++)
				{
					DynamicHeap.Destroy(*(IntPtr*)(heap->_blocks + (IntPtr)i * (IntPtr)sizeof(DynamicHeap.Block*) / (IntPtr)sizeof(DynamicHeap.Block*)));
				}
				Native.Free<DynamicHeap.Bin>(ref heap->_bins);
				Native.Free<DynamicHeap.Block>(ref heap->_blocks);
				Native.Free<DynamicHeap.Object>(ref heap->_gcStack);
				Native.Free<DynamicHeap.Object>(ref heap->_rootList);
				Native.Free<int>(ref heap->_typeMap);
				Native.Free<int>(ref heap->_typeMapStrides);
				Native.Free<DynamicHeap>(ref heap);
			}
		}

		private unsafe static void Destroy(DynamicHeap.Block* block)
		{
			Native.Free<DynamicHeap.Page>(ref block->Pages);
			Native.Free<byte>(ref block->Memory);
			Native.Free<DynamicHeap.Block>(ref block);
		}

		internal unsafe static DynamicHeap* Create(params Type[] types)
		{
			return DynamicHeap.Create(DynamicHeap.Config.Default, types);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static T* SetForcedAlive<[IsUnmanaged] T>(T* ptr) where T : struct, ValueType
		{
			Assert.Check(DynamicHeap._types.ContainsKey(typeof(T)));
			bool flag = ptr != null;
			if (flag)
			{
				DynamicHeap.Object* ptr2 = (DynamicHeap.Object*)(ptr - 8 / sizeof(T));
				Assert.Check(ptr2->Type > 0);
				ptr2->Flags |= DynamicHeap.ObjectFlags.ForceAlive;
			}
			return ptr;
		}

		internal unsafe static DynamicHeap* Create(DynamicHeap.Config config, params Type[] types)
		{
			DynamicHeap.RegisterTypes(types);
			DynamicHeap* ptr = Native.MallocAndClear<DynamicHeap>();
			ptr->_blocks = Native.MallocAndClearPtrArray<DynamicHeap.Block>(255);
			ptr->_blocksUsed = 1;
			ptr->_gcGen = 1;
			ptr->_config = config;
			ptr->_bins = Native.MallocAndClearArray<DynamicHeap.Bin>(49);
			ptr->_gcStackCapacity = 1024;
			ptr->_gcStack = Native.MallocAndClearPtrArray<DynamicHeap.Object>(ptr->_gcStackCapacity);
			ptr->_rootListCount = 0;
			ptr->_rootListCapacity = 1024;
			ptr->_rootList = Native.MallocAndClearPtrArray<DynamicHeap.Object>(ptr->_rootListCapacity);
			for (int i = 0; i < 49; i++)
			{
				DynamicHeap.Bin* ptr2 = ptr->_bins + i;
				ptr2->Index = i;
				ptr2->ObjectWords = DynamicHeap.BinSizes.Sizes[ptr2->Index];
				ptr2->ObjectStride = ptr2->ObjectWords * 8;
				ptr2->ObjectCapacity = ((ptr2->ObjectWords > 0) ? (4096 / ptr2->ObjectWords) : 0);
			}
			ushort maxOffset = DynamicHeap._types.Max((KeyValuePair<Type, DynamicHeap.TypeData> x) => x.Value.Offset);
			ushort offset = DynamicHeap._types.First((KeyValuePair<Type, DynamicHeap.TypeData> x) => x.Value.Offset == maxOffset).Value.Offset;
			ptr->_typeMapLength = (int)(maxOffset + offset);
			ptr->_typeMap = Native.MallocAndClearArray<int>(ptr->_typeMapLength);
			ptr->_typeMapStrides = Native.MallocAndClearArray<int>(ptr->_typeMapLength);
			foreach (DynamicHeap.TypeData typeData in from x in DynamicHeap._types
			select x.Value into x
			orderby x.Offset
			select x)
			{
				Assert.Check((int)typeData.Offset < ptr->_typeMapLength);
				Assert.Check(ptr->_typeMap[typeData.Offset] == 0);
				ptr->_typeMap[typeData.Offset] = typeData.Pointers.Length;
				ptr->_typeMapStrides[typeData.Offset] = typeData.Stride;
				for (int j = 0; j < typeData.Pointers.Length; j++)
				{
					Assert.Check((int)(typeData.Offset + 1) + j < ptr->_typeMapLength);
					Assert.Check(ptr->_typeMap[(int)(typeData.Offset + 1) + j] == 0);
					ptr->_typeMap[(int)(typeData.Offset + 1) + j] = typeData.Pointers[j];
				}
			}
			return ptr;
		}

		private unsafe static ushort NextGen(DynamicHeap* heap)
		{
			ushort num = heap->_gcGen + 1;
			heap->_gcGen = num;
			ushort num2 = num;
			bool flag = num2 == 0;
			if (flag)
			{
				num = heap->_gcGen + 1;
				heap->_gcGen = num;
				num2 = num;
			}
			Assert.Check(heap->_gcGen > 0);
			return num2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static DynamicHeap.Bin* GetBinByIndex(DynamicHeap* heap, int binIndex)
		{
			Assert.Check(binIndex > 0 && binIndex < 49);
			return heap->_bins + binIndex;
		}

		private unsafe static int GetBinIndexForSize(DynamicHeap* heap, int size)
		{
			int bin = DynamicHeap.GetBin(size);
			Assert.Check(bin > 0 && bin < 49);
			DynamicHeap.Bin* ptr = heap->_bins + bin;
			Assert.Check(ptr->ObjectStride >= size);
			Assert.Check(ptr->ObjectWords >= DynamicHeap.WordCount(size));
			DynamicHeap.Bin* ptr2 = heap->_bins + (bin - 1);
			Assert.Check(ptr2->ObjectStride < size);
			Assert.Check(ptr2->ObjectWords < DynamicHeap.WordCount(size));
			return bin;
		}

		private unsafe static byte* AllocateInternal(DynamicHeap* heap, int size, out byte block)
		{
			bool flag = size < 8 || size >= 32768;
			if (flag)
			{
				throw new InvalidOperationException(string.Format("invalid size {0}", size));
			}
			Assert.Check(DynamicHeap.WordCount(size) * 8 >= size);
			int num = DynamicHeap.GetBinIndexForSize(heap, size);
			bool flag2 = false;
			DynamicHeap.Bin* binByIndex;
			byte* ptr;
			for (;;)
			{
				binByIndex = DynamicHeap.GetBinByIndex(heap, num);
				Assert.Check(binByIndex->ObjectStride >= size);
				Assert.Check(binByIndex->ObjectWords >= DynamicHeap.WordCount(size));
				bool flag3 = binByIndex->Pages.Head != null;
				if (flag3)
				{
					ptr = DynamicHeap.TryAllocateFromPage(heap, binByIndex->Pages.Head, size, out block);
					bool flag4 = ptr != null;
					if (flag4)
					{
						break;
					}
				}
				bool flag5 = flag2;
				if (flag5)
				{
					goto Block_5;
				}
				bool flag6 = num + 1 < 49;
				if (!flag6)
				{
					goto IL_141;
				}
				num++;
				flag2 = true;
			}
			heap->_objectsAllocated = heap->_objectsAllocated + 1;
			heap->_memoryAllocated = heap->_memoryAllocated + binByIndex->ObjectStride;
			return ptr;
			Block_5:
			num = DynamicHeap.GetBinIndexForSize(heap, size);
			binByIndex = DynamicHeap.GetBinByIndex(heap, num);
			Assert.Check(binByIndex->ObjectStride >= size);
			Assert.Check(binByIndex->ObjectWords >= DynamicHeap.WordCount(size));
			IL_141:
			Assert.Check(DynamicHeap.PagesWithAvailableObjectsInBin(binByIndex) == 0);
			DynamicHeap.Page* ptr2 = DynamicHeap.AllocatePage(heap);
			Assert.Check(ptr2->ObjectsFree == null);
			Assert.Check(ptr2->ObjectsComitted == 0);
			Assert.Check(ptr2->ObjectsAllocated == 0);
			Assert.Check(ptr2->Prev == null);
			Assert.Check(ptr2->Next == null);
			Assert.Check(ptr2->Bin == 0);
			ptr2->Bin = binByIndex->Index;
			binByIndex->Pages.AddFirst(ptr2);
			byte* ptr3 = DynamicHeap.TryAllocateFromPage(heap, binByIndex->Pages.Head, size, out block);
			bool flag7 = ptr3 == null;
			if (flag7)
			{
				DynamicHeap.ThrowHeapCorrupted();
			}
			heap->_objectsAllocated = heap->_objectsAllocated + 1;
			heap->_memoryAllocated = heap->_memoryAllocated + binByIndex->ObjectStride;
			return ptr3;
		}

		private unsafe static DynamicHeap.Page* AllocatePage_Internal(DynamicHeap* heap, bool mustSucceed)
		{
			DynamicHeap.Block* head = heap->_blocksFreePages.Head;
			bool flag = head != null && head->PagesFree.Head != null;
			DynamicHeap.Page* result;
			if (flag)
			{
				DynamicHeap.Page* ptr = head->PagesFree.RemoveHead();
				ptr->Use++;
				bool flag2 = head->PagesFree.Head == null;
				if (flag2)
				{
					heap->_blocksFreePages.Remove(head);
				}
				result = ptr;
			}
			else
			{
				if (mustSucceed)
				{
					DynamicHeap.ThrowHeapCorrupted();
				}
				Assert.Check(DynamicHeap.BlocksWithAvailablePages(heap) == 0);
				DynamicHeap.AllocateBlock(heap);
				result = DynamicHeap.AllocatePage_Internal(heap, true);
			}
			return result;
		}

		private unsafe static DynamicHeap.Page* AllocatePage(DynamicHeap* heap)
		{
			return DynamicHeap.AllocatePage_Internal(heap, false);
		}

		private unsafe static void AllocateBlock(DynamicHeap* heap)
		{
			bool flag = heap->_blocksUsed == 255;
			if (flag)
			{
				throw new OutOfMemoryException();
			}
			bool flag2 = heap->_blocksUsed > 255;
			if (flag2)
			{
				DynamicHeap.ThrowHeapCorrupted();
			}
			DynamicHeap.Block* ptr = Native.MallocAndClear<DynamicHeap.Block>();
			ptr->Memory = (byte*)Native.MallocAndClear(heap->_config.BlockPageCount * 32768);
			ptr->Pages = Native.MallocAndClearArray<DynamicHeap.Page>(heap->_config.BlockPageCount);
			ref DynamicHeap.Block ptr2 = ref *ptr;
			int blocksUsed = heap->_blocksUsed;
			heap->_blocksUsed = blocksUsed + 1;
			ptr2.Index = (byte)blocksUsed;
			for (int i = 0; i < heap->_config.BlockPageCount; i++)
			{
				DynamicHeap.Page* ptr3 = ptr->Pages + i;
				ptr3->Memory = ptr->Memory + i * 32768;
				ptr3->Block = ptr;
				ptr3->Index = i;
				ptr->PagesFree.AddLast(ptr3);
			}
			*(IntPtr*)(heap->_blocks + (IntPtr)ptr->Index * (IntPtr)sizeof(DynamicHeap.Block*) / (IntPtr)sizeof(DynamicHeap.Block*)) = ptr;
			heap->_blocksFreePages.AddFirst(ptr);
		}

		private unsafe static int BlocksWithAvailablePages(DynamicHeap* heap)
		{
			int num = 0;
			for (int i = 1; i < heap->_blocksUsed; i++)
			{
				bool flag = ((IntPtr*)(heap->_blocks + (IntPtr)i * (IntPtr)sizeof(DynamicHeap.Block*) / (IntPtr)sizeof(DynamicHeap.Block*)))->PagesFree.Head != null;
				if (flag)
				{
					num++;
				}
			}
			return num;
		}

		private unsafe static int PagesWithAvailableObjectsInBin(DynamicHeap.Bin* bin)
		{
			int num = 0;
			for (DynamicHeap.Page* ptr = bin->Pages.Head; ptr != null; ptr = ptr->Next)
			{
				bool flag = ptr->ObjectsAllocated < bin->ObjectCapacity;
				if (flag)
				{
					num++;
				}
			}
			return num;
		}

		private unsafe static int ObjectsFreeCount(DynamicHeap.Page* p)
		{
			DynamicHeap.ObjectFree* ptr = p->ObjectsFree;
			int num = 0;
			while (ptr != null)
			{
				num++;
				bool flag = ptr->Next == 0;
				if (flag)
				{
					break;
				}
				ptr = DynamicHeap.ResolvePageOffset(p, ptr->Next);
			}
			return num;
		}

		private unsafe static byte* TryAllocateFromPage(DynamicHeap* heap, DynamicHeap.Page* page, int size, out byte block)
		{
			DynamicHeap.Bin* binByIndex = DynamicHeap.GetBinByIndex(heap, page->Bin);
			Assert.Check(binByIndex->Index == page->Bin);
			Assert.Check(binByIndex->Pages.Contains(page));
			Assert.Check(binByIndex->ObjectStride >= size);
			bool flag = page->ObjectsFree != null;
			void* ptr;
			if (flag)
			{
				Assert.Check(page->ObjectsComitted > 0);
				Assert.Check(page->ObjectsFreeCount > 0);
				ptr = (void*)page->ObjectsFree;
				int next = page->ObjectsFree->Next;
				page->ObjectsFree->Next = 0;
				page->ObjectsFreeCount = page->ObjectsFreeCount - 1;
				bool flag2 = next != 0;
				if (flag2)
				{
					page->ObjectsFree = DynamicHeap.ResolvePageOffset(page, next);
				}
				else
				{
					page->ObjectsFree = default(DynamicHeap.ObjectFree*);
				}
				block = page->Block->Index;
				Assert.Check(DynamicHeap.IsPtrInBlock(heap, page->Block, ptr));
			}
			else
			{
				bool flag3 = page->ObjectsComitted < binByIndex->ObjectCapacity;
				if (flag3)
				{
					byte* memory = page->Memory;
					int num = page->ObjectsComitted;
					page->ObjectsComitted = num + 1;
					ptr = (void*)((byte*)memory + num * binByIndex->ObjectStride);
					Assert.Check(page->ObjectsComitted <= binByIndex->ObjectCapacity);
					Assert.Check(DynamicHeap.IsPtrInBlock(heap, page->Block, ptr));
					block = page->Block->Index;
				}
				else
				{
					ptr = null;
					block = 0;
				}
			}
			bool flag4 = ptr != null;
			if (flag4)
			{
				Assert.Check(DynamicHeap.IsPtrInBlock(heap, page->Block, ptr));
				Assert.Check(page->ObjectsAllocated < binByIndex->ObjectCapacity);
				int num = page->ObjectsAllocated + 1;
				page->ObjectsAllocated = num;
				bool flag5 = num == binByIndex->ObjectCapacity;
				if (flag5)
				{
					binByIndex->Pages.MoveLast(page);
				}
				Assert.Check<int, int, int, int>(DynamicHeap.ObjectsFreeCount(page) + page->ObjectsAllocated == page->ObjectsComitted, DynamicHeap.ObjectsFreeCount(page), page->ObjectsAllocated, page->ObjectsComitted, page->ObjectsFreeCount);
				Native.MemClear(ptr, size);
			}
			return (byte*)ptr;
		}

		internal static void RegisterTypes(params Type[] types)
		{
			bool flag = DynamicHeap._types != null;
			if (flag)
			{
				Assert.Check(DynamicHeap._typesByOffset != null);
			}
			else
			{
				DynamicHeap._types = new Dictionary<Type, DynamicHeap.TypeData>();
				DynamicHeap._typesByOffset = new Dictionary<ushort, DynamicHeap.TypeData>();
				foreach (Type type in types)
				{
					bool flag2 = !UnsafeUtility.IsUnmanaged(type);
					if (flag2)
					{
						throw new Exception(string.Format("type {0} is not an unmanaged struct", type));
					}
					DynamicHeap._types.Add(type, new DynamicHeap.TypeData
					{
						Type = type
					});
				}
				int num = 1;
				foreach (KeyValuePair<Type, DynamicHeap.TypeData> keyValuePair in DynamicHeap._types)
				{
					List<int> list = new List<int>();
					FieldInfo[] fields = keyValuePair.Value.Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					Assert.Check(UnsafeUtility.SizeOf(keyValuePair.Key) % 8 == 0);
					keyValuePair.Value.Offset = (ushort)num;
					keyValuePair.Value.Stride = UnsafeUtility.SizeOf(keyValuePair.Key) / 8;
					foreach (FieldInfo fieldInfo in fields)
					{
						bool flag3 = fieldInfo.FieldType.IsPointer && DynamicHeap._types.ContainsKey(fieldInfo.FieldType.GetElementType()) && fieldInfo.GetCustomAttribute<DynamicHeap.Ignore>() == null;
						if (flag3)
						{
							int fieldOffset = UnsafeUtility.GetFieldOffset(fieldInfo);
							bool flag4 = fieldOffset % 8 != 0;
							if (flag4)
							{
								throw new Exception(string.Concat(new string[]
								{
									"field ",
									keyValuePair.Key.Name,
									".",
									fieldInfo.Name,
									" is not on an 8 byte offset, can't perform tracking for this pointer"
								}));
							}
							list.Add(fieldOffset / 8);
						}
					}
					keyValuePair.Value.Pointers = list.ToArray();
					DynamicHeap._typesByOffset.Add(keyValuePair.Value.Offset, keyValuePair.Value);
					num += keyValuePair.Value.Pointers.Length + 1;
					Assert.Check(num < 65535);
				}
			}
		}

		private static ushort GetTypeOffset<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return DynamicHeap._types[typeof(T)].Offset;
		}

		private unsafe static bool IsPtrInBlock(DynamicHeap* heap, DynamicHeap.Block* block, void* p)
		{
			return p >= (void*)block->Memory && p < (void*)(block->Memory + heap->_config.BlockPageCount * 32768);
		}

		private unsafe static DynamicHeap.Page* GetPageForPtr(DynamicHeap* heap, DynamicHeap.Block* block, void* ptr)
		{
			Assert.Check((long)((byte*)ptr - (byte*)block->Memory) >> 15 >= 0L);
			Assert.Check((long)((byte*)ptr - (byte*)block->Memory) >> 15 < (long)heap->_config.BlockPageCount);
			long num = (long)((byte*)ptr - (byte*)block->Memory) >> 15;
			Assert.Check((long)block->Pages[num * (long)sizeof(DynamicHeap.Page) / (long)sizeof(DynamicHeap.Page)].Index == num);
			return block->Pages + num * (long)sizeof(DynamicHeap.Page) / (long)sizeof(DynamicHeap.Page);
		}

		private unsafe static int GetPageOffset(DynamicHeap.Page* page, DynamicHeap.ObjectFree* obj)
		{
			bool flag = obj == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				Assert.Check(obj >= (DynamicHeap.ObjectFree*)page->Memory);
				int num = (int)((long)((byte*)obj - (byte*)page->Memory));
				Assert.Check(num < 32768);
				result = num + 1;
			}
			return result;
		}

		private unsafe static DynamicHeap.ObjectFree* ResolvePageOffset(DynamicHeap.Page* page, int offset)
		{
			bool flag = offset == 0;
			DynamicHeap.ObjectFree* result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = (DynamicHeap.ObjectFree*)(page->Memory + (offset - 1));
			}
			return result;
		}

		private unsafe static void FreeInternal(DynamicHeap* heap, void* ptr, DynamicHeap.Object objData)
		{
			Assert.Check(objData.Type > 0);
			Assert.Check(objData.Block >= 1 && objData.Block < byte.MaxValue);
			DynamicHeap.Block* ptr2 = *(IntPtr*)(heap->_blocks + (IntPtr)objData.Block * (IntPtr)sizeof(DynamicHeap.Block*) / (IntPtr)sizeof(DynamicHeap.Block*));
			Assert.Check<long, long>(DynamicHeap.IsPtrInBlock(heap, ptr2, ptr), ptr, ptr2->Memory);
			DynamicHeap.Page* pageForPtr = DynamicHeap.GetPageForPtr(heap, ptr2, ptr);
			((DynamicHeap.ObjectFree*)ptr)->Next = DynamicHeap.GetPageOffset(pageForPtr, pageForPtr->ObjectsFree);
			pageForPtr->ObjectsFree = (DynamicHeap.ObjectFree*)ptr;
			pageForPtr->ObjectsFreeCount++;
			DynamicHeap.Bin* binByIndex = DynamicHeap.GetBinByIndex(heap, pageForPtr->Bin);
			heap->_objectsAllocated = heap->_objectsAllocated - 1;
			heap->_memoryAllocated = heap->_memoryAllocated - binByIndex->ObjectStride;
			DynamicHeap.Page* ptr3 = pageForPtr;
			int num = ptr3->ObjectsAllocated - 1;
			ptr3->ObjectsAllocated = num;
			bool flag = num == 0;
			if (flag)
			{
				binByIndex->Pages.Remove(pageForPtr);
				pageForPtr->Bin = 0;
				pageForPtr->ObjectsFree = default(DynamicHeap.ObjectFree*);
				pageForPtr->ObjectsAllocated = 0;
				pageForPtr->ObjectsComitted = 0;
				ptr2->PagesFree.AddLast(pageForPtr);
				bool flag2 = ptr2->PagesFree.Head == pageForPtr;
				if (flag2)
				{
					heap->_blocksFreePages.AddLast(ptr2);
				}
			}
			else
			{
				Assert.Check(pageForPtr->ObjectsAllocated > 0);
				bool flag3 = binByIndex->ObjectCapacity == pageForPtr->ObjectsAllocated + 1;
				if (flag3)
				{
					binByIndex->Pages.MoveFirst(pageForPtr);
				}
			}
		}

		public unsafe static void Free(DynamicHeap* heap, void* ptr)
		{
			DynamicHeap.Object* ptr2 = (DynamicHeap.Object*)((byte*)ptr - 8);
			DynamicHeap.Object @object = *ptr2;
			bool flag = (@object.Flags & DynamicHeap.ObjectFlags.Root) == DynamicHeap.ObjectFlags.Root;
			if (flag)
			{
				bool flag2 = false;
				for (int i = 0; i < heap->_rootListCount; i++)
				{
					bool flag3 = *(IntPtr*)(heap->_rootList + (IntPtr)i * (IntPtr)sizeof(DynamicHeap.Object*) / (IntPtr)sizeof(DynamicHeap.Object*)) == ptr2;
					if (flag3)
					{
						int num = i;
						int num2 = heap->_rootListCount - 1;
						heap->_rootListCount = num2;
						bool flag4 = num < num2;
						if (flag4)
						{
							*(IntPtr*)(heap->_rootList + (IntPtr)i * (IntPtr)sizeof(DynamicHeap.Object*) / (IntPtr)sizeof(DynamicHeap.Object*)) = *(IntPtr*)(heap->_rootList + (IntPtr)heap->_rootListCount * (IntPtr)sizeof(DynamicHeap.Object*) / (IntPtr)sizeof(DynamicHeap.Object*));
							*(IntPtr*)(heap->_rootList + (IntPtr)heap->_rootListCount * (IntPtr)sizeof(DynamicHeap.Object*) / (IntPtr)sizeof(DynamicHeap.Object*)) = (IntPtr)((UIntPtr)0);
						}
						else
						{
							*(IntPtr*)(heap->_rootList + (IntPtr)i * (IntPtr)sizeof(DynamicHeap.Object*) / (IntPtr)sizeof(DynamicHeap.Object*)) = (IntPtr)((UIntPtr)0);
						}
						flag2 = true;
						break;
					}
				}
				bool flag5 = !flag2;
				if (flag5)
				{
					DynamicHeap.ThrowHeapCorrupted();
				}
			}
			else
			{
				bool flag6 = (@object.Flags & DynamicHeap.ObjectFlags.Tracked) == DynamicHeap.ObjectFlags.Tracked;
				if (flag6)
				{
					throw new InvalidOperationException("Can't manually free a tracked object that isn't a root");
				}
			}
			*ptr2 = default(DynamicHeap.Object);
			DynamicHeap.FreeInternal(heap, (void*)ptr2, @object);
		}

		internal unsafe static void* Allocate(DynamicHeap* heap, int size)
		{
			Assert.Always(size > 0, "array > 0");
			int num = 8;
			byte block;
			byte* ptr = DynamicHeap.AllocateInternal(heap, size + num, out block);
			DynamicHeap.Object* ptr2 = (DynamicHeap.Object*)ptr;
			*ptr2 = default(DynamicHeap.Object);
			ptr2->Gen = heap->_gcGen;
			ptr2->Block = block;
			ptr2->Type = ushort.MaxValue;
			return (void*)(ptr + num);
		}

		internal unsafe static int GetArrayLength(void* ptr)
		{
			DynamicHeap.Object* ptr2 = (DynamicHeap.Object*)((byte*)ptr - 8);
			Assert.Check(ptr2->Array > 0);
			Assert.Check(ptr2->Type > 0);
			return (int)ptr2->Array;
		}

		internal unsafe static T* AllocateTracked<[IsUnmanaged] T>(DynamicHeap* heap, ushort array = 1, bool root = false) where T : struct, ValueType
		{
			Assert.Always(array > 0, "array > 0");
			byte block;
			byte* ptr = DynamicHeap.AllocateInternal(heap, sizeof(T) * (int)array + 8, out block);
			DynamicHeap.Object* ptr2 = (DynamicHeap.Object*)ptr;
			if (root)
			{
				DynamicHeap.InitRoot(heap, ptr2);
			}
			DynamicHeap.InitObj(heap, ptr2, DynamicHeap.GetTypeOffset<T>(), array, block);
			bool flag = DynamicHeap._types[typeof(T)].Pointers.Length == 0;
			if (flag)
			{
				ptr2->Flags |= DynamicHeap.ObjectFlags.Simple;
			}
			return (T*)(ptr + 8);
		}

		internal unsafe static T** AllocateTrackedPointerArray<[IsUnmanaged] T>(DynamicHeap* heap, ushort array, bool root = false) where T : struct, ValueType
		{
			Assert.Always(array > 0, "array > 0");
			byte block;
			byte* ptr = DynamicHeap.AllocateInternal(heap, sizeof(T*) * (int)array + 8, out block);
			DynamicHeap.Object* ptr2 = (DynamicHeap.Object*)ptr;
			if (root)
			{
				DynamicHeap.InitRoot(heap, ptr2);
			}
			DynamicHeap.InitObj(heap, ptr2, DynamicHeap.GetTypeOffset<T>(), array, block);
			ptr2->Flags |= DynamicHeap.ObjectFlags.Pointer;
			return (T**)(ptr + 8);
		}

		private static void ThrowHeapCorrupted()
		{
			throw new Exception("Heap Corrupted");
		}

		private unsafe static void InitObj(DynamicHeap* heap, DynamicHeap.Object* obj, ushort type, ushort array, byte block)
		{
			obj->Gen = heap->_gcGen;
			obj->Type = type;
			obj->Block = block;
			obj->Array = array;
			obj->Flags = (obj->Flags | DynamicHeap.ObjectFlags.Tracked);
			obj->Flags = (obj->Flags | DynamicHeap.ObjectFlags.ForceAlive);
		}

		private unsafe static void InitRoot(DynamicHeap* heap, DynamicHeap.Object* obj)
		{
			bool flag = heap->_rootListCount == heap->_rootListCapacity;
			if (flag)
			{
				heap->_rootList = Native.DoublePtrArray<DynamicHeap.Object>(heap->_rootList, heap->_rootListCapacity);
				heap->_rootListCapacity = heap->_rootListCapacity * 2;
			}
			ref IntPtr rootList = ref *(IntPtr*)heap->_rootList;
			int rootListCount = heap->_rootListCount;
			heap->_rootListCount = rootListCount + 1;
			*(ref rootList + (IntPtr)rootListCount * (IntPtr)sizeof(DynamicHeap.Object*)) = obj;
			obj->Flags = DynamicHeap.ObjectFlags.Root;
		}

		private unsafe static void ExpandStack(DynamicHeap* heap)
		{
			heap->_gcStack = Native.DoublePtrArray<DynamicHeap.Object>(heap->_gcStack, heap->_gcStackCapacity);
			heap->_gcStackCapacity = heap->_gcStackCapacity * 2;
		}

		[MonoPInvokeCallback(typeof(DynamicHeap.CollectGarbageDelegate))]
		public unsafe static void CollectGarbage(DynamicHeap* heap, void** dynamicRoots, int dynamicRootsLength)
		{
			ushort num = heap->_gcGen;
			bool flag = heap->_gcPhase == DynamicHeap.Phase.Idle;
			if (flag)
			{
				num = DynamicHeap.NextGen(heap);
				heap->_gcStackCount = heap->_rootListCount;
				while (heap->_gcStackCapacity < heap->_gcStackCount)
				{
					DynamicHeap.ExpandStack(heap);
				}
				Native.MemCpy((void*)heap->_gcStack, (void*)heap->_rootList, sizeof(DynamicHeap.Object*) * heap->_gcStackCount);
				for (int i = 0; i < dynamicRootsLength; i++)
				{
					bool flag2 = heap->_gcStackCount == heap->_gcStackCapacity;
					if (flag2)
					{
						DynamicHeap.ExpandStack(heap);
					}
					ref IntPtr gcStack = ref *(IntPtr*)heap->_gcStack;
					int num2 = heap->_gcStackCount;
					heap->_gcStackCount = num2 + 1;
					*(ref gcStack + (IntPtr)num2 * (IntPtr)sizeof(DynamicHeap.Object*)) = *(IntPtr*)(dynamicRoots + (IntPtr)i * (IntPtr)sizeof(void*) / (IntPtr)sizeof(void*)) - 8;
				}
				heap->_gcPhase = DynamicHeap.Phase.Mark;
			}
			bool flag3 = heap->_gcPhase == DynamicHeap.Phase.Mark;
			if (flag3)
			{
				int* typeMap = heap->_typeMap;
				int* typeMapStrides = heap->_typeMapStrides;
				while (heap->_gcStackCount > 0)
				{
					IntPtr gcStack2 = heap->_gcStack;
					int num2 = heap->_gcStackCount - 1;
					heap->_gcStackCount = num2;
					DynamicHeap.Object* ptr = *(gcStack2 + (IntPtr)num2 * (IntPtr)sizeof(DynamicHeap.Object*));
					bool flag4 = ptr->Gen == num || ptr->Type == ushort.MaxValue;
					if (!flag4)
					{
						ptr->Gen = num;
						ptr->Flags &= ~DynamicHeap.ObjectFlags.ForceAlive;
						ushort type = ptr->Type;
						byte** ptr2 = (byte**)(ptr + 8 / sizeof(DynamicHeap.Object));
						int array = (int)ptr->Array;
						bool flag5 = (ptr->Flags & DynamicHeap.ObjectFlags.Pointer) == DynamicHeap.ObjectFlags.Pointer;
						if (flag5)
						{
							for (int j = 0; j < array; j++)
							{
								byte* ptr3 = *(IntPtr*)(ptr2 + (IntPtr)j * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*));
								bool flag6 = ptr3 != null;
								if (flag6)
								{
									DynamicHeap.Object* ptr4 = (DynamicHeap.Object*)(ptr3 - 8);
									bool flag7 = ptr4->Type != ushort.MaxValue && ptr4->Gen != num;
									if (flag7)
									{
										bool flag8 = typeMap[ptr4->Type] == 0;
										if (flag8)
										{
											Assert.Check((ptr4->Flags & DynamicHeap.ObjectFlags.Simple) == DynamicHeap.ObjectFlags.Simple);
											ptr4->Gen = num;
										}
										else
										{
											Assert.Check((ptr4->Flags & DynamicHeap.ObjectFlags.Simple) == (DynamicHeap.ObjectFlags)0);
											bool flag9 = heap->_gcStackCount == heap->_gcStackCapacity;
											if (flag9)
											{
												DynamicHeap.ExpandStack(heap);
											}
											ref IntPtr gcStack3 = ref *(IntPtr*)heap->_gcStack;
											num2 = heap->_gcStackCount;
											heap->_gcStackCount = num2 + 1;
											*(ref gcStack3 + (IntPtr)num2 * (IntPtr)sizeof(DynamicHeap.Object*)) = ptr4;
										}
									}
								}
							}
						}
						else
						{
							int num3 = typeMap[type];
							for (int k = 0; k < array; k++)
							{
								for (int l = 1; l <= num3; l++)
								{
									byte* ptr5 = *(IntPtr*)(ptr2 + (IntPtr)typeMap[(int)type + l] * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*));
									bool flag10 = ptr5 != null;
									if (flag10)
									{
										DynamicHeap.Object* ptr6 = (DynamicHeap.Object*)(ptr5 - 8);
										bool flag11 = ptr6->Type != ushort.MaxValue && ptr6->Gen != num;
										if (flag11)
										{
											bool flag12 = typeMap[ptr6->Type] == 0;
											if (flag12)
											{
												Assert.Check((ptr6->Flags & DynamicHeap.ObjectFlags.Simple) == DynamicHeap.ObjectFlags.Simple);
												ptr6->Gen = num;
											}
											else
											{
												Assert.Check((ptr6->Flags & DynamicHeap.ObjectFlags.Simple) == (DynamicHeap.ObjectFlags)0);
												bool flag13 = heap->_gcStackCount == heap->_gcStackCapacity;
												if (flag13)
												{
													DynamicHeap.ExpandStack(heap);
												}
												ref IntPtr gcStack4 = ref *(IntPtr*)heap->_gcStack;
												num2 = heap->_gcStackCount;
												heap->_gcStackCount = num2 + 1;
												*(ref gcStack4 + (IntPtr)num2 * (IntPtr)sizeof(DynamicHeap.Object*)) = ptr6;
											}
										}
									}
								}
								bool flag14 = ptr->Array > 1;
								if (flag14)
								{
									ptr2 += (IntPtr)typeMapStrides[type] * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*);
								}
							}
						}
					}
				}
				heap->_gcBlock = 1;
				heap->_gcBlockPage = 0;
				heap->_gcPhase = DynamicHeap.Phase.Sweep;
			}
			bool flag15 = heap->_gcPhase == DynamicHeap.Phase.Sweep;
			if (flag15)
			{
				while (heap->_gcBlock < heap->_blocksUsed)
				{
					DynamicHeap.Block* ptr7 = *(IntPtr*)(heap->_blocks + (IntPtr)heap->_gcBlock * (IntPtr)sizeof(DynamicHeap.Block*) / (IntPtr)sizeof(DynamicHeap.Block*));
					while (heap->_gcBlockPage < heap->_config.BlockPageCount)
					{
						DynamicHeap.Page* ptr8 = ptr7->Pages + heap->_gcBlockPage;
						bool flag16 = ptr8->ObjectsComitted > 0;
						if (flag16)
						{
							DynamicHeap.Bin* binByIndex = DynamicHeap.GetBinByIndex(heap, ptr8->Bin);
							for (int m = 0; m < ptr8->ObjectsComitted; m++)
							{
								DynamicHeap.Object* ptr9 = (DynamicHeap.Object*)(ptr8->Memory + m * binByIndex->ObjectStride);
								DynamicHeap.Object @object = *ptr9;
								bool flag17 = (@object.Flags & DynamicHeap.ObjectFlags.Tracked) == DynamicHeap.ObjectFlags.Tracked;
								if (flag17)
								{
									bool flag18 = @object.Gen != num;
									if (flag18)
									{
										bool flag19 = (@object.Flags & DynamicHeap.ObjectFlags.Root) == DynamicHeap.ObjectFlags.Root;
										if (flag19)
										{
											DynamicHeap.ThrowHeapCorrupted();
										}
										bool flag20 = (@object.Flags & DynamicHeap.ObjectFlags.ForceAlive) == DynamicHeap.ObjectFlags.ForceAlive;
										if (flag20)
										{
											ptr9->Flags &= ~DynamicHeap.ObjectFlags.ForceAlive;
											ptr9->Gen = num;
										}
										else
										{
											bool flag21 = heap->_gcStackCount == heap->_gcStackCapacity;
											if (flag21)
											{
												DynamicHeap.ExpandStack(heap);
											}
											ref IntPtr gcStack5 = ref *(IntPtr*)heap->_gcStack;
											int num2 = heap->_gcStackCount;
											heap->_gcStackCount = num2 + 1;
											*(ref gcStack5 + (IntPtr)num2 * (IntPtr)sizeof(DynamicHeap.Object*)) = ptr9;
											ptr9->Flags |= DynamicHeap.ObjectFlags.Garbage;
										}
									}
								}
							}
						}
						heap->_gcBlockPage = heap->_gcBlockPage + 1;
					}
					heap->_gcBlockPage = 0;
					heap->_gcBlock = heap->_gcBlock + 1;
				}
				heap->_gcPhase = DynamicHeap.Phase.Free;
			}
			bool flag22 = heap->_gcPhase == DynamicHeap.Phase.Free;
			if (flag22)
			{
				while (heap->_gcStackCount > 0)
				{
					IntPtr gcStack6 = heap->_gcStack;
					int num2 = heap->_gcStackCount - 1;
					heap->_gcStackCount = num2;
					DynamicHeap.Object* ptr10 = *(gcStack6 + (IntPtr)num2 * (IntPtr)sizeof(DynamicHeap.Object*));
					DynamicHeap.Object object2 = *ptr10;
					Assert.Check((object2.Flags & DynamicHeap.ObjectFlags.Garbage) == DynamicHeap.ObjectFlags.Garbage);
					bool flag23 = (object2.Flags & DynamicHeap.ObjectFlags.ForceAlive) == (DynamicHeap.ObjectFlags)0;
					if (flag23)
					{
						*ptr10 = default(DynamicHeap.Object);
						DynamicHeap.FreeInternal(heap, (void*)ptr10, object2);
					}
					else
					{
						ptr10->Flags &= ~DynamicHeap.ObjectFlags.ForceAlive;
					}
				}
				heap->_gcPhase = DynamicHeap.Phase.Idle;
			}
		}

		private static int GetBin(int size)
		{
			Assert.Check(size > 0);
			int num = DynamicHeap.WordCount(size);
			bool flag = num <= 8;
			int result;
			if (flag)
			{
				result = num;
			}
			else
			{
				num--;
				int num2 = DynamicHeap.BitScan((uint)num);
				int num3 = (num2 << 2) + (num >> num2 - 2 & 3) - 3;
				Assert.Check(num3 >= 0 && num3 < 49);
				result = num3;
			}
			return result;
		}

		private static int WordCount(int size)
		{
			Assert.Check(size > 0);
			return (size + 7) / 8;
		}

		private static int BitScan(uint v)
		{
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			return (int)DynamicHeap._debruijnTable[(int)(v * 130329821U >> 27)];
		}

		internal const int WORD_SHIFT = 3;

		internal const int WORD_SIZE = 8;

		internal const int PAGE_SHIFT = 15;

		internal const int PAGE_SIZE = 32768;

		internal const int PAGE_WORD_COUNT = 4096;

		internal const int BIN_COUNT = 49;

		internal const int MAX_BLOCK_COUNT = 255;

		private DynamicHeap.BlockList _blocksFreePages;

		private unsafe DynamicHeap.Block** _blocks;

		private int _blocksUsed;

		private unsafe DynamicHeap.Bin* _bins;

		private unsafe int* _typeMap;

		private int _typeMapLength;

		private unsafe int* _typeMapStrides;

		private ushort _gcGen;

		private int _gcBlock;

		private int _gcBlockPage;

		private DynamicHeap.Phase _gcPhase;

		private unsafe DynamicHeap.Object** _gcStack;

		private int _gcStackCount;

		private int _gcStackCapacity;

		private DynamicHeap.Config _config;

		private unsafe DynamicHeap.Object** _rootList;

		private int _rootListCapacity;

		private int _rootListCount;

		private int _objectsAllocated;

		private int _memoryAllocated;

		private static Dictionary<Type, DynamicHeap.TypeData> _types = null;

		private static Dictionary<ushort, DynamicHeap.TypeData> _typesByOffset = null;

		private static byte[] _debruijnTable = new byte[]
		{
			0,
			9,
			1,
			10,
			13,
			21,
			2,
			29,
			11,
			14,
			16,
			18,
			22,
			25,
			3,
			30,
			8,
			12,
			20,
			28,
			15,
			17,
			24,
			7,
			19,
			27,
			23,
			6,
			26,
			5,
			4,
			31
		};

		private struct BlockList
		{
			public unsafe void AddFirst(DynamicHeap.Block* item)
			{
				Assert.Check(!this.IsInList(item));
				item->Next = this.Head;
				item->Prev = null;
				bool flag = this.Head != null;
				if (flag)
				{
					this.Head->Prev = item;
					this.Head = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public unsafe void MoveFirst(DynamicHeap.Block* item)
			{
				bool flag = this.Head != item;
				if (flag)
				{
					this.Remove(item);
					this.AddFirst(item);
				}
			}

			public unsafe void MoveLast(DynamicHeap.Block* item)
			{
				bool flag = this.Tail != item;
				if (flag)
				{
					this.Remove(item);
					this.AddLast(item);
				}
			}

			public unsafe void AddLast(DynamicHeap.Block* item)
			{
				Assert.Check(!this.IsInList(item));
				item->Next = null;
				item->Prev = this.Tail;
				bool flag = this.Tail != null;
				if (flag)
				{
					this.Tail->Next = item;
					this.Tail = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public unsafe void AddBefore(DynamicHeap.Block* before, DynamicHeap.Block* item)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.IsInList(before));
				Assert.Check(!this.IsInList(item));
				bool flag = before == this.Head;
				if (flag)
				{
					this.AddFirst(item);
				}
				else
				{
					item->Next = before;
					item->Prev = before->Prev;
					before->Prev->Next = item;
					before->Prev = item;
					this.Count++;
				}
				Assert.Check(this.IsInList(before));
				Assert.Check(this.IsInList(item));
			}

			public unsafe void AddAfter(DynamicHeap.Block* after, DynamicHeap.Block* item)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.IsInList(after));
				Assert.Check(!this.IsInList(item));
				bool flag = after == this.Tail;
				if (flag)
				{
					this.AddLast(item);
				}
				else
				{
					item->Next = after->Next;
					item->Prev = after;
					after->Next->Prev = item;
					after->Next = item;
					this.Count++;
				}
				Assert.Check(this.IsInList(after));
				Assert.Check(this.IsInList(item));
			}

			public unsafe bool TryRemoveHead(out DynamicHeap.Block* head)
			{
				bool flag = this.Count == 0;
				bool result;
				if (flag)
				{
					head = (IntPtr)((UIntPtr)0);
					result = false;
				}
				else
				{
					head = this.RemoveHead();
					result = true;
				}
				return result;
			}

			public unsafe DynamicHeap.Block* RemoveHead()
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.Head != null);
				Assert.Check(this.IsInList(this.Head));
				DynamicHeap.Block* head = this.Head;
				this.Remove(head);
				return head;
			}

			public unsafe void Remove(DynamicHeap.Block* item)
			{
				Assert.Check(this.IsInList(item));
				bool flag = item->Prev != null;
				if (flag)
				{
					item->Prev->Next = item->Next;
				}
				bool flag2 = item->Next != null;
				if (flag2)
				{
					item->Next->Prev = item->Prev;
				}
				bool flag3 = item == this.Tail;
				if (flag3)
				{
					this.Tail = item->Prev;
				}
				bool flag4 = item == this.Head;
				if (flag4)
				{
					this.Head = item->Next;
				}
				item->Prev = null;
				item->Next = null;
				this.Count--;
			}

			private unsafe bool IsInList(DynamicHeap.Block* item)
			{
				for (DynamicHeap.Block* ptr = this.Head; ptr != null; ptr = ptr->Next)
				{
					bool flag = ptr == item;
					if (flag)
					{
						return true;
					}
				}
				return false;
			}

			public int Count;

			public unsafe DynamicHeap.Block* Head;

			public unsafe DynamicHeap.Block* Tail;
		}

		internal struct Config
		{
			public static DynamicHeap.Config Default
			{
				get
				{
					DynamicHeap.Config result;
					result.BlockPageCount = 64;
					return result;
				}
			}

			public int BlockPageCount;
		}

		internal enum Phase
		{
			Idle,
			Mark,
			Sweep,
			Free
		}

		private class TypeData
		{
			public TypeData()
			{
			}

			public TypeData(int stride, ushort offset, int[] pointers, Type type)
			{
				this.Stride = stride;
				this.Offset = offset;
				this.Pointers = pointers;
				this.Type = type;
			}

			public int Stride;

			public ushort Offset;

			public int[] Pointers;

			public Type Type;
		}

		public unsafe delegate void CollectGarbageDelegate(DynamicHeap* heap, void** dynamicRoots, int dynamicRootsLength);

		private struct PageList
		{
			public unsafe void AddFirst(DynamicHeap.Page* item)
			{
				Assert.Check(!this.Contains(item));
				item->Next = this.Head;
				item->Prev = null;
				bool flag = this.Head != null;
				if (flag)
				{
					this.Head->Prev = item;
					this.Head = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public unsafe void AddLast(DynamicHeap.Page* item)
			{
				Assert.Check(!this.Contains(item));
				item->Next = null;
				item->Prev = this.Tail;
				bool flag = this.Tail != null;
				if (flag)
				{
					this.Tail->Next = item;
					this.Tail = item;
				}
				else
				{
					this.Head = item;
					this.Tail = item;
				}
				this.Count++;
			}

			public unsafe void AddBefore(DynamicHeap.Page* before, DynamicHeap.Page* item)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.Contains(before));
				Assert.Check(!this.Contains(item));
				bool flag = before == this.Head;
				if (flag)
				{
					this.AddFirst(item);
				}
				else
				{
					item->Next = before;
					item->Prev = before->Prev;
					before->Prev->Next = item;
					before->Prev = item;
					this.Count++;
				}
				Assert.Check(this.Contains(before));
				Assert.Check(this.Contains(item));
			}

			public unsafe void MoveFirst(DynamicHeap.Page* item)
			{
				bool flag = this.Head != item;
				if (flag)
				{
					this.Remove(item);
					this.AddFirst(item);
				}
			}

			public unsafe void MoveLast(DynamicHeap.Page* item)
			{
				bool flag = this.Tail != item;
				if (flag)
				{
					this.Remove(item);
					this.AddLast(item);
				}
			}

			public unsafe void AddAfter(DynamicHeap.Page* after, DynamicHeap.Page* item)
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.Contains(after));
				Assert.Check(!this.Contains(item));
				bool flag = after == this.Tail;
				if (flag)
				{
					this.AddLast(item);
				}
				else
				{
					item->Next = after->Next;
					item->Prev = after;
					after->Next->Prev = item;
					after->Next = item;
					this.Count++;
				}
				Assert.Check(this.Contains(after));
				Assert.Check(this.Contains(item));
			}

			public unsafe bool TryRemoveHead(out DynamicHeap.Page* head)
			{
				bool flag = this.Count == 0;
				bool result;
				if (flag)
				{
					head = (IntPtr)((UIntPtr)0);
					result = false;
				}
				else
				{
					head = this.RemoveHead();
					result = true;
				}
				return result;
			}

			public unsafe DynamicHeap.Page* RemoveHead()
			{
				Assert.Check(this.Count > 0);
				Assert.Check(this.Head != null);
				Assert.Check(this.Contains(this.Head));
				DynamicHeap.Page* head = this.Head;
				this.Remove(head);
				return head;
			}

			public unsafe void Remove(DynamicHeap.Page* item)
			{
				Assert.Check(this.Contains(item));
				bool flag = item->Prev != null;
				if (flag)
				{
					item->Prev->Next = item->Next;
				}
				bool flag2 = item->Next != null;
				if (flag2)
				{
					item->Next->Prev = item->Prev;
				}
				bool flag3 = item == this.Tail;
				if (flag3)
				{
					this.Tail = item->Prev;
				}
				bool flag4 = item == this.Head;
				if (flag4)
				{
					this.Head = item->Next;
				}
				item->Prev = null;
				item->Next = null;
				this.Count--;
			}

			public unsafe bool Contains(DynamicHeap.Page* item)
			{
				for (DynamicHeap.Page* ptr = this.Head; ptr != null; ptr = ptr->Next)
				{
					bool flag = ptr == item;
					if (flag)
					{
						return true;
					}
				}
				return false;
			}

			public int Count;

			public unsafe DynamicHeap.Page* Head;

			public unsafe DynamicHeap.Page* Tail;
		}

		[Flags]
		private enum ObjectFlags : byte
		{
			Tracked = 1,
			Root = 2,
			Pointer = 4,
			Simple = 8,
			ForceAlive = 16,
			Garbage = 32
		}

		[AttributeUsage(AttributeTargets.Field)]
		public sealed class Ignore : Attribute
		{
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct Object
		{
			public const int SIZE = 8;

			public const int WORDS = 1;

			[FieldOffset(0)]
			public DynamicHeap.ObjectFlags Flags;

			[FieldOffset(1)]
			public byte Block;

			[FieldOffset(2)]
			public ushort Gen;

			[FieldOffset(4)]
			public ushort Type;

			[FieldOffset(6)]
			public ushort Array;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct ObjectFree
		{
			public const int SIZE = 8;

			public const int WORDS = 1;

			[FieldOffset(4)]
			public int Next;
		}

		private struct Block
		{
			public byte Index;

			public unsafe DynamicHeap.Block* Prev;

			public unsafe DynamicHeap.Block* Next;

			public unsafe DynamicHeap.Page* Pages;

			public DynamicHeap.PageList PagesFree;

			public unsafe byte* Memory;
		}

		private struct Page
		{
			public unsafe DynamicHeap.Block* Block;

			public int Index;

			public unsafe DynamicHeap.Page* Prev;

			public unsafe DynamicHeap.Page* Next;

			public int Bin;

			public int Use;

			public unsafe byte* Memory;

			public unsafe DynamicHeap.ObjectFree* ObjectsFree;

			public int ObjectsFreeCount;

			public int ObjectsComitted;

			public int ObjectsAllocated;
		}

		private struct Bin
		{
			public int Index;

			public DynamicHeap.PageList Pages;

			public int ObjectWords;

			public int ObjectStride;

			public int ObjectCapacity;
		}

		private static class BinSizes
		{
			public static readonly int[] Sizes = new int[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				10,
				12,
				14,
				16,
				20,
				24,
				28,
				32,
				40,
				48,
				56,
				64,
				80,
				96,
				112,
				128,
				160,
				192,
				224,
				256,
				320,
				384,
				448,
				512,
				640,
				768,
				896,
				1024,
				1280,
				1536,
				1792,
				2048,
				2560,
				3072,
				3584,
				4096,
				5120,
				6144,
				7168,
				8192
			};
		}
	}
}
