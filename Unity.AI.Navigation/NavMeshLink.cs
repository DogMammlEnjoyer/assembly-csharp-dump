using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation
{
	[ExecuteAlways]
	[DefaultExecutionOrder(-101)]
	[AddComponentMenu("Navigation/NavMesh Link", 33)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ai.navigation@2.0/manual/NavMeshLink.html")]
	public class NavMeshLink : MonoBehaviour
	{
		public int agentTypeID
		{
			get
			{
				return this.m_AgentTypeID;
			}
			set
			{
				if (value == this.m_AgentTypeID)
				{
					return;
				}
				this.m_AgentTypeID = value;
				this.UpdateLink();
			}
		}

		public Vector3 startPoint
		{
			get
			{
				return this.m_StartPoint;
			}
			set
			{
				if (value == this.m_StartPoint)
				{
					return;
				}
				this.m_StartPoint = value;
				this.UpdateLink();
			}
		}

		public Vector3 endPoint
		{
			get
			{
				return this.m_EndPoint;
			}
			set
			{
				if (value == this.m_EndPoint)
				{
					return;
				}
				this.m_EndPoint = value;
				this.UpdateLink();
			}
		}

		public Transform startTransform
		{
			get
			{
				return this.m_StartTransform;
			}
			set
			{
				if (value == this.m_StartTransform)
				{
					return;
				}
				this.m_StartTransform = value;
				this.UpdateLink();
			}
		}

		public Transform endTransform
		{
			get
			{
				return this.m_EndTransform;
			}
			set
			{
				if (value == this.m_EndTransform)
				{
					return;
				}
				this.m_EndTransform = value;
				this.UpdateLink();
			}
		}

		public float width
		{
			get
			{
				return this.m_Width;
			}
			set
			{
				if (value.Equals(this.m_Width))
				{
					return;
				}
				this.m_Width = value;
				this.UpdateLink();
			}
		}

		public float costModifier
		{
			get
			{
				if (!this.m_IsOverridingCost)
				{
					return -this.m_CostModifier;
				}
				return this.m_CostModifier;
			}
			set
			{
				bool flag = value >= 0f;
				if (value.Equals(this.costModifier) && flag == this.m_IsOverridingCost)
				{
					return;
				}
				this.m_IsOverridingCost = flag;
				this.m_CostModifier = Mathf.Abs(value);
				this.UpdateLink();
			}
		}

		public bool bidirectional
		{
			get
			{
				return this.m_Bidirectional;
			}
			set
			{
				if (value == this.m_Bidirectional)
				{
					return;
				}
				this.m_Bidirectional = value;
				this.UpdateLink();
			}
		}

		public bool autoUpdate
		{
			get
			{
				return this.m_AutoUpdatePosition;
			}
			set
			{
				if (value == this.m_AutoUpdatePosition)
				{
					return;
				}
				this.m_AutoUpdatePosition = value;
				if (this.m_AutoUpdatePosition)
				{
					NavMeshLink.AddTracking(this);
					return;
				}
				NavMeshLink.RemoveTracking(this);
			}
		}

		public int area
		{
			get
			{
				return this.m_Area;
			}
			set
			{
				if (value == this.m_Area)
				{
					return;
				}
				this.m_Area = value;
				this.UpdateLink();
			}
		}

		public bool activated
		{
			get
			{
				return this.m_Activated;
			}
			set
			{
				this.m_Activated = value;
				NavMesh.SetLinkActive(this.m_LinkInstance, this.m_Activated);
			}
		}

		public bool occupied
		{
			get
			{
				return NavMesh.IsLinkOccupied(this.m_LinkInstance);
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ClearTrackedList()
		{
			NavMeshLink.s_Tracked.Clear();
		}

		private void UpgradeSerializedVersion()
		{
			if (this.m_SerializedVersion < 1)
			{
				this.m_SerializedVersion = 1;
				this.m_IsOverridingCost = (this.m_CostModifier >= 0f);
				this.m_CostModifier = Mathf.Abs(this.m_CostModifier);
				if (this.m_StartTransform == base.gameObject.transform)
				{
					this.m_StartTransform = null;
				}
				if (this.m_EndTransform == base.gameObject.transform)
				{
					this.m_EndTransform = null;
				}
			}
		}

		private void Awake()
		{
			this.UpgradeSerializedVersion();
		}

		private void OnEnable()
		{
			this.AddLink();
			if (this.m_AutoUpdatePosition && NavMesh.IsLinkValid(this.m_LinkInstance))
			{
				NavMeshLink.AddTracking(this);
			}
		}

		private void OnDisable()
		{
			NavMeshLink.RemoveTracking(this);
			NavMesh.RemoveLink(this.m_LinkInstance);
		}

		public void UpdateLink()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			NavMesh.RemoveLink(this.m_LinkInstance);
			this.AddLink();
		}

		private static void AddTracking(NavMeshLink link)
		{
			if (NavMeshLink.s_Tracked.Count == 0)
			{
				NavMesh.onPreUpdate = (NavMesh.OnNavMeshPreUpdate)Delegate.Combine(NavMesh.onPreUpdate, new NavMesh.OnNavMeshPreUpdate(NavMeshLink.UpdateTrackedInstances));
			}
			NavMeshLink.s_Tracked.Add(link);
			link.RecordEndpointTransforms();
		}

		private static void RemoveTracking(NavMeshLink link)
		{
			NavMeshLink.s_Tracked.Remove(link);
			if (NavMeshLink.s_Tracked.Count == 0)
			{
				NavMesh.onPreUpdate = (NavMesh.OnNavMeshPreUpdate)Delegate.Remove(NavMesh.onPreUpdate, new NavMesh.OnNavMeshPreUpdate(NavMeshLink.UpdateTrackedInstances));
			}
		}

		internal void GetWorldPositions(out Vector3 worldStartPosition, out Vector3 worldEndPosition)
		{
			bool flag = this.m_StartTransform == null;
			bool flag2 = this.m_EndTransform == null;
			Matrix4x4 matrix4x = (flag || flag2) ? this.LocalToWorldUnscaled() : Matrix4x4.identity;
			worldStartPosition = (flag ? matrix4x.MultiplyPoint3x4(this.m_StartPoint) : this.m_StartTransform.position);
			worldEndPosition = (flag2 ? matrix4x.MultiplyPoint3x4(this.m_EndPoint) : this.m_EndTransform.position);
		}

		internal void GetLocalPositions(out Vector3 localStartPosition, out Vector3 localEndPosition)
		{
			bool flag = this.m_StartTransform == null;
			bool flag2 = this.m_EndTransform == null;
			Matrix4x4 matrix4x = (flag && flag2) ? Matrix4x4.identity : this.LocalToWorldUnscaled().inverse;
			localStartPosition = (flag ? this.m_StartPoint : matrix4x.MultiplyPoint3x4(this.m_StartTransform.position));
			localEndPosition = (flag2 ? this.m_EndPoint : matrix4x.MultiplyPoint3x4(this.m_EndTransform.position));
		}

		private void AddLink()
		{
			Vector3 startPosition;
			Vector3 endPosition;
			this.GetLocalPositions(out startPosition, out endPosition);
			NavMeshLinkData link = new NavMeshLinkData
			{
				startPosition = startPosition,
				endPosition = endPosition,
				width = this.m_Width,
				costModifier = this.costModifier,
				bidirectional = this.m_Bidirectional,
				area = this.m_Area,
				agentTypeID = this.m_AgentTypeID
			};
			this.m_LinkInstance = NavMesh.AddLink(link, base.transform.position, base.transform.rotation);
			if (NavMesh.IsLinkValid(this.m_LinkInstance))
			{
				NavMesh.SetLinkOwner(this.m_LinkInstance, this);
				NavMesh.SetLinkActive(this.m_LinkInstance, this.m_Activated);
			}
			this.m_LastPosition = base.transform.position;
			this.m_LastRotation = base.transform.rotation;
			this.RecordEndpointTransforms();
			this.GetWorldPositions(out this.m_LastStartWorldPosition, out this.m_LastEndWorldPosition);
		}

		internal void RecordEndpointTransforms()
		{
			this.m_StartTransformWasEmpty = (this.m_StartTransform == null);
			this.m_EndTransformWasEmpty = (this.m_EndTransform == null);
		}

		internal bool HaveTransformsChanged()
		{
			bool flag = this.m_StartTransform == null;
			bool flag2 = this.m_EndTransform == null;
			if (flag && flag2 && this.m_StartTransformWasEmpty && this.m_EndTransformWasEmpty && base.transform.position == this.m_LastPosition && base.transform.rotation == this.m_LastRotation)
			{
				return false;
			}
			Matrix4x4 matrix4x = (flag || flag2) ? this.LocalToWorldUnscaled() : Matrix4x4.identity;
			return (flag ? matrix4x.MultiplyPoint3x4(this.m_StartPoint) : this.m_StartTransform.position) != this.m_LastStartWorldPosition || (flag2 ? matrix4x.MultiplyPoint3x4(this.m_EndPoint) : this.m_EndTransform.position) != this.m_LastEndWorldPosition;
		}

		internal Matrix4x4 LocalToWorldUnscaled()
		{
			return Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		}

		private void OnDidApplyAnimationProperties()
		{
			this.UpdateLink();
		}

		private static void UpdateTrackedInstances()
		{
			foreach (NavMeshLink navMeshLink in NavMeshLink.s_Tracked)
			{
				if (navMeshLink.HaveTransformsChanged())
				{
					navMeshLink.UpdateLink();
				}
				navMeshLink.RecordEndpointTransforms();
			}
		}

		[Obsolete("autoUpdatePositions has been deprecated. Use autoUpdate instead. (UnityUpgradable) -> autoUpdate")]
		public bool autoUpdatePositions
		{
			get
			{
				return this.autoUpdate;
			}
			set
			{
				this.autoUpdate = value;
			}
		}

		[Obsolete("biDirectional has been deprecated. Use bidirectional instead. (UnityUpgradable) -> bidirectional")]
		public bool biDirectional
		{
			get
			{
				return this.bidirectional;
			}
			set
			{
				this.bidirectional = value;
			}
		}

		[Obsolete("costOverride has been deprecated. Use costModifier instead. (UnityUpgradable) -> costModifier")]
		public float costOverride
		{
			get
			{
				return this.costModifier;
			}
			set
			{
				this.costModifier = value;
			}
		}

		[Obsolete("UpdatePositions() has been deprecated. Use UpdateLink() instead. (UnityUpgradable) -> UpdateLink(*)")]
		public void UpdatePositions()
		{
			this.UpdateLink();
		}

		[SerializeField]
		[HideInInspector]
		private byte m_SerializedVersion;

		[SerializeField]
		private int m_AgentTypeID;

		[SerializeField]
		private Vector3 m_StartPoint = new Vector3(0f, 0f, -2.5f);

		[SerializeField]
		private Vector3 m_EndPoint = new Vector3(0f, 0f, 2.5f);

		[SerializeField]
		private Transform m_StartTransform;

		[SerializeField]
		private Transform m_EndTransform;

		[SerializeField]
		private bool m_Activated = true;

		[SerializeField]
		private float m_Width;

		[SerializeField]
		[Min(0f)]
		private float m_CostModifier = -1f;

		[SerializeField]
		private bool m_IsOverridingCost;

		[SerializeField]
		private bool m_Bidirectional = true;

		[SerializeField]
		private bool m_AutoUpdatePosition;

		[SerializeField]
		private int m_Area;

		private NavMeshLinkInstance m_LinkInstance;

		private bool m_StartTransformWasEmpty = true;

		private bool m_EndTransformWasEmpty = true;

		private Vector3 m_LastStartWorldPosition = Vector3.positiveInfinity;

		private Vector3 m_LastEndWorldPosition = Vector3.positiveInfinity;

		private Vector3 m_LastPosition = Vector3.positiveInfinity;

		private Quaternion m_LastRotation = Quaternion.identity;

		private static readonly List<NavMeshLink> s_Tracked = new List<NavMeshLink>();
	}
}
