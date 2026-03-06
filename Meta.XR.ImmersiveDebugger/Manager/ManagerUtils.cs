using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal static class ManagerUtils
	{
		public static void RebuildInspectorForType<T>(IDebugUIPanel panel, InstanceCache cache, Type type, List<ValueTuple<T, DebugMember>> memberPairs, ManagerUtils.RegisterMember<T> memberRegistration) where T : MemberInfo
		{
			foreach (ValueTuple<T, DebugMember> valueTuple in memberPairs)
			{
				T item = valueTuple.Item1;
				DebugMember item2 = valueTuple.Item2;
				if (item.IsStatic())
				{
					InstanceHandle instanceHandle = InstanceHandle.Static(type);
					IInspector inspector = panel.RegisterInspector(instanceHandle, new Category
					{
						Id = item2.Category
					});
					IMember member = (inspector != null) ? inspector.RegisterMember(item, item2) : null;
					if (member != null)
					{
						memberRegistration(member, item, item2, instanceHandle);
					}
				}
				else
				{
					foreach (InstanceHandle instanceHandle2 in cache.GetCacheDataForClass(type))
					{
						IInspector inspector2 = panel.RegisterInspector(instanceHandle2, new Category
						{
							Id = item2.Category
						});
						IMember member2 = (inspector2 != null) ? inspector2.RegisterMember(item, item2) : null;
						if (member2 != null)
						{
							memberRegistration(member2, item, item2, instanceHandle2);
						}
					}
				}
			}
		}

		public delegate void RegisterMember<in T>(IMember memberController, T member, DebugMember attribute, InstanceHandle instanceHandle);
	}
}
