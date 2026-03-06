using System;

namespace Oculus.Interaction.Input
{
	[Flags]
	public enum ControllerButtonUsage
	{
		None = 0,
		PrimaryButton = 1,
		PrimaryTouch = 2,
		SecondaryButton = 4,
		SecondaryTouch = 8,
		GripButton = 16,
		TriggerButton = 32,
		MenuButton = 64,
		Primary2DAxisClick = 128,
		Primary2DAxisTouch = 256,
		Thumbrest = 512
	}
}
