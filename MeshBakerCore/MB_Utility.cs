using System;
using Unity.Collections;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB_Utility
	{
		public static Texture2D createTextureCopy(Texture2D source, bool expectedToBeGammaCorrectedHint)
		{
			Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true, !MBVersion.IsTexture_sRGBgammaCorrected(source, expectedToBeGammaCorrectedHint));
			texture2D.SetPixels(source.GetPixels());
			return texture2D;
		}

		public static bool ArrayBIsSubsetOfA(object[] a, object[] b)
		{
			for (int i = 0; i < b.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < a.Length; j++)
				{
					if (a[j] == b[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public static Material[] GetGOMaterials(GameObject go)
		{
			if (go == null)
			{
				return new Material[0];
			}
			Material[] array = null;
			Mesh mesh = null;
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				array = component.sharedMaterials;
				MeshFilter component2 = go.GetComponent<MeshFilter>();
				if (component2 == null)
				{
					throw new Exception("Object " + ((go != null) ? go.ToString() : null) + " has a MeshRenderer but no MeshFilter.");
				}
				mesh = component2.sharedMesh;
			}
			SkinnedMeshRenderer component3 = go.GetComponent<SkinnedMeshRenderer>();
			if (component3 != null)
			{
				array = component3.sharedMaterials;
				mesh = component3.sharedMesh;
			}
			if (array == null)
			{
				Debug.LogError("Object " + go.name + " does not have a MeshRenderer or a SkinnedMeshRenderer component");
				return new Material[0];
			}
			if (mesh == null)
			{
				Debug.LogError("Object " + go.name + " has a MeshRenderer or SkinnedMeshRenderer but no mesh.");
				return new Material[0];
			}
			if (mesh.subMeshCount < array.Length)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Object ",
					(go != null) ? go.ToString() : null,
					" has only ",
					mesh.subMeshCount.ToString(),
					" submeshes and has ",
					array.Length.ToString(),
					" materials. Extra materials do nothing."
				}));
				Material[] array2 = new Material[mesh.subMeshCount];
				Array.Copy(array, array2, array2.Length);
				array = array2;
			}
			return array;
		}

		public static Mesh GetMesh(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component != null)
			{
				return component.sharedMesh;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				return component2.sharedMesh;
			}
			return null;
		}

		public static void SetMesh(GameObject go, Mesh m)
		{
			if (go == null)
			{
				return;
			}
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component != null)
			{
				component.sharedMesh = m;
				return;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				component2.sharedMesh = m;
			}
		}

		public static Renderer GetRenderer(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				return component;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				return component2;
			}
			return null;
		}

		public static void DisableRendererInSource(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = false;
				return;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				component2.enabled = false;
				return;
			}
		}

		public static bool hasOutOfBoundsUVs(Mesh m, ref Rect uvBounds)
		{
			MB_Utility.MeshAnalysisResult meshAnalysisResult = default(MB_Utility.MeshAnalysisResult);
			bool result = MB_Utility.hasOutOfBoundsUVs(m, ref meshAnalysisResult, -1, 0);
			uvBounds = meshAnalysisResult.uvRect;
			return result;
		}

		public static bool hasOutOfBoundsUVs(Mesh m, ref MB_Utility.MeshAnalysisResult putResultHere, int submeshIndex = -1, int uvChannel = 0)
		{
			if (m == null)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				return putResultHere.hasOutOfBoundsUVs;
			}
			Vector2[] uvs;
			if (uvChannel == 0)
			{
				uvs = m.uv;
			}
			else if (uvChannel == 1)
			{
				uvs = m.uv2;
			}
			else if (uvChannel == 2)
			{
				uvs = m.uv3;
			}
			else
			{
				uvs = m.uv4;
			}
			return MB_Utility.hasOutOfBoundsUVs(uvs, m, ref putResultHere, submeshIndex);
		}

		public static bool hasOutOfBoundsUVs(Vector2[] uvs, Mesh m, ref MB_Utility.MeshAnalysisResult putResultHere, int submeshIndex = -1)
		{
			putResultHere.hasUVs = true;
			if (uvs.Length == 0)
			{
				putResultHere.hasUVs = false;
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = default(Rect);
				return putResultHere.hasOutOfBoundsUVs;
			}
			if (submeshIndex >= m.subMeshCount)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = default(Rect);
				return putResultHere.hasOutOfBoundsUVs;
			}
			float x;
			float num;
			float y;
			float num2;
			if (submeshIndex >= 0)
			{
				int[] triangles = m.GetTriangles(submeshIndex);
				if (triangles.Length == 0)
				{
					putResultHere.hasOutOfBoundsUVs = false;
					putResultHere.uvRect = default(Rect);
					return putResultHere.hasOutOfBoundsUVs;
				}
				num = (x = uvs[triangles[0]].x);
				num2 = (y = uvs[triangles[0]].y);
				foreach (int num3 in triangles)
				{
					if (uvs[num3].x < x)
					{
						x = uvs[num3].x;
					}
					if (uvs[num3].x > num)
					{
						num = uvs[num3].x;
					}
					if (uvs[num3].y < y)
					{
						y = uvs[num3].y;
					}
					if (uvs[num3].y > num2)
					{
						num2 = uvs[num3].y;
					}
				}
			}
			else
			{
				num = (x = uvs[0].x);
				num2 = (y = uvs[0].y);
				for (int j = 0; j < uvs.Length; j++)
				{
					if (uvs[j].x < x)
					{
						x = uvs[j].x;
					}
					if (uvs[j].x > num)
					{
						num = uvs[j].x;
					}
					if (uvs[j].y < y)
					{
						y = uvs[j].y;
					}
					if (uvs[j].y > num2)
					{
						num2 = uvs[j].y;
					}
				}
			}
			Rect uvRect = default(Rect);
			uvRect.x = x;
			uvRect.y = y;
			uvRect.width = num - x;
			uvRect.height = num2 - y;
			if (num > 1f || x < 0f || num2 > 1f || y < 0f)
			{
				putResultHere.hasOutOfBoundsUVs = true;
			}
			else
			{
				putResultHere.hasOutOfBoundsUVs = false;
			}
			putResultHere.uvRect = uvRect;
			return putResultHere.hasOutOfBoundsUVs;
		}

		public static bool hasOutOfBoundsUVs(NativeArray<Vector2> uvs, Mesh m, ref MB_Utility.MeshAnalysisResult putResultHere, int submeshIndex = -1)
		{
			putResultHere.hasUVs = true;
			if (uvs.Length == 0)
			{
				putResultHere.hasUVs = false;
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = default(Rect);
				return putResultHere.hasOutOfBoundsUVs;
			}
			if (submeshIndex >= m.subMeshCount)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = default(Rect);
				return putResultHere.hasOutOfBoundsUVs;
			}
			float x;
			float num;
			float y;
			float num2;
			if (submeshIndex >= 0)
			{
				int[] triangles = m.GetTriangles(submeshIndex);
				if (triangles.Length == 0)
				{
					putResultHere.hasOutOfBoundsUVs = false;
					putResultHere.uvRect = default(Rect);
					return putResultHere.hasOutOfBoundsUVs;
				}
				num = (x = uvs[triangles[0]].x);
				num2 = (y = uvs[triangles[0]].y);
				foreach (int index in triangles)
				{
					if (uvs[index].x < x)
					{
						x = uvs[index].x;
					}
					if (uvs[index].x > num)
					{
						num = uvs[index].x;
					}
					if (uvs[index].y < y)
					{
						y = uvs[index].y;
					}
					if (uvs[index].y > num2)
					{
						num2 = uvs[index].y;
					}
				}
			}
			else
			{
				num = (x = uvs[0].x);
				num2 = (y = uvs[0].y);
				for (int j = 0; j < uvs.Length; j++)
				{
					if (uvs[j].x < x)
					{
						x = uvs[j].x;
					}
					if (uvs[j].x > num)
					{
						num = uvs[j].x;
					}
					if (uvs[j].y < y)
					{
						y = uvs[j].y;
					}
					if (uvs[j].y > num2)
					{
						num2 = uvs[j].y;
					}
				}
			}
			Rect uvRect = default(Rect);
			uvRect.x = x;
			uvRect.y = y;
			uvRect.width = num - x;
			uvRect.height = num2 - y;
			if (num > 1f || x < 0f || num2 > 1f || y < 0f)
			{
				putResultHere.hasOutOfBoundsUVs = true;
			}
			else
			{
				putResultHere.hasOutOfBoundsUVs = false;
			}
			putResultHere.uvRect = uvRect;
			return putResultHere.hasOutOfBoundsUVs;
		}

		public static void setSolidColor(Texture2D t, Color c)
		{
			Color[] pixels = t.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = c;
			}
			t.SetPixels(pixels);
			t.Apply();
		}

		public static Texture2D resampleTexture(Texture2D source, bool expectToBeGammaCorrectedHint, int newWidth, int newHeight)
		{
			TextureFormat format = source.format;
			if (format == TextureFormat.ARGB32 || format == TextureFormat.RGBA32 || format == TextureFormat.BGRA32 || format == TextureFormat.RGB24 || format == TextureFormat.Alpha8 || format == TextureFormat.DXT1)
			{
				Texture2D texture2D = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, true, !MBVersion.IsTexture_sRGBgammaCorrected(source, expectToBeGammaCorrectedHint));
				float num = (float)newWidth;
				float num2 = (float)newHeight;
				for (int i = 0; i < newWidth; i++)
				{
					for (int j = 0; j < newHeight; j++)
					{
						float u = (float)i / num;
						float v = (float)j / num2;
						texture2D.SetPixel(i, j, source.GetPixelBilinear(u, v));
					}
				}
				texture2D.Apply();
				return texture2D;
			}
			Debug.LogError("Can only resize textures in formats ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. texture:" + ((source != null) ? source.ToString() : null) + " was in format: " + source.format.ToString());
			return null;
		}

		public static bool AreAllSharedMaterialsDistinct(Material[] sharedMaterials)
		{
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				for (int j = i + 1; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[i] == sharedMaterials[j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static void doSubmeshesShareVertsOrTris(Mesh m, ref MB_Utility.MeshAnalysisResult mar)
		{
			int[][] array = new int[m.subMeshCount][];
			for (int i = 0; i < m.subMeshCount; i++)
			{
				array[i] = m.GetTriangles(i);
			}
			int[] array2 = new int[m.vertexCount];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = -1;
			}
			bool flag = false;
			for (int k = 0; k < m.subMeshCount; k++)
			{
				foreach (int num in array[k])
				{
					if (array2[num] != -1 && array2[num] != k)
					{
						flag = true;
						break;
					}
					array2[num] = k;
				}
			}
			if (flag)
			{
				mar.hasOverlappingSubmeshVerts = true;
				return;
			}
			mar.hasOverlappingSubmeshVerts = false;
		}

		public static bool GetBounds(GameObject go, out Bounds b)
		{
			if (go == null)
			{
				Debug.LogError("go paramater was null");
				b = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
			Renderer renderer = MB_Utility.GetRenderer(go);
			if (renderer == null)
			{
				Debug.LogError("GetBounds must be called on an object with a Renderer");
				b = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
			if (renderer is MeshRenderer)
			{
				b = renderer.bounds;
				return true;
			}
			if (renderer is SkinnedMeshRenderer)
			{
				b = renderer.bounds;
				return true;
			}
			Debug.LogError("GetBounds must be called on an object with a MeshRender or a SkinnedMeshRenderer.");
			b = new Bounds(Vector3.zero, Vector3.zero);
			return false;
		}

		public static void Destroy(Object o)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(o);
				return;
			}
			Object.DestroyImmediate(o, false);
		}

		public static string ConvertAssetsRelativePathToFullSystemPath(string pth)
		{
			return Application.dataPath.Replace("Assets", "") + pth;
		}

		public static bool IsSceneInstance(GameObject go)
		{
			return go.scene.name != null;
		}

		public static string BoneWeightToString(BoneWeight bw)
		{
			return string.Format("BoneWeight  {0}:{1},  {2}:{3},  {4}:{5}, {6}:{7}", new object[]
			{
				bw.boneIndex0,
				bw.weight0,
				bw.boneIndex1,
				bw.weight1,
				bw.boneIndex2,
				bw.weight2,
				bw.boneIndex3,
				bw.weight3
			});
		}

		public static bool DO_INTEGRITY_CHECKS;

		public struct MeshAnalysisResult
		{
			public Rect uvRect;

			public bool hasOutOfBoundsUVs;

			public bool hasOverlappingSubmeshVerts;

			public bool hasUVs;

			public float submeshArea;
		}

		private class MB_Triangle
		{
			public bool isSame(object obj)
			{
				MB_Utility.MB_Triangle mb_Triangle = (MB_Utility.MB_Triangle)obj;
				return this.vs[0] == mb_Triangle.vs[0] && this.vs[1] == mb_Triangle.vs[1] && this.vs[2] == mb_Triangle.vs[2] && this.submeshIdx != mb_Triangle.submeshIdx;
			}

			public bool sharesVerts(MB_Utility.MB_Triangle obj)
			{
				return ((this.vs[0] == obj.vs[0] || this.vs[0] == obj.vs[1] || this.vs[0] == obj.vs[2]) && this.submeshIdx != obj.submeshIdx) || ((this.vs[1] == obj.vs[0] || this.vs[1] == obj.vs[1] || this.vs[1] == obj.vs[2]) && this.submeshIdx != obj.submeshIdx) || ((this.vs[2] == obj.vs[0] || this.vs[2] == obj.vs[1] || this.vs[2] == obj.vs[2]) && this.submeshIdx != obj.submeshIdx);
			}

			public void Initialize(int[] ts, int idx, int sIdx)
			{
				this.vs[0] = ts[idx];
				this.vs[1] = ts[idx + 1];
				this.vs[2] = ts[idx + 2];
				this.submeshIdx = sIdx;
				Array.Sort<int>(this.vs);
			}

			private int submeshIdx;

			private int[] vs = new int[3];
		}
	}
}
