using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class PolyPath64 : PolyPathBase
	{
		public List<Point64> Polygon { get; private set; }

		public PolyPath64(PolyPathBase parent = null) : base(parent)
		{
		}

		[NullableContext(1)]
		internal override PolyPathBase AddChild(List<Point64> p)
		{
			PolyPathBase polyPathBase = new PolyPath64(this);
			(polyPathBase as PolyPath64).Polygon = p;
			this._childs.Add(polyPathBase);
			return polyPathBase;
		}

		[Nullable(1)]
		[IndexerName("Child")]
		public PolyPath64 this[int index]
		{
			[NullableContext(1)]
			get
			{
				if (index < 0 || index >= this._childs.Count)
				{
					throw new InvalidOperationException();
				}
				return (PolyPath64)this._childs[index];
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
