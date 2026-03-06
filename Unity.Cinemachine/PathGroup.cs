using System;
using System.Collections.Generic;

namespace Unity.Cinemachine
{
	internal class PathGroup
	{
		public PathGroup(List<List<Point64>> paths, JoinType joinType, EndType endType = EndType.Polygon)
		{
			this._inPaths = new List<List<Point64>>(paths);
			this._joinType = joinType;
			this._endType = endType;
			this._outPath = new List<Point64>();
			this._outPaths = new List<List<Point64>>();
			this._pathsReversed = false;
		}

		internal List<List<Point64>> _inPaths;

		internal List<Point64> _outPath;

		internal List<List<Point64>> _outPaths;

		internal JoinType _joinType;

		internal EndType _endType;

		internal bool _pathsReversed;
	}
}
