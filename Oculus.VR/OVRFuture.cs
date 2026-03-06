using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

internal static class OVRFuture
{
	public static OVRTask<OVRPlugin.Result> When(ulong future, CancellationToken cancellationToken = default(CancellationToken))
	{
		OVRFuture.<When>d__0 <When>d__;
		<When>d__.<>t__builder = OVRTaskBuilder<OVRPlugin.Result>.Create();
		<When>d__.future = future;
		<When>d__.cancellationToken = cancellationToken;
		<When>d__.<>1__state = -1;
		<When>d__.<>t__builder.Start<OVRFuture.<When>d__0>(ref <When>d__);
		return <When>d__.<>t__builder.Task;
	}

	[CompilerGenerated]
	internal static OVRPlugin.Result <When>g__LogIfNotSuccess|0_0(OVRPlugin.Result value, string msg)
	{
		if (!value.IsSuccess())
		{
			Debug.LogError(string.Format(msg, value));
		}
		return value;
	}

	[CompilerGenerated]
	internal static void <When>g__CheckCancellationAndThrow|0_1(ulong futureToCancel, CancellationToken token)
	{
		if (token.IsCancellationRequested)
		{
			OVRFuture.<When>g__LogIfNotSuccess|0_0(OVRPlugin.CancelFuture(futureToCancel), "Unable to cancel future: {0}");
			throw new OperationCanceledException("Future was canceled.", token);
		}
	}
}
