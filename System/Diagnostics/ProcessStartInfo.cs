using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System.Diagnostics
{
	/// <summary>Specifies a set of values that are used when you start a process.</summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true, SelfAffectingProcessMgmt = true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ProcessStartInfo
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.ProcessStartInfo" /> class without specifying a file name with which to start the process.</summary>
		public ProcessStartInfo()
		{
		}

		internal ProcessStartInfo(Process parent)
		{
			this.weakParentProcess = new WeakReference(parent);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.ProcessStartInfo" /> class and specifies a file name such as an application or document with which to start the process.</summary>
		/// <param name="fileName">An application or document with which to start a process.</param>
		public ProcessStartInfo(string fileName)
		{
			this.fileName = fileName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.ProcessStartInfo" /> class, specifies an application file name with which to start the process, and specifies a set of command-line arguments to pass to the application.</summary>
		/// <param name="fileName">An application with which to start a process.</param>
		/// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
		public ProcessStartInfo(string fileName, string arguments)
		{
			this.fileName = fileName;
			this.arguments = arguments;
		}

		public Collection<string> ArgumentList
		{
			get
			{
				if (this._argumentList == null)
				{
					this._argumentList = new Collection<string>();
				}
				return this._argumentList;
			}
		}

		/// <summary>Gets or sets the verb to use when opening the application or document specified by the <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property.</summary>
		/// <returns>The action to take with the file that the process opens. The default is an empty string (""), which signifies no action.</returns>
		[DefaultValue("")]
		[TypeConverter("System.Diagnostics.Design.VerbConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("The verb to apply to the document specified by the FileName property.")]
		[NotifyParentProperty(true)]
		public string Verb
		{
			get
			{
				if (this.verb == null)
				{
					return string.Empty;
				}
				return this.verb;
			}
			set
			{
				this.verb = value;
			}
		}

		/// <summary>Gets or sets the set of command-line arguments to use when starting the application.</summary>
		/// <returns>A single string containing the arguments to pass to the target application specified in the <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property. The default is an empty string (""). On Windows Vista and earlier versions of the Windows operating system, the length of the arguments added to the length of the full path to the process must be less than 2080. On Windows 7 and later versions, the length must be less than 32699.  
		///  Arguments are parsed and interpreted by the target application, so must align with the expectations of that application. For.NET applications as demonstrated in the Examples below, spaces are interpreted as a separator between multiple arguments. A single argument that includes spaces must be surrounded by quotation marks, but those quotation marks are not carried through to the target application. In include quotation marks in the final parsed argument, triple-escape each mark.</returns>
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[NotifyParentProperty(true)]
		[SettingsBindable(true)]
		[DefaultValue("")]
		[MonitoringDescription("Command line arguments that will be passed to the application specified by the FileName property.")]
		public string Arguments
		{
			get
			{
				if (this.arguments == null)
				{
					return string.Empty;
				}
				return this.arguments;
			}
			set
			{
				this.arguments = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to start the process in a new window.</summary>
		/// <returns>
		///   <see langword="true" /> if the process should be started without creating a new window to contain it; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[MonitoringDescription("Whether to start the process without creating a new window to contain it.")]
		public bool CreateNoWindow
		{
			get
			{
				return this.createNoWindow;
			}
			set
			{
				this.createNoWindow = value;
			}
		}

		/// <summary>Gets search paths for files, directories for temporary files, application-specific options, and other similar information.</summary>
		/// <returns>A string dictionary that provides environment variables that apply to this process and child processes. The default is <see langword="null" />.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(null)]
		[MonitoringDescription("Set of environment variables that apply to this process and child processes.")]
		[NotifyParentProperty(true)]
		[Editor("System.Diagnostics.Design.StringDictionaryEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public StringDictionary EnvironmentVariables
		{
			get
			{
				if (this.environmentVariables == null)
				{
					this.environmentVariables = new CaseSensitiveStringDictionary();
					if (this.weakParentProcess == null || !this.weakParentProcess.IsAlive || ((Component)this.weakParentProcess.Target).Site == null || !((Component)this.weakParentProcess.Target).Site.DesignMode)
					{
						foreach (object obj in System.Environment.GetEnvironmentVariables())
						{
							DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
							this.environmentVariables.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
						}
					}
				}
				return this.environmentVariables;
			}
		}

		/// <summary>Gets the environment variables that apply to this process and its child processes.</summary>
		/// <returns>A generic dictionary containing the environment variables that apply to this process and its child processes. The default is <see langword="null" />.</returns>
		[DefaultValue(null)]
		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDictionary<string, string> Environment
		{
			get
			{
				if (this.environment == null)
				{
					this.environment = this.EnvironmentVariables.AsGenericDictionary();
				}
				return this.environment;
			}
		}

		/// <summary>Gets or sets a value indicating whether the input for an application is read from the <see cref="P:System.Diagnostics.Process.StandardInput" /> stream.</summary>
		/// <returns>
		///   <see langword="true" /> if input should be read from <see cref="P:System.Diagnostics.Process.StandardInput" />; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[MonitoringDescription("Whether the process command input is read from the Process instance's StandardInput member.")]
		public bool RedirectStandardInput
		{
			get
			{
				return this.redirectStandardInput;
			}
			set
			{
				this.redirectStandardInput = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the textual output of an application is written to the <see cref="P:System.Diagnostics.Process.StandardOutput" /> stream.</summary>
		/// <returns>
		///   <see langword="true" /> if output should be written to <see cref="P:System.Diagnostics.Process.StandardOutput" />; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[NotifyParentProperty(true)]
		[MonitoringDescription("Whether the process output is written to the Process instance's StandardOutput member.")]
		[DefaultValue(false)]
		public bool RedirectStandardOutput
		{
			get
			{
				return this.redirectStandardOutput;
			}
			set
			{
				this.redirectStandardOutput = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the error output of an application is written to the <see cref="P:System.Diagnostics.Process.StandardError" /> stream.</summary>
		/// <returns>
		///   <see langword="true" /> if error output should be written to <see cref="P:System.Diagnostics.Process.StandardError" />; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[MonitoringDescription("Whether the process's error output is written to the Process instance's StandardError member.")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		public bool RedirectStandardError
		{
			get
			{
				return this.redirectStandardError;
			}
			set
			{
				this.redirectStandardError = value;
			}
		}

		/// <summary>Gets or sets the preferred encoding for error output.</summary>
		/// <returns>An object that represents the preferred encoding for error output. The default is <see langword="null" />.</returns>
		public Encoding StandardErrorEncoding
		{
			get
			{
				return this.standardErrorEncoding;
			}
			set
			{
				this.standardErrorEncoding = value;
			}
		}

		/// <summary>Gets or sets the preferred encoding for standard output.</summary>
		/// <returns>An object that represents the preferred encoding for standard output. The default is <see langword="null" />.</returns>
		public Encoding StandardOutputEncoding
		{
			get
			{
				return this.standardOutputEncoding;
			}
			set
			{
				this.standardOutputEncoding = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to use the operating system shell to start the process.</summary>
		/// <returns>
		///   <see langword="true" /> if the shell should be used when starting the process; <see langword="false" /> if the process should be created directly from the executable file. The default is <see langword="true" /> on .NET Framework apps and <see langword="false" /> on .NET Core apps.</returns>
		/// <exception cref="T:System.PlatformNotSupportedException">An attempt to set the value to <see langword="true" /> on Universal Windows Platform (UWP) apps occurs.</exception>
		[NotifyParentProperty(true)]
		[MonitoringDescription("Whether to use the operating system shell to start the process.")]
		[DefaultValue(true)]
		public bool UseShellExecute
		{
			get
			{
				return this.useShellExecute;
			}
			set
			{
				this.useShellExecute = value;
			}
		}

		/// <summary>Gets or sets the user name to use when starting the process. If you use the UPN format, <paramref name="user" />@<paramref name="DNS_domain_name" />, the <see cref="P:System.Diagnostics.ProcessStartInfo.Domain" /> property must be <see langword="null" />.</summary>
		/// <returns>The user name to use when starting the process. If you use the UPN format, <paramref name="user" />@<paramref name="DNS_domain_name" />, the <see cref="P:System.Diagnostics.ProcessStartInfo.Domain" /> property must be <see langword="null" />.</returns>
		[NotifyParentProperty(true)]
		public string UserName
		{
			get
			{
				if (this.userName == null)
				{
					return string.Empty;
				}
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		/// <summary>Gets or sets a secure string that contains the user password to use when starting the process.</summary>
		/// <returns>The user password to use when starting the process.</returns>
		public SecureString Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
			}
		}

		/// <summary>Gets or sets the user password in clear text to use when starting the process.</summary>
		/// <returns>The user password in clear text.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string PasswordInClearText
		{
			get
			{
				return this.passwordInClearText;
			}
			set
			{
				this.passwordInClearText = value;
			}
		}

		/// <summary>Gets or sets a value that identifies the domain to use when starting the process. If this value is <see langword="null" />, the <see cref="P:System.Diagnostics.ProcessStartInfo.UserName" /> property must be specified in UPN format.</summary>
		/// <returns>The Active Directory domain to use when starting the process. If this value is <see langword="null" />, the <see cref="P:System.Diagnostics.ProcessStartInfo.UserName" /> property must be specified in UPN format.</returns>
		[NotifyParentProperty(true)]
		public string Domain
		{
			get
			{
				if (this.domain == null)
				{
					return string.Empty;
				}
				return this.domain;
			}
			set
			{
				this.domain = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the Windows user profile is to be loaded from the registry.</summary>
		/// <returns>
		///   <see langword="true" /> if the Windows user profile should be loaded; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[NotifyParentProperty(true)]
		public bool LoadUserProfile
		{
			get
			{
				return this.loadUserProfile;
			}
			set
			{
				this.loadUserProfile = value;
			}
		}

		/// <summary>Gets or sets the application or document to start.</summary>
		/// <returns>The name of the application to start, or the name of a document of a file type that is associated with an application and that has a default open action available to it. The default is an empty string ("").</returns>
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[Editor("System.Diagnostics.Design.StartFileNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[MonitoringDescription("The name of the application, document or URL to start.")]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindable(true)]
		public string FileName
		{
			get
			{
				if (this.fileName == null)
				{
					return string.Empty;
				}
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		/// <summary>When the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property is <see langword="false" />, gets or sets the working directory for the process to be started. When <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> is <see langword="true" />, gets or sets the directory that contains the process to be started.</summary>
		/// <returns>When <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> is <see langword="true" />, the fully qualified name of the directory that contains the process to be started. When the <see cref="P:System.Diagnostics.ProcessStartInfo.UseShellExecute" /> property is <see langword="false" />, the working directory for the process to be started. The default is an empty string ("").</returns>
		[MonitoringDescription("The initial working directory for the process.")]
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[SettingsBindable(true)]
		[Editor("System.Diagnostics.Design.WorkingDirectoryEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string WorkingDirectory
		{
			get
			{
				if (this.directory == null)
				{
					return string.Empty;
				}
				return this.directory;
			}
			set
			{
				this.directory = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether an error dialog box is displayed to the user if the process cannot be started.</summary>
		/// <returns>
		///   <see langword="true" /> if an error dialog box should be displayed on the screen if the process cannot be started; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		[MonitoringDescription("Whether to show an error dialog to the user if there is an error.")]
		public bool ErrorDialog
		{
			get
			{
				return this.errorDialog;
			}
			set
			{
				this.errorDialog = value;
			}
		}

		/// <summary>Gets or sets the window handle to use when an error dialog box is shown for a process that cannot be started.</summary>
		/// <returns>A pointer to the handle of the error dialog box that results from a process start failure.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr ErrorDialogParentHandle
		{
			get
			{
				return this.errorDialogParentHandle;
			}
			set
			{
				this.errorDialogParentHandle = value;
			}
		}

		/// <summary>Gets or sets the window state to use when the process is started.</summary>
		/// <returns>One of the enumeration values that indicates whether the process is started in a window that is maximized, minimized, normal (neither maximized nor minimized), or not visible. The default is <see langword="Normal" />.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">The window style is not one of the <see cref="T:System.Diagnostics.ProcessWindowStyle" /> enumeration members.</exception>
		[DefaultValue(ProcessWindowStyle.Normal)]
		[MonitoringDescription("How the main window should be created when the process starts.")]
		[NotifyParentProperty(true)]
		public ProcessWindowStyle WindowStyle
		{
			get
			{
				return this.windowStyle;
			}
			set
			{
				if (!Enum.IsDefined(typeof(ProcessWindowStyle), value))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ProcessWindowStyle));
				}
				this.windowStyle = value;
			}
		}

		internal bool HaveEnvVars
		{
			get
			{
				return this.environmentVariables != null;
			}
		}

		public Encoding StandardInputEncoding { get; set; }

		/// <summary>Gets the set of verbs associated with the type of file specified by the <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property.</summary>
		/// <returns>The actions that the system can apply to the file indicated by the <see cref="P:System.Diagnostics.ProcessStartInfo.FileName" /> property.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string[] Verbs
		{
			get
			{
				PlatformID platform = System.Environment.OSVersion.Platform;
				if (platform == PlatformID.Unix || platform == PlatformID.MacOSX || platform == (PlatformID)128)
				{
					return ProcessStartInfo.empty;
				}
				string text = string.IsNullOrEmpty(this.fileName) ? null : Path.GetExtension(this.fileName);
				if (text == null)
				{
					return ProcessStartInfo.empty;
				}
				RegistryKey registryKey = null;
				RegistryKey registryKey2 = null;
				RegistryKey registryKey3 = null;
				string[] result;
				try
				{
					registryKey = Registry.ClassesRoot.OpenSubKey(text);
					string text2 = (registryKey != null) ? (registryKey.GetValue(null) as string) : null;
					registryKey2 = ((text2 != null) ? Registry.ClassesRoot.OpenSubKey(text2) : null);
					registryKey3 = ((registryKey2 != null) ? registryKey2.OpenSubKey("shell") : null);
					result = ((registryKey3 != null) ? registryKey3.GetSubKeyNames() : null);
				}
				finally
				{
					if (registryKey3 != null)
					{
						registryKey3.Close();
					}
					if (registryKey2 != null)
					{
						registryKey2.Close();
					}
					if (registryKey != null)
					{
						registryKey.Close();
					}
				}
				return result;
			}
		}

		private string fileName;

		private string arguments;

		private string directory;

		private string verb;

		private ProcessWindowStyle windowStyle;

		private bool errorDialog;

		private IntPtr errorDialogParentHandle;

		private bool useShellExecute = true;

		private string userName;

		private string domain;

		private SecureString password;

		private string passwordInClearText;

		private bool loadUserProfile;

		private bool redirectStandardInput;

		private bool redirectStandardOutput;

		private bool redirectStandardError;

		private Encoding standardOutputEncoding;

		private Encoding standardErrorEncoding;

		private bool createNoWindow;

		private WeakReference weakParentProcess;

		internal StringDictionary environmentVariables;

		private static readonly string[] empty = new string[0];

		private Collection<string> _argumentList;

		private IDictionary<string, string> environment;
	}
}
