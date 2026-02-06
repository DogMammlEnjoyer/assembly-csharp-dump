using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;

public class MothershipWebSocketWrapper : MothershipWebSocketDelegateWrapper
{
	public MothershipWebSocketWrapper(MothershipClientApiClient client)
	{
		this.swigCMemOwn = false;
		this._client = client;
		this._retryQueue = new MothershipWebSocketRetryQueue();
		this._websockets = new List<ActiveWebSocket>();
	}

	public void RefreshClientTokenHeaders()
	{
		foreach (ActiveWebSocket activeWebSocket in this._websockets)
		{
			activeWebSocket.websocket.SetRequestHeader(MothershipApi.MOTHERSHIP_CLIENT_TOKEN_HEADER, MothershipClientContext.Token);
			foreach (MothershipHttpHeader mothershipHttpHeader in activeWebSocket.requestData.RequestHeaders)
			{
				if (mothershipHttpHeader.Name == MothershipApi.MOTHERSHIP_CLIENT_TOKEN_HEADER)
				{
					mothershipHttpHeader.Value = MothershipClientContext.Token;
					break;
				}
			}
		}
	}

	public void TickWebSockets(float deltaTime)
	{
		foreach (ActiveWebSocket activeWebSocket in this._websockets)
		{
			if (activeWebSocket.websocket.State == WebSocketState.Open)
			{
				activeWebSocket.websocket.DispatchMessageQueue();
			}
		}
		this._retryQueue.Tick(deltaTime);
	}

	public override bool CreateConnection(MothershipOpenWebSocketEventArgs request)
	{
		MothershipWebSocketWrapper.<>c__DisplayClass6_0 CS$<>8__locals1 = new MothershipWebSocketWrapper.<>c__DisplayClass6_0();
		CS$<>8__locals1.request = request;
		CS$<>8__locals1.<>4__this = this;
		ActiveWebSocket activeWebSocket = this._websockets.Find((ActiveWebSocket ws) => ws.requestData.Path == CS$<>8__locals1.request.Path);
		if (activeWebSocket != null)
		{
			Action<MothershipOpenWebSocketEventArgs> resetSocket = activeWebSocket.resetSocket;
			if (resetSocket != null)
			{
				resetSocket(CS$<>8__locals1.request);
			}
			return true;
		}
		CS$<>8__locals1.aws = new ActiveWebSocket();
		CS$<>8__locals1.<CreateConnection>g__CreateSocket|1(CS$<>8__locals1.request);
		return true;
	}

	public override bool CloseConnection(MothershipCloseWebSocketEventArgs request)
	{
		for (int i = 0; i < this._websockets.Count; i++)
		{
			ActiveWebSocket activeWebSocket = this._websockets[i];
			if (!(request.Path != activeWebSocket.requestData.Path))
			{
				try
				{
					activeWebSocket.websocket.Close();
				}
				catch
				{
					Debug.LogError("WebSocket " + request.Path + " failed to close");
				}
				this._retryQueue.RemoveSocket(activeWebSocket);
				this._websockets.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public void CloseConnections()
	{
		Task.Run(new Func<Task>(this.CloseConnectionsAsync)).GetAwaiter().GetResult();
	}

	private Task CloseConnectionsAsync()
	{
		MothershipWebSocketWrapper.<CloseConnectionsAsync>d__9 <CloseConnectionsAsync>d__;
		<CloseConnectionsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<CloseConnectionsAsync>d__.<>4__this = this;
		<CloseConnectionsAsync>d__.<>1__state = -1;
		<CloseConnectionsAsync>d__.<>t__builder.Start<MothershipWebSocketWrapper.<CloseConnectionsAsync>d__9>(ref <CloseConnectionsAsync>d__);
		return <CloseConnectionsAsync>d__.<>t__builder.Task;
	}

	[CompilerGenerated]
	internal static void <CreateConnection>g__OnError|6_5(string error)
	{
		Debug.LogError("WebSocket erroring: " + error);
	}

	private readonly MothershipClientApiClient _client;

	private readonly MothershipWebSocketRetryQueue _retryQueue;

	private readonly List<ActiveWebSocket> _websockets;
}
