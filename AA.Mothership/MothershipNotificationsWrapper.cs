using System;
using System.Runtime.CompilerServices;
using NativeWebSocket;
using UnityEngine;

public class MothershipNotificationsWrapper : NotificationsMessageDelegateWrapper
{
	public WebSocketState SocketState
	{
		get
		{
			return this._state;
		}
	}

	public MothershipNotificationsWrapper([NativeInteger] Action<IntPtr> onOpen = null, [NativeInteger] Action<NotificationsMessageResponse, IntPtr> onMessage = null, [NativeInteger] Action<IntPtr> onClose = null, [NativeInteger] Action<IntPtr> onError = null)
	{
		this.swigCMemOwn = false;
		this._onOpen = onOpen;
		this._onMessage = onMessage;
		this._onClose = onClose;
		this._onError = onError;
		this._state = WebSocketState.Closed;
	}

	public override void OnOpenCallback([NativeInteger] IntPtr userData)
	{
		this._state = WebSocketState.Open;
		Action<IntPtr> onOpen = this._onOpen;
		if (onOpen == null)
		{
			return;
		}
		onOpen(userData);
	}

	public override void OnMessageCallback(MothershipWebSocketMessage message, [NativeInteger] IntPtr userData)
	{
		NotificationsMessageResponse notificationsMessageResponse = NotificationsMessageResponse.FromWebSocketMessage(message);
		if (notificationsMessageResponse == null)
		{
			Debug.LogError("Notification message is invalid");
			return;
		}
		Action<NotificationsMessageResponse, IntPtr> onMessage = this._onMessage;
		if (onMessage == null)
		{
			return;
		}
		onMessage(notificationsMessageResponse, userData);
	}

	public override void OnCloseCallback([NativeInteger] IntPtr userData)
	{
		this._state = WebSocketState.Closed;
		Action<IntPtr> onClose = this._onClose;
		if (onClose == null)
		{
			return;
		}
		onClose(userData);
	}

	public override void OnErrorCallback([NativeInteger] IntPtr userData)
	{
		Action<IntPtr> onError = this._onError;
		if (onError == null)
		{
			return;
		}
		onError(userData);
	}

	private WebSocketState _state;

	[NativeInteger]
	private readonly Action<IntPtr> _onOpen;

	[NativeInteger]
	private readonly Action<NotificationsMessageResponse, IntPtr> _onMessage;

	[NativeInteger]
	private readonly Action<IntPtr> _onClose;

	[NativeInteger]
	private readonly Action<IntPtr> _onError;
}
