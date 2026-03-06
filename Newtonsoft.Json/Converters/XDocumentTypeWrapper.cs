using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class XDocumentTypeWrapper : XObjectWrapper, IXmlDocumentType, IXmlNode
	{
		[NullableContext(1)]
		public XDocumentTypeWrapper(XDocumentType documentType) : base(documentType)
		{
			this._documentType = documentType;
		}

		[Nullable(1)]
		public string Name
		{
			[NullableContext(1)]
			get
			{
				return this._documentType.Name;
			}
		}

		public string System
		{
			get
			{
				return this._documentType.SystemId;
			}
		}

		public string Public
		{
			get
			{
				return this._documentType.PublicId;
			}
		}

		public string InternalSubset
		{
			get
			{
				return this._documentType.InternalSubset;
			}
		}

		public override string LocalName
		{
			get
			{
				return "DOCTYPE";
			}
		}

		[Nullable(1)]
		private readonly XDocumentType _documentType;
	}
}
