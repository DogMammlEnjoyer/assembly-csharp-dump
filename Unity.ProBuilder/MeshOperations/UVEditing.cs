using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.MeshOperations
{
	internal static class UVEditing
	{
		public static bool AutoStitch(ProBuilderMesh mesh, Face f1, Face f2, int channel)
		{
			WingedEdge wingedEdge = WingedEdge.GetWingedEdges(mesh, new Face[]
			{
				f1,
				f2
			}, false).FirstOrDefault((WingedEdge x) => x.face == f1 && x.opposite != null && x.opposite.face == f2);
			if (wingedEdge == null)
			{
				return false;
			}
			if (f1.manualUV)
			{
				f2.manualUV = true;
			}
			f1.textureGroup = -1;
			f2.textureGroup = -1;
			Projection.PlanarProject(mesh, f2, default(Vector3));
			if (UVEditing.AlignEdges(mesh, f2, wingedEdge.edge.local, wingedEdge.opposite.edge.local, channel))
			{
				if (!f2.manualUV)
				{
					UvUnwrapping.SetAutoAndAlignUnwrapParamsToUVs(mesh, new Face[]
					{
						f2
					});
				}
				return true;
			}
			return false;
		}

		private static bool AlignEdges(ProBuilderMesh mesh, Face faceToMove, Edge edgeToAlignTo, Edge edgeToBeAligned, int channel)
		{
			Vector2[] uvs = UVEditing.GetUVs(mesh, channel);
			SharedVertex[] sharedVerticesInternal = mesh.sharedVerticesInternal;
			int[] array = new int[]
			{
				edgeToAlignTo.a,
				-1
			};
			int[] array2 = new int[]
			{
				edgeToAlignTo.b,
				-1
			};
			int sharedVertexHandle = mesh.GetSharedVertexHandle(edgeToAlignTo.a);
			if (sharedVertexHandle < 0)
			{
				return false;
			}
			if (sharedVerticesInternal[sharedVertexHandle].Contains(edgeToBeAligned.a))
			{
				array[1] = edgeToBeAligned.a;
				array2[1] = edgeToBeAligned.b;
			}
			else
			{
				array[1] = edgeToBeAligned.b;
				array2[1] = edgeToBeAligned.a;
			}
			float num = Vector2.Distance(uvs[edgeToAlignTo.a], uvs[edgeToAlignTo.b]);
			float num2 = Vector2.Distance(uvs[edgeToBeAligned.a], uvs[edgeToBeAligned.b]);
			float d = num / num2;
			foreach (int num3 in faceToMove.distinctIndexesInternal)
			{
				uvs[num3] = uvs[num3].ScaleAroundPoint(Vector2.zero, Vector2.one * d);
			}
			Vector2 vector = (uvs[edgeToAlignTo.a] + uvs[edgeToAlignTo.b]) / 2f;
			Vector2 b = (uvs[edgeToBeAligned.a] + uvs[edgeToBeAligned.b]) / 2f;
			Vector2 b2 = vector - b;
			foreach (int num4 in faceToMove.distinctIndexesInternal)
			{
				uvs[num4] += b2;
			}
			Vector2 vector2 = uvs[array2[0]] - uvs[array[0]];
			Vector2 vector3 = uvs[array2[1]] - uvs[array[1]];
			float num5 = Vector2.Angle(vector2, vector3);
			if (Vector3.Cross(vector2, vector3).z < 0f)
			{
				num5 = 360f - num5;
			}
			foreach (int num6 in faceToMove.distinctIndexesInternal)
			{
				uvs[num6] = uvs[num6].RotateAroundPoint(vector, num5);
			}
			float num7 = Mathf.Abs(Vector2.Distance(uvs[array[0]], uvs[array[1]])) + Mathf.Abs(Vector2.Distance(uvs[array2[0]], uvs[array2[1]]));
			if (num7 > 0.02f)
			{
				foreach (int num8 in faceToMove.distinctIndexesInternal)
				{
					uvs[num8] = uvs[num8].RotateAroundPoint(vector, 180f);
				}
				float num9 = Mathf.Abs(Vector2.Distance(uvs[array[0]], uvs[array[1]])) + Mathf.Abs(Vector2.Distance(uvs[array2[0]], uvs[array2[1]]));
				if (num9 >= num7)
				{
					foreach (int num10 in faceToMove.distinctIndexesInternal)
					{
						uvs[num10] = uvs[num10].RotateAroundPoint(vector, 180f);
					}
				}
			}
			mesh.SplitUVs(faceToMove.distinctIndexesInternal);
			mesh.SetTexturesCoincident(array);
			mesh.SetTexturesCoincident(array2);
			UVEditing.ApplyUVs(mesh, uvs, channel, true);
			return true;
		}

		internal static Vector2[] GetUVs(ProBuilderMesh mesh, int channel)
		{
			if (channel != 1)
			{
				if (channel - 2 > 1)
				{
					return mesh.texturesInternal;
				}
				if ((channel == 2) ? mesh.HasArrays(MeshArrays.Texture2) : mesh.HasArrays(MeshArrays.Texture3))
				{
					List<Vector4> list = new List<Vector4>();
					mesh.GetUVs(channel, list);
					return (from x in list
					select x).ToArray<Vector2>();
				}
				return null;
			}
			else
			{
				if (mesh.mesh == null)
				{
					return null;
				}
				return mesh.mesh.uv2;
			}
		}

		internal static void ApplyUVs(ProBuilderMesh mesh, Vector2[] uvs, int channel, bool applyToMesh = true)
		{
			switch (channel)
			{
			case 0:
				mesh.texturesInternal = uvs;
				if (applyToMesh && mesh.mesh != null)
				{
					mesh.mesh.uv = uvs;
					return;
				}
				break;
			case 1:
				if (applyToMesh && mesh.mesh != null)
				{
					mesh.mesh.uv2 = uvs;
					return;
				}
				break;
			case 2:
			case 3:
			{
				int vertexCount = mesh.vertexCount;
				if (vertexCount != uvs.Length)
				{
					throw new IndexOutOfRangeException("uvs");
				}
				List<Vector4> list = new List<Vector4>(vertexCount);
				for (int i = 0; i < vertexCount; i++)
				{
					list.Add(uvs[i]);
				}
				mesh.SetUVs(channel, list);
				if (applyToMesh && mesh.mesh != null)
				{
					mesh.mesh.SetUVs(channel, list);
				}
				break;
			}
			default:
				return;
			}
		}

		public static void SewUVs(this ProBuilderMesh mesh, int[] indexes, float delta)
		{
			Vector2[] array = mesh.texturesInternal;
			if (array == null || array.Length != mesh.vertexCount)
			{
				array = new Vector2[mesh.vertexCount];
			}
			Dictionary<int, int> sharedTextureLookup = mesh.sharedTextureLookup;
			for (int i = 0; i < indexes.Length - 1; i++)
			{
				for (int j = i + 1; j < indexes.Length; j++)
				{
					int count;
					if (!sharedTextureLookup.TryGetValue(indexes[i], out count))
					{
						sharedTextureLookup.Add(indexes[i], count = sharedTextureLookup.Count);
					}
					int b;
					if (!sharedTextureLookup.TryGetValue(indexes[j], out b))
					{
						sharedTextureLookup.Add(indexes[j], b = sharedTextureLookup.Count);
					}
					if (count != b && Vector2.Distance(array[indexes[i]], array[indexes[j]]) < delta)
					{
						Vector3 v = (array[indexes[i]] + array[indexes[j]]) / 2f;
						array[indexes[i]] = v;
						array[indexes[j]] = v;
						foreach (int key in (from x in sharedTextureLookup
						where x.Value == b
						select x into y
						select y.Key).ToArray<int>())
						{
							sharedTextureLookup[key] = count;
						}
					}
				}
			}
			mesh.SetSharedTextures(sharedTextureLookup);
		}

		public static void CollapseUVs(this ProBuilderMesh mesh, int[] indexes)
		{
			Vector2[] texturesInternal = mesh.texturesInternal;
			Vector2 vector = Math.Average(texturesInternal.ValuesWithIndexes(indexes), null);
			foreach (int num in indexes)
			{
				texturesInternal[num] = vector;
			}
			mesh.SetTexturesCoincident(indexes);
		}

		public static void SplitUVs(this ProBuilderMesh mesh, IEnumerable<int> indexes)
		{
			Dictionary<int, int> sharedTextureLookup = mesh.sharedTextureLookup;
			int count = sharedTextureLookup.Count;
			foreach (int key in indexes)
			{
				int num;
				if (sharedTextureLookup.TryGetValue(key, out num))
				{
					sharedTextureLookup[key] = count++;
				}
			}
			mesh.SetSharedTextures(sharedTextureLookup);
		}

		internal static void SplitUVs(ProBuilderMesh mesh, IEnumerable<Face> faces)
		{
			Dictionary<int, int> sharedTextureLookup = mesh.sharedTextureLookup;
			int count = sharedTextureLookup.Count;
			foreach (Face face in faces)
			{
				foreach (int key in face.distinctIndexesInternal)
				{
					int num;
					if (sharedTextureLookup.TryGetValue(key, out num))
					{
						sharedTextureLookup[key] = count++;
					}
				}
			}
			mesh.SetSharedTextures(sharedTextureLookup);
		}

		internal static void ProjectFacesAuto(ProBuilderMesh mesh, Face[] faces, int channel)
		{
			if (faces.Length < 1)
			{
				return;
			}
			int[] array = faces.SelectMany((Face x) => x.distinctIndexesInternal).ToArray<int>();
			Vector3 vector = Vector3.zero;
			foreach (Face face in faces)
			{
				Vector3 b = Math.Normal(mesh, face);
				vector += b;
			}
			vector /= (float)faces.Length;
			Vector2[] array2 = Projection.PlanarProject(mesh.positionsInternal, array, vector);
			Vector2[] uvs = UVEditing.GetUVs(mesh, channel);
			for (int j = 0; j < array.Length; j++)
			{
				uvs[array[j]] = array2[j];
			}
			UVEditing.ApplyUVs(mesh, uvs, channel, true);
			foreach (Face face2 in faces)
			{
				face2.elementGroup = -1;
				mesh.SplitUVs(face2.distinctIndexesInternal);
			}
			mesh.SewUVs(faces.SelectMany((Face x) => x.distinctIndexesInternal).ToArray<int>(), 0.001f);
		}

		public static void ProjectFacesBox(ProBuilderMesh mesh, Face[] faces, int channel = 0)
		{
			Vector2[] uvs = UVEditing.GetUVs(mesh, channel);
			Dictionary<ProjectionAxis, List<Face>> dictionary = new Dictionary<ProjectionAxis, List<Face>>();
			for (int i = 0; i < faces.Length; i++)
			{
				ProjectionAxis key = Projection.VectorToProjectionAxis(Math.Normal(mesh, faces[i]));
				if (dictionary.ContainsKey(key))
				{
					dictionary[key].Add(faces[i]);
				}
				else
				{
					dictionary.Add(key, new List<Face>
					{
						faces[i]
					});
				}
				faces[i].elementGroup = -1;
				faces[i].manualUV = true;
			}
			foreach (KeyValuePair<ProjectionAxis, List<Face>> keyValuePair in dictionary)
			{
				int[] array = keyValuePair.Value.SelectMany((Face x) => x.distinctIndexesInternal).ToArray<int>();
				Vector2[] array2 = Projection.PlanarProject(mesh.positionsInternal, array, Projection.ProjectionAxisToVector(keyValuePair.Key));
				for (int j = 0; j < array.Length; j++)
				{
					uvs[array[j]] = array2[j];
				}
				mesh.SplitUVs(array);
			}
			UVEditing.ApplyUVs(mesh, uvs, channel, true);
		}

		internal static Vector2 FindMinimalUV(Vector2[] uvs, int[] indices = null, float xMin = 0f, float yMin = 0f)
		{
			int num = (indices == null) ? uvs.Length : indices.Length;
			bool flag = xMin == 0f && yMin == 0f;
			for (int i = 0; i < num; i++)
			{
				int num2 = (indices == null) ? i : indices[i];
				if (flag)
				{
					xMin = uvs[num2].x;
					yMin = uvs[num2].y;
					flag = false;
				}
				else
				{
					if (uvs[num2].x < xMin)
					{
						xMin = uvs[num2].x;
					}
					if (uvs[num2].y < yMin)
					{
						yMin = uvs[num2].y;
					}
				}
			}
			return new Vector2(xMin, yMin);
		}

		public static void ProjectFacesBox(ProBuilderMesh mesh, Face[] faces, Vector2 lowerLeftAnchor, int channel = 0)
		{
			Vector2[] uvs = UVEditing.GetUVs(mesh, channel);
			Dictionary<ProjectionAxis, List<Face>> dictionary = new Dictionary<ProjectionAxis, List<Face>>();
			for (int i = 0; i < faces.Length; i++)
			{
				ProjectionAxis key = Projection.VectorToProjectionAxis(Math.Normal(mesh, faces[i]));
				if (dictionary.ContainsKey(key))
				{
					dictionary[key].Add(faces[i]);
				}
				else
				{
					dictionary.Add(key, new List<Face>
					{
						faces[i]
					});
				}
				faces[i].elementGroup = -1;
				faces[i].manualUV = true;
			}
			foreach (KeyValuePair<ProjectionAxis, List<Face>> keyValuePair in dictionary)
			{
				int[] array = keyValuePair.Value.SelectMany((Face x) => x.distinctIndexesInternal).ToArray<int>();
				Vector2[] array2 = Projection.PlanarProject(mesh.positionsInternal, array, Projection.ProjectionAxisToVector(keyValuePair.Key));
				Vector2 b = UVEditing.FindMinimalUV(array2, null, 0f, 0f);
				for (int j = 0; j < array.Length; j++)
				{
					uvs[array[j]] = array2[j] - b;
				}
				mesh.SplitUVs(array);
			}
			UVEditing.ApplyUVs(mesh, uvs, channel, true);
		}

		public static void ProjectFacesSphere(ProBuilderMesh pb, int[] indexes, int channel = 0)
		{
			foreach (Face face in pb.facesInternal)
			{
				if (face.distinctIndexesInternal.ContainsMatch(indexes))
				{
					face.elementGroup = -1;
					face.manualUV = true;
				}
			}
			pb.SplitUVs(indexes);
			Vector2[] array = Projection.SphericalProject(pb.positionsInternal, indexes);
			Vector2[] uvs = UVEditing.GetUVs(pb, channel);
			for (int j = 0; j < indexes.Length; j++)
			{
				uvs[indexes[j]] = array[j];
			}
			UVEditing.ApplyUVs(pb, uvs, channel, true);
		}

		public static Vector2[] FitUVs(Vector2[] uvs)
		{
			Vector2 b = Math.SmallestVector2(uvs);
			for (int i = 0; i < uvs.Length; i++)
			{
				uvs[i] -= b;
			}
			float d = Math.MakeNonZero(Math.LargestValue(Math.LargestVector2(uvs)), 0.0001f);
			for (int i = 0; i < uvs.Length; i++)
			{
				uvs[i] /= d;
			}
			return uvs;
		}
	}
}
