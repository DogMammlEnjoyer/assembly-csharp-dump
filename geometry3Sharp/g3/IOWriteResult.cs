using System;

namespace g3
{
	public struct IOWriteResult
	{
		public IOCode code { readonly get; set; }

		public string message { readonly get; set; }

		public IOWriteResult(IOCode r, string s)
		{
			this = default(IOWriteResult);
			this.code = r;
			this.message = s;
			if (this.message == "")
			{
				this.message = "(no message)";
			}
		}

		public static readonly IOWriteResult Ok = new IOWriteResult(IOCode.Ok, "");
	}
}
