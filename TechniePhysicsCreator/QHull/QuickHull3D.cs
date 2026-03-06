using System;
using System.Collections.Generic;

namespace Technie.PhysicsCreator.QHull
{
	public class QuickHull3D
	{
		public bool getDebug()
		{
			return this.debug;
		}

		public void setDebug(bool enable)
		{
			this.debug = enable;
		}

		public double getDistanceTolerance()
		{
			return this.tolerance;
		}

		public void setExplicitDistanceTolerance(double tol)
		{
			this.explicitTolerance = tol;
		}

		public double getExplicitDistanceTolerance()
		{
			return this.explicitTolerance;
		}

		private void addPointToFace(Vertex vtx, Face face)
		{
			vtx.face = face;
			if (face.outside == null)
			{
				this.claimed.add(vtx);
			}
			else
			{
				this.claimed.insertBefore(vtx, face.outside);
			}
			face.outside = vtx;
		}

		private void removePointFromFace(Vertex vtx, Face face)
		{
			if (vtx == face.outside)
			{
				if (vtx.next != null && vtx.next.face == face)
				{
					face.outside = vtx.next;
				}
				else
				{
					face.outside = null;
				}
			}
			this.claimed.delete(vtx);
		}

		private Vertex removeAllPointsFromFace(Face face)
		{
			if (face.outside != null)
			{
				Vertex vertex = face.outside;
				while (vertex.next != null && vertex.next.face == face)
				{
					vertex = vertex.next;
				}
				this.claimed.delete(face.outside, vertex);
				vertex.next = null;
				return face.outside;
			}
			return null;
		}

		public QuickHull3D()
		{
		}

		public QuickHull3D(double[] coords)
		{
			this.build(coords, coords.Length / 3);
		}

		public QuickHull3D(Point3d[] points)
		{
			this.build(points, points.Length);
		}

		private HalfEdge findHalfEdge(Vertex tail, Vertex head)
		{
			foreach (Face face in this.faces)
			{
				HalfEdge halfEdge = face.findEdge(tail, head);
				if (halfEdge != null)
				{
					return halfEdge;
				}
			}
			return null;
		}

		protected void setHull(double[] coords, int nump, int[][] faceIndices, int numf)
		{
			this.initBuffers(nump);
			this.setPoints(coords, nump);
			this.computeMaxAndMin();
			for (int i = 0; i < numf; i++)
			{
				Face face = Face.create(this.pointBuffer, faceIndices[i]);
				HalfEdge halfEdge = face.he0;
				do
				{
					HalfEdge halfEdge2 = this.findHalfEdge(halfEdge.head(), halfEdge.tail());
					if (halfEdge2 != null)
					{
						halfEdge.setOpposite(halfEdge2);
					}
					halfEdge = halfEdge.next;
				}
				while (halfEdge != face.he0);
				this.faces.Add(face);
			}
		}

		public void build(double[] coords)
		{
			this.build(coords, coords.Length / 3);
		}

		public void build(double[] coords, int nump)
		{
			if (nump < 4)
			{
				throw new SystemException("Less than four input points specified");
			}
			if (coords.Length / 3 < nump)
			{
				throw new SystemException("Coordinate array too small for specified number of points");
			}
			this.initBuffers(nump);
			this.setPoints(coords, nump);
			this.buildHull();
		}

		public void build(Point3d[] points)
		{
			this.build(points, points.Length);
		}

		public void build(Point3d[] points, int nump)
		{
			if (nump < 4)
			{
				throw new SystemException("Less than four input points specified");
			}
			if (points.Length < nump)
			{
				throw new SystemException("Point array too small for specified number of points");
			}
			this.initBuffers(nump);
			this.setPoints(points, nump);
			this.buildHull();
		}

		public void triangulate()
		{
			double minArea = 1000.0 * this.charLength * 2.220446049250313E-16;
			this.newFaces.clear();
			foreach (Face face in this.faces)
			{
				if (face.mark == 1)
				{
					face.triangulate(this.newFaces, minArea);
				}
			}
			for (Face face2 = this.newFaces.first(); face2 != null; face2 = face2.next)
			{
				this.faces.Add(face2);
			}
		}

