using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class WatchManager : IDebugManager
	{
		public void Setup(IDebugUIPanel panel, InstanceCache cache)
		{
			this._uiPanel = panel;
			this._instanceCache = cache;
		}

		public void ProcessType(Type type)
		{
			this.WatchesDict.Remove(type);
			List<ValueTuple<MemberInfo, DebugMember>> list = new List<ValueTuple<MemberInfo, DebugMember>>();
			foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty))
			{
				DebugMember customAttribute = memberInfo.GetCustomAttribute<DebugMember>();
				if (customAttribute != null && WatchManager.IsMemberValidForWatch(memberInfo))
				{
					list.Add(new ValueTuple<MemberInfo, DebugMember>(memberInfo, customAttribute));
				}
			}
			list.AddRange(InspectedDataRegistry.GetMembersForType<MemberInfo>(type, (MemberInfo info, DebugMember _) => WatchManager.IsMemberValidForWatch(info)));
			this.WatchesDict[type] = list;
			ManagerUtils.RebuildInspectorForType<MemberInfo>(this._uiPanel, this._instanceCache, type, list, delegate(IMember memberController, MemberInfo member, DebugMember attribute, InstanceHandle instance)
			{
				Watch watch = memberController.GetWatch();
				if (watch == null || !watch.Matches(member, instance))
				{
					if (member.IsTypeEqual(typeof(Texture2D)))
					{
						memberController.RegisterTexture(WatchUtils.Create(member, instance, attribute) as WatchTexture);
						return;
					}
					memberController.RegisterWatch(WatchUtils.Create(member, instance, attribute));
				}
			});
		}

		internal static bool IsMemberValidForWatch(MemberInfo member)
		{
			MemberTypes memberType = member.MemberType;
			bool flag = ((memberType == MemberTypes.Property || memberType == MemberTypes.Field) & !member.IsBaseTypeEqual(typeof(Enum))) | member.IsTypeEqual(typeof(Texture2D));
			PropertyInfo propertyInfo = member as PropertyInfo;
			return flag & (propertyInfo == null || propertyInfo.CanRead);
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
				return "Watches";
			}
		}

		public int GetCountPerType(Type type)
		{
			List<ValueTuple<MemberInfo, DebugMember>> list;
			this.WatchesDict.TryGetValue(type, out list);
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}

		internal readonly Dictionary<Type, List<ValueTuple<MemberInfo, DebugMember>>> WatchesDict = new Dictionary<Type, List<ValueTuple<MemberInfo, DebugMember>>>();

		private IDebugUIPanel _uiPanel;

		private InstanceCache _instanceCache;
	}
}
