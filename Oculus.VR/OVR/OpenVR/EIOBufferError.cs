using System;

namespace OVR.OpenVR
{
	public enum EIOBufferError
	{
		IOBuffer_Success,
		IOBuffer_OperationFailed = 100,
		IOBuffer_InvalidHandle,
		IOBuffer_InvalidArgument,
		IOBuffer_PathExists,
		IOBuffer_PathDoesNotExist,
		IOBuffer_Permission
	}
}
