using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Liv.Lck.Core.Serialization;
using UnityEngine;
using UnityEngine.Scripting;

namespace Liv.Lck.Core
{
	[Preserve]
	internal class LckTelemetryContextProvider : ILckTelemetryContextProvider
	{
		[Preserve]
		public LckTelemetryContextProvider()
		{
		}

		public void SetTelemetryContext(LckTelemetryContextType contextType, Dictionary<string, object> context)
		{
			if (context == null || !context.Any<KeyValuePair<string, object>>())
			{
				this.ClearTelemetryContext(contextType);
				return;
			}
			byte[] array = this._serializer.Serialize(context);
			IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
			try
			{
				Marshal.Copy(array, 0, intPtr, array.Length);
				TelemetryReturnCode telemetryReturnCode = LckCoreTelemetryNative.set_telemetry_context_from_serialized_data(contextType, intPtr, (UIntPtr)((ulong)((long)array.Length)), this._serializer.SerializationType);
				if (telemetryReturnCode != TelemetryReturnCode.Ok)
				{
					Debug.LogError(string.Format("Failed to set telemetry context (return code={0})", telemetryReturnCode));
				}
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Failed to set telemetry context: {0}", arg));
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
		}

		public void ClearTelemetryContext(LckTelemetryContextType contextType)
		{
			TelemetryReturnCode telemetryReturnCode = LckCoreTelemetryNative.clear_context(contextType);
			if (telemetryReturnCode != TelemetryReturnCode.Ok)
			{
				Debug.LogError(string.Format("Failed to clear telemetry context (return code={0})", telemetryReturnCode));
			}
		}

		private readonly ILckSerializer _serializer = new LckMsgPackSerializer();
	}
}
