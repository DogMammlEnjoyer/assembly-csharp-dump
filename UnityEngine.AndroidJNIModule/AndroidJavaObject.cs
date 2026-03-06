using System;
using System.Text;

namespace UnityEngine
{
	public class AndroidJavaObject : IDisposable
	{
		public AndroidJavaObject(string className, string[] args) : this()
		{
			this._AndroidJavaObject(className, new object[]
			{
				args
			});
		}

		public AndroidJavaObject(string className, AndroidJavaObject[] args) : this()
		{
			this._AndroidJavaObject(className, new object[]
			{
				args
			});
		}

		public AndroidJavaObject(string className, AndroidJavaClass[] args) : this()
		{
			this._AndroidJavaObject(className, new object[]
			{
				args
			});
		}

		public AndroidJavaObject(string className, AndroidJavaProxy[] args) : this()
		{
			this._AndroidJavaObject(className, new object[]
			{
				args
			});
		}

		public AndroidJavaObject(string className, AndroidJavaRunnable[] args) : this()
		{
			this._AndroidJavaObject(className, new object[]
			{
				args
			});
		}

		public AndroidJavaObject(string className, params object[] args) : this()
		{
			this._AndroidJavaObject(className, args);
		}

		public AndroidJavaObject(IntPtr jobject) : this()
		{
			bool flag = jobject == IntPtr.Zero;
			if (flag)
			{
				throw new Exception("JNI: Init'd AndroidJavaObject with null ptr!");
			}
			IntPtr objectClass = AndroidJNISafe.GetObjectClass(jobject);
			this.m_jobject = new GlobalJavaObjectRef(jobject);
			this.m_jclass = new GlobalJavaObjectRef(objectClass);
			AndroidJNISafe.DeleteLocalRef(objectClass);
		}

		public AndroidJavaObject(IntPtr clazz, IntPtr constructorID, params object[] args)
		{
			this.m_jclass = new GlobalJavaObjectRef(clazz);
			this._AndroidJavaObject(constructorID, args);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Call<T>(string methodName, T[] args)
		{
			this._Call(methodName, new object[]
			{
				args
			});
		}

		public void Call<T>(IntPtr methodID, T[] args)
		{
			this._Call(methodID, new object[]
			{
				args
			});
		}

		public void Call(string methodName, params object[] args)
		{
			this._Call(methodName, args);
		}

		public void Call(IntPtr methodID, params object[] args)
		{
			this._Call(methodID, args);
		}

		public void CallStatic<T>(string methodName, T[] args)
		{
			this._CallStatic(methodName, new object[]
			{
				args
			});
		}

		public void CallStatic<T>(IntPtr methodID, T[] args)
		{
			this._CallStatic(methodID, new object[]
			{
				args
			});
		}

		public void CallStatic(string methodName, params object[] args)
		{
			this._CallStatic(methodName, args);
		}

		public void CallStatic(IntPtr methodID, params object[] args)
		{
			this._CallStatic(methodID, args);
		}

		public FieldType Get<FieldType>(string fieldName)
		{
			return this._Get<FieldType>(fieldName);
		}

		public FieldType Get<FieldType>(IntPtr fieldID)
		{
			return this._Get<FieldType>(fieldID);
		}

		public void Set<FieldType>(string fieldName, FieldType val)
		{
			this._Set<FieldType>(fieldName, val);
		}

		public void Set<FieldType>(IntPtr fieldID, FieldType val)
		{
			this._Set<FieldType>(fieldID, val);
		}

		public FieldType GetStatic<FieldType>(string fieldName)
		{
			return this._GetStatic<FieldType>(fieldName);
		}

		public FieldType GetStatic<FieldType>(IntPtr fieldID)
		{
			return this._GetStatic<FieldType>(fieldID);
		}

		public void SetStatic<FieldType>(string fieldName, FieldType val)
		{
			this._SetStatic<FieldType>(fieldName, val);
		}

		public void SetStatic<FieldType>(IntPtr fieldID, FieldType val)
		{
			this._SetStatic<FieldType>(fieldID, val);
		}

		public IntPtr GetRawObject()
		{
			return this._GetRawObject();
		}

		public IntPtr GetRawClass()
		{
			return this._GetRawClass();
		}

		public AndroidJavaObject CloneReference()
		{
			bool flag = this.m_jclass == null;
			if (flag)
			{
				throw new Exception("Cannot clone a disposed reference");
			}
			bool flag2 = this.m_jobject != null;
			AndroidJavaObject result;
			if (flag2)
			{
				result = new AndroidJavaObject
				{
					m_jobject = new GlobalJavaObjectRef(this.m_jobject),
					m_jclass = new GlobalJavaObjectRef(this.m_jclass)
				};
			}
			else
			{
				result = new AndroidJavaClass(this.m_jclass);
			}
			return result;
		}

		public ReturnType Call<ReturnType, T>(string methodName, T[] args)
		{
			return this._Call<ReturnType>(methodName, new object[]
			{
				args
			});
		}

		public ReturnType Call<ReturnType, T>(IntPtr methodID, T[] args)
		{
			return this._Call<ReturnType>(methodID, new object[]
			{
				args
			});
		}

		public ReturnType Call<ReturnType>(string methodName, params object[] args)
		{
			return this._Call<ReturnType>(methodName, args);
		}

		public ReturnType Call<ReturnType>(IntPtr methodID, params object[] args)
		{
			return this._Call<ReturnType>(methodID, args);
		}

		public ReturnType CallStatic<ReturnType, T>(string methodName, T[] args)
		{
			return this._CallStatic<ReturnType>(methodName, new object[]
			{
				args
			});
		}

		public ReturnType CallStatic<ReturnType, T>(IntPtr methodID, T[] args)
		{
			return this._CallStatic<ReturnType>(methodID, new object[]
			{
				args
			});
		}

		public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
		{
			return this._CallStatic<ReturnType>(methodName, args);
		}

		public ReturnType CallStatic<ReturnType>(IntPtr methodID, params object[] args)
		{
			return this._CallStatic<ReturnType>(methodID, args);
		}

		protected void DebugPrint(string msg)
		{
			bool flag = !AndroidJavaObject.enableDebugPrints;
			if (!flag)
			{
				Debug.Log(msg);
			}
		}

		protected void DebugPrint(string call, string methodName, string signature, object[] args)
		{
			bool flag = !AndroidJavaObject.enableDebugPrints;
			if (!flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in args)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append((obj == null) ? "<null>" : obj.GetType().ToString());
				}
				string[] array = new string[7];
				array[0] = call;
				array[1] = "(\"";
				array[2] = methodName;
				array[3] = "\"";
				int num = 4;
				StringBuilder stringBuilder2 = stringBuilder;
				array[num] = ((stringBuilder2 != null) ? stringBuilder2.ToString() : null);
				array[5] = ") = ";
				array[6] = signature;
				Debug.Log(string.Concat(array));
			}
		}

