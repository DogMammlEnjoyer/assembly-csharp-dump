using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace g3
{
	public class gParallel
	{
		public static void ForEach_Sequential<T>(IEnumerable<T> source, Action<T> body)
		{
			foreach (T obj in source)
			{
				body(obj);
			}
		}

		public static void ForEach<T>(IEnumerable<T> source, Action<T> body)
		{
			Parallel.ForEach<T>(source, body);
		}

		public static void Evaluate(params Action[] funcs)
		{
			gParallel.ForEach<int>(Interval1i.Range(funcs.Length), delegate(int i)
			{
				funcs[i]();
			});
		}

		public static void BlockStartEnd(int iStart, int iEnd, Action<int, int> blockF, int iBlockSize = -1, bool bDisableParallel = false)
		{
			if (iBlockSize == -1)
			{
				iBlockSize = 100;
			}
			int num = iEnd - iStart + 1;
			int num2 = num / iBlockSize;
			if (bDisableParallel)
			{
				gParallel.ForEach_Sequential<int>(Interval1i.Range(num2), delegate(int bi)
				{
					int num5 = iStart + iBlockSize * bi;
					blockF(num5, num5 + iBlockSize - 1);
				});
			}
			else
			{
				gParallel.ForEach<int>(Interval1i.Range(num2), delegate(int bi)
				{
					int num5 = iStart + iBlockSize * bi;
					blockF(num5, num5 + iBlockSize - 1);
				});
			}
			int num3 = num - num2 * iBlockSize;
			if (num3 > 0)
			{
				int num4 = iStart + num2 * iBlockSize;
				blockF(num4, num4 + num3 - 1);
			}
		}

		private static void for_each<T>(IEnumerable<T> source, Action<T> body)
		{
			int processorCount = Environment.ProcessorCount;
			int remainingWorkItems = processorCount;
			Exception last_exception = null;
			IEnumerator<T> enumerator = source.GetEnumerator();
			try
			{
				using (ManualResetEvent mre = new ManualResetEvent(false))
				{
					WaitCallback <>9__0;
					for (int i = 0; i < processorCount; i++)
					{
						WaitCallback callBack;
						if ((callBack = <>9__0) == null)
						{
							callBack = (<>9__0 = delegate(object <p0>)
							{
								for (;;)
								{
									IEnumerator<T> enumerator = enumerator;
									T obj;
									lock (enumerator)
									{
										if (!enumerator.MoveNext())
										{
											break;
										}
										obj = enumerator.Current;
									}
									try
									{
										body(obj);
										continue;
									}
									catch (Exception last_exception)
									{
										last_exception = last_exception;
									}
									break;
								}
								if (Interlocked.Decrement(ref remainingWorkItems) == 0)
								{
									mre.Set();
								}
							});
						}
						ThreadPool.QueueUserWorkItem(callBack);
					}
					mre.WaitOne();
				}
			}
			finally
			{
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			if (last_exception != null)
			{
				throw last_exception;
			}
		}
	}
}
