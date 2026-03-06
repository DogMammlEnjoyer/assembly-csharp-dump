using System;
using System.Threading;

namespace Fusion.Sockets
{
	internal struct NetBitBufferBlock
	{
		public unsafe static void Dispose(ref NetBitBufferBlock* block)
		{
			bool flag = block == (IntPtr)((UIntPtr)0);
			if (!flag)
			{
				NetBitBuffer* allocNext;
				for (NetBitBuffer* ptr = block._allocatedHead; ptr != null; ptr = allocNext)
				{
					Assert.Check(ptr->_block == block);
					allocNext = ptr->_allocNext;
					ptr->_block = null;
					Native.Free<NetBitBuffer>(ref ptr);
				}
				Native.Free<NetBitBufferBlock>(ref block);
			}
		}

		public unsafe static NetBitBufferBlock* Create(int packetSize)
		{
			NetBitBufferBlock* ptr = Native.MallocAndClear<NetBitBufferBlock>();
			ptr->_self = ptr;
			ptr->_freeHead = 0;
			ptr->_packetSize = packetSize;
			return ptr;
		}

		public unsafe void Release(NetBitBuffer* ptr)
		{
			Assert.Check(ptr->_block == this._self);
			IntPtr freeHead;
			do
			{
				freeHead = this._freeHead;
				ptr->Next = (NetBitBuffer*)((void*)freeHead);
			}
			while (Interlocked.CompareExchange(ref this._freeHead, (IntPtr)((void*)ptr), freeHead) != freeHead);
		}

		public unsafe NetBitBuffer* TryAcquire()
		{
			NetBitBuffer* ptr;
			bool flag = this.TryAcquire(out ptr);
			NetBitBuffer* result;
			if (flag)
			{
				result = ptr;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public unsafe bool TryAcquire(out NetBitBuffer* ptr)
		{
			IntPtr intPtr;
			for (;;)
			{
				intPtr = Volatile.Read(ref this._freeHead);
				bool flag = intPtr == IntPtr.Zero;
				if (flag)
				{
					break;
				}
				if (!(Interlocked.CompareExchange(ref this._freeHead, (IntPtr)((void*)((NetBitBuffer*)((void*)intPtr))->Next), intPtr) != intPtr))
				{
					goto IL_7B;
				}
			}
			NetBitBuffer* ptr2 = NetBitBuffer.Allocate(0, this._packetSize);
			ptr2->_block = this._self;
			ptr2->_allocNext = this._allocatedHead;
			this._allocatedHead = ptr2;
			intPtr = new IntPtr((void*)ptr2);
			IL_7B:
			ptr = (void*)intPtr;
			Assert.Check(ptr._block == this._self);
			ptr.SetBufferLengthBytes(ptr.Data, this._packetSize);
			Native.MemClear((void*)ptr.Data, this._packetSize);
			ptr.OffsetBits = 0;
			ptr._block = this._self;
			return true;
		}

		private int _packetSize;

		private IntPtr _freeHead;

		private unsafe NetBitBufferBlock* _self;

		private unsafe NetBitBuffer* _allocatedHead;
	}
}
