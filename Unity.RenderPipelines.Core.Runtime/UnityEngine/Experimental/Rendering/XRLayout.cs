using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine.Experimental.Rendering
{
	public class XRLayout
	{
		public void AddCamera(Camera camera, bool enableXR)
		{
			if (camera == null)
			{
				return;
			}
			bool flag = (camera.cameraType == CameraType.Game || camera.cameraType == CameraType.VR) && camera.targetTexture == null && enableXR;
			if (XRSystem.displayActive && flag)
			{
				XRSystem.SetDisplayZRange(camera.nearClipPlane, camera.farClipPlane);
				XRSystem.CreateDefaultLayout(camera, this);
				return;
			}
			this.AddPass(camera, XRSystem.emptyPass);
		}

		public void ReconfigurePass(XRPass xrPass, Camera camera)
		{
			if (xrPass.enabled)
			{
				XRSystem.ReconfigurePass(xrPass, camera);
				xrPass.UpdateCombinedOcclusionMesh();
			}
		}

		public List<ValueTuple<Camera, XRPass>> GetActivePasses()
		{
			return this.m_ActivePasses;
		}

		internal void AddPass(Camera camera, XRPass xrPass)
		{
			xrPass.UpdateCombinedOcclusionMesh();
			this.m_ActivePasses.Add(new ValueTuple<Camera, XRPass>(camera, xrPass));
		}

		internal void Clear()
		{
			for (int i = 0; i < this.m_ActivePasses.Count; i++)
			{
				XRPass item = this.m_ActivePasses[this.m_ActivePasses.Count - i - 1].Item2;
				if (item != XRSystem.emptyPass)
				{
					item.Release();
				}
			}
			this.m_ActivePasses.Clear();
		}

		internal void LogDebugInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("XRSystem setup for frame {0}, active: {1}", Time.frameCount, XRSystem.displayActive);
			stringBuilder.AppendLine();
			for (int i = 0; i < this.m_ActivePasses.Count; i++)
			{
				XRPass item = this.m_ActivePasses[i].Item2;
				for (int j = 0; j < item.viewCount; j++)
				{
					Rect viewport = item.GetViewport(j);
					stringBuilder.AppendFormat("XR Pass {0} Cull {1} View {2} Slice {3} : {4} x {5}", new object[]
					{
						item.multipassId,
						item.cullingPassId,
						j,
						item.GetTextureArraySlice(j),
						viewport.width,
						viewport.height
					});
					stringBuilder.AppendLine();
				}
			}
			Debug.Log(stringBuilder);
		}

		private readonly List<ValueTuple<Camera, XRPass>> m_ActivePasses = new List<ValueTuple<Camera, XRPass>>();
	}
}
