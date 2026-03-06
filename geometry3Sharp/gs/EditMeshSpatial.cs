using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class EditMeshSpatial : ISpatial
	{
		public void RemoveTriangle(int tid)
		{
			if (this.AddedT.Contains(tid))
			{
				this.AddedT.Remove(tid);
				return;
			}
			this.RemovedT.Add(tid);
		}

		public void AddTriangle(int tid)
		{
			this.AddedT.Add(tid);
		}

		public bool SupportsNearestTriangle
		{
			get
			{
				return false;
			}
		}

		public int FindNearestTriangle(Vector3d p, double fMaxDist = 1.7976931348623157E+308)
		{
			return -1;
		}

		public bool SupportsPointContainment
		{
			get
			{
				return false;
			}
		}

		public bool IsInside(Vector3d p)
		{
			return false;
		}

		public bool SupportsTriangleRayIntersection
		{
			get
			{
				return true;
			}
		}

		public int FindNearestHitTriangle(Ray3d ray, double fMaxDist = 1.7976931348623157E+308)
		{
			Func<int, bool> triangleFilterF = this.SourceSpatial.TriangleFilterF;
			this.SourceSpatial.TriangleFilterF = new Func<int, bool>(this.source_filter);
			int num = this.SourceSpatial.FindNearestHitTriangle(ray, double.MaxValue);
			this.SourceSpatial.TriangleFilterF = triangleFilterF;
			int num2;
			IntrRay3Triangle3 intrRay3Triangle = this.find_added_hit(ref ray, out num2);
			if (num == -1 && num2 == -1)
			{
				return -1;
			}
			if (num == -1)
			{
				return num2;
			}
			if (num2 == -1)
			{
				return num;
			}
			IntrRay3Triangle3 intrRay3Triangle2 = (num != -1) ? MeshQueries.TriangleIntersection(this.SourceMesh, num, ray) : null;
			if (intrRay3Triangle.RayParameter >= intrRay3Triangle2.RayParameter)
			{
				return num;
			}
			return num2;
		}

		private bool source_filter(int tid)
		{
			return !this.RemovedT.Contains(tid);
		}

		private IntrRay3Triangle3 find_added_hit(ref Ray3d ray, out int hit_tid)
		{
			hit_tid = -1;
			IntrRay3Triangle3 result = null;
			double num = double.MaxValue;
			Triangle3d t = default(Triangle3d);
			foreach (int num2 in this.AddedT)
			{
				Index3i triangle = this.EditMesh.GetTriangle(num2);
				t.V0 = this.EditMesh.GetVertex(triangle.a);
				t.V1 = this.EditMesh.GetVertex(triangle.b);
				t.V2 = this.EditMesh.GetVertex(triangle.c);
				IntrRay3Triangle3 intrRay3Triangle = new IntrRay3Triangle3(ray, t);
				if (intrRay3Triangle.Find() && intrRay3Triangle.RayParameter < num)
				{
					num = intrRay3Triangle.RayParameter;
					hit_tid = num2;
					result = intrRay3Triangle;
				}
			}
			return result;
		}

		public DMesh3 SourceMesh;

		public DMeshAABBTree3 SourceSpatial;

		public DMesh3 EditMesh;

		private HashSet<int> RemovedT = new HashSet<int>();

		private HashSet<int> AddedT = new HashSet<int>();
	}
}
