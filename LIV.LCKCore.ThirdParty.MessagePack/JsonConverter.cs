using System;
using System.IO;
using System.Text;

namespace SouthPointe.Serialization.MessagePack
{
	public class JsonConverter
	{
		public static string Encode(Stream stream, SerializationContext context = null)
		{
			JsonConverter jsonConverter = new JsonConverter(stream, context);
			string result;
			try
			{
				result = jsonConverter.AppendStream().ToString();
			}
			catch (Exception ex)
			{
				ex.Source = jsonConverter.builder.ToString();
				throw;
			}
			return result;
		}

		private JsonConverter(Stream stream, SerializationContext context = null)
		{
			this.context = (context ?? SerializationContext.Default);
			this.reader = new FormatReader(stream);
			this.builder = new StringBuilder();
			this.indentationSize = 0;
		}

		private JsonConverter AppendStream()
		{
			Format format = this.reader.ReadFormat();
			if (format.IsNil)
			{
				this.Append("null");
			}
			else if (format.IsFalse)
			{
				this.Append("false");
			}
			else if (format.IsTrue)
			{
				this.Append("true");
			}
			else if (format.IsPositiveFixInt)
			{
				this.Append(this.reader.ReadPositiveFixInt(format).ToString());
			}
			else if (format.IsUInt8)
			{
				this.Append(this.reader.ReadUInt8().ToString());
			}
			else if (format.IsUInt16)
			{
				this.Append(this.reader.ReadUInt16().ToString());
			}
			else if (format.IsUInt32)
			{
				this.Append(this.reader.ReadUInt32().ToString());
			}
			else if (format.IsUInt64)
			{
				this.Append(this.reader.ReadUInt64().ToString());
			}
			else if (format.IsNegativeFixInt)
			{
				this.Append(this.reader.ReadNegativeFixInt(format).ToString());
			}
			else if (format.IsInt8)
			{
				this.Append(this.reader.ReadInt8().ToString());
			}
			else if (format.IsInt16)
			{
				this.Append(this.reader.ReadInt16().ToString());
			}
			else if (format.IsInt32)
			{
				this.Append(this.reader.ReadInt32().ToString());
			}
			else if (format.IsInt64)
			{
				this.Append(this.reader.ReadInt64().ToString());
			}
			else if (format.IsFloat32)
			{
				this.Append(this.reader.ReadFloat32().ToString());
			}
			else if (format.IsFloat64)
			{
				this.Append(this.reader.ReadFloat64().ToString());
			}
			else if (format.IsFixStr)
			{
				this.AppendQuotedString(this.reader.ReadFixStr(format));
			}
			else if (format.IsStr8)
			{
				this.AppendQuotedString(this.reader.ReadStr8());
			}
			else if (format.IsStr16)
			{
				this.AppendQuotedString(this.reader.ReadStr16());
			}
			else if (format.IsStr32)
			{
				this.AppendQuotedString(this.reader.ReadStr32());
			}
			else if (format.IsBin8)
			{
				this.StringifyBinary(this.reader.ReadBin8());
			}
			else if (format.IsBin16)
			{
				this.StringifyBinary(this.reader.ReadBin16());
			}
			else if (format.IsBin32)
			{
				this.StringifyBinary(this.reader.ReadBin32());
			}
			else if (format.IsArrayFamily)
			{
				this.ReadArray(format);
			}
			else if (format.IsMapFamily)
			{
				this.ReadMap(format);
			}
			else
			{
				if (!format.IsExtFamily)
				{
					throw new FormatException(format, this.reader);
				}
				this.ReadExt(format);
			}
			return this;
		}

		public override string ToString()
		{
			return this.builder.ToString();
		}

		private JsonConverter Indent()
		{
			if (this.context.JsonOptions.PrettyPrint)
			{
				for (int i = 0; i < this.indentationSize; i++)
				{
					this.Append(this.context.JsonOptions.IndentationString);
				}
			}
			return this;
		}

		private JsonConverter Append(string str)
		{
			this.builder.Append(str);
			return this;
		}

		private JsonConverter AppendIfPretty(string str)
		{
			if (this.context.JsonOptions.PrettyPrint)
			{
				this.Append(str);
			}
			return this;
		}

		private JsonConverter ValueSeparator()
		{
			this.AppendIfPretty(this.context.JsonOptions.ValueSeparator);
			return this;
		}

		private JsonConverter AppendQuotedString(string str)
		{
			return this.Append("\"").Append(str).Append("\"");
		}

		private void StringifyBinary(byte[] bytes)
		{
			this.Append("[");
			foreach (byte b in bytes)
			{
				this.Append("0x").Append(b.ToString("X2")).Append(",");
			}
			this.Append("]");
		}

		private void ReadArray(Format format)
		{
			int num = this.reader.ReadArrayLength(format);
			if (num == 0)
			{
				this.Append("[]");
				return;
			}
			this.Append("[").ValueSeparator();
			this.indentationSize++;
			for (int i = 0; i < num; i++)
			{
				this.Indent().AppendStream();
				if (i < num - 1)
				{
					this.Append(",");
				}
				this.ValueSeparator();
			}
			this.indentationSize--;
			this.Indent().Append("]");
		}

		private void ReadMap(Format format)
		{
			int num = this.reader.ReadMapLength(format);
			if (num == 0)
			{
				this.Append("{}");
				return;
			}
			this.Append("{").ValueSeparator();
			this.indentationSize++;
			for (int i = 0; i < num; i++)
			{
				this.Indent().AppendStream().Append(":").AppendIfPretty(" ").AppendStream();
				if (i < num - 1)
				{
					this.Append(",");
				}
				this.ValueSeparator();
			}
			this.indentationSize--;
			this.Indent().Append("}");
		}

		private void ReadExt(Format format)
		{
			uint length = this.reader.ReadExtLength(format);
			sbyte extType = this.reader.ReadExtType(format);
			object obj = this.context.TypeHandlers.GetExt(extType).ReadExt(length, this.reader);
			this.Append(obj.ToString());
		}

		private readonly SerializationContext context;

		private readonly FormatReader reader;

		private readonly StringBuilder builder;

		private int indentationSize;
	}
}
