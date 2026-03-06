using System;
using System.Collections.Generic;
using System.Threading;

namespace g3
{
	public class PointHashGrid3d<T>
	{
		public PointHashGrid3d(double cellSize, T invalidValue)
		{
			this.Hash = new Dictionary<Vector3i, List<T>>();
			this.Indexer = new ScaleGridIndexer3
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

		public void InsertPoint(T value, Vector3d pos)
		{
			Vector3i idx = this.Indexer.ToGrid(pos);
			this.insert_point(value, idx, true);
		}

		public void InsertPointUnsafe(T value, Vector3d pos)
		{
			Vector3i idx = this.Indexer.ToGrid(pos);
			this.insert_point(value, idx, false);
		}

		public bool RemovePoint(T value, Vector3d pos)
		{
			Vector3i idx = this.Indexer.ToGrid(pos);
			return this.remove_point(value, idx, true);
		}

		public bool RemovePointUnsafe(T value, Vector3d pos)
		{
			Vector3i idx = this.Indexer.ToGrid(pos);
			return this.remove_point(value, idx, false);
		}

		public void UpdatePoint(T value, Vector3d old_pos, Vector3d new_pos)
		{
			Vector3i vector3i = this.Indexer.ToGrid(old_pos);
			Vector3i vector3i2 = this.Indexer.ToGrid(new_pos);
			if (vector3i == vector3i2)
			{
				return;
			}
			this.remove_point(value, vector3i, true);
			this.insert_point(value, vector3i2, true);
		}

		public void UpdatePointUnsafe(T value, Vector3d old_pos, Vector3d new_pos)
		{
			Vector3i vector3i = this.Indexer.ToGrid(old_pos);
			Vector3i vector3i2 = this.Indexer.ToGrid(new_pos);
			if (vector3i == vector3i2)
			{
				return;
			}
			this.remove_point(value, vector3i, false);
			this.insert_point(value, vector3i2, false);
		}

		public KeyValuePair<T, double> FindNearestInRadius(Vector3d query_pt, double radius, Func<T, double> distF, Func<T, bool> ignoreF = null)
		{
			Vector3i vector3i = this.Indexer.ToGrid(query_pt - radius * Vector3d.One);
			Vector3i vector3i2 = this.Indexer.ToGrid(query_pt + radius * Vector3d.One);
			double num = double.MaxValue;
			T key = this.invalidValue;
			if (ignoreF == null)
			{
				ignoreF = ((T pt) => false);
			}
			for (int i = vector3i.z; i <= vector3i2.z; i++)
			{
				for (int j = vector3i.y; j <= vector3i2.y; j++)
				{
					for (int k = vector3i.x; k <= vector3i2.x; k++)
					{
						Vector3i key2 = new Vector3i(k, j, i);
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
			}
			return new KeyValuePair<T, double>(key, num);
		}

		private void insert_point(T value, Vector3i idx, bool threadsafe = true)
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

		private bool remove_point(T value, Vector3i idx, bool threadsafe = true)
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

		public void print_large_buckets()
		{
			foreach (KeyValuePair<Vector3i, List<T>> keyValuePair in this.Hash)
			{
				if (keyValuePair.Value.Count > 512)
				{
					Console.WriteLine("{0} : {1}", keyValuePair.Key, keyValuePair.Value.Count);
				}
			}
		}

		private Dictionary<Vector3i, List<T>> Hash;

		private ScaleGridIndexer3 Indexer;

		private T invalidValue;

		private SpinLock spinlock;
	}
}
