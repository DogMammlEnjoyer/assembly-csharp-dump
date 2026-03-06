using System;

namespace System.ComponentModel
{
	/// <summary>Provides data for a cancelable event.</summary>
	public class CancelEventArgs : EventArgs
	{
		/// <summary>Gets or sets a value indicating whether the event should be canceled.</summary>
		/// <returns>
		///   <see langword="true" /> if the event should be canceled; otherwise, <see langword="false" />.</returns>
		public bool Cancel { get; set; }

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.CancelEventArgs" /> class with the <see cref="P:System.ComponentModel.CancelEventArgs.Cancel" /> property set to <see langword="false" />.</summary>
		public CancelEventArgs()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.CancelEventArgs" /> class with the <see cref="P:System.ComponentModel.CancelEventArgs.Cancel" /> property set to the given value.</summary>
		/// <param name="cancel">
		///   <see langword="true" /> to cancel the event; otherwise, <see langword="false" />.</param>
		public CancelEventArgs(bool cancel)
		{
			this.Cancel = cancel;
		}
	}
}
