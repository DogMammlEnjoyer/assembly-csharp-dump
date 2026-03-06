using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	[Feature(Feature.Scene)]
	public static class Triangulator
	{
		public unsafe static void TriangulatePoints(List<Vector2> vertices, List<List<Vector2>> holes, out Vector2[] outVertices, out int[] outIndices)
		{
			int num = (holes != null) ? (holes.Count + 1) : 1;
			MRUKNativeFuncs.MrukPolygon2f[] array = new MRUKNativeFuncs.MrukPolygon2f[num];
			array[0].numPoints = (uint)vertices.Count;
			array[0].points = vertices.ToArray();
			if (holes != null)
			{
				for (int i = 0; i < holes.Count; i++)
				{
					array[i + 1].numPoints = (uint)holes[i].Count;
					array[i + 1].points = holes[i].ToArray();
				}
			}
			using (Mesh2fDisposer mesh2fDisposer = new Mesh2fDisposer(MRUKNativeFuncs.TriangulatePolygon(array, (uint)num)))
			{
				outVertices = new Vector2[mesh2fDisposer.Mesh.numVertices];
				for (uint num2 = 0U; num2 < mesh2fDisposer.Mesh.numVertices; num2 += 1U)
				{
					outVertices[(int)num2] = mesh2fDisposer.Mesh.vertices[(ulong)num2 * (ulong)((long)sizeof(Vector2)) / (ulong)sizeof(Vector2)];
				}
				outIndices = new int[mesh2fDisposer.Mesh.numIndices];
				for (uint num3 = 0U; num3 < mesh2fDisposer.Mesh.numIndices; num3 += 1U)
				{
					outIndices[(int)num3] = (int)mesh2fDisposer.Mesh.indices[(ulong)num3 * 4UL / 4UL];
				}
			}
		}
	}
}
