using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.UI
{
	[AddComponentMenu("Event/Tracked Device Raycaster")]
	[RequireComponent(typeof(Canvas))]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/TrackedInputDevices.html#tracked-device-raycaster")]
	public class TrackedDeviceRaycaster : BaseRaycaster
	{
		public override Camera eventCamera
		{
			get
			{
				Canvas canvas = this.canvas;
				if (!(canvas != null))
				{
					return null;
				}
				return canvas.worldCamera;
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

		public bool checkFor3DOcclusion
		{
			get
			{
				return this.m_CheckFor3DOcclusion;
			}
			set
			{
				this.m_CheckFor3DOcclusion = value;
			}
		}

		public bool checkFor2DOcclusion
		{
			get
			{
				return this.m_CheckFor2DOcclusion;
			}
			set
			{
				this.m_CheckFor2DOcclusion = value;
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

		public float maxDistance
		{
			get
			{
				return this.m_MaxDistance;
			}
			set
			{
				this.m_MaxDistance = value;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			TrackedDeviceRaycaster.s_Instances.AppendWithCapacity(this, 10);
		}

		protected override void OnDisable()
		{
			int num = TrackedDeviceRaycaster.s_Instances.IndexOfReference(this);
			if (num != -1)
			{
				TrackedDeviceRaycaster.s_Instances.RemoveAtByMovingTailWithCapacity(num);
			}
			base.OnDisable();
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			ExtendedPointerEventData extendedPointerEventData = eventData as ExtendedPointerEventData;
			if (extendedPointerEventData != null && extendedPointerEventData.pointerType == UIPointerType.Tracked)
			{
				this.PerformRaycast(extendedPointerEventData, resultAppendList);
			}
		}

		internal void PerformRaycast(ExtendedPointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			if (this.canvas == null)
			{
				return;
			}
			if (this.eventCamera == null)
			{
				return;
			}
			Ray ray = new Ray(eventData.trackedDevicePosition, eventData.trackedDeviceOrientation * Vector3.forward);
			float num = this.m_MaxDistance;
			RaycastHit raycastHit;
			if (this.m_CheckFor3DOcclusion && Physics.Raycast(ray, out raycastHit, num, this.m_BlockingMask))
			{
				num = raycastHit.distance;
			}
			if (this.m_CheckFor2DOcclusion)
			{
				float distance = num;
				RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(ray, distance, this.m_BlockingMask);
				if (rayIntersection.collider != null)
				{
					num = rayIntersection.distance;
				}
			}
			this.m_RaycastResultsCache.Clear();
			this.SortedRaycastGraphics(this.canvas, ray, this.m_RaycastResultsCache);
			for (int i = 0; i < this.m_RaycastResultsCache.Count; i++)
			{
				bool flag = true;
				TrackedDeviceRaycaster.RaycastHitData raycastHitData = this.m_RaycastResultsCache[i];
				GameObject gameObject = raycastHitData.graphic.gameObject;
				if (this.m_IgnoreReversedGraphics)
				{
					Vector3 direction = ray.direction;
					Vector3 rhs = gameObject.transform.rotation * Vector3.forward;
					flag = (Vector3.Dot(direction, rhs) > 0f);
				}
				flag &= (raycastHitData.distance < num);
				if (flag)
				{
					RaycastResult item = new RaycastResult
					{
						gameObject = gameObject,
						module = this,
						distance = raycastHitData.distance,
						index = (float)resultAppendList.Count,
						depth = raycastHitData.graphic.depth,
						worldPosition = raycastHitData.worldHitPosition,
						screenPosition = raycastHitData.screenPosition
					};
					resultAppendList.Add(item);
				}
			}
		}

		private void SortedRaycastGraphics(Canvas canvas, Ray ray, List<TrackedDeviceRaycaster.RaycastHitData> results)
		{
			IList<Graphic> graphicsForCanvas = GraphicRegistry.GetGraphicsForCanvas(canvas);
			TrackedDeviceRaycaster.s_SortedGraphics.Clear();
			for (int i = 0; i < graphicsForCanvas.Count; i++)
			{
				Graphic graphic = graphicsForCanvas[i];
				Vector3 vector;
				float distance;
				if (graphic.depth != -1 && TrackedDeviceRaycaster.RayIntersectsRectTransform(graphic.rectTransform, ray, out vector, out distance))
				{
					Vector2 vector2 = this.eventCamera.WorldToScreenPoint(vector);
					if (graphic.Raycast(vector2, this.eventCamera))
					{
						TrackedDeviceRaycaster.s_SortedGraphics.Add(new TrackedDeviceRaycaster.RaycastHitData(graphic, vector, vector2, distance));
					}
				}
			}
			TrackedDeviceRaycaster.s_SortedGraphics.Sort((TrackedDeviceRaycaster.RaycastHitData g1, TrackedDeviceRaycaster.RaycastHitData g2) => g2.graphic.depth.CompareTo(g1.graphic.depth));
			results.AddRange(TrackedDeviceRaycaster.s_SortedGraphics);
		}

		private static bool RayIntersectsRectTransform(RectTransform transform, Ray ray, out Vector3 worldPosition, out float distance)
		{
			Vector3[] array = new Vector3[4];
			transform.GetWorldCorners(array);
			Plane plane = new Plane(array[0], array[1], array[2]);
			float num;
			if (plane.Raycast(ray, out num))
			{
				Vector3 point = ray.GetPoint(num);
				Vector3 rhs = array[3] - array[0];
				Vector3 rhs2 = array[1] - array[0];
				float num2 = Vector3.Dot(point - array[0], rhs);
				if (Vector3.Dot(point - array[0], rhs2) >= 0f && num2 >= 0f)
				{
					Vector3 rhs3 = array[1] - array[2];
					Vector3 rhs4 = array[3] - array[2];
					float num3 = Vector3.Dot(point - array[2], rhs3);
					float num4 = Vector3.Dot(point - array[2], rhs4);
					if (num3 >= 0f && num4 >= 0f)
					{
						worldPosition = point;
						distance = num;
						return true;
					}
				}
			}
			worldPosition = Vector3.zero;
			distance = 0f;
			return false;
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

		[NonSerialized]
		private List<TrackedDeviceRaycaster.RaycastHitData> m_RaycastResultsCache = new List<TrackedDeviceRaycaster.RaycastHitData>();

		internal static InlinedArray<TrackedDeviceRaycaster> s_Instances;

		private static readonly List<TrackedDeviceRaycaster.RaycastHitData> s_SortedGraphics = new List<TrackedDeviceRaycaster.RaycastHitData>();

		[FormerlySerializedAs("ignoreReversedGraphics")]
		[SerializeField]
		private bool m_IgnoreReversedGraphics;

		[FormerlySerializedAs("checkFor2DOcclusion")]
		[SerializeField]
		private bool m_CheckFor2DOcclusion;

		[FormerlySerializedAs("checkFor3DOcclusion")]
		[SerializeField]
		private bool m_CheckFor3DOcclusion;

		[Tooltip("Maximum distance (in 3D world space) that rays are traced to find a hit.")]
		[SerializeField]
		private float m_MaxDistance = 1000f;

		[SerializeField]
		private LayerMask m_BlockingMask;

		[NonSerialized]
		private Canvas m_Canvas;

		private struct RaycastHitData
		{
			public RaycastHitData(Graphic graphic, Vector3 worldHitPosition, Vector2 screenPosition, float distance)
			{
				this.graphic = graphic;
				this.worldHitPosition = worldHitPosition;
				this.screenPosition = screenPosition;
				this.distance = distance;
			}

			public readonly Graphic graphic { get; }

			public readonly Vector3 worldHitPosition { get; }

			public readonly Vector2 screenPosition { get; }

			public readonly float distance { get; }
		}
	}
}
