using System;

namespace g3
{
	public struct IOReadResult
	{
		public IOCode code { readonly get; set; }

		public string message { readonly get; set; }

		public IOReadResult(IOCode r, string s)
		{
			this = default(IOReadResult);
			this.code = r;
			this.message = s;
			if (this.message == "")
			{
				this.message = "(no message)";
			}
		}

		public static readonly IOReadResult Ok = new IOReadResult(IOCode.Ok, "");
	}
}
