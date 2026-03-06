using System;

namespace System.CodeDom.Compiler
{
	/// <summary>Defines identifiers used to determine whether a code generator supports certain types of code elements.</summary>
	[Flags]
	public enum GeneratorSupport
	{
		/// <summary>Indicates the generator supports arrays of arrays.</summary>
		ArraysOfArrays = 1,
		/// <summary>Indicates the generator supports a program entry point method designation. This is used when building executables.</summary>
		EntryPointMethod = 2,
		/// <summary>Indicates the generator supports goto statements.</summary>
		GotoStatements = 4,
		/// <summary>Indicates the generator supports referencing multidimensional arrays. Currently, the CodeDom cannot be used to instantiate multidimensional arrays.</summary>
		MultidimensionalArrays = 8,
		/// <summary>Indicates the generator supports static constructors.</summary>
		StaticConstructors = 16,
		/// <summary>Indicates the generator supports <see langword="try...catch" /> statements.</summary>
		TryCatchStatements = 32,
		/// <summary>Indicates the generator supports return type attribute declarations.</summary>
		ReturnTypeAttributes = 64,
		/// <summary>Indicates the generator supports value type declarations.</summary>
		DeclareValueTypes = 128,
		/// <summary>Indicates the generator supports enumeration declarations.</summary>
		DeclareEnums = 256,
		/// <summary>Indicates the generator supports delegate declarations.</summary>
		DeclareDelegates = 512,
		/// <summary>Indicates the generator supports interface declarations.</summary>
		DeclareInterfaces = 1024,
		/// <summary>Indicates the generator supports event declarations.</summary>
		DeclareEvents = 2048,
		/// <summary>Indicates the generator supports assembly attributes.</summary>
		AssemblyAttributes = 4096,
		/// <summary>Indicates the generator supports parameter attributes.</summary>
		ParameterAttributes = 8192,
		/// <summary>Indicates the generator supports reference and out parameters.</summary>
		ReferenceParameters = 16384,
		/// <summary>Indicates the generator supports chained constructor arguments.</summary>
		ChainedConstructorArguments = 32768,
		/// <summary>Indicates the generator supports the declaration of nested types.</summary>
		NestedTypes = 65536,
		/// <summary>Indicates the generator supports the declaration of members that implement multiple interfaces.</summary>
		MultipleInterfaceMembers = 131072,
		/// <summary>Indicates the generator supports public static members.</summary>
		PublicStaticMembers = 262144,
		/// <summary>Indicates the generator supports complex expressions.</summary>
		ComplexExpressions = 524288,
		/// <summary>Indicates the generator supports compilation with Win32 resources.</summary>
		Win32Resources = 1048576,
		/// <summary>Indicates the generator supports compilation with .NET Framework resources. These can be default resources compiled directly into an assembly, or resources referenced in a satellite assembly.</summary>
		Resources = 2097152,
		/// <summary>Indicates the generator supports partial type declarations.</summary>
		PartialTypes = 4194304,
		/// <summary>Indicates the generator supports generic type references.</summary>
		GenericTypeReference = 8388608,
		/// <summary>Indicates the generator supports generic type declarations.</summary>
		GenericTypeDeclaration = 16777216,
		/// <summary>Indicates the generator supports the declaration of indexer properties.</summary>
		DeclareIndexerProperties = 33554432
	}
}
