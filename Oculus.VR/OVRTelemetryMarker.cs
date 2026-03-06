using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

internal struct OVRTelemetryMarker : IDisposable
{
	private OVRTelemetryMarker.OVRTelemetryMarkerState State { readonly get; set; }

	public bool Sent
	{
		get
		{
			return this.State.Sent;
		}
	}

	public OVRPlugin.Qpl.ResultType Result
	{
		get
		{
			return this.State.Result;
		}
	}

	public readonly int MarkerId { get; }

	public readonly int InstanceKey { get; }

	public OVRTelemetryMarker(int markerId, int instanceKey = 0, long timestampMs = -1L, string joindId = null)
	{
		this = new OVRTelemetryMarker(OVRTelemetry.Client, markerId, instanceKey, timestampMs, joindId);
	}

	internal OVRTelemetryMarker(OVRTelemetry.TelemetryClient client, int markerId, int instanceKey = 0, long timestampMs = -1L, string joinId = null)
	{
		this.MarkerId = markerId;
		this.InstanceKey = instanceKey;
		this._client = client;
		this.State = new OVRTelemetryMarker.OVRTelemetryMarkerState(false, OVRPlugin.Qpl.ResultType.Success);
		this._client.MarkerStart(markerId, instanceKey, timestampMs, joinId);
	}

	public OVRTelemetryMarker SetResult(OVRPlugin.Qpl.ResultType result)
	{
		this.State = new OVRTelemetryMarker.OVRTelemetryMarkerState(this.Sent, result);
		return this;
	}

