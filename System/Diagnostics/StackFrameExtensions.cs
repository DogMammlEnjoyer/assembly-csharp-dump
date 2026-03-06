using System;

namespace System.Diagnostics
{
	/// <summary>Provides extension methods for the <see cref="T:System.Diagnostics.StackFrame" /> class, which represents a function call on the call stack for the current thread.</summary>
	public static class StackFrameExtensions
	{
		/// <summary>Indicates whether the native image is available for the specified stack frame.</summary>
		/// <param name="stackFrame">A stack frame.</param>
		/// <returns>
		///   <see langword="true" /> if a native image is available for this stack frame; otherwise, <see langword="false" />.</returns>
		public static bool HasNativeImage(this StackFrame stackFrame)
		{
			return stackFrame.GetNativeImageBase() != IntPtr.Zero;
		}

		/// <summary>Indicates whether information about the method in which the specified frame is executing is available.</summary>
		/// <param name="stackFrame">A stack frame.</param>
		/// <returns>
		///   <see langword="true" /> if information about the method in which the current frame is executing is available; otherwise, <see langword="false" />.</returns>
		public static bool HasMethod(this StackFrame stackFrame)
		{
			return stackFrame.GetMethod() != null;
		}

		/// <summary>Indicates whether an offset from the start of the IL code for the method that is executing is available.</summary>
		/// <param name="stackFrame">A stack frame.</param>
		/// <returns>
		///   <see langword="true" /> if the offset is available; otherwise, <see langword="false" />.</returns>
		public static bool HasILOffset(this StackFrame stackFrame)
		{
			return stackFrame.GetILOffset() != -1;
		}

		/// <summary>Indicates whether the file that contains the code that the specified stack frame is executing is available.</summary>
		/// <param name="stackFrame">A stack frame.</param>
		/// <returns>
		///   <see langword="true" /> if the code that the specified stack frame is executing is available; otherwise, <see langword="false" />.</returns>
		public static bool HasSource(this StackFrame stackFrame)
		{
			return stackFrame.GetFileName() != null;
		}

		/// <summary>Gets an interface pointer to the start of the native code for the method that is being executed.</summary>
		/// <param name="stackFrame">A stack frame.</param>
		/// <returns>An interface pointer to the start of the native code for the method that is being executed or <see cref="F:System.IntPtr.Zero" /> if you're targeting the .NET Framework.</returns>
		public static IntPtr GetNativeIP(this StackFrame stackFrame)
		{
			return IntPtr.Zero;
		}

		/// <summary>Returns a pointer to the base address of the native image that this stack frame is executing.</summary>
		/// <param name="stackFrame">A stack frame.</param>
		/// <returns>A pointer to the base address of the native image or <see cref="F:System.IntPtr.Zero" /> if you're targeting the .NET Framework.</returns>
		public static IntPtr GetNativeImageBase(this StackFrame stackFrame)
		{
			return IntPtr.Zero;
		}
	}
}
