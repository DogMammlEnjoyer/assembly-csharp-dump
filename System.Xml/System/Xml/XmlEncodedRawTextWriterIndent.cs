using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlEncodedRawTextWriterIndent : XmlEncodedRawTextWriter
	{
		public XmlEncodedRawTextWriterIndent(TextWriter writer, XmlWriterSettings settings) : base(writer, settings)
		{
			this.Init(settings);
		}

		public XmlEncodedRawTextWriterIndent(Stream stream, XmlWriterSettings settings) : base(stream, settings)
		{
			this.Init(settings);
		}

		public override XmlWriterSettings Settings
		{
			get
			{
				XmlWriterSettings settings = base.Settings;
				settings.ReadOnly = false;
				settings.Indent = true;
				settings.IndentChars = this.indentChars;
				settings.NewLineOnAttributes = this.newLineOnAttributes;
				settings.ReadOnly = true;
				return settings;
			}
		}

		public override void WriteDocType(string name, string pubid, string sysid, string subset)
		{
			if (!this.mixedContent && this.textPos != this.bufPos)
			{
				this.WriteIndent();
			}
			base.WriteDocType(name, pubid, sysid, subset);
		}

		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			if (!this.mixedContent && this.textPos != this.bufPos)
			{
				this.WriteIndent();
			}
			this.indentLevel++;
			this.mixedContentStack.PushBit(this.mixedContent);
			base.WriteStartElement(prefix, localName, ns);
		}

		internal override void StartElementContent()
		{
			if (this.indentLevel == 1 && this.conformanceLevel == ConformanceLevel.Document)
			{
				this.mixedContent = false;
			}
			else
			{
				this.mixedContent = this.mixedContentStack.PeekBit();
			}
			base.StartElementContent();
		}

		internal override void OnRootElement(ConformanceLevel currentConformanceLevel)
		{
			this.conformanceLevel = currentConformanceLevel;
		}

		internal override void WriteEndElement(string prefix, string localName, string ns)
		{
			this.indentLevel--;
			if (!this.mixedContent && this.contentPos != this.bufPos && this.textPos != this.bufPos)
			{
				this.WriteIndent();
			}
			this.mixedContent = this.mixedContentStack.PopBit();
			base.WriteEndElement(prefix, localName, ns);
		}

		internal override void WriteFullEndElement(string prefix, string localName, string ns)
		{
			this.indentLevel--;
			if (!this.mixedContent && this.contentPos != this.bufPos && this.textPos != this.bufPos)
			{
				this.WriteIndent();
			}
			this.mixedContent = this.mixedContentStack.PopBit();
			base.WriteFullEndElement(prefix, localName, ns);
		}

		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if (this.newLineOnAttributes)
			{
				this.WriteIndent();
			}
			base.WriteStartAttribute(prefix, localName, ns);
		}

		public override void WriteCData(string text)
		{
			this.mixedContent = true;
			base.WriteCData(text);
		}

		public override void WriteComment(string text)
		{
			if (!this.mixedContent && this.textPos != this.bufPos)
			{
				this.WriteIndent();
			}
			base.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string target, string text)
		{
			if (!this.mixedContent && this.textPos != this.bufPos)
			{
				this.WriteIndent();
			}
			base.WriteProcessingInstruction(target, text);
		}

		public override void WriteEntityRef(string name)
		{
			this.mixedContent = true;
			base.WriteEntityRef(name);
		}

		public override void WriteCharEntity(char ch)
		{
			this.mixedContent = true;
			base.WriteCharEntity(ch);
		}

		public override void WriteSurrogateCharEntity(char lowChar, char highChar)
		{
			this.mixedContent = true;
			base.WriteSurrogateCharEntity(lowChar, highChar);
		}

		public override void WriteWhitespace(string ws)
		{
			this.mixedContent = true;
			base.WriteWhitespace(ws);
		}

		public override void WriteString(string text)
		{
			this.mixedContent = true;
			base.WriteString(text);
		}

		public override void WriteChars(char[] buffer, int index, int count)
		{
			this.mixedContent = true;
			base.WriteChars(buffer, index, count);
		}

		public override void WriteRaw(char[] buffer, int index, int count)
		{
			this.mixedContent = true;
			base.WriteRaw(buffer, index, count);
		}

		public override void WriteRaw(string data)
		{
			this.mixedContent = true;
			base.WriteRaw(data);
		}

		public override void WriteBase64(byte[] buffer, int index, int count)
		{
			this.mixedContent = true;
			base.WriteBase64(buffer, index, count);
		}

		private void Init(XmlWriterSettings settings)
		{
			this.indentLevel = 0;
			this.indentChars = settings.IndentChars;
			this.newLineOnAttributes = settings.NewLineOnAttributes;
			this.mixedContentStack = new BitStack();
			if (this.checkCharacters)
			{
				if (this.newLineOnAttributes)
				{
					base.ValidateContentChars(this.indentChars, "IndentChars", true);
					base.ValidateContentChars(this.newLineChars, "NewLineChars", true);
					return;
				}
				base.ValidateContentChars(this.indentChars, "IndentChars", false);
				if (this.newLineHandling != NewLineHandling.Replace)
				{
					base.ValidateContentChars(this.newLineChars, "NewLineChars", false);
				}
			}
		}

		private void WriteIndent()
		{
			base.RawText(this.newLineChars);
			for (int i = this.indentLevel; i > 0; i--)
			{
				base.RawText(this.indentChars);
			}
		}

		public override Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset)
		{
			XmlEncodedRawTextWriterIndent.<WriteDocTypeAsync>d__31 <WriteDocTypeAsync>d__;
			<WriteDocTypeAsync>d__.<>4__this = this;
			<WriteDocTypeAsync>d__.name = name;
			<WriteDocTypeAsync>d__.pubid = pubid;
			<WriteDocTypeAsync>d__.sysid = sysid;
			<WriteDocTypeAsync>d__.subset = subset;
			<WriteDocTypeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteDocTypeAsync>d__.<>1__state = -1;
			<WriteDocTypeAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteDocTypeAsync>d__31>(ref <WriteDocTypeAsync>d__);
			return <WriteDocTypeAsync>d__.<>t__builder.Task;
		}

		public override Task WriteStartElementAsync(string prefix, string localName, string ns)
		{
			XmlEncodedRawTextWriterIndent.<WriteStartElementAsync>d__32 <WriteStartElementAsync>d__;
			<WriteStartElementAsync>d__.<>4__this = this;
			<WriteStartElementAsync>d__.prefix = prefix;
			<WriteStartElementAsync>d__.localName = localName;
			<WriteStartElementAsync>d__.ns = ns;
			<WriteStartElementAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteStartElementAsync>d__.<>1__state = -1;
			<WriteStartElementAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteStartElementAsync>d__32>(ref <WriteStartElementAsync>d__);
			return <WriteStartElementAsync>d__.<>t__builder.Task;
		}

		internal override Task WriteEndElementAsync(string prefix, string localName, string ns)
		{
			XmlEncodedRawTextWriterIndent.<WriteEndElementAsync>d__33 <WriteEndElementAsync>d__;
			<WriteEndElementAsync>d__.<>4__this = this;
			<WriteEndElementAsync>d__.prefix = prefix;
			<WriteEndElementAsync>d__.localName = localName;
			<WriteEndElementAsync>d__.ns = ns;
			<WriteEndElementAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteEndElementAsync>d__.<>1__state = -1;
			<WriteEndElementAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteEndElementAsync>d__33>(ref <WriteEndElementAsync>d__);
			return <WriteEndElementAsync>d__.<>t__builder.Task;
		}

		internal override Task WriteFullEndElementAsync(string prefix, string localName, string ns)
		{
			XmlEncodedRawTextWriterIndent.<WriteFullEndElementAsync>d__34 <WriteFullEndElementAsync>d__;
			<WriteFullEndElementAsync>d__.<>4__this = this;
			<WriteFullEndElementAsync>d__.prefix = prefix;
			<WriteFullEndElementAsync>d__.localName = localName;
			<WriteFullEndElementAsync>d__.ns = ns;
			<WriteFullEndElementAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteFullEndElementAsync>d__.<>1__state = -1;
			<WriteFullEndElementAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteFullEndElementAsync>d__34>(ref <WriteFullEndElementAsync>d__);
			return <WriteFullEndElementAsync>d__.<>t__builder.Task;
		}

		protected internal override Task WriteStartAttributeAsync(string prefix, string localName, string ns)
		{
			XmlEncodedRawTextWriterIndent.<WriteStartAttributeAsync>d__35 <WriteStartAttributeAsync>d__;
			<WriteStartAttributeAsync>d__.<>4__this = this;
			<WriteStartAttributeAsync>d__.prefix = prefix;
			<WriteStartAttributeAsync>d__.localName = localName;
			<WriteStartAttributeAsync>d__.ns = ns;
			<WriteStartAttributeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteStartAttributeAsync>d__.<>1__state = -1;
			<WriteStartAttributeAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteStartAttributeAsync>d__35>(ref <WriteStartAttributeAsync>d__);
			return <WriteStartAttributeAsync>d__.<>t__builder.Task;
		}

		public override Task WriteCDataAsync(string text)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteCDataAsync(text);
		}

		public override Task WriteCommentAsync(string text)
		{
			XmlEncodedRawTextWriterIndent.<WriteCommentAsync>d__37 <WriteCommentAsync>d__;
			<WriteCommentAsync>d__.<>4__this = this;
			<WriteCommentAsync>d__.text = text;
			<WriteCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteCommentAsync>d__.<>1__state = -1;
			<WriteCommentAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteCommentAsync>d__37>(ref <WriteCommentAsync>d__);
			return <WriteCommentAsync>d__.<>t__builder.Task;
		}

		public override Task WriteProcessingInstructionAsync(string target, string text)
		{
			XmlEncodedRawTextWriterIndent.<WriteProcessingInstructionAsync>d__38 <WriteProcessingInstructionAsync>d__;
			<WriteProcessingInstructionAsync>d__.<>4__this = this;
			<WriteProcessingInstructionAsync>d__.target = target;
			<WriteProcessingInstructionAsync>d__.text = text;
			<WriteProcessingInstructionAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteProcessingInstructionAsync>d__.<>1__state = -1;
			<WriteProcessingInstructionAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteProcessingInstructionAsync>d__38>(ref <WriteProcessingInstructionAsync>d__);
			return <WriteProcessingInstructionAsync>d__.<>t__builder.Task;
		}

		public override Task WriteEntityRefAsync(string name)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteEntityRefAsync(name);
		}

		public override Task WriteCharEntityAsync(char ch)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteCharEntityAsync(ch);
		}

		public override Task WriteSurrogateCharEntityAsync(char lowChar, char highChar)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteSurrogateCharEntityAsync(lowChar, highChar);
		}

		public override Task WriteWhitespaceAsync(string ws)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteWhitespaceAsync(ws);
		}

		public override Task WriteStringAsync(string text)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteStringAsync(text);
		}

		public override Task WriteCharsAsync(char[] buffer, int index, int count)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteCharsAsync(buffer, index, count);
		}

		public override Task WriteRawAsync(char[] buffer, int index, int count)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteRawAsync(buffer, index, count);
		}

		public override Task WriteRawAsync(string data)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteRawAsync(data);
		}

		public override Task WriteBase64Async(byte[] buffer, int index, int count)
		{
			base.CheckAsyncCall();
			this.mixedContent = true;
			return base.WriteBase64Async(buffer, index, count);
		}

		private Task WriteIndentAsync()
		{
			XmlEncodedRawTextWriterIndent.<WriteIndentAsync>d__48 <WriteIndentAsync>d__;
			<WriteIndentAsync>d__.<>4__this = this;
			<WriteIndentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteIndentAsync>d__.<>1__state = -1;
			<WriteIndentAsync>d__.<>t__builder.Start<XmlEncodedRawTextWriterIndent.<WriteIndentAsync>d__48>(ref <WriteIndentAsync>d__);
			return <WriteIndentAsync>d__.<>t__builder.Task;
		}

		protected int indentLevel;

		protected bool newLineOnAttributes;

		protected string indentChars;

		protected bool mixedContent;

		private BitStack mixedContentStack;

		protected ConformanceLevel conformanceLevel;
	}
}
