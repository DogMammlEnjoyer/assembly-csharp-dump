using System;
using System.Collections.Generic;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	[Serializable]
	internal class InspectedMember : InspectedItemBase
	{
		internal List<DebugGizmoType> SupportedGizmos { get; private set; }

		public MemberInfo MemberInfo { get; private set; }

		public InspectedMember(MemberInfo member)
		{
			this.enabled = false;
			Type declaringType = member.DeclaringType;
			this.typeName = ((declaringType != null) ? declaringType.AssemblyQualifiedName : null);
			this.memberName = member.Name;
			this.attribute = new DebugMember(DebugColor.Gray);
			this.Initialize();
		}

		public void Initialize()
		{
			base.Valid = false;
			Type type = Type.GetType(this.typeName);
			if (type == null)
			{
				return;
			}
			MemberInfo[] member = type.GetMember(this.memberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (member.Length == 0)
			{
				return;
			}
			this.MemberInfo = member[0];
			base.Valid = true;
			this.SupportedGizmos = new List<DebugGizmoType>();
			this.PopulateSupportedGizmos(this.SupportedGizmos);
		}

		private void PopulateSupportedGizmos(List<DebugGizmoType> supportedGizmos)
		{
			if (supportedGizmos == null)
			{
				throw new NullReferenceException("supportedGizmos array cannot be null");
			}
			supportedGizmos.Add(DebugGizmoType.None);
			if (this.MemberInfo.IsTypeEqual(typeof(Pose)))
			{
				supportedGizmos.Add(DebugGizmoType.Axis);
			}
			if (this.MemberInfo.IsTypeEqual(typeof(Vector3)))
			{
				supportedGizmos.Add(DebugGizmoType.Point);
			}
			if (this.MemberInfo.IsTypeEqual(typeof(Tuple<Vector3, Vector3>)))
			{
				supportedGizmos.Add(DebugGizmoType.Line);
			}
			if (this.MemberInfo.IsTypeEqual(typeof(Vector3[])))
			{
				supportedGizmos.Add(DebugGizmoType.Lines);
			}
			if (this.MemberInfo.IsTypeEqual(typeof(Tuple<Pose, float, float>)))
			{
				supportedGizmos.Add(DebugGizmoType.Plane);
			}
			if (this.MemberInfo.IsTypeEqual(typeof(Tuple<Vector3, float>)))
			{
				supportedGizmos.Add(DebugGizmoType.Cube);
			}
			if (this.MemberInfo.IsTypeEqual(typeof(Tuple<Pose, float, float, float>)))
			{
				supportedGizmos.Add(DebugGizmoType.TopCenterBox);
				supportedGizmos.Add(DebugGizmoType.Box);
			}
		}

		internal const BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		[SerializeField]
		public DebugMember attribute;

		[SerializeField]
		private string memberName;

		[SerializeField]
		internal int _editorSelectedGizmoIndex;
	}
}
