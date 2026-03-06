using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class ClipperD : ClipperBase
	{
		public ClipperD(int roundingDecimalPrecision = 2)
		{
			if (roundingDecimalPrecision < -8 || roundingDecimalPrecision > 8)
			{
				throw new ClipperLibException("Error - RoundingDecimalPrecision exceeds the allowed range.");
			}
			this._scale = Math.Pow(10.0, (double)roundingDecimalPrecision);
			this._invScale = 1.0 / this._scale;
		}

		public void AddPath(List<PointD> path, PathType polytype, bool isOpen = false)
		{
			base.AddPath(Clipper.ScalePath64(path, this._scale), polytype, isOpen);
		}

		public void AddPaths(List<List<PointD>> paths, PathType polytype, bool isOpen = false)
		{
			base.AddPaths(Clipper.ScalePaths64(paths, this._scale), polytype, isOpen);
		}

		public void AddSubject(List<PointD> path)
		{
			this.AddPath(path, PathType.Subject, false);
		}

		public void AddOpenSubject(List<PointD> path)
		{
			this.AddPath(path, PathType.Subject, true);
		}

		public void AddClip(List<PointD> path)
		{
			this.AddPath(path, PathType.Clip, false);
		}

		public void AddSubject(List<List<PointD>> paths)
		{
			this.AddPaths(paths, PathType.Subject, false);
		}

		public void AddOpenSubject(List<List<PointD>> paths)
		{
			this.AddPaths(paths, PathType.Subject, true);
		}

		public void AddClip(List<List<PointD>> paths)
		{
			this.AddPaths(paths, PathType.Clip, false);
		}

		public bool Execute(ClipType clipType, FillRule fillRule, List<List<PointD>> solutionClosed, List<List<PointD>> solutionOpen)
		{
			List<List<Point64>> list = new List<List<Point64>>();
			List<List<Point64>> list2 = new List<List<Point64>>();
			bool flag = true;
			solutionClosed.Clear();
			solutionOpen.Clear();
			try
			{
				base.ExecuteInternal(clipType, fillRule);
				base.BuildPaths(list, list2);
			}
			catch
			{
				flag = false;
			}
			base.ClearSolution();
			if (!flag)
			{
				return false;
			}
			solutionClosed.Capacity = list.Count;
			foreach (List<Point64> path in list)
			{
				solutionClosed.Add(Clipper.ScalePathD(path, this._invScale));
			}
			solutionOpen.Capacity = list2.Count;
			foreach (List<Point64> path2 in list2)
			{
				solutionOpen.Add(Clipper.ScalePathD(path2, this._invScale));
			}
			return true;
		}

		public bool Execute(ClipType clipType, FillRule fillRule, List<List<PointD>> solutionClosed)
		{
			return this.Execute(clipType, fillRule, solutionClosed, new List<List<PointD>>());
		}

		public bool Execute(ClipType clipType, FillRule fillRule, PolyTreeD polytree, List<List<PointD>> openPaths)
		{
			polytree.Clear();
			polytree.Scale = this._scale;
			openPaths.Clear();
			List<List<Point64>> list = new List<List<Point64>>();
			bool flag = true;
			try
			{
				base.ExecuteInternal(clipType, fillRule);
				base.BuildTree(polytree, list);
			}
			catch
			{
				flag = false;
			}
			base.ClearSolution();
			if (!flag)
			{
				return false;
			}
			if (list.Count > 0)
			{
				openPaths.Capacity = list.Count;
				foreach (List<Point64> path in list)
				{
					openPaths.Add(Clipper.ScalePathD(path, this._invScale));
				}
			}
			return true;
		}

		public bool Execute(ClipType clipType, FillRule fillRule, PolyTreeD polytree)
		{
			return this.Execute(clipType, fillRule, polytree, new List<List<PointD>>());
		}

		private readonly double _scale;

		private readonly double _invScale;
	}
}
