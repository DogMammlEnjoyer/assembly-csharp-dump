using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
	[Preserve]
	public static class SimulatedInputLayoutLoader
	{
		[Preserve]
		static SimulatedInputLayoutLoader()
		{
			SimulatedInputLayoutLoader.RegisterInputLayouts();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		[Preserve]
		public static void Initialize()
		{
		}

		private static void RegisterInputLayouts()
		{
			string name = null;
			InputDeviceMatcher inputDeviceMatcher = default(InputDeviceMatcher);
			InputSystem.RegisterLayout<XRSimulatedHMD>(name, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("XRSimulatedHMD", true)));
			string name2 = null;
			inputDeviceMatcher = default(InputDeviceMatcher);
			InputSystem.RegisterLayout<XRSimulatedController>(name2, new InputDeviceMatcher?(inputDeviceMatcher.WithProduct("XRSimulatedController", true)));
		}
	}
}
