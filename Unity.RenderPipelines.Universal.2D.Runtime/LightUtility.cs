using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal.UTess;
using UnityEngine.U2D;

namespace UnityEngine.Rendering.Universal
{
	internal static class LightUtility
	{
		public static bool CheckForChange(Light2D.LightType a, ref Light2D.LightType b)
		{
			bool result = a != b;
			b = a;
			return result;
		}

		public static bool CheckForChange(Component a, ref Component b)
		{
			bool result = a != b;
			b = a;
			return result;
		}

		public static bool CheckForChange(int a, ref int b)
		{
			bool result = a != b;
			b = a;
			return result;
		}

		public static bool CheckForChange(float a, ref float b)
		{
			bool result = a != b;
			b = a;
			return result;
		}

		public static bool CheckForChange(bool a, ref bool b)
		{
			bool result = a != b;
			b = a;
			return result;
		}

		private static bool TestPivot(List<IntPoint> path, int activePoint, long lastPoint)
		{
			for (int i = activePoint; i < path.Count; i++)
			{
				if (path[i].N > lastPoint)
				{
					return true;
				}
			}
			return path[activePoint].N == -1L;
		}

		private static List<IntPoint> DegeneratePivots(List<IntPoint> path, List<IntPoint> inPath, ref int interiorStart)
		{
			List<IntPoint> list = new List<IntPoint>();
			long num = path[0].N;
			long num2 = path[0].N;
			for (int i = 1; i < path.Count; i++)
			{
				if (path[i].N != -1L)
				{
					num = Math.Min(num, path[i].N);
					num2 = Math.Max(num2, path[i].N);
				}
			}
			for (long num3 = 0L; num3 < num; num3 += 1L)
			{
				IntPoint item = path[(int)num];
				item.N = num3;
				list.Add(item);
			}
			list.AddRange(path.GetRange(0, path.Count));
			interiorStart = list.Count;
			for (long num4 = num2 + 1L; num4 < (long)inPath.Count; num4 += 1L)
			{
				IntPoint item2 = inPath[(int)num4];
				item2.N = num4;
				list.Add(item2);
			}
			return list;
		}

		private static List<IntPoint> SortPivots(List<IntPoint> outPath, List<IntPoint> inPath)
		{
			List<IntPoint> list = new List<IntPoint>();
			IntPoint intPoint = outPath[0];
			long n = outPath[0].N;
			int num = 0;
			bool flag = true;
			for (int i = 1; i < outPath.Count; i++)
			{
				if (n > outPath[i].N && flag && outPath[i].N != -1L)
				{
					n = outPath[i].N;
					num = i;
					flag = false;
				}
				else if (outPath[i].N >= n)
				{
					n = outPath[i].N;
					flag = true;
				}
			}
			list.AddRange(outPath.GetRange(num, outPath.Count - num));
			list.AddRange(outPath.GetRange(0, num));
			return list;
		}

		private static List<IntPoint> FixPivots(List<IntPoint> outPath, List<IntPoint> inPath, ref int interiorStart)
		{
			List<IntPoint> list = LightUtility.SortPivots(outPath, inPath);
			long n = list[0].N;
			for (int i = 1; i < list.Count; i++)
			{
				int index = (i == list.Count - 1) ? 0 : (i + 1);
				IntPoint intPoint = list[i - 1];
				IntPoint intPoint2 = list[i];
				IntPoint intPoint3 = list[index];
				if (intPoint.N > intPoint2.N && LightUtility.TestPivot(list, i, n))
				{
					if (intPoint.N == intPoint3.N)
					{
						intPoint2.N = intPoint.N;
					}
					else
					{
						intPoint2.N = ((n + 1L < (long)inPath.Count) ? (n + 1L) : 0L);
					}
					intPoint2.D = 3L;
					list[i] = intPoint2;
				}
				n = list[i].N;
			}
			int j = 1;
			while (j < list.Count - 1)
			{
				IntPoint intPoint4 = list[j - 1];
				IntPoint intPoint5 = list[j];
				IntPoint intPoint6 = list[j + 1];
				if (intPoint5.N - intPoint4.N > 1L)
				{
					if (intPoint5.N == intPoint6.N)
					{
						IntPoint intPoint7 = intPoint5;
						intPoint7.N -= 1L;
						list[j] = intPoint7;
					}
					else
					{
						IntPoint intPoint8 = intPoint5;
						intPoint8.N -= 1L;
						list.Insert(j, intPoint8);
					}
				}
				else
				{
					j++;
				}
			}
			return LightUtility.DegeneratePivots(list, inPath, ref interiorStart);
		}

