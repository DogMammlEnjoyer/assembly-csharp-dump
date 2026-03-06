using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Unity.Cinemachine
{
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Storyboard")]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineStoryboard.html")]
	public class CinemachineStoryboard : CinemachineExtension
	{
		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (vcam != base.ComponentOwner || stage != CinemachineCore.Stage.Finalize)
			{
				return;
			}
			this.UpdateRenderCanvas();
			if (this.ShowImage)
			{
				state.AddCustomBlendable(new CameraState.CustomBlendableItems.Item
				{
					Custom = this,
					Weight = 1f
				});
			}
			if (this.MuteCamera)
			{
				state.BlendHint |= (CameraState.BlendHints)458752;
			}
		}

		private void UpdateRenderCanvas()
		{
			for (int i = 0; i < this.m_CanvasInfo.Count; i++)
			{
				if (this.m_CanvasInfo[i] == null || this.m_CanvasInfo[i].CanvasComponent == null)
				{
					this.m_CanvasInfo.RemoveAt(i--);
				}
				else
				{
					this.m_CanvasInfo[i].CanvasComponent.renderMode = (RenderMode)this.RenderMode;
					this.m_CanvasInfo[i].CanvasComponent.planeDistance = this.PlaneDistance;
					this.m_CanvasInfo[i].CanvasComponent.sortingOrder = this.SortingOrder;
				}
			}
		}

		protected override void ConnectToVcam(bool connect)
		{
			base.ConnectToVcam(connect);
			CinemachineCore.CameraUpdatedEvent.RemoveListener(new UnityAction<CinemachineBrain>(this.CameraUpdatedCallback));
			if (connect)
			{
				CinemachineCore.CameraUpdatedEvent.AddListener(new UnityAction<CinemachineBrain>(this.CameraUpdatedCallback));
				return;
			}
			this.DestroyCanvas();
		}

		private string CanvasName
		{
			get
			{
				return "_CM_canvas" + base.gameObject.GetInstanceID().ToString();
			}
		}

		private void CameraUpdatedCallback(CinemachineBrain brain)
		{
			CinemachineVirtualCameraBase componentOwner = base.ComponentOwner;
			if (componentOwner == null)
			{
				return;
			}
			bool flag = base.enabled && this.ShowImage && CinemachineCore.IsLive(componentOwner);
			uint outputChannel = (uint)componentOwner.OutputChannel;
			if (CinemachineStoryboard.s_StoryboardGlobalMute || (brain.ChannelMask & (OutputChannels)outputChannel) == (OutputChannels)0)
			{
				flag = false;
			}
			CinemachineStoryboard.CanvasInfo canvasInfo = this.LocateMyCanvas(brain, flag);
			if (canvasInfo != null && canvasInfo.Canvas != null)
			{
				canvasInfo.Canvas.SetActive(flag);
			}
		}

		private CinemachineStoryboard.CanvasInfo LocateMyCanvas(CinemachineBrain parent, bool createIfNotFound)
		{
			CinemachineStoryboard.CanvasInfo canvasInfo = null;
			int num = 0;
			while (canvasInfo == null && num < this.m_CanvasInfo.Count)
			{
				if (this.m_CanvasInfo[num] != null && this.m_CanvasInfo[num].CanvasParent == parent)
				{
					canvasInfo = this.m_CanvasInfo[num];
				}
				num++;
			}
			if (createIfNotFound)
			{
				if (canvasInfo == null)
				{
					canvasInfo = new CinemachineStoryboard.CanvasInfo
					{
						CanvasParent = parent
					};
					int childCount = parent.transform.childCount;
					int num2 = 0;
					while (canvasInfo.Canvas == null && num2 < childCount)
					{
						RectTransform rectTransform = parent.transform.GetChild(num2) as RectTransform;
						if (rectTransform != null && rectTransform.name == this.CanvasName)
						{
							canvasInfo.Canvas = rectTransform.gameObject;
							RectTransform[] componentsInChildren = canvasInfo.Canvas.GetComponentsInChildren<RectTransform>();
							canvasInfo.Viewport = ((componentsInChildren.Length > 1) ? componentsInChildren[1] : null);
							canvasInfo.RawImage = canvasInfo.Canvas.GetComponentInChildren<RawImage>();
							canvasInfo.CanvasComponent = canvasInfo.Canvas.GetComponent<Canvas>();
						}
						num2++;
					}
					this.m_CanvasInfo.Add(canvasInfo);
				}
				if (canvasInfo.Canvas == null || canvasInfo.Viewport == null || canvasInfo.RawImage == null || canvasInfo.CanvasComponent == null)
				{
					this.CreateCanvas(canvasInfo);
				}
			}
			return canvasInfo;
		}

		private void CreateCanvas(CinemachineStoryboard.CanvasInfo ci)
		{
			ci.Canvas = new GameObject(this.CanvasName, new Type[]
			{
				typeof(RectTransform)
			});
			ci.Canvas.layer = base.gameObject.layer;
			ci.Canvas.hideFlags = HideFlags.HideAndDontSave;
			ci.Canvas.transform.SetParent(ci.CanvasParent.transform);
			Canvas canvas = ci.CanvasComponent = ci.Canvas.AddComponent<Canvas>();
			canvas.renderMode = (RenderMode)this.RenderMode;
			canvas.sortingOrder = this.SortingOrder;
			canvas.planeDistance = this.PlaneDistance;
			canvas.worldCamera = ci.CanvasParent.OutputCamera;
			GameObject gameObject = new GameObject("Viewport", new Type[]
			{
				typeof(RectTransform)
			});
			gameObject.transform.SetParent(ci.Canvas.transform);
			ci.Viewport = (RectTransform)gameObject.transform;
			gameObject.AddComponent<RectMask2D>();
			gameObject = new GameObject("RawImage", new Type[]
			{
				typeof(RectTransform)
			});
			gameObject.transform.SetParent(ci.Viewport.transform);
			ci.RawImage = gameObject.AddComponent<RawImage>();
		}

		private void DestroyCanvas()
		{
			int activeBrainCount = CinemachineBrain.ActiveBrainCount;
			for (int i = 0; i < activeBrainCount; i++)
			{
				CinemachineBrain activeBrain = CinemachineBrain.GetActiveBrain(i);
				for (int j = activeBrain.transform.childCount - 1; j >= 0; j--)
				{
					RectTransform rectTransform = activeBrain.transform.GetChild(j) as RectTransform;
					if (rectTransform != null && rectTransform.name == this.CanvasName)
					{
						RuntimeUtility.DestroyObject(rectTransform.gameObject);
					}
				}
			}
			this.m_CanvasInfo.Clear();
		}

		private void PlaceImage(CinemachineStoryboard.CanvasInfo ci, float alpha)
		{
			if (ci.RawImage != null && ci.Viewport != null)
			{
				Rect pixelRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
				if (ci.CanvasParent.OutputCamera != null)
				{
					pixelRect = ci.CanvasParent.OutputCamera.pixelRect;
				}
				pixelRect.x -= (float)Screen.width / 2f;
				pixelRect.y -= (float)Screen.height / 2f;
				float num = -Mathf.Clamp(this.SplitView, -1f, 1f) * pixelRect.width;
				Vector2 center = pixelRect.center;
				center.x -= num / 2f;
				ci.Viewport.localPosition = center;
				ci.Viewport.localRotation = Quaternion.identity;
				ci.Viewport.localScale = Vector3.one;
				ci.Viewport.ForceUpdateRectTransforms();
				ci.Viewport.sizeDelta = new Vector2(pixelRect.width + 1f - Mathf.Abs(num), pixelRect.height + 1f);
				Vector2 one = Vector2.one;
				if (this.Image != null && this.Image.width > 0 && this.Image.width > 0 && pixelRect.width > 0f && pixelRect.height > 0f)
				{
					float num2 = pixelRect.height * (float)this.Image.width / (pixelRect.width * (float)this.Image.height);
					switch (this.Aspect)
					{
					case CinemachineStoryboard.FillStrategy.BestFit:
						if (num2 >= 1f)
						{
							one.y /= num2;
						}
						else
						{
							one.x *= num2;
						}
						break;
					case CinemachineStoryboard.FillStrategy.CropImageToFit:
						if (num2 >= 1f)
						{
							one.x *= num2;
						}
						else
						{
							one.y /= num2;
						}
						break;
					}
				}
				one.x *= this.Scale.x;
				one.y *= (this.SyncScale ? this.Scale.x : this.Scale.y);
				ci.RawImage.texture = this.Image;
				Color white = Color.white;
				white.a = this.Alpha * alpha;
				ci.RawImage.color = white;
				center = new Vector2(pixelRect.width * this.Center.x, pixelRect.height * this.Center.y);
				center.x += num / 2f;
				ci.RawImage.rectTransform.localPosition = center;
				ci.RawImage.rectTransform.localRotation = Quaternion.Euler(this.Rotation);
				ci.RawImage.rectTransform.localScale = one;
				ci.RawImage.rectTransform.ForceUpdateRectTransforms();
				ci.RawImage.rectTransform.sizeDelta = pixelRect.size;
			}
		}

		private static void StaticBlendingHandler(CinemachineBrain brain)
		{
			CameraState state = brain.State;
			int numCustomBlendables = state.GetNumCustomBlendables();
			for (int i = 0; i < numCustomBlendables; i++)
			{
				CameraState.CustomBlendableItems.Item customBlendable = state.GetCustomBlendable(i);
				CinemachineStoryboard cinemachineStoryboard = customBlendable.Custom as CinemachineStoryboard;
				if (cinemachineStoryboard != null && cinemachineStoryboard.ComponentOwner != null)
				{
					bool createIfNotFound = true;
					uint outputChannel = (uint)cinemachineStoryboard.ComponentOwner.OutputChannel;
					if (CinemachineStoryboard.s_StoryboardGlobalMute || (brain.ChannelMask & (OutputChannels)outputChannel) == (OutputChannels)0)
					{
						createIfNotFound = false;
					}
					CinemachineStoryboard.CanvasInfo canvasInfo = cinemachineStoryboard.LocateMyCanvas(brain, createIfNotFound);
					if (canvasInfo != null)
					{
						cinemachineStoryboard.PlaceImage(canvasInfo, customBlendable.Weight);
					}
				}
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void InitializeModule()
		{
			CinemachineCore.CameraUpdatedEvent.RemoveListener(new UnityAction<CinemachineBrain>(CinemachineStoryboard.StaticBlendingHandler));
			CinemachineCore.CameraUpdatedEvent.AddListener(new UnityAction<CinemachineBrain>(CinemachineStoryboard.StaticBlendingHandler));
		}

		[Tooltip("If checked, all storyboards are globally muted")]
		public static bool s_StoryboardGlobalMute;

		[Tooltip("If checked, the specified image will be displayed as an overlay over the virtual camera's output")]
		[FormerlySerializedAs("m_ShowImage")]
		public bool ShowImage = true;

		[Tooltip("The image to display")]
		[FormerlySerializedAs("m_Image")]
		public Texture Image;

		[Tooltip("How to handle differences between image aspect and screen aspect")]
		[FormerlySerializedAs("m_Aspect")]
		public CinemachineStoryboard.FillStrategy Aspect;

		[Tooltip("The opacity of the image.  0 is transparent, 1 is opaque")]
		[FormerlySerializedAs("m_Alpha")]
		[Range(0f, 1f)]
		public float Alpha = 1f;

		[Tooltip("The screen-space position at which to display the image.  Zero is center")]
		[FormerlySerializedAs("m_Center")]
		public Vector2 Center = Vector2.zero;

		[Tooltip("The screen-space rotation to apply to the image")]
		[FormerlySerializedAs("m_Rotation")]
		public Vector3 Rotation = Vector3.zero;

		[Tooltip("The screen-space scaling to apply to the image")]
		[FormerlySerializedAs("m_Scale")]
		public Vector2 Scale = Vector3.one;

		[Tooltip("If checked, X and Y scale are synchronized")]
		[FormerlySerializedAs("m_SyncScale")]
		public bool SyncScale = true;

		[Tooltip("If checked, Camera transform will not be controlled by this virtual camera")]
		[FormerlySerializedAs("m_MuteCamera")]
		public bool MuteCamera;

		[Range(-1f, 1f)]
		[Tooltip("Wipe the image on and off horizontally")]
		[FormerlySerializedAs("m_SplitView")]
		public float SplitView;

		[Tooltip("The render mode of the canvas on which the storyboard is drawn.")]
		[FormerlySerializedAs("m_RenderMode")]
		public CinemachineStoryboard.StoryboardRenderMode RenderMode;

		[Tooltip("Allows ordering canvases to render on top or below other canvases.")]
		[FormerlySerializedAs("m_SortingOrder")]
		public int SortingOrder;

		[Tooltip("How far away from the camera is the Canvas generated.")]
		[FormerlySerializedAs("m_PlaneDistance")]
		public float PlaneDistance = 100f;

		private List<CinemachineStoryboard.CanvasInfo> m_CanvasInfo = new List<CinemachineStoryboard.CanvasInfo>();

		public enum FillStrategy
		{
			BestFit,
			CropImageToFit,
			StretchToFit
		}

		private class CanvasInfo
		{
			public GameObject Canvas;

			public Canvas CanvasComponent;

			public CinemachineBrain CanvasParent;

			public RectTransform Viewport;

			public RawImage RawImage;
		}

		public enum StoryboardRenderMode
		{
			ScreenSpaceOverlay,
			ScreenSpaceCamera
		}
	}
}
