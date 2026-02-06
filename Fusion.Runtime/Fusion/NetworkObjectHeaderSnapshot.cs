using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal class NetworkObjectHeaderSnapshot
	{
		public NetworkObjectHeaderSnapshot(Allocator allocator)
		{
		}

		public unsafe NetworkObjectHeaderPtr HeaderPtr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new NetworkObjectHeaderPtr((NetworkObjectHeader*)this._ptr);
			}
		}

		public unsafe ref NetworkObjectHeader Header
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ref *(NetworkObjectHeader*)this._ptr;
			}
		}

		public unsafe Span<int> Raw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return new Span<int>((void*)this._ptr, this.WordCount);
			}
		}

		public void Init(NetworkObjectMeta meta, bool copyData)
		{
			this.Init((int)meta.WordCount);
			if (copyData)
			{
				this.CopyFrom(meta);
			}
		}

		public unsafe void Init(int wordCount)
		{
			Assert.Check(this.<allocator>P);
			Assert.Check(this._ptr == null);
			this.WordCount = wordCount;
			this._ptr = (int*)Allocator.AllocAndClear(this.<allocator>P, wordCount * 4);
		}

		public void Release()
		{
			this.Tick = default(Tick);
			this.WordCount = 0;
			Allocator.Free<int>(this.<allocator>P, ref this._ptr);
		}

		public NetworkObjectHeaderSnapshot Clone(Simulation simulation)
		{
			NetworkObjectHeaderSnapshot snapshot = simulation.GetSnapshot();
			snapshot.Init(this.WordCount);
			snapshot.CopyFrom(this);
			return snapshot;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(int[] target)
		{
			Native.MemCpy(target, this.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyFrom(NetworkObjectMeta meta)
		{
			Native.MemCpy(this.Raw, meta.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(NetworkObjectMeta meta)
		{
			Native.MemCpy(meta.Raw, this.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyFrom(NetworkObjectHeaderSnapshot target)
		{
			Native.MemCpy(this.Raw, target.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(NetworkObjectHeaderSnapshot target)
		{
			Native.MemCpy(target.Raw, this.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyFrom(NetworkObjectHeaderSnapshotRef target)
		{
			Native.MemCpy(this.Raw, target.Raw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(NetworkObjectHeaderSnapshotRef target)
		{
			Native.MemCpy(target.Raw, this.Raw);
		}

		internal unsafe int* GetBehaviourPtr(NetworkBehaviour behaviour)
		{
			return this._ptr + behaviour.WordOffset;
		}

		internal ulong BuildCRC()
		{
			return CRC64.Compute(this.Raw);
		}

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Allocator <allocator>P = allocator;

		internal NetworkObjectHeaderSnapshot Prev;

		internal NetworkObjectHeaderSnapshot Next;

		public Tick Tick;

		public int WordCount;

		private unsafe int* _ptr;
	}
}
