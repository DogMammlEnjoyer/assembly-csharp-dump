using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Linq.Expressions.Compiler;
using System.Reflection;
using System.Threading;

namespace System.Runtime.CompilerServices
{
	/// <summary>Dynamic site type.</summary>
	/// <typeparam name="T">The delegate type.</typeparam>
	public class CallSite<T> : CallSite where T : class
	{
		/// <summary>The update delegate. Called when the dynamic site experiences cache miss.</summary>
		/// <returns>The update delegate.</returns>
		public T Update
		{
			get
			{
				if (this._match)
				{
					return CallSite<T>.s_cachedNoMatch;
				}
				return CallSite<T>.s_cachedUpdate;
			}
		}

		private CallSite(CallSiteBinder binder) : base(binder)
		{
			this.Target = this.GetUpdateDelegate();
		}

		private CallSite() : base(null)
		{
		}

		internal CallSite<T> CreateMatchMaker()
		{
			return new CallSite<T>();
		}

		internal CallSite GetMatchmaker()
		{
			CallSite callSite = this._cachedMatchmaker;
			if (callSite != null)
			{
				callSite = Interlocked.Exchange<CallSite>(ref this._cachedMatchmaker, null);
			}
			CallSite result;
			if ((result = callSite) == null)
			{
				(result = new CallSite<T>())._match = true;
			}
			return result;
		}

		internal void ReleaseMatchmaker(CallSite matchMaker)
		{
			if (this.Rules != null)
			{
				this._cachedMatchmaker = matchMaker;
			}
		}

		/// <summary>Creates an instance of the dynamic call site, initialized with the binder responsible for the runtime binding of the dynamic operations at this call site.</summary>
		/// <param name="binder">The binder responsible for the runtime binding of the dynamic operations at this call site.</param>
		/// <returns>The new instance of dynamic call site.</returns>
		public static CallSite<T> Create(CallSiteBinder binder)
		{
			if (!typeof(T).IsSubclassOf(typeof(MulticastDelegate)))
			{
				throw Error.TypeMustBeDerivedFromSystemDelegate();
			}
			ContractUtils.RequiresNotNull(binder, "binder");
			return new CallSite<T>(binder);
		}

		private T GetUpdateDelegate()
		{
			return this.GetUpdateDelegate(ref CallSite<T>.s_cachedUpdate);
		}

		private T GetUpdateDelegate(ref T addr)
		{
			if (addr == null)
			{
				addr = this.MakeUpdateDelegate();
			}
			return addr;
		}

		private void ClearRuleCache()
		{
			base.Binder.GetRuleCache<T>();
			Dictionary<Type, object> cache = base.Binder.Cache;
			if (cache != null)
			{
				Dictionary<Type, object> obj = cache;
				lock (obj)
				{
					cache.Clear();
				}
			}
		}

		internal void AddRule(T newRule)
		{
			T[] rules = this.Rules;
			if (rules == null)
			{
				this.Rules = new T[]
				{
					newRule
				};
				return;
			}
			T[] array;
			if (rules.Length < 9)
			{
				array = new T[rules.Length + 1];
				Array.Copy(rules, 0, array, 1, rules.Length);
			}
			else
			{
				array = new T[10];
				Array.Copy(rules, 0, array, 1, 9);
			}
			array[0] = newRule;
			this.Rules = array;
		}

		internal void MoveRule(int i)
		{
			if (i > 1)
			{
				T[] rules = this.Rules;
				T t = rules[i];
				rules[i] = rules[i - 1];
				rules[i - 1] = rules[i - 2];
				rules[i - 2] = t;
			}
		}

