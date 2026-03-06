using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	public static class TupleExtensions
	{
		public static bool IsValueTuple(this object obj)
		{
			return obj.GetType().IsValueTupleType();
		}

		public static bool IsValueTupleType(this Type type)
		{
			return type.GetTypeInfo().IsGenericType && TupleExtensions.ValueTupleTypes.Contains(type.GetGenericTypeDefinition());
		}

		public static IEnumerable<object> GetValueTupleItemObjects(this object tuple)
		{
			return from f in tuple.GetType().GetValueTupleItemFields()
			select f.GetValue(tuple);
		}

		public static IEnumerable<Type> GetValueTupleItemTypes(this Type tupleType)
		{
			return from f in tupleType.GetValueTupleItemFields()
			select f.FieldType;
		}

		public static List<FieldInfo> GetValueTupleItemFields(this Type tupleType)
		{
			List<FieldInfo> list = new List<FieldInfo>();
			int num = 1;
			FieldInfo runtimeField;
			while ((runtimeField = tupleType.GetRuntimeField(string.Format("Item{0}", num))) != null)
			{
				num++;
				list.Add(runtimeField);
			}
			return list;
		}

		public static IEnumerable<object> GetValueTupleItemObjectsFlattened(this object tuple)
		{
			foreach (object obj in tuple.GetValueTupleItemObjects())
			{
				if (obj.IsValueTuple())
				{
					foreach (object obj2 in obj.GetValueTupleItemObjectsFlattened())
					{
						yield return obj2;
					}
					IEnumerator<object> enumerator2 = null;
				}
				else
				{
					yield return obj;
				}
			}
			IEnumerator<object> enumerator = null;
			yield break;
			yield break;
		}

		private static readonly HashSet<Type> ValueTupleTypes = new HashSet<Type>(new Type[]
		{
			typeof(ValueTuple<>),
			typeof(ValueTuple<, >),
			typeof(ValueTuple<, , >),
			typeof(ValueTuple<, , , >),
			typeof(ValueTuple<, , , , >),
			typeof(ValueTuple<, , , , , >),
			typeof(ValueTuple<, , , , , , >),
			typeof(ValueTuple<, , , , , , , >)
		});
	}
}
