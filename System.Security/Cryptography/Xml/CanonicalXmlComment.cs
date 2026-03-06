using System;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class CanonicalXmlComment : XmlComment, ICanonicalizableNode
	{
		public CanonicalXmlComment(string comment, XmlDocument doc, bool defaultNodeSetInclusionState, bool includeComments) : base(comment, doc)
		{
			this._isInNodeSet = defaultNodeSetInclusionState;
			this._includeComments = includeComments;
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

		public bool IncludeComments
		{
			get
			{
				return this._includeComments;
			}
		}

		public void Write(StringBuilder strBuilder, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (!this.IsInNodeSet || !this.IncludeComments)
			{
				return;
			}
			if (docPos == DocPosition.AfterRootElement)
			{
				strBuilder.Append('\n');
			}
			strBuilder.Append("<!--");
			strBuilder.Append(this.Value);
			strBuilder.Append("-->");
			if (docPos == DocPosition.BeforeRootElement)
			{
				strBuilder.Append('\n');
			}
		}

		public void WriteHash(HashAlgorithm hash, DocPosition docPos, AncestralNamespaceContextManager anc)
		{
			if (!this.IsInNodeSet || !this.IncludeComments)
			{
				return;
			}
			UTF8Encoding utf8Encoding = new UTF8Encoding(false);
			byte[] bytes = utf8Encoding.GetBytes("(char) 10");
			if (docPos == DocPosition.AfterRootElement)
			{
				hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			}
			bytes = utf8Encoding.GetBytes("<!--");
			hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			bytes = utf8Encoding.GetBytes(this.Value);
			hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			bytes = utf8Encoding.GetBytes("-->");
			hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			if (docPos == DocPosition.BeforeRootElement)
			{
				bytes = utf8Encoding.GetBytes("(char) 10");
				hash.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			}
		}

		private bool _isInNodeSet;

		private bool _includeComments;
	}
}
