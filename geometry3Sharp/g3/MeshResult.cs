using System;

namespace g3
{
	public enum MeshResult
	{
		Ok,
		Failed_NotAVertex,
		Failed_NotATriangle,
		Failed_NotAnEdge,
		Failed_BrokenTopology = 10,
		Failed_HitValenceLimit,
		Failed_IsBoundaryEdge = 20,
		Failed_FlippedEdgeExists,
		Failed_IsBowtieVertex,
		Failed_InvalidNeighbourhood,
		Failed_FoundDuplicateTriangle,
		Failed_CollapseTetrahedron,
		Failed_CollapseTriangle,
		Failed_NotABoundaryEdge,
		Failed_SameOrientation,
		Failed_WouldCreateBowtie = 30,
		Failed_VertexAlreadyExists,
		Failed_CannotAllocateVertex,
		Failed_WouldCreateNonmanifoldEdge = 50,
		Failed_TriangleAlreadyExists,
		Failed_CannotAllocateTriangle
	}
}
