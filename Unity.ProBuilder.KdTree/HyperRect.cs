using System;

namespace UnityEngine.ProBuilder.KdTree
{
	internal struct HyperRect<T>
	{
		public T[] MinPoint
		{
			get
			{
				return this.minPoint;
			}
			set
			{
				this.minPoint = new T[value.Length];
				value.CopyTo(this.minPoint, 0);
			}
		}

		public T[] MaxPoint
		{
			get
			{
				return this.maxPoint;
			}
			set
			{
				this.maxPoint = new T[value.Length];
				value.CopyTo(this.maxPoint, 0);
			}
		}

		public static HyperRect<T> Infinite(int dimensions, ITypeMath<T> math)
		{
			HyperRect<T> result = default(HyperRect<T>);
			result.MinPoint = new T[dimensions];
			result.MaxPoint = new T[dimensions];
			for (int i = 0; i < dimensions; i++)
			{
				result.MinPoint[i] = math.NegativeInfinity;
				result.MaxPoint[i] = math.PositiveInfinity;
			}
			return result;
		}

		public T[] GetClosestPoint(T[] toPoint, ITypeMath<T> math)
		{
			T[] array = new T[toPoint.Length];
			for (int i = 0; i < toPoint.Length; i++)
			{
				if (math.Compare(this.minPoint[i], toPoint[i]) > 0)
				{
					array[i] = this.minPoint[i];
				}
				else if (math.Compare(this.maxPoint[i], toPoint[i]) < 0)
				{
					array[i] = this.maxPoint[i];
				}
				else
				{
					array[i] = toPoint[i];
				}
			}
			return array;
		}

		public HyperRect<T> Clone()
		{
			return new HyperRect<T>
			{
				MinPoint = this.MinPoint,
				MaxPoint = this.MaxPoint
			};
		}

		private T[] minPoint;

		private T[] maxPoint;
	}
}
