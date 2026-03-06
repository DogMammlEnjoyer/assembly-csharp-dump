using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Modio.Customizations
{
	internal static class WssHandler
	{
		private static string GatewayUrl
		{
			get
			{
				return string.Format("wss://g-{0}.ws.{1}.io/", ModioClient.Settings.GameId, Regex.Match(ModioClient.Settings.ServerURL, "https://[^.]+.(?<domain>.+).io").Groups["domain"]);
			}
		}

		public static Task<ValueTuple<Error, WssMessage>> WaitForMessage(string messageOperation, bool checkPreviousUnhandledMessages = false)
		{
			WssHandler.<WaitForMessage>d__6 <WaitForMessage>d__;
			<WaitForMessage>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, WssMessage>>.Create();
			<WaitForMessage>d__.messageOperation = messageOperation;
			<WaitForMessage>d__.checkPreviousUnhandledMessages = checkPreviousUnhandledMessages;
			<WaitForMessage>d__.<>1__state = -1;
			<WaitForMessage>d__.<>t__builder.Start<WssHandler.<WaitForMessage>d__6>(ref <WaitForMessage>d__);
			return <WaitForMessage>d__.<>t__builder.Task;
		}

		public static Task<ValueTuple<Error, T>> DoMessageHandshake<T>(WssMessage message) where T : struct
		{
			WssHandler.<DoMessageHandshake>d__7<T> <DoMessageHandshake>d__;
			<DoMessageHandshake>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, T>>.Create();
			<DoMessageHandshake>d__.message = message;
			<DoMessageHandshake>d__.<>1__state = -1;
			<DoMessageHandshake>d__.<>t__builder.Start<WssHandler.<DoMessageHandshake>d__7<T>>(ref <DoMessageHandshake>d__);
			return <DoMessageHandshake>d__.<>t__builder.Task;
		}

		public static void CancelWaitingFor(string messageOperation)
		{
			if (WssHandler.WaitingForMessages.ContainsKey(messageOperation))
			{
				TaskCompletionSource<WssMessage> taskCompletionSource = WssHandler.WaitingForMessages[messageOperation];
				WssHandler.WaitingForMessages.Remove(messageOperation);
				taskCompletionSource.SetResult(default(WssMessage));
			}
		}

		private static void CancelAllAwaitingMessages()
		{
			List<TaskCompletionSource<WssMessage>> list = WssHandler.WaitingForMessages.Values.ToList<TaskCompletionSource<WssMessage>>();
			WssHandler.WaitingForMessages.Clear();
			foreach (TaskCompletionSource<WssMessage> taskCompletionSource in list)
			{
				taskCompletionSource.SetResult(default(WssMessage));
			}
		}

		public static Task Shutdown()
		{
			WssHandler.<Shutdown>d__10 <Shutdown>d__;
			<Shutdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Shutdown>d__.<>1__state = -1;
			<Shutdown>d__.<>t__builder.Start<WssHandler.<Shutdown>d__10>(ref <Shutdown>d__);
			return <Shutdown>d__.<>t__builder.Task;
		}

		private static Task<Error> EnsureConnection()
		{
			WssHandler.<EnsureConnection>d__11 <EnsureConnection>d__;
			<EnsureConnection>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<EnsureConnection>d__.<>1__state = -1;
			<EnsureConnection>d__.<>t__builder.Start<WssHandler.<EnsureConnection>d__11>(ref <EnsureConnection>d__);
			return <EnsureConnection>d__.<>t__builder.Task;
		}

		public static Task<Error> Send(WssMessage message)
		{
			WssHandler.<Send>d__12 <Send>d__;
			<Send>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<Send>d__.message = message;
			<Send>d__.<>1__state = -1;
			<Send>d__.<>t__builder.Start<WssHandler.<Send>d__12>(ref <Send>d__);
			return <Send>d__.<>t__builder.Task;
		}

		private static void Receive(WssMessages messages)
		{
			foreach (WssMessage wssMessage in messages.messages)
			{
				if (wssMessage.operation == "failed_operation")
				{
					WssHandler.ProcessErrorObject(wssMessage);
				}
				else if (WssHandler.WaitingForMessages.ContainsKey(wssMessage.operation))
				{
					TaskCompletionSource<WssMessage> taskCompletionSource = WssHandler.WaitingForMessages[wssMessage.operation];
					WssHandler.WaitingForMessages.Remove(wssMessage.operation);
					taskCompletionSource.SetResult(wssMessage);
				}
				else
				{
					ModioLog verbose = ModioLog.Verbose;
					if (verbose != null)
					{
						verbose.Log("[Socket] Received unexpected message operation (" + wssMessage.operation + ").\nCaching it temporarily in case we listen for it immediately after.");
					}
					if (WssHandler.UnhandledMessages.ContainsKey(wssMessage.operation))
					{
						WssHandler.UnhandledMessages[wssMessage.operation] = wssMessage;
					}
					else
					{
						WssHandler.UnhandledMessages.Add(wssMessage.operation, wssMessage);
					}
				}
			}
		}

		private static void ProcessErrorObject(WssMessage message)
		{
			WssErrorObject wssErrorObject;
			if (message.TryGetValue<WssErrorObject>(out wssErrorObject))
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Concat(new string[]
					{
						"[Socket] Error received from WssMessages:\n",
						string.Format("Error: [{0}]", wssErrorObject.error.code),
						string.Format(" [{0}]", wssErrorObject.error.error_ref),
						" ",
						wssErrorObject.error.message
					}));
				}
				if (WssHandler.WaitingForMessages.ContainsKey(wssErrorObject.operation))
				{
					TaskCompletionSource<WssMessage> taskCompletionSource = WssHandler.WaitingForMessages[wssErrorObject.operation];
					WssHandler.WaitingForMessages.Remove(wssErrorObject.operation);
					taskCompletionSource.SetResult(default(WssMessage));
					return;
				}
				if (WssHandler.SubscribedMessageListeners.ContainsKey(wssErrorObject.operation))
				{
					Action<WssMessage> action = WssHandler.SubscribedMessageListeners[wssErrorObject.operation];
					if (action != null)
					{
						action(message);
					}
					WssHandler.WaitingForMessages.Remove(wssErrorObject.operation);
					return;
				}
				ModioLog warning = ModioLog.Warning;
				if (warning == null)
				{
					return;
				}
				warning.Log("[Socket:Internal] Could not find any matching listener for the error operation: " + wssErrorObject.operation);
				return;
			}
			else
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 == null)
				{
					return;
				}
				error2.Log("[Socket:Internal] Failed to cast WssMessage (operation \"" + message.operation + "\") into WssErrorObject");
				return;
			}
		}

		private static void Disconnected()
		{
			WssHandler.<Disconnected>d__15 <Disconnected>d__;
			<Disconnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Disconnected>d__.<>1__state = -1;
			<Disconnected>d__.<>t__builder.Start<WssHandler.<Disconnected>d__15>(ref <Disconnected>d__);
		}

		private static ISocketConnection Socket = new SocketConnection();

		private static Dictionary<string, TaskCompletionSource<WssMessage>> WaitingForMessages = new Dictionary<string, TaskCompletionSource<WssMessage>>();

		private static Dictionary<string, Action<WssMessage>> SubscribedMessageListeners = new Dictionary<string, Action<WssMessage>>();

		private static Dictionary<string, WssMessage> UnhandledMessages = new Dictionary<string, WssMessage>();
	}
}
