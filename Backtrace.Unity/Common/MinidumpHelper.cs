using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Backtrace.Unity.Types;
using UnityEngine;

namespace Backtrace.Unity.Common
{
	internal static class MinidumpHelper
	{
		private static bool IsMemoryDumpAvailable()
		{
			return (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) && SystemHelper.IsLibraryAvailable(MinidumpHelper.Libraries);
		}

		[DllImport("dbghelp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, ref MiniDumpExceptionInformation expParam, IntPtr userStreamParam, IntPtr callbackParam);

		[DllImport("dbghelp.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		internal static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

		internal static bool Write(string filePath, MiniDumpType options = MiniDumpType.WithFullMemory, MinidumpException exceptionType = MinidumpException.None)
		{
			if (!MinidumpHelper.IsMemoryDumpAvailable())
			{
				return false;
			}
			Process currentProcess = Process.GetCurrentProcess();
			IntPtr handle = currentProcess.Handle;
			uint id = (uint)currentProcess.Id;
			MiniDumpExceptionInformation instance = MiniDumpExceptionInformation.GetInstance(exceptionType);
			bool result;
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
			{
				result = ((instance.ExceptionPointers == IntPtr.Zero) ? MinidumpHelper.MiniDumpWriteDump(handle, id, fileStream.SafeFileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) : MinidumpHelper.MiniDumpWriteDump(handle, id, fileStream.SafeFileHandle, (uint)options, ref instance, IntPtr.Zero, IntPtr.Zero));
			}
			return result;
		}

		private static readonly string[] Libraries = new string[]
		{
			"kernel32.dll",
			"dbghelp.dll"
		};
	}
}
