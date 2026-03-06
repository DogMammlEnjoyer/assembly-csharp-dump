using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
	/// <summary>Represents a function exported by a <see cref="T:System.ComponentModel.Composition.Primitives.ComposablePart" />.</summary>
	public class ExportedDelegate
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportedDelegate" /> class.</summary>
		protected ExportedDelegate()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Composition.Primitives.ExportedDelegate" /> class for the specified part and method.</summary>
		/// <param name="instance">The part exporting the method.</param>
		/// <param name="method">The method to be exported.</param>
		public ExportedDelegate(object instance, MethodInfo method)
		{
			Requires.NotNull<MethodInfo>(method, "method");
			this._instance = instance;
			this._method = method;
		}

		/// <summary>Gets a delegate of the specified type.</summary>
		/// <param name="delegateType">The type of the delegate to return.</param>
		/// <returns>A delegate of the specified type, or <see langword="null" /> if no such delegate can be created.</returns>
		public virtual Delegate CreateDelegate(Type delegateType)
		{
			Requires.NotNull<Type>(delegateType, "delegateType");
			if (delegateType == typeof(Delegate) || delegateType == typeof(MulticastDelegate))
			{
				delegateType = this.CreateStandardDelegateType();
			}
			return Delegate.CreateDelegate(delegateType, this._instance, this._method, false);
		}

		private Type CreateStandardDelegateType()
		{
			ParameterInfo[] parameters = this._method.GetParameters();
			Type[] array = new Type[parameters.Length + 1];
			array[parameters.Length] = this._method.ReturnType;
			for (int i = 0; i < parameters.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
			}
			return Expression.GetDelegateType(array);
		}

		private object _instance;

		private MethodInfo _method;
	}
}
