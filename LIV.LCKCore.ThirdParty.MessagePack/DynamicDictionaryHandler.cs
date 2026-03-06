using System;
using System.Collections;

namespace SouthPointe.Serialization.MessagePack
{
	public class DynamicDictionaryHandler : ITypeHandler
	{
		public DynamicDictionaryHandler(SerializationContext context, Type type)
		{
			Type[] genericArguments = type.GetGenericArguments();
			this.type = type;
			this.keyHandler = context.TypeHandlers.Get(genericArguments[0]);
			this.valueHandler = context.TypeHandlers.Get(genericArguments[1]);
		}

		public object Read(Format format, FormatReader reader)
		{
			IDictionary dictionary = (IDictionary)Activator.CreateInstance(this.type);
			if (format.IsNil)
			{
				return dictionary;
			}
			for (int i = reader.ReadMapLength(format); i > 0; i--)
			{
				object key = this.keyHandler.Read(reader.ReadFormat(), reader);
				object value = this.valueHandler.Read(reader.ReadFormat(), reader);
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		public void Write(object obj, FormatWriter writer)
		{
			if (obj == null)
			{
				writer.WriteNil();
				return;
			}
			IDictionary dictionary = (IDictionary)obj;
			writer.WriteMapHeader(dictionary.Count);
			foreach (object obj2 in dictionary)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
				this.keyHandler.Write(dictionaryEntry.Key, writer);
				this.valueHandler.Write(dictionaryEntry.Value, writer);
			}
		}

		private readonly Type type;

		private readonly ITypeHandler keyHandler;

		private readonly ITypeHandler valueHandler;
	}
}
