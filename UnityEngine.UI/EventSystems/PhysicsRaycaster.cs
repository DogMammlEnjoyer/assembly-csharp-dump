using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Physics Raycaster")]
	[RequireComponent(typeof(Camera))]
	public class PhysicsRaycaster : BaseRaycaster
	{
		protected PhysicsRaycaster()
		{
		}

		public override Camera eventCamera
		{
			get
			{
				if (this.m_EventCamera == null)
				{
					this.m_EventCamera = base.GetComponent<Camera>();
				}
				if (this.m_EventCamera == null)
				{
					return Camera.main;
				}
				return this.m_EventCamera;
			}
		}

		public virtual int depth
		{
			get
			{
				if (!(this.eventCamera != null))
				{
					return 16777215;
				}
				return (int)this.eventCamera.depth;
			}
		}

		public int finalEventMask
		{
			get
			{
				if (!(this.eventCamera != null))
				{
					return -1;
				}
				return this.eventCamera.cullingMask & this.m_EventMask;
			}
		}

		public LayerMask eventMask
		{
			get
			{
				return this.m_EventMask;
			}
			set
			{
				this.m_EventMask = value;
			}
		}

		public int maxRayIntersections
		{
			get
			{
				return this.m_MaxRayIntersections;
			}
			set
			{
				this.m_MaxRayIntersections = value;
			}
		}

		protected bool ComputeRayAndDistance(PointerEventData eventData, ref Ray ray, ref int eventDisplayIndex, ref float distanceToClipPlane)
		{
			if (this.eventCamera == null)
			{
				return false;
			}
			Vector3 vector = MultipleDisplayUtilities.RelativeMouseAtScaled(eventData.position, eventData.displayIndex);
			if (vector != Vector3.zero)
			{
				eventDisplayIndex = (int)vector.z;
				if (eventDisplayIndex != this.eventCamera.targetDisplay)
				{
					return false;
				}
			}
			else
			{
				vector = eventData.position;
			}
			if (!this.eventCamera.pixelRect.Contains(vector))
			{
				return false;
			}
			ray = this.eventCamera.ScreenPointToRay(vector);
			float z = ray.direction.z;
			distanceToClipPlane = (Mathf.Approximately(0f, z) ? float.PositiveInfinity : Mathf.Abs((this.eventCamera.farClipPlane - this.eventCamera.nearClipPlane) / z));
			return true;
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			Ray r = default(Ray);
			int displayIndex = 0;
			float f = 0f;
			if (!this.ComputeRayAndDistance(eventData, ref r, ref displayIndex, ref f))
			{
				return;
			}
			int num;
			if (this.m_MaxRayIntersections == 0)
			{
				if (ReflectionMethodsCache.Singleton.raycast3DAll == null)
				{
					return;
				}
				this.m_Hits = ReflectionMethodsCache.Singleton.raycast3DAll(r, f, this.finalEventMask);
				num = this.m_Hits.Length;
			}
			else
			{
				if (ReflectionMethodsCache.Singleton.getRaycastNonAlloc == null)
				{
					return;
				}
				if (this.m_LastMaxRayIntersections != this.m_MaxRayIntersections)
				{
					this.m_Hits = new RaycastHit[this.m_MaxRayIntersections];
					this.m_LastMaxRayIntersections = this.m_MaxRayIntersections;
				}
				num = ReflectionMethodsCache.Singleton.getRaycastNonAlloc(r, this.m_Hits, f, this.finalEventMask);
			}
			if (num != 0)
			{
				if (num > 1)
				{
					Array.Sort<RaycastHit>(this.m_Hits, 0, num, PhysicsRaycaster.RaycastHitComparer.instance);
				}
				int i = 0;
				int num2 = num;
				while (i < num2)
				{
					RaycastResult item = new RaycastResult
					{
						gameObject = this.m_Hits[i].collider.gameObject,
						module = this,
						distance = this.m_Hits[i].distance,
						worldPosition = this.m_Hits[i].point,
						worldNormal = this.m_Hits[i].normal,
						screenPosition = eventData.position,
						displayIndex = displayIndex,
						index = (float)resultAppendList.Count,
						sortingLayer = 0,
						sortingOrder = 0
					};
					resultAppendList.Add(item);
					i++;
				}
			}
		}

		protected const int kNoEventMaskSet = -1;

		protected Camera m_EventCamera;

		[SerializeField]
		protected LayerMask m_EventMask = -1;

		[SerializeField]
		protected int m_MaxRayIntersections;

		protected int m_LastMaxRayIntersections;

		private RaycastHit[] m_Hits;

		private class RaycastHitComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit x, RaycastHit y)
			{
				return x.distance.CompareTo(y.distance);
			}

			public static PhysicsRaycaster.RaycastHitComparer instance = new PhysicsRaycaster.RaycastHitComparer();
		}
	}
}
