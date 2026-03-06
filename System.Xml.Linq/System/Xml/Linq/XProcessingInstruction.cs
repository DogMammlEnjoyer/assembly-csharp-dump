using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Xml.Linq
{
	/// <summary>Represents an XML processing instruction.</summary>
	public class XProcessingInstruction : XNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XProcessingInstruction" /> class.</summary>
		/// <param name="target">A <see cref="T:System.String" /> containing the target application for this <see cref="T:System.Xml.Linq.XProcessingInstruction" />.</param>
		/// <param name="data">The string data for this <see cref="T:System.Xml.Linq.XProcessingInstruction" />.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="target" /> or <paramref name="data" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> does not follow the constraints of an XML name.</exception>
		public XProcessingInstruction(string target, string data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			XProcessingInstruction.ValidateName(target);
			this.target = target;
			this.data = data;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Linq.XProcessingInstruction" /> class.</summary>
		/// <param name="other">The <see cref="T:System.Xml.Linq.XProcessingInstruction" /> node to copy from.</param>
		public XProcessingInstruction(XProcessingInstruction other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.target = other.target;
			this.data = other.data;
		}

		internal XProcessingInstruction(XmlReader r)
		{
			this.target = r.Name;
			this.data = r.Value;
			r.Read();
		}

		/// <summary>Gets or sets the string value of this processing instruction.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the string value of this processing instruction.</returns>
		/// <exception cref="T:System.ArgumentNullException">The string <paramref name="value" /> is <see langword="null" />.</exception>
		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
				this.data = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Value);
				}
			}
		}

		/// <summary>Gets the node type for this node.</summary>
		/// <returns>The node type. For <see cref="T:System.Xml.Linq.XProcessingInstruction" /> objects, this value is <see cref="F:System.Xml.XmlNodeType.ProcessingInstruction" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		/// <summary>Gets or sets a string containing the target application for this processing instruction.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the target application for this processing instruction.</returns>
		/// <exception cref="T:System.ArgumentNullException">The string <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> does not follow the constraints of an XML name.</exception>
		public string Target
		{
			get
			{
				return this.target;
			}
			set
			{
				XProcessingInstruction.ValidateName(value);
				bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
				this.target = value;
				if (flag)
				{
					base.NotifyChanged(this, XObjectChangeEventArgs.Name);
				}
			}
		}

		/// <summary>Writes this processing instruction to an <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> to write this processing instruction to.</param>
		public override void WriteTo(XmlWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.WriteProcessingInstruction(this.target, this.data);
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
			return writer.WriteProcessingInstructionAsync(this.target, this.data);
		}

		internal override XNode CloneNode()
		{
			return new XProcessingInstruction(this);
		}

		internal override bool DeepEquals(XNode node)
		{
			XProcessingInstruction xprocessingInstruction = node as XProcessingInstruction;
			return xprocessingInstruction != null && this.target == xprocessingInstruction.target && this.data == xprocessingInstruction.data;
		}

		internal override int GetDeepHashCode()
		{
			return this.target.GetHashCode() ^ this.data.GetHashCode();
		}

		private static void ValidateName(string name)
		{
			XmlConvert.VerifyNCName(name);
			if (string.Equals(name, "xml", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException(SR.Format("'{0}' is an invalid name for a processing instruction.", name));
			}
		}

		internal string target;

		internal string data;
	}
}
