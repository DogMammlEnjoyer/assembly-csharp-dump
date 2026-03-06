using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Math/ColorUtility.h")]
	public class ColorUtility
	{
		[FreeFunction("TryParseHtmlColor", true)]
		internal unsafe static bool DoTryParseHtmlColor(string htmlString, out Color32 color)
		{
			bool result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(htmlString, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = htmlString.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = ColorUtility.DoTryParseHtmlColor_Injected(ref managedSpanWrapper, out color);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		public static bool TryParseHtmlString(string htmlString, out Color color)
		{
			Color32 c;
			bool result = ColorUtility.DoTryParseHtmlColor(htmlString, out c);
			color = c;
			return result;
		}

		public static string ToHtmlStringRGB(Color color)
		{
			Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), 1);
			return string.Format("{0:X2}{1:X2}{2:X2}", color2.r, color2.g, color2.b);
		}

		public static string ToHtmlStringRGBA(Color color)
		{
			Color32 color2 = new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255), (byte)Mathf.Clamp(Mathf.RoundToInt(color.a * 255f), 0, 255));
			return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", new object[]
			{
				color2.r,
				color2.g,
				color2.b,
				color2.a
			});
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoTryParseHtmlColor_Injected(ref ManagedSpanWrapper htmlString, out Color32 color);
	}
}
