using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	public static class VertexPositioning
	{
		public static Vector3[] VerticesInWorldSpace(this ProBuilderMesh mesh)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			int vertexCount = mesh.vertexCount;
			Vector3[] array = new Vector3[vertexCount];
			Vector3[] positionsInternal = mesh.positionsInternal;
			for (int i = 0; i < vertexCount; i++)
			{
				array[i] = mesh.transform.TransformPoint(positionsInternal[i]);
			}
			return array;
		}

		public static void TranslateVerticesInWorldSpace(this ProBuilderMesh mesh, int[] indexes, Vector3 offset)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			mesh.TranslateVerticesInWorldSpace(indexes, offset, 0f, false);
		}

		internal static void TranslateVerticesInWorldSpace(this ProBuilderMesh mesh, int[] indexes, Vector3 offset, float snapValue, bool snapAxisOnly)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			mesh.GetCoincidentVertices(indexes, VertexPositioning.s_CoincidentVertices);
			Matrix4x4 worldToLocalMatrix = mesh.transform.worldToLocalMatrix;
			Vector3 b = worldToLocalMatrix * offset;
			Vector3[] positionsInternal = mesh.positionsInternal;
			if (Mathf.Abs(snapValue) > Mathf.Epsilon)
			{
				Matrix4x4 localToWorldMatrix = mesh.transform.localToWorldMatrix;
				Vector3Mask mask = snapAxisOnly ? new Vector3Mask(offset, 0.0001f) : Vector3Mask.XYZ;
				for (int i = 0; i < VertexPositioning.s_CoincidentVertices.Count; i++)
				{
					Vector3 val = localToWorldMatrix.MultiplyPoint3x4(positionsInternal[VertexPositioning.s_CoincidentVertices[i]] + b);
					positionsInternal[VertexPositioning.s_CoincidentVertices[i]] = worldToLocalMatrix.MultiplyPoint3x4(ProBuilderSnapping.Snap(val, mask * snapValue));
				}
			}
			else
			{
				for (int i = 0; i < VertexPositioning.s_CoincidentVertices.Count; i++)
				{
					positionsInternal[VertexPositioning.s_CoincidentVertices[i]] += b;
				}
			}
			mesh.positionsInternal = positionsInternal;
			mesh.mesh.vertices = positionsInternal;
		}

		public static void TranslateVertices(this ProBuilderMesh mesh, IEnumerable<int> indexes, Vector3 offset)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			mesh.GetCoincidentVertices(indexes, VertexPositioning.s_CoincidentVertices);
			VertexPositioning.TranslateVerticesInternal(mesh, VertexPositioning.s_CoincidentVertices, offset);
		}

		public static void TranslateVertices(this ProBuilderMesh mesh, IEnumerable<Edge> edges, Vector3 offset)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			mesh.GetCoincidentVertices(edges, VertexPositioning.s_CoincidentVertices);
			VertexPositioning.TranslateVerticesInternal(mesh, VertexPositioning.s_CoincidentVertices, offset);
		}

		public static void TranslateVertices(this ProBuilderMesh mesh, IEnumerable<Face> faces, Vector3 offset)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			mesh.GetCoincidentVertices(faces, VertexPositioning.s_CoincidentVertices);
			VertexPositioning.TranslateVerticesInternal(mesh, VertexPositioning.s_CoincidentVertices, offset);
		}

		private static void TranslateVerticesInternal(ProBuilderMesh mesh, IEnumerable<int> indices, Vector3 offset)
		{
			Vector3[] positionsInternal = mesh.positionsInternal;
			int i = 0;
			int count = VertexPositioning.s_CoincidentVertices.Count;
			while (i < count)
			{
				positionsInternal[VertexPositioning.s_CoincidentVertices[i]] += offset;
				i++;
			}
			mesh.mesh.vertices = positionsInternal;
		}

		public static void SetSharedVertexPosition(this ProBuilderMesh mesh, int sharedVertexHandle, Vector3 position)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			Vector3[] positionsInternal = mesh.positionsInternal;
			foreach (int num in mesh.sharedVerticesInternal[sharedVertexHandle])
			{
				positionsInternal[num] = position;
			}
			mesh.positionsInternal = positionsInternal;
			mesh.mesh.vertices = positionsInternal;
		}

		internal static void SetSharedVertexValues(this ProBuilderMesh mesh, int sharedVertexHandle, Vertex vertex)
		{
			Vertex[] vertices = mesh.GetVertices(null);
			foreach (int num in mesh.sharedVerticesInternal[sharedVertexHandle])
			{
				vertices[num] = vertex;
			}
			mesh.SetVertices(vertices, false);
		}

		private static List<int> s_CoincidentVertices = new List<int>();
	}
}
