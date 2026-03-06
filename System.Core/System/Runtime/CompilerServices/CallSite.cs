using System;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using Unity;

namespace System.Runtime.CompilerServices
{
	/// <summary>A dynamic call site base class. This type is used as a parameter type to the dynamic site targets.</summary>
	public class CallSite
	{
		internal CallSite(CallSiteBinder binder)
		{
			this._binder = binder;
		}

		/// <summary>Class responsible for binding dynamic operations on the dynamic site.</summary>
		/// <returns>The <see cref="T:System.Runtime.CompilerServices.CallSiteBinder" /> object responsible for binding dynamic operations.</returns>
		public CallSiteBinder Binder
		{
			get
			{
				return this._binder;
			}
		}

		/// <summary>Creates a call site with the given delegate type and binder.</summary>
		/// <param name="delegateType">The call site delegate type.</param>
		/// <param name="binder">The call site binder.</param>
		/// <returns>The new call site.</returns>
		public static CallSite Create(Type delegateType, CallSiteBinder binder)
		{
			ContractUtils.RequiresNotNull(delegateType, "delegateType");
			ContractUtils.RequiresNotNull(binder, "binder");
			if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
			{
				throw Error.TypeMustBeDerivedFromSystemDelegate();
			}
			CacheDict<Type, Func<CallSiteBinder, CallSite>> cacheDict = CallSite.s_siteCtors;
			if (cacheDict == null)
			{
				cacheDict = (CallSite.s_siteCtors = new CacheDict<Type, Func<CallSiteBinder, CallSite>>(100));
			}
			Func<CallSiteBinder, CallSite> func;
			if (!cacheDict.TryGetValue(delegateType, out func))
			{
				MethodInfo method = typeof(CallSite<>).MakeGenericType(new Type[]
				{
					delegateType
				}).GetMethod("Create");
				if (delegateType.IsCollectible)
				{
					return (CallSite)method.Invoke(null, new object[]
					{
						binder
					});
				}
				func = (Func<CallSiteBinder, CallSite>)method.CreateDelegate(typeof(Func<CallSiteBinder, CallSite>));
				cacheDict.Add(delegateType, func);
			}
			return func(binder);
		}

		internal CallSite()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal const string CallSiteTargetMethodName = "CallSite.Target";

		private static volatile CacheDict<Type, Func<CallSiteBinder, CallSite>> s_siteCtors;

		internal readonly CallSiteBinder _binder;

		internal bool _match;
	}
}
