using System;
using Unity.Mathematics;
using UnityEngine.Splines.ExtrusionShapes;

namespace UnityEngine.Splines
{
	public struct ExtrudeSettings<T> where T : IExtrudeShape
	{
		public int SegmentCount
		{
			get
			{
				return this.m_SegmentCount;
			}
			set
			{
				this.m_SegmentCount = math.clamp(value, 2, 4096);
			}
		}

		public bool CapEnds
		{
			get
			{
				return this.m_CapEnds;
			}
			set
			{
				this.m_CapEnds = value;
			}
		}

		public bool FlipNormals
		{
			get
			{
				return this.m_FlipNormals;
			}
			set
			{
				this.m_FlipNormals = value;
			}
		}

		public float2 Range
		{
			get
			{
				return this.m_Range;
			}
			set
			{
				this.m_Range = math.clamp(new float2(math.min(value.x, value.y), math.max(value.x, value.y)), 0f, 1f);
			}
		}

		public float Radius
		{
			get
			{
				return this.m_Radius;
			}
			set
			{
				this.m_Radius = math.clamp(value, 1E-05f, 10000f);
			}
		}

		public T Shape
		{
			get
			{
				return this.m_Shape;
			}
			set
			{
				this.m_Shape = value;
			}
		}

		internal bool DoCapEnds<K>(K spline) where K : ISpline
		{
			return this.m_CapEnds && !spline.Closed;
		}

		internal bool DoCloseSpline<K>(K spline) where K : ISpline
		{
			return math.abs(1f - (this.Range.y - this.Range.x)) < float.Epsilon && spline.Closed;
		}

		internal int sides
		{
			get
			{
				if (this.Shape is SplineShape)
				{
					T shape;
					if (!this.wrapped)
					{
						shape = this.Shape;
						return shape.SideCount;
					}
					shape = this.Shape;
					return shape.SideCount + 1;
				}
				else
				{
					T shape;
					if (!this.wrapped)
					{
						shape = this.Shape;
						return shape.SideCount + 1;
					}
					shape = this.Shape;
					return shape.SideCount;
				}
			}
		}

		internal bool wrapped
		{
			get
			{
				SplineShape splineShape = this.Shape as SplineShape;
				if (splineShape != null && splineShape.Spline != null)
				{
					return splineShape.Spline.Closed;
				}
				return !(this.Shape is Road);
			}
		}

		public ExtrudeSettings(T shape)
		{
			this = new ExtrudeSettings<T>(16, false, new float2(0f, 1f), 0.5f, shape);
		}

		public ExtrudeSettings(int segments, bool capped, float2 range, float radius, T shape)
		{
			this.m_SegmentCount = math.clamp(segments, 2, 4096);
			this.m_FlipNormals = false;
			this.m_Range = math.clamp(new float2(math.min(range.x, range.y), math.max(range.x, range.y)), 0f, 1f);
			this.m_CapEnds = capped;
			this.m_Radius = math.clamp(radius, 1E-05f, 10000f);
			this.m_Shape = shape;
		}

		private const int k_SegmentsMin = 2;

		private const int k_SegmentsMax = 4096;

		private const float k_RadiusMin = 1E-05f;

		private const float k_RadiusMax = 10000f;

		[SerializeField]
		private T m_Shape;

		[SerializeField]
		private bool m_CapEnds;

		[SerializeField]
		private bool m_FlipNormals;

		[SerializeField]
		private int m_SegmentCount;

		[SerializeField]
		private float m_Radius;

		[SerializeField]
		private Vector2 m_Range;
	}
}
