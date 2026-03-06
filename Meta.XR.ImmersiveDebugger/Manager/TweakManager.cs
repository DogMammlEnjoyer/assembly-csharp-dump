using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class TweakManager : IDebugManager
	{
		public void Setup(IDebugUIPanel panel, InstanceCache cache)
		{
			this._uiPanel = panel;
			this._instanceCache = cache;
		}

		public void ProcessType(Type type)
		{
			this.TweaksDict.Remove(type);
			List<ValueTuple<MemberInfo, DebugMember>> list = new List<ValueTuple<MemberInfo, DebugMember>>();
			foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.SetProperty))
			{
				if (TweakUtils.IsMemberValidForTweak(memberInfo))
				{
					DebugMember customAttribute = memberInfo.GetCustomAttribute<DebugMember>();
					if (customAttribute != null && customAttribute.Tweakable)
					{
						list.Add(new ValueTuple<MemberInfo, DebugMember>(memberInfo, customAttribute));
					}
				}
			}
			list.AddRange(InspectedDataRegistry.GetMembersForType<MemberInfo>(type, (MemberInfo info, DebugMember attribute) => TweakUtils.IsMemberValidForTweak(info) && attribute.Tweakable));
			this.TweaksDict[type] = list;
			ManagerUtils.RebuildInspectorForType<MemberInfo>(this._uiPanel, this._instanceCache, type, list, delegate(IMember memberController, MemberInfo member, DebugMember attribute, InstanceHandle instance)
			{
				Tweak tweak = memberController.GetTweak();
				if (tweak == null || !tweak.Matches(member, instance))
				{
					if (member.IsBaseTypeEqual(typeof(Enum)))
					{
						memberController.RegisterEnum(TweakUtils.Create(member, attribute, instance, member.GetDataType()));
						return;
					}
					TweakUtils.ProcessMinMaxRange(member, attribute, instance);
					memberController.RegisterTweak(TweakUtils.Create(member, attribute, instance));
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
				return "Tweaks";
			}
		}

		public int GetCountPerType(Type type)
		{
			List<ValueTuple<MemberInfo, DebugMember>> list;
			this.TweaksDict.TryGetValue(type, out list);
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}

		internal readonly Dictionary<Type, List<ValueTuple<MemberInfo, DebugMember>>> TweaksDict = new Dictionary<Type, List<ValueTuple<MemberInfo, DebugMember>>>();

		private IDebugUIPanel _uiPanel;

		private InstanceCache _instanceCache;
	}
}
