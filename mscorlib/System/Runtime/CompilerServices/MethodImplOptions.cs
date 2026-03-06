using System;
using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	/// <summary>Defines the details of how a method is implemented.</summary>
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum MethodImplOptions
	{
		/// <summary>The method is implemented in unmanaged code.</summary>
		Unmanaged = 4,
		/// <summary>The method is declared, but its implementation is provided elsewhere.</summary>
		ForwardRef = 16,
		/// <summary>The method signature is exported exactly as declared.</summary>
		PreserveSig = 128,
		/// <summary>The call is internal, that is, it calls a method that is implemented within the common language runtime.</summary>
		InternalCall = 4096,
		/// <summary>The method can be executed by only one thread at a time. Static methods lock on the type, whereas instance methods lock on the instance. Only one thread can execute in any of the instance functions, and only one thread can execute in any of a class's static functions.</summary>
		Synchronized = 32,
		/// <summary>The method cannot be inlined. Inlining is an optimization by which a method call is replaced with the method body.</summary>
		NoInlining = 8,
		/// <summary>The method should be inlined if possible.</summary>
		[ComVisible(false)]
		AggressiveInlining = 256,
		/// <summary>The method is not optimized by the just-in-time (JIT) compiler or by native code generation (see Ngen.exe) when debugging possible code generation problems.</summary>
		NoOptimization = 64,
		/// <summary>The JIT compiler should look for security mitigation attributes, such as the user-defined <see langword="System.Runtime.CompilerServices.SecurityMitigationsAttribute" />. If found, the JIT compiler applies any related security mitigations. Available starting with .NET Framework 4.8.</summary>
		SecurityMitigations = 1024
	}
}
