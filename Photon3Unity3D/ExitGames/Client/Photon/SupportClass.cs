using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using ExitGames.Client.Photon.StructWrapping;

namespace ExitGames.Client.Photon
{
	public class SupportClass
	{
		public static List<MethodInfo> GetMethods(Type type, Type attribute)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			bool flag = type == null;
			List<MethodInfo> result;
			if (flag)
			{
				result = list;
			}
			else
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					bool flag2 = attribute == null || methodInfo.IsDefined(attribute, false);
					if (flag2)
					{
						list.Add(methodInfo);
					}
				}
				result = list;
			}
			return result;
		}

		[Obsolete("Use a Stopwatch (or equivalent) instead.")]
		public static int GetTickCount()
		{
			return SupportClass.IntegerMilliseconds();
		}

		public static byte StartBackgroundCalls(Func<bool> myThread, int millisecondsInterval = 100, string taskName = "")
		{
			object threadListLock = SupportClass.ThreadListLock;
			byte result;
			lock (threadListLock)
			{
				bool flag2 = SupportClass.threadList == null;
				if (flag2)
				{
					SupportClass.threadList = new List<Thread>();
				}
				Thread thread = new Thread(delegate()
				{
					try
					{
						while (myThread())
						{
							Thread.Sleep(millisecondsInterval);
						}
					}
					catch (ThreadAbortException)
					{
					}
				});
				bool flag3 = !string.IsNullOrEmpty(taskName);
				if (flag3)
				{
					thread.Name = taskName;
				}
				thread.IsBackground = true;
				thread.Start();
				for (int i = 0; i < SupportClass.threadList.Count; i++)
				{
					bool flag4 = SupportClass.threadList[i] == null;
					if (flag4)
					{
						SupportClass.threadList[i] = thread;
						return (byte)i;
					}
				}
				bool flag5 = SupportClass.threadList.Count >= 255;
				if (flag5)
				{
					throw new NotSupportedException("StartBackgroundCalls() can run a maximum of 255 threads.");
				}
				SupportClass.threadList.Add(thread);
				result = (byte)(SupportClass.threadList.Count - 1);
			}
			return result;
		}

		public static bool StopBackgroundCalls(byte id)
		{
			object threadListLock = SupportClass.ThreadListLock;
			bool result;
			lock (threadListLock)
			{
				bool flag2 = SupportClass.threadList == null || (int)id >= SupportClass.threadList.Count || SupportClass.threadList[(int)id] == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					SupportClass.threadList[(int)id].Abort();
					SupportClass.threadList[(int)id] = null;
					result = true;
				}
			}
			return result;
		}

		public static bool StopAllBackgroundCalls()
		{
			object threadListLock = SupportClass.ThreadListLock;
			lock (threadListLock)
			{
				bool flag2 = SupportClass.threadList == null;
				if (flag2)
				{
					return false;
				}
				foreach (Thread thread in SupportClass.threadList)
				{
					bool flag3 = thread != null;
					if (flag3)
					{
						thread.Abort();
					}
				}
				SupportClass.threadList.Clear();
			}
			return true;
		}

		public static void WriteStackTrace(Exception throwable, TextWriter stream)
		{
			bool flag = stream != null;
			if (flag)
			{
				stream.WriteLine(throwable.ToString());
				stream.WriteLine(throwable.StackTrace);
				stream.Flush();
			}
			else
			{
				Debug.WriteLine(throwable.ToString());
				Debug.WriteLine(throwable.StackTrace);
			}
		}

		public static void WriteStackTrace(Exception throwable)
		{
			SupportClass.WriteStackTrace(throwable, null);
		}

		public static string DictionaryToString(IDictionary dictionary, bool includeTypes = true)
		{
			bool flag = dictionary == null;
			string result;
			if (flag)
			{
				result = "null";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				foreach (object obj in dictionary.Keys)
				{
					bool flag2 = stringBuilder.Length > 1;
					if (flag2)
					{
						stringBuilder.Append(", ");
					}
					bool flag3 = dictionary[obj] == null;
					Type type;
					string text;
					if (flag3)
					{
						type = typeof(object);
						text = "null";
					}
					else
					{
						type = dictionary[obj].GetType();
						text = dictionary[obj].ToString();
					}
					bool flag4 = type == typeof(IDictionary) || type == typeof(Hashtable);
					if (flag4)
					{
						text = SupportClass.DictionaryToString((IDictionary)dictionary[obj], true);
					}
					else
					{
						bool flag5 = type == typeof(NonAllocDictionary<byte, object>);
						if (flag5)
						{
							text = SupportClass.DictionaryToString((NonAllocDictionary<byte, object>)dictionary[obj], true);
						}
						else
						{
							bool flag6 = type == typeof(string[]);
							if (flag6)
							{
								text = string.Format("{{{0}}}", string.Join(",", (string[])dictionary[obj]));
							}
							else
							{
								bool flag7 = type == typeof(byte[]);
								if (flag7)
								{
									text = string.Format("byte[{0}]", ((byte[])dictionary[obj]).Length);
								}
								else
								{
									StructWrapper structWrapper = dictionary[obj] as StructWrapper;
									bool flag8 = structWrapper != null;
									if (flag8)
									{
										stringBuilder.AppendFormat("{0}={1}", obj, structWrapper.ToString(includeTypes));
										continue;
									}
								}
							}
						}
					}
					if (includeTypes)
					{
						stringBuilder.AppendFormat("({0}){1}=({2}){3}", new object[]
						{
							obj.GetType().Name,
							obj,
							type.Name,
							text
						});
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}", obj, text);
					}
				}
				stringBuilder.Append("}");
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static string DictionaryToString(NonAllocDictionary<byte, object> dictionary, bool includeTypes = true)
		{
			bool flag = dictionary == null;
			string result;
			if (flag)
			{
				result = "null";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("{");
				foreach (byte b in dictionary.Keys)
				{
					bool flag2 = stringBuilder.Length > 1;
					if (flag2)
					{
						stringBuilder.Append(", ");
					}
					bool flag3 = dictionary[b] == null;
					Type type;
					string text;
					if (flag3)
					{
						type = typeof(object);
						text = "null";
					}
					else
					{
						type = dictionary[b].GetType();
						text = dictionary[b].ToString();
					}
					bool flag4 = type == typeof(IDictionary) || type == typeof(Hashtable);
					if (flag4)
					{
						text = SupportClass.DictionaryToString((IDictionary)dictionary[b], true);
					}
					else
					{
						bool flag5 = type == typeof(NonAllocDictionary<byte, object>);
						if (flag5)
						{
							text = SupportClass.DictionaryToString((NonAllocDictionary<byte, object>)dictionary[b], true);
						}
						else
						{
							bool flag6 = type == typeof(string[]);
							if (flag6)
							{
								text = string.Format("{{{0}}}", string.Join(",", (string[])dictionary[b]));
							}
							else
							{
								bool flag7 = type == typeof(byte[]);
								if (flag7)
								{
									text = string.Format("byte[{0}]", ((byte[])dictionary[b]).Length);
								}
								else
								{
									StructWrapper structWrapper = dictionary[b] as StructWrapper;
									bool flag8 = structWrapper != null;
									if (flag8)
									{
										stringBuilder.AppendFormat("{0}={1}", b, structWrapper.ToString(includeTypes));
										continue;
									}
								}
							}
						}
					}
					if (includeTypes)
					{
						stringBuilder.AppendFormat("({0}){1}=({2}){3}", new object[]
						{
							b.GetType().Name,
							b,
							type.Name,
							text
						});
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}", b, text);
					}
				}
				stringBuilder.Append("}");
				result = stringBuilder.ToString();
			}
			return result;
		}

		[Obsolete("Use DictionaryToString() instead.")]
		public static string HashtableToString(Hashtable hash)
		{
			return SupportClass.DictionaryToString(hash, true);
		}

		public static string ByteArrayToString(byte[] list, int length = -1)
		{
			bool flag = list == null;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = length < 0 || length > list.Length;
				if (flag2)
				{
					length = list.Length;
				}
				result = BitConverter.ToString(list, 0, length);
			}
			return result;
		}

		private static uint[] InitializeTable(uint polynomial)
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					bool flag = (num & 1U) == 1U;
					if (flag)
					{
						num = (num >> 1 ^ polynomial);
					}
					else
					{
						num >>= 1;
					}
				}
				array[i] = num;
			}
			return array;
		}

		public static uint CalculateCrc(byte[] buffer, int length)
		{
			uint num = uint.MaxValue;
			uint polynomial = 3988292384U;
			bool flag = SupportClass.crcLookupTable == null;
			if (flag)
			{
				SupportClass.crcLookupTable = SupportClass.InitializeTable(polynomial);
			}
			for (int i = 0; i < length; i++)
			{
				num = (num >> 8 ^ SupportClass.crcLookupTable[(int)((uint)buffer[i] ^ (num & 255U))]);
			}
			return num;
		}

		private static List<Thread> threadList;

		private static readonly object ThreadListLock = new object();

		[Obsolete("Use a Stopwatch (or equivalent) instead.")]
		protected internal static SupportClass.IntegerMillisecondsDelegate IntegerMilliseconds = () => Environment.TickCount;

		private static uint[] crcLookupTable;

		[Obsolete("Use a Stopwatch (or equivalent) instead.")]
		public delegate int IntegerMillisecondsDelegate();

		public class ThreadSafeRandom
		{
			public static int Next()
			{
				Random r = SupportClass.ThreadSafeRandom._r;
				int result;
				lock (r)
				{
					result = SupportClass.ThreadSafeRandom._r.Next();
				}
				return result;
			}

			private static readonly Random _r = new Random();
		}
	}
}
