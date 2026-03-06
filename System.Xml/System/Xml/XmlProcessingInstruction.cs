using System;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents a processing instruction, which XML defines to keep processor-specific information in the text of the document.</summary>
	public class XmlProcessingInstruction : XmlLinkedNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlProcessingInstruction" /> class.</summary>
		/// <param name="target">The target of the processing instruction; see the <see cref="P:System.Xml.XmlProcessingInstruction.Target" /> property.</param>
		/// <param name="data">The content of the instruction; see the <see cref="P:System.Xml.XmlProcessingInstruction.Data" /> property.</param>
		/// <param name="doc">The parent XML document.</param>
		protected internal XmlProcessingInstruction(string target, string data, XmlDocument doc) : base(doc)
		{
			this.target = target;
			this.data = data;
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>For processing instruction nodes, this property returns the target of the processing instruction.</returns>
		public override string Name
		{
			get
			{
				if (this.target != null)
				{
					return this.target;
				}
				return string.Empty;
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For processing instruction nodes, this property returns the target of the processing instruction.</returns>
		public override string LocalName
		{
			get
			{
				return this.Name;
			}
		}

		/// <summary>Gets or sets the value of the node.</summary>
		/// <returns>The entire content of the processing instruction, excluding the target.</returns>
		/// <exception cref="T:System.ArgumentException">Node is read-only. </exception>
		public override string Value
		{
			get
			{
				return this.data;
			}
			set
			{
				this.Data = value;
			}
		}

		/// <summary>Gets the target of the processing instruction.</summary>
		/// <returns>The target of the processing instruction.</returns>
		public string Target
		{
			get
			{
				return this.target;
			}
		}

		/// <summary>Gets or sets the content of the processing instruction, excluding the target.</summary>
		/// <returns>The content of the processing instruction, excluding the target.</returns>
		public string Data
		{
			get
			{
				return this.data;
			}
			set
			{
				XmlNode parentNode = this.ParentNode;
				XmlNodeChangedEventArgs eventArgs = this.GetEventArgs(this, parentNode, parentNode, this.data, value, XmlNodeChangedAction.Change);
				if (eventArgs != null)
				{
					this.BeforeEvent(eventArgs);
				}
				this.data = value;
				if (eventArgs != null)
				{
					this.AfterEvent(eventArgs);
				}
			}
		}

		/// <summary>Gets or sets the concatenated values of the node and all its children.</summary>
		/// <returns>The concatenated values of the node and all its children.</returns>
		public override string InnerText
		{
			get
			{
				return this.data;
			}
			set
			{
				this.Data = value;
			}
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>For <see langword="XmlProcessingInstruction" /> nodes, this value is XmlNodeType.ProcessingInstruction.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. </param>
		/// <returns>The duplicate node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateProcessingInstruction(this.target, this.data);
		}

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			w.WriteProcessingInstruction(this.target, this.data);
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />. Because ProcessingInstruction nodes do not have children, this method has no effect.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
		}

		internal override string XPLocalName
		{
			get
			{
				return this.Name;
			}
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.ProcessingInstruction;
			}
		}

		private string target;

		private string data;
	}
}
