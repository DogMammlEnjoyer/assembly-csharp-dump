using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion.LagCompensation
{
	internal class Mapper
	{
		internal int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._rootToNodeIndex.Count;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool TryGetLeafIndex(HitboxRoot root, out int index)
		{
			return this._rootToNodeIndex.TryGetValue(root, out index);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int GetLeafIndex(HitboxRoot root)
		{
			Assert.Check(this._rootToNodeIndex.ContainsKey(root));
			return this._rootToNodeIndex[root];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void RegisterMapping(HitboxRoot root, int leafIndex)
		{
			this._rootToNodeIndex[root] = leafIndex;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void DeRegister(HitboxRoot root)
		{
			this._rootToNodeIndex.Remove(root);
		}

		private readonly Dictionary<HitboxRoot, int> _rootToNodeIndex = new Dictionary<HitboxRoot, int>();
	}
}
