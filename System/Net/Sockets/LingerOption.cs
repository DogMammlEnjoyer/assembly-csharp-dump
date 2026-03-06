using System;

namespace System.Net.Sockets
{
	/// <summary>Specifies whether a <see cref="T:System.Net.Sockets.Socket" /> will remain connected after a call to the <see cref="M:System.Net.Sockets.Socket.Close" /> or <see cref="M:System.Net.Sockets.TcpClient.Close" /> methods and the length of time it will remain connected, if data remains to be sent.</summary>
	public class LingerOption
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Sockets.LingerOption" /> class.</summary>
		/// <param name="enable">
		///   <see langword="true" /> to remain connected after the <see cref="M:System.Net.Sockets.Socket.Close" /> method is called; otherwise, <see langword="false" />.</param>
		/// <param name="seconds">The number of seconds to remain connected after the <see cref="M:System.Net.Sockets.Socket.Close" /> method is called.</param>
		public LingerOption(bool enable, int seconds)
		{
			this.Enabled = enable;
			this.LingerTime = seconds;
		}

		/// <summary>Gets or sets a value that indicates whether to linger after the <see cref="T:System.Net.Sockets.Socket" /> is closed.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Net.Sockets.Socket" /> should linger after <see cref="M:System.Net.Sockets.Socket.Close" /> is called; otherwise, <see langword="false" />.</returns>
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		/// <summary>Gets or sets the amount of time to remain connected after calling the <see cref="M:System.Net.Sockets.Socket.Close" /> method if data remains to be sent.</summary>
		/// <returns>The amount of time, in seconds, to remain connected after calling <see cref="M:System.Net.Sockets.Socket.Close" />.</returns>
		public int LingerTime
		{
			get
			{
				return this.lingerTime;
			}
			set
			{
				this.lingerTime = value;
			}
		}

		private bool enabled;

		private int lingerTime;
	}
}
