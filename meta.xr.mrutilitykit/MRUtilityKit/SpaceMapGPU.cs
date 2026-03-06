using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Meta.XR.MRUtilityKit
{
	public class SpaceMapGPU : MonoBehaviour
	{
		public UnityEvent SpaceMapCreatedEvent { get; private set; } = new UnityEvent();

		public UnityEvent<MRUKRoom> SpaceMapRoomCreatedEvent { get; private set; } = new UnityEvent<MRUKRoom>();

		public UnityEvent SpaceMapUpdatedEvent { get; private set; } = new UnityEvent();

		public RenderTexture GetSpaceMap(MRUKRoom room = null)
		{
			if (room == null)
			{
				return this.RenderTexture;
			}
			RenderTexture result;
			if (!this._roomTextures.TryGetValue(room, out result))
			{
				Debug.Log(string.Format("Rendertexture for room {0} not found, returning default texture. Call StartSpaceMap(room) to create a texture for a specific room.", room));
				return this.RenderTexture;
			}
			return result;
		}

		public void StartSpaceMap(MRUK.RoomFilter roomFilter)
		{
			List<MRUKRoom> rooms;
			switch (roomFilter)
			{
			case MRUK.RoomFilter.None:
				return;
			case MRUK.RoomFilter.CurrentRoomOnly:
				rooms = new List<MRUKRoom>
				{
					MRUK.Instance.GetCurrentRoom()
				};
				break;
			case MRUK.RoomFilter.AllRooms:
				rooms = MRUK.Instance.Rooms;
				break;
			default:
				throw new ArgumentOutOfRangeException("roomFilter", roomFilter, null);
			}
			this.StartSpaceMapInternal(rooms, this.RenderTexture);
			this.SpaceMapCreatedEvent.Invoke();
		}

		public void StartSpaceMap(MRUKRoom room)
		{
			RenderTexture renderTexture;
			if (!this._roomTextures.TryGetValue(room, out renderTexture))
			{
				renderTexture = SpaceMapGPU.CreateNewRenderTexture(this.RenderTexture.width);
				this._roomTextures[room] = renderTexture;
			}
			this.StartSpaceMapInternal(new List<MRUKRoom>
			{
				room
			}, renderTexture);
			this.SpaceMapRoomCreatedEvent.Invoke(room);
		}

		public Color GetColorAtPosition(Vector3 worldPosition)
		{
			if (this._currentRoomBounds.size.x <= 0f)
			{
				return Color.black;
			}
			Vector2 vector = Rect.PointToNormalized(this._currentRoomBounds, new Vector2(worldPosition.x, worldPosition.z));
			Color pixelBilinear = this.OutputTexture.GetPixelBilinear(vector.x, vector.y);
			float num = 1f - pixelBilinear.r;
			if (pixelBilinear.b <= 0f)
			{
				return this.MapGradient.Evaluate((num >= 0f && num <= 1f) ? num : 0f);
			}
			return this.InsideObjectColor;
		}

		private void Awake()
		{
			this._csSpaceMapKernel = this.CSSpaceMap.FindKernel("SpaceMap");
			this._csFillSpaceMapKernel = this.CSSpaceMap.FindKernel("FillSpaceMap");
			this._csPrepareSpaceMapKernel = this.CSSpaceMap.FindKernel("PrepareSpaceMap");
			this._matFloor = new Material(Shader.Find("Oculus/Unlit"));
			this._matObjects = new Material(Shader.Find("Oculus/Unlit"));
			this._matFloor.color = this._colorFloorWall;
			this._matObjects.color = this._colorSceneObjects;
		}

		private void Start()
		{
			this.InitUpdateGradientTexture();
			this.ApplyMaterial();
			OVRTelemetry.Start(651896914, 0, -1L).Send();
		}

		private void OnEnable()
		{
			if (MRUK.Instance != null)
			{
				MRUK.Instance.RegisterSceneLoadedCallback(new UnityAction(this.SceneLoaded));
				if (this.TrackUpdates)
				{
					MRUK.Instance.RoomCreatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
					MRUK.Instance.RoomRemovedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
					MRUK.Instance.RoomUpdatedEvent.AddListener(new UnityAction<MRUKRoom>(this.ReceiveUpdatedRoom));
				}
			}
		}

		private void OnDisable()
		{
			if (MRUK.Instance == null)
			{
				return;
			}
			MRUK.Instance.SceneLoadedEvent.RemoveListener(new UnityAction(this.SceneLoaded));
			if (this.TrackUpdates)
			{
				MRUK.Instance.RoomCreatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveCreatedRoom));
				MRUK.Instance.RoomRemovedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveRemovedRoom));
				MRUK.Instance.RoomUpdatedEvent.RemoveListener(new UnityAction<MRUKRoom>(this.ReceiveUpdatedRoom));
			}
		}

		private void Update()
		{
			Shader.SetGlobalMatrix(SpaceMapGPU.SpaceMapCameraMatrixID, this._orthoCamProjectionViewMatrix);
			if (this.DebugPlane != null && this.DebugPlane.activeSelf != this.ShowDebugPlane)
			{
				this.DebugPlane.SetActive(this.ShowDebugPlane);
			}
		}

		private void StartSpaceMapInternal(List<MRUKRoom> rooms, RenderTexture rt)
		{
			this.InitializeOrthoCameraMatrixParameters(this.GetBoundingBox(rooms));
			this.UpdateBuffer(rooms, rt);
		}

		private void SceneLoaded()
		{
			if (this.CreateOnStart == MRUK.RoomFilter.None)
			{
				return;
			}
			this.StartSpaceMap(this.CreateOnStart);
		}

		private bool IsInitialized()
		{
			return this._RTextures[0] != null && this._isOrthoCameraInitialized;
		}

		private void UpdateBuffer(MRUKRoom room)
		{
			RenderTexture renderTexture;
			if (!this._roomTextures.TryGetValue(room, out renderTexture))
			{
				renderTexture = SpaceMapGPU.CreateNewRenderTexture(this.RenderTexture.width);
				this._roomTextures[room] = renderTexture;
			}
			this.UpdateBuffer(new List<MRUKRoom>
			{
				room
			}, renderTexture);
		}

		private void UpdateBuffer(List<MRUKRoom> rooms, RenderTexture rt)
		{
			CommandBuffer commandBuffer = new CommandBuffer
			{
				name = "SpaceMap"
			};
			RenderTexture.active = rt;
			GL.Clear(true, true, new Color(1f, 1f, 1f, 1f));
			RenderTexture.active = null;
			commandBuffer.SetRenderTarget(rt);
			int textureDimension = this.TextureDimension;
			if (this._RTextures[0] == null || this._RTextures[0].width != textureDimension || this._RTextures[0].height != textureDimension)
			{
				SpaceMapGPU.TryReleaseRT(this._RTextures[0]);
				SpaceMapGPU.TryReleaseRT(this._RTextures[1]);
				this._RTextures[0] = SpaceMapGPU.CreateNewRenderTexture(textureDimension);
				this._RTextures[1] = SpaceMapGPU.CreateNewRenderTexture(textureDimension);
			}
			commandBuffer.SetViewProjectionMatrices(this._orthoCamViewMatrix, this._orthoCamProjectionMatrix);
			this.DrawRoomsIntoCB(commandBuffer, rooms);
			Graphics.ExecuteCommandBuffer(commandBuffer);
			this.RunSpaceMap(rt);
			if (this.CreateOutputTexture)
			{
				RenderTexture.active = rt;
				this.OutputTexture.ReadPixels(new Rect(0f, 0f, (float)this.TextureDimension, (float)this.TextureDimension), 0, 0);
				this.OutputTexture.Apply();
				RenderTexture.active = null;
			}
			commandBuffer.Clear();
			commandBuffer.Dispose();
		}

		private void DrawRoomsIntoCB(CommandBuffer commandBuffer, List<MRUKRoom> rooms)
		{
			foreach (MRUKRoom mrukroom in rooms)
			{
				Mesh mesh = Utilities.SetupAnchorMeshGeometry(mrukroom.FloorAnchor, false, null);
				commandBuffer.DrawMesh(mesh, mrukroom.FloorAnchor.transform.localToWorldMatrix, this._matFloor);
				foreach (MRUKAnchor mrukanchor in mrukroom.Anchors)
				{
					if (mrukanchor.HasAnyLabel(this.SceneObjectLabels))
					{
						Mesh mesh2 = Utilities.SetupAnchorMeshGeometry(mrukanchor, false, null);
						commandBuffer.DrawMesh(mesh2, mrukanchor.transform.localToWorldMatrix, this._matObjects);
					}
				}
			}
		}

		private void RunSpaceMap(RenderTexture rt)
		{
			this.CSSpaceMap.SetInt(SpaceMapGPU.WidthID, this.TextureDimension);
			this.CSSpaceMap.SetInt(SpaceMapGPU.HeightID, this.TextureDimension);
			this.CSSpaceMap.SetVector(SpaceMapGPU.ColorFloorWallID, this._colorFloorWall);
			this.CSSpaceMap.SetVector(SpaceMapGPU.ColorSceneObjectsID, this._colorSceneObjects);
			this.CSSpaceMap.SetVector(SpaceMapGPU.ColorVirtualObjectsID, this._colorVirtualObjects);
			int threadGroupsX = Mathf.CeilToInt((float)this.TextureDimension / 8f);
			int threadGroupsY = Mathf.CeilToInt((float)this.TextureDimension / 8f);
			this.CSSpaceMap.SetTexture(this._csPrepareSpaceMapKernel, SpaceMapGPU.SourceID, rt);
			this.CSSpaceMap.SetTexture(this._csPrepareSpaceMapKernel, SpaceMapGPU.ResultID, this._RTextures[0]);
			this.CSSpaceMap.Dispatch(this._csPrepareSpaceMapKernel, threadGroupsX, threadGroupsY, 1);
			int num = (int)Mathf.Log((float)this.TextureDimension, 2f);
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				int val = (int)Mathf.Pow(2f, (float)(num - i - 1));
				num2 = i % 2;
				num3 = (i + 1) % 2;
				this.CSSpaceMap.SetInt(SpaceMapGPU.StepID, val);
				this.CSSpaceMap.SetTexture(this._csSpaceMapKernel, SpaceMapGPU.SourceID, this._RTextures[num2]);
				this.CSSpaceMap.SetTexture(this._csSpaceMapKernel, SpaceMapGPU.ResultID, this._RTextures[num3]);
				this.CSSpaceMap.Dispatch(this._csSpaceMapKernel, threadGroupsX, threadGroupsY, 1);
			}
			this.CSSpaceMap.SetTexture(this._csFillSpaceMapKernel, SpaceMapGPU.SourceID, this._RTextures[num3]);
			this.CSSpaceMap.SetTexture(this._csFillSpaceMapKernel, SpaceMapGPU.ResultID, this._RTextures[num2]);
			this.CSSpaceMap.Dispatch(this._csFillSpaceMapKernel, threadGroupsX, threadGroupsY, 1);
			Graphics.Blit(this._RTextures[num2], rt);
			this.gradientMaterial.SetTexture("_MainTex", rt);
			this.SpaceMapUpdatedEvent.Invoke();
		}

		private void ReceiveUpdatedRoom(MRUKRoom room)
		{
			if (this.TrackUpdates)
			{
				this.RegisterAnchorUpdates(room);
				if (this.IsInitialized())
				{
					this.UpdateBuffer(room);
				}
			}
		}

		private void ReceiveCreatedRoom(MRUKRoom room)
		{
			if (this.TrackUpdates && this.CreateOnStart == MRUK.RoomFilter.AllRooms)
			{
				this.RegisterAnchorUpdates(room);
				if (this.IsInitialized())
				{
					this.UpdateBuffer(room);
				}
			}
		}

		private void ReceiveRemovedRoom(MRUKRoom room)
		{
			this.UnregisterAnchorUpdates(room);
			this._roomTextures.Remove(room);
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
			if (!this.TrackUpdates)
			{
				return;
			}
			if (this.IsInitialized())
			{
				this.UpdateBuffer(anchor.Room);
			}
		}

		private void ReceiveAnchorRemovedCallback(MRUKAnchor anchor)
		{
			if (this.IsInitialized())
			{
				this.UpdateBuffer(anchor.Room);
			}
		}

		private void ReceiveAnchorCreatedEvent(MRUKAnchor anchor)
		{
			if (!this.TrackUpdates)
			{
				return;
			}
			if (this.IsInitialized())
			{
				this.UpdateBuffer(anchor.Room);
			}
		}

		private static RenderTexture CreateNewRenderTexture(int wh)
		{
			RenderTexture renderTexture = new RenderTexture(wh, wh, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
			renderTexture.enableRandomWrite = true;
			renderTexture.Create();
			return renderTexture;
		}

		private static void TryReleaseRT(RenderTexture renderTexture)
		{
			if (renderTexture != null)
			{
				renderTexture.Release();
			}
		}

		private void ApplyMaterial()
		{
			this.gradientMaterial.SetTexture("_GradientTex", this._gradientTexture);
			this.gradientMaterial.SetColor("_InsideColor", this.InsideObjectColor);
			if (this.DebugPlane != null)
			{
				this.DebugPlane.GetComponent<Renderer>().material = this.gradientMaterial;
			}
		}

		private void InitUpdateGradientTexture()
		{
			if (this._gradientTexture == null)
			{
				this._gradientTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
			}
			for (int i = 0; i <= this._gradientTexture.width; i++)
			{
				float time = (float)i / ((float)this._gradientTexture.width - 1f);
				this._gradientTexture.SetPixel(i, 0, this.MapGradient.Evaluate(time));
			}
			this._gradientTexture.Apply();
		}

		private void InitializeOrthoCameraMatrixParameters(Rect roomBounds)
		{
			this._currentRoomBounds = roomBounds;
			float size = Mathf.Max(roomBounds.width, roomBounds.height) / 2f;
			this._orthoCamProjectionMatrix = this.CalculateOrthographicProjMatrix(size, 1f, 0.1f, 100f);
			this._orthoCamViewMatrix = this.CalculateViewMatrix();
			this._orthoCamProjectionViewMatrix = this._orthoCamProjectionMatrix * this._orthoCamViewMatrix;
			this._isOrthoCameraInitialized = true;
			this.HandleDebugPlane(roomBounds);
		}

		private Matrix4x4 CalculateOrthographicProjMatrix(float size, float aspect, float near, float far)
		{
			float num = size * aspect;
			float left = -num;
			float bottom = -size;
			return Matrix4x4.Ortho(left, num, bottom, size, near, far);
		}

		private Matrix4x4 CalculateViewMatrix()
		{
			return Matrix4x4.Inverse(Matrix4x4.TRS(new Vector3(this._currentRoomBounds.center.x, 10f, this._currentRoomBounds.center.y), Quaternion.Euler(90f, 0f, 0f), new Vector3(1f, 1f, -1f)));
		}

		private Rect GetBoundingBox(List<MRUKRoom> rooms)
		{
			Bounds bounds = default(Bounds);
			foreach (MRUKRoom mrukroom in rooms)
			{
				if (bounds.extents != Vector3.zero)
				{
					bounds.Encapsulate(mrukroom.GetRoomBounds());
				}
				else
				{
					bounds = mrukroom.GetRoomBounds();
				}
			}
			bounds.Expand(this.CameraCaptureBorderBuffer);
			return Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
		}

		private void HandleDebugPlane(Rect rect)
		{
			if (this.DebugPlane == null)
			{
				return;
			}
			float num = rect.size.x / 10f;
			float num2 = rect.size.y / 10f;
			float x = rect.center.x;
			float y = rect.center.y;
			if (float.IsNaN(x) || float.IsNaN(y) || num == float.NegativeInfinity || num2 == float.NegativeInfinity)
			{
				return;
			}
			this.DebugPlane.transform.localScale = new Vector3(num, 1f, num2);
			this.DebugPlane.transform.position = new Vector3(x, this.DebugPlane.transform.position.y, y);
		}

		[Tooltip("When the scene data is loaded, this controls what room(s) the spacemap will run on.")]
		[Header("Scene and Room Settings")]
		public MRUK.RoomFilter CreateOnStart = MRUK.RoomFilter.CurrentRoomOnly;

		[Tooltip("If enabled, updates on scene elements such as rooms and anchors will be handled by this class")]
		internal bool TrackUpdates = true;

		[Space]
		[Header("Textures")]
		[SerializeField]
		[Tooltip("Use this dimension for SpaceMap in X and Y")]
		public int TextureDimension = 512;

		[Tooltip("Colorize the SpaceMap with this Gradient")]
		public Gradient MapGradient = new Gradient();

		[Space]
		[Header("SpaceMap Settings")]
		[SerializeField]
		private Material gradientMaterial;

		[SerializeField]
		private ComputeShader CSSpaceMap;

		[Tooltip("Those Labels will be taken into account when running the SpaceMap")]
		[SerializeField]
		private MRUKAnchor.SceneLabels SceneObjectLabels;

		[Tooltip("Set a color for the inside of an Object")]
		[SerializeField]
		private Color InsideObjectColor;

		[Tooltip("Add this to the border of the capture Camera")]
		[SerializeField]
		private float CameraCaptureBorderBuffer = 0.5f;

		[Space]
		[Header("SpaceMap Debug Settings")]
		[SerializeField]
		[Tooltip("This setting affects your performance. If enabled, the TextureMap will be filled with the SpaceMap")]
		private bool CreateOutputTexture;

		[Tooltip("The Spacemap will be rendered into this Texture.")]
		[SerializeField]
		internal Texture2D OutputTexture;

		[Tooltip("Add here a debug plane")]
		[SerializeField]
		private GameObject DebugPlane;

		[SerializeField]
		private bool ShowDebugPlane;

		private Color _colorFloorWall = Color.red;

		private Color _colorSceneObjects = Color.green;

		private Color _colorVirtualObjects = Color.blue;

		private Material _matFloor;

		private Material _matObjects;

		private bool _isOrthoCameraInitialized;

		private Matrix4x4 _orthoCamProjectionMatrix;

		private Matrix4x4 _orthoCamViewMatrix;

		private Matrix4x4 _orthoCamProjectionViewMatrix;

		private Rect _currentRoomBounds;

		private RenderTexture[] _RTextures = new RenderTexture[2];

		private const string OculusUnlitShader = "Oculus/Unlit";

		private Texture2D _gradientTexture;

		private int _csSpaceMapKernel;

		private int _csFillSpaceMapKernel;

		private int _csPrepareSpaceMapKernel;

		private const string SHADER_GLOBAL_SPACEMAPCAMERAMATRIX = "_SpaceMapProjectionViewMatrix";

		private const float CameraDistance = 10f;

		private const float AspectRatio = 1f;

		private const float NearClipPlane = 0.1f;

		private const float FarClipPlane = 100f;

		[SerializeField]
		private RenderTexture RenderTexture;

		private static readonly int WidthID = Shader.PropertyToID("Width");

		private static readonly int HeightID = Shader.PropertyToID("Height");

		private static readonly int ColorFloorWallID = Shader.PropertyToID("ColorFloorWall");

		private static readonly int ColorSceneObjectsID = Shader.PropertyToID("ColorSceneObjects");

		private static readonly int ColorVirtualObjectsID = Shader.PropertyToID("ColorVirtualObjects");

		private static readonly int StepID = Shader.PropertyToID("Step");

		private static readonly int SourceID = Shader.PropertyToID("Source");

		private static readonly int ResultID = Shader.PropertyToID("Result");

		private static readonly int SpaceMapCameraMatrixID = Shader.PropertyToID("_SpaceMapProjectionViewMatrix");

		private Dictionary<MRUKRoom, RenderTexture> _roomTextures = new Dictionary<MRUKRoom, RenderTexture>();
	}
}
