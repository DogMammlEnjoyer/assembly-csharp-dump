using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML comment.</summary>
	public class XComment : XNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XComment" /> class with the specified string content.</summary>
		/// <param name="value">A string that contains the contents of the new <see cref="T:System.Xml.Linq.XComment" /> object.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> parameter is <see langword="null" />.</exception>
		public XComment(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.value = value;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XComment" /> class from an existing comment node.</summary>
		/// <param name="other">The <see cref="T:System.Xml.Linq.XComment" /> node to copy from.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="other" /> parameter is <see langword="null" />.</exception>
		public XComment(XComment other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.value = other.value;
		}

		internal XComment(XmlReader r)
		{
			this.value = r.Value;
			r.Read();
		}

		/// <summary>Gets the node type for this node.</summary>
		/// <returns>The node type. For <see cref="T:System.Xml.Linq.XComment" /> objects, this value is <see cref="F:System.Xml.XmlNodeType.Comment" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Comment;
			}
		}

		/// <summary>Gets or sets the string value of this comment.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the string value of this comment.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> is <see langword="null" />.</exception>
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.value = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		/// <summary>Write this comment to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">An <see cref="T:System.Xml.XmlWriter" /> into which this method will write.</param>
		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteComment(this.value);
		}

		public override Task WriteToAsync(XmlWriter writer, CancellationToken cancellationToken)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			return writer.WriteCommentAsync(this.value);
		}

		internal override XNode CloneNode()
		{
			return new XComment(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XComment xcomment = node as XComment;
			return xcomment != null && this.value == xcomment.value;
		}

		internal override int GetDeepHashCode()
		{
			return this.value.GetHashCode();
		}

		internal string value;
	}
}
