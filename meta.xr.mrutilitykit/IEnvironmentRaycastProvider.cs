using System;
using UnityEngine;

namespace Meta.XR
{
	internal interface IEnvironmentRaycastProvider
	{
		bool IsSupported { get; }

		void SetEnabled(bool isEnabled);

		bool IsReady { get; }

		bool Raycast(Ray ray, out EnvironmentRaycastHit hit, float maxDistance = 100f, bool reconstructNormal = true, bool allowOccludedRayOrigin = true);
	}
}
