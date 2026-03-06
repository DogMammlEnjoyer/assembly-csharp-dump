using System;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[AddComponentMenu("XR/XR Interactable Snap Volume", 11)]
	[DefaultExecutionOrder(-99)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactables.XRInteractableSnapVolume.html")]
	public class XRInteractableSnapVolume : MonoBehaviour
	{
		public XRInteractionManager interactionManager
		{
			get
			{
				return this.m_InteractionManager;
			}
			set
			{
				this.m_InteractionManager = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.RegisterWithInteractionManager();
				}
			}
		}

		public Object interactableObject
		{
			get
			{
				return this.m_InteractableObject;
			}
			set
			{
				this.m_InteractableObject = value;
				this.interactable = (value as IXRInteractable);
			}
		}

		public Collider snapCollider
		{
			get
			{
				return this.m_SnapCollider;
			}
			set
			{
				if (this.m_SnapCollider == value)
				{
					return;
				}
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.UnregisterWithInteractionManager();
					this.m_SnapCollider = value;
					this.ValidateSnapCollider();
					this.RefreshSnapColliderEnabled();
					this.RegisterWithInteractionManager();
					return;
				}
				this.m_SnapCollider = value;
			}
		}

		public bool disableSnapColliderWhenSelected
		{
			get
			{
				return this.m_DisableSnapColliderWhenSelected;
			}
			set
			{
				this.m_DisableSnapColliderWhenSelected = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.RefreshSnapColliderEnabled();
				}
			}
		}

		public Collider snapToCollider
		{
			get
			{
				return this.m_SnapToCollider;
			}
			set
			{
				this.m_SnapToCollider = value;
			}
		}

		public IXRInteractable interactable
		{
			get
			{
				return this.m_Interactable;
			}
			set
			{
				this.m_Interactable = value;
				this.m_InteractableObject = (value as Object);
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.SetBoundInteractable(value);
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		protected virtual void Reset()
		{
		}

		protected virtual void Awake()
		{
			if (this.m_SnapCollider == null)
			{
				this.m_SnapCollider = XRInteractableSnapVolume.FindSnapCollider(base.gameObject);
			}
			this.ValidateSnapCollider();
		}

		protected virtual void OnEnable()
		{
			this.FindCreateInteractionManager();
			this.RegisterWithInteractionManager();
			if (this.m_InteractableObject != null)
			{
				IXRInteractable ixrinteractable = this.m_InteractableObject as IXRInteractable;
				if (ixrinteractable != null)
				{
					this.interactable = ixrinteractable;
					return;
				}
			}
			IXRInteractable interactable;
			if ((interactable = this.m_Interactable) == null)
			{
				interactable = (this.m_Interactable = base.GetComponentInParent<IXRInteractable>());
			}
			this.interactable = interactable;
		}

		protected virtual void OnDisable()
		{
			this.UnregisterWithInteractionManager();
			this.SetBoundInteractable(null);
			this.SetSnapColliderEnabled(false);
		}

		private void FindCreateInteractionManager()
		{
			if (this.m_InteractionManager != null)
			{
				return;
			}
			this.m_InteractionManager = ComponentLocatorUtility<XRInteractionManager>.FindOrCreateComponent();
		}

		private void RegisterWithInteractionManager()
		{
			if (this.m_RegisteredInteractionManager == this.m_InteractionManager)
			{
				return;
			}
			this.UnregisterWithInteractionManager();
			if (this.m_InteractionManager != null)
			{
				this.m_InteractionManager.RegisterSnapVolume(this);
				this.m_RegisteredInteractionManager = this.m_InteractionManager;
			}
		}

		private void UnregisterWithInteractionManager()
		{
			if (this.m_RegisteredInteractionManager == null)
			{
				return;
			}
			this.m_RegisteredInteractionManager.UnregisterSnapVolume(this);
			this.m_RegisteredInteractionManager = null;
		}

		protected static Collider FindSnapCollider(GameObject gameObject)
		{
			Collider collider = null;
			foreach (Collider collider2 in gameObject.GetComponents<Collider>())
			{
				if (XRInteractableSnapVolume.SupportsTriggerCollider(collider2))
				{
					if (collider2.isTrigger)
					{
						return collider2;
					}
					if (collider == null)
					{
						collider = collider2;
					}
				}
			}
			return collider;
		}

		internal static bool SupportsTriggerCollider(Collider col)
		{
			if (!(col is BoxCollider) && !(col is SphereCollider) && !(col is CapsuleCollider))
			{
				MeshCollider meshCollider = col as MeshCollider;
				return meshCollider != null && meshCollider.convex;
			}
			return true;
		}

		private void ValidateSnapCollider()
		{
			if (this.m_SnapCollider == null)
			{
				Debug.LogWarning("XR Interactable Snap Volume is missing a Snap Collider assignment.", this);
				return;
			}
			if (!XRInteractableSnapVolume.SupportsTriggerCollider(this.m_SnapCollider))
			{
				Debug.LogError("Snap Collider is set to a collider which does not support being a trigger collider. Set it to a Box Collider, Sphere Collider, Capsule Collider, or convex Mesh Collider.", this);
				return;
			}
			if (!this.m_SnapCollider.isTrigger)
			{
				Debug.LogWarning(string.Format("Snap Collider must be trigger collider, updating {0}.", this.m_SnapCollider), this);
				this.m_SnapCollider.isTrigger = true;
			}
		}

		private void SetSnapColliderEnabled(bool enable)
		{
			if (this.m_SnapCollider != null)
			{
				this.m_SnapCollider.enabled = enable;
			}
		}

		public Vector3 GetClosestPoint(Vector3 point)
		{
			if (!(this.m_SnapToCollider == null) && this.m_SnapToCollider.gameObject.activeInHierarchy && this.m_SnapToCollider.enabled)
			{
				return this.m_SnapToCollider.ClosestPoint(point);
			}
			bool flag;
			if (this.m_Interactable != null)
			{
				Object @object = this.m_Interactable as Object;
				flag = (@object == null || @object != null);
			}
			else
			{
				flag = false;
			}
			if (!flag)
			{
				return base.transform.position;
			}
			return this.m_Interactable.transform.position;
		}

		public Vector3 GetClosestPointOfAttachTransform(IXRInteractor interactor)
		{
			bool flag;
			if (this.m_Interactable != null)
			{
				Object @object = this.m_Interactable as Object;
				flag = (@object == null || @object != null);
			}
			else
			{
				flag = false;
			}
			Vector3 vector = flag ? this.m_Interactable.GetAttachTransform(interactor).position : base.transform.position;
			if (this.m_SnapToCollider == null || !this.m_SnapToCollider.gameObject.activeInHierarchy || !this.m_SnapToCollider.enabled)
			{
				return vector;
			}
			return this.m_SnapToCollider.ClosestPoint(vector);
		}

		private void SetBoundInteractable(IXRInteractable source)
		{
			if (this.m_BoundInteractable == source)
			{
				return;
			}
			if (this.m_BoundSelectInteractable != null)
			{
				this.m_BoundSelectInteractable.firstSelectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnFirstSelectEntered));
				this.m_BoundSelectInteractable.lastSelectExited.RemoveListener(new UnityAction<SelectExitEventArgs>(this.OnLastSelectExited));
			}
			this.m_BoundInteractable = source;
			this.m_BoundSelectInteractable = (source as IXRSelectInteractable);
			if (this.m_BoundSelectInteractable != null)
			{
				this.m_BoundSelectInteractable.firstSelectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnFirstSelectEntered));
				this.m_BoundSelectInteractable.lastSelectExited.AddListener(new UnityAction<SelectExitEventArgs>(this.OnLastSelectExited));
			}
			this.RefreshSnapColliderEnabled();
		}

		private void RefreshSnapColliderEnabled()
		{
			bool flag = this.m_BoundSelectInteractable != null && this.m_BoundSelectInteractable.isSelected;
			if (this.m_DisableSnapColliderWhenSelected)
			{
				this.SetSnapColliderEnabled(!flag);
				return;
			}
			this.SetSnapColliderEnabled(true);
		}

		private void OnFirstSelectEntered(SelectEnterEventArgs args)
		{
			if (this.m_DisableSnapColliderWhenSelected)
			{
				this.SetSnapColliderEnabled(false);
			}
		}

		private void OnLastSelectExited(SelectExitEventArgs args)
		{
			if (this.m_DisableSnapColliderWhenSelected)
			{
				this.SetSnapColliderEnabled(true);
			}
		}

		[SerializeField]
		private XRInteractionManager m_InteractionManager;

		[SerializeField]
		[RequireInterface(typeof(IXRInteractable))]
		private Object m_InteractableObject;

		[SerializeField]
		private Collider m_SnapCollider;

		[SerializeField]
		private bool m_DisableSnapColliderWhenSelected = true;

		[SerializeField]
		private Collider m_SnapToCollider;

		private IXRInteractable m_Interactable;

		private IXRInteractable m_BoundInteractable;

		private IXRSelectInteractable m_BoundSelectInteractable;

		private XRInteractionManager m_RegisteredInteractionManager;
	}
}
