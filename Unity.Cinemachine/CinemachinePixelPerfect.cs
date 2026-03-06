using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Pixel Perfect")]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachinePixelPerfect.html")]
	public class CinemachinePixelPerfect : CinemachineExtension
	{
		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage != CinemachineCore.Stage.Body)
			{
				return;
			}
			CinemachineBrain cinemachineBrain = CinemachineCore.FindPotentialTargetBrain(vcam);
			if (cinemachineBrain == null || !cinemachineBrain.IsLiveChild(vcam, false))
			{
				return;
			}
			PixelPerfectCamera pixelPerfectCamera;
			cinemachineBrain.TryGetComponent<PixelPerfectCamera>(out pixelPerfectCamera);
			if (pixelPerfectCamera == null || !pixelPerfectCamera.isActiveAndEnabled)
			{
				return;
			}
			LensSettings lens = state.Lens;
			lens.OrthographicSize = pixelPerfectCamera.CorrectCinemachineOrthoSize(lens.OrthographicSize);
			state.Lens = lens;
		}
	}
}
