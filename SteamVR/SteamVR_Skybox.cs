using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Skybox : MonoBehaviour
	{
		public void SetTextureByIndex(int i, Texture t)
		{
			switch (i)
			{
			case 0:
				this.front = t;
				return;
			case 1:
				this.back = t;
				return;
			case 2:
				this.left = t;
				return;
			case 3:
				this.right = t;
				return;
			case 4:
				this.top = t;
				return;
			case 5:
				this.bottom = t;
				return;
			default:
				return;
			}
		}

		public Texture GetTextureByIndex(int i)
		{
			switch (i)
			{
			case 0:
				return this.front;
			case 1:
				return this.back;
			case 2:
				return this.left;
			case 3:
				return this.right;
			case 4:
				return this.top;
			case 5:
				return this.bottom;
			default:
				return null;
			}
		}

		public static void SetOverride(Texture front = null, Texture back = null, Texture left = null, Texture right = null, Texture top = null, Texture bottom = null)
		{
			CVRCompositor compositor = OpenVR.Compositor;
			if (compositor != null)
			{
				Texture[] array = new Texture[]
				{
					front,
					back,
					left,
					right,
					top,
					bottom
				};
				Texture_t[] array2 = new Texture_t[6];
				for (int i = 0; i < 6; i++)
				{
					array2[i].handle = ((array[i] != null) ? array[i].GetNativeTexturePtr() : IntPtr.Zero);
					array2[i].eType = SteamVR.instance.textureType;
					array2[i].eColorSpace = EColorSpace.Auto;
				}
				EVRCompositorError evrcompositorError = compositor.SetSkyboxOverride(array2);
				if (evrcompositorError != EVRCompositorError.None)
				{
					Debug.LogError("<b>[SteamVR]</b> Failed to set skybox override with error: " + evrcompositorError.ToString());
					if (evrcompositorError == EVRCompositorError.TextureIsOnWrongDevice)
					{
						Debug.Log("<b>[SteamVR]</b> Set your graphics driver to use the same video card as the headset is plugged into for Unity.");
						return;
					}
					if (evrcompositorError == EVRCompositorError.TextureUsesUnsupportedFormat)
					{
						Debug.Log("<b>[SteamVR]</b> Ensure skybox textures are not compressed and have no mipmaps.");
					}
				}
			}
		}

		public static void ClearOverride()
		{
			CVRCompositor compositor = OpenVR.Compositor;
			if (compositor != null)
			{
				compositor.ClearSkyboxOverride();
			}
		}

		private void OnEnable()
		{
			SteamVR_Skybox.SetOverride(this.front, this.back, this.left, this.right, this.top, this.bottom);
		}

		private void OnDisable()
		{
			SteamVR_Skybox.ClearOverride();
		}

		public Texture front;

		public Texture back;

		public Texture left;

		public Texture right;

		public Texture top;

		public Texture bottom;

		public SteamVR_Skybox.CellSize StereoCellSize = SteamVR_Skybox.CellSize.x32;

		public float StereoIpdMm = 64f;

		public enum CellSize
		{
			x1024,
			x64,
			x32,
			x16,
			x8
		}
	}
}
