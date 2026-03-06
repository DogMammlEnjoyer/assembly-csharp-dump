using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Meta.XR.Util;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Meta.XR.MRUtilityKit
{
	[Obsolete("This component is deprecated.Please use the Immersive Debugger fromMeta > Tools > Immersive Debugger")]
	[Feature(Feature.Scene)]
	public class SceneDebugger : MonoBehaviour
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

		private void Awake()
		{
			this._cameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
			this._canvas = base.GetComponentInChildren<Canvas>();
			if (this.SetupInteractions)
			{
				this.SetupInteractionDependencies();
			}
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
			this._spaceMapGPU = this.GetSpaceMapGPU();
			if (this.MoveCanvasInFrontOfCamera)
			{
				base.StartCoroutine(this.SnapCanvasInFrontOfCamera());
			}
			if (this._spaceMapGPU == null)
			{
				this.Menus[0].transform.FindChildRecursive("SpaceMapGPU").gameObject.SetActive(false);
			}
			Shader shader = Shader.Find("Meta/Lit");
			this._debugMaterial = new Material(shader)
			{
				color = Color.green
			};
			this._navMeshMaterial = new Material(shader)
			{
				color = Color.cyan
			};
			this.SetupCheckerMeshMaterial(shader);
			this.CreateDebugPrimitives();
		}

		private void Update()
		{
			Action debugAction = this._debugAction;
			if (debugAction != null)
			{
				debugAction();
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
						goto IL_E3;
					}
				}
				foreach (GameObject gameObject in this._debugAnchors)
				{
					Object.Destroy(gameObject.gameObject);
				}
				this._previousShowDebugAnchors = this.ShowDebugAnchors;
			}
			IL_E3:
			if (OVRInput.GetDown(OVRInput.RawButton.Start, OVRInput.Controller.Active))
			{
				this.ToggleMenu(!this._canvas.gameObject.activeInHierarchy);
			}
			this.Billboard();
		}

		private void OnDisable()
		{
			this._debugAction = null;
		}

		private void OnSceneLoaded()
		{
			if (MRUK.Instance && MRUK.Instance.GetCurrentRoom() && !this._globalMeshAnchor)
			{
				this._globalMeshAnchor = MRUK.Instance.GetCurrentRoom().GlobalMeshAnchor;
			}
		}

		private void SetupInteractionDependencies()
		{
			if (!this._cameraRig)
			{
				return;
			}
			this.GazePointer.rayTransform = this._cameraRig.centerEyeAnchor;
			this.InputModule.rayTransform = this._cameraRig.rightControllerAnchor;
			this.Raycaster.pointer = this._cameraRig.rightControllerAnchor.gameObject;
			if (this._cameraRig.GetComponentsInChildren<OVRRayHelper>(false).Length != 0)
			{
				return;
			}
			OVRControllerHelper componentInChildren = this._cameraRig.rightControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
			if (componentInChildren)
			{
				componentInChildren.RayHelper = Object.Instantiate<OVRRayHelper>(this.RayHelper, Vector3.zero, Quaternion.identity, componentInChildren.transform);
				componentInChildren.RayHelper.gameObject.SetActive(true);
			}
			OVRControllerHelper componentInChildren2 = this._cameraRig.leftControllerAnchor.GetComponentInChildren<OVRControllerHelper>();
			if (componentInChildren2)
			{
				componentInChildren2.RayHelper = Object.Instantiate<OVRRayHelper>(this.RayHelper, Vector3.zero, Quaternion.identity, componentInChildren2.transform);
				componentInChildren2.RayHelper.gameObject.SetActive(true);
			}
			foreach (OVRHand ovrhand in this._cameraRig.GetComponentsInChildren<OVRHand>())
			{
				ovrhand.RayHelper = Object.Instantiate<OVRRayHelper>(this.RayHelper, Vector3.zero, Quaternion.identity, this._cameraRig.trackingSpace);
				ovrhand.RayHelper.gameObject.SetActive(true);
			}
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

		public void ShowRoomDetailsDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = (Action)Delegate.Combine(this._debugAction, new Action(this.ShowRoomDetails));
				}
				else
				{
					this._debugAction = (Action)Delegate.Remove(this._debugAction, new Action(this.ShowRoomDetails));
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"ShowRoomDetailsDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void GetKeyWallDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
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
					this.SetLogsText("\n[{0}]\nSize: {1}", new object[]
					{
						"GetKeyWallDebugger",
						zero
					});
				}
				if (this._debugCube != null)
				{
					this._debugCube.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"GetKeyWallDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void GetLargestSurfaceDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					MRUKAnchor.SceneLabels sceneLabels = MRUKAnchor.SceneLabels.TABLE;
					if (this.surfaceTypeDropdown)
					{
						sceneLabels = Utilities.StringLabelToEnum(this.surfaceTypeDropdown.options[this.surfaceTypeDropdown.value].text.ToUpper());
					}
					MRUK instance = MRUK.Instance;
					MRUKAnchor mrukanchor;
					if (instance == null)
					{
						mrukanchor = null;
					}
					else
					{
						MRUKRoom currentRoom = instance.GetCurrentRoom();
						mrukanchor = ((currentRoom != null) ? currentRoom.FindLargestSurface(sceneLabels) : null);
					}
					MRUKAnchor mrukanchor2 = mrukanchor;
					if (!(mrukanchor2 != null))
					{
						this.SetLogsText("\n[{0}]\n No surface of type {1} found.", new object[]
						{
							"GetLargestSurfaceDebugger",
							sceneLabels
						});
						this._debugCube.SetActive(false);
						return;
					}
					if (this._debugCube != null)
					{
						Vector3 a = (mrukanchor2.PlaneRect != null) ? new Vector3(mrukanchor2.PlaneRect.Value.width, mrukanchor2.PlaneRect.Value.height, 0.01f) : mrukanchor2.VolumeBounds.Value.size;
						this._debugCube.transform.localScale = a + new Vector3(0.01f, 0.01f, 0.01f);
						this._debugCube.transform.position = ((mrukanchor2.PlaneRect != null) ? mrukanchor2.transform.position : mrukanchor2.transform.TransformPoint(mrukanchor2.VolumeBounds.Value.center));
						this._debugCube.transform.rotation = mrukanchor2.transform.rotation;
					}
				}
				else
				{
					this._debugAction = null;
				}
				if (this._debugCube != null)
				{
					this._debugCube.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"GetLargestSurfaceDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void GetClosestSeatPoseDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
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
							if (this._debugCube != null)
							{
								this._debugCube.transform.localRotation = mrukanchor.transform.localRotation;
								this._debugCube.transform.position = pose.position;
								this._debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
							}
							this.SetLogsText("\n[{0}]\nSeat: {1}\nPosition: {2}\nDistance: {3}", new object[]
							{
								"GetClosestSeatPoseDebugger",
								mrukanchor.name,
								pose.position,
								Vector3.Distance(pose.position, controllerRay.origin).ToString("0.##")
							});
							return;
						}
						this.SetLogsText("\n[{0}]\n No seat found in the scene.", new object[]
						{
							"GetClosestSeatPoseDebugger"
						});
					};
				}
				else
				{
					this._debugAction = null;
				}
				if (this._debugCube != null)
				{
					this._debugCube.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"GetClosestSeatPoseDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void GetClosestSurfacePositionDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
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
							this.SetLogsText("\n[{0}]\nAnchor: {1}\nSurface Position: {2}\nDistance: {3}", new object[]
							{
								"GetClosestSurfacePositionDebugger",
								mrukanchor.name,
								zero,
								Vector3.Distance(origin, zero).ToString("0.##")
							});
						}
					};
				}
				else
				{
					this._debugAction = null;
				}
				if (this._debugNormal != null)
				{
					this._debugNormal.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"GetClosestSurfacePositionDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void GetBestPoseFromRaycastDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
					{
						Ray controllerRay = this.GetControllerRay();
						MRUKAnchor mrukanchor = null;
						MRUK.PositioningMethod positioningMethod = MRUK.PositioningMethod.DEFAULT;
						if (this.positioningMethodDropdown)
						{
							positioningMethod = (MRUK.PositioningMethod)this.positioningMethodDropdown.value;
						}
						MRUK instance = MRUK.Instance;
						Pose? pose;
						if (instance == null)
						{
							pose = null;
						}
						else
						{
							MRUKRoom currentRoom = instance.GetCurrentRoom();
							pose = ((currentRoom != null) ? new Pose?(currentRoom.GetBestPoseFromRaycast(controllerRay, float.PositiveInfinity, default(LabelFilter), out mrukanchor, positioningMethod)) : null);
						}
						Pose? pose2 = pose;
						if (pose2 != null && mrukanchor && this._debugCube)
						{
							this._debugCube.transform.position = pose2.Value.position;
							this._debugCube.transform.rotation = pose2.Value.rotation;
							this._debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
							this.SetLogsText("\n[{0}]\nAnchor: {1}\nPose Position: {2}\nPose Rotation: {3}", new object[]
							{
								"GetBestPoseFromRaycastDebugger",
								mrukanchor.name,
								pose2.Value.position,
								pose2.Value.rotation
							});
						}
					};
				}
				else
				{
					this._debugAction = null;
				}
				if (this._debugCube != null)
				{
					this._debugCube.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"GetBestPoseFromRaycastDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void RayCastDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
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
							this.SetLogsText("\n[{0}]\nAnchor: {1}\nHit point: {2}\nHit normal: {3}\n", new object[]
							{
								"RayCastDebugger",
								mrukanchor.name,
								raycastHit.point,
								raycastHit.normal
							});
						}
					};
				}
				else
				{
					this._debugAction = null;
				}
				if (this._debugNormal != null)
				{
					this._debugNormal.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"RayCastDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void IsPositionInRoomDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
					{
						Ray controllerRay = this.GetControllerRay();
						if (this._debugSphere != null)
						{
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
							this.SetLogsText("\n[{0}]\nPosition: {1}\nIs inside the Room: {2}\n", new object[]
							{
								"IsPositionInRoomDebugger",
								this._debugSphere.transform.position,
								flag2
							});
						}
					};
				}
				if (this._debugSphere != null)
				{
					this._debugSphere.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"IsPositionInRoomDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void ShowDebugAnchorsDebugger(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
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
						this.SetLogsText("\n[{0}]\nHit point: {1}\nHit normal: {2}\n", new object[]
						{
							"ShowDebugAnchorsDebugger",
							raycastHit.point,
							raycastHit.normal
						});
					};
				}
				else
				{
					this._debugAction = null;
					Object.Destroy(this._debugAnchor);
					this._debugAnchor = null;
				}
				if (this._debugNormal != null)
				{
					this._debugNormal.SetActive(isOn);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"ShowDebugAnchorsDebugger",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void DisplayGlobalMesh(bool isOn)
		{
			try
			{
				if (!this._globalMeshAnchor)
				{
					this.SetLogsText("\n[{0}]\nNo global mesh anchor found in the scene.\n", new object[]
					{
						"DisplayGlobalMesh"
					});
				}
				else if (isOn)
				{
					if (this._roomHasChanged || !this._globalMeshGO)
					{
						if (this._globalMeshGO)
						{
							Object.DestroyImmediate(this._globalMeshGO);
						}
						this.InstantiateGlobalMesh(delegate(GameObject globalMeshSegmentGO, Mesh _)
						{
							globalMeshSegmentGO.AddComponent<MeshRenderer>().material = this.visualHelperMaterial;
						});
					}
					else
					{
						this._globalMeshGO.GetComponent<MeshRenderer>().enabled = true;
					}
				}
				else if (this._globalMeshGO)
				{
					this._globalMeshGO.GetComponent<MeshRenderer>().enabled = false;
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"DisplayGlobalMesh",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public void ToggleGlobalMeshCollisions(bool isOn)
		{
			try
			{
				if (!this._globalMeshAnchor)
				{
					this.SetLogsText("\n[{0}]\nNo global mesh anchor found in the scene.\n", new object[]
					{
						"ToggleGlobalMeshCollisions"
					});
				}
				else if (isOn)
				{
					if (this._roomHasChanged || !this._globalMeshCollider)
					{
						GameObject gameObject = new GameObject("_globalMeshCollider");
						gameObject.transform.SetParent(this._globalMeshAnchor.transform, false);
						this._globalMeshCollider = gameObject.AddComponent<MeshCollider>();
						this._globalMeshCollider.sharedMesh = this._globalMeshAnchor.Mesh;
					}
					else
					{
						this._globalMeshCollider.enabled = true;
					}
				}
				else
				{
					this._globalMeshCollider.enabled = false;
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"ToggleGlobalMeshCollisions",
					ex.Message,
					ex.StackTrace
				});
			}
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

		public void ExportJSON(bool isOn)
		{
			try
			{
				if (isOn)
				{
					bool includeGlobalMesh = true;
					if (this.exportGlobalMeshJSONDropdown)
					{
						includeGlobalMesh = (this.exportGlobalMeshJSONDropdown.options[this.exportGlobalMeshJSONDropdown.value].text.ToLower() == "true");
					}
					string contents = MRUK.Instance.SaveSceneToJsonString(includeGlobalMesh, null);
					string path = "MRUK_Export_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
					string text = Path.Combine(Application.persistentDataPath, path);
					File.WriteAllText(text, contents);
					Debug.Log("Saved Scene JSON to " + text);
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"ExportJSON",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public static void DebugDestructibleMeshComponent(DestructibleMeshComponent destructibleMeshComponent)
		{
			if (destructibleMeshComponent == null)
			{
				throw new Exception("Can not debug a null DestructibleMeshComponent.");
			}
			destructibleMeshComponent.DebugDestructibleMeshComponent();
		}

		public void DisplaySpaceMap(bool isOn)
		{
		}

		public void DisplayNavMesh(bool isOn)
		{
			try
			{
				if (isOn)
				{
					this._debugAction = delegate()
					{
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
					};
				}
				else
				{
					Object.DestroyImmediate(this._navMeshViz);
					this._debugAction = null;
				}
			}
			catch (Exception ex)
			{
				this.SetLogsText("\n[{0}]\n {1}\n{2}", new object[]
				{
					"DisplayNavMesh",
					ex.Message,
					ex.StackTrace
				});
			}
		}

		private SpaceMapGPU GetSpaceMapGPU()
		{
			SpaceMapGPU[] array = Object.FindObjectsByType<SpaceMapGPU>(FindObjectsSortMode.None);
			if (array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		private void ShowRoomDetails()
		{
			MRUK instance = MRUK.Instance;
			string text;
			if (instance == null)
			{
				text = null;
			}
			else
			{
				MRUKRoom currentRoom = instance.GetCurrentRoom();
				text = ((currentRoom != null) ? currentRoom.name : null);
			}
			string arg = text ?? "N/A";
			MRUK instance2 = MRUK.Instance;
			int num = (instance2 != null) ? instance2.Rooms.Count : 0;
			this.RoomDetails.text = string.Format("\n[{0}]\nNumber of rooms: {1}\nCurrent room: {2}", "ShowRoomDetailsDebugger", num, arg);
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

		private void CreateDebugPrimitives()
		{
			this._debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this._debugCube.name = "SceneDebugger_Cube";
			this._debugCube.GetComponent<Renderer>().material = this._debugMaterial;
			this._debugCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			this._debugCube.GetComponent<Collider>().enabled = false;
			this._debugCube.SetActive(false);
			this._debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			this._debugSphere.name = "SceneDebugger_Sphere";
			this._debugSphere.GetComponent<Renderer>().material = this._debugMaterial;
			this._debugSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			this._debugSphere.GetComponent<Collider>().enabled = false;
			this._debugSphere.SetActive(false);
			this._debugNormal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			this._debugNormal.name = "SceneDebugger_Normal";
			this._debugNormal.GetComponent<Renderer>().material = this._debugMaterial;
			this._debugNormal.transform.localScale = new Vector3(0.02f, 0.1f, 0.02f);
			this._debugNormal.GetComponent<Collider>().enabled = false;
			this._debugNormal.SetActive(false);
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

		private void ShowHitNormal(Vector3 position, Vector3 normal)
		{
			if (this._debugNormal != null && position != Vector3.zero && normal != Vector3.zero)
			{
				this._debugNormal.SetActive(true);
				this._debugNormal.transform.rotation = Quaternion.FromToRotation(-Vector3.up, normal);
				this._debugNormal.transform.position = position + -this._debugNormal.transform.up * this._debugNormal.transform.localScale.y;
				return;
			}
			this._debugNormal.SetActive(false);
		}

		private void SetLogsText(string logsText, params object[] args)
		{
			if (this.logs)
			{
				this.logs.text = string.Format(logsText, args);
			}
		}

		public void ActivateTab(Image selectedTab)
		{
			foreach (Image image in this.Tabs)
			{
				image.color = this._backgroundColor;
			}
			selectedTab.color = this._foregroundColor;
		}

		public void ActivateMenu(CanvasGroup menuToActivate)
		{
			foreach (CanvasGroup canvasGroup in this.Menus)
			{
				this.ToggleCanvasGroup(canvasGroup, false);
			}
			this.ToggleCanvasGroup(menuToActivate, true);
		}

		private void ToggleCanvasGroup(CanvasGroup canvasGroup, bool shouldShow)
		{
			canvasGroup.interactable = shouldShow;
			canvasGroup.alpha = (shouldShow ? 1f : 0f);
			canvasGroup.blocksRaycasts = shouldShow;
		}

		private void Billboard()
		{
			if (!this._canvas)
			{
				return;
			}
			Vector3 forward = this._canvas.transform.position - this._cameraRig.centerEyeAnchor.transform.position;
			if (forward.sqrMagnitude > 0.01f)
			{
				Quaternion rotation = Quaternion.LookRotation(forward);
				this._canvas.transform.rotation = rotation;
			}
		}

		private void ToggleMenu(bool active)
		{
			if (!this._canvas)
			{
				return;
			}
			this._canvas.gameObject.SetActive(active);
			base.StartCoroutine(this.SnapCanvasInFrontOfCamera());
		}

		private IEnumerator SnapCanvasInFrontOfCamera()
		{
			yield return new WaitUntil(() => this._cameraRig && this._cameraRig.centerEyeAnchor.transform.position != Vector3.zero);
			base.transform.position = this._cameraRig.centerEyeAnchor.transform.position + this._cameraRig.centerEyeAnchor.transform.forward * 0.75f;
			yield break;
		}

		[Tooltip("Material used for visual helpers in debugging")]
		public Material visualHelperMaterial;

		[Tooltip("Visualize anchors")]
		public bool ShowDebugAnchors;

		[Tooltip("On start, place the canvas in front of the user")]
		public bool MoveCanvasInFrontOfCamera = true;

		[Tooltip("When false, use the interaction system already present in the scene")]
		public bool SetupInteractions;

		[Tooltip(" Text field for displaying logs")]
		public TextMeshProUGUI logs;

		[Tooltip("Dropdown to select what surface types to debug")]
		public TMP_Dropdown surfaceTypeDropdown;

		[Tooltip("Dropdown to select whether to export the global mesh with the scene JSON")]
		public TMP_Dropdown exportGlobalMeshJSONDropdown;

		[Tooltip("Dropdown to select what positioning methods to debug")]
		public TMP_Dropdown positioningMethodDropdown;

		[Tooltip("Text field for displaying room details")]
		public TextMeshProUGUI RoomDetails;

		[Tooltip("List of navigable tabs representing sub menus accessible from the top of the debug menu")]
		public List<Image> Tabs = new List<Image>();

		[Tooltip("List of canvas groups for different menus")]
		public List<CanvasGroup> Menus = new List<CanvasGroup>();

		[Tooltip("Helper for ray interactions")]
		public OVRRayHelper RayHelper;

		[Tooltip("Input module for handling VR input")]
		public OVRInputModule InputModule;

		[Tooltip("Raycaster for handling ray interactions")]
		public OVRRaycaster Raycaster;

		[Tooltip("Gaze pointer for VR interactions")]
		public OVRGazePointer GazePointer;

		private readonly Color _foregroundColor = new Color(0.2039f, 0.2549f, 0.2941f, 1f);

		private readonly Color _backgroundColor = new Color(0.11176f, 0.1568f, 0.1843f, 1f);

		private readonly int _srcBlend = Shader.PropertyToID("_SrcBlend");

		private readonly int _dstBlend = Shader.PropertyToID("_DstBlend");

		private readonly int _zWrite = Shader.PropertyToID("_ZWrite");

		private readonly int _cull = Shader.PropertyToID("_Cull");

		private readonly int _color = Shader.PropertyToID("_Color");

		private readonly List<GameObject> _debugAnchors = new List<GameObject>();

		private GameObject _globalMeshGO;

		private Material _debugMaterial;

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

		private Action _debugAction;

		private Canvas _canvas;

		private const float _spawnDistanceFromCamera = 0.75f;

		private SpaceMapGPU _spaceMapGPU;

		private MeshCollider _globalMeshCollider;

		private Material _navMeshMaterial;

		private Material _checkerMeshMaterial;
	}
}
