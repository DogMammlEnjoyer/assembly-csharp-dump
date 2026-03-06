using System;
using Unity;

namespace System.Drawing
{
	/// <summary>Each property of the <see cref="T:System.Drawing.SystemColors" /> class is a <see cref="T:System.Drawing.Color" /> structure that is the color of a Windows display element.</summary>
	public static class SystemColors
	{
		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the active window's border.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the active window's border.</returns>
		public static Color ActiveBorder
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ActiveBorder);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background of the active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the active window's title bar.</returns>
		public static Color ActiveCaption
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ActiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the text in the active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the text in the active window's title bar.</returns>
		public static Color ActiveCaptionText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ActiveCaptionText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the application workspace.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the application workspace.</returns>
		public static Color AppWorkspace
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.AppWorkspace);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the face color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the face color of a 3-D element.</returns>
		public static Color ButtonFace
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ButtonFace);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the highlight color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the highlight color of a 3-D element.</returns>
		public static Color ButtonHighlight
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ButtonHighlight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the shadow color of a 3-D element.</returns>
		public static Color ButtonShadow
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ButtonShadow);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the face color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the face color of a 3-D element.</returns>
		public static Color Control
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.Control);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the shadow color of a 3-D element.</returns>
		public static Color ControlDark
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ControlDark);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the dark shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the dark shadow color of a 3-D element.</returns>
		public static Color ControlDarkDark
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ControlDarkDark);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the light color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the light color of a 3-D element.</returns>
		public static Color ControlLight
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ControlLight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the highlight color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the highlight color of a 3-D element.</returns>
		public static Color ControlLightLight
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ControlLightLight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of text in a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of text in a 3-D element.</returns>
		public static Color ControlText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ControlText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the desktop.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the desktop.</returns>
		public static Color Desktop
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.Desktop);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the lightest color in the color gradient of an active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the lightest color in the color gradient of an active window's title bar.</returns>
		public static Color GradientActiveCaption
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.GradientActiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the lightest color in the color gradient of an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the lightest color in the color gradient of an inactive window's title bar.</returns>
		public static Color GradientInactiveCaption
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.GradientInactiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of dimmed text.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of dimmed text.</returns>
		public static Color GrayText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.GrayText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background of selected items.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the background of selected items.</returns>
		public static Color Highlight
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.Highlight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the text of selected items.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the text of selected items.</returns>
		public static Color HighlightText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.HighlightText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color used to designate a hot-tracked item.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color used to designate a hot-tracked item.</returns>
		public static Color HotTrack
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.HotTrack);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of an inactive window's border.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of an inactive window's border.</returns>
		public static Color InactiveBorder
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.InactiveBorder);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background of an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the background of an inactive window's title bar.</returns>
		public static Color InactiveCaption
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.InactiveCaption);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the text in an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the text in an inactive window's title bar.</returns>
		public static Color InactiveCaptionText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.InactiveCaptionText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background of a ToolTip.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the background of a ToolTip.</returns>
		public static Color Info
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.Info);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the text of a ToolTip.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the text of a ToolTip.</returns>
		public static Color InfoText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.InfoText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of a menu's background.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of a menu's background.</returns>
		public static Color Menu
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.Menu);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background of a menu bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the background of a menu bar.</returns>
		public static Color MenuBar
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.MenuBar);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color used to highlight menu items when the menu appears as a flat menu.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color used to highlight menu items when the menu appears as a flat menu.</returns>
		public static Color MenuHighlight
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.MenuHighlight);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of a menu's text.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of a menu's text.</returns>
		public static Color MenuText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.MenuText);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background of a scroll bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the background of a scroll bar.</returns>
		public static Color ScrollBar
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.ScrollBar);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the background in the client area of a window.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the background in the client area of a window.</returns>
		public static Color Window
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.Window);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of a window frame.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of a window frame.</returns>
		public static Color WindowFrame
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.WindowFrame);
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Color" /> structure that is the color of the text in the client area of a window.</summary>
		/// <returns>A <see cref="T:System.Drawing.Color" /> that is the color of the text in the client area of a window.</returns>
		public static Color WindowText
		{
			get
			{
				return ColorUtil.FromKnownColor(KnownColor.WindowText);
			}
		}

		internal SystemColors()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
