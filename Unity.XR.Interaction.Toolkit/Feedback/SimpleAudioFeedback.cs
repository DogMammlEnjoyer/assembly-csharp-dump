using System;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Feedback
{
	[AddComponentMenu("XR/Feedback/Simple Audio Feedback", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Feedback.SimpleAudioFeedback.html")]
	public class SimpleAudioFeedback : MonoBehaviour
	{
		public AudioSource audioSource
		{
			get
			{
				return this.m_AudioSource;
			}
			set
			{
				this.m_AudioSource = value;
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

		public AudioClip selectEnteredClip
		{
			get
			{
				return this.m_SelectEnteredClip;
			}
			set
			{
				this.m_SelectEnteredClip = value;
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

		public AudioClip selectExitedClip
		{
			get
			{
				return this.m_SelectExitedClip;
			}
			set
			{
				this.m_SelectExitedClip = value;
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

		public AudioClip selectCanceledClip
		{
			get
			{
				return this.m_SelectCanceledClip;
			}
			set
			{
				this.m_SelectCanceledClip = value;
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

		public AudioClip hoverEnteredClip
		{
			get
			{
				return this.m_HoverEnteredClip;
			}
			set
			{
				this.m_HoverEnteredClip = value;
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

		public AudioClip hoverExitedClip
		{
			get
			{
				return this.m_HoverExitedClip;
			}
			set
			{
				this.m_HoverExitedClip = value;
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

		public AudioClip hoverCanceledClip
		{
			get
			{
				return this.m_HoverCanceledClip;
			}
			set
			{
				this.m_HoverCanceledClip = value;
			}
		}

		public bool allowHoverAudioWhileSelecting
		{
			get
			{
				return this.m_AllowHoverAudioWhileSelecting;
			}
			set
			{
				this.m_AllowHoverAudioWhileSelecting = value;
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
			if ((this.m_PlaySelectEntered || this.m_PlaySelectExited || this.m_PlaySelectCanceled || this.m_PlayHoverEntered || this.m_PlayHoverExited || this.m_PlayHoverCanceled) && this.m_AudioSource == null)
			{
				this.CreateAudioSource();
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

		protected void PlayAudio(AudioClip clip)
		{
			if (clip == null)
			{
				return;
			}
			if (this.m_AudioSource == null)
			{
				this.CreateAudioSource();
			}
			this.m_AudioSource.PlayOneShot(clip);
		}

		private void CreateAudioSource()
		{
			if (!base.TryGetComponent<AudioSource>(out this.m_AudioSource))
			{
				this.m_AudioSource = base.gameObject.AddComponent<AudioSource>();
			}
			this.m_AudioSource.loop = false;
			this.m_AudioSource.playOnAwake = false;
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
				this.PlayAudio(this.m_SelectEnteredClip);
			}
		}

		private void OnSelectExited(SelectExitEventArgs args)
		{
			if (this.m_PlaySelectCanceled && args.isCanceled)
			{
				this.PlayAudio(this.m_SelectCanceledClip);
			}
			if (this.m_PlaySelectExited && !args.isCanceled)
			{
				this.PlayAudio(this.m_SelectExitedClip);
			}
		}

		private void OnHoverEntered(HoverEnterEventArgs args)
		{
			if (this.m_PlayHoverEntered && this.IsHoverAudioAllowed(args.interactorObject, args.interactableObject))
			{
				this.PlayAudio(this.m_HoverEnteredClip);
			}
		}

		private void OnHoverExited(HoverExitEventArgs args)
		{
			if (!this.IsHoverAudioAllowed(args.interactorObject, args.interactableObject))
			{
				return;
			}
			if (this.m_PlayHoverCanceled && args.isCanceled)
			{
				this.PlayAudio(this.m_HoverCanceledClip);
			}
			if (this.m_PlayHoverExited && !args.isCanceled)
			{
				this.PlayAudio(this.m_HoverExitedClip);
			}
		}

		private bool IsHoverAudioAllowed(IXRInteractor interactor, IXRInteractable interactable)
		{
			return this.m_AllowHoverAudioWhileSelecting || !SimpleAudioFeedback.IsSelecting(interactor, interactable);
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
		private AudioSource m_AudioSource;

		[SerializeField]
		private bool m_PlaySelectEntered;

		[SerializeField]
		private AudioClip m_SelectEnteredClip;

		[SerializeField]
		private bool m_PlaySelectExited;

		[SerializeField]
		private AudioClip m_SelectExitedClip;

		[SerializeField]
		private bool m_PlaySelectCanceled;

		[SerializeField]
		private AudioClip m_SelectCanceledClip;

		[SerializeField]
		private bool m_PlayHoverEntered;

		[SerializeField]
		private AudioClip m_HoverEnteredClip;

		[SerializeField]
		private bool m_PlayHoverExited;

		[SerializeField]
		private AudioClip m_HoverExitedClip;

		[SerializeField]
		private bool m_PlayHoverCanceled;

		[SerializeField]
		private AudioClip m_HoverCanceledClip;

		[SerializeField]
		private bool m_AllowHoverAudioWhileSelecting;

		private readonly UnityObjectReferenceCache<IXRInteractor, Object> m_InteractorSource = new UnityObjectReferenceCache<IXRInteractor, Object>();
	}
}
