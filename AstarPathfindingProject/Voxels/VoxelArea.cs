using System;
using UnityEngine;

namespace Pathfinding.Voxels
{
	public class VoxelArea
	{
		public void Reset()
		{
			this.ResetLinkedVoxelSpans();
			for (int i = 0; i < this.compactCells.Length; i++)
			{
				this.compactCells[i].count = 0U;
				this.compactCells[i].index = 0U;
			}
		}

		private void ResetLinkedVoxelSpans()
		{
			int num = this.linkedSpans.Length;
			this.linkedSpanCount = this.width * this.depth;
			LinkedVoxelSpan linkedVoxelSpan = new LinkedVoxelSpan(uint.MaxValue, uint.MaxValue, -1, -1);
			for (int i = 0; i < num; i++)
			{
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
			}
			this.removedStackCount = 0;
		}

		public VoxelArea(int width, int depth)
		{
			this.width = width;
			this.depth = depth;
			int num = width * depth;
			this.compactCells = new CompactVoxelCell[num];
			this.linkedSpans = new LinkedVoxelSpan[(int)((float)num * 8f) + 15 & -16];
			this.ResetLinkedVoxelSpans();
			int[] array = new int[4];
			array[0] = -1;
			array[2] = 1;
			this.DirectionX = array;
			this.DirectionZ = new int[]
			{
				0,
				width,
				0,
				-width
			};
			this.VectorDirection = new Vector3[]
			{
				Vector3.left,
				Vector3.forward,
				Vector3.right,
				Vector3.back
			};
		}

		public int GetSpanCountAll()
		{
			int num = 0;
			int num2 = this.width * this.depth;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i;
				while (num3 != -1 && this.linkedSpans[num3].bottom != 4294967295U)
				{
					num++;
					num3 = this.linkedSpans[num3].next;
				}
			}
			return num;
		}

		public int GetSpanCount()
		{
			int num = 0;
			int num2 = this.width * this.depth;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i;
				while (num3 != -1 && this.linkedSpans[num3].bottom != 4294967295U)
				{
					if (this.linkedSpans[num3].area != 0)
					{
						num++;
					}
					num3 = this.linkedSpans[num3].next;
				}
			}
			return num;
		}

		private void PushToSpanRemovedStack(int index)
		{
			if (this.removedStackCount == this.removedStack.Length)
			{
				int[] dst = new int[this.removedStackCount * 4];
				Buffer.BlockCopy(this.removedStack, 0, dst, 0, this.removedStackCount * 4);
				this.removedStack = dst;
			}
			this.removedStack[this.removedStackCount] = index;
			this.removedStackCount++;
		}

		public void AddLinkedSpan(int index, uint bottom, uint top, int area, int voxelWalkableClimb)
		{
			LinkedVoxelSpan[] array = this.linkedSpans;
			if (array[index].bottom == 4294967295U)
			{
				array[index] = new LinkedVoxelSpan(bottom, top, area);
				return;
			}
			int num = -1;
			int num2 = index;
			while (index != -1)
			{
				LinkedVoxelSpan linkedVoxelSpan = array[index];
				if (linkedVoxelSpan.bottom > top)
				{
					break;
				}
				if (linkedVoxelSpan.top < bottom)
				{
					num = index;
					index = linkedVoxelSpan.next;
				}
				else
				{
					bottom = Math.Min(linkedVoxelSpan.bottom, bottom);
					top = Math.Max(linkedVoxelSpan.top, top);
					if (Math.Abs((int)(top - linkedVoxelSpan.top)) <= voxelWalkableClimb)
					{
						area = Math.Max(area, linkedVoxelSpan.area);
					}
					int next = linkedVoxelSpan.next;
					if (num != -1)
					{
						array[num].next = next;
						this.PushToSpanRemovedStack(index);
						index = next;
					}
					else
					{
						if (next == -1)
						{
							array[num2] = new LinkedVoxelSpan(bottom, top, area);
							return;
						}
						array[num2] = array[next];
						this.PushToSpanRemovedStack(next);
					}
				}
			}
			if (this.linkedSpanCount >= array.Length)
			{
				LinkedVoxelSpan[] array2 = array;
				int num3 = this.linkedSpanCount;
				int num4 = this.removedStackCount;
				array = (this.linkedSpans = new LinkedVoxelSpan[array.Length * 2]);
				this.ResetLinkedVoxelSpans();
				this.linkedSpanCount = num3;
				this.removedStackCount = num4;
				for (int i = 0; i < this.linkedSpanCount; i++)
				{
					array[i] = array2[i];
				}
			}
			int num5;
			if (this.removedStackCount > 0)
			{
				this.removedStackCount--;
				num5 = this.removedStack[this.removedStackCount];
			}
			else
			{
				num5 = this.linkedSpanCount;
				this.linkedSpanCount++;
			}
			if (num != -1)
			{
				array[num5] = new LinkedVoxelSpan(bottom, top, area, array[num].next);
				array[num].next = num5;
				return;
			}
			array[num5] = array[num2];
			array[num2] = new LinkedVoxelSpan(bottom, top, area, num5);
		}

		public const uint MaxHeight = 65536U;

		public const int MaxHeightInt = 65536;

		public const uint InvalidSpanValue = 4294967295U;

		public const float AvgSpanLayerCountEstimate = 8f;

		public readonly int width;

		public readonly int depth;

		public CompactVoxelSpan[] compactSpans;

		public CompactVoxelCell[] compactCells;

		public int compactSpanCount;

		public ushort[] tmpUShortArr;

		public int[] areaTypes;

		public ushort[] dist;

		public ushort maxDistance;

		public int maxRegions;

		public int[] DirectionX;

		public int[] DirectionZ;

		public Vector3[] VectorDirection;

		private int linkedSpanCount;

		public LinkedVoxelSpan[] linkedSpans;

		private int[] removedStack = new int[128];

		private int removedStackCount;
	}
}
