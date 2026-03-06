using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_destructible_global_mesh_spawner")]
	public class DestructibleGlobalMeshSpawner : MonoBehaviour
	{
		public bool ReserveSpace
		{
			get
			{
				return this._reserveSpace;
			}
			set
			{
				this._reserveSpace = value;
			}
		}

		public float PointsPerUnitX
		{
			get
			{
				return this._pointsPerUnitX;
			}
			set
			{
				this._pointsPerUnitX = value;
			}
		}

		public float PointsPerUnitY
		{
			get
			{
				return this._pointsPerUnitY;
			}
			set
			{
				this._pointsPerUnitY = value;
			}
		}

		public int MaxPointsCount
		{
			get
			{
				return this._maxPointsCount;
			}
			set
			{
				this._maxPointsCount = value;
			}
		}

		public Material GlobalMeshMaterial
		{
			get
			{
				return this._globalMeshMaterial;
			}
			set
			{
				this._globalMeshMaterial = value;
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

		private void Start()
		{
			OVRTelemetry.Start(651898938, 0, -1L).Send();
			MRUK.Instance.RegisterSceneLoadedCallback(delegate
			{
				if (this.CreateOnRoomLoaded == MRUK.RoomFilter.None)
				{
					return;
				}
				switch (this.CreateOnRoomLoaded)
				{
				case MRUK.RoomFilter.None:
					break;
				case MRUK.RoomFilter.CurrentRoomOnly:
				{
					MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
					if (!this._spawnedDestructibleMeshes.ContainsKey(currentRoom))
					{
						this.AddDestructibleGlobalMesh(MRUK.Instance.GetCurrentRoom());
						return;
					}
					break;
				}
				case MRUK.RoomFilter.AllRooms:
					this.AddDestructibleGlobalMesh();
					return;
				default:
					throw new ArgumentOutOfRangeException();
				}
			});
			MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
			MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
		}

		private void AddDestructibleGlobalMesh()
		{
			foreach (MRUKRoom mrukroom in MRUK.Instance.Rooms)
			{
				if (!mrukroom.GlobalMeshAnchor)
				{
					Debug.LogWarning("Can not find a global mesh anchor, skipping the destructible mesh creation for this room");
				}
				else if (!this._spawnedDestructibleMeshes.ContainsKey(mrukroom))
				{
					this.AddDestructibleGlobalMesh(mrukroom);
				}
			}
		}

		public DestructibleGlobalMesh AddDestructibleGlobalMesh(MRUKRoom room)
		{
			if (this._spawnedDestructibleMeshes.ContainsKey(room))
			{
				throw new Exception("Cannot add a destructible mesh to this room as it already contains one.");
			}
			if (!room.GlobalMeshAnchor)
			{
				throw new Exception("A destructible mesh can not be created for this room as it does not contain a global mesh anchor.");
			}
			GameObject gameObject = new GameObject("DestructibleGlobalMesh");
			gameObject.transform.SetParent(room.GlobalMeshAnchor.transform, false);
			DestructibleMeshComponent destructibleMeshComponent = gameObject.AddComponent<DestructibleMeshComponent>();
			destructibleMeshComponent.GlobalMeshMaterial = this._globalMeshMaterial;
			if (!this._reserveSpace)
			{
				this.ReservedBottom = (this.ReservedTop = -1f);
			}
			destructibleMeshComponent.ReservedBottom = this.ReservedBottom;
			destructibleMeshComponent.ReservedTop = this.ReservedTop;
			destructibleMeshComponent.OnDestructibleMeshCreated = this.OnDestructibleMeshCreated;
			destructibleMeshComponent.OnSegmentationCompleted = this.OnSegmentationCompleted;
			DestructibleGlobalMesh destructibleGlobalMesh = new DestructibleGlobalMesh
			{
				MaxPointsCount = this._maxPointsCount,
				PointsPerUnitX = this._pointsPerUnitX,
				PointsPerUnitY = this._pointsPerUnitY,
				DestructibleMeshComponent = destructibleMeshComponent
			};
			DestructibleGlobalMeshSpawner.CreateDestructibleGlobalMesh(destructibleGlobalMesh, room);
			this._spawnedDestructibleMeshes.Add(room, destructibleGlobalMesh);
			return destructibleGlobalMesh;
		}

		private static void CreateDestructibleGlobalMesh(DestructibleGlobalMesh destructibleGlobalMesh, MRUKRoom room)
		{
			if (!room)
			{
				throw new Exception("Could not find a room for the destructible mesh");
			}
			if (!room.GlobalMeshAnchor || !room.GlobalMeshAnchor.Mesh)
			{
				throw new Exception("Could not load the mesh associated with the global mesh anchor of the room");
			}
			Vector3[] vertices = room.GlobalMeshAnchor.Mesh.vertices;
			int[] triangles = room.GlobalMeshAnchor.Mesh.triangles;
			Vector3[] array = DestructibleGlobalMeshSpawner.ComputeRoomBoxGrid(room, destructibleGlobalMesh.MaxPointsCount, destructibleGlobalMesh.PointsPerUnitX, destructibleGlobalMesh.PointsPerUnitY);
			uint[] meshIndices = Array.ConvertAll<int, uint>(triangles, new Converter<int, uint>(Convert.ToUInt32));
			Vector3[] array2 = new Vector3[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = room.transform.InverseTransformPoint(array[i]);
			}
			destructibleGlobalMesh.DestructibleMeshComponent.SegmentMesh(vertices, meshIndices, array2);
		}

		public bool TryGetDestructibleMeshForRoom(MRUKRoom room, out DestructibleGlobalMesh destructibleGlobalMesh)
		{
			destructibleGlobalMesh = this._spawnedDestructibleMeshes.GetValueOrDefault(room);
			return destructibleGlobalMesh != default(DestructibleGlobalMesh);
		}

		public void RemoveDestructibleGlobalMesh(MRUKRoom room = null)
		{
			if (MRUK.Instance == null || MRUK.Instance.GetCurrentRoom() == null)
			{
				throw new Exception("Can not remove a destructible global mesh when MRUK instance has not been initialized.");
			}
			if (room == null)
			{
				room = MRUK.Instance.GetCurrentRoom();
			}
			DestructibleGlobalMesh destructibleGlobalMesh;
			if (this.TryGetDestructibleMeshForRoom(room, out destructibleGlobalMesh))
			{
				Object.Destroy(destructibleGlobalMesh.DestructibleMeshComponent.gameObject);
				this._spawnedDestructibleMeshes.Remove(room);
			}
		}

		private void ReceiveCreatedRoom(MRUKRoom room)
		{
			if (this.CreateOnRoomLoaded == MRUK.RoomFilter.CurrentRoomOnly && this._spawnedDestructibleMeshes.Count > 0)
			{
				return;
			}
			if (this.CreateOnRoomLoaded == MRUK.RoomFilter.AllRooms)
			{
				this.AddDestructibleGlobalMesh();
			}
		}

		private void ReceiveRemovedRoom(MRUKRoom room)
		{
			if (room == null)
			{
				throw new Exception("Received a Room Removed event but the room is null.");
			}
			this.RemoveDestructibleGlobalMesh(room);
		}

		private static Vector3[] ComputeRoomBoxGrid(MRUKRoom room, int maxPointsCount, float pointsPerUnitX = 1f, float pointPerUnitY = 1f)
		{
			DestructibleGlobalMeshSpawner._points.Clear();
			foreach (MRUKAnchor mrukanchor in room.WallAnchors)
			{
				DestructibleGlobalMeshSpawner.GeneratePoints(DestructibleGlobalMeshSpawner._points, mrukanchor.transform.position, mrukanchor.transform.rotation, mrukanchor.PlaneRect, pointsPerUnitX, pointPerUnitY);
			}
			float num = room.CeilingAnchor.transform.position.y - room.FloorAnchor.transform.position.y;
			float num2 = Mathf.Max(Mathf.Ceil(pointPerUnitY * num), 1f);
			float num3 = num / num2;
			int num4 = 0;
			while ((float)num4 < num2)
			{
				Vector3 position = new Vector3(room.CeilingAnchor.transform.position.x, room.CeilingAnchor.transform.position.y - num3 * (float)num4, room.CeilingAnchor.transform.position.z);
				DestructibleGlobalMeshSpawner.GeneratePoints(DestructibleGlobalMeshSpawner._points, position, room.CeilingAnchor.transform.rotation, room.CeilingAnchor.PlaneRect, pointsPerUnitX, pointPerUnitY);
				num4++;
			}
			DestructibleGlobalMeshSpawner.GeneratePoints(DestructibleGlobalMeshSpawner._points, room.CeilingAnchor.transform.position, room.CeilingAnchor.transform.rotation, room.CeilingAnchor.PlaneRect, pointsPerUnitX, pointPerUnitY);
			DestructibleGlobalMeshSpawner.GeneratePoints(DestructibleGlobalMeshSpawner._points, room.FloorAnchor.transform.position, room.FloorAnchor.transform.rotation, room.FloorAnchor.PlaneRect, pointsPerUnitX, pointPerUnitY);
			if (DestructibleGlobalMeshSpawner._points.Count > maxPointsCount)
			{
				DestructibleGlobalMeshSpawner.Shuffle<Vector3>(DestructibleGlobalMeshSpawner._points);
				DestructibleGlobalMeshSpawner._points.RemoveRange(maxPointsCount, DestructibleGlobalMeshSpawner._points.Count - maxPointsCount);
			}
			return DestructibleGlobalMeshSpawner._points.ToArray();
		}

		private static void GeneratePoints(List<Vector3> points, Vector3 position, Quaternion rotation, Rect? planeBounds, float pointsPerUnitX, float pointsPerUnitY)
		{
			if (planeBounds == null)
			{
				throw new Exception("Failed to generate points as the given plane has no bounds.");
			}
			Vector3 vector = new Vector3(planeBounds.Value.size.x, planeBounds.Value.size.y, 0f);
			Vector3 a = position - rotation * new Vector3(vector.x * 0.5f, vector.y * 0.5f);
			float num = Mathf.Max(Mathf.Ceil(pointsPerUnitX * vector.x), 1f);
			float num2 = Mathf.Max(Mathf.Ceil(pointsPerUnitY * vector.y), 1f);
			Vector2 vector2 = new Vector2(vector.x / (num + 1f), vector.y / (num2 + 1f));
			int num3 = 0;
			while ((float)num3 < num2)
			{
				int num4 = 0;
				while ((float)num4 < num)
				{
					float x = (float)(num4 + 1) * vector2.x;
					float y = (float)(num3 + 1) * vector2.y;
					Vector3 item = a + rotation * new Vector3(x, y);
					points.Add(item);
					num4++;
				}
				num3++;
			}
		}

		private static void Shuffle<T>(List<T> list)
		{
			int i = list.Count;
			while (i > 1)
			{
				i--;
				int num = Random.Range(0, i + 1);
				int index = num;
				int index2 = i;
				T value = list[i];
				T value2 = list[num];
				list[index] = value;
				list[index2] = value2;
			}
		}

		[SerializeField]
		public MRUK.RoomFilter CreateOnRoomLoaded = MRUK.RoomFilter.CurrentRoomOnly;

		public UnityEvent<DestructibleMeshComponent> OnDestructibleMeshCreated;

		public Func<DestructibleMeshComponent.MeshSegmentationResult, DestructibleMeshComponent.MeshSegmentationResult> OnSegmentationCompleted;

		[SerializeField]
		private bool _reserveSpace;

		[SerializeField]
		private Vector3 _reservedMin;

		[SerializeField]
		private Vector3 _reservedMax;

		[SerializeField]
		private Material _globalMeshMaterial;

		[SerializeField]
		private float _pointsPerUnitX = 1f;

		[SerializeField]
		private float _pointsPerUnitY = 1f;

		[SerializeField]
		private int _maxPointsCount = 256;

		[SerializeField]
		private float _reservedTop;

		[SerializeField]
		private float _reservedBottom;

		private readonly Dictionary<MRUKRoom, DestructibleGlobalMesh> _spawnedDestructibleMeshes = new Dictionary<MRUKRoom, DestructibleGlobalMesh>();

		private const string _destructibleGlobalMeshObjectName = "DestructibleGlobalMesh";

		private static List<Vector3> _points = new List<Vector3>();
	}
}
