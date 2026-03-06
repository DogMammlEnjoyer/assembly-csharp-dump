using System;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal class DTSweepDebugContext : TriangulationDebugContext
	{
		public DelaunayTriangle PrimaryTriangle
		{
			get
			{
				return this._primaryTriangle;
			}
			set
			{
				this._primaryTriangle = value;
				this._tcx.Update("set PrimaryTriangle");
			}
		}

		public DelaunayTriangle SecondaryTriangle
		{
			get
			{
				return this._secondaryTriangle;
			}
			set
			{
				this._secondaryTriangle = value;
				this._tcx.Update("set SecondaryTriangle");
			}
		}

		public TriangulationPoint ActivePoint
		{
			get
			{
				return this._activePoint;
			}
			set
			{
				this._activePoint = value;
				this._tcx.Update("set ActivePoint");
			}
		}

		public AdvancingFrontNode ActiveNode
		{
			get
			{
				return this._activeNode;
			}
			set
			{
				this._activeNode = value;
				this._tcx.Update("set ActiveNode");
			}
		}

		public DTSweepConstraint ActiveConstraint
		{
			get
			{
				return this._activeConstraint;
			}
			set
			{
				this._activeConstraint = value;
				this._tcx.Update("set ActiveConstraint");
			}
		}

		public DTSweepDebugContext(DTSweepContext tcx) : base(tcx)
		{
		}

		public bool IsDebugContext
		{
			get
			{
				return true;
			}
		}

		public override void Clear()
		{
			this.PrimaryTriangle = null;
			this.SecondaryTriangle = null;
			this.ActivePoint = null;
			this.ActiveNode = null;
			this.ActiveConstraint = null;
		}

		private DelaunayTriangle _primaryTriangle;

		private DelaunayTriangle _secondaryTriangle;

		private TriangulationPoint _activePoint;

		private AdvancingFrontNode _activeNode;

		private DTSweepConstraint _activeConstraint;
	}
}
