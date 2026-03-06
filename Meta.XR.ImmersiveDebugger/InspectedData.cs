using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	[CreateAssetMenu(fileName = "InspectedData", menuName = "Meta/ImmersiveDebugger/InspectedData", order = 100)]
	public class InspectedData : ScriptableObject
	{
		internal IEnumerable<Type> ExtractTypesFromInspectedMembers()
		{
			HashSet<Type> hashSet = new HashSet<Type>();
			foreach (InspectedMember inspectedMember in this.InspectedMembers)
			{
				inspectedMember.Initialize();
				if (inspectedMember.Valid)
				{
					Type declaringType = inspectedMember.MemberInfo.DeclaringType;
					if (!(declaringType == null))
					{
						hashSet.Add(declaringType);
						InspectedDataRegistry.Add(declaringType, inspectedMember);
					}
				}
			}
			return hashSet;
		}

		[Tooltip("The name of the InspectedData, used to manage this asset in Immersive Debugger settings")]
		[SerializeField]
		internal string DisplayName;

		[SerializeField]
		internal List<InspectedMember> InspectedMembers = new List<InspectedMember>();
	}
}
