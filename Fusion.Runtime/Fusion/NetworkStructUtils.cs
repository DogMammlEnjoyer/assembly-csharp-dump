using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class NetworkStructUtils
	{
		internal static void ResetStatics()
		{
			NetworkStructUtils._wordCounts.Clear();
		}

		public static int GetWordCount<[IsUnmanaged] T>() where T : struct, ValueType, INetworkStruct
		{
			return NetworkStructUtils.GetWordCount(typeof(T));
		}

		public static int GetWordCount(Type type)
		{
			Assert.Check(typeof(INetworkStruct).IsAssignableFrom(type));
			int wordCount;
			bool flag = !NetworkStructUtils._wordCounts.TryGetValue(type, out wordCount);
			if (flag)
			{
				NetworkStructWeavedAttribute networkStructWeavedAttribute = (NetworkStructWeavedAttribute)type.GetCustomAttributes(typeof(NetworkStructWeavedAttribute), false)[0];
				int num = Native.SizeOf(type);
				int num2 = networkStructWeavedAttribute.WordCount * 4;
				bool isGenericComposite = networkStructWeavedAttribute.IsGenericComposite;
				if (isGenericComposite)
				{
					Assert.Always<Type>(type.IsGenericType, "Type not generic {0}", type);
					foreach (Type type2 in type.GetGenericArguments())
					{
						bool flag2 = typeof(INetworkStruct).IsAssignableFrom(type2);
						if (flag2)
						{
							num2 += NetworkStructUtils.GetWordCount(type2) * 4;
						}
					}
				}
				bool flag3 = num2 != num;
				if (flag3)
				{
					Assert.AlwaysFail(string.Format("Size of {0} is invalid, expected size {1} but was size {2}", type, num2, num));
				}
				NetworkStructUtils._wordCounts.Add(type, wordCount = networkStructWeavedAttribute.WordCount);
			}
			return wordCount;
		}

		private static Dictionary<Type, int> _wordCounts = new Dictionary<Type, int>();
	}
}
