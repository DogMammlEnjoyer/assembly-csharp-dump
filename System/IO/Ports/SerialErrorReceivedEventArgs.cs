using System;
using Unity;

namespace System.IO.Ports
{
	/// <summary>Prepares data for the <see cref="E:System.IO.Ports.SerialPort.ErrorReceived" /> event.</summary>
	public class SerialErrorReceivedEventArgs : EventArgs
	{
		internal SerialErrorReceivedEventArgs(SerialError eventType)
		{
			this.eventType = eventType;
		}

		/// <summary>Gets or sets the event type.</summary>
		/// <returns>One of the <see cref="T:System.IO.Ports.SerialError" /> values.</returns>
		public SerialError EventType
		{
			get
			{
				return this.eventType;
			}
		}

		internal SerialErrorReceivedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private SerialError eventType;
	}
}
