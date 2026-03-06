using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Unity;

namespace System.Reflection
{
	/// <summary>Provides information about methods and constructors.</summary>
	[Serializable]
	public abstract class MethodBase : MemberInfo, _MethodBase
	{
		/// <summary>When overridden in a derived class, gets the parameters of the specified method or constructor.</summary>
		/// <returns>An array of type <see langword="ParameterInfo" /> containing information that matches the signature of the method (or constructor) reflected by this <see langword="MethodBase" /> instance.</returns>
		public abstract ParameterInfo[] GetParameters();

		/// <summary>Gets the attributes associated with this method.</summary>
		/// <returns>One of the <see cref="T:System.Reflection.MethodAttributes" /> values.</returns>
		public abstract MethodAttributes Attributes { get; }

		/// <summary>Gets the <see cref="T:System.Reflection.MethodImplAttributes" /> flags that specify the attributes of a method implementation.</summary>
		/// <returns>The method implementation flags.</returns>
		public virtual MethodImplAttributes MethodImplementationFlags
		{
			get
			{
				return this.GetMethodImplementationFlags();
			}
		}

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Reflection.MethodImplAttributes" /> flags.</summary>
		/// <returns>The <see langword="MethodImplAttributes" /> flags.</returns>
		public abstract MethodImplAttributes GetMethodImplementationFlags();

		/// <summary>When overridden in a derived class, gets a <see cref="T:System.Reflection.MethodBody" /> object that provides access to the MSIL stream, local variables, and exceptions for the current method.</summary>
		/// <returns>A <see cref="T:System.Reflection.MethodBody" /> object that provides access to the MSIL stream, local variables, and exceptions for the current method.</returns>
		/// <exception cref="T:System.InvalidOperationException">This method is invalid unless overridden in a derived class.</exception>
		public virtual MethodBody GetMethodBody()
		{
			throw new InvalidOperationException();
		}

		/// <summary>Gets a value indicating the calling conventions for this method.</summary>
		/// <returns>The <see cref="T:System.Reflection.CallingConventions" /> for this method.</returns>
		public virtual CallingConventions CallingConvention
		{
			get
			{
				return CallingConventions.Standard;
			}
		}

		/// <summary>Gets a value indicating whether the method is abstract.</summary>
		/// <returns>
		///   <see langword="true" /> if the method is abstract; otherwise, <see langword="false" />.</returns>
		public bool IsAbstract
		{
			get
			{
				return (this.Attributes & MethodAttributes.Abstract) > MethodAttributes.PrivateScope;
			}
		}

		/// <summary>Gets a value indicating whether the method is a constructor.</summary>
		/// <returns>
		///   <see langword="true" /> if this method is a constructor represented by a <see cref="T:System.Reflection.ConstructorInfo" /> object (see note in Remarks about <see cref="T:System.Reflection.Emit.ConstructorBuilder" /> objects); otherwise, <see langword="false" />.</returns>
		public bool IsConstructor
		{
			get
			{
				return this is ConstructorInfo && !this.IsStatic && (this.Attributes & MethodAttributes.RTSpecialName) == MethodAttributes.RTSpecialName;
			}
		}

		/// <summary>Gets a value indicating whether this method is <see langword="final" />.</summary>
		/// <returns>
		///   <see langword="true" /> if this method is <see langword="final" />; otherwise, <see langword="false" />.</returns>
		public bool IsFinal
		{
			get
			{
				return (this.Attributes & MethodAttributes.Final) > MethodAttributes.PrivateScope;
			}
		}

		/// <summary>Gets a value indicating whether only a member of the same kind with exactly the same signature is hidden in the derived class.</summary>
		/// <returns>
		///   <see langword="true" /> if the member is hidden by signature; otherwise, <see langword="false" />.</returns>
		public bool IsHideBySig
		{
			get
			{
				return (this.Attributes & MethodAttributes.HideBySig) > MethodAttributes.PrivateScope;
			}
		}

