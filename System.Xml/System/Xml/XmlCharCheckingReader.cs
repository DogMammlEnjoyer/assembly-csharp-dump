using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlCharCheckingReader : XmlWrappingReader
	{
		internal XmlCharCheckingReader(XmlReader reader, bool checkCharacters, bool ignoreWhitespace, bool ignoreComments, bool ignorePis, DtdProcessing dtdProcessing) : base(reader)
		{
			this.state = XmlCharCheckingReader.State.Initial;
			this.checkCharacters = checkCharacters;
			this.ignoreWhitespace = ignoreWhitespace;
			this.ignoreComments = ignoreComments;
			this.ignorePis = ignorePis;
			this.dtdProcessing = dtdProcessing;
			this.lastNodeType = XmlNodeType.None;
			if (checkCharacters)
			{
				this.xmlCharType = XmlCharType.Instance;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings = this.reader.Settings;
				if (xmlReaderSettings == null)
				{
					xmlReaderSettings = new XmlReaderSettings();
				}
				else
				{
					xmlReaderSettings = xmlReaderSettings.Clone();
				}
				if (this.checkCharacters)
				{
					xmlReaderSettings.CheckCharacters = true;
				}
				if (this.ignoreWhitespace)
				{
					xmlReaderSettings.IgnoreWhitespace = true;
				}
				if (this.ignoreComments)
				{
					xmlReaderSettings.IgnoreComments = true;
				}
				if (this.ignorePis)
				{
					xmlReaderSettings.IgnoreProcessingInstructions = true;
				}
				if (this.dtdProcessing != (DtdProcessing)(-1))
				{
					xmlReaderSettings.DtdProcessing = this.dtdProcessing;
				}
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override bool MoveToAttribute(string name)
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToAttribute(name, ns);
		}

		public override void MoveToAttribute(int i)
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			this.reader.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.MoveToElement();
		}

		public override bool Read()
		{
			switch (this.state)
			{
			case XmlCharCheckingReader.State.Initial:
				this.state = XmlCharCheckingReader.State.Interactive;
				if (this.reader.ReadState != ReadState.Initial)
				{
					goto IL_55;
				}
				break;
			case XmlCharCheckingReader.State.InReadBinary:
				this.FinishReadBinary();
				this.state = XmlCharCheckingReader.State.Interactive;
				break;
			case XmlCharCheckingReader.State.Error:
				return false;
			case XmlCharCheckingReader.State.Interactive:
				break;
			default:
				return false;
			}
			if (!this.reader.Read())
			{
				return false;
			}
			IL_55:
			XmlNodeType nodeType = this.reader.NodeType;
			if (!this.checkCharacters)
			{
				switch (nodeType)
				{
				case XmlNodeType.ProcessingInstruction:
					if (this.ignorePis)
					{
						return this.Read();
					}
					break;
				case XmlNodeType.Comment:
					if (this.ignoreComments)
					{
						return this.Read();
					}
					break;
				case XmlNodeType.DocumentType:
					if (this.dtdProcessing == DtdProcessing.Prohibit)
					{
						this.Throw("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.", string.Empty);
					}
					else if (this.dtdProcessing == DtdProcessing.Ignore)
					{
						return this.Read();
					}
					break;
				case XmlNodeType.Whitespace:
					if (this.ignoreWhitespace)
					{
						return this.Read();
					}
					break;
				}
				return true;
			}
			switch (nodeType)
			{
			case XmlNodeType.Element:
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
					if (this.reader.MoveToFirstAttribute())
					{
						do
						{
							this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
							this.CheckCharacters(this.reader.Value);
						}
						while (this.reader.MoveToNextAttribute());
						this.reader.MoveToElement();
					}
				}
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				if (this.checkCharacters)
				{
					this.CheckCharacters(this.reader.Value);
				}
				break;
			case XmlNodeType.EntityReference:
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Name);
				}
				break;
			case XmlNodeType.ProcessingInstruction:
				if (this.ignorePis)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Name);
					this.CheckCharacters(this.reader.Value);
				}
				break;
			case XmlNodeType.Comment:
				if (this.ignoreComments)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.CheckCharacters(this.reader.Value);
				}
				break;
			case XmlNodeType.DocumentType:
				if (this.dtdProcessing == DtdProcessing.Prohibit)
				{
					this.Throw("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.", string.Empty);
				}
				else if (this.dtdProcessing == DtdProcessing.Ignore)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Name);
					this.CheckCharacters(this.reader.Value);
					string attribute = this.reader.GetAttribute("SYSTEM");
					if (attribute != null)
					{
						this.CheckCharacters(attribute);
					}
					attribute = this.reader.GetAttribute("PUBLIC");
					int invCharIndex;
					if (attribute != null && (invCharIndex = this.xmlCharType.IsPublicId(attribute)) >= 0)
					{
						this.Throw("'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(attribute, invCharIndex));
					}
				}
				break;
			case XmlNodeType.Whitespace:
				if (this.ignoreWhitespace)
				{
					return this.Read();
				}
				if (this.checkCharacters)
				{
					this.CheckWhitespace(this.reader.Value);
				}
				break;
			case XmlNodeType.SignificantWhitespace:
				if (this.checkCharacters)
				{
					this.CheckWhitespace(this.reader.Value);
				}
				break;
			case XmlNodeType.EndElement:
				if (this.checkCharacters)
				{
					this.ValidateQName(this.reader.Prefix, this.reader.LocalName);
				}
				break;
			}
			this.lastNodeType = nodeType;
			return true;
		}

		public override ReadState ReadState
		{
			get
			{
				switch (this.state)
				{
				case XmlCharCheckingReader.State.Initial:
					if (this.reader.ReadState != ReadState.Closed)
					{
						return ReadState.Initial;
					}
					return ReadState.Closed;
				case XmlCharCheckingReader.State.Error:
					return ReadState.Error;
				}
				return this.reader.ReadState;
			}
		}

		public override bool ReadAttributeValue()
		{
			if (this.state == XmlCharCheckingReader.State.InReadBinary)
			{
				this.FinishReadBinary();
			}
			return this.reader.ReadAttributeValue();
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadContentAsBase64(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadContentAsBase64(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int result = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return result;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadContentAsBinHex(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadContentAsBinHex(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int result = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return result;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadElementContentAsBase64(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadElementContentAsBase64(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int result = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return result;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.ReadState != ReadState.Interactive)
			{
				return 0;
			}
			if (this.state != XmlCharCheckingReader.State.InReadBinary)
			{
				if (base.CanReadBinaryContent && !this.checkCharacters)
				{
					this.readBinaryHelper = null;
					this.state = XmlCharCheckingReader.State.InReadBinary;
					return base.ReadElementContentAsBinHex(buffer, index, count);
				}
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			else if (this.readBinaryHelper == null)
			{
				return base.ReadElementContentAsBinHex(buffer, index, count);
			}
			this.state = XmlCharCheckingReader.State.Interactive;
			int result = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.state = XmlCharCheckingReader.State.InReadBinary;
			return result;
		}

		private void Throw(string res, string arg)
		{
			this.state = XmlCharCheckingReader.State.Error;
			throw new XmlException(res, arg, null);
		}

		private void Throw(string res, string[] args)
		{
			this.state = XmlCharCheckingReader.State.Error;
			throw new XmlException(res, args, null);
		}

		private void CheckWhitespace(string value)
		{
			int invCharIndex;
			if ((invCharIndex = this.xmlCharType.IsOnlyWhitespaceWithPos(value)) != -1)
			{
				this.Throw("The Whitespace or SignificantWhitespace node can contain only XML white space characters. '{0}' is not an XML white space character.", XmlException.BuildCharExceptionArgs(value, invCharIndex));
			}
		}

		private void ValidateQName(string name)
		{
			string text;
			string text2;
			ValidateNames.ParseQNameThrow(name, out text, out text2);
		}

		private void ValidateQName(string prefix, string localName)
		{
			try
			{
				if (prefix.Length > 0)
				{
					ValidateNames.ParseNCNameThrow(prefix);
				}
				ValidateNames.ParseNCNameThrow(localName);
			}
			catch
			{
				this.state = XmlCharCheckingReader.State.Error;
				throw;
			}
		}

		private void CheckCharacters(string value)
		{
			XmlConvert.VerifyCharData(value, ExceptionType.ArgumentException, ExceptionType.XmlException);
		}

		private void FinishReadBinary()
		{
			this.state = XmlCharCheckingReader.State.Interactive;
			if (this.readBinaryHelper != null)
			{
				this.readBinaryHelper.Finish();
			}
		}

		public override Task<bool> ReadAsync()
		{
			XmlCharCheckingReader.<ReadAsync>d__36 <ReadAsync>d__;
			<ReadAsync>d__.<>4__this = this;
			<ReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ReadAsync>d__.<>1__state = -1;
			<ReadAsync>d__.<>t__builder.Start<XmlCharCheckingReader.<ReadAsync>d__36>(ref <ReadAsync>d__);
			return <ReadAsync>d__.<>t__builder.Task;
		}

		public override Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			XmlCharCheckingReader.<ReadContentAsBase64Async>d__37 <ReadContentAsBase64Async>d__;
			<ReadContentAsBase64Async>d__.<>4__this = this;
			<ReadContentAsBase64Async>d__.buffer = buffer;
			<ReadContentAsBase64Async>d__.index = index;
			<ReadContentAsBase64Async>d__.count = count;
			<ReadContentAsBase64Async>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBase64Async>d__.<>1__state = -1;
			<ReadContentAsBase64Async>d__.<>t__builder.Start<XmlCharCheckingReader.<ReadContentAsBase64Async>d__37>(ref <ReadContentAsBase64Async>d__);
			return <ReadContentAsBase64Async>d__.<>t__builder.Task;
		}

		public override Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			XmlCharCheckingReader.<ReadContentAsBinHexAsync>d__38 <ReadContentAsBinHexAsync>d__;
			<ReadContentAsBinHexAsync>d__.<>4__this = this;
			<ReadContentAsBinHexAsync>d__.buffer = buffer;
			<ReadContentAsBinHexAsync>d__.index = index;
			<ReadContentAsBinHexAsync>d__.count = count;
			<ReadContentAsBinHexAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBinHexAsync>d__.<>1__state = -1;
			<ReadContentAsBinHexAsync>d__.<>t__builder.Start<XmlCharCheckingReader.<ReadContentAsBinHexAsync>d__38>(ref <ReadContentAsBinHexAsync>d__);
			return <ReadContentAsBinHexAsync>d__.<>t__builder.Task;
		}

		public override Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			XmlCharCheckingReader.<ReadElementContentAsBase64Async>d__39 <ReadElementContentAsBase64Async>d__;
			<ReadElementContentAsBase64Async>d__.<>4__this = this;
			<ReadElementContentAsBase64Async>d__.buffer = buffer;
			<ReadElementContentAsBase64Async>d__.index = index;
			<ReadElementContentAsBase64Async>d__.count = count;
			<ReadElementContentAsBase64Async>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBase64Async>d__.<>1__state = -1;
			<ReadElementContentAsBase64Async>d__.<>t__builder.Start<XmlCharCheckingReader.<ReadElementContentAsBase64Async>d__39>(ref <ReadElementContentAsBase64Async>d__);
			return <ReadElementContentAsBase64Async>d__.<>t__builder.Task;
		}

		public override Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			XmlCharCheckingReader.<ReadElementContentAsBinHexAsync>d__40 <ReadElementContentAsBinHexAsync>d__;
			<ReadElementContentAsBinHexAsync>d__.<>4__this = this;
			<ReadElementContentAsBinHexAsync>d__.buffer = buffer;
			<ReadElementContentAsBinHexAsync>d__.index = index;
			<ReadElementContentAsBinHexAsync>d__.count = count;
			<ReadElementContentAsBinHexAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBinHexAsync>d__.<>1__state = -1;
			<ReadElementContentAsBinHexAsync>d__.<>t__builder.Start<XmlCharCheckingReader.<ReadElementContentAsBinHexAsync>d__40>(ref <ReadElementContentAsBinHexAsync>d__);
			return <ReadElementContentAsBinHexAsync>d__.<>t__builder.Task;
		}

		private Task FinishReadBinaryAsync()
		{
			XmlCharCheckingReader.<FinishReadBinaryAsync>d__41 <FinishReadBinaryAsync>d__;
			<FinishReadBinaryAsync>d__.<>4__this = this;
			<FinishReadBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishReadBinaryAsync>d__.<>1__state = -1;
			<FinishReadBinaryAsync>d__.<>t__builder.Start<XmlCharCheckingReader.<FinishReadBinaryAsync>d__41>(ref <FinishReadBinaryAsync>d__);
			return <FinishReadBinaryAsync>d__.<>t__builder.Task;
		}

		private XmlCharCheckingReader.State state;

		private bool checkCharacters;

		private bool ignoreWhitespace;

		private bool ignoreComments;

		private bool ignorePis;

		private DtdProcessing dtdProcessing;

		private XmlNodeType lastNodeType;

		private XmlCharType xmlCharType;

		private ReadContentAsBinaryHelper readBinaryHelper;

		private enum State
		{
			Initial,
			InReadBinary,
			Error,
			Interactive
		}
	}
}
