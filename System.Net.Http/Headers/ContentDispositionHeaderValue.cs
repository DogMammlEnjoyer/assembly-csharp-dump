using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the value of the Content-Disposition header.</summary>
	public class ContentDispositionHeaderValue : ICloneable
	{
		private ContentDispositionHeaderValue()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> class.</summary>
		/// <param name="dispositionType">A string that contains a <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" />.</param>
		public ContentDispositionHeaderValue(string dispositionType)
		{
			this.DispositionType = dispositionType;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> class.</summary>
		/// <param name="source">A <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" />.</param>
		protected ContentDispositionHeaderValue(ContentDispositionHeaderValue source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.dispositionType = source.dispositionType;
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue source2 in source.parameters)
				{
					this.Parameters.Add(new NameValueHeaderValue(source2));
				}
			}
		}

		/// <summary>The date at which   the file was created.</summary>
		/// <returns>The file creation date.</returns>
		public DateTimeOffset? CreationDate
		{
			get
			{
				return this.GetDateValue("creation-date");
			}
			set
			{
				this.SetDateValue("creation-date", value);
			}
		}

		/// <summary>The disposition type for a content body part.</summary>
		/// <returns>The disposition type.</returns>
		public string DispositionType
		{
			get
			{
				return this.dispositionType;
			}
			set
			{
				Parser.Token.Check(value);
				this.dispositionType = value;
			}
		}

		/// <summary>A suggestion for how to construct a filename for   storing the message payload to be used if the entity is   detached and stored in a separate file.</summary>
		/// <returns>A suggested filename.</returns>
		public string FileName
		{
			get
			{
				string text = this.FindParameter("filename");
				if (text == null)
				{
					return null;
				}
				return ContentDispositionHeaderValue.DecodeValue(text, false);
			}
			set
			{
				if (value != null)
				{
					value = ContentDispositionHeaderValue.EncodeBase64Value(value);
				}
				this.SetValue("filename", value);
			}
		}

		/// <summary>A suggestion for how to construct filenames for   storing message payloads to be used if the entities are    detached and stored in a separate files.</summary>
		/// <returns>A suggested filename of the form filename*.</returns>
		public string FileNameStar
		{
			get
			{
				string text = this.FindParameter("filename*");
				if (text == null)
				{
					return null;
				}
				return ContentDispositionHeaderValue.DecodeValue(text, true);
			}
			set
			{
				if (value != null)
				{
					value = ContentDispositionHeaderValue.EncodeRFC5987(value);
				}
				this.SetValue("filename*", value);
			}
		}

		/// <summary>The date at   which the file was last modified.</summary>
		/// <returns>The file modification date.</returns>
		public DateTimeOffset? ModificationDate
		{
			get
			{
				return this.GetDateValue("modification-date");
			}
			set
			{
				this.SetDateValue("modification-date", value);
			}
		}

		/// <summary>The name for a content body part.</summary>
		/// <returns>The name for the content body part.</returns>
		public string Name
		{
			get
			{
				string text = this.FindParameter("name");
				if (text == null)
				{
					return null;
				}
				return ContentDispositionHeaderValue.DecodeValue(text, false);
			}
			set
			{
				if (value != null)
				{
					value = ContentDispositionHeaderValue.EncodeBase64Value(value);
				}
				this.SetValue("name", value);
			}
		}

		/// <summary>A set of parameters included the Content-Disposition header.</summary>
		/// <returns>A collection of parameters.</returns>
		public ICollection<NameValueHeaderValue> Parameters
		{
			get
			{
				List<NameValueHeaderValue> result;
				if ((result = this.parameters) == null)
				{
					result = (this.parameters = new List<NameValueHeaderValue>());
				}
				return result;
			}
		}

		/// <summary>The date the file was last read.</summary>
		/// <returns>The last read date.</returns>
		public DateTimeOffset? ReadDate
		{
			get
			{
				return this.GetDateValue("read-date");
			}
			set
			{
				this.SetDateValue("read-date", value);
			}
		}

		/// <summary>The approximate size, in bytes, of the file.</summary>
		/// <returns>The approximate size, in bytes.</returns>
		public long? Size
		{
			get
			{
				long value;
				if (Parser.Long.TryParse(this.FindParameter("size"), out value))
				{
					return new long?(value);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					this.SetValue("size", null);
					return;
				}
				long? num = value;
				long num2 = 0L;
				if (num.GetValueOrDefault() < num2 & num != null)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SetValue("size", value.Value.ToString(CultureInfo.InvariantCulture));
			}
		}

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return new ContentDispositionHeaderValue(this);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			ContentDispositionHeaderValue contentDispositionHeaderValue = obj as ContentDispositionHeaderValue;
			return contentDispositionHeaderValue != null && string.Equals(contentDispositionHeaderValue.dispositionType, this.dispositionType, StringComparison.OrdinalIgnoreCase) && contentDispositionHeaderValue.parameters.SequenceEqual(this.parameters);
		}

		private string FindParameter(string name)
		{
			if (this.parameters == null)
			{
				return null;
			}
			foreach (NameValueHeaderValue nameValueHeaderValue in this.parameters)
			{
				if (string.Equals(nameValueHeaderValue.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return nameValueHeaderValue.Value;
				}
			}
			return null;
		}

		private DateTimeOffset? GetDateValue(string name)
		{
			string text = this.FindParameter(name);
			if (text == null || text == null)
			{
				return null;
			}
			if (text.Length < 3)
			{
				return null;
			}
			if (text[0] == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			DateTimeOffset value;
			if (Lexer.TryGetDateValue(text, out value))
			{
				return new DateTimeOffset?(value);
			}
			return null;
		}

		private static string EncodeBase64Value(string value)
		{
			bool flag = value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"';
			if (flag)
			{
				value = value.Substring(1, value.Length - 2);
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] > '\u007f')
				{
					Encoding utf = Encoding.UTF8;
					return string.Format("\"=?{0}?B?{1}?=\"", utf.WebName, Convert.ToBase64String(utf.GetBytes(value)));
				}
			}
			if (flag || !Lexer.IsValidToken(value))
			{
				return "\"" + value + "\"";
			}
			return value;
		}

		private static string EncodeRFC5987(string value)
		{
			Encoding utf = Encoding.UTF8;
			StringBuilder stringBuilder = new StringBuilder(value.Length + 11);
			stringBuilder.Append(utf.WebName);
			stringBuilder.Append('\'');
			stringBuilder.Append('\'');
			foreach (char c in value)
			{
				if (c > '\u007f')
				{
					foreach (byte b in utf.GetBytes(new char[]
					{
						c
					}))
					{
						stringBuilder.Append('%');
						stringBuilder.Append(b.ToString("X2"));
					}
				}
				else if (!Lexer.IsValidCharacter(c) || c == '*' || c == '?' || c == '%')
				{
					stringBuilder.Append(Uri.HexEscape(c));
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private static string DecodeValue(string value, bool extendedNotation)
		{
			if (value.Length < 2)
			{
				return value;
			}
			string[] array;
			Encoding encoding;
			if (value[0] == '"')
			{
				array = value.Split('?', StringSplitOptions.None);
				if (array.Length != 5 || array[0] != "\"=" || array[4] != "=\"" || (array[2] != "B" && array[2] != "b"))
				{
					return value;
				}
				try
				{
					encoding = Encoding.GetEncoding(array[1]);
					return encoding.GetString(Convert.FromBase64String(array[3]));
				}
				catch
				{
					return value;
				}
			}
			if (!extendedNotation)
			{
				return value;
			}
			array = value.Split('\'', StringSplitOptions.None);
			if (array.Length != 3)
			{
				return null;
			}
			try
			{
				encoding = Encoding.GetEncoding(array[0]);
			}
			catch
			{
				return null;
			}
			value = array[2];
			if (value.IndexOf('%') < 0)
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = null;
			int num = 0;
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				if (c == '%')
				{
					char c2 = c;
					c = Uri.HexUnescape(value, ref i);
					if (c != c2)
					{
						if (array2 == null)
						{
							array2 = new byte[value.Length - i + 1];
						}
						array2[num++] = (byte)c;
						continue;
					}
				}
				else
				{
					i++;
				}
				if (num != 0)
				{
					stringBuilder.Append(encoding.GetChars(array2, 0, num));
					num = 0;
				}
				stringBuilder.Append(c);
			}
			if (num != 0)
			{
				stringBuilder.Append(encoding.GetChars(array2, 0, num));
			}
			return stringBuilder.ToString();
		}

		/// <summary>Serves as a hash function for an  <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.dispositionType.ToLowerInvariant().GetHashCode() ^ HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents content disposition header value information.</param>
		/// <returns>An <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid content disposition header value information.</exception>
		public static ContentDispositionHeaderValue Parse(string input)
		{
			ContentDispositionHeaderValue result;
			if (ContentDispositionHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		private void SetDateValue(string key, DateTimeOffset? value)
		{
			this.SetValue(key, (value == null) ? null : ("\"" + value.Value.ToString("r", CultureInfo.InvariantCulture) + "\""));
		}

		private void SetValue(string key, string value)
		{
			if (this.parameters == null)
			{
				this.parameters = new List<NameValueHeaderValue>();
			}
			this.parameters.SetValue(key, value);
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return this.dispositionType + this.parameters.ToString<NameValueHeaderValue>();
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ContentDispositionHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out ContentDispositionHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			Token token = lexer.Scan(false);
			if (token.Kind != Token.Type.Token)
			{
				return false;
			}
			List<NameValueHeaderValue> list = null;
			string stringValue = lexer.GetStringValue(token);
			token = lexer.Scan(false);
			Token.Type kind = token.Kind;
			if (kind != Token.Type.End)
			{
				if (kind != Token.Type.SeparatorSemicolon)
				{
					return false;
				}
				if (!NameValueHeaderValue.TryParseParameters(lexer, out list, out token) || token != Token.Type.End)
				{
					return false;
				}
			}
			parsedValue = new ContentDispositionHeaderValue
			{
				dispositionType = stringValue,
				parameters = list
			};
			return true;
		}

		private string dispositionType;

		private List<NameValueHeaderValue> parameters;
	}
}
