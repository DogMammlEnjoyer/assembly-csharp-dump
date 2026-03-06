using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class ActionManager : IDebugManager
	{
		public void Setup(IDebugUIPanel uiPanel, InstanceCache instanceCache)
		{
			this._uiPanel = uiPanel;
			this._instanceCache = instanceCache;
		}

		public void ProcessType(Type type)
		{
			this.ActionsDict.Remove(type);
			List<ValueTuple<MethodInfo, DebugMember>> list = new List<ValueTuple<MethodInfo, DebugMember>>();
			foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				DebugMember customAttribute = methodInfo.GetCustomAttribute<DebugMember>();
				if (customAttribute != null)
				{
					list.Add(new ValueTuple<MethodInfo, DebugMember>(methodInfo, customAttribute));
				}
			}
			list.AddRange(InspectedDataRegistry.GetMembersForType<MethodInfo>(type, null));
			this.ActionsDict[type] = list;
			ManagerUtils.RebuildInspectorForType<MethodInfo>(this._uiPanel, this._instanceCache, type, list, delegate(IMember memberController, MethodInfo member, DebugMember attribute, InstanceHandle instance)
			{
				ActionHook action = memberController.GetAction();
				if (action == null || !action.Matches(member, instance))
				{
					memberController.RegisterAction(new ActionHook(member, instance, attribute));
				}
			});
		}

		public void ProcessTypeFromInspector(Type type, InstanceHandle handle, MemberInfo memberInfo, DebugMember memberAttribute)
		{
			throw new NotImplementedException();
		}

		public void ProcessTypeFromHierarchy(Item item, MemberInfo memberInfo)
		{
			throw new NotImplementedException();
		}

		public string TelemetryAnnotation
		{
			get
			{
				return "Actions";
			}
		}

		public int GetCountPerType(Type type)
		{
			List<ValueTuple<MethodInfo, DebugMember>> list;
			this.ActionsDict.TryGetValue(type, out list);
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}

		internal readonly Dictionary<Type, List<ValueTuple<MethodInfo, DebugMember>>> ActionsDict = new Dictionary<Type, List<ValueTuple<MethodInfo, DebugMember>>>();

		private IDebugUIPanel _uiPanel;

		private InstanceCache _instanceCache;
	}
}
