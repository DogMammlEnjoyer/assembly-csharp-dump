using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.Threading;
using Mono.Security;

namespace System
{
	/// <summary>Represents an application domain, which is an isolated environment where applications execute. This class cannot be inherited.</summary>
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_AppDomain))]
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AppDomain : MarshalByRefObject, _AppDomain, IEvidenceFactory
	{
		internal static bool IsAppXModel()
		{
			return false;
		}

		internal static bool IsAppXDesignMode()
		{
			return false;
		}

		internal static void CheckReflectionOnlyLoadSupported()
		{
		}

		internal static void CheckLoadFromSupported()
		{
		}

		private AppDomain()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern AppDomainSetup getSetup();

		private AppDomainSetup SetupInformationNoCopy
		{
			get
			{
				return this.getSetup();
			}
		}

		/// <summary>Gets the application domain configuration information for this instance.</summary>
		/// <returns>The application domain initialization information.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public AppDomainSetup SetupInformation
		{
			get
			{
				return new AppDomainSetup(this.getSetup());
			}
		}

		/// <summary>Gets information describing permissions granted to an application and whether the application has a trust level that allows it to run.</summary>
		/// <returns>An object that encapsulates permission and trust information for the application in the application domain.</returns>
		[MonoTODO]
		public ApplicationTrust ApplicationTrust
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the base directory that the assembly resolver uses to probe for assemblies.</summary>
		/// <returns>The base directory that the assembly resolver uses to probe for assemblies.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public string BaseDirectory
		{
			get
			{
				string applicationBase = this.SetupInformationNoCopy.ApplicationBase;
				if (SecurityManager.SecurityEnabled && applicationBase != null && applicationBase.Length > 0)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, applicationBase).Demand();
				}
				return applicationBase;
			}
		}

		/// <summary>Gets the path under the base directory where the assembly resolver should probe for private assemblies.</summary>
		/// <returns>The path under the base directory where the assembly resolver should probe for private assemblies.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public string RelativeSearchPath
		{
			get
			{
				string privateBinPath = this.SetupInformationNoCopy.PrivateBinPath;
				if (SecurityManager.SecurityEnabled && privateBinPath != null && privateBinPath.Length > 0)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, privateBinPath).Demand();
				}
				return privateBinPath;
			}
		}

		/// <summary>Gets the directory that the assembly resolver uses to probe for dynamically created assemblies.</summary>
		/// <returns>The directory that the assembly resolver uses to probe for dynamically created assemblies.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public string DynamicDirectory
		{
			[SecuritySafeCritical]
			get
			{
				AppDomainSetup setupInformationNoCopy = this.SetupInformationNoCopy;
				if (setupInformationNoCopy.DynamicBase == null)
				{
					return null;
				}
				string text = Path.Combine(setupInformationNoCopy.DynamicBase, setupInformationNoCopy.ApplicationName);
				if (SecurityManager.SecurityEnabled && text != null && text.Length > 0)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text).Demand();
				}
				return text;
			}
		}

		/// <summary>Gets an indication whether the application domain is configured to shadow copy files.</summary>
		/// <returns>
		///   <see langword="true" /> if the application domain is configured to shadow copy files; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public bool ShadowCopyFiles
		{
			get
			{
				return this.SetupInformationNoCopy.ShadowCopyFiles == "true";
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string getFriendlyName();

		/// <summary>Gets the friendly name of this application domain.</summary>
		/// <returns>The friendly name of this application domain.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public string FriendlyName
		{
			[SecuritySafeCritical]
			get
			{
				return this.getFriendlyName();
			}
		}

		/// <summary>Gets the <see cref="T:System.Security.Policy.Evidence" /> associated with this application domain.</summary>
		/// <returns>The evidence associated with this application domain.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public Evidence Evidence
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
			get
			{
				if (this._evidence == null)
				{
					lock (this)
					{
						Assembly entryAssembly = Assembly.GetEntryAssembly();
						if (entryAssembly == null)
						{
							if (this == AppDomain.DefaultDomain)
							{
								return new Evidence();
							}
							this._evidence = AppDomain.DefaultDomain.Evidence;
						}
						else
						{
							this._evidence = Evidence.GetDefaultHostEvidence(entryAssembly);
						}
					}
				}
				return new Evidence(this._evidence);
			}
		}

		internal IPrincipal DefaultPrincipal
		{
			get
			{
				if (AppDomain._principal == null)
				{
					PrincipalPolicy principalPolicy = this._principalPolicy;
					if (principalPolicy != PrincipalPolicy.UnauthenticatedPrincipal)
					{
						if (principalPolicy == PrincipalPolicy.WindowsPrincipal)
						{
							AppDomain._principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
						}
					}
					else
					{
						AppDomain._principal = new GenericPrincipal(new GenericIdentity(string.Empty, string.Empty), null);
					}
				}
				return AppDomain._principal;
			}
		}

		internal PermissionSet GrantedPermissionSet
		{
			get
			{
				return this._granted;
			}
		}

		/// <summary>Gets the permission set of a sandboxed application domain.</summary>
		/// <returns>The permission set of the sandboxed application domain.</returns>
		public PermissionSet PermissionSet
		{
			get
			{
				PermissionSet result;
				if ((result = this._granted) == null)
				{
					result = (this._granted = new PermissionSet(PermissionState.Unrestricted));
				}
				return result;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain getCurDomain();

		/// <summary>Gets the current application domain for the current <see cref="T:System.Threading.Thread" />.</summary>
		/// <returns>The current application domain.</returns>
		public static AppDomain CurrentDomain
		{
			get
			{
				return AppDomain.getCurDomain();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain getRootDomain();

		internal static AppDomain DefaultDomain
		{
			get
			{
				if (AppDomain.default_domain == null)
				{
					AppDomain rootDomain = AppDomain.getRootDomain();
					if (rootDomain == AppDomain.CurrentDomain)
					{
						AppDomain.default_domain = rootDomain;
					}
					else
					{
						AppDomain.default_domain = (AppDomain)RemotingServices.GetDomainProxy(rootDomain);
					}
				}
				return AppDomain.default_domain;
			}
		}

		/// <summary>Appends the specified directory name to the private path list.</summary>
		/// <param name="path">The name of the directory to be appended to the private path.</param>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("AppDomain.AppendPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead.")]
		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void AppendPrivatePath(string path)
		{
			if (path == null || path.Length == 0)
			{
				return;
			}
			AppDomainSetup setupInformationNoCopy = this.SetupInformationNoCopy;
			string text = setupInformationNoCopy.PrivateBinPath;
			if (text == null || text.Length == 0)
			{
				setupInformationNoCopy.PrivateBinPath = path;
				return;
			}
			text = text.Trim();
			if (text[text.Length - 1] != Path.PathSeparator)
			{
				text += Path.PathSeparator.ToString();
			}
			setupInformationNoCopy.PrivateBinPath = text + path;
		}

		/// <summary>Resets the path that specifies the location of private assemblies to the empty string ("").</summary>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("AppDomain.ClearPrivatePath has been deprecated. Please investigate the use of AppDomainSetup.PrivateBinPath instead.")]
		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void ClearPrivatePath()
		{
			this.SetupInformationNoCopy.PrivateBinPath = string.Empty;
		}

		/// <summary>Resets the list of directories containing shadow copied assemblies to the empty string ("").</summary>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecurityCritical]
		[Obsolete("Use AppDomainSetup.ShadowCopyDirectories")]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void ClearShadowCopyPath()
		{
			this.SetupInformationNoCopy.ShadowCopyDirectories = string.Empty;
		}

		/// <summary>Creates a new instance of a specified COM type. Parameters specify the name of a file that contains an assembly containing the type and the name of the type.</summary>
		/// <param name="assemblyName">The name of a file containing an assembly that defines the requested type.</param>
		/// <param name="typeName">The name of the requested type.</param>
		/// <returns>An object that is a wrapper for the new instance specified by <paramref name="typeName" />. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.TypeLoadException">The type cannot be loaded.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.MissingMethodException">No public parameterless constructor was found.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.MemberAccessException">
		///   <paramref name="typeName" /> is an abstract class.  
		/// -or-  
		/// This member was invoked with a late-binding mechanism.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="assemblyName" /> is an empty string ("").</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NullReferenceException">The COM object that is being referred to is <see langword="null" />.</exception>
		public ObjectHandle CreateComInstanceFrom(string assemblyName, string typeName)
		{
			return Activator.CreateComInstanceFrom(assemblyName, typeName);
		}

		/// <summary>Creates a new instance of a specified COM type. Parameters specify the name of a file that contains an assembly containing the type and the name of the type.</summary>
		/// <param name="assemblyFile">The name of a file containing an assembly that defines the requested type.</param>
		/// <param name="typeName">The name of the requested type.</param>
		/// <param name="hashValue">Represents the value of the computed hash code.</param>
		/// <param name="hashAlgorithm">Represents the hash algorithm used by the assembly manifest.</param>
		/// <returns>An object that is a wrapper for the new instance specified by <paramref name="typeName" />. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.TypeLoadException">The type cannot be loaded.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.MissingMethodException">No public parameterless constructor was found.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.MemberAccessException">
		///   <paramref name="typeName" /> is an abstract class.  
		/// -or-  
		/// This member was invoked with a late-binding mechanism.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="assemblyFile" /> is the empty string ("").</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NullReferenceException">The COM object that is being referred to is <see langword="null" />.</exception>
		public ObjectHandle CreateComInstanceFrom(string assemblyFile, string typeName, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			return Activator.CreateComInstanceFrom(assemblyFile, typeName, hashValue, hashAlgorithm);
		}

		internal ObjectHandle InternalCreateInstanceWithNoSecurity(string assemblyName, string typeName)
		{
			return this.CreateInstance(assemblyName, typeName);
		}

		internal ObjectHandle InternalCreateInstanceWithNoSecurity(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			return this.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		internal ObjectHandle InternalCreateInstanceFromWithNoSecurity(string assemblyName, string typeName)
		{
			return this.CreateInstanceFrom(assemblyName, typeName);
		}

		internal ObjectHandle InternalCreateInstanceFromWithNoSecurity(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			return this.CreateInstanceFrom(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <returns>An object that is a wrapper for the new instance specified by <paramref name="typeName" />. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		public ObjectHandle CreateInstance(string assemblyName, string typeName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly. A parameter specifies an array of activation attributes.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects.Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>An object that is a wrapper for the new instance specified by <paramref name="typeName" />. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		public ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName, activationAttributes);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly. Parameters specify a binder, binding flags, constructor arguments, culture-specific information used to interpret arguments, activation attributes, and authorization to create the type.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects using reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">Culture-specific information that governs the coercion of <paramref name="args" /> to the formal types declared for the <paramref name="typeName" /> constructor. If <paramref name="culture" /> is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects.Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <param name="securityAttributes">Information used to authorize creation of <paramref name="typeName" />.</param>
		/// <returns>An object that is a wrapper for the new instance specified by <paramref name="typeName" />. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.  
		///  -or-  
		///  <paramref name="securityAttributes" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="securityAttributes" /> should be <see langword="null." /></exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		/// <summary>Creates a new instance of the specified type. Parameters specify the assembly where the type is defined, and the name of the type.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <returns>An instance of the object specified by <paramref name="typeName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public object CreateInstanceAndUnwrap(string assemblyName, string typeName)
		{
			ObjectHandle objectHandle = this.CreateInstance(assemblyName, typeName);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type. Parameters specify the assembly where the type is defined, the name of the type, and an array of activation attributes.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects.Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>An instance of the object specified by <paramref name="typeName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public object CreateInstanceAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
		{
			ObjectHandle objectHandle = this.CreateInstance(assemblyName, typeName, activationAttributes);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type. Parameters specify the name of the type, and how it is found and created.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects using reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">A culture-specific object used to govern the coercion of types. If <paramref name="culture" /> is <see langword="null" />, the <see langword="CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <param name="securityAttributes">Information used to authorize creation of <paramref name="typeName" />.</param>
		/// <returns>An instance of the object specified by <paramref name="typeName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public object CreateInstanceAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			ObjectHandle objectHandle = this.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly. Parameters specify a binder, binding flags, constructor arguments, culture-specific information used to interpret arguments, and optional activation attributes.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects using reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">Culture-specific information that governs the coercion of <paramref name="args" /> to the formal types declared for the <paramref name="typeName" /> constructor. If <paramref name="culture" /> is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>An object that is a wrapper for the new instance specified by <paramref name="typeName" />. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// <paramref name="assemblyName" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		public ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			return Activator.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, null);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly, specifying whether the case of the type name is ignored; the binding attributes and the binder that are used to select the type to be created; the arguments of the constructor; the culture; and the activation attributes.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects using reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">A culture-specific object used to govern the coercion of types. If <paramref name="culture" /> is <see langword="null" />, the <see langword="CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object. that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>An instance of the object specified by <paramref name="typeName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> or <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching constructor was found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typename" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have permission to call this constructor.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// <paramref name="assemblyName" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public object CreateInstanceAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			ObjectHandle objectHandle = this.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyFile">The name, including the path, of a file that contains an assembly that defines the requested type. The assembly is loaded using the <see cref="M:System.Reflection.Assembly.LoadFrom(System.String)" /> method.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects through reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">Culture-specific information that governs the coercion of <paramref name="args" /> to the formal types declared for the <paramref name="typeName" /> constructor. If <paramref name="culture" /> is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>An object that is a wrapper for the new instance, or <see langword="null" /> if <paramref name="typeName" /> is not found. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyFile" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// <paramref name="assemblyFile" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, null);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file, specifying whether the case of the type name is ignored; the binding attributes and the binder that are used to select the type to be created; the arguments of the constructor; the culture; and the activation attributes.</summary>
		/// <param name="assemblyFile">The file name and path of the assembly that defines the requested type.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects through reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">Culture-specific information that governs the coercion of <paramref name="args" /> to the formal types declared for the <paramref name="typeName" /> constructor. If <paramref name="culture" /> is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>The requested object, or <see langword="null" /> if <paramref name="typeName" /> is not found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// <paramref name="assemblyName" /> was compiled with a later version of the common language runtime that the version that is currently loaded.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public object CreateInstanceFromAndUnwrap(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			ObjectHandle objectHandle = this.CreateInstanceFrom(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyFile">The name, including the path, of a file that contains an assembly that defines the requested type. The assembly is loaded using the <see cref="M:System.Reflection.Assembly.LoadFrom(System.String)" /> method.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <returns>An object that is a wrapper for the new instance, or <see langword="null" /> if <paramref name="typeName" /> is not found. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyFile" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.MissingMethodException">No parameterless public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyFile">The name, including the path, of a file that contains an assembly that defines the requested type. The assembly is loaded using the <see cref="M:System.Reflection.Assembly.LoadFrom(System.String)" /> method.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects.Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>An object that is a wrapper for the new instance, or <see langword="null" /> if <paramref name="typeName" /> is not found. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyFile" />.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName, activationAttributes);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyFile">The name, including the path, of a file that contains an assembly that defines the requested type. The assembly is loaded using the <see cref="M:System.Reflection.Assembly.LoadFrom(System.String)" /> method.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects through reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">Culture-specific information that governs the coercion of <paramref name="args" /> to the formal types declared for the <paramref name="typeName" /> constructor. If <paramref name="culture" /> is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <param name="securityAttributes">Information used to authorize creation of <paramref name="typeName" />.</param>
		/// <returns>An object that is a wrapper for the new instance, or <see langword="null" /> if <paramref name="typeName" /> is not found. The return value needs to be unwrapped to access the real object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.  
		///  -or-  
		///  <paramref name="securityAttributes" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="securityAttributes" /> should be <see langword="null" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyFile" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NullReferenceException">This instance is <see langword="null" />.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Activator.CreateInstanceFrom(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyName">The file name and path of the assembly that defines the requested type.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <returns>The requested object, or <see langword="null" /> if <paramref name="typeName" /> is not found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No parameterless public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName)
		{
			ObjectHandle objectHandle = this.CreateInstanceFrom(assemblyName, typeName);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyName">The file name and path of the assembly that defines the requested type.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly (see the <see cref="P:System.Type.FullName" /> property).</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects.Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <returns>The requested object, or <see langword="null" /> if <paramref name="typeName" /> is not found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No parameterless public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName, object[] activationAttributes)
		{
			ObjectHandle objectHandle = this.CreateInstanceFrom(assemblyName, typeName, activationAttributes);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Creates a new instance of the specified type defined in the specified assembly file.</summary>
		/// <param name="assemblyName">The file name and path of the assembly that defines the requested type.</param>
		/// <param name="typeName">The fully qualified name of the requested type, including the namespace but not the assembly, as returned by the <see cref="P:System.Type.FullName" /> property.</param>
		/// <param name="ignoreCase">A Boolean value specifying whether to perform a case-sensitive search or not.</param>
		/// <param name="bindingAttr">A combination of zero or more bit flags that affect the search for the <paramref name="typeName" /> constructor. If <paramref name="bindingAttr" /> is zero, a case-sensitive search for public constructors is conducted.</param>
		/// <param name="binder">An object that enables the binding, coercion of argument types, invocation of members, and retrieval of <see cref="T:System.Reflection.MemberInfo" /> objects through reflection. If <paramref name="binder" /> is null, the default binder is used.</param>
		/// <param name="args">The arguments to pass to the constructor. This array of arguments must match in number, order, and type the parameters of the constructor to invoke. If the default constructor is preferred, <paramref name="args" /> must be an empty array or null.</param>
		/// <param name="culture">Culture-specific information that governs the coercion of <paramref name="args" /> to the formal types declared for the <paramref name="typeName" /> constructor. If <paramref name="culture" /> is <see langword="null" />, the <see cref="T:System.Globalization.CultureInfo" /> for the current thread is used.</param>
		/// <param name="activationAttributes">An array of one or more attributes that can participate in activation. Typically, an array that contains a single <see cref="T:System.Runtime.Remoting.Activation.UrlAttribute" /> object that specifies the URL that is required to activate a remote object.  
		///  This parameter is related to client-activated objects. Client activation is a legacy technology that is retained for backward compatibility but is not recommended for new development. Distributed applications should instead use Windows Communication Foundation.</param>
		/// <param name="securityAttributes">Information used to authorize creation of <paramref name="typeName" />.</param>
		/// <returns>The requested object, or <see langword="null" /> if <paramref name="typeName" /> is not found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The caller cannot provide activation attributes for an object that does not inherit from <see cref="T:System.MarshalByRefObject" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyName" /> was not found.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> was not found in <paramref name="assemblyName" />.</exception>
		/// <exception cref="T:System.MissingMethodException">No matching public constructor was found.</exception>
		/// <exception cref="T:System.MethodAccessException">The caller does not have sufficient permission to call this constructor.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyName" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public object CreateInstanceFromAndUnwrap(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			ObjectHandle objectHandle = this.CreateInstanceFrom(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
			if (objectHandle == null)
			{
				return null;
			}
			return objectHandle.Unwrap();
		}

		/// <summary>Defines a dynamic assembly with the specified name and access mode.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The access mode for the dynamic assembly.</param>
		/// <returns>A dynamic assembly with the specified name and access mode.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
		{
			return this.DefineDynamicAssembly(name, access, null, null, null, null, null, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, and evidence.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="evidence">The evidence supplied for the dynamic assembly. The evidence is used unaltered as the final set of evidence used for policy resolution.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence)
		{
			return this.DefineDynamicAssembly(name, access, null, evidence, null, null, null, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, and storage directory.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the directory defaults to the current directory.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir)
		{
			return this.DefineDynamicAssembly(name, access, dir, null, null, null, null, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, storage directory, and evidence.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the directory defaults to the current directory.</param>
		/// <param name="evidence">The evidence supplied for the dynamic assembly. The evidence is used unaltered as the final set of evidence used for policy resolution.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence)
		{
			return this.DefineDynamicAssembly(name, access, dir, evidence, null, null, null, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, and permission requests.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="requiredPermissions">The required permissions request.</param>
		/// <param name="optionalPermissions">The optional permissions request.</param>
		/// <param name="refusedPermissions">The refused permissions request.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return this.DefineDynamicAssembly(name, access, null, null, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, evidence, and permission requests.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="evidence">The evidence supplied for the dynamic assembly. The evidence is used unaltered as the final set of evidence used for policy resolution.</param>
		/// <param name="requiredPermissions">The required permissions request.</param>
		/// <param name="optionalPermissions">The optional permissions request.</param>
		/// <param name="refusedPermissions">The refused permissions request.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return this.DefineDynamicAssembly(name, access, null, evidence, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, storage directory, and permission requests.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the directory defaults to the current directory.</param>
		/// <param name="requiredPermissions">The required permissions request.</param>
		/// <param name="optionalPermissions">The optional permissions request.</param>
		/// <param name="refusedPermissions">The refused permissions request.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecuritySafeCritical]
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return this.DefineDynamicAssembly(name, access, dir, null, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, storage directory, evidence, and permission requests.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the directory defaults to the current directory.</param>
		/// <param name="evidence">The evidence supplied for the dynamic assembly. The evidence is used unaltered as the final set of evidence used for policy resolution.</param>
		/// <param name="requiredPermissions">The required permissions request.</param>
		/// <param name="optionalPermissions">The optional permissions request.</param>
		/// <param name="refusedPermissions">The refused permissions request.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions)
		{
			return this.DefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, false);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, storage directory, evidence, permission requests, and synchronization option.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the dynamic assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the directory defaults to the current directory.</param>
		/// <param name="evidence">The evidence supplied for the dynamic assembly. The evidence is used unaltered as the final set of evidence used for policy resolution.</param>
		/// <param name="requiredPermissions">The required permissions request.</param>
		/// <param name="optionalPermissions">The optional permissions request.</param>
		/// <param name="refusedPermissions">The refused permissions request.</param>
		/// <param name="isSynchronized">
		///   <see langword="true" /> to synchronize the creation of modules, types, and members in the dynamic assembly; otherwise, <see langword="false" />.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> begins with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		[SecuritySafeCritical]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			AppDomain.ValidateAssemblyName(name.Name);
			AssemblyBuilder assemblyBuilder = new AssemblyBuilder(name, dir, access, false);
			assemblyBuilder.AddPermissionRequests(requiredPermissions, optionalPermissions, refusedPermissions);
			return assemblyBuilder;
		}

		/// <summary>Defines a dynamic assembly with the specified name, access mode, storage directory, evidence, permission requests, synchronization option, and custom attributes.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the dynamic assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the current directory is used.</param>
		/// <param name="evidence">The evidence that is supplied for the dynamic assembly. The evidence is used unaltered as the final set of evidence used for policy resolution.</param>
		/// <param name="requiredPermissions">The required permissions request.</param>
		/// <param name="optionalPermissions">The optional permissions request.</param>
		/// <param name="refusedPermissions">The refused permissions request.</param>
		/// <param name="isSynchronized">
		///   <see langword="true" /> to synchronize the creation of modules, types, and members in the dynamic assembly; otherwise, <see langword="false" />.</param>
		/// <param name="assemblyAttributes">An enumerable list of attributes to be applied to the assembly, or <see langword="null" /> if there are no attributes.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> starts with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Declarative security for assembly level is no longer enforced")]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
		{
			AssemblyBuilder assemblyBuilder = this.DefineDynamicAssembly(name, access, dir, evidence, requiredPermissions, optionalPermissions, refusedPermissions, isSynchronized);
			if (assemblyAttributes != null)
			{
				foreach (CustomAttributeBuilder customAttribute in assemblyAttributes)
				{
					assemblyBuilder.SetCustomAttribute(customAttribute);
				}
			}
			return assemblyBuilder;
		}

		/// <summary>Defines a dynamic assembly with the specified name, access mode, and custom attributes.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The access mode for the dynamic assembly.</param>
		/// <param name="assemblyAttributes">An enumerable list of attributes to be applied to the assembly, or <see langword="null" /> if there are no attributes.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> starts with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
		{
			return this.DefineDynamicAssembly(name, access, null, null, null, null, null, false, assemblyAttributes);
		}

		/// <summary>Defines a dynamic assembly using the specified name, access mode, storage directory, and synchronization option.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The mode in which the dynamic assembly will be accessed.</param>
		/// <param name="dir">The name of the directory where the dynamic assembly will be saved. If <paramref name="dir" /> is <see langword="null" />, the current directory is used.</param>
		/// <param name="isSynchronized">
		///   <see langword="true" /> to synchronize the creation of modules, types, and members in the dynamic assembly; otherwise, <see langword="false" />.</param>
		/// <param name="assemblyAttributes">An enumerable list of attributes to be applied to the assembly, or <see langword="null" /> if there are no attributes.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> starts with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, bool isSynchronized, IEnumerable<CustomAttributeBuilder> assemblyAttributes)
		{
			return this.DefineDynamicAssembly(name, access, dir, null, null, null, null, isSynchronized, assemblyAttributes);
		}

		/// <summary>Defines a dynamic assembly with the specified name, access mode, and custom attributes, and using the specified source for its security context.</summary>
		/// <param name="name">The unique identity of the dynamic assembly.</param>
		/// <param name="access">The access mode for the dynamic assembly.</param>
		/// <param name="assemblyAttributes">An enumerable list of attributes to be applied to the assembly, or <see langword="null" /> if there are no attributes.</param>
		/// <param name="securityContextSource">The source of the security context.</param>
		/// <returns>A dynamic assembly with the specified name and features.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see langword="Name" /> property of <paramref name="name" /> is <see langword="null" />.  
		///  -or-  
		///  The <see langword="Name" /> property of <paramref name="name" /> starts with white space, or contains a forward or backward slash.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value of <paramref name="securityContextSource" /> was not one of the enumeration values.</exception>
		[MonoLimitation("The argument securityContextSource is ignored")]
		public AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, IEnumerable<CustomAttributeBuilder> assemblyAttributes, SecurityContextSource securityContextSource)
		{
			return this.DefineDynamicAssembly(name, access, assemblyAttributes);
		}

		internal AssemblyBuilder DefineInternalDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access)
		{
			return new AssemblyBuilder(name, null, access, true);
		}

		/// <summary>Executes the code in another application domain that is identified by the specified delegate.</summary>
		/// <param name="callBackDelegate">A delegate that specifies a method to call.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="callBackDelegate" /> is <see langword="null" />.</exception>
		public void DoCallBack(CrossAppDomainDelegate callBackDelegate)
		{
			if (callBackDelegate != null)
			{
				callBackDelegate();
			}
		}

		/// <summary>Executes the assembly contained in the specified file.</summary>
		/// <param name="assemblyFile">The name of the file that contains the assembly to execute.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		public int ExecuteAssembly(string assemblyFile)
		{
			return this.ExecuteAssembly(assemblyFile, null, null);
		}

		/// <summary>Executes the assembly contained in the specified file, using the specified evidence.</summary>
		/// <param name="assemblyFile">The name of the file that contains the assembly to execute.</param>
		/// <param name="assemblySecurity">Evidence for loading the assembly.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity)
		{
			return this.ExecuteAssembly(assemblyFile, assemblySecurity, null);
		}

		/// <summary>Executes the assembly contained in the specified file, using the specified evidence and arguments.</summary>
		/// <param name="assemblyFile">The name of the file that contains the assembly to execute.</param>
		/// <param name="assemblySecurity">The supplied evidence for the assembly.</param>
		/// <param name="args">The arguments to the entry point of the assembly.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="assemblySecurity" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="assemblySecurity" /> should be <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity, string[] args)
		{
			Assembly a = Assembly.LoadFrom(assemblyFile, assemblySecurity);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Executes the assembly contained in the specified file, using the specified evidence, arguments, hash value, and hash algorithm.</summary>
		/// <param name="assemblyFile">The name of the file that contains the assembly to execute.</param>
		/// <param name="assemblySecurity">The supplied evidence for the assembly.</param>
		/// <param name="args">The arguments to the entry point of the assembly.</param>
		/// <param name="hashValue">Represents the value of the computed hash code.</param>
		/// <param name="hashAlgorithm">Represents the hash algorithm used by the assembly manifest.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyFile" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="assemblySecurity" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="assemblySecurity" /> should be <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity, string[] args, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			Assembly a = Assembly.LoadFrom(assemblyFile, assemblySecurity, hashValue, hashAlgorithm);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Executes the assembly contained in the specified file, using the specified arguments.</summary>
		/// <param name="assemblyFile">The name of the file that contains the assembly to execute.</param>
		/// <param name="args">The arguments to the entry point of the assembly.</param>
		/// <returns>The value that is returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// <paramref name="assemblyFile" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		public int ExecuteAssembly(string assemblyFile, string[] args)
		{
			Assembly a = Assembly.LoadFrom(assemblyFile, null);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Executes the assembly contained in the specified file, using the specified arguments, hash value, and hash algorithm.</summary>
		/// <param name="assemblyFile">The name of the file that contains the assembly to execute.</param>
		/// <param name="args">The arguments to the entry point of the assembly.</param>
		/// <param name="hashValue">Represents the value of the computed hash code.</param>
		/// <param name="hashAlgorithm">Represents the hash algorithm used by the assembly manifest.</param>
		/// <returns>The value that is returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyFile" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyFile" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyFile" /> is not a valid assembly.  
		/// -or-  
		/// <paramref name="assemblyFile" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		public int ExecuteAssembly(string assemblyFile, string[] args, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			Assembly a = Assembly.LoadFrom(assemblyFile, null, hashValue, hashAlgorithm);
			return this.ExecuteAssemblyInternal(a, args);
		}

		private int ExecuteAssemblyInternal(Assembly a, string[] args)
		{
			if (a.EntryPoint == null)
			{
				throw new MissingMethodException("Entry point not found in assembly '" + a.FullName + "'.");
			}
			return this.ExecuteAssembly(a, args);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int ExecuteAssembly(Assembly a, string[] args);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Assembly[] GetAssemblies(bool refOnly);

		/// <summary>Gets the assemblies that have been loaded into the execution context of this application domain.</summary>
		/// <returns>An array of assemblies in this application domain.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		public Assembly[] GetAssemblies()
		{
			return this.GetAssemblies(false);
		}

		/// <summary>Gets the value stored in the current application domain for the specified name.</summary>
		/// <param name="name">The name of a predefined application domain property, or the name of an application domain property you have defined.</param>
		/// <returns>The value of the <paramref name="name" /> property, or <see langword="null" /> if the property does not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern object GetData(string name);

		/// <summary>Gets the type of the current instance.</summary>
		/// <returns>The type of the current instance.</returns>
		public new Type GetType()
		{
			return base.GetType();
		}

		/// <summary>Gives the <see cref="T:System.AppDomain" /> an infinite lifetime by preventing a lease from being created.</summary>
		/// <returns>Always <see langword="null" />.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecurityCritical]
		public override object InitializeLifetimeService()
		{
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Assembly LoadAssembly(string assemblyRef, Evidence securityEvidence, bool refOnly, ref StackCrawlMark stackMark);

		/// <summary>Loads an <see cref="T:System.Reflection.Assembly" /> given its <see cref="T:System.Reflection.AssemblyName" />.</summary>
		/// <param name="assemblyRef">An object that describes the assembly to load.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyRef" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyRef" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyRef" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyRef" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[SecuritySafeCritical]
		public Assembly Load(AssemblyName assemblyRef)
		{
			return this.Load(assemblyRef, null);
		}

		internal Assembly LoadSatellite(AssemblyName assemblyRef, bool throwOnError, ref StackCrawlMark stackMark)
		{
			if (assemblyRef == null)
			{
				throw new ArgumentNullException("assemblyRef");
			}
			Assembly assembly = this.LoadAssembly(assemblyRef.FullName, null, false, ref stackMark);
			if (assembly == null && throwOnError)
			{
				throw new FileNotFoundException(null, assemblyRef.Name);
			}
			return assembly;
		}

		/// <summary>Loads an <see cref="T:System.Reflection.Assembly" /> given its <see cref="T:System.Reflection.AssemblyName" />.</summary>
		/// <param name="assemblyRef">An object that describes the assembly to load.</param>
		/// <param name="assemblySecurity">Evidence for loading the assembly.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyRef" /> is <see langword="null" /></exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyRef" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyRef" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyRef" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity)
		{
			if (assemblyRef == null)
			{
				throw new ArgumentNullException("assemblyRef");
			}
			if (assemblyRef.Name == null || assemblyRef.Name.Length == 0)
			{
				if (assemblyRef.CodeBase != null)
				{
					return Assembly.LoadFrom(assemblyRef.CodeBase, assemblySecurity);
				}
				throw new ArgumentException(Locale.GetText("assemblyRef.Name cannot be empty."), "assemblyRef");
			}
			else
			{
				StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
				Assembly assembly = this.LoadAssembly(assemblyRef.FullName, assemblySecurity, false, ref stackCrawlMark);
				if (assembly != null)
				{
					return assembly;
				}
				if (assemblyRef.CodeBase == null)
				{
					throw new FileNotFoundException(null, assemblyRef.Name);
				}
				string text = assemblyRef.CodeBase;
				if (text.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
				{
					text = new Uri(text).LocalPath;
				}
				try
				{
					assembly = Assembly.LoadFrom(text, assemblySecurity);
				}
				catch
				{
					throw new FileNotFoundException(null, assemblyRef.Name);
				}
				AssemblyName name = assembly.GetName();
				if (assemblyRef.Name != name.Name)
				{
					throw new FileNotFoundException(null, assemblyRef.Name);
				}
				if (assemblyRef.Version != null && assemblyRef.Version != new Version(0, 0, 0, 0) && assemblyRef.Version != name.Version)
				{
					throw new FileNotFoundException(null, assemblyRef.Name);
				}
				if (assemblyRef.CultureInfo != null && assemblyRef.CultureInfo.Equals(name))
				{
					throw new FileNotFoundException(null, assemblyRef.Name);
				}
				byte[] publicKeyToken = assemblyRef.GetPublicKeyToken();
				if (publicKeyToken != null && publicKeyToken.Length != 0)
				{
					byte[] publicKeyToken2 = name.GetPublicKeyToken();
					if (publicKeyToken2 == null || publicKeyToken.Length != publicKeyToken2.Length)
					{
						throw new FileNotFoundException(null, assemblyRef.Name);
					}
					for (int i = publicKeyToken.Length - 1; i >= 0; i--)
					{
						if (publicKeyToken2[i] != publicKeyToken[i])
						{
							throw new FileNotFoundException(null, assemblyRef.Name);
						}
					}
				}
				return assembly;
			}
		}

		/// <summary>Loads an <see cref="T:System.Reflection.Assembly" /> given its display name.</summary>
		/// <param name="assemblyString">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyString" /> is <see langword="null" /></exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyString" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyString" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyString" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public Assembly Load(string assemblyString)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.Load(assemblyString, null, false, ref stackCrawlMark);
		}

		/// <summary>Loads an <see cref="T:System.Reflection.Assembly" /> given its display name.</summary>
		/// <param name="assemblyString">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="assemblySecurity">Evidence for loading the assembly.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyString" /> is <see langword="null" /></exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="assemblyString" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="assemblyString" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyString" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[SecuritySafeCritical]
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public Assembly Load(string assemblyString, Evidence assemblySecurity)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.Load(assemblyString, assemblySecurity, false, ref stackCrawlMark);
		}

		internal Assembly Load(string assemblyString, Evidence assemblySecurity, bool refonly, ref StackCrawlMark stackMark)
		{
			if (assemblyString == null)
			{
				throw new ArgumentNullException("assemblyString");
			}
			if (assemblyString.Length == 0)
			{
				throw new ArgumentException("assemblyString cannot have zero length");
			}
			Assembly assembly = this.LoadAssembly(assemblyString, assemblySecurity, refonly, ref stackMark);
			if (assembly == null)
			{
				throw new FileNotFoundException(null, assemblyString);
			}
			return assembly;
		}

		/// <summary>Loads the <see cref="T:System.Reflection.Assembly" /> with a common object file format (COFF) based image containing an emitted <see cref="T:System.Reflection.Assembly" />.</summary>
		/// <param name="rawAssembly">An array of type <see langword="byte" /> that is a COFF-based image containing an emitted assembly.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="rawAssembly" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="rawAssembly" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="rawAssembly" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[SecuritySafeCritical]
		public Assembly Load(byte[] rawAssembly)
		{
			return this.Load(rawAssembly, null, null);
		}

		/// <summary>Loads the <see cref="T:System.Reflection.Assembly" /> with a common object file format (COFF) based image containing an emitted <see cref="T:System.Reflection.Assembly" />. The raw bytes representing the symbols for the <see cref="T:System.Reflection.Assembly" /> are also loaded.</summary>
		/// <param name="rawAssembly">An array of type <see langword="byte" /> that is a COFF-based image containing an emitted assembly.</param>
		/// <param name="rawSymbolStore">An array of type <see langword="byte" /> containing the raw bytes representing the symbols for the assembly.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="rawAssembly" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="rawAssembly" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="rawAssembly" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		[SecuritySafeCritical]
		public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
		{
			return this.Load(rawAssembly, rawSymbolStore, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Assembly LoadAssemblyRaw(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence, bool refonly);

		/// <summary>Loads the <see cref="T:System.Reflection.Assembly" /> with a common object file format (COFF) based image containing an emitted <see cref="T:System.Reflection.Assembly" />. The raw bytes representing the symbols for the <see cref="T:System.Reflection.Assembly" /> are also loaded.</summary>
		/// <param name="rawAssembly">An array of type <see langword="byte" /> that is a COFF-based image containing an emitted assembly.</param>
		/// <param name="rawSymbolStore">An array of type <see langword="byte" /> containing the raw bytes representing the symbols for the assembly.</param>
		/// <param name="securityEvidence">Evidence for loading the assembly.</param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="rawAssembly" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.BadImageFormatException">
		///   <paramref name="rawAssembly" /> is not a valid assembly.  
		/// -or-  
		/// Version 2.0 or later of the common language runtime is currently loaded and <paramref name="rawAssembly" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">An assembly or module was loaded twice with two different evidences.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="securityEvidence" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="securityEvidence" /> should be <see langword="null" />.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
		public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence)
		{
			return this.Load(rawAssembly, rawSymbolStore, securityEvidence, false);
		}

		internal Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence, bool refonly)
		{
			if (rawAssembly == null)
			{
				throw new ArgumentNullException("rawAssembly");
			}
			Assembly assembly = this.LoadAssemblyRaw(rawAssembly, rawSymbolStore, securityEvidence, refonly);
			assembly.FromByteArray = true;
			return assembly;
		}

		/// <summary>Establishes the security policy level for this application domain.</summary>
		/// <param name="domainPolicy">The security policy level.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="domainPolicy" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Policy.PolicyException">The security policy level has already been set.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("AppDomain policy levels are obsolete")]
		[SecurityCritical]
		[SecurityPermission(SecurityAction.Demand, ControlPolicy = true)]
		public void SetAppDomainPolicy(PolicyLevel domainPolicy)
		{
			if (domainPolicy == null)
			{
				throw new ArgumentNullException("domainPolicy");
			}
			if (this._granted != null)
			{
				throw new PolicyException(Locale.GetText("An AppDomain policy is already specified."));
			}
			if (this.IsFinalizingForUnload())
			{
				throw new AppDomainUnloadedException();
			}
			PolicyStatement policyStatement = domainPolicy.Resolve(this._evidence);
			this._granted = policyStatement.PermissionSet;
		}

		/// <summary>Establishes the specified directory path as the location where assemblies are shadow copied.</summary>
		/// <param name="path">The fully qualified path to the shadow copy location.</param>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Use AppDomainSetup.SetCachePath")]
		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void SetCachePath(string path)
		{
			this.SetupInformationNoCopy.CachePath = path;
		}

		/// <summary>Specifies how principal and identity objects should be attached to a thread if the thread attempts to bind to a principal while executing in this application domain.</summary>
		/// <param name="policy">One of the <see cref="T:System.Security.Principal.PrincipalPolicy" /> values that specifies the type of the principal object to attach to threads.</param>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlPrincipal = true)]
		public void SetPrincipalPolicy(PrincipalPolicy policy)
		{
			if (this.IsFinalizingForUnload())
			{
				throw new AppDomainUnloadedException();
			}
			this._principalPolicy = policy;
			AppDomain._principal = null;
		}

		/// <summary>Turns on shadow copying.</summary>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Use AppDomainSetup.ShadowCopyFiles")]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void SetShadowCopyFiles()
		{
			this.SetupInformationNoCopy.ShadowCopyFiles = "true";
		}

		/// <summary>Establishes the specified directory path as the location of assemblies to be shadow copied.</summary>
		/// <param name="path">A list of directory names, where each name is separated by a semicolon.</param>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Use AppDomainSetup.ShadowCopyDirectories")]
		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void SetShadowCopyPath(string path)
		{
			this.SetupInformationNoCopy.ShadowCopyDirectories = path;
		}

		/// <summary>Sets the default principal object to be attached to threads if they attempt to bind to a principal while executing in this application domain.</summary>
		/// <param name="principal">The principal object to attach to threads.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="principal" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Policy.PolicyException">The thread principal has already been set.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, ControlPrincipal = true)]
		public void SetThreadPrincipal(IPrincipal principal)
		{
			if (principal == null)
			{
				throw new ArgumentNullException("principal");
			}
			if (AppDomain._principal != null)
			{
				throw new PolicyException(Locale.GetText("principal already present."));
			}
			if (this.IsFinalizingForUnload())
			{
				throw new AppDomainUnloadedException();
			}
			AppDomain._principal = principal;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain InternalSetDomainByID(int domain_id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain InternalSetDomain(AppDomain context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPushDomainRef(AppDomain domain);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPushDomainRefByID(int domain_id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalPopDomainRef();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Context InternalSetContext(Context context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Context InternalGetContext();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Context InternalGetDefaultContext();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalGetProcessGuid(string newguid);

		internal static object InvokeInDomain(AppDomain domain, MethodInfo method, object obj, object[] args)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			bool flag = false;
			object result;
			try
			{
				AppDomain.InternalPushDomainRef(domain);
				flag = true;
				AppDomain.InternalSetDomain(domain);
				Exception ex;
				object obj2 = ((RuntimeMethodInfo)method).InternalInvoke(obj, args, out ex);
				if (ex != null)
				{
					throw ex;
				}
				result = obj2;
			}
			finally
			{
				AppDomain.InternalSetDomain(currentDomain);
				if (flag)
				{
					AppDomain.InternalPopDomainRef();
				}
			}
			return result;
		}

		internal static object InvokeInDomainByID(int domain_id, MethodInfo method, object obj, object[] args)
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			bool flag = false;
			object result;
			try
			{
				AppDomain.InternalPushDomainRefByID(domain_id);
				flag = true;
				AppDomain.InternalSetDomainByID(domain_id);
				Exception ex;
				object obj2 = ((RuntimeMethodInfo)method).InternalInvoke(obj, args, out ex);
				if (ex != null)
				{
					throw ex;
				}
				result = obj2;
			}
			finally
			{
				AppDomain.InternalSetDomain(currentDomain);
				if (flag)
				{
					AppDomain.InternalPopDomainRef();
				}
			}
			return result;
		}

		internal static string GetProcessGuid()
		{
			if (AppDomain._process_guid == null)
			{
				AppDomain._process_guid = AppDomain.InternalGetProcessGuid(Guid.NewGuid().ToString());
			}
			return AppDomain._process_guid;
		}

		/// <summary>Creates a new application domain with the specified name.</summary>
		/// <param name="friendlyName">The friendly name of the domain.</param>
		/// <returns>The newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.PlatformNotSupportedException">Method not supported on .NET Core.</exception>
		public static AppDomain CreateDomain(string friendlyName)
		{
			return AppDomain.CreateDomain(friendlyName, null, null);
		}

		/// <summary>Creates a new application domain with the given name using the supplied evidence.</summary>
		/// <param name="friendlyName">The friendly name of the domain. This friendly name can be displayed in user interfaces to identify the domain. For more information, see <see cref="P:System.AppDomain.FriendlyName" />.</param>
		/// <param name="securityInfo">Evidence that establishes the identity of the code that runs in the application domain. Pass <see langword="null" /> to use the evidence of the current application domain.</param>
		/// <returns>The newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo)
		{
			return AppDomain.CreateDomain(friendlyName, securityInfo, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AppDomain createDomain(string friendlyName, AppDomainSetup info);

		/// <summary>Creates a new application domain using the specified name, evidence, and application domain setup information.</summary>
		/// <param name="friendlyName">The friendly name of the domain. This friendly name can be displayed in user interfaces to identify the domain. For more information, see <see cref="P:System.AppDomain.FriendlyName" />.</param>
		/// <param name="securityInfo">Evidence that establishes the identity of the code that runs in the application domain. Pass <see langword="null" /> to use the evidence of the current application domain.</param>
		/// <param name="info">An object that contains application domain initialization information.</param>
		/// <returns>The newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		[MonoLimitation("Currently it does not allow the setup in the other domain")]
		[SecurityPermission(SecurityAction.Demand, ControlAppDomain = true)]
		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info)
		{
			if (friendlyName == null)
			{
				throw new ArgumentNullException("friendlyName");
			}
			AppDomain defaultDomain = AppDomain.DefaultDomain;
			if (info == null)
			{
				if (defaultDomain == null)
				{
					info = new AppDomainSetup();
				}
				else
				{
					info = defaultDomain.SetupInformation;
				}
			}
			else
			{
				info = new AppDomainSetup(info);
			}
			if (defaultDomain != null)
			{
				if (!info.Equals(defaultDomain.SetupInformation))
				{
					if (info.ApplicationBase == null)
					{
						info.ApplicationBase = defaultDomain.SetupInformation.ApplicationBase;
					}
					if (info.ConfigurationFile == null)
					{
						info.ConfigurationFile = Path.GetFileName(defaultDomain.SetupInformation.ConfigurationFile);
					}
				}
			}
			else if (info.ConfigurationFile == null)
			{
				info.ConfigurationFile = "[I don't have a config file]";
			}
			if (info.AppDomainInitializer != null && !info.AppDomainInitializer.Method.IsStatic)
			{
				throw new ArgumentException("Non-static methods cannot be invoked as an appdomain initializer");
			}
			info.SerializeNonPrimitives();
			AppDomain appDomain = (AppDomain)RemotingServices.GetDomainProxy(AppDomain.createDomain(friendlyName, info));
			if (securityInfo == null)
			{
				if (defaultDomain == null)
				{
					appDomain._evidence = null;
				}
				else
				{
					appDomain._evidence = defaultDomain.Evidence;
				}
			}
			else
			{
				appDomain._evidence = new Evidence(securityInfo);
			}
			if (info.AppDomainInitializer != null)
			{
				AppDomain.Loader @object = new AppDomain.Loader(info.AppDomainInitializer.Method.DeclaringType.Assembly.Location);
				appDomain.DoCallBack(new CrossAppDomainDelegate(@object.Load));
				AppDomain.Initializer object2 = new AppDomain.Initializer(info.AppDomainInitializer, info.AppDomainInitializerArguments);
				appDomain.DoCallBack(new CrossAppDomainDelegate(object2.Initialize));
			}
			return appDomain;
		}

		/// <summary>Creates a new application domain with the given name, using evidence, application base path, relative search path, and a parameter that specifies whether a shadow copy of an assembly is to be loaded into the application domain.</summary>
		/// <param name="friendlyName">The friendly name of the domain. This friendly name can be displayed in user interfaces to identify the domain. For more information, see <see cref="P:System.AppDomain.FriendlyName" />.</param>
		/// <param name="securityInfo">Evidence that establishes the identity of the code that runs in the application domain. Pass <see langword="null" /> to use the evidence of the current application domain.</param>
		/// <param name="appBasePath">The base directory that the assembly resolver uses to probe for assemblies. For more information, see <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="appRelativeSearchPath">The path relative to the base directory where the assembly resolver should probe for private assemblies. For more information, see <see cref="P:System.AppDomain.RelativeSearchPath" />.</param>
		/// <param name="shadowCopyFiles">If <see langword="true" />, a shadow copy of an assembly is loaded into this application domain.</param>
		/// <returns>The newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
		{
			return AppDomain.CreateDomain(friendlyName, securityInfo, AppDomain.CreateDomainSetup(appBasePath, appRelativeSearchPath, shadowCopyFiles));
		}

		/// <summary>Creates a new application domain using the specified name, evidence, application domain setup information, default permission set, and array of fully trusted assemblies.</summary>
		/// <param name="friendlyName">The friendly name of the domain. This friendly name can be displayed in user interfaces to identify the domain. For more information, see the description of <see cref="P:System.AppDomain.FriendlyName" />.</param>
		/// <param name="securityInfo">Evidence that establishes the identity of the code that runs in the application domain. Pass <see langword="null" /> to use the evidence of the current application domain.</param>
		/// <param name="info">An object that contains application domain initialization information.</param>
		/// <param name="grantSet">A default permission set that is granted to all assemblies loaded into the new application domain that do not have specific grants.</param>
		/// <param name="fullTrustAssemblies">An array of strong names representing assemblies to be considered fully trusted in the new application domain.</param>
		/// <returns>The newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The application domain is <see langword="null" />.  
		///  -or-  
		///  The <see cref="P:System.AppDomainSetup.ApplicationBase" /> property is not set on the <see cref="T:System.AppDomainSetup" /> object that is supplied for <paramref name="info" />.</exception>
		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup info, PermissionSet grantSet, params System.Security.Policy.StrongName[] fullTrustAssemblies)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.ApplicationTrust = new ApplicationTrust(grantSet, fullTrustAssemblies ?? EmptyArray<System.Security.Policy.StrongName>.Value);
			return AppDomain.CreateDomain(friendlyName, securityInfo, info);
		}

		private static AppDomainSetup CreateDomainSetup(string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles)
		{
			AppDomainSetup appDomainSetup = new AppDomainSetup();
			appDomainSetup.ApplicationBase = appBasePath;
			appDomainSetup.PrivateBinPath = appRelativeSearchPath;
			if (shadowCopyFiles)
			{
				appDomainSetup.ShadowCopyFiles = "true";
			}
			else
			{
				appDomainSetup.ShadowCopyFiles = "false";
			}
			return appDomainSetup;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalIsFinalizingForUnload(int domain_id);

		/// <summary>Indicates whether this application domain is unloading, and the objects it contains are being finalized by the common language runtime.</summary>
		/// <returns>
		///   <see langword="true" /> if this application domain is unloading and the common language runtime has started invoking finalizers; otherwise, <see langword="false" />.</returns>
		public bool IsFinalizingForUnload()
		{
			return AppDomain.InternalIsFinalizingForUnload(this.getDomainID());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalUnload(int domain_id);

		private int getDomainID()
		{
			return Thread.GetDomainID();
		}

		/// <summary>Unloads the specified application domain.</summary>
		/// <param name="domain">An application domain to unload.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="domain" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.CannotUnloadAppDomainException">
		///   <paramref name="domain" /> could not be unloaded.</exception>
		/// <exception cref="T:System.Exception">An error occurred during the unload process.</exception>
		[ReliabilityContract(Consistency.MayCorruptAppDomain, Cer.MayFail)]
		[SecurityPermission(SecurityAction.Demand, ControlAppDomain = true)]
		public static void Unload(AppDomain domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			AppDomain.InternalUnload(domain.getDomainID());
		}

		/// <summary>Assigns the specified value to the specified application domain property.</summary>
		/// <param name="name">The name of a user-defined application domain property to create or change.</param>
		/// <param name="data">The value of the property.</param>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetData(string name, object data);

		/// <summary>Assigns the specified value to the specified application domain property, with a specified permission to demand of the caller when the property is retrieved.</summary>
		/// <param name="name">The name of a user-defined application domain property to create or change.</param>
		/// <param name="data">The value of the property.</param>
		/// <param name="permission">The permission to demand of the caller when the property is retrieved.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="name" /> specifies a system-defined property string and <paramref name="permission" /> is not <see langword="null" />.</exception>
		[MonoLimitation("The permission field is ignored")]
		public void SetData(string name, object data, IPermission permission)
		{
			this.SetData(name, data);
		}

		/// <summary>Establishes the specified directory path as the base directory for subdirectories where dynamically generated files are stored and accessed.</summary>
		/// <param name="path">The fully qualified path that is the base directory for subdirectories where dynamic assemblies are stored.</param>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		[Obsolete("Use AppDomainSetup.DynamicBase")]
		[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public void SetDynamicBase(string path)
		{
			this.SetupInformationNoCopy.DynamicBase = path;
		}

		/// <summary>Gets the current thread identifier.</summary>
		/// <returns>A 32-bit signed integer that is the identifier of the current thread.</returns>
		[Obsolete("AppDomain.GetCurrentThreadId has been deprecated because it does not provide a stable Id when managed threads are running on fibers (aka lightweight threads). To get a stable identifier for a managed thread, use the ManagedThreadId property on Thread.'")]
		public static int GetCurrentThreadId()
		{
			return Thread.CurrentThreadId;
		}

		/// <summary>Obtains a string representation that includes the friendly name of the application domain and any context policies.</summary>
		/// <returns>A string formed by concatenating the literal string "Name:", the friendly name of the application domain, and either string representations of the context policies or the string "There are no context policies."</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">The application domain represented by the current <see cref="T:System.AppDomain" /> has been unloaded.</exception>
		[SecuritySafeCritical]
		public override string ToString()
		{
			return this.getFriendlyName();
		}

		private static void ValidateAssemblyName(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw new ArgumentException("The Name of AssemblyName cannot be null or a zero-length string.");
			}
			bool flag = true;
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				if (i == 0 && char.IsWhiteSpace(c))
				{
					flag = false;
					break;
				}
				if (c == '/' || c == '\\' || c == ':')
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				throw new ArgumentException("The Name of AssemblyName cannot start with whitespace, or contain '/', '\\'  or ':'.");
			}
		}

		private void DoAssemblyLoad(Assembly assembly)
		{
			if (this.AssemblyLoad == null)
			{
				return;
			}
			this.AssemblyLoad(this, new AssemblyLoadEventArgs(assembly));
		}

		private Assembly DoAssemblyResolve(string name, Assembly requestingAssembly, bool refonly)
		{
			ResolveEventHandler resolveEventHandler;
			if (refonly)
			{
				resolveEventHandler = this.ReflectionOnlyAssemblyResolve;
			}
			else
			{
				resolveEventHandler = this.AssemblyResolve;
			}
			if (resolveEventHandler == null)
			{
				return null;
			}
			Dictionary<string, object> dictionary;
			if (refonly)
			{
				dictionary = AppDomain.assembly_resolve_in_progress_refonly;
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>();
					AppDomain.assembly_resolve_in_progress_refonly = dictionary;
				}
			}
			else
			{
				dictionary = AppDomain.assembly_resolve_in_progress;
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>();
					AppDomain.assembly_resolve_in_progress = dictionary;
				}
			}
			if (dictionary.ContainsKey(name))
			{
				return null;
			}
			dictionary[name] = null;
			Assembly result;
			try
			{
				Delegate[] invocationList = resolveEventHandler.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Assembly assembly = ((ResolveEventHandler)invocationList[i])(this, new ResolveEventArgs(name, requestingAssembly));
					if (assembly != null)
					{
						return assembly;
					}
				}
				result = null;
			}
			finally
			{
				dictionary.Remove(name);
			}
			return result;
		}

		internal Assembly DoTypeBuilderResolve(TypeBuilder tb)
		{
			if (this.TypeResolve == null)
			{
				return null;
			}
			return this.DoTypeResolve(tb.FullName);
		}

		internal Assembly DoTypeResolve(string name)
		{
			if (this.TypeResolve == null)
			{
				return null;
			}
			Dictionary<string, object> dictionary = AppDomain.type_resolve_in_progress;
			if (dictionary == null)
			{
				dictionary = (AppDomain.type_resolve_in_progress = new Dictionary<string, object>());
			}
			if (dictionary.ContainsKey(name))
			{
				return null;
			}
			dictionary[name] = null;
			Assembly result;
			try
			{
				Delegate[] invocationList = this.TypeResolve.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Assembly assembly = ((ResolveEventHandler)invocationList[i])(this, new ResolveEventArgs(name));
					if (assembly != null)
					{
						return assembly;
					}
				}
				result = null;
			}
			finally
			{
				dictionary.Remove(name);
			}
			return result;
		}

		internal Assembly DoResourceResolve(string name, Assembly requesting)
		{
			if (this.ResourceResolve == null)
			{
				return null;
			}
			Delegate[] invocationList = this.ResourceResolve.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Assembly assembly = ((ResolveEventHandler)invocationList[i])(this, new ResolveEventArgs(name, requesting));
				if (assembly != null)
				{
					return assembly;
				}
			}
			return null;
		}

		private void DoDomainUnload()
		{
			if (this.DomainUnload != null)
			{
				this.DomainUnload(this, null);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void DoUnhandledException(Exception e);

		internal void DoUnhandledException(UnhandledExceptionEventArgs args)
		{
			if (this.UnhandledException != null)
			{
				this.UnhandledException(this, args);
			}
		}

		internal byte[] GetMarshalledDomainObjRef()
		{
			return CADSerializer.SerializeObject(RemotingServices.Marshal(AppDomain.CurrentDomain, null, typeof(AppDomain))).GetBuffer();
		}

		internal void ProcessMessageInDomain(byte[] arrRequest, CADMethodCallMessage cadMsg, out byte[] arrResponse, out CADMethodReturnMessage cadMrm)
		{
			IMessage msg;
			if (arrRequest != null)
			{
				msg = CADSerializer.DeserializeMessage(new MemoryStream(arrRequest), null);
			}
			else
			{
				msg = new MethodCall(cadMsg);
			}
			IMessage message = ChannelServices.SyncDispatchMessage(msg);
			cadMrm = CADMethodReturnMessage.Create(message);
			if (cadMrm == null)
			{
				arrResponse = CADSerializer.SerializeMessage(message).GetBuffer();
				return;
			}
			arrResponse = null;
		}

		/// <summary>Occurs when an assembly is loaded.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event AssemblyLoadEventHandler AssemblyLoad;

		/// <summary>Occurs when the resolution of an assembly fails.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event ResolveEventHandler AssemblyResolve;

		/// <summary>Occurs when an <see cref="T:System.AppDomain" /> is about to be unloaded.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event EventHandler DomainUnload;

		/// <summary>Occurs when the default application domain's parent process exits.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event EventHandler ProcessExit;

		/// <summary>Occurs when the resolution of a resource fails because the resource is not a valid linked or embedded resource in the assembly.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event ResolveEventHandler ResourceResolve;

		/// <summary>Occurs when the resolution of a type fails.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event ResolveEventHandler TypeResolve;

		/// <summary>Occurs when an exception is not caught.</summary>
		[method: SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
		public event UnhandledExceptionEventHandler UnhandledException;

		/// <summary>Occurs when an exception is thrown in managed code, before the runtime searches the call stack for an exception handler in the application domain.</summary>
		public event EventHandler<FirstChanceExceptionEventArgs> FirstChanceException;

		/// <summary>Gets a value that indicates whether the current application domain has a set of permissions that is granted to all assemblies that are loaded into the application domain.</summary>
		/// <returns>
		///   <see langword="true" /> if the current application domain has a homogenous set of permissions; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsHomogenous
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets a value that indicates whether assemblies that are loaded into the current application domain execute with full trust.</summary>
		/// <returns>
		///   <see langword="true" /> if assemblies that are loaded into the current application domain execute with full trust; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsFullyTrusted
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the domain manager that was provided by the host when the application domain was initialized.</summary>
		/// <returns>An object that represents the domain manager provided by the host when the application domain was initialized, or <see langword="null" /> if no domain manager was provided.</returns>
		public AppDomainManager DomainManager
		{
			get
			{
				return this._domain_manager;
			}
		}

		/// <summary>Occurs when the resolution of an assembly fails in the reflection-only context.</summary>
		public event ResolveEventHandler ReflectionOnlyAssemblyResolve;

		/// <summary>Gets the activation context for the current application domain.</summary>
		/// <returns>An object that represents the activation context for the current application domain, or <see langword="null" /> if the domain has no activation context.</returns>
		public ActivationContext ActivationContext
		{
			get
			{
				return this._activation;
			}
		}

		/// <summary>Gets the identity of the application in the application domain.</summary>
		/// <returns>An object that identifies the application in the application domain.</returns>
		public ApplicationIdentity ApplicationIdentity
		{
			get
			{
				return this._applicationIdentity;
			}
		}

		/// <summary>Gets an integer that uniquely identifies the application domain within the process.</summary>
		/// <returns>An integer that identifies the application domain.</returns>
		public int Id
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return this.getDomainID();
			}
		}

		/// <summary>Returns the assembly display name after policy has been applied.</summary>
		/// <param name="assemblyName">The assembly display name, in the form provided by the <see cref="P:System.Reflection.Assembly.FullName" /> property.</param>
		/// <returns>A string containing the assembly display name after policy has been applied.</returns>
		[ComVisible(false)]
		[MonoTODO("This routine only returns the parameter currently")]
		public string ApplyPolicy(string assemblyName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (assemblyName.Length == 0)
			{
				throw new ArgumentException("assemblyName");
			}
			return assemblyName;
		}

		/// <summary>Creates a new application domain with the given name, using evidence, application base path, relative search path, and a parameter that specifies whether a shadow copy of an assembly is to be loaded into the application domain. Specifies a callback method that is invoked when the application domain is initialized, and an array of string arguments to pass the callback method.</summary>
		/// <param name="friendlyName">The friendly name of the domain. This friendly name can be displayed in user interfaces to identify the domain. For more information, see <see cref="P:System.AppDomain.FriendlyName" />.</param>
		/// <param name="securityInfo">Evidence that establishes the identity of the code that runs in the application domain. Pass <see langword="null" /> to use the evidence of the current application domain.</param>
		/// <param name="appBasePath">The base directory that the assembly resolver uses to probe for assemblies. For more information, see <see cref="P:System.AppDomain.BaseDirectory" />.</param>
		/// <param name="appRelativeSearchPath">The path relative to the base directory where the assembly resolver should probe for private assemblies. For more information, see <see cref="P:System.AppDomain.RelativeSearchPath" />.</param>
		/// <param name="shadowCopyFiles">
		///   <see langword="true" /> to load a shadow copy of an assembly into the application domain.</param>
		/// <param name="adInit">An <see cref="T:System.AppDomainInitializer" /> delegate that represents a callback method to invoke when the new <see cref="T:System.AppDomain" /> object is initialized.</param>
		/// <param name="adInitArgs">An array of string arguments to be passed to the callback represented by <paramref name="adInit" />, when the new <see cref="T:System.AppDomain" /> object is initialized.</param>
		/// <returns>The newly created application domain.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		public static AppDomain CreateDomain(string friendlyName, Evidence securityInfo, string appBasePath, string appRelativeSearchPath, bool shadowCopyFiles, AppDomainInitializer adInit, string[] adInitArgs)
		{
			AppDomainSetup appDomainSetup = AppDomain.CreateDomainSetup(appBasePath, appRelativeSearchPath, shadowCopyFiles);
			appDomainSetup.AppDomainInitializerArguments = adInitArgs;
			appDomainSetup.AppDomainInitializer = adInit;
			return AppDomain.CreateDomain(friendlyName, securityInfo, appDomainSetup);
		}

		/// <summary>Executes an assembly given its display name.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The assembly specified by <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly specified by <paramref name="assemblyName" /> is not a valid assembly.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly specified by <paramref name="assemblyName" /> was found, but could not be loaded.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		public int ExecuteAssemblyByName(string assemblyName)
		{
			return this.ExecuteAssemblyByName(assemblyName, null, null);
		}

		/// <summary>Executes an assembly given its display name, using the specified evidence.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="assemblySecurity">Evidence for loading the assembly.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The assembly specified by <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly specified by <paramref name="assemblyName" /> was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly specified by <paramref name="assemblyName" /> is not a valid assembly.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public int ExecuteAssemblyByName(string assemblyName, Evidence assemblySecurity)
		{
			return this.ExecuteAssemblyByName(assemblyName, assemblySecurity, null);
		}

		/// <summary>Executes the assembly given its display name, using the specified evidence and arguments.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="assemblySecurity">Evidence for loading the assembly.</param>
		/// <param name="args">Command-line arguments to pass when starting the process.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The assembly specified by <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly specified by <paramref name="assemblyName" /> was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly specified by <paramref name="assemblyName" /> is not a valid assembly.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="assemblySecurity" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="assemblySecurity" /> should be <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public int ExecuteAssemblyByName(string assemblyName, Evidence assemblySecurity, params string[] args)
		{
			Assembly a = Assembly.Load(assemblyName, assemblySecurity);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Executes the assembly given an <see cref="T:System.Reflection.AssemblyName" />, using the specified evidence and arguments.</summary>
		/// <param name="assemblyName">An <see cref="T:System.Reflection.AssemblyName" /> object representing the name of the assembly.</param>
		/// <param name="assemblySecurity">Evidence for loading the assembly.</param>
		/// <param name="args">Command-line arguments to pass when starting the process.</param>
		/// <returns>The value returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.IO.FileNotFoundException">The assembly specified by <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly specified by <paramref name="assemblyName" /> was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly specified by <paramref name="assemblyName" /> is not a valid assembly.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded and <paramref name="assemblyName" /> was compiled with a later version.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.NotSupportedException">
		///   <paramref name="assemblySecurity" /> is not <see langword="null" />. When legacy CAS policy is not enabled, <paramref name="assemblySecurity" /> should be <see langword="null" />.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		[Obsolete("Use an overload that does not take an Evidence parameter")]
		public int ExecuteAssemblyByName(AssemblyName assemblyName, Evidence assemblySecurity, params string[] args)
		{
			Assembly a = Assembly.Load(assemblyName, assemblySecurity);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Executes the assembly given its display name, using the specified arguments.</summary>
		/// <param name="assemblyName">The display name of the assembly. See <see cref="P:System.Reflection.Assembly.FullName" />.</param>
		/// <param name="args">Command-line arguments to pass when starting the process.</param>
		/// <returns>The value that is returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="assemblyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The assembly specified by <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly specified by <paramref name="assemblyName" /> was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly specified by <paramref name="assemblyName" /> is not a valid assembly.  
		///  -or-  
		///  <paramref name="assemblyName" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		public int ExecuteAssemblyByName(string assemblyName, params string[] args)
		{
			Assembly a = Assembly.Load(assemblyName, null);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Executes the assembly given an <see cref="T:System.Reflection.AssemblyName" />, using the specified arguments.</summary>
		/// <param name="assemblyName">An <see cref="T:System.Reflection.AssemblyName" /> object representing the name of the assembly.</param>
		/// <param name="args">Command-line arguments to pass when starting the process.</param>
		/// <returns>The value that is returned by the entry point of the assembly.</returns>
		/// <exception cref="T:System.IO.FileNotFoundException">The assembly specified by <paramref name="assemblyName" /> is not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly specified by <paramref name="assemblyName" /> was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly specified by <paramref name="assemblyName" /> is not a valid assembly.  
		///  -or-  
		///  <paramref name="assemblyName" /> was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		/// <exception cref="T:System.AppDomainUnloadedException">The operation is attempted on an unloaded application domain.</exception>
		/// <exception cref="T:System.MissingMethodException">The specified assembly has no entry point.</exception>
		public int ExecuteAssemblyByName(AssemblyName assemblyName, params string[] args)
		{
			Assembly a = Assembly.Load(assemblyName, null);
			return this.ExecuteAssemblyInternal(a, args);
		}

		/// <summary>Returns a value that indicates whether the application domain is the default application domain for the process.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.AppDomain" /> object represents the default application domain for the process; otherwise, <see langword="false" />.</returns>
		public bool IsDefaultAppDomain()
		{
			return this == AppDomain.DefaultDomain;
		}

		/// <summary>Returns the assemblies that have been loaded into the reflection-only context of the application domain.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.Assembly" /> objects that represent the assemblies loaded into the reflection-only context of the application domain.</returns>
		/// <exception cref="T:System.AppDomainUnloadedException">An operation is attempted on an unloaded application domain.</exception>
		public Assembly[] ReflectionOnlyGetAssemblies()
		{
			return this.GetAssemblies(true);
		}

		/// <summary>Maps a set of names to a corresponding set of dispatch identifiers.</summary>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="rgszNames">Passed-in array of names to be mapped.</param>
		/// <param name="cNames">Count of the names to be mapped.</param>
		/// <param name="lcid">The locale context in which to interpret the names.</param>
		/// <param name="rgDispId">Caller-allocated array which receives the IDs corresponding to the names.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _AppDomain.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		/// <summary>Retrieves the type information for an object, which can then be used to get the type information for an interface.</summary>
		/// <param name="iTInfo">The type information to return.</param>
		/// <param name="lcid">The locale identifier for the type information.</param>
		/// <param name="ppTInfo">Receives a pointer to the requested type information object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _AppDomain.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>Retrieves the number of type information interfaces that an object provides (either 0 or 1).</summary>
		/// <param name="pcTInfo">Points to a location that receives the number of type information interfaces provided by the object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _AppDomain.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>Provides access to properties and methods exposed by an object.</summary>
		/// <param name="dispIdMember">Identifies the member.</param>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="lcid">The locale context in which to interpret arguments.</param>
		/// <param name="wFlags">Flags describing the context of the call.</param>
		/// <param name="pDispParams">Pointer to a structure containing an array of arguments, an array of argument DISPIDs for named arguments, and counts for the number of elements in the arrays.</param>
		/// <param name="pVarResult">Pointer to the location where the result is to be stored.</param>
		/// <param name="pExcepInfo">Pointer to a structure that contains exception information.</param>
		/// <param name="puArgErr">The index of the first argument that has an error.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _AppDomain.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets a nullable Boolean value that indicates whether any compatibility switches are set, and if so, whether the specified compatibility switch is set.</summary>
		/// <param name="value">The compatibility switch to test.</param>
		/// <returns>A null reference (<see langword="Nothing" /> in Visual Basic) if no compatibility switches are set; otherwise, a Boolean value that indicates whether the compatibility switch that is specified by <paramref name="value" /> is set.</returns>
		public bool? IsCompatibilitySwitchSet(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return new bool?(this.compatibility_switch != null && this.compatibility_switch.Contains(value));
		}

		internal void SetCompatibilitySwitch(string value)
		{
			if (this.compatibility_switch == null)
			{
				this.compatibility_switch = new List<string>();
			}
			this.compatibility_switch.Add(value);
		}

		/// <summary>Gets or sets a value that indicates whether CPU and memory monitoring of application domains is enabled for the current process. Once monitoring is enabled for a process, it cannot be disabled.</summary>
		/// <returns>
		///   <see langword="true" /> if monitoring is enabled; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The current process attempted to assign the value <see langword="false" /> to this property.</exception>
		[MonoTODO("Currently always returns false")]
		public static bool MonitoringIsEnabled
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the number of bytes that survived the last collection and that are known to be referenced by the current application domain.</summary>
		/// <returns>The number of surviving bytes.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see langword="static" /> (<see langword="Shared" /> in Visual Basic) <see cref="P:System.AppDomain.MonitoringIsEnabled" /> property is set to <see langword="false" />.</exception>
		[MonoTODO]
		public long MonitoringSurvivedMemorySize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the total bytes that survived from the last collection for all application domains in the process.</summary>
		/// <returns>The total number of surviving bytes for the process.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see langword="static" /> (<see langword="Shared" /> in Visual Basic) <see cref="P:System.AppDomain.MonitoringIsEnabled" /> property is set to <see langword="false" />.</exception>
		[MonoTODO]
		public static long MonitoringSurvivedProcessMemorySize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the total size, in bytes, of all memory allocations that have been made by the application domain since it was created, without subtracting memory that has been collected.</summary>
		/// <returns>The total size of all memory allocations.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see langword="static" /> (<see langword="Shared" /> in Visual Basic) <see cref="P:System.AppDomain.MonitoringIsEnabled" /> property is set to <see langword="false" />.</exception>
		[MonoTODO]
		public long MonitoringTotalAllocatedMemorySize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the total processor time that has been used by all threads while executing in the current application domain, since the process started.</summary>
		/// <returns>Total processor time for the current application domain.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see langword="static" /> (<see langword="Shared" /> in Visual Basic) <see cref="P:System.AppDomain.MonitoringIsEnabled" /> property is set to <see langword="false" />.</exception>
		[MonoTODO]
		public TimeSpan MonitoringTotalProcessorTime
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private IntPtr _mono_app_domain;

		private static string _process_guid;

		[ThreadStatic]
		private static Dictionary<string, object> type_resolve_in_progress;

		[ThreadStatic]
		private static Dictionary<string, object> assembly_resolve_in_progress;

		[ThreadStatic]
		private static Dictionary<string, object> assembly_resolve_in_progress_refonly;

		private Evidence _evidence;

		private PermissionSet _granted;

		private PrincipalPolicy _principalPolicy;

		[ThreadStatic]
		private static IPrincipal _principal;

		private static AppDomain default_domain;

		private AppDomainManager _domain_manager;

		private ActivationContext _activation;

		private ApplicationIdentity _applicationIdentity;

		private List<string> compatibility_switch;

		[Serializable]
		private class Loader
		{
			public Loader(string assembly)
			{
				this.assembly = assembly;
			}

			public void Load()
			{
				Assembly.LoadFrom(this.assembly);
			}

			private string assembly;
		}

		[Serializable]
		private class Initializer
		{
			public Initializer(AppDomainInitializer initializer, string[] arguments)
			{
				this.initializer = initializer;
				this.arguments = arguments;
			}

			public void Initialize()
			{
				this.initializer(this.arguments);
			}

			private AppDomainInitializer initializer;

			private string[] arguments;
		}
	}
}
