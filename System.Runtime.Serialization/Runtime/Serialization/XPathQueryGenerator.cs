using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization
{
	/// <summary>When given a class representing a data contract, and metadata representing a member of the contract, produces an XPath query for the member.</summary>
	public static class XPathQueryGenerator
	{
		/// <summary>Creates an XPath from a data contract using the specified data contract type, array of metadata elements, and namespaces.</summary>
		/// <param name="type">The type that represents a data contract.</param>
		/// <param name="pathToMember">The metadata, generated using the <see cref="Overload:System.Type.GetMember" /> method of the <see cref="T:System.Type" /> class, that points to the specific data member used to generate the query.</param>
		/// <param name="namespaces">The XML namespaces and their prefixes found in the data contract.</param>
		/// <returns>
		///   <see cref="T:System.String" />  
		///
		/// The XPath generated from the type and member data.</returns>
		public static string CreateFromDataContractSerializer(Type type, MemberInfo[] pathToMember, out XmlNamespaceManager namespaces)
		{
			return XPathQueryGenerator.CreateFromDataContractSerializer(type, pathToMember, null, out namespaces);
		}

		/// <summary>Creates an XPath from a data contract using the specified contract data type, array of metadata elements, the top level element, and namespaces.</summary>
		/// <param name="type">The type that represents a data contract.</param>
		/// <param name="pathToMember">The metadata, generated using the <see cref="Overload:System.Type.GetMember" /> method of the <see cref="T:System.Type" /> class, that points to the specific data member used to generate the query.</param>
		/// <param name="rootElementXpath">The top level element in the xpath.</param>
		/// <param name="namespaces">The XML namespaces and their prefixes found in the data contract.</param>
		/// <returns>
		///   <see cref="T:System.String" />  
		///
		/// The XPath generated from the type and member data.</returns>
		public static string CreateFromDataContractSerializer(Type type, MemberInfo[] pathToMember, StringBuilder rootElementXpath, out XmlNamespaceManager namespaces)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			if (pathToMember == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("pathToMember"));
			}
			DataContract dataContract = DataContract.GetDataContract(type);
			XPathQueryGenerator.ExportContext exportContext;
			if (rootElementXpath == null)
			{
				exportContext = new XPathQueryGenerator.ExportContext(dataContract);
			}
			else
			{
				exportContext = new XPathQueryGenerator.ExportContext(rootElementXpath);
			}
			for (int i = 0; i < pathToMember.Length; i++)
			{
				dataContract = XPathQueryGenerator.ProcessDataContract(dataContract, exportContext, pathToMember[i]);
			}
			namespaces = exportContext.Namespaces;
			return exportContext.XPath;
		}

		private static DataContract ProcessDataContract(DataContract contract, XPathQueryGenerator.ExportContext context, MemberInfo memberNode)
		{
			if (contract is ClassDataContract)
			{
				return XPathQueryGenerator.ProcessClassDataContract((ClassDataContract)contract, context, memberNode);
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString("The path to member was not found for XPath query generator.")));
		}

		private static DataContract ProcessClassDataContract(ClassDataContract contract, XPathQueryGenerator.ExportContext context, MemberInfo memberNode)
		{
			string prefix = context.SetNamespace(contract.Namespace.Value);
			foreach (DataMember dataMember in XPathQueryGenerator.GetDataMembers(contract))
			{
				if (dataMember.MemberInfo.Name == memberNode.Name && dataMember.MemberInfo.DeclaringType.IsAssignableFrom(memberNode.DeclaringType))
				{
					context.WriteChildToContext(dataMember, prefix);
					return dataMember.MemberTypeContract;
				}
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString("The path to member was not found for XPath query generator.")));
		}

		private static IEnumerable<DataMember> GetDataMembers(ClassDataContract contract)
		{
			if (contract.BaseContract != null)
			{
				foreach (DataMember dataMember in XPathQueryGenerator.GetDataMembers(contract.BaseContract))
				{
					yield return dataMember;
				}
				IEnumerator<DataMember> enumerator = null;
			}
			if (contract.Members != null)
			{
				foreach (DataMember dataMember2 in contract.Members)
				{
					yield return dataMember2;
				}
				List<DataMember>.Enumerator enumerator2 = default(List<DataMember>.Enumerator);
			}
			yield break;
			yield break;
		}

		private const string XPathSeparator = "/";

		private const string NsSeparator = ":";

		private class ExportContext
		{
			public ExportContext(DataContract rootContract)
			{
				this.namespaces = new XmlNamespaceManager(new NameTable());
				string str = this.SetNamespace(rootContract.TopLevelElementNamespace.Value);
				this.xPathBuilder = new StringBuilder("/" + str + ":" + rootContract.TopLevelElementName.Value);
			}

			public ExportContext(StringBuilder rootContractXPath)
			{
				this.namespaces = new XmlNamespaceManager(new NameTable());
				this.xPathBuilder = rootContractXPath;
			}

			public void WriteChildToContext(DataMember contextMember, string prefix)
			{
				this.xPathBuilder.Append("/" + prefix + ":" + contextMember.Name);
			}

			public XmlNamespaceManager Namespaces
			{
				get
				{
					return this.namespaces;
				}
			}

			public string XPath
			{
				get
				{
					return this.xPathBuilder.ToString();
				}
			}

			public string SetNamespace(string ns)
			{
				string text = this.namespaces.LookupPrefix(ns);
				if (text == null || text.Length == 0)
				{
					string str = "xg";
					int num = this.nextPrefix;
					this.nextPrefix = num + 1;
					text = str + num.ToString(NumberFormatInfo.InvariantInfo);
					this.Namespaces.AddNamespace(text, ns);
				}
				return text;
			}

			private XmlNamespaceManager namespaces;

			private int nextPrefix;

			private StringBuilder xPathBuilder;
		}
	}
}
