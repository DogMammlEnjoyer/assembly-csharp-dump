using System;

namespace Liv.Lck.Telemetry
{
	public interface ILckTelemetryClient
	{
		void SendTelemetry(LckTelemetryEvent lckTelemetryEvent);

		void SendErrorTelemetry(ILckResult lckResult);
	}
}
