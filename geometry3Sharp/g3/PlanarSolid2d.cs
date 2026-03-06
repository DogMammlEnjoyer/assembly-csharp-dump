using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace g3
{
	public class PlanarSolid2d
	{
		public IParametricCurve2d Outer
		{
			get
			{
				return this.outer;
			}
		}

		public void SetOuter(IParametricCurve2d loop, bool bIsClockwise)
		{
			this.outer = loop;
		}

		public void AddHole(IParametricCurve2d hole)
		{
			if (this.outer == null)
			{
				throw new Exception("PlanarSolid2d.AddHole: outer polygon not set!");
			}
			this.holes.Add(hole);
		}

		private bool HasHoles
		{
			get
			{
				return this.Holes.Count > 0;
			}
		}

		public ReadOnlyCollection<IParametricCurve2d> Holes
		{
			get
			{
				return this.holes.AsReadOnly();
			}
		}

		public bool HasArcLength
		{
			get
			{
				bool flag = this.outer.HasArcLength;
				foreach (IParametricCurve2d parametricCurve2d in this.Holes)
				{
					flag = (flag && parametricCurve2d.HasArcLength);
				}
				return flag;
			}
		}

		public double Perimeter
		{
			get
			{
				if (!this.HasArcLength)
				{
					throw new Exception("PlanarSolid2d.Perimeter: some curves do not have arc length");
				}
				double num = this.outer.ArcLength;
				foreach (IParametricCurve2d parametricCurve2d in this.Holes)
				{
					num += parametricCurve2d.ArcLength;
				}
				return num;
			}
		}

		public GeneralPolygon2d Convert(double fSpacingLength, double fSpacingT, double fDeviationTolerance)
		{
			GeneralPolygon2d generalPolygon2d = new GeneralPolygon2d();
			generalPolygon2d.Outer = new Polygon2d(CurveSampler2.AutoSample(this.outer, fSpacingLength, fSpacingT));
			generalPolygon2d.Outer.Simplify(0.0, fDeviationTolerance, true);
			foreach (IParametricCurve2d curve in this.Holes)
			{
				Polygon2d polygon2d = new Polygon2d(CurveSampler2.AutoSample(curve, fSpacingLength, fSpacingT));
				polygon2d.Simplify(0.0, fDeviationTolerance, true);
				generalPolygon2d.AddHole(polygon2d, false, true);
			}
			return generalPolygon2d;
		}

		private IParametricCurve2d outer;

		private List<IParametricCurve2d> holes = new List<IParametricCurve2d>();
	}
}
