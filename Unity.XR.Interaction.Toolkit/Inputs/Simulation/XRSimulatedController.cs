using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[InputControlLayout(stateType = typeof(XRSimulatedControllerState), commonUsages = new string[]
	{
		"LeftHand",
		"RightHand"
	}, isGenericTypeOfDevice = false, displayName = "XR Simulated Controller", updateBeforeRender = true)]
	[Preserve]
	public class XRSimulatedController : XRController
	{
		public Vector2Control primary2DAxis { get; private set; }

		public AxisControl trigger { get; private set; }

		public AxisControl grip { get; private set; }

		public Vector2Control secondary2DAxis { get; private set; }

		public ButtonControl primaryButton { get; private set; }

		public ButtonControl primaryTouch { get; private set; }

		public ButtonControl secondaryButton { get; private set; }

		public ButtonControl secondaryTouch { get; private set; }

		public ButtonControl gripButton { get; private set; }

		public ButtonControl triggerButton { get; private set; }

		public ButtonControl menuButton { get; private set; }

		public ButtonControl primary2DAxisClick { get; private set; }

		public ButtonControl primary2DAxisTouch { get; private set; }

		public ButtonControl secondary2DAxisClick { get; private set; }

		public ButtonControl secondary2DAxisTouch { get; private set; }

		public AxisControl batteryLevel { get; private set; }

		public ButtonControl userPresence { get; private set; }

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.primary2DAxis = base.GetChildControl<Vector2Control>("primary2DAxis");
			this.trigger = base.GetChildControl<AxisControl>("trigger");
			this.grip = base.GetChildControl<AxisControl>("grip");
			this.secondary2DAxis = base.GetChildControl<Vector2Control>("secondary2DAxis");
			this.primaryButton = base.GetChildControl<ButtonControl>("primaryButton");
			this.primaryTouch = base.GetChildControl<ButtonControl>("primaryTouch");
			this.secondaryButton = base.GetChildControl<ButtonControl>("secondaryButton");
			this.secondaryTouch = base.GetChildControl<ButtonControl>("secondaryTouch");
			this.gripButton = base.GetChildControl<ButtonControl>("gripButton");
			this.triggerButton = base.GetChildControl<ButtonControl>("triggerButton");
			this.menuButton = base.GetChildControl<ButtonControl>("menuButton");
			this.primary2DAxisClick = base.GetChildControl<ButtonControl>("primary2DAxisClick");
			this.primary2DAxisTouch = base.GetChildControl<ButtonControl>("primary2DAxisTouch");
			this.secondary2DAxisClick = base.GetChildControl<ButtonControl>("secondary2DAxisClick");
			this.secondary2DAxisTouch = base.GetChildControl<ButtonControl>("secondary2DAxisTouch");
			this.batteryLevel = base.GetChildControl<AxisControl>("batteryLevel");
			this.userPresence = base.GetChildControl<ButtonControl>("userPresence");
		}

		protected unsafe override long ExecuteCommand(InputDeviceCommand* commandPtr)
		{
			long result;
			if (!XRSimulatorUtility.TryExecuteCommand(commandPtr, out result))
			{
				return base.ExecuteCommand(commandPtr);
			}
			return result;
		}
	}
}
