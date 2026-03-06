using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents a media type with an additional quality factor used in a Content-Type header.</summary>
	public sealed class MediaTypeWithQualityHeaderValue : MediaTypeHeaderValue
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> class.</summary>
		/// <param name="mediaType">A <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> represented as string to initialize the new instance.</param>
		public MediaTypeWithQualityHeaderValue(string mediaType) : base(mediaType)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> class.</summary>
		/// <param name="mediaType">A <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> represented as string to initialize the new instance.</param>
		/// <param name="quality">The quality associated with this header value.</param>
		public MediaTypeWithQualityHeaderValue(string mediaType, double quality) : this(mediaType)
		{
			this.Quality = new double?(quality);
		}

		private MediaTypeWithQualityHeaderValue()
		{
		}

		/// <summary>Gets or sets the quality value for the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" />.</summary>
		/// <returns>The quality value for the <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> object.</returns>
		public double? Quality
		{
			get
			{
				return QualityValue.GetValue(this.parameters);
			}
			set
			{
				QualityValue.SetValue(ref this.parameters, value);
			}
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents media type with quality header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid media type with quality header value information.</exception>
		public new static MediaTypeWithQualityHeaderValue Parse(string input)
		{
			MediaTypeWithQualityHeaderValue result;
			if (MediaTypeWithQualityHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException();
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.MediaTypeWithQualityHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out MediaTypeWithQualityHeaderValue parsedValue)
		{
			Token token;
			if (MediaTypeWithQualityHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		private static bool TryParseElement(Lexer lexer, out MediaTypeWithQualityHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			List<NameValueHeaderValue> parameters = null;
			string media_type;
			Token? token = MediaTypeHeaderValue.TryParseMediaType(lexer, out media_type);
			if (token == null)
			{
				t = Token.Empty;
				return false;
			}
			t = token.Value;
			if (t == Token.Type.SeparatorSemicolon && !NameValueHeaderValue.TryParseParameters(lexer, out parameters, out t))
			{
				return false;
			}
			parsedValue = new MediaTypeWithQualityHeaderValue();
			parsedValue.media_type = media_type;
			parsedValue.parameters = parameters;
			return true;
		}

		internal static bool TryParse(string input, int minimalCount, out List<MediaTypeWithQualityHeaderValue> result)
		{
			return CollectionParser.TryParse<MediaTypeWithQualityHeaderValue>(input, minimalCount, new ElementTryParser<MediaTypeWithQualityHeaderValue>(MediaTypeWithQualityHeaderValue.TryParseElement), out result);
		}
	}
}
