using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class OVRNetwork
{
	public const int MaxBufferLength = 65536;

	public const int MaxPayloadLength = 65524;

	public const uint FrameHeaderMagicIdentifier = 1384359787U;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private struct FrameHeader
	{
		public byte[] ToBytes()
		{
			int num = Marshal.SizeOf<OVRNetwork.FrameHeader>(this);
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr<OVRNetwork.FrameHeader>(this, intPtr, true);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			return array;
		}

		public static OVRNetwork.FrameHeader FromBytes(byte[] arr)
		{
			OVRNetwork.FrameHeader frameHeader = default(OVRNetwork.FrameHeader);
			int num = Marshal.SizeOf<OVRNetwork.FrameHeader>(frameHeader);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(arr, 0, intPtr, num);
			frameHeader = (OVRNetwork.FrameHeader)Marshal.PtrToStructure(intPtr, frameHeader.GetType());
			Marshal.FreeHGlobal(intPtr);
			return frameHeader;
		}

		public uint protocolIdentifier;

		public int payloadType;

		public int payloadLength;

		public const int StructSize = 12;
	}

	public class OVRNetworkTcpServer
	{
		public void StartListening(int listeningPort)
		{
			if (this.tcpListener != null)
			{
				Debug.LogWarning("[OVRNetworkTcpServer] tcpListener is not null");
				return;
			}
			IPAddress any = IPAddress.Any;
			this.tcpListener = new TcpListener(any, listeningPort);
			try
			{
				this.tcpListener.Start();
				Debug.LogFormat("TcpListener started. Local endpoint: {0}", new object[]
				{
					this.tcpListener.LocalEndpoint.ToString()
				});
			}
			catch (SocketException ex)
			{
				Debug.LogWarningFormat("[OVRNetworkTcpServer] Unsable to start TcpListener. Socket exception: {0}", new object[]
				{
					ex.Message
				});
				Debug.LogWarning("It could be caused by multiple instances listening at the same port, or the port is forwarded to the Android device through ADB");
				Debug.LogWarning("If the port is forwarded through ADB, use the Android Tools in Tools/Oculus/System Metrics Profiler to kill the server");
				this.tcpListener = null;
			}
			if (this.tcpListener != null)
			{
				Debug.LogFormat("[OVRNetworkTcpServer] Start Listening on port {0}", new object[]
				{
					listeningPort
				});
				try
				{
					this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.DoAcceptTcpClientCallback), this.tcpListener);
				}
				catch (Exception ex2)
				{
					Debug.LogWarningFormat("[OVRNetworkTcpServer] can't accept new client: {0}", new object[]
					{
						ex2.Message
					});
				}
			}
		}

		public void StopListening()
		{
			if (this.tcpListener == null)
			{
				Debug.LogWarning("[OVRNetworkTcpServer] tcpListener is null");
				return;
			}
			object obj = this.clientsLock;
			lock (obj)
			{
				this.clients.Clear();
			}
			this.tcpListener.Stop();
			this.tcpListener = null;
			Debug.Log("[OVRNetworkTcpServer] Stopped listening");
		}

		private void DoAcceptTcpClientCallback(IAsyncResult ar)
		{
			TcpListener tcpListener = ar.AsyncState as TcpListener;
			try
			{
				TcpClient item = tcpListener.EndAcceptTcpClient(ar);
				object obj = this.clientsLock;
				lock (obj)
				{
					this.clients.Add(item);
					Debug.Log("[OVRNetworkTcpServer] client added");
				}
				try
				{
					this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.DoAcceptTcpClientCallback), this.tcpListener);
				}
				catch (Exception ex)
				{
					Debug.LogWarningFormat("[OVRNetworkTcpServer] can't accept new client: {0}", new object[]
					{
						ex.Message
					});
				}
			}
			catch (ObjectDisposedException)
			{
			}
			catch (Exception ex2)
			{
				Debug.LogWarningFormat("[OVRNetworkTcpServer] EndAcceptTcpClient failed: {0}", new object[]
				{
					ex2.Message
				});
			}
		}

		public bool HasConnectedClient()
		{
			object obj = this.clientsLock;
			lock (obj)
			{
				using (List<TcpClient>.Enumerator enumerator = this.clients.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Connected)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void Broadcast(int payloadType, byte[] payload)
		{
			if (payload.Length > 65524)
			{
				Debug.LogWarningFormat("[OVRNetworkTcpServer] drop payload because it's too long: {0} bytes", new object[]
				{
					payload.Length
				});
			}
			OVRNetwork.FrameHeader frameHeader = default(OVRNetwork.FrameHeader);
			frameHeader.protocolIdentifier = 1384359787U;
			frameHeader.payloadType = payloadType;
			frameHeader.payloadLength = payload.Length;
			byte[] array = frameHeader.ToBytes();
			byte[] array2 = new byte[array.Length + payload.Length];
			array.CopyTo(array2, 0);
			payload.CopyTo(array2, array.Length);
			object obj = this.clientsLock;
			lock (obj)
			{
				foreach (TcpClient tcpClient in this.clients)
				{
					if (tcpClient.Connected)
					{
						try
						{
							tcpClient.GetStream().BeginWrite(array2, 0, array2.Length, new AsyncCallback(this.DoWriteDataCallback), tcpClient.GetStream());
						}
						catch (SocketException ex)
						{
							Debug.LogWarningFormat("[OVRNetworkTcpServer] close client because of socket error: {0}", new object[]
							{
								ex.Message
							});
							tcpClient.GetStream().Close();
							tcpClient.Close();
						}
					}
				}
			}
		}

		private void DoWriteDataCallback(IAsyncResult ar)
		{
			(ar.AsyncState as NetworkStream).EndWrite(ar);
		}

		public TcpListener tcpListener;

		private readonly object clientsLock = new object();

		public readonly List<TcpClient> clients = new List<TcpClient>();
	}

	public class OVRNetworkTcpClient
	{
		public OVRNetwork.OVRNetworkTcpClient.ConnectionState connectionState
		{
			get
			{
				if (this.tcpClient == null)
				{
					return OVRNetwork.OVRNetworkTcpClient.ConnectionState.Disconnected;
				}
				if (this.tcpClient.Connected)
				{
					return OVRNetwork.OVRNetworkTcpClient.ConnectionState.Connected;
				}
				return OVRNetwork.OVRNetworkTcpClient.ConnectionState.Connecting;
			}
		}

		public bool Connected
		{
			get
			{
				return this.connectionState == OVRNetwork.OVRNetworkTcpClient.ConnectionState.Connected;
			}
		}

		public void Connect(int listeningPort)
		{
			if (this.tcpClient == null)
			{
				this.receivedBufferIndex = 0;
				this.receivedBufferDataSize = 0;
				this.readyReceiveDataEvent.Set();
				string host = "127.0.0.1";
				this.tcpClient = new TcpClient(AddressFamily.InterNetwork);
				this.tcpClient.BeginConnect(host, listeningPort, new AsyncCallback(this.ConnectCallback), this.tcpClient);
				if (this.connectionStateChangedCallback != null)
				{
					this.connectionStateChangedCallback();
					return;
				}
			}
			else
			{
				Debug.LogWarning("[OVRNetworkTcpClient] already connected");
			}
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				TcpClient tcpClient = ar.AsyncState as TcpClient;
				tcpClient.EndConnect(ar);
				Debug.LogFormat("[OVRNetworkTcpClient] connected to {0}", new object[]
				{
					tcpClient.ToString()
				});
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("[OVRNetworkTcpClient] connect error {0}", new object[]
				{
					ex.Message
				});
			}
			if (this.connectionStateChangedCallback != null)
			{
				this.connectionStateChangedCallback();
			}
		}

		public void Disconnect()
		{
			if (this.tcpClient != null)
			{
				if (!this.readyReceiveDataEvent.WaitOne(5))
				{
					Debug.LogWarning("[OVRNetworkTcpClient] readyReceiveDataEvent not signaled. data receiving timeout?");
				}
				Debug.Log("[OVRNetworkTcpClient] close tcpClient");
				try
				{
					this.tcpClient.GetStream().Close();
					this.tcpClient.Close();
				}
				catch (Exception ex)
				{
					Debug.LogWarning("[OVRNetworkTcpClient] " + ex.Message);
				}
				this.tcpClient = null;
				if (this.connectionStateChangedCallback != null)
				{
					this.connectionStateChangedCallback();
					return;
				}
			}
			else
			{
				Debug.LogWarning("[OVRNetworkTcpClient] not connected");
			}
		}

		public void Tick()
		{
			if (this.tcpClient == null || !this.tcpClient.Connected)
			{
				return;
			}
			if (this.readyReceiveDataEvent.WaitOne(TimeSpan.Zero) && this.tcpClient.GetStream().DataAvailable)
			{
				if (this.receivedBufferDataSize >= 65536)
				{
					Debug.LogWarning("[OVRNetworkTcpClient] receive buffer overflow. It should not happen since we have the constraint on message size");
					this.Disconnect();
					return;
				}
				this.readyReceiveDataEvent.Reset();
				int count = 65536 - this.receivedBufferDataSize;
				this.tcpClient.GetStream().BeginRead(this.receivedBuffers[this.receivedBufferIndex], this.receivedBufferDataSize, count, new AsyncCallback(this.OnReadDataCallback), this.tcpClient.GetStream());
			}
		}

		private void OnReadDataCallback(IAsyncResult ar)
		{
			NetworkStream networkStream = ar.AsyncState as NetworkStream;
			try
			{
				int num = networkStream.EndRead(ar);
				this.receivedBufferDataSize += num;
				while (this.receivedBufferDataSize >= 12)
				{
					OVRNetwork.FrameHeader frameHeader = OVRNetwork.FrameHeader.FromBytes(this.receivedBuffers[this.receivedBufferIndex]);
					if (frameHeader.protocolIdentifier != 1384359787U)
					{
						Debug.LogWarning("[OVRNetworkTcpClient] header mismatch");
						this.Disconnect();
						return;
					}
					if (frameHeader.payloadLength < 0 || frameHeader.payloadLength > 65524)
					{
						Debug.LogWarningFormat("[OVRNetworkTcpClient] Sanity check failed. PayloadLength %d", new object[]
						{
							frameHeader.payloadLength
						});
						this.Disconnect();
						return;
					}
					if (this.receivedBufferDataSize >= 12 + frameHeader.payloadLength)
					{
						if (this.payloadReceivedCallback != null)
						{
							this.payloadReceivedCallback(frameHeader.payloadType, this.receivedBuffers[this.receivedBufferIndex], 12, frameHeader.payloadLength);
						}
						int num2 = 1 - this.receivedBufferIndex;
						int num3 = this.receivedBufferDataSize - (12 + frameHeader.payloadLength);
						if (num3 > 0)
						{
							Array.Copy(this.receivedBuffers[this.receivedBufferIndex], 12 + frameHeader.payloadLength, this.receivedBuffers[num2], 0, num3);
						}
						this.receivedBufferIndex = num2;
						this.receivedBufferDataSize = num3;
					}
				}
				this.readyReceiveDataEvent.Set();
			}
			catch (SocketException ex)
			{
				Debug.LogErrorFormat("[OVRNetworkTcpClient] OnReadDataCallback: socket error: {0}", new object[]
				{
					ex.Message
				});
				this.Disconnect();
			}
		}

		public Action connectionStateChangedCallback;

		public Action<int, byte[], int, int> payloadReceivedCallback;

		private TcpClient tcpClient;

		private byte[][] receivedBuffers = new byte[][]
		{
			new byte[65536],
			new byte[65536]
		};

		private int receivedBufferIndex;

		private int receivedBufferDataSize;

		private ManualResetEvent readyReceiveDataEvent = new ManualResetEvent(true);

		public enum ConnectionState
		{
			Disconnected,
			Connected,
			Connecting
		}
	}
}
