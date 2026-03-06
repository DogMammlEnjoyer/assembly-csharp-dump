using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/CrashReport/CrashReport.bindings.h")]
	public sealed class CrashReport
	{
		private static int Compare(CrashReport c1, CrashReport c2)
		{
			long ticks = c1.time.Ticks;
			long ticks2 = c2.time.Ticks;
			bool flag = ticks > ticks2;
			int result;
			if (flag)
			{
				result = 1;
			}
			else
			{
				bool flag2 = ticks < ticks2;
				if (flag2)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		private static void PopulateReports()
		{
			object obj = CrashReport.reportsLock;
			lock (obj)
			{
				bool flag2 = CrashReport.internalReports != null;
				if (!flag2)
				{
					string[] reports = CrashReport.GetReports();
					CrashReport.internalReports = new List<CrashReport>(reports.Length);
					foreach (string text in reports)
					{
						double value;
						string reportData = CrashReport.GetReportData(text, out value);
						DateTime dateTime = new DateTime(1970, 1, 1).AddSeconds(value);
						CrashReport.internalReports.Add(new CrashReport(text, dateTime, reportData));
					}
					CrashReport.internalReports.Sort(new Comparison<CrashReport>(CrashReport.Compare));
				}
			}
		}

		public static CrashReport[] reports
		{
			get
			{
				CrashReport.PopulateReports();
				object obj = CrashReport.reportsLock;
				CrashReport[] result;
				lock (obj)
				{
					result = CrashReport.internalReports.ToArray();
				}
				return result;
			}
		}

		public static CrashReport lastReport
		{
			get
			{
				CrashReport.PopulateReports();
				object obj = CrashReport.reportsLock;
				lock (obj)
				{
					bool flag2 = CrashReport.internalReports.Count > 0;
					if (flag2)
					{
						return CrashReport.internalReports[CrashReport.internalReports.Count - 1];
					}
				}
				return null;
			}
		}

		public static void RemoveAll()
		{
			foreach (CrashReport crashReport in CrashReport.reports)
			{
				crashReport.Remove();
			}
		}

		private CrashReport(string id, DateTime time, string text)
		{
			this.id = id;
			this.time = time;
			this.text = text;
		}

		public void Remove()
		{
			bool flag = CrashReport.RemoveReport(this.id);
			if (flag)
			{
				object obj = CrashReport.reportsLock;
				lock (obj)
				{
					CrashReport.internalReports.Remove(this);
				}
			}
		}

		[FreeFunction(Name = "CrashReport_Bindings::GetReports", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetReports();

		[FreeFunction(Name = "CrashReport_Bindings::GetReportData", IsThreadSafe = true)]
		private unsafe static string GetReportData(string id, out double secondsSinceUnixEpoch)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(id, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = id.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpan;
				CrashReport.GetReportData_Injected(ref managedSpanWrapper, out secondsSinceUnixEpoch, out managedSpan);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[FreeFunction(Name = "CrashReport_Bindings::RemoveReport", IsThreadSafe = true)]
		private unsafe static bool RemoveReport(string id)
		{
			bool result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(id, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = id.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = CrashReport.RemoveReport_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetReportData_Injected(ref ManagedSpanWrapper id, out double secondsSinceUnixEpoch, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveReport_Injected(ref ManagedSpanWrapper id);

		private static List<CrashReport> internalReports;

		private static object reportsLock = new object();

		private readonly string id;

		public readonly DateTime time;

		public readonly string text;
	}
}
