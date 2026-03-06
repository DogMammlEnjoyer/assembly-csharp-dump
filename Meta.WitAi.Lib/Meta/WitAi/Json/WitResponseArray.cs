using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Meta.WitAi.Json
{
	public class WitResponseArray : WitResponseNode, IEnumerable
	{
		public override WitResponseNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					return new WitResponseLazyCreator(this);
				}
				return this.m_List[aIndex];
			}
			set
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					this.m_List.Add(value);
					return;
				}
				this.m_List[aIndex] = value;
			}
		}

		public override WitResponseNode this[string aKey]
		{
			get
			{
				return new WitResponseLazyCreator(this);
			}
			set
			{
				this.m_List.Add(value);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public override void Add(string aKey, WitResponseNode aItem)
		{
			if (aItem == null)
			{
				return;
			}
			this.m_List.Add(aItem);
		}

		public override WitResponseNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				return null;
			}
			WitResponseNode result = this.m_List[aIndex];
			this.m_List.RemoveAt(aIndex);
			return result;
		}

		public override WitResponseNode Remove(WitResponseNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		public override IEnumerable<WitResponseNode> Childs
		{
			get
			{
				foreach (WitResponseNode witResponseNode in this.m_List)
				{
					yield return witResponseNode;
				}
				List<WitResponseNode>.Enumerator enumerator = default(List<WitResponseNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		public IEnumerator GetEnumerator()
		{
			foreach (WitResponseNode witResponseNode in this.m_List)
			{
				yield return witResponseNode;
			}
			List<WitResponseNode>.Enumerator enumerator = default(List<WitResponseNode>.Enumerator);
			yield break;
			yield break;
		}

		public override string ToString()
		{
			string text = "[";
			foreach (WitResponseNode witResponseNode in this.m_List)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				text += witResponseNode.ToString();
			}
			text += "]";
			return text;
		}

		public override string ToString(string aPrefix)
		{
			string text = "[";
			foreach (WitResponseNode witResponseNode in this.m_List)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text += witResponseNode.ToString(aPrefix + "   ");
			}
			text = text + "\n" + aPrefix + "]";
			return text;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}

		private List<WitResponseNode> m_List = new List<WitResponseNode>();
	}
}
