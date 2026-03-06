using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Gizmo;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class GizmoManagerForAddon : SubManagerForAddon
	{
		protected override bool RegisterSpecialisedWidget(IMember member, MemberInfo memberInfo, DebugMember memberAttribute, InstanceHandle handle)
		{
			GizmoManagerForAddon.<>c__DisplayClass1_0 CS$<>8__locals1 = new GizmoManagerForAddon.<>c__DisplayClass1_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.memberInfo = memberInfo;
			CS$<>8__locals1.handle = handle;
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Pose)))
			{
				memberAttribute.GizmoType = DebugGizmoType.Axis;
			}
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Vector3)))
			{
				memberAttribute.GizmoType = DebugGizmoType.Point;
			}
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Tuple<Vector3, Vector3>)))
			{
				memberAttribute.GizmoType = DebugGizmoType.Line;
			}
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Vector3[])))
			{
				memberAttribute.GizmoType = DebugGizmoType.Lines;
			}
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Tuple<Pose, float, float>)))
			{
				memberAttribute.GizmoType = DebugGizmoType.Plane;
			}
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Tuple<Vector3, float>)))
			{
				memberAttribute.GizmoType = DebugGizmoType.Cube;
			}
			if (CS$<>8__locals1.memberInfo.IsTypeEqual(typeof(Tuple<Pose, float, float, float>)))
			{
				memberAttribute.GizmoType = DebugGizmoType.Box;
			}
			if (memberAttribute.GizmoType == DebugGizmoType.None)
			{
				return false;
			}
			if (CS$<>8__locals1.memberInfo.DeclaringType == typeof(Transform) && CS$<>8__locals1.memberInfo.Name == "position")
			{
				memberAttribute.ShowGizmoByDefault = true;
			}
			GizmoRendererManager gizmoRendererManager;
			if (!this._memberToGizmoRendererManagerDict.TryGetValue(CS$<>8__locals1.memberInfo, out gizmoRendererManager) && GizmoManager.AddGizmo(CS$<>8__locals1.handle.Type, CS$<>8__locals1.memberInfo, memberAttribute, this.InstanceCache, out gizmoRendererManager))
			{
				this._memberToGizmoRendererManagerDict[CS$<>8__locals1.memberInfo] = gizmoRendererManager;
			}
			if (gizmoRendererManager == null)
			{
				return false;
			}
			GizmoHook gizmo = member.GetGizmo();
			if (gizmo == null || !gizmo.Matches(CS$<>8__locals1.memberInfo, CS$<>8__locals1.handle))
			{
				member.RegisterGizmo(new GizmoHook(CS$<>8__locals1.memberInfo, CS$<>8__locals1.handle, memberAttribute, new Action<bool>(CS$<>8__locals1.<RegisterSpecialisedWidget>g__OnStateChanged|0), new Func<bool>(CS$<>8__locals1.<RegisterSpecialisedWidget>g__GetState|1)));
			}
			return true;
		}

		public override string TelemetryAnnotation
		{
			get
			{
				return "Gizmos";
			}
		}

		private readonly Dictionary<MemberInfo, GizmoRendererManager> _memberToGizmoRendererManagerDict = new Dictionary<MemberInfo, GizmoRendererManager>();
	}
}
