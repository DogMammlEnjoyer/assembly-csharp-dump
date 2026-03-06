using System;
using System.Diagnostics;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[AddComponentMenu("XR/XR Poke Filter", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Filtering.XRPokeFilter.html")]
	public class XRPokeFilter : MonoBehaviour, IXRPokeFilter, IXRSelectFilter, IXRInteractionStrengthFilter, IPokeStateDataProvider
	{
		public XRBaseInteractable pokeInteractable
		{
			get
			{
				return this.m_Interactable;
			}
			set
			{
				this.m_Interactable = value;
				this.Setup();
			}
		}

		public Collider pokeCollider
		{
			get
			{
				return this.m_PokeCollider;
			}
			set
			{
				this.m_PokeCollider = value;
				this.Setup();
			}
		}

		public PokeThresholdDatumProperty pokeConfiguration
		{
			get
			{
				return this.m_PokeConfiguration;
			}
			set
			{
				this.m_PokeConfiguration = value;
				this.Setup();
			}
		}

		public IReadOnlyBindableVariable<PokeStateData> pokeStateData
		{
			get
			{
				XRPokeLogic pokeLogic = this.m_PokeLogic;
				if (pokeLogic == null)
				{
					return null;
				}
				return pokeLogic.pokeStateData;
			}
		}

		public virtual bool canProcess
		{
			get
			{
				return base.isActiveAndEnabled && this.m_PokeLogic != null;
			}
		}

		[Conditional("UNITY_EDITOR")]
		protected virtual void Reset()
		{
		}

		[Conditional("UNITY_EDITOR")]
		protected void OnValidate()
		{
		}

		protected void Awake()
		{
			if (this.m_Interactable == null)
			{
				this.m_Interactable = this.FindPokeInteractable();
			}
			if (this.m_PokeCollider == null)
			{
				this.m_PokeCollider = this.FindPokeCollider();
			}
		}

		protected void Start()
		{
			bool flag = false;
			if (this.m_Interactable == null)
			{
				this.m_Interactable = this.FindPokeInteractable();
				if (this.m_Interactable == null)
				{
					Debug.LogWarning("Could not find associated XRBaseInteractable in scene.This XRPokeFilter will be disabled.", this);
					flag = true;
				}
			}
			if (this.m_PokeCollider == null)
			{
				this.m_PokeCollider = this.FindPokeCollider();
				if (this.m_PokeCollider == null)
				{
					Debug.LogWarning("Could not find a Collider associated with this filter in the scene.This XRPokeFilter will be disabled.", this);
					flag = true;
				}
			}
			if (this.m_PokeConfiguration.Value == null)
			{
				Debug.LogWarning("Poke Data property has been improperly configured. Please assign a Poke Threshold Datum asset if configured to Use Asset.", this);
				flag = true;
			}
			if (flag)
			{
				base.enabled = false;
				return;
			}
			this.Setup();
		}

		protected void OnDestroy()
		{
			this.Unsubscribe();
			XRPokeLogic pokeLogic = this.m_PokeLogic;
			if (pokeLogic == null)
			{
				return;
			}
			pokeLogic.Dispose();
		}

		[Conditional("UNITY_EDITOR")]
		protected void OnDrawGizmosSelected()
		{
		}

		public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			IPokeStateDataProvider pokeStateDataProvider = interactor as IPokeStateDataProvider;
			if (pokeStateDataProvider != null)
			{
				float pokeInteractionOffset = 0f;
				XRPokeInteractor xrpokeInteractor = interactor as XRPokeInteractor;
				if (xrpokeInteractor != null)
				{
					pokeInteractionOffset = xrpokeInteractor.pokeInteractionOffset;
				}
				Transform attachTransform = interactable.GetAttachTransform(interactor);
				return this.m_PokeLogic.MeetsRequirementsForSelectAction(pokeStateDataProvider, attachTransform.position, interactor.GetAttachTransform(interactable).position, pokeInteractionOffset, attachTransform);
			}
			return true;
		}

		public float Process(IXRInteractor interactor, IXRInteractable interactable, float interactionStrength)
		{
			float b = 0f;
			if (interactor is IPokeStateDataProvider)
			{
				IReadOnlyBindableVariable<PokeStateData> pokeStateData = this.pokeStateData;
				b = ((pokeStateData != null) ? pokeStateData.Value.interactionStrength : 0f);
			}
			return Mathf.Max(interactionStrength, b);
		}

		private void OnHoverEntered(HoverEnterEventArgs args)
		{
			if (this.m_PokeLogic == null)
			{
				return;
			}
			IXRHoverInteractor interactorObject = args.interactorObject;
			IXRHoverInteractable interactableObject = args.interactableObject;
			Transform attachTransform = interactorObject.GetAttachTransform(interactableObject);
			Transform attachTransform2 = interactableObject.GetAttachTransform(interactorObject);
			this.m_PokeLogic.OnHoverEntered(interactorObject, attachTransform.GetWorldPose(), attachTransform2);
		}

		private void OnHoverExited(HoverExitEventArgs args)
		{
			if (this.m_PokeLogic == null)
			{
				return;
			}
			this.m_PokeLogic.OnHoverExited(args.interactorObject);
		}

		private XRBaseInteractable FindPokeInteractable()
		{
			if (!(this.m_Interactable != null))
			{
				return base.GetComponentInParent<XRBaseInteractable>();
			}
			return this.m_Interactable;
		}

		private Collider FindPokeCollider()
		{
			if (!(this.m_PokeCollider != null))
			{
				return base.GetComponentInChildren<Collider>();
			}
			return this.m_PokeCollider;
		}

		private void Setup()
		{
			if (this.m_PokeLogic == null)
			{
				this.m_PokeLogic = new XRPokeLogic();
			}
			XRBaseInteractable xrbaseInteractable = this.FindPokeInteractable();
			Collider collider = this.FindPokeCollider();
			PokeThresholdData value = this.m_PokeConfiguration.Value;
			if (xrbaseInteractable != null && collider != null && value != null)
			{
				this.m_PokeLogic.Initialize(xrbaseInteractable.GetAttachTransform(null), value, collider);
				if (Application.isPlaying)
				{
					this.Subscribe(xrbaseInteractable);
				}
			}
		}

		private void Subscribe(XRBaseInteractable interactable)
		{
			if (this.m_SubscribedInteractable == interactable)
			{
				return;
			}
			this.Unsubscribe();
			interactable.selectFilters.Add(this);
			interactable.interactionStrengthFilters.Add(this);
			interactable.hoverEntered.AddListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
			interactable.hoverExited.AddListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
			this.m_SubscribedInteractable = interactable;
		}

		private void Unsubscribe()
		{
			if (this.m_SubscribedInteractable == null)
			{
				return;
			}
			this.m_SubscribedInteractable.selectFilters.Remove(this);
			this.m_SubscribedInteractable.interactionStrengthFilters.Remove(this);
			this.m_SubscribedInteractable.hoverEntered.RemoveListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
			this.m_SubscribedInteractable.hoverExited.RemoveListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
			this.m_SubscribedInteractable = null;
		}

		[SerializeField]
		[Tooltip("The interactable associated with this poke filter.")]
		private XRBaseInteractable m_Interactable;

		[SerializeField]
		[Tooltip("The collider used to compute bounds of the poke interaction.")]
		private Collider m_PokeCollider;

		[SerializeField]
		[Tooltip("The settings used to fine tune the vector and offsets which dictate how the poke interaction will be evaluated.")]
		private PokeThresholdDatumProperty m_PokeConfiguration = new PokeThresholdDatumProperty(new PokeThresholdData());

		private XRPokeLogic m_PokeLogic = new XRPokeLogic();

		private XRBaseInteractable m_SubscribedInteractable;
	}
}
