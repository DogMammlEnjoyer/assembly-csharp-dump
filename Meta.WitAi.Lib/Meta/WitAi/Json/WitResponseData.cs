using System;
using System.IO;

namespace Meta.WitAi.Json
{
	public class WitResponseData : WitResponseNode
	{
		public override string Value
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

		public WitResponseData()
		{
			this.m_Data = "";
		}

		public WitResponseData(string aData)
		{
			this.m_Data = aData;
		}

		public WitResponseData(float aData)
		{
			this.AsFloat = aData;
		}

		public WitResponseData(double aData)
		{
			this.AsDouble = aData;
		}

		public WitResponseData(bool aData)
		{
			this.AsBool = aData;
		}

		public WitResponseData(int aData)
		{
			this.AsInt = aData;
		}

		public override string ToString()
		{
			return "\"" + WitResponseNode.Escape(this.m_Data) + "\"";
		}

		public override string ToString(string aPrefix)
		{
			return "\"" + WitResponseNode.Escape(this.m_Data) + "\"";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			WitResponseData witResponseData = new WitResponseData("")
			{
				AsInt = this.AsInt
			};
			if (witResponseData.m_Data == this.m_Data)
			{
				aWriter.Write(4);
				aWriter.Write(this.AsInt);
				return;
			}
			witResponseData.AsFloat = this.AsFloat;
			if (witResponseData.m_Data == this.m_Data)
			{
				aWriter.Write(7);
				aWriter.Write(this.AsFloat);
				return;
			}
			witResponseData.AsDouble = this.AsDouble;
			if (witResponseData.m_Data == this.m_Data)
			{
				aWriter.Write(5);
				aWriter.Write(this.AsDouble);
				return;
			}
			witResponseData.AsBool = this.AsBool;
			if (witResponseData.m_Data == this.m_Data)
			{
				aWriter.Write(6);
				aWriter.Write(this.AsBool);
				return;
			}
			aWriter.Write(3);
			aWriter.Write(this.m_Data);
		}

		private string m_Data;
	}
}
