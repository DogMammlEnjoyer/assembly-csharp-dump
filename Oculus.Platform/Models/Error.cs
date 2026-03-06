using System;

namespace Oculus.Platform.Models
{
	public class Error
	{
		public Error(int code, string message, int httpCode)
		{
			this.Message = message;
			this.Code = code;
			this.HttpCode = httpCode;
		}

		public readonly int Code;

		public readonly int HttpCode;

		public readonly string Message;
	}
}
