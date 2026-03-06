using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	public static class XRGraphicsAutomatedTests
	{
		private static bool activatedFromCommandLine
		{
			get
			{
				return false;
			}
		}

		public static bool enabled { get; set; } = XRGraphicsAutomatedTests.activatedFromCommandLine;

		internal static void OverrideLayout(XRLayout layout, Camera camera)
		{
			if (XRGraphicsAutomatedTests.enabled && XRGraphicsAutomatedTests.running)
			{
				Matrix4x4 projectionMatrix = camera.projectionMatrix;
				Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
				ScriptableCullingParameters cullingParams;
				if (camera.TryGetCullingParameters(false, out cullingParams))
				{
					cullingParams.stereoProjectionMatrix = projectionMatrix;
					cullingParams.stereoViewMatrix = worldToCameraMatrix;
					cullingParams.stereoSeparationDistance = 0f;
					List<ValueTuple<Camera, XRPass>> activePasses = layout.GetActivePasses();
					for (int i = 0; i < activePasses.Count; i++)
					{
						XRPass item = activePasses[i].Item2;
						item.AssignCullingParams(item.cullingPassId, cullingParams);
						for (int j = 0; j < item.viewCount; j++)
						{
							Matrix4x4 projMatrix = projectionMatrix;
							Matrix4x4 matrix4x = worldToCameraMatrix;
							bool flag = activePasses.Count == 2 && i == 0;
							bool flag2 = activePasses.Count == 1 && j == 0;
							if (flag || flag2)
							{
								FrustumPlanes decomposeProjection = projMatrix.decomposeProjection;
								decomposeProjection.left *= 0.44f;
								decomposeProjection.right *= 0.88f;
								decomposeProjection.top *= 0.11f;
								decomposeProjection.bottom *= 0.33f;
								projMatrix = Matrix4x4.Frustum(decomposeProjection);
								matrix4x *= Matrix4x4.Translate(new Vector3(0.34f, 0.25f, -0.08f));
							}
							XRView xrView = new XRView(projMatrix, matrix4x, Matrix4x4.identity, false, item.GetViewport(j), null, null, item.GetTextureArraySlice(j));
							item.AssignView(j, xrView);
						}
					}
				}
			}
		}

		public static bool running = false;
	}
}
