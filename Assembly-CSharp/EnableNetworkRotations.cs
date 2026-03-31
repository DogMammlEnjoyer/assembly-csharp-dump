using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

public class EnableNetworkRotations : MonoBehaviour
{
	private void OnEnable()
	{
		EnableNetworkRotations.m_enabledRotationEnablers.Add(this);
		if (EnableNetworkRotations.m_enabledRotationEnablers.Count == 1)
		{
			GTPlayerTransform.EnableNetworkRotations();
		}
	}

	private void OnDisable()
	{
		EnableNetworkRotations.m_enabledRotationEnablers.Remove(this);
		if (EnableNetworkRotations.m_enabledRotationEnablers.Count == 0)
		{
			GTPlayerTransform.DisableNetworkRotations();
		}
	}

	private static HashSet<EnableNetworkRotations> m_enabledRotationEnablers = new HashSet<EnableNetworkRotations>(2);
}
