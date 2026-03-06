using System;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class CanonicalXmlAttribute : XmlAttribute, ICanonicalizableNode
	{
		public CanonicalXmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc, bool defaultNodeSetInclusionState) : base(prefix, localName, namespaceURI, doc)
		{
			this.IsInNodeSet = defaultNodeSetInclusionState;
		}

		public bool IsInNodeSet
		{
			get
			{
				return this._isInNodeSet;
			}
			set
			{
				this._isInNodeSet = value;
			}
		}

		public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			strBuilder.Append(" " + this.Name + "=\"");
			strBuilder.Append(Utils.EscapeAttributeValue(this.Value));
			strBuilder.Append("\"");
		}

		public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			UTF8Encoding utf8Encoding = new UTF8Encoding(false);
			byte[] bytes = utf8Encoding.GetBytes(" " + this.Name + "=\"");
			hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			bytes = utf8Encoding.GetBytes(Utils.EscapeAttributeValue(this.Value));
			hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			bytes = utf8Encoding.GetBytes("\"");
			hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
		}

		private bool _isInNodeSet;
	}
}
