using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meta.XR.ImmersiveDebugger
{
	internal static class InspectedDataRegistry
	{
		internal static void Add(Type type, InspectedMember inspectedMember)
		{
			List<InspectedMember> list;
			if (!InspectedDataRegistry.InspectedMembersRegistry.TryGetValue(type, out list))
			{
				list = new List<InspectedMember>();
				InspectedDataRegistry.InspectedMembersRegistry[type] = list;
			}
			list.Add(inspectedMember);
		}

		internal static void Reset()
		{
			Dictionary<Type, List<InspectedMember>> inspectedMembersRegistry = InspectedDataRegistry.InspectedMembersRegistry;
			if (inspectedMembersRegistry == null)
			{
				return;
			}
			inspectedMembersRegistry.Clear();
		}

		internal static List<ValueTuple<T, DebugMember>> GetMembersForType<T>(Type type, Func<T, DebugMember, bool> filterCallback = null) where T : MemberInfo
		{
			List<ValueTuple<T, DebugMember>> list = new List<ValueTuple<T, DebugMember>>();
			List<InspectedMember> list2;
			if (!InspectedDataRegistry.InspectedMembersRegistry.TryGetValue(type, out list2))
			{
				return list;
			}
			foreach (InspectedMember inspectedMember in list2)
			{
				T t = inspectedMember.MemberInfo as T;
				if (t != null && (filterCallback == null || filterCallback(t, inspectedMember.attribute)))
				{
					list.Add(new ValueTuple<T, DebugMember>(t, inspectedMember.attribute));
				}
			}
			return list;
		}

		private static readonly Dictionary<Type, List<InspectedMember>> InspectedMembersRegistry = new Dictionary<Type, List<InspectedMember>>();
	}
}