	public OVRTelemetryMarker AddAnnotation(string annotationKey, string annotationValue)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		if (annotationValue == null)
		{
			annotationValue = string.Empty;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValue, this.InstanceKey);
		return this;
	}

	public OVRTelemetryMarker AddAnnotation(string annotationKey, bool annotationValue)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValue, this.InstanceKey);
		return this;
	}

	public OVRTelemetryMarker AddAnnotation(string annotationKey, double annotationValue)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValue, this.InstanceKey);
		return this;
	}

	public OVRTelemetryMarker AddAnnotation(string annotationKey, long annotationValue)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValue, this.InstanceKey);
		return this;
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, byte** annotationValues, int count)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValues, count, this.InstanceKey);
		return this;
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, ReadOnlySpan<long> annotationValues)
	{
		fixed (long* pinnableReference = annotationValues.GetPinnableReference())
		{
			long* annotationValues2 = pinnableReference;
			return this.AddAnnotation(annotationKey, annotationValues2, annotationValues.Length);
		}
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, long* annotationValues, int count)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValues, count, this.InstanceKey);
		return this;
	}

	public unsafe OVRTelemetryMarker AddAnnotation<[IsUnmanaged] T>(string annotationKey, ReadOnlySpan<T> annotationValues) where T : struct, ValueType, Enum
	{
		Type underlyingType = Enum.GetUnderlyingType(typeof(T));
		if (underlyingType == typeof(long) || underlyingType == typeof(ulong))
		{
			fixed (T* pinnableReference = annotationValues.GetPinnableReference())
			{
				T* annotationValues2 = pinnableReference;
				return this.AddAnnotation(annotationKey, (long*)annotationValues2, annotationValues.Length);
			}
		}
		OVRTelemetryMarker result;
		using (NativeArray<long> nativeArray = new NativeArray<long>(annotationValues.Length, Allocator.Temp, NativeArrayOptions.ClearMemory))
		{
			for (int i = 0; i < annotationValues.Length; i++)
			{
				nativeArray[i] = (long)UnsafeUtility.EnumToInt<T>(*annotationValues[i]);
			}
			result = this.AddAnnotation(annotationKey, (long*)nativeArray.GetUnsafeReadOnlyPtr<long>(), nativeArray.Length);
		}
		return result;
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, ReadOnlySpan<double> annotationValues)
	{
		fixed (double* pinnableReference = annotationValues.GetPinnableReference())
		{
			double* annotationValues2 = pinnableReference;
			return this.AddAnnotation(annotationKey, annotationValues2, annotationValues.Length);
		}
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, double* annotationValues, int count)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValues, count, this.InstanceKey);
		return this;
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, ReadOnlySpan<OVRPlugin.Bool> annotationValues)
	{
		fixed (OVRPlugin.Bool* pinnableReference = annotationValues.GetPinnableReference())
		{
			OVRPlugin.Bool* annotationValues2 = pinnableReference;
			return this.AddAnnotation(annotationKey, annotationValues2, annotationValues.Length);
		}
	}

	public unsafe OVRTelemetryMarker AddAnnotation(string annotationKey, OVRPlugin.Bool* annotationValues, int count)
	{
		if (string.IsNullOrEmpty(annotationKey))
		{
			return this;
		}
		this._client.MarkerAnnotation(this.MarkerId, annotationKey, annotationValues, count, this.InstanceKey);
		return this;
	}

	public OVRTelemetryMarker AddAnnotationIfNotNullOrEmpty(string annotationKey, string annotationValue)
	{
		if (!string.IsNullOrEmpty(annotationValue))
		{
			return this.AddAnnotation(annotationKey, annotationValue);
		}
		return this;
	}

	private static string ApplicationIdentifier
	{
		get
		{
			string result;
			if ((result = OVRTelemetryMarker._applicationIdentifier) == null)
			{
				result = (OVRTelemetryMarker._applicationIdentifier = Application.identifier);
			}
			return result;
		}
	}

	private static string UnityVersion
	{
		get
		{
			string result;
			if ((result = OVRTelemetryMarker._unityVersion) == null)
			{
				result = (OVRTelemetryMarker._unityVersion = Application.unityVersion);
			}
			return result;
		}
	}

	private static bool IsBatchMode
	{
		get
		{
			bool flag = OVRTelemetryMarker._isBatchMode.GetValueOrDefault();
			if (OVRTelemetryMarker._isBatchMode == null)
			{
				flag = Application.isBatchMode;
				OVRTelemetryMarker._isBatchMode = new bool?(flag);
				return flag;
			}
			return flag;
		}
	}

	private bool GetOVRTelemetryConsent()
	{
		return false;
	}

	public OVRTelemetryMarker Send()
	{
		this.AddAnnotation("ProjectName", OVRTelemetryMarker.ApplicationIdentifier);
		this.AddAnnotation("ProjectGuid", OVRRuntimeSettings.Instance.TelemetryProjectGuid);
		this.AddAnnotation("BatchMode", OVRTelemetryMarker.IsBatchMode);
		this.AddAnnotation("ProcessorType", SystemInfo.processorType);
		this.State = new OVRTelemetryMarker.OVRTelemetryMarkerState(true, this.Result);
		this._client.MarkerEnd(this.MarkerId, this.Result, this.InstanceKey, -1L);
		return this;
	}

	public OVRTelemetryMarker SendIf(bool condition)
	{
		if (condition)
		{
			return this.Send();
		}
		this.State = new OVRTelemetryMarker.OVRTelemetryMarkerState(true, this.Result);
		return this;
	}

	public OVRTelemetryMarker AddPoint(OVRTelemetry.MarkerPoint point)
	{
		this._client.MarkerPointCached(this.MarkerId, point.NameHandle, this.InstanceKey, -1L);
		return this;
	}

	public OVRTelemetryMarker AddPoint(string name)
	{
		this._client.MarkerPoint(this.MarkerId, name, this.InstanceKey, -1L);
		return this;
	}

	public unsafe OVRTelemetryMarker AddPoint(string name, OVRPlugin.Qpl.Annotation.Builder annotationBuilder)
	{
		OVRTelemetryMarker result;
		using (NativeArray<OVRPlugin.Qpl.Annotation> nativeArray = annotationBuilder.ToNativeArray(Allocator.Temp))
		{
			result = this.AddPoint(name, (OVRPlugin.Qpl.Annotation*)nativeArray.GetUnsafeReadOnlyPtr<OVRPlugin.Qpl.Annotation>(), nativeArray.Length);
		}
		return result;
	}

	public unsafe OVRTelemetryMarker AddPoint(string name, OVRPlugin.Qpl.Annotation* annotations, int annotationCount)
	{
		this._client.MarkerPoint(this.MarkerId, name, annotations, annotationCount, this.InstanceKey, -1L);
		return this;
	}

	public void Dispose()
	{
		if (!this.Sent)
		{
			this.Send();
		}
	}

	private readonly OVRTelemetry.TelemetryClient _client;

	private static string _applicationIdentifier;

	private static string _unityVersion;

	private static bool? _isBatchMode;

	private const string TelemetryEnabledKey = "OVRTelemetry.TelemetryEnabled";

	internal struct OVRTelemetryMarkerState
	{
		public bool Sent { readonly get; set; }

		public OVRPlugin.Qpl.ResultType Result { readonly get; set; }

		public OVRTelemetryMarkerState(bool sent, OVRPlugin.Qpl.ResultType result)
		{
			this.Result = result;
			this.Sent = sent;
		}
	}
}
