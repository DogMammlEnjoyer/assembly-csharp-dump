using System;

internal class CallbackContainerUnique<T> : CallbackContainer<T> where T : class, ICallbackUnique
{
	public CallbackContainerUnique() : base(10)
	{
	}

	public CallbackContainerUnique(int capacity) : base(capacity)
	{
	}

	public override void Add(in T item)
	{
		T t = item;
		if (t.Registered)
		{
			return;
		}
		base.Add(item);
		t = item;
		t.Registered = true;
	}

	public override bool Remove(in T item)
	{
		T t = item;
		if (!t.Registered)
		{
			return false;
		}
		base.Remove(item);
		t = item;
		t.Registered = false;
		return true;
	}
}
