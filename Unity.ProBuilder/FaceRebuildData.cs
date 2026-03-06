using System;
using System.Collections.Generic;

namespace UnityEngine.ProBuilder
{
	internal sealed class FaceRebuildData
	{
		public int Offset()
		{
			return this._appliedOffset;
		}

		public override string ToString()
		{
			return string.Format("{0}\n{1}", this.vertices.ToString(", "), this.sharedIndexes.ToString(", "));
		}

		public static void Apply(IEnumerable<FaceRebuildData> newFaces, ProBuilderMesh mesh, List<Vertex> vertices = null, List<Face> faces = null)
		{
			if (faces == null)
			{
				faces = new List<Face>(mesh.facesInternal);
			}
			if (vertices == null)
			{
				vertices = new List<Vertex>(mesh.GetVertices(null));
			}
			Dictionary<int, int> sharedVertexLookup = mesh.sharedVertexLookup;
			Dictionary<int, int> sharedTextureLookup = mesh.sharedTextureLookup;
			FaceRebuildData.Apply(newFaces, vertices, faces, sharedVertexLookup, sharedTextureLookup);
			mesh.SetVertices(vertices, false);
			mesh.faces = faces;
			mesh.SetSharedVertices(sharedVertexLookup);
			mesh.SetSharedTextures(sharedTextureLookup);
		}

		public static void Apply(IEnumerable<FaceRebuildData> newFaces, List<Vertex> vertices, List<Face> faces, Dictionary<int, int> sharedVertexLookup, Dictionary<int, int> sharedTextureLookup = null)
		{
			int num = vertices.Count;
			foreach (FaceRebuildData faceRebuildData in newFaces)
			{
				Face face = faceRebuildData.face;
				int count = faceRebuildData.vertices.Count;
				bool flag = sharedVertexLookup != null && faceRebuildData.sharedIndexes != null && faceRebuildData.sharedIndexes.Count == count;
				bool flag2 = sharedTextureLookup != null && faceRebuildData.sharedIndexesUV != null && faceRebuildData.sharedIndexesUV.Count == count;
				for (int i = 0; i < count; i++)
				{
					int num2 = i;
					if (sharedVertexLookup != null)
					{
						sharedVertexLookup.Add(num2 + num, flag ? faceRebuildData.sharedIndexes[num2] : -1);
					}
					if (sharedTextureLookup != null && flag2)
					{
						sharedTextureLookup.Add(num2 + num, faceRebuildData.sharedIndexesUV[num2]);
					}
				}
				faceRebuildData._appliedOffset = num;
				int[] indexesInternal = face.indexesInternal;
				int j = 0;
				int num3 = indexesInternal.Length;
				while (j < num3)
				{
					indexesInternal[j] += num;
					j++;
				}
				num += faceRebuildData.vertices.Count;
				face.indexesInternal = indexesInternal;
				faces.Add(face);
				vertices.AddRange(faceRebuildData.vertices);
			}
		}

		public Face face;

		public List<Vertex> vertices;

		public List<int> sharedIndexes;

		public List<int> sharedIndexesUV;

		private int _appliedOffset;
	}
}
