using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEngine.UIElementsModule",
		"Unity.UIElements.PlayModeTests"
	})]
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextLib.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal class TextLib
	{
		public TextLib(byte[] icuData)
		{
			this.m_Ptr = TextLib.GetInstance(icuData);
		}

		private unsafe static IntPtr GetInstance(byte[] icuData)
		{
			Span<byte> span = new Span<byte>(icuData);
			IntPtr instance_Injected;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				instance_Injected = TextLib.GetInstance_Injected(ref managedSpanWrapper);
			}
			return instance_Injected;
		}

		public NativeTextInfo GenerateText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
		{
			Debug.Assert((settings.fontStyle & FontStyles.Bold) == FontStyles.Normal);
			return this.GenerateTextInternal(settings, textGenerationInfo);
		}

		public void ProcessMeshInfos(NativeTextInfo textInfo, NativeTextGenerationSettings settings)
		{
			Span<ATGMeshInfo> span = textInfo.meshInfos.AsSpan<ATGMeshInfo>();
			for (int i = 0; i < span.Length; i++)
			{
				ref ATGMeshInfo ptr = ref span[i];
				FontAsset fontAssetByID = FontAsset.GetFontAssetByID(ptr.fontAssetId);
				ptr.fontAsset = fontAssetByID;
				ptr.textElementInfoIndicesByAtlas = new List<List<int>>(fontAssetByID.atlasTextures.Length);
				for (int j = 0; j < fontAssetByID.atlasTextures.Length; j++)
				{
					ptr.textElementInfoIndicesByAtlas.Add(new List<int>());
				}
				float num = (float)settings.vertexPadding / 64f;
				float num2 = 1f / (float)fontAssetByID.atlasWidth;
				float num3 = 1f / (float)fontAssetByID.atlasHeight;
				bool hasMultipleColors = false;
				Color? color = null;
				for (int k = 0; k < ptr.textElementInfos.Length; k++)
				{
					ref NativeTextElementInfo ptr2 = ref ptr.textElementInfos[k];
					int glyphID = ptr2.glyphID;
					Glyph glyph;
					bool flag = fontAssetByID.TryAddGlyphInternal((uint)glyphID, out glyph);
					bool flag2 = !flag;
					if (!flag2)
					{
						Color32 color2 = ptr2.topLeft.color;
						bool flag3 = color != null && color.Value != color2;
						if (flag3)
						{
							hasMultipleColors = true;
						}
						color = new Color?(color2);
						GlyphRect glyphRect = glyph.glyphRect;
						while (ptr.textElementInfoIndicesByAtlas.Count < fontAssetByID.atlasTextures.Length)
						{
							ptr.textElementInfoIndicesByAtlas.Add(new List<int>());
						}
						ptr.textElementInfoIndicesByAtlas[glyph.atlasIndex].Add(k);
						bool flag4 = (ptr2.bottomLeft.uv0.x == 0f || ptr2.bottomLeft.uv0.x == 1f) && (ptr2.bottomLeft.uv0.y == 0f || ptr2.bottomLeft.uv0.y == 1f) && (ptr2.topLeft.uv0.x == 0f || ptr2.topLeft.uv0.x == 1f) && (ptr2.topLeft.uv0.y == 0f || ptr2.topLeft.uv0.y == 1f) && (ptr2.topRight.uv0.x == 0f || ptr2.topRight.uv0.x == 1f) && (ptr2.topRight.uv0.y == 0f || ptr2.topRight.uv0.y == 1f) && (ptr2.bottomRight.uv0.x == 0f || ptr2.bottomRight.uv0.x == 1f) && (ptr2.bottomRight.uv0.y == 0f || ptr2.bottomRight.uv0.y == 1f);
						bool flag5 = flag4;
						if (flag5)
						{
							float x = ((float)glyphRect.x - num) * num2;
							float y = ((float)glyphRect.y - num) * num3;
							float x2 = ((float)(glyphRect.x + glyphRect.width) + num) * num2;
							float y2 = ((float)(glyphRect.y + glyphRect.height) + num) * num3;
							ptr2.bottomLeft.uv0 = new Vector2(x, y);
							ptr2.topLeft.uv0 = new Vector2(x, y2);
							ptr2.topRight.uv0 = new Vector2(x2, y2);
							ptr2.bottomRight.uv0 = new Vector2(x2, y);
						}
						else
						{
							Vector2 vector = new Vector2(((float)glyphRect.x - num) * num2, ((float)glyphRect.y - num) * num3);
							Vector2 vector2 = new Vector2(vector.x, ((float)(glyphRect.y + glyphRect.height) + num) * num3);
							Vector2 a = new Vector2(((float)(glyphRect.x + glyphRect.width) + num) * num2, vector2.y);
							ptr2.bottomLeft.uv0 = a * ptr2.bottomLeft.uv0 + vector * (Vector2.one - ptr2.bottomLeft.uv0);
							ptr2.topLeft.uv0 = a * ptr2.topLeft.uv0 + vector * (Vector2.one - ptr2.topLeft.uv0);
							ptr2.topRight.uv0 = a * ptr2.topRight.uv0 + vector * (Vector2.one - ptr2.topRight.uv0);
							ptr2.bottomRight.uv0 = a * ptr2.bottomRight.uv0 + vector * (Vector2.one - ptr2.bottomRight.uv0);
						}
					}
				}
				ptr.hasMultipleColors = hasMultipleColors;
			}
		}

		[NativeMethod(Name = "TextLib::GenerateTextMesh", IsThreadSafe = true)]
		private NativeTextInfo GenerateTextInternal(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
		{
			IntPtr intPtr = TextLib.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			NativeTextInfo result;
			TextLib.GenerateTextInternal_Injected(intPtr, ref settings, textGenerationInfo, out result);
			return result;
		}

		[NativeMethod(Name = "TextLib::MeasureText")]
		public Vector2 MeasureText(NativeTextGenerationSettings settings, IntPtr textGenerationInfo)
		{
			IntPtr intPtr = TextLib.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector2 result;
			TextLib.MeasureText_Injected(intPtr, ref settings, textGenerationInfo, out result);
			return result;
		}

		[NativeMethod(Name = "TextLib::FindIntersectingLink")]
		public static int FindIntersectingLink(Vector2 point, IntPtr textGenerationInfo)
		{
			return TextLib.FindIntersectingLink_Injected(ref point, textGenerationInfo);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInstance_Injected(ref ManagedSpanWrapper icuData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateTextInternal_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, out NativeTextInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MeasureText_Injected(IntPtr _unity_self, [In] ref NativeTextGenerationSettings settings, IntPtr textGenerationInfo, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FindIntersectingLink_Injected([In] ref Vector2 point, IntPtr textGenerationInfo);

		public const int k_unconstrainedScreenSize = -1;

		private readonly IntPtr m_Ptr;

		public static Func<TextAsset> GetICUAssetEditorDelegate;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(TextLib textLib)
			{
				return textLib.m_Ptr;
			}
		}
	}
}
