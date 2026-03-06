using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder
{
	public static class HandleUtility
	{
		internal static Vector3 ScreenToGuiPoint(this Camera camera, Vector3 point, float pixelsPerPoint)
		{
			return new Vector3(point.x / pixelsPerPoint, ((float)camera.pixelHeight - point.y) / pixelsPerPoint, point.z);
		}

		internal static bool FaceRaycast(Ray worldRay, ProBuilderMesh mesh, out RaycastHit hit, HashSet<Face> ignore = null)
		{
			return HandleUtility.FaceRaycast(worldRay, mesh, out hit, float.PositiveInfinity, CullingMode.Back, ignore);
		}

		internal static bool FaceRaycast(Ray worldRay, ProBuilderMesh mesh, out RaycastHit hit, float distance, CullingMode cullingMode, HashSet<Face> ignore = null)
		{
			worldRay.origin -= mesh.transform.position;
			worldRay.origin = mesh.transform.worldToLocalMatrix * worldRay.origin;
			worldRay.direction = mesh.transform.worldToLocalMatrix * worldRay.direction;
			Vector3[] positionsInternal = mesh.positionsInternal;
			Face[] facesInternal = mesh.facesInternal;
			float num = float.PositiveInfinity;
			int num2 = -1;
			Vector3 normal = Vector3.zero;
			int i = 0;
			int num3 = facesInternal.Length;
			while (i < num3)
			{
				if (ignore == null || !ignore.Contains(facesInternal[i]))
				{
					int[] indexesInternal = mesh.facesInternal[i].indexesInternal;
					int j = 0;
					int num4 = indexesInternal.Length;
					while (j < num4)
					{
						Vector3 vector = positionsInternal[indexesInternal[j]];
						Vector3 vector2 = positionsInternal[indexesInternal[j + 1]];
						Vector3 vector3 = positionsInternal[indexesInternal[j + 2]];
						Vector3 vector4 = Vector3.Cross(vector2 - vector, vector3 - vector);
						float num5 = Vector3.Dot(worldRay.direction, vector4);
						bool flag = false;
						if (cullingMode != CullingMode.Back)
						{
							if (cullingMode == CullingMode.Front && num5 < 0f)
							{
								flag = true;
							}
						}
						else if (num5 > 0f)
						{
							flag = true;
						}
						float num6 = 0f;
						Vector3 vector5;
						if (!flag && Math.RayIntersectsTriangle(worldRay, vector, vector2, vector3, out num6, out vector5) && num6 <= num && num6 <= distance)
						{
							normal = vector4;
							num2 = i;
							num = num6;
						}
						j += 3;
					}
				}
				i++;
			}
			hit = new RaycastHit(num, worldRay.GetPoint(num), normal, num2);
			return num2 > -1;
		}

		internal static bool FaceRaycastBothCullModes(Ray worldRay, ProBuilderMesh mesh, ref SimpleTuple<Face, Vector3> back, ref SimpleTuple<Face, Vector3> front)
		{
			worldRay.origin -= mesh.transform.position;
			worldRay.origin = mesh.transform.worldToLocalMatrix * worldRay.origin;
			worldRay.direction = mesh.transform.worldToLocalMatrix * worldRay.direction;
			Vector3[] positionsInternal = mesh.positionsInternal;
			Face[] facesInternal = mesh.facesInternal;
			back.item1 = null;
			front.item1 = null;
			float num = float.PositiveInfinity;
			float num2 = float.PositiveInfinity;
			int i = 0;
			int num3 = facesInternal.Length;
			while (i < num3)
			{
				int[] indexesInternal = mesh.facesInternal[i].indexesInternal;
				int j = 0;
				int num4 = indexesInternal.Length;
				while (j < num4)
				{
					Vector3 vector = positionsInternal[indexesInternal[j]];
					Vector3 vector2 = positionsInternal[indexesInternal[j + 1]];
					Vector3 vector3 = positionsInternal[indexesInternal[j + 2]];
					float num5;
					Vector3 vector4;
					if (Math.RayIntersectsTriangle(worldRay, vector, vector2, vector3, out num5, out vector4) && (num5 < num || num5 < num2))
					{
						Vector3 rhs = Vector3.Cross(vector2 - vector, vector3 - vector);
						if (Vector3.Dot(worldRay.direction, rhs) < 0f)
						{
							if (num5 < num)
							{
								num = num5;
								back.item1 = facesInternal[i];
							}
						}
						else if (num5 < num2)
						{
							num2 = num5;
							front.item1 = facesInternal[i];
						}
					}
					j += 3;
				}
				i++;
			}
			if (back.item1 != null)
			{
				back.item2 = worldRay.GetPoint(num);
			}
			if (front.item1 != null)
			{
				front.item2 = worldRay.GetPoint(num2);
			}
			return back.item1 != null || front.item1 != null;
		}

		internal static bool FaceRaycast(Ray InWorldRay, ProBuilderMesh mesh, out List<RaycastHit> hits, CullingMode cullingMode, HashSet<Face> ignore = null)
		{
			InWorldRay.origin -= mesh.transform.position;
			InWorldRay.origin = mesh.transform.worldToLocalMatrix * InWorldRay.origin;
			InWorldRay.direction = mesh.transform.worldToLocalMatrix * InWorldRay.direction;
			Vector3[] positionsInternal = mesh.positionsInternal;
			hits = new List<RaycastHit>();
			for (int i = 0; i < mesh.facesInternal.Length; i++)
			{
				if (ignore == null || !ignore.Contains(mesh.facesInternal[i]))
				{
					int[] indexesInternal = mesh.facesInternal[i].indexesInternal;
					for (int j = 0; j < indexesInternal.Length; j += 3)
					{
						Vector3 vector = positionsInternal[indexesInternal[j]];
						Vector3 vector2 = positionsInternal[indexesInternal[j + 1]];
						Vector3 vector3 = positionsInternal[indexesInternal[j + 2]];
						float distance = 0f;
						Vector3 vector4;
						if (Math.RayIntersectsTriangle(InWorldRay, vector, vector2, vector3, out distance, out vector4))
						{
							Vector3 vector5 = Vector3.Cross(vector2 - vector, vector3 - vector);
							switch (cullingMode)
							{
							case CullingMode.Back:
								if (Vector3.Dot(InWorldRay.direction, vector5) >= 0f)
								{
									goto IL_162;
								}
								break;
							case CullingMode.Front:
								if (Vector3.Dot(InWorldRay.direction, vector5) <= 0f)
								{
									goto IL_162;
								}
								break;
							case CullingMode.FrontBack:
								break;
							default:
								goto IL_162;
							}
							hits.Add(new RaycastHit(distance, InWorldRay.GetPoint(distance), vector5, i));
						}
						IL_162:;
					}
				}
			}
			return hits.Count > 0;
		}

		internal static Ray InverseTransformRay(this Transform transform, Ray InWorldRay)
		{
			Vector3 vector = InWorldRay.origin;
			vector -= transform.position;
			vector = transform.worldToLocalMatrix * vector;
			Vector3 direction = transform.worldToLocalMatrix.MultiplyVector(InWorldRay.direction);
			return new Ray(vector, direction);
		}

		internal static bool MeshRaycast(Ray InWorldRay, GameObject gameObject, out RaycastHit hit, float distance = float.PositiveInfinity)
		{
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			Mesh mesh = (component != null) ? component.sharedMesh : null;
			if (!mesh)
			{
				hit = null;
				return false;
			}
			return HandleUtility.MeshRaycast(gameObject.transform.InverseTransformRay(InWorldRay), mesh.vertices, mesh.triangles, out hit, distance);
		}

		internal static bool MeshRaycast(Ray InRay, Vector3[] mesh, int[] triangles, out RaycastHit hit, float distance = float.PositiveInfinity)
		{
			float num = float.PositiveInfinity;
			Vector3 normal = Vector3.zero;
			Vector3 zero = Vector3.zero;
			int num2 = -1;
			Vector3 origin = InRay.origin;
			Vector3 direction = InRay.direction;
			for (int i = 0; i < triangles.Length; i += 3)
			{
				Vector3 vert = mesh[triangles[i]];
				Vector3 vert2 = mesh[triangles[i + 1]];
				Vector3 vert3 = mesh[triangles[i + 2]];
				if (Math.RayIntersectsTriangle2(origin, direction, vert, vert2, vert3, ref distance, ref zero) && distance < num)
				{
					num2 = i / 3;
					num = distance;
					normal = zero;
				}
			}
			hit = new RaycastHit(num, InRay.GetPoint(num), normal, num2);
			return num2 > -1;
		}

		internal static bool PointIsOccluded(Camera cam, ProBuilderMesh pb, Vector3 worldPoint)
		{
			Vector3 normalized = (cam.transform.position - worldPoint).normalized;
			RaycastHit raycastHit;
			return HandleUtility.FaceRaycast(new Ray(worldPoint + normalized * 0.0001f, normalized), pb, out raycastHit, Vector3.Distance(cam.transform.position, worldPoint), CullingMode.Front, null);
		}

		public static Quaternion GetRotation(ProBuilderMesh mesh, IEnumerable<int> indices)
		{
			if (!mesh.HasArrays(MeshArrays.Normal))
			{
				Normals.CalculateNormals(mesh);
			}
			if (!mesh.HasArrays(MeshArrays.Tangent))
			{
				Normals.CalculateTangents(mesh);
			}
			Vector3[] normalsInternal = mesh.normalsInternal;
			Vector4[] tangentsInternal = mesh.tangentsInternal;
			Vector3 zero = Vector3.zero;
			Vector4 zero2 = Vector4.zero;
			float num = 0f;
			foreach (int num2 in indices)
			{
				Vector3 vector = normalsInternal[num2];
				Vector4 vector2 = tangentsInternal[num2];
				zero.x += vector.x;
				zero.y += vector.y;
				zero.z += vector.z;
				zero2.x += vector2.x;
				zero2.y += vector2.y;
				zero2.z += vector2.z;
				zero2.w += vector2.w;
				num += 1f;
			}
			zero.x /= num;
			zero.y /= num;
			zero.z /= num;
			zero2.x /= num;
			zero2.y /= num;
			zero2.z /= num;
			zero2.w /= num;
			if (zero == Vector3.zero || zero2 == Vector4.zero)
			{
				return mesh.transform.rotation;
			}
			Vector3 upwards = Vector3.Cross(zero, zero2 * zero2.w);
			return mesh.transform.rotation * Quaternion.LookRotation(zero, upwards);
		}

		public static Quaternion GetFaceRotation(ProBuilderMesh mesh, HandleOrientation orientation, IEnumerable<Face> faces)
		{
			if (mesh == null)
			{
				return Quaternion.identity;
			}
			if (orientation == HandleOrientation.ActiveObject)
			{
				return mesh.transform.rotation;
			}
			if (orientation == HandleOrientation.ActiveElement)
			{
				return HandleUtility.GetFaceRotation(mesh, faces.Last<Face>());
			}
			return Quaternion.identity;
		}

		public static Quaternion GetFaceRotation(ProBuilderMesh mesh, Face face)
		{
			if (mesh == null)
			{
				return Quaternion.identity;
			}
			if (face == null)
			{
				return mesh.transform.rotation;
			}
			Normal normal = Math.NormalTangentBitangent(mesh, face);
			if (normal.normal == Vector3.zero || normal.bitangent == Vector3.zero)
			{
				return mesh.transform.rotation;
			}
			return mesh.transform.rotation * Quaternion.LookRotation(normal.normal, normal.bitangent);
		}

		public static Quaternion GetEdgeRotation(ProBuilderMesh mesh, HandleOrientation orientation, IEnumerable<Edge> edges)
		{
			if (mesh == null)
			{
				return Quaternion.identity;
			}
			if (orientation == HandleOrientation.ActiveObject)
			{
				return mesh.transform.rotation;
			}
			if (orientation == HandleOrientation.ActiveElement)
			{
				return HandleUtility.GetEdgeRotation(mesh, edges.Last<Edge>());
			}
			return Quaternion.identity;
		}

		public static Quaternion GetEdgeRotation(ProBuilderMesh mesh, Edge edge)
		{
			if (mesh == null)
			{
				return Quaternion.identity;
			}
			return HandleUtility.GetFaceRotation(mesh, mesh.GetFace(edge));
		}

		public static Quaternion GetVertexRotation(ProBuilderMesh mesh, HandleOrientation orientation, IEnumerable<int> vertices)
		{
			if (mesh == null)
			{
				return Quaternion.identity;
			}
			if (orientation != HandleOrientation.ActiveObject)
			{
				if (orientation != HandleOrientation.ActiveElement)
				{
					return Quaternion.identity;
				}
				if (mesh.selectedVertexCount >= 1)
				{
					return HandleUtility.GetRotation(mesh, vertices);
				}
			}
			return mesh.transform.rotation;
		}

		public static Quaternion GetVertexRotation(ProBuilderMesh mesh, int vertex)
		{
			if (mesh == null)
			{
				return Quaternion.identity;
			}
			if (vertex < 0)
			{
				return mesh.transform.rotation;
			}
			return HandleUtility.GetRotation(mesh, new int[]
			{
				vertex
			});
		}

		internal static Vector3 GetActiveElementPosition(ProBuilderMesh mesh, IEnumerable<Face> faces)
		{
			return mesh.transform.TransformPoint(Math.GetBounds(mesh.positionsInternal, faces.Last<Face>().distinctIndexesInternal).center);
		}

		internal static Vector3 GetActiveElementPosition(ProBuilderMesh mesh, IEnumerable<Edge> edges)
		{
			Edge edge = edges.Last<Edge>();
			return mesh.transform.TransformPoint(Math.GetBounds(mesh.positionsInternal, new int[]
			{
				edge.a,
				edge.b
			}).center);
		}

		internal static Vector3 GetActiveElementPosition(ProBuilderMesh mesh, IEnumerable<int> vertices)
		{
			return mesh.transform.TransformPoint(mesh.positionsInternal[vertices.First<int>()]);
		}
	}
}
