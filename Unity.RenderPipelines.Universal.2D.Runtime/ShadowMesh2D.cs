using System;
using Unity.Collections;

namespace UnityEngine.Rendering.Universal
{
	[Serializable]
	internal class ShadowMesh2D : ShadowShape2D
	{
		public Mesh mesh
		{
			get
			{
				return this.m_Mesh;
			}
		}

		public BoundingSphere boundingSphere
		{
			get
			{
				return this.m_BoundingSphere;
			}
		}

		public ShadowMesh2D.EdgeProcessing edgeProcessing
		{
			get
			{
				return this.m_EdgeProcessing;
			}
			set
			{
				this.m_EdgeProcessing = value;
			}
		}

		public float trimEdge
		{
			get
			{
				return this.m_TrimEdge;
			}
			set
			{
				this.m_TrimEdge = value;
			}
		}

		internal static void DuplicateShadowMesh(Mesh source, out Mesh dest)
		{
			dest = new Mesh();
			dest.Clear();
			if (source != null)
			{
				dest.vertices = source.vertices;
				dest.tangents = source.tangents;
				dest.triangles = source.triangles;
				dest.bounds = source.bounds;
			}
		}

		internal void CopyFrom(ShadowMesh2D source)
		{
			ShadowMesh2D.DuplicateShadowMesh(source.m_Mesh, out this.m_Mesh);
			this.m_TrimEdge = source.trimEdge;
			this.m_LocalBounds = source.m_LocalBounds;
			this.m_EdgeProcessing = source.edgeProcessing;
		}

		internal void AddCircle(Vector3 center, float r, NativeArray<Vector3> generatedVertices, NativeArray<int> generatedIndices, bool reverseWindingOrder, ref int vertexWritePos, ref int indexWritePos)
		{
			float num = (float)(reverseWindingOrder ? 1 : -1);
			float num2 = 16f;
			int num3 = vertexWritePos;
			int num4 = 0;
			while ((float)num4 < num2)
			{
				float f = num * (6.2831855f * (float)num4 / num2);
				float x = r * Mathf.Cos(f) + center.x;
				float y = r * Mathf.Sin(f) + center.y;
				int num5 = indexWritePos;
				indexWritePos = num5 + 1;
				generatedIndices[num5] = vertexWritePos;
				num5 = indexWritePos;
				indexWritePos = num5 + 1;
				generatedIndices[num5] = (((float)(num4 + 1) < num2) ? (vertexWritePos + 1) : num3);
				num5 = vertexWritePos;
				vertexWritePos = num5 + 1;
				generatedVertices[num5] = new Vector3(x, y, 0f);
				num4++;
			}
		}

		internal void AddCapsuleCap(Vector3 center, float r, Vector3 otherCenter, NativeArray<Vector3> generatedVertices, NativeArray<int> generatedIndices, bool reverseWindingOrder, ref int vertexWritePos, ref int indexWritePos)
		{
			float num = 8f;
			Vector3 normalized = (otherCenter - center).normalized;
			float num2 = Mathf.Acos(Vector3.Dot(normalized, new Vector3(1f, 0f, 0f)));
			float num3 = (Vector3.Dot(normalized, new Vector3(0f, 1f, 0f)) < 0f) ? -1f : 1f;
			float num4 = num2 * num3;
			float num6;
			float num7;
			if (reverseWindingOrder)
			{
				float num5 = 1.5707964f;
				num6 = num4 + num5;
				num7 = num6 + 3.1415927f;
			}
			else
			{
				float num8 = 4.712389f;
				num6 = num4 + num8;
				num7 = num6 - 3.1415927f;
			}
			float num9 = num7 - num6;
			int num10 = 0;
			float f;
			int num11;
			while ((float)num10 < num)
			{
				f = num9 * (float)num10 / num + num6;
				float x = r * Mathf.Cos(f) + center.x;
				float y = r * Mathf.Sin(f) + center.y;
				num11 = indexWritePos;
				indexWritePos = num11 + 1;
				generatedIndices[num11] = vertexWritePos;
				num11 = indexWritePos;
				indexWritePos = num11 + 1;
				generatedIndices[num11] = vertexWritePos + 1;
				num11 = vertexWritePos;
				vertexWritePos = num11 + 1;
				generatedVertices[num11] = new Vector3(x, y, 0f);
				num10++;
			}
			f = num9 + num6;
			num11 = vertexWritePos;
			vertexWritePos = num11 + 1;
			generatedVertices[num11] = new Vector3(r * Mathf.Cos(f) + center.x, r * Mathf.Sin(f) + center.y, 0f);
		}

