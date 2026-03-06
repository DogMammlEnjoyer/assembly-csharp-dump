using System;
using System.Collections.Generic;

namespace g3
{
	public interface IMesh : IPointSet
	{
		int TriangleCount { get; }

		int MaxTriangleID { get; }

		bool HasVertexUVs { get; }

		Vector2f GetVertexUV(int i);

		NewVertexInfo GetVertexAll(int i);

		bool HasTriangleGroups { get; }

		Index3i GetTriangle(int i);

		int GetTriangleGroup(int i);

		bool IsTriangle(int tID);

		IEnumerable<int> TriangleIndices();
	}
}
