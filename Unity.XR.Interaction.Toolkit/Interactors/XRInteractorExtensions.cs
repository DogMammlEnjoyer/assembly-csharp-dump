using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public static class XRInteractorExtensions
	{
		public static bool IsBlockedByInteractionWithinGroup(this IXRInteractor interactor)
		{
			IXRGroupMember ixrgroupMember = interactor as IXRGroupMember;
			if (ixrgroupMember == null)
			{
				return false;
			}
			IXRInteractionGroup topLevelContainingGroup = ixrgroupMember.GetTopLevelContainingGroup();
			if (topLevelContainingGroup == null)
			{
				return false;
			}
			IXRInteractor activeInteractor = topLevelContainingGroup.activeInteractor;
			return activeInteractor != null && activeInteractor != interactor;
		}
	}
}
