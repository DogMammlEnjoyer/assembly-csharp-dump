using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Unity.Cinemachine
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class Clipper64 : ClipperBase
	{
		internal new void AddPath(List<Point64> path, PathType polytype, bool isOpen = false)
		{
			base.AddPath(path, polytype, isOpen);
		}

		internal new void AddPaths(List<List<Point64>> paths, PathType polytype, bool isOpen = false)
		{
			base.AddPaths(paths, polytype, isOpen);
		}

		public void AddSubject(List<List<Point64>> paths)
		{
			this.AddPaths(paths, PathType.Subject, false);
		}

		public void AddOpenSubject(List<List<Point64>> paths)
		{
			this.AddPaths(paths, PathType.Subject, true);
		}

		public void AddClip(List<List<Point64>> paths)
		{
			this.AddPaths(paths, PathType.Clip, false);
		}

		public bool Execute(ClipType clipType, FillRule fillRule, List<List<Point64>> solutionClosed, List<List<Point64>> solutionOpen)
		{
			solutionClosed.Clear();
			solutionOpen.Clear();
			try
			{
				base.ExecuteInternal(clipType, fillRule);
				base.BuildPaths(solutionClosed, solutionOpen);
			}
			catch
			{
				this._succeeded = false;
			}
			base.ClearSolution();
			return this._succeeded;
		}

		public bool Execute(ClipType clipType, FillRule fillRule, List<List<Point64>> solutionClosed)
		{
			return this.Execute(clipType, fillRule, solutionClosed, new List<List<Point64>>());
		}

		public bool Execute(ClipType clipType, FillRule fillRule, PolyTree64 polytree, List<List<Point64>> openPaths)
		{
			polytree.Clear();
			openPaths.Clear();
			this._using_polytree = true;
			try
			{
				base.ExecuteInternal(clipType, fillRule);
				base.BuildTree(polytree, openPaths);
			}
			catch
			{
				this._succeeded = false;
			}
			base.ClearSolution();
			return this._succeeded;
		}

		public bool Execute(ClipType clipType, FillRule fillRule, PolyTree64 polytree)
		{
			return this.Execute(clipType, fillRule, polytree, new List<List<Point64>>());
		}
	}
}
