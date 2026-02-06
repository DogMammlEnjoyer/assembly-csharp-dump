using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fusion
{
	internal static class ReaderWriterCache
	{
		public static IElementReaderWriter<T> Get<T>(Type readerWriterType)
		{
			Dictionary<Type, object> readerWriters = ReaderWriterCache._readerWriters;
			IElementReaderWriter<T> result;
			lock (readerWriters)
			{
				object obj;
				bool flag2 = ReaderWriterCache._readerWriters.TryGetValue(readerWriterType, out obj);
				if (flag2)
				{
					result = (IElementReaderWriter<T>)obj;
				}
				else
				{
					MethodInfo method = readerWriterType.GetMethod("GetInstance", BindingFlags.Static | BindingFlags.Public);
					bool flag3 = method == null;
					if (flag3)
					{
						throw new InvalidOperationException(string.Format("Can't find GetInstance method on {0}", readerWriterType));
					}
					object obj2 = method.Invoke(null, Array.Empty<object>());
					Assert.Check(obj2);
					ReaderWriterCache._readerWriters.Add(readerWriterType, obj2);
					result = (IElementReaderWriter<T>)obj2;
				}
			}
			return result;
		}

		private static readonly Dictionary<Type, object> _readerWriters = new Dictionary<Type, object>();
	}
}
