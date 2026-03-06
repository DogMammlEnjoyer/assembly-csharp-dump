using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security;

namespace System.Runtime.CompilerServices
{
	/// <summary>Provides a set of static methods and properties that provide support for compilers. This class cannot be inherited.</summary>
	public static class RuntimeHelpers
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeArray(Array array, IntPtr fldHandle);

		/// <summary>Provides a fast way to initialize an array from data that is stored in a module.</summary>
		/// <param name="array">The array to be initialized.</param>
		/// <param name="fldHandle">A field handle that specifies the location of the data used to initialize the array.</param>
		public static void InitializeArray(Array array, RuntimeFieldHandle fldHandle)
		{
			if (array == null || fldHandle.Value == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			RuntimeHelpers.InitializeArray(array, fldHandle.Value);
		}

		/// <summary>Gets the offset, in bytes, to the data in the given string.</summary>
		/// <returns>The byte offset, from the start of the <see cref="T:System.String" /> object to the first character in the string.</returns>
		public static extern int OffsetToStringData { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		/// <summary>Serves as a hash function for a particular object, and is suitable for use in algorithms and data structures that use hash codes, such as a hash table.</summary>
		/// <param name="o">An object to retrieve the hash code for.</param>
		/// <returns>A hash code for the object identified by the <paramref name="o" /> parameter.</returns>
		public static int GetHashCode(object o)
		{
			return object.InternalGetHashCode(o);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Object" /> instances are considered equal.</summary>
		/// <param name="o1">The first object to compare.</param>
		/// <param name="o2">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="o1" /> parameter is the same instance as the <paramref name="o2" /> parameter, or if both are <see langword="null" />, or if o1.Equals(o2) returns <see langword="true" />; otherwise, <see langword="false" />.</returns>
		public new static bool Equals(object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			if (o1 is ValueType)
			{
				return ValueType.DefaultEquals(o1, o2);
			}
			return object.Equals(o1, o2);
		}

		/// <summary>Boxes a value type.</summary>
		/// <param name="obj">The value type to be boxed.</param>
		/// <returns>A boxed copy of <paramref name="obj" /> if it is a value class; otherwise, <paramref name="obj" /> itself.</returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object GetObjectValue(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RunClassConstructor(IntPtr type);

		/// <summary>Runs a specified class constructor method.</summary>
		/// <param name="type">A type handle that specifies the class constructor method to run.</param>
		/// <exception cref="T:System.TypeInitializationException">The class initializer throws an exception.</exception>
		public static void RunClassConstructor(RuntimeTypeHandle type)
		{
			if (type.Value == IntPtr.Zero)
			{
				throw new ArgumentException("Handle is not initialized.", "type");
			}
			RuntimeHelpers.RunClassConstructor(type.Value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SufficientExecutionStack();

		/// <summary>Ensures that the remaining stack space is large enough to execute the average .NET Framework function.</summary>
		/// <exception cref="T:System.InsufficientExecutionStackException">The available stack space is insufficient to execute the average .NET Framework function.</exception>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void EnsureSufficientExecutionStack()
		{
			if (RuntimeHelpers.SufficientExecutionStack())
			{
				return;
			}
			throw new InsufficientExecutionStackException();
		}

		public static bool TryEnsureSufficientExecutionStack()
		{
			return RuntimeHelpers.SufficientExecutionStack();
		}

		/// <summary>Executes code using a <see cref="T:System.Delegate" /> while using another <see cref="T:System.Delegate" /> to execute additional code in case of an exception.</summary>
		/// <param name="code">A delegate to the code to try.</param>
		/// <param name="backoutCode">A delegate to the code to run if an exception occurs.</param>
		/// <param name="userData">The data to pass to <paramref name="code" /> and <paramref name="backoutCode" />.</param>
		public static void ExecuteCodeWithGuaranteedCleanup(RuntimeHelpers.TryCode code, RuntimeHelpers.CleanupCode backoutCode, object userData)
		{
		}

		/// <summary>Designates a body of code as a constrained execution region (CER).</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void PrepareConstrainedRegions()
		{
		}

		/// <summary>Designates a body of code as a constrained execution region (CER) without performing any probing.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void PrepareConstrainedRegionsNoOP()
		{
		}

		/// <summary>Probes for a certain amount of stack space to ensure that a stack overflow cannot happen within a subsequent block of code (assuming that your code uses only a finite and moderate amount of stack space). We recommend that you use a constrained execution region (CER) instead of this method.</summary>
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void ProbeForSufficientStack()
		{
		}

		/// <summary>Indicates that the specified delegate should be prepared for inclusion in a constrained execution region (CER).</summary>
		/// <param name="d">The delegate type to prepare.</param>
		[SecurityCritical]
		public static void PrepareDelegate(Delegate d)
		{
		}

		/// <summary>Provides a way for applications to dynamically prepare <see cref="T:System.AppDomain" /> event delegates.</summary>
		/// <param name="d">The event delegate to prepare.</param>
		[SecurityCritical]
		public static void PrepareContractedDelegate(Delegate d)
		{
		}

		/// <summary>Prepares a method for inclusion in a constrained execution region (CER).</summary>
		/// <param name="method">A handle to the method to prepare.</param>
		public static void PrepareMethod(RuntimeMethodHandle method)
		{
		}

		/// <summary>Prepares a method for inclusion in a constrained execution region (CER) with the specified instantiation.</summary>
		/// <param name="method">A handle to the method to prepare.</param>
		/// <param name="instantiation">The instantiation to pass to the method.</param>
		public static void PrepareMethod(RuntimeMethodHandle method, RuntimeTypeHandle[] instantiation)
		{
		}

		/// <summary>Runs a specified module constructor method.</summary>
		/// <param name="module">A handle that specifies the module constructor method to run.</param>
		/// <exception cref="T:System.TypeInitializationException">The module constructor throws an exception.</exception>
		public static void RunModuleConstructor(ModuleHandle module)
		{
			if (module == ModuleHandle.EmptyHandle)
			{
				throw new ArgumentException("Handle is not initialized.", "module");
			}
			RuntimeHelpers.RunModuleConstructor(module.Value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RunModuleConstructor(IntPtr module);

		public static bool IsReferenceOrContainsReferences<T>()
		{
			return !typeof(T).IsValueType || RuntimeTypeHandle.HasReferences(typeof(T) as RuntimeType);
		}

		public static object GetUninitializedObject(Type type)
		{
			return FormatterServices.GetUninitializedObject(type);
		}

		public static T[] GetSubArray<T>(T[] array, Range range)
		{
			Type elementType = array.GetType().GetElementType();
			Span<T> span = array.AsSpan(range);
			if (elementType.IsValueType)
			{
				return span.ToArray();
			}
			T[] array2 = (T[])Array.CreateInstance(elementType, span.Length);
			span.CopyTo(array2);
			return array2;
		}

		/// <summary>Represents a delegate to code that should be run in a try block.</summary>
		/// <param name="userData">Data to pass to the delegate.</param>
		public delegate void TryCode(object userData);

		/// <summary>Represents a method to run when an exception occurs.</summary>
		/// <param name="userData">Data to pass to the delegate.</param>
		/// <param name="exceptionThrown">
		///   <see langword="true" /> to express that an exception was thrown; otherwise, <see langword="false" />.</param>
		public delegate void CleanupCode(object userData, bool exceptionThrown);
	}
}
