using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class PointHashGrid2d<T>
	{
		public PointHashGrid2d(double cellSize, T invalidValue)
		{
			this.Hash = new Dictionary<Vector2i, List<T>>();
			this.Indexer = new ScaleGridIndexer2
			{
				CellSize = cellSize
			};
			this.spinlock = default(SpinLock);
			this.invalidValue = invalidValue;
		}

		public T InvalidValue
		{
			get
			{
				return this.invalidValue;
			}
		}

		public void InsertPoint(T value, Vector2d pos)
		{
			Vector2i idx = this.Indexer.ToGrid(pos);
			this.insert_point(value, idx, true);
		}

		public void InsertPointUnsafe(T value, Vector2d pos)
		{
			Vector2i idx = this.Indexer.ToGrid(pos);
			this.insert_point(value, idx, false);
		}

		public bool RemovePoint(T value, Vector2d pos)
		{
			Vector2i idx = this.Indexer.ToGrid(pos);
			return this.remove_point(value, idx, true);
		}

		public bool RemovePointUnsafe(T value, Vector2d pos)
		{
			Vector2i idx = this.Indexer.ToGrid(pos);
			return this.remove_point(value, idx, false);
		}

		public void UpdatePoint(T value, Vector2d old_pos, Vector2d new_pos)
		{
			Vector2i vector2i = this.Indexer.ToGrid(old_pos);
			Vector2i vector2i2 = this.Indexer.ToGrid(new_pos);
			if (vector2i == vector2i2)
			{
				return;
			}
			this.remove_point(value, vector2i, true);
			this.insert_point(value, vector2i2, true);
		}

		public void UpdatePointUnsafe(T value, Vector2d old_pos, Vector2d new_pos)
		{
			Vector2i vector2i = this.Indexer.ToGrid(old_pos);
			Vector2i vector2i2 = this.Indexer.ToGrid(new_pos);
			if (vector2i == vector2i2)
			{
				return;
			}
			this.remove_point(value, vector2i, false);
			this.insert_point(value, vector2i2, false);
		}

		public KeyValuePair<T, double> FindNearestInRadius(Vector2d query_pt, double radius, Func<T, double> distF, Func<T, bool> ignoreF = null)
		{
			Vector2i vector2i = this.Indexer.ToGrid(query_pt - radius * Vector2d.One);
			Vector2i vector2i2 = this.Indexer.ToGrid(query_pt + radius * Vector2d.One);
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

		private void insert_point(T value, Vector2i idx, bool threadsafe = true)
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

		private bool remove_point(T value, Vector2i idx, bool threadsafe = true)
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

		private T invalidValue;

		private SpinLock spinlock;
	}
}
