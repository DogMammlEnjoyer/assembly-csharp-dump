using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection.Emit
{
	/// <summary>Defines and represents a dynamic method that can be compiled, executed, and discarded. Discarded methods are available for garbage collection.</summary>
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DynamicMethod : MethodInfo
	{
		/// <summary>Creates a dynamic method that is global to a module, specifying the method name, return type, parameter types, and module.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="m">A <see cref="T:System.Reflection.Module" /> representing the module with which the dynamic method is to be logically associated.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.  
		///  -or-  
		///  <paramref name="m" /> is a module that provides anonymous hosting for dynamic methods.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="m" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="returnType" /> is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m) : this(name, returnType, parameterTypes, m, false)
		{
		}

		/// <summary>Creates a dynamic method, specifying the method name, return type, parameter types, and the type with which the dynamic method is logically associated.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="owner">A <see cref="T:System.Type" /> with which the dynamic method is logically associated. The dynamic method has access to all members of the type.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.  
		///  -or-  
		///  <paramref name="owner" /> is an interface, an array, an open generic type, or a type parameter of a generic type or method.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="owner" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="returnType" /> is <see langword="null" />, or is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner) : this(name, returnType, parameterTypes, owner, false)
		{
		}

		/// <summary>Creates a dynamic method that is global to a module, specifying the method name, return type, parameter types, module, and whether just-in-time (JIT) visibility checks should be skipped for types and members accessed by the Microsoft intermediate language (MSIL) of the dynamic method.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="m">A <see cref="T:System.Reflection.Module" /> representing the module with which the dynamic method is to be logically associated.</param>
		/// <param name="skipVisibility">
		///   <see langword="true" /> to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic method.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.  
		///  -or-  
		///  <paramref name="m" /> is a module that provides anonymous hosting for dynamic methods.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="m" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="returnType" /> is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility) : this(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, m, skipVisibility)
		{
		}

		/// <summary>Creates a dynamic method, specifying the method name, return type, parameter types, the type with which the dynamic method is logically associated, and whether just-in-time (JIT) visibility checks should be skipped for types and members accessed by the Microsoft intermediate language (MSIL) of the dynamic method.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="owner">A <see cref="T:System.Type" /> with which the dynamic method is logically associated. The dynamic method has access to all members of the type.</param>
		/// <param name="skipVisibility">
		///   <see langword="true" /> to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic method; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.  
		///  -or-  
		///  <paramref name="owner" /> is an interface, an array, an open generic type, or a type parameter of a generic type or method.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="owner" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="returnType" /> is <see langword="null" />, or is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility) : this(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, owner, skipVisibility)
		{
		}

		/// <summary>Creates a dynamic method, specifying the method name, attributes, calling convention, return type, parameter types, the type with which the dynamic method is logically associated, and whether just-in-time (JIT) visibility checks should be skipped for types and members accessed by the Microsoft intermediate language (MSIL) of the dynamic method.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="attributes">A bitwise combination of <see cref="T:System.Reflection.MethodAttributes" /> values that specifies the attributes of the dynamic method. The only combination allowed is <see cref="F:System.Reflection.MethodAttributes.Public" /> and <see cref="F:System.Reflection.MethodAttributes.Static" />.</param>
		/// <param name="callingConvention">The calling convention for the dynamic method. Must be <see cref="F:System.Reflection.CallingConventions.Standard" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="owner">A <see cref="T:System.Type" /> with which the dynamic method is logically associated. The dynamic method has access to all members of the type.</param>
		/// <param name="skipVisibility">
		///   <see langword="true" /> to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic method; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.  
		///  -or-  
		///  <paramref name="owner" /> is an interface, an array, an open generic type, or a type parameter of a generic type or method.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="owner" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="attributes" /> is a combination of flags other than <see cref="F:System.Reflection.MethodAttributes.Public" /> and <see cref="F:System.Reflection.MethodAttributes.Static" />.  
		/// -or-  
		/// <paramref name="callingConvention" /> is not <see cref="F:System.Reflection.CallingConventions.Standard" />.  
		/// -or-  
		/// <paramref name="returnType" /> is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type owner, bool skipVisibility) : this(name, attributes, callingConvention, returnType, parameterTypes, owner, owner.Module, skipVisibility, false)
		{
		}

		/// <summary>Creates a dynamic method that is global to a module, specifying the method name, attributes, calling convention, return type, parameter types, module, and whether just-in-time (JIT) visibility checks should be skipped for types and members accessed by the Microsoft intermediate language (MSIL) of the dynamic method.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="attributes">A bitwise combination of <see cref="T:System.Reflection.MethodAttributes" /> values that specifies the attributes of the dynamic method. The only combination allowed is <see cref="F:System.Reflection.MethodAttributes.Public" /> and <see cref="F:System.Reflection.MethodAttributes.Static" />.</param>
		/// <param name="callingConvention">The calling convention for the dynamic method. Must be <see cref="F:System.Reflection.CallingConventions.Standard" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="m">A <see cref="T:System.Reflection.Module" /> representing the module with which the dynamic method is to be logically associated.</param>
		/// <param name="skipVisibility">
		///   <see langword="true" /> to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic method; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.  
		///  -or-  
		///  <paramref name="m" /> is a module that provides anonymous hosting for dynamic methods.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="m" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="attributes" /> is a combination of flags other than <see cref="F:System.Reflection.MethodAttributes.Public" /> and <see cref="F:System.Reflection.MethodAttributes.Static" />.  
		/// -or-  
		/// <paramref name="callingConvention" /> is not <see cref="F:System.Reflection.CallingConventions.Standard" />.  
		/// -or-  
		/// <paramref name="returnType" /> is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Module m, bool skipVisibility) : this(name, attributes, callingConvention, returnType, parameterTypes, null, m, skipVisibility, false)
		{
		}

		/// <summary>Initializes an anonymously hosted dynamic method, specifying the method name, return type, and parameter types.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="returnType" /> is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes) : this(name, returnType, parameterTypes, false)
		{
		}

		/// <summary>Initializes an anonymously hosted dynamic method, specifying the method name, return type, parameter types, and whether just-in-time (JIT) visibility checks should be skipped for types and members accessed by the Microsoft intermediate language (MSIL) of the dynamic method.</summary>
		/// <param name="name">The name of the dynamic method. This can be a zero-length string, but it cannot be <see langword="null" />.</param>
		/// <param name="returnType">A <see cref="T:System.Type" /> object that specifies the return type of the dynamic method, or <see langword="null" /> if the method has no return type.</param>
		/// <param name="parameterTypes">An array of <see cref="T:System.Type" /> objects specifying the types of the parameters of the dynamic method, or <see langword="null" /> if the method has no parameters.</param>
		/// <param name="restrictedSkipVisibility">
		///   <see langword="true" /> to skip JIT visibility checks on types and members accessed by the MSIL of the dynamic method, with this restriction: the trust level of the assemblies that contain those types and members must be equal to or less than the trust level of the call stack that emits the dynamic method; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.ArgumentException">An element of <paramref name="parameterTypes" /> is <see langword="null" /> or <see cref="T:System.Void" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="returnType" /> is a type for which <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		[MonoTODO("Visibility is not restricted")]
		public DynamicMethod(string name, Type returnType, Type[] parameterTypes, bool restrictedSkipVisibility) : this(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static, CallingConventions.Standard, returnType, parameterTypes, null, null, restrictedSkipVisibility, true)
		{
		}

		private DynamicMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type owner, Module m, bool skipVisibility, bool anonHosted)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (returnType == null)
			{
				returnType = typeof(void);
			}
			if (m == null && !anonHosted)
			{
				throw new ArgumentNullException("m");
			}
			if (returnType.IsByRef)
			{
				throw new ArgumentException("Return type can't be a byref type", "returnType");
			}
			if (parameterTypes != null)
			{
				for (int i = 0; i < parameterTypes.Length; i++)
				{
					if (parameterTypes[i] == null)
					{
						throw new ArgumentException("Parameter " + i.ToString() + " is null", "parameterTypes");
					}
				}
			}
			if (owner != null && (owner.IsArray || owner.IsInterface))
			{
				throw new ArgumentException("Owner can't be an array or an interface.");
			}
			if (m == null)
			{
				m = DynamicMethod.AnonHostModuleHolder.AnonHostModule;
			}
			this.name = name;
			this.attributes = (attributes | MethodAttributes.Static);
			this.callingConvention = callingConvention;
			this.returnType = returnType;
			this.parameters = parameterTypes;
			this.owner = owner;
			this.module = m;
			this.skipVisibility = skipVisibility;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void create_dynamic_method(DynamicMethod m);

		private void CreateDynMethod()
		{
			lock (this)
			{
				if (this.mhandle.Value == IntPtr.Zero)
				{
					if (this.ilgen == null || this.ilgen.ILOffset == 0)
					{
						throw new InvalidOperationException("Method '" + this.name + "' does not have a method body.");
					}
					this.ilgen.label_fixup(this);
					try
					{
						this.creating = true;
						if (this.refs != null)
						{
							for (int i = 0; i < this.refs.Length; i++)
							{
								if (this.refs[i] is DynamicMethod)
								{
									DynamicMethod dynamicMethod = (DynamicMethod)this.refs[i];
									if (!dynamicMethod.creating)
									{
										dynamicMethod.CreateDynMethod();
									}
								}
							}
						}
					}
					finally
					{
						this.creating = false;
					}
					DynamicMethod.create_dynamic_method(this);
					this.ilgen = null;
				}
			}
		}

		/// <summary>Completes the dynamic method and creates a delegate that can be used to execute it.</summary>
		/// <param name="delegateType">A delegate type whose signature matches that of the dynamic method.</param>
		/// <returns>A delegate of the specified type, which can be used to execute the dynamic method.</returns>
		/// <exception cref="T:System.InvalidOperationException">The dynamic method has no method body.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="delegateType" /> has the wrong number of parameters or the wrong parameter types.</exception>
		[ComVisible(true)]
		public sealed override Delegate CreateDelegate(Type delegateType)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			if (this.deleg != null)
			{
				return this.deleg;
			}
			this.CreateDynMethod();
			this.deleg = Delegate.CreateDelegate(delegateType, null, this);
			return this.deleg;
		}

		/// <summary>Completes the dynamic method and creates a delegate that can be used to execute it, specifying the delegate type and an object the delegate is bound to.</summary>
		/// <param name="delegateType">A delegate type whose signature matches that of the dynamic method, minus the first parameter.</param>
		/// <param name="target">An object the delegate is bound to. Must be of the same type as the first parameter of the dynamic method.</param>
		/// <returns>A delegate of the specified type, which can be used to execute the dynamic method with the specified target object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The dynamic method has no method body.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="target" /> is not the same type as the first parameter of the dynamic method, and is not assignable to that type.  
		/// -or-  
		/// <paramref name="delegateType" /> has the wrong number of parameters or the wrong parameter types.</exception>
		[ComVisible(true)]
		public sealed override Delegate CreateDelegate(Type delegateType, object target)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			this.CreateDynMethod();
			return Delegate.CreateDelegate(delegateType, target, this);
		}

		/// <summary>Defines a parameter of the dynamic method.</summary>
		/// <param name="position">The position of the parameter in the parameter list. Parameters are indexed beginning with the number 1 for the first parameter.</param>
		/// <param name="attributes">A bitwise combination of <see cref="T:System.Reflection.ParameterAttributes" /> values that specifies the attributes of the parameter.</param>
		/// <param name="parameterName">The name of the parameter. The name can be a zero-length string.</param>
		/// <returns>Always returns <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The method has no parameters.  
		///  -or-  
		///  <paramref name="position" /> is less than 0.  
		///  -or-  
		///  <paramref name="position" /> is greater than the number of the method's parameters.</exception>
		public ParameterBuilder DefineParameter(int position, ParameterAttributes attributes, string parameterName)
		{
			if (position < 0 || position > this.parameters.Length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			this.RejectIfCreated();
			ParameterBuilder parameterBuilder = new ParameterBuilder(this, position, attributes, parameterName);
			if (this.pinfo == null)
			{
				this.pinfo = new ParameterBuilder[this.parameters.Length + 1];
			}
			this.pinfo[position] = parameterBuilder;
			return parameterBuilder;
		}

		/// <summary>Returns the base implementation for the method.</summary>
		/// <returns>The base implementation of the method.</returns>
		public override MethodInfo GetBaseDefinition()
		{
			return this;
		}

		/// <summary>Returns all the custom attributes defined for the method.</summary>
		/// <param name="inherit">
		///   <see langword="true" /> to search the method's inheritance chain to find the custom attributes; <see langword="false" /> to check only the current method.</param>
		/// <returns>An array of objects representing all the custom attributes of the method.</returns>
		public override object[] GetCustomAttributes(bool inherit)
		{
			return new object[]
			{
				new MethodImplAttribute(this.GetMethodImplementationFlags())
			};
		}

		/// <summary>Returns the custom attributes of the specified type that have been applied to the method.</summary>
		/// <param name="attributeType">A <see cref="T:System.Type" /> representing the type of custom attribute to return.</param>
		/// <param name="inherit">
		///   <see langword="true" /> to search the method's inheritance chain to find the custom attributes; <see langword="false" /> to check only the current method.</param>
		/// <returns>An array of objects representing the attributes of the method that are of type <paramref name="attributeType" /> or derive from type <paramref name="attributeType" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is <see langword="null" />.</exception>
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			if (attributeType.IsAssignableFrom(typeof(MethodImplAttribute)))
			{
				return new object[]
				{
					new MethodImplAttribute(this.GetMethodImplementationFlags())
				};
			}
			return EmptyArray<object>.Value;
		}

		/// <summary>Returns a <see cref="T:System.Reflection.Emit.DynamicILInfo" /> object that can be used to generate a method body from metadata tokens, scopes, and Microsoft intermediate language (MSIL) streams.</summary>
		/// <returns>A <see cref="T:System.Reflection.Emit.DynamicILInfo" /> object that can be used to generate a method body from metadata tokens, scopes, and MSIL streams.</returns>
		public DynamicILInfo GetDynamicILInfo()
		{
			if (this.il_info == null)
			{
				this.il_info = new DynamicILInfo(this);
			}
			return this.il_info;
		}

		/// <summary>Returns a Microsoft intermediate language (MSIL) generator for the method with a default MSIL stream size of 64 bytes.</summary>
		/// <returns>An <see cref="T:System.Reflection.Emit.ILGenerator" /> object for the method.</returns>
		public ILGenerator GetILGenerator()
		{
			return this.GetILGenerator(64);
		}

		/// <summary>Returns a Microsoft intermediate language (MSIL) generator for the method with the specified MSIL stream size.</summary>
		/// <param name="streamSize">The size of the MSIL stream, in bytes.</param>
		/// <returns>An <see cref="T:System.Reflection.Emit.ILGenerator" /> object for the method, with the specified MSIL stream size.</returns>
		public ILGenerator GetILGenerator(int streamSize)
		{
			if ((this.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != MethodImplAttributes.IL || (this.GetMethodImplementationFlags() & MethodImplAttributes.ManagedMask) != MethodImplAttributes.IL)
			{
				throw new InvalidOperationException("Method body should not exist.");
			}
			if (this.ilgen != null)
			{
				return this.ilgen;
			}
			this.ilgen = new ILGenerator(this.Module, new DynamicMethodTokenGenerator(this), streamSize);
			return this.ilgen;
		}

		/// <summary>Returns the implementation flags for the method.</summary>
		/// <returns>A bitwise combination of <see cref="T:System.Reflection.MethodImplAttributes" /> values representing the implementation flags for the method.</returns>
		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MethodImplAttributes.NoInlining;
		}

		/// <summary>Returns the parameters of the dynamic method.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.ParameterInfo" /> objects that represent the parameters of the dynamic method.</returns>
		public override ParameterInfo[] GetParameters()
		{
			return this.GetParametersInternal();
		}

		internal override ParameterInfo[] GetParametersInternal()
		{
			if (this.parameters == null)
			{
				return EmptyArray<ParameterInfo>.Value;
			}
			ParameterInfo[] array = new ParameterInfo[this.parameters.Length];
			for (int i = 0; i < this.parameters.Length; i++)
			{
				ParameterInfo[] array2 = array;
				int num = i;
				ParameterBuilder[] array3 = this.pinfo;
				array2[num] = RuntimeParameterInfo.New((array3 != null) ? array3[i + 1] : null, this.parameters[i], this, i + 1);
			}
			return array;
		}

		internal override int GetParametersCount()
		{
			if (this.parameters != null)
			{
				return this.parameters.Length;
			}
			return 0;
		}

		internal override Type GetParameterType(int pos)
		{
			return this.parameters[pos];
		}

		/// <summary>Invokes the dynamic method using the specified parameters, under the constraints of the specified binder, with the specified culture information.</summary>
		/// <param name="obj">This parameter is ignored for dynamic methods, because they are static. Specify <see langword="null" />.</param>
		/// <param name="invokeAttr">A bitwise combination of <see cref="T:System.Reflection.BindingFlags" /> values.</param>
		/// <param name="binder">A <see cref="T:System.Reflection.Binder" /> object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects through reflection. If <paramref name="binder" /> is <see langword="null" />, the default binder is used. For more details, see <see cref="T:System.Reflection.Binder" />.</param>
		/// <param name="parameters">An argument list. This is an array of arguments with the same number, order, and type as the parameters of the method to be invoked. If there are no parameters this parameter should be <see langword="null" />.</param>
		/// <param name="culture">An instance of <see cref="T:System.Globalization.CultureInfo" /> used to govern the coercion of types. If this is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used. For example, this information is needed to correctly convert a <see cref="T:System.String" /> that represents 1000 to a <see cref="T:System.Double" /> value, because 1000 is represented differently by different cultures.</param>
		/// <returns>A <see cref="T:System.Object" /> containing the return value of the invoked method.</returns>
		/// <exception cref="T:System.NotSupportedException">The <see cref="F:System.Reflection.CallingConventions.VarArgs" /> calling convention is not supported.</exception>
		/// <exception cref="T:System.Reflection.TargetParameterCountException">The number of elements in <paramref name="parameters" /> does not match the number of parameters in the dynamic method.</exception>
		/// <exception cref="T:System.ArgumentException">The type of one or more elements of <paramref name="parameters" /> does not match the type of the corresponding parameter of the dynamic method.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The dynamic method is associated with a module, is not anonymously hosted, and was constructed with <paramref name="skipVisibility" /> set to <see langword="false" />, but the dynamic method accesses members that are not <see langword="public" /> or <see langword="internal" /> (<see langword="Friend" /> in Visual Basic).  
		///  -or-  
		///  The dynamic method is anonymously hosted and was constructed with <paramref name="skipVisibility" /> set to <see langword="false" />, but it accesses members that are not <see langword="public" />.  
		///  -or-  
		///  The dynamic method contains unverifiable code. See the "Verification" section in Remarks for <see cref="T:System.Reflection.Emit.DynamicMethod" />.</exception>
		[SecuritySafeCritical]
		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			object result;
			try
			{
				this.CreateDynMethod();
				if (this.method == null)
				{
					this.method = new RuntimeMethodInfo(this.mhandle);
				}
				result = this.method.Invoke(obj, invokeAttr, binder, parameters, culture);
			}
			catch (MethodAccessException inner)
			{
				throw new TargetInvocationException("Method cannot be invoked.", inner);
			}
			return result;
		}

		/// <summary>Indicates whether the specified custom attribute type is defined.</summary>
		/// <param name="attributeType">A <see cref="T:System.Type" /> representing the type of custom attribute to search for.</param>
		/// <param name="inherit">
		///   <see langword="true" /> to search the method's inheritance chain to find the custom attributes; <see langword="false" /> to check only the current method.</param>
		/// <returns>
		///   <see langword="true" /> if the specified custom attribute type is defined; otherwise, <see langword="false" />.</returns>
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			return attributeType.IsAssignableFrom(typeof(MethodImplAttribute));
		}

		/// <summary>Returns the signature of the method, represented as a string.</summary>
		/// <returns>A string representing the method signature.</returns>
		public override string ToString()
		{
			string text = string.Empty;
			ParameterInfo[] parametersInternal = this.GetParametersInternal();
			for (int i = 0; i < parametersInternal.Length; i++)
			{
				if (i > 0)
				{
					text += ", ";
				}
				text += parametersInternal[i].ParameterType.Name;
			}
			return string.Concat(new string[]
			{
				this.ReturnType.Name,
				" ",
				this.Name,
				"(",
				text,
				")"
			});
		}

		/// <summary>Gets the attributes specified when the dynamic method was created.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Reflection.MethodAttributes" /> values representing the attributes for the method.</returns>
		public override MethodAttributes Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		/// <summary>Gets the calling convention specified when the dynamic method was created.</summary>
		/// <returns>One of the <see cref="T:System.Reflection.CallingConventions" /> values that indicates the calling convention of the method.</returns>
		public override CallingConventions CallingConvention
		{
			get
			{
				return this.callingConvention;
			}
		}

		/// <summary>Gets the type that declares the method, which is always <see langword="null" /> for dynamic methods.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets or sets a value indicating whether the local variables in the method are zero-initialized.</summary>
		/// <returns>
		///   <see langword="true" /> if the local variables in the method are zero-initialized; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool InitLocals
		{
			get
			{
				return this.init_locals;
			}
			set
			{
				this.init_locals = value;
			}
		}

		/// <summary>Not supported for dynamic methods.</summary>
		/// <returns>Not supported for dynamic methods.</returns>
		/// <exception cref="T:System.InvalidOperationException">Not allowed for dynamic methods.</exception>
		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return this.mhandle;
			}
		}

		/// <summary>Gets the module with which the dynamic method is logically associated.</summary>
		/// <returns>The <see cref="T:System.Reflection.Module" /> with which the current dynamic method is associated.</returns>
		public override Module Module
		{
			get
			{
				return this.module;
			}
		}

		/// <summary>Gets the name of the dynamic method.</summary>
		/// <returns>The simple name of the method.</returns>
		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets the class that was used in reflection to obtain the method.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		public override Type ReflectedType
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the return parameter of the dynamic method.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		[MonoTODO("Not implemented")]
		public override ParameterInfo ReturnParameter
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the type of return value for the dynamic method.</summary>
		/// <returns>A <see cref="T:System.Type" /> representing the type of the return value of the current method; <see cref="T:System.Void" /> if the method has no return type.</returns>
		public override Type ReturnType
		{
			get
			{
				return this.returnType;
			}
		}

		/// <summary>Gets the custom attributes of the return type for the dynamic method.</summary>
		/// <returns>An <see cref="T:System.Reflection.ICustomAttributeProvider" /> representing the custom attributes of the return type for the dynamic method.</returns>
		[MonoTODO("Not implemented")]
		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private void RejectIfCreated()
		{
			if (this.mhandle.Value != IntPtr.Zero)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
		}

		internal int AddRef(object reference)
		{
			if (this.refs == null)
			{
				this.refs = new object[4];
			}
			if (this.nrefs >= this.refs.Length - 1)
			{
				object[] destinationArray = new object[this.refs.Length * 2];
				Array.Copy(this.refs, destinationArray, this.refs.Length);
				this.refs = destinationArray;
			}
			this.refs[this.nrefs] = reference;
			this.refs[this.nrefs + 1] = null;
			this.nrefs += 2;
			return this.nrefs - 1;
		}

		private RuntimeMethodHandle mhandle;

		private string name;

		private Type returnType;

		private Type[] parameters;

		private MethodAttributes attributes;

		private CallingConventions callingConvention;

		private Module module;

		private bool skipVisibility;

		private bool init_locals = true;

		private ILGenerator ilgen;

		private int nrefs;

		private object[] refs;

		private IntPtr referenced_by;

		private Type owner;

		private Delegate deleg;

		private RuntimeMethodInfo method;

		private ParameterBuilder[] pinfo;

		internal bool creating;

		private DynamicILInfo il_info;

		private static class AnonHostModuleHolder
		{
			static AnonHostModuleHolder()
			{
				AssemblyName assemblyName = new AssemblyName();
				assemblyName.Name = "Anonymously Hosted DynamicMethods Assembly";
				DynamicMethod.AnonHostModuleHolder.anon_host_module = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run).GetManifestModule();
			}

			public static Module AnonHostModule
			{
				get
				{
					return DynamicMethod.AnonHostModuleHolder.anon_host_module;
				}
			}

			public static readonly Module anon_host_module;
		}
	}
}
