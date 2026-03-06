using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.OpenXR.Input
{
	[Preserve]
	[InputControlLayout(displayName = "OpenXR HMD")]
	internal class OpenXRHmd : XRHMD
	{
		[Preserve]
		[InputControl]
		private ButtonControl userPresence { get; set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.userPresence = base.GetChildControl<ButtonControl>("UserPresence");
		}
	}
}
