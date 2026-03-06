using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class PointSet : Triangulatable
	{
		public IList<TriangulationPoint> Points { get; private set; }

		public IList<DelaunayTriangle> Triangles { get; private set; }

		public PointSet(List<TriangulationPoint> points)
		{
			this.Points = new List<TriangulationPoint>(points);
		}

		public virtual TriangulationMode TriangulationMode
		{
			get
			{
				return TriangulationMode.Unconstrained;
			}
		}

		public void AddTriangle(DelaunayTriangle t)
		{
			this.Triangles.Add(t);
		}

		public void AddTriangles(IEnumerable<DelaunayTriangle> list)
		{
			foreach (DelaunayTriangle item in list)
			{
				this.Triangles.Add(item);
			}
		}

		public void ClearTriangles()
		{
			this.Triangles.Clear();
		}

		public virtual void Prepare(TriangulationContext tcx)
		{
			if (this.Triangles == null)
			{
				this.Triangles = new List<DelaunayTriangle>(this.Points.Count);
			}
			else
			{
				this.Triangles.Clear();
			}
			tcx.Points.AddRange(this.Points);
		}
	}
}
