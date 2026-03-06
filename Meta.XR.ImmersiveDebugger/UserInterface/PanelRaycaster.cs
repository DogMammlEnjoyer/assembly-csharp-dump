using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	[RequireComponent(typeof(Canvas))]
	internal class PanelRaycaster : OVRRaycaster
	{
		public override void OnPointerEnter(PointerEventData e)
		{
		}

		public override bool IsFocussed()
		{
			return false;
		}

		protected override void OnEnable()
		{
			PanelInputModule.RegisterRaycaster(this);
		}

		protected override void OnDisable()
		{
			PanelInputModule.UnregisterRaycaster(this);
		}

		public bool IsValid
		{
			get
			{
				return this.eventCamera != null;
			}
		}
	}
}
