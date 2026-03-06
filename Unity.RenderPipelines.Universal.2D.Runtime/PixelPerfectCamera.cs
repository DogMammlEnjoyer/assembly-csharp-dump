using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.U2D;

namespace UnityEngine.Rendering.Universal
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Rendering/2D/Pixel Perfect Camera")]
	[RequireComponent(typeof(Camera))]
	[MovedFrom(true, "UnityEngine.Experimental.Rendering.Universal", null, null)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/index.html?subfolder=/manual/2d-pixelperfect.html%23properties")]
	public class PixelPerfectCamera : MonoBehaviour, IPixelPerfectCamera, ISerializationCallbackReceiver
	{
		public PixelPerfectCamera.CropFrame cropFrame
		{
			get
			{
				return this.m_CropFrame;
			}
			set
			{
				this.m_CropFrame = value;
			}
		}

		public PixelPerfectCamera.GridSnapping gridSnapping
		{
			get
			{
				return this.m_GridSnapping;
			}
			set
			{
				this.m_GridSnapping = value;
			}
		}

		public float orthographicSize
		{
			get
			{
				return this.m_Internal.orthoSize;
			}
		}

		public int assetsPPU
		{
			get
			{
				return this.m_AssetsPPU;
			}
			set
			{
				this.m_AssetsPPU = ((value > 0) ? value : 1);
			}
		}

		public int refResolutionX
		{
			get
			{
				return this.m_RefResolutionX;
			}
			set
			{
				this.m_RefResolutionX = ((value > 0) ? value : 1);
			}
		}

		public int refResolutionY
		{
			get
			{
				return this.m_RefResolutionY;
			}
			set
			{
				this.m_RefResolutionY = ((value > 0) ? value : 1);
			}
		}

		[Obsolete("Use gridSnapping instead", false)]
		public bool upscaleRT
		{
			get
			{
				return this.m_GridSnapping == PixelPerfectCamera.GridSnapping.UpscaleRenderTexture;
			}
			set
			{
				this.m_GridSnapping = (value ? PixelPerfectCamera.GridSnapping.UpscaleRenderTexture : PixelPerfectCamera.GridSnapping.None);
			}
		}

		[Obsolete("Use gridSnapping instead", false)]
		public bool pixelSnapping
		{
			get
			{
				return this.m_GridSnapping == PixelPerfectCamera.GridSnapping.PixelSnapping;
			}
			set
			{
				this.m_GridSnapping = (value ? PixelPerfectCamera.GridSnapping.PixelSnapping : PixelPerfectCamera.GridSnapping.None);
			}
		}

		[Obsolete("Use cropFrame instead", false)]
		public bool cropFrameX
		{
			get
			{
				return this.m_CropFrame == PixelPerfectCamera.CropFrame.StretchFill || this.m_CropFrame == PixelPerfectCamera.CropFrame.Windowbox || this.m_CropFrame == PixelPerfectCamera.CropFrame.Pillarbox;
			}
			set
			{
				if (value)
				{
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.None)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.Pillarbox;
						return;
					}
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.Letterbox)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.Windowbox;
						return;
					}
				}
				else
				{
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.Pillarbox)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.None;
						return;
					}
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.Windowbox || this.m_CropFrame == PixelPerfectCamera.CropFrame.StretchFill)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.Letterbox;
					}
				}
			}
		}

		[Obsolete("Use cropFrame instead", false)]
		public bool cropFrameY
		{
			get
			{
				return this.m_CropFrame == PixelPerfectCamera.CropFrame.StretchFill || this.m_CropFrame == PixelPerfectCamera.CropFrame.Windowbox || this.m_CropFrame == PixelPerfectCamera.CropFrame.Letterbox;
			}
			set
			{
				if (value)
				{
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.None)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.Letterbox;
						return;
					}
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.Pillarbox)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.Windowbox;
						return;
					}
				}
				else
				{
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.Letterbox)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.None;
						return;
					}
					if (this.m_CropFrame == PixelPerfectCamera.CropFrame.Windowbox || this.m_CropFrame == PixelPerfectCamera.CropFrame.StretchFill)
					{
						this.m_CropFrame = PixelPerfectCamera.CropFrame.Pillarbox;
					}
				}
			}
		}

		[Obsolete("Use cropFrame instead", false)]
		public bool stretchFill
		{
			get
			{
				return this.m_CropFrame == PixelPerfectCamera.CropFrame.StretchFill;
			}
			set
			{
				if (value)
				{
					this.m_CropFrame = PixelPerfectCamera.CropFrame.StretchFill;
					return;
				}
				this.m_CropFrame = PixelPerfectCamera.CropFrame.Windowbox;
			}
		}

		public int pixelRatio
		{
			get
			{
				if (!this.m_CinemachineCompatibilityMode)
				{
					return this.m_Internal.zoom;
				}
				if (this.m_GridSnapping == PixelPerfectCamera.GridSnapping.UpscaleRenderTexture)
				{
					return this.m_Internal.zoom * this.m_Internal.cinemachineVCamZoom;
				}
				return this.m_Internal.cinemachineVCamZoom;
			}
		}

		public bool requiresUpscalePass
		{
			get
			{
				return this.m_Internal.requiresUpscaling;
			}
		}

		public Vector3 RoundToPixel(Vector3 position)
		{
			float unitsPerPixel = this.m_Internal.unitsPerPixel;
			if (unitsPerPixel == 0f)
			{
				return position;
			}
			Vector3 result;
			result.x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
			result.y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
			result.z = Mathf.Round(position.z / unitsPerPixel) * unitsPerPixel;
			return result;
		}

		public float CorrectCinemachineOrthoSize(float targetOrthoSize)
		{
			this.m_CinemachineCompatibilityMode = true;
			if (this.m_Internal == null)
			{
				return targetOrthoSize;
			}
			return this.m_Internal.CorrectCinemachineOrthoSize(targetOrthoSize);
		}

		internal FilterMode finalBlitFilterMode
		{
			get
			{
				if (this.m_FilterMode != PixelPerfectCamera.PixelPerfectFilterMode.RetroAA)
				{
					return FilterMode.Point;
				}
				return FilterMode.Bilinear;
			}
		}

		internal Vector2Int offscreenRTSize
		{
			get
			{
				return new Vector2Int(this.m_Internal.offscreenRTWidth, this.m_Internal.offscreenRTHeight);
			}
		}

		private Vector2Int cameraRTSize
		{
			get
			{
				RenderTexture targetTexture = this.m_Camera.targetTexture;
				if (!(targetTexture == null))
				{
					return new Vector2Int(targetTexture.width, targetTexture.height);
				}
				return new Vector2Int(Screen.width, Screen.height);
			}
		}

		private void PixelSnap()
		{
			Vector3 position = this.m_Camera.transform.position;
			Vector3 vector = this.RoundToPixel(position) - position;
			vector.z = -vector.z;
			Matrix4x4 inverse = Matrix4x4.TRS(position + vector, Quaternion.identity, Vector3.one).inverse;
			Matrix4x4 inverse2 = Matrix4x4.Rotate(this.m_Camera.transform.rotation).inverse;
			Matrix4x4 lhs = Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
			this.m_Camera.worldToCameraMatrix = lhs * inverse2 * inverse;
		}

		private void Awake()
		{
			this.m_Camera = base.GetComponent<Camera>();
			this.m_Internal = new PixelPerfectCameraInternal(this);
			this.UpdateCameraProperties();
		}

		private void UpdateCameraProperties()
		{
			Vector2Int cameraRTSize = this.cameraRTSize;
			this.m_Internal.CalculateCameraProperties(cameraRTSize.x, cameraRTSize.y);
			if (this.m_Internal.useOffscreenRT)
			{
				this.m_Camera.pixelRect = this.m_Internal.CalculateFinalBlitPixelRect(cameraRTSize.x, cameraRTSize.y);
				return;
			}
			this.m_Camera.rect = new Rect(0f, 0f, 1f, 1f);
		}

		private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			if (camera == this.m_Camera)
			{
				this.UpdateCameraProperties();
				this.PixelSnap();
				if (!this.m_CinemachineCompatibilityMode)
				{
					this.m_Camera.orthographicSize = this.m_Internal.orthoSize;
				}
				PixelPerfectRendering.pixelSnapSpacing = this.m_Internal.unitsPerPixel;
			}
		}

		private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			if (camera == this.m_Camera)
			{
				PixelPerfectRendering.pixelSnapSpacing = 0f;
			}
		}

		private void OnEnable()
		{
			this.m_CinemachineCompatibilityMode = false;
			RenderPipelineManager.beginCameraRendering += this.OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering += this.OnEndCameraRendering;
		}

		internal void OnDisable()
		{
			RenderPipelineManager.beginCameraRendering -= this.OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering -= this.OnEndCameraRendering;
			this.m_Camera.rect = new Rect(0f, 0f, 1f, 1f);
			this.m_Camera.ResetWorldToCameraMatrix();
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
		}

		[SerializeField]
		private int m_AssetsPPU = 100;

		[SerializeField]
		private int m_RefResolutionX = 320;

		[SerializeField]
		private int m_RefResolutionY = 180;

		[SerializeField]
		private PixelPerfectCamera.CropFrame m_CropFrame;

		[SerializeField]
		private PixelPerfectCamera.GridSnapping m_GridSnapping;

		[SerializeField]
		private PixelPerfectCamera.PixelPerfectFilterMode m_FilterMode;

		private Camera m_Camera;

		private PixelPerfectCameraInternal m_Internal;

		private bool m_CinemachineCompatibilityMode;

		public enum CropFrame
		{
			None,
			Pillarbox,
			Letterbox,
			Windowbox,
			StretchFill
		}

		public enum GridSnapping
		{
			None,
			PixelSnapping,
			UpscaleRenderTexture
		}

		public enum PixelPerfectFilterMode
		{
			RetroAA,
			Point
		}

		private enum ComponentVersions
		{
			Version_Unserialized,
			Version_1
		}
	}
}
