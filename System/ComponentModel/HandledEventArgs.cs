using System;

namespace System.ComponentModel
{
	/// <summary>Provides data for events that can be handled completely in an event handler.</summary>
	public class HandledEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.HandledEventArgs" /> class with a default <see cref="P:System.ComponentModel.HandledEventArgs.Handled" /> property value of <see langword="false" />.</summary>
		public HandledEventArgs() : this(false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.HandledEventArgs" /> class with the specified default value for the <see cref="P:System.ComponentModel.HandledEventArgs.Handled" /> property.</summary>
		/// <param name="defaultHandledValue">The default value for the <see cref="P:System.ComponentModel.HandledEventArgs.Handled" /> property.</param>
		public HandledEventArgs(bool defaultHandledValue)
		{
			this.Handled = defaultHandledValue;
		}

		/// <summary>Gets or sets a value that indicates whether the event handler has completely handled the event or whether the system should continue its own processing.</summary>
		/// <returns>
		///   <see langword="true" /> if the event has been completely handled; otherwise, <see langword="false" />.</returns>
		public bool Handled { get; set; }
	}
}
