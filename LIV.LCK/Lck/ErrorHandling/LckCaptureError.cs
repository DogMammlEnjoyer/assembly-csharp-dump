using System;

namespace Liv.Lck.ErrorHandling
{
	internal struct LckCaptureError
	{
		public CaptureErrorType Type { readonly get; set; }

		public string Message { readonly get; set; }

		public LckCaptureError(CaptureErrorType type, string message)
		{
			this.Type = type;
			this.Message = message;
		}
	}
}
