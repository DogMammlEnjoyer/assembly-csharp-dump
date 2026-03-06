using System;

namespace g3
{
	public class CancelFunction : ICancelSource
	{
		public CancelFunction(Func<bool> cancelF)
		{
			this.CancelF = cancelF;
		}

		public bool Cancelled()
		{
			return this.CancelF();
		}

		public Func<bool> CancelF;
	}
}
