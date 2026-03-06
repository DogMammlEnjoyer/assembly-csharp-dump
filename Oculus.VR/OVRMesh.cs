using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class OVRMesh : MonoBehaviour
{
	public bool IsInitialized { get; private set; }

	public Mesh Mesh
	{
		get
		{
			return this._mesh;
		}
	}

	internal OVRMesh.MeshType GetMeshType()
	{
		return this._meshType;
	}

	internal void SetMeshType(OVRMesh.MeshType type)
	{
		this._meshType = type;
	}

	private void Awake()
	{
		if (this._dataProvider == null)
		{
			this._dataProvider = base.GetComponent<OVRMesh.IOVRMeshDataProvider>();
		}
		if (this._dataProvider != null)
		{
			this._meshType = this._dataProvider.GetMeshType();
		}
		if (this.ShouldInitialize())
		{
			this.Initialize(this._meshType);
		}
	}

	private bool ShouldInitialize()
	{
		if (this._loadedMeshType != this._meshType)
		{
			return true;
		}
		if (this.IsInitialized)
		{
			return false;
		}
		if (this._meshType == OVRMesh.MeshType.None)
		{
			return false;
		}
		this._meshType.IsHand();
		return true;
	}

	private void Initialize(OVRMesh.MeshType meshType)
	{
		this._mesh = new Mesh();
		OVRPlugin.Mesh ovrpMesh;
		if (OVRPlugin.GetMesh((OVRPlugin.MeshType)meshType, out ovrpMesh))
		{
			this.TransformOvrpMesh(ovrpMesh, this._mesh);
			this.IsInitialized = true;
		}
		this._loadedMeshType = meshType;
	}

	private void TransformOvrpMesh(OVRPlugin.Mesh ovrpMesh, Mesh mesh)
	{
		int numVertices = (int)ovrpMesh.NumVertices;
		int numIndices = (int)ovrpMesh.NumIndices;
		using (OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector3f> nativeArrayHelper = new OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector3f>(ovrpMesh.VertexPositions, numVertices))
		{
			using (OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector3f> nativeArrayHelper2 = new OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector3f>(ovrpMesh.VertexNormals, numVertices))
			{
				using (OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector2f> nativeArrayHelper3 = new OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector2f>(ovrpMesh.VertexUV0, numVertices))
				{
					using (OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector4f> nativeArrayHelper4 = new OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector4f>(ovrpMesh.BlendWeights, numVertices))
					{
						using (OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector4s> nativeArrayHelper5 = new OVRMeshJobs.NativeArrayHelper<OVRPlugin.Vector4s>(ovrpMesh.BlendIndices, numVertices))
						{
							using (OVRMeshJobs.NativeArrayHelper<short> nativeArrayHelper6 = new OVRMeshJobs.NativeArrayHelper<short>(ovrpMesh.Indices, numIndices))
							{
								using (NativeArray<Vector3> vertices = new NativeArray<Vector3>(numVertices, Allocator.TempJob, NativeArrayOptions.ClearMemory))
								{
									using (NativeArray<Vector3> normals = new NativeArray<Vector3>(numVertices, Allocator.TempJob, NativeArrayOptions.ClearMemory))
									{
										using (NativeArray<Vector2> uv = new NativeArray<Vector2>(numVertices, Allocator.TempJob, NativeArrayOptions.ClearMemory))
										{
											using (NativeArray<BoneWeight> boneWeights = new NativeArray<BoneWeight>(numVertices, Allocator.TempJob, NativeArrayOptions.ClearMemory))
											{
												using (NativeArray<uint> triangles = new NativeArray<uint>(numIndices, Allocator.TempJob, NativeArrayOptions.ClearMemory))
												{
													OVRMeshJobs.TransformToUnitySpaceJob transformToUnitySpaceJob = new OVRMeshJobs.TransformToUnitySpaceJob
													{
														Vertices = vertices,
														Normals = normals,
														UV = uv,
														BoneWeights = boneWeights,
														MeshVerticesPosition = nativeArrayHelper.UnityNativeArray,
														MeshNormals = nativeArrayHelper2.UnityNativeArray,
														MeshUV = nativeArrayHelper3.UnityNativeArray,
														MeshBoneWeights = nativeArrayHelper4.UnityNativeArray,
														MeshBoneIndices = nativeArrayHelper5.UnityNativeArray
													};
													OVRMeshJobs.TransformTrianglesJob transformTrianglesJob = new OVRMeshJobs.TransformTrianglesJob
													{
														Triangles = triangles,
														MeshIndices = nativeArrayHelper6.UnityNativeArray,
														NumIndices = numIndices
													};
													JobHandle job = transformToUnitySpaceJob.Schedule(numVertices, 20, default(JobHandle));
													JobHandle job2 = transformTrianglesJob.Schedule(numIndices, 60, default(JobHandle));
													JobHandle.CombineDependencies(job, job2).Complete();
													mesh.SetVertices<Vector3>(transformToUnitySpaceJob.Vertices);
													mesh.SetNormals<Vector3>(transformToUnitySpaceJob.Normals);
													mesh.SetUVs<Vector2>(0, transformToUnitySpaceJob.UV);
													mesh.boneWeights = transformToUnitySpaceJob.BoneWeights.ToArray();
													mesh.SetIndexBufferParams(numIndices, IndexFormat.UInt32);
													mesh.SetIndexBufferData<uint>(transformTrianglesJob.Triangles, 0, 0, numIndices, MeshUpdateFlags.Default);
													mesh.SetSubMesh(0, new SubMeshDescriptor(0, numIndices, MeshTopology.Triangles), MeshUpdateFlags.Default);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	[SerializeField]
	private OVRMesh.IOVRMeshDataProvider _dataProvider;

	[SerializeField]
	private OVRMesh.MeshType _meshType = OVRMesh.MeshType.None;

	private OVRMesh.MeshType _loadedMeshType = OVRMesh.MeshType.None;

	private Mesh _mesh;

	public interface IOVRMeshDataProvider
	{
		OVRMesh.MeshType GetMeshType();
	}

	public enum MeshType
	{
		None = -1,
		[InspectorName("OVR Hand (Left)")]
		HandLeft,
		[InspectorName("OVR Hand (Right)")]
		HandRight,
		[InspectorName("OpenXR Hand (Left)")]
		XRHandLeft = 4,
		[InspectorName("OpenXR Hand (Right)")]
		XRHandRight
	}
}
