using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class PolygonSet
	{
		public PolygonSet()
		{
		}

		public PolygonSet(Polygon poly)
		{
			this._polygons.Add(poly);
		}

		public void Add(Polygon p)
		{
			this._polygons.Add(p);
		}

		public IEnumerable<Polygon> Polygons
		{
			get
			{
				return this._polygons;
			}
		}

		protected List<Polygon> _polygons = new List<Polygon>();
	}
}
