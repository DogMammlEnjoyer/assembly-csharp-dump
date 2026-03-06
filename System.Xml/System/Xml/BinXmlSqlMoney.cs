using System;
using System.Globalization;

namespace System.Xml
{
	internal struct BinXmlSqlMoney
	{
		public BinXmlSqlMoney(int v)
		{
			this.data = (long)v;
		}

		public BinXmlSqlMoney(long v)
		{
			this.data = v;
		}

		public decimal ToDecimal()
		{
			bool isNegative;
			ulong num;
			if (this.data < 0L)
			{
				isNegative = true;
				num = (ulong)(-(ulong)this.data);
			}
			else
			{
				isNegative = false;
				num = (ulong)this.data;
			}
			return new decimal((int)num, (int)(num >> 32), 0, isNegative, 4);
		}

		public override string ToString()
		{
			return this.ToDecimal().ToString("#0.00##", CultureInfo.InvariantCulture);
		}

		private long data;
	}
}
