using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Meta.XR.Util;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[HelpURL("https://developer.oculus.com/documentation/unity/unity-scene-use-scene-anchors/")]
[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
[Feature(Feature.Scene)]
public class OVRSceneManager : MonoBehaviour
{
	public Transform InitialAnchorParent
	{
		get
		{
			return this._initialAnchorParent;
		}
		set
		{
			this._initialAnchorParent = value;
		}
	}

	public event Action LoadSceneModelFailedPermissionNotGranted;

	internal OVRSceneManager.LogForwarder? Verbose
	{
		get
		{
			if (!this.VerboseLogging)
			{
				return null;
			}
			return new OVRSceneManager.LogForwarder?(default(OVRSceneManager.LogForwarder));
		}
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private static void Log(string message, GameObject gameObject = null)
	{
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private static void LogWarning(string message, GameObject gameObject = null)
	{
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	private static void LogError(string message, GameObject gameObject = null)
	{
	}

	private void Awake()
	{
		if (Object.FindObjectsByType<OVRSceneManager>(FindObjectsSortMode.None).Length > 1)
		{
			default(OVRSceneManager.LogForwarder).LogError("OVRSceneManager", "Found multiple OVRSceneManagers. Destroying '" + base.name + "'.", null);
			base.enabled = false;
			Object.DestroyImmediate(this);
		}
	}

	private void Start()
	{
		OVRTelemetryMarker ovrtelemetryMarker = OVRTelemetry.Start(163061745, 0, -1L);
		ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("basic_prefabs", (this.PlanePrefab != null || this.VolumePrefab != null) ? "true" : "false");
		ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("prefab_overrides", (this.PrefabOverrides.Count > 0) ? "true" : "false");
		ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("active_rooms_only", this.ActiveRoomsOnly ? "true" : "false");
		ovrtelemetryMarker.Send();
	}

	private static void LogResult(OVRAnchor.FetchResult value)
	{
		((OVRPlugin.Result)value).IsSuccess();
	}

	internal static OVRTask<bool> FetchAnchorsAsync<T>(List<OVRAnchor> anchors, Action<List<OVRAnchor>, int> incrementalResultsCallback = null) where T : struct, IOVRAnchorComponent<T>
	{
		OVRSceneManager.<FetchAnchorsAsync>d__36<T> <FetchAnchorsAsync>d__;
		<FetchAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<bool>.Create();
		<FetchAnchorsAsync>d__.anchors = anchors;
		<FetchAnchorsAsync>d__.incrementalResultsCallback = incrementalResultsCallback;
		<FetchAnchorsAsync>d__.<>1__state = -1;
		<FetchAnchorsAsync>d__.<>t__builder.Start<OVRSceneManager.<FetchAnchorsAsync>d__36<T>>(ref <FetchAnchorsAsync>d__);
		return <FetchAnchorsAsync>d__.<>t__builder.Task;
	}

	internal static OVRTask<bool> FetchAnchorsAsync(IEnumerable<Guid> uuids, List<OVRAnchor> anchors)
	{
		OVRSceneManager.<FetchAnchorsAsync>d__37 <FetchAnchorsAsync>d__;
		<FetchAnchorsAsync>d__.<>t__builder = OVRTaskBuilder<bool>.Create();
		<FetchAnchorsAsync>d__.uuids = uuids;
		<FetchAnchorsAsync>d__.anchors = anchors;
		<FetchAnchorsAsync>d__.<>1__state = -1;
		<FetchAnchorsAsync>d__.<>t__builder.Start<OVRSceneManager.<FetchAnchorsAsync>d__37>(ref <FetchAnchorsAsync>d__);
		return <FetchAnchorsAsync>d__.<>t__builder.Task;
	}

	internal void OnApplicationPause(bool isPaused)
	{
		OVRSceneManager.<OnApplicationPause>d__38 <OnApplicationPause>d__;
		<OnApplicationPause>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnApplicationPause>d__.<>4__this = this;
		<OnApplicationPause>d__.isPaused = isPaused;
		<OnApplicationPause>d__.<>1__state = -1;
		<OnApplicationPause>d__.<>t__builder.Start<OVRSceneManager.<OnApplicationPause>d__38>(ref <OnApplicationPause>d__);
	}

	private void QueryForExistingAnchorsTransform()
	{
		OVRSceneManager.<QueryForExistingAnchorsTransform>d__39 <QueryForExistingAnchorsTransform>d__;
		<QueryForExistingAnchorsTransform>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<QueryForExistingAnchorsTransform>d__.<>1__state = -1;
		<QueryForExistingAnchorsTransform>d__.<>t__builder.Start<OVRSceneManager.<QueryForExistingAnchorsTransform>d__39>(ref <QueryForExistingAnchorsTransform>d__);
	}

	public bool LoadSceneModel()
	{
		this._hasLoadBeenRequested = true;
		this.DestroyExistingAnchors();
		OVRTask<OVRSceneManager.LoadSceneModelResult> task = this.LoadSceneModelAsync();
		if (!task.IsCompleted)
		{
			this.<LoadSceneModel>g__AwaitTask|40_0(task);
			return true;
		}
		return this.<LoadSceneModel>g__InterpretResult|40_1(task.GetResult());
	}

	private OVRTask<OVRSceneManager.Metrics> ProcessBatch(List<OVRAnchor> rooms, int startingIndex)
	{
		OVRSceneManager.<ProcessBatch>d__44 <ProcessBatch>d__;
		<ProcessBatch>d__.<>t__builder = OVRTaskBuilder<OVRSceneManager.Metrics>.Create();
		<ProcessBatch>d__.<>4__this = this;
		<ProcessBatch>d__.rooms = rooms;
		<ProcessBatch>d__.startingIndex = startingIndex;
		<ProcessBatch>d__.<>1__state = -1;
		<ProcessBatch>d__.<>t__builder.Start<OVRSceneManager.<ProcessBatch>d__44>(ref <ProcessBatch>d__);
		return <ProcessBatch>d__.<>t__builder.Task;
	}

	private OVRTask<OVRSceneManager.LoadSceneModelResult> LoadSceneModelAsync()
	{
		OVRSceneManager.<LoadSceneModelAsync>d__45 <LoadSceneModelAsync>d__;
		<LoadSceneModelAsync>d__.<>t__builder = OVRTaskBuilder<OVRSceneManager.LoadSceneModelResult>.Create();
		<LoadSceneModelAsync>d__.<>4__this = this;
		<LoadSceneModelAsync>d__.<>1__state = -1;
		<LoadSceneModelAsync>d__.<>t__builder.Start<OVRSceneManager.<LoadSceneModelAsync>d__45>(ref <LoadSceneModelAsync>d__);
		return <LoadSceneModelAsync>d__.<>t__builder.Task;
	}

	private static OVRTask<ValueTuple<OVRSceneManager.LoadSceneModelResult, int>> FilterByActiveRoom(List<OVRAnchor> rooms, Dictionary<OVRAnchor, OVRSceneManager.RoomLayoutUuids> layouts)
	{
		OVRSceneManager.<FilterByActiveRoom>d__46 <FilterByActiveRoom>d__;
		<FilterByActiveRoom>d__.<>t__builder = OVRTaskBuilder<ValueTuple<OVRSceneManager.LoadSceneModelResult, int>>.Create();
		<FilterByActiveRoom>d__.rooms = rooms;
		<FilterByActiveRoom>d__.layouts = layouts;
		<FilterByActiveRoom>d__.<>1__state = -1;
		<FilterByActiveRoom>d__.<>t__builder.Start<OVRSceneManager.<FilterByActiveRoom>d__46>(ref <FilterByActiveRoom>d__);
		return <FilterByActiveRoom>d__.<>t__builder.Task;
	}

	private static bool IsUserInRoom(Vector3 userPosition, OVRAnchor floor, OVRAnchor ceiling)
	{
		OVRPlugin.Posef posef;
		if (!OVRPlugin.TryLocateSpace(floor.Handle, OVRPlugin.GetTrackingOriginType(), out posef))
		{
			return false;
		}
		OVRPlugin.Posef posef2;
		if (!OVRPlugin.TryLocateSpace(ceiling.Handle, OVRPlugin.GetTrackingOriginType(), out posef2))
		{
			return false;
		}
		int length;
		if (!OVRPlugin.GetSpaceBoundary2DCount(floor.Handle, out length))
		{
			return false;
		}
		bool result;
		using (NativeArray<Vector2> nativeArray = new NativeArray<Vector2>(length, Allocator.Temp, NativeArrayOptions.ClearMemory))
		{
			if (!OVRPlugin.GetSpaceBoundary2D(floor.Handle, nativeArray))
			{
				result = false;
			}
			else if (userPosition.y < posef.Position.y)
			{
				result = false;
			}
			else if (userPosition.y > posef2.Position.y)
			{
				result = false;
			}
			else
			{
				Vector3 point = userPosition - posef.Position.FromVector3f();
				Vector3 v = Quaternion.Inverse(posef.Orientation.FromQuatf()) * point;
				result = OVRSceneManager.PointInPolygon2D(nativeArray, v);
			}
		}
		return result;
	}

	private void DestroyExistingAnchors()
	{
		List<OVRSceneAnchor> list;
		using (new OVRObjectPool.ListScope<OVRSceneAnchor>(ref list))
		{
			OVRSceneAnchor.GetSceneAnchors(list);
			foreach (OVRSceneAnchor ovrsceneAnchor in list)
			{
				Object.Destroy(ovrsceneAnchor.gameObject);
			}
		}
		this.RoomLayout = null;
	}

	public bool RequestSceneCapture()
	{
		bool flag = OVRPlugin.RequestSceneCapture(out this._sceneCaptureRequestId);
		if (!flag)
		{
			Action unexpectedErrorWithSceneCapture = this.UnexpectedErrorWithSceneCapture;
			if (unexpectedErrorWithSceneCapture == null)
			{
				return flag;
			}
			unexpectedErrorWithSceneCapture();
		}
		return flag;
	}

	public OVRTask<bool> DoesRoomSetupExist(IEnumerable<string> requestedAnchorClassifications)
	{
		OVRTask<bool> task = OVRTask.FromGuid<bool>(Guid.NewGuid());
		OVRSceneManager.CheckIfClassificationsAreValid(requestedAnchorClassifications);
		List<OVRAnchor> list;
		using (new OVRObjectPool.ListScope<OVRAnchor>(ref list))
		{
			OVRSceneManager.FetchAnchorsAsync<OVRRoomLayout>(list, null).ContinueWith<List<OVRAnchor>>(delegate(bool result, List<OVRAnchor> anchors)
			{
				OVRSceneManager.CheckClassificationsInRooms(result, anchors, requestedAnchorClassifications, task);
			}, list);
		}
		return task;
	}

	private static void CheckIfClassificationsAreValid(IEnumerable<string> requestedAnchorClassifications)
	{
		if (requestedAnchorClassifications == null)
		{
			throw new ArgumentNullException("requestedAnchorClassifications");
		}
		foreach (string text in requestedAnchorClassifications)
		{
			if (!OVRSceneManager.Classification.Set.Contains(text))
			{
				throw new ArgumentException("requestedAnchorClassifications contains invalid anchor Classification " + text + ".");
			}
		}
	}

	private static void GetUuidsToQuery(OVRAnchor anchor, HashSet<Guid> uuidsToQuery)
	{
		OVRAnchorContainer ovranchorContainer;
		if (anchor.TryGetComponent<OVRAnchorContainer>(out ovranchorContainer))
		{
			foreach (Guid item in ovranchorContainer.Uuids)
			{
				uuidsToQuery.Add(item);
			}
		}
	}

	private static void CheckClassificationsInRooms(bool success, List<OVRAnchor> rooms, IEnumerable<string> requestedAnchorClassifications, OVRTask<bool> task)
	{
		if (!success)
		{
			return;
		}
		HashSet<Guid> hashSet;
		using (new OVRObjectPool.HashSetScope<Guid>(ref hashSet))
		{
			List<Guid> list;
			using (new OVRObjectPool.ListScope<Guid>(ref list))
			{
				for (int i = 0; i < rooms.Count; i++)
				{
					OVRSceneManager.GetUuidsToQuery(rooms[i], hashSet);
					list.AddRange(hashSet);
					hashSet.Clear();
				}
				List<OVRAnchor> roomAnchors;
				using (new OVRObjectPool.ListScope<OVRAnchor>(ref roomAnchors))
				{
					OVRSceneManager.FetchAnchorsAsync(list, roomAnchors).ContinueWith(delegate(bool result)
					{
						OVRSceneManager.CheckIfAnchorsContainClassifications(result, roomAnchors, requestedAnchorClassifications, task);
					});
				}
			}
		}
	}

	private static void CheckIfAnchorsContainClassifications(bool success, List<OVRAnchor> roomAnchors, IEnumerable<string> requestedAnchorClassifications, OVRTask<bool> task)
	{
		if (!success)
		{
			return;
		}
		List<string> list;
		using (new OVRObjectPool.ListScope<string>(ref list))
		{
			OVRSceneManager.CollectLabelsFromAnchors(roomAnchors, list);
			foreach (string item in requestedAnchorClassifications)
			{
				int num = list.IndexOf(item);
				if (num < 0)
				{
					task.SetResult(false);
					return;
				}
				list.RemoveAt(num);
			}
		}
		task.SetResult(true);
	}

	private static void CollectLabelsFromAnchors(List<OVRAnchor> anchors, List<string> labels)
	{
		for (int i = 0; i < anchors.Count; i++)
		{
			OVRSemanticLabels ovrsemanticLabels;
			if (anchors[i].TryGetComponent<OVRSemanticLabels>(out ovrsemanticLabels))
			{
				labels.AddRange(ovrsemanticLabels.Labels.Split(',', StringSplitOptions.None));
			}
		}
	}

	private static void OnTrackingSpaceChanged(Transform trackingSpace)
	{
		OVRSceneManager.UpdateAllSceneAnchors();
	}

	private void Update()
	{
		this.UpdateSomeSceneAnchors();
	}

	private static void UpdateAllSceneAnchors()
	{
		foreach (OVRSceneAnchor ovrsceneAnchor in OVRSceneAnchor.SceneAnchors.Values)
		{
			ovrsceneAnchor.TryUpdateTransform(true);
			OVRScenePlane ovrscenePlane;
			if (ovrsceneAnchor.TryGetComponent<OVRScenePlane>(out ovrscenePlane))
			{
				ovrscenePlane.UpdateTransform();
				ovrscenePlane.RequestBoundary();
			}
			OVRSceneVolume ovrsceneVolume;
			if (ovrsceneAnchor.TryGetComponent<OVRSceneVolume>(out ovrsceneVolume))
			{
				ovrsceneVolume.UpdateTransform();
			}
		}
	}

	private void UpdateSomeSceneAnchors()
	{
		for (int i = 0; i < Math.Min(OVRSceneAnchor.SceneAnchorsList.Count, this.MaxSceneAnchorUpdatesPerFrame); i++)
		{
			this._sceneAnchorUpdateIndex %= OVRSceneAnchor.SceneAnchorsList.Count;
			List<OVRSceneAnchor> sceneAnchorsList = OVRSceneAnchor.SceneAnchorsList;
			int sceneAnchorUpdateIndex = this._sceneAnchorUpdateIndex;
			this._sceneAnchorUpdateIndex = sceneAnchorUpdateIndex + 1;
			sceneAnchorsList[sceneAnchorUpdateIndex].TryUpdateTransform(false);
		}
	}

	private OVRSceneManager.RoomLayoutInformation GetRoomLayoutInformation()
	{
		OVRSceneManager.RoomLayoutInformation roomLayoutInformation = new OVRSceneManager.RoomLayoutInformation();
		if (OVRSceneRoom.SceneRoomsList.Count > 0)
		{
			roomLayoutInformation.Floor = OVRSceneRoom.SceneRoomsList[0].Floor;
			roomLayoutInformation.Ceiling = OVRSceneRoom.SceneRoomsList[0].Ceiling;
			roomLayoutInformation.Walls.Clear();
			roomLayoutInformation.Walls.AddRange(OVRSceneRoom.SceneRoomsList[0].Walls);
		}
		return roomLayoutInformation;
	}

	private void OnEnable()
	{
		OVRManager.SceneCaptureComplete += this.OVRManager_SceneCaptureComplete;
		if (OVRManager.display != null)
		{
			OVRManager.display.RecenteredPose += OVRSceneManager.UpdateAllSceneAnchors;
		}
		if (!this._cameraRig)
		{
			this._cameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
		}
		if (this._cameraRig)
		{
			this._cameraRig.TrackingSpaceChanged += OVRSceneManager.OnTrackingSpaceChanged;
		}
	}

	private void OnDisable()
	{
		OVRManager.SceneCaptureComplete -= this.OVRManager_SceneCaptureComplete;
		if (OVRManager.display != null)
		{
			OVRManager.display.RecenteredPose -= OVRSceneManager.UpdateAllSceneAnchors;
		}
		if (this._cameraRig)
		{
			this._cameraRig.TrackingSpaceChanged -= OVRSceneManager.OnTrackingSpaceChanged;
		}
	}

	internal static bool PointInPolygon2D(NativeArray<Vector2> boundaryVertices, Vector2 target)
	{
		if (boundaryVertices.Length < 3)
		{
			return false;
		}
		int num = 0;
		float x = target.x;
		float y = target.y;
		for (int i = 0; i < boundaryVertices.Length; i++)
		{
			float x2 = boundaryVertices[i].x;
			float y2 = boundaryVertices[i].y;
			float x3 = boundaryVertices[(i + 1) % boundaryVertices.Length].x;
			float y3 = boundaryVertices[(i + 1) % boundaryVertices.Length].y;
			if (y < y2 != y < y3 && x < x2 + (y - y2) / (y3 - y2) * (x3 - x2))
			{
				num += ((y2 < y3) ? 1 : -1);
			}
		}
		return num != 0;
	}

	private void OVRManager_SceneCaptureComplete(ulong requestId, bool result)
	{
		if (requestId != this._sceneCaptureRequestId)
		{
			if (this.Verbose == null)
			{
				return;
			}
			OVRSceneManager.LogForwarder? logForwarder;
			logForwarder.GetValueOrDefault().LogWarning("OVRSceneManager", string.Format("Scene Room Setup with requestId: [{0}] was ignored, as it was not issued by this Scene Load request.", requestId), null);
			return;
		}
		else if (result)
		{
			Action sceneCaptureReturnedWithoutError = this.SceneCaptureReturnedWithoutError;
			if (sceneCaptureReturnedWithoutError == null)
			{
				return;
			}
			sceneCaptureReturnedWithoutError();
			return;
		}
		else
		{
			Action unexpectedErrorWithSceneCapture = this.UnexpectedErrorWithSceneCapture;
			if (unexpectedErrorWithSceneCapture == null)
			{
				return;
			}
			unexpectedErrorWithSceneCapture();
			return;
		}
	}

	internal OVRSceneAnchor InstantiateSceneAnchor(OVRAnchor anchor, OVRSceneAnchor prefab)
	{
		OVRSpace ovrspace = anchor.Handle;
		Guid uuid = anchor.Uuid;
		string text;
		string[] array = OVRPlugin.GetSpaceSemanticLabels(ovrspace, out text) ? text.Split(',', StringSplitOptions.None) : Array.Empty<string>();
		if (this.PrefabOverrides.Count > 0)
		{
			foreach (string text2 in array)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					foreach (OVRScenePrefabOverride ovrscenePrefabOverride in this.PrefabOverrides)
					{
						if (ovrscenePrefabOverride.ClassificationLabel == text2)
						{
							prefab = ovrscenePrefabOverride.Prefab;
							break;
						}
					}
				}
			}
		}
		if (prefab == null)
		{
			if (this.Verbose != null)
			{
				OVRSceneManager.LogForwarder? logForwarder;
				logForwarder.GetValueOrDefault().Log("OVRSceneManager", string.Format("No prefab was provided for space: [{0}]", ovrspace) + ((array.Length != 0) ? (" with semantic label " + array[0]) : ""), null);
			}
			return null;
		}
		OVRSceneAnchor ovrsceneAnchor = Object.Instantiate<OVRSceneAnchor>(prefab, Vector3.zero, Quaternion.identity, this._initialAnchorParent);
		ovrsceneAnchor.gameObject.SetActive(true);
		ovrsceneAnchor.Initialize(anchor);
		return ovrsceneAnchor;
	}

	[CompilerGenerated]
	private void <LoadSceneModel>g__AwaitTask|40_0(OVRTask<OVRSceneManager.LoadSceneModelResult> task)
	{
		OVRSceneManager.<<LoadSceneModel>g__AwaitTask|40_0>d <<LoadSceneModel>g__AwaitTask|40_0>d;
		<<LoadSceneModel>g__AwaitTask|40_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
		<<LoadSceneModel>g__AwaitTask|40_0>d.<>4__this = this;
		<<LoadSceneModel>g__AwaitTask|40_0>d.task = task;
		<<LoadSceneModel>g__AwaitTask|40_0>d.<>1__state = -1;
		<<LoadSceneModel>g__AwaitTask|40_0>d.<>t__builder.Start<OVRSceneManager.<<LoadSceneModel>g__AwaitTask|40_0>d>(ref <<LoadSceneModel>g__AwaitTask|40_0>d);
	}

	[CompilerGenerated]
	private bool <LoadSceneModel>g__InterpretResult|40_1(OVRSceneManager.LoadSceneModelResult result)
	{
		switch (result)
		{
		case OVRSceneManager.LoadSceneModelResult.FailureScenePermissionNotGranted:
		{
			Action loadSceneModelFailedPermissionNotGranted = this.LoadSceneModelFailedPermissionNotGranted;
			if (loadSceneModelFailedPermissionNotGranted != null)
			{
				loadSceneModelFailedPermissionNotGranted();
			}
			return true;
		}
		case OVRSceneManager.LoadSceneModelResult.Success:
		{
			Action sceneModelLoadedSuccessfully = this.SceneModelLoadedSuccessfully;
			if (sceneModelLoadedSuccessfully != null)
			{
				sceneModelLoadedSuccessfully();
			}
			return true;
		}
		case OVRSceneManager.LoadSceneModelResult.NoSceneModelToLoad:
		{
			Action noSceneModelToLoad = this.NoSceneModelToLoad;
			if (noSceneModelToLoad != null)
			{
				noSceneModelToLoad();
			}
			return true;
		}
		default:
			return false;
		}
	}

	internal const string DeprecationMessage = "OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)";

	[FormerlySerializedAs("planePrefab")]
	[Tooltip("A prefab that will be used to instantiate any Plane found when querying the Scene model. If the anchor contains both Volume and Plane elements, Volume will be used instead.")]
	public OVRSceneAnchor PlanePrefab;

	[FormerlySerializedAs("volumePrefab")]
	[Tooltip("A prefab that will be used to instantiate any Volume found when querying the Scene model. This anchor may also contain Plane elements.")]
	public OVRSceneAnchor VolumePrefab;

	[FormerlySerializedAs("prefabOverrides")]
	[Tooltip("Overrides the instantiation of the generic Plane/Volume prefabs with specialized ones.")]
	public List<OVRScenePrefabOverride> PrefabOverrides = new List<OVRScenePrefabOverride>();

	[Tooltip("Scene manager will only present the room(s) the user is currently in.")]
	public bool ActiveRoomsOnly = true;

	[FormerlySerializedAs("verboseLogging")]
	[Tooltip("When enabled, verbose debug logs will be emitted.")]
	public bool VerboseLogging;

	[Tooltip("The maximum number of scene anchors that will be updated each frame.")]
	public int MaxSceneAnchorUpdatesPerFrame = 3;

	[SerializeField]
	[Tooltip("(Optional) The parent transform for each new scene anchor. Changing this value does not affect existing scene anchors. May be null.")]
	internal Transform _initialAnchorParent;

	public Action SceneModelLoadedSuccessfully;

	public Action NoSceneModelToLoad;

	public Action SceneCaptureReturnedWithoutError;

	public Action UnexpectedErrorWithSceneCapture;

	public Action NewSceneModelAvailable;

	[Obsolete("RoomLayout is obsoleted. For each room's layout information (floor, ceiling, walls) see OVRSceneRoom.", false)]
	public OVRSceneManager.RoomLayoutInformation RoomLayout;

	private ulong _sceneCaptureRequestId = ulong.MaxValue;

	private OVRCameraRig _cameraRig;

	private int _sceneAnchorUpdateIndex;

	private bool _hasLoadBeenRequested;

	[Obsolete("OVRSceneManager and associated classes are deprecated (v65), please use MR Utility Kit instead (https://developer.oculus.com/documentation/unity/unity-mr-utility-kit-overview)")]
	public static class Classification
	{
		public static IReadOnlyList<string> List { get; } = new string[]
		{
			"FLOOR",
			"CEILING",
			"WALL_FACE",
			"DESK",
			"COUCH",
			"DOOR_FRAME",
			"WINDOW_FRAME",
			"OTHER",
			"STORAGE",
			"BED",
			"SCREEN",
			"LAMP",
			"PLANT",
			"TABLE",
			"WALL_ART",
			"INVISIBLE_WALL_FACE",
			"GLOBAL_MESH"
		};

		public static HashSet<string> Set { get; } = new HashSet<string>(OVRSceneManager.Classification.List);

		public const string Floor = "FLOOR";

		public const string Ceiling = "CEILING";

		public const string WallFace = "WALL_FACE";

		[Obsolete("Deprecated. Use Table classification instead.")]
		public const string Desk = "DESK";

		public const string Couch = "COUCH";

		public const string DoorFrame = "DOOR_FRAME";

		public const string WindowFrame = "WINDOW_FRAME";

		public const string Other = "OTHER";

		public const string Storage = "STORAGE";

		public const string Bed = "BED";

		public const string Screen = "SCREEN";

		public const string Lamp = "LAMP";

		public const string Plant = "PLANT";

		public const string Table = "TABLE";

		public const string WallArt = "WALL_ART";

		public const string InvisibleWallFace = "INVISIBLE_WALL_FACE";

		public const string GlobalMesh = "GLOBAL_MESH";
	}

	[Obsolete("RoomLayoutInformation is obsoleted. For each room's layout information (floor, ceiling, walls) see OVRSceneRoom.", false)]
	public class RoomLayoutInformation
	{
		public OVRScenePlane Floor;

		public OVRScenePlane Ceiling;

		public List<OVRScenePlane> Walls = new List<OVRScenePlane>();
	}

	internal struct LogForwarder
	{
		public void Log(string context, string message, GameObject gameObject = null)
		{
			Debug.Log("[" + context + "] " + message, gameObject);
		}

		public void LogWarning(string context, string message, GameObject gameObject = null)
		{
			Debug.LogWarning("[" + context + "] " + message, gameObject);
		}

		public void LogError(string context, string message, GameObject gameObject = null)
		{
			Debug.LogError("[" + context + "] " + message, gameObject);
		}
	}

	internal static class Development
	{
		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void Log(string context, string message, GameObject gameObject = null)
		{
			Debug.Log("[" + context + "] " + message, gameObject);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void LogWarning(string context, string message, GameObject gameObject = null)
		{
			Debug.LogWarning("[" + context + "] " + message, gameObject);
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		public static void LogError(string context, string message, GameObject gameObject = null)
		{
			Debug.LogError("[" + context + "] " + message, gameObject);
		}
	}

	public enum LoadSceneModelResult
	{
		Success,
		NoSceneModelToLoad,
		FailureScenePermissionNotGranted = -1,
		FailureUnexpectedError = -2
	}

	internal struct Metrics
	{
		public static OVRSceneManager.Metrics operator +(OVRSceneManager.Metrics lhs, OVRSceneManager.Metrics rhs)
		{
			return new OVRSceneManager.Metrics
			{
				TotalRoomCount = lhs.TotalRoomCount + rhs.TotalRoomCount,
				CandidateRoomCount = lhs.CandidateRoomCount + rhs.CandidateRoomCount,
				Loaded = lhs.Loaded + rhs.Loaded,
				Failed = lhs.Failed + rhs.Failed,
				SkippedUserNotInRoom = lhs.SkippedUserNotInRoom + rhs.SkippedUserNotInRoom,
				SkippedAlreadyInstantiated = lhs.SkippedAlreadyInstantiated + rhs.SkippedAlreadyInstantiated
			};
		}

		public int TotalRoomCount;

		public int CandidateRoomCount;

		public int Loaded;

		public int Failed;

		public int SkippedUserNotInRoom;

		public int SkippedAlreadyInstantiated;
	}

	internal struct RoomLayoutUuids
	{
		public Guid Floor;

		public Guid Ceiling;

		public Guid[] Walls;
	}
}
