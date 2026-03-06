using System;
using System.ComponentModel;

namespace System.Drawing
{
	/// <summary>Translates colors to and from GDI+ <see cref="T:System.Drawing.Color" /> structures. This class cannot be inherited.</summary>
	public sealed class ColorTranslator
	{
		private ColorTranslator()
		{
		}

		/// <summary>Translates an HTML color representation to a GDI+ <see cref="T:System.Drawing.Color" /> structure.</summary>
		/// <param name="htmlColor">The string representation of the Html color to translate.</param>
		/// <returns>The <see cref="T:System.Drawing.Color" /> structure that represents the translated HTML color or <see cref="F:System.Drawing.Color.Empty" /> if <paramref name="htmlColor" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.Exception">
		///   <paramref name="htmlColor" /> is not a valid HTML color name.</exception>
		public static Color FromHtml(string htmlColor)
		{
			if (string.IsNullOrEmpty(htmlColor))
			{
				return Color.Empty;
			}
			string text = htmlColor.ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2421214875U)
			{
				if (num <= 1269553309U)
				{
					if (num != 596358999U)
					{
						if (num != 599085576U)
						{
							if (num != 1269553309U)
							{
								goto IL_1D4;
							}
							if (!(text == "background"))
							{
								goto IL_1D4;
							}
							return SystemColors.Desktop;
						}
						else
						{
							if (!(text == "lightgrey"))
							{
								goto IL_1D4;
							}
							return Color.LightGray;
						}
					}
					else if (!(text == "threedlightshadow"))
					{
						goto IL_1D4;
					}
				}
				else if (num != 1827578293U)
				{
					if (num != 2013097724U)
					{
						if (num != 2421214875U)
						{
							goto IL_1D4;
						}
						if (!(text == "buttonhighlight"))
						{
							goto IL_1D4;
						}
					}
					else
					{
						if (!(text == "threedface"))
						{
							goto IL_1D4;
						}
						goto IL_198;
					}
				}
				else
				{
					if (!(text == "buttonshadow"))
					{
						goto IL_1D4;
					}
					return SystemColors.ControlDark;
				}
				return SystemColors.ControlLightLight;
			}
			if (num <= 3097911352U)
			{
				if (num != 2626440157U)
				{
					if (num != 3065004610U)
					{
						if (num != 3097911352U)
						{
							goto IL_1D4;
						}
						if (!(text == "buttonface"))
						{
							goto IL_1D4;
						}
					}
					else
					{
						if (!(text == "buttontext"))
						{
							goto IL_1D4;
						}
						return SystemColors.ControlText;
					}
				}
				else
				{
					if (!(text == "infobackground"))
					{
						goto IL_1D4;
					}
					return SystemColors.Info;
				}
			}
			else if (num != 3564260295U)
			{
				if (num != 4069790686U)
				{
					if (num != 4289079553U)
					{
						goto IL_1D4;
					}
					if (!(text == "threeddarkshadow"))
					{
						goto IL_1D4;
					}
					return SystemColors.ControlDarkDark;
				}
				else
				{
					if (!(text == "captiontext"))
					{
						goto IL_1D4;
					}
					return SystemColors.ActiveCaptionText;
				}
			}
			else
			{
				if (!(text == "threedhighlight"))
				{
					goto IL_1D4;
				}
				return SystemColors.ControlLight;
			}
			IL_198:
			return SystemColors.Control;
			IL_1D4:
			if (htmlColor[0] == '#' && htmlColor.Length == 4)
			{
				char c = htmlColor[1];
				char c2 = htmlColor[2];
				char c3 = htmlColor[3];
				htmlColor = new string(new char[]
				{
					'#',
					c,
					c,
					c2,
					c2,
					c3,
					c3
				});
			}
			return (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(htmlColor);
		}

		internal static Color FromBGR(int bgr)
		{
			Color color = Color.FromArgb(255, bgr & 255, bgr >> 8 & 255, bgr >> 16 & 255);
			Color result = KnownColors.FindColorMatch(color);
			if (!result.IsEmpty)
			{
				return result;
			}
			return color;
		}

