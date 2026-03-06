using System;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class InteractableController : Controller
	{
		private protected bool Hover
		{
			protected get
			{
				return this._hover;
			}
			private set
			{
				if (this._hover == value)
				{
					return;
				}
				this._hover = value;
				this.OnHoverChanged();
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._handler = base.GameObject.AddComponent<PointerHandler>();
			this._handler.Controller = this;
		}

		public void OnPointerEnter()
		{
			this.Hover = true;
		}

		public void OnPointerExit()
		{
			this.Hover = false;
		}

		public virtual void OnPointerClick()
		{
		}

		protected virtual void OnHoverChanged()
		{
		}

		protected virtual void OnDisable()
		{
			this.Hover = false;
		}

		protected void PlayHaptics(OVRHapticsClip hapticsClip)
		{
			if (hapticsClip == null)
			{
				return;
			}
			OVRInput.Controller activeController = OVRInput.GetActiveController();
			if (activeController == OVRInput.Controller.LTouch)
			{
				OVRHaptics.LeftChannel.Mix(hapticsClip);
				return;
			}
			if (activeController != OVRInput.Controller.RTouch)
			{
				return;
			}
			OVRHaptics.RightChannel.Mix(hapticsClip);
		}

		private PointerHandler _handler;

		private bool _hover;
	}
}
