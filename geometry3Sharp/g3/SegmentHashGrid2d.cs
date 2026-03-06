using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class SegmentHashGrid2d<T>
	{
		public SegmentHashGrid2d(double cellSize, T invalidValue)
		{
			this.Hash = new Dictionary<Vector2i, List<T>>();
			this.Indexer = new ScaleGridIndexer2
			{
				CellSize = cellSize
			};
			this.MaxExtent = 0.0;
			this.spinlock = default(SpinLock);
			this.invalidValue = invalidValue;
		}

		public void InsertSegment(T value, Vector2d center, double extent)
		{
			Vector2i idx = this.Indexer.ToGrid(center);
			if (extent > this.MaxExtent)
			{
				this.MaxExtent = extent;
			}
			this.insert_segment(value, idx, true);
		}

		public void InsertSegmentUnsafe(T value, Vector2d center, double extent)
		{
			Vector2i idx = this.Indexer.ToGrid(center);
			if (extent > this.MaxExtent)
			{
				this.MaxExtent = extent;
			}
			this.insert_segment(value, idx, false);
		}

		public bool RemoveSegment(T value, Vector2d center)
		{
			Vector2i idx = this.Indexer.ToGrid(center);
			return this.remove_segment(value, idx, true);
		}

		public bool RemoveSegmentUnsafe(T value, Vector2d center)
		{
			Vector2i idx = this.Indexer.ToGrid(center);
			return this.remove_segment(value, idx, false);
		}

		public void UpdateSegment(T value, Vector2d old_center, Vector2d new_center, double new_extent)
		{
			if (new_extent > this.MaxExtent)
			{
				this.MaxExtent = new_extent;
			}
			Vector2i vector2i = this.Indexer.ToGrid(old_center);
			Vector2i vector2i2 = this.Indexer.ToGrid(new_center);
			if (vector2i == vector2i2)
			{
				return;
			}
			this.remove_segment(value, vector2i, true);
			this.insert_segment(value, vector2i2, true);
		}

		public void UpdateSegmentUnsafe(T value, Vector2d old_center, Vector2d new_center, double new_extent)
		{
			if (new_extent > this.MaxExtent)
			{
				this.MaxExtent = new_extent;
			}
			Vector2i vector2i = this.Indexer.ToGrid(old_center);
			Vector2i vector2i2 = this.Indexer.ToGrid(new_center);
			if (vector2i == vector2i2)
			{
				return;
			}
			this.remove_segment(value, vector2i, false);
			this.insert_segment(value, vector2i2, false);
		}

		public KeyValuePair<T, double> FindNearestInRadius(Vector2d query_pt, double radius, Func<T, double> distF, Func<T, bool> ignoreF = null)
		{
			double f = radius + this.MaxExtent;
			Vector2i vector2i = this.Indexer.ToGrid(query_pt - f * Vector2d.One);
			Vector2i vector2i2 = this.Indexer.ToGrid(query_pt + f * Vector2d.One);
			double num = double.MaxValue;
			T key = this.invalidValue;
			if (ignoreF == null)
			{
				ignoreF = ((T pt) => false);
			}
			for (int i = vector2i.y; i <= vector2i2.y; i++)
			{
				for (int j = vector2i.x; j <= vector2i2.x; j++)
				{
					Vector2i key2 = new Vector2i(j, i);
					List<T> list;
					if (this.Hash.TryGetValue(key2, out list))
					{
						foreach (T t in list)
						{
							if (!ignoreF(t))
							{
								double num2 = distF(t);
								if (num2 < radius && num2 < num)
								{
									key = t;
									num = num2;
								}
							}
						}
					}
				}
			}
			return new KeyValuePair<T, double>(key, num);
		}

		public KeyValuePair<T, double> FindNearestInSquaredRadius(Vector2d query_pt, double radiusSqr, Func<T, double> distSqrF, Func<T, bool> ignoreF = null)
		{
			double f = Math.Sqrt(radiusSqr) + this.MaxExtent;
			Vector2i vector2i = this.Indexer.ToGrid(query_pt - f * Vector2d.One);
			Vector2i vector2i2 = this.Indexer.ToGrid(query_pt + f * Vector2d.One);
			double num = double.MaxValue;
			T key = this.invalidValue;
			if (ignoreF == null)
			{
				ignoreF = ((T pt) => false);
			}
			for (int i = vector2i.y; i <= vector2i2.y; i++)
			{
				for (int j = vector2i.x; j <= vector2i2.x; j++)
				{
					Vector2i key2 = new Vector2i(j, i);
					List<T> list;
					if (this.Hash.TryGetValue(key2, out list))
					{
						foreach (T t in list)
						{
							if (!ignoreF(t))
							{
								double num2 = distSqrF(t);
								if (num2 < radiusSqr && num2 < num)
								{
									key = t;
									num = num2;
								}
							}
						}
					}
				}
			}
			return new KeyValuePair<T, double>(key, num);
		}

		private void insert_segment(T value, Vector2i idx, bool threadsafe = true)
		{
			bool flag = false;
			while (threadsafe && !flag)
			{
				this.spinlock.Enter(ref flag);
			}
			List<T> list;
			if (this.Hash.TryGetValue(idx, out list))
			{
				list.Add(value);
			}
			else
			{
				this.Hash[idx] = new List<T>
				{
					value
				};
			}
			if (flag)
			{
				this.spinlock.Exit();
			}
		}

		private bool remove_segment(T value, Vector2i idx, bool threadsafe = true)
		{
			bool flag = false;
			while (threadsafe && !flag)
			{
				this.spinlock.Enter(ref flag);
			}
			bool result = false;
			List<T> list;
			if (this.Hash.TryGetValue(idx, out list))
			{
				result = list.Remove(value);
			}
			if (flag)
			{
				this.spinlock.Exit();
			}
			return result;
		}

		private Dictionary<Vector2i, List<T>> Hash;

		private ScaleGridIndexer2 Indexer;

		private double MaxExtent;

		private T invalidValue;

		private SpinLock spinlock;
	}
}
