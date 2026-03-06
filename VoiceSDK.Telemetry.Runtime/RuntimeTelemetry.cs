using System;
using System.Collections.Generic;

namespace Meta.Voice.TelemetryUtilities
{
	public class RuntimeTelemetry : ITelemetryWriter
	{
		internal RuntimeTelemetry()
		{
		}

		public static RuntimeTelemetry Instance { get; } = new RuntimeTelemetry();

		public void RegisterWriter(ITelemetryWriter writer)
		{
			this._writers.Add(writer);
		}

		public void StartEvent(OperationID operationId, RuntimeTelemetryEventType runtimeTelemetryEventType)
		{
			foreach (ITelemetryWriter telemetryWriter in this._writers)
			{
				telemetryWriter.StartEvent(operationId, runtimeTelemetryEventType);
			}
		}

		public void LogEventTermination(OperationID operationId, TerminationReason reason = TerminationReason.Successful, string message = "")
		{
			foreach (ITelemetryWriter telemetryWriter in this._writers)
			{
				telemetryWriter.LogEventTermination(operationId, reason, message);
			}
		}

		public void LogInstantaneousEvent(OperationID operationId, RuntimeTelemetryEventType runtimeTelemetryEventType, Dictionary<string, string> annotations = null)
		{
			foreach (ITelemetryWriter telemetryWriter in this._writers)
			{
				telemetryWriter.LogInstantaneousEvent(operationId, runtimeTelemetryEventType, annotations);
			}
		}

		public void LogPoint(OperationID operationId, RuntimeTelemetryPoint point)
		{
			foreach (ITelemetryWriter telemetryWriter in this._writers)
			{
				telemetryWriter.LogPoint(operationId, point);
			}
		}

		public void LogPoint(string operationId, RuntimeTelemetryPoint point)
		{
			this.LogPoint((OperationID)operationId, point);
		}

		public void AnnotateEvent(OperationID operationID, string annotationKey, string annotationValue)
		{
			foreach (ITelemetryWriter telemetryWriter in this._writers)
			{
				telemetryWriter.AnnotateEvent(operationID, annotationKey, annotationValue);
			}
		}

		private readonly List<ITelemetryWriter> _writers = new List<ITelemetryWriter>();
	}
}
