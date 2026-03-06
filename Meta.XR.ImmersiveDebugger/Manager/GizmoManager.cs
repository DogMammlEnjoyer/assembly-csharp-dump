using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Gizmo;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class GizmoManager : IDebugManager
	{
		public void Setup(IDebugUIPanel panel, InstanceCache cache)
		{
			this._uiPanel = panel;
			this._instanceCache = cache;
		}

		public void ProcessType(Type type)
		{
			GizmoManager.<>c__DisplayClass4_0 CS$<>8__locals1 = new GizmoManager.<>c__DisplayClass4_0();
			CS$<>8__locals1.type = type;
			CS$<>8__locals1.<>4__this = this;
			this.RemoveGizmosForType(CS$<>8__locals1.type);
			this.GizmosDict.Remove(CS$<>8__locals1.type);
			CS$<>8__locals1.gizmosList = new List<ValueTuple<MemberInfo, GizmoRendererManager>>();
			CS$<>8__locals1.membersList = new List<ValueTuple<MemberInfo, DebugMember>>();
			CS$<>8__locals1.memberToGizmoRendererManagerDict = new Dictionary<MemberInfo, GizmoRendererManager>();
			foreach (MemberInfo memberInfo in CS$<>8__locals1.type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.GetProperty))
			{
				DebugMember customAttribute = memberInfo.GetCustomAttribute<DebugMember>();
				GizmoRendererManager gizmoRendererManager;
				if (customAttribute != null && customAttribute.GizmoType != DebugGizmoType.None && GizmoManager.AddGizmo(CS$<>8__locals1.type, memberInfo, customAttribute, this._instanceCache, out gizmoRendererManager))
				{
					CS$<>8__locals1.gizmosList.Add(new ValueTuple<MemberInfo, GizmoRendererManager>(memberInfo, gizmoRendererManager));
					CS$<>8__locals1.membersList.Add(new ValueTuple<MemberInfo, DebugMember>(memberInfo, customAttribute));
					CS$<>8__locals1.memberToGizmoRendererManagerDict[memberInfo] = gizmoRendererManager;
				}
			}
			InspectedDataRegistry.GetMembersForType<MemberInfo>(CS$<>8__locals1.type, delegate(MemberInfo info, DebugMember attribute)
			{
				if (attribute.GizmoType == DebugGizmoType.None)
				{
					return false;
				}
				GizmoRendererManager gizmoRendererManager2;
				if (!GizmoManager.AddGizmo(CS$<>8__locals1.type, info, attribute, CS$<>8__locals1.<>4__this._instanceCache, out gizmoRendererManager2))
				{
					return false;
				}
				CS$<>8__locals1.gizmosList.Add(new ValueTuple<MemberInfo, GizmoRendererManager>(info, gizmoRendererManager2));
				CS$<>8__locals1.membersList.Add(new ValueTuple<MemberInfo, DebugMember>(info, attribute));
				CS$<>8__locals1.memberToGizmoRendererManagerDict[info] = gizmoRendererManager2;
				return false;
			});
			this.GizmosDict[CS$<>8__locals1.type] = CS$<>8__locals1.gizmosList;
			ManagerUtils.RebuildInspectorForType<MemberInfo>(this._uiPanel, this._instanceCache, CS$<>8__locals1.type, CS$<>8__locals1.membersList, delegate(IMember memberController, MemberInfo member, DebugMember attribute, InstanceHandle instance)
			{
				GizmoManager.<>c__DisplayClass4_1 CS$<>8__locals2 = new GizmoManager.<>c__DisplayClass4_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.member = member;
				CS$<>8__locals2.instance = instance;
				GizmoHook gizmo = memberController.GetGizmo();
				if (gizmo == null || !gizmo.Matches(CS$<>8__locals2.member, CS$<>8__locals2.instance))
				{
					memberController.RegisterGizmo(new GizmoHook(CS$<>8__locals2.member, CS$<>8__locals2.instance, attribute, new Action<bool>(CS$<>8__locals2.<ProcessType>g__OnStateChanged|2), new Func<bool>(CS$<>8__locals2.<ProcessType>g__GetState|3)));
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

		internal static bool AddGizmo(Type type, MemberInfo member, DebugMember gizmoAttribute, InstanceCache instanceCache, out GizmoRendererManager gizmoRendererManager)
		{
			if (!GizmoTypesRegistry.IsValidDataTypeForGizmoType(member.GetDataType(), gizmoAttribute.GizmoType))
			{
				Debug.LogWarning("Invalid registration of gizmo " + member.Name + ": type not matching gizmo type");
				gizmoRendererManager = null;
				return false;
			}
			GameObject gameObject = new GameObject(member.Name + "Gizmo");
			gizmoRendererManager = gameObject.AddComponent<GizmoRendererManager>();
			gizmoRendererManager.Setup(type, member, gizmoAttribute.GizmoType, gizmoAttribute.Color, instanceCache);
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gameObject);
			}
			return true;
		}

		private void RemoveGizmosForType(Type type)
		{
			List<ValueTuple<MemberInfo, GizmoRendererManager>> list;
			if (this.GizmosDict.TryGetValue(type, out list))
			{
				foreach (ValueTuple<MemberInfo, GizmoRendererManager> valueTuple in list)
				{
					Object.Destroy(valueTuple.Item2.gameObject);
				}
				this.GizmosDict.Remove(type);
			}
		}

		public string TelemetryAnnotation
		{
			get
			{
				return "Gizmos";
			}
		}

		public int GetCountPerType(Type type)
		{
			List<ValueTuple<MemberInfo, GizmoRendererManager>> list;
			this.GizmosDict.TryGetValue(type, out list);
			if (list == null)
			{
				return 0;
			}
			return list.Count;
		}

		internal readonly Dictionary<Type, List<ValueTuple<MemberInfo, GizmoRendererManager>>> GizmosDict = new Dictionary<Type, List<ValueTuple<MemberInfo, GizmoRendererManager>>>();

		private IDebugUIPanel _uiPanel;

		private InstanceCache _instanceCache;
	}
}
