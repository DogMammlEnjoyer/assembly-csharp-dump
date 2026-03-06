using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;

namespace System.Threading
{
	/// <summary>Provides the functionality that allows a common language runtime host to participate in the flow, or migration, of the execution context.</summary>
	public class HostExecutionContextManager
	{
		/// <summary>Captures the host execution context from the current thread.</summary>
		/// <returns>A <see cref="T:System.Threading.HostExecutionContext" /> object representing the host execution context of the current thread.</returns>
		[MonoTODO]
		public virtual HostExecutionContext Capture()
		{
			throw new NotImplementedException();
		}

		/// <summary>Restores the host execution context to its prior state.</summary>
		/// <param name="previousState">The previous context state to revert to.</param>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="previousState" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="previousState" /> was not created on the current thread.  
		/// -or-  
		/// <paramref name="previousState" /> is not the last state for the <see cref="T:System.Threading.HostExecutionContext" />.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO]
		public virtual void Revert(object previousState)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets the current host execution context to the specified host execution context.</summary>
		/// <param name="hostExecutionContext">The <see cref="T:System.Threading.HostExecutionContext" /> to be set.</param>
		/// <returns>An object for restoring the <see cref="T:System.Threading.HostExecutionContext" /> to its previous state.</returns>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="hostExecutionContext" /> was not acquired through a capture operation.  
		/// -or-  
		/// <paramref name="hostExecutionContext" /> has been the argument to a previous <see cref="M:System.Threading.HostExecutionContextManager.SetHostExecutionContext(System.Threading.HostExecutionContext)" /> method call.</exception>
		[MonoTODO]
		[SecurityPermission(SecurityAction.LinkDemand, Infrastructure = true)]
		public virtual object SetHostExecutionContext(HostExecutionContext hostExecutionContext)
		{
			throw new NotImplementedException();
		}
	}
}
