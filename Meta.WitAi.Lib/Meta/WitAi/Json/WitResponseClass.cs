using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meta.WitAi.Json
{
	public class WitResponseClass : WitResponseNode, IEnumerable
	{
		public override string[] ChildNodeNames
		{
			get
			{
				return this.m_Dict.Keys.ToArray<string>();
			}
		}

		public bool HasChild(string child)
		{
			return this.m_Dict.ContainsKey(child);
		}

		public override WitResponseNode this[string aKey]
		{
			get
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					return this.m_Dict[aKey];
				}
				return new WitResponseLazyCreator(this, aKey);
			}
			set
			{
				if (string.IsNullOrEmpty(aKey))
				{
					return;
				}
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = value;
					return;
				}
				this.m_Dict.TryAdd(aKey, value);
			}
		}

		public override WitResponseNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return null;
				}
				return this.m_Dict.ElementAt(aIndex).Value;
			}
			set
			{
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return;
				}
				string key = this.m_Dict.ElementAt(aIndex).Key;
				this.m_Dict[key] = value;
			}
		}

		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		public override void Add(string aKey, WitResponseNode aItem)
		{
			if (string.IsNullOrEmpty(aKey))
			{
				this.m_Dict.TryAdd(Guid.NewGuid().ToString(), aItem);
				return;
			}
			if (this.m_Dict.ContainsKey(aKey))
			{
				this.m_Dict[aKey] = aItem;
				return;
			}
			this.m_Dict.TryAdd(aKey, aItem);
		}

		public override WitResponseNode Remove(string aKey)
		{
			if (!this.m_Dict.ContainsKey(aKey))
			{
				return null;
			}
			WitResponseNode result;
			this.m_Dict.TryRemove(aKey, out result);
			return result;
		}

		public override WitResponseNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_Dict.Count)
			{
				return null;
			}
			KeyValuePair<string, WitResponseNode> keyValuePair = this.m_Dict.ElementAt(aIndex);
			WitResponseNode result;
			this.m_Dict.TryRemove(keyValuePair.Key, out result);
			return result;
		}

		public override WitResponseNode Remove(WitResponseNode aNode)
		{
			WitResponseNode result;
			try
			{
				KeyValuePair<string, WitResponseNode> keyValuePair = (from k in this.m_Dict
				where k.Value == aNode
				select k).First<KeyValuePair<string, WitResponseNode>>();
				WitResponseNode witResponseNode;
				this.m_Dict.TryRemove(keyValuePair.Key, out witResponseNode);
				result = aNode;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public override IEnumerable<WitResponseNode> Childs
		{
			get
			{
				foreach (KeyValuePair<string, WitResponseNode> keyValuePair in this.m_Dict)
				{
					yield return keyValuePair.Value;
				}
				IEnumerator<KeyValuePair<string, WitResponseNode>> enumerator = null;
				yield break;
				yield break;
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (KeyValuePair<string, WitResponseNode> keyValuePair in this.m_Dict)
			{
				yield return keyValuePair;
			}
			IEnumerator<KeyValuePair<string, WitResponseNode>> enumerator = null;
			yield break;
			yield break;
		}

		public T GetChild<T>(string aKey, T defaultValue = default(T))
		{
			if (!this.HasChild(aKey))
			{
				return defaultValue;
			}
			WitResponseNode witResponseNode = this[aKey];
			if (!(witResponseNode == null))
			{
				return witResponseNode.Cast<T>(defaultValue);
			}
			return defaultValue;
		}

		public override string ToString()
		{
			return this.ToFilteredString(false);
		}

		public string ToString(bool ignoreEmptyFields)
		{
			return this.ToFilteredString(ignoreEmptyFields);
		}

		private string ToFilteredString(bool ignoreEmptyFields = false)
		{
			string text = "{";
			foreach (KeyValuePair<string, WitResponseNode> keyValuePair in this.m_Dict.ToArray())
			{
				if (!ignoreEmptyFields || !string.IsNullOrEmpty(keyValuePair.Value))
				{
					if (text.Length > 2)
					{
						text += ", ";
					}
					string[] array2 = new string[5];
					array2[0] = text;
					array2[1] = "\"";
					array2[2] = WitResponseNode.Escape(keyValuePair.Key);
					array2[3] = "\": ";
					int num = 4;
					WitResponseNode value = keyValuePair.Value;
					array2[num] = (((value != null) ? value.ToString() : null) ?? "\"\"");
					text = string.Concat(array2);
				}
			}
			return text + "}";
		}

		public override string ToString(string aPrefix)
		{
			string text = "{ ";
			foreach (KeyValuePair<string, WitResponseNode> keyValuePair in this.m_Dict)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				string[] array = new string[5];
				array[0] = text;
				array[1] = "\"";
				array[2] = WitResponseNode.Escape(keyValuePair.Key);
				array[3] = "\": ";
				int num = 4;
				WitResponseNode value = keyValuePair.Value;
				array[num] = (((value != null) ? value.ToString(aPrefix) : null) ?? "\"\"");
				text = string.Concat(array);
			}
			text = text + "\n" + aPrefix + "}";
			return text;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(2);
			aWriter.Write(this.m_Dict.Count);
			foreach (string text in this.m_Dict.Keys)
			{
				aWriter.Write(text);
				this.m_Dict[text].Serialize(aWriter);
			}
		}

		private ConcurrentDictionary<string, WitResponseNode> m_Dict = new ConcurrentDictionary<string, WitResponseNode>();
	}
}
