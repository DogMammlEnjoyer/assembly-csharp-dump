using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace UnityEngine.Splines
{
	[Serializable]
	public class SplineData<T> : IEnumerable<DataPoint<T>>, IEnumerable, ISplineModificationHandler
	{
		public DataPoint<T> this[int index]
		{
			get
			{
				return this.m_DataPoints[index];
			}
			set
			{
				this.SetDataPoint(index, value);
			}
		}

		public PathIndexUnit PathIndexUnit
		{
			get
			{
				return this.m_IndexUnit;
			}
			set
			{
				this.m_IndexUnit = value;
			}
		}

		public T DefaultValue
		{
			get
			{
				return this.m_DefaultValue;
			}
			set
			{
				this.m_DefaultValue = value;
			}
		}

		public int Count
		{
			get
			{
				return this.m_DataPoints.Count;
			}
		}

		public IEnumerable<float> Indexes
		{
			get
			{
				return from dp in this.m_DataPoints
				select dp.Index;
			}
		}

		[Obsolete("Use Changed instead.", false)]
		public event Action changed;

		public event Action Changed;

		public SplineData()
		{
		}

		public SplineData(T init)
		{
			this.Add(0f, init);
			this.SetDirty();
		}

		public SplineData(IEnumerable<DataPoint<T>> dataPoints)
		{
			foreach (DataPoint<T> dataPoint in dataPoints)
			{
				this.Add(dataPoint);
			}
			this.SetDirty();
		}

		private void SetDirty()
		{
			Action action = this.changed;
			if (action != null)
			{
				action();
			}
			Action action2 = this.Changed;
			if (action2 == null)
			{
				return;
			}
			action2();
		}

		public void Add(float t, T data)
		{
			this.Add(new DataPoint<T>(t, data));
		}

		public int Add(DataPoint<T> dataPoint)
		{
			int num = this.m_DataPoints.BinarySearch(0, this.Count, dataPoint, SplineData<T>.k_DataPointComparer);
			num = ((num < 0) ? (~num) : num);
			this.m_DataPoints.Insert(num, dataPoint);
			this.SetDirty();
			return num;
		}

		public int AddDataPointWithDefaultValue(float t, bool useDefaultValue = false)
		{
			DataPoint<T> dataPoint = new DataPoint<T>(t, this.m_DefaultValue);
			if (this.Count == 0 || useDefaultValue)
			{
				return this.Add(dataPoint);
			}
			if (this.Count == 1)
			{
				dataPoint.Value = this.m_DataPoints[0].Value;
				return this.Add(dataPoint);
			}
			int num = this.m_DataPoints.BinarySearch(0, this.Count, dataPoint, SplineData<T>.k_DataPointComparer);
			num = ((num < 0) ? (~num) : num);
			dataPoint.Value = ((num == 0) ? this.m_DataPoints[0].Value : this.m_DataPoints[num - 1].Value);
			this.m_DataPoints.Insert(num, dataPoint);
			this.SetDirty();
			return num;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.m_DataPoints.RemoveAt(index);
			this.SetDirty();
		}

		public bool RemoveDataPoint(float t)
		{
			bool flag = this.m_DataPoints.Remove(this.m_DataPoints.FirstOrDefault((DataPoint<T> point) => Mathf.Approximately(point.Index, t)));
			if (flag)
			{
				this.SetDirty();
			}
			return flag;
		}

		public int MoveDataPoint(int index, float newIndex)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			DataPoint<T> dataPoint = this.m_DataPoints[index];
			if (Mathf.Approximately(newIndex, dataPoint.Index))
			{
				return index;
			}
			this.RemoveAt(index);
			dataPoint.Index = newIndex;
			return this.Add(dataPoint);
		}

		public void Clear()
		{
			this.m_DataPoints.Clear();
			this.SetDirty();
		}

		private static int Wrap(int value, int lowerBound, int upperBound)
		{
			int num = upperBound - lowerBound + 1;
			if (value < lowerBound)
			{
				value += num * ((lowerBound - value) / num + 1);
			}
			return lowerBound + (value - lowerBound) % num;
		}

		private int ResolveBinaryIndex(int index, bool wrap)
		{
			index = ((index < 0) ? (~index) : index) - 1;
			if (wrap)
			{
				index = SplineData<T>.Wrap(index, 0, this.Count - 1);
			}
			return math.clamp(index, 0, this.Count - 1);
		}

		private ValueTuple<int, int, float> GetIndex(float t, float splineLength, int knotCount, bool closed)
		{
			if (this.Count < 1)
			{
				return default(ValueTuple<int, int, float>);
			}
			this.SortIfNecessary();
			float num = splineLength;
			if (this.m_IndexUnit == PathIndexUnit.Normalized)
			{
				num = 1f;
			}
			else if (this.m_IndexUnit == PathIndexUnit.Knot)
			{
				num = (float)(closed ? knotCount : (knotCount - 1));
			}
			float x = math.ceil(this.m_DataPoints[this.m_DataPoints.Count - 1].Index / num) * num;
			float num2 = closed ? math.max(x, num) : num;
			if (closed)
			{
				if (t < 0f)
				{
					t = num2 + t % num2;
				}
				else
				{
					t %= num2;
				}
			}
			else
			{
				t = math.clamp(t, 0f, num2);
			}
			int index = this.m_DataPoints.BinarySearch(0, this.Count, new DataPoint<T>(t, default(T)), SplineData<T>.k_DataPointComparer);
			int num3 = this.ResolveBinaryIndex(index, closed);
			int num4 = closed ? ((num3 + 1) % this.Count) : math.clamp(num3 + 1, 0, this.Count - 1);
			float index2 = this.m_DataPoints[num3].Index;
			float num5 = this.m_DataPoints[num4].Index;
			if (num3 > num4)
			{
				num5 += num2;
			}
			if (t < index2 && closed)
			{
				t += num2;
			}
			if (index2 == num5)
			{
				return new ValueTuple<int, int, float>(num3, num4, index2);
			}
			return new ValueTuple<int, int, float>(num3, num4, math.abs(math.max(0f, t - index2) / (num5 - index2)));
		}

		public T Evaluate<TSpline, TInterpolator>(TSpline spline, float t, PathIndexUnit indexUnit, TInterpolator interpolator) where TSpline : ISpline where TInterpolator : IInterpolator<T>
		{
			if (indexUnit == this.m_IndexUnit)
			{
				return this.Evaluate<TSpline, TInterpolator>(spline, t, interpolator);
			}
			return this.Evaluate<TSpline, TInterpolator>(spline, spline.ConvertIndexUnit(t, indexUnit, this.m_IndexUnit), interpolator);
		}

		public T Evaluate<TSpline, TInterpolator>(TSpline spline, float t, TInterpolator interpolator) where TSpline : ISpline where TInterpolator : IInterpolator<T>
		{
			int count = spline.Count;
			if (count < 1 || this.m_DataPoints.Count == 0)
			{
				return default(T);
			}
			ValueTuple<int, int, float> index = this.GetIndex(t, spline.GetLength(), count, spline.Closed);
			DataPoint<T> dataPoint = this.m_DataPoints[index.Item1];
			DataPoint<T> dataPoint2 = this.m_DataPoints[index.Item2];
			return interpolator.Interpolate(dataPoint.Value, dataPoint2.Value, index.Item3);
		}

		public void SetDataPoint(int index, DataPoint<T> value)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.RemoveAt(index);
			this.Add(value);
			this.SetDirty();
		}

		public void SetDataPointNoSort(int index, DataPoint<T> value)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.m_NeedsSort = true;
			this.m_DataPoints[index] = value;
		}

		public void SortIfNecessary()
		{
			if (!this.m_NeedsSort)
			{
				return;
			}
			this.m_NeedsSort = false;
			this.m_DataPoints.Sort();
			this.SetDirty();
		}

		internal void ForceSort()
		{
			this.m_NeedsSort = true;
			this.SortIfNecessary();
		}

		public void ConvertPathUnit<TSplineType>(TSplineType spline, PathIndexUnit toUnit) where TSplineType : ISpline
		{
			if (toUnit == this.m_IndexUnit)
			{
				return;
			}
			for (int i = 0; i < this.m_DataPoints.Count; i++)
			{
				DataPoint<T> dataPoint = this.m_DataPoints[i];
				float index = spline.ConvertIndexUnit(dataPoint.Index, this.m_IndexUnit, toUnit);
				this.m_DataPoints[i] = new DataPoint<T>(index, dataPoint.Value);
			}
			this.m_IndexUnit = toUnit;
			this.SetDirty();
		}

		public float GetNormalizedInterpolation<TSplineType>(TSplineType spline, float t) where TSplineType : ISpline
		{
			return SplineUtility.GetNormalizedInterpolation<TSplineType>(spline, t, this.m_IndexUnit);
		}

		public IEnumerator<DataPoint<T>> GetEnumerator()
		{
			int i = 0;
			int c = this.Count;
			while (i < c)
			{
				yield return this.m_DataPoints[i];
				int num = i + 1;
				i = num;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private static float WrapInt(float index, int lowerBound, int upperBound)
		{
			return (float)SplineData<T>.Wrap((int)math.floor(index), lowerBound, upperBound) + math.frac(index);
		}

		private static float ClampInt(float index, int lowerBound, int upperBound)
		{
			return (float)math.clamp((int)math.floor(index), lowerBound, upperBound) + math.frac(index);
		}

		void ISplineModificationHandler.OnSplineModified(SplineModificationData data)
		{
			if (this.m_IndexUnit != PathIndexUnit.Knot)
			{
				return;
			}
			if (data.Modification == SplineModification.KnotModified || data.Modification == SplineModification.KnotReordered || data.Modification == SplineModification.Default)
			{
				return;
			}
			int knotIndex = data.KnotIndex;
			float prevCurveLength = data.PrevCurveLength;
			float nextCurveLength = data.NextCurveLength;
			List<int> list = new List<int>();
			int i = 0;
			int count = this.Count;
			while (i < count)
			{
				DataPoint<T> value = this.m_DataPoints[i];
				int num = (int)math.floor(value.Index);
				float num2 = value.Index - (float)num;
				if (data.Modification == SplineModification.KnotInserted)
				{
					float curveLength = data.Spline.GetCurveLength(data.Spline.PreviousIndex(knotIndex));
					if (num == knotIndex - 1)
					{
						if (num2 < curveLength / prevCurveLength)
						{
							value.Index = (float)num + num2 * (prevCurveLength / curveLength);
							goto IL_2A9;
						}
						value.Index = (float)(num + 1) + (num2 * prevCurveLength - curveLength) / (prevCurveLength - curveLength);
						goto IL_2A9;
					}
					else if (data.Spline.Closed && num == data.Spline.Count - 2 && knotIndex == 0)
					{
						if (num2 < curveLength / prevCurveLength)
						{
							value.Index = (float)(num + 1) + num2 * (prevCurveLength / curveLength);
							goto IL_2A9;
						}
						value.Index = (num2 * prevCurveLength - curveLength) / (prevCurveLength - curveLength);
						goto IL_2A9;
					}
					else
					{
						if (num >= knotIndex)
						{
							value.Index += 1f;
							goto IL_2A9;
						}
						goto IL_2A9;
					}
				}
				else if (data.Modification == SplineModification.KnotRemoved)
				{
					if (knotIndex == -1)
					{
						list.Add(i);
					}
					else
					{
						bool flag = num2 == 0f && num == knotIndex;
						bool flag2 = !data.Spline.Closed && ((num <= 0 && knotIndex == 0) || (knotIndex == data.Spline.Count && math.ceil(value.Index) >= (float)knotIndex));
						if (flag || flag2 || data.Spline.Count == 1)
						{
							list.Add(i);
							goto IL_2A9;
						}
						if (num == knotIndex - 1)
						{
							value.Index = (float)num + num2 * prevCurveLength / (prevCurveLength + nextCurveLength);
							goto IL_2A9;
						}
						if (num == knotIndex)
						{
							value.Index = (float)(num - 1) + (prevCurveLength + num2 * nextCurveLength) / (prevCurveLength + nextCurveLength);
							goto IL_2A9;
						}
						if (data.Spline.Closed && knotIndex == 0 && num == data.Spline.Count)
						{
							value.Index = (float)(num - 1) + num2 * prevCurveLength / (prevCurveLength + nextCurveLength);
							goto IL_2A9;
						}
						if (num >= knotIndex)
						{
							value.Index -= 1f;
							goto IL_2A9;
						}
						goto IL_2A9;
					}
				}
				else
				{
					if (!data.Spline.Closed && math.ceil(value.Index) >= (float)data.Spline.Count)
					{
						list.Add(i);
						goto IL_2A9;
					}
					goto IL_2A9;
				}
				IL_2B8:
				i++;
				continue;
				IL_2A9:
				this.m_DataPoints[i] = value;
				goto IL_2B8;
			}
			for (int j = list.Count - 1; j > -1; j--)
			{
				this.m_DataPoints.RemoveAt(list[j]);
			}
		}

		private static readonly DataPointComparer<DataPoint<T>> k_DataPointComparer = new DataPointComparer<DataPoint<T>>();

		[SerializeField]
		private PathIndexUnit m_IndexUnit = PathIndexUnit.Knot;

		[SerializeField]
		private T m_DefaultValue;

		[SerializeField]
		private List<DataPoint<T>> m_DataPoints = new List<DataPoint<T>>();

		[NonSerialized]
		private bool m_NeedsSort;
	}
}
