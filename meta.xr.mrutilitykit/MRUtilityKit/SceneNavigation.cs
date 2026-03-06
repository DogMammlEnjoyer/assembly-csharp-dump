using System;
using System.Collections.Generic;
using Meta.XR.Util;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_scene_navigation")]
	[Feature(Feature.Scene)]
	public class SceneNavigation : MonoBehaviour
	{
		public UnityEvent OnNavMeshInitialized { get; private set; } = new UnityEvent();

		public Dictionary<MRUKAnchor, GameObject> Obstacles { get; private set; } = new Dictionary<MRUKAnchor, GameObject>();

		[Obsolete("Navigable surfaces are now handled as NavMeshBuildSource hence this container is not going to be populated.Access the anchors used as navigable surfaces directly.")]
		public Dictionary<MRUKAnchor, GameObject> Surfaces { get; private set; } = new Dictionary<MRUKAnchor, GameObject>();

		private Transform ObstacleRoot
		{
			get
			{
				if (this._obstaclesRoot == null)
				{
					this._obstaclesRoot = new GameObject("_obstacles").transform;
				}
				return this._obstaclesRoot;
			}
		}

		private void Awake()
		{
			this._navMeshSurface = base.gameObject.GetComponent<NavMeshSurface>();
			this._cachedNavigableSceneLabels = this.NavigableSurfaces;
		}

		private void Start()
		{
			OVRTelemetry.Start(651889094, 0, -1L).Send();
			if (MRUK.Instance == null)
			{
				return;
			}
			MRUK.Instance.RegisterSceneLoadedCallback(new UnityAction(this.OnSceneLoadedEvent));
			if (!this.TrackUpdates)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
			MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
			MRUK.Instance.RoomUpdatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveUpdatedRoom));
		}

		private void OnSceneLoadedEvent()
		{
			switch (this.BuildOnSceneLoaded)
			{
			case MRUK.RoomFilter.None:
				break;
			case MRUK.RoomFilter.CurrentRoomOnly:
				this.BuildSceneNavMeshForRoom(MRUK.Instance.GetCurrentRoom());
				return;
			case MRUK.RoomFilter.AllRooms:
				this.BuildSceneNavMesh();
				break;
			default:
				return;
			}
		}

		private void ReceiveCreatedRoom(MRUKRoom room)
		{
			if (!this.TrackUpdates)
			{
				return;
			}
			this.BuildSceneNavMeshForRoom(room);
		}

		private void ReceiveUpdatedRoom(MRUKRoom room)
		{
			if (!this.TrackUpdates)
			{
				return;
			}
			this.RemoveNavMeshData();
			this.BuildSceneNavMeshForRoom(room);
		}

		private void ReceiveRemovedRoom(MRUKRoom room)
		{
			if (!this.TrackUpdates)
			{
				return;
			}
			this.RemoveNavMeshData();
		}

		public void ToggleGlobalMeshNavigation(bool useGlobalMesh, int agentTypeID = -1)
		{
			if (useGlobalMesh && MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor == null)
			{
				Debug.LogWarning("[MRUK] No Global Mesh anchor was found in the scene.");
				return;
			}
			if (useGlobalMesh)
			{
				this._cachedNavigableSceneLabels = this.NavigableSurfaces;
				this.NavigableSurfaces = MRUKAnchor.SceneLabels.GLOBAL_MESH;
			}
			else
			{
				this.NavigableSurfaces = this._cachedNavigableSceneLabels;
			}
			this.BuildSceneNavMesh();
		}

		public void BuildSceneNavMesh()
		{
			this.BuildSceneNavMeshForRoom(null);
		}

		public void BuildSceneNavMeshForRoom(MRUKRoom room = null)
		{
			if (!MRUK.Instance)
			{
				throw new NullReferenceException("MRUK instance is not initialized.");
			}
			List<MRUKRoom> list;
			if (!(room != null))
			{
				list = MRUK.Instance.Rooms;
			}
			else
			{
				(list = new List<MRUKRoom>()).Add(room);
			}
			List<MRUKRoom> list2 = list;
			if (list2.Count == 0)
			{
				throw new InvalidOperationException("No rooms available for NavMesh building.");
			}
			this.CreateNavMeshSurface();
			this.RemoveNavMeshData();
			Bounds bounds = this.ResizeNavMeshFromRoomBounds(ref this._navMeshSurface, list2);
			this._sources.Clear();
			NavMeshBuildSettings navMeshBuildSettings = (!this.CustomAgent) ? NavMesh.GetSettingsByIndex(this.AgentIndex) : this.CreateNavMeshBuildSettings(this.AgentRadius, this.AgentHeight, this.AgentMaxSlope, this.AgentClimb);
			this._navMeshSurface.agentTypeID = navMeshBuildSettings.agentTypeID;
			SceneNavigation.ValidateBuildSettings(navMeshBuildSettings, bounds);
			if (this.UseSceneData)
			{
				this.CreateObstacles(list2);
				this.CollectSceneSources(list2, this._sources);
			}
			else
			{
				NavMeshBuilder.CollectSources(bounds, this._navMeshSurface.layerMask, this._navMeshSurface.useGeometry, 0, this.GenerateLinks, new List<NavMeshBuildMarkup>(), false, this._sources);
			}
			NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(navMeshBuildSettings, this._sources, bounds, Vector3.zero, Quaternion.identity);
			this._navMeshSurface.navMeshData = navMeshData;
			this._navMeshSurface.AddData();
			this.InitializeNavMesh(navMeshBuildSettings.agentTypeID);
		}

		private void CollectSceneSources(List<MRUKRoom> rooms, ICollection<NavMeshBuildSource> sources)
		{
			NavMeshBuildSource item = default(NavMeshBuildSource);
			foreach (MRUKRoom mrukroom in rooms)
			{
				foreach (MRUKAnchor mrukanchor in mrukroom.Anchors)
				{
					if (mrukanchor && mrukanchor.HasAnyLabel(this.NavigableSurfaces))
					{
						item.transform = mrukanchor.transform.localToWorldMatrix;
						item.sourceObject = Utilities.SetupAnchorMeshGeometry(mrukanchor, true, null);
						item.shape = NavMeshBuildSourceShape.Mesh;
						sources.Add(item);
					}
				}
			}
		}

		public NavMeshBuildSettings CreateNavMeshBuildSettings(float agentRadius, float agentHeight, float agentMaxSlope, float agentClimb)
		{
			NavMeshBuildSettings result = NavMesh.CreateSettings();
			result.agentRadius = agentRadius;
			result.agentHeight = agentHeight;
			result.agentSlope = agentMaxSlope;
			result.agentClimb = agentClimb;
			result.overrideVoxelSize = this.OverrideVoxelSize;
			if (this.OverrideVoxelSize)
			{
				result.voxelSize = this.VoxelSize;
			}
			result.overrideTileSize = this.OverrideTileSize;
			if (this.OverrideTileSize)
			{
				result.tileSize = this.TileSize;
			}
			return result;
		}

		public void CreateNavMeshSurface()
		{
			this._navMeshSurface = base.GetComponent<NavMeshSurface>();
			if (!this._navMeshSurface)
			{
				this._navMeshSurface = base.gameObject.AddComponent<NavMeshSurface>();
			}
			this._navMeshSurface.minRegionArea = 0.01f;
			this._navMeshSurface.voxelSize = this.VoxelSize;
			if (!this.UseSceneData)
			{
				this._navMeshSurface.collectObjects = this.CollectObjects;
				this._navMeshSurface.useGeometry = this.CollectGeometry;
				this._navMeshSurface.hideFlags = HideFlags.None;
			}
			else
			{
				this._navMeshSurface.collectObjects = CollectObjects.Children;
				this._navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
				this._navMeshSurface.hideFlags = HideFlags.NotEditable;
			}
			this._navMeshSurface.layerMask = this.Layers;
		}

		public void RemoveNavMeshData()
		{
			if (!this._navMeshSurface)
			{
				return;
			}
			this._navMeshSurface.navMeshData = null;
			this._navMeshSurface.RemoveData();
			if (this.Obstacles != null)
			{
				this.ClearObstacles(null);
			}
		}

		public Bounds ResizeNavMeshFromRoomBounds(ref NavMeshSurface surface, MRUKRoom room = null)
		{
			List<MRUKRoom> list;
			if (!(room != null))
			{
				(list = new List<MRUKRoom>()).Add(MRUK.Instance.GetCurrentRoom());
			}
			else
			{
				(list = new List<MRUKRoom>()).Add(room);
			}
			List<MRUKRoom> rooms = list;
			return this.ResizeNavMeshFromRoomBounds(ref surface, rooms);
		}

		public Bounds ResizeNavMeshFromRoomBounds(ref NavMeshSurface surface, List<MRUKRoom> rooms)
		{
			if (rooms.Count == 0)
			{
				throw new InvalidOperationException("No rooms available to resize the NavMeshSurface.");
			}
			Bounds roomBounds = rooms[0].GetRoomBounds();
			for (int i = 1; i < rooms.Count; i++)
			{
				roomBounds.Encapsulate(rooms[i].GetRoomBounds());
			}
			Vector3 center = new Vector3(roomBounds.center.x, roomBounds.center.y, roomBounds.center.z);
			surface.center = center;
			Vector3 a = new Vector3(roomBounds.size.x, roomBounds.size.y, roomBounds.size.z);
			surface.size = a * 1.1f;
			return new Bounds(surface.center, surface.size);
		}

		private void InitializeNavMesh(int agentTypeID)
		{
			if (this._navMeshSurface.navMeshData.sourceBounds.extents.x * this._navMeshSurface.navMeshData.sourceBounds.extents.z <= 0f)
			{
				Debug.LogWarning("Failed to generate a nav mesh, this may be because the room is too small or the AgentType settings are to strict");
				return;
			}
			if (this.Agents != null)
			{
				foreach (NavMeshAgent navMeshAgent in this.Agents)
				{
					navMeshAgent.agentTypeID = agentTypeID;
				}
			}
			UnityEvent onNavMeshInitialized = this.OnNavMeshInitialized;
			if (onNavMeshInitialized == null)
			{
				return;
			}
			onNavMeshInitialized.Invoke();
		}

		public void CreateObstacles(MRUKRoom room = null)
		{
			List<MRUKRoom> list;
			if (!(room == null))
			{
				(list = new List<MRUKRoom>()).Add(MRUK.Instance.GetCurrentRoom());
			}
			else
			{
				(list = new List<MRUKRoom>()).Add(room);
			}
			List<MRUKRoom> rooms = list;
			this.CreateObstacles(rooms);
		}

		public void CreateObstacles(List<MRUKRoom> rooms)
		{
			this.ObstacleRoot.transform.SetParent(base.transform);
			foreach (MRUKRoom mrukroom in rooms)
			{
				foreach (MRUKAnchor anchor in mrukroom.Anchors)
				{
					this.CreateObstacle(anchor, true, false, 0.2f, 0.2f);
				}
			}
		}

		public void CreateObstacle(MRUKAnchor anchor, bool shouldCarve = true, bool carveOnlyStationary = false, float carvingTimeToStationary = 0.2f, float carvingMoveThreshold = 0.2f)
		{
			if (!anchor || !anchor.HasAnyLabel(this.SceneObstacles))
			{
				return;
			}
			if (this.Obstacles.ContainsKey(anchor))
			{
				Debug.LogWarning("Anchor already associated with an obstacle from this SceneNavigation");
				return;
			}
			Vector3 obstacleSize;
			Vector3 obstacleCenter;
			if (anchor.VolumeBounds != null)
			{
				obstacleSize = anchor.VolumeBounds.Value.size;
				obstacleCenter = anchor.VolumeBounds.Value.center;
			}
			else
			{
				if (anchor.PlaneRect == null)
				{
					return;
				}
				obstacleSize = anchor.PlaneRect.Value.size;
				obstacleCenter = anchor.PlaneRect.Value.center;
			}
			this.InstantiateObstacle(anchor, shouldCarve, carveOnlyStationary, carvingTimeToStationary, carvingMoveThreshold, obstacleSize, obstacleCenter);
		}

		private void InstantiateObstacle(MRUKAnchor anchor, bool shouldCarve, bool carveOnlyStationary, float carvingTimeToStationary, float carvingMoveThreshold, Vector3 obstacleSize, Vector3 obstacleCenter)
		{
			GameObject gameObject = new GameObject("_obstacles_" + anchor.name);
			gameObject.transform.SetParent(this._obstaclesRoot.transform);
			NavMeshObstacle navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
			navMeshObstacle.carving = shouldCarve;
			navMeshObstacle.carveOnlyStationary = carveOnlyStationary;
			navMeshObstacle.carvingTimeToStationary = carvingTimeToStationary;
			navMeshObstacle.carvingMoveThreshold = carvingMoveThreshold;
			navMeshObstacle.shape = NavMeshObstacleShape.Box;
			navMeshObstacle.transform.position = anchor.transform.position;
			navMeshObstacle.transform.rotation = anchor.transform.rotation;
			navMeshObstacle.size = obstacleSize;
			navMeshObstacle.center = obstacleCenter;
			this.Obstacles.Add(anchor, gameObject);
		}

		private List<Mesh> CreateRoomBridges(List<ValueTuple<MRUKAnchor, MRUKAnchor>> connections)
		{
			this._connectionMeshes.Clear();
			Vector3[] array = new Vector3[4];
			int[] triangles = new int[]
			{
				0,
				2,
				1,
				1,
				2,
				3
			};
			for (int i = 0; i < connections.Count; i++)
			{
				MRUKAnchor item = connections[i].Item1;
				MRUKAnchor item2 = connections[i].Item2;
				if (item.PlaneRect != null && item2.PlaneRect != null)
				{
					List<Vector2> planeBoundary2D = item.PlaneBoundary2D;
					List<Vector2> planeBoundary2D2 = item2.PlaneBoundary2D;
					Mesh mesh = new Mesh();
					array[0] = item.transform.TransformPoint(new Vector3(planeBoundary2D[0].x, planeBoundary2D[0].y, 0f));
					array[1] = item2.transform.TransformPoint(new Vector3(planeBoundary2D2[1].x, planeBoundary2D2[1].y, 0f));
					array[2] = item.transform.TransformPoint(new Vector3(planeBoundary2D[1].x, planeBoundary2D[1].y, 0f));
					array[3] = item2.transform.TransformPoint(new Vector3(planeBoundary2D2[0].x, planeBoundary2D2[0].y, 0f));
					mesh.vertices = array;
					mesh.triangles = triangles;
					mesh.RecalculateNormals();
					this._connectionMeshes.Add(mesh);
				}
			}
			return this._connectionMeshes;
		}

		[Obsolete("Navigable surfaces are now handled as NavMeshBuildSource, and are automatically created when buildingthe NavMesh using the scene data. Use EffectMesh to spawn colliders in the place of anchors.", true)]
		public void CreateNavigableSurfaces(MRUKRoom room = null)
		{
			List<MRUKRoom> list = new List<MRUKRoom>();
			if (room)
			{
				list.Add(room);
			}
			else
			{
				list = MRUK.Instance.Rooms;
			}
			if (this._surfacesRoot == null)
			{
				this._surfacesRoot = new GameObject("_surface").transform;
			}
			this._surfacesRoot.transform.SetParent(base.transform);
			foreach (MRUKRoom mrukroom in list)
			{
				foreach (MRUKAnchor anchor in mrukroom.Anchors)
				{
					this.CreateNavigableSurface(anchor);
				}
			}
			this._navMeshSurface.collectObjects = CollectObjects.Children;
		}

		private void CreateNavigableSurface(MRUKAnchor anchor)
		{
			if (!anchor || !anchor.HasAnyLabel(this.NavigableSurfaces))
			{
				return;
			}
			GameObject gameObject = new GameObject("_surface_" + anchor.name);
			gameObject.transform.SetParent(this._surfacesRoot.transform);
			gameObject.gameObject.layer = SceneNavigation.GetFirstLayerFromLayerMask(this.Layers);
			if (!anchor || !anchor.HasAnyLabel(this.NavigableSurfaces))
			{
				return;
			}
			if (this.Surfaces.ContainsKey(anchor))
			{
				Debug.LogWarning("Anchor already associated with an obstacle from this SceneNavigation");
				return;
			}
			Vector3 size;
			Vector3 center;
			if (anchor.VolumeBounds != null)
			{
				size = anchor.VolumeBounds.Value.size;
				center = anchor.VolumeBounds.Value.center;
			}
			else
			{
				if (anchor.PlaneRect == null)
				{
					MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = anchor.Mesh;
					meshCollider.transform.position = anchor.transform.position;
					meshCollider.transform.rotation = anchor.transform.rotation;
					this.Surfaces.Add(anchor, gameObject);
					return;
				}
				size = anchor.PlaneRect.Value.size;
				center = anchor.PlaneRect.Value.center;
			}
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.transform.position = anchor.transform.position;
			boxCollider.transform.rotation = anchor.transform.rotation;
			boxCollider.size = size;
			boxCollider.center = center;
			this.Surfaces.Add(anchor, gameObject);
		}

		public void ClearObstacles(MRUKRoom room = null)
		{
			List<MRUKAnchor> list = new List<MRUKAnchor>();
			foreach (KeyValuePair<MRUKAnchor, GameObject> keyValuePair in this.Obstacles)
			{
				if (!(room != null) || !(keyValuePair.Key.Room != room))
				{
					Object.DestroyImmediate(keyValuePair.Value);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (MRUKAnchor key in list)
			{
				this.Obstacles.Remove(key);
			}
		}

		public void ClearObstacle(MRUKAnchor anchor)
		{
			GameObject obj;
			if (!this.Obstacles.TryGetValue(anchor, out obj))
			{
				return;
			}
			Object.DestroyImmediate(obj);
			this.Obstacles.Remove(anchor);
		}

		private void ClearSurfaces(MRUKRoom room = null)
		{
			List<MRUKAnchor> list = new List<MRUKAnchor>();
			foreach (KeyValuePair<MRUKAnchor, GameObject> keyValuePair in this.Surfaces)
			{
				if (!(room != null) || !(keyValuePair.Key.Room != room))
				{
					Object.DestroyImmediate(keyValuePair.Value);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (MRUKAnchor key in list)
			{
				this.Surfaces.Remove(key);
			}
		}

		[Obsolete("Navigable surfaces are now handled as NavMeshBuildSource hence their destruction is handled internally.")]
		public void ClearSurface(MRUKAnchor anchor)
		{
			if (!this.Surfaces.ContainsKey(anchor))
			{
				return;
			}
			Object.DestroyImmediate(this.Surfaces[anchor]);
			this.Surfaces.Remove(anchor);
		}

		public static int GetFirstLayerFromLayerMask(LayerMask layerMask)
		{
			int result = 0;
			for (int i = 0; i < 32; i++)
			{
				if ((1 << i & layerMask) != 0)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public static bool ValidateBuildSettings(NavMeshBuildSettings navMeshBuildSettings, Bounds navMeshBounds)
		{
			string[] array = navMeshBuildSettings.ValidationReport(navMeshBounds);
			if (array.Length == 0)
			{
				return true;
			}
			string text = "Some NavMeshBuildSettings constraints were violated:\n";
			foreach (string str in array)
			{
				text = text + "- " + str + "\n";
			}
			Debug.LogWarning(text);
			return false;
		}

		private void OnDestroy()
		{
			this.OnNavMeshInitialized.RemoveAllListeners();
			if (!MRUK.Instance)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
			MRUK.Instance.RoomRemovedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
			MRUK.Instance.RoomUpdatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveUpdatedRoom));
			MRUK.Instance.SceneLoadedEvent.RemoveListener(new UnityAction(this.OnSceneLoadedEvent));
		}

		[Tooltip("When the scene data is loaded, this controls what room(s) will be used when baking the NavMesh.")]
		public MRUK.RoomFilter BuildOnSceneLoaded = MRUK.RoomFilter.CurrentRoomOnly;

		[Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
		internal bool TrackUpdates = true;

		[Tooltip("Used for specifying the type of geometry to collect when building a NavMesh")]
		public NavMeshCollectGeometry CollectGeometry = NavMeshCollectGeometry.PhysicsColliders;

		[Tooltip("Used for specifying the type of objects to include when building a NavMesh")]
		public CollectObjects CollectObjects = CollectObjects.Children;

		[Tooltip("The minimum distance to the walls where the navigation mesh can exist.")]
		public float AgentRadius = 0.2f;

		[Tooltip("How much vertical clearance space must exist.")]
		public float AgentHeight = 0.5f;

		[Tooltip("The height of discontinuities in the level the agent can climb over (i.e. steps and stairs).")]
		public float AgentClimb = 0.04f;

		[Tooltip("Maximum slope the agent can walk up.")]
		public float AgentMaxSlope = 5.5f;

		[Tooltip("The agents that will be assigned to the NavMesh generated with the scene data.")]
		public List<NavMeshAgent> Agents;

		[FormerlySerializedAs("SceneObjectsToInclude")]
		[Tooltip("The scene objects that will contribute to the creation of the NavMesh.")]
		public MRUKAnchor.SceneLabels NavigableSurfaces;

		[Tooltip("The scene objects that will carve a hole in the NavMesh.")]
		public MRUKAnchor.SceneLabels SceneObstacles;

		[Tooltip("A bitmask representing the layers to consider when selecting what that will be used for baking.")]
		public LayerMask Layers;

		[Tooltip("The agent's used that is going to be used to build the NavMesh")]
		public int AgentIndex;

		[Tooltip("Determines whether scene data should be used for NavMesh generation.")]
		public bool UseSceneData = true;

		[Tooltip("Determines whether a custom NavMeshAgent configuration should be used. If true, a new agent will be created when building the NavMesh.")]
		public bool CustomAgent = true;

		[Tooltip("Allows overriding the default voxel size used in NavMesh generation. Enable this to specify a custom voxel size.")]
		public bool OverrideVoxelSize;

		[Tooltip("The NavMesh voxel size in world length units. Should be 4-6 voxels per character diameter.")]
		public float VoxelSize;

		[Tooltip("Allows overriding the default tile size used in NavMesh generation. Enable this to specify a custom tile size.")]
		public bool OverrideTileSize;

		[Tooltip("Specifies the tile size for the NavMesh if OverrideTileSize is enabled. Represents the width and height of the square tiles in world units.")]
		public int TileSize = 256;

		[Tooltip("Enables the generation of off-mesh links in the NavMesh, allowing agents to navigate between disconnected mesh regions, such as jumping or climbing.")]
		public bool GenerateLinks;

		private EffectMesh _effectMesh;

		private readonly List<NavMeshBuildSource> _sources = new List<NavMeshBuildSource>();

		private readonly List<Mesh> _connectionMeshes = new List<Mesh>();

		private const float _minimumNavMeshSurfaceArea = 0f;

		private NavMeshSurface _navMeshSurface;

		private const string _obstaclePrefix = "_obstacles";

		private Transform _obstaclesRoot;

		private Transform _surfacesRoot;

		private MRUKAnchor.SceneLabels _cachedNavigableSceneLabels;
	}
}
