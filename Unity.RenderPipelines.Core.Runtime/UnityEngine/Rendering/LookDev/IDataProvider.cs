using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.LookDev
{
	public interface IDataProvider
	{
		void FirstInitScene(StageRuntimeInterface stage);

		void UpdateSky(Camera camera, Sky sky, StageRuntimeInterface stage);

		IEnumerable<string> supportedDebugModes { get; }

		void UpdateDebugMode(int debugIndex);

		void GetShadowMask(ref RenderTexture output, StageRuntimeInterface stage);

		void OnBeginRendering(StageRuntimeInterface stage);

		void OnEndRendering(StageRuntimeInterface stage);

		void Cleanup(StageRuntimeInterface SRI);
	}
}
