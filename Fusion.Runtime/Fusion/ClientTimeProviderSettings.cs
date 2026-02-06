using System;

namespace Fusion
{
	internal struct ClientTimeProviderSettings
	{
		public static ClientTimeProviderSettings Default()
		{
			TickRate.Resolved resolved = TickRate.Resolve(TickRate.Default);
			return new ClientTimeProviderSettings
			{
				TimeScaleOffsetMax = 0.05,
				SampleWindowSeconds = 1.0,
				OutgoingQuantile = 0.99,
				IncomingQuantile = 0.99,
				OutgoingRedundancy = 1.0,
				IncomingRedundancy = 1.0,
				OutgoingSendRate = resolved.ClientSend,
				IncomingSendRate = resolved.ServerSend,
				OutgoingSendDelta = resolved.ClientSendDelta,
				IncomingSendDelta = resolved.ServerSendDelta,
				PredictionMax = double.MaxValue,
				InputDelayMin = 0.0,
				InputDelayMax = 0.0,
				ClientTickRate = resolved.Client,
				ClientSimDeltaTime = resolved.ClientTickDelta,
				ServerTickRate = resolved.Server,
				ServerSimDeltaTime = resolved.ServerTickDelta
			};
		}

		public double TimeScaleOffsetMax;

		public double SampleWindowSeconds;

		public double OutgoingQuantile;

		public double IncomingQuantile;

		public double OutgoingRedundancy;

		public double IncomingRedundancy;

		public int OutgoingSendRate;

		public int IncomingSendRate;

		public double OutgoingSendDelta;

		public double IncomingSendDelta;

		public double PredictionMax;

		public double InputDelayMin;

		public double InputDelayMax;

		public int ClientTickRate;

		public double ClientSimDeltaTime;

		public int ServerTickRate;

		public double ServerSimDeltaTime;
	}
}
