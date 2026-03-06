using System;

internal static class OVRResult
{
	public static OVRResult<TStatus> From<TStatus>(TStatus status) where TStatus : struct, Enum, IConvertible
	{
		return OVRResult<TStatus>.From(status);
	}

	public static OVRResult<TResult, TStatus> From<TResult, TStatus>(TResult result, TStatus status) where TStatus : struct, Enum, IConvertible
	{
		return OVRResult<TResult, TStatus>.From(result, status);
	}
}
