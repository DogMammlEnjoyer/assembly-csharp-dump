using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	public class Hashtable : Dictionary<object, object>
	{
		public static object GetBoxedByte(byte value)
		{
			return Hashtable.boxedByte[(int)value];
		}

		static Hashtable()
		{
			int num = 256;
			Hashtable.boxedByte = new object[num];
			for (int i = 0; i < num; i++)
			{
				Hashtable.boxedByte[i] = (byte)i;
			}
		}

		public Hashtable()
		{
		}

		public Hashtable(int x) : base(x)
		{
		}

		public new object this[object key]
		{
			get
			{
				object result = null;
				base.TryGetValue(key, out result);
				return result;
			}
			set
			{
				base[key] = value;
			}
		}

		public object this[byte key]
		{
			get
			{
				object result = null;
				base.TryGetValue(Hashtable.boxedByte[(int)key], out result);
				return result;
			}
			set
			{
				base[Hashtable.boxedByte[(int)key]] = value;
			}
		}

		public void Add(byte k, object v)
		{
			base.Add(Hashtable.boxedByte[(int)k], v);
		}

		public void Remove(byte k)
		{
			base.Remove(Hashtable.boxedByte[(int)k]);
		}

		public bool ContainsKey(byte key)
		{
			return base.ContainsKey(Hashtable.boxedByte[(int)key]);
		}

		public new DictionaryEntryEnumerator GetEnumerator()
		{
			return new DictionaryEntryEnumerator(base.GetEnumerator());
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (object obj in base.Keys)
			{
				bool flag = obj == null || this[obj] == null;
				if (flag)
				{
					List<string> list2 = list;
					string str = (obj != null) ? obj.ToString() : null;
					string str2 = "=";
					object obj2 = this[obj];
					list2.Add(str + str2 + ((obj2 != null) ? obj2.ToString() : null));
				}
				else
				{
					List<string> list3 = list;
					string[] array = new string[8];
					array[0] = "(";
					int num = 1;
					Type type = obj.GetType();
					array[num] = ((type != null) ? type.ToString() : null);
					array[2] = ")";
					array[3] = ((obj != null) ? obj.ToString() : null);
					array[4] = "=(";
					int num2 = 5;
					Type type2 = this[obj].GetType();
					array[num2] = ((type2 != null) ? type2.ToString() : null);
					array[6] = ")";
					int num3 = 7;
					object obj3 = this[obj];
					array[num3] = ((obj3 != null) ? obj3.ToString() : null);
					list3.Add(string.Concat(array));
				}
			}
			return string.Join(", ", list.ToArray());
		}

		public object Clone()
		{
			return new Dictionary<object, object>(this);
		}

		internal static readonly object[] boxedByte;
	}
}