		internal T MakeUpdateDelegate()
		{
			Type typeFromHandle = typeof(T);
			MethodInfo invokeMethod = typeFromHandle.GetInvokeMethod();
			Type[] array;
			if (typeFromHandle.IsGenericType && CallSite<T>.IsSimpleSignature(invokeMethod, out array))
			{
				MethodInfo methodInfo = null;
				MethodInfo methodInfo2 = null;
				if (invokeMethod.ReturnType == typeof(void))
				{
					if (typeFromHandle == DelegateHelpers.GetActionType(array.AddFirst(typeof(CallSite))))
					{
						methodInfo = typeof(UpdateDelegates).GetMethod("UpdateAndExecuteVoid" + array.Length.ToString(), BindingFlags.Static | BindingFlags.NonPublic);
						methodInfo2 = typeof(UpdateDelegates).GetMethod("NoMatchVoid" + array.Length.ToString(), BindingFlags.Static | BindingFlags.NonPublic);
					}
				}
				else if (typeFromHandle == DelegateHelpers.GetFuncType(array.AddFirst(typeof(CallSite))))
				{
					methodInfo = typeof(UpdateDelegates).GetMethod("UpdateAndExecute" + (array.Length - 1).ToString(), BindingFlags.Static | BindingFlags.NonPublic);
					methodInfo2 = typeof(UpdateDelegates).GetMethod("NoMatch" + (array.Length - 1).ToString(), BindingFlags.Static | BindingFlags.NonPublic);
				}
				if (methodInfo != null)
				{
					CallSite<T>.s_cachedNoMatch = (T)((object)methodInfo2.MakeGenericMethod(array).CreateDelegate(typeFromHandle));
					return (T)((object)methodInfo.MakeGenericMethod(array).CreateDelegate(typeFromHandle));
				}
			}
			CallSite<T>.s_cachedNoMatch = this.CreateCustomNoMatchDelegate(invokeMethod);
			return this.CreateCustomUpdateDelegate(invokeMethod);
		}

		private static bool IsSimpleSignature(MethodInfo invoke, out Type[] sig)
		{
			ParameterInfo[] parametersCached = invoke.GetParametersCached();
			ContractUtils.Requires(parametersCached.Length != 0 && parametersCached[0].ParameterType == typeof(CallSite), "T");
			Type[] array = new Type[(invoke.ReturnType != typeof(void)) ? parametersCached.Length : (parametersCached.Length - 1)];
			bool result = true;
			for (int i = 1; i < parametersCached.Length; i++)
			{
				ParameterInfo parameterInfo = parametersCached[i];
				if (parameterInfo.IsByRefParameter())
				{
					result = false;
				}
				array[i - 1] = parameterInfo.ParameterType;
			}
			if (invoke.ReturnType != typeof(void))
			{
				array[array.Length - 1] = invoke.ReturnType;
			}
			sig = array;
			return result;
		}

