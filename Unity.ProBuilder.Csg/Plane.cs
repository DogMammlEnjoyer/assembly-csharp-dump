using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.Csg
{
	internal sealed class Plane
	{
		public Plane()
		{
			this.normal = Vector3.zero;
			this.w = 0f;
		}

		public Plane(Vector3 a, Vector3 b, Vector3 c)
		{
			this.normal = Vector3.Cross(b - a, c - a);
			this.w = Vector3.Dot(this.normal, a);
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", this.normal, this.w);
		}

		public bool Valid()
		{
			return this.normal.magnitude > 0f;
		}

		public void Flip()
		{
			this.normal *= -1f;
			this.w *= -1f;
		}

		public void SplitPolygon(Polygon polygon, List<Polygon> coplanarFront, List<Polygon> coplanarBack, List<Polygon> front, List<Polygon> back)
		{
			Plane.EPolygonType epolygonType = Plane.EPolygonType.Coplanar;
			List<Plane.EPolygonType> list = new List<Plane.EPolygonType>();
			for (int i = 0; i < polygon.vertices.Count; i++)
			{
				float num = Vector3.Dot(this.normal, polygon.vertices[i].position) - this.w;
				Plane.EPolygonType epolygonType2 = (num < -CSG.epsilon) ? Plane.EPolygonType.Back : ((num > CSG.epsilon) ? Plane.EPolygonType.Front : Plane.EPolygonType.Coplanar);
				epolygonType |= epolygonType2;
				list.Add(epolygonType2);
			}
			switch (epolygonType)
			{
			case Plane.EPolygonType.Coplanar:
				if (Vector3.Dot(this.normal, polygon.plane.normal) > 0f)
				{
					coplanarFront.Add(polygon);
					return;
				}
				coplanarBack.Add(polygon);
				return;
			case Plane.EPolygonType.Front:
				front.Add(polygon);
				return;
			case Plane.EPolygonType.Back:
				back.Add(polygon);
				return;
			case Plane.EPolygonType.Spanning:
			{
				List<Vertex> list2 = new List<Vertex>();
				List<Vertex> list3 = new List<Vertex>();
				for (int j = 0; j < polygon.vertices.Count; j++)
				{
					int index = (j + 1) % polygon.vertices.Count;
					Plane.EPolygonType epolygonType3 = list[j];
					Plane.EPolygonType epolygonType4 = list[index];
					Vertex vertex = polygon.vertices[j];
					Vertex y = polygon.vertices[index];
					if (epolygonType3 != Plane.EPolygonType.Back)
					{
						list2.Add(vertex);
					}
					if (epolygonType3 != Plane.EPolygonType.Front)
					{
						list3.Add(vertex);
					}
					if ((epolygonType3 | epolygonType4) == Plane.EPolygonType.Spanning)
					{
						float weight = (this.w - Vector3.Dot(this.normal, vertex.position)) / Vector3.Dot(this.normal, y.position - vertex.position);
						Vertex item = vertex.Mix(y, weight);
						list2.Add(item);
						list3.Add(item);
					}
				}
				if (list2.Count >= 3)
				{
					if (list2.SequenceEqual(polygon.vertices))
					{
						front.Add(polygon);
					}
					else
					{
						Polygon polygon2 = new Polygon(list2, polygon.material);
						if (polygon2.plane.Valid())
						{
							front.Add(polygon2);
						}
					}
				}
				if (list3.Count >= 3)
				{
					if (list3.SequenceEqual(polygon.vertices))
					{
						back.Add(polygon);
						return;
					}
					Polygon polygon3 = new Polygon(list3, polygon.material);
					if (polygon3.plane.Valid())
					{
						back.Add(polygon3);
					}
				}
				return;
			}
			default:
				return;
			}
		}

		public Vector3 normal;

		public float w;

		[Flags]
		private enum EPolygonType
		{
			Coplanar = 0,
			Front = 1,
			Back = 2,
			Spanning = 3
		}
	}
}
