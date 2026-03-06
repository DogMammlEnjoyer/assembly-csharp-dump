using System;
using System.Runtime.ConstrainedExecution;
using System.Security;

namespace System.Diagnostics.Contracts
{
	/// <summary>Provides methods and data for the <see cref="E:System.Diagnostics.Contracts.Contract.ContractFailed" /> event.</summary>
	public sealed class ContractFailedEventArgs : EventArgs
	{
		/// <summary>Provides data for the <see cref="E:System.Diagnostics.Contracts.Contract.ContractFailed" /> event.</summary>
		/// <param name="failureKind">One of the enumeration values that specifies the contract that failed.</param>
		/// <param name="message">The message for the event.</param>
		/// <param name="condition">The condition for the event.</param>
		/// <param name="originalException">The exception that caused the event.</param>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public ContractFailedEventArgs(ContractFailureKind failureKind, string message, string condition, Exception originalException)
		{
			this._failureKind = failureKind;
			this._message = message;
			this._condition = condition;
			this._originalException = originalException;
		}

		/// <summary>Gets the message that describes the <see cref="E:System.Diagnostics.Contracts.Contract.ContractFailed" /> event.</summary>
		/// <returns>The message that describes the event.</returns>
		public string Message
		{
			get
			{
				return this._message;
			}
		}

		/// <summary>Gets the condition for the failure of the contract.</summary>
		/// <returns>The condition for the failure.</returns>
		public string Condition
		{
			get
			{
				return this._condition;
			}
		}

		/// <summary>Gets the type of contract that failed.</summary>
		/// <returns>One of the enumeration values that specifies the type of contract that failed.</returns>
		public ContractFailureKind FailureKind
		{
			get
			{
				return this._failureKind;
			}
		}

		/// <summary>Gets the original exception that caused the <see cref="E:System.Diagnostics.Contracts.Contract.ContractFailed" /> event.</summary>
		/// <returns>The exception that caused the event.</returns>
		public Exception OriginalException
		{
			get
			{
				return this._originalException;
			}
		}

		/// <summary>Indicates whether the <see cref="E:System.Diagnostics.Contracts.Contract.ContractFailed" /> event has been handled.</summary>
		/// <returns>
		///   <see langword="true" /> if the event has been handled; otherwise, <see langword="false" />.</returns>
		public bool Handled
		{
			get
			{
				return this._handled;
			}
		}

		/// <summary>Sets the <see cref="P:System.Diagnostics.Contracts.ContractFailedEventArgs.Handled" /> property to <see langword="true" />.</summary>
		[SecurityCritical]
		public void SetHandled()
		{
			this._handled = true;
		}

		/// <summary>Indicates whether the code contract escalation policy should be applied.</summary>
		/// <returns>
		///   <see langword="true" /> to apply the escalation policy; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool Unwind
		{
			get
			{
				return this._unwind;
			}
		}

		/// <summary>Sets the <see cref="P:System.Diagnostics.Contracts.ContractFailedEventArgs.Unwind" /> property to <see langword="true" />.</summary>
		[SecurityCritical]
		public void SetUnwind()
		{
			this._unwind = true;
		}

		private ContractFailureKind _failureKind;

		private string _message;

		private string _condition;

		private Exception _originalException;

		private bool _handled;

		private bool _unwind;

		internal Exception thrownDuringHandler;
	}
}
