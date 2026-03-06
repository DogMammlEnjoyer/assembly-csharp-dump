using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace System.Reflection
{
	internal static class DispatchProxyGenerator
	{
		internal static object CreateProxyInstance(Type baseType, Type interfaceType)
		{
			return Activator.CreateInstance(DispatchProxyGenerator.GetProxyType(baseType, interfaceType), new object[]
			{
				new Action<object[]>(DispatchProxyGenerator.Invoke)
			});
		}

		private static Type GetProxyType(Type baseType, Type interfaceType)
		{
			Dictionary<Type, Dictionary<Type, Type>> obj = DispatchProxyGenerator.s_baseTypeAndInterfaceToGeneratedProxyType;
			Type result;
			lock (obj)
			{
				Dictionary<Type, Type> dictionary = null;
				if (!DispatchProxyGenerator.s_baseTypeAndInterfaceToGeneratedProxyType.TryGetValue(baseType, out dictionary))
				{
					dictionary = new Dictionary<Type, Type>();
					DispatchProxyGenerator.s_baseTypeAndInterfaceToGeneratedProxyType[baseType] = dictionary;
				}
				Type type = null;
				if (!dictionary.TryGetValue(interfaceType, out type))
				{
					type = DispatchProxyGenerator.GenerateProxyType(baseType, interfaceType);
					dictionary[interfaceType] = type;
				}
				result = type;
			}
			return result;
		}

		private static Type GenerateProxyType(Type baseType, Type interfaceType)
		{
			TypeInfo typeInfo = baseType.GetTypeInfo();
			if (!interfaceType.GetTypeInfo().IsInterface)
			{
				throw new ArgumentException(SR.Format("The type '{0}' must be an interface, not a class.", interfaceType.FullName), "T");
			}
			if (typeInfo.IsSealed)
			{
				throw new ArgumentException(SR.Format("The base type '{0}' cannot be sealed.", typeInfo.FullName), "TProxy");
			}
			if (typeInfo.IsAbstract)
			{
				throw new ArgumentException(SR.Format("The base type '{0}' cannot be abstract.", baseType.FullName), "TProxy");
			}
			if (!typeInfo.DeclaredConstructors.Any((ConstructorInfo c) => c.IsPublic && c.GetParameters().Length == 0))
			{
				throw new ArgumentException(SR.Format("The base type '{0}' must have a public parameterless constructor.", baseType.FullName), "TProxy");
			}
			DispatchProxyGenerator.ProxyBuilder proxyBuilder = DispatchProxyGenerator.s_proxyAssembly.CreateProxy("generatedProxy", baseType);
			foreach (Type iface in interfaceType.GetTypeInfo().ImplementedInterfaces)
			{
				proxyBuilder.AddInterfaceImpl(iface);
			}
			proxyBuilder.AddInterfaceImpl(interfaceType);
			return proxyBuilder.CreateType();
		}

		private static void Invoke(object[] args)
		{
			DispatchProxyGenerator.PackedArgs packedArgs = new DispatchProxyGenerator.PackedArgs(args);
			MethodBase methodBase = DispatchProxyGenerator.s_proxyAssembly.ResolveMethodToken(packedArgs.DeclaringType, packedArgs.MethodToken);
			if (methodBase.IsGenericMethodDefinition)
			{
				methodBase = ((MethodInfo)methodBase).MakeGenericMethod(packedArgs.GenericTypes);
			}
			try
			{
				object returnValue = DispatchProxyGenerator.s_dispatchProxyInvokeMethod.Invoke(packedArgs.DispatchProxy, new object[]
				{
					methodBase,
					packedArgs.Args
				});
				packedArgs.ReturnValue = returnValue;
			}
			catch (TargetInvocationException ex)
			{
				ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
			}
		}

		private const int InvokeActionFieldAndCtorParameterIndex = 0;

		private static readonly Dictionary<Type, Dictionary<Type, Type>> s_baseTypeAndInterfaceToGeneratedProxyType = new Dictionary<Type, Dictionary<Type, Type>>();

		private static readonly DispatchProxyGenerator.ProxyAssembly s_proxyAssembly = new DispatchProxyGenerator.ProxyAssembly();

		private static readonly MethodInfo s_dispatchProxyInvokeMethod = typeof(DispatchProxy).GetTypeInfo().GetDeclaredMethod("Invoke");

		private class PackedArgs
		{
			internal PackedArgs() : this(new object[DispatchProxyGenerator.PackedArgs.PackedTypes.Length])
			{
			}

			internal PackedArgs(object[] args)
			{
				this._args = args;
			}

			internal DispatchProxy DispatchProxy
			{
				get
				{
					return (DispatchProxy)this._args[0];
				}
			}

			internal Type DeclaringType
			{
				get
				{
					return (Type)this._args[1];
				}
			}

			internal int MethodToken
			{
				get
				{
					return (int)this._args[2];
				}
			}

			internal object[] Args
			{
				get
				{
					return (object[])this._args[3];
				}
			}

			internal Type[] GenericTypes
			{
				get
				{
					return (Type[])this._args[4];
				}
			}

			internal object ReturnValue
			{
				set
				{
					this._args[5] = value;
				}
			}

			internal const int DispatchProxyPosition = 0;

			internal const int DeclaringTypePosition = 1;

			internal const int MethodTokenPosition = 2;

			internal const int ArgsPosition = 3;

			internal const int GenericTypesPosition = 4;

			internal const int ReturnValuePosition = 5;

			internal static readonly Type[] PackedTypes = new Type[]
			{
				typeof(object),
				typeof(Type),
				typeof(int),
				typeof(object[]),
				typeof(Type[]),
				typeof(object)
			};

			private object[] _args;
		}

		private class ProxyAssembly
		{
			public ProxyAssembly()
			{
				this._ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ProxyBuilder"), AssemblyBuilderAccess.Run);
				this._mb = this._ab.DefineDynamicModule("testmod");
			}

			internal ConstructorInfo IgnoresAccessChecksAttributeConstructor
			{
				get
				{
					if (this._ignoresAccessChecksToAttributeConstructor == null)
					{
						TypeInfo typeInfo = this.GenerateTypeInfoOfIgnoresAccessChecksToAttribute();
						this._ignoresAccessChecksToAttributeConstructor = typeInfo.DeclaredConstructors.Single<ConstructorInfo>();
					}
					return this._ignoresAccessChecksToAttributeConstructor;
				}
			}

			public DispatchProxyGenerator.ProxyBuilder CreateProxy(string name, Type proxyBaseType)
			{
				int num = Interlocked.Increment(ref this._typeId);
				TypeBuilder tb = this._mb.DefineType(name + "_" + num.ToString(), TypeAttributes.Public, proxyBaseType);
				return new DispatchProxyGenerator.ProxyBuilder(this, tb, proxyBaseType);
			}

			private TypeInfo GenerateTypeInfoOfIgnoresAccessChecksToAttribute()
			{
				TypeBuilder typeBuilder = this._mb.DefineType("System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute", TypeAttributes.Public, typeof(Attribute));
				FieldBuilder fieldBuilder = typeBuilder.DefineField("assemblyName", typeof(string), FieldAttributes.Private);
				ILGenerator ilgenerator = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[]
				{
					fieldBuilder.FieldType
				}).GetILGenerator();
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldarg, 1);
				ilgenerator.Emit(OpCodes.Stfld, fieldBuilder);
				ilgenerator.Emit(OpCodes.Ret);
				typeBuilder.DefineProperty("AssemblyName", PropertyAttributes.None, CallingConventions.HasThis, typeof(string), null);
				ILGenerator ilgenerator2 = typeBuilder.DefineMethod("get_AssemblyName", MethodAttributes.Public, CallingConventions.HasThis, typeof(string), null).GetILGenerator();
				ilgenerator2.Emit(OpCodes.Ldarg_0);
				ilgenerator2.Emit(OpCodes.Ldfld, fieldBuilder);
				ilgenerator2.Emit(OpCodes.Ret);
				TypeInfo typeInfo = typeof(AttributeUsageAttribute).GetTypeInfo();
				ConstructorInfo con = typeInfo.DeclaredConstructors.Single((ConstructorInfo c) => c.GetParameters().Count<ParameterInfo>() == 1 && c.GetParameters()[0].ParameterType == typeof(AttributeTargets));
				PropertyInfo propertyInfo = typeInfo.DeclaredProperties.Single((PropertyInfo f) => string.Equals(f.Name, "AllowMultiple"));
				CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(con, new object[]
				{
					AttributeTargets.Assembly
				}, new PropertyInfo[]
				{
					propertyInfo
				}, new object[]
				{
					true
				});
				typeBuilder.SetCustomAttribute(customAttribute);
				return typeBuilder.CreateTypeInfo();
			}

			internal void GenerateInstanceOfIgnoresAccessChecksToAttribute(string assemblyName)
			{
				CustomAttributeBuilder customAttribute = new CustomAttributeBuilder(this.IgnoresAccessChecksAttributeConstructor, new object[]
				{
					assemblyName
				});
				this._ab.SetCustomAttribute(customAttribute);
			}

			internal void EnsureTypeIsVisible(Type type)
			{
				TypeInfo typeInfo = type.GetTypeInfo();
				if (!typeInfo.IsVisible)
				{
					string name = typeInfo.Assembly.GetName().Name;
					if (!this._ignoresAccessAssemblyNames.Contains(name))
					{
						this.GenerateInstanceOfIgnoresAccessChecksToAttribute(name);
						this._ignoresAccessAssemblyNames.Add(name);
					}
				}
			}

			internal void GetTokenForMethod(MethodBase method, out Type type, out int token)
			{
				type = method.DeclaringType;
				token = 0;
				if (!this._methodToToken.TryGetValue(method, out token))
				{
					this._methodsByToken.Add(method);
					token = this._methodsByToken.Count - 1;
					this._methodToToken[method] = token;
				}
			}

			internal MethodBase ResolveMethodToken(Type type, int token)
			{
				return this._methodsByToken[token];
			}

			private AssemblyBuilder _ab;

			private ModuleBuilder _mb;

			private int _typeId;

			private Dictionary<MethodBase, int> _methodToToken = new Dictionary<MethodBase, int>();

			private List<MethodBase> _methodsByToken = new List<MethodBase>();

			private HashSet<string> _ignoresAccessAssemblyNames = new HashSet<string>();

			private ConstructorInfo _ignoresAccessChecksToAttributeConstructor;
		}

		private class ProxyBuilder
		{
			internal ProxyBuilder(DispatchProxyGenerator.ProxyAssembly assembly, TypeBuilder tb, Type proxyBaseType)
			{
				this._assembly = assembly;
				this._tb = tb;
				this._proxyBaseType = proxyBaseType;
				this._fields = new List<FieldBuilder>();
				this._fields.Add(tb.DefineField("invoke", typeof(Action<object[]>), FieldAttributes.Private));
			}

			private void Complete()
			{
				Type[] array = new Type[this._fields.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._fields[i].FieldType;
				}
				ILGenerator ilgenerator = this._tb.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, array).GetILGenerator();
				ConstructorInfo con = this._proxyBaseType.GetTypeInfo().DeclaredConstructors.SingleOrDefault((ConstructorInfo c) => c.IsPublic && c.GetParameters().Length == 0);
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Call, con);
				for (int j = 0; j < array.Length; j++)
				{
					ilgenerator.Emit(OpCodes.Ldarg_0);
					ilgenerator.Emit(OpCodes.Ldarg, j + 1);
					ilgenerator.Emit(OpCodes.Stfld, this._fields[j]);
				}
				ilgenerator.Emit(OpCodes.Ret);
			}

			internal Type CreateType()
			{
				this.Complete();
				return this._tb.CreateTypeInfo().AsType();
			}

			internal void AddInterfaceImpl(Type iface)
			{
				this._assembly.EnsureTypeIsVisible(iface);
				this._tb.AddInterfaceImplementation(iface);
				Dictionary<MethodInfo, DispatchProxyGenerator.ProxyBuilder.PropertyAccessorInfo> dictionary = new Dictionary<MethodInfo, DispatchProxyGenerator.ProxyBuilder.PropertyAccessorInfo>(DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer.Instance);
				foreach (PropertyInfo propertyInfo in iface.GetRuntimeProperties())
				{
					DispatchProxyGenerator.ProxyBuilder.PropertyAccessorInfo value = new DispatchProxyGenerator.ProxyBuilder.PropertyAccessorInfo(propertyInfo.GetMethod, propertyInfo.SetMethod);
					if (propertyInfo.GetMethod != null)
					{
						dictionary[propertyInfo.GetMethod] = value;
					}
					if (propertyInfo.SetMethod != null)
					{
						dictionary[propertyInfo.SetMethod] = value;
					}
				}
				Dictionary<MethodInfo, DispatchProxyGenerator.ProxyBuilder.EventAccessorInfo> dictionary2 = new Dictionary<MethodInfo, DispatchProxyGenerator.ProxyBuilder.EventAccessorInfo>(DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer.Instance);
				foreach (EventInfo eventInfo in iface.GetRuntimeEvents())
				{
					DispatchProxyGenerator.ProxyBuilder.EventAccessorInfo value2 = new DispatchProxyGenerator.ProxyBuilder.EventAccessorInfo(eventInfo.AddMethod, eventInfo.RemoveMethod, eventInfo.RaiseMethod);
					if (eventInfo.AddMethod != null)
					{
						dictionary2[eventInfo.AddMethod] = value2;
					}
					if (eventInfo.RemoveMethod != null)
					{
						dictionary2[eventInfo.RemoveMethod] = value2;
					}
					if (eventInfo.RaiseMethod != null)
					{
						dictionary2[eventInfo.RaiseMethod] = value2;
					}
				}
				foreach (MethodInfo methodInfo in iface.GetRuntimeMethods())
				{
					MethodBuilder methodBuilder = this.AddMethodImpl(methodInfo);
					DispatchProxyGenerator.ProxyBuilder.PropertyAccessorInfo propertyAccessorInfo;
					if (dictionary.TryGetValue(methodInfo, out propertyAccessorInfo))
					{
						if (DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer.Instance.Equals(propertyAccessorInfo.InterfaceGetMethod, methodInfo))
						{
							propertyAccessorInfo.GetMethodBuilder = methodBuilder;
						}
						else
						{
							propertyAccessorInfo.SetMethodBuilder = methodBuilder;
						}
					}
					DispatchProxyGenerator.ProxyBuilder.EventAccessorInfo eventAccessorInfo;
					if (dictionary2.TryGetValue(methodInfo, out eventAccessorInfo))
					{
						if (DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer.Instance.Equals(eventAccessorInfo.InterfaceAddMethod, methodInfo))
						{
							eventAccessorInfo.AddMethodBuilder = methodBuilder;
						}
						else if (DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer.Instance.Equals(eventAccessorInfo.InterfaceRemoveMethod, methodInfo))
						{
							eventAccessorInfo.RemoveMethodBuilder = methodBuilder;
						}
						else
						{
							eventAccessorInfo.RaiseMethodBuilder = methodBuilder;
						}
					}
				}
				foreach (PropertyInfo propertyInfo2 in iface.GetRuntimeProperties())
				{
					DispatchProxyGenerator.ProxyBuilder.PropertyAccessorInfo propertyAccessorInfo2 = dictionary[propertyInfo2.GetMethod ?? propertyInfo2.SetMethod];
					PropertyBuilder propertyBuilder = this._tb.DefineProperty(propertyInfo2.Name, propertyInfo2.Attributes, propertyInfo2.PropertyType, (from p in propertyInfo2.GetIndexParameters()
					select p.ParameterType).ToArray<Type>());
					if (propertyAccessorInfo2.GetMethodBuilder != null)
					{
						propertyBuilder.SetGetMethod(propertyAccessorInfo2.GetMethodBuilder);
					}
					if (propertyAccessorInfo2.SetMethodBuilder != null)
					{
						propertyBuilder.SetSetMethod(propertyAccessorInfo2.SetMethodBuilder);
					}
				}
				foreach (EventInfo eventInfo2 in iface.GetRuntimeEvents())
				{
					DispatchProxyGenerator.ProxyBuilder.EventAccessorInfo eventAccessorInfo2 = dictionary2[eventInfo2.AddMethod ?? eventInfo2.RemoveMethod];
					EventBuilder eventBuilder = this._tb.DefineEvent(eventInfo2.Name, eventInfo2.Attributes, eventInfo2.EventHandlerType);
					if (eventAccessorInfo2.AddMethodBuilder != null)
					{
						eventBuilder.SetAddOnMethod(eventAccessorInfo2.AddMethodBuilder);
					}
					if (eventAccessorInfo2.RemoveMethodBuilder != null)
					{
						eventBuilder.SetRemoveOnMethod(eventAccessorInfo2.RemoveMethodBuilder);
					}
					if (eventAccessorInfo2.RaiseMethodBuilder != null)
					{
						eventBuilder.SetRaiseMethod(eventAccessorInfo2.RaiseMethodBuilder);
					}
				}
			}

			private MethodBuilder AddMethodImpl(MethodInfo mi)
			{
				ParameterInfo[] parameters = mi.GetParameters();
				Type[] array = DispatchProxyGenerator.ProxyBuilder.ParamTypes(parameters, false);
				MethodBuilder methodBuilder = this._tb.DefineMethod(mi.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, mi.ReturnType, array);
				if (mi.ContainsGenericParameters)
				{
					Type[] genericArguments = mi.GetGenericArguments();
					string[] array2 = new string[genericArguments.Length];
					for (int i = 0; i < genericArguments.Length; i++)
					{
						array2[i] = genericArguments[i].Name;
					}
					GenericTypeParameterBuilder[] array3 = methodBuilder.DefineGenericParameters(array2);
					for (int j = 0; j < array3.Length; j++)
					{
						array3[j].SetGenericParameterAttributes(genericArguments[j].GetTypeInfo().GenericParameterAttributes);
					}
				}
				ILGenerator ilgenerator = methodBuilder.GetILGenerator();
				DispatchProxyGenerator.ProxyBuilder.ParametersArray parametersArray = new DispatchProxyGenerator.ProxyBuilder.ParametersArray(ilgenerator, array);
				ilgenerator.Emit(OpCodes.Nop);
				DispatchProxyGenerator.ProxyBuilder.GenericArray<object> genericArray = new DispatchProxyGenerator.ProxyBuilder.GenericArray<object>(ilgenerator, DispatchProxyGenerator.ProxyBuilder.ParamTypes(parameters, true).Length);
				for (int k = 0; k < parameters.Length; k++)
				{
					if (!parameters[k].IsOut)
					{
						genericArray.BeginSet(k);
						parametersArray.Get(k);
						genericArray.EndSet(parameters[k].ParameterType);
					}
				}
				DispatchProxyGenerator.ProxyBuilder.GenericArray<object> genericArray2 = new DispatchProxyGenerator.ProxyBuilder.GenericArray<object>(ilgenerator, DispatchProxyGenerator.PackedArgs.PackedTypes.Length);
				genericArray2.BeginSet(0);
				ilgenerator.Emit(OpCodes.Ldarg_0);
				genericArray2.EndSet(typeof(DispatchProxy));
				MethodInfo runtimeMethod = typeof(Type).GetRuntimeMethod("GetTypeFromHandle", new Type[]
				{
					typeof(RuntimeTypeHandle)
				});
				Type cls;
				int arg;
				this._assembly.GetTokenForMethod(mi, out cls, out arg);
				genericArray2.BeginSet(1);
				ilgenerator.Emit(OpCodes.Ldtoken, cls);
				ilgenerator.Emit(OpCodes.Call, runtimeMethod);
				genericArray2.EndSet(typeof(object));
				genericArray2.BeginSet(2);
				ilgenerator.Emit(OpCodes.Ldc_I4, arg);
				genericArray2.EndSet(typeof(int));
				genericArray2.BeginSet(3);
				genericArray.Load();
				genericArray2.EndSet(typeof(object[]));
				if (mi.ContainsGenericParameters)
				{
					genericArray2.BeginSet(4);
					Type[] genericArguments2 = mi.GetGenericArguments();
					DispatchProxyGenerator.ProxyBuilder.GenericArray<Type> genericArray3 = new DispatchProxyGenerator.ProxyBuilder.GenericArray<Type>(ilgenerator, genericArguments2.Length);
					for (int l = 0; l < genericArguments2.Length; l++)
					{
						genericArray3.BeginSet(l);
						ilgenerator.Emit(OpCodes.Ldtoken, genericArguments2[l]);
						ilgenerator.Emit(OpCodes.Call, runtimeMethod);
						genericArray3.EndSet(typeof(Type));
					}
					genericArray3.Load();
					genericArray2.EndSet(typeof(Type[]));
				}
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldfld, this._fields[0]);
				genericArray2.Load();
				ilgenerator.Emit(OpCodes.Call, DispatchProxyGenerator.ProxyBuilder.s_delegateInvoke);
				for (int m = 0; m < parameters.Length; m++)
				{
					if (parameters[m].ParameterType.IsByRef)
					{
						parametersArray.BeginSet(m);
						genericArray.Get(m);
						parametersArray.EndSet(m, typeof(object));
					}
				}
				if (mi.ReturnType != typeof(void))
				{
					genericArray2.Get(5);
					DispatchProxyGenerator.ProxyBuilder.Convert(ilgenerator, typeof(object), mi.ReturnType, false);
				}
				ilgenerator.Emit(OpCodes.Ret);
				this._tb.DefineMethodOverride(methodBuilder, mi);
				return methodBuilder;
			}

			private static Type[] ParamTypes(ParameterInfo[] parms, bool noByRef)
			{
				Type[] array = new Type[parms.Length];
				for (int i = 0; i < parms.Length; i++)
				{
					array[i] = parms[i].ParameterType;
					if (noByRef && array[i].IsByRef)
					{
						array[i] = array[i].GetElementType();
					}
				}
				return array;
			}

			private static int GetTypeCode(Type type)
			{
				if (type == null)
				{
					return 0;
				}
				if (type == typeof(bool))
				{
					return 3;
				}
				if (type == typeof(char))
				{
					return 4;
				}
				if (type == typeof(sbyte))
				{
					return 5;
				}
				if (type == typeof(byte))
				{
					return 6;
				}
				if (type == typeof(short))
				{
					return 7;
				}
				if (type == typeof(ushort))
				{
					return 8;
				}
				if (type == typeof(int))
				{
					return 9;
				}
				if (type == typeof(uint))
				{
					return 10;
				}
				if (type == typeof(long))
				{
					return 11;
				}
				if (type == typeof(ulong))
				{
					return 12;
				}
				if (type == typeof(float))
				{
					return 13;
				}
				if (type == typeof(double))
				{
					return 14;
				}
				if (type == typeof(decimal))
				{
					return 15;
				}
				if (type == typeof(DateTime))
				{
					return 16;
				}
				if (type == typeof(string))
				{
					return 18;
				}
				if (type.GetTypeInfo().IsEnum)
				{
					return DispatchProxyGenerator.ProxyBuilder.GetTypeCode(Enum.GetUnderlyingType(type));
				}
				return 1;
			}

			private static void Convert(ILGenerator il, Type source, Type target, bool isAddress)
			{
				if (target == source)
				{
					return;
				}
				TypeInfo typeInfo = source.GetTypeInfo();
				TypeInfo typeInfo2 = target.GetTypeInfo();
				if (source.IsByRef)
				{
					Type elementType = source.GetElementType();
					DispatchProxyGenerator.ProxyBuilder.Ldind(il, elementType);
					DispatchProxyGenerator.ProxyBuilder.Convert(il, elementType, target, isAddress);
					return;
				}
				if (typeInfo2.IsValueType)
				{
					if (typeInfo.IsValueType)
					{
						OpCode opcode = DispatchProxyGenerator.ProxyBuilder.s_convOpCodes[DispatchProxyGenerator.ProxyBuilder.GetTypeCode(target)];
						il.Emit(opcode);
						return;
					}
					il.Emit(OpCodes.Unbox, target);
					if (!isAddress)
					{
						DispatchProxyGenerator.ProxyBuilder.Ldind(il, target);
						return;
					}
				}
				else if (typeInfo2.IsAssignableFrom(typeInfo))
				{
					if (typeInfo.IsValueType || source.IsGenericParameter)
					{
						if (isAddress)
						{
							DispatchProxyGenerator.ProxyBuilder.Ldind(il, source);
						}
						il.Emit(OpCodes.Box, source);
						return;
					}
				}
				else
				{
					if (target.IsGenericParameter)
					{
						il.Emit(OpCodes.Unbox_Any, target);
						return;
					}
					il.Emit(OpCodes.Castclass, target);
				}
			}

			private static void Ldind(ILGenerator il, Type type)
			{
				OpCode opcode = DispatchProxyGenerator.ProxyBuilder.s_ldindOpCodes[DispatchProxyGenerator.ProxyBuilder.GetTypeCode(type)];
				if (!opcode.Equals(OpCodes.Nop))
				{
					il.Emit(opcode);
					return;
				}
				il.Emit(OpCodes.Ldobj, type);
			}

			private static void Stind(ILGenerator il, Type type)
			{
				OpCode opcode = DispatchProxyGenerator.ProxyBuilder.s_stindOpCodes[DispatchProxyGenerator.ProxyBuilder.GetTypeCode(type)];
				if (!opcode.Equals(OpCodes.Nop))
				{
					il.Emit(opcode);
					return;
				}
				il.Emit(OpCodes.Stobj, type);
			}

			private static readonly MethodInfo s_delegateInvoke = typeof(Action<object[]>).GetTypeInfo().GetDeclaredMethod("Invoke");

			private DispatchProxyGenerator.ProxyAssembly _assembly;

			private TypeBuilder _tb;

			private Type _proxyBaseType;

			private List<FieldBuilder> _fields;

			private static OpCode[] s_convOpCodes = new OpCode[]
			{
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Conv_I1,
				OpCodes.Conv_I2,
				OpCodes.Conv_I1,
				OpCodes.Conv_U1,
				OpCodes.Conv_I2,
				OpCodes.Conv_U2,
				OpCodes.Conv_I4,
				OpCodes.Conv_U4,
				OpCodes.Conv_I8,
				OpCodes.Conv_U8,
				OpCodes.Conv_R4,
				OpCodes.Conv_R8,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop
			};

			private static OpCode[] s_ldindOpCodes = new OpCode[]
			{
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Ldind_I1,
				OpCodes.Ldind_I2,
				OpCodes.Ldind_I1,
				OpCodes.Ldind_U1,
				OpCodes.Ldind_I2,
				OpCodes.Ldind_U2,
				OpCodes.Ldind_I4,
				OpCodes.Ldind_U4,
				OpCodes.Ldind_I8,
				OpCodes.Ldind_I8,
				OpCodes.Ldind_R4,
				OpCodes.Ldind_R8,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Ldind_Ref
			};

			private static OpCode[] s_stindOpCodes = new OpCode[]
			{
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Stind_I1,
				OpCodes.Stind_I2,
				OpCodes.Stind_I1,
				OpCodes.Stind_I1,
				OpCodes.Stind_I2,
				OpCodes.Stind_I2,
				OpCodes.Stind_I4,
				OpCodes.Stind_I4,
				OpCodes.Stind_I8,
				OpCodes.Stind_I8,
				OpCodes.Stind_R4,
				OpCodes.Stind_R8,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Nop,
				OpCodes.Stind_Ref
			};

			private class ParametersArray
			{
				internal ParametersArray(ILGenerator il, Type[] paramTypes)
				{
					this._il = il;
					this._paramTypes = paramTypes;
				}

				internal void Get(int i)
				{
					this._il.Emit(OpCodes.Ldarg, i + 1);
				}

				internal void BeginSet(int i)
				{
					this._il.Emit(OpCodes.Ldarg, i + 1);
				}

				internal void EndSet(int i, Type stackType)
				{
					Type elementType = this._paramTypes[i].GetElementType();
					DispatchProxyGenerator.ProxyBuilder.Convert(this._il, stackType, elementType, false);
					DispatchProxyGenerator.ProxyBuilder.Stind(this._il, elementType);
				}

				private ILGenerator _il;

				private Type[] _paramTypes;
			}

			private class GenericArray<T>
			{
				internal GenericArray(ILGenerator il, int len)
				{
					this._il = il;
					this._lb = il.DeclareLocal(typeof(T[]));
					il.Emit(OpCodes.Ldc_I4, len);
					il.Emit(OpCodes.Newarr, typeof(T));
					il.Emit(OpCodes.Stloc, this._lb);
				}

				internal void Load()
				{
					this._il.Emit(OpCodes.Ldloc, this._lb);
				}

				internal void Get(int i)
				{
					this._il.Emit(OpCodes.Ldloc, this._lb);
					this._il.Emit(OpCodes.Ldc_I4, i);
					this._il.Emit(OpCodes.Ldelem_Ref);
				}

				internal void BeginSet(int i)
				{
					this._il.Emit(OpCodes.Ldloc, this._lb);
					this._il.Emit(OpCodes.Ldc_I4, i);
				}

				internal void EndSet(Type stackType)
				{
					DispatchProxyGenerator.ProxyBuilder.Convert(this._il, stackType, typeof(T), false);
					this._il.Emit(OpCodes.Stelem_Ref);
				}

				private ILGenerator _il;

				private LocalBuilder _lb;
			}

			private sealed class PropertyAccessorInfo
			{
				public MethodInfo InterfaceGetMethod { get; }

				public MethodInfo InterfaceSetMethod { get; }

				public MethodBuilder GetMethodBuilder { get; set; }

				public MethodBuilder SetMethodBuilder { get; set; }

				public PropertyAccessorInfo(MethodInfo interfaceGetMethod, MethodInfo interfaceSetMethod)
				{
					this.InterfaceGetMethod = interfaceGetMethod;
					this.InterfaceSetMethod = interfaceSetMethod;
				}
			}

			private sealed class EventAccessorInfo
			{
				public MethodInfo InterfaceAddMethod { get; }

				public MethodInfo InterfaceRemoveMethod { get; }

				public MethodInfo InterfaceRaiseMethod { get; }

				public MethodBuilder AddMethodBuilder { get; set; }

				public MethodBuilder RemoveMethodBuilder { get; set; }

				public MethodBuilder RaiseMethodBuilder { get; set; }

				public EventAccessorInfo(MethodInfo interfaceAddMethod, MethodInfo interfaceRemoveMethod, MethodInfo interfaceRaiseMethod)
				{
					this.InterfaceAddMethod = interfaceAddMethod;
					this.InterfaceRemoveMethod = interfaceRemoveMethod;
					this.InterfaceRaiseMethod = interfaceRaiseMethod;
				}
			}

			private sealed class MethodInfoEqualityComparer : EqualityComparer<MethodInfo>
			{
				private MethodInfoEqualityComparer()
				{
				}

				public sealed override bool Equals(MethodInfo left, MethodInfo right)
				{
					if (left == right)
					{
						return true;
					}
					if (left == null)
					{
						return right == null;
					}
					if (right == null)
					{
						return false;
					}
					if (!object.Equals(left.DeclaringType, right.DeclaringType))
					{
						return false;
					}
					if (!object.Equals(left.ReturnType, right.ReturnType))
					{
						return false;
					}
					if (left.CallingConvention != right.CallingConvention)
					{
						return false;
					}
					if (left.IsStatic != right.IsStatic)
					{
						return false;
					}
					if (left.Name != right.Name)
					{
						return false;
					}
					Type[] genericArguments = left.GetGenericArguments();
					Type[] genericArguments2 = right.GetGenericArguments();
					if (genericArguments.Length != genericArguments2.Length)
					{
						return false;
					}
					for (int i = 0; i < genericArguments.Length; i++)
					{
						if (!object.Equals(genericArguments[i], genericArguments2[i]))
						{
							return false;
						}
					}
					ParameterInfo[] parameters = left.GetParameters();
					ParameterInfo[] parameters2 = right.GetParameters();
					if (parameters.Length != parameters2.Length)
					{
						return false;
					}
					for (int j = 0; j < parameters.Length; j++)
					{
						if (!object.Equals(parameters[j].ParameterType, parameters2[j].ParameterType))
						{
							return false;
						}
					}
					return true;
				}

				public sealed override int GetHashCode(MethodInfo obj)
				{
					if (obj == null)
					{
						return 0;
					}
					int num = obj.DeclaringType.GetHashCode();
					num ^= obj.Name.GetHashCode();
					foreach (ParameterInfo parameterInfo in obj.GetParameters())
					{
						num ^= parameterInfo.ParameterType.GetHashCode();
					}
					return num;
				}

				public static readonly DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer Instance = new DispatchProxyGenerator.ProxyBuilder.MethodInfoEqualityComparer();
			}
		}
	}
}
