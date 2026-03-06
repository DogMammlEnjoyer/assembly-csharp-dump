using System;
using System.IO.Enumeration;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.IO
{
	/// <summary>Provides the base class for both <see cref="T:System.IO.FileInfo" /> and <see cref="T:System.IO.DirectoryInfo" /> objects.</summary>
	[Serializable]
	public abstract class FileSystemInfo : MarshalByRefObject, ISerializable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileSystemInfo" /> class.</summary>
		protected FileSystemInfo()
		{
		}

		internal static FileSystemInfo Create(string fullPath, ref FileSystemEntry findData)
		{
			DirectoryInfo directoryInfo = findData.IsDirectory ? new DirectoryInfo(fullPath, null, new string(findData.FileName), true) : new FileInfo(fullPath, null, new string(findData.FileName), true);
			directoryInfo.Init(findData._info);
			return directoryInfo;
		}

		internal void Invalidate()
		{
			this._dataInitialized = -1;
		}

		internal unsafe void Init(Interop.NtDll.FILE_FULL_DIR_INFORMATION* info)
		{
			this._data.dwFileAttributes = (int)info->FileAttributes;
			this._data.ftCreationTime = *(Interop.Kernel32.FILE_TIME*)(&info->CreationTime);
			this._data.ftLastAccessTime = *(Interop.Kernel32.FILE_TIME*)(&info->LastAccessTime);
			this._data.ftLastWriteTime = *(Interop.Kernel32.FILE_TIME*)(&info->LastWriteTime);
			this._data.nFileSizeHigh = (uint)(info->EndOfFile >> 32);
			this._data.nFileSizeLow = (uint)info->EndOfFile;
			this._dataInitialized = 0;
		}

		/// <summary>Gets or sets the attributes for the current file or directory.</summary>
		/// <returns>
		///   <see cref="T:System.IO.FileAttributes" /> of the current <see cref="T:System.IO.FileSystemInfo" />.</returns>
		/// <exception cref="T:System.IO.FileNotFoundException">The specified file doesn't exist. Only thrown when setting the property value.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid. For example, it's on an unmapped drive. Only thrown when setting the property value.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller doesn't have the required permission.</exception>
		/// <exception cref="T:System.ArgumentException">The caller attempts to set an invalid file attribute.  
		///  -or-  
		///  The user attempts to set an attribute value but doesn't have write permission.</exception>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		public FileAttributes Attributes
		{
			get
			{
				this.EnsureDataInitialized();
				return (FileAttributes)this._data.dwFileAttributes;
			}
			set
			{
				FileSystem.SetAttributes(this.FullPath, value);
				this._dataInitialized = -1;
			}
		}

		internal bool ExistsCore
		{
			get
			{
				if (this._dataInitialized == -1)
				{
					this.Refresh();
				}
				return this._dataInitialized == 0 && this._data.dwFileAttributes != -1 && this is DirectoryInfo == ((this._data.dwFileAttributes & 16) == 16);
			}
		}

		internal DateTimeOffset CreationTimeCore
		{
			get
			{
				this.EnsureDataInitialized();
				return this._data.ftCreationTime.ToDateTimeOffset();
			}
			set
			{
				FileSystem.SetCreationTime(this.FullPath, value, this is DirectoryInfo);
				this._dataInitialized = -1;
			}
		}

		internal DateTimeOffset LastAccessTimeCore
		{
			get
			{
				this.EnsureDataInitialized();
				return this._data.ftLastAccessTime.ToDateTimeOffset();
			}
			set
			{
				FileSystem.SetLastAccessTime(this.FullPath, value, this is DirectoryInfo);
				this._dataInitialized = -1;
			}
		}

		internal DateTimeOffset LastWriteTimeCore
		{
			get
			{
				this.EnsureDataInitialized();
				return this._data.ftLastWriteTime.ToDateTimeOffset();
			}
			set
			{
				FileSystem.SetLastWriteTime(this.FullPath, value, this is DirectoryInfo);
				this._dataInitialized = -1;
			}
		}

		internal long LengthCore
		{
			get
			{
				this.EnsureDataInitialized();
				return (long)((ulong)this._data.nFileSizeHigh << 32 | ((ulong)this._data.nFileSizeLow & (ulong)-1));
			}
		}

		private void EnsureDataInitialized()
		{
			if (this._dataInitialized == -1)
			{
				this._data = default(Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA);
				this.Refresh();
			}
			if (this._dataInitialized != 0)
			{
				throw Win32Marshal.GetExceptionForWin32Error(this._dataInitialized, this.FullPath);
			}
		}

		/// <summary>Refreshes the state of the object.</summary>
		/// <exception cref="T:System.IO.IOException">A device such as a disk drive is not ready.</exception>
		public void Refresh()
		{
			this._dataInitialized = FileSystem.FillAttributeInfo(this.FullPath, ref this._data, false);
		}

		internal string NormalizedPath
		{
			get
			{
				if (!PathInternal.EndsWithPeriodOrSpace(this.FullPath))
				{
					return this.FullPath;
				}
				return PathInternal.EnsureExtendedPrefix(this.FullPath);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.FileSystemInfo" /> class with serialized data.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The specified <see cref="T:System.Runtime.Serialization.SerializationInfo" /> is null.</exception>
		protected FileSystemInfo(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.FullPath = Path.GetFullPathInternal(info.GetString("FullPath"));
			this.OriginalPath = info.GetString("OriginalPath");
			this._name = info.GetString("Name");
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the file name and additional exception information.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		[ComVisible(false)]
		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("OriginalPath", this.OriginalPath, typeof(string));
			info.AddValue("FullPath", this.FullPath, typeof(string));
			info.AddValue("Name", this.Name, typeof(string));
		}

		/// <summary>Gets the full path of the directory or file.</summary>
		/// <returns>A string containing the full path.</returns>
		/// <exception cref="T:System.IO.PathTooLongException">The fully qualified path and file name exceed the system-defined maximum length.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public virtual string FullName
		{
			get
			{
				return this.FullPath;
			}
		}

		/// <summary>Gets the string representing the extension part of the file.</summary>
		/// <returns>A string containing the <see cref="T:System.IO.FileSystemInfo" /> extension.</returns>
		public string Extension
		{
			get
			{
				int length = this.FullPath.Length;
				int num = length;
				while (--num >= 0)
				{
					char c = this.FullPath[num];
					if (c == '.')
					{
						return this.FullPath.Substring(num, length - num);
					}
					if (PathInternal.IsDirectorySeparator(c) || c == Path.VolumeSeparatorChar)
					{
						break;
					}
				}
				return string.Empty;
			}
		}

		/// <summary>For files, gets the name of the file. For directories, gets the name of the last directory in the hierarchy if a hierarchy exists. Otherwise, the <see langword="Name" /> property gets the name of the directory.</summary>
		/// <returns>A string that is the name of the parent directory, the name of the last directory in the hierarchy, or the name of a file, including the file name extension.</returns>
		public virtual string Name
		{
			get
			{
				return this._name;
			}
		}

		/// <summary>Gets a value indicating whether the file or directory exists.</summary>
		/// <returns>
		///   <see langword="true" /> if the file or directory exists; otherwise, <see langword="false" />.</returns>
		public virtual bool Exists
		{
			get
			{
				bool result;
				try
				{
					result = this.ExistsCore;
				}
				catch
				{
					result = false;
				}
				return result;
			}
		}

		/// <summary>Deletes a file or directory.</summary>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid; for example, it is on an unmapped drive.</exception>
		/// <exception cref="T:System.IO.IOException">There is an open handle on the file or directory, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and Files.</exception>
		public abstract void Delete();

		/// <summary>Gets or sets the creation time of the current file or directory.</summary>
		/// <returns>The creation date and time of the current <see cref="T:System.IO.FileSystemInfo" /> object.</returns>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid; for example, it is on an unmapped drive.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The caller attempts to set an invalid creation time.</exception>
		public DateTime CreationTime
		{
			get
			{
				return this.CreationTimeUtc.ToLocalTime();
			}
			set
			{
				this.CreationTimeUtc = value.ToUniversalTime();
			}
		}

		/// <summary>Gets or sets the creation time, in coordinated universal time (UTC), of the current file or directory.</summary>
		/// <returns>The creation date and time in UTC format of the current <see cref="T:System.IO.FileSystemInfo" /> object.</returns>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid; for example, it is on an unmapped drive.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The caller attempts to set an invalid access time.</exception>
		public DateTime CreationTimeUtc
		{
			get
			{
				return this.CreationTimeCore.UtcDateTime;
			}
			set
			{
				this.CreationTimeCore = File.GetUtcDateTimeOffset(value);
			}
		}

		/// <summary>Gets or sets the time the current file or directory was last accessed.</summary>
		/// <returns>The time that the current file or directory was last accessed.</returns>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The caller attempts to set an invalid access time</exception>
		public DateTime LastAccessTime
		{
			get
			{
				return this.LastAccessTimeUtc.ToLocalTime();
			}
			set
			{
				this.LastAccessTimeUtc = value.ToUniversalTime();
			}
		}

		/// <summary>Gets or sets the time, in coordinated universal time (UTC), that the current file or directory was last accessed.</summary>
		/// <returns>The UTC time that the current file or directory was last accessed.</returns>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The caller attempts to set an invalid access time.</exception>
		public DateTime LastAccessTimeUtc
		{
			get
			{
				return this.LastAccessTimeCore.UtcDateTime;
			}
			set
			{
				this.LastAccessTimeCore = File.GetUtcDateTimeOffset(value);
			}
		}

		/// <summary>Gets or sets the time when the current file or directory was last written to.</summary>
		/// <returns>The time the current file was last written.</returns>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The caller attempts to set an invalid write time.</exception>
		public DateTime LastWriteTime
		{
			get
			{
				return this.LastWriteTimeUtc.ToLocalTime();
			}
			set
			{
				this.LastWriteTimeUtc = value.ToUniversalTime();
			}
		}

		/// <summary>Gets or sets the time, in coordinated universal time (UTC), when the current file or directory was last written to.</summary>
		/// <returns>The UTC time when the current file was last written to.</returns>
		/// <exception cref="T:System.IO.IOException">
		///   <see cref="M:System.IO.FileSystemInfo.Refresh" /> cannot initialize the data.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">The current operating system is not Windows NT or later.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The caller attempts to set an invalid write time.</exception>
		public DateTime LastWriteTimeUtc
		{
			get
			{
				return this.LastWriteTimeCore.UtcDateTime;
			}
			set
			{
				this.LastWriteTimeCore = File.GetUtcDateTimeOffset(value);
			}
		}

		public override string ToString()
		{
			return this.OriginalPath ?? string.Empty;
		}

		private Interop.Kernel32.WIN32_FILE_ATTRIBUTE_DATA _data;

		private int _dataInitialized = -1;

		/// <summary>Represents the fully qualified path of the directory or file.</summary>
		/// <exception cref="T:System.IO.PathTooLongException">The fully qualified path exceeds the system-defined maximum length.</exception>
		protected string FullPath;

		/// <summary>The path originally specified by the user, whether relative or absolute.</summary>
		protected string OriginalPath;

		internal string _name;
	}
}
