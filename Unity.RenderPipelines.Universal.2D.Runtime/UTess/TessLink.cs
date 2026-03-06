using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct TessLink
	{
		internal static TessLink CreateLink(int count, Allocator allocator)
		{
			TessLink result = default(TessLink);
			result.roots = new NativeArray<int>(count, allocator, NativeArrayOptions.ClearMemory);
			result.ranks = new NativeArray<int>(count, allocator, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < count; i++)
			{
				result.roots[i] = i;
				result.ranks[i] = 0;
			}
			return result;
		}

		internal static void DestroyLink(TessLink link)
		{
			link.ranks.Dispose();
			link.roots.Dispose();
		}

		internal int Find(int x)
		{
			int index = x;
			while (this.roots[x] != x)
			{
				x = this.roots[x];
			}
			while (this.roots[index] != x)
			{
				int num = this.roots[index];
				this.roots[index] = x;
				index = num;
			}
			return x;
		}

		internal void Link(int x, int y)
		{
			int num = this.Find(x);
			int num2 = this.Find(y);
			if (num == num2)
			{
				return;
			}
			int num3 = this.ranks[num];
			int num4 = this.ranks[num2];
			if (num3 < num4)
			{
				this.roots[num] = num2;
				return;
			}
			if (num4 < num3)
			{
				this.roots[num2] = num;
				return;
			}
			this.roots[num2] = num;
			int index = num;
			int value = this.ranks[index] + 1;
			this.ranks[index] = value;
		}

		internal NativeArray<int> roots;

		internal NativeArray<int> ranks;
	}
}
