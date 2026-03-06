using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization.Diagnostics;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	/// <summary>Allows the transformation of a set of .NET Framework types that are used in data contracts into an XML schema file (.xsd).</summary>
	public class XsdDataContractExporter
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.XsdDataContractExporter" /> class.</summary>
		public XsdDataContractExporter()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Serialization.XsdDataContractExporter" /> class with the specified set of schemas.</summary>
		/// <param name="schemas">An <see cref="T:System.Xml.Schema.XmlSchemaSet" /> that contains the schemas to be exported.</param>
		public XsdDataContractExporter(XmlSchemaSet schemas)
		{
			this.schemas = schemas;
		}

		/// <summary>Gets or sets an <see cref="T:System.Runtime.Serialization.ExportOptions" /> that contains options that can be set for the export operation.</summary>
		/// <returns>An <see cref="T:System.Runtime.Serialization.ExportOptions" /> that contains options used to customize how types are exported to schemas.</returns>
		public ExportOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		/// <summary>Gets the collection of exported XML schemas.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaSet" /> that contains the schemas transformed from the set of common language runtime (CLR) types after calling the <see cref="Overload:System.Runtime.Serialization.XsdDataContractExporter.Export" /> method.</returns>
		public XmlSchemaSet Schemas
		{
			get
			{
				XmlSchemaSet schemaSet = this.GetSchemaSet();
				SchemaImporter.CompileSchemaSet(schemaSet);
				return schemaSet;
			}
		}

		private XmlSchemaSet GetSchemaSet()
		{
			if (this.schemas == null)
			{
				this.schemas = new XmlSchemaSet();
				this.schemas.XmlResolver = null;
			}
			return this.schemas;
		}

		private DataContractSet DataContractSet
		{
			get
			{
				if (this.dataContractSet == null)
				{
					this.dataContractSet = new DataContractSet((this.Options == null) ? null : this.Options.GetSurrogate());
				}
				return this.dataContractSet;
			}
		}

		private void TraceExportBegin()
		{
			if (DiagnosticUtility.ShouldTraceInformation)
			{
				TraceUtility.Trace(TraceEventType.Information, 196616, SR.GetString("XSD export begins"));
			}
		}

		private void TraceExportEnd()
		{
			if (DiagnosticUtility.ShouldTraceInformation)
			{
				TraceUtility.Trace(TraceEventType.Information, 196617, SR.GetString("XSD export ends"));
			}
		}

		private void TraceExportError(Exception exception)
		{
			if (DiagnosticUtility.ShouldTraceError)
			{
				TraceUtility.Trace(TraceEventType.Error, 196620, SR.GetString("XSD export error"), null, exception);
			}
		}

		/// <summary>Transforms the types contained in the specified collection of assemblies.</summary>
		/// <param name="assemblies">A <see cref="T:System.Collections.Generic.ICollection`1" /> (of <see cref="T:System.Reflection.Assembly" />) that contains the types to export.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="assemblies" /> argument is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">An <see cref="T:System.Reflection.Assembly" /> in the collection is <see langword="null" />.</exception>
		public void Export(ICollection<Assembly> assemblies)
		{
			if (assemblies == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
			}
			this.TraceExportBegin();
			DataContractSet dataContractSet = (this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet);
			try
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null assembly.", new object[]
						{
							"assemblies"
						})));
					}
					Type[] types = assembly.GetTypes();
					for (int i = 0; i < types.Length; i++)
					{
						this.CheckAndAddType(types[i]);
					}
				}
				this.Export();
			}
			catch (Exception exception)
			{
				if (Fx.IsFatal(exception))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(exception);
				throw;
			}
			this.TraceExportEnd();
		}

		/// <summary>Transforms the types contained in the <see cref="T:System.Collections.Generic.ICollection`1" /> passed to this method.</summary>
		/// <param name="types">A  <see cref="T:System.Collections.Generic.ICollection`1" /> (of <see cref="T:System.Type" />) that contains the types to export.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="types" /> argument is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">A type in the collection is <see langword="null" />.</exception>
		public void Export(ICollection<Type> types)
		{
			if (types == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
			}
			this.TraceExportBegin();
			DataContractSet dataContractSet = (this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet);
			try
			{
				foreach (Type type in types)
				{
					if (type == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null type.", new object[]
						{
							"types"
						})));
					}
					this.AddType(type);
				}
				this.Export();
			}
			catch (Exception exception)
			{
				if (Fx.IsFatal(exception))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(exception);
				throw;
			}
			this.TraceExportEnd();
		}

		/// <summary>Transforms the specified .NET Framework type into an XML schema definition language (XSD) schema.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> to transform into an XML schema.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> argument is <see langword="null" />.</exception>
		public void Export(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			this.TraceExportBegin();
			DataContractSet dataContractSet = (this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet);
			try
			{
				this.AddType(type);
				this.Export();
			}
			catch (Exception exception)
			{
				if (Fx.IsFatal(exception))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(exception);
				throw;
			}
			this.TraceExportEnd();
		}

		/// <summary>Returns the contract name and contract namespace for the <see cref="T:System.Type" />.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> that was exported.</param>
		/// <returns>An <see cref="T:System.Xml.XmlQualifiedName" /> that represents the contract name of the type and its namespace.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> argument is <see langword="null" />.</exception>
		public XmlQualifiedName GetSchemaTypeName(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			type = this.GetSurrogatedType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			XmlDataContract xmlDataContract = dataContract as XmlDataContract;
			if (xmlDataContract != null && xmlDataContract.IsAnonymous)
			{
				return XmlQualifiedName.Empty;
			}
			return dataContract.StableName;
		}

		/// <summary>Returns the XML schema type for the specified type.</summary>
		/// <param name="type">The type to return a schema for.</param>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaType" /> that contains the XML schema.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> argument is <see langword="null" />.</exception>
		public XmlSchemaType GetSchemaType(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			type = this.GetSurrogatedType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			XmlDataContract xmlDataContract = dataContract as XmlDataContract;
			if (xmlDataContract != null && xmlDataContract.IsAnonymous)
			{
				return xmlDataContract.XsdType;
			}
			return null;
		}

		/// <summary>Returns the top-level name and namespace for the <see cref="T:System.Type" />.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> to query.</param>
		/// <returns>The <see cref="T:System.Xml.XmlQualifiedName" /> that represents the top-level name and namespace for this <see cref="T:System.Type" />, which is written to the stream when writing this object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="type" /> argument is <see langword="null" />.</exception>
		public XmlQualifiedName GetRootElementName(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			type = this.GetSurrogatedType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			if (dataContract.HasRoot)
			{
				return new XmlQualifiedName(dataContract.TopLevelElementName.Value, dataContract.TopLevelElementNamespace.Value);
			}
			return null;
		}

		private Type GetSurrogatedType(Type type)
		{
			IDataContractSurrogate surrogate;
			if (this.options != null && (surrogate = this.Options.GetSurrogate()) != null)
			{
				type = DataContractSurrogateCaller.GetDataContractType(surrogate, type);
			}
			return type;
		}

		private void CheckAndAddType(Type type)
		{
			type = this.GetSurrogatedType(type);
			if (!type.ContainsGenericParameters && DataContract.IsTypeSerializable(type))
			{
				this.AddType(type);
			}
		}

		private void AddType(Type type)
		{
			this.DataContractSet.Add(type);
		}

		private void Export()
		{
			this.AddKnownTypes();
			new SchemaExporter(this.GetSchemaSet(), this.DataContractSet).Export();
		}

		private void AddKnownTypes()
		{
			if (this.Options != null)
			{
				Collection<Type> knownTypes = this.Options.KnownTypes;
				if (knownTypes != null)
				{
					for (int i = 0; i < knownTypes.Count; i++)
					{
						Type type = knownTypes[i];
						if (type == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null known type.")));
						}
						this.AddType(type);
					}
				}
			}
		}

		/// <summary>Gets a value that indicates whether the set of .common language runtime (CLR) types contained in a set of assemblies can be exported.</summary>
		/// <param name="assemblies">A <see cref="T:System.Collections.Generic.ICollection`1" /> of <see cref="T:System.Reflection.Assembly" /> that contains the assemblies with the types to export.</param>
		/// <returns>
		///   <see langword="true" /> if the types can be exported; otherwise, <see langword="false" />.</returns>
		public bool CanExport(ICollection<Assembly> assemblies)
		{
			if (assemblies == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
			}
			DataContractSet dataContractSet = (this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet);
			bool result;
			try
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null assembly.", new object[]
						{
							"assemblies"
						})));
					}
					Type[] types = assembly.GetTypes();
					for (int i = 0; i < types.Length; i++)
					{
						this.CheckAndAddType(types[i]);
					}
				}
				this.AddKnownTypes();
				result = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				result = false;
			}
			catch (Exception exception)
			{
				if (Fx.IsFatal(exception))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(exception);
				throw;
			}
			return result;
		}

		/// <summary>Gets a value that indicates whether the set of .common language runtime (CLR) types contained in a <see cref="T:System.Collections.Generic.ICollection`1" /> can be exported.</summary>
		/// <param name="types">A <see cref="T:System.Collections.Generic.ICollection`1" /> that contains the specified types to export.</param>
		/// <returns>
		///   <see langword="true" /> if the types can be exported; otherwise, <see langword="false" />.</returns>
		public bool CanExport(ICollection<Type> types)
		{
			if (types == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
			}
			DataContractSet dataContractSet = (this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet);
			bool result;
			try
			{
				foreach (Type type in types)
				{
					if (type == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR.GetString("Cannot export null type.", new object[]
						{
							"types"
						})));
					}
					this.AddType(type);
				}
				this.AddKnownTypes();
				result = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				result = false;
			}
			catch (Exception exception)
			{
				if (Fx.IsFatal(exception))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(exception);
				throw;
			}
			return result;
		}

		/// <summary>Gets a value that indicates whether the specified common language runtime (CLR) type can be exported.</summary>
		/// <param name="type">The <see cref="T:System.Type" /> to export.</param>
		/// <returns>
		///   <see langword="true" /> if the type can be exported; otherwise, <see langword="false" />.</returns>
		public bool CanExport(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			DataContractSet dataContractSet = (this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet);
			bool result;
			try
			{
				this.AddType(type);
				this.AddKnownTypes();
				result = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				result = false;
			}
			catch (Exception exception)
			{
				if (Fx.IsFatal(exception))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(exception);
				throw;
			}
			return result;
		}

		private ExportOptions options;

		private XmlSchemaSet schemas;

		private DataContractSet dataContractSet;
	}
}
