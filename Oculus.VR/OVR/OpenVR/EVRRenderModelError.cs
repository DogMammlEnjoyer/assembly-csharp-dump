using System;

namespace OVR.OpenVR
{
	public enum EVRRenderModelError
	{
		None,
		Loading = 100,
		NotSupported = 200,
		InvalidArg = 300,
		InvalidModel,
		NoShapes,
		MultipleShapes,
		TooManyVertices,
		MultipleTextures,
		BufferTooSmall,
		NotEnoughNormals,
		NotEnoughTexCoords,
		InvalidTexture = 400
	}
}