		/// <summary>Gets a value indicating whether this method has a special name.</summary>
		/// <returns>
		///   <see langword="true" /> if this method has a special name; otherwise, <see langword="false" />.</returns>
		public bool IsSpecialName
		{
			get
			{
				return (this.Attributes & MethodAttributes.SpecialName) > MethodAttributes.PrivateScope;
			}
		}

		/// <summary>Gets a value indicating whether the method is <see langword="static" />.</summary>
		/// <returns>
		///   <see langword="true" /> if this method is <see langword="static" />; otherwise, <see langword="false" />.</returns>
		public bool IsStatic
		{
			get
			{
				return (this.Attributes & MethodAttributes.Static) > MethodAttributes.PrivateScope;
			}
		}

		/// <summary>Gets a value indicating whether the method is <see langword="virtual" />.</summary>
		/// <returns>
		///   <see langword="true" /> if this method is <see langword="virtual" />; otherwise, <see langword="false" />.</returns>
		public bool IsVirtual
		{
			get
			{
				return (this.Attributes & MethodAttributes.Virtual) > MethodAttributes.PrivateScope;
			}
		}

		/// <summary>Gets a value indicating whether the potential visibility of this method or constructor is described by <see cref="F:System.Reflection.MethodAttributes.Assembly" />; that is, the method or constructor is visible at most to other types in the same assembly, and is not visible to derived types outside the assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if the visibility of this method or constructor is exactly described by <see cref="F:System.Reflection.MethodAttributes.Assembly" />; otherwise, <see langword="false" />.</returns>
		public bool IsAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
			}
		}

		/// <summary>Gets a value indicating whether the visibility of this method or constructor is described by <see cref="F:System.Reflection.MethodAttributes.Family" />; that is, the method or constructor is visible only within its class and derived classes.</summary>
		/// <returns>
		///   <see langword="true" /> if access to this method or constructor is exactly described by <see cref="F:System.Reflection.MethodAttributes.Family" />; otherwise, <see langword="false" />.</returns>
		public bool IsFamily
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
			}
		}

		/// <summary>Gets a value indicating whether the visibility of this method or constructor is described by <see cref="F:System.Reflection.MethodAttributes.FamANDAssem" />; that is, the method or constructor can be called by derived classes, but only if they are in the same assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if access to this method or constructor is exactly described by <see cref="F:System.Reflection.MethodAttributes.FamANDAssem" />; otherwise, <see langword="false" />.</returns>
		public bool IsFamilyAndAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
			}
		}

		/// <summary>Gets a value indicating whether the potential visibility of this method or constructor is described by <see cref="F:System.Reflection.MethodAttributes.FamORAssem" />; that is, the method or constructor can be called by derived classes wherever they are, and by classes in the same assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if access to this method or constructor is exactly described by <see cref="F:System.Reflection.MethodAttributes.FamORAssem" />; otherwise, <see langword="false" />.</returns>
		public bool IsFamilyOrAssembly
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;
			}
		}

		/// <summary>Gets a value indicating whether this member is private.</summary>
		/// <returns>
		///   <see langword="true" /> if access to this method is restricted to other members of the class itself; otherwise, <see langword="false" />.</returns>
		public bool IsPrivate
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
			}
		}

		/// <summary>Gets a value indicating whether this is a public method.</summary>
		/// <returns>
		///   <see langword="true" /> if this method is public; otherwise, <see langword="false" />.</returns>
		public bool IsPublic
		{
			get
			{
				return (this.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
			}
		}

		public virtual bool IsConstructedGenericMethod
		{
			get
			{
				return this.IsGenericMethod && !this.IsGenericMethodDefinition;
			}
		}

		/// <summary>Gets a value indicating whether the method is generic.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Reflection.MethodBase" /> represents a generic method; otherwise, <see langword="false" />.</returns>
		public virtual bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the method is a generic method definition.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Reflection.MethodBase" /> object represents the definition of a generic method; otherwise, <see langword="false" />.</returns>
		public virtual bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		/// <summary>Returns an array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic method or the type parameters of a generic method definition.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic method or the type parameters of a generic method definition. Returns an empty array if the current method is not a generic method.</returns>
		/// <exception cref="T:System.NotSupportedException">The current object is a <see cref="T:System.Reflection.ConstructorInfo" />. Generic constructors are not supported in the .NET Framework version 2.0. This exception is the default behavior if this method is not overridden in a derived class.</exception>
		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Gets a value indicating whether the generic method contains unassigned generic type parameters.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Reflection.MethodBase" /> object represents a generic method that contains unassigned generic type parameters; otherwise, <see langword="false" />.</returns>
		public virtual bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		/// <summary>Invokes the method or constructor represented by the current instance, using the specified parameters.</summary>
		/// <param name="obj">The object on which to invoke the method or constructor. If a method is static, this argument is ignored. If a constructor is static, this argument must be <see langword="null" /> or an instance of the class that defines the constructor.</param>
		/// <param name="parameters">An argument list for the invoked method or constructor. This is an array of objects with the same number, order, and type as the parameters of the method or constructor to be invoked. If there are no parameters, <paramref name="parameters" /> should be <see langword="null" />.  
		///  If the method or constructor represented by this instance takes a <see langword="ref" /> parameter (<see langword="ByRef" /> in Visual Basic), no special attribute is required for that parameter in order to invoke the method or constructor using this function. Any object in this array that is not explicitly initialized with a value will contain the default value for that object type. For reference-type elements, this value is <see langword="null" />. For value-type elements, this value is 0, 0.0, or <see langword="false" />, depending on the specific element type.</param>
		/// <returns>An object containing the return value of the invoked method, or <see langword="null" /> in the case of a constructor.</returns>
		/// <exception cref="T:System.Reflection.TargetException">In the.NET for Windows Store apps or the Portable Class Library, catch <see cref="T:System.Exception" /> instead.  
		///
		///
		///
		///
		///  The <paramref name="obj" /> parameter is <see langword="null" /> and the method is not static.  
		///  -or-  
		///  The method is not declared or inherited by the class of <paramref name="obj" />.  
		///  -or-  
		///  A static constructor is invoked, and <paramref name="obj" /> is neither <see langword="null" /> nor an instance of the class that declared the constructor.</exception>
		/// <exception cref="T:System.ArgumentException">The elements of the <paramref name="parameters" /> array do not match the signature of the method or constructor reflected by this instance.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The invoked method or constructor throws an exception.  
		///  -or-  
		///  The current instance is a <see cref="T:System.Reflection.Emit.DynamicMethod" /> that contains unverifiable code. See the "Verification" section in Remarks for <see cref="T:System.Reflection.Emit.DynamicMethod" />.</exception>
		/// <exception cref="T:System.Reflection.TargetParameterCountException">The <paramref name="parameters" /> array does not have the correct number of arguments.</exception>
		/// <exception cref="T:System.MethodAccessException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.MemberAccessException" />, instead.  
		///
		///
		///
		///
		///  The caller does not have permission to execute the method or constructor that is represented by the current instance.</exception>
		/// <exception cref="T:System.InvalidOperationException">The type that declares the method is an open generic type. That is, the <see cref="P:System.Type.ContainsGenericParameters" /> property returns <see langword="true" /> for the declaring type.</exception>
		/// <exception cref="T:System.NotSupportedException">The current instance is a <see cref="T:System.Reflection.Emit.MethodBuilder" />.</exception>
		[DebuggerStepThrough]
		[DebuggerHidden]
		public object Invoke(object obj, object[] parameters)
		{
			return this.Invoke(obj, BindingFlags.Default, null, parameters, null);
		}

		/// <summary>When overridden in a derived class, invokes the reflected method or constructor with the given parameters.</summary>
		/// <param name="obj">The object on which to invoke the method or constructor. If a method is static, this argument is ignored. If a constructor is static, this argument must be <see langword="null" /> or an instance of the class that defines the constructor.</param>
		/// <param name="invokeAttr">A bitmask that is a combination of 0 or more bit flags from <see cref="T:System.Reflection.BindingFlags" />. If <paramref name="binder" /> is <see langword="null" />, this parameter is assigned the value <see cref="F:System.Reflection.BindingFlags.Default" />; thus, whatever you pass in is ignored.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see langword="MemberInfo" /> objects via reflection. If <paramref name="binder" /> is <see langword="null" />, the default binder is used.</param>
		/// <param name="parameters">An argument list for the invoked method or constructor. This is an array of objects with the same number, order, and type as the parameters of the method or constructor to be invoked. If there are no parameters, this should be <see langword="null" />.  
		///  If the method or constructor represented by this instance takes a ByRef parameter, there is no special attribute required for that parameter in order to invoke the method or constructor using this function. Any object in this array that is not explicitly initialized with a value will contain the default value for that object type. For reference-type elements, this value is <see langword="null" />. For value-type elements, this value is 0, 0.0, or <see langword="false" />, depending on the specific element type.</param>
		/// <param name="culture">An instance of <see langword="CultureInfo" /> used to govern the coercion of types. If this is <see langword="null" />, the <see langword="CultureInfo" /> for the current thread is used. (This is necessary to convert a <see langword="String" /> that represents 1000 to a <see langword="Double" /> value, for example, since 1000 is represented differently by different cultures.)</param>
		/// <returns>An <see langword="Object" /> containing the return value of the invoked method, or <see langword="null" /> in the case of a constructor, or <see langword="null" /> if the method's return type is <see langword="void" />. Before calling the method or constructor, <see langword="Invoke" /> checks to see if the user has access permission and verifies that the parameters are valid.</returns>
		/// <exception cref="T:System.Reflection.TargetException">The <paramref name="obj" /> parameter is <see langword="null" /> and the method is not static.  
		///  -or-  
		///  The method is not declared or inherited by the class of <paramref name="obj" />.  
		///  -or-  
		///  A static constructor is invoked, and <paramref name="obj" /> is neither <see langword="null" /> nor an instance of the class that declared the constructor.</exception>
		/// <exception cref="T:System.ArgumentException">The type of the <paramref name="parameters" /> parameter does not match the signature of the method or constructor reflected by this instance.</exception>
		/// <exception cref="T:System.Reflection.TargetParameterCountException">The <paramref name="parameters" /> array does not have the correct number of arguments.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The invoked method or constructor throws an exception.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to execute the method or constructor that is represented by the current instance.</exception>
		/// <exception cref="T:System.InvalidOperationException">The type that declares the method is an open generic type. That is, the <see cref="P:System.Type.ContainsGenericParameters" /> property returns <see langword="true" /> for the declaring type.</exception>
		public abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);

		/// <summary>Gets a handle to the internal metadata representation of a method.</summary>
		/// <returns>A <see cref="T:System.RuntimeMethodHandle" /> object.</returns>
		public abstract RuntimeMethodHandle MethodHandle { get; }

		/// <summary>Gets a value that indicates whether the current method or constructor is security-critical or security-safe-critical at the current trust level, and therefore can perform critical operations.</summary>
		/// <returns>
		///   <see langword="true" /> if the current method or constructor is security-critical or security-safe-critical at the current trust level; <see langword="false" /> if it is transparent.</returns>
		public virtual bool IsSecurityCritical
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a value that indicates whether the current method or constructor is security-safe-critical at the current trust level; that is, whether it can perform critical operations and can be accessed by transparent code.</summary>
		/// <returns>
		///   <see langword="true" /> if the method or constructor is security-safe-critical at the current trust level; <see langword="false" /> if it is security-critical or transparent.</returns>
		public virtual bool IsSecuritySafeCritical
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a value that indicates whether the current method or constructor is transparent at the current trust level, and therefore cannot perform critical operations.</summary>
		/// <returns>
		///   <see langword="true" /> if the method or constructor is security-transparent at the current trust level; otherwise, <see langword="false" />.</returns>
		public virtual bool IsSecurityTransparent
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An object to compare with this instance, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> equals the type and value of this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.MethodBase" /> objects are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(MethodBase left, MethodBase right)
		{
			if (left == right)
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			MethodInfo left2;
			MethodInfo right2;
			if ((left2 = (left as MethodInfo)) != null && (right2 = (right as MethodInfo)) != null)
			{
				return left2 == right2;
			}
			ConstructorInfo left3;
			ConstructorInfo right3;
			return (left3 = (left as ConstructorInfo)) != null && (right3 = (right as ConstructorInfo)) != null && left3 == right3;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.MethodBase" /> objects are not equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(MethodBase left, MethodBase right)
		{
			return !(left == right);
		}

		internal virtual ParameterInfo[] GetParametersInternal()
		{
			return this.GetParameters();
		}

		internal virtual int GetParametersCount()
		{
			return this.GetParametersInternal().Length;
		}

		internal virtual Type GetParameterType(int pos)
		{
			throw new NotImplementedException();
		}

		internal virtual int get_next_table_index(object obj, int table, int count)
		{
			if (this is MethodBuilder)
			{
				return ((MethodBuilder)this).get_next_table_index(obj, table, count);
			}
			if (this is ConstructorBuilder)
			{
				return ((ConstructorBuilder)this).get_next_table_index(obj, table, count);
			}
			throw new Exception("Method is not a builder method");
		}

		internal virtual string FormatNameAndSig(bool serialization)
		{
			StringBuilder stringBuilder = new StringBuilder(this.Name);
			stringBuilder.Append("(");
			stringBuilder.Append(MethodBase.ConstructParameters(this.GetParameterTypes(), this.CallingConvention, serialization));
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		internal virtual Type[] GetParameterTypes()
		{
			ParameterInfo[] parametersNoCopy = this.GetParametersNoCopy();
			Type[] array = new Type[parametersNoCopy.Length];
			for (int i = 0; i < parametersNoCopy.Length; i++)
			{
				array[i] = parametersNoCopy[i].ParameterType;
			}
			return array;
		}

		internal virtual ParameterInfo[] GetParametersNoCopy()
		{
			return this.GetParameters();
		}

		/// <summary>Gets method information by using the method's internal metadata representation (handle).</summary>
		/// <param name="handle">The method's handle.</param>
		/// <returns>A <see langword="MethodBase" /> containing information about the method.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="handle" /> is invalid.</exception>
		public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle)
		{
			if (handle.IsNullHandle())
			{
				throw new ArgumentException(Environment.GetResourceString("The handle is invalid."));
			}
			MethodBase methodFromHandleInternalType = RuntimeMethodInfo.GetMethodFromHandleInternalType(handle.Value, IntPtr.Zero);
			if (methodFromHandleInternalType == null)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			Type declaringType = methodFromHandleInternalType.DeclaringType;
			if (declaringType != null && declaringType.IsGenericType)
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Cannot resolve method {0} because the declaring type of the method handle {1} is generic. Explicitly provide the declaring type to GetMethodFromHandle."), methodFromHandleInternalType, declaringType.GetGenericTypeDefinition()));
			}
			return methodFromHandleInternalType;
		}

		/// <summary>Gets a <see cref="T:System.Reflection.MethodBase" /> object for the constructor or method represented by the specified handle, for the specified generic type.</summary>
		/// <param name="handle">A handle to the internal metadata representation of a constructor or method.</param>
		/// <param name="declaringType">A handle to the generic type that defines the constructor or method.</param>
		/// <returns>A <see cref="T:System.Reflection.MethodBase" /> object representing the method or constructor specified by <paramref name="handle" />, in the generic type specified by <paramref name="declaringType" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="handle" /> is invalid.</exception>
		[ComVisible(false)]
		public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle, RuntimeTypeHandle declaringType)
		{
			if (handle.IsNullHandle())
			{
				throw new ArgumentException(Environment.GetResourceString("The handle is invalid."));
			}
			MethodBase methodFromHandleInternalType = RuntimeMethodInfo.GetMethodFromHandleInternalType(handle.Value, declaringType.Value);
			if (methodFromHandleInternalType == null)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			return methodFromHandleInternalType;
		}

		internal static string ConstructParameters(Type[] parameterTypes, CallingConventions callingConvention, bool serialization)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = "";
			foreach (Type type in parameterTypes)
			{
				stringBuilder.Append(value);
				string text = type.FormatTypeName(serialization);
				if (type.IsByRef && !serialization)
				{
					stringBuilder.Append(text.TrimEnd(new char[]
					{
						'&'
					}));
					stringBuilder.Append(" ByRef");
				}
				else
				{
					stringBuilder.Append(text);
				}
				value = ", ";
			}
			if ((callingConvention & CallingConventions.VarArgs) == CallingConventions.VarArgs)
			{
				stringBuilder.Append(value);
				stringBuilder.Append("...");
			}
			return stringBuilder.ToString();
		}

		/// <summary>Returns a <see langword="MethodBase" /> object representing the currently executing method.</summary>
		/// <returns>
		///   <see cref="M:System.Reflection.MethodBase.GetCurrentMethod" /> is a static method that is called from within an executing method and that returns information about that method.  
		/// A <see langword="MethodBase" /> object representing the currently executing method.</returns>
		/// <exception cref="T:System.Reflection.TargetException">This member was invoked with a late-binding mechanism.</exception>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodBase GetCurrentMethod();

		/// <summary>Maps a set of names to a corresponding set of dispatch identifiers.</summary>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="rgszNames">Passed-in array of names to be mapped.</param>
		/// <param name="cNames">Count of the names to be mapped.</param>
		/// <param name="lcid">The locale context in which to interpret the names.</param>
		/// <param name="rgDispId">Caller-allocated array which receives the IDs corresponding to the names.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _MethodBase.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Runtime.InteropServices._MethodBase.GetType" />.</summary>
		/// <returns>For a description of this member, see <see cref="M:System.Runtime.InteropServices._MethodBase.GetType" />.</returns>
		Type _MethodBase.GetType()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Retrieves the type information for an object, which can then be used to get the type information for an interface.</summary>
		/// <param name="iTInfo">The type information to return.</param>
		/// <param name="lcid">The locale identifier for the type information.</param>
		/// <param name="ppTInfo">Receives a pointer to the requested type information object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _MethodBase.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Retrieves the number of type information interfaces that an object provides (either 0 or 1).</summary>
		/// <param name="pcTInfo">Points to a location that receives the number of type information interfaces provided by the object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _MethodBase.GetTypeInfoCount(out uint pcTInfo)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Provides access to properties and methods exposed by an object.</summary>
		/// <param name="dispIdMember">Identifies the member.</param>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="lcid">The locale context in which to interpret arguments.</param>
		/// <param name="wFlags">Flags describing the context of the call.</param>
		/// <param name="pDispParams">Pointer to a structure containing an array of arguments, an array of argument DISPIDs for named arguments, and counts for the number of elements in the arrays.</param>
		/// <param name="pVarResult">Pointer to the location where the result is to be stored.</param>
		/// <param name="pExcepInfo">Pointer to a structure that contains exception information.</param>
		/// <param name="puArgErr">The index of the first argument that has an error.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _MethodBase.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
