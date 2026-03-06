using System;
using System.IO;

namespace System.Net.Security
{
	/// <summary>Provides methods for passing credentials across a stream and requesting or performing authentication for client-server applications.</summary>
	public abstract class AuthenticatedStream : Stream
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Security.AuthenticatedStream" /> class.</summary>
		/// <param name="innerStream">A <see cref="T:System.IO.Stream" /> object used by the <see cref="T:System.Net.Security.AuthenticatedStream" /> for sending and receiving data.</param>
		/// <param name="leaveInnerStreamOpen">A <see cref="T:System.Boolean" /> that indicates whether closing this <see cref="T:System.Net.Security.AuthenticatedStream" /> object also closes <paramref name="innerStream" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="innerStream" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="innerStream" /> is equal to <see cref="F:System.IO.Stream.Null" />.</exception>
		protected AuthenticatedStream(Stream innerStream, bool leaveInnerStreamOpen)
		{
			if (innerStream == null || innerStream == Stream.Null)
			{
				throw new ArgumentNullException("innerStream");
			}
			if (!innerStream.CanRead || !innerStream.CanWrite)
			{
				throw new ArgumentException(SR.GetString("The stream has to be read/write."), "innerStream");
			}
			this._InnerStream = innerStream;
			this._LeaveStreamOpen = leaveInnerStreamOpen;
		}

		/// <summary>Gets whether the stream used by this <see cref="T:System.Net.Security.AuthenticatedStream" /> for sending and receiving data has been left open.</summary>
		/// <returns>
		///   <see langword="true" /> if the inner stream has been left open; otherwise, <see langword="false" />.</returns>
		public bool LeaveInnerStreamOpen
		{
			get
			{
				return this._LeaveStreamOpen;
			}
		}

		/// <summary>Gets the stream used by this <see cref="T:System.Net.Security.AuthenticatedStream" /> for sending and receiving data.</summary>
		/// <returns>A <see cref="T:System.IO.Stream" /> object.</returns>
		protected Stream InnerStream
		{
			get
			{
				return this._InnerStream;
			}
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Security.AuthenticatedStream" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					if (this._LeaveStreamOpen)
					{
						this._InnerStream.Flush();
					}
					else
					{
						this._InnerStream.Close();
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether authentication was successful.</summary>
		/// <returns>
		///   <see langword="true" /> if successful authentication occurred; otherwise, <see langword="false" />.</returns>
		public abstract bool IsAuthenticated { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether both server and client have been authenticated.</summary>
		/// <returns>
		///   <see langword="true" /> if the client and server have been authenticated; otherwise, <see langword="false" />.</returns>
		public abstract bool IsMutuallyAuthenticated { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether data sent using this <see cref="T:System.Net.Security.AuthenticatedStream" /> is encrypted.</summary>
		/// <returns>
		///   <see langword="true" /> if data is encrypted before being transmitted over the network and decrypted when it reaches the remote endpoint; otherwise, <see langword="false" />.</returns>
		public abstract bool IsEncrypted { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the data sent using this stream is signed.</summary>
		/// <returns>
		///   <see langword="true" /> if the data is signed before being transmitted; otherwise, <see langword="false" />.</returns>
		public abstract bool IsSigned { get; }

		/// <summary>Gets a <see cref="T:System.Boolean" /> value that indicates whether the local side of the connection was authenticated as the server.</summary>
		/// <returns>
		///   <see langword="true" /> if the local endpoint was authenticated as the server side of a client-server authenticated connection; <see langword="false" /> if the local endpoint was authenticated as the client.</returns>
		public abstract bool IsServer { get; }

		private Stream _InnerStream;

		private bool _LeaveStreamOpen;
	}
}