		protected void initBuffers(int nump)
		{
			if (this.pointBuffer.Length < nump)
			{
				Vertex[] array = new Vertex[nump];
				this.vertexPointIndices = new int[nump];
				for (int i = 0; i < this.pointBuffer.Length; i++)
				{
					array[i] = this.pointBuffer[i];
				}
				for (int j = this.pointBuffer.Length; j < nump; j++)
				{
					array[j] = new Vertex();
				}
				this.pointBuffer = array;
			}
			this.faces.Clear();
			this.claimed.clear();
			this.numFaces = 0;
			this.numPoints = nump;
		}

		protected void setPoints(double[] coords, int nump)
		{
			for (int i = 0; i < nump; i++)
			{
				Vertex vertex = this.pointBuffer[i];
				vertex.pnt.set(coords[i * 3], coords[i * 3 + 1], coords[i * 3 + 2]);
				vertex.index = i;
			}
		}

		protected void setPoints(Point3d[] pnts, int nump)
		{
			for (int i = 0; i < nump; i++)
			{
				Vertex vertex = this.pointBuffer[i];
				vertex.pnt.set(pnts[i]);
				vertex.index = i;
			}
		}

		protected void computeMaxAndMin()
		{
			Vector3d vector3d = new Vector3d();
			Vector3d vector3d2 = new Vector3d();
			for (int i = 0; i < 3; i++)
			{
				this.maxVtxs[i] = (this.minVtxs[i] = this.pointBuffer[0]);
			}
			vector3d.set(this.pointBuffer[0].pnt);
			vector3d2.set(this.pointBuffer[0].pnt);
			for (int j = 1; j < this.numPoints; j++)
			{
				Point3d pnt = this.pointBuffer[j].pnt;
				if (pnt.x > vector3d.x)
				{
					vector3d.x = pnt.x;
					this.maxVtxs[0] = this.pointBuffer[j];
				}
				else if (pnt.x < vector3d2.x)
				{
					vector3d2.x = pnt.x;
					this.minVtxs[0] = this.pointBuffer[j];
				}
				if (pnt.y > vector3d.y)
				{
					vector3d.y = pnt.y;
					this.maxVtxs[1] = this.pointBuffer[j];
				}
				else if (pnt.y < vector3d2.y)
				{
					vector3d2.y = pnt.y;
					this.minVtxs[1] = this.pointBuffer[j];
				}
				if (pnt.z > vector3d.z)
				{
					vector3d.z = pnt.z;
					this.maxVtxs[2] = this.pointBuffer[j];
				}
				else if (pnt.z < vector3d2.z)
				{
					vector3d2.z = pnt.z;
					this.minVtxs[2] = this.pointBuffer[j];
				}
			}
			this.charLength = Math.Max(vector3d.x - vector3d2.x, vector3d.y - vector3d2.y);
			this.charLength = Math.Max(vector3d.z - vector3d2.z, this.charLength);
			if (this.explicitTolerance == -1.0)
			{
				this.tolerance = 6.661338147750939E-16 * (Math.Max(Math.Abs(vector3d.x), Math.Abs(vector3d2.x)) + Math.Max(Math.Abs(vector3d.y), Math.Abs(vector3d2.y)) + Math.Max(Math.Abs(vector3d.z), Math.Abs(vector3d2.z)));
				return;
			}
			this.tolerance = this.explicitTolerance;
		}

