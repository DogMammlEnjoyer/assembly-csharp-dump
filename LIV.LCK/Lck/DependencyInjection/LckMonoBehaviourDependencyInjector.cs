using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Liv.Lck.DependencyInjection
{
	public class LckMonoBehaviourDependencyInjector
	{
		public LckMonoBehaviourDependencyInjector(LckServiceProvider lckServiceProvider)
		{
			this._lckServiceProvider = lckServiceProvider;
		}

		public void Inject(MonoBehaviour instance)
		{
			if (!LckMonoBehaviourDependencyInjector.IsInjectable(instance))
			{
				return;
			}
			Type type = instance.GetType();
			while (type != null && type != typeof(MonoBehaviour))
			{
				foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (Attribute.IsDefined(fieldInfo, typeof(InjectLckAttribute)))
					{
						object service = this._lckServiceProvider.GetService(fieldInfo.FieldType);
						if (service != null)
						{
							fieldInfo.SetValue(instance, service);
						}
					}
				}
				foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (Attribute.IsDefined(propertyInfo, typeof(InjectLckAttribute)) && propertyInfo.CanWrite)
					{
						object service2 = this._lckServiceProvider.GetService(propertyInfo.PropertyType);
						if (service2 != null)
						{
							propertyInfo.SetValue(instance, service2);
						}
					}
				}
				foreach (MethodInfo methodInfo in from member in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where Attribute.IsDefined(member, typeof(InjectLckAttribute))
				select member)
				{
					Type[] array = methodInfo.GetParameters().Select((ParameterInfo parameter) => parameter.ParameterType).ToArray<Type>();
					List<object> list = new List<object>();
					foreach (Type type2 in array)
					{
						object service3 = this._lckServiceProvider.GetService(type2);
						if (service3 == null)
						{
							throw new Exception(string.Format("Failed to inject dependency {0} into method '{1}' of class '{2}'.", type2, methodInfo.Name, type.Name));
						}
						list.Add(service3);
					}
					methodInfo.Invoke(instance, list.ToArray());
				}
				type = type.BaseType;
			}
		}

		private static bool IsInjectable(object obj)
		{
			Type type = obj.GetType();
			while (type != null && type != typeof(object))
			{
				if (type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Any((MemberInfo member) => Attribute.IsDefined(member, typeof(InjectLckAttribute))))
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		private const BindingFlags _bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly LckServiceProvider _lckServiceProvider;
	}
}
