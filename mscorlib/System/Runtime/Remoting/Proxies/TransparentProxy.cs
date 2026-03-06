using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Mono;

namespace System.Runtime.Remoting.Proxies
{
	[StructLayout(LayoutKind.Sequential)]
	internal class TransparentProxy
	{
		internal RuntimeType GetProxyType()
		{
			return (RuntimeType)Type.GetTypeFromHandle(this._class.ProxyClass.GetTypeHandle());
		}

		private bool IsContextBoundObject
		{
			get
			{
				return this.GetProxyType().IsContextful;
			}
		}

		private Context TargetContext
		{
			get
			{
				return this._rp._targetContext;
			}
		}

		private bool InCurrentContext()
		{
			return this.IsContextBoundObject && this.TargetContext == Thread.CurrentContext;
		}

		internal object LoadRemoteFieldNew(IntPtr classPtr, IntPtr fieldPtr)
		{
			RuntimeClassHandle runtimeClassHandle = new RuntimeClassHandle(classPtr);
			RuntimeFieldHandle handle = new RuntimeFieldHandle(fieldPtr);
			RuntimeTypeHandle typeHandle = runtimeClassHandle.GetTypeHandle();
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(handle);
			if (this.InCurrentContext())
			{
				object server = this._rp._server;
				return fieldFromHandle.GetValue(server);
			}
			string fullName = Type.GetTypeFromHandle(typeHandle).FullName;
			string name = fieldFromHandle.Name;
			object[] in_args = new object[]
			{
				fullName,
				name
			};
			object[] out_args = new object[1];
			MethodInfo method = typeof(object).GetMethod("FieldGetter", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
			{
				throw new MissingMethodException("System.Object", "FieldGetter");
			}
			MonoMethodMessage msg = new MonoMethodMessage(method, in_args, out_args);
			Exception ex;
			object[] array;
			RealProxy.PrivateInvoke(this._rp, msg, out ex, out array);
			if (ex != null)
			{
				throw ex;
			}
			return array[0];
		}

		internal void StoreRemoteField(IntPtr classPtr, IntPtr fieldPtr, object arg)
		{
			RuntimeClassHandle runtimeClassHandle = new RuntimeClassHandle(classPtr);
			RuntimeFieldHandle handle = new RuntimeFieldHandle(fieldPtr);
			RuntimeTypeHandle typeHandle = runtimeClassHandle.GetTypeHandle();
			FieldInfo fieldFromHandle = FieldInfo.GetFieldFromHandle(handle);
			if (this.InCurrentContext())
			{
				object server = this._rp._server;
				fieldFromHandle.SetValue(server, arg);
				return;
			}
			string fullName = Type.GetTypeFromHandle(typeHandle).FullName;
			string name = fieldFromHandle.Name;
			object[] in_args = new object[]
			{
				fullName,
				name,
				arg
			};
			MethodInfo method = typeof(object).GetMethod("FieldSetter", BindingFlags.Instance | BindingFlags.NonPublic);
			if (method == null)
			{
				throw new MissingMethodException("System.Object", "FieldSetter");
			}
			MonoMethodMessage msg = new MonoMethodMessage(method, in_args, null);
			Exception ex;
			object[] array;
			RealProxy.PrivateInvoke(this._rp, msg, out ex, out array);
			if (ex != null)
			{
				throw ex;
			}
		}

		public RealProxy _rp;

		private RuntimeRemoteClassHandle _class;

		private bool _custom_type_info;
	}
}