		internal void AddCapsule(Vector3 pt0, Vector3 pt1, float r0, float r1, NativeArray<Vector3> generatedVertices, NativeArray<int> generatedIndices, bool reverseWindingOrder, ref int vertexWritePos, ref int indexWritePos)
		{
			Vector3 normalized = (pt1 - pt0).normalized;
			new Vector3(normalized.y, -normalized.x, 0f);
			new Vector3(-normalized.y, normalized.x, 0f);
			if (pt1.x < pt0.x)
			{
				Vector3 vector = pt0;
				pt0 = pt1;
				pt1 = vector;
			}
			int value = vertexWritePos;
			this.AddCapsuleCap(pt0, r0, pt1, generatedVertices, generatedIndices, reverseWindingOrder, ref vertexWritePos, ref indexWritePos);
			int num = indexWritePos;
			indexWritePos = num + 1;
			generatedIndices[num] = vertexWritePos - 1;
			num = indexWritePos;
			indexWritePos = num + 1;
			generatedIndices[num] = vertexWritePos;
			this.AddCapsuleCap(pt1, r1, pt0, generatedVertices, generatedIndices, reverseWindingOrder, ref vertexWritePos, ref indexWritePos);
			num = indexWritePos;
			indexWritePos = num + 1;
			generatedIndices[num] = vertexWritePos - 1;
			num = indexWritePos;
			indexWritePos = num + 1;
			generatedIndices[num] = value;
		}

		internal int AddShape(NativeArray<Vector3> vertices, NativeArray<int> indices, int indicesProcessed, NativeArray<Vector3> generatedVertices, NativeArray<int> generatedIndices, ref int vertexWritePos, ref int indexWritePos)
		{
			int num = indicesProcessed;
			int num2 = indices[num];
			int num3 = indices[num];
			int value = vertexWritePos;
			int num4 = vertexWritePos;
			vertexWritePos = num4 + 1;
			generatedVertices[num4] = vertices[num2];
			bool flag = true;
			while (num < indices.Length && flag)
			{
				int num5 = indices[num++];
				int num6 = indices[num++];
				num4 = indexWritePos;
				indexWritePos = num4 + 1;
				generatedIndices[num4] = vertexWritePos - 1;
				if (num6 != num3)
				{
					num4 = indexWritePos;
					indexWritePos = num4 + 1;
					generatedIndices[num4] = vertexWritePos;
					num4 = vertexWritePos;
					vertexWritePos = num4 + 1;
					generatedVertices[num4] = vertices[num6];
					flag = (num5 == num2);
				}
				else
				{
					num4 = indexWritePos;
					indexWritePos = num4 + 1;
					generatedIndices[num4] = value;
					flag = false;
				}
				num2 = num6;
			}
			return num;
		}

