using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class EnableOVRConcurrentHandsAndControllers : MonoBehaviour
	{
		private void OnEnable()
		{
			if (OVRPlugin.SetSimultaneousHandsAndControllersEnabled(true))
			{
				Debug.Log("Concurrent hands and controllers mode succesfully set.");
				return;
			}
			Debug.LogWarning("Concurrent Hands and controllers not supported.");
		}

		private void OnDisable()
		{
			if (OVRPlugin.SetSimultaneousHandsAndControllersEnabled(false))
			{
				Debug.Log("Concurrent hands and controllers mode succesfully unset.");
				return;
			}
			Debug.LogWarning("Concurrent Hands and controllers not supported.");
		}
	}
}
