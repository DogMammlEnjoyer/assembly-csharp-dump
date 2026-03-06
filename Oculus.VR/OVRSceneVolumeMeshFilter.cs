using System;
using System.Collections;
using Meta.XR.Util;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#further-scene-model-unity-components")]
[RequireComponent(typeof(MeshFilter))]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRSceneVolumeMeshFilter : MonoBehaviour
{
	public bool IsCompleted { get; private set; }

	private void Start()
	{
		this._mesh = new Mesh
		{
			name = "OVRSceneVolumeMeshFilter (anonymous)"
		};
		this._meshFilter = base.GetComponent<MeshFilter>();
		this._meshFilter.sharedMesh = this._mesh;
		base.StartCoroutine(this.CreateVolumeMesh());
	}

	private IEnumerator CreateVolumeMesh()
	{
		OVRSceneAnchor sceneAnchor;
		if (!base.TryGetComponent<OVRSceneAnchor>(out sceneAnchor))
		{
			this.IsCompleted = true;
			yield break;
		}
		int num;
		int num2;
		using (NativeArray<int> meshCountResults = new NativeArray<int>(2, Allocator.TempJob, NativeArrayOptions.ClearMemory))
		{
			JobHandle job = new OVRSceneVolumeMeshFilter.GetTriangleMeshCountsJob
			{
				Space = sceneAnchor.Space,
				Results = meshCountResults
			}.Schedule(default(JobHandle));
			while (!OVRSceneVolumeMeshFilter.IsJobDone(job))
			{
				yield return null;
			}
			num = meshCountResults[0];
			num2 = meshCountResults[1];
			job = default(JobHandle);
		}
		NativeArray<int> meshCountResults = default(NativeArray<int>);
		if (num == -1)
		{
			this.IsCompleted = true;
			yield break;
		}
		NativeArray<Vector3> vertices = new NativeArray<Vector3>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		NativeArray<int> triangles = new NativeArray<int>(num2 * 3, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
		JobHandle dependsOn = new OVRSceneVolumeMeshFilter.GetTriangleMeshJob
		{
			Space = sceneAnchor.Space,
			Vertices = vertices,
			Triangles = triangles
		}.Schedule(default(JobHandle));
		JobHandle inputDeps = new OVRSceneVolumeMeshFilter.PopulateMeshDataJob
		{
			Vertices = vertices,
			Triangles = triangles,
			MeshData = meshDataArray[0]
		}.Schedule(dependsOn);
		JobHandle disposeVerticesJob = JobHandle.CombineDependencies(vertices.Dispose(inputDeps), triangles.Dispose(inputDeps));
		while (!OVRSceneVolumeMeshFilter.IsJobDone(disposeVerticesJob))
		{
			yield return null;
		}
		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, this._mesh, MeshUpdateFlags.Default);
		this._mesh.RecalculateNormals();
		this._mesh.RecalculateBounds();
		MeshCollider collider;
		if (base.TryGetComponent<MeshCollider>(out collider))
		{
			JobHandle job = new OVRSceneVolumeMeshFilter.BakeMeshJob
			{
				MeshID = this._mesh.GetInstanceID(),
				Convex = collider.convex
			}.Schedule(default(JobHandle));
			while (!OVRSceneVolumeMeshFilter.IsJobDone(job))
			{
				yield return null;
			}
			collider.sharedMesh = this._mesh;
			job = default(JobHandle);
		}
		this.IsCompleted = true;
		yield break;
		yield break;
	}

	private static bool IsJobDone(JobHandle job)
	{
		bool isCompleted = job.IsCompleted;
		if (isCompleted)
		{
			job.Complete();
		}
		return isCompleted;
	}

	private Mesh _mesh;

	private MeshFilter _meshFilter;

	private struct GetTriangleMeshCountsJob : IJob
	{
		public void Execute()
		{
			this.Results[0] = -1;
			this.Results[1] = -1;
			int value;
			int value2;
			if (OVRPlugin.GetSpaceTriangleMeshCounts(this.Space, out value, out value2))
			{
				this.Results[0] = value;
				this.Results[1] = value2;
			}
		}

		public OVRSpace Space;

		[WriteOnly]
		public NativeArray<int> Results;
	}

	private struct GetTriangleMeshJob : IJob
	{
		public void Execute()
		{
			OVRPlugin.GetSpaceTriangleMesh(this.Space, this.Vertices, this.Triangles);
		}

		public OVRSpace Space;

		[WriteOnly]
		public NativeArray<Vector3> Vertices;

		[WriteOnly]
		public NativeArray<int> Triangles;
	}

	private struct PopulateMeshDataJob : IJob
	{
		public void Execute()
		{
			this.MeshData.SetVertexBufferParams(this.Vertices.Length, new VertexAttributeDescriptor[]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
				new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 1)
			});
			NativeArray<Vector3> vertexData = this.MeshData.GetVertexData<Vector3>(0);
			for (int i = 0; i < vertexData.Length; i++)
			{
				Vector3 vector = this.Vertices[i];
				vertexData[i] = new Vector3(-vector.x, vector.y, vector.z);
			}
			this.MeshData.SetIndexBufferParams(this.Triangles.Length, IndexFormat.UInt32);
			NativeArray<int> indexData = this.MeshData.GetIndexData<int>();
			for (int j = 0; j < indexData.Length; j += 3)
			{
				indexData[j] = this.Triangles[j];
				indexData[j + 1] = this.Triangles[j + 2];
				indexData[j + 2] = this.Triangles[j + 1];
			}
			this.MeshData.subMeshCount = 1;
			this.MeshData.SetSubMesh(0, new SubMeshDescriptor(0, this.Triangles.Length, MeshTopology.Triangles), MeshUpdateFlags.Default);
		}

		[ReadOnly]
		public NativeArray<Vector3> Vertices;

		[ReadOnly]
		public NativeArray<int> Triangles;

		[WriteOnly]
		public Mesh.MeshData MeshData;
	}

	private struct BakeMeshJob : IJob
	{
		public void Execute()
		{
			Physics.BakeMesh(this.MeshID, this.Convex);
		}

		public int MeshID;

		public bool Convex;
	}
}
