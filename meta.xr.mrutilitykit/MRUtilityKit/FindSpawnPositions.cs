using System;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
	[Feature(Feature.Scene)]
	public class FindSpawnPositions : MonoBehaviour
	{
		private void Start()
		{
			OVRTelemetry.Start(651888440, 0, -1L).Send();
			if (MRUK.Instance && this.SpawnOnStart != MRUK.RoomFilter.None)
			{
				MRUK.Instance.RegisterSceneLoadedCallback(delegate
				{
					MRUK.RoomFilter spawnOnStart = this.SpawnOnStart;
					if (spawnOnStart != MRUK.RoomFilter.CurrentRoomOnly)
					{
						if (spawnOnStart == MRUK.RoomFilter.AllRooms)
						{
							this.StartSpawn();
							return;
						}
					}
					else
					{
						this.StartSpawn(MRUK.Instance.GetCurrentRoom());
					}
				});
			}
		}

		public void StartSpawn()
		{
			foreach (MRUKRoom room in MRUK.Instance.Rooms)
			{
				this.StartSpawn(room);
			}
		}

		public void StartSpawn(MRUKRoom room)
		{
			Bounds? prefabBounds = Utilities.GetPrefabBounds(this.SpawnObject);
			float num = 0f;
			float d = (prefabBounds != null) ? (-prefabBounds.GetValueOrDefault().min.y) : 0f;
			float d2 = (prefabBounds != null) ? prefabBounds.GetValueOrDefault().center.y : 0f;
			Bounds bounds = default(Bounds);
			if (prefabBounds != null)
			{
				num = Mathf.Min(new float[]
				{
					-prefabBounds.Value.min.x,
					-prefabBounds.Value.min.z,
					prefabBounds.Value.max.x,
					prefabBounds.Value.max.z
				});
				if (num < 0f)
				{
					num = 0f;
				}
				Vector3 min = prefabBounds.Value.min;
				Vector3 max = prefabBounds.Value.max;
				min.y += 0.01f;
				if (max.y < min.y)
				{
					max.y = min.y;
				}
				bounds.SetMinMax(min, max);
				if (this.OverrideBounds > 0f)
				{
					Vector3 center = new Vector3(0f, 0.01f, 0f);
					Vector3 size = new Vector3(this.OverrideBounds * 2f, 0.02f, this.OverrideBounds * 2f);
					bounds = new Bounds(center, size);
				}
			}
			for (int i = 0; i < this.SpawnAmount; i++)
			{
				bool flag = false;
				int j = 0;
				while (j < this.MaxIterations)
				{
					Vector3 vector = Vector3.zero;
					Vector3 toDirection = Vector3.zero;
					if (this.SpawnLocations == FindSpawnPositions.SpawnLocation.Floating)
					{
						Vector3? vector2 = room.GenerateRandomPositionInRoom(num, true);
						if (vector2 != null)
						{
							vector = vector2.Value;
							goto IL_2C6;
						}
						break;
					}
					else
					{
						MRUK.SurfaceType surfaceType = (MRUK.SurfaceType)0;
						switch (this.SpawnLocations)
						{
						case FindSpawnPositions.SpawnLocation.AnySurface:
							surfaceType |= MRUK.SurfaceType.FACING_UP;
							surfaceType |= MRUK.SurfaceType.VERTICAL;
							surfaceType |= MRUK.SurfaceType.FACING_DOWN;
							break;
						case FindSpawnPositions.SpawnLocation.VerticalSurfaces:
							surfaceType |= MRUK.SurfaceType.VERTICAL;
							break;
						case FindSpawnPositions.SpawnLocation.OnTopOfSurfaces:
							surfaceType |= MRUK.SurfaceType.FACING_UP;
							break;
						case FindSpawnPositions.SpawnLocation.HangingDown:
							surfaceType |= MRUK.SurfaceType.FACING_DOWN;
							break;
						}
						Vector3 vector3;
						Vector3 vector4;
						if (!room.GenerateRandomPositionOnSurface(surfaceType, num, new LabelFilter(new MRUKAnchor.SceneLabels?(this.Labels), null), out vector3, out vector4))
						{
							goto IL_2C6;
						}
						vector = vector3 + vector4 * d;
						toDirection = vector4;
						Vector3 vector5 = vector + vector4 * d2;
						RaycastHit raycastHit;
						if (room.IsPositionInRoom(vector5, true) && !room.IsPositionInSceneVolume(vector5, 0f) && !room.Raycast(new Ray(vector3, vector4), this.SurfaceClearanceDistance, out raycastHit))
						{
							goto IL_2C6;
						}
					}
					IL_371:
					j++;
					continue;
					IL_2C6:
					Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, toDirection);
					if (this.CheckOverlaps && prefabBounds != null && Physics.CheckBox(vector + quaternion * bounds.center, bounds.extents, quaternion, this.LayerMask, QueryTriggerInteraction.Ignore))
					{
						goto IL_371;
					}
					flag = true;
					if (this.SpawnObject.gameObject.scene.path == null)
					{
						Object.Instantiate<GameObject>(this.SpawnObject, vector, quaternion, base.transform);
						break;
					}
					this.SpawnObject.transform.position = vector;
					this.SpawnObject.transform.rotation = quaternion;
					return;
				}
				if (!flag)
				{
					Debug.LogWarning(string.Format("Failed to find valid spawn position after {0} iterations. Only spawned {1} prefabs instead of {2}.", this.MaxIterations, i, this.SpawnAmount));
					return;
				}
			}
		}

		[Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
		public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

		[SerializeField]
		[Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
		public GameObject SpawnObject;

		[SerializeField]
		[Tooltip("Number of SpawnObject(s) to place into the scene per room, only applies to Prefabs.")]
		public int SpawnAmount = 8;

		[SerializeField]
		[Tooltip("Maximum number of times to attempt spawning/moving an object before giving up.")]
		public int MaxIterations = 1000;

		[FormerlySerializedAs("selectedSnapOption")]
		[SerializeField]
		[Tooltip("Attach content to scene surfaces.")]
		public FindSpawnPositions.SpawnLocation SpawnLocations;

		[SerializeField]
		[Tooltip("When using surface spawning, use this to filter which anchor labels should be included. Eg, spawn only on TABLE or OTHER.")]
		public MRUKAnchor.SceneLabels Labels = (MRUKAnchor.SceneLabels)(-1);

		[SerializeField]
		[Tooltip("If enabled then the spawn position will be checked to make sure there is no overlap with physics colliders including themselves.")]
		public bool CheckOverlaps = true;

		[SerializeField]
		[Tooltip("Required free space for the object (Set negative to auto-detect using GetPrefabBounds)")]
		public float OverrideBounds = -1f;

		[FormerlySerializedAs("layerMask")]
		[SerializeField]
		[Tooltip("Set the layer(s) for the physics bounding box checks, collisions will be avoided with these layers.")]
		public LayerMask LayerMask = -1;

		[SerializeField]
		[Tooltip("The clearance distance required in front of the surface in order for it to be considered a valid spawn position")]
		public float SurfaceClearanceDistance = 0.1f;

		public enum SpawnLocation
		{
			Floating,
			AnySurface,
			VerticalSurfaces,
			OnTopOfSurfaces,
			HangingDown
		}
	}
}
