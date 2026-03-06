using System;
using System.Dynamic.Utils;
using Unity;

namespace System.Linq.Expressions
{
	/// <summary>Stores information necessary to emit debugging symbol information for a source file, in particular the file name and unique language identifier.</summary>
	public class SymbolDocumentInfo
	{
		internal SymbolDocumentInfo(string fileName)
		{
			ContractUtils.RequiresNotNull(fileName, "fileName");
			this.FileName = fileName;
		}

		/// <summary>The source file name.</summary>
		/// <returns>The string representing the source file name.</returns>
		public string FileName { get; }

		/// <summary>Returns the language's unique identifier, if any.</summary>
		/// <returns>The language's unique identifier</returns>
		public virtual Guid Language
		{
			get
			{
				return Guid.Empty;
			}
		}

		/// <summary>Returns the language vendor's unique identifier, if any.</summary>
		/// <returns>The language vendor's unique identifier.</returns>
		public virtual Guid LanguageVendor
		{
			get
			{
				return Guid.Empty;
			}
		}

		/// <summary>Returns the document type's unique identifier, if any. Defaults to the GUID for a text file.</summary>
		/// <returns>The document type's unique identifier.</returns>
		public virtual Guid DocumentType
		{
			get
			{
				return SymbolDocumentInfo.DocumentType_Text;
			}
		}

		internal SymbolDocumentInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal static readonly Guid DocumentType_Text = new Guid(1518771467, 26129, 4563, 189, 42, 0, 0, 248, 8, 73, 189);
	}
}
