using System;
using System.Globalization;
using System.IO;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json
{
	public class JsonTextReader : JsonReader, IJsonLineInfo
	{
		public JsonTextReader(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this._reader = reader;
			this._lineNumber = 1;
		}

		public IArrayPool<char> ArrayPool
		{
			get
			{
				return this._arrayPool;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._arrayPool = value;
			}
		}

		private void EnsureBufferNotEmpty()
		{
			if (this._stringBuffer.IsEmpty)
			{
				this._stringBuffer = new StringBuffer(this._arrayPool, 1024);
			}
		}

		private void OnNewLine(int pos)
		{
			this._lineNumber++;
			this._lineStartPos = pos;
		}

		private void ParseString(char quote, ReadType readType)
		{
			this._charPos++;
			this.ShiftBufferIfNeeded();
			this.ReadStringIntoBuffer(quote);
			base.SetPostValueState(true);
			switch (readType)
			{
			case ReadType.ReadAsInt32:
			case ReadType.ReadAsDecimal:
			case ReadType.ReadAsBoolean:
				return;
			case ReadType.ReadAsBytes:
			{
				byte[] value;
				Guid guid;
				if (this._stringReference.Length == 0)
				{
					value = new byte[0];
				}
				else if (this._stringReference.Length == 36 && ConvertUtils.TryConvertGuid(this._stringReference.ToString(), out guid))
				{
					value = guid.ToByteArray();
				}
				else
				{
					value = Convert.FromBase64CharArray(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				}
				base.SetToken(JsonToken.Bytes, value, false);
				return;
			}
			case ReadType.ReadAsString:
			{
				string value2 = this._stringReference.ToString();
				base.SetToken(JsonToken.String, value2, false);
				this._quoteChar = quote;
				return;
			}
			}
			if (this._dateParseHandling != DateParseHandling.None)
			{
				DateParseHandling dateParseHandling;
				if (readType == ReadType.ReadAsDateTime)
				{
					dateParseHandling = DateParseHandling.DateTime;
				}
				else if (readType == ReadType.ReadAsDateTimeOffset)
				{
					dateParseHandling = DateParseHandling.DateTimeOffset;
				}
				else
				{
					dateParseHandling = this._dateParseHandling;
				}
				DateTimeOffset dateTimeOffset;
				if (dateParseHandling == DateParseHandling.DateTime)
				{
					DateTime dateTime;
					if (DateTimeUtils.TryParseDateTime(this._stringReference, base.DateTimeZoneHandling, base.DateFormatString, base.Culture, out dateTime))
					{
						base.SetToken(JsonToken.Date, dateTime, false);
						return;
					}
				}
				else if (DateTimeUtils.TryParseDateTimeOffset(this._stringReference, base.DateFormatString, base.Culture, out dateTimeOffset))
				{
					base.SetToken(JsonToken.Date, dateTimeOffset, false);
					return;
				}
			}
			base.SetToken(JsonToken.String, this._stringReference.ToString(), false);
			this._quoteChar = quote;
		}

		private static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		private void ShiftBufferIfNeeded()
		{
			int num = this._chars.Length;
			if ((double)(num - this._charPos) <= (double)num * 0.1)
			{
				int num2 = this._charsUsed - this._charPos;
				if (num2 > 0)
				{
					JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num2);
				}
				this._lineStartPos -= this._charPos;
				this._charPos = 0;
				this._charsUsed = num2;
				this._chars[this._charsUsed] = '\0';
			}
		}

		private int ReadData(bool append)
		{
			return this.ReadData(append, 0);
		}

		private int ReadData(bool append, int charsRequired)
		{
			if (this._isEndOfFile)
			{
				return 0;
			}
			if (this._charsUsed + charsRequired >= this._chars.Length - 1)
			{
				if (append)
				{
					int minSize = Math.Max(this._chars.Length * 2, this._charsUsed + charsRequired + 1);
					char[] array = BufferUtils.RentBuffer(this._arrayPool, minSize);
					JsonTextReader.BlockCopyChars(this._chars, 0, array, 0, this._chars.Length);
					BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
					this._chars = array;
				}
				else
				{
					int num = this._charsUsed - this._charPos;
					if (num + charsRequired + 1 >= this._chars.Length)
					{
						char[] array2 = BufferUtils.RentBuffer(this._arrayPool, num + charsRequired + 1);
						if (num > 0)
						{
							JsonTextReader.BlockCopyChars(this._chars, this._charPos, array2, 0, num);
						}
						BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
						this._chars = array2;
					}
					else if (num > 0)
					{
						JsonTextReader.BlockCopyChars(this._chars, this._charPos, this._chars, 0, num);
					}
					this._lineStartPos -= this._charPos;
					this._charPos = 0;
					this._charsUsed = num;
				}
			}
			int count = this._chars.Length - this._charsUsed - 1;
			int num2 = this._reader.Read(this._chars, this._charsUsed, count);
			this._charsUsed += num2;
			if (num2 == 0)
			{
				this._isEndOfFile = true;
			}
			this._chars[this._charsUsed] = '\0';
			return num2;
		}

		private bool EnsureChars(int relativePosition, bool append)
		{
			return this._charPos + relativePosition < this._charsUsed || this.ReadChars(relativePosition, append);
		}

		private bool ReadChars(int relativePosition, bool append)
		{
			if (this._isEndOfFile)
			{
				return false;
			}
			int num = this._charPos + relativePosition - this._charsUsed + 1;
			int num2 = 0;
			do
			{
				int num3 = this.ReadData(append, num - num2);
				if (num3 == 0)
				{
					break;
				}
				num2 += num3;
			}
			while (num2 < num);
			return num2 >= num;
		}

		public override bool Read()
		{
			this.EnsureBuffer();
			for (;;)
			{
				switch (this._currentState)
				{
				case JsonReader.State.Start:
				case JsonReader.State.Property:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.Array:
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
					goto IL_4C;
				case JsonReader.State.ObjectStart:
				case JsonReader.State.Object:
					goto IL_53;
				case JsonReader.State.PostValue:
					if (this.ParsePostValue())
					{
						return true;
					}
					continue;
				case JsonReader.State.Finished:
					goto IL_64;
				}
				break;
			}
			goto IL_D2;
			IL_4C:
			return this.ParseValue();
			IL_53:
			return this.ParseObject();
			IL_64:
			if (!this.EnsureChars(0, false))
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			this.EatWhitespace(false);
			if (this._isEndOfFile)
			{
				base.SetToken(JsonToken.None);
				return false;
			}
			if (this._chars[this._charPos] == '/')
			{
				this.ParseComment(true);
				return true;
			}
			throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			IL_D2:
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		public override int? ReadAsInt32()
		{
			return (int?)this.ReadNumberValue(ReadType.ReadAsInt32);
		}

		public override DateTime? ReadAsDateTime()
		{
			return (DateTime?)this.ReadStringValue(ReadType.ReadAsDateTime);
		}

		public override string ReadAsString()
		{
			return (string)this.ReadStringValue(ReadType.ReadAsString);
		}

		public override byte[] ReadAsBytes()
		{
			this.EnsureBuffer();
			bool flag = false;
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.PostValue:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
			{
				char c;
				for (;;)
				{
					c = this._chars[this._charPos];
					if (c <= '\'')
					{
						if (c <= '\r')
						{
							if (c != '\0')
							{
								switch (c)
								{
								case '\t':
									break;
								case '\n':
									this.ProcessLineFeed();
									continue;
								case '\v':
								case '\f':
									goto IL_20A;
								case '\r':
									this.ProcessCarriageReturn(false);
									continue;
								default:
									goto IL_20A;
								}
							}
							else
							{
								if (this.ReadNullChar())
								{
									break;
								}
								continue;
							}
						}
						else if (c != ' ')
						{
							if (c != '"' && c != '\'')
							{
								goto IL_20A;
							}
							goto IL_F4;
						}
						this._charPos++;
						continue;
					}
					if (c <= '[')
					{
						if (c == ',')
						{
							this.ProcessValueComma();
							continue;
						}
						if (c == '/')
						{
							this.ParseComment(false);
							continue;
						}
						if (c == '[')
						{
							goto IL_16A;
						}
					}
					else
					{
						if (c == ']')
						{
							goto IL_1A5;
						}
						if (c == 'n')
						{
							goto IL_186;
						}
						if (c == '{')
						{
							this._charPos++;
							base.SetToken(JsonToken.StartObject);
							base.ReadIntoWrappedTypeObject();
							flag = true;
							continue;
						}
					}
					IL_20A:
					this._charPos++;
					if (!char.IsWhiteSpace(c))
					{
						goto Block_21;
					}
				}
				base.SetToken(JsonToken.None, null, false);
				return null;
				IL_F4:
				this.ParseString(c, ReadType.ReadAsBytes);
				byte[] array = (byte[])this.Value;
				if (flag)
				{
					base.ReaderReadAndAssert();
					if (this.TokenType != JsonToken.EndObject)
					{
						throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
					}
					base.SetToken(JsonToken.Bytes, array, false);
				}
				return array;
				IL_16A:
				this._charPos++;
				base.SetToken(JsonToken.StartArray);
				return base.ReadArrayIntoByteArray();
				IL_186:
				this.HandleNull();
				return null;
				IL_1A5:
				this._charPos++;
				if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
				{
					base.SetToken(JsonToken.EndArray);
					return null;
				}
				throw this.CreateUnexpectedCharacterException(c);
				Block_21:
				throw this.CreateUnexpectedCharacterException(c);
			}
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			}
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		private object ReadStringValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.PostValue:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
			{
				char c;
				for (;;)
				{
					c = this._chars[this._charPos];
					if (c <= 'I')
					{
						if (c <= '\r')
						{
							if (c != '\0')
							{
								switch (c)
								{
								case '\t':
									break;
								case '\n':
									this.ProcessLineFeed();
									continue;
								case '\v':
								case '\f':
									goto IL_346;
								case '\r':
									this.ProcessCarriageReturn(false);
									continue;
								default:
									goto IL_346;
								}
							}
							else
							{
								if (this.ReadNullChar())
								{
									break;
								}
								continue;
							}
						}
						else
						{
							switch (c)
							{
							case ' ':
								break;
							case '!':
							case '#':
							case '$':
							case '%':
							case '&':
							case '(':
							case ')':
							case '*':
							case '+':
								goto IL_346;
							case '"':
							case '\'':
								goto IL_15A;
							case ',':
								this.ProcessValueComma();
								continue;
							case '-':
								goto IL_203;
							case '.':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								goto IL_236;
							case '/':
								this.ParseComment(false);
								continue;
							default:
								if (c != 'I')
								{
									goto IL_346;
								}
								goto IL_2B2;
							}
						}
						this._charPos++;
						continue;
					}
					if (c <= ']')
					{
						if (c == 'N')
						{
							goto IL_2BA;
						}
						if (c == ']')
						{
							goto IL_2E1;
						}
					}
					else
					{
						if (c == 'f')
						{
							goto IL_25E;
						}
						if (c == 'n')
						{
							goto IL_2C2;
						}
						if (c == 't')
						{
							goto IL_25E;
						}
					}
					IL_346:
					this._charPos++;
					if (!char.IsWhiteSpace(c))
					{
						goto Block_26;
					}
				}
				base.SetToken(JsonToken.None, null, false);
				return null;
				IL_15A:
				this.ParseString(c, readType);
				switch (readType)
				{
				case ReadType.ReadAsBytes:
					return this.Value;
				case ReadType.ReadAsString:
					return this.Value;
				case ReadType.ReadAsDateTime:
					if (this.Value is DateTime)
					{
						return (DateTime)this.Value;
					}
					return base.ReadDateTimeString((string)this.Value);
				case ReadType.ReadAsDateTimeOffset:
					if (this.Value is DateTimeOffset)
					{
						return (DateTimeOffset)this.Value;
					}
					return base.ReadDateTimeOffsetString((string)this.Value);
				}
				throw new ArgumentOutOfRangeException("readType");
				IL_203:
				if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
				{
					return this.ParseNumberNegativeInfinity(readType);
				}
				this.ParseNumber(readType);
				return this.Value;
				IL_236:
				if (readType != ReadType.ReadAsString)
				{
					this._charPos++;
					throw this.CreateUnexpectedCharacterException(c);
				}
				this.ParseNumber(ReadType.ReadAsString);
				return this.Value;
				IL_25E:
				if (readType != ReadType.ReadAsString)
				{
					this._charPos++;
					throw this.CreateUnexpectedCharacterException(c);
				}
				string text = (c == 't') ? JsonConvert.True : JsonConvert.False;
				if (!this.MatchValueWithTrailingSeparator(text))
				{
					throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
				}
				base.SetToken(JsonToken.String, text);
				return text;
				IL_2B2:
				return this.ParseNumberPositiveInfinity(readType);
				IL_2BA:
				return this.ParseNumberNaN(readType);
				IL_2C2:
				this.HandleNull();
				return null;
				IL_2E1:
				this._charPos++;
				if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
				{
					base.SetToken(JsonToken.EndArray);
					return null;
				}
				throw this.CreateUnexpectedCharacterException(c);
				Block_26:
				throw this.CreateUnexpectedCharacterException(c);
			}
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			}
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		private JsonReaderException CreateUnexpectedCharacterException(char c)
		{
			return JsonReaderException.Create(this, "Unexpected character encountered while parsing value: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		public override bool? ReadAsBoolean()
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.PostValue:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
			{
				char c;
				for (;;)
				{
					c = this._chars[this._charPos];
					if (c <= '9')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_274;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								switch (c)
								{
								case ' ':
									break;
								case '!':
								case '#':
								case '$':
								case '%':
								case '&':
								case '(':
								case ')':
								case '*':
								case '+':
									goto IL_274;
								case '"':
								case '\'':
									goto IL_146;
								case ',':
									this.ProcessValueComma();
									continue;
								case '-':
								case '.':
								case '0':
								case '1':
								case '2':
								case '3':
								case '4':
								case '5':
								case '6':
								case '7':
								case '8':
								case '9':
									goto IL_177;
								case '/':
									this.ParseComment(false);
									continue;
								default:
									goto IL_274;
								}
								break;
							}
							this._charPos++;
							continue;
						}
						if (this.ReadNullChar())
						{
							break;
						}
						continue;
					}
					else if (c <= 'f')
					{
						if (c == ']')
						{
							goto IL_206;
						}
						if (c == 'f')
						{
							goto IL_1A5;
						}
					}
					else
					{
						if (c == 'n')
						{
							goto IL_166;
						}
						if (c == 't')
						{
							goto IL_1A5;
						}
					}
					IL_274:
					this._charPos++;
					if (!char.IsWhiteSpace(c))
					{
						goto Block_16;
					}
				}
				base.SetToken(JsonToken.None, null, false);
				return null;
				IL_146:
				this.ParseString(c, ReadType.Read);
				return base.ReadBooleanString(this._stringReference.ToString());
				IL_166:
				this.HandleNull();
				return null;
				IL_177:
				this.ParseNumber(ReadType.Read);
				bool flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
				base.SetToken(JsonToken.Boolean, flag, false);
				return new bool?(flag);
				IL_1A5:
				bool flag2 = c == 't';
				string value = flag2 ? JsonConvert.True : JsonConvert.False;
				if (!this.MatchValueWithTrailingSeparator(value))
				{
					throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
				}
				base.SetToken(JsonToken.Boolean, flag2);
				return new bool?(flag2);
				IL_206:
				this._charPos++;
				if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
				{
					base.SetToken(JsonToken.EndArray);
					return null;
				}
				throw this.CreateUnexpectedCharacterException(c);
				Block_16:
				throw this.CreateUnexpectedCharacterException(c);
			}
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			}
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		private void ProcessValueComma()
		{
			this._charPos++;
			if (this._currentState != JsonReader.State.PostValue)
			{
				base.SetToken(JsonToken.Undefined);
				throw this.CreateUnexpectedCharacterException(',');
			}
			base.SetStateBasedOnCurrent();
		}

		private object ReadNumberValue(ReadType readType)
		{
			this.EnsureBuffer();
			switch (this._currentState)
			{
			case JsonReader.State.Start:
			case JsonReader.State.Property:
			case JsonReader.State.ArrayStart:
			case JsonReader.State.Array:
			case JsonReader.State.PostValue:
			case JsonReader.State.ConstructorStart:
			case JsonReader.State.Constructor:
			{
				char c;
				for (;;)
				{
					c = this._chars[this._charPos];
					if (c <= '9')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_28D;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								switch (c)
								{
								case ' ':
									break;
								case '!':
								case '#':
								case '$':
								case '%':
								case '&':
								case '(':
								case ')':
								case '*':
								case '+':
									goto IL_28D;
								case '"':
								case '\'':
									goto IL_140;
								case ',':
									this.ProcessValueComma();
									continue;
								case '-':
									goto IL_1D0;
								case '.':
								case '0':
								case '1':
								case '2':
								case '3':
								case '4':
								case '5':
								case '6':
								case '7':
								case '8':
								case '9':
									goto IL_203;
								case '/':
									this.ParseComment(false);
									continue;
								default:
									goto IL_28D;
								}
								break;
							}
							this._charPos++;
							continue;
						}
						if (this.ReadNullChar())
						{
							break;
						}
						continue;
					}
					else if (c <= 'N')
					{
						if (c == 'I')
						{
							goto IL_1C8;
						}
						if (c == 'N')
						{
							goto IL_1C0;
						}
					}
					else
					{
						if (c == ']')
						{
							goto IL_228;
						}
						if (c == 'n')
						{
							goto IL_1B8;
						}
					}
					IL_28D:
					this._charPos++;
					if (!char.IsWhiteSpace(c))
					{
						goto Block_19;
					}
				}
				base.SetToken(JsonToken.None, null, false);
				return null;
				IL_140:
				this.ParseString(c, readType);
				if (readType == ReadType.ReadAsInt32)
				{
					return base.ReadInt32String(this._stringReference.ToString());
				}
				if (readType == ReadType.ReadAsDecimal)
				{
					return base.ReadDecimalString(this._stringReference.ToString());
				}
				if (readType != ReadType.ReadAsDouble)
				{
					throw new ArgumentOutOfRangeException("readType");
				}
				return base.ReadDoubleString(this._stringReference.ToString());
				IL_1B8:
				this.HandleNull();
				return null;
				IL_1C0:
				return this.ParseNumberNaN(readType);
				IL_1C8:
				return this.ParseNumberPositiveInfinity(readType);
				IL_1D0:
				if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
				{
					return this.ParseNumberNegativeInfinity(readType);
				}
				this.ParseNumber(readType);
				return this.Value;
				IL_203:
				this.ParseNumber(readType);
				return this.Value;
				IL_228:
				this._charPos++;
				if (this._currentState == JsonReader.State.Array || this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.PostValue)
				{
					base.SetToken(JsonToken.EndArray);
					return null;
				}
				throw this.CreateUnexpectedCharacterException(c);
				Block_19:
				throw this.CreateUnexpectedCharacterException(c);
			}
			case JsonReader.State.Finished:
				this.ReadFinished();
				return null;
			}
			throw JsonReaderException.Create(this, "Unexpected state: {0}.".FormatWith(CultureInfo.InvariantCulture, base.CurrentState));
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			return (DateTimeOffset?)this.ReadStringValue(ReadType.ReadAsDateTimeOffset);
		}

		public override decimal? ReadAsDecimal()
		{
			return (decimal?)this.ReadNumberValue(ReadType.ReadAsDecimal);
		}

		public override double? ReadAsDouble()
		{
			return (double?)this.ReadNumberValue(ReadType.ReadAsDouble);
		}

		private void HandleNull()
		{
			if (!this.EnsureChars(1, true))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			if (this._chars[this._charPos + 1] == 'u')
			{
				this.ParseNull();
				return;
			}
			this._charPos += 2;
			throw this.CreateUnexpectedCharacterException(this._chars[this._charPos - 1]);
		}

		private void ReadFinished()
		{
			if (this.EnsureChars(0, false))
			{
				this.EatWhitespace(false);
				if (this._isEndOfFile)
				{
					return;
				}
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Additional text encountered after finished reading JSON content: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				this.ParseComment(false);
			}
			base.SetToken(JsonToken.None);
		}

		private bool ReadNullChar()
		{
			if (this._charsUsed == this._charPos)
			{
				if (this.ReadData(false) == 0)
				{
					this._isEndOfFile = true;
					return true;
				}
			}
			else
			{
				this._charPos++;
			}
			return false;
		}

		private void EnsureBuffer()
		{
			if (this._chars == null)
			{
				this._chars = BufferUtils.RentBuffer(this._arrayPool, 1024);
				this._chars[0] = '\0';
			}
		}

		private void ReadStringIntoBuffer(char quote)
		{
			int num = this._charPos;
			int charPos = this._charPos;
			int num2 = this._charPos;
			this._stringBuffer.Position = 0;
			char c2;
			for (;;)
			{
				char c = this._chars[num++];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						if (c != '\n')
						{
							if (c == '\r')
							{
								this._charPos = num - 1;
								this.ProcessCarriageReturn(true);
								num = this._charPos;
							}
						}
						else
						{
							this._charPos = num - 1;
							this.ProcessLineFeed();
							num = this._charPos;
						}
					}
					else if (this._charsUsed == num - 1)
					{
						num--;
						if (this.ReadData(true) == 0)
						{
							break;
						}
					}
				}
				else if (c != '"' && c != '\'')
				{
					if (c == '\\')
					{
						this._charPos = num;
						if (!this.EnsureChars(0, true))
						{
							goto Block_10;
						}
						int writeToPosition = num - 1;
						c2 = this._chars[num];
						num++;
						char c3;
						if (c2 <= '\\')
						{
							if (c2 <= '\'')
							{
								if (c2 != '"' && c2 != '\'')
								{
									goto Block_14;
								}
							}
							else if (c2 != '/')
							{
								if (c2 != '\\')
								{
									goto Block_16;
								}
								c3 = '\\';
								goto IL_287;
							}
							c3 = c2;
						}
						else if (c2 <= 'f')
						{
							if (c2 != 'b')
							{
								if (c2 != 'f')
								{
									goto Block_19;
								}
								c3 = '\f';
							}
							else
							{
								c3 = '\b';
							}
						}
						else
						{
							if (c2 != 'n')
							{
								switch (c2)
								{
								case 'r':
									c3 = '\r';
									goto IL_287;
								case 't':
									c3 = '\t';
									goto IL_287;
								case 'u':
									this._charPos = num;
									c3 = this.ParseUnicode();
									if (StringUtils.IsLowSurrogate(c3))
									{
										c3 = '�';
									}
									else if (StringUtils.IsHighSurrogate(c3))
									{
										bool flag;
										do
										{
											flag = false;
											if (this.EnsureChars(2, true) && this._chars[this._charPos] == '\\' && this._chars[this._charPos + 1] == 'u')
											{
												char writeChar = c3;
												this._charPos += 2;
												c3 = this.ParseUnicode();
												if (!StringUtils.IsLowSurrogate(c3))
												{
													if (StringUtils.IsHighSurrogate(c3))
													{
														writeChar = '�';
														flag = true;
													}
													else
													{
														writeChar = '�';
													}
												}
												this.EnsureBufferNotEmpty();
												this.WriteCharToBuffer(writeChar, num2, writeToPosition);
												num2 = this._charPos;
											}
											else
											{
												c3 = '�';
											}
										}
										while (flag);
									}
									num = this._charPos;
									goto IL_287;
								}
								goto Block_21;
							}
							c3 = '\n';
						}
						IL_287:
						this.EnsureBufferNotEmpty();
						this.WriteCharToBuffer(c3, num2, writeToPosition);
						num2 = num;
					}
				}
				else if (this._chars[num - 1] == quote)
				{
					goto Block_28;
				}
			}
			this._charPos = num;
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_10:
			throw JsonReaderException.Create(this, "Unterminated string. Expected delimiter: {0}.".FormatWith(CultureInfo.InvariantCulture, quote));
			Block_14:
			Block_16:
			Block_19:
			Block_21:
			this._charPos = num;
			throw JsonReaderException.Create(this, "Bad JSON escape sequence: {0}.".FormatWith(CultureInfo.InvariantCulture, "\\" + c2.ToString()));
			Block_28:
			num--;
			if (charPos == num2)
			{
				this._stringReference = new StringReference(this._chars, charPos, num - charPos);
			}
			else
			{
				this.EnsureBufferNotEmpty();
				if (num > num2)
				{
					this._stringBuffer.Append(this._arrayPool, this._chars, num2, num - num2);
				}
				this._stringReference = new StringReference(this._stringBuffer.InternalBuffer, 0, this._stringBuffer.Position);
			}
			num++;
			this._charPos = num;
		}

		private void WriteCharToBuffer(char writeChar, int lastWritePosition, int writeToPosition)
		{
			if (writeToPosition > lastWritePosition)
			{
				this._stringBuffer.Append(this._arrayPool, this._chars, lastWritePosition, writeToPosition - lastWritePosition);
			}
			this._stringBuffer.Append(this._arrayPool, writeChar);
		}

		private char ParseUnicode()
		{
			if (this.EnsureChars(4, true))
			{
				char result = Convert.ToChar(ConvertUtils.HexTextToInt(this._chars, this._charPos, this._charPos + 4));
				this._charPos += 4;
				return result;
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unicode character.");
		}

		private void ReadNumberIntoBuffer()
		{
			int num = this._charPos;
			for (;;)
			{
				char c = this._chars[num];
				if (c <= 'F')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '+':
						case '-':
						case '.':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
						case 'A':
						case 'B':
						case 'C':
						case 'D':
						case 'E':
						case 'F':
							goto IL_E4;
						}
						break;
					}
					this._charPos = num;
					if (this._charsUsed != num)
					{
						return;
					}
					if (this.ReadData(true) == 0)
					{
						return;
					}
					continue;
				}
				else if (c != 'X')
				{
					switch (c)
					{
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						break;
					default:
						if (c != 'x')
						{
							goto Block_6;
						}
						break;
					}
				}
				IL_E4:
				num++;
			}
			Block_6:
			this._charPos = num;
			char c2 = this._chars[this._charPos];
			if (char.IsWhiteSpace(c2) || c2 == ',' || c2 == '}' || c2 == ']' || c2 == ')' || c2 == '/')
			{
				return;
			}
			throw JsonReaderException.Create(this, "Unexpected character encountered while parsing number: {0}.".FormatWith(CultureInfo.InvariantCulture, c2));
		}

		private void ClearRecentString()
		{
			this._stringBuffer.Position = 0;
			this._stringReference = default(StringReference);
		}

		private bool ParsePostValue()
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= ')')
				{
					if (c <= '\r')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_143;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								goto IL_143;
							}
						}
						else
						{
							if (this._charsUsed != this._charPos)
							{
								this._charPos++;
								continue;
							}
							if (this.ReadData(false) == 0)
							{
								break;
							}
							continue;
						}
					}
					else if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_143;
						}
						goto IL_E2;
					}
					this._charPos++;
					continue;
				}
				if (c <= '/')
				{
					if (c == ',')
					{
						goto IL_103;
					}
					if (c == '/')
					{
						goto IL_FA;
					}
				}
				else
				{
					if (c == ']')
					{
						goto IL_CA;
					}
					if (c == '}')
					{
						goto IL_B2;
					}
				}
				IL_143:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_15E;
				}
				this._charPos++;
			}
			this._currentState = JsonReader.State.Finished;
			return false;
			IL_B2:
			this._charPos++;
			base.SetToken(JsonToken.EndObject);
			return true;
			IL_CA:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_E2:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_FA:
			this.ParseComment(true);
			return true;
			IL_103:
			this._charPos++;
			base.SetStateBasedOnCurrent();
			return false;
			IL_15E:
			throw JsonReaderException.Create(this, "After parsing a value an unexpected character was encountered: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool ParseObject()
		{
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.ProcessLineFeed();
							continue;
						case '\v':
						case '\f':
							goto IL_BD;
						case '\r':
							this.ProcessCarriageReturn(false);
							continue;
						default:
							goto IL_BD;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c != ' ')
				{
					if (c == '/')
					{
						goto IL_8A;
					}
					if (c != '}')
					{
						goto IL_BD;
					}
					goto IL_72;
				}
				this._charPos++;
				continue;
				IL_BD:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_D8;
				}
				this._charPos++;
			}
			return false;
			IL_72:
			base.SetToken(JsonToken.EndObject);
			this._charPos++;
			return true;
			IL_8A:
			this.ParseComment(true);
			return true;
			IL_D8:
			return this.ParseProperty();
		}

		private bool ParseProperty()
		{
			char c = this._chars[this._charPos];
			char c2;
			if (c == '"' || c == '\'')
			{
				this._charPos++;
				c2 = c;
				this.ShiftBufferIfNeeded();
				this.ReadStringIntoBuffer(c2);
			}
			else
			{
				if (!this.ValidIdentifierChar(c))
				{
					throw JsonReaderException.Create(this, "Invalid property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				c2 = '\0';
				this.ShiftBufferIfNeeded();
				this.ParseUnquotedProperty();
			}
			string text;
			if (this.NameTable != null)
			{
				text = this.NameTable.Get(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length);
				if (text == null)
				{
					text = this._stringReference.ToString();
				}
			}
			else
			{
				text = this._stringReference.ToString();
			}
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != ':')
			{
				throw JsonReaderException.Create(this, "Invalid character after parsing property name. Expected ':' but got: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			base.SetToken(JsonToken.PropertyName, text);
			this._quoteChar = c2;
			this.ClearRecentString();
			return true;
		}

		private bool ValidIdentifierChar(char value)
		{
			return char.IsLetterOrDigit(value) || value == '_' || value == '$';
		}

		private void ParseUnquotedProperty()
		{
			int charPos = this._charPos;
			char c;
			for (;;)
			{
				if (this._chars[this._charPos] == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_3B;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					c = this._chars[this._charPos];
					if (!this.ValidIdentifierChar(c))
					{
						goto IL_7D;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing unquoted property name.");
			IL_3B:
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
			return;
			IL_7D:
			if (char.IsWhiteSpace(c) || c == ':')
			{
				this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
				return;
			}
			throw JsonReaderException.Create(this, "Invalid JavaScript property identifier character: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
		}

		private bool ParseValue()
		{
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c <= 'N')
				{
					if (c <= ' ')
					{
						if (c != '\0')
						{
							switch (c)
							{
							case '\t':
								break;
							case '\n':
								this.ProcessLineFeed();
								continue;
							case '\v':
							case '\f':
								goto IL_276;
							case '\r':
								this.ProcessCarriageReturn(false);
								continue;
							default:
								if (c != ' ')
								{
									goto IL_276;
								}
								break;
							}
							this._charPos++;
							continue;
						}
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(false) == 0)
						{
							break;
						}
						continue;
					}
					else if (c <= '/')
					{
						if (c == '"')
						{
							goto IL_116;
						}
						switch (c)
						{
						case '\'':
							goto IL_116;
						case ')':
							goto IL_234;
						case ',':
							goto IL_22A;
						case '-':
							goto IL_1A3;
						case '/':
							goto IL_1D3;
						}
					}
					else
					{
						if (c == 'I')
						{
							goto IL_199;
						}
						if (c == 'N')
						{
							goto IL_18F;
						}
					}
				}
				else if (c <= 'f')
				{
					if (c == '[')
					{
						goto IL_1FB;
					}
					if (c == ']')
					{
						goto IL_212;
					}
					if (c == 'f')
					{
						goto IL_128;
					}
				}
				else if (c <= 't')
				{
					if (c == 'n')
					{
						goto IL_130;
					}
					if (c == 't')
					{
						goto IL_120;
					}
				}
				else
				{
					if (c == 'u')
					{
						goto IL_1DC;
					}
					if (c == '{')
					{
						goto IL_1E4;
					}
				}
				IL_276:
				if (!char.IsWhiteSpace(c))
				{
					goto IL_291;
				}
				this._charPos++;
			}
			return false;
			IL_116:
			this.ParseString(c, ReadType.Read);
			return true;
			IL_120:
			this.ParseTrue();
			return true;
			IL_128:
			this.ParseFalse();
			return true;
			IL_130:
			if (this.EnsureChars(1, true))
			{
				char c2 = this._chars[this._charPos + 1];
				if (c2 == 'u')
				{
					this.ParseNull();
				}
				else
				{
					if (c2 != 'e')
					{
						throw this.CreateUnexpectedCharacterException(this._chars[this._charPos]);
					}
					this.ParseConstructor();
				}
				return true;
			}
			this._charPos++;
			throw base.CreateUnexpectedEndException();
			IL_18F:
			this.ParseNumberNaN(ReadType.Read);
			return true;
			IL_199:
			this.ParseNumberPositiveInfinity(ReadType.Read);
			return true;
			IL_1A3:
			if (this.EnsureChars(1, true) && this._chars[this._charPos + 1] == 'I')
			{
				this.ParseNumberNegativeInfinity(ReadType.Read);
			}
			else
			{
				this.ParseNumber(ReadType.Read);
			}
			return true;
			IL_1D3:
			this.ParseComment(true);
			return true;
			IL_1DC:
			this.ParseUndefined();
			return true;
			IL_1E4:
			this._charPos++;
			base.SetToken(JsonToken.StartObject);
			return true;
			IL_1FB:
			this._charPos++;
			base.SetToken(JsonToken.StartArray);
			return true;
			IL_212:
			this._charPos++;
			base.SetToken(JsonToken.EndArray);
			return true;
			IL_22A:
			base.SetToken(JsonToken.Undefined);
			return true;
			IL_234:
			this._charPos++;
			base.SetToken(JsonToken.EndConstructor);
			return true;
			IL_291:
			if (char.IsNumber(c) || c == '-' || c == '.')
			{
				this.ParseNumber(ReadType.Read);
				return true;
			}
			throw this.CreateUnexpectedCharacterException(c);
		}

		private void ProcessLineFeed()
		{
			this._charPos++;
			this.OnNewLine(this._charPos);
		}

		private void ProcessCarriageReturn(bool append)
		{
			this._charPos++;
			if (this.EnsureChars(1, append) && this._chars[this._charPos] == '\n')
			{
				this._charPos++;
			}
			this.OnNewLine(this._charPos);
		}

		private bool EatWhitespace(bool oneOrMore)
		{
			bool flag = false;
			bool flag2 = false;
			while (!flag)
			{
				char c = this._chars[this._charPos];
				if (c != '\0')
				{
					if (c != '\n')
					{
						if (c != '\r')
						{
							if (c == ' ' || char.IsWhiteSpace(c))
							{
								flag2 = true;
								this._charPos++;
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							this.ProcessCarriageReturn(false);
						}
					}
					else
					{
						this.ProcessLineFeed();
					}
				}
				else if (this._charsUsed == this._charPos)
				{
					if (this.ReadData(false) == 0)
					{
						flag = true;
					}
				}
				else
				{
					this._charPos++;
				}
			}
			return !oneOrMore || flag2;
		}

		private void ParseConstructor()
		{
			if (!this.MatchValueWithTrailingSeparator("new"))
			{
				throw JsonReaderException.Create(this, "Unexpected content while parsing JSON.");
			}
			this.EatWhitespace(false);
			int charPos = this._charPos;
			char c;
			for (;;)
			{
				c = this._chars[this._charPos];
				if (c == '\0')
				{
					if (this._charsUsed != this._charPos)
					{
						goto IL_53;
					}
					if (this.ReadData(true) == 0)
					{
						break;
					}
				}
				else
				{
					if (!char.IsLetterOrDigit(c))
					{
						goto IL_85;
					}
					this._charPos++;
				}
			}
			throw JsonReaderException.Create(this, "Unexpected end while parsing constructor.");
			IL_53:
			int charPos2 = this._charPos;
			this._charPos++;
			goto IL_F7;
			IL_85:
			if (c == '\r')
			{
				charPos2 = this._charPos;
				this.ProcessCarriageReturn(true);
			}
			else if (c == '\n')
			{
				charPos2 = this._charPos;
				this.ProcessLineFeed();
			}
			else if (char.IsWhiteSpace(c))
			{
				charPos2 = this._charPos;
				this._charPos++;
			}
			else
			{
				if (c != '(')
				{
					throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, c));
				}
				charPos2 = this._charPos;
			}
			IL_F7:
			this._stringReference = new StringReference(this._chars, charPos, charPos2 - charPos);
			string value = this._stringReference.ToString();
			this.EatWhitespace(false);
			if (this._chars[this._charPos] != '(')
			{
				throw JsonReaderException.Create(this, "Unexpected character while parsing constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
			}
			this._charPos++;
			this.ClearRecentString();
			base.SetToken(JsonToken.StartConstructor, value);
		}

		private void ParseNumber(ReadType readType)
		{
			this.ShiftBufferIfNeeded();
			char c = this._chars[this._charPos];
			int charPos = this._charPos;
			this.ReadNumberIntoBuffer();
			base.SetPostValueState(true);
			this._stringReference = new StringReference(this._chars, charPos, this._charPos - charPos);
			bool flag = char.IsDigit(c) && this._stringReference.Length == 1;
			bool flag2 = c == '0' && this._stringReference.Length > 1 && this._stringReference.Chars[this._stringReference.StartIndex + 1] != '.' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'e' && this._stringReference.Chars[this._stringReference.StartIndex + 1] != 'E';
			JsonToken newToken;
			object value;
			if (readType == ReadType.ReadAsString)
			{
				string text = this._stringReference.ToString();
				if (flag2)
				{
					try
					{
						if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
						{
							Convert.ToInt64(text, 16);
						}
						else
						{
							Convert.ToInt64(text, 8);
						}
						goto IL_16B;
					}
					catch (Exception ex)
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text), ex);
					}
				}
				double num;
				if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
				{
					throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
				}
				IL_16B:
				newToken = JsonToken.String;
				value = text;
			}
			else if (readType == ReadType.ReadAsInt32)
			{
				if (flag)
				{
					value = (int)(c - '0');
				}
				else
				{
					if (flag2)
					{
						string text2 = this._stringReference.ToString();
						try
						{
							value = (text2.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(text2, 16) : Convert.ToInt32(text2, 8));
							goto IL_27C;
						}
						catch (Exception ex2)
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, text2), ex2);
						}
					}
					int num2;
					ParseResult parseResult = ConvertUtils.Int32TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num2);
					if (parseResult == ParseResult.Success)
					{
						value = num2;
					}
					else
					{
						if (parseResult == ParseResult.Overflow)
						{
							throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int32.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
						}
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid integer.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
				}
				IL_27C:
				newToken = JsonToken.Integer;
			}
			else if (readType == ReadType.ReadAsDecimal)
			{
				if (flag)
				{
					value = c - 48m;
				}
				else
				{
					if (flag2)
					{
						string text3 = this._stringReference.ToString();
						try
						{
							value = Convert.ToDecimal(text3.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text3, 16) : Convert.ToInt64(text3, 8));
							goto IL_362;
						}
						catch (Exception ex3)
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text3), ex3);
						}
					}
					decimal num3;
					if (!decimal.TryParse(this._stringReference.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num3))
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					value = num3;
				}
				IL_362:
				newToken = JsonToken.Float;
			}
			else if (readType == ReadType.ReadAsDouble)
			{
				if (flag)
				{
					value = (double)c - 48.0;
				}
				else
				{
					if (flag2)
					{
						string text4 = this._stringReference.ToString();
						try
						{
							value = Convert.ToDouble(text4.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text4, 16) : Convert.ToInt64(text4, 8));
							goto IL_442;
						}
						catch (Exception ex4)
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, text4), ex4);
						}
					}
					double num4;
					if (!double.TryParse(this._stringReference.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out num4))
					{
						throw JsonReaderException.Create(this, "Input string '{0}' is not a valid double.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					value = num4;
				}
				IL_442:
				newToken = JsonToken.Float;
			}
			else if (flag)
			{
				value = (long)((ulong)c - 48UL);
				newToken = JsonToken.Integer;
			}
			else if (flag2)
			{
				string text5 = this._stringReference.ToString();
				try
				{
					value = (text5.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt64(text5, 16) : Convert.ToInt64(text5, 8));
				}
				catch (Exception ex5)
				{
					throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text5), ex5);
				}
				newToken = JsonToken.Integer;
			}
			else
			{
				long num5;
				ParseResult parseResult2 = ConvertUtils.Int64TryParse(this._stringReference.Chars, this._stringReference.StartIndex, this._stringReference.Length, out num5);
				if (parseResult2 == ParseResult.Success)
				{
					value = num5;
					newToken = JsonToken.Integer;
				}
				else
				{
					if (parseResult2 == ParseResult.Overflow)
					{
						throw JsonReaderException.Create(this, "JSON integer {0} is too large or small for an Int64.".FormatWith(CultureInfo.InvariantCulture, this._stringReference.ToString()));
					}
					string text6 = this._stringReference.ToString();
					if (this._floatParseHandling == FloatParseHandling.Decimal)
					{
						decimal num6;
						if (!decimal.TryParse(text6, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num6))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid decimal.".FormatWith(CultureInfo.InvariantCulture, text6));
						}
						value = num6;
					}
					else
					{
						double num7;
						if (!double.TryParse(text6, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num7))
						{
							throw JsonReaderException.Create(this, "Input string '{0}' is not a valid number.".FormatWith(CultureInfo.InvariantCulture, text6));
						}
						value = num7;
					}
					newToken = JsonToken.Float;
				}
			}
			this.ClearRecentString();
			base.SetToken(newToken, value, false);
		}

		private void ParseComment(bool setToken)
		{
			this._charPos++;
			if (!this.EnsureChars(1, false))
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			bool flag;
			if (this._chars[this._charPos] == '*')
			{
				flag = false;
			}
			else
			{
				if (this._chars[this._charPos] != '/')
				{
					throw JsonReaderException.Create(this, "Error parsing comment. Expected: *, got {0}.".FormatWith(CultureInfo.InvariantCulture, this._chars[this._charPos]));
				}
				flag = true;
			}
			this._charPos++;
			int charPos = this._charPos;
			for (;;)
			{
				char c = this._chars[this._charPos];
				if (c <= '\n')
				{
					if (c != '\0')
					{
						if (c == '\n')
						{
							if (flag)
							{
								goto Block_16;
							}
							this.ProcessLineFeed();
							continue;
						}
					}
					else
					{
						if (this._charsUsed != this._charPos)
						{
							this._charPos++;
							continue;
						}
						if (this.ReadData(true) == 0)
						{
							break;
						}
						continue;
					}
				}
				else if (c != '\r')
				{
					if (c == '*')
					{
						this._charPos++;
						if (!flag && this.EnsureChars(0, true) && this._chars[this._charPos] == '/')
						{
							goto Block_14;
						}
						continue;
					}
				}
				else
				{
					if (flag)
					{
						goto Block_15;
					}
					this.ProcessCarriageReturn(true);
					continue;
				}
				this._charPos++;
			}
			if (!flag)
			{
				throw JsonReaderException.Create(this, "Unexpected end while parsing comment.");
			}
			this.EndComment(setToken, charPos, this._charPos);
			return;
			Block_14:
			this.EndComment(setToken, charPos, this._charPos - 1);
			this._charPos++;
			return;
			Block_15:
			this.EndComment(setToken, charPos, this._charPos);
			return;
			Block_16:
			this.EndComment(setToken, charPos, this._charPos);
		}

		private void EndComment(bool setToken, int initialPosition, int endPosition)
		{
			if (setToken)
			{
				base.SetToken(JsonToken.Comment, new string(this._chars, initialPosition, endPosition - initialPosition));
			}
		}

		private bool MatchValue(string value)
		{
			if (!this.EnsureChars(value.Length - 1, true))
			{
				this._charPos = this._charsUsed;
				throw base.CreateUnexpectedEndException();
			}
			for (int i = 0; i < value.Length; i++)
			{
				if (this._chars[this._charPos + i] != value[i])
				{
					this._charPos += i;
					return false;
				}
			}
			this._charPos += value.Length;
			return true;
		}

		private bool MatchValueWithTrailingSeparator(string value)
		{
			return this.MatchValue(value) && (!this.EnsureChars(0, false) || this.IsSeparator(this._chars[this._charPos]) || this._chars[this._charPos] == '\0');
		}

		private bool IsSeparator(char c)
		{
			if (c <= ')')
			{
				switch (c)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				case '\v':
				case '\f':
					goto IL_8C;
				default:
					if (c != ' ')
					{
						if (c != ')')
						{
							goto IL_8C;
						}
						if (base.CurrentState == JsonReader.State.Constructor || base.CurrentState == JsonReader.State.ConstructorStart)
						{
							return true;
						}
						return false;
					}
					break;
				}
				return true;
			}
			if (c <= '/')
			{
				if (c != ',')
				{
					if (c != '/')
					{
						goto IL_8C;
					}
					if (!this.EnsureChars(1, false))
					{
						return false;
					}
					char c2 = this._chars[this._charPos + 1];
					return c2 == '*' || c2 == '/';
				}
			}
			else if (c != ']' && c != '}')
			{
				goto IL_8C;
			}
			return true;
			IL_8C:
			if (char.IsWhiteSpace(c))
			{
				return true;
			}
			return false;
		}

		private void ParseTrue()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.True))
			{
				base.SetToken(JsonToken.Boolean, true);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private void ParseNull()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Null))
			{
				base.SetToken(JsonToken.Null);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing null value.");
		}

		private void ParseUndefined()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.Undefined))
			{
				base.SetToken(JsonToken.Undefined);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing undefined value.");
		}

		private void ParseFalse()
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.False))
			{
				base.SetToken(JsonToken.Boolean, false);
				return;
			}
			throw JsonReaderException.Create(this, "Error parsing boolean value.");
		}

		private object ParseNumberNegativeInfinity(ReadType readType)
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.NegativeInfinity))
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.NegativeInfinity);
						return JsonConvert.NegativeInfinity;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_57;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, double.NegativeInfinity);
					return double.NegativeInfinity;
				}
				IL_57:
				throw JsonReaderException.Create(this, "Cannot read -Infinity value.");
			}
			throw JsonReaderException.Create(this, "Error parsing -Infinity value.");
		}

		private object ParseNumberPositiveInfinity(ReadType readType)
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.PositiveInfinity))
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.PositiveInfinity);
						return JsonConvert.PositiveInfinity;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_57;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, double.PositiveInfinity);
					return double.PositiveInfinity;
				}
				IL_57:
				throw JsonReaderException.Create(this, "Cannot read Infinity value.");
			}
			throw JsonReaderException.Create(this, "Error parsing Infinity value.");
		}

		private object ParseNumberNaN(ReadType readType)
		{
			if (this.MatchValueWithTrailingSeparator(JsonConvert.NaN))
			{
				if (readType != ReadType.Read)
				{
					if (readType == ReadType.ReadAsString)
					{
						base.SetToken(JsonToken.String, JsonConvert.NaN);
						return JsonConvert.NaN;
					}
					if (readType != ReadType.ReadAsDouble)
					{
						goto IL_57;
					}
				}
				if (this._floatParseHandling == FloatParseHandling.Double)
				{
					base.SetToken(JsonToken.Float, double.NaN);
					return double.NaN;
				}
				IL_57:
				throw JsonReaderException.Create(this, "Cannot read NaN value.");
			}
			throw JsonReaderException.Create(this, "Error parsing NaN value.");
		}

		public override void Close()
		{
			base.Close();
			if (this._chars != null)
			{
				BufferUtils.ReturnBuffer(this._arrayPool, this._chars);
				this._chars = null;
			}
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
			this._stringBuffer.Clear(this._arrayPool);
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start && this.LinePosition == 0 && this.TokenType != JsonToken.Comment)
				{
					return 0;
				}
				return this._lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return this._charPos - this._lineStartPos;
			}
		}

		private const char UnicodeReplacementChar = '�';

		private const int MaximumJavascriptIntegerCharacterLength = 380;

		private readonly TextReader _reader;

		private char[] _chars;

		private int _charsUsed;

		private int _charPos;

		private int _lineStartPos;

		private int _lineNumber;

		private bool _isEndOfFile;

		private StringBuffer _stringBuffer;

		private StringReference _stringReference;

		private IArrayPool<char> _arrayPool;

		internal PropertyNameTable NameTable;
	}
}
