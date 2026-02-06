using System;
using Fusion.Statistics;

namespace Fusion
{
	internal class ServerTimeProvider : ITimeProvider
	{
		internal ServerTimeProvider()
		{
			this._settings = ServerTimeProviderSettings.Default();
		}

		internal ServerTimeProvider(ServerTimeProviderSettings settings)
		{
			this._settings = settings;
		}

		private void Reset(Tick snapshot)
		{
			this._time = (double)snapshot * this._settings.SimDeltaTime;
		}

		private void Update(double unscaledDeltaTime)
		{
			this._time += unscaledDeltaTime;
		}

		bool ITimeProvider.IsRunning()
		{
			return true;
		}

		void ITimeProvider.Configure(SimulationRuntimeConfig src)
		{
			this._settings.SimDeltaTime = src.TickRate.ClientTickDelta;
		}

		void ITimeProvider.Configure(TimeSyncConfiguration tsc)
		{
		}

		void ITimeProvider.Reset(double roundTripTime, Tick snapshot)
		{
			this.Reset(snapshot);
		}

		void ITimeProvider.Snap()
		{
		}

		void ITimeProvider.Update(double unscaledDeltaTime)
		{
			this.Update(unscaledDeltaTime);
		}

		void ITimeProvider.OnSnapshotReceived(double roundTripTime, Tick snapshot)
		{
		}

		void ITimeProvider.OnFeedbackReceived(Simulation.TimeFeedback feedback)
		{
		}

		void ITimeProvider.ResetFeedback()
		{
		}

		Instant ITimeProvider.Now()
		{
			return new Instant
			{
				Input = this._time,
				Local = this._time,
				Remote = this._time
			};
		}

		void ITimeProvider.Log(FusionStatisticsManager stats)
		{
		}

		void ITimeProvider.SetPlayerIndex(int index)
		{
		}

		void ITimeProvider.StartTrace()
		{
		}

		void ITimeProvider.StopTrace()
		{
		}

		private ServerTimeProviderSettings _settings;

		private double _time;
	}
}
