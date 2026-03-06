using System;

namespace UnityEngine.ProBuilder
{
	internal struct SimpleTuple<T1, T2, T3>
	{
		public T1 item1
		{
			get
			{
				return this.m_Item1;
			}
			set
			{
				this.m_Item1 = value;
			}
		}

		public T2 item2
		{
			get
			{
				return this.m_Item2;
			}
			set
			{
				this.m_Item2 = value;
			}
		}

		public T3 item3
		{
			get
			{
				return this.m_Item3;
			}
			set
			{
				this.m_Item3 = value;
			}
		}

		public SimpleTuple(T1 item1, T2 item2, T3 item3)
		{
			this.m_Item1 = item1;
			this.m_Item2 = item2;
			this.m_Item3 = item3;
		}

		public override string ToString()
		{
			string format = "{0}, {1}, {2}";
			T1 item = this.item1;
			object arg = item.ToString();
			T2 item2 = this.item2;
			object arg2 = item2.ToString();
			T3 item3 = this.item3;
			return string.Format(format, arg, arg2, item3.ToString());
		}

		private T1 m_Item1;

		private T2 m_Item2;

		private T3 m_Item3;
	}
}
