using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	[Feature(Feature.Scene)]
	public class SceneDecorator : MonoBehaviour
	{
		private void Start()
		{
			this._poolManagerSingleton = base.gameObject.AddComponent<PoolManagerSingleton>();
			this._poolManagerComponent = base.gameObject.AddComponent<PoolManagerComponent>();
			this.InitPools();
			OVRTelemetry.Start(651888752, 0, -1L).Send();
			if (MRUK.Instance == null)
			{
				return;
			}
			MRUK.Instance.SceneLoadedEvent.AddListener(delegate()
			{
				if (this.DecorateOnStart == MRUK.RoomFilter.None)
				{
					return;
				}
				MRUK.RoomFilter decorateOnStart2 = this.DecorateOnStart;
				if (decorateOnStart2 == MRUK.RoomFilter.CurrentRoomOnly)
				{
					this.DecorateScene(MRUK.Instance.GetCurrentRoom(), this._recursionDepth);
					return;
				}
				if (decorateOnStart2 != MRUK.RoomFilter.AllRooms)
				{
					return;
				}
				this.DecorateScene(MRUK.Instance.Rooms, this._recursionDepth);
			});
			if (MRUK.Instance.IsInitialized)
			{
				MRUK.RoomFilter decorateOnStart = this.DecorateOnStart;
				if (decorateOnStart != MRUK.RoomFilter.CurrentRoomOnly)
				{
					if (decorateOnStart == MRUK.RoomFilter.AllRooms)
					{
						this.DecorateScene(MRUK.Instance.Rooms, this._recursionDepth);
					}
				}
				else
				{
					this.DecorateScene(MRUK.Instance.GetCurrentRoom(), this._recursionDepth);
				}
			}
			if (!this.TrackUpdates)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRoomCreated));
			MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRoomRemoved));
		}

		private void OnDestroy()
		{
			if (MRUK.Instance == null)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRoomCreated));
			MRUK.Instance.RoomRemovedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRoomRemoved));
		}

		private void InitPools()
		{
			List<PoolManagerComponent.PoolDesc> list = new List<PoolManagerComponent.PoolDesc>();
			foreach (SceneDecoration sceneDecoration in this.sceneDecorations)
			{
				foreach (GameObject primitive in sceneDecoration.decorationPrefabs)
				{
					list.Add(new PoolManagerComponent.PoolDesc
					{
						poolType = PoolManagerComponent.PoolDesc.PoolType.FIXED,
						size = sceneDecoration.Poolsize,
						primitive = primitive,
						callbackProviderOverride = null
					});
				}
			}
			this._poolManagerComponent.defaultPools = list.ToArray();
			this._poolManagerComponent.InitDefaultPools(null);
		}

		private void OnEnable()
		{
			if (!MRUK.Instance)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRoomCreated));
			MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRoomRemoved));
		}

		private void OnDisable()
		{
			if (!MRUK.Instance)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRoomCreated));
			MRUK.Instance.RoomRemovedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRoomRemoved));
		}

		private void ReceiveRoomRemoved(MRUKRoom room)
		{
			this.ClearDecorations(room);
			this.UnRegisterAnchorUpdates(room);
		}

		private void ReceiveRoomCreated(MRUKRoom room)
		{
			if (this.TrackUpdates && this.DecorateOnStart == MRUK.RoomFilter.AllRooms)
			{
				this.DecorateScene(room);
				this.RegisterAnchorUpdates(room);
			}
		}

		private void UnRegisterAnchorUpdates(MRUKRoom room)
		{
			room.AnchorCreatedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorCreated));
			room.AnchorRemovedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorRemoved));
			room.AnchorUpdatedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorUpdated));
		}

		private void ReceiveAnchorUpdated(MRUKAnchor anchor)
		{
			if (this.TrackUpdates)
			{
				this.ClearDecorations(anchor);
				this.Decorate(anchor);
			}
		}

		private void ReceiveAnchorRemoved(MRUKAnchor anchor)
		{
			this.ClearDecorations(anchor);
		}

		private void ReceiveAnchorCreated(MRUKAnchor anchor)
		{
			this.Decorate(anchor);
		}

		private void RegisterAnchorUpdates(MRUKRoom room)
		{
			room.AnchorCreatedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorCreated));
			room.AnchorRemovedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorRemoved));
			room.AnchorUpdatedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorUpdated));
		}

		private void ClearDecorations(MRUKAnchor anchor)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (KeyValuePair<GameObject, MRUKAnchor> keyValuePair in this._spawnedDecorations)
			{
				if (!(keyValuePair.Value != anchor))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (GameObject go in list)
			{
				this._poolManagerSingleton.Release(go);
			}
		}

		private void ClearDecorations(MRUKRoom room)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (KeyValuePair<GameObject, MRUKAnchor> keyValuePair in this._spawnedDecorations)
			{
				if (!(keyValuePair.Value.Room != room))
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (GameObject go in list)
			{
				this._poolManagerSingleton.Release(go);
			}
		}

		public void ClearDecorations()
		{
			foreach (KeyValuePair<GameObject, MRUKAnchor> keyValuePair in this._spawnedDecorations)
			{
				this._poolManagerSingleton.Release(keyValuePair.Key);
			}
		}

		private void DecorateScene(MRUKRoom room)
		{
			this.DecorateScene(room, 0);
		}

		public void DecorateScene()
		{
			foreach (MRUKRoom room in MRUK.Instance.Rooms)
			{
				this.DecorateScene(room, 0);
			}
		}

		private void DecorateScene(List<MRUKRoom> rooms, int recursionDepth)
		{
			foreach (MRUKRoom room in rooms)
			{
				this.DecorateScene(room, recursionDepth);
			}
		}

		private void DecorateScene(MRUKRoom room, int recursionDepth)
		{
			if (recursionDepth >= this.recursionLimit)
			{
				return;
			}
			foreach (SceneDecoration sceneDecoration in this.sceneDecorations)
			{
				this.Decorate(room, sceneDecoration);
			}
		}

		private void Decorate(MRUKAnchor anchor)
		{
			foreach (SceneDecoration sceneDecoration in this.sceneDecorations)
			{
				this.Decorate(anchor, sceneDecoration);
			}
		}

		private void Decorate(MRUKAnchor anchor, SceneDecoration sceneDecoration)
		{
			MRUKAnchor.SceneLabels executeSceneLabels = sceneDecoration.executeSceneLabels;
			if (anchor.Label == executeSceneLabels)
			{
				this.Distribute(anchor, sceneDecoration);
			}
		}

		private void Decorate(MRUKRoom room, SceneDecoration sceneDecoration)
		{
			MRUKAnchor.SceneLabels executeSceneLabels = sceneDecoration.executeSceneLabels;
			foreach (object obj in Enum.GetValues(typeof(MRUKAnchor.SceneLabels)))
			{
				MRUKAnchor.SceneLabels sceneLabels = (MRUKAnchor.SceneLabels)obj;
				if ((executeSceneLabels & sceneLabels) != (MRUKAnchor.SceneLabels)0)
				{
					List<MRUKAnchor> anchorsWithLabel = this.GetAnchorsWithLabel(room, sceneLabels);
					if (anchorsWithLabel != null)
					{
						foreach (MRUKAnchor sceneAnchor in anchorsWithLabel)
						{
							this.Distribute(sceneAnchor, sceneDecoration);
						}
					}
				}
			}
		}

		private void Distribute(MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration)
		{
			if (sceneDecoration.decorationPrefabs.Length == 0)
			{
				Debug.LogWarning("No decoration prefab added to " + sceneDecoration.name);
				return;
			}
			if (this._poolManagerComponent.defaultPools.Length == 0)
			{
				this.InitPools();
			}
			switch (sceneDecoration.distributionType)
			{
			case DistributionType.GRID:
				sceneDecoration.gridDistribution.Distribute(this, sceneAnchor, sceneDecoration);
				return;
			case DistributionType.SIMPLEX:
				sceneDecoration.simplexDistribution.Distribute(this, sceneAnchor, sceneDecoration);
				return;
			case DistributionType.STAGGERED_CONCENTRIC:
				sceneDecoration.staggeredConcentricDistribution.Distribute(this, sceneAnchor, sceneDecoration);
				return;
			}
			sceneDecoration.randomDistribution.Distribute(this, sceneAnchor, sceneDecoration);
		}

		private static void TestCollider(Collider c, Vector3 worldPos, Vector3 rayDir, SceneDecoration sceneDecoration, ref RaycastHit closestHit)
		{
			rayDir = (sceneDecoration.selectBehind ? (-rayDir) : rayDir);
			RaycastHit raycastHit;
			if (c.Raycast(new Ray(worldPos, rayDir), out raycastHit, float.PositiveInfinity) && raycastHit.distance < Mathf.Abs(closestHit.distance))
			{
				closestHit = raycastHit;
				closestHit.distance = (sceneDecoration.selectBehind ? (-closestHit.distance) : closestHit.distance);
				if (sceneDecoration.DrawDebugRaysAndImpactPoints)
				{
					Debug.DrawLine(worldPos, closestHit.point, Color.magenta, 3600f);
					Utilities.DrawWireSphere(worldPos, 0.05f, Color.cyan, 3600f, 3);
					Utilities.DrawWireSphere(closestHit.point, 0.05f, Color.blue, 3600f, 3);
				}
			}
		}

		private static void TestPhysicsLayers(Vector3 worldPos, Vector3 rayDir, SceneDecoration sceneDecoration, ref RaycastHit closestHit)
		{
			rayDir = (sceneDecoration.selectBehind ? (-rayDir) : rayDir);
			if (sceneDecoration.DrawDebugRaysAndImpactPoints)
			{
				Utilities.DrawWireSphere(worldPos, 0.05f, Color.cyan, 3600f, 3);
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(worldPos, rayDir), out raycastHit, float.PositiveInfinity, sceneDecoration.targetPhysicsLayers) && raycastHit.distance < Mathf.Abs(closestHit.distance))
			{
				closestHit = raycastHit;
				closestHit.distance = (sceneDecoration.selectBehind ? (-closestHit.distance) : closestHit.distance);
				if (sceneDecoration.DrawDebugRaysAndImpactPoints)
				{
					Debug.DrawLine(worldPos, closestHit.point, Color.red, 3600f);
					Utilities.DrawWireSphere(closestHit.point, 0.05f, Color.blue, 3600f, 3);
				}
			}
		}

		private bool TestConstraints(SceneDecoration sceneDecoration, Candidate c)
		{
			foreach (Constraint constraint in sceneDecoration.constraints)
			{
				if (constraint.enabled)
				{
					ConstraintModeCheck modeCheck = constraint.modeCheck;
					float num = constraint.mask.SampleMask(c);
					bool flag = constraint.mask.Check(c);
					if (modeCheck != ConstraintModeCheck.Value)
					{
						if (modeCheck != ConstraintModeCheck.Bool)
						{
							throw new ArgumentOutOfRangeException();
						}
						if (!flag)
						{
							return false;
						}
					}
					else if (num < constraint.min | num > constraint.max)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void ApplyModifiers(GameObject decorationGO, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration, Candidate candidate)
		{
			foreach (Modifier modifier in sceneDecoration.modifiers)
			{
				if (modifier.enabled)
				{
					modifier.ApplyModifier(decorationGO, sceneAnchor, sceneDecoration, candidate);
				}
			}
		}

		public void GenerateOn(Vector2 localPos, Vector2 localPosNormalized, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration)
		{
			Vector3 position = Vector3.zero;
			if (sceneDecoration.placement == Placement.SPHERICAL)
			{
				localPos *= new Vector2(2f * SceneDecorator.PI, SceneDecorator.PI);
				float num = Mathf.Sin(localPos.x);
				position = new Vector3(num * Mathf.Cos(localPos.y), num * Mathf.Sin(localPos.y), Mathf.Cos(localPos.x));
			}
			else
			{
				position = new Vector3(localPos.x, localPos.y, 0f) + sceneDecoration.rayOffset;
			}
			this.GenerateAt(sceneAnchor.transform.TransformPoint(position), localPos, localPosNormalized, sceneAnchor, sceneDecoration);
		}

		private void GenerateAt(Vector3 worldPos, Vector2 localPos, Vector2 localPosNormalized, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration)
		{
			Vector3 vector = sceneDecoration.placementDirection;
			switch (sceneDecoration.placement)
			{
			case Placement.LOCAL_PLANAR:
				vector = sceneAnchor.transform.rotation * vector;
				break;
			case Placement.SPHERICAL:
				vector = (worldPos - sceneAnchor.transform.position).normalized;
				break;
			}
			RaycastHit hit = new RaycastHit
			{
				distance = float.PositiveInfinity
			};
			Target targets = sceneDecoration.targets;
			foreach (object obj in Enum.GetValues(typeof(Target)))
			{
				Target target = (Target)obj;
				if ((targets & target) != (Target)0)
				{
					if (target <= Target.PHYSICS_LAYERS)
					{
						if (target - Target.GLOBAL_MESH > 1)
						{
							if (target == Target.PHYSICS_LAYERS)
							{
								SceneDecorator.TestPhysicsLayers(worldPos, vector, sceneDecoration, ref hit);
								continue;
							}
						}
					}
					else
					{
						if (target == Target.CUSTOM_COLLIDERS)
						{
							Collider[] array = this.customColliders;
							for (int i = 0; i < array.Length; i++)
							{
								SceneDecorator.TestCollider(array[i], worldPos, vector, sceneDecoration, ref hit);
							}
							continue;
						}
						if (target == Target.CUSTOM_TAGS)
						{
							foreach (string tag in this.customTargetTags)
							{
								if (sceneAnchor.gameObject.CompareTag(tag))
								{
									MeshCollider componentInChildren = sceneAnchor.gameObject.GetComponentInChildren<MeshCollider>();
									if (!(componentInChildren == null))
									{
										SceneDecorator.TestCollider(componentInChildren, worldPos, vector, sceneDecoration, ref hit);
									}
								}
							}
							continue;
						}
						if (target == Target.SCENE_ANCHORS)
						{
							Ray ray = new Ray(worldPos, vector);
							sceneAnchor.Raycast(ray, float.PositiveInfinity, out hit, MRUKAnchor.ComponentType.All);
							continue;
						}
					}
					if (sceneAnchor.Room.GlobalMeshAnchor != null)
					{
						MeshCollider componentInChildren2 = sceneAnchor.Room.GlobalMeshAnchor.gameObject.GetComponentInChildren<MeshCollider>();
						if (!(componentInChildren2 == null))
						{
							SceneDecorator.TestCollider(componentInChildren2, worldPos, vector, sceneDecoration, ref hit);
						}
					}
				}
			}
			if (float.IsPositiveInfinity(hit.distance))
			{
				return;
			}
			Vector3 anchorCompDists;
			float closestSurfacePosition = sceneAnchor.GetClosestSurfacePosition(hit.point, out anchorCompDists, MRUKAnchor.ComponentType.All);
			GameObject gameObject = sceneDecoration.decorationPrefabs[Random.Range(0, sceneDecoration.decorationPrefabs.Length)];
			Candidate candidate = new Candidate
			{
				decorationPrefab = gameObject,
				localPos = localPos,
				localPosNormalized = localPosNormalized,
				hit = hit,
				anchorDist = closestSurfacePosition,
				anchorCompDists = anchorCompDists,
				slope = 57.29578f * Mathf.Acos(Vector3.Dot(hit.normal, -vector))
			};
			if (!this.TestConstraints(sceneDecoration, candidate))
			{
				return;
			}
			Transform transform;
			switch (sceneDecoration.spawnHierarchy)
			{
			default:
				transform = null;
				break;
			case SpawnHierarchy.SCENE_DECORATOR_CHILD:
				transform = base.gameObject.transform;
				break;
			case SpawnHierarchy.ANCHOR_CHILD:
				transform = sceneAnchor.transform;
				break;
			case SpawnHierarchy.TARGET_CHILD:
				transform = hit.transform;
				break;
			case SpawnHierarchy.TARGET_COLLIDER_CHILD:
				transform = ((hit.collider == null) ? null : hit.collider.transform);
				break;
			}
			gameObject = this._poolManagerSingleton.Create(gameObject, hit.point, Quaternion.identity, sceneAnchor, transform);
			if (gameObject == null)
			{
				return;
			}
			this._spawnedDecorations[gameObject] = sceneAnchor;
			if (sceneDecoration.lifetime > 0f)
			{
				Object.Destroy(gameObject, sceneDecoration.lifetime);
			}
			SceneDecorator sceneDecorator;
			if (gameObject.TryGetComponent<SceneDecorator>(out sceneDecorator))
			{
				sceneDecorator._parent = this;
				sceneDecorator._recursionDepth = this._recursionDepth + 1;
			}
			if (transform != null & sceneDecoration.discardParentScaling)
			{
				Vector3 localScale = gameObject.transform.localScale;
				Vector3 lossyScale = transform.lossyScale;
				localScale.x *= 1f / lossyScale.x;
				localScale.y *= 1f / lossyScale.y;
				localScale.z *= 1f / lossyScale.z;
				gameObject.transform.localScale = localScale;
			}
			this.ApplyModifiers(gameObject, sceneAnchor, sceneDecoration, candidate);
		}

		private List<MRUKAnchor> GetAnchorsWithLabel(MRUKRoom room, MRUKAnchor.SceneLabels label)
		{
			List<MRUKAnchor> list = new List<MRUKAnchor>();
			foreach (MRUKAnchor mrukanchor in room.Anchors)
			{
				if (mrukanchor.Label == label)
				{
					list.Add(mrukanchor);
				}
			}
			return list;
		}

		public static readonly float PI = 3.14159f;

		[SerializeField]
		public List<SceneDecoration> sceneDecorations;

		[SerializeField]
		public Collider[] customColliders;

		[SerializeField]
		public string[] customTargetTags;

		[SerializeField]
		public int recursionLimit = 3;

		private int _recursionDepth;

		private SceneDecorator _parent;

		[Tooltip("When the scene data is loaded, this controls what room(s) the decorator will add decorations.")]
		public MRUK.RoomFilter DecorateOnStart = MRUK.RoomFilter.AllRooms;

		[Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
		internal bool TrackUpdates = true;

		private PoolManagerComponent _poolManagerComponent;

		private PoolManagerSingleton _poolManagerSingleton;

		private Dictionary<GameObject, MRUKAnchor> _spawnedDecorations = new Dictionary<GameObject, MRUKAnchor>();

		public interface IDistribution
		{
			void Distribute(SceneDecorator sceneDecorator, MRUKAnchor sceneAnchor, SceneDecoration sceneDecoration);
		}
	}
}
