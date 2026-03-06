using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public static class XRGroupMemberExtensions
	{
		public static IXRInteractionGroup GetTopLevelContainingGroup(this IXRGroupMember groupMember)
		{
			IXRInteractionGroup ixrinteractionGroup = groupMember.containingGroup;
			IXRGroupMember ixrgroupMember;
			for (IXRInteractionGroup ixrinteractionGroup2 = ixrinteractionGroup; ixrinteractionGroup2 != null; ixrinteractionGroup2 = ((ixrgroupMember != null) ? ixrgroupMember.containingGroup : null))
			{
				ixrinteractionGroup = ixrinteractionGroup2;
				ixrgroupMember = (ixrinteractionGroup as IXRGroupMember);
			}
			return ixrinteractionGroup;
		}
	}
}
