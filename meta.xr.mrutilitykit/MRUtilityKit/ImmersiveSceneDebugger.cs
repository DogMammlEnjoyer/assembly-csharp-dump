using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Meta.XR.Util;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
	[Feature(Feature.Scene)]
	public class ImmersiveSceneDebugger : MonoBehaviour
	{
		private bool _roomHasChanged
		{
			get
			{
				if (this._currentRoom == MRUK.Instance.GetCurrentRoom())
				{
					return false;
				}
				this._currentRoom = MRUK.Instance.GetCurrentRoom();
				this._globalMeshAnchor = this._currentRoom.GlobalMeshAnchor;
				return true;
			}
		}

		private bool ShouldDisplayGlobalMesh
		{
			get
			{
				return this._shouldDisplayGlobalMesh;
			}
			set
			{
				this._shouldDisplayGlobalMesh = value;
				this.DisplayGlobalMesh(value);
			}
		}

		private bool ShouldToggleGlobalMeshCollision
		{
			get
			{
				return this._shouldToggleGlobalMeshCollision;
			}
			set
			{
				this._shouldToggleGlobalMeshCollision = value;
				this.ToggleGlobalMeshCollisions(value);
			}
		}

		private bool ShouldDisplayNavMesh
		{
			get
			{
				return this._shouldDisplayNavMesh;
			}
			set
			{
				this._shouldDisplayNavMesh = value;
				this.DisplayNavMesh(value);
			}
		}

		internal static ImmersiveSceneDebugger Instance { get; private set; }

		private void Awake()
		{
			if (ImmersiveSceneDebugger.Instance != null && ImmersiveSceneDebugger.Instance != this)
			{
				Object.Destroy(this);
			}
			else
			{
				ImmersiveSceneDebugger.Instance = this;
			}
			this._cameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
			this._isPositionInRoom = this.IsPositionInRoomDebugger();
			this._showDebugAnchorsDebugAction = this.ShowDebugAnchorsDebugger();
			this._raycastDebugger = this.RayCastDebugger();
			this._getBestPoseFromRaycastDebugger = this.GetBestPoseFromRaycastDebugger();
			this._getKeyWallDebugger = this.GetKeyWallDebugger();
			this._getLaunchSpaceSetupDebugger = this.GetLaunchSpaceSetupDebugger();
			this._getLargestSurfaceDebugger = this.GetLargestSurfaceDebugger();
			this._getClosestSeatPoseDebugger = this.GetClosestSeatPoseDebugger();
			this._getClosestSurfacePositionDebugger = this.GetClosestSurfacePositionDebugger();
		}

		private void Start()
		{
			MRUK instance = MRUK.Instance;
			if (instance != null)
			{
				instance.RegisterSceneLoadedCallback(new UnityAction(this.OnSceneLoaded));
			}
			OVRTelemetry.Start(651897568, 0, -1L).Send();
			MRUK instance2 = MRUK.Instance;
			this._currentRoom = ((instance2 != null) ? instance2.GetCurrentRoom() : null);
			this._sceneDetails = this.ShowRoomDetails();
			this._debugMaterial = new Material(this._debugShader)
			{
				color = Color.green
			};
			this._navMeshMaterial = new Material(this._debugShader)
			{
				color = Color.cyan
			};
			this.SetupCheckerMeshMaterial(this._debugShader);
			this.CreateDebugPrimitives();
		}

		private void Update()
		{
			if (this._currentDebugAction != null)
			{
				this._currentDebugAction.GetValueOrDefault().Execute();
			}
			if (!this._currentDebugMessage.Equals(this._debugMessage))
			{
				this._currentDebugMessage = this._debugMessage;
				Debug.Log(this._currentDebugMessage);
			}
			if (this.ShowDebugAnchors != this._previousShowDebugAnchors)
			{
				if (this.ShowDebugAnchors)
				{
					using (List<MRUKRoom>.Enumerator enumerator = MRUK.Instance.Rooms.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MRUKRoom mrukroom = enumerator.Current;
							foreach (MRUKAnchor anchor in mrukroom.Anchors)
							{
								GameObject item = this.GenerateDebugAnchor(anchor);
								this._debugAnchors.Add(item);
							}
						}
						goto IL_110;
					}
				}
				foreach (GameObject gameObject in this._debugAnchors)
				{
					Object.Destroy(gameObject.gameObject);
				}
				IL_110:
				this._previousShowDebugAnchors = this.ShowDebugAnchors;
			}
		}

		private void OnDisable()
		{
			this._currentDebugAction = null;
		}

		public void OnDestroy()
		{
			MRUK instance = MRUK.Instance;
			if (instance == null)
			{
				return;
			}
			instance.SceneLoadedEvent.RemoveListener(new UnityAction(this.OnSceneLoaded));
		}

		private void OnSceneLoaded()
		{
			this.CreateDebugPrimitives();
			if (MRUK.Instance && MRUK.Instance.GetCurrentRoom() && !this._globalMeshAnchor)
			{
				this._globalMeshAnchor = MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor;
			}
		}

		private void IsPositionInRoom()
		{
			this.SetDebugAction(this._isPositionInRoom);
		}

		private void DisplayDebugAnchors()
		{
			this.SetDebugAction(this._showDebugAnchorsDebugAction);
		}

		private void Raycast()
		{
			this.SetDebugAction(this._raycastDebugger);
		}

		private void GetBestPoseFromRayCast()
		{
			this.SetDebugAction(this._getBestPoseFromRaycastDebugger);
		}

		private void GetKeyWall()
		{
			this.SetDebugAction(this._getKeyWallDebugger);
		}

		private void GetLaunchSpaceSetup()
		{
			this.SetDebugAction(this._getLaunchSpaceSetupDebugger);
		}

		private void GetLargestSurface()
		{
			this.SetDebugAction(this._getLargestSurfaceDebugger);
		}

		private void GetClosestSeatPose()
		{
			this.SetDebugAction(this._getClosestSeatPoseDebugger);
		}

		private void GetClosestSurfacePosition()
		{
			this.SetDebugAction(this._getClosestSurfacePositionDebugger);
		}

		private void SetDebugAction(ImmersiveSceneDebugger.DebugAction newDebugAction)
		{
			if (this._currentDebugAction != null)
			{
				this._currentDebugAction.GetValueOrDefault().Cleanup();
			}
			ImmersiveSceneDebugger.DebugAction? currentDebugAction = this._currentDebugAction;
			if (currentDebugAction != null && (currentDebugAction == null || currentDebugAction.GetValueOrDefault() == newDebugAction))
			{
				this._currentDebugAction = null;
				return;
			}
			this._currentDebugAction = new ImmersiveSceneDebugger.DebugAction?(newDebugAction);
			if (this._currentDebugAction == null)
			{
				return;
			}
			this._currentDebugAction.GetValueOrDefault().Setup();
		}

		private Ray GetControllerRay()
		{
			Vector3 position;
			Vector3 forward;
			if (OVRInput.activeControllerType == OVRInput.Controller.Touch || OVRInput.activeControllerType == OVRInput.Controller.RTouch)
			{
				position = this._cameraRig.rightHandOnControllerAnchor.position;
				forward = this._cameraRig.rightHandOnControllerAnchor.forward;
			}
			else if (OVRInput.activeControllerType == OVRInput.Controller.LTouch)
			{
				position = this._cameraRig.leftHandOnControllerAnchor.position;
				forward = this._cameraRig.leftHandOnControllerAnchor.forward;
			}
			else
			{
				OVRHand componentInChildren = this._cameraRig.rightHandAnchor.GetComponentInChildren<OVRHand>();
				if (componentInChildren != null)
				{
					position = componentInChildren.PointerPose.position;
					forward = componentInChildren.PointerPose.forward;
				}
				else
				{
					position = this._cameraRig.centerEyeAnchor.position;
					forward = this._cameraRig.centerEyeAnchor.forward;
				}
			}
			return new Ray(position, forward);
		}

		private ImmersiveSceneDebugger.DebugAction GetKeyWallDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				this._debugCube.SetActive(true);
				Vector2 zero = Vector2.zero;
				MRUK instance = MRUK.Instance;
				MRUKAnchor mrukanchor;
				if (instance == null)
				{
					mrukanchor = null;
				}
				else
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					mrukanchor = ((currentRoom != null) ? currentRoom.GetKeyWall(out zero, 0.1f) : null);
				}
				MRUKAnchor mrukanchor2 = mrukanchor;
				if (mrukanchor2 != null && this._debugCube != null)
				{
					this._debugCube.transform.localScale = new Vector3(zero.x, zero.y, 0.05f);
					this._debugCube.transform.position = mrukanchor2.transform.position;
					this._debugCube.transform.rotation = mrukanchor2.transform.rotation;
				}
				this._debugMessage = string.Format("[{0}] Size: {1}", "GetKeyWallDebugger", zero);
			}, delegate()
			{
			}, delegate()
			{
				this._debugCube.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction GetLaunchSpaceSetupDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				ImmersiveSceneDebugger.<>c.<<GetLaunchSpaceSetupDebugger>b__80_0>d <<GetLaunchSpaceSetupDebugger>b__80_0>d;
				<<GetLaunchSpaceSetupDebugger>b__80_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<GetLaunchSpaceSetupDebugger>b__80_0>d.<>1__state = -1;
				<<GetLaunchSpaceSetupDebugger>b__80_0>d.<>t__builder.Start<ImmersiveSceneDebugger.<>c.<<GetLaunchSpaceSetupDebugger>b__80_0>d>(ref <<GetLaunchSpaceSetupDebugger>b__80_0>d);
			}, delegate()
			{
			}, delegate()
			{
			});
		}

		private ImmersiveSceneDebugger.DebugAction GetLargestSurfaceDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				this._debugCube.SetActive(true);
			}, delegate()
			{
				MRUK instance = MRUK.Instance;
				MRUKAnchor mrukanchor;
				if (instance == null)
				{
					mrukanchor = null;
				}
				else
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					mrukanchor = ((currentRoom != null) ? currentRoom.FindLargestSurface(this._largestSurfaceFilter) : null);
				}
				MRUKAnchor mrukanchor2 = mrukanchor;
				if (mrukanchor2 != null)
				{
					if (this._debugCube != null)
					{
						Vector3 a = (mrukanchor2.PlaneRect != null) ? new Vector3(mrukanchor2.PlaneRect.Value.width, mrukanchor2.PlaneRect.Value.height, 0.01f) : mrukanchor2.VolumeBounds.Value.size;
						this._debugCube.transform.localScale = a + new Vector3(0.01f, 0.01f, 0.01f);
						this._debugCube.transform.position = ((mrukanchor2.PlaneRect != null) ? mrukanchor2.transform.position : mrukanchor2.transform.TransformPoint(mrukanchor2.VolumeBounds.Value.center));
						this._debugCube.transform.rotation = mrukanchor2.transform.rotation;
					}
					this._debugMessage = string.Format("[{0}] Anchor: {1} Type: {2}", "GetLargestSurface", mrukanchor2.name, mrukanchor2.Label);
					return;
				}
				this._debugMessage = "[GetLargestSurface] Cannot get surface area for this label in this scene.";
				this._debugCube.SetActive(false);
			}, delegate()
			{
				this._debugCube.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction GetClosestSeatPoseDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				MRUKAnchor mrukanchor = null;
				Pose pose = default(Pose);
				Ray controllerRay = this.GetControllerRay();
				MRUK instance = MRUK.Instance;
				if (instance != null)
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					if (currentRoom != null)
					{
						currentRoom.TryGetClosestSeatPose(controllerRay, out pose, out mrukanchor);
					}
				}
				if (mrukanchor)
				{
					this._debugCube.SetActive(true);
					if (this._debugCube != null)
					{
						this._debugCube.transform.localRotation = mrukanchor.transform.localRotation;
						this._debugCube.transform.position = pose.position;
						this._debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					}
					this._debugMessage = string.Format("[{0}] Seat: {1} Position: {2}", "GetClosestSeatPoseDebugger", mrukanchor.name, pose.position) + "Distance: " + Vector3.Distance(pose.position, controllerRay.origin).ToString("0.##");
					return;
				}
				this._debugMessage = "[GetClosestSeatPoseDebugger]  No seat found in the scene.";
			}, delegate()
			{
			}, delegate()
			{
				this._debugCube.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction GetClosestSurfacePositionDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				this._debugNormal.SetActive(true);
			}, delegate()
			{
				Vector3 origin = this.GetControllerRay().origin;
				Vector3 zero = Vector3.zero;
				Vector3 up = Vector3.up;
				MRUKAnchor mrukanchor = null;
				MRUK instance = MRUK.Instance;
				if (instance != null)
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					if (currentRoom != null)
					{
						currentRoom.TryGetClosestSurfacePosition(origin, out zero, out mrukanchor, out up, default(LabelFilter));
					}
				}
				this.ShowHitNormal(zero, up);
				if (mrukanchor != null)
				{
					this._debugMessage = string.Format("[{0}] Anchor: {1} Surface Position: {2} Distance: {3}", new object[]
					{
						"GetClosestSurfacePosition",
						mrukanchor.name,
						zero,
						Vector3.Distance(origin, zero).ToString("0.##")
					});
				}
			}, delegate()
			{
				this._debugNormal.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction GetBestPoseFromRaycastDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				this._debugCube.SetActive(true);
			}, delegate()
			{
				Ray controllerRay = this.GetControllerRay();
				MRUKAnchor mrukanchor = null;
				MRUK instance = MRUK.Instance;
				Pose? pose;
				if (instance == null)
				{
					pose = null;
				}
				else
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					pose = ((currentRoom != null) ? new Pose?(currentRoom.GetBestPoseFromRaycast(controllerRay, float.PositiveInfinity, default(LabelFilter), out mrukanchor, this._positioningMethod)) : null);
				}
				Pose? pose2 = pose;
				if (pose2 != null && mrukanchor && this._debugCube)
				{
					this._debugCube.transform.position = pose2.Value.position;
					this._debugCube.transform.rotation = pose2.Value.rotation;
					this._debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					this._debugMessage = string.Format("[{0}] Anchor: {1} Pose Position: {2} Pose Rotation: {3}", new object[]
					{
						"GetBestPoseFromRayCast",
						mrukanchor.name,
						pose2.Value.position,
						pose2.Value.rotation
					});
				}
			}, delegate()
			{
				this._debugCube.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction RayCastDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(delegate()
			{
				this._debugNormal.SetActive(true);
			}, delegate()
			{
				Ray controllerRay = this.GetControllerRay();
				RaycastHit raycastHit = default(RaycastHit);
				MRUKAnchor mrukanchor = null;
				MRUK instance = MRUK.Instance;
				if (instance != null)
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					if (currentRoom != null)
					{
						currentRoom.Raycast(controllerRay, float.PositiveInfinity, out raycastHit, out mrukanchor);
					}
				}
				this.ShowHitNormal(raycastHit.point, raycastHit.normal);
				if (mrukanchor != null)
				{
					this._debugMessage = string.Format("[{0}] Anchor: {1} Hit point: {2} Hit normal: {3}", new object[]
					{
						"Raycast",
						mrukanchor.name,
						raycastHit.point,
						raycastHit.normal
					});
				}
			}, delegate()
			{
				this._debugNormal.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction IsPositionInRoomDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(null, delegate()
			{
				Ray controllerRay = this.GetControllerRay();
				if (this._debugSphere != null)
				{
					this._debugSphere.SetActive(true);
					MRUK instance = MRUK.Instance;
					bool? flag;
					if (instance == null)
					{
						flag = null;
					}
					else
					{
						MRUKRoom currentRoom = instance.GetCurrentRoom();
						flag = ((currentRoom != null) ? new bool?(currentRoom.IsPositionInRoom(this._debugSphere.transform.position, true)) : null);
					}
					bool? flag2 = flag;
					this._debugSphere.transform.position = controllerRay.GetPoint(0.2f);
					this._debugSphere.GetComponent<Renderer>().material.color = ((flag2 != null && flag2.Value) ? Color.green : Color.red);
					this._debugMessage = string.Format("[{0}] Position: {1} ", "IsPositionInRoom", this._debugSphere.transform.position) + string.Format("Is inside the Room: {0}", flag2);
				}
			}, delegate()
			{
				this._debugSphere.SetActive(false);
			});
		}

		private ImmersiveSceneDebugger.DebugAction ShowDebugAnchorsDebugger()
		{
			return new ImmersiveSceneDebugger.DebugAction(null, delegate()
			{
				Ray controllerRay = this.GetControllerRay();
				RaycastHit raycastHit = default(RaycastHit);
				MRUKAnchor mrukanchor = null;
				MRUK instance = MRUK.Instance;
				if (instance != null)
				{
					MRUKRoom currentRoom = instance.GetCurrentRoom();
					if (currentRoom != null)
					{
						currentRoom.Raycast(controllerRay, float.PositiveInfinity, out raycastHit, out mrukanchor);
					}
				}
				if (this._previousShownDebugAnchor != mrukanchor && mrukanchor != null)
				{
					Object.Destroy(this._debugAnchor);
					this._debugAnchor = this.GenerateDebugAnchor(mrukanchor);
					this._previousShownDebugAnchor = mrukanchor;
				}
				this.ShowHitNormal(raycastHit.point, raycastHit.normal);
				this._debugMessage = string.Format("[{0}] Hit point: {1} Hit normal: {2}", "ShowDebugAnchorsDebugger", raycastHit.point, raycastHit.normal);
			}, delegate()
			{
				Object.Destroy(this._debugAnchor);
				this._debugAnchor = null;
				if (this._debugNormal != null)
				{
					this._debugNormal.SetActive(false);
				}
			});
		}

		public void DisplayGlobalMesh(bool isOn)
		{
			if (!this._globalMeshAnchor)
			{
				Debug.Log("[DisplayGlobalMesh] No global mesh anchor found in the scene.");
				return;
			}
			if (!isOn)
			{
				if (this._globalMeshGO)
				{
					this._globalMeshGO.GetComponent<MeshRenderer>().enabled = false;
				}
				return;
			}
			if (this._roomHasChanged || !this._globalMeshGO)
			{
				if (this._globalMeshGO)
				{
					Object.DestroyImmediate(this._globalMeshGO);
				}
				this.InstantiateGlobalMesh(delegate(GameObject globalMeshSegmentGO, Mesh mesh)
				{
					globalMeshSegmentGO.AddComponent<MeshRenderer>().material = this.visualHelperMaterial;
				});
				return;
			}
			this._globalMeshGO.GetComponent<MeshRenderer>().enabled = true;
		}

		public void ToggleGlobalMeshCollisions(bool isOn)
		{
			if (!this._globalMeshAnchor)
			{
				Debug.Log("[ToggleGlobalMeshCollisions] No global mesh anchor found in the scene.");
				return;
			}
			if (!isOn)
			{
				if (this._globalMeshCollider)
				{
					this._globalMeshCollider.enabled = false;
				}
				return;
			}
			if (this._roomHasChanged || !this._globalMeshCollider)
			{
				if (this._globalMeshCollider)
				{
					Object.DestroyImmediate(this._globalMeshCollider);
				}
				GameObject gameObject = new GameObject("_globalMeshCollider");
				gameObject.transform.SetParent(this._globalMeshAnchor.transform, false);
				this._globalMeshCollider = gameObject.AddComponent<MeshCollider>();
				this._globalMeshCollider.sharedMesh = this._globalMeshAnchor.GlobalMesh;
				return;
			}
			this._globalMeshCollider.enabled = true;
		}

		private void InstantiateGlobalMesh(Action<GameObject, Mesh> onMeshSegmentInstantiated)
		{
			Mesh mesh = Utilities.AddBarycentricCoordinatesToMesh(this._globalMeshAnchor.Mesh);
			this._globalMeshGO = new GameObject("_globalMeshViz");
			this._globalMeshGO.transform.SetParent(MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor.transform, false);
			this._globalMeshGO.AddComponent<MeshFilter>().mesh = mesh;
			if (onMeshSegmentInstantiated != null)
			{
				onMeshSegmentInstantiated(this._globalMeshGO, mesh);
			}
		}

		public void ExportJSON()
		{
			string text = "";
			try
			{
				string contents = MRUK.Instance.SaveSceneToJsonString(this.exportGlobalMeshJSON, null);
				string path = "MRUK_Export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
				text = Path.Combine(Application.persistentDataPath, path);
				File.WriteAllText(text, contents);
			}
			catch (Exception ex)
			{
				Debug.LogError("Could not save Scene JSON to " + text + ". Exception: " + ex.Message);
				return;
			}
			Debug.Log("Saved Scene JSON to " + text);
		}

		public void DisplayNavMesh(bool isOn)
		{
			if (!isOn)
			{
				Object.DestroyImmediate(this._navMeshViz);
				return;
			}
			NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();
			if (navMeshTriangulation.areas.Length == 0 && this._navMeshTriangulation.Equals(navMeshTriangulation))
			{
				return;
			}
			MeshRenderer meshRenderer;
			MeshFilter meshFilter;
			if (!this._navMeshViz)
			{
				this._navMeshViz = new GameObject("_navMeshViz");
				meshRenderer = this._navMeshViz.AddComponent<MeshRenderer>();
				meshFilter = this._navMeshViz.AddComponent<MeshFilter>();
			}
			else
			{
				meshRenderer = this._navMeshViz.GetComponent<MeshRenderer>();
				meshFilter = this._navMeshViz.GetComponent<MeshFilter>();
				Object.DestroyImmediate(meshFilter.mesh);
				meshFilter.mesh = null;
			}
			Mesh mesh = new Mesh
			{
				indexFormat = ((navMeshTriangulation.indices.Length > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16)
			};
			mesh.SetVertices(navMeshTriangulation.vertices);
			mesh.SetIndices(navMeshTriangulation.indices, MeshTopology.Triangles, 0);
			meshRenderer.material = this._navMeshMaterial;
			meshFilter.mesh = mesh;
			this._navMeshTriangulation = navMeshTriangulation;
		}

		private string ShowRoomDetails()
		{
			string arg = "N/A";
			int num = 0;
			int num2 = 0;
			if (MRUK.Instance)
			{
				num2 = ((MRUK.Instance.Rooms != null) ? MRUK.Instance.Rooms.Count : 0);
				if (MRUK.Instance.GetCurrentRoom())
				{
					arg = ((MRUK.Instance.GetCurrentRoom() != null) ? MRUK.Instance.GetCurrentRoom().name : "N/A");
					num = ((MRUK.Instance.GetCurrentRoom().Anchors != null) ? MRUK.Instance.GetCurrentRoom().Anchors.Count : 0);
				}
			}
			return string.Format("Room Details: Number of rooms: {0}; Current room: {1}; Number room anchors:{2}", num2, arg, num);
		}

		private GameObject GenerateDebugAnchor(MRUKAnchor anchor)
		{
			Vector3 vector = anchor.transform.position;
			Quaternion rotation = anchor.transform.rotation;
			Vector3 localScale;
			if (anchor.VolumeBounds != null)
			{
				this.CreateDebugPrefabSource(anchor);
				Bounds value = anchor.VolumeBounds.Value;
				localScale = value.size;
				vector += rotation * value.center;
			}
			else
			{
				this.CreateDebugPrefabSource(anchor);
				localScale = Vector3.zero;
				if (anchor.PlaneRect != null)
				{
					Vector2 size = anchor.PlaneRect.Value.size;
					localScale = new Vector3(size.x, size.y, 1f);
				}
			}
			this._debugAnchor.transform.position = vector;
			this._debugAnchor.transform.rotation = rotation;
			this.ScaleChildren(this._debugAnchor.transform, localScale);
			this._debugAnchor.transform.parent = null;
			this._debugAnchor.SetActive(true);
			return this._debugAnchor;
		}

		private void ScaleChildren(Transform parent, Vector3 localScale)
		{
			foreach (object obj in parent)
			{
				((Transform)obj).localScale = localScale;
			}
		}

		private void CreateDebugPrefabSource(MRUKAnchor anchor)
		{
			string name = (anchor.VolumeBounds == null) ? "PlanePrefab" : "VolumePrefab";
			this._debugAnchor = new GameObject(name);
			GameObject gameObject = new GameObject("MeshParent");
			gameObject.transform.SetParent(this._debugAnchor.transform);
			gameObject.SetActive(false);
			GameObject gameObject2 = new GameObject("Pivot");
			gameObject2.transform.SetParent(this._debugAnchor.transform);
			if (anchor.VolumeBounds != null)
			{
				this.CreateGridPattern(gameObject2.transform, new Vector3(0f, 0f, 0.5f), Quaternion.identity);
				this.CreateGridPattern(gameObject2.transform, new Vector3(0f, 0f, -0.5f), Quaternion.Euler(180f, 0f, 0f));
				this.CreateGridPattern(gameObject2.transform, new Vector3(0f, 0.5f, 0f), Quaternion.Euler(-90f, 0f, 0f));
				this.CreateGridPattern(gameObject2.transform, new Vector3(0f, -0.5f, 0f), Quaternion.Euler(90f, 0f, 0f));
				this.CreateGridPattern(gameObject2.transform, new Vector3(-0.5f, 0f, 0f), Quaternion.Euler(0f, -90f, 90f));
				this.CreateGridPattern(gameObject2.transform, new Vector3(0.5f, 0f, 0f), Quaternion.Euler(0f, 90f, 90f));
				return;
			}
			this.CreateGridPattern(gameObject2.transform, Vector3.zero, Quaternion.identity);
		}

		private void CreateGridPattern(Transform parentTransform, Vector3 localOffset, Quaternion localRotation)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			gameObject.name = "Checker";
			gameObject.transform.SetParent(parentTransform, false);
			gameObject.transform.localPosition = localOffset;
			gameObject.transform.localRotation = localRotation;
			gameObject.transform.localScale = Vector3.one;
			Object.DestroyImmediate(gameObject.GetComponent<Collider>());
			if (this._debugCheckerMesh == null)
			{
				this._debugCheckerMesh = new Mesh();
				float num = 0.1f;
				float num2 = -0.5f;
				float num3 = -0.5f;
				int num4 = 50;
				int num5 = num4 * 4;
				int num6 = num4 * 6;
				Vector3[] array = new Vector3[num5];
				Vector2[] array2 = new Vector2[num5];
				Color32[] array3 = new Color32[num5];
				Vector3[] array4 = new Vector3[num5];
				Vector4[] array5 = new Vector4[num5];
				int[] array6 = new int[num6];
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				for (int i = 0; i < 10; i++)
				{
					bool flag = i % 2 == 0;
					for (int j = 0; j < 10; j++)
					{
						if (flag)
						{
							for (int k = 0; k < 4; k++)
							{
								Vector3 vector = new Vector3(num2 + (float)i * num, num3 + (float)j * num, 0.001f);
								switch (k)
								{
								case 1:
									vector += new Vector3(0f, num, 0f);
									break;
								case 2:
									vector += new Vector3(num, num, 0f);
									break;
								case 3:
									vector += new Vector3(num, 0f, 0f);
									break;
								}
								array[num7] = vector;
								array2[num7] = Vector2.zero;
								array3[num7] = Color.black;
								array4[num7] = Vector3.forward;
								array5[num7] = Vector3.right;
								num7++;
							}
							int num10 = num9 * 4;
							array6[num8++] = num10;
							array6[num8++] = num10 + 2;
							array6[num8++] = num10 + 1;
							array6[num8++] = num10;
							array6[num8++] = num10 + 3;
							array6[num8++] = num10 + 2;
							num9++;
						}
						flag = !flag;
					}
				}
				this._debugCheckerMesh.Clear();
				this._debugCheckerMesh.name = "CheckerMesh";
				this._debugCheckerMesh.vertices = array;
				this._debugCheckerMesh.uv = array2;
				this._debugCheckerMesh.colors32 = array3;
				this._debugCheckerMesh.triangles = array6;
				this._debugCheckerMesh.normals = array4;
				this._debugCheckerMesh.tangents = array5;
				this._debugCheckerMesh.RecalculateNormals();
				this._debugCheckerMesh.RecalculateTangents();
			}
			gameObject.GetComponent<MeshFilter>().mesh = this._debugCheckerMesh;
			gameObject.GetComponent<MeshRenderer>().material = this._checkerMeshMaterial;
		}

		private void SetupCheckerMeshMaterial(Shader debugShader)
		{
			this._checkerMeshMaterial = new Material(debugShader);
			this._checkerMeshMaterial.SetOverrideTag("RenderType", "Transparent");
			this._checkerMeshMaterial.SetInt(this._srcBlend, 5);
			this._checkerMeshMaterial.SetInt(this._dstBlend, 1);
			this._checkerMeshMaterial.SetInt(this._zWrite, 0);
			this._checkerMeshMaterial.SetInt(this._cull, 2);
			this._checkerMeshMaterial.DisableKeyword("_ALPHATEST_ON");
			this._checkerMeshMaterial.EnableKeyword("_ALPHABLEND_ON");
			this._checkerMeshMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			this._checkerMeshMaterial.renderQueue = 3000;
		}

		private void CreateDebugPrimitives()
		{
			this._debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this._debugCube.name = "SceneDebugger_Cube";
			Renderer component = this._debugCube.GetComponent<Renderer>();
			if (component)
			{
				component.material = this._debugMaterial;
				component.shadowCastingMode = ShadowCastingMode.Off;
				component.receiveShadows = false;
			}
			this._debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			this._debugCube.GetComponent<Collider>().enabled = false;
			this._debugCube.SetActive(false);
			this._debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			this._debugSphere.name = "SceneDebugger_Sphere";
			Renderer component2 = this._debugSphere.GetComponent<Renderer>();
			if (component2)
			{
				component2.material = this._debugMaterial;
				component2.shadowCastingMode = ShadowCastingMode.Off;
				component2.receiveShadows = false;
			}
			this._debugSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			this._debugSphere.GetComponent<Collider>().enabled = false;
			this._debugSphere.SetActive(false);
			this._debugNormal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			this._debugNormal.name = "SceneDebugger_Normal";
			Renderer component3 = this._debugNormal.GetComponent<Renderer>();
			if (component3)
			{
				component3.material = this._debugMaterial;
				component3.shadowCastingMode = ShadowCastingMode.Off;
				component3.receiveShadows = false;
			}
			this._debugNormal.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f);
			this._debugNormal.GetComponent<Collider>().enabled = false;
			this._debugNormal.SetActive(false);
		}

		private void ShowHitNormal(Vector3 position, Vector3 normal)
		{
			if (this._debugNormal == null)
			{
				return;
			}
			if (position != Vector3.zero && normal != Vector3.zero)
			{
				this._debugNormal.SetActive(true);
				this._debugNormal.transform.rotation = Quaternion.FromToRotation(-Vector3.up, normal);
				this._debugNormal.transform.position = position + -this._debugNormal.transform.up * this._debugNormal.transform.localScale.y;
				return;
			}
			this._debugNormal.SetActive(false);
		}

		[Tooltip("Visualize anchors")]
		public bool ShowDebugAnchors;

		[SerializeField]
		private Material visualHelperMaterial;

		[SerializeField]
		private Shader _debugShader;

		private readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");

		private readonly int _dstBlend = Shader.PropertyToID("_DstBlend");

		private readonly int _zWrite = Shader.PropertyToID("_ZWrite");

		private readonly int _cull = Shader.PropertyToID("_Cull");

		private readonly int _color = Shader.PropertyToID("_Color");

		private readonly List<GameObject> _debugAnchors = new List<GameObject>();

		private GameObject _globalMeshGO;

		private OVRCameraRig _cameraRig;

		private MRUKRoom _currentRoom;

		private GameObject _debugCube;

		private GameObject _debugSphere;

		private GameObject _debugNormal;

		private GameObject _navMeshViz;

		private GameObject _debugAnchor;

		private bool _previousShowDebugAnchors;

		private Mesh _debugCheckerMesh;

		private MRUKAnchor _previousShownDebugAnchor;

		private MRUKAnchor _globalMeshAnchor;

		private NavMeshTriangulation _navMeshTriangulation;

		private SpaceMapGPU _spaceMapGPU;

		private MeshCollider _globalMeshCollider;

		private Material _navMeshMaterial;

		private string _debugMessage = "";

		private string _currentDebugMessage = "";

		private string _sceneDetails = "";

		private Material _debugMaterial;

		private Material _checkerMeshMaterial;

		private ImmersiveSceneDebugger.DebugAction _isPositionInRoom;

		private ImmersiveSceneDebugger.DebugAction _showDebugAnchorsDebugAction;

		private ImmersiveSceneDebugger.DebugAction _raycastDebugger;

		private MRUK.PositioningMethod _positioningMethod = MRUK.PositioningMethod.CENTER;

		private ImmersiveSceneDebugger.DebugAction _getBestPoseFromRaycastDebugger;

		private ImmersiveSceneDebugger.DebugAction _getKeyWallDebugger;

		private ImmersiveSceneDebugger.DebugAction _getLaunchSpaceSetupDebugger;

		private MRUKAnchor.SceneLabels _largestSurfaceFilter = MRUKAnchor.SceneLabels.TABLE;

		private ImmersiveSceneDebugger.DebugAction _getLargestSurfaceDebugger;

		private ImmersiveSceneDebugger.DebugAction _getClosestSeatPoseDebugger;

		private ImmersiveSceneDebugger.DebugAction _getClosestSurfacePositionDebugger;

		private bool exportGlobalMeshJSON = true;

		private ImmersiveSceneDebugger.DebugAction? _currentDebugAction;

		private bool _shouldDisplayGlobalMesh;

		private bool _shouldToggleGlobalMeshCollision;

		private bool _shouldDisplayNavMesh;

		private readonly struct DebugAction
		{
			public DebugAction(Action setup, Action execute, Action cleanup)
			{
				this._setup = setup;
				this._execute = execute;
				this._cleanup = cleanup;
			}

			public void Setup()
			{
				Action setup = this._setup;
				if (setup == null)
				{
					return;
				}
				setup();
			}

			public void Cleanup()
			{
				Action cleanup = this._cleanup;
				if (cleanup == null)
				{
					return;
				}
				cleanup();
			}

			public void Execute()
			{
				Action execute = this._execute;
				if (execute == null)
				{
					return;
				}
				execute();
			}

			public bool Equals(ImmersiveSceneDebugger.DebugAction other)
			{
				return this._setup == other._setup && this._execute == other._execute && this._cleanup == other._cleanup;
			}

			public override bool Equals(object obj)
			{
				if (obj is ImmersiveSceneDebugger.DebugAction)
				{
					ImmersiveSceneDebugger.DebugAction other = (ImmersiveSceneDebugger.DebugAction)obj;
					return this.Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				int num = 17 * 23;
				Action setup = this._setup;
				int num2 = (num + ((setup != null) ? setup.GetHashCode() : 0)) * 23;
				Action execute = this._execute;
				int num3 = (num2 + ((execute != null) ? execute.GetHashCode() : 0)) * 23;
				Action cleanup = this._cleanup;
				return num3 + ((cleanup != null) ? cleanup.GetHashCode() : 0);
			}

			public static bool operator ==(ImmersiveSceneDebugger.DebugAction left, ImmersiveSceneDebugger.DebugAction right)
			{
				return left.Equals(right);
			}

			public static bool operator !=(ImmersiveSceneDebugger.DebugAction left, ImmersiveSceneDebugger.DebugAction right)
			{
				return !left.Equals(right);
			}

			private readonly Action _setup;

			private readonly Action _cleanup;

			private readonly Action _execute;
		}
	}
}
