using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	internal sealed class Bounds2D
	{
		public Vector2 size
		{
			get
			{
				return this.m_Size;
			}
			set
			{
				this.m_Size = value;
				this.m_Extents.x = this.m_Size.x * 0.5f;
				this.m_Extents.y = this.m_Size.y * 0.5f;
			}
		}

		public Vector2 extents
		{
			get
			{
				return this.m_Extents;
			}
		}

		public Vector2[] corners
		{
			get
			{
				return new Vector2[]
				{
					new Vector2(this.center.x - this.extents.x, this.center.y + this.extents.y),
					new Vector2(this.center.x + this.extents.x, this.center.y + this.extents.y),
					new Vector2(this.center.x - this.extents.x, this.center.y - this.extents.y),
					new Vector2(this.center.x + this.extents.x, this.center.y - this.extents.y)
				};
			}
		}

		public Bounds2D()
		{
		}

		public Bounds2D(Vector2 center, Vector2 size)
		{
			this.center = center;
			this.size = size;
		}

		public Bounds2D(IList<Vector2> points)
		{
			this.SetWithPoints(points);
		}

		public Bounds2D(IList<Vector2> points, IList<int> indexes)
		{
			this.SetWithPoints(points, indexes);
		}

		internal Bounds2D(Vector3[] points, Edge[] edges)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (points.Length != 0 && edges.Length != 0)
			{
				num = points[edges[0].a].x;
				num3 = points[edges[0].a].y;
				num2 = num;
				num4 = num3;
				for (int i = 0; i < edges.Length; i++)
				{
					num = Mathf.Min(num, points[edges[i].a].x);
					num = Mathf.Min(num, points[edges[i].b].x);
					num3 = Mathf.Min(num3, points[edges[i].a].y);
					num3 = Mathf.Min(num3, points[edges[i].b].y);
					num2 = Mathf.Max(num2, points[edges[i].a].x);
					num2 = Mathf.Max(num2, points[edges[i].b].x);
					num4 = Mathf.Max(num4, points[edges[i].a].y);
					num4 = Mathf.Max(num4, points[edges[i].b].y);
				}
			}
			this.center = new Vector2((num + num2) / 2f, (num3 + num4) / 2f);
			this.size = new Vector3(num2 - num, num4 - num3);
		}

		public Bounds2D(Vector2[] points, int length)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (points.Length != 0)
			{
				num = points[0].x;
				num3 = points[0].y;
				num2 = num;
				num4 = num3;
				for (int i = 1; i < length; i++)
				{
					num = Mathf.Min(num, points[i].x);
					num3 = Mathf.Min(num3, points[i].y);
					num2 = Mathf.Max(num2, points[i].x);
					num4 = Mathf.Max(num4, points[i].y);
				}
			}
			this.center = new Vector2((num + num2) / 2f, (num3 + num4) / 2f);
			this.size = new Vector3(num2 - num, num4 - num3);
		}

		public bool ContainsPoint(Vector2 point)
		{
			return point.x <= this.center.x + this.extents.x && point.x >= this.center.x - this.extents.x && point.y <= this.center.y + this.extents.y && point.y >= this.center.y - this.extents.y;
		}

		public bool IntersectsLineSegment(Vector2 lineStart, Vector2 lineEnd)
		{
			if (this.ContainsPoint(lineStart) || this.ContainsPoint(lineEnd))
			{
				return true;
			}
			Vector2[] corners = this.corners;
			return Math.GetLineSegmentIntersect(corners[0], corners[1], lineStart, lineEnd) || Math.GetLineSegmentIntersect(corners[1], corners[3], lineStart, lineEnd) || Math.GetLineSegmentIntersect(corners[3], corners[2], lineStart, lineEnd) || Math.GetLineSegmentIntersect(corners[2], corners[0], lineStart, lineEnd);
		}

		public bool Intersects(Bounds2D bounds)
		{
			Vector2 vector = this.center - bounds.center;
			Vector2 vector2 = this.size + bounds.size;
			return Mathf.Abs(vector.x) * 2f < vector2.x && Mathf.Abs(vector.y) * 2f < vector2.y;
		}

		public bool Intersects(Rect rect)
		{
			Vector2 vector = this.center - rect.center;
			Vector2 vector2 = this.size + rect.size;
			return Mathf.Abs(vector.x) * 2f < vector2.x && Mathf.Abs(vector.y) * 2f < vector2.y;
		}

		public void SetWithPoints(IList<Vector2> points)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int count = points.Count;
			if (count > 0)
			{
				num = points[0].x;
				num3 = points[0].y;
				num2 = num;
				num4 = num3;
				for (int i = 1; i < count; i++)
				{
					float x = points[i].x;
					float y = points[i].y;
					if (x < num)
					{
						num = x;
					}
					if (x > num2)
					{
						num2 = x;
					}
					if (y < num3)
					{
						num3 = y;
					}
					if (y > num4)
					{
						num4 = y;
					}
				}
			}
			this.center.x = (num + num2) / 2f;
			this.center.y = (num3 + num4) / 2f;
			this.m_Size.x = num2 - num;
			this.m_Size.y = num4 - num3;
			this.m_Extents.x = this.m_Size.x * 0.5f;
			this.m_Extents.y = this.m_Size.y * 0.5f;
		}

		public void SetWithPoints(IList<Vector2> points, IList<int> indexes)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (points.Count > 0 && indexes.Count > 0)
			{
				num = points[indexes[0]].x;
				num3 = points[indexes[0]].y;
				num2 = num;
				num4 = num3;
				for (int i = 1; i < indexes.Count; i++)
				{
					float x = points[indexes[i]].x;
					float y = points[indexes[i]].y;
					if (x < num)
					{
						num = x;
					}
					if (x > num2)
					{
						num2 = x;
					}
					if (y < num3)
					{
						num3 = y;
					}
					if (y > num4)
					{
						num4 = y;
					}
				}
			}
			this.center.x = (num + num2) / 2f;
			this.center.y = (num3 + num4) / 2f;
			this.m_Size.x = num2 - num;
			this.m_Size.y = num4 - num3;
			this.m_Extents.x = this.m_Size.x * 0.5f;
			this.m_Extents.y = this.m_Size.y * 0.5f;
		}

		public static Vector2 Center(IList<Vector2> points)
		{
			int count = points.Count;
			float num = points[0].x;
			float num2 = points[0].y;
			float num3 = num;
			float num4 = num2;
			for (int i = 1; i < count; i++)
			{
				float x = points[i].x;
				float y = points[i].y;
				if (x < num)
				{
					num = x;
				}
				if (x > num3)
				{
					num3 = x;
				}
				if (y < num2)
				{
					num2 = y;
				}
				if (y > num4)
				{
					num4 = y;
				}
			}
			return new Vector2((num + num3) / 2f, (num2 + num4) / 2f);
		}

		public static Vector2 Center(IList<Vector2> points, IList<int> indexes)
		{
			int count = indexes.Count;
			float num = points[indexes[0]].x;
			float num2 = points[indexes[0]].y;
			float num3 = num;
			float num4 = num2;
			for (int i = 1; i < count; i++)
			{
				float x = points[indexes[i]].x;
				float y = points[indexes[i]].y;
				if (x < num)
				{
					num = x;
				}
				if (x > num3)
				{
					num3 = x;
				}
				if (y < num2)
				{
					num2 = y;
				}
				if (y > num4)
				{
					num4 = y;
				}
			}
			return new Vector2((num + num3) / 2f, (num2 + num4) / 2f);
		}

		public static Vector2 Size(IList<Vector2> points, IList<int> indexes)
		{
			int count = indexes.Count;
			float num = points[indexes[0]].x;
			float num2 = points[indexes[0]].y;
			float num3 = num;
			float num4 = num2;
			for (int i = 1; i < count; i++)
			{
				float x = points[indexes[i]].x;
				float y = points[indexes[i]].y;
				if (x < num)
				{
					num = x;
				}
				if (x > num3)
				{
					num3 = x;
				}
				if (y < num2)
				{
					num2 = y;
				}
				if (y > num4)
				{
					num4 = y;
				}
			}
			return new Vector2(num3 - num, num4 - num2);
		}

		internal static Vector2 Center(IList<Vector4> points, IEnumerable<int> indexes)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			if (indexes.Any<int>())
			{
				int index = indexes.First<int>();
				num = points[index].x;
				num3 = points[index].y;
				num2 = num;
				num4 = num3;
				foreach (int index2 in indexes)
				{
					float x = points[index2].x;
					float y = points[index2].y;
					if (x < num)
					{
						num = x;
					}
					if (x > num2)
					{
						num2 = x;
					}
					if (y < num3)
					{
						num3 = y;
					}
					if (y > num4)
					{
						num4 = y;
					}
				}
			}
			return new Vector2((num + num2) / 2f, (num3 + num4) / 2f);
		}

		public override string ToString()
		{
			string[] array = new string[5];
			array[0] = "[cen: ";
			int num = 1;
			Vector2 vector = this.center;
			array[num] = vector.ToString();
			array[2] = " size: ";
			array[3] = this.size.ToString();
			array[4] = "]";
			return string.Concat(array);
		}

		public Vector2 center = Vector2.zero;

		[SerializeField]
		private Vector2 m_Size = Vector2.zero;

		[SerializeField]
		private Vector2 m_Extents = Vector2.zero;
	}
}
