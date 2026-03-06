using System;
using System.Collections.Generic;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Meta.XR.MRUtilityKit
{
	[Feature(Feature.Scene)]
	[HelpURL("https://developers.meta.com/horizon/reference/mruk/latest/class_meta_x_r_m_r_utility_kit_effect_mesh")]
	public class EffectMesh : MonoBehaviour
	{
		public bool CastShadow
		{
			get
			{
				return this.castShadows;
			}
			set
			{
				this.ToggleShadowCasting(value, default(LabelFilter));
				this.castShadows = value;
			}
		}

		public bool HideMesh
		{
			get
			{
				return this.hideMesh;
			}
			set
			{
				this.ToggleEffectMeshVisibility(!value, default(LabelFilter), null);
				this.hideMesh = value;
			}
		}

		[Obsolete("This property is deprecated. Please use 'ToggleEffectMeshColliders' instead.")]
		public bool ToggleColliders
		{
			get
			{
				return this.Colliders;
			}
			set
			{
				this.ToggleEffectMeshColliders(!value, default(LabelFilter));
				this.Colliders = value;
			}
		}

		public IReadOnlyDictionary<MRUKAnchor, EffectMesh.EffectMeshObject> EffectMeshObjects
		{
			get
			{
				return this.effectMeshObjects;
			}
		}

		private void Start()
		{
			OVRTelemetry.Start(651897605, 0, -1L).Send();
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
				MRUK.RoomFilter spawnOnStart = this.SpawnOnStart;
				if (spawnOnStart == MRUK.RoomFilter.CurrentRoomOnly)
				{
					this.CreateMesh(MRUK.Instance.GetCurrentRoom());
					return;
				}
				if (spawnOnStart != MRUK.RoomFilter.AllRooms)
				{
					return;
				}
				this.CreateMesh();
			});
			if (!this.TrackUpdates)
			{
				return;
			}
			MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
			MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
		}

		private void ReceiveRemovedRoom(MRUKRoom room)
		{
			this.DestroyMesh(room);
			this.UnregisterAnchorUpdates(room);
		}

		private void UnregisterAnchorUpdates(MRUKRoom room)
		{
			room.AnchorCreatedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorCreatedEvent));
			room.AnchorRemovedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorRemovedCallback));
			room.AnchorUpdatedEvent.RemoveListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorUpdatedCallback));
		}

		private void RegisterAnchorUpdates(MRUKRoom room)
		{
			room.AnchorCreatedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorCreatedEvent));
			room.AnchorRemovedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorRemovedCallback));
			room.AnchorUpdatedEvent.AddListener(new UnityAction<MRUKAnchor>(this.ReceiveAnchorUpdatedCallback));
		}

		private void ReceiveAnchorUpdatedCallback(MRUKAnchor anchor)
		{
			if (this.SceneTrackingSettings.UnTrackedRooms.Contains(anchor.Room) || this.SceneTrackingSettings.UnTrackedAnchors.Contains(anchor) || !this.TrackUpdates)
			{
				return;
			}
			if (anchor.HasAnyLabel(this.Labels))
			{
				this.DestroyMesh(anchor);
				this.CreateEffectMesh(anchor);
			}
		}

		private void ReceiveAnchorRemovedCallback(MRUKAnchor anchor)
		{
			this.DestroyMesh(anchor);
		}

		private void ReceiveAnchorCreatedEvent(MRUKAnchor anchor)
		{
			if (this.SceneTrackingSettings.UnTrackedRooms.Contains(anchor.Room) || !this.TrackUpdates)
			{
				return;
			}
			if (anchor.HasAnyLabel(this.Labels))
			{
				this.CreateEffectMesh(anchor);
			}
		}

		private void ReceiveCreatedRoom(MRUKRoom room)
		{
			if (this.TrackUpdates && this.SpawnOnStart == MRUK.RoomFilter.AllRooms)
			{
				this.CreateMesh(room);
			}
		}

		public void CreateMesh()
		{
			foreach (MRUKRoom room in MRUK.Instance.Rooms)
			{
				this.CreateMesh(room);
			}
		}

		public void DestroyMesh(LabelFilter label = default(LabelFilter))
		{
			List<MRUKAnchor> list = new List<MRUKAnchor>();
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				bool flag = label.PassesFilter(keyValuePair.Key.Label);
				if (keyValuePair.Value.effectMeshGO && flag)
				{
					Object.DestroyImmediate(keyValuePair.Value.effectMeshGO);
					list.Add(keyValuePair.Key);
				}
			}
			foreach (MRUKAnchor mrukanchor in list)
			{
				this.effectMeshObjects.Remove(mrukanchor);
				this.SceneTrackingSettings.UnTrackedAnchors.Add(mrukanchor);
			}
		}

		public void DestroyMesh(MRUKRoom room)
		{
			foreach (MRUKAnchor anchor in room.Anchors)
			{
				this.DestroyMesh(anchor);
			}
			this.SceneTrackingSettings.UnTrackedRooms.Add(room);
		}

		public void DestroyMesh(MRUKAnchor anchor)
		{
			EffectMesh.EffectMeshObject effectMeshObject;
			if (this.effectMeshObjects.TryGetValue(anchor, out effectMeshObject) && effectMeshObject.effectMeshGO)
			{
				Object.DestroyImmediate(effectMeshObject.effectMeshGO);
				this.effectMeshObjects.Remove(anchor);
				this.SceneTrackingSettings.UnTrackedAnchors.Add(anchor);
			}
		}

		public void AddColliders(LabelFilter label = default(LabelFilter))
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				bool flag = label.PassesFilter(keyValuePair.Key.Label);
				if (keyValuePair.Key && !keyValuePair.Value.collider && flag)
				{
					keyValuePair.Value.collider = this.AddCollider(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public void DestroyColliders(LabelFilter label = default(LabelFilter))
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				bool flag = label.PassesFilter(keyValuePair.Key.Label);
				if (keyValuePair.Value.collider && flag)
				{
					Object.DestroyImmediate(keyValuePair.Value.collider);
				}
			}
		}

		public void ToggleShadowCasting(bool shouldCast, LabelFilter label = default(LabelFilter))
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				bool flag = label.PassesFilter(keyValuePair.Key.Label);
				if (keyValuePair.Value.effectMeshGO && flag)
				{
					ShadowCastingMode shadowCastingMode = this.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
					keyValuePair.Value.effectMeshGO.GetComponent<MeshRenderer>().shadowCastingMode = shadowCastingMode;
				}
			}
		}

		public void ToggleEffectMeshVisibility(bool shouldShow, LabelFilter label = default(LabelFilter), Material materialOverride = null)
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				bool flag = label.PassesFilter(keyValuePair.Key.Label);
				if (keyValuePair.Value.effectMeshGO && flag)
				{
					keyValuePair.Value.effectMeshGO.GetComponent<MeshRenderer>().enabled = shouldShow;
					if (materialOverride)
					{
						keyValuePair.Value.effectMeshGO.GetComponent<MeshRenderer>().material = materialOverride;
					}
				}
			}
		}

		public void ToggleEffectMeshColliders(bool doEnable, LabelFilter label = default(LabelFilter))
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				if (label.PassesFilter(keyValuePair.Key.Label))
				{
					if (!keyValuePair.Value.collider)
					{
						this.AddCollider(keyValuePair.Key, keyValuePair.Value);
					}
					keyValuePair.Value.collider.enabled = doEnable;
				}
			}
		}

		public void OverrideEffectMaterial(Material newMaterial, LabelFilter label = default(LabelFilter))
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				bool flag = label.PassesFilter(keyValuePair.Key.Label);
				if (keyValuePair.Value.effectMeshGO && flag)
				{
					keyValuePair.Value.effectMeshGO.GetComponent<MeshRenderer>().material = newMaterial;
				}
			}
		}

		private static void OrderWalls(List<MRUKAnchor> walls)
		{
			int count = walls.Count;
			if (count <= 1)
			{
				return;
			}
			List<MRUKAnchor> list;
			using (new OVRObjectPool.ListScope<MRUKAnchor>(ref list))
			{
				int index = count - 1;
				MRUKAnchor mrukanchor = walls[index];
				list.Add(mrukanchor);
				walls.RemoveAt(index);
				while (walls.Count > 0)
				{
					float num = float.MaxValue;
					int index2 = -1;
					Vector3 a = mrukanchor.transform.position + mrukanchor.transform.right * mrukanchor.PlaneRect.Value.min.x;
					for (int i = 0; i < walls.Count; i++)
					{
						MRUKAnchor mrukanchor2 = walls[i];
						Vector3 b = mrukanchor2.transform.position + mrukanchor2.transform.right * mrukanchor2.PlaneRect.Value.max.x;
						float num2 = Vector3.Distance(a, b);
						if (num2 < num)
						{
							num = num2;
							index2 = i;
						}
					}
					mrukanchor = walls[index2];
					list.Add(mrukanchor);
					walls.RemoveAt(index2);
				}
				walls.AddRange(list);
			}
		}

		public void CreateMesh(MRUKRoom room)
		{
			this.CreateMesh(room, null);
		}

		private unsafe void CreateMesh(MRUKRoom room, List<MRUKRoom> connectedRooms)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)4];
			*intPtr = 32772;
			Span<MRUKAnchor.SceneLabels> span = new Span<MRUKAnchor.SceneLabels>(intPtr, 1);
			MRUKAnchor.SceneLabels sceneLabels = (MRUKAnchor.SceneLabels)0;
			Span<MRUKAnchor.SceneLabels> span2 = span;
			for (int i = 0; i < span2.Length; i++)
			{
				MRUKAnchor.SceneLabels sceneLabels2 = *span2[i];
				sceneLabels |= sceneLabels2;
			}
			foreach (MRUKAnchor mrukanchor in room.Anchors)
			{
				if (mrukanchor.HasAnyLabel(this.Labels) && !mrukanchor.HasAnyLabel(sceneLabels))
				{
					if (mrukanchor.HasAnyLabel(MRUKAnchor.SceneLabels.GLOBAL_MESH))
					{
						this.CreateGlobalMeshObject(mrukanchor);
					}
					else
					{
						this.CreateEffectMesh(mrukanchor);
					}
				}
			}
			float num = 0f;
			foreach (MRUKAnchor mrukanchor2 in room.Anchors)
			{
				if (mrukanchor2.HasAnyLabel(sceneLabels))
				{
					num += mrukanchor2.PlaneRect.Value.size.x;
				}
			}
			List<MRUKAnchor> list;
			using (new OVRObjectPool.ListScope<MRUKAnchor>(ref list))
			{
				float num2 = 0f;
				span2 = span;
				for (int i = 0; i < span2.Length; i++)
				{
					MRUKAnchor.SceneLabels sceneLabels3 = *span2[i];
					if (this.IncludesLabel(sceneLabels3))
					{
						list.Clear();
						foreach (MRUKAnchor mrukanchor3 in room.Anchors)
						{
							if (mrukanchor3.HasAnyLabel(sceneLabels3))
							{
								list.Add(mrukanchor3);
							}
						}
						EffectMesh.OrderWalls(list);
						foreach (MRUKAnchor mrukanchor4 in list)
						{
							if (this.IncludesLabel(mrukanchor4.Label))
							{
								this.CreateEffectMeshWall(mrukanchor4, num, ref num2, connectedRooms);
							}
						}
					}
				}
			}
			this.RegisterAnchorUpdates(room);
			if (!this.TrackUpdates)
			{
				this.SceneTrackingSettings.UnTrackedRooms.Add(room);
			}
		}

		private bool IncludesLabel(MRUKAnchor.SceneLabels label)
		{
			return (this.Labels & label) > (MRUKAnchor.SceneLabels)0;
		}

		public EffectMesh.EffectMeshObject CreateEffectMesh(MRUKAnchor anchorInfo)
		{
			if (this.effectMeshObjects.ContainsKey(anchorInfo))
			{
				return null;
			}
			EffectMesh.EffectMeshObject effectMeshObject = new EffectMesh.EffectMeshObject();
			Mesh mesh = Utilities.SetupAnchorMeshGeometry(anchorInfo, false, this.textureCoordinateModes);
			GameObject gameObject = new GameObject(anchorInfo.name + EffectMesh.Suffix);
			gameObject.transform.SetParent(anchorInfo.transform, false);
			gameObject.layer = this.Layer;
			effectMeshObject.effectMeshGO = gameObject;
			gameObject.AddComponent<MeshFilter>().mesh = mesh;
			if (this.MeshMaterial != null)
			{
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.material = this.MeshMaterial;
				meshRenderer.shadowCastingMode = (this.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off);
				meshRenderer.enabled = !this.hideMesh;
			}
			mesh.name = anchorInfo.name;
			effectMeshObject.mesh = mesh;
			if (this.Colliders)
			{
				effectMeshObject.collider = this.AddCollider(anchorInfo, effectMeshObject);
			}
			this.effectMeshObjects.Add(anchorInfo, effectMeshObject);
			return effectMeshObject;
		}

		private Collider AddCollider(MRUKAnchor anchorInfo, EffectMesh.EffectMeshObject effectMeshObject)
		{
			if (anchorInfo.VolumeBounds != null)
			{
				BoxCollider boxCollider = effectMeshObject.effectMeshGO.AddComponent<BoxCollider>();
				boxCollider.size = anchorInfo.VolumeBounds.Value.size;
				boxCollider.center = anchorInfo.VolumeBounds.Value.center;
				return boxCollider;
			}
			MeshCollider meshCollider = effectMeshObject.effectMeshGO.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = effectMeshObject.mesh;
			meshCollider.convex = false;
			return meshCollider;
		}

		private float GetSeamlessFactor(float totalWallLength, float stepSize)
		{
			float num = Mathf.Round(totalWallLength / stepSize);
			num = Mathf.Max(1f, num);
			return totalWallLength / num;
		}

		private void CreateEffectMeshWall(MRUKAnchor anchorInfo, float totalWallLength, ref float uSpacing, List<MRUKRoom> connectedRooms)
		{
			if (this.effectMeshObjects.ContainsKey(anchorInfo))
			{
				return;
			}
			EffectMesh.EffectMeshObject effectMeshObject = new EffectMesh.EffectMeshObject();
			GameObject gameObject = new GameObject(anchorInfo.name + EffectMesh.Suffix);
			gameObject.layer = this.Layer;
			gameObject.transform.SetParent(anchorInfo.transform, false);
			effectMeshObject.effectMeshGO = gameObject;
			Mesh mesh = new Mesh();
			gameObject.AddComponent<MeshFilter>().mesh = mesh;
			if (this.MeshMaterial != null)
			{
				MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
				meshRenderer.material = this.MeshMaterial;
				meshRenderer.shadowCastingMode = (this.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off);
				meshRenderer.enabled = !this.hideMesh;
			}
			List<List<Vector2>> list = null;
			Rect value = anchorInfo.PlaneRect.Value;
			foreach (MRUKAnchor mrukanchor in anchorInfo.ChildAnchors)
			{
				if (mrukanchor.PlaneRect != null && (mrukanchor.Label & this.CutHoles) > (MRUKAnchor.SceneLabels)0)
				{
					Vector2 vector = anchorInfo.transform.InverseTransformPoint(mrukanchor.transform.position);
					Rect rect;
					mrukanchor.PlaneRect.Value.position = rect.position + new Vector2(vector.x, vector.y);
					List<Vector2> list2 = new List<Vector2>(mrukanchor.PlaneBoundary2D.Count);
					for (int i = mrukanchor.PlaneBoundary2D.Count - 1; i >= 0; i--)
					{
						list2.Add(mrukanchor.PlaneBoundary2D[i] + vector);
					}
					if (list == null)
					{
						list = new List<List<Vector2>>();
					}
					list.Add(list2);
				}
			}
			Vector2[] array;
			int[] array2;
			Triangulator.TriangulatePoints(anchorInfo.PlaneBoundary2D, list, out array, out array2);
			int num = array.Length;
			int num2 = Math.Min(8, this.textureCoordinateModes.Length);
			Vector3[] array3 = new Vector3[num];
			for (int j = 0; j < array.Length; j++)
			{
				array3[j] = array[j];
			}
			Vector2[][] array4 = new Vector2[num2][];
			for (int k = 0; k < num2; k++)
			{
				array4[k] = new Vector2[num];
			}
			Color32[] array5 = new Color32[num];
			Vector3[] array6 = new Vector3[num];
			Vector4[] array7 = new Vector4[num];
			int num3 = 0;
			float seamlessFactor = this.GetSeamlessFactor(totalWallLength, 1f);
			float width = value.width;
			Vector3 forward = Vector3.forward;
			Vector4 vector2 = new Vector4(1f, 0f, 0f, 1f);
			for (int l = 0; l < array.Length; l++)
			{
				Vector3 vector3 = array3[l];
				float num4 = vector3.x - value.xMin;
				float num5 = vector3.y - value.yMin;
				for (int m = 0; m < num2; m++)
				{
					float num6 = uSpacing;
					EffectMesh.WallTextureCoordinateModeV wallV = this.textureCoordinateModes[m].WallV;
					float num7;
					if (wallV != EffectMesh.WallTextureCoordinateModeV.METRIC)
					{
						num7 = value.height;
					}
					else
					{
						num7 = 1f;
					}
					float num8;
					switch (this.textureCoordinateModes[m].WallU)
					{
					case EffectMesh.WallTextureCoordinateModeU.METRIC:
						num8 = 1f;
						break;
					case EffectMesh.WallTextureCoordinateModeU.METRIC_SEAMLESS:
						num8 = seamlessFactor;
						break;
					case EffectMesh.WallTextureCoordinateModeU.MAINTAIN_ASPECT_RATIO:
						num8 = num7;
						break;
					case EffectMesh.WallTextureCoordinateModeU.MAINTAIN_ASPECT_RATIO_SEAMLESS:
						num8 = this.GetSeamlessFactor(totalWallLength, num7);
						break;
					default:
						num8 = totalWallLength;
						break;
					case EffectMesh.WallTextureCoordinateModeU.STRETCH_SECTION:
						num8 = width;
						num6 = 0f;
						break;
					}
					if (this.textureCoordinateModes[m].WallV == EffectMesh.WallTextureCoordinateModeV.MAINTAIN_ASPECT_RATIO)
					{
						num7 = num8;
					}
					array4[m][num3] = new Vector2((num6 + width - num4) / num8, num5 / num7);
				}
				array3[num3] = new Vector3(vector3.x, vector3.y, 0f);
				array5[num3] = Color.white;
				array6[num3] = forward;
				array7[num3] = vector2;
				num3++;
			}
			uSpacing += width;
			int[] triangles = array2;
			mesh.Clear();
			mesh.name = anchorInfo.name;
			mesh.vertices = array3;
			for (int n = 0; n < num2; n++)
			{
				switch (n)
				{
				case 0:
					mesh.uv = array4[n];
					break;
				case 1:
					mesh.uv2 = array4[n];
					break;
				case 2:
					mesh.uv3 = array4[n];
					break;
				case 3:
					mesh.uv4 = array4[n];
					break;
				case 4:
					mesh.uv5 = array4[n];
					break;
				case 5:
					mesh.uv6 = array4[n];
					break;
				case 6:
					mesh.uv7 = array4[n];
					break;
				case 7:
					mesh.uv8 = array4[n];
					break;
				}
			}
			mesh.colors32 = array5;
			mesh.triangles = triangles;
			mesh.normals = array6;
			mesh.tangents = array7;
			effectMeshObject.mesh = mesh;
			if (this.Colliders)
			{
				effectMeshObject.collider = this.AddCollider(anchorInfo, effectMeshObject);
			}
			this.effectMeshObjects.Add(anchorInfo, effectMeshObject);
		}

		private void CreateGlobalMeshObject(MRUKAnchor globalMeshAnchor)
		{
			if (!globalMeshAnchor)
			{
				Debug.LogWarning("No global mesh was found in the current room");
				return;
			}
			if (this.effectMeshObjects.ContainsKey(globalMeshAnchor))
			{
				return;
			}
			EffectMesh.EffectMeshObject effectMeshObject = new EffectMesh.EffectMeshObject();
			GameObject gameObject = new GameObject(globalMeshAnchor.name + EffectMesh.Suffix, new Type[]
			{
				typeof(MeshFilter),
				typeof(MeshRenderer)
			});
			gameObject.layer = this.Layer;
			gameObject.transform.SetParent(globalMeshAnchor.transform, false);
			effectMeshObject.effectMeshGO = gameObject;
			globalMeshAnchor.Mesh.RecalculateNormals();
			Mesh mesh = globalMeshAnchor.Mesh;
			gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
			if (this.Colliders)
			{
				MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
				meshCollider.sharedMesh = mesh;
				effectMeshObject.collider = meshCollider;
			}
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			if (this.MeshMaterial != null)
			{
				component.material = this.MeshMaterial;
			}
			component.enabled = !this.hideMesh;
			component.shadowCastingMode = (this.castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off);
			effectMeshObject.mesh = mesh;
			this.effectMeshObjects.Add(globalMeshAnchor, effectMeshObject);
		}

		public void SetEffectObjectsParent(Transform newParent)
		{
			foreach (KeyValuePair<MRUKAnchor, EffectMesh.EffectMeshObject> keyValuePair in this.effectMeshObjects)
			{
				keyValuePair.Value.effectMeshGO.transform.SetParent(newParent);
			}
		}

		[Tooltip("When the scene data is loaded, this controls what room(s) the effect mesh is applied to.")]
		public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

		[Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
		internal bool TrackUpdates = true;

		[Tooltip("The material applied to the generated mesh. If you'd like a multi-material room, you can use another EffectMesh object with a different Mesh Material.")]
		[FormerlySerializedAs("_MeshMaterial")]
		public Material MeshMaterial;

		[Obsolete("BorderSize functionality has been removed.")]
		[FormerlySerializedAs("_borderSize")]
		[NonSerialized]
		public float BorderSize;

		[Tooltip("Generate a BoxCollider for each mesh component.")]
		[FormerlySerializedAs("addColliders")]
		public bool Colliders;

		[Tooltip("Cut holes in the mesh for door frames and/or window frames. NOTE: This does not apply if border size is non-zero.")]
		public MRUKAnchor.SceneLabels CutHoles;

		[Tooltip("Whether the effect mesh objects will cast a shadow.")]
		[SerializeField]
		private bool castShadows = true;

		[Tooltip("Hide the effect mesh.")]
		[SerializeField]
		private bool hideMesh;

		private MRUK.SceneTrackingSettings SceneTrackingSettings;

		[HideInInspector]
		public int Layer;

		[Tooltip("Can not exceed 8.")]
		public EffectMesh.TextureCoordinateModes[] textureCoordinateModes = new EffectMesh.TextureCoordinateModes[]
		{
			new EffectMesh.TextureCoordinateModes()
		};

		[Tooltip("Specifies the scene labels that determine which anchors representations are created by the effect mesh.")]
		[FormerlySerializedAs("_include")]
		public MRUKAnchor.SceneLabels Labels;

		private static readonly string Suffix = "_EffectMesh";

		private Dictionary<MRUKAnchor, EffectMesh.EffectMeshObject> effectMeshObjects = new Dictionary<MRUKAnchor, EffectMesh.EffectMeshObject>();

		public enum WallTextureCoordinateModeU
		{
			METRIC,
			METRIC_SEAMLESS,
			MAINTAIN_ASPECT_RATIO,
			MAINTAIN_ASPECT_RATIO_SEAMLESS,
			STRETCH,
			STRETCH_SECTION
		}

		public enum WallTextureCoordinateModeV
		{
			METRIC,
			MAINTAIN_ASPECT_RATIO,
			STRETCH
		}

		public enum AnchorTextureCoordinateMode
		{
			METRIC,
			STRETCH
		}

		[Serializable]
		public class TextureCoordinateModes
		{
			[FormerlySerializedAs("U")]
			public EffectMesh.WallTextureCoordinateModeU WallU;

			[FormerlySerializedAs("V")]
			public EffectMesh.WallTextureCoordinateModeV WallV;

			public EffectMesh.AnchorTextureCoordinateMode AnchorUV;
		}

		public class EffectMeshObject
		{
			public GameObject effectMeshGO;

			public Mesh mesh;

			public Collider collider;
		}
	}
}
