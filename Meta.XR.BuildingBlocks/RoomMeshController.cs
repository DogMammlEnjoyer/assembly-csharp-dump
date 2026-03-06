using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Meta.XR.BuildingBlocks
{
	public class RoomMeshController : MonoBehaviour
	{
		private void Awake()
		{
			this._roomMeshAnchor = base.GetComponent<RoomMeshAnchor>();
			this._roomMeshEvent = Object.FindObjectOfType<RoomMeshEvent>();
		}

		private IEnumerator Start()
		{
			float timeout = 10f;
			float startTime = Time.time;
			while (!OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.Scene))
			{
				if (Time.time - startTime > timeout)
				{
					Debug.LogWarning("[RoomMeshController] Spatial Data permission is required to load Room Mesh.");
					yield break;
				}
				yield return null;
			}
			yield return this.LoadRoomMesh();
			yield return this.UpdateVolume();
			if (this._roomMeshAnchor == null)
			{
				yield break;
			}
			timeout = 3f;
			startTime = Time.time;
			while (!this._roomMeshAnchor.IsCompleted)
			{
				if (Time.time - startTime > timeout)
				{
					Debug.LogWarning("[RoomMeshController] Failed to create Room Mesh.");
					yield break;
				}
				yield return null;
			}
			UnityEvent<MeshFilter> onRoomMeshLoadCompleted = this._roomMeshEvent.OnRoomMeshLoadCompleted;
			if (onRoomMeshLoadCompleted != null)
			{
				onRoomMeshLoadCompleted.Invoke(this._roomMeshAnchor.GetComponent<MeshFilter>());
			}
			yield break;
		}

		private IEnumerator UpdateVolume()
		{
			if (this._roomMeshAnchor == null)
			{
				yield break;
			}
			while (!this._roomMeshAnchor.IsCompleted)
			{
				yield return null;
			}
			MeshFilter component = this._roomMeshAnchor.GetComponent<MeshFilter>();
			Mesh sharedMesh = component.sharedMesh;
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			sharedMesh.GetVertices(list);
			sharedMesh.GetTriangles(list2, 0);
			Color[] array = new Color[list2.Count];
			Vector3[] array2 = new Vector3[list2.Count];
			int[] array3 = new int[list2.Count];
			for (int i = 0; i < list2.Count; i++)
			{
				array[i] = new Color((i % 3 == 0) ? 1f : 0f, (i % 3 == 1) ? 1f : 0f, (i % 3 == 2) ? 1f : 0f);
				array2[i] = list[list2[i]];
				array3[i] = i;
			}
			Mesh mesh = new Mesh
			{
				indexFormat = IndexFormat.UInt32
			};
			mesh.SetVertices(array2);
			mesh.SetColors(array);
			mesh.SetIndices(array3, MeshTopology.Triangles, 0, true, 0);
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			component.sharedMesh = mesh;
			yield break;
		}

		private IEnumerator LoadRoomMesh()
		{
			List<OVRAnchor> anchors;
			using (new OVRObjectPool.ListScope<OVRAnchor>(ref anchors))
			{
				OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> task = OVRAnchor.FetchAnchorsAsync(anchors, new OVRAnchor.FetchOptions
				{
					SingleComponentType = typeof(OVRTriangleMesh)
				}, null);
				while (task.IsPending)
				{
					yield return null;
				}
				if (anchors.Count == 0)
				{
					Debug.LogWarning("[RoomMeshController] No RoomMesh available.");
					yield break;
				}
				foreach (OVRAnchor anchor in anchors)
				{
					OVRLocatable ovrlocatable;
					if (!anchor.TryGetComponent<OVRLocatable>(out ovrlocatable))
					{
						Debug.LogWarning("[RoomMeshController] Failed to localize the room mesh anchor.");
					}
					else
					{
						OVRTask<bool> localizeTask = ovrlocatable.SetEnabledAsync(true, 0.0);
						while (localizeTask.IsPending)
						{
							yield return null;
						}
						this.InstantiateRoomMesh(anchor, this._meshPrefab);
						localizeTask = default(OVRTask<bool>);
						anchor = default(OVRAnchor);
					}
				}
				List<OVRAnchor>.Enumerator enumerator = default(List<OVRAnchor>.Enumerator);
				task = default(OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>);
			}
			anchors = null;
			OVRObjectPool.ListScope<OVRAnchor> listScope = default(OVRObjectPool.ListScope<OVRAnchor>);
			yield break;
			yield break;
		}

		private void InstantiateRoomMesh(OVRAnchor anchor, GameObject prefab)
		{
			this._roomMeshAnchor = Object.Instantiate<GameObject>(prefab, Vector3.zero, Quaternion.identity).GetComponent<RoomMeshAnchor>();
			this._roomMeshAnchor.gameObject.name = this._meshPrefab.name;
			this._roomMeshAnchor.gameObject.SetActive(true);
			this._roomMeshAnchor.Initialize(anchor);
		}

		[SerializeField]
		private GameObject _meshPrefab;

		private RoomMeshEvent _roomMeshEvent;

		private RoomMeshAnchor _roomMeshAnchor;
	}
}
