using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the value of a Via header.</summary>
	public class ViaHeaderValue : ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
		/// <param name="protocolVersion">The protocol version of the received protocol.</param>
		/// <param name="receivedBy">The host and port that the request or response was received by.</param>
		public ViaHeaderValue(string protocolVersion, string receivedBy)
		{
			Parser.Token.Check(protocolVersion);
			Parser.Uri.Check(receivedBy);
			this.ProtocolVersion = protocolVersion;
			this.ReceivedBy = receivedBy;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
		/// <param name="protocolVersion">The protocol version of the received protocol.</param>
		/// <param name="receivedBy">The host and port that the request or response was received by.</param>
		/// <param name="protocolName">The protocol name of the received protocol.</param>
		public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName) : this(protocolVersion, receivedBy)
		{
			if (!string.IsNullOrEmpty(protocolName))
			{
				Parser.Token.Check(protocolName);
				this.ProtocolName = protocolName;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> class.</summary>
		/// <param name="protocolVersion">The protocol version of the received protocol.</param>
		/// <param name="receivedBy">The host and port that the request or response was received by.</param>
		/// <param name="protocolName">The protocol name of the received protocol.</param>
		/// <param name="comment">The comment field used to identify the software of the recipient proxy or gateway.</param>
		public ViaHeaderValue(string protocolVersion, string receivedBy, string protocolName, string comment) : this(protocolVersion, receivedBy, protocolName)
		{
			if (!string.IsNullOrEmpty(comment))
			{
				Parser.Token.CheckComment(comment);
				this.Comment = comment;
			}
		}

		private ViaHeaderValue()
		{
		}

		/// <summary>Gets the comment field used to identify the software of the recipient proxy or gateway.</summary>
		/// <returns>The comment field used to identify the software of the recipient proxy or gateway.</returns>
		public string Comment { get; private set; }

		/// <summary>Gets the protocol name of the received protocol.</summary>
		/// <returns>The protocol name.</returns>
		public string ProtocolName { get; private set; }

		/// <summary>Gets the protocol version of the received protocol.</summary>
		/// <returns>The protocol version.</returns>
		public string ProtocolVersion { get; private set; }

		/// <summary>Gets the host and port that the request or response was received by.</summary>
		/// <returns>The host and port that the request or response was received by.</returns>
		public string ReceivedBy { get; private set; }

		/// <summary>Creates a new object that is a copy of the current <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> instance.</summary>
		/// <returns>A copy of the current instance.</returns>
		object ICloneable.Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			ViaHeaderValue viaHeaderValue = obj as ViaHeaderValue;
			return viaHeaderValue != null && (string.Equals(viaHeaderValue.Comment, this.Comment, StringComparison.Ordinal) && string.Equals(viaHeaderValue.ProtocolName, this.ProtocolName, StringComparison.OrdinalIgnoreCase) && string.Equals(viaHeaderValue.ProtocolVersion, this.ProtocolVersion, StringComparison.OrdinalIgnoreCase)) && string.Equals(viaHeaderValue.ReceivedBy, this.ReceivedBy, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Serves as a hash function for an <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			int num = this.ProtocolVersion.ToLowerInvariant().GetHashCode();
			num ^= this.ReceivedBy.ToLowerInvariant().GetHashCode();
			if (!string.IsNullOrEmpty(this.ProtocolName))
			{
				num ^= this.ProtocolName.ToLowerInvariant().GetHashCode();
			}
			if (!string.IsNullOrEmpty(this.Comment))
			{
				num ^= this.Comment.GetHashCode();
			}
			return num;
		}

		/// <summary>Converts a string to an <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> instance.</summary>
		/// <param name="input">A string that represents via header value information.</param>
		/// <returns>A <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> instance.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="input" /> is a <see langword="null" /> reference.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="input" /> is not valid via header value information.</exception>
		public static ViaHeaderValue Parse(string input)
		{
			ViaHeaderValue result;
			if (ViaHeaderValue.TryParse(input, out result))
			{
				return result;
			}
			throw new FormatException(input);
		}

		/// <summary>Determines whether a string is valid <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> information.</summary>
		/// <param name="input">The string to validate.</param>
		/// <param name="parsedValue">The <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> version of the string.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="input" /> is valid <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> information; otherwise, <see langword="false" />.</returns>
		public static bool TryParse(string input, out ViaHeaderValue parsedValue)
		{
			Token token;
			if (ViaHeaderValue.TryParseElement(new Lexer(input), out parsedValue, out token) && token == Token.Type.End)
			{
				return true;
			}
			parsedValue = null;
			return false;
		}

		internal static bool TryParse(string input, int minimalCount, out List<ViaHeaderValue> result)
		{
			return CollectionParser.TryParse<ViaHeaderValue>(input, minimalCount, new ElementTryParser<ViaHeaderValue>(ViaHeaderValue.TryParseElement), out result);
		}

		private static bool TryParseElement(Lexer lexer, out ViaHeaderValue parsedValue, out Token t)
		{
			parsedValue = null;
			t = lexer.Scan(false);
			if (t != Token.Type.Token)
			{
				return false;
			}
			Token token = lexer.Scan(false);
			ViaHeaderValue viaHeaderValue = new ViaHeaderValue();
			if (token == Token.Type.SeparatorSlash)
			{
				token = lexer.Scan(false);
				if (token != Token.Type.Token)
				{
					return false;
				}
				viaHeaderValue.ProtocolName = lexer.GetStringValue(t);
				viaHeaderValue.ProtocolVersion = lexer.GetStringValue(token);
				token = lexer.Scan(false);
			}
			else
			{
				viaHeaderValue.ProtocolVersion = lexer.GetStringValue(t);
			}
			if (token != Token.Type.Token)
			{
				return false;
			}
			if (lexer.PeekChar() == 58)
			{
				lexer.EatChar();
				t = lexer.Scan(false);
				if (t != Token.Type.Token)
				{
					return false;
				}
			}
			else
			{
				t = token;
			}
			viaHeaderValue.ReceivedBy = lexer.GetStringValue(token, t);
			string comment;
			if (lexer.ScanCommentOptional(out comment, out t))
			{
				t = lexer.Scan(false);
			}
			viaHeaderValue.Comment = comment;
			parsedValue = viaHeaderValue;
			return true;
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.ViaHeaderValue" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			string text = (this.ProtocolName != null) ? string.Concat(new string[]
			{
				this.ProtocolName,
				"/",
				this.ProtocolVersion,
				" ",
				this.ReceivedBy
			}) : (this.ProtocolVersion + " " + this.ReceivedBy);
			if (this.Comment == null)
			{
				return text;
			}
			return text + " " + this.Comment;
		}
	}
}
