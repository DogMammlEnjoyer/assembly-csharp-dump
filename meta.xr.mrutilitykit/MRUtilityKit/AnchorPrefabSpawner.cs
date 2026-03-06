using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_anchor_prefab_spawner")]
	[Feature(Feature.Scene)]
	public class AnchorPrefabSpawner : MonoBehaviour, ICustomAnchorPrefabSpawner
	{
		public Dictionary<MRUKAnchor, GameObject> AnchorPrefabSpawnerObjects { get; } = new Dictionary<MRUKAnchor, GameObject>();

		[Obsolete("Use AnchorPrefabSpawnerObjects property instead. This property is inefficient because it will generate a new list each time it is accessed")]
		public List<GameObject> SpawnedPrefabs
		{
			get
			{
				return new List<GameObject>(this.AnchorPrefabSpawnerObjects.Values);
			}
		}

		protected virtual void Start()
		{
			OVRTelemetry.Start(651902681, 0, -1L).Send();
			if (MRUK.Instance == null)
			{
				return;
			}
			this.SceneTrackingSettings.UnTrackedRooms = new HashSet<MRUKRoom>();
			this.SceneTrackingSettings.UnTrackedAnchors = new HashSet<MRUKAnchor>();
			MRUK.Instance.RegisterSceneLoadedCallback(delegate
			{
				if (this.SpawnOnStart == MRUK.RoomFilter.None)
				{
					return;
				}
				switch (this.SpawnOnStart)
				{
				case MRUK.RoomFilter.None:
					return;
				case MRUK.RoomFilter.CurrentRoomOnly:
					this.SpawnPrefabs(MRUK.Instance.GetCurrentRoom(), true);
					return;
				case MRUK.RoomFilter.AllRooms:
					this.SpawnPrefabs(true);
					return;
				default:
					throw new ArgumentOutOfRangeException();
				}
			});
			bool trackUpdates = this.TrackUpdates;
		}

		protected virtual void OnEnable()
		{
			if (MRUK.Instance)
			{
				MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
				MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
			}
		}

		protected virtual void OnDisable()
		{
			if (MRUK.Instance)
			{
				MRUK.Instance.RoomCreatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
				MRUK.Instance.RoomRemovedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
			}
		}

		protected virtual void ReceiveRemovedRoom(MRUKRoom room)
		{
			this.ClearPrefabs(room);
			this.UnRegisterAnchorUpdates(room);
		}

		protected virtual void UnRegisterAnchorUpdates(MRUKRoom room)
		{
			room.AnchorCreatedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorCreatedEvent));
			room.AnchorRemovedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorRemovedCallback));
			room.AnchorUpdatedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorUpdatedCallback));
		}

		protected virtual void RegisterAnchorUpdates(MRUKRoom room)
		{
			room.AnchorCreatedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorCreatedEvent));
			room.AnchorRemovedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorRemovedCallback));
			room.AnchorUpdatedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorUpdatedCallback));
		}

		protected virtual void ReceiveAnchorUpdatedCallback(MRUKAnchor anchorInfo)
		{
			if (this.SceneTrackingSettings.UnTrackedRooms.Contains(anchorInfo.Room) || this.SceneTrackingSettings.UnTrackedAnchors.Contains(anchorInfo) || !this.TrackUpdates)
			{
				return;
			}
			this.ClearPrefabs();
			this.SpawnPrefabs(anchorInfo);
		}

		protected virtual void ReceiveAnchorRemovedCallback(MRUKAnchor anchorInfo)
		{
			this.ClearPrefabs();
		}

		protected virtual void ReceiveAnchorCreatedEvent(MRUKAnchor anchorInfo)
		{
			if (this.SceneTrackingSettings.UnTrackedRooms.Contains(anchorInfo.Room) || !this.TrackUpdates)
			{
				return;
			}
			this.SpawnPrefabs(true);
		}

		protected virtual void ReceiveCreatedRoom(MRUKRoom room)
		{
			if (this.TrackUpdates && this.SpawnOnStart == MRUK.RoomFilter.AllRooms)
			{
				this.SpawnPrefabs(room, true);
				this.RegisterAnchorUpdates(room);
			}
		}

		protected virtual void ClearPrefabs(MRUKRoom room)
		{
			List<MRUKAnchor> list = new List<MRUKAnchor>();
			foreach (KeyValuePair<MRUKAnchor, GameObject> keyValuePair in this.AnchorPrefabSpawnerObjects)
			{
				if (!(keyValuePair.Key.Room != room))
				{
					this.ClearPrefab(keyValuePair.Value);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (MRUKAnchor key in list)
			{
				this.AnchorPrefabSpawnerObjects.Remove(key);
			}
			this.SceneTrackingSettings.UnTrackedRooms.Add(room);
		}

		protected virtual void ClearPrefab(GameObject go)
		{
			Object.Destroy(go);
		}

		protected virtual void ClearPrefab(MRUKAnchor anchorInfo)
		{
			if (!this.AnchorPrefabSpawnerObjects.ContainsKey(anchorInfo))
			{
				return;
			}
			this.ClearPrefab(this.AnchorPrefabSpawnerObjects[anchorInfo]);
			this.AnchorPrefabSpawnerObjects.Remove(anchorInfo);
			this.SceneTrackingSettings.UnTrackedAnchors.Add(anchorInfo);
		}

		protected virtual void ClearPrefabs()
		{
			foreach (KeyValuePair<MRUKAnchor, GameObject> keyValuePair in this.AnchorPrefabSpawnerObjects)
			{
				this.ClearPrefab(keyValuePair.Value);
			}
			this.AnchorPrefabSpawnerObjects.Clear();
		}

		protected virtual void SpawnPrefabs(bool clearPrefabs = true)
		{
			if (clearPrefabs)
			{
				this.ClearPrefabs();
			}
			foreach (MRUKRoom room in MRUK.Instance.Rooms)
			{
				this.SpawnPrefabsInternal(room);
			}
			UnityEvent unityEvent = this.onPrefabSpawned;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		protected virtual void SpawnPrefabs(MRUKRoom room, bool clearPrefabs = true)
		{
			if (clearPrefabs)
			{
				this.ClearPrefabs();
			}
			this.SpawnPrefabsInternal(room);
			UnityEvent unityEvent = this.onPrefabSpawned;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		private void SpawnPrefabsInternal(MRUKRoom room)
		{
			this.InitializeRandom(ref this.SeedValue);
			foreach (MRUKAnchor anchorInfo in room.Anchors)
			{
				this.SpawnPrefab(anchorInfo);
			}
		}

		protected virtual void SpawnPrefab(MRUKAnchor anchorInfo)
		{
			AnchorPrefabSpawner.AnchorPrefabGroup anchorPrefabGroup;
			GameObject gameObject = this.LabelToPrefab(anchorInfo.Label, anchorInfo, out anchorPrefabGroup);
			if (gameObject == null)
			{
				return;
			}
			if (this.AnchorPrefabSpawnerObjects.ContainsKey(anchorInfo))
			{
				Debug.LogWarning("Anchor already associated with a gameobject spawned from this AnchorPrefabSpawner");
				return;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, anchorInfo.transform);
			gameObject2.name = gameObject.name + AnchorPrefabSpawner.Suffix;
			gameObject2.name = gameObject.name + AnchorPrefabSpawner.Suffix;
			gameObject2.transform.parent = anchorInfo.transform;
			Bounds? prefabBounds = anchorPrefabGroup.IgnorePrefabSize ? null : Utilities.GetPrefabBounds(gameObject);
			if (prefabBounds == null)
			{
				GridSliceResizer componentInChildren = gameObject2.GetComponentInChildren<GridSliceResizer>(true);
				prefabBounds = ((componentInChildren != null) ? new Bounds?(componentInChildren.OriginalMesh.bounds) : null);
			}
			Vector3 vector = (prefabBounds != null) ? prefabBounds.GetValueOrDefault().size : Vector3.one;
			if (anchorInfo.VolumeBounds != null)
			{
				int num = 0;
				if (anchorPrefabGroup.CalculateFacingDirection && !anchorPrefabGroup.MatchAspectRatio)
				{
					anchorInfo.Room.GetDirectionAwayFromClosestWall(anchorInfo, out num, null);
				}
				Bounds anchorVolumeBounds = AnchorPrefabSpawnerUtilities.RotateVolumeBounds(anchorInfo.VolumeBounds.Value, num);
				Vector3 size = anchorVolumeBounds.size;
				Vector3 localScale = new Vector3(size.x / vector.x, size.z / vector.y, size.y / vector.z);
				if (anchorPrefabGroup.MatchAspectRatio)
				{
					AnchorPrefabSpawnerUtilities.MatchAspectRatio(anchorInfo, anchorPrefabGroup.CalculateFacingDirection, vector, size, ref num, ref anchorVolumeBounds, ref localScale);
				}
				localScale = ((anchorPrefabGroup.Scaling == AnchorPrefabSpawner.ScalingMode.Custom) ? this.CustomPrefabScaling(localScale) : AnchorPrefabSpawnerUtilities.ScalePrefab(localScale, anchorPrefabGroup.Scaling));
				Vector3 point = (anchorPrefabGroup.Alignment == AnchorPrefabSpawner.AlignMode.Custom) ? this.CustomPrefabAlignment(anchorVolumeBounds, prefabBounds) : AnchorPrefabSpawnerUtilities.AlignPrefabPivot(anchorVolumeBounds, prefabBounds, localScale, anchorPrefabGroup.Alignment);
				gameObject2.transform.localPosition = Quaternion.AngleAxis((float)(num * 90), Vector3.forward) * point;
				gameObject2.transform.localRotation = Quaternion.Euler((float)((num - 1) * 90), -90f, -90f);
				gameObject2.transform.localScale = localScale;
			}
			else if (anchorInfo.PlaneRect != null)
			{
				Vector2 size2 = anchorInfo.PlaneRect.Value.size;
				Vector2 localScale2 = new Vector2(size2.x / vector.x, size2.y / vector.y);
				gameObject2.transform.localScale = ((anchorPrefabGroup.Scaling == AnchorPrefabSpawner.ScalingMode.Custom) ? this.CustomPrefabScaling(localScale2) : AnchorPrefabSpawnerUtilities.ScalePrefab(localScale2, anchorPrefabGroup.Scaling));
				gameObject2.transform.localPosition = ((anchorPrefabGroup.Alignment == AnchorPrefabSpawner.AlignMode.Custom) ? this.CustomPrefabAlignment(anchorInfo.PlaneRect.Value, prefabBounds) : AnchorPrefabSpawnerUtilities.AlignPrefabPivot(anchorInfo.PlaneRect.Value, prefabBounds, localScale2, anchorPrefabGroup.Alignment));
			}
			this.AnchorPrefabSpawnerObjects.Add(anchorInfo, gameObject2);
		}

		private GameObject LabelToPrefab(MRUKAnchor.SceneLabels labels, MRUKAnchor anchor, out AnchorPrefabSpawner.AnchorPrefabGroup prefabGroup)
		{
			foreach (AnchorPrefabSpawner.AnchorPrefabGroup anchorPrefabGroup in this.PrefabsToSpawn)
			{
				if ((anchorPrefabGroup.Labels & labels) != (MRUKAnchor.SceneLabels)0 && ((anchorPrefabGroup.Prefabs != null && anchorPrefabGroup.Prefabs.Count != 0) || anchorPrefabGroup.PrefabSelection == AnchorPrefabSpawner.SelectionMode.Custom))
				{
					GameObject result;
					if (anchorPrefabGroup.PrefabSelection == AnchorPrefabSpawner.SelectionMode.Custom)
					{
						result = this.CustomPrefabSelection(anchor, anchorPrefabGroup.Prefabs);
					}
					else
					{
						result = AnchorPrefabSpawnerUtilities.SelectPrefab(anchor, anchorPrefabGroup.PrefabSelection, anchorPrefabGroup.Prefabs, this._random);
					}
					prefabGroup = anchorPrefabGroup;
					return result;
				}
			}
			prefabGroup = default(AnchorPrefabSpawner.AnchorPrefabGroup);
			return null;
		}

		public void InitializeRandom(ref int seed)
		{
			if (seed == 0)
			{
				seed = Environment.TickCount;
			}
			this._random = new Random(seed);
		}

		public virtual GameObject CustomPrefabSelection(MRUKAnchor anchor, List<GameObject> prefabs)
		{
			throw new Exception("A custom prefab selection method was selected but no implementation was provided. Extend this class and override the `CustomPrefabSelection` method with your custom logic.");
		}

		public virtual Vector3 CustomPrefabScaling(Vector3 localScale)
		{
			throw new NotImplementedException("A custom scaling method for an anchor's volume is selected but no implementation was provided. Extend this class and override the `CustomPrefabVolumeScaling` method with your custom logic.");
		}

		public virtual Vector2 CustomPrefabScaling(Vector2 localScale)
		{
			throw new NotImplementedException("A custom scaling method was selected but no implementation was provided. Extend this class and override the `CustomPrefabPlaneRectScaling` method with your custom logic.");
		}

		public virtual Vector3 CustomPrefabAlignment(Bounds anchorVolumeBounds, Bounds? prefabBounds)
		{
			throw new NotImplementedException("A custom volume alignment method was selected but no implementation was provided.Extend this class and override the `CustomPrefabAlignment` method with your custom logic.");
		}

		public virtual Vector3 CustomPrefabAlignment(Rect anchorPlaneRect, Bounds? prefabBounds)
		{
			throw new NotImplementedException("A custom prefab selection method was selected but no implementation was provided. Extend this class and override the `CustomPrefabAlignment` method with your custom logic.");
		}

		private void OnDestroy()
		{
			this.onPrefabSpawned.RemoveAllListeners();
		}

		[Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
		public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

		[Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
		internal bool TrackUpdates = true;

		[Tooltip("Specify a seed value for consistent prefab selection (0 = Random).")]
		public int SeedValue;

		[Obsolete("Event onPrefabSpawned will be deprecated in a future version")]
		[NonSerialized]
		public UnityEvent onPrefabSpawned = new UnityEvent();

		public List<AnchorPrefabSpawner.AnchorPrefabGroup> PrefabsToSpawn;

		protected Random _random;

		private MRUK.SceneTrackingSettings SceneTrackingSettings;

		private static readonly string Suffix = "(PrefabSpawner Clone)";

		private Func<Vector3, Vector3> _customPrefabScalingVolume;

		private Func<Bounds, Bounds?, ValueTuple<Vector3, Vector3>> _customPrefabAlignmentVolume;

		private Func<Vector2, Vector2> _customPrefabScalingPlaneRect;

		private Func<Rect, Bounds?, ValueTuple<Vector3, Vector2>> _customPrefabAlignmentPlaneRect;

		private Func<MRUKAnchor, List<GameObject>, GameObject> _customPrefabSelection;

		public enum ScalingMode
		{
			Stretch,
			UniformScaling,
			UniformXZScale,
			NoScaling,
			Custom
		}

		public enum AlignMode
		{
			Automatic,
			Bottom,
			Center,
			NoAlignment,
			Custom
		}

		public enum SelectionMode
		{
			Random,
			ClosestSize,
			Custom
		}

		[Serializable]
		public struct AnchorPrefabGroup : IEquatable<AnchorPrefabSpawner.AnchorPrefabGroup>
		{
			public bool Equals(AnchorPrefabSpawner.AnchorPrefabGroup other)
			{
				return this.Labels == other.Labels && object.Equals(this.Prefabs, other.Prefabs) && this.PrefabSelection == other.PrefabSelection && this.MatchAspectRatio == other.MatchAspectRatio && this.CalculateFacingDirection == other.CalculateFacingDirection && this.Scaling == other.Scaling && this.Alignment == other.Alignment && this.IgnorePrefabSize == other.IgnorePrefabSize;
			}

			public override bool Equals(object obj)
			{
				if (obj is AnchorPrefabSpawner.AnchorPrefabGroup)
				{
					AnchorPrefabSpawner.AnchorPrefabGroup other = (AnchorPrefabSpawner.AnchorPrefabGroup)obj;
					return this.Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return HashCode.Combine<int, List<GameObject>, int, bool, bool, int, int, bool>((int)this.Labels, this.Prefabs, (int)this.PrefabSelection, this.MatchAspectRatio, this.CalculateFacingDirection, (int)this.Scaling, (int)this.Alignment, this.IgnorePrefabSize);
			}

			public static bool operator ==(AnchorPrefabSpawner.AnchorPrefabGroup left, AnchorPrefabSpawner.AnchorPrefabGroup right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(AnchorPrefabSpawner.AnchorPrefabGroup left, AnchorPrefabSpawner.AnchorPrefabGroup right)
			{
				return !left.Equals(right);
			}

			[FormerlySerializedAs("_include")]
			[SerializeField]
			[Tooltip("Anchors to include.")]
			public MRUKAnchor.SceneLabels Labels;

			[SerializeField]
			[Tooltip("Prefab(s) to spawn (randomly chosen from list.)")]
			public List<GameObject> Prefabs;

			[SerializeField]
			[Tooltip("The logic that determines what prefab to chose when spawning the relative labels' game objects")]
			public AnchorPrefabSpawner.SelectionMode PrefabSelection;

			[SerializeField]
			[Tooltip("When enabled, the prefab will be rotated to try and match the aspect ratio of the volume as closely as possible. This is most useful for long and thin volumes, keep this disabled for objects with an aspect ratio close to 1:1. Only applies to volumes.")]
			public bool MatchAspectRatio;

			[SerializeField]
			[Tooltip("When calculate facing direction is enabled the prefab will be rotated to face away from the closest wall. If match aspect ratio is also enabled then that will take precedence and it will be constrained to a choice between 2 directions only.Only applies to volumes.")]
			public bool CalculateFacingDirection;

			[SerializeField]
			[Tooltip("Set what scaling mode to apply to the prefab. By default the prefab will be stretched to fit the size of the plane/volume. But in some cases this may not be desirable and can be customized here.")]
			public AnchorPrefabSpawner.ScalingMode Scaling;

			[SerializeField]
			[Tooltip("Spawn new object at the center, top or bottom of the anchor.")]
			public AnchorPrefabSpawner.AlignMode Alignment;

			[SerializeField]
			[Tooltip("Don't analyze prefab, just assume a default scale of 1.")]
			public bool IgnorePrefabSize;
		}
	}
}
