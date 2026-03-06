using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace System.Xml
{
	internal class DtdParser : IDtdParser
	{
		private DtdParser()
		{
		}

		internal static IDtdParser Create()
		{
			return new DtdParser();
		}

		private void Initialize(IDtdParserAdapter readerAdapter)
		{
			this.readerAdapter = readerAdapter;
			this.readerAdapterWithValidation = (readerAdapter as IDtdParserAdapterWithValidation);
			this.nameTable = readerAdapter.NameTable;
			IDtdParserAdapterWithValidation dtdParserAdapterWithValidation = readerAdapter as IDtdParserAdapterWithValidation;
			if (dtdParserAdapterWithValidation != null)
			{
				this.validate = dtdParserAdapterWithValidation.DtdValidation;
			}
			IDtdParserAdapterV1 dtdParserAdapterV = readerAdapter as IDtdParserAdapterV1;
			if (dtdParserAdapterV != null)
			{
				this.v1Compat = dtdParserAdapterV.V1CompatibilityMode;
				this.normalize = dtdParserAdapterV.Normalization;
				this.supportNamespaces = dtdParserAdapterV.Namespaces;
			}
			this.schemaInfo = new SchemaInfo();
			this.schemaInfo.SchemaType = SchemaType.DTD;
			this.stringBuilder = new StringBuilder();
			Uri baseUri = readerAdapter.BaseUri;
			if (baseUri != null)
			{
				this.documentBaseUri = baseUri.ToString();
			}
			this.freeFloatingDtd = false;
		}

		private void InitializeFreeFloatingDtd(string baseUri, string docTypeName, string publicId, string systemId, string internalSubset, IDtdParserAdapter adapter)
		{
			this.Initialize(adapter);
			if (docTypeName == null || docTypeName.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(docTypeName, "docTypeName");
			}
			XmlConvert.VerifyName(docTypeName);
			int num = docTypeName.IndexOf(':');
			if (num == -1)
			{
				this.schemaInfo.DocTypeName = new XmlQualifiedName(this.nameTable.Add(docTypeName));
			}
			else
			{
				this.schemaInfo.DocTypeName = new XmlQualifiedName(this.nameTable.Add(docTypeName.Substring(0, num)), this.nameTable.Add(docTypeName.Substring(num + 1)));
			}
			if (systemId != null && systemId.Length > 0)
			{
				int invCharPos;
				if ((invCharPos = this.xmlCharType.IsOnlyCharData(systemId)) >= 0)
				{
					this.ThrowInvalidChar(this.curPos, systemId, invCharPos);
				}
				this.systemId = systemId;
			}
			if (publicId != null && publicId.Length > 0)
			{
				int invCharPos;
				if ((invCharPos = this.xmlCharType.IsPublicId(publicId)) >= 0)
				{
					this.ThrowInvalidChar(this.curPos, publicId, invCharPos);
				}
				this.publicId = publicId;
			}
			if (internalSubset != null && internalSubset.Length > 0)
			{
				this.readerAdapter.PushInternalDtd(baseUri, internalSubset);
				this.hasFreeFloatingInternalSubset = true;
			}
			Uri baseUri2 = this.readerAdapter.BaseUri;
			if (baseUri2 != null)
			{
				this.documentBaseUri = baseUri2.ToString();
			}
			this.freeFloatingDtd = true;
		}

		IDtdInfo IDtdParser.ParseInternalDtd(IDtdParserAdapter adapter, bool saveInternalSubset)
		{
			this.Initialize(adapter);
			this.Parse(saveInternalSubset);
			return this.schemaInfo;
		}

		IDtdInfo IDtdParser.ParseFreeFloatingDtd(string baseUri, string docTypeName, string publicId, string systemId, string internalSubset, IDtdParserAdapter adapter)
		{
			this.InitializeFreeFloatingDtd(baseUri, docTypeName, publicId, systemId, internalSubset, adapter);
			this.Parse(false);
			return this.schemaInfo;
		}

		private bool ParsingInternalSubset
		{
			get
			{
				return this.externalEntitiesDepth == 0;
			}
		}

		private bool IgnoreEntityReferences
		{
			get
			{
				return this.scanningFunction == DtdParser.ScanningFunction.CondSection3;
			}
		}

		private bool SaveInternalSubsetValue
		{
			get
			{
				return this.readerAdapter.EntityStackLength == 0 && this.internalSubsetValueSb != null;
			}
		}

		private bool ParsingTopLevelMarkup
		{
			get
			{
				return this.scanningFunction == DtdParser.ScanningFunction.SubsetContent || (this.scanningFunction == DtdParser.ScanningFunction.ParamEntitySpace && this.savedScanningFunction == DtdParser.ScanningFunction.SubsetContent);
			}
		}

		private bool SupportNamespaces
		{
			get
			{
				return this.supportNamespaces;
			}
		}

		private bool Normalize
		{
			get
			{
				return this.normalize;
			}
		}

		private void Parse(bool saveInternalSubset)
		{
			if (this.freeFloatingDtd)
			{
				this.ParseFreeFloatingDtd();
			}
			else
			{
				this.ParseInDocumentDtd(saveInternalSubset);
			}
			this.schemaInfo.Finish();
			if (this.validate && this.undeclaredNotations != null)
			{
				foreach (DtdParser.UndeclaredNotation undeclaredNotation in this.undeclaredNotations.Values)
				{
					for (DtdParser.UndeclaredNotation undeclaredNotation2 = undeclaredNotation; undeclaredNotation2 != null; undeclaredNotation2 = undeclaredNotation2.next)
					{
						this.SendValidationEvent(XmlSeverityType.Error, new XmlSchemaException("The '{0}' notation is not declared.", undeclaredNotation.name, this.BaseUriStr, undeclaredNotation.lineNo, undeclaredNotation.linePos));
					}
				}
			}
		}

		private void ParseInDocumentDtd(bool saveInternalSubset)
		{
			this.LoadParsingBuffer();
			this.scanningFunction = DtdParser.ScanningFunction.QName;
			this.nextScaningFunction = DtdParser.ScanningFunction.Doctype1;
			if (this.GetToken(false) != DtdParser.Token.QName)
			{
				this.OnUnexpectedError();
			}
			this.schemaInfo.DocTypeName = this.GetNameQualified(true);
			DtdParser.Token token = this.GetToken(false);
			if (token == DtdParser.Token.SYSTEM || token == DtdParser.Token.PUBLIC)
			{
				this.ParseExternalId(token, DtdParser.Token.DOCTYPE, out this.publicId, out this.systemId);
				token = this.GetToken(false);
			}
			if (token != DtdParser.Token.GreaterThan)
			{
				if (token == DtdParser.Token.LeftBracket)
				{
					if (saveInternalSubset)
					{
						this.SaveParsingBuffer();
						this.internalSubsetValueSb = new StringBuilder();
					}
					this.ParseInternalSubset();
				}
				else
				{
					this.OnUnexpectedError();
				}
			}
			this.SaveParsingBuffer();
			if (this.systemId != null && this.systemId.Length > 0)
			{
				this.ParseExternalSubset();
			}
		}

		private void ParseFreeFloatingDtd()
		{
			if (this.hasFreeFloatingInternalSubset)
			{
				this.LoadParsingBuffer();
				this.ParseInternalSubset();
				this.SaveParsingBuffer();
			}
			if (this.systemId != null && this.systemId.Length > 0)
			{
				this.ParseExternalSubset();
			}
		}

		private void ParseInternalSubset()
		{
			this.ParseSubset();
		}

		private void ParseExternalSubset()
		{
			if (!this.readerAdapter.PushExternalSubset(this.systemId, this.publicId))
			{
				return;
			}
			Uri baseUri = this.readerAdapter.BaseUri;
			if (baseUri != null)
			{
				this.externalDtdBaseUri = baseUri.ToString();
			}
			this.externalEntitiesDepth++;
			this.LoadParsingBuffer();
			this.ParseSubset();
		}

		private void ParseSubset()
		{
			for (;;)
			{
				DtdParser.Token token = this.GetToken(false);
				int num = this.currentEntityId;
				switch (token)
				{
				case DtdParser.Token.AttlistDecl:
					this.ParseAttlistDecl();
					break;
				case DtdParser.Token.ElementDecl:
					this.ParseElementDecl();
					break;
				case DtdParser.Token.EntityDecl:
					this.ParseEntityDecl();
					break;
				case DtdParser.Token.NotationDecl:
					this.ParseNotationDecl();
					break;
				case DtdParser.Token.Comment:
					this.ParseComment();
					break;
				case DtdParser.Token.PI:
					this.ParsePI();
					break;
				case DtdParser.Token.CondSectionStart:
					if (this.ParsingInternalSubset)
					{
						this.Throw(this.curPos - 3, "A conditional section is not allowed in an internal subset.");
					}
					this.ParseCondSection();
					num = this.currentEntityId;
					break;
				case DtdParser.Token.CondSectionEnd:
					if (this.condSectionDepth > 0)
					{
						this.condSectionDepth--;
						if (this.validate && this.currentEntityId != this.condSectionEntityIds[this.condSectionDepth])
						{
							this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
						}
					}
					else
					{
						this.Throw(this.curPos - 3, "']]>' is not expected.");
					}
					break;
				case DtdParser.Token.Eof:
					goto IL_1A9;
				default:
					if (token == DtdParser.Token.RightBracket)
					{
						goto IL_126;
					}
					break;
				}
				if (this.currentEntityId != num)
				{
					if (this.validate)
					{
						this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
					}
					else if (!this.v1Compat)
					{
						this.Throw(this.curPos, "The parameter entity replacement text must nest properly within markup declarations.");
					}
				}
			}
			IL_126:
			if (this.ParsingInternalSubset)
			{
				if (this.condSectionDepth != 0)
				{
					this.Throw(this.curPos, "There is an unclosed conditional section.");
				}
				if (this.internalSubsetValueSb != null)
				{
					this.SaveParsingBuffer(this.curPos - 1);
					this.schemaInfo.InternalDtdSubset = this.internalSubsetValueSb.ToString();
					this.internalSubsetValueSb = null;
				}
				if (this.GetToken(false) != DtdParser.Token.GreaterThan)
				{
					this.ThrowUnexpectedToken(this.curPos, ">");
					return;
				}
			}
			else
			{
				this.Throw(this.curPos, "Expected DTD markup was not found.");
			}
			return;
			IL_1A9:
			if (this.ParsingInternalSubset && !this.freeFloatingDtd)
			{
				this.Throw(this.curPos, "Incomplete DTD content.");
			}
			if (this.condSectionDepth != 0)
			{
				this.Throw(this.curPos, "There is an unclosed conditional section.");
			}
		}

		private void ParseAttlistDecl()
		{
			if (this.GetToken(true) == DtdParser.Token.QName)
			{
				XmlQualifiedName nameQualified = this.GetNameQualified(true);
				SchemaElementDecl schemaElementDecl;
				if (!this.schemaInfo.ElementDecls.TryGetValue(nameQualified, out schemaElementDecl) && !this.schemaInfo.UndeclaredElementDecls.TryGetValue(nameQualified, out schemaElementDecl))
				{
					schemaElementDecl = new SchemaElementDecl(nameQualified, nameQualified.Namespace);
					this.schemaInfo.UndeclaredElementDecls.Add(nameQualified, schemaElementDecl);
				}
				SchemaAttDef schemaAttDef = null;
				DtdParser.Token token;
				for (;;)
				{
					token = this.GetToken(false);
					if (token != DtdParser.Token.QName)
					{
						break;
					}
					XmlQualifiedName nameQualified2 = this.GetNameQualified(true);
					schemaAttDef = new SchemaAttDef(nameQualified2, nameQualified2.Namespace);
					schemaAttDef.IsDeclaredInExternal = !this.ParsingInternalSubset;
					schemaAttDef.LineNumber = this.LineNo;
					schemaAttDef.LinePosition = this.LinePos - (this.curPos - this.tokenStartPos);
					bool flag = schemaElementDecl.GetAttDef(schemaAttDef.Name) != null;
					this.ParseAttlistType(schemaAttDef, schemaElementDecl, flag);
					this.ParseAttlistDefault(schemaAttDef, flag);
					if (schemaAttDef.Prefix.Length > 0 && schemaAttDef.Prefix.Equals("xml"))
					{
						if (schemaAttDef.Name.Name == "space")
						{
							if (this.v1Compat)
							{
								string text = schemaAttDef.DefaultValueExpanded.Trim();
								if (text.Equals("preserve") || text.Equals("default"))
								{
									schemaAttDef.Reserved = SchemaAttDef.Reserve.XmlSpace;
								}
							}
							else
							{
								schemaAttDef.Reserved = SchemaAttDef.Reserve.XmlSpace;
								if (schemaAttDef.TokenizedType != XmlTokenizedType.ENUMERATION)
								{
									this.Throw("Enumeration data type required.", string.Empty, schemaAttDef.LineNumber, schemaAttDef.LinePosition);
								}
								if (this.validate)
								{
									schemaAttDef.CheckXmlSpace(this.readerAdapterWithValidation.ValidationEventHandling);
								}
							}
						}
						else if (schemaAttDef.Name.Name == "lang")
						{
							schemaAttDef.Reserved = SchemaAttDef.Reserve.XmlLang;
						}
					}
					if (!flag)
					{
						schemaElementDecl.AddAttDef(schemaAttDef);
					}
				}
				if (token == DtdParser.Token.GreaterThan)
				{
					if (this.v1Compat && schemaAttDef != null && schemaAttDef.Prefix.Length > 0 && schemaAttDef.Prefix.Equals("xml") && schemaAttDef.Name.Name == "space")
					{
						schemaAttDef.Reserved = SchemaAttDef.Reserve.XmlSpace;
						if (schemaAttDef.Datatype.TokenizedType != XmlTokenizedType.ENUMERATION)
						{
							this.Throw("Enumeration data type required.", string.Empty, schemaAttDef.LineNumber, schemaAttDef.LinePosition);
						}
						if (this.validate)
						{
							schemaAttDef.CheckXmlSpace(this.readerAdapterWithValidation.ValidationEventHandling);
						}
					}
					return;
				}
			}
			this.OnUnexpectedError();
		}

		private void ParseAttlistType(SchemaAttDef attrDef, SchemaElementDecl elementDecl, bool ignoreErrors)
		{
			DtdParser.Token token = this.GetToken(true);
			if (token != DtdParser.Token.CDATA)
			{
				elementDecl.HasNonCDataAttribute = true;
			}
			if (this.IsAttributeValueType(token))
			{
				attrDef.TokenizedType = (XmlTokenizedType)token;
				attrDef.SchemaType = XmlSchemaType.GetBuiltInSimpleType(attrDef.Datatype.TypeCode);
				if (token == DtdParser.Token.ID)
				{
					if (this.validate && elementDecl.IsIdDeclared)
					{
						SchemaAttDef attDef = elementDecl.GetAttDef(attrDef.Name);
						if ((attDef == null || attDef.Datatype.TokenizedType != XmlTokenizedType.ID) && !ignoreErrors)
						{
							this.SendValidationEvent(XmlSeverityType.Error, "The attribute of type ID is already declared on the '{0}' element.", elementDecl.Name.ToString());
						}
					}
					elementDecl.IsIdDeclared = true;
					return;
				}
				if (token != DtdParser.Token.NOTATION)
				{
					return;
				}
				if (this.validate)
				{
					if (elementDecl.IsNotationDeclared && !ignoreErrors)
					{
						this.SendValidationEvent(this.curPos - 8, XmlSeverityType.Error, "No element type can have more than one NOTATION attribute specified.", elementDecl.Name.ToString());
					}
					else
					{
						if (elementDecl.ContentValidator != null && elementDecl.ContentValidator.ContentType == XmlSchemaContentType.Empty && !ignoreErrors)
						{
							this.SendValidationEvent(this.curPos - 8, XmlSeverityType.Error, "An attribute of type NOTATION must not be declared on an element declared EMPTY.", elementDecl.Name.ToString());
						}
						elementDecl.IsNotationDeclared = true;
					}
				}
				if (this.GetToken(true) == DtdParser.Token.LeftParen && this.GetToken(false) == DtdParser.Token.Name)
				{
					do
					{
						string nameString = this.GetNameString();
						if (!this.schemaInfo.Notations.ContainsKey(nameString))
						{
							this.AddUndeclaredNotation(nameString);
						}
						if (this.validate && !this.v1Compat && attrDef.Values != null && attrDef.Values.Contains(nameString) && !ignoreErrors)
						{
							this.SendValidationEvent(XmlSeverityType.Error, new XmlSchemaException("'{0}' is a duplicate notation value.", nameString, this.BaseUriStr, this.LineNo, this.LinePos));
						}
						attrDef.AddValue(nameString);
						DtdParser.Token token2 = this.GetToken(false);
						if (token2 == DtdParser.Token.RightParen)
						{
							return;
						}
						if (token2 != DtdParser.Token.Or)
						{
							break;
						}
					}
					while (this.GetToken(false) == DtdParser.Token.Name);
				}
			}
			else if (token == DtdParser.Token.LeftParen)
			{
				attrDef.TokenizedType = XmlTokenizedType.ENUMERATION;
				attrDef.SchemaType = XmlSchemaType.GetBuiltInSimpleType(attrDef.Datatype.TypeCode);
				if (this.GetToken(false) == DtdParser.Token.Nmtoken)
				{
					attrDef.AddValue(this.GetNameString());
					for (;;)
					{
						DtdParser.Token token2 = this.GetToken(false);
						if (token2 == DtdParser.Token.RightParen)
						{
							break;
						}
						if (token2 != DtdParser.Token.Or || this.GetToken(false) != DtdParser.Token.Nmtoken)
						{
							goto IL_280;
						}
						string nmtokenString = this.GetNmtokenString();
						if (this.validate && !this.v1Compat && attrDef.Values != null && attrDef.Values.Contains(nmtokenString) && !ignoreErrors)
						{
							this.SendValidationEvent(XmlSeverityType.Error, new XmlSchemaException("'{0}' is a duplicate enumeration value.", nmtokenString, this.BaseUriStr, this.LineNo, this.LinePos));
						}
						attrDef.AddValue(nmtokenString);
					}
					return;
				}
			}
			IL_280:
			this.OnUnexpectedError();
		}

		private void ParseAttlistDefault(SchemaAttDef attrDef, bool ignoreErrors)
		{
			DtdParser.Token token = this.GetToken(true);
			switch (token)
			{
			case DtdParser.Token.REQUIRED:
				attrDef.Presence = SchemaDeclBase.Use.Required;
				return;
			case DtdParser.Token.IMPLIED:
				attrDef.Presence = SchemaDeclBase.Use.Implied;
				return;
			case DtdParser.Token.FIXED:
				attrDef.Presence = SchemaDeclBase.Use.Fixed;
				if (this.GetToken(true) != DtdParser.Token.Literal)
				{
					goto IL_CF;
				}
				break;
			default:
				if (token != DtdParser.Token.Literal)
				{
					goto IL_CF;
				}
				break;
			}
			if (this.validate && attrDef.Datatype.TokenizedType == XmlTokenizedType.ID && !ignoreErrors)
			{
				this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "An attribute of type ID must have a declared default of either #IMPLIED or #REQUIRED.", string.Empty);
			}
			if (attrDef.TokenizedType != XmlTokenizedType.CDATA)
			{
				attrDef.DefaultValueExpanded = this.GetValueWithStrippedSpaces();
			}
			else
			{
				attrDef.DefaultValueExpanded = this.GetValue();
			}
			attrDef.ValueLineNumber = this.literalLineInfo.lineNo;
			attrDef.ValueLinePosition = this.literalLineInfo.linePos + 1;
			DtdValidator.SetDefaultTypedValue(attrDef, this.readerAdapter);
			return;
			IL_CF:
			this.OnUnexpectedError();
		}

		private void ParseElementDecl()
		{
			if (this.GetToken(true) == DtdParser.Token.QName)
			{
				SchemaElementDecl schemaElementDecl = null;
				XmlQualifiedName nameQualified = this.GetNameQualified(true);
				if (this.schemaInfo.ElementDecls.TryGetValue(nameQualified, out schemaElementDecl))
				{
					if (this.validate)
					{
						this.SendValidationEvent(this.curPos - nameQualified.Name.Length, XmlSeverityType.Error, "The '{0}' element has already been declared.", this.GetNameString());
					}
				}
				else
				{
					if (this.schemaInfo.UndeclaredElementDecls.TryGetValue(nameQualified, out schemaElementDecl))
					{
						this.schemaInfo.UndeclaredElementDecls.Remove(nameQualified);
					}
					else
					{
						schemaElementDecl = new SchemaElementDecl(nameQualified, nameQualified.Namespace);
					}
					this.schemaInfo.ElementDecls.Add(nameQualified, schemaElementDecl);
				}
				schemaElementDecl.IsDeclaredInExternal = !this.ParsingInternalSubset;
				DtdParser.Token token = this.GetToken(true);
				if (token != DtdParser.Token.LeftParen)
				{
					if (token != DtdParser.Token.ANY)
					{
						if (token != DtdParser.Token.EMPTY)
						{
							goto IL_181;
						}
						schemaElementDecl.ContentValidator = ContentValidator.Empty;
					}
					else
					{
						schemaElementDecl.ContentValidator = ContentValidator.Any;
					}
				}
				else
				{
					int startParenEntityId = this.currentEntityId;
					DtdParser.Token token2 = this.GetToken(false);
					if (token2 != DtdParser.Token.None)
					{
						if (token2 != DtdParser.Token.PCDATA)
						{
							goto IL_181;
						}
						ParticleContentValidator particleContentValidator = new ParticleContentValidator(XmlSchemaContentType.Mixed);
						particleContentValidator.Start();
						particleContentValidator.OpenGroup();
						this.ParseElementMixedContent(particleContentValidator, startParenEntityId);
						schemaElementDecl.ContentValidator = particleContentValidator.Finish(true);
					}
					else
					{
						ParticleContentValidator particleContentValidator2 = new ParticleContentValidator(XmlSchemaContentType.ElementOnly);
						particleContentValidator2.Start();
						particleContentValidator2.OpenGroup();
						this.ParseElementOnlyContent(particleContentValidator2, startParenEntityId);
						schemaElementDecl.ContentValidator = particleContentValidator2.Finish(true);
					}
				}
				if (this.GetToken(false) != DtdParser.Token.GreaterThan)
				{
					this.ThrowUnexpectedToken(this.curPos, ">");
				}
				return;
			}
			IL_181:
			this.OnUnexpectedError();
		}

		private void ParseElementOnlyContent(ParticleContentValidator pcv, int startParenEntityId)
		{
			Stack<DtdParser.ParseElementOnlyContent_LocalFrame> stack = new Stack<DtdParser.ParseElementOnlyContent_LocalFrame>();
			DtdParser.ParseElementOnlyContent_LocalFrame parseElementOnlyContent_LocalFrame = new DtdParser.ParseElementOnlyContent_LocalFrame(startParenEntityId);
			stack.Push(parseElementOnlyContent_LocalFrame);
			for (;;)
			{
				DtdParser.Token token = this.GetToken(false);
				if (token != DtdParser.Token.QName)
				{
					if (token == DtdParser.Token.LeftParen)
					{
						pcv.OpenGroup();
						parseElementOnlyContent_LocalFrame = new DtdParser.ParseElementOnlyContent_LocalFrame(this.currentEntityId);
						stack.Push(parseElementOnlyContent_LocalFrame);
						continue;
					}
					if (token != DtdParser.Token.GreaterThan)
					{
						goto IL_148;
					}
					this.Throw(this.curPos, "Invalid content model.");
					goto IL_14E;
				}
				else
				{
					pcv.AddName(this.GetNameQualified(true), null);
					this.ParseHowMany(pcv);
				}
				IL_78:
				token = this.GetToken(false);
				switch (token)
				{
				case DtdParser.Token.RightParen:
					pcv.CloseGroup();
					if (this.validate && this.currentEntityId != parseElementOnlyContent_LocalFrame.startParenEntityId)
					{
						this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
					}
					this.ParseHowMany(pcv);
					break;
				case DtdParser.Token.GreaterThan:
					this.Throw(this.curPos, "Invalid content model.");
					break;
				case DtdParser.Token.Or:
					if (parseElementOnlyContent_LocalFrame.parsingSchema == DtdParser.Token.Comma)
					{
						this.Throw(this.curPos, "Invalid content model.");
					}
					pcv.AddChoice();
					parseElementOnlyContent_LocalFrame.parsingSchema = DtdParser.Token.Or;
					continue;
				default:
					if (token == DtdParser.Token.Comma)
					{
						if (parseElementOnlyContent_LocalFrame.parsingSchema == DtdParser.Token.Or)
						{
							this.Throw(this.curPos, "Invalid content model.");
						}
						pcv.AddSequence();
						parseElementOnlyContent_LocalFrame.parsingSchema = DtdParser.Token.Comma;
						continue;
					}
					goto IL_148;
				}
				IL_14E:
				stack.Pop();
				if (stack.Count > 0)
				{
					parseElementOnlyContent_LocalFrame = stack.Peek();
					goto IL_78;
				}
				break;
				IL_148:
				this.OnUnexpectedError();
				goto IL_14E;
			}
		}

		private void ParseHowMany(ParticleContentValidator pcv)
		{
			switch (this.GetToken(false))
			{
			case DtdParser.Token.Star:
				pcv.AddStar();
				return;
			case DtdParser.Token.QMark:
				pcv.AddQMark();
				return;
			case DtdParser.Token.Plus:
				pcv.AddPlus();
				return;
			default:
				return;
			}
		}

		private void ParseElementMixedContent(ParticleContentValidator pcv, int startParenEntityId)
		{
			bool flag = false;
			int num = -1;
			int num2 = this.currentEntityId;
			for (;;)
			{
				DtdParser.Token token = this.GetToken(false);
				if (token == DtdParser.Token.RightParen)
				{
					break;
				}
				if (token == DtdParser.Token.Or)
				{
					if (!flag)
					{
						flag = true;
					}
					else
					{
						pcv.AddChoice();
					}
					if (this.validate)
					{
						num = this.currentEntityId;
						if (num2 < num)
						{
							this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
						}
					}
					if (this.GetToken(false) == DtdParser.Token.QName)
					{
						XmlQualifiedName nameQualified = this.GetNameQualified(true);
						if (pcv.Exists(nameQualified) && this.validate)
						{
							this.SendValidationEvent(XmlSeverityType.Error, "The '{0}' element already exists in the content model.", nameQualified.ToString());
						}
						pcv.AddName(nameQualified, null);
						if (!this.validate)
						{
							continue;
						}
						num2 = this.currentEntityId;
						if (num2 < num)
						{
							this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
							continue;
						}
						continue;
					}
				}
				this.OnUnexpectedError();
			}
			pcv.CloseGroup();
			if (this.validate && this.currentEntityId != startParenEntityId)
			{
				this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
			}
			if (this.GetToken(false) == DtdParser.Token.Star && flag)
			{
				pcv.AddStar();
				return;
			}
			if (flag)
			{
				this.ThrowUnexpectedToken(this.curPos, "*");
			}
		}

		private void ParseEntityDecl()
		{
			bool flag = false;
			DtdParser.Token token = this.GetToken(true);
			if (token != DtdParser.Token.Name)
			{
				if (token != DtdParser.Token.Percent)
				{
					goto IL_1D6;
				}
				flag = true;
				if (this.GetToken(true) != DtdParser.Token.Name)
				{
					goto IL_1D6;
				}
			}
			XmlQualifiedName nameQualified = this.GetNameQualified(false);
			SchemaEntity schemaEntity = new SchemaEntity(nameQualified, flag);
			schemaEntity.BaseURI = this.BaseUriStr;
			schemaEntity.DeclaredURI = ((this.externalDtdBaseUri.Length == 0) ? this.documentBaseUri : this.externalDtdBaseUri);
			if (flag)
			{
				if (!this.schemaInfo.ParameterEntities.ContainsKey(nameQualified))
				{
					this.schemaInfo.ParameterEntities.Add(nameQualified, schemaEntity);
				}
			}
			else if (!this.schemaInfo.GeneralEntities.ContainsKey(nameQualified))
			{
				this.schemaInfo.GeneralEntities.Add(nameQualified, schemaEntity);
			}
			schemaEntity.DeclaredInExternal = !this.ParsingInternalSubset;
			schemaEntity.ParsingInProgress = true;
			DtdParser.Token token2 = this.GetToken(true);
			if (token2 - DtdParser.Token.PUBLIC > 1)
			{
				if (token2 != DtdParser.Token.Literal)
				{
					goto IL_1D6;
				}
				schemaEntity.Text = this.GetValue();
				schemaEntity.Line = this.literalLineInfo.lineNo;
				schemaEntity.Pos = this.literalLineInfo.linePos;
			}
			else
			{
				string pubid;
				string url;
				this.ParseExternalId(token2, DtdParser.Token.EntityDecl, out pubid, out url);
				schemaEntity.IsExternal = true;
				schemaEntity.Url = url;
				schemaEntity.Pubid = pubid;
				if (this.GetToken(false) == DtdParser.Token.NData)
				{
					if (flag)
					{
						this.ThrowUnexpectedToken(this.curPos - 5, ">");
					}
					if (!this.whitespaceSeen)
					{
						this.Throw(this.curPos - 5, "'{0}' is an unexpected token. Expecting white space.", "NDATA");
					}
					if (this.GetToken(true) != DtdParser.Token.Name)
					{
						goto IL_1D6;
					}
					schemaEntity.NData = this.GetNameQualified(false);
					string name = schemaEntity.NData.Name;
					if (!this.schemaInfo.Notations.ContainsKey(name))
					{
						this.AddUndeclaredNotation(name);
					}
				}
			}
			if (this.GetToken(false) == DtdParser.Token.GreaterThan)
			{
				schemaEntity.ParsingInProgress = false;
				return;
			}
			IL_1D6:
			this.OnUnexpectedError();
		}

		private void ParseNotationDecl()
		{
			if (this.GetToken(true) != DtdParser.Token.Name)
			{
				this.OnUnexpectedError();
			}
			XmlQualifiedName nameQualified = this.GetNameQualified(false);
			SchemaNotation schemaNotation = null;
			if (!this.schemaInfo.Notations.ContainsKey(nameQualified.Name))
			{
				if (this.undeclaredNotations != null)
				{
					this.undeclaredNotations.Remove(nameQualified.Name);
				}
				schemaNotation = new SchemaNotation(nameQualified);
				this.schemaInfo.Notations.Add(schemaNotation.Name.Name, schemaNotation);
			}
			else if (this.validate)
			{
				this.SendValidationEvent(this.curPos - nameQualified.Name.Length, XmlSeverityType.Error, "The notation '{0}' has already been declared.", nameQualified.Name);
			}
			DtdParser.Token token = this.GetToken(true);
			if (token == DtdParser.Token.SYSTEM || token == DtdParser.Token.PUBLIC)
			{
				string pubid;
				string systemLiteral;
				this.ParseExternalId(token, DtdParser.Token.NOTATION, out pubid, out systemLiteral);
				if (schemaNotation != null)
				{
					schemaNotation.SystemLiteral = systemLiteral;
					schemaNotation.Pubid = pubid;
				}
			}
			else
			{
				this.OnUnexpectedError();
			}
			if (this.GetToken(false) != DtdParser.Token.GreaterThan)
			{
				this.OnUnexpectedError();
			}
		}

		private void AddUndeclaredNotation(string notationName)
		{
			if (this.undeclaredNotations == null)
			{
				this.undeclaredNotations = new Dictionary<string, DtdParser.UndeclaredNotation>();
			}
			DtdParser.UndeclaredNotation undeclaredNotation = new DtdParser.UndeclaredNotation(notationName, this.LineNo, this.LinePos - notationName.Length);
			DtdParser.UndeclaredNotation undeclaredNotation2;
			if (this.undeclaredNotations.TryGetValue(notationName, out undeclaredNotation2))
			{
				undeclaredNotation.next = undeclaredNotation2.next;
				undeclaredNotation2.next = undeclaredNotation;
				return;
			}
			this.undeclaredNotations.Add(notationName, undeclaredNotation);
		}

		private void ParseComment()
		{
			this.SaveParsingBuffer();
			try
			{
				if (this.SaveInternalSubsetValue)
				{
					this.readerAdapter.ParseComment(this.internalSubsetValueSb);
					this.internalSubsetValueSb.Append("-->");
				}
				else
				{
					this.readerAdapter.ParseComment(null);
				}
			}
			catch (XmlException ex)
			{
				if (!(ex.ResString == "Unexpected end of file while parsing {0} has occurred.") || this.currentEntityId == 0)
				{
					throw;
				}
				this.SendValidationEvent(XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", null);
			}
			this.LoadParsingBuffer();
		}

		private void ParsePI()
		{
			this.SaveParsingBuffer();
			if (this.SaveInternalSubsetValue)
			{
				this.readerAdapter.ParsePI(this.internalSubsetValueSb);
				this.internalSubsetValueSb.Append("?>");
			}
			else
			{
				this.readerAdapter.ParsePI(null);
			}
			this.LoadParsingBuffer();
		}

		private void ParseCondSection()
		{
			int num = this.currentEntityId;
			DtdParser.Token token = this.GetToken(false);
			if (token != DtdParser.Token.IGNORE)
			{
				if (token == DtdParser.Token.INCLUDE && this.GetToken(false) == DtdParser.Token.LeftBracket)
				{
					if (this.validate && num != this.currentEntityId)
					{
						this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
					}
					if (this.validate)
					{
						if (this.condSectionEntityIds == null)
						{
							this.condSectionEntityIds = new int[2];
						}
						else if (this.condSectionEntityIds.Length == this.condSectionDepth)
						{
							int[] destinationArray = new int[this.condSectionEntityIds.Length * 2];
							Array.Copy(this.condSectionEntityIds, 0, destinationArray, 0, this.condSectionEntityIds.Length);
							this.condSectionEntityIds = destinationArray;
						}
						this.condSectionEntityIds[this.condSectionDepth] = num;
					}
					this.condSectionDepth++;
					return;
				}
			}
			else if (this.GetToken(false) == DtdParser.Token.LeftBracket)
			{
				if (this.validate && num != this.currentEntityId)
				{
					this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
				}
				if (this.GetToken(false) == DtdParser.Token.CondSectionEnd)
				{
					if (this.validate && num != this.currentEntityId)
					{
						this.SendValidationEvent(this.curPos, XmlSeverityType.Error, "The parameter entity replacement text must nest properly within markup declarations.", string.Empty);
						return;
					}
					return;
				}
			}
			this.OnUnexpectedError();
		}

		private void ParseExternalId(DtdParser.Token idTokenType, DtdParser.Token declType, out string publicId, out string systemId)
		{
			LineInfo keywordLineInfo = new LineInfo(this.LineNo, this.LinePos - 6);
			publicId = null;
			systemId = null;
			if (this.GetToken(true) != DtdParser.Token.Literal)
			{
				this.ThrowUnexpectedToken(this.curPos, "\"", "'");
			}
			if (idTokenType == DtdParser.Token.SYSTEM)
			{
				systemId = this.GetValue();
				if (systemId.IndexOf('#') >= 0)
				{
					this.Throw(this.curPos - systemId.Length - 1, "Fragment identifier '{0}' cannot be part of the system identifier '{1}'.", new string[]
					{
						systemId.Substring(systemId.IndexOf('#')),
						systemId
					});
				}
				if (declType == DtdParser.Token.DOCTYPE && !this.freeFloatingDtd)
				{
					this.literalLineInfo.linePos = this.literalLineInfo.linePos + 1;
					this.readerAdapter.OnSystemId(systemId, keywordLineInfo, this.literalLineInfo);
					return;
				}
			}
			else
			{
				publicId = this.GetValue();
				int num;
				if ((num = this.xmlCharType.IsPublicId(publicId)) >= 0)
				{
					this.ThrowInvalidChar(this.curPos - 1 - publicId.Length + num, publicId, num);
				}
				if (declType == DtdParser.Token.DOCTYPE && !this.freeFloatingDtd)
				{
					this.literalLineInfo.linePos = this.literalLineInfo.linePos + 1;
					this.readerAdapter.OnPublicId(publicId, keywordLineInfo, this.literalLineInfo);
					if (this.GetToken(false) == DtdParser.Token.Literal)
					{
						if (!this.whitespaceSeen)
						{
							this.Throw("'{0}' is an unexpected token. Expecting white space.", new string(this.literalQuoteChar, 1), this.literalLineInfo.lineNo, this.literalLineInfo.linePos);
						}
						systemId = this.GetValue();
						this.literalLineInfo.linePos = this.literalLineInfo.linePos + 1;
						this.readerAdapter.OnSystemId(systemId, keywordLineInfo, this.literalLineInfo);
						return;
					}
					this.ThrowUnexpectedToken(this.curPos, "\"", "'");
					return;
				}
				else
				{
					if (this.GetToken(false) == DtdParser.Token.Literal)
					{
						if (!this.whitespaceSeen)
						{
							this.Throw("'{0}' is an unexpected token. Expecting white space.", new string(this.literalQuoteChar, 1), this.literalLineInfo.lineNo, this.literalLineInfo.linePos);
						}
						systemId = this.GetValue();
						return;
					}
					if (declType != DtdParser.Token.NOTATION)
					{
						this.ThrowUnexpectedToken(this.curPos, "\"", "'");
					}
				}
			}
		}

		private DtdParser.Token GetToken(bool needWhiteSpace)
		{
			this.whitespaceSeen = false;
			for (;;)
			{
				char c = this.chars[this.curPos];
				if (c <= '\r')
				{
					if (c != '\0')
					{
						switch (c)
						{
						case '\t':
							goto IL_14D;
						case '\n':
							this.whitespaceSeen = true;
							this.curPos++;
							this.readerAdapter.OnNewLine(this.curPos);
							continue;
						case '\r':
							this.whitespaceSeen = true;
							if (this.chars[this.curPos + 1] == '\n')
							{
								if (this.Normalize)
								{
									this.SaveParsingBuffer();
									IDtdParserAdapter dtdParserAdapter = this.readerAdapter;
									int currentPosition = dtdParserAdapter.CurrentPosition;
									dtdParserAdapter.CurrentPosition = currentPosition + 1;
								}
								this.curPos += 2;
							}
							else
							{
								if (this.curPos + 1 >= this.charsUsed && !this.readerAdapter.IsEof)
								{
									goto IL_388;
								}
								this.chars[this.curPos] = '\n';
								this.curPos++;
							}
							this.readerAdapter.OnNewLine(this.curPos);
							continue;
						}
						break;
					}
					if (this.curPos != this.charsUsed)
					{
						this.ThrowInvalidChar(this.chars, this.charsUsed, this.curPos);
						goto IL_388;
					}
					goto IL_388;
				}
				else if (c != ' ')
				{
					if (c != '%')
					{
						break;
					}
					if (this.charsUsed - this.curPos < 2)
					{
						goto IL_388;
					}
					if (this.xmlCharType.IsWhiteSpace(this.chars[this.curPos + 1]))
					{
						break;
					}
					if (this.IgnoreEntityReferences)
					{
						this.curPos++;
						continue;
					}
					this.HandleEntityReference(true, false, false);
					continue;
				}
				IL_14D:
				this.whitespaceSeen = true;
				this.curPos++;
				continue;
				IL_388:
				if ((this.readerAdapter.IsEof || this.ReadData() == 0) && !this.HandleEntityEnd(false))
				{
					if (this.scanningFunction == DtdParser.ScanningFunction.SubsetContent)
					{
						return DtdParser.Token.Eof;
					}
					this.Throw(this.curPos, "Incomplete DTD content.");
				}
			}
			if (needWhiteSpace && !this.whitespaceSeen && this.scanningFunction != DtdParser.ScanningFunction.ParamEntitySpace)
			{
				this.Throw(this.curPos, "'{0}' is an unexpected token. Expecting white space.", this.ParseUnexpectedToken(this.curPos));
			}
			this.tokenStartPos = this.curPos;
			for (;;)
			{
				switch (this.scanningFunction)
				{
				case DtdParser.ScanningFunction.SubsetContent:
					goto IL_2A9;
				case DtdParser.ScanningFunction.Name:
					goto IL_294;
				case DtdParser.ScanningFunction.QName:
					goto IL_29B;
				case DtdParser.ScanningFunction.Nmtoken:
					goto IL_2A2;
				case DtdParser.ScanningFunction.Doctype1:
					goto IL_2B0;
				case DtdParser.ScanningFunction.Doctype2:
					goto IL_2B7;
				case DtdParser.ScanningFunction.Element1:
					goto IL_2BE;
				case DtdParser.ScanningFunction.Element2:
					goto IL_2C5;
				case DtdParser.ScanningFunction.Element3:
					goto IL_2CC;
				case DtdParser.ScanningFunction.Element4:
					goto IL_2D3;
				case DtdParser.ScanningFunction.Element5:
					goto IL_2DA;
				case DtdParser.ScanningFunction.Element6:
					goto IL_2E1;
				case DtdParser.ScanningFunction.Element7:
					goto IL_2E8;
				case DtdParser.ScanningFunction.Attlist1:
					goto IL_2EF;
				case DtdParser.ScanningFunction.Attlist2:
					goto IL_2F6;
				case DtdParser.ScanningFunction.Attlist3:
					goto IL_2FD;
				case DtdParser.ScanningFunction.Attlist4:
					goto IL_304;
				case DtdParser.ScanningFunction.Attlist5:
					goto IL_30B;
				case DtdParser.ScanningFunction.Attlist6:
					goto IL_312;
				case DtdParser.ScanningFunction.Attlist7:
					goto IL_319;
				case DtdParser.ScanningFunction.Entity1:
					goto IL_33C;
				case DtdParser.ScanningFunction.Entity2:
					goto IL_343;
				case DtdParser.ScanningFunction.Entity3:
					goto IL_34A;
				case DtdParser.ScanningFunction.Notation1:
					goto IL_320;
				case DtdParser.ScanningFunction.CondSection1:
					goto IL_351;
				case DtdParser.ScanningFunction.CondSection2:
					goto IL_358;
				case DtdParser.ScanningFunction.CondSection3:
					goto IL_35F;
				case DtdParser.ScanningFunction.SystemId:
					goto IL_327;
				case DtdParser.ScanningFunction.PublicId1:
					goto IL_32E;
				case DtdParser.ScanningFunction.PublicId2:
					goto IL_335;
				case DtdParser.ScanningFunction.ClosingTag:
					goto IL_366;
				case DtdParser.ScanningFunction.ParamEntitySpace:
					this.whitespaceSeen = true;
					this.scanningFunction = this.savedScanningFunction;
					continue;
				}
				break;
			}
			return DtdParser.Token.None;
			IL_294:
			return this.ScanNameExpected();
			IL_29B:
			return this.ScanQNameExpected();
			IL_2A2:
			return this.ScanNmtokenExpected();
			IL_2A9:
			return this.ScanSubsetContent();
			IL_2B0:
			return this.ScanDoctype1();
			IL_2B7:
			return this.ScanDoctype2();
			IL_2BE:
			return this.ScanElement1();
			IL_2C5:
			return this.ScanElement2();
			IL_2CC:
			return this.ScanElement3();
			IL_2D3:
			return this.ScanElement4();
			IL_2DA:
			return this.ScanElement5();
			IL_2E1:
			return this.ScanElement6();
			IL_2E8:
			return this.ScanElement7();
			IL_2EF:
			return this.ScanAttlist1();
			IL_2F6:
			return this.ScanAttlist2();
			IL_2FD:
			return this.ScanAttlist3();
			IL_304:
			return this.ScanAttlist4();
			IL_30B:
			return this.ScanAttlist5();
			IL_312:
			return this.ScanAttlist6();
			IL_319:
			return this.ScanAttlist7();
			IL_320:
			return this.ScanNotation1();
			IL_327:
			return this.ScanSystemId();
			IL_32E:
			return this.ScanPublicId1();
			IL_335:
			return this.ScanPublicId2();
			IL_33C:
			return this.ScanEntity1();
			IL_343:
			return this.ScanEntity2();
			IL_34A:
			return this.ScanEntity3();
			IL_351:
			return this.ScanCondSection1();
			IL_358:
			return this.ScanCondSection2();
			IL_35F:
			return this.ScanCondSection3();
			IL_366:
			return this.ScanClosingTag();
		}

		private DtdParser.Token ScanSubsetContent()
		{
			for (;;)
			{
				char c = this.chars[this.curPos];
				if (c != '<')
				{
					if (c == ']')
					{
						if (this.charsUsed - this.curPos < 2 && !this.readerAdapter.IsEof)
						{
							goto IL_513;
						}
						if (this.chars[this.curPos + 1] != ']')
						{
							goto Block_40;
						}
						if (this.charsUsed - this.curPos < 3 && !this.readerAdapter.IsEof)
						{
							goto IL_513;
						}
						if (this.chars[this.curPos + 1] == ']' && this.chars[this.curPos + 2] == '>')
						{
							goto Block_43;
						}
					}
					if (this.charsUsed - this.curPos != 0)
					{
						this.Throw(this.curPos, "Expected DTD markup was not found.");
					}
				}
				else
				{
					char c2 = this.chars[this.curPos + 1];
					if (c2 != '!')
					{
						if (c2 == '?')
						{
							goto IL_41B;
						}
						if (this.charsUsed - this.curPos >= 2)
						{
							goto Block_38;
						}
					}
					else
					{
						char c3 = this.chars[this.curPos + 2];
						if (c3 <= 'A')
						{
							if (c3 != '-')
							{
								if (c3 == 'A')
								{
									if (this.charsUsed - this.curPos >= 9)
									{
										goto Block_22;
									}
									goto IL_513;
								}
							}
							else
							{
								if (this.chars[this.curPos + 3] == '-')
								{
									goto Block_35;
								}
								if (this.charsUsed - this.curPos >= 4)
								{
									this.Throw(this.curPos, "Expected DTD markup was not found.");
									goto IL_513;
								}
								goto IL_513;
							}
						}
						else if (c3 != 'E')
						{
							if (c3 != 'N')
							{
								if (c3 == '[')
								{
									goto IL_38A;
								}
							}
							else
							{
								if (this.charsUsed - this.curPos >= 10)
								{
									goto Block_28;
								}
								goto IL_513;
							}
						}
						else if (this.chars[this.curPos + 3] == 'L')
						{
							if (this.charsUsed - this.curPos >= 9)
							{
								break;
							}
							goto IL_513;
						}
						else if (this.chars[this.curPos + 3] == 'N')
						{
							if (this.charsUsed - this.curPos >= 8)
							{
								goto Block_17;
							}
							goto IL_513;
						}
						else
						{
							if (this.charsUsed - this.curPos >= 4)
							{
								goto Block_21;
							}
							goto IL_513;
						}
						if (this.charsUsed - this.curPos >= 3)
						{
							this.Throw(this.curPos + 2, "Expected DTD markup was not found.");
						}
					}
				}
				IL_513:
				if (this.ReadData() == 0)
				{
					this.Throw(this.charsUsed, "Incomplete DTD content.");
				}
			}
			if (this.chars[this.curPos + 4] != 'E' || this.chars[this.curPos + 5] != 'M' || this.chars[this.curPos + 6] != 'E' || this.chars[this.curPos + 7] != 'N' || this.chars[this.curPos + 8] != 'T')
			{
				this.Throw(this.curPos, "Expected DTD markup was not found.");
			}
			this.curPos += 9;
			this.scanningFunction = DtdParser.ScanningFunction.QName;
			this.nextScaningFunction = DtdParser.ScanningFunction.Element1;
			return DtdParser.Token.ElementDecl;
			Block_17:
			if (this.chars[this.curPos + 4] != 'T' || this.chars[this.curPos + 5] != 'I' || this.chars[this.curPos + 6] != 'T' || this.chars[this.curPos + 7] != 'Y')
			{
				this.Throw(this.curPos, "Expected DTD markup was not found.");
			}
			this.curPos += 8;
			this.scanningFunction = DtdParser.ScanningFunction.Entity1;
			return DtdParser.Token.EntityDecl;
			Block_21:
			this.Throw(this.curPos, "Expected DTD markup was not found.");
			return DtdParser.Token.None;
			Block_22:
			if (this.chars[this.curPos + 3] != 'T' || this.chars[this.curPos + 4] != 'T' || this.chars[this.curPos + 5] != 'L' || this.chars[this.curPos + 6] != 'I' || this.chars[this.curPos + 7] != 'S' || this.chars[this.curPos + 8] != 'T')
			{
				this.Throw(this.curPos, "Expected DTD markup was not found.");
			}
			this.curPos += 9;
			this.scanningFunction = DtdParser.ScanningFunction.QName;
			this.nextScaningFunction = DtdParser.ScanningFunction.Attlist1;
			return DtdParser.Token.AttlistDecl;
			Block_28:
			if (this.chars[this.curPos + 3] != 'O' || this.chars[this.curPos + 4] != 'T' || this.chars[this.curPos + 5] != 'A' || this.chars[this.curPos + 6] != 'T' || this.chars[this.curPos + 7] != 'I' || this.chars[this.curPos + 8] != 'O' || this.chars[this.curPos + 9] != 'N')
			{
				this.Throw(this.curPos, "Expected DTD markup was not found.");
			}
			this.curPos += 10;
			this.scanningFunction = DtdParser.ScanningFunction.Name;
			this.nextScaningFunction = DtdParser.ScanningFunction.Notation1;
			return DtdParser.Token.NotationDecl;
			IL_38A:
			this.curPos += 3;
			this.scanningFunction = DtdParser.ScanningFunction.CondSection1;
			return DtdParser.Token.CondSectionStart;
			Block_35:
			this.curPos += 4;
			return DtdParser.Token.Comment;
			IL_41B:
			this.curPos += 2;
			return DtdParser.Token.PI;
			Block_38:
			this.Throw(this.curPos, "Expected DTD markup was not found.");
			return DtdParser.Token.None;
			Block_40:
			this.curPos++;
			this.scanningFunction = DtdParser.ScanningFunction.ClosingTag;
			return DtdParser.Token.RightBracket;
			Block_43:
			this.curPos += 3;
			return DtdParser.Token.CondSectionEnd;
		}

		private DtdParser.Token ScanNameExpected()
		{
			this.ScanName();
			this.scanningFunction = this.nextScaningFunction;
			return DtdParser.Token.Name;
		}

		private DtdParser.Token ScanQNameExpected()
		{
			this.ScanQName();
			this.scanningFunction = this.nextScaningFunction;
			return DtdParser.Token.QName;
		}

		private DtdParser.Token ScanNmtokenExpected()
		{
			this.ScanNmtoken();
			this.scanningFunction = this.nextScaningFunction;
			return DtdParser.Token.Nmtoken;
		}

		private DtdParser.Token ScanDoctype1()
		{
			char c = this.chars[this.curPos];
			if (c <= 'P')
			{
				if (c == '>')
				{
					this.curPos++;
					this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
					return DtdParser.Token.GreaterThan;
				}
				if (c == 'P')
				{
					if (!this.EatPublicKeyword())
					{
						this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
					}
					this.nextScaningFunction = DtdParser.ScanningFunction.Doctype2;
					this.scanningFunction = DtdParser.ScanningFunction.PublicId1;
					return DtdParser.Token.PUBLIC;
				}
			}
			else
			{
				if (c == 'S')
				{
					if (!this.EatSystemKeyword())
					{
						this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
					}
					this.nextScaningFunction = DtdParser.ScanningFunction.Doctype2;
					this.scanningFunction = DtdParser.ScanningFunction.SystemId;
					return DtdParser.Token.SYSTEM;
				}
				if (c == '[')
				{
					this.curPos++;
					this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
					return DtdParser.Token.LeftBracket;
				}
			}
			this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanDoctype2()
		{
			char c = this.chars[this.curPos];
			if (c == '>')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
				return DtdParser.Token.GreaterThan;
			}
			if (c == '[')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
				return DtdParser.Token.LeftBracket;
			}
			this.Throw(this.curPos, "Expecting an internal subset or the end of the DOCTYPE declaration.");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanClosingTag()
		{
			if (this.chars[this.curPos] != '>')
			{
				this.ThrowUnexpectedToken(this.curPos, ">");
			}
			this.curPos++;
			this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
			return DtdParser.Token.GreaterThan;
		}

		private DtdParser.Token ScanElement1()
		{
			for (;;)
			{
				char c = this.chars[this.curPos];
				if (c != '(')
				{
					if (c != 'A')
					{
						if (c != 'E')
						{
							goto IL_10A;
						}
						if (this.charsUsed - this.curPos >= 5)
						{
							if (this.chars[this.curPos + 1] == 'M' && this.chars[this.curPos + 2] == 'P' && this.chars[this.curPos + 3] == 'T' && this.chars[this.curPos + 4] == 'Y')
							{
								goto Block_7;
							}
							goto IL_10A;
						}
					}
					else if (this.charsUsed - this.curPos >= 3)
					{
						if (this.chars[this.curPos + 1] == 'N' && this.chars[this.curPos + 2] == 'Y')
						{
							goto Block_10;
						}
						goto IL_10A;
					}
					IL_11B:
					if (this.ReadData() == 0)
					{
						this.Throw(this.curPos, "Incomplete DTD content.");
						continue;
					}
					continue;
					IL_10A:
					this.Throw(this.curPos, "Invalid content model.");
					goto IL_11B;
				}
				break;
			}
			this.scanningFunction = DtdParser.ScanningFunction.Element2;
			this.curPos++;
			return DtdParser.Token.LeftParen;
			Block_7:
			this.curPos += 5;
			this.scanningFunction = DtdParser.ScanningFunction.ClosingTag;
			return DtdParser.Token.EMPTY;
			Block_10:
			this.curPos += 3;
			this.scanningFunction = DtdParser.ScanningFunction.ClosingTag;
			return DtdParser.Token.ANY;
		}

		private DtdParser.Token ScanElement2()
		{
			if (this.chars[this.curPos] == '#')
			{
				while (this.charsUsed - this.curPos < 7)
				{
					if (this.ReadData() == 0)
					{
						this.Throw(this.curPos, "Incomplete DTD content.");
					}
				}
				if (this.chars[this.curPos + 1] == 'P' && this.chars[this.curPos + 2] == 'C' && this.chars[this.curPos + 3] == 'D' && this.chars[this.curPos + 4] == 'A' && this.chars[this.curPos + 5] == 'T' && this.chars[this.curPos + 6] == 'A')
				{
					this.curPos += 7;
					this.scanningFunction = DtdParser.ScanningFunction.Element6;
					return DtdParser.Token.PCDATA;
				}
				this.Throw(this.curPos + 1, "Expecting 'PCDATA'.");
			}
			this.scanningFunction = DtdParser.ScanningFunction.Element3;
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanElement3()
		{
			char c = this.chars[this.curPos];
			if (c == '(')
			{
				this.curPos++;
				return DtdParser.Token.LeftParen;
			}
			if (c != '>')
			{
				this.ScanQName();
				this.scanningFunction = DtdParser.ScanningFunction.Element4;
				return DtdParser.Token.QName;
			}
			this.curPos++;
			this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
			return DtdParser.Token.GreaterThan;
		}

		private DtdParser.Token ScanElement4()
		{
			this.scanningFunction = DtdParser.ScanningFunction.Element5;
			char c = this.chars[this.curPos];
			DtdParser.Token result;
			if (c != '*')
			{
				if (c != '+')
				{
					if (c != '?')
					{
						return DtdParser.Token.None;
					}
					result = DtdParser.Token.QMark;
				}
				else
				{
					result = DtdParser.Token.Plus;
				}
			}
			else
			{
				result = DtdParser.Token.Star;
			}
			if (this.whitespaceSeen)
			{
				this.Throw(this.curPos, "White space not allowed before '?', '*', or '+'.");
			}
			this.curPos++;
			return result;
		}

		private DtdParser.Token ScanElement5()
		{
			char c = this.chars[this.curPos];
			if (c <= ',')
			{
				if (c == ')')
				{
					this.curPos++;
					this.scanningFunction = DtdParser.ScanningFunction.Element4;
					return DtdParser.Token.RightParen;
				}
				if (c == ',')
				{
					this.curPos++;
					this.scanningFunction = DtdParser.ScanningFunction.Element3;
					return DtdParser.Token.Comma;
				}
			}
			else
			{
				if (c == '>')
				{
					this.curPos++;
					this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
					return DtdParser.Token.GreaterThan;
				}
				if (c == '|')
				{
					this.curPos++;
					this.scanningFunction = DtdParser.ScanningFunction.Element3;
					return DtdParser.Token.Or;
				}
			}
			this.Throw(this.curPos, "Expecting '?', '*', or '+'.");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanElement6()
		{
			char c = this.chars[this.curPos];
			if (c == ')')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.Element7;
				return DtdParser.Token.RightParen;
			}
			if (c != '|')
			{
				this.ThrowUnexpectedToken(this.curPos, ")", "|");
				return DtdParser.Token.None;
			}
			this.curPos++;
			this.nextScaningFunction = DtdParser.ScanningFunction.Element6;
			this.scanningFunction = DtdParser.ScanningFunction.QName;
			return DtdParser.Token.Or;
		}

		private DtdParser.Token ScanElement7()
		{
			this.scanningFunction = DtdParser.ScanningFunction.ClosingTag;
			if (this.chars[this.curPos] == '*' && !this.whitespaceSeen)
			{
				this.curPos++;
				return DtdParser.Token.Star;
			}
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanAttlist1()
		{
			if (this.chars[this.curPos] == '>')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
				return DtdParser.Token.GreaterThan;
			}
			if (!this.whitespaceSeen)
			{
				this.Throw(this.curPos, "'{0}' is an unexpected token. Expecting white space.", this.ParseUnexpectedToken(this.curPos));
			}
			this.ScanQName();
			this.scanningFunction = DtdParser.ScanningFunction.Attlist2;
			return DtdParser.Token.QName;
		}

		private DtdParser.Token ScanAttlist2()
		{
			for (;;)
			{
				char c = this.chars[this.curPos];
				if (c <= 'C')
				{
					if (c == '(')
					{
						break;
					}
					if (c != 'C')
					{
						goto IL_44E;
					}
					if (this.charsUsed - this.curPos >= 5)
					{
						goto Block_6;
					}
				}
				else if (c != 'E')
				{
					if (c != 'I')
					{
						if (c != 'N')
						{
							goto IL_44E;
						}
						if (this.charsUsed - this.curPos >= 8 || this.readerAdapter.IsEof)
						{
							char c2 = this.chars[this.curPos + 1];
							if (c2 == 'M')
							{
								goto IL_390;
							}
							if (c2 == 'O')
							{
								goto Block_24;
							}
							this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
						}
					}
					else if (this.charsUsed - this.curPos >= 6)
					{
						goto Block_17;
					}
				}
				else if (this.charsUsed - this.curPos >= 9)
				{
					this.scanningFunction = DtdParser.ScanningFunction.Attlist6;
					if (this.chars[this.curPos + 1] != 'N' || this.chars[this.curPos + 2] != 'T' || this.chars[this.curPos + 3] != 'I' || this.chars[this.curPos + 4] != 'T')
					{
						this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
					}
					char c2 = this.chars[this.curPos + 5];
					if (c2 == 'I')
					{
						goto IL_17C;
					}
					if (c2 == 'Y')
					{
						goto IL_1C3;
					}
					this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
				}
				IL_45F:
				if (this.ReadData() == 0)
				{
					this.Throw(this.curPos, "Incomplete DTD content.");
					continue;
				}
				continue;
				IL_44E:
				this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
				goto IL_45F;
			}
			this.curPos++;
			this.scanningFunction = DtdParser.ScanningFunction.Nmtoken;
			this.nextScaningFunction = DtdParser.ScanningFunction.Attlist5;
			return DtdParser.Token.LeftParen;
			Block_6:
			if (this.chars[this.curPos + 1] != 'D' || this.chars[this.curPos + 2] != 'A' || this.chars[this.curPos + 3] != 'T' || this.chars[this.curPos + 4] != 'A')
			{
				this.Throw(this.curPos, "Invalid attribute type.");
			}
			this.curPos += 5;
			this.scanningFunction = DtdParser.ScanningFunction.Attlist6;
			return DtdParser.Token.CDATA;
			IL_17C:
			if (this.chars[this.curPos + 6] != 'E' || this.chars[this.curPos + 7] != 'S')
			{
				this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
			}
			this.curPos += 8;
			return DtdParser.Token.ENTITIES;
			IL_1C3:
			this.curPos += 6;
			return DtdParser.Token.ENTITY;
			Block_17:
			this.scanningFunction = DtdParser.ScanningFunction.Attlist6;
			if (this.chars[this.curPos + 1] != 'D')
			{
				this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
			}
			if (this.chars[this.curPos + 2] != 'R')
			{
				this.curPos += 2;
				return DtdParser.Token.ID;
			}
			if (this.chars[this.curPos + 3] != 'E' || this.chars[this.curPos + 4] != 'F')
			{
				this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
			}
			if (this.chars[this.curPos + 5] != 'S')
			{
				this.curPos += 5;
				return DtdParser.Token.IDREF;
			}
			this.curPos += 6;
			return DtdParser.Token.IDREFS;
			Block_24:
			if (this.chars[this.curPos + 2] != 'T' || this.chars[this.curPos + 3] != 'A' || this.chars[this.curPos + 4] != 'T' || this.chars[this.curPos + 5] != 'I' || this.chars[this.curPos + 6] != 'O' || this.chars[this.curPos + 7] != 'N')
			{
				this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
			}
			this.curPos += 8;
			this.scanningFunction = DtdParser.ScanningFunction.Attlist3;
			return DtdParser.Token.NOTATION;
			IL_390:
			if (this.chars[this.curPos + 2] != 'T' || this.chars[this.curPos + 3] != 'O' || this.chars[this.curPos + 4] != 'K' || this.chars[this.curPos + 5] != 'E' || this.chars[this.curPos + 6] != 'N')
			{
				this.Throw(this.curPos, "'{0}' is an invalid attribute type.");
			}
			this.scanningFunction = DtdParser.ScanningFunction.Attlist6;
			if (this.chars[this.curPos + 7] == 'S')
			{
				this.curPos += 8;
				return DtdParser.Token.NMTOKENS;
			}
			this.curPos += 7;
			return DtdParser.Token.NMTOKEN;
		}

		private DtdParser.Token ScanAttlist3()
		{
			if (this.chars[this.curPos] == '(')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.Name;
				this.nextScaningFunction = DtdParser.ScanningFunction.Attlist4;
				return DtdParser.Token.LeftParen;
			}
			this.ThrowUnexpectedToken(this.curPos, "(");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanAttlist4()
		{
			char c = this.chars[this.curPos];
			if (c == ')')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.Attlist6;
				return DtdParser.Token.RightParen;
			}
			if (c != '|')
			{
				this.ThrowUnexpectedToken(this.curPos, ")", "|");
				return DtdParser.Token.None;
			}
			this.curPos++;
			this.scanningFunction = DtdParser.ScanningFunction.Name;
			this.nextScaningFunction = DtdParser.ScanningFunction.Attlist4;
			return DtdParser.Token.Or;
		}

		private DtdParser.Token ScanAttlist5()
		{
			char c = this.chars[this.curPos];
			if (c == ')')
			{
				this.curPos++;
				this.scanningFunction = DtdParser.ScanningFunction.Attlist6;
				return DtdParser.Token.RightParen;
			}
			if (c != '|')
			{
				this.ThrowUnexpectedToken(this.curPos, ")", "|");
				return DtdParser.Token.None;
			}
			this.curPos++;
			this.scanningFunction = DtdParser.ScanningFunction.Nmtoken;
			this.nextScaningFunction = DtdParser.ScanningFunction.Attlist5;
			return DtdParser.Token.Or;
		}

		private DtdParser.Token ScanAttlist6()
		{
			for (;;)
			{
				char c = this.chars[this.curPos];
				if (c == '"')
				{
					break;
				}
				if (c != '#')
				{
					if (c == '\'')
					{
						break;
					}
					this.Throw(this.curPos, "Expecting an attribute type.");
				}
				else if (this.charsUsed - this.curPos >= 6)
				{
					char c2 = this.chars[this.curPos + 1];
					if (c2 == 'F')
					{
						goto IL_1E1;
					}
					if (c2 != 'I')
					{
						if (c2 == 'R')
						{
							if (this.charsUsed - this.curPos >= 9)
							{
								goto Block_6;
							}
						}
						else
						{
							this.Throw(this.curPos, "Expecting an attribute type.");
						}
					}
					else if (this.charsUsed - this.curPos >= 8)
					{
						goto Block_13;
					}
				}
				if (this.ReadData() == 0)
				{
					this.Throw(this.curPos, "Incomplete DTD content.");
				}
			}
			this.ScanLiteral(DtdParser.LiteralType.AttributeValue);
			this.scanningFunction = DtdParser.ScanningFunction.Attlist1;
			return DtdParser.Token.Literal;
			Block_6:
			if (this.chars[this.curPos + 2] != 'E' || this.chars[this.curPos + 3] != 'Q' || this.chars[this.curPos + 4] != 'U' || this.chars[this.curPos + 5] != 'I' || this.chars[this.curPos + 6] != 'R' || this.chars[this.curPos + 7] != 'E' || this.chars[this.curPos + 8] != 'D')
			{
				this.Throw(this.curPos, "Expecting an attribute type.");
			}
			this.curPos += 9;
			this.scanningFunction = DtdParser.ScanningFunction.Attlist1;
			return DtdParser.Token.REQUIRED;
			Block_13:
			if (this.chars[this.curPos + 2] != 'M' || this.chars[this.curPos + 3] != 'P' || this.chars[this.curPos + 4] != 'L' || this.chars[this.curPos + 5] != 'I' || this.chars[this.curPos + 6] != 'E' || this.chars[this.curPos + 7] != 'D')
			{
				this.Throw(this.curPos, "Expecting an attribute type.");
			}
			this.curPos += 8;
			this.scanningFunction = DtdParser.ScanningFunction.Attlist1;
			return DtdParser.Token.IMPLIED;
			IL_1E1:
			if (this.chars[this.curPos + 2] != 'I' || this.chars[this.curPos + 3] != 'X' || this.chars[this.curPos + 4] != 'E' || this.chars[this.curPos + 5] != 'D')
			{
				this.Throw(this.curPos, "Expecting an attribute type.");
			}
			this.curPos += 6;
			this.scanningFunction = DtdParser.ScanningFunction.Attlist7;
			return DtdParser.Token.FIXED;
		}

		private DtdParser.Token ScanAttlist7()
		{
			char c = this.chars[this.curPos];
			if (c == '"' || c == '\'')
			{
				this.ScanLiteral(DtdParser.LiteralType.AttributeValue);
				this.scanningFunction = DtdParser.ScanningFunction.Attlist1;
				return DtdParser.Token.Literal;
			}
			this.ThrowUnexpectedToken(this.curPos, "\"", "'");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanLiteral(DtdParser.LiteralType literalType)
		{
			char c = this.chars[this.curPos];
			char value = (literalType == DtdParser.LiteralType.AttributeValue) ? ' ' : '\n';
			int num = this.currentEntityId;
			this.literalLineInfo.Set(this.LineNo, this.LinePos);
			this.curPos++;
			this.tokenStartPos = this.curPos;
			this.stringBuilder.Length = 0;
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)this.chars[this.curPos]] & 128) == 0 || this.chars[this.curPos] == '%')
				{
					if (this.chars[this.curPos] == c && this.currentEntityId == num)
					{
						break;
					}
					int num2 = this.curPos - this.tokenStartPos;
					if (num2 > 0)
					{
						this.stringBuilder.Append(this.chars, this.tokenStartPos, num2);
						this.tokenStartPos = this.curPos;
					}
					char c2 = this.chars[this.curPos];
					if (c2 <= '\'')
					{
						switch (c2)
						{
						case '\t':
							if (literalType == DtdParser.LiteralType.AttributeValue && this.Normalize)
							{
								this.stringBuilder.Append(' ');
								this.tokenStartPos++;
							}
							this.curPos++;
							continue;
						case '\n':
							this.curPos++;
							if (this.Normalize)
							{
								this.stringBuilder.Append(value);
								this.tokenStartPos = this.curPos;
							}
							this.readerAdapter.OnNewLine(this.curPos);
							continue;
						case '\v':
						case '\f':
							goto IL_54D;
						case '\r':
							if (this.chars[this.curPos + 1] == '\n')
							{
								if (this.Normalize)
								{
									if (literalType == DtdParser.LiteralType.AttributeValue)
									{
										this.stringBuilder.Append(this.readerAdapter.IsEntityEolNormalized ? "  " : " ");
									}
									else
									{
										this.stringBuilder.Append(this.readerAdapter.IsEntityEolNormalized ? "\r\n" : "\n");
									}
									this.tokenStartPos = this.curPos + 2;
									this.SaveParsingBuffer();
									IDtdParserAdapter dtdParserAdapter = this.readerAdapter;
									int currentPosition = dtdParserAdapter.CurrentPosition;
									dtdParserAdapter.CurrentPosition = currentPosition + 1;
								}
								this.curPos += 2;
							}
							else
							{
								if (this.curPos + 1 == this.charsUsed)
								{
									goto IL_5CF;
								}
								this.curPos++;
								if (this.Normalize)
								{
									this.stringBuilder.Append(value);
									this.tokenStartPos = this.curPos;
								}
							}
							this.readerAdapter.OnNewLine(this.curPos);
							continue;
						default:
							switch (c2)
							{
							case '"':
							case '\'':
								break;
							case '#':
							case '$':
								goto IL_54D;
							case '%':
								if (literalType != DtdParser.LiteralType.EntityReplText)
								{
									this.curPos++;
									continue;
								}
								this.HandleEntityReference(true, true, literalType == DtdParser.LiteralType.AttributeValue);
								this.tokenStartPos = this.curPos;
								continue;
							case '&':
								if (literalType == DtdParser.LiteralType.SystemOrPublicID)
								{
									this.curPos++;
									continue;
								}
								if (this.curPos + 1 == this.charsUsed)
								{
									goto IL_5CF;
								}
								if (this.chars[this.curPos + 1] == '#')
								{
									this.SaveParsingBuffer();
									int num3 = this.readerAdapter.ParseNumericCharRef(this.SaveInternalSubsetValue ? this.internalSubsetValueSb : null);
									this.LoadParsingBuffer();
									this.stringBuilder.Append(this.chars, this.curPos, num3 - this.curPos);
									this.readerAdapter.CurrentPosition = num3;
									this.tokenStartPos = num3;
									this.curPos = num3;
									continue;
								}
								this.SaveParsingBuffer();
								if (literalType == DtdParser.LiteralType.AttributeValue)
								{
									int num4 = this.readerAdapter.ParseNamedCharRef(true, this.SaveInternalSubsetValue ? this.internalSubsetValueSb : null);
									this.LoadParsingBuffer();
									if (num4 >= 0)
									{
										this.stringBuilder.Append(this.chars, this.curPos, num4 - this.curPos);
										this.readerAdapter.CurrentPosition = num4;
										this.tokenStartPos = num4;
										this.curPos = num4;
										continue;
									}
									this.HandleEntityReference(false, true, true);
									this.tokenStartPos = this.curPos;
									continue;
								}
								else
								{
									int num5 = this.readerAdapter.ParseNamedCharRef(false, null);
									this.LoadParsingBuffer();
									if (num5 >= 0)
									{
										this.tokenStartPos = this.curPos;
										this.curPos = num5;
										continue;
									}
									this.stringBuilder.Append('&');
									this.curPos++;
									this.tokenStartPos = this.curPos;
									XmlQualifiedName entityName = this.ScanEntityName();
									this.VerifyEntityReference(entityName, false, false, false);
									continue;
								}
								break;
							default:
								goto IL_54D;
							}
							break;
						}
					}
					else
					{
						if (c2 == '<')
						{
							if (literalType == DtdParser.LiteralType.AttributeValue)
							{
								this.Throw(this.curPos, "'{0}', hexadecimal value {1}, is an invalid attribute character.", XmlException.BuildCharExceptionArgs('<', '\0'));
							}
							this.curPos++;
							continue;
						}
						if (c2 != '>')
						{
							goto IL_54D;
						}
					}
					this.curPos++;
					continue;
					IL_54D:
					if (this.curPos != this.charsUsed)
					{
						if (!XmlCharType.IsHighSurrogate((int)this.chars[this.curPos]))
						{
							goto IL_5B4;
						}
						if (this.curPos + 1 != this.charsUsed)
						{
							this.curPos++;
							if (XmlCharType.IsLowSurrogate((int)this.chars[this.curPos]))
							{
								this.curPos++;
								continue;
							}
							goto IL_5B4;
						}
					}
					IL_5CF:
					if ((this.readerAdapter.IsEof || this.ReadData() == 0) && (literalType == DtdParser.LiteralType.SystemOrPublicID || !this.HandleEntityEnd(true)))
					{
						this.Throw(this.curPos, "There is an unclosed literal string.");
					}
					this.tokenStartPos = this.curPos;
				}
				else
				{
					this.curPos++;
				}
			}
			if (this.stringBuilder.Length > 0)
			{
				this.stringBuilder.Append(this.chars, this.tokenStartPos, this.curPos - this.tokenStartPos);
			}
			this.curPos++;
			this.literalQuoteChar = c;
			return DtdParser.Token.Literal;
			IL_5B4:
			this.ThrowInvalidChar(this.chars, this.charsUsed, this.curPos);
			return DtdParser.Token.None;
		}

		private XmlQualifiedName ScanEntityName()
		{
			try
			{
				this.ScanName();
			}
			catch (XmlException ex)
			{
				this.Throw("An error occurred while parsing EntityName.", string.Empty, ex.LineNumber, ex.LinePosition);
			}
			if (this.chars[this.curPos] != ';')
			{
				this.ThrowUnexpectedToken(this.curPos, ";");
			}
			XmlQualifiedName nameQualified = this.GetNameQualified(false);
			this.curPos++;
			return nameQualified;
		}

		private DtdParser.Token ScanNotation1()
		{
			char c = this.chars[this.curPos];
			if (c == 'P')
			{
				if (!this.EatPublicKeyword())
				{
					this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
				}
				this.nextScaningFunction = DtdParser.ScanningFunction.ClosingTag;
				this.scanningFunction = DtdParser.ScanningFunction.PublicId1;
				return DtdParser.Token.PUBLIC;
			}
			if (c != 'S')
			{
				this.Throw(this.curPos, "Expecting a system identifier or a public identifier.");
				return DtdParser.Token.None;
			}
			if (!this.EatSystemKeyword())
			{
				this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
			}
			this.nextScaningFunction = DtdParser.ScanningFunction.ClosingTag;
			this.scanningFunction = DtdParser.ScanningFunction.SystemId;
			return DtdParser.Token.SYSTEM;
		}

		private DtdParser.Token ScanSystemId()
		{
			if (this.chars[this.curPos] != '"' && this.chars[this.curPos] != '\'')
			{
				this.ThrowUnexpectedToken(this.curPos, "\"", "'");
			}
			this.ScanLiteral(DtdParser.LiteralType.SystemOrPublicID);
			this.scanningFunction = this.nextScaningFunction;
			return DtdParser.Token.Literal;
		}

		private DtdParser.Token ScanEntity1()
		{
			if (this.chars[this.curPos] == '%')
			{
				this.curPos++;
				this.nextScaningFunction = DtdParser.ScanningFunction.Entity2;
				this.scanningFunction = DtdParser.ScanningFunction.Name;
				return DtdParser.Token.Percent;
			}
			this.ScanName();
			this.scanningFunction = DtdParser.ScanningFunction.Entity2;
			return DtdParser.Token.Name;
		}

		private DtdParser.Token ScanEntity2()
		{
			char c = this.chars[this.curPos];
			if (c <= '\'')
			{
				if (c == '"' || c == '\'')
				{
					this.ScanLiteral(DtdParser.LiteralType.EntityReplText);
					this.scanningFunction = DtdParser.ScanningFunction.ClosingTag;
					return DtdParser.Token.Literal;
				}
			}
			else
			{
				if (c == 'P')
				{
					if (!this.EatPublicKeyword())
					{
						this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
					}
					this.nextScaningFunction = DtdParser.ScanningFunction.Entity3;
					this.scanningFunction = DtdParser.ScanningFunction.PublicId1;
					return DtdParser.Token.PUBLIC;
				}
				if (c == 'S')
				{
					if (!this.EatSystemKeyword())
					{
						this.Throw(this.curPos, "Expecting external ID, '[' or '>'.");
					}
					this.nextScaningFunction = DtdParser.ScanningFunction.Entity3;
					this.scanningFunction = DtdParser.ScanningFunction.SystemId;
					return DtdParser.Token.SYSTEM;
				}
			}
			this.Throw(this.curPos, "Expecting an external identifier or an entity value.");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanEntity3()
		{
			if (this.chars[this.curPos] == 'N')
			{
				while (this.charsUsed - this.curPos < 5)
				{
					if (this.ReadData() == 0)
					{
						goto IL_9A;
					}
				}
				if (this.chars[this.curPos + 1] == 'D' && this.chars[this.curPos + 2] == 'A' && this.chars[this.curPos + 3] == 'T' && this.chars[this.curPos + 4] == 'A')
				{
					this.curPos += 5;
					this.scanningFunction = DtdParser.ScanningFunction.Name;
					this.nextScaningFunction = DtdParser.ScanningFunction.ClosingTag;
					return DtdParser.Token.NData;
				}
			}
			IL_9A:
			this.scanningFunction = DtdParser.ScanningFunction.ClosingTag;
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanPublicId1()
		{
			if (this.chars[this.curPos] != '"' && this.chars[this.curPos] != '\'')
			{
				this.ThrowUnexpectedToken(this.curPos, "\"", "'");
			}
			this.ScanLiteral(DtdParser.LiteralType.SystemOrPublicID);
			this.scanningFunction = DtdParser.ScanningFunction.PublicId2;
			return DtdParser.Token.Literal;
		}

		private DtdParser.Token ScanPublicId2()
		{
			if (this.chars[this.curPos] != '"' && this.chars[this.curPos] != '\'')
			{
				this.scanningFunction = this.nextScaningFunction;
				return DtdParser.Token.None;
			}
			this.ScanLiteral(DtdParser.LiteralType.SystemOrPublicID);
			this.scanningFunction = this.nextScaningFunction;
			return DtdParser.Token.Literal;
		}

		private DtdParser.Token ScanCondSection1()
		{
			if (this.chars[this.curPos] != 'I')
			{
				this.Throw(this.curPos, "Conditional sections must specify the keyword 'IGNORE' or 'INCLUDE'.");
			}
			this.curPos++;
			for (;;)
			{
				if (this.charsUsed - this.curPos >= 5)
				{
					char c = this.chars[this.curPos];
					if (c == 'G')
					{
						goto IL_121;
					}
					if (c != 'N')
					{
						goto IL_1AA;
					}
					if (this.charsUsed - this.curPos >= 6)
					{
						break;
					}
				}
				if (this.ReadData() == 0)
				{
					this.Throw(this.curPos, "Incomplete DTD content.");
				}
			}
			if (this.chars[this.curPos + 1] == 'C' && this.chars[this.curPos + 2] == 'L' && this.chars[this.curPos + 3] == 'U' && this.chars[this.curPos + 4] == 'D' && this.chars[this.curPos + 5] == 'E' && !this.xmlCharType.IsNameSingleChar(this.chars[this.curPos + 6]))
			{
				this.nextScaningFunction = DtdParser.ScanningFunction.SubsetContent;
				this.scanningFunction = DtdParser.ScanningFunction.CondSection2;
				this.curPos += 6;
				return DtdParser.Token.INCLUDE;
			}
			goto IL_1AA;
			IL_121:
			if (this.chars[this.curPos + 1] == 'N' && this.chars[this.curPos + 2] == 'O' && this.chars[this.curPos + 3] == 'R' && this.chars[this.curPos + 4] == 'E' && !this.xmlCharType.IsNameSingleChar(this.chars[this.curPos + 5]))
			{
				this.nextScaningFunction = DtdParser.ScanningFunction.CondSection3;
				this.scanningFunction = DtdParser.ScanningFunction.CondSection2;
				this.curPos += 5;
				return DtdParser.Token.IGNORE;
			}
			IL_1AA:
			this.Throw(this.curPos - 1, "Conditional sections must specify the keyword 'IGNORE' or 'INCLUDE'.");
			return DtdParser.Token.None;
		}

		private DtdParser.Token ScanCondSection2()
		{
			if (this.chars[this.curPos] != '[')
			{
				this.ThrowUnexpectedToken(this.curPos, "[");
			}
			this.curPos++;
			this.scanningFunction = this.nextScaningFunction;
			return DtdParser.Token.LeftBracket;
		}

		private DtdParser.Token ScanCondSection3()
		{
			int num = 0;
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)this.chars[this.curPos]] & 64) == 0 || this.chars[this.curPos] == ']')
				{
					char c = this.chars[this.curPos];
					if (c <= '&')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
							this.curPos++;
							this.readerAdapter.OnNewLine(this.curPos);
							continue;
						case '\v':
						case '\f':
							goto IL_21A;
						case '\r':
							if (this.chars[this.curPos + 1] == '\n')
							{
								this.curPos += 2;
							}
							else
							{
								if (this.curPos + 1 >= this.charsUsed && !this.readerAdapter.IsEof)
								{
									goto IL_29C;
								}
								this.curPos++;
							}
							this.readerAdapter.OnNewLine(this.curPos);
							continue;
						default:
							if (c != '"' && c != '&')
							{
								goto IL_21A;
							}
							break;
						}
					}
					else if (c != '\'')
					{
						if (c != '<')
						{
							if (c != ']')
							{
								goto IL_21A;
							}
							if (this.charsUsed - this.curPos < 3)
							{
								goto IL_29C;
							}
							if (this.chars[this.curPos + 1] != ']' || this.chars[this.curPos + 2] != '>')
							{
								this.curPos++;
								continue;
							}
							if (num > 0)
							{
								num--;
								this.curPos += 3;
								continue;
							}
							break;
						}
						else
						{
							if (this.charsUsed - this.curPos < 3)
							{
								goto IL_29C;
							}
							if (this.chars[this.curPos + 1] != '!' || this.chars[this.curPos + 2] != '[')
							{
								this.curPos++;
								continue;
							}
							num++;
							this.curPos += 3;
							continue;
						}
					}
					this.curPos++;
					continue;
					IL_21A:
					if (this.curPos != this.charsUsed)
					{
						if (!XmlCharType.IsHighSurrogate((int)this.chars[this.curPos]))
						{
							goto IL_281;
						}
						if (this.curPos + 1 != this.charsUsed)
						{
							this.curPos++;
							if (XmlCharType.IsLowSurrogate((int)this.chars[this.curPos]))
							{
								this.curPos++;
								continue;
							}
							goto IL_281;
						}
					}
					IL_29C:
					if (this.readerAdapter.IsEof || this.ReadData() == 0)
					{
						if (this.HandleEntityEnd(false))
						{
							continue;
						}
						this.Throw(this.curPos, "There is an unclosed conditional section.");
					}
					this.tokenStartPos = this.curPos;
				}
				else
				{
					this.curPos++;
				}
			}
			this.curPos += 3;
			this.scanningFunction = DtdParser.ScanningFunction.SubsetContent;
			return DtdParser.Token.CondSectionEnd;
			IL_281:
			this.ThrowInvalidChar(this.chars, this.charsUsed, this.curPos);
			return DtdParser.Token.None;
		}

		private void ScanName()
		{
			this.ScanQName(false);
		}

		private void ScanQName()
		{
			this.ScanQName(this.SupportNamespaces);
		}

		private void ScanQName(bool isQName)
		{
			this.tokenStartPos = this.curPos;
			int num = -1;
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)this.chars[this.curPos]] & 4) != 0 || this.chars[this.curPos] == ':')
				{
					this.curPos++;
				}
				else if (this.curPos + 1 >= this.charsUsed)
				{
					if (this.ReadDataInName())
					{
						continue;
					}
					this.Throw(this.curPos, "Unexpected end of file while parsing {0} has occurred.", "Name");
				}
				else
				{
					this.Throw(this.curPos, "Name cannot begin with the '{0}' character, hexadecimal value {1}.", XmlException.BuildCharExceptionArgs(this.chars, this.charsUsed, this.curPos));
				}
				for (;;)
				{
					if ((this.xmlCharType.charProperties[(int)this.chars[this.curPos]] & 8) != 0)
					{
						this.curPos++;
					}
					else if (this.chars[this.curPos] == ':')
					{
						if (isQName)
						{
							break;
						}
						this.curPos++;
					}
					else
					{
						if (this.curPos != this.charsUsed)
						{
							goto IL_173;
						}
						if (!this.ReadDataInName())
						{
							goto Block_9;
						}
					}
				}
				if (num != -1)
				{
					this.Throw(this.curPos, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(':', '\0'));
				}
				num = this.curPos - this.tokenStartPos;
				this.curPos++;
			}
			Block_9:
			if (this.tokenStartPos == this.curPos)
			{
				this.Throw(this.curPos, "Unexpected end of file while parsing {0} has occurred.", "Name");
			}
			IL_173:
			this.colonPos = ((num == -1) ? -1 : (this.tokenStartPos + num));
		}

		private bool ReadDataInName()
		{
			int num = this.curPos - this.tokenStartPos;
			this.curPos = this.tokenStartPos;
			bool result = this.ReadData() != 0;
			this.tokenStartPos = this.curPos;
			this.curPos += num;
			return result;
		}

		private void ScanNmtoken()
		{
			this.tokenStartPos = this.curPos;
			int num;
			for (;;)
			{
				if ((this.xmlCharType.charProperties[(int)this.chars[this.curPos]] & 8) != 0 || this.chars[this.curPos] == ':')
				{
					this.curPos++;
				}
				else
				{
					if (this.curPos < this.charsUsed)
					{
						break;
					}
					num = this.curPos - this.tokenStartPos;
					this.curPos = this.tokenStartPos;
					if (this.ReadData() == 0)
					{
						if (num > 0)
						{
							goto Block_5;
						}
						this.Throw(this.curPos, "Unexpected end of file while parsing {0} has occurred.", "NmToken");
					}
					this.tokenStartPos = this.curPos;
					this.curPos += num;
				}
			}
			if (this.curPos - this.tokenStartPos == 0)
			{
				this.Throw(this.curPos, "The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(this.chars, this.charsUsed, this.curPos));
			}
			return;
			Block_5:
			this.tokenStartPos = this.curPos;
			this.curPos += num;
		}

		private bool EatPublicKeyword()
		{
			while (this.charsUsed - this.curPos < 6)
			{
				if (this.ReadData() == 0)
				{
					return false;
				}
			}
			if (this.chars[this.curPos + 1] != 'U' || this.chars[this.curPos + 2] != 'B' || this.chars[this.curPos + 3] != 'L' || this.chars[this.curPos + 4] != 'I' || this.chars[this.curPos + 5] != 'C')
			{
				return false;
			}
			this.curPos += 6;
			return true;
		}

		private bool EatSystemKeyword()
		{
			while (this.charsUsed - this.curPos < 6)
			{
				if (this.ReadData() == 0)
				{
					return false;
				}
			}
			if (this.chars[this.curPos + 1] != 'Y' || this.chars[this.curPos + 2] != 'S' || this.chars[this.curPos + 3] != 'T' || this.chars[this.curPos + 4] != 'E' || this.chars[this.curPos + 5] != 'M')
			{
				return false;
			}
			this.curPos += 6;
			return true;
		}

		private XmlQualifiedName GetNameQualified(bool canHavePrefix)
		{
			if (this.colonPos == -1)
			{
				return new XmlQualifiedName(this.nameTable.Add(this.chars, this.tokenStartPos, this.curPos - this.tokenStartPos));
			}
			if (canHavePrefix)
			{
				return new XmlQualifiedName(this.nameTable.Add(this.chars, this.colonPos + 1, this.curPos - this.colonPos - 1), this.nameTable.Add(this.chars, this.tokenStartPos, this.colonPos - this.tokenStartPos));
			}
			this.Throw(this.tokenStartPos, "'{0}' is an unqualified name and cannot contain the character ':'.", this.GetNameString());
			return null;
		}

		private string GetNameString()
		{
			return new string(this.chars, this.tokenStartPos, this.curPos - this.tokenStartPos);
		}

		private string GetNmtokenString()
		{
			return this.GetNameString();
		}

		private string GetValue()
		{
			if (this.stringBuilder.Length == 0)
			{
				return new string(this.chars, this.tokenStartPos, this.curPos - this.tokenStartPos - 1);
			}
			return this.stringBuilder.ToString();
		}

		private string GetValueWithStrippedSpaces()
		{
			return DtdParser.StripSpaces((this.stringBuilder.Length == 0) ? new string(this.chars, this.tokenStartPos, this.curPos - this.tokenStartPos - 1) : this.stringBuilder.ToString());
		}

		private int ReadData()
		{
			this.SaveParsingBuffer();
			int result = this.readerAdapter.ReadData();
			this.LoadParsingBuffer();
			return result;
		}

		private void LoadParsingBuffer()
		{
			this.chars = this.readerAdapter.ParsingBuffer;
			this.charsUsed = this.readerAdapter.ParsingBufferLength;
			this.curPos = this.readerAdapter.CurrentPosition;
		}

		private void SaveParsingBuffer()
		{
			this.SaveParsingBuffer(this.curPos);
		}

		private void SaveParsingBuffer(int internalSubsetValueEndPos)
		{
			if (this.SaveInternalSubsetValue)
			{
				int currentPosition = this.readerAdapter.CurrentPosition;
				if (internalSubsetValueEndPos - currentPosition > 0)
				{
					this.internalSubsetValueSb.Append(this.chars, currentPosition, internalSubsetValueEndPos - currentPosition);
				}
			}
			this.readerAdapter.CurrentPosition = this.curPos;
		}

		private bool HandleEntityReference(bool paramEntity, bool inLiteral, bool inAttribute)
		{
			this.curPos++;
			return this.HandleEntityReference(this.ScanEntityName(), paramEntity, inLiteral, inAttribute);
		}

		private bool HandleEntityReference(XmlQualifiedName entityName, bool paramEntity, bool inLiteral, bool inAttribute)
		{
			this.SaveParsingBuffer();
			if (paramEntity && this.ParsingInternalSubset && !this.ParsingTopLevelMarkup)
			{
				this.Throw(this.curPos - entityName.Name.Length - 1, "A parameter entity reference is not allowed in internal markup.");
			}
			SchemaEntity schemaEntity = this.VerifyEntityReference(entityName, paramEntity, true, inAttribute);
			if (schemaEntity == null)
			{
				return false;
			}
			if (schemaEntity.ParsingInProgress)
			{
				this.Throw(this.curPos - entityName.Name.Length - 1, paramEntity ? "Parameter entity '{0}' references itself." : "General entity '{0}' references itself.", entityName.Name);
			}
			int num;
			if (schemaEntity.IsExternal)
			{
				if (!this.readerAdapter.PushEntity(schemaEntity, out num))
				{
					return false;
				}
				this.externalEntitiesDepth++;
			}
			else
			{
				if (schemaEntity.Text.Length == 0)
				{
					return false;
				}
				if (!this.readerAdapter.PushEntity(schemaEntity, out num))
				{
					return false;
				}
			}
			this.currentEntityId = num;
			if (paramEntity && !inLiteral && this.scanningFunction != DtdParser.ScanningFunction.ParamEntitySpace)
			{
				this.savedScanningFunction = this.scanningFunction;
				this.scanningFunction = DtdParser.ScanningFunction.ParamEntitySpace;
			}
			this.LoadParsingBuffer();
			return true;
		}

		private bool HandleEntityEnd(bool inLiteral)
		{
			this.SaveParsingBuffer();
			IDtdEntityInfo dtdEntityInfo;
			if (!this.readerAdapter.PopEntity(out dtdEntityInfo, out this.currentEntityId))
			{
				return false;
			}
			this.LoadParsingBuffer();
			if (dtdEntityInfo == null)
			{
				if (this.scanningFunction == DtdParser.ScanningFunction.ParamEntitySpace)
				{
					this.scanningFunction = this.savedScanningFunction;
				}
				return false;
			}
			if (dtdEntityInfo.IsExternal)
			{
				this.externalEntitiesDepth--;
			}
			if (!inLiteral && this.scanningFunction != DtdParser.ScanningFunction.ParamEntitySpace)
			{
				this.savedScanningFunction = this.scanningFunction;
				this.scanningFunction = DtdParser.ScanningFunction.ParamEntitySpace;
			}
			return true;
		}

		private SchemaEntity VerifyEntityReference(XmlQualifiedName entityName, bool paramEntity, bool mustBeDeclared, bool inAttribute)
		{
			SchemaEntity schemaEntity;
			if (paramEntity)
			{
				this.schemaInfo.ParameterEntities.TryGetValue(entityName, out schemaEntity);
			}
			else
			{
				this.schemaInfo.GeneralEntities.TryGetValue(entityName, out schemaEntity);
			}
			if (schemaEntity == null)
			{
				if (paramEntity)
				{
					if (this.validate)
					{
						this.SendValidationEvent(this.curPos - entityName.Name.Length - 1, XmlSeverityType.Error, "Reference to undeclared parameter entity '{0}'.", entityName.Name);
					}
				}
				else if (mustBeDeclared)
				{
					if (!this.ParsingInternalSubset)
					{
						if (this.validate)
						{
							this.SendValidationEvent(this.curPos - entityName.Name.Length - 1, XmlSeverityType.Error, "Reference to undeclared entity '{0}'.", entityName.Name);
						}
					}
					else
					{
						this.Throw(this.curPos - entityName.Name.Length - 1, "Reference to undeclared entity '{0}'.", entityName.Name);
					}
				}
				return null;
			}
			if (!schemaEntity.NData.IsEmpty)
			{
				this.Throw(this.curPos - entityName.Name.Length - 1, "Reference to unparsed entity '{0}'.", entityName.Name);
			}
			if (inAttribute && schemaEntity.IsExternal)
			{
				this.Throw(this.curPos - entityName.Name.Length - 1, "External entity '{0}' reference cannot appear in the attribute value.", entityName.Name);
			}
			return schemaEntity;
		}

		private void SendValidationEvent(int pos, XmlSeverityType severity, string code, string arg)
		{
			this.SendValidationEvent(severity, new XmlSchemaException(code, arg, this.BaseUriStr, this.LineNo, this.LinePos + (pos - this.curPos)));
		}

		private void SendValidationEvent(XmlSeverityType severity, string code, string arg)
		{
			this.SendValidationEvent(severity, new XmlSchemaException(code, arg, this.BaseUriStr, this.LineNo, this.LinePos));
		}

		private void SendValidationEvent(XmlSeverityType severity, XmlSchemaException e)
		{
			IValidationEventHandling validationEventHandling = this.readerAdapterWithValidation.ValidationEventHandling;
			if (validationEventHandling != null)
			{
				validationEventHandling.SendEvent(e, severity);
			}
		}

		private bool IsAttributeValueType(DtdParser.Token token)
		{
			return token >= DtdParser.Token.CDATA && token <= DtdParser.Token.NOTATION;
		}

		private int LineNo
		{
			get
			{
				return this.readerAdapter.LineNo;
			}
		}

		private int LinePos
		{
			get
			{
				return this.curPos - this.readerAdapter.LineStartPosition;
			}
		}

		private string BaseUriStr
		{
			get
			{
				Uri baseUri = this.readerAdapter.BaseUri;
				if (!(baseUri != null))
				{
					return string.Empty;
				}
				return baseUri.ToString();
			}
		}

		private void OnUnexpectedError()
		{
			this.Throw(this.curPos, "An internal error has occurred.");
		}

		private void Throw(int curPos, string res)
		{
			this.Throw(curPos, res, string.Empty);
		}

		private void Throw(int curPos, string res, string arg)
		{
			this.curPos = curPos;
			Uri baseUri = this.readerAdapter.BaseUri;
			this.readerAdapter.Throw(new XmlException(res, arg, this.LineNo, this.LinePos, (baseUri == null) ? null : baseUri.ToString()));
		}

		private void Throw(int curPos, string res, string[] args)
		{
			this.curPos = curPos;
			Uri baseUri = this.readerAdapter.BaseUri;
			this.readerAdapter.Throw(new XmlException(res, args, this.LineNo, this.LinePos, (baseUri == null) ? null : baseUri.ToString()));
		}

		private void Throw(string res, string arg, int lineNo, int linePos)
		{
			Uri baseUri = this.readerAdapter.BaseUri;
			this.readerAdapter.Throw(new XmlException(res, arg, lineNo, linePos, (baseUri == null) ? null : baseUri.ToString()));
		}

		private void ThrowInvalidChar(int pos, string data, int invCharPos)
		{
			this.Throw(pos, "'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(data, invCharPos));
		}

		private void ThrowInvalidChar(char[] data, int length, int invCharPos)
		{
			this.Throw(invCharPos, "'{0}', hexadecimal value {1}, is an invalid character.", XmlException.BuildCharExceptionArgs(data, length, invCharPos));
		}

		private void ThrowUnexpectedToken(int pos, string expectedToken)
		{
			this.ThrowUnexpectedToken(pos, expectedToken, null);
		}

		private void ThrowUnexpectedToken(int pos, string expectedToken1, string expectedToken2)
		{
			string text = this.ParseUnexpectedToken(pos);
			if (expectedToken2 != null)
			{
				this.Throw(this.curPos, "'{0}' is an unexpected token. The expected token is '{1}' or '{2}'.", new string[]
				{
					text,
					expectedToken1,
					expectedToken2
				});
				return;
			}
			this.Throw(this.curPos, "'{0}' is an unexpected token. The expected token is '{1}'.", new string[]
			{
				text,
				expectedToken1
			});
		}

		private string ParseUnexpectedToken(int startPos)
		{
			if (this.xmlCharType.IsNCNameSingleChar(this.chars[startPos]))
			{
				int num = startPos;
				while (this.xmlCharType.IsNCNameSingleChar(this.chars[num]))
				{
					num++;
				}
				int num2 = num - startPos;
				return new string(this.chars, startPos, (num2 > 0) ? num2 : 1);
			}
			return new string(this.chars, startPos, 1);
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

		Task<IDtdInfo> IDtdParser.ParseInternalDtdAsync(IDtdParserAdapter adapter, bool saveInternalSubset)
		{
			DtdParser.<System-Xml-IDtdParser-ParseInternalDtdAsync>d__153 <System-Xml-IDtdParser-ParseInternalDtdAsync>d__;
			<System-Xml-IDtdParser-ParseInternalDtdAsync>d__.<>4__this = this;
			<System-Xml-IDtdParser-ParseInternalDtdAsync>d__.adapter = adapter;
			<System-Xml-IDtdParser-ParseInternalDtdAsync>d__.saveInternalSubset = saveInternalSubset;
			<System-Xml-IDtdParser-ParseInternalDtdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IDtdInfo>.Create();
			<System-Xml-IDtdParser-ParseInternalDtdAsync>d__.<>1__state = -1;
			<System-Xml-IDtdParser-ParseInternalDtdAsync>d__.<>t__builder.Start<DtdParser.<System-Xml-IDtdParser-ParseInternalDtdAsync>d__153>(ref <System-Xml-IDtdParser-ParseInternalDtdAsync>d__);
			return <System-Xml-IDtdParser-ParseInternalDtdAsync>d__.<>t__builder.Task;
		}

		Task<IDtdInfo> IDtdParser.ParseFreeFloatingDtdAsync(string baseUri, string docTypeName, string publicId, string systemId, string internalSubset, IDtdParserAdapter adapter)
		{
			DtdParser.<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__154 <System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.<>4__this = this;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.baseUri = baseUri;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.docTypeName = docTypeName;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.publicId = publicId;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.systemId = systemId;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.internalSubset = internalSubset;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.adapter = adapter;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<IDtdInfo>.Create();
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.<>1__state = -1;
			<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.<>t__builder.Start<DtdParser.<System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__154>(ref <System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__);
			return <System-Xml-IDtdParser-ParseFreeFloatingDtdAsync>d__.<>t__builder.Task;
		}

		private Task ParseAsync(bool saveInternalSubset)
		{
			DtdParser.<ParseAsync>d__155 <ParseAsync>d__;
			<ParseAsync>d__.<>4__this = this;
			<ParseAsync>d__.saveInternalSubset = saveInternalSubset;
			<ParseAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseAsync>d__.<>1__state = -1;
			<ParseAsync>d__.<>t__builder.Start<DtdParser.<ParseAsync>d__155>(ref <ParseAsync>d__);
			return <ParseAsync>d__.<>t__builder.Task;
		}

		private Task ParseInDocumentDtdAsync(bool saveInternalSubset)
		{
			DtdParser.<ParseInDocumentDtdAsync>d__156 <ParseInDocumentDtdAsync>d__;
			<ParseInDocumentDtdAsync>d__.<>4__this = this;
			<ParseInDocumentDtdAsync>d__.saveInternalSubset = saveInternalSubset;
			<ParseInDocumentDtdAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseInDocumentDtdAsync>d__.<>1__state = -1;
			<ParseInDocumentDtdAsync>d__.<>t__builder.Start<DtdParser.<ParseInDocumentDtdAsync>d__156>(ref <ParseInDocumentDtdAsync>d__);
			return <ParseInDocumentDtdAsync>d__.<>t__builder.Task;
		}

		private Task ParseFreeFloatingDtdAsync()
		{
			DtdParser.<ParseFreeFloatingDtdAsync>d__157 <ParseFreeFloatingDtdAsync>d__;
			<ParseFreeFloatingDtdAsync>d__.<>4__this = this;
			<ParseFreeFloatingDtdAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseFreeFloatingDtdAsync>d__.<>1__state = -1;
			<ParseFreeFloatingDtdAsync>d__.<>t__builder.Start<DtdParser.<ParseFreeFloatingDtdAsync>d__157>(ref <ParseFreeFloatingDtdAsync>d__);
			return <ParseFreeFloatingDtdAsync>d__.<>t__builder.Task;
		}

		private Task ParseInternalSubsetAsync()
		{
			return this.ParseSubsetAsync();
		}

		private Task ParseExternalSubsetAsync()
		{
			DtdParser.<ParseExternalSubsetAsync>d__159 <ParseExternalSubsetAsync>d__;
			<ParseExternalSubsetAsync>d__.<>4__this = this;
			<ParseExternalSubsetAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseExternalSubsetAsync>d__.<>1__state = -1;
			<ParseExternalSubsetAsync>d__.<>t__builder.Start<DtdParser.<ParseExternalSubsetAsync>d__159>(ref <ParseExternalSubsetAsync>d__);
			return <ParseExternalSubsetAsync>d__.<>t__builder.Task;
		}

		private Task ParseSubsetAsync()
		{
			DtdParser.<ParseSubsetAsync>d__160 <ParseSubsetAsync>d__;
			<ParseSubsetAsync>d__.<>4__this = this;
			<ParseSubsetAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseSubsetAsync>d__.<>1__state = -1;
			<ParseSubsetAsync>d__.<>t__builder.Start<DtdParser.<ParseSubsetAsync>d__160>(ref <ParseSubsetAsync>d__);
			return <ParseSubsetAsync>d__.<>t__builder.Task;
		}

		private Task ParseAttlistDeclAsync()
		{
			DtdParser.<ParseAttlistDeclAsync>d__161 <ParseAttlistDeclAsync>d__;
			<ParseAttlistDeclAsync>d__.<>4__this = this;
			<ParseAttlistDeclAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseAttlistDeclAsync>d__.<>1__state = -1;
			<ParseAttlistDeclAsync>d__.<>t__builder.Start<DtdParser.<ParseAttlistDeclAsync>d__161>(ref <ParseAttlistDeclAsync>d__);
			return <ParseAttlistDeclAsync>d__.<>t__builder.Task;
		}

		private Task ParseAttlistTypeAsync(SchemaAttDef attrDef, SchemaElementDecl elementDecl, bool ignoreErrors)
		{
			DtdParser.<ParseAttlistTypeAsync>d__162 <ParseAttlistTypeAsync>d__;
			<ParseAttlistTypeAsync>d__.<>4__this = this;
			<ParseAttlistTypeAsync>d__.attrDef = attrDef;
			<ParseAttlistTypeAsync>d__.elementDecl = elementDecl;
			<ParseAttlistTypeAsync>d__.ignoreErrors = ignoreErrors;
			<ParseAttlistTypeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseAttlistTypeAsync>d__.<>1__state = -1;
			<ParseAttlistTypeAsync>d__.<>t__builder.Start<DtdParser.<ParseAttlistTypeAsync>d__162>(ref <ParseAttlistTypeAsync>d__);
			return <ParseAttlistTypeAsync>d__.<>t__builder.Task;
		}

		private Task ParseAttlistDefaultAsync(SchemaAttDef attrDef, bool ignoreErrors)
		{
			DtdParser.<ParseAttlistDefaultAsync>d__163 <ParseAttlistDefaultAsync>d__;
			<ParseAttlistDefaultAsync>d__.<>4__this = this;
			<ParseAttlistDefaultAsync>d__.attrDef = attrDef;
			<ParseAttlistDefaultAsync>d__.ignoreErrors = ignoreErrors;
			<ParseAttlistDefaultAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseAttlistDefaultAsync>d__.<>1__state = -1;
			<ParseAttlistDefaultAsync>d__.<>t__builder.Start<DtdParser.<ParseAttlistDefaultAsync>d__163>(ref <ParseAttlistDefaultAsync>d__);
			return <ParseAttlistDefaultAsync>d__.<>t__builder.Task;
		}

		private Task ParseElementDeclAsync()
		{
			DtdParser.<ParseElementDeclAsync>d__164 <ParseElementDeclAsync>d__;
			<ParseElementDeclAsync>d__.<>4__this = this;
			<ParseElementDeclAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseElementDeclAsync>d__.<>1__state = -1;
			<ParseElementDeclAsync>d__.<>t__builder.Start<DtdParser.<ParseElementDeclAsync>d__164>(ref <ParseElementDeclAsync>d__);
			return <ParseElementDeclAsync>d__.<>t__builder.Task;
		}

		private Task ParseElementOnlyContentAsync(ParticleContentValidator pcv, int startParenEntityId)
		{
			DtdParser.<ParseElementOnlyContentAsync>d__165 <ParseElementOnlyContentAsync>d__;
			<ParseElementOnlyContentAsync>d__.<>4__this = this;
			<ParseElementOnlyContentAsync>d__.pcv = pcv;
			<ParseElementOnlyContentAsync>d__.startParenEntityId = startParenEntityId;
			<ParseElementOnlyContentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseElementOnlyContentAsync>d__.<>1__state = -1;
			<ParseElementOnlyContentAsync>d__.<>t__builder.Start<DtdParser.<ParseElementOnlyContentAsync>d__165>(ref <ParseElementOnlyContentAsync>d__);
			return <ParseElementOnlyContentAsync>d__.<>t__builder.Task;
		}

		private Task ParseHowManyAsync(ParticleContentValidator pcv)
		{
			DtdParser.<ParseHowManyAsync>d__166 <ParseHowManyAsync>d__;
			<ParseHowManyAsync>d__.<>4__this = this;
			<ParseHowManyAsync>d__.pcv = pcv;
			<ParseHowManyAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseHowManyAsync>d__.<>1__state = -1;
			<ParseHowManyAsync>d__.<>t__builder.Start<DtdParser.<ParseHowManyAsync>d__166>(ref <ParseHowManyAsync>d__);
			return <ParseHowManyAsync>d__.<>t__builder.Task;
		}

		private Task ParseElementMixedContentAsync(ParticleContentValidator pcv, int startParenEntityId)
		{
			DtdParser.<ParseElementMixedContentAsync>d__167 <ParseElementMixedContentAsync>d__;
			<ParseElementMixedContentAsync>d__.<>4__this = this;
			<ParseElementMixedContentAsync>d__.pcv = pcv;
			<ParseElementMixedContentAsync>d__.startParenEntityId = startParenEntityId;
			<ParseElementMixedContentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseElementMixedContentAsync>d__.<>1__state = -1;
			<ParseElementMixedContentAsync>d__.<>t__builder.Start<DtdParser.<ParseElementMixedContentAsync>d__167>(ref <ParseElementMixedContentAsync>d__);
			return <ParseElementMixedContentAsync>d__.<>t__builder.Task;
		}

		private Task ParseEntityDeclAsync()
		{
			DtdParser.<ParseEntityDeclAsync>d__168 <ParseEntityDeclAsync>d__;
			<ParseEntityDeclAsync>d__.<>4__this = this;
			<ParseEntityDeclAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseEntityDeclAsync>d__.<>1__state = -1;
			<ParseEntityDeclAsync>d__.<>t__builder.Start<DtdParser.<ParseEntityDeclAsync>d__168>(ref <ParseEntityDeclAsync>d__);
			return <ParseEntityDeclAsync>d__.<>t__builder.Task;
		}

		private Task ParseNotationDeclAsync()
		{
			DtdParser.<ParseNotationDeclAsync>d__169 <ParseNotationDeclAsync>d__;
			<ParseNotationDeclAsync>d__.<>4__this = this;
			<ParseNotationDeclAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseNotationDeclAsync>d__.<>1__state = -1;
			<ParseNotationDeclAsync>d__.<>t__builder.Start<DtdParser.<ParseNotationDeclAsync>d__169>(ref <ParseNotationDeclAsync>d__);
			return <ParseNotationDeclAsync>d__.<>t__builder.Task;
		}

		private Task ParseCommentAsync()
		{
			DtdParser.<ParseCommentAsync>d__170 <ParseCommentAsync>d__;
			<ParseCommentAsync>d__.<>4__this = this;
			<ParseCommentAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseCommentAsync>d__.<>1__state = -1;
			<ParseCommentAsync>d__.<>t__builder.Start<DtdParser.<ParseCommentAsync>d__170>(ref <ParseCommentAsync>d__);
			return <ParseCommentAsync>d__.<>t__builder.Task;
		}

		private Task ParsePIAsync()
		{
			DtdParser.<ParsePIAsync>d__171 <ParsePIAsync>d__;
			<ParsePIAsync>d__.<>4__this = this;
			<ParsePIAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParsePIAsync>d__.<>1__state = -1;
			<ParsePIAsync>d__.<>t__builder.Start<DtdParser.<ParsePIAsync>d__171>(ref <ParsePIAsync>d__);
			return <ParsePIAsync>d__.<>t__builder.Task;
		}

		private Task ParseCondSectionAsync()
		{
			DtdParser.<ParseCondSectionAsync>d__172 <ParseCondSectionAsync>d__;
			<ParseCondSectionAsync>d__.<>4__this = this;
			<ParseCondSectionAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ParseCondSectionAsync>d__.<>1__state = -1;
			<ParseCondSectionAsync>d__.<>t__builder.Start<DtdParser.<ParseCondSectionAsync>d__172>(ref <ParseCondSectionAsync>d__);
			return <ParseCondSectionAsync>d__.<>t__builder.Task;
		}

		private Task<Tuple<string, string>> ParseExternalIdAsync(DtdParser.Token idTokenType, DtdParser.Token declType)
		{
			DtdParser.<ParseExternalIdAsync>d__173 <ParseExternalIdAsync>d__;
			<ParseExternalIdAsync>d__.<>4__this = this;
			<ParseExternalIdAsync>d__.idTokenType = idTokenType;
			<ParseExternalIdAsync>d__.declType = declType;
			<ParseExternalIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<string, string>>.Create();
			<ParseExternalIdAsync>d__.<>1__state = -1;
			<ParseExternalIdAsync>d__.<>t__builder.Start<DtdParser.<ParseExternalIdAsync>d__173>(ref <ParseExternalIdAsync>d__);
			return <ParseExternalIdAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> GetTokenAsync(bool needWhiteSpace)
		{
			DtdParser.<GetTokenAsync>d__174 <GetTokenAsync>d__;
			<GetTokenAsync>d__.<>4__this = this;
			<GetTokenAsync>d__.needWhiteSpace = needWhiteSpace;
			<GetTokenAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<GetTokenAsync>d__.<>1__state = -1;
			<GetTokenAsync>d__.<>t__builder.Start<DtdParser.<GetTokenAsync>d__174>(ref <GetTokenAsync>d__);
			return <GetTokenAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanSubsetContentAsync()
		{
			DtdParser.<ScanSubsetContentAsync>d__175 <ScanSubsetContentAsync>d__;
			<ScanSubsetContentAsync>d__.<>4__this = this;
			<ScanSubsetContentAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanSubsetContentAsync>d__.<>1__state = -1;
			<ScanSubsetContentAsync>d__.<>t__builder.Start<DtdParser.<ScanSubsetContentAsync>d__175>(ref <ScanSubsetContentAsync>d__);
			return <ScanSubsetContentAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanNameExpectedAsync()
		{
			DtdParser.<ScanNameExpectedAsync>d__176 <ScanNameExpectedAsync>d__;
			<ScanNameExpectedAsync>d__.<>4__this = this;
			<ScanNameExpectedAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanNameExpectedAsync>d__.<>1__state = -1;
			<ScanNameExpectedAsync>d__.<>t__builder.Start<DtdParser.<ScanNameExpectedAsync>d__176>(ref <ScanNameExpectedAsync>d__);
			return <ScanNameExpectedAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanQNameExpectedAsync()
		{
			DtdParser.<ScanQNameExpectedAsync>d__177 <ScanQNameExpectedAsync>d__;
			<ScanQNameExpectedAsync>d__.<>4__this = this;
			<ScanQNameExpectedAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanQNameExpectedAsync>d__.<>1__state = -1;
			<ScanQNameExpectedAsync>d__.<>t__builder.Start<DtdParser.<ScanQNameExpectedAsync>d__177>(ref <ScanQNameExpectedAsync>d__);
			return <ScanQNameExpectedAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanNmtokenExpectedAsync()
		{
			DtdParser.<ScanNmtokenExpectedAsync>d__178 <ScanNmtokenExpectedAsync>d__;
			<ScanNmtokenExpectedAsync>d__.<>4__this = this;
			<ScanNmtokenExpectedAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanNmtokenExpectedAsync>d__.<>1__state = -1;
			<ScanNmtokenExpectedAsync>d__.<>t__builder.Start<DtdParser.<ScanNmtokenExpectedAsync>d__178>(ref <ScanNmtokenExpectedAsync>d__);
			return <ScanNmtokenExpectedAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanDoctype1Async()
		{
			DtdParser.<ScanDoctype1Async>d__179 <ScanDoctype1Async>d__;
			<ScanDoctype1Async>d__.<>4__this = this;
			<ScanDoctype1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanDoctype1Async>d__.<>1__state = -1;
			<ScanDoctype1Async>d__.<>t__builder.Start<DtdParser.<ScanDoctype1Async>d__179>(ref <ScanDoctype1Async>d__);
			return <ScanDoctype1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanElement1Async()
		{
			DtdParser.<ScanElement1Async>d__180 <ScanElement1Async>d__;
			<ScanElement1Async>d__.<>4__this = this;
			<ScanElement1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanElement1Async>d__.<>1__state = -1;
			<ScanElement1Async>d__.<>t__builder.Start<DtdParser.<ScanElement1Async>d__180>(ref <ScanElement1Async>d__);
			return <ScanElement1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanElement2Async()
		{
			DtdParser.<ScanElement2Async>d__181 <ScanElement2Async>d__;
			<ScanElement2Async>d__.<>4__this = this;
			<ScanElement2Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanElement2Async>d__.<>1__state = -1;
			<ScanElement2Async>d__.<>t__builder.Start<DtdParser.<ScanElement2Async>d__181>(ref <ScanElement2Async>d__);
			return <ScanElement2Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanElement3Async()
		{
			DtdParser.<ScanElement3Async>d__182 <ScanElement3Async>d__;
			<ScanElement3Async>d__.<>4__this = this;
			<ScanElement3Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanElement3Async>d__.<>1__state = -1;
			<ScanElement3Async>d__.<>t__builder.Start<DtdParser.<ScanElement3Async>d__182>(ref <ScanElement3Async>d__);
			return <ScanElement3Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanAttlist1Async()
		{
			DtdParser.<ScanAttlist1Async>d__183 <ScanAttlist1Async>d__;
			<ScanAttlist1Async>d__.<>4__this = this;
			<ScanAttlist1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanAttlist1Async>d__.<>1__state = -1;
			<ScanAttlist1Async>d__.<>t__builder.Start<DtdParser.<ScanAttlist1Async>d__183>(ref <ScanAttlist1Async>d__);
			return <ScanAttlist1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanAttlist2Async()
		{
			DtdParser.<ScanAttlist2Async>d__184 <ScanAttlist2Async>d__;
			<ScanAttlist2Async>d__.<>4__this = this;
			<ScanAttlist2Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanAttlist2Async>d__.<>1__state = -1;
			<ScanAttlist2Async>d__.<>t__builder.Start<DtdParser.<ScanAttlist2Async>d__184>(ref <ScanAttlist2Async>d__);
			return <ScanAttlist2Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanAttlist6Async()
		{
			DtdParser.<ScanAttlist6Async>d__185 <ScanAttlist6Async>d__;
			<ScanAttlist6Async>d__.<>4__this = this;
			<ScanAttlist6Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanAttlist6Async>d__.<>1__state = -1;
			<ScanAttlist6Async>d__.<>t__builder.Start<DtdParser.<ScanAttlist6Async>d__185>(ref <ScanAttlist6Async>d__);
			return <ScanAttlist6Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanLiteralAsync(DtdParser.LiteralType literalType)
		{
			DtdParser.<ScanLiteralAsync>d__186 <ScanLiteralAsync>d__;
			<ScanLiteralAsync>d__.<>4__this = this;
			<ScanLiteralAsync>d__.literalType = literalType;
			<ScanLiteralAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanLiteralAsync>d__.<>1__state = -1;
			<ScanLiteralAsync>d__.<>t__builder.Start<DtdParser.<ScanLiteralAsync>d__186>(ref <ScanLiteralAsync>d__);
			return <ScanLiteralAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanNotation1Async()
		{
			DtdParser.<ScanNotation1Async>d__187 <ScanNotation1Async>d__;
			<ScanNotation1Async>d__.<>4__this = this;
			<ScanNotation1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanNotation1Async>d__.<>1__state = -1;
			<ScanNotation1Async>d__.<>t__builder.Start<DtdParser.<ScanNotation1Async>d__187>(ref <ScanNotation1Async>d__);
			return <ScanNotation1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanSystemIdAsync()
		{
			DtdParser.<ScanSystemIdAsync>d__188 <ScanSystemIdAsync>d__;
			<ScanSystemIdAsync>d__.<>4__this = this;
			<ScanSystemIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanSystemIdAsync>d__.<>1__state = -1;
			<ScanSystemIdAsync>d__.<>t__builder.Start<DtdParser.<ScanSystemIdAsync>d__188>(ref <ScanSystemIdAsync>d__);
			return <ScanSystemIdAsync>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanEntity1Async()
		{
			DtdParser.<ScanEntity1Async>d__189 <ScanEntity1Async>d__;
			<ScanEntity1Async>d__.<>4__this = this;
			<ScanEntity1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanEntity1Async>d__.<>1__state = -1;
			<ScanEntity1Async>d__.<>t__builder.Start<DtdParser.<ScanEntity1Async>d__189>(ref <ScanEntity1Async>d__);
			return <ScanEntity1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanEntity2Async()
		{
			DtdParser.<ScanEntity2Async>d__190 <ScanEntity2Async>d__;
			<ScanEntity2Async>d__.<>4__this = this;
			<ScanEntity2Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanEntity2Async>d__.<>1__state = -1;
			<ScanEntity2Async>d__.<>t__builder.Start<DtdParser.<ScanEntity2Async>d__190>(ref <ScanEntity2Async>d__);
			return <ScanEntity2Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanEntity3Async()
		{
			DtdParser.<ScanEntity3Async>d__191 <ScanEntity3Async>d__;
			<ScanEntity3Async>d__.<>4__this = this;
			<ScanEntity3Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanEntity3Async>d__.<>1__state = -1;
			<ScanEntity3Async>d__.<>t__builder.Start<DtdParser.<ScanEntity3Async>d__191>(ref <ScanEntity3Async>d__);
			return <ScanEntity3Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanPublicId1Async()
		{
			DtdParser.<ScanPublicId1Async>d__192 <ScanPublicId1Async>d__;
			<ScanPublicId1Async>d__.<>4__this = this;
			<ScanPublicId1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanPublicId1Async>d__.<>1__state = -1;
			<ScanPublicId1Async>d__.<>t__builder.Start<DtdParser.<ScanPublicId1Async>d__192>(ref <ScanPublicId1Async>d__);
			return <ScanPublicId1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanPublicId2Async()
		{
			DtdParser.<ScanPublicId2Async>d__193 <ScanPublicId2Async>d__;
			<ScanPublicId2Async>d__.<>4__this = this;
			<ScanPublicId2Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanPublicId2Async>d__.<>1__state = -1;
			<ScanPublicId2Async>d__.<>t__builder.Start<DtdParser.<ScanPublicId2Async>d__193>(ref <ScanPublicId2Async>d__);
			return <ScanPublicId2Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanCondSection1Async()
		{
			DtdParser.<ScanCondSection1Async>d__194 <ScanCondSection1Async>d__;
			<ScanCondSection1Async>d__.<>4__this = this;
			<ScanCondSection1Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanCondSection1Async>d__.<>1__state = -1;
			<ScanCondSection1Async>d__.<>t__builder.Start<DtdParser.<ScanCondSection1Async>d__194>(ref <ScanCondSection1Async>d__);
			return <ScanCondSection1Async>d__.<>t__builder.Task;
		}

		private Task<DtdParser.Token> ScanCondSection3Async()
		{
			DtdParser.<ScanCondSection3Async>d__195 <ScanCondSection3Async>d__;
			<ScanCondSection3Async>d__.<>4__this = this;
			<ScanCondSection3Async>d__.<>t__builder = AsyncTaskMethodBuilder<DtdParser.Token>.Create();
			<ScanCondSection3Async>d__.<>1__state = -1;
			<ScanCondSection3Async>d__.<>t__builder.Start<DtdParser.<ScanCondSection3Async>d__195>(ref <ScanCondSection3Async>d__);
			return <ScanCondSection3Async>d__.<>t__builder.Task;
		}

		private Task ScanNameAsync()
		{
			return this.ScanQNameAsync(false);
		}

		private Task ScanQNameAsync()
		{
			return this.ScanQNameAsync(this.SupportNamespaces);
		}

		private Task ScanQNameAsync(bool isQName)
		{
			DtdParser.<ScanQNameAsync>d__198 <ScanQNameAsync>d__;
			<ScanQNameAsync>d__.<>4__this = this;
			<ScanQNameAsync>d__.isQName = isQName;
			<ScanQNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ScanQNameAsync>d__.<>1__state = -1;
			<ScanQNameAsync>d__.<>t__builder.Start<DtdParser.<ScanQNameAsync>d__198>(ref <ScanQNameAsync>d__);
			return <ScanQNameAsync>d__.<>t__builder.Task;
		}

		private Task<bool> ReadDataInNameAsync()
		{
			DtdParser.<ReadDataInNameAsync>d__199 <ReadDataInNameAsync>d__;
			<ReadDataInNameAsync>d__.<>4__this = this;
			<ReadDataInNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ReadDataInNameAsync>d__.<>1__state = -1;
			<ReadDataInNameAsync>d__.<>t__builder.Start<DtdParser.<ReadDataInNameAsync>d__199>(ref <ReadDataInNameAsync>d__);
			return <ReadDataInNameAsync>d__.<>t__builder.Task;
		}

		private Task ScanNmtokenAsync()
		{
			DtdParser.<ScanNmtokenAsync>d__200 <ScanNmtokenAsync>d__;
			<ScanNmtokenAsync>d__.<>4__this = this;
			<ScanNmtokenAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ScanNmtokenAsync>d__.<>1__state = -1;
			<ScanNmtokenAsync>d__.<>t__builder.Start<DtdParser.<ScanNmtokenAsync>d__200>(ref <ScanNmtokenAsync>d__);
			return <ScanNmtokenAsync>d__.<>t__builder.Task;
		}

		private Task<bool> EatPublicKeywordAsync()
		{
			DtdParser.<EatPublicKeywordAsync>d__201 <EatPublicKeywordAsync>d__;
			<EatPublicKeywordAsync>d__.<>4__this = this;
			<EatPublicKeywordAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<EatPublicKeywordAsync>d__.<>1__state = -1;
			<EatPublicKeywordAsync>d__.<>t__builder.Start<DtdParser.<EatPublicKeywordAsync>d__201>(ref <EatPublicKeywordAsync>d__);
			return <EatPublicKeywordAsync>d__.<>t__builder.Task;
		}

		private Task<bool> EatSystemKeywordAsync()
		{
			DtdParser.<EatSystemKeywordAsync>d__202 <EatSystemKeywordAsync>d__;
			<EatSystemKeywordAsync>d__.<>4__this = this;
			<EatSystemKeywordAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<EatSystemKeywordAsync>d__.<>1__state = -1;
			<EatSystemKeywordAsync>d__.<>t__builder.Start<DtdParser.<EatSystemKeywordAsync>d__202>(ref <EatSystemKeywordAsync>d__);
			return <EatSystemKeywordAsync>d__.<>t__builder.Task;
		}

		private Task<int> ReadDataAsync()
		{
			DtdParser.<ReadDataAsync>d__203 <ReadDataAsync>d__;
			<ReadDataAsync>d__.<>4__this = this;
			<ReadDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<ReadDataAsync>d__.<>1__state = -1;
			<ReadDataAsync>d__.<>t__builder.Start<DtdParser.<ReadDataAsync>d__203>(ref <ReadDataAsync>d__);
			return <ReadDataAsync>d__.<>t__builder.Task;
		}

		private Task<bool> HandleEntityReferenceAsync(bool paramEntity, bool inLiteral, bool inAttribute)
		{
			this.curPos++;
			return this.HandleEntityReferenceAsync(this.ScanEntityName(), paramEntity, inLiteral, inAttribute);
		}

		private Task<bool> HandleEntityReferenceAsync(XmlQualifiedName entityName, bool paramEntity, bool inLiteral, bool inAttribute)
		{
			DtdParser.<HandleEntityReferenceAsync>d__205 <HandleEntityReferenceAsync>d__;
			<HandleEntityReferenceAsync>d__.<>4__this = this;
			<HandleEntityReferenceAsync>d__.entityName = entityName;
			<HandleEntityReferenceAsync>d__.paramEntity = paramEntity;
			<HandleEntityReferenceAsync>d__.inLiteral = inLiteral;
			<HandleEntityReferenceAsync>d__.inAttribute = inAttribute;
			<HandleEntityReferenceAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<HandleEntityReferenceAsync>d__.<>1__state = -1;
			<HandleEntityReferenceAsync>d__.<>t__builder.Start<DtdParser.<HandleEntityReferenceAsync>d__205>(ref <HandleEntityReferenceAsync>d__);
			return <HandleEntityReferenceAsync>d__.<>t__builder.Task;
		}

		private IDtdParserAdapter readerAdapter;

		private IDtdParserAdapterWithValidation readerAdapterWithValidation;

		private XmlNameTable nameTable;

		private SchemaInfo schemaInfo;

		private XmlCharType xmlCharType = XmlCharType.Instance;

		private string systemId = string.Empty;

		private string publicId = string.Empty;

		private bool normalize = true;

		private bool validate;

		private bool supportNamespaces = true;

		private bool v1Compat;

		private char[] chars;

		private int charsUsed;

		private int curPos;

		private DtdParser.ScanningFunction scanningFunction;

		private DtdParser.ScanningFunction nextScaningFunction;

		private DtdParser.ScanningFunction savedScanningFunction;

		private bool whitespaceSeen;

		private int tokenStartPos;

		private int colonPos;

		private StringBuilder internalSubsetValueSb;

		private int externalEntitiesDepth;

		private int currentEntityId;

		private bool freeFloatingDtd;

		private bool hasFreeFloatingInternalSubset;

		private StringBuilder stringBuilder;

		private int condSectionDepth;

		private LineInfo literalLineInfo = new LineInfo(0, 0);

		private char literalQuoteChar = '"';

		private string documentBaseUri = string.Empty;

		private string externalDtdBaseUri = string.Empty;

		private Dictionary<string, DtdParser.UndeclaredNotation> undeclaredNotations;

		private int[] condSectionEntityIds;

		private const int CondSectionEntityIdsInitialSize = 2;

		private enum Token
		{
			CDATA,
			ID,
			IDREF,
			IDREFS,
			ENTITY,
			ENTITIES,
			NMTOKEN,
			NMTOKENS,
			NOTATION,
			None,
			PERef,
			AttlistDecl,
			ElementDecl,
			EntityDecl,
			NotationDecl,
			Comment,
			PI,
			CondSectionStart,
			CondSectionEnd,
			Eof,
			REQUIRED,
			IMPLIED,
			FIXED,
			QName,
			Name,
			Nmtoken,
			Quote,
			LeftParen,
			RightParen,
			GreaterThan,
			Or,
			LeftBracket,
			RightBracket,
			PUBLIC,
			SYSTEM,
			Literal,
			DOCTYPE,
			NData,
			Percent,
			Star,
			QMark,
			Plus,
			PCDATA,
			Comma,
			ANY,
			EMPTY,
			IGNORE,
			INCLUDE
		}

		private enum ScanningFunction
		{
			SubsetContent,
			Name,
			QName,
			Nmtoken,
			Doctype1,
			Doctype2,
			Element1,
			Element2,
			Element3,
			Element4,
			Element5,
			Element6,
			Element7,
			Attlist1,
			Attlist2,
			Attlist3,
			Attlist4,
			Attlist5,
			Attlist6,
			Attlist7,
			Entity1,
			Entity2,
			Entity3,
			Notation1,
			CondSection1,
			CondSection2,
			CondSection3,
			Literal,
			SystemId,
			PublicId1,
			PublicId2,
			ClosingTag,
			ParamEntitySpace,
			None
		}

		private enum LiteralType
		{
			AttributeValue,
			EntityReplText,
			SystemOrPublicID
		}

		private class UndeclaredNotation
		{
			internal UndeclaredNotation(string name, int lineNo, int linePos)
			{
				this.name = name;
				this.lineNo = lineNo;
				this.linePos = linePos;
				this.next = null;
			}

			internal string name;

			internal int lineNo;

			internal int linePos;

			internal DtdParser.UndeclaredNotation next;
		}

		private class ParseElementOnlyContent_LocalFrame
		{
			public ParseElementOnlyContent_LocalFrame(int startParentEntityIdParam)
			{
				this.startParenEntityId = startParentEntityIdParam;
				this.parsingSchema = DtdParser.Token.None;
			}

			public int startParenEntityId;

			public DtdParser.Token parsingSchema;
		}
	}
}
