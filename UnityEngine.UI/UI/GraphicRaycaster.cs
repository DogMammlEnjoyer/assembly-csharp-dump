using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("Event/Graphic Raycaster")]
	[RequireComponent(typeof(Canvas))]
	public class GraphicRaycaster : BaseRaycaster
	{
		public override int sortOrderPriority
		{
			get
			{
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					return this.canvas.sortingOrder;
				}
				return base.sortOrderPriority;
			}
		}

		public override int renderOrderPriority
		{
			get
			{
				if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					return this.canvas.rootCanvas.renderOrder;
				}
				return base.renderOrderPriority;
			}
		}

		public bool ignoreReversedGraphics
		{
			get
			{
				return this.m_IgnoreReversedGraphics;
			}
			set
			{
				this.m_IgnoreReversedGraphics = value;
			}
		}

		public GraphicRaycaster.BlockingObjects blockingObjects
		{
			get
			{
				return this.m_BlockingObjects;
			}
			set
			{
				this.m_BlockingObjects = value;
			}
		}

		public LayerMask blockingMask
		{
			get
			{
				return this.m_BlockingMask;
			}
			set
			{
				this.m_BlockingMask = value;
			}
		}

		protected GraphicRaycaster()
		{
		}

		private Canvas canvas
		{
			get
			{
				if (this.m_Canvas != null)
				{
					return this.m_Canvas;
				}
				this.m_Canvas = base.GetComponent<Canvas>();
				return this.m_Canvas;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (this.canvas == null)
			{
				return;
			}
			IList<Graphic> raycastableGraphicsForCanvas = GraphicRegistry.GetRaycastableGraphicsForCanvas(this.canvas);
			if (raycastableGraphicsForCanvas == null || raycastableGraphicsForCanvas.Count == 0)
			{
				return;
			}
			Camera eventCamera = this.eventCamera;
			int targetDisplay;
			if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay || eventCamera == null)
			{
				targetDisplay = this.canvas.targetDisplay;
			}
			else
			{
				targetDisplay = eventCamera.targetDisplay;
			}
			Vector3 relativeMousePositionForRaycast = MultipleDisplayUtilities.GetRelativeMousePositionForRaycast(eventData);
			if ((int)relativeMousePositionForRaycast.z != targetDisplay)
			{
				return;
			}
			Vector2 vector;
			if (eventCamera == null)
			{
				float num = (float)Screen.width;
				float num2 = (float)Screen.height;
				if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
				{
					num = (float)Display.displays[targetDisplay].systemWidth;
					num2 = (float)Display.displays[targetDisplay].systemHeight;
				}
				vector = new Vector2(relativeMousePositionForRaycast.x / num, relativeMousePositionForRaycast.y / num2);
			}
			else
			{
				vector = eventCamera.ScreenToViewportPoint(relativeMousePositionForRaycast);
			}
			if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
			{
				return;
			}
			float num3 = float.MaxValue;
			Ray r = default(Ray);
			if (eventCamera != null)
			{
				r = eventCamera.ScreenPointToRay(relativeMousePositionForRaycast);
			}
			if (this.canvas.renderMode != RenderMode.ScreenSpaceOverlay && this.blockingObjects != GraphicRaycaster.BlockingObjects.None)
			{
				float f = 100f;
				if (eventCamera != null)
				{
					float z = r.direction.z;
					f = (Mathf.Approximately(0f, z) ? float.PositiveInfinity : Mathf.Abs((eventCamera.farClipPlane - eventCamera.nearClipPlane) / z));
				}
				RaycastHit raycastHit;
				if ((this.blockingObjects == GraphicRaycaster.BlockingObjects.ThreeD || this.blockingObjects == GraphicRaycaster.BlockingObjects.All) && ReflectionMethodsCache.Singleton.raycast3D != null && ReflectionMethodsCache.Singleton.raycast3D(r, out raycastHit, f, this.m_BlockingMask))
				{
					num3 = raycastHit.distance;
				}
				if ((this.blockingObjects == GraphicRaycaster.BlockingObjects.TwoD || this.blockingObjects == GraphicRaycaster.BlockingObjects.All) && ReflectionMethodsCache.Singleton.raycast2D != null)
				{
					RaycastHit2D[] array = ReflectionMethodsCache.Singleton.getRayIntersectionAll(r, f, this.m_BlockingMask);
					if (array.Length != 0)
					{
						num3 = array[0].distance;
					}
				}
			}
			this.m_RaycastResults.Clear();
			GraphicRaycaster.Raycast(this.canvas, eventCamera, relativeMousePositionForRaycast, raycastableGraphicsForCanvas, this.m_RaycastResults);
			int count = this.m_RaycastResults.Count;
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = this.m_RaycastResults[i].gameObject;
				bool flag = true;
				if (this.ignoreReversedGraphics)
				{
					if (eventCamera == null)
					{
						Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
						flag = (Vector3.Dot(Vector3.forward, rhs) > 0f);
					}
					else
					{
						Vector3 b = eventCamera.transform.rotation * Vector3.forward * eventCamera.nearClipPlane;
						flag = (Vector3.Dot(gameObject.transform.position - eventCamera.transform.position - b, gameObject.transform.forward) >= 0f);
					}
				}
				if (flag)
				{
					Transform transform = gameObject.transform;
					Vector3 forward = transform.forward;
					float num4;
					if (eventCamera == null || this.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
					{
						num4 = 0f;
					}
					else
					{
						num4 = Vector3.Dot(forward, transform.position - r.origin) / Vector3.Dot(forward, r.direction);
						if (num4 < 0f)
						{
							goto IL_464;
						}
					}
					if (num4 < num3)
					{
						RaycastResult item = new RaycastResult
						{
							gameObject = gameObject,
							module = this,
							distance = num4,
							screenPosition = relativeMousePositionForRaycast,
							displayIndex = targetDisplay,
							index = (float)resultAppendList.Count,
							depth = this.m_RaycastResults[i].depth,
							sortingLayer = this.canvas.sortingLayerID,
							sortingOrder = this.canvas.sortingOrder,
							worldPosition = r.origin + r.direction * num4,
							worldNormal = -forward
						};
						resultAppendList.Add(item);
					}
				}
				IL_464:;
			}
		}

		public override Camera eventCamera
		{
			get
			{
				Canvas canvas = this.canvas;
				RenderMode renderMode = canvas.renderMode;
				if (renderMode == RenderMode.ScreenSpaceOverlay || (renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null))
				{
					return null;
				}
				return canvas.worldCamera ?? Camera.main;
			}
		}

		private static void Raycast(Canvas canvas, Camera eventCamera, Vector2 pointerPosition, IList<Graphic> foundGraphics, List<Graphic> results)
		{
			int count = foundGraphics.Count;
			for (int i = 0; i < count; i++)
			{
				Graphic graphic = foundGraphics[i];
				if (graphic.raycastTarget && !graphic.canvasRenderer.cull && graphic.depth != -1 && RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, pointerPosition, eventCamera, graphic.raycastPadding) && (!(eventCamera != null) || eventCamera.WorldToScreenPoint(graphic.rectTransform.position).z <= eventCamera.farClipPlane) && graphic.Raycast(pointerPosition, eventCamera))
				{
					GraphicRaycaster.s_SortedGraphics.Add(graphic);
				}
			}
			GraphicRaycaster.s_SortedGraphics.Sort((Graphic g1, Graphic g2) => g2.depth.CompareTo(g1.depth));
			count = GraphicRaycaster.s_SortedGraphics.Count;
			for (int j = 0; j < count; j++)
			{
				results.Add(GraphicRaycaster.s_SortedGraphics[j]);
			}
			GraphicRaycaster.s_SortedGraphics.Clear();
		}

		protected const int kNoEventMaskSet = -1;

		[FormerlySerializedAs("ignoreReversedGraphics")]
		[SerializeField]
		private bool m_IgnoreReversedGraphics = true;

		[FormerlySerializedAs("blockingObjects")]
		[SerializeField]
		private GraphicRaycaster.BlockingObjects m_BlockingObjects;

		[SerializeField]
		protected LayerMask m_BlockingMask = -1;

		private Canvas m_Canvas;

		[NonSerialized]
		private List<Graphic> m_RaycastResults = new List<Graphic>();

		[NonSerialized]
		private static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();

		public enum BlockingObjects
		{
			None,
			TwoD,
			ThreeD,
			All
		}
	}
}