		protected void createInitialSimplex()
		{
			double num = 0.0;
			int num2 = 0;
			for (int i = 0; i < 3; i++)
			{
				double num3 = this.maxVtxs[i].pnt.get(i) - this.minVtxs[i].pnt.get(i);
				if (num3 > num)
				{
					num = num3;
					num2 = i;
				}
			}
			if (num <= this.tolerance)
			{
				throw new SystemException("Input points appear to be coincident");
			}
			Vertex[] array = new Vertex[4];
			array[0] = this.maxVtxs[num2];
			array[1] = this.minVtxs[num2];
			Vector3d vector3d = new Vector3d();
			Vector3d vector3d2 = new Vector3d();
			Vector3d vector3d3 = new Vector3d();
			Vector3d vector3d4 = new Vector3d();
			double num4 = 0.0;
			vector3d.sub(array[1].pnt, array[0].pnt);
			vector3d.normalize();
			for (int j = 0; j < this.numPoints; j++)
			{
				vector3d2.sub(this.pointBuffer[j].pnt, array[0].pnt);
				vector3d4.cross(vector3d, vector3d2);
				double num5 = vector3d4.normSquared();
				if (num5 > num4 && this.pointBuffer[j] != array[0] && this.pointBuffer[j] != array[1])
				{
					num4 = num5;
					array[2] = this.pointBuffer[j];
					vector3d3.set(vector3d4);
				}
			}
			if (Math.Sqrt(num4) <= 100.0 * this.tolerance)
			{
				throw new SystemException("Input points appear to be colinear");
			}
			vector3d3.normalize();
			double num6 = 0.0;
			double num7 = array[2].pnt.dot(vector3d3);
			for (int k = 0; k < this.numPoints; k++)
			{
				double num8 = Math.Abs(this.pointBuffer[k].pnt.dot(vector3d3) - num7);
				if (num8 > num6 && this.pointBuffer[k] != array[0] && this.pointBuffer[k] != array[1] && this.pointBuffer[k] != array[2])
				{
					num6 = num8;
					array[3] = this.pointBuffer[k];
				}
			}
			if (Math.Abs(num6) <= 100.0 * this.tolerance)
			{
				throw new SystemException("Input points appear to be coplanar");
			}
			Face[] array2 = new Face[4];
			if (array[3].pnt.dot(vector3d3) - num7 < 0.0)
			{
				array2[0] = Face.createTriangle(array[0], array[1], array[2]);
				array2[1] = Face.createTriangle(array[3], array[1], array[0]);
				array2[2] = Face.createTriangle(array[3], array[2], array[1]);
				array2[3] = Face.createTriangle(array[3], array[0], array[2]);
				for (int l = 0; l < 3; l++)
				{
					int num9 = (l + 1) % 3;
					array2[l + 1].getEdge(1).setOpposite(array2[num9 + 1].getEdge(0));
					array2[l + 1].getEdge(2).setOpposite(array2[0].getEdge(num9));
				}
			}
			else
			{
				array2[0] = Face.createTriangle(array[0], array[2], array[1]);
				array2[1] = Face.createTriangle(array[3], array[0], array[1]);
				array2[2] = Face.createTriangle(array[3], array[1], array[2]);
				array2[3] = Face.createTriangle(array[3], array[2], array[0]);
				for (int m = 0; m < 3; m++)
				{
					int num10 = (m + 1) % 3;
					array2[m + 1].getEdge(0).setOpposite(array2[num10 + 1].getEdge(1));
					array2[m + 1].getEdge(2).setOpposite(array2[0].getEdge((3 - m) % 3));
				}
			}
			for (int n = 0; n < 4; n++)
			{
				this.faces.Add(array2[n]);
			}
			for (int num11 = 0; num11 < this.numPoints; num11++)
			{
				Vertex vertex = this.pointBuffer[num11];
				if (vertex != array[0] && vertex != array[1] && vertex != array[2] && vertex != array[3])
				{
					num6 = this.tolerance;
					Face face = null;
					for (int num12 = 0; num12 < 4; num12++)
					{
						double num13 = array2[num12].distanceToPlane(vertex.pnt);
						if (num13 > num6)
						{
							face = array2[num12];
							num6 = num13;
						}
					}
					if (face != null)
					{
						this.addPointToFace(vertex, face);
					}
				}
			}
		}

		public int getNumVertices()
		{
			return this.numVertices;
		}

		public Point3d[] getVertices()
		{
			Point3d[] array = new Point3d[this.numVertices];
			for (int i = 0; i < this.numVertices; i++)
			{
				array[i] = this.pointBuffer[this.vertexPointIndices[i]].pnt;
			}
			return array;
		}

