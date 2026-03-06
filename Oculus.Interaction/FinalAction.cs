using System;

namespace Oculus.Interaction
{
	public class FinalAction
	{
		public FinalAction(Action action)
		{
			this._action = action;
		}

		public void Cancel()
		{
			this._cancelled = true;
		}

		~FinalAction()
		{
			if (!this._cancelled)
			{
				Context.ExecuteOnMainThread(this._action);
			}
		}

		private readonly Action _action;

		private bool _cancelled;
	}
}
