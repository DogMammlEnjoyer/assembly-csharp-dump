using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
	[Serializable]
	public class XRLastSelectedEvaluator : XRTargetEvaluator, IXRTargetEvaluatorLinkable
	{
		public float maxTime
		{
			get
			{
				return this.m_MaxTime;
			}
			set
			{
				this.m_MaxTime = value;
			}
		}

		private void OnSelect(SelectEnterEventArgs args)
		{
			if (base.enabled)
			{
				IXRInteractable interactableObject = args.interactableObject;
				if (interactableObject != null)
				{
					this.m_InteractableSelectionTimeMap[interactableObject] = Time.time;
				}
			}
		}

		public virtual void OnLink(IXRInteractor interactor)
		{
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			if (ixrselectInteractor != null)
			{
				ixrselectInteractor.selectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnSelect));
			}
		}

		public virtual void OnUnlink(IXRInteractor interactor)
		{
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			if (ixrselectInteractor != null)
			{
				ixrselectInteractor.selectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnSelect));
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_InteractableSelectionTimeMap.Clear();
		}

		protected override float CalculateNormalizedScore(IXRInteractor interactor, IXRInteractable target)
		{
			float num;
			if (!this.m_InteractableSelectionTimeMap.TryGetValue(target, out num) || this.m_MaxTime <= 0f)
			{
				return 0.5f;
			}
			return (1f - Mathf.Clamp01((Time.time - num) / this.m_MaxTime)) * 0.5f + 0.5f;
		}

		private readonly Dictionary<IXRInteractable, float> m_InteractableSelectionTimeMap = new Dictionary<IXRInteractable, float>();

		[Tooltip("Any Interactable which was last selected over Max Time seconds ago will receive a normalized score of 0.")]
		[SerializeField]
		private float m_MaxTime = 10f;
	}
}
