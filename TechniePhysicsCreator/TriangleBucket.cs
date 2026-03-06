using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class TriangleBucket
	{
		public float Area
		{
			get
			{
				return this.totalArea;
			}
		}

		public TriangleBucket(Triangle initialTriangle)
		{
			this.triangles = new List<Triangle>();
			this.triangles.Add(initialTriangle);
			this.CalculateNormal();
			this.CalcTotalArea();
		}

		public void Add(Triangle t)
		{
			this.triangles.Add(t);
			this.CalculateNormal();
			this.CalcTotalArea();
		}

		public void Add(TriangleBucket otherBucket)
		{
			foreach (Triangle item in otherBucket.triangles)
			{
				this.triangles.Add(item);
			}
			this.CalculateNormal();
			this.CalcTotalArea();
		}

		private void CalculateNormal()
		{
			this.averagedNormal = Vector3.zero;
			foreach (Triangle triangle in this.triangles)
			{
				this.averagedNormal += triangle.normal * triangle.area;
			}
			this.averagedNormal.Normalize();
		}

		public Vector3 GetAverageNormal()
		{
			return this.averagedNormal;
		}

		public Vector3 GetAverageCenter()
		{
			return this.triangles[0].center;
		}

		private void CalcTotalArea()
		{
			this.totalArea = 0f;
			foreach (Triangle triangle in this.triangles)
			{
				this.totalArea += triangle.area;
			}
		}

		private List<Triangle> triangles;

		private Vector3 averagedNormal;

		private Vector3 averagedCenter;

		private float totalArea;
	}
}
