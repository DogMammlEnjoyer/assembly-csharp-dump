using System;
using System.Collections.Generic;

namespace g3
{
	public class ConvexHull2
	{
		public int Dimension
		{
			get
			{
				return this.mDimension;
			}
		}

		public int NumSimplices
		{
			get
			{
				return this.mNumSimplices;
			}
		}

		public int[] HullIndices
		{
			get
			{
				return this.mIndices;
			}
		}

		public ConvexHull2(IList<Vector2d> vertices, double epsilon, QueryNumberType queryType)
		{
			this.mVertices = vertices;
			this.mNumVertices = vertices.Count;
			this.mDimension = 0;
			this.mNumSimplices = 0;
			this.mIndices = null;
			this.mSVertices = null;
			this.mEpsilon = epsilon;
			this.mQuery = null;
			this.mLineOrigin = Vector2d.Zero;
			this.mLineDirection = Vector2d.Zero;
			Vector2d.Information information;
			Vector2d.GetInformation(this.mVertices, this.mEpsilon, out information);
			if (information.mDimension == 0)
			{
				this.mDimension = 0;
				this.mIndices = null;
				return;
			}
			if (information.mDimension == 1)
			{
				this.mDimension = 1;
				this.mLineOrigin = information.mOrigin;
				this.mLineDirection = information.mDirection0;
				return;
			}
			this.mDimension = 2;
			int num = information.mExtreme[0];
			int num2 = information.mExtreme[1];
			int num3 = information.mExtreme[2];
			this.mSVertices = new Vector2d[this.mNumVertices];
			if (queryType != QueryNumberType.QT_RATIONAL && queryType != QueryNumberType.QT_FILTERED)
			{
				Vector2d o = new Vector2d(information.mMin[0], information.mMin[1]);
				double f = 1.0 / information.mMaxRange;
				for (int i = 0; i < this.mNumVertices; i++)
				{
					this.mSVertices[i] = (this.mVertices[i] - o) * f;
				}
				double f2;
				if (queryType == QueryNumberType.QT_INT64)
				{
					f2 = 1048576.0;
					this.mQuery = new Query2Int64(this.mSVertices);
				}
				else
				{
					if (queryType == QueryNumberType.QT_INTEGER)
					{
						throw new NotImplementedException("ConvexHull2: Query type QT_INTEGER not currently supported");
					}
					f2 = 1.0;
					this.mQuery = new Query2d(this.mSVertices);
				}
				for (int j = 0; j < this.mNumVertices; j++)
				{
					this.mSVertices[j] *= f2;
				}
				ConvexHull2.Edge edge;
				ConvexHull2.Edge edge2;
				ConvexHull2.Edge edge3;
				if (information.mExtremeCCW)
				{
					edge = new ConvexHull2.Edge(num, num2);
					edge2 = new ConvexHull2.Edge(num2, num3);
					edge3 = new ConvexHull2.Edge(num3, num);
				}
				else
				{
					edge = new ConvexHull2.Edge(num, num3);
					edge2 = new ConvexHull2.Edge(num3, num2);
					edge3 = new ConvexHull2.Edge(num2, num);
				}
				edge.Insert(edge3, edge2);
				edge2.Insert(edge, edge3);
				edge3.Insert(edge2, edge);
				ConvexHull2.Edge edge4 = edge;
				int num4 = 0;
				while (this.Update(ref edge4, num4))
				{
					num4 = (num4 + 31337) % this.mNumVertices;
					if (num4 == 0)
					{
						edge4.GetIndices(ref this.mNumSimplices, ref this.mIndices);
						return;
					}
				}
				return;
			}
			throw new NotImplementedException("ConvexHull2: Query type QT_RATIONAL/QT_FILTERED not currently supported");
		}

		public void Get1DHullInfo(out Vector2d origin, out Vector2d direction)
		{
			origin = this.mLineOrigin;
			direction = this.mLineDirection;
		}

		public Polygon2d GetHullPolygon()
		{
			if (this.mIndices == null)
			{
				return null;
			}
			Polygon2d polygon2d = new Polygon2d();
			for (int i = 0; i < this.mIndices.Length; i++)
			{
				polygon2d.AppendVertex(this.mVertices[this.mIndices[i]]);
			}
			return polygon2d;
		}

		private bool Update(ref ConvexHull2.Edge hull, int i)
		{
			ConvexHull2.Edge edge = null;
			ConvexHull2.Edge edge2 = hull;
			while (edge2.GetSign(i, this.mQuery) <= 0)
			{
				edge2 = edge2.E1;
				if (edge2 == hull)
				{
					IL_25:
					if (edge == null)
					{
						return true;
					}
					ConvexHull2.Edge e = edge.E0;
					if (e == null)
					{
						return false;
					}
					ConvexHull2.Edge e2 = edge.E1;
					if (e2 == null)
					{
						return false;
					}
					edge.DeleteSelf();
					while (e.GetSign(i, this.mQuery) > 0)
					{
						hull = e;
						e = e.E0;
						if (e == null)
						{
							return false;
						}
						e.E1.DeleteSelf();
					}
					while (e2.GetSign(i, this.mQuery) > 0)
					{
						hull = e2;
						e2 = e2.E1;
						if (e2 == null)
						{
							return false;
						}
						e2.E0.DeleteSelf();
					}
					ConvexHull2.Edge edge3 = new ConvexHull2.Edge(e.V[1], i);
					ConvexHull2.Edge edge4 = new ConvexHull2.Edge(i, e2.V[0]);
					edge3.Insert(e, edge4);
					edge4.Insert(edge3, e2);
					hull = edge3;
					return true;
				}
			}
			edge = edge2;
			goto IL_25;
		}

		private IList<Vector2d> mVertices;

		private int mNumVertices;

		private int mDimension;

		private int mNumSimplices;

		private double mEpsilon;

		private Vector2d[] mSVertices;

		private int[] mIndices;

		private Query2 mQuery;

		private Vector2d mLineOrigin;

		private Vector2d mLineDirection;

		protected class Edge
		{
			public Edge(int v0, int v1)
			{
				this.Sign = 0;
				this.Time = -1;
				this.V[0] = v0;
				this.V[1] = v1;
				this.E0 = null;
				this.E1 = null;
			}

			public int GetSign(int i, Query2 query)
			{
				if (i != this.Time)
				{
					this.Time = i;
					this.Sign = query.ToLine(i, this.V[0], this.V[1]);
				}
				return this.Sign;
			}

			public void Insert(ConvexHull2.Edge adj0, ConvexHull2.Edge adj1)
			{
				adj0.E1 = this;
				adj1.E0 = this;
				this.E0 = adj0;
				this.E1 = adj1;
			}

			public void DeleteSelf()
			{
				if (this.E0 != null)
				{
					this.E0.E1 = null;
				}
				if (this.E1 != null)
				{
					this.E1.E0 = null;
				}
			}

			public void GetIndices(ref int numIndices, ref int[] indices)
			{
				numIndices = 0;
				ConvexHull2.Edge edge = this;
				do
				{
					numIndices++;
					edge = edge.E1;
				}
				while (edge != this);
				indices = new int[numIndices];
				numIndices = 0;
				edge = this;
				do
				{
					indices[numIndices] = edge.V[0];
					numIndices++;
					edge = edge.E1;
				}
				while (edge != this);
			}

			public Vector2i V;

			public ConvexHull2.Edge E0;

			public ConvexHull2.Edge E1;

			public int Sign;

			public int Time;
		}
	}
}
