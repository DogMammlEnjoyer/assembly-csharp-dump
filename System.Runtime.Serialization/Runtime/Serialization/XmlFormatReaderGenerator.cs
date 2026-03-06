using System;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class XmlFormatReaderGenerator
	{
		[SecurityCritical]
		public XmlFormatReaderGenerator()
		{
			this.helper = new XmlFormatReaderGenerator.CriticalHelper();
		}

		[SecurityCritical]
		public XmlFormatClassReaderDelegate GenerateClassReader(ClassDataContract classContract)
		{
			XmlFormatClassReaderDelegate result;
			try
			{
				if (TD.DCGenReaderStartIsEnabled())
				{
					TD.DCGenReaderStart("Class", classContract.UnderlyingType.FullName);
				}
				result = this.helper.GenerateClassReader(classContract);
			}
			finally
			{
				if (TD.DCGenReaderStopIsEnabled())
				{
					TD.DCGenReaderStop();
				}
			}
			return result;
		}

		[SecurityCritical]
		public XmlFormatCollectionReaderDelegate GenerateCollectionReader(CollectionDataContract collectionContract)
		{
			XmlFormatCollectionReaderDelegate result;
			try
			{
				if (TD.DCGenReaderStartIsEnabled())
				{
					TD.DCGenReaderStart("Collection", collectionContract.UnderlyingType.FullName);
				}
				result = this.helper.GenerateCollectionReader(collectionContract);
			}
			finally
			{
				if (TD.DCGenReaderStopIsEnabled())
				{
					TD.DCGenReaderStop();
				}
			}
			return result;
		}

		[SecurityCritical]
		public XmlFormatGetOnlyCollectionReaderDelegate GenerateGetOnlyCollectionReader(CollectionDataContract collectionContract)
		{
			XmlFormatGetOnlyCollectionReaderDelegate result;
			try
			{
				if (TD.DCGenReaderStartIsEnabled())
				{
					TD.DCGenReaderStart("GetOnlyCollection", collectionContract.UnderlyingType.FullName);
				}
				result = this.helper.GenerateGetOnlyCollectionReader(collectionContract);
			}
			finally
			{
				if (TD.DCGenReaderStopIsEnabled())
				{
					TD.DCGenReaderStop();
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		internal static object UnsafeGetUninitializedObject(int id)
		{
			return FormatterServices.GetUninitializedObject(DataContract.GetDataContractForInitialization(id).TypeForInitialization);
		}

		[SecurityCritical]
		private XmlFormatReaderGenerator.CriticalHelper helper;

		private class CriticalHelper
		{
			internal XmlFormatClassReaderDelegate GenerateClassReader(ClassDataContract classContract)
			{
				return (XmlReaderDelegator xr, XmlObjectSerializerReadContext ctx, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces) => new XmlFormatReaderInterpreter(classContract).ReadFromXml(xr, ctx, memberNames, memberNamespaces);
			}

			internal XmlFormatCollectionReaderDelegate GenerateCollectionReader(CollectionDataContract collectionContract)
			{
				return (XmlReaderDelegator xr, XmlObjectSerializerReadContext ctx, XmlDictionaryString inm, XmlDictionaryString ins, CollectionDataContract cc) => new XmlFormatReaderInterpreter(collectionContract, false).ReadCollectionFromXml(xr, ctx, inm, ins, cc);
			}

			internal XmlFormatGetOnlyCollectionReaderDelegate GenerateGetOnlyCollectionReader(CollectionDataContract collectionContract)
			{
				return delegate(XmlReaderDelegator xr, XmlObjectSerializerReadContext ctx, XmlDictionaryString inm, XmlDictionaryString ins, CollectionDataContract cc)
				{
					new XmlFormatReaderInterpreter(collectionContract, true).ReadGetOnlyCollectionFromXml(xr, ctx, inm, ins, cc);
				};
			}
		}
	}
}
