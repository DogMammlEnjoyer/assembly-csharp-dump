using System;

namespace System.Drawing
{
	/// <summary>Each property of the <see cref="T:System.Drawing.SystemIcons" /> class is an <see cref="T:System.Drawing.Icon" /> object for Windows system-wide icons. This class cannot be inherited.</summary>
	public sealed class SystemIcons
	{
		static SystemIcons()
		{
			SystemIcons.icons[0] = new Icon("Mono.ico", true);
			SystemIcons.icons[1] = new Icon("Information.ico", true);
			SystemIcons.icons[2] = new Icon("Error.ico", true);
			SystemIcons.icons[3] = new Icon("Warning.ico", true);
			SystemIcons.icons[4] = new Icon("Question.ico", true);
			SystemIcons.icons[5] = new Icon("Shield.ico", true);
		}

		private SystemIcons()
		{
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the default application icon (WIN32: IDI_APPLICATION).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the default application icon.</returns>
		public static Icon Application
		{
			get
			{
				return SystemIcons.icons[0];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system asterisk icon (WIN32: IDI_ASTERISK).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system asterisk icon.</returns>
		public static Icon Asterisk
		{
			get
			{
				return SystemIcons.icons[1];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system error icon (WIN32: IDI_ERROR).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system error icon.</returns>
		public static Icon Error
		{
			get
			{
				return SystemIcons.icons[2];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system exclamation icon (WIN32: IDI_EXCLAMATION).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system exclamation icon.</returns>
		public static Icon Exclamation
		{
			get
			{
				return SystemIcons.icons[3];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system hand icon (WIN32: IDI_HAND).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system hand icon.</returns>
		public static Icon Hand
		{
			get
			{
				return SystemIcons.icons[2];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system information icon (WIN32: IDI_INFORMATION).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system information icon.</returns>
		public static Icon Information
		{
			get
			{
				return SystemIcons.icons[1];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system question icon (WIN32: IDI_QUESTION).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system question icon.</returns>
		public static Icon Question
		{
			get
			{
				return SystemIcons.icons[4];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the system warning icon (WIN32: IDI_WARNING).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the system warning icon.</returns>
		public static Icon Warning
		{
			get
			{
				return SystemIcons.icons[3];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the Windows logo icon (WIN32: IDI_WINLOGO).</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the Windows logo icon.</returns>
		public static Icon WinLogo
		{
			get
			{
				return SystemIcons.icons[0];
			}
		}

		/// <summary>Gets an <see cref="T:System.Drawing.Icon" /> object that contains the shield icon.</summary>
		/// <returns>An <see cref="T:System.Drawing.Icon" /> object that contains the shield icon.</returns>
		public static Icon Shield
		{
			get
			{
				return SystemIcons.icons[5];
			}
		}

		private static Icon[] icons = new Icon[6];

		private const int Application_Winlogo = 0;

		private const int Asterisk_Information = 1;

		private const int Error_Hand = 2;

		private const int Exclamation_Warning = 3;

		private const int Question_ = 4;

		private const int Shield_ = 5;
	}
}
