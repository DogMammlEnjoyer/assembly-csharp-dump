using System;
using UnityEngine;

namespace Fusion.LagCompensation
{
	public class BVHNodeDrawInfo
	{
		internal BVHNodeDrawInfo(HitboxBuffer buffer)
		{
			this.Buffer = buffer;
		}

		public Bounds Bounds
		{
			get
			{
				return this.Buffer.BVH.GetNode(this.NodeIndex).Box;
			}
		}

		public int Depth
		{
			get
			{
				return this.Buffer.BVH.GetNode(this.NodeIndex).Depth;
			}
		}

		public int MaxDepth
		{
			get
			{
				return this.Buffer.BVH.maxDepth;
			}
		}

		internal BVHNodeDrawInfo FromBVHNode(ref BVHNode node)
		{
			this.NodeIndex = node.Index;
			return this;
		}

		internal HitboxBuffer Buffer;

		internal int NodeIndex;
	}
}
