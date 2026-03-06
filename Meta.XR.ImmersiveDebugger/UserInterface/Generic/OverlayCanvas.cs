using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	[RequireComponent(typeof(Canvas))]
	public sealed class OverlayCanvas : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			OverlayCanvas.FrustumPlanes = new Plane[6];
		}

		public OverlayCanvasPanel Panel { get; set; }

		private void Start()
		{
			Transform transform = this.Panel.Transform;
			RectTransform rectTransform = this.Panel.RectTransform;
			Rect rect = rectTransform.rect;
			float width = rect.width;
			float height = rect.height;
			float num = (width >= height) ? 1f : (width / height);
			float num2 = (height >= width) ? 1f : (height / width);
			int num3 = this._scaleViewport ? 0 : 8;
			int num4 = Mathf.CeilToInt(num * (float)(1600 - num3 * 2));
			int num5 = Mathf.CeilToInt(num2 * (float)(1600 - num3 * 2));
			int num6 = num4 + num3 * 2;
			int num7 = num5 + num3 * 2;
			float x = width * ((float)num6 / (float)num4);
			float num8 = height * ((float)num7 / (float)num5);
			float num9 = (float)num4 / (float)num6;
			float num10 = (float)num5 / (float)num7;
			this._renderTexture = new RenderTexture(num6, num7, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
			{
				useMipMap = !this._scaleViewport
			};
			GameObject gameObject = new GameObject(base.name + " Overlay Camera");
			gameObject.transform.SetParent(transform, false);
			this._camera = gameObject.AddComponent<Camera>();
			this._camera.stereoTargetEye = StereoTargetEyeMask.None;
			this._camera.transform.position = transform.position - transform.forward;
			this._camera.orthographic = true;
			this._camera.enabled = false;
			this._camera.targetTexture = this._renderTexture;
			this._camera.cullingMask = 1 << base.gameObject.layer;
			this._camera.clearFlags = CameraClearFlags.Color;
			this._camera.backgroundColor = Color.clear;
			this._camera.orthographicSize = 0.5f * num8 * rectTransform.localScale.y;
			this._camera.nearClipPlane = 0.99f;
			this._camera.farClipPlane = 1.01f;
			this._quad = new Mesh
			{
				name = base.name + " Overlay Quad",
				vertices = new Vector3[]
				{
					new Vector3(-0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f),
					new Vector3(0.5f, 0.5f),
					new Vector3(0.5f, -0.5f)
				},
				uv = new Vector2[]
				{
					new Vector2(0f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),
					new Vector2(1f, 0f)
				},
				triangles = new int[]
				{
					0,
					1,
					2,
					2,
					3,
					0
				},
				bounds = new Bounds(Vector3.zero, Vector3.one)
			};
			this._quad.UploadMeshData(true);
			Shader shader = Shader.Find("UI/IDF Prerendered");
			this._defaultMat = new Material(shader)
			{
				mainTexture = this._renderTexture,
				color = Color.black,
				mainTextureOffset = new Vector2(0.5f - 0.5f * num9, 0.5f - 0.5f * num10),
				mainTextureScale = new Vector2(num9, num10)
			};
			GameObject gameObject2 = new GameObject(base.name + " MeshRenderer");
			gameObject2.transform.SetParent(base.transform, false);
			gameObject2.AddComponent<MeshFilter>().sharedMesh = this._quad;
			this._meshRenderer = gameObject2.AddComponent<MeshRenderer>();
			this._meshRenderer.sharedMaterial = this._defaultMat;
			gameObject2.layer = RuntimeSettings.Instance.MeshRendererLayer;
			gameObject2.transform.localScale = new Vector3(width, height, 1f);
			GameObject gameObject3 = new GameObject(base.name + " Overlay");
			gameObject3.transform.SetParent(base.transform, false);
			this._overlay = gameObject3.AddComponent<OVROverlay>();
			this._overlay.isDynamic = true;
			this._overlay.isAlphaPremultiplied = !Application.isMobilePlatform;
			this._overlay.textures[0] = this._renderTexture;
			this._overlay.currentOverlayType = OVROverlay.OverlayType.Overlay;
			this._overlay.compositionDepth = RuntimeSettings.Instance.OverlayDepth;
			this._overlay.noDepthBufferTesting = true;
			this._overlay.transform.localScale = new Vector3(x, num8, 1f);
			this._overlay.currentOverlayShape = OVROverlay.OverlayShape.Cylinder;
			gameObject3.transform.SetParent(this.Panel.Interface.Transform, false);
		}

		private void OnDestroy()
		{
			Object.Destroy(this._defaultMat);
			Object.Destroy(this._quad);
			Object.Destroy(this._renderTexture);
		}

		private void OnEnable()
		{
			if (this._meshRenderer)
			{
				this._meshRenderer.enabled = true;
			}
			if (this._overlay)
			{
				this._overlay.enabled = true;
			}
			if (this._camera)
			{
				this._camera.enabled = true;
			}
		}

		private void OnDisable()
		{
			if (this._meshRenderer)
			{
				this._meshRenderer.enabled = false;
			}
			if (this._overlay)
			{
				this._overlay.enabled = false;
			}
			if (this._camera)
			{
				this._camera.enabled = false;
			}
		}

		private bool ShouldRender(Camera baseCamera)
		{
			if (baseCamera == null)
			{
				return false;
			}
			for (int i = 0; i < 2; i++)
			{
				Camera.StereoscopicEye eye = (Camera.StereoscopicEye)i;
				GeometryUtility.CalculateFrustumPlanes(baseCamera.GetStereoProjectionMatrix(eye) * baseCamera.GetStereoViewMatrix(eye), OverlayCanvas.FrustumPlanes);
				if (GeometryUtility.TestPlanesAABB(OverlayCanvas.FrustumPlanes, this._meshRenderer.bounds))
				{
					return true;
				}
			}
			return false;
		}

		private void Update()
		{
			Camera main = Camera.main;
			if (!this.ShouldRender(main))
			{
				return;
			}
			Transform transform = this.Panel.Transform;
			RectTransform rectTransform = this.Panel.RectTransform;
			if (this._scaleViewport)
			{
				Rect rect = rectTransform.rect;
				float magnitude = (main.transform.position - base.transform.position).magnitude;
				float num = Mathf.Ceil(1f * Mathf.Max(rect.width * transform.lossyScale.x, rect.height * transform.lossyScale.y) / magnitude / 8f * (float)this._renderTexture.height) * 8f;
				num = Mathf.Clamp(num, 200f, (float)this._renderTexture.height);
				float num2 = num - 2f;
				this._camera.orthographicSize = 0.5f * rect.height * rectTransform.localScale.y * num / num2;
				float num3 = rect.width / rect.height;
				float num4 = num2 * num3;
				float num5 = Mathf.Ceil((num4 + 2f) * 0.5f) * 2f / (float)this._renderTexture.width;
				float num6 = num / (float)this._renderTexture.height;
				float num7 = num4 / (float)this._renderTexture.width;
				float num8 = num2 / (float)this._renderTexture.height;
				this._camera.rect = new Rect((1f - num5) / 2f, (1f - num6) / 2f, num5, num6);
				Rect rect2 = new Rect(0.5f - 0.5f * num7, 0.5f - 0.5f * num8, num7, num8);
				this._defaultMat.mainTextureOffset = rect2.min;
				this._defaultMat.mainTextureScale = rect2.size;
				this._overlay.overrideTextureRectMatrix = true;
				rect2.y = 1f - rect2.height - rect2.y;
				Rect rect3 = new Rect(0f, 0f, 1f, 1f);
				this._overlay.SetSrcDestRects(rect2, rect2, rect3, rect3);
			}
			this._camera.Render();
			Transform transform2 = this._overlay.transform;
			transform2.localPosition = Vector3.zero;
			transform2.localRotation = transform.localRotation;
			Vector2 vector = rectTransform.sizeDelta / this.Panel.PixelsPerUnit;
			transform2.localScale = new Vector3(vector.x, vector.y, this.Panel.SphericalCoordinates.x);
		}

		private static Plane[] FrustumPlanes = new Plane[6];

		private Camera _camera;

		private OVROverlay _overlay;

		private RenderTexture _renderTexture;

		private MeshRenderer _meshRenderer;

		private Mesh _quad;

		private Material _defaultMat;

		private const int MaxTextureSize = 1600;

		private const int MinTextureSize = 200;

		private const float PixelsPerUnit = 1f;

		private readonly bool _scaleViewport = Application.isMobilePlatform;
	}
}
