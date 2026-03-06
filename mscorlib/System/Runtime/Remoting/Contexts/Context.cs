using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	/// <summary>Defines an environment for the objects that are resident inside it and for which a policy can be enforced.</summary>
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public class Context
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterContext(Context ctx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseContext(Context ctx);

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Remoting.Contexts.Context" /> class.</summary>
		public Context()
		{
			this.domain_id = Thread.GetDomainID();
			this.context_id = Interlocked.Increment(ref Context.global_count);
			Context.RegisterContext(this);
		}

		/// <summary>Cleans up the backing objects for the nondefault contexts.</summary>
		~Context()
		{
			Context.ReleaseContext(this);
		}

		/// <summary>Gets the default context for the current application domain.</summary>
		/// <returns>The default context for the <see cref="T:System.AppDomain" /> namespace.</returns>
		public static Context DefaultContext
		{
			get
			{
				return AppDomain.InternalGetDefaultContext();
			}
		}

		/// <summary>Gets the context ID for the current context.</summary>
		/// <returns>The context ID for the current context.</returns>
		public virtual int ContextID
		{
			get
			{
				return this.context_id;
			}
		}

		/// <summary>Gets the array of the current context properties.</summary>
		/// <returns>The current context properties array; otherwise, <see langword="null" /> if the context does not have any properties attributed to it.</returns>
		public virtual IContextProperty[] ContextProperties
		{
			get
			{
				if (this.context_properties == null)
				{
					return new IContextProperty[0];
				}
				return this.context_properties.ToArray();
			}
		}

		internal bool IsDefaultContext
		{
			get
			{
				return this.context_id == 0;
			}
		}

		internal bool NeedsContextSink
		{
			get
			{
				return this.context_id != 0 || (Context.global_dynamic_properties != null && Context.global_dynamic_properties.HasProperties) || (this.context_dynamic_properties != null && this.context_dynamic_properties.HasProperties);
			}
		}

		/// <summary>Registers a dynamic property implementing the <see cref="T:System.Runtime.Remoting.Contexts.IDynamicProperty" /> interface with the remoting service.</summary>
		/// <param name="prop">The dynamic property to register.</param>
		/// <param name="obj">The object/proxy for which the property is registered.</param>
		/// <param name="ctx">The context for which the property is registered.</param>
		/// <returns>
		///   <see langword="true" /> if the property was successfully registered; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">Either <paramref name="prop" /> or its name is <see langword="null" />, or it is not dynamic (it does not implement <see cref="T:System.Runtime.Remoting.Contexts.IDynamicProperty" />).</exception>
		/// <exception cref="T:System.ArgumentException">Both an object as well as a context are specified (both <paramref name="obj" /> and <paramref name="ctx" /> are not <see langword="null" />).</exception>
		public static bool RegisterDynamicProperty(IDynamicProperty prop, ContextBoundObject obj, Context ctx)
		{
			return Context.GetDynamicPropertyCollection(obj, ctx).RegisterDynamicProperty(prop);
		}

		/// <summary>Unregisters a dynamic property implementing the <see cref="T:System.Runtime.Remoting.Contexts.IDynamicProperty" /> interface.</summary>
		/// <param name="name">The name of the dynamic property to unregister.</param>
		/// <param name="obj">The object/proxy for which the property is registered.</param>
		/// <param name="ctx">The context for which the property is registered.</param>
		/// <returns>
		///   <see langword="true" /> if the object was successfully unregistered; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">Both an object as well as a context are specified (both <paramref name="obj" /> and <paramref name="ctx" /> are not <see langword="null" />).</exception>
		public static bool UnregisterDynamicProperty(string name, ContextBoundObject obj, Context ctx)
		{
			return Context.GetDynamicPropertyCollection(obj, ctx).UnregisterDynamicProperty(name);
		}

		private static DynamicPropertyCollection GetDynamicPropertyCollection(ContextBoundObject obj, Context ctx)
		{
			if (ctx == null && obj != null)
			{
				if (RemotingServices.IsTransparentProxy(obj))
				{
					return RemotingServices.GetRealProxy(obj).ObjectIdentity.ClientDynamicProperties;
				}
				return obj.ObjectIdentity.ServerDynamicProperties;
			}
			else
			{
				if (ctx != null && obj == null)
				{
					if (ctx.context_dynamic_properties == null)
					{
						ctx.context_dynamic_properties = new DynamicPropertyCollection();
					}
					return ctx.context_dynamic_properties;
				}
				if (ctx == null && obj == null)
				{
					if (Context.global_dynamic_properties == null)
					{
						Context.global_dynamic_properties = new DynamicPropertyCollection();
					}
					return Context.global_dynamic_properties;
				}
				throw new ArgumentException("Either obj or ctx must be null");
			}
		}

		internal static void NotifyGlobalDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (Context.global_dynamic_properties != null && Context.global_dynamic_properties.HasProperties)
			{
				Context.global_dynamic_properties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		internal static bool HasGlobalDynamicSinks
		{
			get
			{
				return Context.global_dynamic_properties != null && Context.global_dynamic_properties.HasProperties;
			}
		}

		internal void NotifyDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (this.context_dynamic_properties != null && this.context_dynamic_properties.HasProperties)
			{
				this.context_dynamic_properties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		internal bool HasDynamicSinks
		{
			get
			{
				return this.context_dynamic_properties != null && this.context_dynamic_properties.HasProperties;
			}
		}

		internal bool HasExitSinks
		{
			get
			{
				return !(this.GetClientContextSinkChain() is ClientContextTerminatorSink) || this.HasDynamicSinks || Context.HasGlobalDynamicSinks;
			}
		}

		/// <summary>Returns a specific context property, specified by name.</summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>The specified context property.</returns>
		public virtual IContextProperty GetProperty(string name)
		{
			if (this.context_properties == null)
			{
				return null;
			}
			foreach (IContextProperty contextProperty in this.context_properties)
			{
				if (contextProperty.Name == name)
				{
					return contextProperty;
				}
			}
			return null;
		}

		/// <summary>Sets a specific context property by name.</summary>
		/// <param name="prop">The actual context property.</param>
		/// <exception cref="T:System.InvalidOperationException">The context is frozen.</exception>
		/// <exception cref="T:System.ArgumentNullException">The property or the property name is <see langword="null" />.</exception>
		public virtual void SetProperty(IContextProperty prop)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("IContextProperty");
			}
			if (this == Context.DefaultContext)
			{
				throw new InvalidOperationException("Can not add properties to default context");
			}
			if (this.context_properties == null)
			{
				this.context_properties = new List<IContextProperty>();
			}
			this.context_properties.Add(prop);
		}

		/// <summary>Freezes the context, making it impossible to add or remove context properties from the current context.</summary>
		/// <exception cref="T:System.InvalidOperationException">The context is already frozen.</exception>
		public virtual void Freeze()
		{
			if (this.context_properties != null)
			{
				foreach (IContextProperty contextProperty in this.context_properties)
				{
					contextProperty.Freeze(this);
				}
			}
		}

		/// <summary>Returns a <see cref="T:System.String" /> class representation of the current context.</summary>
		/// <returns>A <see cref="T:System.String" /> class representation of the current context.</returns>
		public override string ToString()
		{
			return "ContextID: " + this.context_id.ToString();
		}

		internal IMessageSink GetServerContextSinkChain()
		{
			if (this.server_context_sink_chain == null)
			{
				if (Context.default_server_context_sink == null)
				{
					Context.default_server_context_sink = new ServerContextTerminatorSink();
				}
				this.server_context_sink_chain = Context.default_server_context_sink;
				if (this.context_properties != null)
				{
					for (int i = this.context_properties.Count - 1; i >= 0; i--)
					{
						IContributeServerContextSink contributeServerContextSink = this.context_properties[i] as IContributeServerContextSink;
						if (contributeServerContextSink != null)
						{
							this.server_context_sink_chain = contributeServerContextSink.GetServerContextSink(this.server_context_sink_chain);
						}
					}
				}
			}
			return this.server_context_sink_chain;
		}

		internal IMessageSink GetClientContextSinkChain()
		{
			if (this.client_context_sink_chain == null)
			{
				this.client_context_sink_chain = new ClientContextTerminatorSink(this);
				if (this.context_properties != null)
				{
					foreach (IContextProperty contextProperty in this.context_properties)
					{
						IContributeClientContextSink contributeClientContextSink = contextProperty as IContributeClientContextSink;
						if (contributeClientContextSink != null)
						{
							this.client_context_sink_chain = contributeClientContextSink.GetClientContextSink(this.client_context_sink_chain);
						}
					}
				}
			}
			return this.client_context_sink_chain;
		}

		internal IMessageSink CreateServerObjectSinkChain(MarshalByRefObject obj, bool forceInternalExecute)
		{
			IMessageSink messageSink = new StackBuilderSink(obj, forceInternalExecute);
			messageSink = new ServerObjectTerminatorSink(messageSink);
			messageSink = new LeaseSink(messageSink);
			if (this.context_properties != null)
			{
				for (int i = this.context_properties.Count - 1; i >= 0; i--)
				{
					IContributeObjectSink contributeObjectSink = this.context_properties[i] as IContributeObjectSink;
					if (contributeObjectSink != null)
					{
						messageSink = contributeObjectSink.GetObjectSink(obj, messageSink);
					}
				}
			}
			return messageSink;
		}

		internal IMessageSink CreateEnvoySink(MarshalByRefObject serverObject)
		{
			IMessageSink messageSink = EnvoyTerminatorSink.Instance;
			if (this.context_properties != null)
			{
				foreach (IContextProperty contextProperty in this.context_properties)
				{
					IContributeEnvoySink contributeEnvoySink = contextProperty as IContributeEnvoySink;
					if (contributeEnvoySink != null)
					{
						messageSink = contributeEnvoySink.GetEnvoySink(serverObject, messageSink);
					}
				}
			}
			return messageSink;
		}

		internal static Context SwitchToContext(Context newContext)
		{
			return AppDomain.InternalSetContext(newContext);
		}

		internal static Context CreateNewContext(IConstructionCallMessage msg)
		{
			Context context = new Context();
			foreach (object obj in msg.ContextProperties)
			{
				IContextProperty contextProperty = (IContextProperty)obj;
				if (context.GetProperty(contextProperty.Name) == null)
				{
					context.SetProperty(contextProperty);
				}
			}
			context.Freeze();
			using (IEnumerator enumerator = msg.ContextProperties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!((IContextProperty)enumerator.Current).IsNewContextOK(context))
					{
						throw new RemotingException("A context property did not approve the candidate context for activating the object");
					}
				}
			}
			return context;
		}

		/// <summary>Executes code in another context.</summary>
		/// <param name="deleg">The delegate used to request the callback.</param>
		public void DoCallBack(CrossContextDelegate deleg)
		{
			lock (this)
			{
				if (this.callback_object == null)
				{
					Context newContext = Context.SwitchToContext(this);
					this.callback_object = new ContextCallbackObject();
					Context.SwitchToContext(newContext);
				}
			}
			this.callback_object.DoCallBack(deleg);
		}

		private LocalDataStore MyLocalStore
		{
			get
			{
				if (this._localDataStore == null)
				{
					LocalDataStoreMgr localDataStoreMgr = Context._localDataStoreMgr;
					lock (localDataStoreMgr)
					{
						if (this._localDataStore == null)
						{
							this._localDataStore = Context._localDataStoreMgr.CreateLocalDataStore();
						}
					}
				}
				return this._localDataStore.Store;
			}
		}

		/// <summary>Allocates an unnamed data slot.</summary>
		/// <returns>A local data slot.</returns>
		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return Context._localDataStoreMgr.AllocateDataSlot();
		}

		/// <summary>Allocates a named data slot.</summary>
		/// <param name="name">The required name for the data slot.</param>
		/// <returns>A local data slot object.</returns>
		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			return Context._localDataStoreMgr.AllocateNamedDataSlot(name);
		}

		/// <summary>Frees a named data slot on all the contexts.</summary>
		/// <param name="name">The name of the data slot to free.</param>
		public static void FreeNamedDataSlot(string name)
		{
			Context._localDataStoreMgr.FreeNamedDataSlot(name);
		}

		/// <summary>Looks up a named data slot.</summary>
		/// <param name="name">The data slot name.</param>
		/// <returns>A local data slot.</returns>
		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			return Context._localDataStoreMgr.GetNamedDataSlot(name);
		}

		/// <summary>Retrieves the value from the specified slot on the current context.</summary>
		/// <param name="slot">The data slot that contains the data.</param>
		/// <returns>The data associated with <paramref name="slot" />.</returns>
		public static object GetData(LocalDataStoreSlot slot)
		{
			return Thread.CurrentContext.MyLocalStore.GetData(slot);
		}

		/// <summary>Sets the data in the specified slot on the current context.</summary>
		/// <param name="slot">The data slot where the data is to be added.</param>
		/// <param name="data">The data that is to be added.</param>
		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			Thread.CurrentContext.MyLocalStore.SetData(slot, data);
		}

		private int domain_id;

		private int context_id;

		private UIntPtr static_data;

		private UIntPtr data;

		[ContextStatic]
		private static object[] local_slots;

		private static IMessageSink default_server_context_sink;

		private IMessageSink server_context_sink_chain;

		private IMessageSink client_context_sink_chain;

		private List<IContextProperty> context_properties;

		private static int global_count;

		private volatile LocalDataStoreHolder _localDataStore;

		private static LocalDataStoreMgr _localDataStoreMgr = new LocalDataStoreMgr();

		private static DynamicPropertyCollection global_dynamic_properties;

		private DynamicPropertyCollection context_dynamic_properties;

		private ContextCallbackObject callback_object;
	}
}
