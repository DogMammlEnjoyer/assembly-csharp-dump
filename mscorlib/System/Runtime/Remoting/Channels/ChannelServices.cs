using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	/// <summary>Provides static methods to aid with remoting channel registration, resolution, and URL discovery. This class cannot be inherited.</summary>
	[ComVisible(true)]
	public sealed class ChannelServices
	{
		private ChannelServices()
		{
		}

		internal static CrossContextChannel CrossContextChannel
		{
			get
			{
				return ChannelServices._crossContextSink;
			}
		}

		internal static IMessageSink CreateClientChannelSinkChain(string url, object remoteChannelData, out string objectUri)
		{
			object[] channelDataArray = (object[])remoteChannelData;
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in ChannelServices.registeredChannels)
				{
					IChannelSender channelSender = ((IChannel)obj) as IChannelSender;
					if (channelSender != null)
					{
						IMessageSink messageSink = ChannelServices.CreateClientChannelSinkChain(channelSender, url, channelDataArray, out objectUri);
						if (messageSink != null)
						{
							return messageSink;
						}
					}
				}
				RemotingConfiguration.LoadDefaultDelayedChannels();
				foreach (object obj2 in ChannelServices.delayedClientChannels)
				{
					IChannelSender channelSender2 = (IChannelSender)obj2;
					IMessageSink messageSink2 = ChannelServices.CreateClientChannelSinkChain(channelSender2, url, channelDataArray, out objectUri);
					if (messageSink2 != null)
					{
						ChannelServices.delayedClientChannels.Remove(channelSender2);
						ChannelServices.RegisterChannel(channelSender2);
						return messageSink2;
					}
				}
			}
			objectUri = null;
			return null;
		}

		internal static IMessageSink CreateClientChannelSinkChain(IChannelSender sender, string url, object[] channelDataArray, out string objectUri)
		{
			objectUri = null;
			if (channelDataArray == null)
			{
				return sender.CreateMessageSink(url, null, out objectUri);
			}
			foreach (object obj in channelDataArray)
			{
				IMessageSink messageSink;
				if (obj is IChannelDataStore)
				{
					messageSink = sender.CreateMessageSink(null, obj, out objectUri);
				}
				else
				{
					messageSink = sender.CreateMessageSink(url, obj, out objectUri);
				}
				if (messageSink != null)
				{
					return messageSink;
				}
			}
			return null;
		}

		/// <summary>Gets a list of currently registered channels.</summary>
		/// <returns>An array of all the currently registered channels.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static IChannel[] RegisteredChannels
		{
			get
			{
				object syncRoot = ChannelServices.registeredChannels.SyncRoot;
				IChannel[] result;
				lock (syncRoot)
				{
					List<IChannel> list = new List<IChannel>();
					for (int i = 0; i < ChannelServices.registeredChannels.Count; i++)
					{
						IChannel channel = (IChannel)ChannelServices.registeredChannels[i];
						if (!(channel is CrossAppDomainChannel))
						{
							list.Add(channel);
						}
					}
					result = list.ToArray();
				}
				return result;
			}
		}

		/// <summary>Creates a channel sink chain for the specified channel.</summary>
		/// <param name="provider">The first provider in the chain of sink providers that will create the channel sink chain.</param>
		/// <param name="channel">The <see cref="T:System.Runtime.Remoting.Channels.IChannelReceiver" /> for which to create the channel sink chain.</param>
		/// <returns>A new channel sink chain for the specified channel.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static IServerChannelSink CreateServerChannelSinkChain(IServerChannelSinkProvider provider, IChannelReceiver channel)
		{
			IServerChannelSinkProvider serverChannelSinkProvider = provider;
			while (serverChannelSinkProvider.Next != null)
			{
				serverChannelSinkProvider = serverChannelSinkProvider.Next;
			}
			serverChannelSinkProvider.Next = new ServerDispatchSinkProvider();
			return provider.CreateSink(channel);
		}

		/// <summary>Dispatches incoming remote calls.</summary>
		/// <param name="sinkStack">The stack of server channel sinks that the message already traversed.</param>
		/// <param name="msg">The message to dispatch.</param>
		/// <param name="replyMsg">When this method returns, contains a <see cref="T:System.Runtime.Remoting.Messaging.IMessage" /> that holds the reply from the server to the message that is contained in the <paramref name="msg" /> parameter. This parameter is passed uninitialized.</param>
		/// <returns>A <see cref="T:System.Runtime.Remoting.Channels.ServerProcessing" /> that gives the status of the server message processing.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="msg" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static ServerProcessing DispatchMessage(IServerChannelSinkStack sinkStack, IMessage msg, out IMessage replyMsg)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			replyMsg = ChannelServices.SyncDispatchMessage(msg);
			if (RemotingServices.IsOneWay(((IMethodMessage)msg).MethodBase))
			{
				return ServerProcessing.OneWay;
			}
			return ServerProcessing.Complete;
		}

		/// <summary>Returns a registered channel with the specified name.</summary>
		/// <param name="name">The channel name.</param>
		/// <returns>An interface to a registered channel, or <see langword="null" /> if the channel is not registered.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static IChannel GetChannel(string name)
		{
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			IChannel result;
			lock (syncRoot)
			{
				foreach (object obj in ChannelServices.registeredChannels)
				{
					IChannel channel = (IChannel)obj;
					if (channel.ChannelName == name && !(channel is CrossAppDomainChannel))
					{
						return channel;
					}
				}
				result = null;
			}
			return result;
		}

		/// <summary>Returns a <see cref="T:System.Collections.IDictionary" /> of properties for a given proxy.</summary>
		/// <param name="obj">The proxy to retrieve properties for.</param>
		/// <returns>An interface to the dictionary of properties, or <see langword="null" /> if no properties were found.</returns>
		/// <exception cref="T:System.Security.SecurityException">At least one of the callers that is higher in the callstack does not have permission to configure remoting types and channels.</exception>
		public static IDictionary GetChannelSinkProperties(object obj)
		{
			if (!RemotingServices.IsTransparentProxy(obj))
			{
				throw new ArgumentException("obj must be a proxy", "obj");
			}
			IMessageSink messageSink = ((ClientIdentity)RemotingServices.GetRealProxy(obj).ObjectIdentity).ChannelSink;
			List<IDictionary> list = new List<IDictionary>();
			while (messageSink != null && !(messageSink is IClientChannelSink))
			{
				messageSink = messageSink.NextSink;
			}
			if (messageSink == null)
			{
				return new Hashtable();
			}
			for (IClientChannelSink clientChannelSink = messageSink as IClientChannelSink; clientChannelSink != null; clientChannelSink = clientChannelSink.NextChannelSink)
			{
				list.Add(clientChannelSink.Properties);
			}
			return new AggregateDictionary(list.ToArray());
		}

		/// <summary>Returns an array of all the URLs that can be used to reach the specified object.</summary>
		/// <param name="obj">The object to retrieve the URL array for.</param>
		/// <returns>An array of strings that contains the URLs that can be used to remotely identify the object, or <see langword="null" /> if none were found.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static string[] GetUrlsForObject(MarshalByRefObject obj)
		{
			string objectUri = RemotingServices.GetObjectUri(obj);
			if (objectUri == null)
			{
				return new string[0];
			}
			List<string> list = new List<string>();
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj2 in ChannelServices.registeredChannels)
				{
					if (!(obj2 is CrossAppDomainChannel))
					{
						IChannelReceiver channelReceiver = obj2 as IChannelReceiver;
						if (channelReceiver != null)
						{
							list.AddRange(channelReceiver.GetUrlsForUri(objectUri));
						}
					}
				}
			}
			return list.ToArray();
		}

		/// <summary>Registers a channel with the channel services. <see cref="M:System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(System.Runtime.Remoting.Channels.IChannel)" /> is obsolete. Please use <see cref="M:System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(System.Runtime.Remoting.Channels.IChannel,System.Boolean)" /> instead.</summary>
		/// <param name="chnl">The channel to register.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="chnl" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Runtime.Remoting.RemotingException">The channel has already been registered.</exception>
		/// <exception cref="T:System.Security.SecurityException">At least one of the callers higher in the callstack does not have permission to configure remoting types and channels.</exception>
		[Obsolete("Use RegisterChannel(IChannel,Boolean)")]
		public static void RegisterChannel(IChannel chnl)
		{
			ChannelServices.RegisterChannel(chnl, false);
		}

		/// <summary>Registers a channel with the channel services.</summary>
		/// <param name="chnl">The channel to register.</param>
		/// <param name="ensureSecurity">
		///   <see langword="true" /> ensures that security is enabled; otherwise <see langword="false" />. Setting the value to <see langword="false" /> does not effect the security setting on the TCP or IPC channel.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="chnl" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Runtime.Remoting.RemotingException">The channel has already been registered.</exception>
		/// <exception cref="T:System.Security.SecurityException">At least one of the callers higher in the call stack does not have permission to configure remoting types and channels.</exception>
		/// <exception cref="T:System.NotSupportedException">Not supported in Windows 98 for <see cref="T:System.Runtime.Remoting.Channels.Tcp.TcpServerChannel" /> and on all platforms for <see cref="T:System.Runtime.Remoting.Channels.Http.HttpServerChannel" />. Host the service using Internet Information Services (IIS) if you require a secure HTTP channel.</exception>
		public static void RegisterChannel(IChannel chnl, bool ensureSecurity)
		{
			if (chnl == null)
			{
				throw new ArgumentNullException("chnl");
			}
			if (ensureSecurity)
			{
				ISecurableChannel securableChannel = chnl as ISecurableChannel;
				if (securableChannel == null)
				{
					throw new RemotingException(string.Format("Channel {0} is not securable while ensureSecurity is specified as true", chnl.ChannelName));
				}
				securableChannel.IsSecured = true;
			}
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			lock (syncRoot)
			{
				int num = -1;
				for (int i = 0; i < ChannelServices.registeredChannels.Count; i++)
				{
					IChannel channel = (IChannel)ChannelServices.registeredChannels[i];
					if (channel.ChannelName == chnl.ChannelName && chnl.ChannelName != "")
					{
						throw new RemotingException("Channel " + channel.ChannelName + " already registered");
					}
					if (channel.ChannelPriority < chnl.ChannelPriority && num == -1)
					{
						num = i;
					}
				}
				if (num != -1)
				{
					ChannelServices.registeredChannels.Insert(num, chnl);
				}
				else
				{
					ChannelServices.registeredChannels.Add(chnl);
				}
				IChannelReceiver channelReceiver = chnl as IChannelReceiver;
				if (channelReceiver != null && ChannelServices.oldStartModeTypes.Contains(chnl.GetType().ToString()))
				{
					channelReceiver.StartListening(null);
				}
			}
		}

		internal static void RegisterChannelConfig(ChannelData channel)
		{
			IServerChannelSinkProvider serverChannelSinkProvider = null;
			IClientChannelSinkProvider clientChannelSinkProvider = null;
			for (int i = channel.ServerProviders.Count - 1; i >= 0; i--)
			{
				IServerChannelSinkProvider serverChannelSinkProvider2 = (IServerChannelSinkProvider)ChannelServices.CreateProvider(channel.ServerProviders[i] as ProviderData);
				serverChannelSinkProvider2.Next = serverChannelSinkProvider;
				serverChannelSinkProvider = serverChannelSinkProvider2;
			}
			for (int j = channel.ClientProviders.Count - 1; j >= 0; j--)
			{
				IClientChannelSinkProvider clientChannelSinkProvider2 = (IClientChannelSinkProvider)ChannelServices.CreateProvider(channel.ClientProviders[j] as ProviderData);
				clientChannelSinkProvider2.Next = clientChannelSinkProvider;
				clientChannelSinkProvider = clientChannelSinkProvider2;
			}
			Type type = Type.GetType(channel.Type);
			if (type == null)
			{
				throw new RemotingException("Type '" + channel.Type + "' not found");
			}
			bool flag = typeof(IChannelSender).IsAssignableFrom(type);
			bool flag2 = typeof(IChannelReceiver).IsAssignableFrom(type);
			Type[] types;
			object[] parameters;
			if (flag && flag2)
			{
				types = new Type[]
				{
					typeof(IDictionary),
					typeof(IClientChannelSinkProvider),
					typeof(IServerChannelSinkProvider)
				};
				parameters = new object[]
				{
					channel.CustomProperties,
					clientChannelSinkProvider,
					serverChannelSinkProvider
				};
			}
			else if (flag)
			{
				types = new Type[]
				{
					typeof(IDictionary),
					typeof(IClientChannelSinkProvider)
				};
				parameters = new object[]
				{
					channel.CustomProperties,
					clientChannelSinkProvider
				};
			}
			else
			{
				if (!flag2)
				{
					Type type2 = type;
					throw new RemotingException(((type2 != null) ? type2.ToString() : null) + " is not a valid channel type");
				}
				types = new Type[]
				{
					typeof(IDictionary),
					typeof(IServerChannelSinkProvider)
				};
				parameters = new object[]
				{
					channel.CustomProperties,
					serverChannelSinkProvider
				};
			}
			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor == null)
			{
				Type type3 = type;
				throw new RemotingException(((type3 != null) ? type3.ToString() : null) + " does not have a valid constructor");
			}
			IChannel channel2;
			try
			{
				channel2 = (IChannel)constructor.Invoke(parameters);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			lock (syncRoot)
			{
				if (channel.DelayLoadAsClientChannel == "true" && !(channel2 is IChannelReceiver))
				{
					ChannelServices.delayedClientChannels.Add(channel2);
				}
				else
				{
					ChannelServices.RegisterChannel(channel2);
				}
			}
		}

		private static object CreateProvider(ProviderData prov)
		{
			Type type = Type.GetType(prov.Type);
			if (type == null)
			{
				throw new RemotingException("Type '" + prov.Type + "' not found");
			}
			object[] args = new object[]
			{
				prov.CustomProperties,
				prov.CustomData
			};
			object result;
			try
			{
				result = Activator.CreateInstance(type, args);
			}
			catch (Exception innerException)
			{
				if (innerException is TargetInvocationException)
				{
					innerException = ((TargetInvocationException)innerException).InnerException;
				}
				string str = "An instance of provider '";
				Type type2 = type;
				throw new RemotingException(str + ((type2 != null) ? type2.ToString() : null) + "' could not be created: " + innerException.Message);
			}
			return result;
		}

		/// <summary>Synchronously dispatches the incoming message to the server-side chain(s) based on the URI embedded in the message.</summary>
		/// <param name="msg">The message to dispatch.</param>
		/// <returns>A reply message is returned by the call to the server-side chain.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="msg" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static IMessage SyncDispatchMessage(IMessage msg)
		{
			IMessage message = ChannelServices.CheckIncomingMessage(msg);
			if (message != null)
			{
				return ChannelServices.CheckReturnMessage(msg, message);
			}
			message = ChannelServices._crossContextSink.SyncProcessMessage(msg);
			return ChannelServices.CheckReturnMessage(msg, message);
		}

		/// <summary>Asynchronously dispatches the given message to the server-side chain(s) based on the URI embedded in the message.</summary>
		/// <param name="msg">The message to dispatch.</param>
		/// <param name="replySink">The sink that will process the return message if it is not <see langword="null" />.</param>
		/// <returns>A <see cref="T:System.Runtime.Remoting.Messaging.IMessageCtrl" /> object used to control the asynchronously dispatched message.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="msg" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static IMessageCtrl AsyncDispatchMessage(IMessage msg, IMessageSink replySink)
		{
			IMessage message = ChannelServices.CheckIncomingMessage(msg);
			if (message != null)
			{
				replySink.SyncProcessMessage(ChannelServices.CheckReturnMessage(msg, message));
				return null;
			}
			if (RemotingConfiguration.CustomErrorsEnabled(ChannelServices.IsLocalCall(msg)))
			{
				replySink = new ExceptionFilterSink(msg, replySink);
			}
			return ChannelServices._crossContextSink.AsyncProcessMessage(msg, replySink);
		}

		private static ReturnMessage CheckIncomingMessage(IMessage msg)
		{
			IMethodMessage methodMessage = (IMethodMessage)msg;
			ServerIdentity serverIdentity = RemotingServices.GetIdentityForUri(methodMessage.Uri) as ServerIdentity;
			if (serverIdentity == null)
			{
				return new ReturnMessage(new RemotingException("No receiver for uri " + methodMessage.Uri), (IMethodCallMessage)msg);
			}
			RemotingServices.SetMessageTargetIdentity(msg, serverIdentity);
			return null;
		}

		internal static IMessage CheckReturnMessage(IMessage callMsg, IMessage retMsg)
		{
			IMethodReturnMessage methodReturnMessage = retMsg as IMethodReturnMessage;
			if (methodReturnMessage != null && methodReturnMessage.Exception != null && RemotingConfiguration.CustomErrorsEnabled(ChannelServices.IsLocalCall(callMsg)))
			{
				retMsg = new MethodResponse(new Exception("Server encountered an internal error. For more information, turn off customErrors in the server's .config file."), (IMethodCallMessage)callMsg);
			}
			return retMsg;
		}

		private static bool IsLocalCall(IMessage callMsg)
		{
			return true;
		}

		/// <summary>Unregisters a particular channel from the registered channels list.</summary>
		/// <param name="chnl">The channel to unregister.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="chnl" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The channel is not registered.</exception>
		/// <exception cref="T:System.Security.SecurityException">At least one of the callers higher in the callstack does not have permission to configure remoting types and channels.</exception>
		public static void UnregisterChannel(IChannel chnl)
		{
			if (chnl == null)
			{
				throw new ArgumentNullException();
			}
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			lock (syncRoot)
			{
				for (int i = 0; i < ChannelServices.registeredChannels.Count; i++)
				{
					if (ChannelServices.registeredChannels[i] == chnl)
					{
						ChannelServices.registeredChannels.RemoveAt(i);
						IChannelReceiver channelReceiver = chnl as IChannelReceiver;
						if (channelReceiver != null)
						{
							channelReceiver.StopListening(null);
						}
						return;
					}
				}
				throw new RemotingException("Channel not registered");
			}
		}

		internal static object[] GetCurrentChannelInfo()
		{
			List<object> list = new List<object>();
			object syncRoot = ChannelServices.registeredChannels.SyncRoot;
			lock (syncRoot)
			{
				foreach (object obj in ChannelServices.registeredChannels)
				{
					IChannelReceiver channelReceiver = obj as IChannelReceiver;
					if (channelReceiver != null)
					{
						object channelData = channelReceiver.ChannelData;
						if (channelData != null)
						{
							list.Add(channelData);
						}
					}
				}
			}
			return list.ToArray();
		}

		private static ArrayList registeredChannels = new ArrayList();

		private static ArrayList delayedClientChannels = new ArrayList();

		private static CrossContextChannel _crossContextSink = new CrossContextChannel();

		internal static string CrossContextUrl = "__CrossContext";

		private static IList oldStartModeTypes = new string[]
		{
			"Novell.Zenworks.Zmd.Public.UnixServerChannel",
			"Novell.Zenworks.Zmd.Public.UnixChannel"
		};
	}
}
