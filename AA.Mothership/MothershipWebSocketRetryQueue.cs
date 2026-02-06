using System;
using System.Collections.Generic;

internal class MothershipWebSocketRetryQueue
{
	public void AddSocket(ActiveWebSocket socket)
	{
		if (this.GetRetryingSocket(socket) != null)
		{
			return;
		}
		this._websockets.Add(new MothershipWebSocketRetryQueue.RetryingWebSocket(socket));
	}

	public void RemoveSocket(ActiveWebSocket socket)
	{
		for (int i = 0; i < this._websockets.Count; i++)
		{
			if (this._websockets[i].Equals(socket))
			{
				this._websockets.RemoveAt(i);
				return;
			}
		}
	}

	public void ClearSockets()
	{
		this._websockets.Clear();
	}

	public void RetrySocket(ActiveWebSocket socket)
	{
		MothershipWebSocketRetryQueue.RetryingWebSocket retryingSocket = this.GetRetryingSocket(socket);
		if (retryingSocket != null)
		{
			retryingSocket.Retry();
		}
	}

	public void ResetSocket(ActiveWebSocket socket)
	{
		MothershipWebSocketRetryQueue.RetryingWebSocket retryingSocket = this.GetRetryingSocket(socket);
		if (retryingSocket != null)
		{
			retryingSocket.Reset();
		}
	}

	public void Tick(float deltaSeconds)
	{
		foreach (MothershipWebSocketRetryQueue.RetryingWebSocket retryingWebSocket in this._websockets)
		{
			retryingWebSocket.Tick(deltaSeconds);
		}
	}

	private MothershipWebSocketRetryQueue.RetryingWebSocket GetRetryingSocket(ActiveWebSocket socket)
	{
		foreach (MothershipWebSocketRetryQueue.RetryingWebSocket retryingWebSocket in this._websockets)
		{
			if (retryingWebSocket.Equals(socket))
			{
				return retryingWebSocket;
			}
		}
		return null;
	}

	private const float MAX_RETRY_SECONDS = 120f;

	private const float INITIAL_RETRY_SECONDS = 5f;

	private readonly List<MothershipWebSocketRetryQueue.RetryingWebSocket> _websockets = new List<MothershipWebSocketRetryQueue.RetryingWebSocket>();

	private class RetryingWebSocket : IEquatable<ActiveWebSocket>
	{
		public RetryingWebSocket(ActiveWebSocket socket)
		{
			this._websocket = socket;
		}

		public void Retry()
		{
			this._retryEnabled = true;
		}

		public void Reset()
		{
			this._timeLeft = 5f;
			this._lastSetTime = this._timeLeft;
		}

		public void Tick(float deltaSeconds)
		{
			if (!this._retryEnabled)
			{
				return;
			}
			this._timeLeft -= deltaSeconds;
			if (this._timeLeft > 0f)
			{
				return;
			}
			this._timeLeft = MathF.Min(this._lastSetTime * 2f, 120f);
			this._lastSetTime = this._timeLeft;
			this._retryEnabled = false;
			Action<MothershipOpenWebSocketEventArgs> resetSocket = this._websocket.resetSocket;
			if (resetSocket == null)
			{
				return;
			}
			resetSocket(this._websocket.requestData);
		}

		public bool Equals(ActiveWebSocket other)
		{
			return this._websocket == other;
		}

		private readonly ActiveWebSocket _websocket;

		private float _lastSetTime = 5f;

		private float _timeLeft = 5f;

		private bool _retryEnabled;
	}
}