		private void _AndroidJavaObject(string className, params object[] args)
		{
			this.DebugPrint("Creating AndroidJavaObject from " + className);
			IntPtr intPtr = AndroidJNISafe.FindClass(className.Replace('.', '/'));
			this.m_jclass = new GlobalJavaObjectRef(intPtr);
			AndroidJNISafe.DeleteLocalRef(intPtr);
			IntPtr constructorID = AndroidJNIHelper.GetConstructorID(this.m_jclass, args);
			this._AndroidJavaObject(constructorID, args);
		}

		private unsafe void _AndroidJavaObject(IntPtr constructorID, params object[] args)
		{
			Span<jvalue> span;
			if (args == null || args.Length == 0)
			{
				span = default(Span<jvalue>);
			}
			else
			{
				int num = args.Length;
				Span<jvalue> span2 = new Span<jvalue>(stackalloc byte[checked(unchecked((UIntPtr)num) * (UIntPtr)sizeof(jvalue))], num);
				span = span2;
			}
			Span<jvalue> span3 = span;
			AndroidJNIHelper.CreateJNIArgArray(args, span3);
			try
			{
				IntPtr intPtr = AndroidJNISafe.NewObject(this.m_jclass, constructorID, span3);
				this.m_jobject = new GlobalJavaObjectRef(intPtr);
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, span3);
			}
		}

		internal AndroidJavaObject()
		{
		}

		~AndroidJavaObject()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool flag = this.m_jobject != null;
			if (flag)
			{
				this.m_jobject.Dispose();
				this.m_jobject = null;
			}
			bool flag2 = this.m_jclass != null;
			if (flag2)
			{
				this.m_jclass.Dispose();
				this.m_jclass = null;
			}
		}

		protected void _Call(string methodName, params object[] args)
		{
			IntPtr methodID = AndroidJNIHelper.GetMethodID(this.m_jclass, methodName, args, false);
			this._Call(methodID, args);
		}

		protected unsafe void _Call(IntPtr methodID, params object[] args)
		{
			Span<jvalue> span;
			if (args == null || args.Length == 0)
			{
				span = default(Span<jvalue>);
			}
			else
			{
				int num = args.Length;
				Span<jvalue> span2 = new Span<jvalue>(stackalloc byte[checked(unchecked((UIntPtr)num) * (UIntPtr)sizeof(jvalue))], num);
				span = span2;
			}
			Span<jvalue> span3 = span;
			bool flag = span3.Length > 0;
			if (flag)
			{
				AndroidJNISafe.PushLocalFrame(span3.Length);
				AndroidJNIHelper.CreateJNIArgArray(args, span3);
			}
			try
			{
				AndroidJNISafe.CallVoidMethod(this.m_jobject, methodID, span3);
			}
			finally
			{
				bool flag2 = span3.Length > 0;
				if (flag2)
				{
					AndroidJNI.PopLocalFrame(IntPtr.Zero);
				}
			}
		}

		protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
		{
			IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(this.m_jclass, methodName, args, false);
			return this._Call<ReturnType>(methodID, args);
		}

		protected unsafe ReturnType _Call<ReturnType>(IntPtr methodID, params object[] args)
		{
			Span<jvalue> span;
			if (args == null || args.Length == 0)
			{
				span = default(Span<jvalue>);
			}
			else
			{
				int num = args.Length;
				Span<jvalue> span2 = new Span<jvalue>(stackalloc byte[checked(unchecked((UIntPtr)num) * (UIntPtr)sizeof(jvalue))], num);
				span = span2;
			}
			Span<jvalue> span3 = span;
			AndroidJNI.PushLocalFrame(span3.Length + 1);
			AndroidJNIHelper.CreateJNIArgArray(args, span3);
			ReturnType result;
			try
			{
				bool flag = AndroidReflection.IsPrimitive(typeof(ReturnType));
				if (flag)
				{
					bool flag2 = typeof(ReturnType) == typeof(int);
					if (flag2)
					{
						result = (ReturnType)((object)AndroidJNISafe.CallIntMethod(this.m_jobject, methodID, span3));
					}
					else
					{
						bool flag3 = typeof(ReturnType) == typeof(bool);
						if (flag3)
						{
							result = (ReturnType)((object)AndroidJNISafe.CallBooleanMethod(this.m_jobject, methodID, span3));
						}
						else
						{
							bool flag4 = typeof(ReturnType) == typeof(byte);
							if (flag4)
							{
								Debug.LogWarning("Return type <Byte> for Java method call is obsolete, use return type <SByte> instead");
								result = (ReturnType)((object)((byte)AndroidJNISafe.CallSByteMethod(this.m_jobject, methodID, span3)));
							}
							else
							{
								bool flag5 = typeof(ReturnType) == typeof(sbyte);
								if (flag5)
								{
									result = (ReturnType)((object)AndroidJNISafe.CallSByteMethod(this.m_jobject, methodID, span3));
								}
								else
								{
									bool flag6 = typeof(ReturnType) == typeof(short);
									if (flag6)
									{
										result = (ReturnType)((object)AndroidJNISafe.CallShortMethod(this.m_jobject, methodID, span3));
									}
									else
									{
										bool flag7 = typeof(ReturnType) == typeof(long);
										if (flag7)
										{
											result = (ReturnType)((object)AndroidJNISafe.CallLongMethod(this.m_jobject, methodID, span3));
										}
										else
										{
											bool flag8 = typeof(ReturnType) == typeof(float);
											if (flag8)
											{
												result = (ReturnType)((object)AndroidJNISafe.CallFloatMethod(this.m_jobject, methodID, span3));
											}
											else
											{
												bool flag9 = typeof(ReturnType) == typeof(double);
												if (flag9)
												{
													result = (ReturnType)((object)AndroidJNISafe.CallDoubleMethod(this.m_jobject, methodID, span3));
												}
												else
												{
													bool flag10 = typeof(ReturnType) == typeof(char);
													if (flag10)
													{
														result = (ReturnType)((object)AndroidJNISafe.CallCharMethod(this.m_jobject, methodID, span3));
													}
													else
													{
														result = default(ReturnType);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag11 = typeof(ReturnType) == typeof(string);
					if (flag11)
					{
						result = (ReturnType)((object)AndroidJNISafe.CallStringMethod(this.m_jobject, methodID, span3));
					}
					else
					{
						bool flag12 = typeof(ReturnType) == typeof(AndroidJavaClass);
						if (flag12)
						{
							IntPtr intPtr = AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, span3);
							result = ((intPtr == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)((object)new AndroidJavaClass(intPtr))));
						}
						else
						{
							bool flag13 = typeof(ReturnType) == typeof(AndroidJavaObject);
							if (flag13)
							{
								IntPtr intPtr2 = AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, span3);
								result = ((intPtr2 == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)((object)new AndroidJavaObject(intPtr2))));
							}
							else
							{
								bool flag14 = AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType));
								if (!flag14)
								{
									string str = "JNI: Unknown return type '";
									Type typeFromHandle = typeof(ReturnType);
									throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "'");
								}
								IntPtr jobject = AndroidJNISafe.CallObjectMethod(this.m_jobject, methodID, span3);
								result = AndroidJavaObject.FromJavaArray<ReturnType>(jobject);
							}
						}
					}
				}
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
			return result;
		}

		protected FieldType _Get<FieldType>(string fieldName)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, false);
			return this._Get<FieldType>(fieldID);
		}

		protected FieldType _Get<FieldType>(IntPtr fieldID)
		{
			bool flag = AndroidReflection.IsPrimitive(typeof(FieldType));
			FieldType result;
			if (flag)
			{
				bool flag2 = typeof(FieldType) == typeof(int);
				if (flag2)
				{
					result = (FieldType)((object)AndroidJNISafe.GetIntField(this.m_jobject, fieldID));
				}
				else
				{
					bool flag3 = typeof(FieldType) == typeof(bool);
					if (flag3)
					{
						result = (FieldType)((object)AndroidJNISafe.GetBooleanField(this.m_jobject, fieldID));
					}
					else
					{
						bool flag4 = typeof(FieldType) == typeof(byte);
						if (flag4)
						{
							Debug.LogWarning("Field type <Byte> for Java get field call is obsolete, use field type <SByte> instead");
							result = (FieldType)((object)((byte)AndroidJNISafe.GetSByteField(this.m_jobject, fieldID)));
						}
						else
						{
							bool flag5 = typeof(FieldType) == typeof(sbyte);
							if (flag5)
							{
								result = (FieldType)((object)AndroidJNISafe.GetSByteField(this.m_jobject, fieldID));
							}
							else
							{
								bool flag6 = typeof(FieldType) == typeof(short);
								if (flag6)
								{
									result = (FieldType)((object)AndroidJNISafe.GetShortField(this.m_jobject, fieldID));
								}
								else
								{
									bool flag7 = typeof(FieldType) == typeof(long);
									if (flag7)
									{
										result = (FieldType)((object)AndroidJNISafe.GetLongField(this.m_jobject, fieldID));
									}
									else
									{
										bool flag8 = typeof(FieldType) == typeof(float);
										if (flag8)
										{
											result = (FieldType)((object)AndroidJNISafe.GetFloatField(this.m_jobject, fieldID));
										}
										else
										{
											bool flag9 = typeof(FieldType) == typeof(double);
											if (flag9)
											{
												result = (FieldType)((object)AndroidJNISafe.GetDoubleField(this.m_jobject, fieldID));
											}
											else
											{
												bool flag10 = typeof(FieldType) == typeof(char);
												if (flag10)
												{
													result = (FieldType)((object)AndroidJNISafe.GetCharField(this.m_jobject, fieldID));
												}
												else
												{
													result = default(FieldType);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag11 = typeof(FieldType) == typeof(string);
				if (flag11)
				{
					result = (FieldType)((object)AndroidJNISafe.GetStringField(this.m_jobject, fieldID));
				}
				else
				{
					bool flag12 = typeof(FieldType) == typeof(AndroidJavaClass);
					if (flag12)
					{
						IntPtr objectField = AndroidJNISafe.GetObjectField(this.m_jobject, fieldID);
						result = ((objectField == IntPtr.Zero) ? default(FieldType) : ((FieldType)((object)AndroidJavaObject.AndroidJavaClassDeleteLocalRef(objectField))));
					}
					else
					{
						bool flag13 = typeof(FieldType) == typeof(AndroidJavaObject);
						if (flag13)
						{
							IntPtr objectField2 = AndroidJNISafe.GetObjectField(this.m_jobject, fieldID);
							result = ((objectField2 == IntPtr.Zero) ? default(FieldType) : ((FieldType)((object)AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(objectField2))));
						}
						else
						{
							bool flag14 = AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType));
							if (!flag14)
							{
								string str = "JNI: Unknown field type '";
								Type typeFromHandle = typeof(FieldType);
								throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "'");
							}
							IntPtr objectField3 = AndroidJNISafe.GetObjectField(this.m_jobject, fieldID);
							result = AndroidJavaObject.FromJavaArrayDeleteLocalRef<FieldType>(objectField3);
						}
					}
				}
			}
			return result;
		}

		protected void _Set<FieldType>(string fieldName, FieldType val)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, false);
			this._Set<FieldType>(fieldID, val);
		}

		protected void _Set<FieldType>(IntPtr fieldID, FieldType val)
		{
			bool flag = AndroidReflection.IsPrimitive(typeof(FieldType));
			if (flag)
			{
				bool flag2 = typeof(FieldType) == typeof(int);
				if (flag2)
				{
					AndroidJNISafe.SetIntField(this.m_jobject, fieldID, (int)((object)val));
				}
				else
				{
					bool flag3 = typeof(FieldType) == typeof(bool);
					if (flag3)
					{
						AndroidJNISafe.SetBooleanField(this.m_jobject, fieldID, (bool)((object)val));
					}
					else
					{
						bool flag4 = typeof(FieldType) == typeof(byte);
						if (flag4)
						{
							Debug.LogWarning("Field type <Byte> for Java set field call is obsolete, use field type <SByte> instead");
							AndroidJNISafe.SetSByteField(this.m_jobject, fieldID, (sbyte)((byte)((object)val)));
						}
						else
						{
							bool flag5 = typeof(FieldType) == typeof(sbyte);
							if (flag5)
							{
								AndroidJNISafe.SetSByteField(this.m_jobject, fieldID, (sbyte)((object)val));
							}
							else
							{
								bool flag6 = typeof(FieldType) == typeof(short);
								if (flag6)
								{
									AndroidJNISafe.SetShortField(this.m_jobject, fieldID, (short)((object)val));
								}
								else
								{
									bool flag7 = typeof(FieldType) == typeof(long);
									if (flag7)
									{
										AndroidJNISafe.SetLongField(this.m_jobject, fieldID, (long)((object)val));
									}
									else
									{
										bool flag8 = typeof(FieldType) == typeof(float);
										if (flag8)
										{
											AndroidJNISafe.SetFloatField(this.m_jobject, fieldID, (float)((object)val));
										}
										else
										{
											bool flag9 = typeof(FieldType) == typeof(double);
											if (flag9)
											{
												AndroidJNISafe.SetDoubleField(this.m_jobject, fieldID, (double)((object)val));
											}
											else
											{
												bool flag10 = typeof(FieldType) == typeof(char);
												if (flag10)
												{
													AndroidJNISafe.SetCharField(this.m_jobject, fieldID, (char)((object)val));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag11 = typeof(FieldType) == typeof(string);
				if (flag11)
				{
					AndroidJNISafe.SetStringField(this.m_jobject, fieldID, (string)((object)val));
				}
				else
				{
					bool flag12 = typeof(FieldType) == typeof(AndroidJavaClass);
					if (flag12)
					{
						AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, (val == null) ? IntPtr.Zero : ((AndroidJavaClass)((object)val)).m_jclass);
					}
					else
					{
						bool flag13 = typeof(FieldType) == typeof(AndroidJavaObject);
						if (flag13)
						{
							AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, (val == null) ? IntPtr.Zero : ((AndroidJavaObject)((object)val)).m_jobject);
						}
						else
						{
							bool flag14 = AndroidReflection.IsAssignableFrom(typeof(AndroidJavaProxy), typeof(FieldType));
							if (flag14)
							{
								AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, (val == null) ? IntPtr.Zero : ((AndroidJavaProxy)((object)val)).GetRawProxy());
							}
							else
							{
								bool flag15 = AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType));
								if (!flag15)
								{
									string str = "JNI: Unknown field type '";
									Type typeFromHandle = typeof(FieldType);
									throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "'");
								}
								IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)((object)val));
								AndroidJNISafe.SetObjectField(this.m_jobject, fieldID, val2);
							}
						}
					}
				}
			}
		}

		protected void _CallStatic(string methodName, params object[] args)
		{
			IntPtr methodID = AndroidJNIHelper.GetMethodID(this.m_jclass, methodName, args, true);
			this._CallStatic(methodID, args);
		}

		protected unsafe void _CallStatic(IntPtr methodID, params object[] args)
		{
			Span<jvalue> span;
			if (args == null || args.Length == 0)
			{
				span = default(Span<jvalue>);
			}
			else
			{
				int num = args.Length;
				Span<jvalue> span2 = new Span<jvalue>(stackalloc byte[checked(unchecked((UIntPtr)num) * (UIntPtr)sizeof(jvalue))], num);
				span = span2;
			}
			Span<jvalue> span3 = span;
			bool flag = span3.Length > 0;
			if (flag)
			{
				AndroidJNISafe.PushLocalFrame(span3.Length);
				AndroidJNIHelper.CreateJNIArgArray(args, span3);
			}
			try
			{
				AndroidJNISafe.CallStaticVoidMethod(this.m_jclass, methodID, span3);
			}
			finally
			{
				bool flag2 = span3.Length > 0;
				if (flag2)
				{
					AndroidJNI.PopLocalFrame(IntPtr.Zero);
				}
			}
		}

		protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
		{
			IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(this.m_jclass, methodName, args, true);
			return this._CallStatic<ReturnType>(methodID, args);
		}

		protected unsafe ReturnType _CallStatic<ReturnType>(IntPtr methodID, params object[] args)
		{
			Span<jvalue> span;
			if (args == null || args.Length == 0)
			{
				span = default(Span<jvalue>);
			}
			else
			{
				int num = args.Length;
				Span<jvalue> span2 = new Span<jvalue>(stackalloc byte[checked(unchecked((UIntPtr)num) * (UIntPtr)sizeof(jvalue))], num);
				span = span2;
			}
			Span<jvalue> span3 = span;
			AndroidJNI.PushLocalFrame(span3.Length + 1);
			AndroidJNIHelper.CreateJNIArgArray(args, span3);
			ReturnType result;
			try
			{
				bool flag = AndroidReflection.IsPrimitive(typeof(ReturnType));
				if (flag)
				{
					bool flag2 = typeof(ReturnType) == typeof(int);
					if (flag2)
					{
						result = (ReturnType)((object)AndroidJNISafe.CallStaticIntMethod(this.m_jclass, methodID, span3));
					}
					else
					{
						bool flag3 = typeof(ReturnType) == typeof(bool);
						if (flag3)
						{
							result = (ReturnType)((object)AndroidJNISafe.CallStaticBooleanMethod(this.m_jclass, methodID, span3));
						}
						else
						{
							bool flag4 = typeof(ReturnType) == typeof(byte);
							if (flag4)
							{
								Debug.LogWarning("Return type <Byte> for Java method call is obsolete, use return type <SByte> instead");
								result = (ReturnType)((object)((byte)AndroidJNISafe.CallStaticSByteMethod(this.m_jclass, methodID, span3)));
							}
							else
							{
								bool flag5 = typeof(ReturnType) == typeof(sbyte);
								if (flag5)
								{
									result = (ReturnType)((object)AndroidJNISafe.CallStaticSByteMethod(this.m_jclass, methodID, span3));
								}
								else
								{
									bool flag6 = typeof(ReturnType) == typeof(short);
									if (flag6)
									{
										result = (ReturnType)((object)AndroidJNISafe.CallStaticShortMethod(this.m_jclass, methodID, span3));
									}
									else
									{
										bool flag7 = typeof(ReturnType) == typeof(long);
										if (flag7)
										{
											result = (ReturnType)((object)AndroidJNISafe.CallStaticLongMethod(this.m_jclass, methodID, span3));
										}
										else
										{
											bool flag8 = typeof(ReturnType) == typeof(float);
											if (flag8)
											{
												result = (ReturnType)((object)AndroidJNISafe.CallStaticFloatMethod(this.m_jclass, methodID, span3));
											}
											else
											{
												bool flag9 = typeof(ReturnType) == typeof(double);
												if (flag9)
												{
													result = (ReturnType)((object)AndroidJNISafe.CallStaticDoubleMethod(this.m_jclass, methodID, span3));
												}
												else
												{
													bool flag10 = typeof(ReturnType) == typeof(char);
													if (flag10)
													{
														result = (ReturnType)((object)AndroidJNISafe.CallStaticCharMethod(this.m_jclass, methodID, span3));
													}
													else
													{
														result = default(ReturnType);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag11 = typeof(ReturnType) == typeof(string);
					if (flag11)
					{
						result = (ReturnType)((object)AndroidJNISafe.CallStaticStringMethod(this.m_jclass, methodID, span3));
					}
					else
					{
						bool flag12 = typeof(ReturnType) == typeof(AndroidJavaClass);
						if (flag12)
						{
							IntPtr intPtr = AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, span3);
							result = ((intPtr == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)((object)new AndroidJavaClass(intPtr))));
						}
						else
						{
							bool flag13 = typeof(ReturnType) == typeof(AndroidJavaObject);
							if (flag13)
							{
								IntPtr intPtr2 = AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, span3);
								result = ((intPtr2 == IntPtr.Zero) ? default(ReturnType) : ((ReturnType)((object)new AndroidJavaObject(intPtr2))));
							}
							else
							{
								bool flag14 = AndroidReflection.IsAssignableFrom(typeof(Array), typeof(ReturnType));
								if (!flag14)
								{
									string str = "JNI: Unknown return type '";
									Type typeFromHandle = typeof(ReturnType);
									throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "'");
								}
								IntPtr jobject = AndroidJNISafe.CallStaticObjectMethod(this.m_jclass, methodID, span3);
								result = AndroidJavaObject.FromJavaArray<ReturnType>(jobject);
							}
						}
					}
				}
			}
			finally
			{
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
			}
			return result;
		}

		protected FieldType _GetStatic<FieldType>(string fieldName)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, true);
			return this._GetStatic<FieldType>(fieldID);
		}

		protected FieldType _GetStatic<FieldType>(IntPtr fieldID)
		{
			bool flag = AndroidReflection.IsPrimitive(typeof(FieldType));
			FieldType result;
			if (flag)
			{
				bool flag2 = typeof(FieldType) == typeof(int);
				if (flag2)
				{
					result = (FieldType)((object)AndroidJNISafe.GetStaticIntField(this.m_jclass, fieldID));
				}
				else
				{
					bool flag3 = typeof(FieldType) == typeof(bool);
					if (flag3)
					{
						result = (FieldType)((object)AndroidJNISafe.GetStaticBooleanField(this.m_jclass, fieldID));
					}
					else
					{
						bool flag4 = typeof(FieldType) == typeof(byte);
						if (flag4)
						{
							Debug.LogWarning("Field type <Byte> for Java get field call is obsolete, use field type <SByte> instead");
							result = (FieldType)((object)((byte)AndroidJNISafe.GetStaticSByteField(this.m_jclass, fieldID)));
						}
						else
						{
							bool flag5 = typeof(FieldType) == typeof(sbyte);
							if (flag5)
							{
								result = (FieldType)((object)AndroidJNISafe.GetStaticSByteField(this.m_jclass, fieldID));
							}
							else
							{
								bool flag6 = typeof(FieldType) == typeof(short);
								if (flag6)
								{
									result = (FieldType)((object)AndroidJNISafe.GetStaticShortField(this.m_jclass, fieldID));
								}
								else
								{
									bool flag7 = typeof(FieldType) == typeof(long);
									if (flag7)
									{
										result = (FieldType)((object)AndroidJNISafe.GetStaticLongField(this.m_jclass, fieldID));
									}
									else
									{
										bool flag8 = typeof(FieldType) == typeof(float);
										if (flag8)
										{
											result = (FieldType)((object)AndroidJNISafe.GetStaticFloatField(this.m_jclass, fieldID));
										}
										else
										{
											bool flag9 = typeof(FieldType) == typeof(double);
											if (flag9)
											{
												result = (FieldType)((object)AndroidJNISafe.GetStaticDoubleField(this.m_jclass, fieldID));
											}
											else
											{
												bool flag10 = typeof(FieldType) == typeof(char);
												if (flag10)
												{
													result = (FieldType)((object)AndroidJNISafe.GetStaticCharField(this.m_jclass, fieldID));
												}
												else
												{
													result = default(FieldType);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag11 = typeof(FieldType) == typeof(string);
				if (flag11)
				{
					result = (FieldType)((object)AndroidJNISafe.GetStaticStringField(this.m_jclass, fieldID));
				}
				else
				{
					bool flag12 = typeof(FieldType) == typeof(AndroidJavaClass);
					if (flag12)
					{
						IntPtr staticObjectField = AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID);
						result = ((staticObjectField == IntPtr.Zero) ? default(FieldType) : ((FieldType)((object)AndroidJavaObject.AndroidJavaClassDeleteLocalRef(staticObjectField))));
					}
					else
					{
						bool flag13 = typeof(FieldType) == typeof(AndroidJavaObject);
						if (flag13)
						{
							IntPtr staticObjectField2 = AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID);
							result = ((staticObjectField2 == IntPtr.Zero) ? default(FieldType) : ((FieldType)((object)AndroidJavaObject.AndroidJavaObjectDeleteLocalRef(staticObjectField2))));
						}
						else
						{
							bool flag14 = AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType));
							if (!flag14)
							{
								string str = "JNI: Unknown field type '";
								Type typeFromHandle = typeof(FieldType);
								throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "'");
							}
							IntPtr staticObjectField3 = AndroidJNISafe.GetStaticObjectField(this.m_jclass, fieldID);
							result = AndroidJavaObject.FromJavaArrayDeleteLocalRef<FieldType>(staticObjectField3);
						}
					}
				}
			}
			return result;
		}

		protected void _SetStatic<FieldType>(string fieldName, FieldType val)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(this.m_jclass, fieldName, true);
			this._SetStatic<FieldType>(fieldID, val);
		}

		protected void _SetStatic<FieldType>(IntPtr fieldID, FieldType val)
		{
			bool flag = AndroidReflection.IsPrimitive(typeof(FieldType));
			if (flag)
			{
				bool flag2 = typeof(FieldType) == typeof(int);
				if (flag2)
				{
					AndroidJNISafe.SetStaticIntField(this.m_jclass, fieldID, (int)((object)val));
				}
				else
				{
					bool flag3 = typeof(FieldType) == typeof(bool);
					if (flag3)
					{
						AndroidJNISafe.SetStaticBooleanField(this.m_jclass, fieldID, (bool)((object)val));
					}
					else
					{
						bool flag4 = typeof(FieldType) == typeof(byte);
						if (flag4)
						{
							Debug.LogWarning("Field type <Byte> for Java set field call is obsolete, use field type <SByte> instead");
							AndroidJNISafe.SetStaticSByteField(this.m_jclass, fieldID, (sbyte)((byte)((object)val)));
						}
						else
						{
							bool flag5 = typeof(FieldType) == typeof(sbyte);
							if (flag5)
							{
								AndroidJNISafe.SetStaticSByteField(this.m_jclass, fieldID, (sbyte)((object)val));
							}
							else
							{
								bool flag6 = typeof(FieldType) == typeof(short);
								if (flag6)
								{
									AndroidJNISafe.SetStaticShortField(this.m_jclass, fieldID, (short)((object)val));
								}
								else
								{
									bool flag7 = typeof(FieldType) == typeof(long);
									if (flag7)
									{
										AndroidJNISafe.SetStaticLongField(this.m_jclass, fieldID, (long)((object)val));
									}
									else
									{
										bool flag8 = typeof(FieldType) == typeof(float);
										if (flag8)
										{
											AndroidJNISafe.SetStaticFloatField(this.m_jclass, fieldID, (float)((object)val));
										}
										else
										{
											bool flag9 = typeof(FieldType) == typeof(double);
											if (flag9)
											{
												AndroidJNISafe.SetStaticDoubleField(this.m_jclass, fieldID, (double)((object)val));
											}
											else
											{
												bool flag10 = typeof(FieldType) == typeof(char);
												if (flag10)
												{
													AndroidJNISafe.SetStaticCharField(this.m_jclass, fieldID, (char)((object)val));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag11 = typeof(FieldType) == typeof(string);
				if (flag11)
				{
					AndroidJNISafe.SetStaticStringField(this.m_jclass, fieldID, (string)((object)val));
				}
				else
				{
					bool flag12 = typeof(FieldType) == typeof(AndroidJavaClass);
					if (flag12)
					{
						AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, (val == null) ? IntPtr.Zero : ((AndroidJavaClass)((object)val)).m_jclass);
					}
					else
					{
						bool flag13 = typeof(FieldType) == typeof(AndroidJavaObject);
						if (flag13)
						{
							AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, (val == null) ? IntPtr.Zero : ((AndroidJavaObject)((object)val)).m_jobject);
						}
						else
						{
							bool flag14 = AndroidReflection.IsAssignableFrom(typeof(AndroidJavaProxy), typeof(FieldType));
							if (flag14)
							{
								AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, (val == null) ? IntPtr.Zero : ((AndroidJavaProxy)((object)val)).GetRawProxy());
							}
							else
							{
								bool flag15 = AndroidReflection.IsAssignableFrom(typeof(Array), typeof(FieldType));
								if (!flag15)
								{
									string str = "JNI: Unknown field type '";
									Type typeFromHandle = typeof(FieldType);
									throw new Exception(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "'");
								}
								IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)((object)val));
								AndroidJNISafe.SetStaticObjectField(this.m_jclass, fieldID, val2);
							}
						}
					}
				}
			}
		}

		internal static AndroidJavaObject AndroidJavaObjectDeleteLocalRef(IntPtr jobject)
		{
			AndroidJavaObject result;
			try
			{
				result = new AndroidJavaObject(jobject);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(jobject);
			}
			return result;
		}

		internal static AndroidJavaClass AndroidJavaClassDeleteLocalRef(IntPtr jclass)
		{
			AndroidJavaClass result;
			try
			{
				result = new AndroidJavaClass(jclass);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(jclass);
			}
			return result;
		}

		internal static ReturnType FromJavaArrayDeleteLocalRef<ReturnType>(IntPtr jobject)
		{
			bool flag = jobject == IntPtr.Zero;
			ReturnType result;
			if (flag)
			{
				result = default(ReturnType);
			}
			else
			{
				try
				{
					result = (ReturnType)((object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(jobject));
				}
				finally
				{
					AndroidJNISafe.DeleteLocalRef(jobject);
				}
			}
			return result;
		}

		internal static ReturnType FromJavaArray<ReturnType>(IntPtr jobject)
		{
			bool flag = jobject == IntPtr.Zero;
			ReturnType result;
			if (flag)
			{
				result = default(ReturnType);
			}
			else
			{
				result = (ReturnType)((object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(jobject));
			}
			return result;
		}

		protected IntPtr _GetRawObject()
		{
			return (this.m_jobject == null) ? IntPtr.Zero : this.m_jobject;
		}

		protected IntPtr _GetRawClass()
		{
			return this.m_jclass;
		}

		private static bool enableDebugPrints;

		internal GlobalJavaObjectRef m_jobject;

		internal GlobalJavaObjectRef m_jclass;
	}
}
