using System;

namespace Meta.WitAi.Json
{
	internal class WitResponseLazyCreator : WitResponseNode
	{
		public WitResponseLazyCreator(WitResponseNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		public WitResponseLazyCreator(WitResponseNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		private void Set(WitResponseNode aVal)
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
		}

		public override WitResponseNode this[int aIndex]
		{
			get
			{
				return new WitResponseLazyCreator(this);
			}
			set
			{
				WitResponseArray aVal = new WitResponseArray
				{
					value
				};
				this.Set(aVal);
			}
		}

		public override WitResponseNode this[string aKey]
		{
			get
			{
				return new WitResponseLazyCreator(this, aKey);
			}
			set
			{
				WitResponseClass aVal = new WitResponseClass
				{
					{
						aKey,
						value
					}
				};
				this.Set(aVal);
			}
		}

		public override void Add(WitResponseNode aItem)
		{
			WitResponseArray aVal = new WitResponseArray
			{
				aItem
			};
			this.Set(aVal);
		}

		public override void Add(string aKey, WitResponseNode aItem)
		{
			WitResponseClass aVal = new WitResponseClass
			{
				{
					aKey,
					aItem
				}
			};
			this.Set(aVal);
		}

		public static bool operator ==(WitResponseLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		public static bool operator !=(WitResponseLazyCreator a, object b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return "";
		}

		public override string ToString(string aPrefix)
		{
			return "";
		}

		public override int AsInt
		{
			get
			{
				WitResponseData aVal = new WitResponseData(0);
				this.Set(aVal);
				return 0;
			}
			set
			{
				WitResponseData aVal = new WitResponseData(value);
				this.Set(aVal);
			}
		}

		public override float AsFloat
		{
			get
			{
				WitResponseData aVal = new WitResponseData(0f);
				this.Set(aVal);
				return 0f;
			}
			set
			{
				WitResponseData aVal = new WitResponseData(value);
				this.Set(aVal);
			}
		}

		public override double AsDouble
		{
			get
			{
				WitResponseData aVal = new WitResponseData(0.0);
				this.Set(aVal);
				return 0.0;
			}
			set
			{
				WitResponseData aVal = new WitResponseData(value);
				this.Set(aVal);
			}
		}

		public override bool AsBool
		{
			get
			{
				WitResponseData aVal = new WitResponseData(false);
				this.Set(aVal);
				return false;
			}
			set
			{
				WitResponseData aVal = new WitResponseData(value);
				this.Set(aVal);
			}
		}

		public override WitResponseArray AsArray
		{
			get
			{
				WitResponseArray witResponseArray = new WitResponseArray();
				this.Set(witResponseArray);
				return witResponseArray;
			}
		}

		public override WitResponseClass AsObject
		{
			get
			{
				WitResponseClass witResponseClass = new WitResponseClass();
				this.Set(witResponseClass);
				return witResponseClass;
			}
		}

		private WitResponseNode m_Node;

		private string m_Key;
	}
}
