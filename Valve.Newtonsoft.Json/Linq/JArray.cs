using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Valve.Newtonsoft.Json.Utilities;

namespace Valve.Newtonsoft.Json.Linq
{
	public class JArray : JContainer, IList<JToken>, ICollection<JToken>, IEnumerable<JToken>, IEnumerable
	{
		protected override IList<JToken> ChildrenTokens
		{
			get
			{
				return this._values;
			}
		}

		public override JTokenType Type
		{
			get
			{
				return JTokenType.Array;
			}
		}

		public JArray()
		{
		}

		public JArray(JArray other) : base(other)
		{
		}

		public JArray(params object[] content) : this(content)
		{
		}

		public JArray(object content)
		{
			this.Add(content);
		}

		internal override bool DeepEquals(JToken node)
		{
			JArray jarray = node as JArray;
			return jarray != null && base.ContentsEqual(jarray);
		}

		internal override JToken CloneToken()
		{
			return new JArray(this);
		}

		public new static JArray Load(JsonReader reader)
		{
			return JArray.Load(reader, null);
		}

		public new static JArray Load(JsonReader reader, JsonLoadSettings settings)
		{
			if (reader.TokenType == JsonToken.None && !reader.Read())
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader.");
			}
			reader.MoveToContent();
			if (reader.TokenType != JsonToken.StartArray)
			{
				throw JsonReaderException.Create(reader, "Error reading JArray from JsonReader. Current JsonReader item is not an array: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			JArray jarray = new JArray();
			jarray.SetLineInfo(reader as IJsonLineInfo, settings);
			jarray.ReadTokenFrom(reader, settings);
			return jarray;
		}

		public new static JArray Parse(string json)
		{
			return JArray.Parse(json, null);
		}

		public new static JArray Parse(string json, JsonLoadSettings settings)
		{
			JArray result;
			using (JsonReader jsonReader = new JsonTextReader(new StringReader(json)))
			{
				JArray jarray = JArray.Load(jsonReader, settings);
				if (jsonReader.Read() && jsonReader.TokenType != JsonToken.Comment)
				{
					throw JsonReaderException.Create(jsonReader, "Additional text found in JSON string after parsing content.");
				}
				result = jarray;
			}
			return result;
		}

		public new static JArray FromObject(object o)
		{
			return JArray.FromObject(o, JsonSerializer.CreateDefault());
		}

		public new static JArray FromObject(object o, JsonSerializer jsonSerializer)
		{
			JToken jtoken = JToken.FromObjectInternal(o, jsonSerializer);
			if (jtoken.Type != JTokenType.Array)
			{
				throw new ArgumentException("Object serialized to {0}. JArray instance expected.".FormatWith(CultureInfo.InvariantCulture, jtoken.Type));
			}
			return (JArray)jtoken;
		}

		public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
		{
			writer.WriteStartArray();
			for (int i = 0; i < this._values.Count; i++)
			{
				this._values[i].WriteTo(writer, converters);
			}
			writer.WriteEndArray();
		}

		public override JToken this[object key]
		{
			get
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Accessed JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				return this.GetItem((int)key);
			}
			set
			{
				ValidationUtils.ArgumentNotNull(key, "key");
				if (!(key is int))
				{
					throw new ArgumentException("Set JArray values with invalid key value: {0}. Int32 array index expected.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.ToString(key)));
				}
				this.SetItem((int)key, value);
			}
		}

		public JToken this[int index]
		{
			get
			{
				return this.GetItem(index);
			}
			set
			{
				this.SetItem(index, value);
			}
		}

		internal override int IndexOfItem(JToken item)
		{
			return this._values.IndexOfReference(item);
		}

		internal override void MergeItem(object content, JsonMergeSettings settings)
		{
			IEnumerable enumerable = (base.IsMultiContent(content) || content is JArray) ? ((IEnumerable)content) : null;
			if (enumerable == null)
			{
				return;
			}
			JContainer.MergeEnumerableContent(this, enumerable, settings);
		}

		public int IndexOf(JToken item)
		{
			return this.IndexOfItem(item);
		}

		public void Insert(int index, JToken item)
		{
			this.InsertItem(index, item, false);
		}

		public void RemoveAt(int index)
		{
			this.RemoveItemAt(index);
		}

		public IEnumerator<JToken> GetEnumerator()
		{
			return this.Children().GetEnumerator();
		}

		public void Add(JToken item)
		{
			this.Add(item);
		}

		public void Clear()
		{
			this.ClearItems();
		}

		public bool Contains(JToken item)
		{
			return this.ContainsItem(item);
		}

		public void CopyTo(JToken[] array, int arrayIndex)
		{
			this.CopyItemsTo(array, arrayIndex);
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(JToken item)
		{
			return this.RemoveItem(item);
		}

		internal override int GetDeepHashCode()
		{
			return base.ContentsHashCode();
		}

		private readonly List<JToken> _values = new List<JToken>();
	}
}
