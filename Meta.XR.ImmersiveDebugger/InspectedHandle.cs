using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	[Serializable]
	internal class InspectedHandle : InspectedItemBase
	{
		public InstanceHandle InstanceHandle { get; private set; }

		public Type Type { get; private set; }

		public InspectedHandle(DebugInspector owner, Type type)
		{
			this.enabled = false;
			this.typeName = type.AssemblyQualifiedName;
			this.Initialize(owner);
		}

		public void Initialize(DebugInspector owner)
		{
			base.Valid = false;
			this.Type = Type.GetType(this.typeName);
			if (this.Type == null)
			{
				return;
			}
			Component component = owner.GetComponent(this.Type);
			if (component == null)
			{
				return;
			}
			this.InstanceHandle = new InstanceHandle(this.Type, component);
			foreach (InspectedMember inspectedMember in this.inspectedMembers)
			{
				inspectedMember.Initialize();
			}
			Type type = this.Type;
			while (type != null && type != typeof(Component) && type != typeof(MonoBehaviour))
			{
				foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					InspectedMember item;
					if (!this.TryGetMember(memberInfo, out item) && memberInfo.IsCompatibleWithDebugInspector())
					{
						item = new InspectedMember(memberInfo);
						this.inspectedMembers.Add(item);
					}
				}
				type = type.BaseType;
			}
			base.Valid = true;
		}

		private bool TryGetMember(MemberInfo memberInfo, out InspectedMember inspectedMember)
		{
			inspectedMember = null;
			foreach (InspectedMember inspectedMember2 in this.inspectedMembers)
			{
				if (inspectedMember2.MemberInfo == memberInfo)
				{
					inspectedMember = inspectedMember2;
					break;
				}
			}
			return inspectedMember != null;
		}

		[SerializeField]
		public List<InspectedMember> inspectedMembers = new List<InspectedMember>();
	}
}
