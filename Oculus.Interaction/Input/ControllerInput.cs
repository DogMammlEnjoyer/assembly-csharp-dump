using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public struct ControllerInput
	{
		public ControllerButtonUsage ButtonUsageMask { readonly get; set; }

		public bool PrimaryButton
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.PrimaryButton) > ControllerButtonUsage.None;
			}
		}

		public bool PrimaryTouch
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.PrimaryTouch) > ControllerButtonUsage.None;
			}
		}

		public bool SecondaryButton
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.SecondaryButton) > ControllerButtonUsage.None;
			}
		}

		public bool SecondaryTouch
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.SecondaryTouch) > ControllerButtonUsage.None;
			}
		}

		public bool GripButton
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.GripButton) > ControllerButtonUsage.None;
			}
		}

		public bool TriggerButton
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.TriggerButton) > ControllerButtonUsage.None;
			}
		}

		public bool MenuButton
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.MenuButton) > ControllerButtonUsage.None;
			}
		}

		public bool Primary2DAxisClick
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.Primary2DAxisClick) > ControllerButtonUsage.None;
			}
		}

		public bool Primary2DAxisTouch
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.Primary2DAxisTouch) > ControllerButtonUsage.None;
			}
		}

		public bool Thumbrest
		{
			get
			{
				return (this.ButtonUsageMask & ControllerButtonUsage.Thumbrest) > ControllerButtonUsage.None;
			}
		}

		public float Trigger { readonly get; private set; }

		public float Grip { readonly get; private set; }

		public Vector2 Primary2DAxis { readonly get; private set; }

		public Vector2 Secondary2DAxis { readonly get; private set; }

		public void Clear()
		{
			this.ButtonUsageMask = ControllerButtonUsage.None;
			this.Trigger = 0f;
			this.Grip = 0f;
			this.Primary2DAxis = Vector2.zero;
			this.Secondary2DAxis = Vector2.zero;
		}

		public void SetButton(ControllerButtonUsage usage, bool value)
		{
			if (value)
			{
				this.ButtonUsageMask |= usage;
				return;
			}
			this.ButtonUsageMask &= ~usage;
		}

		public void SetAxis1D(ControllerAxis1DUsage usage, float value)
		{
			if (usage == ControllerAxis1DUsage.Trigger)
			{
				this.Trigger = value;
				return;
			}
			if (usage != ControllerAxis1DUsage.Grip)
			{
				return;
			}
			this.Grip = value;
		}

		public void SetAxis2D(ControllerAxis2DUsage usage, Vector2 value)
		{
			if (usage == ControllerAxis2DUsage.Primary2DAxis)
			{
				this.Primary2DAxis = value;
				return;
			}
			if (usage != ControllerAxis2DUsage.Secondary2DAxis)
			{
				return;
			}
			this.Secondary2DAxis = value;
		}
	}
}
