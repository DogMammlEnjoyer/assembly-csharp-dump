using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class ReflectionSource : ISource
	{
		[TupleElementNames(new string[]
		{
			null,
			null,
			"field",
			"method"
		})]
		private Dictionary<ValueTuple<Type, string>, ValueTuple<FieldInfo, MethodInfo>> TypeCache
		{
			[return: TupleElementNames(new string[]
			{
				null,
				null,
				"field",
				"method"
			})]
			get
			{
				if (this.m_TypeCache == null)
				{
					this.m_TypeCache = new Dictionary<ValueTuple<Type, string>, ValueTuple<FieldInfo, MethodInfo>>();
				}
				return this.m_TypeCache;
			}
		}

		public ReflectionSource(SmartFormatter formatter)
		{
			formatter.Parser.AddAlphanumericSelectors();
			formatter.Parser.AddAdditionalSelectorChars("_");
			formatter.Parser.AddOperators(".");
		}

		public bool TryEvaluateSelector(ISelectorInfo selectorInfo)
		{
			object currentValue = selectorInfo.CurrentValue;
			string selector = selectorInfo.SelectorText;
			if (currentValue == null)
			{
				return false;
			}
			Type type = currentValue.GetType();
			ValueTuple<FieldInfo, MethodInfo> valueTuple;
			if (!this.TypeCache.TryGetValue(new ValueTuple<Type, string>(type, selector), out valueTuple))
			{
				foreach (MemberInfo memberInfo in from m in type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
				where string.Equals(m.Name, selector, selectorInfo.FormatDetails.Settings.GetCaseSensitivityComparison())
				select m)
				{
					MemberTypes memberType = memberInfo.MemberType;
					if (memberType == MemberTypes.Field)
					{
						FieldInfo fieldInfo = (FieldInfo)memberInfo;
						selectorInfo.Result = fieldInfo.GetValue(currentValue);
						this.TypeCache[new ValueTuple<Type, string>(type, selector)] = new ValueTuple<FieldInfo, MethodInfo>(fieldInfo, null);
						return true;
					}
					if (memberType == MemberTypes.Method || memberType == MemberTypes.Property)
					{
						MethodInfo methodInfo;
						if (memberInfo.MemberType == MemberTypes.Property)
						{
							PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
							if (!propertyInfo.CanRead)
							{
								continue;
							}
							methodInfo = propertyInfo.GetGetMethod();
						}
						else
						{
							methodInfo = (MethodInfo)memberInfo;
						}
						if (!(methodInfo == null) && methodInfo.GetParameters().Length == 0 && !(methodInfo.ReturnType == typeof(void)))
						{
							this.TypeCache[new ValueTuple<Type, string>(type, selector)] = new ValueTuple<FieldInfo, MethodInfo>(null, methodInfo);
							selectorInfo.Result = methodInfo.Invoke(currentValue, ReflectionSource.k_Empty);
							return true;
						}
					}
				}
				this.TypeCache[new ValueTuple<Type, string>(type, selector)] = new ValueTuple<FieldInfo, MethodInfo>(null, null);
				return false;
			}
			if (valueTuple.Item1 != null)
			{
				selectorInfo.Result = valueTuple.Item1.GetValue(currentValue);
				return true;
			}
			if (valueTuple.Item2 != null)
			{
				selectorInfo.Result = valueTuple.Item2.Invoke(currentValue, ReflectionSource.k_Empty);
				return true;
			}
			return false;
		}

		private static readonly object[] k_Empty = new object[0];

		[TupleElementNames(new string[]
		{
			null,
			null,
			"field",
			"method"
		})]
		private Dictionary<ValueTuple<Type, string>, ValueTuple<FieldInfo, MethodInfo>> m_TypeCache;
	}
}
