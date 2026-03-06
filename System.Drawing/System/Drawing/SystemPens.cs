using System;

namespace System.Drawing
{
	/// <summary>Each property of the <see cref="T:System.Drawing.SystemPens" /> class is a <see cref="T:System.Drawing.Pen" /> that is the color of a Windows display element and that has a width of 1 pixel.</summary>
	public sealed class SystemPens
	{
		private SystemPens()
		{
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the text in the active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the text in the active window's title bar.</returns>
		public static Pen ActiveCaptionText
		{
			get
			{
				if (SystemPens.active_caption_text == null)
				{
					SystemPens.active_caption_text = new Pen(SystemColors.ActiveCaptionText);
					SystemPens.active_caption_text.isModifiable = false;
				}
				return SystemPens.active_caption_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the face color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the face color of a 3-D element.</returns>
		public static Pen Control
		{
			get
			{
				if (SystemPens.control == null)
				{
					SystemPens.control = new Pen(SystemColors.Control);
					SystemPens.control.isModifiable = false;
				}
				return SystemPens.control;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the shadow color of a 3-D element.</returns>
		public static Pen ControlDark
		{
			get
			{
				if (SystemPens.control_dark == null)
				{
					SystemPens.control_dark = new Pen(SystemColors.ControlDark);
					SystemPens.control_dark.isModifiable = false;
				}
				return SystemPens.control_dark;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the dark shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the dark shadow color of a 3-D element.</returns>
		public static Pen ControlDarkDark
		{
			get
			{
				if (SystemPens.control_dark_dark == null)
				{
					SystemPens.control_dark_dark = new Pen(SystemColors.ControlDarkDark);
					SystemPens.control_dark_dark.isModifiable = false;
				}
				return SystemPens.control_dark_dark;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the light color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the light color of a 3-D element.</returns>
		public static Pen ControlLight
		{
			get
			{
				if (SystemPens.control_light == null)
				{
					SystemPens.control_light = new Pen(SystemColors.ControlLight);
					SystemPens.control_light.isModifiable = false;
				}
				return SystemPens.control_light;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the highlight color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the highlight color of a 3-D element.</returns>
		public static Pen ControlLightLight
		{
			get
			{
				if (SystemPens.control_light_light == null)
				{
					SystemPens.control_light_light = new Pen(SystemColors.ControlLightLight);
					SystemPens.control_light_light.isModifiable = false;
				}
				return SystemPens.control_light_light;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of text in a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of text in a 3-D element.</returns>
		public static Pen ControlText
		{
			get
			{
				if (SystemPens.control_text == null)
				{
					SystemPens.control_text = new Pen(SystemColors.ControlText);
					SystemPens.control_text.isModifiable = false;
				}
				return SystemPens.control_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of dimmed text.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of dimmed text.</returns>
		public static Pen GrayText
		{
			get
			{
				if (SystemPens.gray_text == null)
				{
					SystemPens.gray_text = new Pen(SystemColors.GrayText);
					SystemPens.gray_text.isModifiable = false;
				}
				return SystemPens.gray_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the background of selected items.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the background of selected items.</returns>
		public static Pen Highlight
		{
			get
			{
				if (SystemPens.highlight == null)
				{
					SystemPens.highlight = new Pen(SystemColors.Highlight);
					SystemPens.highlight.isModifiable = false;
				}
				return SystemPens.highlight;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the text of selected items.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the text of selected items.</returns>
		public static Pen HighlightText
		{
			get
			{
				if (SystemPens.highlight_text == null)
				{
					SystemPens.highlight_text = new Pen(SystemColors.HighlightText);
					SystemPens.highlight_text.isModifiable = false;
				}
				return SystemPens.highlight_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the text in an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the text in an inactive window's title bar.</returns>
		public static Pen InactiveCaptionText
		{
			get
			{
				if (SystemPens.inactive_caption_text == null)
				{
					SystemPens.inactive_caption_text = new Pen(SystemColors.InactiveCaptionText);
					SystemPens.inactive_caption_text.isModifiable = false;
				}
				return SystemPens.inactive_caption_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the text of a ToolTip.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the text of a ToolTip.</returns>
		public static Pen InfoText
		{
			get
			{
				if (SystemPens.info_text == null)
				{
					SystemPens.info_text = new Pen(SystemColors.InfoText);
					SystemPens.info_text.isModifiable = false;
				}
				return SystemPens.info_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of a menu's text.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of a menu's text.</returns>
		public static Pen MenuText
		{
			get
			{
				if (SystemPens.menu_text == null)
				{
					SystemPens.menu_text = new Pen(SystemColors.MenuText);
					SystemPens.menu_text.isModifiable = false;
				}
				return SystemPens.menu_text;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of a window frame.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of a window frame.</returns>
		public static Pen WindowFrame
		{
			get
			{
				if (SystemPens.window_frame == null)
				{
					SystemPens.window_frame = new Pen(SystemColors.WindowFrame);
					SystemPens.window_frame.isModifiable = false;
				}
				return SystemPens.window_frame;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the text in the client area of a window.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the text in the client area of a window.</returns>
		public static Pen WindowText
		{
			get
			{
				if (SystemPens.window_text == null)
				{
					SystemPens.window_text = new Pen(SystemColors.WindowText);
					SystemPens.window_text.isModifiable = false;
				}
				return SystemPens.window_text;
			}
		}

		/// <summary>Creates a <see cref="T:System.Drawing.Pen" /> from the specified <see cref="T:System.Drawing.Color" />.</summary>
		/// <param name="c">The <see cref="T:System.Drawing.Color" /> for the new <see cref="T:System.Drawing.Pen" />.</param>
		/// <returns>The <see cref="T:System.Drawing.Pen" /> this method creates.</returns>
		public static Pen FromSystemColor(Color c)
		{
			if (c.IsSystemColor)
			{
				return new Pen(c)
				{
					isModifiable = false
				};
			}
			throw new ArgumentException(string.Format("The color {0} is not a system color.", c));
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the active window's border.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the active window's border.</returns>
		public static Pen ActiveBorder
		{
			get
			{
				if (SystemPens.active_border == null)
				{
					SystemPens.active_border = new Pen(SystemColors.ActiveBorder);
					SystemPens.active_border.isModifiable = false;
				}
				return SystemPens.active_border;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the background of the active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the background of the active window's title bar.</returns>
		public static Pen ActiveCaption
		{
			get
			{
				if (SystemPens.active_caption == null)
				{
					SystemPens.active_caption = new Pen(SystemColors.ActiveCaption);
					SystemPens.active_caption.isModifiable = false;
				}
				return SystemPens.active_caption;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the application workspace.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the application workspace.</returns>
		public static Pen AppWorkspace
		{
			get
			{
				if (SystemPens.app_workspace == null)
				{
					SystemPens.app_workspace = new Pen(SystemColors.AppWorkspace);
					SystemPens.app_workspace.isModifiable = false;
				}
				return SystemPens.app_workspace;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the face color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the face color of a 3-D element.</returns>
		public static Pen ButtonFace
		{
			get
			{
				if (SystemPens.button_face == null)
				{
					SystemPens.button_face = new Pen(SystemColors.ButtonFace);
					SystemPens.button_face.isModifiable = false;
				}
				return SystemPens.button_face;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the highlight color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the highlight color of a 3-D element.</returns>
		public static Pen ButtonHighlight
		{
			get
			{
				if (SystemPens.button_highlight == null)
				{
					SystemPens.button_highlight = new Pen(SystemColors.ButtonHighlight);
					SystemPens.button_highlight.isModifiable = false;
				}
				return SystemPens.button_highlight;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the shadow color of a 3-D element.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the shadow color of a 3-D element.</returns>
		public static Pen ButtonShadow
		{
			get
			{
				if (SystemPens.button_shadow == null)
				{
					SystemPens.button_shadow = new Pen(SystemColors.ButtonShadow);
					SystemPens.button_shadow.isModifiable = false;
				}
				return SystemPens.button_shadow;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the Windows desktop.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the Windows desktop.</returns>
		public static Pen Desktop
		{
			get
			{
				if (SystemPens.desktop == null)
				{
					SystemPens.desktop = new Pen(SystemColors.Desktop);
					SystemPens.desktop.isModifiable = false;
				}
				return SystemPens.desktop;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the lightest color in the color gradient of an active window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the lightest color in the color gradient of an active window's title bar.</returns>
		public static Pen GradientActiveCaption
		{
			get
			{
				if (SystemPens.gradient_activecaption == null)
				{
					SystemPens.gradient_activecaption = new Pen(SystemColors.GradientActiveCaption);
					SystemPens.gradient_activecaption.isModifiable = false;
				}
				return SystemPens.gradient_activecaption;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the lightest color in the color gradient of an inactive window's title bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the lightest color in the color gradient of an inactive window's title bar.</returns>
		public static Pen GradientInactiveCaption
		{
			get
			{
				if (SystemPens.gradient_inactivecaption == null)
				{
					SystemPens.gradient_inactivecaption = new Pen(SystemColors.GradientInactiveCaption);
					SystemPens.gradient_inactivecaption.isModifiable = false;
				}
				return SystemPens.gradient_inactivecaption;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color used to designate a hot-tracked item.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color used to designate a hot-tracked item.</returns>
		public static Pen HotTrack
		{
			get
			{
				if (SystemPens.hot_track == null)
				{
					SystemPens.hot_track = new Pen(SystemColors.HotTrack);
					SystemPens.hot_track.isModifiable = false;
				}
				return SystemPens.hot_track;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> is the color of the border of an inactive window.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the border of an inactive window.</returns>
		public static Pen InactiveBorder
		{
			get
			{
				if (SystemPens.inactive_border == null)
				{
					SystemPens.inactive_border = new Pen(SystemColors.InactiveBorder);
					SystemPens.inactive_border.isModifiable = false;
				}
				return SystemPens.inactive_border;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the title bar caption of an inactive window.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the title bar caption of an inactive window.</returns>
		public static Pen InactiveCaption
		{
			get
			{
				if (SystemPens.inactive_caption == null)
				{
					SystemPens.inactive_caption = new Pen(SystemColors.InactiveCaption);
					SystemPens.inactive_caption.isModifiable = false;
				}
				return SystemPens.inactive_caption;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the background of a ToolTip.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the background of a ToolTip.</returns>
		public static Pen Info
		{
			get
			{
				if (SystemPens.info == null)
				{
					SystemPens.info = new Pen(SystemColors.Info);
					SystemPens.info.isModifiable = false;
				}
				return SystemPens.info;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of a menu's background.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of a menu's background.</returns>
		public static Pen Menu
		{
			get
			{
				if (SystemPens.menu == null)
				{
					SystemPens.menu = new Pen(SystemColors.Menu);
					SystemPens.menu.isModifiable = false;
				}
				return SystemPens.menu;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the background of a menu bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the background of a menu bar.</returns>
		public static Pen MenuBar
		{
			get
			{
				if (SystemPens.menu_bar == null)
				{
					SystemPens.menu_bar = new Pen(SystemColors.MenuBar);
					SystemPens.menu_bar.isModifiable = false;
				}
				return SystemPens.menu_bar;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color used to highlight menu items when the menu appears as a flat menu.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color used to highlight menu items when the menu appears as a flat menu.</returns>
		public static Pen MenuHighlight
		{
			get
			{
				if (SystemPens.menu_highlight == null)
				{
					SystemPens.menu_highlight = new Pen(SystemColors.MenuHighlight);
					SystemPens.menu_highlight.isModifiable = false;
				}
				return SystemPens.menu_highlight;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the background of a scroll bar.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the background of a scroll bar.</returns>
		public static Pen ScrollBar
		{
			get
			{
				if (SystemPens.scroll_bar == null)
				{
					SystemPens.scroll_bar = new Pen(SystemColors.ScrollBar);
					SystemPens.scroll_bar.isModifiable = false;
				}
				return SystemPens.scroll_bar;
			}
		}

		/// <summary>Gets a <see cref="T:System.Drawing.Pen" /> that is the color of the background in the client area of a window.</summary>
		/// <returns>A <see cref="T:System.Drawing.Pen" /> that is the color of the background in the client area of a window.</returns>
		public static Pen Window
		{
			get
			{
				if (SystemPens.window == null)
				{
					SystemPens.window = new Pen(SystemColors.Window);
					SystemPens.window.isModifiable = false;
				}
				return SystemPens.window;
			}
		}

		private static Pen active_caption_text;

		private static Pen control;

		private static Pen control_dark;

		private static Pen control_dark_dark;

		private static Pen control_light;

		private static Pen control_light_light;

		private static Pen control_text;

		private static Pen gray_text;

		private static Pen highlight;

		private static Pen highlight_text;

		private static Pen inactive_caption_text;

		private static Pen info_text;

		private static Pen menu_text;

		private static Pen window_frame;

		private static Pen window_text;

		private static Pen active_border;

		private static Pen active_caption;

		private static Pen app_workspace;

		private static Pen button_face;

		private static Pen button_highlight;

		private static Pen button_shadow;

		private static Pen desktop;

		private static Pen gradient_activecaption;

		private static Pen gradient_inactivecaption;

		private static Pen hot_track;

		private static Pen inactive_border;

		private static Pen inactive_caption;

		private static Pen info;

		private static Pen menu;

		private static Pen menu_bar;

		private static Pen menu_highlight;

		private static Pen scroll_bar;

		private static Pen window;
	}
}
