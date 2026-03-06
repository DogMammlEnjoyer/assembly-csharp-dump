using System;

namespace Fusion.Sockets
{
	internal struct ReliableBuffer
	{
		public int SequenceBits
		{
			get
			{
				return this._sequencer.Bits;
			}
		}

		public static ReliableBuffer Create()
		{
			return new ReliableBuffer
			{
				_sequencer = new NetSequencer(4)
			};
		}

		public ulong NextSendSequence()
		{
			return this._sequencer.Next();
		}

		public void Dispose()
		{
			this._receiveList.Dispose();
		}

		public unsafe bool LateReceive(out void* root, out ReliableId id, out byte* data)
		{
			for (ReliableHeader* ptr = this._receiveList.Head; ptr != null; ptr = ptr->Next)
			{
				bool flag = this._sequencer.Distance(ptr->Id.Sequence, this._receiveSequence) == 1;
				if (flag)
				{
					this._receiveSequence = ptr->Id.Sequence;
					this._receiveList.Remove(ptr);
					root = ptr;
					id = ptr->Id;
					data = ptr + 1;
					return true;
				}
			}
			id = default(ReliableId);
			root = (IntPtr)((UIntPtr)0);
			data = (IntPtr)((UIntPtr)0);
			return false;
		}

		public unsafe void LateFree(ref void* root)
		{
			Native.Free(ref root);
		}

		public unsafe bool Receive(NetBitBuffer* buffer, out ReliableId rid)
		{
			Assert.Always<int>(sizeof(ReliableHeader) == 64, "ReliableHeader size mismatch {0}", sizeof(ReliableHeader));
			ReliableId reliableId = default(ReliableId);
			buffer->ReadBytesAligned((void*)(&reliableId), 48);
			bool flag = this._sequencer.Distance(reliableId.Sequence, this._receiveSequence) == 1;
			bool result;
			if (flag)
			{
				this._receiveSequence = reliableId.Sequence;
				rid = reliableId;
				result = true;
			}
			else
			{
				Assert.Check(buffer->IsOnEvenByte);
				reliableId.SliceLength = buffer->LengthBytes - buffer->OffsetBytes;
				byte* ptr = (byte*)Native.Malloc(reliableId.SliceLength + sizeof(ReliableHeader));
				Native.MemCpy((void*)(ptr + sizeof(ReliableHeader)), (void*)buffer->PadToByteBoundaryAndGetPtr(), reliableId.SliceLength);
				ReliableHeader* ptr2 = (ReliableHeader*)ptr;
				ptr2->Id = reliableId;
				this._receiveList.AddLast(ptr2);
				rid = default(ReliableId);
				result = false;
			}
			return result;
		}

		public const int SEQ_BYTES = 4;

		private NetSequencer _sequencer;

		private ReliableList _receiveList;

		private ulong _receiveSequence;
	}
}
