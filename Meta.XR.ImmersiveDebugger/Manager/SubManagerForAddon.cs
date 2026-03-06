using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal abstract class SubManagerForAddon : IDebugManager
	{
		public void Setup(IDebugUIPanel panel, InstanceCache cache)
		{
			this._uiPanel = panel;
			this.InstanceCache = cache;
		}

		public void ProcessType(Type type)
		{
			throw new NotImplementedException();
		}

		public void ProcessTypeFromInspector(Type type, InstanceHandle handle, MemberInfo memberInfo, DebugMember memberAttribute)
		{
			IMember member = this._uiPanel.RegisterInspector(handle, new Category
			{
				Id = memberAttribute.Category
			}).RegisterMember(memberInfo, memberAttribute);
			if (this.RegisterSpecialisedWidget(member, memberInfo, memberAttribute, handle))
			{
				List<MemberInfo> list;
				if (!this._dictionary.TryGetValue(type, out list))
				{
					list = new List<MemberInfo>();
					this._dictionary.Add(type, list);
				}
				if (!list.Contains(memberInfo))
				{
					list.Add(memberInfo);
				}
			}
		}

		public void ProcessTypeFromHierarchy(Item item, MemberInfo memberInfo)
		{
			InstanceHandle handle = item.Handle;
			IInspector inspector = this._uiPanel.RegisterInspector(handle, item.Category);
			DebugMember debugMember = new DebugMember(DebugColor.Gray);
			IMember member = inspector.RegisterMember(memberInfo, debugMember);
			if (this.RegisterSpecialisedWidget(member, memberInfo, debugMember, handle))
			{
				List<MemberInfo> list;
				if (!this._dictionary.TryGetValue(handle.Type, out list))
				{
					list = new List<MemberInfo>();
					this._dictionary.Add(handle.Type, list);
				}
				if (!list.Contains(memberInfo))
				{
					list.Add(memberInfo);
				}
			}
		}

		protected abstract bool RegisterSpecialisedWidget(IMember member, MemberInfo memberInfo, DebugMember memberAttribute, InstanceHandle handle);

		public abstract string TelemetryAnnotation { get; }

		public int GetCountPerType(Type type)
		{
			List<MemberInfo> list;
			this._dictionary.TryGetValue(type, out list);
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}

		private readonly Dictionary<Type, List<MemberInfo>> _dictionary = new Dictionary<Type, List<MemberInfo>>();

		private IDebugUIPanel _uiPanel;

		protected InstanceCache InstanceCache;
	}
}
