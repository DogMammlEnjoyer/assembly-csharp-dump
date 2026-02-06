using System;

namespace Fusion
{
	public enum RpcLocalInvokeResult
	{
		Invoked,
		NotInvokableLocally,
		NotInvokableDuringResim,
		InsufficientSourceAuthority,
		InsufficientTargetAuthority,
		TargetPlayerIsNotLocal,
		PayloadSizeExceeded,
		[Obsolete("Use TargetPlayerIsNotLocal instead")]
		TagetPlayerIsNotLocal = 5
	}
}
