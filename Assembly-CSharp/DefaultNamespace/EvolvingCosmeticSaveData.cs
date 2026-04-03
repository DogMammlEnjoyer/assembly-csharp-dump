using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaTag.Scripts.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace DefaultNamespace
{
	[NullableContext(1)]
	[Nullable(0)]
	public class EvolvingCosmeticSaveData
	{
		public static EvolvingCosmeticSaveData Instance
		{
			get
			{
				EvolvingCosmeticSaveData result;
				if ((result = EvolvingCosmeticSaveData.s_instance) == null)
				{
					result = (EvolvingCosmeticSaveData.s_instance = new EvolvingCosmeticSaveData());
				}
				return result;
			}
		}

		private EvolvingCosmeticSaveData()
		{
			string @string = PlayerPrefs.GetString("EvolvingCosmeticSaveData");
			if (@string != null)
			{
				this.ReadFromJson(@string);
			}
		}

		public string Write()
		{
			JsonSerializer jsonSerializer = new JsonSerializer();
			string result;
			using (TextWriter textWriter = new StringWriterWithEncoding(Encoding.UTF8))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
				{
					jsonSerializer.Serialize(jsonWriter, this);
					result = textWriter.ToString();
				}
			}
			return result;
		}

		private void ReadFromJson(string json)
		{
			using (TextReader textReader = new StringReader(json))
			{
				using (JsonReader jsonReader = new JsonTextReader(textReader))
				{
					while (jsonReader.Read())
					{
						if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "SelectedIndices")
						{
							this.ReadSelectedIndices(jsonReader);
						}
					}
				}
			}
		}

		private void ReadSelectedIndices(JsonReader reader)
		{
			int num = 0;
			string text = null;
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType <= JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.StartObject)
					{
						if (tokenType == JsonToken.PropertyName)
						{
							if (text != null)
							{
								throw new Exception("Json read error");
							}
							string text2 = reader.Value as string;
							if (text2 == null)
							{
								throw new Exception("Json read error");
							}
							text = text2;
						}
					}
					else
					{
						num++;
					}
				}
				else if (tokenType != JsonToken.Integer)
				{
					if (tokenType == JsonToken.EndObject)
					{
						num--;
					}
				}
				else
				{
					if (text == null)
					{
						throw new Exception("Json read error");
					}
					object value = reader.Value;
					if (!(value is long))
					{
						throw new Exception("Json read error");
					}
					long num2 = (long)value;
					this.SelectedIndices[text] = (int)num2;
				}
				if (num <= 0)
				{
					return;
				}
			}
			throw new Exception("Json read error");
		}

		public readonly Dictionary<string, int> SelectedIndices = new Dictionary<string, int>();

		[Nullable(2)]
		private static EvolvingCosmeticSaveData s_instance;

		public const string PlayerPrefsKey = "EvolvingCosmeticSaveData";
	}
}
