using System;

namespace UnityEngine.Rendering.Universal
{
	internal enum RenderPassEvent2D
	{
		None = -1,
		BeforeRendering,
		BeforeRenderingLayer = 100,
		BeforeRenderingShadows = 200,
		BeforeRenderingNormals = 300,
		BeforeRenderingLights = 400,
		BeforeRenderingSprites = 500,
		AfterRenderingLayer = 600,
		BeforeRenderingPostProcessing = 700,
		AfterRenderingPostProcessing = 800,
		AfterRendering = 900
	}
}
