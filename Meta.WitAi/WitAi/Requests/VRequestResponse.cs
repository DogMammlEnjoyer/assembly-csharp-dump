using System;

namespace Meta.WitAi.Requests
{
	internal struct VRequestResponse<TValue>
	{
		public VRequestResponse(TValue value)
		{
			this = new VRequestResponse<TValue>(value, 200, string.Empty);
		}

		public VRequestResponse(int code, string error)
		{
			this = new VRequestResponse<TValue>(default(TValue), code, error);
		}

		public VRequestResponse(TValue value, int code, string error)
		{
			this.Value = value;
			this.Code = code;
			this.Error = error;
		}

		public readonly TValue Value;

		public int Code;

		public string Error;
	}
}
