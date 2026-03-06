using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
	public static class Utilities
	{
		public static Bounds? GetPrefabBounds(GameObject prefab)
		{
			Bounds? result;
			if (Utilities.prefabBoundsCache.TryGetValue(prefab, out result))
			{
				return result;
			}
			Bounds? bounds = Utilities.CalculateBoundsRecursively(prefab.transform);
			Utilities.prefabBoundsCache.Add(prefab, bounds);
			return bounds;
		}

		private static Bounds? CalculateBoundsRecursively(Transform transform)
		{
			Bounds? result = null;
			Renderer component = transform.GetComponent<Renderer>();
			if (component != null && component.bounds.size != Vector3.zero && !(component is ParticleSystemRenderer))
			{
				result = new Bounds?(component.bounds);
			}
			foreach (object obj in transform.transform)
			{
				Bounds? bounds = Utilities.CalculateBoundsRecursively((Transform)obj);
				if (bounds != null)
				{
					if (result != null)
					{
						Bounds value = result.Value;
						value.Encapsulate(bounds.Value);
						result = new Bounds?(value);
					}
					else
					{
						result = bounds;
					}
				}
			}
			return result;
		}

		internal static Mesh SetupAnchorMeshGeometry(MRUKAnchor anchorInfo, bool useFunctionalSurfaces = false, EffectMesh.TextureCoordinateModes[] textureCoordinateModes = null)
		{
			bool flag = false;
			int num = 24;
			int num2 = 36;
			if (anchorInfo.VolumeBounds != null || anchorInfo.PlaneRect != null)
			{
				if (anchorInfo.PlaneRect != null && (useFunctionalSurfaces || anchorInfo.VolumeBounds == null))
				{
					num = anchorInfo.PlaneBoundary2D.Count;
					num2 = (anchorInfo.PlaneBoundary2D.Count - 2) * 3;
					flag = true;
				}
				Vector3[] vertices = new Vector3[num];
				Color32[] colors = new Color32[num];
				Vector3[] normals = new Vector3[num];
				Vector4[] tangents = new Vector4[num];
				int[] triangles = new int[num2];
				int num3 = (textureCoordinateModes == null) ? 0 : Math.Min(8, textureCoordinateModes.Length);
				Vector2[][] array = new Vector2[num3][];
				for (int i = 0; i < num3; i++)
				{
					array[i] = new Vector2[num];
				}
				if (flag)
				{
					Utilities.CreatePolygonMesh(anchorInfo, ref vertices, ref colors, ref normals, ref tangents, ref triangles, ref array, textureCoordinateModes);
				}
				else
				{
					Utilities.CreateVolumeMesh(anchorInfo, ref vertices, ref colors, ref normals, ref tangents, ref triangles, ref array, textureCoordinateModes);
				}
				Mesh mesh = new Mesh
				{
					name = anchorInfo.name,
					vertices = vertices,
					colors32 = colors,
					triangles = triangles,
					normals = normals,
					tangents = tangents
				};
				for (int j = 0; j < num3; j++)
				{
					switch (j)
					{
					case 0:
						mesh.uv = array[j];
						break;
					case 1:
						mesh.uv2 = array[j];
						break;
					case 2:
						mesh.uv3 = array[j];
						break;
					case 3:
						mesh.uv4 = array[j];
						break;
					case 4:
						mesh.uv5 = array[j];
						break;
					case 5:
						mesh.uv6 = array[j];
						break;
					case 6:
						mesh.uv7 = array[j];
						break;
					case 7:
						mesh.uv8 = array[j];
						break;
					}
				}
				mesh.name = anchorInfo.name;
				return mesh;
			}
			if (anchorInfo.Mesh != null)
			{
				return anchorInfo.Mesh;
			}
			throw new InvalidOperationException("No valid geometry data available.");
		}

		private static void CreateVolumeMesh(MRUKAnchor anchorInfo, ref Vector3[] meshVertices, ref Color32[] meshColors, ref Vector3[] meshNormals, ref Vector4[] meshTangents, ref int[] meshTriangles, ref Vector2[][] meshUVs, EffectMesh.TextureCoordinateModes[] textureCoordinateModes = null)
		{
			if (anchorInfo.VolumeBounds == null)
			{
				throw new Exception("Can not create a volume mesh for an anchor without volume bounds.");
			}
			Bounds value = anchorInfo.VolumeBounds.Value;
			Vector3 size = value.size;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 6; i++)
			{
				float x = size.x;
				float y = size.y;
				Vector3 vector;
				Vector3 vector2;
				Vector3 a;
				Vector3 vector3;
				switch (i)
				{
				case 0:
					vector = new Vector3(size.x, size.y, size.z);
					vector2 = Vector3.right;
					a = Vector3.up;
					vector3 = Vector3.forward;
					break;
				case 1:
					vector = new Vector3(size.x, size.z, size.y);
					vector2 = Vector3.right;
					a = -Vector3.forward;
					vector3 = Vector3.up;
					y = size.z;
					break;
				case 2:
					vector = new Vector3(size.x, size.y, size.z);
					vector2 = Vector3.right;
					a = -Vector3.up;
					vector3 = -Vector3.forward;
					break;
				case 3:
					vector = new Vector3(size.x, size.z, size.y);
					vector2 = Vector3.right;
					a = Vector3.forward;
					vector3 = -Vector3.up;
					y = size.z;
					break;
				case 4:
					vector = new Vector3(size.z, size.y, size.x);
					vector2 = -Vector3.forward;
					a = Vector3.up;
					vector3 = Vector3.right;
					x = size.z;
					break;
				case 5:
					vector = new Vector3(size.z, size.y, size.x);
					vector2 = Vector3.forward;
					a = Vector3.up;
					vector3 = -Vector3.right;
					x = size.z;
					break;
				default:
					throw new IndexOutOfRangeException("Index j is out of range");
				}
				for (int j = 0; j < 4; j++)
				{
					float num4 = (j / 2 == 0) ? 0f : 1f;
					float num5 = (j == 1 || j == 2) ? 1f : 0f;
					Vector3 vector4 = value.center - a * vector.y * 0.5f + vector2 * vector.x * 0.5f + vector3 * vector.z * 0.5f;
					vector4 += a * vector.y * num5 - vector2 * vector.x * num4;
					Vector2 a2 = new Vector2(num4, num5);
					for (int k = 0; k < meshUVs.Length; k++)
					{
						Vector2 one = Vector2.one;
						if (textureCoordinateModes != null && textureCoordinateModes[k].AnchorUV == EffectMesh.AnchorTextureCoordinateMode.METRIC)
						{
							one.x = x;
							one.y = y;
						}
						meshUVs[k][num] = Vector2.Scale(a2, one);
					}
					meshVertices[num] = vector4;
					meshColors[num] = Color.white;
					meshNormals[num] = vector3;
					meshTangents[num] = new Vector4(-vector2.x, -vector2.y, -vector2.z, -1f);
					num++;
				}
				Utilities.CreateInteriorTriangleFan(ref meshTriangles, ref num2, num3, 4);
				num3 += 4;
			}
		}

		private static void CreatePolygonMesh(MRUKAnchor anchorInfo, ref Vector3[] meshVertices, ref Color32[] meshColors, ref Vector3[] meshNormals, ref Vector4[] meshTangents, ref int[] meshTriangles, ref Vector2[][] meshUVs, EffectMesh.TextureCoordinateModes[] textureCoordinateModes)
		{
			if (anchorInfo.PlaneRect == null || anchorInfo.PlaneBoundary2D == null)
			{
				throw new Exception("Not enough plane data associated to this anchor to create a polygon mesh.");
			}
			Rect value = anchorInfo.PlaneRect.Value;
			List<Vector2> planeBoundary2D = anchorInfo.PlaneBoundary2D;
			int num = 0;
			int num2 = 0;
			int baseCount = 0;
			for (int i = 0; i < planeBoundary2D.Count; i++)
			{
				Vector2 vector = planeBoundary2D[i];
				for (int j = 0; j < meshUVs.Length; j++)
				{
					Vector2 one = Vector2.one;
					if (textureCoordinateModes[j].AnchorUV == EffectMesh.AnchorTextureCoordinateMode.STRETCH)
					{
						one = new Vector2(1f / (value.xMax - value.xMin), 1f / (value.yMax - value.yMin));
					}
					meshUVs[j][num] = Vector2.Scale(new Vector2(value.xMax - vector.x, vector.y - value.yMin), one);
				}
				meshVertices[num] = new Vector3(vector.x, vector.y, 0f);
				meshColors[num] = Color.white;
				meshNormals[num] = Vector3.forward;
				meshTangents[num] = new Vector4(1f, 0f, 0f, 1f);
				num++;
			}
			Utilities.CreateInteriorPolygon(ref meshTriangles, ref num2, baseCount, planeBoundary2D);
		}

		internal static void CreateInteriorPolygon(ref int[] indexArray, ref int indexCounter, int baseCount, List<Vector2> points)
		{
			Vector2[] array;
			int[] array2;
			Triangulator.TriangulatePoints(points, null, out array, out array2);
			int num = array2.Length / 3;
			for (int i = 0; i < num; i++)
			{
				int num2 = array2[i * 3];
				int num3 = array2[i * 3 + 1];
				int num4 = array2[i * 3 + 2];
				int[] array3 = indexArray;
				int num5 = indexCounter;
				indexCounter = num5 + 1;
				array3[num5] = baseCount + num2;
				int[] array4 = indexArray;
				num5 = indexCounter;
				indexCounter = num5 + 1;
				array4[num5] = baseCount + num3;
				int[] array5 = indexArray;
				num5 = indexCounter;
				indexCounter = num5 + 1;
				array5[num5] = baseCount + num4;
			}
		}

		internal static void CreateInteriorTriangleFan(ref int[] indexArray, ref int indexCounter, int baseCount, int pointsInLoop)
		{
			int num = pointsInLoop - 2;
			for (int i = 0; i < num; i++)
			{
				int num2 = i + 1;
				int num3 = i + 2;
				int[] array = indexArray;
				int num4 = indexCounter;
				indexCounter = num4 + 1;
				array[num4] = baseCount;
				int[] array2 = indexArray;
				num4 = indexCounter;
				indexCounter = num4 + 1;
				array2[num4] = baseCount + num2;
				int[] array3 = indexArray;
				num4 = indexCounter;
				indexCounter = num4 + 1;
				array3[num4] = baseCount + num3;
			}
		}

		internal static Mesh AddBarycentricCoordinatesToMesh(Mesh originalMesh)
		{
			Vector3[] vertices = originalMesh.vertices;
			int[] triangles = originalMesh.triangles;
			int num = triangles.Length;
			NativeArray<Vector3> vertices2 = new NativeArray<Vector3>(num, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			NativeArray<Color> colors = new NativeArray<Color>(num, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			NativeArray<int> indices = new NativeArray<int>(num, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < num; i++)
			{
				colors[i] = new Color((i % 3 == 0) ? 1f : 0f, (i % 3 == 1) ? 1f : 0f, (i % 3 == 2) ? 1f : 0f);
				vertices2[i] = vertices[triangles[i]];
				indices[i] = i;
			}
			Mesh mesh = new Mesh();
			mesh.indexFormat = ((vertices2.Length > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			mesh.SetVertices<Vector3>(vertices2);
			mesh.SetColors<Color>(colors);
			mesh.SetIndices<int>(indices, MeshTopology.Triangles, 0, true, 0);
			return mesh;
		}

		internal static void DestroyGameObjectAndChildren(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return;
			}
			foreach (object obj in gameObject.transform)
			{
				Object.DestroyImmediate(((Transform)obj).gameObject);
			}
			Object.DestroyImmediate(gameObject.gameObject);
		}

		public static bool SequenceEqual<T>(this List<T> list1, List<T> list2)
		{
			if (list1 == null && list2 == null)
			{
				return true;
			}
			if (list1 == null && list2 != null)
			{
				return false;
			}
			if (list1 != null && list2 == null)
			{
				return false;
			}
			if (list1.Count != list2.Count)
			{
				return false;
			}
			for (int i = 0; i < list1.Count; i++)
			{
				if (!object.Equals(list1[i], list2[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsPositionInPolygon(Vector2 position, List<Vector2> polygon)
		{
			int num = 0;
			for (int i = 0; i < polygon.Count; i++)
			{
				Vector2 vector = polygon[i];
				Vector2 vector2 = polygon[(i + 1) % polygon.Count];
				if (position.y > Mathf.Min(vector.y, vector2.y) && position.y <= Mathf.Max(vector.y, vector2.y) && position.x <= Mathf.Max(vector.x, vector2.x) && vector.y != vector2.y)
				{
					float num2 = (position.y - vector.y) / (vector2.y - vector.y);
					float num3 = vector.x + num2 * (vector2.x - vector.x);
					if (vector.x == vector2.x || position.x <= num3)
					{
						num++;
					}
				}
			}
			return num % 2 == 1;
		}

		internal static List<string> SceneLabelsEnumToList(MRUKAnchor.SceneLabels labelFlags)
		{
			List<string> list = new List<string>(1);
			foreach (object obj in Enum.GetValues(typeof(MRUKAnchor.SceneLabels)))
			{
				MRUKAnchor.SceneLabels sceneLabels = (MRUKAnchor.SceneLabels)obj;
				if ((labelFlags & sceneLabels) != (MRUKAnchor.SceneLabels)0)
				{
					list.Add(sceneLabels.ToString());
				}
			}
			return list;
		}

		internal static MRUKAnchor.SceneLabels StringLabelsToEnum(IList<string> labels)
		{
			MRUKAnchor.SceneLabels sceneLabels = (MRUKAnchor.SceneLabels)0;
			foreach (string stringLabel in labels)
			{
				sceneLabels |= Utilities.StringLabelToEnum(stringLabel);
			}
			return sceneLabels;
		}

		internal static MRUKAnchor.SceneLabels StringLabelToEnum(string stringLabel)
		{
			OVRSemanticLabels.Classification classification = OVRSemanticLabels.FromApiLabel(stringLabel);
			if (stringLabel != "OTHER" && classification == OVRSemanticLabels.Classification.Other)
			{
				Debug.LogError("Unknown scene label: " + stringLabel);
			}
			return Utilities.ClassificationToSceneLabel(classification);
		}

		internal static MRUKAnchor.SceneLabels ClassificationToSceneLabel(OVRSemanticLabels.Classification classification)
		{
			return (MRUKAnchor.SceneLabels)(1 << (int)classification);
		}

		internal unsafe static Guid ReverseGuidByteOrder(Guid guid)
		{
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)16], 16);
			guid.TryWriteBytes(span);
			span.Slice(0, 4).Reverse<byte>();
			span.Slice(4, 2).Reverse<byte>();
			span.Slice(6, 2).Reverse<byte>();
			return new Guid(span);
		}

		internal static void DrawWireSphere(Vector3 center, float radius, Color color, float duration, int quality = 3)
		{
			quality = Mathf.Clamp(quality, 1, 10);
			int num = quality << 2;
			int num2 = quality << 3;
			int num3 = num >> 1;
			float angle = 360f / (float)num2;
			float num4 = 180f / (float)num;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = Vector3.forward * radius;
				vector = Quaternion.AngleAxis(num4 * (float)(i - num3), Vector3.right) * vector;
				for (int j = 0; j < num2; j++)
				{
					Vector3 vector2 = Quaternion.AngleAxis(angle, Vector3.up) * vector;
					Debug.DrawLine(vector + center, vector2 + center, color, duration);
					vector = vector2;
				}
			}
			for (int k = 0; k < num; k++)
			{
				Vector3 vector = Vector3.forward * radius;
				vector = Quaternion.AngleAxis(num4 * (float)(k - num3), Vector3.up) * vector;
				Vector3 axis = Quaternion.AngleAxis(90f, Vector3.up) * vector;
				for (int l = 0; l < num2; l++)
				{
					Vector3 vector2 = Quaternion.AngleAxis(angle, axis) * vector;
					Debug.DrawLine(vector + center, vector2 + center, color, duration);
					vector = vector2;
				}
			}
		}

		private static Dictionary<GameObject, Bounds?> prefabBoundsCache = new Dictionary<GameObject, Bounds?>();

		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		public static readonly float InvSqrt2 = 1f / Mathf.Sqrt(2f);

		private const int MAX_VERTICES_PER_MESH = 65535;
	}
}
