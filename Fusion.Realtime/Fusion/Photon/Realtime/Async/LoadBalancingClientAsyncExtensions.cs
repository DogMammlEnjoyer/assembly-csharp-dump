using System;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Async;

namespace Fusion.Photon.Realtime.Async
{
	internal static class LoadBalancingClientAsyncExtensions
	{
		public static Task<RegionHandler> GetRegionsAsync(this LoadBalancingClient client, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancelationToken = default(CancellationToken))
		{
			bool flag = !client.ConnectToNameServer();
			Task<RegionHandler> result2;
			if (flag)
			{
				result2 = Task.FromException<RegionHandler>(new OperationStartException("Failed to get regions"));
			}
			else
			{
				TaskCompletionSource<RegionHandler> result = new TaskCompletionSource<RegionHandler>(externalCancelationToken);
				OperationHandler handler = client.CreateOpHandler(throwOnError, createServiceTask, externalCancelationToken);
				PhotonConnectionCallbacks connectionCallbacks = handler.ConnectionCallbacks;
				connectionCallbacks.Disconnected = (Action<DisconnectCause>)Delegate.Combine(connectionCallbacks.Disconnected, new Action<DisconnectCause>(delegate(DisconnectCause cause)
				{
					handler.SetResult(0);
					result.SetException(new OperationStartException(string.Format("Failed to get regions. Disconnection cause: {0}", cause)));
				}));
				PhotonConnectionCallbacks connectionCallbacks2 = handler.ConnectionCallbacks;
				Action<RegionHandler> <>9__2;
				connectionCallbacks2.RegionListReceived = (Action<RegionHandler>)Delegate.Combine(connectionCallbacks2.RegionListReceived, new Action<RegionHandler>(delegate(RegionHandler regionHandler)
				{
					Action<RegionHandler> onCompleteCallback;
					if ((onCompleteCallback = <>9__2) == null)
					{
						onCompleteCallback = (<>9__2 = delegate(RegionHandler regionHandlerWithPing)
						{
							bool isCancellationRequested = externalCancelationToken.IsCancellationRequested;
							if (isCancellationRequested)
							{
								result.SetResult(null);
								handler.SetResult(0);
							}
							else
							{
								result.SetResult(regionHandlerWithPing);
								handler.SetResult(0);
							}
						});
					}
					regionHandler.PingMinimumOfRegions(onCompleteCallback, string.Empty);
				}));
				result2 = result.Task;
			}
			return result2;
		}

		public static Task ConnectUsingSettingsAsync(this LoadBalancingClient client, AppSettings appSettings, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = client.State != ClientState.Disconnected && client.State > ClientState.PeerCreated;
			Task result;
			if (flag)
			{
				result = Task.FromException(new OperationStartException("Client still connected"));
			}
			else
			{
				bool flag2 = !client.ConnectUsingSettings(appSettings);
				if (flag2)
				{
					result = Task.FromException(new OperationStartException("Failed to start connecting"));
				}
				else
				{
					result = client.CreateOpHandler(true, createServiceTask, externalCancellationToken).Task;
				}
			}
			return result;
		}

		public static Task ReconnectAndRejoinAsync(this LoadBalancingClient client, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = client.State != ClientState.Disconnected;
			Task result;
			if (flag)
			{
				result = Task.FromException(new OperationStartException("Client still connected"));
			}
			else
			{
				bool flag2 = !client.ReconnectAndRejoin();
				if (flag2)
				{
					result = Task.FromException(new OperationStartException("Failed to start reconnecting"));
				}
				else
				{
					result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancellationToken).Task;
				}
			}
			return result;
		}

		public static Task DisconnectAsync(this LoadBalancingClient client, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = client == null;
			Task result;
			if (flag)
			{
				result = Task.CompletedTask;
			}
			else
			{
				bool flag2 = client.State == ClientState.Disconnected;
				if (flag2)
				{
					result = Task.CompletedTask;
				}
				else
				{
					OperationHandler handler = client.CreateOpHandler(true, createServiceTask, externalCancellationToken);
					PhotonConnectionCallbacks connectionCallbacks = handler.ConnectionCallbacks;
					connectionCallbacks.Disconnected = (Action<DisconnectCause>)Delegate.Combine(connectionCallbacks.Disconnected, new Action<DisconnectCause>(delegate(DisconnectCause cause)
					{
						LogStream logInfo = InternalLogStreams.LogInfo;
						if (logInfo != null)
						{
							logInfo.Log(string.Format("Disconnected: {0}", cause));
						}
						handler.SetResult(0);
					}));
					client.Disconnect();
					result = handler.Task;
				}
			}
			return result;
		}

