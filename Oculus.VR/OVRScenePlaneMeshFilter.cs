using System;
using Meta.XR.Util;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/#further-scene-model-unity-components")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRScenePlaneMeshFilter : MonoBehaviour
{
	private void Start()
	{
		this._mesh = new Mesh();
		this._meshFilter = base.GetComponent<MeshFilter>();
		this._meshFilter.sharedMesh = this._mesh;
		OVRSceneAnchor component = base.GetComponent<OVRSceneAnchor>();
		this._mesh.name = (component ? string.Format("{0} {1}", "OVRScenePlaneMeshFilter", component.Uuid) : "OVRScenePlaneMeshFilter (anonymous)");
		this.RequestMeshGeneration();
	}

	internal void ScheduleMeshGeneration()
	{
		if (this._jobHandle != null)
		{
			return;
		}
		OVRScenePlane ovrscenePlane;
		if (!base.TryGetComponent<OVRScenePlane>(out ovrscenePlane) || ovrscenePlane.Boundary.Count < 3)
		{
			return;
		}
		using (new OVRProfilerScope("ScheduleMeshGeneration"))
		{
			int count = ovrscenePlane.Boundary.Count;
			using (new OVRProfilerScope("Copy boundary"))
			{
				this._boundary = new NativeArray<Vector2>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				for (int i = 0; i < ovrscenePlane.Boundary.Count; i++)
				{
					this._boundary[i] = ovrscenePlane.Boundary[i];
				}
			}
			using (new OVRProfilerScope("Schedule TriangulateBoundaryJob"))
			{
				this._triangles = new NativeArray<int>((count - 2) * 3, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				this._jobHandle = new JobHandle?(new OVRScenePlaneMeshFilter.TriangulateBoundaryJob
				{
					Boundary = this._boundary,
					Triangles = this._triangles
				}.Schedule(default(JobHandle)));
			}
		}
	}

	private void Update()
	{
		if (this._jobHandle != null && this._jobHandle.GetValueOrDefault().IsCompleted)
		{
			this._jobHandle.Value.Complete();
			this._jobHandle = null;
			if (this._boundary.IsCreated && this._triangles.IsCreated)
			{
				try
				{
					if (this._triangles[0] == 0 && this._triangles[1] == 0 && this._triangles[2] == 0)
					{
						return;
					}
					using (new OVRProfilerScope("Update mesh"))
					{
						NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(this._boundary.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
						NativeArray<Vector3> nativeArray2 = new NativeArray<Vector3>(this._boundary.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
						NativeArray<Vector2> nativeArray3 = new NativeArray<Vector2>(this._boundary.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
						using (new OVRProfilerScope("Prepare mesh data"))
						{
							for (int i = 0; i < this._boundary.Length; i++)
							{
								Vector2 vector = this._boundary[i];
								nativeArray[i] = new Vector3(vector.x, vector.y, 0f);
								nativeArray2[i] = new Vector3(0f, 0f, 1f);
								nativeArray3[i] = new Vector2(vector.x, vector.y);
							}
						}
						using (nativeArray)
						{
							using (nativeArray2)
							{
								using (nativeArray3)
								{
									using (new OVRProfilerScope("Set mesh data"))
									{
										this._mesh.Clear();
										this._mesh.SetVertices<Vector3>(nativeArray);
										this._mesh.SetIndices<int>(this._triangles, MeshTopology.Triangles, 0, true, 0);
										this._mesh.SetNormals<Vector3>(nativeArray2);
										this._mesh.SetUVs<Vector2>(0, nativeArray3);
										return;
									}
								}
							}
						}
					}
				}
				finally
				{
					this._boundary.Dispose();
					this._triangles.Dispose();
				}
			}
			if (this._meshRequested)
			{
				this.ScheduleMeshGeneration();
			}
			return;
		}
	}

	internal void RequestMeshGeneration()
	{
		this._meshRequested = true;
		if (base.enabled)
		{
			this.ScheduleMeshGeneration();
		}
	}

	private void OnDisable()
	{
		if (this._triangles.IsCreated)
		{
			this._triangles.Dispose(this._jobHandle.GetValueOrDefault());
		}
		this._triangles = default(NativeArray<int>);
		this._jobHandle = null;
	}

	private MeshFilter _meshFilter;

	private Mesh _mesh;

	private JobHandle? _jobHandle;

	private bool _meshRequested;

	private NativeArray<Vector2> _boundary;

	private NativeArray<int> _triangles;

	private struct TriangulateBoundaryJob : IJob
	{
		public void Execute()
		{
			if (this.Boundary.Length == 0 || float.IsNaN(this.Boundary[0].x))
			{
				return;
			}
			OVRScenePlaneMeshFilter.TriangulateBoundaryJob.NList nlist = new OVRScenePlaneMeshFilter.TriangulateBoundaryJob.NList(this.Boundary.Length, Allocator.Temp);
			using (nlist)
			{
				bool flag = true;
				int index = 0;
				while (nlist.Count > 3)
				{
					if (!flag)
					{
						Debug.LogError("[OVRScenePlaneMeshFilter] Plane boundary triangulation failed.");
						this.Triangles[0] = 0;
						this.Triangles[1] = 0;
						this.Triangles[2] = 0;
						return;
					}
					flag = false;
					for (int i = 0; i < nlist.Count; i++)
					{
						int num = nlist[i];
						int at = nlist.GetAt(i - 1);
						int at2 = nlist.GetAt(i + 1);
						Vector2 vector = this.Boundary[num];
						Vector2 vector2 = this.Boundary[at];
						Vector2 vector3 = this.Boundary[at2];
						Vector2 a = vector2 - vector;
						Vector2 b = vector3 - vector;
						if (OVRScenePlaneMeshFilter.TriangulateBoundaryJob.Cross(a, b) >= 0f)
						{
							bool flag2 = true;
							for (int j = 0; j < this.Boundary.Length; j++)
							{
								if (j != num && j != at && j != at2 && OVRScenePlaneMeshFilter.TriangulateBoundaryJob.PointInTriangle(this.Boundary[j], vector, vector2, vector3))
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								this.Triangles[index++] = at2;
								this.Triangles[index++] = num;
								this.Triangles[index++] = at;
								nlist.RemoveAt(i);
								flag = true;
								break;
							}
						}
					}
				}
				this.Triangles[index++] = nlist[2];
				this.Triangles[index++] = nlist[1];
				this.Triangles[index] = nlist[0];
			}
		}

		private static float Cross(Vector2 a, Vector2 b)
		{
			return a.x * b.y - a.y * b.x;
		}

		private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
		{
			return OVRScenePlaneMeshFilter.TriangulateBoundaryJob.Cross(b - a, p - a) >= 0f && OVRScenePlaneMeshFilter.TriangulateBoundaryJob.Cross(c - b, p - b) >= 0f && OVRScenePlaneMeshFilter.TriangulateBoundaryJob.Cross(a - c, p - c) >= 0f;
		}

		[ReadOnly]
		public NativeArray<Vector2> Boundary;

		[WriteOnly]
		public NativeArray<int> Triangles;

		private struct NList : IDisposable
		{
			public int Count { readonly get; private set; }

			public NList(int capacity, Allocator allocator)
			{
				this.Count = capacity;
				this._data = new NativeArray<int>(capacity, allocator, NativeArrayOptions.ClearMemory);
				for (int i = 0; i < capacity; i++)
				{
					this._data[i] = i;
				}
			}

			public void RemoveAt(int index)
			{
				int count = this.Count - 1;
				this.Count = count;
				for (int i = index; i < this.Count; i++)
				{
					this._data[i] = this._data[i + 1];
				}
			}

			public int GetAt(int index)
			{
				if (index >= this.Count)
				{
					return this._data[index % this.Count];
				}
				if (index < 0)
				{
					return this._data[index % this.Count + this.Count];
				}
				return this._data[index];
			}

			public int this[int index]
			{
				get
				{
					return this._data[index];
				}
			}

			public void Dispose()
			{
				this._data.Dispose();
			}

			private NativeArray<int> _data;
		}
	}
}
