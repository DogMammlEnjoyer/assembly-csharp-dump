using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Splines.ExtrusionShapes;

namespace UnityEngine.Splines
{
	public static class SplineMesh
	{
		private static void ExtrudeRing<TSpline, TShape, TVertex>(TSpline spline, ExtrudeSettings<TShape> settings, int segment, NativeArray<TVertex> data, int start, bool uvsAreCaps = false) where TSpline : ISpline where TShape : IExtrudeShape where TVertex : struct, SplineMesh.ISplineVertexData
		{
			TShape shape = settings.Shape;
			int sides = settings.sides;
			float radius = settings.Radius;
			bool wrapped = settings.wrapped;
			float num = math.lerp(settings.Range.x, settings.Range.y, (float)segment / ((float)settings.SegmentCount - 1f));
			float num2 = spline.Closed ? math.frac(num) : math.clamp(num, 0f, 1f);
			float3 @float;
			float3 float2;
			float3 up;
			spline.Evaluate(num2, out @float, out float2, out up);
			float num3 = math.lengthsq(float2);
			if (num3 == 0f || float.IsNaN(num3))
			{
				float t = math.clamp(num2 + 0.0001f * ((num < 1f) ? 1f : -1f), 0f, 1f);
				float3 float3;
				spline.Evaluate(t, out float3, out float2, out up);
			}
			float2 = math.normalize(float2);
			quaternion q = quaternion.LookRotationSafe(float2, up);
			shape.SetSegment(segment, num, @float, float2, up);
			bool flipNormals = settings.FlipNormals;
			for (int i = 0; i < sides; i++)
			{
				TVertex value = Activator.CreateInstance<TVertex>();
				int num4 = flipNormals ? (sides - i - 1) : i;
				float t2 = (float)num4 / ((float)sides - 1f);
				float2 xy = shape.GetPosition(t2, num4) * radius;
				value.position = @float + math.rotate(q, new float3(xy, 0f));
				value.normal = (value.position - @float).normalized * (flipNormals ? -1f : 1f);
				if (uvsAreCaps)
				{
					value.texture = xy.xy / radius / 2f;
				}
				else if (wrapped)
				{
					float num5 = (float)num4 / ((float)sides + (float)(sides % 2));
					float num6 = math.abs(num5 - math.floor(num5 + 0.5f)) * 2f;
					value.texture = new Vector2(1f - num6, num * spline.GetLength());
				}
				else
				{
					value.texture = new Vector2(1f - (float)num4 / ((float)sides - 1f), num * spline.GetLength());
				}
				data[start + i] = value;
			}
			if (SplineMesh.s_IsConvexComputed)
			{
				return;
			}
			SplineMesh.ComputeIsConvex<TVertex>(data, float2, start, sides);
		}

		private static void ComputeIsConvex<TVertex>(NativeArray<TVertex> data, float3 normal, int start, int sideCount) where TVertex : struct, SplineMesh.ISplineVertexData
		{
			SplineMesh.s_IsConvexComputed = true;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < sideCount; i++)
			{
				int num = start + i;
				int num2 = (num + 1) % (sideCount - 1);
				int index = (num2 + 1) % (sideCount - 1);
				TVertex tvertex = data[num];
				Vector3 position = tvertex.position;
				tvertex = data[num2];
				Vector3 position2 = tvertex.position;
				tvertex = data[index];
				Vector3 position3 = tvertex.position;
				Vector3 v = position2 - position;
				Vector3 v2 = position3 - position2;
				float3 y = math.normalizesafe(math.cross(v, v2), default(float3));
				float num3 = math.dot(normal, y);
				if (num3 < 0f)
				{
					flag = true;
				}
				else if (num3 > 0f)
				{
					flag2 = true;
				}
				if (flag && flag2)
				{
					SplineMesh.s_IsConvex = false;
					return;
				}
			}
			SplineMesh.s_IsConvex = true;
		}

