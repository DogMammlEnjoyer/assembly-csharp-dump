using System;

namespace UnityEngine.ProBuilder
{
	public struct SimpleTuple<T1, T2>
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

		public SimpleTuple(T1 item1, T2 item2)
		{
			this.m_Item1 = item1;
			this.m_Item2 = item2;
		}

		public override string ToString()
		{
			string format = "{0}, {1}";
			T1 item = this.item1;
			object arg = item.ToString();
			T2 item2 = this.item2;
			return string.Format(format, arg, item2.ToString());
		}

		private T1 m_Item1;

		private T2 m_Item2;
	}
}
