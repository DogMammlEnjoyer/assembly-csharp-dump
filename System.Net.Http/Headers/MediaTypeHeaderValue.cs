using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a media type used in a Content-Type header as defined in the RFC 2616.</summary>
	public class MediaTypeHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> class.</summary>
		/// <param name="mediaType">The source represented as a string to initialize the new instance.</param>
		public MediaTypeHeaderValue(string mediaType)
		{
			this.MediaType = mediaType;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> class.</summary>
		/// <param name="source">A <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object used to initialize the new instance.</param>
		protected MediaTypeHeaderValue(MediaTypeHeaderValue source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			this.media_type = source.media_type;
			if (source.parameters != null)
			{
				foreach (NameValueHeaderValue source2 in source.parameters)
				{
					this.Parameters.Add(new NameValueHeaderValue(source2));
				}
			}
		}

		internal MediaTypeHeaderValue()
		{
		}

		/// <summary>Gets or sets the character set.</summary>
		/// <returns>The character set.</returns>
		public string CharSet
		{
			get
			{
				if (this.parameters == null)
				{
					return null;
				}
				NameValueHeaderValue nameValueHeaderValue = this.parameters.Find((NameValueHeaderValue l) => string.Equals(l.Name, "charset", StringComparison.OrdinalIgnoreCase));
				if (nameValueHeaderValue == null)
				{
					return null;
				}
				return nameValueHeaderValue.Value;
			}
			set
			{
				if (this.parameters == null)
				{
					this.parameters = new List<NameValueHeaderValue>();
				}
				this.parameters.SetValue("charset", value);
			}
		}

		/// <summary>Gets or sets the media-type header value.</summary>
		/// <returns>The media-type header value.</returns>
		public string MediaType
		{
			get
			{
				return this.media_type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("MediaType");
				}
				string text;
				Token? token = MediaTypeHeaderValue.TryParseMediaType(new Lexer(value), out text);
				if (token == null || token.Value.Kind != Token.Type.End)
				{
					throw new FormatException();
				}
				this.media_type = text;
			}
		}

		/// <summary>Gets or sets the media-type header value parameters.</summary>
		/// <returns>The media-type header value parameters.</returns>
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

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return new MediaTypeHeaderValue(this);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			MediaTypeHeaderValue mediaTypeHeaderValue = obj as MediaTypeHeaderValue;
			return mediaTypeHeaderValue != null && string.Equals(mediaTypeHeaderValue.media_type, this.media_type, StringComparison.OrdinalIgnoreCase) && mediaTypeHeaderValue.parameters.SequenceEqual(this.parameters);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			return this.media_type.ToLowerInvariant().GetHashCode() ^ HashCodeCalculator.Calculate<NameValueHeaderValue>(this.parameters);
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents media type header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid media type header value information.</exception>
		public static MediaTypeHeaderValue Parse(string input)
		{
			MediaTypeHeaderValue result;
			if (MediaTypeHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (this.parameters == null)
			{
				return this.media_type;
			}
			return this.media_type + this.parameters.ToString<NameValueHeaderValue>();
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out MediaTypeHeaderValue parsedValue)
		{
			parsedValue = null;
			Lexer lexer = new Lexer(input);
			List<NameValueHeaderValue> list = null;
			string text;
			Token? token = MediaTypeHeaderValue.TryParseMediaType(lexer, out text);
			if (token == null)
			{
				return false;
			}
			Token.Type kind = token.Value.Kind;
			if (kind != Token.Type.End)
			{
				if (kind != Token.Type.SeparatorSemicolon)
				{
					return false;
				}
				Token token2;
				if (!NameValueHeaderValue.TryParseParameters(lexer, out list, out token2) || token2 != Token.Type.End)
				{
					return false;
				}
			}
			parsedValue = new MediaTypeHeaderValue
			{
				media_type = text,
				parameters = list
			};
			return true;
		}

		internal static Token? TryParseMediaType(Lexer lexer, out string media)
		{
			media = null;
			Token token = lexer.Scan(false);
			if (token != Token.Type.Token)
			{
				return null;
			}
			if (lexer.Scan(false) != Token.Type.SeparatorSlash)
			{
				return null;
			}
			Token token2 = lexer.Scan(false);
			if (token2 != Token.Type.Token)
			{
				return null;
			}
			media = lexer.GetStringValue(token) + "/" + lexer.GetStringValue(token2);
			return new Token?(lexer.Scan(false));
		}

		internal List<NameValueHeaderValue> parameters;

		internal string media_type;
	}
}
