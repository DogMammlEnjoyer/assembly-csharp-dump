using System;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal struct AsyncTerrainToMeshRequest
	{
		internal AsyncTerrainToMeshRequest(ComputeTerrainMeshJob job, JobHandle jobHandle)
		{
			this.m_Job = job;
			this.m_JobHandle = jobHandle;
		}

		public bool done
		{
			get
			{
				return this.m_JobHandle.IsCompleted;
			}
		}

		public Mesh GetMesh()
		{
			if (!this.done)
			{
				return null;
			}
			Mesh mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32;
			mesh.SetVertices<float3>(this.m_Job.positions);
			mesh.SetUVs<float2>(0, this.m_Job.uvs);
			mesh.SetNormals<float3>(this.m_Job.normals);
			mesh.SetIndices(this.TriangleIndicesWithoutHoles().ToArray(), MeshTopology.Triangles, 0);
			this.m_Job.DisposeArrays();
			return mesh;
		}

		public void WaitForCompletion()
		{
			this.m_JobHandle.Complete();
		}

		private List<int> TriangleIndicesWithoutHoles()
		{
			List<int> list = new List<int>((this.m_Job.width - 1) * (this.m_Job.height - 1) * 6);
			for (int i = 0; i < this.m_Job.indices.Length; i += 3)
			{
				int num = this.m_Job.indices[i];
				int num2 = this.m_Job.indices[i + 1];
				int num3 = this.m_Job.indices[i + 2];
				if (num != 0 && num2 != 0 && num3 != 0)
				{
					list.Add(num);
					list.Add(num2);
					list.Add(num3);
				}
			}
			if (list.Count == 0)
			{
				list.Add(0);
				list.Add(0);
				list.Add(0);
			}
			return list;
		}

		private JobHandle m_JobHandle;

		private ComputeTerrainMeshJob m_Job;
	}
}
