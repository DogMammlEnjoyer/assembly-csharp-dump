using System;
using System.Net.Sockets;

namespace System.Net
{
	/// <summary>Identifies a network address. This is an <see langword="abstract" /> class.</summary>
	[Serializable]
	public abstract class EndPoint
	{
		/// <summary>Gets the address family to which the endpoint belongs.</summary>
		/// <returns>One of the <see cref="T:System.Net.Sockets.AddressFamily" /> values.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to get or set the property when the property is not overridden in a descendant class.</exception>
		public virtual AddressFamily AddressFamily
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		/// <summary>Serializes endpoint information into a <see cref="T:System.Net.SocketAddress" /> instance.</summary>
		/// <returns>A <see cref="T:System.Net.SocketAddress" /> instance that contains the endpoint information.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method when the method is not overridden in a descendant class.</exception>
		public virtual SocketAddress Serialize()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		/// <summary>Creates an <see cref="T:System.Net.EndPoint" /> instance from a <see cref="T:System.Net.SocketAddress" /> instance.</summary>
		/// <param name="socketAddress">The socket address that serves as the endpoint for a connection.</param>
		/// <returns>A new <see cref="T:System.Net.EndPoint" /> instance that is initialized from the specified <see cref="T:System.Net.SocketAddress" /> instance.</returns>
		/// <exception cref="T:System.NotImplementedException">Any attempt is made to access the method when the method is not overridden in a descendant class.</exception>
		public virtual EndPoint Create(SocketAddress socketAddress)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}
	}
}
