using System;

namespace System.Net.Mime
{
	/// <summary>Supplies the strings used to specify the disposition type for an email attachment.</summary>
	public static class DispositionTypeNames
	{
		/// <summary>Specifies that the attachment is to be displayed as part of the email message body.</summary>
		public const string Inline = "inline";

		/// <summary>Specifies that the attachment is to be displayed as a file attached to the email message.</summary>
		public const string Attachment = "attachment";
	}
}
