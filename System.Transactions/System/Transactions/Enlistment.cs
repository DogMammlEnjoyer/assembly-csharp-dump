using System;

namespace System.Transactions
{
	/// <summary>Facilitates communication between an enlisted transaction participant and the transaction manager during the final phase of the transaction.</summary>
	public class Enlistment
	{
		internal Enlistment()
		{
			this.done = false;
		}

		/// <summary>Indicates that the transaction participant has completed its work.</summary>
		public void Done()
		{
			this.done = true;
			this.InternalOnDone();
		}

		internal virtual void InternalOnDone()
		{
		}

		internal bool done;
	}
}
