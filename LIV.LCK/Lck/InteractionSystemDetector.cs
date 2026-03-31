using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Liv.Lck
{
	internal class InteractionSystemDetector
	{
		public static IReadOnlyCollection<InteractionSystemDetector.InteractionSystem> GetAvailableInteractionSystems()
		{
			InteractionSystemDetector.EnsureScanned();
			return InteractionSystemDetector._detectedSystems;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void EnsureScanned()
		{
			if (InteractionSystemDetector._scanned)
			{
				return;
			}
			InteractionSystemDetector._scanned = true;
			if (InteractionSystemDetector.AnyTypeExists(InteractionSystemDetector._xrInteractionToolkitTypeNames))
			{
				InteractionSystemDetector._detectedSystems.Add(InteractionSystemDetector.InteractionSystem.XRInteractionToolkit);
			}
			if (InteractionSystemDetector.AnyTypeExists(InteractionSystemDetector._oculusInteractionTypeNames))
			{
				InteractionSystemDetector._detectedSystems.Add(InteractionSystemDetector.InteractionSystem.OculusInteraction);
			}
		}

		private static bool AnyTypeExists(string[] typeNames)
		{
			return typeNames.Any(new Func<string, bool>(InteractionSystemDetector.TypeExists));
		}

		private static bool TypeExists(string fullTypeName)
		{
			return Type.GetType(fullTypeName, false) != null || AppDomain.CurrentDomain.GetAssemblies().Any((Assembly assembly) => InteractionSystemDetector.TypeExistsInAssembly(fullTypeName, assembly));
		}

		private static bool TypeExistsInAssembly(string fullTypeName, Assembly assembly)
		{
			try
			{
				if (assembly.GetType(fullTypeName, false) != null)
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		private static bool _scanned;

		private static readonly List<InteractionSystemDetector.InteractionSystem> _detectedSystems = new List<InteractionSystemDetector.InteractionSystem>();

		private static readonly string[] _xrInteractionToolkitTypeNames = new string[]
		{
			"UnityEngine.XR.Interaction.Toolkit.XRInteractionManager",
			"UnityEngine.XR.Interaction.Toolkit.XRBaseInteractor",
			"UnityEngine.XR.Interaction.Toolkit.XRDirectInteractor"
		};

		private static readonly string[] _oculusInteractionTypeNames = new string[]
		{
			"Oculus.Interaction.Interactor",
			"Oculus.Interaction.HandGrab.HandGrabInteractable",
			"Oculus.Interaction.Interactable",
			"Oculus.Interaction.Input.Hand"
		};

		internal enum InteractionSystem
		{
			XRInteractionToolkit,
			OculusInteraction
		}
	}
}
