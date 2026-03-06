using System;
using Unity;

namespace System.Drawing
{
	/// <summary>Each property of the <see cref="T:System.Drawing.SystemBrushes" /> class is a <see cref="T:System.Drawing.SolidBrush" /> that is the color of a Windows display element.</summary>
	public static class SystemBrushes
	{
		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the active window's border.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the active window's border.</returns>
		public static Brush ActiveBorder
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ActiveBorder);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of the active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of the active window's title bar.</returns>
		public static Brush ActiveCaption
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ActiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text in the active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of the active window's title bar.</returns>
		public static Brush ActiveCaptionText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ActiveCaptionText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the application workspace.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the application workspace.</returns>
		public static Brush AppWorkspace
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.AppWorkspace);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the face color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the face color of a 3-D element.</returns>
		public static Brush ButtonFace
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ButtonFace);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the highlight color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the highlight color of a 3-D element.</returns>
		public static Brush ButtonHighlight
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ButtonHighlight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the shadow color of a 3-D element.</returns>
		public static Brush ButtonShadow
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ButtonShadow);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the face color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the face color of a 3-D element.</returns>
		public static Brush Control
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.Control);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the highlight color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the highlight color of a 3-D element.</returns>
		public static Brush ControlLightLight
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ControlLightLight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the light color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the light color of a 3-D element.</returns>
		public static Brush ControlLight
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ControlLight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the shadow color of a 3-D element.</returns>
		public static Brush ControlDark
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ControlDark);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the dark shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the dark shadow color of a 3-D element.</returns>
		public static Brush ControlDarkDark
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ControlDarkDark);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of text in a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of text in a 3-D element.</returns>
		public static Brush ControlText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ControlText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the desktop.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the desktop.</returns>
		public static Brush Desktop
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.Desktop);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the lightest color in the color gradient of an active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the lightest color in the color gradient of an active window's title bar.</returns>
		public static Brush GradientActiveCaption
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.GradientActiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the lightest color in the color gradient of an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the lightest color in the color gradient of an inactive window's title bar.</returns>
		public static Brush GradientInactiveCaption
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.GradientInactiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of dimmed text.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of dimmed text.</returns>
		public static Brush GrayText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.GrayText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of selected items.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of selected items.</returns>
		public static Brush Highlight
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.Highlight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text of selected items.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text of selected items.</returns>
		public static Brush HighlightText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.HighlightText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color used to designate a hot-tracked item.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color used to designate a hot-tracked item.</returns>
		public static Brush HotTrack
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.HotTrack);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of an inactive window's title bar.</returns>
		public static Brush InactiveCaption
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.InactiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of an inactive window's border.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of an inactive window's border.</returns>
		public static Brush InactiveBorder
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.InactiveBorder);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text in an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text in an inactive window's title bar.</returns>
		public static Brush InactiveCaptionText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.InactiveCaptionText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of a ToolTip.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of a ToolTip.</returns>
		public static Brush Info
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.Info);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text of a ToolTip.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> is the color of the text of a ToolTip.</returns>
		public static Brush InfoText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.InfoText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of a menu's background.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of a menu's background.</returns>
		public static Brush Menu
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.Menu);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of a menu bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of a menu bar.</returns>
		public static Brush MenuBar
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.MenuBar);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color used to highlight menu items when the menu appears as a flat menu.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color used to highlight menu items when the menu appears as a flat menu.</returns>
		public static Brush MenuHighlight
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.MenuHighlight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of a menu's text.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of a menu's text.</returns>
		public static Brush MenuText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.MenuText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of a scroll bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background of a scroll bar.</returns>
		public static Brush ScrollBar
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.ScrollBar);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background in the client area of a window.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the background in the client area of a window.</returns>
		public static Brush Window
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.Window);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of a window frame.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of a window frame.</returns>
		public static Brush WindowFrame
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.WindowFrame);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text in the client area of a window.</summary>
		/// <returns>A <see cref="T:System.Drawing.SolidBrush" /> that is the color of the text in the client area of a window.</returns>
		public static Brush WindowText
		{
			get
			{
				return SystemBrushes.FromSystemColor(SystemColors.WindowText);
			}
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Brush" /> from the specified <see cref="T:System.Drawing.Color" /> structure.</summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color" /> structure from which to create the <see cref="T:System.Drawing.Brush" />.</param>
		/// <returns>The <see cref="T:System.Drawing.Brush" /> this method creates.</returns>
		public static Brush FromSystemColor(Color c)
		{
			if (!c.IsSystemColor())
			{
				throw new ArgumentException(SR.Format("The color {0} is not a system color.", new object[]
				{
					c.ToString()
				}));
			}
			Brush[] array = (Brush[])SafeNativeMethods.Gdip.ThreadData[SystemBrushes.s_systemBrushesKey];
			if (array == null)
			{
				array = new Brush[33];
				SafeNativeMethods.Gdip.ThreadData[SystemBrushes.s_systemBrushesKey] = array;
			}
			int num = (int)c.ToKnownColor();
			if (num > 167)
			{
				num -= 141;
			}
			num--;
			if (array[num] == null)
			{
				array[num] = new SolidBrush(c, true);
			}
			return array[num];
		}

		internal SystemBrushes()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private static readonly object s_systemBrushesKey = new object();
	}
}
