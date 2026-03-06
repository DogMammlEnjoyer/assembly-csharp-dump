using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AOT;
using Meta.XR.ImmersiveDebugger;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_m_r_u_k")]
	[Feature(Feature.Scene)]
	public class MRUK : MonoBehaviour
	{
		public bool IsInitialized { get; private set; }

		public UnityEvent SceneLoadedEvent { get; private set; } = new UnityEvent();

		public UnityEvent<MRUKRoom> RoomCreatedEvent { get; private set; } = new UnityEvent<MRUKRoom>();

		public UnityEvent<MRUKRoom> RoomUpdatedEvent { get; private set; } = new UnityEvent<MRUKRoom>();

		public UnityEvent<MRUKRoom> RoomRemovedEvent { get; private set; } = new UnityEvent<MRUKRoom>();

		public bool IsWorldLockActive
		{
			get
			{
				return this.EnableWorldLock && this._worldLockActive;
			}
		}

		internal OVRCameraRig _cameraRig { get; private set; }

		private void InitializeScene()
		{
			try
			{
				this.SceneLoadedEvent.Invoke();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			this.IsInitialized = true;
		}

		public void RegisterSceneLoadedCallback(UnityAction callback)
		{
			this.SceneLoadedEvent.AddListener(callback);
			if (this.IsInitialized)
			{
				callback();
			}
		}

		[Obsolete("Use UnityEvent RoomCreatedEvent directly instead")]
		public void RegisterRoomCreatedCallback(UnityAction<MRUKRoom> callback)
		{
			this.RoomCreatedEvent.AddListener(callback);
		}

		[Obsolete("Use UnityEvent RoomUpdatedEvent directly instead")]
		public void RegisterRoomUpdatedCallback(UnityAction<MRUKRoom> callback)
		{
			this.RoomUpdatedEvent.AddListener(callback);
		}

		[Obsolete("Use UnityEvent RoomRemovedEvent directly instead")]
		public void RegisterRoomRemovedCallback(UnityAction<MRUKRoom> callback)
		{
			this.RoomRemovedEvent.AddListener(callback);
		}

		[Obsolete("Use Rooms property instead")]
		public List<MRUKRoom> GetRooms()
		{
			return this.Rooms;
		}

		[Obsolete("Use GetCurrentRoom().Anchors instead")]
		public List<MRUKAnchor> GetAnchors()
		{
			return this.GetCurrentRoom().Anchors;
		}

		public MRUKRoom GetCurrentRoom()
		{
			if (this._cachedCurrentRoomFrame != Time.frameCount)
			{
				OVRCameraRig cameraRig = this._cameraRig;
				Vector3? vector = (cameraRig != null) ? new Vector3?(cameraRig.centerEyeAnchor.position) : null;
				if (vector != null)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					MRUKRoom mrukroom = null;
					foreach (MRUKRoom mrukroom2 in this.Rooms)
					{
						if (mrukroom2.IsPositionInRoom(valueOrDefault, false))
						{
							mrukroom = mrukroom2;
							if (mrukroom2.IsLocal)
							{
								break;
							}
						}
					}
					if (mrukroom != null)
					{
						this._cachedCurrentRoom = mrukroom;
						this._cachedCurrentRoomFrame = Time.frameCount;
						return mrukroom;
					}
				}
			}
			if (this._cachedCurrentRoom != null)
			{
				return this._cachedCurrentRoom;
			}
			if (this.Rooms.Count > 0)
			{
				return this.Rooms[0];
			}
			return null;
		}

		public static Task<bool> HasSceneModel()
		{
			MRUK.<HasSceneModel>d__48 <HasSceneModel>d__;
			<HasSceneModel>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<HasSceneModel>d__.<>1__state = -1;
			<HasSceneModel>d__.<>t__builder.Start<MRUK.<HasSceneModel>d__48>(ref <HasSceneModel>d__);
			return <HasSceneModel>d__.<>t__builder.Task;
		}

		public List<MRUKRoom> Rooms { get; } = new List<MRUKRoom>();

		public static MRUK Instance { get; private set; }

		private void Awake()
		{
			this._cameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
			if (MRUK.Instance != null && MRUK.Instance != this)
			{
				Object.Destroy(this);
			}
			else
			{
				MRUK.Instance = this;
			}
			MRUKNative.LoadMRUKSharedLibrary();
			MRUKNativeFuncs.SetLogPrinter(new MRUKNativeFuncs.LogPrinter(MRUK.OnSharedLibLog));
			this.InitializeAnchorStore();
			if (this.SceneSettings != null && this.SceneSettings.LoadSceneOnStartup)
			{
				this.LoadScene(this.SceneSettings.DataSource);
			}
			if (RuntimeSettings.Instance.ImmersiveDebuggerEnabled && this._immersiveSceneDebuggerPrefab != null && !(ImmersiveSceneDebugger.Instance != null))
			{
				Object.Instantiate<GameObject>(this._immersiveSceneDebuggerPrefab);
			}
		}

		private void OnDestroy()
		{
			if (MRUK.Instance == this)
			{
				this.DestroyAnchorStore();
				MRUKNative.FreeMRUKSharedLibrary();
				MRUK.Instance = null;
				this.RoomCreatedEvent.RemoveAllListeners();
				this.RoomRemovedEvent.RemoveAllListeners();
				this.RoomUpdatedEvent.RemoveAllListeners();
				this.SceneLoadedEvent.RemoveAllListeners();
			}
		}

		private void Start()
		{
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.LogPrinter))]
		private unsafe static void OnSharedLibLog(MRUKNativeFuncs.MrukLogLevel logLevel, char* message, uint length)
		{
			try
			{
				LogType logType = LogType.Log;
				switch (logLevel)
				{
				case MRUKNativeFuncs.MrukLogLevel.Debug:
				case MRUKNativeFuncs.MrukLogLevel.Info:
					logType = LogType.Log;
					break;
				case MRUKNativeFuncs.MrukLogLevel.Warn:
					logType = LogType.Warning;
					break;
				case MRUKNativeFuncs.MrukLogLevel.Error:
					logType = LogType.Error;
					break;
				}
				Debug.LogFormat(logType, LogOption.None, null, "MRUK Shared: {0}", new object[]
				{
					Marshal.PtrToStringUTF8((IntPtr)((void*)message), (int)length)
				});
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.TrackingSpacePoseGetter))]
		private static Pose GetTrackingSpacePose()
		{
			Transform trackingSpace = MRUK.GetTrackingSpace();
			Pose lhs = MRUK.FlipZRotateY180((trackingSpace != null) ? new Pose(trackingSpace.position, trackingSpace.rotation) : Pose.identity);
			return MRUK.FlipZRotateY180(Pose.identity).GetTransformedBy(lhs);
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.TrackingSpacePoseSetter))]
		private static void SetTrackingSpacePose(Pose openXrPose)
		{
			Pose pose = MRUK.FlipZRotateY180(MRUK.FlipZRotateY180(Pose.identity).GetTransformedBy(openXrPose));
			Transform trackingSpace = MRUK.GetTrackingSpace();
			if (trackingSpace == null)
			{
				return;
			}
			trackingSpace.SetPositionAndRotation(pose.position, pose.rotation);
		}

		private static Transform GetTrackingSpace()
		{
			if (MRUK.Instance != null && MRUK.Instance._cameraRig != null)
			{
				return MRUK.Instance._cameraRig.trackingSpace;
			}
			Debug.LogError("OVRCameraRig is not present, but MRUK requires it. Please add OVRCameraRig to your scene via 'Meta / Tools / Building Blocks / Camera Rig'.");
			return null;
		}

		private void Update()
		{
			if (this.SceneSettings.LoadSceneOnStartup)
			{
				bool loadSceneCalled = this._loadSceneCalled;
			}
			this.UpdateAnchorStore();
			bool worldLockActive = false;
			if (this._cameraRig)
			{
				if (this.EnableWorldLock)
				{
					MRUKRoom currentRoom = this.GetCurrentRoom();
					if (currentRoom)
					{
						Pose pose = Pose.identity;
						if (MRUKNativeFuncs.AnchorStoreGetWorldLockOffset(currentRoom.Anchor.Uuid, ref pose))
						{
							Pose? prevTrackingSpacePose = this._prevTrackingSpacePose;
							if (prevTrackingSpacePose != null)
							{
								Pose valueOrDefault = prevTrackingSpacePose.GetValueOrDefault();
								if (this._cameraRig.trackingSpace.position != valueOrDefault.position || this._cameraRig.trackingSpace.rotation != valueOrDefault.rotation)
								{
									Debug.LogWarning("MRUK EnableWorldLock is enabled and is controlling the tracking space position.\n" + string.Format("Tracking position was set to {0} and rotation to {1}, this is being overridden by MRUK.\n", this._cameraRig.trackingSpace.position, this._cameraRig.trackingSpace.rotation) + "Use 'TrackingSpaceOffset' instead to translate or rotate the TrackingSpace.");
								}
							}
							pose = MRUK.FlipZ(pose);
							Pose pose2;
							if (currentRoom.FloorAnchor != null && currentRoom.FloorAnchor.HasValidHandle)
							{
								pose2 = currentRoom.FloorAnchor.DeltaPose;
							}
							else
							{
								pose2 = currentRoom.DeltaPose;
							}
							pose2 = pose.GetTransformedBy(pose2);
							Vector3 position = this.TrackingSpaceOffset.MultiplyPoint3x4(pose2.position);
							Quaternion rotation = this.TrackingSpaceOffset.rotation * pose2.rotation;
							this._cameraRig.trackingSpace.SetPositionAndRotation(position, rotation);
							this._prevTrackingSpacePose = new Pose?(new Pose(position, rotation));
							worldLockActive = true;
						}
					}
				}
				else if (this._worldLockWasEnabled)
				{
					this._cameraRig.trackingSpace.localPosition = Vector3.zero;
					this._cameraRig.trackingSpace.localRotation = Quaternion.identity;
					this._prevTrackingSpacePose = null;
				}
				this._worldLockWasEnabled = this.EnableWorldLock;
			}
			this._worldLockActive = worldLockActive;
			this.UpdateTrackables();
		}

		internal Task LoadScene(MRUK.SceneDataSource dataSource)
		{
			MRUK.<LoadScene>d__69 <LoadScene>d__;
			<LoadScene>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadScene>d__.<>4__this = this;
			<LoadScene>d__.dataSource = dataSource;
			<LoadScene>d__.<>1__state = -1;
			<LoadScene>d__.<>t__builder.Start<MRUK.<LoadScene>d__69>(ref <LoadScene>d__);
			return <LoadScene>d__.<>t__builder.Task;
		}

		private int GetRoomIndex(bool fromPrefabs = true)
		{
			int num = this.SceneSettings.RoomIndex;
			if (num == -1)
			{
				num = Random.Range(0, fromPrefabs ? this.SceneSettings.RoomPrefabs.Length : this.SceneSettings.SceneJsons.Length);
			}
			return num;
		}

		internal void OnRoomDestroyed(MRUKRoom room)
		{
			this.Rooms.Remove(room);
			if (this._cachedCurrentRoom == room)
			{
				this._cachedCurrentRoom = null;
			}
		}

		public void ClearScene()
		{
			this.ClearSceneSharedLib();
		}

		public Task<MRUK.LoadDeviceResult> LoadSceneFromSharedRooms(IEnumerable<Guid> roomUuids, Guid groupUuid, [TupleElementNames(new string[]
		{
			"alignmentRoomUuid",
			"floorWorldPoseOnHost"
		})] ValueTuple<Guid, Pose>? alignmentData, bool removeMissingRooms = true)
		{
			MRUK.<LoadSceneFromSharedRooms>d__73 <LoadSceneFromSharedRooms>d__;
			<LoadSceneFromSharedRooms>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromSharedRooms>d__.<>4__this = this;
			<LoadSceneFromSharedRooms>d__.roomUuids = roomUuids;
			<LoadSceneFromSharedRooms>d__.groupUuid = groupUuid;
			<LoadSceneFromSharedRooms>d__.alignmentData = alignmentData;
			<LoadSceneFromSharedRooms>d__.removeMissingRooms = removeMissingRooms;
			<LoadSceneFromSharedRooms>d__.<>1__state = -1;
			<LoadSceneFromSharedRooms>d__.<>t__builder.Start<MRUK.<LoadSceneFromSharedRooms>d__73>(ref <LoadSceneFromSharedRooms>d__);
			return <LoadSceneFromSharedRooms>d__.<>t__builder.Task;
		}

		public Task<MRUK.LoadDeviceResult> LoadSceneFromSharedRooms(Guid groupUuid, [TupleElementNames(new string[]
		{
			"alignmentRoomUuid",
			"floorWorldPoseOnHost"
		})] ValueTuple<Guid, Pose>? alignmentData, bool removeMissingRooms = true)
		{
			return this.LoadSceneFromSharedRooms(null, groupUuid, alignmentData, removeMissingRooms);
		}

		public OVRTask<OVRResult<OVRAnchor.ShareResult>> ShareRoomsAsync(IEnumerable<MRUKRoom> rooms, Guid groupUuid)
		{
			MRUK.<ShareRoomsAsync>d__76 <ShareRoomsAsync>d__;
			<ShareRoomsAsync>d__.<>t__builder = OVRTaskBuilder<OVRResult<OVRAnchor.ShareResult>>.Create();
			<ShareRoomsAsync>d__.rooms = rooms;
			<ShareRoomsAsync>d__.groupUuid = groupUuid;
			<ShareRoomsAsync>d__.<>1__state = -1;
			<ShareRoomsAsync>d__.<>t__builder.Start<MRUK.<ShareRoomsAsync>d__76>(ref <ShareRoomsAsync>d__);
			return <ShareRoomsAsync>d__.<>t__builder.Task;
		}

		public Task<MRUK.LoadDeviceResult> LoadSceneFromDevice(bool requestSceneCaptureIfNoDataFound = true, bool removeMissingRooms = true)
		{
			MRUK.<LoadSceneFromDevice>d__77 <LoadSceneFromDevice>d__;
			<LoadSceneFromDevice>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromDevice>d__.<>4__this = this;
			<LoadSceneFromDevice>d__.requestSceneCaptureIfNoDataFound = requestSceneCaptureIfNoDataFound;
			<LoadSceneFromDevice>d__.removeMissingRooms = removeMissingRooms;
			<LoadSceneFromDevice>d__.<>1__state = -1;
			<LoadSceneFromDevice>d__.<>t__builder.Start<MRUK.<LoadSceneFromDevice>d__77>(ref <LoadSceneFromDevice>d__);
			return <LoadSceneFromDevice>d__.<>t__builder.Task;
		}

		private Task<MRUK.LoadDeviceResult> LoadSceneFromDeviceInternal(bool requestSceneCaptureIfNoDataFound, bool removeMissingRooms, MRUK.SharedRoomsData? sharedRoomsData = null)
		{
			MRUK.<LoadSceneFromDeviceInternal>d__78 <LoadSceneFromDeviceInternal>d__;
			<LoadSceneFromDeviceInternal>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromDeviceInternal>d__.<>4__this = this;
			<LoadSceneFromDeviceInternal>d__.requestSceneCaptureIfNoDataFound = requestSceneCaptureIfNoDataFound;
			<LoadSceneFromDeviceInternal>d__.removeMissingRooms = removeMissingRooms;
			<LoadSceneFromDeviceInternal>d__.sharedRoomsData = sharedRoomsData;
			<LoadSceneFromDeviceInternal>d__.<>1__state = -1;
			<LoadSceneFromDeviceInternal>d__.<>t__builder.Start<MRUK.<LoadSceneFromDeviceInternal>d__78>(ref <LoadSceneFromDeviceInternal>d__);
			return <LoadSceneFromDeviceInternal>d__.<>t__builder.Task;
		}

		private void FindAllObjects(GameObject roomPrefab, out List<GameObject> walls, out List<GameObject> volumes, out List<GameObject> planes)
		{
			walls = new List<GameObject>();
			volumes = new List<GameObject>();
			planes = new List<GameObject>();
			this.FindObjects(MRUKAnchor.SceneLabels.WALL_FACE.ToString(), roomPrefab.transform, ref walls);
			this.FindObjects(MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE.ToString(), roomPrefab.transform, ref walls);
			this.FindObjects(MRUKAnchor.SceneLabels.OTHER.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.TABLE.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.COUCH.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.WINDOW_FRAME.ToString(), roomPrefab.transform, ref planes);
			this.FindObjects(MRUKAnchor.SceneLabels.DOOR_FRAME.ToString(), roomPrefab.transform, ref planes);
			this.FindObjects(MRUKAnchor.SceneLabels.WALL_ART.ToString(), roomPrefab.transform, ref planes);
			this.FindObjects(MRUKAnchor.SceneLabels.PLANT.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.SCREEN.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.BED.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.LAMP.ToString(), roomPrefab.transform, ref volumes);
			this.FindObjects(MRUKAnchor.SceneLabels.STORAGE.ToString(), roomPrefab.transform, ref volumes);
		}

		public Task<MRUK.LoadDeviceResult> LoadSceneFromPrefab(GameObject scenePrefab, bool clearSceneFirst = true)
		{
			MRUK.<LoadSceneFromPrefab>d__80 <LoadSceneFromPrefab>d__;
			<LoadSceneFromPrefab>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromPrefab>d__.<>4__this = this;
			<LoadSceneFromPrefab>d__.scenePrefab = scenePrefab;
			<LoadSceneFromPrefab>d__.clearSceneFirst = clearSceneFirst;
			<LoadSceneFromPrefab>d__.<>1__state = -1;
			<LoadSceneFromPrefab>d__.<>t__builder.Start<MRUK.<LoadSceneFromPrefab>d__80>(ref <LoadSceneFromPrefab>d__);
			return <LoadSceneFromPrefab>d__.<>t__builder.Task;
		}

		[Obsolete("Coordinate system is now obsolete, use the overload that doesn't take this parameter")]
		public string SaveSceneToJsonString(SerializationHelpers.CoordinateSystem coordinateSystem = SerializationHelpers.CoordinateSystem.Unity, bool includeGlobalMesh = true, List<MRUKRoom> rooms = null)
		{
			return this.SaveSceneToJsonString(includeGlobalMesh, rooms);
		}

		public string SaveSceneToJsonString(bool includeGlobalMesh = true, List<MRUKRoom> rooms = null)
		{
			return this.SaveSceneToJsonSharedLib(includeGlobalMesh, rooms);
		}

		public Task<MRUK.LoadDeviceResult> LoadSceneFromJsonString(string jsonString, bool removeMissingRooms = true)
		{
			MRUK.<LoadSceneFromJsonString>d__83 <LoadSceneFromJsonString>d__;
			<LoadSceneFromJsonString>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromJsonString>d__.<>4__this = this;
			<LoadSceneFromJsonString>d__.jsonString = jsonString;
			<LoadSceneFromJsonString>d__.removeMissingRooms = removeMissingRooms;
			<LoadSceneFromJsonString>d__.<>1__state = -1;
			<LoadSceneFromJsonString>d__.<>t__builder.Start<MRUK.<LoadSceneFromJsonString>d__83>(ref <LoadSceneFromJsonString>d__);
			return <LoadSceneFromJsonString>d__.<>t__builder.Task;
		}

		private void FindObjects(string objName, Transform rootTransform, ref List<GameObject> objList)
		{
			if (rootTransform.name.Equals(objName))
			{
				objList.Add(rootTransform.gameObject);
			}
			foreach (object obj in rootTransform)
			{
				Transform rootTransform2 = (Transform)obj;
				this.FindObjects(objName, rootTransform2, ref objList);
			}
		}

		private static bool IsOpenXRAvailable
		{
			get
			{
				return OVRPlugin.initialized;
			}
		}

		private void InitializeAnchorStore()
		{
			if (MRUK.IsOpenXRAvailable)
			{
				ulong nativeOpenXRInstance = OVRPlugin.GetNativeOpenXRInstance();
				ulong nativeOpenXRSession = OVRPlugin.GetNativeOpenXRSession();
				IntPtr openXRInstanceProcAddrFunc = OVRPlugin.GetOpenXRInstanceProcAddrFunc();
				this._currentAppSpace = OVRPlugin.GetAppSpace();
				if (MRUKNativeFuncs.AnchorStoreCreate(nativeOpenXRInstance, nativeOpenXRSession, openXRInstanceProcAddrFunc, this._currentAppSpace, null, 0U) != MRUKNativeFuncs.MrukResult.Success)
				{
					Debug.LogError("Failed to create anchor store");
				}
				else
				{
					this._openXrInitialised = true;
				}
				if (OVRPlugin.RegisterOpenXREventHandler(new OVRPlugin.OpenXREventDelegateType(MRUK.OnOpenXrEvent)) != OVRPlugin.Result.Success)
				{
					Debug.LogError("Failed to register OpenXR event handler");
				}
			}
			else
			{
				MRUKNativeFuncs.AnchorStoreCreateWithoutOpenXr();
			}
			MRUKNativeFuncs.MrukEventListener listener;
			listener.userContext = IntPtr.Zero;
			listener.onPreRoomAnchorAdded = new MRUKNativeFuncs.MrukOnPreRoomAnchorAdded(MRUK.OnPreRoomAnchorAdded);
			listener.onRoomAnchorAdded = new MRUKNativeFuncs.MrukOnRoomAnchorAdded(MRUK.OnRoomAnchorAdded);
			listener.onRoomAnchorUpdated = new MRUKNativeFuncs.MrukOnRoomAnchorUpdated(MRUK.OnRoomAnchorUpdated);
			listener.onRoomAnchorRemoved = new MRUKNativeFuncs.MrukOnRoomAnchorRemoved(MRUK.OnRoomAnchorRemoved);
			listener.onSceneAnchorAdded = new MRUKNativeFuncs.MrukOnSceneAnchorAdded(MRUK.OnSceneAnchorAdded);
			listener.onSceneAnchorUpdated = new MRUKNativeFuncs.MrukOnSceneAnchorUpdated(MRUK.OnSceneAnchorUpdated);
			listener.onSceneAnchorRemoved = new MRUKNativeFuncs.MrukOnSceneAnchorRemoved(MRUK.OnSceneAnchorRemoved);
			listener.onDiscoveryFinished = new MRUKNativeFuncs.MrukOnDiscoveryFinished(MRUK.OnDiscoveryFinished);
			listener.onEnvironmentRaycasterCreated = new MRUKNativeFuncs.MrukOnEnvironmentRaycasterCreated(MRUK.OnEnvironmentRaycasterCreated);
			MRUKNativeFuncs.AnchorStoreRegisterEventListener(listener);
			MRUKNativeFuncs.SetTrackingSpacePoseGetter(new MRUKNativeFuncs.TrackingSpacePoseGetter(MRUK.GetTrackingSpacePose));
			MRUKNativeFuncs.SetTrackingSpacePoseSetter(new MRUKNativeFuncs.TrackingSpacePoseSetter(MRUK.SetTrackingSpacePose));
		}

		private void DestroyAnchorStore()
		{
			if (MRUK.IsOpenXRAvailable)
			{
				OVRPlugin.UnregisterOpenXREventHandler(new OVRPlugin.OpenXREventDelegateType(MRUK.OnOpenXrEvent));
			}
			MRUKNativeFuncs.AnchorStoreDestroy();
		}

		private void UpdateAnchorStore()
		{
			if (MRUK.IsOpenXRAvailable)
			{
				ulong appSpace = OVRPlugin.GetAppSpace();
				if (appSpace != this._currentAppSpace)
				{
					MRUKNativeFuncs.AnchorStoreSetBaseSpace(appSpace);
					this._currentAppSpace = appSpace;
				}
			}
			else if (this._openXrInitialised)
			{
				MRUKNativeFuncs.AnchorStoreShutdownOpenXr();
				this._openXrInitialised = false;
			}
			ulong nextPredictedDisplayTime = OVRPlugin.initialized ? ((ulong)(OVRPlugin.GetPredictedDisplayTime() * 1000000000.0)) : 0UL;
			MRUKNativeFuncs.AnchorStoreTick(nextPredictedDisplayTime);
		}

		private Task<MRUK.LoadDeviceResult> LoadSceneFromDeviceSharedLib(bool requestSceneCaptureIfNoDataFound, bool removeMissingRooms, MRUK.SharedRoomsData? sharedRoomsData = null)
		{
			MRUK.<LoadSceneFromDeviceSharedLib>d__93 <LoadSceneFromDeviceSharedLib>d__;
			<LoadSceneFromDeviceSharedLib>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromDeviceSharedLib>d__.<>4__this = this;
			<LoadSceneFromDeviceSharedLib>d__.requestSceneCaptureIfNoDataFound = requestSceneCaptureIfNoDataFound;
			<LoadSceneFromDeviceSharedLib>d__.removeMissingRooms = removeMissingRooms;
			<LoadSceneFromDeviceSharedLib>d__.sharedRoomsData = sharedRoomsData;
			<LoadSceneFromDeviceSharedLib>d__.<>1__state = -1;
			<LoadSceneFromDeviceSharedLib>d__.<>t__builder.Start<MRUK.<LoadSceneFromDeviceSharedLib>d__93>(ref <LoadSceneFromDeviceSharedLib>d__);
			return <LoadSceneFromDeviceSharedLib>d__.<>t__builder.Task;
		}

		private Task<MRUK.LoadDeviceResult> LoadSceneFromJsonSharedLib(string jsonString, bool removeMissingRooms = true)
		{
			MRUK.<LoadSceneFromJsonSharedLib>d__94 <LoadSceneFromJsonSharedLib>d__;
			<LoadSceneFromJsonSharedLib>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromJsonSharedLib>d__.<>4__this = this;
			<LoadSceneFromJsonSharedLib>d__.jsonString = jsonString;
			<LoadSceneFromJsonSharedLib>d__.removeMissingRooms = removeMissingRooms;
			<LoadSceneFromJsonSharedLib>d__.<>1__state = -1;
			<LoadSceneFromJsonSharedLib>d__.<>t__builder.Start<MRUK.<LoadSceneFromJsonSharedLib>d__94>(ref <LoadSceneFromJsonSharedLib>d__);
			return <LoadSceneFromJsonSharedLib>d__.<>t__builder.Task;
		}

		private unsafe string SaveSceneToJsonSharedLib(bool includeGlobalMesh, List<MRUKRoom> rooms)
		{
			Guid[] array = null;
			if (rooms != null)
			{
				array = new Guid[rooms.Count];
				for (int i = 0; i < rooms.Count; i++)
				{
					array[i] = rooms[i].Anchor.Uuid;
				}
			}
			char* ptr = MRUKNativeFuncs.AnchorStoreSaveSceneToJson(includeGlobalMesh, array, (uint)((array != null) ? array.Length : 0));
			string result = Marshal.PtrToStringUTF8((IntPtr)((void*)ptr));
			MRUKNativeFuncs.AnchorStoreFreeJson(ptr);
			return result;
		}

		private Task<MRUK.LoadDeviceResult> LoadSceneFromPrefabSharedLib(GameObject scenePrefab)
		{
			MRUK.<LoadSceneFromPrefabSharedLib>d__96 <LoadSceneFromPrefabSharedLib>d__;
			<LoadSceneFromPrefabSharedLib>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<LoadSceneFromPrefabSharedLib>d__.<>4__this = this;
			<LoadSceneFromPrefabSharedLib>d__.scenePrefab = scenePrefab;
			<LoadSceneFromPrefabSharedLib>d__.<>1__state = -1;
			<LoadSceneFromPrefabSharedLib>d__.<>t__builder.Start<MRUK.<LoadSceneFromPrefabSharedLib>d__96>(ref <LoadSceneFromPrefabSharedLib>d__);
			return <LoadSceneFromPrefabSharedLib>d__.<>t__builder.Task;
		}

		private MRUKNativeFuncs.MrukSceneAnchor GetAdjacentMrukSceneWall(ref int thisID, List<MRUKNativeFuncs.MrukSceneAnchor> randomWalls)
		{
			Vector3 vector = new Vector2(randomWalls[thisID].plane.width, randomWalls[thisID].plane.height) * 0.5f;
			Vector3 position = randomWalls[thisID].pose.position;
			MRUKNativeFuncs.MrukSceneAnchor mrukSceneAnchor = randomWalls[thisID];
			Vector3 a = position - mrukSceneAnchor.pose.up * vector.y;
			mrukSceneAnchor = randomWalls[thisID];
			Vector3 b = a - mrukSceneAnchor.pose.right * vector.x;
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < randomWalls.Count; i++)
			{
				if (i != thisID)
				{
					Vector2 vector2 = new Vector2(randomWalls[i].plane.width * 0.5f, randomWalls[i].plane.height * 0.5f);
					Vector3 position2 = randomWalls[i].pose.position;
					mrukSceneAnchor = randomWalls[i];
					Vector3 a2 = position2 - mrukSceneAnchor.pose.up * vector2.y;
					mrukSceneAnchor = randomWalls[i];
					float num3 = Vector3.Distance(a2 + mrukSceneAnchor.pose.right * vector2.x, b);
					if (num3 < num)
					{
						num = num3;
						num2 = i;
					}
				}
			}
			thisID = num2;
			return randomWalls[thisID];
		}

		private unsafe MRUKNativeFuncs.MrukSceneAnchor CreateMrukSceneAnchor(string semanticLabel, List<GCHandle> handles, Vector3 position, Quaternion rotation, Vector3 objScale, MRUK.AnchorRepresentation representation)
		{
			MRUKNativeFuncs.MrukSceneAnchor mrukSceneAnchor = new MRUKNativeFuncs.MrukSceneAnchor
			{
				semanticLabel = (MRUKNativeFuncs.MrukLabel)(1 << (int)OVRSemanticLabels.FromApiLabel(semanticLabel))
			};
			mrukSceneAnchor.pose.position = position;
			mrukSceneAnchor.pose.rotation = rotation;
			if ((representation & MRUK.AnchorRepresentation.PLANE) != (MRUK.AnchorRepresentation)0)
			{
				MRUKNativeFuncs.MrukPlane mrukPlane = new MRUKNativeFuncs.MrukPlane
				{
					x = -0.5f * objScale.x,
					y = -0.5f * objScale.y,
					width = objScale.x,
					height = objScale.y
				};
				mrukSceneAnchor.plane = mrukPlane;
				Vector2[] array = new Vector2[]
				{
					new Vector2(mrukPlane.x, mrukPlane.y),
					new Vector2(mrukPlane.x + mrukPlane.width, mrukPlane.y),
					new Vector2(mrukPlane.x + mrukPlane.width, mrukPlane.y + mrukPlane.height),
					new Vector2(mrukPlane.x, mrukPlane.y + mrukPlane.height)
				};
				GCHandle item = GCHandle.Alloc(array, GCHandleType.Pinned);
				handles.Add(item);
				mrukSceneAnchor.planeBoundary = (Vector2*)((void*)item.AddrOfPinnedObject());
				mrukSceneAnchor.planeBoundaryCount = (uint)array.Length;
				mrukSceneAnchor.hasPlane = true;
			}
			if ((representation & MRUK.AnchorRepresentation.VOLUME) != (MRUK.AnchorRepresentation)0)
			{
				Vector3 zero = new Vector3(0f, 0f, -objScale.z * 0.5f);
				if (mrukSceneAnchor.semanticLabel == MRUKNativeFuncs.MrukLabel.Couch)
				{
					zero = Vector3.zero;
				}
				mrukSceneAnchor.volume = new MRUKNativeFuncs.MrukVolume
				{
					min = zero - 0.5f * objScale,
					max = zero + 0.5f * objScale
				};
				mrukSceneAnchor.hasVolume = true;
			}
			mrukSceneAnchor.uuid = Guid.NewGuid();
			return mrukSceneAnchor;
		}

		private void ClearSceneSharedLib()
		{
			MRUKNativeFuncs.AnchorStoreClearRooms();
		}

		private static Vector2 FlipX(Vector2 vector)
		{
			return new Vector2(-vector.x, vector.y);
		}

		private static Vector3 FlipX(Vector3 vector)
		{
			return new Vector3(-vector.x, vector.y, vector.z);
		}

		private static Vector3 FlipZ(Vector3 vector)
		{
			return new Vector3(vector.x, vector.y, -vector.z);
		}

		private static Quaternion FlipZ(Quaternion quaternion)
		{
			return new Quaternion(-quaternion.x, -quaternion.y, quaternion.z, quaternion.w);
		}

		private static Quaternion FlipZRotateY180(Quaternion rotation)
		{
			return new Quaternion(-rotation.z, rotation.w, -rotation.x, rotation.y);
		}

		private static Pose FlipZ(Pose pose)
		{
			return new Pose(MRUK.FlipZ(pose.position), MRUK.FlipZ(pose.rotation));
		}

		internal static Pose FlipZRotateY180(Pose pose)
		{
			return new Pose(MRUK.FlipZ(pose.position), MRUK.FlipZRotateY180(pose.rotation));
		}

		private static MRUKNativeFuncs.MrukVolume ConvertVolume(MRUKNativeFuncs.MrukVolume volume)
		{
			Vector3 min = volume.min;
			Vector3 max = volume.max;
			return new MRUKNativeFuncs.MrukVolume
			{
				min = new Vector3(-max.x, min.y, min.z),
				max = new Vector3(-min.x, max.y, max.z)
			};
		}

		private static MRUKNativeFuncs.MrukPlane ConvertPlane(MRUKNativeFuncs.MrukPlane plane)
		{
			return new MRUKNativeFuncs.MrukPlane
			{
				x = -(plane.x + plane.width),
				y = plane.y,
				width = plane.width,
				height = plane.height
			};
		}

		private MRUKRoom FindRoomByUuid(Guid uuid)
		{
			foreach (MRUKRoom mrukroom in this.Rooms)
			{
				if (mrukroom.Anchor.Uuid == uuid)
				{
					return mrukroom;
				}
			}
			return null;
		}

		private unsafe static void UpdateAnchorProperties(MRUKAnchor anchor, ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor)
		{
			MRUKAnchor.SceneLabels sceneLabels = MRUK.ConvertLabel(sceneAnchor.semanticLabel);
			string name = (sceneLabels != (MRUKAnchor.SceneLabels)0) ? sceneLabels.ToString() : "UNDEFINED_ANCHOR";
			anchor.gameObject.name = name;
			Pose pose = MRUK.FlipZRotateY180(sceneAnchor.pose);
			anchor.InitialPose = pose;
			anchor.gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
			anchor.Label = sceneLabels;
			anchor.Anchor = new OVRAnchor(sceneAnchor.space, sceneAnchor.uuid);
			if (sceneAnchor.hasPlane)
			{
				anchor.PlaneBoundary2D = new List<Vector2>((int)sceneAnchor.planeBoundaryCount);
				int num = 0;
				while ((long)num < (long)((ulong)sceneAnchor.planeBoundaryCount))
				{
					Vector2 vector = sceneAnchor.planeBoundary[((ulong)sceneAnchor.planeBoundaryCount - (ulong)((long)num) - 1UL) * (ulong)((long)sizeof(Vector2)) / (ulong)sizeof(Vector2)];
					anchor.PlaneBoundary2D.Add(MRUK.FlipX(vector));
					num++;
				}
				MRUKNativeFuncs.MrukPlane mrukPlane = MRUK.ConvertPlane(sceneAnchor.plane);
				anchor.PlaneRect = new Rect?(new Rect(mrukPlane.x, mrukPlane.y, mrukPlane.width, mrukPlane.height));
			}
			else
			{
				anchor.PlaneBoundary2D = null;
				anchor.PlaneRect = null;
			}
			if (sceneAnchor.hasVolume)
			{
				MRUKNativeFuncs.MrukVolume mrukVolume = MRUK.ConvertVolume(sceneAnchor.volume);
				anchor.VolumeBounds = new Bounds?(new Bounds((mrukVolume.min + mrukVolume.max) / 2f, mrukVolume.max - mrukVolume.min));
			}
			else
			{
				anchor.VolumeBounds = null;
			}
			if (sceneAnchor.globalMeshPositionsCount > 0U && sceneAnchor.globalMeshIndicesCount > 0U)
			{
				Vector3[] array = new Vector3[sceneAnchor.globalMeshPositionsCount];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)sceneAnchor.globalMeshPositionsCount))
				{
					array[num2] = MRUK.FlipX(sceneAnchor.globalMeshPositions[num2]);
					num2++;
				}
				int[] array2 = new int[sceneAnchor.globalMeshIndicesCount];
				int num3 = 0;
				while ((long)num3 < (long)((ulong)sceneAnchor.globalMeshIndicesCount))
				{
					array2[num3] = (int)sceneAnchor.globalMeshIndices[num3];
					array2[num3 + 1] = (int)sceneAnchor.globalMeshIndices[num3 + 2];
					array2[num3 + 2] = (int)sceneAnchor.globalMeshIndices[num3 + 1];
					num3 += 3;
				}
				Mesh mesh = new Mesh
				{
					indexFormat = ((sceneAnchor.globalMeshIndicesCount > 65535U) ? IndexFormat.UInt32 : IndexFormat.UInt16),
					vertices = array,
					triangles = array2
				};
				anchor.Mesh = mesh;
				return;
			}
			anchor.Mesh = null;
		}

		[MonoPInvokeCallback(typeof(OVRPlugin.OpenXREventDelegateType))]
		private static void OnOpenXrEvent(IntPtr data, IntPtr context)
		{
			try
			{
				MRUKNativeFuncs.AnchorStoreOnOpenXrEvent(data);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnPreRoomAnchorAdded))]
		private static void OnPreRoomAnchorAdded(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, IntPtr userContext)
		{
			try
			{
				Guid uuid = roomAnchor.uuid;
				MRUKRoom mrukroom = new GameObject(string.Format("Room - {0}", uuid)).AddComponent<MRUKRoom>();
				mrukroom.Anchor = new OVRAnchor(roomAnchor.space, uuid);
				if (roomAnchor.pose != Pose.identity)
				{
					Pose pose = MRUK.FlipZRotateY180(roomAnchor.pose);
					mrukroom.InitialPose = pose;
					mrukroom.transform.SetPositionAndRotation(pose.position, pose.rotation);
				}
				MRUK.Instance.Rooms.Add(mrukroom);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnRoomAnchorAdded))]
		private static void OnRoomAnchorAdded(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, IntPtr userContext)
		{
			try
			{
				MRUKRoom mrukroom = MRUK.Instance.FindRoomByUuid(roomAnchor.uuid);
				mrukroom.ComputeRoomInfo();
				MRUK.Instance.RoomCreatedEvent.Invoke(mrukroom);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnRoomAnchorUpdated))]
		private static void OnRoomAnchorUpdated(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, ref Guid oldRoomAnchorUuid, bool significantChange, IntPtr userContext)
		{
			try
			{
				MRUKRoom mrukroom = MRUK.Instance.FindRoomByUuid(oldRoomAnchorUuid);
				mrukroom.Anchor = new OVRAnchor(roomAnchor.space, roomAnchor.uuid);
				if (significantChange)
				{
					mrukroom.ComputeRoomInfo();
					MRUK.Instance.RoomUpdatedEvent.Invoke(mrukroom);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnRoomAnchorRemoved))]
		private static void OnRoomAnchorRemoved(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, IntPtr userContext)
		{
			try
			{
				MRUKRoom mrukroom = MRUK.Instance.FindRoomByUuid(roomAnchor.uuid);
				MRUK.Instance.RoomRemovedEvent.Invoke(mrukroom);
				MRUK.Instance.Rooms.Remove(mrukroom);
				Utilities.DestroyGameObjectAndChildren(mrukroom.gameObject);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnSceneAnchorAdded))]
		private static void OnSceneAnchorAdded(ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor, IntPtr userContext)
		{
			try
			{
				MRUKRoom mrukroom = MRUK.Instance.FindRoomByUuid(sceneAnchor.roomUuid);
				GameObject gameObject = new GameObject();
				MRUKAnchor mrukanchor = gameObject.AddComponent<MRUKAnchor>();
				mrukanchor.Room = mrukroom;
				gameObject.transform.SetParent(mrukroom.transform);
				MRUK.UpdateAnchorProperties(mrukanchor, ref sceneAnchor);
				mrukroom.Anchors.Add(mrukanchor);
				if ((mrukanchor.Label & (MRUKAnchor.SceneLabels.WALL_FACE | MRUKAnchor.SceneLabels.INVISIBLE_WALL_FACE)) != (MRUKAnchor.SceneLabels)0)
				{
					mrukroom.WallAnchors.Add(mrukanchor);
				}
				if ((mrukanchor.Label & MRUKAnchor.SceneLabels.CEILING) != (MRUKAnchor.SceneLabels)0)
				{
					mrukroom.CeilingAnchor = mrukanchor;
				}
				if ((mrukanchor.Label & MRUKAnchor.SceneLabels.FLOOR) != (MRUKAnchor.SceneLabels)0)
				{
					mrukroom.FloorAnchor = mrukanchor;
				}
				mrukroom.AnchorCreatedEvent.Invoke(mrukanchor);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnSceneAnchorUpdated))]
		private static void OnSceneAnchorUpdated(ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor, bool significantChange, IntPtr userContext)
		{
			try
			{
				MRUKRoom mrukroom = MRUK.Instance.FindRoomByUuid(sceneAnchor.roomUuid);
				MRUKAnchor mrukanchor = mrukroom.FindAnchorByUuid(sceneAnchor.uuid);
				MRUK.UpdateAnchorProperties(mrukanchor, ref sceneAnchor);
				if (significantChange)
				{
					mrukroom.AnchorUpdatedEvent.Invoke(mrukanchor);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnSceneAnchorRemoved))]
		private static void OnSceneAnchorRemoved(ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor, IntPtr userContext)
		{
			try
			{
				MRUKRoom mrukroom = MRUK.Instance.FindRoomByUuid(sceneAnchor.roomUuid);
				MRUKAnchor mrukanchor = mrukroom.FindAnchorByUuid(sceneAnchor.uuid);
				mrukroom.AnchorRemovedEvent.Invoke(mrukanchor);
				mrukroom.RemoveAndDestroyAnchor(mrukanchor);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnDiscoveryFinished))]
		private static void OnDiscoveryFinished(MRUKNativeFuncs.MrukResult result, IntPtr userContext)
		{
			try
			{
				MRUK instance = MRUK.Instance;
				if (instance._loadSceneTask != null)
				{
					instance._loadSceneTask.GetValueOrDefault().SetResult(MRUK.ConvertResult(result));
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[MonoPInvokeCallback(typeof(MRUKNativeFuncs.MrukOnEnvironmentRaycasterCreated))]
		private static void OnEnvironmentRaycasterCreated(MRUKNativeFuncs.MrukResult result, IntPtr userContext)
		{
		}

		private Task<MRUK.LoadDeviceResult> WaitForDiscoveryFinished()
		{
			MRUK.<WaitForDiscoveryFinished>d__121 <WaitForDiscoveryFinished>d__;
			<WaitForDiscoveryFinished>d__.<>t__builder = AsyncTaskMethodBuilder<MRUK.LoadDeviceResult>.Create();
			<WaitForDiscoveryFinished>d__.<>4__this = this;
			<WaitForDiscoveryFinished>d__.<>1__state = -1;
			<WaitForDiscoveryFinished>d__.<>t__builder.Start<MRUK.<WaitForDiscoveryFinished>d__121>(ref <WaitForDiscoveryFinished>d__);
			return <WaitForDiscoveryFinished>d__.<>t__builder.Task;
		}

		private static MRUK.LoadDeviceResult ConvertResult(MRUKNativeFuncs.MrukResult result)
		{
			switch (result)
			{
			case MRUKNativeFuncs.MrukResult.Success:
				return MRUK.LoadDeviceResult.Success;
			case MRUKNativeFuncs.MrukResult.ErrorDiscoveryOngoing:
				return MRUK.LoadDeviceResult.DiscoveryOngoing;
			case MRUKNativeFuncs.MrukResult.ErrorInvalidJson:
				return MRUK.LoadDeviceResult.FailureDataIsInvalid;
			case MRUKNativeFuncs.MrukResult.ErrorNoRoomsFound:
				return MRUK.LoadDeviceResult.NoRoomsFound;
			case MRUKNativeFuncs.MrukResult.ErrorInsufficientResources:
				return MRUK.LoadDeviceResult.FailureInsufficientResources;
			case MRUKNativeFuncs.MrukResult.ErrorStorageAtCapacity:
				return MRUK.LoadDeviceResult.StorageAtCapacity;
			case MRUKNativeFuncs.MrukResult.ErrorInsufficientView:
				return MRUK.LoadDeviceResult.FailureInsufficientView;
			case MRUKNativeFuncs.MrukResult.ErrorPermissionInsufficient:
				return MRUK.LoadDeviceResult.FailurePermissionInsufficient;
			case MRUKNativeFuncs.MrukResult.ErrorRateLimited:
				return MRUK.LoadDeviceResult.FailureRateLimited;
			case MRUKNativeFuncs.MrukResult.ErrorTooDark:
				return MRUK.LoadDeviceResult.FailureTooDark;
			case MRUKNativeFuncs.MrukResult.ErrorTooBright:
				return MRUK.LoadDeviceResult.FailureTooBright;
			}
			return MRUK.LoadDeviceResult.Failure;
		}

		private static MRUKAnchor.SceneLabels ConvertLabel(MRUKNativeFuncs.MrukLabel label)
		{
			return (MRUKAnchor.SceneLabels)label;
		}

		public OVRAnchor.TrackerConfiguration TrackerConfiguration
		{
			get
			{
				OVRAnchor.Tracker tracker = this._tracker;
				if (tracker == null)
				{
					return default(OVRAnchor.TrackerConfiguration);
				}
				return tracker.Configuration;
			}
		}

		public void GetTrackables(List<MRUKTrackable> trackables)
		{
			if (trackables == null)
			{
				throw new ArgumentNullException("trackables");
			}
			trackables.Clear();
			foreach (Transform transform in this._trackableTransforms.Values)
			{
				MRUKTrackable item;
				if (transform && transform.TryGetComponent<MRUKTrackable>(out item))
				{
					trackables.Add(item);
				}
			}
		}

		private void OnEnable()
		{
			this._trackerCoroutine = base.StartCoroutine(this.TrackerCoroutine());
		}

		private void UpdateTrackables()
		{
			List<OVRAnchor> list;
			using (new OVRObjectPool.ListScope<OVRAnchor>(ref list))
			{
				foreach (KeyValuePair<OVRAnchor, Transform> keyValuePair in this._trackableTransforms)
				{
					OVRAnchor ovranchor;
					Transform transform;
					keyValuePair.Deconstruct(out ovranchor, out transform);
					OVRAnchor item = ovranchor;
					if (transform == null)
					{
						list.Add(item);
					}
				}
				foreach (OVRAnchor key in list)
				{
					this._trackableTransforms.Remove(key);
					this._trackableStates[key] = MRUK.TrackableState.InstanceDestroyed;
					key.Dispose();
				}
			}
			List<OVRLocatable.TrackingSpacePose> list2;
			using (new OVRObjectPool.ListScope<OVRLocatable.TrackingSpacePose>(ref list2))
			{
				OVRLocatable.UpdateSceneAnchorTransforms(this._trackableTransforms, this._cameraRig ? this._cameraRig.trackingSpace : null, list2);
				using (List<OVRLocatable.TrackingSpacePose>.Enumerator enumerator3 = list2.GetEnumerator())
				{
					foreach (KeyValuePair<OVRAnchor, Transform> keyValuePair in this._trackableTransforms)
					{
						OVRAnchor ovranchor;
						Transform transform;
						keyValuePair.Deconstruct(out ovranchor, out transform);
						Component component = transform;
						enumerator3.MoveNext();
						OVRLocatable.TrackingSpacePose trackingSpacePose = enumerator3.Current;
						MRUKTrackable mruktrackable;
						if (component.TryGetComponent<MRUKTrackable>(out mruktrackable))
						{
							mruktrackable.IsTracked = (trackingSpacePose.IsPositionTracked && trackingSpacePose.IsRotationTracked);
						}
					}
				}
			}
		}

		private void OnDisable()
		{
			if (this._trackerCoroutine != null)
			{
				base.StopCoroutine(this._trackerCoroutine);
				this._trackerCoroutine = null;
			}
			this._tracker.Dispose();
		}

		private void ConfigureTrackerAndLogResult(OVRAnchor.TrackerConfiguration config)
		{
			MRUK.<ConfigureTrackerAndLogResult>d__135 <ConfigureTrackerAndLogResult>d__;
			<ConfigureTrackerAndLogResult>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ConfigureTrackerAndLogResult>d__.<>4__this = this;
			<ConfigureTrackerAndLogResult>d__.config = config;
			<ConfigureTrackerAndLogResult>d__.<>1__state = -1;
			<ConfigureTrackerAndLogResult>d__.<>t__builder.Start<MRUK.<ConfigureTrackerAndLogResult>d__135>(ref <ConfigureTrackerAndLogResult>d__);
		}

		private IEnumerator TrackerCoroutine()
		{
			List<OVRAnchor> anchors = new List<OVRAnchor>();
			HashSet<OVRAnchor> removed = new HashSet<OVRAnchor>();
			OVRAnchor.TrackerConfiguration lastConfig = default(OVRAnchor.TrackerConfiguration);
			bool hasScenePermission = Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE");
			while (base.enabled)
			{
				double nextFetchTime = (double)Time.realtimeSinceStartup + MRUK.TimeBetweenFetchTrackables.TotalSeconds;
				int startFrame = Time.frameCount;
				bool flag = false;
				if (!hasScenePermission && Permission.HasUserAuthorizedPermission("com.oculus.permission.USE_SCENE"))
				{
					flag = (hasScenePermission = true);
				}
				if (lastConfig != this.SceneSettings.TrackerConfiguration || (flag && this._tracker.Configuration != this.SceneSettings.TrackerConfiguration))
				{
					this.ConfigureTrackerAndLogResult(this.SceneSettings.TrackerConfiguration);
					lastConfig = this.SceneSettings.TrackerConfiguration;
				}
				OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>> task = this._tracker.FetchTrackablesAsync(anchors, null);
				while (!task.IsCompleted)
				{
					yield return null;
					if (!base.enabled)
					{
						task.Dispose();
						yield break;
					}
				}
				if (task.GetResult().Success)
				{
					removed.Clear();
					foreach (OVRAnchor item in this._trackableStates.Keys)
					{
						removed.Add(item);
					}
					foreach (OVRAnchor ovranchor in anchors)
					{
						Transform transform;
						MRUKTrackable mruktrackable;
						if (this._trackableStates.TryAdd(ovranchor, MRUK.TrackableState.PendingLocalization))
						{
							OVRLocatable locatable;
							if (ovranchor.TryGetComponent<OVRLocatable>(out locatable))
							{
								this.LocalizeTrackable(ovranchor, locatable);
							}
						}
						else if (this._trackableTransforms.TryGetValue(ovranchor, out transform) && transform && transform.TryGetComponent<MRUKTrackable>(out mruktrackable) && mruktrackable)
						{
							try
							{
								mruktrackable.OnFetch();
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
						}
						removed.Remove(ovranchor);
					}
					foreach (OVRAnchor key in removed)
					{
						this._trackableStates.Remove(key);
					}
					List<MRUKTrackable> list;
					using (new OVRObjectPool.ListScope<MRUKTrackable>(ref list))
					{
						foreach (OVRAnchor key2 in removed)
						{
							Transform transform2;
							MRUKTrackable item2;
							if (this._trackableTransforms.Remove(key2, out transform2) && transform2 && transform2.TryGetComponent<MRUKTrackable>(out item2))
							{
								list.Add(item2);
							}
						}
						using (List<MRUKTrackable>.Enumerator enumerator4 = list.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								MRUKTrackable mruktrackable2 = enumerator4.Current;
								try
								{
									mruktrackable2.IsTracked = false;
									this.SceneSettings.TrackableRemoved.Invoke(mruktrackable2);
								}
								catch (Exception exception2)
								{
									Debug.LogException(exception2);
								}
							}
							goto IL_378;
						}
					}
					goto IL_361;
				}
				IL_378:
				if (!base.enabled || (startFrame != Time.frameCount && (double)Time.realtimeSinceStartup >= nextFetchTime))
				{
					task = default(OVRTask<OVRResult<List<OVRAnchor>, OVRAnchor.FetchResult>>);
					continue;
				}
				IL_361:
				yield return null;
				goto IL_378;
			}
			yield break;
		}

		private void LocalizeTrackable(OVRAnchor anchor, OVRLocatable locatable)
		{
			MRUK.<LocalizeTrackable>d__138 <LocalizeTrackable>d__;
			<LocalizeTrackable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LocalizeTrackable>d__.<>4__this = this;
			<LocalizeTrackable>d__.anchor = anchor;
			<LocalizeTrackable>d__.locatable = locatable;
			<LocalizeTrackable>d__.<>1__state = -1;
			<LocalizeTrackable>d__.<>t__builder.Start<MRUK.<LocalizeTrackable>d__138>(ref <LocalizeTrackable>d__);
		}

		public bool EnableWorldLock = true;

		[HideInInspector]
		public Matrix4x4 TrackingSpaceOffset = Matrix4x4.identity;

		private bool _worldLockActive;

		private bool _worldLockWasEnabled;

		private bool _loadSceneCalled;

		private Pose? _prevTrackingSpacePose;

		private readonly List<OVRSemanticLabels.Classification> _classificationsBuffer = new List<OVRSemanticLabels.Classification>(1);

		[Tooltip("Contains all the information regarding data loading.")]
		public MRUK.MRUKSettings SceneSettings;

		private MRUKRoom _cachedCurrentRoom;

		private int _cachedCurrentRoomFrame;

		[SerializeField]
		internal GameObject _immersiveSceneDebuggerPrefab;

		private OVRTask<MRUK.LoadDeviceResult>? _loadSceneTask;

		private ulong _currentAppSpace;

		private bool _openXrInitialised;

		private readonly OVRAnchor.Tracker _tracker = new OVRAnchor.Tracker();

		private Coroutine _trackerCoroutine;

		private readonly Dictionary<OVRAnchor, MRUK.TrackableState> _trackableStates = new Dictionary<OVRAnchor, MRUK.TrackableState>();

		private readonly Dictionary<OVRAnchor, Transform> _trackableTransforms = new Dictionary<OVRAnchor, Transform>();

		private static readonly TimeSpan TimeBetweenFetchTrackables = TimeSpan.FromSeconds(0.5);

		public enum PositioningMethod
		{
			DEFAULT,
			CENTER,
			EDGE
		}

		public enum SceneDataSource
		{
			Device,
			Prefab,
			DeviceWithPrefabFallback,
			Json,
			DeviceWithJsonFallback
		}

		public enum RoomFilter
		{
			None,
			CurrentRoomOnly,
			AllRooms
		}

		public enum LoadDeviceResult
		{
			Success,
			NoScenePermission,
			NoRoomsFound,
			DiscoveryOngoing,
			Failure = -1000,
			StorageAtCapacity = -9001,
			NotInitialized = -1002,
			FailureDataIsInvalid = -1008,
			FailureInsufficientResources = -9000,
			FailureInsufficientView = -9002,
			FailurePermissionInsufficient = -9003,
			FailureRateLimited = -9004,
			FailureTooDark = -9005,
			FailureTooBright = -9006
		}

		internal struct SceneTrackingSettings
		{
			internal HashSet<MRUKRoom> UnTrackedRooms;

			internal HashSet<MRUKAnchor> UnTrackedAnchors;
		}

		[Flags]
		public enum SurfaceType
		{
			FACING_UP = 1,
			FACING_DOWN = 2,
			VERTICAL = 4
		}

		[Flags]
		private enum AnchorRepresentation
		{
			PLANE = 1,
			VOLUME = 2
		}

		[Serializable]
		public class MRUKSettings
		{
			public OVRAnchor.TrackerConfiguration TrackerConfiguration { get; set; }

			public UnityEvent<MRUKTrackable> TrackableAdded { get; private set; } = new UnityEvent<MRUKTrackable>();

			public UnityEvent<MRUKTrackable> TrackableRemoved { get; private set; } = new UnityEvent<MRUKTrackable>();

			[Header("Data Source settings")]
			[SerializeField]
			[Tooltip("Where to load the data from.")]
			public MRUK.SceneDataSource DataSource;

			[SerializeField]
			[Tooltip("The index (0-based) into the RoomPrefabs or SceneJsons array; -1 is random.")]
			public int RoomIndex = -1;

			[SerializeField]
			[Tooltip("The list of prefab rooms to use.")]
			public GameObject[] RoomPrefabs;

			[SerializeField]
			[Tooltip("The list of JSON text files with scene data to use. Uses RoomIndex")]
			public TextAsset[] SceneJsons;

			[Space]
			[Header("Startup settings")]
			[SerializeField]
			[Tooltip("Trigger a scene load on startup. If set to false, you can call LoadSceneFromDevice(), LoadSceneFromPrefab() or LoadSceneFromJsonString() manually.")]
			public bool LoadSceneOnStartup = true;

			[Space]
			[Header("Other settings")]
			[SerializeField]
			[Tooltip("The width of a seat. Used to calculate seat positions with the COUCH label.")]
			public float SeatWidth = 0.6f;

			[SerializeField]
			[HideInInspector]
			[Obsolete]
			internal string SceneJson;
		}

		private struct SharedRoomsData
		{
			internal IEnumerable<Guid> roomUuids;

			internal Guid groupUuid;

			[TupleElementNames(new string[]
			{
				"alignmentRoomUuid",
				"floorWorldPoseOnHost"
			})]
			internal ValueTuple<Guid, Pose>? alignmentData;
		}

		private enum TrackableState
		{
			PendingLocalization,
			InstanceDestroyed,
			Instantiated,
			LocalizationFailed
		}
	}
}