		public int getVertices(double[] coords)
		{
			for (int i = 0; i < this.numVertices; i++)
			{
				Point3d pnt = this.pointBuffer[this.vertexPointIndices[i]].pnt;
				coords[i * 3] = pnt.x;
				coords[i * 3 + 1] = pnt.y;
				coords[i * 3 + 2] = pnt.z;
			}
			return this.numVertices;
		}

		public int[] getVertexPointIndices()
		{
			int[] array = new int[this.numVertices];
			for (int i = 0; i < this.numVertices; i++)
			{
				array[i] = this.vertexPointIndices[i];
			}
			return array;
		}

		public int getNumFaces()
		{
			return this.faces.Count;
		}

		public int[][] getFaces()
		{
			return this.getFaces(0);
		}

		public int[][] getFaces(int indexFlags)
		{
			int[][] array = new int[this.faces.Count][];
			int num = 0;
			foreach (Face face in this.faces)
			{
				array[num] = new int[face.numVertices()];
				this.getFaceIndices(array[num], face, indexFlags);
				num++;
			}
			return array;
		}

		private void getFaceIndices(int[] indices, Face face, int flags)
		{
			bool flag = (flags & 1) == 0;
			bool flag2 = (flags & 2) != 0;
			bool flag3 = (flags & 8) != 0;
			HalfEdge halfEdge = face.he0;
			int num = 0;
			do
			{
				int num2 = halfEdge.head().index;
				if (flag3)
				{
					num2 = this.vertexPointIndices[num2];
				}
				if (flag2)
				{
					num2++;
				}
				indices[num++] = num2;
				halfEdge = (flag ? halfEdge.next : halfEdge.prev);
			}
			while (halfEdge != face.he0);
		}

		protected void resolveUnclaimedPoints(FaceList newFaces)
		{
			Vertex vertex = this.unclaimed.first();
			for (Vertex vertex2 = vertex; vertex2 != null; vertex2 = vertex)
			{
				vertex = vertex2.next;
				double num = this.tolerance;
				Face face = null;
				for (Face face2 = newFaces.first(); face2 != null; face2 = face2.next)
				{
					if (face2.mark == 1)
					{
						double num2 = face2.distanceToPlane(vertex2.pnt);
						if (num2 > num)
						{
							num = num2;
							face = face2;
						}
						if (num > 1000.0 * this.tolerance)
						{
							break;
						}
					}
				}
				if (face != null)
				{
					this.addPointToFace(vertex2, face);
				}
			}
		}

		protected void deleteFacePoints(Face face, Face absorbingFace)
		{
			Vertex vertex = this.removeAllPointsFromFace(face);
			if (vertex != null)
			{
				if (absorbingFace == null)
				{
					this.unclaimed.addAll(vertex);
					return;
				}
				Vertex vertex2 = vertex;
				for (Vertex vertex3 = vertex2; vertex3 != null; vertex3 = vertex2)
				{
					vertex2 = vertex3.next;
					if (absorbingFace.distanceToPlane(vertex3.pnt) > this.tolerance)
					{
						this.addPointToFace(vertex3, absorbingFace);
					}
					else
					{
						this.unclaimed.add(vertex3);
					}
				}
			}
		}

		protected double oppFaceDistance(HalfEdge he)
		{
			return he.face.distanceToPlane(he.opposite.face.getCentroid());
		}

		private bool doAdjacentMerge(Face face, int mergeType)
		{
			HalfEdge halfEdge = face.he0;
			bool flag = true;
			for (;;)
			{
				Face face2 = halfEdge.oppositeFace();
				bool flag2 = false;
				if (mergeType == 2)
				{
					if (this.oppFaceDistance(halfEdge) > -this.tolerance || this.oppFaceDistance(halfEdge.opposite) > -this.tolerance)
					{
						flag2 = true;
					}
				}
				else if (face.area > face2.area)
				{
					if (this.oppFaceDistance(halfEdge) > -this.tolerance)
					{
						flag2 = true;
					}
					else if (this.oppFaceDistance(halfEdge.opposite) > -this.tolerance)
					{
						flag = false;
					}
				}
				else if (this.oppFaceDistance(halfEdge.opposite) > -this.tolerance)
				{
					flag2 = true;
				}
				else if (this.oppFaceDistance(halfEdge) > -this.tolerance)
				{
					flag = false;
				}
				if (flag2)
				{
					break;
				}
				halfEdge = halfEdge.next;
				if (halfEdge == face.he0)
				{
					goto Block_10;
				}
			}
			int num = face.mergeAdjacentFace(halfEdge, this.discardedFaces);
			for (int i = 0; i < num; i++)
			{
				this.deleteFacePoints(this.discardedFaces[i], face);
			}
			return true;
			Block_10:
			if (!flag)
			{
				face.mark = 2;
			}
			return false;
		}

