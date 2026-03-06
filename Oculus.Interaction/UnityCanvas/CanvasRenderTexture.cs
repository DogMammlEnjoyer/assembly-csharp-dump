using System;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
	[DisallowMultipleComponent]
	public class CanvasRenderTexture : MonoBehaviour
	{
		public LayerMask RenderingLayers
		{
			get
			{
				return this._renderingLayers;
			}
		}

		public int RenderScale
		{
			get
			{
				return this._renderScale;
			}
			set
			{
				if (this._renderScale < 1 || this._renderScale > 3)
				{
					throw new ArgumentException(string.Format("Render scale must be between 1 and 3, but was {0}", value));
				}
				if (this._renderScale == value)
				{
					return;
				}
				this._renderScale = value;
				if (base.isActiveAndEnabled && Application.isPlaying)
				{
					this.UpdateCamera();
				}
			}
		}

		public Camera OverlayCamera
		{
			get
			{
				return this._camera;
			}
		}

		public Texture Texture
		{
			get
			{
				return this._tex;
			}
		}

		public Vector2Int CalcAutoResolution()
		{
			if (this._canvas == null)
			{
				return CanvasRenderTexture.DEFAULT_TEXTURE_RES;
			}
			RectTransform component = this._canvas.GetComponent<RectTransform>();
			if (component == null)
			{
				return CanvasRenderTexture.DEFAULT_TEXTURE_RES;
			}
			Vector2 sizeDelta = component.sizeDelta;
			sizeDelta.x *= component.lossyScale.x;
			sizeDelta.y *= component.lossyScale.y;
			int a = Mathf.RoundToInt(this.UnitsToPixels(sizeDelta.x));
			int a2 = Mathf.RoundToInt(this.UnitsToPixels(sizeDelta.y));
			return new Vector2Int(Mathf.Max(a, 1), Mathf.Max(a2, 1));
		}

		public Vector2Int GetBaseResolutionToUse()
		{
			if (this._dimensionsDriveMode == CanvasRenderTexture.DriveMode.Auto)
			{
				return this.CalcAutoResolution();
			}
			return this._resolution;
		}

		public Vector2Int GetScaledResolutionToUse()
		{
			return Vector2Int.RoundToInt(this.GetBaseResolutionToUse() * (float)this._renderScale);
		}

		public float PixelsToUnits(float pixels)
		{
			return 1f / (float)this._pixelsPerUnit * pixels;
		}

		public float UnitsToPixels(float units)
		{
			return (float)this._pixelsPerUnit * units;
		}

		protected void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected void OnEnable()
		{
			if (this._started)
			{
				if (this._listener == null)
				{
					this._listener = this._canvas.gameObject.AddComponent<CanvasRenderTexture.TransformChangeListener>();
				}
				this._listener.WhenRectTransformDimensionsChanged += this.WhenCanvasRectTransformDimensionsChanged;
				this.UpdateCamera();
			}
		}

		private void WhenCanvasRectTransformDimensionsChanged()
		{
			this.UpdateCamera();
		}

		protected void OnDisable()
		{
			if (this._started)
			{
				Camera camera = this._camera;
				if (((camera != null) ? camera.gameObject : null) != null)
				{
					Object.Destroy(this._camera.gameObject);
				}
				if (this._tex != null)
				{
					Object.DestroyImmediate(this._tex);
				}
				if (this._listener != null)
				{
					this._listener.WhenRectTransformDimensionsChanged -= this.WhenCanvasRectTransformDimensionsChanged;
				}
			}
		}

		protected void UpdateCamera()
		{
			if (!Application.isPlaying || !this._started)
			{
				return;
			}
			try
			{
				if (this._camera == null)
				{
					GameObject gameObject = this.CreateChildObject("__Camera");
					this._camera = gameObject.AddComponent<Camera>();
					this._camera.orthographic = true;
					this._camera.nearClipPlane = -0.1f;
					this._camera.farClipPlane = 0.1f;
					this._camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
					this._camera.clearFlags = CameraClearFlags.Color;
				}
				this.UpdateRenderTexture();
				this.UpdateOrthoSize();
				this.UpdateCameraCullingMask();
			}
			finally
			{
			}
		}

		protected void UpdateRenderTexture()
		{
			try
			{
				Vector2Int scaledResolutionToUse = this.GetScaledResolutionToUse();
				if (this._tex == null || this._tex.width != scaledResolutionToUse.x || this._tex.height != scaledResolutionToUse.y || this._tex.autoGenerateMips != this._generateMipMaps)
				{
					if (this._tex != null)
					{
						this._camera.targetTexture = null;
						Object.DestroyImmediate(this._tex);
					}
					this._tex = new RenderTexture(scaledResolutionToUse.x, scaledResolutionToUse.y, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
					this._tex.filterMode = FilterMode.Bilinear;
					this._tex.autoGenerateMips = this._generateMipMaps;
					this._camera.targetTexture = this._tex;
					this.OnUpdateRenderTexture(this._tex);
				}
			}
			finally
			{
			}
		}

		private void UpdateOrthoSize()
		{
			if (this._camera != null)
			{
				this._camera.orthographicSize = this.PixelsToUnits((float)this.GetBaseResolutionToUse().y) * 0.5f;
			}
		}

		private void UpdateCameraCullingMask()
		{
			if (this._camera != null)
			{
				this._camera.cullingMask = this._renderingLayers.value;
			}
		}

		protected GameObject CreateChildObject(string name)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(this._canvas.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		public void InjectAllCanvasRenderTexture(Canvas canvas, int pixelsPerUnit, int renderScale, LayerMask renderingLayers, bool generateMipMaps)
		{
			this.InjectCanvas(canvas);
			this.InjectPixelsPerUnit(pixelsPerUnit);
			this.InjectRenderScale(renderScale);
			this.InjectRenderingLayers(renderingLayers);
			this.InjectGenerateMipMaps(generateMipMaps);
		}

		public void InjectCanvas(Canvas canvas)
		{
			this._canvas = canvas;
		}

		public void InjectPixelsPerUnit(int pixelsPerUnit)
		{
			this._pixelsPerUnit = pixelsPerUnit;
		}

		public void InjectRenderScale(int renderScale)
		{
			this._renderScale = renderScale;
		}

		public void InjectRenderingLayers(LayerMask renderingLayers)
		{
			this._renderingLayers = renderingLayers;
		}

		public void InjectGenerateMipMaps(bool generateMipMaps)
		{
			this._generateMipMaps = generateMipMaps;
		}

		public const int DEFAULT_UI_LAYERMASK = 32;

		private static readonly Vector2Int DEFAULT_TEXTURE_RES = new Vector2Int(128, 128);

		[Tooltip("The Unity canvas that will be rendered.")]
		[SerializeField]
		private Canvas _canvas;

		[Tooltip("Used to increase resolution of rendered canvas. If you need extra resolution, you can use this as a whole-integer multiplier of the final resolution used to render the texture.")]
		[Range(1f, 3f)]
		[Delayed]
		[SerializeField]
		private int _renderScale = 1;

		[Tooltip("If set to auto, texture dimensions will take the size of the attached RectTransform into consideration, in addition to the configured pixel-per-unit ratio.")]
		[SerializeField]
		private CanvasRenderTexture.DriveMode _dimensionsDriveMode;

		[Tooltip("The exact pixel resolution of the texture used for interface rendering.")]
		[Delayed]
		[SerializeField]
		private Vector2Int _resolution = CanvasRenderTexture.DEFAULT_TEXTURE_RES;

		[Tooltip("Whether or not mip-maps should be auto-generated for the texture. Can help aliasing if the texture can be viewed from many difference distances.")]
		[SerializeField]
		private bool _generateMipMaps;

		[Tooltip("Pixels per unit ratio used to drive the texture dimensions. Determines the RenderTexture size from the canvas world size.")]
		[SerializeField]
		private int _pixelsPerUnit = 100;

		[Header("Rendering Settings")]
		[Tooltip("The layers to render when the rendering texture is created. All child renderers should be part of this mask.")]
		[SerializeField]
		private LayerMask _renderingLayers = 32;

		public Action<Texture> OnUpdateRenderTexture = delegate(Texture <p0>)
		{
		};

		private CanvasRenderTexture.TransformChangeListener _listener;

		private RenderTexture _tex;

		private Camera _camera;

		protected bool _started;

		private class TransformChangeListener : MonoBehaviour
		{
			public event Action WhenRectTransformDimensionsChanged = delegate()
			{
			};

			private void OnRectTransformDimensionsChange()
			{
				this.WhenRectTransformDimensionsChanged();
			}
		}

		public enum DriveMode
		{
			Auto,
			Manual
		}

		public static class Properties
		{
			public static readonly string DimensionDriveMode = "_dimensionsDriveMode";

			public static readonly string Resolution = "_resolution";

			public static readonly string RenderScale = "_renderScale";

			public static readonly string PixelsPerUnit = "_pixelsPerUnit";

			public static readonly string RenderLayers = "_renderingLayers";

			public static readonly string GenerateMipMaps = "_generateMipMaps";

			public static readonly string Canvas = "_canvas";
		}
	}
}
