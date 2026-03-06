using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.XmlConfiguration;

namespace System.Xml
{
	internal class XmlTextReaderImpl : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
	{
		internal XmlTextReaderImpl()
		{
			this.curNode = new XmlTextReaderImpl.NodeData();
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.NoData;
		}

		internal XmlTextReaderImpl(XmlNameTable nt)
		{
			this.v1Compat = true;
			this.outerReader = this;
			this.nameTable = nt;
			nt.Add(string.Empty);
			if (!XmlReaderSettings.EnableLegacyXmlSettings())
			{
				this.xmlResolver = null;
			}
			else
			{
				this.xmlResolver = new XmlUrlResolver();
			}
			this.Xml = nt.Add("xml");
			this.XmlNs = nt.Add("xmlns");
			this.nodes = new XmlTextReaderImpl.NodeData[8];
			this.nodes[0] = new XmlTextReaderImpl.NodeData();
			this.curNode = this.nodes[0];
			this.stringBuilder = new StringBuilder();
			this.xmlContext = new XmlTextReaderImpl.XmlContext();
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.SwitchToInteractiveXmlDecl;
			this.nextParsingFunction = XmlTextReaderImpl.ParsingFunction.DocumentContent;
			this.entityHandling = EntityHandling.ExpandCharEntities;
			this.whitespaceHandling = WhitespaceHandling.All;
			this.closeInput = true;
			this.maxCharactersInDocument = 0L;
			this.maxCharactersFromEntities = 10000000L;
			this.charactersInDocument = 0L;
			this.charactersFromEntities = 0L;
			this.ps.lineNo = 1;
			this.ps.lineStartPos = -1;
		}

		private XmlTextReaderImpl(XmlResolver resolver, XmlReaderSettings settings, XmlParserContext context)
		{
			this.useAsync = settings.Async;
			this.v1Compat = false;
			this.outerReader = this;
			this.xmlContext = new XmlTextReaderImpl.XmlContext();
			XmlNameTable xmlNameTable = settings.NameTable;
			if (context == null)
			{
				if (xmlNameTable == null)
				{
					xmlNameTable = new NameTable();
				}
				else
				{
					this.nameTableFromSettings = true;
				}
				this.nameTable = xmlNameTable;
				this.namespaceManager = new XmlNamespaceManager(xmlNameTable);
			}
			else
			{
				this.SetupFromParserContext(context, settings);
				xmlNameTable = this.nameTable;
			}
			xmlNameTable.Add(string.Empty);
			this.Xml = xmlNameTable.Add("xml");
			this.XmlNs = xmlNameTable.Add("xmlns");
			this.xmlResolver = resolver;
			this.nodes = new XmlTextReaderImpl.NodeData[8];
			this.nodes[0] = new XmlTextReaderImpl.NodeData();
			this.curNode = this.nodes[0];
			this.stringBuilder = new StringBuilder();
			this.entityHandling = EntityHandling.ExpandEntities;
			this.xmlResolverIsSet = settings.IsXmlResolverSet;
			this.whitespaceHandling = (settings.IgnoreWhitespace ? WhitespaceHandling.Significant : WhitespaceHandling.All);
			this.normalize = true;
			this.ignorePIs = settings.IgnoreProcessingInstructions;
			this.ignoreComments = settings.IgnoreComments;
			this.checkCharacters = settings.CheckCharacters;
			this.lineNumberOffset = settings.LineNumberOffset;
			this.linePositionOffset = settings.LinePositionOffset;
			this.ps.lineNo = this.lineNumberOffset + 1;
			this.ps.lineStartPos = -this.linePositionOffset - 1;
			this.curNode.SetLineInfo(this.ps.LineNo - 1, this.ps.LinePos - 1);
			this.dtdProcessing = settings.DtdProcessing;
			this.maxCharactersInDocument = settings.MaxCharactersInDocument;
			this.maxCharactersFromEntities = settings.MaxCharactersFromEntities;
			this.charactersInDocument = 0L;
			this.charactersFromEntities = 0L;
			this.fragmentParserContext = context;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.SwitchToInteractiveXmlDecl;
			this.nextParsingFunction = XmlTextReaderImpl.ParsingFunction.DocumentContent;
			switch (settings.ConformanceLevel)
			{
			case ConformanceLevel.Auto:
				this.fragmentType = XmlNodeType.None;
				this.fragment = true;
				return;
			case ConformanceLevel.Fragment:
				this.fragmentType = XmlNodeType.Element;
				this.fragment = true;
				return;
			}
			this.fragmentType = XmlNodeType.Document;
		}

		internal XmlTextReaderImpl(Stream input) : this(string.Empty, input, new NameTable())
		{
		}

		internal XmlTextReaderImpl(Stream input, XmlNameTable nt) : this(string.Empty, input, nt)
		{
		}

		internal XmlTextReaderImpl(string url, Stream input) : this(url, input, new NameTable())
		{
		}

		internal XmlTextReaderImpl(string url, Stream input, XmlNameTable nt) : this(nt)
		{
			this.namespaceManager = new XmlNamespaceManager(nt);
			if (url == null || url.Length == 0)
			{
				this.InitStreamInput(input, null);
			}
			else
			{
				this.InitStreamInput(url, input, null);
			}
			this.reportedBaseUri = this.ps.baseUriStr;
			this.reportedEncoding = this.ps.encoding;
		}

		internal XmlTextReaderImpl(TextReader input) : this(string.Empty, input, new NameTable())
		{
		}

		internal XmlTextReaderImpl(TextReader input, XmlNameTable nt) : this(string.Empty, input, nt)
		{
		}

		internal XmlTextReaderImpl(string url, TextReader input) : this(url, input, new NameTable())
		{
		}

		internal XmlTextReaderImpl(string url, TextReader input, XmlNameTable nt) : this(nt)
		{
			this.namespaceManager = new XmlNamespaceManager(nt);
			this.reportedBaseUri = ((url != null) ? url : string.Empty);
			this.InitTextReaderInput(this.reportedBaseUri, input);
			this.reportedEncoding = this.ps.encoding;
		}

		internal XmlTextReaderImpl(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context) : this((context != null && context.NameTable != null) ? context.NameTable : new NameTable())
		{
			Encoding encoding = (context != null) ? context.Encoding : null;
			if (context == null || context.BaseURI == null || context.BaseURI.Length == 0)
			{
				this.InitStreamInput(xmlFragment, encoding);
			}
			else
			{
				this.InitStreamInput(this.GetTempResolver().ResolveUri(null, context.BaseURI), xmlFragment, encoding);
			}
			this.InitFragmentReader(fragType, context, false);
			this.reportedBaseUri = this.ps.baseUriStr;
			this.reportedEncoding = this.ps.encoding;
		}

		internal XmlTextReaderImpl(string xmlFragment, XmlNodeType fragType, XmlParserContext context) : this((context == null || context.NameTable == null) ? new NameTable() : context.NameTable)
		{
			if (xmlFragment == null)
			{
				xmlFragment = string.Empty;
			}
			if (context == null)
			{
				this.InitStringInput(string.Empty, Encoding.Unicode, xmlFragment);
			}
			else
			{
				this.reportedBaseUri = context.BaseURI;
				this.InitStringInput(context.BaseURI, Encoding.Unicode, xmlFragment);
			}
			this.InitFragmentReader(fragType, context, false);
			this.reportedEncoding = this.ps.encoding;
		}

		internal XmlTextReaderImpl(string xmlFragment, XmlParserContext context) : this((context == null || context.NameTable == null) ? new NameTable() : context.NameTable)
		{
			this.InitStringInput((context == null) ? string.Empty : context.BaseURI, Encoding.Unicode, "<?xml " + xmlFragment + "?>");
			this.InitFragmentReader(XmlNodeType.XmlDeclaration, context, true);
		}

		public XmlTextReaderImpl(string url) : this(url, new NameTable())
		{
		}

		public XmlTextReaderImpl(string url, XmlNameTable nt) : this(nt)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The URL cannot be empty."), "url");
			}
			this.namespaceManager = new XmlNamespaceManager(nt);
			this.url = url;
			this.ps.baseUri = this.GetTempResolver().ResolveUri(null, url);
			this.ps.baseUriStr = this.ps.baseUri.ToString();
			this.reportedBaseUri = this.ps.baseUriStr;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.OpenUrl;
		}

		internal XmlTextReaderImpl(string uriStr, XmlReaderSettings settings, XmlParserContext context, XmlResolver uriResolver) : this(settings.GetXmlResolver(), settings, context)
		{
			Uri uri = uriResolver.ResolveUri(null, uriStr);
			string text = uri.ToString();
			if (context != null && context.BaseURI != null && context.BaseURI.Length > 0 && !this.UriEqual(uri, text, context.BaseURI, settings.GetXmlResolver()))
			{
				if (text.Length > 0)
				{
					this.Throw("BaseUri must be specified either as an argument of XmlReader.Create or on the XmlParserContext. If it is specified on both, it must be the same base URI.");
				}
				text = context.BaseURI;
			}
			this.reportedBaseUri = text;
			this.closeInput = true;
			this.laterInitParam = new XmlTextReaderImpl.LaterInitParam();
			this.laterInitParam.inputUriStr = uriStr;
			this.laterInitParam.inputbaseUri = uri;
			this.laterInitParam.inputContext = context;
			this.laterInitParam.inputUriResolver = uriResolver;
			this.laterInitParam.initType = XmlTextReaderImpl.InitInputType.UriString;
			if (!settings.Async)
			{
				this.FinishInitUriString();
				return;
			}
			this.laterInitParam.useAsync = true;
		}

		private void FinishInitUriString()
		{
			Stream stream = null;
			if (this.laterInitParam.useAsync)
			{
				Task<object> entityAsync = this.laterInitParam.inputUriResolver.GetEntityAsync(this.laterInitParam.inputbaseUri, string.Empty, typeof(Stream));
				entityAsync.Wait();
				stream = (Stream)entityAsync.Result;
			}
			else
			{
				stream = (Stream)this.laterInitParam.inputUriResolver.GetEntity(this.laterInitParam.inputbaseUri, string.Empty, typeof(Stream));
			}
			if (stream == null)
			{
				throw new XmlException("Cannot resolve '{0}'.", this.laterInitParam.inputUriStr);
			}
			Encoding encoding = null;
			if (this.laterInitParam.inputContext != null)
			{
				encoding = this.laterInitParam.inputContext.Encoding;
			}
			try
			{
				this.InitStreamInput(this.laterInitParam.inputbaseUri, this.reportedBaseUri, stream, null, 0, encoding);
				this.reportedEncoding = this.ps.encoding;
				if (this.laterInitParam.inputContext != null && this.laterInitParam.inputContext.HasDtdInfo)
				{
					this.ProcessDtdFromParserContext(this.laterInitParam.inputContext);
				}
			}
			catch
			{
				stream.Close();
				throw;
			}
			this.laterInitParam = null;
		}

		internal XmlTextReaderImpl(Stream stream, byte[] bytes, int byteCount, XmlReaderSettings settings, Uri baseUri, string baseUriStr, XmlParserContext context, bool closeInput) : this(settings.GetXmlResolver(), settings, context)
		{
			if (context != null && context.BaseURI != null && context.BaseURI.Length > 0 && !this.UriEqual(baseUri, baseUriStr, context.BaseURI, settings.GetXmlResolver()))
			{
				if (baseUriStr.Length > 0)
				{
					this.Throw("BaseUri must be specified either as an argument of XmlReader.Create or on the XmlParserContext. If it is specified on both, it must be the same base URI.");
				}
				baseUriStr = context.BaseURI;
			}
			this.reportedBaseUri = baseUriStr;
			this.closeInput = closeInput;
			this.laterInitParam = new XmlTextReaderImpl.LaterInitParam();
			this.laterInitParam.inputStream = stream;
			this.laterInitParam.inputBytes = bytes;
			this.laterInitParam.inputByteCount = byteCount;
			this.laterInitParam.inputbaseUri = baseUri;
			this.laterInitParam.inputContext = context;
			this.laterInitParam.initType = XmlTextReaderImpl.InitInputType.Stream;
			if (!settings.Async)
			{
				this.FinishInitStream();
				return;
			}
			this.laterInitParam.useAsync = true;
		}

		private void FinishInitStream()
		{
			Encoding encoding = null;
			if (this.laterInitParam.inputContext != null)
			{
				encoding = this.laterInitParam.inputContext.Encoding;
			}
			this.InitStreamInput(this.laterInitParam.inputbaseUri, this.reportedBaseUri, this.laterInitParam.inputStream, this.laterInitParam.inputBytes, this.laterInitParam.inputByteCount, encoding);
			this.reportedEncoding = this.ps.encoding;
			if (this.laterInitParam.inputContext != null && this.laterInitParam.inputContext.HasDtdInfo)
			{
				this.ProcessDtdFromParserContext(this.laterInitParam.inputContext);
			}
			this.laterInitParam = null;
		}

		internal XmlTextReaderImpl(TextReader input, XmlReaderSettings settings, string baseUriStr, XmlParserContext context) : this(settings.GetXmlResolver(), settings, context)
		{
			if (context != null && context.BaseURI != null)
			{
				baseUriStr = context.BaseURI;
			}
			this.reportedBaseUri = baseUriStr;
			this.closeInput = settings.CloseInput;
			this.laterInitParam = new XmlTextReaderImpl.LaterInitParam();
			this.laterInitParam.inputTextReader = input;
			this.laterInitParam.inputContext = context;
			this.laterInitParam.initType = XmlTextReaderImpl.InitInputType.TextReader;
			if (!settings.Async)
			{
				this.FinishInitTextReader();
				return;
			}
			this.laterInitParam.useAsync = true;
		}

		private void FinishInitTextReader()
		{
			this.InitTextReaderInput(this.reportedBaseUri, this.laterInitParam.inputTextReader);
			this.reportedEncoding = this.ps.encoding;
			if (this.laterInitParam.inputContext != null && this.laterInitParam.inputContext.HasDtdInfo)
			{
				this.ProcessDtdFromParserContext(this.laterInitParam.inputContext);
			}
			this.laterInitParam = null;
		}

		internal XmlTextReaderImpl(string xmlFragment, XmlParserContext context, XmlReaderSettings settings) : this(null, settings, context)
		{
			this.InitStringInput(string.Empty, Encoding.Unicode, xmlFragment);
			this.reportedBaseUri = this.ps.baseUriStr;
			this.reportedEncoding = this.ps.encoding;
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				if (this.nameTableFromSettings)
				{
					xmlReaderSettings.NameTable = this.nameTable;
				}
				XmlNodeType xmlNodeType = this.fragmentType;
				if (xmlNodeType != XmlNodeType.None)
				{
					if (xmlNodeType == XmlNodeType.Element)
					{
						xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
						goto IL_46;
					}
					if (xmlNodeType == XmlNodeType.Document)
					{
						xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
						goto IL_46;
					}
				}
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Auto;
				IL_46:
				xmlReaderSettings.CheckCharacters = this.checkCharacters;
				xmlReaderSettings.LineNumberOffset = this.lineNumberOffset;
				xmlReaderSettings.LinePositionOffset = this.linePositionOffset;
				xmlReaderSettings.IgnoreWhitespace = (this.whitespaceHandling == WhitespaceHandling.Significant);
				xmlReaderSettings.IgnoreProcessingInstructions = this.ignorePIs;
				xmlReaderSettings.IgnoreComments = this.ignoreComments;
				xmlReaderSettings.DtdProcessing = this.dtdProcessing;
				xmlReaderSettings.MaxCharactersInDocument = this.maxCharactersInDocument;
				xmlReaderSettings.MaxCharactersFromEntities = this.maxCharactersFromEntities;
				if (!XmlReaderSettings.EnableLegacyXmlSettings())
				{
					xmlReaderSettings.XmlResolver = this.xmlResolver;
				}
				xmlReaderSettings.ReadOnly = true;
				return xmlReaderSettings;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return this.curNode.type;
			}
		}

		public override string Name
		{
			get
			{
				return this.curNode.GetNameWPrefix(this.nameTable);
			}
		}

		public override string LocalName
		{
			get
			{
				return this.curNode.localName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.curNode.ns;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.curNode.prefix;
			}
		}

		public override string Value
		{
			get
			{
				if (this.parsingFunction >= XmlTextReaderImpl.ParsingFunction.PartialTextValue)
				{
					if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.PartialTextValue)
					{
						this.FinishPartialValue();
						this.parsingFunction = this.nextParsingFunction;
					}
					else
					{
						this.FinishOtherValueIterator();
					}
				}
				return this.curNode.StringValue;
			}
		}

		public override int Depth
		{
			get
			{
				return this.curNode.depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.reportedBaseUri;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.curNode.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.curNode.IsDefaultAttribute;
			}
		}

		public override char QuoteChar
		{
			get
			{
				if (this.curNode.type != XmlNodeType.Attribute)
				{
					return '"';
				}
				return this.curNode.quoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.xmlContext.xmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.xmlContext.xmlLang;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.readState;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.parsingFunction == XmlTextReaderImpl.ParsingFunction.Eof;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.attrCount;
			}
		}

		public override string GetAttribute(string name)
		{
			int num;
			if (name.IndexOf(':') == -1)
			{
				num = this.GetIndexOfAttributeWithoutPrefix(name);
			}
			else
			{
				num = this.GetIndexOfAttributeWithPrefix(name);
			}
			if (num < 0)
			{
				return null;
			}
			return this.nodes[num].StringValue;
		}

		public override string GetAttribute(string localName, string namespaceURI)
		{
			namespaceURI = ((namespaceURI == null) ? string.Empty : this.nameTable.Get(namespaceURI));
			localName = this.nameTable.Get(localName);
			for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
			{
				if (Ref.Equal(this.nodes[i].localName, localName) && Ref.Equal(this.nodes[i].ns, namespaceURI))
				{
					return this.nodes[i].StringValue;
				}
			}
			return null;
		}

		public override string GetAttribute(int i)
		{
			if (i < 0 || i >= this.attrCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			return this.nodes[this.index + i + 1].StringValue;
		}

		public override bool MoveToAttribute(string name)
		{
			int num;
			if (name.IndexOf(':') == -1)
			{
				num = this.GetIndexOfAttributeWithoutPrefix(name);
			}
			else
			{
				num = this.GetIndexOfAttributeWithPrefix(name);
			}
			if (num >= 0)
			{
				if (this.InAttributeValueIterator)
				{
					this.FinishAttributeValueIterator();
				}
				this.curAttrIndex = num - this.index - 1;
				this.curNode = this.nodes[num];
				return true;
			}
			return false;
		}

		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			namespaceURI = ((namespaceURI == null) ? string.Empty : this.nameTable.Get(namespaceURI));
			localName = this.nameTable.Get(localName);
			for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
			{
				if (Ref.Equal(this.nodes[i].localName, localName) && Ref.Equal(this.nodes[i].ns, namespaceURI))
				{
					this.curAttrIndex = i - this.index - 1;
					this.curNode = this.nodes[i];
					if (this.InAttributeValueIterator)
					{
						this.FinishAttributeValueIterator();
					}
					return true;
				}
			}
			return false;
		}

		public override void MoveToAttribute(int i)
		{
			if (i < 0 || i >= this.attrCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			if (this.InAttributeValueIterator)
			{
				this.FinishAttributeValueIterator();
			}
			this.curAttrIndex = i;
			this.curNode = this.nodes[this.index + 1 + this.curAttrIndex];
		}

		public override bool MoveToFirstAttribute()
		{
			if (this.attrCount == 0)
			{
				return false;
			}
			if (this.InAttributeValueIterator)
			{
				this.FinishAttributeValueIterator();
			}
			this.curAttrIndex = 0;
			this.curNode = this.nodes[this.index + 1];
			return true;
		}

		public override bool MoveToNextAttribute()
		{
			if (this.curAttrIndex + 1 < this.attrCount)
			{
				if (this.InAttributeValueIterator)
				{
					this.FinishAttributeValueIterator();
				}
				XmlTextReaderImpl.NodeData[] array = this.nodes;
				int num = this.index + 1;
				int num2 = this.curAttrIndex + 1;
				this.curAttrIndex = num2;
				this.curNode = array[num + num2];
				return true;
			}
			return false;
		}

		public override bool MoveToElement()
		{
			if (this.InAttributeValueIterator)
			{
				this.FinishAttributeValueIterator();
			}
			else if (this.curNode.type != XmlNodeType.Attribute)
			{
				return false;
			}
			this.curAttrIndex = -1;
			this.curNode = this.nodes[this.index];
			return true;
		}

		private void FinishInit()
		{
			switch (this.laterInitParam.initType)
			{
			case XmlTextReaderImpl.InitInputType.UriString:
				this.FinishInitUriString();
				return;
			case XmlTextReaderImpl.InitInputType.Stream:
				this.FinishInitStream();
				return;
			case XmlTextReaderImpl.InitInputType.TextReader:
				this.FinishInitTextReader();
				return;
			default:
				return;
			}
		}

		public override bool Read()
		{
			if (this.laterInitParam != null)
			{
				this.FinishInit();
			}
			for (;;)
			{
				switch (this.parsingFunction)
				{
				case XmlTextReaderImpl.ParsingFunction.ElementContent:
					goto IL_85;
				case XmlTextReaderImpl.ParsingFunction.NoData:
					goto IL_2E7;
				case XmlTextReaderImpl.ParsingFunction.OpenUrl:
					this.OpenUrl();
					break;
				case XmlTextReaderImpl.ParsingFunction.SwitchToInteractive:
					this.readState = ReadState.Interactive;
					this.parsingFunction = this.nextParsingFunction;
					continue;
				case XmlTextReaderImpl.ParsingFunction.SwitchToInteractiveXmlDecl:
					break;
				case XmlTextReaderImpl.ParsingFunction.DocumentContent:
					goto IL_8C;
				case XmlTextReaderImpl.ParsingFunction.MoveToElementContent:
					this.ResetAttributes();
					this.index++;
					this.curNode = this.AddNode(this.index, this.index);
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.ElementContent;
					continue;
				case XmlTextReaderImpl.ParsingFunction.PopElementContext:
					this.PopElementContext();
					this.parsingFunction = this.nextParsingFunction;
					continue;
				case XmlTextReaderImpl.ParsingFunction.PopEmptyElementContext:
					this.curNode = this.nodes[this.index];
					this.curNode.IsEmptyElement = false;
					this.ResetAttributes();
					this.PopElementContext();
					this.parsingFunction = this.nextParsingFunction;
					continue;
				case XmlTextReaderImpl.ParsingFunction.ResetAttributesRootLevel:
					this.ResetAttributes();
					this.curNode = this.nodes[this.index];
					this.parsingFunction = ((this.index == 0) ? XmlTextReaderImpl.ParsingFunction.DocumentContent : XmlTextReaderImpl.ParsingFunction.ElementContent);
					continue;
				case XmlTextReaderImpl.ParsingFunction.Error:
				case XmlTextReaderImpl.ParsingFunction.Eof:
				case XmlTextReaderImpl.ParsingFunction.ReaderClosed:
					return false;
				case XmlTextReaderImpl.ParsingFunction.EntityReference:
					goto IL_1B3;
				case XmlTextReaderImpl.ParsingFunction.InIncrementalRead:
					goto IL_2BE;
				case XmlTextReaderImpl.ParsingFunction.FragmentAttribute:
					goto IL_2C6;
				case XmlTextReaderImpl.ParsingFunction.ReportEndEntity:
					goto IL_1C7;
				case XmlTextReaderImpl.ParsingFunction.AfterResolveEntityInContent:
					this.curNode = this.AddNode(this.index, this.index);
					this.reportedEncoding = this.ps.encoding;
					this.reportedBaseUri = this.ps.baseUriStr;
					this.parsingFunction = this.nextParsingFunction;
					continue;
				case XmlTextReaderImpl.ParsingFunction.AfterResolveEmptyEntityInContent:
					goto IL_226;
				case XmlTextReaderImpl.ParsingFunction.XmlDeclarationFragment:
					goto IL_2CD;
				case XmlTextReaderImpl.ParsingFunction.GoToEof:
					goto IL_2DD;
				case XmlTextReaderImpl.ParsingFunction.PartialTextValue:
					this.SkipPartialTextValue();
					continue;
				case XmlTextReaderImpl.ParsingFunction.InReadAttributeValue:
					this.FinishAttributeValueIterator();
					this.curNode = this.nodes[this.index];
					continue;
				case XmlTextReaderImpl.ParsingFunction.InReadValueChunk:
					this.FinishReadValueChunk();
					continue;
				case XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary:
					this.FinishReadContentAsBinary();
					continue;
				case XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary:
					this.FinishReadElementContentAsBinary();
					continue;
				default:
					continue;
				}
				this.readState = ReadState.Interactive;
				this.parsingFunction = this.nextParsingFunction;
				if (this.ParseXmlDeclaration(false))
				{
					goto Block_3;
				}
				this.reportedEncoding = this.ps.encoding;
			}
			IL_85:
			return this.ParseElementContent();
			IL_8C:
			return this.ParseDocumentContent();
			Block_3:
			this.reportedEncoding = this.ps.encoding;
			return true;
			IL_1B3:
			this.parsingFunction = this.nextParsingFunction;
			this.ParseEntityReference();
			return true;
			IL_1C7:
			this.SetupEndEntityNodeInContent();
			this.parsingFunction = this.nextParsingFunction;
			return true;
			IL_226:
			this.curNode = this.AddNode(this.index, this.index);
			this.curNode.SetValueNode(XmlNodeType.Text, string.Empty);
			this.curNode.SetLineInfo(this.ps.lineNo, this.ps.LinePos);
			this.reportedEncoding = this.ps.encoding;
			this.reportedBaseUri = this.ps.baseUriStr;
			this.parsingFunction = this.nextParsingFunction;
			return true;
			IL_2BE:
			this.FinishIncrementalRead();
			return true;
			IL_2C6:
			return this.ParseFragmentAttribute();
			IL_2CD:
			this.ParseXmlDeclarationFragment();
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.GoToEof;
			return true;
			IL_2DD:
			this.OnEof();
			return false;
			IL_2E7:
			this.ThrowWithoutLineInfo("Root element is missing.");
			return false;
		}

		public override void Close()
		{
			this.Close(this.closeInput);
		}

		public override void Skip()
		{
			if (this.readState != ReadState.Interactive)
			{
				return;
			}
			if (this.InAttributeValueIterator)
			{
				this.FinishAttributeValueIterator();
				this.curNode = this.nodes[this.index];
			}
			else
			{
				XmlTextReaderImpl.ParsingFunction parsingFunction = this.parsingFunction;
				if (parsingFunction != XmlTextReaderImpl.ParsingFunction.InIncrementalRead)
				{
					switch (parsingFunction)
					{
					case XmlTextReaderImpl.ParsingFunction.PartialTextValue:
						this.SkipPartialTextValue();
						break;
					case XmlTextReaderImpl.ParsingFunction.InReadValueChunk:
						this.FinishReadValueChunk();
						break;
					case XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary:
						this.FinishReadContentAsBinary();
						break;
					case XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary:
						this.FinishReadElementContentAsBinary();
						break;
					}
				}
				else
				{
					this.FinishIncrementalRead();
				}
			}
			XmlNodeType type = this.curNode.type;
			if (type != XmlNodeType.Element)
			{
				if (type != XmlNodeType.Attribute)
				{
					goto IL_DC;
				}
				this.outerReader.MoveToElement();
			}
			if (!this.curNode.IsEmptyElement)
			{
				int num = this.index;
				this.parsingMode = XmlTextReaderImpl.ParsingMode.SkipContent;
				while (this.outerReader.Read() && this.index > num)
				{
				}
				this.parsingMode = XmlTextReaderImpl.ParsingMode.Full;
			}
			IL_DC:
			this.outerReader.Read();
		}

		public override string LookupNamespace(string prefix)
		{
			if (!this.supportNamespaces)
			{
				return null;
			}
			return this.namespaceManager.LookupNamespace(prefix);
		}

		public override bool ReadAttributeValue()
		{
			if (this.parsingFunction != XmlTextReaderImpl.ParsingFunction.InReadAttributeValue)
			{
				if (this.curNode.type != XmlNodeType.Attribute)
				{
					return false;
				}
				if (this.readState != ReadState.Interactive || this.curAttrIndex < 0)
				{
					return false;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadValueChunk)
				{
					this.FinishReadValueChunk();
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
				{
					this.FinishReadContentAsBinary();
				}
				if (this.curNode.nextAttrValueChunk == null || this.entityHandling == EntityHandling.ExpandEntities)
				{
					XmlTextReaderImpl.NodeData nodeData = this.AddNode(this.index + this.attrCount + 1, this.curNode.depth + 1);
					nodeData.SetValueNode(XmlNodeType.Text, this.curNode.StringValue);
					nodeData.lineInfo = this.curNode.lineInfo2;
					nodeData.depth = this.curNode.depth + 1;
					this.curNode = nodeData;
					nodeData.nextAttrValueChunk = null;
				}
				else
				{
					this.curNode = this.curNode.nextAttrValueChunk;
					this.AddNode(this.index + this.attrCount + 1, this.index + 2);
					this.nodes[this.index + this.attrCount + 1] = this.curNode;
					this.fullAttrCleanup = true;
				}
				this.nextParsingFunction = this.parsingFunction;
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.InReadAttributeValue;
				this.attributeValueBaseEntityId = this.ps.entityId;
				return true;
			}
			else
			{
				if (this.ps.entityId != this.attributeValueBaseEntityId)
				{
					return this.ParseAttributeValueChunk();
				}
				if (this.curNode.nextAttrValueChunk != null)
				{
					this.curNode = this.curNode.nextAttrValueChunk;
					this.nodes[this.index + this.attrCount + 1] = this.curNode;
					return true;
				}
				return false;
			}
		}

		public override void ResolveEntity()
		{
			if (this.curNode.type != XmlNodeType.EntityReference)
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
			}
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadAttributeValue || this.parsingFunction == XmlTextReaderImpl.ParsingFunction.FragmentAttribute)
			{
				switch (this.HandleGeneralEntityReference(this.curNode.localName, true, true, this.curNode.LinePos))
				{
				case XmlTextReaderImpl.EntityType.Expanded:
				case XmlTextReaderImpl.EntityType.ExpandedInAttribute:
					if (this.ps.charsUsed - this.ps.charPos == 0)
					{
						this.emptyEntityInAttributeResolved = true;
						goto IL_164;
					}
					goto IL_164;
				case XmlTextReaderImpl.EntityType.FakeExpanded:
					this.emptyEntityInAttributeResolved = true;
					goto IL_164;
				}
				throw new XmlException("An internal error has occurred.", string.Empty);
			}
			switch (this.HandleGeneralEntityReference(this.curNode.localName, false, true, this.curNode.LinePos))
			{
			case XmlTextReaderImpl.EntityType.Expanded:
			case XmlTextReaderImpl.EntityType.ExpandedInAttribute:
				this.nextParsingFunction = this.parsingFunction;
				if (this.ps.charsUsed - this.ps.charPos == 0 && !this.ps.entity.IsExternal)
				{
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.AfterResolveEmptyEntityInContent;
					goto IL_164;
				}
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.AfterResolveEntityInContent;
				goto IL_164;
			case XmlTextReaderImpl.EntityType.FakeExpanded:
				this.nextParsingFunction = this.parsingFunction;
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.AfterResolveEmptyEntityInContent;
				goto IL_164;
			}
			throw new XmlException("An internal error has occurred.", string.Empty);
			IL_164:
			this.ps.entityResolvedManually = true;
			this.index++;
		}

		internal XmlReader OuterReader
		{
			get
			{
				return this.outerReader;
			}
			set
			{
				this.outerReader = value;
			}
		}

		internal void MoveOffEntityReference()
		{
			if (this.outerReader.NodeType == XmlNodeType.EntityReference && this.parsingFunction == XmlTextReaderImpl.ParsingFunction.AfterResolveEntityInContent && !this.outerReader.Read())
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
			}
		}

		public override string ReadString()
		{
			this.MoveOffEntityReference();
			return base.ReadString();
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
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
			{
				if (this.incReadDecoder == this.base64Decoder)
				{
					return this.ReadContentAsBinary(buffer, index, count);
				}
			}
			else
			{
				if (this.readState != ReadState.Interactive)
				{
					return 0;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary)
				{
					throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
				}
				if (!XmlReader.CanReadContentAs(this.curNode.type))
				{
					throw base.CreateReadContentAsException("ReadContentAsBase64");
				}
				if (!this.InitReadContentAsBinary())
				{
					return 0;
				}
			}
			this.InitBase64Decoder();
			return this.ReadContentAsBinary(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
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
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
			{
				if (this.incReadDecoder == this.binHexDecoder)
				{
					return this.ReadContentAsBinary(buffer, index, count);
				}
			}
			else
			{
				if (this.readState != ReadState.Interactive)
				{
					return 0;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary)
				{
					throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
				}
				if (!XmlReader.CanReadContentAs(this.curNode.type))
				{
					throw base.CreateReadContentAsException("ReadContentAsBinHex");
				}
				if (!this.InitReadContentAsBinary())
				{
					return 0;
				}
			}
			this.InitBinHexDecoder();
			return this.ReadContentAsBinary(buffer, index, count);
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
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary)
			{
				if (this.incReadDecoder == this.base64Decoder)
				{
					return this.ReadElementContentAsBinary(buffer, index, count);
				}
			}
			else
			{
				if (this.readState != ReadState.Interactive)
				{
					return 0;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
				{
					throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
				}
				if (this.curNode.type != XmlNodeType.Element)
				{
					throw base.CreateReadElementContentAsException("ReadElementContentAsBinHex");
				}
				if (!this.InitReadElementContentAsBinary())
				{
					return 0;
				}
			}
			this.InitBase64Decoder();
			return this.ReadElementContentAsBinary(buffer, index, count);
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
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary)
			{
				if (this.incReadDecoder == this.binHexDecoder)
				{
					return this.ReadElementContentAsBinary(buffer, index, count);
				}
			}
			else
			{
				if (this.readState != ReadState.Interactive)
				{
					return 0;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
				{
					throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
				}
				if (this.curNode.type != XmlNodeType.Element)
				{
					throw base.CreateReadElementContentAsException("ReadElementContentAsBinHex");
				}
				if (!this.InitReadElementContentAsBinary())
				{
					return 0;
				}
			}
			this.InitBinHexDecoder();
			return this.ReadElementContentAsBinary(buffer, index, count);
		}

		public override bool CanReadValueChunk
		{
			get
			{
				return true;
			}
		}

		public override int ReadValueChunk(char[] buffer, int index, int count)
		{
			if (!XmlReader.HasValueInternal(this.curNode.type))
			{
				throw new InvalidOperationException(Res.GetString("The ReadValueAsChunk method is not supported on node type {0}.", new object[]
				{
					this.curNode.type
				}));
			}
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
			if (this.parsingFunction != XmlTextReaderImpl.ParsingFunction.InReadValueChunk)
			{
				if (this.readState != ReadState.Interactive)
				{
					return 0;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.PartialTextValue)
				{
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnPartialValue;
				}
				else
				{
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnCachedValue;
					this.nextNextParsingFunction = this.nextParsingFunction;
					this.nextParsingFunction = this.parsingFunction;
				}
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.InReadValueChunk;
				this.readValueOffset = 0;
			}
			if (count == 0)
			{
				return 0;
			}
			int num = 0;
			int num2 = this.curNode.CopyTo(this.readValueOffset, buffer, index + num, count - num);
			num += num2;
			this.readValueOffset += num2;
			if (num == count)
			{
				if (XmlCharType.IsHighSurrogate((int)buffer[index + count - 1]))
				{
					num--;
					this.readValueOffset--;
					if (num == 0)
					{
						this.Throw("The buffer is not large enough to fit a surrogate pair. Please provide a buffer of size at least 2 characters.");
					}
				}
				return num;
			}
			if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnPartialValue)
			{
				this.curNode.SetValue(string.Empty);
				bool flag = false;
				int num3 = 0;
				int num4 = 0;
				while (num < count && !flag)
				{
					int num5 = 0;
					flag = this.ParseText(out num3, out num4, ref num5);
					int num6 = count - num;
					if (num6 > num4 - num3)
					{
						num6 = num4 - num3;
					}
					XmlTextReaderImpl.BlockCopyChars(this.ps.chars, num3, buffer, index + num, num6);
					num += num6;
					num3 += num6;
				}
				this.incReadState = (flag ? XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnCachedValue : XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnPartialValue);
				if (num == count && XmlCharType.IsHighSurrogate((int)buffer[index + count - 1]))
				{
					num--;
					num3--;
					if (num == 0)
					{
						this.Throw("The buffer is not large enough to fit a surrogate pair. Please provide a buffer of size at least 2 characters.");
					}
				}
				this.readValueOffset = 0;
				this.curNode.SetValue(this.ps.chars, num3, num4 - num3);
			}
			return num;
		}

		public bool HasLineInfo()
		{
			return true;
		}

		public int LineNumber
		{
			get
			{
				return this.curNode.LineNo;
			}
		}

		public int LinePosition
		{
			get
			{
				return this.curNode.LinePos;
			}
		}

		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.GetNamespacesInScope(scope);
		}

		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.LookupNamespace(prefix);
		}

		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.LookupPrefix(namespaceName);
		}

		internal IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.namespaceManager.GetNamespacesInScope(scope);
		}

		internal string LookupPrefix(string namespaceName)
		{
			return this.namespaceManager.LookupPrefix(namespaceName);
		}

		internal bool Namespaces
		{
			get
			{
				return this.supportNamespaces;
			}
			set
			{
				if (this.readState != ReadState.Initial)
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				this.supportNamespaces = value;
				if (value)
				{
					if (this.namespaceManager is XmlTextReaderImpl.NoNamespaceManager)
					{
						if (this.fragment && this.fragmentParserContext != null && this.fragmentParserContext.NamespaceManager != null)
						{
							this.namespaceManager = this.fragmentParserContext.NamespaceManager;
						}
						else
						{
							this.namespaceManager = new XmlNamespaceManager(this.nameTable);
						}
					}
					this.xmlContext.defaultNamespace = this.namespaceManager.LookupNamespace(string.Empty);
					return;
				}
				if (!(this.namespaceManager is XmlTextReaderImpl.NoNamespaceManager))
				{
					this.namespaceManager = new XmlTextReaderImpl.NoNamespaceManager();
				}
				this.xmlContext.defaultNamespace = string.Empty;
			}
		}

		internal bool Normalization
		{
			get
			{
				return this.normalize;
			}
			set
			{
				if (this.readState == ReadState.Closed)
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				this.normalize = value;
				if (this.ps.entity == null || this.ps.entity.IsExternal)
				{
					this.ps.eolNormalized = !value;
				}
			}
		}

		internal Encoding Encoding
		{
			get
			{
				if (this.readState != ReadState.Interactive)
				{
					return null;
				}
				return this.reportedEncoding;
			}
		}

		internal WhitespaceHandling WhitespaceHandling
		{
			get
			{
				return this.whitespaceHandling;
			}
			set
			{
				if (this.readState == ReadState.Closed)
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				if (value > WhitespaceHandling.None)
				{
					throw new XmlException("Expected WhitespaceHandling.None, or WhitespaceHandling.All, or WhitespaceHandling.Significant.", string.Empty);
				}
				this.whitespaceHandling = value;
			}
		}

		internal DtdProcessing DtdProcessing
		{
			get
			{
				return this.dtdProcessing;
			}
			set
			{
				if (value > DtdProcessing.Parse)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.dtdProcessing = value;
			}
		}

		internal EntityHandling EntityHandling
		{
			get
			{
				return this.entityHandling;
			}
			set
			{
				if (value != EntityHandling.ExpandEntities && value != EntityHandling.ExpandCharEntities)
				{
					throw new XmlException("Expected EntityHandling.ExpandEntities or EntityHandling.ExpandCharEntities.", string.Empty);
				}
				this.entityHandling = value;
			}
		}

		internal bool IsResolverSet
		{
			get
			{
				return this.xmlResolverIsSet;
			}
		}

		internal XmlResolver XmlResolver
		{
			set
			{
				this.xmlResolver = value;
				this.xmlResolverIsSet = true;
				this.ps.baseUri = null;
				for (int i = 0; i <= this.parsingStatesStackTop; i++)
				{
					this.parsingStatesStack[i].baseUri = null;
				}
			}
		}

		internal void ResetState()
		{
			if (this.fragment)
			{
				this.Throw(new InvalidOperationException(Res.GetString("Cannot call ResetState when parsing an XML fragment.")));
			}
			if (this.readState == ReadState.Initial)
			{
				return;
			}
			this.ResetAttributes();
			while (this.namespaceManager.PopScope())
			{
			}
			while (this.InEntity)
			{
				this.HandleEntityEnd(true);
			}
			this.readState = ReadState.Initial;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.SwitchToInteractiveXmlDecl;
			this.nextParsingFunction = XmlTextReaderImpl.ParsingFunction.DocumentContent;
			this.curNode = this.nodes[0];
			this.curNode.Clear(XmlNodeType.None);
			this.curNode.SetLineInfo(0, 0);
			this.index = 0;
			this.rootElementParsed = false;
			this.charactersInDocument = 0L;
			this.charactersFromEntities = 0L;
			this.afterResetState = true;
		}

		internal TextReader GetRemainder()
		{
			XmlTextReaderImpl.ParsingFunction parsingFunction = this.parsingFunction;
			if (parsingFunction != XmlTextReaderImpl.ParsingFunction.OpenUrl)
			{
				if (parsingFunction - XmlTextReaderImpl.ParsingFunction.Eof <= 1)
				{
					return new StringReader(string.Empty);
				}
				if (parsingFunction == XmlTextReaderImpl.ParsingFunction.InIncrementalRead)
				{
					if (!this.InEntity)
					{
						this.stringBuilder.Append(this.ps.chars, this.incReadLeftStartPos, this.incReadLeftEndPos - this.incReadLeftStartPos);
					}
				}
			}
			else
			{
				this.OpenUrl();
			}
			while (this.InEntity)
			{
				this.HandleEntityEnd(true);
			}
			this.ps.appendMode = false;
			do
			{
				this.stringBuilder.Append(this.ps.chars, this.ps.charPos, this.ps.charsUsed - this.ps.charPos);
				this.ps.charPos = this.ps.charsUsed;
			}
			while (this.ReadData() != 0);
			this.OnEof();
			string s = this.stringBuilder.ToString();
			this.stringBuilder.Length = 0;
			return new StringReader(s);
		}

		internal int ReadChars(char[] buffer, int index, int count)
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InIncrementalRead)
			{
				if (this.incReadDecoder != this.readCharsDecoder)
				{
					if (this.readCharsDecoder == null)
					{
						this.readCharsDecoder = new IncrementalReadCharsDecoder();
					}
					this.readCharsDecoder.Reset();
					this.incReadDecoder = this.readCharsDecoder;
				}
				return this.IncrementalRead(buffer, index, count);
			}
			if (this.curNode.type != XmlNodeType.Element)
			{
				return 0;
			}
			if (this.curNode.IsEmptyElement)
			{
				this.outerReader.Read();
				return 0;
			}
			if (this.readCharsDecoder == null)
			{
				this.readCharsDecoder = new IncrementalReadCharsDecoder();
			}
			this.InitIncrementalRead(this.readCharsDecoder);
			return this.IncrementalRead(buffer, index, count);
		}

		internal int ReadBase64(byte[] array, int offset, int len)
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InIncrementalRead)
			{
				if (this.incReadDecoder != this.base64Decoder)
				{
					this.InitBase64Decoder();
				}
				return this.IncrementalRead(array, offset, len);
			}
			if (this.curNode.type != XmlNodeType.Element)
			{
				return 0;
			}
			if (this.curNode.IsEmptyElement)
			{
				this.outerReader.Read();
				return 0;
			}
			if (this.base64Decoder == null)
			{
				this.base64Decoder = new Base64Decoder();
			}
			this.InitIncrementalRead(this.base64Decoder);
			return this.IncrementalRead(array, offset, len);
		}

		internal int ReadBinHex(byte[] array, int offset, int len)
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InIncrementalRead)
			{
				if (this.incReadDecoder != this.binHexDecoder)
				{
					this.InitBinHexDecoder();
				}
				return this.IncrementalRead(array, offset, len);
			}
			if (this.curNode.type != XmlNodeType.Element)
			{
				return 0;
			}
			if (this.curNode.IsEmptyElement)
			{
				this.outerReader.Read();
				return 0;
			}
			if (this.binHexDecoder == null)
			{
				this.binHexDecoder = new BinHexDecoder();
			}
			this.InitIncrementalRead(this.binHexDecoder);
			return this.IncrementalRead(array, offset, len);
		}

		internal XmlNameTable DtdParserProxy_NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		internal IXmlNamespaceResolver DtdParserProxy_NamespaceResolver
		{
			get
			{
				return this.namespaceManager;
			}
		}

		internal bool DtdParserProxy_DtdValidation
		{
			get
			{
				return this.DtdValidation;
			}
		}

		internal bool DtdParserProxy_Normalization
		{
			get
			{
				return this.normalize;
			}
		}

		internal bool DtdParserProxy_Namespaces
		{
			get
			{
				return this.supportNamespaces;
			}
		}

		internal bool DtdParserProxy_V1CompatibilityMode
		{
			get
			{
				return this.v1Compat;
			}
		}

		internal Uri DtdParserProxy_BaseUri
		{
			get
			{
				if (this.ps.baseUriStr.Length > 0 && this.ps.baseUri == null && this.xmlResolver != null)
				{
					this.ps.baseUri = this.xmlResolver.ResolveUri(null, this.ps.baseUriStr);
				}
				return this.ps.baseUri;
			}
		}

		internal bool DtdParserProxy_IsEof
		{
			get
			{
				return this.ps.isEof;
			}
		}

		internal char[] DtdParserProxy_ParsingBuffer
		{
			get
			{
				return this.ps.chars;
			}
		}

		internal int DtdParserProxy_ParsingBufferLength
		{
			get
			{
				return this.ps.charsUsed;
			}
		}

		internal int DtdParserProxy_CurrentPosition
		{
			get
			{
				return this.ps.charPos;
			}
			set
			{
				this.ps.charPos = value;
			}
		}

		internal int DtdParserProxy_EntityStackLength
		{
			get
			{
				return this.parsingStatesStackTop + 1;
			}
		}

		internal bool DtdParserProxy_IsEntityEolNormalized
		{
			get
			{
				return this.ps.eolNormalized;
			}
		}

		internal IValidationEventHandling DtdParserProxy_ValidationEventHandling
		{
			get
			{
				return this.validationEventHandling;
			}
			set
			{
				this.validationEventHandling = value;
			}
		}

		internal void DtdParserProxy_OnNewLine(int pos)
		{
			this.OnNewLine(pos);
		}

		internal int DtdParserProxy_LineNo
		{
			get
			{
				return this.ps.LineNo;
			}
		}

		internal int DtdParserProxy_LineStartPosition
		{
			get
			{
				return this.ps.lineStartPos;
			}
		}

		internal int DtdParserProxy_ReadData()
		{
			return this.ReadData();
		}

		internal int DtdParserProxy_ParseNumericCharRef(StringBuilder internalSubsetBuilder)
		{
			XmlTextReaderImpl.EntityType entityType;
			return this.ParseNumericCharRef(true, internalSubsetBuilder, out entityType);
		}

		internal int DtdParserProxy_ParseNamedCharRef(bool expand, StringBuilder internalSubsetBuilder)
		{
			return this.ParseNamedCharRef(expand, internalSubsetBuilder);
		}

		internal void DtdParserProxy_ParsePI(StringBuilder sb)
		{
			if (sb == null)
			{
				XmlTextReaderImpl.ParsingMode parsingMode = this.parsingMode;
				this.parsingMode = XmlTextReaderImpl.ParsingMode.SkipNode;
				this.ParsePI(null);
				this.parsingMode = parsingMode;
				return;
			}
			this.ParsePI(sb);
		}

		internal void DtdParserProxy_ParseComment(StringBuilder sb)
		{
			try
			{
				if (sb == null)
				{
					XmlTextReaderImpl.ParsingMode parsingMode = this.parsingMode;
					this.parsingMode = XmlTextReaderImpl.ParsingMode.SkipNode;
					this.ParseCDataOrComment(XmlNodeType.Comment);
					this.parsingMode = parsingMode;
				}
				else
				{
					XmlTextReaderImpl.NodeData nodeData = this.curNode;
					this.curNode = this.AddNode(this.index + this.attrCount + 1, this.index);
					this.ParseCDataOrComment(XmlNodeType.Comment);
					this.curNode.CopyTo(0, sb);
					this.curNode = nodeData;
				}
			}
			catch (XmlException ex)
			{
				if (!(ex.ResString == "Unexpected end of file while parsing {0} has occurred.") || this.ps.entity == null)
				{
					throw;
				}
				this.SendValidationEvent(XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", null, this.ps.LineNo, this.ps.LinePos);
			}
		}

		private bool IsResolverNull
		{
			get
			{
				return this.xmlResolver == null || (XmlReaderSection.ProhibitDefaultUrlResolver && !this.xmlResolverIsSet);
			}
		}

		private XmlResolver GetTempResolver()
		{
			if (this.xmlResolver != null)
			{
				return this.xmlResolver;
			}
			return new XmlUrlResolver();
		}

		internal bool DtdParserProxy_PushEntity(IDtdEntityInfo entity, out int entityId)
		{
			bool result;
			if (entity.IsExternal)
			{
				if (this.IsResolverNull)
				{
					entityId = -1;
					return false;
				}
				result = this.PushExternalEntity(entity);
			}
			else
			{
				this.PushInternalEntity(entity);
				result = true;
			}
			entityId = this.ps.entityId;
			return result;
		}

		internal bool DtdParserProxy_PopEntity(out IDtdEntityInfo oldEntity, out int newEntityId)
		{
			if (this.parsingStatesStackTop == -1)
			{
				oldEntity = null;
				newEntityId = -1;
				return false;
			}
			oldEntity = this.ps.entity;
			this.PopEntity();
			newEntityId = this.ps.entityId;
			return true;
		}

		internal bool DtdParserProxy_PushExternalSubset(string systemId, string publicId)
		{
			if (this.IsResolverNull)
			{
				return false;
			}
			if (this.ps.baseUri == null && !string.IsNullOrEmpty(this.ps.baseUriStr))
			{
				this.ps.baseUri = this.xmlResolver.ResolveUri(null, this.ps.baseUriStr);
			}
			this.PushExternalEntityOrSubset(publicId, systemId, this.ps.baseUri, null);
			this.ps.entity = null;
			this.ps.entityId = 0;
			int charPos = this.ps.charPos;
			if (this.v1Compat)
			{
				this.EatWhitespaces(null);
			}
			if (!this.ParseXmlDeclaration(true))
			{
				this.ps.charPos = charPos;
			}
			return true;
		}

		internal void DtdParserProxy_PushInternalDtd(string baseUri, string internalDtd)
		{
			this.PushParsingState();
			this.RegisterConsumedCharacters((long)internalDtd.Length, false);
			this.InitStringInput(baseUri, Encoding.Unicode, internalDtd);
			this.ps.entity = null;
			this.ps.entityId = 0;
			this.ps.eolNormalized = false;
		}

		internal void DtdParserProxy_Throw(Exception e)
		{
			this.Throw(e);
		}

		internal void DtdParserProxy_OnSystemId(string systemId, LineInfo keywordLineInfo, LineInfo systemLiteralLineInfo)
		{
			XmlTextReaderImpl.NodeData nodeData = this.AddAttributeNoChecks("SYSTEM", this.index + 1);
			nodeData.SetValue(systemId);
			nodeData.lineInfo = keywordLineInfo;
			nodeData.lineInfo2 = systemLiteralLineInfo;
		}

		internal void DtdParserProxy_OnPublicId(string publicId, LineInfo keywordLineInfo, LineInfo publicLiteralLineInfo)
		{
			XmlTextReaderImpl.NodeData nodeData = this.AddAttributeNoChecks("PUBLIC", this.index + 1);
			nodeData.SetValue(publicId);
			nodeData.lineInfo = keywordLineInfo;
			nodeData.lineInfo2 = publicLiteralLineInfo;
		}

		private void Throw(int pos, string res, string arg)
		{
			this.ps.charPos = pos;
			this.Throw(res, arg);
		}

		private void Throw(int pos, string res, string[] args)
		{
			this.ps.charPos = pos;
			this.Throw(res, args);
		}

		private void Throw(int pos, string res)
		{
			this.ps.charPos = pos;
			this.Throw(res, string.Empty);
		}

		private void Throw(string res)
		{
			this.Throw(res, string.Empty);
		}

		private void Throw(string res, int lineNo, int linePos)
		{
			this.Throw(new XmlException(res, string.Empty, lineNo, linePos, this.ps.baseUriStr));
		}

		private void Throw(string res, string arg)
		{
			this.Throw(new XmlException(res, arg, this.ps.LineNo, this.ps.LinePos, this.ps.baseUriStr));
		}

		private void Throw(string res, string arg, int lineNo, int linePos)
		{
			this.Throw(new XmlException(res, arg, lineNo, linePos, this.ps.baseUriStr));
		}

		private void Throw(string res, string[] args)
		{
			this.Throw(new XmlException(res, args, this.ps.LineNo, this.ps.LinePos, this.ps.baseUriStr));
		}

		private void Throw(string res, string arg, Exception innerException)
		{
			this.Throw(res, new string[]
			{
				arg
			}, innerException);
		}

		private void Throw(string res, string[] args, Exception innerException)
		{
			this.Throw(new XmlException(res, args, innerException, this.ps.LineNo, this.ps.LinePos, this.ps.baseUriStr));
		}

		private void Throw(Exception e)
		{
			this.SetErrorState();
			XmlException ex = e as XmlException;
			if (ex != null)
			{
				this.curNode.SetLineInfo(ex.LineNumber, ex.LinePosition);
			}
			throw e;
		}

		private void ReThrow(Exception e, int lineNo, int linePos)
		{
			this.Throw(new XmlException(e.Message, null, lineNo, linePos, this.ps.baseUriStr));
		}

		private void ThrowWithoutLineInfo(string res)
		{
			this.Throw(new XmlException(res, string.Empty, this.ps.baseUriStr));
		}

		private void ThrowWithoutLineInfo(string res, string arg)
		{
			this.Throw(new XmlException(res, arg, this.ps.baseUriStr));
		}

		private void ThrowWithoutLineInfo(string res, string[] args, Exception innerException)
		{
			this.Throw(new XmlException(res, args, innerException, 0, 0, this.ps.baseUriStr));
		}

		private void ThrowInvalidChar(char[] data, int length, int invCharPos)
		{
			this.Throw(invCharPos, "'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(data, length, invCharPos));
		}

		private void SetErrorState()
		{
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.Error;
			this.readState = ReadState.Error;
		}

		private void SendValidationEvent(XmlSeverityType severity, string code, string arg, int lineNo, int linePos)
		{
			this.SendValidationEvent(severity, new XmlSchemaException(code, arg, this.ps.baseUriStr, lineNo, linePos));
		}

		private void SendValidationEvent(XmlSeverityType severity, XmlSchemaException exception)
		{
			if (this.validationEventHandling != null)
			{
				this.validationEventHandling.SendEvent(exception, severity);
			}
		}

		private bool InAttributeValueIterator
		{
			get
			{
				return this.attrCount > 0 && this.parsingFunction >= XmlTextReaderImpl.ParsingFunction.InReadAttributeValue;
			}
		}

		private void FinishAttributeValueIterator()
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadValueChunk)
			{
				this.FinishReadValueChunk();
			}
			else if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
			{
				this.FinishReadContentAsBinary();
			}
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadAttributeValue)
			{
				while (this.ps.entityId != this.attributeValueBaseEntityId)
				{
					this.HandleEntityEnd(false);
				}
				this.emptyEntityInAttributeResolved = false;
				this.parsingFunction = this.nextParsingFunction;
				this.nextParsingFunction = ((this.index > 0) ? XmlTextReaderImpl.ParsingFunction.ElementContent : XmlTextReaderImpl.ParsingFunction.DocumentContent);
			}
		}

		private bool DtdValidation
		{
			get
			{
				return this.validationEventHandling != null;
			}
		}

		private void InitStreamInput(Stream stream, Encoding encoding)
		{
			this.InitStreamInput(null, string.Empty, stream, null, 0, encoding);
		}

		private void InitStreamInput(string baseUriStr, Stream stream, Encoding encoding)
		{
			this.InitStreamInput(null, baseUriStr, stream, null, 0, encoding);
		}

		private void InitStreamInput(Uri baseUri, Stream stream, Encoding encoding)
		{
			this.InitStreamInput(baseUri, baseUri.ToString(), stream, null, 0, encoding);
		}

		private void InitStreamInput(Uri baseUri, string baseUriStr, Stream stream, Encoding encoding)
		{
			this.InitStreamInput(baseUri, baseUriStr, stream, null, 0, encoding);
		}

		private void InitStreamInput(Uri baseUri, string baseUriStr, Stream stream, byte[] bytes, int byteCount, Encoding encoding)
		{
			this.ps.stream = stream;
			this.ps.baseUri = baseUri;
			this.ps.baseUriStr = baseUriStr;
			int num;
			if (bytes != null)
			{
				this.ps.bytes = bytes;
				this.ps.bytesUsed = byteCount;
				num = this.ps.bytes.Length;
			}
			else
			{
				if (this.laterInitParam != null && this.laterInitParam.useAsync)
				{
					num = 65536;
				}
				else
				{
					num = XmlReader.CalcBufferSize(stream);
				}
				if (this.ps.bytes == null || this.ps.bytes.Length < num)
				{
					this.ps.bytes = new byte[num];
				}
			}
			if (this.ps.chars == null || this.ps.chars.Length < num + 1)
			{
				this.ps.chars = new char[num + 1];
			}
			this.ps.bytePos = 0;
			while (this.ps.bytesUsed < 4 && this.ps.bytes.Length - this.ps.bytesUsed > 0)
			{
				int num2 = stream.Read(this.ps.bytes, this.ps.bytesUsed, this.ps.bytes.Length - this.ps.bytesUsed);
				if (num2 == 0)
				{
					this.ps.isStreamEof = true;
					break;
				}
				this.ps.bytesUsed = this.ps.bytesUsed + num2;
			}
			if (encoding == null)
			{
				encoding = this.DetectEncoding();
			}
			this.SetupEncoding(encoding);
			byte[] preamble = this.ps.encoding.GetPreamble();
			int num3 = preamble.Length;
			int num4 = 0;
			while (num4 < num3 && num4 < this.ps.bytesUsed && this.ps.bytes[num4] == preamble[num4])
			{
				num4++;
			}
			if (num4 == num3)
			{
				this.ps.bytePos = num3;
			}
			this.documentStartBytePos = this.ps.bytePos;
			this.ps.eolNormalized = !this.normalize;
			this.ps.appendMode = true;
			this.ReadData();
		}

		private void InitTextReaderInput(string baseUriStr, TextReader input)
		{
			this.InitTextReaderInput(baseUriStr, null, input);
		}

		private void InitTextReaderInput(string baseUriStr, Uri baseUri, TextReader input)
		{
			this.ps.textReader = input;
			this.ps.baseUriStr = baseUriStr;
			this.ps.baseUri = baseUri;
			if (this.ps.chars == null)
			{
				if (this.laterInitParam != null && this.laterInitParam.useAsync)
				{
					this.ps.chars = new char[65537];
				}
				else
				{
					this.ps.chars = new char[4097];
				}
			}
			this.ps.encoding = Encoding.Unicode;
			this.ps.eolNormalized = !this.normalize;
			this.ps.appendMode = true;
			this.ReadData();
		}

		private void InitStringInput(string baseUriStr, Encoding originalEncoding, string str)
		{
			this.ps.baseUriStr = baseUriStr;
			this.ps.baseUri = null;
			int length = str.Length;
			this.ps.chars = new char[length + 1];
			str.CopyTo(0, this.ps.chars, 0, str.Length);
			this.ps.charsUsed = length;
			this.ps.chars[length] = '\0';
			this.ps.encoding = originalEncoding;
			this.ps.eolNormalized = !this.normalize;
			this.ps.isEof = true;
		}

		private void InitFragmentReader(XmlNodeType fragmentType, XmlParserContext parserContext, bool allowXmlDeclFragment)
		{
			this.fragmentParserContext = parserContext;
			if (parserContext != null)
			{
				if (parserContext.NamespaceManager != null)
				{
					this.namespaceManager = parserContext.NamespaceManager;
					this.xmlContext.defaultNamespace = this.namespaceManager.LookupNamespace(string.Empty);
				}
				else
				{
					this.namespaceManager = new XmlNamespaceManager(this.nameTable);
				}
				this.ps.baseUriStr = parserContext.BaseURI;
				this.ps.baseUri = null;
				this.xmlContext.xmlLang = parserContext.XmlLang;
				this.xmlContext.xmlSpace = parserContext.XmlSpace;
			}
			else
			{
				this.namespaceManager = new XmlNamespaceManager(this.nameTable);
				this.ps.baseUriStr = string.Empty;
				this.ps.baseUri = null;
			}
			this.reportedBaseUri = this.ps.baseUriStr;
			if (fragmentType <= XmlNodeType.Attribute)
			{
				if (fragmentType == XmlNodeType.Element)
				{
					this.nextParsingFunction = XmlTextReaderImpl.ParsingFunction.DocumentContent;
					goto IL_147;
				}
				if (fragmentType == XmlNodeType.Attribute)
				{
					this.ps.appendMode = false;
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.SwitchToInteractive;
					this.nextParsingFunction = XmlTextReaderImpl.ParsingFunction.FragmentAttribute;
					goto IL_147;
				}
			}
			else
			{
				if (fragmentType == XmlNodeType.Document)
				{
					goto IL_147;
				}
				if (fragmentType == XmlNodeType.XmlDeclaration)
				{
					if (allowXmlDeclFragment)
					{
						this.ps.appendMode = false;
						this.parsingFunction = XmlTextReaderImpl.ParsingFunction.SwitchToInteractive;
						this.nextParsingFunction = XmlTextReaderImpl.ParsingFunction.XmlDeclarationFragment;
						goto IL_147;
					}
				}
			}
			this.Throw("XmlNodeType {0} is not supported for partial content parsing.", fragmentType.ToString());
			return;
			IL_147:
			this.fragmentType = fragmentType;
			this.fragment = true;
		}

		private void ProcessDtdFromParserContext(XmlParserContext context)
		{
			switch (this.dtdProcessing)
			{
			case DtdProcessing.Prohibit:
				this.ThrowWithoutLineInfo("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.");
				return;
			case DtdProcessing.Ignore:
				break;
			case DtdProcessing.Parse:
				this.ParseDtdFromParserContext();
				break;
			default:
				return;
			}
		}

		private void OpenUrl()
		{
			XmlResolver tempResolver = this.GetTempResolver();
			if (!(this.ps.baseUri != null))
			{
				this.ps.baseUri = tempResolver.ResolveUri(null, this.url);
				this.ps.baseUriStr = this.ps.baseUri.ToString();
			}
			try
			{
				this.OpenUrlDelegate(tempResolver);
			}
			catch
			{
				this.SetErrorState();
				throw;
			}
			if (this.ps.stream == null)
			{
				this.ThrowWithoutLineInfo("Cannot resolve '{0}'.", this.ps.baseUriStr);
			}
			this.InitStreamInput(this.ps.baseUri, this.ps.baseUriStr, this.ps.stream, null);
			this.reportedEncoding = this.ps.encoding;
		}

		private void OpenUrlDelegate(object xmlResolver)
		{
			this.ps.stream = (Stream)this.GetTempResolver().GetEntity(this.ps.baseUri, null, typeof(Stream));
		}

		private Encoding DetectEncoding()
		{
			if (this.ps.bytesUsed < 2)
			{
				return null;
			}
			int num = (int)this.ps.bytes[0] << 8 | (int)this.ps.bytes[1];
			int num2 = (this.ps.bytesUsed >= 4) ? ((int)this.ps.bytes[2] << 8 | (int)this.ps.bytes[3]) : 0;
			if (num <= 15360)
			{
				if (num != 0)
				{
					if (num != 60)
					{
						if (num == 15360)
						{
							if (num2 == 0)
							{
								return Ucs4Encoding.UCS4_Littleendian;
							}
							return Encoding.Unicode;
						}
					}
					else
					{
						if (num2 == 0)
						{
							return Ucs4Encoding.UCS4_3412;
						}
						return Encoding.BigEndianUnicode;
					}
				}
				else if (num2 <= 15360)
				{
					if (num2 == 60)
					{
						return Ucs4Encoding.UCS4_Bigendian;
					}
					if (num2 == 15360)
					{
						return Ucs4Encoding.UCS4_2143;
					}
				}
				else
				{
					if (num2 == 65279)
					{
						return Ucs4Encoding.UCS4_Bigendian;
					}
					if (num2 == 65534)
					{
						return Ucs4Encoding.UCS4_2143;
					}
				}
			}
			else if (num <= 61371)
			{
				if (num != 19567)
				{
					if (num == 61371)
					{
						if ((num2 & 65280) == 48896)
						{
							return new UTF8Encoding(true, true);
						}
					}
				}
				else if (num2 == 42900)
				{
					this.Throw("System does not support '{0}' encoding.", "ebcdic");
				}
			}
			else if (num != 65279)
			{
				if (num == 65534)
				{
					if (num2 == 0)
					{
						return Ucs4Encoding.UCS4_Littleendian;
					}
					return Encoding.Unicode;
				}
			}
			else
			{
				if (num2 == 0)
				{
					return Ucs4Encoding.UCS4_3412;
				}
				return Encoding.BigEndianUnicode;
			}
			return null;
		}

		private void SetupEncoding(Encoding encoding)
		{
			if (encoding == null)
			{
				this.ps.encoding = Encoding.UTF8;
				this.ps.decoder = new SafeAsciiDecoder();
				return;
			}
			this.ps.encoding = encoding;
			string webName = this.ps.encoding.WebName;
			if (webName == "utf-16")
			{
				this.ps.decoder = new UTF16Decoder(false);
				return;
			}
			if (!(webName == "utf-16BE"))
			{
				this.ps.decoder = encoding.GetDecoder();
				return;
			}
			this.ps.decoder = new UTF16Decoder(true);
		}

		private void SwitchEncoding(Encoding newEncoding)
		{
			if ((newEncoding.WebName != this.ps.encoding.WebName || this.ps.decoder is SafeAsciiDecoder) && !this.afterResetState)
			{
				this.UnDecodeChars();
				this.ps.appendMode = false;
				this.SetupEncoding(newEncoding);
				this.ReadData();
			}
		}

		private Encoding CheckEncoding(string newEncodingName)
		{
			if (this.ps.stream == null)
			{
				return this.ps.encoding;
			}
			if (string.Compare(newEncodingName, "ucs-2", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(newEncodingName, "utf-16", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(newEncodingName, "iso-10646-ucs-2", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(newEncodingName, "ucs-4", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (this.ps.encoding.WebName != "utf-16BE" && this.ps.encoding.WebName != "utf-16" && string.Compare(newEncodingName, "ucs-4", StringComparison.OrdinalIgnoreCase) != 0)
				{
					if (this.afterResetState)
					{
						this.Throw("'{0}' is an invalid value for the 'encoding' attribute. The encoding cannot be switched after a call to ResetState.", newEncodingName);
					}
					else
					{
						this.ThrowWithoutLineInfo("There is no Unicode byte order mark. Cannot switch to Unicode.");
					}
				}
				return this.ps.encoding;
			}
			Encoding encoding = null;
			if (string.Compare(newEncodingName, "utf-8", StringComparison.OrdinalIgnoreCase) == 0)
			{
				encoding = new UTF8Encoding(true, true);
			}
			else
			{
				try
				{
					encoding = Encoding.GetEncoding(newEncodingName);
				}
				catch (NotSupportedException innerException)
				{
					this.Throw("System does not support '{0}' encoding.", newEncodingName, innerException);
				}
				catch (ArgumentException innerException2)
				{
					this.Throw("System does not support '{0}' encoding.", newEncodingName, innerException2);
				}
			}
			if (this.afterResetState && this.ps.encoding.WebName != encoding.WebName)
			{
				this.Throw("'{0}' is an invalid value for the 'encoding' attribute. The encoding cannot be switched after a call to ResetState.", newEncodingName);
			}
			return encoding;
		}

		private void UnDecodeChars()
		{
			if (this.maxCharactersInDocument > 0L)
			{
				this.charactersInDocument -= (long)(this.ps.charsUsed - this.ps.charPos);
			}
			if (this.maxCharactersFromEntities > 0L && this.InEntity)
			{
				this.charactersFromEntities -= (long)(this.ps.charsUsed - this.ps.charPos);
			}
			this.ps.bytePos = this.documentStartBytePos;
			if (this.ps.charPos > 0)
			{
				this.ps.bytePos = this.ps.bytePos + this.ps.encoding.GetByteCount(this.ps.chars, 0, this.ps.charPos);
			}
			this.ps.charsUsed = this.ps.charPos;
			this.ps.isEof = false;
		}

		private void SwitchEncodingToUTF8()
		{
			this.SwitchEncoding(new UTF8Encoding(true, true));
		}

		private int ReadData()
		{
			if (this.ps.isEof)
			{
				return 0;
			}
			int num;
			if (this.ps.appendMode)
			{
				if (this.ps.charsUsed == this.ps.chars.Length - 1)
				{
					for (int i = 0; i < this.attrCount; i++)
					{
						this.nodes[this.index + i + 1].OnBufferInvalidated();
					}
					char[] array = new char[this.ps.chars.Length * 2];
					XmlTextReaderImpl.BlockCopyChars(this.ps.chars, 0, array, 0, this.ps.chars.Length);
					this.ps.chars = array;
				}
				if (this.ps.stream != null && this.ps.bytesUsed - this.ps.bytePos < 6 && this.ps.bytes.Length - this.ps.bytesUsed < 6)
				{
					byte[] array2 = new byte[this.ps.bytes.Length * 2];
					XmlTextReaderImpl.BlockCopy(this.ps.bytes, 0, array2, 0, this.ps.bytesUsed);
					this.ps.bytes = array2;
				}
				num = this.ps.chars.Length - this.ps.charsUsed - 1;
				if (num > 80)
				{
					num = 80;
				}
			}
			else
			{
				int num2 = this.ps.chars.Length;
				if (num2 - this.ps.charsUsed <= num2 / 2)
				{
					for (int j = 0; j < this.attrCount; j++)
					{
						this.nodes[this.index + j + 1].OnBufferInvalidated();
					}
					int num3 = this.ps.charsUsed - this.ps.charPos;
					if (num3 < num2 - 1)
					{
						this.ps.lineStartPos = this.ps.lineStartPos - this.ps.charPos;
						if (num3 > 0)
						{
							XmlTextReaderImpl.BlockCopyChars(this.ps.chars, this.ps.charPos, this.ps.chars, 0, num3);
						}
						this.ps.charPos = 0;
						this.ps.charsUsed = num3;
					}
					else
					{
						char[] array3 = new char[this.ps.chars.Length * 2];
						XmlTextReaderImpl.BlockCopyChars(this.ps.chars, 0, array3, 0, this.ps.chars.Length);
						this.ps.chars = array3;
					}
				}
				if (this.ps.stream != null)
				{
					int num4 = this.ps.bytesUsed - this.ps.bytePos;
					if (num4 <= 128)
					{
						if (num4 == 0)
						{
							this.ps.bytesUsed = 0;
						}
						else
						{
							XmlTextReaderImpl.BlockCopy(this.ps.bytes, this.ps.bytePos, this.ps.bytes, 0, num4);
							this.ps.bytesUsed = num4;
						}
						this.ps.bytePos = 0;
					}
				}
				num = this.ps.chars.Length - this.ps.charsUsed - 1;
			}
			if (this.ps.stream != null)
			{
				if (!this.ps.isStreamEof && this.ps.bytePos == this.ps.bytesUsed && this.ps.bytes.Length - this.ps.bytesUsed > 0)
				{
					int num5 = this.ps.stream.Read(this.ps.bytes, this.ps.bytesUsed, this.ps.bytes.Length - this.ps.bytesUsed);
					if (num5 == 0)
					{
						this.ps.isStreamEof = true;
					}
					this.ps.bytesUsed = this.ps.bytesUsed + num5;
				}
				int bytePos = this.ps.bytePos;
				num = this.GetChars(num);
				if (num == 0 && this.ps.bytePos != bytePos)
				{
					return this.ReadData();
				}
			}
			else if (this.ps.textReader != null)
			{
				num = this.ps.textReader.Read(this.ps.chars, this.ps.charsUsed, this.ps.chars.Length - this.ps.charsUsed - 1);
				this.ps.charsUsed = this.ps.charsUsed + num;
			}
			else
			{
				num = 0;
			}
			this.RegisterConsumedCharacters((long)num, this.InEntity);
			if (num == 0)
			{
				this.ps.isEof = true;
			}
			this.ps.chars[this.ps.charsUsed] = '\0';
			return num;
		}

		private int GetChars(int maxCharsCount)
		{
			int num = this.ps.bytesUsed - this.ps.bytePos;
			if (num == 0)
			{
				return 0;
			}
			int num2;
			try
			{
				bool flag;
				this.ps.decoder.Convert(this.ps.bytes, this.ps.bytePos, num, this.ps.chars, this.ps.charsUsed, maxCharsCount, false, out num, out num2, out flag);
			}
			catch (ArgumentException)
			{
				this.InvalidCharRecovery(ref num, out num2);
			}
			this.ps.bytePos = this.ps.bytePos + num;
			this.ps.charsUsed = this.ps.charsUsed + num2;
			return num2;
		}

		private void InvalidCharRecovery(ref int bytesCount, out int charsCount)
		{
			int num = 0;
			int i = 0;
			try
			{
				while (i < bytesCount)
				{
					int num2;
					int num3;
					bool flag;
					this.ps.decoder.Convert(this.ps.bytes, this.ps.bytePos + i, 1, this.ps.chars, this.ps.charsUsed + num, 1, false, out num2, out num3, out flag);
					num += num3;
					i += num2;
				}
			}
			catch (ArgumentException)
			{
			}
			if (num == 0)
			{
				this.Throw(this.ps.charsUsed, "Invalid character in the given encoding.");
			}
			charsCount = num;
			bytesCount = i;
		}

		internal void Close(bool closeInput)
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.ReaderClosed)
			{
				return;
			}
			while (this.InEntity)
			{
				this.PopParsingState();
			}
			this.ps.Close(closeInput);
			this.curNode = XmlTextReaderImpl.NodeData.None;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.ReaderClosed;
			this.reportedEncoding = null;
			this.reportedBaseUri = string.Empty;
			this.readState = ReadState.Closed;
			this.fullAttrCleanup = false;
			this.ResetAttributes();
			this.laterInitParam = null;
		}

		private void ShiftBuffer(int sourcePos, int destPos, int count)
		{
			XmlTextReaderImpl.BlockCopyChars(this.ps.chars, sourcePos, this.ps.chars, destPos, count);
		}

		private bool ParseXmlDeclaration(bool isTextDecl)
		{
			while (this.ps.charsUsed - this.ps.charPos < 6)
			{
				if (this.ReadData() == 0)
				{
					IL_7E0:
					if (!isTextDecl)
					{
						this.parsingFunction = this.nextParsingFunction;
					}
					if (this.afterResetState)
					{
						string webName = this.ps.encoding.WebName;
						if (webName != "utf-8" && webName != "utf-16" && webName != "utf-16BE" && !(this.ps.encoding is Ucs4Encoding))
						{
							this.Throw("'{0}' is an invalid value for the 'encoding' attribute. The encoding cannot be switched after a call to ResetState.", (this.ps.encoding.GetByteCount("A") == 1) ? "UTF-8" : "UTF-16");
						}
					}
					if (this.ps.decoder is SafeAsciiDecoder)
					{
						this.SwitchEncodingToUTF8();
					}
					this.ps.appendMode = false;
					return false;
				}
			}
			if (XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, 5, "<?xml") && !this.xmlCharType.IsNameSingleChar(this.ps.chars[this.ps.charPos + 5]))
			{
				if (!isTextDecl)
				{
					this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos + 2);
					this.curNode.SetNamedNode(XmlNodeType.XmlDeclaration, this.Xml);
				}
				this.ps.charPos = this.ps.charPos + 5;
				StringBuilder stringBuilder = isTextDecl ? new StringBuilder() : this.stringBuilder;
				int num = 0;
				Encoding encoding = null;
				for (;;)
				{
					int length = stringBuilder.Length;
					int num2 = this.EatWhitespaces((num == 0) ? null : stringBuilder);
					if (this.ps.chars[this.ps.charPos] == '?')
					{
						stringBuilder.Length = length;
						if (this.ps.chars[this.ps.charPos + 1] == '>')
						{
							break;
						}
						if (this.ps.charPos + 1 == this.ps.charsUsed)
						{
							goto IL_7B8;
						}
						this.ThrowUnexpectedToken("'>'");
					}
					if (num2 == 0 && num != 0)
					{
						this.ThrowUnexpectedToken("?>");
					}
					int num3 = this.ParseName();
					XmlTextReaderImpl.NodeData nodeData = null;
					char c = this.ps.chars[this.ps.charPos];
					if (c != 'e')
					{
						if (c != 's')
						{
							if (c != 'v' || !XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, num3 - this.ps.charPos, "version") || num != 0)
							{
								goto IL_3B5;
							}
							if (!isTextDecl)
							{
								nodeData = this.AddAttributeNoChecks("version", 1);
							}
						}
						else
						{
							if (!XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, num3 - this.ps.charPos, "standalone") || (num != 1 && num != 2) || isTextDecl)
							{
								goto IL_3B5;
							}
							if (!isTextDecl)
							{
								nodeData = this.AddAttributeNoChecks("standalone", 1);
							}
							num = 2;
						}
					}
					else
					{
						if (!XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, num3 - this.ps.charPos, "encoding") || (num != 1 && (!isTextDecl || num != 0)))
						{
							goto IL_3B5;
						}
						if (!isTextDecl)
						{
							nodeData = this.AddAttributeNoChecks("encoding", 1);
						}
						num = 1;
					}
					IL_3CA:
					if (!isTextDecl)
					{
						nodeData.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
					}
					stringBuilder.Append(this.ps.chars, this.ps.charPos, num3 - this.ps.charPos);
					this.ps.charPos = num3;
					if (this.ps.chars[this.ps.charPos] != '=')
					{
						this.EatWhitespaces(stringBuilder);
						if (this.ps.chars[this.ps.charPos] != '=')
						{
							this.ThrowUnexpectedToken("=");
						}
					}
					stringBuilder.Append('=');
					this.ps.charPos = this.ps.charPos + 1;
					char c2 = this.ps.chars[this.ps.charPos];
					if (c2 != '"' && c2 != '\'')
					{
						this.EatWhitespaces(stringBuilder);
						c2 = this.ps.chars[this.ps.charPos];
						if (c2 != '"' && c2 != '\'')
						{
							this.ThrowUnexpectedToken("\"", "'");
						}
					}
					stringBuilder.Append(c2);
					this.ps.charPos = this.ps.charPos + 1;
					if (!isTextDecl)
					{
						nodeData.quoteChar = c2;
						nodeData.SetLineInfo2(this.ps.LineNo, this.ps.LinePos);
					}
					int num4 = this.ps.charPos;
					char[] chars;
					for (;;)
					{
						chars = this.ps.chars;
						while ((this.xmlCharType.charProperties[(int)chars[num4]] & 128) != 0)
						{
							num4++;
						}
						if (this.ps.chars[num4] == c2)
						{
							break;
						}
						if (num4 != this.ps.charsUsed)
						{
							goto IL_7A3;
						}
						if (this.ReadData() == 0)
						{
							goto Block_57;
						}
					}
					switch (num)
					{
					case 0:
						if (XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos, "1.0"))
						{
							if (!isTextDecl)
							{
								nodeData.SetValue(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos);
							}
							num = 1;
						}
						else
						{
							string arg = new string(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos);
							this.Throw("Version number '{0}' is invalid.", arg);
						}
						break;
					case 1:
					{
						string text = new string(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos);
						encoding = this.CheckEncoding(text);
						if (!isTextDecl)
						{
							nodeData.SetValue(text);
						}
						num = 2;
						break;
					}
					case 2:
						if (XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos, "yes"))
						{
							this.standalone = true;
						}
						else if (XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos, "no"))
						{
							this.standalone = false;
						}
						else
						{
							this.Throw("Syntax for an XML declaration is invalid.", this.ps.LineNo, this.ps.LinePos - 1);
						}
						if (!isTextDecl)
						{
							nodeData.SetValue(this.ps.chars, this.ps.charPos, num4 - this.ps.charPos);
						}
						num = 3;
						break;
					}
					stringBuilder.Append(chars, this.ps.charPos, num4 - this.ps.charPos);
					stringBuilder.Append(c2);
					this.ps.charPos = num4 + 1;
					continue;
					Block_57:
					this.Throw("There is an unclosed literal string.");
					goto IL_7B8;
					IL_7A3:
					this.Throw(isTextDecl ? "Invalid text declaration." : "Syntax for an XML declaration is invalid.");
					goto IL_7B8;
					IL_3B5:
					this.Throw(isTextDecl ? "Invalid text declaration." : "Syntax for an XML declaration is invalid.");
					goto IL_3CA;
					IL_7B8:
					if (this.ps.isEof || this.ReadData() == 0)
					{
						this.Throw("Unexpected end of file has occurred.");
					}
				}
				if (num == 0)
				{
					this.Throw(isTextDecl ? "Invalid text declaration." : "Syntax for an XML declaration is invalid.");
				}
				this.ps.charPos = this.ps.charPos + 2;
				if (!isTextDecl)
				{
					this.curNode.SetValue(stringBuilder.ToString());
					stringBuilder.Length = 0;
					this.nextParsingFunction = this.parsingFunction;
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.ResetAttributesRootLevel;
				}
				if (encoding == null)
				{
					if (isTextDecl)
					{
						this.Throw("Invalid text declaration.");
					}
					if (this.afterResetState)
					{
						string webName2 = this.ps.encoding.WebName;
						if (webName2 != "utf-8" && webName2 != "utf-16" && webName2 != "utf-16BE" && !(this.ps.encoding is Ucs4Encoding))
						{
							this.Throw("'{0}' is an invalid value for the 'encoding' attribute. The encoding cannot be switched after a call to ResetState.", (this.ps.encoding.GetByteCount("A") == 1) ? "UTF-8" : "UTF-16");
						}
					}
					if (this.ps.decoder is SafeAsciiDecoder)
					{
						this.SwitchEncodingToUTF8();
					}
				}
				else
				{
					this.SwitchEncoding(encoding);
				}
				this.ps.appendMode = false;
				return true;
			}
			goto IL_7E0;
		}

		private bool ParseDocumentContent()
		{
			bool flag = false;
			int num;
			for (;;)
			{
				bool flag2 = false;
				num = this.ps.charPos;
				char[] chars = this.ps.chars;
				if (chars[num] == '<')
				{
					flag2 = true;
					if (this.ps.charsUsed - num >= 4)
					{
						num++;
						char c = chars[num];
						if (c != '!')
						{
							if (c != '/')
							{
								if (c != '?')
								{
									goto IL_1D3;
								}
								this.ps.charPos = num + 1;
								if (this.ParsePI())
								{
									break;
								}
								continue;
							}
							else
							{
								this.Throw(num + 1, "Unexpected end tag.");
							}
						}
						else
						{
							num++;
							if (this.ps.charsUsed - num >= 2)
							{
								if (chars[num] == '-')
								{
									if (chars[num + 1] == '-')
									{
										this.ps.charPos = num + 2;
										if (this.ParseComment())
										{
											return true;
										}
										continue;
									}
									else
									{
										this.ThrowUnexpectedToken(num + 1, "-");
									}
								}
								else if (chars[num] == '[')
								{
									if (this.fragmentType != XmlNodeType.Document)
									{
										num++;
										if (this.ps.charsUsed - num >= 6)
										{
											if (XmlConvert.StrEqual(chars, num, 6, "CDATA["))
											{
												goto Block_14;
											}
											this.ThrowUnexpectedToken(num, "CDATA[");
										}
									}
									else
									{
										this.Throw(this.ps.charPos, "Data at the root level is invalid.");
									}
								}
								else if (this.fragmentType == XmlNodeType.Document || this.fragmentType == XmlNodeType.None)
								{
									this.fragmentType = XmlNodeType.Document;
									this.ps.charPos = num;
									if (this.ParseDoctypeDecl())
									{
										return true;
									}
									continue;
								}
								else if (this.ParseUnexpectedToken(num) == "DOCTYPE")
								{
									this.Throw("Unexpected DTD declaration.");
								}
								else
								{
									this.ThrowUnexpectedToken(num, "<!--", "<[CDATA[");
								}
							}
						}
					}
				}
				else if (chars[num] == '&')
				{
					if (this.fragmentType == XmlNodeType.Document)
					{
						this.Throw(num, "Data at the root level is invalid.");
					}
					else
					{
						if (this.fragmentType == XmlNodeType.None)
						{
							this.fragmentType = XmlNodeType.Element;
						}
						int num2;
						XmlTextReaderImpl.EntityType entityType = this.HandleEntityReference(false, XmlTextReaderImpl.EntityExpandType.OnlyGeneral, out num2);
						if (entityType > XmlTextReaderImpl.EntityType.CharacterNamed)
						{
							if (entityType == XmlTextReaderImpl.EntityType.Unexpanded)
							{
								goto Block_26;
							}
							chars = this.ps.chars;
							num = this.ps.charPos;
							continue;
						}
						else
						{
							if (this.ParseText())
							{
								return true;
							}
							continue;
						}
					}
				}
				else if (num != this.ps.charsUsed && ((!this.v1Compat && !flag) || chars[num] != '\0'))
				{
					if (this.fragmentType == XmlNodeType.Document)
					{
						if (this.ParseRootLevelWhitespace())
						{
							return true;
						}
						continue;
					}
					else
					{
						if (this.ParseText())
						{
							goto Block_33;
						}
						continue;
					}
				}
				if (this.ReadData() != 0)
				{
					num = this.ps.charPos;
					num = this.ps.charPos;
					chars = this.ps.chars;
				}
				else
				{
					if (flag2)
					{
						this.Throw("Data at the root level is invalid.");
					}
					if (!this.InEntity)
					{
						goto IL_34B;
					}
					if (this.HandleEntityEnd(true))
					{
						goto Block_39;
					}
				}
			}
			return true;
			Block_14:
			this.ps.charPos = num + 6;
			this.ParseCData();
			if (this.fragmentType == XmlNodeType.None)
			{
				this.fragmentType = XmlNodeType.Element;
			}
			return true;
			IL_1D3:
			if (this.rootElementParsed)
			{
				if (this.fragmentType == XmlNodeType.Document)
				{
					this.Throw(num, "There are multiple root elements.");
				}
				if (this.fragmentType == XmlNodeType.None)
				{
					this.fragmentType = XmlNodeType.Element;
				}
			}
			this.ps.charPos = num;
			this.rootElementParsed = true;
			this.ParseElement();
			return true;
			Block_26:
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.EntityReference)
			{
				this.parsingFunction = this.nextParsingFunction;
			}
			this.ParseEntityReference();
			return true;
			Block_33:
			if (this.fragmentType == XmlNodeType.None && this.curNode.type == XmlNodeType.Text)
			{
				this.fragmentType = XmlNodeType.Element;
			}
			return true;
			Block_39:
			this.SetupEndEntityNodeInContent();
			return true;
			IL_34B:
			if (!this.rootElementParsed && this.fragmentType == XmlNodeType.Document)
			{
				this.ThrowWithoutLineInfo("Root element is missing.");
			}
			if (this.fragmentType == XmlNodeType.None)
			{
				this.fragmentType = (this.rootElementParsed ? XmlNodeType.Document : XmlNodeType.Element);
			}
			this.OnEof();
			return false;
		}

		private bool ParseElementContent()
		{
			int num;
			for (;;)
			{
				num = this.ps.charPos;
				char[] chars = this.ps.chars;
				char c = chars[num];
				if (c != '&')
				{
					if (c == '<')
					{
						char c2 = chars[num + 1];
						if (c2 != '!')
						{
							if (c2 == '/')
							{
								goto IL_13B;
							}
							if (c2 == '?')
							{
								this.ps.charPos = num + 2;
								if (this.ParsePI())
								{
									break;
								}
								continue;
							}
							else if (num + 1 != this.ps.charsUsed)
							{
								goto Block_14;
							}
						}
						else
						{
							num += 2;
							if (this.ps.charsUsed - num >= 2)
							{
								if (chars[num] == '-')
								{
									if (chars[num + 1] == '-')
									{
										this.ps.charPos = num + 2;
										if (this.ParseComment())
										{
											return true;
										}
										continue;
									}
									else
									{
										this.ThrowUnexpectedToken(num + 1, "-");
									}
								}
								else if (chars[num] == '[')
								{
									num++;
									if (this.ps.charsUsed - num >= 6)
									{
										if (XmlConvert.StrEqual(chars, num, 6, "CDATA["))
										{
											goto Block_12;
										}
										this.ThrowUnexpectedToken(num, "CDATA[");
									}
								}
								else if (this.ParseUnexpectedToken(num) == "DOCTYPE")
								{
									this.Throw("Unexpected DTD declaration.");
								}
								else
								{
									this.ThrowUnexpectedToken(num, "<!--", "<[CDATA[");
								}
							}
						}
					}
					else if (num != this.ps.charsUsed)
					{
						if (this.ParseText())
						{
							return true;
						}
						continue;
					}
					if (this.ReadData() == 0)
					{
						if (this.ps.charsUsed - this.ps.charPos != 0)
						{
							this.ThrowUnclosedElements();
						}
						if (!this.InEntity)
						{
							if (this.index == 0 && this.fragmentType != XmlNodeType.Document)
							{
								goto Block_22;
							}
							this.ThrowUnclosedElements();
						}
						if (this.HandleEntityEnd(true))
						{
							goto Block_23;
						}
					}
				}
				else if (this.ParseText())
				{
					return true;
				}
			}
			return true;
			Block_12:
			this.ps.charPos = num + 6;
			this.ParseCData();
			return true;
			IL_13B:
			this.ps.charPos = num + 2;
			this.ParseEndElement();
			return true;
			Block_14:
			this.ps.charPos = num + 1;
			this.ParseElement();
			return true;
			Block_22:
			this.OnEof();
			return false;
			Block_23:
			this.SetupEndEntityNodeInContent();
			return true;
		}

		private void ThrowUnclosedElements()
		{
			if (this.index == 0 && this.curNode.type != XmlNodeType.Element)
			{
				this.Throw(this.ps.charsUsed, "Unexpected end of file has occurred.");
				return;
			}
			int i = (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InIncrementalRead) ? this.index : (this.index - 1);
			this.stringBuilder.Length = 0;
			while (i >= 0)
			{
				XmlTextReaderImpl.NodeData nodeData = this.nodes[i];
				if (nodeData.type == XmlNodeType.Element)
				{
					this.stringBuilder.Append(nodeData.GetNameWPrefix(this.nameTable));
					if (i > 0)
					{
						this.stringBuilder.Append(", ");
					}
					else
					{
						this.stringBuilder.Append(".");
					}
				}
				i--;
			}
			this.Throw(this.ps.charsUsed, "Unexpected end of file has occurred. The following elements are not closed: {0}", this.stringBuilder.ToString());
		}

		private void ParseElement()
		{
			int num = this.ps.charPos;
			char[] chars = this.ps.chars;
			int num2 = -1;
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			while ((this.xmlCharType.charProperties[(int)chars[num]] & 4) != 0)
			{
				num++;
				for (;;)
				{
					if ((this.xmlCharType.charProperties[(int)chars[num]] & 8) != 0)
					{
						num++;
					}
					else
					{
						if (chars[num] != ':')
						{
							goto IL_A2;
						}
						if (num2 == -1)
						{
							break;
						}
						if (this.supportNamespaces)
						{
							goto Block_5;
						}
						num++;
					}
				}
				num2 = num;
				num++;
				continue;
				Block_5:
				this.Throw(num, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(':', '\0'));
				break;
				IL_A2:
				if (num + 1 >= this.ps.charsUsed)
				{
					break;
				}
				IL_C7:
				this.namespaceManager.PushScope();
				if (num2 == -1 || !this.supportNamespaces)
				{
					this.curNode.SetNamedNode(XmlNodeType.Element, this.nameTable.Add(chars, this.ps.charPos, num - this.ps.charPos));
				}
				else
				{
					int charPos = this.ps.charPos;
					int num3 = num2 - charPos;
					if (num3 == this.lastPrefix.Length && XmlConvert.StrEqual(chars, charPos, num3, this.lastPrefix))
					{
						this.curNode.SetNamedNode(XmlNodeType.Element, this.nameTable.Add(chars, num2 + 1, num - num2 - 1), this.lastPrefix, null);
					}
					else
					{
						this.curNode.SetNamedNode(XmlNodeType.Element, this.nameTable.Add(chars, num2 + 1, num - num2 - 1), this.nameTable.Add(chars, this.ps.charPos, num3), null);
						this.lastPrefix = this.curNode.prefix;
					}
				}
				char c = chars[num];
				if ((this.xmlCharType.charProperties[(int)c] & 1) > 0)
				{
					this.ps.charPos = num;
					this.ParseAttributes();
					return;
				}
				if (c == '>')
				{
					this.ps.charPos = num + 1;
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.MoveToElementContent;
				}
				else if (c == '/')
				{
					if (num + 1 == this.ps.charsUsed)
					{
						this.ps.charPos = num;
						if (this.ReadData() == 0)
						{
							this.Throw(num, "Unexpected end of file while parsing {0} has occurred.", ">");
						}
						num = this.ps.charPos;
						chars = this.ps.chars;
					}
					if (chars[num + 1] == '>')
					{
						this.curNode.IsEmptyElement = true;
						this.nextParsingFunction = this.parsingFunction;
						this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PopEmptyElementContext;
						this.ps.charPos = num + 2;
					}
					else
					{
						this.ThrowUnexpectedToken(num, ">");
					}
				}
				else
				{
					this.Throw(num, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(chars, this.ps.charsUsed, num));
				}
				if (this.addDefaultAttributesAndNormalize)
				{
					this.AddDefaultAttributesAndNormalize();
				}
				this.ElementNamespaceLookup();
				return;
			}
			num = this.ParseQName(out num2);
			chars = this.ps.chars;
			goto IL_C7;
		}

		private void AddDefaultAttributesAndNormalize()
		{
			IDtdAttributeListInfo dtdAttributeListInfo = this.dtdInfo.LookupAttributeList(this.curNode.localName, this.curNode.prefix);
			if (dtdAttributeListInfo == null)
			{
				return;
			}
			if (this.normalize && dtdAttributeListInfo.HasNonCDataAttributes)
			{
				for (int i = this.index + 1; i < this.index + 1 + this.attrCount; i++)
				{
					XmlTextReaderImpl.NodeData nodeData = this.nodes[i];
					IDtdAttributeInfo dtdAttributeInfo = dtdAttributeListInfo.LookupAttribute(nodeData.prefix, nodeData.localName);
					if (dtdAttributeInfo != null && dtdAttributeInfo.IsNonCDataType)
					{
						if (this.DtdValidation && this.standalone && dtdAttributeInfo.IsDeclaredInExternal)
						{
							string stringValue = nodeData.StringValue;
							nodeData.TrimSpacesInValue();
							if (stringValue != nodeData.StringValue)
							{
								this.SendValidationEvent(XmlSeverityType.Error, "StandAlone is 'yes' and the value of the attribute '{0}' contains a definition in an external document that changes on normalization.", nodeData.GetNameWPrefix(this.nameTable), nodeData.LineNo, nodeData.LinePos);
							}
						}
						else
						{
							nodeData.TrimSpacesInValue();
						}
					}
				}
			}
			IEnumerable<IDtdDefaultAttributeInfo> enumerable = dtdAttributeListInfo.LookupDefaultAttributes();
			if (enumerable != null)
			{
				int num = this.attrCount;
				XmlTextReaderImpl.NodeData[] array = null;
				if (this.attrCount >= 250)
				{
					array = new XmlTextReaderImpl.NodeData[this.attrCount];
					Array.Copy(this.nodes, this.index + 1, array, 0, this.attrCount);
					object[] array2 = array;
					Array.Sort<object>(array2, XmlTextReaderImpl.DtdDefaultAttributeInfoToNodeDataComparer.Instance);
				}
				foreach (IDtdDefaultAttributeInfo dtdDefaultAttributeInfo in enumerable)
				{
					if (this.AddDefaultAttributeDtd(dtdDefaultAttributeInfo, true, array) && this.DtdValidation && this.standalone && dtdDefaultAttributeInfo.IsDeclaredInExternal)
					{
						string prefix = dtdDefaultAttributeInfo.Prefix;
						string arg = (prefix.Length == 0) ? dtdDefaultAttributeInfo.LocalName : (prefix + ":" + dtdDefaultAttributeInfo.LocalName);
						this.SendValidationEvent(XmlSeverityType.Error, "Markup for unspecified default attribute '{0}' is external and standalone='yes'.", arg, this.curNode.LineNo, this.curNode.LinePos);
					}
				}
				if (num == 0 && this.attrNeedNamespaceLookup)
				{
					this.AttributeNamespaceLookup();
					this.attrNeedNamespaceLookup = false;
				}
			}
		}

		private void ParseEndElement()
		{
			XmlTextReaderImpl.NodeData nodeData = this.nodes[this.index - 1];
			int length = nodeData.prefix.Length;
			int length2 = nodeData.localName.Length;
			while (this.ps.charsUsed - this.ps.charPos < length + length2 + 1 && this.ReadData() != 0)
			{
			}
			char[] chars = this.ps.chars;
			int num;
			if (nodeData.prefix.Length == 0)
			{
				if (!XmlConvert.StrEqual(chars, this.ps.charPos, length2, nodeData.localName))
				{
					this.ThrowTagMismatch(nodeData);
				}
				num = length2;
			}
			else
			{
				int num2 = this.ps.charPos + length;
				if (!XmlConvert.StrEqual(chars, this.ps.charPos, length, nodeData.prefix) || chars[num2] != ':' || !XmlConvert.StrEqual(chars, num2 + 1, length2, nodeData.localName))
				{
					this.ThrowTagMismatch(nodeData);
				}
				num = length2 + length + 1;
			}
			LineInfo lineInfo = new LineInfo(this.ps.lineNo, this.ps.LinePos);
			int num3;
			for (;;)
			{
				num3 = this.ps.charPos + num;
				chars = this.ps.chars;
				if (num3 != this.ps.charsUsed)
				{
					if ((this.xmlCharType.charProperties[(int)chars[num3]] & 8) != 0 || chars[num3] == ':')
					{
						this.ThrowTagMismatch(nodeData);
					}
					if (chars[num3] != '>')
					{
						char c;
						while (this.xmlCharType.IsWhiteSpace(c = chars[num3]))
						{
							num3++;
							if (c != '\n')
							{
								if (c == '\r')
								{
									if (chars[num3] == '\n')
									{
										num3++;
									}
									else if (num3 == this.ps.charsUsed && !this.ps.isEof)
									{
										continue;
									}
									this.OnNewLine(num3);
								}
							}
							else
							{
								this.OnNewLine(num3);
							}
						}
					}
					if (chars[num3] == '>')
					{
						break;
					}
					if (num3 != this.ps.charsUsed)
					{
						this.ThrowUnexpectedToken(num3, ">");
					}
				}
				if (this.ReadData() == 0)
				{
					this.ThrowUnclosedElements();
				}
			}
			this.index--;
			this.curNode = this.nodes[this.index];
			nodeData.lineInfo = lineInfo;
			nodeData.type = XmlNodeType.EndElement;
			this.ps.charPos = num3 + 1;
			this.nextParsingFunction = ((this.index > 0) ? this.parsingFunction : XmlTextReaderImpl.ParsingFunction.DocumentContent);
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PopElementContext;
		}

		private void ThrowTagMismatch(XmlTextReaderImpl.NodeData startTag)
		{
			if (startTag.type == XmlNodeType.Element)
			{
				int num2;
				int num = this.ParseQName(out num2);
				this.Throw("The '{0}' start tag on line {1} position {2} does not match the end tag of '{3}'.", new string[]
				{
					startTag.GetNameWPrefix(this.nameTable),
					startTag.lineInfo.lineNo.ToString(CultureInfo.InvariantCulture),
					startTag.lineInfo.linePos.ToString(CultureInfo.InvariantCulture),
					new string(this.ps.chars, this.ps.charPos, num - this.ps.charPos)
				});
				return;
			}
			this.Throw("Unexpected end tag.");
		}

		private void ParseAttributes()
		{
			int num = this.ps.charPos;
			char[] chars = this.ps.chars;
			for (;;)
			{
				IL_1A:
				int num2 = 0;
				char c;
				while ((this.xmlCharType.charProperties[(int)(c = chars[num])] & 1) != 0)
				{
					if (c == '\n')
					{
						this.OnNewLine(num + 1);
						num2++;
					}
					else if (c == '\r')
					{
						if (chars[num + 1] == '\n')
						{
							this.OnNewLine(num + 2);
							num2++;
							num++;
						}
						else if (num + 1 != this.ps.charsUsed)
						{
							this.OnNewLine(num + 1);
							num2++;
						}
						else
						{
							this.ps.charPos = num;
							IL_42C:
							this.ps.lineNo = this.ps.lineNo - num2;
							if (this.ReadData() != 0)
							{
								num = this.ps.charPos;
								chars = this.ps.chars;
								goto IL_1A;
							}
							this.ThrowUnclosedElements();
							goto IL_1A;
						}
					}
					num++;
				}
				int num3 = 0;
				char c2;
				if ((this.xmlCharType.charProperties[(int)(c2 = chars[num])] & 4) != 0)
				{
					num3 = 1;
				}
				if (num3 == 0)
				{
					if (c2 == '>')
					{
						break;
					}
					if (c2 == '/')
					{
						if (num + 1 == this.ps.charsUsed)
						{
							goto IL_42C;
						}
						if (chars[num + 1] == '>')
						{
							goto Block_11;
						}
						this.ThrowUnexpectedToken(num + 1, ">");
					}
					else
					{
						if (num == this.ps.charsUsed)
						{
							goto IL_42C;
						}
						if (c2 != ':' || this.supportNamespaces)
						{
							this.Throw(num, "Name cannot begin with the '{0}' character, hexadecimal value {1}.", XmlException.BuildCharExceptionArgs(chars, this.ps.charsUsed, num));
						}
					}
				}
				if (num == this.ps.charPos)
				{
					this.ThrowExpectingWhitespace(num);
				}
				this.ps.charPos = num;
				int linePos = this.ps.LinePos;
				int num4 = -1;
				num += num3;
				for (;;)
				{
					char c3;
					if ((this.xmlCharType.charProperties[(int)(c3 = chars[num])] & 8) != 0)
					{
						num++;
					}
					else
					{
						if (c3 != ':')
						{
							goto IL_23E;
						}
						if (num4 != -1)
						{
							if (this.supportNamespaces)
							{
								goto Block_18;
							}
							num++;
						}
						else
						{
							num4 = num;
							num++;
							if ((this.xmlCharType.charProperties[(int)chars[num]] & 4) == 0)
							{
								goto IL_227;
							}
							num++;
						}
					}
				}
				IL_263:
				XmlTextReaderImpl.NodeData nodeData = this.AddAttribute(num, num4);
				nodeData.SetLineInfo(this.ps.LineNo, linePos);
				if (chars[num] != '=')
				{
					this.ps.charPos = num;
					this.EatWhitespaces(null);
					num = this.ps.charPos;
					if (chars[num] != '=')
					{
						this.ThrowUnexpectedToken("=");
					}
				}
				num++;
				char c4 = chars[num];
				if (c4 != '"' && c4 != '\'')
				{
					this.ps.charPos = num;
					this.EatWhitespaces(null);
					num = this.ps.charPos;
					c4 = chars[num];
					if (c4 != '"' && c4 != '\'')
					{
						this.ThrowUnexpectedToken("\"", "'");
					}
				}
				num++;
				this.ps.charPos = num;
				nodeData.quoteChar = c4;
				nodeData.SetLineInfo2(this.ps.LineNo, this.ps.LinePos);
				char c5;
				while ((this.xmlCharType.charProperties[(int)(c5 = chars[num])] & 128) != 0)
				{
					num++;
				}
				if (c5 == c4)
				{
					nodeData.SetValue(chars, this.ps.charPos, num - this.ps.charPos);
					num++;
					this.ps.charPos = num;
				}
				else
				{
					this.ParseAttributeValueSlow(num, c4, nodeData);
					num = this.ps.charPos;
					chars = this.ps.chars;
				}
				if (nodeData.prefix.Length == 0)
				{
					if (Ref.Equal(nodeData.localName, this.XmlNs))
					{
						this.OnDefaultNamespaceDecl(nodeData);
						continue;
					}
					continue;
				}
				else
				{
					if (Ref.Equal(nodeData.prefix, this.XmlNs))
					{
						this.OnNamespaceDecl(nodeData);
						continue;
					}
					if (Ref.Equal(nodeData.prefix, this.Xml))
					{
						this.OnXmlReservedAttribute(nodeData);
						continue;
					}
					continue;
				}
				Block_18:
				this.Throw(num, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(':', '\0'));
				goto IL_263;
				IL_227:
				num = this.ParseQName(out num4);
				chars = this.ps.chars;
				goto IL_263;
				IL_23E:
				if (num + 1 >= this.ps.charsUsed)
				{
					num = this.ParseQName(out num4);
					chars = this.ps.chars;
					goto IL_263;
				}
				goto IL_263;
			}
			this.ps.charPos = num + 1;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.MoveToElementContent;
			goto IL_46C;
			Block_11:
			this.ps.charPos = num + 2;
			this.curNode.IsEmptyElement = true;
			this.nextParsingFunction = this.parsingFunction;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PopEmptyElementContext;
			IL_46C:
			if (this.addDefaultAttributesAndNormalize)
			{
				this.AddDefaultAttributesAndNormalize();
			}
			this.ElementNamespaceLookup();
			if (this.attrNeedNamespaceLookup)
			{
				this.AttributeNamespaceLookup();
				this.attrNeedNamespaceLookup = false;
			}
			if (this.attrDuplWalkCount >= 250)
			{
				this.AttributeDuplCheck();
			}
		}

		private void ElementNamespaceLookup()
		{
			if (this.curNode.prefix.Length == 0)
			{
				this.curNode.ns = this.xmlContext.defaultNamespace;
				return;
			}
			this.curNode.ns = this.LookupNamespace(this.curNode);
		}

		private void AttributeNamespaceLookup()
		{
			for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
			{
				XmlTextReaderImpl.NodeData nodeData = this.nodes[i];
				if (nodeData.type == XmlNodeType.Attribute && nodeData.prefix.Length > 0)
				{
					nodeData.ns = this.LookupNamespace(nodeData);
				}
			}
		}

		private void AttributeDuplCheck()
		{
			if (this.attrCount < 250)
			{
				for (int i = this.index + 1; i < this.index + 1 + this.attrCount; i++)
				{
					XmlTextReaderImpl.NodeData nodeData = this.nodes[i];
					for (int j = i + 1; j < this.index + 1 + this.attrCount; j++)
					{
						if (Ref.Equal(nodeData.localName, this.nodes[j].localName) && Ref.Equal(nodeData.ns, this.nodes[j].ns))
						{
							this.Throw("'{0}' is a duplicate attribute name.", this.nodes[j].GetNameWPrefix(this.nameTable), this.nodes[j].LineNo, this.nodes[j].LinePos);
						}
					}
				}
				return;
			}
			if (this.attrDuplSortingArray == null || this.attrDuplSortingArray.Length < this.attrCount)
			{
				this.attrDuplSortingArray = new XmlTextReaderImpl.NodeData[this.attrCount];
			}
			Array.Copy(this.nodes, this.index + 1, this.attrDuplSortingArray, 0, this.attrCount);
			Array.Sort<XmlTextReaderImpl.NodeData>(this.attrDuplSortingArray, 0, this.attrCount);
			XmlTextReaderImpl.NodeData nodeData2 = this.attrDuplSortingArray[0];
			for (int k = 1; k < this.attrCount; k++)
			{
				XmlTextReaderImpl.NodeData nodeData3 = this.attrDuplSortingArray[k];
				if (Ref.Equal(nodeData2.localName, nodeData3.localName) && Ref.Equal(nodeData2.ns, nodeData3.ns))
				{
					this.Throw("'{0}' is a duplicate attribute name.", nodeData3.GetNameWPrefix(this.nameTable), nodeData3.LineNo, nodeData3.LinePos);
				}
				nodeData2 = nodeData3;
			}
		}

		private void OnDefaultNamespaceDecl(XmlTextReaderImpl.NodeData attr)
		{
			if (!this.supportNamespaces)
			{
				return;
			}
			string text = this.nameTable.Add(attr.StringValue);
			attr.ns = this.nameTable.Add("http://www.w3.org/2000/xmlns/");
			if (!this.curNode.xmlContextPushed)
			{
				this.PushXmlContext();
			}
			this.xmlContext.defaultNamespace = text;
			this.AddNamespace(string.Empty, text, attr);
		}

		private void OnNamespaceDecl(XmlTextReaderImpl.NodeData attr)
		{
			if (!this.supportNamespaces)
			{
				return;
			}
			string text = this.nameTable.Add(attr.StringValue);
			if (text.Length == 0)
			{
				this.Throw("Invalid namespace declaration.", attr.lineInfo2.lineNo, attr.lineInfo2.linePos - 1);
			}
			this.AddNamespace(attr.localName, text, attr);
		}

		private void OnXmlReservedAttribute(XmlTextReaderImpl.NodeData attr)
		{
			string localName = attr.localName;
			if (!(localName == "space"))
			{
				if (!(localName == "lang"))
				{
					return;
				}
				if (!this.curNode.xmlContextPushed)
				{
					this.PushXmlContext();
				}
				this.xmlContext.xmlLang = attr.StringValue;
				return;
			}
			else
			{
				if (!this.curNode.xmlContextPushed)
				{
					this.PushXmlContext();
				}
				string a = XmlConvert.TrimString(attr.StringValue);
				if (a == "preserve")
				{
					this.xmlContext.xmlSpace = XmlSpace.Preserve;
					return;
				}
				if (!(a == "default"))
				{
					this.Throw("'{0}' is an invalid xml:space value.", attr.StringValue, attr.lineInfo.lineNo, attr.lineInfo.linePos);
					return;
				}
				this.xmlContext.xmlSpace = XmlSpace.Default;
				return;
			}
		}

		private void ParseAttributeValueSlow(int curPos, char quoteChar, XmlTextReaderImpl.NodeData attr)
		{
			int num = curPos;
			char[] chars = this.ps.chars;
			int entityId = this.ps.entityId;
			int num2 = 0;
			LineInfo lineInfo = new LineInfo(this.ps.lineNo, this.ps.LinePos);
			XmlTextReaderImpl.NodeData nodeData = null;
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)chars[num]] & 128) == 0)
				{
					if (num - this.ps.charPos > 0)
					{
						this.stringBuilder.Append(chars, this.ps.charPos, num - this.ps.charPos);
						this.ps.charPos = num;
					}
					if (chars[num] == quoteChar && entityId == this.ps.entityId)
					{
						goto IL_63F;
					}
					char c = chars[num];
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
							num++;
							if (this.normalize)
							{
								this.stringBuilder.Append(' ');
								this.ps.charPos = this.ps.charPos + 1;
								continue;
							}
							continue;
						case '\n':
							num++;
							this.OnNewLine(num);
							if (this.normalize)
							{
								this.stringBuilder.Append(' ');
								this.ps.charPos = this.ps.charPos + 1;
								continue;
							}
							continue;
						case '\v':
						case '\f':
							goto IL_4F8;
						case '\r':
							if (chars[num + 1] == '\n')
							{
								num += 2;
								if (this.normalize)
								{
									this.stringBuilder.Append(this.ps.eolNormalized ? "  " : " ");
									this.ps.charPos = num;
								}
							}
							else
							{
								if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_54A;
								}
								num++;
								if (this.normalize)
								{
									this.stringBuilder.Append(' ');
									this.ps.charPos = num;
								}
							}
							this.OnNewLine(num);
							continue;
						default:
							if (c != '"')
							{
								if (c != '&')
								{
									goto IL_4F8;
								}
								if (num - this.ps.charPos > 0)
								{
									this.stringBuilder.Append(chars, this.ps.charPos, num - this.ps.charPos);
								}
								this.ps.charPos = num;
								int entityId2 = this.ps.entityId;
								LineInfo lineInfo2 = new LineInfo(this.ps.lineNo, this.ps.LinePos + 1);
								switch (this.HandleEntityReference(true, XmlTextReaderImpl.EntityExpandType.All, out num))
								{
								case XmlTextReaderImpl.EntityType.CharacterDec:
								case XmlTextReaderImpl.EntityType.CharacterHex:
								case XmlTextReaderImpl.EntityType.CharacterNamed:
									break;
								case XmlTextReaderImpl.EntityType.Expanded:
								case XmlTextReaderImpl.EntityType.Skipped:
								case XmlTextReaderImpl.EntityType.FakeExpanded:
									goto IL_4DB;
								case XmlTextReaderImpl.EntityType.Unexpanded:
									if (this.parsingMode == XmlTextReaderImpl.ParsingMode.Full && this.ps.entityId == entityId)
									{
										int num3 = this.stringBuilder.Length - num2;
										if (num3 > 0)
										{
											XmlTextReaderImpl.NodeData nodeData2 = new XmlTextReaderImpl.NodeData();
											nodeData2.lineInfo = lineInfo;
											nodeData2.depth = attr.depth + 1;
											nodeData2.SetValueNode(XmlNodeType.Text, this.stringBuilder.ToString(num2, num3));
											this.AddAttributeChunkToList(attr, nodeData2, ref nodeData);
										}
										this.ps.charPos = this.ps.charPos + 1;
										string text = this.ParseEntityName();
										XmlTextReaderImpl.NodeData nodeData3 = new XmlTextReaderImpl.NodeData();
										nodeData3.lineInfo = lineInfo2;
										nodeData3.depth = attr.depth + 1;
										nodeData3.SetNamedNode(XmlNodeType.EntityReference, text);
										this.AddAttributeChunkToList(attr, nodeData3, ref nodeData);
										this.stringBuilder.Append('&');
										this.stringBuilder.Append(text);
										this.stringBuilder.Append(';');
										num2 = this.stringBuilder.Length;
										lineInfo.Set(this.ps.LineNo, this.ps.LinePos);
										this.fullAttrCleanup = true;
									}
									else
									{
										this.ps.charPos = this.ps.charPos + 1;
										this.ParseEntityName();
									}
									num = this.ps.charPos;
									break;
								case XmlTextReaderImpl.EntityType.ExpandedInAttribute:
									if (this.parsingMode == XmlTextReaderImpl.ParsingMode.Full && entityId2 == entityId)
									{
										int num4 = this.stringBuilder.Length - num2;
										if (num4 > 0)
										{
											XmlTextReaderImpl.NodeData nodeData4 = new XmlTextReaderImpl.NodeData();
											nodeData4.lineInfo = lineInfo;
											nodeData4.depth = attr.depth + 1;
											nodeData4.SetValueNode(XmlNodeType.Text, this.stringBuilder.ToString(num2, num4));
											this.AddAttributeChunkToList(attr, nodeData4, ref nodeData);
										}
										XmlTextReaderImpl.NodeData nodeData5 = new XmlTextReaderImpl.NodeData();
										nodeData5.lineInfo = lineInfo2;
										nodeData5.depth = attr.depth + 1;
										nodeData5.SetNamedNode(XmlNodeType.EntityReference, this.ps.entity.Name);
										this.AddAttributeChunkToList(attr, nodeData5, ref nodeData);
										this.fullAttrCleanup = true;
									}
									num = this.ps.charPos;
									break;
								default:
									goto IL_4DB;
								}
								IL_4E7:
								chars = this.ps.chars;
								continue;
								IL_4DB:
								num = this.ps.charPos;
								goto IL_4E7;
							}
							break;
						}
					}
					else if (c != '\'')
					{
						if (c == '<')
						{
							this.Throw(num, "'{0}', hexadecimal value {1}, is an invalid attribute character.", XmlException.BuildCharExceptionArgs('<', '\0'));
							goto IL_54A;
						}
						if (c != '>')
						{
							goto IL_4F8;
						}
					}
					num++;
					continue;
					IL_4F8:
					if (num != this.ps.charsUsed)
					{
						if (XmlCharType.IsHighSurrogate((int)chars[num]))
						{
							if (num + 1 == this.ps.charsUsed)
							{
								goto IL_54A;
							}
							num++;
							if (XmlCharType.IsLowSurrogate((int)chars[num]))
							{
								num++;
								continue;
							}
						}
						this.ThrowInvalidChar(chars, this.ps.charsUsed, num);
					}
					IL_54A:
					if (this.ReadData() == 0)
					{
						if (this.ps.charsUsed - this.ps.charPos > 0)
						{
							if (this.ps.chars[this.ps.charPos] != '\r')
							{
								this.Throw("Unexpected end of file has occurred.");
							}
						}
						else
						{
							if (!this.InEntity)
							{
								if (this.fragmentType == XmlNodeType.Attribute)
								{
									break;
								}
								this.Throw("There is an unclosed literal string.");
							}
							if (this.HandleEntityEnd(true))
							{
								this.Throw("An internal error has occurred.");
							}
							if (entityId == this.ps.entityId)
							{
								num2 = this.stringBuilder.Length;
								lineInfo.Set(this.ps.LineNo, this.ps.LinePos);
							}
						}
					}
					num = this.ps.charPos;
					chars = this.ps.chars;
				}
				else
				{
					num++;
				}
			}
			if (entityId != this.ps.entityId)
			{
				this.Throw("Entity replacement text must nest properly within markup declarations.");
			}
			IL_63F:
			if (attr.nextAttrValueChunk != null)
			{
				int num5 = this.stringBuilder.Length - num2;
				if (num5 > 0)
				{
					XmlTextReaderImpl.NodeData nodeData6 = new XmlTextReaderImpl.NodeData();
					nodeData6.lineInfo = lineInfo;
					nodeData6.depth = attr.depth + 1;
					nodeData6.SetValueNode(XmlNodeType.Text, this.stringBuilder.ToString(num2, num5));
					this.AddAttributeChunkToList(attr, nodeData6, ref nodeData);
				}
			}
			this.ps.charPos = num + 1;
			attr.SetValue(this.stringBuilder.ToString());
			this.stringBuilder.Length = 0;
		}

		private void AddAttributeChunkToList(XmlTextReaderImpl.NodeData attr, XmlTextReaderImpl.NodeData chunk, ref XmlTextReaderImpl.NodeData lastChunk)
		{
			if (lastChunk == null)
			{
				lastChunk = chunk;
				attr.nextAttrValueChunk = chunk;
				return;
			}
			lastChunk.nextAttrValueChunk = chunk;
			lastChunk = chunk;
		}

		private bool ParseText()
		{
			int num = 0;
			if (this.parsingMode != XmlTextReaderImpl.ParsingMode.Full)
			{
				int num2;
				int num3;
				while (!this.ParseText(out num2, out num3, ref num))
				{
				}
			}
			else
			{
				this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
				int num2;
				int num3;
				if (this.ParseText(out num2, out num3, ref num))
				{
					if (num3 - num2 != 0)
					{
						XmlNodeType textNodeType = this.GetTextNodeType(num);
						if (textNodeType != XmlNodeType.None)
						{
							this.curNode.SetValueNode(textNodeType, this.ps.chars, num2, num3 - num2);
							return true;
						}
					}
				}
				else if (this.v1Compat)
				{
					do
					{
						if (num3 - num2 > 0)
						{
							this.stringBuilder.Append(this.ps.chars, num2, num3 - num2);
						}
					}
					while (!this.ParseText(out num2, out num3, ref num));
					if (num3 - num2 > 0)
					{
						this.stringBuilder.Append(this.ps.chars, num2, num3 - num2);
					}
					XmlNodeType textNodeType2 = this.GetTextNodeType(num);
					if (textNodeType2 != XmlNodeType.None)
					{
						this.curNode.SetValueNode(textNodeType2, this.stringBuilder.ToString());
						this.stringBuilder.Length = 0;
						return true;
					}
					this.stringBuilder.Length = 0;
				}
				else
				{
					if (num > 32)
					{
						this.curNode.SetValueNode(XmlNodeType.Text, this.ps.chars, num2, num3 - num2);
						this.nextParsingFunction = this.parsingFunction;
						this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PartialTextValue;
						return true;
					}
					if (num3 - num2 > 0)
					{
						this.stringBuilder.Append(this.ps.chars, num2, num3 - num2);
					}
					bool flag;
					do
					{
						flag = this.ParseText(out num2, out num3, ref num);
						if (num3 - num2 > 0)
						{
							this.stringBuilder.Append(this.ps.chars, num2, num3 - num2);
						}
					}
					while (!flag && num <= 32 && this.stringBuilder.Length < 4096);
					XmlNodeType xmlNodeType = (this.stringBuilder.Length < 4096) ? this.GetTextNodeType(num) : XmlNodeType.Text;
					if (xmlNodeType != XmlNodeType.None)
					{
						this.curNode.SetValueNode(xmlNodeType, this.stringBuilder.ToString());
						this.stringBuilder.Length = 0;
						if (!flag)
						{
							this.nextParsingFunction = this.parsingFunction;
							this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PartialTextValue;
						}
						return true;
					}
					this.stringBuilder.Length = 0;
					if (!flag)
					{
						while (!this.ParseText(out num2, out num3, ref num))
						{
						}
					}
				}
			}
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.ReportEndEntity)
			{
				this.SetupEndEntityNodeInContent();
				this.parsingFunction = this.nextParsingFunction;
				return true;
			}
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.EntityReference)
			{
				this.parsingFunction = this.nextNextParsingFunction;
				this.ParseEntityReference();
				return true;
			}
			return false;
		}

		private bool ParseText(out int startPos, out int endPos, ref int outOrChars)
		{
			char[] chars = this.ps.chars;
			int num = this.ps.charPos;
			int num2 = 0;
			int num3 = -1;
			int num4 = outOrChars;
			char c;
			int num7;
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)(c = chars[num])] & 64) == 0)
				{
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
							num++;
							continue;
						case '\n':
							num++;
							this.OnNewLine(num);
							continue;
						case '\v':
						case '\f':
							break;
						case '\r':
							if (chars[num + 1] == '\n')
							{
								if (!this.ps.eolNormalized && this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
								{
									if (num - this.ps.charPos > 0)
									{
										if (num2 == 0)
										{
											num2 = 1;
											num3 = num;
										}
										else
										{
											this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
											num3 = num - num2;
											num2++;
										}
									}
									else
									{
										this.ps.charPos = this.ps.charPos + 1;
									}
								}
								num += 2;
							}
							else
							{
								if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_366;
								}
								if (!this.ps.eolNormalized)
								{
									chars[num] = '\n';
								}
								num++;
							}
							this.OnNewLine(num);
							continue;
						default:
							if (c == '&')
							{
								int num6;
								XmlTextReaderImpl.EntityType entityType;
								int num5;
								if ((num5 = this.ParseCharRefInline(num, out num6, out entityType)) > 0)
								{
									if (num2 > 0)
									{
										this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
									}
									num3 = num - num2;
									num2 += num5 - num - num6;
									num = num5;
									if (!this.xmlCharType.IsWhiteSpace(chars[num5 - num6]) || (this.v1Compat && entityType == XmlTextReaderImpl.EntityType.CharacterDec))
									{
										num4 |= 255;
										continue;
									}
									continue;
								}
								else
								{
									if (num > this.ps.charPos)
									{
										goto IL_42F;
									}
									switch (this.HandleEntityReference(false, XmlTextReaderImpl.EntityExpandType.All, out num))
									{
									case XmlTextReaderImpl.EntityType.CharacterDec:
										if (!this.v1Compat)
										{
											goto IL_221;
										}
										num4 |= 255;
										break;
									case XmlTextReaderImpl.EntityType.CharacterHex:
									case XmlTextReaderImpl.EntityType.CharacterNamed:
										goto IL_221;
									case XmlTextReaderImpl.EntityType.Expanded:
									case XmlTextReaderImpl.EntityType.Skipped:
									case XmlTextReaderImpl.EntityType.FakeExpanded:
										goto IL_249;
									case XmlTextReaderImpl.EntityType.Unexpanded:
										goto IL_1F4;
									default:
										goto IL_249;
									}
									IL_255:
									chars = this.ps.chars;
									continue;
									IL_249:
									num = this.ps.charPos;
									goto IL_255;
									IL_221:
									if (!this.xmlCharType.IsWhiteSpace(this.ps.chars[num - 1]))
									{
										num4 |= 255;
										goto IL_255;
									}
									goto IL_255;
								}
							}
							break;
						}
					}
					else
					{
						if (c == '<')
						{
							goto IL_42F;
						}
						if (c == ']')
						{
							if (this.ps.charsUsed - num >= 3 || this.ps.isEof)
							{
								if (chars[num + 1] == ']' && chars[num + 2] == '>')
								{
									this.Throw(num, "']]>' is not allowed in character data.");
								}
								num4 |= 93;
								num++;
								continue;
							}
							goto IL_366;
						}
					}
					if (num != this.ps.charsUsed)
					{
						char c2 = chars[num];
						if (XmlCharType.IsHighSurrogate((int)c2))
						{
							if (num + 1 == this.ps.charsUsed)
							{
								goto IL_366;
							}
							num++;
							if (XmlCharType.IsLowSurrogate((int)chars[num]))
							{
								num++;
								num4 |= (int)c2;
								continue;
							}
						}
						num7 = num - this.ps.charPos;
						if (this.ZeroEndingStream(num))
						{
							goto Block_29;
						}
						this.ThrowInvalidChar(this.ps.chars, this.ps.charsUsed, this.ps.charPos + num7);
					}
					IL_366:
					if (num > this.ps.charPos)
					{
						goto IL_42F;
					}
					if (this.ReadData() == 0)
					{
						if (this.ps.charsUsed - this.ps.charPos > 0)
						{
							if (this.ps.chars[this.ps.charPos] != '\r' && this.ps.chars[this.ps.charPos] != ']')
							{
								this.Throw("Unexpected end of file has occurred.");
							}
						}
						else
						{
							if (!this.InEntity)
							{
								goto IL_423;
							}
							if (this.HandleEntityEnd(true))
							{
								goto Block_36;
							}
						}
					}
					num = this.ps.charPos;
					chars = this.ps.chars;
				}
				else
				{
					num4 |= (int)c;
					num++;
				}
			}
			IL_1F4:
			this.nextParsingFunction = this.parsingFunction;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.EntityReference;
			goto IL_423;
			Block_29:
			chars = this.ps.chars;
			num = this.ps.charPos + num7;
			goto IL_42F;
			Block_36:
			this.nextParsingFunction = this.parsingFunction;
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.ReportEndEntity;
			IL_423:
			startPos = (endPos = num);
			return true;
			IL_42F:
			if (this.parsingMode == XmlTextReaderImpl.ParsingMode.Full && num2 > 0)
			{
				this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
			}
			startPos = this.ps.charPos;
			endPos = num - num2;
			this.ps.charPos = num;
			outOrChars = num4;
			return c == '<';
		}

		private void FinishPartialValue()
		{
			this.curNode.CopyTo(this.readValueOffset, this.stringBuilder);
			int num = 0;
			int num2;
			int num3;
			while (!this.ParseText(out num2, out num3, ref num))
			{
				this.stringBuilder.Append(this.ps.chars, num2, num3 - num2);
			}
			this.stringBuilder.Append(this.ps.chars, num2, num3 - num2);
			this.curNode.SetValue(this.stringBuilder.ToString());
			this.stringBuilder.Length = 0;
		}

		private void FinishOtherValueIterator()
		{
			switch (this.parsingFunction)
			{
			case XmlTextReaderImpl.ParsingFunction.InReadAttributeValue:
				break;
			case XmlTextReaderImpl.ParsingFunction.InReadValueChunk:
				if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnPartialValue)
				{
					this.FinishPartialValue();
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnCachedValue;
					return;
				}
				if (this.readValueOffset > 0)
				{
					this.curNode.SetValue(this.curNode.StringValue.Substring(this.readValueOffset));
					this.readValueOffset = 0;
					return;
				}
				break;
			case XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary:
			case XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary:
				switch (this.incReadState)
				{
				case XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnCachedValue:
					if (this.readValueOffset > 0)
					{
						this.curNode.SetValue(this.curNode.StringValue.Substring(this.readValueOffset));
						this.readValueOffset = 0;
						return;
					}
					break;
				case XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnPartialValue:
					this.FinishPartialValue();
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnCachedValue;
					return;
				case XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_End:
					this.curNode.SetValue(string.Empty);
					break;
				default:
					return;
				}
				break;
			default:
				return;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SkipPartialTextValue()
		{
			int num = 0;
			this.parsingFunction = this.nextParsingFunction;
			int num2;
			int num3;
			while (!this.ParseText(out num2, out num3, ref num))
			{
			}
		}

		private void FinishReadValueChunk()
		{
			this.readValueOffset = 0;
			if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnPartialValue)
			{
				this.SkipPartialTextValue();
				return;
			}
			this.parsingFunction = this.nextParsingFunction;
			this.nextParsingFunction = this.nextNextParsingFunction;
		}

		private void FinishReadContentAsBinary()
		{
			this.readValueOffset = 0;
			if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnPartialValue)
			{
				this.SkipPartialTextValue();
			}
			else
			{
				this.parsingFunction = this.nextParsingFunction;
				this.nextParsingFunction = this.nextNextParsingFunction;
			}
			if (this.incReadState != XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_End)
			{
				while (this.MoveToNextContentNode(true))
				{
				}
			}
		}

		private void FinishReadElementContentAsBinary()
		{
			this.FinishReadContentAsBinary();
			if (this.curNode.type != XmlNodeType.EndElement)
			{
				this.Throw("'{0}' is an invalid XmlNodeType.", this.curNode.type.ToString());
			}
			this.outerReader.Read();
		}

		private bool ParseRootLevelWhitespace()
		{
			XmlNodeType whitespaceType = this.GetWhitespaceType();
			if (whitespaceType == XmlNodeType.None)
			{
				this.EatWhitespaces(null);
				if (this.ps.chars[this.ps.charPos] == '<' || this.ps.charsUsed - this.ps.charPos == 0 || this.ZeroEndingStream(this.ps.charPos))
				{
					return false;
				}
			}
			else
			{
				this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
				this.EatWhitespaces(this.stringBuilder);
				if (this.ps.chars[this.ps.charPos] == '<' || this.ps.charsUsed - this.ps.charPos == 0 || this.ZeroEndingStream(this.ps.charPos))
				{
					if (this.stringBuilder.Length > 0)
					{
						this.curNode.SetValueNode(whitespaceType, this.stringBuilder.ToString());
						this.stringBuilder.Length = 0;
						return true;
					}
					return false;
				}
			}
			if (this.xmlCharType.IsCharData(this.ps.chars[this.ps.charPos]))
			{
				this.Throw("Data at the root level is invalid.");
			}
			else
			{
				this.ThrowInvalidChar(this.ps.chars, this.ps.charsUsed, this.ps.charPos);
			}
			return false;
		}

		private void ParseEntityReference()
		{
			this.ps.charPos = this.ps.charPos + 1;
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			this.curNode.SetNamedNode(XmlNodeType.EntityReference, this.ParseEntityName());
		}

		private XmlTextReaderImpl.EntityType HandleEntityReference(bool isInAttributeValue, XmlTextReaderImpl.EntityExpandType expandType, out int charRefEndPos)
		{
			if (this.ps.charPos + 1 == this.ps.charsUsed && this.ReadData() == 0)
			{
				this.Throw("Unexpected end of file has occurred.");
			}
			if (this.ps.chars[this.ps.charPos + 1] == '#')
			{
				XmlTextReaderImpl.EntityType result;
				charRefEndPos = this.ParseNumericCharRef(expandType != XmlTextReaderImpl.EntityExpandType.OnlyGeneral, null, out result);
				return result;
			}
			charRefEndPos = this.ParseNamedCharRef(expandType != XmlTextReaderImpl.EntityExpandType.OnlyGeneral, null);
			if (charRefEndPos >= 0)
			{
				return XmlTextReaderImpl.EntityType.CharacterNamed;
			}
			if (expandType == XmlTextReaderImpl.EntityExpandType.OnlyCharacter || (this.entityHandling != EntityHandling.ExpandEntities && (!isInAttributeValue || !this.validatingReaderCompatFlag)))
			{
				return XmlTextReaderImpl.EntityType.Unexpanded;
			}
			this.ps.charPos = this.ps.charPos + 1;
			int linePos = this.ps.LinePos;
			int num;
			try
			{
				num = this.ParseName();
			}
			catch (XmlException)
			{
				this.Throw("An error occurred while parsing EntityName.", this.ps.LineNo, linePos);
				return XmlTextReaderImpl.EntityType.Skipped;
			}
			if (this.ps.chars[num] != ';')
			{
				this.ThrowUnexpectedToken(num, ";");
			}
			int linePos2 = this.ps.LinePos;
			string name = this.nameTable.Add(this.ps.chars, this.ps.charPos, num - this.ps.charPos);
			this.ps.charPos = num + 1;
			charRefEndPos = -1;
			XmlTextReaderImpl.EntityType result2 = this.HandleGeneralEntityReference(name, isInAttributeValue, false, linePos2);
			this.reportedBaseUri = this.ps.baseUriStr;
			this.reportedEncoding = this.ps.encoding;
			return result2;
		}

		private XmlTextReaderImpl.EntityType HandleGeneralEntityReference(string name, bool isInAttributeValue, bool pushFakeEntityIfNullResolver, int entityStartLinePos)
		{
			IDtdEntityInfo dtdEntityInfo = null;
			if (this.dtdInfo == null && this.fragmentParserContext != null && this.fragmentParserContext.HasDtdInfo && this.dtdProcessing == DtdProcessing.Parse)
			{
				this.ParseDtdFromParserContext();
			}
			if (this.dtdInfo == null || (dtdEntityInfo = this.dtdInfo.LookupEntity(name)) == null)
			{
				if (this.disableUndeclaredEntityCheck)
				{
					dtdEntityInfo = new SchemaEntity(new XmlQualifiedName(name), false)
					{
						Text = string.Empty
					};
				}
				else
				{
					this.Throw("Reference to undeclared entity '{0}'.", name, this.ps.LineNo, entityStartLinePos);
				}
			}
			if (dtdEntityInfo.IsUnparsedEntity)
			{
				if (this.disableUndeclaredEntityCheck)
				{
					dtdEntityInfo = new SchemaEntity(new XmlQualifiedName(name), false)
					{
						Text = string.Empty
					};
				}
				else
				{
					this.Throw("Reference to unparsed entity '{0}'.", name, this.ps.LineNo, entityStartLinePos);
				}
			}
			if (this.standalone && dtdEntityInfo.IsDeclaredInExternal)
			{
				this.Throw("Standalone document declaration must have a value of 'no' because an external entity '{0}' is referenced.", dtdEntityInfo.Name, this.ps.LineNo, entityStartLinePos);
			}
			if (dtdEntityInfo.IsExternal)
			{
				if (isInAttributeValue)
				{
					this.Throw("External entity '{0}' reference cannot appear in the attribute value.", name, this.ps.LineNo, entityStartLinePos);
					return XmlTextReaderImpl.EntityType.Skipped;
				}
				if (this.parsingMode == XmlTextReaderImpl.ParsingMode.SkipContent)
				{
					return XmlTextReaderImpl.EntityType.Skipped;
				}
				if (this.IsResolverNull)
				{
					if (pushFakeEntityIfNullResolver)
					{
						this.PushExternalEntity(dtdEntityInfo);
						this.curNode.entityId = this.ps.entityId;
						return XmlTextReaderImpl.EntityType.FakeExpanded;
					}
					return XmlTextReaderImpl.EntityType.Skipped;
				}
				else
				{
					this.PushExternalEntity(dtdEntityInfo);
					this.curNode.entityId = this.ps.entityId;
					if (!isInAttributeValue || !this.validatingReaderCompatFlag)
					{
						return XmlTextReaderImpl.EntityType.Expanded;
					}
					return XmlTextReaderImpl.EntityType.ExpandedInAttribute;
				}
			}
			else
			{
				if (this.parsingMode == XmlTextReaderImpl.ParsingMode.SkipContent)
				{
					return XmlTextReaderImpl.EntityType.Skipped;
				}
				this.PushInternalEntity(dtdEntityInfo);
				this.curNode.entityId = this.ps.entityId;
				if (!isInAttributeValue || !this.validatingReaderCompatFlag)
				{
					return XmlTextReaderImpl.EntityType.Expanded;
				}
				return XmlTextReaderImpl.EntityType.ExpandedInAttribute;
			}
		}

		private bool InEntity
		{
			get
			{
				return this.parsingStatesStackTop >= 0;
			}
		}

		private bool HandleEntityEnd(bool checkEntityNesting)
		{
			if (this.parsingStatesStackTop == -1)
			{
				this.Throw("An internal error has occurred.");
			}
			if (this.ps.entityResolvedManually)
			{
				this.index--;
				if (checkEntityNesting && this.ps.entityId != this.nodes[this.index].entityId)
				{
					this.Throw("Incomplete entity contents.");
				}
				this.lastEntity = this.ps.entity;
				this.PopEntity();
				return true;
			}
			if (checkEntityNesting && this.ps.entityId != this.nodes[this.index].entityId)
			{
				this.Throw("Incomplete entity contents.");
			}
			this.PopEntity();
			this.reportedEncoding = this.ps.encoding;
			this.reportedBaseUri = this.ps.baseUriStr;
			return false;
		}

		private void SetupEndEntityNodeInContent()
		{
			this.reportedEncoding = this.ps.encoding;
			this.reportedBaseUri = this.ps.baseUriStr;
			this.curNode = this.nodes[this.index];
			this.curNode.SetNamedNode(XmlNodeType.EndEntity, this.lastEntity.Name);
			this.curNode.lineInfo.Set(this.ps.lineNo, this.ps.LinePos - 1);
			if (this.index == 0 && this.parsingFunction == XmlTextReaderImpl.ParsingFunction.ElementContent)
			{
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.DocumentContent;
			}
		}

		private void SetupEndEntityNodeInAttribute()
		{
			this.curNode = this.nodes[this.index + this.attrCount + 1];
			XmlTextReaderImpl.NodeData nodeData = this.curNode;
			nodeData.lineInfo.linePos = nodeData.lineInfo.linePos + this.curNode.localName.Length;
			this.curNode.type = XmlNodeType.EndEntity;
		}

		private bool ParsePI()
		{
			return this.ParsePI(null);
		}

		private bool ParsePI(StringBuilder piInDtdStringBuilder)
		{
			if (this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
			{
				this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			}
			int num = this.ParseName();
			string text = this.nameTable.Add(this.ps.chars, this.ps.charPos, num - this.ps.charPos);
			if (string.Compare(text, "xml", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.Throw(text.Equals("xml") ? "Unexpected XML declaration. The XML declaration must be the first node in the document, and no white space characters are allowed to appear before it." : "'{0}' is an invalid name for processing instructions.", text);
			}
			this.ps.charPos = num;
			if (piInDtdStringBuilder == null)
			{
				if (!this.ignorePIs && this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
				{
					this.curNode.SetNamedNode(XmlNodeType.ProcessingInstruction, text);
				}
			}
			else
			{
				piInDtdStringBuilder.Append(text);
			}
			char c = this.ps.chars[this.ps.charPos];
			if (this.EatWhitespaces(piInDtdStringBuilder) == 0)
			{
				if (this.ps.charsUsed - this.ps.charPos < 2)
				{
					this.ReadData();
				}
				if (c != '?' || this.ps.chars[this.ps.charPos + 1] != '>')
				{
					this.Throw("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(this.ps.chars, this.ps.charsUsed, this.ps.charPos));
				}
			}
			int num2;
			int num3;
			if (this.ParsePIValue(out num2, out num3))
			{
				if (piInDtdStringBuilder == null)
				{
					if (this.ignorePIs)
					{
						return false;
					}
					if (this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
					{
						this.curNode.SetValue(this.ps.chars, num2, num3 - num2);
					}
				}
				else
				{
					piInDtdStringBuilder.Append(this.ps.chars, num2, num3 - num2);
				}
			}
			else
			{
				StringBuilder stringBuilder;
				if (piInDtdStringBuilder == null)
				{
					if (this.ignorePIs || this.parsingMode != XmlTextReaderImpl.ParsingMode.Full)
					{
						while (!this.ParsePIValue(out num2, out num3))
						{
						}
						return false;
					}
					stringBuilder = this.stringBuilder;
				}
				else
				{
					stringBuilder = piInDtdStringBuilder;
				}
				do
				{
					stringBuilder.Append(this.ps.chars, num2, num3 - num2);
				}
				while (!this.ParsePIValue(out num2, out num3));
				stringBuilder.Append(this.ps.chars, num2, num3 - num2);
				if (piInDtdStringBuilder == null)
				{
					this.curNode.SetValue(this.stringBuilder.ToString());
					this.stringBuilder.Length = 0;
				}
			}
			return true;
		}

		private bool ParsePIValue(out int outStartPos, out int outEndPos)
		{
			if (this.ps.charsUsed - this.ps.charPos < 2 && this.ReadData() == 0)
			{
				this.Throw(this.ps.charsUsed, "Unexpected end of file while parsing {0} has occurred.", "PI");
			}
			int num = this.ps.charPos;
			char[] chars = this.ps.chars;
			int num2 = 0;
			int num3 = -1;
			for (;;)
			{
				char c;
				if ((this.xmlCharType.charProperties[(int)(c = chars[num])] & 64) == 0 || c == '?')
				{
					char c2 = chars[num];
					if (c2 <= '&')
					{
						switch (c2)
						{
						case '\t':
							break;
						case '\n':
							num++;
							this.OnNewLine(num);
							continue;
						case '\v':
						case '\f':
							goto IL_1F0;
						case '\r':
							if (chars[num + 1] == '\n')
							{
								if (!this.ps.eolNormalized && this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
								{
									if (num - this.ps.charPos > 0)
									{
										if (num2 == 0)
										{
											num2 = 1;
											num3 = num;
										}
										else
										{
											this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
											num3 = num - num2;
											num2++;
										}
									}
									else
									{
										this.ps.charPos = this.ps.charPos + 1;
									}
								}
								num += 2;
							}
							else
							{
								if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_247;
								}
								if (!this.ps.eolNormalized)
								{
									chars[num] = '\n';
								}
								num++;
							}
							this.OnNewLine(num);
							continue;
						default:
							if (c2 != '&')
							{
								goto IL_1F0;
							}
							break;
						}
					}
					else if (c2 != '<')
					{
						if (c2 != '?')
						{
							if (c2 != ']')
							{
								goto IL_1F0;
							}
						}
						else
						{
							if (chars[num + 1] == '>')
							{
								break;
							}
							if (num + 1 != this.ps.charsUsed)
							{
								num++;
								continue;
							}
							goto IL_247;
						}
					}
					num++;
					continue;
					IL_1F0:
					if (num == this.ps.charsUsed)
					{
						goto IL_247;
					}
					if (XmlCharType.IsHighSurrogate((int)chars[num]))
					{
						if (num + 1 == this.ps.charsUsed)
						{
							goto IL_247;
						}
						num++;
						if (XmlCharType.IsLowSurrogate((int)chars[num]))
						{
							num++;
							continue;
						}
					}
					this.ThrowInvalidChar(chars, this.ps.charsUsed, num);
				}
				else
				{
					num++;
				}
			}
			if (num2 > 0)
			{
				this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
				outEndPos = num - num2;
			}
			else
			{
				outEndPos = num;
			}
			outStartPos = this.ps.charPos;
			this.ps.charPos = num + 2;
			return true;
			IL_247:
			if (num2 > 0)
			{
				this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
				outEndPos = num - num2;
			}
			else
			{
				outEndPos = num;
			}
			outStartPos = this.ps.charPos;
			this.ps.charPos = num;
			return false;
		}

		private bool ParseComment()
		{
			if (this.ignoreComments)
			{
				XmlTextReaderImpl.ParsingMode parsingMode = this.parsingMode;
				this.parsingMode = XmlTextReaderImpl.ParsingMode.SkipNode;
				this.ParseCDataOrComment(XmlNodeType.Comment);
				this.parsingMode = parsingMode;
				return false;
			}
			this.ParseCDataOrComment(XmlNodeType.Comment);
			return true;
		}

		private void ParseCData()
		{
			this.ParseCDataOrComment(XmlNodeType.CDATA);
		}

		private void ParseCDataOrComment(XmlNodeType type)
		{
			int num;
			int num2;
			if (this.parsingMode != XmlTextReaderImpl.ParsingMode.Full)
			{
				while (!this.ParseCDataOrComment(type, out num, out num2))
				{
				}
				return;
			}
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			if (this.ParseCDataOrComment(type, out num, out num2))
			{
				this.curNode.SetValueNode(type, this.ps.chars, num, num2 - num);
				return;
			}
			do
			{
				this.stringBuilder.Append(this.ps.chars, num, num2 - num);
			}
			while (!this.ParseCDataOrComment(type, out num, out num2));
			this.stringBuilder.Append(this.ps.chars, num, num2 - num);
			this.curNode.SetValueNode(type, this.stringBuilder.ToString());
			this.stringBuilder.Length = 0;
		}

		private bool ParseCDataOrComment(XmlNodeType type, out int outStartPos, out int outEndPos)
		{
			if (this.ps.charsUsed - this.ps.charPos < 3 && this.ReadData() == 0)
			{
				this.Throw("Unexpected end of file while parsing {0} has occurred.", (type == XmlNodeType.Comment) ? "Comment" : "CDATA");
			}
			int num = this.ps.charPos;
			char[] chars = this.ps.chars;
			int num2 = 0;
			int num3 = -1;
			char c = (type == XmlNodeType.Comment) ? '-' : ']';
			for (;;)
			{
				char c2;
				if ((this.xmlCharType.charProperties[(int)(c2 = chars[num])] & 64) == 0 || c2 == c)
				{
					if (chars[num] == c)
					{
						if (chars[num + 1] == c)
						{
							if (chars[num + 2] == '>')
							{
								break;
							}
							if (num + 2 == this.ps.charsUsed)
							{
								goto IL_27D;
							}
							if (type == XmlNodeType.Comment)
							{
								this.Throw(num, "An XML comment cannot contain '--', and '-' cannot be the last character.");
							}
						}
						else if (num + 1 == this.ps.charsUsed)
						{
							goto IL_27D;
						}
						num++;
					}
					else
					{
						char c3 = chars[num];
						if (c3 <= '&')
						{
							switch (c3)
							{
							case '\t':
								break;
							case '\n':
								num++;
								this.OnNewLine(num);
								continue;
							case '\v':
							case '\f':
								goto IL_22B;
							case '\r':
								if (chars[num + 1] == '\n')
								{
									if (!this.ps.eolNormalized && this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
									{
										if (num - this.ps.charPos > 0)
										{
											if (num2 == 0)
											{
												num2 = 1;
												num3 = num;
											}
											else
											{
												this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
												num3 = num - num2;
												num2++;
											}
										}
										else
										{
											this.ps.charPos = this.ps.charPos + 1;
										}
									}
									num += 2;
								}
								else
								{
									if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
									{
										goto IL_27D;
									}
									if (!this.ps.eolNormalized)
									{
										chars[num] = '\n';
									}
									num++;
								}
								this.OnNewLine(num);
								continue;
							default:
								if (c3 != '&')
								{
									goto IL_22B;
								}
								break;
							}
						}
						else if (c3 != '<' && c3 != ']')
						{
							goto IL_22B;
						}
						num++;
						continue;
						IL_22B:
						if (num == this.ps.charsUsed)
						{
							goto IL_27D;
						}
						if (!XmlCharType.IsHighSurrogate((int)chars[num]))
						{
							goto IL_26A;
						}
						if (num + 1 == this.ps.charsUsed)
						{
							goto IL_27D;
						}
						num++;
						if (!XmlCharType.IsLowSurrogate((int)chars[num]))
						{
							goto IL_26A;
						}
						num++;
					}
				}
				else
				{
					num++;
				}
			}
			if (num2 > 0)
			{
				this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
				outEndPos = num - num2;
			}
			else
			{
				outEndPos = num;
			}
			outStartPos = this.ps.charPos;
			this.ps.charPos = num + 3;
			return true;
			IL_26A:
			this.ThrowInvalidChar(chars, this.ps.charsUsed, num);
			IL_27D:
			if (num2 > 0)
			{
				this.ShiftBuffer(num3 + num2, num3, num - num3 - num2);
				outEndPos = num - num2;
			}
			else
			{
				outEndPos = num;
			}
			outStartPos = this.ps.charPos;
			this.ps.charPos = num;
			return false;
		}

		private bool ParseDoctypeDecl()
		{
			if (this.dtdProcessing == DtdProcessing.Prohibit)
			{
				this.ThrowWithoutLineInfo(this.v1Compat ? "DTD is prohibited in this XML document." : "For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.");
			}
			while (this.ps.charsUsed - this.ps.charPos < 8)
			{
				if (this.ReadData() == 0)
				{
					this.Throw("Unexpected end of file while parsing {0} has occurred.", "DOCTYPE");
				}
			}
			if (!XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, 7, "DOCTYPE"))
			{
				this.ThrowUnexpectedToken((!this.rootElementParsed && this.dtdInfo == null) ? "DOCTYPE" : "<!--");
			}
			if (!this.xmlCharType.IsWhiteSpace(this.ps.chars[this.ps.charPos + 7]))
			{
				this.ThrowExpectingWhitespace(this.ps.charPos + 7);
			}
			if (this.dtdInfo != null)
			{
				this.Throw(this.ps.charPos - 2, "Cannot have multiple DTDs.");
			}
			if (this.rootElementParsed)
			{
				this.Throw(this.ps.charPos - 2, "DTD must be defined before the document root element.");
			}
			this.ps.charPos = this.ps.charPos + 8;
			this.EatWhitespaces(null);
			if (this.dtdProcessing == DtdProcessing.Parse)
			{
				this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
				this.ParseDtd();
				this.nextParsingFunction = this.parsingFunction;
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.ResetAttributesRootLevel;
				return true;
			}
			this.SkipDtd();
			return false;
		}

		private void ParseDtd()
		{
			IDtdParser dtdParser = DtdParser.Create();
			this.dtdInfo = dtdParser.ParseInternalDtd(new XmlTextReaderImpl.DtdParserProxy(this), true);
			if ((this.validatingReaderCompatFlag || !this.v1Compat) && (this.dtdInfo.HasDefaultAttributes || this.dtdInfo.HasNonCDataAttributes))
			{
				this.addDefaultAttributesAndNormalize = true;
			}
			this.curNode.SetNamedNode(XmlNodeType.DocumentType, this.dtdInfo.Name.ToString(), string.Empty, null);
			this.curNode.SetValue(this.dtdInfo.InternalDtdSubset);
		}

		private void SkipDtd()
		{
			int num;
			int charPos = this.ParseQName(out num);
			this.ps.charPos = charPos;
			this.EatWhitespaces(null);
			if (this.ps.chars[this.ps.charPos] == 'P')
			{
				while (this.ps.charsUsed - this.ps.charPos < 6)
				{
					if (this.ReadData() == 0)
					{
						this.Throw("Unexpected end of file has occurred.");
					}
				}
				if (!XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, 6, "PUBLIC"))
				{
					this.ThrowUnexpectedToken("PUBLIC");
				}
				this.ps.charPos = this.ps.charPos + 6;
				if (this.EatWhitespaces(null) == 0)
				{
					this.ThrowExpectingWhitespace(this.ps.charPos);
				}
				this.SkipPublicOrSystemIdLiteral();
				if (this.EatWhitespaces(null) == 0)
				{
					this.ThrowExpectingWhitespace(this.ps.charPos);
				}
				this.SkipPublicOrSystemIdLiteral();
				this.EatWhitespaces(null);
			}
			else if (this.ps.chars[this.ps.charPos] == 'S')
			{
				while (this.ps.charsUsed - this.ps.charPos < 6)
				{
					if (this.ReadData() == 0)
					{
						this.Throw("Unexpected end of file has occurred.");
					}
				}
				if (!XmlConvert.StrEqual(this.ps.chars, this.ps.charPos, 6, "SYSTEM"))
				{
					this.ThrowUnexpectedToken("SYSTEM");
				}
				this.ps.charPos = this.ps.charPos + 6;
				if (this.EatWhitespaces(null) == 0)
				{
					this.ThrowExpectingWhitespace(this.ps.charPos);
				}
				this.SkipPublicOrSystemIdLiteral();
				this.EatWhitespaces(null);
			}
			else if (this.ps.chars[this.ps.charPos] != '[' && this.ps.chars[this.ps.charPos] != '>')
			{
				this.Throw("Expecting external ID, '[' or '>'.");
			}
			if (this.ps.chars[this.ps.charPos] == '[')
			{
				this.ps.charPos = this.ps.charPos + 1;
				this.SkipUntil(']', true);
				this.EatWhitespaces(null);
				if (this.ps.chars[this.ps.charPos] != '>')
				{
					this.ThrowUnexpectedToken(">");
				}
			}
			else if (this.ps.chars[this.ps.charPos] == '>')
			{
				this.curNode.SetValue(string.Empty);
			}
			else
			{
				this.Throw("Expecting an internal subset or the end of the DOCTYPE declaration.");
			}
			this.ps.charPos = this.ps.charPos + 1;
		}

		private void SkipPublicOrSystemIdLiteral()
		{
			char c = this.ps.chars[this.ps.charPos];
			if (c != '"' && c != '\'')
			{
				this.ThrowUnexpectedToken("\"", "'");
			}
			this.ps.charPos = this.ps.charPos + 1;
			this.SkipUntil(c, false);
		}

		private void SkipUntil(char stopChar, bool recognizeLiterals)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			char c = '"';
			char[] chars = this.ps.chars;
			int num = this.ps.charPos;
			for (;;)
			{
				char c2;
				if ((this.xmlCharType.charProperties[(int)(c2 = chars[num])] & 128) == 0 || chars[num] == stopChar || c2 == '-' || c2 == '?')
				{
					if (c2 == stopChar && !flag)
					{
						break;
					}
					this.ps.charPos = num;
					if (c2 <= '&')
					{
						switch (c2)
						{
						case '\t':
							break;
						case '\n':
							num++;
							this.OnNewLine(num);
							continue;
						case '\v':
						case '\f':
							goto IL_2D1;
						case '\r':
							if (chars[num + 1] == '\n')
							{
								num += 2;
							}
							else
							{
								if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_32F;
								}
								num++;
							}
							this.OnNewLine(num);
							continue;
						default:
							if (c2 == '"')
							{
								goto IL_2AC;
							}
							if (c2 != '&')
							{
								goto IL_2D1;
							}
							break;
						}
					}
					else if (c2 <= '-')
					{
						if (c2 == '\'')
						{
							goto IL_2AC;
						}
						if (c2 != '-')
						{
							goto IL_2D1;
						}
						if (flag2)
						{
							if (num + 2 >= this.ps.charsUsed && !this.ps.isEof)
							{
								goto IL_32F;
							}
							if (chars[num + 1] == '-' && chars[num + 2] == '>')
							{
								flag2 = false;
								num += 2;
								continue;
							}
						}
						num++;
						continue;
					}
					else
					{
						switch (c2)
						{
						case '<':
							if (chars[num + 1] == '?')
							{
								if (recognizeLiterals && !flag && !flag2)
								{
									flag3 = true;
									num += 2;
									continue;
								}
							}
							else if (chars[num + 1] == '!')
							{
								if (num + 3 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_32F;
								}
								if (chars[num + 2] == '-' && chars[num + 3] == '-' && recognizeLiterals && !flag && !flag3)
								{
									flag2 = true;
									num += 4;
									continue;
								}
							}
							else if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
							{
								goto IL_32F;
							}
							num++;
							continue;
						case '=':
							goto IL_2D1;
						case '>':
							break;
						case '?':
							if (flag3)
							{
								if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_32F;
								}
								if (chars[num + 1] == '>')
								{
									flag3 = false;
									num++;
									continue;
								}
							}
							num++;
							continue;
						default:
							if (c2 != ']')
							{
								goto IL_2D1;
							}
							break;
						}
					}
					num++;
					continue;
					IL_2AC:
					if (flag)
					{
						if (c == c2)
						{
							flag = false;
						}
					}
					else if (recognizeLiterals && !flag2 && !flag3)
					{
						flag = true;
						c = c2;
					}
					num++;
					continue;
					IL_2D1:
					if (num != this.ps.charsUsed)
					{
						if (XmlCharType.IsHighSurrogate((int)chars[num]))
						{
							if (num + 1 == this.ps.charsUsed)
							{
								goto IL_32F;
							}
							num++;
							if (XmlCharType.IsLowSurrogate((int)chars[num]))
							{
								num++;
								continue;
							}
						}
						this.ThrowInvalidChar(chars, this.ps.charsUsed, num);
					}
					IL_32F:
					if (this.ReadData() == 0)
					{
						if (this.ps.charsUsed - this.ps.charPos > 0)
						{
							if (this.ps.chars[this.ps.charPos] != '\r')
							{
								this.Throw("Unexpected end of file has occurred.");
							}
						}
						else
						{
							this.Throw("Unexpected end of file has occurred.");
						}
					}
					chars = this.ps.chars;
					num = this.ps.charPos;
				}
				else
				{
					num++;
				}
			}
			this.ps.charPos = num + 1;
		}

		private int EatWhitespaces(StringBuilder sb)
		{
			int num = this.ps.charPos;
			int num2 = 0;
			char[] chars = this.ps.chars;
			for (;;)
			{
				char c = chars[num];
				switch (c)
				{
				case '\t':
					break;
				case '\n':
					num++;
					this.OnNewLine(num);
					continue;
				case '\v':
				case '\f':
					goto IL_FE;
				case '\r':
					if (chars[num + 1] == '\n')
					{
						int num3 = num - this.ps.charPos;
						if (sb != null && !this.ps.eolNormalized)
						{
							if (num3 > 0)
							{
								sb.Append(chars, this.ps.charPos, num3);
								num2 += num3;
							}
							this.ps.charPos = num + 1;
						}
						num += 2;
					}
					else
					{
						if (num + 1 >= this.ps.charsUsed && !this.ps.isEof)
						{
							goto IL_155;
						}
						if (!this.ps.eolNormalized)
						{
							chars[num] = '\n';
						}
						num++;
					}
					this.OnNewLine(num);
					continue;
				default:
					if (c != ' ')
					{
						goto IL_FE;
					}
					break;
				}
				num++;
				continue;
				IL_155:
				int num4 = num - this.ps.charPos;
				if (num4 > 0)
				{
					if (sb != null)
					{
						sb.Append(this.ps.chars, this.ps.charPos, num4);
					}
					this.ps.charPos = num;
					num2 += num4;
				}
				if (this.ReadData() == 0)
				{
					if (this.ps.charsUsed - this.ps.charPos == 0)
					{
						return num2;
					}
					if (this.ps.chars[this.ps.charPos] != '\r')
					{
						this.Throw("Unexpected end of file has occurred.");
					}
				}
				num = this.ps.charPos;
				chars = this.ps.chars;
				continue;
				IL_FE:
				if (num != this.ps.charsUsed)
				{
					break;
				}
				goto IL_155;
			}
			int num5 = num - this.ps.charPos;
			if (num5 > 0)
			{
				if (sb != null)
				{
					sb.Append(this.ps.chars, this.ps.charPos, num5);
				}
				this.ps.charPos = num;
				num2 += num5;
			}
			return num2;
		}

		private int ParseCharRefInline(int startPos, out int charCount, out XmlTextReaderImpl.EntityType entityType)
		{
			if (this.ps.chars[startPos + 1] == '#')
			{
				return this.ParseNumericCharRefInline(startPos, true, null, out charCount, out entityType);
			}
			charCount = 1;
			entityType = XmlTextReaderImpl.EntityType.CharacterNamed;
			return this.ParseNamedCharRefInline(startPos, true, null);
		}

		private int ParseNumericCharRef(bool expand, StringBuilder internalSubsetBuilder, out XmlTextReaderImpl.EntityType entityType)
		{
			int num2;
			int num;
			while ((num = this.ParseNumericCharRefInline(this.ps.charPos, expand, internalSubsetBuilder, out num2, out entityType)) == -2)
			{
				if (this.ReadData() == 0)
				{
					this.Throw("Unexpected end of file while parsing {0} has occurred.");
				}
			}
			if (expand)
			{
				this.ps.charPos = num - num2;
			}
			return num;
		}

		private int ParseNumericCharRefInline(int startPos, bool expand, StringBuilder internalSubsetBuilder, out int charCount, out XmlTextReaderImpl.EntityType entityType)
		{
			int num = 0;
			string res = null;
			char[] chars = this.ps.chars;
			int num2 = startPos + 2;
			charCount = 0;
			int num3 = 0;
			try
			{
				if (chars[num2] == 'x')
				{
					num2++;
					num3 = num2;
					res = "Invalid syntax for a hexadecimal numeric entity reference.";
					for (;;)
					{
						char c = chars[num2];
						checked
						{
							if (c >= '0' && c <= '9')
							{
								num = num * 16 + (int)c - 48;
							}
							else if (c >= 'a' && c <= 'f')
							{
								num = num * 16 + 10 + (int)c - 97;
							}
							else
							{
								if (c < 'A' || c > 'F')
								{
									break;
								}
								num = num * 16 + 10 + (int)c - 65;
							}
						}
						num2++;
					}
					entityType = XmlTextReaderImpl.EntityType.CharacterHex;
				}
				else
				{
					if (num2 >= this.ps.charsUsed)
					{
						entityType = XmlTextReaderImpl.EntityType.Skipped;
						return -2;
					}
					num3 = num2;
					res = "Invalid syntax for a decimal numeric entity reference.";
					while (chars[num2] >= '0' && chars[num2] <= '9')
					{
						num = checked(num * 10 + (int)chars[num2] - 48);
						num2++;
					}
					entityType = XmlTextReaderImpl.EntityType.CharacterDec;
				}
			}
			catch (OverflowException innerException)
			{
				this.ps.charPos = num2;
				entityType = XmlTextReaderImpl.EntityType.Skipped;
				this.Throw("Invalid value of a character entity reference.", null, innerException);
			}
			if (chars[num2] != ';' || num3 == num2)
			{
				if (num2 == this.ps.charsUsed)
				{
					return -2;
				}
				this.Throw(num2, res);
			}
			if (num <= 65535)
			{
				char c2 = (char)num;
				if (!this.xmlCharType.IsCharData(c2) && ((this.v1Compat && this.normalize) || (!this.v1Compat && this.checkCharacters)))
				{
					this.Throw((this.ps.chars[startPos + 2] == 'x') ? (startPos + 3) : (startPos + 2), "'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(c2, '\0'));
				}
				if (expand)
				{
					if (internalSubsetBuilder != null)
					{
						internalSubsetBuilder.Append(this.ps.chars, this.ps.charPos, num2 - this.ps.charPos + 1);
					}
					chars[num2] = c2;
				}
				charCount = 1;
				return num2 + 1;
			}
			char c3;
			char c4;
			XmlCharType.SplitSurrogateChar(num, out c3, out c4);
			if (this.normalize && (!XmlCharType.IsHighSurrogate((int)c4) || !XmlCharType.IsLowSurrogate((int)c3)))
			{
				this.Throw((this.ps.chars[startPos + 2] == 'x') ? (startPos + 3) : (startPos + 2), "'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(c4, c3));
			}
			if (expand)
			{
				if (internalSubsetBuilder != null)
				{
					internalSubsetBuilder.Append(this.ps.chars, this.ps.charPos, num2 - this.ps.charPos + 1);
				}
				chars[num2 - 1] = c4;
				chars[num2] = c3;
			}
			charCount = 2;
			return num2 + 1;
		}

		private int ParseNamedCharRef(bool expand, StringBuilder internalSubsetBuilder)
		{
			int num2;
			int num;
			for (;;)
			{
				num = (num2 = this.ParseNamedCharRefInline(this.ps.charPos, expand, internalSubsetBuilder));
				if (num2 != -2)
				{
					break;
				}
				if (this.ReadData() == 0)
				{
					return -1;
				}
			}
			if (num2 == -1)
			{
				return -1;
			}
			if (expand)
			{
				this.ps.charPos = num - 1;
			}
			return num;
		}

		private int ParseNamedCharRefInline(int startPos, bool expand, StringBuilder internalSubsetBuilder)
		{
			int num = startPos + 1;
			char[] chars = this.ps.chars;
			char c = chars[num];
			if (c <= 'g')
			{
				if (c != 'a')
				{
					if (c == 'g')
					{
						if (this.ps.charsUsed - num < 3)
						{
							return -2;
						}
						if (chars[num + 1] == 't' && chars[num + 2] == ';')
						{
							num += 3;
							char c2 = '>';
							goto IL_175;
						}
						return -1;
					}
				}
				else
				{
					num++;
					if (chars[num] == 'm')
					{
						if (this.ps.charsUsed - num < 3)
						{
							return -2;
						}
						if (chars[num + 1] == 'p' && chars[num + 2] == ';')
						{
							num += 3;
							char c2 = '&';
							goto IL_175;
						}
						return -1;
					}
					else if (chars[num] == 'p')
					{
						if (this.ps.charsUsed - num < 4)
						{
							return -2;
						}
						if (chars[num + 1] == 'o' && chars[num + 2] == 's' && chars[num + 3] == ';')
						{
							num += 4;
							char c2 = '\'';
							goto IL_175;
						}
						return -1;
					}
					else
					{
						if (num < this.ps.charsUsed)
						{
							return -1;
						}
						return -2;
					}
				}
			}
			else if (c != 'l')
			{
				if (c == 'q')
				{
					if (this.ps.charsUsed - num < 5)
					{
						return -2;
					}
					if (chars[num + 1] == 'u' && chars[num + 2] == 'o' && chars[num + 3] == 't' && chars[num + 4] == ';')
					{
						num += 5;
						char c2 = '"';
						goto IL_175;
					}
					return -1;
				}
			}
			else
			{
				if (this.ps.charsUsed - num < 3)
				{
					return -2;
				}
				if (chars[num + 1] == 't' && chars[num + 2] == ';')
				{
					num += 3;
					char c2 = '<';
					goto IL_175;
				}
				return -1;
			}
			return -1;
			IL_175:
			if (expand)
			{
				if (internalSubsetBuilder != null)
				{
					internalSubsetBuilder.Append(this.ps.chars, this.ps.charPos, num - this.ps.charPos);
				}
				char c2;
				this.ps.chars[num - 1] = c2;
			}
			return num;
		}

		private int ParseName()
		{
			int num;
			return this.ParseQName(false, 0, out num);
		}

		private int ParseQName(out int colonPos)
		{
			return this.ParseQName(true, 0, out colonPos);
		}

		private int ParseQName(bool isQName, int startOffset, out int colonPos)
		{
			int num = -1;
			int num2 = this.ps.charPos + startOffset;
			for (;;)
			{
				char[] chars = this.ps.chars;
				if ((this.xmlCharType.charProperties[(int)chars[num2]] & 4) != 0)
				{
					num2++;
				}
				else
				{
					if (num2 + 1 >= this.ps.charsUsed)
					{
						if (this.ReadDataInName(ref num2))
						{
							continue;
						}
						this.Throw(num2, "Unexpected end of file while parsing {0} has occurred.", "Name");
					}
					if (chars[num2] != ':' || this.supportNamespaces)
					{
						this.Throw(num2, "Name cannot begin with the '{0}' character, hexadecimal value {1}.", XmlException.BuildCharExceptionArgs(chars, this.ps.charsUsed, num2));
					}
				}
				for (;;)
				{
					if ((this.xmlCharType.charProperties[(int)chars[num2]] & 8) != 0)
					{
						num2++;
					}
					else if (chars[num2] == ':')
					{
						if (this.supportNamespaces)
						{
							break;
						}
						num = num2 - this.ps.charPos;
						num2++;
					}
					else
					{
						if (num2 != this.ps.charsUsed)
						{
							goto IL_135;
						}
						if (!this.ReadDataInName(ref num2))
						{
							goto IL_124;
						}
						chars = this.ps.chars;
					}
				}
				if (num != -1 || !isQName)
				{
					this.Throw(num2, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(':', '\0'));
				}
				num = num2 - this.ps.charPos;
				num2++;
			}
			IL_124:
			this.Throw(num2, "Unexpected end of file while parsing {0} has occurred.", "Name");
			IL_135:
			colonPos = ((num == -1) ? -1 : (this.ps.charPos + num));
			return num2;
		}

		private bool ReadDataInName(ref int pos)
		{
			int num = pos - this.ps.charPos;
			bool result = this.ReadData() != 0;
			pos = this.ps.charPos + num;
			return result;
		}

		private string ParseEntityName()
		{
			int num;
			try
			{
				num = this.ParseName();
			}
			catch (XmlException)
			{
				this.Throw("An error occurred while parsing EntityName.");
				return null;
			}
			if (this.ps.chars[num] != ';')
			{
				this.Throw("An error occurred while parsing EntityName.");
			}
			string result = this.nameTable.Add(this.ps.chars, this.ps.charPos, num - this.ps.charPos);
			this.ps.charPos = num + 1;
			return result;
		}

		private XmlTextReaderImpl.NodeData AddNode(int nodeIndex, int nodeDepth)
		{
			XmlTextReaderImpl.NodeData nodeData = this.nodes[nodeIndex];
			if (nodeData != null)
			{
				nodeData.depth = nodeDepth;
				return nodeData;
			}
			return this.AllocNode(nodeIndex, nodeDepth);
		}

		private XmlTextReaderImpl.NodeData AllocNode(int nodeIndex, int nodeDepth)
		{
			if (nodeIndex >= this.nodes.Length - 1)
			{
				XmlTextReaderImpl.NodeData[] destinationArray = new XmlTextReaderImpl.NodeData[this.nodes.Length * 2];
				Array.Copy(this.nodes, 0, destinationArray, 0, this.nodes.Length);
				this.nodes = destinationArray;
			}
			XmlTextReaderImpl.NodeData nodeData = this.nodes[nodeIndex];
			if (nodeData == null)
			{
				nodeData = new XmlTextReaderImpl.NodeData();
				this.nodes[nodeIndex] = nodeData;
			}
			nodeData.depth = nodeDepth;
			return nodeData;
		}

		private XmlTextReaderImpl.NodeData AddAttributeNoChecks(string name, int attrDepth)
		{
			XmlTextReaderImpl.NodeData nodeData = this.AddNode(this.index + this.attrCount + 1, attrDepth);
			nodeData.SetNamedNode(XmlNodeType.Attribute, this.nameTable.Add(name));
			this.attrCount++;
			return nodeData;
		}

		private XmlTextReaderImpl.NodeData AddAttribute(int endNamePos, int colonPos)
		{
			if (colonPos == -1 || !this.supportNamespaces)
			{
				string text = this.nameTable.Add(this.ps.chars, this.ps.charPos, endNamePos - this.ps.charPos);
				return this.AddAttribute(text, string.Empty, text);
			}
			this.attrNeedNamespaceLookup = true;
			int charPos = this.ps.charPos;
			int num = colonPos - charPos;
			if (num == this.lastPrefix.Length && XmlConvert.StrEqual(this.ps.chars, charPos, num, this.lastPrefix))
			{
				return this.AddAttribute(this.nameTable.Add(this.ps.chars, colonPos + 1, endNamePos - colonPos - 1), this.lastPrefix, null);
			}
			string prefix = this.nameTable.Add(this.ps.chars, charPos, num);
			this.lastPrefix = prefix;
			return this.AddAttribute(this.nameTable.Add(this.ps.chars, colonPos + 1, endNamePos - colonPos - 1), prefix, null);
		}

		private XmlTextReaderImpl.NodeData AddAttribute(string localName, string prefix, string nameWPrefix)
		{
			XmlTextReaderImpl.NodeData nodeData = this.AddNode(this.index + this.attrCount + 1, this.index + 1);
			nodeData.SetNamedNode(XmlNodeType.Attribute, localName, prefix, nameWPrefix);
			int num = 1 << (int)localName[0];
			if ((this.attrHashtable & num) == 0)
			{
				this.attrHashtable |= num;
			}
			else if (this.attrDuplWalkCount < 250)
			{
				this.attrDuplWalkCount++;
				for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
				{
					if (Ref.Equal(this.nodes[i].localName, nodeData.localName))
					{
						this.attrDuplWalkCount = 250;
						break;
					}
				}
			}
			this.attrCount++;
			return nodeData;
		}

		private void PopElementContext()
		{
			this.namespaceManager.PopScope();
			if (this.curNode.xmlContextPushed)
			{
				this.PopXmlContext();
			}
		}

		private void OnNewLine(int pos)
		{
			this.ps.lineNo = this.ps.lineNo + 1;
			this.ps.lineStartPos = pos - 1;
		}

		private void OnEof()
		{
			this.curNode = this.nodes[0];
			this.curNode.Clear(XmlNodeType.None);
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.Eof;
			this.readState = ReadState.EndOfFile;
			this.reportedEncoding = null;
		}

		private string LookupNamespace(XmlTextReaderImpl.NodeData node)
		{
			string text = this.namespaceManager.LookupNamespace(node.prefix);
			if (text != null)
			{
				return text;
			}
			this.Throw("'{0}' is an undeclared prefix.", node.prefix, node.LineNo, node.LinePos);
			return null;
		}

		private void AddNamespace(string prefix, string uri, XmlTextReaderImpl.NodeData attr)
		{
			if (uri == "http://www.w3.org/2000/xmlns/")
			{
				if (Ref.Equal(prefix, this.XmlNs))
				{
					this.Throw("Prefix \"xmlns\" is reserved for use by XML.", attr.lineInfo2.lineNo, attr.lineInfo2.linePos);
				}
				else
				{
					this.Throw("Prefix '{0}' cannot be mapped to namespace name reserved for \"xml\" or \"xmlns\".", prefix, attr.lineInfo2.lineNo, attr.lineInfo2.linePos);
				}
			}
			else if (uri == "http://www.w3.org/XML/1998/namespace" && !Ref.Equal(prefix, this.Xml) && !this.v1Compat)
			{
				this.Throw("Prefix '{0}' cannot be mapped to namespace name reserved for \"xml\" or \"xmlns\".", prefix, attr.lineInfo2.lineNo, attr.lineInfo2.linePos);
			}
			if (uri.Length == 0 && prefix.Length > 0)
			{
				this.Throw("Invalid namespace declaration.", attr.lineInfo.lineNo, attr.lineInfo.linePos);
			}
			try
			{
				this.namespaceManager.AddNamespace(prefix, uri);
			}
			catch (ArgumentException e)
			{
				this.ReThrow(e, attr.lineInfo.lineNo, attr.lineInfo.linePos);
			}
		}

		private void ResetAttributes()
		{
			if (this.fullAttrCleanup)
			{
				this.FullAttributeCleanup();
			}
			this.curAttrIndex = -1;
			this.attrCount = 0;
			this.attrHashtable = 0;
			this.attrDuplWalkCount = 0;
		}

		private void FullAttributeCleanup()
		{
			for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
			{
				XmlTextReaderImpl.NodeData nodeData = this.nodes[i];
				nodeData.nextAttrValueChunk = null;
				nodeData.IsDefaultAttribute = false;
			}
			this.fullAttrCleanup = false;
		}

		private void PushXmlContext()
		{
			this.xmlContext = new XmlTextReaderImpl.XmlContext(this.xmlContext);
			this.curNode.xmlContextPushed = true;
		}

		private void PopXmlContext()
		{
			this.xmlContext = this.xmlContext.previousContext;
			this.curNode.xmlContextPushed = false;
		}

		private XmlNodeType GetWhitespaceType()
		{
			if (this.whitespaceHandling != WhitespaceHandling.None)
			{
				if (this.xmlContext.xmlSpace == XmlSpace.Preserve)
				{
					return XmlNodeType.SignificantWhitespace;
				}
				if (this.whitespaceHandling == WhitespaceHandling.All)
				{
					return XmlNodeType.Whitespace;
				}
			}
			return XmlNodeType.None;
		}

		private XmlNodeType GetTextNodeType(int orChars)
		{
			if (orChars > 32)
			{
				return XmlNodeType.Text;
			}
			return this.GetWhitespaceType();
		}

		private void PushExternalEntityOrSubset(string publicId, string systemId, Uri baseUri, string entityName)
		{
			Uri uri;
			if (!string.IsNullOrEmpty(publicId))
			{
				try
				{
					uri = this.xmlResolver.ResolveUri(baseUri, publicId);
					if (this.OpenAndPush(uri))
					{
						return;
					}
				}
				catch (Exception)
				{
				}
			}
			uri = this.xmlResolver.ResolveUri(baseUri, systemId);
			try
			{
				if (this.OpenAndPush(uri))
				{
					return;
				}
			}
			catch (Exception ex)
			{
				if (this.v1Compat)
				{
					throw;
				}
				string message = ex.Message;
				this.Throw(new XmlException((entityName == null) ? "An error has occurred while opening external DTD '{0}': {1}" : "An error has occurred while opening external entity '{0}': {1}", new string[]
				{
					uri.ToString(),
					message
				}, ex, 0, 0));
			}
			if (entityName == null)
			{
				this.ThrowWithoutLineInfo("Cannot resolve external DTD subset - public ID = '{0}', system ID = '{1}'.", new string[]
				{
					(publicId != null) ? publicId : string.Empty,
					systemId
				}, null);
				return;
			}
			this.Throw((this.dtdProcessing == DtdProcessing.Ignore) ? "Cannot resolve entity reference '{0}' because the DTD has been ignored. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method." : "Cannot resolve entity reference '{0}'.", entityName);
		}

		private bool OpenAndPush(Uri uri)
		{
			if (this.xmlResolver.SupportsType(uri, typeof(TextReader)))
			{
				TextReader textReader = (TextReader)this.xmlResolver.GetEntity(uri, null, typeof(TextReader));
				if (textReader == null)
				{
					return false;
				}
				this.PushParsingState();
				this.InitTextReaderInput(uri.ToString(), uri, textReader);
			}
			else
			{
				Stream stream = (Stream)this.xmlResolver.GetEntity(uri, null, typeof(Stream));
				if (stream == null)
				{
					return false;
				}
				this.PushParsingState();
				this.InitStreamInput(uri, stream, null);
			}
			return true;
		}

		private bool PushExternalEntity(IDtdEntityInfo entity)
		{
			if (!this.IsResolverNull)
			{
				Uri baseUri = null;
				if (!string.IsNullOrEmpty(entity.BaseUriString))
				{
					baseUri = this.xmlResolver.ResolveUri(null, entity.BaseUriString);
				}
				this.PushExternalEntityOrSubset(entity.PublicId, entity.SystemId, baseUri, entity.Name);
				this.RegisterEntity(entity);
				int charPos = this.ps.charPos;
				if (this.v1Compat)
				{
					this.EatWhitespaces(null);
				}
				if (!this.ParseXmlDeclaration(true))
				{
					this.ps.charPos = charPos;
				}
				return true;
			}
			Encoding encoding = this.ps.encoding;
			this.PushParsingState();
			this.InitStringInput(entity.SystemId, encoding, string.Empty);
			this.RegisterEntity(entity);
			this.RegisterConsumedCharacters(0L, true);
			return false;
		}

		private void PushInternalEntity(IDtdEntityInfo entity)
		{
			Encoding encoding = this.ps.encoding;
			this.PushParsingState();
			this.InitStringInput((entity.DeclaredUriString != null) ? entity.DeclaredUriString : string.Empty, encoding, entity.Text ?? string.Empty);
			this.RegisterEntity(entity);
			this.ps.lineNo = entity.LineNumber;
			this.ps.lineStartPos = -entity.LinePosition - 1;
			this.ps.eolNormalized = true;
			this.RegisterConsumedCharacters((long)entity.Text.Length, true);
		}

		private void PopEntity()
		{
			if (this.ps.stream != null)
			{
				this.ps.stream.Close();
			}
			this.UnregisterEntity();
			this.PopParsingState();
			this.curNode.entityId = this.ps.entityId;
		}

		private void RegisterEntity(IDtdEntityInfo entity)
		{
			if (this.currentEntities != null && this.currentEntities.ContainsKey(entity))
			{
				this.Throw(entity.IsParameterEntity ? "Parameter entity '{0}' references itself." : "General entity '{0}' references itself.", entity.Name, this.parsingStatesStack[this.parsingStatesStackTop].LineNo, this.parsingStatesStack[this.parsingStatesStackTop].LinePos);
			}
			this.ps.entity = entity;
			int num = this.nextEntityId;
			this.nextEntityId = num + 1;
			this.ps.entityId = num;
			if (entity != null)
			{
				if (this.currentEntities == null)
				{
					this.currentEntities = new Dictionary<IDtdEntityInfo, IDtdEntityInfo>();
				}
				this.currentEntities.Add(entity, entity);
			}
		}

		private void UnregisterEntity()
		{
			if (this.ps.entity != null)
			{
				this.currentEntities.Remove(this.ps.entity);
			}
		}

		private void PushParsingState()
		{
			if (this.parsingStatesStack == null)
			{
				this.parsingStatesStack = new XmlTextReaderImpl.ParsingState[2];
			}
			else if (this.parsingStatesStackTop + 1 == this.parsingStatesStack.Length)
			{
				XmlTextReaderImpl.ParsingState[] destinationArray = new XmlTextReaderImpl.ParsingState[this.parsingStatesStack.Length * 2];
				Array.Copy(this.parsingStatesStack, 0, destinationArray, 0, this.parsingStatesStack.Length);
				this.parsingStatesStack = destinationArray;
			}
			this.parsingStatesStackTop++;
			this.parsingStatesStack[this.parsingStatesStackTop] = this.ps;
			this.ps.Clear();
		}

		private void PopParsingState()
		{
			this.ps.Close(true);
			XmlTextReaderImpl.ParsingState[] array = this.parsingStatesStack;
			int num = this.parsingStatesStackTop;
			this.parsingStatesStackTop = num - 1;
			this.ps = array[num];
		}

		private void InitIncrementalRead(IncrementalReadDecoder decoder)
		{
			this.ResetAttributes();
			decoder.Reset();
			this.incReadDecoder = decoder;
			this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
			this.incReadDepth = 1;
			this.incReadLeftStartPos = this.ps.charPos;
			this.incReadLeftEndPos = this.ps.charPos;
			this.incReadLineInfo.Set(this.ps.LineNo, this.ps.LinePos);
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.InIncrementalRead;
		}

		private int IncrementalRead(Array array, int index, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException((this.incReadDecoder is IncrementalReadCharsDecoder) ? "buffer" : "array");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException((this.incReadDecoder is IncrementalReadCharsDecoder) ? "count" : "len");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException((this.incReadDecoder is IncrementalReadCharsDecoder) ? "index" : "offset");
			}
			if (array.Length - index < count)
			{
				throw new ArgumentException((this.incReadDecoder is IncrementalReadCharsDecoder) ? "count" : "len");
			}
			if (count == 0)
			{
				return 0;
			}
			this.curNode.lineInfo = this.incReadLineInfo;
			this.incReadDecoder.SetNextOutputBuffer(array, index, count);
			this.IncrementalRead();
			return this.incReadDecoder.DecodedCount;
		}

		private int IncrementalRead()
		{
			int num = 0;
			int num3;
			int num4;
			int num5;
			int num7;
			for (;;)
			{
				int num2 = this.incReadLeftEndPos - this.incReadLeftStartPos;
				if (num2 > 0)
				{
					try
					{
						num3 = this.incReadDecoder.Decode(this.ps.chars, this.incReadLeftStartPos, num2);
					}
					catch (XmlException e)
					{
						this.ReThrow(e, this.incReadLineInfo.lineNo, this.incReadLineInfo.linePos);
						return 0;
					}
					if (num3 < num2)
					{
						break;
					}
					this.incReadLeftStartPos = 0;
					this.incReadLeftEndPos = 0;
					this.incReadLineInfo.linePos = this.incReadLineInfo.linePos + num3;
					if (this.incReadDecoder.IsFull)
					{
						return num3;
					}
				}
				num4 = 0;
				num5 = 0;
				int num10;
				for (;;)
				{
					switch (this.incReadState)
					{
					case XmlTextReaderImpl.IncrementalReadState.Text:
					case XmlTextReaderImpl.IncrementalReadState.StartTag:
					case XmlTextReaderImpl.IncrementalReadState.Attributes:
					case XmlTextReaderImpl.IncrementalReadState.AttributeValue:
						goto IL_1D7;
					case XmlTextReaderImpl.IncrementalReadState.PI:
						if (this.ParsePIValue(out num4, out num5))
						{
							this.ps.charPos = this.ps.charPos - 2;
							this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
						}
						break;
					case XmlTextReaderImpl.IncrementalReadState.CDATA:
						if (this.ParseCDataOrComment(XmlNodeType.CDATA, out num4, out num5))
						{
							this.ps.charPos = this.ps.charPos - 3;
							this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
						}
						break;
					case XmlTextReaderImpl.IncrementalReadState.Comment:
						if (this.ParseCDataOrComment(XmlNodeType.Comment, out num4, out num5))
						{
							this.ps.charPos = this.ps.charPos - 3;
							this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
						}
						break;
					case XmlTextReaderImpl.IncrementalReadState.ReadData:
						if (this.ReadData() == 0)
						{
							this.ThrowUnclosedElements();
						}
						this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
						num4 = this.ps.charPos;
						num5 = num4;
						goto IL_1D7;
					case XmlTextReaderImpl.IncrementalReadState.EndElement:
						goto IL_17A;
					case XmlTextReaderImpl.IncrementalReadState.End:
						return num;
					default:
						goto IL_1D7;
					}
					IL_6A4:
					int num6 = num5 - num4;
					if (num6 <= 0)
					{
						continue;
					}
					try
					{
						num7 = this.incReadDecoder.Decode(this.ps.chars, num4, num6);
					}
					catch (XmlException e2)
					{
						this.ReThrow(e2, this.incReadLineInfo.lineNo, this.incReadLineInfo.linePos);
						return 0;
					}
					num += num7;
					if (this.incReadDecoder.IsFull)
					{
						goto Block_54;
					}
					continue;
					IL_1D7:
					char[] chars = this.ps.chars;
					num4 = this.ps.charPos;
					num5 = num4;
					int num8;
					for (;;)
					{
						this.incReadLineInfo.Set(this.ps.LineNo, this.ps.LinePos);
						if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.Attributes)
						{
							char c;
							while ((this.xmlCharType.charProperties[(int)(c = chars[num5])] & 128) != 0)
							{
								if (c == '/')
								{
									break;
								}
								num5++;
							}
						}
						else
						{
							while ((this.xmlCharType.charProperties[(int)chars[num5]] & 128) != 0)
							{
								num5++;
							}
						}
						if (chars[num5] == '&' || chars[num5] == '\t')
						{
							num5++;
						}
						else
						{
							if (num5 - num4 > 0)
							{
								break;
							}
							char c2 = chars[num5];
							if (c2 <= '"')
							{
								if (c2 == '\n')
								{
									num5++;
									this.OnNewLine(num5);
									continue;
								}
								if (c2 == '\r')
								{
									if (chars[num5 + 1] == '\n')
									{
										num5 += 2;
									}
									else
									{
										if (num5 + 1 >= this.ps.charsUsed)
										{
											goto IL_691;
										}
										num5++;
									}
									this.OnNewLine(num5);
									continue;
								}
								if (c2 != '"')
								{
									goto IL_67A;
								}
							}
							else if (c2 <= '/')
							{
								if (c2 != '\'')
								{
									if (c2 != '/')
									{
										goto IL_67A;
									}
									if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.Attributes)
									{
										if (this.ps.charsUsed - num5 < 2)
										{
											goto IL_691;
										}
										if (chars[num5 + 1] == '>')
										{
											this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
											this.incReadDepth--;
										}
									}
									num5++;
									continue;
								}
							}
							else if (c2 != '<')
							{
								if (c2 != '>')
								{
									goto IL_67A;
								}
								if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.Attributes)
								{
									this.incReadState = XmlTextReaderImpl.IncrementalReadState.Text;
								}
								num5++;
								continue;
							}
							else
							{
								if (this.incReadState != XmlTextReaderImpl.IncrementalReadState.Text)
								{
									num5++;
									continue;
								}
								if (this.ps.charsUsed - num5 < 2)
								{
									goto IL_691;
								}
								char c3 = chars[num5 + 1];
								if (c3 != '!')
								{
									if (c3 != '/')
									{
										if (c3 == '?')
										{
											goto Block_31;
										}
										int num9;
										num8 = this.ParseQName(true, 1, out num9);
										if (XmlConvert.StrEqual(this.ps.chars, this.ps.charPos + 1, num8 - this.ps.charPos - 1, this.curNode.localName) && (this.ps.chars[num8] == '>' || this.ps.chars[num8] == '/' || this.xmlCharType.IsWhiteSpace(this.ps.chars[num8])))
										{
											goto IL_594;
										}
										num5 = num8;
										num4 = this.ps.charPos;
										chars = this.ps.chars;
										continue;
									}
									else
									{
										int num11;
										num10 = this.ParseQName(true, 2, out num11);
										if (!XmlConvert.StrEqual(chars, this.ps.charPos + 2, num10 - this.ps.charPos - 2, this.curNode.GetNameWPrefix(this.nameTable)) || (this.ps.chars[num10] != '>' && !this.xmlCharType.IsWhiteSpace(this.ps.chars[num10])))
										{
											num5 = num10;
											num4 = this.ps.charPos;
											chars = this.ps.chars;
											continue;
										}
										int num12 = this.incReadDepth - 1;
										this.incReadDepth = num12;
										if (num12 > 0)
										{
											num5 = num10 + 1;
											continue;
										}
										goto IL_47C;
									}
								}
								else
								{
									if (this.ps.charsUsed - num5 < 4)
									{
										goto IL_691;
									}
									if (chars[num5 + 2] == '-' && chars[num5 + 3] == '-')
									{
										goto Block_34;
									}
									if (this.ps.charsUsed - num5 < 9)
									{
										goto IL_691;
									}
									if (XmlConvert.StrEqual(chars, num5 + 2, 7, "[CDATA["))
									{
										goto Block_36;
									}
									continue;
								}
							}
							XmlTextReaderImpl.IncrementalReadState incrementalReadState = this.incReadState;
							if (incrementalReadState != XmlTextReaderImpl.IncrementalReadState.Attributes)
							{
								if (incrementalReadState == XmlTextReaderImpl.IncrementalReadState.AttributeValue && chars[num5] == this.curNode.quoteChar)
								{
									this.incReadState = XmlTextReaderImpl.IncrementalReadState.Attributes;
								}
							}
							else
							{
								this.curNode.quoteChar = chars[num5];
								this.incReadState = XmlTextReaderImpl.IncrementalReadState.AttributeValue;
							}
							num5++;
							continue;
							IL_67A:
							if (num5 == this.ps.charsUsed)
							{
								goto IL_691;
							}
							num5++;
						}
					}
					IL_698:
					this.ps.charPos = num5;
					goto IL_6A4;
					IL_691:
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadData;
					goto IL_698;
					IL_594:
					this.incReadDepth++;
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.Attributes;
					num5 = num8;
					goto IL_698;
					Block_36:
					num5 += 9;
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.CDATA;
					goto IL_698;
					Block_34:
					num5 += 4;
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.Comment;
					goto IL_698;
					Block_31:
					num5 += 2;
					this.incReadState = XmlTextReaderImpl.IncrementalReadState.PI;
					goto IL_698;
				}
				IL_47C:
				this.ps.charPos = num10;
				if (this.xmlCharType.IsWhiteSpace(this.ps.chars[num10]))
				{
					this.EatWhitespaces(null);
				}
				if (this.ps.chars[this.ps.charPos] != '>')
				{
					this.ThrowUnexpectedToken(">");
				}
				this.ps.charPos = this.ps.charPos + 1;
				this.incReadState = XmlTextReaderImpl.IncrementalReadState.EndElement;
			}
			this.incReadLeftStartPos += num3;
			this.incReadLineInfo.linePos = this.incReadLineInfo.linePos + num3;
			return num3;
			IL_17A:
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PopElementContext;
			this.nextParsingFunction = ((this.index > 0 || this.fragmentType != XmlNodeType.Document) ? XmlTextReaderImpl.ParsingFunction.ElementContent : XmlTextReaderImpl.ParsingFunction.DocumentContent);
			this.outerReader.Read();
			this.incReadState = XmlTextReaderImpl.IncrementalReadState.End;
			return num;
			Block_54:
			this.incReadLeftStartPos = num4 + num7;
			this.incReadLeftEndPos = num5;
			this.incReadLineInfo.linePos = this.incReadLineInfo.linePos + num7;
			return num;
		}

		private void FinishIncrementalRead()
		{
			this.incReadDecoder = new IncrementalReadDummyDecoder();
			this.IncrementalRead();
			this.incReadDecoder = null;
		}

		private bool ParseFragmentAttribute()
		{
			if (this.curNode.type == XmlNodeType.None)
			{
				this.curNode.type = XmlNodeType.Attribute;
				this.curAttrIndex = 0;
				this.ParseAttributeValueSlow(this.ps.charPos, ' ', this.curNode);
			}
			else
			{
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.InReadAttributeValue;
			}
			if (this.ReadAttributeValue())
			{
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.FragmentAttribute;
				return true;
			}
			this.OnEof();
			return false;
		}

		private bool ParseAttributeValueChunk()
		{
			char[] chars = this.ps.chars;
			int num = this.ps.charPos;
			this.curNode = this.AddNode(this.index + this.attrCount + 1, this.index + 2);
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			if (this.emptyEntityInAttributeResolved)
			{
				this.curNode.SetValueNode(XmlNodeType.Text, string.Empty);
				this.emptyEntityInAttributeResolved = false;
				return true;
			}
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)chars[num]] & 128) == 0)
				{
					char c = chars[num];
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
						case '\n':
							if (this.normalize)
							{
								chars[num] = ' ';
							}
							num++;
							continue;
						case '\v':
						case '\f':
							goto IL_21F;
						case '\r':
							num++;
							continue;
						default:
							if (c != '"')
							{
								if (c != '&')
								{
									goto IL_21F;
								}
								if (num - this.ps.charPos > 0)
								{
									this.stringBuilder.Append(chars, this.ps.charPos, num - this.ps.charPos);
								}
								this.ps.charPos = num;
								XmlTextReaderImpl.EntityType entityType = this.HandleEntityReference(true, XmlTextReaderImpl.EntityExpandType.OnlyCharacter, out num);
								if (entityType > XmlTextReaderImpl.EntityType.CharacterNamed)
								{
									if (entityType == XmlTextReaderImpl.EntityType.Unexpanded)
									{
										goto IL_1C5;
									}
								}
								else
								{
									chars = this.ps.chars;
									if (this.normalize && this.xmlCharType.IsWhiteSpace(chars[this.ps.charPos]) && num - this.ps.charPos == 1)
									{
										chars[this.ps.charPos] = ' ';
									}
								}
								chars = this.ps.chars;
								continue;
							}
							break;
						}
					}
					else if (c != '\'')
					{
						if (c == '<')
						{
							this.Throw(num, "'{0}', hexadecimal value {1}, is an invalid attribute character.", XmlException.BuildCharExceptionArgs('<', '\0'));
							goto IL_271;
						}
						if (c != '>')
						{
							goto IL_21F;
						}
					}
					num++;
					continue;
					IL_21F:
					if (num != this.ps.charsUsed)
					{
						if (XmlCharType.IsHighSurrogate((int)chars[num]))
						{
							if (num + 1 == this.ps.charsUsed)
							{
								goto IL_271;
							}
							num++;
							if (XmlCharType.IsLowSurrogate((int)chars[num]))
							{
								num++;
								continue;
							}
						}
						this.ThrowInvalidChar(chars, this.ps.charsUsed, num);
					}
					IL_271:
					if (num - this.ps.charPos > 0)
					{
						this.stringBuilder.Append(chars, this.ps.charPos, num - this.ps.charPos);
						this.ps.charPos = num;
					}
					if (this.ReadData() == 0)
					{
						if (this.stringBuilder.Length > 0)
						{
							goto IL_2F6;
						}
						if (this.HandleEntityEnd(false))
						{
							goto Block_25;
						}
					}
					num = this.ps.charPos;
					chars = this.ps.chars;
				}
				else
				{
					num++;
				}
			}
			IL_1C5:
			if (this.stringBuilder.Length == 0)
			{
				XmlTextReaderImpl.NodeData nodeData = this.curNode;
				nodeData.lineInfo.linePos = nodeData.lineInfo.linePos + 1;
				this.ps.charPos = this.ps.charPos + 1;
				this.curNode.SetNamedNode(XmlNodeType.EntityReference, this.ParseEntityName());
				return true;
			}
			goto IL_2F6;
			Block_25:
			this.SetupEndEntityNodeInAttribute();
			return true;
			IL_2F6:
			if (num - this.ps.charPos > 0)
			{
				this.stringBuilder.Append(chars, this.ps.charPos, num - this.ps.charPos);
				this.ps.charPos = num;
			}
			this.curNode.SetValueNode(XmlNodeType.Text, this.stringBuilder.ToString());
			this.stringBuilder.Length = 0;
			return true;
		}

		private void ParseXmlDeclarationFragment()
		{
			try
			{
				this.ParseXmlDeclaration(false);
			}
			catch (XmlException ex)
			{
				this.ReThrow(ex, ex.LineNumber, ex.LinePosition - 6);
			}
		}

		private void ThrowUnexpectedToken(int pos, string expectedToken)
		{
			this.ThrowUnexpectedToken(pos, expectedToken, null);
		}

		private void ThrowUnexpectedToken(string expectedToken1)
		{
			this.ThrowUnexpectedToken(expectedToken1, null);
		}

		private void ThrowUnexpectedToken(int pos, string expectedToken1, string expectedToken2)
		{
			this.ps.charPos = pos;
			this.ThrowUnexpectedToken(expectedToken1, expectedToken2);
		}

		private void ThrowUnexpectedToken(string expectedToken1, string expectedToken2)
		{
			string text = this.ParseUnexpectedToken();
			if (text == null)
			{
				this.Throw("Unexpected end of file has occurred.");
			}
			if (expectedToken2 != null)
			{
				this.Throw("'{0}' is an unexpected token. The expected token is '{1}' or '{2}'.", new string[]
				{
					text,
					expectedToken1,
					expectedToken2
				});
				return;
			}
			this.Throw("'{0}' is an unexpected token. The expected token is '{1}'.", new string[]
			{
				text,
				expectedToken1
			});
		}

		private string ParseUnexpectedToken(int pos)
		{
			this.ps.charPos = pos;
			return this.ParseUnexpectedToken();
		}

		private string ParseUnexpectedToken()
		{
			if (this.ps.charPos == this.ps.charsUsed)
			{
				return null;
			}
			if (this.xmlCharType.IsNCNameSingleChar(this.ps.chars[this.ps.charPos]))
			{
				int num = this.ps.charPos + 1;
				while (this.xmlCharType.IsNCNameSingleChar(this.ps.chars[num]))
				{
					num++;
				}
				return new string(this.ps.chars, this.ps.charPos, num - this.ps.charPos);
			}
			return new string(this.ps.chars, this.ps.charPos, 1);
		}

		private void ThrowExpectingWhitespace(int pos)
		{
			string text = this.ParseUnexpectedToken(pos);
			if (text == null)
			{
				this.Throw(pos, "Unexpected end of file has occurred.");
				return;
			}
			this.Throw(pos, "'{0}' is an unexpected token. Expecting white space.", text);
		}

		private int GetIndexOfAttributeWithoutPrefix(string name)
		{
			name = this.nameTable.Get(name);
			if (name == null)
			{
				return -1;
			}
			for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
			{
				if (Ref.Equal(this.nodes[i].localName, name) && this.nodes[i].prefix.Length == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private int GetIndexOfAttributeWithPrefix(string name)
		{
			name = this.nameTable.Add(name);
			if (name == null)
			{
				return -1;
			}
			for (int i = this.index + 1; i < this.index + this.attrCount + 1; i++)
			{
				if (Ref.Equal(this.nodes[i].GetNameWPrefix(this.nameTable), name))
				{
					return i;
				}
			}
			return -1;
		}

		private bool ZeroEndingStream(int pos)
		{
			if (this.v1Compat && pos == this.ps.charsUsed - 1 && this.ps.chars[pos] == '\0' && this.ReadData() == 0 && this.ps.isStreamEof)
			{
				this.ps.charsUsed = this.ps.charsUsed - 1;
				return true;
			}
			return false;
		}

		private void ParseDtdFromParserContext()
		{
			IDtdParser dtdParser = DtdParser.Create();
			this.dtdInfo = dtdParser.ParseFreeFloatingDtd(this.fragmentParserContext.BaseURI, this.fragmentParserContext.DocTypeName, this.fragmentParserContext.PublicId, this.fragmentParserContext.SystemId, this.fragmentParserContext.InternalSubset, new XmlTextReaderImpl.DtdParserProxy(this));
			if ((this.validatingReaderCompatFlag || !this.v1Compat) && (this.dtdInfo.HasDefaultAttributes || this.dtdInfo.HasNonCDataAttributes))
			{
				this.addDefaultAttributesAndNormalize = true;
			}
		}

		private bool InitReadContentAsBinary()
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadValueChunk)
			{
				throw new InvalidOperationException(Res.GetString("ReadValueChunk calls cannot be mixed with ReadContentAsBase64 or ReadContentAsBinHex."));
			}
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InIncrementalRead)
			{
				throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadChars, ReadBase64, and ReadBinHex."));
			}
			if (!XmlReader.IsTextualNode(this.curNode.type) && !this.MoveToNextContentNode(false))
			{
				return false;
			}
			this.SetupReadContentAsBinaryState(XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary);
			this.incReadLineInfo.Set(this.curNode.LineNo, this.curNode.LinePos);
			return true;
		}

		private bool InitReadElementContentAsBinary()
		{
			bool isEmptyElement = this.curNode.IsEmptyElement;
			this.outerReader.Read();
			if (isEmptyElement)
			{
				return false;
			}
			if (!this.MoveToNextContentNode(false))
			{
				if (this.curNode.type != XmlNodeType.EndElement)
				{
					this.Throw("'{0}' is an invalid XmlNodeType.", this.curNode.type.ToString());
				}
				this.outerReader.Read();
				return false;
			}
			this.SetupReadContentAsBinaryState(XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary);
			this.incReadLineInfo.Set(this.curNode.LineNo, this.curNode.LinePos);
			return true;
		}

		private bool MoveToNextContentNode(bool moveIfOnContentNode)
		{
			for (;;)
			{
				switch (this.curNode.type)
				{
				case XmlNodeType.Attribute:
					goto IL_52;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if (!moveIfOnContentNode)
					{
						return true;
					}
					goto IL_6B;
				case XmlNodeType.EntityReference:
					this.outerReader.ResolveEntity();
					goto IL_6B;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.EndEntity:
					goto IL_6B;
				}
				break;
				IL_6B:
				moveIfOnContentNode = false;
				if (!this.outerReader.Read())
				{
					return false;
				}
			}
			return false;
			IL_52:
			return !moveIfOnContentNode;
		}

		private void SetupReadContentAsBinaryState(XmlTextReaderImpl.ParsingFunction inReadBinaryFunction)
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.PartialTextValue)
			{
				this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnPartialValue;
			}
			else
			{
				this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnCachedValue;
				this.nextNextParsingFunction = this.nextParsingFunction;
				this.nextParsingFunction = this.parsingFunction;
			}
			this.readValueOffset = 0;
			this.parsingFunction = inReadBinaryFunction;
		}

		private void SetupFromParserContext(XmlParserContext context, XmlReaderSettings settings)
		{
			XmlNameTable xmlNameTable = settings.NameTable;
			this.nameTableFromSettings = (xmlNameTable != null);
			if (context.NamespaceManager != null)
			{
				if (xmlNameTable != null && xmlNameTable != context.NamespaceManager.NameTable)
				{
					throw new XmlException("XmlReaderSettings.XmlNameTable must be the same name table as in XmlParserContext.NameTable or XmlParserContext.NamespaceManager.NameTable, or it must be null.");
				}
				this.namespaceManager = context.NamespaceManager;
				this.xmlContext.defaultNamespace = this.namespaceManager.LookupNamespace(string.Empty);
				xmlNameTable = this.namespaceManager.NameTable;
			}
			else if (context.NameTable != null)
			{
				if (xmlNameTable != null && xmlNameTable != context.NameTable)
				{
					throw new XmlException("XmlReaderSettings.XmlNameTable must be the same name table as in XmlParserContext.NameTable or XmlParserContext.NamespaceManager.NameTable, or it must be null.", string.Empty);
				}
				xmlNameTable = context.NameTable;
			}
			else if (xmlNameTable == null)
			{
				xmlNameTable = new NameTable();
			}
			this.nameTable = xmlNameTable;
			if (this.namespaceManager == null)
			{
				this.namespaceManager = new XmlNamespaceManager(xmlNameTable);
			}
			this.xmlContext.xmlSpace = context.XmlSpace;
			this.xmlContext.xmlLang = context.XmlLang;
		}

		internal override IDtdInfo DtdInfo
		{
			get
			{
				return this.dtdInfo;
			}
		}

		internal void SetDtdInfo(IDtdInfo newDtdInfo)
		{
			this.dtdInfo = newDtdInfo;
			if (this.dtdInfo != null && (this.validatingReaderCompatFlag || !this.v1Compat) && (this.dtdInfo.HasDefaultAttributes || this.dtdInfo.HasNonCDataAttributes))
			{
				this.addDefaultAttributesAndNormalize = true;
			}
		}

		internal IValidationEventHandling ValidationEventHandling
		{
			set
			{
				this.validationEventHandling = value;
			}
		}

		internal XmlTextReaderImpl.OnDefaultAttributeUseDelegate OnDefaultAttributeUse
		{
			set
			{
				this.onDefaultAttributeUse = value;
			}
		}

		internal bool XmlValidatingReaderCompatibilityMode
		{
			set
			{
				this.validatingReaderCompatFlag = value;
				if (value)
				{
					this.nameTable.Add("http://www.w3.org/2001/XMLSchema");
					this.nameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
					this.nameTable.Add("urn:schemas-microsoft-com:datatypes");
				}
			}
		}

		internal XmlNodeType FragmentType
		{
			get
			{
				return this.fragmentType;
			}
		}

		internal void ChangeCurrentNodeType(XmlNodeType newNodeType)
		{
			this.curNode.type = newNodeType;
		}

		internal XmlResolver GetResolver()
		{
			if (this.IsResolverNull)
			{
				return null;
			}
			return this.xmlResolver;
		}

		internal object InternalSchemaType
		{
			get
			{
				return this.curNode.schemaType;
			}
			set
			{
				this.curNode.schemaType = value;
			}
		}

		internal object InternalTypedValue
		{
			get
			{
				return this.curNode.typedValue;
			}
			set
			{
				this.curNode.typedValue = value;
			}
		}

		internal bool StandAlone
		{
			get
			{
				return this.standalone;
			}
		}

		internal override XmlNamespaceManager NamespaceManager
		{
			get
			{
				return this.namespaceManager;
			}
		}

		internal bool V1Compat
		{
			get
			{
				return this.v1Compat;
			}
		}

		internal ConformanceLevel V1ComformanceLevel
		{
			get
			{
				if (this.fragmentType != XmlNodeType.Element)
				{
					return ConformanceLevel.Document;
				}
				return ConformanceLevel.Fragment;
			}
		}

		private bool AddDefaultAttributeDtd(IDtdDefaultAttributeInfo defAttrInfo, bool definedInDtd, XmlTextReaderImpl.NodeData[] nameSortedNodeData)
		{
			if (defAttrInfo.Prefix.Length > 0)
			{
				this.attrNeedNamespaceLookup = true;
			}
			string localName = defAttrInfo.LocalName;
			string prefix = defAttrInfo.Prefix;
			if (nameSortedNodeData != null)
			{
				if (Array.BinarySearch<object>(nameSortedNodeData, defAttrInfo, XmlTextReaderImpl.DtdDefaultAttributeInfoToNodeDataComparer.Instance) >= 0)
				{
					return false;
				}
			}
			else
			{
				for (int i = this.index + 1; i < this.index + 1 + this.attrCount; i++)
				{
					if (this.nodes[i].localName == localName && this.nodes[i].prefix == prefix)
					{
						return false;
					}
				}
			}
			XmlTextReaderImpl.NodeData nodeData = this.AddDefaultAttributeInternal(defAttrInfo.LocalName, null, defAttrInfo.Prefix, defAttrInfo.DefaultValueExpanded, defAttrInfo.LineNumber, defAttrInfo.LinePosition, defAttrInfo.ValueLineNumber, defAttrInfo.ValueLinePosition, defAttrInfo.IsXmlAttribute);
			if (this.DtdValidation)
			{
				if (this.onDefaultAttributeUse != null)
				{
					this.onDefaultAttributeUse(defAttrInfo, this);
				}
				nodeData.typedValue = defAttrInfo.DefaultValueTyped;
			}
			return nodeData != null;
		}

		internal bool AddDefaultAttributeNonDtd(SchemaAttDef attrDef)
		{
			string text = this.nameTable.Add(attrDef.Name.Name);
			string text2 = this.nameTable.Add(attrDef.Prefix);
			string text3 = this.nameTable.Add(attrDef.Name.Namespace);
			if (text2.Length == 0 && text3.Length > 0)
			{
				text2 = this.namespaceManager.LookupPrefix(text3);
				if (text2 == null)
				{
					text2 = string.Empty;
				}
			}
			for (int i = this.index + 1; i < this.index + 1 + this.attrCount; i++)
			{
				if (this.nodes[i].localName == text && (this.nodes[i].prefix == text2 || (this.nodes[i].ns == text3 && text3 != null)))
				{
					return false;
				}
			}
			XmlTextReaderImpl.NodeData nodeData = this.AddDefaultAttributeInternal(text, text3, text2, attrDef.DefaultValueExpanded, attrDef.LineNumber, attrDef.LinePosition, attrDef.ValueLineNumber, attrDef.ValueLinePosition, attrDef.Reserved > SchemaAttDef.Reserve.None);
			nodeData.schemaType = ((attrDef.SchemaType == null) ? attrDef.Datatype : attrDef.SchemaType);
			nodeData.typedValue = attrDef.DefaultValueTyped;
			return true;
		}

		private XmlTextReaderImpl.NodeData AddDefaultAttributeInternal(string localName, string ns, string prefix, string value, int lineNo, int linePos, int valueLineNo, int valueLinePos, bool isXmlAttribute)
		{
			XmlTextReaderImpl.NodeData nodeData = this.AddAttribute(localName, prefix, (prefix.Length > 0) ? null : localName);
			if (ns != null)
			{
				nodeData.ns = ns;
			}
			nodeData.SetValue(value);
			nodeData.IsDefaultAttribute = true;
			nodeData.lineInfo.Set(lineNo, linePos);
			nodeData.lineInfo2.Set(valueLineNo, valueLinePos);
			if (nodeData.prefix.Length == 0)
			{
				if (Ref.Equal(nodeData.localName, this.XmlNs))
				{
					this.OnDefaultNamespaceDecl(nodeData);
					if (!this.attrNeedNamespaceLookup && this.nodes[this.index].prefix.Length == 0)
					{
						this.nodes[this.index].ns = this.xmlContext.defaultNamespace;
					}
				}
			}
			else if (Ref.Equal(nodeData.prefix, this.XmlNs))
			{
				this.OnNamespaceDecl(nodeData);
				if (!this.attrNeedNamespaceLookup)
				{
					string localName2 = nodeData.localName;
					for (int i = this.index; i < this.index + this.attrCount + 1; i++)
					{
						if (this.nodes[i].prefix.Equals(localName2))
						{
							this.nodes[i].ns = this.namespaceManager.LookupNamespace(localName2);
						}
					}
				}
			}
			else if (isXmlAttribute)
			{
				this.OnXmlReservedAttribute(nodeData);
			}
			this.fullAttrCleanup = true;
			return nodeData;
		}

		internal bool DisableUndeclaredEntityCheck
		{
			set
			{
				this.disableUndeclaredEntityCheck = value;
			}
		}

		private int ReadContentAsBinary(byte[] buffer, int index, int count)
		{
			if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_End)
			{
				return 0;
			}
			this.incReadDecoder.SetNextOutputBuffer(buffer, index, count);
			int num;
			int num2;
			int num3;
			XmlTextReaderImpl.ParsingFunction inReadBinaryFunction;
			for (;;)
			{
				num = 0;
				try
				{
					num = this.curNode.CopyToBinary(this.incReadDecoder, this.readValueOffset);
				}
				catch (XmlException e)
				{
					this.curNode.AdjustLineInfo(this.readValueOffset, this.ps.eolNormalized, ref this.incReadLineInfo);
					this.ReThrow(e, this.incReadLineInfo.lineNo, this.incReadLineInfo.linePos);
				}
				this.readValueOffset += num;
				if (this.incReadDecoder.IsFull)
				{
					break;
				}
				if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnPartialValue)
				{
					this.curNode.SetValue(string.Empty);
					bool flag = false;
					num2 = 0;
					num3 = 0;
					while (!this.incReadDecoder.IsFull && !flag)
					{
						int num4 = 0;
						this.incReadLineInfo.Set(this.ps.LineNo, this.ps.LinePos);
						flag = this.ParseText(out num2, out num3, ref num4);
						try
						{
							num = this.incReadDecoder.Decode(this.ps.chars, num2, num3 - num2);
						}
						catch (XmlException e2)
						{
							this.ReThrow(e2, this.incReadLineInfo.lineNo, this.incReadLineInfo.linePos);
						}
						num2 += num;
					}
					this.incReadState = (flag ? XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnCachedValue : XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_OnPartialValue);
					this.readValueOffset = 0;
					if (this.incReadDecoder.IsFull)
					{
						goto Block_8;
					}
				}
				inReadBinaryFunction = this.parsingFunction;
				this.parsingFunction = this.nextParsingFunction;
				this.nextParsingFunction = this.nextNextParsingFunction;
				if (!this.MoveToNextContentNode(true))
				{
					goto Block_9;
				}
				this.SetupReadContentAsBinaryState(inReadBinaryFunction);
				this.incReadLineInfo.Set(this.curNode.LineNo, this.curNode.LinePos);
			}
			return this.incReadDecoder.DecodedCount;
			Block_8:
			this.curNode.SetValue(this.ps.chars, num2, num3 - num2);
			XmlTextReaderImpl.AdjustLineInfo(this.ps.chars, num2 - num, num2, this.ps.eolNormalized, ref this.incReadLineInfo);
			this.curNode.SetLineInfo(this.incReadLineInfo.lineNo, this.incReadLineInfo.linePos);
			return this.incReadDecoder.DecodedCount;
			Block_9:
			this.SetupReadContentAsBinaryState(inReadBinaryFunction);
			this.incReadState = XmlTextReaderImpl.IncrementalReadState.ReadContentAsBinary_End;
			return this.incReadDecoder.DecodedCount;
		}

		private int ReadElementContentAsBinary(byte[] buffer, int index, int count)
		{
			if (count == 0)
			{
				return 0;
			}
			int num = this.ReadContentAsBinary(buffer, index, count);
			if (num > 0)
			{
				return num;
			}
			if (this.curNode.type != XmlNodeType.EndElement)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.curNode.type.ToString(), this);
			}
			this.parsingFunction = this.nextParsingFunction;
			this.nextParsingFunction = this.nextNextParsingFunction;
			this.outerReader.Read();
			return 0;
		}

		private void InitBase64Decoder()
		{
			if (this.base64Decoder == null)
			{
				this.base64Decoder = new Base64Decoder();
			}
			else
			{
				this.base64Decoder.Reset();
			}
			this.incReadDecoder = this.base64Decoder;
		}

		private void InitBinHexDecoder()
		{
			if (this.binHexDecoder == null)
			{
				this.binHexDecoder = new BinHexDecoder();
			}
			else
			{
				this.binHexDecoder.Reset();
			}
			this.incReadDecoder = this.binHexDecoder;
		}

		private bool UriEqual(Uri uri1, string uri1Str, string uri2Str, XmlResolver resolver)
		{
			if (resolver == null)
			{
				return uri1Str == uri2Str;
			}
			if (uri1 == null)
			{
				uri1 = resolver.ResolveUri(null, uri1Str);
			}
			Uri obj = resolver.ResolveUri(null, uri2Str);
			return uri1.Equals(obj);
		}

		private void RegisterConsumedCharacters(long characters, bool inEntityReference)
		{
			if (this.maxCharactersInDocument > 0L)
			{
				long num = this.charactersInDocument + characters;
				if (num < this.charactersInDocument)
				{
					this.ThrowWithoutLineInfo("The input document has exceeded a limit set by {0}.", "MaxCharactersInDocument");
				}
				else
				{
					this.charactersInDocument = num;
				}
				if (this.charactersInDocument > this.maxCharactersInDocument)
				{
					this.ThrowWithoutLineInfo("The input document has exceeded a limit set by {0}.", "MaxCharactersInDocument");
				}
			}
			if (this.maxCharactersFromEntities > 0L && inEntityReference)
			{
				long num2 = this.charactersFromEntities + characters;
				if (num2 < this.charactersFromEntities)
				{
					this.ThrowWithoutLineInfo("The input document has exceeded a limit set by {0}.", "MaxCharactersFromEntities");
				}
				else
				{
					this.charactersFromEntities = num2;
				}
				if (this.charactersFromEntities > this.maxCharactersFromEntities)
				{
					this.ThrowWithoutLineInfo("The input document has exceeded a limit set by {0}.", "MaxCharactersFromEntities");
				}
			}
		}

		internal unsafe static void AdjustLineInfo(char[] chars, int startPos, int endPos, bool isNormalized, ref LineInfo lineInfo)
		{
			fixed (char* ptr = &chars[startPos])
			{
				XmlTextReaderImpl.AdjustLineInfo(ptr, endPos - startPos, isNormalized, ref lineInfo);
			}
		}

		internal unsafe static void AdjustLineInfo(string str, int startPos, int endPos, bool isNormalized, ref LineInfo lineInfo)
		{
			fixed (string text = str)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				XmlTextReaderImpl.AdjustLineInfo(ptr + startPos, endPos - startPos, isNormalized, ref lineInfo);
			}
		}

		internal unsafe static void AdjustLineInfo(char* pChars, int length, bool isNormalized, ref LineInfo lineInfo)
		{
			int num = -1;
			for (int i = 0; i < length; i++)
			{
				char c = pChars[i];
				if (c != '\n')
				{
					if (c == '\r')
					{
						if (!isNormalized)
						{
							lineInfo.lineNo++;
							num = i;
							if (i + 1 < length && pChars[i + 1] == '\n')
							{
								i++;
								num++;
							}
						}
					}
				}
				else
				{
					lineInfo.lineNo++;
					num = i;
				}
			}
			if (num >= 0)
			{
				lineInfo.linePos = length - num;
			}
		}

		internal static string StripSpaces(string value)
		{
			int length = value.Length;
			if (length <= 0)
			{
				return string.Empty;
			}
			int num = 0;
			StringBuilder stringBuilder = null;
			while (value[num] == ' ')
			{
				num++;
				if (num == length)
				{
					return " ";
				}
			}
			int i;
			for (i = num; i < length; i++)
			{
				if (value[i] == ' ')
				{
					int num2 = i + 1;
					while (num2 < length && value[num2] == ' ')
					{
						num2++;
					}
					if (num2 == length)
					{
						if (stringBuilder == null)
						{
							return value.Substring(num, i - num);
						}
						stringBuilder.Append(value, num, i - num);
						return stringBuilder.ToString();
					}
					else if (num2 > i + 1)
					{
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder(length);
						}
						stringBuilder.Append(value, num, i - num + 1);
						num = num2;
						i = num2 - 1;
					}
				}
			}
			if (stringBuilder != null)
			{
				if (i > num)
				{
					stringBuilder.Append(value, num, i - num);
				}
				return stringBuilder.ToString();
			}
			if (num != 0)
			{
				return value.Substring(num, length - num);
			}
			return value;
		}

		internal static void StripSpaces(char[] value, int index, ref int len)
		{
			if (len <= 0)
			{
				return;
			}
			int num = index;
			int num2 = index + len;
			while (value[num] == ' ')
			{
				num++;
				if (num == num2)
				{
					len = 1;
					return;
				}
			}
			int num3 = num - index;
			for (int i = num; i < num2; i++)
			{
				char c;
				if ((c = value[i]) == ' ')
				{
					int num4 = i + 1;
					while (num4 < num2 && value[num4] == ' ')
					{
						num4++;
					}
					if (num4 == num2)
					{
						num3 += num4 - i;
						break;
					}
					if (num4 > i + 1)
					{
						num3 += num4 - i - 1;
						i = num4 - 1;
					}
				}
				value[i - num3] = c;
			}
			len -= num3;
		}

		internal static void BlockCopyChars(char[] src, int srcOffset, char[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset * 2, dst, dstOffset * 2, count * 2);
		}

		internal static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
		{
			Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);
		}

		private void CheckAsyncCall()
		{
			if (!this.useAsync)
			{
				throw new InvalidOperationException(Res.GetString("Set XmlReaderSettings.Async to true if you want to use Async Methods."));
			}
		}

		public override Task<string> GetValueAsync()
		{
			this.CheckAsyncCall();
			if (this.parsingFunction >= XmlTextReaderImpl.ParsingFunction.PartialTextValue)
			{
				return this._GetValueAsync();
			}
			return Task.FromResult<string>(this.curNode.StringValue);
		}

		private Task<string> _GetValueAsync()
		{
			XmlTextReaderImpl.<_GetValueAsync>d__473 <_GetValueAsync>d__;
			<_GetValueAsync>d__.<>4__this = this;
			<_GetValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<_GetValueAsync>d__.<>1__state = -1;
			<_GetValueAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<_GetValueAsync>d__473>(ref <_GetValueAsync>d__);
			return <_GetValueAsync>d__.<>t__builder.Task;
		}

		private Task FinishInitAsync()
		{
			switch (this.laterInitParam.initType)
			{
			case XmlTextReaderImpl.InitInputType.UriString:
				return this.FinishInitUriStringAsync();
			case XmlTextReaderImpl.InitInputType.Stream:
				return this.FinishInitStreamAsync();
			case XmlTextReaderImpl.InitInputType.TextReader:
				return this.FinishInitTextReaderAsync();
			default:
				return AsyncHelper.DoneTask;
			}
		}

		private Task FinishInitUriStringAsync()
		{
			XmlTextReaderImpl.<FinishInitUriStringAsync>d__475 <FinishInitUriStringAsync>d__;
			<FinishInitUriStringAsync>d__.<>4__this = this;
			<FinishInitUriStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishInitUriStringAsync>d__.<>1__state = -1;
			<FinishInitUriStringAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishInitUriStringAsync>d__475>(ref <FinishInitUriStringAsync>d__);
			return <FinishInitUriStringAsync>d__.<>t__builder.Task;
		}

		private Task FinishInitStreamAsync()
		{
			XmlTextReaderImpl.<FinishInitStreamAsync>d__476 <FinishInitStreamAsync>d__;
			<FinishInitStreamAsync>d__.<>4__this = this;
			<FinishInitStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishInitStreamAsync>d__.<>1__state = -1;
			<FinishInitStreamAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishInitStreamAsync>d__476>(ref <FinishInitStreamAsync>d__);
			return <FinishInitStreamAsync>d__.<>t__builder.Task;
		}

		private Task FinishInitTextReaderAsync()
		{
			XmlTextReaderImpl.<FinishInitTextReaderAsync>d__477 <FinishInitTextReaderAsync>d__;
			<FinishInitTextReaderAsync>d__.<>4__this = this;
			<FinishInitTextReaderAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishInitTextReaderAsync>d__.<>1__state = -1;
			<FinishInitTextReaderAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishInitTextReaderAsync>d__477>(ref <FinishInitTextReaderAsync>d__);
			return <FinishInitTextReaderAsync>d__.<>t__builder.Task;
		}

		public override Task<bool> ReadAsync()
		{
			this.CheckAsyncCall();
			if (this.laterInitParam != null)
			{
				return this.FinishInitAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
			}
			for (;;)
			{
				switch (this.parsingFunction)
				{
				case XmlTextReaderImpl.ParsingFunction.ElementContent:
					goto IL_9E;
				case XmlTextReaderImpl.ParsingFunction.NoData:
					goto IL_2DC;
				case XmlTextReaderImpl.ParsingFunction.SwitchToInteractive:
					this.readState = ReadState.Interactive;
					this.parsingFunction = this.nextParsingFunction;
					break;
				case XmlTextReaderImpl.ParsingFunction.SwitchToInteractiveXmlDecl:
					goto IL_C4;
				case XmlTextReaderImpl.ParsingFunction.DocumentContent:
					goto IL_A5;
				case XmlTextReaderImpl.ParsingFunction.MoveToElementContent:
					this.ResetAttributes();
					this.index++;
					this.curNode = this.AddNode(this.index, this.index);
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.ElementContent;
					break;
				case XmlTextReaderImpl.ParsingFunction.PopElementContext:
					this.PopElementContext();
					this.parsingFunction = this.nextParsingFunction;
					break;
				case XmlTextReaderImpl.ParsingFunction.PopEmptyElementContext:
					this.curNode = this.nodes[this.index];
					this.curNode.IsEmptyElement = false;
					this.ResetAttributes();
					this.PopElementContext();
					this.parsingFunction = this.nextParsingFunction;
					break;
				case XmlTextReaderImpl.ParsingFunction.ResetAttributesRootLevel:
					this.ResetAttributes();
					this.curNode = this.nodes[this.index];
					this.parsingFunction = ((this.index == 0) ? XmlTextReaderImpl.ParsingFunction.DocumentContent : XmlTextReaderImpl.ParsingFunction.ElementContent);
					break;
				case XmlTextReaderImpl.ParsingFunction.Error:
				case XmlTextReaderImpl.ParsingFunction.Eof:
				case XmlTextReaderImpl.ParsingFunction.ReaderClosed:
					goto IL_2D6;
				case XmlTextReaderImpl.ParsingFunction.EntityReference:
					goto IL_186;
				case XmlTextReaderImpl.ParsingFunction.InIncrementalRead:
					goto IL_29E;
				case XmlTextReaderImpl.ParsingFunction.FragmentAttribute:
					goto IL_2AA;
				case XmlTextReaderImpl.ParsingFunction.ReportEndEntity:
					goto IL_19F;
				case XmlTextReaderImpl.ParsingFunction.AfterResolveEntityInContent:
					this.curNode = this.AddNode(this.index, this.index);
					this.reportedEncoding = this.ps.encoding;
					this.reportedBaseUri = this.ps.baseUriStr;
					this.parsingFunction = this.nextParsingFunction;
					break;
				case XmlTextReaderImpl.ParsingFunction.AfterResolveEmptyEntityInContent:
					goto IL_202;
				case XmlTextReaderImpl.ParsingFunction.XmlDeclarationFragment:
					goto IL_2B6;
				case XmlTextReaderImpl.ParsingFunction.GoToEof:
					goto IL_2CA;
				case XmlTextReaderImpl.ParsingFunction.PartialTextValue:
					goto IL_2ED;
				case XmlTextReaderImpl.ParsingFunction.InReadAttributeValue:
					this.FinishAttributeValueIterator();
					this.curNode = this.nodes[this.index];
					break;
				case XmlTextReaderImpl.ParsingFunction.InReadValueChunk:
					goto IL_306;
				case XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary:
					goto IL_31F;
				case XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary:
					goto IL_338;
				}
			}
			IL_9E:
			return this.ParseElementContentAsync();
			IL_A5:
			return this.ParseDocumentContentAsync();
			IL_C4:
			return this.ReadAsync_SwitchToInteractiveXmlDecl();
			IL_186:
			this.parsingFunction = this.nextParsingFunction;
			return this.ParseEntityReferenceAsync().ReturnTaskBoolWhenFinish(true);
			IL_19F:
			this.SetupEndEntityNodeInContent();
			this.parsingFunction = this.nextParsingFunction;
			return AsyncHelper.DoneTaskTrue;
			IL_202:
			this.curNode = this.AddNode(this.index, this.index);
			this.curNode.SetValueNode(XmlNodeType.Text, string.Empty);
			this.curNode.SetLineInfo(this.ps.lineNo, this.ps.LinePos);
			this.reportedEncoding = this.ps.encoding;
			this.reportedBaseUri = this.ps.baseUriStr;
			this.parsingFunction = this.nextParsingFunction;
			return AsyncHelper.DoneTaskTrue;
			IL_29E:
			this.FinishIncrementalRead();
			return AsyncHelper.DoneTaskTrue;
			IL_2AA:
			return Task.FromResult<bool>(this.ParseFragmentAttribute());
			IL_2B6:
			this.ParseXmlDeclarationFragment();
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.GoToEof;
			return AsyncHelper.DoneTaskTrue;
			IL_2CA:
			this.OnEof();
			return AsyncHelper.DoneTaskFalse;
			IL_2D6:
			return AsyncHelper.DoneTaskFalse;
			IL_2DC:
			this.ThrowWithoutLineInfo("Root element is missing.");
			return AsyncHelper.DoneTaskFalse;
			IL_2ED:
			return this.SkipPartialTextValueAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
			IL_306:
			return this.FinishReadValueChunkAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
			IL_31F:
			return this.FinishReadContentAsBinaryAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
			IL_338:
			return this.FinishReadElementContentAsBinaryAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ReadAsync));
		}

		private Task<bool> ReadAsync_SwitchToInteractiveXmlDecl()
		{
			this.readState = ReadState.Interactive;
			this.parsingFunction = this.nextParsingFunction;
			Task<bool> task = this.ParseXmlDeclarationAsync(false);
			if (task.IsSuccess())
			{
				return this.ReadAsync_SwitchToInteractiveXmlDecl_Helper(task.Result);
			}
			return this._ReadAsync_SwitchToInteractiveXmlDecl(task);
		}

		private Task<bool> _ReadAsync_SwitchToInteractiveXmlDecl(Task<bool> task)
		{
			XmlTextReaderImpl.<_ReadAsync_SwitchToInteractiveXmlDecl>d__480 <_ReadAsync_SwitchToInteractiveXmlDecl>d__;
			<_ReadAsync_SwitchToInteractiveXmlDecl>d__.<>4__this = this;
			<_ReadAsync_SwitchToInteractiveXmlDecl>d__.task = task;
			<_ReadAsync_SwitchToInteractiveXmlDecl>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ReadAsync_SwitchToInteractiveXmlDecl>d__.<>1__state = -1;
			<_ReadAsync_SwitchToInteractiveXmlDecl>d__.<>t__builder.Start<XmlTextReaderImpl.<_ReadAsync_SwitchToInteractiveXmlDecl>d__480>(ref <_ReadAsync_SwitchToInteractiveXmlDecl>d__);
			return <_ReadAsync_SwitchToInteractiveXmlDecl>d__.<>t__builder.Task;
		}

		private Task<bool> ReadAsync_SwitchToInteractiveXmlDecl_Helper(bool finish)
		{
			if (finish)
			{
				this.reportedEncoding = this.ps.encoding;
				return AsyncHelper.DoneTaskTrue;
			}
			this.reportedEncoding = this.ps.encoding;
			return this.ReadAsync();
		}

		public override Task SkipAsync()
		{
			XmlTextReaderImpl.<SkipAsync>d__482 <SkipAsync>d__;
			<SkipAsync>d__.<>4__this = this;
			<SkipAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SkipAsync>d__.<>1__state = -1;
			<SkipAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<SkipAsync>d__482>(ref <SkipAsync>d__);
			return <SkipAsync>d__.<>t__builder.Task;
		}

		private Task<int> ReadContentAsBase64_AsyncHelper(Task<bool> task, byte[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadContentAsBase64_AsyncHelper>d__483 <ReadContentAsBase64_AsyncHelper>d__;
			<ReadContentAsBase64_AsyncHelper>d__.<>4__this = this;
			<ReadContentAsBase64_AsyncHelper>d__.task = task;
			<ReadContentAsBase64_AsyncHelper>d__.buffer = buffer;
			<ReadContentAsBase64_AsyncHelper>d__.index = index;
			<ReadContentAsBase64_AsyncHelper>d__.count = count;
			<ReadContentAsBase64_AsyncHelper>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBase64_AsyncHelper>d__.<>1__state = -1;
			<ReadContentAsBase64_AsyncHelper>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadContentAsBase64_AsyncHelper>d__483>(ref <ReadContentAsBase64_AsyncHelper>d__);
			return <ReadContentAsBase64_AsyncHelper>d__.<>t__builder.Task;
		}

		public override Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			this.CheckAsyncCall();
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
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
			{
				if (this.incReadDecoder == this.base64Decoder)
				{
					return this.ReadContentAsBinaryAsync(buffer, index, count);
				}
			}
			else
			{
				if (this.readState != ReadState.Interactive)
				{
					return AsyncHelper.DoneTaskZero;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary)
				{
					throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
				}
				if (!XmlReader.CanReadContentAs(this.curNode.type))
				{
					throw base.CreateReadContentAsException("ReadContentAsBase64");
				}
				Task<bool> task = this.InitReadContentAsBinaryAsync();
				if (!task.IsSuccess())
				{
					return this.ReadContentAsBase64_AsyncHelper(task, buffer, index, count);
				}
				if (!task.Result)
				{
					return AsyncHelper.DoneTaskZero;
				}
			}
			this.InitBase64Decoder();
			return this.ReadContentAsBinaryAsync(buffer, index, count);
		}

		public override Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadContentAsBinHexAsync>d__485 <ReadContentAsBinHexAsync>d__;
			<ReadContentAsBinHexAsync>d__.<>4__this = this;
			<ReadContentAsBinHexAsync>d__.buffer = buffer;
			<ReadContentAsBinHexAsync>d__.index = index;
			<ReadContentAsBinHexAsync>d__.count = count;
			<ReadContentAsBinHexAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBinHexAsync>d__.<>1__state = -1;
			<ReadContentAsBinHexAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadContentAsBinHexAsync>d__485>(ref <ReadContentAsBinHexAsync>d__);
			return <ReadContentAsBinHexAsync>d__.<>t__builder.Task;
		}

		private Task<int> ReadElementContentAsBase64Async_Helper(Task<bool> task, byte[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadElementContentAsBase64Async_Helper>d__486 <ReadElementContentAsBase64Async_Helper>d__;
			<ReadElementContentAsBase64Async_Helper>d__.<>4__this = this;
			<ReadElementContentAsBase64Async_Helper>d__.task = task;
			<ReadElementContentAsBase64Async_Helper>d__.buffer = buffer;
			<ReadElementContentAsBase64Async_Helper>d__.index = index;
			<ReadElementContentAsBase64Async_Helper>d__.count = count;
			<ReadElementContentAsBase64Async_Helper>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBase64Async_Helper>d__.<>1__state = -1;
			<ReadElementContentAsBase64Async_Helper>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadElementContentAsBase64Async_Helper>d__486>(ref <ReadElementContentAsBase64Async_Helper>d__);
			return <ReadElementContentAsBase64Async_Helper>d__.<>t__builder.Task;
		}

		public override Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			this.CheckAsyncCall();
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
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadElementContentAsBinary)
			{
				if (this.incReadDecoder == this.base64Decoder)
				{
					return this.ReadElementContentAsBinaryAsync(buffer, index, count);
				}
			}
			else
			{
				if (this.readState != ReadState.Interactive)
				{
					return AsyncHelper.DoneTaskZero;
				}
				if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.InReadContentAsBinary)
				{
					throw new InvalidOperationException(Res.GetString("ReadContentAsBase64 and ReadContentAsBinHex method calls cannot be mixed with calls to ReadElementContentAsBase64 and ReadElementContentAsBinHex."));
				}
				if (this.curNode.type != XmlNodeType.Element)
				{
					throw base.CreateReadElementContentAsException("ReadElementContentAsBinHex");
				}
				Task<bool> task = this.InitReadElementContentAsBinaryAsync();
				if (!task.IsSuccess())
				{
					return this.ReadElementContentAsBase64Async_Helper(task, buffer, index, count);
				}
				if (!task.Result)
				{
					return AsyncHelper.DoneTaskZero;
				}
			}
			this.InitBase64Decoder();
			return this.ReadElementContentAsBinaryAsync(buffer, index, count);
		}

		public override Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadElementContentAsBinHexAsync>d__488 <ReadElementContentAsBinHexAsync>d__;
			<ReadElementContentAsBinHexAsync>d__.<>4__this = this;
			<ReadElementContentAsBinHexAsync>d__.buffer = buffer;
			<ReadElementContentAsBinHexAsync>d__.index = index;
			<ReadElementContentAsBinHexAsync>d__.count = count;
			<ReadElementContentAsBinHexAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBinHexAsync>d__.<>1__state = -1;
			<ReadElementContentAsBinHexAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadElementContentAsBinHexAsync>d__488>(ref <ReadElementContentAsBinHexAsync>d__);
			return <ReadElementContentAsBinHexAsync>d__.<>t__builder.Task;
		}

		public override Task<int> ReadValueChunkAsync(char[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadValueChunkAsync>d__489 <ReadValueChunkAsync>d__;
			<ReadValueChunkAsync>d__.<>4__this = this;
			<ReadValueChunkAsync>d__.buffer = buffer;
			<ReadValueChunkAsync>d__.index = index;
			<ReadValueChunkAsync>d__.count = count;
			<ReadValueChunkAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadValueChunkAsync>d__.<>1__state = -1;
			<ReadValueChunkAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadValueChunkAsync>d__489>(ref <ReadValueChunkAsync>d__);
			return <ReadValueChunkAsync>d__.<>t__builder.Task;
		}

		internal Task<int> DtdParserProxy_ReadDataAsync()
		{
			this.CheckAsyncCall();
			return this.ReadDataAsync();
		}

		internal Task<int> DtdParserProxy_ParseNumericCharRefAsync(StringBuilder internalSubsetBuilder)
		{
			XmlTextReaderImpl.<DtdParserProxy_ParseNumericCharRefAsync>d__491 <DtdParserProxy_ParseNumericCharRefAsync>d__;
			<DtdParserProxy_ParseNumericCharRefAsync>d__.<>4__this = this;
			<DtdParserProxy_ParseNumericCharRefAsync>d__.internalSubsetBuilder = internalSubsetBuilder;
			<DtdParserProxy_ParseNumericCharRefAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<DtdParserProxy_ParseNumericCharRefAsync>d__.<>1__state = -1;
			<DtdParserProxy_ParseNumericCharRefAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<DtdParserProxy_ParseNumericCharRefAsync>d__491>(ref <DtdParserProxy_ParseNumericCharRefAsync>d__);
			return <DtdParserProxy_ParseNumericCharRefAsync>d__.<>t__builder.Task;
		}

		internal Task<int> DtdParserProxy_ParseNamedCharRefAsync(bool expand, StringBuilder internalSubsetBuilder)
		{
			this.CheckAsyncCall();
			return this.ParseNamedCharRefAsync(expand, internalSubsetBuilder);
		}

		internal Task DtdParserProxy_ParsePIAsync(StringBuilder sb)
		{
			XmlTextReaderImpl.<DtdParserProxy_ParsePIAsync>d__493 <DtdParserProxy_ParsePIAsync>d__;
			<DtdParserProxy_ParsePIAsync>d__.<>4__this = this;
			<DtdParserProxy_ParsePIAsync>d__.sb = sb;
			<DtdParserProxy_ParsePIAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DtdParserProxy_ParsePIAsync>d__.<>1__state = -1;
			<DtdParserProxy_ParsePIAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<DtdParserProxy_ParsePIAsync>d__493>(ref <DtdParserProxy_ParsePIAsync>d__);
			return <DtdParserProxy_ParsePIAsync>d__.<>t__builder.Task;
		}

		internal Task DtdParserProxy_ParseCommentAsync(StringBuilder sb)
		{
			XmlTextReaderImpl.<DtdParserProxy_ParseCommentAsync>d__494 <DtdParserProxy_ParseCommentAsync>d__;
			<DtdParserProxy_ParseCommentAsync>d__.<>4__this = this;
			<DtdParserProxy_ParseCommentAsync>d__.sb = sb;
			<DtdParserProxy_ParseCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DtdParserProxy_ParseCommentAsync>d__.<>1__state = -1;
			<DtdParserProxy_ParseCommentAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<DtdParserProxy_ParseCommentAsync>d__494>(ref <DtdParserProxy_ParseCommentAsync>d__);
			return <DtdParserProxy_ParseCommentAsync>d__.<>t__builder.Task;
		}

		internal Task<Tuple<int, bool>> DtdParserProxy_PushEntityAsync(IDtdEntityInfo entity)
		{
			XmlTextReaderImpl.<DtdParserProxy_PushEntityAsync>d__495 <DtdParserProxy_PushEntityAsync>d__;
			<DtdParserProxy_PushEntityAsync>d__.<>4__this = this;
			<DtdParserProxy_PushEntityAsync>d__.entity = entity;
			<DtdParserProxy_PushEntityAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, bool>>.Create();
			<DtdParserProxy_PushEntityAsync>d__.<>1__state = -1;
			<DtdParserProxy_PushEntityAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<DtdParserProxy_PushEntityAsync>d__495>(ref <DtdParserProxy_PushEntityAsync>d__);
			return <DtdParserProxy_PushEntityAsync>d__.<>t__builder.Task;
		}

		internal Task<bool> DtdParserProxy_PushExternalSubsetAsync(string systemId, string publicId)
		{
			XmlTextReaderImpl.<DtdParserProxy_PushExternalSubsetAsync>d__496 <DtdParserProxy_PushExternalSubsetAsync>d__;
			<DtdParserProxy_PushExternalSubsetAsync>d__.<>4__this = this;
			<DtdParserProxy_PushExternalSubsetAsync>d__.systemId = systemId;
			<DtdParserProxy_PushExternalSubsetAsync>d__.publicId = publicId;
			<DtdParserProxy_PushExternalSubsetAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<DtdParserProxy_PushExternalSubsetAsync>d__.<>1__state = -1;
			<DtdParserProxy_PushExternalSubsetAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<DtdParserProxy_PushExternalSubsetAsync>d__496>(ref <DtdParserProxy_PushExternalSubsetAsync>d__);
			return <DtdParserProxy_PushExternalSubsetAsync>d__.<>t__builder.Task;
		}

		private Task InitStreamInputAsync(Uri baseUri, Stream stream, Encoding encoding)
		{
			return this.InitStreamInputAsync(baseUri, baseUri.ToString(), stream, null, 0, encoding);
		}

		private Task InitStreamInputAsync(Uri baseUri, string baseUriStr, Stream stream, Encoding encoding)
		{
			return this.InitStreamInputAsync(baseUri, baseUriStr, stream, null, 0, encoding);
		}

		private Task InitStreamInputAsync(Uri baseUri, string baseUriStr, Stream stream, byte[] bytes, int byteCount, Encoding encoding)
		{
			XmlTextReaderImpl.<InitStreamInputAsync>d__499 <InitStreamInputAsync>d__;
			<InitStreamInputAsync>d__.<>4__this = this;
			<InitStreamInputAsync>d__.baseUri = baseUri;
			<InitStreamInputAsync>d__.baseUriStr = baseUriStr;
			<InitStreamInputAsync>d__.stream = stream;
			<InitStreamInputAsync>d__.bytes = bytes;
			<InitStreamInputAsync>d__.byteCount = byteCount;
			<InitStreamInputAsync>d__.encoding = encoding;
			<InitStreamInputAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<InitStreamInputAsync>d__.<>1__state = -1;
			<InitStreamInputAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<InitStreamInputAsync>d__499>(ref <InitStreamInputAsync>d__);
			return <InitStreamInputAsync>d__.<>t__builder.Task;
		}

		private Task InitTextReaderInputAsync(string baseUriStr, TextReader input)
		{
			return this.InitTextReaderInputAsync(baseUriStr, null, input);
		}

		private Task InitTextReaderInputAsync(string baseUriStr, Uri baseUri, TextReader input)
		{
			this.ps.textReader = input;
			this.ps.baseUriStr = baseUriStr;
			this.ps.baseUri = baseUri;
			if (this.ps.chars == null)
			{
				int num;
				if (this.laterInitParam != null && this.laterInitParam.useAsync)
				{
					num = 65536;
				}
				else
				{
					num = 4096;
				}
				this.ps.chars = new char[num + 1];
			}
			this.ps.encoding = Encoding.Unicode;
			this.ps.eolNormalized = !this.normalize;
			this.ps.appendMode = true;
			return this.ReadDataAsync();
		}

		private Task ProcessDtdFromParserContextAsync(XmlParserContext context)
		{
			switch (this.dtdProcessing)
			{
			case DtdProcessing.Prohibit:
				this.ThrowWithoutLineInfo("For security reasons DTD is prohibited in this XML document. To enable DTD processing set the DtdProcessing property on XmlReaderSettings to Parse and pass the settings into XmlReader.Create method.");
				break;
			case DtdProcessing.Parse:
				return this.ParseDtdFromParserContextAsync();
			}
			return AsyncHelper.DoneTask;
		}

		private Task SwitchEncodingAsync(Encoding newEncoding)
		{
			if ((newEncoding.WebName != this.ps.encoding.WebName || this.ps.decoder is SafeAsciiDecoder) && !this.afterResetState)
			{
				this.UnDecodeChars();
				this.ps.appendMode = false;
				this.SetupEncoding(newEncoding);
				return this.ReadDataAsync();
			}
			return AsyncHelper.DoneTask;
		}

		private Task SwitchEncodingToUTF8Async()
		{
			return this.SwitchEncodingAsync(new UTF8Encoding(true, true));
		}

		private Task<int> ReadDataAsync()
		{
			XmlTextReaderImpl.<ReadDataAsync>d__505 <ReadDataAsync>d__;
			<ReadDataAsync>d__.<>4__this = this;
			<ReadDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadDataAsync>d__.<>1__state = -1;
			<ReadDataAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadDataAsync>d__505>(ref <ReadDataAsync>d__);
			return <ReadDataAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseXmlDeclarationAsync(bool isTextDecl)
		{
			XmlTextReaderImpl.<ParseXmlDeclarationAsync>d__506 <ParseXmlDeclarationAsync>d__;
			<ParseXmlDeclarationAsync>d__.<>4__this = this;
			<ParseXmlDeclarationAsync>d__.isTextDecl = isTextDecl;
			<ParseXmlDeclarationAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseXmlDeclarationAsync>d__.<>1__state = -1;
			<ParseXmlDeclarationAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseXmlDeclarationAsync>d__506>(ref <ParseXmlDeclarationAsync>d__);
			return <ParseXmlDeclarationAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseDocumentContentAsync()
		{
			bool needMoreChars;
			int num;
			char[] chars;
			char c;
			for (;;)
			{
				needMoreChars = false;
				num = this.ps.charPos;
				chars = this.ps.chars;
				if (chars[num] != '<')
				{
					goto IL_24E;
				}
				needMoreChars = true;
				if (this.ps.charsUsed - num < 4)
				{
					break;
				}
				num++;
				c = chars[num];
				if (c != '!')
				{
					if (c != '/')
					{
						goto Block_3;
					}
					this.Throw(num + 1, "Unexpected end tag.");
				}
				else
				{
					num++;
					if (this.ps.charsUsed - num < 2)
					{
						goto Block_5;
					}
					if (chars[num] == '-')
					{
						if (chars[num + 1] == '-')
						{
							goto Block_7;
						}
						this.ThrowUnexpectedToken(num + 1, "-");
					}
					else if (chars[num] == '[')
					{
						if (this.fragmentType != XmlNodeType.Document)
						{
							num++;
							if (this.ps.charsUsed - num < 6)
							{
								goto Block_10;
							}
							if (XmlConvert.StrEqual(chars, num, 6, "CDATA["))
							{
								goto Block_11;
							}
							this.ThrowUnexpectedToken(num, "CDATA[");
						}
						else
						{
							this.Throw(this.ps.charPos, "Data at the root level is invalid.");
						}
					}
					else
					{
						if (this.fragmentType == XmlNodeType.Document || this.fragmentType == XmlNodeType.None)
						{
							goto IL_189;
						}
						if (this.ParseUnexpectedToken(num) == "DOCTYPE")
						{
							this.Throw("Unexpected DTD declaration.");
						}
						else
						{
							this.ThrowUnexpectedToken(num, "<!--", "<[CDATA[");
						}
					}
				}
			}
			return this.ParseDocumentContentAsync_ReadData(needMoreChars);
			Block_3:
			if (c == '?')
			{
				this.ps.charPos = num + 1;
				return this.ParsePIAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseDocumentContentAsync));
			}
			if (this.rootElementParsed)
			{
				if (this.fragmentType == XmlNodeType.Document)
				{
					this.Throw(num, "There are multiple root elements.");
				}
				if (this.fragmentType == XmlNodeType.None)
				{
					this.fragmentType = XmlNodeType.Element;
				}
			}
			this.ps.charPos = num;
			this.rootElementParsed = true;
			return this.ParseElementAsync().ReturnTaskBoolWhenFinish(true);
			Block_5:
			return this.ParseDocumentContentAsync_ReadData(needMoreChars);
			Block_7:
			this.ps.charPos = num + 2;
			return this.ParseCommentAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseDocumentContentAsync));
			Block_10:
			return this.ParseDocumentContentAsync_ReadData(needMoreChars);
			Block_11:
			this.ps.charPos = num + 6;
			return this.ParseCDataAsync().CallBoolTaskFuncWhenFinish(new Func<Task<bool>>(this.ParseDocumentContentAsync_CData));
			IL_189:
			this.fragmentType = XmlNodeType.Document;
			this.ps.charPos = num;
			return this.ParseDoctypeDeclAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseDocumentContentAsync));
			IL_24E:
			if (chars[num] == '&')
			{
				return this.ParseDocumentContentAsync_ParseEntity();
			}
			if (num == this.ps.charsUsed || (this.v1Compat && chars[num] == '\0'))
			{
				return this.ParseDocumentContentAsync_ReadData(needMoreChars);
			}
			if (this.fragmentType == XmlNodeType.Document)
			{
				return this.ParseRootLevelWhitespaceAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseDocumentContentAsync));
			}
			return this.ParseDocumentContentAsync_WhiteSpace();
		}

		private Task<bool> ParseDocumentContentAsync_CData()
		{
			if (this.fragmentType == XmlNodeType.None)
			{
				this.fragmentType = XmlNodeType.Element;
			}
			return AsyncHelper.DoneTaskTrue;
		}

		private Task<bool> ParseDocumentContentAsync_ParseEntity()
		{
			XmlTextReaderImpl.<ParseDocumentContentAsync_ParseEntity>d__509 <ParseDocumentContentAsync_ParseEntity>d__;
			<ParseDocumentContentAsync_ParseEntity>d__.<>4__this = this;
			<ParseDocumentContentAsync_ParseEntity>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseDocumentContentAsync_ParseEntity>d__.<>1__state = -1;
			<ParseDocumentContentAsync_ParseEntity>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseDocumentContentAsync_ParseEntity>d__509>(ref <ParseDocumentContentAsync_ParseEntity>d__);
			return <ParseDocumentContentAsync_ParseEntity>d__.<>t__builder.Task;
		}

		private Task<bool> ParseDocumentContentAsync_WhiteSpace()
		{
			Task<bool> task = this.ParseTextAsync();
			if (!task.IsSuccess())
			{
				return this._ParseDocumentContentAsync_WhiteSpace(task);
			}
			if (task.Result)
			{
				if (this.fragmentType == XmlNodeType.None && this.curNode.type == XmlNodeType.Text)
				{
					this.fragmentType = XmlNodeType.Element;
				}
				return AsyncHelper.DoneTaskTrue;
			}
			return this.ParseDocumentContentAsync();
		}

		private Task<bool> _ParseDocumentContentAsync_WhiteSpace(Task<bool> task)
		{
			XmlTextReaderImpl.<_ParseDocumentContentAsync_WhiteSpace>d__511 <_ParseDocumentContentAsync_WhiteSpace>d__;
			<_ParseDocumentContentAsync_WhiteSpace>d__.<>4__this = this;
			<_ParseDocumentContentAsync_WhiteSpace>d__.task = task;
			<_ParseDocumentContentAsync_WhiteSpace>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ParseDocumentContentAsync_WhiteSpace>d__.<>1__state = -1;
			<_ParseDocumentContentAsync_WhiteSpace>d__.<>t__builder.Start<XmlTextReaderImpl.<_ParseDocumentContentAsync_WhiteSpace>d__511>(ref <_ParseDocumentContentAsync_WhiteSpace>d__);
			return <_ParseDocumentContentAsync_WhiteSpace>d__.<>t__builder.Task;
		}

		private Task<bool> ParseDocumentContentAsync_ReadData(bool needMoreChars)
		{
			XmlTextReaderImpl.<ParseDocumentContentAsync_ReadData>d__512 <ParseDocumentContentAsync_ReadData>d__;
			<ParseDocumentContentAsync_ReadData>d__.<>4__this = this;
			<ParseDocumentContentAsync_ReadData>d__.needMoreChars = needMoreChars;
			<ParseDocumentContentAsync_ReadData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseDocumentContentAsync_ReadData>d__.<>1__state = -1;
			<ParseDocumentContentAsync_ReadData>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseDocumentContentAsync_ReadData>d__512>(ref <ParseDocumentContentAsync_ReadData>d__);
			return <ParseDocumentContentAsync_ReadData>d__.<>t__builder.Task;
		}

		private Task<bool> ParseElementContentAsync()
		{
			int num;
			char c2;
			for (;;)
			{
				num = this.ps.charPos;
				char[] chars = this.ps.chars;
				char c = chars[num];
				if (c == '&')
				{
					goto IL_1B4;
				}
				if (c != '<')
				{
					goto IL_1CC;
				}
				c2 = chars[num + 1];
				if (c2 != '!')
				{
					break;
				}
				num += 2;
				if (this.ps.charsUsed - num < 2)
				{
					goto Block_5;
				}
				if (chars[num] == '-')
				{
					if (chars[num + 1] == '-')
					{
						goto Block_7;
					}
					this.ThrowUnexpectedToken(num + 1, "-");
				}
				else if (chars[num] == '[')
				{
					num++;
					if (this.ps.charsUsed - num < 6)
					{
						goto Block_9;
					}
					if (XmlConvert.StrEqual(chars, num, 6, "CDATA["))
					{
						goto Block_10;
					}
					this.ThrowUnexpectedToken(num, "CDATA[");
				}
				else if (this.ParseUnexpectedToken(num) == "DOCTYPE")
				{
					this.Throw("Unexpected DTD declaration.");
				}
				else
				{
					this.ThrowUnexpectedToken(num, "<!--", "<[CDATA[");
				}
			}
			if (c2 == '/')
			{
				this.ps.charPos = num + 2;
				return this.ParseEndElementAsync().ReturnTaskBoolWhenFinish(true);
			}
			if (c2 == '?')
			{
				this.ps.charPos = num + 2;
				return this.ParsePIAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseElementContentAsync));
			}
			if (num + 1 == this.ps.charsUsed)
			{
				return this.ParseElementContent_ReadData();
			}
			this.ps.charPos = num + 1;
			return this.ParseElementAsync().ReturnTaskBoolWhenFinish(true);
			Block_5:
			return this.ParseElementContent_ReadData();
			Block_7:
			this.ps.charPos = num + 2;
			return this.ParseCommentAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseElementContentAsync));
			Block_9:
			return this.ParseElementContent_ReadData();
			Block_10:
			this.ps.charPos = num + 6;
			return this.ParseCDataAsync().ReturnTaskBoolWhenFinish(true);
			IL_1B4:
			return this.ParseTextAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseElementContentAsync));
			IL_1CC:
			if (num == this.ps.charsUsed)
			{
				return this.ParseElementContent_ReadData();
			}
			return this.ParseTextAsync().ContinueBoolTaskFuncWhenFalse(new Func<Task<bool>>(this.ParseElementContentAsync));
		}

		private Task<bool> ParseElementContent_ReadData()
		{
			XmlTextReaderImpl.<ParseElementContent_ReadData>d__514 <ParseElementContent_ReadData>d__;
			<ParseElementContent_ReadData>d__.<>4__this = this;
			<ParseElementContent_ReadData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseElementContent_ReadData>d__.<>1__state = -1;
			<ParseElementContent_ReadData>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseElementContent_ReadData>d__514>(ref <ParseElementContent_ReadData>d__);
			return <ParseElementContent_ReadData>d__.<>t__builder.Task;
		}

		private Task ParseElementAsync()
		{
			int num = this.ps.charPos;
			char[] chars = this.ps.chars;
			int num2 = -1;
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			while ((this.xmlCharType.charProperties[(int)chars[num]] & 4) != 0)
			{
				num++;
				for (;;)
				{
					if ((this.xmlCharType.charProperties[(int)chars[num]] & 8) != 0)
					{
						num++;
					}
					else
					{
						if (chars[num] != ':')
						{
							goto IL_A2;
						}
						if (num2 == -1)
						{
							break;
						}
						if (this.supportNamespaces)
						{
							goto Block_5;
						}
						num++;
					}
				}
				num2 = num;
				num++;
				continue;
				Block_5:
				this.Throw(num, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(':', '\0'));
				break;
				IL_A2:
				if (num + 1 >= this.ps.charsUsed)
				{
					break;
				}
				return this.ParseElementAsync_SetElement(num2, num);
			}
			Task<Tuple<int, int>> task = this.ParseQNameAsync();
			return this.ParseElementAsync_ContinueWithSetElement(task);
		}

		private Task ParseElementAsync_ContinueWithSetElement(Task<Tuple<int, int>> task)
		{
			if (task.IsSuccess())
			{
				Tuple<int, int> result = task.Result;
				int item = result.Item1;
				int item2 = result.Item2;
				return this.ParseElementAsync_SetElement(item, item2);
			}
			return this._ParseElementAsync_ContinueWithSetElement(task);
		}

		private Task _ParseElementAsync_ContinueWithSetElement(Task<Tuple<int, int>> task)
		{
			XmlTextReaderImpl.<_ParseElementAsync_ContinueWithSetElement>d__517 <_ParseElementAsync_ContinueWithSetElement>d__;
			<_ParseElementAsync_ContinueWithSetElement>d__.<>4__this = this;
			<_ParseElementAsync_ContinueWithSetElement>d__.task = task;
			<_ParseElementAsync_ContinueWithSetElement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<_ParseElementAsync_ContinueWithSetElement>d__.<>1__state = -1;
			<_ParseElementAsync_ContinueWithSetElement>d__.<>t__builder.Start<XmlTextReaderImpl.<_ParseElementAsync_ContinueWithSetElement>d__517>(ref <_ParseElementAsync_ContinueWithSetElement>d__);
			return <_ParseElementAsync_ContinueWithSetElement>d__.<>t__builder.Task;
		}

		private Task ParseElementAsync_SetElement(int colonPos, int pos)
		{
			char[] chars = this.ps.chars;
			this.namespaceManager.PushScope();
			if (colonPos == -1 || !this.supportNamespaces)
			{
				this.curNode.SetNamedNode(XmlNodeType.Element, this.nameTable.Add(chars, this.ps.charPos, pos - this.ps.charPos));
			}
			else
			{
				int charPos = this.ps.charPos;
				int num = colonPos - charPos;
				if (num == this.lastPrefix.Length && XmlConvert.StrEqual(chars, charPos, num, this.lastPrefix))
				{
					this.curNode.SetNamedNode(XmlNodeType.Element, this.nameTable.Add(chars, colonPos + 1, pos - colonPos - 1), this.lastPrefix, null);
				}
				else
				{
					this.curNode.SetNamedNode(XmlNodeType.Element, this.nameTable.Add(chars, colonPos + 1, pos - colonPos - 1), this.nameTable.Add(chars, this.ps.charPos, num), null);
					this.lastPrefix = this.curNode.prefix;
				}
			}
			char c = chars[pos];
			bool flag = (this.xmlCharType.charProperties[(int)c] & 1) > 0;
			this.ps.charPos = pos;
			if (flag)
			{
				return this.ParseAttributesAsync();
			}
			return this.ParseElementAsync_NoAttributes();
		}

		private Task ParseElementAsync_NoAttributes()
		{
			int charPos = this.ps.charPos;
			char[] chars = this.ps.chars;
			char c = chars[charPos];
			if (c == '>')
			{
				this.ps.charPos = charPos + 1;
				this.parsingFunction = XmlTextReaderImpl.ParsingFunction.MoveToElementContent;
			}
			else if (c == '/')
			{
				if (charPos + 1 == this.ps.charsUsed)
				{
					this.ps.charPos = charPos;
					return this.ParseElementAsync_ReadData(charPos);
				}
				if (chars[charPos + 1] == '>')
				{
					this.curNode.IsEmptyElement = true;
					this.nextParsingFunction = this.parsingFunction;
					this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PopEmptyElementContext;
					this.ps.charPos = charPos + 2;
				}
				else
				{
					this.ThrowUnexpectedToken(charPos, ">");
				}
			}
			else
			{
				this.Throw(charPos, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(chars, this.ps.charsUsed, charPos));
			}
			if (this.addDefaultAttributesAndNormalize)
			{
				this.AddDefaultAttributesAndNormalize();
			}
			this.ElementNamespaceLookup();
			return AsyncHelper.DoneTask;
		}

		private Task ParseElementAsync_ReadData(int pos)
		{
			XmlTextReaderImpl.<ParseElementAsync_ReadData>d__520 <ParseElementAsync_ReadData>d__;
			<ParseElementAsync_ReadData>d__.<>4__this = this;
			<ParseElementAsync_ReadData>d__.pos = pos;
			<ParseElementAsync_ReadData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseElementAsync_ReadData>d__.<>1__state = -1;
			<ParseElementAsync_ReadData>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseElementAsync_ReadData>d__520>(ref <ParseElementAsync_ReadData>d__);
			return <ParseElementAsync_ReadData>d__.<>t__builder.Task;
		}

		private Task ParseEndElementAsync()
		{
			XmlTextReaderImpl.NodeData nodeData = this.nodes[this.index - 1];
			int length = nodeData.prefix.Length;
			int length2 = nodeData.localName.Length;
			if (this.ps.charsUsed - this.ps.charPos < length + length2 + 1)
			{
				return this._ParseEndElmentAsync();
			}
			return this.ParseEndElementAsync_CheckNameAndParse();
		}

		private Task _ParseEndElmentAsync()
		{
			XmlTextReaderImpl.<_ParseEndElmentAsync>d__522 <_ParseEndElmentAsync>d__;
			<_ParseEndElmentAsync>d__.<>4__this = this;
			<_ParseEndElmentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<_ParseEndElmentAsync>d__.<>1__state = -1;
			<_ParseEndElmentAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<_ParseEndElmentAsync>d__522>(ref <_ParseEndElmentAsync>d__);
			return <_ParseEndElmentAsync>d__.<>t__builder.Task;
		}

		private Task ParseEndElmentAsync_PrepareData()
		{
			XmlTextReaderImpl.<ParseEndElmentAsync_PrepareData>d__523 <ParseEndElmentAsync_PrepareData>d__;
			<ParseEndElmentAsync_PrepareData>d__.<>4__this = this;
			<ParseEndElmentAsync_PrepareData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseEndElmentAsync_PrepareData>d__.<>1__state = -1;
			<ParseEndElmentAsync_PrepareData>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseEndElmentAsync_PrepareData>d__523>(ref <ParseEndElmentAsync_PrepareData>d__);
			return <ParseEndElmentAsync_PrepareData>d__.<>t__builder.Task;
		}

		private Task ParseEndElementAsync_CheckNameAndParse()
		{
			XmlTextReaderImpl.NodeData nodeData = this.nodes[this.index - 1];
			int length = nodeData.prefix.Length;
			int length2 = nodeData.localName.Length;
			char[] chars = this.ps.chars;
			int nameLen;
			if (nodeData.prefix.Length == 0)
			{
				if (!XmlConvert.StrEqual(chars, this.ps.charPos, length2, nodeData.localName))
				{
					return this.ThrowTagMismatchAsync(nodeData);
				}
				nameLen = length2;
			}
			else
			{
				int num = this.ps.charPos + length;
				if (!XmlConvert.StrEqual(chars, this.ps.charPos, length, nodeData.prefix) || chars[num] != ':' || !XmlConvert.StrEqual(chars, num + 1, length2, nodeData.localName))
				{
					return this.ThrowTagMismatchAsync(nodeData);
				}
				nameLen = length2 + length + 1;
			}
			LineInfo endTagLineInfo = new LineInfo(this.ps.lineNo, this.ps.LinePos);
			return this.ParseEndElementAsync_Finish(nameLen, nodeData, endTagLineInfo);
		}

		private Task ParseEndElementAsync_Finish(int nameLen, XmlTextReaderImpl.NodeData startTagNode, LineInfo endTagLineInfo)
		{
			Task task = this.ParseEndElementAsync_CheckEndTag(nameLen, startTagNode, endTagLineInfo);
			while (task.IsSuccess())
			{
				switch (this.parseEndElement_NextFunc)
				{
				case XmlTextReaderImpl.ParseEndElementParseFunction.CheckEndTag:
					task = this.ParseEndElementAsync_CheckEndTag(nameLen, startTagNode, endTagLineInfo);
					break;
				case XmlTextReaderImpl.ParseEndElementParseFunction.ReadData:
					task = this.ParseEndElementAsync_ReadData();
					break;
				case XmlTextReaderImpl.ParseEndElementParseFunction.Done:
					return task;
				}
			}
			return this.ParseEndElementAsync_Finish(task, nameLen, startTagNode, endTagLineInfo);
		}

		private Task ParseEndElementAsync_Finish(Task task, int nameLen, XmlTextReaderImpl.NodeData startTagNode, LineInfo endTagLineInfo)
		{
			XmlTextReaderImpl.<ParseEndElementAsync_Finish>d__528 <ParseEndElementAsync_Finish>d__;
			<ParseEndElementAsync_Finish>d__.<>4__this = this;
			<ParseEndElementAsync_Finish>d__.task = task;
			<ParseEndElementAsync_Finish>d__.nameLen = nameLen;
			<ParseEndElementAsync_Finish>d__.startTagNode = startTagNode;
			<ParseEndElementAsync_Finish>d__.endTagLineInfo = endTagLineInfo;
			<ParseEndElementAsync_Finish>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseEndElementAsync_Finish>d__.<>1__state = -1;
			<ParseEndElementAsync_Finish>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseEndElementAsync_Finish>d__528>(ref <ParseEndElementAsync_Finish>d__);
			return <ParseEndElementAsync_Finish>d__.<>t__builder.Task;
		}

		private Task ParseEndElementAsync_CheckEndTag(int nameLen, XmlTextReaderImpl.NodeData startTagNode, LineInfo endTagLineInfo)
		{
			int num;
			for (;;)
			{
				num = this.ps.charPos + nameLen;
				char[] chars = this.ps.chars;
				if (num == this.ps.charsUsed)
				{
					break;
				}
				bool flag = false;
				if ((this.xmlCharType.charProperties[(int)chars[num]] & 8) != 0 || chars[num] == ':')
				{
					flag = true;
				}
				if (flag)
				{
					goto Block_2;
				}
				if (chars[num] != '>')
				{
					char c;
					while (this.xmlCharType.IsWhiteSpace(c = chars[num]))
					{
						num++;
						if (c != '\n')
						{
							if (c == '\r')
							{
								if (chars[num] == '\n')
								{
									num++;
								}
								else if (num == this.ps.charsUsed && !this.ps.isEof)
								{
									continue;
								}
								this.OnNewLine(num);
							}
						}
						else
						{
							this.OnNewLine(num);
						}
					}
				}
				if (chars[num] == '>')
				{
					goto IL_F4;
				}
				if (num == this.ps.charsUsed)
				{
					goto Block_9;
				}
				this.ThrowUnexpectedToken(num, ">");
			}
			this.parseEndElement_NextFunc = XmlTextReaderImpl.ParseEndElementParseFunction.ReadData;
			return AsyncHelper.DoneTask;
			Block_2:
			return this.ThrowTagMismatchAsync(startTagNode);
			Block_9:
			this.parseEndElement_NextFunc = XmlTextReaderImpl.ParseEndElementParseFunction.ReadData;
			return AsyncHelper.DoneTask;
			IL_F4:
			this.index--;
			this.curNode = this.nodes[this.index];
			startTagNode.lineInfo = endTagLineInfo;
			startTagNode.type = XmlNodeType.EndElement;
			this.ps.charPos = num + 1;
			this.nextParsingFunction = ((this.index > 0) ? this.parsingFunction : XmlTextReaderImpl.ParsingFunction.DocumentContent);
			this.parsingFunction = XmlTextReaderImpl.ParsingFunction.PopElementContext;
			this.parseEndElement_NextFunc = XmlTextReaderImpl.ParseEndElementParseFunction.Done;
			return AsyncHelper.DoneTask;
		}

		private Task ParseEndElementAsync_ReadData()
		{
			XmlTextReaderImpl.<ParseEndElementAsync_ReadData>d__530 <ParseEndElementAsync_ReadData>d__;
			<ParseEndElementAsync_ReadData>d__.<>4__this = this;
			<ParseEndElementAsync_ReadData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseEndElementAsync_ReadData>d__.<>1__state = -1;
			<ParseEndElementAsync_ReadData>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseEndElementAsync_ReadData>d__530>(ref <ParseEndElementAsync_ReadData>d__);
			return <ParseEndElementAsync_ReadData>d__.<>t__builder.Task;
		}

		private Task ThrowTagMismatchAsync(XmlTextReaderImpl.NodeData startTag)
		{
			XmlTextReaderImpl.<ThrowTagMismatchAsync>d__531 <ThrowTagMismatchAsync>d__;
			<ThrowTagMismatchAsync>d__.<>4__this = this;
			<ThrowTagMismatchAsync>d__.startTag = startTag;
			<ThrowTagMismatchAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ThrowTagMismatchAsync>d__.<>1__state = -1;
			<ThrowTagMismatchAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ThrowTagMismatchAsync>d__531>(ref <ThrowTagMismatchAsync>d__);
			return <ThrowTagMismatchAsync>d__.<>t__builder.Task;
		}

		private Task ParseAttributesAsync()
		{
			XmlTextReaderImpl.<ParseAttributesAsync>d__532 <ParseAttributesAsync>d__;
			<ParseAttributesAsync>d__.<>4__this = this;
			<ParseAttributesAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseAttributesAsync>d__.<>1__state = -1;
			<ParseAttributesAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseAttributesAsync>d__532>(ref <ParseAttributesAsync>d__);
			return <ParseAttributesAsync>d__.<>t__builder.Task;
		}

		private Task ParseAttributeValueSlowAsync(int curPos, char quoteChar, XmlTextReaderImpl.NodeData attr)
		{
			XmlTextReaderImpl.<ParseAttributeValueSlowAsync>d__533 <ParseAttributeValueSlowAsync>d__;
			<ParseAttributeValueSlowAsync>d__.<>4__this = this;
			<ParseAttributeValueSlowAsync>d__.curPos = curPos;
			<ParseAttributeValueSlowAsync>d__.quoteChar = quoteChar;
			<ParseAttributeValueSlowAsync>d__.attr = attr;
			<ParseAttributeValueSlowAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseAttributeValueSlowAsync>d__.<>1__state = -1;
			<ParseAttributeValueSlowAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseAttributeValueSlowAsync>d__533>(ref <ParseAttributeValueSlowAsync>d__);
			return <ParseAttributeValueSlowAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseTextAsync()
		{
			int num = 0;
			if (this.parsingMode != XmlTextReaderImpl.ParsingMode.Full)
			{
				return this._ParseTextAsync(null);
			}
			this.curNode.SetLineInfo(this.ps.LineNo, this.ps.LinePos);
			Task<Tuple<int, int, int, bool>> task = this.ParseTextAsync(num);
			if (!task.IsSuccess())
			{
				return this._ParseTextAsync(task);
			}
			Tuple<int, int, int, bool> result = task.Result;
			int item = result.Item1;
			int item2 = result.Item2;
			num = result.Item3;
			bool item3 = result.Item4;
			if (!item3)
			{
				return this._ParseTextAsync(task);
			}
			if (item2 - item == 0)
			{
				return this.ParseTextAsync_IgnoreNode();
			}
			XmlNodeType textNodeType = this.GetTextNodeType(num);
			if (textNodeType == XmlNodeType.None)
			{
				return this.ParseTextAsync_IgnoreNode();
			}
			this.curNode.SetValueNode(textNodeType, this.ps.chars, item, item2 - item);
			return AsyncHelper.DoneTaskTrue;
		}

		private Task<bool> _ParseTextAsync(Task<Tuple<int, int, int, bool>> parseTask)
		{
			XmlTextReaderImpl.<_ParseTextAsync>d__535 <_ParseTextAsync>d__;
			<_ParseTextAsync>d__.<>4__this = this;
			<_ParseTextAsync>d__.parseTask = parseTask;
			<_ParseTextAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<_ParseTextAsync>d__.<>1__state = -1;
			<_ParseTextAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<_ParseTextAsync>d__535>(ref <_ParseTextAsync>d__);
			return <_ParseTextAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseTextAsync_IgnoreNode()
		{
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.ReportEndEntity)
			{
				this.SetupEndEntityNodeInContent();
				this.parsingFunction = this.nextParsingFunction;
				return AsyncHelper.DoneTaskTrue;
			}
			if (this.parsingFunction == XmlTextReaderImpl.ParsingFunction.EntityReference)
			{
				this.parsingFunction = this.nextNextParsingFunction;
				return this.ParseEntityReferenceAsync().ReturnTaskBoolWhenFinish(true);
			}
			return AsyncHelper.DoneTaskFalse;
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync(int outOrChars)
		{
			Task<Tuple<int, int, int, bool>> task = this.ParseTextAsync(outOrChars, this.ps.chars, this.ps.charPos, 0, -1, outOrChars, '\0');
			while (task.IsSuccess())
			{
				outOrChars = this.lastParseTextState.outOrChars;
				char[] chars = this.lastParseTextState.chars;
				int pos = this.lastParseTextState.pos;
				int rcount = this.lastParseTextState.rcount;
				int rpos = this.lastParseTextState.rpos;
				int orChars = this.lastParseTextState.orChars;
				char c = this.lastParseTextState.c;
				switch (this.parseText_NextFunction)
				{
				case XmlTextReaderImpl.ParseTextFunction.ParseText:
					task = this.ParseTextAsync(outOrChars, chars, pos, rcount, rpos, orChars, c);
					break;
				case XmlTextReaderImpl.ParseTextFunction.Entity:
					task = this.ParseTextAsync_ParseEntity(outOrChars, chars, pos, rcount, rpos, orChars, c);
					break;
				case XmlTextReaderImpl.ParseTextFunction.Surrogate:
					task = this.ParseTextAsync_Surrogate(outOrChars, chars, pos, rcount, rpos, orChars, c);
					break;
				case XmlTextReaderImpl.ParseTextFunction.ReadData:
					task = this.ParseTextAsync_ReadData(outOrChars, chars, pos, rcount, rpos, orChars, c);
					break;
				case XmlTextReaderImpl.ParseTextFunction.NoValue:
					return this.ParseTextAsync_NoValue(outOrChars, pos);
				case XmlTextReaderImpl.ParseTextFunction.PartialValue:
					return this.ParseTextAsync_PartialValue(pos, rcount, rpos, orChars, c);
				}
			}
			return this.ParseTextAsync_AsyncFunc(task);
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync_AsyncFunc(Task<Tuple<int, int, int, bool>> task)
		{
			XmlTextReaderImpl.<ParseTextAsync_AsyncFunc>d__543 <ParseTextAsync_AsyncFunc>d__;
			<ParseTextAsync_AsyncFunc>d__.<>4__this = this;
			<ParseTextAsync_AsyncFunc>d__.task = task;
			<ParseTextAsync_AsyncFunc>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int, int, bool>>.Create();
			<ParseTextAsync_AsyncFunc>d__.<>1__state = -1;
			<ParseTextAsync_AsyncFunc>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseTextAsync_AsyncFunc>d__543>(ref <ParseTextAsync_AsyncFunc>d__);
			return <ParseTextAsync_AsyncFunc>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync(int outOrChars, char[] chars, int pos, int rcount, int rpos, int orChars, char c)
		{
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)(c = chars[pos])] & 64) == 0)
				{
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
							pos++;
							continue;
						case '\n':
							pos++;
							this.OnNewLine(pos);
							continue;
						case '\v':
						case '\f':
							goto IL_214;
						case '\r':
							if (chars[pos + 1] == '\n')
							{
								if (!this.ps.eolNormalized && this.parsingMode == XmlTextReaderImpl.ParsingMode.Full)
								{
									if (pos - this.ps.charPos > 0)
									{
										if (rcount == 0)
										{
											rcount = 1;
											rpos = pos;
										}
										else
										{
											this.ShiftBuffer(rpos + rcount, rpos, pos - rpos - rcount);
											rpos = pos - rcount;
											rcount++;
										}
									}
									else
									{
										this.ps.charPos = this.ps.charPos + 1;
									}
								}
								pos += 2;
							}
							else
							{
								if (pos + 1 >= this.ps.charsUsed && !this.ps.isEof)
								{
									goto IL_12C;
								}
								if (!this.ps.eolNormalized)
								{
									chars[pos] = '\n';
								}
								pos++;
							}
							this.OnNewLine(pos);
							continue;
						}
						break;
					}
					if (c == '<')
					{
						goto IL_15C;
					}
					if (c != ']')
					{
						goto Block_6;
					}
					if (this.ps.charsUsed - pos < 3 && !this.ps.isEof)
					{
						goto Block_15;
					}
					if (chars[pos + 1] == ']' && chars[pos + 2] == '>')
					{
						this.Throw(pos, "']]>' is not allowed in character data.");
					}
					orChars |= 93;
					pos++;
				}
				else
				{
					orChars |= (int)c;
					pos++;
				}
			}
			if (c == '&')
			{
				this.lastParseTextState = new XmlTextReaderImpl.ParseTextState(outOrChars, chars, pos, rcount, rpos, orChars, c);
				this.parseText_NextFunction = XmlTextReaderImpl.ParseTextFunction.Entity;
				return this.parseText_dummyTask;
			}
			Block_6:
			goto IL_214;
			IL_12C:
			this.lastParseTextState = new XmlTextReaderImpl.ParseTextState(outOrChars, chars, pos, rcount, rpos, orChars, c);
			this.parseText_NextFunction = XmlTextReaderImpl.ParseTextFunction.ReadData;
			return this.parseText_dummyTask;
			IL_15C:
			this.lastParseTextState = new XmlTextReaderImpl.ParseTextState(outOrChars, chars, pos, rcount, rpos, orChars, c);
			this.parseText_NextFunction = XmlTextReaderImpl.ParseTextFunction.PartialValue;
			return this.parseText_dummyTask;
			Block_15:
			this.lastParseTextState = new XmlTextReaderImpl.ParseTextState(outOrChars, chars, pos, rcount, rpos, orChars, c);
			this.parseText_NextFunction = XmlTextReaderImpl.ParseTextFunction.ReadData;
			return this.parseText_dummyTask;
			IL_214:
			if (pos == this.ps.charsUsed)
			{
				this.lastParseTextState = new XmlTextReaderImpl.ParseTextState(outOrChars, chars, pos, rcount, rpos, orChars, c);
				this.parseText_NextFunction = XmlTextReaderImpl.ParseTextFunction.ReadData;
				return this.parseText_dummyTask;
			}
			this.lastParseTextState = new XmlTextReaderImpl.ParseTextState(outOrChars, chars, pos, rcount, rpos, orChars, c);
			this.parseText_NextFunction = XmlTextReaderImpl.ParseTextFunction.Surrogate;
			return this.parseText_dummyTask;
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync_ParseEntity(int outOrChars, char[] chars, int pos, int rcount, int rpos, int orChars, char c)
		{
			XmlTextReaderImpl.<ParseTextAsync_ParseEntity>d__545 <ParseTextAsync_ParseEntity>d__;
			<ParseTextAsync_ParseEntity>d__.<>4__this = this;
			<ParseTextAsync_ParseEntity>d__.outOrChars = outOrChars;
			<ParseTextAsync_ParseEntity>d__.chars = chars;
			<ParseTextAsync_ParseEntity>d__.pos = pos;
			<ParseTextAsync_ParseEntity>d__.rcount = rcount;
			<ParseTextAsync_ParseEntity>d__.rpos = rpos;
			<ParseTextAsync_ParseEntity>d__.orChars = orChars;
			<ParseTextAsync_ParseEntity>d__.c = c;
			<ParseTextAsync_ParseEntity>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int, int, bool>>.Create();
			<ParseTextAsync_ParseEntity>d__.<>1__state = -1;
			<ParseTextAsync_ParseEntity>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseTextAsync_ParseEntity>d__545>(ref <ParseTextAsync_ParseEntity>d__);
			return <ParseTextAsync_ParseEntity>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync_Surrogate(int outOrChars, char[] chars, int pos, int rcount, int rpos, int orChars, char c)
		{
			XmlTextReaderImpl.<ParseTextAsync_Surrogate>d__546 <ParseTextAsync_Surrogate>d__;
			<ParseTextAsync_Surrogate>d__.<>4__this = this;
			<ParseTextAsync_Surrogate>d__.outOrChars = outOrChars;
			<ParseTextAsync_Surrogate>d__.chars = chars;
			<ParseTextAsync_Surrogate>d__.pos = pos;
			<ParseTextAsync_Surrogate>d__.rcount = rcount;
			<ParseTextAsync_Surrogate>d__.rpos = rpos;
			<ParseTextAsync_Surrogate>d__.orChars = orChars;
			<ParseTextAsync_Surrogate>d__.c = c;
			<ParseTextAsync_Surrogate>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int, int, bool>>.Create();
			<ParseTextAsync_Surrogate>d__.<>1__state = -1;
			<ParseTextAsync_Surrogate>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseTextAsync_Surrogate>d__546>(ref <ParseTextAsync_Surrogate>d__);
			return <ParseTextAsync_Surrogate>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync_ReadData(int outOrChars, char[] chars, int pos, int rcount, int rpos, int orChars, char c)
		{
			XmlTextReaderImpl.<ParseTextAsync_ReadData>d__547 <ParseTextAsync_ReadData>d__;
			<ParseTextAsync_ReadData>d__.<>4__this = this;
			<ParseTextAsync_ReadData>d__.outOrChars = outOrChars;
			<ParseTextAsync_ReadData>d__.chars = chars;
			<ParseTextAsync_ReadData>d__.pos = pos;
			<ParseTextAsync_ReadData>d__.rcount = rcount;
			<ParseTextAsync_ReadData>d__.rpos = rpos;
			<ParseTextAsync_ReadData>d__.orChars = orChars;
			<ParseTextAsync_ReadData>d__.c = c;
			<ParseTextAsync_ReadData>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int, int, bool>>.Create();
			<ParseTextAsync_ReadData>d__.<>1__state = -1;
			<ParseTextAsync_ReadData>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseTextAsync_ReadData>d__547>(ref <ParseTextAsync_ReadData>d__);
			return <ParseTextAsync_ReadData>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync_NoValue(int outOrChars, int pos)
		{
			return Task.FromResult<Tuple<int, int, int, bool>>(new Tuple<int, int, int, bool>(pos, pos, outOrChars, true));
		}

		private Task<Tuple<int, int, int, bool>> ParseTextAsync_PartialValue(int pos, int rcount, int rpos, int orChars, char c)
		{
			if (this.parsingMode == XmlTextReaderImpl.ParsingMode.Full && rcount > 0)
			{
				this.ShiftBuffer(rpos + rcount, rpos, pos - rpos - rcount);
			}
			int charPos = this.ps.charPos;
			int item = pos - rcount;
			this.ps.charPos = pos;
			return Task.FromResult<Tuple<int, int, int, bool>>(new Tuple<int, int, int, bool>(charPos, item, orChars, c == '<'));
		}

		private Task FinishPartialValueAsync()
		{
			XmlTextReaderImpl.<FinishPartialValueAsync>d__550 <FinishPartialValueAsync>d__;
			<FinishPartialValueAsync>d__.<>4__this = this;
			<FinishPartialValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishPartialValueAsync>d__.<>1__state = -1;
			<FinishPartialValueAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishPartialValueAsync>d__550>(ref <FinishPartialValueAsync>d__);
			return <FinishPartialValueAsync>d__.<>t__builder.Task;
		}

		private Task FinishOtherValueIteratorAsync()
		{
			XmlTextReaderImpl.<FinishOtherValueIteratorAsync>d__551 <FinishOtherValueIteratorAsync>d__;
			<FinishOtherValueIteratorAsync>d__.<>4__this = this;
			<FinishOtherValueIteratorAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishOtherValueIteratorAsync>d__.<>1__state = -1;
			<FinishOtherValueIteratorAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishOtherValueIteratorAsync>d__551>(ref <FinishOtherValueIteratorAsync>d__);
			return <FinishOtherValueIteratorAsync>d__.<>t__builder.Task;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private Task SkipPartialTextValueAsync()
		{
			XmlTextReaderImpl.<SkipPartialTextValueAsync>d__552 <SkipPartialTextValueAsync>d__;
			<SkipPartialTextValueAsync>d__.<>4__this = this;
			<SkipPartialTextValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SkipPartialTextValueAsync>d__.<>1__state = -1;
			<SkipPartialTextValueAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<SkipPartialTextValueAsync>d__552>(ref <SkipPartialTextValueAsync>d__);
			return <SkipPartialTextValueAsync>d__.<>t__builder.Task;
		}

		private Task FinishReadValueChunkAsync()
		{
			this.readValueOffset = 0;
			if (this.incReadState == XmlTextReaderImpl.IncrementalReadState.ReadValueChunk_OnPartialValue)
			{
				return this.SkipPartialTextValueAsync();
			}
			this.parsingFunction = this.nextParsingFunction;
			this.nextParsingFunction = this.nextNextParsingFunction;
			return AsyncHelper.DoneTask;
		}

		private Task FinishReadContentAsBinaryAsync()
		{
			XmlTextReaderImpl.<FinishReadContentAsBinaryAsync>d__554 <FinishReadContentAsBinaryAsync>d__;
			<FinishReadContentAsBinaryAsync>d__.<>4__this = this;
			<FinishReadContentAsBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishReadContentAsBinaryAsync>d__.<>1__state = -1;
			<FinishReadContentAsBinaryAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishReadContentAsBinaryAsync>d__554>(ref <FinishReadContentAsBinaryAsync>d__);
			return <FinishReadContentAsBinaryAsync>d__.<>t__builder.Task;
		}

		private Task FinishReadElementContentAsBinaryAsync()
		{
			XmlTextReaderImpl.<FinishReadElementContentAsBinaryAsync>d__555 <FinishReadElementContentAsBinaryAsync>d__;
			<FinishReadElementContentAsBinaryAsync>d__.<>4__this = this;
			<FinishReadElementContentAsBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FinishReadElementContentAsBinaryAsync>d__.<>1__state = -1;
			<FinishReadElementContentAsBinaryAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<FinishReadElementContentAsBinaryAsync>d__555>(ref <FinishReadElementContentAsBinaryAsync>d__);
			return <FinishReadElementContentAsBinaryAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseRootLevelWhitespaceAsync()
		{
			XmlTextReaderImpl.<ParseRootLevelWhitespaceAsync>d__556 <ParseRootLevelWhitespaceAsync>d__;
			<ParseRootLevelWhitespaceAsync>d__.<>4__this = this;
			<ParseRootLevelWhitespaceAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseRootLevelWhitespaceAsync>d__.<>1__state = -1;
			<ParseRootLevelWhitespaceAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseRootLevelWhitespaceAsync>d__556>(ref <ParseRootLevelWhitespaceAsync>d__);
			return <ParseRootLevelWhitespaceAsync>d__.<>t__builder.Task;
		}

		private Task ParseEntityReferenceAsync()
		{
			XmlTextReaderImpl.<ParseEntityReferenceAsync>d__557 <ParseEntityReferenceAsync>d__;
			<ParseEntityReferenceAsync>d__.<>4__this = this;
			<ParseEntityReferenceAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseEntityReferenceAsync>d__.<>1__state = -1;
			<ParseEntityReferenceAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseEntityReferenceAsync>d__557>(ref <ParseEntityReferenceAsync>d__);
			return <ParseEntityReferenceAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, XmlTextReaderImpl.EntityType>> HandleEntityReferenceAsync(bool isInAttributeValue, XmlTextReaderImpl.EntityExpandType expandType)
		{
			XmlTextReaderImpl.<HandleEntityReferenceAsync>d__558 <HandleEntityReferenceAsync>d__;
			<HandleEntityReferenceAsync>d__.<>4__this = this;
			<HandleEntityReferenceAsync>d__.isInAttributeValue = isInAttributeValue;
			<HandleEntityReferenceAsync>d__.expandType = expandType;
			<HandleEntityReferenceAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, XmlTextReaderImpl.EntityType>>.Create();
			<HandleEntityReferenceAsync>d__.<>1__state = -1;
			<HandleEntityReferenceAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<HandleEntityReferenceAsync>d__558>(ref <HandleEntityReferenceAsync>d__);
			return <HandleEntityReferenceAsync>d__.<>t__builder.Task;
		}

		private Task<XmlTextReaderImpl.EntityType> HandleGeneralEntityReferenceAsync(string name, bool isInAttributeValue, bool pushFakeEntityIfNullResolver, int entityStartLinePos)
		{
			XmlTextReaderImpl.<HandleGeneralEntityReferenceAsync>d__559 <HandleGeneralEntityReferenceAsync>d__;
			<HandleGeneralEntityReferenceAsync>d__.<>4__this = this;
			<HandleGeneralEntityReferenceAsync>d__.name = name;
			<HandleGeneralEntityReferenceAsync>d__.isInAttributeValue = isInAttributeValue;
			<HandleGeneralEntityReferenceAsync>d__.pushFakeEntityIfNullResolver = pushFakeEntityIfNullResolver;
			<HandleGeneralEntityReferenceAsync>d__.entityStartLinePos = entityStartLinePos;
			<HandleGeneralEntityReferenceAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XmlTextReaderImpl.EntityType>.Create();
			<HandleGeneralEntityReferenceAsync>d__.<>1__state = -1;
			<HandleGeneralEntityReferenceAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<HandleGeneralEntityReferenceAsync>d__559>(ref <HandleGeneralEntityReferenceAsync>d__);
			return <HandleGeneralEntityReferenceAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParsePIAsync()
		{
			return this.ParsePIAsync(null);
		}

		private Task<bool> ParsePIAsync(StringBuilder piInDtdStringBuilder)
		{
			XmlTextReaderImpl.<ParsePIAsync>d__561 <ParsePIAsync>d__;
			<ParsePIAsync>d__.<>4__this = this;
			<ParsePIAsync>d__.piInDtdStringBuilder = piInDtdStringBuilder;
			<ParsePIAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParsePIAsync>d__.<>1__state = -1;
			<ParsePIAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParsePIAsync>d__561>(ref <ParsePIAsync>d__);
			return <ParsePIAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int, bool>> ParsePIValueAsync()
		{
			XmlTextReaderImpl.<ParsePIValueAsync>d__562 <ParsePIValueAsync>d__;
			<ParsePIValueAsync>d__.<>4__this = this;
			<ParsePIValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int, bool>>.Create();
			<ParsePIValueAsync>d__.<>1__state = -1;
			<ParsePIValueAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParsePIValueAsync>d__562>(ref <ParsePIValueAsync>d__);
			return <ParsePIValueAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseCommentAsync()
		{
			XmlTextReaderImpl.<ParseCommentAsync>d__563 <ParseCommentAsync>d__;
			<ParseCommentAsync>d__.<>4__this = this;
			<ParseCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseCommentAsync>d__.<>1__state = -1;
			<ParseCommentAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseCommentAsync>d__563>(ref <ParseCommentAsync>d__);
			return <ParseCommentAsync>d__.<>t__builder.Task;
		}

		private Task ParseCDataAsync()
		{
			return this.ParseCDataOrCommentAsync(XmlNodeType.CDATA);
		}

		private Task ParseCDataOrCommentAsync(XmlNodeType type)
		{
			XmlTextReaderImpl.<ParseCDataOrCommentAsync>d__565 <ParseCDataOrCommentAsync>d__;
			<ParseCDataOrCommentAsync>d__.<>4__this = this;
			<ParseCDataOrCommentAsync>d__.type = type;
			<ParseCDataOrCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseCDataOrCommentAsync>d__.<>1__state = -1;
			<ParseCDataOrCommentAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseCDataOrCommentAsync>d__565>(ref <ParseCDataOrCommentAsync>d__);
			return <ParseCDataOrCommentAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int, bool>> ParseCDataOrCommentTupleAsync(XmlNodeType type)
		{
			XmlTextReaderImpl.<ParseCDataOrCommentTupleAsync>d__566 <ParseCDataOrCommentTupleAsync>d__;
			<ParseCDataOrCommentTupleAsync>d__.<>4__this = this;
			<ParseCDataOrCommentTupleAsync>d__.type = type;
			<ParseCDataOrCommentTupleAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int, bool>>.Create();
			<ParseCDataOrCommentTupleAsync>d__.<>1__state = -1;
			<ParseCDataOrCommentTupleAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseCDataOrCommentTupleAsync>d__566>(ref <ParseCDataOrCommentTupleAsync>d__);
			return <ParseCDataOrCommentTupleAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ParseDoctypeDeclAsync()
		{
			XmlTextReaderImpl.<ParseDoctypeDeclAsync>d__567 <ParseDoctypeDeclAsync>d__;
			<ParseDoctypeDeclAsync>d__.<>4__this = this;
			<ParseDoctypeDeclAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ParseDoctypeDeclAsync>d__.<>1__state = -1;
			<ParseDoctypeDeclAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseDoctypeDeclAsync>d__567>(ref <ParseDoctypeDeclAsync>d__);
			return <ParseDoctypeDeclAsync>d__.<>t__builder.Task;
		}

		private Task ParseDtdAsync()
		{
			XmlTextReaderImpl.<ParseDtdAsync>d__568 <ParseDtdAsync>d__;
			<ParseDtdAsync>d__.<>4__this = this;
			<ParseDtdAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseDtdAsync>d__.<>1__state = -1;
			<ParseDtdAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseDtdAsync>d__568>(ref <ParseDtdAsync>d__);
			return <ParseDtdAsync>d__.<>t__builder.Task;
		}

		private Task SkipDtdAsync()
		{
			XmlTextReaderImpl.<SkipDtdAsync>d__569 <SkipDtdAsync>d__;
			<SkipDtdAsync>d__.<>4__this = this;
			<SkipDtdAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SkipDtdAsync>d__.<>1__state = -1;
			<SkipDtdAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<SkipDtdAsync>d__569>(ref <SkipDtdAsync>d__);
			return <SkipDtdAsync>d__.<>t__builder.Task;
		}

		private Task SkipPublicOrSystemIdLiteralAsync()
		{
			char c = this.ps.chars[this.ps.charPos];
			if (c != '"' && c != '\'')
			{
				this.ThrowUnexpectedToken("\"", "'");
			}
			this.ps.charPos = this.ps.charPos + 1;
			return this.SkipUntilAsync(c, false);
		}

		private Task SkipUntilAsync(char stopChar, bool recognizeLiterals)
		{
			XmlTextReaderImpl.<SkipUntilAsync>d__571 <SkipUntilAsync>d__;
			<SkipUntilAsync>d__.<>4__this = this;
			<SkipUntilAsync>d__.stopChar = stopChar;
			<SkipUntilAsync>d__.recognizeLiterals = recognizeLiterals;
			<SkipUntilAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SkipUntilAsync>d__.<>1__state = -1;
			<SkipUntilAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<SkipUntilAsync>d__571>(ref <SkipUntilAsync>d__);
			return <SkipUntilAsync>d__.<>t__builder.Task;
		}

		private Task<int> EatWhitespacesAsync(StringBuilder sb)
		{
			XmlTextReaderImpl.<EatWhitespacesAsync>d__572 <EatWhitespacesAsync>d__;
			<EatWhitespacesAsync>d__.<>4__this = this;
			<EatWhitespacesAsync>d__.sb = sb;
			<EatWhitespacesAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<EatWhitespacesAsync>d__.<>1__state = -1;
			<EatWhitespacesAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<EatWhitespacesAsync>d__572>(ref <EatWhitespacesAsync>d__);
			return <EatWhitespacesAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<XmlTextReaderImpl.EntityType, int>> ParseNumericCharRefAsync(bool expand, StringBuilder internalSubsetBuilder)
		{
			XmlTextReaderImpl.<ParseNumericCharRefAsync>d__573 <ParseNumericCharRefAsync>d__;
			<ParseNumericCharRefAsync>d__.<>4__this = this;
			<ParseNumericCharRefAsync>d__.expand = expand;
			<ParseNumericCharRefAsync>d__.internalSubsetBuilder = internalSubsetBuilder;
			<ParseNumericCharRefAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<XmlTextReaderImpl.EntityType, int>>.Create();
			<ParseNumericCharRefAsync>d__.<>1__state = -1;
			<ParseNumericCharRefAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseNumericCharRefAsync>d__573>(ref <ParseNumericCharRefAsync>d__);
			return <ParseNumericCharRefAsync>d__.<>t__builder.Task;
		}

		private Task<int> ParseNamedCharRefAsync(bool expand, StringBuilder internalSubsetBuilder)
		{
			XmlTextReaderImpl.<ParseNamedCharRefAsync>d__574 <ParseNamedCharRefAsync>d__;
			<ParseNamedCharRefAsync>d__.<>4__this = this;
			<ParseNamedCharRefAsync>d__.expand = expand;
			<ParseNamedCharRefAsync>d__.internalSubsetBuilder = internalSubsetBuilder;
			<ParseNamedCharRefAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ParseNamedCharRefAsync>d__.<>1__state = -1;
			<ParseNamedCharRefAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseNamedCharRefAsync>d__574>(ref <ParseNamedCharRefAsync>d__);
			return <ParseNamedCharRefAsync>d__.<>t__builder.Task;
		}

		private Task<int> ParseNameAsync()
		{
			XmlTextReaderImpl.<ParseNameAsync>d__575 <ParseNameAsync>d__;
			<ParseNameAsync>d__.<>4__this = this;
			<ParseNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ParseNameAsync>d__.<>1__state = -1;
			<ParseNameAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseNameAsync>d__575>(ref <ParseNameAsync>d__);
			return <ParseNameAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, int>> ParseQNameAsync()
		{
			return this.ParseQNameAsync(true, 0);
		}

		private Task<Tuple<int, int>> ParseQNameAsync(bool isQName, int startOffset)
		{
			XmlTextReaderImpl.<ParseQNameAsync>d__577 <ParseQNameAsync>d__;
			<ParseQNameAsync>d__.<>4__this = this;
			<ParseQNameAsync>d__.isQName = isQName;
			<ParseQNameAsync>d__.startOffset = startOffset;
			<ParseQNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, int>>.Create();
			<ParseQNameAsync>d__.<>1__state = -1;
			<ParseQNameAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseQNameAsync>d__577>(ref <ParseQNameAsync>d__);
			return <ParseQNameAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<int, bool>> ReadDataInNameAsync(int pos)
		{
			XmlTextReaderImpl.<ReadDataInNameAsync>d__578 <ReadDataInNameAsync>d__;
			<ReadDataInNameAsync>d__.<>4__this = this;
			<ReadDataInNameAsync>d__.pos = pos;
			<ReadDataInNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, bool>>.Create();
			<ReadDataInNameAsync>d__.<>1__state = -1;
			<ReadDataInNameAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadDataInNameAsync>d__578>(ref <ReadDataInNameAsync>d__);
			return <ReadDataInNameAsync>d__.<>t__builder.Task;
		}

		private Task<string> ParseEntityNameAsync()
		{
			XmlTextReaderImpl.<ParseEntityNameAsync>d__579 <ParseEntityNameAsync>d__;
			<ParseEntityNameAsync>d__.<>4__this = this;
			<ParseEntityNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ParseEntityNameAsync>d__.<>1__state = -1;
			<ParseEntityNameAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseEntityNameAsync>d__579>(ref <ParseEntityNameAsync>d__);
			return <ParseEntityNameAsync>d__.<>t__builder.Task;
		}

		private Task PushExternalEntityOrSubsetAsync(string publicId, string systemId, Uri baseUri, string entityName)
		{
			XmlTextReaderImpl.<PushExternalEntityOrSubsetAsync>d__580 <PushExternalEntityOrSubsetAsync>d__;
			<PushExternalEntityOrSubsetAsync>d__.<>4__this = this;
			<PushExternalEntityOrSubsetAsync>d__.publicId = publicId;
			<PushExternalEntityOrSubsetAsync>d__.systemId = systemId;
			<PushExternalEntityOrSubsetAsync>d__.baseUri = baseUri;
			<PushExternalEntityOrSubsetAsync>d__.entityName = entityName;
			<PushExternalEntityOrSubsetAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<PushExternalEntityOrSubsetAsync>d__.<>1__state = -1;
			<PushExternalEntityOrSubsetAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<PushExternalEntityOrSubsetAsync>d__580>(ref <PushExternalEntityOrSubsetAsync>d__);
			return <PushExternalEntityOrSubsetAsync>d__.<>t__builder.Task;
		}

		private Task<bool> OpenAndPushAsync(Uri uri)
		{
			XmlTextReaderImpl.<OpenAndPushAsync>d__581 <OpenAndPushAsync>d__;
			<OpenAndPushAsync>d__.<>4__this = this;
			<OpenAndPushAsync>d__.uri = uri;
			<OpenAndPushAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<OpenAndPushAsync>d__.<>1__state = -1;
			<OpenAndPushAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<OpenAndPushAsync>d__581>(ref <OpenAndPushAsync>d__);
			return <OpenAndPushAsync>d__.<>t__builder.Task;
		}

		private Task<bool> PushExternalEntityAsync(IDtdEntityInfo entity)
		{
			XmlTextReaderImpl.<PushExternalEntityAsync>d__582 <PushExternalEntityAsync>d__;
			<PushExternalEntityAsync>d__.<>4__this = this;
			<PushExternalEntityAsync>d__.entity = entity;
			<PushExternalEntityAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<PushExternalEntityAsync>d__.<>1__state = -1;
			<PushExternalEntityAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<PushExternalEntityAsync>d__582>(ref <PushExternalEntityAsync>d__);
			return <PushExternalEntityAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ZeroEndingStreamAsync(int pos)
		{
			XmlTextReaderImpl.<ZeroEndingStreamAsync>d__583 <ZeroEndingStreamAsync>d__;
			<ZeroEndingStreamAsync>d__.<>4__this = this;
			<ZeroEndingStreamAsync>d__.pos = pos;
			<ZeroEndingStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ZeroEndingStreamAsync>d__.<>1__state = -1;
			<ZeroEndingStreamAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ZeroEndingStreamAsync>d__583>(ref <ZeroEndingStreamAsync>d__);
			return <ZeroEndingStreamAsync>d__.<>t__builder.Task;
		}

		private Task ParseDtdFromParserContextAsync()
		{
			XmlTextReaderImpl.<ParseDtdFromParserContextAsync>d__584 <ParseDtdFromParserContextAsync>d__;
			<ParseDtdFromParserContextAsync>d__.<>4__this = this;
			<ParseDtdFromParserContextAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseDtdFromParserContextAsync>d__.<>1__state = -1;
			<ParseDtdFromParserContextAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ParseDtdFromParserContextAsync>d__584>(ref <ParseDtdFromParserContextAsync>d__);
			return <ParseDtdFromParserContextAsync>d__.<>t__builder.Task;
		}

		private Task<bool> InitReadContentAsBinaryAsync()
		{
			XmlTextReaderImpl.<InitReadContentAsBinaryAsync>d__585 <InitReadContentAsBinaryAsync>d__;
			<InitReadContentAsBinaryAsync>d__.<>4__this = this;
			<InitReadContentAsBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<InitReadContentAsBinaryAsync>d__.<>1__state = -1;
			<InitReadContentAsBinaryAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<InitReadContentAsBinaryAsync>d__585>(ref <InitReadContentAsBinaryAsync>d__);
			return <InitReadContentAsBinaryAsync>d__.<>t__builder.Task;
		}

		private Task<bool> InitReadElementContentAsBinaryAsync()
		{
			XmlTextReaderImpl.<InitReadElementContentAsBinaryAsync>d__586 <InitReadElementContentAsBinaryAsync>d__;
			<InitReadElementContentAsBinaryAsync>d__.<>4__this = this;
			<InitReadElementContentAsBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<InitReadElementContentAsBinaryAsync>d__.<>1__state = -1;
			<InitReadElementContentAsBinaryAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<InitReadElementContentAsBinaryAsync>d__586>(ref <InitReadElementContentAsBinaryAsync>d__);
			return <InitReadElementContentAsBinaryAsync>d__.<>t__builder.Task;
		}

		private Task<bool> MoveToNextContentNodeAsync(bool moveIfOnContentNode)
		{
			XmlTextReaderImpl.<MoveToNextContentNodeAsync>d__587 <MoveToNextContentNodeAsync>d__;
			<MoveToNextContentNodeAsync>d__.<>4__this = this;
			<MoveToNextContentNodeAsync>d__.moveIfOnContentNode = moveIfOnContentNode;
			<MoveToNextContentNodeAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<MoveToNextContentNodeAsync>d__.<>1__state = -1;
			<MoveToNextContentNodeAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<MoveToNextContentNodeAsync>d__587>(ref <MoveToNextContentNodeAsync>d__);
			return <MoveToNextContentNodeAsync>d__.<>t__builder.Task;
		}

		private Task<int> ReadContentAsBinaryAsync(byte[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadContentAsBinaryAsync>d__588 <ReadContentAsBinaryAsync>d__;
			<ReadContentAsBinaryAsync>d__.<>4__this = this;
			<ReadContentAsBinaryAsync>d__.buffer = buffer;
			<ReadContentAsBinaryAsync>d__.index = index;
			<ReadContentAsBinaryAsync>d__.count = count;
			<ReadContentAsBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadContentAsBinaryAsync>d__.<>1__state = -1;
			<ReadContentAsBinaryAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadContentAsBinaryAsync>d__588>(ref <ReadContentAsBinaryAsync>d__);
			return <ReadContentAsBinaryAsync>d__.<>t__builder.Task;
		}

		private Task<int> ReadElementContentAsBinaryAsync(byte[] buffer, int index, int count)
		{
			XmlTextReaderImpl.<ReadElementContentAsBinaryAsync>d__589 <ReadElementContentAsBinaryAsync>d__;
			<ReadElementContentAsBinaryAsync>d__.<>4__this = this;
			<ReadElementContentAsBinaryAsync>d__.buffer = buffer;
			<ReadElementContentAsBinaryAsync>d__.index = index;
			<ReadElementContentAsBinaryAsync>d__.count = count;
			<ReadElementContentAsBinaryAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadElementContentAsBinaryAsync>d__.<>1__state = -1;
			<ReadElementContentAsBinaryAsync>d__.<>t__builder.Start<XmlTextReaderImpl.<ReadElementContentAsBinaryAsync>d__589>(ref <ReadElementContentAsBinaryAsync>d__);
			return <ReadElementContentAsBinaryAsync>d__.<>t__builder.Task;
		}

		private readonly bool useAsync;

		private XmlTextReaderImpl.LaterInitParam laterInitParam;

		private XmlCharType xmlCharType = XmlCharType.Instance;

		private XmlTextReaderImpl.ParsingState ps;

		private XmlTextReaderImpl.ParsingFunction parsingFunction;

		private XmlTextReaderImpl.ParsingFunction nextParsingFunction;

		private XmlTextReaderImpl.ParsingFunction nextNextParsingFunction;

		private XmlTextReaderImpl.NodeData[] nodes;

		private XmlTextReaderImpl.NodeData curNode;

		private int index;

		private int curAttrIndex = -1;

		private int attrCount;

		private int attrHashtable;

		private int attrDuplWalkCount;

		private bool attrNeedNamespaceLookup;

		private bool fullAttrCleanup;

		private XmlTextReaderImpl.NodeData[] attrDuplSortingArray;

		private XmlNameTable nameTable;

		private bool nameTableFromSettings;

		private XmlResolver xmlResolver;

		private string url = string.Empty;

		private bool normalize;

		private bool supportNamespaces = true;

		private WhitespaceHandling whitespaceHandling;

		private DtdProcessing dtdProcessing = DtdProcessing.Parse;

		private EntityHandling entityHandling;

		private bool ignorePIs;

		private bool ignoreComments;

		private bool checkCharacters;

		private int lineNumberOffset;

		private int linePositionOffset;

		private bool closeInput;

		private long maxCharactersInDocument;

		private long maxCharactersFromEntities;

		private bool v1Compat;

		private XmlNamespaceManager namespaceManager;

		private string lastPrefix = string.Empty;

		private XmlTextReaderImpl.XmlContext xmlContext;

		private XmlTextReaderImpl.ParsingState[] parsingStatesStack;

		private int parsingStatesStackTop = -1;

		private string reportedBaseUri;

		private Encoding reportedEncoding;

		private IDtdInfo dtdInfo;

		private XmlNodeType fragmentType = XmlNodeType.Document;

		private XmlParserContext fragmentParserContext;

		private bool fragment;

		private IncrementalReadDecoder incReadDecoder;

		private XmlTextReaderImpl.IncrementalReadState incReadState;

		private LineInfo incReadLineInfo;

		private BinHexDecoder binHexDecoder;

		private Base64Decoder base64Decoder;

		private int incReadDepth;

		private int incReadLeftStartPos;

		private int incReadLeftEndPos;

		private IncrementalReadCharsDecoder readCharsDecoder;

		private int attributeValueBaseEntityId;

		private bool emptyEntityInAttributeResolved;

		private IValidationEventHandling validationEventHandling;

		private XmlTextReaderImpl.OnDefaultAttributeUseDelegate onDefaultAttributeUse;

		private bool validatingReaderCompatFlag;

		private bool addDefaultAttributesAndNormalize;

		private StringBuilder stringBuilder;

		private bool rootElementParsed;

		private bool standalone;

		private int nextEntityId = 1;

		private XmlTextReaderImpl.ParsingMode parsingMode;

		private ReadState readState;

		private IDtdEntityInfo lastEntity;

		private bool afterResetState;

		private int documentStartBytePos;

		private int readValueOffset;

		private long charactersInDocument;

		private long charactersFromEntities;

		private Dictionary<IDtdEntityInfo, IDtdEntityInfo> currentEntities;

		private bool disableUndeclaredEntityCheck;

		private XmlReader outerReader;

		private bool xmlResolverIsSet;

		private string Xml;

		private string XmlNs;

		private const int MaxBytesToMove = 128;

		private const int ApproxXmlDeclLength = 80;

		private const int NodesInitialSize = 8;

		private const int InitialAttributesCount = 4;

		private const int InitialParsingStateStackSize = 2;

		private const int InitialParsingStatesDepth = 2;

		private const int DtdChidrenInitialSize = 2;

		private const int MaxByteSequenceLen = 6;

		private const int MaxAttrDuplWalkCount = 250;

		private const int MinWhitespaceLookahedCount = 4096;

		private const string XmlDeclarationBegining = "<?xml";

		private XmlTextReaderImpl.ParseEndElementParseFunction parseEndElement_NextFunc;

		private XmlTextReaderImpl.ParseTextFunction parseText_NextFunction;

		private XmlTextReaderImpl.ParseTextState lastParseTextState;

		private Task<Tuple<int, int, int, bool>> parseText_dummyTask = Task.FromResult<Tuple<int, int, int, bool>>(new Tuple<int, int, int, bool>(0, 0, 0, false));

		private enum ParsingFunction
		{
			ElementContent,
			NoData,
			OpenUrl,
			SwitchToInteractive,
			SwitchToInteractiveXmlDecl,
			DocumentContent,
			MoveToElementContent,
			PopElementContext,
			PopEmptyElementContext,
			ResetAttributesRootLevel,
			Error,
			Eof,
			ReaderClosed,
			EntityReference,
			InIncrementalRead,
			FragmentAttribute,
			ReportEndEntity,
			AfterResolveEntityInContent,
			AfterResolveEmptyEntityInContent,
			XmlDeclarationFragment,
			GoToEof,
			PartialTextValue,
			InReadAttributeValue,
			InReadValueChunk,
			InReadContentAsBinary,
			InReadElementContentAsBinary
		}

		private enum ParsingMode
		{
			Full,
			SkipNode,
			SkipContent
		}

		private enum EntityType
		{
			CharacterDec,
			CharacterHex,
			CharacterNamed,
			Expanded,
			Skipped,
			FakeExpanded,
			Unexpanded,
			ExpandedInAttribute
		}

		private enum EntityExpandType
		{
			All,
			OnlyGeneral,
			OnlyCharacter
		}

		private enum IncrementalReadState
		{
			Text,
			StartTag,
			PI,
			CDATA,
			Comment,
			Attributes,
			AttributeValue,
			ReadData,
			EndElement,
			End,
			ReadValueChunk_OnCachedValue,
			ReadValueChunk_OnPartialValue,
			ReadContentAsBinary_OnCachedValue,
			ReadContentAsBinary_OnPartialValue,
			ReadContentAsBinary_End
		}

		private class LaterInitParam
		{
			public bool useAsync;

			public Stream inputStream;

			public byte[] inputBytes;

			public int inputByteCount;

			public Uri inputbaseUri;

			public string inputUriStr;

			public XmlResolver inputUriResolver;

			public XmlParserContext inputContext;

			public TextReader inputTextReader;

			public XmlTextReaderImpl.InitInputType initType = XmlTextReaderImpl.InitInputType.Invalid;
		}

		private enum InitInputType
		{
			UriString,
			Stream,
			TextReader,
			Invalid
		}

		private enum ParseEndElementParseFunction
		{
			CheckEndTag,
			ReadData,
			Done
		}

		private class ParseTextState
		{
			public ParseTextState(int outOrChars, char[] chars, int pos, int rcount, int rpos, int orChars, char c)
			{
				this.outOrChars = outOrChars;
				this.chars = chars;
				this.pos = pos;
				this.rcount = rcount;
				this.rpos = rpos;
				this.orChars = orChars;
				this.c = c;
			}

			public int outOrChars;

			public char[] chars;

			public int pos;

			public int rcount;

			public int rpos;

			public int orChars;

			public char c;
		}

		private enum ParseTextFunction
		{
			ParseText,
			Entity,
			Surrogate,
			ReadData,
			NoValue,
			PartialValue
		}

		private struct ParsingState
		{
			internal void Clear()
			{
				this.chars = null;
				this.charPos = 0;
				this.charsUsed = 0;
				this.encoding = null;
				this.stream = null;
				this.decoder = null;
				this.bytes = null;
				this.bytePos = 0;
				this.bytesUsed = 0;
				this.textReader = null;
				this.lineNo = 1;
				this.lineStartPos = -1;
				this.baseUriStr = string.Empty;
				this.baseUri = null;
				this.isEof = false;
				this.isStreamEof = false;
				this.eolNormalized = true;
				this.entityResolvedManually = false;
			}

			internal void Close(bool closeInput)
			{
				if (closeInput)
				{
					if (this.stream != null)
					{
						this.stream.Close();
						return;
					}
					if (this.textReader != null)
					{
						this.textReader.Close();
					}
				}
			}

			internal int LineNo
			{
				get
				{
					return this.lineNo;
				}
			}

			internal int LinePos
			{
				get
				{
					return this.charPos - this.lineStartPos;
				}
			}

			internal char[] chars;

			internal int charPos;

			internal int charsUsed;

			internal Encoding encoding;

			internal bool appendMode;

			internal Stream stream;

			internal Decoder decoder;

			internal byte[] bytes;

			internal int bytePos;

			internal int bytesUsed;

			internal TextReader textReader;

			internal int lineNo;

			internal int lineStartPos;

			internal string baseUriStr;

			internal Uri baseUri;

			internal bool isEof;

			internal bool isStreamEof;

			internal IDtdEntityInfo entity;

			internal int entityId;

			internal bool eolNormalized;

			internal bool entityResolvedManually;
		}

		private class XmlContext
		{
			internal XmlContext()
			{
				this.xmlSpace = XmlSpace.None;
				this.xmlLang = string.Empty;
				this.defaultNamespace = string.Empty;
				this.previousContext = null;
			}

			internal XmlContext(XmlTextReaderImpl.XmlContext previousContext)
			{
				this.xmlSpace = previousContext.xmlSpace;
				this.xmlLang = previousContext.xmlLang;
				this.defaultNamespace = previousContext.defaultNamespace;
				this.previousContext = previousContext;
			}

			internal XmlSpace xmlSpace;

			internal string xmlLang;

			internal string defaultNamespace;

			internal XmlTextReaderImpl.XmlContext previousContext;
		}

		private class NoNamespaceManager : XmlNamespaceManager
		{
			public override string DefaultNamespace
			{
				get
				{
					return string.Empty;
				}
			}

			public override void PushScope()
			{
			}

			public override bool PopScope()
			{
				return false;
			}

			public override void AddNamespace(string prefix, string uri)
			{
			}

			public override void RemoveNamespace(string prefix, string uri)
			{
			}

			public override IEnumerator GetEnumerator()
			{
				return null;
			}

			public override IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
			{
				return null;
			}

			public override string LookupNamespace(string prefix)
			{
				return string.Empty;
			}

			public override string LookupPrefix(string uri)
			{
				return null;
			}

			public override bool HasNamespace(string prefix)
			{
				return false;
			}
		}

		internal class DtdParserProxy : IDtdParserAdapterV1, IDtdParserAdapterWithValidation, IDtdParserAdapter
		{
			internal DtdParserProxy(XmlTextReaderImpl reader)
			{
				this.reader = reader;
			}

			XmlNameTable IDtdParserAdapter.NameTable
			{
				get
				{
					return this.reader.DtdParserProxy_NameTable;
				}
			}

			IXmlNamespaceResolver IDtdParserAdapter.NamespaceResolver
			{
				get
				{
					return this.reader.DtdParserProxy_NamespaceResolver;
				}
			}

			Uri IDtdParserAdapter.BaseUri
			{
				get
				{
					return this.reader.DtdParserProxy_BaseUri;
				}
			}

			bool IDtdParserAdapter.IsEof
			{
				get
				{
					return this.reader.DtdParserProxy_IsEof;
				}
			}

			char[] IDtdParserAdapter.ParsingBuffer
			{
				get
				{
					return this.reader.DtdParserProxy_ParsingBuffer;
				}
			}

			int IDtdParserAdapter.ParsingBufferLength
			{
				get
				{
					return this.reader.DtdParserProxy_ParsingBufferLength;
				}
			}

			int IDtdParserAdapter.CurrentPosition
			{
				get
				{
					return this.reader.DtdParserProxy_CurrentPosition;
				}
				set
				{
					this.reader.DtdParserProxy_CurrentPosition = value;
				}
			}

			int IDtdParserAdapter.EntityStackLength
			{
				get
				{
					return this.reader.DtdParserProxy_EntityStackLength;
				}
			}

			bool IDtdParserAdapter.IsEntityEolNormalized
			{
				get
				{
					return this.reader.DtdParserProxy_IsEntityEolNormalized;
				}
			}

			void IDtdParserAdapter.OnNewLine(int pos)
			{
				this.reader.DtdParserProxy_OnNewLine(pos);
			}

			int IDtdParserAdapter.LineNo
			{
				get
				{
					return this.reader.DtdParserProxy_LineNo;
				}
			}

			int IDtdParserAdapter.LineStartPosition
			{
				get
				{
					return this.reader.DtdParserProxy_LineStartPosition;
				}
			}

			int IDtdParserAdapter.ReadData()
			{
				return this.reader.DtdParserProxy_ReadData();
			}

			int IDtdParserAdapter.ParseNumericCharRef(StringBuilder internalSubsetBuilder)
			{
				return this.reader.DtdParserProxy_ParseNumericCharRef(internalSubsetBuilder);
			}

			int IDtdParserAdapter.ParseNamedCharRef(bool expand, StringBuilder internalSubsetBuilder)
			{
				return this.reader.DtdParserProxy_ParseNamedCharRef(expand, internalSubsetBuilder);
			}

			void IDtdParserAdapter.ParsePI(StringBuilder sb)
			{
				this.reader.DtdParserProxy_ParsePI(sb);
			}

			void IDtdParserAdapter.ParseComment(StringBuilder sb)
			{
				this.reader.DtdParserProxy_ParseComment(sb);
			}

			bool IDtdParserAdapter.PushEntity(IDtdEntityInfo entity, out int entityId)
			{
				return this.reader.DtdParserProxy_PushEntity(entity, out entityId);
			}

			bool IDtdParserAdapter.PopEntity(out IDtdEntityInfo oldEntity, out int newEntityId)
			{
				return this.reader.DtdParserProxy_PopEntity(out oldEntity, out newEntityId);
			}

			bool IDtdParserAdapter.PushExternalSubset(string systemId, string publicId)
			{
				return this.reader.DtdParserProxy_PushExternalSubset(systemId, publicId);
			}

			void IDtdParserAdapter.PushInternalDtd(string baseUri, string internalDtd)
			{
				this.reader.DtdParserProxy_PushInternalDtd(baseUri, internalDtd);
			}

			void IDtdParserAdapter.Throw(Exception e)
			{
				this.reader.DtdParserProxy_Throw(e);
			}

			void IDtdParserAdapter.OnSystemId(string systemId, LineInfo keywordLineInfo, LineInfo systemLiteralLineInfo)
			{
				this.reader.DtdParserProxy_OnSystemId(systemId, keywordLineInfo, systemLiteralLineInfo);
			}

			void IDtdParserAdapter.OnPublicId(string publicId, LineInfo keywordLineInfo, LineInfo publicLiteralLineInfo)
			{
				this.reader.DtdParserProxy_OnPublicId(publicId, keywordLineInfo, publicLiteralLineInfo);
			}

			bool IDtdParserAdapterWithValidation.DtdValidation
			{
				get
				{
					return this.reader.DtdParserProxy_DtdValidation;
				}
			}

			IValidationEventHandling IDtdParserAdapterWithValidation.ValidationEventHandling
			{
				get
				{
					return this.reader.DtdParserProxy_ValidationEventHandling;
				}
			}

			bool IDtdParserAdapterV1.Normalization
			{
				get
				{
					return this.reader.DtdParserProxy_Normalization;
				}
			}

			bool IDtdParserAdapterV1.Namespaces
			{
				get
				{
					return this.reader.DtdParserProxy_Namespaces;
				}
			}

			bool IDtdParserAdapterV1.V1CompatibilityMode
			{
				get
				{
					return this.reader.DtdParserProxy_V1CompatibilityMode;
				}
			}

			Task<int> IDtdParserAdapter.ReadDataAsync()
			{
				return this.reader.DtdParserProxy_ReadDataAsync();
			}

			Task<int> IDtdParserAdapter.ParseNumericCharRefAsync(StringBuilder internalSubsetBuilder)
			{
				return this.reader.DtdParserProxy_ParseNumericCharRefAsync(internalSubsetBuilder);
			}

			Task<int> IDtdParserAdapter.ParseNamedCharRefAsync(bool expand, StringBuilder internalSubsetBuilder)
			{
				return this.reader.DtdParserProxy_ParseNamedCharRefAsync(expand, internalSubsetBuilder);
			}

			Task IDtdParserAdapter.ParsePIAsync(StringBuilder sb)
			{
				return this.reader.DtdParserProxy_ParsePIAsync(sb);
			}

			Task IDtdParserAdapter.ParseCommentAsync(StringBuilder sb)
			{
				return this.reader.DtdParserProxy_ParseCommentAsync(sb);
			}

			Task<Tuple<int, bool>> IDtdParserAdapter.PushEntityAsync(IDtdEntityInfo entity)
			{
				return this.reader.DtdParserProxy_PushEntityAsync(entity);
			}

			Task<bool> IDtdParserAdapter.PushExternalSubsetAsync(string systemId, string publicId)
			{
				return this.reader.DtdParserProxy_PushExternalSubsetAsync(systemId, publicId);
			}

			private XmlTextReaderImpl reader;
		}

		private class NodeData : IComparable
		{
			internal static XmlTextReaderImpl.NodeData None
			{
				get
				{
					if (XmlTextReaderImpl.NodeData.s_None == null)
					{
						XmlTextReaderImpl.NodeData.s_None = new XmlTextReaderImpl.NodeData();
					}
					return XmlTextReaderImpl.NodeData.s_None;
				}
			}

			internal NodeData()
			{
				this.Clear(XmlNodeType.None);
				this.xmlContextPushed = false;
			}

			internal int LineNo
			{
				get
				{
					return this.lineInfo.lineNo;
				}
			}

			internal int LinePos
			{
				get
				{
					return this.lineInfo.linePos;
				}
			}

			internal bool IsEmptyElement
			{
				get
				{
					return this.type == XmlNodeType.Element && this.isEmptyOrDefault;
				}
				set
				{
					this.isEmptyOrDefault = value;
				}
			}

			internal bool IsDefaultAttribute
			{
				get
				{
					return this.type == XmlNodeType.Attribute && this.isEmptyOrDefault;
				}
				set
				{
					this.isEmptyOrDefault = value;
				}
			}

			internal bool ValueBuffered
			{
				get
				{
					return this.value == null;
				}
			}

			internal string StringValue
			{
				get
				{
					if (this.value == null)
					{
						this.value = new string(this.chars, this.valueStartPos, this.valueLength);
					}
					return this.value;
				}
			}

			internal void TrimSpacesInValue()
			{
				if (this.ValueBuffered)
				{
					XmlTextReaderImpl.StripSpaces(this.chars, this.valueStartPos, ref this.valueLength);
					return;
				}
				this.value = XmlTextReaderImpl.StripSpaces(this.value);
			}

			internal void Clear(XmlNodeType type)
			{
				this.type = type;
				this.ClearName();
				this.value = string.Empty;
				this.valueStartPos = -1;
				this.nameWPrefix = string.Empty;
				this.schemaType = null;
				this.typedValue = null;
			}

			internal void ClearName()
			{
				this.localName = string.Empty;
				this.prefix = string.Empty;
				this.ns = string.Empty;
				this.nameWPrefix = string.Empty;
			}

			internal void SetLineInfo(int lineNo, int linePos)
			{
				this.lineInfo.Set(lineNo, linePos);
			}

			internal void SetLineInfo2(int lineNo, int linePos)
			{
				this.lineInfo2.Set(lineNo, linePos);
			}

			internal void SetValueNode(XmlNodeType type, string value)
			{
				this.type = type;
				this.ClearName();
				this.value = value;
				this.valueStartPos = -1;
			}

			internal void SetValueNode(XmlNodeType type, char[] chars, int startPos, int len)
			{
				this.type = type;
				this.ClearName();
				this.value = null;
				this.chars = chars;
				this.valueStartPos = startPos;
				this.valueLength = len;
			}

			internal void SetNamedNode(XmlNodeType type, string localName)
			{
				this.SetNamedNode(type, localName, string.Empty, localName);
			}

			internal void SetNamedNode(XmlNodeType type, string localName, string prefix, string nameWPrefix)
			{
				this.type = type;
				this.localName = localName;
				this.prefix = prefix;
				this.nameWPrefix = nameWPrefix;
				this.ns = string.Empty;
				this.value = string.Empty;
				this.valueStartPos = -1;
			}

			internal void SetValue(string value)
			{
				this.valueStartPos = -1;
				this.value = value;
			}

			internal void SetValue(char[] chars, int startPos, int len)
			{
				this.value = null;
				this.chars = chars;
				this.valueStartPos = startPos;
				this.valueLength = len;
			}

			internal void OnBufferInvalidated()
			{
				if (this.value == null)
				{
					this.value = new string(this.chars, this.valueStartPos, this.valueLength);
				}
				this.valueStartPos = -1;
			}

			internal void CopyTo(int valueOffset, StringBuilder sb)
			{
				if (this.value == null)
				{
					sb.Append(this.chars, this.valueStartPos + valueOffset, this.valueLength - valueOffset);
					return;
				}
				if (valueOffset <= 0)
				{
					sb.Append(this.value);
					return;
				}
				sb.Append(this.value, valueOffset, this.value.Length - valueOffset);
			}

			internal int CopyTo(int valueOffset, char[] buffer, int offset, int length)
			{
				if (this.value == null)
				{
					int num = this.valueLength - valueOffset;
					if (num > length)
					{
						num = length;
					}
					XmlTextReaderImpl.BlockCopyChars(this.chars, this.valueStartPos + valueOffset, buffer, offset, num);
					return num;
				}
				int num2 = this.value.Length - valueOffset;
				if (num2 > length)
				{
					num2 = length;
				}
				this.value.CopyTo(valueOffset, buffer, offset, num2);
				return num2;
			}

			internal int CopyToBinary(IncrementalReadDecoder decoder, int valueOffset)
			{
				if (this.value == null)
				{
					return decoder.Decode(this.chars, this.valueStartPos + valueOffset, this.valueLength - valueOffset);
				}
				return decoder.Decode(this.value, valueOffset, this.value.Length - valueOffset);
			}

			internal void AdjustLineInfo(int valueOffset, bool isNormalized, ref LineInfo lineInfo)
			{
				if (valueOffset == 0)
				{
					return;
				}
				if (this.valueStartPos != -1)
				{
					XmlTextReaderImpl.AdjustLineInfo(this.chars, this.valueStartPos, this.valueStartPos + valueOffset, isNormalized, ref lineInfo);
					return;
				}
				XmlTextReaderImpl.AdjustLineInfo(this.value, 0, valueOffset, isNormalized, ref lineInfo);
			}

			internal string GetNameWPrefix(XmlNameTable nt)
			{
				if (this.nameWPrefix != null)
				{
					return this.nameWPrefix;
				}
				return this.CreateNameWPrefix(nt);
			}

			internal string CreateNameWPrefix(XmlNameTable nt)
			{
				if (this.prefix.Length == 0)
				{
					this.nameWPrefix = this.localName;
				}
				else
				{
					this.nameWPrefix = nt.Add(this.prefix + ":" + this.localName);
				}
				return this.nameWPrefix;
			}

			int IComparable.CompareTo(object obj)
			{
				XmlTextReaderImpl.NodeData nodeData = obj as XmlTextReaderImpl.NodeData;
				if (nodeData == null)
				{
					return 1;
				}
				if (!Ref.Equal(this.localName, nodeData.localName))
				{
					return string.CompareOrdinal(this.localName, nodeData.localName);
				}
				if (Ref.Equal(this.ns, nodeData.ns))
				{
					return 0;
				}
				return string.CompareOrdinal(this.ns, nodeData.ns);
			}

			private static volatile XmlTextReaderImpl.NodeData s_None;

			internal XmlNodeType type;

			internal string localName;

			internal string prefix;

			internal string ns;

			internal string nameWPrefix;

			private string value;

			private char[] chars;

			private int valueStartPos;

			private int valueLength;

			internal LineInfo lineInfo;

			internal LineInfo lineInfo2;

			internal char quoteChar;

			internal int depth;

			private bool isEmptyOrDefault;

			internal int entityId;

			internal bool xmlContextPushed;

			internal XmlTextReaderImpl.NodeData nextAttrValueChunk;

			internal object schemaType;

			internal object typedValue;
		}

		private class DtdDefaultAttributeInfoToNodeDataComparer : IComparer<object>
		{
			internal static IComparer<object> Instance
			{
				get
				{
					return XmlTextReaderImpl.DtdDefaultAttributeInfoToNodeDataComparer.s_instance;
				}
			}

			public int Compare(object x, object y)
			{
				if (x == null)
				{
					if (y != null)
					{
						return -1;
					}
					return 0;
				}
				else
				{
					if (y == null)
					{
						return 1;
					}
					XmlTextReaderImpl.NodeData nodeData = x as XmlTextReaderImpl.NodeData;
					string localName;
					string prefix;
					if (nodeData != null)
					{
						localName = nodeData.localName;
						prefix = nodeData.prefix;
					}
					else
					{
						IDtdDefaultAttributeInfo dtdDefaultAttributeInfo = x as IDtdDefaultAttributeInfo;
						if (dtdDefaultAttributeInfo == null)
						{
							throw new XmlException("An XML error has occurred.", string.Empty);
						}
						localName = dtdDefaultAttributeInfo.LocalName;
						prefix = dtdDefaultAttributeInfo.Prefix;
					}
					nodeData = (y as XmlTextReaderImpl.NodeData);
					string localName2;
					string prefix2;
					if (nodeData != null)
					{
						localName2 = nodeData.localName;
						prefix2 = nodeData.prefix;
					}
					else
					{
						IDtdDefaultAttributeInfo dtdDefaultAttributeInfo2 = y as IDtdDefaultAttributeInfo;
						if (dtdDefaultAttributeInfo2 == null)
						{
							throw new XmlException("An XML error has occurred.", string.Empty);
						}
						localName2 = dtdDefaultAttributeInfo2.LocalName;
						prefix2 = dtdDefaultAttributeInfo2.Prefix;
					}
					int num = string.Compare(localName, localName2, StringComparison.Ordinal);
					if (num != 0)
					{
						return num;
					}
					return string.Compare(prefix, prefix2, StringComparison.Ordinal);
				}
			}

			private static IComparer<object> s_instance = new XmlTextReaderImpl.DtdDefaultAttributeInfoToNodeDataComparer();
		}

		internal delegate void OnDefaultAttributeUseDelegate(IDtdDefaultAttributeInfo defaultAttribute, XmlTextReaderImpl coreReader);
	}
}