		protected void calculateHorizon(Point3d eyePnt, HalfEdge edge0, Face face, List<HalfEdge> horizon)
		{
			this.deleteFacePoints(face, null);
			face.mark = 3;
			HalfEdge halfEdge;
			if (edge0 == null)
			{
				edge0 = face.getEdge(0);
				halfEdge = edge0;
			}
			else
			{
				halfEdge = edge0.getNext();
			}
			do
			{
				Face face2 = halfEdge.oppositeFace();
				if (face2.mark == 1)
				{
					if (face2.distanceToPlane(eyePnt) > this.tolerance)
					{
						this.calculateHorizon(eyePnt, halfEdge.getOpposite(), face2, horizon);
					}
					else
					{
						horizon.Add(halfEdge);
					}
				}
				halfEdge = halfEdge.getNext();
			}
			while (halfEdge != edge0);
		}

		private HalfEdge addAdjoiningFace(Vertex eyeVtx, HalfEdge he)
		{
			Face face = Face.createTriangle(eyeVtx, he.tail(), he.head());
			this.faces.Add(face);
			face.getEdge(-1).setOpposite(he.getOpposite());
			return face.getEdge(0);
		}

		protected void addNewFaces(FaceList newFaces, Vertex eyeVtx, List<HalfEdge> horizon)
		{
			newFaces.clear();
			HalfEdge halfEdge = null;
			HalfEdge halfEdge2 = null;
			foreach (HalfEdge he in horizon)
			{
				HalfEdge halfEdge3 = this.addAdjoiningFace(eyeVtx, he);
				if (halfEdge != null)
				{
					halfEdge3.next.setOpposite(halfEdge);
				}
				else
				{
					halfEdge2 = halfEdge3;
				}
				newFaces.add(halfEdge3.getFace());
				halfEdge = halfEdge3;
			}
			halfEdge2.next.setOpposite(halfEdge);
		}

		protected Vertex nextPointToAdd()
		{
			if (!this.claimed.isEmpty())
			{
				Face face = this.claimed.first().face;
				Vertex result = null;
				double num = 0.0;
				Vertex vertex = face.outside;
				while (vertex != null && vertex.face == face)
				{
					double num2 = face.distanceToPlane(vertex.pnt);
					if (num2 > num)
					{
						num = num2;
						result = vertex;
					}
					vertex = vertex.next;
				}
				return result;
			}
			return null;
		}

		protected void addPointToHull(Vertex eyeVtx)
		{
			this.horizon.Clear();
			this.unclaimed.clear();
			this.removePointFromFace(eyeVtx, eyeVtx.face);
			this.calculateHorizon(eyeVtx.pnt, null, eyeVtx.face, this.horizon);
			this.newFaces.clear();
			this.addNewFaces(this.newFaces, eyeVtx, this.horizon);
			for (Face face = this.newFaces.first(); face != null; face = face.next)
			{
				if (face.mark == 1)
				{
					while (this.doAdjacentMerge(face, 1))
					{
					}
				}
			}
			for (Face face2 = this.newFaces.first(); face2 != null; face2 = face2.next)
			{
				if (face2.mark == 2)
				{
					face2.mark = 1;
					while (this.doAdjacentMerge(face2, 2))
					{
					}
				}
			}
			this.resolveUnclaimedPoints(this.newFaces);
		}

