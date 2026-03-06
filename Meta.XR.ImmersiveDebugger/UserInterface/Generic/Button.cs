using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Button : InteractableController
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			Button._hapticsClip = null;
		}

		private static OVRHapticsClip HapticsClip
		{
			get
			{
				if (OVRHaptics.Config.SampleSizeInBytes == 0)
				{
					return null;
				}
				OVRHapticsClip result;
				if ((result = Button._hapticsClip) == null)
				{
					result = (Button._hapticsClip = new OVRHapticsClip(new byte[]
					{
						128,
						byte.MaxValue,
						byte.MaxValue,
						128,
						byte.MaxValue
					}, 5));
				}
				return result;
			}
		}

		public Action Callback { get; set; }

		public override void OnPointerClick()
		{
			Action callback = this.Callback;
			if (callback != null)
			{
				callback();
			}
			Telemetry.OnButtonClicked(this);
		}

		protected override void OnHoverChanged()
		{
			base.OnHoverChanged();
			if (base.Hover)
			{
				base.PlayHaptics(Button.HapticsClip);
			}
		}

		private static OVRHapticsClip _hapticsClip;
	}
}