		public static bool GetVertexAndIndexCount(int sides, int segments, bool capped, bool closed, bool closeRing, out int vertexCount, out int indexCount)
		{
			vertexCount = sides * (segments + (capped ? 2 : 0));
			indexCount = (closeRing ? sides : (sides - 1)) * 6 * (segments - (closed ? 0 : 1)) + (capped ? ((sides - 2) * 3 * 2) : 0);
			return vertexCount > 3 && indexCount > 5;
		}

		public static void GetVertexAndIndexCount(int sides, int segments, bool capped, bool closed, Vector2 range, out int vertexCount, out int indexCount)
		{
			SplineMesh.GetVertexAndIndexCount(sides, segments, capped, closed, true, out vertexCount, out indexCount);
		}

		private static bool GetVertexAndIndexCount<T, K>(T spline, ExtrudeSettings<K> settings, out int vertexCount, out int indexCount) where T : ISpline where K : IExtrudeShape
		{
			return SplineMesh.GetVertexAndIndexCount(settings.sides, settings.SegmentCount, settings.DoCapEnds<T>(spline), settings.DoCloseSpline<T>(spline), settings.wrapped, out vertexCount, out indexCount);
		}

		public static void Extrude<T>(T spline, Mesh mesh, float radius, int sides, int segments, bool capped = true) where T : ISpline
		{
			SplineMesh.Extrude<T>(spline, mesh, radius, sides, segments, capped, new float2(0f, 1f));
		}

		public static void Extrude<T, K>(T spline, Mesh mesh, float radius, int segments, bool capped, K shape) where T : ISpline where K : IExtrudeShape
		{
			SplineMesh.Extrude<T, K>(spline, mesh, radius, segments, capped, new float2(0f, 1f), shape);
		}

		public static void Extrude<T>(T spline, Mesh mesh, float radius, int sides, int segments, bool capped, float2 range) where T : ISpline
		{
			SplineMesh.s_DefaultShape.SideCount = sides;
			SplineMesh.Extrude<T, Circle>(spline, mesh, radius, segments, capped, range, SplineMesh.s_DefaultShape);
		}

		public static void Extrude<T, K>(T spline, Mesh mesh, float radius, int segments, bool capped, float2 range, K shape) where T : ISpline where K : IExtrudeShape
		{
			ExtrudeSettings<K> settings = new ExtrudeSettings<K>
			{
				Radius = radius,
				CapEnds = capped,
				Range = range,
				SegmentCount = segments,
				Shape = shape
			};
			SplineMesh.Extrude<T, K>(spline, mesh, settings);
		}

