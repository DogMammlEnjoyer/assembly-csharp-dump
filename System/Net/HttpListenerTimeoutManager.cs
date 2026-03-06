using System;

namespace System.Net
{
	/// <summary>The timeout manager to use for an <see cref="T:System.Net.HttpListener" /> object.</summary>
	public class HttpListenerTimeoutManager
	{
		/// <summary>Gets or sets the time, in seconds, allowed for the request entity body to arrive.</summary>
		/// <returns>The time, in seconds, allowed for the request entity body to arrive.</returns>
		[MonoTODO]
		public TimeSpan EntityBody
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to drain the entity body on a Keep-Alive connection.</summary>
		/// <returns>The time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to drain the entity body on a Keep-Alive connection.</returns>
		[MonoTODO]
		public TimeSpan DrainEntityBody
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the time, in seconds, allowed for the request to remain in the request queue before the <see cref="T:System.Net.HttpListener" /> picks it up.</summary>
		/// <returns>The time, in seconds, allowed for the request to remain in the request queue before the <see cref="T:System.Net.HttpListener" /> picks it up.</returns>
		[MonoTODO]
		public TimeSpan RequestQueue
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the time, in seconds, allowed for an idle connection.</summary>
		/// <returns>The time, in seconds, allowed for an idle connection.</returns>
		[MonoTODO]
		public TimeSpan IdleConnection
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to parse the request header.</summary>
		/// <returns>The time, in seconds, allowed for the <see cref="T:System.Net.HttpListener" /> to parse the request header.</returns>
		[MonoTODO]
		public TimeSpan HeaderWait
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the minimum send rate, in bytes-per-second, for the response.</summary>
		/// <returns>The minimum send rate, in bytes-per-second, for the response.</returns>
		[MonoTODO]
		public long MinSendBytesPerSecond
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
