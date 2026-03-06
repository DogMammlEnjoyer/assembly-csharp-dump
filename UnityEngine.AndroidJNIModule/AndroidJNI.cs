using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/AndroidJNI/Public/AndroidJNIBindingsHelpers.h")]
	[StaticAccessor("AndroidJNIBindingsHelpers", StaticAccessorType.DoubleColon)]
	[NativeConditional("PLATFORM_ANDROID")]
	public static class AndroidJNI
	{
		[ThreadSafe]
		private static void ReleaseStringChars(AndroidJNI.JStringBinding str)
		{
			AndroidJNI.ReleaseStringChars_Injected(ref str);
		}

		[StaticAccessor("jni", StaticAccessorType.DoubleColon)]
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetJavaVM();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int AttachCurrentThread();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int DetachCurrentThread();

		[RequiredByNativeCode]
		private static void InvokeAction(Action action)
		{
			action();
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void InvokeAttached(Action action);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetVersion();

		[ThreadSafe]
		public unsafe static IntPtr FindClass(string name)
		{
			IntPtr result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = AndroidJNI.FindClass_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr FromReflectedMethod(IntPtr refMethod);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr FromReflectedField(IntPtr refField);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetSuperclass(IntPtr clazz);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsAssignableFrom(IntPtr clazz1, IntPtr clazz2);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int Throw(IntPtr obj);

		[ThreadSafe]
		public unsafe static int ThrowNew(IntPtr clazz, string message)
		{
			int result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = message.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = AndroidJNI.ThrowNew_Injected(clazz, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ExceptionOccurred();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExceptionDescribe();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExceptionClear();

		[ThreadSafe]
		public unsafe static void FatalError(string message)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = message.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AndroidJNI.FatalError_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int PushLocalFrame(int capacity);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr PopLocalFrame(IntPtr ptr);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewGlobalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteGlobalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void QueueDeleteGlobalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern uint GetQueueGlobalRefsCount();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CleanQueueGlobalRefs();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewWeakGlobalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteWeakGlobalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewLocalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeleteLocalRef(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSameObject(IntPtr obj1, IntPtr obj2);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int EnsureLocalCapacity(int capacity);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr AllocObject(IntPtr clazz);

		public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.NewObject(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static IntPtr NewObject(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.NewObjectA(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr NewObjectA(IntPtr clazz, IntPtr methodID, jvalue* args);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetObjectClass(IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsInstanceOf(IntPtr obj, IntPtr clazz);

		[ThreadSafe]
		public unsafe static IntPtr GetMethodID(IntPtr clazz, string name, string sig)
		{
			IntPtr methodID_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = sig.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				methodID_Injected = AndroidJNI.GetMethodID_Injected(clazz, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return methodID_Injected;
		}

		[ThreadSafe]
		public unsafe static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
		{
			IntPtr fieldID_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = sig.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				fieldID_Injected = AndroidJNI.GetFieldID_Injected(clazz, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return fieldID_Injected;
		}

		[ThreadSafe]
		public unsafe static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
		{
			IntPtr staticMethodID_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = sig.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				staticMethodID_Injected = AndroidJNI.GetStaticMethodID_Injected(clazz, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return staticMethodID_Injected;
		}

		[ThreadSafe]
		public unsafe static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
		{
			IntPtr staticFieldID_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = sig.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				staticFieldID_Injected = AndroidJNI.GetStaticFieldID_Injected(clazz, ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
			return staticFieldID_Injected;
		}

		public static IntPtr NewString(string chars)
		{
			return AndroidJNI.NewStringFromStr(chars);
		}

		[ThreadSafe]
		private unsafe static IntPtr NewStringFromStr(string chars)
		{
			IntPtr result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(chars, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = chars.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = AndroidJNI.NewStringFromStr_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[ThreadSafe]
		public unsafe static IntPtr NewString(char[] chars)
		{
			Span<char> span = new Span<char>(chars);
			IntPtr result;
			fixed (char* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				result = AndroidJNI.NewString_Injected(ref managedSpanWrapper);
			}
			return result;
		}

		[ThreadSafe]
		public unsafe static IntPtr NewStringUTF(string bytes)
		{
			IntPtr result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(bytes, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = bytes.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = AndroidJNI.NewStringUTF_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		public static string GetStringChars(IntPtr str)
		{
			string result;
			using (AndroidJNI.JStringBinding stringCharsInternal = AndroidJNI.GetStringCharsInternal(str))
			{
				result = stringCharsInternal.ToString();
			}
			return result;
		}

		[ThreadSafe]
		private static AndroidJNI.JStringBinding GetStringCharsInternal(IntPtr str)
		{
			AndroidJNI.JStringBinding result;
			AndroidJNI.GetStringCharsInternal_Injected(str, out result);
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetStringLength(IntPtr str);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetStringUTFLength(IntPtr str);

		[ThreadSafe]
		public static string GetStringUTFChars(IntPtr str)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				AndroidJNI.GetStringUTFChars_Injected(str, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStringMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static string CallStringMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStringMethodUnsafe(obj, methodID, args2);
			}
		}

		public unsafe static string CallStringMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args)
		{
			string result;
			using (AndroidJNI.JStringBinding jstringBinding = AndroidJNI.CallStringMethodUnsafeInternal(obj, methodID, args))
			{
				result = jstringBinding.ToString();
			}
			return result;
		}

		[ThreadSafe]
		private unsafe static AndroidJNI.JStringBinding CallStringMethodUnsafeInternal(IntPtr obj, IntPtr methodID, jvalue* args)
		{
			AndroidJNI.JStringBinding result;
			AndroidJNI.CallStringMethodUnsafeInternal_Injected(obj, methodID, args, out result);
			return result;
		}

		public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallObjectMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallObjectMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr CallObjectMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallIntMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static int CallIntMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallIntMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern int CallIntMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallBooleanMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallBooleanMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern bool CallBooleanMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallShortMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static short CallShortMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallShortMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern short CallShortMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		[Obsolete("AndroidJNI.CallByteMethod is obsolete. Use AndroidJNI.CallSByteMethod method instead")]
		public static byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return (byte)AndroidJNI.CallSByteMethod(obj, methodID, args);
		}

		public static sbyte CallSByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallSByteMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static sbyte CallSByteMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallSByteMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern sbyte CallSByteMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallCharMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static char CallCharMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallCharMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern char CallCharMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallFloatMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static float CallFloatMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallFloatMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern float CallFloatMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallDoubleMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static double CallDoubleMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallDoubleMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern double CallDoubleMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallLongMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static long CallLongMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallLongMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern long CallLongMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
		{
			AndroidJNI.CallVoidMethod(obj, methodID, new Span<jvalue>(args));
		}

		public unsafe static void CallVoidMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				AndroidJNI.CallVoidMethodUnsafe(obj, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void CallVoidMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

		public static string GetStringField(IntPtr obj, IntPtr fieldID)
		{
			string result;
			using (AndroidJNI.JStringBinding stringFieldInternal = AndroidJNI.GetStringFieldInternal(obj, fieldID))
			{
				result = stringFieldInternal.ToString();
			}
			return result;
		}

		[ThreadSafe]
		private static AndroidJNI.JStringBinding GetStringFieldInternal(IntPtr obj, IntPtr fieldID)
		{
			AndroidJNI.JStringBinding result;
			AndroidJNI.GetStringFieldInternal_Injected(obj, fieldID, out result);
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetObjectField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetBooleanField(IntPtr obj, IntPtr fieldID);

		[Obsolete("AndroidJNI.GetByteField is obsolete. Use AndroidJNI.GetSByteField method instead")]
		public static byte GetByteField(IntPtr obj, IntPtr fieldID)
		{
			return (byte)AndroidJNI.GetSByteField(obj, fieldID);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern sbyte GetSByteField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern char GetCharField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short GetShortField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIntField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetLongField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloatField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double GetDoubleField(IntPtr obj, IntPtr fieldID);

		[ThreadSafe]
		public unsafe static void SetStringField(IntPtr obj, IntPtr fieldID, string val)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = val.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AndroidJNI.SetStringField_Injected(obj, fieldID, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val);

		[Obsolete("AndroidJNI.SetByteField is obsolete. Use AndroidJNI.SetSByteField method instead")]
		public static void SetByteField(IntPtr obj, IntPtr fieldID, byte val)
		{
			AndroidJNI.SetSByteField(obj, fieldID, (sbyte)val);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSByteField(IntPtr obj, IntPtr fieldID, sbyte val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCharField(IntPtr obj, IntPtr fieldID, char val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShortField(IntPtr obj, IntPtr fieldID, short val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIntField(IntPtr obj, IntPtr fieldID, int val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLongField(IntPtr obj, IntPtr fieldID, long val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetFloatField(IntPtr obj, IntPtr fieldID, float val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDoubleField(IntPtr obj, IntPtr fieldID, double val);

		public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticStringMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticStringMethodUnsafe(clazz, methodID, args2);
			}
		}

		public unsafe static string CallStaticStringMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args)
		{
			string result;
			using (AndroidJNI.JStringBinding jstringBinding = AndroidJNI.CallStaticStringMethodUnsafeInternal(clazz, methodID, args))
			{
				result = jstringBinding.ToString();
			}
			return result;
		}

		[ThreadSafe]
		private unsafe static AndroidJNI.JStringBinding CallStaticStringMethodUnsafeInternal(IntPtr clazz, IntPtr methodID, jvalue* args)
		{
			AndroidJNI.JStringBinding result;
			AndroidJNI.CallStaticStringMethodUnsafeInternal_Injected(clazz, methodID, args, out result);
			return result;
		}

		public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticObjectMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticObjectMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr CallStaticObjectMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticIntMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticIntMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern int CallStaticIntMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticBooleanMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticBooleanMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern bool CallStaticBooleanMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticShortMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticShortMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern short CallStaticShortMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		[Obsolete("AndroidJNI.CallStaticByteMethod is obsolete. Use AndroidJNI.CallStaticSByteMethod method instead")]
		public static byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return (byte)AndroidJNI.CallStaticSByteMethod(clazz, methodID, args);
		}

		public static sbyte CallStaticSByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticSByteMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static sbyte CallStaticSByteMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticSByteMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern sbyte CallStaticSByteMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticCharMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticCharMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern char CallStaticCharMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticFloatMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticFloatMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern float CallStaticFloatMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticDoubleMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticDoubleMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern double CallStaticDoubleMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			return AndroidJNI.CallStaticLongMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				return AndroidJNI.CallStaticLongMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern long CallStaticLongMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
		{
			AndroidJNI.CallStaticVoidMethod(clazz, methodID, new Span<jvalue>(args));
		}

		public unsafe static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
		{
			fixed (jvalue* pinnableReference = args.GetPinnableReference())
			{
				jvalue* args2 = pinnableReference;
				AndroidJNI.CallStaticVoidMethodUnsafe(clazz, methodID, args2);
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void CallStaticVoidMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

		public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID)
		{
			string result;
			using (AndroidJNI.JStringBinding staticStringFieldInternal = AndroidJNI.GetStaticStringFieldInternal(clazz, fieldID))
			{
				result = staticStringFieldInternal.ToString();
			}
			return result;
		}

		[ThreadSafe]
		private static AndroidJNI.JStringBinding GetStaticStringFieldInternal(IntPtr clazz, IntPtr fieldID)
		{
			AndroidJNI.JStringBinding result;
			AndroidJNI.GetStaticStringFieldInternal_Injected(clazz, fieldID, out result);
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID);

		[Obsolete("AndroidJNI.GetStaticByteField is obsolete. Use AndroidJNI.GetStaticSByteField method instead")]
		public static byte GetStaticByteField(IntPtr clazz, IntPtr fieldID)
		{
			return (byte)AndroidJNI.GetStaticSByteField(clazz, fieldID);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern sbyte GetStaticSByteField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern char GetStaticCharField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short GetStaticShortField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetStaticIntField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetStaticLongField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetStaticFloatField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID);

		[ThreadSafe]
		public unsafe static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = val.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AndroidJNI.SetStaticStringField_Injected(clazz, fieldID, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val);

		[Obsolete("AndroidJNI.SetStaticByteField is obsolete. Use AndroidJNI.SetStaticSByteField method instead")]
		public static void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val)
		{
			AndroidJNI.SetStaticSByteField(clazz, fieldID, (sbyte)val);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticSByteField(IntPtr clazz, IntPtr fieldID, sbyte val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val);

		[ThreadSafe]
		private unsafe static IntPtr ConvertToBooleanArray(bool[] array)
		{
			Span<bool> span = new Span<bool>(array);
			IntPtr result;
			fixed (bool* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				result = AndroidJNI.ConvertToBooleanArray_Injected(ref managedSpanWrapper);
			}
			return result;
		}

		public static IntPtr ToBooleanArray(bool[] array)
		{
			return (array == null) ? IntPtr.Zero : AndroidJNI.ConvertToBooleanArray(array);
		}

		[Obsolete("AndroidJNI.ToByteArray is obsolete. Use AndroidJNI.ToSByteArray method instead")]
		[ThreadSafe]
		public unsafe static IntPtr ToByteArray(byte[] array)
		{
			Span<byte> span = new Span<byte>(array);
			IntPtr result;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				result = AndroidJNI.ToByteArray_Injected(ref managedSpanWrapper);
			}
			return result;
		}

		public unsafe static IntPtr ToSByteArray(sbyte[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				sbyte* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToSByteArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToSByteArray(sbyte* array, int length);

		public unsafe static IntPtr ToCharArray(char[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				char* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToCharArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToCharArray(char* array, int length);

		public unsafe static IntPtr ToShortArray(short[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				short* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToShortArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToShortArray(short* array, int length);

		public unsafe static IntPtr ToIntArray(int[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				int* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToIntArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToIntArray(int* array, int length);

		public unsafe static IntPtr ToLongArray(long[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				long* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToLongArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToLongArray(long* array, int length);

		public unsafe static IntPtr ToFloatArray(float[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				float* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToFloatArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToFloatArray(float* array, int length);

		public unsafe static IntPtr ToDoubleArray(double[] array)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				double* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToDoubleArray(array2, array.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToDoubleArray(double* array, int length);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr ToObjectArray(IntPtr* array, int length, IntPtr arrayClass);

		public unsafe static IntPtr ToObjectArray(IntPtr[] array, IntPtr arrayClass)
		{
			bool flag = array == null;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				IntPtr* array2;
				if (array == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				result = AndroidJNI.ToObjectArray(array2, array.Length, arrayClass);
			}
			return result;
		}

		public static IntPtr ToObjectArray(IntPtr[] array)
		{
			return AndroidJNI.ToObjectArray(array, IntPtr.Zero);
		}

		[ThreadSafe]
		public static bool[] FromBooleanArray(IntPtr array)
		{
			bool[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AndroidJNI.FromBooleanArray_Injected(array, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				bool[] array2;
				blittableArrayWrapper.Unmarshal<bool>(ref array2);
				result = array2;
			}
			return result;
		}

		[ThreadSafe]
		[Obsolete("AndroidJNI.FromByteArray is obsolete. Use AndroidJNI.FromSByteArray method instead")]
		public static byte[] FromByteArray(IntPtr array)
		{
			byte[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AndroidJNI.FromByteArray_Injected(array, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				byte[] array2;
				blittableArrayWrapper.Unmarshal<byte>(ref array2);
				result = array2;
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern sbyte[] FromSByteArray(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern char[] FromCharArray(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern short[] FromShortArray(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern int[] FromIntArray(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern long[] FromLongArray(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern float[] FromFloatArray(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern double[] FromDoubleArray(IntPtr array);

		[ThreadSafe]
		public static IntPtr[] FromObjectArray(IntPtr array)
		{
			IntPtr[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				AndroidJNI.FromObjectArray_Injected(array, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				IntPtr[] array2;
				blittableArrayWrapper.Unmarshal<IntPtr>(ref array2);
				result = array2;
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetArrayLength(IntPtr array);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewBooleanArray(int size);

		[Obsolete("AndroidJNI.NewByteArray is obsolete. Use AndroidJNI.NewSByteArray method instead")]
		public static IntPtr NewByteArray(int size)
		{
			return AndroidJNI.NewSByteArray(size);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewSByteArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewCharArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewShortArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewIntArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewLongArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewFloatArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewDoubleArray(int size);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetBooleanArrayElement(IntPtr array, int index);

		[Obsolete("AndroidJNI.GetByteArrayElement is obsolete. Use AndroidJNI.GetSByteArrayElement method instead")]
		public static byte GetByteArrayElement(IntPtr array, int index)
		{
			return (byte)AndroidJNI.GetSByteArrayElement(array, index);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern sbyte GetSByteArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern char GetCharArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short GetShortArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetIntArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetLongArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetFloatArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double GetDoubleArrayElement(IntPtr array, int index);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr GetObjectArrayElement(IntPtr array, int index);

		[Obsolete("AndroidJNI.SetBooleanArrayElement(IntPtr, int, byte) is obsolete. Use AndroidJNI.SetBooleanArrayElement(IntPtr, int, bool) method instead")]
		public static void SetBooleanArrayElement(IntPtr array, int index, byte val)
		{
			AndroidJNI.SetBooleanArrayElement(array, index, val > 0);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetBooleanArrayElement(IntPtr array, int index, bool val);

		[Obsolete("AndroidJNI.SetByteArrayElement is obsolete. Use AndroidJNI.SetSByteArrayElement method instead")]
		public static void SetByteArrayElement(IntPtr array, int index, sbyte val)
		{
			AndroidJNI.SetSByteArrayElement(array, index, val);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetSByteArrayElement(IntPtr array, int index, sbyte val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCharArrayElement(IntPtr array, int index, char val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetShortArrayElement(IntPtr array, int index, short val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetIntArrayElement(IntPtr array, int index, int val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLongArrayElement(IntPtr array, int index, long val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetFloatArrayElement(IntPtr array, int index, float val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDoubleArrayElement(IntPtr array, int index, double val);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetObjectArrayElement(IntPtr array, int index, IntPtr obj);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern IntPtr NewDirectByteBuffer(byte* buffer, long capacity);

		public static IntPtr NewDirectByteBuffer(NativeArray<byte> buffer)
		{
			return AndroidJNI.NewDirectByteBufferFromNativeArray<byte>(buffer);
		}

		public static IntPtr NewDirectByteBuffer(NativeArray<sbyte> buffer)
		{
			return AndroidJNI.NewDirectByteBufferFromNativeArray<sbyte>(buffer);
		}

		private unsafe static IntPtr NewDirectByteBufferFromNativeArray<T>(NativeArray<T> buffer) where T : struct
		{
			bool flag = !buffer.IsCreated || buffer.Length <= 0;
			IntPtr result;
			if (flag)
			{
				result = IntPtr.Zero;
			}
			else
			{
				result = AndroidJNI.NewDirectByteBuffer((byte*)buffer.GetUnsafePtr<T>(), (long)buffer.Length);
			}
			return result;
		}

		public unsafe static sbyte* GetDirectBufferAddress(IntPtr buffer)
		{
			return null;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long GetDirectBufferCapacity(IntPtr buffer);

		private unsafe static NativeArray<T> GetDirectBuffer<T>(IntPtr buffer) where T : struct
		{
			bool flag = buffer == IntPtr.Zero;
			NativeArray<T> result;
			if (flag)
			{
				result = default(NativeArray<T>);
			}
			else
			{
				sbyte* directBufferAddress = AndroidJNI.GetDirectBufferAddress(buffer);
				bool flag2 = directBufferAddress == null;
				if (flag2)
				{
					result = default(NativeArray<T>);
				}
				else
				{
					long directBufferCapacity = AndroidJNI.GetDirectBufferCapacity(buffer);
					bool flag3 = directBufferCapacity > 2147483647L;
					if (flag3)
					{
						throw new Exception(string.Format("Direct buffer is too large ({0}) for NativeArray (max {1})", directBufferCapacity, int.MaxValue));
					}
					result = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)directBufferAddress, (int)directBufferCapacity, Allocator.None);
				}
			}
			return result;
		}

		public static NativeArray<byte> GetDirectByteBuffer(IntPtr buffer)
		{
			return AndroidJNI.GetDirectBuffer<byte>(buffer);
		}

		public static NativeArray<sbyte> GetDirectSByteBuffer(IntPtr buffer)
		{
			return AndroidJNI.GetDirectBuffer<sbyte>(buffer);
		}

		public static int RegisterNatives(IntPtr clazz, JNINativeMethod[] methods)
		{
			bool flag = methods == null || methods.Length == 0;
			int result;
			if (flag)
			{
				result = -1;
			}
			else
			{
				foreach (JNINativeMethod jninativeMethod in methods)
				{
					bool flag2 = string.IsNullOrEmpty(jninativeMethod.name) || string.IsNullOrEmpty(jninativeMethod.signature);
					if (flag2)
					{
						return -1;
					}
				}
				IntPtr natives = AndroidJNI.RegisterNativesAllocate(methods.Length);
				for (int j = 0; j < methods.Length; j++)
				{
					AndroidJNI.RegisterNativesSet(natives, j, methods[j].name, methods[j].signature, methods[j].fnPtr);
				}
				result = AndroidJNI.RegisterNativesAndFree(clazz, natives, methods.Length);
			}
			return result;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr RegisterNativesAllocate(int length);

		[ThreadSafe]
		private unsafe static void RegisterNativesSet(IntPtr natives, int idx, string name, string signature, IntPtr fnPtr)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(signature, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = signature.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				AndroidJNI.RegisterNativesSet_Injected(natives, idx, ref managedSpanWrapper, ref managedSpanWrapper2, fnPtr);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int RegisterNativesAndFree(IntPtr clazz, IntPtr natives, int n);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int UnregisterNatives(IntPtr clazz);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseStringChars_Injected([In] ref AndroidJNI.JStringBinding str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindClass_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ThrowNew_Injected(IntPtr clazz, ref ManagedSpanWrapper message);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FatalError_Injected(ref ManagedSpanWrapper message);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetMethodID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetFieldID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetStaticMethodID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetStaticFieldID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr NewStringFromStr_Injected(ref ManagedSpanWrapper chars);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr NewString_Injected(ref ManagedSpanWrapper chars);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr NewStringUTF_Injected(ref ManagedSpanWrapper bytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStringCharsInternal_Injected(IntPtr str, out AndroidJNI.JStringBinding ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStringUTFChars_Injected(IntPtr str, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void CallStringMethodUnsafeInternal_Injected(IntPtr obj, IntPtr methodID, jvalue* args, out AndroidJNI.JStringBinding ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStringFieldInternal_Injected(IntPtr obj, IntPtr fieldID, out AndroidJNI.JStringBinding ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStringField_Injected(IntPtr obj, IntPtr fieldID, ref ManagedSpanWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void CallStaticStringMethodUnsafeInternal_Injected(IntPtr clazz, IntPtr methodID, jvalue* args, out AndroidJNI.JStringBinding ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStaticStringFieldInternal_Injected(IntPtr clazz, IntPtr fieldID, out AndroidJNI.JStringBinding ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStaticStringField_Injected(IntPtr clazz, IntPtr fieldID, ref ManagedSpanWrapper val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ConvertToBooleanArray_Injected(ref ManagedSpanWrapper array);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ToByteArray_Injected(ref ManagedSpanWrapper array);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FromBooleanArray_Injected(IntPtr array, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FromByteArray_Injected(IntPtr array, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FromObjectArray_Injected(IntPtr array, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RegisterNativesSet_Injected(IntPtr natives, int idx, ref ManagedSpanWrapper name, ref ManagedSpanWrapper signature, IntPtr fnPtr);

		private struct JStringBinding : IDisposable
		{
			public unsafe override string ToString()
			{
				bool flag = this.length == 0;
				string result;
				if (flag)
				{
					result = ((this.chars == IntPtr.Zero) ? null : string.Empty);
				}
				else
				{
					result = new string((char*)((void*)this.chars), 0, this.length);
				}
				return result;
			}

			public void Dispose()
			{
				bool flag = this.length > 0;
				if (flag)
				{
					AndroidJNI.ReleaseStringChars(this);
				}
			}

			private IntPtr javaString;

			private IntPtr chars;

			private int length;

			private bool ownsRef;
		}
	}
}
