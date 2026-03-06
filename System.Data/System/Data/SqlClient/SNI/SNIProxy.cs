using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace System.Data.SqlClient.SNI
{
	internal class SNIProxy
	{
		public void Terminate()
		{
		}

		public uint EnableSsl(SNIHandle handle, uint options)
		{
			uint result;
			try
			{
				result = handle.EnableSsl(options);
			}
			catch (Exception sniException)
			{
				result = SNICommon.ReportSNIError(SNIProviders.SSL_PROV, 31U, sniException);
			}
			return result;
		}

		public uint DisableSsl(SNIHandle handle)
		{
			handle.DisableSsl();
			return 0U;
		}

		public void GenSspiClientContext(SspiClientContextStatus sspiClientContextStatus, byte[] receivedBuff, ref byte[] sendBuff, byte[] serverName)
		{
			SafeDeleteContext securityContext = sspiClientContextStatus.SecurityContext;
			ContextFlagsPal contextFlags = sspiClientContextStatus.ContextFlags;
			SafeFreeCredentials credentialsHandle = sspiClientContextStatus.CredentialsHandle;
			string package = "Negotiate";
			if (securityContext == null)
			{
				credentialsHandle = NegotiateStreamPal.AcquireDefaultCredential(package, false);
			}
			SecurityBuffer[] inSecurityBufferArray;
			if (receivedBuff != null)
			{
				inSecurityBufferArray = new SecurityBuffer[]
				{
					new SecurityBuffer(receivedBuff, SecurityBufferType.SECBUFFER_TOKEN)
				};
			}
			else
			{
				inSecurityBufferArray = new SecurityBuffer[0];
			}
			SecurityBuffer securityBuffer = new SecurityBuffer(NegotiateStreamPal.QueryMaxTokenSize(package), SecurityBufferType.SECBUFFER_TOKEN);
			ContextFlagsPal requestedContextFlags = ContextFlagsPal.Delegate | ContextFlagsPal.MutualAuth | ContextFlagsPal.Confidentiality | ContextFlagsPal.Connection;
			string @string = Encoding.UTF8.GetString(serverName);
			SecurityStatusPal securityStatusPal = NegotiateStreamPal.InitializeSecurityContext(credentialsHandle, ref securityContext, @string, requestedContextFlags, inSecurityBufferArray, securityBuffer, ref contextFlags);
			if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.CompleteNeeded || securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.CompAndContinue)
			{
				inSecurityBufferArray = new SecurityBuffer[]
				{
					securityBuffer
				};
				securityStatusPal = NegotiateStreamPal.CompleteAuthToken(ref securityContext, inSecurityBufferArray);
				securityBuffer.token = null;
			}
			sendBuff = securityBuffer.token;
			if (sendBuff == null)
			{
				sendBuff = Array.Empty<byte>();
			}
			sspiClientContextStatus.SecurityContext = securityContext;
			sspiClientContextStatus.ContextFlags = contextFlags;
			sspiClientContextStatus.CredentialsHandle = credentialsHandle;
			if (!SNIProxy.IsErrorStatus(securityStatusPal.ErrorCode))
			{
				return;
			}
			if (securityStatusPal.ErrorCode == SecurityStatusPalErrorCode.InternalError)
			{
				throw new InvalidOperationException(SQLMessage.KerberosTicketMissingError() + "\n" + securityStatusPal.ToString());
			}
			throw new InvalidOperationException(SQLMessage.SSPIGenerateError() + "\n" + securityStatusPal.ToString());
		}

		private static bool IsErrorStatus(SecurityStatusPalErrorCode errorCode)
		{
			return errorCode != SecurityStatusPalErrorCode.NotSet && errorCode != SecurityStatusPalErrorCode.OK && errorCode != SecurityStatusPalErrorCode.ContinueNeeded && errorCode != SecurityStatusPalErrorCode.CompleteNeeded && errorCode != SecurityStatusPalErrorCode.CompAndContinue && errorCode != SecurityStatusPalErrorCode.ContextExpired && errorCode != SecurityStatusPalErrorCode.CredentialsNeeded && errorCode != SecurityStatusPalErrorCode.Renegotiate;
		}

		public uint InitializeSspiPackage(ref uint maxLength)
		{
			throw new PlatformNotSupportedException();
		}

		public uint SetConnectionBufferSize(SNIHandle handle, uint bufferSize)
		{
			handle.SetBufferSize((int)bufferSize);
			return 0U;
		}

		public uint PacketGetData(SNIPacket packet, byte[] inBuff, ref uint dataSize)
		{
			int num = 0;
			packet.GetData(inBuff, ref num);
			dataSize = (uint)num;
			return 0U;
		}

		public uint ReadSyncOverAsync(SNIHandle handle, out SNIPacket packet, int timeout)
		{
			return handle.Receive(out packet, timeout);
		}

		public uint GetConnectionId(SNIHandle handle, ref Guid clientConnectionId)
		{
			clientConnectionId = handle.ConnectionId;
			return 0U;
		}

		public uint WritePacket(SNIHandle handle, SNIPacket packet, bool sync)
		{
			SNIPacket snipacket = packet.Clone();
			uint result;
			if (sync)
			{
				result = handle.Send(snipacket);
				snipacket.Dispose();
			}
			else
			{
				result = handle.SendAsync(snipacket, true, null);
			}
			return result;
		}

		public SNIHandle CreateConnectionHandle(object callbackObject, string fullServerName, bool ignoreSniOpenTimeout, long timerExpire, out byte[] instanceName, ref byte[] spnBuffer, bool flushCache, bool async, bool parallel, bool isIntegratedSecurity)
		{
			instanceName = new byte[1];
			bool flag;
			string localDBDataSource = this.GetLocalDBDataSource(fullServerName, out flag);
			if (flag)
			{
				return null;
			}
			fullServerName = (localDBDataSource ?? fullServerName);
			DataSource dataSource = DataSource.ParseServerName(fullServerName);
			if (dataSource == null)
			{
				return null;
			}
			SNIHandle result = null;
			switch (dataSource.ConnectionProtocol)
			{
			case DataSource.Protocol.TCP:
			case DataSource.Protocol.None:
			case DataSource.Protocol.Admin:
				result = this.CreateTcpHandle(dataSource, timerExpire, callbackObject, parallel);
				break;
			case DataSource.Protocol.NP:
				result = this.CreateNpHandle(dataSource, timerExpire, callbackObject, parallel);
				break;
			}
			if (isIntegratedSecurity)
			{
				try
				{
					spnBuffer = SNIProxy.GetSqlServerSPN(dataSource);
				}
				catch (Exception sniException)
				{
					SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.INVALID_PROV, 44U, sniException);
				}
			}
			return result;
		}

		private static byte[] GetSqlServerSPN(DataSource dataSource)
		{
			string serverName = dataSource.ServerName;
			string portOrInstanceName = null;
			if (dataSource.Port != -1)
			{
				portOrInstanceName = dataSource.Port.ToString();
			}
			else if (!string.IsNullOrWhiteSpace(dataSource.InstanceName))
			{
				portOrInstanceName = dataSource.InstanceName;
			}
			else if (dataSource.ConnectionProtocol == DataSource.Protocol.TCP)
			{
				portOrInstanceName = 1433.ToString();
			}
			return SNIProxy.GetSqlServerSPN(serverName, portOrInstanceName);
		}

		private static byte[] GetSqlServerSPN(string hostNameOrAddress, string portOrInstanceName)
		{
			IPHostEntry iphostEntry = null;
			string str;
			try
			{
				iphostEntry = Dns.GetHostEntry(hostNameOrAddress);
			}
			catch (SocketException)
			{
			}
			finally
			{
				str = (((iphostEntry != null) ? iphostEntry.HostName : null) ?? hostNameOrAddress);
			}
			string text = "MSSQLSvc/" + str;
			if (!string.IsNullOrWhiteSpace(portOrInstanceName))
			{
				text = text + ":" + portOrInstanceName;
			}
			return Encoding.UTF8.GetBytes(text);
		}

		private SNITCPHandle CreateTcpHandle(DataSource details, long timerExpire, object callbackObject, bool parallel)
		{
			string serverName = details.ServerName;
			if (string.IsNullOrWhiteSpace(serverName))
			{
				SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 0U, 25U, string.Empty);
				return null;
			}
			int port = -1;
			bool flag = details.ConnectionProtocol == DataSource.Protocol.Admin;
			if (details.IsSsrpRequired)
			{
				try
				{
					port = (flag ? SSRP.GetDacPortByInstanceName(serverName, details.InstanceName) : SSRP.GetPortByInstanceName(serverName, details.InstanceName));
					goto IL_98;
				}
				catch (SocketException sniException)
				{
					SNILoadHandle.SingletonInstance.LastError = new SNIError(SNIProviders.TCP_PROV, 25U, sniException);
					return null;
				}
			}
			if (details.Port != -1)
			{
				port = details.Port;
			}
			else
			{
				port = (flag ? 1434 : 1433);
			}
			IL_98:
			return new SNITCPHandle(serverName, port, timerExpire, callbackObject, parallel);
		}

		private SNINpHandle CreateNpHandle(DataSource details, long timerExpire, object callbackObject, bool parallel)
		{
			if (parallel)
			{
				SNICommon.ReportSNIError(SNIProviders.NP_PROV, 0U, 49U, string.Empty);
				return null;
			}
			return new SNINpHandle(details.PipeHostName, details.PipeName, timerExpire, callbackObject);
		}

		public uint ReadAsync(SNIHandle handle, out SNIPacket packet)
		{
			packet = null;
			return handle.ReceiveAsync(ref packet);
		}

		public void PacketSetData(SNIPacket packet, byte[] data, int length)
		{
			packet.SetData(data, length);
		}

		public void PacketRelease(SNIPacket packet)
		{
			packet.Release();
		}

		public uint CheckConnection(SNIHandle handle)
		{
			return handle.CheckConnection();
		}

		public SNIError GetLastError()
		{
			return SNILoadHandle.SingletonInstance.LastError;
		}

		private string GetLocalDBDataSource(string fullServerName, out bool error)
		{
			string result = null;
			bool flag;
			string localDBInstance = DataSource.GetLocalDBInstance(fullServerName, out flag);
			if (flag)
			{
				error = true;
				return null;
			}
			if (!string.IsNullOrEmpty(localDBInstance))
			{
				result = LocalDB.GetLocalDBConnectionString(localDBInstance);
				if (fullServerName == null)
				{
					error = true;
					return null;
				}
			}
			error = false;
			return result;
		}

		private const int DefaultSqlServerPort = 1433;

		private const int DefaultSqlServerDacPort = 1434;

		private const string SqlServerSpnHeader = "MSSQLSvc";

		public static readonly SNIProxy Singleton = new SNIProxy();

		internal class SspiClientContextResult
		{
			internal const uint OK = 0U;

			internal const uint Failed = 1U;

			internal const uint KerberosTicketMissing = 2U;
		}
	}
}