		public override void SetShape(NativeArray<Vector3> vertices, NativeArray<int> indices, NativeArray<float> radii, Matrix4x4 transform, ShadowShape2D.WindingOrder windingOrder = ShadowShape2D.WindingOrder.Clockwise, bool allowTriming = true, bool createInteriorGeometry = false)
		{
			if (this.m_TrimEdge == -1f)
			{
				this.m_TrimEdge = this.m_InitialTrim;
			}
			if (this.m_Mesh == null)
			{
				this.m_Mesh = new Mesh();
			}
			if (indices.Length == 0)
			{
				this.m_Mesh.Clear();
				return;
			}
			bool flag = windingOrder == ShadowShape2D.WindingOrder.CounterClockwise;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < indices.Length; i += 2)
			{
				int num3 = indices[i];
				int num4 = indices[i + 1];
				if (radii[num3] > 0f || radii[num4] > 0f)
				{
					if (num3 == num4)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
			}
			int num5 = num2 * 2;
			int num6 = num2 * 8;
			int num7 = num * 2 * 8;
			int num8 = (indices.Length >> 1) - (num2 + num);
			int num9 = 2 * (num8 + num5 + 2 * num6 + num7);
			int length = num9;
			NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int> generatedIndices = new NativeArray<int>(num9, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num10 = 0;
			int num11 = 0;
			int j = 0;
			while (j < indices.Length)
			{
				int index = indices[j];
				int index2 = indices[j + 1];
				float num12 = radii[index];
				float r = radii[index2];
				if (radii[index] > 0f || radii[index2] > 0f)
				{
					Vector3 vector = vertices[index];
					Vector3 pt = vertices[index2];
					if (vertices[index].x == vertices[index2].x && vertices[index].y == vertices[index2].y)
					{
						this.AddCircle(vector, num12, nativeArray, generatedIndices, flag, ref num10, ref num11);
					}
					else
					{
						this.AddCapsule(vector, pt, num12, r, nativeArray, generatedIndices, flag, ref num10, ref num11);
					}
					j += 2;
				}
				else
				{
					j = this.AddShape(vertices, indices, j, nativeArray, generatedIndices, ref num10, ref num11);
				}
			}
			for (int k = 0; k < nativeArray.Length; k++)
			{
				nativeArray[k] = transform.MultiplyPoint(nativeArray[k]);
			}
			NativeArray<ShadowEdge> inEdges;
			NativeArray<int> inShapeStartingEdge;
			NativeArray<bool> inShapeIsClosedArray;
			ShadowUtility.CalculateEdgesFromLines(ref generatedIndices, out inEdges, out inShapeStartingEdge, out inShapeIsClosedArray);
			if (flag)
			{
				ShadowUtility.ReverseWindingOrder(ref inShapeStartingEdge, ref inEdges);
			}
			if (this.m_EdgeProcessing == ShadowMesh2D.EdgeProcessing.Clipping)
			{
				NativeArray<Vector3> inVertices;
				NativeArray<ShadowEdge> inEdges2;
				NativeArray<int> inShapeStartingEdge2;
				ShadowUtility.ClipEdges(ref nativeArray, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, this.trimEdge, out inVertices, out inEdges2, out inShapeStartingEdge2);
				if (inShapeStartingEdge2.Length > 0)
				{
					this.m_LocalBounds = ShadowUtility.GenerateShadowMesh(this.m_Mesh, inVertices, inEdges2, inShapeStartingEdge2, inShapeIsClosedArray, true, createInteriorGeometry, ShadowShape2D.OutlineTopology.Lines);
				}
				else
				{
					this.m_LocalBounds = default(Bounds);
					this.m_Mesh.Clear();
				}
				inVertices.Dispose();
				inEdges2.Dispose();
				inShapeStartingEdge2.Dispose();
			}
			else
			{
				this.m_LocalBounds = ShadowUtility.GenerateShadowMesh(this.m_Mesh, nativeArray, inEdges, inShapeStartingEdge, inShapeIsClosedArray, true, createInteriorGeometry, ShadowShape2D.OutlineTopology.Lines);
			}
			nativeArray.Dispose();
			generatedIndices.Dispose();
			inEdges.Dispose();
			inShapeIsClosedArray.Dispose();
			inShapeStartingEdge.Dispose();
		}

		private bool AreDegenerateVertices(NativeArray<Vector3> vertices)
		{
			if (vertices.Length == 0)
			{
				return true;
			}
			int index = vertices.Length - 1;
			for (int i = 0; i < vertices.Length; i++)
			{
				if (vertices[index].x != vertices[i].x || vertices[index].y != vertices[i].y)
				{
					return false;
				}
				index = i;
			}
			return true;
		}

		public override void SetShape(NativeArray<Vector3> vertices, NativeArray<int> indices, ShadowShape2D.OutlineTopology outlineTopology, ShadowShape2D.WindingOrder windingOrder = ShadowShape2D.WindingOrder.Clockwise, bool allowTrimming = true, bool createInteriorGeometry = false)
		{
			if (this.AreDegenerateVertices(vertices) || indices.Length == 0)
			{
				this.m_Mesh.Clear();
				return;
			}
			if (this.m_TrimEdge == -1f)
			{
				this.m_TrimEdge = this.m_InitialTrim;
			}
			bool flag = false;
			if (this.m_Mesh == null)
			{
				this.m_Mesh = new Mesh();
			}
			NativeArray<ShadowEdge> inEdges;
			NativeArray<int> inShapeStartingEdge;
			NativeArray<bool> inShapeIsClosedArray;
			if (outlineTopology == ShadowShape2D.OutlineTopology.Triangles)
			{
				NativeArray<Vector3> nativeArray;
				ShadowUtility.CalculateEdgesFromTriangles(ref vertices, ref indices, true, out nativeArray, out inEdges, out inShapeStartingEdge, out inShapeIsClosedArray);
				flag = true;
				vertices = nativeArray;
			}
			else
			{
				ShadowUtility.CalculateEdgesFromLines(ref indices, out inEdges, out inShapeStartingEdge, out inShapeIsClosedArray);
			}
			if (windingOrder == ShadowShape2D.WindingOrder.CounterClockwise)
			{
				ShadowUtility.ReverseWindingOrder(ref inShapeStartingEdge, ref inEdges);
			}
			if (this.m_EdgeProcessing == ShadowMesh2D.EdgeProcessing.Clipping && allowTrimming)
			{
				NativeArray<Vector3> inVertices;
				NativeArray<ShadowEdge> inEdges2;
				NativeArray<int> inShapeStartingEdge2;
				ShadowUtility.ClipEdges(ref vertices, ref inEdges, ref inShapeStartingEdge, ref inShapeIsClosedArray, this.trimEdge, out inVertices, out inEdges2, out inShapeStartingEdge2);
				this.m_LocalBounds = ShadowUtility.GenerateShadowMesh(this.m_Mesh, inVertices, inEdges2, inShapeStartingEdge2, inShapeIsClosedArray, allowTrimming, createInteriorGeometry, outlineTopology);
				inVertices.Dispose();
				inEdges2.Dispose();
				inShapeStartingEdge2.Dispose();
			}
			else
			{
				this.m_LocalBounds = ShadowUtility.GenerateShadowMesh(this.m_Mesh, vertices, inEdges, inShapeStartingEdge, inShapeIsClosedArray, allowTrimming, createInteriorGeometry, outlineTopology);
			}
			if (flag)
			{
				vertices.Dispose();
			}
			inEdges.Dispose();
			inShapeStartingEdge.Dispose();
			inShapeIsClosedArray.Dispose();
		}

		public void SetShapeWithLines(NativeArray<Vector3> vertices, NativeArray<int> indices, bool allowTrimming)
		{
			this.SetShape(vertices, indices, ShadowShape2D.OutlineTopology.Lines, ShadowShape2D.WindingOrder.Clockwise, allowTrimming, false);
		}

		public override void SetFlip(bool flipX, bool flipY)
		{
			this.m_FlipX = flipX;
			this.m_FlipY = flipY;
		}

		public override void GetFlip(out bool flipX, out bool flipY)
		{
			flipX = this.m_FlipX;
			flipY = this.m_FlipY;
		}

		public override void SetDefaultTrim(float trim)
		{
			this.m_InitialTrim = trim;
		}

		public void UpdateBoundingSphere(Transform transform)
		{
			Vector3 a = transform.TransformPoint(this.m_LocalBounds.max);
			Vector3 b = transform.TransformPoint(this.m_LocalBounds.min);
			Vector3 vector = 0.5f * (a + b);
			float rad = Vector3.Magnitude(a - vector);
			this.m_BoundingSphere = new BoundingSphere(vector, rad);
		}

		internal const int k_CapsuleCapSegments = 8;

		internal const float k_TrimEdgeUninitialized = -1f;

		[SerializeField]
		private Mesh m_Mesh;

		[SerializeField]
		private Bounds m_LocalBounds;

		[SerializeField]
		private ShadowMesh2D.EdgeProcessing m_EdgeProcessing = ShadowMesh2D.EdgeProcessing.Clipping;

		[SerializeField]
		private float m_TrimEdge = -1f;

		[SerializeField]
		private bool m_FlipX;

		[SerializeField]
		private bool m_FlipY;

		[SerializeField]
		private float m_InitialTrim;

		internal BoundingSphere m_BoundingSphere;

		public enum EdgeProcessing
		{
			None,
			Clipping
		}
	}
}
