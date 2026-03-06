using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class PolyPathD : PolyPathBase
	{
		internal double Scale { get; set; }

		public List<PointD> Polygon { get; private set; }

		public PolyPathD(PolyPathBase parent = null) : base(parent)
		{
		}

		[NullableContext(1)]
		internal override PolyPathBase AddChild(List<Point64> p)
		{
			PolyPathBase polyPathBase = new PolyPathD(this);
			(polyPathBase as PolyPathD).Scale = this.Scale;
			(polyPathBase as PolyPathD).Polygon = Clipper.ScalePathD(p, 1.0 / this.Scale);
			this._childs.Add(polyPathBase);
			return polyPathBase;
		}

		[Nullable(1)]
		[IndexerName("Child")]
		public PolyPathD this[int index]
		{
			[NullableContext(1)]
			get
			{
				if (index < 0 || index >= this._childs.Count)
				{
					throw new InvalidOperationException();
				}
				return (PolyPathD)this._childs[index];
			}
		}

		public double Area()
		{
			double num = (this.Polygon == null) ? 0.0 : Clipper.Area(this.Polygon);
			foreach (PolyPathBase polyPathBase in this._childs)
			{
				PolyPath64 polyPath = (PolyPath64)polyPathBase;
				num += polyPath.Area();
			}
			return num;
		}
	}
}
