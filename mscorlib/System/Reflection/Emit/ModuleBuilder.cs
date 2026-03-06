using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity;

namespace System.Reflection.Emit
{
	/// <summary>Defines and represents a module in a dynamic assembly.</summary>
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ModuleBuilder))]
	[ClassInterface(ClassInterfaceType.None)]
	[StructLayout(LayoutKind.Sequential)]
	public class ModuleBuilder : Module, _ModuleBuilder
	{
		/// <summary>For a description of this member, see <see cref="M:System.Runtime.InteropServices._ModuleBuilder.GetIDsOfNames(System.Guid@,System.IntPtr,System.UInt32,System.UInt32,System.IntPtr)" />.</summary>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="rgszNames">Passed-in array of names to be mapped.</param>
		/// <param name="cNames">Count of the names to be mapped.</param>
		/// <param name="lcid">The locale context in which to interpret the names.</param>
		/// <param name="rgDispId">Caller-allocated array which receives the IDs corresponding to the names.</param>
		/// <exception cref="T:System.NotImplementedException">The method is called late-bound using the COM IDispatch interface.</exception>
		void _ModuleBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Runtime.InteropServices._ModuleBuilder.GetTypeInfo(System.UInt32,System.UInt32,System.IntPtr)" />.</summary>
		/// <param name="iTInfo">The type information to return.</param>
		/// <param name="lcid">The locale identifier for the type information.</param>
		/// <param name="ppTInfo">A pointer to the requested type information object.</param>
		/// <exception cref="T:System.NotImplementedException">The method is called late-bound using the COM IDispatch interface.</exception>
		void _ModuleBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Runtime.InteropServices._ModuleBuilder.GetTypeInfoCount(System.UInt32@)" />.</summary>
		/// <param name="pcTInfo">The location that receives the number of type information interfaces provided by the object.</param>
		/// <exception cref="T:System.NotImplementedException">The method is called late-bound using the COM IDispatch interface.</exception>
		void _ModuleBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Runtime.InteropServices._ModuleBuilder.Invoke(System.UInt32,System.Guid@,System.UInt32,System.Int16,System.IntPtr,System.IntPtr,System.IntPtr,System.IntPtr)" />.</summary>
		/// <param name="dispIdMember">The member ID.</param>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="lcid">The locale context in which to interpret arguments.</param>
		/// <param name="wFlags">Flags describing the context of the call.</param>
		/// <param name="pDispParams">Pointer to a structure containing an array of arguments, an array of argument DISPIDs for named arguments, and counts for the number of elements in the arrays.</param>
		/// <param name="pVarResult">Pointer to the location where the result is to be stored.</param>
		/// <param name="pExcepInfo">Pointer to a structure that contains exception information.</param>
		/// <param name="puArgErr">The index of the first argument that has an error.</param>
		/// <exception cref="T:System.NotImplementedException">The method is called late-bound using the COM IDispatch interface.</exception>
		void _ModuleBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void basic_init(ModuleBuilder ab);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrappers_type(ModuleBuilder mb, Type ab);

		internal ModuleBuilder(AssemblyBuilder assb, string name, string fullyqname, bool emitSymbolInfo, bool transient)
		{
			this.scopename = name;
			this.name = name;
			this.fqname = fullyqname;
			this.assemblyb = assb;
			this.assembly = assb;
			this.transient = transient;
			this.guid = Guid.FastNewGuidArray();
			this.table_idx = this.get_next_table_index(this, 0, 1);
			this.name_cache = new Dictionary<TypeName, TypeBuilder>();
			this.us_string_cache = new Dictionary<string, int>(512);
			ModuleBuilder.basic_init(this);
			this.CreateGlobalType();
			if (assb.IsRun)
			{
				Type ab = new TypeBuilder(this, TypeAttributes.Abstract, 16777215).CreateType();
				ModuleBuilder.set_wrappers_type(this, ab);
			}
			if (emitSymbolInfo)
			{
				Assembly assembly = Assembly.LoadWithPartialName("Mono.CompilerServices.SymbolWriter");
				Type type = null;
				if (assembly != null)
				{
					type = assembly.GetType("Mono.CompilerServices.SymbolWriter.SymbolWriterImpl");
				}
				if (type == null)
				{
					ModuleBuilder.WarnAboutSymbolWriter("Failed to load the default Mono.CompilerServices.SymbolWriter assembly");
				}
				else
				{
					try
					{
						this.symbolWriter = (ISymbolWriter)Activator.CreateInstance(type, new object[]
						{
							this
						});
					}
					catch (MissingMethodException)
					{
						ModuleBuilder.WarnAboutSymbolWriter("The default Mono.CompilerServices.SymbolWriter is not available on this platform");
						return;
					}
				}
				string text = this.fqname;
				if (this.assemblyb.AssemblyDir != null)
				{
					text = Path.Combine(this.assemblyb.AssemblyDir, text);
				}
				this.symbolWriter.Initialize(IntPtr.Zero, text, true);
			}
		}

		private static void WarnAboutSymbolWriter(string message)
		{
			if (ModuleBuilder.has_warned_about_symbolWriter)
			{
				return;
			}
			ModuleBuilder.has_warned_about_symbolWriter = true;
			Console.Error.WriteLine("WARNING: {0}", message);
		}

		/// <summary>Gets a <see langword="String" /> representing the fully qualified name and path to this module.</summary>
		/// <returns>The fully qualified module name.</returns>
		public override string FullyQualifiedName
		{
			get
			{
				string text = this.fqname;
				if (text == null)
				{
					return null;
				}
				if (this.assemblyb.AssemblyDir != null)
				{
					text = Path.Combine(this.assemblyb.AssemblyDir, text);
					text = Path.GetFullPath(text);
				}
				return text;
			}
		}

		/// <summary>Returns a value that indicates whether this dynamic module is transient.</summary>
		/// <returns>
		///   <see langword="true" /> if this dynamic module is transient; otherwise, <see langword="false" />.</returns>
		public bool IsTransient()
		{
			return this.transient;
		}

		/// <summary>Completes the global function definitions and global data definitions for this dynamic module.</summary>
		/// <exception cref="T:System.InvalidOperationException">This method was called previously.</exception>
		public void CreateGlobalFunctions()
		{
			if (this.global_type_created != null)
			{
				throw new InvalidOperationException("global methods already created");
			}
			if (this.global_type != null)
			{
				this.global_type_created = this.global_type.CreateType();
			}
		}

		/// <summary>Defines an initialized data field in the .sdata section of the portable executable (PE) file.</summary>
		/// <param name="name">The name used to refer to the data. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="data">The binary large object (BLOB) of data.</param>
		/// <param name="attributes">The attributes for the field. The default is <see langword="Static" />.</param>
		/// <returns>A field to reference the data.</returns>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="name" /> is zero.  
		///  -or-  
		///  The size of <paramref name="data" /> is less than or equal to zero or greater than or equal to 0x3f0000.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> or <paramref name="data" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Reflection.Emit.ModuleBuilder.CreateGlobalFunctions" /> has been previously called.</exception>
		public FieldBuilder DefineInitializedData(string name, byte[] data, FieldAttributes attributes)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			FieldAttributes fieldAttributes = attributes & ~(FieldAttributes.RTSpecialName | FieldAttributes.HasFieldMarshal | FieldAttributes.HasDefault | FieldAttributes.HasFieldRVA);
			FieldBuilder fieldBuilder = this.DefineDataImpl(name, data.Length, fieldAttributes | FieldAttributes.HasFieldRVA);
			fieldBuilder.SetRVAData(data);
			return fieldBuilder;
		}

		/// <summary>Defines an uninitialized data field in the .sdata section of the portable executable (PE) file.</summary>
		/// <param name="name">The name used to refer to the data. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="size">The size of the data field.</param>
		/// <param name="attributes">The attributes for the field.</param>
		/// <returns>A field to reference the data.</returns>
		/// <exception cref="T:System.ArgumentException">The length of <paramref name="name" /> is zero.  
		///  -or-  
		///  <paramref name="size" /> is less than or equal to zero, or greater than or equal to 0x003f0000.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Reflection.Emit.ModuleBuilder.CreateGlobalFunctions" /> has been previously called.</exception>
		public FieldBuilder DefineUninitializedData(string name, int size, FieldAttributes attributes)
		{
			return this.DefineDataImpl(name, size, attributes & ~(FieldAttributes.RTSpecialName | FieldAttributes.HasFieldMarshal | FieldAttributes.HasDefault | FieldAttributes.HasFieldRVA));
		}

		private FieldBuilder DefineDataImpl(string name, int size, FieldAttributes attributes)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("name cannot be empty", "name");
			}
			if (this.global_type_created != null)
			{
				throw new InvalidOperationException("global fields already created");
			}
			if (size <= 0 || size >= 4128768)
			{
				throw new ArgumentException("Data size must be > 0 and < 0x3f0000", null);
			}
			this.CreateGlobalType();
			string className = "$ArrayType$" + size.ToString();
			Type type = this.GetType(className, false, false);
			if (type == null)
			{
				TypeBuilder typeBuilder = this.DefineType(className, TypeAttributes.Public | TypeAttributes.ExplicitLayout | TypeAttributes.Sealed, this.assemblyb.corlib_value_type, null, PackingSize.Size1, size);
				typeBuilder.CreateType();
				type = typeBuilder;
			}
			FieldBuilder fieldBuilder = this.global_type.DefineField(name, type, attributes | FieldAttributes.Static);
			if (this.global_fields != null)
			{
				FieldBuilder[] array = new FieldBuilder[this.global_fields.Length + 1];
				Array.Copy(this.global_fields, array, this.global_fields.Length);
				array[this.global_fields.Length] = fieldBuilder;
				this.global_fields = array;
			}
			else
			{
				this.global_fields = new FieldBuilder[1];
				this.global_fields[0] = fieldBuilder;
			}
			return fieldBuilder;
		}

		private void addGlobalMethod(MethodBuilder mb)
		{
			if (this.global_methods != null)
			{
				MethodBuilder[] array = new MethodBuilder[this.global_methods.Length + 1];
				Array.Copy(this.global_methods, array, this.global_methods.Length);
				array[this.global_methods.Length] = mb;
				this.global_methods = array;
				return;
			}
			this.global_methods = new MethodBuilder[1];
			this.global_methods[0] = mb;
		}

		/// <summary>Defines a global method with the specified name, attributes, return type, and parameter types.</summary>
		/// <param name="name">The name of the method. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attributes">The attributes of the method. <paramref name="attributes" /> must include <see cref="F:System.Reflection.MethodAttributes.Static" />.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the method's parameters.</param>
		/// <returns>The defined global method.</returns>
		/// <exception cref="T:System.ArgumentException">The method is not static. That is, <paramref name="attributes" /> does not include <see cref="F:System.Reflection.MethodAttributes.Static" />.  
		///  -or-  
		///  The length of <paramref name="name" /> is zero  
		///  -or-  
		///  An element in the <see cref="T:System.Type" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Reflection.Emit.ModuleBuilder.CreateGlobalFunctions" /> has been previously called.</exception>
		public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
		{
			return this.DefineGlobalMethod(name, attributes, CallingConventions.Standard, returnType, parameterTypes);
		}

		/// <summary>Defines a global method with the specified name, attributes, calling convention, return type, and parameter types.</summary>
		/// <param name="name">The name of the method. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attributes">The attributes of the method. <paramref name="attributes" /> must include <see cref="F:System.Reflection.MethodAttributes.Static" />.</param>
		/// <param name="callingConvention">The calling convention for the method.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the method's parameters.</param>
		/// <returns>The defined global method.</returns>
		/// <exception cref="T:System.ArgumentException">The method is not static. That is, <paramref name="attributes" /> does not include <see cref="F:System.Reflection.MethodAttributes.Static" />.  
		///  -or-  
		///  An element in the <see cref="T:System.Type" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <see cref="M:System.Reflection.Emit.ModuleBuilder.CreateGlobalFunctions" /> has been previously called.</exception>
		public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return this.DefineGlobalMethod(name, attributes, callingConvention, returnType, null, null, parameterTypes, null, null);
		}

		/// <summary>Defines a global method with the specified name, attributes, calling convention, return type, custom modifiers for the return type, parameter types, and custom modifiers for the parameter types.</summary>
		/// <param name="name">The name of the method. <paramref name="name" /> cannot contain embedded null characters.</param>
		/// <param name="attributes">The attributes of the method. <paramref name="attributes" /> must include <see cref="F:System.Reflection.MethodAttributes.Static" />.</param>
		/// <param name="callingConvention">The calling convention for the method.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="requiredReturnTypeCustomModifiers">An array of types representing the required custom modifiers for the return type, such as <see cref="T:System.Runtime.CompilerServices.IsConst" /> or <see cref="T:System.Runtime.CompilerServices.IsBoxed" />. If the return type has no required custom modifiers, specify <see langword="null" />.</param>
		/// <param name="optionalReturnTypeCustomModifiers">An array of types representing the optional custom modifiers for the return type, such as <see cref="T:System.Runtime.CompilerServices.IsConst" /> or <see cref="T:System.Runtime.CompilerServices.IsBoxed" />. If the return type has no optional custom modifiers, specify <see langword="null" />.</param>
		/// <param name="parameterTypes">The types of the method's parameters.</param>
		/// <param name="requiredParameterTypeCustomModifiers">An array of arrays of types. Each array of types represents the required custom modifiers for the corresponding parameter of the global method. If a particular argument has no required custom modifiers, specify <see langword="null" /> instead of an array of types. If the global method has no arguments, or if none of the arguments have required custom modifiers, specify <see langword="null" /> instead of an array of arrays.</param>
		/// <param name="optionalParameterTypeCustomModifiers">An array of arrays of types. Each array of types represents the optional custom modifiers for the corresponding parameter. If a particular argument has no optional custom modifiers, specify <see langword="null" /> instead of an array of types. If the global method has no arguments, or if none of the arguments have optional custom modifiers, specify <see langword="null" /> instead of an array of arrays.</param>
		/// <returns>The defined global method.</returns>
		/// <exception cref="T:System.ArgumentException">The method is not static. That is, <paramref name="attributes" /> does not include <see cref="F:System.Reflection.MethodAttributes.Static" />.  
		///  -or-  
		///  An element in the <see cref="T:System.Type" /> array is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="M:System.Reflection.Emit.ModuleBuilder.CreateGlobalFunctions" /> method has been previously called.</exception>
		public MethodBuilder DefineGlobalMethod(string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes, Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if ((attributes & MethodAttributes.Static) == MethodAttributes.PrivateScope)
			{
				throw new ArgumentException("global methods must be static");
			}
			if (this.global_type_created != null)
			{
				throw new InvalidOperationException("global methods already created");
			}
			this.CreateGlobalType();
			MethodBuilder methodBuilder = this.global_type.DefineMethod(name, attributes, callingConvention, returnType, requiredReturnTypeCustomModifiers, optionalReturnTypeCustomModifiers, parameterTypes, requiredParameterTypeCustomModifiers, optionalParameterTypeCustomModifiers);
			this.addGlobalMethod(methodBuilder);
			return methodBuilder;
		}

		/// <summary>Defines a <see langword="PInvoke" /> method with the specified name, the name of the DLL in which the method is defined, the attributes of the method, the calling convention of the method, the return type of the method, the types of the parameters of the method, and the <see langword="PInvoke" /> flags.</summary>
		/// <param name="name">The name of the <see langword="PInvoke" /> method. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="dllName">The name of the DLL in which the <see langword="PInvoke" /> method is defined.</param>
		/// <param name="attributes">The attributes of the method.</param>
		/// <param name="callingConvention">The method's calling convention.</param>
		/// <param name="returnType">The method's return type.</param>
		/// <param name="parameterTypes">The types of the method's parameters.</param>
		/// <param name="nativeCallConv">The native calling convention.</param>
		/// <param name="nativeCharSet">The method's native character set.</param>
		/// <returns>The defined <see langword="PInvoke" /> method.</returns>
		/// <exception cref="T:System.ArgumentException">The method is not static or if the containing type is an interface.  
		///  -or-  
		///  The method is abstract.  
		///  -or-  
		///  The method was previously defined.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> or <paramref name="dllName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The containing type has been previously created using <see cref="M:System.Reflection.Emit.TypeBuilder.CreateType" /></exception>
		public MethodBuilder DefinePInvokeMethod(string name, string dllName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			return this.DefinePInvokeMethod(name, dllName, name, attributes, callingConvention, returnType, parameterTypes, nativeCallConv, nativeCharSet);
		}

		/// <summary>Defines a <see langword="PInvoke" /> method with the specified name, the name of the DLL in which the method is defined, the attributes of the method, the calling convention of the method, the return type of the method, the types of the parameters of the method, and the <see langword="PInvoke" /> flags.</summary>
		/// <param name="name">The name of the <see langword="PInvoke" /> method. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="dllName">The name of the DLL in which the <see langword="PInvoke" /> method is defined.</param>
		/// <param name="entryName">The name of the entry point in the DLL.</param>
		/// <param name="attributes">The attributes of the method.</param>
		/// <param name="callingConvention">The method's calling convention.</param>
		/// <param name="returnType">The method's return type.</param>
		/// <param name="parameterTypes">The types of the method's parameters.</param>
		/// <param name="nativeCallConv">The native calling convention.</param>
		/// <param name="nativeCharSet">The method's native character set.</param>
		/// <returns>The defined <see langword="PInvoke" /> method.</returns>
		/// <exception cref="T:System.ArgumentException">The method is not static or if the containing type is an interface or if the method is abstract of if the method was previously defined.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> or <paramref name="dllName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The containing type has been previously created using <see cref="M:System.Reflection.Emit.TypeBuilder.CreateType" /></exception>
		public MethodBuilder DefinePInvokeMethod(string name, string dllName, string entryName, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, CallingConvention nativeCallConv, CharSet nativeCharSet)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if ((attributes & MethodAttributes.Static) == MethodAttributes.PrivateScope)
			{
				throw new ArgumentException("global methods must be static");
			}
			if (this.global_type_created != null)
			{
				throw new InvalidOperationException("global methods already created");
			}
			this.CreateGlobalType();
			MethodBuilder methodBuilder = this.global_type.DefinePInvokeMethod(name, dllName, entryName, attributes, callingConvention, returnType, parameterTypes, nativeCallConv, nativeCharSet);
			this.addGlobalMethod(methodBuilder);
			return methodBuilder;
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> for a private type with the specified name in this module.</summary>
		/// <param name="name">The full path of the type, including the namespace. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <returns>A private type with the specified name.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public TypeBuilder DefineType(string name)
		{
			return this.DefineType(name, TypeAttributes.NotPublic);
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> given the type name and the type attributes.</summary>
		/// <param name="name">The full path of the type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attr">The attributes of the defined type.</param>
		/// <returns>A <see langword="TypeBuilder" /> created with all of the requested attributes.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public TypeBuilder DefineType(string name, TypeAttributes attr)
		{
			if ((attr & TypeAttributes.ClassSemanticsMask) != TypeAttributes.NotPublic)
			{
				return this.DefineType(name, attr, null, null);
			}
			return this.DefineType(name, attr, typeof(object), null);
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> given type name, its attributes, and the type that the defined type extends.</summary>
		/// <param name="name">The full path of the type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attr">The attribute to be associated with the type.</param>
		/// <param name="parent">The type that the defined type extends.</param>
		/// <returns>A <see langword="TypeBuilder" /> created with all of the requested attributes.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent)
		{
			return this.DefineType(name, attr, parent, null);
		}

		private void AddType(TypeBuilder tb)
		{
			if (this.types != null)
			{
				if (this.types.Length == this.num_types)
				{
					TypeBuilder[] destinationArray = new TypeBuilder[this.types.Length * 2];
					Array.Copy(this.types, destinationArray, this.num_types);
					this.types = destinationArray;
				}
			}
			else
			{
				this.types = new TypeBuilder[1];
			}
			this.types[this.num_types] = tb;
			this.num_types++;
		}

		private TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces, PackingSize packingSize, int typesize)
		{
			if (name == null)
			{
				throw new ArgumentNullException("fullname");
			}
			TypeIdentifier key = TypeIdentifiers.FromInternal(name);
			if (this.name_cache.ContainsKey(key))
			{
				throw new ArgumentException("Duplicate type name within an assembly.");
			}
			TypeBuilder typeBuilder = new TypeBuilder(this, name, attr, parent, interfaces, packingSize, typesize, null);
			this.AddType(typeBuilder);
			this.name_cache.Add(key, typeBuilder);
			return typeBuilder;
		}

		internal void RegisterTypeName(TypeBuilder tb, TypeName name)
		{
			this.name_cache.Add(name, tb);
		}

		internal TypeBuilder GetRegisteredType(TypeName name)
		{
			TypeBuilder result = null;
			this.name_cache.TryGetValue(name, out result);
			return result;
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> given the type name, attributes, the type that the defined type extends, and the interfaces that the defined type implements.</summary>
		/// <param name="name">The full path of the type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attr">The attributes to be associated with the type.</param>
		/// <param name="parent">The type that the defined type extends.</param>
		/// <param name="interfaces">The list of interfaces that the type implements.</param>
		/// <returns>A <see langword="TypeBuilder" /> created with all of the requested attributes.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		[ComVisible(true)]
		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, Type[] interfaces)
		{
			return this.DefineType(name, attr, parent, interfaces, PackingSize.Unspecified, 0);
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> given the type name, the attributes, the type that the defined type extends, and the total size of the type.</summary>
		/// <param name="name">The full path of the type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attr">The attributes of the defined type.</param>
		/// <param name="parent">The type that the defined type extends.</param>
		/// <param name="typesize">The total size of the type.</param>
		/// <returns>A <see langword="TypeBuilder" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, int typesize)
		{
			return this.DefineType(name, attr, parent, null, PackingSize.Unspecified, typesize);
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> given the type name, the attributes, the type that the defined type extends, and the packing size of the type.</summary>
		/// <param name="name">The full path of the type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attr">The attributes of the defined type.</param>
		/// <param name="parent">The type that the defined type extends.</param>
		/// <param name="packsize">The packing size of the type.</param>
		/// <returns>A <see langword="TypeBuilder" /> object.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packsize)
		{
			return this.DefineType(name, attr, parent, null, packsize, 0);
		}

		/// <summary>Constructs a <see langword="TypeBuilder" /> given the type name, attributes, the type that the defined type extends, the packing size of the defined type, and the total size of the defined type.</summary>
		/// <param name="name">The full path of the type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="attr">The attributes of the defined type.</param>
		/// <param name="parent">The type that the defined type extends.</param>
		/// <param name="packingSize">The packing size of the type.</param>
		/// <param name="typesize">The total size of the type.</param>
		/// <returns>A <see langword="TypeBuilder" /> created with all of the requested attributes.</returns>
		/// <exception cref="T:System.ArgumentException">A type with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  Nested type attributes are set on a type that is not nested.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public TypeBuilder DefineType(string name, TypeAttributes attr, Type parent, PackingSize packingSize, int typesize)
		{
			return this.DefineType(name, attr, parent, null, packingSize, typesize);
		}

		/// <summary>Returns the named method on an array class.</summary>
		/// <param name="arrayClass">An array class.</param>
		/// <param name="methodName">The name of a method on the array class.</param>
		/// <param name="callingConvention">The method's calling convention.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the method's parameters.</param>
		/// <returns>The named method on an array class.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="arrayClass" /> is not an array.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="arrayClass" /> or <paramref name="methodName" /> is <see langword="null" />.</exception>
		public MethodInfo GetArrayMethod(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return new MonoArrayMethod(arrayClass, methodName, callingConvention, returnType, parameterTypes);
		}

		/// <summary>Defines an enumeration type that is a value type with a single non-static field called <paramref name="value__" /> of the specified type.</summary>
		/// <param name="name">The full path of the enumeration type. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="visibility">The type attributes for the enumeration. The attributes are any bits defined by <see cref="F:System.Reflection.TypeAttributes.VisibilityMask" />.</param>
		/// <param name="underlyingType">The underlying type for the enumeration. This must be a built-in integer type.</param>
		/// <returns>The defined enumeration.</returns>
		/// <exception cref="T:System.ArgumentException">Attributes other than visibility attributes are provided.  
		///  -or-  
		///  An enumeration with the given name exists in the parent assembly of this module.  
		///  -or-  
		///  The visibility attributes do not match the scope of the enumeration. For example, <see cref="F:System.Reflection.TypeAttributes.NestedPublic" /> is specified for <paramref name="visibility" />, but the enumeration is not a nested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public EnumBuilder DefineEnum(string name, TypeAttributes visibility, Type underlyingType)
		{
			TypeIdentifier key = TypeIdentifiers.FromInternal(name);
			if (this.name_cache.ContainsKey(key))
			{
				throw new ArgumentException("Duplicate type name within an assembly.");
			}
			EnumBuilder enumBuilder = new EnumBuilder(this, name, visibility, underlyingType);
			TypeBuilder typeBuilder = enumBuilder.GetTypeBuilder();
			this.AddType(typeBuilder);
			this.name_cache.Add(key, typeBuilder);
			return enumBuilder;
		}

		/// <summary>Gets the named type defined in the module.</summary>
		/// <param name="className">The name of the <see cref="T:System.Type" /> to get.</param>
		/// <returns>The requested type, if the type is defined in this module; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="className" /> is zero or is greater than 1023.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="className" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The requested <see cref="T:System.Type" /> is non-public and the caller does not have <see cref="T:System.Security.Permissions.ReflectionPermission" /> to reflect non-public objects outside the current assembly.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">An error is encountered while loading the <see cref="T:System.Type" />.</exception>
		[ComVisible(true)]
		public override Type GetType(string className)
		{
			return this.GetType(className, false, false);
		}

		/// <summary>Gets the named type defined in the module, optionally ignoring the case of the type name.</summary>
		/// <param name="className">The name of the <see cref="T:System.Type" /> to get.</param>
		/// <param name="ignoreCase">If <see langword="true" />, the search is case-insensitive. If <see langword="false" />, the search is case-sensitive.</param>
		/// <returns>The requested type, if the type is defined in this module; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="className" /> is zero or is greater than 1023.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="className" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The requested <see cref="T:System.Type" /> is non-public and the caller does not have <see cref="T:System.Security.Permissions.ReflectionPermission" /> to reflect non-public objects outside the current assembly.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		[ComVisible(true)]
		public override Type GetType(string className, bool ignoreCase)
		{
			return this.GetType(className, false, ignoreCase);
		}

		private TypeBuilder search_in_array(TypeBuilder[] arr, int validElementsInArray, TypeName className)
		{
			for (int i = 0; i < validElementsInArray; i++)
			{
				if (string.Compare(className.DisplayName, arr[i].FullName, true, CultureInfo.InvariantCulture) == 0)
				{
					return arr[i];
				}
			}
			return null;
		}

		private TypeBuilder search_nested_in_array(TypeBuilder[] arr, int validElementsInArray, TypeName className)
		{
			for (int i = 0; i < validElementsInArray; i++)
			{
				if (string.Compare(className.DisplayName, arr[i].Name, true, CultureInfo.InvariantCulture) == 0)
				{
					return arr[i];
				}
			}
			return null;
		}

		private TypeBuilder GetMaybeNested(TypeBuilder t, IEnumerable<TypeName> nested)
		{
			TypeBuilder typeBuilder = t;
			foreach (TypeName className in nested)
			{
				if (typeBuilder.subtypes == null)
				{
					return null;
				}
				typeBuilder = this.search_nested_in_array(typeBuilder.subtypes, typeBuilder.subtypes.Length, className);
				if (typeBuilder == null)
				{
					return null;
				}
			}
			return typeBuilder;
		}

		/// <summary>Gets the named type defined in the module, optionally ignoring the case of the type name. Optionally throws an exception if the type is not found.</summary>
		/// <param name="className">The name of the <see cref="T:System.Type" /> to get.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />.</param>
		/// <param name="ignoreCase">If <see langword="true" />, the search is case-insensitive. If <see langword="false" />, the search is case-sensitive.</param>
		/// <returns>The specified type, if the type is declared in this module; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="className" /> is zero or is greater than 1023.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="className" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The requested <see cref="T:System.Type" /> is non-public and the caller does not have <see cref="T:System.Security.Permissions.ReflectionPermission" /> to reflect non-public objects outside the current assembly.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the specified type is not found.</exception>
		[ComVisible(true)]
		public override Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			if (className.Length == 0)
			{
				throw new ArgumentException("className");
			}
			TypeBuilder typeBuilder = null;
			if (this.types == null && throwOnError)
			{
				throw new TypeLoadException(className);
			}
			TypeSpec typeSpec = TypeSpec.Parse(className);
			if (!ignoreCase)
			{
				TypeName key = typeSpec.TypeNameWithoutModifiers();
				this.name_cache.TryGetValue(key, out typeBuilder);
			}
			else
			{
				if (this.types != null)
				{
					typeBuilder = this.search_in_array(this.types, this.num_types, typeSpec.Name);
				}
				if (!typeSpec.IsNested && typeBuilder != null)
				{
					typeBuilder = this.GetMaybeNested(typeBuilder, typeSpec.Nested);
				}
			}
			if (typeBuilder == null && throwOnError)
			{
				throw new TypeLoadException(className);
			}
			if (typeBuilder != null && (typeSpec.HasModifiers || typeSpec.IsByRef))
			{
				Type type = typeBuilder;
				if (typeBuilder != null)
				{
					TypeBuilder typeBuilder2 = typeBuilder;
					if (typeBuilder2.is_created)
					{
						type = typeBuilder2.CreateType();
					}
				}
				foreach (ModifierSpec modifierSpec in typeSpec.Modifiers)
				{
					if (modifierSpec is PointerSpec)
					{
						type = type.MakePointerType();
					}
					else if (modifierSpec is ArraySpec)
					{
						ArraySpec arraySpec = modifierSpec as ArraySpec;
						if (arraySpec.IsBound)
						{
							return null;
						}
						if (arraySpec.Rank == 1)
						{
							type = type.MakeArrayType();
						}
						else
						{
							type = type.MakeArrayType(arraySpec.Rank);
						}
					}
				}
				if (typeSpec.IsByRef)
				{
					type = type.MakeByRefType();
				}
				typeBuilder = (type as TypeBuilder);
				if (typeBuilder == null)
				{
					return type;
				}
			}
			if (typeBuilder != null && typeBuilder.is_created)
			{
				return typeBuilder.CreateType();
			}
			return typeBuilder;
		}

		internal int get_next_table_index(object obj, int table, int count)
		{
			if (this.table_indexes == null)
			{
				this.table_indexes = new int[64];
				for (int i = 0; i < 64; i++)
				{
					this.table_indexes[i] = 1;
				}
				this.table_indexes[2] = 2;
			}
			int result = this.table_indexes[table];
			this.table_indexes[table] += count;
			return result;
		}

		/// <summary>Applies a custom attribute to this module by using a custom attribute builder.</summary>
		/// <param name="customBuilder">An instance of a helper class that specifies the custom attribute to apply.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="customBuilder" /> is <see langword="null" />.</exception>
		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (this.cattrs != null)
			{
				CustomAttributeBuilder[] array = new CustomAttributeBuilder[this.cattrs.Length + 1];
				this.cattrs.CopyTo(array, 0);
				array[this.cattrs.Length] = customBuilder;
				this.cattrs = array;
				return;
			}
			this.cattrs = new CustomAttributeBuilder[1];
			this.cattrs[0] = customBuilder;
		}

		/// <summary>Applies a custom attribute to this module by using a specified binary large object (BLOB) that represents the attribute.</summary>
		/// <param name="con">The constructor for the custom attribute.</param>
		/// <param name="binaryAttribute">A byte BLOB representing the attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="con" /> or <paramref name="binaryAttribute" /> is <see langword="null" />.</exception>
		[ComVisible(true)]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			this.SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		/// <summary>Returns the symbol writer associated with this dynamic module.</summary>
		/// <returns>The symbol writer associated with this dynamic module.</returns>
		public ISymbolWriter GetSymWriter()
		{
			return this.symbolWriter;
		}

		/// <summary>Defines a document for source.</summary>
		/// <param name="url">The URL for the document.</param>
		/// <param name="language">The GUID that identifies the document language. This can be <see cref="F:System.Guid.Empty" />.</param>
		/// <param name="languageVendor">The GUID that identifies the document language vendor. This can be <see cref="F:System.Guid.Empty" />.</param>
		/// <param name="documentType">The GUID that identifies the document type. This can be <see cref="F:System.Guid.Empty" />.</param>
		/// <returns>The defined document.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="url" /> is <see langword="null" />. This is a change from earlier versions of the .NET Framework.</exception>
		/// <exception cref="T:System.InvalidOperationException">This method is called on a dynamic module that is not a debug module.</exception>
		public ISymbolDocumentWriter DefineDocument(string url, Guid language, Guid languageVendor, Guid documentType)
		{
			if (this.symbolWriter != null)
			{
				return this.symbolWriter.DefineDocument(url, language, languageVendor, documentType);
			}
			return null;
		}

		/// <summary>Returns all the classes defined within this module.</summary>
		/// <returns>An array that contains the types defined within the module that is reflected by this instance.</returns>
		/// <exception cref="T:System.Reflection.ReflectionTypeLoadException">One or more classes in a module could not be loaded.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public override Type[] GetTypes()
		{
			if (this.types == null)
			{
				return Type.EmptyTypes;
			}
			int num = this.num_types;
			Type[] array = new Type[num];
			Array.Copy(this.types, array, num);
			for (int i = 0; i < array.Length; i++)
			{
				if (this.types[i].is_created)
				{
					array[i] = this.types[i].CreateType();
				}
			}
			return array;
		}

		/// <summary>Defines the named managed embedded resource with the given attributes that is to be stored in this module.</summary>
		/// <param name="name">The name of the resource. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="description">The description of the resource.</param>
		/// <param name="attribute">The resource attributes.</param>
		/// <returns>A resource writer for the defined resource.</returns>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="name" /> is zero.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">This module is transient.  
		///  -or-  
		///  The containing assembly is not persistable.</exception>
		public IResourceWriter DefineResource(string name, string description, ResourceAttributes attribute)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("name cannot be empty");
			}
			if (this.transient)
			{
				throw new InvalidOperationException("The module is transient");
			}
			if (!this.assemblyb.IsSave)
			{
				throw new InvalidOperationException("The assembly is transient");
			}
			ResourceWriter resourceWriter = new ResourceWriter(new MemoryStream());
			if (this.resource_writers == null)
			{
				this.resource_writers = new Hashtable();
			}
			this.resource_writers[name] = resourceWriter;
			if (this.resources != null)
			{
				MonoResource[] destinationArray = new MonoResource[this.resources.Length + 1];
				Array.Copy(this.resources, destinationArray, this.resources.Length);
				this.resources = destinationArray;
			}
			else
			{
				this.resources = new MonoResource[1];
			}
			int num = this.resources.Length - 1;
			this.resources[num].name = name;
			this.resources[num].attrs = attribute;
			return resourceWriter;
		}

		/// <summary>Defines the named managed embedded resource to be stored in this module.</summary>
		/// <param name="name">The name of the resource. <paramref name="name" /> cannot contain embedded nulls.</param>
		/// <param name="description">The description of the resource.</param>
		/// <returns>A resource writer for the defined resource.</returns>
		/// <exception cref="T:System.ArgumentException">Length of <paramref name="name" /> is zero.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">This module is transient.  
		///  -or-  
		///  The containing assembly is not persistable.</exception>
		public IResourceWriter DefineResource(string name, string description)
		{
			return this.DefineResource(name, description, ResourceAttributes.Public);
		}

		/// <summary>Defines an unmanaged embedded resource given an opaque binary large object (BLOB) of bytes.</summary>
		/// <param name="resource">An opaque BLOB that represents an unmanaged resource</param>
		/// <exception cref="T:System.ArgumentException">An unmanaged resource has already been defined in the module's assembly.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="resource" /> is <see langword="null" />.</exception>
		[MonoTODO]
		public void DefineUnmanagedResource(byte[] resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}
			throw new NotImplementedException();
		}

		/// <summary>Defines an unmanaged resource given the name of Win32 resource file.</summary>
		/// <param name="resourceFileName">The name of the unmanaged resource file.</param>
		/// <exception cref="T:System.ArgumentException">An unmanaged resource has already been defined in the module's assembly.  
		///  -or-  
		///  <paramref name="resourceFileName" /> is the empty string ("").</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="resourceFileName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="resourceFileName" /> is not found.  
		/// -or-  
		/// <paramref name="resourceFileName" /> is a directory.</exception>
		[MonoTODO]
		public void DefineUnmanagedResource(string resourceFileName)
		{
			if (resourceFileName == null)
			{
				throw new ArgumentNullException("resourceFileName");
			}
			if (resourceFileName == string.Empty)
			{
				throw new ArgumentException("resourceFileName");
			}
			if (!File.Exists(resourceFileName) || Directory.Exists(resourceFileName))
			{
				throw new FileNotFoundException("File '" + resourceFileName + "' does not exist or is a directory.");
			}
			throw new NotImplementedException();
		}

		/// <summary>Defines a binary large object (BLOB) that represents a manifest resource to be embedded in the dynamic assembly.</summary>
		/// <param name="name">The case-sensitive name for the resource.</param>
		/// <param name="stream">A stream that contains the bytes for the resource.</param>
		/// <param name="attribute">An enumeration value that specifies whether the resource is public or private.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="stream" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is a zero-length string.</exception>
		/// <exception cref="T:System.InvalidOperationException">The dynamic assembly that contains the current module is transient; that is, no file name was specified when <see cref="M:System.Reflection.Emit.AssemblyBuilder.DefineDynamicModule(System.String,System.String)" /> was called.</exception>
		public void DefineManifestResource(string name, Stream stream, ResourceAttributes attribute)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name == string.Empty)
			{
				throw new ArgumentException("name cannot be empty");
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (this.transient)
			{
				throw new InvalidOperationException("The module is transient");
			}
			if (!this.assemblyb.IsSave)
			{
				throw new InvalidOperationException("The assembly is transient");
			}
			if (this.resources != null)
			{
				MonoResource[] destinationArray = new MonoResource[this.resources.Length + 1];
				Array.Copy(this.resources, destinationArray, this.resources.Length);
				this.resources = destinationArray;
			}
			else
			{
				this.resources = new MonoResource[1];
			}
			int num = this.resources.Length - 1;
			this.resources[num].name = name;
			this.resources[num].attrs = attribute;
			this.resources[num].stream = stream;
		}

		/// <summary>This method does nothing.</summary>
		/// <param name="name">The name of the custom attribute</param>
		/// <param name="data">An opaque binary large object (BLOB) of bytes that represents the value of the custom attribute.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="url" /> is <see langword="null" />.</exception>
		[MonoTODO]
		public void SetSymCustomAttribute(string name, byte[] data)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets the user entry point.</summary>
		/// <param name="entryPoint">The user entry point.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="entryPoint" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This method is called on a dynamic module that is not a debug module.  
		///  -or-  
		///  <paramref name="entryPoint" /> is not contained in this dynamic module.</exception>
		[MonoTODO]
		public void SetUserEntryPoint(MethodInfo entryPoint)
		{
			if (entryPoint == null)
			{
				throw new ArgumentNullException("entryPoint");
			}
			if (entryPoint.DeclaringType.Module != this)
			{
				throw new InvalidOperationException("entryPoint is not contained in this module");
			}
			throw new NotImplementedException();
		}

		/// <summary>Returns the token used to identify the specified method within this module.</summary>
		/// <param name="method">The method to get a token for.</param>
		/// <returns>The token used to identify the specified method within this module.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="method" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The declaring type for the method is not in this module.</exception>
		public MethodToken GetMethodToken(MethodInfo method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return new MethodToken(this.GetToken(method));
		}

		/// <summary>Returns the token used to identify the method that has the specified attributes and parameter types within this module.</summary>
		/// <param name="method">The method to get a token for.</param>
		/// <param name="optionalParameterTypes">A collection of the types of the optional parameters to the method.</param>
		/// <returns>The token used to identify the specified method within this module.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="method" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The declaring type for the method is not in this module.</exception>
		public MethodToken GetMethodToken(MethodInfo method, IEnumerable<Type> optionalParameterTypes)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return new MethodToken(this.GetToken(method, optionalParameterTypes));
		}

		/// <summary>Returns the token for the named method on an array class.</summary>
		/// <param name="arrayClass">The object for the array.</param>
		/// <param name="methodName">A string that contains the name of the method.</param>
		/// <param name="callingConvention">The calling convention for the method.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The token for the named method on an array class.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="arrayClass" /> is not an array.  
		/// -or-  
		/// The length of <paramref name="methodName" /> is zero.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="arrayClass" /> or <paramref name="methodName" /> is <see langword="null" />.</exception>
		public MethodToken GetArrayMethodToken(Type arrayClass, string methodName, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
		{
			return this.GetMethodToken(this.GetArrayMethod(arrayClass, methodName, callingConvention, returnType, parameterTypes));
		}

		/// <summary>Returns the token used to identify the specified constructor within this module.</summary>
		/// <param name="con">The constructor to get a token for.</param>
		/// <returns>The token used to identify the specified constructor within this module.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="con" /> is <see langword="null" />.</exception>
		[ComVisible(true)]
		public MethodToken GetConstructorToken(ConstructorInfo con)
		{
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			return new MethodToken(this.GetToken(con));
		}

		/// <summary>Returns the token used to identify the constructor that has the specified attributes and parameter types within this module.</summary>
		/// <param name="constructor">The constructor to get a token for.</param>
		/// <param name="optionalParameterTypes">A collection of the types of the optional parameters to the constructor.</param>
		/// <returns>The token used to identify the specified constructor within this module.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="constructor" /> is <see langword="null" />.</exception>
		public MethodToken GetConstructorToken(ConstructorInfo constructor, IEnumerable<Type> optionalParameterTypes)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
			return new MethodToken(this.GetToken(constructor, optionalParameterTypes));
		}

		/// <summary>Returns the token used to identify the specified field within this module.</summary>
		/// <param name="field">The field to get a token for.</param>
		/// <returns>The token used to identify the specified field within this module.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="field" /> is <see langword="null" />.</exception>
		public FieldToken GetFieldToken(FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			return new FieldToken(this.GetToken(field));
		}

		/// <summary>Defines a token for the signature that has the specified character array and signature length.</summary>
		/// <param name="sigBytes">The signature binary large object (BLOB).</param>
		/// <param name="sigLength">The length of the signature BLOB.</param>
		/// <returns>A token for the specified signature.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="sigBytes" /> is <see langword="null" />.</exception>
		[MonoTODO]
		public SignatureToken GetSignatureToken(byte[] sigBytes, int sigLength)
		{
			throw new NotImplementedException();
		}

		/// <summary>Defines a token for the signature that is defined by the specified <see cref="T:System.Reflection.Emit.SignatureHelper" />.</summary>
		/// <param name="sigHelper">The signature.</param>
		/// <returns>A token for the defined signature.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="sigHelper" /> is <see langword="null" />.</exception>
		public SignatureToken GetSignatureToken(SignatureHelper sigHelper)
		{
			if (sigHelper == null)
			{
				throw new ArgumentNullException("sigHelper");
			}
			return new SignatureToken(this.GetToken(sigHelper));
		}

		/// <summary>Returns the token of the given string in the module's constant pool.</summary>
		/// <param name="str">The string to add to the module's constant pool.</param>
		/// <returns>The token of the string in the constant pool.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="str" /> is <see langword="null" />.</exception>
		public StringToken GetStringConstant(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return new StringToken(this.GetToken(str));
		}

		/// <summary>Returns the token used to identify the specified type within this module.</summary>
		/// <param name="type">The type object that represents the class type.</param>
		/// <returns>The token used to identify the given type within this module.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="type" /> is a <see langword="ByRef" /> type.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This is a non-transient module that references a transient module.</exception>
		public TypeToken GetTypeToken(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type.IsByRef)
			{
				throw new ArgumentException("type can't be a byref type", "type");
			}
			if (!this.IsTransient() && type.Module is ModuleBuilder && ((ModuleBuilder)type.Module).IsTransient())
			{
				throw new InvalidOperationException("a non-transient module can't reference a transient module");
			}
			return new TypeToken(this.GetToken(type));
		}

		/// <summary>Returns the token used to identify the type with the specified name.</summary>
		/// <param name="name">The name of the class, including the namespace.</param>
		/// <returns>The token used to identify the type with the specified name within this module.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is the empty string ("").  
		/// -or-  
		/// <paramref name="name" /> represents a <see langword="ByRef" /> type.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// The type specified by <paramref name="name" /> could not be found.</exception>
		/// <exception cref="T:System.InvalidOperationException">This is a non-transient module that references a transient module.</exception>
		public TypeToken GetTypeToken(string name)
		{
			return this.GetTypeToken(this.GetType(name));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int getUSIndex(ModuleBuilder mb, string str);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int getToken(ModuleBuilder mb, object obj, bool create_open_instance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int getMethodToken(ModuleBuilder mb, MethodBase method, Type[] opt_param_types);

		internal int GetToken(string str)
		{
			int usindex;
			if (!this.us_string_cache.TryGetValue(str, out usindex))
			{
				usindex = ModuleBuilder.getUSIndex(this, str);
				this.us_string_cache[str] = usindex;
			}
			return usindex;
		}

		private int GetPseudoToken(MemberInfo member, bool create_open_instance)
		{
			Dictionary<MemberInfo, int> dictionary = create_open_instance ? this.inst_tokens_open : this.inst_tokens;
			int num;
			if (dictionary == null)
			{
				dictionary = new Dictionary<MemberInfo, int>(ReferenceEqualityComparer<MemberInfo>.Instance);
				if (create_open_instance)
				{
					this.inst_tokens_open = dictionary;
				}
				else
				{
					this.inst_tokens = dictionary;
				}
			}
			else if (dictionary.TryGetValue(member, out num))
			{
				return num;
			}
			if (member is TypeBuilderInstantiation || member is SymbolType)
			{
				num = ModuleBuilder.typespec_tokengen--;
			}
			else if (member is FieldOnTypeBuilderInst)
			{
				num = ModuleBuilder.memberref_tokengen--;
			}
			else if (member is ConstructorOnTypeBuilderInst)
			{
				num = ModuleBuilder.memberref_tokengen--;
			}
			else if (member is MethodOnTypeBuilderInst)
			{
				num = ModuleBuilder.memberref_tokengen--;
			}
			else if (member is FieldBuilder)
			{
				num = ModuleBuilder.memberref_tokengen--;
			}
			else if (member is TypeBuilder)
			{
				if (create_open_instance && (member as TypeBuilder).ContainsGenericParameters)
				{
					num = ModuleBuilder.typespec_tokengen--;
				}
				else if (member.Module == this)
				{
					num = ModuleBuilder.typedef_tokengen--;
				}
				else
				{
					num = ModuleBuilder.typeref_tokengen--;
				}
			}
			else
			{
				if (member is EnumBuilder)
				{
					num = this.GetPseudoToken((member as EnumBuilder).GetTypeBuilder(), create_open_instance);
					dictionary[member] = num;
					return num;
				}
				if (member is ConstructorBuilder)
				{
					if (member.Module == this && !(member as ConstructorBuilder).TypeBuilder.ContainsGenericParameters)
					{
						num = ModuleBuilder.methoddef_tokengen--;
					}
					else
					{
						num = ModuleBuilder.memberref_tokengen--;
					}
				}
				else if (member is MethodBuilder)
				{
					MethodBuilder methodBuilder = member as MethodBuilder;
					if (member.Module == this && !methodBuilder.TypeBuilder.ContainsGenericParameters && !methodBuilder.IsGenericMethodDefinition)
					{
						num = ModuleBuilder.methoddef_tokengen--;
					}
					else
					{
						num = ModuleBuilder.memberref_tokengen--;
					}
				}
				else
				{
					if (!(member is GenericTypeParameterBuilder))
					{
						throw new NotImplementedException();
					}
					num = ModuleBuilder.typespec_tokengen--;
				}
			}
			dictionary[member] = num;
			this.RegisterToken(member, num);
			return num;
		}

		internal int GetToken(MemberInfo member)
		{
			if (member is ConstructorBuilder || member is MethodBuilder || member is FieldBuilder)
			{
				return this.GetPseudoToken(member, false);
			}
			return ModuleBuilder.getToken(this, member, true);
		}

		internal int GetToken(MemberInfo member, bool create_open_instance)
		{
			if (member is TypeBuilderInstantiation || member is FieldOnTypeBuilderInst || member is ConstructorOnTypeBuilderInst || member is MethodOnTypeBuilderInst || member is SymbolType || member is FieldBuilder || member is TypeBuilder || member is ConstructorBuilder || member is MethodBuilder || member is GenericTypeParameterBuilder || member is EnumBuilder)
			{
				return this.GetPseudoToken(member, create_open_instance);
			}
			return ModuleBuilder.getToken(this, member, create_open_instance);
		}

		internal int GetToken(MethodBase method, IEnumerable<Type> opt_param_types)
		{
			if (method is ConstructorBuilder || method is MethodBuilder)
			{
				return this.GetPseudoToken(method, false);
			}
			if (opt_param_types == null)
			{
				return ModuleBuilder.getToken(this, method, true);
			}
			List<Type> list = new List<Type>(opt_param_types);
			return ModuleBuilder.getMethodToken(this, method, list.ToArray());
		}

		internal int GetToken(MethodBase method, Type[] opt_param_types)
		{
			if (method is ConstructorBuilder || method is MethodBuilder)
			{
				return this.GetPseudoToken(method, false);
			}
			return ModuleBuilder.getMethodToken(this, method, opt_param_types);
		}

		internal int GetToken(SignatureHelper helper)
		{
			return ModuleBuilder.getToken(this, helper, true);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void RegisterToken(object obj, int token);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object GetRegisteredToken(int token);

		internal TokenGenerator GetTokenGenerator()
		{
			if (this.token_gen == null)
			{
				this.token_gen = new ModuleBuilderTokenGenerator(this);
			}
			return this.token_gen;
		}

		internal static object RuntimeResolve(object obj)
		{
			if (obj is MethodBuilder)
			{
				return (obj as MethodBuilder).RuntimeResolve();
			}
			if (obj is ConstructorBuilder)
			{
				return (obj as ConstructorBuilder).RuntimeResolve();
			}
			if (obj is FieldBuilder)
			{
				return (obj as FieldBuilder).RuntimeResolve();
			}
			if (obj is GenericTypeParameterBuilder)
			{
				return (obj as GenericTypeParameterBuilder).RuntimeResolve();
			}
			if (obj is FieldOnTypeBuilderInst)
			{
				return (obj as FieldOnTypeBuilderInst).RuntimeResolve();
			}
			if (obj is MethodOnTypeBuilderInst)
			{
				return (obj as MethodOnTypeBuilderInst).RuntimeResolve();
			}
			if (obj is ConstructorOnTypeBuilderInst)
			{
				return (obj as ConstructorOnTypeBuilderInst).RuntimeResolve();
			}
			if (obj is Type)
			{
				return (obj as Type).RuntimeResolve();
			}
			throw new NotImplementedException(obj.GetType().FullName);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void build_metadata(ModuleBuilder mb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void WriteToFile(IntPtr handle);

		private void FixupTokens(Dictionary<int, int> token_map, Dictionary<int, MemberInfo> member_map, Dictionary<MemberInfo, int> inst_tokens, bool open)
		{
			foreach (KeyValuePair<MemberInfo, int> keyValuePair in inst_tokens)
			{
				MemberInfo key = keyValuePair.Key;
				int value = keyValuePair.Value;
				MemberInfo memberInfo;
				if (key is TypeBuilderInstantiation || key is SymbolType)
				{
					memberInfo = (key as Type).RuntimeResolve();
				}
				else if (key is FieldOnTypeBuilderInst)
				{
					memberInfo = (key as FieldOnTypeBuilderInst).RuntimeResolve();
				}
				else if (key is ConstructorOnTypeBuilderInst)
				{
					memberInfo = (key as ConstructorOnTypeBuilderInst).RuntimeResolve();
				}
				else if (key is MethodOnTypeBuilderInst)
				{
					memberInfo = (key as MethodOnTypeBuilderInst).RuntimeResolve();
				}
				else if (key is FieldBuilder)
				{
					memberInfo = (key as FieldBuilder).RuntimeResolve();
				}
				else if (key is TypeBuilder)
				{
					memberInfo = (key as TypeBuilder).RuntimeResolve();
				}
				else if (key is EnumBuilder)
				{
					memberInfo = (key as EnumBuilder).RuntimeResolve();
				}
				else if (key is ConstructorBuilder)
				{
					memberInfo = (key as ConstructorBuilder).RuntimeResolve();
				}
				else if (key is MethodBuilder)
				{
					memberInfo = (key as MethodBuilder).RuntimeResolve();
				}
				else
				{
					if (!(key is GenericTypeParameterBuilder))
					{
						throw new NotImplementedException();
					}
					memberInfo = (key as GenericTypeParameterBuilder).RuntimeResolve();
				}
				int value2 = this.GetToken(memberInfo, open);
				token_map[value] = value2;
				member_map[value] = memberInfo;
				this.RegisterToken(memberInfo, value);
			}
		}

		private void FixupTokens()
		{
			Dictionary<int, int> token_map = new Dictionary<int, int>();
			Dictionary<int, MemberInfo> member_map = new Dictionary<int, MemberInfo>();
			if (this.inst_tokens != null)
			{
				this.FixupTokens(token_map, member_map, this.inst_tokens, false);
			}
			if (this.inst_tokens_open != null)
			{
				this.FixupTokens(token_map, member_map, this.inst_tokens_open, true);
			}
			if (this.types != null)
			{
				for (int i = 0; i < this.num_types; i++)
				{
					this.types[i].FixupTokens(token_map, member_map);
				}
			}
		}

		internal void Save()
		{
			if (this.transient && !this.is_main)
			{
				return;
			}
			if (this.types != null)
			{
				for (int i = 0; i < this.num_types; i++)
				{
					if (!this.types[i].is_created)
					{
						throw new NotSupportedException("Type '" + this.types[i].FullName + "' was not completed.");
					}
				}
			}
			this.FixupTokens();
			if (this.global_type != null && this.global_type_created == null)
			{
				this.global_type_created = this.global_type.CreateType();
			}
			if (this.resources != null)
			{
				for (int j = 0; j < this.resources.Length; j++)
				{
					IResourceWriter resourceWriter;
					if (this.resource_writers != null && (resourceWriter = (this.resource_writers[this.resources[j].name] as IResourceWriter)) != null)
					{
						ResourceWriter resourceWriter2 = (ResourceWriter)resourceWriter;
						resourceWriter2.Generate();
						MemoryStream memoryStream = (MemoryStream)resourceWriter2._output;
						this.resources[j].data = new byte[memoryStream.Length];
						memoryStream.Seek(0L, SeekOrigin.Begin);
						memoryStream.Read(this.resources[j].data, 0, (int)memoryStream.Length);
					}
					else
					{
						Stream stream = this.resources[j].stream;
						if (stream != null)
						{
							try
							{
								long length = stream.Length;
								this.resources[j].data = new byte[length];
								stream.Seek(0L, SeekOrigin.Begin);
								stream.Read(this.resources[j].data, 0, (int)length);
							}
							catch
							{
							}
						}
					}
				}
			}
			ModuleBuilder.build_metadata(this);
			string text = this.fqname;
			if (this.assemblyb.AssemblyDir != null)
			{
				text = Path.Combine(this.assemblyb.AssemblyDir, text);
			}
			try
			{
				File.Delete(text);
			}
			catch
			{
			}
			using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write))
			{
				this.WriteToFile(fileStream.Handle);
			}
			File.SetAttributes(text, (FileAttributes)(-2147483648));
			if (this.types != null && this.symbolWriter != null)
			{
				for (int k = 0; k < this.num_types; k++)
				{
					this.types[k].GenerateDebugInfo(this.symbolWriter);
				}
				this.symbolWriter.Close();
			}
		}

		internal string FileName
		{
			get
			{
				return this.fqname;
			}
		}

		internal bool IsMain
		{
			set
			{
				this.is_main = value;
			}
		}

		internal void CreateGlobalType()
		{
			if (this.global_type == null)
			{
				this.global_type = new TypeBuilder(this, TypeAttributes.NotPublic, 1);
			}
		}

		internal override Guid GetModuleVersionId()
		{
			return new Guid(this.guid);
		}

		/// <summary>Gets the dynamic assembly that defined this instance of <see cref="T:System.Reflection.Emit.ModuleBuilder" />.</summary>
		/// <returns>The dynamic assembly that defined the current dynamic module.</returns>
		public override Assembly Assembly
		{
			get
			{
				return this.assemblyb;
			}
		}

		/// <summary>A string that indicates that this is an in-memory module.</summary>
		/// <returns>Text that indicates that this is an in-memory module.</returns>
		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets a string that represents the name of the dynamic module.</summary>
		/// <returns>The name of the dynamic module.</returns>
		public override string ScopeName
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets a universally unique identifier (UUID) that can be used to distinguish between two versions of a module.</summary>
		/// <returns>A <see cref="T:System.Guid" /> that can be used to distinguish between two versions of a module.</returns>
		public override Guid ModuleVersionId
		{
			get
			{
				return this.GetModuleVersionId();
			}
		}

		/// <summary>Gets a value indicating whether the object is a resource.</summary>
		/// <returns>
		///   <see langword="true" /> if the object is a resource; otherwise, <see langword="false" />.</returns>
		public override bool IsResource()
		{
			return false;
		}

		/// <summary>Returns the module-level method that matches the specified criteria.</summary>
		/// <param name="name">The method name.</param>
		/// <param name="bindingAttr">A combination of <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <param name="binder">An object that implements <see langword="Binder" />, containing properties related to this method.</param>
		/// <param name="callConvention">The calling convention for the method.</param>
		/// <param name="types">The parameter types of the method.</param>
		/// <param name="modifiers">An array of parameter modifiers used to make binding work with parameter signatures in which the types have been modified.</param>
		/// <returns>A method that is defined at the module level, and matches the specified criteria; or <see langword="null" /> if such a method does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />, <paramref name="types" /> is <see langword="null" />, or an element of <paramref name="types" /> is <see langword="null" />.</exception>
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (this.global_type_created == null)
			{
				return null;
			}
			if (types == null)
			{
				return this.global_type_created.GetMethod(name);
			}
			return this.global_type_created.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
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
		public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveField(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
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
		public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveMember(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		internal MemberInfo ResolveOrGetRegisteredToken(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError error;
			MemberInfo memberInfo = RuntimeModule.ResolveMemberToken(this._impl, metadataToken, RuntimeModule.ptrs_from_types(genericTypeArguments), RuntimeModule.ptrs_from_types(genericMethodArguments), out error);
			if (memberInfo != null)
			{
				return memberInfo;
			}
			memberInfo = (this.GetRegisteredToken(metadataToken) as MemberInfo);
			if (memberInfo == null)
			{
				throw RuntimeModule.resolve_token_exception(this.Name, metadataToken, error, "MemberInfo");
			}
			return memberInfo;
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
		public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveMethod(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		/// <summary>Returns the string identified by the specified metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a string in the string heap of the module.</param>
		/// <returns>A <see cref="T:System.String" /> containing a string value from the metadata string heap.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a token for a string in the scope of the current module.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public override string ResolveString(int metadataToken)
		{
			return RuntimeModule.ResolveString(this, this._impl, metadataToken);
		}

		/// <summary>Returns the signature blob identified by a metadata token.</summary>
		/// <param name="metadataToken">A metadata token that identifies a signature in the module.</param>
		/// <returns>An array of bytes representing the signature blob.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="metadataToken" /> is not a valid <see langword="MemberRef" />, <see langword="MethodDef" />, <see langword="TypeSpec" />, signature, or <see langword="FieldDef" /> token in the scope of the current module.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="metadataToken" /> is not a valid token in the scope of the current module.</exception>
		public override byte[] ResolveSignature(int metadataToken)
		{
			return RuntimeModule.ResolveSignature(this, this._impl, metadataToken);
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
		public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveType(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		/// <summary>Returns a value that indicates whether this instance is equal to the specified object.</summary>
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

		/// <summary>Returns a value that indicates whether the specified attribute type has been applied to this module.</summary>
		/// <param name="attributeType">The type of custom attribute to test for.</param>
		/// <param name="inherit">This argument is ignored for objects of this type.</param>
		/// <returns>
		///   <see langword="true" /> if one or more instances of <paramref name="attributeType" /> have been applied to this module; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="attributeType" /> is not a <see cref="T:System.Type" /> object supplied by the runtime. For example, <paramref name="attributeType" /> is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> object.</exception>
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return base.IsDefined(attributeType, inherit);
		}

		/// <summary>Returns all the custom attributes that have been applied to the current <see cref="T:System.Reflection.Emit.ModuleBuilder" />.</summary>
		/// <param name="inherit">This argument is ignored for objects of this type.</param>
		/// <returns>An array that contains the custom attributes; the array is empty if there are no attributes.</returns>
		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.GetCustomAttributes(null, inherit);
		}

		/// <summary>Returns all the custom attributes that have been applied to the current <see cref="T:System.Reflection.Emit.ModuleBuilder" />, and that derive from a specified attribute type.</summary>
		/// <param name="attributeType">The base type from which attributes derive.</param>
		/// <param name="inherit">This argument is ignored for objects of this type.</param>
		/// <returns>An array that contains the custom attributes that are derived, at any level, from <paramref name="attributeType" />; the array is empty if there are no such attributes.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="attributeType" /> is not a <see cref="T:System.Type" /> object supplied by the runtime. For example, <paramref name="attributeType" /> is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> object.</exception>
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (this.cattrs == null || this.cattrs.Length == 0)
			{
				return Array.Empty<object>();
			}
			if (attributeType is TypeBuilder)
			{
				throw new InvalidOperationException("First argument to GetCustomAttributes can't be a TypeBuilder");
			}
			List<object> list = new List<object>();
			for (int i = 0; i < this.cattrs.Length; i++)
			{
				Type type = this.cattrs[i].Ctor.GetType();
				if (type is TypeBuilder)
				{
					throw new InvalidOperationException("Can't construct custom attribute for TypeBuilder type");
				}
				if (attributeType == null || attributeType.IsAssignableFrom(type))
				{
					list.Add(this.cattrs[i].Invoke());
				}
			}
			return list.ToArray();
		}

		/// <summary>Returns a module-level field, defined in the .sdata region of the portable executable (PE) file, that has the specified name and binding attributes.</summary>
		/// <param name="name">The field name.</param>
		/// <param name="bindingAttr">A combination of the <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <returns>A field that has the specified name and binding attributes, or <see langword="null" /> if the field does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (this.global_type_created == null)
			{
				throw new InvalidOperationException("Module-level fields cannot be retrieved until after the CreateGlobalFunctions method has been called for the module.");
			}
			return this.global_type_created.GetField(name, bindingAttr);
		}

		/// <summary>Returns all fields defined in the .sdata region of the portable executable (PE) file that match the specified binding flags.</summary>
		/// <param name="bindingFlags">A combination of the <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <returns>An array of fields that match the specified flags; the array is empty if no such fields exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="name" /> parameter is <see langword="null" />.</exception>
		public override FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			if (this.global_type_created == null)
			{
				throw new InvalidOperationException("Module-level fields cannot be retrieved until after the CreateGlobalFunctions method has been called for the module.");
			}
			return this.global_type_created.GetFields(bindingFlags);
		}

		/// <summary>Returns all the methods that have been defined at the module level for the current <see cref="T:System.Reflection.Emit.ModuleBuilder" />, and that match the specified binding flags.</summary>
		/// <param name="bindingFlags">A combination of <see langword="BindingFlags" /> bit flags used to control the search.</param>
		/// <returns>An array that contains all the module-level methods that match <paramref name="bindingFlags" />.</returns>
		public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			if (this.global_type_created == null)
			{
				throw new InvalidOperationException("Module-level methods cannot be retrieved until after the CreateGlobalFunctions method has been called for the module.");
			}
			return this.global_type_created.GetMethods(bindingFlags);
		}

		/// <summary>Gets a token that identifies the current dynamic module in metadata.</summary>
		/// <returns>An integer token that identifies the current module in metadata.</returns>
		public override int MetadataToken
		{
			get
			{
				return RuntimeModule.get_MetadataToken(this);
			}
		}

		internal ModuleBuilder()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal IntPtr _impl;

		internal Assembly assembly;

		internal string fqname;

		internal string name;

		internal string scopename;

		internal bool is_resource;

		internal int token;

		private UIntPtr dynamic_image;

		private int num_types;

		private TypeBuilder[] types;

		private CustomAttributeBuilder[] cattrs;

		private byte[] guid;

		private int table_idx;

		internal AssemblyBuilder assemblyb;

		private MethodBuilder[] global_methods;

		private FieldBuilder[] global_fields;

		private bool is_main;

		private MonoResource[] resources;

		private IntPtr unparented_classes;

		private int[] table_indexes;

		private TypeBuilder global_type;

		private Type global_type_created;

		private Dictionary<TypeName, TypeBuilder> name_cache;

		private Dictionary<string, int> us_string_cache;

		private bool transient;

		private ModuleBuilderTokenGenerator token_gen;

		private Hashtable resource_writers;

		private ISymbolWriter symbolWriter;

		private static bool has_warned_about_symbolWriter;

		private static int typeref_tokengen = 33554431;

		private static int typedef_tokengen = 50331647;

		private static int typespec_tokengen = 469762047;

		private static int memberref_tokengen = 184549375;

		private static int methoddef_tokengen = 117440511;

		private Dictionary<MemberInfo, int> inst_tokens;

		private Dictionary<MemberInfo, int> inst_tokens_open;
	}
}
