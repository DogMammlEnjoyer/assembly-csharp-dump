using System;
using System.Threading;

namespace System
{
	/// <summary>Represents the status of an asynchronous operation.</summary>
	public interface IAsyncResult
	{
		/// <summary>Gets a value that indicates whether the asynchronous operation has completed.</summary>
		/// <returns>
		///   <see langword="true" /> if the operation is complete; otherwise, <see langword="false" />.</returns>
		bool IsCompleted { get; }

		/// <summary>Gets a <see cref="T:System.Threading.WaitHandle" /> that is used to wait for an asynchronous operation to complete.</summary>
		/// <returns>A <see cref="T:System.Threading.WaitHandle" /> that is used to wait for an asynchronous operation to complete.</returns>
		WaitHandle AsyncWaitHandle { get; }

		/// <summary>Gets a user-defined object that qualifies or contains information about an asynchronous operation.</summary>
		/// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
		object AsyncState { get; }

		/// <summary>Gets a value that indicates whether the asynchronous operation completed synchronously.</summary>
		/// <returns>
		///   <see langword="true" /> if the asynchronous operation completed synchronously; otherwise, <see langword="false" />.</returns>
		bool CompletedSynchronously { get; }
	}
}
