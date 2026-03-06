using System;
using System.Collections.Generic;
using System.Reflection;

namespace SouthPointe.Serialization.MessagePack
{
	public class DynamicMapHandler : ITypeHandler
	{
		public DynamicMapHandler(SerializationContext context, Lazy<MapDefinition> lazyDefinition)
		{
			this.context = context;
			this.lazyDefinition = lazyDefinition;
			this.nameHandler = context.TypeHandlers.Get<string>();
			this.nameConverter = context.MapOptions.NamingStrategy;
		}

		public object Read(Format format, FormatReader reader)
		{
			MapDefinition value = this.lazyDefinition.Value;
			if (format.IsMapFamily)
			{
				object obj = Activator.CreateInstance(value.Type);
				this.InvokeCallback<OnDeserializingAttribute>(obj, value);
				for (int i = reader.ReadMapLength(format); i > 0; i--)
				{
					string text = (string)this.nameHandler.Read(reader.ReadFormat(), reader);
					text = this.nameConverter.OnUnpack(text, value);
					if (value.FieldHandlers.ContainsKey(text))
					{
						object value2 = value.FieldHandlers[text].Read(reader.ReadFormat(), reader);
						value.FieldInfos[text].SetValue(obj, value2);
					}
					else
					{
						if (!this.context.MapOptions.IgnoreUnknownFieldOnUnpack)
						{
							string str = text;
							string str2 = " does not exist for type: ";
							Type type = value.Type;
							throw new MissingFieldException(str + str2 + ((type != null) ? type.ToString() : null));
						}
						reader.Skip();
					}
				}
				this.InvokeCallback<OnDeserializedAttribute>(obj, value);
				return obj;
			}
			if (format.IsEmptyArray && this.context.MapOptions.AllowEmptyArrayOnUnpack)
			{
				return Activator.CreateInstance(value.Type);
			}
			if (format.IsNil)
			{
				return null;
			}
			throw new FormatException(this, format, reader);
		}

		public void Write(object obj, FormatWriter writer)
		{
			if (obj == null)
			{
				writer.WriteNil();
				return;
			}
			MapDefinition value = this.lazyDefinition.Value;
			this.InvokeCallback<OnSerializingAttribute>(obj, value);
			writer.WriteMapHeader(this.DetermineSize(obj, value));
			foreach (KeyValuePair<string, FieldInfo> keyValuePair in value.FieldInfos)
			{
				object value2 = keyValuePair.Value.GetValue(obj);
				if (!this.context.MapOptions.IgnoreNullOnPack || value2 != null)
				{
					string obj2 = this.nameConverter.OnPack(keyValuePair.Key, value);
					this.nameHandler.Write(obj2, writer);
					value.FieldHandlers[keyValuePair.Key].Write(value2, writer);
				}
			}
			this.InvokeCallback<OnSerializedAttribute>(obj, value);
		}

		private int DetermineSize(object obj, MapDefinition definition)
		{
			if (!this.context.MapOptions.IgnoreNullOnPack)
			{
				return definition.FieldInfos.Count;
			}
			int num = 0;
			using (Dictionary<string, FieldInfo>.ValueCollection.Enumerator enumerator = definition.FieldInfos.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetValue(obj) != null)
					{
						num++;
					}
				}
			}
			return num;
		}

		private void InvokeCallback<T>(object obj, MapDefinition definition) where T : Attribute
		{
			Type typeFromHandle = typeof(T);
			if (definition.Callbacks.ContainsKey(typeFromHandle))
			{
				MethodInfo[] array = definition.Callbacks[typeFromHandle];
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Invoke(obj, DynamicMapHandler.callbackParameters);
				}
			}
		}

		private readonly SerializationContext context;

		private readonly Lazy<MapDefinition> lazyDefinition;

		private readonly ITypeHandler nameHandler;

		private readonly IMapNamingStrategy nameConverter;

		private static readonly object[] callbackParameters = new object[0];
	}
}
