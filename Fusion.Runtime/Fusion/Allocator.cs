using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion.Statistics;

namespace Fusion
{
	public sealed class Allocator : IDisposable
	{
		internal unsafe static void Free<[IsUnmanaged] T>(Allocator allocator, ref T* ptr) where T : struct, ValueType
		{
			bool flag = ptr == (IntPtr)((UIntPtr)0);
			if (!flag)
			{
				T* ptr2 = ptr;
				ptr = (IntPtr)((UIntPtr)0);
				try
				{
					allocator.FreeInternal((void*)ptr2);
				}
				catch (Exception error)
				{
					LogStream logException = InternalLogStreams.LogException;
					if (logException != null)
					{
						logException.Log(error);
					}
				}
			}
		}

		internal unsafe static T* AllocAndClearArray<[IsUnmanaged] T>(Allocator allocator, int length) where T : struct, ValueType
		{
			return (T*)allocator.AllocAndClear(sizeof(T) * length);
		}

		internal unsafe static void* AllocAndClear(Allocator allocator, int size)
		{
			return allocator.AllocAndClear(size);
		}

		internal unsafe static T* AllocAndClear<[IsUnmanaged] T>(Allocator allocator) where T : struct, ValueType
		{
			return (T*)allocator.AllocAndClear(sizeof(T));
		}

		internal static void Dispose(Allocator allocator)
		{
			allocator.Dispose();
		}

		internal Allocator.Config Configuration
		{
			get
			{
				return this._config;
			}
		}

