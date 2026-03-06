using System;

namespace System.Xml.Schema
{
	/// <summary>Represents the W3C XML Schema Definition Language (XSD) schema types.</summary>
	public enum XmlTypeCode
	{
		/// <summary>No type information.</summary>
		None,
		/// <summary>An item such as a node or atomic value.</summary>
		Item,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Node,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Document,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Element,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Attribute,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Namespace,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		ProcessingInstruction,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Comment,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		Text,
		/// <summary>Any atomic value of a union.</summary>
		AnyAtomicType,
		/// <summary>An untyped atomic value.</summary>
		UntypedAtomic,
		/// <summary>A W3C XML Schema <see langword="xs:string" /> type.</summary>
		String,
		/// <summary>A W3C XML Schema <see langword="xs:boolean" /> type.</summary>
		Boolean,
		/// <summary>A W3C XML Schema <see langword="xs:decimal" /> type.</summary>
		Decimal,
		/// <summary>A W3C XML Schema <see langword="xs:float" /> type.</summary>
		Float,
		/// <summary>A W3C XML Schema <see langword="xs:double" /> type.</summary>
		Double,
		/// <summary>A W3C XML Schema <see langword="xs:Duration" /> type.</summary>
		Duration,
		/// <summary>A W3C XML Schema <see langword="xs:dateTime" /> type.</summary>
		DateTime,
		/// <summary>A W3C XML Schema <see langword="xs:time" /> type.</summary>
		Time,
		/// <summary>A W3C XML Schema <see langword="xs:date" /> type.</summary>
		Date,
		/// <summary>A W3C XML Schema <see langword="xs:gYearMonth" /> type.</summary>
		GYearMonth,
		/// <summary>A W3C XML Schema <see langword="xs:gYear" /> type.</summary>
		GYear,
		/// <summary>A W3C XML Schema <see langword="xs:gMonthDay" /> type.</summary>
		GMonthDay,
		/// <summary>A W3C XML Schema <see langword="xs:gDay" /> type.</summary>
		GDay,
		/// <summary>A W3C XML Schema <see langword="xs:gMonth" /> type.</summary>
		GMonth,
		/// <summary>A W3C XML Schema <see langword="xs:hexBinary" /> type.</summary>
		HexBinary,
		/// <summary>A W3C XML Schema <see langword="xs:base64Binary" /> type.</summary>
		Base64Binary,
		/// <summary>A W3C XML Schema <see langword="xs:anyURI" /> type.</summary>
		AnyUri,
		/// <summary>A W3C XML Schema <see langword="xs:QName" /> type.</summary>
		QName,
		/// <summary>A W3C XML Schema <see langword="xs:NOTATION" /> type.</summary>
		Notation,
		/// <summary>A W3C XML Schema <see langword="xs:normalizedString" /> type.</summary>
		NormalizedString,
		/// <summary>A W3C XML Schema <see langword="xs:token" /> type.</summary>
		Token,
		/// <summary>A W3C XML Schema <see langword="xs:language" /> type.</summary>
		Language,
		/// <summary>A W3C XML Schema <see langword="xs:NMTOKEN" /> type.</summary>
		NmToken,
		/// <summary>A W3C XML Schema <see langword="xs:Name" /> type.</summary>
		Name,
		/// <summary>A W3C XML Schema <see langword="xs:NCName" /> type.</summary>
		NCName,
		/// <summary>A W3C XML Schema <see langword="xs:ID" /> type.</summary>
		Id,
		/// <summary>A W3C XML Schema <see langword="xs:IDREF" /> type.</summary>
		Idref,
		/// <summary>A W3C XML Schema <see langword="xs:ENTITY" /> type.</summary>
		Entity,
		/// <summary>A W3C XML Schema <see langword="xs:integer" /> type.</summary>
		Integer,
		/// <summary>A W3C XML Schema <see langword="xs:nonPositiveInteger" /> type.</summary>
		NonPositiveInteger,
		/// <summary>A W3C XML Schema <see langword="xs:negativeInteger" /> type.</summary>
		NegativeInteger,
		/// <summary>A W3C XML Schema <see langword="xs:long" /> type.</summary>
		Long,
		/// <summary>A W3C XML Schema <see langword="xs:int" /> type.</summary>
		Int,
		/// <summary>A W3C XML Schema <see langword="xs:short" /> type.</summary>
		Short,
		/// <summary>A W3C XML Schema <see langword="xs:byte" /> type.</summary>
		Byte,
		/// <summary>A W3C XML Schema <see langword="xs:nonNegativeInteger" /> type.</summary>
		NonNegativeInteger,
		/// <summary>A W3C XML Schema <see langword="xs:unsignedLong" /> type.</summary>
		UnsignedLong,
		/// <summary>A W3C XML Schema <see langword="xs:unsignedInt" /> type.</summary>
		UnsignedInt,
		/// <summary>A W3C XML Schema <see langword="xs:unsignedShort" /> type.</summary>
		UnsignedShort,
		/// <summary>A W3C XML Schema <see langword="xs:unsignedByte" /> type.</summary>
		UnsignedByte,
		/// <summary>A W3C XML Schema <see langword="xs:positiveInteger" /> type.</summary>
		PositiveInteger,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		YearMonthDuration,
		/// <summary>This value supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
		DayTimeDuration
	}
}
