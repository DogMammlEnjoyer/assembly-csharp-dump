using System;

public interface ICallbackUnique : ICallBack
{
	bool Registered { get; set; }
}
