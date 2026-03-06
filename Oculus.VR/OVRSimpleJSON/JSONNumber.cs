using System;
using System.Globalization;
using System.Text;

namespace OVRSimpleJSON
{
	public class JSONNumber : JSONNode
	{
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		public override string Value
		{
			get
			{
				return this.m_Data.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				double data;
				if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out data))
				{
					this.m_Data = data;
				}
			}
		}

		public override double AsDouble
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public override long AsLong
		{
			get
			{
				return (long)this.m_Data;
			}
			set
			{
				this.m_Data = (double)value;
			}
		}

		public override ulong AsULong
		{
			get
			{
				return (ulong)this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		public override JSONNode Clone()
		{
			return new JSONNumber(this.m_Data);
		}

		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.Value.ToString(CultureInfo.InvariantCulture));
		}

		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JSONNumber jsonnumber = obj as JSONNumber;
			if (jsonnumber != null)
			{
				return this.m_Data == jsonnumber.m_Data;
			}
			return JSONNumber.IsNumeric(obj) && Convert.ToDouble(obj) == this.m_Data;
		}

		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		public override void Clear()
		{
			this.m_Data = 0.0;
		}

		private double m_Data;
	}
}
