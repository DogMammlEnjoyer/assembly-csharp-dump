using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class ControllerHoverHighlight : MonoBehaviour
	{
		protected void Awake()
		{
			this.hand = base.GetComponentInParent<Hand>();
		}

		protected void OnHandInitialized(int deviceIndex)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.hand.renderModelPrefab);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = this.hand.renderModelPrefab.transform.localScale;
			this.renderModel = gameObject.GetComponent<RenderModel>();
			this.renderModel.SetInputSource(this.hand.handType);
			this.renderModel.OnHandInitialized(deviceIndex);
			this.renderModel.SetMaterial(this.highLightMaterial);
			this.hand.SetHoverRenderModel(this.renderModel);
			this.renderModel.onControllerLoaded += this.RenderModel_onControllerLoaded;
			this.renderModel.Hide();
		}

		private void RenderModel_onControllerLoaded()
		{
			this.renderModel.Hide();
		}

		protected void OnParentHandHoverBegin(Interactable other)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (other.transform.parent != base.transform.parent)
			{
				this.ShowHighlight();
			}
		}

		private void OnParentHandHoverEnd(Interactable other)
		{
			this.HideHighlight();
		}

		private void OnParentHandInputFocusAcquired()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.hand.hoveringInteractable && this.hand.hoveringInteractable.transform.parent != base.transform.parent)
			{
				this.ShowHighlight();
			}
		}

		private void OnParentHandInputFocusLost()
		{
			this.HideHighlight();
		}

		public void ShowHighlight()
		{
			if (this.renderModel == null)
			{
				return;
			}
			if (this.fireHapticsOnHightlight)
			{
				this.hand.TriggerHapticPulse(500);
			}
			this.renderModel.Show(false);
		}

		public void HideHighlight()
		{
			if (this.renderModel == null)
			{
				return;
			}
			if (this.fireHapticsOnHightlight)
			{
				this.hand.TriggerHapticPulse(300);
			}
			this.renderModel.Hide();
		}

		public Material highLightMaterial;

		public bool fireHapticsOnHightlight = true;

		protected Hand hand;

		protected RenderModel renderModel;

		protected SteamVR_Events.Action renderModelLoadedAction;
	}
}
