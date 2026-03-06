using System;
using System.Globalization;
using System.IO;

namespace Valve.Newtonsoft.Json.Serialization
{
	internal class TraceJsonReader : JsonReader, IJsonLineInfo
	{
		public TraceJsonReader(JsonReader innerReader)
		{
			this._innerReader = innerReader;
			this._sw = new StringWriter(CultureInfo.InvariantCulture);
			this._sw.Write("Deserialized JSON: " + Environment.NewLine);
			this._textWriter = new JsonTextWriter(this._sw);
			this._textWriter.Formatting = Formatting.Indented;
		}

		public string GetDeserializedJsonMessage()
		{
			return this._sw.ToString();
		}

		public override bool Read()
		{
			bool result = this._innerReader.Read();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override int? ReadAsInt32()
		{
			int? result = this._innerReader.ReadAsInt32();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override string ReadAsString()
		{
			string result = this._innerReader.ReadAsString();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override byte[] ReadAsBytes()
		{
			byte[] result = this._innerReader.ReadAsBytes();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override decimal? ReadAsDecimal()
		{
			decimal? result = this._innerReader.ReadAsDecimal();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override double? ReadAsDouble()
		{
			double? result = this._innerReader.ReadAsDouble();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override bool? ReadAsBoolean()
		{
			bool? result = this._innerReader.ReadAsBoolean();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override DateTime? ReadAsDateTime()
		{
			DateTime? result = this._innerReader.ReadAsDateTime();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? result = this._innerReader.ReadAsDateTimeOffset();
			this._textWriter.WriteToken(this._innerReader, false, false, true);
			return result;
		}

		public override int Depth
		{
			get
			{
				return this._innerReader.Depth;
			}
		}

		public override string Path
		{
			get
			{
				return this._innerReader.Path;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this._innerReader.QuoteChar;
			}
			protected internal set
			{
				this._innerReader.QuoteChar = value;
			}
		}

		public override JsonToken TokenType
		{
			get
			{
				return this._innerReader.TokenType;
			}
		}

		public override object Value
		{
			get
			{
				return this._innerReader.Value;
			}
		}

		public override Type ValueType
		{
			get
			{
				return this._innerReader.ValueType;
			}
		}

		public override void Close()
		{
			this._innerReader.Close();
		}

		bool IJsonLineInfo.HasLineInfo()
		{
			IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
			return jsonLineInfo != null && jsonLineInfo.HasLineInfo();
		}

		int IJsonLineInfo.LineNumber
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LineNumber;
			}
		}

		int IJsonLineInfo.LinePosition
		{
			get
			{
				IJsonLineInfo jsonLineInfo = this._innerReader as IJsonLineInfo;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LinePosition;
			}
		}

		private readonly JsonReader _innerReader;

		private readonly JsonTextWriter _textWriter;

		private readonly StringWriter _sw;
	}
}
