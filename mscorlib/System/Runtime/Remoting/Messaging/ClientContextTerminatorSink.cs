using System;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Messaging
{
	internal class ClientContextTerminatorSink : IMessageSink
	{
		public ClientContextTerminatorSink(Context ctx)
		{
			this._context = ctx;
		}

		public IMessage SyncProcessMessage(IMessage msg)
		{
			Context.NotifyGlobalDynamicSinks(true, msg, true, false);
			this._context.NotifyDynamicSinks(true, msg, true, false);
			IMessage result;
			if (msg is IConstructionCallMessage)
			{
				result = ActivationServices.RemoteActivate((IConstructionCallMessage)msg);
			}
			else
			{
				result = RemotingServices.GetMessageTargetIdentity(msg).ChannelSink.SyncProcessMessage(msg);
			}
			Context.NotifyGlobalDynamicSinks(false, msg, true, false);
			this._context.NotifyDynamicSinks(false, msg, true, false);
			return result;
		}

		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			if (this._context.HasDynamicSinks || Context.HasGlobalDynamicSinks)
			{
				Context.NotifyGlobalDynamicSinks(true, msg, true, true);
				this._context.NotifyDynamicSinks(true, msg, true, true);
				if (replySink != null)
				{
					replySink = new ClientContextReplySink(this._context, replySink);
				}
			}
			IMessageCtrl result = RemotingServices.GetMessageTargetIdentity(msg).ChannelSink.AsyncProcessMessage(msg, replySink);
			if (replySink == null && (this._context.HasDynamicSinks || Context.HasGlobalDynamicSinks))
			{
				Context.NotifyGlobalDynamicSinks(false, msg, true, true);
				this._context.NotifyDynamicSinks(false, msg, true, true);
			}
			return result;
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		private Context _context;
	}
}
