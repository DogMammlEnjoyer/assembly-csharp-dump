using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder.Poly2Tri
{
	internal abstract class TriangulationContext
	{
		public TriangulationDebugContext DebugContext { get; protected set; }

		public TriangulationMode TriangulationMode { get; protected set; }

		public Triangulatable Triangulatable { get; private set; }

		public int StepCount { get; private set; }

		public void Done()
		{
			int stepCount = this.StepCount;
			this.StepCount = stepCount + 1;
		}

		public abstract TriangulationAlgorithm Algorithm { get; }

		public virtual void PrepareTriangulation(Triangulatable t)
		{
			this.Triangulatable = t;
			this.TriangulationMode = t.TriangulationMode;
			t.Prepare(this);
		}

		public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

		public void Update(string message)
		{
		}

		public virtual void Clear()
		{
			this.Points.Clear();
			if (this.DebugContext != null)
			{
				this.DebugContext.Clear();
			}
			this.StepCount = 0;
		}

		public virtual bool IsDebugEnabled { get; protected set; }

		public DTSweepDebugContext DTDebugContext
		{
			get
			{
				return this.DebugContext as DTSweepDebugContext;
			}
		}

		public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();

		public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>();
	}
}
