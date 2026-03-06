using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class TriangleBinsGrid2d
	{
		public TriangleBinsGrid2d(AxisAlignedBox2d bounds, int numCells)
		{
			this.bounds = bounds;
			double num = bounds.MaxDim / (double)numCells;
			Vector2d origin = bounds.Min - num * 0.5 * Vector2d.One;
			this.indexer = new ShiftGridIndexer2(origin, num);
			this.bins_x = (int)(bounds.Width / num) + 2;
			this.bins_y = (int)(bounds.Height / num) + 2;
			this.grid_bounds = new AxisAlignedBox2i(0, 0, this.bins_x - 1, this.bins_y - 1);
			this.bins_list = new SmallListSet();
			this.bins_list.Resize(this.bins_x * this.bins_y);
		}

		public AxisAlignedBox2d Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public void InsertTriangle(int triangle_id, ref Vector2d a, ref Vector2d b, ref Vector2d c)
		{
			this.insert_triangle(triangle_id, ref a, ref b, ref c, true);
		}

		public void InsertTriangleUnsafe(int triangle_id, ref Vector2d a, ref Vector2d b, ref Vector2d c)
		{
			this.insert_triangle(triangle_id, ref a, ref b, ref c, false);
		}

		public void RemoveTriangle(int triangle_id, ref Vector2d a, ref Vector2d b, ref Vector2d c)
		{
			this.remove_triangle(triangle_id, ref a, ref b, ref c, true);
		}

		public void RemoveTriangleUnsafe(int triangle_id, ref Vector2d a, ref Vector2d b, ref Vector2d c)
		{
			this.remove_triangle(triangle_id, ref a, ref b, ref c, false);
		}

		public int FindContainingTriangle(Vector2d query_pt, Func<int, Vector2d, bool> containsF, Func<int, bool> ignoreF = null)
		{
			Vector2i vector2i = this.indexer.ToGrid(query_pt);
			if (!this.grid_bounds.Contains(vector2i))
			{
				return -1;
			}
			int list_index = vector2i.y * this.bins_x + vector2i.x;
			if (ignoreF == null)
			{
				using (IEnumerator<int> enumerator = this.bins_list.ValueItr(list_index).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int num = enumerator.Current;
						if (containsF(num, query_pt))
						{
							return num;
						}
					}
					return -1;
				}
			}
			foreach (int num2 in this.bins_list.ValueItr(list_index))
			{
				if (!ignoreF(num2) && containsF(num2, query_pt))
				{
					return num2;
				}
			}
			return -1;
		}

		public void FindTrianglesInRange(AxisAlignedBox2d range, HashSet<int> triangles)
		{
			Vector2i vector2i = this.indexer.ToGrid(range.Min);
			if (!this.grid_bounds.Contains(vector2i))
			{
				throw new Exception("TriangleBinsGrid2d.FindTrianglesInRange: range.Min is out of bounds");
			}
			Vector2i vector2i2 = this.indexer.ToGrid(range.Max);
			if (!this.grid_bounds.Contains(vector2i2))
			{
				throw new Exception("TriangleBinsGrid2d.FindTrianglesInRange: range.Max is out of bounds");
			}
			for (int i = vector2i.y; i <= vector2i2.y; i++)
			{
				for (int j = vector2i.x; j <= vector2i2.x; j++)
				{
					int list_index = i * this.bins_x + j;
					foreach (int item in this.bins_list.ValueItr(list_index))
					{
						triangles.Add(item);
					}
				}
			}
		}

		private void insert_triangle(int triangle_id, ref Vector2d a, ref Vector2d b, ref Vector2d c, bool threadsafe = true)
		{
			bool flag = false;
			while (threadsafe && !flag)
			{
				this.spinlock.Enter(ref flag);
			}
			AxisAlignedBox2d axisAlignedBox2d = BoundsUtil.Bounds(ref a, ref b, ref c);
			Vector2i vector2i = this.indexer.ToGrid(axisAlignedBox2d.Min);
			Vector2i vector2i2 = this.indexer.ToGrid(axisAlignedBox2d.Max);
			for (int i = vector2i.y; i <= vector2i2.y; i++)
			{
				for (int j = vector2i.x; j <= vector2i2.x; j++)
				{
					int list_index = i * this.bins_x + j;
					this.bins_list.Insert(list_index, triangle_id);
				}
			}
			if (flag)
			{
				this.spinlock.Exit();
			}
		}

		private void remove_triangle(int triangle_id, ref Vector2d a, ref Vector2d b, ref Vector2d c, bool threadsafe = true)
		{
			bool flag = false;
			while (threadsafe && !flag)
			{
				this.spinlock.Enter(ref flag);
			}
			AxisAlignedBox2d axisAlignedBox2d = BoundsUtil.Bounds(ref a, ref b, ref c);
			Vector2i vector2i = this.indexer.ToGrid(axisAlignedBox2d.Min);
			Vector2i vector2i2 = this.indexer.ToGrid(axisAlignedBox2d.Max);
			for (int i = vector2i.y; i <= vector2i2.y; i++)
			{
				for (int j = vector2i.x; j <= vector2i2.x; j++)
				{
					int list_index = i * this.bins_x + j;
					this.bins_list.Remove(list_index, triangle_id);
				}
			}
			if (flag)
			{
				this.spinlock.Exit();
			}
		}

		private ShiftGridIndexer2 indexer;

		private AxisAlignedBox2d bounds;

		private SmallListSet bins_list;

		private int bins_x;

		private int bins_y;

		private AxisAlignedBox2i grid_bounds;

		private SpinLock spinlock;
	}
}