		internal static List<Vector2> GetOutlinePath(Vector3[] shapePath, float offsetDistance)
		{
			List<IntPoint> list = new List<IntPoint>();
			List<Vector2> list2 = new List<Vector2>();
			for (int i = 0; i < shapePath.Length; i++)
			{
				Vector2 vector = new Vector2(shapePath[i].x, shapePath[i].y) * 10000f;
				list.Add(new IntPoint((long)vector.x, (long)vector.y));
			}
			List<List<IntPoint>> list3 = new List<List<IntPoint>>();
			ClipperOffset clipperOffset = new ClipperOffset(24.0);
			clipperOffset.AddPath(list, JoinTypes.jtRound, EndTypes.etClosedPolygon);
			clipperOffset.Execute(ref list3, (double)(10000f * offsetDistance), list.Count);
			if (list3.Count > 0)
			{
				int num = 0;
				List<IntPoint> list4 = list3[0];
				list4 = LightUtility.FixPivots(list4, list, ref num);
				for (int j = 0; j < list4.Count; j++)
				{
					list2.Add(new Vector2((float)list4[j].X / 10000f, (float)list4[j].Y / 10000f));
				}
			}
			return list2;
		}

		private static void TransferToMesh(NativeArray<LightUtility.LightMeshVertex> vertices, int vertexCount, NativeArray<ushort> indices, int indexCount, Light2D light)
		{
			Mesh lightMesh = light.lightMesh;
			lightMesh.SetVertexBufferParams(vertexCount, LightUtility.LightMeshVertex.VertexLayout);
			lightMesh.SetVertexBufferData<LightUtility.LightMeshVertex>(vertices, 0, 0, vertexCount, 0, MeshUpdateFlags.Default);
			lightMesh.SetIndices<ushort>(indices, 0, indexCount, MeshTopology.Triangles, 0, true, 0);
			light.vertices = new LightUtility.LightMeshVertex[vertexCount];
			NativeArray<LightUtility.LightMeshVertex>.Copy(vertices, light.vertices, vertexCount);
			light.indices = new ushort[indexCount];
			NativeArray<ushort>.Copy(indices, light.indices, indexCount);
		}

