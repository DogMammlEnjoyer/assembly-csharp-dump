using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Csg
{
	internal sealed class Polygon
	{
		public Polygon(List<Vertex> list, Material mat)
		{
			this.vertices = list;
			this.plane = new Plane(list[0].position, list[1].position, list[2].position);
			this.material = mat;
		}

		public void Flip()
		{
			this.vertices.Reverse();
			for (int i = 0; i < this.vertices.Count; i++)
			{
				this.vertices[i].Flip();
			}
			this.plane.Flip();
		}

		public override string ToString()
		{
			return string.Format("[{0}] {1}", this.vertices.Count, this.plane.normal);
		}

		public List<Vertex> vertices;

		public Plane plane;

		public Material material;
	}
}
