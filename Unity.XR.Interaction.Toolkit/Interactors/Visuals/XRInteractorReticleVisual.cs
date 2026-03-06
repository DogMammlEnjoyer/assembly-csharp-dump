using System;
using Unity.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[AddComponentMenu("XR/Visual/XR Interactor Reticle Visual", 11)]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorReticleVisual.html")]
	public class XRInteractorReticleVisual : MonoBehaviour
	{
		public float maxRaycastDistance
		{
			get
			{
				return this.m_MaxRaycastDistance;
			}
			set
			{
				this.m_MaxRaycastDistance = value;
			}
		}

		public GameObject reticlePrefab
		{
			get
			{
				return this.m_ReticlePrefab;
			}
			set
			{
				this.m_ReticlePrefab = value;
				this.SetupReticlePrefab();
			}
		}

		public float prefabScalingFactor
		{
			get
			{
				return this.m_PrefabScalingFactor;
			}
			set
			{
				this.m_PrefabScalingFactor = value;
			}
		}

		public bool undoDistanceScaling
		{
			get
			{
				return this.m_UndoDistanceScaling;
			}
			set
			{
				this.m_UndoDistanceScaling = value;
			}
		}

		public bool alignPrefabWithSurfaceNormal
		{
			get
			{
				return this.m_AlignPrefabWithSurfaceNormal;
			}
			set
			{
				this.m_AlignPrefabWithSurfaceNormal = value;
			}
		}

		public float endpointSmoothingTime
		{
			get
			{
				return this.m_EndpointSmoothingTime;
			}
			set
			{
				this.m_EndpointSmoothingTime = value;
			}
		}

		public bool drawWhileSelecting
		{
			get
			{
				return this.m_DrawWhileSelecting;
			}
			set
			{
				this.m_DrawWhileSelecting = value;
			}
		}

		public bool drawOnNoHit
		{
			get
			{
				return this.m_DrawOnNoHit;
			}
			set
			{
				this.m_DrawOnNoHit = value;
			}
		}

		public LayerMask raycastMask
		{
			get
			{
				return this.m_RaycastMask;
			}
			set
			{
				this.m_RaycastMask = value;
			}
		}

		public bool reticleActive
		{
			get
			{
				return this.m_ReticleActive;
			}
			set
			{
				this.m_ReticleActive = value;
				if (this.m_ReticleInstance != null)
				{
					this.m_ReticleInstance.SetActive(value);
				}
			}
		}

		protected void Awake()
		{
			this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
			if (base.TryGetComponent<XRBaseInteractor>(out this.m_Interactor))
			{
				this.m_Interactor.selectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnSelectEntered));
			}
			this.FindXROrigin();
			this.SetupReticlePrefab();
			this.reticleActive = false;
		}

		protected void OnDisable()
		{
			this.reticleActive = false;
		}

		protected void Update()
		{
			if (this.m_Interactor != null && this.UpdateReticleTarget())
			{
				this.ActivateReticleAtTarget();
				return;
			}
			this.reticleActive = false;
		}

		protected void OnDestroy()
		{
			if (this.m_InteractorLinePoints.IsCreated)
			{
				this.m_InteractorLinePoints.Dispose();
			}
			if (this.m_Interactor != null)
			{
				this.m_Interactor.selectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnSelectEntered));
			}
		}

		private void FindXROrigin()
		{
			if (this.m_XROrigin == null)
			{
				ComponentLocatorUtility<XROrigin>.TryFindComponent(out this.m_XROrigin);
			}
		}

		private void SetupReticlePrefab()
		{
			if (this.m_ReticleInstance != null)
			{
				Object.Destroy(this.m_ReticleInstance);
			}
			if (this.m_ReticlePrefab != null)
			{
				this.m_ReticleInstance = Object.Instantiate<GameObject>(this.m_ReticlePrefab);
			}
		}

		private static RaycastHit FindClosestHit(RaycastHit[] hits, int hitCount)
		{
			int num = 0;
			float num2 = float.MaxValue;
			for (int i = 0; i < hitCount; i++)
			{
				if (hits[i].distance < num2)
				{
					num2 = hits[i].distance;
					num = i;
				}
			}
			return hits[num];
		}

		private bool TryGetRaycastPoint(ref Vector3 raycastPos, ref Vector3 raycastNormal)
		{
			bool result = false;
			int num = this.m_LocalPhysicsScene.Raycast(this.m_Interactor.attachTransform.position, this.m_Interactor.attachTransform.forward, this.m_RaycastHits, this.m_MaxRaycastDistance, this.m_RaycastMask, QueryTriggerInteraction.UseGlobal);
			if (num != 0)
			{
				RaycastHit raycastHit = XRInteractorReticleVisual.FindClosestHit(this.m_RaycastHits, num);
				raycastPos = raycastHit.point;
				raycastNormal = raycastHit.normal;
				result = true;
			}
			return result;
		}

		private bool UpdateReticleTarget()
		{
			if (!this.m_DrawWhileSelecting && this.m_Interactor.hasSelection)
			{
				return false;
			}
			if (this.m_Interactor.disableVisualsWhenBlockedInGroup && this.m_Interactor.IsBlockedByInteractionWithinGroup())
			{
				return false;
			}
			bool flag = false;
			Vector3 target = Vector3.zero;
			Vector3 vector = Vector3.zero;
			XRRayInteractor xrrayInteractor = this.m_Interactor as XRRayInteractor;
			if (xrrayInteractor != null)
			{
				RaycastHit? raycastHit;
				int num;
				RaycastResult? raycastResult;
				int num2;
				bool flag2;
				if (xrrayInteractor.TryGetCurrentRaycast(out raycastHit, out num, out raycastResult, out num2, out flag2))
				{
					if (flag2)
					{
						RaycastResult value = raycastResult.Value;
						target = value.worldPosition;
						vector = value.worldNormal;
						if (Vector3.Dot(xrrayInteractor.rayOriginTransform.forward, vector) > 0f)
						{
							vector *= -1f;
						}
						flag = true;
					}
					else if (raycastHit != null)
					{
						RaycastHit value2 = raycastHit.Value;
						target = value2.point;
						vector = value2.normal;
						flag = true;
					}
				}
				else if (this.m_DrawOnNoHit && xrrayInteractor.GetLinePoints(ref this.m_InteractorLinePoints, out num2, null))
				{
					NativeArray<Vector3> interactorLinePoints = this.m_InteractorLinePoints;
					target = ((this.m_InteractorLinePoints.Length > 0) ? this.m_InteractorLinePoints[this.m_InteractorLinePoints.Length - 1] : Vector3.zero);
				}
			}
			else if (this.TryGetRaycastPoint(ref target, ref vector))
			{
				flag = true;
			}
			this.m_HasRaycastHit = flag;
			if (flag || this.m_DrawOnNoHit)
			{
				Vector3 zero = Vector3.zero;
				this.m_TargetEndPoint = Vector3.SmoothDamp(this.m_TargetEndPoint, target, ref zero, this.m_EndpointSmoothingTime);
				this.m_TargetEndNormal = Vector3.SmoothDamp(this.m_TargetEndNormal, vector, ref zero, this.m_EndpointSmoothingTime);
				return true;
			}
			return false;
		}

		private void ActivateReticleAtTarget()
		{
			if (this.m_ReticleInstance != null)
			{
				Vector3 vector = (this.m_XROrigin != null && this.m_XROrigin.Origin != null) ? this.m_XROrigin.Origin.transform.up : Vector3.up;
				if (this.m_AlignPrefabWithSurfaceNormal && this.m_HasRaycastHit)
				{
					Vector3 vector2 = vector;
					float num = Vector3.Dot(this.m_TargetEndNormal, vector2);
					if (Mathf.Approximately(Mathf.Abs(num), 1f))
					{
						vector2 = this.m_Interactor.transform.forward * num;
					}
					Vector3 vector3 = Vector3.ProjectOnPlane(vector2, this.m_TargetEndNormal);
					if (vector3 != Vector3.zero)
					{
						this.m_ReticleInstance.transform.SetWorldPose(new Pose(this.m_TargetEndPoint, Quaternion.LookRotation(vector3, this.m_TargetEndNormal)));
					}
					else
					{
						this.m_ReticleInstance.transform.position = this.m_TargetEndPoint;
					}
				}
				else
				{
					this.m_ReticleInstance.transform.SetWorldPose(new Pose(this.m_TargetEndPoint, Quaternion.LookRotation(vector, (this.m_Interactor.attachTransform.position - this.m_TargetEndPoint).normalized)));
				}
				float num2 = this.m_PrefabScalingFactor;
				if (this.m_UndoDistanceScaling)
				{
					num2 *= Vector3.Distance(this.m_Interactor.attachTransform.position, this.m_TargetEndPoint);
				}
				this.m_ReticleInstance.transform.localScale = new Vector3(num2, num2, num2);
				this.reticleActive = true;
			}
		}

		private void OnSelectEntered(SelectEnterEventArgs args)
		{
			this.reticleActive = false;
		}

		private const int k_MaxRaycastHits = 10;

		[SerializeField]
		private float m_MaxRaycastDistance = 10f;

		[SerializeField]
		private GameObject m_ReticlePrefab;

		[SerializeField]
		private float m_PrefabScalingFactor = 1f;

		[SerializeField]
		private bool m_UndoDistanceScaling = true;

		[SerializeField]
		private bool m_AlignPrefabWithSurfaceNormal = true;

		[SerializeField]
		private float m_EndpointSmoothingTime = 0.02f;

		[SerializeField]
		private bool m_DrawWhileSelecting;

		[SerializeField]
		private bool m_DrawOnNoHit;

		[SerializeField]
		private LayerMask m_RaycastMask = -1;

		private bool m_ReticleActive;

		private NativeArray<Vector3> m_InteractorLinePoints;

		private XROrigin m_XROrigin;

		private GameObject m_ReticleInstance;

		private XRBaseInteractor m_Interactor;

		private Vector3 m_TargetEndPoint;

		private Vector3 m_TargetEndNormal;

		private PhysicsScene m_LocalPhysicsScene;

		private bool m_HasRaycastHit;

		private readonly RaycastHit[] m_RaycastHits = new RaycastHit[10];
	}
}
