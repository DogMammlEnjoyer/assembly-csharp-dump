using System;
using System.Globalization;
using System.Text;

namespace OVRSimpleJSON
{
	internal class JSONLazyCreator : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.None;
			}
		}

		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		private T Set<T>(T aVal) where T : JSONNode
		{
			if (this.m_Key == null)
			{
				this.m_Node.Add(aVal);
			}
			else
			{
				this.m_Node.Add(this.m_Key, aVal);
			}
			this.m_Node = null;
			return aVal;
		}

		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				this.Set<JSONArray>(new JSONArray()).Add(value);
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				this.Set<JSONObject>(new JSONObject()).Add(aKey, value);
			}
		}

		public override void Add(JSONNode aItem)
		{
			this.Set<JSONArray>(new JSONArray()).Add(aItem);
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			this.Set<JSONObject>(new JSONObject()).Add(aKey, aItem);
		}

		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override int AsInt
		{
			get
			{
				this.Set<JSONNumber>(new JSONNumber(0.0));
				return 0;
			}
			set
			{
				this.Set<JSONNumber>(new JSONNumber((double)value));
			}
		}

		public override float AsFloat
		{
			get
			{
				this.Set<JSONNumber>(new JSONNumber(0.0));
				return 0f;
			}
			set
			{
				this.Set<JSONNumber>(new JSONNumber((double)value));
			}
		}

		public override double AsDouble
		{
			get
			{
				this.Set<JSONNumber>(new JSONNumber(0.0));
				return 0.0;
			}
			set
			{
				this.Set<JSONNumber>(new JSONNumber(value));
			}
		}

		public override long AsLong
		{
			get
			{
				if (JSONNode.longAsString)
				{
					this.Set<JSONString>(new JSONString("0"));
				}
				else
				{
					this.Set<JSONNumber>(new JSONNumber(0.0));
				}
				return 0L;
			}
			set
			{
				if (JSONNode.longAsString)
				{
					this.Set<JSONString>(new JSONString(value.ToString(CultureInfo.InvariantCulture)));
					return;
				}
				this.Set<JSONNumber>(new JSONNumber((double)value));
			}
		}

		public override ulong AsULong
		{
			get
			{
				if (JSONNode.longAsString)
				{
					this.Set<JSONString>(new JSONString("0"));
				}
				else
				{
					this.Set<JSONNumber>(new JSONNumber(0.0));
				}
				return 0UL;
			}
			set
			{
				if (JSONNode.longAsString)
				{
					this.Set<JSONString>(new JSONString(value.ToString(CultureInfo.InvariantCulture)));
					return;
				}
				this.Set<JSONNumber>(new JSONNumber(value));
			}
		}

		public override bool AsBool
		{
			get
			{
				this.Set<JSONBool>(new JSONBool(false));
				return false;
			}
			set
			{
				this.Set<JSONBool>(new JSONBool(value));
			}
		}

		public override JSONArray AsArray
		{
			get
			{
				return this.Set<JSONArray>(new JSONArray());
			}
		}

		public override JSONObject AsObject
		{
			get
			{
				return this.Set<JSONObject>(new JSONObject());
			}
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		private JSONNode m_Node;

		private string m_Key;
	}
}
