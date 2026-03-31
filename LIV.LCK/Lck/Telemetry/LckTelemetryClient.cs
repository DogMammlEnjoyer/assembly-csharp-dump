using System;
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
					LckLog.LogError(string.Format("Failed to send telemetry event: {0} (return code={1})", lckTelemetryEvent.EventType, telemetryReturnCode), "SerializeAndSend", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckTelemetryClient.cs", 51);
				}
			}
			catch (Exception arg)
			{
				LckLog.LogError(string.Format("Failed to send telemetry event: {0}. Exception: {1}", lckTelemetryEvent.EventType, arg), "SerializeAndSend", ".\\Packages\\tv.liv.lck\\Runtime\\Scripts\\Components\\LckTelemetryClient.cs", 56);
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		private readonly ILckSerializer _serializer;
	}
}
