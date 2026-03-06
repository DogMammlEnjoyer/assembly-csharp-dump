using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	[AddComponentMenu("Event/Tracked Device Physics Raycaster", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.UI.TrackedDevicePhysicsRaycaster.html")]
	public class TrackedDevicePhysicsRaycaster : BaseRaycaster
	{
		public QueryTriggerInteraction raycastTriggerInteraction
		{
			get
			{
				return this.m_RaycastTriggerInteraction;
			}
			set
			{
				this.m_RaycastTriggerInteraction = value;
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
				this.m_MaxRayIntersections = Math.Max(value, 1);
			}
		}

		public override Camera eventCamera
		{
			get
			{
				if (this.m_EventCamera == null)
				{
					this.m_EventCamera = base.GetComponent<Camera>();
				}
				if (!(this.m_EventCamera != null))
				{
					return Camera.main;
				}
				return this.m_EventCamera;
			}
		}

		public void SetEventCamera(Camera newEventCamera)
		{
			this.m_EventCamera = newEventCamera;
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			TrackedDeviceEventData trackedDeviceEventData = eventData as TrackedDeviceEventData;
			if (trackedDeviceEventData != null)
			{
				this.PerformRaycasts(trackedDeviceEventData, resultAppendList);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			this.m_RaycastHits = new RaycastHit[this.m_MaxRayIntersections];
			this.m_RaycastArrayWrapper = new TrackedDevicePhysicsRaycaster.RaycastHitArraySegment(this.m_RaycastHits, 0);
		}

		private void PerformRaycasts(TrackedDeviceEventData eventData, List<RaycastResult> resultAppendList)
		{
			Camera eventCamera = this.eventCamera;
			if (eventCamera == null)
			{
				if (!this.m_HasWarnedEventCameraNull)
				{
					Debug.LogWarning("Event Camera must be set on TrackedDevicePhysicsRaycaster to determine screen space coordinates. UI events will not function correctly until it is set.", this);
					this.m_HasWarnedEventCameraNull = true;
				}
				return;
			}
			List<Vector3> rayPoints = eventData.rayPoints;
			int intVal = eventData.layerMask & this.m_EventMask;
			float num = 0f;
			for (int i = 1; i < rayPoints.Count; i++)
			{
				Vector3 from = rayPoints[i - 1];
				Vector3 to = rayPoints[i];
				if (this.PerformRaycast(from, to, intVal, eventCamera, resultAppendList, ref num))
				{
					eventData.rayHitIndex = i;
					return;
				}
			}
		}

		private bool PerformRaycast(Vector3 from, Vector3 to, LayerMask layerMask, Camera currentEventCamera, List<RaycastResult> resultAppendList, ref float existingHitLength)
		{
			bool result = false;
			float num = Vector3.Distance(to, from);
			Ray ray = new Ray(from, (to - from).normalized * num);
			float num2 = num;
			this.m_MaxRayIntersections = Math.Max(this.m_MaxRayIntersections, 1);
			if (this.m_RaycastHits.Length != this.m_MaxRayIntersections)
			{
				Array.Resize<RaycastHit>(ref this.m_RaycastHits, this.m_MaxRayIntersections);
			}
			int count = this.m_LocalPhysicsScene.Raycast(ray.origin, ray.direction, this.m_RaycastHits, num2, layerMask, this.m_RaycastTriggerInteraction);
			this.m_RaycastArrayWrapper.count = count;
			this.m_RaycastResultsCache.Clear();
			this.m_RaycastResultsCache.AddRange(this.m_RaycastArrayWrapper);
			SortingHelpers.Sort<RaycastHit>(this.m_RaycastResultsCache, this.m_RaycastHitComparer);
			foreach (RaycastHit raycastHit in this.m_RaycastResultsCache)
			{
				GameObject gameObject = raycastHit.collider.gameObject;
				Vector2 vector = currentEventCamera.WorldToScreenPoint(raycastHit.point);
				int displayIndex = (int)Display.RelativeMouseAt(vector).z;
				if (raycastHit.distance <= num2)
				{
					RaycastResult item = new RaycastResult
					{
						gameObject = gameObject,
						module = this,
						distance = existingHitLength + raycastHit.distance,
						index = (float)resultAppendList.Count,
						depth = 0,
						sortingLayer = 0,
						sortingOrder = 0,
						worldPosition = raycastHit.point,
						worldNormal = raycastHit.normal,
						screenPosition = vector,
						displayIndex = displayIndex
					};
					resultAppendList.Add(item);
					result = true;
				}
			}
			existingHitLength += num2;
			return result;
		}

		private const int k_EverythingLayerMask = -1;

		[SerializeField]
		[Tooltip("Specifies whether the ray cast should hit triggers. Use Global refers to the Queries Hit Triggers setting in Physics Project Settings.")]
		private QueryTriggerInteraction m_RaycastTriggerInteraction = QueryTriggerInteraction.Ignore;

		[SerializeField]
		[Tooltip("Layer mask used to filter events. Always combined with the ray cast mask of the UI interactor.")]
		private LayerMask m_EventMask = -1;

		[SerializeField]
		[Tooltip("The max number of intersections allowed. Value will be clamped to greater than 0.")]
		private int m_MaxRayIntersections = 10;

		[SerializeField]
		[Tooltip("The event camera for this ray caster. The event camera is used to determine the screen position and display of the ray cast results.")]
		private Camera m_EventCamera;

		private bool m_HasWarnedEventCameraNull;

		private RaycastHit[] m_RaycastHits;

		private readonly TrackedDevicePhysicsRaycaster.RaycastHitComparer m_RaycastHitComparer = new TrackedDevicePhysicsRaycaster.RaycastHitComparer();

		private TrackedDevicePhysicsRaycaster.RaycastHitArraySegment m_RaycastArrayWrapper;

		private readonly List<RaycastHit> m_RaycastResultsCache = new List<RaycastHit>();

		private PhysicsScene m_LocalPhysicsScene;

		private class RaycastHitArraySegment : IEnumerable<RaycastHit>, IEnumerable, IEnumerator<RaycastHit>, IEnumerator, IDisposable
		{
			public int count
			{
				get
				{
					return this.m_Count;
				}
				set
				{
					this.m_Count = value;
				}
			}

			public RaycastHit Current
			{
				get
				{
					return this.m_Hits[this.m_CurrentIndex];
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public RaycastHitArraySegment(RaycastHit[] raycastHits, int count)
			{
				this.m_Hits = raycastHits;
				this.m_Count = count;
			}

			public bool MoveNext()
			{
				this.m_CurrentIndex++;
				return this.m_CurrentIndex < this.m_Count;
			}

			public void Reset()
			{
				this.m_CurrentIndex = -1;
			}

			public void Dispose()
			{
			}

			public IEnumerator<RaycastHit> GetEnumerator()
			{
				this.Reset();
				return this;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private int m_Count;

			private readonly RaycastHit[] m_Hits;

			private int m_CurrentIndex;
		}

		private sealed class RaycastHitComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit a, RaycastHit b)
			{
				return a.distance.CompareTo(b.distance);
			}
		}
	}
}
