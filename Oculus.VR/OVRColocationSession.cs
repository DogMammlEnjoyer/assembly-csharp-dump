using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class OVRColocationSession
{
	public static event Action<OVRColocationSession.Data> ColocationSessionDiscovered;

	public unsafe static OVRTask<OVRResult<Guid, OVRColocationSession.Result>> StartAdvertisementAsync(ReadOnlySpan<byte> colocationSessionData)
	{
		if (colocationSessionData.Length > 1024)
		{
			throw new ArgumentException(string.Format("Colocation Session Advertisement can only store up to {0} bytes of data", 1024));
		}
		OVRPlugin.ColocationSessionStartAdvertisementInfo info = default(OVRPlugin.ColocationSessionStartAdvertisementInfo);
		fixed (byte* pinnableReference = colocationSessionData.GetPinnableReference())
		{
			byte* groupMetadata = pinnableReference;
			info.GroupMetadata = groupMetadata;
			info.PeerMetadataCount = (uint)colocationSessionData.Length;
			ulong requestId;
			return OVRTask.Build(OVRPlugin.StartColocationSessionAdvertisement(info, out requestId), requestId).ToTask<Guid, OVRColocationSession.Result>();
		}
	}

	public static OVRTask<OVRResult<OVRColocationSession.Result>> StopAdvertisementAsync()
	{
		ulong requestId;
		return OVRTask.Build(OVRPlugin.StopColocationSessionAdvertisement(out requestId), requestId).ToResultTask<OVRColocationSession.Result>();
	}

	public static OVRTask<OVRResult<OVRColocationSession.Result>> StartDiscoveryAsync()
	{
		ulong requestId;
		return OVRTask.Build(OVRPlugin.StartColocationSessionDiscovery(out requestId), requestId).ToResultTask<OVRColocationSession.Result>();
	}

	public static OVRTask<OVRResult<OVRColocationSession.Result>> StopDiscoveryAsync()
	{
		ulong requestId;
		return OVRTask.Build(OVRPlugin.StopColocationSessionDiscovery(out requestId), requestId).ToResultTask<OVRColocationSession.Result>();
	}

	internal static void OnColocationSessionStartAdvertisementComplete(ulong requestId, OVRPlugin.Result result, Guid uuid)
	{
		OVRTask.SetResult<OVRResult<Guid, OVRColocationSession.Result>>(requestId, OVRResult<Guid, OVRColocationSession.Result>.From(uuid, (OVRColocationSession.Result)result));
	}

	internal static void OnColocationSessionStopAdvertisementComplete(ulong requestId, OVRPlugin.Result result)
	{
		OVRTask.SetResult<OVRResult<OVRColocationSession.Result>>(requestId, OVRResult<OVRColocationSession.Result>.From((OVRColocationSession.Result)result));
	}

	internal static void OnColocationSessionStartDiscoveryComplete(ulong requestId, OVRPlugin.Result result)
	{
		OVRTask.SetResult<OVRResult<OVRColocationSession.Result>>(requestId, OVRResult<OVRColocationSession.Result>.From((OVRColocationSession.Result)result));
	}

	internal static void OnColocationSessionStopDiscoveryComplete(ulong requestId, OVRPlugin.Result result)
	{
		OVRTask.SetResult<OVRResult<OVRColocationSession.Result>>(requestId, OVRResult<OVRColocationSession.Result>.From((OVRColocationSession.Result)result));
	}

	internal unsafe static void OnColocationSessionDiscoveryResult(ulong requestId, Guid uuid, uint metaDataCount, byte* metaDataPtr)
	{
		byte[] array = new byte[metaDataCount];
		Marshal.Copy((IntPtr)((void*)metaDataPtr), array, 0, (int)metaDataCount);
		OVRColocationSession.Data obj = new OVRColocationSession.Data
		{
			AdvertisementUuid = uuid,
			Metadata = array
		};
		Action<OVRColocationSession.Data> colocationSessionDiscovered = OVRColocationSession.ColocationSessionDiscovered;
		if (colocationSessionDiscovered == null)
		{
			return;
		}
		colocationSessionDiscovered(obj);
	}

	internal static void OnColocationSessionAdvertisementComplete(ulong requestId, OVRPlugin.Result result)
	{
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogWarning(string.Format("Colocation Session Advertisement unexpectedly completed with result: {0}", result));
		}
	}

	internal static void OnColocationSessionDiscoveryComplete(ulong requestId, OVRPlugin.Result result)
	{
		if (result != OVRPlugin.Result.Success)
		{
			Debug.LogWarning(string.Format("Colocation Session Discovery unexpectedly completed with result: {0}", result));
		}
	}

	public struct Data
	{
		public Guid AdvertisementUuid { readonly get; internal set; }

		public byte[] Metadata { readonly get; internal set; }

		public const int MaxMetadataSize = 1024;
	}

	[OVRResultStatus]
	public enum Result
	{
		Success,
		AlreadyAdvertising = 3001,
		AlreadyDiscovering,
		Failure = -1000,
		Unsupported = -1004,
		OperationFailed = -1006,
		InvalidData = -1008,
		NetworkFailed = -3002,
		NoDiscoveryMethodAvailable = -3003
	}
}
