using System;

namespace Meta.Voice.Logging
{
	public interface IErrorMitigator
	{
		string GetMitigation(ErrorCode errorCode);

		void SetMitigation(ErrorCode errorCode, string mitigation);
	}
}
