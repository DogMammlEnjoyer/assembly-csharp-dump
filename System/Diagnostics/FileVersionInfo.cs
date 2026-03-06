using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;

namespace System.Diagnostics
{
	/// <summary>Provides version information for a physical file on disk.</summary>
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class FileVersionInfo
	{
		private FileVersionInfo()
		{
			this.comments = null;
			this.companyname = null;
			this.filedescription = null;
			this.filename = null;
			this.fileversion = null;
			this.internalname = null;
			this.language = null;
			this.legalcopyright = null;
			this.legaltrademarks = null;
			this.originalfilename = null;
			this.privatebuild = null;
			this.productname = null;
			this.productversion = null;
			this.specialbuild = null;
			this.isdebug = false;
			this.ispatched = false;
			this.isprerelease = false;
			this.isprivatebuild = false;
			this.isspecialbuild = false;
			this.filemajorpart = 0;
			this.fileminorpart = 0;
			this.filebuildpart = 0;
			this.fileprivatepart = 0;
			this.productmajorpart = 0;
			this.productminorpart = 0;
			this.productbuildpart = 0;
			this.productprivatepart = 0;
		}

		/// <summary>Gets the comments associated with the file.</summary>
		/// <returns>The comments associated with the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string Comments
		{
			get
			{
				return this.comments;
			}
		}

		/// <summary>Gets the name of the company that produced the file.</summary>
		/// <returns>The name of the company that produced the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string CompanyName
		{
			get
			{
				return this.companyname;
			}
		}

		/// <summary>Gets the build number of the file.</summary>
		/// <returns>A value representing the build number of the file or <see langword="null" /> if the file did not contain version information.</returns>
		public int FileBuildPart
		{
			get
			{
				return this.filebuildpart;
			}
		}

		/// <summary>Gets the description of the file.</summary>
		/// <returns>The description of the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string FileDescription
		{
			get
			{
				return this.filedescription;
			}
		}

		/// <summary>Gets the major part of the version number.</summary>
		/// <returns>A value representing the major part of the version number or 0 (zero) if the file did not contain version information.</returns>
		public int FileMajorPart
		{
			get
			{
				return this.filemajorpart;
			}
		}

		/// <summary>Gets the minor part of the version number of the file.</summary>
		/// <returns>A value representing the minor part of the version number of the file or 0 (zero) if the file did not contain version information.</returns>
		public int FileMinorPart
		{
			get
			{
				return this.fileminorpart;
			}
		}

		/// <summary>Gets the name of the file that this instance of <see cref="T:System.Diagnostics.FileVersionInfo" /> describes.</summary>
		/// <returns>The name of the file described by this instance of <see cref="T:System.Diagnostics.FileVersionInfo" />.</returns>
		public string FileName
		{
			get
			{
				return this.filename;
			}
		}

		/// <summary>Gets the file private part number.</summary>
		/// <returns>A value representing the file private part number or <see langword="null" /> if the file did not contain version information.</returns>
		public int FilePrivatePart
		{
			get
			{
				return this.fileprivatepart;
			}
		}

		/// <summary>Gets the file version number.</summary>
		/// <returns>The version number of the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string FileVersion
		{
			get
			{
				return this.fileversion;
			}
		}

		/// <summary>Gets the internal name of the file, if one exists.</summary>
		/// <returns>The internal name of the file. If none exists, this property will contain the original name of the file without the extension.</returns>
		public string InternalName
		{
			get
			{
				return this.internalname;
			}
		}

		/// <summary>Gets a value that specifies whether the file contains debugging information or is compiled with debugging features enabled.</summary>
		/// <returns>
		///   <see langword="true" /> if the file contains debugging information or is compiled with debugging features enabled; otherwise, <see langword="false" />.</returns>
		public bool IsDebug
		{
			get
			{
				return this.isdebug;
			}
		}

		/// <summary>Gets a value that specifies whether the file has been modified and is not identical to the original shipping file of the same version number.</summary>
		/// <returns>
		///   <see langword="true" /> if the file is patched; otherwise, <see langword="false" />.</returns>
		public bool IsPatched
		{
			get
			{
				return this.ispatched;
			}
		}

		/// <summary>Gets a value that specifies whether the file is a development version, rather than a commercially released product.</summary>
		/// <returns>
		///   <see langword="true" /> if the file is prerelease; otherwise, <see langword="false" />.</returns>
		public bool IsPreRelease
		{
			get
			{
				return this.isprerelease;
			}
		}

		/// <summary>Gets a value that specifies whether the file was built using standard release procedures.</summary>
		/// <returns>
		///   <see langword="true" /> if the file is a private build; <see langword="false" /> if the file was built using standard release procedures or if the file did not contain version information.</returns>
		public bool IsPrivateBuild
		{
			get
			{
				return this.isprivatebuild;
			}
		}

		/// <summary>Gets a value that specifies whether the file is a special build.</summary>
		/// <returns>
		///   <see langword="true" /> if the file is a special build; otherwise, <see langword="false" />.</returns>
		public bool IsSpecialBuild
		{
			get
			{
				return this.isspecialbuild;
			}
		}

		/// <summary>Gets the default language string for the version info block.</summary>
		/// <returns>The description string for the Microsoft Language Identifier in the version resource or <see langword="null" /> if the file did not contain version information.</returns>
		public string Language
		{
			get
			{
				return this.language;
			}
		}

		/// <summary>Gets all copyright notices that apply to the specified file.</summary>
		/// <returns>The copyright notices that apply to the specified file.</returns>
		public string LegalCopyright
		{
			get
			{
				return this.legalcopyright;
			}
		}

		/// <summary>Gets the trademarks and registered trademarks that apply to the file.</summary>
		/// <returns>The trademarks and registered trademarks that apply to the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string LegalTrademarks
		{
			get
			{
				return this.legaltrademarks;
			}
		}

		/// <summary>Gets the name the file was created with.</summary>
		/// <returns>The name the file was created with or <see langword="null" /> if the file did not contain version information.</returns>
		public string OriginalFilename
		{
			get
			{
				return this.originalfilename;
			}
		}

		/// <summary>Gets information about a private version of the file.</summary>
		/// <returns>Information about a private version of the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string PrivateBuild
		{
			get
			{
				return this.privatebuild;
			}
		}

		/// <summary>Gets the build number of the product this file is associated with.</summary>
		/// <returns>A value representing the build number of the product this file is associated with or <see langword="null" /> if the file did not contain version information.</returns>
		public int ProductBuildPart
		{
			get
			{
				return this.productbuildpart;
			}
		}

		/// <summary>Gets the major part of the version number for the product this file is associated with.</summary>
		/// <returns>A value representing the major part of the product version number or <see langword="null" /> if the file did not contain version information.</returns>
		public int ProductMajorPart
		{
			get
			{
				return this.productmajorpart;
			}
		}

		/// <summary>Gets the minor part of the version number for the product the file is associated with.</summary>
		/// <returns>A value representing the minor part of the product version number or <see langword="null" /> if the file did not contain version information.</returns>
		public int ProductMinorPart
		{
			get
			{
				return this.productminorpart;
			}
		}

		/// <summary>Gets the name of the product this file is distributed with.</summary>
		/// <returns>The name of the product this file is distributed with or <see langword="null" /> if the file did not contain version information.</returns>
		public string ProductName
		{
			get
			{
				return this.productname;
			}
		}

		/// <summary>Gets the private part number of the product this file is associated with.</summary>
		/// <returns>A value representing the private part number of the product this file is associated with or <see langword="null" /> if the file did not contain version information.</returns>
		public int ProductPrivatePart
		{
			get
			{
				return this.productprivatepart;
			}
		}

		/// <summary>Gets the version of the product this file is distributed with.</summary>
		/// <returns>The version of the product this file is distributed with or <see langword="null" /> if the file did not contain version information.</returns>
		public string ProductVersion
		{
			get
			{
				return this.productversion;
			}
		}

		/// <summary>Gets the special build information for the file.</summary>
		/// <returns>The special build information for the file or <see langword="null" /> if the file did not contain version information.</returns>
		public string SpecialBuild
		{
			get
			{
				return this.specialbuild;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void GetVersionInfo_icall(char* fileName, int fileName_length);

		private unsafe void GetVersionInfo_internal(string fileName)
		{
			fixed (string text = fileName)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.GetVersionInfo_icall(ptr, (fileName != null) ? fileName.Length : 0);
			}
		}

		/// <summary>Returns a <see cref="T:System.Diagnostics.FileVersionInfo" /> representing the version information associated with the specified file.</summary>
		/// <param name="fileName">The fully qualified path and name of the file to retrieve the version information for.</param>
		/// <returns>A <see cref="T:System.Diagnostics.FileVersionInfo" /> containing information about the file. If the file did not contain version information, the <see cref="T:System.Diagnostics.FileVersionInfo" /> contains only the name of the file requested.</returns>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified cannot be found.</exception>
		public static FileVersionInfo GetVersionInfo(string fileName)
		{
			if (!File.Exists(Path.GetFullPath(fileName)))
			{
				throw new FileNotFoundException(fileName);
			}
			FileVersionInfo fileVersionInfo = new FileVersionInfo();
			fileVersionInfo.GetVersionInfo_internal(fileName);
			return fileVersionInfo;
		}

		private static void AppendFormat(StringBuilder sb, string format, params object[] args)
		{
			sb.AppendFormat(format, args);
		}

		/// <summary>Returns a partial list of properties in the <see cref="T:System.Diagnostics.FileVersionInfo" /> and their values.</summary>
		/// <returns>A list of the following properties in this class and their values:  
		///  <see cref="P:System.Diagnostics.FileVersionInfo.FileName" />, <see cref="P:System.Diagnostics.FileVersionInfo.InternalName" />, <see cref="P:System.Diagnostics.FileVersionInfo.OriginalFilename" />, <see cref="P:System.Diagnostics.FileVersionInfo.FileVersion" />, <see cref="P:System.Diagnostics.FileVersionInfo.FileDescription" />, <see cref="P:System.Diagnostics.FileVersionInfo.ProductName" />, <see cref="P:System.Diagnostics.FileVersionInfo.ProductVersion" />, <see cref="P:System.Diagnostics.FileVersionInfo.IsDebug" />, <see cref="P:System.Diagnostics.FileVersionInfo.IsPatched" />, <see cref="P:System.Diagnostics.FileVersionInfo.IsPreRelease" />, <see cref="P:System.Diagnostics.FileVersionInfo.IsPrivateBuild" />, <see cref="P:System.Diagnostics.FileVersionInfo.IsSpecialBuild" />,  
		///  <see cref="P:System.Diagnostics.FileVersionInfo.Language" />.  
		///  If the file did not contain version information, this list will contain only the name of the requested file. Boolean values will be <see langword="false" />, and all other entries will be <see langword="null" />.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			FileVersionInfo.AppendFormat(stringBuilder, "File:             {0}{1}", new object[]
			{
				this.FileName,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "InternalName:     {0}{1}", new object[]
			{
				this.internalname,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "OriginalFilename: {0}{1}", new object[]
			{
				this.originalfilename,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "FileVersion:      {0}{1}", new object[]
			{
				this.fileversion,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "FileDescription:  {0}{1}", new object[]
			{
				this.filedescription,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Product:          {0}{1}", new object[]
			{
				this.productname,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "ProductVersion:   {0}{1}", new object[]
			{
				this.productversion,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Debug:            {0}{1}", new object[]
			{
				this.isdebug,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Patched:          {0}{1}", new object[]
			{
				this.ispatched,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "PreRelease:       {0}{1}", new object[]
			{
				this.isprerelease,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "PrivateBuild:     {0}{1}", new object[]
			{
				this.isprivatebuild,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "SpecialBuild:     {0}{1}", new object[]
			{
				this.isspecialbuild,
				Environment.NewLine
			});
			FileVersionInfo.AppendFormat(stringBuilder, "Language          {0}{1}", new object[]
			{
				this.language,
				Environment.NewLine
			});
			return stringBuilder.ToString();
		}

		private string comments;

		private string companyname;

		private string filedescription;

		private string filename;

		private string fileversion;

		private string internalname;

		private string language;

		private string legalcopyright;

		private string legaltrademarks;

		private string originalfilename;

		private string privatebuild;

		private string productname;

		private string productversion;

		private string specialbuild;

		private bool isdebug;

		private bool ispatched;

		private bool isprerelease;

		private bool isprivatebuild;

		private bool isspecialbuild;

		private int filemajorpart;

		private int fileminorpart;

		private int filebuildpart;

		private int fileprivatepart;

		private int productmajorpart;

		private int productminorpart;

		private int productbuildpart;

		private int productprivatepart;
	}
}
