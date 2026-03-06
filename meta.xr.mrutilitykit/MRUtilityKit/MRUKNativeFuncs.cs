using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Meta.XR.MRUtilityKit
{
	internal static class MRUKNativeFuncs
	{
		internal static void LoadNativeFunctions()
		{
			MRUKNativeFuncs.SetLogPrinter = MRUKNative.LoadFunction<MRUKNativeFuncs.SetLogPrinterDelegate>("SetLogPrinter");
			MRUKNativeFuncs.AnchorStoreCreate = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreCreateDelegate>("AnchorStoreCreate");
			MRUKNativeFuncs.AnchorStoreCreateWithoutOpenXr = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreCreateWithoutOpenXrDelegate>("AnchorStoreCreateWithoutOpenXr");
			MRUKNativeFuncs.AnchorStoreShutdownOpenXr = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreShutdownOpenXrDelegate>("AnchorStoreShutdownOpenXr");
			MRUKNativeFuncs.AnchorStoreDestroy = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreDestroyDelegate>("AnchorStoreDestroy");
			MRUKNativeFuncs.AnchorStoreSetBaseSpace = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreSetBaseSpaceDelegate>("AnchorStoreSetBaseSpace");
			MRUKNativeFuncs.AnchorStoreStartDiscovery = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreStartDiscoveryDelegate>("AnchorStoreStartDiscovery");
			MRUKNativeFuncs.AnchorStoreStartQueryByLocalGroup = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreStartQueryByLocalGroupDelegate>("AnchorStoreStartQueryByLocalGroup");
			MRUKNativeFuncs.AnchorStoreLoadSceneFromJson = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreLoadSceneFromJsonDelegate>("AnchorStoreLoadSceneFromJson");
			MRUKNativeFuncs.AnchorStoreSaveSceneToJson = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreSaveSceneToJsonDelegate>("AnchorStoreSaveSceneToJson");
			MRUKNativeFuncs.AnchorStoreFreeJson = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreFreeJsonDelegate>("AnchorStoreFreeJson");
			MRUKNativeFuncs.AnchorStoreLoadSceneFromPrefab = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreLoadSceneFromPrefabDelegate>("AnchorStoreLoadSceneFromPrefab");
			MRUKNativeFuncs.AnchorStoreClearRooms = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreClearRoomsDelegate>("AnchorStoreClearRooms");
			MRUKNativeFuncs.AnchorStoreClearRoom = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreClearRoomDelegate>("AnchorStoreClearRoom");
			MRUKNativeFuncs.AnchorStoreOnOpenXrEvent = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreOnOpenXrEventDelegate>("AnchorStoreOnOpenXrEvent");
			MRUKNativeFuncs.AnchorStoreTick = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreTickDelegate>("AnchorStoreTick");
			MRUKNativeFuncs.AnchorStoreRegisterEventListener = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreRegisterEventListenerDelegate>("AnchorStoreRegisterEventListener");
			MRUKNativeFuncs.AnchorStoreRaycastRoom = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreRaycastRoomDelegate>("AnchorStoreRaycastRoom");
			MRUKNativeFuncs.AnchorStoreRaycastRoomAll = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreRaycastRoomAllDelegate>("AnchorStoreRaycastRoomAll");
			MRUKNativeFuncs.AnchorStoreRaycastAnchor = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreRaycastAnchorDelegate>("AnchorStoreRaycastAnchor");
			MRUKNativeFuncs.AnchorStoreRaycastAnchorAll = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreRaycastAnchorAllDelegate>("AnchorStoreRaycastAnchorAll");
			MRUKNativeFuncs.AnchorStoreIsDiscoveryRunning = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreIsDiscoveryRunningDelegate>("AnchorStoreIsDiscoveryRunning");
			MRUKNativeFuncs.AnchorStoreGetWorldLockOffset = MRUKNative.LoadFunction<MRUKNativeFuncs.AnchorStoreGetWorldLockOffsetDelegate>("AnchorStoreGetWorldLockOffset");
			MRUKNativeFuncs.AddVectors = MRUKNative.LoadFunction<MRUKNativeFuncs.AddVectorsDelegate>("AddVectors");
			MRUKNativeFuncs.TriangulatePolygon = MRUKNative.LoadFunction<MRUKNativeFuncs.TriangulatePolygonDelegate>("TriangulatePolygon");
			MRUKNativeFuncs.FreeMesh = MRUKNative.LoadFunction<MRUKNativeFuncs.FreeMeshDelegate>("FreeMesh");
			MRUKNativeFuncs.ComputeMeshSegmentation = MRUKNative.LoadFunction<MRUKNativeFuncs.ComputeMeshSegmentationDelegate>("ComputeMeshSegmentation");
			MRUKNativeFuncs.FreeMeshSegmentation = MRUKNative.LoadFunction<MRUKNativeFuncs.FreeMeshSegmentationDelegate>("FreeMeshSegmentation");
			MRUKNativeFuncs._TestUuidMarshalling = MRUKNative.LoadFunction<MRUKNativeFuncs._TestUuidMarshallingDelegate>("_TestUuidMarshalling");
			MRUKNativeFuncs.StringToMrukLabel = MRUKNative.LoadFunction<MRUKNativeFuncs.StringToMrukLabelDelegate>("StringToMrukLabel");
			MRUKNativeFuncs.CreateEnvironmentRaycaster = MRUKNative.LoadFunction<MRUKNativeFuncs.CreateEnvironmentRaycasterDelegate>("CreateEnvironmentRaycaster");
			MRUKNativeFuncs.DestroyEnvironmentRaycaster = MRUKNative.LoadFunction<MRUKNativeFuncs.DestroyEnvironmentRaycasterDelegate>("DestroyEnvironmentRaycaster");
			MRUKNativeFuncs.PerformEnvironmentRaycast = MRUKNative.LoadFunction<MRUKNativeFuncs.PerformEnvironmentRaycastDelegate>("PerformEnvironmentRaycast");
			MRUKNativeFuncs.SetTrackingSpacePoseGetter = MRUKNative.LoadFunction<MRUKNativeFuncs.SetTrackingSpacePoseGetterDelegate>("SetTrackingSpacePoseGetter");
			MRUKNativeFuncs.SetTrackingSpacePoseSetter = MRUKNative.LoadFunction<MRUKNativeFuncs.SetTrackingSpacePoseSetterDelegate>("SetTrackingSpacePoseSetter");
		}

		internal static void UnloadNativeFunctions()
		{
			MRUKNativeFuncs.SetLogPrinter = null;
			MRUKNativeFuncs.AnchorStoreCreate = null;
			MRUKNativeFuncs.AnchorStoreCreateWithoutOpenXr = null;
			MRUKNativeFuncs.AnchorStoreShutdownOpenXr = null;
			MRUKNativeFuncs.AnchorStoreDestroy = null;
			MRUKNativeFuncs.AnchorStoreSetBaseSpace = null;
			MRUKNativeFuncs.AnchorStoreStartDiscovery = null;
			MRUKNativeFuncs.AnchorStoreStartQueryByLocalGroup = null;
			MRUKNativeFuncs.AnchorStoreLoadSceneFromJson = null;
			MRUKNativeFuncs.AnchorStoreSaveSceneToJson = null;
			MRUKNativeFuncs.AnchorStoreFreeJson = null;
			MRUKNativeFuncs.AnchorStoreLoadSceneFromPrefab = null;
			MRUKNativeFuncs.AnchorStoreClearRooms = null;
			MRUKNativeFuncs.AnchorStoreClearRoom = null;
			MRUKNativeFuncs.AnchorStoreOnOpenXrEvent = null;
			MRUKNativeFuncs.AnchorStoreTick = null;
			MRUKNativeFuncs.AnchorStoreRegisterEventListener = null;
			MRUKNativeFuncs.AnchorStoreRaycastRoom = null;
			MRUKNativeFuncs.AnchorStoreRaycastRoomAll = null;
			MRUKNativeFuncs.AnchorStoreRaycastAnchor = null;
			MRUKNativeFuncs.AnchorStoreRaycastAnchorAll = null;
			MRUKNativeFuncs.AnchorStoreIsDiscoveryRunning = null;
			MRUKNativeFuncs.AnchorStoreGetWorldLockOffset = null;
			MRUKNativeFuncs.AddVectors = null;
			MRUKNativeFuncs.TriangulatePolygon = null;
			MRUKNativeFuncs.FreeMesh = null;
			MRUKNativeFuncs.ComputeMeshSegmentation = null;
			MRUKNativeFuncs.FreeMeshSegmentation = null;
			MRUKNativeFuncs._TestUuidMarshalling = null;
			MRUKNativeFuncs.StringToMrukLabel = null;
			MRUKNativeFuncs.CreateEnvironmentRaycaster = null;
			MRUKNativeFuncs.DestroyEnvironmentRaycaster = null;
			MRUKNativeFuncs.PerformEnvironmentRaycast = null;
			MRUKNativeFuncs.SetTrackingSpacePoseGetter = null;
			MRUKNativeFuncs.SetTrackingSpacePoseSetter = null;
		}

		internal static MRUKNativeFuncs.SetLogPrinterDelegate SetLogPrinter;

		internal static MRUKNativeFuncs.AnchorStoreCreateDelegate AnchorStoreCreate;

		internal static MRUKNativeFuncs.AnchorStoreCreateWithoutOpenXrDelegate AnchorStoreCreateWithoutOpenXr;

		internal static MRUKNativeFuncs.AnchorStoreShutdownOpenXrDelegate AnchorStoreShutdownOpenXr;

		internal static MRUKNativeFuncs.AnchorStoreDestroyDelegate AnchorStoreDestroy;

		internal static MRUKNativeFuncs.AnchorStoreSetBaseSpaceDelegate AnchorStoreSetBaseSpace;

		internal static MRUKNativeFuncs.AnchorStoreStartDiscoveryDelegate AnchorStoreStartDiscovery;

		internal static MRUKNativeFuncs.AnchorStoreStartQueryByLocalGroupDelegate AnchorStoreStartQueryByLocalGroup;

		internal static MRUKNativeFuncs.AnchorStoreLoadSceneFromJsonDelegate AnchorStoreLoadSceneFromJson;

		internal static MRUKNativeFuncs.AnchorStoreSaveSceneToJsonDelegate AnchorStoreSaveSceneToJson;

		internal static MRUKNativeFuncs.AnchorStoreFreeJsonDelegate AnchorStoreFreeJson;

		internal static MRUKNativeFuncs.AnchorStoreLoadSceneFromPrefabDelegate AnchorStoreLoadSceneFromPrefab;

		internal static MRUKNativeFuncs.AnchorStoreClearRoomsDelegate AnchorStoreClearRooms;

		internal static MRUKNativeFuncs.AnchorStoreClearRoomDelegate AnchorStoreClearRoom;

		internal static MRUKNativeFuncs.AnchorStoreOnOpenXrEventDelegate AnchorStoreOnOpenXrEvent;

		internal static MRUKNativeFuncs.AnchorStoreTickDelegate AnchorStoreTick;

		internal static MRUKNativeFuncs.AnchorStoreRegisterEventListenerDelegate AnchorStoreRegisterEventListener;

		internal static MRUKNativeFuncs.AnchorStoreRaycastRoomDelegate AnchorStoreRaycastRoom;

		internal static MRUKNativeFuncs.AnchorStoreRaycastRoomAllDelegate AnchorStoreRaycastRoomAll;

		internal static MRUKNativeFuncs.AnchorStoreRaycastAnchorDelegate AnchorStoreRaycastAnchor;

		internal static MRUKNativeFuncs.AnchorStoreRaycastAnchorAllDelegate AnchorStoreRaycastAnchorAll;

		internal static MRUKNativeFuncs.AnchorStoreIsDiscoveryRunningDelegate AnchorStoreIsDiscoveryRunning;

		internal static MRUKNativeFuncs.AnchorStoreGetWorldLockOffsetDelegate AnchorStoreGetWorldLockOffset;

		internal static MRUKNativeFuncs.AddVectorsDelegate AddVectors;

		internal static MRUKNativeFuncs.TriangulatePolygonDelegate TriangulatePolygon;

		internal static MRUKNativeFuncs.FreeMeshDelegate FreeMesh;

		internal static MRUKNativeFuncs.ComputeMeshSegmentationDelegate ComputeMeshSegmentation;

		internal static MRUKNativeFuncs.FreeMeshSegmentationDelegate FreeMeshSegmentation;

		internal static MRUKNativeFuncs._TestUuidMarshallingDelegate _TestUuidMarshalling;

		internal static MRUKNativeFuncs.StringToMrukLabelDelegate StringToMrukLabel;

		internal static MRUKNativeFuncs.CreateEnvironmentRaycasterDelegate CreateEnvironmentRaycaster;

		internal static MRUKNativeFuncs.DestroyEnvironmentRaycasterDelegate DestroyEnvironmentRaycaster;

		internal static MRUKNativeFuncs.PerformEnvironmentRaycastDelegate PerformEnvironmentRaycast;

		internal static MRUKNativeFuncs.SetTrackingSpacePoseGetterDelegate SetTrackingSpacePoseGetter;

		internal static MRUKNativeFuncs.SetTrackingSpacePoseSetterDelegate SetTrackingSpacePoseSetter;

		public enum MrukSceneModel
		{
			V2FallbackV1,
			V1,
			V2
		}

		public enum MrukLogLevel
		{
			Debug,
			Info,
			Warn,
			Error
		}

		public enum MrukResult
		{
			Success,
			ErrorInvalidArgs,
			ErrorUnknown,
			ErrorInternal,
			ErrorDiscoveryOngoing,
			ErrorInvalidJson,
			ErrorNoRoomsFound,
			ErrorInsufficientResources,
			ErrorStorageAtCapacity,
			ErrorInsufficientView,
			ErrorPermissionInsufficient,
			ErrorRateLimited,
			ErrorTooDark,
			ErrorTooBright
		}

		public enum MrukSurfaceType
		{
			None,
			Plane,
			Volume,
			Mesh = 4,
			All = 7
		}

		public enum MrukLabel
		{
			Floor = 1,
			Ceiling,
			WallFace = 4,
			Table = 8,
			Couch = 16,
			DoorFrame = 32,
			WindowFrame = 64,
			Other = 128,
			Storage = 256,
			Bed = 512,
			Screen = 1024,
			Lamp = 2048,
			Plant = 4096,
			WallArt = 8192,
			SceneMesh = 16384,
			InvisibleWallFace = 32768,
			Unknown = 131072,
			InnerWallFace = 262144,
			Tabletop = 524288,
			SittingArea = 1048576,
			SleepingArea = 2097152,
			StorageTop = 4194304
		}

		public enum MrukEnvironmentRaycastStatus
		{
			Hit = 1,
			NoHit,
			HitPointOccluded,
			HitPointOutsideFov,
			RayOccluded,
			InvalidOrientation,
			Max = 2147483647
		}

		public unsafe delegate void LogPrinter(MRUKNativeFuncs.MrukLogLevel logLevel, char* message, uint length);

		public delegate void MrukOnPreRoomAnchorAdded(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, IntPtr userContext);

		public delegate void MrukOnRoomAnchorAdded(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, IntPtr userContext);

		public delegate void MrukOnRoomAnchorUpdated(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, ref Guid oldRoomAnchorUuid, [MarshalAs(UnmanagedType.U1)] bool significantChange, IntPtr userContext);

		public delegate void MrukOnRoomAnchorRemoved(ref MRUKNativeFuncs.MrukRoomAnchor roomAnchor, IntPtr userContext);

		public delegate void MrukOnSceneAnchorAdded(ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor, IntPtr userContext);

		public delegate void MrukOnSceneAnchorUpdated(ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor, [MarshalAs(UnmanagedType.U1)] bool significantChange, IntPtr userContext);

		public delegate void MrukOnSceneAnchorRemoved(ref MRUKNativeFuncs.MrukSceneAnchor sceneAnchor, IntPtr userContext);

		public delegate void MrukOnDiscoveryFinished(MRUKNativeFuncs.MrukResult result, IntPtr userContext);

		public delegate void MrukOnEnvironmentRaycasterCreated(MRUKNativeFuncs.MrukResult result, IntPtr userContext);

		public delegate Pose TrackingSpacePoseGetter();

		public delegate void TrackingSpacePoseSetter(Pose pose);

		public struct MrukLabelFilter
		{
			public uint surfaceType;

			public uint includedLabels;

			[MarshalAs(UnmanagedType.U1)]
			public bool includedLabelsSet;
		}

		public struct MrukPolygon2f
		{
			public Vector2[] points;

			public uint numPoints;
		}

		public struct MrukMesh2f
		{
			public unsafe Vector2* vertices;

			public uint numVertices;

			public unsafe uint* indices;

			public uint numIndices;
		}

		public struct MrukMesh3f
		{
			public unsafe Vector3* vertices;

			public uint numVertices;

			public unsafe uint* indices;

			public uint numIndices;
		}

		public struct MrukVolume
		{
			public Vector3 min;

			public Vector3 max;
		}

		public struct MrukPlane
		{
			public float x;

			public float y;

			public float width;

			public float height;
		}

		public struct MrukSceneAnchor
		{
			public ulong space;

			public Guid uuid;

			public Guid roomUuid;

			public Pose pose;

			public MRUKNativeFuncs.MrukVolume volume;

			public MRUKNativeFuncs.MrukPlane plane;

			public MRUKNativeFuncs.MrukLabel semanticLabel;

			public unsafe Vector2* planeBoundary;

			public unsafe uint* globalMeshIndices;

			public unsafe Vector3* globalMeshPositions;

			public uint planeBoundaryCount;

			public uint globalMeshIndicesCount;

			public uint globalMeshPositionsCount;

			[MarshalAs(UnmanagedType.U1)]
			public bool hasVolume;

			[MarshalAs(UnmanagedType.U1)]
			public bool hasPlane;
		}

		public struct MrukRoomAnchor
		{
			public ulong space;

			public Guid uuid;

			public Pose pose;
		}

		public struct MrukEventListener
		{
			public MRUKNativeFuncs.MrukOnPreRoomAnchorAdded onPreRoomAnchorAdded;

			public MRUKNativeFuncs.MrukOnRoomAnchorAdded onRoomAnchorAdded;

			public MRUKNativeFuncs.MrukOnRoomAnchorUpdated onRoomAnchorUpdated;

			public MRUKNativeFuncs.MrukOnRoomAnchorRemoved onRoomAnchorRemoved;

			public MRUKNativeFuncs.MrukOnSceneAnchorAdded onSceneAnchorAdded;

			public MRUKNativeFuncs.MrukOnSceneAnchorUpdated onSceneAnchorUpdated;

			public MRUKNativeFuncs.MrukOnSceneAnchorRemoved onSceneAnchorRemoved;

			public MRUKNativeFuncs.MrukOnDiscoveryFinished onDiscoveryFinished;

			public MRUKNativeFuncs.MrukOnEnvironmentRaycasterCreated onEnvironmentRaycasterCreated;

			public IntPtr userContext;
		}

		public struct MrukHit
		{
			public Guid roomAnchorUuid;

			public Guid sceneAnchorUuid;

			public float hitDistance;

			public Vector3 hitPosition;

			public Vector3 hitNormal;
		}

		public struct MrukSharedRoomsData
		{
			public Guid groupUuid;

			public unsafe Guid* roomUuids;

			public uint numRoomUuids;

			public Guid alignmentRoomUuid;

			public Pose roomWorldPoseOnHost;
		}

		public struct _MrukUuidAlignmentTest
		{
			public byte padding;

			public Guid uuid;
		}

		public struct MrukEnvironmentRaycastHitPointGetInfo
		{
			public Vector3 startPoint;

			public Vector3 direction;

			public uint filterCount;

			public float maxDistance;
		}

		public struct MrukEnvironmentRaycastHitPoint
		{
			public MRUKNativeFuncs.MrukEnvironmentRaycastStatus status;

			public Vector3 point;

			public Quaternion orientation;

			public Vector3 normal;
		}

		internal delegate void SetLogPrinterDelegate(MRUKNativeFuncs.LogPrinter printer);

		internal delegate MRUKNativeFuncs.MrukResult AnchorStoreCreateDelegate(ulong xrInstance, ulong xrSession, IntPtr xrInstanceProcAddrFunc, ulong baseSpace, string[] availableOpenXrExtensions, uint availableOpenXrExtensionsCount);

		internal delegate MRUKNativeFuncs.MrukResult AnchorStoreCreateWithoutOpenXrDelegate();

		internal delegate void AnchorStoreShutdownOpenXrDelegate();

		internal delegate void AnchorStoreDestroyDelegate();

		internal delegate void AnchorStoreSetBaseSpaceDelegate(ulong baseSpace);

		internal delegate MRUKNativeFuncs.MrukResult AnchorStoreStartDiscoveryDelegate([MarshalAs(UnmanagedType.U1)] bool shouldRemoveMissingRooms, MRUKNativeFuncs.MrukSceneModel sceneModel);

		internal delegate MRUKNativeFuncs.MrukResult AnchorStoreStartQueryByLocalGroupDelegate(MRUKNativeFuncs.MrukSharedRoomsData sharedRoomsData, [MarshalAs(UnmanagedType.U1)] bool shouldRemoveMissingRooms, MRUKNativeFuncs.MrukSceneModel sceneModel);

		internal delegate MRUKNativeFuncs.MrukResult AnchorStoreLoadSceneFromJsonDelegate(string jsonString, [MarshalAs(UnmanagedType.U1)] bool shouldRemoveMissingRooms, MRUKNativeFuncs.MrukSceneModel sceneModel);

		internal unsafe delegate char* AnchorStoreSaveSceneToJsonDelegate([MarshalAs(UnmanagedType.U1)] bool includeGlobalMesh, Guid[] roomUuids, uint numRoomUuids);

		internal unsafe delegate void AnchorStoreFreeJsonDelegate(char* jsonString);

		internal unsafe delegate MRUKNativeFuncs.MrukResult AnchorStoreLoadSceneFromPrefabDelegate(MRUKNativeFuncs.MrukRoomAnchor* roomAnchors, uint numRoomAnchors, MRUKNativeFuncs.MrukSceneAnchor* sceneAnchors, uint numSceneAnchors);

		internal delegate void AnchorStoreClearRoomsDelegate();

		internal delegate void AnchorStoreClearRoomDelegate(Guid roomUuid);

		internal delegate void AnchorStoreOnOpenXrEventDelegate(IntPtr baseEventHeader);

		internal delegate void AnchorStoreTickDelegate(ulong nextPredictedDisplayTime);

		internal delegate void AnchorStoreRegisterEventListenerDelegate(MRUKNativeFuncs.MrukEventListener listener);

		[return: MarshalAs(UnmanagedType.U1)]
		internal delegate bool AnchorStoreRaycastRoomDelegate(Guid roomUuid, Vector3 origin, Vector3 direction, float maxDistance, MRUKNativeFuncs.MrukLabelFilter labelFilter, ref MRUKNativeFuncs.MrukHit outHit);

		[return: MarshalAs(UnmanagedType.U1)]
		internal delegate bool AnchorStoreRaycastRoomAllDelegate(Guid roomUuid, Vector3 origin, Vector3 direction, float maxDistance, MRUKNativeFuncs.MrukLabelFilter labelFilter, ref MRUKNativeFuncs.MrukHit outHits, ref uint outHitsCount);

		[return: MarshalAs(UnmanagedType.U1)]
		internal delegate bool AnchorStoreRaycastAnchorDelegate(Guid sceneAnchorUuid, Vector3 origin, Vector3 direction, float maxDistance, uint surfaceTypes, ref MRUKNativeFuncs.MrukHit outHit);

		[return: MarshalAs(UnmanagedType.U1)]
		internal delegate bool AnchorStoreRaycastAnchorAllDelegate(Guid sceneAnchorUuid, Vector3 origin, Vector3 direction, float maxDistance, uint surfaceTypes, ref MRUKNativeFuncs.MrukHit outHits, ref uint outHitsCount);

		[return: MarshalAs(UnmanagedType.U1)]
		internal delegate bool AnchorStoreIsDiscoveryRunningDelegate();

		[return: MarshalAs(UnmanagedType.U1)]
		internal delegate bool AnchorStoreGetWorldLockOffsetDelegate(Guid roomUuid, ref Pose offset);

		internal delegate Vector3 AddVectorsDelegate(Vector3 a, Vector3 b);

		internal delegate MRUKNativeFuncs.MrukMesh2f TriangulatePolygonDelegate(MRUKNativeFuncs.MrukPolygon2f[] polygons, uint numPolygons);

		internal delegate void FreeMeshDelegate(ref MRUKNativeFuncs.MrukMesh2f mesh);

		internal unsafe delegate MRUKNativeFuncs.MrukResult ComputeMeshSegmentationDelegate(Vector3[] vertices, uint numVertices, uint[] indices, uint numIndices, Vector3[] segmentationPoints, uint numSegmentationPoints, Vector3 reservedMin, Vector3 reservedMax, out MRUKNativeFuncs.MrukMesh3f* meshSegments, out uint numSegments, out MRUKNativeFuncs.MrukMesh3f reservedSegment);

		internal unsafe delegate void FreeMeshSegmentationDelegate(MRUKNativeFuncs.MrukMesh3f* meshSegments, uint numSegments, ref MRUKNativeFuncs.MrukMesh3f reservedSegment);

		internal delegate Guid _TestUuidMarshallingDelegate(MRUKNativeFuncs._MrukUuidAlignmentTest packedUuid);

		internal delegate MRUKNativeFuncs.MrukLabel StringToMrukLabelDelegate(string label);

		internal delegate void CreateEnvironmentRaycasterDelegate();

		internal delegate void DestroyEnvironmentRaycasterDelegate();

		internal delegate void PerformEnvironmentRaycastDelegate(ref MRUKNativeFuncs.MrukEnvironmentRaycastHitPointGetInfo info, ref MRUKNativeFuncs.MrukEnvironmentRaycastHitPoint hitPoint);

		internal delegate void SetTrackingSpacePoseGetterDelegate(MRUKNativeFuncs.TrackingSpacePoseGetter getter);

		internal delegate void SetTrackingSpacePoseSetterDelegate(MRUKNativeFuncs.TrackingSpacePoseSetter setter);
	}
}