		protected void buildHull()
		{
			int num = 0;
			this.computeMaxAndMin();
			this.createInitialSimplex();
			Vertex eyeVtx;
			while ((eyeVtx = this.nextPointToAdd()) != null)
			{
				this.addPointToHull(eyeVtx);
				num++;
			}
			this.reindexFacesAndVertices();
		}

		private void markFaceVertices(Face face, int mark)
		{
			HalfEdge firstEdge = face.getFirstEdge();
			HalfEdge halfEdge = firstEdge;
			do
			{
				halfEdge.head().index = mark;
				halfEdge = halfEdge.next;
			}
			while (halfEdge != firstEdge);
		}

		protected void reindexFacesAndVertices()
		{
			for (int i = 0; i < this.numPoints; i++)
			{
				this.pointBuffer[i].index = -1;
			}
			this.numFaces = 0;
			for (int j = 0; j < this.faces.Count; j++)
			{
				Face face = this.faces[j];
				if (face.mark != 1)
				{
					this.faces.RemoveAt(j);
					j--;
				}
				else
				{
					this.markFaceVertices(face, 0);
					this.numFaces++;
				}
			}
			this.numVertices = 0;
			for (int k = 0; k < this.numPoints; k++)
			{
				Vertex vertex = this.pointBuffer[k];
				if (vertex.index == 0)
				{
					this.vertexPointIndices[this.numVertices] = k;
					Vertex vertex2 = vertex;
					int num = this.numVertices;
					this.numVertices = num + 1;
					vertex2.index = num;
				}
			}
		}

		protected bool checkFaceConvexity(Face face, double tol)
		{
			HalfEdge halfEdge = face.he0;
			for (;;)
			{
				face.checkConsistency();
				if (this.oppFaceDistance(halfEdge) > tol)
				{
					break;
				}
				if (this.oppFaceDistance(halfEdge.opposite) > tol)
				{
					return false;
				}
				if (halfEdge.next.oppositeFace() == halfEdge.oppositeFace())
				{
					return false;
				}
				halfEdge = halfEdge.next;
				if (halfEdge == face.he0)
				{
					return true;
				}
			}
			return false;
		}

		protected bool checkFaces(double tol)
		{
			bool result = true;
			foreach (Face face in this.faces)
			{
				if (face.mark == 1 && !this.checkFaceConvexity(face, tol))
				{
					result = false;
				}
			}
			return result;
		}

		public bool check()
		{
			return this.check(this.getDistanceTolerance());
		}

		public bool check(double tol)
		{
			double num = 10.0 * tol;
			if (!this.checkFaces(this.tolerance))
			{
				return false;
			}
			for (int i = 0; i < this.numPoints; i++)
			{
				Point3d pnt = this.pointBuffer[i].pnt;
				foreach (Face face in this.faces)
				{
					if (face.mark == 1 && face.distanceToPlane(pnt) > num)
					{
						return false;
					}
				}
			}
			return true;
		}

		public const int CLOCKWISE = 1;

		public const int INDEXED_FROM_ONE = 2;

		public const int INDEXED_FROM_ZERO = 4;

		public const int POINT_RELATIVE = 8;

		public const double AUTOMATIC_TOLERANCE = -1.0;

		protected int findIndex = -1;

		protected double charLength;

		protected bool debug;

		protected Vertex[] pointBuffer = new Vertex[0];

		protected int[] vertexPointIndices = new int[0];

		private Face[] discardedFaces = new Face[3];

		private Vertex[] maxVtxs = new Vertex[3];

		private Vertex[] minVtxs = new Vertex[3];

		protected List<Face> faces = new List<Face>(16);

		protected List<HalfEdge> horizon = new List<HalfEdge>(16);

		private FaceList newFaces = new FaceList();

		private VertexList unclaimed = new VertexList();

		private VertexList claimed = new VertexList();

		protected int numVertices;

		protected int numFaces;

		protected int numPoints;

		protected double explicitTolerance = -1.0;

		protected double tolerance;

		private const double DOUBLE_PREC = 2.220446049250313E-16;

		private const int NONCONVEX_WRT_LARGER_FACE = 1;

		private const int NONCONVEX = 2;
	}
}
