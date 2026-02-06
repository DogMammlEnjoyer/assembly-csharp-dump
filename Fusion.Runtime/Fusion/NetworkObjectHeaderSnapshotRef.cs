using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal readonly ref struct NetworkObjectHeaderSnapshotRef
	{
		public NetworkObjectHeaderSnapshotRef(NetworkObjectHeaderSnapshot snapshot)
		{
			this.<snapshot>P = snapshot;
		}

		public static implicit operator NetworkObjectHeaderSnapshotRef(NetworkObjectHeaderSnapshot snapshot)
		{
			return new NetworkObjectHeaderSnapshotRef(snapshot);
		}

		public Tick Tick
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.<snapshot>P.Tick;
			}
		}

		public ref NetworkObjectHeader Header
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.<snapshot>P.Header;
			}
		}

		public ulong SnapshotCRC
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.<snapshot>P.BuildCRC();
			}
		}

		internal Span<int> Raw
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.<snapshot>P.Raw;
			}
		}

		public void CopyFrom(NetworkObjectMeta target)
		{
			this.<snapshot>P.CopyFrom(target);
		}

		public void CopyFrom(NetworkObjectHeaderSnapshotRef target)
		{
			this.<snapshot>P.CopyFrom(target);
		}

		public void CopyTo(NetworkObjectMeta target)
		{
			this.<snapshot>P.CopyTo(target);
		}

		public void CopyTo(int[] target)
		{
			this.<snapshot>P.CopyTo(target);
		}

		public void CopyTo(NetworkObjectHeaderSnapshotRef target)
		{
			this.<snapshot>P.CopyTo(target);
		}

		internal unsafe int* GetBehaviourPtr(NetworkBehaviour behaviour)
		{
			return this.<snapshot>P.GetBehaviourPtr(behaviour);
		}

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly NetworkObjectHeaderSnapshot <snapshot>P;
	}
}