		private T CreateCustomUpdateDelegate(MethodInfo invoke)
		{
			Type returnType = invoke.GetReturnType();
			bool flag = returnType == typeof(void);
			ArrayBuilder<Expression> builder = new ArrayBuilder<Expression>(13);
			ArrayBuilder<ParameterExpression> builder2 = new ArrayBuilder<ParameterExpression>(8 + (flag ? 0 : 1));
			ParameterExpression[] array = Array.ConvertAll<ParameterInfo, ParameterExpression>(invoke.GetParametersCached(), (ParameterInfo p) => Expression.Parameter(p.ParameterType, p.Name));
			LabelTarget labelTarget = Expression.Label(returnType);
			Type[] typeArguments = new Type[]
			{
				typeof(T)
			};
			ParameterExpression parameterExpression = array[0];
			ParameterExpression[] array2 = array.RemoveFirst<ParameterExpression>();
			ParameterExpression parameterExpression2 = Expression.Variable(typeof(CallSite<T>), "this");
			builder2.UncheckedAdd(parameterExpression2);
			builder.UncheckedAdd(Expression.Assign(parameterExpression2, Expression.Convert(parameterExpression, parameterExpression2.Type)));
			ParameterExpression parameterExpression3 = Expression.Variable(typeof(T[]), "applicable");
			builder2.UncheckedAdd(parameterExpression3);
			ParameterExpression parameterExpression4 = Expression.Variable(typeof(T), "rule");
			builder2.UncheckedAdd(parameterExpression4);
			ParameterExpression parameterExpression5 = Expression.Variable(typeof(T), "originalRule");
			builder2.UncheckedAdd(parameterExpression5);
			Expression expression = Expression.Field(parameterExpression2, "Target");
			builder.UncheckedAdd(Expression.Assign(parameterExpression5, expression));
			ParameterExpression parameterExpression6 = null;
			if (!flag)
			{
				builder2.UncheckedAdd(parameterExpression6 = Expression.Variable(labelTarget.Type, "result"));
			}
			ParameterExpression parameterExpression7 = Expression.Variable(typeof(int), "count");
			builder2.UncheckedAdd(parameterExpression7);
			ParameterExpression parameterExpression8 = Expression.Variable(typeof(int), "index");
			builder2.UncheckedAdd(parameterExpression8);
			builder.UncheckedAdd(Expression.Assign(parameterExpression, Expression.Call(CachedReflectionInfo.CallSiteOps_CreateMatchmaker.MakeGenericMethod(typeArguments), parameterExpression2)));
			Expression test = Expression.Call(CachedReflectionInfo.CallSiteOps_GetMatch, parameterExpression);
			Expression expression2 = Expression.Call(CachedReflectionInfo.CallSiteOps_ClearMatch, parameterExpression);
			Expression expression3 = parameterExpression4;
			Expression[] list = array;
			Expression expression4 = Expression.Invoke(expression3, new TrueReadOnlyCollection<Expression>(list));
			Expression arg = Expression.Call(CachedReflectionInfo.CallSiteOps_UpdateRules.MakeGenericMethod(typeArguments), parameterExpression2, parameterExpression8);
			Expression expression5;
			if (flag)
			{
				expression5 = Expression.Block(expression4, Expression.IfThen(test, Expression.Block(arg, Expression.Return(labelTarget))));
			}
			else
			{
				expression5 = Expression.Block(Expression.Assign(parameterExpression6, expression4), Expression.IfThen(test, Expression.Block(arg, Expression.Return(labelTarget, parameterExpression6))));
			}
			Expression expression6 = Expression.Assign(parameterExpression4, Expression.ArrayAccess(parameterExpression3, new TrueReadOnlyCollection<Expression>(new Expression[]
			{
				parameterExpression8
			})));
			Expression arg2 = expression6;
			LabelTarget labelTarget2 = Expression.Label();
			Expression arg3 = Expression.IfThen(Expression.Equal(parameterExpression8, parameterExpression7), Expression.Break(labelTarget2));
			Expression expression7 = Expression.PreIncrementAssign(parameterExpression8);
			builder.UncheckedAdd(Expression.IfThen(Expression.NotEqual(Expression.Assign(parameterExpression3, Expression.Call(CachedReflectionInfo.CallSiteOps_GetRules.MakeGenericMethod(typeArguments), parameterExpression2)), Expression.Constant(null, parameterExpression3.Type)), Expression.Block(Expression.Assign(parameterExpression7, Expression.ArrayLength(parameterExpression3)), Expression.Assign(parameterExpression8, Utils.Constant(0)), Expression.Loop(Expression.Block(arg3, arg2, Expression.IfThen(Expression.NotEqual(Expression.Convert(parameterExpression4, typeof(object)), Expression.Convert(parameterExpression5, typeof(object))), Expression.Block(Expression.Assign(expression, parameterExpression4), expression5, expression2)), expression7), labelTarget2, null))));
			ParameterExpression parameterExpression9 = Expression.Variable(typeof(RuleCache<T>), "cache");
			builder2.UncheckedAdd(parameterExpression9);
			builder.UncheckedAdd(Expression.Assign(parameterExpression9, Expression.Call(CachedReflectionInfo.CallSiteOps_GetRuleCache.MakeGenericMethod(typeArguments), parameterExpression2)));
			builder.UncheckedAdd(Expression.Assign(parameterExpression3, Expression.Call(CachedReflectionInfo.CallSiteOps_GetCachedRules.MakeGenericMethod(typeArguments), parameterExpression9)));
			if (flag)
			{
				expression5 = Expression.Block(expression4, Expression.IfThen(test, Expression.Return(labelTarget)));
			}
			else
			{
				expression5 = Expression.Block(Expression.Assign(parameterExpression6, expression4), Expression.IfThen(test, Expression.Return(labelTarget, parameterExpression6)));
			}
			Expression arg4 = Expression.TryFinally(expression5, Expression.IfThen(test, Expression.Block(Expression.Call(CachedReflectionInfo.CallSiteOps_AddRule.MakeGenericMethod(typeArguments), parameterExpression2, parameterExpression4), Expression.Call(CachedReflectionInfo.CallSiteOps_MoveRule.MakeGenericMethod(typeArguments), parameterExpression9, parameterExpression4, parameterExpression8))));
			arg2 = Expression.Assign(expression, expression6);
			builder.UncheckedAdd(Expression.Assign(parameterExpression8, Utils.Constant(0)));
			builder.UncheckedAdd(Expression.Assign(parameterExpression7, Expression.ArrayLength(parameterExpression3)));
			builder.UncheckedAdd(Expression.Loop(Expression.Block(arg3, arg2, arg4, expression2, expression7), labelTarget2, null));
			builder.UncheckedAdd(Expression.Assign(parameterExpression4, Expression.Constant(null, parameterExpression4.Type)));
			ParameterExpression parameterExpression10 = Expression.Variable(typeof(object[]), "args");
			Expression[] list2 = Array.ConvertAll<ParameterExpression, Expression>(array2, (ParameterExpression p) => CallSite<T>.Convert(p, typeof(object)));
			builder2.UncheckedAdd(parameterExpression10);
			builder.UncheckedAdd(Expression.Assign(parameterExpression10, Expression.NewArrayInit(typeof(object), new TrueReadOnlyCollection<Expression>(list2))));
			Expression arg5 = Expression.Assign(expression, parameterExpression5);
			arg2 = Expression.Assign(expression, Expression.Assign(parameterExpression4, Expression.Call(CachedReflectionInfo.CallSiteOps_Bind.MakeGenericMethod(typeArguments), Expression.Property(parameterExpression2, "Binder"), parameterExpression2, parameterExpression10)));
			arg4 = Expression.TryFinally(expression5, Expression.IfThen(test, Expression.Call(CachedReflectionInfo.CallSiteOps_AddRule.MakeGenericMethod(typeArguments), parameterExpression2, parameterExpression4)));
			builder.UncheckedAdd(Expression.Loop(Expression.Block(arg5, arg2, arg4, expression2), null, null));
			builder.UncheckedAdd(Expression.Default(labelTarget.Type));
			return Expression.Lambda<T>(Expression.Label(labelTarget, Expression.Block(builder2.ToReadOnly<ParameterExpression>(), builder.ToReadOnly<Expression>())), "CallSite.Target", true, new TrueReadOnlyCollection<ParameterExpression>(array)).Compile();
		}

		private T CreateCustomNoMatchDelegate(MethodInfo invoke)
		{
			ParameterExpression[] array = Array.ConvertAll<ParameterInfo, ParameterExpression>(invoke.GetParametersCached(), (ParameterInfo p) => Expression.Parameter(p.ParameterType, p.Name));
			return Expression.Lambda<T>(Expression.Block(Expression.Call(typeof(CallSiteOps).GetMethod("SetNotMatched"), array[0]), Expression.Default(invoke.GetReturnType())), new TrueReadOnlyCollection<ParameterExpression>(array)).Compile();
		}

		private static Expression Convert(Expression arg, Type type)
		{
			if (TypeUtils.AreReferenceAssignable(type, arg.Type))
			{
				return arg;
			}
			return Expression.Convert(arg, type);
		}

		/// <summary>The Level 0 cache - a delegate specialized based on the site history.</summary>
		public T Target;

		internal T[] Rules;

		internal CallSite _cachedMatchmaker;

		private static T s_cachedUpdate;

		private static volatile T s_cachedNoMatch;

		private const int MaxRules = 10;
	}
}