		public static bool Extrude<T, K>(T spline, Mesh mesh, ExtrudeSettings<K> settings) where T : ISpline where K : IExtrudeShape
		{
			int num;
			int indexCount;
			if (!SplineMesh.GetVertexAndIndexCount<T, K>(spline, settings, out num, out indexCount))
			{
				return false;
			}
			Mesh.MeshDataArray data = Mesh.AllocateWritableMeshData(1);
			Mesh.MeshData meshData = data[0];
			IndexFormat indexFormat = (num >= 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;
			meshData.SetIndexBufferParams(indexCount, indexFormat);
			meshData.SetVertexBufferParams(num, SplineMesh.k_PipeVertexAttribs);
			NativeArray<SplineMesh.VertexData> vertexData = meshData.GetVertexData<SplineMesh.VertexData>(0);
			if (indexFormat == IndexFormat.UInt16)
			{
				NativeArray<ushort> indexData = meshData.GetIndexData<ushort>();
				SplineMesh.Extrude<T, SplineMesh.VertexData, ushort, K>(spline, vertexData, indexData, settings, 0, 0);
			}
			else
			{
				NativeArray<uint> indexData2 = meshData.GetIndexData<uint>();
				SplineMesh.Extrude<T, SplineMesh.VertexData, uint, K>(spline, vertexData, indexData2, settings, 0, 0);
			}
			mesh.Clear();
			meshData.subMeshCount = 1;
			meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount, MeshTopology.Triangles), MeshUpdateFlags.Default);
			Mesh.ApplyAndDisposeWritableMeshData(data, mesh, MeshUpdateFlags.Default);
			mesh.RecalculateBounds();
			return true;
		}

		public static void Extrude<T>(IReadOnlyList<T> splines, Mesh mesh, float radius, int sides, float segmentsPerUnit, bool capped, float2 range) where T : ISpline
		{
			SplineMesh.s_DefaultShape.SideCount = sides;
			ExtrudeSettings<Circle> settings = new ExtrudeSettings<Circle>(SplineMesh.s_DefaultShape)
			{
				Radius = radius,
				SegmentCount = (int)segmentsPerUnit,
				CapEnds = capped,
				Range = range
			};
			SplineMesh.Extrude<T, Circle>(splines, mesh, settings, segmentsPerUnit);
		}

		internal static void Extrude<T, K>(IReadOnlyList<T> splines, Mesh mesh, ExtrudeSettings<K> settings, float segmentsPerUnit) where T : ISpline where K : IExtrudeShape
		{
			SplineMesh.<>c__DisplayClass19_0<T, K> CS$<>8__locals1;
			CS$<>8__locals1.settings = settings;
			CS$<>8__locals1.segmentsPerUnit = segmentsPerUnit;
			mesh.Clear();
			if (splines == null)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("Trying to extrude a spline mesh with no valid splines.");
				}
				return;
			}
			Mesh.MeshDataArray data = Mesh.AllocateWritableMeshData(1);
			Mesh.MeshData meshData = data[0];
			meshData.subMeshCount = 1;
			int num = 0;
			int num2 = 0;
			ValueTuple<int, int>[] array = new ValueTuple<int, int>[splines.Count];
			for (int i = 0; i < splines.Count; i++)
			{
				T t = splines[i];
				if (t.Count >= 2)
				{
					CS$<>8__locals1.settings.SegmentCount = SplineMesh.<Extrude>g__GetSegmentCount|19_0<T, K>(splines[i], ref CS$<>8__locals1);
					int num3;
					int num4;
					SplineMesh.GetVertexAndIndexCount<T, K>(splines[i], CS$<>8__locals1.settings, out num3, out num4);
					array[i] = new ValueTuple<int, int>(num2, num);
					num += num3;
					num2 += num4;
				}
			}
			IndexFormat indexFormat = (num >= 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16;
			meshData.SetIndexBufferParams(num2, indexFormat);
			meshData.SetVertexBufferParams(num, SplineMesh.k_PipeVertexAttribs);
			NativeArray<SplineMesh.VertexData> vertexData = meshData.GetVertexData<SplineMesh.VertexData>(0);
			if (indexFormat == IndexFormat.UInt16)
			{
				NativeArray<ushort> indexData = meshData.GetIndexData<ushort>();
				for (int j = 0; j < splines.Count; j++)
				{
					T t = splines[j];
					if (t.Count >= 2)
					{
						CS$<>8__locals1.settings.SegmentCount = SplineMesh.<Extrude>g__GetSegmentCount|19_0<T, K>(splines[j], ref CS$<>8__locals1);
						SplineMesh.Extrude<T, SplineMesh.VertexData, ushort, K>(splines[j], vertexData, indexData, CS$<>8__locals1.settings, array[j].Item2, array[j].Item1);
					}
				}
			}
			else
			{
				NativeArray<uint> indexData2 = meshData.GetIndexData<uint>();
				for (int k = 0; k < splines.Count; k++)
				{
					T t = splines[k];
					if (t.Count >= 2)
					{
						CS$<>8__locals1.settings.SegmentCount = SplineMesh.<Extrude>g__GetSegmentCount|19_0<T, K>(splines[k], ref CS$<>8__locals1);
						SplineMesh.Extrude<T, SplineMesh.VertexData, uint, K>(splines[k], vertexData, indexData2, CS$<>8__locals1.settings, array[k].Item2, array[k].Item1);
					}
				}
			}
			meshData.SetSubMesh(0, new SubMeshDescriptor(0, num2, MeshTopology.Triangles), MeshUpdateFlags.Default);
			Mesh.ApplyAndDisposeWritableMeshData(data, mesh, MeshUpdateFlags.Default);
			mesh.RecalculateBounds();
		}

		public static void Extrude<TSplineType, TVertexType, TIndexType>(TSplineType spline, NativeArray<TVertexType> vertices, NativeArray<TIndexType> indices, float radius, int sides, int segments, bool capped, float2 range) where TSplineType : ISpline where TVertexType : struct, SplineMesh.ISplineVertexData where TIndexType : struct
		{
			SplineMesh.s_DefaultShape.SideCount = math.clamp(sides, 2, 2084);
			SplineMesh.Extrude<TSplineType, TVertexType, TIndexType, Circle>(spline, vertices, indices, new ExtrudeSettings<Circle>(segments, capped, range, radius, SplineMesh.s_DefaultShape), 0, 0);
		}

		private static void Extrude<TSplineType, TVertexType, TIndexType, TShapeType>(TSplineType spline, NativeArray<TVertexType> vertices, NativeArray<TIndexType> indices, ExtrudeSettings<TShapeType> settings, int vertexArrayOffset = 0, int indicesArrayOffset = 0) where TSplineType : ISpline where TVertexType : struct, SplineMesh.ISplineVertexData where TIndexType : struct where TShapeType : IExtrudeShape
		{
			int sides = settings.sides;
			int segmentCount = settings.SegmentCount;
			float2 range = settings.Range;
			bool flag = settings.DoCapEnds<TSplineType>(spline);
			int num;
			int num2;
			if (!SplineMesh.GetVertexAndIndexCount<TSplineType, TShapeType>(spline, settings, out num, out num2))
			{
				return;
			}
			if (settings.Shape == null)
			{
				throw new ArgumentNullException("Shape", "Shape template is null.");
			}
			if (sides < 2)
			{
				throw new ArgumentOutOfRangeException("sides", "Sides must be greater than 2");
			}
			if (segmentCount < 2)
			{
				throw new ArgumentOutOfRangeException("segments", "Segments must be greater than 2");
			}
			if (vertices.Length < num)
			{
				throw new ArgumentOutOfRangeException(string.Format("Vertex array is incorrect size. Expected {0} or more, but received {1}.", num, vertices.Length));
			}
			if (indices.Length < num2)
			{
				throw new ArgumentOutOfRangeException(string.Format("Index array is incorrect size. Expected {0} or more, but received {1}.", num2, indices.Length));
			}
			if (typeof(TIndexType) == typeof(ushort))
			{
				SplineMesh.WindTris<TSplineType, TShapeType>(indices.Reinterpret<ushort>(), spline, settings, vertexArrayOffset, indicesArrayOffset);
			}
			else
			{
				if (!(typeof(TIndexType) == typeof(uint)))
				{
					throw new ArgumentException("Indices must be UInt16 or UInt32", "indices");
				}
				SplineMesh.WindTris<TSplineType, TShapeType>(indices.Reinterpret<uint>(), spline, settings, vertexArrayOffset, indicesArrayOffset);
			}
			TShapeType shape = settings.Shape;
			shape.Setup(spline, segmentCount);
			SplineMesh.s_IsConvexComputed = false;
			for (int i = 0; i < segmentCount; i++)
			{
				SplineMesh.ExtrudeRing<TSplineType, TShapeType, TVertexType>(spline, settings, i, vertices, vertexArrayOffset + i * sides, false);
			}
			if (flag)
			{
				int num3 = vertexArrayOffset + segmentCount * sides;
				int num4 = vertexArrayOffset + (segmentCount + 1) * sides;
				float2 @float = spline.Closed ? math.frac(range) : math.clamp(range, 0f, 1f);
				SplineMesh.ExtrudeRing<TSplineType, TShapeType, TVertexType>(spline, settings, 0, vertices, num3, true);
				SplineMesh.ExtrudeRing<TSplineType, TShapeType, TVertexType>(spline, settings, segmentCount - 1, vertices, num4, true);
				float3 float2 = math.normalize(spline.EvaluateTangent(@float.x));
				float num5 = math.lengthsq(float2);
				if (num5 == 0f || float.IsNaN(num5))
				{
					float2 = math.normalize(spline.EvaluateTangent(@float.x + 0.0001f));
				}
				float3 float3 = math.normalize(spline.EvaluateTangent(@float.y));
				num5 = math.lengthsq(float3);
				if (num5 == 0f || float.IsNaN(num5))
				{
					float3 = math.normalize(spline.EvaluateTangent(@float.y - 0.0001f));
				}
				for (int j = 0; j < sides; j++)
				{
					TVertexType value = vertices[num3 + j];
					TVertexType value2 = vertices[num4 + j];
					value.normal = -float2;
					value2.normal = float3;
					vertices[num3 + j] = value;
					vertices[num4 + j] = value2;
				}
			}
		}

		private static void WindTris<T, K>(NativeArray<ushort> indices, T spline, ExtrudeSettings<K> settings, int vertexArrayOffset = 0, int indexArrayOffset = 0) where T : ISpline where K : IExtrudeShape
		{
			bool flag = settings.DoCloseSpline<T>(spline);
			int segmentCount = settings.SegmentCount;
			int sides = settings.sides;
			bool wrapped = settings.wrapped;
			bool flag2 = settings.DoCapEnds<T>(spline);
			int num = wrapped ? sides : (sides - 1);
			for (int i = 0; i < (flag ? segmentCount : (segmentCount - 1)); i++)
			{
				for (int j = 0; j < (wrapped ? sides : (sides - 1)); j++)
				{
					int num2 = vertexArrayOffset + i * sides + j;
					int num3 = vertexArrayOffset + i * sides + (j + 1) % sides;
					int num4 = vertexArrayOffset + (i + 1) % segmentCount * sides + j;
					int num5 = vertexArrayOffset + (i + 1) % segmentCount * sides + (j + 1) % sides;
					indices[indexArrayOffset + i * num * 6 + j * 6] = (ushort)num2;
					indices[indexArrayOffset + i * num * 6 + j * 6 + 1] = (ushort)num3;
					indices[indexArrayOffset + i * num * 6 + j * 6 + 2] = (ushort)num4;
					indices[indexArrayOffset + i * num * 6 + j * 6 + 3] = (ushort)num3;
					indices[indexArrayOffset + i * num * 6 + j * 6 + 4] = (ushort)num5;
					indices[indexArrayOffset + i * num * 6 + j * 6 + 5] = (ushort)num4;
				}
			}
			if (flag2)
			{
				int num6 = vertexArrayOffset + segmentCount * sides;
				int num7 = indexArrayOffset + num * 6 * (segmentCount - 1);
				int num8 = vertexArrayOffset + (segmentCount + 1) * sides;
				int num9 = indexArrayOffset + (segmentCount - 1) * 6 * num + (sides - 2) * 3;
				ushort num10 = 0;
				while ((int)num10 < sides - 2)
				{
					indices[num7 + (int)(num10 * 3)] = (ushort)num6;
					indices[num7 + (int)(num10 * 3) + 1] = (ushort)(num6 + (int)num10 + 2);
					indices[num7 + (int)(num10 * 3) + 2] = (ushort)(num6 + (int)num10 + 1);
					indices[num9 + (int)(num10 * 3)] = (ushort)num8;
					indices[num9 + (int)(num10 * 3) + 1] = (ushort)(num8 + (int)num10 + 1);
					indices[num9 + (int)(num10 * 3) + 2] = (ushort)(num8 + (int)num10 + 2);
					num10 += 1;
				}
			}
		}

		private static void WindTris<T, K>(NativeArray<uint> indices, T spline, ExtrudeSettings<K> settings, int vertexArrayOffset = 0, int indexArrayOffset = 0) where T : ISpline where K : IExtrudeShape
		{
			bool flag = settings.DoCloseSpline<T>(spline);
			int segmentCount = settings.SegmentCount;
			int sides = settings.sides;
			bool wrapped = settings.wrapped;
			bool flag2 = settings.DoCapEnds<T>(spline);
			int num = wrapped ? sides : (sides - 1);
			for (int i = 0; i < (flag ? segmentCount : (segmentCount - 1)); i++)
			{
				for (int j = 0; j < (wrapped ? sides : (sides - 1)); j++)
				{
					int num2 = vertexArrayOffset + i * sides + j;
					int num3 = vertexArrayOffset + i * sides + (j + 1) % sides;
					int num4 = vertexArrayOffset + (i + 1) % segmentCount * sides + j;
					int num5 = vertexArrayOffset + (i + 1) % segmentCount * sides + (j + 1) % sides;
					indices[indexArrayOffset + i * num * 6 + j * 6] = (uint)((ushort)num2);
					indices[indexArrayOffset + i * num * 6 + j * 6 + 1] = (uint)((ushort)num3);
					indices[indexArrayOffset + i * num * 6 + j * 6 + 2] = (uint)((ushort)num4);
					indices[indexArrayOffset + i * num * 6 + j * 6 + 3] = (uint)((ushort)num3);
					indices[indexArrayOffset + i * num * 6 + j * 6 + 4] = (uint)((ushort)num5);
					indices[indexArrayOffset + i * num * 6 + j * 6 + 5] = (uint)((ushort)num4);
				}
			}
			if (flag2)
			{
				int num6 = vertexArrayOffset + segmentCount * sides;
				int num7 = indexArrayOffset + num * 6 * (segmentCount - 1);
				int num8 = vertexArrayOffset + (segmentCount + 1) * sides;
				int num9 = indexArrayOffset + (segmentCount - 1) * 6 * num + (sides - 2) * 3;
				ushort num10 = 0;
				while ((int)num10 < sides - 2)
				{
					indices[num7 + (int)(num10 * 3)] = (uint)((ushort)num6);
					indices[num7 + (int)(num10 * 3) + 1] = (uint)((ushort)(num6 + (int)num10 + 2));
					indices[num7 + (int)(num10 * 3) + 2] = (uint)((ushort)(num6 + (int)num10 + 1));
					indices[num9 + (int)(num10 * 3)] = (uint)((ushort)num8);
					indices[num9 + (int)(num10 * 3) + 1] = (uint)((ushort)(num8 + (int)num10 + 1));
					indices[num9 + (int)(num10 * 3) + 2] = (uint)((ushort)(num8 + (int)num10 + 2));
					num10 += 1;
				}
			}
		}

		[CompilerGenerated]
		internal static int <Extrude>g__GetSegmentCount|19_0<T, K>(T spline, ref SplineMesh.<>c__DisplayClass19_0<T, K> A_1) where T : ISpline where K : IExtrudeShape
		{
			float num = Mathf.Abs(A_1.settings.Range.y - A_1.settings.Range.x);
			return Mathf.Max((int)Mathf.Ceil(spline.GetLength() * num * A_1.segmentsPerUnit), 1);
		}

		private const int k_SidesMin = 2;

		private const int k_SidesMax = 2084;

		private static readonly VertexAttributeDescriptor[] k_PipeVertexAttribs = new VertexAttributeDescriptor[]
		{
			new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
			new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0)
		};

		private static readonly Circle s_DefaultShape = new Circle();

		internal static bool s_IsConvex;

		private static bool s_IsConvexComputed;

		public interface ISplineVertexData
		{
			Vector3 position { get; set; }

			Vector3 normal { get; set; }

			Vector2 texture { get; set; }
		}

		private struct VertexData : SplineMesh.ISplineVertexData
		{
			public Vector3 position { readonly get; set; }

			public Vector3 normal { readonly get; set; }

			public Vector2 texture { readonly get; set; }
		}
	}
}
