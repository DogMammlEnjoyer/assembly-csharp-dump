using System;
using UnityEngine;

namespace Valve.VR
{
	[ExecuteInEditMode]
	public class SteamVR_SphericalProjection : MonoBehaviour
	{
		public void Set(Vector3 N, float phi0, float phi1, float theta0, float theta1, Vector3 uAxis, Vector3 uOrigin, float uScale, Vector3 vAxis, Vector3 vOrigin, float vScale)
		{
			if (SteamVR_SphericalProjection.material == null)
			{
				SteamVR_SphericalProjection.material = new Material(Shader.Find("Custom/SteamVR_SphericalProjection"));
			}
			SteamVR_SphericalProjection.material.SetVector("_N", new Vector4(N.x, N.y, N.z));
			SteamVR_SphericalProjection.material.SetFloat("_Phi0", phi0 * 0.017453292f);
			SteamVR_SphericalProjection.material.SetFloat("_Phi1", phi1 * 0.017453292f);
			SteamVR_SphericalProjection.material.SetFloat("_Theta0", theta0 * 0.017453292f + 1.5707964f);
			SteamVR_SphericalProjection.material.SetFloat("_Theta1", theta1 * 0.017453292f + 1.5707964f);
			SteamVR_SphericalProjection.material.SetVector("_UAxis", uAxis);
			SteamVR_SphericalProjection.material.SetVector("_VAxis", vAxis);
			SteamVR_SphericalProjection.material.SetVector("_UOrigin", uOrigin);
			SteamVR_SphericalProjection.material.SetVector("_VOrigin", vOrigin);
			SteamVR_SphericalProjection.material.SetFloat("_UScale", uScale);
			SteamVR_SphericalProjection.material.SetFloat("_VScale", vScale);
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			Graphics.Blit(src, dest, SteamVR_SphericalProjection.material);
		}

		private static Material material;
	}
}