		/// <summary>Translates an OLE color value to a GDI+ <see cref="T:System.Drawing.Color" /> structure.</summary>
		/// <param name="oleColor">The OLE color to translate.</param>
		/// <returns>The <see cref="T:System.Drawing.Color" /> structure that represents the translated OLE color.</returns>
		public static Color FromOle(int oleColor)
		{
			return ColorTranslator.FromBGR(oleColor);
		}

		/// <summary>Translates a Windows color value to a GDI+ <see cref="T:System.Drawing.Color" /> structure.</summary>
		/// <param name="win32Color">The Windows color to translate.</param>
		/// <returns>The <see cref="T:System.Drawing.Color" /> structure that represents the translated Windows color.</returns>
		public static Color FromWin32(int win32Color)
		{
			return ColorTranslator.FromBGR(win32Color);
		}

		/// <summary>Translates the specified <see cref="T:System.Drawing.Color" /> structure to an HTML string color representation.</summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color" /> structure to translate.</param>
		/// <returns>The string that represents the HTML color.</returns>
		public static string ToHtml(Color c)
		{
			if (c.IsEmpty)
			{
				return string.Empty;
			}
			if (c.IsSystemColor)
			{
				KnownColor kc = c.ToKnownColor();
				switch (kc)
				{
				case KnownColor.ActiveBorder:
				case KnownColor.ActiveCaption:
				case KnownColor.AppWorkspace:
				case KnownColor.GrayText:
				case KnownColor.Highlight:
				case KnownColor.HighlightText:
				case KnownColor.InactiveBorder:
				case KnownColor.InactiveCaption:
				case KnownColor.InactiveCaptionText:
				case KnownColor.InfoText:
				case KnownColor.Menu:
				case KnownColor.MenuText:
				case KnownColor.ScrollBar:
				case KnownColor.Window:
				case KnownColor.WindowFrame:
				case KnownColor.WindowText:
					return KnownColors.GetName(kc).ToLowerInvariant();
				case KnownColor.ActiveCaptionText:
					return "captiontext";
				case KnownColor.Control:
					return "buttonface";
				case KnownColor.ControlDark:
					return "buttonshadow";
				case KnownColor.ControlDarkDark:
					return "threeddarkshadow";
				case KnownColor.ControlLight:
					return "buttonface";
				case KnownColor.ControlLightLight:
					return "buttonhighlight";
				case KnownColor.ControlText:
					return "buttontext";
				case KnownColor.Desktop:
					return "background";
				case KnownColor.HotTrack:
					return "highlight";
				case KnownColor.Info:
					return "infobackground";
				default:
					return string.Empty;
				}
			}
			else
			{
				if (!c.IsNamedColor)
				{
					return ColorTranslator.FormatHtml((int)c.R, (int)c.G, (int)c.B);
				}
				if (c == Color.LightGray)
				{
					return "LightGrey";
				}
				return c.Name;
			}
		}

		private static char GetHexNumber(int b)
		{
			return (char)((b > 9) ? (55 + b) : (48 + b));
		}

		private static string FormatHtml(int r, int g, int b)
		{
			return new string(new char[]
			{
				'#',
				ColorTranslator.GetHexNumber(r >> 4 & 15),
				ColorTranslator.GetHexNumber(r & 15),
				ColorTranslator.GetHexNumber(g >> 4 & 15),
				ColorTranslator.GetHexNumber(g & 15),
				ColorTranslator.GetHexNumber(b >> 4 & 15),
				ColorTranslator.GetHexNumber(b & 15)
			});
		}

		/// <summary>Translates the specified <see cref="T:System.Drawing.Color" /> structure to an OLE color.</summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color" /> structure to translate.</param>
		/// <returns>The OLE color value.</returns>
		public static int ToOle(Color c)
		{
			return (int)c.B << 16 | (int)c.G << 8 | (int)c.R;
		}

		/// <summary>Translates the specified <see cref="T:System.Drawing.Color" /> structure to a Windows color.</summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color" /> structure to translate.</param>
		/// <returns>The Windows color value.</returns>
		public static int ToWin32(Color c)
		{
			return (int)c.B << 16 | (int)c.G << 8 | (int)c.R;
		}
	}
}
