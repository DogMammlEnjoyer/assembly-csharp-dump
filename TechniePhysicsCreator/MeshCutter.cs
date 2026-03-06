using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class MeshCutter
	{
		public void Cut(CuttableMesh input, Plane worldCutPlane)
		{
			this.inputMesh = input;
			this.outputFrontSubMeshes = new List<CuttableSubMesh>();
			this.outputBackSubMeshes = new List<CuttableSubMesh>();
			Transform transform = this.inputMesh.GetTransform();
			Plane cutPlane;
			if (transform != null)
			{
				Vector3 inPoint = transform.InverseTransformPoint(MeshCutter.ClosestPointOnPlane(worldCutPlane, Vector3.zero));
				Vector3 inNormal = transform.InverseTransformDirection(worldCutPlane.normal);
				cutPlane = new Plane(inNormal, inPoint);
			}
			else
			{
				cutPlane = worldCutPlane;
			}
			foreach (CuttableSubMesh inputSubMesh in input.GetSubMeshes())
			{
				this.Cut(inputSubMesh, cutPlane);
			}
		}

		private static Vector3 ClosestPointOnPlane(Plane plane, Vector3 point)
		{
			return plane.ClosestPointOnPlane(point);
		}

		public CuttableMesh GetFrontOutput()
		{
			return new CuttableMesh(this.inputMesh, this.outputFrontSubMeshes);
		}

		public CuttableMesh GetBackOutput()
		{
			return new CuttableMesh(this.inputMesh, this.outputBackSubMeshes);
		}

		private void Cut(CuttableSubMesh inputSubMesh, Plane cutPlane)
		{
			bool hasNormals = inputSubMesh.HasNormals();
			bool hasColours = inputSubMesh.HasColours();
			bool hasUvs = inputSubMesh.HasUvs();
			bool hasUv = inputSubMesh.HasUv1();
			CuttableSubMesh cuttableSubMesh = new CuttableSubMesh(hasNormals, hasColours, hasUvs, hasUv);
			CuttableSubMesh cuttableSubMesh2 = new CuttableSubMesh(hasNormals, hasColours, hasUvs, hasUv);
			for (int i = 0; i < inputSubMesh.NumVertices(); i += 3)
			{
				int num = i;
				int num2 = i + 1;
				int num3 = i + 2;
				Vector3 vertex = inputSubMesh.GetVertex(num);
				Vector3 vertex2 = inputSubMesh.GetVertex(num2);
				Vector3 vertex3 = inputSubMesh.GetVertex(num3);
				VertexClassification vertexClassification = this.Classify(vertex, cutPlane);
				VertexClassification vertexClassification2 = this.Classify(vertex2, cutPlane);
				VertexClassification vertexClassification3 = this.Classify(vertex3, cutPlane);
				int num4 = 0;
				int num5 = 0;
				this.CountSides(vertexClassification, ref num4, ref num5);
				this.CountSides(vertexClassification2, ref num4, ref num5);
				this.CountSides(vertexClassification3, ref num4, ref num5);
				if (num4 > 0 && num5 == 0)
				{
					this.KeepTriangle(num, num2, num3, inputSubMesh, cuttableSubMesh);
				}
				else if (num4 == 0 && num5 > 0)
				{
					this.KeepTriangle(num, num2, num3, inputSubMesh, cuttableSubMesh2);
				}
				else if (num4 == 2 && num5 == 1)
				{
					if (vertexClassification == VertexClassification.Back)
					{
						this.SplitA(num, num2, num3, inputSubMesh, cutPlane, cuttableSubMesh2, cuttableSubMesh);
					}
					else if (vertexClassification2 == VertexClassification.Back)
					{
						this.SplitA(num2, num3, num, inputSubMesh, cutPlane, cuttableSubMesh2, cuttableSubMesh);
					}
					else
					{
						this.SplitA(num3, num, num2, inputSubMesh, cutPlane, cuttableSubMesh2, cuttableSubMesh);
					}
				}
				else if (num4 == 1 && num5 == 2)
				{
					if (vertexClassification == VertexClassification.Front)
					{
						this.SplitA(num, num2, num3, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
					}
					else if (vertexClassification2 == VertexClassification.Front)
					{
						this.SplitA(num2, num3, num, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
					}
					else
					{
						this.SplitA(num3, num, num2, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
					}
				}
				else if (num4 == 1 && num5 == 1)
				{
					if (vertexClassification == VertexClassification.OnPlane)
					{
						if (vertexClassification3 == VertexClassification.Front)
						{
							this.SplitB(num3, num, num2, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
						}
						else
						{
							this.SplitBFlipped(num2, num3, num, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
						}
					}
					else if (vertexClassification2 == VertexClassification.OnPlane)
					{
						if (vertexClassification == VertexClassification.Front)
						{
							this.SplitB(num, num2, num3, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
						}
						else
						{
							this.SplitBFlipped(num3, num, num2, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
						}
					}
					else if (vertexClassification2 == VertexClassification.Front)
					{
						this.SplitB(num2, num3, num, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
					}
					else
					{
						this.SplitBFlipped(num, num2, num3, inputSubMesh, cutPlane, cuttableSubMesh, cuttableSubMesh2);
					}
				}
				else if (num4 == 0 && num5 == 0)
				{
					Vector3 lhs = vertex2 - vertex;
					Vector3 rhs = vertex3 - vertex;
					if (Vector3.Dot(Vector3.Cross(lhs, rhs), cutPlane.normal) > 0f)
					{
						this.KeepTriangle(num, num2, num3, inputSubMesh, cuttableSubMesh2);
					}
					else
					{
						this.KeepTriangle(num, num2, num3, inputSubMesh, cuttableSubMesh);
					}
				}
			}
			this.outputFrontSubMeshes.Add(cuttableSubMesh);
			this.outputBackSubMeshes.Add(cuttableSubMesh2);
		}

		private VertexClassification Classify(Vector3 vertex, Plane cutPlane)
		{
			Vector3 point = new Vector3(vertex.x, vertex.y, vertex.z);
			float distanceToPoint = cutPlane.GetDistanceToPoint(point);
			double num = 9.999999747378752E-06;
			if ((double)distanceToPoint > -num && (double)distanceToPoint < num)
			{
				return VertexClassification.OnPlane;
			}
			if (distanceToPoint > 0f)
			{
				return VertexClassification.Front;
			}
			return VertexClassification.Back;
		}

		private void CountSides(VertexClassification c, ref int numFront, ref int numBehind)
		{
			if (c == VertexClassification.Front)
			{
				numFront++;
				return;
			}
			if (c == VertexClassification.Back)
			{
				numBehind++;
			}
		}

		private void KeepTriangle(int i0, int i1, int i2, CuttableSubMesh inputSubMesh, CuttableSubMesh destSubMesh)
		{
			destSubMesh.CopyVertex(i0, inputSubMesh);
			destSubMesh.CopyVertex(i1, inputSubMesh);
			destSubMesh.CopyVertex(i2, inputSubMesh);
		}

		private void SplitA(int i0, int i1, int i2, CuttableSubMesh inputSubMesh, Plane cutPlane, CuttableSubMesh frontSubMesh, CuttableSubMesh backSubMesh)
		{
			Vector3 vertex = inputSubMesh.GetVertex(i0);
			Vector3 vertex2 = inputSubMesh.GetVertex(i1);
			Vector3 vertex3 = inputSubMesh.GetVertex(i2);
			float weight;
			this.CalcIntersection(vertex, vertex2, cutPlane, out weight);
			float weight2;
			this.CalcIntersection(vertex3, vertex, cutPlane, out weight2);
			frontSubMesh.CopyVertex(i0, inputSubMesh);
			frontSubMesh.AddInterpolatedVertex(i0, i1, weight, inputSubMesh);
			frontSubMesh.AddInterpolatedVertex(i2, i0, weight2, inputSubMesh);
			backSubMesh.AddInterpolatedVertex(i0, i1, weight, inputSubMesh);
			backSubMesh.CopyVertex(i1, inputSubMesh);
			backSubMesh.CopyVertex(i2, inputSubMesh);
			backSubMesh.CopyVertex(i2, inputSubMesh);
			backSubMesh.AddInterpolatedVertex(i2, i0, weight2, inputSubMesh);
			backSubMesh.AddInterpolatedVertex(i0, i1, weight, inputSubMesh);
		}

		private void SplitB(int i0, int i1, int i2, CuttableSubMesh inputSubMesh, Plane cutPlane, CuttableSubMesh frontSubMesh, CuttableSubMesh backSubMesh)
		{
			Vector3 vertex = inputSubMesh.GetVertex(i0);
			Vector3 vertex2 = inputSubMesh.GetVertex(i2);
			float weight;
			this.CalcIntersection(vertex2, vertex, cutPlane, out weight);
			frontSubMesh.CopyVertex(i0, inputSubMesh);
			frontSubMesh.CopyVertex(i1, inputSubMesh);
			frontSubMesh.AddInterpolatedVertex(i2, i0, weight, inputSubMesh);
			backSubMesh.CopyVertex(i1, inputSubMesh);
			backSubMesh.CopyVertex(i2, inputSubMesh);
			backSubMesh.AddInterpolatedVertex(i2, i0, weight, inputSubMesh);
		}

		private void SplitBFlipped(int i0, int i1, int i2, CuttableSubMesh inputSubMesh, Plane cutPlane, CuttableSubMesh frontSubMesh, CuttableSubMesh backSubMesh)
		{
			Vector3 vertex = inputSubMesh.GetVertex(i0);
			Vector3 vertex2 = inputSubMesh.GetVertex(i1);
			float weight;
			this.CalcIntersection(vertex, vertex2, cutPlane, out weight);
			frontSubMesh.CopyVertex(i0, inputSubMesh);
			frontSubMesh.AddInterpolatedVertex(i0, i1, weight, inputSubMesh);
			frontSubMesh.CopyVertex(i2, inputSubMesh);
			backSubMesh.CopyVertex(i1, inputSubMesh);
			backSubMesh.CopyVertex(i2, inputSubMesh);
			backSubMesh.AddInterpolatedVertex(i0, i1, weight, inputSubMesh);
		}

		private Vector3 CalcIntersection(Vector3 v0, Vector3 v1, Plane plane, out float weight)
		{
			Vector3 a = v1 - v0;
			float magnitude = a.magnitude;
			Ray ray = new Ray(v0, a / magnitude);
			float num;
			plane.Raycast(ray, out num);
			Vector3 result = ray.origin + ray.direction * num;
			weight = num / magnitude;
			return result;
		}

		private CuttableMesh inputMesh;

		private List<CuttableSubMesh> outputFrontSubMeshes;

		private List<CuttableSubMesh> outputBackSubMeshes;
	}
}
