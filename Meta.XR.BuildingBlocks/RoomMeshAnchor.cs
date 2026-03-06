using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomMeshAnchor : MonoBehaviour
{
	public bool IsCompleted { get; private set; }

	private bool Valid
	{
		get
		{
			return this._anchor.Handle > 0UL;
		}
	}

	private bool IsComponentEnabled<T>() where T : struct, IOVRAnchorComponent<T>
	{
		T t;
		return this._anchor.TryGetComponent<T>(out t) && t.IsEnabled;
	}

	private void Awake()
	{
		this._mesh = new Mesh
		{
			name = "RoomMeshAnchor (anonymous)"
		};
		if (!base.TryGetComponent<MeshFilter>(out this._meshFilter))
		{
			this._meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		this._meshFilter.sharedMesh = this._mesh;
	}

	internal void Initialize(OVRAnchor anchor)
	{
		RoomMeshAnchor.<Initialize>d__14 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Initialize>d__.<>4__this = this;
		<Initialize>d__.anchor = anchor;
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<RoomMeshAnchor.<Initialize>d__14>(ref <Initialize>d__);
	}

	private IEnumerator GenerateRoomMesh()
	{
		int num;
		int num2;
		using (NativeArray<int> meshCountResults = new NativeArray<int>(2, Allocator.TempJob, NativeArrayOptions.ClearMemory))
		{
			JobHandle job = new RoomMeshAnchor.GetTriangleMeshCountsJob
			{
				Space = this._anchor.Handle,
				Results = meshCountResults
			}.Schedule(default(JobHandle));
			while (!RoomMeshAnchor.IsJobDone(job))
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
		JobHandle dependsOn = new RoomMeshAnchor.GetTriangleMeshJob
		{
			Space = this._anchor.Handle,
			Vertices = vertices,
			Triangles = triangles
		}.Schedule(default(JobHandle));
		JobHandle inputDeps = new RoomMeshAnchor.PopulateMeshDataJob
		{
			Vertices = vertices,
			Triangles = triangles,
			MeshData = meshDataArray[0]
		}.Schedule(dependsOn);
		JobHandle disposeVerticesJob = JobHandle.CombineDependencies(vertices.Dispose(inputDeps), triangles.Dispose(inputDeps));
		while (!RoomMeshAnchor.IsJobDone(disposeVerticesJob))
		{
			yield return null;
		}
		Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, this._mesh, MeshUpdateFlags.Default);
		this._mesh.RecalculateNormals();
		this._mesh.RecalculateBounds();
		MeshCollider collider;
		if (base.TryGetComponent<MeshCollider>(out collider))
		{
			JobHandle job = new RoomMeshAnchor.BakeMeshJob
			{
				MeshID = this._mesh.GetInstanceID(),
				Convex = collider.convex
			}.Schedule(default(JobHandle));
			while (!RoomMeshAnchor.IsJobDone(job))
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

	private Task<T> EnableComponent<T>() where T : struct, IOVRAnchorComponent<T>
	{
		RoomMeshAnchor.<EnableComponent>d__16<T> <EnableComponent>d__;
		<EnableComponent>d__.<>t__builder = AsyncTaskMethodBuilder<T>.Create();
		<EnableComponent>d__.<>4__this = this;
		<EnableComponent>d__.<>1__state = -1;
		<EnableComponent>d__.<>t__builder.Start<RoomMeshAnchor.<EnableComponent>d__16<T>>(ref <EnableComponent>d__);
		return <EnableComponent>d__.<>t__builder.Task;
	}

	private bool TryUpdateTransform()
	{
		if (!this.Valid || !base.enabled || !this.IsComponentEnabled<OVRLocatable>())
		{
			return false;
		}
		OVRPlugin.Posef posef;
		OVRPlugin.SpaceLocationFlags value;
		if (!OVRPlugin.TryLocateSpace(this._anchor.Handle, OVRPlugin.GetTrackingOriginType(), out posef, out value) || !value.IsOrientationValid() || !value.IsPositionValid())
		{
			return false;
		}
		OVRPose ovrpose = new OVRPose
		{
			position = posef.Position.FromFlippedZVector3f(),
			orientation = posef.Orientation.FromFlippedZQuatf() * RoomMeshAnchor.RotateY180
		}.ToWorldSpacePose(Camera.main);
		base.transform.SetPositionAndRotation(ovrpose.position, ovrpose.orientation);
		return true;
	}

	private void OnDestroy()
	{
		Object.Destroy(this._mesh);
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

	private OVRAnchor _anchor;

	private static readonly Quaternion RotateY180 = Quaternion.Euler(0f, 180f, 0f);

	private OVRSemanticLabels _labels;

	private OVRTriangleMesh _triangleMeshComponent;

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
