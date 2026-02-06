using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal static class ICallbacksExtensions
	{
		[Conditional("FUSION_UNITY")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeOnInput(this Simulation.ICallbacks callbacks, SimulationInput input)
		{
			callbacks.OnInput(input);
		}

		[Conditional("FUSION_UNITY")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InvokeOnInputMissing(this Simulation.ICallbacks callbacks, SimulationInput input)
		{
			callbacks.OnInputMissing(input);
		}
	}
}
