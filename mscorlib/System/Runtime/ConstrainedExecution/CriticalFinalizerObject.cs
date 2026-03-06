using System;

namespace System.Runtime.ConstrainedExecution
{
	/// <summary>Ensures that all finalization code in derived classes is marked as critical.</summary>
	public abstract class CriticalFinalizerObject
	{
		/// <summary>Releases all the resources used by the <see cref="T:System.Runtime.ConstrainedExecution.CriticalFinalizerObject" /> class.</summary>
		~CriticalFinalizerObject()
		{
		}
	}
}
