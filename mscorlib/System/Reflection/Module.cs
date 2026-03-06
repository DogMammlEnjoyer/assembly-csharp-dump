using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Unity;

namespace System.Reflection
{
	/// <summary>Performs reflection on a module.</summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class Module : ICustomAttributeProvider, ISerializable, _Module
	{
		/// <summary>Gets the appropriate <see cref="T:System.Reflection.Assembly" /> for this instance of <see cref="T:System.Reflection.Module" />.</summary>
		/// <returns>An <see langword="Assembly" /> object.</returns>
		public virtual Assembly Assembly
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a string representing the fully qualified name and path to this module.</summary>
		/// <returns>The fully qualified module name.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permissions.</exception>
		public virtual string FullyQualifiedName
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a <see langword="String" /> representing the name of the module with the path removed.</summary>
		/// <returns>The module name with no path.</returns>
		public virtual string Name
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets the metadata stream version.</summary>
		/// <returns>A 32-bit integer representing the metadata stream version. The high-order two bytes represent the major version number, and the low-order two bytes represent the minor version number.</returns>
		public virtual int MDStreamVersion
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a universally unique identifier (UUID) that can be used to distinguish between two versions of a module.</summary>
		/// <returns>A <see cref="T:System.Guid" /> that can be used to distinguish between two versions of a module.</returns>
		public virtual Guid ModuleVersionId
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a string representing the name of the module.</summary>
		/// <returns>The module name.</returns>
		public virtual string ScopeName
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a handle for the module.</summary>
		/// <returns>A <see cref="T:System.ModuleHandle" /> structure for the current module.</returns>
		public ModuleHandle ModuleHandle
		{
			get
			{
				return this.GetModuleHandleImpl();
			}
		}

		internal virtual ModuleHandle GetModuleHandleImpl()
		{
			return ModuleHandle.EmptyHandle;
		}

		/// <summary>Gets a pair of values indicating the nature of the code in a module and the platform targeted by the module.</summary>
		/// <param name="peKind">When this method returns, a combination of the <see cref="T:System.Reflection.PortableExecutableKinds" /> values indicating the nature of the code in the module.</param>
		/// <param name="machine">When this method returns, one of the <see cref="T:System.Reflection.ImageFileMachine" /> values indicating the platform targeted by the module.</param>
		public virtual void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Gets a value indicating whether the object is a resource.</summary>
		/// <returns>
		///   <see langword="true" /> if the object is a resource; otherwise, <see langword="false" />.</returns>
		public virtual bool IsResource()
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns a value that indicates whether the specified attribute type has been applied to this module.</summary>
		/// <param name="attributeType">The type of custom attribute to test for.</param>
		/// <param name="inherit">This argument is ignored for objects of this type.</param>
		/// <returns>
		///   <see langword="true" /> if one or more instances of <paramref name="attributeType" /> have been applied to this module; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="attributeType" /> is not a <see cref="T:System.Type" /> object supplied by the runtime. For example, <paramref name="attributeType" /> is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> object.</exception>
		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Gets a collection that contains this module's custom attributes.</summary>
		/// <returns>A collection that contains this module's custom attributes.</returns>
		public virtual IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				return this.GetCustomAttributesData();
			}
		}

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects for the current module, which can be used in the reflection-only context.</summary>
		/// <returns>A generic list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the current module.</returns>
		public virtual IList<CustomAttributeData> GetCustomAttributesData()
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns all custom attributes.</summary>
		/// <param name="inherit">This argument is ignored for objects of this type.</param>
		/// <returns>An array of type <see langword="Object" /> containing all custom attributes.</returns>
		public virtual object[] GetCustomAttributes(bool inherit)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Gets custom attributes of the specified type.</summary>
		/// <param name="attributeType">The type of attribute to get.</param>
		/// <param name="inherit">This argument is ignored for objects of this type.</param>
		/// <returns>An array of type <see langword="Object" /> containing all custom attributes of the specified type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="attributeType" /> is not a <see cref="T:System.Type" /> object supplied by the runtime. For example, <paramref name="attributeType" /> is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> object.</exception>
		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns a method having the specified name.</summary>
		/// <param name="name">The method name.</param>
		/// <returns>A <see langword="MethodInfo" /> object having the specified name, or <see langword="null" /> if the method does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public MethodInfo GetMethod(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
		}

		/// <summary>Returns a method having the specified name and parameter types.</summary>
		/// <param name="name">The method name.</param>
		/// <param name="types">The parameter types to search for.</param>
		/// <returns>A <see langword="MethodInfo" /> object in accordance with the specified criteria, or <see langword="null" /> if the method does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />, <paramref name="types" /> is <see langword="null" />, or <paramref name="types" /> (i) is <see langword="null" />.</exception>
		public MethodInfo GetMethod(string name, Type[] types)
		{
			return this.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		/// <summary>Returns a method having the specified name, binding information, calling convention, and parameter types and modifiers.</summary>
		/// <param name="name">The method name.</param>
		/// <param name="bindingAttr">One of the <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <param name="binder">An object that implements <see langword="Binder" />, containing properties related to this method.</param>
		/// <param name="callConvention">The calling convention for the method.</param>
		/// <param name="types">The parameter types to search for.</param>
		/// <param name="modifiers">An array of parameter modifiers used to make binding work with parameter signatures in which the types have been modified.</param>
		/// <returns>A <see langword="MethodInfo" /> object in accordance with the specified criteria, or <see langword="null" /> if the method does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />, <paramref name="types" /> is <see langword="null" />, or <paramref name="types" /> (i) is <see langword="null" />.</exception>
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		/// <summary>Returns the method implementation in accordance with the specified criteria.</summary>
		/// <param name="name">The method name.</param>
		/// <param name="bindingAttr">One of the <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <param name="binder">An object that implements <see langword="Binder" />, containing properties related to this method.</param>
		/// <param name="callConvention">The calling convention for the method.</param>
		/// <param name="types">The parameter types to search for.</param>
		/// <param name="modifiers">An array of parameter modifiers used to make binding work with parameter signatures in which the types have been modified.</param>
		/// <returns>A <see langword="MethodInfo" /> object containing implementation information as specified, or <see langword="null" /> if the method does not exist.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">
		///   <paramref name="types" /> is <see langword="null" />.</exception>
		protected virtual MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the global methods defined on the module.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all the global methods defined on the module; if there are no global methods, an empty array is returned.</returns>
		public MethodInfo[] GetMethods()
		{
			return this.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Returns the global methods defined on the module that match the specified binding flags.</summary>
		/// <param name="bindingFlags">A bitwise combination of <see cref="T:System.Reflection.BindingFlags" /> values that limit the search.</param>
		/// <returns>An array of type <see cref="T:System.Reflection.MethodInfo" /> representing the global methods defined on the module that match the specified binding flags; if no global methods match the binding flags, an empty array is returned.</returns>
		public virtual MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns a field having the specified name.</summary>
		/// <param name="name">The field name.</param>
		/// <returns>A <see langword="FieldInfo" /> object having the specified name, or <see langword="null" /> if the field does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		public FieldInfo GetField(string name)
		{
			return this.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Returns a field having the specified name and binding attributes.</summary>
		/// <param name="name">The field name.</param>
		/// <param name="bindingAttr">One of the <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <returns>A <see langword="FieldInfo" /> object having the specified name and binding attributes, or <see langword="null" /> if the field does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		public virtual FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the global fields defined on the module.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing the global fields defined on the module; if there are no global fields, an empty array is returned.</returns>
		public FieldInfo[] GetFields()
		{
			return this.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Returns the global fields defined on the module that match the specified binding flags.</summary>
		/// <param name="bindingFlags">A bitwise combination of <see cref="T:System.Reflection.BindingFlags" /> values that limit the search.</param>
		/// <returns>An array of type <see cref="T:System.Reflection.FieldInfo" /> representing the global fields defined on the module that match the specified binding flags; if no global fields match the binding flags, an empty array is returned.</returns>
		public virtual FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns all the types defined within this module.</summary>
		/// <returns>An array of type <see langword="Type" /> containing types defined within the module that is reflected by this instance.</returns>
		/// <exception cref="T:System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public virtual Type[] GetTypes()
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the specified type, performing a case-sensitive search.</summary>
		/// <param name="className">The name of the type to locate. The name must be fully qualified with the namespace.</param>
		/// <returns>A <see langword="Type" /> object representing the given type, if the type is in this module; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="className" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The class initializers are invoked and an exception is thrown.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="className" /> is a zero-length string.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="className" /> requires a dependent assembly that could not be found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">
		///   <paramref name="className" /> requires a dependent assembly that was found but could not be loaded.  
		/// -or-  
		/// The current assembly was loaded into the reflection-only context, and <paramref name="className" /> requires a dependent assembly that was not preloaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="className" /> requires a dependent assembly, but the file is not a valid assembly.  
		/// -or-  
		/// <paramref name="className" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
		public virtual Type GetType(string className)
		{
			return this.GetType(className, false, false);
		}

		/// <summary>Returns the specified type, searching the module with the specified case sensitivity.</summary>
		/// <param name="className">The name of the type to locate. The name must be fully qualified with the namespace.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> for case-insensitive search; otherwise, <see langword="false" />.</param>
		/// <returns>A <see langword="Type" /> object representing the given type, if the type is in this module; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="className" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The class initializers are invoked and an exception is thrown.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="className" /> is a zero-length string.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="className" /> requires a dependent assembly that could not be found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">
		///   <paramref name="className" /> requires a dependent assembly that was found but could not be loaded.  
		/// -or-  
		/// The current assembly was loaded into the reflection-only context, and <paramref name="className" /> requires a dependent assembly that was not preloaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="className" /> requires a dependent assembly, but the file is not a valid assembly.  
		/// -or-  
		/// <paramref name="className" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
		public virtual Type GetType(string className, bool ignoreCase)
		{
			return this.GetType(className, false, ignoreCase);
		}

		/// <summary>Returns the specified type, specifying whether to make a case-sensitive search of the module and whether to throw an exception if the type cannot be found.</summary>
		/// <param name="className">The name of the type to locate. The name must be fully qualified with the namespace.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> for case-insensitive search; otherwise, <see langword="false" />.</param>
		/// <returns>A <see cref="T:System.Type" /> object representing the specified type, if the type is declared in this module; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="className" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The class initializers are invoked and an exception is thrown.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="className" /> is a zero-length string.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwOnError" /> is <see langword="true" />, and the type cannot be found.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="className" /> requires a dependent assembly that could not be found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">
		///   <paramref name="className" /> requires a dependent assembly that was found but could not be loaded.  
		/// -or-  
		/// The current assembly was loaded into the reflection-only context, and <paramref name="className" /> requires a dependent assembly that was not preloaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="className" /> requires a dependent assembly, but the file is not a valid assembly.  
		/// -or-  
		/// <paramref name="className" /> requires a dependent assembly which was compiled for a version of the runtime later than the currently loaded version.</exception>
		public virtual Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns an array of classes accepted by the given filter and filter criteria.</summary>
		/// <param name="filter">The delegate used to filter the classes.</param>
		/// <param name="filterCriteria">An Object used to filter the classes.</param>
		/// <returns>An array of type <see langword="Type" /> containing classes that were accepted by the filter.</returns>
		/// <exception cref="T:System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
		public virtual Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			Type[] types = this.GetTypes();
			int num = 0;
			for (int i = 0; i < types.Length; i++)
			{
				if (filter != null && !filter(types[i], filterCriteria))
				{
					types[i] = null;
				}
				else
				{
					num++;
				}
			}
			if (num == types.Length)
			{
				return types;
			}
			Type[] array = new Type[num];
			num = 0;
			for (int j = 0; j < types.Length; j++)
			{
				if (types[j] != null)
				{
					array[num++] = types[j];
				}
			}
			return array;
		}

		/// <summary>Gets a token that identifies the module in metadata.</summary>
		/// <returns>An integer token that identifies the current module in metadata.</returns>
		public virtual int MetadataToken
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Returns the field identified by the specified metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a field in the module.</param>
		/// <returns>A <see cref="T:System.Reflection.FieldInfo" /> object representing the field that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a field in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> identifies a field whose parent <see langword="TypeSpec" /> has a signature containing element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method).</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public FieldInfo ResolveField(int metadataToken)
		{
			return this.ResolveField(metadataToken, null, null);
		}

		/// <summary>Returns the field identified by the specified metadata token, in the context defined by the specified generic type parameters.</summary>
		/// <param name="metadataToken">A metadata token that identifies a field in the module.</param>
		/// <param name="genericTypeArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the type where the token is in scope, or <see langword="null" /> if that type is not generic.</param>
		/// <param name="genericMethodArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the method where the token is in scope, or <see langword="null" /> if that method is not generic.</param>
		/// <returns>A <see cref="T:System.Reflection.FieldInfo" /> object representing the field that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a field in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> identifies a field whose parent <see langword="TypeSpec" /> has a signature containing element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method), and the necessary generic type arguments were not supplied for either or both of <paramref name="genericTypeArguments" /> and <paramref name="genericMethodArguments" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public virtual FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the type or member identified by the specified metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a type or member in the module.</param>
		/// <returns>A <see cref="T:System.Reflection.MemberInfo" /> object representing the type or member that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a type or member in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> is a <see langword="MethodSpec" /> or <see langword="TypeSpec" /> whose signature contains element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method).  
		/// -or-  
		/// <paramref name="metadataToken" /> identifies a property or event.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public MemberInfo ResolveMember(int metadataToken)
		{
			return this.ResolveMember(metadataToken, null, null);
		}

		/// <summary>Returns the type or member identified by the specified metadata token, in the context defined by the specified generic type parameters.</summary>
		/// <param name="metadataToken">A metadata token that identifies a type or member in the module.</param>
		/// <param name="genericTypeArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the type where the token is in scope, or <see langword="null" /> if that type is not generic.</param>
		/// <param name="genericMethodArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the method where the token is in scope, or <see langword="null" /> if that method is not generic.</param>
		/// <returns>A <see cref="T:System.Reflection.MemberInfo" /> object representing the type or member that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a type or member in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> is a <see langword="MethodSpec" /> or <see langword="TypeSpec" /> whose signature contains element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method), and the necessary generic type arguments were not supplied for either or both of <paramref name="genericTypeArguments" /> and <paramref name="genericMethodArguments" />.  
		/// -or-  
		/// <paramref name="metadataToken" /> identifies a property or event.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public virtual MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the method or constructor identified by the specified metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a method or constructor in the module.</param>
		/// <returns>A <see cref="T:System.Reflection.MethodBase" /> object representing the method or constructor that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a method or constructor in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> is a <see langword="MethodSpec" /> whose signature contains element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method).</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public MethodBase ResolveMethod(int metadataToken)
		{
			return this.ResolveMethod(metadataToken, null, null);
		}

		/// <summary>Returns the method or constructor identified by the specified metadata token, in the context defined by the specified generic type parameters.</summary>
		/// <param name="metadataToken">A metadata token that identifies a method or constructor in the module.</param>
		/// <param name="genericTypeArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the type where the token is in scope, or <see langword="null" /> if that type is not generic.</param>
		/// <param name="genericMethodArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the method where the token is in scope, or <see langword="null" /> if that method is not generic.</param>
		/// <returns>A <see cref="T:System.Reflection.MethodBase" /> object representing the method that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a method or constructor in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> is a <see langword="MethodSpec" /> whose signature contains element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method), and the necessary generic type arguments were not supplied for either or both of <paramref name="genericTypeArguments" /> and <paramref name="genericMethodArguments" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public virtual MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the signature blob identified by a metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a signature in the module.</param>
		/// <returns>An array of bytes representing the signature blob.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a valid <see langword="MemberRef" />, <see langword="MethodDef" />, <see langword="TypeSpec" />, signature, or <see langword="FieldDef" /> token in the scope of the current module.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public virtual byte[] ResolveSignature(int metadataToken)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the string identified by the specified metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a string in the string heap of the module.</param>
		/// <returns>A <see cref="T:System.String" /> containing a string value from the metadata string heap.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a string in the scope of the current module.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public virtual string ResolveString(int metadataToken)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns the type identified by the specified metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a type in the module.</param>
		/// <returns>A <see cref="T:System.Type" /> object representing the type that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a type in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> is a <see langword="TypeSpec" /> whose signature contains element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method).</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public Type ResolveType(int metadataToken)
		{
			return this.ResolveType(metadataToken, null, null);
		}

		/// <summary>Returns the type identified by the specified metadata token, in the context defined by the specified generic type parameters.</summary>
		/// <param name="metadataToken">A metadata token that identifies a type in the module.</param>
		/// <param name="genericTypeArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the type where the token is in scope, or <see langword="null" /> if that type is not generic.</param>
		/// <param name="genericMethodArguments">An array of <see cref="T:System.Type" /> objects representing the generic type arguments of the method where the token is in scope, or <see langword="null" /> if that method is not generic.</param>
		/// <returns>A <see cref="T:System.Type" /> object representing the type that is identified by the specified metadata token.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a type in the scope of the current module.  
		/// -or-  
		/// <paramref name="metadataToken" /> is a <see langword="TypeSpec" /> whose signature contains element type <see langword="var" /> (a type parameter of a generic type) or <see langword="mvar" /> (a type parameter of a generic method), and the necessary generic type arguments were not supplied for either or both of <paramref name="genericTypeArguments" /> and <paramref name="genericMethodArguments" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public virtual Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Provides an <see cref="T:System.Runtime.Serialization.ISerializable" /> implementation for serialized objects.</summary>
		/// <param name="info">The information and data needed to serialize or deserialize an object.</param>
		/// <param name="context">The context for the serialization.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Determines whether this module and the specified object are equal.</summary>
		/// <param name="o">The object to compare with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="o" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Module" /> objects are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(Module left, Module right)
		{
			return left == right || (left != null && right != null && left.Equals(right));
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Module" /> objects are not equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(Module left, Module right)
		{
			return !(left == right);
		}

		/// <summary>Returns the name of the module.</summary>
		/// <returns>A <see langword="String" /> representing the name of this module.</returns>
		public override string ToString()
		{
			return this.ScopeName;
		}

		private static bool FilterTypeNameImpl(Type cls, object filterCriteria)
		{
			if (filterCriteria == null || !(filterCriteria is string))
			{
				throw new InvalidFilterCriteriaException("A String must be provided for the filter criteria.");
			}
			string text = (string)filterCriteria;
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				text = text.Substring(0, text.Length - 1);
				return cls.Name.StartsWith(text, StringComparison.Ordinal);
			}
			return cls.Name.Equals(text);
		}

		private static bool FilterTypeNameIgnoreCaseImpl(Type cls, object filterCriteria)
		{
			if (filterCriteria == null || !(filterCriteria is string))
			{
				throw new InvalidFilterCriteriaException("A String must be provided for the filter criteria.");
			}
			string text = (string)filterCriteria;
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				text = text.Substring(0, text.Length - 1);
				string name = cls.Name;
				return name.Length >= text.Length && string.Compare(name, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
			}
			return string.Compare(text, cls.Name, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal Guid MvId
		{
			get
			{
				return this.GetModuleVersionId();
			}
		}

		internal static Guid Mono_GetGuid(Module module)
		{
			return module.GetModuleVersionId();
		}

		internal virtual Guid GetModuleVersionId()
		{
			throw new NotImplementedException();
		}

		/// <summary>Returns an <see langword="X509Certificate" /> object corresponding to the certificate included in the Authenticode signature of the assembly which this module belongs to. If the assembly has not been Authenticode signed, <see langword="null" /> is returned.</summary>
		/// <returns>An <see langword="X509Certificate" /> object, or <see langword="null" /> if the assembly to which this module belongs has not been Authenticode signed.</returns>
		public virtual X509Certificate GetSignerCertificate()
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Maps a set of names to a corresponding set of dispatch identifiers.</summary>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="rgszNames">Passed-in array of names to be mapped.</param>
		/// <param name="cNames">Count of the names to be mapped.</param>
		/// <param name="lcid">The locale context in which to interpret the names.</param>
		/// <param name="rgDispId">Caller-allocated array that receives the IDs corresponding to the names.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Module.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Retrieves the type information for an object, which can then be used to get the type information for an interface.</summary>
		/// <param name="iTInfo">The type information to return.</param>
		/// <param name="lcid">The locale identifier for the type information.</param>
		/// <param name="ppTInfo">Receives a pointer to the requested type information object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Module.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Retrieves the number of type information interfaces that an object provides (either 0 or 1).</summary>
		/// <param name="pcTInfo">Points to a location that receives the number of type information interfaces provided by the object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Module.GetTypeInfoCount(out uint pcTInfo)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>Provides access to properties and methods exposed by an object.</summary>
		/// <param name="dispIdMember">Identifies the member.</param>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="lcid">The locale context in which to interpret arguments.</param>
		/// <param name="wFlags">Flags describing the context of the call.</param>
		/// <param name="pDispParams">Pointer to a structure containing an array of arguments, an array of argument DispIDs for named arguments, and counts for the number of elements in the arrays.</param>
		/// <param name="pVarResult">Pointer to the location where the result is to be stored.</param>
		/// <param name="pExcepInfo">Pointer to a structure that contains exception information.</param>
		/// <param name="puArgErr">The index of the first argument that has an error.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Module.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		/// <summary>A <see langword="TypeFilter" /> object that filters the list of types defined in this module based upon the name. This field is case-sensitive and read-only.</summary>
		public static readonly TypeFilter FilterTypeName = new TypeFilter(Module.FilterTypeNameImpl);

		/// <summary>A <see langword="TypeFilter" /> object that filters the list of types defined in this module based upon the name. This field is case-insensitive and read-only.</summary>
		public static readonly TypeFilter FilterTypeNameIgnoreCase = new TypeFilter(Module.FilterTypeNameIgnoreCaseImpl);

		private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
	}
}
