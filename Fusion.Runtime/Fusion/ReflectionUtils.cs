using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fusion
{
	public static class ReflectionUtils
	{
		public static T GetCustomAttributeOrThrow<T>(this MemberInfo member, bool inherit) where T : Attribute
		{
			object[] customAttributes = member.GetCustomAttributes(typeof(T), inherit);
			bool flag = customAttributes.Length == 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("T", string.Format("{0} has no attribute {1}", member, typeof(T)));
			}
			bool flag2 = customAttributes.Length > 1;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("{0} has more than one attribute {1}", member, typeof(T)));
			}
			return (T)((object)customAttributes[0]);
		}

		public static NetworkBehaviourWeavedAttribute GetWeavedAttributeOrThrow(Type type)
		{
			NetworkBehaviourWeavedAttribute customAttributeOrThrow;
			try
			{
				customAttributeOrThrow = type.GetCustomAttributeOrThrow(false);
			}
			catch (ArgumentOutOfRangeException)
			{
				throw new InvalidOperationException(string.Format("Type {0} has not been weaved. Has the assembly {1} been added to {2}?", type, type.Assembly.GetName().Name, "NetworkProjectConfig"));
			}
			return customAttributeOrThrow;
		}

		public static IEnumerable<Assembly> GetAllWeavedAssemblies()
		{
			return new ReflectionUtils.<GetAllWeavedAssemblies>d__2(-2);
		}

		public static IEnumerable<Type> GetAllSimulationBehaviourTypes()
		{
			return new ReflectionUtils.<GetAllSimulationBehaviourTypes>d__3(-2);
		}

		public static IEnumerable<Type> GetAllWeavedSimulationBehaviourTypes()
		{
			return new ReflectionUtils.<GetAllWeavedSimulationBehaviourTypes>d__4(-2);
		}

		public static IEnumerable<Type> GetAllNetworkBehaviourTypes()
		{
			return new ReflectionUtils.<GetAllNetworkBehaviourTypes>d__5(-2);
		}

		public static IEnumerable<Type> GetAllWeavedNetworkBehaviourTypes()
		{
			return new ReflectionUtils.<GetAllWeavedNetworkBehaviourTypes>d__6(-2);
		}

		public static IEnumerable<Type> GetAllWeaverGeneratedTypes()
		{
			return new ReflectionUtils.<GetAllWeaverGeneratedTypes>d__7(-2);
		}
	}
}
