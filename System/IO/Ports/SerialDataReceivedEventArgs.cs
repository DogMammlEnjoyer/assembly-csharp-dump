using System;
using Unity;

namespace System.IO.Ports
{
	/// <summary>Provides data for the <see cref="E:System.IO.Ports.SerialPort.DataReceived" /> event.</summary>
	public class SerialDataReceivedEventArgs : EventArgs
	{
		internal SerialDataReceivedEventArgs(SerialData eventType)
		{
			this.eventType = eventType;
		}

		/// <summary>Gets or sets the event type.</summary>
		/// <returns>One of the <see cref="T:System.IO.Ports.SerialData" /> values.</returns>
		public SerialData EventType
		{
			get
			{
				return this.eventType;
			}
		}

		internal SerialDataReceivedEventArgs()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private SerialData eventType;
	}
}
