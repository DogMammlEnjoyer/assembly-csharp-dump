using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Fusion
{
	public static class Assert
	{
		[Conditional("DEBUG")]
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Fail()
		{
			throw new AssertException();
		}

		[Conditional("DEBUG")]
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Fail(string error)
		{
			throw new AssertException(error);
		}

		[Conditional("DEBUG")]
		[DoesNotReturn]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Fail(string format, params object[] args)
		{
			throw new AssertException(string.Format(format, args));
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:null=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check(object condition)
		{
			if (condition == null)
			{
				throw new AssertException();
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:null=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public unsafe static void Check(void* condition)
		{
			if (condition == null)
			{
				throw new AssertException();
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check([DoesNotReturnIf(false)] bool condition)
		{
			if (!condition)
			{
				throw new AssertException();
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check([DoesNotReturnIf(false)] bool condition, string error)
		{
			if (!condition)
			{
				throw new AssertException(error);
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, arg0));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0, T1 arg1)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, arg0, arg1));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1, T2>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, arg0, arg1, arg2));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1, T2, T3>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3
				}));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0>([DoesNotReturnIf(false)] bool condition, T0 arg0)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("{0}", arg0));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1}", arg0, arg1));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1, T2>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1} arg2:{2}", arg0, arg1, arg2));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1, T2, T3>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1} arg2:{2} arg3:{3}", new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3
				}));
			}
		}

		[Conditional("DEBUG")]
		[AssertionMethod]
		[ContractAnnotation("condition:false=>halt")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Check<T0, T1, T2, T3, T4>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1} arg2:{2} arg3:{3} arg4:{4}", new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3,
					arg4
				}));
			}
		}

		[Obsolete("Use overload with a message instead")]
		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void AlwaysFail()
		{
			throw new AssertException();
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void AlwaysFail(string error)
		{
			throw new AssertException(error);
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void AlwaysFail(object error)
		{
			throw new AssertException((error != null) ? error.ToString() : null);
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void AlwaysFail<T>(T error) where T : struct
		{
			throw new AssertException(error.ToString());
		}

		[Obsolete("Use overload with a message instead")]
		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always([DoesNotReturnIf(false)] bool condition)
		{
			if (!condition)
			{
				throw new AssertException();
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always([DoesNotReturnIf(false)] bool condition, string error)
		{
			if (!condition)
			{
				throw new AssertException(error);
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, arg0));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0, T1>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0, T1 arg1)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, arg0, arg1));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0, T1, T2>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, arg0, arg1, arg2));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[StringFormatMethod("format")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0, T1, T2, T3>([DoesNotReturnIf(false)] bool condition, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			if (!condition)
			{
				throw new AssertException(string.Format(format, new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3
				}));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0>([DoesNotReturnIf(false)] bool condition, T0 arg0)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0}", arg0));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0, T1>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1}", arg0, arg1));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0, T1, T2>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1, T2 arg2)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1} arg2:{2}", arg0, arg1, arg2));
			}
		}

		[ContractAnnotation("condition:false=>halt")]
		[AssertionMethod]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Always<T0, T1, T2, T3>([DoesNotReturnIf(false)] bool condition, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			if (!condition)
			{
				throw new AssertException(string.Format("arg0:{0} arg1:{1} arg2:{2} arg3:{3}", new object[]
				{
					arg0,
					arg1,
					arg2,
					arg3
				}));
			}
		}
	}
}
