using System;
using UnityEngine;

internal interface ILckVideoTextureProvider
{
	RenderTexture CameraTrackTexture { get; }
}
