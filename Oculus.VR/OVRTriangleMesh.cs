using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

public readonly struct OVRTriangleMesh : IOVRAnchorComponent<OVRTriangleMesh>, IEquatable<OVRTriangleMesh>
{
	OVRPlugin.SpaceComponentType IOVRAnchorComponent<OVRTriangleMesh>.Type
	{
		get
		{
			return this.Type;
		}
	}

	ulong IOVRAnchorComponent<OVRTriangleMesh>.Handle
	{
		get
		{
			return this.Handle;
		}
	}

	OVRTriangleMesh IOVRAnchorComponent<OVRTriangleMesh>.FromAnchor(OVRAnchor anchor)
	{
		return new OVRTriangleMesh(anchor);
	}

	public bool IsNull
	{
		get
		{
			return this.Handle == 0UL;
		}
	}

	public bool IsEnabled
	{
		get
		{
			bool flag;
			bool flag2;
			return !this.IsNull && OVRPlugin.GetSpaceComponentStatus(this.Handle, this.Type, out flag, out flag2) && flag && !flag2;
		}
	}

	OVRTask<bool> IOVRAnchorComponent<OVRTriangleMesh>.SetEnabledAsync(bool enabled, double timeout)
	{
		throw new NotSupportedException("The TriangleMesh component cannot be enabled or disabled.");
	}

	public bool Equals(OVRTriangleMesh other)
	{
		return this.Handle == other.Handle;
	}

	public static bool operator ==(OVRTriangleMesh lhs, OVRTriangleMesh rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(OVRTriangleMesh lhs, OVRTriangleMesh rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is OVRTriangleMesh)
		{
			OVRTriangleMesh other = (OVRTriangleMesh)obj;
			return this.Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return this.Handle.GetHashCode() * 486187739 + ((int)this.Type).GetHashCode();
	}

	public override string ToString()
	{
		return string.Format("{0}.TriangleMesh", this.Handle);
	}

	internal OVRPlugin.SpaceComponentType Type
	{
		get
		{
			return OVRPlugin.SpaceComponentType.TriangleMesh;
		}
	}

	internal ulong Handle { get; }

	private OVRTriangleMesh(OVRAnchor anchor)
	{
		this.Handle = anchor.Handle;
	}

	public bool TryGetCounts(out int vertexCount, out int triangleCount)
	{
		return OVRPlugin.GetSpaceTriangleMeshCounts(this.Handle, out vertexCount, out triangleCount);
	}

	public bool TryGetMeshRawUntransformed(NativeArray<Vector3> positions, NativeArray<int> indices)
	{
		return OVRPlugin.GetSpaceTriangleMesh(this.Handle, positions, indices);
	}

	public bool TryGetMesh(NativeArray<Vector3> positions, NativeArray<int> indices)
	{
		if (!this.TryGetMeshRawUntransformed(positions, indices))
		{
			return false;
		}
		for (int i = 0; i < positions.Length; i++)
		{
			Vector3 vector = positions[i];
			positions[i] = new Vector3(-vector.x, vector.y, vector.z);
		}
		NativeArray<OVRTriangleMesh.Triangle> nativeArray = indices.Reinterpret<OVRTriangleMesh.Triangle>(4);
		for (int j = 0; j < nativeArray.Length; j++)
		{
			OVRTriangleMesh.Triangle triangle = nativeArray[j];
			nativeArray[j] = new OVRTriangleMesh.Triangle
			{
				A = triangle.A,
				B = triangle.C,
				C = triangle.B
			};
		}
		return true;
	}

	public JobHandle ScheduleGetMeshJob(NativeArray<Vector3> positions, NativeArray<int> indices, JobHandle dependencies = default(JobHandle))
	{
		JobHandle dependsOn = new OVRTriangleMesh.GetMeshJob
		{
			Positions = positions,
			Indices = indices,
			Space = this.Handle
		}.Schedule(dependencies);
		NativeArray<OVRTriangleMesh.Triangle> triangles = indices.Reinterpret<OVRTriangleMesh.Triangle>(4);
		return JobHandle.CombineDependencies(new OVRTriangleMesh.NegateXJob
		{
			Positions = positions
		}.Schedule(positions.Length, 32, dependsOn), new OVRTriangleMesh.FlipTriangleWindingJob
		{
			Triangles = triangles
		}.Schedule(triangles.Length, 32, dependsOn));
	}

	public static readonly OVRTriangleMesh Null;

	private struct GetMeshJob : IJob
	{
		public void Execute()
		{
			if (!OVRPlugin.GetSpaceTriangleMesh(this.Space, this.Positions, this.Indices))
			{
				UnsafeUtility.MemSet(this.Indices.GetUnsafePtr<int>(), 0, (long)(this.Indices.Length * 4));
			}
		}

		public ulong Space;

		public NativeArray<Vector3> Positions;

		public NativeArray<int> Indices;
	}

	private struct Triangle
	{
		public int A;

		public int B;

		public int C;
	}

	private struct FlipTriangleWindingJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			OVRTriangleMesh.Triangle triangle = this.Triangles[index];
			this.Triangles[index] = new OVRTriangleMesh.Triangle
			{
				A = triangle.A,
				B = triangle.C,
				C = triangle.B
			};
		}

		public NativeArray<OVRTriangleMesh.Triangle> Triangles;
	}

	private struct NegateXJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			Vector3 vector = this.Positions[index];
			this.Positions[index] = new Vector3(-vector.x, vector.y, vector.z);
		}

		public NativeArray<Vector3> Positions;
	}
}
