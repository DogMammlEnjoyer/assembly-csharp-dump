using System;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Feedback
{
	[AddComponentMenu("XR/Feedback/Simple Haptic Feedback", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Feedback.SimpleHapticFeedback.html")]
	public class SimpleHapticFeedback : MonoBehaviour
	{
		public HapticImpulsePlayer hapticImpulsePlayer
		{
			get
			{
				return this.m_HapticImpulsePlayer;
			}
			set
			{
				this.m_HapticImpulsePlayer = value;
			}
		}

		public bool playSelectEntered
		{
			get
			{
				return this.m_PlaySelectEntered;
			}
			set
			{
				this.m_PlaySelectEntered = value;
			}
		}

		public HapticImpulseData selectEnteredData
		{
			get
			{
				return this.m_SelectEnteredData;
			}
			set
			{
				this.m_SelectEnteredData = value;
			}
		}

		public bool playSelectExited
		{
			get
			{
				return this.m_PlaySelectExited;
			}
			set
			{
				this.m_PlaySelectExited = value;
			}
		}

		public HapticImpulseData selectExitedData
		{
			get
			{
				return this.m_SelectExitedData;
			}
			set
			{
				this.m_SelectExitedData = value;
			}
		}

		public bool playSelectCanceled
		{
			get
			{
				return this.m_PlaySelectCanceled;
			}
			set
			{
				this.m_PlaySelectCanceled = value;
			}
		}

		public HapticImpulseData selectCanceledData
		{
			get
			{
				return this.m_SelectCanceledData;
			}
			set
			{
				this.m_SelectCanceledData = value;
			}
		}

		public bool playHoverEntered
		{
			get
			{
				return this.m_PlayHoverEntered;
			}
			set
			{
				this.m_PlayHoverEntered = value;
			}
		}

		public HapticImpulseData hoverEnteredData
		{
			get
			{
				return this.m_HoverEnteredData;
			}
			set
			{
				this.m_HoverEnteredData = value;
			}
		}

		public bool playHoverExited
		{
			get
			{
				return this.m_PlayHoverExited;
			}
			set
			{
				this.m_PlayHoverExited = value;
			}
		}

		public HapticImpulseData hoverExitedData
		{
			get
			{
				return this.m_HoverExitedData;
			}
			set
			{
				this.m_HoverExitedData = value;
			}
		}

		public bool playHoverCanceled
		{
			get
			{
				return this.m_PlayHoverCanceled;
			}
			set
			{
				this.m_PlayHoverCanceled = value;
			}
		}

		public HapticImpulseData hoverCanceledData
		{
			get
			{
				return this.m_HoverCanceledData;
			}
			set
			{
				this.m_HoverCanceledData = value;
			}
		}

		public bool allowHoverHapticsWhileSelecting
		{
			get
			{
				return this.m_AllowHoverHapticsWhileSelecting;
			}
			set
			{
				this.m_AllowHoverHapticsWhileSelecting = value;
			}
		}

		[Conditional("UNITY_EDITOR")]
		protected void Reset()
		{
		}

		protected void Awake()
		{
			if (this.m_InteractorSourceObject == null)
			{
				this.m_InteractorSourceObject = (base.GetComponentInParent<IXRInteractor>(true) as Object);
			}
			if ((this.m_PlaySelectEntered || this.m_PlaySelectExited || this.m_PlaySelectCanceled || this.m_PlayHoverEntered || this.m_PlayHoverExited || this.m_PlayHoverCanceled) && this.m_HapticImpulsePlayer == null)
			{
				this.CreateHapticImpulsePlayer();
			}
		}

		protected void OnEnable()
		{
			this.Subscribe(this.GetInteractorSource());
		}

		protected void OnDisable()
		{
			this.Unsubscribe(this.GetInteractorSource());
		}

		public IXRInteractor GetInteractorSource()
		{
			return this.m_InteractorSource.Get(this.m_InteractorSourceObject);
		}

		public void SetInteractorSource(IXRInteractor interactor)
		{
			if (Application.isPlaying && base.isActiveAndEnabled)
			{
				this.Unsubscribe(this.m_InteractorSource.Get(this.m_InteractorSourceObject));
			}
			this.m_InteractorSource.Set(ref this.m_InteractorSourceObject, interactor);
			if (Application.isPlaying && base.isActiveAndEnabled)
			{
				this.Subscribe(interactor);
			}
		}

		protected bool SendHapticImpulse(HapticImpulseData data)
		{
			return data != null && this.SendHapticImpulse(data.amplitude, data.duration, data.frequency);
		}

		protected bool SendHapticImpulse(float amplitude, float duration, float frequency)
		{
			if (this.m_HapticImpulsePlayer == null)
			{
				this.CreateHapticImpulsePlayer();
			}
			return this.m_HapticImpulsePlayer.SendHapticImpulse(amplitude, duration, frequency);
		}

		private void CreateHapticImpulsePlayer()
		{
			this.m_HapticImpulsePlayer = HapticImpulsePlayer.GetOrCreateInHierarchy(base.gameObject);
		}

		private void Subscribe(IXRInteractor interactor)
		{
			if (interactor != null)
			{
				Object @object = interactor as Object;
				if (@object == null || !(@object == null))
				{
					IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
					if (ixrselectInteractor != null)
					{
						ixrselectInteractor.selectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnSelectEntered));
						ixrselectInteractor.selectExited.AddListener(new UnityAction<SelectExitEventArgs>(this.OnSelectExited));
					}
					IXRHoverInteractor ixrhoverInteractor = interactor as IXRHoverInteractor;
					if (ixrhoverInteractor != null)
					{
						ixrhoverInteractor.hoverEntered.AddListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
						ixrhoverInteractor.hoverExited.AddListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
					}
					return;
				}
			}
		}

		private void Unsubscribe(IXRInteractor interactor)
		{
			if (interactor != null)
			{
				Object @object = interactor as Object;
				if (@object == null || !(@object == null))
				{
					IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
					if (ixrselectInteractor != null)
					{
						ixrselectInteractor.selectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnSelectEntered));
						ixrselectInteractor.selectExited.RemoveListener(new UnityAction<SelectExitEventArgs>(this.OnSelectExited));
					}
					IXRHoverInteractor ixrhoverInteractor = interactor as IXRHoverInteractor;
					if (ixrhoverInteractor != null)
					{
						ixrhoverInteractor.hoverEntered.RemoveListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
						ixrhoverInteractor.hoverExited.RemoveListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
					}
					return;
				}
			}
		}

		private void OnSelectEntered(SelectEnterEventArgs args)
		{
			if (this.m_PlaySelectEntered)
			{
				this.SendHapticImpulse(this.m_SelectEnteredData);
			}
		}

		private void OnSelectExited(SelectExitEventArgs args)
		{
			if (this.m_PlaySelectCanceled && args.isCanceled)
			{
				this.SendHapticImpulse(this.m_SelectCanceledData);
			}
			if (this.m_PlaySelectExited && !args.isCanceled)
			{
				this.SendHapticImpulse(this.m_SelectExitedData);
			}
		}

		private void OnHoverEntered(HoverEnterEventArgs args)
		{
			if (this.m_PlayHoverEntered && this.IsHoverHapticsAllowed(args.interactorObject, args.interactableObject))
			{
				this.SendHapticImpulse(this.m_HoverEnteredData);
			}
		}

		private void OnHoverExited(HoverExitEventArgs args)
		{
			if (!this.IsHoverHapticsAllowed(args.interactorObject, args.interactableObject))
			{
				return;
			}
			if (this.m_PlayHoverCanceled && args.isCanceled)
			{
				this.SendHapticImpulse(this.m_HoverCanceledData);
			}
			if (this.m_PlayHoverExited && !args.isCanceled)
			{
				this.SendHapticImpulse(this.m_HoverExitedData);
			}
		}

		private bool IsHoverHapticsAllowed(IXRInteractor interactor, IXRInteractable interactable)
		{
			return this.m_AllowHoverHapticsWhileSelecting || !SimpleHapticFeedback.IsSelecting(interactor, interactable);
		}

		private static bool IsSelecting(IXRInteractor interactor, IXRInteractable interactable)
		{
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			if (ixrselectInteractor != null)
			{
				IXRSelectInteractable ixrselectInteractable = interactable as IXRSelectInteractable;
				if (ixrselectInteractable != null)
				{
					return ixrselectInteractor.IsSelecting(ixrselectInteractable);
				}
			}
			return false;
		}

		[SerializeField]
		[RequireInterface(typeof(IXRInteractor))]
		private Object m_InteractorSourceObject;

		[SerializeField]
		private HapticImpulsePlayer m_HapticImpulsePlayer;

		[SerializeField]
		private bool m_PlaySelectEntered;

		[SerializeField]
		private HapticImpulseData m_SelectEnteredData = new HapticImpulseData
		{
			amplitude = 0.5f,
			duration = 0.1f
		};

		[SerializeField]
		private bool m_PlaySelectExited;

		[SerializeField]
		private HapticImpulseData m_SelectExitedData = new HapticImpulseData
		{
			amplitude = 0.5f,
			duration = 0.1f
		};

		[SerializeField]
		private bool m_PlaySelectCanceled;

		[SerializeField]
		private HapticImpulseData m_SelectCanceledData = new HapticImpulseData
		{
			amplitude = 0.5f,
			duration = 0.1f
		};

		[SerializeField]
		private bool m_PlayHoverEntered;

		[SerializeField]
		private HapticImpulseData m_HoverEnteredData = new HapticImpulseData
		{
			amplitude = 0.25f,
			duration = 0.1f
		};

		[SerializeField]
		private bool m_PlayHoverExited;

		[SerializeField]
		private HapticImpulseData m_HoverExitedData = new HapticImpulseData
		{
			amplitude = 0.25f,
			duration = 0.1f
		};

		[SerializeField]
		private bool m_PlayHoverCanceled;

		[SerializeField]
		private HapticImpulseData m_HoverCanceledData = new HapticImpulseData
		{
			amplitude = 0.25f,
			duration = 0.1f
		};

		[SerializeField]
		private bool m_AllowHoverHapticsWhileSelecting;

		private readonly UnityObjectReferenceCache<IXRInteractor, Object> m_InteractorSource = new UnityObjectReferenceCache<IXRInteractor, Object>();
	}
}
