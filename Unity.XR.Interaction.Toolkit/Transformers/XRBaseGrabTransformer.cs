using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
	public abstract class XRBaseGrabTransformer : MonoBehaviour, IXRGrabTransformer
	{
		public virtual bool canProcess
		{
			get
			{
				return base.isActiveAndEnabled;
			}
		}

		protected virtual XRBaseGrabTransformer.RegistrationMode registrationMode
		{
			get
			{
				return XRBaseGrabTransformer.RegistrationMode.Single;
			}
		}

		internal XRBaseGrabTransformer.RegistrationMode GetRegistrationMode()
		{
			return this.registrationMode;
		}

		protected virtual void Start()
		{
			XRGrabInteractable xrgrabInteractable;
			if (base.TryGetComponent<XRGrabInteractable>(out xrgrabInteractable))
			{
				if (xrgrabInteractable.startingSingleGrabTransformers.Contains(this) || xrgrabInteractable.startingMultipleGrabTransformers.Contains(this))
				{
					return;
				}
				for (int i = xrgrabInteractable.singleGrabTransformersCount - 1; i >= 0; i--)
				{
					if (xrgrabInteractable.GetSingleGrabTransformerAt(i) == this)
					{
						return;
					}
				}
				for (int j = xrgrabInteractable.multipleGrabTransformersCount - 1; j >= 0; j--)
				{
					if (xrgrabInteractable.GetMultipleGrabTransformerAt(j) == this)
					{
						return;
					}
				}
				switch (this.registrationMode)
				{
				case XRBaseGrabTransformer.RegistrationMode.None:
					break;
				case XRBaseGrabTransformer.RegistrationMode.Single:
					xrgrabInteractable.AddSingleGrabTransformer(this);
					return;
				case XRBaseGrabTransformer.RegistrationMode.Multiple:
					xrgrabInteractable.AddMultipleGrabTransformer(this);
					return;
				case XRBaseGrabTransformer.RegistrationMode.SingleAndMultiple:
					xrgrabInteractable.AddSingleGrabTransformer(this);
					xrgrabInteractable.AddMultipleGrabTransformer(this);
					break;
				default:
					return;
				}
			}
		}

		protected virtual void OnDestroy()
		{
			XRGrabInteractable xrgrabInteractable;
			if (base.TryGetComponent<XRGrabInteractable>(out xrgrabInteractable))
			{
				xrgrabInteractable.RemoveSingleGrabTransformer(this);
				xrgrabInteractable.RemoveMultipleGrabTransformer(this);
			}
		}

		public virtual void OnLink(XRGrabInteractable grabInteractable)
		{
		}

		public virtual void OnGrab(XRGrabInteractable grabInteractable)
		{
		}

		public virtual void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
		{
		}

		public abstract void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale);

		public virtual void OnUnlink(XRGrabInteractable grabInteractable)
		{
		}

		public enum RegistrationMode
		{
			None,
			Single,
			Multiple,
			SingleAndMultiple
		}
	}
}