		public static Task LeaveRoomAsync(this LoadBalancingClient client, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = client == null;
			Task result;
			if (flag)
			{
				result = Task.CompletedTask;
			}
			else
			{
				bool flag2 = client.State == ClientState.Disconnected || !client.InRoom;
				if (flag2)
				{
					result = Task.CompletedTask;
				}
				else
				{
					OperationHandler handler = client.CreateOpHandler(true, createServiceTask, externalCancellationToken);
					PhotonMatchmakingCallbacks matchmakingCallbacks = handler.MatchmakingCallbacks;
					matchmakingCallbacks.LeftRoom = (Action)Delegate.Combine(matchmakingCallbacks.LeftRoom, new Action(delegate()
					{
						LogStream logInfo = InternalLogStreams.LogInfo;
						if (logInfo != null)
						{
							logInfo.Log("Left Room");
						}
						handler.SetResult(0);
					}));
					client.OpLeaveRoom(false, false);
					result = handler.Task;
				}
			}
			return result;
		}

		public static Task<short> CreateRoomAsync(this LoadBalancingClient client, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = !client.OpCreateRoom(enterRoomParams);
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new OperationStartException("Failed to send CreateRoom operation"));
			}
			else
			{
				result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancellationToken).Task;
			}
			return result;
		}

		public static Task<short> CreateOrJoinRoomAsync(this LoadBalancingClient client, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = !client.OpJoinOrCreateRoom(enterRoomParams);
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new OperationStartException("Failed to send CreateRoom operation"));
			}
			else
			{
				result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancellationToken).Task;
			}
			return result;
		}

		public static Task<short> JoinRoomAsync(this LoadBalancingClient client, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = !client.OpJoinRoom(enterRoomParams);
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new OperationStartException("Failed to send JoinRoom operation"));
			}
			else
			{
				result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancellationToken).Task;
			}
			return result;
		}

		public static Task<short> JoinRandomOrCreateRoomAsync(this LoadBalancingClient client, OpJoinRandomRoomParams joinRandomRoomParams, EnterRoomParams enterRoomParams, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = !client.OpJoinRandomOrCreateRoom(joinRandomRoomParams, enterRoomParams);
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new OperationStartException("Failed to send JoinRandomOrCreateRoom operation"));
			}
			else
			{
				result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancellationToken).Task;
			}
			return result;
		}

		public static Task<short> JoinRandomRoomAsync(this LoadBalancingClient client, OpJoinRandomRoomParams joinRandomRoomParams, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			bool flag = !client.OpJoinRandomRoom(joinRandomRoomParams);
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new OperationStartException("Failed to send JoinRandomRoom operation"));
			}
			else
			{
				result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancellationToken).Task;
			}
			return result;
		}

		public static Task<short> JoinLobbyAsync(this LoadBalancingClient client, TypedLobby lobby, bool throwOnError = true, bool createServiceTask = true, CancellationToken externalCancelationToken = default(CancellationToken))
		{
			bool flag = !client.OpJoinLobby(lobby);
			Task<short> result;
			if (flag)
			{
				result = Task.FromException<short>(new OperationStartException("Failed to send JoinLobby operation"));
			}
			else
			{
				result = client.CreateOpHandler(throwOnError, createServiceTask, externalCancelationToken).Task;
			}
			return result;
		}

		public static OperationHandler CreateOpHandler(this LoadBalancingClient client, bool throwOnErrors = true, bool createServiceTask = true, CancellationToken externalCancellationToken = default(CancellationToken))
		{
			OperationHandler handler = new OperationHandler(throwOnErrors, externalCancellationToken);
			client.AddCallbackTarget(handler);
			TaskManager.ContinueWhenAll(new Task[]
			{
				handler.Task
			}, delegate(CancellationToken token)
			{
				client.RemoveCallbackTarget(handler);
				return Task.CompletedTask;
			}, handler.Token);
			if (createServiceTask)
			{
				client.Service_ClientUpdate(handler.Token, handler.CompletionSource);
			}
			return handler;
		}

		public static void Service_ClientUpdate(this LoadBalancingClient client, CancellationToken token, TaskCompletionSource<short> completionSource)
		{
			TaskManager.Service(delegate()
			{
				try
				{
					bool flag = !token.IsCancellationRequested;
					if (flag)
					{
						client.Service();
					}
				}
				catch (Exception exception)
				{
					completionSource.TrySetException(exception);
				}
				return Task.FromResult<bool>(client.IsConnected);
			}, token, 10, "AsyncClientUpdate");
		}

		private const int SERVICE_INTERVAL_MS = 10;
	}
}
