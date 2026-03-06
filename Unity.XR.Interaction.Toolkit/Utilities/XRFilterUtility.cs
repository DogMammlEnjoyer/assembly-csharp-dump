using System;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal static class XRFilterUtility
	{
		public static bool Process(SmallRegistrationList<IXRHoverFilter> filters, IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			if (filters.registeredSnapshot.Count == 0)
			{
				return true;
			}
			bool bufferChanges = filters.bufferChanges;
			filters.bufferChanges = true;
			bool result = true;
			try
			{
				foreach (IXRHoverFilter ixrhoverFilter in filters.registeredSnapshot)
				{
					if (ixrhoverFilter.canProcess && !ixrhoverFilter.Process(interactor, interactable))
					{
						result = false;
						break;
					}
				}
			}
			finally
			{
				if (!bufferChanges)
				{
					filters.bufferChanges = false;
				}
			}
			return result;
		}

		public static bool Process(SmallRegistrationList<IXRSelectFilter> filters, IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			if (filters.registeredSnapshot.Count == 0)
			{
				return true;
			}
			bool bufferChanges = filters.bufferChanges;
			filters.bufferChanges = true;
			bool result = true;
			try
			{
				foreach (IXRSelectFilter ixrselectFilter in filters.registeredSnapshot)
				{
					if (ixrselectFilter.canProcess && !ixrselectFilter.Process(interactor, interactable))
					{
						result = false;
						break;
					}
				}
			}
			finally
			{
				if (!bufferChanges)
				{
					filters.bufferChanges = false;
				}
			}
			return result;
		}

		public static float Process(SmallRegistrationList<IXRInteractionStrengthFilter> filters, IXRInteractor interactor, IXRInteractable interactable, float interactionStrength)
		{
			if (filters.registeredSnapshot.Count == 0)
			{
				return interactionStrength;
			}
			bool bufferChanges = filters.bufferChanges;
			filters.bufferChanges = true;
			try
			{
				foreach (IXRInteractionStrengthFilter ixrinteractionStrengthFilter in filters.registeredSnapshot)
				{
					if (ixrinteractionStrengthFilter.canProcess)
					{
						interactionStrength = ixrinteractionStrengthFilter.Process(interactor, interactable, interactionStrength);
					}
				}
			}
			finally
			{
				if (!bufferChanges)
				{
					filters.bufferChanges = false;
				}
			}
			return interactionStrength;
		}
	}
}
