using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	/// <summary>Implements the <see cref="T:System.Runtime.Remoting.Messaging.IMethodReturnMessage" /> interface to create a message that acts as a response to a method call on a remote object.</summary>
	[ComVisible(true)]
	public class MethodReturnMessageWrapper : InternalMessageWrapper, IMethodReturnMessage, IMethodMessage, IMessage
	{
		/// <summary>Wraps an <see cref="T:System.Runtime.Remoting.Messaging.IMethodReturnMessage" /> to create a <see cref="T:System.Runtime.Remoting.Messaging.MethodReturnMessageWrapper" />.</summary>
		/// <param name="msg">A message that acts as an outgoing method call on a remote object.</param>
		public MethodReturnMessageWrapper(IMethodReturnMessage msg) : base(msg)
		{
			if (msg.Exception != null)
			{
				this._exception = msg.Exception;
				this._args = new object[0];
				return;
			}
			this._args = msg.Args;
			this._return = msg.ReturnValue;
			if (msg.MethodBase != null)
			{
				this._outArgInfo = new ArgInfo(msg.MethodBase, ArgInfoType.Out);
			}
		}

		/// <summary>Gets the number of arguments passed to the method.</summary>
		/// <returns>A <see cref="T:System.Int32" /> that represents the number of arguments passed to a method.</returns>
		public virtual int ArgCount
		{
			[SecurityCritical]
			get
			{
				return this._args.Length;
			}
		}

		/// <summary>Gets an array of arguments passed to the method.</summary>
		/// <returns>An array of type <see cref="T:System.Object" /> that represents the arguments passed to a method.</returns>
		public virtual object[] Args
		{
			[SecurityCritical]
			get
			{
				return this._args;
			}
			set
			{
				this._args = value;
			}
		}

		/// <summary>Gets the exception thrown during the method call, or <see langword="null" /> if the method did not throw an exception.</summary>
		/// <returns>The <see cref="T:System.Exception" /> thrown during the method call, or <see langword="null" /> if the method did not throw an exception.</returns>
		public virtual Exception Exception
		{
			[SecurityCritical]
			get
			{
				return this._exception;
			}
			set
			{
				this._exception = value;
			}
		}

		/// <summary>Gets a flag that indicates whether the method can accept a variable number of arguments.</summary>
		/// <returns>
		///   <see langword="true" /> if the method can accept a variable number of arguments; otherwise, <see langword="false" />.</returns>
		public virtual bool HasVarArgs
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).HasVarArgs;
			}
		}

		/// <summary>Gets the <see cref="T:System.Runtime.Remoting.Messaging.LogicalCallContext" /> for the current method call.</summary>
		/// <returns>The <see cref="T:System.Runtime.Remoting.Messaging.LogicalCallContext" /> for the current method call.</returns>
		public virtual LogicalCallContext LogicalCallContext
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).LogicalCallContext;
			}
		}

		/// <summary>Gets the <see cref="T:System.Reflection.MethodBase" /> of the called method.</summary>
		/// <returns>The <see cref="T:System.Reflection.MethodBase" /> of the called method.</returns>
		public virtual MethodBase MethodBase
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).MethodBase;
			}
		}

		/// <summary>Gets the name of the invoked method.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the name of the invoked method.</returns>
		public virtual string MethodName
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).MethodName;
			}
		}

		/// <summary>Gets an object that contains the method signature.</summary>
		/// <returns>A <see cref="T:System.Object" /> that contains the method signature.</returns>
		public virtual object MethodSignature
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).MethodSignature;
			}
		}

		/// <summary>Gets the number of arguments in the method call that are marked as <see langword="ref" /> parameters or <see langword="out" /> parameters.</summary>
		/// <returns>A <see cref="T:System.Int32" /> that represents the number of arguments in the method call that are marked as <see langword="ref" /> parameters or <see langword="out" /> parameters.</returns>
		public virtual int OutArgCount
		{
			[SecurityCritical]
			get
			{
				if (this._outArgInfo == null)
				{
					return 0;
				}
				return this._outArgInfo.GetInOutArgCount();
			}
		}

		/// <summary>Gets an array of arguments in the method call that are marked as <see langword="ref" /> parameters or <see langword="out" /> parameters.</summary>
		/// <returns>An array of type <see cref="T:System.Object" /> that represents the arguments in the method call that are marked as <see langword="ref" /> parameters or <see langword="out" /> parameters.</returns>
		public virtual object[] OutArgs
		{
			[SecurityCritical]
			get
			{
				if (this._outArgInfo == null)
				{
					return this._args;
				}
				return this._outArgInfo.GetInOutArgs(this._args);
			}
		}

		/// <summary>An <see cref="T:System.Collections.IDictionary" /> interface that represents a collection of the remoting message's properties.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionary" /> interface that represents a collection of the remoting message's properties.</returns>
		public virtual IDictionary Properties
		{
			[SecurityCritical]
			get
			{
				if (this._properties == null)
				{
					this._properties = new MethodReturnMessageWrapper.DictionaryWrapper(this, this.WrappedMessage.Properties);
				}
				return this._properties;
			}
		}

		/// <summary>Gets the return value of the method call.</summary>
		/// <returns>A <see cref="T:System.Object" /> that represents the return value of the method call.</returns>
		public virtual object ReturnValue
		{
			[SecurityCritical]
			get
			{
				return this._return;
			}
			set
			{
				this._return = value;
			}
		}

		/// <summary>Gets the full type name of the remote object on which the method call is being made.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the full type name of the remote object on which the method call is being made.</returns>
		public virtual string TypeName
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).TypeName;
			}
		}

		/// <summary>Gets the Uniform Resource Identifier (URI) of the remote object on which the method call is being made.</summary>
		/// <returns>The URI of a remote object.</returns>
		public string Uri
		{
			[SecurityCritical]
			get
			{
				return ((IMethodReturnMessage)this.WrappedMessage).Uri;
			}
			set
			{
				this.Properties["__Uri"] = value;
			}
		}

		/// <summary>Gets a method argument, as an object, at a specified index.</summary>
		/// <param name="argNum">The index of the requested argument.</param>
		/// <returns>The method argument as an object.</returns>
		[SecurityCritical]
		public virtual object GetArg(int argNum)
		{
			return this._args[argNum];
		}

		/// <summary>Gets the name of a method argument at a specified index.</summary>
		/// <param name="index">The index of the requested argument.</param>
		/// <returns>The name of the method argument.</returns>
		[SecurityCritical]
		public virtual string GetArgName(int index)
		{
			return ((IMethodReturnMessage)this.WrappedMessage).GetArgName(index);
		}

		/// <summary>Returns the specified argument marked as a <see langword="ref" /> parameter or an <see langword="out" /> parameter.</summary>
		/// <param name="argNum">The index of the requested argument.</param>
		/// <returns>The specified argument marked as a <see langword="ref" /> parameter or an <see langword="out" /> parameter.</returns>
		[SecurityCritical]
		public virtual object GetOutArg(int argNum)
		{
			return this._args[this._outArgInfo.GetInOutArgIndex(argNum)];
		}

		/// <summary>Returns the name of the specified argument marked as a <see langword="ref" /> parameter or an <see langword="out" /> parameter.</summary>
		/// <param name="index">The index of the requested argument.</param>
		/// <returns>The argument name, or <see langword="null" /> if the current method is not implemented.</returns>
		[SecurityCritical]
		public virtual string GetOutArgName(int index)
		{
			return this._outArgInfo.GetInOutArgName(index);
		}

		private object[] _args;

		private ArgInfo _outArgInfo;

		private MethodReturnMessageWrapper.DictionaryWrapper _properties;

		private Exception _exception;

		private object _return;

		private class DictionaryWrapper : MethodReturnDictionary
		{
			public DictionaryWrapper(IMethodReturnMessage message, IDictionary wrappedDictionary) : base(message)
			{
				this._wrappedDictionary = wrappedDictionary;
				base.MethodKeys = MethodReturnMessageWrapper.DictionaryWrapper._keys;
			}

			protected override IDictionary AllocInternalProperties()
			{
				return this._wrappedDictionary;
			}

			protected override void SetMethodProperty(string key, object value)
			{
				if (key == "__Args")
				{
					((MethodReturnMessageWrapper)this._message)._args = (object[])value;
					return;
				}
				if (key == "__Return")
				{
					((MethodReturnMessageWrapper)this._message)._return = value;
					return;
				}
				base.SetMethodProperty(key, value);
			}

			protected override object GetMethodProperty(string key)
			{
				if (key == "__Args")
				{
					return ((MethodReturnMessageWrapper)this._message)._args;
				}
				if (key == "__Return")
				{
					return ((MethodReturnMessageWrapper)this._message)._return;
				}
				return base.GetMethodProperty(key);
			}

			private IDictionary _wrappedDictionary;

			private static string[] _keys = new string[]
			{
				"__Args",
				"__Return"
			};
		}
	}
}