		public unsafe string LogPointerInfo(void* p)
		{
			return string.Format("heap-start:{0} heap-end:{1} ptr:{2}", (IntPtr)((void*)this._heap), (IntPtr)((void*)(this._heap + this._config.HeapSizeUsable)), (IntPtr)p);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe Ptr Ptr(void* p)
		{
			Assert.Check(this.IsPointerInHeap(p));
			Ptr result;
			result.Address = (int)((long)((byte*)p - (byte*)this._root));
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void* Ptr(Ptr ptr)
		{
			Assert.Check(ptr);
			byte* ptr2 = this._root + ptr.Address;
			Assert.Check<int>(this.IsPointerInHeap((void*)ptr2), ptr.Address);
			return (void*)ptr2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe T* Ptr<[IsUnmanaged] T>(Ptr ptr) where T : struct, ValueType
		{
			byte* ptr2 = this._root + ptr.Address;
			Assert.Check<int>(this.IsPointerInHeap((void*)ptr2), ptr.Address);
			return (T*)ptr2;
		}

		internal unsafe bool IsPointerInHeap(void* p)
		{
			return p >= (void*)this._heap && p < (void*)(this._heap + this._config.HeapSizeUsable);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int WordCount(int size)
		{
			Assert.Check(size > 0);
			return size + 7 >> 3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref Allocator.Bucket GetBucket(int index)
		{
			Assert.Check(index >= 0 && index < 57);
			return ref this._buckets[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref Allocator.Bucket GetBucketForBlock(ref Allocator.Block block)
		{
			return this.GetBucket(block.Bucket);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref Allocator.BlockList GetBucketList(int index)
		{
			Assert.Check(index >= 0 && index < 57);
			return ref this._bucketsLists[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref Allocator.Block GetBlock(int index)
		{
			Assert.Check(index >= 0 && index < this._config.BlockCount);
			return ref this._blocks[index];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ref Allocator.Block GetBlock(long index)
		{
			Assert.Check(index >= 0L && index < (long)this._config.BlockCount);
			return ref this._blocks[(int)(checked((IntPtr)index))];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetBlockBucket(long index)
		{
			Assert.Check(index >= 0L && index < (long)this._config.BlockCount);
			return this._blocks[(int)(checked((IntPtr)index))].Bucket;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe ref Allocator.Block GetBlockForPointer(void* ptr)
		{
			Assert.Check(this.IsPointerInHeap(ptr));
			Assert.Check((long)((byte*)ptr - (byte*)this._heap) >> this._config.BlockShift >= 0L);
			Assert.Check((long)((byte*)ptr - (byte*)this._heap) >> this._config.BlockShift < (long)this._config.BlockCount);
			return ref this._blocks[(int)(checked((IntPtr)(unchecked((long)((byte*)ptr - (byte*)this._heap)) >> this._config.BlockShift)))];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe int GetBlockIndexForPointer(void* ptr)
		{
			Assert.Check(this.IsPointerInHeap(ptr));
			Assert.Check((long)((byte*)ptr - (byte*)this._heap) >> this._config.BlockShift >= 0L);
			Assert.Check((long)((byte*)ptr - (byte*)this._heap) >> this._config.BlockShift < (long)this._config.BlockCount);
			return (int)((long)((byte*)ptr - (byte*)this._heap) >> this._config.BlockShift);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe byte* GetBlockMemory(ref Allocator.Block block)
		{
			return this._heap + block.Index * this._config.BlockByteSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe byte* GetBlockMemory(long blockIndex)
		{
			Assert.Check(blockIndex >= 0L && blockIndex < (long)this._config.BlockCount);
			return this._heap + blockIndex * (long)this._config.BlockByteSize;
		}

		internal unsafe bool TryGetSegmentRoot(void* ptr, out void* root, out long segmentIndex)
		{
			bool flag = this.IsPointerInHeap(ptr);
			bool result;
			if (flag)
			{
				long num = (long)((byte*)ptr - (byte*)this._heap) >> this._config.BlockShift;
				ref Allocator.Block block = ref this.GetBlock(num);
				byte* blockMemory = this.GetBlockMemory(num);
				segmentIndex = (long)((byte*)ptr - (byte*)blockMemory) / (long)this._buckets[block.Bucket].SegmentStride;
				root = blockMemory + segmentIndex * (long)this._buckets[block.Bucket].SegmentStride;
				result = true;
			}
			else
			{
				root = (IntPtr)((UIntPtr)0);
				segmentIndex = 0L;
				result = false;
			}
			return result;
		}

		internal unsafe void* GetSegmentRoot(void* ptr)
		{
			void* ptr2;
			long num;
			bool flag = this.TryGetSegmentRoot(ptr, out ptr2, out num);
			void* result;
			if (flag)
			{
				result = ptr2;
			}
			else
			{
				Assert.AlwaysFail("[Allocator] TryGetSegmentRoot failed");
				result = null;
			}
			return result;
		}

		[return: NotNull]
		internal unsafe T* AllocArray<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return (T*)this.Alloc(sizeof(T) * length);
		}

		[return: NotNull]
		internal unsafe T* AllocAndClearArray<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return (T*)this.AllocAndClear(sizeof(T) * length);
		}

		[return: NotNull]
		internal unsafe T* Alloc<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return (T*)this.Alloc(sizeof(T));
		}

		[return: NotNull]
		internal unsafe T* AllocAndClear<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return (T*)this.AllocAndClear(sizeof(T));
		}

		[return: NotNull]
		internal unsafe void* AllocAndClear(int size)
		{
			void* ptr = this.Alloc(size);
			Native.MemClear(ptr, size);
			return ptr;
		}

		internal unsafe int GetTotalSegmentsUsedInBytes()
		{
			int num = 0;
			for (int i = 0; i < 57; i++)
			{
				int segmentStride = this.GetBucket(i).SegmentStride;
				Allocator.BlockList blockList = *this.GetBucketList(i);
				Allocator.Block block;
				for (int j = blockList.Head; j > -1; j = block.Next)
				{
					block = this.GetBlock(j);
					num += block.SegmentsAllocated * segmentStride;
				}
			}
			return num;
		}

		internal unsafe void GetMemorySnapshot(ref MemoryStatisticsSnapshot snapshot)
		{
			bool flag = snapshot.BucketFullBlocksCount == null;
			if (flag)
			{
				snapshot.BucketFullBlocksCount = new int[57];
			}
			bool flag2 = snapshot.BucketUsedBlocksCount == null;
			if (flag2)
			{
				snapshot.BucketUsedBlocksCount = new int[57];
			}
			bool flag3 = snapshot.BucketFreeBlocksCount == null;
			if (flag3)
			{
				snapshot.BucketFreeBlocksCount = new int[57];
			}
			int num = 0;
			Allocator.Block block;
			for (int i = this._blocksFreeList.Head; i > -1; i = block.Next)
			{
				block = this.GetBlock(i);
				num++;
			}
			snapshot.TotalFreeBlocks = num;
			for (int j = 0; j < 57; j++)
			{
				ref Allocator.Bucket bucket = ref this.GetBucket(j);
				Allocator.BlockList blockList = *this.GetBucketList(j);
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				Allocator.Block block2;
				for (int k = blockList.Head; k > -1; k = block2.Next)
				{
					block2 = this.GetBlock(k);
					bool flag4 = block2.SegmentsUsed == bucket.SegmentCapacity;
					if (flag4)
					{
						num2++;
					}
					num3 = block2.SegmentsAllocated;
					num4 = block2.SegmentsFreeCount(this);
				}
				snapshot.BucketFullBlocksCount[j] = num2;
				snapshot.BucketUsedBlocksCount[j] = num3;
				snapshot.BucketFreeBlocksCount[j] = num4;
			}
		}

		internal int GetFreeSegmentsInBytes()
		{
			int totalSegmentsUsedInBytes = this.GetTotalSegmentsUsedInBytes();
			int num = this._config.BlockCount * this._config.BlockByteSize;
			return num - totalSegmentsUsedInBytes;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool CanAllocSize(int size)
		{
			bool flag = size < 1 || size >= this._config.BlockByteSize;
			return !flag;
		}

		internal void ValidateSentinels()
		{
		}

		internal unsafe bool ValidatePointer(void* ptr, out string error)
		{
			bool flag = !this.IsPointerInHeap(ptr);
			bool result;
			if (flag)
			{
				error = string.Format("Pointer doesn't belong to this heap, ptr:{0}, heap-start:{1}, heap-end:{2}", (IntPtr)ptr, (IntPtr)((void*)this._heap), (IntPtr)((void*)(this._heap + this._config.HeapSizeUsable)));
				result = false;
			}
			else
			{
				ref Allocator.Block blockForPointer = ref this.GetBlockForPointer(ptr);
				string text = "";
				Allocator.ValidateSegmentSentinels(ptr, this.GetBucket(blockForPointer.Bucket).SegmentStride, ref text);
				bool flag2 = text.Length > 0;
				if (flag2)
				{
					error = text;
					result = false;
				}
				else
				{
					error = string.Empty;
					result = true;
				}
			}
			return result;
		}

		[return: NotNull]
		private unsafe void* Alloc(int size)
		{
			bool flag = !this.CanAllocSize(size);
			if (flag)
			{
				throw new InvalidOperationException(string.Format("[Allocator] Invalid size. [Size={0}, BlockByteSize={1}]", size, this._config.BlockByteSize));
			}
			size = size;
			Assert.Check(Allocator.WordCount(size) * 8 >= size);
			byte b = this._bucketsMap[Allocator.WordCount(size)];
			int i = (int)b;
			while (i < 57)
			{
				this.DebugVerifyBucketIntegrity(i);
				ref Allocator.Bucket bucket = ref this.GetBucket(i);
				ref Allocator.BlockList bucketList = ref this.GetBucketList(i);
				Assert.Check(bucket.SegmentStride >= size);
				Assert.Check(bucket.SegmentWordCount >= Allocator.WordCount(size));
				bool flag2 = bucketList.Head > -1;
				if (flag2)
				{
					ref Allocator.Block block = ref this.GetBlock(bucketList.Head);
					void* ptr = this.TryAllocateSegmentFromBlock(bucket, ref block, size);
					bool flag3 = ptr != null;
					if (flag3)
					{
						Assert.Check(this.IsPointerInHeap(ptr));
						Assert.Check(block.SegmentsAllocated > 0);
						return ptr;
					}
				}
				bool flag4 = !this._blocksFreeList.IsEmpty;
				if (!flag4)
				{
					i++;
					continue;
				}
				ref Allocator.Block ptr2 = ref this._blocksFreeList.RemoveHead(this);
				Assert.Check(ptr2.SegmentsFree == default(Ptr));
				Assert.Check(ptr2.SegmentsUsed == 0);
				Assert.Check(ptr2.SegmentsAllocated == 0);
				Assert.Check(ptr2.Prev == -1);
				Assert.Check(ptr2.Next == -1);
				Assert.Check(ptr2.Bucket == 255);
				ptr2.Bucket = bucket.Index;
				bucketList.AddFirst(this, ref ptr2);
				void* ptr3 = this.TryAllocateSegmentFromBlock(bucket, ref ptr2, size);
				bool flag5 = ptr3 == null;
				if (flag5)
				{
					throw new Exception(string.Format("[Allocator] Failed to allocate segment from block. [Size={0}, Block={1}]", size, ptr2.ToString()));
				}
				Assert.Check(this.GetBlockIndexForPointer(ptr3) == ptr2.Index);
				Assert.Check(ptr2.SegmentsAllocated > 0);
				Assert.Check(this.IsPointerInHeap(ptr3));
				return ptr3;
			}
			throw new OutOfMemoryException(string.Format("[Allocator] Out of Memory. All buckets are full. [Size={0}]", size));
		}

		private unsafe void* TryAllocateSegmentFromBlock(in Allocator.Bucket bucket, ref Allocator.Block block, int size)
		{
			Assert.Check(bucket.Index == block.Bucket);
			Assert.Check(this.GetBucketList(bucket.Index).Contains(this, ref block));
			Assert.Check(block.SegmentsAllocated >= 0);
			Assert.Check(bucket.SegmentStride >= size);
			bool flag = false;
			Assert.Check(block.SegmentsFreeCount(this) + block.SegmentsAllocated == block.SegmentsUsed);
			bool flag2 = block.SegmentsFree.Address > 0;
			void* ptr;
			int num;
			if (flag2)
			{
				Assert.Check(block.SegmentsUsed > 0);
				ptr = this.Ptr(block.SegmentsFree);
				block.SegmentsFree = ((Allocator.Segment*)ptr)->Next;
				((Allocator.Segment*)ptr)->Next = default(Ptr);
				flag = true;
				num = 666;
			}
			else
			{
				bool flag3 = block.SegmentsUsed < bucket.SegmentCapacity;
				if (flag3)
				{
					byte* blockMemory = this.GetBlockMemory(ref block);
					int num2 = block.SegmentsUsed;
					block.SegmentsUsed = num2 + 1;
					ptr = (void*)((byte*)blockMemory + num2 * bucket.SegmentStride);
					Assert.Check(block.SegmentsUsed <= bucket.SegmentCapacity);
					num = 1;
				}
				else
				{
					ptr = null;
					num = 2;
				}
			}
			bool flag4 = ptr != null;
			if (flag4)
			{
				block.AllocCount++;
				Assert.Check(block.SegmentsAllocated < bucket.SegmentCapacity);
				int num2 = block.SegmentsAllocated + 1;
				block.SegmentsAllocated = num2;
				bool flag5 = num2 == bucket.SegmentCapacity;
				if (flag5)
				{
					ref Allocator.BlockList bucketList = ref this.GetBucketList(bucket.Index);
					bucketList.MoveLast(this, ref block);
				}
				bool flag6 = !this._allocated.Add((IntPtr)ptr);
				if (flag6)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("{0} already in _allocated set", (IntPtr)ptr));
					}
				}
				ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>(ptr, bucket.SegmentStride);
				bool flag7 = flag;
				if (flag7)
				{
					ref ReadOnlySpan<byte> ptr2 = ref readOnlySpan;
					readOnlySpan = ptr2.Slice(4, ptr2.Length - 4);
				}
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					bool flag8 = *readOnlySpan[i] != 170;
					if (flag8)
					{
						void* ptr3;
						long num3;
						Assert.Always(this.TryGetSegmentRoot(ptr, out ptr3, out num3), "TryGetSegmentRoot(result, out _, out var segmentIndex)");
						LogStream logError2 = InternalLogStreams.LogError;
						if (logError2 != null)
						{
							logError2.Log(string.Format("Expected {0:X2} at index [{1}] but found {2} (segmentIndex: {3}) {4}", new object[]
							{
								170,
								i,
								*readOnlySpan[i],
								num3,
								num
							}));
						}
						break;
					}
				}
			}
			Assert.Check(block.SegmentsFreeCount(this) + block.SegmentsAllocated == block.SegmentsUsed);
			this.DebugVerifyBucketIntegrity(bucket.Index);
			return ptr;
		}

		private unsafe void FreeInternal(void* ptr)
		{
			bool flag = !this.IsPointerInHeap(ptr);
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(string.Format("{0} doesn't belong to this heap, ptr:{1}, heap-start:{2}, heap-end:{3}", new object[]
					{
						"ptr",
						(IntPtr)ptr,
						(IntPtr)((void*)this._heap),
						(IntPtr)((void*)(this._heap + this._config.HeapSizeUsable))
					}));
				}
			}
			else
			{
				ref Allocator.Block blockForPointer = ref this.GetBlockForPointer(ptr);
				ptr = ptr;
				bool flag2 = !this._allocated.Remove((IntPtr)ptr);
				if (flag2)
				{
					LogStream logError2 = InternalLogStreams.LogError;
					if (logError2 != null)
					{
						logError2.Log(string.Format("{0} was not in _allocated set, {1}", (IntPtr)ptr, this.LogPointerInfo(ptr)));
					}
				}
				else
				{
					bool flag3 = blockForPointer.SegmentsFreeContains(this, this.Ptr(ptr));
					if (flag3)
					{
						LogStream logError3 = InternalLogStreams.LogError;
						if (logError3 != null)
						{
							logError3.Log(string.Format("{0} was in free list, {1}", (IntPtr)ptr, this.LogPointerInfo(ptr)));
						}
					}
					else
					{
						bool flag4 = blockForPointer.SegmentsAllocated == 0;
						if (flag4)
						{
							LogStream logError4 = InternalLogStreams.LogError;
							if (logError4 != null)
							{
								logError4.Log("block has no segments allocated, " + this.LogPointerInfo(ptr));
							}
						}
						else
						{
							int segmentStride = this._buckets[blockForPointer.Bucket].SegmentStride;
							Allocator.Segment* ptr2 = (Allocator.Segment*)ptr;
							byte* ptr3 = (byte*)ptr2;
							for (int i = 0; i < segmentStride; i++)
							{
								ptr3[i] = 170;
							}
							ptr2->Next = blockForPointer.SegmentsFree;
							blockForPointer.SegmentsFree = this.Ptr((void*)ptr2);
							ref Allocator.Block ptr4 = ref blockForPointer;
							int num = ptr4.SegmentsAllocated - 1;
							ptr4.SegmentsAllocated = num;
							bool flag5 = num == 0;
							if (flag5)
							{
								int bucket = blockForPointer.Bucket;
								ref Allocator.BlockList bucketList = ref this.GetBucketList(bucket);
								bucketList.Remove(this, ref blockForPointer);
								blockForPointer.Bucket = 255;
								blockForPointer.SegmentsFree = default(Ptr);
								blockForPointer.SegmentsUsed = 0;
								blockForPointer.SegmentsAllocated = 0;
								byte* blockMemory = this.GetBlockMemory(ref blockForPointer);
								for (int j = 0; j < this._config.BlockByteSize; j++)
								{
									blockMemory[j] = 170;
								}
								this._blocksFreeList.AddFirst(this, ref blockForPointer);
								this.DebugVerifyBucketIntegrity(bucket);
							}
							else
							{
								Assert.Check(blockForPointer.SegmentsFreeContains(this, this.Ptr(ptr)));
								ref Allocator.Bucket bucketForBlock = ref this.GetBucketForBlock(ref blockForPointer);
								bool flag6 = bucketForBlock.SegmentCapacity == blockForPointer.SegmentsAllocated + 1;
								if (flag6)
								{
									ref Allocator.BlockList bucketList2 = ref this.GetBucketList(blockForPointer.Bucket);
									bucketList2.MoveFirst(this, ref blockForPointer);
								}
								this.DebugVerifyBucketIntegrity(bucketForBlock.Index);
							}
						}
					}
				}
			}
		}

		[Conditional("DEBUG")]
		private unsafe void DebugVerifyBucketIntegrity(int index)
		{
			ref Allocator.Bucket bucket = ref this.GetBucket(index);
			Allocator.BlockList blockList = *this.GetBucketList(index);
			int i = blockList.Head;
			bool flag = false;
			while (i > -1)
			{
				ref Allocator.Block ptr = ref this._blocks[i];
				for (;;)
				{
					bool flag2 = flag;
					if (flag2)
					{
						goto Block_1;
					}
					bool flag3 = ptr.SegmentsAllocated == bucket.SegmentCapacity;
					if (!flag3)
					{
						goto IL_96;
					}
					flag = true;
				}
				IL_CF:
				i = ptr.Next;
				continue;
				Block_1:
				Assert.Check(ptr.SegmentsUsed == bucket.SegmentCapacity);
				Assert.Check(ptr.SegmentsAllocated == bucket.SegmentCapacity);
				Assert.Check(ptr.SegmentsFreeCount(this) == 0);
				goto IL_CF;
				IL_96:
				Assert.Check<int, int>(ptr.SegmentsFreeCount(this) + ptr.SegmentsAllocated == ptr.SegmentsUsed, ptr.SegmentsFreeCount(this) + ptr.SegmentsAllocated, ptr.SegmentsUsed);
				goto IL_CF;
			}
		}

		public void Dispose()
		{
			Native.Free<byte>(ref this._root);
			this._heap = null;
		}

		private unsafe Allocator(Allocator.Config config)
		{
			Assert.Check(sizeof(Ptr) == 4);
			Assert.Check(sizeof(Allocator.Bucket) == 16);
			Assert.Check(sizeof(Allocator.Config) == 12);
			Assert.Check(sizeof(Allocator.Segment) == 4);
			Assert.Check(sizeof(Allocator.BlockList) == 8);
			this._config = config;
			this._buckets = new Allocator.Bucket[57];
			this._bucketsMap = new byte[config.BlockWordCount];
			this._bucketsLists = new Allocator.BlockList[57];
			for (int i = 0; i < 57; i++)
			{
				this._bucketsLists[i] = new Allocator.BlockList();
			}
			this._blocks = new Allocator.Block[config.BlockCount];
			this._blocksFreeList = new Allocator.BlockList();
			Assert.Always(true, "HEAP_ALIGNMENT == Native.ALIGNMENT");
			this._root = (byte*)Native.MallocAndClear(config.HeapSizeAllocated + 8);
			Assert.Check(Native.IsPointerAligned((void*)this._root, 8));
			this._heap = this._root + 8;
			for (int j = 0; j < config.HeapSizeAllocated; j++)
			{
				this._heap[j] = 170;
			}
			for (int k = 0; k < Allocator.AllocatorBucketSize.Sizes.Length; k++)
			{
				this._buckets[k] = Allocator.Bucket.Create(k, Allocator.AllocatorBucketSize.Sizes[k], config);
			}
			byte b = 0;
			for (int l = 0; l < config.BlockWordCount; l++)
			{
				bool flag = this._buckets[(int)b].SegmentWordCount < l;
				if (flag)
				{
					b += 1;
				}
				Assert.Check(this._buckets[(int)b].SegmentWordCount >= l);
				this._bucketsMap[l] = b;
			}
			for (int m = 0; m < config.BlockCount; m++)
			{
				ref Allocator.Block ptr = ref this._blocks[m];
				ptr = new Allocator.Block(m);
				this._blocksFreeList.AddLast(this, ref ptr);
			}
		}

		public static Allocator Create(Allocator.Config config)
		{
			return new Allocator(config);
		}

		[Conditional("ENABLE_ALLOCATOR_SENTINELS")]
		private unsafe static void InitSegmentSentinels(void* memory, int size)
		{
			ulong num = 6086271824754218827UL;
			ulong num2 = 14358455476591988877UL;
			Span<byte> dst = new Span<byte>(memory, 0);
			Span<byte> dst2 = new Span<byte>((void*)((byte*)memory + size), 0);
			new ReadOnlySpan<byte>((void*)(&num), 8).RepeatingCopyTo(dst);
			new ReadOnlySpan<byte>((void*)(&num2), 8).RepeatingCopyTo(dst2);
		}

		[Conditional("ENABLE_ALLOCATOR_SENTINELS")]
		private unsafe void ValidateSegmentSentinels(void* memory, int size)
		{
			ulong num = 6086271824754218827UL;
			ulong num2 = 14358455476591988877UL;
			ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>(memory, 0);
			ReadOnlySpan<byte> readOnlySpan2 = new ReadOnlySpan<byte>((void*)((byte*)memory + size), 0);
			bool flag = !readOnlySpan.RepeatingSequenceEqualTo(new ReadOnlySpan<byte>((void*)(&num), 8));
			bool flag2 = !readOnlySpan2.RepeatingSequenceEqualTo(new ReadOnlySpan<byte>((void*)(&num2), 8));
			bool flag3 = flag;
			if (flag3)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(string.Format("MSG500 Leading sentinel mismatch (Ptr:{0}, Size:{1}, Config:{2}): {3}", new object[]
					{
						(IntPtr)memory,
						size,
						this._config,
						BinUtils.BytesToHex(readOnlySpan, 0)
					}));
				}
			}
			bool flag4 = flag2;
			if (flag4)
			{
				LogStream logError2 = InternalLogStreams.LogError;
				if (logError2 != null)
				{
					logError2.Log(string.Format("MSG501 Trailing sentinel mismatch (Ptr:{0}, Size:{1}, Config:{2}): {3}", new object[]
					{
						(IntPtr)memory,
						size,
						this._config,
						BinUtils.BytesToHex(readOnlySpan2, 0)
					}));
				}
			}
		}

		private unsafe static void ValidateSegmentSentinels(void* memory, int size, ref string error)
		{
			ulong num = 6086271824754218827UL;
			ulong num2 = 14358455476591988877UL;
			ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>(memory, 0);
			ReadOnlySpan<byte> readOnlySpan2 = new ReadOnlySpan<byte>((void*)((byte*)memory + size), 0);
			bool flag = !readOnlySpan.RepeatingSequenceEqualTo(new ReadOnlySpan<byte>((void*)(&num), 8));
			bool flag2 = !readOnlySpan2.RepeatingSequenceEqualTo(new ReadOnlySpan<byte>((void*)(&num2), 8));
			bool flag3 = flag && flag2;
			if (flag3)
			{
				error = string.Concat(new string[]
				{
					"Leading & trailing mismatch: [",
					BinUtils.BytesToHex(readOnlySpan, 0),
					"], [",
					BinUtils.BytesToHex(readOnlySpan2, 0),
					"]"
				});
			}
			else
			{
				bool flag4 = flag;
				if (flag4)
				{
					error = "Trailing sentinel mismatch: [" + BinUtils.BytesToHex(readOnlySpan, 0) + "]";
				}
				else
				{
					bool flag5 = flag2;
					if (flag5)
					{
						error = "Trailing sentinel mismatch: [" + BinUtils.BytesToHex(readOnlySpan2, 0) + "]";
					}
				}
			}
		}

		private const int WORD_SHIFT = 3;

		private const int WORD_BYTE_SIZE = 8;

		internal const byte PATTERN = 170;

		public const int HEAP_ALIGNMENT = 8;

		public const int REPLICATE_WORD_SHIFT = 2;

		public const int REPLICATE_WORD_SIZE = 4;

		public const int REPLICATE_WORD_ALIGN = 4;

		public const int BUCKET_COUNT = 57;

		public const byte BUCKET_INVALID = 255;

		private const int PTR_SIZE = 8;

		private unsafe byte* _root;

		private unsafe byte* _heap;

		private readonly Allocator.Block[] _blocks;

		private Allocator.BlockList _blocksFreeList;

		private readonly Allocator.Bucket[] _buckets;

		private readonly byte[] _bucketsMap;

		private readonly Allocator.BlockList[] _bucketsLists;

		private readonly Allocator.Config _config;

		private HashSet<IntPtr> _allocated = new HashSet<IntPtr>();

		private const int SentinelLeadingSize = 0;

		private const int SentinelTrailingSize = 0;

		private const ulong SentinelLeadingPattern = 6086271824754218827UL;

		private const ulong SentinelTrailingPattern = 14358455476591988877UL;

		private const int BLOCK_INVALID = -1;

		private struct Block
		{
			public Block(int index)
			{
				this.SegmentsFree = default(Ptr);
				this.SegmentsUsed = 0;
				this.SegmentsAllocated = 0;
				this.AllocCount = 0;
				this.Prev = -1;
				this.Next = -1;
				this.Bucket = 255;
				this.Index = index;
			}

			public unsafe int SegmentsFreeCount(in Allocator a)
			{
				int num = 0;
				Ptr ptr = this.SegmentsFree;
				while (ptr)
				{
					num++;
					ptr = ((Allocator.Segment*)a.Ptr(ptr))->Next;
				}
				return num;
			}

			public unsafe bool SegmentsFreeContains(in Allocator a, Ptr ptr)
			{
				Assert.Check(ptr);
				Ptr ptr2 = this.SegmentsFree;
				while (ptr2)
				{
					bool flag = ptr2 == ptr;
					if (flag)
					{
						return true;
					}
					ptr2 = ((Allocator.Segment*)a.Ptr(ptr2))->Next;
				}
				return false;
			}

			public override string ToString()
			{
				return string.Format("[Block: Bucket={0}, SegmentsUsed={1}, SegmentsAllocated={2}, Index={3}]", new object[]
				{
					this.Bucket,
					this.SegmentsUsed,
					this.SegmentsAllocated,
					this.Index
				});
			}

			public int Prev;

			public int Next;

			public int Bucket;

			public Ptr SegmentsFree;

			public int SegmentsUsed;

			public int SegmentsAllocated;

			public readonly int Index;

			public int AllocCount;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct BlockList
		{
			public BlockList()
			{
				this.Head = -1;
				this.Tail = -1;
			}

			public bool IsEmpty
			{
				get
				{
					return this.Head == -1;
				}
			}

			public void AddFirst(in Allocator a, ref Allocator.Block item)
			{
				Assert.Check(item.Next == -1);
				Assert.Check(item.Prev == -1);
				Assert.Check(!this.Contains(a, ref item));
				item.Next = this.Head;
				item.Prev = -1;
				bool flag = this.Head > -1;
				if (flag)
				{
					a._blocks[this.Head].Prev = item.Index;
					this.Head = item.Index;
				}
				else
				{
					this.Head = item.Index;
					this.Tail = item.Index;
				}
				Assert.Check(this.Contains(a, ref item));
				this.DebugVerifyListIntegrity(a);
			}

			public void AddLast(in Allocator a, ref Allocator.Block item)
			{
				Assert.Check(item.Next == -1);
				Assert.Check(item.Prev == -1);
				Assert.Check(!this.Contains(a, ref item));
				item.Next = -1;
				item.Prev = this.Tail;
				bool flag = this.Tail > -1;
				if (flag)
				{
					a._blocks[this.Tail].Next = item.Index;
					this.Tail = item.Index;
				}
				else
				{
					this.Head = item.Index;
					this.Tail = item.Index;
				}
				Assert.Check(this.Contains(a, ref item));
				this.DebugVerifyListIntegrity(a);
			}

			public void MoveFirst(in Allocator a, ref Allocator.Block item)
			{
				Assert.Check(this.Contains(a, ref item));
				bool flag = item.Index != this.Head;
				if (flag)
				{
					this.Remove(a, ref item);
					this.AddFirst(a, ref item);
				}
			}

			public void MoveLast(in Allocator a, ref Allocator.Block item)
			{
				Assert.Check(this.Contains(a, ref item));
				bool flag = item.Index != this.Tail;
				if (flag)
				{
					this.Remove(a, ref item);
					this.AddLast(a, ref item);
				}
			}

			public ref Allocator.Block RemoveHead(in Allocator a)
			{
				Assert.Check(!this.IsEmpty);
				ref Allocator.Block ptr = ref a._blocks[this.Head];
				this.Remove(a, ref ptr);
				return ref ptr;
			}

			public void Remove(in Allocator a, ref Allocator.Block item)
			{
				Assert.Check(this.Contains(a, ref item));
				bool flag = item.Prev > -1;
				if (flag)
				{
					a._blocks[item.Prev].Next = item.Next;
				}
				bool flag2 = item.Next > -1;
				if (flag2)
				{
					a._blocks[item.Next].Prev = item.Prev;
				}
				bool flag3 = item.Index == this.Tail;
				if (flag3)
				{
					this.Tail = item.Prev;
				}
				bool flag4 = item.Index == this.Head;
				if (flag4)
				{
					this.Head = item.Next;
				}
				item.Prev = -1;
				item.Next = -1;
				this.DebugVerifyListIntegrity(a);
				Assert.Check(!this.Contains(a, ref item));
			}

			public readonly bool Contains(in Allocator a, ref Allocator.Block item)
			{
				for (int i = this.Head; i > -1; i = a._blocks[i].Next)
				{
					bool flag = i == item.Index;
					if (flag)
					{
						return true;
					}
				}
				return false;
			}

			[Conditional("DEBUG")]
			private readonly void DebugVerifyListIntegrity(in Allocator a)
			{
				Allocator.Block ptr;
				for (int i = this.Head; i > -1; i = ptr.Next)
				{
					ptr = ref a._blocks[i];
					bool flag = i == this.Head;
					if (flag)
					{
						Assert.Check(ptr.Prev == -1);
					}
					bool flag2 = i == this.Tail;
					if (flag2)
					{
						Assert.Check(ptr.Next == -1);
					}
					bool flag3 = i != this.Head && i != this.Tail;
					if (flag3)
					{
						Assert.Check(ptr.Prev > -1);
						Assert.Check(ptr.Next > -1);
					}
				}
			}

			public override string ToString()
			{
				return string.Format("[BlockList: Head={0}, Tail={1}]", this.Head, this.Tail);
			}

			public const int SIZE = 8;

			[FieldOffset(0)]
			public int Head;

			[FieldOffset(4)]
			public int Tail;
		}

		[StructLayout(LayoutKind.Explicit)]
		private readonly struct Bucket
		{
			public Bucket(int index, int stride, int wordCount, int capacity)
			{
				this.Index = index;
				this.SegmentStride = stride;
				this.SegmentWordCount = wordCount;
				this.SegmentCapacity = capacity;
			}

			public static Allocator.Bucket Create(int index, int wordCount, Allocator.Config config)
			{
				return new Allocator.Bucket(index, wordCount * 8, wordCount, (wordCount > 0) ? (config.BlockWordCount / wordCount) : 0);
			}

			public override string ToString()
			{
				return string.Format("[Bucket: Index={0}, SegmentStride={1}, SegmentWordCount={2}, SegmentCapacity={3}]", new object[]
				{
					this.Index,
					this.SegmentStride,
					this.SegmentWordCount,
					this.SegmentCapacity
				});
			}

			public const int SIZE = 16;

			[FieldOffset(0)]
			public readonly int Index;

			[FieldOffset(4)]
			public readonly int SegmentStride;

			[FieldOffset(8)]
			public readonly int SegmentWordCount;

			[FieldOffset(12)]
			public readonly int SegmentCapacity;
		}

		private static class AllocatorBucketSize
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
				8192,
				10240,
				12288,
				14336,
				16384,
				20480,
				24576,
				28672,
				32768
			};
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Config
		{
			public int BlockByteSize
			{
				get
				{
					return 1 << this.BlockShift;
				}
			}

			public int BlockWordCount
			{
				get
				{
					return Allocator.WordCount(this.BlockByteSize);
				}
			}

			public int HeapSizeUsable
			{
				get
				{
					return this.BlockByteSize * this.BlockCount;
				}
			}

			public int HeapSizeAllocated
			{
				get
				{
					return this.HeapSizeUsable + 8;
				}
			}

			public Config(PageSizes shift, int count, int globalsSize)
			{
				this.BlockShift = (int)shift;
				this.BlockCount = Math.Max(1, count);
				this.GlobalsSize = globalsSize;
			}

			public bool Equals(Allocator.Config other)
			{
				return this.BlockShift == other.BlockShift && this.BlockCount == other.BlockCount;
			}

			public override bool Equals(object obj)
			{
				bool result;
				if (obj is Allocator.Config)
				{
					Allocator.Config other = (Allocator.Config)obj;
					result = this.Equals(other);
				}
				else
				{
					result = false;
				}
				return result;
			}

			public override int GetHashCode()
			{
				return this.BlockShift * 397 ^ this.BlockCount;
			}

			public override string ToString()
			{
				return string.Format("[Allocator.Config: {0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}]", new object[]
				{
					12,
					this.BlockShift,
					this.BlockCount,
					this.GlobalsSize,
					this.BlockByteSize,
					this.BlockWordCount,
					this.HeapSizeUsable,
					this.HeapSizeAllocated
				});
			}

			public const int SIZE = 12;

			public const PageSizes DEFAULT_BLOCK_SHIFT = PageSizes._32Kb;

			public const int DEFAULT_BLOCK_COUNT = 256;

			[FieldOffset(0)]
			public int BlockShift;

			[FieldOffset(4)]
			public int BlockCount;

			[FieldOffset(8)]
			public int GlobalsSize;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct Segment
		{
			public const int SIZE = 4;

			[FieldOffset(0)]
			public Ptr Next;
		}
	}
}
