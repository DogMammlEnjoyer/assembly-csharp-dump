using System;
using POpusCodec.Enums;

namespace POpusCodec
{
	public class OpusException : Exception
	{
		public OpusStatusCode StatusCode
		{
			get
			{
				return this._statusCode;
			}
		}

		public OpusException(OpusStatusCode statusCode, string message) : base(message + " (" + statusCode.ToString() + ")")
		{
			this._statusCode = statusCode;
		}

		private OpusStatusCode _statusCode;
	}
}