		public static Bounds GenerateShapeMesh(Light2D light, Vector3[] shapePath, float falloffDistance, float batchColor)
		{
			Random.State state = Random.state;
			Random.InitState(123456);
			Color color = new Color(0f, 0f, batchColor, 1f);
			Color color2 = new Color(0f, 0f, batchColor, 0f);
			int num = shapePath.Length;
			NativeArray<int2> edges = new NativeArray<int2>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float2> points = new NativeArray<float2>(num, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < num; i++)
			{
				int num2 = i + 1;
				if (num2 == num)
				{
					num2 = 0;
				}
				int2 @int = new int2(i, num2);
				edges[i] = @int;
				int x = @int.x;
				points[x] = new float2(shapePath[x].x, shapePath[x].y);
			}
			NativeArray<int> nativeArray = new NativeArray<int>(edges.Length * 8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<float2> nativeArray2 = new NativeArray<float2>(edges.Length * 8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<int2> nativeArray3 = new NativeArray<int2>(edges.Length * 8, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			ModuleHandle.Tessellate(Allocator.Temp, points, edges, ref nativeArray2, ref num3, ref nativeArray, ref num4, ref nativeArray3, ref num5);
			int num6 = shapePath.Length;
			List<IntPoint> list = new List<IntPoint>();
			for (int j = 0; j < num6; j++)
			{
				long num7 = (long)((double)shapePath[j].x * 10000.0);
				long num8 = (long)((double)shapePath[j].y * 10000.0);
				list.Add(new IntPoint(num7 + (long)Random.Range(-10, 10), num8 + (long)Random.Range(-10, 10))
				{
					N = (long)j,
					D = -1L
				});
			}
			int num9 = num6 - 1;
			int num10 = 0;
			List<List<IntPoint>> list2 = new List<List<IntPoint>>();
			ClipperOffset clipperOffset = new ClipperOffset(24.0);
			clipperOffset.AddPath(list, JoinTypes.jtRound, EndTypes.etClosedPolygon);
			clipperOffset.Execute(ref list2, (double)(10000f * falloffDistance), list.Count);
			if (list2.Count > 0)
			{
				List<IntPoint> list3 = list2[0];
				long num11 = (long)num6;
				for (int k = 0; k < list3.Count; k++)
				{
					num11 = ((list3[k].N != -1L) ? Math.Min(num11, list3[k].N) : num11);
				}
				bool flag = num11 == 0L;
				list3 = LightUtility.FixPivots(list3, list, ref num10);
				int length = num3 + list3.Count + num6;
				int length2 = num4 + list3.Count * 6 + 6;
				NativeArray<LightUtility.LightMeshVertex> vertices = new NativeArray<LightUtility.LightMeshVertex>(length, Allocator.Temp, NativeArrayOptions.ClearMemory);
				NativeArray<ushort> indices = new NativeArray<ushort>(length2, Allocator.Temp, NativeArrayOptions.ClearMemory);
				for (int l = 0; l < num4; l++)
				{
					indices[l] = (ushort)nativeArray[l];
				}
				for (int m = 0; m < num3; m++)
				{
					int index = m;
					LightUtility.LightMeshVertex value = new LightUtility.LightMeshVertex
					{
						position = new float3(nativeArray2[m].x, nativeArray2[m].y, 0f),
						color = color
					};
					vertices[index] = value;
				}
				int num12 = num3;
				int num13 = num4;
				ushort[] array = new ushort[num6];
				for (int n = 0; n < num6; n++)
				{
					int index2 = num12++;
					LightUtility.LightMeshVertex value = new LightUtility.LightMeshVertex
					{
						position = new float3(shapePath[n].x, shapePath[n].y, 0f),
						color = color
					};
					vertices[index2] = value;
					array[n] = (ushort)(num12 - 1);
				}
				ushort num14 = (ushort)num12;
				ushort num15 = num14;
				long num16 = (list3[0].N == -1L) ? 0L : list3[0].N;
				for (int num17 = 0; num17 < list3.Count; num17++)
				{
					IntPoint intPoint = list3[num17];
					float2 @float = new float2((float)intPoint.X / 10000f, (float)intPoint.Y / 10000f);
					long num18 = (intPoint.N == -1L) ? 0L : intPoint.N;
					int index3 = num12++;
					LightUtility.LightMeshVertex value = new LightUtility.LightMeshVertex
					{
						position = new float3(@float.x, @float.y, 0f),
						color = ((num10 > num17) ? color2 : color)
					};
					vertices[index3] = value;
					if (num16 != num18)
					{
						indices[num13++] = array[(int)(checked((IntPtr)num16))];
						indices[num13++] = array[(int)(checked((IntPtr)num18))];
						indices[num13++] = (ushort)(num12 - 1);
					}
					indices[num13++] = array[(int)(checked((IntPtr)num16))];
					indices[num13++] = num14;
					num14 = (indices[num13++] = (ushort)(num12 - 1));
					num16 = num18;
				}
				indices[num13++] = num15;
				indices[num13++] = array[(int)(checked((IntPtr)num11))];
				indices[num13++] = (flag ? array[num9] : num14);
				indices[num13++] = (flag ? num15 : num14);
				indices[num13++] = (flag ? num14 : array[(int)(checked((IntPtr)num11))]);
				if (flag)
				{
					float num19 = 0.001f;
					ushort num20 = array[num9];
					bool flag2 = MathF.Abs(vertices[(int)num20].position.x - vertices[(int)indices[num13 - 1]].position.x) > num19 || MathF.Abs(vertices[(int)num20].position.y - vertices[(int)indices[num13 - 1]].position.y) > num19;
					bool flag3 = MathF.Abs(vertices[(int)num20].position.x - vertices[(int)indices[num13 - 2]].position.x) > num19 || MathF.Abs(vertices[(int)num20].position.y - vertices[(int)indices[num13 - 2]].position.y) > num19;
					if (!flag2 || !flag3)
					{
						num20 = (ushort)(num10 + num6 + num3 - 1);
					}
					indices[num13++] = num20;
				}
				else
				{
					indices[num13++] = array[(int)(checked((IntPtr)(unchecked(num11 - 1L))))];
				}
				LightUtility.TransferToMesh(vertices, num12, indices, num13, light);
			}
			Random.state = state;
			return light.lightMesh.GetSubMesh(0).bounds;
		}

		public static Bounds GenerateParametricMesh(Light2D light, float radius, float falloffDistance, float angle, int sides, float batchColor)
		{
			float num = 1.5707964f + 0.017453292f * angle;
			if (sides < 3)
			{
				radius = 0.70710677f * radius;
				sides = 4;
			}
			if (sides == 4)
			{
				num = 0.7853982f + 0.017453292f * angle;
			}
			int num2 = 1 + 2 * sides;
			int num3 = 9 * sides;
			NativeArray<LightUtility.LightMeshVertex> nativeArray = new NativeArray<LightUtility.LightMeshVertex>(num2, Allocator.Temp, NativeArrayOptions.ClearMemory);
			NativeArray<ushort> nativeArray2 = new NativeArray<ushort>(num3, Allocator.Temp, NativeArrayOptions.ClearMemory);
			ushort num4 = (ushort)(2 * sides);
			Mesh lightMesh = light.lightMesh;
			Color color = new Color(0f, 0f, batchColor, 1f);
			nativeArray[(int)num4] = new LightUtility.LightMeshVertex
			{
				position = float3.zero,
				color = color
			};
			float num5 = 6.2831855f / (float)sides;
			float3 @float = new float3(float.MaxValue, float.MaxValue, 0f);
			float3 float2 = new float3(float.MinValue, float.MinValue, 0f);
			for (int i = 0; i < sides; i++)
			{
				float num6 = (float)(i + 1) * num5;
				float3 float3 = new float3(math.cos(num6 + num), math.sin(num6 + num), 0f);
				float3 float4 = radius * float3;
				int num7 = (2 * i + 2) % (2 * sides);
				nativeArray[num7] = new LightUtility.LightMeshVertex
				{
					position = float4,
					color = new Color(float3.x, float3.y, batchColor, 0f)
				};
				nativeArray[num7 + 1] = new LightUtility.LightMeshVertex
				{
					position = float4,
					color = color
				};
				int num8 = 9 * i;
				nativeArray2[num8] = (ushort)(num7 + 1);
				nativeArray2[num8 + 1] = (ushort)(2 * i + 1);
				nativeArray2[num8 + 2] = num4;
				nativeArray2[num8 + 3] = (ushort)num7;
				nativeArray2[num8 + 4] = (ushort)(2 * i);
				nativeArray2[num8 + 5] = (ushort)(2 * i + 1);
				nativeArray2[num8 + 6] = (ushort)(num7 + 1);
				nativeArray2[num8 + 7] = (ushort)num7;
				nativeArray2[num8 + 8] = (ushort)(2 * i + 1);
				@float = math.min(@float, float4 + float3 * falloffDistance);
				float2 = math.max(float2, float4 + float3 * falloffDistance);
			}
			lightMesh.SetVertexBufferParams(num2, LightUtility.LightMeshVertex.VertexLayout);
			lightMesh.SetVertexBufferData<LightUtility.LightMeshVertex>(nativeArray, 0, 0, num2, 0, MeshUpdateFlags.Default);
			lightMesh.SetIndices<ushort>(nativeArray2, MeshTopology.Triangles, 0, false, 0);
			light.vertices = new LightUtility.LightMeshVertex[num2];
			NativeArray<LightUtility.LightMeshVertex>.Copy(nativeArray, light.vertices, num2);
			light.indices = new ushort[num3];
			NativeArray<ushort>.Copy(nativeArray2, light.indices, num3);
			return new Bounds
			{
				min = @float,
				max = float2
			};
		}

		public static Bounds GenerateSpriteMesh(Light2D light, Sprite sprite, float batchColor)
		{
			Mesh lightMesh = light.lightMesh;
			if (sprite == null)
			{
				lightMesh.Clear();
				return new Bounds(Vector3.zero, Vector3.zero);
			}
			Vector2[] uv = sprite.uv;
			NativeSlice<Vector3> vertexAttribute = sprite.GetVertexAttribute(VertexAttribute.Position);
			NativeSlice<Vector2> vertexAttribute2 = sprite.GetVertexAttribute(VertexAttribute.TexCoord0);
			NativeArray<ushort> indices = sprite.GetIndices();
			0.5f * (sprite.bounds.min + sprite.bounds.max);
			NativeArray<LightUtility.LightMeshVertex> nativeArray = new NativeArray<LightUtility.LightMeshVertex>(indices.Length, Allocator.Temp, NativeArrayOptions.ClearMemory);
			Color color = new Color(0f, 0f, batchColor, 1f);
			for (int i = 0; i < vertexAttribute.Length; i++)
			{
				nativeArray[i] = new LightUtility.LightMeshVertex
				{
					position = new Vector3(vertexAttribute[i].x, vertexAttribute[i].y, 0f),
					color = color,
					uv = vertexAttribute2[i]
				};
			}
			lightMesh.SetVertexBufferParams(nativeArray.Length, LightUtility.LightMeshVertex.VertexLayout);
			lightMesh.SetVertexBufferData<LightUtility.LightMeshVertex>(nativeArray, 0, 0, nativeArray.Length, 0, MeshUpdateFlags.Default);
			lightMesh.SetIndices<ushort>(indices, MeshTopology.Triangles, 0, true, 0);
			light.vertices = new LightUtility.LightMeshVertex[nativeArray.Length];
			NativeArray<LightUtility.LightMeshVertex>.Copy(nativeArray, light.vertices, nativeArray.Length);
			light.indices = new ushort[indices.Length];
			NativeArray<ushort>.Copy(indices, light.indices, indices.Length);
			return lightMesh.GetSubMesh(0).bounds;
		}

		public static int GetShapePathHash(Vector3[] path)
		{
			int num = -2128831035;
			if (path != null)
			{
				foreach (Vector3 vector in path)
				{
					num = (num * 16777619 ^ vector.GetHashCode());
				}
			}
			else
			{
				num = 0;
			}
			return num;
		}

		private enum PivotType
		{
			PivotBase,
			PivotCurve,
			PivotIntersect,
			PivotSkip,
			PivotClip
		}

		[Serializable]
		internal struct LightMeshVertex
		{
			public Vector3 position;

			public Color color;

			public Vector2 uv;

			public static readonly VertexAttributeDescriptor[] VertexLayout = new VertexAttributeDescriptor[]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
				new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4, 0),
				new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0)
			};
		}
	}
}
