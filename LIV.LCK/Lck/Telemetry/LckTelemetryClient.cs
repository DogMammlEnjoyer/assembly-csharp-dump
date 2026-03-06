using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Liv.Lck.Core;
using Liv.Lck.Core.Serialization;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck.Telemetry
{
	internal class LckTelemetryClient : ILckTelemetryClient
	{
		[Preserve]
		public LckTelemetryClient(ILckSerializer serializer)
		{
			this._serializer = serializer;
		}

		public void SendErrorTelemetry(ILckResult lckResult)
		{
			Dictionary<string, object> context = new Dictionary<string, object>
			{
				{
					"error",
					lckResult.Error
				},
				{
					"errorString",
					lckResult.Error.ToString()
				},
				{
					"message",
					lckResult.Message
				}
			};
			this.SendTelemetry(new LckTelemetryEvent(LckTelemetryEventType.RecorderError, context));
		}

		public void SendTelemetry(LckTelemetryEvent lckTelemetryEvent)
		{
			if (Application.isEditor)
			{
				return;
			}
			this.SerializeAndSend(lckTelemetryEvent);
		}

		private void SerializeAndSend(LckTelemetryEvent lckTelemetryEvent)
		{
			byte[] array = this._serializer.Serialize(lckTelemetryEvent.Context);
			IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
			try
			{
				Marshal.Copy(array, 0, intPtr, array.Length);
				TelemetryReturnCode telemetryReturnCode = LckCoreTelemetryNative.send_telemetry_event_with_context(lckTelemetryEvent.EventType, intPtr, (UIntPtr)((ulong)((long)array.Length)), this._serializer.SerializationType);
				if (telemetryReturnCode != TelemetryReturnCode.Ok)
				{
					LckLog.LogError(string.Format("Failed to send telemetry event: {0} (return code={1})", lckTelemetryEvent.EventType, telemetryReturnCode));
				}
			}
			catch (Exception arg)
			{
				LckLog.LogError(string.Format("Failed to send telemetry event: {0}. Exception: {1}", lckTelemetryEvent.EventType, arg));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		private readonly ILckSerializer _serializer;
	}
}
