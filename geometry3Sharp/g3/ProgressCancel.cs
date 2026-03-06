using System;

namespace g3
{
	public class ProgressCancel
	{
		public ProgressCancel(ICancelSource source)
		{
			this.Source = source;
		}

		public ProgressCancel(Func<bool> cancelF)
		{
			this.Source = new CancelFunction(cancelF);
		}

		public bool Cancelled()
		{
			if (this.WasCancelled)
			{
				return true;
			}
			this.WasCancelled = this.Source.Cancelled();
			return this.WasCancelled;
		}

		public ICancelSource Source;

		private bool WasCancelled;
	}
}
