using System;
using UnityEngine;

internal class OVRSampledEventSender
{
	public OVRSampledEventSender(int markerId, float recordRecordChance, Func<OVRTelemetryMarker, OVRTelemetryMarker> addAnnotationsFunc = null)
	{
		this._markerId = markerId;
		this._recordChance = recordRecordChance;
		this._addAnnotationsFunc = addAnnotationsFunc;
	}

	public void Send()
	{
		if (!this._shouldSend)
		{
			return;
		}
		this._marker.Send();
		this._shouldSend = false;
	}

	public void Start()
	{
		if (!OVRSampledEventSender.ShouldSendEvent(this._recordChance))
		{
			return;
		}
		this._marker = OVRTelemetry.Start(this._markerId, 0, -1L);
		if (this._addAnnotationsFunc != null)
		{
			this._marker = this._addAnnotationsFunc(this._marker);
		}
		this._shouldSend = true;
	}

	private static bool ShouldSendEvent(float chance)
	{
		return Random.value < chance;
	}

	private OVRTelemetryMarker _marker;

	private bool _shouldSend;

	private readonly float _recordChance;

	private readonly int _markerId;

	private readonly Func<OVRTelemetryMarker, OVRTelemetryMarker> _addAnnotationsFunc;
}
