using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.InputSystem.Utilities
{
	internal static class SpriteUtilities
	{
		public unsafe static Sprite CreateCircleSprite(int radius, Color32 colour)
		{
			int num = radius * 2;
			Texture2D texture2D = new Texture2D(num, num, DefaultFormat.LDR, TextureCreationFlags.None);
			NativeArray<Color32> rawTextureData = texture2D.GetRawTextureData<Color32>();
			Color32* unsafePtr = (Color32*)rawTextureData.GetUnsafePtr<Color32>();
			UnsafeUtility.MemSet((void*)unsafePtr, 0, (long)(rawTextureData.Length * UnsafeUtility.SizeOf<Color32>()));
			uint* ptr = (uint*)UnsafeUtility.AddressOf<Color32>(ref colour);
			ulong num2 = (ulong)(*(long*)ptr << 32 | (long)((ulong)(*ptr)));
			float num3 = (float)(radius * radius);
			for (int i = -radius; i < radius; i++)
			{
				int num4 = (int)Mathf.Sqrt(num3 - (float)(i * i));
				Color32* ptr2 = unsafePtr + (i + radius) * num + radius - num4;
				for (int j = 0; j < num4; j++)
				{
					*(long*)ptr2 = (long)num2;
					ptr2 += 2;
				}
			}
			texture2D.Apply();
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)num, (float)num), new Vector2((float)radius, (float)radius), 1f, 0U, SpriteMeshType.FullRect);
		}
	}
}
