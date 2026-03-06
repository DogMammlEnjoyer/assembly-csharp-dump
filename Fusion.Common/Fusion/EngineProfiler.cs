using System;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace Fusion
{
	public static class EngineProfiler
	{
		[Conditional("ENABLE_PROFILER")]
		public static void Begin(string sample)
		{
			Profiler.BeginSample(sample);
		}

		[Conditional("ENABLE_PROFILER")]
		public static void End()
		{
			Profiler.EndSample();
		}

		[Conditional("ENABLE_PROFILER")]
		public static void RoundTripTime(float value)
		{
			Action<float> roundTripTimeCallback = EngineProfiler.RoundTripTimeCallback;
			if (roundTripTimeCallback != null)
			{
				roundTripTimeCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void Resimulations(int value)
		{
			Action<int> resimulationsCallback = EngineProfiler.ResimulationsCallback;
			if (resimulationsCallback != null)
			{
				resimulationsCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void WorldSnapshotSize(int value)
		{
			Action<int> worldSnapshotSizeCallback = EngineProfiler.WorldSnapshotSizeCallback;
			if (worldSnapshotSizeCallback != null)
			{
				worldSnapshotSizeCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InputSize(int value)
		{
			Action<int> inputSizeCallback = EngineProfiler.InputSizeCallback;
			if (inputSizeCallback != null)
			{
				inputSizeCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InputQueue(int value)
		{
			Action<int> inputQueueCallback = EngineProfiler.InputQueueCallback;
			if (inputQueueCallback != null)
			{
				inputQueueCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void RpcIn(int value)
		{
			Action<int> rpcInCallback = EngineProfiler.RpcInCallback;
			if (rpcInCallback != null)
			{
				rpcInCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void RpcOut(int value)
		{
			Action<int> rpcOutCallback = EngineProfiler.RpcOutCallback;
			if (rpcOutCallback != null)
			{
				rpcOutCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void StateRecvDelta(float value)
		{
			Action<float> stateRecvDeltaCallback = EngineProfiler.StateRecvDeltaCallback;
			if (stateRecvDeltaCallback != null)
			{
				stateRecvDeltaCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void StateRecvDeltaDeviation(float value)
		{
			Action<float> stateRecvDeltaDeviationCallback = EngineProfiler.StateRecvDeltaDeviationCallback;
			if (stateRecvDeltaDeviationCallback != null)
			{
				stateRecvDeltaDeviationCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InterpolationSpeed(float value)
		{
			Action<float> interpolationSpeedCallback = EngineProfiler.InterpolationSpeedCallback;
			if (interpolationSpeedCallback != null)
			{
				interpolationSpeedCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InterpolationOffset(float value)
		{
			Action<float> interpolationOffsetCallback = EngineProfiler.InterpolationOffsetCallback;
			if (interpolationOffsetCallback != null)
			{
				interpolationOffsetCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InterpolationOffsetDeviation(float value)
		{
			Action<float> interpolationOffsetDeviationCallback = EngineProfiler.InterpolationOffsetDeviationCallback;
			if (interpolationOffsetDeviationCallback != null)
			{
				interpolationOffsetDeviationCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InputRecvDelta(float value)
		{
			Action<float> inputRecvDeltaCallback = EngineProfiler.InputRecvDeltaCallback;
			if (inputRecvDeltaCallback != null)
			{
				inputRecvDeltaCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void InputRecvDeltaDeviation(float value)
		{
			Action<float> inputRecvDeltaDeviationCallback = EngineProfiler.InputRecvDeltaDeviationCallback;
			if (inputRecvDeltaDeviationCallback != null)
			{
				inputRecvDeltaDeviationCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void SimulationSpeed(float value)
		{
			Action<float> simulationSpeedCallback = EngineProfiler.SimulationSpeedCallback;
			if (simulationSpeedCallback != null)
			{
				simulationSpeedCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void SimulationOffset(float value)
		{
			Action<float> simulationOffsetCallback = EngineProfiler.SimulationOffsetCallback;
			if (simulationOffsetCallback != null)
			{
				simulationOffsetCallback(value);
			}
		}

		[Conditional("ENABLE_PROFILER")]
		public static void SimulationOffsetDeviation(float value)
		{
			Action<float> simulationOffsetDeviationCallback = EngineProfiler.SimulationOffsetDeviationCallback;
			if (simulationOffsetDeviationCallback != null)
			{
				simulationOffsetDeviationCallback(value);
			}
		}

		public static Action<float> RoundTripTimeCallback;

		public static Action<int> ResimulationsCallback;

		public static Action<int> WorldSnapshotSizeCallback;

		public static Action<int> InputSizeCallback;

		public static Action<int> InputQueueCallback;

		public static Action<int> RpcInCallback;

		public static Action<int> RpcOutCallback;

		public static Action<float> StateRecvDeltaCallback;

		public static Action<float> StateRecvDeltaDeviationCallback;

		public static Action<float> InterpolationSpeedCallback;

		public static Action<float> InterpolationOffsetCallback;

		public static Action<float> InterpolationOffsetDeviationCallback;

		public static Action<float> InputRecvDeltaCallback;

		public static Action<float> SimulationSpeedCallback;

		public static Action<float> InputRecvDeltaDeviationCallback;

		public static Action<float> SimulationOffsetCallback;

		public static Action<float> SimulationOffsetDeviationCallback;
	}
}
