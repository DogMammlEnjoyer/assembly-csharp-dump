using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Meta.XR.Acoustics
{
	[Serializable]
	internal sealed class Spectrum : IEnumerable<Spectrum.Point>, IEnumerable
	{
		IEnumerator<Spectrum.Point> IEnumerable<Spectrum.Point>.GetEnumerator()
		{
			return this.points.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.points.GetEnumerator();
		}

		internal void Add(float frequency, float data)
		{
			this.points.Add(new Spectrum.Point(frequency, data));
		}

		internal Spectrum(Spectrum other = null)
		{
			if (other != null)
			{
				this.Clone(other);
			}
		}

		internal void Clone(Spectrum other)
		{
			if (this == other)
			{
				return;
			}
			this.selection = other.selection;
			this.points = new List<Spectrum.Point>(other.points);
		}

		internal void Sort()
		{
			if (this.points.Count != 0)
			{
				Spectrum.Point item = this.points[this.selection];
				this.points.Sort();
				this.selection = this.points.IndexOf(item);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Spectrum.Point point in this.points)
			{
				stringBuilder.Append(string.Format("[{0}, {1}] ", point.frequency, point.data));
			}
			return stringBuilder.ToString();
		}

		internal float this[float f]
		{
			get
			{
				if (this.points.Count > 0)
				{
					Spectrum.Point point = new Spectrum.Point(float.MinValue, 0f);
					Spectrum.Point point2 = new Spectrum.Point(float.MaxValue, 0f);
					foreach (Spectrum.Point point3 in this.points)
					{
						if (point3.frequency < f)
						{
							if (point3.frequency > point.frequency)
							{
								point = point3;
							}
						}
						else if (point3.frequency < point2.frequency)
						{
							point2 = point3;
						}
					}
					if (point.frequency == -3.4028235E+38f)
					{
						point.data = (from p in this.points
						orderby p.frequency
						select p).First<Spectrum.Point>().data;
					}
					if (point2.frequency == 3.4028235E+38f)
					{
						point2.data = (from p in this.points
						orderby p.frequency
						select p).Last<Spectrum.Point>().data;
					}
					return Mathf.Lerp(point.data, point2.data, (f - point.frequency) / (point2.frequency - point.frequency));
				}
				return 0f;
			}
		}

		[SerializeField]
		internal int selection = int.MaxValue;

		[SerializeField]
		internal List<Spectrum.Point> points = new List<Spectrum.Point>();

		[Serializable]
		internal struct Point : IComparable<Spectrum.Point>
		{
			internal Point(float frequency = 0f, float data = 0f)
			{
				this.frequency = frequency;
				this.data = data;
			}

			public int CompareTo(Spectrum.Point other)
			{
				return this.frequency.CompareTo(other.frequency);
			}

			public static implicit operator Spectrum.Point(Vector2 v)
			{
				return new Spectrum.Point(v.x, v.y);
			}

			public static implicit operator Vector2(Spectrum.Point point)
			{
				return new Vector2(point.frequency, point.data);
			}

			public override string ToString()
			{
				return string.Format("({0}Hz, {1:0.00})", this.frequency, this.data);
			}

			[SerializeField]
			internal float frequency;

			[SerializeField]
			internal float data;
		}
	}
}
