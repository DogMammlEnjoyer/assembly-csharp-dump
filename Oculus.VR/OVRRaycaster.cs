using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[HelpURL("https://developer.oculus.com/documentation/unity/unity-dronerage-example-scenes/")]
public class OVRRaycaster : GraphicRaycaster, IPointerEnterHandler, IEventSystemHandler
{
	protected OVRRaycaster()
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
			this.m_RayTransformer = base.GetComponent<OVRRayTransformer>();
			return this.m_Canvas;
		}
	}

	private OVRRayTransformer rayTransformer
	{
		get
		{
			return this.m_RayTransformer;
		}
	}

	public override Camera eventCamera
	{
		get
		{
			return this.canvas.worldCamera;
		}
	}

	public override int sortOrderPriority
	{
		get
		{
			return this.sortOrder;
		}
	}

	protected override void Start()
	{
		if (!this.canvas.worldCamera)
		{
			Debug.Log("Canvas does not have an event camera attached. Attaching OVRCameraRig.centerEyeAnchor as default.");
			OVRCameraRig ovrcameraRig = Object.FindAnyObjectByType<OVRCameraRig>();
			if (ovrcameraRig)
			{
				this.canvas.worldCamera = ovrcameraRig.centerEyeAnchor.gameObject.GetComponent<Camera>();
			}
		}
	}

	private void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList, Ray ray, bool checkForBlocking, bool checkOnlyRaycastable)
	{
		if (this.canvas == null)
		{
			return;
		}
		float num = float.MaxValue;
		if (checkForBlocking && base.blockingObjects != GraphicRaycaster.BlockingObjects.None)
		{
			float farClipPlane = this.eventCamera.farClipPlane;
			if (base.blockingObjects == GraphicRaycaster.BlockingObjects.ThreeD || base.blockingObjects == GraphicRaycaster.BlockingObjects.All)
			{
				UnityEngine.RaycastHit[] array = Physics.RaycastAll(ray, farClipPlane, this.m_BlockingMask);
				if (array.Length != 0 && array[0].distance < num)
				{
					num = array[0].distance;
				}
			}
			if (base.blockingObjects == GraphicRaycaster.BlockingObjects.TwoD || base.blockingObjects == GraphicRaycaster.BlockingObjects.All)
			{
				RaycastHit2D[] rayIntersectionAll = Physics2D.GetRayIntersectionAll(ray, farClipPlane, this.m_BlockingMask);
				if (rayIntersectionAll.Length != 0 && rayIntersectionAll[0].fraction * farClipPlane < num)
				{
					num = rayIntersectionAll[0].fraction * farClipPlane;
				}
			}
		}
		this.m_RaycastResults.Clear();
		this.GraphicRaycast(this.canvas, this.rayTransformer, ray, this.m_RaycastResults, checkOnlyRaycastable);
		for (int i = 0; i < this.m_RaycastResults.Count; i++)
		{
			GameObject gameObject = this.m_RaycastResults[i].graphic.gameObject;
			bool flag = true;
			if (base.ignoreReversedGraphics)
			{
				Vector3 direction = ray.direction;
				Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
				flag = (Vector3.Dot(direction, rhs) > 0f);
			}
			if (this.eventCamera.transform.InverseTransformPoint(this.m_RaycastResults[i].worldPos).z <= 0f)
			{
				flag = false;
			}
			if (flag)
			{
				float num2 = Vector3.Distance(ray.origin, this.m_RaycastResults[i].worldPos);
				if (num2 < num)
				{
					RaycastResult item = new RaycastResult
					{
						gameObject = gameObject,
						module = this,
						distance = num2,
						index = (float)resultAppendList.Count,
						depth = this.m_RaycastResults[i].graphic.depth,
						worldPosition = this.m_RaycastResults[i].worldPos
					};
					resultAppendList.Add(item);
				}
			}
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (eventData.IsVRPointer())
		{
			this.Raycast(eventData, resultAppendList, eventData.GetRay(), true, false);
		}
	}

	internal void RaycastOnRaycastableGraphics(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (eventData.IsVRPointer())
		{
			this.Raycast(eventData, resultAppendList, eventData.GetRay(), false, true);
		}
	}

	public void RaycastPointer(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		if (this.pointer != null && this.pointer.activeInHierarchy)
		{
			this.Raycast(eventData, resultAppendList, new Ray(this.eventCamera.transform.position, (this.pointer.transform.position - this.eventCamera.transform.position).normalized), false, false);
		}
	}

	private void GraphicRaycast(Canvas canvas, OVRRayTransformer rayTransformer, Ray ray, List<OVRRaycaster.RaycastHit> results, bool checkOnlyRaycastableGraphics)
	{
		if (rayTransformer != null)
		{
			ray = rayTransformer.TransformRay(ray);
		}
		IList<Graphic> list = checkOnlyRaycastableGraphics ? GraphicRegistry.GetRaycastableGraphicsForCanvas(canvas) : GraphicRegistry.GetGraphicsForCanvas(canvas);
		OVRRaycaster.s_SortedGraphics.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			Graphic graphic = list[i];
			Vector3 vector;
			if (graphic.depth != -1 && !(this.pointer == graphic.gameObject) && OVRRaycaster.RayIntersectsRectTransform(graphic.rectTransform, ray, out vector))
			{
				Vector2 sp = this.eventCamera.WorldToScreenPoint(vector);
				if (graphic.Raycast(sp, this.eventCamera))
				{
					OVRRaycaster.RaycastHit item;
					item.graphic = graphic;
					item.worldPos = vector;
					item.fromMouse = false;
					OVRRaycaster.s_SortedGraphics.Add(item);
				}
			}
		}
		OVRRaycaster.s_SortedGraphics.Sort((OVRRaycaster.RaycastHit g1, OVRRaycaster.RaycastHit g2) => g2.graphic.depth.CompareTo(g1.graphic.depth));
		for (int j = 0; j < OVRRaycaster.s_SortedGraphics.Count; j++)
		{
			results.Add(OVRRaycaster.s_SortedGraphics[j]);
		}
	}

	public Vector2 GetScreenPosition(RaycastResult raycastResult)
	{
		return this.eventCamera.WorldToScreenPoint(raycastResult.worldPosition);
	}

	private static bool RayIntersectsRectTransform(RectTransform rectTransform, Ray ray, out Vector3 worldPos)
	{
		rectTransform.GetWorldCorners(OVRRaycaster._corners);
		Plane plane = new Plane(OVRRaycaster._corners[0], OVRRaycaster._corners[1], OVRRaycaster._corners[2]);
		float distance;
		if (!plane.Raycast(ray, out distance))
		{
			worldPos = Vector3.zero;
			return false;
		}
		Vector3 point = ray.GetPoint(distance);
		Vector3 vector = OVRRaycaster._corners[3] - OVRRaycaster._corners[0];
		Vector3 vector2 = OVRRaycaster._corners[1] - OVRRaycaster._corners[0];
		float num = Vector3.Dot(point - OVRRaycaster._corners[0], vector);
		float num2 = Vector3.Dot(point - OVRRaycaster._corners[0], vector2);
		if (num < vector.sqrMagnitude && num2 < vector2.sqrMagnitude && num >= 0f && num2 >= 0f)
		{
			worldPos = OVRRaycaster._corners[0] + num2 * vector2 / vector2.sqrMagnitude + num * vector / vector.sqrMagnitude;
			return true;
		}
		worldPos = Vector3.zero;
		return false;
	}

	public virtual bool IsFocussed()
	{
		OVRInputModule ovrinputModule = EventSystem.current.currentInputModule as OVRInputModule;
		return ovrinputModule && ovrinputModule.activeGraphicRaycaster == this;
	}

	public virtual void OnPointerEnter(PointerEventData e)
	{
		if (e.IsVRPointer())
		{
			OVRInputModule ovrinputModule = EventSystem.current.currentInputModule as OVRInputModule;
			if (ovrinputModule != null)
			{
				ovrinputModule.activeGraphicRaycaster = this;
			}
		}
	}

	[Tooltip("A world space pointer for this canvas")]
	public GameObject pointer;

	public int sortOrder;

	[NonSerialized]
	private Canvas m_Canvas;

	[NonSerialized]
	private OVRRayTransformer m_RayTransformer;

	[NonSerialized]
	private List<OVRRaycaster.RaycastHit> m_RaycastResults = new List<OVRRaycaster.RaycastHit>();

	[NonSerialized]
	private static readonly List<OVRRaycaster.RaycastHit> s_SortedGraphics = new List<OVRRaycaster.RaycastHit>();

	private static readonly Vector3[] _corners = new Vector3[4];

	private struct RaycastHit
	{
		public Graphic graphic;

		public Vector3 worldPos;

		public bool fromMouse;
	}
}
