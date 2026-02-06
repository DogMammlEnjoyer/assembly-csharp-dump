using System;

public class MothershipAuthRefreshRequiredCallback : AuthRefreshRequiredDelegateWrapper
{
	public MothershipAuthRefreshRequiredCallback(Action<string> authRefreshFunction)
	{
		this.swigCMemOwn = false;
		this._authRefreshFunction = authRefreshFunction;
	}

	public override void AuthRefreshRequired(string arg0)
	{
		if (this._authRefreshFunction != null)
		{
			this._authRefreshFunction(arg0);
		}
	}

	private Action<string> _authRefreshFunction;
}
