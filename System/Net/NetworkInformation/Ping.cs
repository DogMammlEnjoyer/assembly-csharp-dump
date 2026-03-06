using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.NetworkInformation
{
	/// <summary>Allows an application to determine whether a remote computer is accessible over the network.</summary>
	[MonoTODO("IPv6 support is missing")]
	public class Ping : Component, IDisposable
	{
		/// <summary>Occurs when an asynchronous operation to send an Internet Control Message Protocol (ICMP) echo message and receive the corresponding ICMP echo reply message completes or is canceled.</summary>
		public event PingCompletedEventHandler PingCompleted;

		static Ping()
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Ping.CheckLinuxCapabilities();
				if (!Ping.canSendPrivileged && WindowsIdentity.GetCurrent().Name == "root")
				{
					Ping.canSendPrivileged = true;
				}
				foreach (string text in Ping.PingBinPaths)
				{
					if (File.Exists(text))
					{
						Ping.PingBinPath = text;
						break;
					}
				}
			}
			else
			{
				Ping.canSendPrivileged = true;
			}
			if (Ping.PingBinPath == null)
			{
				Ping.PingBinPath = "/bin/ping";
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkInformation.Ping" /> class.</summary>
		public Ping()
		{
			RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider();
			byte[] array = new byte[2];
			randomNumberGenerator.GetBytes(array);
			this.identifier = (ushort)((int)array[0] + ((int)array[1] << 8));
		}

		[DllImport("libc")]
		private static extern int capget(ref Ping.cap_user_header_t header, ref Ping.cap_user_data_t data);

		private static void CheckLinuxCapabilities()
		{
			try
			{
				Ping.cap_user_header_t cap_user_header_t = default(Ping.cap_user_header_t);
				Ping.cap_user_data_t cap_user_data_t = default(Ping.cap_user_data_t);
				cap_user_header_t.version = 429392688U;
				int num = -1;
				try
				{
					num = Ping.capget(ref cap_user_header_t, ref cap_user_data_t);
				}
				catch (Exception)
				{
				}
				if (num != -1)
				{
					Ping.canSendPrivileged = ((cap_user_data_t.effective & 8192U) > 0U);
				}
			}
			catch
			{
				Ping.canSendPrivileged = false;
			}
		}

		void IDisposable.Dispose()
		{
		}

		/// <summary>Raises the <see cref="E:System.Net.NetworkInformation.Ping.PingCompleted" /> event.</summary>
		/// <param name="e">A <see cref="T:System.Net.NetworkInformation.PingCompletedEventArgs" /> object that contains event data.</param>
		protected void OnPingCompleted(PingCompletedEventArgs e)
		{
			this.user_async_state = null;
			this.worker = null;
			if (this.cts != null)
			{
				this.cts.Dispose();
				this.cts = null;
			}
			if (this.PingCompleted != null)
			{
				this.PingCompleted(this, e);
			}
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message, if one was received, or describes the reason for the failure if no message was received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(IPAddress address)
		{
			return this.Send(address, 4000);
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer. This method allows you to specify a time-out value for the operation.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message if one was received, or provides the reason for the failure if no message was received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(IPAddress address, int timeout)
		{
			return this.Send(address, timeout, Ping.default_buffer);
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message, if one was received, or provides the reason for the failure, if no message was received. The method will return <see cref="F:System.Net.NetworkInformation.IPStatus.PacketTooBig" /> if the packet exceeds the Maximum Transmission Unit (MTU).</returns>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />, or the <paramref name="buffer" /> size is greater than 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(IPAddress address, int timeout, byte[] buffer)
		{
			return this.Send(address, timeout, buffer, new PingOptions());
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message, if one was received, or provides the reason for the failure, if no message was received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(string hostNameOrAddress)
		{
			return this.Send(hostNameOrAddress, 4000);
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This method allows you to specify a time-out value for the operation.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message if one was received, or provides the reason for the failure if no message was received.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(string hostNameOrAddress, int timeout)
		{
			return this.Send(hostNameOrAddress, timeout, Ping.default_buffer);
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message if one was received, or provides the reason for the failure if no message was received.</returns>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />, or the <paramref name="buffer" /> size is greater than 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer)
		{
			return this.Send(hostNameOrAddress, timeout, buffer, new PingOptions());
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP packet.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="options">A <see cref="T:System.Net.NetworkInformation.PingOptions" /> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message if one was received, or provides the reason for the failure if no message was received.</returns>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is a zero length string.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />, or the <paramref name="buffer" /> size is greater than 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostNameOrAddress);
			return this.Send(hostAddresses[0], timeout, buffer, options);
		}

		/// <summary>Attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" /> and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP echo message packet.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="options">A <see cref="T:System.Net.NetworkInformation.PingOptions" /> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.PingReply" /> object that provides information about the ICMP echo reply message, if one was received, or provides the reason for the failure, if no message was received. The method will return <see cref="F:System.Net.NetworkInformation.IPStatus.PacketTooBig" /> if the packet exceeds the Maximum Transmission Unit (MTU).</returns>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />, or the <paramref name="buffer" /> size is greater than 65500 bytes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (timeout < 0)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout must be non-negative integer");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length > 65500)
			{
				throw new ArgumentException("buffer");
			}
			if (Ping.canSendPrivileged)
			{
				return this.SendPrivileged(address, timeout, buffer, options);
			}
			return this.SendUnprivileged(address, timeout, buffer, options);
		}

		private PingReply SendPrivileged(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			IPEndPoint ipendPoint = new IPEndPoint(address, 0);
			PingReply result;
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp))
			{
				if (options != null)
				{
					socket.DontFragment = options.DontFragment;
					socket.Ttl = (short)options.Ttl;
				}
				socket.SendTimeout = timeout;
				socket.ReceiveTimeout = timeout;
				byte[] array = new Ping.IcmpMessage(8, 0, this.identifier, 0, buffer).GetBytes();
				socket.SendBufferSize = array.Length;
				socket.SendTo(array, array.Length, SocketFlags.None, ipendPoint);
				Stopwatch stopwatch = Stopwatch.StartNew();
				array = new byte[array.Length + 40];
				SocketError socketError;
				long elapsedMilliseconds;
				Ping.IcmpMessage icmpMessage;
				for (;;)
				{
					EndPoint endPoint = ipendPoint;
					socketError = SocketError.Success;
					int num = socket.ReceiveFrom(array, 0, array.Length, SocketFlags.None, ref endPoint, out socketError);
					if (socketError != SocketError.Success)
					{
						break;
					}
					elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
					int num2 = (int)(array[0] & 15) << 2;
					int size = num - num2;
					if (!((IPEndPoint)endPoint).Address.Equals(ipendPoint.Address))
					{
						long num3 = (long)timeout - elapsedMilliseconds;
						if (num3 <= 0L)
						{
							goto Block_7;
						}
						socket.ReceiveTimeout = (int)num3;
					}
					else
					{
						icmpMessage = new Ping.IcmpMessage(array, num2, size);
						if (icmpMessage.Identifier == this.identifier && icmpMessage.Type != 8)
						{
							goto IL_195;
						}
						long num4 = (long)timeout - elapsedMilliseconds;
						if (num4 <= 0L)
						{
							goto Block_9;
						}
						socket.ReceiveTimeout = (int)num4;
					}
				}
				if (socketError == SocketError.TimedOut)
				{
					return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
				}
				throw new NotSupportedException(string.Format("Unexpected socket error during ping request: {0}", socketError));
				Block_7:
				return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
				Block_9:
				return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
				IL_195:
				result = new PingReply(address, icmpMessage.Data, options, elapsedMilliseconds, icmpMessage.IPStatus);
			}
			return result;
		}

		private PingReply SendUnprivileged(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Process process = new Process();
			string arguments = this.BuildPingArgs(address, timeout, options);
			long roundtripTime = 0L;
			process.StartInfo.FileName = Ping.PingBinPath;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			IPStatus status = IPStatus.Unknown;
			try
			{
				process.Start();
				process.StandardOutput.ReadToEnd();
				process.StandardError.ReadToEnd();
				roundtripTime = stopwatch.ElapsedMilliseconds;
				if (!process.WaitForExit(timeout) || (process.HasExited && process.ExitCode == 2))
				{
					status = IPStatus.TimedOut;
				}
				else if (process.ExitCode == 0)
				{
					status = IPStatus.Success;
				}
				else if (process.ExitCode == 1)
				{
					status = IPStatus.TtlExpired;
				}
			}
			catch
			{
			}
			finally
			{
				if (!process.HasExited)
				{
					process.Kill();
				}
				process.Dispose();
			}
			return new PingReply(address, buffer, options, roundtripTime, status);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		public void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken)
		{
			this.SendAsync(address, 4000, Ping.default_buffer, new PingOptions(), userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="M:System.Net.NetworkInformation.Ping.SendAsync(System.Net.IPAddress,System.Int32,System.Byte[],System.Object)" /> method is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public void SendAsync(IPAddress address, int timeout, object userToken)
		{
			this.SendAsync(address, 4000, Ping.default_buffer, userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to the <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> method is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public void SendAsync(IPAddress address, object userToken)
		{
			this.SendAsync(address, 4000, userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="hostNameOrAddress" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="hostNameOrAddress" /> could not be resolved to a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
		{
			this.SendAsync(hostNameOrAddress, timeout, buffer, new PingOptions(), userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP packet.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="buffer">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="options">A <see cref="T:System.Net.NetworkInformation.PingOptions" /> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="hostNameOrAddress" /> could not be resolved to a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
		{
			IPAddress address = Dns.GetHostEntry(hostNameOrAddress).AddressList[0];
			this.SendAsync(address, timeout, buffer, options, userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="hostNameOrAddress" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="hostNameOrAddress" /> could not be resolved to a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public void SendAsync(string hostNameOrAddress, int timeout, object userToken)
		{
			this.SendAsync(hostNameOrAddress, timeout, Ping.default_buffer, userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message to the specified computer, and receive a corresponding ICMP echo reply message from that computer.</summary>
		/// <param name="hostNameOrAddress">A <see cref="T:System.String" /> that identifies the computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="hostNameOrAddress" /> is <see langword="null" /> or is an empty string ("").</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="M:System.Net.NetworkInformation.Ping.SendAsync(System.String,System.Object)" /> method is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="hostNameOrAddress" /> could not be resolved to a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public void SendAsync(string hostNameOrAddress, object userToken)
		{
			this.SendAsync(hostNameOrAddress, 4000, userToken);
		}

		/// <summary>Asynchronously attempts to send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receive a corresponding ICMP echo reply message from that computer. This overload allows you to specify a time-out value for the operation and control fragmentation and Time-to-Live values for the ICMP echo message packet.</summary>
		/// <param name="address">An <see cref="T:System.Net.IPAddress" /> that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">An <see cref="T:System.Int32" /> value that specifies the maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="options">A <see cref="T:System.Net.NetworkInformation.PingOptions" /> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
		/// <param name="userToken">An object that is passed to the method invoked when the asynchronous operation completes.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendAsync" /> is in progress.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="address" /> is an IPv6 address and the local computer is running an operating system earlier than Windows 2000.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65500 bytes.</exception>
		public void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
		{
			if (this.worker != null || this.cts != null)
			{
				throw new InvalidOperationException("Another SendAsync operation is in progress");
			}
			this.worker = new BackgroundWorker();
			this.worker.DoWork += delegate(object o, DoWorkEventArgs ea)
			{
				try
				{
					this.user_async_state = ea.Argument;
					ea.Result = this.Send(address, timeout, buffer, options);
				}
				catch (Exception result)
				{
					ea.Result = result;
				}
			};
			this.worker.WorkerSupportsCancellation = true;
			this.worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs ea)
			{
				this.OnPingCompleted(new PingCompletedEventArgs(ea.Error, ea.Cancelled, this.user_async_state, ea.Result as PingReply));
			};
			this.worker.RunWorkerAsync(userToken);
		}

		/// <summary>Cancels all pending asynchronous requests to send an Internet Control Message Protocol (ICMP) echo message and receives a corresponding ICMP echo reply message.</summary>
		public void SendAsyncCancel()
		{
			if (this.cts != null)
			{
				this.cts.Cancel();
				return;
			}
			if (this.worker == null)
			{
				throw new InvalidOperationException("SendAsync operation is not in progress");
			}
			this.worker.CancelAsync();
		}

		private string BuildPingArgs(IPAddress address, int timeout, PingOptions options)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			StringBuilder stringBuilder = new StringBuilder();
			uint num = Convert.ToUInt32(Math.Floor((double)(timeout + 1000) / 1000.0));
			bool isMacOS = Platform.IsMacOS;
			if (!isMacOS)
			{
				stringBuilder.AppendFormat(invariantCulture, "-q -n -c {0} -w {1} -t {2} -M ", 1, num, options.Ttl);
			}
			else
			{
				stringBuilder.AppendFormat(invariantCulture, "-q -n -c {0} -t {1} -o -m {2} ", 1, num, options.Ttl);
			}
			if (!isMacOS)
			{
				stringBuilder.Append(options.DontFragment ? "do " : "dont ");
			}
			else if (options.DontFragment)
			{
				stringBuilder.Append("-D ");
			}
			stringBuilder.Append(address.ToString());
			return stringBuilder.ToString();
		}

		/// <summary>Send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receives a corresponding ICMP echo reply message from that computer as an asynchronous operation. This overload allows you to specify a time-out value for the operation and a buffer to use for send and receive.</summary>
		/// <param name="address">An IP address that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">The maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendPingAsync" /> is in progress.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65,500 bytes.</exception>
		public Task<PingReply> SendPingAsync(IPAddress address, int timeout, byte[] buffer)
		{
			return this.SendPingAsync(address, 4000, Ping.default_buffer, new PingOptions());
		}

		/// <summary>Send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receives a corresponding ICMP echo reply message from that computer as an asynchronous operation. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="address">An IP address that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">The maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<PingReply> SendPingAsync(IPAddress address, int timeout)
		{
			return this.SendPingAsync(address, 4000, Ping.default_buffer);
		}

		/// <summary>Send an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receives a corresponding ICMP echo reply message from that computer as an asynchronous operation.</summary>
		/// <param name="address">An IP address that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendPingAsync" /> is in progress.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		public Task<PingReply> SendPingAsync(IPAddress address)
		{
			return this.SendPingAsync(address, 4000);
		}

		/// <summary>Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer as an asynchronous operation. This overload allows you to specify a time-out value for the operation and a buffer to use for send and receive.</summary>
		/// <param name="hostNameOrAddress">The computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">The maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout, byte[] buffer)
		{
			return this.SendPingAsync(hostNameOrAddress, timeout, buffer, new PingOptions());
		}

		/// <summary>Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer as an asynchronous operation. This overload allows you to specify a time-out value for the operation, a buffer to use for send and receive, and control fragmentation and Time-to-Live values for the ICMP echo message packet.</summary>
		/// <param name="hostNameOrAddress">The computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">The maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="options">A <see cref="T:System.Net.NetworkInformation.PingOptions" /> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
		{
			IPAddress address = Dns.GetHostEntry(hostNameOrAddress).AddressList[0];
			return this.SendPingAsync(address, timeout, buffer, options);
		}

		/// <summary>Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer as an asynchronous operation. This overload allows you to specify a time-out value for the operation.</summary>
		/// <param name="hostNameOrAddress">The computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <param name="timeout">The maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<PingReply> SendPingAsync(string hostNameOrAddress, int timeout)
		{
			return this.SendPingAsync(hostNameOrAddress, timeout, Ping.default_buffer);
		}

		/// <summary>Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the specified computer, and receive a corresponding ICMP echo reply message from that computer as an asynchronous operation.</summary>
		/// <param name="hostNameOrAddress">The computer that is the destination for the ICMP echo message. The value specified for this parameter can be a host name or a string representation of an IP address.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		public Task<PingReply> SendPingAsync(string hostNameOrAddress)
		{
			return this.SendPingAsync(hostNameOrAddress, 4000);
		}

		/// <summary>Sends an Internet Control Message Protocol (ICMP) echo message with the specified data buffer to the computer that has the specified <see cref="T:System.Net.IPAddress" />, and receives a corresponding ICMP echo reply message from that computer as an asynchronous operation. This overload allows you to specify a time-out value for the operation, a buffer to use for send and receive, and control fragmentation and Time-to-Live values for the ICMP echo message packet.</summary>
		/// <param name="address">An IP address that identifies the computer that is the destination for the ICMP echo message.</param>
		/// <param name="timeout">The maximum number of milliseconds (after sending the echo message) to wait for the ICMP echo reply message.</param>
		/// <param name="buffer">A <see cref="T:System.Byte" /> array that contains data to be sent with the ICMP echo message and returned in the ICMP echo reply message. The array cannot contain more than 65,500 bytes.</param>
		/// <param name="options">A <see cref="T:System.Net.NetworkInformation.PingOptions" /> object used to control fragmentation and Time-to-Live values for the ICMP echo message packet.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="address" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="buffer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="timeout" /> is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">A call to <see cref="Overload:System.Net.NetworkInformation.Ping.SendPingAsync" /> is in progress.</exception>
		/// <exception cref="T:System.Net.NetworkInformation.PingException">An exception was thrown while sending or receiving the ICMP messages. See the inner exception for the exact exception that was thrown.</exception>
		/// <exception cref="T:System.Net.Sockets.SocketException">
		///   <paramref name="address" /> is not a valid IP address.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This object has been disposed.</exception>
		/// <exception cref="T:System.ArgumentException">The size of <paramref name="buffer" /> exceeds 65,500 bytes.</exception>
		public Task<PingReply> SendPingAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			if (this.worker != null || this.cts != null)
			{
				throw new InvalidOperationException("Another SendAsync operation is in progress");
			}
			this.cts = new CancellationTokenSource();
			Task<PingReply> task = Task<PingReply>.Factory.StartNew(() => this.Send(address, timeout, buffer, options), this.cts.Token);
			task.ContinueWith(delegate(Task<PingReply> t)
			{
				if (t.IsCanceled)
				{
					this.OnPingCompleted(new PingCompletedEventArgs(null, true, null, null));
					return;
				}
				if (t.IsFaulted)
				{
					this.OnPingCompleted(new PingCompletedEventArgs(t.Exception, false, null, null));
					return;
				}
				this.OnPingCompleted(new PingCompletedEventArgs(null, false, null, t.Result));
			});
			return task;
		}

		private const int DefaultCount = 1;

		private static readonly string[] PingBinPaths = new string[]
		{
			"/bin/ping",
			"/sbin/ping",
			"/usr/sbin/ping"
		};

		private static readonly string PingBinPath;

		private static bool canSendPrivileged;

		private const int default_timeout = 4000;

		private ushort identifier;

		private const uint _LINUX_CAPABILITY_VERSION_1 = 429392688U;

		private static readonly byte[] default_buffer = new byte[0];

		private BackgroundWorker worker;

		private object user_async_state;

		private CancellationTokenSource cts;

		private struct cap_user_header_t
		{
			public uint version;

			public int pid;
		}

		private struct cap_user_data_t
		{
			public uint effective;

			public uint permitted;

			public uint inheritable;
		}

		private class IcmpMessage
		{
			public IcmpMessage(byte[] bytes, int offset, int size)
			{
				this.bytes = new byte[size];
				Buffer.BlockCopy(bytes, offset, this.bytes, 0, size);
			}

			public IcmpMessage(byte type, byte code, ushort identifier, ushort sequence, byte[] data)
			{
				this.bytes = new byte[data.Length + 8];
				this.bytes[0] = type;
				this.bytes[1] = code;
				this.bytes[4] = (byte)(identifier & 255);
				this.bytes[5] = (byte)(identifier >> 8);
				this.bytes[6] = (byte)(sequence & 255);
				this.bytes[7] = (byte)(sequence >> 8);
				Buffer.BlockCopy(data, 0, this.bytes, 8, data.Length);
				ushort num = Ping.IcmpMessage.ComputeChecksum(this.bytes);
				this.bytes[2] = (byte)(num & 255);
				this.bytes[3] = (byte)(num >> 8);
			}

			public byte Type
			{
				get
				{
					return this.bytes[0];
				}
			}

			public byte Code
			{
				get
				{
					return this.bytes[1];
				}
			}

			public ushort Identifier
			{
				get
				{
					return (ushort)((int)this.bytes[4] + ((int)this.bytes[5] << 8));
				}
			}

			public ushort Sequence
			{
				get
				{
					return (ushort)((int)this.bytes[6] + ((int)this.bytes[7] << 8));
				}
			}

			public byte[] Data
			{
				get
				{
					byte[] array = new byte[this.bytes.Length - 8];
					Buffer.BlockCopy(this.bytes, 8, array, 0, array.Length);
					return array;
				}
			}

			public byte[] GetBytes()
			{
				return this.bytes;
			}

			private static ushort ComputeChecksum(byte[] data)
			{
				uint num = 0U;
				for (int i = 0; i < data.Length; i += 2)
				{
					ushort num2 = (ushort)((i + 1 < data.Length) ? data[i + 1] : 0);
					num2 = (ushort)(num2 << 8);
					num2 += (ushort)data[i];
					num += (uint)num2;
				}
				num = (num >> 16) + (num & 65535U);
				return (ushort)(~(ushort)num);
			}

			public IPStatus IPStatus
			{
				get
				{
					byte type = this.Type;
					switch (type)
					{
					case 0:
						return IPStatus.Success;
					case 1:
					case 2:
						break;
					case 3:
						switch (this.Code)
						{
						case 0:
							return IPStatus.DestinationNetworkUnreachable;
						case 1:
							return IPStatus.DestinationHostUnreachable;
						case 2:
							return IPStatus.DestinationProtocolUnreachable;
						case 3:
							return IPStatus.DestinationPortUnreachable;
						case 4:
							return IPStatus.BadOption;
						case 5:
							return IPStatus.BadRoute;
						}
						break;
					case 4:
						return IPStatus.SourceQuench;
					default:
						switch (type)
						{
						case 8:
							return IPStatus.Success;
						case 11:
						{
							byte code = this.Code;
							if (code == 0)
							{
								return IPStatus.TimeExceeded;
							}
							if (code == 1)
							{
								return IPStatus.TtlReassemblyTimeExceeded;
							}
							break;
						}
						case 12:
							return IPStatus.ParameterProblem;
						}
						break;
					}
					return IPStatus.Unknown;
				}
			}

			private byte[] bytes;
		}
	}
}
