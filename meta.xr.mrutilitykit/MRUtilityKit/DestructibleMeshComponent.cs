using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_destructible_mesh_component")]
	public class DestructibleMeshComponent : MonoBehaviour
	{
		public Material GlobalMeshMaterial
		{
			get
			{
				return this._destructibleMeshMaterial;
			}
			set
			{
				this._destructibleMeshMaterial = value;
			}
		}

		public float ReservedTop
		{
			get
			{
				return this._reservedTop;
			}
			set
			{
				this._reservedTop = value;
			}
		}

		public float ReservedBottom
		{
			get
			{
				return this._reservedBottom;
			}
			set
			{
				this._reservedBottom = value;
			}
		}

		public GameObject ReservedSegment { get; private set; }

		public unsafe void SegmentMesh(Vector3[] meshPositions, uint[] meshIndices, Vector3[] segmentationPoints)
		{
			Vector3 reservedMin = new Vector3(-1f, -1f, this.ReservedBottom);
			Vector3 reservedMax = new Vector3(-1f, -1f, this.ReservedTop);
			this._segmentationTask = Task.Run<DestructibleMeshComponent.MeshSegmentationResult>(delegate()
			{
				MRUKNativeFuncs.MrukMesh3f* ptr;
				uint numSegments;
				MRUKNativeFuncs.MrukMesh3f reservedSegment;
				if (MRUKNativeFuncs.ComputeMeshSegmentation(meshPositions, (uint)meshPositions.Length, meshIndices, (uint)meshIndices.Length, segmentationPoints, (uint)segmentationPoints.Length, reservedMin, reservedMax, out ptr, out numSegments, out reservedSegment) == MRUKNativeFuncs.MrukResult.Success)
				{
					DestructibleMeshComponent.MeshSegmentationResult result = this.ProcessSegments(ptr, numSegments, reservedSegment);
					MRUKNativeFuncs.FreeMeshSegmentation(ptr, numSegments, ref reservedSegment);
					return result;
				}
				MRUKNativeFuncs.FreeMeshSegmentation(ptr, numSegments, ref reservedSegment);
				throw new Exception(string.Format("Failed to segment the mesh: {0}", MRUKNativeFuncs.MrukResult.ErrorInvalidArgs));
			});
			this._segmentationTask.ContinueWith(new Action<Task<DestructibleMeshComponent.MeshSegmentationResult>>(this.OnSegmentationTaskCompleted), TaskScheduler.FromCurrentSynchronizationContext());
		}

		public int GetDestructibleMeshSegmentsCount()
		{
			return base.transform.childCount;
		}

		public void GetDestructibleMeshSegments<T>(T segments) where T : IList<GameObject>
		{
			if (segments == null)
			{
				throw new ArgumentNullException("segments", "Cannot populate the managed container with the global mesh segments as it was never initialized.");
			}
			if (segments.IsReadOnly)
			{
				throw new NotSupportedException("The segments collection is read-only and cannot be modified.");
			}
			segments.Clear();
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				segments.Add(transform.gameObject);
			}
		}

		public void GetDestructibleMeshSegments(GameObject[] segments)
		{
			if (segments == null)
			{
				throw new ArgumentNullException("segments", "Cannot populate the array with the global mesh segments as it was never initialized.");
			}
			int num = 0;
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (num >= segments.Length)
				{
					throw new ArgumentException("The provided array does not have enough space to hold all segments.", "segments");
				}
				segments[num++] = transform.gameObject;
			}
		}

		public void DestroySegment(GameObject segment)
		{
			this.GetDestructibleMeshSegments<List<GameObject>>(this._segments);
			if (!this._segments.Contains(segment))
			{
				Debug.LogError("The segment that has been requested to be destroyed does not belong to the destructible mesh anymore.This could be due to the segment being already been destroyed or it had its parent changed.");
				return;
			}
			if (segment == this.ReservedSegment)
			{
				Debug.LogWarning("The segment that has been requested to be destroyed is the reserved segment and it should not be destroyed.In case the deletion is intended destroy the ReservedSegment game object directly, together with its mesh and material.");
				return;
			}
			MeshFilter meshFilter;
			if (segment.TryGetComponent<MeshFilter>(out meshFilter) && meshFilter.mesh != null)
			{
				Object.Destroy(meshFilter.sharedMesh);
			}
			MeshRenderer meshRenderer;
			if (segment.TryGetComponent<MeshRenderer>(out meshRenderer) && meshRenderer.material != null)
			{
				Object.Destroy(meshRenderer.material);
			}
			Object.Destroy(segment);
		}

		private void OnSegmentationTaskCompleted(Task<DestructibleMeshComponent.MeshSegmentationResult> task)
		{
			if (task.Status == TaskStatus.RanToCompletion)
			{
				try
				{
					Func<DestructibleMeshComponent.MeshSegmentationResult, DestructibleMeshComponent.MeshSegmentationResult> onSegmentationCompleted = this.OnSegmentationCompleted;
					DestructibleMeshComponent.MeshSegmentationResult result = (onSegmentationCompleted != null) ? onSegmentationCompleted(task.Result) : task.Result;
					this.CreateDestructibleMesh(result);
					UnityEvent<DestructibleMeshComponent> onDestructibleMeshCreated = this.OnDestructibleMeshCreated;
					if (onDestructibleMeshCreated != null)
					{
						onDestructibleMeshCreated.Invoke(this);
					}
					return;
				}
				catch (Exception ex)
				{
					Debug.LogError("Error processing segmentation results: " + ex.Message);
					return;
				}
			}
			if (task.IsFaulted)
			{
				string str = "Segmentation task failed: ";
				AggregateException exception = task.Exception;
				string str2;
				if (exception == null)
				{
					str2 = null;
				}
				else
				{
					Exception innerException = exception.InnerException;
					str2 = ((innerException != null) ? innerException.Message : null);
				}
				Debug.LogError(str + str2);
				return;
			}
			if (task.IsCanceled)
			{
				Debug.LogWarning("Segmentation task was canceled.");
			}
		}

		private unsafe DestructibleMeshComponent.MeshSegmentationResult ProcessSegments(MRUKNativeFuncs.MrukMesh3f* segments, uint numSegments, MRUKNativeFuncs.MrukMesh3f reservedSegment)
		{
			DestructibleMeshComponent.MeshSegmentationResult meshSegmentationResult = new DestructibleMeshComponent.MeshSegmentationResult
			{
				segments = new List<DestructibleMeshComponent.MeshSegment>(),
				reservedSegment = new DestructibleMeshComponent.MeshSegment
				{
					positions = new Vector3[reservedSegment.numVertices],
					indices = new int[reservedSegment.numIndices]
				}
			};
			for (uint num = 0U; num < numSegments; num += 1U)
			{
				DestructibleMeshComponent.MeshSegment meshSegment = new DestructibleMeshComponent.MeshSegment
				{
					positions = new Vector3[segments[(ulong)num * (ulong)((long)sizeof(MRUKNativeFuncs.MrukMesh3f)) / (ulong)sizeof(MRUKNativeFuncs.MrukMesh3f)].numVertices],
					indices = new int[segments[(ulong)num * (ulong)((long)sizeof(MRUKNativeFuncs.MrukMesh3f)) / (ulong)sizeof(MRUKNativeFuncs.MrukMesh3f)].numIndices]
				};
				IntPtr intPtr = (IntPtr)((void*)segments[(ulong)num * (ulong)((long)sizeof(MRUKNativeFuncs.MrukMesh3f)) / (ulong)sizeof(MRUKNativeFuncs.MrukMesh3f)].vertices);
				int num2 = 0;
				while ((long)num2 < (long)((ulong)segments[(ulong)num * (ulong)((long)sizeof(MRUKNativeFuncs.MrukMesh3f)) / (ulong)sizeof(MRUKNativeFuncs.MrukMesh3f)].numVertices))
				{
					float x = Marshal.PtrToStructure<float>(intPtr);
					intPtr = IntPtr.Add(intPtr, 4);
					float y = Marshal.PtrToStructure<float>(intPtr);
					intPtr = IntPtr.Add(intPtr, 4);
					float z = Marshal.PtrToStructure<float>(intPtr);
					intPtr = IntPtr.Add(intPtr, 4);
					meshSegment.positions[num2] = new Vector3(x, y, z);
					num2++;
				}
				IntPtr intPtr2 = (IntPtr)((void*)segments[(ulong)num * (ulong)((long)sizeof(MRUKNativeFuncs.MrukMesh3f)) / (ulong)sizeof(MRUKNativeFuncs.MrukMesh3f)].indices);
				int num3 = 0;
				while ((long)num3 < (long)((ulong)segments[(ulong)num * (ulong)((long)sizeof(MRUKNativeFuncs.MrukMesh3f)) / (ulong)sizeof(MRUKNativeFuncs.MrukMesh3f)].numIndices))
				{
					meshSegment.indices[num3] = Marshal.ReadInt32(intPtr2);
					intPtr2 = IntPtr.Add(intPtr2, 4);
					num3++;
				}
				meshSegmentationResult.segments.Add(meshSegment);
			}
			IntPtr intPtr3 = (IntPtr)((void*)reservedSegment.vertices);
			int num4 = 0;
			while ((long)num4 < (long)((ulong)reservedSegment.numVertices))
			{
				float x2 = Marshal.PtrToStructure<float>(intPtr3);
				intPtr3 = IntPtr.Add(intPtr3, 4);
				float y2 = Marshal.PtrToStructure<float>(intPtr3);
				intPtr3 = IntPtr.Add(intPtr3, 4);
				float z2 = Marshal.PtrToStructure<float>(intPtr3);
				intPtr3 = IntPtr.Add(intPtr3, 4);
				meshSegmentationResult.reservedSegment.positions[num4] = new Vector3(x2, y2, z2);
				num4++;
			}
			IntPtr intPtr4 = (IntPtr)((void*)reservedSegment.indices);
			int num5 = 0;
			while ((long)num5 < (long)((ulong)reservedSegment.numIndices))
			{
				meshSegmentationResult.reservedSegment.indices[num5] = Marshal.ReadInt32(intPtr4);
				intPtr4 = IntPtr.Add(intPtr4, 4);
				num5++;
			}
			return meshSegmentationResult;
		}

		private void CreateDestructibleMesh(DestructibleMeshComponent.MeshSegmentationResult result)
		{
			foreach (DestructibleMeshComponent.MeshSegment meshSegment in result.segments)
			{
				this.CreateMeshSegment(meshSegment.positions, meshSegment.indices, meshSegment.uv, meshSegment.tangents, meshSegment.colors, false);
			}
			if (result.reservedSegment.indices.Length != 0)
			{
				this.ReservedSegment = this.CreateMeshSegment(result.reservedSegment.positions, result.reservedSegment.indices, result.reservedSegment.uv, result.reservedSegment.tangents, result.reservedSegment.colors, true);
			}
		}

		private GameObject CreateMeshSegment(Vector3[] positions, int[] indices, Vector2[] uv = null, Vector4[] tangents = null, Color[] colors = null, bool isReserved = false)
		{
			if (positions.Length == 0 || indices.Length == 0)
			{
				return null;
			}
			GameObject gameObject = new GameObject(isReserved ? "ReservedMeshSegment" : "DestructibleMeshSegment");
			gameObject.transform.SetParent(base.transform, false);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			Renderer renderer = gameObject.AddComponent<MeshRenderer>();
			meshFilter.mesh.indexFormat = ((positions.Length > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			meshFilter.mesh.SetVertices(positions);
			meshFilter.mesh.SetIndices(indices, MeshTopology.Triangles, 0);
			meshFilter.mesh.SetTangents(tangents);
			meshFilter.mesh.SetUVs(0, uv);
			meshFilter.mesh.SetColors(colors);
			renderer.material = this.GlobalMeshMaterial;
			return gameObject;
		}

		public void OnDestroy()
		{
			this.GetDestructibleMeshSegments<List<GameObject>>(this._segments);
			for (int i = this._segments.Count - 1; i >= 0; i--)
			{
				MeshFilter meshFilter;
				if (this._segments[i].TryGetComponent<MeshFilter>(out meshFilter) && meshFilter.mesh != null)
				{
					Object.Destroy(meshFilter.sharedMesh);
				}
				MeshRenderer meshRenderer;
				if (this._segments[i].TryGetComponent<MeshRenderer>(out meshRenderer) && meshRenderer.material != null)
				{
					Object.Destroy(meshRenderer.material);
				}
				Object.Destroy(this._segments[i]);
			}
			this._segmentationTask.Dispose();
			this._segmentationTask = null;
		}

		public void DebugDestructibleMeshComponent()
		{
			List<GameObject> list = new List<GameObject>();
			this.GetDestructibleMeshSegments<List<GameObject>>(list);
			foreach (GameObject gameObject in list)
			{
				Material material = new Material(Shader.Find("Meta/Lit"))
				{
					color = Random.ColorHSV()
				};
				MeshRenderer meshRenderer;
				if (gameObject.TryGetComponent<MeshRenderer>(out meshRenderer))
				{
					meshRenderer.material = material;
				}
			}
		}

		public UnityEvent<DestructibleMeshComponent> OnDestructibleMeshCreated;

		public Func<DestructibleMeshComponent.MeshSegmentationResult, DestructibleMeshComponent.MeshSegmentationResult> OnSegmentationCompleted;

		[SerializeField]
		private Material _destructibleMeshMaterial;

		[SerializeField]
		private float _reservedTop = -1f;

		[SerializeField]
		private float _reservedBottom = -1f;

		private Task<DestructibleMeshComponent.MeshSegmentationResult> _segmentationTask;

		private readonly List<GameObject> _segments = new List<GameObject>();

		public struct MeshSegment
		{
			public Vector3[] positions;

			public int[] indices;

			public Vector2[] uv;

			public Vector4[] tangents;

			public Color[] colors;
		}

		public struct MeshSegmentationResult
		{
			public List<DestructibleMeshComponent.MeshSegment> segments;

			public DestructibleMeshComponent.MeshSegment reservedSegment;
		}
	}
}
