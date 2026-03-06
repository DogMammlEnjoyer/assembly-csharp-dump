using System;
using System.Collections.Generic;

namespace Pathfinding.Poly2Tri
{
	public class Polygon : Triangulatable
	{
		public Polygon(IList<PolygonPoint> points)
		{
			if (points.Count < 3)
			{
				throw new ArgumentException("List has fewer than 3 points", "points");
			}
			this._points.Capacity = Math.Max(this._points.Capacity, this._points.Count + points.Count);
			for (int i = 0; i < points.Count; i++)
			{
				this._points.Add(points[i]);
			}
			if (points[0].Equals(points[points.Count - 1]))
			{
				this._points.RemoveAt(this._points.Count - 1);
			}
		}

		public Polygon(params PolygonPoint[] points) : this(points)
		{
		}

		public TriangulationMode TriangulationMode
		{
			get
			{
				return TriangulationMode.Polygon;
			}
		}

		public void AddSteinerPoint(TriangulationPoint point)
		{
			if (this._steinerPoints == null)
			{
				this._steinerPoints = new List<TriangulationPoint>();
			}
			this._steinerPoints.Add(point);
		}

		public void AddSteinerPoints(List<TriangulationPoint> points)
		{
			if (this._steinerPoints == null)
			{
				this._steinerPoints = new List<TriangulationPoint>();
			}
			this._steinerPoints.AddRange(points);
		}

		public void ClearSteinerPoints()
		{
			if (this._steinerPoints != null)
			{
				this._steinerPoints.Clear();
			}
		}

		public void AddHole(Polygon poly)
		{
			if (this._holes == null)
			{
				this._holes = new List<Polygon>();
			}
			this._holes.Add(poly);
		}

		public void InsertPointAfter(PolygonPoint point, PolygonPoint newPoint)
		{
			int num = this._points.IndexOf(point);
			if (num == -1)
			{
				throw new ArgumentException("Tried to insert a point into a Polygon after a point not belonging to the Polygon", "point");
			}
			newPoint.Next = point.Next;
			newPoint.Previous = point;
			point.Next.Previous = newPoint;
			point.Next = newPoint;
			this._points.Insert(num + 1, newPoint);
		}

		public void AddPoints(IEnumerable<PolygonPoint> list)
		{
			foreach (PolygonPoint polygonPoint in list)
			{
				polygonPoint.Previous = this._last;
				if (this._last != null)
				{
					polygonPoint.Next = this._last.Next;
					this._last.Next = polygonPoint;
				}
				this._last = polygonPoint;
				this._points.Add(polygonPoint);
			}
			PolygonPoint polygonPoint2 = (PolygonPoint)this._points[0];
			this._last.Next = polygonPoint2;
			polygonPoint2.Previous = this._last;
		}

		public void AddPoint(PolygonPoint p)
		{
			p.Previous = this._last;
			p.Next = this._last.Next;
			this._last.Next = p;
			this._points.Add(p);
		}

		public void RemovePoint(PolygonPoint p)
		{
			PolygonPoint next = p.Next;
			PolygonPoint previous = p.Previous;
			previous.Next = next;
			next.Previous = previous;
			this._points.Remove(p);
		}

		public IList<TriangulationPoint> Points
		{
			get
			{
				return this._points;
			}
		}

		public IList<DelaunayTriangle> Triangles
		{
			get
			{
				return this._triangles;
			}
		}

		public IList<Polygon> Holes
		{
			get
			{
				return this._holes;
			}
		}

		public void AddTriangle(DelaunayTriangle t)
		{
			this._triangles.Add(t);
		}

		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			this._triangles.AddRange(list);
		}

		public void ClearTriangles()
		{
			if (this._triangles != null)
			{
				this._triangles.Clear();
			}
		}

		public void Prepare(TriangulationContext tcx)
		{
			if (this._triangles == null)
			{
				this._triangles = new List<DelaunayTriangle>(this._points.Count);
			}
			else
			{
				this._triangles.Clear();
			}
			for (int i = 0; i < this._points.Count - 1; i++)
			{
				tcx.NewConstraint(this._points[i], this._points[i + 1]);
			}
			tcx.NewConstraint(this._points[0], this._points[this._points.Count - 1]);
			tcx.Points.AddRange(this._points);
			if (this._holes != null)
			{
				foreach (Polygon polygon in this._holes)
				{
					for (int j = 0; j < polygon._points.Count - 1; j++)
					{
						tcx.NewConstraint(polygon._points[j], polygon._points[j + 1]);
					}
					tcx.NewConstraint(polygon._points[0], polygon._points[polygon._points.Count - 1]);
					tcx.Points.AddRange(polygon._points);
				}
			}
			if (this._steinerPoints != null)
			{
				tcx.Points.AddRange(this._steinerPoints);
			}
		}

		protected List<TriangulationPoint> _points = new List<TriangulationPoint>();

		protected List<TriangulationPoint> _steinerPoints;

		protected List<Polygon> _holes;

		protected List<DelaunayTriangle> _triangles;

		protected PolygonPoint _last;
	}
}
